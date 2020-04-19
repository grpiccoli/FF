using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FPFI.Models
{
    public class Product : Indexed
    {
        [Display(Name = "Product")]
        public int Id { get; set; }

        [Display(ShortName = "name", Order = 4)]
        public string X_1 { get; set; }

        [Display(ShortName="diameter", Order = 1)]
        public int Diameter { get; set; }

        [Display(ShortName = "length", Order = 2)]
        public int Length { get; set; }
    }
}
