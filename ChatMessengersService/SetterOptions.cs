using System.IO;

namespace ChatMessengersService
{
    public sealed class SetterOptions
    {
        private string m_GlobalOptionsPath;
        private string m_LocalOptionsPath;
        //--------------------------------------------------------------
        public SetterOptions(string GlobalOptionsPath, string LocalOptionsPath)
        {
            m_GlobalOptionsPath = GlobalOptionsPath;
            m_LocalOptionsPath = LocalOptionsPath;
        }
        //--------------------------------------------------------------
        public void SetOptions(ref string GlobalOptionsData, ref string LocalOptionsData)
        {
            SetGlobalOptions(ref GlobalOptionsData);
            SetLocalOptions(ref LocalOptionsData);
        }
        //--------------------------------------------------------------
        #region Вспомогательные закрытые методы и атрибуты
        //--------------------------------------------------------------
        private void SetGlobalOptions(ref string GlobalOptionsData)
        {
            using (StreamReader reader = new StreamReader(m_GlobalOptionsPath))
            {
                GlobalOptionsData = reader.ReadToEnd();
            }
        }
        //--------------------------------------------------------------
        private void SetLocalOptions(ref string LocalOptionsData)
        {
            using (StreamReader reader = new StreamReader(m_LocalOptionsPath))
            {
                LocalOptionsData = reader.ReadToEnd();
            }
        }
        //--------------------------------------------------------------
        #endregion
        //--------------------------------------------------------------
    }
}
