using System;
using System.Collections.Generic;
using System.Text;

namespace TopLearn.Core.Convertors
{
    public class OptimizeText
    {
        public static string OptimizeEmail(string email) => email.Trim().ToLower();
    }
}
