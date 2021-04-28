using Amazon.SellingPartner.Sdk.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Amazon.SellingPartner.Sdk
{
    public class SellingPartnerClient:IAmazonClient
    {

        private readonly Config _config;

        private readonly RequestHeader _header;

        public SellingPartnerClient(Config config, RequestHeader header)
        {
            _config = config;
            _header = header;
        }

        #region Get

        public async Task<AmazonResult<K>> GetAsync<T, K>(BaseRequest<T,K> request)
        {
            AmazonResult<K> result = new AmazonResult<K>();

            request.Config = _config;

            request.Header = _header;

            var client = new RestClient(_config.EndPoint);

            var rq = new RestRequest(_config.EndPoint + Util.ExtractCanonicalURIParameters(request) + "?" + Util.ExtractCanonicalQueryString(request));

            rq.AddHeader("Authorization", Util.AddSignature(request));

            rq.AddHeader("host", request.Header.Host);

            rq.AddHeader("x-amz-date", request.Header.XAmzDate.ToString(Util.ISO8601BasicDateTimeFormat, CultureInfo.InvariantCulture));

            rq.AddHeader("x-amz-access-token", request.Token);

            var httpResponse = client.Get(rq).Content;
          
            var data = JsonConvert.DeserializeObject<K>(httpResponse);

            return await Task.FromResult(result);

        }

        #endregion

        #region Post

        public async Task<AmazonResult<K>> PostAsync<T, K>(BaseRequest<T,K> request)
        {
            AmazonResult<K> result = new AmazonResult<K>();

            request.RequestType = RequestEnum.POST;

            request.Config = _config;

            request.Header = _header;

            var client = new RestClient(_config.EndPoint);

            var rq = new RestRequest(_config.EndPoint + Util.ExtractCanonicalURIParameters(request) + "?" + Util.ExtractCanonicalQueryString(request));

            rq.AddHeader("Authorization", Util.AddSignature(request));

            rq.AddHeader("x-amz-date", request.Header.XAmzDate.ToString(Util.ISO8601BasicDateTimeFormat, CultureInfo.InvariantCulture));

            rq.AddHeader("x-amz-access-token", request.Token);

            rq.AddJsonBody(JsonConvert.SerializeObject(request.Parameters));

            var httpResponse = client.Post(rq).Content;

           
            return await Task.FromResult(result);
        }

        #endregion
    }
}
