using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FPFI.Models.ViewModels
{
    public class UserViewModel
    {
        [Required]
        [Display(Name = "Role")]
        public string ID { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "User Claims")]
        public List<SelectListItem> UserClaims { get; set; }

        public List<SelectListItem> ApplicationRoles { get; set; }

        [Display(Name = "Role")]
        public string ApplicationRoleID { get; set; }
    }
}
