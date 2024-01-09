using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkMessagesWebServer.DBObjects.MessengerDialog
{
    [Table("PERSON")]
    public sealed class Person
    {
        [Column("PER_ID")]
        public int Id { get; set; }

        [Column("MESSENGER_TYPE_ID")]
        public int? PriorityMessengerTypeId { get; set; }

        [Column("PER_LIVE_ADDR")]
        public int? PersonLiveAddress { get; set; }

        [Column("PER_REG_ADDR")]
        public int? PersonRegistrationAddress { get; set; }
    }
}
