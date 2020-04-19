using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
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
        [Display(Name ="Start", ShortName = "", Description = "")]
        public int AgeStart { get; set; }

        [Range(2, 100)]
        [Display(Name = "End")]
        public int AgeEnd { get; set; }

        public Dist Distribution { get; set; }

        [Display(Name = "Distribution Thinning")]
        public Dist DistributionThinning { get; set; }

        [Display(Name = "Taper function")]
        public Model Model { get; set; }

        [Display(Name = "Log Volume Formula")]
        public VolF VolumeFormula { get; set; }

        public Way Way { get; set; }

        public string IP { get; set; }

        [Display(Name = "Date Submitted")]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime? ProcessStart { get; set; }

        public double? ProcessTime { get; set; }

        public Stage Stage { get; set; }

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
        [Display(Name = "DSS's Max-Burkhart model", Description = "the modified segmented Max-Burkhart model implemented in DSS grandis")]
        dss,
        [Display(Name ="Fang model")]
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
        [Display(Name = "segmented Max-Burkhart", Description = "the segmented Max-Burkhart model")]
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
        warner
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
