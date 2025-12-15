using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using TaiXiangGou.API.Models;

namespace TaiXiangGou.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BannersController : ControllerBase
    {
        private readonly ISqlSugarClient _db;

        public BannersController(ISqlSugarClient db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] bool? status = null)
        {
            var query = _db.Queryable<Banner>();
            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }
            var list = await query.OrderBy(x => x.SortOrder).ToListAsync();
            return Ok(new { code = 200, data = list, message = "success" });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var banner = await _db.Queryable<Banner>().Where(x => x.Id == id).FirstAsync();
            if (banner == null)
            {
                return NotFound(new { code = 404, message = "轮播图不存在" });
            }
            return Ok(new { code = 200, data = banner, message = "success" });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Banner banner)
        {
            banner.CreateTime = DateTime.Now;
            banner.UpdateTime = DateTime.Now;
            var id = await _db.Insertable(banner).ExecuteReturnIdentityAsync();
            return Ok(new { code = 200, data = new { id }, message = "创建成功" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Banner banner)
        {
            var exist = await _db.Queryable<Banner>().Where(x => x.Id == id).FirstAsync();
            if (exist == null)
            {
                return NotFound(new { code = 404, message = "轮播图不存在" });
            }
            banner.Id = id;
            banner.UpdateTime = DateTime.Now;
            await _db.Updateable(banner).ExecuteCommandAsync();
            return Ok(new { code = 200, message = "更新成功" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var count = await _db.Deleteable<Banner>().Where(x => x.Id == id).ExecuteCommandAsync();
            if (count == 0)
            {
                return NotFound(new { code = 404, message = "轮播图不存在" });
            }
            return Ok(new { code = 200, message = "删除成功" });
        }
    }
}

