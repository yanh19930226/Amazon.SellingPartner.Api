using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmazonGather
{
    public class ProxyIp
    {
        public static List<mayidaili> IpList = new List<mayidaili>();

        public static List<string> ErrList = new List<string>();

        /// <summary>
        /// 获取代理 IP:端口
        /// </summary>
        /// <returns></returns>
        public static string GetIpSingle()
        {
            if (IpList.Count > 0)
            {
                var item = IpList.OrderBy(obj => obj.Count).FirstOrDefault();
                item.Count++;
                Form1.SetMessage("代理IP：" + item.host + ":" + item.port + "；次数:" + item.Count);
                return item.host + ":" + item.port;
            }
            else
            {
                return null;
            }
        }

        public static void ModifyError(string vProxyIp)
        {
            string[] ParamList = vProxyIp.Split(':');
            if (ParamList != null && ParamList.Length == 2)
            {
                var item = IpList.Where(obj => obj.host == ParamList[0]).FirstOrDefault();
                item.ErrCount++;
                if (item.ErrCount >= 3)
                {
                    IpList.RemoveAll(obj => obj.host == ParamList[0]);
                }
            }
        }

        public static void CheckSuccess(IList<mayidaili> list)
        {
            IList<mayidaili> newList = new List<mayidaili>();
            try
            {
                IpList.Clear();
                IpList.AddRange(list);
                //mayidaili info = null;
                //string url = "http://www.mayidaili.com/proxy/get-proxy-info/";
                //int pageSize = (list.Count / 50) + 1;
                //for (int i = 0; i < pageSize; i++)
                //{
                //    IList<mayidaili> cklist = list.Skip(i * 50).Take(50).ToList();
                //    string strPostdata = "proxys=" + System.Web.HttpUtility.UrlEncode(Newtonsoft.Json.JsonConvert.SerializeObject(list));
                //    string responseText = "";
                //    if (new cn.Util.HttpPost().PostX(url, "UTF-8", out responseText, strPostdata))
                //    {

                //        if (responseText.Get("info").Get("ok") == "True")
                //        {
                //            foreach (var item in responseText.Get("data").GetList())
                //            {
                //                info = new mayidaili()
                //                {
                //                    Count = 0,
                //                    port = item.ToString().Get("port"),
                //                    host = item.ToString().Get("host")
                //                };

                //                if (ErrList.Contains(info.host))
                //                {
                //                    continue;
                //                }

                //                if (!newList.Contains(info))
                //                {
                //                    newList.Add(info);
                //                }
                //            }
                //        }
                //        else
                //        {
                //            Form1.SetMessage("验证代理IP失败：" + responseText);
                //        }
                //    }
                //    else
                //    {
                //        Form1.SetMessage("检测代理IP失败");
                //    }
                //}

                //if (newList != null && newList.Count > 0)
                //{
                //    IpList.Clear();
                //    IpList.AddRange(list);
                //    Form1.SetMessage("获取[" + list.Count + "]个代理IP");
                //}
                //else
                //{
                //    Form1.SetMessage("获取[0]个代理IP");
                //}

            }
            catch (Exception ex)
            {
                Form1.SetMessage("检测代理IP失败；" + ex.Message);
            }
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class mayidaili
    {
        [JsonProperty]
        public string host { get; set; }

        [JsonProperty]
        public string port { get; set; }

        /// <summary>
        /// 运行次数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 运行错误次数
        /// </summary>
        public int ErrCount { get; set; }

    }
}
