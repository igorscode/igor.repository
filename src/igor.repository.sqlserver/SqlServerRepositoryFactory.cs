using System;
using System.Collections.Generic;
using System.Text;

namespace igor.repository.sqlserver
{
    public class SqlServerRepositoryFactory : IRepositoryFactory
    {
        #region properties

        string connectionString;

        #endregion

        #region constructors

        public SqlServerRepositoryFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }

        //public SqlServerRepositoryFactory()
        //{}

        //public void SetConnectionString(string connectionString)
        //{
        //    this.connectionString = connectionString;
        //}

        #endregion

        #region interface

        public IRepository<T> CreateRepository<T>(string collectionName)
        {
            var repository = new SqlServerRepository<T>(this.connectionString, collectionName);

            return repository;
        }

        #endregion
    }
}
