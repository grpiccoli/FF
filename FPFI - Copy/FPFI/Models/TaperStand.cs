using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPFI.Models
{
    public class TaperStand
    {
        public int Id { get; set; }

        public int Entry3Id { get; set; }
        public Entry3 Entry3 { get; set; }

        public int Id_ { get; set; }

        public string Macrostand { get; set; }

        public int Pyear { get; set; }

        public int Idg { get; set; }

        public int Age { get; set; }
    }
}
