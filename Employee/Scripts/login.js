$(function () {
    layui.use(['layer'], function () {
        var layer = layui.layer;
        $('#particles').particleground({
            dotColor: '#eee',
            lineColor: '#5cbdaa',
            directionX: 'left',
            directionY: 'center'
        });

        $('#btnLogin').click(function () {
            var name = $("#txtUserName").val();
            var pwd = $("#txtUserPwd").val();
            var code = $("#txtCode").val();
            var date1 = new Date();
            if (name == "") {
                $("#txtUserName").focus();
                layer.tips('  请输入用户名！  ', '#txtUserName', {
                    tips: [1, '#009688'],
                    time: 2000
                });
                return false;
            } else if (pwd == "") {
                $("#txtUserPwd").focus();
                layer.tips('  请输入密码！  ', '#txtUserPwd', {
                    tips: [1, '#009688'],
                    time: 2000
                });
                return false;
            } else if (code == "") {
                $("#txtCode").focus();
                layer.tips('  请输入验证码！  ', '#txtCode', {
                    tips: [1, '#009688'],
                    time: 2000
                });
                return false;
            }

            $.ajax({
                url: '/Home/Login',
                type: 'post',
                data: { loginName: name, pwd: pwd, code: code },
                beforeSend: function () {
                    $('#btnLogin').text('正在登录，请稍后……');
                },
                success: function (msg) {
                    var timediff = new Date().getTime() - date1.getTime();
                    if (msg == "1") {
                        $("#txtCode").focus();
                        layer.tips('  验证码有误，请重新输入！  ', '#txtCode', {
                            tips: [1, '#009688'],
                            time: 2000
                        });
                        setTimeout(function () {
                            $('#btnLogin').text('登 录');
                        }, timediff <= 800 ? 800 : 0)
                        changeCode();
                    } else if (msg == "2") {
                        $("#txtUserName").focus();
                        layer.tips('  用户名或密码有误，请重新输入！  ', '#txtUserName', {
                            tips: [1, '#009688'],
                            time: 2000
                        });
                        setTimeout(function () {
                            $('#btnLogin').text('登 录');
                        }, timediff <= 800 ? 800 : 0)
                        changeCode();
                    } else if (msg == "3") {
                        $("#txtUserName").focus();
                        layer.tips('  该账号已被禁用，请联系管理员！  ', '#txtUserName', {
                            tips: [1, '#009688'],
                            time: 2000
                        });
                        setTimeout(function () {
                            $('#btnLogin').text('登 录');
                        }, timediff <= 800 ? 800 : 0)
                        changeCode();
                    } else if (msg == "") {
                        $('#btnLogin').text('登录成功，正在跳转……');
                        setTimeout(function () {
                            window.location.href = '/Home/Default';
                        }, 800)
                        
                    } else {
                        setTimeout(function () {
                            $('#btnLogin').text('登 录');
                        }, timediff <= 800 ? 800 : 0)
                        layer.alert(msg, { title: '系统提示', icon: 2 });
                    }
                }
            })
        })
    })
})

function changeCode() {
    $("#txtCode").val("");
    $("#imgCode").attr("src", "/Home/VerifyCode?time=" + Math.random());
}

//回车键
document.onkeydown = function (e) {
    if (!e) e = window.event; //火狐中是 window.event
    if ((e.keyCode || e.which) == 13) {
        var btn = document.getElementById("btnLogin")
        btn.click();
    }
}