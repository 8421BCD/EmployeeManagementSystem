layui.use(['element', 'layer', 'form', 'table'], function () {
    var $ = layui.$;
    var layer = layui.layer;
    var element = layui.element;
    var form = layui.form;

    var f = '';

    element.on('nav(menu)', function (elem) {
        var url = elem.attr('lay-href');
        if (url && f != url) {
            f = url;
            $('.layui-body').removeClass('main-body');
            //$('.layui-body').load(url);
            com.load(url);
        }
    });
    element.on('nav(left-nav)', function (elem) {
        flexLeftNav(2);
        var id = elem.attr('lay-key');
        getMenu(id);
    });
    element.on('nav(right-nav)', function (elem) {
        var e = elem.attr('lay-event');
        if (e == 'userInfo') {
            $.get('/Users/UserInfo', function (rtnStr) {
                layer.open({
                    type: 1,
                    title: '个人资料',
                    area: ['900px', '500px'],
                    content: rtnStr,
                    btn: ['保存', '关闭'],
                    success: function (layero, index) {
                        form.on('submit(btn-submit)', function (obj) {
                            com.getAjax('/Users/Edit', obj.field, function (data) {
                                if (data == '') {
                                    layer.close(index);
                                    layer.msg(com.saveSuccess, { time: 1000 });
                                }
                                else {
                                    layer.alert(data, { title: '系统提示', icon: 2 });
                                }
                            })
                        });
                    },
                    yes: function (index, layero) {
                        layero.find('#btn-submit').click();
                    }
                });
            })
        } else if (e == 'updatePwd') {
            $.get('/Users/UpdatePass', function (rtnStr) {
                layer.open({
                    type: 1,
                    title: '修改密码',
                    area: ['600px', '300px'],
                    content: rtnStr,
                    btn: ['保存', '关闭'],
                    success: function (layero, index) {
                        form.on('submit(btn-submit)', function (obj) {
                            com.getAjax('/Users/UpdatePass', obj.field, function (data) {
                                if (data == '') {
                                    layer.close(index);
                                    layer.msg(com.saveSuccess, { time: 1000 });
                                }
                                else {
                                    layer.alert(data, { title: '系统提示', icon: 2 });
                                }
                            })
                        });
                    },
                    yes: function (index, layero) {
                        layero.find('#btn-submit').click();
                    }
                });
            })
        } else if (e == "logout") {
            layer.confirm("您确定要退出本次登录吗?", { title: '系统提示', icon: 3 }, function (index) {
                location.href = "/Home/Logout";
            });
        } else if (e == 'theme') {
            $.get('/Home/Theme', function (rtnStr) {
                layer.open({
                    type: 1,
                    title: '主题',
                    content: rtnStr,
                    offset: 'rb',
                    shadeClose: true,
                    title: false,
                    closeBtn: false,
                    skin: "layui-anim layui-anim-rl layui-layer-right",
                    anim: -1
                });
            })
        }
    });
    $('.layui-flex a').click(function () {
        flexLeftNav(1);
    })

    initChart();
    function getMenu(id) {
        $.post("/Home/GetLeftMenu", { parentID: id }, function (data) {
            $('#menu').html(data);
            element.render('nav(menu)');
        })
    }
    function flexLeftNav(t) {
        var cls = $('.layui-flex i').attr('class');
        if (t == 1) {
            if (cls.indexOf('layui-icon-shrink-right') > 0) {
                $('.layui-side').animate({ left: "-200px" });
                $('.layui-body').animate({ left: "0px" }, function () {
                    $('.layui-flex i').attr('class', 'layui-icon layui-icon-spread-left');
                    $('.layui-flex a').attr('title', '展开左侧菜单');
                    if ($('#tb').length > 0) {
                        layui.table.resize('tb');
                    }
                    if ($('#chart1').length > 0) {
                        echarts.init(document.getElementById('chart1')).resize();
                        echarts.init(document.getElementById('chart2')).resize();
                    }
                });
            } else {
                $('.layui-side').animate({ left: "0px" });
                $('.layui-body').animate({ left: "200px" }, function () {
                    $('.layui-flex i').attr('class', 'layui-icon layui-icon-shrink-right');
                    $('.layui-flex a').attr('title', '折叠左侧菜单');
                    if ($('#tb').length > 0) {
                        layui.table.resize('tb');
                    }
                    if ($('#chart1').length > 0) {
                        echarts.init(document.getElementById('chart1')).resize();
                        echarts.init(document.getElementById('chart2')).resize();
                    }
                });
            }
        }
        else {
            if (cls.indexOf('layui-icon-spread-left') > 0) {
                $('.layui-side').animate({ left: "0px" });
                $('.layui-body').animate({ left: "200px" }, function () {
                    $('.layui-flex i').attr('class', 'layui-icon layui-icon-shrink-right');
                    $('.layui-flex a').attr('title', '折叠左侧菜单');
                    if ($('#tb').length > 0) {
                        layui.table.resize('tb');
                    }
                    if ($('#chart1').length > 0) {
                        echarts.init(document.getElementById('chart1')).resize();
                        echarts.init(document.getElementById('chart2')).resize();
                    }
                });
            }
        }
    }
    function initChart() {
        var chart1 = echarts.init(document.getElementById('chart1'));
        var option1 = {
            color: ['#0c9184'],
            title: {
                text: '实时访问量统计',
                x: 'center',
                textStyle: {
                    fontSize: 14,
                    color: '#333'
                }
            },
            tooltip: {
                trigger: 'axis'
            },
            xAxis: {
                type: 'category',
                data: ['15:00', '14:00', '13:00', '12:00', '11:00', '10:00', '09:00']
            },
            yAxis: {
                type: 'value'
            },
            series: [{
                data: [28, 39, 45, 88, 69, 80, 75],
                type: 'line',
                name: '访问量',
                smooth: true,
                symbolSize: 8,
            }],

        };
        chart1.setOption(option1);

        var chart2 = echarts.init(document.getElementById('chart2'));
        var option2 = {
            color: ['#0c9184'],
            title: {
                text: '最近7日订单统计',
                x: 'center',
                textStyle: {
                    fontSize: 14,
                    color: '#333'
                }
            },
            tooltip: {
                trigger: 'axis'
            },
            xAxis: {
                type: 'category',
                data: ['05-16', '05-15', '05-14', '05-13', '05-12', '05-11', '05-10']
            },
            yAxis: {
                type: 'value'
            },
            series: [{
                data: [56, 39, 45, 20, 69, 80, 75],
                type: 'line',
                name: '订单数量',
                smooth: true,
                symbolSize: 8,
            }],

        };
        chart2.setOption(option2);
    }
});

(function (T, h, i, n, k, P, a, g, e) { g = function () { P = h.createElement(i); a = h.getElementsByTagName(i)[0]; P.src = k; P.charset = "utf-8"; P.async = 1; a.parentNode.insertBefore(P, a) }; T["ThinkPageWeatherWidgetObject"] = n; T[n] || (T[n] = function () { (T[n].q = T[n].q || []).push(arguments) }); T[n].l = +new Date(); if (T.attachEvent) { T.attachEvent("onload", g) } else { T.addEventListener("load", g, false) } }(window, document, "script", "tpwidget", "//widget.seniverse.com/widget/chameleon.js"));

tpwidget("init", {
    "flavor": "slim",
    "location": "WX4FBXXFKE4F",
    "geolocation": "enabled",
    "language": "zh-chs",
    "unit": "c",
    "theme": "chameleon",
    "container": "tp-weather-widget",
    "bubble": "enabled",
    "alarmType": "badge",
    "color": "#FFFFFF",
    "uid": "U515609638",
    "hash": "670ac4236d30a43bfbc2eebbf3c6051a"
});
tpwidget("show");