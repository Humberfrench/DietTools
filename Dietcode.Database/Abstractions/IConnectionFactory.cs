using System.Data;

namespace Dietcode.Database.Abstractions
{
    public interface IConnectionFactory
    {
        IDbConnection Create();
    }
}
