using System.Collections.Generic;

namespace FPFI.Data
{
    public class RoleData
    {
        public static List<string> ApplicationRoles { get; set; } = new List<string>
                                                            {
                                                                "Administrator",
                                                                "Editor",
                                                                "Guest"
                                                            };
    }
}
