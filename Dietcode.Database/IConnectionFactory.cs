using System.Data;

namespace Dietcode.Database
{
    public interface IConnectionFactory
    {
        IDbConnection Connection();
    }
}
