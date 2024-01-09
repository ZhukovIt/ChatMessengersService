using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMessagesWebServer.DBObjects.CreateMessengerDialog
{
    public sealed class CreateMessengerDialogContext : DbContext
    {
        public CreateMessengerDialogContext(string _NameConnectionString) : base(_NameConnectionString)
        {
            
        }
        //--------------------------------------------------------------------------------------
        public DbSet<CreationMessengerDialog> CreationMessengerDialogs { get; set; }
        //--------------------------------------------------------------------------------------
        public DbSet<CreationMessengerDialogMessage> CreationMessengerDialogMessages { get; set; }
        //--------------------------------------------------------------------------------------
        public DbSet<CreationMessengerDialogMessageAttachment> CreationMessengerDialogMessageAttachments { get; set; }
        //--------------------------------------------------------------------------------------
    }
}
