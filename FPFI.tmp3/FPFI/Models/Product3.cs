using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPFI.Models
{
    public class Product3:Product
    {
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
        VP1,VP2,VP3,VP4,VP5,VP6
    }
}
