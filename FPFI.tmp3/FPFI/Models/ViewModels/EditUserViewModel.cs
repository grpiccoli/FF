using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FPFI.Models.ViewModels
{
    public class EditUserViewModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        [Display(Name = "Permisos de Usuario")]
        public List<SelectListItem> UserClaims { get; set; }

        public List<SelectListItem> ApplicationRoles { get; set; }

        [Display(Name = "Rol")]
        public string ApplicationRoleID { get; set; }
    }
}
