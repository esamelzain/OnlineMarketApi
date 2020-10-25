using OnlineMarketApi.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineMarketApi.Models.RequestsResponse
{
    public class RegisterRequest
    {
        public string UserName { get; set; }
        public string UserPhone { get; set; }
        public string UserImg { get; set; }
        public string UserPassword { get; set; }
        public string Email { get; set; }
    }
    public class ChangePassword
    {
        public string Id { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
    public class ResetPassword
    {
        public string UserName { get; set; }
        public string NewPassword { get; set; }
    }
    public class UserResponce
    {
        public string Id { get; set; }
        public int ConfirmCode { get; set; }
        public string UserName { get; set; }
        public string UserPhone { get; set; }
        public string UserImg { get; set; }
        public string UserPassword { get; set; }
        public string Email { get; set; }
        public bool IsConfirmed { get; set; }
    }
    public class EndLoginToken : BaseResponse
    {
        public string Token { get; set; }
        public UserResponce UserInfo { get; set; }
    }
    public class VerifyCodeRequest 
    {
        public string UserName { get; set; }
        public int Code { get; set; }
    }
    public class VerifyCodeResponse : BaseResponse
    {
        public bool IsVerified { get; set; }
    }
}
