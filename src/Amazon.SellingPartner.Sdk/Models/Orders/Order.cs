using System;
using System.Collections.Generic;
using System.Text;

namespace Amazon.SellingPartner.Sdk.Models.Orders
{
    public class GetOrderByIdRequest : BaseRequest<GetOrderByIdRequestPara,BaseResponse<GetOrderByIdResponse>>
    {
        public GetOrderByIdRequest(string orderId,GetOrderByIdRequestPara para,string token) : base(para,token)
        {
            this.OrderId = orderId;
        }
        public string OrderId { get; set; }

        public override string Uri => "/orders/v0/orders/"+ OrderId;
    }

    public class GetOrderByIdResponse
    {
        public string AmazonOrderId { get; set; }

        public string PurchaseDate { get; set; }

        public string LastUpdateDate { get; set; }

        public string OrderStatus { get; set; }

        public string SellerOrderId { get; set; }

        public string FulfillmentChannel { get; set; }

        public string SalesChannel { get; set; }

        public string ShipServiceLevel { get; set; }
    }

    public class GetOrderByIdRequestPara: BasePara
    {

    }
}
