using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulkMessagesWebServer.DBObjects;
using BulkMessagesWebServer.DBObjects.MassMessages;
using Xunit;

namespace BulkMessagesWebServer.Tests.Integrations
{
    public sealed class MassMessagesContextTests
    {
        [Fact]
        public void Check_Waiting_Objects_From_Sender_Status_Types_Entity()
        {
            using (MassMessagesContext db = new MassMessagesContext("TestDB"))
            {
                // Подготовка (Arrange)
                int _CountSenderStatusTypes = db.SenderStatusTypes.AsQueryable().Count();

                SenderStatusType _FirstSenderStatusType = db.SenderStatusTypes.AsQueryable().FirstOrDefault(row => row.Id == 1);
                SenderStatusType _SecondSenderStatusType = db.SenderStatusTypes.AsQueryable().FirstOrDefault(row => row.Id == 2);
                SenderStatusType _ThirdSenderStatusType = db.SenderStatusTypes.AsQueryable().FirstOrDefault(row => row.Id == 3);

                string _FirstSenderStatusTypeName = _FirstSenderStatusType.Name;
                string _SecondSenderStatusTypeName = _SecondSenderStatusType.Name;
                string _ThirdSenderStatusTypeName = _ThirdSenderStatusType.Name;

                // Проверка (Assert)
                Assert.Equal(3, _CountSenderStatusTypes);
                Assert.Equal("Ожидает отправки", _FirstSenderStatusTypeName);
                Assert.Equal("Отправлено", _SecondSenderStatusTypeName);
                Assert.Equal("Не отправлено", _ThirdSenderStatusTypeName);
            }
        }
    }
}
