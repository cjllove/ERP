<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="sldUpload.aspx.cs" Inherits="ERPProject.ERPStorage.sldUpload" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>申领单导入</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="TabStrip1" EnableAjaxLoading="false" AjaxLoadingType="Mask" runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right" EnableTabCloseMenu="false" ActiveTabIndex="1" runat="server">
            <Tabs>
                <f:Tab Title="历史查询" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel2" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="Panel3" ShowBorder="false" BodyPadding="0px"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar1" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="操作信息：查看历史导入单据！" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                <f:Button ID="btnSearch" Icon="Magnifier" Text="查 询" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnSearch_Click" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow ColumnWidth="50% 25% 25%">
                                                    <Items>
                                                        <f:TextBox ID="lstBILLNO" runat="server" Label="单据编号" />
                                                        <f:DropDownList ID="lstLRY" runat="server" Label="录入员" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="lstLRRQ1" runat="server" Label="录入日期" Required="true" ShowRedStar="true" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" AnchorValue="100% -113" ShowBorder="false" ShowHeader="false" EnableColumnLines="true" 
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableTextSelection="true" EnableCheckBoxSelect="true"
                                    DataKeyNames="SEQNO" EnableHeaderMenu="true" SortField="SEQNO" SortDirection="ASC" AllowSorting="true" EnableMultiSelect="true" KeepCurrentSelection="true">
                                    <Columns>
                                        <f:RowNumberField Width="35px" EnablePagingNumber="true" />
                                        <f:BoundField Width="160px" DataField="BILLNO" HeaderText="单据编号" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="DEPTOUT" HeaderText="出库库房编号" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="DEPTOUTNAME" HeaderText="出库库房" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="DEPTID" HeaderText="申领科室编号" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="DEPTNAME" HeaderText="申领科室" TextAlign="Center" />
                                        <%--<f:BoundField Width="90px" DataField="XSRQ" HeaderText="申领日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />--%>
                                        <f:BoundField Width="90px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                                        <f:BoundField Width="110px" DataField="GDNAME" HeaderText="名称" TextAlign="Center" />
                                        <f:BoundField Width="110px" DataField="BARCODE" HeaderText="条码" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="UNIT" HeaderText="单位" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="GDSPEC" HeaderText="商品规格" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="GDMODE" HeaderText="型号" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="DHSL" HeaderText="订货数" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="PH" HeaderText="批号" TextAlign="Center" />
                                        <%--<f:BoundField Width="90px" DataField="RQ_SC" HeaderText="生产日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="90px" DataField="YXQZ" HeaderText="有效期至" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />--%>
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="申领单导入" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="PanelPicture" runat="server" ShowHeader="false" ShowBorder="false"
                            BodyPadding="0px" Layout="Fit">
                            <Items>
                                <f:Grid ID="GridStock" ShowBorder="false" ShowHeader="false" EnableCheckBoxSelect="false" IsDatabasePaging="true"
                                    AllowSorting="true" AutoScroll="true" runat="server" EnableColumnLines="true" AllowPaging="true" PageSize="50"
                                    DataKeyNames="IMPSEQ" OnPageIndexChange="GridStock_PageIndexChange">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar2" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText1" Text="导入申领单:" EnableAjax="true" runat="server" />
                                                <f:FileUpload runat="server" ID="fuDocument" EmptyText="导入EXCEL文件" Width="300" ShowRedStar="true" AutoPostBack="true" OnFileSelected="btnSelect_Click" />
                                                <f:ToolbarFill runat="server"></f:ToolbarFill>
                                                <f:Button runat="server" ID="btnClear" Icon="Delete" Hidden="true" Text="清空列表数据" DisableControlBeforePostBack="false" Enabled="true" OnClick="btnClear_Click" EnableDefaultState="false"></f:Button>
                                                <f:ToolbarSeparator runat="server"></f:ToolbarSeparator>
                                                 <f:Button ID="btnclearall" Icon="Erase" Text="清 除" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnclearall_Click" />
                                                <f:Button runat="server" ID="btnSave" Icon="Disk" Enabled="false" ConfirmText="是否提交数据生成对应的单据？" EnableDefaultState="false"
                                                    Text="提 交" DisableControlBeforePostBack="false" ValidateForms="FormMain" OnClick="btnSave_Click">
                                                </f:Button>
                                                <f:LinkButton ID="LinkButton1" Text="下载模板" EnablePostBack="false" OnClientClick="DownLoadModelclick();" runat="server" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Columns>
                                        <f:RowNumberField Width="35px" EnablePagingNumber="true" />
                                        <%--<f:BoundField Width="160px" DataField="MEMO" HeaderText="提示信息" TextAlign="Center" />--%>
                                        <f:BoundField Width="100px" DataField="DEPTOUT" HeaderText="出库库房编号" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="DEPTOUTNAME" HeaderText="出库库房" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="DEPTID" HeaderText="申领科室编号" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="DEPTNAME" HeaderText="申领科室" TextAlign="Center" />
                                        <%--<f:BoundField Width="90px" DataField="XSRQ" HeaderText="申领日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />--%>
                                        <f:BoundField Width="90px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                                        <f:BoundField Width="110px" DataField="GDNAME" HeaderText="名称" TextAlign="Center" />
                                        <f:BoundField Width="110px" DataField="BARCODE" HeaderText="条码" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="UNIT" HeaderText="单位" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="GDSPEC" HeaderText="商品规格" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="GDMODE" HeaderText="型号" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="DHSL" HeaderText="订货数" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="PH" HeaderText="批号" TextAlign="Center" />
                                       <%-- <f:BoundField Width="90px" DataField="RQ_SC" HeaderText="生产日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="90px" DataField="YXQZ" HeaderText="有效期至" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />--%>
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
        <f:HiddenField ID="hfdseqno" runat="server"></f:HiddenField>
        </form>
        <script type="text/javascript">
            function DownLoadModelclick() {
                window.location.href = '申领单导入模板.xlsx';
            }
        </script>
</body>
</html>
