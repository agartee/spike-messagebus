using Azure.Messaging.ServiceBus;
using Spike.Common;
using Spike.Common.Models;
using Spike.Common.Services;
using System.Text;

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
            var messages = await messageOutboxReader.GetNextBatch();

            foreach (var msgInfo in messages)
            {
                try
                {
                    var serviceBusMessage = BuildServiceBusMessage(msgInfo);

                    await serviceBusSender.SendMessageAsync(serviceBusMessage.Value, cancellationToken);
                    await messageOutboxReader.ReportSuccess(msgInfo.Id);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to send message ID {Id}", msgInfo.Id);
                    await messageOutboxReader.ReportFailure(msgInfo.Id);
                }
            }
        }

        private static Result<ServiceBusMessage> BuildServiceBusMessage(DomainMessageInfo msgInfo)
        {
            var body = Encoding.UTF8.GetBytes(msgInfo.Body);

            var sbMessage = new ServiceBusMessage(body)
            {
                MessageId = msgInfo.Id.ToString(),
                CorrelationId = msgInfo.CorrelationId.ToString(),
                ContentType = "application/json",
                Subject = msgInfo.TypeName,
                
                ApplicationProperties = { ["MessageType"] = msgInfo.TypeName }
            };
            return Result<ServiceBusMessage>.Success(sbMessage);
        }
    }
}
