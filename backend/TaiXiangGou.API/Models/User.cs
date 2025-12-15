using SqlSugar;

namespace TaiXiangGou.API.Models
{
    /// <summary>
    /// 小程序用户表
    /// </summary>
    [SugarTable("users")]
    public class User
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "id")]
        public long Id { get; set; }

        /// <summary>
        /// 微信小程序 openid
        /// </summary>
        [SugarColumn(Length = 64, ColumnName = "openid")]
        public string OpenId { get; set; } = string.Empty;

        /// <summary>
        /// 昵称
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true, ColumnName = "nick_name")]
        public string? NickName { get; set; }

        /// <summary>
        /// 头像地址
        /// </summary>
        [SugarColumn(Length = 255, IsNullable = true, ColumnName = "avatar_url")]
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// 性别：0未知 1男 2女（与微信一致）
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnName = "gender")]
        public int? Gender { get; set; }

        [SugarColumn(Length = 50, IsNullable = true, ColumnName = "country")]
        public string? Country { get; set; }

        [SugarColumn(Length = 50, IsNullable = true, ColumnName = "province")]
        public string? Province { get; set; }

        [SugarColumn(Length = 50, IsNullable = true, ColumnName = "city")]
        public string? City { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "create_time")]
        public DateTime? CreateTime { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "update_time")]
        public DateTime? UpdateTime { get; set; }
    }
}


