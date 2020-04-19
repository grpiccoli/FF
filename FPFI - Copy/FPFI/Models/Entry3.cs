using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FPFI.Models
{
    public class Entry3:Entry
    {
        public bool Include_Thinning { get; set; } //true

        [Display(Name = "Species - Region")]
        public int TreeId { get; set; }
        public virtual Tree Tree { get; set; }

        public bool ByClass { get; set; } //true

        public double Stump { get; set; }

        public double MgDisc { get; set; }

        public double LengthDisc { get; set; }

        //children
        public virtual ICollection<Input3> Inputs { get; set; }

        public virtual ICollection<Product3> Products { get; set; }

        public virtual ICollection<TaperLog> TaperLogs { get; set; }

        public virtual ICollection<TaperStandHarvest> TaperStandHarvests { get; set; }

        public virtual ICollection<TaperStandThinning> TaperStandThinnings { get; set; }

        public virtual Parameter3 Parameter { get; set; }

        public virtual ICollection<Simulation3> Simulations { get; set; }

        public virtual ICollection<VP> VPs { get; set; }
    }
}
