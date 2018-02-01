using igor.repository;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace igor.repository.mongo.async
{
    public class MongoAsyncRepositoryFactory : IAsyncRepositoryFactory
    {
        #region properties

        //private string _connectionString;
        private IMongoDatabase _db;
        
        #endregion

        #region constructors

        public MongoAsyncRepositoryFactory(string connectionString, string dbName)
        {
            //_connectionString = connectionString;

            MongoClient mongoClient = new MongoClient(connectionString);
            _db = mongoClient.GetDatabase(dbName);
        }

        #endregion

        #region interface

        public IAsyncRepository<T> CreateAsyncRepository<T>(string collectionName)
        {
            //var repository = new MongoAsyncRepository<T>(this.connectionString, collectionName);

            var repository = new MongoAsyncRepository<T>(collectionName, _db);

            return repository;
        }

        #endregion
    }
}

