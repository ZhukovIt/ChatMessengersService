using System;

namespace SiMed.Clinic.DataModel
{
    [Flags]
    public enum ConstructElementsFlags : uint
    {
        RoundImage = 0x0001,
        RoundFrame = 0x0002,
        ShadowInsideImage = 0x0004,
        All = 0x0007
    }
}
