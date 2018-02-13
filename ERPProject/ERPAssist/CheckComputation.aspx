<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckComputation.aspx.cs" Inherits="ERPProject.ERPAssist.CheckComputation" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>单据凭证核对</title>
    <style type="text/css" media="all">
        .x-grid-row.highlight td {
            background-color: lightgreen;
            background-image: none;
        }
        .color1 {
            background-color:forestgreen;
            color: #fff;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="Panel1"
            runat="server" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBox"
            ShowHeader="false">
            <Toolbars>
                <f:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                        <f:Label ID="lblTOTAL" runat="server" Label="合计金额" LabelWidth="70px"></f:Label>
                         <f:ToolbarSeparator runat="server" />
                        <f:Label ID="lblSUBSUM" runat="server" Label="合计金额(已回收)" LabelWidth="130px"></f:Label>
                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                        <f:Button ID="btnAudit" Icon="UserTick" Text="回 收" ConfirmText="是否确认回收选中单据?" runat="server" EnableDefaultState="false" OnClick="btnAudit_Click" />
                        <f:ToolbarSeparator runat="server" />
                        <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" DisableControlBeforePostBack="true" runat="server" ValidateForms="FormUser" EnableDefaultState="false" OnClick="btSearch_Click" />
                        <f:ToolbarSeparator runat="server" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="5px"
                    ShowHeader="False" LabelWidth="70px" runat="server">
                    <Rows>
                        <f:FormRow>
                            <Items>
                                <f:TriggerBox ShowTrigger="false" ID="tgbSEQNO" Label="单据编号" MaxLength="20" EmptyText="输入单据编号信息" runat="server" OnTriggerClick="btSearch_Click"></f:TriggerBox>
                                <f:DropDownList ID="ddlDEPTID" runat="server" Label="科室" EnableEdit="true">
                                </f:DropDownList>
                                <f:DropDownList ID="ddlFLAG" runat="server" Label="回收状态">
                                    <f:ListItem Text="--请选择--" Value="" />
                                    <f:ListItem Text="未回收" Value="N" Selected="true" />
                                    <f:ListItem Text="已回收" Value="Y" />
                                </f:DropDownList>
                               <f:DropDownList ID="ddlHSY" runat="server" Label="回收员" EnableEdit="true" ForceSelection="true" /> 
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <%--<f:DropDownList ID="ddlTYPE" runat="server" Label="单据类型">
                                    <f:ListItem Text="--请选择--" Value="" />
                                    <f:ListItem Text="条码回收单" Value="DSH" />
                                    <f:ListItem Text="销售退货单" Value="XST" />
                                    <f:ListItem Text="科室使用单" Value="XSD" />
                                    <f:ListItem Text="高值销售单" Value="XSG" />
                                    <f:ListItem Text="使用汇总单" Value="CKH" />
                                </f:DropDownList>--%>
                                <f:DatePicker ID="dpkout1" runat="server" Label="审核日期" Required="true"></f:DatePicker>
                                <f:DatePicker ID="dpkout2" runat="server" Label="    至" Required="true"></f:DatePicker>
                                <f:DatePicker ID="dpkhs1" runat="server" Label="回收日期" ></f:DatePicker>
                                <f:DatePicker ID="dpkhs2" runat="server" Label="    至" ></f:DatePicker>
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
                <f:Grid ID="GridGoods" BoxFlex="1" ShowBorder="false" ShowHeader="false" EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridGoods_RowDoubleClick" KeepCurrentSelection="true"
                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableColumnLines="true" EnableTextSelection="true" EnableCheckBoxSelect="true" OnRowDataBound="GridGoods_RowDataBound"
                    PageSize="100" DataKeyNames="SEQNO,STR5" IsDatabasePaging="true" AllowPaging="true" OnPageIndexChange="GridGoods_PageIndexChange"
                    AllowSorting="true" SortField="SEQNO" SortDirection="DESC" OnSort="GridGoods_Sort">
                    <Columns>
                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                        <f:BoundField Width="90px" DataField="SHRQ" SortField="SHRQ" HeaderText="审核日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="120px" DataField="SEQNO" SortField="SEQNO" HeaderText="单号编号" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="HSRQ" SortField="HSRQ" HeaderText="回收日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                       <%-- <f:BoundField Width="120px" DataField="DEPTOUTNAME" SortField="DEPTOUTNAME" HeaderText="出库(使用)库房" />--%>
                        <f:BoundField Width="130px" DataField="DEPTIDNAME" SortField="DEPTIDNAME" HeaderText="业务部门" TextAlign="Center" />
                        <f:BoundField Width="10px" DataField="BILLTYPE" HeaderText="单号类型" Hidden="true" />
                        <f:BoundField Width="100px" DataField="BILLTYPENAME" HeaderText="单号类型" TextAlign="Center" />
                        <%--<f:BoundField Width="60px" DataField="FLAGNAME" HeaderText="单号状态" TextAlign="Center" />--%>
                        <f:BoundField Width="80px" DataField="SHRNAME" HeaderText="操作员" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="SUBNUM" HeaderText="明细条数" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="SUBSUM" HeaderText="合计金额" TextAlign="Right" DataFormatString="{0:f2}" />
                        <f:BoundField Width="90px" DataField="HSFLAG" ColumnID="HSFLAG" HeaderText="回收状态" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="ISCHECK" HeaderText="回收状态" TextAlign="Center" Hidden="true" />
                        <f:BoundField MinWidth="90px" ExpandUnusedSpace="true" DataField="HSRNAME" HeaderText="回收人" TextAlign="Center"/>
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
    <f:HiddenField ID="hfdValue" runat="server" />
    <f:HiddenField ID="highlightRows" runat="server"></f:HiddenField>
    <f:HiddenField ID="hfdBillno" runat="server"></f:HiddenField>
    <f:Window ID="Window1" Title="单据信息" Hidden="true" EnableIFrame="true" runat="server"
        EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Width="820px" Height="480px">
    </f:Window>
</body>
</html>
