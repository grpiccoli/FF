using System.ComponentModel.DataAnnotations;

namespace FPFI.Models
{
    public class Graphs
    {
        [Display(Name = "Dg", GroupName = "Dg", Order = 1)]
        [DisplayFormat(DataFormatString = "{0:N6}")]
        public double Dg { get; set; }

        [Display(Order = 2, GroupName = "Dg")]
        public double? CAI_Dg { get; set; }

        [Display(Order = 3, GroupName = "Dg")]
        public double MAI_Dg { get; set; }

        [Display(Name = "Vt", GroupName = "Vt", Order = 3)]
        public double Vt { get; set; }

        [Display(Order = 4, GroupName = "Vt")]
        public double? CAI_Vt { get; set; }

        [Display(Order = 5, GroupName = "Vt")]
        public double MAI_Vt { get; set; }

        [Display(Name = "Hd", Order = 6, GroupName = "Hd")]
        [DisplayFormat(DataFormatString = "{0:N6}")]
        public double Hd { get; set; }

        [Display(Name = "N", Order = 7, GroupName = "Hd")]
        [DisplayFormat(DataFormatString = "{0:N4}")]
        public double N { get; set; }
    }
}
