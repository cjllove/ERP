<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FP_Manage.aspx.cs" Inherits="ERPProject.ERPBasic.FP_Manage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>发票管理</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="PanelMain" runat="server" />
        <f:Panel ID="PanelMain" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
            ShowHeader="false">
            <Toolbars>
                <f:Toolbar ID="Toolbar2" runat="server">
                    <Items>
                        <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="操作信息：发票信息维护界面！" runat="server" />
                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                        <f:Button ID="btnNew" Icon="PageAdd" Text="新 增" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnNew_Click" />
                        <f:Button ID="bntClear" Icon="Erase" Text="清 除" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="bntClear_Click" />
                        <f:ToolbarSeparator runat="server" />
                        <f:Button ID="btnSave" Icon="Disk" runat="server" EnableDefaultState="false" ValidateForms="Formlist" EnablePostBack="true" Text="保 存" OnClick="btnSave_Click"></f:Button>
                        <f:Button ID="btnDel" Icon="PageCancel" runat="server" EnableDefaultState="false" EnablePostBack="true" Text="删 除" ConfirmText="是否确定删除选中信息?" OnClick="btnDel_Click"></f:Button>
                        <f:ToolbarSeparator runat="server" />
                        <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="bntSearch_Click" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                    ShowHeader="False" LabelWidth="90px" runat="server">
                    <Rows>
                        <f:FormRow ColumnWidths="26% 26% 25% 23%">
                            <Items>
                                <f:TextBox ID="lstSEQNO" runat="server" Label="发票号码" EmptyText="设定后不允许更改" Required="true" ShowRedStar="true" MaxLength="20" />
                                <f:NumberBox ID="nbxFPJE" runat="server" Label="发票金额" DecimalPrecision="4" Text="0" ShowRedStar="true" Required="true" MinValue="0" MaxValue="999999999"></f:NumberBox>
                                <f:DropDownList ID="lstLRY" runat="server" Label="录入员" EnableEdit="true" Enabled="false" />
                                <f:DatePicker ID="lstLRRQ" runat="server" Label="录入日期" Required="true" Enabled="false" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow ColumnWidths="26% 26% 25% 23%">
                            <Items>
                                <f:DropDownList ID="lstFPTYPE" runat="server" Label="发票类型" ShowRedStar="true" Required="true" EnableEdit="true" ForceSelection="true" />
                                <f:DropDownList ID="ddlSUPID" Label="供应商" runat="server" EnableEdit="true" ShowRedStar="true" Required="true" ForceSelection="true"></f:DropDownList>
                                <f:TextBox ID="tbxUSEJE" runat="server" Label="发票剩余金额" LabelWidth="100px" Enabled="false"></f:TextBox>
                                <f:DropDownList ID="ddlFLAG" Label="发票状态" runat="server" Enabled="false">
                                    <f:ListItem Value="N" Text="未使用" Selected="true" />
                                    <f:ListItem Value="F" Text="已使用" />
                                </f:DropDownList>
                            </Items>
                        </f:FormRow>
                        <f:FormRow ColumnWidths="26% 74%">
                            <Items>
                                <f:DropDownList ID="ddlTAXRATE" Label="发票税率" runat="server" EnableEdit="true" ForceSelection="true"></f:DropDownList>
                                <f:TextBox ID="tbxMEMO" runat="server" Label="备注" MaxLength="50"></f:TextBox>
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
                <f:Grid ID="GridList" AnchorValue="100% -106" ShowBorder="false" ShowHeader="false" EnableCollapse="true"
                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;"
                    DataKeyNames="FLAG,SEQNO" EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridList_RowDoubleClick">
                    <Columns>
                        <f:RowNumberField runat="server"></f:RowNumberField>
                        <f:BoundField Width="170px" DataField="SEQNO" HeaderText="发票号码" EnableColumnHide="false" EnableHeaderMenu="false" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="FPTYPE" HeaderText="发票类型" TextAlign="Center" EnableColumnHide="false" EnableHeaderMenu="false" Hidden="true" />
                        <f:BoundField Width="90px" DataField="FPTYPENAME" HeaderText="发票类型" TextAlign="Center" EnableColumnHide="false" EnableHeaderMenu="false" />
                        <f:BoundField Width="90px" DataField="FLAG" HeaderText="发票状态" TextAlign="Center" EnableColumnHide="false" EnableHeaderMenu="false" Hidden="true" />
                        <f:BoundField Width="80px" DataField="FLAGNAME" HeaderText="发票状态" TextAlign="Center" EnableColumnHide="false" EnableHeaderMenu="false" />
                        <f:BoundField Width="60px" DataField="TAXRATE" HeaderText="税率" TextAlign="Center" EnableColumnHide="false" EnableHeaderMenu="false" />
                        <f:BoundField Width="150px" DataField="SUPID" HeaderText="供应商" TextAlign="Center" EnableColumnHide="false" EnableHeaderMenu="false" Hidden="true" />
                        <f:BoundField Width="130px" DataField="SUPNAME" HeaderText="供应商名称" TextAlign="Center" EnableColumnHide="false" EnableHeaderMenu="false" />
                        <f:BoundField Width="130px" DataField="LRY" HeaderText="录入员" TextAlign="Center" EnableColumnHide="false" EnableHeaderMenu="false" Hidden="true" />
                        <f:BoundField Width="70px" DataField="LRYNAME" HeaderText="录入员" TextAlign="Center" EnableColumnHide="false" EnableHeaderMenu="false" />
                        <f:BoundField Width="90px" DataField="LRRQ" HeaderText="录入日期" TextAlign="Center" EnableColumnHide="false" EnableHeaderMenu="false" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="110px" DataField="FPJE" HeaderText="发票金额" TextAlign="Center" EnableColumnHide="false" EnableHeaderMenu="false" DataFormatString="{0:F2}" />
                        <f:BoundField Width="110px" DataField="JSJE" HeaderText="已结算金额" TextAlign="Center" EnableColumnHide="false" EnableHeaderMenu="false" DataFormatString="{0:F2}" />
                        <f:BoundField Width="110px" DataField="USEJE" HeaderText="发票剩余金额" TextAlign="Center" EnableColumnHide="false" EnableHeaderMenu="false" DataFormatString="{0:F2}" />
                        <f:BoundField Width="60px" DataField="NUM1" HeaderText="结算次数" EnableColumnHide="false" EnableHeaderMenu="false" TextAlign="Center" />
                        <f:BoundField Width="180px" DataField="MEMO" HeaderText="备注" EnableColumnHide="false" EnableHeaderMenu="false" TextAlign="Center" />
                        <f:BoundField Width="220px" DataField="JSDH" HeaderText="结算单号" EnableColumnHide="false" EnableHeaderMenu="false" TextAlign="Center" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
</body>
</html>
