using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkMessagesWebServer.DBObjects.MassMessagesDBLogs
{
    [Table("MES_LOG")]
    public sealed class MessageLog
    {
        [Column("MES_LOG_ID")]
        public int Id { get; set; }

        [Column("MES_BAT_ID")]
        public int? MessageBatchId { get; set; }

        [Column("MES_LOG_EXT_ID")]
        public string ExternalId { get; set; }

        [Column("MES_SEND_STATE_ID")]
        public int SendStatusId { get; set; }

        [Column("MES_LOG_SEND_ERROR")]
        public string SendErrorMessage { get; set; }

        [Column("PER_ID")]
        public int? PersonId { get; set; }
    }
}
