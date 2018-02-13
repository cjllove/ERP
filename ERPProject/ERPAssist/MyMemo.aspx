<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MyMemo.aspx.cs" Inherits="ERPProject.ERPAssist.MyMemo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>备忘录</title>
    <style>

    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" runat="server" Title="备忘录" ShowBorder="true" ShowHeader="false" BodyPadding="0px" Layout="VBOX">
            <Toolbars>
                <f:Toolbar ID="Toolbar1" runat="server" Position="Top">
                    <Items>
                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                        <f:ToolbarSeparator runat="server" />
                        <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnablePostBack="true" runat="server" OnClick="btnSearch_Click" EnableDefaultState="false" />
                        <f:Button ID="Button1" Icon="Erase" Text="新　增" EnablePostBack="true" runat="server" OnClick="btnAddNew_Click" EnableDefaultState="false" />
                        <f:ToolbarSeparator runat="server" />
                        <f:Button ID="btnClear" Icon="Erase" Text="清 　空" EnablePostBack="true" runat="server" OnClick="btnClear_Click" EnableDefaultState="false" />
                        <f:ToolbarSeparator runat="server" />
                        <f:Button ID="btnSave" Icon="UserTick" Text="保　存" EnablePostBack="true" runat="server" ConfirmText="是否确认保存？" OnClick="btnSave_Click" EnableDefaultState="false" />
                        <f:ToolbarSeparator runat="server" />
                        <f:Button ID="btnCancel" Icon="Book" Text="取　消" EnablePostBack="true" runat="server" OnClick="btnCancel_Click" EnableDefaultState="false" />
                        <f:ToolbarSeparator runat="server" />
                        <f:Button ID="btnComplete" Icon="AwardStarSilver1" Text="完　成" EnablePostBack="true" runat="server" OnClick="btnComplete_Click" EnableDefaultState="false" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Form runat="server" ID="FormSearchItem" ShowBorder="false" AutoScroll="false" BodyPadding="5px"
                    ShowHeader="false" LabelWidth="80px">
                    <Rows>
                        <f:FormRow ColumnWidths="40% 30% 30%">
                            <Items>
                                <f:TextBox ID="tbSearchTitle" runat="server" Label="查询标题" MaxLength="50"></f:TextBox>
                                <f:DatePicker runat="server" Required="true" EnableEdit="true" Label="开始日期" EmptyText="请选择日期" ID="dpSearchStart" ShowRedStar="True"></f:DatePicker>
                                <f:DatePicker runat="server" Required="true" EnableEdit="true" Label="结束日期" EmptyText="请选择日期" ID="dpSearchEnd" ShowRedStar="True"></f:DatePicker>
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
                <f:Panel ID="Panel2" runat="server" ShowBorder="false" ShowHeader="false" BodyPadding="0px" Layout="HBox" BoxConfigAlign="Stretch" BoxConfigPosition="Start" BoxFlex="1" AnchorValue="100% -37">
                    <Items>
                        <f:Panel ID="Panel3" runat="server" ShowHeader="false" ShowBorder="false" BodyPadding="0px" Layout="VBOX" BoxFlex="13">
                            <Items>
                                <f:Grid ID="GridList" BoxFlex="1" AnchorValue="100% -200" ShowBorder="false" ShowHeader="false" EnableCollapse="true" EnableColumnLines="true"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;"
                                    DataKeyNames="SEQNO" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick"
                                    AllowSorting="true" EnableHeaderMenu="true" SortField="SEQNO" OnSort="GridList_Sort" SortDirection="ASC"
                                    IsDatabasePaging="true" AllowPaging="true" PageSize="70" OnPageIndexChange="GridList_PageIndexChange">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField Width="170px" DataField="TITLE" HeaderText="备忘主题" SortField="TITLE" TextAlign="left" />
                                        <f:BoundField Width="100px" DataField="LRNAME" HeaderText="录入人" SortField="LRNAME" TextAlign="Center" />
                                        <f:BoundField Width="120px" DataField="LRRQ" HeaderText="录入时间" SortField="LRRQ" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="120px" DataField="WARINGRQ" HeaderText="提醒时间" SortField="WARINGRQ" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="120px" DataField="STATUS" HeaderText="状态" SortField="STATUS" TextAlign="Center" DataFormatString="" />
                                        <f:BoundField Width="100px" DataField="CLOSENAME" HeaderText="关闭人" SortField="CLOSENAME" TextAlign="Center" />
                                        <f:BoundField Width="120px" DataField="CLOSERQ" HeaderText="关闭时间" SortField="CLOSERQ" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="100px" DataField="LOOKPER" HeaderText="范围" SortField="LOOKPER" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="100px" DataField="MEMO" HeaderText="内围" SortField="MEMO" TextAlign="Center" Hidden="true" />
                                        <f:TemplateField ColumnID="expander" RenderAsRowExpander="true">
                                            <ItemTemplate>
                                                <div class="expander">
                                                    <div style="line-height: 25px;">
                                                        <strong>备忘录内容：</strong>
                                                        <%# Eval("MEMO") %>
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                        </f:TemplateField>
                                    </Columns>
                                </f:Grid>
                                <f:Form runat="server" ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px"
                                    ShowHeader="false" LabelWidth="80px" CssStyle="border-top: 1px solid #99bce8;">
                                    <Rows>
                                        <f:FormRow ColumnWidths="50% 20% 20% 10%">
                                            <Items>
                                                <f:TextBox runat="server" ID="tbxTITLE" Label="备忘主题" MaxLength="50" Required="true" ShowRedStar="True"></f:TextBox>
                                                <f:DatePicker runat="server" Required="true" EnableEdit="true" Label="提醒日期" EmptyText="请选择日期" ID="dptWARINGRQ" ShowRedStar="True"></f:DatePicker>
                                                <f:DropDownList ID="rblISPRIVATE" runat="server" Label="查看权限" AutoPostBack="true" LabelWidth="80px" OnSelectedIndexChanged="rblISPRIVATE_SelectedIndexChanged">
                                                    <f:ListItem  Selected="true" Text="私有" Value="N" />
                                                    <f:ListItem  Text="公有" Value="Y"  />
                                                    <f:ListItem Text="强制" Value="S" />
                                                </f:DropDownList>
                                                <%--<f:RadioButtonList ID="rblISPRIVATE" runat="server" Label="查看权限" AutoPostBack="true" LabelWidth="80px" OnSelectedIndexChanged="rblISPRIVATE_SelectedIndexChanged">
                                                    <f:RadioItem Selected="true" Text="私有" Value="N" />
                                                    <f:RadioItem Text="公有" Value="Y" />
                                                    <f:RadioItem Text="强制" Value="S" />
                                                </f:RadioButtonList>--%>
                                                <f:Button runat="server" ID="btnDistribution" OnClick="btnDistribution_Click" Text="分配人员" Icon="GroupAdd" EnableDefaultState="false"></f:Button>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:TextArea ID="taxMEMO" runat="server" Label="备忘内容" AutoGrowHeight="true" AutoGrowHeightMin="100" AutoGrowHeightMax="250">
                                                </f:TextArea>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow Hidden="true">
                                            <Items>
                                                <f:HiddenField ID="hfdLOOKPER" runat="server" />
                                                <f:HiddenField ID="hfdIsNew" runat="server" Text="Y" />
                                                <f:HiddenField ID="hfdSeqno" runat="server" Text="" />
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                            </Items>
                        </f:Panel>
                        <f:Grid ID="GridPerson" runat="server" BoxFlex="3" ShowHeader="false" DataKeyNames="USERID,USERNAME,FLAGNAME,FLAG,LRRQ,LOOKRQ">
                            <Columns>
                                <f:BoundField Width="70px" DataField="USERID" Hidden="true" />
                                <f:BoundField Width="230px" DataField="USERNAME" HeaderText="姓名" ExpandUnusedSpace="true" EnableHeaderMenu="false" />
                                <f:BoundField Width="70px" DataField="FLAGNAME" HeaderText="状态" EnableHeaderMenu="false" TextAlign="Center" />
                                <f:BoundField Hidden="true" DataField="FLAG" HeaderText="状态" EnableHeaderMenu="false" />
                                <f:BoundField Hidden="true" DataField="LRRQ" HeaderText="录入日期" EnableHeaderMenu="false" />
                                <f:BoundField Hidden="true" DataField="LOOKRQ" HeaderText="查看日期" EnableHeaderMenu="false" />
                            </Columns>
                        </f:Grid>
                    </Items>
                </f:Panel>
            </Items>
        </f:Panel>
        <f:Window ID="winPerson" Title="选择人员" BodyPadding="0px" IsModal="true" Hidden="true"
            EnableMaximize="true" EnableResize="true" Target="Top" Width="550px" Height="300px" Layout="Fit"
            runat="server">
            <Items>
                <f:Form ID="FormPersons" runat="server" ShowBorder="false" ShowHeader="false" AutoScroll="true">
                    <Rows>
                        <f:FormRow>
                            <Items>
                                <f:CheckBoxList ID="chkPersons" runat="server" ColumnNumber="5" ColumnWidth="16%" ColumnVertical="true"></f:CheckBoxList>
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
            </Items>
            <Toolbars>
                <f:Toolbar Position="Top" runat="server">
                    <Items>
                        <f:DropDownList ID="ddlDept" runat="server" Label="请选择角色" AutoPostBack="true" OnSelectedIndexChanged="ddlDept_SelectedIndexChanged"></f:DropDownList>
                    </Items>
                </f:Toolbar>
                <f:Toolbar Position="Bottom" ToolbarAlign="Right" ID="toolbar2" runat="server">
                    <Items>
                        <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="备注：可以选择多位人员！" runat="server" />
                        <f:ToolbarFill ID="ToolbarFill3" runat="server">
                        </f:ToolbarFill>
                        <f:Button ID="btnCondSave" ValidateForms="formRoles" Icon="DiskEdit" runat="server"
                            OnClick="btnCondSave_Click" CssStyle="margin-right:20px" Text="保存后关闭">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
    </form>
</body>
</html>

