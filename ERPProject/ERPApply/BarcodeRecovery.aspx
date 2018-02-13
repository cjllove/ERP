<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BarcodeRecovery.aspx.cs" Inherits="ERPProject.ERPApply.BarcodeRecovery" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>条码回收管理</title>
    <script src="/res/js/CreateControl.js" type="text/javascript"></script>
    <script src="/res/js/GRInstall.js" type="text/javascript"></script>
</head>
<body>
    <div style="width: 0px; height: 0px">
        <script type="text/javascript">
            CreateDisplayViewerEx("0%", "0%", "", "", false, "");
        </script>
    </div>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="TabStrip1"
            runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="1" runat="server">
            <Tabs>
                <f:Tab Title="单据列表" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel2" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBox"
                            ShowHeader="false">
                            <Toolbars>
                                <f:Toolbar ID="Toolbar2" runat="server">
                                    <Items>
                                        <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="操作信息：双击打开单据明细！" runat="server" />
                                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btnCreate" Icon="ArrowJoin" Text="生成出库单" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnCreate_Click" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="bntClear" Icon="Erase" Text="清 除" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />

                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                    </Items>
                                </f:Toolbar>
                            </Toolbars>
                            <Items>
                                <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                    ShowHeader="False" LabelWidth="80px" runat="server">
                                    <Rows>
                                        <f:FormRow>
                                            <Items>
                                                <f:TextBox ID="lstBILLNO" runat="server" Label="单据编号" MaxLength="15" />
                                                <f:DropDownList ID="lstDEPTID" runat="server" Label="回收科室" EnableEdit="true" ForceSelection="true" />
                                                <f:DropDownList ID="lstFLAG" runat="server" Label="单据状态" EnableEdit="true" ForceSelection="true" />
                                                <f:DropDownList ID="lstSLR" runat="server" Label="回收人" EnableEdit="true" ForceSelection="true" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:DropDownList ID="ddlTYPE" runat="server" Label="是否生成" ForceSelection="true" Hidden="true">
                                                    <f:ListItem Selected="true" Text="---请选择---" Value="" />
                                                    <f:ListItem Text="未生成" Value="N" />
                                                    <f:ListItem Text="已生成" Value="Y" />
                                                </f:DropDownList>
                                                <f:DropDownList ID="lstDEPTOUT" runat="server" Label="回收部门" EnableEdit="true" ForceSelection="true" />
                                                <f:DatePicker ID="lstLRRQ1" runat="server" Label="录入日期" Required="true" ShowRedStar="true" />
                                                <f:DatePicker ID="lstLRRQ2" runat="server" Label="　　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                                <f:Grid ID="GridList" BoxFlex="1" ShowBorder="false" ShowHeader="false"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableTextSelection="true"
                                    DataKeyNames="SEQNO,FLAG" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick"
                                    OnRowDataBound="GridList_RowDataBound" EnableColumnLines="true" EnableCheckBoxSelect="true" EnableMultiSelect="true"
                                    AllowSorting="true" EnableHeaderMenu="true" SortField="BILLNO" OnSort="GridList_Sort">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField DataField="SEQNO" Hidden="true" SortField="SEQNO" />
                                        <f:BoundField Width="110px" DataField="BILLNO" SortField="BILLNO" HeaderText="单据编号" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="FLAG" ColumnID="FLAG" SortField="FLAG" HeaderText="单据状态" TextAlign="Center" />
                                        <f:BoundField Width="150px" DataField="DEPTOUT" SortField="DEPTOUT" HeaderText="回收部门" />
                                        <f:BoundField Width="150px" DataField="DEPTID" SortField="DEPTID" HeaderText="回收科室" />
                                        <f:BoundField Width="90px" DataField="XSRQ" SortField="XSRQ" HeaderText="申领日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="90px" DataField="CATID" SortField="CATID" HeaderText="商品种类" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="70px" DataField="SUBSUM" SortField="SUBSUM" HeaderText="金额" TextAlign="Right" />
                                        <f:BoundField Width="90px" DataField="SUBNUM" SortField="SUBNUM" HeaderText="明细条数" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="SLR" SortField="SLR" HeaderText="回收人" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="LRY" SortField="LRY" HeaderText="录入员" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="LRRQ" SortField="LRRQ" HeaderText="录入日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="70px" DataField="SHR" SortField="SHR" HeaderText="审核员" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="SHRQ" SortField="SHRQ" HeaderText="审核日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="100px" DataField="MEMO" SortField="MEMO" HeaderText="备注" TextAlign="Center" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="单据信息" Icon="PageWord" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBox"
                            ShowHeader="false">
                            <Toolbars>
                                <f:Toolbar ID="Toolbar1" runat="server">
                                    <Items>
                                        <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="操作信息：请首先维护单据头!" runat="server" />
                                        <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                        <%-- <f:Button ID="btnGoods" Icon="Magnifier" Text="商 品" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />--%>
                                        <f:Button ID="btnNew" Icon="PageAdd" Text="新 单" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                        <f:Button ID="btnDel" Icon="PageCancel" Text="整单删除" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确定删除此单据?" Enabled="false" runat="server" OnClick="btnBill_Click" />
                                        <f:Button ID="btnSave" Icon="Disk" Text="保 存" EnableDefaultState="false" EnablePostBack="true" runat="server" ValidateForms="FormDoc" OnClick="btnBill_Click" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btnAddRow" Icon="Add" Text="增 行" EnableDefaultState="false" runat="server" OnClick="btnBill_Click" />
                                        <f:Button ID="btnDelRow" Icon="Delete" Text="删 行" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否确定删除选中行?" OnClick="btnBill_Click" />
                                        <f:Button ID="btnNext" Icon="ForwardBlue" Text="下 页" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                        <f:Button ID="btnBef" Icon="RewindBlue" Text="上 页" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btnAudit" Icon="UserTick" Text="审 核" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否确认保存并审核此单据?" OnClick="btnBill_Click" />
                                        <f:Button ID="btnPrint" Icon="Printer" Text="打 印" EnableDefaultState="false" EnablePostBack="false" runat="server" OnClientClick="btnPrint_Bill()" />
                                        <f:Button ID="btnCopy" Icon="PageCopy" Text="复 制" EnableDefaultState="false" ConfirmText="是否确定复制单据信息?" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                        <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认导出此单据信息?" runat="server" OnClick="btnBill_Click" />
                                    </Items>
                                </f:Toolbar>
                            </Toolbars>
                            <Items>
                                <f:Form ID="FormDoc" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                    ShowHeader="False" LabelWidth="80px" runat="server">
                                    <Rows>
                                        <f:FormRow Hidden="true">
                                            <Items>
                                                <f:TextBox ID="docSEQNO" runat="server" Hidden="true" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:DropDownList ID="docDEPTOUT" runat="server" Label="回收部门" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                                                <f:DropDownList ID="docSLR" runat="server" Label="回收人" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                                                <f:TextBox ID="docBILLNO" runat="server" Label="单据编号" Enabled="false" />
                                                <f:DropDownList ID="docFLAG" runat="server" Label="单据状态" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:DropDownList ID="docDEPTID" runat="server" Label="回收科室" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                                                <f:DatePicker ID="docXSRQ" runat="server" Label="回收日期" />
                                                <f:DropDownList ID="docLRY" runat="server" Label="录入员" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                <f:DatePicker ID="docLRRQ" runat="server" Label="录入日期" Enabled="false" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:TriggerBox ID="trbBARCODE" Label="扫描条码" runat="server" EmptyText="输入或扫描条码" ShowRedStar="true" OnTriggerClick="trbBARCODE_TriggerClick"></f:TriggerBox>
                                                <f:TextBox ID="docMEMO" runat="server" Label="备注说明" MaxLength="80" />
                                                <f:DropDownList ID="docSHR" runat="server" Label="审核人" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                <f:DatePicker ID="docSHRQ" runat="server" Label="审核日期" Enabled="false" />
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                                <f:Grid ID="GridGoods" BoxFlex="1" ShowBorder="false" ShowHeader="false" EnableSummary="true" SummaryPosition="Bottom" EnableTextSelection="true"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableColumnLines="true"  EnableRowSelectEvent="true" EnableCheckBoxSelect="false"  
                                    DataKeyNames="GDSEQ,Str1,BarCode" AllowCellEditing="true" ClicksToEdit="1" EnableAfterEditEvent="false" OnAfterEdit="GridGoods_AfterEdit">
                                    <Columns>
                                        <f:RenderField Width="35px" EnableLock="true" Locked="true" TextAlign="Center" ColumnID="ROWNO" DataField="ROWNO" FieldType="String" EnableHeaderMenu="false">
                                            <Editor>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" ColumnID="STR1" DataField="Str1" FieldType="String"
                                            HeaderText="定数条码" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblstr1" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="GDSEQ" DataField="GDSEQ" FieldType="String"
                                            HeaderText="商品编码" EnableHeaderMenu="false" TextAlign="Center">
                                            <Editor>
                                                <%--<f:TriggerBox ID="trbEditorGDSEQ" Required="true" runat="server" OnTriggerClick="tbxINSERT_TextChanged" TriggerIcon="Search" Enabled="false"></f:TriggerBox>--%>
                                                <f:Label ID="lblEditorGDSEQ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="BarCode" DataField="BarCode" FieldType="String"
                                            HeaderText="商品条码" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorBARCODE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" ColumnID="GDNAME" DataField="GDNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="商品名称" EnableLock="true" Locked="true">
                                            <Editor>
                                                <f:Label ID="lblEditorName" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="150px" ColumnID="GDSPEC" DataField="GDSPEC" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="商品规格">
                                            <Editor>
                                                <f:Label ID="lblEditorGDSPEC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="UNIT" DataField="UNIT" FieldType="String"
                                            HeaderText="包装单位编码" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorUNIT" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="UNITNAME" DataField="UNITNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="包装单位" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorUNITNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="BZHL" DataField="BZHL" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="定数含量" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorBZHL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="DHSL" DataField="DHSL" FieldType="Int" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="申领数量" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorDHSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="BZSL" DataField="BZSL" FieldType="Float" EnableHeaderMenu="false"
                                            HeaderText="回收定数" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblBZSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="60px" ColumnID="JXTAX" DataField="JXTAX" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="税率" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorJXTAX" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="HSJJ" DataField="HSJJ" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="含税进价" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblHSJJ" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="HSJE" DataField="HSJE" FieldType="Float" EnableHeaderMenu="false"
                                            HeaderText="含税金额" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorHSJE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="ZPBH" DataField="ZPBH" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="制品编号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorZPBH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="PRODUCER" DataField="PRODUCER" FieldType="String"
                                            HeaderText="生产厂家编码" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorPRODUCER" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" ColumnID="PRODUCERNAME" DataField="PRODUCERNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="生产厂家">
                                            <Editor>
                                                <f:Label ID="lblEditorPRODUCERNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="HWID" DataField="HWID" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="货位" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorHWID" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="PH" DataField="PH" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="批号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblPH" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="110px" ColumnID="YXQZ" DataField="YXQZ" EnableHeaderMenu="false" EnableColumnHide="false" FieldType="Date"
                                            HeaderText="有效期至" TextAlign="Center" Renderer="Date" RendererArgument="yyyy-MM-dd">
                                            <Editor>
                                                <f:DatePicker ID="dpkEditorYXQZ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="RQ_SC" DataField="RQ_SC" EnableHeaderMenu="false" FieldType="Date"
                                            HeaderText="生产日期" TextAlign="Center" Renderer="Date" RendererArgument="yyyy-MM-dd">
                                            <Editor>
                                                <f:Label ID="lblEditorRQ_SC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="150px" ColumnID="PZWH" DataField="PZWH" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="注册证号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorPZWH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="MEMO" DataField="MEMO" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="备注" TextAlign="Center">
                                            <Editor>
                                                <f:TextBox ID="tbxEditorMEMO" runat="server" MaxLength="80" />
                                            </Editor>
                                        </f:RenderField>
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
        <f:HiddenField ID="hfdValue" runat="server" />
        <f:HiddenField ID="highlightRows" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdROWID" runat="server"></f:HiddenField>
        <f:Window ID="Window1" Title="商品信息" Hidden="true" EnableIFrame="true" runat="server"
            EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True"
            Width="820px" Height="480px" OnClose="Window1_Close">
        </f:Window>
    </form>
    <script type="text/javascript">
        function btnPrint_Bill() {
            var billNo = F('<%= docBILLNO.ClientID%>').getValue();
            var billState = F('<%= docFLAG.ClientID%>').getValue();
            if (billNo == "") {
                F.alert("请选择要打印的单据信息！");
                return;
            }
            if (billState != "Y") {
                F.alert("选择单据未审核,不允许打印！");
                return;
            }
            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "/grf/kssyckd.grf?timestamp=" + new Date().getTime();
            var dataurl = "/captcha/PrintReport.aspx?Method=GetXSDataBill&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
    </script>
</body>
</html>
