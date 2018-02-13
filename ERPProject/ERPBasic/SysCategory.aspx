<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SysCategory.aspx.cs" Inherits="ERPProject.ERPBasic.SysCategory" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
    <f:Panel ID="Panel1" runat="server" Layout="HBox" BoxConfigAlign="Stretch" BoxConfigPosition="Left"
        ShowBorder="false" BodyPadding="0px" ShowHeader="false">
        <Items>
            <f:Tree ID="TreeDic" Title="树形列表" BoxFlex="1" Width="250px" ShowBorder="false" runat="server" CssStyle="border-right: 1px solid #99bce8;"
                OnNodeCommand="TreeDic_NodeCommand" OnNodeExpand="TreeDic_NodeExpand" EnableExpandEvent="true" />
            <f:Panel ID="Panel2" runat="server" BoxFlex="3" ShowBorder="false" BodyPadding="0px" Layout="VBOX" ShowHeader="false">
                <Items>
                    <f:Form ID="FormMx" ShowBorder="false" BodyPadding="5px" ShowHeader="False" LabelWidth="60px" runat="server">
                        <Rows>
                            <f:FormRow ColumnWidths="20% 25% 25% 20% 10%">
                                <Items>
                                    <f:TextBox ID="tbxCode" runat="server" Label="编码" EmptyText="设定后不可更改" Required="true" ShowRedStar="true"
                                        Enabled="false">
                                    </f:TextBox>
                                    <f:TextBox ID="tbxName" runat="server" Label="名称" Required="true" ShowRedStar="true">
                                    </f:TextBox>
                                    <f:DropDownList ID="ddlManager" runat="server" Label="主管">
                                    </f:DropDownList>
                                    <f:DropDownList ID="ddlType" runat="server" Label="类别" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true">
                                    </f:DropDownList>
                                    <f:CheckBox ID="cbxFlag" ShowLabel="false" runat="server" Text="状态" Checked="True">
                                    </f:CheckBox>
                                </Items>
                            </f:FormRow>
                            <f:FormRow ColumnWidths="70% 20% 10%">
                                <Items>
                                    <f:TextBox ID="tbxMemo" runat="server" Label="备注">
                                    </f:TextBox>
                                    <f:DropDownList ID="ddlSJCODE" runat="server" Label="上级" Enabled="false" ShowRedStar="true" Required="true" EnableEdit="true" ForceSelection="true">
                                    </f:DropDownList>
                                    <f:CheckBox ID="cbxIsLast" ShowLabel="false" runat="server" Text="末级">
                                    </f:CheckBox>
                                </Items>
                            </f:FormRow>
                            <f:FormRow Hidden="true">
                                <Items>
                                    <f:NumberBox ID="tbxCLASS" runat="server" Label="级次" Hidden="true" EnableAjax="true" >
                                    </f:NumberBox>
                                </Items>
                            </f:FormRow>
                        </Rows>
                        <Toolbars>
                            <f:Toolbar ID="Toolbar2" Position="Top" runat="server">
                                <Items>
                                    <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="编码规则：" EnableAjax="true" runat="server" />
                                    <f:ToolbarText ID="tbxRule" CssStyle="" Text="2-2-2" EnableAjax="true" runat="server" />
                                    <f:ToolbarFill ID="ToolBarFill2" runat="server" />
                                    <f:Button ID="btnAdd" Icon="Add" OnClick="btnAdd_Click" Text="增 加" CssStyle=" margin-right:20px; " EnableDefaultState="false"
                                        runat="server" />
                                    <f:Button ID="btnDel" Icon="Delete" OnClick="btnDel_Click" Text="删 除" ConfirmText="是否确认删除此信息?" CssStyle=" margin-right:20px; " EnableDefaultState="false"
                                        runat="server" />
                                    <f:Button ID="btnSave" runat="server" Icon="Disk" OnClick="btnSave_Click" EnablePostBack="true" EnableDefaultState="false"
                                        Text="保存" ValidateForms="FormMx">
                                    </f:Button>
                                </Items>
                            </f:Toolbar>
                        </Toolbars>
                    </f:Form>
                    <f:Grid ID="GridMx" BoxFlex="1" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;"
                        AllowPaging="false" runat="server" IsDatabasePaging="false" EnableColumnLines="true">
                        <Toolbars>
                            <f:Toolbar ID="Toolbar1" Position="Top" runat="server">
                                <Items>
                                    <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="查询条件：" runat="server" />
                                    <f:DropDownList ID="ddlSearch" runat="server" EnableEdit="true" ForceSelection="true">
                                    </f:DropDownList>
                                    <f:Button ID="btnSearch" runat="server" CssStyle="margin-left: 10px;" Icon="magnifier"
                                        OnClick="btnSearch_Click" EnablePostBack="true" Text="查询">
                                    </f:Button>
                                    <f:Button ID="btnExp" runat="server" CssStyle="margin-left: 10px;" Icon="DatabaseGo"
                                        OnClick="btnExp_Click" EnablePostBack="true" Text="导出" EnableAjax="false">
                                    </f:Button>
                                </Items>
                            </f:Toolbar>
                        </Toolbars>
                        <Columns>
                            <f:BoundField DataField="TYPE" HeaderText="类别" Hidden="true" />
                            <f:BoundField DataField="CODE" Width="200px" HeaderText="编码" />
                            <f:BoundField DataField="NAME" Width="200px" HeaderText="名称" />
                            <f:BoundField DataField="FLAG" Width="50px" HeaderText="状态" />
                            <f:BoundField DataField="TYPE" Width="100px" HeaderText="类别" />
                            <f:BoundField DataField="LEVELS" Width="50px" HeaderText="级次" />
                            <f:BoundField DataField="ISLAST" Width="50px" HeaderText="末级" />
                            <f:BoundField DataField="MEMO" Width="200px" HeaderText="备注" ExpandUnusedSpace="true" />
                            <f:BoundField DataField="STR1" Hidden="true" />
                            <f:BoundField DataField="STR2" Hidden="true" />
                            <f:BoundField DataField="STR3" Hidden="true" />
                        </Columns>
                    </f:Grid>
                </Items>
            </f:Panel>
        </Items>
    </f:Panel>
    </form>
</body>
</html>
