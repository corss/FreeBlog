using FreeBlog.IService;
using FreeBlog.Model.Models;
using FreeBlog.Service.Base;

namespace FreeBlog.Service
{
    /// <summary>
    /// 网站基本配置
    /// </summary>
    public class ConfigBaseService : BaseRepository<ConfigBase>, IConfigBaseService
    {
        public ConfigBaseService(IFreeSql fsql) : base(fsql)
        {
        }

    }
}
