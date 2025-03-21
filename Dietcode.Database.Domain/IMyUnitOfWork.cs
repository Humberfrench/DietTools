using Dietcode.Core.DomainValidator;

namespace Dietcode.Database.Domain
{
    public interface IMyUnitOfWork<T> where T : class, new()
    {
        void BeginTransaction();
        ValidationResult<T> SaveChanges();
    }
}
