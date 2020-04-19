using System.ComponentModel.DataAnnotations;

namespace FPFI.Models
{
    public class TaperStandThinning:TaperStand
    {
        [Display(ShortName = "thin_year")]
        public int Thin_year { get; set; }

        [Display(ShortName = "thin.name")]
        public string Thin_name { get; set; }
    }
}
