using System.Web;
using System.Web.Optimization;

namespace Employee
{
    public class BundleConfig
    {
        // 有关捆绑的详细信息，请访问 https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/layui").Include(
                        "~/Scripts/src/layui.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Scripts/src/css/layui.css"));
        }
    }
}
