<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DrugAgainPrint.aspx.cs" Inherits="ERPProject.ERPApply.DrugAgainPrint" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>非定数条码重打</title>
    <script src="../res/js/CreateControl.js" type="text/javascript"></script>
    <style type="text/css" media="all">
        .x-grid-row.highlight td {
            background-color: lightpink;
            background-image: none;
        }
    </style>
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
        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBox"
            ShowHeader="false">
            <Toolbars>
                <f:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                        <f:Button ID="btnPrint" Icon="Printer" Text="打 印" runat="server" OnClick="btnPrint_Click" EnableDefaultState="false" />
                        <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" OnClick="btnExport_Click" ConfirmText="是否导出当前信息?" DisableControlBeforePostBack="false"
                            EnablePostBack="true" EnableAjax="false" EnableAjaxLoading="true" AjaxLoadingType="Mask" runat="server" EnableDefaultState="false" />
                        <f:ToolbarSeparator runat="server" />
                        <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" DisableControlBeforePostBack="true" runat="server" ValidateForms="FormUser" OnClick="btSearch_Click" EnableDefaultState="false" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="5px 10px 0px 10px"
                    ShowHeader="False" LabelWidth="75px" runat="server">
                    <Rows>
                        <f:FormRow>
                            <Items>
                                <f:TriggerBox ID="trbSearch" runat="server" ShowTrigger="false" Label="商品信息" OnTriggerClick="trbSearch_TriggerClick"></f:TriggerBox>
                                <f:TextBox ID="tbxBILL" runat="server" Label="出库单号" MaxLength="15" />
                                <f:DropDownList ID="ddlFlag" runat="server" Label="是否回收">
                                    <f:ListItem Text="---请选择---" Value="" Selected="true" />
                                    <f:ListItem Text="未回收" Value="N" />
                                    <f:ListItem Text="已回收" Value="Y" />
                                    <f:ListItem Text="已退货" Value="R" />
                                </f:DropDownList>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:DropDownList ID="ddlDEPTINT" runat="server" Label="使用科室" EnableEdit="true" ForceSelection="true" />
                                <f:DatePicker ID="dpkout1" runat="server" Label="出库日期" Required="true"></f:DatePicker>
                                <f:DatePicker ID="dpkout2" runat="server" Label="　　至" Required="true" CompareControl="dpkout1" CompareOperator="GreaterThanEqual" CompareMessage="结束日期应大于开始日期!"></f:DatePicker>
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
                <f:Grid ID="GridGoods" BoxFlex="1" ShowBorder="false" ShowHeader="false" EnableCheckBoxSelect="true" EnableMultiSelect="true" OnRowDataBound="GridGoods_RowDataBound"
                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableTextSelection="true" KeepCurrentSelection="true"
                    PageSize="50" DataKeyNames="BARCODE,FLAG" IsDatabasePaging="true" AllowPaging="true" OnPageIndexChange="GridGoods_PageIndexChange" EnableColumnLines="true">
                    <Columns>
                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                        <f:BoundField Width="120px" DataField="DEPTIN" HeaderText="使用科室" />
                        <f:BoundField Width="110px" DataField="Seqno" HeaderText="出库单号" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                        <f:BoundField Width="170px" DataField="gdname" HeaderText="商品名称" />
                        <f:BoundField Width="150px" DataField="gdspec" HeaderText="商品规格" />
                        <f:BoundField Width="50px" DataField="unitname" HeaderText="单位" TextAlign="Center" />
                        <f:BoundField Width="180px" ColumnID="BARCODE" DataField="BARCODE" HeaderText="条码" TextAlign="Center" />
                        <f:BoundField Width="190px" DataField="producername" HeaderText="生产厂家" />
                        <%--<f:BoundField Width="60px" DataField="FLAG" HeaderText="状态" Hidden="true" />--%>
                        <f:BoundField Width="60px" DataField="FLAGCH" HeaderText="状态" TextAlign="Center" />
                        <f:BoundField Width="120px" DataField="DEPTOUT" HeaderText="出库库房" />
                        <f:BoundField Width="80px" DataField="DHSL" HeaderText="数量" TextAlign="Right" />
                        <f:BoundField Width="90px" DataField="INS_TIME" HeaderText="出库日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
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
        <f:HiddenField ID="highlightRows" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdCurrent" runat="server" />
    </form>
    <script type="text/javascript">
        var GridGoods = '<%=GridGoods.ClientID%>';
        function getBARCODE() {

                var str2 = "";
                var data = F(GridGoods).data;
                var selected = F(GridGoods).getSelectedRows();
                for (var i = 0; i < data.length; i++) {
                    var values = data[i].values;
                    for (var j = 0; j < selected.length; j++) {
                        if (data[i].id == selected[j]) {
                            str2 += values["BARCODE"]+",";
                        }
                    }
                }
                str2 = str2.substring(0, str2.length - 1)
                return str2;
        }
       function OnBARCODEPrintEnd() {
            //打印数据已经成功发送到打印机数据缓冲池触发本事件
           var billNo = getBARCODE();
            var lry = F('<%= hfdCurrent.ClientID%>').getValue();
            $.post("/captcha/PrintReport.aspx?Method=PrintNum", { seqno: billNo, user: lry, oper: 'P' });
        }
        function PrintClick() {
            var No = F('<%= echo.ClientID%>').getValue();
            if (No == "") {
                F.alert("请选择需要重新打印的条码！");
                return;
            }
            var Report = ReportViewer.Report;
            Report.OnPrintEnd = OnBARCODEPrintEnd();
            ReportViewer.ReportURL = "/grf/F_barcode_echo.grf";
            var dataurl = "/captcha/PrintReport.aspx?Method=GetData_echo&osid=" + No;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
      
        var highlightRowsClientID = '<%= highlightRows.ClientID %>';
        var gridClientID = '<%= GridGoods.ClientID %>';
        function highlightRows() {
            // 增加延迟，等待HiddenField更新完毕
            window.setTimeout(function () {
                var highlightRows = F(highlightRowsClientID);
                var grid = F(gridClientID);
                $(grid.el.dom).find('.x-grid-row.highlight').removeClass('highlight');
                $.each(highlightRows.getValue().split(','), function (index, item) {
                    if (item !== '') {
                        var row = grid.getView().getNode(parseInt(item, 10));
                        $(row).addClass('highlight');
                    }
                });
            }, 100);
        }
    </script>
</body>
</html>