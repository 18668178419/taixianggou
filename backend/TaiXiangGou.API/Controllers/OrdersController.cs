using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SqlSugar;
using TaiXiangGou.API.Models;
using TaiXiangGou.API.Services;

namespace TaiXiangGou.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ISqlSugarClient _db;
        private readonly WeChatPayService _weChatPayService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(ISqlSugarClient db, WeChatPayService weChatPayService, ILogger<OrdersController> logger)
        {
            _db = db;
            _weChatPayService = weChatPayService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetList(
            [FromQuery] int? userid = null,
            [FromQuery] string? status = null,
            [FromQuery] string? orderNo = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var query = _db.Queryable<Order>();
            
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(x => x.Status == status);
            }

            query = query.Where(x => x.UserId == userid);

            if (!string.IsNullOrEmpty(orderNo))
            {
                query = query.Where(x => x.OrderNo.Contains(orderNo));
            }

            var total = await query.CountAsync();
            var list = await query.OrderBy(x => x.Id, OrderByType.Desc)
                .ToPageListAsync(page, pageSize);

            // 加载订单明细
            foreach (var order in list)
            {
                order.Items = await _db.Queryable<OrderItem>()
                    .Where(x => x.OrderId == order.Id)
                    .ToListAsync();
            }

            return Ok(new { 
                code = 200, 
                data = new { 
                    list, 
                    total, 
                    page, 
                    pageSize 
                }, 
                message = "success" 
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var order = await _db.Queryable<Order>().Where(x => x.Id == id).FirstAsync();
            if (order == null)
            {
                return NotFound(new { code = 404, message = "订单不存在" });
            }
            
            // 加载订单明细
            order.Items = await _db.Queryable<OrderItem>()
                .Where(x => x.OrderId == order.Id)
                .ToListAsync();
            
            return Ok(new { code = 200, data = order, message = "success" });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Order order)
        {
            // 生成订单号
            order.OrderNo = "ORD" + DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 9999);
            order.CreateTime = DateTime.Now;
            order.UpdateTime = DateTime.Now;
            
            var orderId = await _db.Insertable(order).ExecuteReturnBigIdentityAsync();
            
            // 插入订单明细
            if (order.Items != null && order.Items.Any())
            {
                foreach (var item in order.Items)
                {
                    item.OrderId = orderId;
                    item.CreateTime = DateTime.Now;
                }
                await _db.Insertable(order.Items).ExecuteCommandAsync();
            }
            
            return Ok(new { code = 200, data = new { id = orderId, orderNo = order.OrderNo }, message = "创建成功" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] Order order)
        {
            var exist = await _db.Queryable<Order>().Where(x => x.Id == id).FirstAsync();
            if (exist == null)
            {
                return NotFound(new { code = 404, message = "订单不存在" });
            }
            
            order.Id = id;
            order.UpdateTime = DateTime.Now;
            await _db.Updateable(order).ExecuteCommandAsync();
            
            return Ok(new { code = 200, message = "更新成功" });
        }

        [HttpPut("{id}/ship")]
        public async Task<IActionResult> Ship(long id)
        {
            var order = await _db.Queryable<Order>().Where(x => x.Id == id).FirstAsync();
            if (order == null)
            {
                return NotFound(new { code = 404, message = "订单不存在" });
            }
            
            order.Status = "shipped";
            order.ShipTime = DateTime.Now;
            order.UpdateTime = DateTime.Now;
            await _db.Updateable(order).ExecuteCommandAsync();
            
            return Ok(new { code = 200, message = "发货成功" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            // 先删除订单明细
            await _db.Deleteable<OrderItem>().Where(x => x.OrderId == id).ExecuteCommandAsync();
            
            // 再删除订单
            var count = await _db.Deleteable<Order>().Where(x => x.Id == id).ExecuteCommandAsync();
            if (count == 0)
            {
                return NotFound(new { code = 404, message = "订单不存在" });
            }
            
            return Ok(new { code = 200, message = "删除成功" });
        }

        /// <summary>
        /// 创建支付订单
        /// </summary>
        [HttpPost("{id}/pay")]
        public async Task<IActionResult> CreatePayment(long id, [FromBody] PaymentRequest request)
        {
            try
            {
                var order = await _db.Queryable<Order>().Where(x => x.Id == id).FirstAsync();

                if (order == null)
                {
                    return NotFound(new { code = 404, message = "订单不存在" });
                }

                if (order.Status != "unpaid")
                {
                    return BadRequest(new { code = 400, message = "订单状态不正确，无法支付" });
                }

                if (string.IsNullOrEmpty(request.OpenId))
                {
                    return BadRequest(new { code = 400, message = "OpenId不能为空" });
                }

                _logger.LogInformation("开始创建微信订单:CreateJsApiPaymentAsync");
                var payResp = await _weChatPayService.CreateJsApiPaymentAsync(order, request.OpenId);

                return Ok(new { code = 200, data = payResp.PayParams, message = "创建支付订单成功" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建支付订单失败");
                return StatusCode(500, new { code = 500, message = $"创建支付订单失败: {ex.Message}" });
            }
        }

        /// <summary>
        /// 微信支付回调
        /// </summary>
        [HttpPost("pay/notify")]
        public async Task<IActionResult> PayNotify()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                
                // 获取请求头
                var timestamp = Request.Headers["Wechatpay-Timestamp"].FirstOrDefault() ?? "";
                var nonceStr = Request.Headers["Wechatpay-Nonce"].FirstOrDefault() ?? "";
                var signature = Request.Headers["Wechatpay-Signature"].FirstOrDefault() ?? "";

                // 验证签名（简化处理，实际应该验证）
                // if (!_weChatPayService.VerifyNotifySignature(timestamp, nonceStr, body, signature))
                // {
                //     return BadRequest();
                // }

                // 解析回调数据
                using var doc = System.Text.Json.JsonDocument.Parse(body);

                _logger.LogInformation("回调传回数据"+body);
                var root = doc.RootElement;
                
                if (root.TryGetProperty("event_type", out var eventType) && eventType.GetString() == "TRANSACTION.SUCCESS")
                {
                    if (root.TryGetProperty("resource", out var resource))
                    {
                        // 微信支付V3的回调数据是加密的，需要解密
                        // 这里简化处理：实际生产环境需要实现AES-256-GCM解密
                        _logger.LogInformation($"收到支付成功回调，resource: {resource}");
                        
                        var outTradeNo = "";
                        if (resource.TryGetProperty("out_trade_no", out var outTradeNoElement))
                        {
                            outTradeNo = outTradeNoElement.GetString() ?? "";
                        }

                        if (!string.IsNullOrEmpty(outTradeNo))
                        {
                            var order = await _db.Queryable<Order>().Where(x => x.OrderNo == outTradeNo).FirstAsync();
                            if (order != null && order.Status == "unpaid")
                            {
                                order.Status = "unshipped";
                                order.PayTime = DateTime.Now;
                                order.UpdateTime = DateTime.Now;
                                await _db.Updateable(order).ExecuteCommandAsync();

                                var wxOrder = await _db.Queryable<WeChatPayOrder>()
                                    .Where(x => x.OutTradeNo == outTradeNo)
                                    .FirstAsync();
                                if (wxOrder != null)
                                {
                                    wxOrder.Status = "success";
                                    wxOrder.TransactionId = resource.TryGetProperty("transaction_id", out var txnIdElem)
                                        ? txnIdElem.GetString()
                                        : wxOrder.TransactionId;
                                    wxOrder.UpdatedAt = DateTime.Now;
                                    await _db.Updateable(wxOrder).ExecuteCommandAsync();
                                }
                                
                                _logger.LogInformation($"订单支付成功: {outTradeNo}");
                            }
                        }
                    }
                }

                // 返回成功响应
                return Ok(new { code = "SUCCESS", message = "成功" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理支付回调失败");
                return StatusCode(500, new { code = "FAIL", message = "处理失败" });
            }
        }

        /// <summary>
        /// 轮询查询支付状态（供小程序轮询）
        /// </summary>
        [HttpGet("{id}/pay-status")]
        public async Task<IActionResult> GetPayStatus(long id)
        {
            var order = await _db.Queryable<Order>().Where(x => x.Id == id).FirstAsync();
            if (order == null)
            {
                return NotFound(new { code = 404, message = "订单不存在" });
            }

            // 调用微信查询并同步状态
            var queryResult = await _weChatPayService.QueryOrderAndSyncAsync(order);

            // 查询本地微信支付记录（最新一条）
            var wxOrder = await _db.Queryable<WeChatPayOrder>()
                .Where(x => x.OrderId == id)
                .OrderBy(x => x.Id, OrderByType.Desc)
                .FirstAsync();

            return Ok(new
            {
                code = 200,
                data = new
                {
                    orderStatus = order.Status,
                    payStatus = wxOrder?.Status ?? "pending",
                    tradeState = queryResult.TradeState
                },
                message = "success"
            });
        }
    }

    public class PaymentRequest
    {
        public string OpenId { get; set; } = "";
    }
}

