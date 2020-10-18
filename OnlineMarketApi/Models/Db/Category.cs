using System;
using System.Collections.Generic;

namespace OnlineMarketApi.Models.Db
{
    public partial class Category
    {
        public Category()
        {
            Product = new HashSet<Product>();
        }

        public string Id { get; set; }
        public string CategoryName { get; set; }
        public string CategoryImg { get; set; }

        public ICollection<Product> Product { get; set; }
    }
}
