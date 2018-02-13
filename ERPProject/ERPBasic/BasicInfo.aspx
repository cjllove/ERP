<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BasicInfo.aspx.cs" Inherits="ERPProject.ERPBasic.BasicInfo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" runat="server" Layout="HBox" BoxConfigAlign="Stretch" BoxConfigPosition="Left"
            ShowBorder="false" BodyPadding="0px" ShowHeader="false">
            <Items>
                <f:Tree ID="TreeDic" Title="部门列表" BoxFlex="1" ShowBorder="false" runat="server" CssStyle="border-right: 1px solid #99bce8;"
                    OnNodeCommand="TreeDic_NodeCommand" OnNodeExpand="TreeDic_NodeExpand" EnableExpandEvent="true" />
                <f:Panel ID="Panel2" runat="server" BoxFlex="3" ShowBorder="false" BodyPadding="0px" Layout="VBOX" ShowHeader="false">
                    <Items>
                        <f:Form ID="FormMx" ShowBorder="false" BodyPadding="5px" ShowHeader="False" LabelWidth="80px" runat="server">
                            <Rows>
                                <f:FormRow ColumnWidths="30% 30% 40%">
                                    <Items>
                                        <f:TextBox ID="tbxCode" runat="server" Label="编码" Required="true" ShowRedStar="true"
                                            Enabled="false">
                                        </f:TextBox>
                                        <f:DropDownList ID="ddlSJCODE" runat="server" Label="上级" Enabled="false" ShowRedStar="true" EnableEdit="true" ForceSelection="true">
                                        </f:DropDownList>
                                        <f:RadioButtonList ID="rblDHZQ1" Label="星期一" runat="server">
                                            <f:RadioItem Text="上午送" Value="A" />
                                            <f:RadioItem Text="下午送" Value="P" />
                                            <f:RadioItem Text="不送" Value="N" Selected="true" />
                                        </f:RadioButtonList>
                                    </Items>
                                </f:FormRow>
                                <f:FormRow ColumnWidths="30% 30% 40%">
                                    <Items>
                                        <f:TextBox ID="tbxName" runat="server" Label="名称" Required="true" ShowRedStar="true">
                                        </f:TextBox>
                                        <f:TextBox ID="tbxSTR1" runat="server" Label="HIS编码" MaxLength="20" />
                                        <f:RadioButtonList ID="rblDHZQ2" Label="星期二" runat="server">
                                            <f:RadioItem Text="上午送" Value="A" />
                                            <f:RadioItem Text="下午送" Value="P" />
                                            <f:RadioItem Text="不送" Value="N" Selected="true" />
                                        </f:RadioButtonList>
                                    </Items>
                                </f:FormRow>
                                <f:FormRow ColumnWidths="30% 30% 40%">
                                    <Items>
                                        <f:DropDownList ID="ddlISORDER" runat="server" Label="订货" ShowRedStar="true" Required="true" EnableEdit="true" ForceSelection="true">
                                            <f:ListItem Text="有订货权限" Value="Y" />
                                            <f:ListItem Text="无订货权限" Value="N" Selected="true" />
                                        </f:DropDownList>
                                        <f:DropDownList ID="ddlSTR4" runat="server" Label="配送员"></f:DropDownList>
                                        <f:RadioButtonList ID="rblDHZQ3" Label="星期三" runat="server">
                                            <f:RadioItem Text="上午送" Value="A" />
                                            <f:RadioItem Text="下午送" Value="P" />
                                            <f:RadioItem Text="不送" Value="N" Selected="true" />
                                        </f:RadioButtonList>
                                    </Items>
                                </f:FormRow>
                                <f:FormRow ColumnWidths="30% 30% 40%">
                                    <Items>
                                        <f:DropDownList ID="ddlType" runat="server" Label="类别" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true">
                                        </f:DropDownList>
                                        <f:DropDownList ID="ddlSTR3" runat="server" Label="配送路线"></f:DropDownList>
                                        <f:RadioButtonList ID="rblDHZQ4" Label="星期四" runat="server">
                                            <f:RadioItem Text="上午送" Value="A" />
                                            <f:RadioItem Text="下午送" Value="P" />
                                            <f:RadioItem Text="不送" Value="N" Selected="true" />
                                        </f:RadioButtonList>
                                    </Items>
                                </f:FormRow>
                                <f:FormRow ColumnWidths="30% 30% 40%">
                                    <Items>
                                        <f:DropDownList ID="ddlManager" runat="server" Label="主管" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true">
                                        </f:DropDownList>
                                        <f:DropDownList ID="ddlSTOCK" runat="server" Label="默认库房" EnableEdit="true" ForceSelection="true" Required="true" ShowRedStar="true" />
                                        <f:RadioButtonList ID="rblDHZQ5" Label="星期五" runat="server">
                                            <f:RadioItem Text="上午送" Value="A" />
                                            <f:RadioItem Text="下午送" Value="P" />
                                            <f:RadioItem Text="不送" Value="N" Selected="true" />
                                        </f:RadioButtonList>
                                    </Items>
                                </f:FormRow>
                                <f:FormRow ColumnWidths="10% 10% 10% 30% 40%">
                                    <Items>
                                        <f:CheckBox ID="cbxFlag" ShowLabel="false" runat="server" Text="状态" Checked="True">
                                        </f:CheckBox>
                                        <f:CheckBox ID="cbxIsLast" ShowLabel="false" runat="server" Text="末级">
                                        </f:CheckBox>
                                        <f:CheckBox ID="cbxSTR5" ShowLabel="false" runat="server" Text="评价" Checked="true">
                                        </f:CheckBox>
                                        <f:CheckBox ID="cbxNUM1" ShowLabel="false" runat="server" Text="出库即使用" Checked="true">
                                        </f:CheckBox>
                                        <f:RadioButtonList ID="rblDHZQ6" Label="星期六" runat="server">
                                            <f:RadioItem Text="上午送" Value="A" />
                                            <f:RadioItem Text="下午送" Value="P" />
                                            <f:RadioItem Text="不送" Value="N" Selected="true" />
                                        </f:RadioButtonList>
                                    </Items>
                                </f:FormRow>
                                <f:FormRow ColumnWidths="30% 30% 40%">
                                    <Items>
                                        <f:NumberBox ID="nbxKCZQ" runat="server" Label="库存周期" MaxValue="15" MinValue="0" ShowRedStar="false" EmptyText="库存存放的货量周期数" NoDecimal="true"></f:NumberBox>
                                        <f:TextBox ID="tbxSTR6" runat="server" Label="联系电话"></f:TextBox>
                                        <f:RadioButtonList ID="rblDHZQ7" Label="星期日" runat="server">
                                            <f:RadioItem Text="上午送" Value="A" />
                                            <f:RadioItem Text="下午送" Value="P" />
                                            <f:RadioItem Text="不送" Value="N" Selected="true" />
                                        </f:RadioButtonList>
                                    </Items>
                                </f:FormRow>
                                <f:FormRow ColumnWidths="30% 30% 40%">
                                    <Items>
                                        <f:DropDownList ID="ddlHOUSE" runat="server" Label="楼栋"  ></f:DropDownList>
                                        <f:DropDownList ID="ddlFLOOR" runat="server" Label="楼层" ></f:DropDownList>
                                        <f:TextBox ID="tbxMemo" runat="server" Label="备注" MaxLength="80">
                                        </f:TextBox>
                                    </Items>
                                </f:FormRow>
                                <f:FormRow Hidden="true">
                                    <Items>
                                        <f:NumberBox ID="tbxCLASS" runat="server" Label="级次" Hidden="true" EnableAjax="true">
                                        </f:NumberBox>
                                    </Items>
                                </f:FormRow>
                            </Rows>
                            <Toolbars>
                                <f:Toolbar ID="Toolbar2" Position="Top" runat="server">
                                    <Items>
                                        <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="编码规则：" EnableAjax="true" runat="server" />
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
                            AllowPaging="false" runat="server" IsDatabasePaging="false" DataKeyNames="CODE" EnableColumnLines="true">
                            <Toolbars>
                                <f:Toolbar ID="Toolbar1" Position="Top" runat="server">
                                    <Items>
                                        <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="查询条件：" runat="server" />
                                        <f:DropDownList ID="ddlSearch" runat="server" EnableEdit="true" ForceSelection="true">
                                        </f:DropDownList>
                                        <f:Button ID="btnSearch" runat="server" CssStyle="margin-left: 10px;" Icon="magnifier" EnableDefaultState="false"
                                            OnClick="btnSearch_Click" EnablePostBack="true" Text="查询">
                                        </f:Button>
                                        <f:Button ID="btnExp" runat="server" CssStyle="margin-left: 10px;" Icon="DatabaseGo" EnableAjax="false" EnableDefaultState="false"
                                            OnClick="btnExp_Click" EnablePostBack="true" Text="导出" ConfirmText="是否确认导出此信息?" DisableControlBeforePostBack="false">
                                        </f:Button>
                                    </Items>
                                </f:Toolbar>
                            </Toolbars>
                            <Columns>
                                <%-- <f:BoundField DataField="TYPE" HeaderText="类别" Hidden="true" />--%>
                                <f:BoundField DataField="CODE" Width="200px" HeaderText="编码" />
                                <f:BoundField DataField="NAME" Width="200px" HeaderText="名称" />
                                <f:BoundField DataField="FLAG" Width="50px" HeaderText="状态" />
                                <f:BoundField DataField="TYPE" Width="100px" HeaderText="类别" />
                                <f:BoundField DataField="CLASS" Width="50px" HeaderText="级次" />
                                <f:BoundField DataField="ISLAST" Width="50px" HeaderText="末级" />
                                <f:BoundField DataField="STR1" Width="100px" HeaderText="HIS编码  " Hidden="false" />
                                <f:BoundField DataField="MEMO" Width="200px" HeaderText="备注" ExpandUnusedSpace="true" />
                            </Columns>
                        </f:Grid>
                    </Items>
                </f:Panel>
            </Items>
        </f:Panel>
    </form>
</body>
</html>
