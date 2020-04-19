using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPFI.Models
{
    public class Product
    {
        public int Id { get; set; }

        public int EntryId { get; set; }

        public virtual Entry Entry { get; set; }

        public string X_1 { get; set; }

        public int Diameter { get; set; }

        public int Length { get; set; }
    }
}
