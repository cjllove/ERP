<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GZ_Consumable.aspx.cs" Inherits="ERPProject.ERPQuery.GZ_Consumable" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>高值商品使用明细及汇总</title>
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
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="TabStrip1" runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="1" runat="server">
            <Tabs>
                <f:Tab Title="高值商品使用明细" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel1" runat="server" ShowBorder="False" BodyPadding="0px" Layout="Region"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="PanelBody" ShowBorder="false" BodyPadding="0px" RegionPosition="Top" Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar1" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText" runat="server" Text="操作信息："></f:ToolbarText>
                                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                <f:Button ID="btnAudit" Icon="ArrowRotateClockwise" Text="清 空" OnClick="Tab1btClear_Click" EnablePostBack="true" runat="server" EnableDefaultState="false" />
                                                <f:Button ID="btnPrint" Icon="Printer" Text="打 印" EnableDefaultState="false" EnablePostBack="false" runat="server" OnClientClick="Tab1btnPrint_Bill()"  />
                                               <%-- <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnExp" runat="server" CssStyle="margin-left: 10px;" Icon="DatabaseGo" EnableAjax="false" EnableDefaultState="false"
                                                    OnClick="Tab1btExport_Click" EnablePostBack="true" Text="导出" ConfirmText="是否确认导出此进销存信息?" />
                                                <f:ToolbarSeparator runat="server" />--%>
                                                <f:Button  ID="btSearch"  Icon="Magnifier"  Text="查 询"  OnClick="Tab1btSearch_Click" EnablePostBack="true" runat="server" EnableDefaultState="false"/>
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                </f:Panel>
                                <f:Panel ID="Panel2" BodyPadding="0px" RegionSplit="true" EnableCollapse="true" RegionPosition="Top" ShowBorder="false" Layout="Anchor" ShowHeader="False" runat="server">
                                    <Items>
                                        <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="10px 0px 5px 10px" ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow ColumnWidths="25% 25% 25% 25%">
                                                    <Items>
                                                        <f:TextBox ID="txbGDSEQ" runat="server" Label="商品信息" EmptyText="可输入编码、名称或助记码" />
                                                        <f:DropDownList ID="ddlDEPTID" runat="server" Label="科室" EnableEdit="true" ForceSelection="true" />
                                                        <f:TextBox ID="tbxNAME" runat="server" Label="患者姓名" EmptyText="输入患者姓名"></f:TextBox>
                                                        <f:TextBox ID="TextZYH" runat="server" Label="住院号" EmptyText="输入住院号"></f:TextBox>
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow ColumnWidths="50% 50%">
                                                    <Items>
                                                        <f:DatePicker ID="dpkDATE1" runat="server" Label="使用日期" Required="true" ShowRedStar="true"></f:DatePicker>
                                                        <f:DatePicker ID="dpkDATE2" runat="server" Label=" 至 " Required="true" ShowRedStar="true"></f:DatePicker>
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="Tab1GridGoods" runat="server" ShowBorder="false" ShowHeader="false" AllowColumnLocking="false"
                                    AllowSorting="false" AutoScroll="true" OnSort="Tab1GridGoods_Sort" SortField="SEQNO" SortDirection="DESC" DataKeyNames="GDSEQ"
                                    IsDatabasePaging="true" AllowPaging="true" PageSize="100" OnPageIndexChange="Tab1GridGoods_PageIndexChange"
                                    SummaryPosition="Bottom" EnableSummary="true"
                                    EnableColumnLines="true" EnableTextSelection="true">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" EnableLock="true" Locked="true" />
                                        <f:BoundField Width="120px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                         <f:BoundField Width="150px" DataField="GDNAME" SortField="GDNAME" HeaderText="商品名称" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                         <f:BoundField Width="150px" DataField="GDSPEC" SortField="GDSPEC" HeaderText="商品规格" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                         <f:BoundField Width="50px" DataField="UNITNAME" SortField="UNITNAME" HeaderText="单位" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                         <f:BoundField Width="80px" DataField="HSJJ" SortField="HSJJ" HeaderText="单价" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                         <f:BoundField Width="80px" DataField="SL" SortField="SL" HeaderText="数量" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                         <f:BoundField Width="80px" DataField="HSJE" SortField="HSJE" HeaderText="金额" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                         <f:BoundField Width="80px" DataField="PPID" SortField="PPID" HeaderText="品牌" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                         <f:BoundField Width="150px" DataField="SUPPLIERNAME" SortField="SUPPLIERNAME" HeaderText="供应商" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                         <f:BoundField Width="100px" DataField="DEPTNAME" SortField="DEPTNAME" HeaderText="使用科室" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                         <f:BoundField Width="150px" DataField="CUSTID" SortField="CUSTID" HeaderText="患者姓名" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                         <f:BoundField Width="150px" DataField="STR7" SortField="STR7" HeaderText="住院号" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                     </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="高值商品使用汇总" Icon="PageWord" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel3" runat="server" ShowBorder="False" BodyPadding="0px" Layout="Region"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="Panel4" ShowBorder="false" BodyPadding="0px" RegionPosition="Top" Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar2" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText1" runat="server" Text="操作信息："></f:ToolbarText>
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="Button1" Icon="ArrowRotateClockwise" Text="清 空" OnClick="Tab2btClear_Click" EnablePostBack="true" runat="server" EnableDefaultState="false" />
                                                 <f:Button ID="Button2" Icon="Printer" Text="打 印" EnableDefaultState="false" EnablePostBack="false" runat="server" OnClientClick="Tab2btnPrint_Bill()"  />
                                                <%--<f:ToolbarSeparator runat="server" />
                                                <f:Button ID="Button2" runat="server" CssStyle="margin-left: 10px;" Icon="DatabaseGo" EnableAjax="false" EnableDefaultState="false"
                                                    OnClick="Tab2btExport_Click" EnablePostBack="true" Text="导出" ConfirmText="是否确认导出此进销存信息?" />
                                                <f:ToolbarSeparator runat="server" />--%>
                                                <f:Button ID="Button3" Icon="Magnifier" Text="查 询" OnClick="Tab2btSearch_Click" runat="server" EnableDefaultState="false" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                </f:Panel>
                                <f:Panel ID="Panel5" BodyPadding="0px" RegionSplit="true" EnableCollapse="true" RegionPosition="Top" ShowBorder="false" Layout="Anchor" ShowHeader="False" runat="server">
                                    <Items>
                                        <f:Form ID="Form2" ShowBorder="false" AutoScroll="false" BodyPadding="10px 0px 5px 10px" ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow ColumnWidths="25% 25% 25% 25%">
                                                    <Items>
                                                        <f:TextBox ID="TextBox1" runat="server" Label="商品信息" EmptyText="可输入编码、名称或助记码" />
                                                        <f:DropDownList ID="ddlDEPTID2" runat="server" Label="科室" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="DatePicker1" runat="server" Label="使用日期" Required="true" ShowRedStar="true"></f:DatePicker>
                                                        <f:DatePicker ID="DatePicker2" runat="server" Label=" 至 " Required="true" ShowRedStar="true"></f:DatePicker>
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="Tab2GridGoods" runat="server" ShowBorder="false" ShowHeader="false" AllowColumnLocking="false"
                                    AllowSorting="false" AutoScroll="true" OnSort="Tab2GridGoods_Sort" SortField="SEQNO" SortDirection="DESC" DataKeyNames="GDSEQ"
                                    IsDatabasePaging="true" AllowPaging="true" PageSize="100" OnPageIndexChange="Tab2GridGoods_PageIndexChange"
                                    SummaryPosition="Bottom" EnableSummary="true"
                                    EnableColumnLines="true" EnableTextSelection="true">
                                    <Columns>
                                       <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" EnableLock="true" Locked="true" />
                                        <f:BoundField Width="120px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                         <f:BoundField Width="150px" DataField="GDNAME" SortField="GDNAME" HeaderText="商品名称" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                         <f:BoundField Width="150px" DataField="GDSPEC" SortField="GDSPEC" HeaderText="商品规格" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                         <f:BoundField Width="50px" DataField="UNITNAME" SortField="UNITNAME" HeaderText="单位" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                         <f:BoundField Width="80px" DataField="HSJJ" SortField="HSJJ" HeaderText="单价" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                         <f:BoundField Width="80px" DataField="SL" SortField="SL" HeaderText="数量" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                         <f:BoundField Width="80px" DataField="HSJE" SortField="HSJE" HeaderText="金额" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                         <f:BoundField Width="80px" DataField="PPID" SortField="PPID" HeaderText="品牌" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                         <f:BoundField Width="150px" DataField="SUPPLIERNAME" SortField="SUPPLIERNAME" HeaderText="供应商" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />
                                         <f:BoundField Width="100px" DataField="DEPTNAME" SortField="DEPTNAME" HeaderText="使用科室" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="false" />

                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
    </form>
    <f:HiddenField ID="USERID" runat="server" />
    <script type="text/javascript">
        function Tab1btnPrint_Bill() {
            var grfGDSEQ = F('<%= txbGDSEQ.ClientID%>').getValue();
            var grfDEPTID = F('<%= ddlDEPTID.ClientID%>').getValue();
            var grfNAME = F('<%= tbxNAME.ClientID%>').getValue();
            var grfZYH = F('<%= TextZYH.ClientID%>').getValue();
            var grfDATE1 = F('<%= dpkDATE1.ClientID%>').getText();
            var grfDATE2 = F('<%= dpkDATE2.ClientID%>').getText();
            var GrfUSER = F('<%= USERID.ClientID%>').getValue();

            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "/grf/GZSYMX.grf?201601062022";
            var dataurl = "/captcha/PrintReport.aspx?Method=GetGZSYMX&GDSEQ=" + escape(grfGDSEQ) + "&DEPTID=" + grfDEPTID + "&NAME=" + escape(grfNAME) + "&ZYH=" + grfZYH + "&DATE1=" + grfDATE1 + "&DATE2=" + grfDATE2 + "&user=" + GrfUSER;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
        function Tab2btnPrint_Bill() {
            var grfGDSEQ = F('<%= txbGDSEQ.ClientID%>').getValue();
            var grfDEPTID = F('<%= ddlDEPTID.ClientID%>').getValue();
            var grfDATE1 = F('<%= dpkDATE1.ClientID%>').getText();
            var grfDATE2 = F('<%= dpkDATE2.ClientID%>').getText();
            var GrfUSER = F('<%= USERID.ClientID%>').getValue();

            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "/grf/GZSYHZ.grf?201601062022";
            var dataurl = "/captcha/PrintReport.aspx?Method=GetGZSYHZ&GDSEQ=" + escape(grfGDSEQ) + "&DEPTID=" + grfDEPTID  + "&DATE1=" + grfDATE1 + "&DATE2=" + grfDATE2 + "&user=" + GrfUSER;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
    </script>
</body>
</html>
