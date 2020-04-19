using System.ComponentModel.DataAnnotations;

namespace FPFI.Models
{
    public class Product3 : Product
    {
        public int Entry3Id { get; set; }

        public virtual Entry3 Entry { get; set; }

        [Display(ShortName = "value", Order = 3)]
        public int Value { get; set; }

        [Display(ShortName = "log_type", Order = 5)]
        public LogType LogType { get; set; }

        [RegularExpression("VP[0-9]+", ErrorMessage = "The Field {0} should be VP followed by a number")]
        [Display(ShortName = "class", Order = 6)]
        public string Class { get;set;}
    }

    public enum LogType
    {
        p,u
    }

    //public enum Class
    //{
    //    VP1 = 1,VP2 = 2,VP3 = 3,VP4 = 4,VP5 = 5,VP6 = 6
    //}
}
