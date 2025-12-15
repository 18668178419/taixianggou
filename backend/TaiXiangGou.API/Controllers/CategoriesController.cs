using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using TaiXiangGou.API.Models;

namespace TaiXiangGou.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ISqlSugarClient _db;

        public CategoriesController(ISqlSugarClient db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] bool? status = null)
        {
            var query = _db.Queryable<Category>();
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
            var category = await _db.Queryable<Category>().Where(x => x.Id == id).FirstAsync();
            if (category == null)
            {
                return NotFound(new { code = 404, message = "分类不存在" });
            }
            return Ok(new { code = 200, data = category, message = "success" });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Category category)
        {
            category.CreateTime = DateTime.Now;
            category.UpdateTime = DateTime.Now;
            var id = await _db.Insertable(category).ExecuteReturnIdentityAsync();
            return Ok(new { code = 200, data = new { id }, message = "创建成功" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Category category)
        {
            var exist = await _db.Queryable<Category>().Where(x => x.Id == id).FirstAsync();
            if (exist == null)
            {
                return NotFound(new { code = 404, message = "分类不存在" });
            }
            category.Id = id;
            category.UpdateTime = DateTime.Now;
            await _db.Updateable(category).ExecuteCommandAsync();
            return Ok(new { code = 200, message = "更新成功" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var count = await _db.Deleteable<Category>().Where(x => x.Id == id).ExecuteCommandAsync();
            if (count == 0)
            {
                return NotFound(new { code = 404, message = "分类不存在" });
            }
            return Ok(new { code = 200, message = "删除成功" });
        }
    }
}

