using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FPFI.Models
{
    public class Simulation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        
        [Display(Name = "ID", ShortName = "id")]
        [Range(1, 1000000)]
        public int Id_ { get; set; }

        [Display(Name = "Macrostand", ShortName = "macrostand")]
        [RegularExpression("[A-Z]+[0-9]+", ErrorMessage = "The Field {0} should be a letter followed by a number")]
        public string Macrostand { get; set; }

        [Display(Name = "Age", ShortName = "Age")]
        [Range(0, 40)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N6}")]
        public double Age { get; set; }

        [Display(Name = "N", ShortName = "N")]
        [DisplayFormat(DataFormatString = "{0:N4}")]
        public double N { get; set; }

        [Display(Name = "BA", ShortName = "BA")]
        [DisplayFormat(DataFormatString = "{0:N6}")]
        public double BA { get; set; }

        [Display(Name = "Dg", ShortName = "Dg")]
        [DisplayFormat(DataFormatString = "{0:N6}")]
        public double Dg { get; set; }

        [Display(Name = "Hd", ShortName = "Hd")]
        [DisplayFormat(DataFormatString = "{0:N6}")]
        public double Hd { get; set; }

        [Display(Name = "Vt", ShortName = "Vt")]
        public double Vt { get; set; }

        public double Sd { get; set; }

        public double Thin_trees { get; set; }

        public bool Thinaction { get; set; }

        [Display(Name = "Thin Types", ShortName = "thinTypes")]
        [RegularExpression("(dist|high),*(dist|high)*")]
        public string ThinTypes { get; set; }

        [Display(Name = "Thinning coeficients", ShortName = "thin_coef")]
        [RegularExpression(@"[0-9]+(\.[0-9]+)*,*[0-9]*(\.[0-9]*)*")]
        public string ThinCoefs { get; set; }

        public string Distr { get; set; }

        public int Idg { get; set; }

        public double? CAI_Dg { get; set; }

        public double? CAI_Vt { get; set; }

        public double MAI_Dg { get; set; }

        public double MAI_Vt { get; set; }
    }
}
