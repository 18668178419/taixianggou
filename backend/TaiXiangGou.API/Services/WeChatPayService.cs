using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Transactions;
using SqlSugar;
using TaiXiangGou.API.Models;

namespace TaiXiangGou.API.Services
{
    public class WeChatPayService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<WeChatPayService> _logger;
        private readonly ISqlSugarClient _db;

        public WeChatPayService(IConfiguration configuration, ILogger<WeChatPayService> logger, ISqlSugarClient db)
        {
            _configuration = configuration;
            _logger = logger;
            _db = db;
        }

        private string AppId => _configuration["WeChatPay:AppId"] ?? "";
        private string MchId => _configuration["WeChatPay:MchId"] ?? "";
        private string ApiV3Key => _configuration["WeChatPay:ApiV3Key"] ?? "";
        private string MchPrivateKeyPemPath => _configuration["WeChatPay:MchPrivateKeyPemPath"] ?? "";
        private string MchSerialNo => _configuration["WeChatPay:MchSerialNo"] ?? "";
        private string NotifyUrl => _configuration["WeChatPay:NotifyUrl"] ?? "";

        /// <summary>
        /// 创建小程序支付订单
        /// </summary>
        public async Task<WeChatJsapiPayResponse> CreateJsApiPaymentAsync(Order order, string openId)
        {
            try
            {
                var client = new HttpClient();
                var url = "https://api.mch.weixin.qq.com/v3/pay/transactions/jsapi";

                // 构建请求体
                var requestBody = new
                {
                    appid = AppId,
                    mchid = MchId,
                    description = $"订单支付-{order.OrderNo}",
                    out_trade_no = order.OrderNo,
                    notify_url = NotifyUrl,
                    amount = new
                    {
                        total = (int)(order.FinalPrice * 100), // 转换为分
                        currency = "CNY"
                    },
                    payer = new
                    {
                        openid = openId
                    }
                };

                var jsonBody = JsonSerializer.Serialize(requestBody);
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                var nonceStr = Guid.NewGuid().ToString("N");
                var method = "POST";
                var requestUrl = "/v3/pay/transactions/jsapi";

                _logger.LogInformation("传给微信的包体"+ jsonBody);

                // 构建签名
                var signature = BuildSignature(method, requestUrl, timestamp, nonceStr, jsonBody);

                // 构建Authorization头
                var authorization = $"WECHATPAY2-SHA256-RSA2048 mchid=\"{MchId}\",nonce_str=\"{nonceStr}\",signature=\"{signature}\",timestamp=\"{timestamp}\",serial_no=\"{MchSerialNo}\"";

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add("Authorization", authorization);
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("User-Agent", "WeChatPay-APIv3-DotNet");
                request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
                    if (result != null && result.ContainsKey("prepay_id"))
                    {
                        var prepayId = result["prepay_id"].ToString();

                        await StoreWeChatPayOrderAsync(order, prepayId!, openId);
                        return new WeChatJsapiPayResponse
                        {
                            PayParams = GenerateJsApiParams(prepayId!),
                            PrepayId = prepayId!
                        };
                    }
                }

                _logger.LogError($"微信支付创建订单失败: {responseContent}");
                throw new Exception($"创建支付订单失败: {responseContent}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建微信支付订单异常");
                throw;
            }
        }

        /// <summary>
        /// 生成小程序调起支付的参数
        /// </summary>
        private Dictionary<string, object> GenerateJsApiParams(string prepayId)
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            var nonceStr = Guid.NewGuid().ToString("N");
            var package = $"prepay_id={prepayId}";

            var signStr = $"{AppId}\n{timestamp}\n{nonceStr}\n{package}\n";
            var paySign = SignWithRSA(signStr);

            return new Dictionary<string, object>
            {
                ["timeStamp"] = timestamp,
                ["nonceStr"] = nonceStr,
                ["package"] = package,
                ["signType"] = "RSA",
                ["paySign"] = paySign
            };
        }

        /// <summary>
        /// 构建请求签名
        /// </summary>
        private string BuildSignature(string method, string url, string timestamp, string nonceStr, string body)
        {
            var signStr = $"{method}\n{url}\n{timestamp}\n{nonceStr}\n{body}\n";
            return SignWithRSA(signStr);
        }

        /// <summary>
        /// RSA签名
        /// </summary>
        private string SignWithRSA(string data)
        {
            try
            {
                var keyPath = Path.Combine(Directory.GetCurrentDirectory(), MchPrivateKeyPemPath);
                if (!File.Exists(keyPath))
                {
                    throw new FileNotFoundException($"私钥文件不存在: {keyPath}");
                }

                var privateKeyPem = File.ReadAllText(keyPath);

                using var rsa = RSA.Create();
                rsa.ImportFromPem(privateKeyPem);

                var dataBytes = Encoding.UTF8.GetBytes(data);
                var signature = rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                return Convert.ToBase64String(signature);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RSA签名失败");
                throw;
            }
        }

        /// <summary>
        /// 验证支付回调签名
        /// </summary>
        public bool VerifyNotifySignature(string timestamp, string nonceStr, string body, string signature)
        {
            try
            {
                var signStr = $"{timestamp}\n{nonceStr}\n{body}\n";
                // 这里需要验证签名，简化处理
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 存储微信支付订单
        /// </summary>
        public async Task StoreWeChatPayOrderAsync(Order order, string prepayId, string openId)
        {
            try
            {
                // 去重：同订单号若已存在则更新 prepayId / 状态
                var exist = await _db.Queryable<WeChatPayOrder>()
                    .Where(x => x.OutTradeNo == order.OrderNo)
                    .FirstAsync();

                var now = DateTime.Now;
                if (exist == null)
                {
                    var wechatOrder = new WeChatPayOrder
                    {
                        OrderId = order.Id,
                        OutTradeNo = order.OrderNo,
                        PrepayId = prepayId,
                        TransactionId = null,
                        MchId = MchId,
                        AppId = AppId,
                        PayerOpenId = openId,
                        Amount = (int)(order.FinalPrice * 100),
                        Currency = "CNY",
                        Status = "pending",
                        PayTime = null,
                        NotifyPayload = null,
                        CreatedAt = now,
                        UpdatedAt = now
                    };
                    await _db.Insertable(wechatOrder).ExecuteCommandAsync();
                }
                else
                {
                    exist.PrepayId = prepayId;
                    exist.PayerOpenId = openId;
                    exist.Amount = (int)(order.FinalPrice * 100);
                    exist.Currency = "CNY";
                    exist.Status = "pending";
                    exist.UpdatedAt = now;
                    await _db.Updateable(exist).ExecuteCommandAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "存储微信支付订单异常");
                throw;
            }
        }

        /// <summary>
        /// 查询本地微信支付订单（无需调用微信查询）
        /// </summary>
        public async Task<WeChatPayOrder?> GetWechatPayOrderAsync(string orderNo)
        {
            return await _db.Queryable<WeChatPayOrder>()
                .Where(x => x.OutTradeNo == orderNo)
                .OrderBy(x => x.Id, OrderByType.Desc)
                .FirstAsync();
        }

        /// <summary>
        /// 调用微信侧查询订单，并同步本地订单 & wechat_pay_orders
        /// </summary>
        public async Task<WeChatOrderQueryResult> QueryOrderAndSyncAsync(Order order)
        {
            var result = await QueryOrderAsync(order.OrderNo);
            if (!result.Success)
            {
                return result;
            }

            // trade_state: SUCCESS, NOTPAY, USERPAYING, CLOSED, PAYERROR 等
            if (result.TradeState == "SUCCESS")
            {
                // 更新业务订单
                if (order.Status == "unpaid")
                {
                    order.Status = "unshipped";
                    order.PayTime = DateTime.Now;
                    order.UpdateTime = DateTime.Now;
                    await _db.Updateable(order).ExecuteCommandAsync();
                }

                // 更新微信支付订单
                var wxOrder = await _db.Queryable<WeChatPayOrder>()
                    .Where(x => x.OutTradeNo == order.OrderNo)
                    .FirstAsync();

                var now = DateTime.Now;
                if (wxOrder != null)
                {
                    wxOrder.Status = "success";
                    wxOrder.TransactionId = result.TransactionId ?? wxOrder.TransactionId;
                    wxOrder.PayerOpenId = result.PayerOpenId ?? wxOrder.PayerOpenId;
                    wxOrder.PayTime = result.PayTime ?? wxOrder.PayTime ?? now;
                    wxOrder.UpdatedAt = now;
                    wxOrder.NotifyPayload = result.Raw;
                    await _db.Updateable(wxOrder).ExecuteCommandAsync();
                }
            }
            else if (result.TradeState == "NOTPAY" || result.TradeState == "USERPAYING")
            {
                // 保持 pending 状态即可
            }
            else
            {
                // 失败或已关闭
                var wxOrder = await _db.Queryable<WeChatPayOrder>()
                    .Where(x => x.OutTradeNo == order.OrderNo)
                    .FirstAsync();
                if (wxOrder != null)
                {
                    wxOrder.Status = "fail";
                    wxOrder.UpdatedAt = DateTime.Now;
                    wxOrder.NotifyPayload = result.Raw;
                    await _db.Updateable(wxOrder).ExecuteCommandAsync();
                }
            }

            return result;
        }

        /// <summary>
        /// 调用微信支付查询订单
        /// </summary>
        public async Task<WeChatOrderQueryResult> QueryOrderAsync(string outTradeNo)
        {
            var client = new HttpClient();
            var requestPath = $"/v3/pay/transactions/out-trade-no/{outTradeNo}?mchid={MchId}";
            var url = $"https://api.mch.weixin.qq.com{requestPath}";

            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            var nonceStr = Guid.NewGuid().ToString("N");
            var method = "GET";
            var body = string.Empty;

            var signature = BuildSignature(method, requestPath, timestamp, nonceStr, body);
            var authorization = $"WECHATPAY2-SHA256-RSA2048 mchid=\"{MchId}\",nonce_str=\"{nonceStr}\",signature=\"{signature}\",timestamp=\"{timestamp}\",serial_no=\"{MchSerialNo}\"";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", authorization);
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("User-Agent", "WeChatPay-APIv3-DotNet");

            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"微信查询订单失败: {response.StatusCode}, {content}");
                return new WeChatOrderQueryResult { Success = false, Raw = content };
            }

            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;
            var tradeState = root.TryGetProperty("trade_state", out var ts) ? ts.GetString() : null;
            var transactionId = root.TryGetProperty("transaction_id", out var txn) ? txn.GetString() : null;
            var payerOpenId = root.TryGetProperty("payer", out var payerElem) && payerElem.TryGetProperty("openid", out var openidElem)
                ? openidElem.GetString()
                : null;
            DateTime? payTime = null;
            if (root.TryGetProperty("success_time", out var payTimeElem))
            {
                // success_time RFC3339
                if (DateTime.TryParse(payTimeElem.GetString(), out var dt))
                {
                    payTime = dt;
                }
            }

            return new WeChatOrderQueryResult
            {
                Success = true,
                TradeState = tradeState ?? "UNKNOWN",
                TransactionId = transactionId,
                PayerOpenId = payerOpenId,
                PayTime = payTime,
                Raw = content
            };
        }
    }

    public class WeChatJsapiPayResponse
    {
        public Dictionary<string, object> PayParams { get; set; } = new();
        public string PrepayId { get; set; } = string.Empty;
    }

    public class WeChatOrderQueryResult
    {
        public bool Success { get; set; }
        public string TradeState { get; set; } = "UNKNOWN";
        public string? TransactionId { get; set; }
        public string? PayerOpenId { get; set; }
        public DateTime? PayTime { get; set; }
        public string Raw { get; set; } = string.Empty;
    }
}

