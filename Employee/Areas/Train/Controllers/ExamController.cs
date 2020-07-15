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
    public class ExamController : Controller
    {
        // GET: Train/Exam
        public ActionResult ExamView()
        {
            return View();
        }

        public ActionResult GetList(int page, int limit, string keyword)
        {
            MyContext context = new MyContext();
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


            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(u => u.工号.Contains(keyword));
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

        public ActionResult ExamDetail(int ?id)
        {
            if (id == null)
            {
                return View();
            }

            using (MyContext context = new MyContext())
            {

                培训成绩表 info = context.培训成绩表.FirstOrDefault(u => u.编号 == id);
                ViewBag.Info = Newtonsoft.Json.JsonConvert.SerializeObject(info);
                return View();
            }
        }
        public ActionResult Add(培训成绩表 adddata, int edit)
        {
            using (MyContext context = new MyContext())
            {
                培训课程表 课程 = context.培训课程表.FirstOrDefault(u => u.课程号 == adddata.课程号);
                if (课程 == null)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "课程不存在"
                    });
                }
                if (edit == 1)
                {
                    培训成绩表 now = new 培训成绩表();
                    now = context.培训成绩表.FirstOrDefault(u => u.编号 == adddata.编号);
                    now.课程号 = adddata.课程号;
                    now.成绩 = adddata.成绩;
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
                    context.培训成绩表.Add(adddata);
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
                培训成绩表 now = context.培训成绩表.FirstOrDefault(u => u.编号 == id);
                context.培训成绩表.Remove(now);
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
        public ActionResult BatchDel(List<int> ids)
        {
            using (MyContext context = new MyContext())
            {
                var delUsersQuery = context.培训成绩表.Where(u => ids.Contains(u.编号));
                context.培训成绩表.RemoveRange(delUsersQuery);
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