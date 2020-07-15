using EF;
using Employee.MyFilter;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Employee.Areas.Attend.Controllers
{
    public class PunchController : Controller
    {
        // GET: Attend/Punch
        [AttendFilter]
        public ActionResult PunchView()
        {
            return View();
        }
        public ActionResult Punchcard()
        {
            string id = this.User.Identity.Name;
            using (MyContext context = new MyContext())
            {
                string sql = "exec 打卡 @id, @time";
                int cnt = context.Database.ExecuteSqlCommand(sql, new SqlParameter("@id", id), new SqlParameter("@time", DateTime.Now.ToString()));
                if(cnt > 0)
                {
                    return Json(new
                    {
                        Success = true,
                        Message = "打卡成功"
                    });
                }
                return Json(new
                {
                    Success = false,
                    Message = "打卡失败"
                });
            }
        }
        [AttendFilter]
        public ActionResult GetList(int page, int limit, string keyword, string 起始日期, string 结束日期)
        {
            MyContext context = new MyContext();
            var query = context.每日打卡表.AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(u => u.工号.Contains(keyword));
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
            var pageQuery = query.OrderBy(a => a.编号).Skip(limit * (page - 1)).Take(limit).ToList();


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