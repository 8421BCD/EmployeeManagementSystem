using EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Employee.Areas.Common.Controllers
{
    public class LeaveController : Controller
    {
        // GET: Common/Leave
        public ActionResult LeaveView()
        {
            return View();
        }
        public ActionResult LeaveDetail(int? id)
        {
            if (id == null)
            {
                return View();
            }

            using (MyContext context = new MyContext())
            {

                请假表 info = context.请假表.FirstOrDefault(u => u.编号 == id);
                ViewBag.Info = Newtonsoft.Json.JsonConvert.SerializeObject(info);
                return View();
            }
        }
        public ActionResult GetList(int page, int limit)
        {
            MyContext context = new MyContext();
            var query = context.请假表.AsQueryable();
            string id = this.User.Identity.Name;
            query = query.Where(u => u.工号 == id);
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
        public ActionResult Add(请假表 adddata, int edit)
        {
            using (MyContext context = new MyContext())
            {
                adddata.工号 = this.User.Identity.Name;
                if (edit == 1)
                {
                    请假表 now = new 请假表();
                    now = context.请假表.FirstOrDefault(u => u.编号 == adddata.编号);
                    now.请假日期 = adddata.请假日期;
                    now.请假天数 = adddata.请假天数;
                    now.请假原因 = adddata.请假原因;
                    now.是否审批 = "否";
                }
                else
                {
                    员工表 now = context.员工表.FirstOrDefault(u => u.工号 == adddata.工号);
                    if (now == null)
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "该员工不存在"
                        });
                    }
                    adddata.是否审批 = "否";
                    context.请假表.Add(adddata);
                }
                int flg = context.SaveChanges();
                if (flg > 0 || edit == 1)
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
        public ActionResult Del(int id)
        {
            using (MyContext context = new MyContext())
            {
                请假表 now = context.请假表.FirstOrDefault(u => u.编号 == id);
                context.请假表.Remove(now);
                if (context.SaveChanges() > 0)
                {
                    return Json(new
                    {
                        Success = true,
                        Message = "删除成功"
                    });
                }
                return Json(new
                {
                    Success = false,
                    Message = "删除失败"
                });
            }
        }
    }
}