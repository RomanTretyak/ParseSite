using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParseSites
{
    static class ExtensionString
    {
        public static bool IsValidEmail(this string input)
        {
            var regEx = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            return regEx.IsMatch(input);
        }

        public static string ReplaceCode(this string input)
        {
            if (string.IsNullOrEmpty(input) == false)
            {
                input = Regex.Replace(input, @"\t|\n|\r", "").Trim();
            }
            return input;
        }
    }
}
