using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FPFI.Models
{
    public class Simulation : Graphs
    {
        [Display(Name = "Simulations")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        
        [Display(Name = "ID", ShortName = "id")]
        [Range(1, 1000000)]
        public int Id_ { get; set; }

        [Display(Name = "Macrostand", ShortName = "macrostand")]
        [RegularExpression("[A-Z]+[0-9]+", ErrorMessage = "The Field {0} should be a letter followed by a number")]
        public string Macrostand { get; set; }

        [Display(Name = "Age")]
        [Range(0, 40)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N6}")]
        public double Age { get; set; }

        [Display(Name = "BA")]
        [DisplayFormat(DataFormatString = "{0:N6}")]
        public double BA { get; set; }

        [Display(Name = "Sd", ShortName = "sd")]
        public double Sd { get; set; }

        [Display(ShortName = "thin_trees")]
        public double Thin_trees { get; set; }

        [Display(ShortName = "thinaction")]
        public bool Thinaction { get; set; }

        [Display(Name = "Thinning Types", ShortName = "thin_type")]
        [RegularExpression("(dist|high),*(dist|high)*")]
        public string ThinTypes { get; set; }

        [Display(Name = "Thinning coeficients", ShortName = "thin_coef")]
        [RegularExpression(@"[0-9]+(\.[0-9]+)*,*[0-9]*(\.[0-9]*)*")]
        public string ThinCoefs { get; set; }

        [Display(ShortName = "distr")]
        public string Distr { get; set; }

        [Display(ShortName = "idg")]
        public int Idg { get; set; }
    }
}
