﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="XSSearch.aspx.cs" Inherits="ERPProject.ERPQuery.XSSearch" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>

    <script src="../res/js/CreateControl.js" type="text/javascript"></script>
    <script src="../res/js/GRInstall.js" type="text/javascript"></script>
    <script src="../res/js/jquery.ymh.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="TabStrip1" runat="server" />
           <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="0" runat="server">
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
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="bntClear" Icon="Erase" Text="清 空" EnableDefaultState="false" EnablePostBack="true" runat="server"  />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnExp" runat="server" CssStyle="margin-left: 10px;" Icon="PageExcel" EnableAjax="false" EnableDefaultState="false"
                                                    OnClick="btExport_Click" EnablePostBack="true" Text="导出全部" ConfirmText="是否确认导出此科室申领信息?" DisableControlBeforePostBack="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                    <f:Button ID="bntSearch" Icon="Magnifier" EnableDefaultState="false" Text="查 询" EnablePostBack="true" runat="server"  ValidateForms="Formlist" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DatePicker ID="lstBEGRQ" runat="server" Label="查询期间" Required="true" ShowRedStar="true" />
                                                        <f:DatePicker ID="lstENDRQ" runat="server" Label="　　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                                                        <f:DropDownList ID="ddlDEPTIDZ" runat="server" Label="查询科室" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="ddlISGZZ" runat="server" Label="是否高值" >
                                                            <f:ListItem  Text="--请选择--" Value=""/>
                                                            <f:ListItem Text="是" Value="Y" />
                                                            <f:ListItem Text="否" Value="N" />
                                                        </f:DropDownList>
                                                    </Items>
                                                    </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                    <f:DropDownList ID="ddlSupplierZ" runat="server" Label="供应商"></f:DropDownList>
                                                    <f:DropDownList ID="ddlBilltypeZ" runat="server" Label="单据类型">
                                                        <f:ListItem Text="--请选择--" Value="" />
                                                        <f:ListItem Text="科室使用单" Value="XSD" />
                                                        <f:ListItem Text="科室高值使用单" Value="XSG" />
                                                        <f:ListItem Text="定数出退货单" Value="DST" />
                                                        <f:ListItem Text="定数出库单" Value="DSC" />
                                                        <f:ListItem Text="销售退货单" Value="XST" />
                                                    </f:DropDownList>
                                                        <f:Label runat="server"></f:Label>     
                                                                                                           <f:Label runat="server"></f:Label>

                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" AnchorValue="100% -138" ShowBorder="false" ShowHeader="false" EnableCheckBoxSelect="true" EnableSummary="true" SummaryPosition="Bottom"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" AllowPaging="true" IsDatabasePaging="true" OnPageIndexChange="GridList_PageIndexChange"
                                    DataKeyNames="SEQNO,FLAG" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick" EnableTextSelection="true"
                                     EnableColumnLines="true" EnableMultiSelect="true"
                                    AllowSorting="true" EnableHeaderMenu="true" SortField="FLAG,BILLNO" OnSort="GridList_Sort" SortDirection="ASC" KeepCurrentSelection="true">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField DataField="SEQNO" Hidden="true" />
                                        <f:BoundField DataField="DEPTID" HeaderText="科室编码"></f:BoundField>
                                        <f:BoundField DataField="DEPTNAME" HeaderText="科室名称"></f:BoundField>
                                        <f:BoundField DataField="SL" HeaderText="数量"></f:BoundField>
                                        <f:BoundField DataField="HSJE" HeaderText="金额"></f:BoundField>
                                        <f:BoundField DataField="SUPNAME" HeaderText="供应商"></f:BoundField>
                                        <f:BoundField MinWidth="120px" ExpandUnusedSpace="true" DataField="MEMO" HeaderText="备注" SortField="MEMO" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                 
                <f:Tab Title="单据信息" Icon="Table" Layout="Fit" runat="server">
         <Items>
                       <f:Panel ID="Panel1" runat="server" ShowBorder="False" BodyPadding="0px" Layout="Anchor"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="PanelBody" ShowBorder="false" BodyPadding="0px"
                                        Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar1" runat="server">
                                            <Items>
                                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                <f:Button ID="btnAudit" Icon="ArrowRotateClockwise" Text="清 空" OnClick="btClear_Click" EnablePostBack="true" runat="server" EnableDefaultState="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <%--<f:Button ID="btnPrint" Icon="Printer" Text="打 印" DisableControlBeforePostBack="true" runat="server" OnClientClick="btnPrint_onclick()" EnableDefaultState="false" />--%>
                                                <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" OnClick="btExport_Click" ConfirmText="是否导出当前信息?" DisableControlBeforePostBack="false" EnableDefaultState="false"
                                                    EnablePostBack="true" EnableAjax="false" EnableAjaxLoading="true" AjaxLoadingType="Mask" runat="server" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" OnClick="btSearch_Click" runat="server" EnableDefaultState="false" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="5px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DatePicker ID="lstLRRQ1" runat="server" Label="查询期间" Required="true" ShowRedStar="true" />
                                                        <f:DatePicker ID="lstLRRQ2" runat="server" Label="　　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                                                        <f:DropDownList ID="ddlDEPTID" runat="server" Label="查询科室" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="ddlISGZ" runat="server" Label="是否高值" >
                                                            <f:ListItem  Text="--请选择--" Value=""/>
                                                            <f:ListItem Text="是" Value="Y" />
                                                            <f:ListItem Text="否" Value="N" />
                                                        </f:DropDownList>
                                                    </Items>
                                                    </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                    <f:TriggerBox ID="tbxGOODS" runat="server" Label="查询商品" EmptyText="可输入编码、名称或助记码" TriggerIcon="Search" OnTriggerClick="btSearch_Click"></f:TriggerBox>
                                                    <f:DropDownList ID="ddlSupplier" runat="server" Label="供应商"></f:DropDownList>
                                                    <f:DropDownList ID="ddlBilltype" runat="server" Label="单据类型">
                                                        <f:ListItem Text="--请选择--" Value="" />
                                                        <f:ListItem Text="科室使用单" Value="XSD" />
                                                        <f:ListItem Text="科室高值使用单" Value="XSG" />
                                                        <f:ListItem Text="定数出退货单" Value="DST" />
                                                        <f:ListItem Text="定数出库单" Value="DSC" />
                                                        <f:ListItem Text="销售退货单" Value="XST" />
                                                    </f:DropDownList>
                                                        <f:Label runat="server"></f:Label>
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridGoods" AnchorValue="100% -108px" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;" 
                                    AllowSorting="true" AutoScroll="true" runat="server" DataKeyNames="GDSEQ" IsDatabasePaging="true" AllowPaging="true" SummaryPosition="Bottom" EnableSummary="true"
                                    PageSize="100" OnPageIndexChange="GridGoods_PageIndexChange" OnSort="GridGoods_Sort"  EnableColumnLines="true" EnableTextSelection="true">
                                    <Columns>
                                        <f:RowNumberField Width="30px" TextAlign="Center" />
                                        <f:BoundField Width="115px" DataField="RQSJ" SortField="RQSJ" HeaderText="时间" />
                                        <f:BoundField Width="115px" DataField="BILLNO" SortField="BILLNO" HeaderText="单据编号" />
                                        <f:BoundField Width="110px" DataField="BILLTYPE" SortField="BILLTYPE" HeaderText="单据类型" />
                                        <f:BoundField Width="50px" DataField="ROWNO" SortField="ROWNO" HeaderText="行号"  TextAlign="Center"/>
                                        <f:BoundField Width="100px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                                        <f:BoundField Width="185px" DataField="GDNAME" SortField="GDNAME" ColumnID="GDNAME" HeaderText="商品名称" />
                                        <f:BoundField Width="150px" DataField="GDSPEC" SortField="GDSPEC" HeaderText="规格·容量" />
                                        <f:BoundField Width="45px" DataField="UNIT" SortField="UNIT" HeaderText="单位" TextAlign="Center" />
                                        <f:BoundField Width="130px" DataField="DEPTNAME" SortField="DEPTNAME" HeaderText="科室名称" />
                                        <f:BoundField Width="220px" DataField="SUPNAME" SortField="SUPNAME" HeaderText="供应商"/>
                                        <f:BoundField Width="220px" DataField="PSSNAME" SortField="PSSNAME" HeaderText="配送商"/>
                                        <f:BoundField Width="80px" DataField="HSJJ" SortField="HSJJ" HeaderText="含税进价" DataFormatString="{0:F4}" TextAlign="Right" />
                                        <f:BoundField Width="80px" DataField="SL" SortField="SL" ColumnID="SL" HeaderText="数量" TextAlign="Right" />
                                        <f:BoundField Width="80px" DataField="JE" SortField="JE" ColumnID="JE" HeaderText="金额" DataFormatString="{0:F2}" TextAlign="Right" />
                                        <f:BoundField Width="120px" DataField="PH" SortField="PH" HeaderText="批号" />
                                        <f:BoundField Width="120px" DataField="PZWH" SortField="PZWH" HeaderText="批准文号" />
                                        <f:BoundField Width="75px" DataField="RQ_SC" SortField="RQ_SC" HeaderText="生产日期" />
                                        <f:BoundField Width="75px" DataField="YXQZ" SortField="YXQZ" HeaderText="有效期至" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
     </Items>
                </f:Tab>
                            </Tabs>
        </f:TabStrip>
     </form>
    <script type="text/javascript">
        <%--function btnPrint_onclick() {
            if (F('<%= lstLRRQ1.ClientID%>').lastValue > F('<%= lstLRRQ2.ClientID%>').lastValue) {
                F.util.alert('开始日期大于结束日期', F.util.formAlertTitle, Ext.MessageBox.INFO);
                return;
            }

            var deptid = F('<%= ddlDEPTID.ClientID%>').getValue();
            var gdseq = F('<%= tbxGOODS.ClientID%>').getValue();
            var begrq = F('<%= lstLRRQ1.ClientID%>').getRawValue();
            var endrq = F('<%= lstLRRQ2.ClientID%>').getRawValue();

            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "/grf/kfckdhz.grf?201601062022";
            var dataurl = "/captcha/PrintReport.aspx?Method=GetStockOutData&deptid=" + deptid + "&gdseq=" + gdseq + "&b=" + begrq + "&e=" + endrq;
            if (deptid != "") {
                Report.ParameterByName("DEPT").AsString = F('<%= ddlDEPTID.ClientID%>').getRawValue();
            }
            else {
                Report.ParameterByName("DEPT").AsString = "全部出库科室";
            }

            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }--%>
    </script>
</body>
</html>