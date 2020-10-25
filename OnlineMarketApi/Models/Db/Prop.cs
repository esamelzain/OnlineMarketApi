using System;
using System.Collections.Generic;

namespace OnlineMarketApi.Models.Db
{
    public partial class Prop
    {
        public Prop()
        {
            ProductProp = new HashSet<ProductProp>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<ProductProp> ProductProp { get; set; }
    }
}
