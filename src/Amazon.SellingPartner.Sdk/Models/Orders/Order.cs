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

    

    public class GetOrderByIdResponse: Order
    {
        
    }

    public class GetOrderByIdRequestPara: SellingPartnerApiBasePara
    {

    }
}
