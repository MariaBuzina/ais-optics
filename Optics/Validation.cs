using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optics
{
    public static class Validation
    {
        public static bool IsValidLogin(char c)
        {
            return char.IsDigit(c) ||
                char.IsControl(c) ||
                c >= 'a' && c <= 'z';
        }
        public static bool IsValidPassword(char c)
        {
            return char.IsDigit(c) ||
                char.IsControl(c) ||
                c >= 'a' && c <= 'z' ||
                c >= 'A' && c <= 'Z' ||
                "!@#$%?(){}[]*/.^+,".Contains(c);
        }
        public static bool IsValidFIO(char c)
        {
            return char.IsControl(c) ||
                c >= 'а' && c <= 'я' ||
                c >= 'А' && c <= 'Я' ||
                "-".Contains(c);
        }
        public static bool IsRussianLetter(char c)
        {
            return char.IsControl(c) ||
                c >= 'а' && c <= 'я' ||
                c >= 'А' && c <= 'Я';
        }
        public static bool IsEnglishLetter(char c)
        {
            return char.IsControl(c) ||
                c >= 'a' && c <= 'z' ||
                c >= 'A' && c <= 'Z';
        }
        public static bool IsDigit(char c)
        {
            return char.IsControl(c) ||
                char.IsDigit(c);
        }
        public static bool IsValidArticle(char c)
        {
            return char.IsControl(c) ||
                char.IsDigit(c) ||
                c >= 'A' && c <= 'Z';
        }
        public static bool IsValidAddress(char c)
        {
            return char.IsControl(c) ||
                char.IsWhiteSpace(c) ||
                char.IsDigit(c) ||
                c >= 'а' && c <= 'я' ||
                c >= 'А' && c <= 'Я' ||
                ".,".Contains(c);
        }
    }
}
