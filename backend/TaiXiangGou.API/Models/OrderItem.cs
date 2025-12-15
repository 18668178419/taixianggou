using SqlSugar;

namespace TaiXiangGou.API.Models
{
    [SugarTable("order_items")]
    public class OrderItem
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "id")]
        public long Id { get; set; }

        [SugarColumn(ColumnName = "order_id")]
        public long OrderId { get; set; }

        [SugarColumn(ColumnName = "goods_id")]
        public int GoodsId { get; set; }

        [SugarColumn(Length = 200, ColumnName = "goods_name")]
        public string GoodsName { get; set; } = string.Empty;

        [SugarColumn(Length = 500, IsNullable = true, ColumnName = "goods_image")]
        public string? GoodsImage { get; set; }

        [SugarColumn(DecimalDigits = 2, ColumnName = "goods_price")]
        public decimal GoodsPrice { get; set; }

        [SugarColumn(ColumnName = "count")]
        public int Count { get; set; }

        [SugarColumn(Length = 200, IsNullable = true, ColumnName = "specs")]
        public string? Specs { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "create_time")]
        public DateTime? CreateTime { get; set; }
    }
}

