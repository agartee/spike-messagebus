using Microsoft.EntityFrameworkCore;
using Spike.Messaging.SqlServer.Models;

namespace Spike.Messaging.SqlServer.Services
{
    public interface IMessageOutbox
    {
        DbSet<MessageData> MessageOutbox { get; set; }
    }
}
