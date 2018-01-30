using System;
using System.Collections.Generic;
using System.Text;

namespace igor.repository
{
    public interface IAsyncRepositoryFactory
    {
        IAsyncRepository<T> CreateAsyncRepository<T>(string collectionName);
    }
}
