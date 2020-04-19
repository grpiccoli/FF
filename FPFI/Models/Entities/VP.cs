using System.ComponentModel.DataAnnotations;

namespace FPFI.Models
{
    public class VP
    {
        [Display(Name = "VP")]
        public int Id { get; set; }

        public int Entry3Id { get; set; }
        public virtual Entry3 Entry { get; set; }

        public string Class { get; set; }

        public double Value { get; set; }

        public TypeLog Type { get; set; }

        public int Idg { get; set; }
    }
}
