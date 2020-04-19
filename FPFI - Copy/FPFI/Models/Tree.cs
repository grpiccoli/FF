using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FPFI.Models
{
    public class Tree
    {
        public int Id { get; set; }

        public int SpeciesId { get; set; }
        public Species Species { get; set; }

        public int RegionId { get; set; }
        public Region Region { get; set; }

        public ICollection<Entry3> Entries { get; set; }
    }

    public enum Sp3
    {
        [Display(Name = "Eucalytus sp.")]
        eucalyptus,
        [Display(Name = "Eucalytus grandis")]
        eucalyptus_grandis,
        [Display(Name = "Eucalytus globulus")]
        eucalyptus_globulus,
        [Display(Name = "Eucalytus nitens")]
        eucalyptus_nitens,
        [Display(Name = "Gmelina")]
        gmelina,
        [Display(Name = "Pinus radiata")]
        pinus_radiata,
        [Display(Name = "Pinus taeda")]
        pinus_taeda,
        [Display(Name = "Teak")]
        teak
    }

    public enum Regions
    {
        [Display(ShortName ="pradaria")]
        pradaria,
        [Display(ShortName = "uruguay")]
        uruguay,
        [Display(ShortName = "fds")]
        fds,
        [Display(Name ="Uruguay Pulp",ShortName ="uruguay_pulp")]
        uruguay_pulp,
        [Display(Name = "Uruguay Solid", ShortName = "uruguay_solid")]
        uruguay_solid,
        [Display(Name = "Uruguay Guanare", ShortName = "uruguay_guanare")]
        uruguay_guanare,
        [Display(Name = "Brazil MS", ShortName = "brazil_ms")]
        brazil_ms,
        [Display(Name = "Brazil RS", ShortName = "brazil_rs")]
        brazil_rs,
        [Display(ShortName = "chile")]
        chile,
        [Display(ShortName = "ecuador")]
        ecuador,
        [Display(Name="New Zealand",ShortName = "new_zealand")]
        new_zealand,
        [Display(ShortName = "argentina")]
        argentina,
        [Display(Name= "South East USA", ShortName = "south-east-usa")]
        south_east_usa,
        [Display(ShortName = "brazil")]
        brazil,
        [Display(ShortName = "nicaragua")]
        nicaragua,
        [Display(ShortName = "panama")]
        panama
    }
}
