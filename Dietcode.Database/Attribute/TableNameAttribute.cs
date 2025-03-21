using Dapper.Contrib.Extensions;

namespace Dietcode.Database.Attribute
{
    public class TableNameAttribute : TableAttribute
    {
        public TableNameAttribute(string tableName) : base(tableName)
        {
        }
    }
}
