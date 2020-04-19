using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPFI.Models
{
    public class Parameter2:Parameter
    {
        public int Entry2Id { get; set; }
        public virtual Entry2 Entry2 { get; set; }
    }
}
