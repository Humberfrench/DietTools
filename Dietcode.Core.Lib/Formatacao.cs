namespace Dietcode.Core.Lib
{
    public static class Formatacao
    {
        // ============================================
        // MÉTODO PRINCIPAL — MASCARAR CARTÃO
        // ============================================
        public static string MascararCartaoDeCredito(string cartao)
        {
            if (string.IsNullOrWhiteSpace(cartao))
                return string.Empty;

            // remove espaços e formatações opcionais
            var cc = cartao.Replace(" ", "").Replace("-", "");

            if (cc.Length < 10)
                return string.Empty; // mínimo 6 + 4

            var bin = cc.Substring(0, 6);
            var last4 = cc[^4..]; // syntax mais segura

            return $"{bin}******{last4}";
        }

        // ============================================
        // BIN ÚNICO (6 + 4)
        // ============================================
        public static string ObterBinUnico(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return string.Empty;

            var cc = cardNumber.Replace(" ", "").Replace("-", "");

            if (cc.Length < 10)
                return string.Empty;

            return $"{cc[..6]}{cc[^4..]}";
        }

        // ============================================
        // BIN INÍCIO
        // ============================================
        public static string ObterBinInicio(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return string.Empty;

            var cc = cardNumber.Replace(" ", "").Replace("-", "");

            return cc.Length >= 6 ? cc[..6] : string.Empty;
        }

        // ============================================
        // BIN FIM
        // ============================================
        public static string ObterBinFim(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return string.Empty;

            var cc = cardNumber.Replace(" ", "").Replace("-", "");

            return cc.Length >= 4 ? cc[^4..] : string.Empty;
        }
    }
}
