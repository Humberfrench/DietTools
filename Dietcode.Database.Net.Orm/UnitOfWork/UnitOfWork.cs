using Credpay.Tools.DomainValidator;
using Dietcode.Core.DomainValidator;
using Dietcode.Database.Net.Domain;
using Dietcode.Database.Net.Orm.Context;
using System;

namespace Dietcode.Database.Net.Domain.UnitOfWork
{
    public class UnitOfWork<T> : IUnitOfWork<T> where T : class, new()
    {

        private readonly ThisDatabase<T> dbContext;
        private readonly ValidationResult<T> validationResult;

        private bool _disposed;

        public UnitOfWork(IContextManager<ThisDatabase<T>> contextManager)
        {
            dbContext = contextManager.GetContext();
            validationResult = new ValidationResult<T>();
        }

        public void BeginTransaction()
        {
            _disposed = false;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public ValidationResult<T> SaveChanges()
        {
            try
            {
                var dados = dbContext.SaveChanges();
            }
            //EntityValidationException
            catch (Exception ex)
            {
                if (ex.Message == "An error occurred while updating the entries. See the inner exception for details.")
                {
                    var inner = ex.InnerException;
                    if (inner != null)
                    {
                        if (inner.Message == "An error occurred while updating the entries. See the inner exception for details.")
                        {
                            var inner2 = inner.InnerException;
                            if (inner2 != null)
                            {
                                validationResult.Add(inner2.Message);
                            }
                        }
                        else
                        {
                            validationResult.Add(inner.Message);
                        }
                    }
                }
                else
                {
                    validationResult.Add(ex.Message);
                }
            }
            return validationResult;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    dbContext.Dispose();
                }
            }
            _disposed = true;
        }
    }
}
