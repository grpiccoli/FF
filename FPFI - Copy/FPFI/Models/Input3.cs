using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPFI.Models
{
    public class Input3:Input
    {
        public int Entry3Id { get; set; }

        public virtual Entry3 Entry { get; set; }

        public double DBH_sd { get; set; }

        public double DBH_max { get; set; }

        public string Random_SI { get; set; }

        public string Random_BA { get; set; }
    }
}
