using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace TopLearn.Core.Security
{
    public class PasswordHelper
    {
        public static string Hash(string password)
        {
            SHA256 sha256 = new SHA256Managed();
            byte[] result = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

            StringBuilder sb = new StringBuilder();

            foreach (var t in result)
            {
                sb.Append(t.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
