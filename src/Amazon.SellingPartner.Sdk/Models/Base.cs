using System;
using System.Collections.Generic;
using System.Text;

namespace Amazon.SellingPartner.Sdk.Models
{
    public abstract class BaseRequest<T,K>
    {
        #region Divde
        /// <summary>
        /// 配置
        /// </summary>
        public Config Config { get; set; }
        /// <summary>
        /// 请求头
        /// </summary>
        public RequestHeader Header { get; set; }
        #endregion

        protected BaseRequest(T Parameters, string token)
        {
            this.Parameters = Parameters;
            this.Token = token;
        }
        public string Token { get; set; }
        /// <summary>
        /// 默认请求方法
        /// </summary>
        public RequestEnum RequestType { get; set; } = RequestEnum.GET;
        /// <summary>
        /// 请求参数
        /// </summary>
        public T Parameters { get; set; }
        /// <summary>
        /// 默认具体资源地址
        /// </summary>
        public virtual string Uri { get; set; } = "/";

    }
    public class Config
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string AccessKey { get; set; }

        public string SecretKey { get; set; }
        /// <summary>
        /// 具体服务
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        /// 默认区域us-east-1
        /// </summary>
        public virtual string Region { get; set; } = "us-east-1";
        /// <summary>
        /// EndPoint
        /// </summary>
        public virtual string EndPoint { get; set; }
    }

    public class RequestHeader
    {
        /// <summary>
        /// 不同服务的地址:SellingPartner or Advertise etc
        /// </summary>
        [PropertieName("Host")]
        public string Host { get; set; }
        /// <summary>
        /// ContentType
        /// </summary>
        [PropertieName("Content-Type")]
        public string ContentType { get; set; } = "application/json; charset=utf-8";
        /// <summary>
        /// RequestDate
        /// </summary>
        [PropertieName("X-Amz-Date")]
        public DateTime XAmzDate { get; set; } = DateTime.UtcNow;
    }

    public class PropertieNameAttribute : Attribute
    {
        public String Name { get; set; }

        public PropertieNameAttribute()
        {

        }

        public PropertieNameAttribute(String name)
        {
            Name = name;
        }
    }
    public class BaseResponse<T>
    {
        public T payload { get; set; }

        public List<Error> errors { get; set; }

    }

    public class BasePara
    {
        public string MarketplaceId { get; set; } = "ATVPDKIKX0DER";
    }

    public class Error
    {
        public string code { get; set; }
        public string message { get; set; }

        public string details { get; set; }
    }
}
