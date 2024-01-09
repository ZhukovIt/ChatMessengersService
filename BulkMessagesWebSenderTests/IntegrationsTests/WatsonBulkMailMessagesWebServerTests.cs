using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using BulkMessagesWebServer.Tests.Common;
using WatsonWebserver;
using System.Net.Http;
using BulkMessagesWebServer.DataModel;
using BulkMessagesWebServer.DBObjects;
using Moq;

namespace BulkMessagesWebServer.Tests.EndToEnd
{
    public sealed class WatsonBulkMailMessagesWebServerTests
    {
        [Fact]
        public void Launch_Test_Web_Server()
        {
            MainOptions _Options = MotherObject.CreateMainOptions();
            IMassMessagesRepository _Repository = new EntityFrameworkMassMessagesRepository("TestDB");
            WatsonBulkMessagesWebServer sut = new WatsonBulkMessagesWebServer(_Repository, _Options, "Test");

            try
            {
                sut.Start();
                
                while (true)
                {

                }
            }
            finally
            {
                sut.Stop();
            }
        }
        //----------------------------------------------------------------------------------------------
        [Fact]
        public void Launch_Work_Web_Server()
        {
            MainOptions _Options = MotherObject.CreateMainOptions();
            IMassMessagesRepository _Repository = new EntityFrameworkMassMessagesRepository("WorkDB");
            WatsonBulkMessagesWebServer sut = new WatsonBulkMessagesWebServer(_Repository, _Options, "Test");

            try
            {
                sut.Start();

                while (true)
                {

                }
            }
            finally
            {
                sut.Stop();
            }
        }
        //----------------------------------------------------------------------------------------------
    }
}
