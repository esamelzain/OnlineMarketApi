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
    public class OffersController : Controller
    {
        private readonly OnlineMarketContext _context = new OnlineMarketContext();
        private readonly IHostingEnvironment _env;

        public OffersController(IHostingEnvironment env)
        {
            _env = env;
        }

        // GET: api/Offers
        [HttpPost]
        [Route("GetOffers")]
        public Offers GetOffers(PaginationRequest paginationRequest)
        {
            try
            {
                int skip = paginationRequest.Page * 9;
                var Offers = _context.Offer.Where(o => o.IsActive == true).Skip(skip).Take(9).ToList();
                List<ROffer> ROffers = new List<ROffer>();
                foreach (var Offer in Offers)
                {
                    var P = _context.Product.SingleOrDefault(p => p.Id == Offer.ProductId);
                    RProduct product = new RProduct
                    {
                        Id = P.Id,
                        CategoryId = P.CategoryId,
                        Description = P.Description,
                        ExpireDate = P.ExpireDate,
                        Img = P.Img,
                        Name = P.Name,
                        Price = P.Price
                    };
                    ROffers.Add(new ROffer
                    {
                        Id = Offer.Id,
                        IsActive = Offer.IsActive,
                        OffPercentage = Offer.OffPercentage,
                        ProductId = Offer.ProductId,
                        Product = product,
                        responseMessage = null
                    });
                }
                return new Offers
                {
                    count = _context.Offer.Count(),
                    offers = ROffers,
                    responseMessage = Helper.GetErrorMessage("200", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
            catch (Exception ex)
            {
                return new Offers
                {
                    responseMessage = Helper.GetErrorMessage("500", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
        }
        // GET: api/Offers/5
        [HttpPost]
        [Route("GetOffer")]
        public async Task<ROffer> GetOffer(IdClass idClass)
        {
            if (!ModelState.IsValid)
            {
                return new ROffer
                {
                    responseMessage = Helper.GetErrorMessage("406", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }

            var Offer = await _context.Offer.FindAsync(idClass.Id);

            if (Offer == null)
            {
                return new ROffer
                {
                    responseMessage = Helper.GetErrorMessage("402", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
            var P = _context.Product.SingleOrDefault(p => p.Id == Offer.ProductId);
            RProduct product = new RProduct
            {
                Id = P.Id,
                CategoryId = P.CategoryId,
                Description = P.Description,
                ExpireDate = P.ExpireDate,
                Img = P.Img,
                Name = P.Name,
                Price = P.Price
            };
            return new ROffer
            {
                Id = Offer.Id,
                IsActive = Offer.IsActive,
                OffPercentage = Offer.OffPercentage,
                ProductId = Offer.ProductId,
                Product = product,
                responseMessage = Helper.GetErrorMessage("200", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
            };
        }

        // PUT: api/Offers/5
        [HttpPost]
        [Route("EditOffer")]
        public async Task<BaseResponse> EditOffer(ROffer ROffer)
        {
            if (!ModelState.IsValid)
            {
                return new BaseResponse
                {
                    responseMessage = Helper.GetErrorMessage("406", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
            Offer offer = new Offer
            {
                Id = ROffer.Id,
                IsActive = ROffer.IsActive,
                OffPercentage = ROffer.OffPercentage,
                ProductId = ROffer.ProductId,
            };
            _context.Entry(offer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OfferExists(ROffer.Id))
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

        // POST: api/Offers
        [HttpPost]
        [Route("AddOffer")]
        public async Task<BaseResponse> AddOffer(ROffer ROffer)
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
                if (OfferExists(ROffer.Id))
                {
                    return new BaseResponse
                    {
                        responseMessage = Helper.GetErrorMessage("441", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                    };
                }
                Offer Offer = new Offer
                {
                    Id = Guid.NewGuid().ToString(),
                    IsActive = ROffer.IsActive,
                    OffPercentage = ROffer.OffPercentage,
                    ProductId = ROffer.ProductId,
                };
                _context.Offer.Add(Offer);
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

        // DELETE: api/Offers/5
        [HttpPost]
        [Route("DeleteOffer")]
        public async Task<BaseResponse> DeleteOffer(IdClass idClass)
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

                var Offer = await _context.Offer.FindAsync(idClass.Id);
                if (Offer == null)
                {
                    return new BaseResponse
                    {
                        responseMessage = Helper.GetErrorMessage("402", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                    };
                }

                _context.Offer.Remove(Offer);
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
        private bool OfferExists(string id)
        {
            return _context.Offer.Any(e => e.Id == id);
        }
    }
}