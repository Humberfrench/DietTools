using System.Data;

namespace Dietcode.Database.Interfaces
{
    public interface IConnectionFactory
    {
        IDbConnection Connection();
    }
}
