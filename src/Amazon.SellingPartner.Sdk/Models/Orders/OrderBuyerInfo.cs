using System;
using System.Collections.Generic;
using System.Text;

namespace Amazon.SellingPartner.Sdk.Models.Orders
{
    public class GetOrderBuyerInfoRequest : BaseRequest<GetOrderBuyerInfoRequestPara, BaseResponse<GetOrderBuyerInfoResponse>>
    {
        public GetOrderBuyerInfoRequest(string orderId, GetOrderBuyerInfoRequestPara para, string token) : base(para, token)
        {
            this.OrderId = orderId;
        }
        public string OrderId { get; set; }

        public override string Uri => "/orders/v0/orders/" + OrderId + "/buyerInfo";
    }

    public class GetOrderBuyerInfoResponse 
    {
        public string AmazonOrderId { get; set; }

        public string BuyerEmail { get; set; }

        public string BuyerName { get; set; }

        public string PurchaseOrderNumber { get; set; }
    }

    public class GetOrderBuyerInfoRequestPara : SellingPartnerApiBasePara
    {

    }
}
