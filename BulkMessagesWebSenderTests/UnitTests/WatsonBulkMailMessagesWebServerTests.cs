using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using WatsonWebserver;
using BulkMessagesWebServer.Tests.Common;
using System.Net.Http;
using BulkMessagesWebServer.DataModel;
using BulkMessagesWebServer.DBObjects;
using Moq;
using Newtonsoft.Json;

namespace BulkMessagesWebServer.Tests.Unit
{
    public sealed class WatsonBulkMailMessagesWebServerTests
    {
        [Fact]
        public void Server_Not_Launch_If_Main_Options_Is_Not_Initialized()
        {
            Mock<IMassMessagesRepository> mock = new Mock<IMassMessagesRepository>();
            WatsonBulkMessagesWebServer sut = new WatsonBulkMessagesWebServer(mock.Object, null, "Test");

            try
            {
                sut.Start();
                Assert.Null(sut.Server);
                Assert.NotNull(sut.LastException);
            }
            finally
            {
                sut.Stop();
            }
        }
        //-----------------------------------------------------------------------------------
        [Fact]
        public void Can_Redirect_To_Home_Page()
        {
            // Подготовка (Arrange)
            HttpClient _Client = null;
            HttpResponseMessage _Response = null;
            MainOptions _Options = MotherObject.CreateMainOptions();
            _Options.Domain = "localhost";
            Mock<IMassMessagesRepository> mock = new Mock<IMassMessagesRepository>();
            WatsonBulkMessagesWebServer sut = new WatsonBulkMessagesWebServer(mock.Object, _Options, "Test");

            try
            {
                // Действие (Act)
                sut.Start();
                _Client = new HttpClient();
                _Response = _Client.GetAsync($"http://{_Options.Domain}:{_Options.Port}/").Result;

                // Проверка (Assert)
                Assert.Equal(200, (int)_Response.StatusCode);
            }
            finally
            {
                _Response?.Dispose();
                _Client?.Dispose();
                sut?.Stop();
            }
        }
        //----------------------------------------------------------------------------------------------
    }
}
