using Dietcode.Classic.Lib.Interfaces;

namespace Dietcode.Classic.Lib.Pagging
{
    public class PageParameter : IPageParameter
    {
        public const int DefaultPageSize = 100;
        public const int MaxPageSize = 500;

        private int _pageSize = DefaultPageSize;
        private int _pageNumber = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = Clamp(value, 1, MaxPageSize);
        }

        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = Math.Max(1, value);
        }

        private static int Clamp(int value, int min, int max)
            => value < min ? min : value > max ? max : value;
    }
}

