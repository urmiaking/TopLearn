using System;
using System.Collections.Generic;
using System.Text;

namespace TopLearn.Core.Generators
{
    public class Generator
    {
        public static string GenerationUniqueName() => Guid.NewGuid().ToString().Replace("-", "");
    }
}
