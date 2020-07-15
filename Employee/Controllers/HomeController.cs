using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using EF;
using Employee.BLL;
using Employee.Model;
using Helper;

namespace Employee.Controllers
{
    public class HomeController : Controller
    {
        MD5Encrypt md5encrypt = new MD5Encrypt();
        Email email = new Email();
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            if(HttpContext.Session["CurrentUser"] == null || !(HttpContext.Session["CurrentUser"] is 用户表))
            {
                Response.Redirect("~/Home/Sign");    
            }
            return View();
        }
        [AllowAnonymous]
        public ActionResult index1()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult index2()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Sign()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(string loginid, string loginpassword)
        {
            using(MyContext context = new MyContext())
            {
                用户表 user = context.用户表.FirstOrDefault(u => u.账号 == loginid);
                if (user == null)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "用户不存在"
                    });
                }
                if (!md5encrypt.verifyMd5Hash(loginpassword, user.密码))
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "密码错误"
                    });
                }
                HttpContext.Session["CurrentUser"] = user;
                HttpContext.Session.Timeout = 2;

                FormsAuthentication.SetAuthCookie(user.账号, false);
                string sql = "insert into 日志表 values(@账号, @操作, @日期)";
                context.Database.ExecuteSqlCommand(sql, new SqlParameter("@账号", user.账号), new SqlParameter("@操作", "登录"),new SqlParameter("@日期", DateTime.Now.ToString()));
                return Json(new
                {
                    Success = true,
                    Message = "验证成功"
                });
            }
        }

        
        public ActionResult Logout()
        {
            HttpContext.Session["CurrentUser"] = null;
            using (MyContext context = new MyContext())
            {
                string sql = "insert into 日志表 values(@账号, @操作, @日期)";
                context.Database.ExecuteSqlCommand(sql, new SqlParameter("@账号", this.User.Identity.Name), new SqlParameter("@操作", "退出"), new SqlParameter("@日期", DateTime.Now.ToString()));
            }
            FormsAuthentication.SignOut();
            return RedirectToAction("Sign", "Home");
        }
        public ActionResult log()
        {
            return View();
        }
        public ActionResult PersonDetail()
        {
            using (MyContext context = new MyContext())
            {
                string id = this.User.Identity.Name;
                员工视图 info = context.员工视图.FirstOrDefault(u => u.工号 == id);
                ViewBag.Info = Newtonsoft.Json.JsonConvert.SerializeObject(info);
                return View();
            }
        }
        
    }
}