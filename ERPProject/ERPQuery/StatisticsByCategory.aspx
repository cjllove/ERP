<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StatisticsByCategory.aspx.cs" Inherits="ERPProject.ERPQuery.StatisticsByCategory" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>按商品类别进行统计</title>
    <style type="text/css" media="all">
        .ColBlue {
            font-size: 12px;
            color: blue;
        }

        .x-grid-row-summary .x-grid-cell-inner {
            font-weight: bold;
            color: blue;
        }

        .x-grid-row-summary .x-grid-cell {
            background-color: #fff !important;
        }

        .x-grid-row.highlight td {
            background-color: yellow;
            background-image: none;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="TabStrip1" runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="0" runat="server">
            <Tabs>
                <f:Tab Title="按商品类别汇总" Icon="PageWord" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel4" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBox"
                            ShowHeader="false">
                            <Toolbars>
                                <f:Toolbar ID="Toolbar3" runat="server">
                                    <Items>
                                        <f:ToolbarFill ID="ToolbarFill3" runat="server" />
                                        <%--<f:Button ID="Button3" Icon="DatabaseGo" Text="导 出" OnClick="btExport_Click" ConfirmText="是否导出当前信息?" DisableControlBeforePostBack="false"
                                            EnablePostBack="true" EnableAjax="false" EnableAjaxLoading="true" AjaxLoadingType="Mask" runat="server" />--%>
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btnQuery" Icon="Magnifier" Text="查 询" OnClick="btnQuery_Click" runat="server" />
                                    </Items>
                                </f:Toolbar>
                            </Toolbars>
                            <Items>
                                <f:Form ID="Form2" ShowBorder="false" AutoScroll="false" BodyPadding="5px"
                                    ShowHeader="False" LabelWidth="80px" runat="server">
                                    <Rows>
                                        <f:FormRow ColumnWidths="40% 20% 20% 20%">
                                            <Items>
                                                <f:DropDownList ID="docCategory" runat="server" Label="商品类别" EnableEdit="true" ForceSelection="true" DataSimulateTreeLevelField="LEVELS" EnableSimulateTree="true" />
                                                <f:DropDownList ID="ddlISGZ" runat="server" Label="是否高值">
                                                    <f:ListItem Selected="true" Text="---请选择---" Value="" />
                                                    <f:ListItem Text="高值" Value="Y" />
                                                    <f:ListItem Text="非高值" Value="N" />
                                                </f:DropDownList>
                                                <f:DatePicker ID="dpkRQ1" runat="server" Label="查询期间" Required="true" ShowRedStar="true" />
                                                <f:DatePicker ID="dpkRQ2" runat="server" Label="　　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                                <f:Grid ID="GridCategoryStock" BoxFlex="1" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;" AllowColumnLocking="true"
                                    EnableColumnLines="true" EnableTextSelection="true" AutoScroll="true" runat="server" DataKeyNames="TYPEID" OnRowDataBound="GridCategoryStock_RowDataBound"
                                    EnableSummary="true" SummaryPosition="Bottom" EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridCategoryStock_RowDoubleClick">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" />
                                        <f:BoundField Hidden="true" DataField="CATEGORYID" ColumnID="CATEGORYID" HeaderText="商品类别编码" />
                                        <f:BoundField Width="170px" DataField="CATEGORYNAME" ColumnID="CATEGORYNAME" HeaderText="商品类别" EnableLock="true" Locked="true" />
                                        <f:BoundField Width="100px" DataField="RKSL" ColumnID="RKSL" HeaderText="入库数量" TextAlign="Right" />
                                        <f:BoundField Width="100px" DataField="RKJE" ColumnID="RKJE" HeaderText="入库金额" DataFormatString="{0:F2}" TextAlign="Right" />
                                        <f:BoundField Width="100px" DataField="CKSL" ColumnID="CKSL" HeaderText="出库数量" TextAlign="Right" />
                                        <f:BoundField Width="100px" DataField="CKJE" ColumnID="CKJE" HeaderText="出库金额" DataFormatString="{0:F2}" TextAlign="Right" />
                                        <f:BoundField Width="100px" DataField="SYSL" ColumnID="SYSL" HeaderText="使用数量" TextAlign="Right" />
                                        <f:BoundField Width="100px" DataField="SYJE" ColumnID="SYJE" HeaderText="使用金额" DataFormatString="{0:F2}" TextAlign="Right" />
                                        <f:GroupField HeaderText="库存数量" TextAlign="Center">
                                            <Columns>
                                                <f:BoundField Width="100px" DataField="KSKCSL" ColumnID="KSKCSL" SortField="KSKCSL" HeaderText="科室库存"
                                                    TextAlign="Center" />
                                                <f:BoundField Width="100px" DataField="KSKCJE" ColumnID="KSKCJE" SortField="KSKCJE" HeaderText="科室金额"
                                                    TextAlign="Center" />
                                                <f:BoundField Width="100px" DataField="KFKCSL" ColumnID="KFKCSL" SortField="KFKCSL" HeaderText="库房库存"
                                                    TextAlign="Center" />
                                                <f:BoundField Width="100px" DataField="KFKCJE" ColumnID="KFKCJE" SortField="KFKCJE" HeaderText="库房金额"
                                                    TextAlign="Center" />
                                            </Columns>
                                        </f:GroupField>
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="按科室类别汇总" Icon="PageWord" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="PanelBody" ShowBorder="false" BodyPadding="0px"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar1" runat="server">
                                            <Items>
                                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                <f:Button ID="btnClear" Icon="ArrowRotateClockwise" Text="清 空" OnClick="btnClear_Click" EnablePostBack="true" runat="server" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnPrint" Icon="Printer" Text="打 印" DisableControlBeforePostBack="true" runat="server" OnClientClick="btnPrint_onclick()" />
                                                <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" OnClick="btnExport_Click" ConfirmText="是否导出当前信息?" DisableControlBeforePostBack="false"
                                                    EnablePostBack="true" EnableAjax="false" Hidden="true" EnableAjaxLoading="true" AjaxLoadingType="Mask" runat="server" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnSearch" Icon="Magnifier" Text="查 询" OnClick="btnSearch_Click" runat="server" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="5px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow ColumnWidths="20% 20% 30% 30%">
                                                    <Items>
                                                        <f:DatePicker ID="lstLRRQ1" runat="server" Label="查询期间" Required="true" ShowRedStar="true" />
                                                        <f:DatePicker ID="lstLRRQ2" runat="server" Label="　　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                                                        <f:DropDownList ID="ddlDEPTID" runat="server" Label="库房/科室" EnableEdit="true" ForceSelection="true">
                                                        </f:DropDownList>
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridJXC" AnchorValue="100% -79" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;"
                                    EnableColumnLines="true" EnableTextSelection="true" AutoScroll="true" runat="server" DataKeyNames="DEPTNAME" AllowColumnLocking="true">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" />
                                        <f:BoundField Width="170px" DataField="DEPTNAME" SortField="DEPTNAME" HeaderText="科室" EnableLock="true" Locked="true" />
                                        <%--<f:BoundField Width="100px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                                        <f:BoundField Width="190px" DataField="GDNAME" SortField="GDNAME" ColumnID="GDNAME" HeaderText="商品名称" />
                                        <f:BoundField Width="150px" DataField="GDSPEC" SortField="GDSPEC" HeaderText="规格·容量" />
                                        <f:BoundField Width="50px" DataField="UNIT" SortField="UNIT" HeaderText="单位" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="HSJJ" SortField="HSJJ" HeaderText="含税进价" DataFormatString="{0:F4}" TextAlign="Right" />
                                        <f:BoundField Width="70px" DataField="SL" SortField="SL" ColumnID="SL" HeaderText="数量" TextAlign="Right" />
                                        <f:BoundField Width="100px" DataField="JE" SortField="JE" ColumnID="JE" HeaderText="金额" DataFormatString="{0:F2}" TextAlign="Right" />
                                        <f:BoundField Width="150px" DataField="PRODUCERNAME" SortField="PRODUCERNAME" HeaderText="生产厂家" />--%>
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
        <f:HiddenField ID="highlightRows" runat="server"></f:HiddenField>
    </form>
</body>
</html>
