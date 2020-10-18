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
    public class OrdersController : Controller
    {
        private readonly OnlineMarketContext _context = new OnlineMarketContext();
        private readonly IHostingEnvironment _env;

        public OrdersController(IHostingEnvironment env)
        {
            _env = env;
        }
        [Route("AddOrder")]
        [HttpPost]
        public async Task<BaseResponse> AddOrder(ROrder rOrder)
        {
            try
            {
                EndUser endUser = new EndUser();
                var arrUserNameandPassword = rOrder.UserId.Split(':');
                if (Helper.IsAuthorizedEndUser(arrUserNameandPassword[0], arrUserNameandPassword[1]))
                {
                    endUser = _context.EndUser.SingleOrDefault(u => u.UserName == arrUserNameandPassword[0]);
                }
                else
                {
                    return new BaseResponse
                    {
                        responseMessage = Helper.GetErrorMessage("503", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                    };
                }
                Order order = new Order
                {
                    Id = Guid.NewGuid().ToString(),
                    OrderStatus = rOrder.OrderStatus,
                    DeliveryLocation = rOrder.DeliveryLocation,
                    DeliveryPrice = rOrder.DeliveryPrice,
                    DeliveryTime = rOrder.DeliveryTime,
                    DeliveryType = rOrder.DeliveryType,
                    OrderTime = rOrder.OrderTime,
                    SubTotal = rOrder.SubTotal,
                    TotalPrice = rOrder.TotalPrice,
                    UserId = endUser.Id
                };
                await _context.Order.AddAsync(order);
                List<OrderDetail> details = new List<OrderDetail>();
                foreach (var detail in rOrder.OrderDetails)
                {
                    details.Add(new OrderDetail
                    {
                        Id = Guid.NewGuid().ToString(),
                        IsSpOffer = detail.IsSpOffer,
                        OrderId = order.Id,
                        PaidPrice = detail.PaidPrice,
                        ProductId = detail.ProductId,
                        Quantity = detail.Quantity
                    });
                }
                await _context.OrderDetail.AddRangeAsync(details);
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
        [Route("EditOrder")]
        [HttpPost]
        public async Task<BaseResponse> EditOrder(ROrder rOrder)
        {
            try
            {
                Order order = new Order
                {
                    Id = rOrder.Id,
                    OrderStatus = rOrder.OrderStatus,
                    DeliveryLocation = rOrder.DeliveryLocation,
                    DeliveryPrice = rOrder.DeliveryPrice,
                    DeliveryTime = rOrder.DeliveryTime,
                    DeliveryType = rOrder.DeliveryType,
                    OrderTime = rOrder.OrderTime,
                    SubTotal = rOrder.SubTotal,
                    TotalPrice = rOrder.TotalPrice,
                    UserId = rOrder.UserId
                };
                _context.Entry(order).State = EntityState.Modified;
                var dets = _context.OrderDetail.Where(d => d.OrderId == order.Id).ToList();
                if (dets.Count > 0)
                {
                    _context.OrderDetail.RemoveRange(dets);
                }
                await _context.SaveChangesAsync();
                List<OrderDetail> details = new List<OrderDetail>();
                foreach (var detail in rOrder.OrderDetails)
                {
                    details.Add(new OrderDetail
                    {
                        Id = Guid.NewGuid().ToString(),
                        IsSpOffer = detail.IsSpOffer,
                        OrderId = order.Id,
                        PaidPrice = detail.PaidPrice,
                        ProductId = detail.ProductId,
                        Quantity = detail.Quantity
                    });
                }
                await _context.OrderDetail.AddRangeAsync(details);
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
        [Route("OrdersHistory")]
        [HttpPost]
        public async Task<Orders> OrdersHistory(HistoryRequest historyRequest)
        {
            try
            {
                EndUser endUser = new EndUser();
                var arrUserNameandPassword = historyRequest.token.Split(':');
                if (Helper.IsAuthorizedEndUser(arrUserNameandPassword[0], arrUserNameandPassword[1]))
                {
                    endUser = await _context.EndUser.SingleOrDefaultAsync(u => u.UserName == arrUserNameandPassword[0]);
                }
                else
                {
                    return new Orders
                    {
                        responseMessage = Helper.GetErrorMessage("503", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                    };
                }
                var skip = historyRequest.Page * 10;
                var orders = _context.Order.Where(o => o.UserId == endUser.Id).Skip(skip).Take(10).ToList();
                List<ROrder> rOrders = new List<ROrder>();
                foreach (var order in orders)
                {
                    List<ROrderDetail> rOrderDetails = new List<ROrderDetail>();
                    foreach (var orderDetail in order.OrderDetail)
                    {
                        Product product = _context.Product.SingleOrDefault(p => p.Id == orderDetail.ProductId);
                        rOrderDetails.Add(new ROrderDetail
                        {
                            Id = orderDetail.Id,
                            IsSpOffer = orderDetail.IsSpOffer,
                            OrderId = orderDetail.OrderId,
                            PaidPrice = orderDetail.PaidPrice,
                            Quantity = orderDetail.Quantity,
                            Product = new RProduct
                            {
                                Id = product.Id,
                                CategoryId = product.CategoryId,
                                Description = product.Description,
                                ExpireDate = product.ExpireDate,
                                Img = product.Img,
                                Name = product.Name,
                                Price = product.Price,
                            }
                        });
                    }
                    rOrders.Add(new ROrder
                    {
                        Id = order.Id,
                        DeliveryLocation = order.DeliveryLocation,
                        DeliveryPrice = order.DeliveryPrice,
                        DeliveryTime = order.DeliveryTime,
                        DeliveryType = order.DeliveryType,
                        OrderStatus = order.OrderStatus,
                        OrderTime = order.OrderTime,
                        SubTotal = order.SubTotal,
                        TotalPrice = order.TotalPrice,
                        OrderDetails = rOrderDetails
                    });
                }
                return new Orders
                {
                    count = _context.Order.Where(o => o.UserId == endUser.Id).Count(),
                    orders = rOrders,
                    responseMessage = Helper.GetErrorMessage("200", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
            catch (Exception ex)
            {
                return new Orders
                {
                    responseMessage = Helper.GetErrorMessage("500", Path.Combine(_env.WebRootPath, "ErrorMessages.json"))
                };
            }
        }
    }
}