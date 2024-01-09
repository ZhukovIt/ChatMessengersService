using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkMessagesWebServer.DBObjects.MassMessagesDBLogs
{
    [Table("MES_BATCH")]
    public sealed class MessageBatch
    {
        [Column("MES_BAT_ID")]
        public int Id { get; set; }

        [Column("MES_BAT_EXT_ID")]
        public string ExternalId { get; set; }
    }
}
