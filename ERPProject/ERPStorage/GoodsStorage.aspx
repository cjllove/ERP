<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoodsStorage.aspx.cs" Inherits="ERPProject.ERPStorage.GoodsStorage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>商品入库管理</title>
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
        <f:PageManager ID="PageManager1" AutoSizePanelID="TabStrip1" EnableAjaxLoading="false" AjaxLoadingType="Mask" runat="server" OnCustomEvent="PageManager1_CustomEvent" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right" EnableTabCloseMenu="false" ActiveTabIndex="1" runat="server">
            <Tabs>
                <f:Tab Title="单据列表" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel2" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="Panel3" ShowBorder="false" BodyPadding="0px"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar2" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="操作信息：双击打开单据明细！" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />

                                                <f:Button ID="btnEvaluate" Icon="AwardStarBronze2" Text="供应商评价" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnEvaluate_Click" />
                                                <f:ToolbarSeparator runat="server" ID="Line1" />
                                                <f:Button ID="bntClear" Icon="Erase" Text="清 除" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <%-- 导入，模板下载 --%>
                                                <f:ToolbarSeparator runat="server" />
                                                <f:LinkButton ID="LinkButton1" Text="下载模板" EnablePostBack="false" OnClientClick="DownLoadModelclick();" runat="server" />
                                                <f:FileUpload runat="server" ID="fuDocument" EmptyText="导入EXCEL文件" Width="150" ShowRedStar="true" AutoPostBack="true" OnFileSelected="btnSelect_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <%--<f:Button ID="btnBatchAudit" Icon="UserTick" Text="批量审核" ConfirmText="是否确认批量审核选中单据？" EnableDefaultState="false" OnClick="btnBatchAudit_Click" runat="server" />
                                                <f:Button ID="btnBatchPrint" Icon="Printer" Text="批量打印" runat="server" EnableDefaultState="false" OnClick="btnBatchPrint_Click" />
                                                <f:ToolbarSeparator runat="server" />--%>
                                                <f:Button ID="btnBatchAudit" Icon="UserTick" Text="批量审核" ConfirmText="是否确认批量审核选中单据？" EnableDefaultState="false" OnClick="btnBatchAudit_Click" runat="server" Hidden="true" />
                                                <f:Button ID="btnBatchPrint" Icon="Printer" Text="批量打印" runat="server" EnableDefaultState="false" OnClick="btnBatchPrint_Click" Hidden="true" />
                                                <f:Button ID="btExport" Icon="DatabaseGo" Text="导 出" EnableDefaultState="false" ConfirmText="是否确认导出商品入库数据?" EnablePostBack="true" runat="server" OnClick="btExport_Click" EnableAjax="false" DisableControlBeforePostBack="false" />
                                                <f:ToolbarSeparator runat="server" ID="ListLine" />
                                                <f:Button ID="btnAllCommit" Icon="UserTick" Text="提 交" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否确认提交选中单据？" OnClick="btnAllCommit_Click" />
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
                                                        <f:TextBox ID="lstBILLNO" runat="server" Label="单据编号" />
                                                        <f:DropDownList ID="lstFLAG" runat="server" Label="单据状态" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="lstLRY" runat="server" Label="录入员" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="lstDEPTID" runat="server" Label="收货地点" EnableEdit="true" ForceSelection="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TextBox ID="lstDDBH" runat="server" Label="订单号" />
                                                        <f:DropDownList ID="lstSUPID" runat="server" Label="送货商" EnableEdit="true" ForceSelection="true"></f:DropDownList>
                                                        <f:DatePicker ID="lstLRRQ1" runat="server" Label="录入日期" Required="true" ShowRedStar="true" />
                                                        <f:DatePicker ID="lstLRRQ2" runat="server" Label="　　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" AnchorValue="100% -113" ShowBorder="false" ShowHeader="false" EnableColumnLines="true" OnRowDataBound="GridList_RowDataBound"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableTextSelection="true" EnableCheckBoxSelect="true"
                                    DataKeyNames="SEQNO,FLAG,PSSID,PSSNAME,BILLTYPE,DDBH,FLAG_CN" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick"
                                    EnableHeaderMenu="true" SortField="SEQNO" SortDirection="ASC" AllowSorting="true" EnableMultiSelect="true" KeepCurrentSelection="true"
                                    OnSort="GridList_Sort">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField Hidden="true" DataField="SEQNO" />
                                        <f:BoundField Hidden="true" DataField="BILLTYPE" />
                                        <f:BoundField Width="120px" DataField="BILLNO" HeaderText="单据编号" SortField="BILLNO" TextAlign="Center" />
                                        <f:BoundField Width="0px" Hidden="true" DataField="FLAG" ColumnID="FLAG" HeaderText="单据状态" SortField="FLAG" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="FLAG_CN" ColumnID="FLAG_CN" HeaderText="单据状态" SortField="FLAG_CN" TextAlign="Center" />
                                        <f:BoundField Width="110px" DataField="DDBH" HeaderText="订单编号" SortField="DDBH" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="DEPTID" HeaderText="收货地点" SortField="DEPTID" TextAlign="Center" />
                                        <f:BoundField Width="150px" DataField="PSSID" HeaderText="送货商ID" SortField="PSSID" Hidden="true" />
                                        <f:BoundField Width="150px" DataField="PSSNAME" HeaderText="送货商名称" SortField="SUPNAME" />
                                        <f:BoundField Width="90px" DataField="DHRQ" HeaderText="收货日期" SortField="DHRQ" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
									<f:BoundField Width="40px" DataField="PRINTNUM" ColumnID="PRINTNUM" HeaderText="打印" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="SUBSUM" HeaderText="收货金额" SortField="SUBSUM" TextAlign="Right" DataFormatString="{0:F2}" />
                                        <f:BoundField Width="90px" DataField="CGY" HeaderText="业务员" SortField="CGY" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="LRY" HeaderText="录入员" SortField="LRY" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="LRRQ" HeaderText="录入日期" SortField="LRRQ" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="90px" DataField="SHR" HeaderText="审核员" SortField="SHR" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="SHRQ" HeaderText="审核日期" TextAlign="Center" SortField="SHRQ" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="400px" DataField="MEMO" HeaderText="备注" SortField="MEMO" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="单据信息" Icon="PageWord" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Region"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="Panel8" BodyPadding="0px" RegionSplit="false" EnableCollapse="false" RegionPosition="Top" ShowBorder="false"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar1" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="操作信息：单据操作主界面！" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="btnScan" Icon="TableConnect" Text="追溯码" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnScan_Click" />
                                                <f:Button ID="btnCK" Icon="Cart" Text="转出出库" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnCK_Click" Enabled="false" />
                                                <f:ToolbarSeparator runat="server" ID="WebLine1" />
                                                <f:Button ID="btnNew" Icon="PageAdd" Text="新 单" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnSave" Icon="Disk" Text="保 存" EnableDefaultState="false" EnablePostBack="true" ValidateForms="FormDoc" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnCommit" Icon="UserTick" Text="提 交" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否确认保存并提交此单据?" OnClick="btnCommit_Click" ValidateForms="FormDoc" />
                                                <f:ToolbarSeparator runat="server" ID="WebLine2" />
                                                <f:Button ID="btnCopy" Icon="PageCopy" Text="复 制" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认复制此单据信息?" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnDel" Icon="PageCancel" Text="整单删除" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否要删除此单据?" OnClick="btnBill_Click" />
                                                <f:Button ID="btnAddRow" Icon="Add" Text="增 行" EnableDefaultState="false" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnDelRow" Icon="Delete" Text="删 行" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认删除选中行信息?" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" />
                                                <f:ToolbarSeparator runat="server" ID="WebLine3" />
                                                <f:Button ID="btnAudit" Icon="UserTick" Text="审 核" EnableDefaultState="false" EnablePostBack="true"  runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnCancel" Icon="UserCross" Text="驳 回" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnHC" Icon="ArrowRedo" Text="回 撤" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否确认回撤此单据？" OnClick="btnHC_Click" />
                                                <f:ToolbarSeparator runat="server" ID="WebLine4" />
                                                <f:Button ID="btnPrint" Icon="Printer" Text="打 印" EnableDefaultState="false" runat="server" OnClientClick="PrintYSRKD()" />

                                                <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认导出此单据信息?" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" ID="WebLine5" />

                                                <f:Button ID="btnNext" Icon="ForwardBlue" Text="下 页" EnableDefaultState="false" EnablePostBack="true" runat="server" Hidden="true" OnClick="btnBill_Click" />
                                                <f:Button ID="btnBef" Icon="RewindBlue" Text="上 页" EnableDefaultState="false" EnablePostBack="true" runat="server" Hidden="true" OnClick="btnBill_Click" />
                                                <f:Button ID="btnGoods" Icon="Magnifier" Text="追加商品" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" />

                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                </f:Panel>
                                <f:Panel ID="Panel4" BodyPadding="0px" RegionSplit="true" EnableCollapse="true" RegionPosition="Top" ShowBorder="false"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Items>
                                        <f:Form ID="FormDoc" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow Hidden="true">
                                                    <Items>
                                                        <f:TextBox ID="docSEQNO" runat="server" Hidden="true" />
                                                        <f:TextBox ID="docDNLSEQNO" runat="server" Hidden="true" />

                                                        <f:TextBox ID="docNUM1" runat="server" Hidden="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TriggerBox ID="docDDBH" runat="server" Label="订单号" LabelWidth="80px" MaxLength="28" EmptyText="可输入订货单号,回车确认" ShowRedStar="true" TriggerIcon="Search" ShowTrigger="false" OnTriggerClick="docDDBH_TextChanged"></f:TriggerBox>
                                                        <f:DropDownList ID="docCGY" runat="server" Label="操作员" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true"></f:DropDownList>
                                                        <f:TextBox ID="docBILLNO" runat="server" Label="单据编号" EmptyText="系统自动生成" MaxLength="15" />
                                                        <f:DropDownList ID="docFLAG" runat="server" Label="单据状态" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                    </Items>
                                                </f:FormRow>

                                                <f:FormRow ColumnWidths="25% 25% 25% 25%">
                                                    <Items>
                                                        <f:DropDownList ID="docPSSID" runat="server" Label="送货商" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true">
                                                        </f:DropDownList>
                                                        <f:TextBox ID="docINVOICENUMBER" runat="server" Label="发票号" EmptyText="多个发票号用逗号分隔" MaxLength="500" />
                                                        <f:DropDownList ID="docLRY" runat="server" Label="录入员" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="docLRRQ" runat="server" Label="录入日期" Enabled="false" ShowRedStar="true" Required="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="docDEPTID" runat="server" Label="收货地点" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true">
                                                        </f:DropDownList>
                                                        <f:DatePicker ID="docDHRQ" runat="server" Label="收货日期" Required="true" ShowRedStar="true" />
                                                        <f:DropDownList ID="docSHR" runat="server" Label="审核员" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="docSHRQ" runat="server" Label="审核日期" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TextBox ID="docMEMO" Label="备 注" runat="server" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridCom" AnchorValue="100% -144" ShowBorder="false" ShowHeader="false" EnableColumnLines="true"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;"
                                    DataKeyNames="GDSEQ,MEMO,BZSL,YRKSL,DDSL" AllowCellEditing="true" ClicksToEdit="1" EnableAfterEditEvent="false"
                                    EnableSummary="true" SummaryPosition="Bottom" AllowColumnLocking="true" EnableTextSelection="true">
                                    <Columns>
                                        <f:RenderField Width="35px" EnableLock="true" Locked="true" ColumnID="ROWNO" DataField="ROWNO" FieldType="String" EnableHeaderMenu="false" TextAlign="Center">
                                            <Editor>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="120px" DataField="GDSEQ" ColumnID="GDSEQ" HeaderText="商品编码" FieldType="String" EnableHeaderMenu="false" EnableLock="true" Locked="true">
                                            <Editor>
                                                <f:Label ID="labGDSEQ" runat="server" BoxConfigAlign="Middle">
                                                </f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" DataField="GDNAME" ColumnID="GDNAME" HeaderText="商品名称" FieldType="String" EnableLock="true" Locked="true">
                                            <Editor>
                                                <f:Label ID="comGDNAME" BoxConfigAlign="Middle" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" DataField="GDSPEC" ColumnID="GDSPEC" HeaderText="商品规格" FieldType="String" EnableLock="true" Locked="true">
                                            <Editor>
                                                <f:Label ID="comGDSPEC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" DataField="BZSL" ColumnID="BZSL" HeaderText="入库数<font color='red'>*</font>" TextAlign="Center" FieldType="Int">
                                            <Editor>
                                                <f:NumberBox ID="comBZSL" CssClass="ColBlue" Required="true" runat="server" MinValue="0" DecimalPrecision="2" MaxValue="99999999" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" DataField="YRKSL" ColumnID="YRKSL" HeaderText="已入库数" TextAlign="Center" FieldType="Int">
                                            <Editor>
                                                <f:NumberBox ID="comYRKSL" CssClass="ColBlue" Required="true" runat="server" MinValue="0" DecimalPrecision="2" MaxValue="99999999" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" DataField="DDSL" ColumnID="DDSL" HeaderText="订货数" TextAlign="Center" FieldType="Int">
                                            <Editor>
                                                <f:Label ID="comDDSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="UNITNAME" DataField="UNITNAME" FieldType="String" HeaderText="入库单位" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="comUNITNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="95px" DataField="PH" ColumnID="PH" HeaderText="批号<font color='red'>*</font>" TextAlign="Center" EnableLock="true">
                                            <Editor>
                                                <f:TextBox ID="comPH" Required="true" runat="server" MaxLength="20" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" DataField="RQ_SC" ColumnID="RQ_SC" HeaderText="生产日期<font color='red'>*</font>" TextAlign="Center" FieldType="Date" Renderer="Date" RendererArgument="yyyy-MM-dd">
                                            <Editor>
                                                <f:DatePicker ID="comRQ_SC" Required="true" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" DataField="YXQZ" ColumnID="YXQZ" HeaderText="有效期至<font color='red'>*</font>" TextAlign="Center" FieldType="Date" Renderer="Date" RendererArgument="yyyy-MM-dd">
                                            <Editor>
                                                <f:DatePicker ID="comYXQZ" Required="true" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" DataField="DEPTID" ColumnID="DEPTID" HeaderText="收货科室编码" TextAlign="Right" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="Label2" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="110px" DataField="DEPTNAME" ColumnID="DEPTNAME" HeaderText="收货科室" TextAlign="Right" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="Label3" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="250px" DataField="PZWH" ColumnID="PZWH" HeaderText="注册证号<font color='red'>*</font>" TextAlign="Center">
                                            <Editor>
                                                <%--<f:Label ID="comPZWH" runat="server" />--%>
                                                <f:TextBox ID="comPZWH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" DataField="BZHL" ColumnID="BZHL" HeaderText="包装含量" TextAlign="Center" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comBZHL" runat="server" />
                                            </Editor>
                                        </f:RenderField>

                                        <f:RenderField Width="110px" DataField="SSSL" ColumnID="SSSL" HeaderText="入库数(最小单位)" TextAlign="Center" FieldType="Int" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comSSSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="110px" DataField="UNITSMALLNAME" ColumnID="UNITSMALLNAME" HeaderText="最小包装单位" FieldType="Auto" TextAlign="Center" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="labUNITSMALLNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" DataField="HWID" ColumnID="HWID" HeaderText="货位" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="comHWID" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField DataField="JXTAX" ColumnID="JXTAX" HeaderText="税率" TextAlign="Center" Width="0px" Hidden="true">
                                            <Editor>
                                                <f:Label ID="comJXTAX" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <%--<f:RenderField Width="80px" DataField="HSJJ" ColumnID="HSJJ" HeaderText="含税进价" TextAlign="Right" FieldType="Auto">
                                            <Editor>
                                                 <f:NumberBox ID="comHSJJ" CssClass="ColBlue" DecimalPrecision="6" NoNegative="True" Required="True" runat="server" MinValue="0" MaxValue="99999999" />
                                             </Editor>
                                        </f:RenderField>--%>
                                        <f:RenderField Width="90px" DataField="HSJJ" ColumnID="HSJJ" HeaderText="含税进价" TextAlign="Right" FieldType="String" RendererFunction="round4">
                                            <Editor>
                                                <f:Label ID="Label1" runat="server" />
                                            </Editor>
                                        </f:RenderField>

                                        <f:RenderField Width="90px" DataField="HSJE" ColumnID="HSJE" HeaderText="含税金额" TextAlign="Right" RendererFunction="round2">
                                            <Editor>
                                                <f:Label ID="comHSJE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" ColumnID="PRODUCERNAME" DataField="PRODUCERNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="生产厂家">
                                            <Editor>
                                                <f:Label ID="lblEditorPRODUCERNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="95px" DataField="MJPH" ColumnID="MJPH" HeaderText="灭菌批号" TextAlign="Center">
                                            <Editor>
                                                <f:TextBox ID="tbxMJPH" Required="true" runat="server" MaxLength="20" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" DataField="MJRQ" ColumnID="MJRQ" HeaderText="灭菌日期" TextAlign="Center" FieldType="Date" Renderer="Date" RendererArgument="yyyy-MM-dd">
                                            <Editor>
                                                <f:DatePicker ID="dpkMJRQ" Required="true" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" DataField="MJXQ" ColumnID="MJXQ" HeaderText="灭菌效期" TextAlign="Center" FieldType="Date" Renderer="Date" RendererArgument="yyyy-MM-dd">
                                            <Editor>
                                                <f:DatePicker ID="dpkMJXQ" Required="true" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <%--<f:RenderField Width="180px" ColumnID="SUPNAME" DataField="SUPNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="供应商">
                                            <Editor>
                                                <f:Label ID="lblSUPNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>--%>
                                        <f:RenderField Width="0px" Hidden="true" DataField="ZPBH" ColumnID="ZPBH" HeaderText="制品编号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="comZPBH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" DataField="MEMO" ColumnID="MEMO" HeaderText="备注" TextAlign="Center">
                                            <Editor>
                                                <f:TextBox ID="comMEMO" runat="server" MaxLength="80" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" DataField="BARCODE" ColumnID="BARCODE" HeaderText="商品条码" TextAlign="Center"
                                            FieldType="String" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comBARCODE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="UNIT" DataField="UNIT" FieldType="String" HeaderText="包装单位编码" TextAlign="Center"
                                            EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comUNIT" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" DataField="ISLOT" ColumnID="ISLOT" HeaderText="批号管理" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comISLOT" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" DataField="ISGZ" ColumnID="ISGZ" HeaderText="是否贵重" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comISGZ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <%--<f:RenderField Width="0px" DataField="CODEINFO" ColumnID="CODEINFO" HeaderText="商品赋码信息" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comCODEINFO" runat="server" />
                                            </Editor>
                                        </f:RenderField>--%>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="SUPID" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="供应商">
                                            <Editor>
                                                <f:Label ID="lblSUPID" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="PRODUCER" DataField="PRODUCER" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="生产厂家编码" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorPRODUCER" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="NUM1" DataField="NUM1" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="订单行号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblNUM1" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                    </Columns>
                                    <Listeners>
                                        <f:Listener Event="beforeedit" Handler="onGridBeforeEdit" />
                                        <f:Listener Event="afteredit" Handler="onGridAfterEdit" />
                                    </Listeners>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                        <f:HiddenField ID="ColorForGridGoods" runat="server"></f:HiddenField>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
        <f:HiddenField ID="hfdSave" runat="server" />
        <f:HiddenField ID="hfdValue" runat="server" />
        <f:HiddenField ID="hfdIsLog" runat="server" />
        <f:HiddenField ID="hfdOper" runat="server" />
        <f:HiddenField ID="hfdDG" runat="server" />
        <f:HiddenField ID="hfdOneCode" runat="server" />
        <f:HiddenField ID="print_liu" runat="server" Text=""></f:HiddenField>
        <f:HiddenField ID="print_a4" runat="server" Text=""></f:HiddenField>
        <f:HiddenField ID="highlightRows" runat="server"></f:HiddenField>
        <f:HiddenField ID="highlightRowYellow" runat="server"></f:HiddenField>
        <f:HiddenField ID="hdfScan" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdCurrent" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdBills" runat="server" Label="需要打印的单据号"></f:HiddenField>
        <f:Window ID="Window1" Title="商品信息" Hidden="true" EnableIFrame="true" runat="server"
            EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True"
            Width="820px" Height="480px" OnClose="Window1_Close">
        </f:Window>
        <f:Window ID="WindowLot" Title="商品批号信息" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="510px" Height="360px">
            <Items>
                <f:Grid ID="GridLot" ShowBorder="false" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server"
                    DataKeyNames="PH,YXQZ,RQ_SC,PZWH">
                    <Columns>
                        <f:BoundField Width="70px" DataField="GDSEQ" Hidden="true" />
                        <f:BoundField Width="100px" DataField="PH" HeaderText="批号" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="YXQZ" HeaderText="效期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="100px" DataField="RQ_SC" HeaderText="生产日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="120px" DataField="PZWH" HeaderText="注册证号" TextAlign="Center" />
                        <f:TemplateField HeaderText="数量" Width="70px" TextAlign="Center">
                            <ItemTemplate>
                                <asp:TextBox runat="server" Width="98%" ID="tbxNumber" CssClass="number"
                                    TabIndex='<%# Container.DataItemIndex + 100 %>' Text='<%# Eval("SL") %>'></asp:TextBox>
                            </ItemTemplate>
                        </f:TemplateField>
                    </Columns>
                </f:Grid>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar3" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:HiddenField ID="hfdRowIndex" runat="server" />
                        <f:Button ID="btnClosePostBack" Text="确定" Icon="SystemSave" runat="server" OnClick="btnClosePostBack_Click">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
        <f:Window ID="WindowReject" Title="驳回信息" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="370px" Height="220px">
            <Items>
                <f:Form ID="Form2" ShowBorder="false" AutoScroll="false" BodyPadding="5px 10px"
                    ShowHeader="False" LabelWidth="75px" runat="server">
                    <Rows>
                        <f:FormRow>
                            <Items>
                                <f:DropDownList ID="ddlReject" runat="server" Label="驳回原因" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:TextArea ID="txaMemo" runat="server" Label="详细说明" Height="100px" />
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar4" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="btnRejectSubmit" Text="确定" Icon="SystemSave" runat="server" EnableDefaultState="false" OnClick="btnRejectSubmit_Click" ValidateForms="Form2">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
        <f:Window ID="WindowScan" Title="追溯码扫描信息(自动保存)" Hidden="true" EnableIFrame="false" runat="server" AutoScroll="true"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="860px" Height="500px">
            <Items>
                <f:Grid ID="GridSacn" ShowBorder="false" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server" EnableColumnLines="true" OnRowDataBound="GridSacn_RowDataBound"
                    DataKeyNames="ONECODE" EnableTextSelection="true">
                    <Columns>
                        <f:RowNumberField runat="server" Width="40px" TextAlign="Center" />
                        <f:CheckBoxField Width="40px" RenderAsStaticField="true" DataField="FLAGNAME" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="GDSEQ" HeaderText="商品编码" Hidden="true" />
                        <f:BoundField Width="145px" DataField="GDNAME" HeaderText="商品名称" />
                        <f:BoundField Width="91px" DataField="GDSPEC" HeaderText="商品规格" />
                        <f:BoundField Width="50px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                        <f:BoundField Width="120px" DataField="PH" HeaderText="批号" TextAlign="Center" />
                        <f:BoundField Width="50px" DataField="BZSL" HeaderText="入库数" TextAlign="Center" Hidden="true" />
                        <f:BoundField Width="230px" ColumnID="ONECODE" DataField="ONECODE" HeaderText="商品追溯码" TextAlign="Center"></f:BoundField>
                        <f:BoundField Width="180px" DataField="STR1" HeaderText="本位码" TextAlign="Center" />
                    </Columns>
                </f:Grid>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar5" runat="server" Position="Top" ToolbarAlign="Right">
                    <Items>
                        <f:CheckBox Text="手工" runat="server" ID="Input"></f:CheckBox>
                        <f:TriggerBox ID="zsmERP" runat="server" Label="ERP条码" LabelWidth="80px" MaxLength="50" EmptyText="扫描ERP追溯码追溯码" ShowRedStar="true" TriggerIcon="Search" ShowTrigger="false" OnTriggerClick="zsmERP_TextChanged"></f:TriggerBox>
                        <f:TriggerBox ID="zsmScan" runat="server" Label="本位码" LabelWidth="80px" EmptyText="扫描生产追溯码" TriggerIcon="Search" ShowTrigger="false" OnTriggerClick="zsmScan_TextChanged"></f:TriggerBox>
                        <f:Button ID="zsmALL" Icon="TableRowInsert" Text="一键入库" EnableDefaultState="false" runat="server" ConfirmText="是否将全部追溯码入库？" OnClick="zsmALL_Click"></f:Button>
                        <f:Button ID="zsmDelete" Icon="Delete" Text="删 除" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否删除选中商品的本位码?" OnClick="zsmDelete_Click" />
                        <f:Button ID="btnLabelPrint" Text="标签打印" EnableDefaultState="false" Icon="Printer" runat="server" OnClientClick="btnPrint_BQ()"></f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
        <f:Window ID="Window2" Title="" Hidden="true" EnableIFrame="true" runat="server"
            EnableMaximize="true" EnableResize="true" Width="600px" Height="800px" Layout="Fit">
        </f:Window>
        <f:Window ID="winCk" Title="入库转出库" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="300px" Height="110px">
            <Items>
                <f:Form ID="Form3" ShowBorder="false" AutoScroll="false" BodyPadding="5px 10px"
                    ShowHeader="False" LabelWidth="75px" runat="server">
                    <Rows>
                        <f:FormRow>
                            <Items>
                                <f:DropDownList ID="ddlDEPTCK" runat="server" Label="出库部门" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar6" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="btnCkSure" Text="确定" Icon="SystemSave" EnableDefaultState="false" runat="server" OnClick="btnCkSure_Click">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
    </form>
    <script type="text/javascript">
        var GridScan = '<%=GridSacn.ClientID%>';
        function getStr2s() {
            var str2 = "";
            var data = F(GridScan).data;
            var selected = F(GridScan).getSelectedRows();
            for (var i = 0; i < data.length; i++) {
                var values = data[i].values;
                //for (var j = 0; j < selected.length; j++) {
                    //if (data[i].id == selected[j]) {
                        str2 += values["ONECODE"]+",";
                    //}
                //}
            }
            str2 = str2.substring(0, str2.length - 1)
            return str2;
        }
       function OnStr2PrintEnd() {
            //打印数据已经成功发送到打印机数据缓冲池触发本事件
           var billNo = getStr2s();
            var lry = F('<%= hfdCurrent.ClientID%>').getValue();
            $.post("/captcha/PrintReport.aspx?Method=PrintNum", { seqno: billNo, user: lry, oper: 'P' });
        }

        function checkandsave() {
            var showtem = "";
            var i = 0;
            for (i = 0; i <= F('<%=GridCom.Rows.Count%>') ; i++) {

            }
            if (showtem.length > 0) {
                confirm(showtem);
            }
        }
        function PosGoodsQt(Info) {
            F('<%= hfdValue.ClientID%>').setValue(Info);
            F.customEvent('GoodsAdd');
        }

        function PrintYSRKD() {
            var billNo = F('<%= hfdBills.ClientID%>').getValue();
            if (billNo == "") {
                F.alert("请选择要打印的单据信息！");
                return;
            }
            var Report = ReportViewer.Report;
            Report.OnPrintEnd = OnPrintEnd();
            ReportViewer.ReportURL = "<%=YSRKD%>?time=" + new Date().getTime();
            var dataurl = "/captcha/PrintReport.aspx?Method=GetStorageData&osid=" + billNo;

            ReportViewer.Report.LoadDataFromURL(dataurl);

            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();

        }

        function OnPrintEnd() {
            //打印数据已经成功发送到打印机数据缓冲池触发本事件
            var billNo = F('<%= hfdBills.ClientID%>').getValue();
            var lry = F('<%= hfdCurrent.ClientID%>').getValue();
            $.post("/captcha/PrintReport.aspx?Method=EidtPrintNum", { seqno: billNo, user: lry, oper: 'P' });
            F.customEvent("Search");
        }
        function btnPrint_BQ() {
            //高值码打印
            var billNo = F('<%= docSEQNO.ClientID%>').getValue();
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
            Report.OnPrintEnd = OnStr2PrintEnd();
            ReportViewer.ReportURL = "/grf/TraceCode.grf?timestamp=" + new Date().getTime();
            var dataurl = "/captcha/PrintReport.aspx?Method=GoodsRkGz&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }

        var arrEditor = ["BZSL", "PH", "RQ_SC", "YXQZ", "PZWH"];//Gird中可编辑的表格
        function onGridBeforeEdit(event, value, params) {

            if ((F('<%= docFLAG.ClientID%>').getValue() == "M" || F('<%= docFLAG.ClientID%>').getValue() == "R") && F('<%= hfdOper.ClientID%>').getValue() == "input") {
                if (arrEditor.indexOf(params.columnId) >= 0) {
                    return true;
                }
                else {
                    return false;
                }
            }
            else if (F('<%= docFLAG.ClientID%>').getValue() == "N" && F('<%= hfdOper.ClientID%>').getValue() == "audit" && params.columnId == "PH") {
                return true;
            }
            else
                return false;
        }
        function onGridAfterEdit(event, value, params) {
            var me = this, columnId = params.columnId, rowId = params.rowId;
            if (columnId === 'BZSL') {
                var BZSL = me.getCellValue(rowId, 'BZSL');
                var BZHL = me.getCellValue(rowId, 'BZHL');
                var HSJJ = me.getCellValue(rowId, 'HSJJ');
                me.updateCellValue(rowId, 'SSSL', BZSL * BZHL);
                me.updateCellValue(rowId, 'HSJE', BZSL * HSJJ);
                var BZSLTotal = 0, HSJETotal = 0, DHSLTotal = 0;
                me.getRowEls().each(function (index, tr) {
                    BZSLTotal += me.getCellValue(tr, 'BZSL');
                    //   DHSLTotal += me.getCellValue(tr, 'SSSL'); 
                    DHSLTotal += me.getCellValue(tr, 'DDSL');
                    HSJETotal += me.getCellValue(tr, 'BZSL') * me.getCellValue(tr, 'HSJJ');
                });
                me.updateSummaryCellValue('GDNAME', "本页合计", true);
                me.updateSummaryCellValue('BZSL', BZSLTotal, true);
                me.updateSummaryCellValue('HSJE', HSJETotal, true);
                me.updateSummaryCellValue('DDSL', DHSLTotal, true);
            }
        }
        //下载模板
        function DownLoadModelclick() {
            window.location.href = '验收入库导入模板.xls';
        }
    </script>
</body>
</html>
