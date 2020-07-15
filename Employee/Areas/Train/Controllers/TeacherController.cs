using EF;
using Employee.MyFilter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Employee.Areas.Train.Controllers
{
    [TrainFilter]
    public class TeacherController : Controller
    {
        // GET: Train/Teacher
        public ActionResult TeacherView()
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
                string url = "https://localhost:44324/Img/" + fileNewName + extName;
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
            var query = context.老师表.AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(u => u.老师编号.Contains(keyword));
            }
            var pageQuery = query.OrderBy(a => a.老师编号).Skip(limit * (page - 1)).Take(limit).ToList();

            var result = new
            {
                code = 0,
                msg = "",
                count = query.Count(),
                data = pageQuery
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TeacherDetail(string id = null)
        {
            if (id == null)
            {
                return View();
            }

            using (MyContext context = new MyContext())
            {

                老师表 info = context.老师表.FirstOrDefault(u => u.老师编号 == id);
                ViewBag.Info = Newtonsoft.Json.JsonConvert.SerializeObject(info);
                return View();
            }
        }
        public ActionResult Add(老师表 adddata, int edit)
        {
            using (MyContext context = new MyContext())
            {
                if (edit == 1)
                {
                    老师表 now = context.老师表.FirstOrDefault(u => u.老师编号 == adddata.老师编号);
                    now.老师编号 = adddata.老师编号;
                    now.老师姓名 = adddata.老师姓名;
                    now.照片 = adddata.照片;
                }
                else
                {
                    老师表 check = context.老师表.FirstOrDefault(u => u.老师编号 == adddata.老师编号);
                    if (check != null)
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "老师编号已存在"
                        });
                    }
                    context.老师表.Add(adddata);
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
    }
}