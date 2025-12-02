using System.Text.RegularExpressions;

namespace Dietcode.Core.Lib
{
    public static class PhoneValidator
    {
        private static readonly Regex DigitsOnly = new(@"^\d+$", RegexOptions.Compiled);
        private static readonly Regex MobilePattern = new(@"^\d{2}9\d{8}$", RegexOptions.Compiled);

        /// <summary>
        /// Valida telefone fixo no padrão brasileiro (10 dígitos: DDD + número).
        /// </summary>
        public static PhoneValidatorData IsValidPhoneNumber(string phoneNumber)
        {
            phoneNumber = Sanitize(phoneNumber);

            if (!DigitsOnly.IsMatch(phoneNumber))
                return Error("O número de telefone fixo deve ter somente dígitos");

            if (phoneNumber.Length != 10)
                return Error("O número de telefone fixo deve ter 10 dígitos, contendo DDD + Número");

            // regra ANATEL: telefones fixos começam de 2 a 5 (dependendo da região)
            if (!IsValidLandlineStart(phoneNumber))
                return Error("O número de telefone fixo é inválido");

            return Ok();
        }

        /// <summary>
        /// Valida telefone celular no padrão brasileiro (11 dígitos: DDD + 9 + número).
        /// </summary>
        public static PhoneValidatorData IsValidCellNumber(string phoneNumber)
        {
            phoneNumber = Sanitize(phoneNumber);

            if (!DigitsOnly.IsMatch(phoneNumber))
                return Error("O número de telefone celular deve ter somente dígitos, contendo DDD + Número");

            if (phoneNumber.Length != 11)
                return Error("O número de telefone celular deve ter 11 dígitos, contendo DDD + Número");

            if (!MobilePattern.IsMatch(phoneNumber))
                return Error("O número de telefone celular é inválido");

            return Ok();
        }

        // -------------------------------
        // Helpers
        // -------------------------------

        private static string Sanitize(string phone)
            => phone?.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "") ?? string.Empty;

        private static bool IsValidLandlineStart(string phone)
        {
            // posição 2 é o primeiro dígito do número (após DDD)
            if (!char.IsDigit(phone[2]))
                return false;

            int firstDigit = phone[2] - '0';

            // Fixo começa entre 2 e 5 (ANATEL)
            return firstDigit >= 2 && firstDigit <= 5;
        }

        private static PhoneValidatorData Ok()
            => new() { Valid = true, Message = "OK" };

        private static PhoneValidatorData Error(string msg)
            => new() { Valid = false, Message = msg };
    }
}
