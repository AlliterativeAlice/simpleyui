using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace SimpleYUI
{
    public static class Bundler
    {

        public static Bundle CSS(bool debug)
        {
            return new Bundle(Bundle.BundleType.CSS,debug);
        }

        public static Bundle CSS(bool debug, bool useCompression, bool removeComments, int lineBreakPosition)
        {
            return new Bundle(Bundle.BundleType.CSS, debug);
        }

        public static Bundle CSS()
        {
            return CSS(PageIsInDebugMode());
        }

        public static Bundle JavaScript(bool debug)
        {
            return new Bundle(Bundle.BundleType.JavaScript, debug);
        }

        public static Bundle JavaScript(bool debug, bool useCompression, bool obfuscate, bool preserveSemicolons, bool disableOptimizations, bool ignoreEval, int lineBreakPosition)
        {
            return new Bundle(Bundle.BundleType.JavaScript, debug);
        }

        public static Bundle JavaScript()
        {
            return JavaScript(PageIsInDebugMode());
        }

        private static bool PageIsInDebugMode()
        {
            if (!String.IsNullOrEmpty(HttpContext.Current.Request["debug"]) && HttpContext.Current.Request["debug"] == "true") return true;
            return false;
        }
    }
}
