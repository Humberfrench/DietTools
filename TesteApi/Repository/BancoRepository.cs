using Dietcode.Database.Domain;
using Dietcode.Database.Orm;
using Dietcode.Database.Orm.Context;
using TesteApi.Domain;
using TesteApi.Interfaces;

namespace TesteApi.Repository
{
    public sealed class BancoRepository(IMyContextManager<ThisDatabase<Banco>> contextManager)
        : BaseRepository<Banco, int>(contextManager),
          IBancoRepository,
          IBaseRepository<Banco, int>
    {
    }
}
