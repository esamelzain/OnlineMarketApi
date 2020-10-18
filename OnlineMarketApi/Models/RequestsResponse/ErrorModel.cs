using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineMarketApi.Models.RequestsResponse
{
    public class ErrorModel
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorMessageAr { get; set; }
    }
}
