using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FPFI.Models
{
    public class Tree
    {
        public int Id { get; set; }

        public int SpeciesId { get; set; }
        public Species Species { get; set; }

        public int RegionId { get; set; }
        public Region Region { get; set; }

        public bool Default { get; set; }

        public ICollection<Entry3> Entries { get; set; }
    }
}
