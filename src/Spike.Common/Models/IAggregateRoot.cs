namespace Spike.Common.Models
{
    public interface IAggregateRoot
    {
        object GetGenericId();
    }

    public interface IAggregateRoot<TId> : IAggregateRoot where TId : struct, IStronglyTypedId
    {
        TId Id { get; }
    }
}
