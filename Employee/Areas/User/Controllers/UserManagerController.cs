using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Employee.Model;
using Employee.BLL;
using EF;
using Employee.MyFilter;
using Helper;
using System.Data.Entity.Validation;

namespace Employee.Areas.User.Controllers
{
    public class UserManagerController : Controller
    {
        UserManager usermanager = new UserManager();
        MD5Encrypt md5encrypt = new MD5Encrypt();
        // GET: User/UserManager
        [UserFilter]
        public ActionResult UserView()
        {
            return View();
        }
        [UserFilter]
        public ActionResult GetList(int page, int limit, string keyword)
        {
            MyContext context = new MyContext();
            var query = context.用户表.AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(u => u.账号.Contains(keyword));
            }
            var pageQuery = query.OrderBy(a => a.账号).Skip(limit * (page - 1)).Take(limit).ToList();


            var result = new
            {
                code = 0,
                msg = "",
                count = query.Count(),
                data = pageQuery
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [UserFilter]
        public ActionResult UserDetail(string id = null)
        {
            if (id == null)
            {
                return View();
            }
            using (MyContext context = new MyContext())
            {
                用户表 info = context.用户表.FirstOrDefault(u => u.账号 == id);
                ViewBag.Info = Newtonsoft.Json.JsonConvert.SerializeObject(info);
                return View();
            }
        }
        public ActionResult PasswordDetail(string id = null)
        {
            return View();
        }
        public ActionResult Changepwd(string 原密码, string 新密码, string 确认新密码)
        {
            using(MyContext context = new MyContext())
            {
                string id = this.User.Identity.Name;
                用户表 user = context.用户表.FirstOrDefault(u => u.账号 == id);
                if(!md5encrypt.verifyMd5Hash(原密码, user.密码))
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "密码错误"
                    });
                }
                if(新密码 != 确认新密码)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "两次密码输入不一致"
                    });
                }
                user.密码 = md5encrypt.getMd5Hash(新密码);
                try
                {
                    int flag = context.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    throw dbEx;
                }
                return Json(new
                {
                    Success = true,
                    Message = "修改成功"
                });

            }
        }
        [UserFilter]
        public ActionResult Add(用户表 adddata, int edit)
        {
            using (MyContext context = new MyContext())
            {
                if(adddata.权限类型 < 0 ||adddata.权限类型 > 2)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "权限类型必须为0-2的整数"
                    });
                }
                if (edit == 1)
                {
                    用户表 now = context.用户表.FirstOrDefault(u => u.账号 == adddata.账号);
                    now.账号 = adddata.账号;
                    now.密码 = adddata.密码;
                    now.权限类型 = adddata.权限类型;
                }
                else
                {
                    用户表 now = context.用户表.FirstOrDefault(u => u.账号 == adddata.账号);
                    员工表 person = context.员工表.FirstOrDefault(u => u.工号 == adddata.账号);
                    if (now != null)
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "账号已存在"
                        });
                    }
                    if(person == null)
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "无法为不存在的员工创建账号"
                        });
                    }
                    adddata.密码 = new MD5Encrypt().getMd5Hash(adddata.密码);
                    context.用户表.Add(adddata);
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
        [UserFilter]
        public ActionResult Del(string id)
        {
            using (MyContext context = new MyContext())
            {
                用户表 now = context.用户表.FirstOrDefault(u => u.账号 == id);
                context.用户表.Remove(now);
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