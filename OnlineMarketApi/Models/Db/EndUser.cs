using System;
using System.Collections.Generic;

namespace OnlineMarketApi.Models.Db
{
    public partial class EndUser
    {
        public EndUser()
        {
            Order = new HashSet<Order>();
        }

        public string Id { get; set; }
        public string UserName { get; set; }
        public string UserPhone { get; set; }
        public string UserImg { get; set; }
        public string UserPassword { get; set; }
        public bool IsActive { get; set; }

        public ICollection<Order> Order { get; set; }
    }
}
