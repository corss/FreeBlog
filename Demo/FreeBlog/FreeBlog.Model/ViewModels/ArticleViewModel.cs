using System;
using System.Collections.Generic;
using System.Text;

namespace FreeBlog.Model.ViewModels
{
    public class ArticleViewModel
    {
        /// <summary>
        /// 分配任务ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 栏目
        /// </summary>
        public List<ArticleMenuModel> ArticleMenu { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 浏览次数
        /// </summary>
        public int PageView { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        public string ImgUrl { get; set; }
        /// <summary>
        /// 文件地址
        /// </summary>
        public string FileUrl { get; set; }
        /// <summary>
        /// 状态 1.未审核 2.已审核 3.标记删除
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public string AddDate { get; set; }
        /// <summary>
        /// 截止时间
        /// </summary>
        public string EndDate { get; set; }
        /// <summary>
        /// 实际完成时间
        /// </summary>
        public string CompleteDate { get; set; }
        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsComplete { get; set; }

        /// <summary>
        /// 每月报指定次数
        /// </summary>
        public int NumMonth { get; set; }
        /// <summary>
        /// 每年报指定次数
        /// </summary>
        public int NumYear { get; set; }

        /// <summary>
        /// 指定月份，可存多个，按 “,”分割，如：,1,2,3, 代表123月份
        /// </summary>
        public string Month { get; set; }
        /// <summary>
        /// 指定月份对应次数,可存多个，，按 “,”分割,并与月份一一对应 
        /// </summary>
        public string Numth { get; set; }
    }
}
