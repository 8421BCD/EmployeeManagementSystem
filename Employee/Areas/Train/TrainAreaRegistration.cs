using System.Web.Mvc;

namespace Employee.Areas.Train
{
    public class TrainAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Train";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Train_default",
                "Train/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new string[]
                {"Employee.Areas.Train.Controllers"}

            );
        }
    }
}