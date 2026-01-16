namespace Dietcode.Core.Lib
{
    public static class Month
    {
        private static readonly List<string> _months =
        [
            "Janeiro",
            "Fevereiro",
            "Março",
            "Abril",
            "Maio",
            "Junho",
            "Julho",
            "Agosto",
            "Setembro",
            "Outubro",
            "Novembro",
            "Dezembro"
        ];

        /// <summary>
        /// Retorna o nome completo do mês.
        /// Aceita índice iniciando em 0 (Janeiro) ou 1 (Janeiro).
        /// </summary>
        public static string Name(int index)
        {
            index = NormalizeIndex(index);
            return _months[index];
        }

        /// <summary>
        /// Retorna o nome abreviado do mês (3 letras).
        /// Aceita índice iniciando em 0 ou 1.
        /// </summary>
        public static string Short(int index)
        {
            index = NormalizeIndex(index);
            return _months[index].Substring(0, 3);
        }

        public static int Count => _months.Count;

        /// <summary>
        /// Permite index 1-12 ou 0-11 de forma segura.
        /// </summary>
        private static int NormalizeIndex(int index)
        {
            // se vier 1..12, converte para 0..11
            if (index >= 1 && index <= 12)
                index--;

            if (index < 0 || index >= _months.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Índice de mês inválido. Use 1-12 ou 0-11.");

            return index;
        }
    }
}
