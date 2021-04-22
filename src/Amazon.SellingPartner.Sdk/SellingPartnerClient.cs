using Amazon.SellingPartner.Sdk.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Amazon.SellingPartner.Sdk
{
    public class SellingPartnerClient:IAmazonClient
    {
        private HttpClient _client { get; }

        private readonly Config _config;
        private readonly RequestHeader _header;
        private readonly string _endPoint;
        private readonly string _serviceName;
        private readonly string _region;

        public SellingPartnerClient(Config config, RequestHeader header,string endPoint,string serviceName,string region)
        {
            _client = new HttpClient();
            _config = config;
            _header = header;
            _endPoint = endPoint;
            _serviceName = serviceName;
            _region = region;
        }

        #region Private
        private class JsonContent : StringContent
        {
            public JsonContent(object obj) :
            base(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")
            { }
        }

        #endregion

        #region Get

        #endregion

        #region Post
        public async Task<AmazonResult<K>> PostAsync<T, K>(BaseRequest<T> request)
        {
            AmazonResult<K> result = new AmazonResult<K>();

            request.Config = _config;
            request.Header = _header;
            request.ServiceName = _serviceName;
            request.Region = _region;

            _client.DefaultRequestHeaders.Clear();

            _client.DefaultRequestHeaders.Add("Authorization:",Util.AddSignature(request));


            var httpResponse = await _client.PostAsync(_endPoint, new JsonContent(new { request.Parameters }));

            //var content = await httpResponse.Content.ReadAsStringAsync();

            //T obj = JsonConvert.DeserializeObject<T>(content);

            //if (httpResponse.StatusCode != HttpStatusCode.OK)
            //{
            //    result.Failed(httpResponse.Content.ToString());
            //}
            //else
            //{
            //    result.Success(httpResponse.Content.ToString());
            //}
            //result.Result = obj;

            return await Task.FromResult(result);
        }
        #endregion
    }
}
