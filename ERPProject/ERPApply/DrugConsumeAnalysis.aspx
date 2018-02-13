<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DrugConsumeAnalysis.aspx.cs" Inherits="ERPProject.ERPApply.DrugConsumeAnalysis" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>使用信息分析</title>
    <style type="text/css" media="all">
        .ColBlue {
            font-size: 12px;
            color: blue;
        }

        .x-grid-row-summary .x-grid-cell-inner {
            font-weight: bold;
            color: blue;
        }

        .x-grid-row-summary .x-grid-cell {
            background-color: #fff !important;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="TabStrip1" runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="1" runat="server">
            <Tabs>
                <f:Tab Title="按品规汇总" Icon="PageWord" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBox"
                            ShowHeader="false">
                            <Toolbars>
                                <f:Toolbar ID="Toolbar1" runat="server">
                                    <Items>
                                        <f:Label ID="lblSUBSUM" runat="server" Label="合计数量" LabelWidth="70px"></f:Label>
                                        <f:Label ID="lblSUBNUM" runat="server" Label="合计金额" LabelWidth="70px" CssStyle="margin-left:30px"></f:Label>
                                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                        <f:Button ID="btClear" Icon="ArrowRotateClockwise" Text="清 空" OnClick="btClear_Click" EnablePostBack="true" runat="server" />
                                        <f:ToolbarSeparator runat="server" />
                                        <%--<f:Button ID="btnPrint" Icon="Printer" Text="打 印" DisableControlBeforePostBack="true" runat="server" OnClientClick="btnPrint_onclick()" />
                                        <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" OnClick="btExport_Click" ConfirmText="是否导出当前信息?" DisableControlBeforePostBack="false"
                                            EnablePostBack="true" EnableAjax="false" EnableAjaxLoading="true" AjaxLoadingType="Mask" runat="server" />
                                        <f:ToolbarSeparator runat="server" />--%>
                                        <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" OnClick="btSearch_Click" runat="server" />
                                    </Items>
                                </f:Toolbar>
                            </Toolbars>
                            <Items>
                                <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="5px"
                                    ShowHeader="False" LabelWidth="75px" runat="server">
                                    <Rows>
                                        <f:FormRow>
                                            <Items>
                                                <f:DatePicker ID="lstLRRQ1" runat="server" Label="查询期间" Required="true" />
                                                <f:DatePicker ID="lstLRRQ2" runat="server" Label="　　至" LabelSeparator="" Required="true" />
                                                <f:DropDownList ID="lstDEPTID" runat="server" Label="查询科室" EnableEdit="true" ForceSelection="true" />
                                                <f:TriggerBox ID="trbGOODS" runat="server" Label="查询商品" EmptyText="可输入编码、名称或助记码" TriggerIcon="Search" OnTriggerClick="btSearch_Click"></f:TriggerBox>
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                                <f:Grid ID="GridGoods" BoxFlex="1" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;"
                                    PageSize="100" IsDatabasePaging="true" AllowPaging="true" OnPageIndexChange="GridGoods_PageIndexChange" EnableColumnLines="true" EnableTextSelection="true"
                                    AllowSorting="true" AutoScroll="true" runat="server" DataKeyNames="DEPTNAME,GDSEQ" OnSort="GridGoods_Sort" SortField="DEPTNAME" SortDirection="ASC"
                                    EnableSummary="true" SummaryPosition="Bottom" EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridGoods_RowDoubleClick">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" />
                                        <f:BoundField Width="130px" DataField="DEPTNAME" SortField="DEPTNAME" HeaderText="科室名称" />
                                        <f:BoundField Width="100px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                                        <f:BoundField Width="200px" DataField="GDNAME" SortField="GDNAME" ColumnID="GDNAME" HeaderText="商品名称" />
                                        <f:BoundField Width="150px" DataField="GDSPEC" SortField="GDSPEC" HeaderText="规格·容量" />
                                        <f:BoundField Width="100px" DataField="CATTYPE" SortField="CATTYPE" ColumnID="CATTYPE" HeaderText="商品类别" TextAlign="Center" />
                                        <f:BoundField Width="50px" DataField="UNIT" SortField="UNIT" HeaderText="单位" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="HSJJ" SortField="HSJJ" HeaderText="含税进价" DataFormatString="{0:F4}" TextAlign="Right" />
                                        <f:BoundField Width="80px" DataField="SL" SortField="SL" ColumnID="SL" HeaderText="数量" TextAlign="Right" />
                                        <f:BoundField Width="90px" DataField="JE" SortField="JE" ColumnID="JE" HeaderText="金额" DataFormatString="{0:F2}" TextAlign="Right" />
                                        <f:BoundField Width="170px" DataField="SUPNAME" SortField="SUPNAME" HeaderText="供应商" />
                                        <f:BoundField Width="170px" DataField="PRODUCER" SortField="PRODUCER" HeaderText="生产厂家" />
                                        <f:BoundField Width="190px" DataField="PZWH" SortField="PZWH" HeaderText="批准文号" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="明细查看" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel2" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBox"
                            ShowHeader="false">
                            <Toolbars>
                                <f:Toolbar ID="Toolbar2" runat="server">
                                    <Items>
                                        <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                        <f:Button ID="btnClear" Icon="ArrowRotateClockwise" Text="清 空" OnClick="btnClear_Click" EnablePostBack="true" runat="server" />
                                        <%--<f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btnPrint1" Icon="Printer" Text="打 印" DisableControlBeforePostBack="true" runat="server" OnClientClick="btnPrint1_onclick()" />
                                        <f:Button ID="btnExport1" Icon="DatabaseGo" Text="导 出" OnClick="btnExport1_Click" ConfirmText="是否导出当前信息?" DisableControlBeforePostBack="false"
                                            EnablePostBack="true" EnableAjax="false" EnableAjaxLoading="true" AjaxLoadingType="Mask" runat="server" />--%>
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btnSearch" Icon="Magnifier" Text="查 询" OnClick="btnSearch_Click" runat="server" />
                                    </Items>
                                </f:Toolbar>
                            </Toolbars>
                            <Items>
                                <f:Form ID="FormSearch" ShowBorder="false" AutoScroll="false" BodyPadding="5px"
                                    ShowHeader="False" LabelWidth="75px" runat="server">
                                    <Rows>
                                        <f:FormRow>
                                            <Items>
                                                <f:DatePicker ID="dpkBegRQ" runat="server" Label="查询期间" Required="true" />
                                                <f:DatePicker ID="dpkEndRQ" runat="server" Label="　　至" LabelSeparator="" Required="true" />
                                                <f:DropDownList ID="ddlDEPTID" runat="server" Label="查询科室" EnableEdit="true" ForceSelection="true" />
                                                <f:TriggerBox ID="trbSearch" runat="server" Label="查询商品" EmptyText="可输入编码、名称或助记码" TriggerIcon="Search" OnTriggerClick="btnSearch_Click"></f:TriggerBox>
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                                <f:Grid ID="GridCom" BoxFlex="1" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;"
                                    EnableColumnLines="true" EnableTextSelection="true" AutoScroll="true" runat="server" DataKeyNames="GDSEQ"
                                    EnableSummary="true" SummaryPosition="Bottom" AllowPaging="true" IsDatabasePaging="true" PageSize="100"
                                    OnPageIndexChange="GridCom_PageIndexChange">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField Width="80px" DataField="SHRQ" SortField="SHRQ" HeaderText="使用日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="120px" DataField="BILLNO" SortField="BILLNO" HeaderText="单据编号" TextAlign="Center" />
                                        <f:BoundField Width="120px" DataField="DEPTNAME" SortField="DEPTNAME" HeaderText="科室" />
                                        <f:BoundField Width="90px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                                        <f:BoundField Width="180px" DataField="GDNAME" SortField="GDNAME" ColumnID="COUNTTITLE" HeaderText="商品名称" />
                                        <f:BoundField Width="150px" DataField="GDSPEC" SortField="GDSPEC" HeaderText="规格·容量" />
                                        <f:BoundField Width="40px" DataField="UNIT" SortField="UNIT" HeaderText="单位" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="HSJJ" SortField="HSJJ" HeaderText="含税进价" DataFormatString="{0:F4}" TextAlign="Right" />
                                        <f:BoundField Width="90px" DataField="HJSL" SortField="HJSL" ColumnID="HJSL" HeaderText="数量" TextAlign="Right" />
                                        <f:BoundField Width="90px" DataField="HJJE" SortField="HJJE" ColumnID="HJJE" HeaderText="金额" DataFormatString="{0:F2}" TextAlign="Right" />
                                        <f:BoundField Width="60px" DataField="ISGZ" SortField="ISGZ" HeaderText="是否高值" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="PH" SortField="PH" ColumnID="PH" HeaderText="批号" />
                                        <f:BoundField Width="80px" DataField="YXQZ" SortField="YXQZ" ColumnID="YXQZ" TextAlign="Center" HeaderText="效期" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="170px" DataField="BARCODE" HeaderText="条码" SortField="BARCODE" TextAlign="Center" />
                                        <f:BoundField Width="170px" DataField="SUPNAME" SortField="SUPNAME" HeaderText="供应商" />
                                        <f:BoundField Width="170px" DataField="PRODUCER" SortField="PRODUCER" HeaderText="生产厂家" />
                                        <f:BoundField Width="190px" DataField="PZWH" SortField="PZWH" HeaderText="批准文号" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
    </form>
</body>
</html>
