using Credpay.Tools.DomainValidator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dietcode.Database.Net.Domain
{
    public interface IUnitOfWork<T> where T : class, new()
    {
        void BeginTransaction();
        ValidationResult<T> SaveChanges();
    }
}
