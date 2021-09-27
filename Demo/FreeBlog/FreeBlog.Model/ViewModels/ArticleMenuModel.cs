using System;
using System.Collections.Generic;
using System.Text;

namespace FreeBlog.Model.ViewModels
{
    public class ArticleMenuModel
    {
        /// <summary>
        /// 栏目ID
        /// </summary>
        public int ArticleMenuID { get; set; }
        /// <summary>
        /// 栏目名
        /// </summary>
        public string Names { get; set; }


        //public ArticleMenuModel(int ArticleMenuID, string Names)
        //{
        //    this.ArticleMenuID = ID;
        //    this.Names = Names;
        //}
    }
}
