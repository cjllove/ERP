<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SalesReturn.aspx.cs" Inherits="ERPProject.ERPStorage.SalesReturn" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>供应商退货</title>
    <script src="../res/js/CreateControl.js" type="text/javascript"></script>
    <script src="../res/js/GRInstall.js" type="text/javascript"></script>
</head>
<body>
    <script type="text/javascript">
        CreateDisplayViewerEx("0%", "0%", "", "", false, "");
    </script>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="TabStrip1" runat="server" OnCustomEvent="PageManager1_CustomEvent" />
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

                                                <f:ToolbarFill ID="ToolbarFill3" runat="server" />
                                                <f:Button ID="bntClear" Icon="Erase" Text="清 空" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />

                                                <f:Button ID="btnAllCommit" Icon="UserTick" Text="提 交" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否确认提交选中单据？" OnClick="btnAllCommit_Click" />
                                                 <f:Button ID="btExport" Icon="DatabaseGo" Text="导 出" EnableDefaultState="false" ConfirmText="是否确认导出采购退货信息?" EnablePostBack="true" runat="server" OnClick="btExport_Click" EnableAjax="false" DisableControlBeforePostBack="false" />
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
                                                        <f:TextBox ID="lstBILLNO" runat="server" Label="单据编号" EmptyText="请输入退货单号查询"/>
                                                        <f:DropDownList ID="lstFLAG" runat="server" Label="单据状态" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="lstCGY" runat="server" Label="退货员" EnableEdit="true" ForceSelection="true">
                                                        </f:DropDownList>
                                                        <f:DropDownList ID="lstDEPTID" runat="server" Label="退货地点" EnableEdit="true" ForceSelection="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TextBox ID="lstRKDH" runat="server" Label="原单号" EmptyText="请输入入库单号查询"/>
                                                        <f:DropDownList ID="lstPSSID" runat="server" Label="送货商" EnableEdit="true" ForceSelection="true">
                                                        </f:DropDownList>
                                                        <f:DatePicker ID="lstLRRQ1" runat="server" Label="录入日期" Required="true" ShowRedStar="true" />
                                                        <f:DatePicker ID="lstLRRQ2" runat="server" Label="　　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" AnchorValue="100% -113" ShowBorder="false" ShowHeader="false" EnableCheckBoxSelect="true" EnableMultiSelect="true"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" OnRowDataBound="GridList_RowDataBound" EnableTextSelection="true"
                                    DataKeyNames="SEQNO,FLAG" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick" EnableColumnLines="true"
                                    EnableHeaderMenu="true" SortField="SEQNO" OnSort="GridList_Sort" SortDirection="ASC" AllowSorting="true" KeepCurrentSelection="true">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField Hidden="true" DataField="SEQNO" />
                                        <f:BoundField Width="110px" DataField="BILLNO" HeaderText="单据编号" SortField="BILLNO" TextAlign="Center" />
                                        <f:BoundField Width="0px" Hidden="true" DataField="FLAG" HeaderText="单据状态" TextAlign="Center" SortField="FLAG" />
                                        <f:BoundField Width="90px" DataField="FLAG_CN" ColumnID="FLAG_CN" HeaderText="单据状态" TextAlign="Center" SortField="FLAG_CN" />
                                        <f:BoundField Width="110px" DataField="RKDH" HeaderText="入库单编号" TextAlign="Center" SortField="RKDH" />
                                        <f:BoundField Width="120px" DataField="DEPTID" HeaderText="退货部门" SortField="DEPTID" />
                                        <f:BoundField Width="170px" DataField="SUPNAME" HeaderText="送货商名称" SortField="SUPNAME" />
                                        <f:BoundField Width="80px" DataField="THRQ" HeaderText="退货日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="THRQ" />
                                        <f:BoundField Width="90px" DataField="SUBSUM" HeaderText="退货金额" TextAlign="Right" DataFormatString="{0:F2}" SortField="SUBSUM" />
                                        <f:BoundField Width="70px" DataField="CGY" HeaderText="业务员" TextAlign="Center" SortField="CGY" />
                                        <f:BoundField Width="70px" DataField="LRY" HeaderText="录入员" TextAlign="Center" SortField="LRY" />
                                        <f:BoundField Width="90px" DataField="LRRQ" HeaderText="录入日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="LRRQ" />
                                        <f:BoundField Width="70px" DataField="SHR" HeaderText="审核员" TextAlign="Center" SortField="SHR" />
                                        <f:BoundField Width="90px" DataField="SHRQ" HeaderText="审核日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="SHRQ" />
                                        <f:BoundField Width="100px" DataField="MEMO" HeaderText="备注" TextAlign="Center" SortField="MEMO" HtmlEncode="false" DataToolTipField="MEMO" />
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
                                                <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="操作信息：库房退货主界面！<b style='color:red;'>退货数为负数</b>" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="btnScan" Icon="TableConnect" Text="追溯码" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnScan_Click" />
                                                <f:ToolbarSeparator runat="server" ID="WebLine1" />
                                                <f:Button ID="btnNew" Icon="PageAdd" Text="新 单" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnSave" Icon="Disk" Text="保 存" EnableDefaultState="false" ValidateForms="FormDoc" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnCommit" Icon="UserTick" Text="提 交" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否保存并提交此单据？" runat="server" OnClick="btnCommit_Click" ValidateForms="FormDoc" />
                                                <f:ToolbarSeparator runat="server" ID="WebLine2" />
                                                <f:Button ID="btnCopy" Icon="PageCopy" Text="复 制" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认复制此单据信息?" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnDel" Icon="PageCancel" Text="整单删除" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否确定删除此单据?" OnClick="btnBill_Click" />
                                                <f:Button ID="btnAddRow" Icon="Add" Text="增 行" EnableDefaultState="false" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" />
                                                <f:Button ID="btnDelRow" Icon="Delete" Text="删 行" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否确定删除选中行?" OnClick="btnBill_Click" />

                                                <f:ToolbarSeparator runat="server" ID="WebLine3" />
                                                <f:Button ID="btnAudit" Icon="UserTick" Text="审 核" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认审核此单据?" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnCancel" Icon="UserCross" Text="驳 回" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />

                                                <f:ToolbarSeparator runat="server" ID="WebLine4" />
                                                <f:Button ID="btnPrint" Icon="Printer" Text="打 印" EnableDefaultState="false" EnablePostBack="false" runat="server" OnClientClick="btnPrint_onclick()" />
                                                <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认导出此单据信息?" runat="server" OnClick="btnBill_Click" />

                                                <f:Button ID="btnNext" Icon="ForwardBlue" Text="下 页" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnBef" Icon="RewindBlue" Text="上 页" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" ID="WebLine5" />
                                                <f:Button ID="btnGoods" Icon="Magnifier" Text="追加商品" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" />
                                                <f:Button ID="btnGoodsZP" Icon="ZoomIn" Text="追加赠品" Hidden="true" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnGoodsZP_Click" ValidateForms="FormDoc" />
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
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <%--<f:TextBox ID="docRKDH" runat="server" Label="入库单号" EmptyText="请填写对应的入库单号" Required="true" ShowRedStar="true" MaxLength="15" AutoPostBack="true" OnTextChanged="docRKDH_TextChanged" />--%>
                                                        <f:TriggerBox ID="tgbRKDH" runat="server" Label="入库单号" EmptyText="请填写对应的入库单号" TriggerIcon="Search" Required="true" ShowRedStar="true" MaxLength="15" AutoPostBack="true" OnTriggerClick="tgbRKDH_TriggerClick" />
                                                        <f:DropDownList ID="docCGY" runat="server" Label="退货员" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                                                        <f:TextBox ID="docBILLNO" runat="server" Label="单据编号" MaxLength="15" EmptyText="系统自动生成" />
                                                        <f:DropDownList ID="docFLAG" runat="server" Label="单据状态" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                    </Items>
                                                </f:FormRow>

                                                <f:FormRow ColumnWidths="50% 25% 25%">
                                                    <Items>
                                                        <f:DropDownList ID="docPSSID" runat="server" Label="送货商" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true">
                                                        </f:DropDownList>
                                                        <f:DropDownList ID="docSHR" runat="server" Label="审核员" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="docSHRQ" runat="server" Label="审核日期" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="docDEPTID" runat="server" Label="退货地点" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="docTHRQ" runat="server" Label="退货日期" Required="true" ShowRedStar="true" />
                                                        <f:DropDownList ID="docLRY" runat="server" Label="录入员" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="docLRRQ" runat="server" Label="录入日期" Enabled="false" Required="true" ShowRedStar="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow ColumnWidths="25% 75%">
                                                    <Items>
                                                        <f:DropDownList ID="docTHTYPE" runat="server" Label="退货原因" Required="true" ShowRedStar="true" EnableEdit="false" ForceSelection="true" />
                                                        <f:TextBox ID="docMEMO" runat="server" Label="退货说明" MaxLength="80" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridGoods" AnchorValue="100% -175" ShowBorder="false" ShowHeader="false" EnableSummary="true" SummaryPosition="Bottom"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableColumnLines="true" EnableTextSelection="true"
                                    DataKeyNames="GDSEQ" AllowCellEditing="true" ClicksToEdit="1" EnableAfterEditEvent="true" OnAfterEdit="GridGoods_AfterEdit" AllowColumnLocking="true">
                                    <Columns>
                                        <f:RenderField Width="35px" EnableLock="true" TextAlign="Center" Locked="true" ColumnID="ROWNO" DataField="ROWNO" FieldType="String" EnableHeaderMenu="false">
                                            <Editor>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" DataField="GDSEQ" ColumnID="GDSEQ" HeaderText="商品编码" FieldType="String" EnableHeaderMenu="false" EnableLock="true" Locked="true">
                                            <Editor>
                                                <f:Label ID="labGDSEQ" runat="server" BoxConfigAlign="Middle">
                                                </f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="165px" DataField="GDNAME" ColumnID="GDNAME" HeaderText="商品名称" FieldType="String" EnableHeaderMenu="false" EnableLock="true" Locked="true">
                                            <Editor>
                                                <f:Label ID="comGDNAME" BoxConfigAlign="Middle" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="120px" DataField="GDSPEC" ColumnID="GDSPEC" HeaderText="商品规格" FieldType="String" EnableHeaderMenu="false" EnableLock="true" Locked="true">
                                            <Editor>
                                                <f:Label ID="comGDSPEC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" DataField="BZSL" ColumnID="BZSL" HeaderText="退货数<font color='red'>*</font>" TextAlign="Center" FieldType="Auto">
                                            <Editor>
                                                <f:NumberBox ID="comBZSL" CssClass="ColBlue" Required="true" runat="server" MaxValue="0" NoDecimal="True" DecimalPrecision="6" MaxLength="18" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="UNITNAME" DataField="UNITNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="包装单位" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorUNITNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" DataField="BZHL" ColumnID="BZHL" HeaderText="包装含量" TextAlign="Center" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comBZHL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="110px" DataField="THSL" ColumnID="THSL" HeaderText="退货数量(最小包装)" TextAlign="Center" FieldType="Int"
                                            EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comTHSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="KCSL" DataField="KCSL" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="可退数量" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="comKCSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="KC" DataField="KC" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="剩余库存" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="comKC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" Hidden="true" DataField="UNITSMALLNAME" ColumnID="UNITSMALLNAME" HeaderText="最小包装单位" FieldType="Auto" TextAlign="Center" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="Label1" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" DataField="HWID" ColumnID="HWID" HeaderText="货位" TextAlign="Center" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comHWID" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" DataField="PH" ColumnID="PH" HeaderText="批号" TextAlign="Center" EnableHeaderMenu="false">
                                            <Editor>
                                                <%-- <f:TextBox ID="comPH" Required="true" runat="server" />--%>
                                                <f:Label ID="lblPH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="110px" ColumnID="YXQZ" DataField="YXQZ" EnableHeaderMenu="false"
                                            HeaderText="有效期至" TextAlign="Center" Renderer="Date" RendererArgument="yyyy-MM-dd" FieldType="Date">
                                            <Editor>
                                                <f:Label ID="comYXQZ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="110px" DataField="RQ_SC" ColumnID="RQ_SC" HeaderText="生产日期" TextAlign="Center" Renderer="Date" RendererArgument="yyyy-MM-dd" FieldType="Date">
                                            <Editor>
                                                <f:Label ID="comRQ_SC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" DataField="JXTAX" ColumnID="JXTAX" HeaderText="税率" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="comJXTAX" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" DataField="HSJJ" ColumnID="HSJJ" HeaderText="含税进价" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblHSJJ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" DataField="HSJE" ColumnID="HSJE" HeaderText="含税金额" TextAlign="Right">
                                            <Editor>
                                                <f:Label ID="comHSJE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" Hidden="true"  ColumnID="NUM1NAME" DataField="NUM1NAME" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="赠品标志" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="Label2" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="150px" DataField="PRODUCERNAME" ColumnID="PRODUCERNAME" FieldType="String" HeaderText="生产商" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="comPRODUCERNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" DataField="PZWH" ColumnID="PZWH" HeaderText="注册证号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="comPZWH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" hidden="true" DataField="ZPBH" ColumnID="ZPBH" HeaderText="制品编号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="comZPBH" runat="server" />
                                            </Editor>
                                        </f:RenderField>

                                        <f:RenderField Width="100px" DataField="MEMO" ColumnID="MEMO" HeaderText="备注" TextAlign="Center">
                                            <Editor>
                                                <f:TextBox ID="comMEMO" Required="true" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" DataField="BARCODE" ColumnID="BARCODE" HeaderText="商品条码" TextAlign="Center" FieldType="String" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comBARCODE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="UNIT" DataField="UNIT" FieldType="String" HeaderText="包装单位编码" TextAlign="Center"
                                            EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="lblEditorUNIT" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" DataField="PRODUCER" ColumnID="PRODUCER" FieldType="String" HeaderText="生产商编码"
                                            EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comPRODUCER" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" DataField="SUPID" ColumnID="SUPID" HeaderText="供应商编码" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comSUPID" runat="server" />
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
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="NUM1" DataField="NUM1" EnableHeaderMenu="false" EnableColumnHide="false" HeaderText="赠品标志" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="comNUM1" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" DataField="SUPNAME" ColumnID="SUPNAME" HeaderText="供应商" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="comSUPNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" DataField="NUM2" ColumnID="NUM2" HeaderText="系号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="comNUM2" runat="server" />
                                            </Editor>
                                        </f:RenderField>
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
        <f:HiddenField ID="hfdOper" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdValue" runat="server" />
        <f:HiddenField ID="highlightRows" runat="server"></f:HiddenField>
        <f:HiddenField ID="highlightRowYellow" runat="server"></f:HiddenField>
        <f:HiddenField ID="hdfZP" runat="server" />
        <f:Window ID="Window1" Title="商品信息" Hidden="true" EnableIFrame="true" runat="server"
            EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True"
            Width="820px" Height="480px" OnClose="Window1_Close">
        </f:Window>
        <f:Window ID="WindowLot" Title="商品批号信息" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="830px" Height="400px">
            <Items>
                <f:Grid ID="GridLot" ShowBorder="false" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server"
                    DataKeyNames="GDSEQ,PH,ROWNO" EnableColumnLines="true">
                    <Columns>
                        <f:BoundField Width="30px" DataField="ROWNO" TextAlign="Center" />
                        <f:BoundField Width="105px" DataField="GDSEQ" HeaderText="商品编码" />
                        <f:BoundField Width="160px" DataField="GDNAME" HeaderText="商品名称" TextAlign="Left" />
                        <f:BoundField Width="90px" DataField="PH" HeaderText="批号" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="YXQZ" HeaderText="效期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="80px" DataField="RQ_SC" HeaderText="生产日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="80px" DataField="HWID" HeaderText="货位" />
                        <f:BoundField Width="80px" DataField="KCSL" HeaderText="可退数量" />
                        <f:BoundField Width="80px" DataField="KC" HeaderText="剩余库存" />
                        <f:BoundField Width="80px" DataField="NUM2" HeaderText="系号" Hidden="true"  />
                        <f:TemplateField HeaderText="数量" Width="70px" TextAlign="Center">
                            <ItemTemplate>
                                <asp:TextBox runat="server" Width="98%" ID="tbxNumber" CssClass="number"
                                    TabIndex='<%# Container.DataItemIndex + 100 %>' Text='<%# Eval("SL") %>'></asp:TextBox>
                            </ItemTemplate>
                        </f:TemplateField>
                        <f:BoundField Width="120px" DataField="PZWH" HeaderText="注册证号" TextAlign="Center" />
                    </Columns>
                </f:Grid>
            </Items>
             <Toolbars>
                <f:Toolbar ID="Toolbar8" runat="server" Position="Top" ToolbarAlign="left">
                    <Items>
                        <f:TriggerBox ID="tgblotGoods" runat="server" Label="商品信息" LabelWidth="75px" Width="220px" EmptyText="入库单商品名称、编码、助记码查询" OnTriggerClick="btnRkGoods_TriggerClick" />
                        <f:TriggerBox ID="tgbPh" runat="server" Label="批号信息" LabelWidth="75px" Width="310px" EmptyText="入库单商品批号查询" OnTriggerClick="btnRkGoods_TriggerClick" />
                        <f:Button runat="server" Text="查 询" ID="Button1" EnableDefaultState="false" OnClick="btnRkGoods_TriggerClick"></f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Toolbars>
                <f:Toolbar ID="Toolbar3" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:HiddenField ID="hfdRowIndex" runat="server" />
                        <f:Button ID="btnClosePostBack" Text="确定" Icon="SystemSave" runat="server" OnClick="btnClosePostBack_Click">
                        </f:Button>
                        <f:Button ID="btnClose" Text="关闭" Icon="SystemClose" runat="server" OnClick="btnClose_Click">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
        <f:Window ID="WindowReject" Title="驳回信息" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="370px" Height="200px">
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
                                <f:TextArea ID="txaMemo" runat="server" Label="详细说明" Height="80px" />
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar6" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="btnRejectSubmit" Text="确定" Icon="SystemSave" runat="server" OnClick="btnRejectSubmit_Click">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
        <f:Window ID="WindowScan" Title="追溯码扫描信息(自动保存)" Hidden="true" EnableIFrame="false" runat="server" AutoScroll="true"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="770px" Height="360px">
            <Items>
                <f:Grid ID="GridSacn" ShowBorder="false" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server" EnableColumnLines="true" EnableTextSelection="true"
                    DataKeyNames="ONECODE" AllowCellEditing="true" ClicksToEdit="1">
                    <Columns>
                        <f:RowNumberField runat="server" Width="50px" />
                        <f:BoundField Width="90px" DataField="GDSEQ" HeaderText="商品编码" Hidden="true" />
                        <f:BoundField Width="145px" DataField="GDNAME" HeaderText="商品名称" />
                        <f:BoundField Width="91px" DataField="GDSPEC" HeaderText="商品规格" />
                        <f:BoundField Width="50px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                        <f:BoundField Width="55px" DataField="BZHL" HeaderText="包装含量" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="PH" HeaderText="批号" TextAlign="Center" />
                        <f:BoundField Width="180px" DataField="ONECODE" HeaderText="商品追溯码"></f:BoundField>
                        <f:BoundField Width="180px" DataField="STR1" HeaderText="供应商追溯码" TextAlign="Center" />
                    </Columns>
                </f:Grid>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar5" runat="server" Position="Top" ToolbarAlign="Right">
                    <Items>
                        <%--<f:Button ID="btnLabelPrint" Text="标签打印" Icon="Printer" runat="server" OnClientClick="btnPrint_BQ()">
                        </f:Button>--%>
                        <f:ToolbarFill runat="server"></f:ToolbarFill>
                        <f:TextBox ID="zsmScan" runat="server" Label="扫描条码" LabelWidth="75px" EmptyText="扫描供应商追溯码" Width="350px" ShowRedStar="true" AutoPostBack="true" OnTextChanged="zsmScan_TextChanged"></f:TextBox>
                        <f:ToolbarSeparator runat="server" CssStyle="margin-left:30px" />
                        <f:Button ID="zsmDelete" Icon="Delete" Text="删 除" EnablePostBack="true" EnableDefaultState="false" runat="server" ConfirmText="是否删除选中商品追溯码?" OnClick="zsmDelete_Click" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
        <f:Window ID="WinBillno" Title="入库单号查询" Hidden="true" EnableIFrame="false" runat="server" EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="600px" Height="350px">
            <Items>
                <f:Grid ID="GridBill" ShowBorder="false" EnableTextSelection="true" OnPageIndexChange="GridBill_PageIndexChange" EnableMultiSelect="false" EnableCheckBoxSelect="true" KeepCurrentSelection="true"
                    ShowHeader="false" IsDatabasePaging="true" AllowPaging="true" OnRowDoubleClick="GridBill_RowDoubleClick" PageSize="10" AllowSorting="false" CheckBoxSelectOnly="false"
                    AutoScroll="true" runat="server" EnableColumnLines="true" DataKeyNames="SEQNO" EnableRowDoubleClickEvent="true">
                    <Columns>
                        <f:RowNumberField Width="30px" runat="server"></f:RowNumberField>
                        <f:BoundField Width="120px" DataField="SEQNO" HeaderText="单据编号" />
                        <f:BoundField Width="140px" DataField="DEPTNAME" HeaderText="入库库房" />
                        <f:BoundField Width="140px" DataField="PSSNAME" HeaderText="配送商" />
                        <f:BoundField Width="95px" DataField="SHRQ" HeaderText="入库日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                    </Columns>
                </f:Grid>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar4" runat="server" Position="Top" ToolbarAlign="Left">
                    <Items>
                        <f:TriggerBox ID="tgbBillNo" runat="server" Label="入库单号" LabelWidth="75px" Width="220px" EmptyText="输入入库单号查询" OnTriggerClick="btnSrchBill_Click" />
                        <f:TriggerBox ID="tgbGoods" runat="server" Label="商品信息" LabelWidth="75px" Width="310px" EmptyText="入库单商品名称、编码、简称查询" OnTriggerClick="btnSrchBill_Click" />
                        <f:Button runat="server" Text="查 询" ID="btnSrchBill" EnableDefaultState="false" OnClick="btnSrchBill_Click"></f:Button>
                    </Items>
                </f:Toolbar>
                <f:Toolbar ID="Toolbar7" runat="server" Position="Top" ToolbarAlign="Left">
                    <Items>
                        <f:DropDownList ID="ddlDEPTIN" runat="server" Label="入库部门" LabelWidth="75px" Width="220px"></f:DropDownList>
                        <f:DatePicker ID="dbkTime1" runat="server" Label="入库日期" ShowRedStar="true" Required="true" LabelWidth="75px" Width="170px" />
                        <f:DatePicker ID="dbkTime2" LabelWidth="40px" Width="140px" runat="server" Label="至" ShowRedStar="true" Required="true" CompareControl="dbkTime1" CompareOperator="GreaterThanEqual" CompareType="String" CompareValue="dbkTime1" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Toolbars>
                <f:Toolbar runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="btnBillSure" Text="确定" EnableDefaultState="false" Icon="SystemSave" runat="server" OnClick="btnBillSure_Click"></f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
    </form>
    <script type="text/javascript">
        function PosGoodsQt(Info) {
            F('<%= hfdValue.ClientID%>').setValue(Info);
            F.customEvent('GoodsAdd');
        }
        function btnPrint_onclick() {

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
            //ReportViewer.ReportURL = "/ERPXM/TJ_YKGZ/grf/GoodsRtn.grf?123";
            //var dataurl = "/ERPXM/TJ_YKGZ/TJPrintReport.aspx?Method=GoodsRtn&osid=" + billNo;
            ReportViewer.ReportURL = "<%=CGTHD%>?timestamp=" + new Date().getTime();
            var dataurl = "/captcha/PrintReport.aspx?Method=GoodsRtn&osid=" + billNo;

            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
        function onGridBeforeEdit(event, values, params) {
            var flag = F('<%= docFLAG.ClientID%>').getValue();
            var oper = F('<%= hfdOper.ClientID%>').getValue();
            if ((",R,M").indexOf(flag) > 0 && (",input").indexOf(oper) > 0) {
                return true;
            }
            else {
                return false;
            }
        }

    </script>
</body>
</html>
