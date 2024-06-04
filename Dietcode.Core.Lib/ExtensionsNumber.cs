namespace Dietcode.Core.Lib
{
    public static partial class Extensions
    {
        public static bool IsGreaterThan(this int i, int value)
        {
            return i > value;
        }
        public static bool IsLessThan(this int i, int value)
        {
            return i < value;
        }
        public static bool IsEqual(this int i, int value)
        {
            return i == value;
        }
    }
}
