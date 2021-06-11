using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmazonGather
{
    public class Product
    {
        public string AsinId { get; set; }

        /// <summary>
        /// 商品地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 处理过的标题
        /// 注释：替换掉品牌
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 原标题
        /// </summary>
        public string OldTitle { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public string Price { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        /// 尺寸
        /// </summary>
        public string Size { get; set; }

        public IList<ColorModel> ColorList { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        public IList<string> Details { get; set; }

        /// <summary>
        /// 排名
        /// </summary>
        public IList<string> RankList { get; set; }

        public string MarketplaceID { get; set; }
    }

    public class ColorModel
    {
        /// <summary>
        /// 颜色
        /// </summary>
        public string ColorName { get; set; }

        public string AsinId { get; set; }

        /// <summary>
        /// 缩略图
        /// </summary>
        public string Thumb { get; set; }

        /// <summary>
        /// 大图
        /// </summary>
        public string Large { get; set; }

        /// <summary>
        /// 下载保存路径
        /// </summary>
        public string SavePath { get; set; }
    }
}
