using SqlSugar;

namespace TaiXiangGou.API.Models
{
    /// <summary>
    /// 收货地址
    /// </summary>
    [SugarTable("addresses")]
    public class Address
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "id")]
        public long Id { get; set; }

        [SugarColumn(ColumnName = "user_id")]
        public long UserId { get; set; }

        [SugarColumn(Length = 50, ColumnName = "name")]
        public string Name { get; set; } = string.Empty;

        [SugarColumn(Length = 20, ColumnName = "phone")]
        public string Phone { get; set; } = string.Empty;

        [SugarColumn(Length = 50, IsNullable = true, ColumnName = "province")]
        public string? Province { get; set; }

        [SugarColumn(Length = 50, IsNullable = true, ColumnName = "city")]
        public string? City { get; set; }

        [SugarColumn(Length = 50, IsNullable = true, ColumnName = "district")]
        public string? District { get; set; }

        [SugarColumn(Length = 255, ColumnName = "detail")]
        public string Detail { get; set; } = string.Empty;

        [SugarColumn(ColumnName = "is_default")]
        public bool IsDefault { get; set; } = false;

        [SugarColumn(IsNullable = true, ColumnName = "create_time")]
        public DateTime? CreateTime { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "update_time")]
        public DateTime? UpdateTime { get; set; }
    }
}


