
namespace SiMed.Clinic.DataModel
{
    public sealed class DirectorRoundImage
    {
        private AbstractImageBuilder m_Builder;

        public DirectorRoundImage(AbstractImageBuilder Builder)
        {
            m_Builder = Builder;
        }

        public void Construct(ConstructElementsFlags flags)
        {
            m_Builder.BuildRoundImage();

            if (flags.IsSet(ConstructElementsFlags.RoundFrame))
            {
                m_Builder.BuildRoundFrame();
            }

            if (flags.IsSet(ConstructElementsFlags.ShadowInsideImage))
            {
                m_Builder.BuildShadowInsideImage();
            }
        }
    }
}
