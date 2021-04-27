using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Amazon.SellingPartner.Api.Controllers
{
    public class OauthController : Controller
    {
        /// <summary>
        /// 获取登入Amazon链接
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAmazonLoginUrl")]
        public IActionResult GetAmazonLoginUrl()
        {
            var LoginUrl = "";
            return Ok(LoginUrl);
        }
        /// <summary>
        /// 根据授权码获取用户Token
        /// </summary>
        /// <param name="code">授权码</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetUserTokenAsync")]
        public async Task<IActionResult> GetUserTokenAsync(string selling_partner_id, string mws_auth_token,string spapi_oauth_code)
        {
            //使用获取到的Code 获取Token
            var Toekn = "";

            var _client = new HttpClient();
            var  kv = new Dictionary<string, string>();
            kv.Add("grant_type", "authorization_code");
            kv.Add("code", "");
            kv.Add("client_id", "");
            kv.Add("client_secret", "");

            var httpContent = new FormUrlEncodedContent(kv);

            var httpResponse = await _client.PostAsync("https://api.amazon.com/auth/o2/token", httpContent);

            var content = await httpResponse.Content.ReadAsStringAsync();

            TokenResponse obj = JsonConvert.DeserializeObject<TokenResponse>(content);

            //save selling_partner_id and token to Db etc

            return Ok(Toekn);
        }
    }

    public class TokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string refresh_token { get; set; }
    }
}
