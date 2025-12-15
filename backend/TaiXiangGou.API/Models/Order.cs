using SqlSugar;

namespace TaiXiangGou.API.Models
{
    [SugarTable("orders")]
    public class Order
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "id")]
        public long Id { get; set; }

        [SugarColumn(Length = 50, ColumnName = "order_no")]
        public string OrderNo { get; set; } = string.Empty;

        // 用户ID，关联 users.id
        [SugarColumn(IsNullable = true, ColumnName = "user_id")]
        public long? UserId { get; set; }

        [SugarColumn(Length = 50, IsNullable = true, ColumnName = "user_name")]
        public string? UserName { get; set; }

        [SugarColumn(Length = 20, IsNullable = true, ColumnName = "user_phone")]
        public string? UserPhone { get; set; }

        [SugarColumn(Length = 500, IsNullable = true, ColumnName = "address")]
        public string? Address { get; set; }

        [SugarColumn(DecimalDigits = 2, ColumnName = "total_price")]
        public decimal TotalPrice { get; set; }

        [SugarColumn(DecimalDigits = 2, ColumnName = "shipping_fee")]
        public decimal ShippingFee { get; set; }

        [SugarColumn(DecimalDigits = 2, ColumnName = "final_price")]
        public decimal FinalPrice { get; set; }

        [SugarColumn(Length = 500, IsNullable = true, ColumnName = "remark")]
        public string? Remark { get; set; }

        [SugarColumn(Length = 20, ColumnName = "status")]
        public string Status { get; set; } = "unpaid";

        [SugarColumn(Length = 20, IsNullable = true, ColumnName = "ship_user")]
        public string? ShipUser { get; set; }
        [SugarColumn(Length = 20, IsNullable = true, ColumnName = "tjr")]
        public string? Tjr { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "pay_time")]
        public DateTime? PayTime { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "ship_time")]
        public DateTime? ShipTime { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "receive_time")]
        public DateTime? ReceiveTime { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "create_time")]
        public DateTime? CreateTime { get; set; }

        [SugarColumn(IsNullable = true, ColumnName = "update_time")]
       
        public DateTime? UpdateTime { get; set; }

        [SugarColumn(IsIgnore = true)]
        public List<OrderItem>? Items { get; set; }
    }
}

