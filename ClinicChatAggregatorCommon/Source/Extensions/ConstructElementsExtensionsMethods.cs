using System;

namespace SiMed.Clinic.DataModel
{
    public static class ConstructElementsExtensionsMethods
    {
        public static bool IsSet(this ConstructElementsFlags flags, ConstructElementsFlags flagToTest)
        {
            if (flagToTest == 0)
            {
                throw new ArgumentOutOfRangeException("flagToTest", "Значение не может быть равно 0");
            }
            return (flags & flagToTest) == flagToTest;
        }

        public static bool IsClear(this ConstructElementsFlags flags, ConstructElementsFlags flagToTest)
        {
            if (flagToTest == 0)
            {
                throw new ArgumentOutOfRangeException("flagToTest", "Значение не может быть равно 0");
            }
            return !IsSet(flags, flagToTest);
        }

        public static bool AnyFlagsSet(this ConstructElementsFlags flags, ConstructElementsFlags testFlags)
        {
            return (flags & testFlags) != 0;
        }

        public static ConstructElementsFlags Set(this ConstructElementsFlags flags, ConstructElementsFlags setFlags)
        {
            return flags | setFlags;
        }

        public static ConstructElementsFlags Clear(this ConstructElementsFlags flags, ConstructElementsFlags clearFlags)
        {
            return flags & ~clearFlags;
        }

        public static void ForEach(this ConstructElementsFlags flags, Action<ConstructElementsFlags> processFlag)
        {
            if (processFlag == null)
            {
                throw new ArgumentNullException("processFlag");
            }
            for (uint bit = 1; bit != 0; bit <<= 1)
            {
                uint temp = ((uint)flags) & bit;
                if (temp != 0)
                {
                    processFlag((ConstructElementsFlags)temp);
                }
            }
        }
    }
}
