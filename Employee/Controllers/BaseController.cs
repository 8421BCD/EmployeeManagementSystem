using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Employee.Model;

namespace Employee
{
    public class BaseController : Controller
    {
        // GET: Base
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (HttpContext.Session["CurentUser"] == null || !(HttpContext.Session["CurentUser"] is User))
            {
                filterContext.Result = new RedirectResult("~/Home/Sign");
            }
        }
    }
}