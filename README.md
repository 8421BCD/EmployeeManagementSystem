![img](https://raw.githubusercontent.com/8421BCD/CDN/master//img/20.png)



# Personnel and  Payroll Management System



## Development environment

### 1、IDE

Visual Studio 2019

### 2、Frame

ASP.NET MVC + LayUI

### 3、Database

Microsoft SQL Server Management Studio 18

## System function

### 1、Function

#### Personnel information management

##### 1）Function Descriptions

- Add, delete, query and modify the employee basic information record. For new employees, the system automatically assigns them a system account.
- Add, delete, query and modify the post dictionary. Ensure no one holds the job while deleting a job.
- Record all the information about the department and post changes of employees.

##### 2）Interface presentation

###### 1、Login

###### ![image-20200616145543741](https://raw.githubusercontent.com/8421BCD/CDN/master//img/image-20200616145543741.png)

###### 2、Employees' basic information![image-20200616125408068](https://raw.githubusercontent.com/8421BCD/CDN/master//img/20200717090417.png)

###### 3、Add and modify employees' basic information![image-20200616125524140](https://raw.githubusercontent.com/8421BCD/CDN/master//img/20200717090905.png)

![image-20200616125553769](https://raw.githubusercontent.com/8421BCD/CDN/master//img/20200717091031.png)

##### 4、code

***controller***

```c#
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
```

***View***

```html
@{
    Layout = "~/Views/Shared/_Layout_2.cshtml";
    ViewBag.Title = "员工管理";
}

<style>
    .layui-table-cell .layui-form-checkbox[lay-skin="primary"] {
        transform: translateY(20%);
    }
</style>


<div class="current-location">
    <span>当前位置：</span>
    <a><cite>系统管理</cite></a>
    | <a><cite>人事信息管理</cite></a>
    | <a><cite>员工信息</cite></a>
</div>

<div id="tablee" lay-filter="jsTable"></div>
<script type="text/html" id="topbar">

    <div class="layui-inline" id="searchKeywordf">
        <input type="text" autocomplete="off" id="keyword" placeholder="请输入要查询的工号..." class="layui-input">
    </div>
    <button class="layui-btn" data-type="search" id="search" lay-event="search">
        <i class="layui-icon layui-icon-search"></i>查询
    </button>
    <button class="layui-btn" data-type="add" id="add" lay-event="add">
        <i class="layui-icon layui-icon-add-1"></i>添加
    </button>
</script>
<script type="text/html" id="tbar">
    <a class="layui-btn layui-btn-xs" lay-event="Edit" title="编辑">
        <i class="layui-icon layui-icon-edit"></i>编辑
    </a>
</script>
<script type="text/html" id="image">
    <img src="{{d.照片}}" style="width:108px; height:100px;" />
</script>

<style type="text/css">
    .layui-table-cell {
        text-align: center;
        height: auto;
        white-space: normal;
    }
</style>

@section Scripts{
    <script type="text/javascript">
        layui.use(['table', 'layer', 'laydate', 'util'], function ()
        {
            var table = layui.table;
            var $ = layui.$;
            var layer = layui.layer;
            var laydate = layui.laydate;
            var util = layui.util;
            table.render({
                elem: '#tablee'
                , height: 510
                , where: $("#keyword").val()
                , url: "@Url.Action("GetList")"//数据接口
                , toolbar: "#topbar"
                , page: true //开启分页
                , cols: [[
                    { type: "checkbox", style: "height:110px;"},
                    {  align: 'center', width: 80, style: "height:110px;", templet: function (obj) {
                        return obj.LAY_INDEX;
                    }
                    },
                    //刘文涵
                    { field: '工号', align: 'center', title: '工号', width: 100, style: "height:110px;", sort: true },
                    { field: '照片', align: 'center', title: '照片', width: 130, align: "center", style: "height:110px;",templet:"#image"},
                    { field: '姓名', align: 'center', title: '姓名', width: 100, style: "height:110px;",sort: true },
                    { field: '性别', align: 'center', title: '性别', width: 100, style: "height:110px;", sort: true },
                    { field: '是否在职', align: 'center', title: '是否在职', width: 150, style: "height:110px;", sort: true },
                    {
                        field: '出生日期', align: 'center', title: '出生日期', width: 150, style: "height:110px;",sort: true,
                        templet: function (d) {
                            var val = d.出生日期;
                            var date = new Date(parseInt(val.replace("/Date(", "").replace(")/", ""), 10));
                            //月份为0-11，所以+1，月份小于10时补个0
                            var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
                            var currentDate = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
                            return date.getFullYear() + "-" + month + "-" + currentDate;
                        }
                    },
                    { field: '身份证号码', align: 'center', title: '身份证号码', width: 200, style: "height:110px;",sort: true },
                    { field: '职务名称', align: 'center', title: '职务', width: 200, style: "height:110px;",sort: true },
                    { field: '部门名称', align: 'center', title: '部门', width: 200, style: "height:110px;",sort: true },
                    { field: '学历', align: 'center', title: '学历', width: 200, style: "height:110px;",sort: true },
                    { field: '籍贯', align: 'center', title: '籍贯', width: 200, style: "height:110px;",sort: true },
                    { field: '婚姻状态', align: 'center', title: '婚姻状态', width: 200, style: "height:110px;",sort: true },
                    { field: '电话', align: 'center', title: '电话', width: 200, style: "height:110px;",sort: true },
                    { field: '邮箱', align: 'center', title: '邮箱', width: 200, style: "height:110px;",sort: true },
                    { field: '基本工资', align: 'center', title: '基本工资', width: 200, style: "height:110px;",sort: true },
                    { title: '操作', fixed: 'right', width: 100, style: "height:110px;",align: 'center', toolbar: '#tbar' }
                ]]
            });
            table.on("toolbar(jsTable)", function (obj)
            {
            switch (obj.event) {
                case 'search':
                    table.reload("tablee", {
                        page: { curr: 1 },
                        where: { keyword: $('#keyword').val() }
                    }, 'data');
                    break;
                case 'add':
                    layer.open({
                        type: 2,
                        content: '@Url.Action("PersonDetail")', //刘文涵
                        title: "新增",
                        area: ["700px", "450px"],
                        end: function () { // layui 关闭弹框时的回调函数
                            $("#search").click();
                        }
                    });
                    break;
            }
            }
            )
           table.on("tool(jsTable)", function (obj) {
                var event = obj.event;
                var data = obj.data;
                if (event == "Edit")
                {
                     layer.open({
                         type: 2,
                         content: '@Url.Action("PersonDetail")?id=' + data.工号,/*刘文涵*/
                       title: "修改",
                       area: ["700px", "450px"],
                       end: function () { // layui 关闭弹框时的回调函数
                                $("#search").click();
                            }
                })
                }
           })

     });
    </script>
}
```

#### Attendance management

##### **1）Function Descriptions**

- Maintain the daily overtime record of the stuff
- Examine and approve the requests for leave from employees
- Print the related report

##### **2）Interface presentation**

###### 1、Examine and approve the requests

![image-20200616130656382](https://raw.githubusercontent.com/8421BCD/CDN/master//img/5.png)

#### Salary management

##### **1）Function description**

- The formula of the monthly salary calculation is: real salary = attendance salary(basic salary * attendance rate) + post salary + rewards and punishment amount + overtime salary + pay-for-performance - withholding salary. And the formula of the attendance rate is: the attendance rate = the number of full attendance days / the number of working days * basic salary
- Maintain the employees' record such as rewards and punishment, pay-for-performance and withholding salary
- Calculate and back up the monthly wages.
- Print the related report

##### **2）Interface presentation**

###### 1、Query, calculate and back up the monthly wages

![image-20200616133028438](https://raw.githubusercontent.com/8421BCD/CDN/master//img/6.png)

![image-20200616131152814](https://raw.githubusercontent.com/8421BCD/CDN/master//img/7.png)

###### 2、Backups

![image-20200616131250777](https://raw.githubusercontent.com/8421BCD/CDN/master//img/8.png)

###### 3、Maintain the rewards and punishment salary

![image-20200616133227071](https://raw.githubusercontent.com/8421BCD/CDN/master//img/9.png)

![image-20200616133259278](https://raw.githubusercontent.com/8421BCD/CDN/master//img/10.png)

##### **4）code**

***The calculation of monthly wages***

```c#
public ActionResult MonthWageCalc()
        {
            using(MyContext context = new MyContext())
            {
                context.Database.ExecuteSqlCommand("exec 计算月工资");
                return Json(new
                {
                    Success = true,
                    Message = "计算完成"
                });
            }
        }
```

Back up

```c#
public ActionResult MonthWageCalc()
        {
            using(MyContext context = new MyContext())
            {
                context.Database.ExecuteSqlCommand("exec 计算月工资");
                return Json(new
                {
                    Success = true,
                    Message = "计算完成"
                });
            }
        }
```



#### Training management

##### **1）Function descriptions**

- Maintain the information of training cource
- Maintain the information of training score
- Maintain the information of teachers

##### 3）Interface presentation

###### Training cources

![image-20200616150428094](https://raw.githubusercontent.com/8421BCD/CDN/master//img/11.png)

#### Self-service

##### **1）Function description**

- Change user's password
- Retrieve password using mailbox
- Clock in
- Apply for a leave: user submits the form and waits for review.
- Query personal information including wages, attendance, training and so on

##### **2）Interface presentation**

###### Ask for leave

![image-20200616131444695](https://raw.githubusercontent.com/8421BCD/CDN/master//img/12.png)

###### Change password

![image-20200616131643995](https://raw.githubusercontent.com/8421BCD/CDN/master//img/13.png)

###### Retrieve password

![image-20200616131738449](https://raw.githubusercontent.com/8421BCD/CDN/master//img/14.png)

![image-20200616131907673](https://raw.githubusercontent.com/8421BCD/CDN/master//img/20200717091644.png)

##### 4）code

```c#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class Email
    {
        public string SendEmail(string sto, string sToSubject, string sContent, string sFilePath)
        {
            string sRestring = "";
            try
            {
                //===========================正常邮件发布===========================================

                string sSmtp = "smtp.qq.com";
                string sPort = "25";
                string sFrom = "网站客服 <1114068569@qq.com>";
                string sAccount = "1114068569@qq.com";
                string sPass = "ruoqphwfnulpgcca";//这个密码切记一定要qq邮箱里面生成的授权码！授权码 ！授权码 ！

                System.Net.Mail.SmtpClient client = new SmtpClient();
                client.Host = sSmtp;
                client.UseDefaultCredentials = false;
                client.Port = Convert.ToInt16(sPort);
                client.Credentials = new System.Net.NetworkCredential(sAccount, sPass);

                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                System.Net.Mail.MailMessage message = new MailMessage(sFrom, sto);
                message.Subject = sToSubject;
                message.Body = sContent;
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.IsBodyHtml = true;
                ///message.EnableSsl = false;

                //添加附件
                if (!string.IsNullOrEmpty(sFilePath))
                {
                    Attachment data = new Attachment(sFilePath, System.Net.Mime.MediaTypeNames.Application.Octet);
                    message.Attachments.Add(data);
                }

                client.Send(message);

                //==================================End邮件发布===========================

                sRestring = "ok";
            }
            catch (Exception ex)
            {
                sRestring = "failed," + ex.Message.ToString();
            }
            return sRestring;
        }

    }
}

```

```c#
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    /// <summary>
    /// 不可逆加密，限于字母和数字
    /// </summary>
    public class MD5Encrypt
    {
        public string getMd5Hash(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
        public bool verifyMd5Hash(string input, string hash)
        {
            // Hash the input.
            string hashOfInput = getMd5Hash(input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

```



#### User management

##### **1）Function descriptions**

- Add, delete the account and modify user's permission.

##### **2）Interface presentation**

###### 1、User management

![image-20200616132014348](https://raw.githubusercontent.com/8421BCD/CDN/master//img/17.png)
