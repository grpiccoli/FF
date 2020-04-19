using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FPFI.Models
{
    public class Entry3:Entry
    {
        [Display(Description = "if TRUE, the yield by production at every thinning will be added to the output. Default is TRUE.")]
        public bool Include_Thinning { get; set; } //true

        [Display(Name = "Species - Region",Description = "A String indicating the species. Options are 'eucalyptus', 'pine', 'teak'. An optional string value, indicating the region. If NULL, global parameters for the species will be used. Options are listed in the details section. Default is NULL.")]
        public int TreeId { get; set; }
        public virtual Tree Tree { get; set; }

        [Display(Description = "If TRUE, and output = 'summary', the data will be aggregated using the class (column class in the products table), instead of the log name (column name). Default is FALSE.")]
        public bool ByClass { get; set; } //true

        [Display(Description = "Stump height (in m). Default is 0.1.")]
        public double Stump { get; set; }

        [Display(Description = "Discount in cm to be applied to the middle girth of the logs. Default is 0 (no discount).")]
        public double MgDisc { get; set; }

        [Display(Description = "Discount in cm to be applied to the merchantable lengths of the logs. Default is 0 (no discount).")]
        public double LengthDisc { get; set; }

        //children
        public virtual ICollection<Input3> Inputs { get; set; }

        public virtual ICollection<Product3> Products { get; set; }

        public virtual ICollection<TaperLog> TaperLogs { get; set; }

        public virtual ICollection<TaperStandHarvest> TaperStandHarvests { get; set; }

        public virtual ICollection<TaperStandThinning> TaperStandThinnings { get; set; }

        public virtual Parameter3 Parameter { get; set; }

        public virtual ICollection<Simulation3> Simulations { get; set; }

        public virtual ICollection<VP> VPs { get; set; }
    }
}
