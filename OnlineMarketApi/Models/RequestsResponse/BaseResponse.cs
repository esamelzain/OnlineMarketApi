using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineMarketApi.Models.RequestsResponse
{
    public class BaseResponse
    {
        public ErrorModel responseMessage { get; set; }
    }
    public class IdClass
    {
        public string Id { get; set; }
    }
}
