using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMessagesWebServer.DBObjects.OptionsContext
{
    public sealed class OptionsContext : DbContext
    {
        public OptionsContext(string _NameConnectionString) : base(_NameConnectionString)
        {

        }
        //-------------------------------------------------------------------------------------------
        public DbSet<Option> Options { get; set; }
        //-------------------------------------------------------------------------------------------
    }
}
