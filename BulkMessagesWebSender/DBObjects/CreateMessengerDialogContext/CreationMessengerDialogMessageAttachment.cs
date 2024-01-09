using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkMessagesWebServer.DBObjects.CreateMessengerDialog
{
    [Table("MESSENGER_DIALOG_MESSAGE_ATTACHMENT")]
    public sealed class CreationMessengerDialogMessageAttachment
    {
        [Column("MES_DIAL_MES_ATT_ID")]
        public int Id { get; set; }

        [Column("MES_DIAL_MES_ID")]
        public int MessengerDialogMessageId { get; set; }

        [Column("MES_DIAL_MES_ATT_NAME")]
        public string Name { get; set; }

        [Column("MES_DIAL_MES_ATT_DATA")]
        public string Data { get; set; }
    }
}
