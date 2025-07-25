﻿using Spike.Common.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spike.SqlServer.Models
{
    [Table(TableName)]
    public class MessageData
    {
        public const string TableName = "MessageOutbox";

        public required Guid Id { get; set; }
        public required Guid CorrelationId { get; set; }
        [MaxLength(200)]
        public required string TypeName { get; set; }
        public required string Body { get; set; }
        public required DateTime Created { get; set; }
        public required int CommitSequence { get; set; }
        public MessageStatus Status { get; set; } = MessageStatus.Pending;
        public int SendAttemptCount { get; set; } = 0;
        public DateTime? LastDequeuedAt { get; set; }
    }
}
