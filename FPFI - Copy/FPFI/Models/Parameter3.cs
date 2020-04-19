using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPFI.Models
{
    public class Parameter3:Parameter
    {
        public int Entry3Id { get; set; }
        public virtual Entry3 Entry3 { get; set; }
    }
}
