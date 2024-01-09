using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkMessagesWebServer.DBObjects.MessengerDialog
{
    [Table("ADDRESS")]
    public sealed class Address
    {
        [Column("ADDR_ID")]
        public int Id { get; set; }

        [Column("ADDR_PHONE")]
        public string FirstPhone { get; set; }

        [Column("ADDR_PHONE2")]
        public string SecondPhone { get; set; }
    }
}
