using OnlineMarketApi.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineMarketApi.Models.RequestsResponse
{
    public class Orders : BaseResponse
    {
        public List<ROrder> orders { get; set; }
        public int count { get; set; }

    }
    public class HistoryRequest:PaginationRequest
    {
        public string token { get; set; }
    }
    public class ROrder : BaseResponse
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public decimal DeliveryPrice { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderTime { get; set; }
        public int OrderStatus { get; set; }
        public DateTime DeliveryTime { get; set; }
        public int DeliveryType { get; set; }
        public string DeliveryLocation { get; set; }
        public List<ROrderDetail> OrderDetails { get; set; }
    }
    public partial class ROrderDetail
    {
        public string Id { get; set; }
        public string OrderId { get; set; }
        public decimal PaidPrice { get; set; }
        public bool? IsSpOffer { get; set; }
        public string ProductId { get; set; }
        public RProduct Product { get; set; }
        public int Quantity { get; set; }

    }
}
