using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FPFI.Models
{
    public class Entry3
    {
        public bool Include_Thinning { get; set; }

        public Tree Tree { get; set; }

        public bool ByClass { get; set; }

        public double Stump { get; set; }

        public double MgDisc { get; set; }

        public double LengthDisc { get; set; }

        //children
        public virtual IEnumerable<Input3> Inputs { get; set; }

        public virtual IEnumerable<Product3> Products { get; set; }

        //public virtual IEnumerable<Taper3> Tapers { get; set; }
    }
}
