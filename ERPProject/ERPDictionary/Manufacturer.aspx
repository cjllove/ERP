<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Manufacturer.aspx.cs" Inherits="ERPProject.ERPDictionary.Manufacturer" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>生产厂家资料维护</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="MainPanel"
            runat="server" />
        <f:Panel ID="MainPanel" runat="server" BodyPadding="0px" ShowBorder="False" Layout="Anchor" ShowHeader="False">
            <Items>
                <f:Grid ID="GridProducer" ShowBorder="False" EnableCheckBoxSelect="true" AnchorValue="100% -155px" ShowHeader="false" runat="server" DataKeyNames="Guid" EnableRowDoubleClickEvent="true" 
                    OnRowDoubleClick="GridProducer_RowDoubleClick"  OnPageIndexChange="Grid1_PageIndexChange" PageSize="50" IsDatabasePaging="true"  AllowPaging="true" EnableColumnLines="true" >
                    <Toolbars>
                        <f:Toolbar ID="Toolbar1" runat="server">
                            <Items>
                                <f:TriggerBox ID="trbSearch" runat="server" Label=" 查询信息" EmptyText="输入生产商编码或者生产商名称关键词进行查询" TriggerIcon="Search" LabelWidth="60px" Width="490px" OnTriggerClick="trbSearch_TriggerClick" />
                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                <f:Button ID="btnNew" Text="新 增" Icon="Add" runat="server" OnClick="btnNew_Click"></f:Button>
                                <f:ToolbarSeparator runat="server"></f:ToolbarSeparator>
                                <f:Button ID="btnDelete" Text="删 除" Icon="Delete" ConfirmText="是否确认删除此信息?" runat="server" OnClick="btnDelete_Click"></f:Button>
                                <f:ToolbarSeparator runat="server"></f:ToolbarSeparator>
                                <f:Button ID="btnSave" Text="保 存" Icon="Disk" runat="server" ValidateForms="FormProducer" OnClick="btnSave_Click"></f:Button>
                                <f:ToolbarSeparator runat="server"></f:ToolbarSeparator>
                                <f:Button ID="btExp" CssStyle="margin-left: 15px;" OnClick="btExp_Click" Icon="PageExcel" Text="导 出" EnableAjax="false" DisableControlBeforePostBack="false" runat="server" />
                                <f:ToolbarSeparator runat="server"></f:ToolbarSeparator>
                                <f:Button ID="btnSearch" OnClick="trbSearch_TriggerClick" Icon="Magnifier" Text="查 询" runat="server"></f:Button>
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Columns>
                        <f:BoundField Width="130px" DataField="CODE" HeaderText="生产商编码" />
                        <f:BoundField Width="130px" DataField="NAME" ExpandUnusedSpace="true" HeaderText="生产商名称" />
                        <f:BoundField Width="130px" DataField="TEL" HeaderText="公司电话" />
                        <f:BoundField Width="90px" DataField="LINKMAN" HeaderText="联系人" />
                        <f:BoundField Width="230px" DataField="LOGINADDR" HeaderText="注册地址" />
                    </Columns>
                </f:Grid>
                <f:Form ID="FormProducer" ShowBorder="False" BodyPadding="10px" ShowHeader="False" runat="server"
                    CssStyle="border-top: 1px solid #99bce8;" LabelWidth="80px">
                    <Rows>
                        <f:FormRow>
                            <Items>
                                <f:TextBox ID="tbxCODE" Label="生产商编码" runat="server" Required="true" ShowRedStar="true" MaxLength="50" />
                                <f:DropDownList runat="server" ID="ddlCORPKID" Label="企业性质" EnableEdit="true" ForceSelection="true"></f:DropDownList>
                                <f:DropDownList runat="server" ID="ddlCORPTYPE" Label="企业类别" EnableEdit="true" ForceSelection="true">
                                </f:DropDownList>
                                <f:TextBox ID="tbxLEADER" Label="法人代表" runat="server" MaxLength="20" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow ColumnWidths="50% 25% 25%">
                            <Items>
                                <f:TextBox ID="tbxNAME" Label="生产商名称" runat="server" Required="true" ShowRedStar="true" MaxLength="60" />
                                <f:TextBox ID="tbxTEL" Label="公司电话" runat="server" MaxLength="20" />
                                <f:TextBox ID="tbxFAX" Label="公司传真" runat="server" MaxLength="20" />

                            </Items>
                        </f:FormRow>
                        <f:FormRow ColumnWidths="50% 25% 25%">
                            <Items>
                                <f:TextBox ID="tbxENAME" Label="英文名称" runat="server" MaxLength="60" />
                                <f:TextBox ID="tbxZIP" Label="邮政编码" runat="server" MaxLength="6" />
                                <f:TextBox ID="tbxEMAIL" Label="电子邮箱" runat="server" MaxLength="60" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow ColumnWidths="50% 50%">
                            <Items>
                                <f:TextBox ID="tbxLOGINADDR" Label="注册地址" runat="server" MaxLength="60" />
                                <f:TextBox ID="tbxJYFW" Label="经营范围" runat="server" MaxLength="60" />

                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:TextBox ID="tbxLINKFAX" Label="联系人传真" runat="server" MaxLength="40" />

                                <f:TextBox ID="tbxLINKMAN" Label="联系人姓名" runat="server" MaxLength="20" />
                                <f:TextBox ID="tbxLINKTEL" Label="联系人电话" runat="server" MaxLength="40" />
                                <f:TextBox ID="tbxLINKEMAIL" Label="联系人邮箱" runat="server" MaxLength="40" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow Hidden="true">
                            <Items>
                                <f:HiddenField ID="hfdIsNew" runat="server" Text="Y" />
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
            </Items>
        </f:Panel>
    </form>
</body>
</html>
