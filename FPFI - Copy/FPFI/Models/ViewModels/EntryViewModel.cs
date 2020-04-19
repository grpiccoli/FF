using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FPFI.Models;

namespace FPFI.Models.ViewModels
{
    public class EntryViewModel
    {
        public int Id { get; set; }

        public Stage Stage { get; set; }

        public DateTime? ProcessStart { get; set; }

        public double? ProcessTime { get; set; }

        public string IP { get; set; }

        public int Version { get; set; }
    }
}
