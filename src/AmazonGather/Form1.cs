using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AmazonGather
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private int NormalCount = 0;

        private int MinTime = 0;
        
        private void button1_Click(object sender, EventArgs e)
        {
            int selId = comboBox1.SelectedIndex;
            if (selId == 2)
            {
                //下载图片
                ImageDown.AddedToDownload();
            }
            else
            {
                button1.Enabled = false;
                System.Threading.Thread thread1 = new System.Threading.Thread(obj=>Run(selId));
                thread1.Start();
            }

        }


        void Run(int selId)
        {
            if (selId == 3)
            {
                Cache cache = new Cache();
                cache.ReadCache();

            }

            var dic = ReadShop();
            if (dic.Count <= 0)
            {
                Form1.SetMessage("未获取到要执行的店铺！");
            }
            int i = 0;

            MerchantItems info = new MerchantItems();
            foreach (var item in dic)
            {
                i++;
                bool result = info.ReadWebPage(item.Key, item.Value);
                if (!result)
                {
                    SetMessage("两分钟后在重新尝试访问");
                    System.Threading.Thread.Sleep(1000 * 60 * 1);

                    for (int j = 0; j < 3; j++)
                    {
                        result = info.ReadWebPage(item.Key, item.Value);
                        if (result)
                        {
                            break;
                        }
                        SetMessage("三分钟后在重新尝试访问");
                        System.Threading.Thread.Sleep(1000 * 60 * 3);
                    }
                }

                if (!result)
                {
                    SetMessage("尝试三次都获取失败。跳过执行下一个！");
                    continue;
                }

                while (true)
                {
                    if (info.IsFinish())
                    {
                        var product = info.GetProductData();

                        if (product.Count > 0)
                        {
                            WriteCSV(product, "店铺[" + item.Key + "]商品_" + System.DateTime.Now.Millisecond + ".csv");
                        }
                        else
                        {
                            SetMessage("店铺[" + item.Key + "] :未读取到商品！");
                        }
                        break;
                    }
                }
            }
            //button1.Enabled = true;
        }


        public static void SetMessage(string param)
        {
            Console.WriteLine(param);
        }

        private static void WriteCSV(IList<Product> list, string fileName)
        {
           SetMessage("开始生成文件...");
            string BaseImgPath = Common.basePath + "ProductFile";
            if (!System.IO.Directory.Exists(BaseImgPath))
            {
                System.IO.Directory.CreateDirectory(BaseImgPath);
            }

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(BaseImgPath + "/" + fileName, false, Encoding.UTF8))
            {
                //string title = "Asin,ParentAsin,标题,原标题,价格,品牌,颜色,图片路径,原图地址";
                string title = "SPU,标题,价格,颜色,主图地址,款式,尺寸,描述,一级分类,二级分类,产品分类";
                //string title = "SPU,标题,价格,颜色,主图地址";

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
                        //_vals += _color.AsinId + "," + item.AsinId + ",\"" + ConvertString(item.Title) + "\",\"" + ConvertString(item.OldTitle) + "\"," + item.Price + ",\"" + ConvertString(item.BrandName) + "\"";
                        _vals += _color.AsinId + ",\"" + ConvertString(item.Title) + "\",\""+ item.Price.Replace("$"," ")+ "\",\"" + ConvertString(_color.ColorName) + "\",\"" + ConvertString(_color.Large) + "\"";
                        //_vals += ",\"" + ConvertString(_color.ColorName) + "\"";
                        //_vals += ",\"" + ConvertString(_color.SavePath) + "\"";
                        //_vals += ",\"" + ConvertString(_color.Large) + "\"";
                        _vals += ",\"\"";
                        _vals += ",\"\"";
                        if (item.Details == null)
                        {
                            _vals += ",\"\"";
                        }
                        else
                        {
                            _vals += ",\"" + ConvertString(string.Join("|||", item.Details)) + "\"";
                        }
                        _vals += ",\"\"";//一级分类
                        _vals += ",\"\"";//二级分类
                        _vals += ",\"\"";//品牌分类

                        //if (item.RankList == null)
                        //{
                        //    _vals += ",\"\"";
                        //}
                        //else
                        //{
                        //    _vals += ",\"" + ConvertString(string.Join("|||", item.RankList)) + "\"";
                        //}

                        //_vals += ",\"" + ConvertString(item.Url) + "\"";
                        sw.Write(_vals);
                        sw.WriteLine("");
                    }
                }

                sw.Flush();
            }

           SetMessage("*********************************************************");
           SetMessage("*********************************************************");
           SetMessage("");
           SetMessage("商品总数：" + list.Count);
           SetMessage("生成文件：" + fileName);
           SetMessage("");
           SetMessage("*********************************************************");
           SetMessage("*********************************************************");
        }

        static string ConvertString(string param)
        {
            if (string.IsNullOrEmpty(param))
            {
                return "";
            }

            return param.Replace("\"", "\"\"");
        }

        static IDictionary<string, string> ReadShop()
        {
            var dic = new Dictionary<string, string>();
            string path = Common.basePath + "ShopUrl.txt";
            string csvPath = Common.basePath + "ProductFile";
            if (!System.IO.File.Exists(path))
            {
               SetMessage("ShopUrl.txt文件为空。");
               SetMessage("文件内容一行保存一个网址");
               SetMessage("网址格式例如：https://www.amazon.com/s?me=A1TUFMIMFECDY9&amp;marketplaceID=ATVPDKIKX0DER");
                System.IO.File.Create(path);
            }
            else
            {
                string[] vals = System.IO.File.ReadAllLines(path, Encoding.UTF8);
                if (vals != null)
                {
                    int i = 0;
                    foreach (var item in vals)
                    {
                        if (string.IsNullOrEmpty(item))
                        {
                            continue;
                        }
                        i++;
                        string key = i.ToString();
                        int _startIndex = item.IndexOf("me=");
                        int _endIndex = item.IndexOf("&marketplaceID");
                        if (_startIndex > -1)
                        {
                            key = item.Substring(_startIndex + 3, (_endIndex - _startIndex - 3));
                        }

                        var csvFiles = System.IO.Directory.GetFiles(csvPath, "*" + key + "*.csv");
                        if (csvFiles != null && csvFiles.Length > 0)
                        {
                           SetMessage("店铺【" + key + "】已下载");
                            continue;
                        }

                        if (!dic.Keys.Contains(key))
                        {
                            dic.Add(key, item);
                        }
                    }
                }
            }

            return dic;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (MerchantItems.timer1 == null)
            {
                MessageBox.Show("任务未启动");
            }
            else
            {
                int period = 0;
                if (!int.TryParse(txtProTime.Text, out period))
                {
                    MessageBox.Show("读取商品间隔时间只能为整数");
                }
                else
                {
                    MinTime = period;
                    MerchantItems.timer1.Change(1000, period * 1000);
                    MessageBox.Show("读取商品间隔时间调整为:" + period + "秒");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MerchantItems.timerListPage == null)
            {
                MessageBox.Show("任务未启动");
            }
            else
            {
                int period = 0;
                if (!int.TryParse(txtListTime.Text, out period))
                {
                    MessageBox.Show("读取列表间隔时间只能为整数");
                }
                else
                {
                    MerchantItems.timerListPage.Change(1000, period * 1000);
                    MessageBox.Show("读取列表间隔时间只能为整数:" + period + "秒");
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MerchantItems.RunTime = dateTimePicker1.Value;
            MessageBox.Show("下次运行时间调整为:" + MerchantItems.RunTime);
            Console.WriteLine("Running Time:" + MerchantItems.RunTime);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                MerchantItems.IsDownImg = true;
                MessageBox.Show("同步下载图片");
            }
            else
            {
                MerchantItems.IsDownImg = false;
                MessageBox.Show("取消下载图片");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Console.WriteLine("生成缓存文件");
            Cache.SaveTemporaryFile();
        }

    }
}
