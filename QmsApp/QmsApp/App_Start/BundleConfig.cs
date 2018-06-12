using System.Web;
using System.Web.Optimization;

namespace QmsApp
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
          bundles.Add(new StyleBundle("~/Content/css").Include(
              "~/css/bootstrap.min.css",
              "~/css/bootstrap-responsive.min.css",
              "~/css/maruti-style.css",
              "~/css/maruti-media.css"
              ));
            //canvas
    
            //bootstrap
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/js/bootstrap.min.js")); 
            //flot
            bundles.Add(new ScriptBundle("~/bundles/flot").Include(
                "~/js/jquery.flot.min.js",
                "~/js/jquery.flot.resize.min.js",
                "~/js/jquery.flot.pie.min.js"
                )); 
            //js
            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                "~/js/jquery.min.js",
                "~/js/maruti.js"
               
                ));  
            //chart
            bundles.Add(new ScriptBundle("~/bundles/chart").Include(
              
               "~/js/maruti.charts.js",
                 "~/js/jquery.ui.custom.js"
                ));
            bundles.Add(new ScriptBundle("~/bundles/canvas").Include(
        "~/js/excanvas.min.js",
          "~/js/jquery.peity.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/tables").Include("~/js/maruti.tables.js"));
            bundles.Add(new ScriptBundle("~/bundles/interface").Include("~/js/jquery.gritter.min.js"));
        }
    }
}
