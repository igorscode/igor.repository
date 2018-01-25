using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace igor.repository.sqlserver.helpers
{
    public class TypeHelper
    {
        private static IList<string> DbTypes = new List<string>()
        {
            "bigint",
            "binary",
            "bit",
            "char",
            "date",
            "datetime",
            "datetime2",
            "datetimeoffset",
            "decimal",
            "filestream",
            "float",
            //"geography",
            //"geometry",
            //"hierarchyid",
            "image",
            "int",
            "money",
            "nchar",
            "ntext",
            "numeric",
            "nvarchar",
            "real",
            "rowversion",
            "smalldatetime",
            "smallint",
            "smallmoney",
            "sql_variant",
            //"sysname",
            "text",
            "time",
            "timestamp",
            "tinyint",
            "uniqueidentifier",
            "varbinary",
            "varchar",
            "xml"
        };

        private static IList<string> NetTypes = new List<string>()
        {
            "Boolean",
            "Byte",
            "Char",
            "DateTime",
            "DateTime2",
            "DateTimeOffset",
            "Decimal",
            "Double",
            "Guid",
            "Int16",
            "Int32",
            "Int64",
            "Object",
            "SByte",
            "Single",
            "String",
            "TimeSpan",
            "UInt16",
            "UInt32",
            "UInt64",
            "Xml"
        };

        public static string GetNetNullableType(string assemblyQualifiedName)
        {
            if (assemblyQualifiedName.Contains("System.Boolean")) return "Boolean";
            if (assemblyQualifiedName.Contains("System.Byte")) return "Byte";
            if (assemblyQualifiedName.Contains("System.Char")) return "Char";
            if (assemblyQualifiedName.Contains("System.DateTime")) return "DateTime";
            if (assemblyQualifiedName.Contains("System.DateTime2")) return "DateTime2";
            if (assemblyQualifiedName.Contains("System.DateTimeOffset")) return "DateTimeOffset";
            if (assemblyQualifiedName.Contains("System.Decimal")) return "Decimal";
            if (assemblyQualifiedName.Contains("System.Double")) return "Double";
            if (assemblyQualifiedName.Contains("System.Guid")) return "Guid";
            if (assemblyQualifiedName.Contains("System.Int16")) return "Int16";
            if (assemblyQualifiedName.Contains("System.Int32")) return "Int32";
            if (assemblyQualifiedName.Contains("System.Int64")) return "Int64";
            if (assemblyQualifiedName.Contains("System.Object")) return "Object";
            if (assemblyQualifiedName.Contains("System.SByte")) return "SByte";
            if (assemblyQualifiedName.Contains("System.Single")) return "Single";
            if (assemblyQualifiedName.Contains("System.String")) return "String";
            if (assemblyQualifiedName.Contains("System.TimeSpan")) return "TimeSpan";
            if (assemblyQualifiedName.Contains("System.UInt16")) return "UInt16";
            if (assemblyQualifiedName.Contains("System.UInt32")) return "UInt32";
            if (assemblyQualifiedName.Contains("System.UInt64")) return "UInt64";
            if (assemblyQualifiedName.Contains("System.Xml")) return "Xml";

            throw new Exception("AssemblyQualifiedName not supoerted");
        }

        public static string GetDbType(string netType)
        {
            switch (netType)
            {
                case "Boolean":
                    return "bit";
                case "Byte":
                    return "tinyint";
                case "Char":
                    return "varchar(1)";
                case "DateTime":
                    return "smalldatetime";
                case "DateTime2":
                    return "dateTime";
                case "DateTimeOffset":
                    return "datetimeoffset";
                case "Decimal":
                    return "decimal";
                case "Double":
                    return "float";
                case "Guid":
                    return "uniqueidentifier";
                case "Int16":
                    return "smallint";
                case "Int32":
                    return "int";
                case "Int64":
                    return "int";
                case "Object":
                    throw new Exception("Object type not suported");
                case "SByte":
                    throw new Exception("SByte type not suported");
                case "Single":
                    return "real";
                case "String":
                    return "nvarchar(max)";
                case "TimeSpan":
                    return "time";
                case "UInt16":
                    return "smallint";
                case "UInt32":
                    return "int";
                case "UInt64":
                    return "int";
                case "Xml":
                    return "xml";
                default:
                    return "";
                    //throw new Exception("Defult Type not supoerted");
            }
        }

        public static void SetValue(object instance, SqlDataReader reader, PropertyInfo propertyInfo, string netType)
        {
            try
            {
                Int32 ordinal = reader.GetOrdinal(propertyInfo.Name);
                object value = reader[propertyInfo.Name];
                bool isNull = reader.IsDBNull(ordinal);

                switch (netType)
                {
                    case "Boolean":
                        if (!isNull)
                            propertyInfo.SetValue(instance, reader.GetBoolean(ordinal));
                        else
                            propertyInfo.SetValue(instance, false);
                        return;
                    case "Byte":
                        if (!isNull) propertyInfo.SetValue(instance, reader.GetByte(ordinal));
                        return;
                    case "Char":
                        if (!isNull) propertyInfo.SetValue(instance, reader.GetChar(ordinal));
                        return;
                    case "DateTime":
                        if (!isNull) propertyInfo.SetValue(instance, reader.GetDateTime(ordinal));
                        return;
                    case "DateTime2":
                        if (!isNull) propertyInfo.SetValue(instance, reader.GetDateTime(ordinal));
                        return;
                    case "DateTimeOffset":
                        if (!isNull) propertyInfo.SetValue(instance, reader.GetDateTimeOffset(ordinal));
                        return;
                    case "Decimal":
                        if (!isNull) propertyInfo.SetValue(instance, reader.GetDecimal(ordinal));
                        return;
                    case "Double":
                        if (!isNull) propertyInfo.SetValue(instance, reader.GetDouble(ordinal));
                        return;
                    case "Guid":
                        if (!isNull) propertyInfo.SetValue(instance, reader.GetGuid(ordinal));
                        return;
                    case "Int16":
                        if (!isNull) propertyInfo.SetValue(instance, reader.GetInt16(ordinal));
                        return;
                    case "Int32":
                        if (!isNull) propertyInfo.SetValue(instance, reader.GetInt32(ordinal));
                        return;
                    case "Int64":
                        if (!isNull) propertyInfo.SetValue(instance, reader.GetInt64(ordinal));
                        return;
                    case "Object":
                        throw new Exception("Object type not suported");
                    case "SByte":
                        throw new Exception("SByte type not suported");
                    case "Single":
                        if (!isNull) propertyInfo.SetValue(instance, reader.GetFloat(ordinal));
                        return;
                    case "String":
                        if (!isNull)
                            propertyInfo.SetValue(instance, reader.GetString(ordinal));
                        else
                            propertyInfo.SetValue(instance, "");
                        return;
                    case "TimeSpan":
                        propertyInfo.SetValue(instance, reader.GetTimeSpan(ordinal));
                        return;
                    case "UInt16":
                        propertyInfo.SetValue(instance, reader.GetInt16(ordinal));
                        return;
                    case "UInt32":
                        propertyInfo.SetValue(instance, reader.GetInt32(ordinal));
                        return;
                    case "UInt64":
                        propertyInfo.SetValue(instance, reader.GetInt64(ordinal));
                        return;
                    case "Xml":
                        propertyInfo.SetValue(instance, reader.GetSqlXml(ordinal));
                        return;
                        //default:
                        //throw new Exception("Defult Type not supoerted");
                }
            }
            catch { }
        }
    }
}
