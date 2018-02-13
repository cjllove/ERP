/*********************************************************************************
* 系 统 名 ：威高讯通院内物流管理系统HSC  V3.0+
* 程 序 ID ：wego.js
* 功能概要 ：系统主界面函数
* 0.00      2016/02/21             新作成
*********************************************************************************/
//四舍五入
function round2(value) {
    var f = parseFloat(value);
    if (isNaN(f)) {
        return '0.00';
    }
    var f = Math.round(value * 100) / 100;
    var s = f.toString();
    var rs = s.indexOf('.');
    if (rs < 0) {
        rs = s.length;
        s += '.';
    }
    while (s.length <= rs + 2) {
        s += '0';
    }
    return s;
}

function round4(value) {
    var f = parseFloat(value);
    if (isNaN(f)) {
        return '0.0000';
    }
    var f = Math.round(value * 10000) / 10000;
    var s = f.toString();
    var rs = s.indexOf('.');
    if (rs < 0) {
        rs = s.length;
        s += '.';
    }
    while (s.length <= rs + 4) {
        s += '0';
    }
    return s;
}

function round6(value) {
    var f = parseFloat(value);
    if (isNaN(f)) {
        return '0.000000';
    }
    var f = Math.round(value * 1000000) / 1000000;
    var s = f.toString();
    var rs = s.indexOf('.');
    if (rs < 0) {
        rs = s.length;
        s += '.';
    }
    while (s.length <= rs + 6) {
        s += '0';
    }
    return s;
}

/*
  * 添 加 人：c
  * 添加时间：2015-06-30 
  * 备注说明：写入cookies
 */
function setCookie(name, value) {
    var Days = 30;
    var exp = new Date();
    exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);
    document.cookie = name + "=" + escape(value) + ";expires=" + exp.toGMTString();
}
/*
  * 添 加 人：c
  * 添加时间：2015-06-30 
  * 备注说明：读取cookies 
 */
function getCookie(name) {
    var arr, reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");

    if (arr = document.cookie.match(reg))
        return unescape(arr[2]);
    else
        return null;
}
/*
  * 添 加 人：c
  * 添加时间：2015-06-30 
  * 备注说明：删除cookies  
 */
function delCookie(name) {
    var exp = new Date();
    exp.setTime(exp.getTime() - 1);
    var cval = getCookie(name);
    if (cval != null)
        document.cookie = name + "=" + cval + ";expires=" + exp.toGMTString();
}


/***曹**/

/**
 * 检查浏览器版本
 * @return object browser info
 */
var browser = (function () {
    var userAgent = navigator.userAgent,
    ua = userAgent.toLowerCase(),
    browserList = {
        msie: /(?:msie\s|trident.*rv:)([\w.]+)/i,
        firefox: /Firefox\/([\w.]+)/i,
        chrome: /Chrome\/([\w.]+)/i,
        safari: /version\/([\w.]+).*Safari/i,
        opera: /(?:OPR\/|Opera.+version\/)([\w.]+)/i
    },
    kernels = {
        MSIE: /(compatible;\smsie\s|Trident\/)[\w.]+/i,
        Camino: /Camino/i,
        KHTML: /KHTML/i,
        Presto: /Presto\/[\w.]+/i,
        Gecko: /Gecko\/[\w.]+/i,
        WebKit: /AppleWebKit\/[\w.]+/i
    },
    browser = {
        kernel: 'unknow',
        version: 'unknow'
    }
    // 检测浏览器
    for (var i in browserList) {
        var matchs = ua.match(browserList[i]);
        browser[i] = matchs ? true : false;
        if (matchs) {
            browser.version = matchs[1];
        }
    }
    // 检测引擎
    for (var i in kernels) {
        var matchs = ua.match(kernels[i]);
        if (matchs) {
            browser.kernel = matchs[0];
        }
    }
    // 系统
    var os = ua.match(/(Windows\sNT\s|Mac\sOS\sX\s|Android\s|ipad.*\sos\s|iphone\sos\s)([\d._-]+)/i);
    browser.os = os !== null ? os[0] : false;
    // 是否移动端
    browser.mobile = ua.match(/Mobile/i) !== null ? true : false;
    return browser;
}());

//登录页面通知遮罩
if (location.href.toLowerCase().indexOf("login.aspx") > -1) {
    F.ready(function () {
        $.getScript("/res/js/layer.js", function () {
            var notice = function () {
                var noticeRead = getCookie("noticeRead");
                if (noticeRead != "1") {
                    $.ajax({
                        url: '/content.html',
                        success: function (content) {
                            layer.open({
                                type: 1
                                , title: false //不显示标题栏
                                , closeBtn: false
                                , area: '500px;'
                                , shade: 0.8
                                , id: 'LAY_layuipro' //设定一个id，防止重复弹出
                                , btn: ['我知道了', '暂时关闭']
                                , moveType: 1 //拖拽模式，0或者1
                                , content: '<div style="padding: 50px; line-height: 22px; background-color: #393D49; color: #fff; font-weight: 300;">' + content + '</div>'
                                , success: function (layero) {
                                    var btn = layero.find('.layui-layer-btn');
                                    btn.css('text-align', 'center');
                                    btn.find('.layui-layer-btn0').attr({
                                        href: 'javascript:void(0);'
                                        , target: '_blank'
                                    }).on('click', function () {
                                        setCookie("noticeRead", "1");
                                    });
                                }
                            });
                        }
                    });
                } 
            }

            notice();
        });
    });
}