using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMessagesWebServer
{
    public sealed class MainOptions
    {
        private string m_Domain;
        private int m_Port;
        private string m_GUID;
        //----------------------------------------------------
        public string Domain
        {
            get
            {
                return m_Domain;
            }

            set
            {
                m_Domain = value;
            }
        }
        //----------------------------------------------------
        public int Port
        {
            get
            {
                return m_Port;
            }
        }
        //----------------------------------------------------
        public string GUID
        {
            get
            {
                return m_GUID;
            }
        }
        //----------------------------------------------------
        public MainOptions(string _Domain, int _Port)
        {
            m_Domain = _Domain;
            m_Port = _Port;
        }
        //----------------------------------------------------
        public MainOptions SetGUID(string _GUID)
        {
            m_GUID = _GUID;
            return this;
        }
        //----------------------------------------------------
        public void IncrementPort()
        {
            m_Port++;

            if (m_Port > 65535)
            {
                m_Port = 61000;
            }
        }
        //----------------------------------------------------
    }
}
