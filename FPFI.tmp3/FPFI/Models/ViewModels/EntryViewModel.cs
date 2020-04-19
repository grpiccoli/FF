using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FPFI.Models;

namespace FPFI.Models.ViewModels
{
    public class EntryViewModel
    {
        public int EntryId { get; set; }

        //Settings
        [Range(1,99)]
        [Display(Name ="Start", ShortName = "", Description = "")]
        public int AgeStart { get; set; }

        [Range(2, 100)]
        [Display(Name = "End")]
        public int AgeEnd { get; set; }

        public Models.Dist Distribution { get; set; }

        [Display(Name = "Distribution Thinning")]
        public Models.Dist DistributionThinning { get; set; }

        [Range(0, 99)]
        [Display(Name = "Standard Deviation")]
        public decimal Deviation { get; set; }

        public Models.Sp Species { get; set; }

        [Display(Name = "Height function")]
        public Models.Hfunc HeightFunction { get; set; }

        [Display(Name = "Taper function")]
        public Models.Model Model { get; set; }

        [Display(Name = "Log Volume Formula")]
        public Models.VolF VolumeFormula { get; set; }

        public Models.Way Way { get; set; }

        //DATA
        public string IP { get; set; }

        public DateTime? CreationDate { get; set; }

        //INPUT
        public int Inputs { get; set; }

        public int Products { get; set; }

        //CONFIG
        public double Beta1 { get; set; }
        public double Beta2 { get; set; }
        public double Beta3 { get; set; }
        public double Beta4 { get; set; }
        public double Alpha1 { get; set; }
        public double Alpha2 { get; set; }
        public bool Processed { get; set; }
        public DateTime ProcessDate { get; set; }
        public TimeSpan ProcessTime { get; set; }
    }
}
