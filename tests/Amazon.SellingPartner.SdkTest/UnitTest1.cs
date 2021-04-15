using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Amazon.SellingPartner.SdkTest
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {

            List<string> result = new List<string>();
            var header = new RequestHeader();

            Type type = header.GetType();
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var attrs=type.GetCustomAttributes();
            foreach (var property in props)
            {
                var attr = Attribute.GetCustomAttribute(property, typeof(PropertieNameAttribute));
                if (attr != null)
                {
                    result.Add(property.Name);
                }
            }
            foreach (var item in attrs)
            {
                //Console.WriteLine(item.Name);
            }
        }
    }
    [PropertieName("RequestHeader")]
    public class RequestHeader
    {
        /// <summary>
        /// 不同服务的地址:SellingPartner or Advertise etc
        /// </summary>
        [PropertieName("Host")]
        public string Host { get; set; } = "";
        /// <summary>
        /// ContentType
        /// </summary>
        [PropertieName("Content-Type")]
        public string ContentType { get; set; } = "application/x-www-form-urlencoded; charset=utf-8";
        /// <summary>
        /// RequestDate
        /// </summary>
        [PropertieName("X-Amz-Date")]
        public DateTime XAmzDate { get; set; }
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
}
