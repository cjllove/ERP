<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SupFx.aspx.cs" Inherits="ERPProject.ERPQuery.SupFx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>供应商供货分析</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="TabStrip1" runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="0" runat="server">
            <Tabs>
                <f:Tab Title="供应商排行" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel1" runat="server" ShowBorder="False" BodyPadding="0px" Layout="Anchor"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="PanelBody" ShowBorder="false" BodyPadding="0px"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar1" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText" runat="server" Text="操作信息：到货情况分析查询主界面！"></f:ToolbarText>
                                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                <f:Button ID="btnAudit" Icon="ArrowRotateClockwise" Text="清 空" OnClick="btClear_Click" EnablePostBack="true" runat="server" EnableDefaultState="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnExp" runat="server" CssStyle="margin-left: 10px;" Icon="DatabaseGo" EnableAjax="false" EnableDefaultState="false"
                                                    OnClick="btExport_Click" EnablePostBack="true" Text="导出" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" OnClick="btSearch_Click" runat="server" EnableDefaultState="false" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="10px 0px 10px 10px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="lstSUPTID" runat="server" Label="供应商" EnableEdit="true" ForceSelection="true">
                                                        </f:DropDownList>
                                                        <f:DatePicker ID="dpkDATE1" runat="server" Label="查询期间" Required="true" ShowRedStar="true"></f:DatePicker>
                                                        <f:DatePicker ID="dpkDATE2" runat="server" Label=" 至" LabelSeparator="" Required="true" ShowRedStar="true" LabelWidth="40px"></f:DatePicker>
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridGoods" AnchorValue="100% -84" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;"
                                    EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridGoods_RowDoubleClick" OnSort="GridGoods_Sort" SortDirection="DESC" SortField="SLZB"
                                    AllowSorting="true" AutoScroll="true" runat="server" DataKeyNames="SUPID" EnableColumnLines="true">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField Width="120px" DataField="SUPID" SortField="SUPID" HeaderText="供应商编码" TextAlign="Center" />
                                        <f:BoundField Width="220px" DataField="SUPNAME" SortField="SUPNAME" HeaderText="供应商名称" />
                                        <f:BoundField Width="130px" DataField="XSSL" HeaderText="使用数量" TextAlign="Right" />
                                        <f:BoundField Width="130px" DataField="XSJE" HeaderText="使用金额" TextAlign="Right" />
                                        <f:BoundField Width="120px" DataField="SLZB" SortField="SLZB" HeaderText="数量占比" DataFormatString="{0:p2}" TextAlign="Center" />
                                        <f:BoundField Width="120px" DataField="JEZB" SortField="JEZB" HeaderText="金额占比" DataFormatString="{0:p2}" TextAlign="Center" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="供应商明细排行" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel2" runat="server" ShowBorder="False" BodyPadding="0px" Layout="Anchor"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="Panel3" ShowBorder="false" BodyPadding="0px"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar2" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="操作信息：按订货日期倒序排列！" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="btnCl" Icon="ArrowRotateClockwise" Text="清 空" OnClick="btClear_Click" EnablePostBack="true" runat="server" EnableDefaultState="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnExpt" runat="server" CssStyle="margin-left: 10px;" Icon="DatabaseGo" EnableAjax="false" EnableDefaultState="false"
                                                    OnClick="btExport_Click" EnablePostBack="true" Text="导出" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnSch" Icon="Magnifier" Text="查 询" OnClick="btnSch_Click" runat="server" EnableDefaultState="false" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="FormList" ShowBorder="false" AutoScroll="false" BodyPadding="10px 0px 10px 10px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="ddlSUPID" runat="server" Label="供应商" EnableEdit="true" ForceSelection="true">
                                                        </f:DropDownList>
                                                        <f:TextBox ID="lisGDSEQ" runat="server" Label="商品信息" EmptyText="请输入商品信息" MaxLength="20" />
                                                        <f:DatePicker ID="lisDATE1" runat="server" Label="查询期间" Required="true" ShowRedStar="true"></f:DatePicker>
                                                        <f:DatePicker ID="lisDATE2" runat="server" Label="　至" LabelSeparator="" Required="true" ShowRedStar="true" LabelWidth="40px"></f:DatePicker>
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" AnchorValue="100% -82" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;"
                                    AllowPaging="true" IsDatabasePaging="true" SortField="SLZB" SortDirection="DESC" PageSize="50" OnSort="GridList_Sort" OnPageIndexChange="GridGoods_PageIndexChange"
                                    AllowSorting="true" AutoScroll="true" runat="server" DataKeyNames="GDSEQ" EnableColumnLines="true">
                                    <Columns>
                                        <f:RowNumberField Width="40px"></f:RowNumberField>
                                        <f:BoundField Width="110px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" />
                                        <f:BoundField Width="220px" DataField="GDNAME" SortField="GDNAME" HeaderText="商品名称" />
                                        <f:BoundField Width="110px" DataField="GDSPEC" HeaderText="规格·容量" />
                                        <f:BoundField Width="50px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="XSSL" SortField="XSSL" HeaderText="使用数" TextAlign="Right" />
                                        <f:BoundField Width="110px" DataField="XSJE" SortField="XSJE" HeaderText="使用金额" TextAlign="Right" />
                                        <f:BoundField Width="90px" DataField="SLZB" SortField="SLZB" HeaderText="数量占比" TextAlign="Right" DataFormatString="{0:p}" />
                                        <f:BoundField Width="100px" DataField="JEZB" SortField="JEZB" HeaderText="金额占比" TextAlign="Right" DataFormatString="{0:p}" />
                                        <f:BoundField Width="180px" DataField="PRODUCERNAME" HeaderText="生产厂家" />
                                        <f:BoundField Width="180px" DataField="PIZNO" HeaderText="批准文号" />
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
