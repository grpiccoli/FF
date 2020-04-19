using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FPFI.Models
{
    public class Input
    {
        public int Id { get; set; }

        public int EntryId { get; set; }

        public virtual Entry Entry { get; set; }

        [Display(Name = "ID",ShortName ="id")]
        [Range(1, 1000000)]
        public int Id_ { get; set; }

        [Display(Name = "Macrostand",ShortName ="macrostand")]
        [RegularExpression("[A-Z]+[0-9]+", ErrorMessage = "The Field {0} should be a letter followed by a number")]
        public string Macrostand { get; set; }

        [Display(Name = "P Year", ShortName = "pyear")]
        [DisplayFormat(DataFormatString = "{0:yyyy}")]
        [Range(1970, 2100)]
        public int Pyear { get; set; }

        [Display(Name = "Age", ShortName ="Age")]
        [Range(0, 40)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N6}")]
        public double Age { get; set; }

        [Display(Name = "N",ShortName ="N")]
        [DisplayFormat(DataFormatString = "{0:N4}")]
        public double N { get; set; }

        [Display(Name = "BA",ShortName ="BA")]
        [DisplayFormat(DataFormatString = "{0:N6}")]
        public double BA { get; set; }

        [Display(Name = "Dg",ShortName ="Dg")]
        [DisplayFormat(DataFormatString = "{0:N6}")]
        public double Dg { get; set; }

        [Display(Name = "D100",ShortName ="d100")]
        [DisplayFormat(DataFormatString = "{0:N6}")]
        public double D100 { get; set; }

        [Display(Name = "Hd",ShortName ="Hd")]
        [DisplayFormat(DataFormatString = "{0:N6}")]
        public double Hd { get; set; }

        [Display(Name = "Vt",ShortName ="Vt")]
        public int Vt { get; set; }

        [Display(Name = "Years",ShortName ="years")]
        public int Years { get; set; }

        [Display(Name = "Thinning Ages",ShortName ="thinningAge")]
        [RegularExpression("[0-9]+,*[0-9]*")]
        public string ThinningAges { get; set; }

        [Display(Name = "N After Thinning",ShortName ="n.afterThin")]
        [RegularExpression("[0-9]+,*[0-9]*")]
        public string NAfterThins { get; set; }

        [Display(Name = "Thin Types",ShortName ="thinTypes")]
        [RegularExpression("(dist|high),*(dist|high)*")]
        public string ThinTypes { get; set; }

        [Display(Name = "Thinning coeficients",ShortName ="thin_coef")]
        [RegularExpression(@"[0-9]+(\.[0-9]+)*,*[0-9]*(\.[0-9]*)*")]
        public string ThinCoefs { get; set; }

        [Display(Name = "Hp",ShortName ="hp")]
        public int Hp { get; set; }

        [Display(Name = "Hm",ShortName ="hm")]
        public int Hm { get; set; }
    }
}
