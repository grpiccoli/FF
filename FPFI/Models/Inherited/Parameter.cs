using System.ComponentModel.DataAnnotations;

namespace FPFI.Models
{
    public class Parameter : Indexed
    {
        [Display(Name = "Parameter")]
        public int Id { get; set; }

        [Display(ShortName="beta1", Order = 1)]
        public double Beta1 { get; set; }

        [Display(ShortName = "beta2", Order = 2)]
        public double Beta2 { get; set; }

        [Display(ShortName = "beta3", Order = 3)]
        public double Beta3 { get; set; }

        [Display(ShortName = "beta4", Order = 4)]
        public double Beta4 { get; set; }

        [Display(ShortName = "a1", Order = 5)]
        public double Alpha1 { get; set; }

        [Display(ShortName = "a2", Order = 6)]
        public double Alpha2 { get; set; }
    }
}
