using System;
using System.Collections.Generic;
using System.Text;

namespace igor.repository
{
    public interface IRepositoryFactory
    {
        IRepository<T> CreateRepository<T>(string collectionName);
    }
}
