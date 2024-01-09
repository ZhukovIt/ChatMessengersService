using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkMessagesWebServer.DBObjects.CreateMessengerDialog
{
    [Table("MESSENGER_DIALOG")]
    public sealed class CreationMessengerDialog
    {
        [Column("MES_DIAL_ID")]
        public int Id { get; set; }

        [Column("SOURCE_TYPE_ID")]
        public int SourceTypeId { get; set; }

        [Column("MES_DIAL_TYPE_ID")]
        public int TypeId { get; set; }

        [Column("MES_DIAL_STAT_TYPE_ID")]
        public int StatusTypeId { get; set; }

        [Column("MES_DIAL_CREATE_DATE")]
        public DateTime CreationDateTime { get; set; }

        [Column("MES_DIAL_CLIENT_LOGIN")]
        public string Login { get; set; }

        [Column("MES_DIAL_CLIENT_PHONE")]
        public string Phone { get; set; }

        [Column("MES_DIAL_MESSAGE_IS_READ")]
        public byte IsRead { get; set; }

        [Column("PER_MES_TYPE_ID")]
        public int? PersonMessengerTypeId { get; set; }
    }
}
