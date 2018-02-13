﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PictureManage.aspx.cs" Inherits="ERPProject.ERPDictionary.PictureManage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>商品图片批量维护</title>
    <link rel="stylesheet" type="text/css" href="/res/webuploader/webuploader.css" />
    <link rel="stylesheet" type="text/css" href="/res/webuploader/detail/style.css" />
    <style>
        .w1000 {
            margin: 0 auto;
            width: 90%;
        }

            .w1000 img {
                width: 100%;
            }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="PanelDetail" runat="server" OnCustomEvent="PageManager1_CustomEvent" />
        <f:Panel ID="PanelDetail" runat="server" ShowBorder="false" BodyPadding="0px" Layout="HBox" BoxConfigAlign="Stretch" BoxConfigPosition="Start" BoxConfigChildMargin="0" ShowHeader="False">
            <Items>
                <f:Panel ID="PanelPicture" runat="server" ShowHeader="false" BoxFlex="2" ShowBorder="false"
                    BodyPadding="0px" Layout="Fit">
                    <Items>
                        <f:Grid ID="gridGoods" ShowBorder="false" ShowHeader="false" AllowSorting="false" EnableCheckBoxSelect="false"
                            AutoScroll="true" runat="server" EnableRowClickEvent="true" EnableColumnLines="true" PageSize="50" AllowPaging="true"
                            IsDatabasePaging="true" DataKeyNames="GDSEQ,ROWNO,FLAG,ISFLAG7" OnRowClick="gridGoods_RowClick" OnPageIndexChange="gridPic_PageIndexChange">
                            <Toolbars>
                                <f:Toolbar ID="Toolbar2" runat="server">
                                    <Items>
                                        <f:Label runat="server" ID="lblLicenseID" Hidden="true" />
                                        
                                        <f:CheckBox ID="cbxNonPic" ShowLabel="false" runat="server" CssStyle="margin-left:5px;" Text="无图" />
                                        <f:DropDownList ID="ddlISFLAG7" runat="server" Label="类型" CssStyle="margin-left:5px;" LabelWidth="45px" Width="150px">
                                            <f:ListItem Text="---请选择---" />
                                            <f:ListItem Text="本地商品" Value="Y" Selected="true" />
                                            <f:ListItem Text="下传商品" Value="N" />
                                        </f:DropDownList>
                                        <f:TriggerBox ID="trbSearch" runat="server" EmptyText="按商品编码,商品名称,助记码查询" OnTriggerClick="trbSearch_TriggerClick" TriggerIcon="Search" />
                                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                        <f:Button ID="btnPic" Hidden="true" runat="server" EnablePostBack="true" Text="图片上传" Icon="PictureAdd" OnClick="btnPic_Click" EnableDefaultState="false" />
                                        <f:Button ID="btnSearch" runat="server" EnablePostBack="true" Text="查 询" Icon="SystemSearch" OnClick="btnSearch_Click" EnableDefaultState="false" />
                                    </Items>
                                </f:Toolbar>
                            </Toolbars>
                            <Columns>
                                <f:RowNumberField Width="30" TextAlign="Center" />
                                <f:BoundField DataField="GDSEQ" Hidden="true" />
                                <f:BoundField Width="105px" DataField="GDID" HeaderText="商品编码" TextAlign="Center" />
                                <f:BoundField Width="140px" DataField="GDNAME" HeaderText="商品名称" TextAlign="Left" />
                                <f:BoundField Width="60px" DataField="FLAG_CN" HeaderText="状态" TextAlign="Center" />
                                <f:BoundField Width="60px" DataField="ISFLAG7_CN" HeaderText="商品类型" TextAlign="Center" />
                                <f:BoundField Width="80px" DataField="HSJJ" HeaderText="含税进价" TextAlign="Center" DataFormatString="{0:F2}" />
                                <f:BoundField Width="60px" DataField="LSJ" HeaderText="售价" TextAlign="Center" DataFormatString="{0:F2}" />
                                <f:BoundField Width="60px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                                <f:BoundField Width="130px" DataField="GDSPEC" HeaderText="规格·容量" TextAlign="Center" />
                                <f:BoundField Width="140px" DataField="PIZNO" HeaderText="注册证号" TextAlign="Center" />
                                <f:BoundField Width="90px" DataField="ZJM" HeaderText="助记码" TextAlign="Center" />
                                <f:BoundField Width="80px" DataField="CATID0NAME" HeaderText="商品种类" TextAlign="Center" />
                                <f:BoundField Width="0px" DataField="CATID0NAME_F" HeaderText="商品分类" TextAlign="Center" Hidden="true" />
                                <f:BoundField Width="140px" DataField="PRODUCERNAME" HeaderText="产地" TextAlign="Center" />
                                <f:BoundField Width="80px" DataField="BZHL" HeaderText="包装含量" Hidden="true" />
                                <f:BoundField Width="150px" DataField="SUPNAME" HeaderText="供应商" TextAlign="Center" />
                                <f:BoundField Width="0px" DataField="ISFLAG7" />
                            </Columns>
                        </f:Grid>
                    </Items>
                </f:Panel>
                <f:Panel ID="sfmImage" runat="server" CssStyle="border-left: 1px solid #99bce8;" Width="460px"
                    AutoScroll="true" ShowBorder="false" ShowHeader="false" BodyPadding="0px">
                    <Items>
                        <f:Panel runat="server" ShowHeader="false" ShowBorder="false" Height="200px">
                            <Items>
                                <f:ContentPanel runat="server" ShowHeader="false" ShowBorder="false" Height="200px">
                                    <div id="wrapper">
                                        <div id="container">
                                            <!--头部，相册选择和格式选择-->

                                            <div id="uploader">
                                                <div class="queueList">
                                                    <div id="dndArea" class="placeholder">
                                                        <div id="filePicker"></div>
                                                        <p>或将照片拖到这里，单次最多可选3张</p>
                                                    </div>
                                                </div>
                                                <div class="statusBar" style="display: none;">
                                                    <div class="progress">
                                                        <span class="text">0%</span>
                                                        <span class="percentage"></span>
                                                    </div>
                                                    <div class="info"></div>
                                                    <div class="btns">
                                                        <div id="filePicker2"></div>
                                                        <div class="uploadBtn">开始上传</div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </f:ContentPanel>
                            </Items>
                        </f:Panel>
                        <f:Panel ID="gridPicList" runat="server" ShowBorder="false" Height="150px" BodyPadding="0" ShowHeader="false" Layout="Fit">
                            <Items>
                                <f:Grid ID="gridPic" ShowBorder="false" ShowHeader="false" AllowSorting="false" EnableCheckBoxSelect="false"
                                    AutoScroll="true" runat="server" PageSize="10" EnableRowClickEvent="true" AllowCellEditing="true" ClicksToEdit="1"
                                    DataKeyNames="GDSEQ,ROWNO,FLAG" EnableColumnLines="true" OnRowCommand="gridPic_RowCommand" OnRowClick="gridPic_RowClick">
                                    <Columns>
                                        <f:RowNumberField Width="25" TextAlign="Center" />
                                        <f:LinkButtonField ColumnID="shanchu" CommandName="shanchu" ConfirmText="是否要删除该商品图片？" Text="删除" ConfirmTarget="Top" Width="40px" />
                                        <f:BoundField DataField="GDSEQ" Width="84px" HeaderText="商品编码" TextAlign="Center" />
                                        <f:BoundField DataField="ROWNO" Width="0px" HeaderText="系号" TextAlign="Center" Hidden="true" />
                                        <f:BoundField DataField="FLAG" Width="56px" HeaderText="状态" TextAlign="Center" />
                                        <f:BoundField DataField="GDPICT" Width="103px" HeaderText="图片名" TextAlign="Center" />
                                        <f:RenderField DataField="STR1" Width="98px" ColumnID="STR1" HeaderText="图片别名" TextAlign="Center">
                                            <Editor>
                                                <f:TextBox ID="tbxSTR1" EmptyText="编辑别名" MaxLength="50" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:LinkButtonField CommandName="lbfsava" ColumnID="lbfsave" Text="保存别名" Width="76px" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                        <f:Panel ID="panelPic" runat="server" ShowBorder="false" ShowHeader="false" BodyPadding="0" Layout="Fit">
                            <Items>
                                <f:Form ID="FormPic" ShowBorder="false" AutoScroll="false" CssStyle="padding:5px;" ShowHeader="False" LabelWidth="80px" runat="server">
                                    <Rows>
                                        <f:FormRow>
                                            <Items>
                                                <f:Image ID="imgBMPPATH" CssClass="w1000" Height="250px" runat="server" ImageUrl="~/res/images/model.png" />
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Panel>
            </Items>
        </f:Panel>
        <f:HiddenField ID="hfdGDID" runat="server" />
        <f:HiddenField ID="hfdGDSEQ" runat="server" />
        <f:HiddenField ID="hfdISFLAG7" runat="server" />
        <f:HiddenField ID="hfdTempPic" runat="server" />
        <f:HiddenField ID="hfdTempPic1" runat="server" />

    </form>
    <%--<script>
        var imgBMPPATH = '<%= imgBMPPATH.ClientID%>'; //图片点击看大图
        F.ready(function () {
            $('#' + imgBMPPATH).on('click', function (e) { window.open($(e.currentTarget).find('img').eq(0).attr('src')) });
        });
    </script>--%>
    <script type="text/javascript" src="/res/webuploader/webuploader.nolog.min.js"></script>
    <script type="text/javascript" src="/res/webuploader/detail/myupload.js"></script>
    <script>
        var ISFLAG7 = ('<%= hfdISFLAG7.ClientID%>');
        function rebind(gdid) {
            $('#uploader').myUpload("destroy");

            $('#uploader').myUpload('rebind', {
                formData: {
                    owner: gdid
                }
            });

        }
        var _init = true;
        function initUploader() {
            $('#uploader').myUpload({
                onBeforeFileQueued: function () {
                    //console.log(F(GDID).getValue())
                    //console.log(this)
                    //console.log(this.options.formData.owner)
                    if (!this.options.formData.owner) {
                        //if (F(GDID).getValue().length <= 0) {
                        F.util.alert("未选择商品，无法进行添加")
                        return false;
                    }
                    if (F(ISFLAG7).getValue() == "N") {
                        F.util.alert("下传商品，不允许在ERP维护图片")
                        return false;
                    }
                }
            });
        }

        F.ready(function () {
            initUploader();
        })

    </script>
</body>
</html>
