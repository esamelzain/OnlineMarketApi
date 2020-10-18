using OnlineMarketApi.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineMarketApi.Models.RequestsResponse
{
    public class SearchEngine
    {
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }
    }
    public class Key
    {
        public string key { get; set; }
    }
    public class RSearchEngine : BaseResponse
    {
        public List<RRProduct> Products { get; set; }
        public List<RCategory> Categories { get; set; }
    }
    public class RRProduct : BaseResponse
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
