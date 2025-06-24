using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Spike.Domain.Services;
using Spike.Messaging.Models;
using Spike.SqlServer;
using Spike.SqlServer.Extensions;

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
            return string.Format(
                @"SET NOCOUNT ON;
	            
                DECLARE @results TABLE 
	            (
		            [Id] UNIQUEIDENTIFIER
                    , [TypeName] VARCHAR(200)
		            , [Json] VARCHAR(MAX)
		            , [Created] DATETIME
		            , [CommitSequence] INT
                    , [IsSending] BIT
                    , [SendAttemptCount] INT
                    , [LastSendAttempt] DATETIME
	            )

	            INSERT INTO @results (
                      [Id]
                    , [TypeName]
                    , [Json]
                    , [Created]
                    , [CommitSequence]
                    , [IsSending]
                    , [SendAttemptCount]
                    , [LastSendAttempt]
                )
	            SELECT TOP ({0}) 
                      [Id]
                    , [TypeName]
                    , [Json]
                    , [Created]
                    , [CommitSequence]
                    , [IsSending]
                    , [SendAttemptCount]
                    , [LastSendAttempt]
	            FROM 
                    [MessageOutbox] WITH (ROWLOCK, UPDLOCK, READPAST)
	            WHERE 
                    [isSending] = 0
                    AND (
                        [LastSendAttempt] IS NULL 
                        OR GETUTCDATE() >= DATEADD(second, @retryAfterSeconds, [LastSendAttempt])
                    )
                    AND [SendAttemptCount] < @maxRetries
                ORDER BY 
                      [Created]
                    , [CommitSequence]

	            UPDATE 
                    [MessageOutbox]
	            SET 
                    [IsSending] = 1
	            WHERE 
                    [Id] IN (SELECT [Id] FROM @results)
	
	            SELECT * from @results",
                maxCountParam.Value).TrimExtraWhitespace();
        }
    }
}
