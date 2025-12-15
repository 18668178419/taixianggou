using SqlSugar;

namespace TaiXiangGou.API.Models
{
    /// <summary>
    /// 商品收藏
    /// </summary>
    [SugarTable("favorites")]
    public class Favorite
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "id")]
        public long Id { get; set; }

        [SugarColumn(ColumnName = "user_id")]
        public long UserId { get; set; }

        [SugarColumn(ColumnName = "goods_id")]
        public int GoodsId { get; set; }

        [SugarColumn(Length = 200, IsNullable = true, ColumnName = "goods_name")]
        public string? GoodsName { get; set; }

        [SugarColumn(Length = 500, IsNullable = true, ColumnName = "goods_image")]
        public string? GoodsImage { get; set; }

        [SugarColumn(DecimalDigits = 2, ColumnName = "goods_price")]
        public decimal GoodsPrice { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "create_time")]
        public DateTime? CreateTime { get; set; }
    }
}


