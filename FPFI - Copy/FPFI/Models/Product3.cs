using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPFI.Models
{
    public class Product3:Product
    {
        public int Entry3Id { get; set; }

        public virtual Entry3 Entry { get; set; }

        public int Value { get; set; }

        public LogType LogType { get; set; }

        public Class Class { get;set;}
    }

    public enum LogType
    {
        p,u
    }

    public enum Class
    {
        VP1 = 1,VP2 = 2,VP3 = 3,VP4 = 4,VP5 = 5,VP6 = 6
    }
}
