using System;
using System.Collections.Generic;
using System.Text;

namespace FreeBlog.Model.ViewModels.Api
{
    public class ArticlePostViewModel
    {
 public int ID { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Contents { get; set; }

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
        public DateTime CompleteDate { get; set; }
        /// <summary>
        /// 文件地址
        /// </summary>
        public string FileUrl { get; set; }

        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsComplete { get; set; }
        /// <summary>
        /// 顺序
        /// </summary>
        public int Sorting { get; set; }
        /// <summary>
        /// 状态 1.未审核 2.已审核 3.标记删除
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// 简介
        /// </summary>
        public string Synopsis { get; set; }


        /// <summary>
        /// 部门ID
        /// </summary>
        public int DepartmentID { get; set; }

        /// <summary>
        /// 栏目类型
        /// </summary>
        public string ArticleMenuIDs { get; set; }

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
