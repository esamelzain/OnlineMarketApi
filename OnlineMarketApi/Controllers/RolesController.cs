using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
    public class RolesController : Controller
    {
        private readonly OnlineMarketContext _context = new OnlineMarketContext();
        private readonly IHostingEnvironment _env;
        public RolesController(IHostingEnvironment env)
        {
            _env = env;
        }

        // GET: api/Roles
        [HttpPost]
        [Route("GetRoles")]
        public Roles GetRoles(PaginationRequest paginationRequest)
        {
            try
            {
                int skip = paginationRequest.Page * 9;
                var roles = _context.Role.Skip(skip).Take(9).ToList();
                List<RRole> rRoles = new List<RRole>();
                foreach (var role in roles)
                {
                    rRoles.Add(new RRole
                    {
                        Id = role.Id,
                        Name = role.Name,
                    });
                }
                return new Roles
                {
                    count = _context.Role.Count(),
                    roles = rRoles,
                    responseMessage = Helper.GetErrorMessage("200", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
            catch (Exception ex)
            {
                return new Roles
                {
                    responseMessage = Helper.GetErrorMessage("500", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
        }
        // GET: api/Roles/5
        [HttpPost]
        [Route("GetRole")]
        public async Task<RRole> GetRole(IdClass idClass)
        {
            if (!ModelState.IsValid)
            {
                return new RRole
                {
                    responseMessage = Helper.GetErrorMessage("406", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }

            var Role = await _context.Role.FindAsync(idClass.Id);

            if (Role == null)
            {
                return new RRole
                {
                    responseMessage = Helper.GetErrorMessage("402", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
            return new RRole
            {
                Id = Role.Id,
                Name = Role.Name,
                responseMessage = Helper.GetErrorMessage("200", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
            };
        }

        // PUT: api/Roles/5
        [HttpPost]
        [Route("EditRole")]
        public async Task<BaseResponse> EditRole(RRole rRole)
        {
            if (!ModelState.IsValid)
            {
                return new BaseResponse
                {
                    responseMessage = Helper.GetErrorMessage("406", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
            Role Role = new Role
            {
                Id = rRole.Id,
                Name = rRole.Name,
            };
            _context.Entry(Role).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleExists(rRole.Id))
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

            return new BaseResponse
            {
                responseMessage = Helper.GetErrorMessage("200", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
            };
        }

        // POST: api/Roles
        [HttpPost]
        [Route("AddRole")]
        public async Task<BaseResponse> AddRole(RRole rRole)
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
                Role Role = new Role
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = rRole.Name
                };
                _context.Role.Add(Role);
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

        // DELETE: api/Roles/5
        [HttpPost]
        [Route("DeleteRole")]
        public async Task<BaseResponse> DeleteRole(IdClass idClass)
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

                var Role = await _context.Role.FindAsync(idClass.Id);
                if (Role == null)
                {
                    return new BaseResponse
                    {
                        responseMessage = Helper.GetErrorMessage("402", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                    };
                }

                _context.Role.Remove(Role);
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
        private bool RoleExists(string id)
        {
            return _context.Role.Any(e => e.Id == id);
        }
    }
}