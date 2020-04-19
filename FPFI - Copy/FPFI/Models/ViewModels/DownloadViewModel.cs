using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FPFI.Models.ViewModels
{
    public class DownloadViewModel
    {
        public int Id { get; set; }

        public bool Xlsx { get; set; }

        public bool Xml { get; set; }

        public bool Csv { get; set; }

        public int Version { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password have 8 alphanumeric characters including at least 1 lowercase letter and 1 number")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).{8,}$", ErrorMessage = "Password have 8 alphanumeric characters including at least 1 lowercase letter and 1 number")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
