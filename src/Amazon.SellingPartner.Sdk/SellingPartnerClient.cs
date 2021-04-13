using Amazon.SellingPartner.Sdk.Models;
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
        public SellingPartnerClient(HttpClient client)
        {
            _client = client;
          
        }
        #region Private
        


        #endregion

        #region Get

        #endregion

        #region Post
        public async Task<K> PostAsync<T, K>(BaseRequest<T> request)
        {
            AmazonResult<T> result = new AmazonResult<T>();

            //构建规范请求
            var CanonicalRequestHash = Util.BuildStringToSignCanonicalRequestHash(request);

            //创建签名字符串
            var StringToSign= Util.BuildStringToSign("",request.RequestDate,request.Region,request.Service, CanonicalRequestHash);

            //签名计算
            var Signature = Util.CalculateSignature(StringToSign, request.Region, request.Service, request.RequestDate,request.SecretKey);






            _client.DefaultRequestHeaders.Clear();

            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + request.Token);
            

            var httpResponse = await _client.PostAsync(url, new JsonContent(new { request.Parameter }));

            var content = await httpResponse.Content.ReadAsStringAsync();

            T obj = JsonConvert.DeserializeObject<T>(content);

            if (httpResponse.StatusCode != HttpStatusCode.OK)
            {
                result.Failed(httpResponse.Content.ToString());
            }
            else
            {
                result.Success(httpResponse.Content.ToString());
            }
            result.Result = obj;

            return await Task.FromResult(result);
        }
        #endregion
    }
}
