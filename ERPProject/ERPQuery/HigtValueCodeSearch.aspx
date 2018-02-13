<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HigtValueCodeSearch.aspx.cs" Inherits="ERPProject.ERPQuery.HigtValueCodeSearch" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>商品高值码查询</title>
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
                                <%--<f:CheckBox ID="chkISDG" runat="server" Text="包含代管商品" CssStyle="margin-left:10px;"></f:CheckBox>--%>
                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                <f:Button ID="btnAudit" Icon="ArrowRotateClockwise" Text="清 空" OnClick="btnAudit_Click" EnablePostBack="true" runat="server" EnableDefaultState="false" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="btnPrint" Icon="Printer" Text="打 印" DisableControlBeforePostBack="true" runat="server" Hidden="true" EnableDefaultState="false" />
                                <%--<f:ToolbarSeparator runat="server" />--%>
                                <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" OnClick="btnExport_Click" ConfirmText="是否导出当前商品库存信息?" DisableControlBeforePostBack="false"
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
                                <f:FormRow ColumnWidths="30% 30% 20% 20%">
                                    <Items>
                                        <f:DropDownList ID="ddlDEPTID" runat="server" Label="库房/科室" EnableEdit="true" ForceSelection="true"  />
                                        <f:TriggerBox ID="tbxGOODS" runat="server" Label="商品" EmptyText="可输入编码、名称或助记码" TriggerIcon="Search" OnTriggerClick="tbxGOODS_TriggerClick"></f:TriggerBox>
                                        <f:DatePicker ID="lstLRRQ1" runat="server" Label="使用日期" Required="true" />
                                        <f:DatePicker ID="lstLRRQ2" runat="server" Label="　　至" LabelSeparator="" Required="true" />
                                    </Items>
                                </f:FormRow>
                            </Rows>
                            <Rows>
                                <f:FormRow ColumnWidths="30% 30% 40%">
                                    <Items>
                                        <f:TriggerBox ID="tbxBILLNO" runat="server" Label="单据编号" EmptyText="请输入单据编号" TriggerIcon="Search" OnTriggerClick="btSearch_Click"></f:TriggerBox>
                                        <f:DropDownList ID="ddlFLag" runat="server" Label="使用状态" EnableEdit="true" ForceSelection="true" />
                                        <f:TriggerBox ID="tbxONECODE" runat="server" Label="追溯码" EmptyText="请输入追溯码" TriggerIcon="Search" OnTriggerClick="btSearch_Click"></f:TriggerBox>
                                    </Items>
                                </f:FormRow>
                            </Rows>
                        </f:Form>
                    </Items>
                </f:Panel>
                <f:Grid ID="GridGoods" AnchorValue="100% -110" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;"
                    PageSize="100" IsDatabasePaging="true" AllowPaging="true" OnPageIndexChange="GridGoods_PageIndexChange"
                    EnableColumnLines="true" EnableTextSelection="true"
                    AllowSorting="true" AutoScroll="true" runat="server" DataKeyNames="GDSEQ" OnSort="GridGoods_Sort" SortField="INSTIME" SortDirection="DESC">
                    <Columns>
                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                        <f:BoundField Width="120px" DataField="DEPTID" SortField="DEPTID" HeaderText="库房/科室" />
                        <f:BoundField Width="110px" DataField="BILLNO" SortField="BILLNO" HeaderText="单据号" TextAlign="Center" />
                        <f:BoundField Width="85px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                        <f:BoundField Width="180px" DataField="GDNAME" SortField="GDNAME" HeaderText="商品名称" ExpandUnusedSpace="true" />
                        <f:BoundField Width="100px" DataField="GDSPEC" SortField="GDSPEC" HeaderText="规格·容量" />
                        <f:BoundField Width="50px" DataField="UNITNAME" SortField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="BZHL" SortField="BZHL" HeaderText="包装含量" TextAlign="Center" />
                        <f:BoundField Width="200px" DataField="ONECODE" SortField="ONECODE" HeaderText="追溯码" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="FLAG" SortField="FLAG" HeaderText="使用状态" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="PH" SortField="PH" HeaderText="批号" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="RQ_SC" SortField="RQ_SC" HeaderText="生产日期" TextAlign="Center"  DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="80px" DataField="YXQZ" SortField="YXQZ" HeaderText="效期" TextAlign="Center"  DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="140px" DataField="INSTIME" SortField="INSTIME" HeaderText="处理时间" TextAlign="Center" />
                        <f:BoundField Width="200px" DataField="STR1" SortField="STR1" HeaderText="供应商唯一码" TextAlign="Center"  DataFormatString="{0:yyyy-MM-dd}" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
</body>
</html>
