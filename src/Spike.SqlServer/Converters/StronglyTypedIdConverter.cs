using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Spike.SqlServer.Converters
{
    public class StronglyTypedIdConverter<TId> : ValueConverter<TId, Guid>
    where TId : struct
    {
        public StronglyTypedIdConverter(
            Func<Guid, TId> fromGuid,
            Func<TId, Guid> toGuid)
            : base(
                convertToProviderExpression: id => toGuid(id),
                convertFromProviderExpression: value => fromGuid(value))
        {
        }
    }
}
