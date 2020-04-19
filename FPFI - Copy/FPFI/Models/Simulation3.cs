using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FPFI.Models
{
    public class Simulation3:Simulation
    {
        public int Entry3Id { get; set; }
        public Entry3 Entry3 { get; set; }
    }
}
