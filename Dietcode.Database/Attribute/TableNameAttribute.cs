using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
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
