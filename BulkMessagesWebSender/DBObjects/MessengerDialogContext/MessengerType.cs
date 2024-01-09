using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkMessagesWebServer.DBObjects.MessengerDialog
{
    [Table("MESSENGER_TYPE")]
    public sealed class MessengerType
    {
        [Column("MESSENGER_TYPE_ID")]
        public int Id { get; set; }

        [Column("MESSENGER_TYPE_NAME")]
        public string Name { get; set; }
    }
}
