using System;
using System.Collections.Generic;

namespace OnlineMarketApi.Models.Db
{
    public partial class Offer
    {
        public string Id { get; set; }
        public decimal OffPercentage { get; set; }
        public string ProductId { get; set; }
        public bool IsActive { get; set; }

        public Product Product { get; set; }
    }
}
