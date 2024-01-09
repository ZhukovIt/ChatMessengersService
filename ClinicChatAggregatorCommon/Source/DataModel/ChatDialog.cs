using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiMed.Clinic.DataModel
{
    public class ChatDialog
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public SourceMessage MessengerAccount { get; set; }

        public Customer Customer { get; set; }

        public DialogType DialogType { get; set; }

        public long? AnswerDateTime { get; set; }
    }

    public enum DialogType
    {
        /// <summary>
        /// Сообщение
        /// </summary>
        Message,
        /// <summary>
        /// Комментарий
        /// </summary>
        Comment
    }
}
