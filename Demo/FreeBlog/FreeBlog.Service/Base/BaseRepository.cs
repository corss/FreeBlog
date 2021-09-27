using FreeBlog.Common;
using FreeBlog.IService.Base;
using FreeSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FreeBlog.Service.Base
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity>
          where TEntity : class
    {
        protected IFreeSql _fsql;

        public BaseRepository(IFreeSql fsql) : base()
        {
            _fsql = fsql;
            if (_fsql == null) throw new NullReferenceException("fsql 参数不可为空");
        }

        public ISelect<TEntity> Select => _fsql.Select<TEntity>();
        public IUpdate<TEntity> UpdateDiy => _fsql.Update<TEntity>();
        public void Delete(Expression<Func<TEntity, bool>> predicate) => _fsql.Delete<TEntity>().Where(predicate).ExecuteAffrows();
        public void Delete(TEntity entity) => _fsql.Delete<TEntity>(entity).ExecuteAffrows();
        public Task DeleteAsync(Expression<Func<TEntity, bool>> predicate) => _fsql.Delete<TEntity>().Where(predicate).ExecuteAffrowsAsync();
        public Task DeleteAsync(TEntity entity) => _fsql.Delete<TEntity>(entity).ExecuteAffrowsAsync();
       
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="dywhere"></param>
        /// <returns></returns>
        IDelete<TEntity> IBasicRepository<TEntity>.DeleteDiy(object dywhere)
        {
            return _fsql.Delete<TEntity>(dywhere);
        }
        /// <summary>
        /// 根据条件查询数据
        /// </summary>
        /// <param name="where">条件表达式树</param>
        /// <returns></returns>
        public List<TEntity> GetWhere(ISelect<TEntity> source)
        {
            var query = source;
            return query.ToList();
        }
        /// <summary>
        /// 根据条件查询数据
        /// </summary>
        /// <param name="where">条件表达式树</param>
        /// <returns></returns>
        public List<TEntity> GetWhere(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, object>> order, string orderEnum = "Asc")
        {
            var query = Select.Where(where).OrderByIf(orderEnum == "Asc", order).OrderByIf(orderEnum == "Desc", order);
            return query.ToList();
        }
        /// <summary>
        /// 根据条件查询数据2
        /// </summary>
        /// <param name="where">条件表达式树</param>
        /// <returns></returns>
        public List<TEntity> GetWhere(ISelect<TEntity> source, Expression<Func<TEntity, object>> order, string orderEnum = "Asc")
        {
            var query = source.OrderByIf(orderEnum == "Asc", order).OrderByIf(orderEnum == "Desc", order);
            return query.ToList();
        }
        /// <summary>
        /// 根据条件查询分页数据
        /// </summary>
        /// <param name="source"></param>
        /// <param name="parm"></param>
        /// <param name="orderFileds">排序</param>
        /// <returns></returns>
        public PagedInfo<TEntity> GetPages(ISelect<TEntity> source, PageParm parm, string orderFileds = null)
        {
            int TotalCount = 0;
            PagedInfo<TEntity> page = new PagedInfo<TEntity>();
            //var source = _fsql.Select<TEntity>().Where(where).OrderBy(orderFileds != null, orderFileds);
            //if (orderFileds != null)
            //    source = source.OrderBy(orderFileds);
            var list = source.Page(parm.PageIndex, parm.PageSize).Count(out var total);
            if (list != null)
            {
                page.PageIndex = parm.PageIndex;
                page.PageSize = parm.PageSize;
                page.TotalCount = (int)total;
                page.TotalPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(page.TotalCount) / page.PageSize));
                page.DataSource = list.ToList();
                return page;
            }
            return null;
        }
        /// <summary>
        /// 根据条件查询分页数据
        /// </summary>
        /// <param name="source">条件</param>
        /// <param name="parm">分页参数</param>
        /// <param name="order">排序字段 a => new { a.ID, a.Sorting }</param>
        /// <param name="orderEnum">排序方式 Asc Desc</param>
        /// <returns></returns>
        public PagedInfo<TEntity> GetPages(ISelect<TEntity> source, PageParm parm, Expression<Func<TEntity, object>> order, string orderEnum)
        {
            //var source = Db.Queryable<T>().Where(where);
            //return source.ToPage(parm);

            int TotalCount = 0;
            PagedInfo<TEntity> page = new PagedInfo<TEntity>();
            var sources = source.OrderByIf(orderEnum == "Asc", order).OrderByIf(orderEnum == "Desc", order);
            var list = source.Page(parm.PageIndex, parm.PageSize);
            if (list != null)
            {
                page.PageIndex = parm.PageIndex;
                page.PageSize = parm.PageSize;
                page.TotalCount = TotalCount;
                page.TotalPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(page.TotalCount) / page.PageSize));
                page.DataSource = list.ToList();
                return page;
            }
            return null;
        }


        public TEntity Insert(TEntity entity) => _fsql.Insert<TEntity>().AppendData(entity).ExecuteInserted().FirstOrDefault();
        /// <summary>
        /// list格式，批量新增,返回行数
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Insert2(List<TEntity> entity) => _fsql.Insert<TEntity>().AppendData(entity).ExecuteAffrows();
        /// <summary>
        /// 批量新增,返回行数
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Insert3(TEntity entity) => _fsql.Insert<TEntity>().AppendData(entity).ExecuteAffrows();
        async public Task<TEntity> InsertAsync(TEntity entity) => (await _fsql.Insert<TEntity>().AppendData(entity).ExecuteInsertedAsync()).FirstOrDefault();

        public void Update(TEntity entity) => _fsql.Update<TEntity>().SetSource(entity).ExecuteAffrows();
        public Task UpdateAsync(TEntity entity) => _fsql.Update<TEntity>().SetSource(entity).ExecuteAffrowsAsync();

    }
    public abstract class BaseRepository<TEntity, TKey> : BaseRepository<TEntity>, IRepository<TEntity, TKey>
        where TEntity : class
    {
        public BaseRepository(IFreeSql fsql) : base(fsql)
        {
        }

        public void Delete(TKey id) => _fsql.Delete<TEntity>(id).ExecuteAffrows();
        public Task DeleteAsync(TKey id) => _fsql.Delete<TEntity>(id).ExecuteAffrowsAsync();

        public TEntity Find(TKey id) => _fsql.Select<TEntity>(id).ToOne();
        public Task<TEntity> FindAsync(TKey id) => _fsql.Select<TEntity>(id).ToOneAsync();

        public TEntity Get(TKey id) => Find(id);
        public Task<TEntity> GetAsync(TKey id) => FindAsync(id);


    }

}
