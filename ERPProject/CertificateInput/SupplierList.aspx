<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SupplierList.aspx.cs" Inherits="ERPProject.CertificateInput.SupplierList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
        <form id="form1" runat="server">
    <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="True" BodyPadding="0px" Layout="Anchor"
            ShowHeader="false">
            <Toolbars>
                <f:Toolbar ID="Toolbar2" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="btnClosePostBack" Text="确 定" Icon="SystemSave" runat="server" OnClick="btnClosePostBack_Click">
                        </f:Button>
                        <f:Button ID="btnClose" Text="关 闭" Icon="SystemClose" runat="server" OnClick="btnClose_Click">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Form ID="FormSearch" ShowBorder="false" AutoScroll="false" BodyPadding="5px 30px 0px 30px"
                    ShowHeader="False" LabelWidth="60px" runat="server">
                    <Rows>
                        <f:FormRow>
                            <Items>
                                <f:TriggerBox ID="trbSearch" runat="server" LabelWidth="100px" Label="查询信息" 
                                    EmptyText="输入机构编号/名称" TriggerIcon="Search" OnTriggerClick="trbSearch_TriggerClick" />
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
                <f:Grid ID="GridSupplier" AnchorValue="100% -30" ShowBorder="true" ShowHeader="false" EnableRowDoubleClickEvent="true"
                    AutoScroll="true" runat="server" DataKeyNames="SUPID" EnableColumnLines="true"
                    OnRowDoubleClick="GridSupplier_RowDoubleClick" IsDatabasePaging="true" 
                    PageSize="30" AllowPaging="true" OnPageIndexChange="GridSupplier_PageIndexChange">
                    <Columns>
                        <f:BoundField Width="120px"  DataField="SUPID" HeaderText="供应商编号" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="260px"  DataField="SUPNAME" HeaderText="机构名称" EnableColumnHide="true" EnableHeaderMenu="false" TextAlign="left" ExpandUnusedSpace="true" />
                        <f:BoundField Width="100px"  DataField="SUPCAT" Hidden="true" HeaderText="供应商分类" EnableColumnHide="true" EnableHeaderMenu="false" TextAlign="Center" />
                        <f:BoundField Width="100px"  DataField="REGID" Hidden="true" HeaderText="地区" EnableColumnHide="true" EnableHeaderMenu="false" TextAlign="Center" ExpandUnusedSpace="true"/>
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
            <f:HiddenField ID="hfdLicType" runat="server"></f:HiddenField>
    </form>
    
</body>
</html>
