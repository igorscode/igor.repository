using System;
using System.Collections.Generic;
using System.Text;

namespace igor.repository.sqlserver.linq
{
    internal static class FieldValueSerializerHelper
    {
        public static string GetValue(object value, Type fieldType)
        {
            switch (fieldType.Name)
            {
                case "Guid":
                    return "'" + ((Guid)value).ToString() + "'";
                case "String":
                    return "'" + value.ToString() + "'";
                case "Int32":
                case "Int64":
                    return value.ToString();
            }

            return "";
        }
    }
}
