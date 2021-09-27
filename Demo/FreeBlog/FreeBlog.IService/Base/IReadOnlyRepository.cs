
using FreeBlog.IService.Base;
using FreeSql;
using System.Threading.Tasks;

namespace FreeBlog.IService
{
    public interface IReadOnlyRepository<TEntity> : IRepository
            where TEntity : class
    {
        ISelect<TEntity> Select { get; }
    }

    public interface IReadOnlyRepository<TEntity, TKey> : IReadOnlyRepository<TEntity>
        where TEntity : class
    {
        TEntity Get(TKey id);
        Task<TEntity> GetAsync(TKey id);

        TEntity Find(TKey id);
        Task<TEntity> FindAsync(TKey id);
    }

}
