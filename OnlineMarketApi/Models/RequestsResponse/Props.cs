using OnlineMarketApi.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineMarketApi.Models.RequestsResponse
{
    public class RProps : BaseResponse
    {
        //List<Props>
        public int count { get; set; }
        public List<RProp> props { get; set; }
    }
    public class RProp : BaseResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class RPropRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class RProductProp
    {
        public int Id { get; set; }
        public string ProductId { get; set; }
        public int PropId { get; set; }
        public string Value { get; set; }
    }
}
