using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeBlog.Model.ViewModels
{
    public class MenuListModel
    {
        public string path { get; set; }
        public string component { get; set; }
        public bool alwaysShow { get; set; }
        public string name { get; set; }

        public meta meta { get; set; }

        public IEnumerable<object> children { get; set; }
    }
}
