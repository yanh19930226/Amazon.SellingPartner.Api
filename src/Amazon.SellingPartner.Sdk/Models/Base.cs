using System;
using System.Collections.Generic;
using System.Text;

namespace Amazon.SellingPartner.Sdk.Models
{
    public abstract class BaseRequest<T>
    {
        public string ClientId { get; set; } = "";

        public string ClientSecret { get; set; } = "";

        public string AccessKey { get; set; } = "";

        public string SecretKey { get; set; } = "";
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
        /// 查询字符串
        /// </summary>
        public abstract string  QueryString{ get; }

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
