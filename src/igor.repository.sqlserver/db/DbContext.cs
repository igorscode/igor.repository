using igor.repository.sqlserver.linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace igor.repository.sqlserver.db
{
    public class DbContext
    {

        #region constructors

        private string _connectionString;
        public IDictionary<string, DbCollection> _collections;

        public DbContext(string connectionString)
        {
            _connectionString = connectionString;

            _collections = new Dictionary<string, DbCollection>();
        }

        #endregion

        #region

        public void AddCollection(string collectionName, Type collectionType)
        {
            DbCollection collection = new DbCollection(collectionName, collectionType);
            collection.initializeCollection(_connectionString);
            _collections.Add(collectionName, collection);
        }

        #endregion

        #region sql strings 

        public string GetSelectString<T>(string collectionName,
                                            Expression<Func<T, Boolean>> filter,
                                            string orderBy,
                                            bool onlyOne = false)
        {
            ExpressionTranslator expressionTranslator = new ExpressionTranslator();

            DbCollection collection = _collections[collectionName];

            StringBuilder str = new StringBuilder();

            str.Append(" select ");
            if (onlyOne) str.Append(" Top 1 ");
            str.Append(collection.GetFields());
            str.Append(" from [");
            str.Append(collectionName);
            str.Append("] ");

            string sqlWhere = filter == null ? "" : expressionTranslator.Translate<T>(filter);
            str.Append(string.IsNullOrEmpty(sqlWhere) ? " " : " where " + sqlWhere);

            if (!string.IsNullOrEmpty(orderBy)) str.Append(orderBy);

            return str.ToString();
        }

        public string GetInsertString(string collectionName,
                                        Object entity)
        {
            DbCollection collection = _collections[collectionName];

            StringBuilder str = new StringBuilder();

            str.Append(" insert into [");
            str.Append(collectionName);
            str.Append("] (");
            str.Append(collection.GetFields());
            str.Append(") values (");
            str.Append(collection.GetFieldValuesForInsert(entity));
            str.Append(") ");

            return str.ToString();
        }

        public string GetUpdateString<T>(string collectionName,
                                            Expression<Func<T, Boolean>> filter,
                                            T entity)
        {
            ExpressionTranslator expressionTranslator = new ExpressionTranslator();

            DbCollection collection = _collections[collectionName];

            StringBuilder str = new StringBuilder();

            str.Append(" update  [");
            str.Append(collectionName);
            str.Append("] ");
            str.Append(" set ");
            str.Append(collection.GetFieldValuesForUpdate(entity));

            string sqlWhere = filter == null ? "" : expressionTranslator.Translate<T>(filter);
            str.Append(string.IsNullOrEmpty(sqlWhere) ? " " : " where " + sqlWhere);

            return str.ToString();
        }

        public string GetDeleteString<T>(string collectionName,
                                            Expression<Func<T, Boolean>> filter)
        {
            ExpressionTranslator expressionTranslator = new ExpressionTranslator();

            DbCollection collection = _collections[collectionName];

            StringBuilder str = new StringBuilder();

            str.Append(" delete from [");
            str.Append(collectionName);
            str.Append("] ");

            string sqlWhere = filter == null ? "" : expressionTranslator.Translate<T>(filter);
            str.Append(string.IsNullOrEmpty(sqlWhere) ? " " : " where " + sqlWhere);

            return str.ToString();
        }

        public string GetExistsString<T>(string collectionName,
                                            Expression<Func<T, Boolean>> filter)
        {
            ExpressionTranslator expressionTranslator = new ExpressionTranslator();

            DbCollection collection = _collections[collectionName];

            StringBuilder str = new StringBuilder();

            str.Append(" select count(*) from [");
            str.Append(collectionName);
            str.Append("] ");

            string sqlWhere = filter == null ? "" : expressionTranslator.Translate<T>(filter);
            str.Append(string.IsNullOrEmpty(sqlWhere) ? " " : " where " + sqlWhere);

            return str.ToString();
        }

        #endregion

    }
}
