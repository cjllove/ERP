﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StockSearch.aspx.cs" Inherits="ERPProject.ERPQuery.StockSearch" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title>商品库存查询</title>
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
        <f:pagemanager id="PageManager1" autosizepanelid="Panel1" runat="server" />
        <f:panel id="Panel1" runat="server" showborder="false" bodypadding="0px" layout="Anchor"
            showheader="false">
            <Items>
                <f:Panel ID="PanelBody" ShowBorder="false" BodyPadding="0px"
                    Layout="Anchor" ShowHeader="False" runat="server">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar1" runat="server">
                            <Items>
                                <f:CheckBox runat="server" ID="chkISPHALL" Text="合并商品批号" AutoPostBack="true" Checked="true" OnCheckedChanged="chkISPHALL_CheckedChanged"></f:CheckBox>
                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                <f:Button ID="btnAudit" Icon="ArrowRotateClockwise" Text="清 空" OnClick="btClear_Click" EnablePostBack="true" runat="server" EnableDefaultState="false" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="BtnPrintKC" Icon="Printer" Text="打印库存" EnablePostBack="true" runat="server" OnClientClick="btnPrint_BillKC()" EnableDefaultState="false" />
                                <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" OnClick="btExport_Click" ConfirmText="是否导出当前商品库存信息?" DisableControlBeforePostBack="false"
                                    EnablePostBack="true" EnableAjax="false" EnableAjaxLoading="true" AjaxLoadingType="Mask" runat="server" EnableDefaultState="false" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" OnClick="btSearch_Click" runat="server" EnableDefaultState="false" />
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Items>
                        <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="5px"
                            ShowHeader="False" LabelWidth="80px" runat="server">
                            <Rows>
                                <f:FormRow ColumnWidths="25% 25% 25% 25% ">
                                    <Items>
                                        <f:DropDownList ID="ddlDEPTID" runat="server" Label="库房/科室" EnableEdit="true" ForceSelection="true">
                                        </f:DropDownList>
                                        <f:TriggerBox ID="tbxGOODS" runat="server" Label="商品信息" EmptyText="可输入编码、名称或助记码" ShowTrigger="false" TriggerIcon="Search" OnTriggerClick="btSearch_Click"></f:TriggerBox>
                                        <f:DropDownList ID="ddlSHSID" runat="server" Label="送货商" EnableEdit="true" ForceSelection="true">
                                        </f:DropDownList>
                                        <%--<f:DropDownList ID="ddlSUPID" runat="server" Label="供应商" EnableEdit="true" ForceSelection="true">
                                        </f:DropDownList>
                                        <f:DropDownList ID="ddlPSSID" runat="server" Label="配送商" EnableEdit="true" ForceSelection="true">
                                        </f:DropDownList>--%>
                                    </Items>
                                </f:FormRow>
                                <f:FormRow ColumnWidths="25% 25% 25% 12% 13% ">
                                    <Items>
                                        <f:TriggerBox ID="tbxHWID" runat="server" Label="货位" EmptyText="请输入货位ID" MaxLength="20" TriggerIcon="Search" ShowTrigger="false" OnTriggerClick="btSearch_Click"></f:TriggerBox>
                                        <f:TriggerBox ID="tbxPHID" runat="server" Label="批号信息" EmptyText="请输入批次号" ShowTrigger="false" TriggerIcon="Search" OnTriggerClick="btSearch_Click"></f:TriggerBox>
                                        <%--<f:TriggerBox ID="tbxBILLNO" runat="server" Label="入库单号" EmptyText="可模糊输入入库单号" MaxLength="20" ShowTrigger="false" TriggerIcon="Search" OnTriggerClick="btSearch_Click"></f:TriggerBox>--%>
                                        <f:DropDownList ID="ddlAll" runat="server" Label="显示模式">
                                            <f:ListItem Text="显示所有" Value="" />
                                            <f:ListItem Text="有库存信息" Value="N" Selected="true" />
                                        </f:DropDownList>

                                    </Items>
                                </f:FormRow>
                                <f:FormRow>
                                    <Items>
                                        <f:DropDownList runat="server" ID="ddlISFLAG7" Label="商品类型">
                                            <f:ListItem Text="-- 全部 --" Value="" />
                                            <f:ListItem Text="下传商品" Value="N" />
                                            <f:ListItem Text="本地商品" Value="Y" />
                                        </f:DropDownList>
                                        <f:DropDownList ID="ddlISDG" runat="server" Label="商品模式">
                                            <f:ListItem Text="--请选择--" Value="" Selected="true" />
                                            <f:ListItem Text="托管" Value="0" />	 
                                            <f:ListItem Text="直供" Value="Z" />
                                        </f:DropDownList>
                                        <f:DropDownList ID="ddlISGZ" runat="server" Label="高值">
                                            <f:ListItem Text="--请选择--" Value="" />
                                            <f:ListItem Text="是" Value="Y" />
                                            <f:ListItem Text="否" Value="N" />
                                        </f:DropDownList>
                                    </Items>
                                </f:FormRow>
                                <f:FormRow Hidden="true">
                                    <Items>
                                        <f:HiddenField ID="USERID" runat="server" />
                                    </Items>
                                </f:FormRow>
                            </Rows>
                        </f:Form>
                    </Items>
                </f:Panel>
                <f:Grid ID="GridGoods" AnchorValue="100% -140" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;" OnRowCommand="GridGoods_RowCommand"
                    PageSize="100" IsDatabasePaging="true" AllowPaging="true" OnPageIndexChange="GridGoods_PageIndexChange" EnableColumnLines="true" EnableTextSelection="true" AllowColumnLocking="true"
                    AllowSorting="true" AutoScroll="true" runat="server" DataKeyNames="GDSEQ,LOCKSL,DEPTID,PHID,PICINO" OnSort="GridGoods_Sort" SortField="GDNAME" SortDirection="ASC"
                    EnableSummary="true" SummaryPosition="Bottom">
                    <Columns>
                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" EnableLock="true" Locked="true" />
                        <f:BoundField Width="130px" DataField="DEPTIDNAME" SortField="DEPTIDNAME" HeaderText="存货地点" EnableLock="true" Locked="true" />
                        <f:BoundField Width="105px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" TextAlign="Center" EnableLock="true" Locked="true" />
                        <f:BoundField Width="180px" DataField="GDNAME" SortField="GDNAME" ColumnID="GDNAME" HeaderText="商品名称" EnableLock="true" Locked="true" />
                        <f:BoundField Width="100px" DataField="GDSPEC" SortField="GDSPEC" HeaderText="规格·容量" />
                        <f:BoundField Width="50px" DataField="UNIT" SortField="UNIT" HeaderText="单位" TextAlign="Center" />
                        <f:BoundField Width="60px" DataField="BZHL" Hidden="true" HeaderText="包装含量" />
                        <f:BoundField Width="70px" DataField="HWID" SortField="HWID" ColumnID="HWID" HeaderText="货位" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="KCSL" SortField="KCSL" ColumnID="KCSL" HeaderText="库存数" TextAlign="Right" />
                        <%--<f:BoundField Width="80px" DataField="LOCKSL" SortField="LOCKSL" ColumnID="LOCKSL" HeaderText="预占库存数" TextAlign="Right" />--%>
                        <f:LinkButtonField Width="100px" DataTextField="LOCKSL" SortField="LOCKSL" ColumnID="LOCKSL" HeaderText="预占库存数" TextAlign="Right" EnableAjaxLoading="false" CommandName="FuncAction"></f:LinkButtonField>
                        <f:BoundField Width="90px" DataField="HSJJ" SortField="HSJJ" HeaderText="含税进价" DataFormatString="{0:F4}" TextAlign="Right" />
                        <f:BoundField Width="90px" DataField="HSJE" SortField="HSJE" ColumnID="HSJE" HeaderText="含税金额" DataFormatString="{0:F2}" TextAlign="Right" />
                        <f:BoundField Width="60px" DataField="IERP_CN" SortField="IERP_CN" TextAlign="center" HeaderText="盘点" />
                        <f:BoundField Width="60px" DataField="ISJF_CN" SortField="ISJF_CN" TextAlign="center" HeaderText="计费" />
                        <f:BoundField Width="60px" DataField="ISCFG_CN" SortField="ISCFG_CN" TextAlign="center" HeaderText="科室使用" />
                        <f:BoundField Width="100px" DataField="ZPBH" SortField="ZPBH" HeaderText="制品编号" Hidden="true" />
                        <f:BoundField Width="120px" DataField="BILLNO" SortField="BILLNO" ColumnID="BILLNO" HeaderText="单据编号" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="PHID" ColumnID="PHID" SortField="PHID" HeaderText="批号" />
                        <f:BoundField Width="80px" DataField="RQ_SC" ColumnID="RQ_SC" SortField="RQ_SC" HeaderText="生产日期" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="80px" DataField="YXQZ" ColumnID="YXQZ" SortField="YXQZ" HeaderText="效期" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="120px" DataField="BAR3" SortField="BAR3" HeaderText="ERP编码" />
                        <f:BoundField Width="120px" DataField="CATID" SortField="CATID" HeaderText="商品分类" TextAlign="Center" />
                        <f:BoundField Width="170px" DataField="PRODUCERNAME" SortField="PRODUCERNAME" HeaderText="生产厂家" />
                        <f:BoundField Width="170px" DataField="PIZNO" ColumnID="PIZNO" SortField="PIZNO" HeaderText="注册证号" />
                        <f:BoundField Width="170px" DataField="PSSID" ColumnID="PSSID" SortField="PSSID" HeaderText="配送商" />
                        <f:BoundField Width="170px" DataField="SUPID" ColumnID="SUPID" SortField="SUPID" HeaderText="供应商" />
                        <f:BoundField Width="60px" DataField="ISDG" SortField="ISDG" TextAlign="center" HeaderText="代管" Hidden="true" />
                        <f:BoundField Width="60px" DataField="ISGZ" SortField="ISGZ" TextAlign="center" HeaderText="高值" Hidden="false" />

                        <f:BoundField Width="60px" DataField="ISFLAG7_CN" SortField="ISFLAG7_CN" HeaderText="商品类型" />
                        <f:BoundField Width="60px" DataField="DEPTID" SortField="DEPTID" HeaderText="科室编码" Hidden="true" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:panel>
        <f:hiddenfield runat="server" id="hfdISNOPH"></f:hiddenfield>
        <f:window id="WinYz" title="商品预占信息" hidden="true" enableiframe="false" runat="server"
            enablemaximize="true" enableresize="true" target="Parent" ismodal="True" layout="Fit" width="700px" height="360px">
            <Items>
                <f:Grid ID="GridYz" ShowBorder="false" ShowHeader="false" EnableColumnLines="true" AllowSorting="false" IsDatabasePaging="true" OnPageIndexChange="GridYz_PageIndexChange" PageSize="50" AllowPaging="true" AutoScroll="true" runat="server" EnableTextSelection="true">
                    <Columns>
                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                        <f:BoundField Width="115px" DataField="LOCKBILLNO" HeaderText="单据编号" TextAlign="Center" />
                        <f:BoundField Width="120px" DataField="DEPTIDNAME" SortField="DEPTIDNAME" HeaderText="科室名称" TextAlign="Center" />
                        <f:BoundField Width="40px" DataField="LOCKROWNO" HeaderText="行号" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                        <f:BoundField Width="180px" DataField="GDNAME" HeaderText="商品名称" />
                        <f:BoundField Width="100px" DataField="GDSPEC" HeaderText="规格·容量" />
                        <f:BoundField Width="50px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="LOCKKCSL" HeaderText="预占库存数" TextAlign="Right" />
                        <f:BoundField Width="70px" DataField="HSJJ" HeaderText="含税进价" DataFormatString="{0:F4}" TextAlign="Right" />
                        <f:BoundField Width="80px" DataField="HSJE" HeaderText="含税金额" DataFormatString="{0:F2}" TextAlign="Right" />
                        <f:BoundField Width="120px" DataField="BAR3" HeaderText="ERP编码" />
                        <f:BoundField Width="170px" DataField="PRODUCERNAME" HeaderText="生产厂家" />
                        <f:BoundField Width="170px" DataField="PIZNO" HeaderText="注册证号" />
                    </Columns>
                </f:Grid>
                <f:HiddenField ID="hdfGdseq" runat="server" />
                <f:HiddenField ID="hdfDept" runat="server" />
                <f:HiddenField ID="hdfPhid" runat="server" />
                <f:HiddenField ID="hdfPicino" runat="server" />
            </Items>
        </f:window>
    </form>
    <%--<script type="text/javascript" src="../res/js/jquery-1.11.1.min.js"></script>
    <script type="text/javascript" src="../res/js/jquery-ui.min.js"></script>--%>
    <script language="javascript" type="text/javascript">
        function btnPrint_BillKC() {
            var GrfQBKC = "";
            var GrfKSKF = F('<%= ddlDEPTID.ClientID%>').getValue();
            var GrfGOODS = F('<%= tbxGOODS.ClientID%>').getValue();
<%--            var GrfGYS = F('<%= ddlSUPID.ClientID%>').getValue();
            var GrfSCCJ = F('<%= ddlPSSID.ClientID%>').getValue();--%>
            var GrfSCCJ = F('<%= ddlSHSID.ClientID%>').getValue();
            var GrfHWID = F('<%= tbxHWID.ClientID%>').getValue();
            var GrfPHID = F('<%= tbxPHID.ClientID%>').getValue();
            var GrfRKDH = '';
            var GrfISDG = F('<%= ddlISDG.ClientID%>').getValue(); <%--<%=getISDG()%>;--%>
            var GrfUSER = F('<%= USERID.ClientID%>').getValue();
            var GrfISNOPH = F('<%= hfdISNOPH.ClientID%>').getValue();

            if (GrfISNOPH == "N") {
                var Report = ReportViewer.Report;
                ReportViewer.ReportURL = "/grf/SPKCXX.grf?verson=2";
                var dataurl = "/captcha/PrintReport.aspx?Method=GetGoodsStock&QBKC=" + GrfQBKC + "&KSKF=" + GrfKSKF + "&GOODS=" + GrfGOODS + "&SCCJ=" + GrfSCCJ + "&HWID=" + GrfHWID + "&PHID=" + GrfPHID + "&RKDH=" + GrfRKDH + "&ISDG=" + GrfISDG + "&USER=" + GrfUSER;
                ReportViewer.Report.LoadDataFromURL(dataurl);
                var print = getCookie("WEGOERP_PRINT_SPKCXX");
                if (print != null && print != "") {
                    setCookie("WEGOERP_PRINT_SPKCXX", print);
                    ReportViewer.Report.Printer.PrinterName = print;
                }
                ReportViewer.Start();
                ReportViewer.Report.PrintPreview(true);
                ReportViewer.Stop();
            } else {
                var Report = ReportViewer.Report;
                ReportViewer.ReportURL = "/grf/SPKCXX_NOPH.grf?verson=2";
                var dataurl = "/captcha/PrintReport.aspx?Method=GetGoodsStock_NoPh&QBKC=" + GrfQBKC + "&KSKF=" + GrfKSKF + "&GOODS=" + GrfGOODS + "&RKDH=" + GrfRKDH + "&ISDG=" + GrfISDG + "&USER=" + GrfUSER;
                ReportViewer.Report.LoadDataFromURL(dataurl);
                var print = getCookie("WEGOERP_PRINT_SPKCXX_NOPH");
                if (print != null && print != "") {
                    setCookie("WEGOERP_PRINT_SPKCXX_NOPH", print);
                    ReportViewer.Report.Printer.PrinterName = print;
                }
                ReportViewer.Start();
                ReportViewer.Report.PrintPreview(true);
                ReportViewer.Stop();
            }

        }
    </script>
</body>
</html>
