using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dietcode.Database
{
    public interface IConnectionFactory
    {
        IDbConnection Connection();
    }
}
