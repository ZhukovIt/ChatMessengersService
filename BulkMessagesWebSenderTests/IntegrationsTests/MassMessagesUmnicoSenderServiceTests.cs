using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using SiMed.Clinic.DataModel;
using ChatMessengersService.MassMessagesUmnicoSenders;
using BulkMessagesWebServer.DBObjects;
using BulkMessagesWebServer;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using System.Configuration;

namespace ChatMessengersService.Tests.EndToEnd
{
    public sealed class MassMessagesUmnicoSenderServiceTests
    {
        [Fact]
        public void Launch_Sender_Service()
        {
            // Подготовка (Arrange)
            Mock<IMessengerCommon> mock = new Mock<IMessengerCommon>();
            mock.Setup(x => x.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<MessengersType>())).Returns("");
            mock.Setup(x => x.FirstSendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MessengersType>())).Returns("");

            IMassMessagesRepository repository = new EntityFrameworkMassMessagesRepository("WorkDB");

            MassMessagesUmnicoSenderService sut = new MassMessagesUmnicoSenderService(mock.Object, repository, "Test");

            // Действие (Act)
            sut.Start();
        }
        //----------------------------------------------------------------------------------------------------------
        [Fact]
        public void Launch_Server_And_Sender_Service_With_Entity_Framework()
        {
            // Подготовка (Arrange)
            MainOptions _Options = BulkMessagesWebServer.Tests.Common.MotherObject.CreateMainOptions();
            IMassMessagesRepository _Repository = new EntityFrameworkMassMessagesRepository("WorkDB");
            WatsonBulkMessagesWebServer server = new WatsonBulkMessagesWebServer(_Repository, _Options, "Test");

            IMessengerCommon _ChatMessenger = MotherObject.CreateChatAggregator(
                "Umnico",
                Path.Combine(Application.StartupPath, "Globaloptions.txt"),
                (string _Message) => { },
                Path.Combine(Application.StartupPath, "Localoptions.txt"));

            MassMessagesUmnicoSenderService service = new MassMessagesUmnicoSenderService(_ChatMessenger, _Repository, "Test");

            Thread loggerThread = new Thread(() => service.Start());
            
            // Действие (Act)
            try
            {
                loggerThread.Start();
                server.Start();

                while (true)
                {

                }
            }
            finally
            {
                service.Stop();
                server.Stop();
            }
        }
        //----------------------------------------------------------------------------------------------------------
        [Fact]
        public void Launch_Server_And_Sender_Service_With_Clean_SQL()
        {
            // Подготовка (Arrange)
            MainOptions _Options = BulkMessagesWebServer.Tests.Common.MotherObject.CreateMainOptions();
            string _ConnectionString = ConfigurationManager.ConnectionStrings["WorkDB"].ConnectionString;
            IMassMessagesRepository _Repository = new SqlMassMessagesRepository(_ConnectionString);
            WatsonBulkMessagesWebServer server = new WatsonBulkMessagesWebServer(_Repository, _Options, "Test");

            IMessengerCommon _ChatMessenger = MotherObject.CreateChatAggregator(
                "Umnico",
                Path.Combine(Application.StartupPath, "Globaloptions.txt"),
                (string _Message) => { },
                Path.Combine(Application.StartupPath, "Localoptions.txt"));

            MassMessagesUmnicoSenderService service = new MassMessagesUmnicoSenderService(_ChatMessenger, _Repository, "Test");

            Thread loggerThread = new Thread(() => service.Start());

            // Действие (Act)
            try
            {
                loggerThread.Start();
                server.Start();

                while (true)
                {

                }
            }
            finally
            {
                service.Stop();
                server.Stop();
            }
        }
        //----------------------------------------------------------------------------------------------------------
    }
}
