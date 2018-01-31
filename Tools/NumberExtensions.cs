using System;
using System.Collections.Generic;
using System.Text;

namespace Tools
{
   public static class NumberExtensions
    {
        public static int ConvertToMoneyCent(this decimal money)
        {
            int cent = (int)(money * 100);
            return cent;
        }
    }
}
