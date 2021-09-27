using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeBlog.Model.ViewModels
{
    /// <summary>
    /// 用于绑定页面上的菜单
    /// </summary>
    public class MenuViewModel
    {
        public int id { get; set; }

        public string component { get; set; }
        public int ? parentId { get; set; } 

        public string parentName { get; set; }

        public  string label { get; set; }

        public string code { get; set; }

        public string path { get; set; }

        public string name { get; set; }

        public string url { get; set; }

        public int  orderNum { get; set; }

        public string type { get; set; }

        public string icon { get; set; }

        public string remark { get; set; }

        public DateTime createTime { get; set; }

        public DateTime updateTime { get; set; }
        public IEnumerable<Object> children { get; set; }

        public Boolean open { get; set; }

    }
}
