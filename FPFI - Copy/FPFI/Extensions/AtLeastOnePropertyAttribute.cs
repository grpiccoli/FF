using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FPFI.Extensions
{
    public class AtLeastOnePropertyAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var list = value as IList<SelectListItem>;
            if (list != null)
            {
                return list.Where(c => c.Selected).Count() > 0;
            }
            return false;
        }
    }
}
