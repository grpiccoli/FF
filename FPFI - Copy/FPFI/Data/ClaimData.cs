using System.Collections.Generic;

namespace FPFI.Data
{
    public class ClaimData
    {
        public static List<string> UserClaims { get; set; } = new List<string>
                                                            {
                                                                "Others",
                                                                "Data",
                                                                "Apps",
                                                                "Users"
                                                            };
    }
}
