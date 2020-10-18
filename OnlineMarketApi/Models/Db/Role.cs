using System;
using System.Collections.Generic;

namespace OnlineMarketApi.Models.Db
{
    public partial class Role
    {
        public Role()
        {
            User = new HashSet<User>();
        }

        public string Id { get; set; }
        public string Name { get; set; }

        public ICollection<User> User { get; set; }
    }
}
