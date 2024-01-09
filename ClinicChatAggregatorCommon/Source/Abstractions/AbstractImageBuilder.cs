using System.Drawing;

namespace SiMed.Clinic.DataModel
{
    public abstract class AbstractImageBuilder
    {
        protected Image m_Result;

        public Image Result => m_Result;

        protected AbstractImageBuilder(Image Result)
        {
            m_Result = Result;
        }

        public abstract void BuildRoundImage();

        public abstract void BuildRoundFrame();

        public abstract void BuildShadowInsideImage();

    }
}
