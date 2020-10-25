using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using OnlineMarketApi.Handlers;
using OnlineMarketApi.Models.Db;
using OnlineMarketApi.Models.RequestsResponse;

namespace OnlineMarketApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProperitiesController : ControllerBase
    {
        private readonly OnlineMarketContext _context = new OnlineMarketContext();
        private readonly IHostingEnvironment _env;

        public ProperitiesController(IHostingEnvironment env)
        {
            _env = env;
        }
        // GET: api/GetAllProperities
        [HttpPost]
        [Route("GetAllProperities")]
        public RProps GetAllProperities()
        {
            try
            {
                // int skip = paginationRequest.Page * 10;
                var props = _context.Prop.ToList();
                List<RProp> Rprops = new List<RProp>();
                foreach (var prop in props)
                {
                    Rprops.Add(new  RProp
                    {
                        Id = prop.Id,
                        Name = prop.Name
                                            });
                }
                return new RProps
                {
                    count = _context.Prop.Count(),
                    props = Rprops,
                    responseMessage = Helper.GetErrorMessage("200", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
            catch (Exception ex)
            {
                return new RProps
                {
                    responseMessage = Helper.GetErrorMessage("500", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
        }
        // GET: api/Properities
        [HttpPost]
        [Route("GetProperities")]
        public RProps GetProperities(PaginationRequest paginationRequest)
        {
            try
            {
                int skip = paginationRequest.Page * 9;
                var props = _context.Prop.Skip(skip).Take(9).ToList();
                List<RProp> RProps = new List<RProp>();
                foreach (var prop in props)
                {
                    RProps.Add(new RProp
                    {
                        Id = prop.Id,
                        Name = prop.Name,
                    });
                }
                return new RProps
                {
                    count = _context.Prop.Count(),
                    props = RProps,
                    responseMessage = Helper.GetErrorMessage("200", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
            catch (Exception ex)
            {
                return new RProps
                {
                    responseMessage = Helper.GetErrorMessage("500", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
        }
        // GET: api/property/5
        [HttpPost]
        [Route("GetProperty")]
        public async Task<RProp> GetProperty(IdClass idClass)
        {
            if (!ModelState.IsValid)
            {
                return new RProp
                {
                    responseMessage = Helper.GetErrorMessage("406", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
            var id = int.Parse(idClass.Id);
            var property = await _context.Prop.FindAsync(id);

            if (property == null)
            {
                return new RProp
                {
                    responseMessage = Helper.GetErrorMessage("402", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
            return new RProp
            {
                Id = property.Id,
                Name = property.Name,
                responseMessage = Helper.GetErrorMessage("200", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
            };
        }

        // PUT: api/property/5
        [HttpPost]
        [Route("EditProperty")]
        public async Task<BaseResponse> EditProperty(RPropRequest rProp)
        {
            if (!ModelState.IsValid)
            {
                return new BaseResponse
                {
                    responseMessage = Helper.GetErrorMessage("406", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
            Prop prop = new Prop
            {
                Id = rProp.Id,
                Name = rProp.Name
            };
            _context.Entry(prop).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PropExists(rProp.Id))
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

        private bool PropExists(int id)
        {
            return _context.Prop.Any(e => e.Id == id);
        }

        // POST: api/Categories
        [HttpPost]
        [Route("AddProperty")]
        public async Task<BaseResponse> AddProperty(RPropRequest rProp)
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
                Prop prop = new Prop
                {
                    Name = rProp.Name
                };
                _context.Prop.Add(prop);
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

        // DELETE: api/Categories/5
        [HttpPost]
        [Route("DeleteProperty")]
        public async Task<BaseResponse> DeleteProperty(IdClass idClass)
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
                var id = int.Parse(idClass.Id);
                var prop = await _context.Prop.FindAsync(id);
                if (prop == null)
                {
                    return new BaseResponse
                    {
                        responseMessage = Helper.GetErrorMessage("402", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                    };
                }

                _context.Prop.Remove(prop);
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
    }
}
