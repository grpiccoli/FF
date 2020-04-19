using System.ComponentModel.DataAnnotations;

namespace FPFI.Models
{
    public class TaperStand
    {
        [Display(Name = "Taper Stand")]
        public int Id { get; set; }

        public int Entry3Id { get; set; }
        public Entry3 Entry3 { get; set; }

        public double? NA { get; set; }

        [Display(Name = "ID", ShortName = "id")]
        public int Id_ { get; set; }

        [Display(ShortName = "macrostand")]
        public string Macrostand { get; set; }

        [Display(ShortName = "pyear")]
        public int Pyear { get; set; }

        [Display(ShortName = "idg")]
        public int Idg { get; set; }

        public int Age { get; set; }
    }
}
