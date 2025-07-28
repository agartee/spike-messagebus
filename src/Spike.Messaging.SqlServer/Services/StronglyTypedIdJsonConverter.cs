using Spike.Common.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Spike.Messaging.SqlServer.Services
{
    public class StronglyTypedIdJsonConverter<T> : JsonConverter<T>
    where T : struct, IStronglyTypedId
    {
        private static readonly Func<Guid, T> factory;

        static StronglyTypedIdJsonConverter()
        {
            var ctor = typeof(T).GetConstructor([typeof(Guid)]) 
                ?? throw new InvalidOperationException($"Type {typeof(T)} must have a constructor with a single Guid parameter.");
            
            factory = (Guid g) => (T)ctor.Invoke([g]);
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var guid = reader.GetGuid();
            
            return factory(guid);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value);
        }
    }
}
