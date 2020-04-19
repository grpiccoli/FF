using FPFI.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FPFI.Models.ViewModels
{
    public class UserViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8,ErrorMessage ="Password must have at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).{8,}$", ErrorMessage = "Password must include at least 1 lowercase letter and 1 number")]
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
        [AtLeastOneProperty(ErrorMessage = "You must select at least one User Claim")]
        public List<SelectListItem> UserClaims { get; set; }

        [Display(Name = "Role")]
        public List<SelectListItem> ApplicationRoles { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string ApplicationRoleID { get; set; }
    }
}
