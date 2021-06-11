using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmazonGather
{
    public class WebInfo
    {
        /// <summary>
        /// 网页路径
        /// </summary>
        public string Url { get; set; }

        public string AsinId { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 错误次数
        /// </summary>
        public int ErrorCount { get; set; }

        /// <summary>
        /// 页面类型
        /// </summary>
        public PageTypes PageType { get; set; }

        public bool IsSuccess { get; set; }

        public string MarketplaceID { get; set; }
    }

    public enum PageTypes
    {
        /// <summary>
        /// 页面
        /// </summary>
        Page = 1,

        /// <summary>
        /// 列表
        /// </summary>
        List = 2,
    }

    public class StatisticsMessage
    {
        public void ClearValue()
        {
            IsNextPage = true;
            TotalPageCount = 0;
            TotalProductCount = 0;
            SuccessProcudtCount = 0;
            Common.LoadFile = 0;
            Common.TotalFile = 0;
        }
        /// <summary>
        /// 是否最后一页
        /// </summary>
        public bool IsNextPage { get; set; }

        /// <summary>
        /// 总共多少页
        /// </summary>
        public int TotalPageCount { get; set; }

        /// <summary>
        /// 总共多少个商品
        /// </summary>
        public int TotalProductCount { get; set; }

        /// <summary>
        /// 成功读取多少个
        /// </summary>
        public int SuccessProcudtCount { get; set; }
    }
}
