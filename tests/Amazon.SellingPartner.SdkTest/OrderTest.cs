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
                AccessKey = "V93GkqmzYh8xdYCy7WYhsYOvtEbz3/VVGpZKmdld",
                SecretKey = "AKIASGO577SSPQDFLQMT",
                EndPoint = "https://sellingpartnerapi-na.amazon.com",
                ServiceName = "execute-api",
            };
            var header = new RequestHeader()
            {
                Host = "sellingpartnerapi-na.amazon.com",
                ContentType= "application/json; charset=utf-8",
                XAmzAccessToken= "Atza|IwEBIEKeaolLdpj-f_DmOlFNhlx_ZtSgWnSSagsJ-RIV7c4dusozmHtfII6PSwLpdWstxJHX0L_ForMcyZ-SCXkrIyn5BTDKpMEUtrNvvTjpjo7bIv5z2t2j7GRj89w8KxF6CVAljayPiDF0eI0JxkkJyrK7O0BHMmXyTgTpBNzhRwfdAdQvH79M-8GGSCuyAXKopzhc4H0AZ2xuXLCmGWb2bHDenbcXkOx-a_DPfrOwttBiDJO-Mb6zn03heGc5nQElLN-dRKjqdpjs8ka8wCOiPhayD00hV5RflKgrUhEQPW2YlImev0ua5NaRi6pi0gIHwfAAZXQ8GI9n0dTXIFcXgm7Y"
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

            var res = await _client.GetAsync(new GetOrderByIdRequest("",para, ""));

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

            var res = await _client.GetAsync(new GetOrderListRequest(para, "Atza|IwEBIEKeaolLdpj-f_DmOlFNhlx_ZtSgWnSSagsJ-RIV7c4dusozmHtfII6PSwLpdWstxJHX0L_ForMcyZ-SCXkrIyn5BTDKpMEUtrNvvTjpjo7bIv5z2t2j7GRj89w8KxF6CVAljayPiDF0eI0JxkkJyrK7O0BHMmXyTgTpBNzhRwfdAdQvH79M-8GGSCuyAXKopzhc4H0AZ2xuXLCmGWb2bHDenbcXkOx-a_DPfrOwttBiDJO-Mb6zn03heGc5nQElLN-dRKjqdpjs8ka8wCOiPhayD00hV5RflKgrUhEQPW2YlImev0ua5NaRi6pi0gIHwfAAZXQ8GI9n0dTXIFcXgm7Y"));

            Assert.Equal("", "");
        }

    }
}
