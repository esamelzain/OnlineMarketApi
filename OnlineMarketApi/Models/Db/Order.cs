using System;
using System.Collections.Generic;

namespace OnlineMarketApi.Models.Db
{
    public partial class Order
    {
        public Order()
        {
            OrderDetail = new HashSet<OrderDetail>();
        }

        public string Id { get; set; }
        public string UserId { get; set; }
        public decimal DeliveryPrice { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderTime { get; set; }
        public int OrderStatus { get; set; }
        public DateTime DeliveryTime { get; set; }
        public int DeliveryType { get; set; }
        public string DeliveryLocation { get; set; }

        public EndUser User { get; set; }
        public ICollection<OrderDetail> OrderDetail { get; set; }
    }
}
