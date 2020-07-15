using System.Web.Mvc;

namespace Employee.Areas.Attend
{
    public class AttendAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Attend";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Attend_default",
                "Attend/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new string[]
                {"Employee.Areas.Attend.Controllers"}
            );
        }
    }
}