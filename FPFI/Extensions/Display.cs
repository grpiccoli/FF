using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FPFI.Extensions
{
    public static class Display
    {
        public static string GetAttribute<T>(string field, string attribute)
        {
            var displayAttributes = TypeDescriptor.GetProperties(typeof(T)).Find(field, false).Attributes.OfType<DisplayAttribute>().FirstOrDefault();
            if (displayAttributes == null) return String.Empty;
            switch (attribute.ToLower())
            {
                case "description":
                    return displayAttributes.Description ?? String.Empty;
                case "name":
                    return displayAttributes.GetName() ?? String.Empty;
                case "groupname":
                    return displayAttributes.GetGroupName() ?? String.Empty;
                case "shortname":
                    return displayAttributes.GetShortName() ?? String.Empty;
                default:
                    return String.Empty;
            }
        }
    }
}
