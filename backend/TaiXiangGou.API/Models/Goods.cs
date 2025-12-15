using SqlSugar;

namespace TaiXiangGou.API.Models
{
    [SugarTable("goods")]
    public class Goods
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "id")]
        public int Id { get; set; }

        [SugarColumn(Length = 200, ColumnName = "name")]
        public string Name { get; set; } = string.Empty;

        [SugarColumn(IsNullable = true, ColumnDataType = "text", ColumnName = "description")]
        public string? Description { get; set; }

        [SugarColumn(Length = 500, IsNullable = true, ColumnName = "image")]
        public string? Image { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "text", ColumnName = "images")]
        public string? Images { get; set; }

        [SugarColumn(ColumnName = "category_id")]
        public int CategoryId { get; set; }

        [SugarColumn(DecimalDigits = 2, ColumnName = "price")]
        public decimal Price { get; set; }

        [SugarColumn(DecimalDigits = 2, IsNullable = true, ColumnName = "original_price")]
        public decimal? OriginalPrice { get; set; }

        [SugarColumn(ColumnName = "stock")]
        public int Stock { get; set; }

        [SugarColumn(ColumnName = "sales")]
        public int Sales { get; set; }

        [SugarColumn(ColumnName = "is_recommend")]
        public bool IsRecommend { get; set; }

        [SugarColumn(ColumnName = "is_hot")]
        public bool IsHot { get; set; }

        [SugarColumn(Length = 200, IsNullable = true, ColumnName = "tags")]
        public string? Tags { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "text", ColumnName = "specs")]
        public string? Specs { get; set; }

        [SugarColumn(ColumnName = "status")]
        public bool Status { get; set; } = true;

        [SugarColumn(IsNullable = true, ColumnName = "create_time")]
        public DateTime? CreateTime { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "update_time")]
        public DateTime? UpdateTime { get; set; }
    }
}

