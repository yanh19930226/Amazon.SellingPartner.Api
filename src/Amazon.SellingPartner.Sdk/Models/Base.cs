﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Amazon.SellingPartner.Sdk.Models
{
    public abstract class BaseRequest<T>
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

        protected BaseRequest(T Parameters)
        {
            this.Parameters = Parameters;
        }

        /// <summary>
        /// 请求方法
        /// </summary>
        public RequestEnum RequestType { get; set; } = RequestEnum.GET;
        /// <summary>
        /// 请求参数
        /// </summary>
        public T Parameters { get; set; }
        /// <summary>
        /// 具体资源地址
        /// </summary>
        public virtual string Uri { get; set; } = "/";

    }
    public class Config
    {
        public string ClientId { get; set; } = "amzn1.application-oa2-client.c2fc8f7819ba4c9987ecfb133f641d34";

        public string ClientSecret { get; set; } = "c0e0d14973452e7b3f2e99f863d3998c7a65adc80fe80f4d081ff9e298acf122";

        public string AccessKey { get; set; } = "V93GkqmzYh8xdYCy7WYhsYOvtEbz3/VVGpZKmdld";

        public string SecretKey { get; set; } = "V93GkqmzYh8xdYCy7WYhsYOvtEbz3/VVGpZKmdld";
        /// <summary>
        /// 具体服务
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        /// 区域默认us-east-1
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
        public DateTime XAmzDate { get; set; }= DateTime.UtcNow;
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

    public class Error
    {
        public string code { get; set; }
        public string message { get; set; }

        public string details { get; set; }
    }
}
