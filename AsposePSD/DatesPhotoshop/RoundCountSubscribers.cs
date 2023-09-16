using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VManager.AsposePSD.DatesPhotoshop
{
    public class RoundCountSubscribers
    {
        public static ulong RoundToNearestCircularNumber(ulong number)
        {
            ulong roundedNumber = 0;

            ulong factor = 1;
            ulong temp = number;

            while (temp >= 10)
            {
                temp /= 10;
                factor *= 10;
            }

            if (temp == 1 || temp == 5)
            {
                roundedNumber = temp * factor;
            }
            else if (temp < 5)
            {
                roundedNumber = 1 * factor;
            }
            else if (temp > 5)
            {
                roundedNumber = 5 * factor;
            }

            return roundedNumber;
        }
    }
}
