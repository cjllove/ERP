<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QuestionManage.aspx.cs" Inherits="ERPProject.ERPProject.QuestionManage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>项目问题管理</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="PanelMain" runat="server" />
        <f:Panel ID="PanelMain" runat="server" Layout="Anchor" ShowBorder="false" BodyPadding="0px" ShowHeader="false">
            <Toolbars>
                <f:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                        <f:HyperLink ID="lnkImage" runat="server" Text="问题截图" Hidden="true" Target="_blank"></f:HyperLink>
                        <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                        <f:Button ID="btnSelect" CssStyle="margin-left: 15px;" Icon="Add" Text="导入" runat="server" OnClick="btnSelect_Click">
                        </f:Button>
                        <f:Button ID="btnNew" CssStyle="margin-left: 15px;" Icon="Add" Text="新增" runat="server" OnClick="btnNew_Click">
                        </f:Button>
                        <f:Button ID="btnDelete" CssStyle="margin-left: 15px;" Icon="Delete" OnClick="btnDelete_Click"
                            Text="删除" EnablePostBack="true" runat="server">
                        </f:Button>
                        <f:Button ID="btSave" CssStyle="margin-left: 15px;margin-right: 11px;" OnClick="btSave_Click"
                            Icon="Disk" Text="保存" DisableControlBeforePostBack="false" runat="server" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Form ID="FormQuest" ShowBorder="false" AutoScroll="false" BodyPadding="5px 10px 0px 10px"
                    ShowHeader="False" LabelWidth="80px" runat="server">
                    <Rows>
                        <f:FormRow ColumnWidths=" 15%  35% 25% 25%">
                            <Items>
                                <f:TextBox ID="hfdSEQ" Label="系号" Enabled="false" runat="server" />
                                <f:DropDownList ID="ddlXMBH" runat="server" Label="项目名称" Required="true" ShowRedStar="true">
                                </f:DropDownList>
                                <f:DropDownList ID="ddlQTYPE" runat="server" Label="问题类型" Required="true" ShowRedStar="true"></f:DropDownList>
                                <f:DropDownList ID="ddlQLEVEL" runat="server" Label="问题级别" Required="true" ShowRedStar="true"></f:DropDownList>
                            </Items>
                        </f:FormRow>
                        <f:FormRow ColumnWidths="50% 25% 25%">
                            <Items>
                                <f:FileUpload ID="fudIMAGE" runat="server" Label="问题截图">
                                </f:FileUpload>
                                <f:DropDownList ID="ddlSTATUS" runat="server" Label="问题状态" Required="true" ShowRedStar="true" AutoPostBack="true" OnSelectedIndexChanged="ddlSTATUS_Changed" />
                                <f:DatePicker ID="dpkENDTIME" runat="server" Label="希望解决日"></f:DatePicker>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:DropDownList ID="ddlCREUSER" runat="server" Label="提出人"></f:DropDownList>
                                <f:DatePicker ID="dpkCRETIME" runat="server" Label="提出时间"></f:DatePicker>
                                <f:DropDownList ID="ddlDOUSER" runat="server" Label="解决人"></f:DropDownList>
                                <f:DatePicker ID="dpkDOTIME" runat="server" Label="实际解决日"></f:DatePicker>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:TextArea ID="txaQUESTION" runat="server" Label="问题描述" Height="45px" Required="true" ShowRedStar="true"></f:TextArea>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:TextArea ID="txaHOWDO" runat="server" Label="解决方案" Height="45px"></f:TextArea>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>

                                <f:HiddenField ID="hfdISNEW" runat="server" Text="Y" />
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
                <f:Grid ID="GridQuestion" ShowBorder="false" ShowHeader="false" AllowSorting="false" AnchorValue="100% -190"
                    AutoScroll="true" runat="server" DataKeyNames="SEQ" CssStyle="border-top: 1px solid #99bce8;" EnableAjaxLoading="true"
                    EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridQuestion_RowDoubleClick">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar2" runat="server">
                            <Items>
                                <f:DatePicker ID="lstCRETIME1" runat="server" LabelWidth="70px" Label="提出时间"></f:DatePicker>
                                <f:DatePicker ID="lstCRETIME2" runat="server" LabelWidth="30px" Label="　至" LabelSeparator=""></f:DatePicker>

                                <f:DropDownList ID="lstDOUSER" runat="server" LabelWidth="55px" Label="解决人"></f:DropDownList>
                                <f:DropDownList ID="lstSTATUS" runat="server" LabelWidth="70px" Label="问题状态"></f:DropDownList>

                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                <f:Button ID="btnSearch" CssStyle="margin-left: 15px;margin-right: 11px;" OnClick="btnSearch_Click"
                                    Icon="Magnifier" Text="查询" DisableControlBeforePostBack="false" runat="server" />

                                <f:Button ID="btnExport" CssStyle="margin-left: 15px;" Icon="DatabaseGo" OnClick="btnExport_Click"
                                    Text="导出" EnablePostBack="true" runat="server">
                                </f:Button>
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Columns>
                        <f:RowNumberField />
                        <f:BoundField Width="40px" DataField="SEQ" HeaderText="系号" EnableHeaderMenu="false" EnableColumnHide="true" TextAlign="Center" />
                        <f:BoundField Width="170px" DataField="XMMC" HeaderText="项目名称" Hidden="true" />
                        <f:BoundField Width="70px" DataField="QTYPENAME" HeaderText="问题类别" EnableHeaderMenu="false" EnableColumnHide="true" TextAlign="Center" />
                        <f:BoundField Width="70px" DataField="QLEVELNAME" HeaderText="问题级别" EnableHeaderMenu="false" EnableColumnHide="true" TextAlign="Center" />
                        <f:BoundField Width="70px" DataField="STATUSNAME" HeaderText="当前状态" EnableHeaderMenu="false" EnableColumnHide="true" TextAlign="Center" />
                        <f:BoundField Width="270px" DataField="QUESTION" HeaderText="问题描述" EnableHeaderMenu="false" EnableColumnHide="true" />
                        <f:BoundField Width="90px" DataField="ENDTIME" HeaderText="希望解决日" DataFormatString="{0:yyyy-MM-dd}" EnableHeaderMenu="false" TextAlign="Center" />
                        <f:BoundField Width="70px" DataField="CREUSER" HeaderText="提出人" EnableHeaderMenu="false" />
                        <f:BoundField Width="80px" DataField="CRETIME" HeaderText="提出时间" DataFormatString="{0:yyyy-MM-dd}" EnableHeaderMenu="false" />
                        <f:BoundField Width="70px" DataField="DOUSER" HeaderText="解决人" EnableHeaderMenu="false" />
                        <f:BoundField Width="270px" DataField="HOWDO" HeaderText="解决方案" EnableHeaderMenu="false" />
                        <f:BoundField Width="80px" DataField="DOTIME" HeaderText="实际解决日" DataFormatString="{0:yyyy-MM-dd}" EnableHeaderMenu="false" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
        <f:HiddenField ID="hfdFlag" runat="server" />
    </form>
</body>
</html>
