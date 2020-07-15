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
    public class PositionController : Controller
    {
        // GET: Personnel/Position
        public ActionResult PositionView()
        {
            return View();
        }

        public ActionResult GetList(int page, int limit, string keyword)
        {
                MyContext context = new MyContext();
                var query = context.职务表.AsQueryable();
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    keyword = keyword.Trim();
                    query = query.Where(u => u.职务编号.Contains(keyword));
                }
                var pageQuery = query.OrderBy(a => a.职务编号).Skip(limit * (page - 1)).Take(limit).ToList();


                var result = new
                {
                    code = 0,
                    msg = "",
                    count = query.Count(),
                    data = pageQuery
                };
                return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult PositionDetail(string id = null)
        {
            if (id == null)
            {
                return View();
            }

            using (MyContext context = new MyContext())
            {

                职务表 info = context.职务表.FirstOrDefault(u => u.职务编号 == id);
                ViewBag.Info = Newtonsoft.Json.JsonConvert.SerializeObject(info);
                return View();
            }
        }
        public ActionResult Add(职务表 adddata, int edit)
        {
            using (MyContext context = new MyContext())
            {
                if(edit == 1)
                {
                    职务表 now = context.职务表.FirstOrDefault(u => u.职务编号 == adddata.职务编号);
                    now.职务编号 = adddata.职务编号;
                    now.职务名称 = adddata.职务名称;
                    now.职务工资 = adddata.职务工资;
                    now.备注 = adddata.备注;
                }
                else
                {
                    职务表 check = context.职务表.FirstOrDefault(u => u.职务编号 == adddata.职务编号);
                    if (check != null)
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "职务编号已存在"
                        });
                    }
                    context.职务表.Add(adddata);
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
            using(MyContext context = new MyContext())
            {
                员工表 tmp = context.员工表.FirstOrDefault(u => u.职务编号 == id);
                if(tmp != null)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "此职务已任职，不可删除"
                    });
                }
                职务表 now = context.职务表.FirstOrDefault(u => u.职务编号 == id);
                context.职务表.Remove(now);
                if(context.SaveChanges() > 0)
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
                    if(context.员工表.FirstOrDefault(u => u.职务编号 == id) != null)
                    {
                        flag = 1;
                        idd = id;
                        break;
                    }
                }
                if(flag == 1)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "编号为" + idd +　"的职务已任职，不可删除"
                    });
                }
                var delUsersQuery = context.职务表.Where(u => ids.Contains(u.职务编号));
                context.职务表.RemoveRange(delUsersQuery);
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