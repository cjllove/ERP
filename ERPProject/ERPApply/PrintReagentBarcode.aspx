<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrintReagentBarcode.aspx.cs" Inherits="ERPProject.ERPApply.PrintReagentBarcode" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>试剂条码重打</title>
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
                        <f:ToolbarText ID="ToolbarText" runat="server" Text="操作信息：查询和重打未回收的试剂条码！"></f:ToolbarText>
                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                        <f:Button ID="btnPrint" Icon="Printer" Text="打 印" EnableDefaultState="false" EnablePostBack="false" runat="server" OnClientClick="btnPrint_onclick()" />
                        <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" EnableDefaultState="false" DisableControlBeforePostBack="true" runat="server" OnClick="btSearch_Click" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="5px 10px 0px 10px"
                    ShowHeader="False" LabelWidth="75px" runat="server">
                    <Rows>
                        <f:FormRow>
                            <Items>
                                <f:TextBox ID="tbxGDSEQ" runat="server" Label="商品信息" MaxLength="20" EmptyText="请输入商品编码或名称" />
                                <f:DropDownList ID="ddlDEPTINT" runat="server" Label="入库科室" EnableEdit="true" ForceSelection="true">
                                </f:DropDownList>
                                <f:DropDownList ID="ddlFLAG" runat="server" Label="状态">
                                    <f:ListItem Text="--请选择--" Value="" />
                                    <f:ListItem Text="未使用" Value="N" />
                                    <f:ListItem Text="已使用" Value="Y" />
                                    <f:ListItem Text="部分使用" Value="R" />
                                    <f:ListItem Text="作废" Value="F" />
                                </f:DropDownList>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:TextBox ID="tbxBILL" runat="server" Label="入库单号" EmptyText="入库单号信息" MaxLength="15" />
                                <f:DatePicker ID="dpkout1" runat="server" Label="入库日期" Required="true"></f:DatePicker>
                                <f:DatePicker ID="dpkout2" runat="server" Label="至" Required="true" CompareControl="dpkout1" CompareOperator="GreaterThanEqual" CompareMessage="结束日期应大于开始日期!"></f:DatePicker>
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
                <f:Grid ID="GridGoods" AnchorValue="100% -30" ShowBorder="false" ShowHeader="false" EnableCheckBoxSelect="true" EnableMultiSelect="true" EnableRowSelectEvent="true" EnableTextSelection="true"
                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" CheckBoxSelectOnly="true" EnableColumnLines="true"
                    PageSize="50" DataKeyNames="BARCODE" IsDatabasePaging="true" AllowPaging="true" OnRowSelect="GridGoods_RowSelect" OnRowCommand="GridGoods_RowCommand">
                    <Columns>
                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                        <f:LinkButtonField Text="[作废]" Width="60px" CommandName="actZF" ConfirmText="是否确认作废此试剂条码"></f:LinkButtonField>
                        <f:BoundField Width="120px" DataField="rkseqno" HeaderText="入库单号" TextAlign="Center" />
                        <f:BoundField Width="110px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                        <f:BoundField Width="150px" DataField="gdname" HeaderText="商品名称" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="gdspec" HeaderText="商品规格" TextAlign="Center" />
                        <f:BoundField Width="50px" DataField="unitname" HeaderText="单位" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="FLAGNAME" HeaderText="状态" TextAlign="Center" />
                        <f:BoundField Width="220px" DataField="BARCODE" HeaderText="试剂条码" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="SL" HeaderText="入库数量" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="NUM1" HeaderText="出库数量" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="NUM2" HeaderText="退货数量" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="PH" HeaderText="批次" TextAlign="Center" />
                        <f:BoundField Width="110px" DataField="RQ_SC" HeaderText="生产日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="110px" DataField="YXQZ" HeaderText="有效期至" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="110px" DataField="INRQ" HeaderText="入库日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="80px" DataField="RKY" HeaderText="操作人" TextAlign="Center" />
                        <f:BoundField Width="120px" DataField="OUTBILLNO" HeaderText="出库单号" TextAlign="Center" />
                        <f:BoundField Width="130px" DataField="OUTRQ" HeaderText="出库日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="80px" DataField="SLY" HeaderText="申领人" TextAlign="Center" Hidden="true" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
        <f:HiddenField ID="echo" runat="server"></f:HiddenField>
    </form>
    <script type="text/javascript">
        function btnPrint_onclick() {
            var No = F('<%= echo.ClientID%>').getValue();
            if (No == "") {
                F.alert("请选择需要重新打印的试剂条码！");
                return;
            }
            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "/grf/onecode_SJ.grf";
            var dataurl = "/captcha/PrintReport.aspx?Method=GETGoodsSJ&osid=" + No;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
    </script>
</body>
</html>
