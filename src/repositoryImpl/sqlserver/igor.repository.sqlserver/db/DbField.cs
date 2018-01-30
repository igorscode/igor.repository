using System;

namespace igor.repository.sqlserver.db
{
    public class DbField
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsNullable { get; set; }
        public string Size { get; set; }
        public string Precision { get; set; }
    }
}
