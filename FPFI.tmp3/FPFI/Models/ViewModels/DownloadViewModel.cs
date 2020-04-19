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

        public bool Xls { get; set; }

        public bool Xml { get; set; }

        public bool Csv { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression("(?!^[0-9]*$)(?!^[a-zA-Z]*$)^([a-zA-Z0-9]{8,16})$",ErrorMessage ="Password must have 8 to 16 characters and at least one number [0-9] and one letter [a-zA-Z]")]
        public string Password { get; set; }
    }
}
