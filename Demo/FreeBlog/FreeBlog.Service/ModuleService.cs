using FreeBlog.IService;
using FreeBlog.Model.Models;
using FreeBlog.Service.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeBlog.Service
{
    public class ModuleService : BaseRepository<Module>, IModuleService { 
        public ModuleService(IFreeSql fsql) : base(fsql)
        {
        }


    }
}
