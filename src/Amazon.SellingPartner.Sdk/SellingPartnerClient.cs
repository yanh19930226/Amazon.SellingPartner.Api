using Amazon.SellingPartner.Sdk.Models;
using Newtonsoft.Json;
using RestSharp;
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

        public SellingPartnerClient(Config config, RequestHeader header)
        {
            _client = new HttpClient();
            _config = config;
            _header = header;
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

            _client.DefaultRequestHeaders.Clear();

            var auth = Util.AddSignature(request);

            var client = new RestClient(_config.EndPoint);

            var rq = new RestRequest(request.Uri);

            rq.AddHeader("Authorization", auth);

            rq.AddJsonBody(JsonConvert.SerializeObject(request.Parameters));


            var httpResponse = client.Post(rq).Content;

            _client.DefaultRequestHeaders.Add("Authorization:", Util.AddSignature(request));

            var httpResponses = await _client.PostAsync(_config.EndPoint, new JsonContent(new { request.Parameters }));



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
