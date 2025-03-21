using Dapper.Contrib.Extensions;

namespace Dietcode.Database.Attribute
{
    public class WriteColAttribute : WriteAttribute
    {
        public WriteColAttribute(bool write) : base(write)
        {
        }
    }
}
