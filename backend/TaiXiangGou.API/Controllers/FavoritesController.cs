using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using TaiXiangGou.API.Models;

namespace TaiXiangGou.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoritesController : ControllerBase
    {
        private readonly ISqlSugarClient _db;

        public FavoritesController(ISqlSugarClient db)
        {
            _db = db;
        }

        /// <summary>
        /// 收藏列表
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetList(
            [FromQuery] long userId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            if (userId <= 0)
            {
                return BadRequest(new { code = 400, message = "userId不能为空" });
            }

            var query = _db.Queryable<Favorite>().Where(x => x.UserId == userId);

            var total = await query.CountAsync();
            var list = await query
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
        /// 检查是否已收藏
        /// </summary>
        [HttpGet("check")]
        public async Task<IActionResult> Check(
            [FromQuery] long userId,
            [FromQuery] int goodsId)
        {
            if (userId <= 0 || goodsId <= 0)
            {
                return BadRequest(new { code = 400, message = "参数错误" });
            }

            var favorite = await _db.Queryable<Favorite>()
                .Where(x => x.UserId == userId && x.GoodsId == goodsId)
                .FirstAsync();

            return Ok(new
            {
                code = 200,
                data = new
                {
                    isFavorite = favorite != null,
                    id = favorite?.Id
                },
                message = "success"
            });
        }

        /// <summary>
        /// 添加收藏
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Favorite favorite)
        {
            if (favorite.UserId <= 0 || favorite.GoodsId <= 0)
            {
                return BadRequest(new { code = 400, message = "参数错误" });
            }

            // 去重
            var exist = await _db.Queryable<Favorite>()
                .Where(x => x.UserId == favorite.UserId && x.GoodsId == favorite.GoodsId)
                .FirstAsync();

            if (exist != null)
            {
                return Ok(new
                {
                    code = 200,
                    data = new { id = exist.Id },
                    message = "已收藏"
                });
            }

            favorite.CreateTime = DateTime.Now;
            var id = await _db.Insertable(favorite).ExecuteReturnBigIdentityAsync();

            return Ok(new
            {
                code = 200,
                data = new { id },
                message = "收藏成功"
            });
        }

        /// <summary>
        /// 取消收藏（按主键ID）
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var count = await _db.Deleteable<Favorite>().Where(x => x.Id == id).ExecuteCommandAsync();
            if (count == 0)
            {
                return NotFound(new { code = 404, message = "记录不存在" });
            }

            return Ok(new { code = 200, message = "删除成功" });
        }

        /// <summary>
        /// 按用户和商品取消收藏
        /// </summary>
        [HttpDelete("by-goods")]
        public async Task<IActionResult> DeleteByGoods(
            [FromQuery] long userId,
            [FromQuery] int goodsId)
        {
            if (userId <= 0 || goodsId <= 0)
            {
                return BadRequest(new { code = 400, message = "参数错误" });
            }

            var count = await _db.Deleteable<Favorite>()
                .Where(x => x.UserId == userId && x.GoodsId == goodsId)
                .ExecuteCommandAsync();

            if (count == 0)
            {
                return NotFound(new { code = 404, message = "记录不存在" });
            }

            return Ok(new { code = 200, message = "删除成功" });
        }
    }
}


