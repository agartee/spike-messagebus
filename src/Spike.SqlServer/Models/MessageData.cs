using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spike.SqlServer.Models
{
    [Table(TABLE_NAME)]
    public class MessageData
    {
        public const string TABLE_NAME = "MessageOutbox";

        public required Guid Id { get; set; }
        [MaxLength(200)]
        public string? TypeName { get; set; }
        public required string Body { get; set; }
        public required DateTime Created { get; set; }
        public required int CommitSequence { get; set; }
        public bool IsSending { get; set; } = false;
        public int SendAttemptCount { get; set; } = 0;
        public DateTime? LastSendAttempt { get; set; }
    }
}
