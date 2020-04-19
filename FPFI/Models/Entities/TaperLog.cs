using System.ComponentModel.DataAnnotations;

namespace FPFI.Models
{
    public class TaperLog
    {
        [Display(Name = "Taper Log")]
        public int Id { get; set; }

        public int Entry3Id { get; set; }
        public Entry3 Entry3 { get; set; }

        [Display(ShortName = "idseq")]
        public int Idseq { get; set; }

        [Display(ShortName = "grade")]
        public string Grade { get; set; }

        [Display(ShortName = "log_type")]
        public string LogType { get; set; }

        [Display(ShortName = "log")]
        public int Log { get; set; }

        //public double Diameter { get; set; }

        [Display(ShortName = "volume")]
        public double Volume { get; set; }

        [Display(ShortName = "product")]
        public string Product { get; set; }

        [Display(ShortName = "value")]
        public int Value { get; set; }

        [Display(ShortName = "class")]
        public string Class { get; set; }

        [Display(Name = "ID", ShortName = "id")]
        public int Id_ { get; set; }

        [Display(ShortName = "idg")]
        public int Idg { get; set; }

        [Display(ShortName = "dbh")]
        public double Dbh { get; set; }

        [Display(ShortName = "ht")]
        public double Ht { get; set; }

        [Display(ShortName = "freq")]
        public double Freq { get; set; }

        [Display(ShortName = "idgu")]
        public int Idgu { get; set; }

        [Display(ShortName = "hp")]
        public int Hp { get; set; }

        [Display(ShortName = "hm")]
        public int Hm { get; set; }

        public double LED_h { get; set; }

        public double LED { get; set; }

        public double MED_h { get; set; }

        public double MED { get; set; }

        public double SED_h { get; set; }

        public double SED { get; set; }

        public TypeLog Type { get; set; }
    }

    public enum TypeLog
    {
        Harvest,Thinning
    }
}
