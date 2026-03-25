using Dietcode.Core.Lib.Interfaces;

namespace Dietcode.Core.Lib.Pagging
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
            set => _pageSize = Math.Clamp(value, 1, MaxPageSize);
        }

        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = Math.Max(1, value);
        }

    }
}
