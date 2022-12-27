using Silverback.Messaging.Messages;
using Silverback.Messaging.Serialization;

namespace Silverback.Samples.Kafka.Batch.Producer
{
    internal class MyCustomSerializer : IMessageSerializer
    {
        public MyCustomSerializer()
        {
        }

        public ValueTask<Stream?> SerializeAsync(object? message, MessageHeaderCollection messageHeaders, MessageSerializationContext context)
        {
            throw new NotImplementedException();
        }

        public ValueTask<(object? Message, Type MessageType)> DeserializeAsync(Stream? messageStream, MessageHeaderCollection messageHeaders,
            MessageSerializationContext context)
        {
            throw new NotImplementedException();
        }

        public bool RequireHeaders { get; }
    }
}