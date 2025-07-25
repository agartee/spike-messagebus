using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Spike.Common.Models;
using Spike.Common.Services;
using Spike.SqlServer;
using Spike.SqlServer.Models;

namespace Spike.WebApp.Services
{
    public class SqlServerMessageOutboxReader : IMessageOutboxReader
    {
        private readonly SpikeDbContext dbContext;

        public SqlServerMessageOutboxReader(SpikeDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<DomainMessageInfo>> GetNextBatch(int batchSize, int retryAfterSeconds, int maxRetries)
        {
            var maxCountParam = new SqlParameter(
                "@maxCount", batchSize);
            var minRetryDateTimeParam = new SqlParameter(
                "@retryAfterSeconds", retryAfterSeconds);
            var maxRetriesParam = new SqlParameter(
                "@maxRetries", maxRetries);

            var sql = CreateDequeueQuery(maxCountParam);

            var data = await dbContext.MessageOutbox
                .FromSqlRaw(sql, maxCountParam, minRetryDateTimeParam, maxRetriesParam)
                .AsNoTracking()
                .ToListAsync();

            return data
                .Select(m => new DomainMessageInfo
                {
                    Id = m.Id,
                    Body = m.Body,
                    TypeName = m.TypeName,
                    Created = m.Created,
                    CommitSequence = m.CommitSequence,
                    SendAttemptCount = m.SendAttemptCount,
                    LastSendAttempt = m.LastSendAttempt
                })
                .ToList();
        }

        public async Task ReportFailure(Guid domainMessageId)
        {
            var data = await dbContext.MessageOutbox
                .Where(m => m.Id == domainMessageId)
                .SingleOrDefaultAsync();

            if (data is null)
                return;

            data.IsSending = false;
            data.SendAttemptCount = data.SendAttemptCount++;
            data.LastSendAttempt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();
        }

        public async Task ReportSuccess(Guid domainMessageId)
        {
            var data = await dbContext.MessageOutbox
                .Where(m => m.Id == domainMessageId)
                .SingleOrDefaultAsync();

            if (data != null)
                dbContext.MessageOutbox.Remove(data);

            await dbContext.SaveChangesAsync();
        }

        private static string CreateDequeueQuery(SqlParameter maxCountParam)
        {
            const string tableName = $"[{SpikeDbContext.SchemaName}].[{MessageData.TableName}]";

            return string.Format(
                @$"SET NOCOUNT ON;
	            
                DECLARE @results TABLE 
	            (
		            [{nameof(MessageData.Id)}] UNIQUEIDENTIFIER
                    , [{nameof(MessageData.TypeName)}] VARCHAR(200)
		            , [{nameof(MessageData.Body)}] VARCHAR(MAX)
		            , [{nameof(MessageData.Created)}] DATETIME
		            , [{nameof(MessageData.CommitSequence)}] INT
                    , [{nameof(MessageData.IsSending)}] BIT
                    , [{nameof(MessageData.SendAttemptCount)}] INT
                    , [{nameof(MessageData.LastSendAttempt)}] DATETIME
	            )

	            INSERT INTO @results (
                      [{nameof(MessageData.Id)}]
                    , [{nameof(MessageData.TypeName)}]
                    , [{nameof(MessageData.Body)}]
                    , [{nameof(MessageData.Created)}]
                    , [{nameof(MessageData.CommitSequence)}]
                    , [{nameof(MessageData.IsSending)}]
                    , [{nameof(MessageData.SendAttemptCount)}]
                    , [{nameof(MessageData.LastSendAttempt)}]
                )
	            SELECT TOP (@maxCount) 
                      [{nameof(MessageData.Id)}]
                    , [{nameof(MessageData.TypeName)}] 
                    , [{nameof(MessageData.Body)}]
                    , [{nameof(MessageData.Created)}]
                    , [{nameof(MessageData.CommitSequence)}]
                    , [{nameof(MessageData.IsSending)}]
                    , [{nameof(MessageData.SendAttemptCount)}]
                    , [{nameof(MessageData.LastSendAttempt)}]
	            FROM 
                    {tableName} WITH (ROWLOCK, UPDLOCK, READPAST)
	            WHERE 
                    [{nameof(MessageData.IsSending)}] = 0
                    AND (
                        [{nameof(MessageData.LastSendAttempt)}] IS NULL 
                        OR GETUTCDATE() >= DATEADD(second, @retryAfterSeconds, [{nameof(MessageData.LastSendAttempt)}])
                    )
                    AND [{nameof(MessageData.SendAttemptCount)}] < @maxRetries
                ORDER BY 
                      [{nameof(MessageData.Created)}]
                    , [{nameof(MessageData.CommitSequence)}]

	            UPDATE 
                    {tableName}
	            SET 
                    [{nameof(MessageData.IsSending)}] = 1
	            WHERE 
                    [{nameof(MessageData.Id)}] IN (SELECT [{nameof(MessageData.Id)}] FROM @results)
	
	            SELECT * from @results",
                maxCountParam.Value);
        }
    }
}
