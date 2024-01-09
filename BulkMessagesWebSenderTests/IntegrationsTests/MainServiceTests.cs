using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatMessengersService;
using Xunit;

namespace BulkMessagesWebServerTests.IntegrationsTests
{
    public sealed class MainServiceTests
    {
        [Fact]
        public void Launch()
        {
            MainService sut = new MainService("TestDB");

            Assert.Null(sut.LastException);      
        }
    }
}
