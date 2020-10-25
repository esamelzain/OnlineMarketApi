using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineMarketApi.Handlers;
using OnlineMarketApi.Helpers;
using OnlineMarketApi.Models.Db;
using OnlineMarketApi.Models.RequestsResponse;

namespace OnlineMarketApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class EndUsersController : ControllerBase
    {
        OnlineMarketContext _context = new OnlineMarketContext();
        private IHostingEnvironment _env;

        public EndUsersController(IHostingEnvironment env)
        {
            _env = env;
        }
        [HttpPost]
        [Route("Register")]
        public async Task<BaseResponse> Register(RegisterRequest registerRequest)
        {
            try
            {
                if (UserExists(registerRequest.UserName))
                {
                    return new BaseResponse
                    {
                        responseMessage = Helper.GetErrorMessage("441", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                    };
                }
                var hash = Hash.Create(registerRequest.UserPassword);
                EndUser user = new EndUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserImg = registerRequest.UserImg,
                    UserName = registerRequest.UserName,
                    UserPhone = registerRequest.UserPhone,
                    UserPassword = hash,
                    Email = registerRequest.Email,
                    ConfirmCode = new Random().Next(1, 9999),
                    IsConfirmed = false,
                };
                await _context.EndUser.AddAsync(user);
                await _context.SaveChangesAsync();
                var Settings = _context.Setting.ToList();
                Mail.Send(new List<string> { user.Email }, "VerifyCode", "Your Verification Code Is : " + user.ConfirmCode, Settings);
                return new BaseResponse
                {
                    responseMessage = Helper.GetErrorMessage("200", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    responseMessage = Helper.GetErrorMessage("500", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
        }
        [HttpPost]
        [Route("Login")]
        public async Task<EndLoginToken> Login(LoginRequest loginRequest)
        {
            try
            {
                var user = await _context.EndUser.SingleOrDefaultAsync(u => u.UserName == loginRequest.UserName);
                if (user == null)
                {
                    return new EndLoginToken
                    {
                        responseMessage = Helper.GetErrorMessage("405", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                    };
                }
                var hash = Hash.Create(loginRequest.Password);
                if (hash != user.UserPassword)
                {
                    return new EndLoginToken
                    {
                        responseMessage = Helper.GetErrorMessage("404", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                    };
                }
                string token = Hash.Create(loginRequest.UserName + ":" + hash);
                //user.Token = token;
                //await _context.SaveChangesAsync();
               
                return new EndLoginToken
                {
                    Token = token, 
                    UserInfo = new UserResponce
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        IsConfirmed = user.IsConfirmed==null?false:(bool)user.IsConfirmed,
                        Email = user.Email,
                        UserImg = user.UserImg,
                        UserPhone = user.UserPhone
                    },
                    responseMessage = Helper.GetErrorMessage("200", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
            catch (Exception ex)
            {
                return new EndLoginToken
                {
                    responseMessage = Helper.GetErrorMessage("500", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
        }
        [HttpPost]
        [Route("VerifyCode")]
        public VerifyCodeResponse VerifyCode(VerifyCodeRequest verifyCodeRequest)
        {
            try
            {
                var user = _context.EndUser.SingleOrDefault(u => u.UserName == verifyCodeRequest.UserName);
                if (user == null)
                {
                    return new VerifyCodeResponse
                    {
                        responseMessage = Helper.GetErrorMessage("508", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                    };
                }
                if (user.ConfirmCode == verifyCodeRequest.Code)
                {
                    user.IsConfirmed = true;
                    _context.Update(user);
                    _context.SaveChanges();
                    return new VerifyCodeResponse
                    {
                        IsVerified = user.IsConfirmed == null ? false : (bool)user.IsConfirmed,
                        responseMessage = Helper.GetErrorMessage("200", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                    };
                }
                else
                {
                    return new VerifyCodeResponse
                    {
                        responseMessage = Helper.GetErrorMessage("509", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                    };
                }
            }
            catch (Exception ex)
            {
                return new VerifyCodeResponse
                {
                    responseMessage = Helper.GetErrorMessage("500", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
        }
        [HttpPost]
        [Route("ResendCode")]
        public VerifyCodeResponse ResendCode(VerifyCodeRequest verifyCodeRequest)
        {
            try
            {
                var user = _context.EndUser.SingleOrDefault(u => u.UserName == verifyCodeRequest.UserName);
                var Settings = _context.Setting.ToList();
                Mail.Send(new List<string> { user.Email }, "VerifyCode","Your Verification Code Is : "+user.ConfirmCode, Settings);
                return new VerifyCodeResponse
                {
                    IsVerified = (user.IsConfirmed == null) ? false : (bool)user.IsConfirmed,
                    responseMessage = Helper.GetErrorMessage("200", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
            catch (Exception ex)
            {
                return new VerifyCodeResponse
                {
                    responseMessage = Helper.GetErrorMessage("500", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
        }
        [HttpPost]
        [Route("ChangePassword")]
        public BaseResponse ChangePassword(ChangePassword changePassword)
        {
            try
            {
                var oldHash = Hash.Create(changePassword.OldPassword);
                var newHash = Hash.Create(changePassword.NewPassword);
                var user = _context.EndUser.SingleOrDefault(c => c.Id == changePassword.Id);
                if (user == null)
                {
                    return new BaseResponse
                    {
                        responseMessage = Helper.GetErrorMessage("405", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                    };
                }
                if (oldHash != user.UserPassword)
                {
                    return new BaseResponse
                    {
                        responseMessage = Helper.GetErrorMessage("404", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                    };
                }
                user.UserPassword = newHash;
                _context.EndUser.Update(user);
                _context.SaveChanges();
                return new BaseResponse
                {
                    responseMessage = Helper.GetErrorMessage("200", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    responseMessage = Helper.GetErrorMessage("200", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
        }
        [HttpPost]
        [Route("ResetPassword")]
        public BaseResponse ResetPassword(ResetPassword resetPassword)
        {
            try
            {
                var user = _context.EndUser.SingleOrDefault(c => c.UserName == resetPassword.UserName);
                if (user == null)
                {
                    return new BaseResponse
                    {
                        responseMessage = Helper.GetErrorMessage("405", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                    };
                }
                var newHash = Hash.Create(resetPassword.NewPassword);
                user.UserPassword = newHash;
                _context.EndUser.Update(user);
                _context.SaveChanges();
                return new BaseResponse
                {
                    responseMessage = Helper.GetErrorMessage("200", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    responseMessage = Helper.GetErrorMessage("200", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
        }
        private bool UserExists(string name)
        {
            return _context.EndUser.Any(e => e.UserName == name);
        }
    }
}