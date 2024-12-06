using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optics
{
    public static class GenerationData
    {
        public static string GenerationLogin()
        {
            string chars = "0123456789!@#$%^&*()";

            Random random = new Random();
            StringBuilder password = new StringBuilder();

            for (int i = 0; i < 10; i++)
            {
                password.Append(chars[random.Next(chars.Length)]);
            }
            string login = "login" + password.ToString();
            return login;
        }
        public static string GenerationPassword()
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%?(){}[]*/.^+,";

            Random random = new Random();
            StringBuilder password = new StringBuilder();

            for (int i = 0; i < 10; i++)
            {
                password.Append(chars[random.Next(chars.Length)]);
            }
            return password.ToString();
        }
        public static string GenerationArticle()
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            Random random = new Random();
            StringBuilder password = new StringBuilder();

            for (int i = 0; i < 6; i++)
            {
                password.Append(chars[random.Next(chars.Length)]);
            }
            return password.ToString();
        }
    }
}
