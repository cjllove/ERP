﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeptDetailSearch.aspx.cs" Inherits="ERPProject.ERPQuery.DeptDetailSearch" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>

    <script src="../res/js/CreateControl.js" type="text/javascript"></script>
    <script src="../res/js/GRInstall.js" type="text/javascript"></script>
    <script src="../res/js/jquery.ymh.js" type="text/javascript"></script>
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

        .x-grid-row.highlight td {
            background-color: lightgreen;
            background-image: none;
        }

        .x-grid-row.highRedwlight td {
            background-color: red;
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
        <f:PageManager ID="PageManager1" AutoSizePanelID="TabStrip1" runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="1" runat="server">
            <Tabs>
                <f:Tab Title="计费与非计费明细" Icon="Table" Layout="Fit" runat="server" Hidden="true">
                    <Items>
                        <f:Panel ID="Panel2" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="Panel3" ShowBorder="false" BodyPadding="0px"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar2" runat="server">
                                            <Items>
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="btnClear1" Icon="ArrowRotateClockwise" Text="清 空" OnClick="btnClear1_Click" EnablePostBack="true" runat="server" EnableDefaultState="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnPrint1" Icon="Printer" Text="打 印" DisableControlBeforePostBack="true" runat="server" OnClientClick="btnPrint1_onclick()" EnableDefaultState="false" Hidden="true" />
                                                <f:Button ID="btnExport1" Icon="DatabaseGo" Text="导 出" OnClick="btnExport1_Click" ConfirmText="是否导出当前信息?" DisableControlBeforePostBack="false" EnableDefaultState="false"
                                                    EnablePostBack="true" EnableAjax="false" EnableAjaxLoading="true" AjaxLoadingType="Mask" runat="server" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnSearch1" Icon="Magnifier" Text="查 询" OnClick="btnSearch1_Click" runat="server" EnableDefaultState="false" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="FormSearch" ShowBorder="false" AutoScroll="false" BodyPadding="5px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow ColumnWidths="25% 25% 25% 25%">
                                                    <Items>
                                                         <f:TriggerBox ID="trbSearch" runat="server" Label="查询商品" EmptyText="可输入编码、名称或助记码" TriggerIcon="Search" OnTriggerClick="btSearch_Click"></f:TriggerBox>
                                                        <f:DropDownList ID="ddlISJF" runat="server" Label="是否计费">
                                                            <f:ListItem  Value="" Text="--请选择--"/>
                                                            <f:ListItem Value="Y" Text="是" />
                                                            <f:ListItem Value="N" Text="否" />
                                                        </f:DropDownList>
                                                        <f:DatePicker ID="dpkBegRQ" runat="server" Label="查询期间" Required="true" ShowRedStar="true" />
                                                        <f:DatePicker ID="dpkEndRQ" runat="server" Label="　　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridCom" AnchorValue="100% -80" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;"
                                    EnableColumnLines="true" EnableTextSelection="true" AutoScroll="true" runat="server" DataKeyNames="GDSEQ"
                                    EnableSummary="true" SummaryPosition="Bottom">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField Width="100px" DataField="DEPTID" SortField="DEPTID" HeaderText="科室编码"></f:BoundField>
                                        <f:BoundField Width="100px" DataField="DEPTNAME" SortField="DEPTNAME" HeaderText="科室名称"></f:BoundField>
                                        <f:BoundField Width="100px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" TextAlign="Center"  />
                                        <f:BoundField Width="200px" DataField="GDNAME" SortField="GDNAME" ColumnID="COUNTTITLE" HeaderText="商品名称" ExpandUnusedSpace="true" />
                                        <f:BoundField Width="230px" DataField="GDSPEC" SortField="GDSPEC" HeaderText="规格" />
                                        <f:BoundField Width="50px" DataField="UNIT" SortField="UNIT" HeaderText="单位" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="HSJJ" SortField="HSJJ" HeaderText="单价" DataFormatString="{0:F4}" TextAlign="Right" />
                                        <f:BoundField Width="90px" DataField="SL" SortField="SL" ColumnID="HJSL" HeaderText="数量" TextAlign="Right" />
                                        <f:BoundField Width="90px" DataField="JE" SortField="JE" ColumnID="HJJE" HeaderText="总金额" DataFormatString="{0:F2}" TextAlign="Right" />
                                        <f:BoundField Width="120px" DataField="ISJF" SortField="ISJF" HeaderText="是否计费" TextAlign="Center" />
                                    </Columns>
                                </f:Grid>
                                <f:HiddenField runat="server" ID="hfdDEPTID"></f:HiddenField>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="科室商品明细查询" Icon="PageWord" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
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
                                                <f:Button ID="btnPrint" Icon="Printer" Text="打 印" DisableControlBeforePostBack="true" runat="server" OnClientClick="btnPrint_onclick()" EnableDefaultState="false"  Hidden="true"/>
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
                                                          <f:DropDownList runat="server" ID="ddlPSSID" Label="配送商"></f:DropDownList>
                                                        <f:DropDownList runat="server" ID="ddlISGZ" Label="是否高值">
                                                            <f:ListItem  Value="" Text="--请选择--" Selected="true" />
                                                            <f:ListItem Value="Y" Text="是" />
                                                            <f:ListItem Value="N" Text="否" />
                                                        </f:DropDownList>
                                                         <f:DatePicker ID="lstLRRQ1" runat="server" Label="查询期间" Required="true" ShowRedStar="true" />
                                                        <f:DatePicker ID="lstLRRQ2" runat="server" Label="　　至" LabelSeparator="" Required="true" ShowRedStar="true" />

                                                      </Items>
                                                    </f:FormRow>
                                        <f:FormRow ColumnWidths="25% 50% 25%">
                                                    <Items>
                                                <f:DropDownList runat="server" ID="ddlDEPT" Label="科室名称" EnableEdit="true"></f:DropDownList>

                                                        <f:TextBox EmptyText="输入商品编码、名称查询" runat="server" ID="tbxGDSEQ" Label="商品信息"></f:TextBox>
                                                        <f:Label runat="server"></f:Label>
                                                                                </Items>
                                                </f:FormRow>
                                            </Rows>
                                           
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridGoods" AnchorValue="100% -100"  ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;"  EnableRowDoubleClickEvent="false" OnRowDoubleClick="GridGoods_RowDoubleClick"
                                    PageSize="500" IsDatabasePaging="true" AllowPaging="true" OnPageIndexChange="GridGoods_PageIndexChange" EnableColumnLines="true" EnableTextSelection="true"
                                    AllowSorting="true" AutoScroll="true" runat="server" DataKeyNames="DEPTID" OnSort="GridGoods_Sort" SortField="DEPTID" SortDirection="ASC"
                                    EnableSummary="true" SummaryPosition="Bottom">
                                    <Columns>
                                        <f:RowNumberField Width="40px" TextAlign="Center" />
                                         <f:BoundField Width="65px" DataField="DEPTID" SortField="DEPTID" HeaderText="科室编码" TextAlign="Center" ColumnID="DEPTID" />
                                        <f:BoundField Width="130px" DataField="DEPTNAME" SortField="DEPTNAME" HeaderText="科室名称" ColumnID="DEPTNAME" />
                                       <f:BoundField Width="100px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" ColumnID="GDSEQ" />
                                        <f:BoundField Width="125px" DataField="GDNAME" SortField="GDNAME" HeaderText="商品名称" ColumnID="GDNAME"  ExpandUnusedSpace="true"/>
                                        <f:BoundField Width="130px" DataField="PSSID" SortField="PSSID" HeaderText="供应商编码" ColumnID="PSSID"  Hidden="true"/>
                                        <f:BoundField Width="130px" DataField="PSSNAME" SortField="PSSNAME" HeaderText="供应商名称" ColumnID="PSSNAME"  ExpandUnusedSpace="true"/>
                                        <f:BoundField Width="80px" DataField="GZSL" SortField="GZSL" HeaderText="高值数量" ColumnID="GZSL" />
                                        <f:BoundField Width="80px" DataField="GZHSJE" SortField="GZHSJE" HeaderText="高值金额" ColumnID="GZHSJE" />
                                        <f:BoundField Width="80px" DataField="JFSL" SortField="JFSL" HeaderText="计费数量" ColumnID="JFSL" />
                                      <f:BoundField Width="80px" DataField="JFHSJE" SortField="JFHSJE" HeaderText="计费金额" ColumnID="JFHSJE" />
                                           <f:BoundField Width="80px" DataField="FJFSL" SortField="FJFSL" HeaderText="非计费数量" ColumnID="FJFSL" />
                                      <f:BoundField Width="80px" DataField="FJFHSJE" SortField="FJFHSJE" HeaderText="非计费金额" ColumnID="FJFHSJE" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
    </form>

      
</body>
</html>
