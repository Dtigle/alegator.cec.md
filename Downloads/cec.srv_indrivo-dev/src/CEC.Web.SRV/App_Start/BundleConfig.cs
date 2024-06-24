using System.Web;
using System.Web.Optimization;

namespace CEC.Web.SRV
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-ui").Include(
                        "~/Scripts/jquery-ui-{version}.js",
                        "~/Scripts/jquery-ui-sliderAccess.js",
                        "~/Scripts/jquery-ui-timepicker-addon.js",
                        "~/Scripts/i18n/jquery.ui.datepicker-ro.js",
                        "~/Scripts/jquery.maskedinput.min.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jquery.cookie").Include(
                        "~/Scripts/jquery.cookie.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/i18n/jquery.validator.messages_ro.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryform").Include(
                        "~/Scripts/jquery.form.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/devoops").Include("~/Scripts/devoops.js"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap-dialog").Include("~/Scripts/bootstrap-dialog.js"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap-select").Include("~/Scripts/bootstrap-select.js"));
            bundles.Add(new ScriptBundle("~/bundles/idleTimeout").Include(
                "~/Scripts/jquery.idletimer.js",
                "~/Scripts/jquery.idletimeout.js"));

            bundles.Add(new ScriptBundle("~/bundles/app").Include("~/Scripts/cec.app.js"));

            bundles.Add(new ScriptBundle("~/bundles/select2").Include(
                "~/Scripts/select2.min.js",
                "~/Scripts/Select2-locales/select2_locale_ro.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqgrid").Include(
                "~/Scripts/jquery.jqGrid.min.js",
                "~/Scripts/jqgrid-plugins/jQuery.jqGrid.setColWidth.js",
                "~/Scripts/i18n/grid.locale-ro.js",
                "~/Scripts/jquery.contextMenu.js",
                "~/Scripts/jquery.ui.position.js",
                "~/Scripts/jquery-multiselect/jquery.multiselect.js" //checkbox multiselect for column chooser

                //"~/Scripts/jquery.jqGrid/plugins/grid.addons.js",
                /*"~/Scripts/jquery.jqGrid/plugins/grid.postext.js",
                "~/Scripts/jquery.jqGrid/plugins/grid.setcolumns.js",
                "~/Scripts/jquery.jqGrid/plugins/jquery.contextmenu.js",
                "~/Scripts/jquery.jqGrid/plugins/jquery.searchFilter.js",
                "~/Scripts/jquery.jqGrid/plugins/jquery.tablednd.js",*/
                ));

            bundles.Add(new ScriptBundle("~/bundles/openlayers").Include(
                "~/Scripts/openlayers/OpenLayers.js"));

            bundles.Add(new ScriptBundle("~/bundles/terminal").Include(
             "~/Scripts/jquery.terminal-0.8.8.js",
             "~/Scripts/jquery.mousewheel.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/css/bootstrap-dialog.css",
                      "~/Content/css/bootstrap-select.css",
                      "~/Content/bootstrapValidator.css",
                      "~/Content/css/style.css",
                      "~/Content/jquery-ui-timepicker-addon.css",
                      "~/Content/themes/base/jquery-ui.all.css",
                      "~/Content/themes/base/minified/jquery-ui.min.css",
                      "~/Content/jquery.jqGrid/ui.jqgrid.css",
                      "~/Content/jqGrid.bootstrap.css",
                      "~/Content/css/select2.css",
                      "~/Scripts/openlayers/theme/default/style.css",
                      "~/Content/jquery.contextMenu.css",
                      "~/Content/cec.app.css",
                      "~/Content/jquery.terminal.css",
                      "~/Scripts/jquery-multiselect/jquery.multiselect.css" //checkbox multiselect for column chooser

                      ));
            BundleTable.EnableOptimizations = false;
        }
    }
}
