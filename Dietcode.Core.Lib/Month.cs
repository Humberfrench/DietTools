using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dietcode.Core.Lib
{
    public class Month
    {
        private static List<string> _months = new List<string>()
        {
            "Janeiro",
            "Fevereiro",
            "Março",
            "Abril",
            "Maio",
            "Junho",
            "Julho",
            "Agosto",
            "Setembro",
            "Outubro",
            "Novembro",
            "Dezembro"
        };

        public static string Name(int i)
        {
            return _months[i];
        }

        public static string Short(int i)
        {
            return _months[i].Substring(0, 3);
        }

        public static int Count { get { return _months.Count; } }
    }
}
