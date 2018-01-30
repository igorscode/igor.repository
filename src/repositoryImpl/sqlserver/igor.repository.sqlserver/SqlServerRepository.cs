using igor.repository.sqlserver.db;
using igor.repository.sqlserver.helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace igor.repository.sqlserver
{
    public class SqlServerRepository<T> : IRepository<T>
    {

        #region properties

        private string _connectionString;
        private string _collectionName;
        private DbContext _dbContext;

        #endregion

        #region constructors

        public SqlServerRepository(string connectionString, string collectionName)
        {
            _connectionString = connectionString;
            _collectionName = collectionName;

            _dbContext = new DbContext(connectionString);
            _dbContext.AddCollection(collectionName, typeof(T));
        }

        #endregion

        #region interface

        public void Create(T entity)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string queryString = _dbContext.GetInsertString(_collectionName, entity);

                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public long Update(T entity, Expression<Func<T, Boolean>> filter)
        {
            long retval = -1;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string queryString = _dbContext.GetUpdateString(_collectionName, filter, entity);

                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                retval = command.ExecuteNonQuery();
            }

            return retval;
        }

        public long Delete(Expression<Func<T, Boolean>> filter)
        {
            long retval = -1;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string queryString = _dbContext.GetDeleteString<T>(_collectionName, filter);

                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                retval = command.ExecuteNonQuery();
            }

            return retval;
        }

        public T Get(Expression<Func<T, Boolean>> filter)
        {
            T instance = Activator.CreateInstance<T>();

            IList<EntityField> entityFields = _dbContext._collections[_collectionName].EntityFields;

            string queryString = _dbContext.GetSelectString(_collectionName, filter, "", true);

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    foreach (var propertyInfo in instance.GetType().GetRuntimeProperties())
                    {
                        if (entityFields.Any(x => x.Name == propertyInfo.Name))
                        {
                            TypeHelper.SetValue(instance, reader, propertyInfo, entityFields.FirstOrDefault(x => x.Name == propertyInfo.Name).NetType);
                        }
                    }
                }
            }

            return instance;
        }

        public IList<T> Find(Expression<Func<T, Boolean>> filter)
        {
            IList<T> list = new List<T>();

            IList<EntityField> entityFields = _dbContext._collections[_collectionName].EntityFields;

            string queryString = _dbContext.GetSelectString(_collectionName, filter, "");

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    T instance = Activator.CreateInstance<T>();

                    foreach (var propertyInfo in instance.GetType().GetRuntimeProperties())
                    {
                        if (entityFields.Any(x => x.Name == propertyInfo.Name))
                        {
                            TypeHelper.SetValue(instance, reader, propertyInfo, entityFields.FirstOrDefault(x => x.Name == propertyInfo.Name).NetType);
                        }
                    }

                    list.Add(instance);
                }
            }

            return list;
        }

        public bool Exists(Expression<Func<T, bool>> filter)
        {
            bool exists = false;

            string queryString = _dbContext.GetExistsString(_collectionName, filter);

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);

                connection.Open();
                long count = (long)command.ExecuteScalar();

                exists = count > 0;
            }

            return exists;
        }

        #endregion
    }
}
