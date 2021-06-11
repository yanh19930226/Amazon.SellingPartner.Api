using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AmazonGather
{
    public class Cache
    {
        private static string savePath = AppDomain.CurrentDomain.BaseDirectory + "Cache";

        private static IDictionary<string, Product> ExpProductDic = null;

        public Cache()
        {
            if (!System.IO.Directory.Exists(savePath))
            {
                System.IO.Directory.CreateDirectory(savePath);
            }
        }

        private static IDictionary<string, Product> GetProduct()
        {
            var dic = new Dictionary<string, Product>();


            Product info = null;
            string shopId = "";


            using (FileStream fs = System.IO.File.Open(savePath + "\\CacheProDetailed.csv", FileMode.Open, FileAccess.Read))
            {
                StreamReader sr = new StreamReader(fs, Encoding.Default);
                int index = 0;
                while (!sr.EndOfStream)
                {
                    string _randline = sr.ReadLine();
                    var arr = Common.SplitStr(_randline);
                    //var arr = _randline.Split('\t');
                    if (arr.Length != 12)
                    {
                        //arr = (_randline + sr.ReadLine()).Split('\t');
                        arr = Common.SplitStr(_randline + sr.ReadLine());
                        if (arr.Length != 12)
                        {
                            Form1.SetMessage("解析错误：" + _randline);
                            continue;
                        }
                    }

                    if (arr != null && arr.Length > 0)
                    {
                        index++;
                        if (index == 1)
                        {
                            //标题
                            continue;
                        }
                        else
                        {
                            string _parentAsinid = arr[1].ToString().Trim();

                            if (!dic.Keys.Contains(_parentAsinid))
                            {
                                IList<ColorModel> ColorTextList = new List<ColorModel>();
                                if (!string.IsNullOrEmpty(arr[8]))
                                {
                                    ColorTextList.Add(new ColorModel()
                                    {
                                        AsinId = arr[0].ToString(),
                                        ColorName = arr[6].Trim(),
                                        SavePath = arr[7],
                                        Large = arr[8]
                                    });
                                }

                                info = new Product()
                                {
                                    AsinId = _parentAsinid,
                                    ColorList = ColorTextList,
                                    Title = arr[2].ToString(),
                                    OldTitle = arr[3].ToString(),
                                    Price = arr[4].ToString(),
                                    BrandName = arr[5].ToString(),
                                    Details = arr[9].Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries),
                                    RankList = new List<string>() { arr[10].ToString() },
                                    Url = arr[11],
                                    MarketplaceID = shopId
                                };

                                dic.Add(_parentAsinid, info);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(arr[8]))
                                {
                                    if (dic[_parentAsinid].ColorList.Where(obj => obj.AsinId == arr[0].ToString()).Count() == 0)
                                    {
                                        dic[info.AsinId].ColorList.Add(new ColorModel()
                                        {
                                            AsinId = arr[0].ToString(),
                                            ColorName = arr[6].Trim(),
                                            SavePath = arr[7],
                                            Large = arr[8]
                                        });
                                    }
                                }
                            }
                        }
                    }
                }//while
            }

            return dic;
        }

        public static void Remover(string asinId)
        {
            if (ExpProductDic.Keys.Contains(asinId))
            {
                ExpProductDic.Remove(asinId);
            }
        }

        public void ReadCache()
        {
            ExpProductDic = GetProduct();
        }

        /// <summary>
        /// 获取缓存内已经解析的商品信息
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Product GetCacheProSingle(string asinId)
        {
            if (ExpProductDic == null)
            {
                return null;
            }

            if (ExpProductDic.Keys.Contains(asinId))
            {
                return ExpProductDic[asinId];
            }
            else
            {
                return null;
            }
        }

        public static void SaveTemporaryFile()
        {
            try
            {
                string savePath = AppDomain.CurrentDomain.BaseDirectory + "/Cache";
                if (!System.IO.Directory.Exists(savePath))
                {
                    System.IO.Directory.CreateDirectory(savePath);
                }

                string fileName = savePath + "/CacheProDetailed.csv";
                if (System.IO.File.Exists(fileName))
                {
                    System.IO.File.Delete(fileName);
                }

                var expProDic = MerchantItems.ExpProductDic;
                WriteCSV(expProDic.Values.ToList(), fileName);
            }
            catch
            {

            }
        }

        private static void WriteCSV(IList<Product> list, string fileName)
        {
            Form1.SetMessage("开始生成临时存储文件...");
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fileName, false, Encoding.UTF8))
            {
                string title = "Asin,ParentAsin,标题,原标题,价格,品牌,颜色,图片路径,原图地址,简介,排名,商品地址";
                sw.Write(title);
                sw.WriteLine("");

                string _vals = "";
                foreach (var item in list)
                {
                    if (item.ColorList == null)
                    {
                        continue;
                    }

                    foreach (var _color in item.ColorList)
                    {
                        _vals = "";
                        _vals += _color.AsinId + "," + item.AsinId + ",\"" + ConvertString(item.Title) + "\",\"" + ConvertString(item.OldTitle) + "\"," + item.Price + ",\"" + ConvertString(item.BrandName) + "\"";

                        _vals += ",\"" + ConvertString(_color.ColorName) + "\"";
                        _vals += ",\"" + ConvertString(_color.SavePath) + "\"";
                        _vals += ",\"" + ConvertString(_color.Large) + "\"";

                        if (item.Details == null)
                        {
                            _vals += ",\"\"";
                        }
                        else
                        {
                            _vals += ",\"" + ConvertString(string.Join("|||", item.Details)) + "\"";
                        }

                        if (item.RankList == null)
                        {
                            _vals += ",\"\"";
                        }
                        else
                        {
                            _vals += ",\"" + ConvertString(string.Join("|||", item.RankList)) + "\"";
                        }

                        _vals += ",\"" + ConvertString(item.Url) + "\"";
                        sw.Write(_vals);
                        sw.WriteLine("");
                    }
                }

                sw.Flush();
            }

            Form1.SetMessage("*********************************************************");
            Form1.SetMessage("*********************************************************");
            Form1.SetMessage("");
            Form1.SetMessage("商品总数：" + list.Count);
            Form1.SetMessage("生成文件：" + fileName);
            Form1.SetMessage("");
            Form1.SetMessage("*********************************************************");
            Form1.SetMessage("*********************************************************");
        }

        static string ConvertString(string param)
        {
            if (string.IsNullOrEmpty(param))
            {
                return "";
            }

            return param.Replace("\"", "\"\"");
        }
    }
}
