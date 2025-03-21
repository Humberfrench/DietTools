using Credpay.Tools.DomainValidator;

namespace Dietcode.Database.Net.Domain
{
    public interface IMyUnitOfWork<T> where T : class, new()
    {
        void BeginTransaction();
        ValidationResult<T> SaveChanges();
    }
}
