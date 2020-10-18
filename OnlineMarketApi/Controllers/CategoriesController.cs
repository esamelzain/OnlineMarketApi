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
    public class CategoriesController : ControllerBase
    {
        private readonly OnlineMarketContext _context = new OnlineMarketContext();
        private readonly IHostingEnvironment _env;

        public CategoriesController(IHostingEnvironment env)
        {
            _env = env;
        }
        // GET: api/AllCategories
        [HttpPost]
        [Route("GetAllCategories")]
        public Categories GetAllCategories()
        {
            try
            {
                // int skip = paginationRequest.Page * 10;
                var cats = _context.Category.ToList();
                List<RCategory> rCategories = new List<RCategory>();
                foreach (var cat in cats)
                {
                    rCategories.Add(new RCategory
                    {
                        Id = cat.Id,
                        CategoryName = cat.CategoryName,
                        CategoryImg = cat.CategoryImg,
                        responseMessage = null 
                    });
                }
                return new Categories
                {
                    count = _context.Category.Count(),
                    categories = rCategories,
                    responseMessage = Helper.GetErrorMessage("200", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            } 
            catch (Exception ex)
            {
                return new Categories
                {
                    responseMessage = Helper.GetErrorMessage("500", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
        }
        // GET: api/Categories
        [HttpPost]
        [Route("GetCategories")]
        public Categories GetCategories(PaginationRequest paginationRequest)
        {
            try
            {
                int skip = paginationRequest.Page * 9;
                var cats = _context.Category.Skip(skip).Take(9).ToList();
                List<RCategory> rCategories = new List<RCategory>();
                foreach (var cat in cats)
                {
                    rCategories.Add(new RCategory
                    {
                        Id = cat.Id,
                        CategoryName = cat.CategoryName,
                        CategoryImg = cat.CategoryImg,
                        responseMessage = null
                    });
                }
                return new Categories
                {
                    count = _context.Category.Count(),
                    categories = rCategories,
                    responseMessage = Helper.GetErrorMessage("200", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
            catch (Exception ex)
            {
                return new Categories
                {
                    responseMessage = Helper.GetErrorMessage("500", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
        }
        // GET: api/Categories/5
        [HttpPost]
        [Route("GetCategory")]
        public async Task<RCategory> GetCategory(IdClass idClass)
        {
            if (!ModelState.IsValid)
            {
                return new RCategory
                {
                    responseMessage = Helper.GetErrorMessage("406", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }

            var category = await _context.Category.FindAsync(idClass.Id);

            if (category == null)
            {
                return new RCategory
                {
                    responseMessage = Helper.GetErrorMessage("402", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
            return new RCategory
            {
                Id = category.Id,
                CategoryImg = category.CategoryImg,
                CategoryName = category.CategoryName,
                responseMessage = Helper.GetErrorMessage("200", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
            };
        }

        // PUT: api/Categories/5
        [HttpPost]
        [Route("EditCategory")]
        public async Task<BaseResponse> EditCategory(RCategory rcategory)
        {
            if (!ModelState.IsValid)
            {
                return new BaseResponse
                {
                    responseMessage = Helper.GetErrorMessage("406", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
            Category category = new Category
            {
                Id = rcategory.Id,
                CategoryImg = rcategory.CategoryImg,
                CategoryName = rcategory.CategoryName
            };
            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(rcategory.Id))
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

        // POST: api/Categories
        [HttpPost]
        [Route("AddCategory")]
        public async Task<BaseResponse> AddCategory(RCategory rcategory)
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
                Category category = new Category
                {
                    Id = Guid.NewGuid().ToString(),
                    CategoryImg = rcategory.CategoryImg,
                    CategoryName = rcategory.CategoryName
                };
                _context.Category.Add(category);
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
        [Route("DeleteCategory")]
        public async Task<BaseResponse> DeleteCategory(IdClass idClass)
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

                var category = await _context.Category.FindAsync(idClass.Id);
                if (category == null)
                {
                    return new BaseResponse
                    {
                        responseMessage = Helper.GetErrorMessage("402", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                    };
                }

                _context.Category.Remove(category);
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
        private bool CategoryExists(string id)
        {
            return _context.Category.Any(e => e.Id == id);
        }
    }
}