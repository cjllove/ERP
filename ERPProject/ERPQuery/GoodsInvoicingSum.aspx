<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoodsInvoicingSum.aspx.cs" Inherits="ERPProject.ERPQuery.GoodsInvoicingSum" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>商品进销存汇总</title>
    <style type="text/css">
        .x-grid-row-summary .x-grid-cell-inner {
            font-weight: bold;
            color: red;
        }
        .num-font{
            font-weight: bold;
            font-size:xx-large;
            color: red;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="False" BodyPadding="0px" Layout="Anchor"
            ShowHeader="false">
            <Items>
                <f:Panel ID="PanelBody" ShowBorder="false" BodyPadding="0px"
                    Layout="Anchor" ShowHeader="False" runat="server">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar1" runat="server">
                            <Items>
                                <f:ToolbarText ID="ToolbarText" runat="server" Text="操作信息：查询商品的进销存汇总信息！"></f:ToolbarText>
                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                <f:Button ID="btnAudit" Icon="ArrowRotateClockwise" Text="清 空" OnClick="btClear_Click" EnablePostBack="true" runat="server" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="btnExp" runat="server" CssStyle="margin-left: 10px;" Icon="DatabaseGo" EnableAjax="false" DisableControlBeforePostBack="false"
                                    OnClick="btExport_Click" EnablePostBack="true" Text="导出" DisableControlBeforePostBack="false" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" OnClick="btSearch_Click" runat="server" />
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Items>
                        <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="10px 0px 10px 10px"
                            ShowHeader="False" LabelWidth="80px" runat="server">
                            <Rows>
                                <f:FormRow ColumnWidths="20% 40% 20% 20%">
                                    <Items>
                                        <f:TextBox ID="txbGDSEQ" runat="server" Label="商品信息" />
                                        <f:DropDownList ID="ddlSUPID" runat="server" Label="送货商" EnableEdit="true" ForceSelection="true" />
                                        <f:DropDownList ID="ddlISGZ" runat="server" Label="是否高值">
                                            <f:ListItem Text="--请选择--" Value="" />
                                            <f:ListItem Text="是" Value="Y" />
                                            <f:ListItem Text="否" Value="N" />
                                        </f:DropDownList>
                                        <f:DropDownList ID="ddlInvoicType" runat="server" Label="进出类别">
                                            <f:ListItem Text="全部" Value="" /> 
                                            <f:ListItem Text="入库" Value="1" /> 
                                            <f:ListItem Text="出库" Value="-1" /> 
                                        </f:DropDownList>
                                    </Items>
                                </f:FormRow>
                                <f:FormRow>
                                    <Items>
                                        <f:DropDownList ID="ddlDEPTID" runat="server" Label="部门"></f:DropDownList>
                                        <f:Label ID="lblHJSL" runat="server" Label="合计数量" CssClass="num-font" CssStyle="font-weight: bold;font-size:xx-large;color: red;"></f:Label>
                                        <f:Label ID="lblHJJE" runat="server" Label="合计金额" CssClass="num-font"></f:Label>
                                        <f:DatePicker ID="dpkDATE1" runat="server" Label="操作日期" Required="true" ShowRedStar="true"></f:DatePicker>
                                         <f:DatePicker ID="dpkDATE2" runat="server" Label="　至" LabelSeparator="" Required="true" ShowRedStar="true" LabelWidth="40px"></f:DatePicker>
                                   </Items>
                                </f:FormRow>
                            </Rows>
                        </f:Form>
                    </Items>
                </f:Panel>
                <f:Grid ID="GridGoods" AnchorValue="100% -100px" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;"
                    AllowSorting="false" AutoScroll="true" runat="server" DataKeyNames="GDSEQ" EnableColumnLines="true" EnableSummary="true"
                    SummaryPosition="Bottom" AllowPaging="true" PageSize="100" IsDatabasePaging="true" OnPageIndexChange="GridGoods_PageIndexChange">
                    <Columns>
                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                        <f:BoundField Width="150px" DataField="DEPTNAME" HeaderText="科室" />
                        <f:BoundField Width="90px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                        <f:BoundField Width="180px" DataField="GDNAME" ColumnID="GDNAME" HeaderText="商品名称" />
                        <f:BoundField Width="130px" DataField="GDSPEC" HeaderText="规格·容量" />
                        <f:BoundField Width="110px" DataField="CATNAME" HeaderText="商品类别" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="SL" ColumnID="SL" HeaderText="合计数量" TextAlign="Right" />
                        <f:BoundField Width="100px" DataField="HSJE" ColumnID="HSJE" HeaderText="合计金额" TextAlign="Right" />
                        <f:BoundField Width="230px" DataField="SUPNAME" HeaderText="供应商" />
                        <f:BoundField Width="120px" DataField="PZWH" HeaderText="批准文号" TextAlign="Center" />
                        <f:BoundField Width="60px" DataField="ISGZ" SortField="ISGZ" HeaderText="是否高值" TextAlign="Center" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
</body>
</html>
