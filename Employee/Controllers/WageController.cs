using EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Employee.Controllers
{
    public class WageController : Controller
    {
        // GET: Wage
        public ActionResult WageView()
        {
            return View();
        }
        public ActionResult GetDepList()
        {
            using (MyContext context = new MyContext())
            {
                var deps = from p in context.部门表 select p;

                List<部门表> list = new List<部门表>();
                foreach (var dep in deps)
                {
                    list.Add(dep);
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetList(int page, int limit, string 部门名称, string 起始日期, string 结束日期)
        {
            MyContext context = new MyContext();

            var query =
                from a in context.累计工资表
                join b in context.员工视图
                on a.工号 equals b.工号
                select new
                {
                    日期 = a.日期,
                    工号 = b.工号,
                    姓名 = b.姓名,
                    部门名称 = b.部门名称,
                    月出勤工资 = a.月出勤工资,
                    职务工资 = a.职务工资,
                    奖惩金额 = a.奖惩金额,
                    加班费 = a.加班费,
                    绩效工资 = a.绩效工资,
                    代扣款 = a.代扣款,
                    实发工资 = a.实发工资
                };
            string 工号 = this.User.Identity.Name;
            query = query.Where(u => u.工号 == 工号);
            if (!string.IsNullOrWhiteSpace(部门名称))
            {
                query = query.Where(u => u.部门名称 == 部门名称);
            }

            if (!string.IsNullOrWhiteSpace(起始日期))
            {
                DateTime start = Convert.ToDateTime(起始日期);
                query = query.Where(u => u.日期 >= start);
            }
            if (!string.IsNullOrWhiteSpace(结束日期))
            {
                DateTime end = Convert.ToDateTime(结束日期);
                query = query.Where(u => u.日期 <= end);
            }

            var pageQuery = query.OrderBy(a => a.工号).Skip(limit * (page - 1)).Take(limit).ToList();


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