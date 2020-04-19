using System.ComponentModel.DataAnnotations;

namespace FPFI.Models
{
    public class Input : Indexed
    {
        [Display(Name = "Input")]
        public int Id { get; set; }

        [Display(Name = "ID",ShortName ="id", Order = 1)]
        [Range(1, 1000000)]
        public int Id_ { get; set; }

        [Display(Name = "Macrostand",ShortName ="macrostand", Order = 2)]
        //[RegularExpression("[A-Z]+[0-9]+", ErrorMessage = "The Field {0} should be a letter followed by a number")]
        public string Macrostand { get; set; }

        [Display(Name = "P Year", ShortName = "pyear", Order = 3)]
        [DisplayFormat(DataFormatString = "{0:yyyy}")]
        [Range(1970, 2100)]
        public int Pyear { get; set; }

        [Display(Name = "Age", ShortName ="Age", Order = 4)]
        [Range(0, 40)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N6}")]
        public double Age { get; set; }

        [Display(Name = "N",ShortName ="N", Order = 5)]
        [DisplayFormat(DataFormatString = "{0:N4}")]
        public double N { get; set; }

        [Display(Name = "BA",ShortName ="BA", Order = 6)]
        [DisplayFormat(DataFormatString = "{0:N6}")]
        public double BA { get; set; }

        [Display(Name = "Dg",ShortName ="Dg", Order = 7)]
        [DisplayFormat(DataFormatString = "{0:N6}")]
        public double Dg { get; set; }

        [Display(Name = "D100",ShortName ="d100", Order = 10)]
        [DisplayFormat(DataFormatString = "{0:N6}")]
        public double D100 { get; set; }

        [Display(Name = "Hd",ShortName ="Hd", Order = 11)]
        [DisplayFormat(DataFormatString = "{0:N6}")]
        public double Hd { get; set; }

        [Display(Name = "Vt",ShortName ="Vt", Order = 12)]
        public int Vt { get; set; }

        [Display(Name = "Years",ShortName ="years", Order = 13)]
        public int Years { get; set; }

        [Display(Name = "Thinning Ages",ShortName ="thinningAge", Order = 14)]
        [RegularExpression("[0-9]+,*[0-9]*")]
        public string ThinningAges { get; set; }

        [Display(Name = "N After Thinning",ShortName ="n.afterThin", Order = 15)]
        [RegularExpression("[0-9]+,*[0-9]*")]
        public string NAfterThins { get; set; }

        [Display(Name = "Thin Types",ShortName ="thinTypes", Order = 16)]
        [RegularExpression("(dist|high),*(dist|high)*")]
        public string ThinTypes { get; set; }

        [Display(Name = "Thinning coeficients",ShortName ="thin_coeff", Order = 17)]
        [RegularExpression(@"[0-9]+(\.[0-9]+)*,*[0-9]*(\.[0-9]*)*")]
        public string ThinCoefs { get; set; }

        [Display(Name = "Hp",ShortName ="hp", Order = 18)]
        public int Hp { get; set; }

        [Display(Name = "Hm",ShortName ="hm", Order = 19)]
        public int Hm { get; set; }
    }
}
