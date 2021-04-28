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
                XAmzAccessToken= "Atza|IwEBIBnUKyHtmFz9Iqadvb1qWRVuWlJ08AJlWbyi6bk3g7QNRGfSOWDkPclYYaJEjdGv4xxrlgDzjTRHjDeoEE6LA1EWYbdQRiUROcVkAC0ZVGN977Wolcuf_YCz1gUnIHBfAFQhWKaFYnFHYs1xwYUO3bq-m301JJ-327C07GSnwEkof4ecaKa8qMOh6ie4t7upWjVO7szXb2TdPijLIaaKo67KwZdC-9f715PQzd8vRLpC9IuaNBTs4shxJl9lShjk_Ho_He0ySTSju_if1qDN5rPD-nakmOjVkbgENePWuddf9p-oP-HFt2DNBcKrXlfpgx94R-OJRwRRoUGfWgyUMaBZ"
            };

            _client = new SellingPartnerClient(config, header);
        }

        /// <summary>
        /// Id获取订单
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task getOrderById()
        {
            var para = new GetOrderByIdRequestPara()
            {
                
            };

            var res = await _client.GetAsync(new GetOrderByIdRequest("112-8204866-3349817", para, "Atza|IwEBIBnUKyHtmFz9Iqadvb1qWRVuWlJ08AJlWbyi6bk3g7QNRGfSOWDkPclYYaJEjdGv4xxrlgDzjTRHjDeoEE6LA1EWYbdQRiUROcVkAC0ZVGN977Wolcuf_YCz1gUnIHBfAFQhWKaFYnFHYs1xwYUO3bq-m301JJ-327C07GSnwEkof4ecaKa8qMOh6ie4t7upWjVO7szXb2TdPijLIaaKo67KwZdC-9f715PQzd8vRLpC9IuaNBTs4shxJl9lShjk_Ho_He0ySTSju_if1qDN5rPD-nakmOjVkbgENePWuddf9p-oP-HFt2DNBcKrXlfpgx94R-OJRwRRoUGfWgyUMaBZ"));

            Assert.Equal("", "");
        }

        /// <summary>
        /// Id获取订单买家
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task getOrderBuyerInfo()
        {
            var para = new GetOrderBuyerInfoRequestPara()
            {

            };

            var res = await _client.GetAsync(new GetOrderBuyerInfoRequest("112-8204866-3349817", para, "Atza|IwEBIBnUKyHtmFz9Iqadvb1qWRVuWlJ08AJlWbyi6bk3g7QNRGfSOWDkPclYYaJEjdGv4xxrlgDzjTRHjDeoEE6LA1EWYbdQRiUROcVkAC0ZVGN977Wolcuf_YCz1gUnIHBfAFQhWKaFYnFHYs1xwYUO3bq-m301JJ-327C07GSnwEkof4ecaKa8qMOh6ie4t7upWjVO7szXb2TdPijLIaaKo67KwZdC-9f715PQzd8vRLpC9IuaNBTs4shxJl9lShjk_Ho_He0ySTSju_if1qDN5rPD-nakmOjVkbgENePWuddf9p-oP-HFt2DNBcKrXlfpgx94R-OJRwRRoUGfWgyUMaBZ"));

            Assert.Equal("", "");
        }

        /// <summary>
        /// Id获取订单地址
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task getGetOrderAddress()
        {
            var para = new GetOrderAddressRequestPara()
            {

            };

            var res = await _client.GetAsync(new GetOrderAddressRequest("112-8204866-3349817", para, "Atza|IwEBIBnUKyHtmFz9Iqadvb1qWRVuWlJ08AJlWbyi6bk3g7QNRGfSOWDkPclYYaJEjdGv4xxrlgDzjTRHjDeoEE6LA1EWYbdQRiUROcVkAC0ZVGN977Wolcuf_YCz1gUnIHBfAFQhWKaFYnFHYs1xwYUO3bq-m301JJ-327C07GSnwEkof4ecaKa8qMOh6ie4t7upWjVO7szXb2TdPijLIaaKo67KwZdC-9f715PQzd8vRLpC9IuaNBTs4shxJl9lShjk_Ho_He0ySTSju_if1qDN5rPD-nakmOjVkbgENePWuddf9p-oP-HFt2DNBcKrXlfpgx94R-OJRwRRoUGfWgyUMaBZ"));

            Assert.Equal("", "");
        }

        /// <summary>
        /// 获取订单列表
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

            var res = await _client.GetAsync(new GetOrderListRequest(para, "Atza|IwEBIBnUKyHtmFz9Iqadvb1qWRVuWlJ08AJlWbyi6bk3g7QNRGfSOWDkPclYYaJEjdGv4xxrlgDzjTRHjDeoEE6LA1EWYbdQRiUROcVkAC0ZVGN977Wolcuf_YCz1gUnIHBfAFQhWKaFYnFHYs1xwYUO3bq-m301JJ-327C07GSnwEkof4ecaKa8qMOh6ie4t7upWjVO7szXb2TdPijLIaaKo67KwZdC-9f715PQzd8vRLpC9IuaNBTs4shxJl9lShjk_Ho_He0ySTSju_if1qDN5rPD-nakmOjVkbgENePWuddf9p-oP-HFt2DNBcKrXlfpgx94R-OJRwRRoUGfWgyUMaBZ"));

            Assert.Equal("", "");
        }

    }
}
