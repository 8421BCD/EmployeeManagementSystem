using System.Web.Mvc;

namespace Employee.Areas.Wage
{
    public class WageAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Wage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Wage_default",
                "Wage/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new string[]
                {"Employee.Areas.Wage.Controllers"}

            );
        }
    }
}