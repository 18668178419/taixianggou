using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SqlSugar;
using TaiXiangGou.API.Models;
using TaiXiangGou.API.Services;

namespace TaiXiangGou.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoodsController : ControllerBase
    {
        private readonly ISqlSugarClient _db;
        private readonly ILogger<OrdersController> _logger;

        public GoodsController(ISqlSugarClient db, ILogger<OrdersController> logger)
        {
            _db = db;
        
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetList(
            [FromQuery] int? categoryId = null,
            [FromQuery] string? keyword = null,
            [FromQuery] bool? isRecommend = null,
            [FromQuery] bool? isHot = null,
            [FromQuery] bool? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var baseQuery = _db.Queryable<Goods>();

          

            if (categoryId.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.CategoryId == categoryId.Value);
            }
            
            if (!string.IsNullOrEmpty(keyword))
            {
                baseQuery = baseQuery.Where(x => x.Name.Contains(keyword));
            }
            
            if (isRecommend.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.IsRecommend == isRecommend.Value);
            }
            
            if (isHot.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.IsHot == isHot.Value);
            }

            if (status.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.Status == status.Value);
            }
            else {
                baseQuery = baseQuery.Where(x => x.Status ==true);
            }

            var total = await baseQuery.Clone().CountAsync();
            var list = await baseQuery.Clone().OrderBy(x => x.Id, OrderByType.Desc)
                .ToPageListAsync(page, pageSize);

            // 处理JSON字段
            foreach (var item in list)
            {
                if (!string.IsNullOrEmpty(item.Images))
                {
                    try
                    {
                        item.Images = item.Images; // 保持JSON字符串
                    }
                    catch { }
                }
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
        public async Task<IActionResult> GetById(int id)
        {
            var goods = await _db.Queryable<Goods>().Where(x => x.Id == id).FirstAsync();
            if (goods == null)
            {
                return NotFound(new { code = 404, message = "商品不存在" });
            }
            return Ok(new { code = 200, data = goods, message = "success" });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Goods goods)
        {
            goods.CreateTime = DateTime.Now;
            goods.UpdateTime = DateTime.Now;
            var id = await _db.Insertable(goods).ExecuteReturnIdentityAsync();
            return Ok(new { code = 200, data = new { id }, message = "创建成功" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Goods goods)
        {
            var exist = await _db.Queryable<Goods>().Where(x => x.Id == id).FirstAsync();
            if (exist == null)
            {
                return NotFound(new { code = 404, message = "商品不存在" });
            }
            goods.Id = id;
            goods.UpdateTime = DateTime.Now;
            await _db.Updateable(goods).ExecuteCommandAsync();
            return Ok(new { code = 200, message = "更新成功" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var count = await _db.Deleteable<Goods>().Where(x => x.Id == id).ExecuteCommandAsync();
            if (count == 0)
            {
                return NotFound(new { code = 404, message = "商品不存在" });
            }
            return Ok(new { code = 200, message = "删除成功" });
        }
    }
}

