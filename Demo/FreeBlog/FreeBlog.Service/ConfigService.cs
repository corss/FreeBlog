using FreeBlog.IService;
using FreeBlog.Model.Models;
using FreeBlog.Service.Base;

namespace FreeBlog.Service
{
    public class ConfigService : BaseRepository<Config>, IConfigService
    {
        public ConfigService(IFreeSql fsql) : base(fsql)
        {
        }
    }
}
