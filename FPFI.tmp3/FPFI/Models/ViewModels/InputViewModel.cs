using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FPFI.Models.ViewModels
{
    public class InputViewModel
    {
        public IFormFile Excel { get; set; }

        [Range(1,99)]
        [Display(Name ="Start")]
        public int AgeStart { get; set; }

        [Range(2, 100)]
        [Display(Name = "End")]
        public int AgeEnd { get; set; }

        public Dist Distribution { get; set; }

        [Display(Name = "Distribution Thinning")]
        public Dist DistributionThinning { get; set; }

        [Range(0, 99)]
        [Display(Name = "Standard Deviation")]
        public decimal Deviation { get; set; }

        public Sp Species { get; set; }

        [Display(Name = "Height function")]
        public Hfunc HeightFunction { get; set; }

        [Display(Name = "Taper function")]
        public Model Model { get; set; }

        [Display(Name = "Log Volume Formula")]
        public VolF VolumeFormula { get; set; }

        public Way Way { get; set; }
    }
}
