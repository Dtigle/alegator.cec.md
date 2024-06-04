using System.Web;
using System.Web.Optimization;

namespace CEC.SAISE.EDayModule
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
                "~/Scripts/jquery.maskedinput.min.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                "~/Scripts/knockout-3.3.0.js",
                "~/Scripts/knockout.validation.js",
				"~/Scripts/knockout-file-bindings.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/moment").Include(
				"~/Scripts/moment-with-locales.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js",
				"~/Scripts/bootstrap-datepicker.js",
				"~/Scripts/bootstrap-dialog.js",
                "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrapsmall").Include(
                "~/Scripts/bootstrap.js",
                "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/devoops").Include("~/Scripts/devoops.js"));
            bundles.Add(new ScriptBundle("~/bundles/app").Include("~/Scripts/cec.saise.js"));

            bundles.Add(new ScriptBundle("~/bundles/select2").Include(
                "~/Scripts/select2.min.js",
                "~/Scripts/Select2-locales/select2_locale_ro.js"
                ));

            bundles.Add(new StyleBundle("~/Content/css/styles").Include(
                      "~/Content/css/font-awesome.min.css",
                      "~/Content/css/google-fonts.css",
                      "~/Content/bootstrap.css",
                      "~/Content/css/style.css",
					  "~/Content/Site.css",
					  "~/Content/bootstrap-datepicker/bootstrap-datepicker.css",
					  "~/Content/knockout-file-bindings.css",
					  "~/Content/themes/base/jquery-ui.all.css",
					  "~/Content/themes/base/minified/jquery-ui.min.css",
					  "~/Content/jquery.jqGrid/ui.jqgrid.css",
					  "~/Content/jqGrid.bootstrap.css",
                      "~/Content/css/select2.css"));

			bundles.Add(new ScriptBundle("~/bundles/idleTimeout").Include(
				"~/Scripts/jquery.idletimer.js",
				"~/Scripts/jquery.idletimeout.js"));

			bundles.Add(new ScriptBundle("~/bundles/jqGrid").Include(
				"~/Scripts/jquery.jqGrid.min.js",
                "~/Scripts/i18n/grid.locale-ro.js"
				));

			BundleTable.EnableOptimizations = true;
        }
    }
}
