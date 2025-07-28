using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Spike.Messaging.Models;
using Spike.Messaging.Services;
using Spike.Messaging.SqlServer.Extensions;
using Spike.Messaging.SqlServer.Models;
using System.Collections.Immutable;

namespace Spike.Messaging.SqlServer.Services
{

    public class SqlServerMessageOutboxReader<TDbContext> : IMessageOutboxReader
        where TDbContext : DbContext, IMessageOutbox
    {
        private readonly TDbContext dbContext;
        private readonly SqlServerMessageOutboxOptions options;

        public SqlServerMessageOutboxReader(TDbContext dbContext, SqlServerMessageOutboxOptions options)
        {
            this.dbContext = dbContext;
            this.options = options;
        }

        public async Task<IEnumerable<DomainMessageInfo>> GetNextBatch()
        {
            var maxCountParam = new SqlParameter(
                "@maxCount", options.BatchSize);
            var retryAfterSecondsParam = new SqlParameter(
                "@retryAfterSeconds", options.RetryAfterSeconds);
            
            var sql = CreateDequeueQuery().ToCompactSql();

            var data = await dbContext.MessageOutbox
                .FromSqlRaw(sql, maxCountParam, retryAfterSecondsParam)
                .AsNoTracking()
                .ToListAsync();

            return data
                .Select(m => new DomainMessageInfo
                {
                    Id = m.Id,
                    CorrelationId = m.CorrelationId,
                    Body = m.Body,
                    TypeName = m.TypeName,
                    Created = m.Created,
                    CommitSequence = m.CommitSequence,
                    Status = m.Status,
                    SendAttemptCount = m.SendAttemptCount,
                    LastDequeuedAt = m.LastDequeuedAt
                })
                .ToImmutableList();
        }

        public async Task ReportFailure(Guid domainMessageId)
        {
            var data = await dbContext.MessageOutbox
                .Where(m => m.Id == domainMessageId)
                .SingleOrDefaultAsync();

            if (data is null)
                return;

            data.Status = data.SendAttemptCount <= options.MaxRetries
                ? MessageStatus.Pending
                : MessageStatus.Failed;

            await dbContext.SaveChangesAsync();
        }

        public async Task ReportSuccess(Guid domainMessageId)
        {
            var data = await dbContext.MessageOutbox
                .Where(m => m.Id == domainMessageId)
                .SingleOrDefaultAsync();

            if (data != null)
                data.Status = MessageStatus.Sent;

            await dbContext.SaveChangesAsync();
        }

        private string CreateDequeueQuery()
        {
            var tableName = $"[{options.SchemaName}].[{MessageData.TableName}]";

            return 
                @$"SET NOCOUNT ON;
	            
                DECLARE @results TABLE 
	            (
		            [{nameof(MessageData.Id)}] UNIQUEIDENTIFIER		            
                    , [{nameof(MessageData.CorrelationId)}] UNIQUEIDENTIFIER
                    , [{nameof(MessageData.TypeName)}] VARCHAR(200)
		            , [{nameof(MessageData.Body)}] VARCHAR(MAX)
		            , [{nameof(MessageData.Created)}] DATETIME
		            , [{nameof(MessageData.CommitSequence)}] INT
                    , [{nameof(MessageData.Status)}] INT
                    , [{nameof(MessageData.SendAttemptCount)}] INT
                    , [{nameof(MessageData.LastDequeuedAt)}] DATETIME
	            )

	            INSERT INTO @results (
                      [{nameof(MessageData.Id)}]
                    , [{nameof(MessageData.CorrelationId)}]                    
                    , [{nameof(MessageData.TypeName)}]
                    , [{nameof(MessageData.Body)}]
                    , [{nameof(MessageData.Created)}]
                    , [{nameof(MessageData.CommitSequence)}]
                    , [{nameof(MessageData.Status)}]
                    , [{nameof(MessageData.SendAttemptCount)}]
                    , [{nameof(MessageData.LastDequeuedAt)}]
                )
	            SELECT TOP (@maxCount) 
                      [{nameof(MessageData.Id)}]
                    , [{nameof(MessageData.CorrelationId)}] 
                    , [{nameof(MessageData.TypeName)}] 
                    , [{nameof(MessageData.Body)}]
                    , [{nameof(MessageData.Created)}]
                    , [{nameof(MessageData.CommitSequence)}]
                    , [{nameof(MessageData.Status)}]
                    , [{nameof(MessageData.SendAttemptCount)}]
                    , [{nameof(MessageData.LastDequeuedAt)}]
	            FROM 
                    {tableName} WITH (ROWLOCK, UPDLOCK, READPAST)
	            WHERE 
                    [{nameof(MessageData.Status)}] = {(int)MessageStatus.Pending}
                    AND (
                        [{nameof(MessageData.LastDequeuedAt)}] IS NULL 
                        OR GETUTCDATE() >= DATEADD(second, @retryAfterSeconds, [{nameof(MessageData.LastDequeuedAt)}])
                    )
                ORDER BY 
                      [{nameof(MessageData.Created)}]
                    , [{nameof(MessageData.CommitSequence)}]

	            UPDATE 
                    {tableName}
	            SET 
                    [{nameof(MessageData.Status)}] = {(int)MessageStatus.Sending},
                    [{nameof(MessageData.SendAttemptCount)}] = [{nameof(MessageData.SendAttemptCount)}] + 1,
                    [{nameof(MessageData.LastDequeuedAt)}] = GETUTCDATE()
	            WHERE 
                    [{nameof(MessageData.Id)}] IN (SELECT [{nameof(MessageData.Id)}] FROM @results)
	
	            SELECT * from @results";
        }
    }
}
