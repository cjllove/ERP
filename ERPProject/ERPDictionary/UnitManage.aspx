<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnitManage.aspx.cs" Inherits="ERPProject.ERPDictionary.UnitManage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="MainPanel"
            runat="server" />
        <f:Panel ID="MainPanel" runat="server" Layout="HBox" ShowBorder="false" BoxConfigAlign="Stretch" BoxConfigPosition="Start" ShowHeader="False">
            <Items>
                <f:Panel ID="PanelLeft" ShowBorder="false" BoxFlex="1" BodyPadding="0px"
                    Layout="Fit" ShowHeader="False" runat="server">
                    <Items>
                        <f:Grid ID="GridUnit" ShowBorder="false" ShowHeader="false" runat="server" DataKeyNames="Code,Name"
                            CssStyle="border-right: 1px solid #99bce8;" EnableColumnLines="true"
                            EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridUnit_RowDoubleClick">
                            <Columns>
                                <f:BoundField Width="80px" DataField="Code" HeaderText="单位编码" />
                                <f:BoundField Width="100px" DataField="Name" HeaderText="单位名称" ExpandUnusedSpace="true" />
                                <f:BoundField DataField="FLAG" Hidden="true" />
                            </Columns>
                        </f:Grid>
                    </Items>
                </f:Panel>
                <f:Panel ID="PanelRight" ShowBorder="false" BoxFlex="2" BodyPadding="0px" Layout="Fit" ShowHeader="False" runat="server">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar1" runat="server">
                            <Items>
                                <f:Button ID="btnNew" Text="新增" Icon="Add" runat="server" OnClick="btnNew_Click" EnableDefaultState="false" ></f:Button>
                                <f:Button ID="btnDelete" Text="删除" Icon="Delete" runat="server" ConfirmText="是否确认删除此信息?" OnClick="btnDelete_Click" EnableDefaultState="false"></f:Button>
                                <f:Button ID="btnSave" Text="保存" Icon="Disk" ValidateForms="FormUnit" runat="server" OnClick="btnSave_Click" EnableDefaultState="false"></f:Button>
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Items>
                        <f:Form ID="FormUnit" ShowBorder="false" BodyPadding="10px" ShowHeader="False" runat="server" LabelWidth="100px" EnableCollapse="true">
                            <Rows>
                                <f:FormRow>
                                    <Items>
                                        <f:CheckBox ID="ckbFlag" runat="server" Label="是否有效"  />
                                    </Items>
                                </f:FormRow>
                                <f:FormRow Width="300px">
                                    <Items>
                                        <f:TextBox ID="tbxCode" Label="单位编码" runat="server" MaxLength="15"  ShowRedStar="true" />
                                    </Items>
                                </f:FormRow>
                                <f:FormRow Width="300px">     
                                    <Items>
                                        <f:TextBox ID="tbxName" Label="单位名称" runat="server" MaxLength="30" ShowRedStar="true" />
                                    </Items>
                                </f:FormRow>
                            </Rows>
                        </f:Form>
                        <f:HiddenField ID="hfdIsNew" runat="server" Text="Y" />
                    </Items>
                </f:Panel>
            </Items>
        </f:Panel>
    </form>
</body>
</html>
