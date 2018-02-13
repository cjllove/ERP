﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoodsChecking.aspx.cs" Inherits="ERPProject.ERPApply.GoodsChecking" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <script src="../../res/js/jquery.ymh.js" type="text/javascript"></script>
    <title>单品追踪</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="Panel1"
            runat="server" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
            ShowHeader="false">
            <Toolbars>
                <f:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                        <f:ToolbarText ID="ToolbarText" runat="server" Text="操作信息：按商品查询关联单据！"></f:ToolbarText>
                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                        <f:Button ID="btnExp" runat="server" Icon="DatabaseGo" EnableAjax="false" OnClick="btnExp_Click" EnablePostBack="true" DisableControlBeforePostBack="false" Text="导出" EnableDefaultState="false"></f:Button>
                        <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" DisableControlBeforePostBack="true" runat="server" ValidateForms="FormUser" OnClick="btSearch_Click" EnableDefaultState="false" />
                        <f:ToolbarSeparator runat="server" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="5px 10px 0px 10px"
                    ShowHeader="False" LabelWidth="80px" runat="server">
                    <Rows>
                        <f:FormRow>
                            <Items>
                                <f:TriggerBox ID="tgbGDSEQ" runat="server" ShowTrigger="true" Label="商品编码" MaxLength="20" EmptyText="可输入商品编码、名称或助记码"></f:TriggerBox>
                                <f:DropDownList ID="ddlISGZ" runat="server" Label="是否高值">
                                    <f:ListItem Text="--请选择--" Value="" />
                                    <f:ListItem Text="是" Value="Y" />
                                    <f:ListItem Text="否" Value="N" />
                                </f:DropDownList>
                                <f:DatePicker ID="dpkout1" runat="server" Label="单据日期" ShowRedStar="true" Required="true"></f:DatePicker>
                                <f:DatePicker ID="dpkout2" runat="server" Label="　　至" ShowRedStar="true" Required="true"></f:DatePicker>
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
                <f:Grid ID="GridGoods" AnchorValue="100% -30" ShowBorder="false" ShowHeader="false" EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridGoods_RowDoubleClick"
                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableColumnLines="true" EnableTextSelection="true"
                    PageSize="50" DataKeyNames="GDSEQ" IsDatabasePaging="true" AllowPaging="true" OnPageIndexChange="GridGoods_PageIndexChange">
                    <Columns>
                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                        <f:BoundField Width="90px" DataField="SHRQ" HeaderText="日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="120px" DataField="SEQNO" HeaderText="单号编号" TextAlign="Center" />
                        <f:BoundField Width="10px" DataField="BILLTYPE" HeaderText="单号类型" Hidden="true" />
                        <f:BoundField Width="100px" DataField="BILLTYPENAME" HeaderText="单号类型" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="FLAGNAME" HeaderText="单号状态" TextAlign="Center" />
                        <f:BoundField Width="70px" DataField="SHRNAME" HeaderText="审核人" TextAlign="Center" />
                        <f:BoundField Width="110px" DataField="DEPTOUTNAME" HeaderText="出库(使用)库房" />
                        <f:BoundField Width="110px" DataField="DEPTINNAME" HeaderText="使用(退货)科室" />
                        <f:BoundField Width="90px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                        <f:BoundField Width="180px" DataField="GDNAME" HeaderText="商品名称" />
                        <f:BoundField Width="100px" DataField="GDSPEC" HeaderText="商品规格" />
                        <f:BoundField Width="150px" DataField="PRODUCERNAME" HeaderText="生产厂家" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="SL" HeaderText="数量" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center"></f:BoundField>
                        <f:BoundField Width="80px" DataField="HSJJ" HeaderText="单价" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="HSJE" HeaderText="金额" TextAlign="Center" />
                        <f:BoundField Width="60px" DataField="ISGZ" SortField="ISGZ" HeaderText="是否高值" Hidden="false" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
    <f:HiddenField ID="hfdValue" runat="server" />
    <f:Window ID="Window1" Title="单据信息" Hidden="true" EnableIFrame="true" runat="server"
        EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Width="820px" Height="480px">
    </f:Window>
</body>
</html>
