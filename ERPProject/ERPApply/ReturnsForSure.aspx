﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReturnsForSure.aspx.cs" Inherits="ERPProject.ERPApply.ReturnsForSure" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>科室申退确定</title>
        <style type="text/css" media="all">
        .ColBlue {
            font-size: 12px;
            color: blue;
        }

        .x-grid-row-summary .x-grid-cell-inner {
            font-weight: bold;
            color: blue;
        }

        .x-grid-row-summary .x-grid-cell {
            background-color: #fff !important;
        }
    </style>
</head>
<body>
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
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="bntClear" Icon="Erase" Text="清 除" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
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
                                                        <f:DropDownList ID="lstDEPTID" runat="server" Label="退货科室" EnableEdit="true" ForceSelection="true"/>
                                                        <f:DropDownList ID="lstFLAG" runat="server" Label="单据状态" EnableEdit="true" ForceSelection="true"/>
                                                        <f:DropDownList ID="lstSLR" runat="server" Label="退货人" EnableEdit="true" ForceSelection="true"/>
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:HiddenField ID="hfd" runat="server" />
                                                        <f:DropDownList ID="lstDEPTOUT" runat="server" Label="收货部门" EnableEdit="true" ForceSelection="true"/>
                                                        <f:DatePicker ID="lstLRRQ1" runat="server" Label="录入日期" />
                                                        <f:DatePicker ID="lstLRRQ2" runat="server" Label="　　至" LabelSeparator="" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" AnchorValue="100% -95" ShowBorder="false" ShowHeader="false"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;"
                                    DataKeyNames="SEQNO" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField DataField="SEQNO" Hidden="true" />
                                        <f:BoundField Width="100px" DataField="BILLNO" HeaderText="单据编号" TextAlign="Center" />
                                        <f:BoundField Width="60px" DataField="FLAG" HeaderText="单据状态" TextAlign="Center" />
                                        <f:BoundField Width="150px" DataField="DEPTOUT" HeaderText="出库部门" TextAlign="Center" />
                                        <f:BoundField Width="150px" DataField="DEPTID" HeaderText="申领科室" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="XSRQ" HeaderText="申领日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="70px" DataField="SUBNUM" HeaderText="商品种类" TextAlign="Right" DataFormatString="{0:F2}" />
                                        <f:BoundField Width="70px" DataField="SLR" HeaderText="申领人" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="LRY" HeaderText="录入员" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="LRRQ" HeaderText="录入日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="70px" DataField="SHR" HeaderText="审核员" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="SHRQ" HeaderText="审核日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="100px" DataField="MEMO" HeaderText="备注" TextAlign="Center" />
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
                                                <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="操作信息：还没有写好哟！" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="btnNew" Icon="PageAdd" Text="新 增" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnDel" Icon="PageCancel" Text="删 除" EnablePostBack="true" ConfirmText="是否确认删除此单据?" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnSave" Icon="Disk" Text="保 存" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnAudit" Icon="UserTick" Text="审 核" EnablePostBack="true" runat="server" ConfirmText="是否确认审核此单据?" OnClick="btnBill_Click" />
                                                <f:Button ID="btnPrint" Icon="Printer" Text="打 印" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnCopy" Icon="PageCopy" Text="复 制" EnablePostBack="true" ConfirmText="是否确认复制此单据信息?" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" EnablePostBack="true" ConfirmText="是否确认导出此单据信息?" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnAddRow" Icon="Add" Text="增 行" runat="server" />
                                                <f:Button ID="btnDelRow" Icon="Delete" Text="删 行" EnablePostBack="true" ConfirmText="是否确认删除选中行信息?" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnNext" Icon="ForwardBlue" Text="下 页" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnBef" Icon="RewindBlue" Text="上 页" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />

                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnGoods" Icon="Magnifier" Text="商 品" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
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
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="docDEPTOUT" runat="server" Label="出库部门" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true"/>
                                                        <f:DropDownList ID="docSLR" runat="server" Label="申领人" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true"/>
                                                        <f:DropDownList ID="docFLAG" runat="server" Label="单据状态" Enabled="false" EnableEdit="true" ForceSelection="true"/>
                                                        <f:TextBox ID="docBILLNO" runat="server" Label="单据编号" Enabled="false" />

                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="docDEPTID" runat="server" Label="收货科室" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true"/>
                                                        <f:DatePicker ID="docXSRQ" runat="server" Label="申领日期" />
                                                        <f:DropDownList ID="docLRY" runat="server" Label="录入员" Enabled="false" EnableEdit="true" ForceSelection="true"/>
                                                        <f:DatePicker ID="docLRRQ" runat="server" Label="录入日期" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridGoods" AnchorValue="100% -95" ShowBorder="false" ShowHeader="false" EnableSummary="true" SummaryPosition="Bottom"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;"
                                    DataKeyNames="GDSEQ" AllowCellEditing="true" ClicksToEdit="1" EnableAfterEditEvent="true" OnAfterEdit="GridGoods_AfterEdit">
                                    <Columns>
                                        <f:RowNumberField Width="30px"></f:RowNumberField>
                                        <f:BoundField Width="70px" DataField="GDSEQ" Hidden="true" />
                                        <f:RenderField Width="105px" ColumnID="BARCODE" DataField="BARCODE" FieldType="String"
                                            HeaderText="商品编码">
                                            <Editor>
                                                <f:TextBox ID="tbxEditorCODE" Required="true" runat="server">
                                                </f:TextBox>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" ColumnID="NAME" DataField="NAME" FieldType="String"
                                            HeaderText="商品名称" EnableLock="true" Locked="true">
                                            <Editor>
                                                <f:Label ID="lblEditorName" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="GDSPEC" DataField="GDSPEC" FieldType="String"
                                            HeaderText="商品规格">
                                            <Editor>
                                                <f:Label ID="lblEditorGDSPEC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="UNIT" DataField="UNIT" FieldType="String"
                                            HeaderText="包装单位" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorUNIT" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="BZHL" DataField="BZHL" FieldType="String"
                                            HeaderText="包装含量" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorBZHL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:BoundField Width="70px" DataField="DHSL" HeaderText="申退数" TextAlign="Center" />
                                        <f:RenderField Width="70px" ColumnID="XSSL" DataField="XSSL" FieldType="Int"
                                            HeaderText="实收数">
                                            <Editor>
                                                <f:NumberBox ID="nbxXSSL" runat="server" MinValue="0" NoDecimal="true" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:BoundField Width="90px" DataField="JXTAX" HeaderText="税率" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="HSJJ" HeaderText="含税进价" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="HSJE" HeaderText="含税金额" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="ZPBH" HeaderText="制品编号" TextAlign="Center" />
                                        <f:BoundField Width="180px" DataField="CDID" HeaderText="产地" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="HWID" HeaderText="货位" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="PH" HeaderText="批号" Hidden="true" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="PZWH" HeaderText="注册证号" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="RQ_SC" HeaderText="生产日期" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="YXQZ" HeaderText="有效期至" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="MEMO" HeaderText="备注" TextAlign="Center" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
        <f:HiddenField ID="hfdValue" runat="server" />
        <f:Window ID="Window1" Title="商品信息" Hidden="true" EnableIFrame="true" runat="server"
            EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True"
            Width="820px" Height="480px" OnClose="Window1_Close">
        </f:Window>
    </form>
</body>
</html>
