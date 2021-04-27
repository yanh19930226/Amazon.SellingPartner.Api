using Amazon.SellingPartner.Sdk;
using Amazon.SellingPartner.Sdk.Models;
using Amazon.SellingPartner.Sdk.Models.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Amazon.SellingPartner.SdkTest
{
    public class SellingPartnerClientTest
    {
        private readonly SellingPartnerClient _client;

        public SellingPartnerClientTest()
        {
            var config = new Config() {
                ClientId= "amzn1.application-oa2-client.c2fc8f7819ba4c9987ecfb133f641d34",
                ClientSecret= "c0e0d14973452e7b3f2e99f863d3998c7a65adc80fe80f4d081ff9e298acf122",
                AccessKey= "V93GkqmzYh8xdYCy7WYhsYOvtEbz3/VVGpZKmdld",
                SecretKey= "AKIASGO577SSPQDFLQMT",
                EndPoint= "https://sellingpartnerapi-na.amazon.com",
                ServiceName= "ec2",
            };
            var header = new RequestHeader() {
                Host = "ec2.amazonaws.com"
            };

            _client = new SellingPartnerClient(config, header);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ClientPostTest()
        {
            var para = new TestRequestParameter() { 
               Action= "DescribeRegions",
               Version= "2013-10-15"
            };

           var res= await _client.PostAsync(new TestRequest(para,""));

            Assert.Equal("", "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ClientGetTest()
        {
            var para = new TestRequestParameter()
            {
                Action = "DescribeRegions",
                Version = "2013-10-15"
            };

            var res = await _client.GetAsync(new TestRequest(para,""));

            Assert.Equal("", "");
        }



        //[Fact]
        //public void Test1()
        //{

        //    List<string> result = new List<string>();
        //    var header = new RequestHeader();

        //    Type type = header.GetType();
        //    var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        //    var attrs=type.GetCustomAttributes();
        //    foreach (var property in props)
        //    {
        //        var attr = Attribute.GetCustomAttribute(property, typeof(PropertieNameAttribute));
        //        if (attr != null)
        //        {
        //            result.Add(property.Name);
        //        }
        //    }
        //    foreach (var item in attrs)
        //    {
        //        //Console.WriteLine(item.Name);
        //    }
        //}
    }
    //[PropertieName("RequestHeader")]
    //public class RequestHeader
    //{
    //    /// <summary>
    //    /// ��ͬ����ĵ�ַ:SellingPartner or Advertise etc
    //    /// </summary>
    //    [PropertieName("Host")]
    //    public string Host { get; set; } = "";
    //    /// <summary>
    //    /// ContentType
    //    /// </summary>
    //    [PropertieName("Content-Type")]
    //    public string ContentType { get; set; } = "application/x-www-form-urlencoded; charset=utf-8";
    //    /// <summary>
    //    /// RequestDate
    //    /// </summary>
    //    [PropertieName("X-Amz-Date")]
    //    public DateTime XAmzDate { get; set; }
    //}

    //public class PropertieNameAttribute : Attribute
    //{
    //    public String Name { get; set; }

    //    public PropertieNameAttribute()
    //    {

    //    }

    //    public PropertieNameAttribute(String name)
    //    {
    //        Name = name;
    //    }
    //}
}
