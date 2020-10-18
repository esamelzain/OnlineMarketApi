using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineMarketApi.Models.RequestsResponse
{
    public class Offers : BaseResponse
    {
        public List<ROffer> offers { get; set; }
        public int count { get; set; }

    }
    public class ROffer : BaseResponse
    {
        public string Id { get; set; }
        public decimal OffPercentage { get; set; }
        public string ProductId { get; set; }
        public RProduct Product { get; set; }
        public bool IsActive { get; set; }
    }
}
