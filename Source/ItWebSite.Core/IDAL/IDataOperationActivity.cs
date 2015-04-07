using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ItWebSite.Core.IDAL
{
    public interface IDataOperationActivity<T>
    {
        int Insert(T entity);

        int SaveOrUpdate(T entity);

        int Modify(T entity);

        IEnumerable<T> QueryByFun(Expression<Func<T, bool>> fun);

        IEnumerable<T> QueryByIds(IEnumerable<dynamic> ids);

        int DeleteById(dynamic id);

        IEnumerable<T> QueryAll();

        T QueryById(dynamic id);

        int DeleteAll();

        T FirstOrDefault();

        T FirstOrDefault(Expression<Func<T, bool>> fun);

        Task<int> InsertAsync(T entity);

        Task<int> SaveOrUpdateAsync(T entity);

        Task<int> ModifyAsync(T entity);

        Task<IEnumerable<T>> QueryByFunAsync(Expression<Func<T, bool>> fun);

        Task<int> DeleteByIdAsync(dynamic id);

        Task<IEnumerable<T>> QueryAllAsync();

        Task<T> QueryByIdAsync(dynamic id);

        Task<int> DeleteAllAsync();

        Task<T> FirstOrDefaultAsync();

        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> fun);

        Task<IEnumerable<T>> QueryByIdsAsync(IEnumerable<dynamic> ids);

        IEnumerable<T> QueryLast(int count);

        Task<IEnumerable<T>> QueryLastAsync(int count);

        IEnumerable<T> QueryLastByFun(Expression<Func<T, bool>> fun,int count);

        Task<IEnumerable<T>> QueryLastByFunAsync(Expression<Func<T, bool>> fun, int count);

        IEnumerable<T> QueryPage(Expression<Func<T, bool>> whereFunc, Expression<Func<T, object>> orderByFunc, bool isAsc, int pageNum, int count);

        Task<IEnumerable<T>> QueryPageAsync(Expression<Func<T, bool>> whereFunc, Expression<Func<T, object>> orderByFunc, bool isAsc, int pageNum, int count);

        IQueryable<T> GetQueryAble();

        int QueryCount();

    }
}
