using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineMarketApi.Handlers;
using OnlineMarketApi.Models.Db;
using OnlineMarketApi.Models.RequestsResponse;

namespace OnlineMarketApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly OnlineMarketContext _context = new OnlineMarketContext();
        private readonly IHostingEnvironment _env;
        public UsersController(IHostingEnvironment env)
        {
            _env = env;
        }

        [HttpPost]
        [Route("GetUsers")]
        public Users GetUsers(PaginationRequest paginationRequest)
        {
            try
            {
                int skip = paginationRequest.Page * 9;
                var users = _context.User.Skip(skip).Take(9).ToList();
                List<RUser> rusers = new List<RUser>();
                foreach (var user in users)
                {
                    rusers.Add(new RUser
                    {
                        Id = user.Id,
                        Img = user.Img,
                        UserName = user.UserName,
                        RoleId = user.RoleId,
                        Password = user.Password
                    });
                }
                return new Users
                {
                    count = _context.User.Count(),
                    users = rusers,
                    responseMessage = Helper.GetErrorMessage("200", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
            catch (Exception ex)
            {
                return new Users
                {
                    responseMessage = Helper.GetErrorMessage("500", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
        }

        [HttpPost]
        [Route("GetUser")]
        public async Task<RUser> GetUser(IdClass idClass)
        {
            if (!ModelState.IsValid)
            {
                return new RUser
                {
                    responseMessage = Helper.GetErrorMessage("406", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
            var user = await _context.User.FindAsync(idClass.Id);
            if (user == null)
            {
                return new RUser
                {
                    responseMessage = Helper.GetErrorMessage("402", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
            return new RUser
            {
                Img = user.Img,
                UserName = user.UserName,
                RoleId = user.RoleId,
                Password = user.Password,
                Id = user.Id,
                responseMessage = Helper.GetErrorMessage("200", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
            };
        }

        //[Authorize]
        [Route("AddUser")]
        [HttpPost]
        public async Task<BaseResponse> AddUser(RUser rUser)
        {
            try
            {
                var hash = Hash.Create(rUser.Password);
                User user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    Img = rUser.Img,
                    UserName = rUser.UserName,
                    RoleId = rUser.RoleId,
                    Password = hash
                };
                if (_context.User.Any(u => u.UserName == user.UserName))
                {
                    return new BaseResponse
                    {
                        responseMessage = Helper.GetErrorMessage("441", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                    };
                }
                _context.User.Add(user);
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

        [Route("EditUser")]
        [HttpPost]
        public async Task<BaseResponse> EditUser(RUser rUser)
        {
            try
            {
                Models.Db.User user = new User
                {
                    Img = rUser.Img,
                    UserName = rUser.UserName,
                    RoleId = rUser.RoleId,
                    Password = rUser.Password
                };
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return new BaseResponse
                {
                    responseMessage = Helper.GetErrorMessage("200", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(rUser.Id))
                {
                    return new BaseResponse
                    {
                        responseMessage = Helper.GetErrorMessage("402", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                    };
                }
                else
                {
                    return new BaseResponse
                    {
                        responseMessage = Helper.GetErrorMessage("500", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                    };
                }
            }
        }

        [HttpPost]
        [Route("DeleteUser")]
        public async Task<BaseResponse> DeleteUser(IdClass idClass)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new BaseResponse
                    {
                        responseMessage = Helper.GetErrorMessage("406", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                    };
                }
                var user = await _context.User.FindAsync(idClass.Id);
                if (user == null)
                {
                    return new BaseResponse
                    {
                        responseMessage = Helper.GetErrorMessage("402", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                    };
                }
                _context.User.Remove(user);
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
                var user = await _context.User.SingleOrDefaultAsync(u => u.UserName == loginRequest.UserName);
                if (user == null)
                {
                    return new LoginToken
                    {
                        responseMessage = Helper.GetErrorMessage("405", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                    };
                }
                var hash = Hash.Create(loginRequest.Password);
                if (hash != user.Password)
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
        private bool UserExists(string id)
        {
            return _context.Category.Any(e => e.Id == id);
        }


    }
}