

using Dietcode.Classic.Lib.Interfaces;

namespace Dietcode.Classic.Lib.Pagging
{
    public class PagedCollection<T> : ISuccessPayload
    {
        public PagedCollection()
        {
            Items = Array.Empty<T>();
        }

        public PagedCollection(IReadOnlyList<T> items, int totalItems, int pageNumber, int pageSize)
        {
            Items = items ?? Array.Empty<T>();
            TotalItems = totalItems;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        /// <summary>
        /// Quantidade máxima de itens por página.
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// Página atual (base 1).
        /// </summary>
        public int PageNumber { get; }

        /// <summary>
        /// Total de registros disponíveis.
        /// </summary>
        public int TotalItems { get; }

        /// <summary>
        /// Total de páginas calculadas.
        /// </summary>
        public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling((double)TotalItems / PageSize);

        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;

        public int? PreviousPage => HasPrevious ? PageNumber - 1 : null;
        public int? NextPage => HasNext ? PageNumber + 1 : null;

        /// <summary>
        /// Itens da página atual.
        /// </summary>
        public IReadOnlyList<T> Items { get; }
    }
}

