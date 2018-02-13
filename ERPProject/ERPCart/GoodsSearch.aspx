<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoodsSearch.aspx.cs" Inherits="ERPProject.ERPCart.GoodsSearch" %>

<%--<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>--%>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>商品搜索</title>
    <link rel="stylesheet" href="../res/css/shoufengqin.css" />
    <link href="../res/css/gwselect.css" rel="stylesheet" type="text/css" />
    <link href="../res/css/gwNumberBox.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        /* 半透明的遮罩层 */
        #overlay {
            background: #000;
            filter: alpha(opacity=50); /* IE的透明度 */
            opacity: 0.5; /* 透明度 */
            display: none;
            position: absolute;
            top: 0px;
            left: 0px;
            width: 100%;
            height: 100%;
            z-index: 100; /* 此处的图层要大于页面 */
            display: none;
        }

        .shade_dialog {
            position: absolute;
            width: 680px;
            height: 440px;
            display: none;
            z-index: 200;
            border: 1px solid #C2C2C2;
            background: #F0F5FB;
            padding: 10px;
        }

        .shade_dialog_detail {
            height: 220px;
            border: 1px solid #AED0EA;
            padding: 10px;
            background: #fff;
            margin-bottom: 10px;
        }

            .shade_dialog_detail .tupian {
                float: left;
            }

            .shade_dialog_detail .mcdd {
                line-height: 25px;
                color: #999;
                float: left;
                margin-left: 10px;
                margin-top: 15px;
            }

            .shade_dialog_detail strong {
                font-size: 16px;
                color: #6184BE;
                background: #fff;
                width: 400px;
                height: 35px;
                overflow: hidden;
                display: inline-block;
            }

        .shade_dialog_content {
            height: 180px;
            border: 1px solid #AED0EA;
            padding: 10px;
            background: #fff;
        }

            .shade_dialog_content ul {
            }

                .shade_dialog_content ul li {
                    float: left;
                    width: 300px;
                    list-style: none;
                    line-height: 22px;
                    text-indent: 15px;
                    margin-left: 15px;
                    margin-top: 15px;
                    color: #999;
                }

        .closeIcon {
            position: absolute;
            right: -15px;
            top: -15px;
        }

            .closeIcon a {
                display: block;
                width: 32px;
                height: 32px;
                overflow: hidden;
                text-indent: -9999px;
                background: url(../res/images/gw_close.png) no-repeat;
            }

        .goods_info {
            margin-bottom: 20px;
        }

            .goods_info h2 {
                font-size: 16px;
                line-height: 24px;
                margin: 5px 0 10px;
                color: #0440a5;
            }

            .goods_info p {
                font-size: 12px;
                line-height: 24px;
                color: #666;
            }

        .gw-number-box {
            text-align: center;
        }

        .addCart {
            margin-left: 20px;
            cursor: pointer;
        }
    </style>

    <style type="text/css">
        h1, h2, h3, h4, div, p, ul, li, img {
            margin: 0;
            padding: 0;
        }

        li {
            list-style: none;
        }

        body {
            background: #F0F5FB;
        }

        div#container {
            width: 95%;
            margin: 0 auto;
        }

        div#header {
            height: 80px;
            width: 100%;
            overflow:hidden;
        }
        .search-left {
            width: 25%;
            /*margin: 0px auto;*/
            float:left;
            /*padding-left:10px;*/
            padding-top: 25px;
            display:inline-block;
        }
        .search {
            width: 75%;
            /*margin: 0px auto;*/
            /*float:left;*/
            padding-left:10px;
            padding-top: 25px;
            display:inline-block;
           
        }

        #txtSearchString {
            border: #aed0ea 1px solid;
            min-width: 110px;            
            width:50%;
            height: 35px;
            padding: 5px 0;
            vertical-align: middle;
            line-height: 23px;
            background:none;
            text-indent: 10px;
            color: #666;
            float: left;
        }

        .search img {
            float: left;
        }

        div#menu {
            height: auto;
            width: 25%;
            float: left;
        }

            div#menu h2 {
                background-color:#d7ebf9;
                height: 33px;
                line-height: 33px;
                text-align: center;
                font-size: 13px;
                border:1px #aed0ea solid;
                border-bottom:none;
                color:#2779aa;
                font-weight:normal;
            }

        div#wrapper {
            padding: 10px;
            padding-top: 0;
            border: 1px solid #AED0EA;
            background: #fcfcfc;
        }

        div#right {
            height: auto;
            width: 75%;
            float: left;
        }

        ul.m_c {
        }

            ul.m_c li {
                padding: 15px 20px;
                border-bottom: #ebebeb 1px solid;
                height: 124px;
            }

                ul.m_c li:hover {
                    background: #F7FBFF;
                }

                ul.m_c li .mcdd {
                    line-height: 25px;
                    color: #999;
                    float: left;
                    margin-left: 10px;
                    margin-top: 15px;
                }

                    ul.m_c li .mcdd strong {
                        font-size: 16px;
                        color: #6184BE;
                        width: 400px;
                        height: 35px;
                        overflow: hidden;
                        display: inline-block;
                    }

                ul.m_c li .jgmj {
                    float: right;
                    margin-top: 10px;
                    text-align: center;
                    width: 150px;
                    line-height: 30px;
                    font-size: 14px;
                    color: #666;
                    font-family: Arial;
                }

                    ul.m_c li .jgmj strong {
                        font-size: 30px;
                        color: #FF541F;
                        padding-right: 6px;
                    }

                ul.m_c li .tupian {
                    float: left;
                }

        iframe {
            margin: 0;
            padding: 0;
        }


        .r_main {
            padding: 5px;
            margin: 0 0px 0 10px;
            border: 1px solid #AED0EA;
            background: #fcfcfc;
        }

            .r_main .bt {
                padding: 0 20px;
                height: 33px;
                line-height: 33px;
                background: #F5F5F5;
                text-align: right;
                font-size: 14px;
            }

            .r_main p.title {
                float: left;
            }
        #ddlDEPTOUT {
            width:100%;
        
        }
        #ddlDEPTOUT_wrapper {
            width:100%;
        }
        #ddlDEPTOUT input {
            height:35px;
            line-height:35px;
            padding:8px;
            
        }
        #ddlDEPTOUT .f-field-fieldlabel {
            font-size:14px;
            font-family:微软雅黑;
            padding:3px;
        
        }
        #BtnIns {
            box-sizing:border-box;
            padding:4px 10px 3px 10px;
            height:35px;
            background-color:#1ea3d8;
            margin-left:10px;
            border-radius:3px;
        }
        #BtnIns a{
            text-decoration:none;
            color:white;
            font-size:12px;
           
        }
        .hide {
            display:none;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" runat="server" EnableAjaxLoading="false" />
        <input id="hidSearchConCat" type="hidden" value="" />
        <input id="hidSearchConKeyword" type="hidden" value="" />
        <input id="hidGdseq" type="hidden" value="" />
        <input id="hidNext" type="hidden" value="0" />
        <div id="container">
            <div id="header">
                <div class="search-left">
                    <f:DropDownList runat="server" LabelWidth="80px" ID="ddlDEPTOUT" Label="库房"  EnableEdit="true" ForceSelection="true" AutoPostBack="true" ShowRedStar="true" Required="true" OnSelectedIndexChanged="ddlDEPTOUT_SelectedIndexChanged"></f:DropDownList>
                </div>
                <div class="search">
                    <asp:TextBox ID="txtSearchString" runat="server" Style="" onkeydown="if(event.keyCode==13) submitKeyword();"></asp:TextBox><img src="../res/images/buttonSearch.png" onclick="submitKeyword();" />
                    <f:LinkButton runat="server" Text="载入模板" ID="BtnIns" OnClick="BtnIns_Click"></f:LinkButton>
                </div>
            </div>
            <div id="menu">
                <h2>商品分类</h2>
                <div id="wrapper">
                    <main class="main-wrapper">
                        
                            <asp:ScriptManager runat="server" EnablePartialRendering="true" ></asp:ScriptManager>
                            <asp:UpdatePanel ID="UpdateList" runat="server" RenderMode="Block" UpdateMode="Always" ChildrenAsTriggers="True">
                                <Triggers>
                                    <asp:PostBackTrigger ControlID= "RepeaterCat"/>
                                </Triggers>
                                        <ContentTemplate>
                                            <asp:Button CssClass="hide" ID="btnPostBack" runat="server" OnClick="btnPostBack_Click" />
                                            <div id="thirth">
                                            <asp:Repeater ID="RepeaterCat" runat="server" OnPreRender="RepeaterCat_PreRender" OnItemDataBound="RepeaterCat_ItemDataBound">
                                                <ItemTemplate>
                                                    <asp:HiddenField ID="hfdCode" runat="server" Value='<%#Eval("CODE") %>'></asp:HiddenField>
                                                    <h3><%#Eval("NAME") %></h3>
                                                    <div>
                                                        <asp:Repeater ID="RepeaterCatN" runat="server">
                                                            <ItemTemplate>
                                                                <p class="list_style"><a href="javascript:void(0);" onclick="submitCAT('<%#Eval("CODE") %>','<%#Eval("NAME") %>')"><%#Eval("NAME") %></a></p>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                                </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                        
                        <!-- end of thirth -->

                    </main><!-- end of main-wrapper -->
                    <div style="clear: both;"></div>
                </div>
            </div>

            <div id="right">
                <div class="r_main">
                    <div class="bt">
                        <%--<f:DropDownList runat="server" ID="ddlDEPTOUT" Label="库房" AutoPostBack="true" ShowRedStar="true" Required="true" OnSelectedIndexChanged="ddlDEPTOUT_SelectedIndexChanged"></f:DropDownList>--%>
                        
                        <p class="title">点选的商品类别：</p>
                    </div>
                    <div id="content" class="gw-select"></div>
                    <iframe id="iframeGoods" width="100%" height="1800px" src="GoodsSearchContent.aspx" frameborder="no" border="0" marginwidth="0" marginheight="0" scrolling="no"></iframe>
                </div>
            </div>

        </div>

        <!-- JS遮罩层 -->
        <div id="overlay"></div>
        <!-- end JS遮罩层 -->
        <!-- 对话框 -->
        <div id="goodsDetail" class="shade_dialog">
            <div class="closeIcon"><a href="javascript:void(0);" class="closeIt" onclick="hideOverlay();">关闭</a></div>
            <div class="shade_dialog_detail">
                <p class="tupian"></p>
                <p class="mcdd"></p>
            </div>
            <div class="shade_dialog_content"></div>
        </div>
        <f:HiddenField runat="server" ID="Path"></f:HiddenField>
        <!-- JS遮罩层上方的对话框 -->
        <f:Window ID="Window1" Title="商品信息" Hidden="true" Height="520px" Width="510px" EnableIFrame="true" AutoScroll="false" runat="server" EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True">
        </f:Window>
        <f:Window ID="Window2" Title="加载模板信息" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="false" EnableResize="false" IsModal="True" Layout="Fit" Width="470px" Height="320px">
            <Items>
                <f:Grid ID="GridTemplate" ShowBorder="false" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server" EnableMultiSelect="false"
                    DataKeyNames="GROUPID,GROUPNAME" EnableColumnLines="true" OnRowCommand="GridTemplate_RowCommand"
                    EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridTemplate_RowDoubleClick">
                    <Columns>
                        <f:RowNumberField runat="server" Width="30" TextAlign="Center" />
                        <f:BoundField DataField="GROUPID" EnableColumnHide="true" EnableHeaderMenu="false" Hidden="true" />
                        <f:BoundField DataField="GROUPNAME" HeaderText="模板名称" EnableColumnHide="true" EnableHeaderMenu="false" ExpandUnusedSpace="true" />
                        <f:BoundField DataField="USERNAME" HeaderText="添加人" Width="90px" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:LinkButtonField HeaderText="操作" Width="50px" TextAlign="Center" CommandName="FileDelete" Text="删除" ConfirmText="确定要删除该模板么？" />
                    </Columns>
                </f:Grid>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar8" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="btnLoadTemplateClose" Text="保存确定" Icon="SystemSave" runat="server" OnClick="btnLoadTemplateClose_Click">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
    </form>
    <script src="../res/js/html5shiv.js"></script>

    <script src="../res/js/rlaccordion.js"></script>

    <script src="../res/js/jquery.gwselect.js" type="text/javascript"></script>
    <script src="../res/js/jquery.gwNumberBox.js" type="text/javascript"></script>

    <script type="text/javascript">
        /* 分类查询 */
        function submitCAT(catId, catName) {
            var keyword = document.getElementById("hidSearchConKeyword").value;
            var catIds = document.getElementById("hidSearchConCat").value;
            if (catIds != null && catIds != '') {
                //判断是否已经存在选择类别
                if (("," + catIds).indexOf(catId) > 0)
                    return;
                catIds = catIds + ',' + catId;
            }
            else {
                catIds = catId;
            }

            document.getElementById("hidSearchConCat").value = catIds;

            //调用条件组件
            $('.gw-select').gwSelect('add', { text: '(' + catId + ')' + catName, closeEvent: myCloseEvent, openEvent: myOpenEvent });
            function myOpenEvent() {
                $(this).attr('data-value', catId);
            }
            function myCloseEvent() {
                var removeCatId = $(this).attr('data-value');
                var keywordHid = document.getElementById("hidSearchConKeyword").value;
                var catIdsHid = document.getElementById("hidSearchConCat").value;
                var catIdsN = '';
                if (catIdsHid.indexOf(",") > 0) {
                    var catIdsArray = catIdsHid.split(",");
                    for (i = 0; i < catIdsArray.length; i++) {
                        if (catIdsArray[i] != removeCatId && catIdsArray[i] != null && catIdsArray[i] != "") {
                            catIdsN += catIdsArray[i] + ',';
                        }
                    }
                    //去掉最后的，
                    catIdsN = catIdsN.substring(0, catIdsN.length - 1)
                }
                document.getElementById("hidSearchConCat").value = catIdsN;
                document.getElementById("iframeGoods").src = 'GoodsSearchContent.aspx?k=' + keywordHid + '&catid=' + catIdsN + '&dpetid=' + F('<%= ddlDEPTOUT.ClientID%>').getValue();
            }

            document.getElementById("iframeGoods").src = 'GoodsSearchContent.aspx?k=' + keyword + '&catid=' + catIds + '&dpetid=' + F('<%= ddlDEPTOUT.ClientID%>').getValue();
        }

        /* 关键字查询 */
        function submitKeyword() {
            var keyword = document.getElementById("txtSearchString").value;
            if (keyword == null || keyword == '') {
                return;
            }
            var catIds = document.getElementById("hidSearchConCat").value;
            //调用条件组件
            $('.gw-select').gwSelect('add', { text: keyword, closeEvent: myCloseEvent, openEvent: myOpenEvent });
            function myCloseEvent() {
                document.getElementById("hidSearchConKeyword").value = '';
                var catIdsHid = document.getElementById("hidSearchConCat").value;
                document.getElementById("iframeGoods").src = 'GoodsSearchContent.aspx?k=&catid=' + catIdsHid + '&dpetid=' + F('<%= ddlDEPTOUT.ClientID%>').getValue();
            }

            function myOpenEvent() {
                $('li[data-type=keywords]').remove();
                $(this).attr('data-type', 'keywords');;
            }
            document.getElementById("hidSearchConKeyword").value = keyword;
            document.getElementById("iframeGoods").src = 'GoodsSearchContent.aspx?k=' + keyword + '&catid=' + catIds + '&dpetid=' + F('<%= ddlDEPTOUT.ClientID%>').getValue();
        }

        $(function () {

            $("#thirth").rlAccordion('mix', {
                childNum: 0
            });

        });
    </script>

    <script type="text/javascript">
        /* 显示遮罩层 */
        function showOverlay(gdseq) {
            $("#overlay").height(pageHeight());
            $("#overlay").width(pageWidth());

            // fadeTo第一个参数为速度，第二个为透明度
            // 多重方式控制透明度，保证兼容性，但也带来修改麻烦的问题
            $("#overlay").fadeTo(200, 0.5);
            adjust("#goodsDetail");
            $("#goodsDetail").fadeTo(200, 1);

            //取商品信息
            $.getJSON("../ERPCart/CartShowAndDo.aspx?gdseq=" + gdseq, function (data) {
                if (data != null && data != "") {

                    if (data.picpath != null && data.picpath != "") {
                        if (data.picpath.length > 1) {
                            if (data.picpath.length < $("#hidNext").val()) {
                                $("#hidNext").attr("value", "0");
                            }
                            if (parseInt($("#hidNext").val()) < 0) {
                                $("#hidNext").attr("value", data.picpath.length);
                            }
                            $(".tupian")[0].innerHTML = '<a style="position: absolute;font-size: 30px;margin-top: 80px;color: white;" href="javascript:void(0);" onclick="LastPic()"><</a><img src="' + data.picpath[$("#hidNext").val()].path + '" width="200" height="200" /><a style="position: absolute;font-size: 30px;margin-top: 80px;color: white;margin-left: -22px;" href="javascript:void(0);" onclick="NextPic()">></a>';
                            //$(".tupian")[0].innerHTML = '<a style="position: absolute;font-size: 30px;margin-top: 80px;color: white;" href="javascript:void(0);" onclick="LastPic()"><</a><img src="../captcha/GoodsPicture.aspx?picPath=' + data.picpath[$("#hidNext").val()].path + '" width="200" height="200" /><a style="position: absolute;font-size: 30px;margin-top: 80px;color: white;margin-left: -22px;" href="javascript:void(0);" onclick="NextPic()">></a>';
                        }
                        else {
                            $(".tupian")[0].innerHTML = '<img src="' + data.picpath[0].path + '" width="200" height="200" />';
                            //$(".tupian")[0].innerHTML = '<img src="../captcha/GoodsPicture.aspx?picPath=' + data.picpath[0].path + '" width="200" height="200" />';
                        }
                    } else {
                        $(".tupian")[0].innerHTML = '<img src="../res/images/noPic.jpg" width="200" height="200" />';
                    }

                    $(".mcdd")[0].innerHTML = '<div class="goods_info"><h2>' + data.gdname + '</h2>' +
                    '<p class="goods_gdseq" id = "GdseqShow">商品编码：' + data.gdseq + '</p>' +
                    '<p class="goods_gdspec">商品规格：' + data.gdspec + '</p> ' +
                    '<p class="goods_pizno">生产商：' + data.producername + '</p></div>' +
                    '<input id="inputDSSL" type="text" class="gw-number-box" style="width:100px;height:34px; line-height:34px;"/>' +
                    '<img src="../res/images/addCart.jpg" onclick="addCart2()" class="addCart" />' +
                    '<img src="../res/images/addCart2.jpg" onclick="addCart()" class="addCart" />';


                    $(".shade_dialog_content")[0].innerHTML = '<ul><!--<li>商品名称：' + data.gdname + '</li><li>商品编码：' + data.gdseq + '</li>-->' +
                    '<!--<li>规格型号：' + data.gdspec + '</li>--><li>包装含量：' + data.bzhl + '</li> ' +
                    '<li>单&nbsp;&nbsp;位：' + data.unitname + '</li><li>分&nbsp;&nbsp;类：' + data.catname + '</li>' +
                    '<li>进&nbsp;&nbsp;价：' + data.jj + '</li><li>税&nbsp;&nbsp;率：' + data.jxtax + '</li>' +
                    '<li>供应商：' + data.supname + '</li><li>注册证号：' + data.pizno + '</li>' +
                    '<!--<li>注册证号：' + data.pizno + '</li>--><li>ERP编码：' + data.eascode + '</li>';

                    $('.gw-number-box').gwNumberBox({ min: 1, max: 1000, defaultVal: 1 });
                    $("#hidGdseq").attr("value", data.gdseq);
                } else {
                    return;
                }
            });
        }
        /* 隐藏覆盖层 */
        function hideOverlay() {
            $("#overlay").fadeOut(200);
            $("#goodsDetail").fadeOut(200);
        }
        /* 下张图片 */
        function NextPic() {
            $("#hidNext").attr("value", parseInt($("#hidNext").val()) + 1);
            showOverlay($("#hidGdseq").val());
        }
        function LastPic() {
            $("#hidNext").attr("value", parseInt($("#hidNext").val()) - 1);
            showOverlay($("#hidGdseq").val());
        }
        /* 显示大图 */
        function ShowPic(Gdseq) {
            F('Window1').f_show("/ERPQuery/GoodsPic.aspx?Gdseq=" + Gdseq, "大图", "500px", "500px")
        }
        /* 当前页面高度 */
        function pageHeight() {
            return document.body.scrollHeight;
        }

        /* 当前页面宽度 */
        function pageWidth() {
            return document.body.scrollWidth;
        }

        /* 定位到页面中心 */
        function adjust(id) {
            var w = $(id).width();
            var h = $(id).height();

            var t = scrollY() + (windowHeight() / 2) - (h / 2);
            if (t < 0) t = 0;

            var l = scrollX() + (windowWidth() / 2) - (w / 2);
            if (l < 0) l = 0;

            $(id).css({ left: l + 'px', top: t + 'px' });
        }

        //浏览器视口的高度
        function windowHeight() {
            var de = document.documentElement;

            return self.innerHeight || (de && de.clientHeight) || document.body.clientHeight;
        }

        //浏览器视口的宽度
        function windowWidth() {
            var de = document.documentElement;

            return self.innerWidth || (de && de.clientWidth) || document.body.clientWidth
        }

        /* 浏览器垂直滚动位置 */
        function scrollY() {
            var de = document.documentElement;

            return self.pageYOffset || (de && de.scrollTop) || document.body.scrollTop;
        }

        /* 浏览器水平滚动位置 */
        function scrollX() {
            var de = document.documentElement;

            return self.pageXOffset || (de && de.scrollLeft) || document.body.scrollLeft;
        }
        /* 把商品添加到购物车 */
        function addCart() {
            var gdseq = $("#hidGdseq").val();
            var dhs = $("#inputDSSL").val();
            var _p = parent;
            //将商品保存到数据库
            $.getJSON("../captcha/CartShowAndDo.aspx?gdseq=" + gdseq + "&dhs=" + dhs, function (data) {
                //if (parent.F('regionPanel_mainRegion_mainTabStrip').getTab('6122')) {
                //    parent.F('regionPanel_mainRegion_mainTabStrip').removeTab('6122');
                //}
                //parent.F('regionPanel_mainRegion_mainTabStrip').addTab({ id: '6122', url: '/ERPCart/GoodsCart.aspx', title: '购物车', closable: true, iconCls: 'x-tab-icon-el _extjs_res_ext_theme_classic_images_tree_leaf_gif ' });

                //if (_p.changeCartState) {
                //    _p.changeCartState();
                //}
                $.ajax(
                   '/captcha/CartShowAndDo.aspx?oper=sum', {
                       method: 'POST',
                       //data: src,
                       dataType: 'json',
                       success: function (result) {
                           top.$('#RegionPanel_topPanel .cart').attr('cart-count', result.dhs);
                           var tab = top.F(top.mainTabStripClientID).getTab('RegionPanel_mainTabStrip_6122')                           
                           if (tab && !tab.hidden) {
                               top.refreshTab('RegionPanel_mainTabStrip_6122');
                           } else {
                               top.F(top.mainTabStripClientID).addTab({
                                   id: 'RegionPanel_mainTabStrip_6122',
                                   iframe: true,
                                   iframeUrl: "/ERPCart/GoodsCart.aspx?fid=6122",
                                   title: "购物车",
                                   closable: true
                               });
                           }
                       }
                   })
            });
            hideOverlay();
        }
        function addCart2() {
            var gdseq = $("#hidGdseq").val();
            var dhs = $("#inputDSSL").val();
            var _p = parent;
            //将商品保存到数据库
            $.getJSON("../captcha/CartShowAndDo.aspx?gdseq=" + gdseq + "&dhs=" + dhs, function (data) {
                //if (parent.F('regionPanel_mainRegion_mainTabStrip').getTab('6122')) {
                //    parent.F('regionPanel_mainRegion_mainTabStrip').removeTab('6122');
                //}
                //if (_p.changeCartState) {
                //    _p.changeCartState();
                //}
                $.ajax(
                   '/captcha/CartShowAndDo.aspx?oper=sum', {
                       method: 'POST',
                       //data: src,
                       dataType: 'json',
                       success: function (result) {
                           top.$('#RegionPanel_topPanel .cart').attr('cart-count', result.dhs);
                           top.refreshTab('RegionPanel_mainTabStrip_6122');
                       }
                   })
            });
            hideOverlay();
        }
        function PosaddCart(gdseq, dhs) {
            var _p = parent;
            //将商品保存到数据库
            $.getJSON("../captcha/CartShowAndDo.aspx?gdseq=" + gdseq + "&dhs=" + dhs, function (data) {
                //if (parent.F('regionPanel_mainRegion_mainTabStrip').getTab('6122')) {
                //    parent.F('regionPanel_mainRegion_mainTabStrip').removeTab('6122');
                //}
                //if (_p.changeCartState) {
                //    _p.changeCartState();
                //}
                $.ajax(
                   '/captcha/CartShowAndDo.aspx?oper=sum', {
                       method: 'POST',
                       //data: src,
                       dataType: 'json',
                       success: function (result) {
                           top.$('#RegionPanel_topPanel .cart').attr('cart-count', result.dhs);
                           top.refreshTab('RegionPanel_mainTabStrip_6122');
                       }
                   })
               
            });
            hideOverlay();
        }
        function TemplateRefresh() {
            var _p = parent;
            //if (parent.F('regionPanel_mainRegion_mainTabStrip').getTab('6122')) {
            //    parent.F('regionPanel_mainRegion_mainTabStrip').removeTab('6122');
            //}
            //parent.F('regionPanel_mainRegion_mainTabStrip').addTab({ id: '6122', url: '/ERPCart/GoodsCart.aspx', title: '购物车', closable: true, iconCls: 'x-tab-icon-el _extjs_res_ext_theme_classic_images_tree_leaf_gif ' });
            //top.refreshTab('RegionPanel_mainTabStrip_6122');
            $.ajax(
                   '/captcha/CartShowAndDo.aspx?oper=sum', {
                       method: 'POST',
                       //data: src,
                       dataType: 'json',
                       success: function (result) {
                           top.$('#RegionPanel_topPanel .cart').attr('cart-count', result.dhs);
                           console.log(top.F(top.mainTabStripClientID).getTab('RegionPanel_mainTabStrip_6122'))
                           var tab = top.F(top.mainTabStripClientID).getTab('RegionPanel_mainTabStrip_6122')
                           if (tab && !tab.hidden) {
                               top.refreshTab('RegionPanel_mainTabStrip_6122');
                           } else {
                               top.F(top.mainTabStripClientID).addTab({
                                   id: 'RegionPanel_mainTabStrip_6122',
                                   iframe: true,
                                   iframeUrl: "/ERPCart/GoodsCart.aspx?fid=6122",
                                   title: "购物车",
                                   closable: true
                               });
                           }
                           
                           top.F(top.mainTabStripClientID).activeTab('RegionPanel_mainTabStrip_6122');
                           //top.addTab('')
                           //top.activeTab('RegionPanel_mainTabStrip_6122');
                       }
                   })
        }
    </script>
    <script type="text/javascript">
        $('.gw-select').gwSelect();
        $('.gw-number-box').gwNumberBox({ min: 0, max: 100, defaultVal: 1 });
    </script>
</body>
</html>
