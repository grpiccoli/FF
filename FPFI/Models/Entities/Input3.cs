using System.ComponentModel.DataAnnotations;

namespace FPFI.Models
{
    public class Input3 : Input
    {
        public int Entry3Id { get; set; }

        public virtual Entry3 Entry { get; set; }

        [Display(ShortName ="DBH_sd", Order = 8)]
        public double DBH_sd { get; set; }

        [Display(ShortName = "DBH_max", Order = 9)]
        public double DBH_max { get; set; }

        [Display(ShortName = "random_SI", Order = 20)]
        public string Random_SI { get; set; }

        [Display(ShortName = "random_BA", Order = 21)]
        public string Random_BA { get; set; }
    }
}
