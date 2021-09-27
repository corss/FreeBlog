using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeBlog.Model.ViewModels.Api
{
    /// <summary>
    /// 树状图
    /// </summary>
    public class ELTreeViewModel
    {
        public int id { get; set; }
        public string label { get; set; }
        public List<ELTreeViewModel> children { get; set; }
    }
}
