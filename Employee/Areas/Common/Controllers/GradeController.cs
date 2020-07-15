using EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Employee.Areas.Common.Controllers
{
    public class GradeController : Controller
    {
        // GET: Common/Grade
        public ActionResult GradeView()
        {
            return View();
        }
        
        public ActionResult GetList(int page, int limit, string keyword)
        {
            MyContext context = new MyContext();

            //var query = context.培训成绩表.AsQueryable();
            var query = /*context.月工资表.AsQueryable();*/
               from a in context.培训成绩表
               join b in context.培训课程表
               on a.课程号 equals b.课程号
               select new
               {
                   编号 = a.编号,
                   工号 = a.工号,
                   课程号 = b.课程号,
                   课程名 = b.课程名,
                   成绩 = a.成绩
               };
            string 工号 = this.User.Identity.Name;
            query = query.Where(u => u.工号 == 工号);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(u => u.课程号 == keyword);
            }
            var pageQuery = query.OrderBy(a => a.成绩).Skip(limit * (page - 1)).Take(limit).ToList();

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