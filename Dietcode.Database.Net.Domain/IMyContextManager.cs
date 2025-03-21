namespace Dietcode.Database.Net.Domain
{
    public interface IMyContextManager<ContextT>
    {
        ContextT GetContext();
    }
}
