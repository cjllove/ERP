<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddGoodsNew.aspx.cs" Inherits="ERPProject.ERPApply.AddGoodsNew" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>商品新增管理</title>
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
        <f:PageManager ID="PageManager1" AutoSizePanelID="TabStrip1" runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="1"
            runat="server">
            <Tabs>
                <f:Tab Title="单据列表" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel2" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="Panel3" ShowBorder="false" BodyPadding="0px" Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar2" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="操作信息：双击打开单据明细！" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                <f:Button ID="bntClear" Icon="Erase" Text="清 除" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px" ColumnWidth="45% 25% 10% 10% 10%"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TextBox ID="lstBILLNO" runat="server" Label="单据编号" MaxLength="20" EmptyText="输入单据信息" />
                                                        <f:DropDownList ID="lstDEPTID" runat="server" Label="使用科室" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="lstFLAG" runat="server" Label="状态" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="lstLRRQ1" runat="server" Label="使用日期" Required="true" ShowRedStar="true" />
                                                        <f:DatePicker ID="lstLRRQ2" runat="server" Label="　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" AnchorValue="100% -70" ShowBorder="false" ShowHeader="false" EnableTextSelection="true"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" OnRowDataBound="GridList_RowDataBound"
                                    DataKeyNames="SEQNO" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick" EnableColumnLines="true"
                                    AllowSorting="true" EnableHeaderMenu="true" SortField="BILLNO" OnSort="GridList_Sort">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField DataField="SEQNO" Hidden="true" SortField="SEQNO" />
                                        <f:BoundField Width="120px" DataField="BILLNO" HeaderText="单据编号" SortField="BILLNO" TextAlign="Center" />
                                        <f:BoundField Width="0px" Hidden="true" DataField="FLAG" HeaderText="单据状态" SortField="FLAG" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="FLAGNAME" HeaderText="单据状态" SortField="FLAG" TextAlign="Center" />
                                        <f:BoundField Width="150px" DataField="DEPTID" HeaderText="使用科室" SortField="DEPTID" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="SUBNUM" HeaderText="明细条数" SortField="SUBNUM" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="SUBSUM" HeaderText="合计金额" SortField="SUBSUM" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="LRY" HeaderText="申领人" SortField="LRY" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="LRRQ" HeaderText="申领日期" SortField="LRRQ" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="80px" DataField="LRYTEL" HeaderText="申领人电话" SortField="LRYTEL" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="SPR" HeaderText="提案人" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="SPRQ" HeaderText="提案日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="70px" DataField="SHR" HeaderText="审核员" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="SHRQ" HeaderText="审核日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="170px" DataField="MEMO" HeaderText="备注" SortField="MEMO" TextAlign="Center" ExpandUnusedSpace="true" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="单据信息" Icon="PageWord" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="PanelBody" ShowBorder="false" BodyPadding="0px"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar1" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="操作信息：商品新增操作界面！" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="btnNew" Icon="PageAdd" Text="新 单" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnDel" Icon="PageCancel" Text="删 除" EnablePostBack="true" ConfirmText="是否删除此单据?" runat="server" OnClick="btnBill_Click" Hidden="true" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnAddRow" Icon="Add" Text="增 行" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" />
                                                <f:Button ID="btnDelRow" Icon="Delete" Text="删 行" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否删除选中行信息?" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnSave" Icon="Disk" Text="保 存" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnSaveFa" Icon="Disk" Text="保 存" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnSaveFa_Click" ValidateForms="FormDoc" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnTJ" Icon="UserTick" Text="需求提交" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnTJ_Click" ValidateForms="FormDoc" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnTJFA" Icon="UserTick" Text="方案提交" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnTJFA_Click" ValidateForms="FormDoc" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnDelete" Icon="UserCross" Text="废 弃" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnDelete_Click" Hidden="true" />
                                                <f:Button ID="btnAudit" Icon="UserTick" Text="审 批" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认审核此单据?" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" />
                                                <f:Button ID="btnCancel" Icon="UserCross" Text="驳 回" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnCancel_Click" />
                                                <f:Button ID="btnPrint" Icon="Printer" Text="打 印" EnableDefaultState="false" EnablePostBack="false" runat="server" OnClientClick="btnPrint_Bill()" />
                                                <f:Button ID="btnCopy" Icon="PageCopy" Text="复 制" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否确认复制此单据信息?" OnClick="btnBill_Click" />
                                                <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否确认导出此单据信息?" OnClick="btnBill_Click" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="FormDoc" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow Hidden="true">
                                                    <Items>
                                                        <f:TextBox ID="tbxSEQNO" runat="server" Hidden="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="ddlDEPTID" runat="server" Label="申领科室" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="docLRY" runat="server" Label="申领人" Enabled="false" ShowRedStar="true" Required="true" />
                                                        <f:TextBox ID="tbxBILLNO" runat="server" Label="单据编号" EmptyText="自动生成" MaxLength="20" />
                                                        <f:DropDownList ID="ddlFLAG" runat="server" Label="单据状态" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TextBox ID="tbxLRYTEL" runat="server" Label="申领电话" RegexPattern="NUMBER" MaxLength="11" ShowRedStar="true" Required="true"></f:TextBox>
                                                        <f:DatePicker ID="dpkLRRQ" runat="server" Label="申领日期" Enabled="false" />
                                                        <f:DropDownList ID="ddlSPR" runat="server" Label="提案人" Enabled="false" />
                                                        <f:DropDownList ID="ddlSHR" runat="server" Label="审核人" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow ColumnWidths="50% 25% 25%">
                                                    <Items>
                                                        <f:TextBox ID="tbxMEMO" runat="server" Label="申领事由" />
                                                        <f:DatePicker ID="dpkSPRQ" runat="server" Label="提案日期" Enabled="false" />
                                                        <f:DatePicker ID="dpkSHRQ" runat="server" Label="审核日期" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridGoods" AnchorValue="100% -143" ShowBorder="false" ShowHeader="false" EnableTextSelection="true"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;"
                                    DataKeyNames="SEQNO,ROWNO" AllowCellEditing="true" ClicksToEdit="1" EnableAfterEditEvent="true" OnAfterEdit="GridGoods_AfterEdit" EnableRowLines="true">
                                    <Columns>
                                        <f:RowNumberField Width="50px" TextAlign="Center"></f:RowNumberField>
                                        <f:GroupField HeaderText="商品申请" TextAlign="Center">
                                            <Columns>
                                                <f:RenderField Width="220px" ColumnID="MEMOGOODS" DataField="MEMOGOODS" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                                    HeaderText="需求说明" TextAlign="Center">
                                                    <Editor>
                                                        <f:TextBox ID="lblMEMOGOODS" runat="server" />
                                                    </Editor>
                                                </f:RenderField>
                                                <f:RenderField Width="60px" ColumnID="SL" DataField="SL" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                                    HeaderText="数量" TextAlign="Center">
                                                    <Editor>
                                                        <f:NumberBox ID="lblSL" runat="server" MinValue="0" DecimalPrecision="6" MaxValue="99999999" />
                                                    </Editor>
                                                </f:RenderField>
                                            </Columns>
                                        </f:GroupField>
                                        <f:GroupField HeaderText="采购方案" TextAlign="Center">
                                            <Columns>
                                                <f:RenderField Width="100px" ColumnID="ISNEW" DataField="ISNEW" FieldType="String" RendererFunction="RenderISNEW"
                                                    HeaderText="是否新增" TextAlign="Center">
                                                    <Editor>
                                                        <f:DropDownList ID="comISNEW" runat="server">
                                                            <f:ListItem Text="是" Value="Y" />
                                                            <f:ListItem Text="否" Value="N" />
                                                        </f:DropDownList>
                                                    </Editor>
                                                </f:RenderField>
                                                <f:RenderField Width="180px" ColumnID="GDNAME" DataField="GDNAME" FieldType="String"
                                                    HeaderText="商品名称" EnableLock="true" Locked="true">
                                                    <Editor>
                                                        <f:TextBox ID="lblGDNAME" runat="server" />
                                                    </Editor>
                                                </f:RenderField>
                                                <f:RenderField Width="80px" ColumnID="GDSPEC" DataField="GDSPEC" FieldType="String"
                                                    HeaderText="商品规格">
                                                    <Editor>
                                                        <f:TextBox ID="lblGDSPEC" runat="server" />
                                                    </Editor>
                                                </f:RenderField>
                                                <f:RenderField Width="80px" ColumnID="UNIT" DataField="UNIT" FieldType="String"
                                                    HeaderText="包装单位" TextAlign="Center">
                                                    <Editor>
                                                        <f:TextBox ID="lblUNIT" runat="server" />
                                                    </Editor>
                                                </f:RenderField>
                                                <f:RenderField Width="180px" ColumnID="PRODUCER" DataField="PRODUCER" FieldType="String"
                                                    HeaderText="生产厂家" TextAlign="Center">
                                                    <Editor>
                                                        <f:TextBox ID="lblPRODUCER" runat="server" />
                                                    </Editor>
                                                </f:RenderField>
                                                <f:RenderField Width="80px" DataField="HSJJ" ColumnID="HSJJ" HeaderText="含税进价" EnableColumnHide="false" EnableHeaderMenu="false" TextAlign="Center">
                                                    <Editor>
                                                        <f:NumberBox ID="lblHSJJ" runat="server" MinValue="0" DecimalPrecision="6" MaxValue="99999999" />
                                                    </Editor>
                                                </f:RenderField>
                                                <f:RenderField Width="80px" DataField="HSJE" ColumnID="HSJE" HeaderText="合计金额" EnableColumnHide="false" EnableHeaderMenu="false" TextAlign="Center">
                                                    <Editor>
                                                        <f:Label ID="lblHSJE" runat="server" />
                                                    </Editor>
                                                </f:RenderField>
                                                <f:RenderField Width="0px" ColumnID="MEMOBUY" Hidden="true" DataField="MEMOBUY" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                                    HeaderText="采购方案" TextAlign="Center">
                                                    <Editor>
                                                        <f:TextBox ID="lblMEMOBUY" runat="server" />
                                                    </Editor>
                                                </f:RenderField>
                                            </Columns>
                                        </f:GroupField>
                                        <f:GroupField HeaderText="领导审批" TextAlign="Center">
                                            <Columns>
                                                <f:RenderField Width="100px" ColumnID="ISPASS" DataField="ISPASS" RendererFunction="RenderISPASS"
                                                    HeaderText="是否同意" TextAlign="Center" Renderer="Date" RendererArgument="yyyy-MM-dd">
                                                    <Editor>
                                                        <f:DropDownList ID="comISPASS" runat="server">
                                                            <f:ListItem Text="同意提案" Value="Y" />
                                                            <f:ListItem Text="方案修改" Value="R" />
                                                            <f:ListItem Text="不通过" Value="N" />
                                                        </f:DropDownList>
                                                    </Editor>
                                                </f:RenderField>

                                                <f:RenderField Width="100px" ColumnID="MEMOPASS" DataField="MEMOPASS" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                                    HeaderText="审批意见" TextAlign="Center">
                                                    <Editor>
                                                        <f:TextBox ID="lblMEMOPASS" runat="server" />
                                                    </Editor>
                                                </f:RenderField>
                                            </Columns>
                                        </f:GroupField>
                                        <%--<f:RenderField Width="0px" ColumnID="SEQNO" DataField="SEQNO" FieldType="String"
                                            HeaderText="SEQNO" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblSEQNO" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="ROWNO" DataField="ROWNO" FieldType="String"
                                            HeaderText="ROWNO" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblROWNO" runat="server" />
                                            </Editor>
                                        </f:RenderField>--%>
                                    </Columns>
                                    <Listeners>
                                        <f:Listener Event="beforeedit" Handler="onGridBeforeEdit" />
                                    </Listeners>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
        <f:HiddenField ID="hfdValue" runat="server" />
        <f:HiddenField ID="hfdDgAudit" runat="server" />
        <f:HiddenField ID="hfdOper" runat="server" />
        <f:HiddenField ID="hfdISTJFA" Text="N" runat="server" />
        <f:HiddenField ID="highlightRowGreen" runat="server"></f:HiddenField>
        <f:HiddenField ID="highlightRowYellow" runat="server"></f:HiddenField>
        <f:HiddenField ID="highlightRowRed" runat="server"></f:HiddenField>
        <f:HiddenField ID="highlightRowBlue" runat="server"></f:HiddenField>
        <f:Window ID="Window1" Title="商品信息" Hidden="true" EnableIFrame="true" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True"
            Width="820px" Height="480px" OnClose="Window1_Close">
        </f:Window>
    </form>
    <script type="text/javascript">
        //grid 下单菜单绑定值
        var comISNEW = '<%=comISNEW.ClientID%>'
        var comISPASS = '<%=comISPASS.ClientID%>'
        function RenderISNEW(value) {
            return F(comISNEW).getTextByValue(value);
        }
        function RenderISPASS(value) {
            return F(comISPASS).getTextByValue(value);
            //var result = '';
            //var arrayItems = F(comISPASS).getStore().data.items;
            //for (var i = 0; i < arrayItems.length; i++) {
            //    if (value == arrayItems[i].data.value) {
            //        result = arrayItems[i].data.text;
            //        break;
            //    }
            //}
            //return result;
        }
        //Gird中可编辑的表格
        function onGridBeforeEdit(event, value, params) {
            var ddlFLAG = F('<%= ddlFLAG.ClientID%>').getValue();
            var hfdOper = F('<%= hfdOper.ClientID%>').getValue();
            if ((",M").indexOf(ddlFLAG) > 0 && hfdOper == "sq") {
                return true;
            } else if ((",N,R").indexOf(ddlFLAG) > 0 && hfdOper == "fa") {
                return true;
            } else if (ddlFLAG == "S" && hfdOper == "sp") {
                return true;
            }
            else
                return false;
        }
        function btnPrint_Bill() {
            var billNo = F('<%= tbxBILLNO.ClientID%>').getValue();
            var billState = F('<%= ddlFLAG.ClientID%>').getValue();
            if (billNo == "") {
                F.alert("请选择要打印的单据信息！");
                return;
            }
            if (billState != "Y") {
                F.alert("选择单据未审核,不允许打印！");
                return;
            }
            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "/grf/GoodsNew.grf?201512230934";
            var dataurl = "/captcha/PrintReport.aspx?Method=GetGoodsNew&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
    </script>
</body>
</html>
