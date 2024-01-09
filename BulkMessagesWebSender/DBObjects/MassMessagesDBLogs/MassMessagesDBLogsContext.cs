using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMessagesWebServer.DBObjects.MassMessagesDBLogs
{
    public sealed class MassMessagesDBLogsContext : DbContext
    {
        public MassMessagesDBLogsContext(string _NameConnectionString) : base(_NameConnectionString)
        {

        }
        //--------------------------------------------------------------------------------------------
        public DbSet<MessageLog> MessageLogs { get; set; }
        //--------------------------------------------------------------------------------------------
        public DbSet<MessageBatch> MessageBatches { get; set; }
        //--------------------------------------------------------------------------------------------
    }
}
