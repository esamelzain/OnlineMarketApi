using System;
using System.Collections.Generic;

namespace OnlineMarketApi.Models.Db
{
    public partial class Product
    {
        public Product()
        {
            Offer = new HashSet<Offer>();
            OrderDetail = new HashSet<OrderDetail>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Img { get; set; }
        public DateTime ExpireDate { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public string CategoryId { get; set; }

        public Category Category { get; set; }
        public ICollection<Offer> Offer { get; set; }
        public ICollection<OrderDetail> OrderDetail { get; set; }
    }
}
