using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmazonGather
{
    public class MerchantItems
    {
        /// <summary>
        /// 所有的商品页URL
        /// </summary>
        public IList<WebInfo> PageList = new List<WebInfo>();

        /// <summary>
        /// 所有商品详细页集合
        /// ASIN,WebInfo
        /// </summary>
        public IDictionary<string, WebInfo> ProDic = new Dictionary<string, WebInfo>();

        /// <summary>
        /// 获取到的商品信息
        /// </summary>
        public static IDictionary<string, Product> ExpProductDic = new Dictionary<string, Product>();

        /// <summary>
        /// 定时解析商品详细页面
        /// </summary>
        public static System.Threading.Timer timer1 = null;

        /// <summary>
        /// 定时解析商品列表页面
        /// </summary>
        public static System.Threading.Timer timerListPage = null;

        /// <summary>
        /// 是否正在分析列表页
        /// </summary>
        bool ReadListPage = false;

        /// <summary>
        /// 统计消息
        /// </summary>
        StatisticsMessage SMessage = null;
        
        /// <summary>
        /// 下次运行时间；
        /// 默认最小值，则允许运行
        /// </summary>
        public static DateTime RunTime = DateTime.MinValue;

        /// <summary>
        /// 是否同步下载图片
        /// </summary>
        public static bool IsDownImg = true;

        System.ComponentModel.BackgroundWorker dWorker = new System.ComponentModel.BackgroundWorker();

        /// <summary>
        /// 初始化
        /// </summary>
        public void ClearValue()
        {
            SMessage = new StatisticsMessage();
            SMessage.ClearValue();
            ExpProductDic.Clear();
            ProDic.Clear();
            PageList.Clear();
            timer1 = null;
            timerListPage = null;
            dWorker.DoWork += dWorker_DoWork;
        }

        /// <summary>
        /// 让程序暂停执行
        /// </summary>
        /// <returns></returns>
        bool RunSleep()
        {
            bool _IsRun = false;
            if (RunTime == DateTime.MinValue || RunTime < System.DateTime.Now)
            {
                //Common.ClearCookies();
                RunTime = DateTime.MinValue;
                _IsRun = true;
            }
            else
            {
                _IsRun = false;
            }

            return _IsRun;
        }

        /// <summary>
        /// 暂停设置
        /// </summary>
        void PuaseRun()
        {
            Common.TotalErrorCount++;
            if (Common.TotalErrorCount >= 10)
            {
                //当错误累计到"10"的时候计数器重置未"1"
                Common.TotalErrorCount = 1;
            }
            Random random = new Random();
            int num = random.Next(4, (Common.TotalErrorCount + 4));
            RunTime = System.DateTime.Now.AddMinutes(num);
            Console.WriteLine("下次运行时间：" + RunTime.ToString("yyyy-MM-dd HH:mm:ss"));
        }


        /// <summary>
        /// 获取所有页面
        /// </summary>
        /// <param name="url"></param>
        /// <param name="marketplaceID"></param>
        public bool ReadWebPage(string marketplaceID, string url)
        {
            ClearValue();
            SMessage.TotalPageCount = 1;
            Console.WriteLine("开始获取商品列表页面；");
            Console.WriteLine("URL:" + url);
            Console.WriteLine("......");
            HtmlAgilityPack.HtmlNode SerchNode = null;
            bool IsSuccess = false;
            //代理IP
            string vProxyIp = string.Empty;

            try
            {
                var doc = Common.GetPageDoc(url, out vProxyIp);
                SerchNode = doc.GetElementbyId("search");
                if (SerchNode != null)
                {
                    var productPage = SerchNode.SelectSingleNode(".//li[@class='a-last']");
                    if (productPage == null)
                    {
                        SMessage.IsNextPage = true;
                        SMessage.TotalPageCount++;
                        PageList.Add(new WebInfo()
                        {
                            AsinId = "",
                            Url = url,
                            Message = "",
                            ErrorCount = 0,
                            PageType = PageTypes.List,
                            IsSuccess = false,
                            MarketplaceID = marketplaceID
                        });
                    }
                    else
                    {
                        string nextUrl = productPage.SelectSingleNode("a").Attributes["href"].Value;
                        var _totalPageNodel = SerchNode.SelectNodes(".//li[@class='a-disabled']");
                        int _totalPageCount = 0; //总页数
                        foreach (var item in _totalPageNodel)
                        {
                            if (int.TryParse(item.InnerText, out _totalPageCount))
                            {
                                break;
                            }
                        }

                        if (_totalPageCount > 0)
                        {
                            int startPage = nextUrl.IndexOf("&amp;page=");
                            int endPage = nextUrl.IndexOf("&amp;marketplaceID");
                            string startPageStr = nextUrl.Substring(0, startPage) + "&amp;page=";
                            string endPageStr = nextUrl.Substring(endPage);
                            string webUrl = url.Substring(0, url.IndexOf(".com")) + ".com";
                            for (int i = 1; i <= _totalPageCount; i++)
                            {
                                SMessage.TotalPageCount++;
                                PageList.Add(new WebInfo()
                                {
                                    AsinId = "",
                                    Url = webUrl + startPageStr + i + endPageStr,
                                    Message = "",
                                    ErrorCount = 0,
                                    PageType = PageTypes.List,
                                    IsSuccess = false,
                                    MarketplaceID = marketplaceID
                                });
                            }
                        }

                    }//productPage == null

                    IsSuccess = true;
                }
                else
                {
                    Console.WriteLine("未读取到商品页");
                    IsSuccess = false;
                }//SerchNode
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                string logerror = "\r\n*******************ReadWeb****************************\r\n";
                logerror += "商品列表页：" + url;
                logerror += "\r\n错误：" + ex.Message;
                Common.WriteLog(Common.basePath + "/log.txt", logerror);
                Console.WriteLine("商品列表获取错误:" + logerror);
            }
            finally
            {
                if (IsSuccess == true)
                {
                    Console.WriteLine("==========================================");
                    Console.WriteLine("=== 总共获取到[" + PageList.Count + "]页！ ===");
                    Console.WriteLine("==========================================");

                    if (timerListPage == null && PageList.Count > 0)
                    {
                        timerListPage = new System.Threading.Timer(obj => timerListPage_Tick(), null, 0, Common.ListInterval);
                    }
                }
                else
                {
                    ProxyIp.ModifyError(vProxyIp);
                }
            }

            return IsSuccess;
        }

        void timerListPage_Tick()
        {
            if (!RunSleep())
            {
                return;
            }

            if (ProDic.Count > 50)
            {
                return;
            }

            if (dWorker.IsBusy)
            {
                return;
            }

            if (!ReadListPage)
            {
                if (PageList.Count <= 0)
                {
                    timerListPage.Dispose();
                    timerListPage = null;
                    Console.WriteLine("============================");
                    Console.WriteLine("=== 列表页解析完成！ ===");
                    Console.WriteLine("============================");
                }
                else
                {
                    ReadListPage = true;
                    var item = PageList.FirstOrDefault();
                    PageList.Remove(item);
                    dWorker.RunWorkerAsync(item);
                    Console.WriteLine("\r\n未处理列表页：" + PageList.Count);
                }
            }
        }

        void dWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            var info = e.Argument as WebInfo;
            if (info.PageType == PageTypes.Page)
            {
                ReadProduct(info);
            }
            else
            {
                ReadWeb(info);
            }
        }

        /// <summary>
        /// 读取列表中的商品
        /// </summary>
        /// <param name="info"></param>
        public void ReadWeb(WebInfo info)
        {
            Console.WriteLine("开始解析商品列表页面；");
            Console.WriteLine("URL:" + info.Url);
            Console.WriteLine("......");
            HtmlAgilityPack.HtmlNode SerchNode = null;
            //代理IP
            string vProxyIp = string.Empty;

            try
            {
                var doc = Common.GetPageDoc(info.Url, out vProxyIp);
                SerchNode = doc.GetElementbyId("search");
                if (SerchNode != null)
                {
                    var S_Result_List = SerchNode.SelectNodes("//*[@id='search']/div[1]/div/div[1]/div/span[3]/div[2]/div[@data-uuid]");
                    if (S_Result_List != null)
                    {
                        string webUrl = info.Url.Substring(0, info.Url.IndexOf(".com")) + ".com";
                        foreach (var item in S_Result_List)
                        {
                            string asinId = item.Attributes["data-asin"].Value;
                            string url = item.SelectSingleNode(".//a[@class='a-link-normal']").Attributes["href"].Value;

                            if (ExpProductDic.Keys.Contains(asinId))
                            {
                                //已经获取了，直接跳过
                                continue;
                            }

                            if (!ProDic.Keys.Contains(asinId))
                            {
                                ProDic.Add(asinId, new WebInfo()
                                {
                                    AsinId = asinId,
                                    ErrorCount = 0,
                                    IsSuccess = false,
                                    Message = "",
                                    PageType = PageTypes.Page,
                                    Url = webUrl + url,
                                    MarketplaceID = info.MarketplaceID
                                });
                                SMessage.TotalProductCount++;
                            }
                        }
                    }

                    info.IsSuccess = true;
                }
                else
                {
                    info.ErrorCount++;
                    info.Message = "未获取到ID(search)";
                    info.IsSuccess = false;
                    Console.WriteLine("未获取到ID(search)");
                }
            }
            catch (Exception ex)
            {
                info.ErrorCount++;
                info.IsSuccess = false;
                string logerror = "\r\n*******************ReadWeb****************************\r\n";
                logerror += "商品列表页：" + info.Url;
                logerror += "\r\n错误：" + ex.Message;
                info.Message = ex.Message;

                Console.WriteLine("商品列表页URL：" + info.Url + "；获取错误");
                Console.WriteLine("错误：" + logerror);
            }
            finally
            {
                ReadListPage = false;
                if (info.IsSuccess == false)
                {
                    ProxyIp.ModifyError(vProxyIp);
                    if (info.ErrorCount < Common.ErrorCount)
                    {
                        if (!PageList.Contains(info))
                        {
                            PageList.Add(info);
                        }
                    }
                    else
                    {
                        Common.WriteListPage(info);
                        Console.WriteLine("商品URL：" + info.Url + "；[" + Common.ErrorCount + "]次都获取错误；停止解析。");
                    }

                    PuaseRun();
                }
                else
                {
                    if (timer1 == null)
                    {
                        timer1 = new System.Threading.Timer(obj => timer1_Tick(), null, 0, Common.PageInterval);
                    }
                }
            }

        }

        private void timer1_Tick()
        {
            if (!RunSleep())
            {
                return;
            }

            if (dWorker.IsBusy)
            {
                return;
            }

            if (ProDic.Count == 0 && SMessage.IsNextPage == true)
            {
                timer1.Dispose();
                timer1 = null;
                Console.WriteLine("============================");
                Console.WriteLine("    成功：" + SMessage.SuccessProcudtCount + " 个商品");
                Console.WriteLine("    失败：" + (SMessage.TotalProductCount - SMessage.SuccessProcudtCount) + " 个商品");
                Console.WriteLine("    总共：[" + SMessage.TotalPageCount + "]页；" + SMessage.TotalProductCount + " 个商品");
                Console.WriteLine("=== 商品读取定时器停止！ ===");
                Console.WriteLine("============================");
            }
            else
            {
                if (ProDic.Count > 0)
                {
                    Console.WriteLine("\r\n未处理商品：" + ProDic.Count);
                    var item = ProDic.FirstOrDefault();
                    ProDic.Remove(item.Key);
                    dWorker.RunWorkerAsync(item.Value);
                }
            }
        }

        /// <summary>
        /// 读取商品详细信息
        /// </summary>
        /// <param name="info"></param>
        public void ReadProduct(WebInfo info)
        {
            string vProxyIp = string.Empty;
            try
            {
                #region Cache

                var _cachePro = Cache.GetCacheProSingle(info.AsinId);
                if (_cachePro != null)
                {
                    info.IsSuccess = true;
                    _cachePro.MarketplaceID = info.MarketplaceID;

                    if (!ExpProductDic.Keys.Contains(_cachePro.AsinId))
                    {
                        ExpProductDic.Add(_cachePro.AsinId, _cachePro);
                    }

                    if (_cachePro.ColorList != null && _cachePro.ColorList.Count > 0)
                    {
                        foreach (var item in _cachePro.ColorList)
                        {
                            DownImage(item, _cachePro.AsinId, info.MarketplaceID);
                        }
                    }

                    Cache.Remover(info.AsinId);
                    System.Console.WriteLine("商品ASIN：" + info.AsinId + "；缓存读取成功！");

                    return;
                }


                #endregion Cache
                var doc = Common.GetPageDoc(info.Url, out vProxyIp);
                if (doc.GetElementbyId("productTitle") == null)
                {
                    throw new Exception("产品页面获取失败！请查看浏览器打开商品是否需要验证。");
                }

                Product product = new Product()
                {
                    OldTitle = doc.GetElementbyId("productTitle").InnerText.Trim(),
                    AsinId = info.AsinId,
                    Url = info.Url,

                    MarketplaceID = info.MarketplaceID
                };

                if (doc.GetElementbyId("priceblock_ourprice") != null)
                {
                    product.Price = doc.GetElementbyId("priceblock_ourprice").InnerText.Trim();
                }
                else
                {
                    var _priceNode = doc.GetElementbyId("tmmSwatches");
                    if (_priceNode != null)
                    {
                        _priceNode = _priceNode.SelectSingleNode(".//span[@class='a-size-base a-color-price a-color-price']");
                        if (_priceNode != null)
                        {
                            product.Price = Common.StripHTML(_priceNode.InnerHtml).Trim();
                        }
                    }
                }


                if (doc.GetElementbyId("bylineInfo") != null)
                {
                    product.BrandName = doc.GetElementbyId("bylineInfo").InnerText.Trim();
                    product.Title = Common.StripHTML(product.OldTitle.Replace(product.BrandName, ""));
                }
                else
                {
                    product.Title = Common.StripHTML(product.OldTitle);
                }

                #region 简介
                //var detailsNodels = doc.GetElementbyId("feature-bullets");
                //if (detailsNodels != null)
                //{
                //    var a_list_item = detailsNodels.SelectNodes(".//span[@class='a-list-item']");
                //    if (a_list_item != null)
                //    {
                //        IList<string> _list = new List<string>();
                //        foreach (var item in a_list_item)
                //        {
                //            _list.Add(Common.StripHTML(item.InnerText.Trim()).Replace("\r\n", " "));
                //        }
                //        product.Details = _list;
                //    }
                //}
                #endregion

                #region 图片
                IList<ColorModel> ColorTextList = new List<ColorModel>();
                var ColorNodel = doc.GetElementbyId("variation_color_name");
                if (ColorNodel != null)
                {
                    var ColorsNodels = ColorNodel.SelectNodes(".//li[contains(@class,'swatchSelect') or contains(@class,'swatchAvailable')]");
                    if (ColorsNodels != null)
                    {
                        foreach (var item in ColorsNodels)
                        {
                            var _imgUrl = item.SelectSingleNode(".//img[@class='imgSwatch']");
                            if (_imgUrl != null)
                            {
                                ColorTextList.Add(new ColorModel()
                                {
                                    ColorName = _imgUrl.Attributes["alt"].Value.Trim(),
                                });
                            }
                        }
                    }
                    else
                    {
                        var singleColor = ColorNodel.SelectSingleNode(".//span[@class='selection']");
                        if (singleColor != null)
                        {
                            ColorTextList.Add(new ColorModel()
                            {
                                ColorName = singleColor.InnerText.Trim(),
                            });
                        }
                    }
                }

                var ImageNodel = doc.GetElementbyId("imageBlockVariations_feature_div");
                if (ImageNodel != null && ColorTextList.Count > 1)
                {
                    var _ImgScritp = ImageNodel.SelectSingleNode("script[1]");
                    var _forSize = doc.GetElementbyId("variation_size_name");  //选中的SIze
                    if (_ImgScritp != null)
                    {
                        string _scriptText = _ImgScritp.InnerText;
                        if (!string.IsNullOrEmpty(_scriptText))
                        {
                            string _startText = "jQuery.parseJSON(";
                            string _endText = "data[\"alwaysIncludeVideo\"]";
                            int _startIndex = _scriptText.IndexOf(_startText) + _startText.Length + 1;
                            int _endIndex = _scriptText.IndexOf(_endText);
                            var mainJson = _scriptText.Substring(_startIndex, (_endIndex - _startIndex)).Replace("}');", "}").Replace("\n');", "");
                            var colorImages = mainJson.Get("colorImages").Replace(@"\\", "");
                            //var colorImages = mainJson.Get("colorImages");
                            var colorAsin = mainJson.Get("colorToAsin").Replace(@"\\", "");
                            if (colorImages != null)
                            {
                                foreach (var item in ColorTextList)
                                {
                                    string _colorName = System.Web.HttpUtility.HtmlDecode(item.ColorName);

                                    var isbol = GetImgUrl(info.AsinId, _colorName, colorImages, colorAsin, item);
                                    if (isbol)
                                    {
                                        continue;
                                    }

                                    Console.WriteLine("获取颜色图片失败：" + info.AsinId);
                                    Console.WriteLine("颜色前加One Size再次查询：" + info.AsinId);
                                    isbol = GetImgUrl(info.AsinId, "One Size " + _colorName, colorImages, colorAsin, item);
                                    if (isbol)
                                    {
                                        continue;
                                    }
                                    isbol = GetImgUrl(info.AsinId, "one size " + _colorName, colorImages, colorAsin, item);
                                    if (isbol)
                                    {
                                        continue;
                                    }
                                    Console.WriteLine("获取颜色图片失败：" + info.AsinId);
                                    Console.WriteLine("颜色后加One Size再次查询：" + info.AsinId);
                                    isbol = GetImgUrl(info.AsinId, _colorName + " One Size", colorImages, colorAsin, item);
                                    if (isbol)
                                    {
                                        continue;
                                    }
                                    isbol = GetImgUrl(info.AsinId, _colorName + " one size", colorImages, colorAsin, item);
                                    if (isbol)
                                    {
                                        continue;
                                    }

                                    //特殊商品 B07KRWPJWJ  图片是  Size +color
                                    //Console.WriteLine("特殊商品获取颜色处理 Size+Color：" + info.AsinId);
                                    //if (_forSize != null)
                                    //{
                                    //    var _selSize = _forSize.SelectSingleNode(".//span[@class='selection']");
                                    //    if (_selSize != null)
                                    //    {
                                    //        string _selSizeHtm = _selSize.InnerHtml.Trim();
                                    //        if (_selSizeHtm.IndexOf("&#") > -1)
                                    //        {
                                    //            _selSizeHtm = System.Web.HttpUtility.HtmlDecode(_selSizeHtm);
                                    //        }

                                    //        isbol = GetImgUrl(info.AsinId, _selSizeHtm + " " + _colorName, colorImages, colorAsin, item);
                                    //        if (isbol)
                                    //        {
                                    //            continue;
                                    //        }

                                    //        isbol = GetImgUrl(info.AsinId, _colorName + " " + _selSizeHtm, colorImages, colorAsin, item);
                                    //        if (isbol)
                                    //        {
                                    //            continue;
                                    //        }

                                    //        isbol = GetImgUrl(info.AsinId, _selSizeHtm.Replace("\"", "\\\"") + " " + _colorName, colorImages, colorAsin, item);
                                    //        if (isbol)
                                    //        {
                                    //            continue;
                                    //        }

                                    //        isbol = GetImgUrl(info.AsinId, _colorName + " " + _selSizeHtm.Replace("\"", "\\\""), colorImages, colorAsin, item);
                                    //        if (isbol)
                                    //        {
                                    //            continue;
                                    //        }
                                    //    }

                                    //    //尺寸不存在 或者尺寸缺货
                                    //    var _sizeLi = _forSize.SelectNodes(".//li[@class='swatchAvailable']");
                                    //    if (_sizeLi != null)
                                    //    {
                                    //        var _selSizeHtm = _sizeLi[0].SelectSingleNode(".//span[@class='a-size-base']").InnerHtml.Trim();
                                    //        if (_selSizeHtm.IndexOf("&#") > -1)
                                    //        {
                                    //            _selSizeHtm = System.Web.HttpUtility.HtmlDecode(_selSizeHtm);
                                    //        }

                                    //        isbol = GetImgUrl(info.AsinId, _selSizeHtm + " " + _colorName, colorImages, colorAsin, item);
                                    //        if (isbol)
                                    //        {
                                    //            continue;
                                    //        }

                                    //        isbol = GetImgUrl(info.AsinId, _colorName + " " + _selSizeHtm, colorImages, colorAsin, item);
                                    //        if (isbol)
                                    //        {
                                    //            continue;
                                    //        }

                                    //        isbol = GetImgUrl(info.AsinId, _selSizeHtm.Replace("\"", "\\\"") + " " + _colorName, colorImages, colorAsin, item);
                                    //        if (isbol)
                                    //        {
                                    //            continue;
                                    //        }

                                    //        isbol = GetImgUrl(info.AsinId, _colorName + " " + _selSizeHtm.Replace("\"", "\\\""), colorImages, colorAsin, item);
                                    //        if (isbol)
                                    //        {
                                    //            continue;
                                    //        }
                                    //    }

                                    //}

                                    if (string.IsNullOrEmpty(item.Large) && ColorTextList.Count > 1)
                                    {
                                        continue;
                                        //throw new Exception("ASIN[" + info.AsinId + "]错误：未获取到图片地址");
                                    }

                                }//ColorTextList
                            }
                        }
                    }
                }
                else if (ColorTextList.Count == 1)
                {
                    ImageNodel = doc.GetElementbyId("landingImage");
                    if (ImageNodel != null)
                    {
                        if (string.IsNullOrEmpty(ImageNodel.Attributes["data-old-hires"].Value))
                        {
                            var imgList = ImageNodel.Attributes["data-a-dynamic-image"].Value;
                            imgList = imgList.Replace("&gt;", ">");
                            imgList = imgList.Replace("&lt;", "<");
                            imgList = imgList.Replace("&nbsp;", " ");
                            imgList = imgList.Replace("&quot;", "\"");
                            imgList = imgList.Replace("&#39;", "\'");
                            imgList = imgList.Replace("\\\\", "\\");//对斜线的转义
                            imgList = imgList.Replace("\\n", "\n");
                            imgList = imgList.Replace("\\r", "\r");
                            var dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, int[]>>(imgList);
                            int imgSize = 0;
                            string imgUrl = "";
                            foreach (var item1 in dic)
                            {
                                int size = item1.Value[0];
                                if (imgSize < size)
                                {
                                    imgSize = size;
                                    imgUrl = item1.Key;
                                }
                            }

                            if (!string.IsNullOrEmpty(imgUrl))
                            {
                                ColorTextList[0].AsinId = info.AsinId;

                                int qian = imgUrl.IndexOf("._");
                                if (qian > 0)
                                {
                                    string _imgurl2 = imgUrl.Substring(0, qian) + ".jpg";
                                    ColorTextList[0].Large = _imgurl2;
                                }
                                else
                                {
                                    ColorTextList[0].Large = imgUrl;
                                }
                            }
                            else
                            {
                                throw new Exception("ASIN[" + info.AsinId + "]错误：未获取到图片地址");
                            }
                        }
                        else
                        {
                            ColorTextList[0].AsinId = info.AsinId;
                            ColorTextList[0].Large = ImageNodel.Attributes["data-old-hires"].Value;
                        }
                    }
                }

                if (ColorTextList.Count <= 0)
                {
                    ImageNodel = doc.GetElementbyId("landingImage");
                    if (ImageNodel != null)
                    {
                        if (string.IsNullOrEmpty(ImageNodel.Attributes["data-old-hires"].Value))
                        {
                            var imgList = ImageNodel.Attributes["data-a-dynamic-image"].Value;
                            imgList = imgList.Replace("&gt;", ">");
                            imgList = imgList.Replace("&lt;", "<");
                            imgList = imgList.Replace("&nbsp;", " ");
                            imgList = imgList.Replace("&quot;", "\"");
                            imgList = imgList.Replace("&#39;", "\'");
                            imgList = imgList.Replace("\\\\", "\\");//对斜线的转义
                            imgList = imgList.Replace("\\n", "\n");
                            imgList = imgList.Replace("\\r", "\r");
                            var dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, int[]>>(imgList);
                            int imgSize = 0;
                            string imgUrl = "";
                            foreach (var item in dic)
                            {
                                int size = item.Value[0];
                                if (imgSize < size)
                                {
                                    imgSize = size;
                                    imgUrl = item.Key;
                                }
                            }

                            if (!string.IsNullOrEmpty(imgUrl))
                            {
                                var _colorModel = new ColorModel()
                                {
                                    AsinId = info.AsinId,
                                    ColorName = "unknown",
                                };

                                int qian = imgUrl.IndexOf("._");
                                if (qian > 0)
                                {
                                    string _imgurl2 = imgUrl.Substring(0, qian) + ".jpg";
                                    _colorModel.Large = _imgurl2;
                                }
                                else
                                {
                                    _colorModel.Large = imgUrl;
                                }

                                ColorTextList.Add(_colorModel);
                            }
                            else
                            {
                                throw new Exception("ASIN[" + info.AsinId + "]错误：未获取到图片地址");
                            }
                        }
                        else
                        {
                            ColorTextList.Add(new ColorModel()
                            {
                                AsinId = info.AsinId,
                                Large = ImageNodel.Attributes["data-old-hires"].Value,
                                ColorName = "unknown"
                            });
                        }

                    }
                }

                product.ColorList = ColorTextList;

                #endregion

                #region 排名
                //var RankNodel = doc.GetElementbyId("SalesRank");
                //if (RankNodel != null)
                //{
                //    IList<string> RandList = new List<string>();
                //    string OneRank = RankNodel.InnerHtml;
                //    int b_index = OneRank.IndexOf("#");
                //    if (b_index > -1)
                //    {
                //        OneRank = OneRank.Substring(b_index);
                //        b_index = OneRank.IndexOf("in");
                //        if (b_index > -1)
                //        {
                //            OneRank = OneRank.Substring(0, b_index - 1);
                //            RandList.Add(Common.StripHTML(OneRank.Trim().Replace(",", "").Replace("#", "")));
                //        }
                //    }

                //    product.RankList = RandList;
                //}

                //if (RankNodel == null)
                //{
                //    RankNodel = doc.GetElementbyId("productDetails_detailBullets_sections1");
                //    if (RankNodel != null)
                //    {
                //        var trTags = RankNodel.SelectNodes("..//tr");
                //        if (trTags != null)
                //        {
                //            IList<string> RandList = new List<string>();
                //            foreach (var _tr in trTags)
                //            {
                //                var _th = _tr.SelectSingleNode(".//th").InnerText;
                //                _th = Common.StripHTML(_th).Trim().ToLower();
                //                if (_th == "best sellers rank")
                //                {
                //                    var _td = _tr.SelectSingleNode(".//td").InnerText;
                //                    _td = Common.StripHTML(_td);

                //                    int b_index = _td.IndexOf("#");
                //                    if (b_index > -1)
                //                    {
                //                        _td = _td.Substring(b_index);
                //                        b_index = _td.IndexOf("in");
                //                        if (b_index > -1)
                //                        {
                //                            _td = _td.Substring(0, b_index - 1);
                //                            RandList.Add(_td.Trim().Replace(",", "").Replace("#", ""));
                //                        }
                //                    }
                //                }
                //            }

                //            product.RankList = RandList;
                //        }//trTags
                //    }//RankNodel
                //}
                #endregion

                if (!ExpProductDic.Keys.Contains(product.AsinId))
                {
                    ExpProductDic.Add(product.AsinId, product);
                }

                #region 图片下载
                //if (product.ColorList != null && product.ColorList.Count > 0)
                //{
                //    foreach (var item in product.ColorList)
                //    {
                //        DownImage(item, product.AsinId, product.MarketplaceID);
                //    }
                //}
                #endregion

                info.IsSuccess = true;
            }
            catch (Exception ex)
            {
                info.ErrorCount++;
                info.IsSuccess = false;
                string logerror = "\r\n********************GetProductList***************************\r\n";
                logerror += "商品：" + info.Url;
                logerror += "\r\n错误：" + ex.Message;
                info.Message = ex.Message;
                Console.WriteLine("获取错误：" + logerror + "\r\n 获取错误,休眠一会儿");
            }
            finally
            {
                if (info.IsSuccess == false)
                {
                    ProxyIp.ModifyError(vProxyIp);
                    //读取失败加回到等待列表中
                    if (info.ErrorCount < Common.ErrorCount)
                    {
                        if (!ProDic.Keys.Contains(info.AsinId))
                        {
                            ProDic.Add(info.AsinId, info);
                        }
                    }
                    else
                    {
                        Common.WritePage(info);
                    }

                    PuaseRun();
                }

                if (info.IsSuccess == true)
                {
                    SMessage.SuccessProcudtCount++;
                    Console.WriteLine("商品URL：" + info.Url + "；解析成功！");
                    Console.WriteLine("已处理：" + SMessage.SuccessProcudtCount + " / " + SMessage.TotalProductCount);
                }

                if (ExpProductDic.Count > 0 && ExpProductDic.Count % 100 == 0)
                {
                    Cache.SaveTemporaryFile();
                }
            }
        }

        public bool GetImgUrl(string asinId, string colorName, string colorImages, string colorAsin, ColorModel item)
        {
            List<object> _colorList = null;

            var rv = GetKesValueSingle(colorImages, colorName);
            if (rv.Status == cn.Util.ReturnEnum.Yes)
            {
                _colorList = colorImages.Get(colorName).GetList();
                item.AsinId = colorAsin.Get(colorName).Get("asin");
                foreach (var item2 in _colorList)
                {
                    if (item2.ToString().Get("variant").ToUpper().Equals("MAIN"))
                    {
                        var hiRes = GetKesValueSingle(item2.ToString(), "hiRes");
                        if (hiRes.Status != cn.Util.ReturnEnum.Yes)
                        {
                            item.Large = item2.ToString().Get("large");
                        }
                        else
                        {
                            item.Large = hiRes.Value;
                        }
                        item.Thumb = item2.ToString().Get("thumb");

                        break;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        cn.Util.ReturnValue<string> GetKesValueSingle(string content, string param)
        {
            var rv = new cn.Util.ReturnValue<string>();
            try
            {
                rv.True(content.Get(param));
            }
            catch (Exception)
            {
                rv.False("Fail");
            }

            return rv;
        }

         void DownImage(ColorModel info, string asinId, string marketplaceID)
        {
            //info.SavePath = "img/" + marketplaceID +"/"+ asinId + "/" + info.ColorName + "/";
            info.SavePath = "img/" + marketplaceID;
            string savePath = Common.basePath + info.SavePath;
            if (!System.IO.Directory.Exists(savePath))
            {
                System.IO.Directory.CreateDirectory(savePath);
            }

            string imgName = "";

            if (string.IsNullOrEmpty(info.AsinId)==false&& string.IsNullOrEmpty(info.Large)==false)
            {
                imgName = info.AsinId + "_" + info.ColorName.Trim().Replace(" ", "_") + info.Large.Substring(info.Large.LastIndexOf("."));
            }

            //info.SavePath += "/" + imgName;
            info.SavePath = imgName;

            if (!System.IO.File.Exists(savePath + "/" + imgName))
            {
                if (IsDownImg)
                {
                    try
                    {
                        Common.TotalFile++;
                        var webClient = Common.WClient();
                        webClient.DownloadFileAsync(new Uri(info.Large), savePath + "/" + imgName);
                    }
                    catch
                    {
                       Console.WriteLine("图片下载失败：" + info.Large);
                        Common.WriteImg(info.Large + "|" + savePath + "/" + imgName);
                    }
                }
            }
        }

         /// <summary>
         /// 获取解析到的商品信息
         /// </summary>
         /// <returns></returns>
         public IList<Product> GetProductData()
         {
             return ExpProductDic.Values.ToList();
         }

         /// <summary>
         /// 是否已完成
         /// </summary>
         /// <returns></returns>
         public bool IsFinish()
         {
             if (timerListPage == null && PageList.Count <= 0 && timer1 == null && ProDic.Count <= 0 && dWorker.IsBusy== false)
             {
                 return true;
             }
             else
             {
                 return false;
             }
         }

    }
}
