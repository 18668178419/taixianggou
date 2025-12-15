using SqlSugar;

namespace TaiXiangGou.API.Models
{
    [SugarTable("banners")]
    public class Banner
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "id")]
        public int Id { get; set; }

        [SugarColumn(Length = 100, IsNullable = true, ColumnName = "title")]
        public string? Title { get; set; }

        [SugarColumn(Length = 500, ColumnName = "image")]
        public string Image { get; set; } = string.Empty;

        [SugarColumn(Length = 500, IsNullable = true, ColumnName = "link")]
        public string? Link { get; set; }

        // 假设数据库列为 sort_order
        [SugarColumn(ColumnName = "sort_order")]
        public int SortOrder { get; set; }

        // 假设数据库列为 status (tinyint/boolean)
        [SugarColumn(ColumnName = "status")]
        public bool Status { get; set; } = true;

        // 假设数据库列为 create_time
        [SugarColumn(IsNullable = true, ColumnName = "create_time")]
        public DateTime? CreateTime { get; set; }

        // 假设数据库列为 update_time
        [SugarColumn(IsNullable = true, ColumnName = "update_time")]
        public DateTime? UpdateTime { get; set; }

        // 示例：实体中有临时字段，不映射到数据库
        [SugarColumn(IsIgnore = true)]
        public string? TransientField { get; set; }
    }
}

