using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPFI.Models
{
    public class Product2:Product
    {
        public int Entry2Id { get; set; }

        public virtual Entry2 Entry { get; set; }

        public int Priority { get; set; }
    }
}
