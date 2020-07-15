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
    public class WorkdayController : Controller
    {
        // GET: Attend/Workday
        public ActionResult WorkdayView()
        {
            return View();
        }

        public ActionResult GetList(int page, int limit)
        {
            MyContext context = new MyContext();
            var query = context.月应出勤天数表.AsQueryable();
            var pageQuery = query.OrderBy(a => a.月份).Skip(limit * (page - 1)).Take(limit).ToList();


            var result = new
            {
                code = 0,
                msg = "",
                count = query.Count(),
                data = pageQuery
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult WorkdayDetail(int id)
        {
            using (MyContext context = new MyContext())
            {

                月应出勤天数表 info = context.月应出勤天数表.FirstOrDefault(u => u.月份 == id);
                ViewBag.Info = Newtonsoft.Json.JsonConvert.SerializeObject(info);
                return View();
            }
        }
        public ActionResult Add(月应出勤天数表 adddata)
        {
            using (MyContext context = new MyContext())
            {
                if(adddata.天数 > 31)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "天数不合法"
                    });
                }
                月应出勤天数表 now = context.月应出勤天数表.FirstOrDefault(u => u.月份 == adddata.月份);
                now.天数 = adddata.天数;
                int flg = context.SaveChanges();
                if (flg > 0)
                {
                    return Json(new
                    {
                        Success = true,
                        Message = "操作成功"
                    });
                }
                return Json(new
                {
                    Success = false,
                    Message = "操作失败"
                });
            }

        }
    }
}