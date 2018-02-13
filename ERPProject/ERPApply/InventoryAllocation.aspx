<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InventoryAllocation.aspx.cs" Inherits="ERPProject.ERPApply.InventoryAllocation" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>科室库存分配</title>
    <script src="../res/js/CreateControl.js" type="text/javascript"></script>
    <script src="../res/js/GRInstall.js" type="text/javascript"></script>
</head>
<body>
    <div style="width: 0px; height: 0px">
        <script type="text/javascript">
            CreateDisplayViewerEx("0%", "0%", "", "", false, "");
        </script>
    </div>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="TabStrip1" runat="server" OnCustomEvent="PageManager1_CustomEvent" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="0" runat="server">
            <Tabs>
                <f:Tab Title="分配列表" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel2" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="Panel4" ShowBorder="false" BodyPadding="0px"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar1" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="操作信息：双击打开单据明细！" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="btnAuto" Text="自动分配" EnableDefaultState="false" OnClick="btnAuto_Click" runat="server" ConfirmText="是否确认进行自动分配？" EnablePostBack="true" Icon="UserMature"></f:Button>
                                                <f:Button ID="btnHand" Text="手工分配" EnableDefaultState="false" OnClick="btnHand_Click" runat="server" ConfirmText="是否确认进行手工分配？" EnablePostBack="true" Icon="UserMagnify"></f:Button>
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnPrint" Icon="Printer" Text="打印拣货单" EnableDefaultState="false" OnClick="btnPrint_Click" runat="server" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnSearch" Icon="Magnifier" EnableDefaultState="false" Text="查 询" EnablePostBack="true" runat="server" OnClick="btnSearch_Click" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="Form3" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="lstDEPTOUT" runat="server" Label="出库部门" EnableEdit="true" ForceSelection="true" Required="true" ShowRedStar="true" AutoPostBack="true" OnSelectedIndexChanged="lstDEPTOUT_SelectedIndexChanged" />
                                                        <f:TextBox ID="lstBILLNO" runat="server" Label="单据编号" MaxLength="20" EmptyText="输入申领单号或定数单号" />
                                                        <f:DropDownList ID="lstLX" runat="server" Label="配送路线" EnableEdit="true" ForceSelection="true">
                                                        </f:DropDownList>
                                                        <f:DropDownList ID="ddlHOUSE" runat="server" Label="楼　　栋" EnableEdit="true" ForceSelection="true"></f:DropDownList>
                                                        <f:DropDownList ID="ddlFLOOR" runat="server" Label="楼　　层" EnableEdit="true" ForceSelection="true"></f:DropDownList>
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="lstDEPTID" runat="server" Label="申领科室" EnableEdit="true" ForceSelection="true">
                                                        </f:DropDownList>
                                                        <f:TextBox ID="lstGoods" runat="server" Label="商品信息" MaxLength="20" EmptyText="输入商品信息" />
                                                        <f:DropDownList ID="ddlTYPEFP" runat="server" Label="申领类型">
                                                            <f:ListItem Text="--请选择--" Value="" />
                                                            <f:ListItem Text="定数申领" Value="1" />
                                                            <f:ListItem Text="非定申领" Value="0" />
                                                        </f:DropDownList>
                                                        <f:DatePicker ID="dpktime1" runat="server" Label="申请日期" Required="true" ShowRedStar="true" />
                                                        <f:DatePicker ID="dpktime2" runat="server" Label="至" LabelSeparator="" Required="true" ShowRedStar="true" CompareControl="dpktime1" CompareOperator="GreaterThanEqual" CompareType="String" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" AnchorValue="100% -110" ShowBorder="false" ShowHeader="false" EnableCheckBoxSelect="true"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;"
                                    DataKeyNames="SEQNO" EnableTextSelection="true" EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridList_RowDoubleClick"
                                    EnableColumnLines="true" EnableMultiSelect="true" KeepCurrentSelection="true" AllowSorting="true" EnableHeaderMenu="true" SortField="BILLNO" SortDirection="ASC">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField Width="80px" DataField="FPTYPENAME" HeaderText="申请类型" TextAlign="Center" />
                                        <f:BoundField Width="120px" DataField="SEQNO" HeaderText="单据编号" SortField="SEQNO" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="FLAGNAME" HeaderText="单据状态" TextAlign="Center" />
                                        <f:BoundField Width="120px" DataField="DEPTOUTNAME" HeaderText="出库部门" SortField="DEPTOUT" />
                                        <f:BoundField Width="130px" DataField="DEPTIDNAME" HeaderText="申领科室" SortField="DEPTID" />
                                        <f:BoundField Width="60px" DataField="SUBNUM" HeaderText="条目数" TextAlign="Center" />
                                        <f:BoundField Width="60px" DataField="LRYNAME" HeaderText="申领人" TextAlign="Center" />
                                        <f:BoundField Width="60px" DataField="SHRNAME" HeaderText="提交员" TextAlign="Center" SortField="SHR" />
                                        <f:BoundField Width="80px" DataField="SHRQ" HeaderText="提交日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="SHRQ" />
                                        <f:BoundField MinWidth="150px" DataField="STR3" HeaderText="备注" ExpandUnusedSpace="true"/>
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="手工分配" Icon="PageWord" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="PanelMain" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor" ShowHeader="false">
                            <Items>
                                <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" ShowHeader="False" LabelWidth="70px" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar2" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText1" runat="server" Text="操作说明：手工分配界面！" />
                                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                <f:Button ID="btnSure" Icon="UserTick" Text="配给确认" EnableDefaultState="false" ConfirmText="确认按以下方案配给？" OnClick="btnSure_Click" EnablePostBack="true" runat="server" />
                                                <f:Button ID="btnCanl" Icon="UserCross" Text="取消分配" EnableDefaultState="false" ConfirmText="确认取消本次手工分配？" OnClick="btnCanl_Click" EnablePostBack="true" runat="server" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                </f:Form>
                                <f:Panel ID="Panel3" ShowBorder="false" AnchorValue="100% -27" BodyPadding="0px" Layout="HBox" BoxConfigAlign="Stretch" BoxConfigPosition="Start"
                                    ShowHeader="False" runat="server" CssStyle="border-top: 1px solid #99bce8;">
                                    <Items>
                                        <f:Grid ID="GridGoods" ShowBorder="false" ShowHeader="false" AllowSorting="false" CssStyle="border-right: 1px solid #99bce8;" EnableColumnLines="true"
                                            BoxFlex="1" AutoScroll="true" runat="server" DataKeyNames="GDSEQ,KCSL,GDNAME" EnableTextSelection="true" PageSize="50" IsDatabasePaging="true" EnableSummary="false" SummaryPosition="Bottom"
                                            AllowPaging="true" OnPageIndexChange="GridGoods_PageIndexChange" OnRowDataBound="GridGoods_RowDataBound" EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridGoods_RowDoubleClick">
                                            <Columns>
                                                <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                                <f:BoundField Width="100px" DataField="GDSEQ" HeaderText="商品编码" />
                                                <f:BoundField Width="150px" DataField="GDNAME" HeaderText="商品名称" ExpandUnusedSpace="true" />
                                                <f:BoundField Width="120px" DataField="GDSPEC" HeaderText="规格·容量" />
                                                <f:BoundField Width="40px" DataField="UNITNAME" HeaderText="单位" EnableHeaderMenu="false" TextAlign="Center" />
                                                <f:BoundField Width="60px" DataField="KCSL" HeaderText="库存数" TextAlign="Center" />
                                                <f:BoundField Width="60px" DataField="SLSL" ColumnID="SLSL" HeaderText="申领数" TextAlign="Center" />
                                            </Columns>
                                        </f:Grid>
                                        <f:Panel ID="Panel1" ShowBorder="false" BoxFlex="1" BodyPadding="0px" Layout="Anchor" ShowHeader="False" runat="server">
                                            <Items>
                                                <f:Form ID="Form2" ShowBorder="false" AutoScroll="false" BodyPadding="5px 10px 5px 10px" Height="46px"
                                                    ShowHeader="False" LabelWidth="80px" runat="server">
                                                    <Rows>
                                                        <f:FormRow ColumnWidths="60% 20% 20%">
                                                            <Items>
                                                                <f:Label ID="lblMessage" runat="server" EncodeText="false" Text="" />
                                                            </Items>
                                                        </f:FormRow>
                                                    </Rows>
                                                </f:Form>
                                                <f:Grid ID="GridGoodsList" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;" EnableColumnLines="true"
                                                    AutoScroll="true" runat="server" DataKeyNames="SEQNO,ROWNO,FPTYPENAME,XSSL" AllowSorting="false" AnchorValue="100% -52" EnableSummary="false" SummaryPosition="Bottom"
                                                    AllowCellEditing="true" ClicksToEdit="1" EnableTextSelection="true">
                                                    <Columns>
                                                        <f:RowNumberField Width="30px" TextAlign="Center"></f:RowNumberField>
                                                        <f:BoundField DataField="GDSEQ" Hidden="true" />
                                                        <f:BoundField Width="80px" DataField="DEPTIDNAME" HeaderText="申领科室" EnableHeaderMenu="false" />
                                                        <f:BoundField Width="100px" DataField="GDSEQ" HeaderText="商品编码" EnableHeaderMenu="false" Hidden="true" TextAlign="Center" />
                                                        <f:BoundField Width="120px" DataField="GDNAME" HeaderText="商品名称" EnableHeaderMenu="false" Hidden="true" />
                                                        <f:BoundField Width="70px" DataField="GDSPEC" HeaderText="商品规格" EnableHeaderMenu="false" />
                                                        <f:BoundField Width="60px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                                                        <f:RenderField Width="70px" DataField="XSSL" HeaderText="申领数" EnableHeaderMenu="false" FieldType="Auto" TextAlign="Center" />
                                                        <f:RenderField Width="80px" ColumnID="FPSL" DataField="FPSL" FieldType="Auto" EnableHeaderMenu="false" TextAlign="Center" HeaderText="分配数量<font color='red'>*</font>">
                                                            <Editor>
                                                                <f:NumberBox ID="nbxFPSL" runat="server" MinValue="0" DecimalPrecision="2" MaxLength="10" />
                                                            </Editor>
                                                        </f:RenderField>
                                                        <f:BoundField Width="80px" DataField="FPTYPENAME" HeaderText="申领类型" EnableHeaderMenu="false" TextAlign="Center" />
                                                        <f:BoundField Width="110px" DataField="SEQNO" HeaderText="单据编号" EnableHeaderMenu="false" TextAlign="Center" />
                                                        <f:BoundField Width="60px" DataField="ROWNO" HeaderText="行号" EnableHeaderMenu="false" TextAlign="Center" />
                                                        <f:BoundField DataField="FPTYPE" Hidden="true" EnableHeaderMenu="false" />
                                                    </Columns>
                                                </f:Grid>
                                            </Items>
                                        </f:Panel>
                                    </Items>
                                </f:Panel>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
        <f:HiddenField ID="hfdBCode" runat="server" />
        <f:HiddenField ID="hfdValue" runat="server" />
        <f:Window ID="Window1" Title="出库单据信息" Hidden="true" EnableIFrame="true" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Width="820px" Height="480px">
        </f:Window>
    </form>
    <script type="text/javascript">
        function btnPrint_onclick(billno) {
            ReportViewer.ReportURL = "/grf/Jhd_InventoryAllocation.grf?time=" + new Date().getTime();
            var dataurl = "/captcha/PrintReport.aspx?Method=InventoryAllocation&osid=" + billno.toString();
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
    </script>
</body>
</html>
