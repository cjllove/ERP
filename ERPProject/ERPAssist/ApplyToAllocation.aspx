<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApplyToAllocation.aspx.cs" Inherits="ERPProject.ERPAssist.ApplyToAllocation" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="PanelMain" runat="server" AjaxLoadingType="Mask" />
        <f:Panel ID="PanelMain" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBox"
            ShowHeader="false">
            <Toolbars>
                <f:Toolbar ID="Toolbar2" runat="server">
                    <Items>
                        <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="操作信息：" runat="server" />
                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                        <f:Button ID="btnExport" runat="server" CssStyle="margin-left: 10px;" Icon="PageExcel" EnableAjax="false" EnableDefaultState="false"
                            OnClick="btnExport_Click" EnablePostBack="true" Text="导 出" ConfirmText="是否确认导出此科室申领信息?" DisableControlBeforePostBack="false" />
                        <f:ToolbarSeparator runat="server" />
                        <f:Button ID="btnAudit" Icon="UserTick" EnableDefaultState="false" Text="生成调拨单" Hidden="true" ConfirmText="确认将选中商品生成调拨单？" runat="server" OnClick="btnAudit_Click" />
                        <f:Button ID="bntSearch" Icon="Magnifier" EnableDefaultState="false" Text="查 询" EnablePostBack="true" runat="server" OnClick="bntSearch_Click" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="5px"
                    ShowHeader="False" LabelWidth="70px" runat="server">
                    <Rows>
                        <f:FormRow>
                            <Items>
                                <f:DropDownList ID="lstDEPTID" runat="server" Label="申领科室" EnableEdit="true" ForceSelection="true">
                                </f:DropDownList>
                                <f:DropDownList ID="lstFLAG" runat="server" Label="单据状态" EnableEdit="true" ForceSelection="true" />
                                <f:DatePicker ID="lstLRRQ1" runat="server" Label="申领日期" Required="true" />
                                <f:DatePicker ID="lstLRRQ2" runat="server" Label="　　至" LabelSeparator="" Required="true" />
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
                <f:Grid ID="GridList" BoxFlex="1" ShowBorder="false" ShowHeader="false" EnableCheckBoxSelect="true"
                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" 
                    DataKeyNames="DEPTID,GDSEQ" EnableTextSelection="true" EnableColumnLines="true" EnableHeaderMenu="true"  KeepCurrentSelection="true">
                    <Columns>
                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                        <f:BoundField Width="100px" DataField="DEPTOUT" HeaderText="出库部门" SortField="DEPTOUT" />
                        <f:BoundField Width="130px" DataField="DEPTID" HeaderText="申领科室" SortField="DEPTID" />
                        <f:BoundField Width="90px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="GDNAME" HeaderText="商品名称" ExpandUnusedSpace="true" MinWidth="100px" />
                        <f:BoundField Width="150px" DataField="GDSPEC" HeaderText="商品规格" TextAlign="Center" SortField="SUBNUM" />
                        <f:BoundField Width="70px" DataField="SLS" HeaderText="申请数量" TextAlign="Center" SortField="SLR" />
                        <f:BoundField Width="60px" DataField="UNIT" HeaderText="单位" TextAlign="Center" SortField="LRY" />
                        <f:BoundField Width="170px" DataField="PRODUCER" HeaderText="生产厂家" SortField="MEMO" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
</body>
</html>
