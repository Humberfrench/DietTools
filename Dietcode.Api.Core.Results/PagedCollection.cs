
namespace Dietcode.Api.Core.Results
{
    public class PagedCollection<TCollectionItem>
    {
        public PagedCollection()
        {
            Items = new List<TCollectionItem>();
        }

        /// <summary>
        /// Limite de objetos retornados.
        /// </summary>
        public int Limite { get; set; }

        /// <summary>
        /// Posicao atual na paginacao.
        /// </summary>
        public int Posicao { get; set; }

        /// <summary>
        /// Total de paginas.
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Conteúdo dos objetos paginados.
        /// </summary>
        public IEnumerable<TCollectionItem> Items { get; set; }
    }
}
