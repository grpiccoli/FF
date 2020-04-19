using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPFI.Models
{
    public class Input2:Input
    {
        public int Entry2Id { get; set; }

        public virtual Entry2 Entry { get; set; }
    }
}
