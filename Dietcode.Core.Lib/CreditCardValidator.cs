namespace Dietcode.Core.Lib
{
    public static class CreditCardValidator
    {
        // ============================================================
        //                    LUHN VALIDATOR
        // ============================================================
        public static bool IsValidLuhnn(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            int sum = 0;

            for (int i = value.Length - 1; i >= 0; i--)
            {
                if (!int.TryParse(value[i].ToString(), out int digit))
                    return false;

                // dobra dígitos em posição alternada
                int calc = (i % 2 == value.Length % 2) ? digit * 2 : digit;

                sum += calc > 9 ? calc - 9 : calc;
            }

            return sum > 0 && sum % 10 == 0;
        }

        public static bool IsValidCreditCardNumber(string creditCardNumber)
        {
            if (string.IsNullOrWhiteSpace(creditCardNumber))
                return false;

            if (!creditCardNumber.All(char.IsDigit))
                return false;

            if (creditCardNumber.Length is < 12 or > 19)
                return false;

            return IsValidLuhnn(creditCardNumber);
        }

        // ============================================================
        //                     FLAG / BANDEIRA
        // ============================================================
        public static string ValidaBandeira(string card)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(card) || card.Length < 6)
                    return "ND";

                int first = int.Parse(card[..1]);
                int second = int.Parse(card[..2]);
                int third = int.Parse(card[..3]);
                int forth = int.Parse(card[..4]);
                int bin6 = int.Parse(card[..6]);

                return first switch
                {
                    0 => "VISA",  // teste braspag

                    4 => BinElo(bin6) || BinElo(forth) ? "ELO" : "VISA",

                    5 => second != 50 ? "MASTERCARD"
                                      : forth == 5067 || BinElo(bin6)
                                      ? "ELO"
                                      : "MASTERCARD",

                    2 => Between(bin6, 222100, 272099) ? "MASTERCARD" : "ND",

                    3 => ValidateRange3(second, third, forth),

                    6 => ValidateRange6(second, third, forth, bin6),

                    _ => "ND"
                };
            }
            catch
            {
                return "ND";
            }
        }

        private static string ValidateRange3(int second, int third, int forth)
        {
            if (BinAmex(second)) return "AMEX";
            if (BinHipercard(forth)) return "HIPERCARD";
            if (BinDinners(second) || BinDinners(third)) return "DINERS";
            if (BinJcb(second)) return "JCB";

            return "ND";
        }

        private static string ValidateRange6(int second, int third, int forth, int bin6)
        {
            if (BinElo(bin6)) return "ELO";
            if (BinDiscover(second) || BinDiscover(third) || BinDiscover(forth)) return "DISCOVER";
            if (BinHipercard(bin6)) return "HIPERCARD";

            return "ND";
        }

        // ============================================================
        //                         BIN RULES
        // ============================================================
        private static bool BinElo(int bin)
        {
            return bin switch
            {
                4011 or 431274 or 438935 or 451416 or 457393 or 4576 or
                457631 or 457632 or 504175 or 627780 or 636297 or 636368 or 636369
                    => true,

                _ => BinEloInRange(bin)
            };

            static bool BinEloInRange(int b)
            {
                return
                    Between(b, 506699, 506778) ||
                    Between(b, 509000, 509999) ||
                    Between(b, 650031, 650033) ||
                    Between(b, 650035, 650051) ||
                    Between(b, 650405, 650439) ||
                    Between(b, 650485, 650538) ||
                    Between(b, 650541, 650598) ||
                    Between(b, 650700, 650718) ||
                    Between(b, 650720, 650727) ||
                    Between(b, 650901, 650920) ||
                    Between(b, 651652, 651679) ||
                    Between(b, 655000, 655019) ||
                    Between(b, 655021, 655058);
            }
        }

        private static bool BinHipercard(int bin) =>
            bin is 384100 or 384140 or 384160 or 606282 or
                   637095 or 637568 or 637599 or 637609 or 637612;

        private static bool BinDinners(int bin) =>
            bin is 301 or 305 or 36 or 38;

        private static bool BinJcb(int bin) =>
            bin is 35;

        private static bool BinDiscover(int bin) =>
            bin is 6011 or 622 or 64 or 65;

        private static bool BinAmex(int bin) =>
            bin is 34 or 37;

        private static bool Between(int value, int menor, int maior) =>
            value >= menor && value <= maior;

        // ============================================================
        //                     BIN ÚNICO
        // ============================================================
        public static string ObterBinUnico(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Length < 10)
                return string.Empty;

            return $"{cardNumber[..6]}{cardNumber[^4..]}";
        }
    }
}
