using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPFI.Models.ViewModels
{
    public class ResultsViewModel
    {
        public IEnumerable<EntryViewModel> Entries { get; set; }

        public int? I { get; set; }

        public int? V { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public bool? Dl { get; set; }

        public Entry2 Running2 { get; set; }

        public Entry3 Running3 { get; set; }

        public bool Error { get; set; }

        public DateTime? ATD { get; set; }

        public DateTime? ETA { get; set; }

        public double ProgressPercentage { get; set; }
    }
}
