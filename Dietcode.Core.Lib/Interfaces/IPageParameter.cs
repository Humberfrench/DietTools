namespace Dietcode.Core.Lib.Interfaces
{
    public interface IPageParameter
    {
        /// <summary>
        /// Limite de objetos retornados.
        /// </summary>
        int Limite { get; set; }

        /// <summary>
        /// Posicao atual na paginacao.
        /// </summary>
        int Posicao { get; set; }
    }
}
