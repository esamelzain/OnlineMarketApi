using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineMarketApi.Models.RequestsResponse
{
    public class Products : BaseResponse
    {
        public List<RProduct> products { get; set; }
        public int count { get; set; }

    }
    public class RProduct : BaseResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Img { get; set; }
        public DateTime ExpireDate { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public string CategoryId { get; set; }
        public ROffer ROffer { get; set; }

    }
}
