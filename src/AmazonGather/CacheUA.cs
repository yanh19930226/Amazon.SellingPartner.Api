using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmazonGather
{
    public class CacheUA
    {
        public static List<string> UAList = new List<string>();

        public static void Reset()
        {
            if (UAList.Count <= 0)
            {
                try
                {
                    string filePath = Common.basePath + "/UserAgentList";
                    string[] files = System.IO.Directory.GetFiles(filePath, "*.txt");
                    foreach (var item in files)
                    {
                        UAList.AddRange(System.IO.File.ReadAllLines(item));
                    }
                    Form1.SetMessage("共获取到【" + UAList.Count + "】个User-Agent-List");
                }
                catch (Exception ex)
                {
                    Form1.SetMessage("读取User-Agent-List缓存错误；" + ex.Message);
                }
                finally
                {
                    if (UAList.Count <= 0)
                    {
                        Form1.SetMessage("读取缓存失败，添加默认的UA；");
                        UAList.Add("Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.86 Safari/537.36");
                    }
                }
            }
        }

        public static string GetRdoUa()
        {
            if (UAList.Count <= 0)
            {
                Reset();
            }
            Random rdo = new Random();
            int i = rdo.Next(0, UAList.Count);

            return UAList[i];
        }

        /// <summary>
        /// 留存
        /// </summary>
        private static List<string> TransferList = new List<string>();

        public static string GetRdoUaSingle()
        {
            if (UAList.Count <= 0)
            {
                Form1.SetMessage("重置UA");
                if (TransferList.Count > 0)
                {
                    UAList.AddRange(TransferList);
                    TransferList.Clear();
                }
                else
                {
                    Reset();
                }
            }

            Random rdo = new Random();
            int i = rdo.Next(0, UAList.Count);
            string ua = UAList[i];
            UAList.RemoveAt(i);
            TransferList.Add(ua);
            Form1.SetMessage("剩余UA【" + UAList.Count + "】");

            return ua;
        }
    }
}
