using Azure.Messaging.ServiceBus;
using Spike.Domain.Services;
using System.Text.Json;

namespace Spike.WebApp.Services
{
    public class OutboxDispatchWorker : IOutboxDispatchWorker
    {
        private readonly IMessageOutboxReader messageOutboxReader;
        private readonly ServiceBusSender serviceBusSender;
        private readonly ILogger<OutboxDispatchWorker> logger;

        public OutboxDispatchWorker(
            IMessageOutboxReader messageOutboxReader,
            ServiceBusSender serviceBusSender,
            ILogger<OutboxDispatchWorker> logger)
        {
            this.messageOutboxReader = messageOutboxReader;
            this.serviceBusSender = serviceBusSender;
            this.logger = logger;
        }

        public async Task DispatchPendingMessages(CancellationToken cancellationToken)
        {
            var messages = await messageOutboxReader.GetNextBatch(batchSize: 10, retryAfterMins: 1, maxRetries: 5);

            foreach (var msgInfo in messages)
            {
                try
                {
                    var messageType = ResolveMessageType(msgInfo.TypeName);
                    if (messageType == null)
                    {
                        logger.LogWarning("Unknown message type: {TypeName}", msgInfo.TypeName);
                        await messageOutboxReader.ReportFailure(msgInfo.Id);
                        continue;
                    }

                    var serviceBusMessage = BuildServiceBusMessage(
                        JsonSerializer.Deserialize(msgInfo.Body, messageType)!,
                        messageType);

                    await serviceBusSender.SendMessageAsync(serviceBusMessage, cancellationToken);
                    await messageOutboxReader.ReportSuccess(msgInfo.Id);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to send message ID {Id}", msgInfo.Id);
                    await messageOutboxReader.ReportFailure(msgInfo.Id);
                }
            }
        }

        private static Type? ResolveMessageType(string? typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName)) return null;

            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.FullName == typeName);
        }

        private static ServiceBusMessage BuildServiceBusMessage(object message, Type type)
        {
            var body = JsonSerializer.SerializeToUtf8Bytes(message, type);
            var sbMessage = new ServiceBusMessage(body)
            {
                ApplicationProperties = { [".NET_Type"] = type.FullName! }
            };
            return sbMessage;
        }
    }
}
