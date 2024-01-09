using BulkMessagesWebServer.DBObjects.MassMessages;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMessagesWebServer.DBObjects
{
    public sealed class MassMessagesContext : DbContext
    {
        public MassMessagesContext(string _NameConnectionString) : base(_NameConnectionString)
        {
            SenderStatusTypes.Load();
        }
        //--------------------------------------------------------------------------------------
        public DbSet<Sender> Senders { get; set; }
        //--------------------------------------------------------------------------------------
        public DbSet<SenderStatusType> SenderStatusTypes { get; set; }
        //--------------------------------------------------------------------------------------
        public DbSet<Person> Persons { get; set; }
        //--------------------------------------------------------------------------------------
    }
}
