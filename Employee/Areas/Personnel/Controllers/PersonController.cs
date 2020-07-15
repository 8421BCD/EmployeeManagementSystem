using Employee.BLL;
using Employee.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EF;
using System.Web.Script.Serialization;
using System.IO;
using Employee.MyFilter;
using Helper;
using System.Net.Mail;

namespace Employee.Areas.Personnel.Controllers
{
    [PersonnelFilter]
    public class PersonController : Controller
    {
        // GET: Personnel/Person
        public ActionResult PersonView()
        {
            return View();
        }

        public ActionResult Upload()
        {
            try
            {
                HttpFileCollectionBase files = Request.Files;
                HttpPostedFileBase file = files[0];
                //获取文件名后缀
                string extName = Path.GetExtension(file.FileName).ToLower();
                string path = Server.MapPath("/Img/");//path为某个文件夹的绝对路径，不要直接保存到数据库
                                                          //    string path = "F:\\TgeoSmart\\Image\\";
                                                          //生成新文件的名称，guid保证某一时刻内图片名唯一（文件不会被覆盖）
                string fileNewName = Guid.NewGuid().ToString();
                string ImageUrl = path + fileNewName + extName;
                //SaveAs将文件保存到指定文件夹中
                file.SaveAs(ImageUrl);
                //此路径为相对路径，只有把相对路径保存到数据库中图片才能正确显示（不加~为相对路径）
                //https://localhost:44324
                string url = "/Img/" + fileNewName + extName;
                return Json(new
                {
                    Result = true,
                    Data = url
                });
            }
            catch (Exception exception)
            {
                return Json(new
                {
                    Result = false,
                    exception.Message
                });
            }
        }
        public ActionResult GetList(int page, int limit, string keyword)
        {
            MyContext context = new MyContext();
            var query = context.员工视图.AsQueryable();
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

        public ActionResult PersonDetail(string id = null)
        {
            if (id == null)
            {
                return View();
            }
            using (MyContext context = new MyContext())
            {
                员工视图 info = context.员工视图.FirstOrDefault(u => u.工号 == id);
                ViewBag.Info = Newtonsoft.Json.JsonConvert.SerializeObject(info);
                return View();
            }
        }
        public ActionResult Add(员工视图 adddata, int edit)
        {
            using (MyContext context = new MyContext())
            {
                if (edit == 1)
                {
                    Console.Write(adddata);
                    员工表 now = context.员工表.FirstOrDefault(u => u.工号 == adddata.工号);
                    部门表 dep = context.部门表.FirstOrDefault(u => u.部门名称 == adddata.部门名称);
                    职务表 pos = context.职务表.FirstOrDefault(u => u.职务名称 == adddata.职务名称);
                    string depid = dep.部门编号;
                    string posid = pos.职务编号;
                    now.工号 = adddata.工号;
                    now.性别 = adddata.性别;
                    now.出生日期 = adddata.出生日期;
                    now.身份证号码 = adddata.身份证号码;
                    now.职务编号 = posid;
                    now.部门编号 = depid;
                    now.学历 = adddata.学历;
                    now.籍贯 = adddata.籍贯;
                    now.婚姻状态 = adddata.婚姻状态;
                    now.电话 = adddata.电话;
                    now.邮箱 = adddata.邮箱;
                    now.是否在职 = adddata.是否在职;
                    now.基本工资 = adddata.基本工资;
                    now.照片 = adddata.照片;
                }
                else
                {
                    员工表 check = context.员工表.FirstOrDefault(u => u.工号 == adddata.工号);
                    if (check != null)
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "工号已存在"
                        });
                    }
                    员工表 now = new 员工表();
                    部门表 dep = context.部门表.FirstOrDefault(u => u.部门名称 == adddata.部门名称);
                    职务表 pos = context.职务表.FirstOrDefault(u => u.职务名称 == adddata.职务名称);
                    string depid = dep.部门编号;
                    string posid = pos.职务编号;
                    now.工号 = adddata.工号;
                    now.姓名 = adddata.姓名;
                    now.性别 = adddata.性别;
                    now.出生日期 = adddata.出生日期;
                    now.身份证号码 = adddata.身份证号码;
                    now.职务编号 = posid;
                    now.部门编号 = depid;
                    now.学历 = adddata.学历;
                    now.籍贯 = adddata.籍贯;
                    now.婚姻状态 = adddata.婚姻状态;
                    now.电话 = adddata.电话;
                    now.邮箱 = adddata.邮箱;
                    now.是否在职 = adddata.是否在职;
                    now.基本工资 = adddata.基本工资;
                    now.照片 = adddata.照片;
                    context.员工表.Add(now);

                    用户表 user = new 用户表();
                    user.账号 = now.工号;
                    user.密码 = new MD5Encrypt().getMd5Hash(now.工号);
                    user.权限类型 = 0;
                    context.用户表.Add(user);
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
                员工表 now = context.员工表.FirstOrDefault(u => u.工号 == id);
                now.是否在职 = "否";
                if (context.SaveChanges() > 0)
                {
                    return Json(new
                    {
                        Success = true,
                        Message = "开除成功"
                    });
                }
                return Json(new
                {
                    Success = false,
                    Message = "开除失败"
                });
            }
        }
        public ActionResult GetDepList()
        {
            using (MyContext context = new MyContext())
            {
                var deps = from p in context.部门表 select p;

                List<部门表> list = new List<部门表>();
                foreach(var dep in deps)
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
        //public List<职务表> GetDepList()
        //{
        //    using (MyContext context = new MyContext())
        //    {
        //        var deps = context.部门表;
        //        List<部门表> list = new List<部门表>();
        //        foreach (var dep in deps)
        //        {
        //            list.Append(dep);
        //        }
        //        return list;
        //    }
        //}
        //public ActionResult BatchDel(List<string> ids)
        //{
        //    using (MyContext context = new MyContext())
        //    {
        //        int flag = 0;
        //        string idd = null;
        //        foreach (string id in ids)
        //        {
        //            if (context.员工表.FirstOrDefault(u => u.工号 == id) != null)
        //            {
        //                flag = 1;
        //                idd = id;
        //                break;
        //            }
        //        }
        //        if (flag == 1)
        //        {
        //            return Json(new
        //            {
        //                Success = false,
        //                Message = "编号为" + idd + "的职务已任职，不可删除"
        //            });
        //        }
        //        var delUsersQuery = context.员工表.Where(u => ids.Contains(u.工号));
        //        context.员工表.RemoveRange(delUsersQuery);
        //        if (context.SaveChanges() > 0)
        //        {
        //            return Json(new
        //            {
        //                Success = true,
        //                Message = "删除成功"
        //            });
        //        }

        //        return Json(new
        //        {
        //            Success = false,
        //            Message = "删除失败"
        //        });
        //    }
        //}
    }
}