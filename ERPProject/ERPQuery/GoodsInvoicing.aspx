﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoodsInvoicing.aspx.cs" Inherits="ERPProject.ERPQuery.GoodsInvoicing" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>商品进销存明细查询</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="False" BodyPadding="0px" Layout="VBox"
            ShowHeader="false">
            <Toolbars>
                <f:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                        <f:ToolbarText ID="ToolbarText" runat="server" Text="操作信息：查询商品的进销存信息！"></f:ToolbarText>

                        <%--<f:CheckBox ID="chkISDG" runat="server" Text="包含代管商品" CssStyle="margin-left:10px;"></f:CheckBox>--%>
                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                        <f:Button ID="btnAudit" Icon="ArrowRotateClockwise" Text="清 空" OnClick="btClear_Click" EnablePostBack="true" runat="server" EnableDefaultState="false" />
                        <f:ToolbarSeparator runat="server" />
                        <f:Button ID="btnExp" runat="server" CssStyle="margin-left: 10px;" Icon="DatabaseGo" EnableAjax="false" EnableDefaultState="false" DisableControlBeforePostBack="false"
                            OnClick="btExport_Click"  Text="导出" ConfirmText="是否确认导出此进销存信息?" />
                        <f:ToolbarSeparator runat="server" />
                        <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" OnClick="btSearch_Click" runat="server" EnableDefaultState="false" />
                    </Items>
                </f:Toolbar>
            </Toolbars> 
            <Items>
                <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="5px"
                    ShowHeader="False" LabelWidth="70px" runat="server">
                    <Rows>
                        <f:FormRow>
                            <Items>
                                <f:TextBox ID="tbxBILLNO" runat="server" Label="单据编号" EmptyText="输入单据编号查询"></f:TextBox>
                                <f:TextBox ID="txbGDSEQ" runat="server" Label="商品信息" EmptyText="可输入编码、名称、批号、货位或助记码" />
                                <f:DropDownList ID="ddlDEPTID" runat="server" Label="部门" EnableEdit="true" ForceSelection="true" />
                                <f:DropDownList ID="ddlISGZ" runat="server" Label="是否高值">
                                    <f:ListItem Text="--请选择--" Value="" />
                                    <f:ListItem Text="是" Value="Y" />
                                    <f:ListItem Text="否" Value="N" />
                                </f:DropDownList>
                                <f:DropDownList runat="server" ID="ddlISFLAG7" Label="是否本地">
                                    <f:ListItem Text="-- 请选择 --" Value="" />
                                    <f:ListItem Text="是" Value="Y" />
                                    <f:ListItem Text="否" Value="N" />
                                </f:DropDownList>
                                <%-- <f:TriggerBox ID="tgbPH" runat="server" Label="批次" EmptyText="批次信息" MaxLength="20"></f:TriggerBox>--%>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:DropDownList ID="ddlSUPID" runat="server" Label="供应商" EnableEdit="true" ForceSelection="true" />
                                <f:DropDownList ID="ddlSCSID" runat="server" Label="配送商" EnableEdit="true" ForceSelection="true" />
                                <%--<f:DropDownList ID="srhCATID0" Label="商品种类" runat="server" EnableEdit="true" ForceSelection="true" />--%>
                                <f:DropDownList ID="ddlISADD" runat="server" Label="增减库存">
                                    <f:ListItem Text="--请选择--" Value="" />
                                    <f:ListItem Text="增库存" Value="1" />
                                    <f:ListItem Text="减库存" Value="-1" />
                                </f:DropDownList>
                                <f:DatePicker ID="dpkDATE1" runat="server" Label="操作日期" Required="true"></f:DatePicker>
                                <f:DatePicker ID="dpkDATE2" ShowRedStar="true" Required="true" runat="server" Label="　至" LabelSeparator=""></f:DatePicker>
                                <%-- <f:TriggerBox ID="tgbHWID" runat="server" Label="货位" EmptyText="货位信息" MaxLength="20"></f:TriggerBox>--%>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <%-- <f:DropDownList ID="ddlBillType" runat="server" Label="单据类别" EnableEdit="true" ForceSelection="true"></f:DropDownList>--%>
                                <f:CheckBoxList ID="cblBillType" Label="单据类别" ColumnNumber="7" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rblAutoPostBack_SelectedIndexChanged">
                                </f:CheckBoxList>
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
                <f:Grid ID="GridGoods" BoxFlex="1" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;" AllowColumnLocking="true"
                    AllowSorting="true" AutoScroll="true" runat="server" DataKeyNames="GDSEQ" IsDatabasePaging="true" AllowPaging="true" SummaryPosition="Bottom" EnableSummary="true"
                    PageSize="100" OnPageIndexChange="GridGoods_PageIndexChange" OnSort="GridGoods_Sort" SortField="RQSJ" SortDirection="DESC" EnableColumnLines="true" EnableTextSelection="true">
                    <Columns>
                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" EnableLock="true" Locked="true" />
                        <%--<f:BoundField Width="50px" DataField="SEQNO" SortField="SEQNO" HeaderText="系号" Hidden="false" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="true" TextAlign="Center" />--%>
                        <f:BoundField Width="130px" DataField="RQSJ" SortField="RQSJ" HeaderText="时间" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="true" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="BILLTYPE" SortField="BILLTYPE" HeaderText="单据类型" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="true" TextAlign="Center" />
                        <f:BoundField Width="110px" TextAlign="Center" DataField="BILLNO" SortField="BILLNO" HeaderText="单据编号" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="true" />
                        <f:BoundField Width="100px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="true" />
                        <f:BoundField Width="170px" DataField="GDNAME" SortField="GDNAME" ColumnID="GDNAME" HeaderText="商品名称" EnableLock="true" Locked="true" />
                        <f:BoundField Width="130px" DataField="GDSPEC" SortField="GDSPEC" HeaderText="规格·容量" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <%--<f:BoundField Width="60px" DataField="ISDG" SortField="ISDG" HeaderText="是否代管" EnableColumnHide="true" EnableHeaderMenu="false" Hidden="true" />--%>
                        <f:BoundField Width="70px" DataField="SL" SortField="SL" ColumnID="SL" HeaderText="数量" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="50px" DataField="UNIT" HeaderText="单位" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false"></f:BoundField>
                        <f:BoundField Width="70px" DataField="HSJJ" SortField="HSJJ" HeaderText="进价" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="90px" DataField="HSJE" HeaderText="含税金额" ColumnID="HSJE" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="90px" DataField="DEPTNAME" HeaderText="科室" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="150px" DataField="SUPNAME" HeaderText="供应商" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="150px" DataField="PSSNAME" HeaderText="配送商" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="70px" DataField="HWID" SortField="HWID" HeaderText="货位" EnableColumnHide="true" EnableHeaderMenu="false" TextAlign="Center" />
                        <f:BoundField Width="70px" DataField="PH" SortField="PH" HeaderText="批号" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="80px" DataField="RQ_SC" HeaderText="生产日期" DataFormatString="{0:yyyy-MM-dd}" EnableColumnHide="true" EnableHeaderMenu="false" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="YXQZ" SortField="YXQZ" HeaderText="效期" DataFormatString="{0:yyyy-MM-dd}" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="40px" DataField="ROWNO" HeaderText="行号" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" Hidden="true" />
                        <f:BoundField Width="60px" DataField="KCADD" SortField="KCADD" HeaderText="操作" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="80px" DataField="ISFLAG3" SortField="ISFLAG3" HeaderText="是否直送" EnableColumnHide="true" EnableHeaderMenu="false" TextAlign="Center" />
                        <f:BoundField Width="60px" DataField="JXTAX" SortField="JXTAX" HeaderText="税率" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" Hidden="true" />
                        <f:BoundField Width="50px" DataField="LSJ" SortField="LSJ" HeaderText="售价" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" Hidden="true" />
                        <f:BoundField Width="100px" DataField="BHSJJ" SortField="BHSJJ" HeaderText="不含税进价" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" Hidden="true" />
                        <f:BoundField Width="90px" DataField="LSJE" HeaderText="零售金额" ColumnID="LSJE" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" Hidden="true" />
                        <f:BoundField Width="90px" DataField="BHSJE" HeaderText="不含税金额" ColumnID="BHSJE" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" Hidden="true" />
                        <f:BoundField Width="110px" DataField="PZWH" HeaderText="注册证号" EnableColumnHide="true" EnableHeaderMenu="false" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="ZPBH" HeaderText="制品编号" EnableColumnHide="true" EnableHeaderMenu="false" Hidden="true" />
                        <f:BoundField Width="70px" DataField="OPERGH" HeaderText="操作员" EnableColumnHide="true" EnableHeaderMenu="false" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="CATNAME" HeaderText="商品类别" EnableColumnHide="true" EnableHeaderMenu="false" TextAlign="Center" />
                        <f:BoundField Width="60px" DataField="ISGZ" SortField="ISGZ" HeaderText="高值" TextAlign="Center" />
                        <f:BoundField Width="60px" DataField="ISFLAG7" SortField="ISFLAG7" HeaderText="本地" TextAlign="Center" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
</body>
</html>
