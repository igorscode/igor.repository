using igor.repository.sqlserver.db;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace igor.repository.sqlserver.helpers
{
    public class SqlHelper
    {
        public static bool ExistTable(string tableName,
                                        string connectionString)
        {
            bool exist = false;

            StringBuilder queryString = new StringBuilder();
            queryString.Append(" SELECT count(table_name) ");
            queryString.Append(" FROM INFORMATION_SCHEMA.TABLES ");
            queryString.Append(" WHERE TABLE_NAME = '" + tableName + "' ");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString.ToString(), connection);

                connection.Open();

                exist = 0 < (Int32)command.ExecuteScalar();
            }

            return exist;
        }

        public static IList<DbField> InitializeCollectionFields(string collectionName,
                                                                string connectionString)
        {
            IList<DbField> dbFields = new List<DbField>();

            StringBuilder queryString = new StringBuilder();
            queryString.Append(" SELECT  c.object_id, ");
            queryString.Append("    c.column_id,  ");
            queryString.Append("	o.Name as Table_Name, ");
            queryString.Append("    c.Name as Field_Name, ");
            queryString.Append("    t.Name as Data_Type, ");
            queryString.Append("    c.is_identity as IsIdentity, ");
            queryString.Append("    t.is_nullable as IsNullable, ");
            queryString.Append("    t.max_length as Length_Size, ");
            queryString.Append("    t.precision as [Precision] ");
            queryString.Append(" FROM sys.columns c ");
            queryString.Append("     INNER JOIN sys.objects o ON o.object_id = c.object_id ");
            queryString.Append("     LEFT JOIN  sys.types t on t.system_type_id = c.system_type_id ");
            queryString.Append(" WHERE o.type = 'U' ");
            queryString.Append("     and t.Name != 'sysname' ");
            queryString.Append("     and c.object_id = OBJECT_ID('" + collectionName + "') ");
            queryString.Append(" ORDER BY c.column_id ");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString.ToString(), connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    DbField dbField = new DbField();

                    dbField.Name = reader[3].ToString();
                    dbField.Type = reader[4].ToString();
                    dbField.IsIdentity = reader.GetBoolean(5);
                    dbField.IsNullable = reader.GetBoolean(6);
                    dbField.Size = reader[7].ToString();
                    dbField.Precision = reader[8].ToString();

                    dbFields.Add(dbField);
                }
            }

            return dbFields;
        }

        public static void CreateTable(string tableName,
                                        IList<EntityField> fields,
                                        string connectionString)
        {
            StringBuilder queryString = new StringBuilder();
            queryString.Append(" create table " + tableName);
            queryString.Append(" ( ");

            string sqlFields = "";
            foreach (EntityField field in fields)
            {
                sqlFields += " [" + field.Name + "] " + field.DbType;
                sqlFields += field.Name.ToLower() != "id" ? "," : " not null primary key,";
            }

            queryString.Append(sqlFields.Substring(0, sqlFields.Length - 1));
            queryString.Append(" ) ");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString.ToString(), connection);

                connection.Open();
                int i = command.ExecuteNonQuery();
            }
        }

        public static void AddField(string tableName,
                                    string fieldName,
                                    string dbType,
                                    bool isNullable,
                                    string connectionString)
        {
            StringBuilder queryString = new StringBuilder();
            queryString.Append(" ALTER TABLE " + tableName);

            if (!String.IsNullOrEmpty(dbType))
            {
                if (isNullable)
                {
                    queryString.Append(" ADD [" + fieldName + "] " + dbType + " null ; ");
                }
                else
                {
                    queryString.Append(" ADD [" + fieldName + "] " + dbType + " not null ; ");
                }
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString.ToString(), connection);

                connection.Open();
                int i = command.ExecuteNonQuery();
            }
        }
    }
}
