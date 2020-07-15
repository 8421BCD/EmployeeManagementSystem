using EF;
using Employee.MyFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Employee.Areas.Attend.Controllers
{
    [AttendFilter]
    public class LeaveController : Controller
    {
        // GET: Attend/Leave
        public ActionResult LeaveView()
        {
            return View();
        }

        public ActionResult GetList(int page, int limit, string keyword)
        {
            MyContext context = new MyContext();
            var query = context.请假表.AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(u => u.工号.Contains(keyword));
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
        public ActionResult Agree(int id)
        {
            using (MyContext context = new MyContext())
            {
                请假表 now = context.请假表.FirstOrDefault(u => u.编号 == id);
                now.是否审批 = "是";
                if (context.SaveChanges() > 0)
                {
                    return Json(new
                    {
                        Success = true,
                        Message = "审批成功"
                    });
                }
                return Json(new
                {
                    Success = false,
                    Message = "审批失败"
                });
            }
        }
        public ActionResult Rollback(int id)
        {
            using (MyContext context = new MyContext())
            {
                请假表 now = context.请假表.FirstOrDefault(u => u.编号 == id);
                now.是否审批 = "否";
                if (context.SaveChanges() > 0)
                {
                    return Json(new
                    {
                        Success = true,
                        Message = "驳回成功"
                    });
                }
                return Json(new
                {
                    Success = false,
                    Message = "驳回失败"
                });
            }
        }
    }
}