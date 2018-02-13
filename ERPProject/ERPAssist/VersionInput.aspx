<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VersionInput.aspx.cs" Inherits="ERPProject.ERPAssist.VersionInput" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>系统发布历史</title>
    <style type="text/css">
        .x-grid-cell-rowbody {
            border-top: 1px solid #ccc;
            border-right: 1px solid #ccc;
        }
        /*pre强制换行*/
        pre{ border:#CCC solid 1px; padding:10px; background-color:#F9F9F9;
        font-family:"Courier New", Courier, Arial; font-size:12px; line-height:1.75;
        white-space: pre-wrap!important;    /*保留空白，进行换行*/
        word-wrap: break-word!important;    /*连续字符换行*/
        /*white-space:normal!important;*/    /*忽略空白，进行换行*/}
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel runat="server" ID="Panel1" Title="备忘录" ShowBorder="true" AnchorValue="100%" ShowHeader="false" BodyPadding="0px" Layout="Anchor">
            <Items>
                <f:Grid ID="GridList" runat="server" AnchorValue="100% -120" ShowBorder="false" ShowHeader="false" EnableCollapse="true" EnableColumnLines="true"
                    AutoScroll="true" EnableTextSelection="true" DataKeyNames="VERSION" EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridList_RowDoubleClick"
                    AllowSorting="true" EnableHeaderMenu="true" SortField="SEQNO" OnSort="GridList_Sort" SortDirection="ASC"
                    IsDatabasePaging="true" AllowPaging="true" PageSize="100" OnPageIndexChange="GridList_PageIndexChange">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar2" runat="server">
                            <Items>
                                <f:ToolbarText ID="ToolbarText2" Text="操作信息：双击打开单据！" runat="server" />
                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                <f:Button ID="btnAddNew" Icon="Erase" Text="新　增" EnablePostBack="true" runat="server" OnClick="btnAddNew_Click" EnableDefaultState="false"/>
                                <f:ToolbarSeparator runat="server" />

                                <f:Button ID="btnSave" Icon="UserTick" Text="保　存" EnablePostBack="true" runat="server" ConfirmText="是否确认提交选中单据？" OnClick="btnSave_Click" EnableDefaultState="false"/>
                                <f:ToolbarSeparator runat="server" />

                                <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnablePostBack="true" runat="server" OnClick="btnSearch_Click" EnableDefaultState="false"/>

                            </Items>
                        </f:Toolbar>
                    </Toolbars>

                    <Columns>
                        <f:RowNumberField Width="50px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                        <f:BoundField Width="180px" DataField="SYSNAME" HeaderText="系统名称" SortField="TITLE" />
                        <f:BoundField Width="100px" DataField="VERSION" HeaderText="版本号" SortField="LRNAME" TextAlign="Center" />
                        <f:BoundField Width="120px" DataField="UPTDATE" HeaderText="发布日期" SortField="LRRQ" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="0px" DataField="FLAG" HeaderText="状态" SortField="STATUS" TextAlign="Center" DataFormatString="" Hidden="true" />
                        <f:BoundField Width="100px" DataField="UPTPER" HeaderText="发布人" SortField="CLOSENAME" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="TESTPER" HeaderText="测试人" SortField="CLOSENAME" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="CHECKPER" HeaderText="验收人" SortField="CLOSENAME" TextAlign="Center" />
                        <f:TemplateField ColumnID="expander" RenderAsRowExpander="true">
                            <ItemTemplate>
                                <div class="expander">
                                    <div style="line-height: 25px;">
                                        <strong>更新内容：</strong>
                                        <pre><%# Eval("UPTMEMO") %></pre>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </f:TemplateField>
                    </Columns>
                </f:Grid>

                <f:Panel ID="Panel2" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Column" ShowHeader="false" CssStyle="padding:10px; border-top: 1px solid #99bce8;">
                    <Items>
                        <f:Panel ID="Panel3" ShowBorder="false" BodyPadding="0px" ColumnWidth="50%" ShowHeader="False" runat="server">

                            <Items>
                                <f:Form ID="FormList" runat="server" ShowBorder="false" LabelWidth="80px" ShowHeader="false">
                                    <Rows>
                                        <f:FormRow>
                                            <Items>
                                                <f:TextBox runat="server" ID="tbxSYSNAME" Label="系统名称"></f:TextBox>
                                                <f:TextBox runat="server" ID="tbxVERSION" Label="版本号" Required="true" ShowRedStar="True"></f:TextBox>


                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:DatePicker runat="server" Required="true" EnableEdit="true" Label="发布日期" EmptyText="请选择日期" ID="dptUPTDATE" ShowRedStar="True"></f:DatePicker>
                                                <f:DropDownList runat="server" ID="ddlUPTPER" Label="发布人"></f:DropDownList>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>

                                                <f:DropDownList runat="server" ID="ddlTESTPER" Label="测试人"></f:DropDownList>
                                                <f:DropDownList runat="server" ID="ddlCHECKPER" Label="验收人"></f:DropDownList>
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                            </Items>
                        </f:Panel>

                        <f:Panel ID="Panel4" runat="server" ColumnWidth="50%" ShowBorder="false" ShowHeader="false">
                            <Items>
                                <f:Form runat="server" ShowBorder="false" ShowHeader="false">
                                    <Rows>
                                        <f:FormRow ColumnWidths="5% 95%">
                                            <Items>
                                                <f:Label runat="server"></f:Label>
                                                <f:TextArea ID="tbxUPTMEMO" runat="server" Label="更新内容" Height="70" AutoGrowHeight="true"></f:TextArea>
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>

                            </Items>
                        </f:Panel>
                    </Items>
                </f:Panel>
            </Items>
        </f:Panel>
        <f:HiddenField runat="server" ID="hfdValue" Text="N"></f:HiddenField>
    </form>
</body>
</html>
