namespace Dietcode.Core.DomainValidator.ObjectValue
{
    /// <summary>
    /// Representa uma entrada contendo um nome e um valor associado.
    /// Pode ser usada para armazenar pares chave-valor em contexto de validação ou mensagens.
    /// </summary>
    public class Entries
    {
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="Entries"/> com valores padrão.
        /// </summary>
        public Entries()
        {
            EntryName = string.Empty;
            EntryKeyValue = string.Empty;
        }

        /// <summary>
        /// Nome da entrada (chave identificadora).
        /// </summary>
        public string EntryName { get; set; }

        /// <summary>
        /// Valor associado à entrada (pode ser de qualquer tipo).
        /// </summary>
        public object EntryKeyValue { get; set; }
    }
}
