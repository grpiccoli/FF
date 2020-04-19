using System;
using System.ComponentModel.DataAnnotations;

namespace FPFI.Models.ViewModels
{
    public class InViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "ID")]
        [Range(1,1000000)]
        public int Id_ { get; set; }

        [Display(Name = "Macrostand")]
        [RegularExpression("[A-Z]+[0-9]+",ErrorMessage = "The Field {0} should be a letter followed by a number")]
        public string Macrostand { get; set; }

        [Display(Name = "P Year")]
        [DisplayFormat(DataFormatString ="{0:yyyy}")]
        [Range(1970,2100)]
        public int Pyear { get; set; }

        [Display(Name = "Age")]
        [Range(0,40)]
        public decimal Age { get; set; }

        [Display(Name = "N")]
        public decimal N { get; set; }

        [Display(Name = "BA")]
        public decimal BA { get; set; }

        [Display(Name = "Dg")]
        public decimal Dg { get; set; }

        [Display(Name = "D100")]
        public decimal D100 { get; set; }

        [Display(Name = "Hd")]
        public decimal Hd { get; set; }

        [Display(Name = "Vt")]
        public int Vt { get; set; }

        [Display(Name = "Years")]
        public int Years { get; set; }

        [Display(Name = "Thinning Ages")]
        [RegularExpression("[0-9]+,*[0-9]*")]
        public string ThinningAges { get; set; }

        [Display(Name = "N After Thinning")]
        [RegularExpression("[0-9]+,*[0-9]*")]
        public string NAfterThins { get; set; }

        [Display(Name = "Thin Types")]
        [RegularExpression("(dist|high),*(dist|high)*")]
        public string ThinTypes { get; set; }

        [Display(Name = "Thinning coeficients")]
        [RegularExpression(@"[0-9]+(\.[0-9]+)*,*[0-9]*(\.[0-9]*)*")]
        public string ThinCoefs { get; set; }

        [Display(Name = "Hp")]
        public int Hp { get; set; }

        [Display(Name = "Hm")]
        public int Hm { get; set; }
    }
}
