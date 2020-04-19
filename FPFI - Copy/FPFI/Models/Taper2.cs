using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FPFI.Models
{
    public class Taper2
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }

        public int Id_ { get; set; }

        public int Entry2Id { get; set; }

        public virtual Entry2 Entry { get; set; }

        public int Idg { get; set; }

        public double Dbh { get; set; }

        public double Ht { get; set; }

        public int Freq { get; set; }

        public int Idgu { get; set; }

        public int Hp { get; set; }

        public int Hm { get; set; }

        public double MerchVol { get; set; }
    }
}
