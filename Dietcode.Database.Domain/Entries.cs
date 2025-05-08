using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dietcode.Database.Domain
{
    public class Entries
    {
        public string EntryName { get; set; } = string.Empty;

        public object EntryKeyValue { get; set; } = new object();
    }
}
