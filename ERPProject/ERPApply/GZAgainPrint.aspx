﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GZAgainPrint.aspx.cs" Inherits="ERPProject.ERPApply.GZAgainPrint" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>高值条码管理</title>
    <script src="../res/js/CreateControl.js" type="text/javascript"></script>
</head>
<body>
    <div style="width: 0px; height: 0px">
        <script type="text/javascript">
            CreateDisplayViewerEx("0%", "0%", "", "", false, "");
        </script>
    </div>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="Panel1"
            runat="server" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
            ShowHeader="false">
            <Toolbars>
                <f:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                        <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="操作信息：查询并打印高值条码！" runat="server" />
                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                        <f:Button ID="btnPrint" Icon="Printer" Text="打 印" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnPrint_Click" />
                        <f:ToolbarSeparator runat="server"></f:ToolbarSeparator>
                        <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" EnableDefaultState="false" DisableControlBeforePostBack="true" runat="server" ValidateForms="FormUser" OnClick="btSearch_Click" />
                        <f:ToolbarSeparator runat="server" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="5px 10px 0px 10px"
                    ShowHeader="False" LabelWidth="75px" runat="server">
                    <Rows>
                        <f:FormRow>
                            <Items>
                                <f:TriggerBox ID="tbxGDSEQ" runat="server" Label="商品信息" ShowTrigger="false" MaxLength="20" EmptyText="请输入商品编码或商品名称" TriggerIcon="Search" OnTriggerClick="btSearch_Click"></f:TriggerBox>
                                <f:TriggerBox ID="tgbCODE" runat="server" Label="追溯码" ShowTrigger="false" MaxLength="36" EmptyText="请输入高值追溯码" TriggerIcon="Search" OnTriggerClick="btSearch_Click"></f:TriggerBox>
                                <f:DropDownList ID="ddlFLAG" runat="server" Label="状态">
                                    <f:ListItem Text="--请选择" Value="" />
                                    <f:ListItem Text="已退货" Value="T" />
                                    <f:ListItem Text="未入库" Value="N" />
                                    <f:ListItem Text="已入库" Value="Y" />
                                    <f:ListItem Text="已出库" Value="C" />
                                    <f:ListItem Text="科室退货" Value="R" />
                                    <f:ListItem Text="已使用" Value="G" />
                                    <f:ListItem Text="已损益" Value="S" />
                                </f:DropDownList>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:DropDownList ID="lstDEPTID" runat="server" Label="使用部门" EnableEdit="true" ForceSelection="true" />
                                <f:DatePicker ID="dpkout1" runat="server" Label="生成日期" Required="true"></f:DatePicker>
                                <f:DatePicker ID="dpkout2" runat="server" Label="至" Required="true" CompareControl="dpkout1" CompareOperator="GreaterThanEqual" CompareMessage="结束日期应大于开始日期!"></f:DatePicker>
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
                <f:Grid ID="GridGoods" AnchorValue="100% -62" ShowBorder="false" ShowHeader="false" EnableCheckBoxSelect="true" EnableMultiSelect="true" EnableRowSelectEvent="true" OnRowDataBound="GridGoods_RowDataBound"
                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableColumnLines="true" EnableTextSelection="true"
                    PageSize="50" DataKeyNames="ONECODE,FLAG" IsDatabasePaging="true" AllowPaging="true" KeepCurrentSelection="true" OnPageIndexChange="GridGoods_PageIndexChange">
                    <Columns>
                        <f:RowNumberField Width="35px" ColumnID="Sel" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                        <f:BoundField Width="80px" DataField="FLAG" HeaderText="状态" TextAlign="Center" Hidden="true" />
                        <f:BoundField Width="100px" DataField="BILLNO" HeaderText="订货单号" TextAlign="Center" Hidden="true" />
                        <f:BoundField Width="100px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                        <f:BoundField Width="180px" DataField="GDNAME" HeaderText="商品名称" />
                        <f:BoundField Width="100px" DataField="GDSPEC" HeaderText="商品规格" />
                        <f:BoundField Width="50px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="HSJJ" HeaderText="价格" TextAlign="Right" DataFormatString="{0:F4}" />
                        <f:BoundField Width="150px" DataField="PRODUCERNAME" HeaderText="生产厂家" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="FLAGNAME" ColumnID="FLAGNAME" HeaderText="状态" TextAlign="Center" />
                        <f:BoundField Width="260px" ColumnID="ONECODE" DataField="ONECODE" HeaderText="高值追溯码" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="DEPTCURNAME" HeaderText="当前部门" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="BZHL" HeaderText="包装含量" TextAlign="Center" />
                        <f:BoundField Width="130px" DataField="BAR3" HeaderText="ERP编码" TextAlign="Center" />
                        <f:BoundField Width="130px" DataField="PH" HeaderText="批号" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="RQ_SC" HeaderText="生产日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="100px" DataField="YXQZ" HeaderText="有效期至" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
        <f:HiddenField ID="bcdConstant" runat="server"></f:HiddenField>
        <f:HiddenField ID="mbxCartNo" runat="server"></f:HiddenField>
        <f:HiddenField ID="mbxDept" runat="server"></f:HiddenField>
        <f:HiddenField ID="mbxGDNAME" runat="server"></f:HiddenField>
        <f:HiddenField ID="mbxGDSPEC" runat="server"></f:HiddenField>
        <f:HiddenField ID="mbxQuantity" runat="server"></f:HiddenField>
        <f:HiddenField ID="mbxProducer" runat="server"></f:HiddenField>
        <f:HiddenField ID="echo" runat="server"></f:HiddenField>
        <f:HiddenField ID="hdfIndex" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdCurrent" runat="server" />
    </form>
    <script type="text/javascript">
        var GridGoods = '<%=GridGoods.ClientID%>';
        function getStr2s() {
            var str2 = "";
            var data = F(GridGoods).data;
            var selected = F(GridGoods).getSelectedRows();
            for (var i = 0; i < data.length; i++) {
                var values = data[i].values;
                for (var j = 0; j < selected.length; j++) {
                    if (data[i].id == selected[j]) {
                        str2 += values["ONECODE"]+",";
                    }
                }  
            }
            str2 = str2.substring(0, str2.length - 1)
            return str2;
        }
       function OnStr2PrintEnd() {
            //打印数据已经成功发送到打印机数据缓冲池触发本事件
           var billNo = getStr2s();
            var lry = F('<%= hfdCurrent.ClientID%>').getValue();
            $.post("/captcha/PrintReport.aspx?Method=PrintNum", { seqno: billNo, user: lry, oper: 'P' });
        }
        function btnPrint_onclick() {
            var billNo = F('<%= echo.ClientID%>').getValue();
            if (billNo == "") {
                F.alert("请选择要打印的追溯码信息！");
                return;
            }
            var Report = ReportViewer.Report;
            Report.OnPrintEnd = OnStr2PrintEnd();
            ReportViewer.ReportURL = "/grf/TraceCode.grf?timestamp=" + new Date().getTime();
            var dataurl = "/captcha/PrintReport.aspx?Method=GetData_GZ&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
    </script>
</body>
</html>