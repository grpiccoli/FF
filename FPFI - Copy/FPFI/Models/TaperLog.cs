using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPFI.Models
{
    public class TaperLog
    {
        public int Id { get; set; }

        public int Entry3Id { get; set; }
        public Entry3 Entry3 { get; set; }

        public int Idseq { get; set; }

        public string Grade { get; set; }

        public string LogType { get; set; }

        public int Log { get; set; }

        public double Diameter { get; set; }

        public double Volume { get; set; }

        public string Product { get; set; }

        public int Value { get; set; }

        public string Class { get; set; }

        public int Id_ { get; set; }

        public int Idg { get; set; }

        public double Dbh { get; set; }

        public double Ht { get; set; }

        public double Freq { get; set; }

        public int Idgu { get; set; }

        public int Hp { get; set; }

        public int Hm { get; set; }

        public TypeLog Type { get; set; }
    }

    public enum TypeLog
    {
        Harvest,Thinning
    }
}
