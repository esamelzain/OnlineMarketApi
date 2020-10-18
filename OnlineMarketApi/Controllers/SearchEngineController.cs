using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using OnlineMarketApi.Handlers;
using OnlineMarketApi.Models.Db;
using OnlineMarketApi.Models.RequestsResponse;

namespace OnlineMarketApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchEngineController : Controller
    {
        private readonly OnlineMarketContext _context = new OnlineMarketContext();
        private readonly IHostingEnvironment _env;

        public SearchEngineController(IHostingEnvironment env)
        {
            _env = env;
        }
        [Route("SearchByKey")]
        [HttpPost]
        public RSearchEngine SearchByKey(Key key)
        {
            try
            {
                RSearchEngine rSearchEngine = new RSearchEngine();
                rSearchEngine = Helper.GetSearchByKey(key.key);
                if (rSearchEngine == null)
                {
                    return new RSearchEngine
                    {
                        responseMessage = Helper.GetErrorMessage("402", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                    };
                }
                else if(rSearchEngine.Categories.Count==0 && rSearchEngine.Products.Count == 0)
                {
                    return new RSearchEngine
                    {
                        responseMessage = Helper.GetErrorMessage("402", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                    };
                }
                else
                {
                    rSearchEngine.responseMessage = Helper.GetErrorMessage("200", Path.Combine(_env.WebRootPath, "ErrorMessages.json"));
                    return rSearchEngine;
                }
            }
            catch (Exception ex)
            {
                return new RSearchEngine
                {
                    responseMessage = Helper.GetErrorMessage("500", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
        }
    }
}