namespace Dietcode.Core.Lib
{
    public static class Formatacao
    {
        public static string MascararCartaoDeCredito(string cartao)
        {
            var bin = cartao.Substring(0, 6);

            var last = cartao.Substring(cartao.Trim().Length - 4, 4);

            return $"{bin}******{last}";
        }


        public static string ObterBinUnico(string cardNumber)
        {
            return $"{cardNumber.Substring(0, 6)}{cardNumber.Substring(cardNumber.Length - 4, 4)}";
        }

    }
}
