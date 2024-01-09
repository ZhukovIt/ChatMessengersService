using System;
using System.Collections.Generic;

namespace SiMed.Clinic.DataModel
{
    public interface IMessengerCommon
    {
        string SendMessage(string idDialog, string text, string customId, string pathMedia = null, MessengersType? typeMessenger = null);

        string FirstSendMessage(string idSourceMessages, string loginOrPhone, string text, string customId, string pathMedia = null, MessengersType? typeMessenger = null);

        IEnumerable<Message> CheckNewMessages();

        bool CheckConnection();

        bool SetOptions(string _SystemOptions, string _LocalOptions);

        bool ShowLocalOptions(ref string _LocalOptions);

        bool ShowSystemOptions(ref string _SystemOptions);

        bool Init(ClinicChatAggregatorApplicationType source = ClinicChatAggregatorApplicationType.Program);

        IEnumerable<SourceMessage> GetSourceMessages();

        IEnumerable<Manager> GetManagers();

        Customer GetCustomer(string idCustomer);

        void SaveExceptionToLog(Exception ex, string source);

        bool CheckPhoneNumberInSourceType(string _SourceTypeUID, string _Phone, out string _Result);
    }
}
