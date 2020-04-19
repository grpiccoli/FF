using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FPFI.Models
{
    public class Entry2:Entry
    {
        [Range(0, 99)]
        [Display(Name = "Standard Deviation")]
        public decimal Deviation { get; set; }

        public Sp Species { get; set; }

        [Display(Name = "Height function")]
        public Hfunc HeightFunction { get; set; }

        public virtual ICollection<Input2> Inputs { get; set; }

        public virtual ICollection<Product2> Products { get; set; }

        public virtual ICollection<Taper2> Tapers { get; set; }

        public virtual Parameter2 Parameter { get; set; }

        public virtual ICollection<Simulation2> Simulations { get; set; }
    }

    public enum Sp
    {
        [Display(Name = "Eucalyptus")]
        eucalyptus,
        [Display(Name = "Pine")]
        pine,
        [Display(Name = "Teca")]
        teca
    }

    public enum Hfunc
    {
        [Display(Name = "GUANARE")]
        GUANARE
    }
}
