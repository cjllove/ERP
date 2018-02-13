﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DrugChecking.aspx.cs" Inherits="ERPProject.ERPApply.DrugChecking" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>非定数对账</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="Panel1"
            runat="server" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor" ShowHeader="false">
            <Toolbars>
                <f:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                        <f:ToolbarText ID="ToolbarText" runat="server" Text="操作信息：查询科室使用明细！"></f:ToolbarText>
                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                        <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" OnClick="btExport_Click" DisableControlBeforePostBack="false" EnableDefaultState="false"
                            EnablePostBack="true" EnableAjax="false" EnableAjaxLoading="true" AjaxLoadingType="Mask" runat="server" />
                        <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" EnableDefaultState="false" DisableControlBeforePostBack="true" runat="server" ValidateForms="FormUser" OnClick="btSearch_Click" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px" ShowHeader="false" LabelWidth="75px" runat="server">
                    <Rows>
                        <f:FormRow>
                            <Items>
                                <f:DropDownList ID="ddlDEPTINT" runat="server" Label="使用科室" Required="true" EnableEdit="true" ForceSelection="true" />
                                <f:TextBox ID="tbxGDSEQ" runat="server" Label="商品编码" MaxLength="20" EmptyText="可输入商品编码、名称或助记码" />
                                <f:DropDownList ID="ddlSFHS" runat="server" Label="回收状态" EnableEdit="true" ForceSelection="true">
                                    <f:ListItem Text="--请选择--" Value="" />
                                    <%--<f:ListItem Text="未使用&未退货&未回收" Value="N" />--%>
                                    <f:ListItem Text="已使用" Value="Y" />
                                    <f:ListItem Text="已退货" Value="S" />
                                     <f:ListItem Text="已登记" Value="J" />
                                     <%--<f:ListItem Text="已驳回" Value="R" />--%>
                                    <f:ListItem Text="已结算" Value="G" />
                                </f:DropDownList>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                               <f:TextBox ID="txtSEQNO" runat="server" Label="单据编号"　/>
                                <f:DatePicker ID="dpkout1" runat="server" Label="出库日期"></f:DatePicker>
                                <f:DatePicker ID="dpkout2" runat="server" Label="  至    " ></f:DatePicker>
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
                <f:Grid ID="GridGoods" AnchorValue="100% -73" ShowBorder="false" ShowHeader="false" EnableTextSelection="true"
                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;"
                    PageSize="50" DataKeyNames="GDSEQ" IsDatabasePaging="true" AllowPaging="true" OnPageIndexChange="GridGoods_PageIndexChange">
                    <Columns>
                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                        <f:BoundField Width="80px" DataField="XSRQ" HeaderText="使用日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="110px" DataField="SEQNO" HeaderText="使用单号" TextAlign="Center" />
                        <f:BoundField Width="110px" DataField="DEPTNAME" HeaderText="使用科室" />
                        <f:BoundField Width="90px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                        <f:BoundField Width="180px" DataField="GDNAME" HeaderText="商品名称" ExpandUnusedSpace="true" />
                        <f:BoundField Width="100px" DataField="GDSPEC" HeaderText="商品规格" />
                        <f:BoundField Width="80px" DataField="DHSL" HeaderText="数量" TextAlign="Center" />
                        <f:BoundField Width="260px" DataField="BARCODE" HeaderText="条码" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="FLAGNAME" HeaderText="回收标志" TextAlign="Center" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
</body>
</html>