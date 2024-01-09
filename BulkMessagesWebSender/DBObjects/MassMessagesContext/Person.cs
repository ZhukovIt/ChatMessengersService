using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkMessagesWebServer.DBObjects.MassMessages
{
    [Table("PERSON")]
    public class Person
    {
        [Column("PER_ID")]
        public int Id { get; set; }
    }
}
