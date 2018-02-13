﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dx_Search.aspx.cs" Inherits="ERPProject.ERPQuery.Dx_Search" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>商品动销查询</title>    
    <script src="../res/js/CreateControl.js" type="text/javascript"></script>
    <script src="../res/js/GRInstall.js" type="text/javascript"></script>
</head>
<body>
    <div style="width: 0px; height: 0px">
        <script type="text/javascript">
            CreateDisplayViewerEx("0%", "0%", "", "", false, "");
        </script>
    </div>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="TabStrip1" runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="1" runat="server">
            <Tabs>
                <f:Tab Title="商品动销明细" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel1" runat="server" ShowBorder="False" BodyPadding="0px" Layout="Region"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="PanelBody" ShowBorder="false" BodyPadding="0px" RegionPosition="Top" Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar1" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText" runat="server" Text="操作信息："></f:ToolbarText>
                                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                <f:Button ID="btnPrt" Icon="Printer" Text="打 印" EnableDefaultState="false" EnablePostBack="false" runat="server" OnClientClick="btn_Bill()" />
                                                <f:Button ID="btnExpect" runat="server" Icon="DatabaseGo" EnableAjax="false" EnableDefaultState="false"
                                                    OnClick="btnExp_Click" EnablePostBack="true" Text="导出" />
                                                <f:Button ID="btnAudit" Icon="ArrowRotateClockwise" Text="清 空" Hidden="true" EnablePostBack="true" runat="server" EnableDefaultState="false" />
                                                <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" runat="server" OnClick="btSearch_Click" EnableDefaultState="false" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                </f:Panel>
                                <f:Panel ID="Panel2" BodyPadding="0px" RegionSplit="true" EnableCollapse="true" RegionPosition="Top" ShowBorder="false" Layout="Anchor" ShowHeader="False" runat="server">
                                    <Items>
                                        <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="10px 0px 5px 10px" ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TextBox ID="txbGDSEQ" runat="server" Label="商品信息" EmptyText="可输入编码、名称或助记码" />
                                                        <f:DropDownList ID="ddlDEPTID" runat="server" Label="科室" EnableEdit="true" ForceSelection="true"></f:DropDownList>
                                                        <f:DropDownList ID="ddlGZ" runat="server" Label="商品类型">
                                                            <f:ListItem Text="--请选择--" Value="" />
                                                            <f:ListItem Text="高值" Value="Y" Selected="true" />
                                                            <f:ListItem Text="非高值" Value="N" />
                                                        </f:DropDownList>
                                                        <f:DatePicker ID="dpkDATE1" runat="server" Label="使用日期" Required="true" ShowRedStar="true"></f:DatePicker>
                                                        <f:DatePicker ID="dpkDATE2" runat="server" Label=" 至 " Required="true" ShowRedStar="true"></f:DatePicker>
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="grdList" runat="server" ShowBorder="false" ShowHeader="false" AllowColumnLocking="false" EnableSummary="true" SummaryPosition="Bottom"
                                    AllowSorting="false" AutoScroll="true" SortField="RQSJ" SortDirection="DESC" DataKeyNames="GDSEQ"
                                    IsDatabasePaging="true" AllowPaging="true" PageSize="100" EnableColumnLines="true" EnableTextSelection="true">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" EnableLock="true" Locked="true" />
                                        <f:BoundField Width="100px" DataField="DEPTNAME" SortField="DEPTNAME" HeaderText="使用科室" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                        <f:BoundField Width="110px" DataField="RQSJ" SortField="RQSJ" HeaderText="单据时间" DataFormatString="{0:yyyy-MM-dd}" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                        <f:BoundField Width="120px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                        <f:BoundField Width="150px" DataField="GDNAME" ColumnID="GDNAME" SortField="GDNAME" HeaderText="商品名称" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                        <f:BoundField Width="150px" DataField="GDSPEC" SortField="GDSPEC" HeaderText="商品规格" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                        <f:BoundField Width="50px" DataField="UNITNAME" SortField="UNITNAME" HeaderText="单位" TextAlign="Center" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                        <f:BoundField Width="80px" DataField="HSJJ" SortField="HSJJ" HeaderText="单价" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                        <f:BoundField Width="80px" DataField="SL" SortField="SL" ColumnID="SL" HeaderText="数量" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                        <f:BoundField Width="80px" DataField="HSJE" SortField="HSJE" ColumnID="HSJE" HeaderText="金额" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                        <f:BoundField Width="80px" DataField="PH" SortField="PH" HeaderText="批号" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                        <f:BoundField Width="110px" DataField="YXQZ" SortField="YXQZ" HeaderText="有效期至" DataFormatString="{0:yyyy-MM-dd}" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                        <f:BoundField Width="110px" DataField="RQ_SC" SortField="RQ_SC" HeaderText="生产日期" DataFormatString="{0:yyyy-MM-dd}" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                        <f:BoundField Width="120px" DataField="PRODUCERNAME" SortField="PRODUCERNAME" HeaderText="生产厂家" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="商品动销汇总" Icon="PageWord" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel3" runat="server" ShowBorder="False" BodyPadding="0px" Layout="Region"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="Panel4" ShowBorder="false" BodyPadding="0px" RegionPosition="Top" Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar2" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText1" runat="server" Text="操作信息："></f:ToolbarText>
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="btnPrint" Icon="Printer" Text="打 印" EnableDefaultState="false" EnablePostBack="false" runat="server" OnClientClick="btnPrint_Bill()" />
                                                <f:Button ID="btnExp" runat="server" Icon="DatabaseGo" EnableAjax="false" EnableDefaultState="false"
                                                    OnClick="btnExpect_Click" EnablePostBack="true" Text="导出" />
                                                <f:Button ID="btnClear" Icon="ArrowRotateClockwise" Text="清 空" Hidden="true" OnClick="btnClear_Click" EnablePostBack="true" runat="server" EnableDefaultState="false" />
                                                <f:Button ID="btnSearch" Icon="Magnifier" Text="查 询" OnClick="btnSearch_Click" runat="server" EnableDefaultState="false" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                </f:Panel>
                                <f:Panel ID="Panel5" BodyPadding="0px" RegionSplit="true" EnableCollapse="true" RegionPosition="Top" ShowBorder="false" Layout="Anchor" ShowHeader="False" runat="server">
                                    <Items>
                                        <f:Form ID="Form2" ShowBorder="false" AutoScroll="false" BodyPadding="10px 0px 5px 10px" ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow ColumnWidths="25% 25% 25% 25%">
                                                    <Items>
                                                        <f:TriggerBox ID="tbxGDSEQ" MaxLength="20" runat="server" Label="商品信息" EmptyText="可输入编码、名称或助记码" ShowTrigger="false" OnTriggerClick="btnSearch_Click"></f:TriggerBox>
                                                        <f:DropDownList ID="schDEPTID" runat="server" Label="科室" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="isGZ" runat="server" Label="商品类型">
                                                            <f:ListItem Text="--请选择--" Value="" />
                                                            <f:ListItem Text="高值" Value="Y" Selected="true" />
                                                            <f:ListItem Text="非高值" Value="N" />
                                                        </f:DropDownList>
                                                        <f:DatePicker ID="dpkTime1" runat="server" Label="使用日期" Required="true" ShowRedStar="true"></f:DatePicker>
                                                        <f:DatePicker ID="dpkTime2" runat="server" Label=" 至 " Required="true" ShowRedStar="true"></f:DatePicker>
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="gidGoods" runat="server" ShowBorder="false" ShowHeader="false" AllowColumnLocking="false" EnableRowDoubleClickEvent="true" OnRowDoubleClick="gidGoods_RowDoubleClick"
                                    AllowSorting="true" AutoScroll="true" OnSort="gidGoods_Sort" SortField="DEPTID" SortDirection="DESC" DataKeyNames="GDSEQ,DEPTID" EnableSummary="true" SummaryPosition="Bottom"
                                    IsDatabasePaging="true" AllowPaging="true" PageSize="100" OnPageIndexChange="gidGoods_PageIndexChange" EnableColumnLines="true" EnableTextSelection="true" IsDatabaseSorting="true">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" EnableLock="true" Locked="true" />
                                        <f:BoundField Width="100px" DataField="DEPTNAME" SortField="DEPTID" HeaderText="部门" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="true" />
                                        <f:BoundField Width="120px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="true" />
                                        <f:BoundField Width="150px" DataField="GDNAME" SortField="GDNAME" HeaderText="商品名称" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="true" />
                                        <f:BoundField Width="150px" DataField="GDSPEC" ColumnID="GDSPEC" SortField="GDSPEC" HeaderText="商品规格" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="true" />
                                        <f:BoundField Width="50px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="true" />
                                        <f:BoundField Width="80px" DataField="HSJJ" SortField="HSJJ" HeaderText="单价" TextAlign="Center" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="true" />
                                        <f:BoundField Width="80px" DataField="RKSL" ColumnID="RKSL" SortField="RKSL" HeaderText="入库数量" TextAlign="Right" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                        <f:BoundField Width="80px" DataField="RKJE" ColumnID="RKJE" SortField="RKJE" HeaderText="入库金额" TextAlign="Right" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                        <f:BoundField Width="80px" DataField="CKSL" ColumnID="CKSL" SortField="CKSL" HeaderText="出库数量" TextAlign="Right" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                        <f:BoundField Width="80px" DataField="CKJE" ColumnID="CKJE" SortField="CKJE" HeaderText="出库金额" TextAlign="Right" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                        <f:BoundField Width="80px" DataField="THSL" ColumnID="THSL" SortField="THSL" HeaderText="退货数量" TextAlign="Right" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                        <f:BoundField Width="80px" DataField="THJE" ColumnID="THJE" SortField="THJE" HeaderText="退货金额" TextAlign="Right" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                        <f:BoundField Width="80px" DataField="KCSL" ColumnID="KCSL" SortField="KCSL" HeaderText="库存数量" TextAlign="Right" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                        <f:BoundField Width="80px" DataField="KCJE" ColumnID="KCJE" SortField="KCJE" HeaderText="库存金额" TextAlign="Right" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                        <f:BoundField Width="120px" DataField="PRODUCERNAME" SortField="PRODUCERNAME" HeaderText="生产厂家" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
        <f:HiddenField runat="server" ID="hfdUSERID"></f:HiddenField>
        <f:HiddenField runat="server" ID="hfddpkTime1"></f:HiddenField>
        <f:HiddenField runat="server" ID="hfddpkTime2"></f:HiddenField>
        <f:HiddenField runat="server" ID="hfddpkDATE1"></f:HiddenField>
        <f:HiddenField runat="server" ID="hfddpkDATE2"></f:HiddenField>
    </form>
    <script type="text/javascript">
        function btnPrint_Bill() {
            var GrfGDSEQ = F('<%= tbxGDSEQ.ClientID%>').getValue();
            var GrfDEPTID = F('<%= schDEPTID.ClientID%>').getValue();
            var GrfisGZ = F('<%= isGZ.ClientID%>').getValue();
            var GrfdpkTime1 = F('<%= dpkTime1.ClientID%>').value;
            var GrfdpkTime2 = F('<%= dpkTime2.ClientID%>').value;
            var GrfUserId = F('<%= hfdUSERID.ClientID%>').getValue();

            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "/grf/GoodsDxHZ.grf?time=20160725";
            var dataurl = "/captcha/PrintReport.aspx?Method=GetGoodsDX_HZ&gdseq=" + GrfGDSEQ + "&deptid=" + GrfDEPTID + "&isgz=" + GrfisGZ + "&dpkTime1=" + GrfdpkTime1 + "&dpkTime2=" + GrfdpkTime2 + "&userid=" + GrfUserId;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
        function btn_Bill() {
            var GrfGDSEQ = F('<%= txbGDSEQ.ClientID%>').getValue();
            var GrfDEPTID = F('<%= ddlDEPTID.ClientID%>').getValue();
            var GrfisGZ = F('<%= ddlGZ.ClientID%>').getValue();
            var GrfdpkTime1 = F('<%= dpkDATE1.ClientID%>').value;
            var GrfdpkTime2 = F('<%= dpkDATE2.ClientID%>').value;
            var GrfUserId = F('<%= hfdUSERID.ClientID%>').getValue();

            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "/grf/GoodsDxMX.grf?time=20160725";
            var dataurl = "/captcha/PrintReport.aspx?Method=GetGoodsDX_MX&gdseq=" + GrfGDSEQ + "&deptid=" + GrfDEPTID + "&isgz=" + GrfisGZ + "&dpkTime1=" + GrfdpkTime1 + "&dpkTime2=" + GrfdpkTime2 + "&userid=" + GrfUserId;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
    </script>
</body>
</html>