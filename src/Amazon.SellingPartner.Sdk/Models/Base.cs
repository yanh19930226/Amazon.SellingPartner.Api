using System;
using System.Collections.Generic;
using System.Text;

namespace Amazon.SellingPartner.Sdk.Models
{
    public abstract class BaseRequest<T>
    {
       /// <summary>
       /// 配置
       /// </summary>
        public Config Config { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        public string Region { get; set; }
       
        /// <summary>
        /// 服务:比如sellingpartner Api;adfee api 
        /// </summary>
        public string Service { get; set; }

        public RequestHeader Header { get; set; }

        public DateTime RequestDate { get; set; }
        /// <summary>
        /// 请求参数
        /// </summary>
        public T Parameters { get; set; }
        /// <summary>
        /// 请求方法
        /// </summary>
        public RequestEnum RequestType { get; set; } = RequestEnum.GET;
        /// <summary>
        /// 资源
        /// </summary>
        public abstract string Uri { get; }
        /// <summary>
        /// 查询字符串(方法Action)
        /// </summary>
        public abstract string  QueryString{ get; }

    }
    public class Config
    {
        public string ClientId { get; set; } = "amzn1.application-oa2-client.c2fc8f7819ba4c9987ecfb133f641d34";

        public string ClientSecret { get; set; } = "c0e0d14973452e7b3f2e99f863d3998c7a65adc80fe80f4d081ff9e298acf122";

        public string AccessKey { get; set; } = "V93GkqmzYh8xdYCy7WYhsYOvtEbz3/VVGpZKmdld";

        public string SecretKey { get; set; } = "V93GkqmzYh8xdYCy7WYhsYOvtEbz3/VVGpZKmdld";
    }

    public class RequestHeader
    {
        /// <summary>
        /// 主机地址
        /// </summary>
        public string Host { get; set; } = "";
        /// <summary>
        /// ContentType
        /// </summary>
        public string ContentType { get; set; } = "application/x-www-form-urlencoded; charset=utf-8";
    }

    public class BaseResponse<T>
    {
        public T payload { get; set; }

        public List<Error> errors { get; set; }

    }

    public class Error
    {
        public string code { get; set; }
        public string message { get; set; }

        public string details { get; set; }
    }
}
