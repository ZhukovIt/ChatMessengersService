using System;
using System.Text;
using System.Xml.Serialization;

namespace SiMed.ChatMessengers.Umnico
{
    public class CMUmnicoGlobalOptions : BaseOptions
    {
        public string LOGIN;
        public string PASSWORD;
        public string GUID;
        public string MAIN_URL;
        public CMUmnicoGlobalOptions()
        {
            MAIN_URL = "";// https://api.umnico.com/v1.3
            LOGIN = "";// pdomashnev@simplex48.ru
            PASSWORD = "";// Q!23werty
            GUID = "";
        }
        public override string Pack()
        {
            System.IO.MemoryStream memStream = new System.IO.MemoryStream();
            XmlSerializer serializer = new XmlSerializer(typeof(CMUmnicoGlobalOptions));

            serializer.Serialize(memStream, this);
            memStream.Position = 0;
            string XmlStr = Encoding.UTF8.GetString(memStream.GetBuffer());
            return XmlStr;
        }

        public override BaseOptions Unpack(string _Source)
        {
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(CMUmnicoGlobalOptions));
                System.IO.MemoryStream memStream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(_Source));
                CMUmnicoGlobalOptions options = (CMUmnicoGlobalOptions)deserializer.Deserialize(memStream);
                return options;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
