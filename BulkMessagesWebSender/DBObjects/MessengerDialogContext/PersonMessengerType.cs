using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkMessagesWebServer.DBObjects.MessengerDialog
{
    [Table("PERSON_MESSENGER_TYPES")]
    public sealed class PersonMessengerType
    {
        [Column("PER_MES_TYPE_ID")]
        public int Id { get; set; }

        [Column("MESSENGER_TYPE_ID")]
        public int MessengerTypeId { get; set; }

        [Column("PER_ID")]
        public int PersonId { get; set; }
    }
}
