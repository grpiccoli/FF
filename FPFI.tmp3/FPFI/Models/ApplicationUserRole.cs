using Microsoft.AspNetCore.Identity;

namespace FPFI.Models
{
    public class ApplicationUserRole : IdentityUserRole<string>
    {
        public string RoleAssigner { get; set; }
    }
}
