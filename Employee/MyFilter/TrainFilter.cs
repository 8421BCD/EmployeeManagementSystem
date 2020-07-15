using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Routing;
using EF;
using Microsoft.Ajax.Utilities;

namespace Employee.MyFilter
{
    public class TrainFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string id = filterContext.HttpContext.User.Identity.Name;
            using (MyContext context = new MyContext())
            {
                用户表 user = context.用户表.FirstOrDefault(u => u.账号 == id);
                员工视图 emp = context.员工视图.FirstOrDefault(u => u.工号 == id);
                if (user.权限类型 == 0 || user.权限类型 == 1 && emp.部门名称 != "培训部")
                    filterContext.Result = new RedirectToRouteResult("Default", new RouteValueDictionary(new { controller = "Home", action = "index" }));
            }
        }
    }
}