using FPFI.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FPFI.Models.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        [Display(Name = "User Claims")]
        [AtLeastOneProperty(ErrorMessage = "You must select at least one User Claim")]
        public List<SelectListItem> UserClaims { get; set; }

        [Display(Name = "Role")]
        public List<SelectListItem> ApplicationRoles { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string ApplicationRoleID { get; set; }
    }
}
