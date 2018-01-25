using igor.repository.sqlserver.helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace igor.repository.sqlserver.db
{
    public class DbCollection
    {
        private string _collectionName { get; }
        private Type _collectionType;
        public IList<EntityField> EntityFields { get; }

        public DbCollection(string collectionName, Type collectionType)
        {
            _collectionName = collectionName;
            _collectionType = collectionType;
            EntityFields = new List<EntityField>();

            foreach (var propertyInfo in _collectionType.GetRuntimeProperties())
            {
                EntityFields.Add(new EntityField(propertyInfo));
            }
        }

        public void initializeCollection(string connectionString)
        {
            if (!SqlHelper.ExistTable(_collectionName, connectionString))
            {
                SqlHelper.CreateTable(_collectionName, EntityFields, connectionString);
            }
            else
            {
                //recuperamos todos los campos de la tabla
                IList<DbField> dbFields = SqlHelper.InitializeCollectionFields(_collectionName, connectionString);

                //si falta algun campo lo añadimos
                foreach (EntityField entityField in EntityFields)
                {
                    if (!dbFields.Any(x => x.Name == entityField.Name) && !String.IsNullOrEmpty(entityField.DbType))
                    {
                        SqlHelper.AddField(_collectionName, entityField.Name, entityField.DbType, entityField.IsNullable, connectionString);
                    }
                }
            }
        }

        public string GetFields()
        {
            string strFields = "";

            //foreach (EntityField field in EntityFields)
            foreach (EntityField field in EntityFields.Where(x => !String.IsNullOrEmpty(x.DbType)))
            {
                strFields += "[" + field.Name + "],";
            }
            if (!string.IsNullOrEmpty(strFields)) strFields = strFields.Substring(0, strFields.Length - 1);

            return strFields;
        }

        public string GetFieldValuesForInsert(object entity)
        {
            string strValues = "";

            Type t = entity.GetType();
            var props = t.GetRuntimeProperties();

            //foreach (EntityField field in EntityFields)
            foreach (EntityField field in EntityFields.Where(x => !String.IsNullOrEmpty(x.DbType)))
            {
                strValues += GetSqlValueString(entity, field.PropertyInfo, field.EntityType) + ",";
            }
            if (!string.IsNullOrEmpty(strValues)) strValues = strValues.Substring(0, strValues.Length - 1);

            return strValues;
        }

        public string GetFieldValuesForUpdate(object entity)
        {
            string strValues = "";

            Type t = entity.GetType();
            var props = t.GetRuntimeProperties();
            foreach (EntityField entityField in EntityFields)
            {
                strValues += " [" + entityField.Name + "] = " + GetSqlValueString(entity, entityField.PropertyInfo, entityField.EntityType) + ",";
            }
            if (!string.IsNullOrEmpty(strValues)) strValues = strValues.Substring(0, strValues.Length - 1);

            return strValues;
        }

        private string GetSqlValueString(object entity, PropertyInfo propertyInfo, Type entityType)
        {
            object value = propertyInfo.GetValue(entity);

            switch (entityType.Name)
            {
                case "Guid":
                    return "'" + value.ToString() + "'";
                case "String":
                    return value == null ? "''" : "'" + value.ToString() + "'";
                case "Boolean":
                    return (bool)value ? "1" : "0";
                case "Int32":
                    return value == null ? "null" : value.ToString();
                default:
                    return GetSqlNullableValueString(entity, propertyInfo, entityType);
            }
        }

        private string GetSqlNullableValueString(object entity, PropertyInfo propertyInfo, Type entityType)
        {
            object value = propertyInfo.GetValue(entity);

            if (value == null) return "null";
            if (entityType.AssemblyQualifiedName.Contains("System.Boolean")) return (bool)value ? "1" : "0";
            if (entityType.AssemblyQualifiedName.Contains("System.Byte")) throw new Exception("Type not supoerted");
            if (entityType.AssemblyQualifiedName.Contains("System.Char")) throw new Exception("Type not supoerted");
            if (entityType.AssemblyQualifiedName.Contains("System.DateTime")) throw new Exception("Type not supoerted");
            if (entityType.AssemblyQualifiedName.Contains("System.DateTime2")) throw new Exception("Type not supoerted");
            if (entityType.AssemblyQualifiedName.Contains("System.DateTimeOffset")) throw new Exception("Type not supoerted");
            if (entityType.AssemblyQualifiedName.Contains("System.Decimal")) throw new Exception("Type not supoerted");
            if (entityType.AssemblyQualifiedName.Contains("System.Double")) throw new Exception("Type not supoerted");
            if (entityType.AssemblyQualifiedName.Contains("System.Guid")) return "'" + value.ToString() + "'";
            if (entityType.AssemblyQualifiedName.Contains("System.Int16")) throw new Exception("Type not supoerted");
            if (entityType.AssemblyQualifiedName.Contains("System.Int32")) throw new Exception("Type not supoerted");
            if (entityType.AssemblyQualifiedName.Contains("System.Int64")) throw new Exception("Type not supoerted");
            if (entityType.AssemblyQualifiedName.Contains("System.Object")) throw new Exception("Type not supoerted");
            if (entityType.AssemblyQualifiedName.Contains("System.SByte")) throw new Exception("Type not supoerted");
            if (entityType.AssemblyQualifiedName.Contains("System.Single")) throw new Exception("Type not supoerted");
            if (entityType.AssemblyQualifiedName.Contains("System.String")) return value.ToString();
            if (entityType.AssemblyQualifiedName.Contains("System.TimeSpan")) throw new Exception("Type not supoerted");
            if (entityType.AssemblyQualifiedName.Contains("System.UInt16")) throw new Exception("Type not supoerted");
            if (entityType.AssemblyQualifiedName.Contains("System.UInt32")) throw new Exception("Type not supoerted");
            if (entityType.AssemblyQualifiedName.Contains("System.UInt64")) throw new Exception("Type not supoerted");
            if (entityType.AssemblyQualifiedName.Contains("System.Xml")) throw new Exception("System.Xml Type not supoerted");

            return "";            

        }

    }
}
