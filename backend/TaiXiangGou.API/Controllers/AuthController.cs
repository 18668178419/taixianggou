using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using TaiXiangGou.API.Models;

namespace TaiXiangGou.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ISqlSugarClient _db;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ISqlSugarClient db,
            ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _db = db;
            _logger = logger;
        }

        /// <summary>
        /// 通过code获取openid（原始接口，保留兼容）
        /// </summary>
        [HttpPost("getOpenId")]
        public async Task<IActionResult> GetOpenId([FromBody] GetOpenIdRequest request)
        {
            try
            {
                var wxResult = await GetWxSessionAsync(request.Code);
                if (!wxResult.Success)
                {
                    return BadRequest(new { code = 400, message = wxResult.ErrorMessage ?? "获取openid失败" });
                }

                return Ok(new
                {
                    code = 200,
                    data = new
                    {
                        openid = wxResult.OpenId
                    },
                    message = "success"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取openid异常");
                return StatusCode(500, new { code = 500, message = $"获取openid失败: {ex.Message}" });
            }
        }

        /// <summary>
        /// 小程序登录：code + 用户信息，创建/更新用户并返回用户ID
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Code))
                {
                    return BadRequest(new { code = 400, message = "code不能为空" });
                }

                var wxResult = await GetWxSessionAsync(request.Code);
                if (!wxResult.Success || string.IsNullOrEmpty(wxResult.OpenId))
                {
                    return BadRequest(new { code = 400, message = wxResult.ErrorMessage ?? "获取openid失败" });
                }

                var openId = wxResult.OpenId!;

                // 查询是否已有用户
                var user = await _db.Queryable<User>()
                    .Where(x => x.OpenId == openId)
                    .FirstAsync();

                var now = DateTime.Now;

                if (user == null)
                {
                    user = new User
                    {
                        OpenId = openId,
                        NickName = request.UserInfo?.NickName,
                        AvatarUrl = request.UserInfo?.AvatarUrl,
                        Gender = request.UserInfo?.Gender,
                        Country = request.UserInfo?.Country,
                        Province = request.UserInfo?.Province,
                        City = request.UserInfo?.City,
                        CreateTime = now,
                        UpdateTime = now
                    };

                    var newId = await _db.Insertable(user).ExecuteReturnBigIdentityAsync();
                    user.Id = newId;
                }
                else
                {
                    user.NickName = request.UserInfo?.NickName ?? user.NickName;
                    user.AvatarUrl = request.UserInfo?.AvatarUrl ?? user.AvatarUrl;
                    user.Gender = request.UserInfo?.Gender ?? user.Gender;
                    user.Country = request.UserInfo?.Country ?? user.Country;
                    user.Province = request.UserInfo?.Province ?? user.Province;
                    user.City = request.UserInfo?.City ?? user.City;
                    user.UpdateTime = now;

                    await _db.Updateable(user).ExecuteCommandAsync();
                }

                return Ok(new
                {
                    code = 200,
                    data = new
                    {
                        id = user.Id,
                        openid = user.OpenId,
                        nickName = user.NickName,
                        avatarUrl = user.AvatarUrl,
                        gender = user.Gender,
                        country = user.Country,
                        province = user.Province,
                        city = user.City
                    },
                    message = "success"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "登录失败");
                return StatusCode(500, new { code = 500, message = $"登录失败: {ex.Message}" });
            }
        }

        /// <summary>
        /// 调用微信 jscode2session 接口
        /// </summary>
        private async Task<WxSessionResult> GetWxSessionAsync(string code)
        {
            var appId = _configuration["ConnectionStrings:appid"] ?? "";
            var secret = _configuration["ConnectionStrings:secret"] ?? "";

            if (string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(secret))
            {
                return new WxSessionResult
                {
                    Success = false,
                    ErrorMessage = "微信配置错误"
                };
            }

            var client = _httpClientFactory.CreateClient();
            var url = $"https://api.weixin.qq.com/sns/jscode2session?appid={appId}&secret={secret}&js_code={code}&grant_type=authorization_code";

            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new WxSessionResult
                {
                    Success = false,
                    ErrorMessage = "调用微信接口失败"
                };
            }

            var result = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(content);
            if (result == null)
            {
                return new WxSessionResult
                {
                    Success = false,
                    ErrorMessage = "微信返回数据为空"
                };
            }

            if (result.ContainsKey("errcode"))
            {
                return new WxSessionResult
                {
                    Success = false,
                    ErrorMessage = result.ContainsKey("errmsg") ? result["errmsg"]?.ToString() : "微信接口错误"
                };
            }

            if (!result.ContainsKey("openid"))
            {
                return new WxSessionResult
                {
                    Success = false,
                    ErrorMessage = "未获取到openid"
                };
            }

            return new WxSessionResult
            {
                Success = true,
                OpenId = result["openid"]?.ToString()
            };
        }
    }

    public class GetOpenIdRequest
    {
        public string Code { get; set; } = "";
    }

    public class LoginRequest
    {
        public string Code { get; set; } = "";
        public LoginUserInfo? UserInfo { get; set; }
    }

    public class LoginUserInfo
    {
        public string? NickName { get; set; }
        public string? AvatarUrl { get; set; }
        public int? Gender { get; set; }
        public string? Country { get; set; }
        public string? Province { get; set; }
        public string? City { get; set; }
    }

    public class WxSessionResult
    {
        public bool Success { get; set; }
        public string? OpenId { get; set; }
        public string? ErrorMessage { get; set; }
    }
}

