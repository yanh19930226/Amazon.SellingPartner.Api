using Amazon.SellingPartner.Sdk;
using Amazon.SellingPartner.Sdk.Models;
using Amazon.SellingPartner.Sdk.Models.Orders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Amazon.SellingPartner.SdkTest
{
    public class OrderTest
    {
        private readonly SellingPartnerClient _client;
        public OrderTest()
        {
            var config = new Config()
            {
                ClientId = "amzn1.application-oa2-client.c2fc8f7819ba4c9987ecfb133f641d34",
                ClientSecret = "c0e0d14973452e7b3f2e99f863d3998c7a65adc80fe80f4d081ff9e298acf122",
                AccessKey = "AKIASGO577SSPQDFLQMT",
                SecretKey = "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",
                EndPoint = "https://sellingpartnerapi-na.amazon.com",
                ServiceName = "execute-api",
            };

            var header = new RequestHeader()
            {
                Host = "sellingpartnerapi-na.amazon.com",
                //ContentType= "application/json; charset=utf-8",
                XAmzAccessToken= "Atza|IwEBIKfmTCxqXAgcJsbxK_WatCD_u_dFuBz-fKMHeeniE_kb7OlyJhiwNWHMI-GUFhFDoeSPGqaP_5HAP52Glr7_nTB0-_EZ_uJm0GWl2xx1cS8BBtSdyIkl-PsgMGOCdKvMB2AmHxdTEwgK2KgIGr2e680hw1L6ukP-rwKxDtCfxIx9olc4tODS_G5EPBXApAcESWjdNEhpbgJG6im2RfOQoYANdAFNKjZHeAfLCZ4XLnSydmEQnAWBlRAPef6ECVjScQtBW4xjeVaSXM8wiwfz4QcyrueLYk_69fqdTyrs8DGGOrAPupR7H_mJC2SpIxIVi_vmRpEDGZaEAcee-l6MhtW6"
            };

            _client = new SellingPartnerClient(config, header);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task getOrderById()
        {
            var para = new GetOrderByIdRequestPara()
            {

            };

            var res = await _client.GetAsync(new GetOrderByIdRequest("12333",para, ""));

            Assert.Equal("", "");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task getOrderList()
        {
            var para = new GetOrderListRequestPara()
            {
                 CreatedBefore= DateTime.UtcNow.ToString("yyyy-MM-dd"),
                 CreatedAfter= DateTime.UtcNow.AddDays(-5).ToString("yyyy-MM-dd"),
                 MaxResultsPerPage = "100",
                //OrderStatuses = "",
                //BuyerEmail = "",
                //NextToken = ""
            };

            var res = await _client.GetAsync(new GetOrderListRequest(para, "Atza|IwEBIKfmTCxqXAgcJsbxK_WatCD_u_dFuBz-fKMHeeniE_kb7OlyJhiwNWHMI-GUFhFDoeSPGqaP_5HAP52Glr7_nTB0-_EZ_uJm0GWl2xx1cS8BBtSdyIkl-PsgMGOCdKvMB2AmHxdTEwgK2KgIGr2e680hw1L6ukP-rwKxDtCfxIx9olc4tODS_G5EPBXApAcESWjdNEhpbgJG6im2RfOQoYANdAFNKjZHeAfLCZ4XLnSydmEQnAWBlRAPef6ECVjScQtBW4xjeVaSXM8wiwfz4QcyrueLYk_69fqdTyrs8DGGOrAPupR7H_mJC2SpIxIVi_vmRpEDGZaEAcee-l6MhtW6"));

            Assert.Equal("", "");
        }

    }
}
