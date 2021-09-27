using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeBlog.Model.ViewModels
{
    /// <summary>
    /// 通用类
    /// </summary>
    public class BaseViewModel
    {
        public BaseViewModel(int ID, string Title)
        {
            this.ID = ID;
            this.Title = Title;
        }

        public int ID { get; set; }
        public string Title { get; set; }
    }
}
