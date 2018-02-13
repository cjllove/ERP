<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MonthPurchase.aspx.cs" Inherits="ERPProject.ERPStorage.MonthPurchase" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <script src="../res/js/CreateControl.js" type="text/javascript"></script>
    <script src="../res/js/GRInstall.js" type="text/javascript"></script>
    <title>月度采购管理</title>
</head>
<body>
    <div style="width: 0px; height: 0px">
        <script type="text/javascript">
            CreateDisplayViewerEx("0%", "0%", "", "", false, "");
        </script>
    </div>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="TabStrip1" runat="server" OnCustomEvent="PageManager1_CustomEvent" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="1" runat="server">
            <Tabs>
                <f:Tab Title="月度采购查询" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel2" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBox"
                            ShowHeader="false">
                            <Toolbars>
                                <f:Toolbar ID="Toolbar2" runat="server">
                                    <Items>
                                        <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="操作信息：双击打开单据明细！" runat="server" />
                                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                        <f:Button ID="bntClear" Icon="Erase" Text="清 空" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnClear_Click" />
                                        <f:Button ID="btnAuditBatch" Icon="UserTick" Text="审 批" EnableDefaultState="false" Hidden="true" EnablePostBack="true" runat="server" OnClick="btnAuditBatch_Click" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnMySerarch_Click" />
                                    </Items>
                                </f:Toolbar>
                            </Toolbars>
                            <Items>
                                <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 5px 5px 5px"
                                    ShowHeader="False" LabelWidth="80px" runat="server">
                                    <Rows>
                                        <f:FormRow ColumnWidths="33% 33% 34%">
                                            <Items>
                                                <f:TextBox ID="lstBILLNO" runat="server" Label="单据编号" MaxLength="15" />
                                                <f:DropDownList ID="lstFLAG" runat="server" Label="单据状态" EnableEdit="true" ForceSelection="true" />
                                                <f:DropDownList ID="lstDEPTID" runat="server" Label="订货部门" EnableEdit="true" ForceSelection="true" />
                                                <f:TriggerBox ID="tbxJHYF" runat="server" Label="计划月份" TriggerIcon="Date" ShowRedStar="true" EnableEdit="true"></f:TriggerBox>
                                            </Items>
                                        </f:FormRow>

                                    </Rows>
                                </f:Form>
                                <f:Grid ID="GridList" BoxFlex="1" ShowBorder="false" ShowHeader="false" EnableCheckBoxSelect="false"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;"
                                    DataKeyNames="SEQNO,ROWNO,BILLNO,FLAG" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick"
                                    EnableColumnLines="true" EnableMultiSelect="false" KeepCurrentSelection="true"
                                    AllowSorting="true" EnableHeaderMenu="true" OnRowDataBound="GridList_RowDataBound"
                                    IsDatabasePaging="true" AllowPaging="true" PageSize="50" OnPageIndexChange="GridList_PageIndexChange">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField DataField="SEQNO" Hidden="true" />
                                        <f:BoundField Width="120px" DataField="BILLNO" HeaderText="单据编号" SortField="BILLNO" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="BILLTYPE" HeaderText="单据类别" SortField="BILLTYPE" Hidden="true" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="FLAG" ColumnID="FLAG" HeaderText="单据状态" SortField="FLAG" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="DEPTID" HeaderText="订货部门" SortField="DEPTID" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="XDRQ" HeaderText="计划月份" TextAlign="Center" SortField="YSRQ" DataFormatString="{0:yyyy-MM}" />
                                        <f:BoundField Width="80px" DataField="SUBNUM" HeaderText="明细条数" TextAlign="Center" SortField="SUBNUM" />
                                        <f:BoundField Width="120px" DataField="SUBSUM" HeaderText="汇总金额" TextAlign="Center" SortField="SUBSUM" />
                                        <f:BoundField Width="70px" DataField="LRY" HeaderText="录入员" TextAlign="Center" SortField="LRY" />
                                        <f:BoundField Width="80px" DataField="LRRQ" HeaderText="录入日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="LRRQ" />
                                        <f:BoundField Width="70px" DataField="SHR" HeaderText="审核员" TextAlign="Center" SortField="SHR" />
                                        <f:BoundField Width="80px" DataField="SHRQ" HeaderText="审核日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="SHRQ" />
                                        <f:BoundField Width="100px" DataField="MEMO" HeaderText="备注" SortField="MEMO" ExpandUnusedSpace="true" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="生成月度采购" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBox"
                            ShowHeader="false">
                            <Toolbars>
                                <f:Toolbar ID="Toolbar1" runat="server">
                                    <Items>
                                        <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="月度预算管理（生成月度预算单）" runat="server" />
                                        <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                        <f:Button ID="btnGenerate" Icon="BasketEdit" Text="生 成" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnGenerate_Click" />
                                        <f:Button ID="btnNew" Icon="PageAdd" Text="新 单" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnNew_Click" />
                                        <f:Button ID="btnDelRow" Icon="Delete" Text="删 行" EnableDefaultState="false" EnablePostBack="true" Enabled="false" ConfirmText="是否确认删除改行信息?" runat="server" OnClick="btnBill_Click" />
                                        <f:Button ID="btnSave" Icon="Disk" Text="保 存" EnableDefaultState="false" EnablePostBack="true" runat="server" Enabled="false" ValidateForms="FormDoc" OnClick="btnBill_Click" />
                                        <f:Button ID="btnSumbit" Icon="UserTick" Text="提 交" EnableDefaultState="false" ConfirmText="是否确认提交此单据？" EnablePostBack="true" runat="server" Enabled="false" ValidateForms="FormDoc" OnClick="btnSumbit_Click" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btnAudit" Icon="UserTick" Text="审 批" EnableDefaultState="false" EnablePostBack="true" Enabled="false" ConfirmText="是否确认审核此单据?" runat="server" OnClick="btnBill_Click" />
                                        <f:Button ID="btnCancel" Icon="UserCross" Text="驳 回" EnableDefaultState="false" EnablePostBack="true" Enabled="false" runat="server" OnClick="btnCancel_Click" />
                                        <f:Button ID="btnPrint" Icon="Printer" Text="打 印" EnableDefaultState="false" EnablePostBack="false" runat="server" Enabled="false" OnClientClick="btnPrint_Bill();" />
                                        <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" EnableDefaultState="false" OnClick="btnBill_Click" ConfirmText="是否确认导出此单据信息?" DisableControlBeforePostBack="false" Enabled="false"
                                            EnablePostBack="true" EnableAjax="false" EnableAjaxLoading="true" AjaxLoadingType="Mask" runat="server" />
                                    </Items>
                                </f:Toolbar>
                            </Toolbars>
                            <Items>
                                <f:Form ID="FormDoc" ShowBorder="false" AutoScroll="false" BodyPadding="10px 5px 5px 5px"
                                    ShowHeader="False" LabelWidth="80px" runat="server">
                                    <Rows>
                                        <f:FormRow Hidden="true">
                                            <Items>
                                                <f:TextBox ID="docBILLNO" runat="server" Label="单据编号" Hidden="true" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow ColumnWidths="25% 25% 25% 25% ">
                                            <Items>
                                                <f:TextBox ID="docSEQNO" runat="server" Label="单据编号" Enabled="false" MaxLength="20" EmptyText="系统自动生成" />
                                                <f:DropDownList ID="docFLAG" runat="server" Label="单据状态" Enabled="false" EnableEdit="true" ForceSelection="true" LabelWidth="90px" />
                                                <f:DropDownList ID="ddlDEPTID" runat="server" Enabled="false" Label="订货部门" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                                                <f:NumberBox ID="nbxYSSL" runat="server" Label="预算调整幅度" Enabled="false" LabelWidth="100px" Text="1" MinValue="0" MaxValue="1000"></f:NumberBox>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow ColumnWidths="25% 75% ">
                                            <Items>
                                                <f:TextBox ID="docYSRQ" runat="server" Label="计划月份" Enabled="true" LabelWidth="90px"></f:TextBox>
                                                <f:TextBox ID="docMEMO" runat="server" Label="备注说明" MaxLength="80" LabelWidth="90px" />

                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:DropDownList ID="docLRY" runat="server" Label="录入员" EnableEdit="true" Enabled="false" ForceSelection="true" LabelWidth="90px" />
                                                <f:DatePicker ID="docLRRQ" runat="server" Label="录入日期" Enabled="false" />
                                                <f:DropDownList ID="docSHR" runat="server" Label="审核人" Enabled="false" EnableEdit="true" ForceSelection="true" LabelWidth="90px" />
                                                <f:DatePicker ID="docSHRQ" runat="server" Label="审核日期" Enabled="false" LabelWidth="90px" />
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                                <f:Grid ID="GridGoods" BoxFlex="1" ShowBorder="false" ShowHeader="false" EnableSummary="true" SummaryPosition="Bottom"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableColumnLines="true" AllowColumnLocking="true"
                                    DataKeyNames="GDSEQ" AllowCellEditing="true" ClicksToEdit="1" EnableAfterEditEvent="true" OnAfterEdit="GridGoods_AfterEdit">
                                    <Columns>
                                        <f:RenderField Width="35px" EnableLock="true" Locked="true" ColumnID="ROWNO" DataField="ROWNO" FieldType="String" EnableHeaderMenu="false">
                                            <Editor>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="105px" ColumnID="GDSEQ" DataField="GDSEQ" FieldType="String" TextAlign="Center" EnableLock="true" Locked="true"
                                            HeaderText="商品编码" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="lblEditorGDSEQ" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="160px" ColumnID="GDNAME" DataField="GDNAME" FieldType="String" EnableLock="true" Locked="true"
                                            HeaderText="商品名称" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="lblGDNAME" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="GDSPEC" DataField="GDSPEC" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="商品规格" EnableLock="true" Locked="true">
                                            <Editor>
                                                <f:Label ID="lblGDSPEC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="ADVSL" DataField="ADVSL" FieldType="String" TextAlign="Center"
                                            HeaderText="建议采购量">
                                            <Editor>
                                                <f:Label ID="lblEditorSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="UNIT" DataField="UNIT" FieldType="String" EnableHeaderMenu="false" TextAlign="Center"
                                            HeaderText="订货单位">
                                            <Editor>
                                                <f:Label ID="lblEditorUNIT" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="BZHL" DataField="BZHL" FieldType="Auto" EnableHeaderMenu="false"
                                            HeaderText="包装含量" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorBZHL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="85px" ColumnID="DHS" DataField="DHS" FieldType="String" EnableHeaderMenu="false" HeaderText="计划数量" TextAlign="Right">
                                            <Editor>
                                                <f:NumberBox ID="comDHSL" runat="server" MinValue="0" MaxValue="99999999" NoDecimal="true" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="85px" ColumnID="ZGKC" DataField="ZGKC" FieldType="Auto" EnableHeaderMenu="false" TextAlign="Right"
                                            HeaderText="最高库存">
                                            <Editor>
                                                <f:Label ID="lblZGKC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="85px" ColumnID="ZDKC" DataField="ZDKC" FieldType="Auto" EnableHeaderMenu="false" TextAlign="Right"
                                            HeaderText="最低库存">
                                            <Editor>
                                                <f:Label ID="lblZDKC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="85px" ColumnID="AVGSL" DataField="AVGSL" FieldType="Auto" EnableHeaderMenu="false" TextAlign="Right"
                                            HeaderText="日均用量">
                                            <Editor>
                                                <f:Label ID="lblAVGSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="85px" ColumnID="KCSL" DataField="KCSL" FieldType="Auto" EnableHeaderMenu="false"
                                            HeaderText="库存数量" TextAlign="Right">
                                            <Editor>
                                                <f:Label ID="lblKCSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="85px" ColumnID="HSJJ" DataField="HSJJ" FieldType="Auto" EnableHeaderMenu="false"
                                            HeaderText="含税进价" RendererFunction="round4" TextAlign="Right">
                                            <Editor>
                                                <f:Label ID="lblHSJJ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="85px" ColumnID="HSJE" DataField="HSJE" FieldType="Auto" EnableHeaderMenu="false" RendererFunction="round2"
                                            HeaderText="含税金额" TextAlign="Right">
                                            <Editor>
                                                <f:Label ID="lblMUSTJE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="200px" ColumnID="PRODUCER" DataField="PRODUCER" FieldType="Auto" EnableHeaderMenu="false"
                                            HeaderText="生产商">
                                            <Editor>
                                                <f:Label ID="lblPRODUCTER" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="130px" ColumnID="PZWH" DataField="PZWH" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText=" 注册证号">
                                            <Editor>
                                                <f:Label ID="lblPIZNO" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="120px" ColumnID="MEMO" DataField="MEMO" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="备注">
                                            <Editor>
                                                <f:TextBox ID="lblMEMO" runat="server" />
                                            </Editor>
                                        </f:RenderField>

                                        <f:RenderField Width="0px" ColumnID="CATID" DataField="CATID" FieldType="String" Hidden="true"
                                            HeaderText="产品分类">
                                            <Editor>
                                                <f:Label ID="lblCATID" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="SUPID" DataField="SUPID" FieldType="String" Hidden="true"
                                            HeaderText="供应商">
                                            <Editor>
                                                <f:Label ID="lblSUPID" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="ISLOT" DataField="ISLOT" FieldType="String" Hidden="true"
                                            HeaderText="批号管理">
                                            <Editor>
                                                <f:Label ID="lblISLOT" runat="server" />
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
        <f:HiddenField ID="hfdoper" runat="server"></f:HiddenField>
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
                                <f:TextArea ID="txaMemo" runat="server" Label="详细说明" Height="100px" MaxLength="40" />
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar4" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="btnRejectSubmit" Text="确定" EnableDefaultState="false" Icon="SystemSave" runat="server" OnClick="btnRejectSubmit_Click">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
        <f:Window ID="winDataSelectDate" Title="生成日期" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="350px" Height="220px">
            <Items>
                <f:Form ID="Form3" ShowBorder="false" AutoScroll="false" BodyPadding="5px 10px"
                    ShowHeader="False" LabelWidth="75px" runat="server">
                    <Rows>
                        <f:FormRow>
                            <Items>
                                <f:DropDownList ID="ddlDeptYs" runat="server" LabelWidth="80px" Label="预算部门" ShowRedStar="true" Required="true" EnableEdit="true" ForceSelection="true" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:NumberBox ID="nbxTz" runat="server" Label="预算调整幅度" LabelWidth="110px" Text="1" DecimalPrecision="2" MinValue="0.01" MaxValue="100" Required="true" ShowRedStar="true"></f:NumberBox>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:DatePicker ID="dpStartDate" runat="server" LabelWidth="110px" Label="参考开始日期" Required="true" ShowRedStar="true" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:DatePicker ID="dpEndDate" runat="server" LabelWidth="110px" Label="参考结束日期" ShowRedStar="true" Required="true" />
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar3" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="btnSureDate" Text="确定并关闭" EnableDefaultState="false" ValidateForms="Form3" Icon="SystemSave" runat="server" OnClick="btnSureDate_Click">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
    </form>
    <script src="../res/my97/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        function btnPrint_Bill() {
            var billNo = F('<%= docSEQNO.ClientID%>').getValue();
            if (billNo == "") {
                F.alert("单据不存在！");
                return;
            }
            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "/grf/CGJH.grf?timestamp=" + new Date().getTime();
            var dataurl = "/captcha/PrintReport.aspx?Method=getJHBill&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
    </script>
    <script language="javascript" type="text/javascript">
        function onGridBeforeEdit(event, value, params) {
            var flag = F('<%= docFLAG.ClientID%>').getValue();
            if ((",N,Y,C,G,M").indexOf(flag) > 0) {
                if (flag == "Y" || flag == "C")
                    return false;
                else {
                    return true;
                }
            }
            else
                return false;
        }
    </script>
    <script language="javascript" type="text/javascript">
        var tbxMyBoxClientID3 = '<%= tbxJHYF.ClientID %>'
        F.ready(function () {
            var tbxMyBox3 = F(tbxMyBoxClientID3);

            tbxMyBox3.onTriggerClick = function () {
                WdatePicker({
                    el: tbxMyBoxClientID3 + '-inputEl',
                    dateFmt: 'yyyy-MM',
                    onpicked: function () {
                        tbxMyBox3.validate();
                    }
                });
            };
        });
    </script>
</body>
</html>
