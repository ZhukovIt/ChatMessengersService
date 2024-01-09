using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiMed.Clinic.DataModel
{
    public class SourceMessage
    {
        public string Id { get; set; }

        public MessengersType Type { get; set; }

        public string Login { get; set; }
    }
}
