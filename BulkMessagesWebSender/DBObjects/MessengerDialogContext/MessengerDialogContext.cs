using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMessagesWebServer.DBObjects.MessengerDialog
{
    public sealed class MessengerDialogContext : DbContext
    {
        public MessengerDialogContext(string _NameConnectionString) : base(_NameConnectionString)
        {
            MessengerTypes.Load();
        }
        //-------------------------------------------------------------------------------------------
        public DbSet<Address> Addresses { get; set; }
        //-------------------------------------------------------------------------------------------
        public DbSet<Person> Persons { get; set; }
        //-------------------------------------------------------------------------------------------
        public DbSet<MessengerType> MessengerTypes { get; set; }
        //-------------------------------------------------------------------------------------------
        public DbSet<PersonMessengerType> PersonMessengerTypes { get; set; }
        //-------------------------------------------------------------------------------------------
        public DbSet<MessengerDialog> MessengerDialogs { get; set; }
        //-------------------------------------------------------------------------------------------
        public DbSet<MessengerDialogMessage> MessengerDialogMessages { get; set; }
        //-------------------------------------------------------------------------------------------
        public DbSet<SourceType> SourceTypes { get; set; }
        //-------------------------------------------------------------------------------------------
    }
}
