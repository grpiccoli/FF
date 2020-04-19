using System;

namespace FPFI.Models.ViewModels
{
    public class EntryViewModel
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public Stage Stage { get; set; }

        public DateTime? ProcessStart { get; set; }

        public double? ProcessTime { get; set; }

        public string IP { get; set; }

        public int Version { get; set; }

        public string Output { get; set; }
    }
}
