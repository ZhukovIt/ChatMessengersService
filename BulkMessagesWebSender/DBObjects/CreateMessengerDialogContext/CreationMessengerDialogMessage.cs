using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkMessagesWebServer.DBObjects.CreateMessengerDialog
{
    [Table("MESSENGER_DIALOG_MESSAGE")]
    public sealed class CreationMessengerDialogMessage
    {
        [Column("MES_DIAL_MES_ID")]
        public int Id { get; set; }

        [Column("MES_TYPE_ID")]
        public int TypeId { get; set; }

        [Column("MES_STAT_TYPE_ID")]
        public int StatusTypeId { get; set; }

        [Column("MES_DIAL_ID")]
        public int MessengerDialogId { get; set; }

        [Column("MES_DIAL_MES_TEXT")]
        public string Text { get; set; }

        [Column("MES_DIAL_MES_DEPARTURE_DATE")]
        public DateTime CreationDateTime { get; set; }

        [Column("MES_DIAL_MES_GUID")]
        public string Guid { get; set; }

        [Column("SEC_USER_FIO")]
        public string AuthorName { get; set; }
    }
}
