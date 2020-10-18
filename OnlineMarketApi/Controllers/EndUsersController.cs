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
                    UserPassword = hash
                };
                await _context.EndUser.AddAsync(user);
                await _context.SaveChangesAsync();
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
        public async Task<LoginToken> Login(LoginRequest loginRequest)
        {
            try
            {
                var user = await _context.EndUser.SingleOrDefaultAsync(u => u.UserName == loginRequest.UserName);
                if (user == null)
                {
                    return new LoginToken
                    {
                        responseMessage = Helper.GetErrorMessage("405", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                    };
                }
                var hash = Hash.Create(loginRequest.Password);
                if (hash != user.UserPassword)
                {
                    return new LoginToken
                    {
                        responseMessage = Helper.GetErrorMessage("404", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                    };
                }
                string token = Hash.Create(loginRequest.UserName + ":" + hash);
                //user.Token = token;
                await _context.SaveChangesAsync();
                return new LoginToken
                {
                    Token = token,
                    responseMessage = Helper.GetErrorMessage("200", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
            catch (Exception ex)
            {
                return new LoginToken
                {
                    responseMessage = Helper.GetErrorMessage("500", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
        }

        private bool UserExists(string name)
        {
            return _context.EndUser.Any(e => e.UserName == name);
        }
    }
}