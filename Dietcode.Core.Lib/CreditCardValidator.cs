namespace Dietcode.Core.Lib
{
    public static class CreditCardValidator
    {
        // About the Algorithm
        /**
            @See https://en.wikipedia.org/wiki/Luhn_algorithm
            Steps:
            1 - From the rightmost Digit of a Numeric String, Double the value of every digit on odd positions
            2 - If the obtained value is greather than 9, subtract 9 from it
            3 - Sum all values
            4 - Calculate the Modulus of the value on 10 basis, if is zero so the String has a Luhnn Check Valid
        **/

        public static bool IsValidLuhnn(string value)
        {
            int valSum = 0;

            for (var i = value.Length - 1; i >= 0; i--)
            {
                //parse to int the current rightmost digit, if fail return false (not-valid id)

                if (!int.TryParse(value.Substring(i, 1), out var currentDigit))
                    return false;

                var currentProcNum = currentDigit << (1 + i & 1);
                //summarize the processed digits
                valSum += currentProcNum > 9 ? currentProcNum - 9 : currentProcNum;

            }

            // if digits sum is exactly divisible by 10, return true (valid), else false (not-valid)
            // valSum must be greater than zero to avoid validate 0000000...00 value
            return valSum > 0 && valSum % 10 == 0;
        }

        public static bool IsValidCreditCardNumber(string creditCardNumber)
        {
            // rule #1, must be only numbers
            if (creditCardNumber.All(char.IsDigit) == false)
            {
                return false;
            }
            // rule #2, must have at least 12 and max of 19 digits
            if (12 > creditCardNumber.Length || creditCardNumber.Length > 19)
            {
                return false;
            }
            // rule #3, must pass Luhnn Algorithm
            return IsValidLuhnn(creditCardNumber);

        }

        #region Validar Bandeira

        public static string ValidaBandeira(string card)
        {
            try
            {
                var first = Int16.Parse(card.Substring(0, 1));
                var second = Int16.Parse(card.Substring(0, 2));
                var third = Int16.Parse(card.Substring(0, 3));
                var forth = Int16.Parse(card.Substring(0, 4));
                var bin = Int32.Parse(card.Substring(0, 6));

                if (first == 0) //teste braspag
                {
                    return "VISA";
                }
                if (first == 4)
                {
                    if (BinElo(bin))
                    {
                        return "ELO";
                    }
                    if (BinElo(forth))
                    {
                        return "ELO";
                    }
                    return "VISA";
                }

                if (first == 5)
                {
                    if (second != 50)
                    {
                        return "MASTERCARD";
                    }
                    else
                    {
                        if (forth == 5067)
                        {
                            return "ELO";
                        }
                        else
                        {
                            if (BinElo(bin))
                            {
                                return "ELO";
                            }
                        }
                    }
                    return "MASTERCARD";
                }

                if (first == 2)
                {
                    if (Between(bin, 222100, 272099))
                    {
                        return "MASTERCARD";
                    }
                    return "ND";
                }

                if (first == 3)
                {
                    if (BinAmex(second))
                    {
                        return "AMEX";
                    }
                    if (BinHipercard(forth))
                    {
                        return "HIPERCARD";
                    }
                    if (BinDinners(second))
                    {
                        return "DINERS";
                    }
                    if (BinDinners(third))
                    {
                        return "DINERS";
                    }
                    if (BinJcb(second))
                    {
                        return "JCB";
                    }

                }

                if (first == 6)
                {
                    if (BinElo(bin))
                    {
                        return "ELO";
                    }
                    if (BinDiscover(second))
                    {
                        return "DISCOVER";
                    }
                    if (BinDiscover(third))
                    {
                        return "DISCOVER";
                    }
                    if (BinDiscover(forth))
                    {
                        return "DISCOVER";
                    }
                    if (BinHipercard(bin))
                    {
                        return "HIPERCARD";
                    }
                    return "ND";
                }


            }
            catch (Exception)
            {
                return "ND";
            }

            return "ND";

        }

        private static bool BinElo(int bin)
        {
            switch (bin)
            {
                case 4011: return true;
                case 431274: return true;
                case 438935: return true;
                case 451416: return true;
                case 457393: return true;
                case 4576: return true;
                case 457631: return true;
                case 457632: return true;
                case 504175: return true;
                case 627780: return true;
                case 636297: return true;
                case 636368: return true;
                case 636369: return true;
                default: return BinEloInRange(bin);
            }

            bool BinEloInRange(int binValue)
            {
                var retorno = Between(binValue, 506699, 506778);
                if (retorno)
                {
                    return true;
                }

                retorno = Between(binValue, 509000, 509999);
                if (retorno)
                {
                    return true;
                }

                retorno = Between(binValue, 650031, 650033);
                if (retorno)
                {
                    return true;
                }

                retorno = Between(binValue, 650035, 650051);
                if (retorno)
                {
                    return true;
                }

                retorno = Between(binValue, 650405, 650439);
                if (retorno)
                {
                    return true;
                }

                retorno = Between(binValue, 650485, 650538);
                if (retorno)
                {
                    return true;
                }

                retorno = Between(binValue, 650541, 650598);
                if (retorno)
                {
                    return true;
                }

                retorno = Between(binValue, 650700, 650718);
                if (retorno)
                {
                    return true;
                }

                retorno = Between(binValue, 650720, 650727);
                if (retorno)
                {
                    return true;
                }

                retorno = Between(binValue, 650901, 650920);
                if (retorno)
                {
                    return true;
                }

                retorno = Between(binValue, 651652, 651679);
                if (retorno)
                {
                    return true;
                }

                retorno = Between(binValue, 655000, 655019);
                if (retorno)
                {
                    return true;
                }

                retorno = Between(binValue, 655021, 655058);
                return retorno;
            }
        }

        private static bool BinHipercard(int bin)
        {
            switch (bin)
            {
                case 384100: return true;
                case 384140: return true;
                case 384160: return true;
                case 606282: return true;
                case 637095: return true;
                case 637568: return true;
                case 637599: return true;
                case 637609: return true;
                case 637612: return true;
            }

            return false;
        }

        private static bool BinDinners(int bin)
        {
            switch (bin)
            {
                case 301: return true;
                case 305: return true;
                case 36: return true;
                case 38: return true;
            }

            return false;
        }

        private static bool BinJcb(int bin)
        {
            switch (bin)
            {
                case 35: return true;
            }

            return false;
        }

        private static bool BinDiscover(int bin)
        {
            switch (bin)
            {
                case 6011: return true;
                case 622: return true;
                case 64: return true;
                case 65: return true;
            }

            return false;
        }

        private static bool BinAmex(int bin)
        {
            switch (bin)
            {
                case 34: return true;
                case 37: return true;
            }

            return false;
        }

        private static bool Between(int value, int menor, int maior)
        {
            return (value >= menor) && (value <= maior);
        }

        #endregion

        public static string ObterBinUnico(string cardNumber)
        {
            return $"{cardNumber.Substring(0, 6)}{cardNumber.Substring(cardNumber.Length - 4, 4)}";
        }
    }
}