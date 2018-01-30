using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace igor.repository
{
    public interface IRepository<T>
    {
        void Create(T entity);
        long Update(T entity, Expression<Func<T, bool>> filter);
        long Delete(Expression<Func<T, bool>> filter);
        T Get(Expression<Func<T, bool>> filter);
        IList<T> Find(Expression<Func<T, bool>> filter);
        bool Exists(Expression<Func<T, bool>> filter);
    }
}
