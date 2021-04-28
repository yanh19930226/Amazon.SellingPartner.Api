using System;
using System.Collections.Generic;
using System.Text;

namespace Amazon.SellingPartner.Sdk.Models.Orders
{

    public class GetOrderAddressRequest : BaseRequest<GetOrderAddressRequestPara, BaseResponse<GetOrderAddressResponse>>
    {
        public GetOrderAddressRequest(string orderId, GetOrderAddressRequestPara para, string token) : base(para, token)
        {
            this.OrderId = orderId;
        }
        public string OrderId { get; set; }
        public override string Uri => "/orders/v0/orders/" + OrderId + "/address";

    }
    public class GetOrderAddressResponse
    {
        public string AmazonOrderId { get; set; }
        public ShippingAddress ShippingAddress { get; set; }
    }

    public class ShippingAddress
    {
        public string Name { get; set; }
        public string OrderId { get; set; }
        public string AddressLine1 { get; set; }
        public string City { get; set; }
        public string StateOrRegion { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }

    }
    public class GetOrderAddressRequestPara : SellingPartnerApiBasePara
    {
    }
}
