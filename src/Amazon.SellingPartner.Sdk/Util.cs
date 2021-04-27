using Amazon.SellingPartner.Sdk.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Amazon.SellingPartner.Sdk
{
    public static class Util
    {
        public const string ValidUrlCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
        public const string SignerMethod = "AWS4-HMAC-SHA256";
        public const string CredentialSubHeaderName = "Credential";
        public const string SignatureSubHeaderName = "Signature";
        public const string SignedHeadersSubHeaderName = "SignedHeaders";
        public const string TerminationString = "aws4_request";
        public const string ISO8601BasicDateTimeFormat = "yyyyMMddTHHmmssZ";
        public const string ISO8601BasicDateFormat = "yyyyMMdd";
        public const string Slash = "/";

        public const string HostHeaderName = "Host";
        public const string XAmzDateHeaderName = "X-Amz-Date";
        public const string XAmzAccessTokenHeaderName = "X-Amz-Access-Token";
        public const string ContentTypeHeaderName = "Content-Type";

        private readonly static Regex CompressWhitespaceRegex = new Regex("\\s+");


        /// <summary>
        /// Returns hashed value of input data using SHA256
        /// </summary>
        /// <param name="data">String to be hashed</param>
        /// <returns>Hashed value of input data</returns>
        private static byte[] Hash(string data)
        {
            return new SHA256CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(data));
        }

        /// <summary>
        /// Returns lowercase hexadecimal string of input byte array
        /// </summary>
        /// <param name="data">Data to be converted</param>
        /// <returns>Lowercase hexadecimal string</returns>
        private static string ToHex(byte[] data)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2", CultureInfo.InvariantCulture));
            }

            return sb.ToString();
        }

        /// <summary>
        ///HMACSHA256根据给定key计算value的Hash
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">String to be hashed</param>
        /// <returns>Hashed value of input data</returns>
        private static byte[] GetKeyedHash(byte[] key, string value)
        {
            KeyedHashAlgorithm hashAlgorithm = new HMACSHA256(key);
            hashAlgorithm.Initialize();
            return hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(value));
        }

        /// <summary>
        /// Returns URL encoded version of input data according to RFC-3986
        /// </summary>
        /// <param name="data">String to be URL-encoded</param>
        /// <returns>URL encoded version of input data</returns>
        public static string UrlEncode(string data)
        {
            StringBuilder encoded = new StringBuilder();
            foreach (char symbol in Encoding.UTF8.GetBytes(data))
            {
                if (ValidUrlCharacters.IndexOf(symbol) != -1)
                {
                    encoded.Append(symbol);
                }
                else
                {
                    encoded.Append("%").Append(string.Format(CultureInfo.InvariantCulture, "{0:X2}", (int)symbol));
                }
            }
            return encoded.ToString();
        }

        /// <summary>
        /// 构造规范请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string BuildStringToSignCanonicalRequestHash<T,K>(BaseRequest<T,K> request)
        {

            var canonicalizedRequest = new StringBuilder();
            //Request Method
            canonicalizedRequest.AppendFormat("{0}\n", request.RequestType);

            //CanonicalURI
            canonicalizedRequest.AppendFormat("{0}\n", ExtractCanonicalURIParameters(request));

            //CanonicalQueryString
            canonicalizedRequest.AppendFormat("{0}\n", ExtractCanonicalQueryString(request));

            //CanonicalHeaders
            canonicalizedRequest.AppendFormat("{0}\n", ExtractCanonicalHeaders(request));

            //SignedHeaders
            canonicalizedRequest.AppendFormat("{0}\n", ExtractSignedHeaders(request));

            // Hash(digest) the payload in the body
            canonicalizedRequest.AppendFormat(HashRequestBody(request));

            string canonicalRequest = canonicalizedRequest.ToString();

            //Create a digest(hash) of the canonical request
            return ToHex(Hash(canonicalRequest));

        }

        /// <summary>
        /// 构造字典
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static Dictionary<string, string> ToDictionary(object obj)
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            Type t = obj.GetType(); // 获取对象对应的类， 对应的类型

            PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance); // 获取当前type公共属性

            foreach (PropertyInfo p in pi)
            {
                MethodInfo m = p.GetGetMethod();

                if (m != null && m.IsPublic)
                {
                    // 进行判NULL处理 
                    if (m.Invoke(obj, new object[] { }) != null)
                    {
                        if (m.ReturnType == typeof(string))
                        {
                            map.Add(p.Name, (string)m.Invoke(obj, new object[] { })); // 向字典添加元素
                        }
                        else
                        {
                            map.Add(p.Name, JsonConvert.SerializeObject(m.Invoke(obj, new object[] { }))); // 向字典添加元素
                        }
                    }
                }
            }

            return map.OrderBy(p => p.Key).ToDictionary(p => p.Key, o => o.Value);
        }

        #region 构造规范URI参数

        /// <summary>
        /// 构造规范URI参数
        /// </summary>
        /// <param name="Uri"></param>
        /// <returns></returns>
        public static string ExtractCanonicalURIParameters<T,K>(BaseRequest<T,K> request)
        {
            string canonicalUri = string.Empty;

            if (string.IsNullOrEmpty(request.Uri))
            {
                canonicalUri = Slash;
            }
            else
            {
                if (!request.Uri.StartsWith(Slash))
                {
                    canonicalUri = Slash;
                }
                //Split path at / into segments
                IEnumerable<string> encodedSegments = request.Uri.Split(new char[] { '/' }, StringSplitOptions.None);

                // Encode twice
                encodedSegments = encodedSegments.Select(segment => UrlEncode(segment));
                encodedSegments = encodedSegments.Select(segment => UrlEncode(segment));

                canonicalUri += string.Join(Slash, encodedSegments.ToArray());
            }

            return canonicalUri;
        }

        /// <summary>
        ///构造规范查询字符串
        /// </summary>
        /// <param name="request">RestRequest</param>
        /// <returns>Query parameters in canonical order with URL encoding</returns>
        public static string ExtractCanonicalQueryString<T,K>(BaseRequest<T,K> request)
        {
            var sortedqueryParameters = ToDictionary(request.Parameters);

            StringBuilder canonicalQueryString = new StringBuilder();

            foreach (var key in sortedqueryParameters.Keys)
            {
                if (canonicalQueryString.Length > 0)
                {
                    canonicalQueryString.Append("&");
                }
                canonicalQueryString.AppendFormat("{0}={1}",
                   UrlEncode(key),
                   UrlEncode(sortedqueryParameters[key]));
            }

            return canonicalQueryString.ToString();

        }

        /// <summary>
        /// 构建规范标题
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string ExtractCanonicalHeaders<T,K>(BaseRequest<T,K> request)
        {
            Dictionary<string, string> sortedHeaders = new Dictionary<string, string>();
            sortedHeaders.Add(HostHeaderName, request.Header.Host);
            sortedHeaders.Add(ContentTypeHeaderName, request.Header.ContentType);
            sortedHeaders.Add(XAmzDateHeaderName, request.Header.XAmzDate.ToString(ISO8601BasicDateTimeFormat, CultureInfo.InvariantCulture));
            sortedHeaders.Add(XAmzAccessTokenHeaderName, request.Header.XAmzAccessToken);

            //这里要排序
            sortedHeaders =sortedHeaders.OrderBy(p => p.Key).ToDictionary(p => p.Key, o => o.Value);

            StringBuilder headerString = new StringBuilder();

            foreach (string headerName in sortedHeaders.Keys)
            {
                headerString.AppendFormat("{0}:{1}\n",
                    headerName.ToLowerInvariant(),
                    CompressWhitespaceRegex.Replace(sortedHeaders[headerName].Trim(), " "));
            }

            return headerString.ToString();
        }

        /// <summary>
        /// 构建签名的标题
        /// </summary>
        /// <param name="request">RestRequest</param>
        /// <returns>List of Http headers in canonical order</returns>
        public static string ExtractSignedHeaders<T,K>(BaseRequest<T,K> request)
        {
            List<string> result = new List<string>();
            try
            {
                Type type = request.Header.GetType();
                object obj = Activator.CreateInstance(type);
                var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var property in props)
                {
                    var attr = property.GetCustomAttribute(typeof(PropertieNameAttribute),true) as PropertieNameAttribute;
                    if (attr != null)
                    {
                        result.Add(attr.Name.ToLower());
                    }
                }
            }
            catch (Exception ex)
            { }

            List<string> rawHeaders = result.Select(header => header.Trim().ToLowerInvariant())
                                                        .ToList();
            rawHeaders.Sort(StringComparer.OrdinalIgnoreCase);

            return string.Join(";", rawHeaders);
        }

        /// <summary>
        /// 构建请求体Hash
        /// </summary>
        /// <param name="request">RestRequest</param>
        /// <returns>Hexadecimal hashed value of payload in the body of request</returns>
        public static string HashRequestBody<T,K>(BaseRequest<T,K> request)
        {
            var parametersJson = JsonConvert.SerializeObject(request.Parameters);
            return ToHex(Hash(parametersJson.ToString()));
        } 
        #endregion

        /// <summary>
        /// 构造凭据作用域值
        /// </summary>
        /// <param name="signingDate"></param>
        /// <param name="Region"></param>
        /// <param name="ServiceName"></param>
        /// <returns></returns>
        public static string BuildScope(DateTime signingDate, string Region, string ServiceName)
        {
            return string.Format("{0}/{1}/{2}/{3}",
                                 signingDate.ToString(ISO8601BasicDateFormat, CultureInfo.InvariantCulture),
                                 Region,
                                 ServiceName,
                                 TerminationString);
        }

        /// <summary>
        /// 构造签名字符串
        /// </summary>
        /// <param name="Algorithm"></param>
        /// <param name="RequestDateTime"></param>
        /// <param name="Region"></param>
        /// <param name="ServiceName"></param>
        /// <param name="CanonicalRequestHash"></param>
        /// <returns></returns>
        public static string BuildStringToSign(string Algorithm,DateTime RequestDateTime,string Region,string ServiceName,string CanonicalRequestHash)
        {
            var scope= BuildScope(RequestDateTime, Region, ServiceName);
            var StringToSign =
                                   Algorithm + "\n" +
                                   RequestDateTime.ToString(ISO8601BasicDateTimeFormat, CultureInfo.InvariantCulture) + "\n" +
                                   scope + "\n" +
                                   CanonicalRequestHash;
            return StringToSign;
        }

        /// <summary>
        /// 计算签名
        /// </summary>
        /// <param name="StringToSign"></param>
        /// <param name="Region"></param>
        /// <param name="ServiceName"></param>
        /// <param name="SigningDate"></param>
        /// <param name="SecretKey"></param>
        /// <returns></returns>
        public static string CalculateSignature(string StringToSign,string Region,string ServiceName, DateTime SigningDate,string SecretKey)
        {
            string date = SigningDate.ToString(ISO8601BasicDateFormat, CultureInfo.InvariantCulture);

            byte[] kSecret = Encoding.UTF8.GetBytes("AWS4" + SecretKey);
            byte[] kDate = GetKeyedHash(kSecret, date);
            byte[] kRegion = GetKeyedHash(kDate, Region);
            byte[] kService = GetKeyedHash(kRegion, ServiceName);
            byte[] kSigning = GetKeyedHash(kService, TerminationString);

            // Calculate the signature
            return ToHex(GetKeyedHash(kSigning, StringToSign));
        }

        /// <summary>
        /// 将签名添加到请求头
        /// </summary>
        /// <param name="restRequest">Request to be signed</param>
        /// <param name="accessKeyId">Access Key Id</param>
        /// <param name="signedHeaders">Signed Headers</param>
        /// <param name="signature">The signature to add</param>
        /// <param name="region">AWS region for the request</param>
        /// <param name="signingDate">Signature date</param>
        public static string AddSignature<T,K>(BaseRequest<T,K> request)
        {
            var RequestDate = request.Header.XAmzDate;

            //构建规范请求
            var CanonicalRequestHash = Util.BuildStringToSignCanonicalRequestHash(request);

            //创建签名字符串
            var StringToSign = Util.BuildStringToSign(SignerMethod, RequestDate, request.Config.Region, request.Config.ServiceName, CanonicalRequestHash);

            //签名计算
            var Signature = Util.CalculateSignature(StringToSign, request.Config.Region, request.Config.ServiceName, RequestDate, request.Config.SecretKey);

            //凭据范围
            string scope = BuildScope(RequestDate, request.Config.Region, request.Config.ServiceName);

            StringBuilder authorizationHeaderValueBuilder = new StringBuilder();
            authorizationHeaderValueBuilder.AppendFormat("{0}", SignerMethod);
            authorizationHeaderValueBuilder.AppendFormat(" {0}={1}/{2},", CredentialSubHeaderName, request.Config.SecretKey, scope);
            authorizationHeaderValueBuilder.AppendFormat(" {0}={1},", SignedHeadersSubHeaderName, ExtractSignedHeaders(request));
            authorizationHeaderValueBuilder.AppendFormat(" {0}={1}", SignatureSubHeaderName, Signature);

            return authorizationHeaderValueBuilder.ToString();
          
        }
    }
}
