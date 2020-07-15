using EF;
using Employee.MyFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Employee.Areas.Wage.Controllers
{
    [WageFilter]
    public class MonthWageController : Controller
    {
        // GET: Wage/MonthWage
        public ActionResult MonthWageView()
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
        public ActionResult GetList(int page, int limit, string keyword = null, string 部门名称 = null)
        {
            MyContext context = new MyContext();

            var query = /*context.月工资表.AsQueryable();*/
                from a in context.月工资表
                join b in context.员工视图
                on a.工号 equals b.工号
                select new
                {
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
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(u => u.工号.Contains(keyword));
            }
            if (!string.IsNullOrWhiteSpace(部门名称))
            {
                query = query.Where(u => u.部门名称 == 部门名称);
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
        public ActionResult MonthWageCalc()
        {
            using(MyContext context = new MyContext())
            {
                context.Database.ExecuteSqlCommand("exec 计算月工资");
                return Json(new
                {
                    Success = true,
                    Message = "计算完成"
                });
            }
        }
        public ActionResult Backup()
        {
            using (MyContext context = new MyContext())
            {
                context.Database.ExecuteSqlCommand("exec 月工资备份");
                return Json(new
                {
                    Success = true,
                    Message = "备份完成"
                });
            }

        }

    }
}