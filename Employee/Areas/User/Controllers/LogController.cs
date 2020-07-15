using EF;
using Employee.MyFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Employee.Areas.User.Controllers
{
    [UserFilter]
    public class LogController : Controller
    {
        // GET: User/Log
        public ActionResult LogView()
        {
            return View();
        }
        public ActionResult GetList(int page, int limit, string keyword, string 起始日期, string 结束日期)
        {
            MyContext context = new MyContext();
            var query = context.日志表.AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(u => u.账号.Contains(keyword));
            }
            if (!string.IsNullOrWhiteSpace(起始日期))
            {
                DateTime start = Convert.ToDateTime(起始日期);
                query = query.Where(u => u.操作日期 >= start);
            }
            if (!string.IsNullOrWhiteSpace(结束日期))
            {
                DateTime end = Convert.ToDateTime(结束日期);
                query = query.Where(u => u.操作日期 <= end);
            }
            var pageQuery = query.OrderBy(a => a.账号).Skip(limit * (page - 1)).Take(limit).ToList();

            var result = new
            {
                code = 0,
                msg = "",
                count = query.Count(),
                data = pageQuery
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}