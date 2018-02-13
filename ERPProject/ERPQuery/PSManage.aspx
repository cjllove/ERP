<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PSManage.aspx.cs" Inherits="ERPProject.ERPQuery.PSManage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>配送日程管理</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="TabStrip1" runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right" EnableTabCloseMenu="false" ActiveTabIndex="1" runat="server">
            <Tabs>
                <f:Tab Title="配送日程维护" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel runat="server" BodyPadding="0px" ShowBorder="False" Layout="Anchor" ShowHeader="False">
                            <Items>
                                <f:Grid ID="GridMange" ShowBorder="False" EnableCheckBoxSelect="true" AnchorValue="100% -100px" ShowHeader="false" runat="server" DataKeyNames="DEPTID,TYPE"
                                    OnPageIndexChange="GridMange_PageIndexChange" PageSize="50" IsDatabasePaging="true" AllowPaging="true" EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridMange_RowDoubleClick" EnableColumnLines="true">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar1" runat="server">
                                            <Items>
                                                <f:DropDownList ID="StrDeptid" runat="server" Label="科室" LabelWidth="37px"></f:DropDownList>
                                                <f:DropDownList ID="StrType" runat="server" Label="作业类型" LabelWidth="60px"></f:DropDownList>
                                                <f:TriggerBox ID="StrMANAGER" runat="server" Label=" 负责人" LabelWidth="50px" EmptyText="负责人关键词" TriggerIcon="Search" Width="250px" OnTriggerClick="trbSearch_TriggerClick" />
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="btnNew" Text="新 增" Icon="Add" runat="server" OnClick="btnNew_Click"></f:Button>
                                                <f:Button ID="btnDelete" Text="删 除" Icon="Delete" ConfirmText="是否确认删除选中行信息?" runat="server" OnClick="btnDelete_Click"></f:Button>
                                                <f:Button ID="btnSave" Text="保 存" Icon="Disk" runat="server" ValidateForms="FormMange" OnClick="btnSave_Click"></f:Button>
                                                <f:ToolbarSeparator runat="server"></f:ToolbarSeparator>
                                                <f:Button ID="btnSearch" OnClick="trbSearch_TriggerClick" Icon="Magnifier" Text="查 询" runat="server"></f:Button>
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" />
                                        <f:BoundField Width="60px" DataField="DEPTID" HeaderText="科室编码" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="120px" DataField="DEPTNAME" HeaderText="科室名称" TextAlign="Center" />
                                        <f:BoundField Width="50px" DataField="TYPE" HeaderText="作业类型" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="100px" DataField="TYPENAME" HeaderText="作业类型" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="MANAGERNAME" HeaderText="负责人" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="MONDAY" HeaderText="星期一" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="TUESDAY" HeaderText="星期二" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="WEDNESDAY" HeaderText="星期三" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="THURSDAY" HeaderText="星期四" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="FRIDAY" HeaderText="星期五" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="SATURDAY" HeaderText="星期六" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="SUNDAY" HeaderText="星期日" TextAlign="Center" />
                                        <f:BoundField Width="180px" DataField="MEMO" HeaderText="备注" TextAlign="Center" ExpandUnusedSpace="true" />
                                    </Columns>
                                </f:Grid>
                                <f:Form ID="FormMange" ShowBorder="False" BodyPadding="10px" ShowHeader="False" runat="server"
                                    CssStyle="border-top: 1px solid #99bce8;" LabelWidth="80px">
                                    <Rows>
                                        <f:FormRow>
                                            <Items>
                                                <f:DropDownList runat="server" ID="ddlDEPTID" Label="科室" ShowRedStar="true" Required="true" EnableEdit="true" ForceSelection="true"></f:DropDownList>
                                                <f:DropDownList runat="server" ID="ddlTYPE" Label="业务类型" ShowRedStar="true" Required="true" EnableEdit="true" ForceSelection="true"></f:DropDownList>
                                                <f:DropDownList runat="server" ID="ddlMANAGER" Label="负责人" ShowRedStar="true" Required="true" EnableEdit="true" ForceSelection="true"></f:DropDownList>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:CheckBox runat="server" ID="chkMONDAY" Text="星期一"></f:CheckBox>
                                                <f:CheckBox runat="server" ID="chkTUESDAY" Text="星期二"></f:CheckBox>
                                                <f:CheckBox runat="server" ID="chkWEDNESDAY" Text="星期三"></f:CheckBox>
                                                <f:CheckBox runat="server" ID="chkTHURSDAY" Text="星期四"></f:CheckBox>
                                                <f:CheckBox runat="server" ID="chkFRIDAY" Text="星期五"></f:CheckBox>
                                                <f:CheckBox runat="server" ID="chkSATURDAY" Text="星期六"></f:CheckBox>
                                                <f:CheckBox runat="server" ID="chkSUNDAY" Text="星期日"></f:CheckBox>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow ColumnWidths="100%">
                                            <Items>
                                                <f:TextBox ID="tbxMEMO" Label="备注" runat="server" MaxLength="100" />
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="配送日程查看" runat="server" Layout="Fit">
                    <Items>
                        <f:Panel runat="server" BodyPadding="0px" ShowBorder="False" Layout="Anchor" ShowHeader="False">
                            <Items>
                                <f:Grid ID="GridView" ShowBorder="False" ExpandAllRowExpanders="true" EnableCollapse="true" EnableRowLines="true" EnableCheckBoxSelect="false" AnchorValue="100% -100px" ShowHeader="false" runat="server" DataKeyNames="DEPTID,TYPE">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar2" runat="server">
                                            <Items>
                                                <f:DropDownList ID="ddlDept" runat="server" Label="科室" CssStyle="margin-left:30px" LabelWidth="37px"></f:DropDownList>
                                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                <f:ToolbarSeparator runat="server"></f:ToolbarSeparator>
                                                <f:Button ID="btnSch" OnClick="btnSch_Click" Icon="Magnifier" Text="查 询" runat="server"></f:Button>
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Columns>
                                        <f:RowNumberField Width="35px" runat="server"></f:RowNumberField>
                                        <f:GroupField Width="250px" runat="server" HeaderText="星期一">
                                            <Columns>
                                                <f:ImageField Width="50px" DataImageUrlField="MONDAY" TextAlign="Center" DataImageUrlFormatString="~/res/images/work/{0}.png" HeaderText="配送"></f:ImageField>
                                                <f:ImageField Width="60px" DataImageUrlField="MONDAY" TextAlign="Center" DataImageUrlFormatString="~/res/images/work/{0}.png" HeaderText="条码回收"></f:ImageField>
                                                <f:BoundField Width="100px" HeaderText="科室" runat="server" TextAlign="Center"></f:BoundField>
                                            </Columns>
                                        </f:GroupField>
                                        <f:ImageField Width="150px" DataImageUrlField="MONDAY" TextAlign="Center" DataImageUrlFormatString="~/res/images/work/{0}.png" HeaderText="星期一"></f:ImageField>
                                        <f:ImageField Width="150px" DataImageUrlField="TUESDAY" TextAlign="Center" DataImageUrlFormatString="~/res/images/work/{0}.png" HeaderText="星期二"></f:ImageField>
                                        <f:ImageField Width="150px" DataImageUrlField="WEDNESDAY" TextAlign="Center" DataImageUrlFormatString="~/res/images/work/{0}.png" HeaderText="星期三"></f:ImageField>
                                        <f:ImageField Width="150px" DataImageUrlField="THURSDAY" TextAlign="Center" DataImageUrlFormatString="~/res/images/work/{0}.png" HeaderText="星期四"></f:ImageField>
                                        <f:ImageField Width="150px" DataImageUrlField="FRIDAY" TextAlign="Center" DataImageUrlFormatString="~/res/images/work/{0}.png" HeaderText="星期五"></f:ImageField>
                                        <f:ImageField Width="150px" DataImageUrlField="SATURDAY" TextAlign="Center" DataImageUrlFormatString="~/res/images/work/{0}.png" HeaderText="星期六"></f:ImageField>
                                        <f:ImageField Width="150px" DataImageUrlField="SUNDAY" TextAlign="Center" DataImageUrlFormatString="~/res/images/work/{0}.png" HeaderText="星期日"></f:ImageField>
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
    </form>
</body>
</html>
