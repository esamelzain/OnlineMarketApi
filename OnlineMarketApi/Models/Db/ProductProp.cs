using System;
using System.Collections.Generic;

namespace OnlineMarketApi.Models.Db
{
    public partial class ProductProp
    {
        public int Id { get; set; }
        public string ProductId { get; set; }
        public int PropId { get; set; }
        public string Value { get; set; }

        public Product Product { get; set; }
        public Prop Prop { get; set; }
    }
}
