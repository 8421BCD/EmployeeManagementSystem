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
    public class PrizeController : Controller
    {
        // GET: Wage/Prize
        public ActionResult PrizeView()
        {
            return View();
        }

        public ActionResult GetList(int page, int limit, string keyword)
        {
            MyContext context = new MyContext();
            var query = context.奖惩记录表.AsQueryable();
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

        public ActionResult PrizeDetail(int ?id)
        {
            if (id == null)
            {
                return View();
            }

            using (MyContext context = new MyContext())
            {

                奖惩记录表 info = context.奖惩记录表.FirstOrDefault(u => u.编号 == id);
                ViewBag.Info = Newtonsoft.Json.JsonConvert.SerializeObject(info);
                return View();
            }
        }
        public ActionResult Add(奖惩记录表 adddata, int edit)
        {
            using (MyContext context = new MyContext())
            {
                DateTime start = context.公司信息表.FirstOrDefault().工资结算日期;
                DateTime end = DateTime.Now;
                if (adddata.日期 < start)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "日期非法！不可操作已核算完成且备份的工资记录"
                    });
                }
                if (adddata.日期 > end)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "日期不可晚于当前日期！"
                    });
                }
                if (edit == 1)
                {
                    奖惩记录表 now = new 奖惩记录表();
                    now = context.奖惩记录表.FirstOrDefault(u => u.编号 == adddata.编号);
                    now.日期 = adddata.日期;
                    now.奖惩事由 = adddata.奖惩事由;
                    now.奖惩金额 = adddata.奖惩金额;
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
                    context.奖惩记录表.Add(adddata);
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
                奖惩记录表 now = context.奖惩记录表.FirstOrDefault(u => u.编号 == id);
                context.奖惩记录表.Remove(now);
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
                var delUsersQuery = context.奖惩记录表.Where(u => ids.Contains(u.编号));
                context.奖惩记录表.RemoveRange(delUsersQuery);
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