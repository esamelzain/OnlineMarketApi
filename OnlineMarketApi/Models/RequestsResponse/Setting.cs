using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineMarketApi.Models.RequestsResponse
{
    public class Settings:BaseResponse
    {
        public List<RSetting> settings { get; set; }
        public int count { get; set; }

    }
    public class RSetting :BaseResponse
    {
        public string Setting { get; set; }
        public string Value { get; set; }
    }
}
