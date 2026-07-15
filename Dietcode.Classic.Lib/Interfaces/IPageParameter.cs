namespace Dietcode.Classic.Lib.Interfaces
{
    public interface IPageParameter
    {
        /// <summary>
        /// Limite de objetos retornados.
        /// </summary>
        int PageSize { get; set; }

        /// <summary>
        /// Posicao atual na paginacao.
        /// </summary>
        int PageNumber { get; set; }
    }
}

