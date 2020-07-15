using Employee.BLL;
using Employee.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EF;
using System.Web.Script.Serialization;
using Employee.MyFilter;

namespace Employee.Areas.Personnel.Controllers
{
    [PersonnelFilter]
    public class DepartmentController : Controller
    {
        public ActionResult DepartmentView()
        {
            return View();
        }

        public ActionResult GetList(int page, int limit, string keyword)
        {
            MyContext context = new MyContext();
            var query = context.部门表.AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(u => u.部门编号.Contains(keyword));
            }
            var pageQuery = query.OrderBy(a => a.部门编号).Skip(limit * (page - 1)).Take(limit).ToList();

            var result = new
            {
                code = 0,
                msg = "",
                count = query.Count(),
                data = pageQuery
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DepartmentDetail(string id = null)
        {
            if (id == null)
            {
                return View();
            }

            using (MyContext context = new MyContext())
            {

                部门表 info = context.部门表.FirstOrDefault(u => u.部门编号 == id);
                ViewBag.Info = Newtonsoft.Json.JsonConvert.SerializeObject(info);
                return View();
            }
        }
        public ActionResult Add(部门表 adddata, int edit)
        {
            using (MyContext context = new MyContext())
            {
                if (edit == 1)
                {
                    部门表 now = context.部门表.FirstOrDefault(u => u.部门编号 == adddata.部门编号);
                    now.部门编号 = adddata.部门编号;
                    now.部门名称 = adddata.部门名称;
                    now.备注 = adddata.备注;
                }
                else
                {
                    部门表 check = context.部门表.FirstOrDefault(u => u.部门编号 == adddata.部门编号);
                    if (check != null)
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "部门编号已存在"
                        });
                    }
                    context.部门表.Add(adddata);
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
                if(id == "001" || id == "002" || id == "003" || id == "004")
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "该部门不可删除"
                    });
                }
                员工表 tmp = context.员工表.FirstOrDefault(u => u.部门编号 == id);
                if (tmp != null)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "此部门有员工，不可删除"
                    });
                }
                部门表 now = context.部门表.FirstOrDefault(u => u.部门编号 == id);
                context.部门表.Remove(now);
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
        public ActionResult BatchDel(List<string> ids)
        {
            using (MyContext context = new MyContext())
            {
                int flag = 0;
                string idd = null;
                foreach (string id in ids)
                {
                    if (context.员工表.FirstOrDefault(u => u.部门编号 == id) != null || id == "001" || id == "002" || id == "003" || id == "004")
                    {
                        flag = 1;
                        idd = id;
                        break;
                    }
                }
                if (flag == 1)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "编号为" + idd + "的部门不可删除"
                    });
                }
                var delUsersQuery = context.部门表.Where(u => ids.Contains(u.部门编号));
                context.部门表.RemoveRange(delUsersQuery);
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