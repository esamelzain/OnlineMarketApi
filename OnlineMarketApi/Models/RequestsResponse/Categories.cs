using OnlineMarketApi.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineMarketApi.Models.RequestsResponse
{
    public class Categories : BaseResponse
    {
        public List<RCategory> categories { get; set; }
        public int count { get; set; }
    }
    public class RCategory : BaseResponse
    {
        public string Id { get; set; }
        public string CategoryName { get; set; }
        public string CategoryImg { get; set; }
        public List<RProduct> RProducts { get; set; }
    }
}
