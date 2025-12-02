using Dietcode.Core.DomainValidator;
using Dietcode.Core.Lib;
using Dietcode.Database.Domain;
using Dietcode.Database.Orm.Context;
using Dietcode.Database.Orm.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dietcode.Database.Orm.UnitOfWork
{
    public class MyUnitOfWork<T> : IMyUnitOfWork<T>, IDisposable where T : class, new()
    {
        private readonly ThisDatabase<T> dbContext;
        private readonly ValidationResult<T> validationResult;
        private bool _disposed;
        private ILogger logger =>InternalOrmLoggerFactory.Instance.CreateLogger<MyUnitOfWork<T>>();

        public MyUnitOfWork(ThisDatabase<T> dbContext)
        {
            this.dbContext = dbContext;
            validationResult = new ValidationResult<T>();
        }

        public void BeginTransaction()
        {
            // Se quiser implementar transação explícita depois, é aqui.
            _disposed = false;
        }

        public ValidationResult<T> SaveChanges()
        {
           logger.LogInformation("UoW SaveChanges for {Entity} at {TimeUtc}",typeof(T).Name,DateTime.UtcNow);
           try
            {
                var affected = dbContext.SaveChanges();
                var entries = dbContext.ChangeTracker.Entries().ToList();
                var keys = GetPrimaryKeyValues();
                

                validationResult.Mensagem = $"Dados salvos com sucesso. Total: {entries.Count}";
                logger.LogInformation("Dados salvos com sucesso. Total: {count}", affected);
            }
            catch (Exception ex)
            {
                var mensagens = ColetarMensagens(ex);
                validationResult.Mensagem = string.Join(" | ", mensagens);
                logger.LogInformation("UoW Error SaveChanges for {Entity} at {TimeUtc}, {mensagem}", 
                                       typeof(T).Name, DateTime.UtcNow, mensagens.ToJson());

                foreach (var msg in mensagens)
                {
                    validationResult.AddError(msg);
                }
            }

            return validationResult;
        }

        private static List<string> ColetarMensagens(Exception ex)
        {
            var mensagens = new List<string>();

            var atual = ex;
            while (atual != null)
            {
                if (!string.IsNullOrWhiteSpace(atual.Message))
                    mensagens.Add(atual.Message);

                atual = atual.InnerException;
            }

            return mensagens;
        }

        public List<Entries> GetPrimaryKeyValues()
        {
            var modifiedEntities = dbContext.ChangeTracker.Entries().ToList();
            var entries = new List<Entries>();

            foreach (var change in modifiedEntities)
            {
                object? keyValue = null;

                var pkProperty = change.Properties
                    .FirstOrDefault(p => p.Metadata.IsPrimaryKey());

                if (pkProperty != null)
                {
                    keyValue = pkProperty.CurrentValue;
                }

                entries.Add(new Entries
                {
                    EntryName = change.Entity.GetType().Name,
                    EntryKeyValue = keyValue ?? string.Empty
                });
            }

            return entries;
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
