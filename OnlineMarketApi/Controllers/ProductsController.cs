using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using OnlineMarketApi.Handlers;
using OnlineMarketApi.Models.Db;
using OnlineMarketApi.Models.RequestsResponse;

namespace OnlineMarketApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly OnlineMarketContext _context = new OnlineMarketContext();
        private readonly IHostingEnvironment _env;

        public ProductsController(IHostingEnvironment env)
        {
            _env = env;
        }

        // GET: api/Products
        [HttpPost]
        [Route("GetProducts")]
        public Products GetProducts(PaginationRequest paginationRequest)
        {
            try
            {
                int skip = paginationRequest.Page * 9;
                var products = _context.Product.Skip(skip).Take(9).ToList();
                List<RProduct> rProducts = new List<RProduct>();
                foreach (var product in products)
                {
                    var hasValidOffer = _context.Offer.SingleOrDefault(o => o.IsActive == true && o.ProductId == product.Id);
                    ROffer offer = null;
                    if (hasValidOffer != null)
                    {
                        offer = new ROffer
                        {
                            Id = hasValidOffer.Id,
                            IsActive = hasValidOffer.IsActive,
                            OffPercentage = hasValidOffer.OffPercentage,
                            ProductId = hasValidOffer.ProductId
                        };
                    }
                    rProducts.Add(new RProduct
                    {
                        Id = product.Id,
                        CategoryId = product.CategoryId,
                        Description = product.Description,
                        ExpireDate = product.ExpireDate,
                        Img = product.Img,
                        Name = product.Name,
                        Price = product.Price,
                        Quantity = product.Quantity,
                        ROffer = offer,
                        responseMessage = null
                    });
                }
                return new Products
                {
                    count = _context.Product.Count(),
                    products = rProducts.Distinct().ToList(),
                    responseMessage = Helper.GetErrorMessage("200", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
            catch (Exception ex)
            {
                return new Products
                {
                    responseMessage = Helper.GetErrorMessage("500", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
        }

        [HttpPost]
        [Route("GetMustBuyedProducts")]
        public Categories GetMustBuyedProducts(PaginationRequest paginationRequest)
        {
            try
            {
                Products products = new Products();
                products = Helper.GetMustProducts(paginationRequest.Page * 9);
                products.responseMessage = Helper.GetErrorMessage("200", Path.Combine(_env.WebRootPath, "ErrorMessages.json"));
                List<RCategory> categories = new List<RCategory>();
                foreach (var product in products.products)
                {
                    var cat = _context.Category.SingleOrDefault(c => c.Id == product.CategoryId);
                    var prods = _context.Product.Where(p => p.CategoryId == cat.Id).ToList();
                    List<RProduct> RProduct = new List<RProduct>();
                    foreach (var prod in prods)
                    {
                        RProduct.Add(new RProduct
                        {
                            Id = prod.Id,
                            CategoryId = prod.CategoryId,
                            Description = prod.Description,
                            ExpireDate = prod.ExpireDate,
                            Img = prod.Img,
                            Name = prod.Name,
                            Price = prod.Price,
                            Quantity = prod.Quantity,
                            responseMessage = null
                        });
                    }
                    var Rcat = new RCategory
                    {
                        Id = cat.Id,
                        CategoryImg = cat.CategoryImg,
                        CategoryName = cat.CategoryName,
                        RProducts = RProduct
                    };
                    if (categories.Count == 0)
                    {
                        categories.Add(Rcat);
                    }
                    else
                    {
                        bool found = false;
                        foreach (var item in categories)
                        {
                            if (item.Id == Rcat.Id)
                            {
                                found = true;
                            }
                        }
                        if (!found)
                        {
                            categories.Add(Rcat);
                        }
                    }
                }
                return new Categories
                {
                    categories = categories.Distinct().ToList(),
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

        [HttpPost]
        [Route("GetProductsByCategory")]
        public Products GetProductsByCategory(IdClass idClass)
        {
            try
            {
                var products = _context.Product.Where(pr => pr.CategoryId == idClass.Id);
                List<RProduct> rProducts = new List<RProduct>();
                foreach (var product in products)
                {
                    rProducts.Add(new RProduct
                    {
                        Id = product.Id,
                        CategoryId = product.CategoryId,
                        Description = product.Description,
                        ExpireDate = product.ExpireDate,
                        Img = product.Img,
                        Name = product.Name,
                        Price = product.Price,
                        Quantity = product.Quantity,
                        responseMessage = null
                    });
                }
                return new Products
                {
                    products = rProducts,
                    responseMessage = Helper.GetErrorMessage("200", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
            catch (Exception ex)
            {
                return new Products
                {
                    responseMessage = Helper.GetErrorMessage("500", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
        }

        // GET: api/Products/5
        [HttpPost]
        [Route("GetProduct")]
        public async Task<RProduct> GetProduct(IdClass idClass)
        {
            if (!ModelState.IsValid)
            {
                return new RProduct
                {
                    responseMessage = Helper.GetErrorMessage("406", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }

            var product = await _context.Product.FindAsync(idClass.Id);
            if (product == null)
            {
                return new RProduct
                {
                    responseMessage = Helper.GetErrorMessage("402", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
            var productProps = _context.ProductProp.Where(pP => pP.ProductId == idClass.Id).ToList();
            List<RProductProp> RProductProp = new List<RProductProp>();
            foreach (var prop in productProps)
            {
                RProductProp.Add(new Models.RequestsResponse.RProductProp
                {
                    Value = prop.Value
                });
            }
            return new RProduct
            {
                Id = product.Id,
                CategoryId = product.CategoryId,
                Description = product.Description,
                ExpireDate = product.ExpireDate,
                Img = product.Img,
                Name = product.Name,
                Price = product.Price,
                Quantity = product.Quantity,
                Props = RProductProp,
                responseMessage = Helper.GetErrorMessage("200", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
            };
        }

        // PUT: api/Products/5
        [HttpPost]
        [Route("EditProduct")]
        public async Task<BaseResponse> EditProduct(RProduct rProduct)
        {
            if (!ModelState.IsValid)
            {
                return new BaseResponse
                {
                    responseMessage = Helper.GetErrorMessage("406", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
            Product product = new Product
            {
                Id = rProduct.Id,
                CategoryId = rProduct.CategoryId,
                Description = rProduct.Description,
                ExpireDate = rProduct.ExpireDate,
                Img = rProduct.Img,
                Name = rProduct.Name,
                Price = rProduct.Price,
                Quantity = rProduct.Quantity,
            };
            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(rProduct.Id))
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

        // POST: api/Products
        [HttpPost]
        [Route("AddProduct")]
        public async Task<BaseResponse> AddCategory(RProduct rProduct)
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
                Product product = new Product
                {
                    Id = Guid.NewGuid().ToString(),
                    CategoryId = rProduct.CategoryId,
                    Description = rProduct.Description,
                    ExpireDate = rProduct.ExpireDate,
                    Img = rProduct.Img,
                    Name = rProduct.Name,
                    Price = rProduct.Price,
                    Quantity = rProduct.Quantity,
                };
                List<ProductProp> props = new List<ProductProp>();
                _context.Product.Add(product);
                await _context.SaveChangesAsync();
                foreach (var prop in rProduct.Props)
                {
                    props.Add(new ProductProp
                    {
                        ProductId = product.Id,
                        PropId = prop.PropId,
                        Value = prop.Value
                    });
                }
                _context.ProductProp.AddRange(props);
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

        // DELETE: api/Products/5
        [HttpPost]
        [Route("DeleteProduct")]
        public async Task<BaseResponse> DeleteProduct(IdClass idClass)
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

                var product = await _context.Product.FindAsync(idClass.Id);
                if (product == null)
                {
                    return new BaseResponse
                    {
                        responseMessage = Helper.GetErrorMessage("402", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                    };
                }

                _context.Product.Remove(product);
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

        private bool ProductExists(string id)
        {
            return _context.Product.Any(e => e.Id == id);
        }
    }
}