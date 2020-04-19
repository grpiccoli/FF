using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FPFI.Models
{
    public class Entry
    {
        public int Id { get; set; }

        public string ApplicationUserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        [NotMapped]
        public IFormFile Excel { get; set; }

        [Range(1,99)]
        [Display(Name ="Start", ShortName = "", Description = "A vector with the c(min, max) age range to be used in the cutting simulation.")]
        public int AgeStart { get; set; }

        [Range(2, 100)]
        [Display(Name = "End")]
        public int AgeEnd { get; set; }

        [Display(Description = "A number indicating the standard deviation.")]
        public Dist Distribution { get; set; }

        [Display(Name = "Distribution Thinning",Description = @"A string, indicating the distribution type to be used to simulate the trees. Options are ""normal"" or ""weibull"". Default to normal. As a improvement, this must be separate for thinning (as a vector with different distributions for every thinning) and to produce trees.")]
        public Dist DistributionThinning { get; set; }

        [Display(Name = "Taper function",Description = "Model to be used")]
        public Model Model { get; set; }

        [Display(Name = "Log Volume Formula",Description = @"Volume formula to use. Currently log volumes are calculated using the Smalian formula (""smalian""), Newton formula (""newton""), geometric formula (""geometric""), Hoppus formula (""hoppus""), and JAS formula (""jas"").")]
        public VolF VolumeFormula { get; set; }

        [Display(Description = @"A string indicating the taper way to compute the volume production. Options are ""lmak"" or ""lpBuck"". Defualt is lmak.")]
        public Way Way { get; set; }

        public string IP { get; set; }

        [Display(Name = "Date Submitted")]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime? ProcessStart { get; set; }

        public double? ProcessTime { get; set; }

        public Stage Stage { get; set; }

        [Display(Description = "Add a brief name or comment that describes your entry")]
        public string Description { get; set; }

        [Display(Name = "Log")]
        public string Output { get; set; }
    }

    public enum Stage
    {
        Created = 0,
        [Display(Name = "Queued or Failed")]
        Submitted = 1,
        [Display(Name = "Data Read Done")]
        Read = 2,
        [Display(Name = "Simulation Done")]
        Simulated = 3,
        [Display(Name = "Simulations Uploaded to Database")]
        SimUploaded = 4,
        [Display(Name = "Tapers Uploaded to Database")]
        TaperUploaded = 5,
        [Display(Name = "Taper Attributes Uploaded to Database")]
        DiamUploaded = 6,
        [Display(Name = "Processing Finished, Email Sent")]
        EmailSent = 7,
        [Display(Name = "Error while processing entry")]
        Error = 8
    }

    public enum Dist
    {
        [Display(Name = "Normal")]
        normal,
        [Display(Name = "Weibull")]
        weibull,
    }

    public enum Model
    {
        [Display(Name = "DSS's Max-Burkhart model", Description = "the modified segmented Max-Burkhart model implemented in DSS grandis (SAG grandis model)")]
        dss,
        [Display(Name ="Fang model", Description ="the Fang-Border-Bailey model")]
        fang,
        [Display(Name ="Bruce's model", Description = "the Bruce's model for pine")]
        bruce,
        [Display(Name = "Cao's Max-Burkhart model", Description = "the modified segmented Max-Burkhart model developed by Cao")]
        cao,
        [Display(Name ="FPFI's Max-Burkhart model",Description = "the modified segmented Max-Burkhart model implemented by FPFI")]
        cao2,
        [Display(Name ="Granflor's FDS model", Description = "the 7th degree polynomial model developed by Granflor for FDS")]
        fds,
        [Display(Name = "Garay", Description = "the Garay's taper model")]
        garay,
        [Display(Name = "segmented Max-Burkhart", Description = "the segmented Max-Burkhart polynomail model")]
        mb,
        [Display(Name = "Thomas-Parresol for pine", Description = "the Thomas and Parresol model for pine")]
        parresol,
        [Display(Name = "Polynomial 5", Description = "the 5th degree polynomial model")]
        poly5,
        [Display(Name = "Grandflor polynomial", Description = "the 6th degree polynomial model developed by Granflor for PRAD")]
        prad,
        [Display(Name = "SisPinus", Description = "the 4th degree polynomial model used in SisPinus")]
        sispinus,
        [Display(Name = "Warner", Description = "Warner")]
        warner,
        [Display(Name ="Muhairwe",Description = "the Muhairwe's model modified by Methol")]
        nm
    }

    public enum VolF
    {
        [Display(Name = "Smalian formula")]
        smalian,
        [Display(Name = "Newton formula")]
        newton,
        [Display(Name = "Geometric formula")]
        geometric,
        [Display(Name = "Hoppus formula")]
        hoppus,
        [Display(Name = "JAS formula")]
        jas
    }

    public enum Way
    {
        [Display(Name = "lmak")]
        lmak,
        [Display(Name = "lpBuck")]
        lpBuck
    }
}
