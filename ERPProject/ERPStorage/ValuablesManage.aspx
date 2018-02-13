<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ValuablesManage.aspx.cs" Inherits="ERPProject.ERPStorage.ValuablesManage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>高值备货管理</title>
    <script src="../res/js/CreateControl.js" type="text/javascript"></script>
    <script src="../res/js/GRInstall.js" type="text/javascript"></script>
    <script src="../../res/js/jquery.ymh.js" type="text/javascript"></script>
    <style type="text/css" media="all">
        .no-width {
            width: 0;
        }

        .x-grid-row-summary .x-grid-cell-inner {
            font-weight: bold;
            color: blue;
        }

        .x-grid-row-summary .x-grid-cell {
            background-color: #fff !important;
        }

        .ColBlue {
            font-size: 12px;
            color: blue;
        }

        .x-grid-row.highlight td {
            background-color: lightgreen;
            background-image: none;
        }

        .x-grid-row.Colorhighlight td {
            background-color: lightblue;
            background-image: none;
        }

        .x-grid-row.highyellowlight td {
            background-color: yellow;
            background-image: none;
        }
    </style>
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
                                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                <f:Button ID="bntClear" Icon="Erase" Text="清 除" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btExport" Icon="DatabaseGo" Text="导 出" ConfirmText="是否确认导出高值跟台商品数据?" EnablePostBack="true" runat="server" OnClick="btExport_Click" EnableAjax="false" DisableControlBeforePostBack="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnAllCommit" Icon="UserTick" Text="提 交" EnablePostBack="true" runat="server" ConfirmText="是否确认提交选中单据？" OnClick="btnAllCommit_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />

                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="70px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TextBox ID="lstBILLNO" runat="server" Label="单据编号" />
                                                        <f:DropDownList ID="lstFLAG" runat="server" Label="单据状态" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="lstLRY" runat="server" Label="录入员" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="lstDEPTID" runat="server" Label="订货科室" EnableEdit="true" ForceSelection="true">
                                                            <Listeners>
                                                                <f:Listener Event="beforequery" Handler="onDropDownquery" />
                                                            </Listeners>
                                                        </f:DropDownList>
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow ColumnWidths="50% 25% 25%">
                                                    <Items>
                                                        <f:DropDownList ID="lstPSSID" runat="server" Label="送货商" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="lstLRRQ1" runat="server" Label="录入日期" Required="true" ShowRedStar="true" />
                                                        <f:DatePicker ID="lstLRRQ2" runat="server" Label="　　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" AnchorValue="100% -95" ShowBorder="false" ShowHeader="false" EnableColumnLines="true" OnRowDataBound="GridList_RowDataBound"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableTextSelection="true" EnableCheckBoxSelect="true"
                                    DataKeyNames="SEQNO,FLAG" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick"
                                    EnableHeaderMenu="true" SortField="SEQNO" SortDirection="ASC" AllowSorting="true" EnableMultiSelect="true" CheckBoxSelectOnly="true">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField Hidden="true" DataField="SEQNO" />
                                        <f:BoundField Width="110px" DataField="BILLNO" HeaderText="单据编号" SortField="BILLNO" TextAlign="Center" />
                                        <f:BoundField Width="60px" DataField="FLAG" HeaderText="单据状态" SortField="FLAG" TextAlign="Center" />
                                        <f:BoundField Width="110px" DataField="DDBH" HeaderText="订单编号" SortField="DDBH" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="DEPTID" HeaderText="收货地点" SortField="DEPTID" TextAlign="Center" />
                                        <f:BoundField Width="150px" DataField="SUPNAME" HeaderText="供货商名称" SortField="SUPNAME" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="DHRQ" HeaderText="收货日期" SortField="DHRQ" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="70px" DataField="SUBSUM" HeaderText="收货金额" SortField="SUBSUM" TextAlign="Right" DataFormatString="{0:F2}" />
                                        <f:BoundField Width="50px" DataField="CGY" HeaderText="业务员" SortField="CGY" TextAlign="Center" />
                                        <f:BoundField Width="50px" DataField="LRY" HeaderText="录入员" SortField="LRY" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="LRRQ" HeaderText="录入日期" SortField="LRRQ" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="50px" DataField="SHR" HeaderText="审核员" SortField="SHR" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="SHRQ" HeaderText="审核日期" TextAlign="Center" SortField="SHRQ" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="100px" DataField="MEMO" HeaderText="备注" SortField="MEMO" TextAlign="Center" ExpandUnusedSpace="true" />
                                    </Columns>
                                    <Listeners>
                                        <f:Listener Event="cellkeydown" Handler="onCellClick" />
                                        <f:Listener Event="cellmousedown" Handler="onMouseDown" />
                                    </Listeners>
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
                                                <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="操作信息：单据操作主界面！" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="btnNew" Icon="PageAdd" Text="新 单" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnDel" Icon="PageCancel" Text="整单删除" EnablePostBack="true" runat="server" ConfirmText="是否要删除此单据?" OnClick="btnBill_Click" />
                                                <f:Button ID="btnSave" Icon="Disk" Text="保 存" EnablePostBack="true" ValidateForms="FormDoc" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnCommit" Icon="Disk" Text="提 交" EnablePostBack="true" runat="server" OnClick="btnCommit_Click" ValidateForms="FormDoc" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnAudit" Icon="UserTick" Text="审 核" EnablePostBack="true" ConfirmText="是否确认审核此单据?" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnCancel" Icon="UserCross" Text="驳 回" EnablePostBack="true" runat="server" OnClick="btnBill_Click" Hidden="true" />
                                                <f:Button ID="btnPrint" Icon="Printer" EnablePostBack="false" Text="打印单据" runat="server" OnClientClick="btnPrint_onclick()" />
                                                <f:Button ID="btnCopy" Icon="PageCopy" Text="复 制" EnablePostBack="true" ConfirmText="是否确认复制此单据信息?" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" EnablePostBack="true" ConfirmText="是否确认导出此单据信息?" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnAddRow" Icon="Add" Text="增 行" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnDelRow" Icon="Delete" Text="删 行" EnablePostBack="true" ConfirmText="是否确认删除选中行信息?" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" />
                                                <f:Button ID="btnNext" Icon="ForwardBlue" Text="下 页" EnablePostBack="true" runat="server" Hidden="true" OnClick="btnBill_Click" />
                                                <f:Button ID="btnBef" Icon="RewindBlue" Text="上 页" EnablePostBack="true" runat="server" Hidden="true" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnGoods" Icon="Magnifier" Text="追加商品" EnablePostBack="true" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" />

                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="FormDoc" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="70px" runat="server">
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
                                                        <f:DropDownList ID="docDEPTID" runat="server" Label="订货部门" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true">
                                                            <Listeners>
                                                                <f:Listener Event="beforequery" Handler="onDropDownquery" />
                                                            </Listeners>
                                                        </f:DropDownList>
                                                        <f:DatePicker ID="docDHRQ" runat="server" Label="订货日期" Required="true" ShowRedStar="true" />
                                                        <f:TextBox ID="docBILLNO" runat="server" Label="单据编号" ShowRedStar="true" MaxLength="15" />
                                                        <f:DropDownList ID="docFLAG" runat="server" Label="单据状态" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                    </Items>
                                                </f:FormRow>

                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="docPSSID" runat="server" Label="供货商" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="docCGY" runat="server" Label="操作员" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="docLRY" runat="server" Label="录入员" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="docLRRQ" runat="server" Label="录入日期" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:RadioButtonList ID="rblOPER" runat="server" Label="操作类别">
                                                            <f:RadioItem Selected="true" Text="跟台" Value="GT" />
                                                            <f:RadioItem Text="备货" Value="BH" />
                                                        </f:RadioButtonList>
                                                        <f:TextBox ID="tbxMEMO" runat="server" Label="备注" />
                                                        <f:DropDownList ID="docSHR" runat="server" Label="审核员" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="docSHRQ" runat="server" Label="审核日期" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridCom" AnchorValue="100% -122" ShowBorder="false" ShowHeader="false" EnableColumnLines="true"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;"
                                    DataKeyNames="GDSEQ,MEMO" AllowCellEditing="true" ClicksToEdit="1" EnableAfterEditEvent="true" OnAfterEdit="GridCom_AfterEdit"
                                    EnableSummary="true" SummaryPosition="Bottom">
                                    <Columns>
                                        <f:RowNumberField Width="30px" TextAlign="Center" />
                                        <f:RenderField Width="100px" DataField="GDSEQ" ColumnID="GDSEQ" HeaderText="商品编码" TextAlign="Center" FieldType="String" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="labGDSEQ" runat="server" BoxConfigAlign="Middle">
                                                </f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" DataField="GDNAME" ColumnID="GDNAME" HeaderText="商品名称" FieldType="String">
                                            <Editor>
                                                <f:Label ID="comGDNAME" BoxConfigAlign="Middle" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="150px" DataField="GDSPEC" ColumnID="GDSPEC" HeaderText="商品规格" FieldType="String">
                                            <Editor>
                                                <f:Label ID="comGDSPEC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="60px" DataField="BZSL" ColumnID="BZSL" HeaderText="请领数" TextAlign="Center" FieldType="Auto">
                                            <Editor>
                                                <f:NumberBox ID="comBZSL" CssClass="ColBlue" Required="true" runat="server" MinValue="0" DecimalPrecision="6" MaxValue="99999999" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="60px" ColumnID="UNITNAME" DataField="UNITNAME" FieldType="String" HeaderText="请领单位" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="comUNITNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="60px" DataField="BZHL" ColumnID="BZHL" HeaderText="包装含量" TextAlign="Center" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comBZHL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" DataField="DDSL" ColumnID="DDSL" HeaderText="请领数量(最小单位)" TextAlign="Center" FieldType="Auto">
                                            <Editor>
                                                <f:Label ID="comDDSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" DataField="UNITSMALLNAME" ColumnID="UNITSMALLNAME" HeaderText="最小包装单位" FieldType="Auto" TextAlign="Center" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="labUNITSMALLNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" DataField="HSJJ" ColumnID="HSJJ" HeaderText="含税进价" TextAlign="Right" FieldType="Auto" RendererFunction="renderGender4">
                                            <Editor>
                                                <f:Label ID="Label1" runat="server" />
                                            </Editor>
                                        </f:RenderField>

                                        <f:RenderField Width="80px" DataField="HSJE" ColumnID="HSJE" HeaderText="含税金额" TextAlign="Right" RendererFunction="renderGender">
                                            <Editor>
                                                <f:Label ID="comHSJE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" DataField="PRODUCERNAME" ColumnID="PRODUCERNAME" FieldType="String" HeaderText="生产商">
                                            <Editor>
                                                <f:Label ID="comPRODUCERNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="130px" DataField="PZWH" ColumnID="PZWH" HeaderText="注册证号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="comPZWH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField DataField="JXTAX" ColumnID="JXTAX" HeaderText="税率" TextAlign="Center" Width="0px">
                                            <Editor>
                                                <f:Label ID="comJXTAX" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" DataField="ZPBH" ColumnID="ZPBH" HeaderText="制品编号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="comZPBH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" DataField="MEMO" ColumnID="MEMO" HeaderText="备注" TextAlign="Center">
                                            <Editor>
                                                <f:TextBox ID="comMEMO" runat="server" MaxLength="80" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" DataField="BARCODE" ColumnID="BARCODE" HeaderText="商品条码" TextAlign="Center"
                                            FieldType="String" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comBARCODE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="UNIT" DataField="UNIT" FieldType="String" HeaderText="包装单位编码" TextAlign="Center"
                                            EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comUNIT" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" DataField="ISLOT" ColumnID="ISLOT" HeaderText="批号管理" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comISLOT" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" DataField="ISGZ" ColumnID="ISGZ" HeaderText="是否贵重" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comISGZ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" DataField="CODEINFO" ColumnID="CODEINFO" HeaderText="商品赋码信息" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comCODEINFO" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" DataField="PRODUCER" ColumnID="PRODUCER" FieldType="String" HeaderText="生产商编码"
                                            EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comPRODUCER" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" DataField="SUPID" ColumnID="SUPID" HeaderText="供应商编码" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comSUPID" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                    </Columns>
                                    <Listeners>
                                        <f:Listener Event="cellclick" Handler="onEditorCellClick" />
                                    </Listeners>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                        <f:HiddenField ID="ColorForGridGoods" runat="server"></f:HiddenField>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
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
        <f:Window ID="Window1" Title="商品信息" Hidden="true" EnableIFrame="true" runat="server"
            EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True"
            Width="820px" Height="480px" OnClose="Window1_Close">
        </f:Window>
        <f:Window ID="WindowReject" Title="驳回信息" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="370px" Height="200px">
            <Items>
                <f:Form ID="Form2" ShowBorder="false" AutoScroll="false" BodyPadding="5px 10px"
                    ShowHeader="False" LabelWidth="70px" runat="server">
                    <Rows>
                        <f:FormRow Hidden="true">
                            <Items>
                                <f:DropDownList ID="ddlReject" runat="server" Label="驳回原因" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow Hidden="true">
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
                        <f:Button ID="btnRejectSubmit" Text="确定" Icon="SystemSave" runat="server" OnClick="btnRejectSubmit_Click">
                        </f:Button>
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
            if (billNo == "") {
                F.alert("请选择要打印的单据信息！");
                return;
            }
            if (billState != "Y") {
                F.alert("选择单据未审核,不允许打印！");
                return;
            }
            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "/grf/HighGoodsStorage.grf";
            var dataurl = "/captcha/PrintReport.aspx?Method=GoodsYRK&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();

        }

        var arrEditor = [4, 15];//Gird中可编辑的表格
        function onEditorCellClick(grid, rowIndex, columnIndex, e) {
            if (F('<%= docFLAG.ClientID%>').getValue() == "M" && F('<%= hfdOper.ClientID%>').getValue() == "input") {
                if (arrEditor.indexOf(columnIndex) >= 0) {
                    return true;
                }
                else {
                    return false;
                }
            }
            else
                return false;
        }
        var highlightRowsClientID = '<%= highlightRows.ClientID %>';
        var gridClientID = '<%= GridList.ClientID %>';
        function highlightRows() {
            // 增加延迟，等待HiddenField更新完毕
            window.setTimeout(function () {
                var highlightRows = F(highlightRowsClientID);
                var grid = F(gridClientID);
                $(grid.el.dom).find('.x-grid-row.highlight').removeClass('highlight');
                $.each(highlightRows.getValue().split(','), function (index, item) {
                    if (item !== '') {
                        var row = grid.getView().getNode(parseInt(item, 10));
                        $(row).addClass('highlight');
                    }
                });
            }, 100);
        }
        _eleSelected = false;
        // 页面第一个加载完毕后执行的函数
        F.ready(function () {
            var redstarID = "";
            $redstarID = $('#BZSL');
            $redstarID.addClass("redstar");
            var grid = F(gridClientID);
            grid.on('columnhide', function () {
                highlightRows();
                highlightRowsForYellow();
            });
            grid.on('columnshow', function () {
                highlightRows();
                highlightRowsForYellow();
            });
            grid.getStore().on('refresh', function () {
                highlightRows();
                highlightRowsForYellow();
            });
            highlightRows();
            highlightRowsForYellow();
        });

        function removeAll(grid) {
            //Ext.getCmp(grid).getStore().suspendEvents(true);
            Ext.getCmp(grid).getStore().removeAll();
        }


        var highlightForGoodsGrid = '<%= ColorForGridGoods.ClientID %>';
        var gridClientForGoodsGrid = '<%= GridCom.ClientID %>';
        function highlightRowsForGoodsGrid() {
            // 增加延迟，等待HiddenField更新完毕
            window.setTimeout(function () {
                var highlightRows = F(highlightForGoodsGrid);
                var grid = F(gridClientForGoodsGrid);
                $(grid.el.dom).find('.x-grid-row.Colorhighlight').removeClass('Colorhighlight');
                $.each(highlightRows.getValue().split(','), function (index, item) {
                    if (item !== '') {
                        var row = grid.getView().getNode(parseInt(item, 10));
                        $(row).addClass('Colorhighlight');
                    }
                });
            }, 100);
        }

        var highyellowlightRowsClientID = '<%= highlightRowYellow.ClientID %>';
        function highlightRowsForYellow() {
            // 增加延迟，等待HiddenField更新完毕
            window.setTimeout(function () {
                var highlightRows = F(highyellowlightRowsClientID);
                var grid = F(gridClientID);
                $(grid.el.dom).find('.x-grid-row.highyellowlight').removeClass('highyellowlight');
                $.each(highlightRows.getValue().split(','), function (index, item) {
                    if (item !== '') {
                        var row = grid.getView().getNode(parseInt(item, 10));
                        $(row).addClass('highyellowlight');
                    }
                });
            }, 100);
        }
    </script>
</body>
</html>
