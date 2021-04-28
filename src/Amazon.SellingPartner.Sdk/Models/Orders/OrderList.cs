using System;
using System.Collections.Generic;
using System.Text;

namespace Amazon.SellingPartner.Sdk.Models.Orders
{
    public class GetOrderListRequest : BaseRequest<GetOrderListRequestPara,BaseResponse<GetOrderListResponse>>
    {
        public GetOrderListRequest(GetOrderListRequestPara para, string token) : base(para, token)
        {

        }

        public override string Uri => "/orders/v0/orders";
    }

    public class GetOrderListRequestPara : SellingPartnerApiBasePara
    {
        public string CreatedBefore { get; set; }
        public string CreatedAfter { get; set; }
        public string MaxResultsPerPage { get; set; }
        //public string OrderStatuses { get; set; }
        //public string BuyerEmail { get; set; }
        //public string NextToken { get; set; }

    }

    public class GetOrderListResponse
    {
        public List<Order> Orders { get; set; }
    }
}
