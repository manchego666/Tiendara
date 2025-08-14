using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiendara.CapaLogica
{
    public static class Mathx
    {
        public static decimal R2(decimal v) => decimal.Round(v, 2, MidpointRounding.AwayFromZero);
        public static decimal R4(decimal v) => decimal.Round(v, 4, MidpointRounding.AwayFromZero);
    }
}