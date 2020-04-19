using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FPFI.Models
{
    public class Diam
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }

        public int Entry2Id { get; set; }

        public int Idg { get; set; }

        public string Name { get; set; }

        public double Value { get; set; }
    }
}
