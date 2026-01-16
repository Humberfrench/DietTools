

using Dietcode.Core.Lib.Interfaces;

namespace Dietcode.Core.Lib.Pagging
{
    public class PagedCollection<T> : ISuccessPayload
    {
        public PagedCollection()
        {
            Items = Array.Empty<T>();
        }

        /// <summary>
        /// Quantidade máxima de itens por página.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Página atual (base 1).
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Total de registros disponíveis.
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Total de páginas calculadas.
        /// </summary>
        public int TotalPages =>
            PageSize <= 0 ? 0 : (int)Math.Ceiling((double)TotalItems / PageSize);

        /// <summary>
        /// Itens da página atual.
        /// </summary>
        public IReadOnlyCollection<T> Items { get; set; }
    }
}
