using OnlineMarketApi.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineMarketApi.Models.RequestsResponse
{
    public class RegisterRequest
    {
        public string UserName { get; set; }
        public string UserPhone { get; set; }
        public string UserImg { get; set; }
        public string UserPassword { get; set; }
    }
}
