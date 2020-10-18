using System;
using System.Collections.Generic;

namespace OnlineMarketApi.Models.Db
{
    public partial class OrderDetail
    {
        public string Id { get; set; }
        public string OrderId { get; set; }
        public decimal PaidPrice { get; set; }
        public bool? IsSpOffer { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }

        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}
