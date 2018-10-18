// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;

namespace Softeq.XToolkit.Common.Helpers
{
    public class CounterNamesHelper
    {
        private readonly string _str1;
        private readonly string _str2;
        private readonly string _str3;

        public CounterNamesHelper(string str1, string str2, string str3)
        {
            _str1 = str1;
            _str2 = str2;
            _str3 = str3;
        }

        public string GetCorrectString(int quantity)
        {
            return LastLetter(quantity, new List<string> {_str1, _str2, _str3});
        }

        private static string LastLetter(int quantity, IList<string> variants)
        {
            quantity = Math.Abs(quantity);

            string word;
            if (quantity.ToString().IndexOf('.') > -1)
            {
                word = variants[1];
            }
            else
            {
                word = quantity % 10 == 1 && quantity % 100 != 11
                    ? variants[0]
                    : quantity % 10 >= 2 && quantity % 10 <= 4 && (quantity % 100 < 10 || quantity % 100 >= 20)
                        ? variants[1]
                        : variants[2];
            }

            return word;
        }
    }
}