using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FPFI.Models
{
    public class Simulation2:Simulation
    {
        public int Entry2Id { get; set; }
        public Entry2 Entry2 { get; set; }
    }
}
