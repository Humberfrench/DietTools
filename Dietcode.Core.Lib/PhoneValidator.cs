using System.Text.RegularExpressions;

namespace Dietcode.Core.Lib
{
    public static class PhoneValidator
    {
        public static PhoneValidatorData IsValidPhoneNumber(string phoneNumber)
        {
            var phoneValidatorData = new PhoneValidatorData
            {
                Valid = true,
                Message = "OK"
            };
            string pattern = @"^\d+$";
            var valid = Regex.IsMatch(phoneNumber, pattern);
            if (!valid)
            {
                phoneValidatorData.Message = "O número de telefone fixo deve ter somente dígitos";
                phoneValidatorData.Valid = false;
                return phoneValidatorData;
            }
            if (phoneNumber.Length != 10)
            {
                phoneValidatorData.Message = "O número de telefone fixo deve ter 10 dígitos, contendo DDD + Número";
                phoneValidatorData.Valid = false;
                return phoneValidatorData;
            }
            if (Convert.ToInt16(phoneNumber.Substring(2, 1)) < 2)
            {
                phoneValidatorData.Message = "O número de telefone fixo é inválido";
                phoneValidatorData.Valid = false;
                return phoneValidatorData;
            }

            return phoneValidatorData;
        }
        public static PhoneValidatorData IsValidCellNumber(string phoneNumber)
        {
            //11999999999
            var phoneValidatorData = new PhoneValidatorData
            {
                Valid = true,
                Message = "OK"
            };
            string pattern = @"^\d+$";
            string patternMobile = @"^\d{2}9\d{8}$";
            var valid = Regex.IsMatch(phoneNumber, pattern);
            var validMobile = Regex.IsMatch(phoneNumber, patternMobile);

            if (!valid)
            {
                phoneValidatorData.Message = "O número de telefone celular deve ter somente dígitos, contendo DDD + Número";
                phoneValidatorData.Valid = false;
                return phoneValidatorData;
            }

            if (phoneNumber.Length != 11)
            {
                phoneValidatorData.Message = "O número de telefone celular deve ter 11 dígitos, contendo DDD + Número";
                phoneValidatorData.Valid = false;
                return phoneValidatorData;
            }
            if (!validMobile)
            {
                phoneValidatorData.Message = "O número de telefone celular é inválido";
                phoneValidatorData.Valid = false;
                return phoneValidatorData;
            }

            return phoneValidatorData;
        }
    }
}
