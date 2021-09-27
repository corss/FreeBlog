using FreeBlog.Model.Models;
using FreeBlog.Model.ViewModels.Api;
using System.Collections.Generic;
using System.Linq;

namespace FreeBlog.Api.Helper
{
    public class GetMenusHelps
    {
        public static List<ELTreeViewModel> GetChildren(IEnumerable<Menu> data, int ID)
        {
            var data2 = data.Where(a => a.parentId == ID&&a.open==true);
            if (data2 != null && data2.Count() > 0)
            {
                List<ELTreeViewModel> list = new List<ELTreeViewModel>();
                foreach (var item in data2)
                {
                    list.Add(new ELTreeViewModel
                    {
                        id = item.id,
                        label = item.label,
                        children = GetChildren(data, item.id)
                    });
                }
                return list;
            }
            return null;
        }
    }
}
