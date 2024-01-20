using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkMessagesWebServer.DBObjects.MassMessages
{
    [Table("MES_UMNICO_SENDER")]
    public class Sender
    {
        [Column("MES_UMN_SEND_ID")]
        public int Id { get; set; }

        [Column("MES_UMN_SEND_GUID")]
        public string Guid { get; set; }

        [Column("MES_UMN_SEND_TEXT")]
        public string Text { get; set; }

        [Column("MES_UMN_SEND_STAT_TYPE_ID")]
        public int SenderStatusTypeId { get; set; }

        [Column("MES_UMN_SEND_IMG_NAME")]
        public string ImageName { get; set; }

        [Column("PER_ID")]
        public int PersonId { get; set; }

        [Column("BRANCH_NAME")]
        public string BranchName { get; set; }
    }
}
