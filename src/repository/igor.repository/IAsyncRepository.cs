using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace igor.repository
{
    public interface IAsyncRepository<T>
    {
        Task Create(T entity);
        Task<long> Update(T entity, Expression<Func<T, bool>> filter);
        Task<long> Delete(Expression<Func<T, bool>> filter);
        Task<T> Get(Expression<Func<T, bool>> filter);
        Task<IList<T>> Find(Expression<Func<T, bool>> filter);
        Task<bool> Exists(Expression<Func<T, bool>> filter);
    }
}
