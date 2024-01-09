using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BulkMessagesWebServer.DBObjects;
using BulkMessagesWebServer.DataModel.FileSystem.Images;
using System.IO;

namespace BulkMessagesWebServer.DataModel
{
    public sealed class MassMessagesPutter
    {
        private readonly IMassMessagesRepository m_Repository;
        private readonly ImagesService m_ImagesService;
        private readonly MassMessagesLogsWorker m_MassMessagesLogsWorker;
        private string m_BranchName;
        //-------------------------------------------------------------------------------------
        public string BranchName
        {
            get { return m_BranchName; }
            set { m_BranchName = value; }
        }
        //-------------------------------------------------------------------------------------
        public MassMessagesPutter(IMassMessagesRepository _Repository, MassMessagesLogsWorker _MassMessagesLogsWorker)
        {
            m_MassMessagesLogsWorker = _MassMessagesLogsWorker;

            if (_Repository == null)
            {
                throw new ArgumentNullException("Объект типа IMassMessagesRepository оказался равен null!");
            }

            m_Repository = _Repository;

            string _DirectoryName = Path.Combine(Application.StartupPath, "MassMessagesImages");

            m_ImagesService = new ImagesService(_DirectoryName);
        }
        //-------------------------------------------------------------------------------------
        public SendMessagesResponse PutMessagesInDataBase(SendMessagesRequest _Request)
        {
            SendMessagesResponse _Response = new SendMessagesResponse();

            _Response.MesUmnGuids = new Dictionary<string, string>();

            string _ImageFilePath = m_ImagesService.AddImage(_Request.Image);

            SendMessagesTypes _SendMessagesType = _Request.SendMessagesType;

            if (_SendMessagesType == SendMessagesTypes.OneMessageToOneRecipient)
            {
                PersonInfo _PersonInfo = _Request.PersonsInfo.First();

                int _PersonId = _PersonInfo.PersonId;

                string _Text = _PersonInfo.Text;

                string _InputGuid = _Request.Guid;

                string _OutGuid = Guid.NewGuid().ToString();

                m_Repository.AddNewMesUmnicoSender(_PersonId, _Text, _OutGuid, _ImageFilePath, BranchName);

                _Response.MesUmnGuids.Add(_InputGuid, _OutGuid);

                m_MassMessagesLogsWorker.SaveMessage(
                    new LogData()
                    {
                        Source = GetType().Name,
                        Message = $"Сообщение массовой рассылки с PER_ID = {_PersonId} подготовлено к отправке"
                    }.SetDeveloperComment($"Тип отправки = {_SendMessagesType}"));
            }
            else if (_SendMessagesType == SendMessagesTypes.OneMessageToManyRecipients)
            {
                string _InputGuid = _Request.Guid;

                string _OutGuid = Guid.NewGuid().ToString();

                List<int> _PersonIds = new List<int>();

                foreach (PersonInfo _PersonInfo in _Request.PersonsInfo)
                {
                    int _PersonId = _PersonInfo.PersonId;

                    _PersonIds.Add(_PersonId);

                    string _Text = _PersonInfo.Text;

                    m_Repository.AddNewMesUmnicoSender(_PersonId, _Text, _OutGuid, _ImageFilePath, BranchName);

                    _Response.MesUmnGuids[_InputGuid] = _OutGuid;
                }

                m_MassMessagesLogsWorker.SaveMessage(
                    new LogData()
                    {
                        Source = GetType().Name,
                        Message = $"Сообщения массовой рассылки с PER_ID = ({string.Join(", ", _PersonIds)}) подготовлены к отправке"
                    }.SetDeveloperComment($"Тип отправки = {_SendMessagesType}"));
            }
            else if (_SendMessagesType == SendMessagesTypes.ManyPersonalMessagesToManyRecipients)
            {
                List<int> _PersonIds = new List<int>();

                foreach (PersonInfo _PersonInfo in _Request.PersonsInfo)
                {
                    int _PersonId = _PersonInfo.PersonId;

                    _PersonIds.Add(_PersonId);

                    string _Text = _PersonInfo.Text;

                    string _InputGuid = _PersonInfo.Guid;

                    string _OutGuid = Guid.NewGuid().ToString();

                    m_Repository.AddNewMesUmnicoSender(_PersonId, _Text, _OutGuid, _ImageFilePath, BranchName);

                    _Response.MesUmnGuids.Add(_InputGuid, _OutGuid);
                }

                m_MassMessagesLogsWorker.SaveMessage(
                    new LogData()
                    {
                        Source = GetType().Name,
                        Message = $"Сообщения массовой рассылки с PER_ID = ({string.Join(", ", _PersonIds)}) подготовлены к отправке"
                    }.SetDeveloperComment($"Тип отправки = {_SendMessagesType}"));
            }
            else
            {
                throw new NotImplementedException($"Тип SendMessagesTypes не поддерживает значение {_SendMessagesType}!");
            }

            _Response.Success = true;

            return _Response;
        }
        //-------------------------------------------------------------------------------------
    }
}
