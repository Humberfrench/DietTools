using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dietcode.Database.Domain
{
    public interface IContextManager<ContextT>
    {
        ContextT GetContext();
    }
}
