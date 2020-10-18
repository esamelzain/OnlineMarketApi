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
    public class SettingsController : Controller
    {
        private readonly OnlineMarketContext _context = new OnlineMarketContext();
        private readonly IHostingEnvironment _env;

        public SettingsController(IHostingEnvironment env)
        {
            _env = env;
        }

        // GET: api/Settings
        [HttpPost]
        [Route("GetSettings")]
        public Settings GetSettings(PaginationRequest paginationRequest)
        {
            try
            {
                int skip = paginationRequest.Page * 9;
                var settings = _context.Setting.Skip(skip).Take(9).ToList();
                List<RSetting> RSettings = new List<RSetting>();
                foreach (var setting in settings)
                {
                    RSettings.Add(new RSetting
                    {
                        Setting = setting.Setting1,
                        Value = setting.Value,
                        responseMessage = null
                    });
                }
                return new Settings
                {
                    count = _context.Setting.Count(),
                    settings = RSettings,
                    responseMessage = Helper.GetErrorMessage("200", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
            catch (Exception ex)
            {
                return new Settings
                {
                    responseMessage = Helper.GetErrorMessage("500", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
        }
        // GET: api/Settings/5
        [HttpPost]
        [Route("GetSetting")]
        public async Task<RSetting> GetSetting(IdClass idClass)
        {
            if (!ModelState.IsValid)
            {
                return new RSetting
                {
                    responseMessage = Helper.GetErrorMessage("406", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }

            var Setting = await _context.Setting.FindAsync(idClass.Id);

            if (Setting == null)
            {
                return new RSetting
                {
                    responseMessage = Helper.GetErrorMessage("402", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
            return new RSetting
            {
                Setting = Setting.Setting1,
                Value = Setting.Value,
                responseMessage = Helper.GetErrorMessage("200", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
            };
        }

        // PUT: api/Settings/5
        [HttpPost]
        [Route("EditSetting")]
        public async Task<BaseResponse> EditSetting(RSetting RSetting)
        {
            if (!ModelState.IsValid)
            {
                return new BaseResponse
                {
                    responseMessage = Helper.GetErrorMessage("406", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
            Setting Setting = new Setting
            {
                Setting1 = RSetting.Setting,
                Value = RSetting.Value,
            };
            _context.Entry(Setting).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SettingExists(RSetting.Setting))
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

        // POST: api/Settings
        [HttpPost]
        [Route("AddSetting")]
        public async Task<BaseResponse> AddSetting(RSetting RSetting)
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
                if (SettingExists(RSetting.Setting))
                {
                    return new BaseResponse
                    {
                        responseMessage = Helper.GetErrorMessage("441", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                    };
                }
                Setting Setting = new Setting
                {
                    Setting1 = RSetting.Setting,
                    Value = RSetting.Value,
                };
                _context.Setting.Add(Setting);
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

        // DELETE: api/Settings/5
        [HttpPost]
        [Route("DeleteSetting")]
        public async Task<BaseResponse> DeleteSetting(IdClass idClass)
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

                var Setting = await _context.Setting.FindAsync(idClass.Id);
                if (Setting == null)
                {
                    return new BaseResponse
                    {
                        responseMessage = Helper.GetErrorMessage("402", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                    };
                }

                _context.Setting.Remove(Setting);
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
        private bool SettingExists(string Setting)
        {
            return _context.Setting.Any(e => e.Setting1 == Setting);
        }
    }
}