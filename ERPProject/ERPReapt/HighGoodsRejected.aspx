<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HighGoodsRejected.aspx.cs" Inherits="ERPProject.ERPReapt.HighGoodsRejected" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>高值扫码退货</title>
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
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="TabStrip1"
            runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="1"
            runat="server">
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
                                                <f:Button ID="bntClear" Icon="Erase" Text="清 除" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnAllCommit" Icon="UserTick" Text="提 交" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否确认提交选中单据？" OnClick="btnAllCommit_Click" />
                                                <%--        <f:Button ID="btnEpt" Icon="DatabaseGo" Text="导 出" EnableDefaultState="false" EnableAjax="false" EnablePostBack="true" runat="server" ConfirmText="是否确认导出此信息?" DisableControlBeforePostBack="false" OnClick="btnEpt_Click" />--%>
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
                                                        <f:DropDownList ID="lstCGY" runat="server" Label="退货员" EnableEdit="true" ForceSelection="true">
                                                        </f:DropDownList>
                                                        <f:DropDownList ID="lstDEPTID" runat="server" Label="退货地点" EnableEdit="true" ForceSelection="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TextBox ID="lstRKDH" runat="server" Label="原单号" />
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
                                <f:Grid ID="GridList" AnchorValue="100% -106" ShowBorder="false" ShowHeader="false" EnableColumnLines="true"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" OnRowDataBound="GridList_RowDataBound"
                                    DataKeyNames="SEQNO,FLAG" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick" EnableTextSelection="true"
                                    EnableHeaderMenu="true" SortField="BILLNO" OnSort="GridList_Sort" SortDirection="ASC" AllowSorting="true"
                                    IsDatabasePaging="true" AllowPaging="true" PageSize="100" OnPageIndexChange="GridList_PageIndexChange" EnableMultiSelect="true" EnableCheckBoxSelect="true">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField Hidden="true" DataField="SEQNO" />
                                        <f:BoundField Width="110px" DataField="BILLNO" HeaderText="单据编号" SortField="BILLNO" TextAlign="Center" />
                                        <f:BoundField Width="0px" Hidden="true" DataField="FLAG" HeaderText="单据状态" TextAlign="Center" SortField="FLAG" />
                                        <f:BoundField Width="90px" DataField="FLAG_CN" ColumnID="FLAG_CN" HeaderText="单据状态" TextAlign="Center" SortField="FLAG_CN" />
                                        <f:BoundField Width="120px" DataField="DEPTID" HeaderText="退货部门" SortField="DEPTID" />
                                        <f:BoundField Width="170px" DataField="SUPNAME" HeaderText="送货商名称" SortField="SUPNAME" />
                                        <f:BoundField Width="80px" DataField="THRQ" HeaderText="退货日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="THRQ" />
                                        <f:BoundField Width="90px" DataField="SUBSUM" HeaderText="退货金额" TextAlign="Right" DataFormatString="{0:F2}" SortField="SUBSUM" />
                                        <f:BoundField Width="70px" DataField="CGY" HeaderText="业务员" TextAlign="Center" SortField="CGY" />
                                        <f:BoundField Width="70px" DataField="LRY" HeaderText="录入员" TextAlign="Center" SortField="LRY" />
                                        <f:BoundField Width="90px" DataField="LRRQ" HeaderText="录入日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="LRRQ" />
                                        <f:BoundField Width="70px" DataField="SHR" HeaderText="审核员" TextAlign="Center" SortField="SHR" />
                                        <f:BoundField Width="90px" DataField="SHRQ" HeaderText="审核日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="SHRQ" />
                                        <f:BoundField Width="100px" DataField="MEMO" HeaderText="备注" TextAlign="Center" SortField="MEMO" />
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
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="btnNew" Icon="PageAdd" Text="新 单" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnDel" Icon="PageCancel" Text="整单删除" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否删除整张单据?" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnSave" Icon="Disk" Text="保 存" EnableDefaultState="false" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" />

                                                <f:Button ID="btnCommit" Icon="UserTick" Text="提 交" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否保存并提交此单据？" runat="server" OnClick="btnCommit_Click" ValidateForms="FormDoc" />

                                                <f:Button ID="btnAudit" Icon="UserTick" Text="审 核" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认审核此单据?" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" />
                                                <f:Button ID="btnCancel" Icon="UserCross" Text="驳 回" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnScan" Icon="TableConnect" Text="追溯码" EnableDefaultState="false" EnablePostBack="true" runat="server" Hidden="true" OnClick="btnScan_Click" />
                                                <f:Button ID="btnPrt" Icon="Printer" Text="打印拣货单" EnableDefaultState="false" EnablePostBack="false" runat="server" Hidden="true" OnClientClick="btn_Bill()" />
                                                <f:Button ID="btnPrint" Icon="Printer" Text="打印" EnableDefaultState="false" EnablePostBack="false" runat="server" OnClientClick="btnPrint_Bill()" />
                                                <f:Button ID="btnExport" Hidden="true" Icon="DatabaseGo" Text="导 出" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否确认导出此单据信息?" OnClick="btnBill_Click" />

                                                <f:Button ID="btnDelRow" Icon="Delete" Text="删 行" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否删除选中行信息?" runat="server" OnClick="btnBill_Click" />

                                                <f:Button ID="btnGoods" Hidden="true" Icon="Magnifier" Text="追加商品" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" />
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
                                                        <f:TextBox ID="docNUM1" runat="server" Hidden="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TriggerBox ID="trbBARCODE" Label="高值条码" runat="server" EmptyText="扫描或输入高值码信息" ShowRedStar="true" OnTriggerClick="trbBARCODE_TriggerClick"></f:TriggerBox>
                                                        <f:DropDownList ID="docCGY" runat="server" Label="退货员" Required="true" ShowRedStar="true"></f:DropDownList>
                                                        <f:TextBox ID="docBILLNO" runat="server" Label="单据编号" MaxLength="20" EmptyText="自动生成" />
                                                        <f:DropDownList ID="docFLAG" runat="server" Label="单据状态" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="docDEPTID" runat="server" Label="退货地点" EnableEdit="true" ShowRedStar="true" ForceSelection="true">
                                                        </f:DropDownList>
                                                        <f:DatePicker ID="docTHRQ" runat="server" Label="退货日期" Required="true" ShowRedStar="true" />
                                                        <f:DropDownList ID="docLRY" runat="server" Label="录入员" Enabled="false" />
                                                        <f:DatePicker ID="docLRRQ" runat="server" Label="录入日期" Enabled="false" Required="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow ColumnWidths="50% 25% 25%">
                                                    <Items>
                                                        <f:DropDownList ID="docPSSID" runat="server" Label="送货商" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true"></f:DropDownList>
                                                        <f:DropDownList ID="docSHR" runat="server" Label="审批员" Enabled="false" />
                                                        <f:DatePicker ID="docSHRQ" runat="server" Label="审批日期" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow ColumnWidths="25% 75%">
                                                    <Items>
                                                        <f:DropDownList ID="docTHTYPE" runat="server" Label="退货原因" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                                                        <f:TextBox ID="docMEMO" runat="server" Label="退货说明" MaxLength="80" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridGoods" AnchorValue="100% -143" ShowBorder="false" ShowHeader="false" EnableSummary="true" SummaryPosition="Bottom" AllowColumnLocking="true"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableColumnLines="true" EnableTextSelection="true"
                                    DataKeyNames="GDSEQ" AllowCellEditing="true">
                                    <Columns>
                                        <f:RenderField Width="35px" EnableLock="true" TextAlign="Center" Locked="true" ColumnID="ROWNO" DataField="ROWNO" FieldType="String" EnableHeaderMenu="false">
                                            <Editor>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="280px" DataField="STR2" ColumnID="STR2" HeaderText="条码信息" EnableLock="true" Locked="true" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="Label1" runat="server" />
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
                                        <f:RenderField Width="90px" ColumnID="NUM1NAME" DataField="NUM1NAME" EnableHeaderMenu="false" EnableColumnHide="false"
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

                                        <f:RenderField Width="0px" Hidden="true" ColumnID="SPZTSL" DataField="SPZTSL" FieldType="Int" EnableHeaderMenu="false"
                                            HeaderText="在途数量" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="comSPZTSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" DataField="PZWH" ColumnID="PZWH" HeaderText="注册证号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="comPZWH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" DataField="ZPBH" ColumnID="ZPBH" HeaderText="制品编号" TextAlign="Center">
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
        <f:HiddenField ID="hdfIndex" runat="server" />
        <f:HiddenField ID="hfdOper" runat="server" />
        <f:HiddenField ID="hdfZP" runat="server" />
        <f:Window ID="Window1" Title="商品信息" Hidden="true" EnableIFrame="true" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True"
            Width="820px" Height="480px" OnClose="Window1_Close">
        </f:Window>
        <f:Window ID="WindowScan" Title="赋码扫描(自动保存)" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="580px" Height="340px">
            <Items>
                <f:Grid ID="GridSacn" ShowBorder="false" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server"
                    DataKeyNames="ONECODE" AllowCellEditing="true" ClicksToEdit="1">
                    <Columns>
                        <f:RowNumberField runat="server" Width="35px" />
                        <f:BoundField Width="90px" DataField="GDSEQ" HeaderText="商品编码" Hidden="true" />
                        <f:BoundField Width="145px" DataField="GDNAME" HeaderText="商品名称" />
                        <f:BoundField Width="91px" DataField="GDSPEC" HeaderText="商品规格" />
                        <f:BoundField Width="50px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                        <f:BoundField Width="53px" DataField="BZHL" HeaderText="包装含量" TextAlign="Center" />
                        <f:BoundField Width="50px" DataField="BZSL" HeaderText="入库数" TextAlign="Center" Hidden="true" />
                        <f:RenderField Width="180px" DataField="ONECODE" HeaderText="商品追溯码">
                            <Editor>
                                <f:Label ID="lalONECODE" runat="server" />
                            </Editor>
                        </f:RenderField>
                    </Columns>
                </f:Grid>
            </Items>
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
    </form>
    <script type="text/javascript">
        function onGridBeforeEdit(event, value, params) {
            if (F('<%= docFLAG.ClientID%>').getValue() == "M" || F('<%= docFLAG.ClientID%>').getValue() == "R") {
                return true;
            }
            else
                return false;
        }
        function btnPrint_Bill() {
            var billNo = F('<%= docBILLNO.ClientID%>').getValue();
            var billState = F('<%= docFLAG.ClientID%>').getValue();
            if (billNo == "") {
                F.alert("请选择要打印的单据信息！");
                return;
            }
            if (billState != "Y" && billState != "G") {
                F.alert("选择单据未审核,不允许打印！");
                return;
            }
            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "/grf/gzthd.grf?timestamp=" + new Date().getTime();
            var dataurl = "/captcha/PrintReport.aspx?Method=GetGZDataBill_Rtn&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
        function btn_Bill() {
            var billNo = F('<%= docBILLNO.ClientID%>').getValue();
            if (billNo == "") {
                F.alert("请选择要打印的单据信息！");
                return;
            }
            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "/grf/cksld.grf";
            var dataurl = "/captcha/PrintReport.aspx?Method=GetData_Jh&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
    </script>
</body>
</html>
