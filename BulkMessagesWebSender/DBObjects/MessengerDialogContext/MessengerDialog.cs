using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkMessagesWebServer.DBObjects.MessengerDialog
{
    [Table("MESSENGER_DIALOG")]
    public sealed class MessengerDialog
    {
        [Column("MES_DIAL_ID")]
        public int Id { get; set; }

        [Column("PER_MES_TYPE_ID")]
        public int? PersonMessengerTypeId { get; set; }

        [Column("MES_DIAL_TYPE_ID")]
        public int MessengerDialogTypeId { get; set; }

        [Column("MES_DIAL_EXTERNAL_ID")]
        public string MessengerDialogUId { get; set; }

        [Column("SOURCE_TYPE_ID")]
        public int SourceTypeId { get; set; }

        [Column("MES_DIAL_CLIENT_LOGIN")]
        public string Login { get; set; }

        [Column("MES_DIAL_CLIENT_PHONE")]
        public string Phone { get; set; }
    }
}
