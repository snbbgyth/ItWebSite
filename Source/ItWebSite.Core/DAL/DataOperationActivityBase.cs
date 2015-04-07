using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using ItWebSite.Core.IDAL;
using ItWebSite.Core.Model;
using ItWebSite.Core.QueueDAL;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace ItWebSite.Core.DAL
{
    public abstract class DataOperationActivityBase<T> : IDataOperationActivity<T> where T : BaseTable, new()
    {
        #region Private Variable

        #endregion

        #region Private Property

        #endregion

        public DataOperationActivityBase()
        {

        }

        public virtual int Insert(T entity)
        {
            try
            {
                InitInsertBaseTable(entity);
                using (var session = FluentNHibernateDal.Instance.GetSession())
                {
                    session.Transaction.Begin();
                    var result = session.Save(entity);
                    session.Flush();
                    session.Transaction.Commit();
                    return Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                LogInfoQueue.Instance.Insert(GetType(), MethodBase.GetCurrentMethod().Name, ex);
                return 0;
            }
        }

        private void InitInsertBaseTable(T entity)
        {
            entity.CreateDate = DateTime.Now;
            entity.LastModifyDate = DateTime.Now;
        }

        public virtual int SaveOrUpdate(T entity)
        {
            try
            {
                InitInsertBaseTable(entity);
                using (var session = FluentNHibernateDal.Instance.GetSession())
                {
                    session.SaveOrUpdate(entity);
                    session.Flush();
                    return 1;
                }
            }
            catch (Exception ex)
            {
                LogInfoQueue.Instance.Insert(GetType(), MethodBase.GetCurrentMethod().Name, ex);
                return 0;
            }
        }

        private void InitModifyBaseTable(T entity)
        {
            entity.LastModifyDate = DateTime.Now;
        }

        public virtual int Modify(T entity)
        {
            try
            {
                InitModifyBaseTable(entity);
                using (var session = FluentNHibernateDal.Instance.GetSession())
                {
                    session.Update(entity);
                    session.Flush();
                    return 1;
                }
            }
            catch (Exception ex)
            {
                LogInfoQueue.Instance.Insert(GetType(), MethodBase.GetCurrentMethod().Name, ex);
                return 0;
            }
        }

        public virtual IEnumerable<T> QueryByFun(Expression<Func<T, bool>> fun)
        {
            var entityList = new List<T>();
            try
            {
                using (var session = FluentNHibernateDal.Instance.GetSession())
                {
                    if (fun != null)
                    {
                        entityList = session.QueryOver<T>().Where(fun).List().ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                LogInfoQueue.Instance.Insert(GetType(), MethodBase.GetCurrentMethod().Name, ex);
            }
            return entityList;
        }

        public virtual int Delete(dynamic entity)
        {
            int result = 0;

            try
            {
                using (var session = FluentNHibernateDal.Instance.GetSession())
                {
                    var queryString = string.Format("delete {0} where Id = :id", typeof(T).Name);
                    result = session.CreateQuery(queryString).SetParameter("id", entity.Id).ExecuteUpdate();
                }
            }
            catch (Exception ex)
            {
                LogInfoQueue.Instance.Insert(GetType(), MethodBase.GetCurrentMethod().Name, ex);
            }
            return result;
        }

        public virtual int DeleteById(dynamic id)
        {
            int reslut = 0;

            try
            {
                using (var session = FluentNHibernateDal.Instance.GetSession())
                {
                    session.Delete(string.Format("from {0} where id = {1}", typeof(T).Name, id));
                    session.Flush();
                }
            }
            catch (Exception ex)
            {
                LogInfoQueue.Instance.Insert(GetType(), MethodBase.GetCurrentMethod().Name, ex);
            }
            return reslut;
        }

        public virtual IEnumerable<T> QueryAll()
        {
            var entityList = new List<T>();

            try
            {
                using (var session = FluentNHibernateDal.Instance.GetSession())
                {
                    entityList = session.CreateCriteria(typeof(T)).List<T>().ToList();
                }
            }
            catch (Exception ex)
            {
                LogInfoQueue.Instance.Insert(GetType(), MethodBase.GetCurrentMethod().Name, ex);
            }
            return entityList;
        }

        public virtual int QueryCount()
        {
            var entityList = new List<T>();
            try
            {
                using (var session = FluentNHibernateDal.Instance.GetSession())
                {
                   return  session.QueryOver<T>().RowCount() ;
                }
            }
            catch (Exception ex)
            {
                LogInfoQueue.Instance.Insert(GetType(), MethodBase.GetCurrentMethod().Name, ex);
            }
            return 0;
        }

        public virtual IEnumerable<T> QueryByIds(IEnumerable<dynamic> ids)
        {
            var entityList = new List<T>();
            try
            {
                using (var session = FluentNHibernateDal.Instance.GetSession())
                {
                    var queryString = string.Format("select * from {0} where Id in(:ids)", typeof(T).Name);
                    entityList = session.CreateQuery(queryString)
                                    .SetParameter("ids", string.Join(",", ids))
                                    .Future<T>().ToList();
                }
            }
            catch (Exception ex)
            {
                LogInfoQueue.Instance.Insert(GetType(), MethodBase.GetCurrentMethod().Name, ex);
            }
            return entityList;
        }

        public virtual T QueryById(dynamic id)
        {
            T entity = default(T);

            try
            {
                using (var session = FluentNHibernateDal.Instance.GetSession())
                {
                    entity = session.Get<T>(id);
                }
            }
            catch (Exception ex)
            {
                LogInfoQueue.Instance.Insert(GetType(), MethodBase.GetCurrentMethod().Name, ex);
            }
            return entity;
        }



        public virtual int DeleteAll()
        {
            int result = 0;

            try
            {
                using (var session = FluentNHibernateDal.Instance.GetSession())
                {
                    var queryString = string.Format("from {0} ", typeof(T).Name);
                    result = session.Delete(queryString);
                    session.Flush();
                }
            }
            catch (Exception ex)
            {
                LogInfoQueue.Instance.Insert(GetType(), MethodBase.GetCurrentMethod().Name, ex);
            }
            return result;
        }

        public T FirstOrDefault()
        {
            try
            {
                using (var session = FluentNHibernateDal.Instance.GetSession())
                {
                    return session.QueryOver<T>().SingleOrDefault();
                }
            }
            catch (Exception ex)
            {
                LogInfoQueue.Instance.Insert(GetType(), MethodBase.GetCurrentMethod().Name, ex);
            }
            return default(T);
        }


        public IEnumerable<T> QueryLast(int count)
        {
            try
            {
                using (var session = FluentNHibernateDal.Instance.GetSession())
                {
                    return session.QueryOver<T>().OrderBy(t => t.CreateDate).Desc.Take(count).List<T>();
                }
            }
            catch (Exception ex)
            {
                LogInfoQueue.Instance.Insert(GetType(), MethodBase.GetCurrentMethod().Name, ex);
                return new List<T>();
            }
        }


        public IEnumerable<T> QueryPage( Expression<Func<T, bool>> whereFunc, Expression<Func<T, object>> orderByFunc, bool isAsc, int pageNum, int count)
        {
            try
            {
                using (var session = FluentNHibernateDal.Instance.GetSession())
                {
                    if (whereFunc == null&&orderByFunc==null)
                        return session.QueryOver<T>().Skip((pageNum - 1) * count).Take(count ).List();
                    if (whereFunc == null )
                    {
                        if (isAsc)
                            return session.QueryOver<T>().OrderBy(orderByFunc).Asc.Skip((pageNum - 1) * count).Take(count ).List(); ;
                        return session.QueryOver<T>().OrderBy(orderByFunc).Desc.Skip((pageNum - 1) * count).Take(count ).List(); ;

                    }
                    if (isAsc)
                        return session.QueryOver<T>().Where(whereFunc).OrderBy(orderByFunc).Asc.Skip((pageNum - 1) * count).Take(count ).List(); ;
                    return session.QueryOver<T>().Where(whereFunc).OrderBy(orderByFunc).Desc.Skip((pageNum-1) * count).Take(count ).List(); ;
                }
            }
            catch (Exception ex)
            {
                LogInfoQueue.Instance.Insert(GetType(), MethodBase.GetCurrentMethod().Name, ex);
                return new List<T>();
            }
        }

        public IEnumerable<T> QueryLastByFun(Expression<Func<T, bool>> fun, int count)
        {
            var entityList = new List<T>();
            try
            {
                using (var session = FluentNHibernateDal.Instance.GetSession())
                {
                    if (fun != null)
                    {
                        return session.QueryOver<T>().Where(fun).OrderBy(t => t.CreateDate).Desc.Take(count).Future().ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                LogInfoQueue.Instance.Insert(GetType(), MethodBase.GetCurrentMethod().Name, ex);
            }
            return entityList;
        }

        public IQueryable<T> GetQueryAble()
        {
            try
            {
                var session = FluentNHibernateDal.Instance.GetSession();
                return session.QueryOver<T>().Future().AsQueryable();
            }
            catch (Exception ex)
            {
                LogInfoQueue.Instance.Insert(GetType(), MethodBase.GetCurrentMethod().Name, ex);
                return null;
            }
        }

        /// <summary>
        /// Find first or default by funcation
        /// </summary>
        /// <param name="fun">condition funcation</param>
        /// <returns>ClientTableState entity</returns>
        public T FirstOrDefault(Expression<Func<T, bool>> fun)
        {

            try
            {
                var entityList = QueryByFun(fun);
                if (entityList != null && entityList.Any()) return entityList.FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogInfoQueue.Instance.Insert(GetType(), MethodBase.GetCurrentMethod().Name, ex);
            }
            return null;
        }


        public async Task<int> InsertAsync(T entity)
        {
            var task = Task.Factory.StartNew(() => Insert(entity));
            return await task;
        }

        public async Task<int> SaveOrUpdateAsync(T entity)
        {
            var task = Task.Factory.StartNew(() => SaveOrUpdate(entity));
            return await task;
        }

        public async Task<int> ModifyAsync(T entity)
        {
            var task = Task.Factory.StartNew(() => Modify(entity));
            return await task;
        }

        public async Task<IEnumerable<T>> QueryByFunAsync(Expression<Func<T, bool>> fun)
        {
            var task = Task.Factory.StartNew(() => QueryByFun(fun));
            return await task;
        }

        public async Task<int> DeleteByIdAsync(dynamic id)
        {
            var task = Task.Factory.StartNew(() => DeleteById(id));
            return await task;
        }

        public async Task<IEnumerable<T>> QueryAllAsync()
        {
            var task = Task.Factory.StartNew(() => QueryAll());
            return await task;
        }

        public async Task<T> QueryByIdAsync(dynamic id)
        {
            var task = Task.Factory.StartNew(() => QueryById(id));
            return await task;
        }

        public async Task<int> DeleteAllAsync()
        {
            var task = Task.Factory.StartNew(() => DeleteAll());
            return await task;
        }

        public async Task<IEnumerable<T>> QueryByIdsAsync(IEnumerable<dynamic> ids)
        {
            var task = Task.Factory.StartNew(() => QueryByIds(ids));
            return await task;
        }

        public async Task<T> FirstOrDefaultAsync()
        {
            var task = Task.Factory.StartNew(() => FirstOrDefault());
            return await task;
        }

        /// <summary>
        /// Async Find First Or Default
        /// </summary>
        /// <param name="fun"></param>
        /// <returns></returns>
        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> fun)
        {
            var task = Task.Factory.StartNew(() => FirstOrDefault(fun));
            return await task;
        }


        public async Task<IEnumerable<T>> QueryLastAsync(int count)
        {
            var task = Task.Factory.StartNew(() => QueryLast(count));
            return await task;
        }

        public async Task<IEnumerable<T>> QueryLastByFunAsync(Expression<Func<T, bool>> fun, int count)
        {
            var task = Task.Factory.StartNew(() => QueryLastByFun(fun, count));
            return await task;
        }

        public async Task<IEnumerable<T>> QueryPageAsync(Expression<Func<T, bool>> whereFunc, Expression<Func<T, object>> orderByFunc, bool isAsc, int pageNum, int count)
        {
            var task = Task.Factory.StartNew(() => QueryPage(whereFunc,orderByFunc, isAsc, pageNum, count));
            return await task;
        }
    }
}
