using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmazonGather
{
    public static class ExtObject
    {
        /// <summary>
        /// Json查询
        /// 查询默认第一层级
        /// </summary>
        /// <param name="value">json字符串</param>
        /// <param name="name">属性名</param>
        /// <returns></returns>
        public static string Get(this string value, string name)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(value)[name].ToString();
        }

        public static List<object> GetList(this string value)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<object>>(value);
        }
    }
}
