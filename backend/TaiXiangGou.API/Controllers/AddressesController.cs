using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using TaiXiangGou.API.Models;

namespace TaiXiangGou.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddressesController : ControllerBase
    {
        private readonly ISqlSugarClient _db;

        public AddressesController(ISqlSugarClient db)
        {
            _db = db;
        }

        /// <summary>
        /// 地址列表
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetList(
            [FromQuery] long userId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            if (userId <= 0)
            {
                return BadRequest(new { code = 400, message = "userId不能为空" });
            }

            var query = _db.Queryable<Address>().Where(x => x.UserId == userId);

            var total = await query.CountAsync();
            var list = await query
                .OrderBy(x => x.IsDefault ? 0 : 1)
                .OrderBy(x => x.Id, OrderByType.Desc)
                .ToPageListAsync(page, pageSize);

            return Ok(new
            {
                code = 200,
                data = new
                {
                    list,
                    total,
                    page,
                    pageSize
                },
                message = "success"
            });
        }

        /// <summary>
        /// 获取单个地址
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var address = await _db.Queryable<Address>().Where(x => x.Id == id).FirstAsync();
            if (address == null)
            {
                return NotFound(new { code = 404, message = "地址不存在" });
            }

            return Ok(new { code = 200, data = address, message = "success" });
        }

        /// <summary>
        /// 获取默认地址
        /// </summary>
        [HttpGet("default")]
        public async Task<IActionResult> GetDefault([FromQuery] long userId)
        {
            if (userId <= 0)
            {
                return BadRequest(new { code = 400, message = "userId不能为空" });
            }

            var address = await _db.Queryable<Address>()
                .Where(x => x.UserId == userId && x.IsDefault)
                .OrderBy(x => x.Id, OrderByType.Desc)
                .FirstAsync();

            return Ok(new { code = 200, data = address, message = "success" });
        }

        /// <summary>
        /// 新增地址
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Address address)
        {
            if (address.UserId <= 0 || string.IsNullOrWhiteSpace(address.Name) || string.IsNullOrWhiteSpace(address.Phone))
            {
                return BadRequest(new { code = 400, message = "参数错误" });
            }

            var now = DateTime.Now;
            address.CreateTime = now;
            address.UpdateTime = now;

            if (address.IsDefault)
            {
                // 将该用户其他地址默认标记清除
                await _db.Updateable<Address>()
                    .SetColumns(x => new Address { IsDefault = false })
                    .Where(x => x.UserId == address.UserId)
                    .ExecuteCommandAsync();
            }

            var id = await _db.Insertable(address).ExecuteReturnBigIdentityAsync();

            return Ok(new
            {
                code = 200,
                data = new { id },
                message = "创建成功"
            });
        }

        /// <summary>
        /// 更新地址
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] Address address)
        {
            var exist = await _db.Queryable<Address>().Where(x => x.Id == id).FirstAsync();
            if (exist == null)
            {
                return NotFound(new { code = 404, message = "地址不存在" });
            }

            address.Id = id;
            address.UserId = exist.UserId; // 防止用户ID被篡改
            address.UpdateTime = DateTime.Now;

            if (address.IsDefault && !exist.IsDefault)
            {
                // 如果本次设置为默认地址，则清除该用户其他默认地址
                await _db.Updateable<Address>()
                    .SetColumns(x => new Address { IsDefault = false })
                    .Where(x => x.UserId == exist.UserId)
                    .ExecuteCommandAsync();
            }

            await _db.Updateable(address).ExecuteCommandAsync();

            return Ok(new { code = 200, message = "更新成功" });
        }

        /// <summary>
        /// 设置默认地址
        /// </summary>
        [HttpPut("{id}/default")]
        public async Task<IActionResult> SetDefault(long id)
        {
            var address = await _db.Queryable<Address>().Where(x => x.Id == id).FirstAsync();
            if (address == null)
            {
                return NotFound(new { code = 404, message = "地址不存在" });
            }

            var userId = address.UserId;

            // 取消该用户其他默认地址
            await _db.Updateable<Address>()
                .SetColumns(x => new Address { IsDefault = false })
                .Where(x => x.UserId == userId)
                .ExecuteCommandAsync();

            address.IsDefault = true;
            address.UpdateTime = DateTime.Now;
            await _db.Updateable(address).ExecuteCommandAsync();

            return Ok(new { code = 200, message = "设置成功" });
        }

        /// <summary>
        /// 删除地址
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var count = await _db.Deleteable<Address>().Where(x => x.Id == id).ExecuteCommandAsync();
            if (count == 0)
            {
                return NotFound(new { code = 404, message = "地址不存在" });
            }

            return Ok(new { code = 200, message = "删除成功" });
        }
    }
}


