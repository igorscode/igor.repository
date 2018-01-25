using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace igor.repository
{
    public interface IRepository<T>
    {
        void Create(T entity);
        int Update(T entity, Expression<Func<T, bool>> filter);
        int Delete(Expression<Func<T, bool>> filter);
        T Get(Expression<Func<T, bool>> filter);
        IList<T> GetList(Expression<Func<T, bool>> filter);
        bool Exists(Expression<Func<T, bool>> filter);
    }
}
