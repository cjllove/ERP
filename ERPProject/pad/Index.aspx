<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="ERPProject.pad.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="minimal-ui"/>
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no"/>
    <meta name="apple-mobile-web-app-capable" content="yes" />
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link rel="stylesheet" href="/res/css/pad.css" />
    <link rel="stylesheet" href="/res/css/iconfont/iconfont.css" />
    <style>
        #form1{font: bold 12px/100% "微软雅黑", "Lucida Grande", "Lucida Sans", Helvetica, Arial, Sans;;}
        .lblSysName{padding-left:15px;}
        .lblWelcome{padding-right:15px;}
        .pad-menu-main {
            width:100%;
            display:inline-block;
            line-height:100px;
        }
        .pad-menu-item {
            width:32%;
            height:300px;
            float:left;
            display:inline-block;
            background:#e3f3ff;
            margin-left:1%;
        }
        .pad-menu-item {
            padding:60px;
            border:1px solid #aed0ea;
            box-shadow: 2px 2px 3px lightgray;
            margin-bottom:10px;
            /*background:-webkit-gradient(linear, 0 0, 0 bottom, from(#ffffff), to(#aed0ea)));*/ 
            
            background: -webkit-linear-gradient(#ffffff, #DEEDF7); /* Safari 5.1 - 6.0 */
            background: -o-linear-gradient(#ffffff,  #DEEDF7); /* Opera 11.1 - 12.0 */
            background: -moz-linear-gradient(#ffffff, #DEEDF7); /* Firefox 3.6 - 15 */
            background: linear-gradient(#ffffff, #DEEDF7); /* 标准的语法 */
        }
        .pad-menu-item .f-btn-inner{
            padding-top:50%;
        }
        .pad-menu-item i.f-btn-icon{
            margin-top:-120px;
        }
        .pad-menu-item i:before {
            font-size:10em;
            /*text-shadow: 5px 5px 5px lightgray;*/
            text-shadow: #333 0px 1px 1px;
        }
        .pad-menu-item .f-btn-text {
            font-size:1.8em;
            margin-top:20px;
            color:#333;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1"  runat="server" />
        <f:Panel ID="Panel1" ShowBorder="false" ShowHeader="false" Layout="VBox" BoxConfigChildMargin="0 0 5px 0" BodyPadding="10px" runat="server" AutoScroll="true">
             <Toolbars>
                        <f:Toolbar runat="server" HeaderStyle="true" >
                            <Items>
                                <%--<f:Button runat="server" ID="TestBtn" Text="测试" Size="Large" EnablePostBack="false" EnableDefaultState="false" IconFont="ArrowLeft">
                                    <Listeners>
                                         <f:Listener Event="click" Handler="onSlideRightClick" />
                                    </Listeners>
                                </f:Button>--%>
                                <f:Label runat="server" ID="lblSysName" CssClass="large-font lblSysName"></f:Label>
                                <f:ToolbarFill runat="server"></f:ToolbarFill>
                                <f:Label runat="server" ID="lblWelcome" CssClass="large-font lblWelcome"></f:Label>
                                <f:Button runat="server" Text="退出登录" Size="Large" EnablePostBack="false" EnableDefaultState="false"  ID="Button1">
                                    <Listeners>
                                        <f:Listener Event="click" Handler="login" />
                                    </Listeners>
                                 </f:Button>
                            </Items>
                            </f:Toolbar>
                 </Toolbars>
                <Items>
                    <f:ContentPanel runat="server" ShowBorder="false" ShowHeader="false" ID="MenuContent">
                        <div class="pad-menu-main">
                           <%-- <div class="pad-menu-item f-btn ui-corner-all f-btn-icon-top" data-url="/pad/gzconsumable.aspx">
                                <span class="f-btn-inner">
                                    <i class="f-btn-icon ui-icon f-icon-gear" style="display: inline-block;"></i>
                                    <span class="f-btn-text">系统维护</span>
                                </span>
                            </div>
                            <div class="pad-menu-item f-btn ui-corner-all f-btn-icon-top" data-url="//baidu.com"></div>--%>
                           
                        </div>
                    </f:ContentPanel>
                </Items>
         </f:Panel>
        <f:Panel runat="server" Layout="Fit" ID="Panel2" ShowHeader="false"  ShowBorder="false" Hidden="true" AutoScroll="false" BodyPadding="0" IsViewPort="true">
            <Items>
                <f:Window runat="server" IFrameUrl="#" ID="MainWindow" Layout="Fit" EnableIFrame="true" ShowHeader="false" ShowBorder="false" IsModal="false" BodyPadding="0"></f:Window>
            </Items>
        </f:Panel>
    </form>
    <script src="/res/js/pad.js" type="text/javascript"></script>
    <script>
          var Panel1 = '<%=Panel1.ClientID%>';
        var Panel2 = '<%=Panel2.ClientID%>';
        var MainWindow = '<%=MainWindow.ClientID%>';
        var MenuContent = '<%=MenuContent.ClientID%>';
        function onSlideRightClick() {
            //F.slideLeft(Panel1, Panel2);
            
        }
        F.ready(function () {
            F(MenuContent).el.on('touchend', '.pad-menu-main .pad-menu-item', function (e) {
                //if (F(MainWindow).iframeUrl != $(e.currentTarget).attr('data-url')) {
                //    F(MainWindow).setIFrameUrl($(e.currentTarget).attr('data-url'))
                //}
                //onSlideRightClick();
                location.href = $(e.currentTarget).attr('data-url')
            })
            menuInit();
        })
        var MENU_TEMPLATE = '<div class="pad-menu-item f-btn ui-corner-all f-btn-icon-top iconfont {1}" data-url="{0}">'
                                +'<span class="f-btn-inner">'
                                 +'   <i class="f-btn-icon ui-icon f-icon-{1}" style="display: inline-block;"></i>'
                                 +'   <span class="f-btn-text">{2}</span>'
                                +'</span>'
                                +'</div>'
        window.addEventListener(orientationEvent, function () {
            //alert('screen:' + window.orientation);
            //F(Panel2).reset();
            setTimeout(function () { F(MainWindow).doLayout(); },1000)
            
        }, false);
        function menuInit() {
            $.ajax('/pad/index.aspx?oper=menu', {
                success: function (data) {
                    var resultData = eval('(' + data + ')');
                    var objData = resultData["data"];
                    var result = resultData["result"];
                    if (result == "fail") {
                        F.alert(objData);
                        return;
                    }
                    objData = eval('(' + objData + ')');
                    $('.pad-menu-main').html();
                    if (objData.length > 0) {
                        for (var i = 0; i < objData.length; i++) {
                            var content = MENU_TEMPLATE
                            .replace(/\{0\}/ig, convertEmptyStr(objData[i]["RUNWHAT"]))
                            .replace(/\{1\}/ig, convertEmptyStr(objData[i]["FUNICO"].toLowerCase()))
                            .replace(/\{2\}/ig, convertEmptyStr(objData[i]["FUNCNAME"]))
                            var contentObj = $(content);
                            if (objData[i]["FUNICO"].indexOf('icon-') == -1) {
                                contentObj.removeClass("iconfont")
                            }
                            $('.pad-menu-main').append(contentObj)
                        }
                        
                    }
                    F(MenuContent).show();
                }
            })
        }

        function home() {
            F.slideRight(Panel2, Panel1);
            
        }
        function login() {
            location.href = '/pad/Login.aspx';
        }
    </script>
</body>
</html>
