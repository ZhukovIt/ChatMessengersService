using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkMessagesWebServer.DBObjects.MessengerDialog
{
    [Table("SOURCE_TYPE")]
    public sealed class SourceType
    {
        [Column("SOURCE_TYPE_ID")]
        public int Id { get; set; }

        [Column("MESSENGER_TYPE_ID")]
        public int MessengerTypeId { get; set; }

        [Column("SOURCE_TYPE_UID")]
        public string SourceTypeUId { get; set; }
    }
}
