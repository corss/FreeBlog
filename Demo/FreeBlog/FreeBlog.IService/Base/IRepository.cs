using FreeBlog.Common;
using FreeSql;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using System.Threading.Tasks;

namespace FreeBlog.IService.Base
{
    public interface IRepository
    {
        //预留
    }

    public interface IRepository<TEntity> : IReadOnlyRepository<TEntity>, IBasicRepository<TEntity>
        where TEntity : class
    {
        void Delete(Expression<Func<TEntity, bool>> predicate);
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// 根据条件查询数据
        /// </summary>
        /// <param name="where">条件表达式树</param>
        /// <returns></returns>
        List<TEntity> GetWhere(ISelect<TEntity> source);
        /// <summary>
        /// 根据条件查询数据1
        /// </summary>
        /// <param name="where">条件表达式树</param>
        /// <returns></returns>
        List<TEntity> GetWhere(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, object>> order, string orderEnum = "Asc");


        /// <summary>
        /// 根据条件查询数据1
        /// </summary>
        /// <param name="where">条件表达式树</param>
        /// <returns></returns>
        List<TEntity> GetWhere(ISelect<TEntity> source, Expression<Func<TEntity, object>> order, string orderEnum = "Asc");
        /// <summary>
        /// 根据条件查询分页数据
        /// </summary>
        /// <param name="source">条件</param>
        /// <param name="parm">分页参数</param>
        /// <param name="orderFileds">排序 Sorting,ID DESC</param>
        /// <returns></returns>
        PagedInfo<TEntity> GetPages(ISelect<TEntity> source, PageParm parm, string orderFileds = null);

        /// <summary>
        /// 根据条件查询分页数据
        /// </summary>
        /// <param name="source">条件</param>
        /// <param name="parm">分页参数</param>
        /// <param name="order">排序字段 a => new { a.ID, a.Sorting }</param>
        /// <param name="orderEnum">排序方式 Asc Desc</param>
        /// <returns></returns>
        PagedInfo<TEntity> GetPages(ISelect<TEntity> source, PageParm parm, Expression<Func<TEntity, object>> order, string orderEnum);

    }

    public interface IRepository<TEntity, TKey> : IRepository<TEntity>, IReadOnlyRepository<TEntity, TKey>, IBasicRepository<TEntity, TKey>
        where TEntity : class
    {
    }

}
