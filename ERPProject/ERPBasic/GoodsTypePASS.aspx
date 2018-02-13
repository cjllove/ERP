<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoodsTypePASS.aspx.cs" Inherits="ERPProject.ERPBasic.GoodsTypePASS" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>申领审批设置</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="MainPanel"
            runat="server" />
        <f:Panel ID="MainPanel" runat="server" AutoScroll="false" BodyPadding="0px" Layout="HBOX" ShowBorder="false" ShowHeader="false">
            <Items>
                <f:Grid ID="GridGoodsType" ShowBorder="false" ShowHeader="false" BoxFlex="1" AnchorValue="100% -230" AutoScroll="true" EnableColumnLines="true"
                    EnableCheckBoxSelect="false" DataKeyNames="CODE" EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridGoodsType_RowDoubleClick"
                    runat="server" OnPageIndexChange="GridGoodsType_PageIndexChange" PageSize="50" IsDatabasePaging="true" AllowPaging="true">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar1" runat="server">
                            <Items>
                                <f:TriggerBox ID="tgbSearch" Label="查询信息" TriggerIcon="Search" Width="450px" runat="server" OnTriggerClick="tgbSearch_TriggerClick"></f:TriggerBox>

                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                <f:ToolbarSeparator runat="server"></f:ToolbarSeparator>
                                <f:Button ID="btnSave" Text="保存" Icon="Disk" runat="server" ValidateForms="FormGoodsType" EnableDefaultState="false" OnClick="btnSave_Click"></f:Button>

                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Columns>
                        <f:BoundField Width="110px" DataField="CODE" HeaderText="商品类别编码" TextAlign="Center" />
                        <f:BoundField Width="150px" DataField="NAME" HeaderText="商品类别名称" />
                        <f:BoundField Width="130px" DataField="STR2NAME" HeaderText="默认库房" />
                        <f:BoundField Width="100px" DataField="STR1" ExpandUnusedSpace="true" HeaderText="申领审批人" />
                        <f:BoundField Width="0px" DataField="STR2" Hidden="true" />
                    </Columns>
                </f:Grid>

                <f:Panel ID="PanelCond" BoxFlex="1" ShowBorder="false" BodyPadding="10px" Height="230" ShowHeader="false" runat="server" CssStyle="border-left: 1px solid #99bce8;" AutoScroll="true">
                    <Items>
                        <f:Form ID="FormGoodsType" ShowBorder="false" BodyPadding="10px" ShowHeader="False" runat="server" LabelWidth="105px">
                            <Rows>
                                <f:FormRow>
                                    <Items>
                                        <f:TextBox ID="tbxCODE" Label="商品类别编码" runat="server" Enabled="false" ShowRedStar="true" Required="true" />
                                        <f:TextBox ID="tbxNAME" Label="商品类别名称" runat="server" Enabled="false" ShowRedStar="true" Required="true" />
                                        <f:DropDownList ID="ddlSTR2" Label="默认库房" LabelWidth="80px" runat="server" EnableEdit="true" ForceSelection="true" ShowRedStar="true" Required="true">
                                            <%--<Listeners>
                                                <f:Listener Event="beforequery" Handler="onDropDownquery" />
                                            </Listeners>--%>
                                        </f:DropDownList>
                                    </Items>
                                </f:FormRow>
                                <f:FormRow>
                                    <Items>
                                        <f:CheckBoxList ID="cblSTR1" runat="server" Label="系   统   用   户" ColumnNumber="15"></f:CheckBoxList>
                                    </Items>
                                </f:FormRow>
                            </Rows>
                        </f:Form>
                    </Items>
                </f:Panel>

            </Items>
        </f:Panel>
    </form>
</body>
</html>
