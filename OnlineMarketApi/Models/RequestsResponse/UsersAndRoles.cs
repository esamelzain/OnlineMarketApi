using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineMarketApi.Models.RequestsResponse
{
    public class Users:BaseResponse
    {
        public List<RUser> users { get; set; }
        public int count { get; set; }

    }
    public class RUser:BaseResponse
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Img { get; set; }
        public string RoleId { get; set; }
    }
    public class LoginToken : BaseResponse
    {
        public string Token { get; set; }
    }

    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
