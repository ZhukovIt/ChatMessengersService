using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BulkMessagesWebServer.DBObjects;
using BulkMessagesWebServer.DBObjects.CreateMessengerDialog;
using BulkMessagesWebServer.DBObjects.MessengerDialog;
using Xunit;

namespace BulkMessagesWebServer.Tests.Integrations
{
    public sealed class EntityFrameworkMassMessagesRepositoryTests : IntegrationTest
    {
        public EntityFrameworkMassMessagesRepositoryTests() : base("TestDB") { }
        //--------------------------------------------------------------------------------------
        [Fact]
        public void Can_Add_New_Record_In_MES_UMNICO_SENDER_Entity()
        {
            IMassMessagesRepository sut = new EntityFrameworkMassMessagesRepository(m_ConnectionString);
            string _Guid = Guid.NewGuid().ToString();

            sut.AddNewMesUmnicoSender(1, "Test", _Guid, null, "Test");

            Assert.True(m_DataRepository.GetMesUmnSendIdByGuid(_Guid) > 0);
        }
        //--------------------------------------------------------------------------------------
        [Fact]
        public void Can_Delete_Record_In_MES_UMNICO_SENDER_Entity()
        {
            IMassMessagesRepository sut = new EntityFrameworkMassMessagesRepository(m_ConnectionString);
            string _Guid = Guid.NewGuid().ToString();
            sut.AddNewMesUmnicoSender(1, "Test", _Guid, null, "Test");

            sut.DeleteMesUmnicoSender(1);

            Assert.True(m_DataRepository.GetMesUmnSendIdByGuid(_Guid) <= 0);
        }
        //--------------------------------------------------------------------------------------
        [Fact]
        public void Sender_Exist_Condition_Is_Passed_Because_Entity_Has_Senders()
        {
            IMassMessagesRepository sut = new EntityFrameworkMassMessagesRepository(m_ConnectionString);
            sut.AddNewMesUmnicoSender(1, "Test", Guid.NewGuid().ToString(), null, "Test");

            bool result = sut.HasSendersWhichNeedSendMessages("Test");

            Assert.True(result);
        }
        //--------------------------------------------------------------------------------------
        [Fact]
        public void Sender_Exist_Condition_Is_Failure_Because_Entity_Is_Empty()
        {
            IMassMessagesRepository sut = new EntityFrameworkMassMessagesRepository(m_ConnectionString);

            bool result = sut.HasSendersWhichNeedSendMessages("Test");

            Assert.False(result);
        }
        //--------------------------------------------------------------------------------------
        [Fact]
        public void Exist_Only_One_Sender_Which_Send_Message_Because_Entity_Has_One_Sender()
        {
            IMassMessagesRepository sut = new EntityFrameworkMassMessagesRepository(m_ConnectionString);
            sut.AddNewMesUmnicoSender(1, "Test", Guid.NewGuid().ToString(), null, "Test");

            var result = sut.GetSendersWhichNeedSendMessages("Test");

            Assert.Equal(1, result.Count());
            Assert.Equal(1, result.First().Id);
        }
        //--------------------------------------------------------------------------------------
        [Fact]
        public void Sender_Which_Send_Message_Is_Not_Exist_Because_Entity_Is_Empty()
        {
            IMassMessagesRepository sut = new EntityFrameworkMassMessagesRepository(m_ConnectionString);

            var result = sut.GetSendersWhichNeedSendMessages("Test");

            Assert.Equal(0, result.Count());
        }
        //--------------------------------------------------------------------------------------
        [Fact]
        public void Create_Source_Type_Data_From_Telegram()
        {
            IMassMessagesRepository sut = new EntityFrameworkMassMessagesRepository(m_ConnectionString);

            var result = sut.CreateSourceTypeData("Telegram");

            Assert.Equal(3, result.Item1);
            Assert.Equal("49113", result.Item2);
        }
        //--------------------------------------------------------------------------------------
        [Fact]
        public void Can_Get_Phone_Number_If_Phone_Number_Is_Exist_In_ADDRESS_Entity()
        {
            IMassMessagesRepository sut = new EntityFrameworkMassMessagesRepository(m_ConnectionString);
            string _PhoneNumber = "9042194323";
            m_DataRepository.UpdatePhoneNumberByPersonId(1, _PhoneNumber);

            string result = sut.GetPhoneNumberFromPersonId(1);

            Assert.Equal(_PhoneNumber, result);
        }
        //--------------------------------------------------------------------------------------
        [Fact]
        public void Can_Not_Get_Phone_Number_If_Phone_Number_Is_Not_Exist_In_ADDRESS_Entity()
        {
            IMassMessagesRepository sut = new EntityFrameworkMassMessagesRepository(m_ConnectionString);
            m_DataRepository.UpdatePhoneNumberByPersonId(1, null);

            string result = sut.GetPhoneNumberFromPersonId(1);

            Assert.Null(result);
        }
        //--------------------------------------------------------------------------------------
        [Fact]
        public void Can_Get_Correct_Telegram_Messenger_Type_Id()
        {
            IMassMessagesRepository sut = new EntityFrameworkMassMessagesRepository(m_ConnectionString);

            int result = sut.GetMessengerTypeIdBySourceTypeId(3);

            Assert.Equal(4, result);
        }
        //--------------------------------------------------------------------------------------
        [Fact]
        public void Can_Get_Correct_Person_Messenger_Type_Id()
        {
            IMassMessagesRepository sut = new EntityFrameworkMassMessagesRepository(m_ConnectionString);

            int result = sut.GetPersonMessengerTypeIdByPersonIdAndMessengerTypeId(1, 4);

            Assert.Equal(1, result);
        }
        //--------------------------------------------------------------------------------------
        [Fact]
        public void Can_Check_Absent_Person_Messenger_Type_Id_In_Messenger_Dialog()
        {
            IMassMessagesRepository sut = new EntityFrameworkMassMessagesRepository(m_ConnectionString);
            m_DataRepository.UpdatePersonMessengerTypeIdByMessengerDialogId(1, null);

            bool result = sut.PersonMessengerTypeIdInMessengerDialogIsEmpty(1);

            Assert.True(result);
        }
        //--------------------------------------------------------------------------------------
        [Fact]
        public void Can_Check_Availability_Person_Messenger_Type_Id_In_Messenger_Dialog()
        {
            IMassMessagesRepository sut = new EntityFrameworkMassMessagesRepository(m_ConnectionString);
            m_DataRepository.UpdatePersonMessengerTypeIdByMessengerDialogId(1, 1);

            bool result = sut.PersonMessengerTypeIdInMessengerDialogIsEmpty(1);

            Assert.False(result);
        }
        //--------------------------------------------------------------------------------------
        [Fact]
        public void Can_Add_New_Record_In_PERSON_MESSENGER_TYPES_Entity()
        {
            IMassMessagesRepository sut = new EntityFrameworkMassMessagesRepository(m_ConnectionString);

            sut.AddPersonMessengerType(1, 1);

            Assert.True(m_DataRepository.GetPersonMessengerTypeIdByPersonIdAndMessengerTypeId(1, 1) > 0);
        }
        //--------------------------------------------------------------------------------------
        [Fact]
        public void Can_Add_New_Record_In_MESSENGER_DIALOG_Entity()
        {
            IMassMessagesRepository sut = new EntityFrameworkMassMessagesRepository(m_ConnectionString);
            DateTime _CreationDateTime = new DateTime(2023, 9, 22, 16, 0, 0);

            sut.AddMessengerDialog(1, "9042194323", _CreationDateTime);

            Assert.True(m_DataRepository.GetMessengerDialogIdBySourceTypeIdAndPhoneNumberAndCreationDateTime(
                1, "9042194323", _CreationDateTime) > 0);
        }
        //--------------------------------------------------------------------------------------
        [Fact]
        public void Can_Update_Record_PER_MES_TYPE_ID_In_MESSENGER_DIALOG_Entity()
        {
            IMassMessagesRepository sut = new EntityFrameworkMassMessagesRepository(m_ConnectionString);

            sut.SetPersonMessengerTypeIdByMessengerDialogId(1, 1);

            Assert.Equal(1, m_DataRepository.GetPersonMessengerTypeIdByMessengerDialogId(1));
        }
        //--------------------------------------------------------------------------------------
        [Fact]
        public void Can_Add_New_Record_In_MESSENGER_DIALOG_MESSAGE_Entity()
        {
            IMassMessagesRepository sut = new EntityFrameworkMassMessagesRepository(m_ConnectionString);
            string _Guid = Guid.NewGuid().ToString();

            sut.AddMessengerDialogMessage(1, "Тест", _Guid, new DateTime(2023, 9, 22, 16, 0, 0));

            Assert.True(m_DataRepository.GetMessengerDialogMessageIdByGuid(_Guid) > 0);
        }
        //--------------------------------------------------------------------------------------
        [Fact]
        public void Can_Add_New_Record_In_MESSENGER_DIALOG_MESSAGE_ATTACHMENT_Entity()
        {
            IMassMessagesRepository sut = new EntityFrameworkMassMessagesRepository(m_ConnectionString);

            sut.AddMessengerDialogMessageAttachment(1, "Тестовый_файл.txt", "");

            Assert.True(m_DataRepository.GetMessengerDialogMessageAttachmentIdByAllData(1, "Тестовый_файл.txt", "") > 0);
        }
        //--------------------------------------------------------------------------------------
        [Fact]
        public void Get_Correct_Message_Log_Id_With_One_Message_To_One_Recepient_Send_Type()
        {
            IMassMessagesRepository sut = new EntityFrameworkMassMessagesRepository(m_ConnectionString);
            string _Guid = m_DataRepository.CreateOneMessageToOneRecepientMessageLog(1, "+79042194323");

            int? result = sut.GetMessageLogIdByGuidAndPersonId(_Guid, 1);

            Assert.Equal(m_DataRepository.GetMessageLogIdByGuid(_Guid), result);
        }
        //--------------------------------------------------------------------------------------
        [Fact]
        public void Get_Correct_Message_Log_Id_With_One_Message_To_Many_Recepients_Send_Type()
        {
            IMassMessagesRepository sut = new EntityFrameworkMassMessagesRepository(m_ConnectionString);
            string _Guid = m_DataRepository.CreateOneMessageToManyRecepientsMessageLog(new Dictionary<int, string>()
            {
                { 1, "+79042194323" },
                { 2, "+79513085221" }
            });

            int? _FirstResult = sut.GetMessageLogIdByGuidAndPersonId(_Guid, 1);
            int? _SecondResult = sut.GetMessageLogIdByGuidAndPersonId(_Guid, 2);

            int _MessageBatchId = m_DataRepository.GetMessageBatchIdByGuid(_Guid);
            int? _FirstMessageLogId = m_DataRepository.GetMessageLogIdByMessageBatchIdAndPersonId(_MessageBatchId, 1);
            Assert.Equal(_FirstMessageLogId, _FirstResult);
            int? _SecondMessageLogId = m_DataRepository.GetMessageLogIdByMessageBatchIdAndPersonId(_MessageBatchId, 2);
            Assert.Equal(_SecondMessageLogId, _SecondResult);
        }
        //--------------------------------------------------------------------------------------
        [Fact]
        public void Get_Correct_Message_Log_Id_With_Many_Personal_Messages_To_Many_Recepients_Send_Type()
        {
            IMassMessagesRepository sut = new EntityFrameworkMassMessagesRepository(m_ConnectionString);
            Dictionary<int, string> _PersonGuids = m_DataRepository
                .CreateManyPersonalMessagesToManyRecepientsMessageLog(
                    new Dictionary<int, string>()
                    {
                        { 1, "+79042194323" },
                        { 2, "+79513085221" }
                    });
            string _FirstGuid = _PersonGuids[1];
            string _SecondGuid = _PersonGuids[2];

            int? _FirstResult = sut.GetMessageLogIdByGuidAndPersonId(_FirstGuid, 1);
            int? _SecondResult = sut.GetMessageLogIdByGuidAndPersonId(_SecondGuid, 2);

            Assert.Equal(m_DataRepository.GetMessageLogIdByGuid(_FirstGuid), _FirstResult);
            Assert.Equal(m_DataRepository.GetMessageLogIdByGuid(_SecondGuid), _SecondResult);
        }
        //--------------------------------------------------------------------------------------
        [Fact]
        public void Can_Update_Send_State_Id()
        {
            IMassMessagesRepository sut = new EntityFrameworkMassMessagesRepository(m_ConnectionString);
            string _Guid = m_DataRepository.CreateOneMessageToOneRecepientMessageLog(1, "+79042194323");
            int _MessageLogId = (int)m_DataRepository.GetMessageLogIdByGuid(_Guid);

            sut.UpdateMessageLogSendStateIdById(_MessageLogId, 2);

            Assert.Equal(2, m_DataRepository.GetMessageLogSendStateIdByMessageLogId(_MessageLogId));
        }
        //--------------------------------------------------------------------------------------
        [Fact]
        public void Can_Set_Error_Message_And_Error_Send_State_Id()
        {
            IMassMessagesRepository sut = new EntityFrameworkMassMessagesRepository(m_ConnectionString);
            string _Guid = m_DataRepository.CreateOneMessageToOneRecepientMessageLog(1, "+79042194323");
            int _MessageLogId = (int)m_DataRepository.GetMessageLogIdByGuid(_Guid);

            sut.SetErrorInMessageLogById(_MessageLogId, "Ошибка!");

            Assert.Equal(3, m_DataRepository.GetMessageLogSendStateIdByMessageLogId(_MessageLogId));
            Assert.Equal("Ошибка!", m_DataRepository.GetMessageLogSendErrorByMessageLogId(_MessageLogId));
        }
        //--------------------------------------------------------------------------------------
        [Fact]
        public void Can_Get_All_Person_Messenger_Types()
        {
            IMassMessagesRepository sut = new EntityFrameworkMassMessagesRepository(m_ConnectionString);
            sut.AddPersonMessengerType(1, 2);
            sut.AddPersonMessengerType(1, 3);
            sut.AddMessengerDialog(1, "9042194323", DateTime.UtcNow, sut.GetPersonMessengerTypeIdByPersonIdAndMessengerTypeId(1, 2));
            sut.AddMessengerDialog(2, "9042194323", DateTime.UtcNow, sut.GetPersonMessengerTypeIdByPersonIdAndMessengerTypeId(1, 3));
            sut.AddMessengerDialog(3, "9042194323", DateTime.UtcNow, sut.GetPersonMessengerTypeIdByPersonIdAndMessengerTypeId(1, 4));

            IEnumerable<MessengerDialogDataModel> personMessengerTypes = sut.GetPersonMessengerTypes(1,
                new string[]
                {
                    "WhatsApp",
                    "Telegram",
                    "Вконтакте"
                });

            Assert.Equal(3, personMessengerTypes.Count());
        }
        //--------------------------------------------------------------------------------------
        [Fact]
        public void Can_Not_Person_Messenger_Types_Because_They_Is_Not_Exists()
        {
            IMassMessagesRepository sut = new EntityFrameworkMassMessagesRepository(m_ConnectionString);
            m_DataRepository.DeleteAllPersonMessengerTypes();

            IEnumerable<MessengerDialogDataModel> personMessengerTypes = sut.GetPersonMessengerTypes(1,
                new string[]
                {
                    "WhatsApp",
                    "Telegram",
                    "Вконтакте"
                });

            Assert.False(personMessengerTypes.Any());
        }
        //--------------------------------------------------------------------------------------
        [Fact]
        public void Can_Not_Person_Messenger_Types_Because_Supported_Messenger_Types_Is_Empty()
        {
            IMassMessagesRepository sut = new EntityFrameworkMassMessagesRepository(m_ConnectionString);
            sut.AddPersonMessengerType(1, 2);
            sut.AddPersonMessengerType(1, 3);
            sut.AddMessengerDialog(1, "9042194323", DateTime.UtcNow, sut.GetPersonMessengerTypeIdByPersonIdAndMessengerTypeId(1, 2));
            sut.AddMessengerDialog(2, "9042194323", DateTime.UtcNow, sut.GetPersonMessengerTypeIdByPersonIdAndMessengerTypeId(1, 3));
            sut.AddMessengerDialog(3, "9042194323", DateTime.UtcNow, sut.GetPersonMessengerTypeIdByPersonIdAndMessengerTypeId(1, 4));

            IEnumerable<MessengerDialogDataModel> personMessengerTypes = sut.GetPersonMessengerTypes(1, new string[0]);

            Assert.False(personMessengerTypes.Any());
        }
        //--------------------------------------------------------------------------------------
        [Fact]
        public void Ignores_Duplicates_When_Collecting_Messenger_Dialogs()
        {
            IMassMessagesRepository sut = new EntityFrameworkMassMessagesRepository(m_ConnectionString);
            sut.AddPersonMessengerType(1, 2);
            int personMessengerTypeId = sut.GetPersonMessengerTypeIdByPersonIdAndMessengerTypeId(1, 2);
            m_DataRepository.CreateMessengerDialog(1, DateTime.Now, "", "9042194323", personMessengerTypeId);
            m_DataRepository.CreateMessengerDialog(1, DateTime.Now, "", "9042194323", personMessengerTypeId);
            m_DataRepository.CreateMessengerDialog(1, DateTime.Now, "", "9042194323", personMessengerTypeId);
            m_DataRepository.CreateMessengerDialog(1, DateTime.Now, "", "9042194323");
            m_DataRepository.CreateMessengerDialog(1, DateTime.Now, "", "9042194323");
            m_DataRepository.CreateMessengerDialog(1, DateTime.Now, "", "9042194323");

            IEnumerable <MessengerDialogDataModel> messengerTypesWhereDialogIsExists = 
                sut.GetMessengerTypesWhereDialogIsExists(1, "9042194323");

            Assert.Equal(1, messengerTypesWhereDialogIsExists.Count());
        }
        //--------------------------------------------------------------------------------------
        [Fact]
        public void Does_Not_Collect_Messenger_Dialogs_Without_External_Id()
        {
            IMassMessagesRepository sut = new EntityFrameworkMassMessagesRepository(m_ConnectionString);
            m_DataRepository.CreateMessengerDialog(1, DateTime.Now, null, "9042194323", 1);
            m_DataRepository.CreateMessengerDialog(1, DateTime.Now, null, "9042194323");

            IEnumerable<MessengerDialogDataModel> messengerTypesWhereDialogIsExists = sut.GetMessengerTypesWhereDialogIsExists(1, "9042194323");

            Assert.False(messengerTypesWhereDialogIsExists.Any());
        }
        //--------------------------------------------------------------------------------------
        [Fact]
        public void Does_Not_Collect_Messenger_Dialogs_That_Are_Comments()
        {
            IMassMessagesRepository sut = new EntityFrameworkMassMessagesRepository(m_ConnectionString);
            m_DataRepository.CreateMessengerDialog(1, DateTime.Now, "", "9042194323", 1, 2);
            m_DataRepository.CreateMessengerDialog(1, DateTime.Now, "", "9042194323", null, 2);

            IEnumerable<MessengerDialogDataModel> messengerTypesWhereDialogIsExists = sut.GetMessengerTypesWhereDialogIsExists(1, "9042194323");

            Assert.False(messengerTypesWhereDialogIsExists.Any());
        }
        //--------------------------------------------------------------------------------------
        [Fact]
        public void Collect_Only_Messenger_Dialogs_Which_Has_Person_Messenger_Type_Id_If_Phone_Number_Is_Absent()
        {
            IMassMessagesRepository sut = new EntityFrameworkMassMessagesRepository(m_ConnectionString);
            m_DataRepository.CreateMessengerDialog(1, DateTime.Now, "", null, 1);
            m_DataRepository.CreateMessengerDialog(1, DateTime.Now, "", null);

            IEnumerable<MessengerDialogDataModel> messengerTypesWhereDialogIsExists = sut.GetMessengerTypesWhereDialogIsExists(1, null);

            Assert.Equal(1, messengerTypesWhereDialogIsExists.Count());
        }
        //--------------------------------------------------------------------------------------
        [Fact]
        public void Collect_Messenger_Dialogs_If_Phone_Number_Contains_In_Login_Or_Phone()
        {
            IMassMessagesRepository sut = new EntityFrameworkMassMessagesRepository(m_ConnectionString);
            m_DataRepository.CreateMessengerDialog(1, DateTime.Now, "", "9042194323");

            IEnumerable<MessengerDialogDataModel> messengerTypesWhereDialogIsExists = sut.GetMessengerTypesWhereDialogIsExists(1, "9042194323");

            Assert.Equal(1, messengerTypesWhereDialogIsExists.Count());
        }
        //--------------------------------------------------------------------------------------
        [Fact]
        public void Not_Allowed_Send_To_WhatsApp_If_Options_Does_Not_Exist()
        {
            IMassMessagesRepository sut = new EntityFrameworkMassMessagesRepository(m_ConnectionString);

            bool result = sut.GetChatAggregatorAllowSendToWhatsAppOptions();

            Assert.False(result);
        }
        //--------------------------------------------------------------------------------------
        [Fact]
        public void Not_Allowed_Send_To_WhatsApp_If_Options_Not_Allows_This_Action()
        {
            IMassMessagesRepository sut = new EntityFrameworkMassMessagesRepository(m_ConnectionString);
            m_DataRepository.AddChatAggregatorAllowSendToWhatsAppParameter(false);

            bool result = sut.GetChatAggregatorAllowSendToWhatsAppOptions();

            Assert.False(result);
        }
        //--------------------------------------------------------------------------------------
        [Fact]
        public void Allowed_Send_To_WhatsApp_If_Options_Allows_This_Action()
        {
            IMassMessagesRepository sut = new EntityFrameworkMassMessagesRepository(m_ConnectionString);
            m_DataRepository.AddChatAggregatorAllowSendToWhatsAppParameter(true);

            bool result = sut.GetChatAggregatorAllowSendToWhatsAppOptions();

            Assert.True(result);
        }
        //--------------------------------------------------------------------------------------
    }
}
