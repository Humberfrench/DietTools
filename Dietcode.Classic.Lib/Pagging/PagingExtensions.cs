using Dietcode.Classic.Lib.Interfaces;

namespace Dietcode.Classic.Lib.Pagging
{
    public static class PagingExtensions
    {
        /// <summary>
        /// Pagina uma coleção em memória (IEnumerable).
        /// </summary>
        public static PagedCollection<T> ToPaged<T>(this IEnumerable<T> source, IPageParameter page)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (page is null) throw new ArgumentNullException(nameof(page));

            var pageSize = Clamp(page.PageSize, 1, PageParameter.MaxPageSize);
            var pageNumber = Math.Max(1, page.PageNumber);

            // Evita enumerar duas vezes se já for ICollection
            int total;
            List<T> items;

            if (source is ICollection<T> col)
            {
                total = col.Count;
                items = col.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            }
            else if (source is IReadOnlyCollection<T> ro)
            {
                total = ro.Count;
                items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            }
            else
            {
                total = source.Count();
                items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            }

            return new PagedCollection<T>(items, total, pageNumber, pageSize);

        }

        /// <summary>
        /// Pagina uma query (IQueryable). Sem async/EF.
        /// Em providers como EF, Count/Skip/Take/ToList normalmente viram SQL.
        /// </summary>
        public static PagedCollection<T> ToPaged<T>(this IQueryable<T> query, IPageParameter page)
        {
            if (query is null) throw new ArgumentNullException(nameof(query));
            if (page is null) throw new ArgumentNullException(nameof(page));

            var pageSize = Clamp(page.PageSize, 1, PageParameter.MaxPageSize);
            var pageNumber = Math.Max(1, page.PageNumber);

            var total = query.Count();
            var items = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedCollection<T>(items, total, pageNumber, pageSize);
        }

        private static int Clamp(int value, int min, int max)
            => value < min ? min : value > max ? max : value;
    }
}
