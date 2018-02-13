<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ToDoQuery.aspx.cs" Inherits="ERPProject.ERPQuery.ToDoQuery" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>待办事谊查询</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="PanelMain" runat="server" />
        <f:Panel ID="PanelMain" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBOX" ShowHeader="false">
            <Items>
                <f:Panel ID="Panel3" ShowBorder="false" BodyPadding="0px"
                    Layout="Anchor" ShowHeader="False" runat="server">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar2" runat="server">
                            <Items>
                                <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="备注：待办事宜历史情况查询！" runat="server" />
                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnablePostBack="true" runat="server" OnClick="bntSearch_Click" EnableDefaultState="false"/>
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Items>
                        <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                            ShowHeader="False" LabelWidth="90px" runat="server">
                            <Rows>
                                <f:FormRow>
                                    <Items>
                                        <f:DatePicker ID="lstSCRQ1" runat="server" Label="查询日期" Required="true" ShowRedStar="true" />
                                        <f:DatePicker ID="lstSCRQ2" runat="server" Label="　　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                                        <f:DropDownList ID="lstDOTYPE" runat="server" Label="查询类别" />
                                        <f:DropDownList ID="lstFLAG" runat="server" Label="查询状态">
                                            <f:ListItem Text="--请选择--" Value="" />
                                            <f:ListItem Text="信息已处理，不显示" Value="Y" />
                                            <f:ListItem Text="信息未处理，显示" Value="N" />
                                        </f:DropDownList>
                                    </Items>
                                </f:FormRow>
                            </Rows>
                        </f:Form>
                    </Items>
                </f:Panel>
                <f:Grid ID="GridList" BoxFlex="1" AnchorValue="100% -70" ShowBorder="false" ShowHeader="false" DataKeyNames="SEQNO" IsDatabasePaging="true"
                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" AllowPaging="true"
                    PageSize="100" OnPageIndexChange="GridList_PageIndexChange" EnableColumnLines="true"  OnPreRowDataBound="Grid1_PreRowDataBound"  OnRowCommand="Grid1_RowCommand">
                    <Columns>
                        <f:RowNumberField Width="30px" TextAlign="Center"></f:RowNumberField>
                        <f:BoundField DataField="SEQNO" Hidden="true" />
                        <f:LinkButtonField TextAlign="Center" ConfirmText="你确定要这么做么？" ConfirmTarget="Top"
                            ColumnID="Delete" Width="80px" CommandName="Delete" Text="删除显示" />
                        <f:BoundField Width="180px" DataField="DONAME" HeaderText="类别" TextAlign="Left" ExpandUnusedSpace="true" />
                        <f:BoundField Width="150px" DataField="FLAGNAME" HeaderText="状态" TextAlign="Center" />
                        <f:BoundField Width="150px" DataField="ROLELISTNAME" HeaderText="待办事谊接收角色" />
                        <f:BoundField Width="100px" DataField="USERNAME" HeaderText="待办事谊接收人" />
                        <f:BoundField Width="120px" DataField="FUNCNAME" HeaderText="执行功能" TextAlign="Center" />
                        <f:BoundField Width="150px" DataField="PARA" HeaderText="执行参数" TextAlign="Center" />
                        <%--<f:BoundField Width="60px" DataField="DEPTID" HeaderText="部门/科室" TextAlign="Center" />--%>
                        <f:BoundField Width="130px" DataField="SCRQ" HeaderText="生成时间" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
                        <f:BoundField Width="60px" DataField="DOUSERNAME" HeaderText="处理人" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="DORQ" HeaderText="处理时间" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="100px" DataField="MEMO" HeaderText="备注" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
</body>
</html>
