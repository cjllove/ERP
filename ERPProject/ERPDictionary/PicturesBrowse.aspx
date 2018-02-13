<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PicturesBrowse.aspx.cs" Inherits="ERPProject.ERPDictionary.PicturesBrowse" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>商品图片浏览</title>
    <style type="text/css">
        .w1000 {
            margin: 0 auto;
            width: 100%;
        }

            .w1000 img {
                width: 100%;
            }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="TabStrip1" runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="1"
            runat="server">
            <Tabs>
                <f:Tab Title="商品列表" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
                            ShowHeader="false">
                            <Toolbars>
                                <f:Toolbar ID="Toolbar1" runat="server">
                                    <Items>
                                        <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="备注：双击查看商品图片信息！" runat="server" />
                                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                        <f:Button ID="btnPrint" Hidden="true" Icon="Printer" Text="打 印" DisableControlBeforePostBack="true" runat="server" EnableDefaultState="false" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" DisableControlBeforePostBack="true" runat="server" OnClick="btSearch_Click" EnableDefaultState="false" />
                                    </Items>
                                </f:Toolbar>
                            </Toolbars>
                            <Items>
                                <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="5px 10px 0px 10px"
                                    ShowHeader="False" LabelWidth="80px" runat="server">
                                    <Rows>
                                        <f:FormRow>
                                            <Items>
                                                <f:TextBox ID="tbxGDSEQ" runat="server" Label="商品编码" />
                                                <%--<f:DropDownList ID="ddlFLAG" runat="server" Label="商品状态" EnableEdit="true" ForceSelection="true" />--%>
                                                <f:TextBox ID="tbxGDNAME" runat="server" Label="商品名称" />
                                                <f:DropDownList ID="ddlCATID" runat="server" Label="商品类别" EnableEdit="true" ForceSelection="true" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:DropDownList ID="ddlSUPID" runat="server" Label="供应商" EnableEdit="true" ForceSelection="true" />
                                                <f:TextBox ID="tbxCD" runat="server" Label="产地"></f:TextBox>
                                                <f:TextBox ID="tbxPIZNO" runat="server" Label="注册证号"></f:TextBox>
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                                <f:Grid ID="GridGoods" AnchorValue="100% -60" ShowBorder="false" ShowHeader="false" EnableRowClickEvent="true"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" OnRowClick="GridGoods_RowDoubleClick"
                                    PageSize="100" DataKeyNames="GDSEQ" IsDatabasePaging="true" AllowPaging="true" OnPageIndexChange="GridGoods_PageIndexChange" EnableColumnLines="true">
                                    <Columns>
                                        <f:RowNumberField Width="30px" TextAlign="Center" />
                                        <f:BoundField Width="95px" DataField="GDID" HeaderText="商品编码" TextAlign="Center" />
                                        <f:BoundField Width="190px" DataField="GDNAME" HeaderText="商品名称" />
                                        <f:BoundField Width="90px" DataField="BARCODE" HeaderText="商品条码" Hidden="true" />
                                        <f:BoundField Width="60px" DataField="FLAGNAME" HeaderText="状态" TextAlign="Center" />
                                        <f:BoundField Width="130px" DataField="CATIDNAME" HeaderText="商品类别" />
                                        <f:BoundField Width="120px" DataField="GDSPEC" HeaderText="规格·容量" />
                                        <f:BoundField Width="60px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="HSJJ" HeaderText="含税进价" TextAlign="Right" DataFormatString="{0:F2}" />
                                        <f:BoundField Width="80px" DataField="LSJ" HeaderText="售价" TextAlign="Right" DataFormatString="{0:F2}" />
                                        <f:BoundField Width="180px" DataField="SUPNAME" HeaderText="供应商" />
                                        <f:BoundField Width="180px" DataField="CD" HeaderText="产地" />
                                        <f:BoundField Width="180px" DataField="PIZNO" HeaderText="注册证号" />
                                        <f:BoundField Width="90px" DataField="ZPBH" HeaderText="制品编号" />
                                    </Columns>
                                </f:Grid>
                                <f:Label ID="labelpic" runat="server" Hidden="true"></f:Label>
                                <f:Label ID="labelpic1" runat="server" Hidden="true"></f:Label>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="图片展示" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="panelPicture" runat="server" ShowBorder="false" BodyPadding="0px" Layout="HBox" BoxConfigAlign="Stretch" BoxConfigPosition="Start" BoxConfigChildMargin="0" ShowHeader="False">
                            <Items>
                                <f:Panel ID="gridPicList" runat="server" ShowBorder="false" BoxFlex="2" BodyPadding="0" ShowHeader="false" Layout="Fit">
                                    <Items>
                                        <f:Grid ID="gridPic" ShowBorder="false" ShowHeader="false" AllowSorting="false" EnableCheckBoxSelect="false"
                                            AutoScroll="true" runat="server" PageSize="10" EnableRowClickEvent="true"
                                            DataKeyNames="GDSEQ,rowno,FLAG,HANGNO" OnRowClick="gridPic_RowClick">
                                            <Columns>
                                                <f:RowNumberField Width="25" TextAlign="Center" />
                                                <f:BoundField DataField="GDSEQ" Width="80px" HeaderText="商品编码" TextAlign="Center" />
                                                <f:BoundField DataField="rowno" Width="60px" Hidden HeaderText="系号" TextAlign="Center" />
                                                <f:BoundField DataField="HANGNO" Width="60px" HeaderText="系号" TextAlign="Center" />
                                                <f:BoundField DataField="FLAG" Width="60px" HeaderText="状态" TextAlign="Center" />
                                                <f:BoundField DataField="GDPICT" Width="103px" HeaderText="图片名" TextAlign="Center" />
                                                <f:BoundField DataField="STR1" Width="0px" HeaderText="别名" TextAlign="Center" Hidden="true" />
                                                <f:BoundField DataField="PICPATH" Width="380px" ExpandUnusedSpace="true" HeaderText="图片路径" TextAlign="Center" />
                                            </Columns>
                                        </f:Grid>
                                    </Items>
                                </f:Panel>
                                <f:Panel ID="Panelp" runat="server" ShowBorder="false" ShowHeader="false" Width="500px"
                                    BodyPadding="0" Layout="VBox" BoxConfigAlign="Stretch" BoxConfigPosition="Start" BoxConfigChildMargin="0">
                                    <Items>
                                        <f:Panel ID="panelPic" runat="server" BoxFlex="2" ShowBorder="false" CssStyle="border-left: 1px solid #99bce8; border-top:0px;" ShowHeader="false" Width="500px" BodyPadding="0" Layout="Fit">
                                            <Items>
                                                <f:Form ID="FormPic" ShowBorder="false" AutoScroll="false" CssStyle="padding:0px;" ShowHeader="False" LabelWidth="80px" runat="server">
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
                                        <f:Panel ID="toolPanel" runat="server" BoxFlex="1" ShowBorder="false" CssStyle="border-left: 1px solid #99bce8; border-top:0px;" ShowHeader="false" BodyPadding="0" Layout="Fit">
                                            <Toolbars>
                                                <f:Toolbar ID="Toolbar2" runat="server">
                                                    <Items>
                                                        <f:Button ID="btnLeft" Text="<<  上一张" CssStyle="margin-left:195px;" DisableControlBeforePostBack="true" runat="server" OnClick="btnLeft_Click" />
                                                        <f:Button ID="btnRight" Text=">>  下一张" CssStyle="margin-right:220px;" DisableControlBeforePostBack="true" runat="server" OnClick="btnRight_Click" />
                                                    </Items>
                                                </f:Toolbar>
                                            </Toolbars>
                                        </f:Panel>
                                    </Items>
                                </f:Panel>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
        <f:HiddenField ID="hfdPic" runat="server" />
        <f:HiddenField ID="hfdPic2" runat="server" />
        <f:HiddenField ID="hfdPic3" runat="server" />
        <f:HiddenField ID="hfdPic4" runat="server" />
    </form>
    <%--    <script>
        var imgBMPPATH = '<%= imgBMPPATH.ClientID%>';
        F.ready(function () {
            $('#' + imgBMPPATH).on('click', function (e) { window.open($(e.currentTarget).find('img').eq(0).attr('src')) });
        });
    </script>--%>
</body>
</html>
