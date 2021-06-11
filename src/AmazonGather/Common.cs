using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace AmazonGather
{
    public class Common
    {
        /// <summary>
        /// 读取失败重复提交最多次数；
        /// 如果超过则记录不在进行读取。
        /// 次数：3
        /// </summary>
        public static int ErrorCount = 3;

        /// <summary>
        /// 已下载
        /// </summary>
        public static int LoadFile = 0;

        /// <summary>
        /// 总文件数
        /// </summary>
        public static int TotalFile = 0;

        /// <summary>
        /// 页面读取的时间间隔(毫秒)
        /// </summary>
        public static int PageInterval = 7000;

        /// <summary>
        /// 列表读取的时间间隔(毫秒)
        /// </summary>
        public static int ListInterval = (1000 * 60 * 1);

        /// <summary>
        /// 累计错误的次数
        /// </summary>
        public static int TotalErrorCount = 0;

        public static string basePath = AppDomain.CurrentDomain.BaseDirectory;

        static string ListErrorPath = "ErrorList";
        static string PageErrorPath = "ErrorPage";
        static string ImgErrorPath = "ErrorImg";

        public static Random _random = new Random();

        /// <summary>
        /// 列表读取错误日志
        /// </summary>
        /// <param name="list"></param>
        public static void WriteListPage(WebInfo info)
        {
            string path = basePath + ListErrorPath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            WriteLog(path + "/" + System.DateTime.Now.ToString("yyyy-MM-dd") + ".txt", "\r\n" + Newtonsoft.Json.JsonConvert.SerializeObject(info));
        }

        /// <summary>
        /// 页面读取错误日志
        /// </summary>
        /// <param name="list"></param>
        public static void WritePage(WebInfo info)
        {
            string path = basePath + PageErrorPath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            WriteLog(path + "/" + System.DateTime.Now.ToString("yyyy-MM-dd") + ".txt", "\r\n" + Newtonsoft.Json.JsonConvert.SerializeObject(info));
        }

        /// <summary>
        /// 下载失败图片Log
        /// </summary>
        /// <param name="param">图片路径|保存路径</param>
        public static void WriteImg(string param)
        {
            string path = basePath + ImgErrorPath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            WriteLog(path + "/" + System.DateTime.Now.ToString("yyyy-MM-dd") + ".txt", "\r\n" + param);
        }


        public static void WriteLog(string path, string content)
        {
            System.IO.File.AppendAllText(path, content, Encoding.UTF8);
        }

        public static string StripHTML(string strHtml)
        {
            string[] aryReg =
            {
              @"<script[^>]*?>.*?</script>",
              @"<(\/\s*)?!?((\w+:)?\w+)(\w+(\s*=?\s*(([""'])(\\[""'tbnr]|[^\7])*?\7|\w+)|.{0})|\s)*?(\/\s*)?>", @"([\r\n])[\s]+", @"&(quot|#34);", @"&(amp|#38);", @"&(lt|#60);", @"&(gt|#62);", @"&(nbsp|#160);", @"&(iexcl|#161);", @"&(cent|#162);", @"&(pound|#163);",@"&(copy|#169);", @"&#(\d+);", @"-->", @"<!--.*\n"
            };

            string[] aryRep =
            {
              "", "", "", "\"", "&", "<", ">", "   ", "\xa1",  //chr(161),
              "\xa2",  //chr(162),
              "\xa3",  //chr(163),
              "\xa9",  //chr(169),
              "", "\r\n", ""
            };

            string newReg = aryReg[0];
            string strOutput = strHtml;
            for (int i = 0; i < aryReg.Length; i++)
            {
                Regex regex = new Regex(aryReg[i], RegexOptions.IgnoreCase);
                strOutput = regex.Replace(strOutput, aryRep[i]);
            }
            strOutput.Replace("<", "");
            strOutput.Replace(">", "");
            strOutput.Replace("\r\n", "");
            return strOutput;
        }


        public static NewWebClient WClient()
        {
            NewWebClient webClient = new NewWebClient();
            webClient.Timeout = 1000 * 60;
            webClient.DownloadProgressChanged += new System.Net.DownloadProgressChangedEventHandler(webClient_DownloadProgressChanged);
            webClient.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(webClient_DownloadFileCompleted);
            return webClient;
        }

        static void webClient_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            Form1.SetMessage(string.Format("正在下载文件，完成进度{0}%  {1}/{2}(字节)"
                                , e.ProgressPercentage
                                , e.BytesReceived
                                , e.TotalBytesToReceive));
        }

        static void webClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            LoadFile++;

            int percent = (int)(100.0 * LoadFile / TotalFile);
            Form1.SetMessage(string.Format("已完成文件下载{0}%  {1}/{2}(文件个数)"
                , percent
                , LoadFile
                , TotalFile));


            if (sender is System.Net.WebClient)
            {
                ((System.Net.WebClient)sender).CancelAsync();
                ((System.Net.WebClient)sender).Dispose();
                if (LoadFile == TotalFile)
                {
                    Form1.SetMessage("*****************************************");
                    Form1.SetMessage("图片已下载完成！" + LoadFile + "/" + TotalFile);
                    Form1.SetMessage("*****************************************");
                }
            }
        }

        /// <summary>
        ///  Cookie 
        /// </summary>
        public static CookieContainer _cookies = new CookieContainer();

        //如果你需要清除cookie 
        public static void ClearCookies(string url, string type = "")
        {
            _cookies = new CookieContainer();
            //int i = _random.Next(1, 10);
            //if (i < 2)
            //{
            //    string cookiesStart = "x-wl-uid=13+pWkaTbhrqmWfistQvRbFQwVFCG6orc1Mqh/CjiAwVdaBT2HUrBE0EWsQz1viFLtjTj+Q8yC08=; aws-target-static-id=1478575308984-117863; s_dslv=1483597035043; regStatus=pre-register; _mkto_trk=id:365-EFI-026&token:_mch-amazon.com-1528796047329-41856; session-id=142-6033177-0005218; ubid-main=190-4542874-8710907; session-id-time=2082787201l; AMCV_4A8581745834114C0A495E2B%40AdobeOrg=-330454231%7CMCIDTS%7C17718%7CMCMID%7C60927906312903516384694826257749968273%7CMCOPTOUT-1530768098s%7CNONE%7CMCAID%7CNONE%7CvVersion%7C3.1.2; s_lv=1530760944767; lc-main=en_US; i18n-prefs=USD; s_pers=%20s_fid%3D41748341B20902D3-02152128FF549CF1%7C1712642977589%3B%20s_dl%3D1%7C1554791977591%3B%20gpv_page%3DUS%253AAZ%253ASOA-overview-sell%7C1554791977597%3B%20s_ev15%3D%255B%255B%2527AZUSSOA-sell%2527%252C%25271554790177605%2527%255D%255D%7C1712642977605%3B; sp-cdn=\"L5Z9:CN\"; session-token=GwpE0cZj611AtaUsET61pxrFWZmASLh+oCWmP7birDUSCEcZZatqLBqnx6Zj92YwbfmwYdhhz0s7inbKE/0womsvxDz1rTLjyZ+WjaoCBtM02FTAa/S+mXDQrNzK2xqVWzGEtCP6FyDbtSQ5E9JL/jEuUcW4jWFmJOQFF83rY3HslAETyArFw9cy86cC8Plr; csm-hit=adb:adblk_no&tb: ";
            //    TimeSpan ts = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1);
            //    string _TotalMilliseconds = ts.TotalMilliseconds.ToString();
            //    string val = "";
            //    if (type == "列表")
            //    {
            //        val = cookiesStart + "s-" + Guid.NewGuid().ToString("N").Substring(0, 20) + "|" + _TotalMilliseconds + "&t:" + _TotalMilliseconds;
            //    }
            //    else
            //    {
            //        val = cookiesStart + Guid.NewGuid().ToString("N").Substring(0, 20) + "+s-" + Guid.NewGuid().ToString("N").Substring(0, 20) + "|" + _TotalMilliseconds + "&t:" + _TotalMilliseconds;
            //    }

            //    _cookies.SetCookies(new Uri(url), val);
            //}
        }

        public static HtmlAgilityPack.HtmlDocument GetPageDoc(string url, out string vProxyIp)
        {
            vProxyIp = string.Empty;
            IWebProxy _WebProxy = null;
            string _proxyIp = ProxyIp.GetIpSingle();
            if (!string.IsNullOrEmpty(_proxyIp))
            {
                vProxyIp = _proxyIp;
                _WebProxy = new WebProxy()
                {
                    Address = new Uri("http://" + _proxyIp)
                };
            }

            ClearCookies(url);

            int i = _random.Next(0, 10);
            if (i < 4)
            {
                return GetPageDocV2(url, _WebProxy);
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);

            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
            HtmlAgilityPack.HtmlWeb.PreRequestHandler handler = delegate(HttpWebRequest request)
            {
                request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate";
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                request.CookieContainer = _cookies;
                request.Timeout = 30000;
                request.Method = "GET";
                if (_WebProxy != null)
                {
                    request.Proxy = _WebProxy;
                }
                //request.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/x-silverlight, application/x-ms-application, application/x-ms-xbap, application/vnd.ms-xpsdocument, application/xaml+xml, application/QVOD, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
                //request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
                request.Headers.Add("Accept-Language", "en-US,en;q=0.9");

                //request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022; MS-RTC LM 8; Alexa Toolbar)";
                request.Headers.Add("UA-CPU", "x86");
                //request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.86 Safari/537.36";
                //request.UserAgent = CacheUA.GetRdoUa();
                request.UserAgent = CacheUA.GetRdoUaSingle();
                request.KeepAlive = true;

                return true;
            };

            web.PreRequest += handler;

            return web.Load(url);
        }

        public static HtmlAgilityPack.HtmlDocument GetPageDocV2(string url, IWebProxy _WebProxy = null)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            string responseText = Get(url, Encoding.Default, _WebProxy);
            doc.LoadHtml(responseText);

            return doc;

        }

        private static bool CheckValidationResult(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate
                , System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors errors)
        {   // 总是接受 认证平台 服务器的证书
            return true;
        }

        /// <summary>
        /// 发起HTTP请求(get)
        /// </summary>
        /// <param name="web"></param>
        /// <param name="encodings"></param>
        /// <returns></returns>
        public static string Get(string web, Encoding encodings, IWebProxy _WebProxy = null)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);

            string outs = "";
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(web);
            request.Proxy = null;
            request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate";
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.CookieContainer = _cookies;
            request.Timeout = 30000;
            request.Method = "GET";
            if (_WebProxy != null)
            {
                request.Proxy = _WebProxy;
            }
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
            request.Headers.Add("Accept-Language", "en-CN,zh;q=0.9");
            request.Headers.Add("UA-CPU", "x86");
            request.UserAgent = CacheUA.GetRdoUaSingle();
            request.KeepAlive = true;

            using (WebResponse wr = request.GetResponse())
            {
                Stream newStream = wr.GetResponseStream();
                HttpWebResponse webResponse = (HttpWebResponse)wr;
                bool bRetVal = true;
                if (webResponse.StatusCode != HttpStatusCode.OK)
                    bRetVal = false;

                if (bRetVal)
                {
                    //创建接收回馈的字节流类
                    Stream receiveStream = webResponse.GetResponseStream();//得到回写的字节流
                    StreamReader readStream = new StreamReader(receiveStream, encodings);
                    outs = readStream.ReadToEnd();
                    readStream.Close();
                }
                if (webResponse != null)
                    webResponse.Close();
                //在这里对接收到的页面内容进行处理
                return outs;
            }
        }



        /// <summary>
        /// wgr 重新处理单元格中的英文逗号，避免前面的用逗号分隔出现的换格问题
        /// </summary>
        /// <param name="strdata"></param>
        /// <returns></returns>
        public static string[] SplitStr(string strdata)
        {
            strdata = strdata.Replace("\r", "").Replace("\"\"", "|&|");
            if (!strdata.EndsWith("\r") && !strdata.EndsWith(","))
            {
                strdata += ",";
            }
            ArrayList cells = new ArrayList();
            string str = "";
            bool flag = false;
            for (int i = 0; i < strdata.Length; i++)
            {
                char ch = strdata[i];

                if (ch == ',')
                {
                    if (!flag)
                    {
                        cells.Add(str.Replace("|&|", "\""));
                        str = "";
                    }
                    else
                        str += ch;
                }
                else if (ch == '\"')
                {
                    if ((++i < strdata.Length) && strdata[i] == '\"')
                    {
                        str += strdata[i];
                    }
                    else
                    {
                        --i;
                        flag = flag ? false : true;
                    }
                }
                else
                {
                    str += ch;
                }
            }
            return (string[])cells.ToArray(typeof(string));
        }

    }
}
