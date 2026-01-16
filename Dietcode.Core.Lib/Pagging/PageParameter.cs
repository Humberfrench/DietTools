using Dietcode.Core.Lib.Interfaces;

namespace Dietcode.Core.Lib.Pagging
{
    public class PageParameter : IPageParameter
    {
        /// <summary>
        /// Limite de objetos retornados. (Default: 100)
        /// </summary>
        public int Limite { get; set; } = 100;

        /// <summary>
        /// Posicao atual na paginacao. (Default: 1)
        /// </summary>
        public int Posicao { get; set; } = 1;
    }
}
