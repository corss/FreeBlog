using FreeSql;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreeBlog.IService.Base
{
    public interface IBasicRepository<TEntity> : IReadOnlyRepository<TEntity>
          where TEntity : class
    {
        TEntity Insert(TEntity entity);
        Task<TEntity> InsertAsync(TEntity entity);
        int Insert2(List<TEntity> entity);
        int Insert3(TEntity entity);
        void Update(TEntity entity);
        Task UpdateAsync(TEntity entity);
        IUpdate<TEntity> UpdateDiy { get; }
        IDelete<TEntity> DeleteDiy(object dywhere);
        void Delete(TEntity entity);
        Task DeleteAsync(TEntity entity);
    }

    public interface IBasicRepository<TEntity, TKey> : IBasicRepository<TEntity>, IReadOnlyRepository<TEntity, TKey>
        where TEntity : class
    {
        void Delete(TKey id);
        Task DeleteAsync(TKey id);
    }

}
