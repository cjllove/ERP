﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoodsStorageAndTicket.aspx.cs" Inherits="ERPProject.ERPPayment.GoodsStorageAndTicket" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>发票与入库管理</title>
</head>
<body>

    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel2" EnableAjaxLoading="false" AjaxLoadingType="Mask" runat="server" />
        <f:Panel ID="Panel2" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBox"
            ShowHeader="false">
            <Items>
                <f:Panel ID="Panel3" ShowBorder="false" Height="380px" BodyPadding="0px" ShowHeader="False" runat="server" Layout="VBox">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar2" runat="server">
                            <Items>
                                <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="操作信息：双击显示单据明细！" runat="server" />
                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />

                                <f:Button ID="bntClear" Icon="Erase" Text="清 除" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="bntClear_Click" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="btExport" Icon="DatabaseGo" Text="导 出" EnableDefaultState="false" ConfirmText="是否确认导出商品入库数据?" EnablePostBack="true" runat="server" OnClick="btExport_Click" EnableAjax="false" DisableControlBeforePostBack="false" />
                                <f:ToolbarSeparator runat="server" ID="ListLine" />
                                <f:Button ID="btnSave" Icon="TableSave" Text="录入发票" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnSave_Click" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="bntSearch_Click" />
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Items>
                        <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                            ShowHeader="False" LabelWidth="70px" runat="server">
                            <Rows>
                                <f:FormRow ColumnWidths=" 15% 20%  15%  15%  22% 13%">
                                    <Items>
                                        <f:DropDownList ID="ddlIsTicket" runat="server" Label="发票标识">
                                            <f:ListItem Value="" Text="请选择" Selected="true" />
                                            <f:ListItem Value="1" Text="是" />
                                            <f:ListItem Value="0" Text="否" />
                                        </f:DropDownList>
                                        <f:TextBox ID="txtTicketNo" runat="server" Label="发票编号" />
                                        <f:TextBox ID="txtRKDBillNo" runat="server" Label="入库单号" OnTextChanged="txtRKDBillNo_TextChanged" AutoPostBack="true"></f:TextBox>
                                        <%-- <f:RadioButtonList ID="rblSearchType" runat="server"  ColumnVertical="true" Label="查询方式">
                                             <f:RadioItem Value="0" Text="发票时间" Selected="true" />
                                            <f:RadioItem Value="1" Text="入库时间"  />
                                         </f:RadioButtonList>--%>
                                        <f:DropDownList runat="server" Label="查询方式" ID="ddlSearchType">
                                            <f:ListItem Value="0" Text="发票时间" />
                                            <f:ListItem Value="1" Text="入账时间" />
                                        </f:DropDownList>
                                        <f:DatePicker ID="dptStartDate" runat="server" Label="开始日期" LabelWidth="70px" />
                                        <f:DatePicker ID="dpttEndDate" runat="server" Label="  至 " LabelWidth="40px" />
                                    </Items>
                                </f:FormRow>
                                <f:FormRow ColumnWidths=" 25% 25%  15%  35%">
                                    <Items>
                                        <f:TriggerBox ID="trbGoods" runat="server" Label="商品信息" EmptyText="请输入商品编码、名称、助记码"></f:TriggerBox>
                                        <f:DropDownList ID="ddlDEPTOUT" runat="server" Label="库房" EnableEdit="true" ForceSelection="true"></f:DropDownList>
                                        <f:DropDownList runat="server" Label="单据类别" ID="ddlBILLTYPE">
                                            <f:ListItem Value="" Text="---请选择---" />
                                            <f:ListItem Value="RKD" Text="入库单" />
                                            <f:ListItem Value="THD" Text="退货单" />
                                        </f:DropDownList>
                                        <f:DropDownList ID="ddlSUPID" runat="server" Label="供应商" EnableEdit="true" ForceSelection="true"></f:DropDownList>
                                    </Items>
                                </f:FormRow>
                            </Rows>
                        </f:Form>

                        <f:Grid ID="GridList" BoxFlex="1" ShowBorder="false" ShowHeader="false" EnableColumnLines="true" OnRowDataBound="GridList_RowDataBound"
                            AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableTextSelection="true" EnableCheckBoxSelect="true" PageSize="100" IsDatabasePaging="true"
                            DataKeyNames="SEQNO,FLAG,BILLTYPE" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick" AllowPaging="true" OnPageIndexChange="GridList_PageIndexChange"
                            EnableHeaderMenu="true" SortField="BILLNO" SortDirection="ASC" AllowSorting="true" EnableMultiSelect="true" KeepCurrentSelection="true" OnSort="GridList_Sort">
                            <Columns>
                                <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                <f:BoundField Hidden="true" DataField="SEQNO" />
                                <f:BoundField Hidden="true" DataField="BILLTYPE" />
                                <f:BoundField Width="70px" DataField="TSTATUS" HeaderText="发票标识" SortField="TSTATUS" TextAlign="Center" />
                                <f:BoundField Width="120px" DataField="INVOICENUMBER" HeaderText="发票编号" SortField="TSTATUS" TextAlign="Center" />
                                <f:BoundField Width="120px" DataField="STR1" HeaderText="发票时间" SortField="STR1" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                <f:BoundField Width="120px" DataField="BILLNO" HeaderText="单据编号" SortField="BILLNO" TextAlign="Center" />
                                <f:BoundField Width="0px" Hidden="true" DataField="FLAG" ColumnID="FLAG" HeaderText="单据状态" SortField="FLAG" TextAlign="Center" />
                                <f:BoundField Width="70px" DataField="FLAG_CN" ColumnID="FLAG_CN" HeaderText="单据状态" SortField="FLAG_CN" TextAlign="Center" />
                                <f:BoundField Width="110px" DataField="DDBH" HeaderText="订单编号" SortField="DDBH" TextAlign="Center" />
                                <f:BoundField Hidden="true" DataField="DEPTID" HeaderText="收货地点" SortField="DEPTID" />
                                <f:BoundField Width="130px" DataField="DEPTNAME" HeaderText="收货地点" SortField="DEPTNAME" />
                                <f:BoundField Width="150px" DataField="PSSID" HeaderText="送货商ID" SortField="PSSID" Hidden="true" />
                                <f:BoundField Width="180px" DataField="PSSNAME" HeaderText="送货商名称" SortField="PSSNAME" />
                                <f:BoundField Width="90px" DataField="DHRQ" HeaderText="收货日期" SortField="DHRQ" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                <f:BoundField Width="90px" DataField="SUBSUM" HeaderText="收货金额" SortField="SUBSUM" TextAlign="Right" DataFormatString="{0:F2}" />
                                <f:BoundField Width="200px" DataField="MEMO" HeaderText="备注" SortField="MEMO" />
                            </Columns>
                            <PageItems>
                                <f:ToolbarSeparator ID="ToolbarSeparator1" runat="server">
                                </f:ToolbarSeparator>
                                <f:ToolbarText ID="tbtSUBSUM" runat="server" Text="收货总金额：" CssStyle="color:red;font-weight: bold;">
                                </f:ToolbarText>
                            </PageItems>
                        </f:Grid>
                    </Items>
                </f:Panel>
                <f:Panel ID="PanelN" ShowBorder="false" BodyPadding="0px" ShowHeader="False" runat="server" Layout="Fit" BoxFlex="1">
                    <Items>
                        <f:Grid ID="GridGoods" Height="200px" ShowBorder="false" ShowHeader="false" EnableColumnLines="true"
                            AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;"
                            DataKeyNames="GDSEQ,MEMO" AllowCellEditing="true" ClicksToEdit="1" EnableAfterEditEvent="false"
                            EnableSummary="true" SummaryPosition="Bottom" AllowColumnLocking="true">
                            <Columns>
                                <f:BoundField Width="35px" EnableLock="true" Locked="true" ColumnID="ROWNO" DataField="ROWNO" TextAlign="Center" />
                                <f:BoundField Width="120px" DataField="GDSEQ" ColumnID="GDSEQ" HeaderText="商品编码" EnableLock="true" Locked="true" />
                                <f:BoundField Width="180px" DataField="GDNAME" ColumnID="GDNAME" HeaderText="商品名称" EnableLock="true" Locked="true" />
                                <f:BoundField Width="100px" DataField="GDSPEC" ColumnID="GDSPEC" HeaderText="商品规格" EnableLock="true" Locked="true" />
                                <f:BoundField Width="70px" DataField="BZSL" ColumnID="BZSL" HeaderText="入库数" TextAlign="Center" />
                                <f:BoundField Width="70px" DataField="DDSL" ColumnID="DDSL" HeaderText="订货数" TextAlign="Center" />
                                <f:BoundField Width="90px" ColumnID="UNITNAME" DataField="UNITNAME" HeaderText="入库单位" TextAlign="Center" />
                                <f:BoundField Width="95px" DataField="PH" ColumnID="PH" HeaderText="批号" TextAlign="Center" />
                                <f:BoundField Width="90px" DataField="RQ_SC" ColumnID="RQ_SC" HeaderText="生产日期" TextAlign="Center" />
                                <f:BoundField Width="90px" DataField="YXQZ" ColumnID="YXQZ" HeaderText="有效期至" TextAlign="Center" />
                                <f:BoundField Width="250px" DataField="PZWH" ColumnID="PZWH" HeaderText="注册证号" TextAlign="Center" />
                                <f:BoundField Width="90px" DataField="BZHL" ColumnID="BZHL" HeaderText="包装含量" TextAlign="Center" EnableColumnHide="false" EnableHeaderMenu="false" />
                                <f:BoundField Width="110px" DataField="SSSL" ColumnID="SSSL" HeaderText="入库数(最小单位)" TextAlign="Center" EnableColumnHide="false" EnableHeaderMenu="false" />
                                <f:BoundField Width="110px" DataField="UNITSMALLNAME" ColumnID="UNITSMALLNAME" HeaderText="最小包装单位" TextAlign="Center" EnableHeaderMenu="false" />
                                <f:BoundField Width="70px" DataField="HWID" ColumnID="HWID" HeaderText="货位" TextAlign="Center" />
                                <f:BoundField DataField="JXTAX" ColumnID="JXTAX" HeaderText="税率" TextAlign="Center" Width="0px" Hidden="true" />
                                <f:BoundField Width="90px" DataField="HSJJ" ColumnID="HSJJ" HeaderText="含税进价" TextAlign="Right" DataFormatString="{0:F2}" />
                                <f:BoundField Width="90px" DataField="HSJE" ColumnID="HSJE" HeaderText="含税金额" TextAlign="Right" DataFormatString="{0:F2}" />
                                <f:BoundField Width="180px" ColumnID="PRODUCERNAME" DataField="PRODUCERNAME" EnableHeaderMenu="false" />
                                <f:BoundField Width="95px" DataField="MJPH" ColumnID="MJPH" HeaderText="灭菌批号" TextAlign="Center" />
                                <f:BoundField Width="90px" DataField="MJRQ" ColumnID="MJRQ" HeaderText="灭菌日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                <f:BoundField Width="90px" DataField="MJXQ" ColumnID="MJXQ" HeaderText="灭菌效期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                <f:BoundField Width="0px" Hidden="true" DataField="ZPBH" ColumnID="ZPBH" HeaderText="制品编号" TextAlign="Center" />
                                <f:BoundField Width="100px" DataField="MEMO" ColumnID="MEMO" HeaderText="备注" TextAlign="Center" />
                                <f:BoundField Width="0px" Hidden="true" DataField="BARCODE" ColumnID="BARCODE" HeaderText="商品条码" TextAlign="Center" />
                                <f:BoundField Width="0px" Hidden="true" ColumnID="UNIT" DataField="UNIT" HeaderText="包装单位编码" TextAlign="Center" />
                                <f:BoundField Width="0px" Hidden="true" DataField="ISLOT" ColumnID="ISLOT" HeaderText="批号管理" EnableColumnHide="false" EnableHeaderMenu="false" />
                                <f:BoundField Width="0px" Hidden="true" DataField="ISGZ" ColumnID="ISGZ" HeaderText="是否贵重" EnableColumnHide="false" EnableHeaderMenu="false" />
                                <%--<f:RenderField Width="0px" DataField="CODEINFO" ColumnID="CODEINFO" HeaderText="商品赋码信息" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comCODEINFO" runat="server" />
                                            </Editor>
                                        </f:RenderField>--%>
                                <f:BoundField Width="0px" Hidden="true" ColumnID="SUPID" EnableHeaderMenu="false" HeaderText="供应商" />
                                <f:BoundField Width="0px" Hidden="true" ColumnID="PRODUCER" DataField="PRODUCER" EnableHeaderMenu="false" EnableColumnHide="false"
                                    HeaderText="生产厂家编码" TextAlign="Center" />
                            </Columns>
                            <Listeners>
                                <f:Listener Event="beforeedit" Handler="onGridBeforeEdit" />
                                <f:Listener Event="afteredit" Handler="onGridAfterEdit" />
                            </Listeners>
                        </f:Grid>
                    </Items>
                </f:Panel>
            </Items>
        </f:Panel>
        <f:Window ID="Window2" Title="添加发票信息" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="false" EnableResize="false" IsModal="True" Layout="Fit" Width="480px" Height="120px">
            <Items>
                <f:Form ID="Form3" ShowBorder="false" AutoScroll="false" BodyPadding="5px 10px"
                    ShowHeader="False" LabelWidth="75px" runat="server">
                    <Rows>
                        <f:FormRow ColumnWidth="50% 50%">
                            <Items>
                                <f:TextBox ID="lstINVOICENUMBER" runat="server" Label="发票编号" Required="true" ShowRedStar="true" />
                                <f:DatePicker ID="lstStr1" runat="server" Label="发票时间" LabelSeparator="" Required="true" ShowRedStar="true" />
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar7" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="btnSaveTicket" Text="保存确定" EnableDefaultState="false" Icon="SystemSave" runat="server" OnClick="btnSaveTicket_Click">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
        <f:HiddenField ID="ColorForGridGoods" runat="server"></f:HiddenField>
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

    </form>
    <script type="text/javascript">
        function PosGoodsQt(Info) {
            F('<%= hfdValue.ClientID%>').setValue(Info);
            F.customEvent('GoodsAdd');
        }
        function onGridBeforeEdit(event, value, params) {

      <%--    if (F('<%= docFLAG.ClientID%>').getValue() == "M" && F('<%= hfdOper.ClientID%>').getValue() == "input") {
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
                return false;--%>
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
                    DHSLTotal += me.getCellValue(tr, 'SSSL');
                    HSJETotal += BZSL * HSJJ;
                });
                me.updateSummaryCellValue('GDNAME', "本页合计", true);
                me.updateSummaryCellValue('BZSL', BZSLTotal, true);
                me.updateSummaryCellValue('HSJE', HSJETotal, true);
                me.updateSummaryCellValue('DDSL', DHSLTotal, true);
            }
        }
    </script>
</body>
</html>

