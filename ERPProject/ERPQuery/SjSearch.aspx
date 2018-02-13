<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SjSearch.aspx.cs" Inherits="ERPProject.ERPQuery.SjSearch" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>试剂出库查询</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
            ShowHeader="false">
            <Items>
                <f:Panel ID="PanelBody" ShowBorder="false" BodyPadding="0px"
                    Layout="Anchor" ShowHeader="False" runat="server">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar1" runat="server">
                            <Items>
                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" EnableDefaultState="false" OnClick="btnExport_Click" ConfirmText="是否导出当前商品库存信息?" DisableControlBeforePostBack="false"
                                    EnablePostBack="true" EnableAjax="false" EnableAjaxLoading="true" AjaxLoadingType="Mask" runat="server" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" EnableDefaultState="false" OnClick="btSearch_Click" runat="server" />
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Items>
                        <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="5px"
                            ShowHeader="False" LabelWidth="75px" runat="server">
                            <Rows>
                                <f:FormRow>
                                    <Items>
                                        <f:DropDownList ID="ddlDEPTID" runat="server" Label="部门科室" EnableEdit="true" ForceSelection="true" />
                                        <f:TriggerBox ID="tbxGOODS" runat="server" Label="商品" ShowTrigger="false" EmptyText="可输入编码、名称或助记码" TriggerIcon="Search" MaxLength="20" OnTriggerClick="tbxGOODS_TriggerClick"></f:TriggerBox>
                                        <f:TriggerBox ID="tgbPH" runat="server" Label="批次" EmptyText="请输入批次信息" ShowTrigger="false" TriggerIcon="Search" MaxLength="10" OnTriggerClick="btSearch_Click"></f:TriggerBox>
                                        <f:DropDownList ID="ddlLRY" runat="server" Label="申领人" EnableEdit="true" ForceSelection="true" />
                                    </Items>
                                </f:FormRow>
                            </Rows>
                            <Rows>
                                <f:FormRow>
                                    <Items>
                                        <f:DropDownList ID="ddlSTR5" runat="server" Label="学科组" EnableEdit="true" ForceSelection="true" />
                                        <f:TriggerBox ID="tbxBILLNO" runat="server" Label="单据编号" ShowTrigger="false" EmptyText="请输入单据编号" TriggerIcon="Search" OnTriggerClick="btSearch_Click"></f:TriggerBox>
                                        <f:DatePicker ID="lstLRRQ1" runat="server" Label="使用日期" Required="true" />
                                        <f:DatePicker ID="lstLRRQ2" runat="server" Label="　　至" LabelSeparator="" Required="true" />
                                    </Items>
                                </f:FormRow>
                            </Rows>
                        </f:Form>
                    </Items>
                </f:Panel>
                <f:Grid ID="GridGoods" AnchorValue="100% -106" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;"
                    PageSize="30" IsDatabasePaging="true" AllowPaging="true" OnPageIndexChange="GridGoods_PageIndexChange" EnableSummary="true" SummaryPosition="Bottom"
                    EnableColumnLines="true" EnableTextSelection="true"
                    AllowSorting="true" AutoScroll="true" runat="server" DataKeyNames="GDSEQ" OnSort="GridGoods_Sort" SortField="INSTIME" SortDirection="DESC">
                    <Columns>
                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                        <f:BoundField Width="120px" DataField="DEPTIDNAME" SortField="DEPTID" HeaderText="科室" />
                        <f:BoundField Width="120px" DataField="STR5NAME" ColumnID="STR5NAME" SortField="STR5" HeaderText="学科组" />
                        <f:BoundField Width="110px" DataField="SEQNO" SortField="SEQNO" HeaderText="单据编号" TextAlign="Center" />
                        <f:BoundField Width="85px" DataField="XSTYPENAME" SortField="XSTYPE" HeaderText="单据类型" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="LRYNAME" HeaderText="录入员" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="LRRQ" HeaderText="录入日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="80px" DataField="SHRNAME" HeaderText="审核员" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="SHRQ" HeaderText="审核日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="110px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" />
                        <f:BoundField Width="180px" DataField="GDNAME" SortField="GDNAME" HeaderText="商品名称" />
                        <f:BoundField Width="100px" DataField="GDSPEC" SortField="GDSPEC" HeaderText="规格·容量" />
                        <f:BoundField Width="80px" DataField="UNITNAME" SortField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="HSJJ" SortField="HSJJ" HeaderText="价格" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="CKSL" ColumnID="CKSL" SortField="CKSL" HeaderText="出库数" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="THSL" ColumnID="THSL" SortField="THSL" HeaderText="退货数" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="HSJE" SortField="HSJE" ColumnID="HSJE" HeaderText="金额" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="PH" SortField="PH" HeaderText="批号" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="RQ_SC" SortField="RQ_SC" HeaderText="生产日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="90px" DataField="YXQZ" SortField="YXQZ" HeaderText="效期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
</body>
</html>
