using SqlSugar;

namespace TaiXiangGou.API.Models
{
    [SugarTable("categories")]
    public class Category
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "id")]
        public int Id { get; set; }

        [SugarColumn(Length = 50, ColumnName = "name")]
        public string Name { get; set; } = string.Empty;

        [SugarColumn(Length = 10, IsNullable = true, ColumnName = "icon")]
        public string? Icon { get; set; }

        [SugarColumn(Length = 500, IsNullable = true, ColumnName = "image")]
        public string? Image { get; set; }

        [SugarColumn(ColumnName = "sort_order")]
        public int SortOrder { get; set; }

        [SugarColumn(ColumnName = "status")]
        public bool Status { get; set; } = true;

        [SugarColumn(IsNullable = true, ColumnName = "create_time")]
        public DateTime? CreateTime { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "update_time")]
        public DateTime? UpdateTime { get; set; }
    }
}

