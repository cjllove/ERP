<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EntrustAudit.aspx.cs" Inherits="ERPProject.ERPEntrust.EntrustAudit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>代管使用确认</title>
    <style type="text/css">
        .ui-state-disabled {
            opacity: .5;
            filter: alpha(opacity=50);
            background-image: none;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" ShowBorder="false" BodyPadding="0px" Layout="Anchor" ShowHeader="False" runat="server">
            <Toolbars>
                <f:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                        <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="操作提示信息：" runat="server" />
                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                        <f:Button ID="bntClear" Icon="Erase" Text="清 除" EnableDefaultState="false" EnablePostBack="true" runat="server" />
                        <f:ToolbarSeparator runat="server" />
                        <f:Button ID="btnAudit" Icon="UserTick" Text="审 核" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否确认审核选中单据？" OnClick="btnAudit_Click" />
                        <f:ToolbarSeparator runat="server" />
                        <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="bntSearch_Click" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                    ShowHeader="False" LabelWidth="80px" runat="server">
                    <Rows>
                        <f:FormRow ColumnWidths="25% 25% 25% 25%">
                            <Items>
                                <f:TextBox ID="tbxBILLNO" runat="server" Label="查询信息" MaxLength="50" EmptyText="请输入原单号、商品编码或商品名称" />
                                <f:DropDownList ID="lstFLAG" runat="server" Label="状态">
                                    <f:ListItem Text="---请选择---" Value="" />
                                    <f:ListItem Text="新生成" Value="N" />
                                    <f:ListItem Text="已下单" Value="Y" />
                                    <f:ListItem Text="已完成" Value="G" />
                                </f:DropDownList>
                                <f:DropDownList ID="lstLRY" runat="server" Label="使用人" />
                                <f:DropDownList ID="lstDEPTID" runat="server" Label="使用部门" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow ColumnWidths="50% 25% 25%">
                            <Items>
                                <f:DropDownList ID="lstSUPID" runat="server" Label="供应商" />
                                <f:DatePicker ID="lstLRRQ1" runat="server" Label="使用时间" Required="true" ShowRedStar="true" />
                                <f:DatePicker ID="lstLRRQ2" runat="server" Label="　　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
                <f:Grid ID="GridList" AnchorValue="100% -70" ShowBorder="false" ShowHeader="false" EnableCheckBoxSelect="true"
                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableTextSelection="true"
                    AllowPaging="true" IsDatabasePaging="true" PageSize="100" OnPageIndexChange="GridList_PageIndexChange" KeepCurrentSelection="true"
                    DataKeyNames="BILLNO,ROWNO,FLAG_CN" EnableRowDoubleClickEvent="true" EnableColumnLines="true" OnRowDataBound="GridList_RowDataBound">
                    <Columns>
                        <f:BoundField Width="110px" DataField="BILLNO" HeaderText="原单据编号" TextAlign="Center" />
                        <f:BoundField Width="0" DataField="ROWNO" HeaderText="行号" TextAlign="Center" Hidden="true" />
                        <f:BoundField Width="70px" DataField="USERNAME" HeaderText="使用人" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="USERTIME" HeaderText="使用时间" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="110px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                        <f:BoundField Width="170px" DataField="GDNAME" HeaderText="商品名称" TextAlign="Left" />
                        <f:BoundField Width="150px" DataField="SUPNAME" HeaderText="供应商" TextAlign="Left" ExpandUnusedSpace="true" />
                        <f:BoundField Width="100px" DataField="ONECODE" HeaderText="唯一码" TextAlign="Center" Hidden="true" />
                        <f:BoundField Width="50px" DataField="SL" HeaderText="数量" TextAlign="Right" />
                        <f:BoundField Width="50px" DataField="UNIT" HeaderText="单位" TextAlign="Center" />
                        <f:BoundField Width="70px" DataField="FLAG_CN" HeaderText="当前状态" TextAlign="Center" DataFormatString="{0:F2}" />
                        <f:BoundField Width="80px" DataField="ORDUSERNAME" HeaderText="生成订单人" TextAlign="Center" />
                        <f:BoundField Width="110px" DataField="ORDBILLNO" HeaderText="订单号" TextAlign="Center" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
</body>
</html>
