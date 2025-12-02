namespace Dietcode.Core.Lib
{
    public static partial class Extensions
    {
        public static bool IsGreaterThan(this int i, int value)
            => i > value;

        public static bool IsLessThan(this int i, int value)
            => i < value;

        public static bool IsEqual(this int i, int value)
            => i == value;

        // 🔥 adicionais úteis (sem substituir os existentes):
        public static bool IsGreaterOrEqual(this int i, int value)
            => i >= value;

        public static bool IsLessOrEqual(this int i, int value)
            => i <= value;

        public static bool IsBetween(this int i, int min, int max, bool inclusive = true)
            => inclusive
                ? i >= min && i <= max
                : i > min && i < max;

        public static bool IsZero(this int i)
            => i == 0;

        public static bool IsPositive(this int i)
            => i > 0;

        public static bool IsNegative(this int i)
            => i < 0;
    }
}
