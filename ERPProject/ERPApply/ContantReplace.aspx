﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ContantReplace.aspx.cs" Inherits="ERPProject.ERPApply.ContantReplace" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>商品替代管理</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="TabStrip1"
            runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="1" runat="server">
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
                                        <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="85px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TextBox ID="lstBILLNO" runat="server" Label="单据编号" MaxLength="15" />
                                                        <f:TextBox ID="lstGDSEQ_OLD" runat="server" Label="原商品信息" MaxLength="15"></f:TextBox>
                                                        <f:TextBox ID="lstGDSEQ" runat="server" Label="商品信息" MaxLength="15"></f:TextBox>
                                                        <f:DropDownList ID="lstLRY" runat="server" Label="录入员" EnableEdit="true" ForceSelection="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="lstTYPE" runat="server" Label="调整类型" EnableEdit="true" ForceSelection="true">
                                                            <f:ListItem Text="--请选择--" Value="" Selected="true" />
                                                            <f:ListItem Text="[1]品种切换" Value="1" />
                                                            <f:ListItem Text="[2]一品多码" Value="2" Selected="true" />
                                                        </f:DropDownList>
                                                        <f:DropDownList ID="lstFLAG" runat="server" Label="单据状态" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="lstLRRQ1" runat="server" Label="申请日期" Required="true" ShowRedStar="true" />
                                                        <f:DatePicker ID="lstLRRQ2" runat="server" Label="　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" AnchorValue="100% -121" ShowBorder="false" ShowHeader="false"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;"
                                    DataKeyNames="SEQNO,FLAG" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick" OnRowDataBound="GridList_RowDataBound"
                                    EnableColumnLines="true">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField DataField="SEQNO" Hidden="true" />
                                        <f:BoundField Width="100px" DataField="BILLNO" HeaderText="单据编号" TextAlign="Center" />
                                        <f:BoundField DataField="FLAG" Hidden="true" />
                                        <f:BoundField Width="60px" DataField="FLAGNAME" ColumnID="FLAGNAME" HeaderText="单据状态" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="TYPENAME" HeaderText="调整类型" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="GDSEQ_OLD" HeaderText="原商品编码" TextAlign="Center" />
                                        <f:BoundField Width="150px" DataField="GDNAME_OLD" HeaderText="原商品名称" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                                        <f:BoundField Width="150px" DataField="GDNAME" HeaderText="商品名称" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="SUBNUM" HeaderText="明细条数" TextAlign="Center" />
                                        <f:BoundField Width="60px" DataField="LRYNAME" HeaderText="录入员" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="LRRQ" HeaderText="录入日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="60px" DataField="SHRNAME" HeaderText="审核员" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="SHRQ" HeaderText="审核日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="180px" DataField="MEMO" HeaderText="备注" ExpandUnusedSpace="true" />
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
                                                <f:Button ID="btnDel" Icon="PageCancel" Text="整单删除" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认删除此单据?" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnSave" Icon="Disk" Text="保 存" EnableDefaultState="false" EnablePostBack="true" runat="server" ValidateForms="FormDoc" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="ButSubmit" Icon="UserTick" Text="提 交" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认提交此单据?" runat="server" OnClick="ButSubmit_Click" />
                                                <f:Button ID="btnAudit" Icon="UserTick" Text="审 核" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认审核此单据?" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnCancel" Icon="UserCross" Text="驳 回" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnPrint" Icon="Printer" Text="打 印" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" Hidden="true" />
                                                <f:Button ID="btnCopy" Icon="PageCopy" Text="复 制" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认复制此单据信息?" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认导出此单据信息?" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnAddRow" Icon="Add" Text="增 行" EnableDefaultState="false" runat="server" ValidateForms="FormDoc" OnClick="btnBill_Click" Hidden="true" />
                                                <f:Button ID="btnDelRow" Icon="Delete" Text="删 行" EnableDefaultState="false" EnablePostBack="true" runat="server" ValidateForms="FormDoc" ConfirmText="是否确认删除选中行?" OnClick="btnBill_Click" />
                                                <f:Button ID="btnNext" Icon="ForwardBlue" Text="下 页" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnBef" Icon="RewindBlue" Text="上 页" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnGoods" Icon="Magnifier" Text="追加科室" EnableDefaultState="false" EnablePostBack="true" runat="server" ValidateForms="FormDoc" OnClick="btnBill_Click" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                </f:Panel>
                                <f:Panel ID="PanelBody" ShowBorder="false" BodyPadding="0px" RegionSplit="true" EnableCollapse="true" RegionPosition="Top"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Items>
                                        <f:Form ID="FormDoc" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="100px" runat="server">
                                            <Rows>
                                                <f:FormRow Hidden="true">
                                                    <Items>
                                                        <f:TextBox ID="docBILLNO" runat="server" Label="单据编号" Hidden="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="ddlTYPE" runat="server" Label="调整类型" ShowRedStar="true" Required="true">
                                                            <f:ListItem Text="[1]品种切换" Value="1" />
                                                            <f:ListItem Text="[2]一品多码" Value="2" Selected="true" />
                                                        </f:DropDownList>
                                                        <f:DropDownList ID="docSLR" runat="server" Label="申请人" ShowRedStar="true" Required="true" EnableEdit="true" ForceSelection="true" />
                                                        <f:TextBox ID="docSEQNO" runat="server" Label="单据编号" MaxLength="20" EmptyText="无需填写" />
                                                        <f:DropDownList ID="docFLAG" runat="server" Label="单据状态" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow ColumnWidths="50% 25% 25%">
                                                    <Items>
                                                        <f:TextBox runat="server" Label="调整原因" ShowRedStar="true" ID="tbxREASON" Required="true" MaxLength="80"></f:TextBox>
                                                        <f:DropDownList ID="docLRY" runat="server" Label="录入员" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="docLRRQ" runat="server" Label="录入日期" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow ColumnWidths="25% 25% 25% 25%">
                                                    <Items>
                                                        <f:TextBox ID="docMEMO" runat="server" Label="备注说明" MaxLength="80" />
                                                        <f:DatePicker ID="docSCDDATE" runat="server" Label="完成日期" Enabled="false" />
                                                        <f:DropDownList ID="docSHR" runat="server" Label="审核人" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="docSHRQ" runat="server" Label="审核日期" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow ColumnWidths="25% 25% 25% 25%">
                                                    <Items>
                                                        <f:TriggerBox ID="tbxGDSEQ_OLD" OnTriggerClick="tbxGDSEQ_OLD_TriggerClick" runat="server" Label="原商品编码" EmptyText="请输入原商品编码" MaxLength="30" ShowRedStar="true" Required="true">
                                                        </f:TriggerBox>
                                                        <f:TextBox ID="tbxGDNAME_OLD" runat="server" Label="原商品名称" Enabled="false" Required="true"></f:TextBox>
                                                        <f:TriggerBox ID="tbxGDSEQ" OnTriggerClick="tbxGDSEQ_TriggerClick" runat="server" Label="商品编码" MaxLength="80" ShowRedStar="true" Required="true" EmptyText="请输入商品编码">
                                                        </f:TriggerBox>
                                                        <f:TextBox ID="tbxGDNAME" runat="server" Label="商品名称" Enabled="false" Required="true"></f:TextBox>
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow ColumnWidths="25% 25% 25% 25%">
                                                    <Items>
                                                        <f:DropDownList ID="ddlUNIT_OLD" runat="server" Label="原单位" Enabled="false"></f:DropDownList>
                                                        <f:TextBox ID="tbxGDSPEC_OLD" runat="server" Label="原规格" Enabled="false"></f:TextBox>
                                                        <f:DropDownList ID="ddlUNIT" runat="server" Label="单位" Enabled="false"></f:DropDownList>
                                                        <f:TextBox ID="tbxGDSPEC" runat="server" Label="规格" Enabled="false"></f:TextBox>
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow ColumnWidths="25% 25% 25% 25%">
                                                    <Items>
                                                        <f:TextBox ID="tbxHSJJ_OLD" runat="server" Label="原价格" Enabled="false"></f:TextBox>
                                                        <f:DropDownList ID="ddlPRODUCER_OLD" runat="server" Label="原生产厂家" Enabled="false"></f:DropDownList>
                                                        <f:TextBox ID="tbxHSJJ" runat="server" Label="价格" Enabled="false"></f:TextBox>
                                                        <f:DropDownList ID="ddlPRODUCER" runat="server" Label="生产厂家" Enabled="false"></f:DropDownList>
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TextBox ID="tbxPZWH_OLD" runat="server" Label="旧注册证号" Enabled="false"></f:TextBox>
                                                        <f:TextBox ID="tbxPZWH" runat="server" Label="注册证号" Enabled="false"></f:TextBox>
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridGoods" AnchorValue="100% -265" ShowBorder="false" ShowHeader="false" EnableTextSelection="true"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" AllowColumnLocking="true"
                                    DataKeyNames="DEPTID,DEPTIDNAME" AllowCellEditing="true" ClicksToEdit="1">
                                    <Columns>
                                        <f:RenderField Width="35px" ColumnID="ROWNO" DataField="ROWNO" FieldType="String" Locked="true" EnableLock="true" TextAlign="Center">
                                            <Editor>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="DEPTID" DataField="DEPTID" FieldType="String" Hidden="true" Locked="true" EnableLock="true"
                                            HeaderText="科室编码" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comDEPTID" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="220px" ColumnID="DEPTIDNAME" DataField="DEPTIDNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="科室名称" EnableLock="true" Locked="true">
                                            <Editor>
                                                <f:Label ID="lblEditorName" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <%--<f:RenderField Width="80px" ColumnID="DSSL_OLD" DataField="DSSL_OLD" FieldType="String" EnableHeaderMenu="false" TextAlign="Center"
                                            HeaderText="原定数数量">
                                            <Editor>
                                                <f:Label ID="comDSSL_OLD" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="DSHL_OLD" DataField="DSHL_OLD" FieldType="String" EnableHeaderMenu="false" TextAlign="Center"
                                            HeaderText="原定数含量">
                                            <Editor>
                                                <f:Label ID="comDSHL_OLD" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="WSSL" DataField="WSSL" FieldType="String" EnableHeaderMenu="false" TextAlign="Center"
                                            HeaderText="未回收定数">
                                            <Editor>
                                                <f:Label ID="comWSSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="DSHL" DataField="DSHL" FieldType="String" EnableHeaderMenu="false" TextAlign="Center"
                                            HeaderText="定数含量">
                                            <Editor>
                                                <f:NumberBox runat="server" NoDecimal="true" ID="nbxDSHL" MinValue="1" MaxLength="5"></f:NumberBox>
                                            </Editor>
                                        </f:RenderField>--%>
                                        <f:RenderField Width="90px" ColumnID="FLAGNAME" DataField="FLAGNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="执行状态" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="comFLAGNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="220px" ColumnID="REASON" DataField="REASON" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="情况说明" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="comREASON" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField ColumnID="MEMO" DataField="MEMO" FieldType="String" EnableHeaderMenu="false" ExpandUnusedSpace="true" MinWidth="150px"
                                            HeaderText="备注<font color='red'>*</font>" TextAlign="Center">
                                            <Editor>
                                                <f:TextBox ID="comMEMO" runat="server" MaxLength="80" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="DSSL" DataField="DSSL" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="定数数量">
                                            <Editor>
                                                <f:Label ID="comDSSL" runat="server" />
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
        <f:Window ID="WindowGoods" Title="科室信息" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="680px" Height="350px">
            <Items>
                <f:Panel ID="PanelDetail" ShowBorder="false" AnchorValue="100% -300px" BodyPadding="0px" Layout="HBox"
                    BoxConfigAlign="Stretch" BoxConfigPosition="Start" ShowHeader="False" runat="server">
                    <Items>
                        <f:Panel ID="Panel4" BoxFlex="2" runat="server" BodyPadding="0" ShowBorder="false" ShowHeader="false" CssStyle="border-right: 1px solid #99bce8;border-bottom: 1px solid #99bce8;"
                            Layout="VBox" BoxConfigAlign="Stretch" BoxConfigPosition="Start" BodyStyle="background-color:#d9e7f8;">
                            <Items>
                                <f:Grid ID="GridDs" ShowBorder="false" ShowHeader="false" AllowSorting="false" EnableCheckBoxSelect="true" EnableMultiSelect="true" KeepCurrentSelection="true"
                                    BoxFlex="1" AutoScroll="true" runat="server" DataKeyNames="DEPTID,DEPTIDNAME,DSSL,DSHL,WSSL,REASON" AnchorValue="100%" EnableColumnLines="true" EnableRowDoubleClickEvent="true">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar5" runat="server">
                                            <Items>
                                                <f:ToolbarFill ID="ToolbarFill3" runat="server" />
                                                <f:TriggerBox ID="tgbExlGoods" EmptyText="请输入科室编码或名称" Width="220px" runat="server" TriggerIcon="Search" Label="科室信息" LabelWidth="75px" OnTriggerClick="btnExlSch_Click"></f:TriggerBox>
                                                <f:ToolbarSeparator runat="server"></f:ToolbarSeparator>
                                                <f:Button ID="btnExlSch" runat="server" Text="查 询" EnableDefaultState="false" Icon="SystemSearch" OnClick="btnExlSch_Click"></f:Button>
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Columns>
                                        <f:RowNumberField runat="server"></f:RowNumberField>
                                        <f:BoundField Width="180px" DataField="DEPTIDNAME" HeaderText="科室名称" />
                                        <f:BoundField Width="80px" DataField="DSSL" HeaderText="定数数量" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="DSHL" HeaderText="定数含量" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="WSSL" HeaderText="未回收定数" TextAlign="Center" />
                                        <f:BoundField Width="160px" DataField="REASON" HeaderText="情况说明" />
                                        <f:BoundField Width="0px" Hidden="true" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                                        <f:BoundField Width="0px" Hidden="true" DataField="DEPTID" HeaderText="科室编码" TextAlign="Center" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Panel>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar3" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="btnClosePostBack" Text="确定" EnableDefaultState="false" Icon="SystemSave" runat="server" OnClick="btnClosePostBack_Click">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
    </form>
</body>
</html>
