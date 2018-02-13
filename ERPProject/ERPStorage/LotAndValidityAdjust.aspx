<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LotAndValidityAdjust.aspx.cs" Inherits="ERPProject.ERPStorage.LotAndValidityAdjust" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>科室商品批号效期调整</title>
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
        <f:PageManager ID="PageManager1" EnableAjaxLoading="false" AutoSizePanelID="TabStrip1" runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right" EnableTabCloseMenu="false" ActiveTabIndex="1" runat="server">
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
                                                <f:ToolbarText ID="ToolbarText2" Text="操作信息：双击打开单据明细！" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                <f:Button ID="bntClear" Icon="Erase" Text="清 空" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnAllCommit" Icon="UserTick" Text="提 交" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否确认提交选中单据？" OnClick="btnAllCommit_Click" />
                                                <f:ToolbarSeparator runat="server" ID="ListLine2" />
                                                <f:Button ID="btnAuditBatch" Icon="UserTick" Text="审 核" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否确认审核选中单据？" OnClick="btnAuditBatch_Click" />
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
                                                        <f:TriggerBox ID="lstBILLNO" runat="server" Label="单据编号" TriggerIcon="Search" MaxLength="15" OnTriggerClick="lstBILLNO_TriggerClick"></f:TriggerBox>
                                                        <f:DropDownList ID="lstFLAG" runat="server" Label="单据状态" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="lstLRY" runat="server" Label="录入员" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="lstDEPTID" runat="server" Label="收货地点" EnableEdit="true" ForceSelection="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="lstCGY" runat="server" Label="采购员" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="lstPSSID" runat="server" Label="送货商" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="lstLRRQ1" runat="server" Label="录入日期" Required="true" ShowRedStar="true" />
                                                        <f:DatePicker ID="lstLRRQ2" runat="server" Label="　　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" AnchorValue="100% -113" ShowBorder="false" ShowHeader="false"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" OnRowDataBound="GridList_RowDataBound"
                                    DataKeyNames="SEQNO,FLAG,DEPTID,LRY" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick" EnableColumnLines="true"
                                    EnableMultiSelect="false" EnableCheckBoxSelect="true" KeepCurrentSelection="true"
                                    EnableHeaderMenu="true" SortField="SEQNO" OnSort="GridList_Sort" SortDirection="ASC" AllowSorting="true">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField Width="70px" DataField="SEQNO" SortField="SEQNO" Hidden="true" />
                                        <f:BoundField Width="120px" DataField="BILLNO" HeaderText="单据编号" SortField="BILLNO" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="FLAG" HeaderText="单据状态" SortField="FLAG" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="90px" DataField="FLAG_CN" ColumnID="FLAG_CN" HeaderText="单据状态" SortField="FLAGNAME" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="ISSENDNAME" HeaderText="传输状态" SortField="ISSENDNAME" TextAlign="Center" />
                                        <f:BoundField Width="110px" DataField="DEPTID" HeaderText="收货地点" SortField="DEPTID" TextAlign="Left" />
                                        <f:BoundField Width="150px" DataField="SUPNAME" HeaderText="送货商名称" SortField="SUPNAME" TextAlign="Left" />
                                        <f:BoundField Width="90px" DataField="TPRQ" HeaderText="调整日期" SortField="XDRQ" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="70px" DataField="SUBNUM" HeaderText="条目数" SortField="SUBNUM" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="SUBSUM" HeaderText="调整金额" SortField="SUBSUM" TextAlign="Right" DataFormatString="{0:F2}" />
                                        <f:BoundField Width="90px" DataField="LRY" HeaderText="录入员" SortField="LRY" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="LRRQ" HeaderText="录入日期" SortField="LRRQ" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="90px" DataField="SHR" HeaderText="审核员" SortField="SHR" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="SHRQ" HeaderText="审核日期" TextAlign="Center" SortField="SHRQ" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField MinWidth="600px" DataField="MEMO" HeaderText="备注" ExpandUnusedSpace="true" />
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


                                                <f:Button ID="btnNew" Icon="PageAdd" Text="新 单" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnSave" Icon="Disk" Text="保 存" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" />
                                                <f:Button ID="btnCommit" Icon="UserTick" Text="提 交" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认提交此单据?" runat="server" OnClick="btnCommit_Click" ValidateForms="FormDoc" />
                                                <f:Button ID="btnDel" Icon="PageCancel" Text="整单删除" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认删除此单据?" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnDelRow" Icon="Delete" Text="删 行" EnableDefaultState="false" ConfirmText="是否确认删除选中行？" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" ID="WebLine3" />
                                                <f:Button ID="btnAudit" Icon="UserTick" Text="审 核" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认审核此单据?" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnCancel" Icon="UserCross" Text="驳 回" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" ID="WebLine4" />
                                                <f:Button ID="btnPrint" Icon="Printer" Text="打 印" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClientClick="btnPrint_onclick()" />
                                                <f:Button ID="btnExport" Icon="PageExcel" Text="导 出" EnableDefaultState="false" ConfirmText="是否确定已经保存数据并导出数据?" EnableAjax="false" DisableControlBeforePostBack="false" runat="server" OnClick="btnBill_Click" />

                                                <%--<f:Button ID="btntext" Text="test" runat="server" OnClientClick="zd()"></f:Button>--%>
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
                                                        <f:DropDownList ID="docISSEND" runat="server" Label="发送状态" Enabled="false" EnableEdit="true" ForceSelection="true" Hidden="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="docDEPTID" runat="server" Label="调整部门" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="docTPRQ" runat="server" Label="调整日期" ShowRedStar="true" Required="true" />
                                                        <f:TextBox ID="docBILLNO" runat="server" Label="单据编号" MaxLength="15" EmptyText="系统自动生成" />
                                                        <f:DropDownList ID="docFLAG" runat="server" Label="单据状态" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow ColumnWidths="25% 25% 25% 25%">
                                                    <Items>
                                                        <f:DropDownList ID="docDHLX" runat="server" Label="订货类型" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true">
                                                        </f:DropDownList>
                                                        <f:DropDownList ID="docPSSID" runat="server" Label="送货商" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="docSHR" runat="server" Label="审核员" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="docSHRQ" runat="server" Label="审核日期" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow ColumnWidths="50% 25% 25%">
                                                    <Items>

                                                        <f:TextBox ID="docMEMO" runat="server" Label="备注" MaxLength="80" EmptyText="备注信息" />
                                                        <f:DropDownList ID="docLRY" runat="server" Label="录入员" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="docLRRQ" runat="server" Label="录入日期" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList Hidden="true" ID="docCGY" runat="server" Label="操作员" Required="true" ShowRedStar="true" MaxLength="8" EnableEdit="true" ForceSelection="true" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridGoods" AnchorValue="100% -145" ShowBorder="false" ShowHeader="false" EnableSummary="true" SummaryPosition="Bottom"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableColumnLines="true" AllowColumnLocking="true"
                                    DataKeyNames="GDSEQ" AllowCellEditing="true" ClicksToEdit="1" EnableAfterEditEvent="true" OnAfterEdit="GridGoods_AfterEdit">
                                    <Columns>

                                        <f:RenderField Width="35px" TextAlign="Center" EnableLock="true" Locked="true" ColumnID="ROWNO" DataField="ROWNO" FieldType="String" EnableHeaderMenu="false">
                                            <Editor>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="105px" DataField="GDSEQ" ColumnID="GDSEQ" HeaderText="商品编码" FieldType="String" EnableHeaderMenu="false" EnableLock="true" Locked="true">
                                            <Editor>
                                                <f:Label ID="labGDSEQ" runat="server" BoxConfigAlign="Middle">
                                                </f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" DataField="GDNAME" ColumnID="GDNAME" HeaderText="商品名称" FieldType="String" EnableHeaderMenu="false" EnableLock="true" Locked="true">
                                            <Editor>
                                                <f:Label ID="comGDNAME" BoxConfigAlign="Middle" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" DataField="GDSPEC" ColumnID="GDSPEC" HeaderText="商品规格" FieldType="String" EnableHeaderMenu="false" EnableLock="true" Locked="true">
                                            <Editor>
                                                <f:Label ID="comGDSPEC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="KCSL" DataField="KCSL" FieldType="Int" EnableHeaderMenu="false"
                                            HeaderText="库存数量" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="comKCSL"  runat="server" ></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="SL" DataField="SL" FieldType="Int" EnableHeaderMenu="false"
                                            HeaderText="调整数量" TextAlign="Center">
                                            <Editor>
                                                <f:NumberBox ID="nbxSL" runat="server" Required="true" MinValue="0" MaxValue="99999999" DecimalPrecision="0" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" DataField="PH" ColumnID="PH" HeaderText="批号" TextAlign="Center" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comPH" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" DataField="NEWPH" ColumnID="NEWPH" HeaderText="新批号" TextAlign="Center" EnableHeaderMenu="false" EnableColumnHide="false">
                                            <Editor>
                                                <f:TextBox ID="comNewPH" Required="true" runat="server" MaxLength="20" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="YXQZ" DataField="YXQZ" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="有效期至" TextAlign="Center" Renderer="Date" RendererArgument="yyyy-MM-dd" NullDisplayText="">
                                            <Editor>
                                                <f:Label ID="comYXQZ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="NEWYXQZ" DataField="NEWYXQZ" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="新效期至" TextAlign="Center" Renderer="Date" RendererArgument="yyyy-MM-dd">
                                            <Editor>
                                                <f:DatePicker ID="comNewYXQZ" runat="server" DateFormatString="yyyy-MM-dd" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="UNITNAME" DataField="UNITNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="订货单位" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorUNITNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="UNIT" DataField="UNIT" FieldType="String" EnableHeaderMenu="false" Hidden="true"
                                            HeaderText="订货单位" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="Label1" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" DataField="RQ_SC" ColumnID="RQ_SC" HeaderText="生产日期" TextAlign="Center" EnableHeaderMenu="false" EnableColumnHide="false"
                                            Renderer="Date" RendererArgument="yyyy-MM-dd" NullDisplayText="">
                                            <Editor>
                                                <f:Label ID="comRQ_SC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" DataField="BZHL" ColumnID="BZHL" HeaderText="包装含量" TextAlign="Center" EnableHeaderMenu="false" EnableColumnHide="false">
                                            <Editor>
                                                <f:Label ID="comBZHL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" DataField="HSJJ" ColumnID="HSJJ" HeaderText="含税进价" FieldType="Auto" TextAlign="Center" EnableHeaderMenu="false" RendererFunction="round4">
                                            <Editor>
                                                <f:Label ID="lblHSJJ" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" DataField="HSJE" ColumnID="HSJE" HeaderText="含税金额" TextAlign="Right" EnableHeaderMenu="false" RendererFunction="round2">
                                            <Editor>
                                                <f:Label ID="lblHSJE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" DataField="ZPBH" ColumnID="ZPBH" HeaderText="制品编号" TextAlign="Center" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:TextBox ID="comZPBH" Required="true" MaxLength="15" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="200px" DataField="PZWH" ColumnID="PZWH" HeaderText="注册证号" TextAlign="Center" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comPZWH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="200px" ColumnID="PRODUCERNAME" DataField="PRODUCERNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="生产厂家">
                                            <Editor>
                                                <f:Label ID="lblEditorPRODUCERNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="HWID" DataField="HWID" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="货位">
                                            <Editor>
                                                <f:Label ID="lblHWID" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="200px" ColumnID="PRODUCER" Hidden="true" DataField="PRODUCER" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="生产厂家">
                                            <Editor>
                                                <f:Label ID="lblEditorPRODUCER" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="150px" DataField="MEMO" ColumnID="MEMO" HeaderText="备注" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:TextBox ID="comMEMO" runat="server" MaxLength="80" />
                                            </Editor>
                                        </f:RenderField>


                                        <f:RenderField Width="0px" Hidden="true" ColumnID="SUPNAME" DataField="SUPNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="供应商">
                                            <Editor>
                                                <f:Label ID="lblSUPNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" DataField="NUM2" ColumnID="NUM2" HeaderText="供货周期" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:NumberBox ID="comNUM2" runat="server" NoDecimal="true" MinValue="1" MaxLength="2"></f:NumberBox>
                                            </Editor>
                                        </f:RenderField>
                                    </Columns>

                                </f:Grid>
                            </Items>
                        </f:Panel>
                        <f:HiddenField ID="ColorForGridGoods" runat="server"></f:HiddenField>
                        <f:HiddenField ID="hdfGL" runat="server"></f:HiddenField>
                        <f:HiddenField ID="hdfBH" runat="server"></f:HiddenField>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
        <f:HiddenField ID="hfdValue" runat="server" />
        <f:HiddenField ID="highlightRows" runat="server"></f:HiddenField>
        <f:HiddenField ID="highlightRowYellow" runat="server"></f:HiddenField>
        <f:HiddenField ID="highlightRowRed" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdOper" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdFid" runat="server"></f:HiddenField>
        <f:HiddenField ID="hdftest" runat="server" />
        <f:Window ID="Window1" Title="商品信息" Hidden="true" EnableIFrame="true" runat="server"
            EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="false" Width="820px" Height="480px" OnClose="Window1_Close">
        </f:Window>
       
        <f:Window ID="WindowLot" Title="商品批号信息" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="610px" Height="360px">
            <Items>
                <f:Grid ID="GridLot" ShowBorder="false" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server"
                    DataKeyNames="GDSEQ,PH,YXQZ,RQ_SC,PZWH" EnableMultiSelect="true" EnableCheckBoxSelect="true">
                    <Columns>
                        <f:BoundField Width="70px" DataField="GDSEQ" HeaderText="商品编码" />
                        <f:BoundField Width="120px" DataField="GDNAME" HeaderText="商品名称" />
                        <f:BoundField Width="120px" DataField="GDSPEC" HeaderText="商品规格" Hidden="true" />
                        <f:BoundField Width="70px" DataField="UNITNAME" HeaderText="单位" />
                        <f:BoundField Width="120px" DataField="UNIT" HeaderText="单位" Hidden="true" />
                        <f:BoundField Width="100px" DataField="PH" HeaderText="批号" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="YXQZ" HeaderText="效期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="100px" DataField="RQ_SC" HeaderText="生产日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="120px" DataField="PZWH" HeaderText="注册证号" TextAlign="Center" />
                        <f:BoundField Width="120px" DataField="KCSL" HeaderText="库存数量" TextAlign="Center" />
                    </Columns>
                </f:Grid>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar3" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:HiddenField ID="hfdRowIndex" runat="server" />
                        <f:Button ID="btnClosePostBack" Text="确定" EnableDefaultState="false" Icon="SystemSave" runat="server" OnClick="btnClosePostBack_Click">
                        </f:Button>
                        <f:Button ID="btnClose" Text="关闭" EnableDefaultState="false" Icon="SystemClose" runat="server">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
    </form>
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
                            <f:TextArea ID="txaMemo" runat="server" Label="详细说明" Height="80px" MaxLength="80" />
                        </Items>
                    </f:FormRow>
                </Rows>
            </f:Form>
        </Items>
        <Toolbars>
            <f:Toolbar ID="Toolbar6" runat="server" Position="Bottom" ToolbarAlign="Right">
                <Items>
                    <f:Button ID="btnRejectSubmit" Text="确定" EnableDefaultState="false" Icon="SystemSave" runat="server" OnClick="btnRejectSubmit_Click">
                    </f:Button>
                </Items>
            </f:Toolbar>
        </Toolbars>
    </f:Window>




    <script type="text/javascript">
        function PosGoodsQt(Info) {
            F('<%= hfdValue.ClientID%>').setValue(Info);
            F.customEvent('GoodsAdd');
        }
        function btnPrint_BQ() {
            var billNo = F('<%= docSEQNO.ClientID%>').getValue();
            var billState = F('<%= docFLAG.ClientID%>').getValue();
            if (billNo == "") {
                F.alert("请选择要打印的单据信息！");
                return;
            }
            if (billState != "Y") {
                F.alert("选择单据未审核或已被结算,不允许打印！");
                return;
            }
            ReportViewer.ReportURL = "/grf/onecode_GZ.grf";
            var dataurl = "/captcha/PrintReport.aspx?Method=GoodsOrderGz&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
        function btnPrint_onclick() {
            var billNo = F('<%= docSEQNO.ClientID%>').getValue();
            var billState = F('<%= docFLAG.ClientID%>').getValue();
            if (billNo == "") {
                F.alert("请选择要打印的单据信息！");
                return;
            }
            if (billState != "Y") {
                F.alert("选择单据未审核或已被结算,不允许打印！");
                return;
            }
            ReportViewer.ReportURL = "<%=GoodOrder%>?time=" + new Date().getTime();
            var dataurl = "/captcha/PrintReport.aspx?Method=GetOrderData&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }

        function onGridBeforeEdit(event, value, params) {
            var flag = F('<%= docFLAG.ClientID%>').getValue();
            var opervalue = F('<%= hfdOper.ClientID%>').getValue();
            if ((",M,R,N").indexOf(flag) > 0) {
                if (flag == "N" && opervalue == "input")
                    return false;
                else {
                    true
                }
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
                me.updateCellValue(rowId, 'DHS', BZSL * BZHL);
                me.updateCellValue(rowId, 'HSJE', BZSL * HSJJ);
                var BZSLTotal = 0, HSJETotal = 0;
                me.getRowEls().each(function (index, tr) {
                    BZSLTotal += me.getCellValue(tr, 'BZSL');
                    HSJETotal += BZSL * HSJJ;
                });
                me.updateSummaryCellValue('GDNAME', "本页合计", true);
                me.updateSummaryCellValue('BZSL', BZSLTotal, true);
                me.updateSummaryCellValue('HSJE', HSJETotal, true);
            }
        }
    </script>
</body>
</html>
