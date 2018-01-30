using igor.repository.sqlserver.helpers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace igor.repository.sqlserver.db
{
    public class EntityField
    {
        public string Name { get; set; }
        public Type EntityType { get; set; }
        public TypeInfo EntityTypeInfo { get; set; }
        public PropertyInfo PropertyInfo { get; set; }

        public bool IsIdentity { get; set; }
        public bool IsNullable { get; set; }
        public bool IsPrimitive { get; set; }

        public bool IsClass { get; set; }
        public bool IsArray { get; set; }
        public bool IsList { get; set; }


        public string NetType { get; set; }
        public string DbType { get; set; }

        public EntityField(PropertyInfo propertyInfo)
        {
            Name = propertyInfo.Name;

            PropertyInfo = propertyInfo;
            EntityType = propertyInfo.PropertyType;
            EntityTypeInfo = propertyInfo.PropertyType.GetTypeInfo();

            IsIdentity = false;
            //Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, IntPtr, UIntPtr, Char, Double, Single.
            IsPrimitive = propertyInfo.PropertyType.GetTypeInfo().IsPrimitive;

            bool isGuid = propertyInfo.PropertyType.Name == "Guid";
            bool isString = propertyInfo.PropertyType.Name == "String";
            IsNullable = propertyInfo.PropertyType.GetTypeInfo().IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
            IsClass = EntityTypeInfo.IsClass;
            IsArray = EntityTypeInfo.IsArray;
            IsList = propertyInfo.PropertyType.GetTypeInfo().IsGenericType && (propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(IList<>) || propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(List<>));

            //if (!IsNullable && (IsPrimitive || isGuid || isString))
            if (!IsNullable)
            {
                if (IsPrimitive || isGuid || isString)
                {
                    NetType = propertyInfo.PropertyType.Name;
                }
            }
            else
            {
                if (!IsClass && !IsArray && !IsList)
                {
                    NetType = TypeHelper.GetNetNullableType(propertyInfo.PropertyType.AssemblyQualifiedName);
                    isGuid = NetType == "Guid";
                    isString = NetType == "String";
                }
            }
            DbType = TypeHelper.GetDbType(NetType);
        }
    }
}
