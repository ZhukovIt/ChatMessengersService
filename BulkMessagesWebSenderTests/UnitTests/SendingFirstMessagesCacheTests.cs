using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatMessengersService.MassMessagesUmnicoSenders;
using Xunit;

namespace BulkMessagesWebServerTests.UnitTests
{
    public class SendingFirstMessagesCacheTests
    {
        [Fact]
        public void Give_Cache_From_Key_If_Cache_Is_Exist()
        {
            SendingFirstMessagesCache sut = new SendingFirstMessagesCache();
            CachedSendingFirstMessage cache = new CachedSendingFirstMessage();
            sut.Add(1, cache, TimeSpan.FromDays(1));

            CachedSendingFirstMessage result = sut.Get(1);

            Assert.True(ReferenceEquals(cache, result));
        }
        //----------------------------------------------------------------
        [Fact]
        public void Not_Give_Cache_From_Key_If_Cache_Is_Not_Exist()
        {
            SendingFirstMessagesCache sut = new SendingFirstMessagesCache();

            CachedSendingFirstMessage result = sut.Get(1);

            Assert.Null(result);
        }
        //----------------------------------------------------------------
        [Fact]
        public void Clear_Old_Cache_If_They_Is_Outdated()
        {
            SendingFirstMessagesCache sut = new SendingFirstMessagesCache();
            CachedSendingFirstMessage oldCache = new CachedSendingFirstMessage();
            sut.Add(1, oldCache, TimeSpan.FromDays(-1));
            CachedSendingFirstMessage cache = new CachedSendingFirstMessage();
            sut.Add(2, cache, TimeSpan.FromDays(1));

            CachedSendingFirstMessage result = sut.Get(2);

            Assert.True(ReferenceEquals(cache, result));
        }
        //----------------------------------------------------------------
    }
}
