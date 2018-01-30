using igor.repository;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace igor.asyncrepository.mongo
{
    public class MongoAsyncRepository<T> : IAsyncRepository<T>
    {
        #region properties

        protected IMongoCollection<T> collection;

        #endregion

        #region constructors

        public MongoAsyncRepository(String collectionName, IMongoDatabase db)
        {
            collection = db.GetCollection<T>(collectionName);
        }

        #endregion

        public async Task Create(T entity)
        {
            await collection.InsertOneAsync(entity);
        }

        public async Task<long> Update(T entity, Expression<Func<T, bool>> filter)
        {
            ReplaceOneResult result = await collection.ReplaceOneAsync(filter, entity);
            return result.ModifiedCount;
        }

        public async Task<long> Delete(Expression<Func<T, bool>> filter)
        {
            DeleteResult result = await collection.DeleteManyAsync(filter);
            return result.DeletedCount;
        }

        public async Task<bool> Exists(Expression<Func<T, bool>> filter)
        {
            long count = await collection.CountAsync<T>(filter, new CountOptions() { Limit = 1 });
            return count > 0;
        }

        public async Task<T> Get(Expression<Func<T, bool>> filter)
        {
            var retval = await collection.Find(filter).Limit(1).ToListAsync();
            return retval[0];
        }

        public async Task<IList<T>> Find(Expression<Func<T, bool>> filter)
        {
            return await collection.Find(filter).ToListAsync();
        }

        //public async Task<IList<T>> Find(Expression<Func<T, bool>> filter,
        //                                Expression<Func<T, bool>> sort,
        //                                bool descendent = false,
        //                                int? skip = 0,
        //                                int? limit = null)
        //{
        //    //var collection = _database.GetCollection<BsonDocument>("restaurants");
        //    //var filter = new BsonDocument();
        //    //var sort = Builders<BsonDocument>.Sort.Ascending("epochdatestamp");
        //    //var options = new FindOptions<BsonDocument>
        //    //{
        //    //    Sort = sort
        //    //};
        //    //var count = 0;
        //    //using (var cursor = await collection.FindAsync(filter, options))
        //    //{
        //    //    while (await cursor.MoveNextAsync())
        //    //    {
        //    //        var batch = cursor.Current;
        //    //        foreach (var document in batch)
        //    //        {
        //    //            // process document
        //    //            count++;
        //    //        }
        //    //    }
        //    //}
        //}
    }
}
