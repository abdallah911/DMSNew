using System.Web;
using System.Web.Optimization;

namespace DMS_Authontication1
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/assets/HomeTemplete/Scripts/jquery-3.0.0.min.js",
                        "~/assets/HomeTemplete/scripts/jquery-ui-1.12.1.js",
                        "~/assets/HomeTemplete/scripts/DataTables/jquery.dataTables.min.js",
                        "~/assets/HomeTemplete/scripts/DataTables/dataTables.bootstrap.js",
                        "~/assets/HomeTemplete/scripts/DataTables/dataTables.responsive.min.js",
                        "~/assets/HomeTemplete/scripts/bootbox.js",
                        "~/assets/HomeTemplete/scripts/toastr.min.js",
                        "~/assets/HomeTemplete/scripts/select2.min.js",
                        "~/assets/global/plugins/jquery-validity/js/jquery.validity.min.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/assets/HomeTemplete/Scripts/jquery.validate*"));
            
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/assets/HomeTemplete/Scripts/kendo/kendo.all.min.js", 
                        "~/assets/HomeTemplete/Scripts/kendo/kendo.aspnetmvc.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/kendo").Include(
                       "~/assets/HomeTemplete/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/assets/HomeTemplete/Scripts/bootstrap.js",
                      "~/assets/HomeTemplete/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/assets/HomeTemplete/Content/css").Include(
                      "~/assets/HomeTemplete/Content/bootstrap.css",
                      "~/assets/HomeTemplete/Content/site.css",
                      "~/assets/HomeTemplete/scripts/jquery-ui-1.12.1/jquery-ui.css",
                      "~/assets/HomeTemplete/Content/DataTables/css/dataTables.bootstrap.css",
                      "~/assets/HomeTemplete/Content/DataTables/css/responsive.dataTables.min.css"
                      ));

            bundles.Add(new StyleBundle("~/assets/HomeTemplete/Content/kendo/css").Include(
                     "~/assets/HomeTemplete/Content/kendo/kendo.common-bootstrap.min.css",
                       "~/assets/HomeTemplete/Content/kendo/kendo.bootstrap.min.css"
                     ));

            bundles.IgnoreList.Clear();

        }
    }
}
