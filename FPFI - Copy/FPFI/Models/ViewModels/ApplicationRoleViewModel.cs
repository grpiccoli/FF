using System.ComponentModel.DataAnnotations;

namespace FPFI.Models.ViewModels
{
    public class ApplicationRoleViewModel
    {
        public string ID { get; set; }

        [Display(Name = "Rol")]
        public string RoleName { get; set; }
        public string Description { get; set; }
    }
}
