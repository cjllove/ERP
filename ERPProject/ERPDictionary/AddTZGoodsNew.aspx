﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddTZGoodsNew.aspx.cs" Inherits="ERPProject.ERPDictionary.AddTZGoodsNew" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>商品新增管理</title>
    <script src="/res/js/CreateControl.js" type="text/javascript"></script>
    <script src="/res/js/GRInstall.js" type="text/javascript"></script>
    <style type="text/css" media="all">
        .f-grid-row[data-color=color1],
        .f-grid-row[data-color=color1] .ui-icon,
        .f-grid-row[data-color=color1] a {
            background-color: #5fe139;
            color: #000000;
        }

        .f-grid-row[data-color=color2],
        .f-grid-row[data-color=color2] .ui-icon,
        .f-grid-row[data-color=color2] a {
            background-color: #0a0ff6;
            color: #fff;
        }

        .f-grid-row[data-color=color3],
        .f-grid-row[data-color=color3] .ui-icon,
        .f-grid-row[data-color=color3] a {
            background-color: red;
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
                                                        <f:DropDownList ID="lstFLAG" runat="server" Label="状态" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="lstLRRQ1" runat="server" Label="使用日期" Required="true" ShowRedStar="true" />
                                                        <f:DatePicker ID="lstLRRQ2" runat="server" Label="　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" AnchorValue="100% -70" ShowBorder="false" ShowHeader="false"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" OnRowDataBound="GridList_RowDataBound"
                                    DataKeyNames="SEQNO" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick" EnableColumnLines="true"
                                    AllowSorting="true" EnableHeaderMenu="true" SortField="BILLNO" OnSort="GridList_Sort">
                                    <Columns>
                                        <f:RowNumberField Width="30px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField DataField="SEQNO" Hidden="true" SortField="SEQNO" />
                                        <f:BoundField Width="100px" DataField="BILLNO" HeaderText="单据编号" SortField="BILLNO" TextAlign="Center" />
                                        <f:BoundField Width="0px" Hidden="true" DataField="FLAG" HeaderText="单据状态" SortField="FLAG" TextAlign="Center" />
                                        <f:BoundField Width="70px" ColumnID="FLAGNAME" DataField="FLAGNAME" HeaderText="单据状态" SortField="FLAG" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="XGTYPENAME" HeaderText="单据类型" SortField="XGTYPE" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="SUBNUM" HeaderText="明细条数" SortField="SUBNUM" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="LRY" HeaderText="申请人" SortField="LRY" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="LRRQ" HeaderText="申请日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="80px" DataField="SPR" HeaderText="审批人" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="SPRQ" HeaderText="审批日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="300px" DataField="MEMO" HeaderText="备注" SortField="MEMO" TextAlign="Center" ExpandUnusedSpace="true" />
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
                                                <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="操作信息：双击打开录入界面！" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="btnNew" Icon="PageAdd" Text="新 单" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnDel" Icon="PageCancel" Text="删除单据" EnablePostBack="true" ConfirmText="是否删除此单据?" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnAddRow" Icon="BookAdd" Text="新 增" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" Hidden="true" />
                                                <f:Button ID="btnUpdate" Icon="BookEdit" Text="修 改" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnUpdate_Click" ValidateForms="FormDoc" Hidden="true" />
                                                <f:Button ID="btnDelect" Icon="BookDelete" Text="禁 用" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnDelect_Click" ValidateForms="FormDoc" Hidden="true" />
                                                <f:Button ID="btnDelRow" Icon="Delete" Text="删 行" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否删除选中行信息?" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnSave" Icon="Disk" Text="保 存" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnTJ" Icon="UserTick" Text="提交" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnTJ_Click" ValidateForms="FormDoc" ConfirmText="是否确认保存并提交此单据？" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnAudit" Icon="UserTick" Text="审 批" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认审核此单据?" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" />
                                                <f:Button ID="btnCancel" Icon="UserCross" Text="驳 回" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnCancel_Click" ConfirmText="是否驳回改单据?" />
                                                <f:Button ID="btnPrint" Icon="Printer" Text="打 印" EnableDefaultState="false" EnablePostBack="false" runat="server" OnClientClick="btnPrint_Bill()" Hidden="true" />
                                                <f:Button ID="btnCopy" Icon="PageCopy" Text="复 制" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否确认复制此单据信息?" OnClick="btnBill_Click" Hidden="true" />
                                                <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" OnClick="btnExport_Click" ConfirmText="是否导出当前商品信息?" DisableControlBeforePostBack="false" EnableDefaultState="false"
                                                    EnablePostBack="true" EnableAjax="false" EnableAjaxLoading="true" AjaxLoadingType="Mask" runat="server" />
                                                <f:Button ID="btnGoods" Icon="Magnifier" Text="追加商品" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnGoods_Click" ValidateForms="FormDoc" />
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
                                                        <f:DropDownList ID="ddlType" runat="server" Label="单据类型">
                                                            <f:ListItem Text="--请选择--" Value="" />
                                                            <f:ListItem Text="新增" Value="N" />
                                                            <f:ListItem Text="修改" Value="M" />
                                                            <f:ListItem Text="删除" Value="D" />
                                                        </f:DropDownList>
                                                        <f:TextBox ID="tbxBILLNO" runat="server" Label="单据编号" EmptyText="自动生成" MaxLength="20" />
                                                        <f:DropDownList ID="ddlFLAG" runat="server" Label="单据状态" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="docLRY" runat="server" Label="录入人" Enabled="false" ShowRedStar="true" Required="true" />
                                                        <f:DatePicker ID="dpkLRRQ" runat="server" Label="录入日期" Enabled="false" />
                                                        <f:DropDownList ID="ddlSPR" runat="server" Label="审核人" Enabled="false" />

                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow ColumnWidths="66% 33%">
                                                    <Items>
                                                        <f:TextBox ID="txtMEMO" runat="server" Label="备注" MaxLength="200" />
                                                        <f:DatePicker ID="dpkSPRQ" runat="server" Label="审批日期" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridGoods" AnchorValue="100% -143" ShowBorder="false" ShowHeader="false"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;"
                                    DataKeyNames="SEQNO,ROWNO,GDSEQ" AllowCellEditing="true" ClicksToEdit="1" EnableRowLines="true" EnableRowDoubleClickEvent="true" OnRowDoubleClick="goodsRow_DoubleClick" EnableColumnLines="true" EnableCollapse="false" EnableTextSelection="true">
                                    <Columns>
                                        <f:RowNumberField Width="50px" TextAlign="Center"></f:RowNumberField>
                                        <f:BoundField Width="100px" ColumnID="GDSEQ" DataField="GDSEQ" HeaderText="商品编码" />
                                        <f:BoundField Width="100px" ColumnID="GDID" DataField="GDID" HeaderText="商品编码" Hidden="true" />
                                        <f:BoundField Width="100px" ColumnID="GDNAME" DataField="GDNAME" HeaderText="商品名称" />
                                        <f:BoundField Width="100px" ColumnID="ZJM" DataField="ZJM" HeaderText="助记码" Hidden="true" />
                                        <f:BoundField Width="100px" ColumnID="NAMEJC" DataField="NAMEJC" HeaderText="通用名" Hidden="true" />
                                        <f:BoundField Width="100px" ColumnID="GDSPEC" DataField="GDSPEC" HeaderText="规格、容量" />
                                        <f:BoundField Width="100px" ColumnID="UNITNAME" DataField="UNITNAME" HeaderText="单位" />
                                        <f:BoundField Width="100px" ColumnID="CATIDNAME" DataField="CATIDNAME" HeaderText="商品类别" Hidden="true" />
                                        <f:BoundField Width="100px" ColumnID="HSJJ" DataField="HSJJ" HeaderText="单价" TextAlign="Center" />
                                        <f:BoundField Width="100px" ColumnID="SUPPLIETNAME" DataField="SUPPLIETNAME" HeaderText="供应商" />
                                        <f:BoundField Width="100px" ColumnID="PRODUCERNAME" DataField="PRODUCERNAME" HeaderText="生产厂家" />
                                        <f:BoundField Width="100px" ColumnID="PIZNO" DataField="PIZNO" HeaderText="注册证号" />
                                        <%--<f:BoundField Width="100px" ColumnID="JFDM" DataField="JFDM" HeaderText="计费代码" />--%>
                                        <f:BoundField Width="100px" ColumnID="CATID0NAME" DataField="CATID0NAME" HeaderText="商品种类" Hidden="true" />
                                        <f:BoundField Width="100px" ColumnID="GZNAME" DataField="GZNAME" HeaderText="是否高值" />
                                        <f:BoundField Width="100px" ColumnID="BDNAME" DataField="BDNAME" HeaderText="是否本地" />
                                        <%--<f:BoundField Width="100px" ColumnID="JFNAME" DataField="JFNAME" HeaderText="是否计费" />--%>
                                        <f:BoundField Width="100px" ColumnID="PHNAME" DataField="PHNAME" HeaderText="是否批号管理" />
                                        <f:BoundField Width="100px" ColumnID="UNIT" DataField="UNIT" HeaderText="单位" Hidden="true" />
                                        <f:BoundField Width="100px" ColumnID="CATID" DataField="CATID" HeaderText="商品类别" Hidden="true" />
                                        <f:BoundField Width="100px" ColumnID="SUPPLIER" DataField="SUPPLIER" HeaderText="供应商" Hidden="true" />
                                        <f:BoundField Width="100px" ColumnID="PRODUCER" DataField="PRODUCER" HeaderText="生产厂家" Hidden="true" />
                                        <f:BoundField Width="100px" ColumnID="CATID0NAME" DataField="CATID0" HeaderText="商品种类" Hidden="true" />
                                        <f:BoundField Width="100px" ColumnID="ISGZ" DataField="ISGZ" HeaderText="是否高值" Hidden="true" />
                                        <f:BoundField Width="100px" ColumnID="ISFLAG7" DataField="ISFLAG7" HeaderText="是否本地" Hidden="true" />
                                        <%--<f:BoundField Width="100px" ColumnID="ISFLAG9" DataField="ISFLAG9" HeaderText="是否计费" Hidden="true" />--%>
                                        <f:BoundField Width="100px" ColumnID="ISLOT" DataField="ISLOT" HeaderText="是否批号管理" Hidden="true" />
                                    </Columns>
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
        <f:HiddenField ID="hfdRowID" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdTEMP" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdIsNew" runat="server" />
        <f:HiddenField ID="hfdSup" runat="server" />
        <f:Window ID="WindowGoods" Title="商品信息" Hidden="true" EnableIFrame="false" runat="server" AutoScroll="true"
            EnableMaximize="false" EnableResize="true" Target="Parent" Layout="Anchor" Width="900px" Height="500px">
            <Items>
                <f:Grid ID="Grid1" ShowBorder="false" ShowHeader="false" AllowSorting="false" Height="250px" EnableMultiSelect="true" AutoScroll="true"
                    runat="server" DataKeyNames="GDSEQ" EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridGoods_RowDoubleClick"
                    PageSize="50" IsDatabasePaging="true" AllowPaging="true" OnPageIndexChange="Grid1_PageIndexChange" EnableColumnLines="true" EnableTextSelection="true" EnableCheckBoxSelect="true">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar3" runat="server">
                            <Items>
                                <%--<f:TriggerBox ID="trbSearch" LabelWidth="70px" Width="330px" runat="server" Label="查询信息" ShowTrigger="false" CssStyle="margin-left:5px;" EmptyText="商品编码,商品名称,助记码,生产商" TriggerIcon="Search" OnTriggerClick="trbSearch_TriggerClick" />
                                <f:Button ID="btnSearch" Icon="Magnifier" Text="查 询" EnablePostBack="true" runat="server" OnClick="btnSearch_Click" EnableDefaultState="false" />--%>
                                <f:TriggerBox ID="trbSearch" runat="server" ShowTrigger="false" EmptyText="可输入商品名称、助记码、商品编码或流水码" TriggerIcon="Search" OnTriggerClick="trbSearch_TriggerClick" Width="300px" />
                                <f:TriggerBox ID="trbGdSpec" runat="server" EmptyText="可输入商品规格查询" MaxLength="20" ShowTrigger="false" TriggerIcon="Search" OnTriggerClick="trbSearch_TriggerClick" Width="250px" />
                                <f:DropDownList ID="ddlISZS" runat="server" OnSelectedIndexChanged="ddlISZS_SelectedIndexChanged" AutoPostBack="true" Width="200px">
                                    <f:ListItem Value="" Text="--请选择--" Selected="true" />
                                    <f:ListItem Value="Y" Text="直送" />
                                    <f:ListItem Value="N" Text="非直送" />
                                </f:DropDownList>
                                <f:Button ID="btnSearch" Icon="Magnifier" Text="查 询" EnablePostBack="true" runat="server" OnClick="btnSearch_Click" EnableDefaultState="false" />
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Columns>
                        <f:RowNumberField Width="35" TextAlign="Center" EnablePagingNumber="true" />
                        <f:BoundField Width="105px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                        <f:BoundField Width="105px" DataField="SUPID" HeaderText="供应商编码" TextAlign="Center" Hidden="true" />
                        <f:BoundField Width="140px" DataField="GDNAME" HeaderText="商品名称" TextAlign="Left" />
                        <f:BoundField Width="130px" DataField="GDSPEC" HeaderText="规格·容量" />
                        <f:BoundField Width="80px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                        <f:BoundField Width="55px" DataField="FLAG_CN" HeaderText="状态" TextAlign="Center" />
                        <f:BoundField Width="60px" DataField="BZHL" HeaderText="包装含量" Hidden="true" />
                        <f:BoundField Width="100px" DataField="HSJJ" HeaderText="含税进价" TextAlign="Right" DataFormatString="{0:F4}" />
                        <f:BoundField Width="80px" DataField="CATID0NAME" HeaderText="商品种类" TextAlign="Center" />
                        <f:BoundField Width="0px" DataField="CATID0NAME_F" HeaderText="商品分类" TextAlign="Center" Hidden="true" />
                        <f:BoundField Width="140px" DataField="PIZNO" HeaderText="注册证号" />
                        <f:BoundField Width="140px" DataField="SUPNAME" HeaderText="配送商" />
                        <f:BoundField Width="140px" DataField="PRODUCERNAME" HeaderText="生产商" />
                        <f:BoundField Width="80px" DataField="LSJ" HeaderText="售价" TextAlign="Right" DataFormatString="{0:F4}" Hidden="true" />
                        <f:BoundField Width="90px" DataField="ZPBH" HeaderText="制品编号" TextAlign="Center" Hidden="true" />
                        <f:BoundField Width="60px" DataField="ISNEW_CN" HeaderText="本地" TextAlign="Center" />
                        <f:BoundField Width="60px" DataField="ISZSNAME" HeaderText="是否直送" TextAlign="Center" />
                    </Columns>
                </f:Grid>
                <f:TabStrip ID="TabStripMain" Height="255px" ShowBorder="false" TabPosition="Right" EnableFrame="false" AutoPostBack="true"
                    EnableTabCloseMenu="false" ActiveTabIndex="0" CssStyle="border-top: 1px solid #99bce8;"
                    runat="server">
                    <Tabs>
                        <f:Tab Title="基本信息" BodyPadding="5px" Layout="Fit" runat="server">
                            <Items>
                                <f:Form ID="FormMain" ShowBorder="false" AutoScroll="false" ShowHeader="False" LabelWidth="90px" runat="server">
                                    <Rows>
                                        <f:FormRow ColumnWidths="50% 0% 25% 25%">
                                            <Items>
                                                <f:TextBox ID="tbxGDID" runat="server" Label="商品编码" EmptyText="设定后不可更改(为空则自动生成)" TabIndex="1" Enabled="false" ShowRedStar="true" MaxLength="20">
                                                </f:TextBox>
                                                <f:DropDownList ID="lstTYPE" runat="server" Label="商品状态" TabIndex="2" EnableEdit="true" Enabled="false" ForceSelection="true" Hidden="true">
                                                    <f:ListItem Text="新增" Value="N" />
                                                    <f:ListItem Text="修改" Value="M" />
                                                    <f:ListItem Text="删除" Value="D" />
                                                </f:DropDownList>
                                                <f:DropDownList ID="ddlCATID" runat="server" Label="商品分类" Required="true" ShowRedStar="true" AutoPostBack="true" ForceSelection="true" OnSelectedIndexChanged="ddlCATID_SelectedIndexChanged" TabIndex="3">
                                                </f:DropDownList>
                                                <f:DropDownList ID="ddlCATID0" runat="server" Label="商品种类" Required="true" ShowRedStar="true" TabIndex="22" Enabled="false">
                                                </f:DropDownList>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:TextBox ID="tbsGDNAME" runat="server" Label="商品名称" Required="true" AutoPostBack="true"
                                                    ShowRedStar="true" OnTextChanged="tbsGDNAME_TextChanged" TabIndex="21" MaxLength="100" />
                                                <f:TextBox ID="tbxZJM" runat="server" Label="助记码" TabIndex="4" MaxLength="50">
                                                </f:TextBox>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow ColumnWidths="50% 25% 25%">
                                            <Items>
                                                <f:TextBox ID="tbxNAMEJC" runat="server" Label="通用名" Required="true" ShowRedStar="true" TabIndex="31" MaxLength="100" />
                                                <f:DropDownList ID="ddlISFLAG7" Label="商品类型" Required="true" ShowRedStar="true" runat="server">
                                                    <f:ListItem Text="--请选择--" Value="" />
                                                    <f:ListItem Text="本地" Value="Y" />
                                                    <f:ListItem Text="下传" Value="N" />
                                                </f:DropDownList>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow ColumnWidths="25% 25% 25% 25%">
                                            <Items>
                                                <f:TextBox ID="tbxGDSPEC" runat="server" Label="规格·容量" Required="true" ShowRedStar="true" TabIndex="52" MaxLength="200">
                                                </f:TextBox>
                                                <f:DropDownList ID="ddlUNIT" runat="server" Label="包装单位" Required="true" ShowRedStar="true" TabIndex="42" EnableEdit="true" ForceSelection="true">
                                                </f:DropDownList>
                                                <f:CheckBox ID="ckbISGZ" runat="server" Label="高值商品" TabIndex="28" />
                                                <f:TextBox ID="tbxGDMODE" runat="server" Label="商品型号"></f:TextBox>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow ColumnWidths="25% 25% 25% 25%">
                                            <Items>
                                                <%-- <f:NumberBox ID="nbbBHSJJ" runat="server" Label="不含税进价" TabIndex="30" DecimalPrecision="6" MinValue="0" MaxLength="12"></f:NumberBox>--%>
                                                <f:NumberBox ID="nbbHSJJ" runat="server" Label="含税进价" TabIndex="29" DecimalPrecision="6" MinValue="0" MaxLength="12" Required="true" ShowRedStar="true"></f:NumberBox>
                                                <f:DropDownList ID="ddlISLOT" runat="server" Label="批号管理" TabIndex="43">
                                                    <f:ListItem Text="不进行" Value="0" />
                                                    <f:ListItem Text="只有入库" Value="1" />
                                                    <f:ListItem Text="全部" Value="2" Selected="true" />
                                                </f:DropDownList>

                                                <%--                                                <f:CheckBox ID="ckbISFLAG9" runat="server" Label="计费商品" TabIndex="28" OnCheckedChanged="ckbISFLAG9_CheckedChanged" Checked="True" AutoPostBack="True" />
                                                <f:TextBox ID="tbxJFDM" runat="server" Label="计费代码" TabIndex="74" MaxLength="50">
                                                </f:TextBox>--%>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow ColumnWidths="50% 25% 25%">
                                            <Items>
                                                <f:DropDownList ID="trbPRODUCER" Label="生产商" runat="server" Required="true" EnableEdit="true" ForceSelection="true" ShowRedStar="true" TabIndex="61">
                                                </f:DropDownList>
                                                <f:TextBox ID="tbxPIZNO" runat="server" Label="注册证号" TabIndex="74" MaxLength="50">
                                                </f:TextBox>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow ColumnWidths="50% 25% 25%">
                                            <Items>
                                                <f:DropDownList ID="trbSUPPLIER" Label="供应商" runat="server" TabIndex="51" EnableEdit="true" ForceSelection="true" />
                                                <f:DropDownList ID="ddlUNIT_DABZ" runat="server" Label="大包装单位" TabIndex="81" EnableEdit="true" ForceSelection="true">
                                                </f:DropDownList>
                                                <f:NumberBox ID="nbbNUM_DABZ" runat="server" Label="大包装数量" TabIndex="82" MinValue="0" NoDecimal="true" MaxLength="16"></f:NumberBox>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:DropDownList ID="ddlUNIT_ZHONGBZ" runat="server" Label="中包装单位" TabIndex="91" EnableEdit="true" ForceSelection="true" Enabled="false">
                                                </f:DropDownList>
                                                <f:NumberBox ID="nbbNUM_ZHONGBZ" runat="server" Label="中包装数量" TabIndex="92" MinValue="0" NoDecimal="true" MaxLength="16"></f:NumberBox>
                                                <f:DropDownList ID="ddlUNIT_ORDER" runat="server" Label="订货单位" TabIndex="84">
                                                    <f:ListItem Text="大包装" Value="D" />
                                                    <f:ListItem Text="中包装" Value="Z" />
                                                    <f:ListItem Text="小包装" Value="X" Selected="true" />
                                                </f:DropDownList>
                                                <f:DropDownList ID="ddlUNIT_SELL" runat="server" Label="出库单位" TabIndex="94">
                                                    <f:ListItem Text="大包装" Value="D" />
                                                    <f:ListItem Text="中包装" Value="Z" />
                                                    <f:ListItem Text="小包装" Value="X" Selected="true" />
                                                </f:DropDownList>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow Hidden="true">
                                            <Items>
                                                <f:NumberBox ID="tbxBZHL" runat="server" Label="包装含量" Hidden="true" NoDecimal="true" MaxLength="8">
                                                </f:NumberBox>

                                                <f:HiddenField ID="hfdGDSEQ" runat="server" />
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                            </Items>
                        </f:Tab>
                        <f:Tab Title="辅助信息" BodyPadding="5px" runat="server">
                            <Items>
                                <f:Form ID="FormAssist" ShowBorder="false" AutoScroll="false" ShowHeader="False" LabelWidth="90px" runat="server">
                                    <Rows>
                                        <f:FormRow ColumnWidths="22% 22% 22% 22% 12%">
                                            <Items>
                                                <f:TextBox ID="tbxINPER" runat="server" Label="引进人" TabIndex="63" MaxLength="10">
                                                </f:TextBox>
                                                <f:DatePicker ID="dpkINRQ" runat="server" Label="引进日期" TabIndex="53">
                                                </f:DatePicker>
                                                <f:TextBox ID="tbxMANAGER" runat="server" Label="主管人员" TabIndex="6" MaxLength="15">
                                                </f:TextBox>
                                                <f:NumberBox ID="nblJHZQ" runat="server" Label="进货周期" TabIndex="17" NoDecimal="true" MinValue="0" MaxLength="4"></f:NumberBox>
                                                <f:CheckBox ID="ckbISFLAG3" runat="server" Label="直送商品" TabIndex="5" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow ColumnWidths="22% 22% 22% 22% 12%">
                                            <Items>
                                                <f:DatePicker ID="dpkBEGRQ" runat="server" Label="使用日期" TabIndex="71">
                                                </f:DatePicker>
                                                <f:DatePicker ID="dpkENDRQ" runat="server" Label="停用日期" TabIndex="72">
                                                </f:DatePicker>
                                                <f:NumberBox ID="tbxZGKC" runat="server" Label="最高库存" TabIndex="10" DecimalPrecision="4" MinValue="0" MaxLength="14">
                                                </f:NumberBox>
                                                <f:NumberBox ID="tbxZDKC" runat="server" Label="最低库存" TabIndex="11" DecimalPrecision="4" MinValue="0" MaxLength="14">
                                                </f:NumberBox>
                                                <f:CheckBox ID="ckbISIN" runat="server" Label="进口商品" TabIndex="23" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow ColumnWidths="22% 22% 22% 22% 12%">
                                            <Items>
                                                <f:NumberBox ID="tbxHSJ" runat="server" Label="核算价" TabIndex="26" DecimalPrecision="4" MinValue="0" MaxLength="14">
                                                </f:NumberBox>
                                                <f:NumberBox ID="tbxLSJ" runat="server" Label="零售价" TabIndex="24" DecimalPrecision="4" MinValue="0" MaxLength="14">
                                                </f:NumberBox>
                                                <f:NumberBox ID="tbxJXTAX" runat="server" Label="商品税率" TabIndex="27" DecimalPrecision="4" MinValue="0" MaxLength="10">
                                                </f:NumberBox>
                                                <f:NumberBox ID="tbxYBJ" runat="server" Label="医保价" TabIndex="25" DecimalPrecision="4" MinValue="0" MaxLength="14">
                                                </f:NumberBox>
                                                <f:CheckBox ID="ckbISYNZJ" runat="server" Label="是否医保" TabIndex="37" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow ColumnWidths="44% 22% 22% 12%">
                                            <Items>
                                                <f:TextBox ID="tbxNAMEEN" runat="server" Label="英文名" TabIndex="41" MaxLength="100">
                                                </f:TextBox>

                                                <f:TextBox ID="tbxBAR1" runat="server" Label="药监码" TabIndex="34" MaxLength="20">
                                                </f:TextBox>
                                                <f:TextBox ID="tbxYCODE" runat="server" Label="原编码" TabIndex="22">
                                                </f:TextBox>

                                                <f:CheckBox ID="ckbISJG" runat="server" Label="监管药品" TabIndex="8" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow ColumnWidths="44% 22% 22% 12%">
                                            <Items>

                                                <f:NumberBox ID="nbbBARCODE_DABZ" runat="server" Label="大包装条码" TabIndex="83" MaxLength="20" NoDecimal="true"></f:NumberBox>
                                                <f:NumberBox ID="nbxNUM2" runat="server" Label="复用次数" TabIndex="25" DecimalPrecision="0" MinValue="0" MaxLength="14" />

                                                <f:TextBox ID="tbxBAR3" runat="server" Label="ERP编码" TabIndex="23" MaxLength="50" Enabled="false" />
                                                <f:CheckBox ID="ckbISFLAG2" runat="server" Label="复用商品" TabIndex="28" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow ColumnWidths="44% 22% 22% 12%">
                                            <Items>
                                                <f:NumberBox ID="nbbBARCODE_ZHONGBZ" runat="server" Label="中包装条码" TabIndex="93" MaxLength="20" NoDecimal="true"></f:NumberBox>
                                                <f:TextBox ID="tbxBAR2" runat="server" Label="统计码" TabIndex="35" MaxLength="20">
                                                </f:TextBox>
                                                <f:NumberBox ID="nbxMJYXQ" runat="server" Label="灭菌效期" TabIndex="25" DecimalPrecision="0" MinValue="0" MaxLength="14" />

                                                <f:CheckBox ID="ckbISFLAG5" runat="server" Label="是否小数" TabIndex="28" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow ColumnWidths="44% 22% 22% 12%">
                                            <Items>

                                                <f:TextBox ID="tbxCATUSER" runat="server" Label="用户分类" TabIndex="35">
                                                </f:TextBox>
                                                <f:TextBox ID="tbxPPID" runat="server" Label="品牌" TabIndex="20"></f:TextBox>
                                                <f:NumberBox ID="nbxKPYXQ" runat="server" Label="开瓶效期" TabIndex="20" DecimalPrecision="0" MinValue="0" />
                                                <f:CheckBox ID="ckbISJB" runat="server" Label="基药商品" TabIndex="18" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow ColumnWidth="44% 22%">
                                            <Items>
                                                <f:NumberBox ID="nbbBARCODE" runat="server" Label="商品条码" TabIndex="93" MaxLength="20" NoDecimal="true"></f:NumberBox>
                                                <f:CheckBox ID="ckbISFLAG6" runat="server" Label="高值扫描" TabIndex="28" />
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                            </Items>
                        </f:Tab>
                    </Tabs>
                </f:TabStrip>
                <f:TabStrip ID="TabStrip2" Height="270px" ShowBorder="false" TabPosition="Right" EnableFrame="false" AutoPostBack="true"
                    EnableTabCloseMenu="false" ActiveTabIndex="0" CssStyle="border-top: 1px solid #99bce8;"
                    runat="server" Hidden="true">
                    <Tabs>
                        <f:Tab Title="原基本信息" BodyPadding="5px" Layout="Fit" runat="server">
                            <Items>
                                <f:Form ID="Form3" ShowBorder="false" AutoScroll="false" ShowHeader="False" LabelWidth="90px" runat="server">
                                    <Rows>
                                        <f:FormRow ColumnWidths="50% 0% 25% 25%">
                                            <Items>
                                                <f:TextBox ID="txtGDID" runat="server" Label="商品编码" EmptyText="设定后不可更改(为空则自动生成)" TabIndex="1" Enabled="false" ShowRedStar="true" MaxLength="20">
                                                </f:TextBox>
                                                <f:DropDownList ID="dllTYPE" runat="server" Label="商品状态" TabIndex="2" EnableEdit="true" Enabled="false" ForceSelection="true" Hidden="true">
                                                    <f:ListItem Text="新增" Value="N" />
                                                    <f:ListItem Text="修改" Value="M" />
                                                    <f:ListItem Text="删除" Value="D" />
                                                </f:DropDownList>
                                                <f:DropDownList ID="dllCATID" runat="server" Label="商品分类" Required="true" ShowRedStar="true" AutoPostBack="true" ForceSelection="true" TabIndex="3">
                                                </f:DropDownList>
                                                <f:DropDownList ID="dllCATID0" runat="server" Label="商品种类" Required="true" ShowRedStar="true" TabIndex="22">
                                                </f:DropDownList>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:TextBox ID="toxGDNAME" runat="server" Label="商品名称" Required="true" AutoPostBack="true"
                                                    ShowRedStar="true" TabIndex="21" MaxLength="100" />
                                                <f:TextBox ID="txtZJM" runat="server" Label="助记码" TabIndex="4" MaxLength="50">
                                                </f:TextBox>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow ColumnWidths="50% 25% 25%">
                                            <Items>
                                                <f:TextBox ID="txtNAMEJC" runat="server" Label="通用名" Required="true" ShowRedStar="true" TabIndex="31" MaxLength="100" />
                                                <f:DropDownList ID="dllISFLAG7" Label="商品类型" Required="true" ShowRedStar="true" runat="server">
                                                    <f:ListItem Text="--请选择--" Value="" />
                                                    <f:ListItem Text="本地" Value="Y" />
                                                    <f:ListItem Text="下传" Value="N" />
                                                </f:DropDownList>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow ColumnWidths="25% 25% 25% 25%">
                                            <Items>
                                                <f:TextBox ID="txtGDSPEC" runat="server" Label="规格·容量" Required="true" ShowRedStar="true" TabIndex="52" MaxLength="200">
                                                </f:TextBox>
                                                <f:DropDownList ID="dllUNIT" runat="server" Label="包装单位" Required="true" ShowRedStar="true" TabIndex="42" EnableEdit="true" ForceSelection="true">
                                                </f:DropDownList>
                                                <f:CheckBox ID="coxISGZ" runat="server" Label="高值商品" TabIndex="28" />
                                                <f:TextBox ID="txtGDMODE" runat="server" Label="商品型号"></f:TextBox>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow ColumnWidths="25% 25% 25% 25%">
                                            <Items>
                                                <%-- <f:NumberBox ID="numBHSJJ" runat="server" Label="不含税进价" TabIndex="30" DecimalPrecision="6" MinValue="0" MaxLength="12"></f:NumberBox>--%>
                                                <f:NumberBox ID="numHSJJ" runat="server" Label="含税进价" TabIndex="29" DecimalPrecision="6" MinValue="0" MaxLength="12" Required="true" ShowRedStar="true"></f:NumberBox>
                                                <f:DropDownList ID="dllISLOT" runat="server" Label="批号管理" TabIndex="43">
                                                    <f:ListItem Text="不进行" Value="0" />
                                                    <f:ListItem Text="只有入库" Value="1" />
                                                    <f:ListItem Text="全部" Value="2" Selected="true" />
                                                </f:DropDownList>

                                                <%--                                                <f:CheckBox ID="coxISFLAG9" runat="server" Label="计费商品" TabIndex="28" OnCheckedChanged="coxISFLAG9_CheckedChanged" Checked="True" AutoPostBack="True" />
                                                <f:TextBox ID="txtJFDM" runat="server" Label="计费代码" TabIndex="74" MaxLength="50">
                                                </f:TextBox>--%>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow ColumnWidths="50% 25% 25%">
                                            <Items>
                                                <f:DropDownList ID="tbbPRODUCER" Label="生产商" runat="server" Required="true" EnableEdit="true" ForceSelection="true" ShowRedStar="true" TabIndex="61">
                                                </f:DropDownList>
                                                <f:TextBox ID="txtPIZNO" runat="server" Label="注册证号" TabIndex="74" MaxLength="50">
                                                </f:TextBox>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow ColumnWidths="50% 25% 25%">
                                            <Items>
                                                <f:DropDownList ID="tbbSUPPLIER" Label="供应商" runat="server" TabIndex="51" EnableEdit="true" ForceSelection="true" />
                                                <f:DropDownList ID="dllUNIT_DABZ" runat="server" Label="大包装单位" TabIndex="81" EnableEdit="true" ForceSelection="true">
                                                </f:DropDownList>
                                                <f:NumberBox ID="numNUM_DABZ" runat="server" Label="大包装数量" TabIndex="82" MinValue="0" NoDecimal="true" MaxLength="16"></f:NumberBox>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:DropDownList ID="dllUNIT_ZHONGBZ" runat="server" Label="中包装单位" TabIndex="91" EnableEdit="true" ForceSelection="true" Enabled="false">
                                                </f:DropDownList>
                                                <f:NumberBox ID="numNUM_ZHONGBZ" runat="server" Label="中包装数量" TabIndex="92" MinValue="0" NoDecimal="true" MaxLength="16"></f:NumberBox>
                                                <f:DropDownList ID="dllUNIT_ORDER" runat="server" Label="订货单位" TabIndex="84">
                                                    <f:ListItem Text="大包装" Value="D" />
                                                    <f:ListItem Text="中包装" Value="Z" />
                                                    <f:ListItem Text="小包装" Value="X" Selected="true" />
                                                </f:DropDownList>
                                                <f:DropDownList ID="dllUNIT_SELL" runat="server" Label="出库单位" TabIndex="94">
                                                    <f:ListItem Text="大包装" Value="D" />
                                                    <f:ListItem Text="中包装" Value="Z" />
                                                    <f:ListItem Text="小包装" Value="X" Selected="true" />
                                                </f:DropDownList>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow Hidden="true">
                                            <Items>
                                                <f:NumberBox ID="txtBZHL" runat="server" Label="包装含量" Hidden="true" NoDecimal="true" MaxLength="8">
                                                </f:NumberBox>

                                                <f:HiddenField ID="fdhGDSEQ" runat="server" />
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                            </Items>
                        </f:Tab>
                        <f:Tab Title="原辅助信息" BodyPadding="5px" runat="server">
                            <Items>
                                <f:Form ID="Form4" ShowBorder="false" AutoScroll="false" ShowHeader="False" LabelWidth="90px" runat="server">
                                    <Rows>
                                        <f:FormRow ColumnWidths="22% 22% 22% 22% 12%">
                                            <Items>
                                                <f:TextBox ID="txtINPER" runat="server" Label="引进人" TabIndex="63" MaxLength="10">
                                                </f:TextBox>
                                                <f:DatePicker ID="kpdINRQ" runat="server" Label="引进日期" TabIndex="53">
                                                </f:DatePicker>
                                                <f:TextBox ID="txtMANAGER" runat="server" Label="主管人员" TabIndex="6" MaxLength="15">
                                                </f:TextBox>
                                                <f:NumberBox ID="nbxJHZQ" runat="server" Label="进货周期" TabIndex="17" NoDecimal="true" MinValue="0" MaxLength="4"></f:NumberBox>
                                                <f:CheckBox ID="coxISFLAG3" runat="server" Label="直送商品" TabIndex="5" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow ColumnWidths="22% 22% 22% 22% 12%">
                                            <Items>
                                                <f:DatePicker ID="kpdBEGRQ" runat="server" Label="使用日期" TabIndex="71">
                                                </f:DatePicker>
                                                <f:DatePicker ID="kpdENDRQ" runat="server" Label="停用日期" TabIndex="72">
                                                </f:DatePicker>
                                                <f:NumberBox ID="txtZGKC" runat="server" Label="最高库存" TabIndex="10" DecimalPrecision="4" MinValue="0" MaxLength="14">
                                                </f:NumberBox>
                                                <f:NumberBox ID="txtZDKC" runat="server" Label="最低库存" TabIndex="11" DecimalPrecision="4" MinValue="0" MaxLength="14">
                                                </f:NumberBox>
                                                <f:CheckBox ID="coxISIN" runat="server" Label="进口商品" TabIndex="23" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow ColumnWidths="22% 22% 22% 22% 12%">
                                            <Items>
                                                <f:NumberBox ID="txtHSJ" runat="server" Label="核算价" TabIndex="26" DecimalPrecision="4" MinValue="0" MaxLength="14">
                                                </f:NumberBox>
                                                <f:NumberBox ID="txtLSJ" runat="server" Label="零售价" TabIndex="24" DecimalPrecision="4" MinValue="0" MaxLength="14">
                                                </f:NumberBox>
                                                <f:NumberBox ID="txtJXTAX" runat="server" Label="商品税率" TabIndex="27" DecimalPrecision="4" MinValue="0" MaxLength="10">
                                                </f:NumberBox>
                                                <f:NumberBox ID="txtYBJ" runat="server" Label="医保价" TabIndex="25" DecimalPrecision="4" MinValue="0" MaxLength="14">
                                                </f:NumberBox>
                                                <f:CheckBox ID="coxISYNZJ" runat="server" Label="是否医保" TabIndex="37" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow ColumnWidths="44% 22% 22% 12%">
                                            <Items>
                                                <f:TextBox ID="txtNAMEEN" runat="server" Label="英文名" TabIndex="41" MaxLength="100">
                                                </f:TextBox>

                                                <f:TextBox ID="txtBAR1" runat="server" Label="药监码" TabIndex="34" MaxLength="20">
                                                </f:TextBox>
                                                <f:TextBox ID="txtYCODE" runat="server" Label="原编码" TabIndex="22">
                                                </f:TextBox>

                                                <f:CheckBox ID="coxISJG" runat="server" Label="监管药品" TabIndex="8" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow ColumnWidths="44% 22% 22% 12%">
                                            <Items>

                                                <f:NumberBox ID="numBARCODE_DABZ" runat="server" Label="大包装条码" TabIndex="83" MaxLength="20" NoDecimal="true"></f:NumberBox>
                                                <f:NumberBox ID="noxNUM2" runat="server" Label="复用次数" TabIndex="25" DecimalPrecision="0" MinValue="0" MaxLength="14" />

                                                <f:TextBox ID="txtBAR3" runat="server" Label="ERP编码" TabIndex="23" MaxLength="50" Enabled="false" />
                                                <f:CheckBox ID="coxISFLAG2" runat="server" Label="复用商品" TabIndex="28" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow ColumnWidths="44% 22% 22% 12%">
                                            <Items>
                                                <f:NumberBox ID="numBARCODE_ZHONGBZ" runat="server" Label="中包装条码" TabIndex="93" MaxLength="20" NoDecimal="true"></f:NumberBox>
                                                <f:TextBox ID="txtBAR2" runat="server" Label="统计码" TabIndex="35" MaxLength="20">
                                                </f:TextBox>
                                                <f:NumberBox ID="noxMJYXQ" runat="server" Label="灭菌效期" TabIndex="25" DecimalPrecision="0" MinValue="0" MaxLength="14" />

                                                <f:CheckBox ID="coxISFLAG5" runat="server" Label="是否小数" TabIndex="28" />
                                            </Items>
                                        </f:FormRow>

                                        <f:FormRow ColumnWidths="44% 22% 22% 12%">
                                            <Items>

                                                <f:TextBox ID="txtCATUSER" runat="server" Label="用户分类" TabIndex="35">
                                                </f:TextBox>
                                                <f:TextBox ID="txtPPID" runat="server" Label="品牌" TabIndex="20"></f:TextBox>
                                                <f:NumberBox ID="noxKPYXQ" runat="server" Label="开瓶效期" TabIndex="20" DecimalPrecision="0" MinValue="0" />
                                                <f:CheckBox ID="coxISJB" runat="server" Label="基药商品" TabIndex="18" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow ColumnWidth="44% 22%">
                                            <Items>
                                                <f:NumberBox ID="numBARCODE" runat="server" Label="商品条码" TabIndex="93" MaxLength="20" NoDecimal="true"></f:NumberBox>
                                                <f:CheckBox ID="coxISFLAG6" runat="server" Label="高值扫描" TabIndex="28" />
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                            </Items>
                        </f:Tab>
                    </Tabs>
                </f:TabStrip>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar4" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="Button2" Text="追 加" Icon="Add" runat="server" EnableDefaultState="false" OnClick="Button2_Click">
                        </f:Button>
                        <f:Button ID="btnRejectSubmit" Text="确 定" Icon="SystemSave" runat="server" OnClick="btnRejectSubmit_Click" EnableDefaultState="false">
                        </f:Button>
                        <f:Button ID="btnClose" Text="关 闭" EnableDefaultState="false" Icon="SystemClose" runat="server" OnClick="btnClose_Click">
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
                                <f:TextArea ID="txaMemo" runat="server" Label="驳回原因" Height="100px" MaxLength="80" Required="true" ShowRedStar="true" />
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar5" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="Button1" Text="确定" Icon="SystemSave" EnableDefaultState="false" runat="server" OnClick="Button1_Click">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
    </form>
    <script type="text/javascript">
        function btnPrint_Bill() {
            var billState = F('<%= ddlFLAG.ClientID%>').getValue();
            if (billNo == "") {
                F.alert("请选择要打印的单据信息！");
                return;
            }
            if (billState == "M") {
                F.alert("选择单据未提交,不允许打印！");
                return;
            }
            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "/ERPXM/XJ_CJRM/GRF/HQWZSQ.grf?time=" + new Date().getTime();;
            var dataurl = "/captcha/PrintReport.aspx?Method=GetGoodsNewCJ&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
    </script>
</body>
</html>
