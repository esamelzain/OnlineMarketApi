using Newtonsoft.Json;
using OnlineMarketApi.Models.Db;
using OnlineMarketApi.Models.RequestsResponse;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineMarketApi.Handlers
{
    public class Helper
    {
        private static readonly OnlineMarketContext _context = new OnlineMarketContext();
        public static ErrorModel GetErrorMessage(string code, string path)
        {
            try
            {
                string json = File.ReadAllText(path);
                var ErrorList = JsonConvert.DeserializeObject<List<ErrorModel>>(json);
                var errorModel = ErrorList.SingleOrDefault(e => e.ErrorCode == code);
                return errorModel;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static Products GetMustProducts(int skip)
        {
            try
            {
                Products ReProducts = new Products();
                List<RProduct> rProducts = new List<RProduct>();
                Dictionary<string, int> keyValuePairs = new Dictionary<string, int>();
                var products = _context.Product.ToList();
                foreach (var product in products)
                {
                    keyValuePairs.Add(product.Id, _context.OrderDetail.Where(d => d.ProductId == product.Id).Count());
                }
                var Ordered = keyValuePairs.OrderBy(key => key.Value).ToList();
                Ordered = Ordered.Skip(skip).Take(9).ToList();
                foreach (var item in keyValuePairs)
                {
                    Product product = new Product();
                    product = products.SingleOrDefault(p => p.Id == item.Key);
                    if (product != null)
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
                ReProducts.products = rProducts;
                return ReProducts;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public static bool IsAuthorizedUser(string userName, string token)
        {
            try
            {
                var user = _context.User.SingleOrDefault(u => u.UserName == userName);
                if (user == null)
                    return false;
                string passHash = user.Password;
                string token2 = Hash.Create(userName + ":" + passHash);
                if (token2 == token)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static bool IsAuthorizedEndUser(string userName, string token)
        {
            try
            {
                var user = _context.EndUser.SingleOrDefault(u => u.UserName == userName);
                if (user == null)
                    return false;
                string passHash = user.UserPassword;
                string token2 = Hash.Create(userName + ":" + passHash);
                if (token2 == token)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static RSearchEngine GetSearchByKey(string key)
        {
            try
            {
                RSearchEngine searchEngine = new RSearchEngine();
                var products = _context.Product.Where(p => p.Name.Contains(key) || p.Description.Contains(key) || p.Category.CategoryName.Contains(key)).ToList().Distinct();
                var categories = _context.Category.Where(c => c.CategoryName.Contains(key)).ToList().Distinct();
                List<RRProduct> RRProduct = new List<RRProduct>();
                foreach (var product in products)
                {
                    var offer = _context.Offer.SingleOrDefault(o => o.ProductId == product.Id && o.IsActive);
                    ROffer rOffer = new ROffer();
                    if (offer != null)
                    {
                        rOffer.Id = offer.Id;
                        rOffer.IsActive = offer.IsActive;
                        rOffer.OffPercentage = offer.OffPercentage;
                        rOffer.ProductId = offer.ProductId;
                    }

                    RRProduct.Add(new RRProduct
                    {
                        Id = product.Id,
                        CategoryId = product.CategoryId,
                        Description = product.Description,
                        ExpireDate = product.ExpireDate,
                        Img = product.Img,
                        Name = product.Name,
                        Price = product.Price,
                        Quantity = product.Quantity,
                        ROffer = offer == null ? null : rOffer
                    });
                }
                List<RCategory> RCategory = new List<RCategory>();
                foreach (var cat in categories)
                {
                    RCategory.Add(new RCategory
                    {
                        CategoryImg = cat.CategoryImg,
                        CategoryName = cat.CategoryName,
                        Id = cat.Id
                        //RProducts = RRProduct
                    });
                }
                searchEngine.Categories = RCategory;
                searchEngine.Products = RRProduct;
                return searchEngine;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
