namespace Dietcode.Database.Domain
{
    public interface IMyContextManager<ContextT>
    {
        ContextT GetContext();
    }
}
