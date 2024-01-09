using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkMessagesWebServer.DBObjects.MessengerDialog
{
    [Table("MESSENGER_DIALOG_MESSAGE")]
    public sealed class MessengerDialogMessage
    {
        [Column("MES_DIAL_MES_ID")]
        public int Id { get; set; }

        [Column("MES_DIAL_ID")]
        public int MessengerDialogId { get; set; }

        [Column("MES_TYPE_ID")]
        public int MessengerDialogMessageTypeId { get; set; }
    }
}
