using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BulkMessagesWebServer.DBObjects.OptionsContext
{
    [Table("OPTIONS")]
    public class Option
    {
        [Key]
        [Column("OPT_NAME")]
        public string Name { get; set; }

        [Column("OPT_VALUE")]
        public string Value { get; set; }
    }
}
