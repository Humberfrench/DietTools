using Dietcode.Core.DomainValidator;
using Dietcode.Database.Domain;
using Dietcode.Database.Orm.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Dietcode.Database.Orm.UnitOfWork
{
    public class MyUnitOfWork<T> : IMyUnitOfWork<T> where T : class, new()
    {

        private readonly ThisDatabase<T> dbContext;
        private readonly ValidationResult<T> validationResult;

        private bool _disposed;

        public MyUnitOfWork(ThisDatabase<T> dbContext)
        {
            this.dbContext = dbContext;
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
                var entries = dbContext.ChangeTracker.Entries().ToList();
                var keys = GetPrimaryKeyValues();
                validationResult.Mensagem = $"Dados salvos com sucesso. Total {entries.Count}";
            }
            //EntityValidationException
            catch (Exception ex)
            {
                var mensagem = ColetarMensagemCompleta(ex);
                validationResult.Mensagem = mensagem;
                //validationResult.Add(mensagem);
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
                                if (inner2.Message == "An error occurred while updating the entries. See the inner exception for details.")
                                {
                                    var inner3 = inner.InnerException;
                                    if (inner3 != null)
                                    {
                                        validationResult.Add(inner3.Message);
                                    }
                                }
                                else
                                {
                                    validationResult.Add(inner2.Message);
                                }
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

        string ColetarMensagemCompleta(Exception ex)
        {
            var mensagem = ex.Message;
            if (ex.InnerException != null)
            {
                mensagem += ex.InnerException.Message;
                if (ex.InnerException.InnerException != null)
                {
                    mensagem += ex.InnerException.InnerException.Message;
                    if (ex.InnerException.InnerException.InnerException != null)
                    {
                        mensagem += ex.InnerException.InnerException.InnerException.Message;
                    }
                }
            }
            return mensagem;
        }
        public List<Entries> GetPrimaryKeyValues()
        {
            //var modifiedEntities = ChangeTracker.Entries().Where(p => p.State == EntityState.Added).ToList();
            //var entries = dbContext.ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
            var modifiedEntities = dbContext.ChangeTracker.Entries().ToList();
            var entries = new List<Entries>();

            foreach (var change in modifiedEntities)
            {

                entries.Add(new Entries
                {
                    EntryName = change.Entity.GetType().Name,
                    EntryKeyValue = ""//this.GetPrimaryKeyValue(change)
                });
            }

            return entries;
        }
        //public object GetPrimaryKeyValue(EntityEntry entry)
        //{
        //    var objectStateEntry = ((IObjectContext)this).ObjectContext.ObjectStateManager.GetObjectStateEntry(entry.Entity);
        //    dbContext.ChangeTracker.
        //    try
        //    {
        //        return objectStateEntry.EntityKey.EntityKeyValues[0].Value;
        //    }
        //    catch (Exception)
        //    {
        //        return "0";
        //    }

        //}


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
