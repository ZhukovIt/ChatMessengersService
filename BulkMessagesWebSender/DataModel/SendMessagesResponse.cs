using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMessagesWebServer
{
    [Serializable]
    public sealed class SendMessagesResponse
    {
        public bool Success { get; set; }

        public string InfoMessage { get; set; }

        public Dictionary<string, string> MesUmnGuids { get; set; }
    }
}
