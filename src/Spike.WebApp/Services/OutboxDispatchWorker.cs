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
            /*
             * Subject Naming Guidelines for Service Bus Messages
             * --------------------------------------------------
             * A good Subject should reflect the domain, entity, and action to ensure clarity and consistency
             * across services, teams, and subscribers.
             *
             * Recommended Patterns:
             *
             *   Format                          Example                     Notes
             *   ------------------------------  --------------------------  -------------------------------
             *   Entity.Action                   "Person.Created"            Most common in event-driven systems
             *   Domain.Entity.Action           "HR.Person.Created"         Ideal for multi-team/bounded contexts
             *   Entity.Action.Version          "Person.Created.v1"         Useful when versioning message contracts
             *   Action.Entity                  "Created.Person"            Less common; harder to scan consistently
             *   Area.Entity.EventType          "Identity.User.PasswordReset" Clear, scalable, and descriptive
             *   CommandType:Entity             "Create:Person"             Useful if using Subject for command routing
             *
             * Tips:
             * - Use PascalCase for consistency (e.g., "Person.Created")
             * - Be explicit about context when messages cross service boundaries
             * - Avoid ambiguous names like "Created" or "Deleted" without entity/domain prefix
             * - Consider adding version suffixes to prepare for schema evolution
             */

            var body = Encoding.UTF8.GetBytes(msgInfo.Body);

            var sbMessage = new ServiceBusMessage(body)
            {
                MessageId = msgInfo.Id.ToString(),
                CorrelationId = msgInfo.CorrelationId.ToString(),
                ContentType = "application/json",
                Subject = ExtractSimpleTypeName(msgInfo.TypeName),
                ApplicationProperties = { ["TypeName"] = msgInfo.TypeName }
            };
            return Result<ServiceBusMessage>.Success(sbMessage);
        }

        public static string ExtractSimpleTypeName(string fullTypeNameOrAssemblyQualifiedName)
        {
            if (string.IsNullOrWhiteSpace(fullTypeNameOrAssemblyQualifiedName))
                throw new ArgumentException("Type name must not be null or empty.", nameof(fullTypeNameOrAssemblyQualifiedName));

            var typeNameOnly = fullTypeNameOrAssemblyQualifiedName.Split(',')[0].Trim();

            var lastDotIndex = typeNameOnly.LastIndexOf('.');
            return lastDotIndex >= 0
                ? typeNameOnly[(lastDotIndex + 1)..]
                : typeNameOnly;
        }
    }
}
