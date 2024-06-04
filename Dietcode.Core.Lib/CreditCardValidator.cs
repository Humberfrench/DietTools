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
    }
}