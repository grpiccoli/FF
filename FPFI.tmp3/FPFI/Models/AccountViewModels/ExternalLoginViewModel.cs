using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FPFI.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [RegularExpression(@"^.*$")]
        public string Name { get; set; }

        [RegularExpression(@"^.*$")]
        public string Last { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Url]
        [Display(Name="Profile Image")]
        public string ProfileImageUrl { get; set; }

        [Display(Name="Short description of access requirements")]
        public string Comment { get; set; }
    }
}
