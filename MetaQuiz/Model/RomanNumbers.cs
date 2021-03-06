﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shenoy.Quiz.Model
{
    class RomanNumbers
    {
        public static String Convert(int num)
        {
            StringBuilder builder = new StringBuilder();
            int nl = num / 50;
            num = num % 50;
            while (nl-- > 0)
                builder.Append("L");
            nl = num / 10;
            if (nl == 4)
            {
                builder.Append("XL");
            }
            else
            {
                while (nl-- > 0)
                    builder.Append("X");
            }
            num = num % 10;
            if (num == 9)
            {
                builder.Append("IX");
                return builder.ToString();
            }
            nl = num / 5;
            num = num % 5;
            while (nl-- > 0)
                builder.Append("V");
            if (num == 4)
            {
                builder.Append("IV");
                return builder.ToString();
            }
            while (num-- > 0)
                builder.Append("I");
            return builder.ToString();
        }
    }

    class GreekNumbers
    {
        public static String Convert(int num)
        {
            string ret = "";
            if (num > 9)
                ret += s_chars[num / 10];
            return ret + s_chars[num % 10];
        }

        private static char[] s_chars =
        {
            (char)0x03b8, //0
            (char)0x03c0, //1
            (char)0x03bb, //2
            (char)0x03a9, //3
            (char)0x03b1, //4
            (char)0x03b4, //5
            (char)0x03b5, //6
            (char)0x03bc, //7
            (char)0x03c7, //8
            (char)0x03c3, //9
        };
    }
}
