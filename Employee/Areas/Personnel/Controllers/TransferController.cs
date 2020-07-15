using EF;
using Employee.MyFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Employee.Areas.Personnel.Controllers
{
    [PersonnelFilter]
    public class TransferController : Controller
    {
        // GET: Personnel/Transfer
        public ActionResult TransferView()
        {
            return View();
        }
        public ActionResult TransferDetail()
        {
            return View();
        }
        public ActionResult GetList(int page, int limit, string keyword)
        {
            MyContext context = new MyContext();
            var query = context.员工调动视图.AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(u => u.工号.Contains(keyword));
            }
            var pageQuery = query.OrderBy(a => a.调动日期).Skip(limit * (page - 1)).Take(limit).ToList();


            var result = new
            {
                code = 0,
                msg = "",
                count = query.Count(),
                data = pageQuery
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Add(string id, string posname, string depname)
        {
            using (MyContext context = new MyContext())
            {
                员工表 emp = context.员工表.FirstOrDefault(u => u.工号 == id);
                string posid = context.职务表.FirstOrDefault(u => u.职务名称 == posname).职务编号;
                string depid = context.部门表.FirstOrDefault(u => u.部门名称 == depname).部门编号;

                emp.职务编号 = posid;
                emp.部门编号 = depid;
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
                    Message = "未发生调动"
                });
            }

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
        public ActionResult GetPosList()
        {
            using (MyContext context = new MyContext())
            {
                var deps = from p in context.职务表 select p;

                List<职务表> list = new List<职务表>();
                foreach (var dep in deps)
                {
                    list.Add(dep);
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }
    }
}