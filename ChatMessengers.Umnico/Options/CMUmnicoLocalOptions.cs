using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SiMed.ChatMessengers.Umnico
{
    public class CMUmnicoLocalOptions : BaseOptions
    {
        public Manager m_Manager;
        public override string Pack()
        {
            System.IO.MemoryStream memStream = new System.IO.MemoryStream();
            XmlSerializer serializer = new XmlSerializer(typeof(CMUmnicoLocalOptions));

            serializer.Serialize(memStream, this);
            memStream.Position = 0;
            string XmlStr = Encoding.UTF8.GetString(memStream.GetBuffer());
            return XmlStr;
        }

        public override BaseOptions Unpack(string _Source)
        {
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(CMUmnicoLocalOptions));
                System.IO.MemoryStream memStream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(_Source));
                CMUmnicoLocalOptions options = (CMUmnicoLocalOptions)deserializer.Deserialize(memStream);
                return options;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
