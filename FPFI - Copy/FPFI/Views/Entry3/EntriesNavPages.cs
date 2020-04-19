using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace FPFI.Views.Entry3
{
    public static class EntriesNavPages
    {
        public static string ActivePageKey => "ActivePage";

        public static string Step1 => "Step1";

        public static string Step2 => "Step2";

        public static string Step3 => "Step3";

        public static string Step4 => "Step4";

        public static string Step5 => "Step5";

        public static string Step1NavClass(string activePage) => PageNavClass(activePage, Step1);

        public static string Step2NavClass(string activePage) => PageNavClass(activePage, Step2);

        public static string Step3NavClass(string activePage) => PageNavClass(activePage, Step3);

        public static string Step4NavClass(string activePage) => PageNavClass(activePage, Step4);

        public static string Step5NavClass(string activePage) => PageNavClass(activePage, Step5);

        public static string Previous(string activePage)
        {
            return activePage[activePage.Length - 1] == '1' ? "disabled" : null;
        }

        public static string PreviousStep(string activePage)
        {
            var prevNumber = (Convert.ToInt16(activePage[activePage.Length - 1].ToString()) - 1).ToString();
            return "Step" + prevNumber;
        }

        public static string Next(string activePage)
        {
            return activePage[activePage.Length - 1] == '5' ? "Submit" : "Next";
        }

        public static string PageNavClass(string activePage, string page)
        {
            if(string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase))
            {
                return "active";
            }
            else if(activePage[activePage.Length-1] < page[page.Length-1])
            {
                return "disabled";
            }
            else
            {
                return null;
            }
        }
        public static void AddActivePage(this ViewDataDictionary viewData, string activePage) => viewData[ActivePageKey] = activePage;
    }
}
