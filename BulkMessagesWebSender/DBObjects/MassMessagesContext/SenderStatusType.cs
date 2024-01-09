using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMessagesWebServer.DBObjects.MassMessages
{
    [Table("MES_UMNICO_SENDER_STATUS_TYPE")]
    public sealed class SenderStatusType
    {
        [Column("MES_UMN_SEND_STAT_TYPE_ID")]
        public int Id { get; set; }

        [Column("MES_UMN_SEND_STAT_TYPE_NAME")]
        public string Name { get; set; }

        public List<Sender> Senders { get; set; }

        public SenderStatusType()
        {
            Senders = new List<Sender>();
        }
    }
}
