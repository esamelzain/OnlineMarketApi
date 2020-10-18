using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineMarketApi.Models.RequestsResponse
{
    public class Roles : BaseResponse
    {
        public List<RRole> roles{ get; set; }
        public int count { get; set; }

    }
    public class RRole : BaseResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
