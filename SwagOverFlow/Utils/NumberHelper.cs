using System;
using System.Linq;

namespace SwagOverFlow.Utils
{
    public static class NumberHelper
    {
        public static Int32 GetNumDecimalPlaces(Decimal value)
        {
            Int32 decimalPlaces = value.ToString().Split('.').Count() > 1
                  ? value.ToString().Split('.').ToList().ElementAt(1).Length
                  : 0;

            return decimalPlaces;
        }

        public static Int32 GetNumDigits(Decimal value)
        {
            Int32 numDigits;

            if (value == 0)
            {
                numDigits = 1;
            }
            else
            {
                numDigits = Convert.ToInt32(Math.Floor(Math.Log10(Math.Abs(Convert.ToDouble(value))) + 1));
            }

            return numDigits;
        }
    }
}
