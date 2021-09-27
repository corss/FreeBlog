using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeBlog.Model.ViewModels
{
 
    /// <summary>
    /// 查看菜单权限表
    /// </summary>
    public class MenuRoleView
    {
        public IEnumerable<MenuViewModel> listmenu { get; set; }
        public List<string> checkList { get; set; }
    }
}
