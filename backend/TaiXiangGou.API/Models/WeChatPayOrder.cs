using SqlSugar;




namespace TaiXiangGou.API.Models
{
    [SugarTable("wechat_pay_orders")]
    public class WeChatPayOrder
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "id")]
        public long Id { get; set; }

        [SugarColumn(ColumnName = "order_id")]
        public long OrderId { get; set; }

        [SugarColumn(Length = 64, ColumnName = "out_trade_no")]
        public string OutTradeNo { get; set; } = string.Empty;

        [SugarColumn(Length = 128, IsNullable = true, ColumnName = "prepay_id")]
        public string? PrepayId { get; set; }

        [SugarColumn(Length = 128, IsNullable = true, ColumnName = "transaction_id")]
        public string? TransactionId { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "mchid")]
        public string? MchId { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "appid")]
        public string? AppId { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "payer_openid")]
        public string? PayerOpenId { get; set; }

        [SugarColumn(ColumnName = "amount")]
        public int Amount { get; set; } // 分

        [SugarColumn(Length = 10, ColumnName = "currency")]
        public string Currency { get; set; } = "CNY";

        [SugarColumn(Length = 32, ColumnName = "status")]
        public string Status { get; set; } = "created";

        [SugarColumn(IsNullable = true, ColumnName = "pay_time")]
        public DateTime? PayTime { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "notify_payload", ColumnDataType = "json")]
        public string? NotifyPayload { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "created_at")]
        public DateTime CreatedAt { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
