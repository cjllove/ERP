﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DepartmentApply.aspx.cs" Inherits="ERPProject.ERPApply.DepartmentApply" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>科室药品申领</title>
    <script src="../res/js/CreateControl.js" type="text/javascript"></script>
    <script src="../res/js/GRInstall.js" type="text/javascript"></script>
    <script src="../res/js/jquery-1.11.1.min.js" type="text/javascript"></script>
    <style type="text/css">
        .f-grid-row[data-color=color1],
        .f-grid-row[data-color=color1] .ui-icon,
        .f-grid-row[data-color=color1] a {
            background-color: #3AA02C;
            color: #fff;
        }

        .f-grid-row[data-color=color3],
        .f-grid-row[data-color=color3] .ui-icon,
        .f-grid-row[data-color=color3] a {
            background-color: #AF5553;
            color: #fff;
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
        <f:PageManager ID="PageManager1" AutoSizePanelID="TabStrip1" runat="server" OnCustomEvent="PageManager1_CustomEvent" AjaxLoadingType="Mask" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="1" runat="server">
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
                                                <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="操作信息：双击打开单据明细" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                <f:Button ID="bntClear" Icon="Erase" Text="清 空" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnExp" runat="server" CssStyle="margin-left: 10px;" Icon="PageExcel" EnableAjax="false" EnableDefaultState="false"
                                                    OnClick="btExport_Click" EnablePostBack="true" Text="导出全部" ConfirmText="是否确认导出此科室申领信息?" DisableControlBeforePostBack="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnAuditBatch" Icon="UserTick" EnableDefaultState="false" Text="审 批" ConfirmText="确认审批选中单据？" EnablePostBack="true" runat="server" OnClick="btnAuditBatch_Click" />
                                               <f:Button ID="btnAllReConfirm"  Text="批量收货确认" Icon="BulletTick" EnablePostBack="true" runat="server" EnableDefaultState="false" OnClick="btnAllReConfirm_Click" Hidden="true" ></f:Button>
                                                 <f:Button ID="bntSearch" Icon="Magnifier" EnableDefaultState="false" Text="查 询" EnablePostBack="true" runat="server" OnClick="btnBill_Click" ValidateForms="Formlist" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TriggerBox ID="tgbBILLNO" runat="server" Label="单据编号" EmptyText="输入单据编号查询" OnTriggerClick="tgbBILLNO_TriggerClick" MaxLength="15"></f:TriggerBox>
                                                        <f:DropDownList ID="lstDEPTID" runat="server" Label="申领科室" EnableEdit="true" ForceSelection="true">
                                                        </f:DropDownList>
                                                        <f:DropDownList ID="lstSLR" runat="server" Label="申领人" EnableEdit="true" ForceSelection="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="lstFLAG" runat="server" Label="单据状态" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="lstDEPTOUT" runat="server" Label="出库部门" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="lstLRY" runat="server" Label="录入员" Required="true" EnableEdit="true" ForceSelection="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TriggerBox ID="tgbGDSEQ" runat="server" Label="商品信息" EmptyText="输入商品信息查询" OnTriggerClick="tgbBILLNO_TriggerClick"></f:TriggerBox>
                                                        <f:DatePicker ID="lstLRRQ1" runat="server" Label="申领日期" Required="true" ShowRedStar="true" />
                                                        <f:DatePicker ID="lstLRRQ2" runat="server" Label="　　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                                                    </Items>
                                                </f:FormRow>

                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" AnchorValue="100% -138" ShowBorder="false" ShowHeader="false" EnableCheckBoxSelect="true" EnableSummary="true" SummaryPosition="Bottom"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" AllowPaging="true" IsDatabasePaging="true" OnPageIndexChange="GridList_PageIndexChange"
                                    DataKeyNames="SEQNO,FLAG" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick" EnableTextSelection="true"
                                    OnRowDataBound="GridList_RowDataBound" EnableColumnLines="true" EnableMultiSelect="true"
                                    AllowSorting="true" EnableHeaderMenu="true" SortField="FLAG,BILLNO" OnSort="GridList_Sort" SortDirection="ASC" KeepCurrentSelection="true">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField DataField="SEQNO" Hidden="true" />
                                        <f:BoundField Width="115px" DataField="BILLNO" HeaderText="单据编号" SortField="BILLNO" TextAlign="Center" />
                                        <f:BoundField DataField="FLAG" SortField="FLAG" Hidden="true" />
                                        <f:BoundField Width="80px" DataField="FLAG_CN" ColumnID="FLAG_CN" HeaderText="单据状态" SortField="FLAG_CN" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="ISSH" ColumnID="ISSH" HeaderText="是否收货确认" SortField="ISSH" Hidden="true" TextAlign="Center"></f:BoundField>
                                        <f:BoundField DataField="DEPTOUT" HeaderText="出库部门" SortField="DEPTOUT" Width="120px" TextAlign="Center" />
                                        <f:BoundField Width="120px" DataField="DEPTID" HeaderText="申领科室" SortField="DEPTID" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="XSRQ" HeaderText="申领日期" ColumnID="XSRQ" TextAlign="Center" SortField="XSRQ" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="80px" DataField="CATID" HeaderText="商品种类" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="90px" DataField="SUBSUM" HeaderText="订货金额" ColumnID="SUBSUM" SortField="SUBSUM" TextAlign="Right" DataFormatString="{0:F4}" />
                                        <f:BoundField Width="70px" DataField="SUBNUM" HeaderText="明细条数" ColumnID="SUBNUM" TextAlign="Center" SortField="SUBNUM" />
                                        <f:BoundField Width="80px" DataField="SLR" HeaderText="申领人" TextAlign="Center" SortField="SLR" />
                                        <f:BoundField Width="80px" DataField="LRY" HeaderText="录入员" TextAlign="Center" SortField="LRY" />
                                        <f:BoundField Width="90px" DataField="LRRQ" HeaderText="录入日期" TextAlign="Center" SortField="LRRQ" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="80px" DataField="SHR" HeaderText="审核员" TextAlign="Center" SortField="SHR" />
                                        <f:BoundField Width="90px" DataField="SHRQ" HeaderText="审核日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="SHRQ" />
                                        <f:BoundField Width="115px" DataField="STR2" HeaderText="出库单号" SortField="STR2" />
                                        <f:BoundField DataField="ISSH_FLAG" Hidden="true"></f:BoundField>
                                        <f:BoundField MinWidth="120px" ExpandUnusedSpace="true" DataField="MEMO" HeaderText="备注" SortField="MEMO" />
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
                                                <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />

                                                <f:Button ID="btnTemplate" EnablePostBack="false" Icon="BookOpen" EnableDefaultState="false" runat="server" Text="模板">
                                                    <Menu ID="Menu1" runat="server">
                                                        <f:MenuButton ID="btnSaveTemplate" Icon="TableSave" EnablePostBack="true" runat="server" OnClick="btnSaveTemplate_Click" Text="保存模板">
                                                        </f:MenuButton>
                                                        <f:MenuButton ID="btnLoadTemplate" Icon="TableAdd" EnablePostBack="true" runat="server" OnClick="btnLoadTemplate_Click" Text="载入模板">
                                                        </f:MenuButton>
                                                        <f:MenuButton ID="btnAuto" Icon="BasketEdit" EnablePostBack="true" runat="server" OnClick="btnAuto_Click" Text="自动生成">
                                                        </f:MenuButton>
                                                    </Menu>
                                                </f:Button>
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnNew" Icon="PageAdd" Text="新 单" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnSave" Icon="Disk" EnableDefaultState="false" Text="保 存" EnablePostBack="true" runat="server" ValidateForms="FormDoc" OnClick="btnBill_Click" />
                                                <f:Button ID="btnSubmit" Icon="UserTick" Text="提 交" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否确认提交此单据?" OnClick="btnSubmit_Click" Enabled="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnCopy" Icon="PageCopy" Text="复 制" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认复制此单据信息?" runat="server" OnClick="btnBill_Click" Enabled="false" />
                                                <f:Button ID="btnDel" Icon="PageCancel" EnableDefaultState="false" Text="整单删除" EnablePostBack="true" ConfirmText="是否确认删除单据?" runat="server" OnClick="btnBill_Click" Enabled="false" />
                                                <f:Button ID="btnDelRow" Icon="Delete" Text="删 行" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否确认删除选中行?" OnClick="btnBill_Click" />

                                                <f:ToolbarSeparator runat="server" ID="Line1" />

                                                <f:Button ID="btnAudit" Icon="UserTick" Text="审 批" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认审核此单据?" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnCancel" Icon="UserCross" Text="驳 回" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnPrint" Icon="Printer" Text="打 印" EnableDefaultState="false" EnablePostBack="false" runat="server" OnClientClick="Print_Click()" />
                                              <f:Button ID="btnReConfirm" Text="收货确认" Icon="BulletTick" EnableDefaultState="false" EnablePostBack="true" runat="server" Hidden="true" OnClick="btnReConfirm_Click"></f:Button>
                                                  <f:Button ID="btnGoods" Icon="Magnifier" Text="追加商品" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                </f:Panel>
                                <f:Panel ID="PanelBody" BodyPadding="0px" RegionSplit="true" EnableCollapse="true" RegionPosition="Top" ShowBorder="false"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Items>
                                        <f:Form ID="FormDoc" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow Hidden="true">
                                                    <Items>
                                                        <f:TextBox ID="docBILLNO" runat="server" Label="单据编号" Hidden="true" />
                                                        <f:DropDownList ID="ddlCATID" Hidden="true" runat="server" Label="申领类别" ShowRedStar="true" Required="true" Enabled="false">
                                                        </f:DropDownList>
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="docDEPTOUT" runat="server" Label="出库部门" ShowRedStar="true" Required="true" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="docSLR" runat="server" Label="申领人" EnableEdit="true" ForceSelection="true" />
                                                        <f:TextBox ID="docSEQNO" runat="server" Label="单据编号" EmptyText="系统自动生成" MaxLength="20" />
                                                        <f:DropDownList ID="docFLAG" runat="server" Label="单据状态" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="docDEPTID" runat="server" Label="申领科室" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" AutoPostBack="true" OnSelectedIndexChanged="docDEPTID_SelectedIndexChanged">
                                                        </f:DropDownList>
                                                        <f:DatePicker ID="docXSRQ" runat="server" Label="申领日期" Required="true" ShowRedStar="true" />
                                                        <f:DropDownList ID="docLRY" runat="server" Label="录入员" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="docLRRQ" runat="server" Label="录入日期" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow ColumnWidths="50% 25% 25%">

                                                    <Items>
                                                        <f:RadioButtonList ID="rblBILLTYPE" runat="server" Label="操作类别">
                                                            <f:RadioItem Value="LYD" Text="物资请领" Selected="true" />
                                                            <f:RadioItem Value="GBD" Text="高值备货" />
                                                        </f:RadioButtonList>
                                                         <%-- <f:RadioButtonList ID="rblISSH" runat="server" Label="是否收货确认" LabelWidth="100px" Enabled="false">
                                                            <f:RadioItem Value="Y" Text="是"  />
                                                            <f:RadioItem Value="N" Text="否"  Selected="true"/>
                                                        </f:RadioButtonList>--%>
                                                       <%-- <f:Label runat="server" ID="getscreen" Label="" Text=""></f:Label>--%>

                                                        <f:DropDownList ID="docSHR" runat="server" Label="审核人" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="docSHRQ" runat="server" Label="审核日期" Enabled="false" />

                                                    </Items>
                                                </f:FormRow>

                                                <f:FormRow ColumnWidths="50% 50%">

                                                    <Items>
                                                        <f:TextBox ID="docBuget" runat="server" Label="预算与执行" LabelWidth="100" CssStyle="border:0px;" Readonly="true"></f:TextBox>
                                                        
                                                        <f:TextBox ID="docMEMO" runat="server" Label="备注说明" MaxLength="80" EmptyText="备注信息" />

                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridGoods" AnchorValue="100% -115" ShowBorder="false" ShowHeader="false" EnableSummary="true" SummaryPosition="Bottom"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableColumnLines="true" AllowColumnLocking="true"
                                    DataKeyNames="GDSEQ,QHSL" AllowCellEditing="true" ClicksToEdit="1" EnableAfterEditEvent="true">
                                    <Columns>
                                        <%--<f:RowNumberField Width="35px" TextAlign="Center"></f:RowNumberField>--%>
                                        <f:RenderField Width="35px" EnableLock="true" TextAlign="Center" Locked="true" ColumnID="ROWNO" DataField="ROWNO" FieldType="String" EnableHeaderMenu="false">
                                            <Editor>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="GDSEQ" EnableLock="true" Locked="true" DataField="GDSEQ" FieldType="String"
                                            HeaderText="商品编码" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="lblEditorGDSEQ" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" ColumnID="GDNAME" DataField="GDNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="商品名称" EnableLock="true" Locked="true">
                                            <Editor>
                                                <f:Label ID="lblEditorName" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" EnableLock="true" Locked="true" ColumnID="GDSPEC" DataField="GDSPEC" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="商品规格">
                                            <Editor>
                                                <f:Label ID="lblEditorGDSPEC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" EnableLock="true" ColumnID="UNITNAME" DataField="UNITNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="申领单位" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorUNITNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" EnableLock="true" ColumnID="BZHL" DataField="BZHL" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="包装含量" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorBZHL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="BZSL" EnableLock="true" DataField="BZSL" FieldType="Int" EnableHeaderMenu="false"
                                            HeaderText="申领数<font color='red'>*</font>" TextAlign="Center">
                                            <Editor>
                                                <f:NumberBox ID="nbxBZSL" runat="server" Required="true" MinValue="0" MaxValue="99999999" DecimalPrecision="2" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="CKKCSL" DataField="CKKCSL" FieldType="Auto" EnableHeaderMenu="false" ID="lineCKKCSL"
                                            HeaderText="当前库存" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblKCSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="QHSL" HtmlEncode="true" DataField="QHSL" FieldType="Auto" EnableHeaderMenu="false" ID="lineQHSL" Hidden="true"
                                            HeaderText="缺货数量" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblQHSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="ZDKC" DataField="ZDKC" FieldType="Auto" EnableHeaderMenu="false" Hidden="true"
                                            HeaderText="最低库存" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="Label1" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="ZGKC" DataField="ZGKC" FieldType="Auto" EnableHeaderMenu="false" Hidden="true"
                                            HeaderText="最高库存" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="Label3" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" EnableLock="true" ColumnID="HSJJ" DataField="HSJJ" EnableHeaderMenu="false" RendererFunction="round4"
                                            HeaderText="含税进价" TextAlign="Right" FieldType="String">
                                            <Editor>
                                                <f:Label ID="lblHSJJ" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" EnableLock="true" ColumnID="HSJE" DataField="HSJE" EnableHeaderMenu="false" RendererFunction="round2"
                                            HeaderText="含税金额" TextAlign="Right">
                                            <Editor>
                                                <f:Label ID="lblEditorHSJE" runat="server" />
                                            </Editor>
                                        </f:RenderField>

                                        <f:RenderField Width="150px" EnableLock="true" ColumnID="PRODUCERNAME" DataField="PRODUCERNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="生产厂家">
                                            <Editor>
                                                <f:Label ID="lblEditorPRODUCERNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="150px" EnableLock="true" ColumnID="PZWH" DataField="PZWH" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="注册证号">
                                            <Editor>
                                                <f:Label ID="lblEditorPZWH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField MinWidth="120px" ExpandUnusedSpace="true" EnableLock="true" ColumnID="MEMO" DataField="MEMO" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="备注<font color='red'>*</font>" TextAlign="Center">
                                            <Editor>
                                                <f:TextBox ID="tbxEditorMEMO" runat="server" MaxLength="80" EmptyText="输入备注信息" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="120px" EnableLock="true" ColumnID="DHSL" DataField="DHSL" FieldType="Int" EnableHeaderMenu="false" EnableColumnHide="false" TextAlign="Center"
                                            HeaderText="申领数量(最小单位)">
                                            <Editor>
                                                <f:Label ID="lblEditorDHSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="120px" EnableLock="true" ColumnID="FPSL" DataField="FPSL" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false" TextAlign="Center"
                                            HeaderText="出库数量">
                                            <Editor>
                                                <f:Label ID="lblEditorFPSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" EnableLock="true" ColumnID="UNITSMALLNAME" DataField="UNITSMALLNAME" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="最小包装单位">
                                            <Editor>
                                                <f:Label ID="lblUNITSMALLNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" EnableLock="true" Hidden="true" ColumnID="PRODUCER" DataField="PRODUCER" FieldType="String"
                                            HeaderText="生产厂家编码" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorPRODUCER" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" EnableLock="true" Hidden="true" ColumnID="ZPBH" DataField="ZPBH" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="制品编号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorZPBH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" EnableLock="true" Hidden="true" ColumnID="PH" DataField="PH" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="批号" TextAlign="Center">
                                            <Editor>
                                                <f:TextBox ID="tbxEditorPH" runat="server" MaxLength="20" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" EnableLock="true" Hidden="true" DataField="ISLOT" ColumnID="ISLOT" HeaderText="批号管理" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comISLOT" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" EnableLock="true" ColumnID="UNIT" DataField="UNIT" FieldType="String"
                                            HeaderText="包装单位编码" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorUNIT" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" EnableLock="true" Hidden="true" ColumnID="FPFLAGNAME" DataField="FPFLAGNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="分配状态" TextAlign="Center">
                                            <Editor>
                                                <f:TextBox ID="TextBox1" runat="server" MaxLength="80" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" EnableLock="true" Hidden="true" DataField="ISGZ" ColumnID="ISGZ" HeaderText="是否贵重" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comISGZ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" EnableLock="true" Hidden="true" DataField="ISJF" ColumnID="ISJF" HeaderText="是否计费" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comISJF" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" EnableLock="true" Hidden="true" ColumnID="HWID" DataField="HWID" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="货位" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorHWID" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" EnableLock="true" Hidden="true" ColumnID="RQ_SC" DataField="RQ_SC" EnableHeaderMenu="false"
                                            HeaderText="生产日期" TextAlign="Center" FieldType="Date" Renderer="Date" RendererArgument="yyyy-MM-dd" NullDisplayText="">
                                            <Editor>
                                                <f:Label ID="lblEditorRQ_SC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" EnableLock="true" Hidden="true" ColumnID="YXQZ" DataField="YXQZ" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="有效期至" TextAlign="Center" FieldType="Date" Renderer="Date" RendererArgument="yyyy-MM-dd" NullDisplayText="">
                                            <Editor>
                                                <f:DatePicker ID="dpkEditorYXQZ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" EnableLock="true" Hidden="true" ColumnID="JXTAX" DataField="JXTAX" EnableHeaderMenu="false"
                                            HeaderText="税率" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorJXTAX" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="BARCODE" DataField="BARCODE" FieldType="String"
                                            HeaderText="商品条码">
                                            <Editor>
                                                <f:Label ID="lblEditorBARCODE" runat="server" />
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
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
        <f:HiddenField ID="hfdValue" runat="server" />
        <f:HiddenField ID="hfdState" runat="server" />
        <f:HiddenField ID="hfdTemplateName" runat="server" />
        <f:HiddenField ID="hfdoper" runat="server"></f:HiddenField>
        <f:HiddenField ID="GoodsIsReagents" runat="server" />
        <f:HiddenField ID="hfdscreen" runat="server" />
        <f:HiddenField ID="hfdTest" runat="server" />
        <f:Window ID="Window3" Title="商品信息" Hidden="true" EnableIFrame="true" runat="server"
            EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True"
            Width="820px" Height="480px" OnClose="Window3_Close">
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
                        <f:Button ID="btnClosePostBack" Text="确定" EnableDefaultState="false" Icon="SystemSave" runat="server" OnClick="btnClosePostBack_Click">
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
                                <f:TextArea ID="txaMemo" runat="server" Label="详细说明" Height="100px" MaxLength="90" />
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar4" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="btnRejectSubmit" Text="确定" Icon="SystemSave" EnableDefaultState="false" runat="server" OnClick="btnRejectSubmit_Click" ValidateForms="Form2">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
    </form>
    <%--多选商品信息--%>
    <f:Window ID="WindowGoods" Title="科室申领商品信息" Hidden="true" EnableIFrame="false" runat="server"
        EnableMaximize="false" EnableResize="true" Target="Self" IsModal="True" BodyPadding="0px" Layout="HBox"
        BoxConfigAlign="Stretch" BoxConfigPosition="Start" Width="970px" Height="420px">
        <Toolbars>
            <f:Toolbar ID="Toolbar6" runat="server">
                <Items>
                    <f:Label ID="Label2" runat="server" Text="商品信息：" />
                    <f:TriggerBox ID="tgbSearch" LabelWidth="75px" runat="server" EmptyText="商品编码或名称" TriggerIcon="Search" OnTriggerClick="tgbSearch_TriggerClick" />
                    <f:ToolbarFill ID="ToolbarFill3" runat="server" />
                    <f:Button ID="btnCollect" runat="server" Text="收藏商品" EnableDefaultState="false" OnClick="btnCollect_Click"></f:Button>
                </Items>
            </f:Toolbar>
        </Toolbars>
        <Items>
            <f:Grid ID="GridNoSelectGoods" ShowBorder="false" ShowHeader="false" AllowSorting="false" CssStyle="border-top: 1px solid #99bce8;border-bottom: 1px solid #99bce8;"
                BoxFlex="1" AutoScroll="true" runat="server" DataKeyNames="GDSEQ" PageSize="50" IsDatabasePaging="true" EnableMultiSelect="true"
                AllowPaging="true" OnPageIndexChange="GridNoSelectGoods_PageIndexChange" EnableColumnLines="true" EnableCheckBoxSelect="true" EnableRowDoubleClickEvent="true"
                OnRowDoubleClick="GridNoSelectGoods_RowDoubleClick" KeepCurrentSelection="true">
                <Columns>
                    <f:BoundField Width="90px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                    <f:BoundField Width="180px" DataField="GDNAME" SortField="GDNAME" HeaderText="商品名称" EnableColumnHide="true" EnableHeaderMenu="false" />
                    <f:BoundField Width="100px" DataField="GDMODE" SortField="GDMODE" HeaderText="型号·尺码" EnableColumnHide="true" EnableHeaderMenu="false" />
                    <f:BoundField Width="120px" DataField="GDSPEC" SortField="GDSPEC" HeaderText="规格·容量" EnableColumnHide="true" EnableHeaderMenu="false" />
                    <f:BoundField Width="70px" DataField="UNIT" HeaderText="包装单位" Hidden="true" EnableColumnHide="true" EnableHeaderMenu="false" />
                    <f:BoundField Width="40px" DataField="UNITNAME" SortField="UNITNAME" HeaderText="单位" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                    <f:BoundField Width="70px" DataField="BZHL" HeaderText="包装数量" Hidden="true" EnableColumnHide="true" EnableHeaderMenu="false" />
                    <f:BoundField Width="165px" DataField="PIZNO" SortField="PIZNO" HeaderText="注册证号" EnableColumnHide="true" EnableHeaderMenu="false" />
                    <f:BoundField Hidden="true" DataField="PRODUCER" HeaderText="生产厂家编码" EnableColumnHide="true" EnableHeaderMenu="false" />
                    <f:BoundField Width="180px" DataField="PRODUCERNAME" SortField="PRODUCERNAME" HeaderText="生产厂家" EnableColumnHide="true" EnableHeaderMenu="false" />
                    <f:BoundField Width="100px" DataField="ZPBH" SortField="ZPBH" HeaderText="制品编号" EnableColumnHide="true" EnableHeaderMenu="false" />
                    <f:BoundField Width="100px" DataField="BARCODE" HeaderText="商品条码" Hidden="true" EnableColumnHide="true" EnableHeaderMenu="false" />
                    <f:BoundField DataField="JXTAX" Hidden="true" HeaderText="税率" />
                    <f:BoundField DataField="HSJJ" Hidden="true" HeaderText="含税进价" />
                    <f:BoundField DataField="YBJ" Hidden="true" HeaderText="医保价" />
                    <f:BoundField DataField="UNIT_ORDER" Hidden="true" HeaderText="订货单位" />
                    <f:BoundField DataField="UNIT_ORDER_NAME" Hidden="true" HeaderText="订货单位名称" />
                    <f:BoundField DataField="BZHL_ORDER" Hidden="true" HeaderText="订货单位包装含量" />
                    <f:BoundField DataField="UNIT_SELL" Hidden="true" HeaderText="出库单位" />
                    <f:BoundField DataField="UNIT_SELL_NAME" Hidden="true" HeaderText="出库单位名称" />
                    <f:BoundField DataField="BZHL_SELL" Hidden="true" HeaderText="出库单位包装含量" />
                    <f:BoundField DataField="SUPPLIER" Hidden="true" HeaderText="供应商" />
                    <f:BoundField DataField="SUPPLIERNAME" Hidden="true" HeaderText="供应商" />
                    <f:BoundField DataField="HWID" Hidden="true" HeaderText="货位ID" />
                    <f:BoundField DataField="ZJM" Hidden="true" HeaderText="助记码" />
                    <f:BoundField DataField="DEPTID" Hidden="true" HeaderText="管理部门" />
                    <f:BoundField DataField="UNIT_DABZ" Hidden="true" HeaderText="大包装单位" />
                    <f:BoundField DataField="UNIT_DABZ_NAME" Hidden="true" HeaderText="大包装单位" />
                    <f:BoundField DataField="UNIT_ZHONGBZ" Hidden="true" HeaderText="中包装单位" />
                    <f:BoundField DataField="UNIT_ZHONGBZ_NAME" Hidden="true" HeaderText="中包装单位" />
                    <f:BoundField DataField="BARCODE_DABZ" Hidden="true" HeaderText="大包装厂家条码" />
                    <f:BoundField DataField="BARCODE_ZHONGBZ" Hidden="true" HeaderText="中包装厂家条码" />
                    <f:BoundField DataField="NUM_DABZ" Hidden="true" HeaderText="大包装系数" />
                    <f:BoundField DataField="NUM_ZHONGBZ" Hidden="true" HeaderText="中包装系数" />
                    <f:BoundField DataField="YCODE" Hidden="true" HeaderText="原编码" />
                    <f:BoundField DataField="NAMEJC" Hidden="true" HeaderText="商品通用名" />
                    <f:BoundField DataField="NAMEEN" Hidden="true" HeaderText="英文名" />
                    <f:BoundField DataField="STRUCT" Hidden="true" HeaderText="主要构成" />
                    <f:BoundField DataField="FLAG" Hidden="true" HeaderText="状态" />
                    <f:BoundField DataField="CATID" Hidden="true" HeaderText="类别" />
                    <f:BoundField DataField="JX" Hidden="true" HeaderText="剂形" />
                    <f:BoundField DataField="YX" Hidden="true" HeaderText="药效" />
                    <f:BoundField DataField="BAR1" Hidden="true" HeaderText="药监码" />
                    <f:BoundField DataField="BAR2" Hidden="true" HeaderText="统计码" />
                    <f:BoundField DataField="BAR3" Hidden="true" HeaderText="其它编码" />
                    <f:BoundField DataField="HISCODE" Hidden="true" HeaderText="商品HIS 代码" />
                    <f:BoundField DataField="HISNAME" Hidden="true" HeaderText="商品HIS 名称" />
                    <f:BoundField DataField="ISLOT" Hidden="true" HeaderText="批号管理" />
                    <f:BoundField DataField="ISGZ" Hidden="true" HeaderText="是否贵重" />
                    <f:BoundField DataField="ISJF" Hidden="true" HeaderText="是否计费" />
                    <f:BoundField DataField="DEFSL" Hidden="true" HeaderText="默认数量" />
                </Columns>
            </f:Grid>
            <f:Panel ID="Panel4" Width="50px" runat="server" BodyPadding="5px" ShowBorder="true" ShowHeader="false"
                Layout="VBox" BoxConfigAlign="Stretch" BoxConfigPosition="Start" BodyStyle="background-color:#d9e7f8;">
                <Items>
                    <f:Panel ID="Panel5" Title="面板1" BoxFlex="1" runat="server" BodyStyle="background-color:#d9e7f8;"
                        BodyPadding="5px" ShowBorder="false" ShowHeader="false">
                        <Items>
                        </Items>
                    </f:Panel>
                    <f:Panel ID="Panel6" runat="server" BodyStyle="background-color:#d9e7f8;"
                        BodyPadding="3px" ShowBorder="false" ShowHeader="false">
                        <Items>
                            <f:Button ID="btnAddRight" runat="server" EnableDefaultState="false" Text=">>" CssStyle="margin-bottom:20px;" OnClick="btnAddRight_Click"></f:Button>
                            <f:Button ID="btnAddLeft" runat="server" EnableDefaultState="false" Text="<<" OnClick="btnAddLeft_Click"></f:Button>
                        </Items>
                    </f:Panel>
                    <f:Panel ID="Panel7" BoxFlex="1" Margin="0" BodyStyle="background-color:#d9e7f8;"
                        runat="server" BodyPadding="5px" ShowBorder="false" ShowHeader="false">
                        <Items>
                        </Items>
                    </f:Panel>
                </Items>
            </f:Panel>
            <f:Grid ID="GridCFGGoods" ShowBorder="false" ShowHeader="false" AllowSorting="false" BoxFlex="1" CssStyle="border-top: 1px solid #99bce8;border-bottom: 1px solid #99bce8;"
                AutoScroll="true" runat="server" DataKeyNames="GDSEQ" EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridCFGGoods_RowDoubleClick"
                EnableMultiSelect="true" EnableColumnLines="true" EnableCheckBoxSelect="true" KeepCurrentSelection="true">
                <Columns>
                    <f:BoundField Width="90px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                    <f:BoundField Width="180px" DataField="GDNAME" SortField="GDNAME" HeaderText="商品名称" EnableColumnHide="true" EnableHeaderMenu="false" />
                    <f:BoundField Width="120px" DataField="GDSPEC" SortField="GDSPEC" HeaderText="规格·容量" EnableColumnHide="true" EnableHeaderMenu="false" />
                    <f:BoundField Width="70px" DataField="UNIT" HeaderText="包装单位" Hidden="true" EnableColumnHide="true" EnableHeaderMenu="false" />
                    <f:BoundField Width="40px" DataField="UNITNAME" SortField="UNITNAME" HeaderText="单位" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                    <f:BoundField Width="70px" DataField="BZHL" HeaderText="包装数量" Hidden="true" EnableColumnHide="true" EnableHeaderMenu="false" />
                    <f:BoundField Width="165px" DataField="PIZNO" SortField="PIZNO" HeaderText="注册证号" EnableColumnHide="true" EnableHeaderMenu="false" />
                    <f:BoundField Hidden="true" DataField="PRODUCER" HeaderText="生产厂家编码" EnableColumnHide="true" EnableHeaderMenu="false" />
                    <f:BoundField Width="180px" DataField="PRODUCERNAME" SortField="PRODUCERNAME" HeaderText="生产厂家" EnableColumnHide="true" EnableHeaderMenu="false" />
                    <f:BoundField Width="100px" DataField="ZPBH" SortField="ZPBH" HeaderText="制品编号" EnableColumnHide="true" EnableHeaderMenu="false" />
                    <f:BoundField Width="100px" DataField="BARCODE" HeaderText="商品条码" Hidden="true" EnableColumnHide="true" EnableHeaderMenu="false" />
                    <f:BoundField DataField="JXTAX" Hidden="true" HeaderText="税率" />
                    <f:BoundField DataField="HSJJ" Hidden="true" HeaderText="含税进价" />
                    <f:BoundField DataField="YBJ" Hidden="true" HeaderText="医保价" />
                    <f:BoundField DataField="UNIT_ORDER" Hidden="true" HeaderText="订货单位" />
                    <f:BoundField DataField="UNIT_ORDER_NAME" Hidden="true" HeaderText="订货单位名称" />
                    <f:BoundField DataField="BZHL_ORDER" Hidden="true" HeaderText="订货单位包装含量" />
                    <f:BoundField DataField="UNIT_SELL" Hidden="true" HeaderText="出库单位" />
                    <f:BoundField DataField="UNIT_SELL_NAME" Hidden="true" HeaderText="出库单位名称" />
                    <f:BoundField DataField="BZHL_SELL" Hidden="true" HeaderText="出库单位包装含量" />
                    <f:BoundField DataField="SUPPLIER" Hidden="true" HeaderText="供应商" />
                    <f:BoundField DataField="SUPPLIERNAME" Hidden="true" HeaderText="供应商" />
                    <f:BoundField DataField="HWID" Hidden="true" HeaderText="货位ID" />
                    <f:BoundField DataField="ZJM" Hidden="true" HeaderText="助记码" />
                    <f:BoundField DataField="DEPTID" Hidden="true" HeaderText="管理部门" />
                    <f:BoundField DataField="UNIT_DABZ" Hidden="true" HeaderText="大包装单位" />
                    <f:BoundField DataField="UNIT_DABZ_NAME" Hidden="true" HeaderText="大包装单位" />
                    <f:BoundField DataField="UNIT_ZHONGBZ" Hidden="true" HeaderText="中包装单位" />
                    <f:BoundField DataField="UNIT_ZHONGBZ_NAME" Hidden="true" HeaderText="中包装单位" />
                    <f:BoundField DataField="BARCODE_DABZ" Hidden="true" HeaderText="大包装厂家条码" />
                    <f:BoundField DataField="BARCODE_ZHONGBZ" Hidden="true" HeaderText="中包装厂家条码" />
                    <f:BoundField DataField="NUM_DABZ" Hidden="true" HeaderText="大包装系数" />
                    <f:BoundField DataField="NUM_ZHONGBZ" Hidden="true" HeaderText="中包装系数" />
                    <f:BoundField DataField="YCODE" Hidden="true" HeaderText="原编码" />
                    <f:BoundField DataField="NAMEJC" Hidden="true" HeaderText="商品通用名" />
                    <f:BoundField DataField="NAMEEN" Hidden="true" HeaderText="英文名" />
                    <f:BoundField DataField="STRUCT" Hidden="true" HeaderText="主要构成" />
                    <f:BoundField DataField="FLAG" Hidden="true" HeaderText="状态" />
                    <f:BoundField DataField="CATID" Hidden="true" HeaderText="类别" />
                    <f:BoundField DataField="JX" Hidden="true" HeaderText="剂形" />
                    <f:BoundField DataField="YX" Hidden="true" HeaderText="药效" />
                    <f:BoundField DataField="BAR1" Hidden="true" HeaderText="药监码" />
                    <f:BoundField DataField="BAR2" Hidden="true" HeaderText="统计码" />
                    <f:BoundField DataField="BAR3" Hidden="true" HeaderText="其它编码" />
                    <f:BoundField DataField="HISCODE" Hidden="true" HeaderText="商品HIS 代码" />
                    <f:BoundField DataField="HISNAME" Hidden="true" HeaderText="商品HIS 名称" />
                    <f:BoundField DataField="ISLOT" Hidden="true" HeaderText="批号管理" />
                    <f:BoundField DataField="ISGZ" Hidden="true" HeaderText="是否贵重" />
                    <f:BoundField DataField="ISJF" Hidden="true" HeaderText="是否计费" />
                    <f:BoundField DataField="DEFSL" Hidden="true" HeaderText="默认数量" />
                </Columns>
            </f:Grid>
        </Items>
        <Toolbars>
            <f:Toolbar ID="Toolbar5" runat="server" Position="Bottom" ToolbarAlign="Right">
                <Items>
                    <f:Button ID="btnSve" Text="确 定" EnableDefaultState="false" Icon="SystemSave" runat="server" OnClick="btnClosePostBack_Click">
                    </f:Button>
                </Items>
            </f:Toolbar>
        </Toolbars>
    </f:Window>
    <f:Window ID="Window1" Title="保存模板信息" Hidden="true" EnableIFrame="false" runat="server"
        EnableMaximize="false" EnableResize="false" IsModal="True" Layout="Fit" Width="270px" Height="120px">
        <Items>
            <f:Form ID="Form3" ShowBorder="false" AutoScroll="false" BodyPadding="5px 10px"
                ShowHeader="False" LabelWidth="75px" runat="server">
                <Rows>
                    <f:FormRow>
                        <Items>
                            <f:TextBox ID="tbsFileName" runat="server" Label="模板名称" MaxLength="20" ShowRedStar="true" Required="true" EmptyText="保存模板的名称"></f:TextBox>
                        </Items>
                    </f:FormRow>
                </Rows>
            </f:Form>
        </Items>
        <Toolbars>
            <f:Toolbar ID="Toolbar7" runat="server" Position="Bottom" ToolbarAlign="Right">
                <Items>
                    <f:Button ID="btnSaveTemplateClose" Text="保存确定" EnableDefaultState="false" Icon="SystemSave" runat="server" OnClick="btnSaveTemplateClose_Click">
                    </f:Button>
                </Items>
            </f:Toolbar>
        </Toolbars>
    </f:Window>
    <f:Window ID="WinAuto" Title="自动出库信息" Hidden="true" EnableIFrame="false" runat="server" ShowHeader="true" ShowBorder="true"
        EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="580px" Height="280px">
        <Items>
            <f:Panel ID="Panel9" runat="server" ShowBorder="false" ShowHeader="false" Layout="Region">
                <Items>
                    <f:Panel runat="server" ID="panelLeftRegion" RegionPosition="Center" RegionSplit="false" EnableCollapse="false" CssStyle="border-bottom: 0px solid #99bce8;"
                        Width="390px" Title="自动出库" ShowBorder="false" ShowHeader="false">
                        <Items>
                            <f:Form ID="Form4" ShowBorder="false" AutoScroll="false" BodyPadding="5px 10px"
                                ShowHeader="False" LabelWidth="75px" runat="server">
                                <Rows>
                                    <f:FormRow>
                                        <Items>
                                            <f:RadioButtonList ID="rblTYPE" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rblTYPE_SelectedIndexChanged">
                                                <f:RadioItem Text="历史出库生成" Value="XS" Selected="true" />
                                                <f:RadioItem Text="库存参数生成" Value="KC" />
                                            </f:RadioButtonList>
                                        </Items>
                                    </f:FormRow>
                                    <f:FormRow>
                                        <Items>
                                            <f:RadioButtonList ID="rblBILTYPE" runat="server">
                                                <f:RadioItem Value="LYD" Text="物资请领" Selected="true" />
                                                <f:RadioItem Value="GBD" Text="高值备货" />
                                            </f:RadioButtonList>
                                        </Items>
                                    </f:FormRow>
                                    <f:FormRow>
                                        <Items>
                                            <f:DropDownList runat="server" ID="ddlDeptOrder" Label="出库库房" ShowRedStar="true" Required="true"></f:DropDownList>
                                            <f:DropDownList runat="server" ID="ddlDeptid" Label="入库科室" ShowRedStar="true" Required="true" OnSelectedIndexChanged="ddlDeptid_SelectedIndexChanged"></f:DropDownList>
                                        </Items>
                                    </f:FormRow>

                                    <f:FormRow>
                                        <Items>
                                            <f:DatePicker runat="server" ID="dbpOrder1" Label="销售日期"></f:DatePicker>
                                            <f:DatePicker runat="server" ID="dbpOrder2" Label="至" LabelWidth="30px" CompareControl="dbpOrder1" CompareOperator="GreaterThanEqual" CompareType="String" CompareMessage="日期维护不正确"></f:DatePicker>
                                        </Items>
                                    </f:FormRow>
                                </Rows>
                                <Rows>
                                    <f:FormRow>
                                        <Items>
                                            <f:Label runat="server" EncodeText="false" Text="<strong>公式说明（考虑订货包装）:</strong>"></f:Label>
                                        </Items>
                                    </f:FormRow>
                                    <f:FormRow>
                                        <Items>
                                            <f:Label ID="memo" runat="server" Text="申请量 = 销售期间的销售量 - 科室库存 - 在途库存"></f:Label>
                                        </Items>
                                    </f:FormRow>
                                </Rows>
                            </f:Form>
                        </Items>
                    </f:Panel>
                </Items>
            </f:Panel>
        </Items>
        <Toolbars>
            <f:Toolbar ID="Toolbar9" runat="server" Position="Bottom" ToolbarAlign="Right">
                <Items>
                    <f:Button ID="btn_Sure" Text="生成计划单" EnableDefaultState="false" Icon="SystemSave" ValidateForms="Form4" runat="server" OnClick="btnSure_Click">
                    </f:Button>
                    <f:Button ID="btn_close" Text="关闭" EnableDefaultState="false" Icon="SystemClose" runat="server" OnClick="btn_close_Click">
                    </f:Button>
                </Items>
            </f:Toolbar>
        </Toolbars>
    </f:Window>
    <f:Window ID="Window2" Title="加载模板信息" Hidden="true" EnableIFrame="false" runat="server"
        EnableMaximize="false" EnableResize="false" IsModal="True" Layout="Fit" Width="470px" Height="320px">
        <Items>
            <f:Grid ID="GridTemplate" ShowBorder="false" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server" EnableMultiSelect="false"
                DataKeyNames="GROUPID,GROUPNAME" EnableColumnLines="true" OnRowCommand="GridTemplate_RowCommand"
                EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridTemplate_RowDoubleClick">
                <Columns>
                    <f:RowNumberField runat="server" Width="30" TextAlign="Center" />
                    <f:BoundField DataField="GROUPID" EnableColumnHide="true" EnableHeaderMenu="false" Hidden="true" />
                    <f:BoundField DataField="GROUPNAME" HeaderText="模板名称" EnableColumnHide="true" EnableHeaderMenu="false" ExpandUnusedSpace="true" />
                    <f:BoundField DataField="USERNAME" HeaderText="添加人" Width="90px" EnableColumnHide="true" EnableHeaderMenu="false" />
                    <f:LinkButtonField HeaderText="操作" Width="50px" TextAlign="Center" CommandName="FileDelete" Text="删除" ConfirmText="确定要删除该模板么？" />
                </Columns>
            </f:Grid>
        </Items>
        <Toolbars>
            <f:Toolbar ID="Toolbar8" runat="server" Position="Bottom" ToolbarAlign="Right">
                <Items>
                    <f:Button ID="btnLoadTemplateClose" Text="保存确定" EnableDefaultState="false" Icon="SystemSave" runat="server" OnClick="btnSaveTemplateClose_Click">
                    </f:Button>
                </Items>
            </f:Toolbar>
        </Toolbars>
    </f:Window>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            if (window.screen.width < 1024 || window.screen.height < 768) {
                F.customEvent('SETSCREEN');
            }

        });
        function PosGoodsQt(Info) {
            F('<%= hfdValue.ClientID%>').setValue(Info);

            F.customEvent('GoodsAdd');
        }
        function onGridBeforeEdit(event, value, params) {
            //console.log(params.tr);
            var flag = F('<%= docFLAG.ClientID%>').getValue();
            var opervalue = F('<%= hfdoper.ClientID%>').getValue();
            if ((",M,R,N").indexOf(flag) > 0 && opervalue == "input" && flag != "N") {
                return true
            }
            else if ((",N").indexOf(flag) > 0 && opervalue == "audit") {
                return true
            }
            else
                return false;
        }
        function Print_Click() {
            var billNo = F('<%= docBILLNO.ClientID%>').getValue();
            var billState = F('<%= docFLAG.ClientID%>').getValue();
            if (billState == "M" || billState == "R") {
                F.alert("选择单据未提交,不允许打印！");
                return;
            }
            if (billNo == "") {
                F.alert("请选择要打印的单据信息！");
                return;
            }
            ReportViewer.ReportURL = "/grf/KSSL.grf?20160111";
            var dataurl = "/captcha/PrintReport.aspx?Method=GetKSSLData&osid=" + billNo;

            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
        function onGridAfterEdit(event, value, params) {
            var me = this, columnId = params.columnId, rowId = params.rowId;
            if (columnId === 'BZSL') {
                var BZSL = me.getCellValue(rowId, 'BZSL');
                var BZHL = me.getCellValue(rowId, 'BZHL');
                var HSJJ = me.getCellValue(rowId, 'HSJJ');
                me.updateCellValue(rowId, 'DHSL', BZSL * BZHL);
                me.updateCellValue(rowId, 'HSJE', BZSL * HSJJ);
                var BZSLTotal = 0, HSJETotal = 0, DHSLTotal = 0;
                me.getRowEls().each(function (index, tr) {
                    BZSLTotal += me.getCellValue(tr, 'BZSL');
                    DHSLTotal += me.getCellValue(tr, 'DHSL');
                    HSJETotal += me.getCellValue(tr, 'BZSL') * me.getCellValue(tr, 'HSJJ');
                });
                me.updateSummaryCellValue('GDNAME', "本页合计", true);
                me.updateSummaryCellValue('BZSL', BZSLTotal, true);
                me.updateSummaryCellValue('HSJE', HSJETotal, true);
                me.updateSummaryCellValue('DHSL', DHSLTotal, true);
            }
        }
    </script>
</body>
</html>

