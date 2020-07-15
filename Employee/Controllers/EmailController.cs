using EF;
using Helper;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Employee.Controllers
{
    public class EmailController : Controller
    {
        MD5Encrypt md5encrypt = new MD5Encrypt();
        Email email = new Email();
        // GET: Eamil
        [HttpGet]
        [AllowAnonymous]
        public ActionResult SendView()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult SendEmail(string emailadd)
        {
        
            string code = md5encrypt.getMd5Hash(new Random().Next(1, 100000).ToString());
            email.SendEmail(emailadd, "找回密码", "https://localhost:44324/Email/ResetView?emailadd=1114068569@qq.com&code=" + code, null);
            using(MyContext context = new MyContext())
            {
                邮件验证表 add = new 邮件验证表();
                add.邮箱 = emailadd;
                add.验证码 = code;
                context.邮件验证表.Add(add);
                int flag = context.SaveChanges();
                if(flag > 0)
                {
                    return Json(new
                    {
                        Success = true,
                        Message = "发送成功"
                    });
                }
            }
            return Json(new
            {
                Success = true,
                Message = "发送失败"
            });
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetView(string emailadd, string code)
        {
            using(MyContext context = new MyContext())
            {
                邮件验证表 exist = new 邮件验证表();
                exist = context.邮件验证表.FirstOrDefault(u => u.邮箱 == emailadd && u.验证码 == code);
                ViewBag.emailadd = emailadd;
                if(exist != null)
                {
                    return View();
                }
            }
            return View("SendView");
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Reset(string pwd1, string pwd2)
        {
            using (MyContext context = new MyContext())
            {
                string emailadd = "1114068569@qq.com";
                if(pwd1 != pwd2)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "两次密码不相同"
                    });
                }
                员工表 emp = new 员工表();
                emp = context.员工表.FirstOrDefault(u => u.邮箱 == emailadd);
                用户表 user = new 用户表();
                user = context.用户表.FirstOrDefault(u => u.账号 == emp.工号);
                user.密码 = md5encrypt.getMd5Hash(pwd1);
                int flag = context.SaveChanges();
                if(flag > 0)
                {
                    return Json(new
                    {
                        Success = true,
                        Message = "修改成功"
                    });
                }
                return Json(new
                {
                    Success = false,
                    Message = "修改失败"
                });
            }
        }
    }
}