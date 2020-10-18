using System;
using System.Collections.Generic;

namespace OnlineMarketApi.Models.Db
{
    public partial class User
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Img { get; set; }
        public string RoleId { get; set; }
        public string Token { get; set; }

        public Role Role { get; set; }
    }
}
