using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiMed.ChatMessengers.Umnico;
using SiMed.Clinic.DataModel;
using BulkMessagesWebServer;
using System.Configuration;
using BulkMessagesWebServer.DataModel;
using BulkMessagesWebServer.DBObjects;
using ChatMessengersService.MassMessagesUmnicoSenders;

namespace ChatMessengersService
{
    public static class MotherObject
    {
        public static IMessengerCommon CreateChatAggregator(string _ChatAggregatorType, string _GlobalOptionsPath, Action<string> _LogDelegate,
            string _LocalOptionsPath = null)
        {
            IMessengerCommon _ChatAggregator;


            switch (_ChatAggregatorType)
            {
                case ChatAggregators.Umnico:
                    _ChatAggregator = new CMUmnico(_LogDelegate);
                    break;
                default:
                    throw new NotImplementedException($"Чат-аггрегатор типа {_ChatAggregatorType} не поддерживается!");
            }

            SetterOptions _SetterOptions = new SetterOptions(_GlobalOptionsPath, _LocalOptionsPath);

            string _GlobalOptionsData = null;
            string _LocalOptionsData = null;

            _SetterOptions.SetOptions(ref _GlobalOptionsData, ref _LocalOptionsData);

            _ChatAggregator.SetOptions(_GlobalOptionsData, _LocalOptionsData);

            _ChatAggregator.Init(ClinicChatAggregatorApplicationType.Service);

            return _ChatAggregator;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public static IMassMessagesRepository CreateMassMessagesRepository(string _MassMessagesRepositoryType, string _NameConnectionString)
        {
            string _ConnectionString = ConfigurationManager.ConnectionStrings[_NameConnectionString].ConnectionString;

            switch (_MassMessagesRepositoryType)
            {
                case MassMessagesRepositories.EntityFramework:
                    return new EntityFrameworkMassMessagesRepository(_NameConnectionString);
                case MassMessagesRepositories.SQL:
                    return new SqlMassMessagesRepository(_ConnectionString);
                default:
                    throw new NotImplementedException($"Репозиторий массовой рассылки типа {_MassMessagesRepositoryType} не поддерживается!");
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public static WatsonBulkMessagesWebServer CreateWatsonBulkMessagesWebServer(IMassMessagesRepository _Repository)
        {
            string _FullURL = ConfigurationManager.AppSettings["BulkMessagesFullURL"];

            if (_FullURL.All(c => c != ':'))
            {
                throw new FormatException("В файле конфигурации значение для AppSettings -> BulkMessagesFullURL не содержит ни одного знака \":\"!");
            }

            string[] _FullURLCollection = _FullURL.Split(':');

            string _URL = _FullURLCollection[0];
            int _Port = int.Parse(_FullURLCollection[1]);

            Guid _AuthGuid;

            if (!Guid.TryParse(ConfigurationManager.AppSettings["BulkMessagesAuthGuid"], out _AuthGuid))
            {
                throw new FormatException("В файле конфигурации значение для AppSettings -> BulkMessagesAuthGuid не соответствует значению Guid!\r\n" +
                    "Значение Guid должно соответствовать формату: xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx, где x - значение в диапозоне 0...9abcdef");
            }

            MainOptions _Options = new MainOptions(_URL, _Port).SetGUID(_AuthGuid.ToString());

            string _BranchName = ConfigurationManager.AppSettings["BranchName"];

            WatsonBulkMessagesWebServer _WebServer = new WatsonBulkMessagesWebServer(_Repository, _Options, _BranchName);

            return _WebServer;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public static MassMessagesUmnicoSenderService CreateMassMessagesUmnicoSenderService(IMessengerCommon _ChatMessenger, 
            IMassMessagesRepository _Repository)
        {
            string _BranchName = ConfigurationManager.AppSettings["BranchName"];

            return new MassMessagesUmnicoSenderService(_ChatMessenger, _Repository, _BranchName);
        }
        //------------------------------------------------------------------------------------------------------------------------
    }
    //------------------------------------------------------------------------------------------------------------------------
}
