namespace Dietcode.Core.Domain.Rules.Interfaces
{
    public interface IRule<in TEntity>
    {
        string MensagemErro { get; }
        bool Validar(TEntity entity);
    }
}
