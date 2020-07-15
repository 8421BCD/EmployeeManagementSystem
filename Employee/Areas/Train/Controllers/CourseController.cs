using EF;
using Employee.MyFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Employee.Areas.Train.Controllers
{
    [TrainFilter]
    public class CourseController : Controller
    {
        // GET: Train/Course
        public ActionResult CourseView()
        {
            return View();
        }

        public ActionResult GetList(int page, int limit, string keyword)
        {
            MyContext context = new MyContext();
            var query = context.培训课程表.AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(u => u.课程号.Contains(keyword));
            }
            var pageQuery = query.OrderBy(a => a.课程号).Skip(limit * (page - 1)).Take(limit).ToList();


            var result = new
            {
                code = 0,
                msg = "",
                count = query.Count(),
                data = pageQuery
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CourseDetail(string id)
        {
            if (id == null)
            {
                return View();
            }

            using (MyContext context = new MyContext())
            {

                培训课程表 info = context.培训课程表.FirstOrDefault(u => u.课程号 == id);
                ViewBag.Info = Newtonsoft.Json.JsonConvert.SerializeObject(info);
                return View();
            }
        }
        public ActionResult Add(培训课程表 adddata, int edit)
        {
            using (MyContext context = new MyContext())
            {
                if (edit == 1)
                {
                    培训课程表 now = new 培训课程表();
                    now = context.培训课程表.FirstOrDefault(u => u.课程号 == adddata.课程号);
                    now.课程名 = adddata.课程名;
                    now.授课老师编号 = adddata.授课老师编号;
                    now.课程学时 = adddata.课程学时;
                }
                else
                {
                    培训课程表 now = context.培训课程表.FirstOrDefault(u => u.课程号 == adddata.课程号);
                    if (now != null)
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "该课程号已存在"
                        });
                    }
                    context.培训课程表.Add(adddata);
                }
                老师表 老师 = context.老师表.FirstOrDefault(u => u.老师编号 == adddata.授课老师编号);
                if(老师 == null)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "该老师不存在"
                    });
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
        public ActionResult Del(string id)
        {
            using (MyContext context = new MyContext())
            {
                培训课程表 now = context.培训课程表.FirstOrDefault(u => u.课程号 == id);
                培训成绩表 check = context.培训成绩表.FirstOrDefault(u => u.课程号 == id);
                if(check != null)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "该课程已有人上，不可删除"
                    });
                }
                context.培训课程表.Remove(now);
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