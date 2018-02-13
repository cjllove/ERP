﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApplyAudit.aspx.cs" Inherits="ERPProject.ERPApply.ApplyAudit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>申领信息审核</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="Panel1"
            runat="server" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="True" BodyPadding="0px" Layout="Anchor"
            ShowHeader="false">
            <Items>
                <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                    ShowHeader="False" LabelWidth="70px" runat="server">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar1" runat="server">
                            <Items>
                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                <f:Button ID="btnAudit" Icon="UserTick" Text="审 核" ConfirmText="是否确认审核此单据?" runat="server" OnClick="btnAudit_Click" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" DisableControlBeforePostBack="true" EnablePostBack="true" runat="server" OnClick="btSearch_Click" />
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Rows>
                        <f:FormRow>
                            <Items>
                                <f:TextBox ID="lstBILLNO" runat="server" Label="单据编号" />
                                <f:DropDownList ID="lstDEPTID" runat="server" Label="收货科室" EnableEdit="true" ForceSelection="true" />
                                <f:DropDownList ID="lstFLAG" runat="server" Label="单据状态" EnableEdit="true" ForceSelection="true" />
                                <f:DropDownList ID="lstSLR" runat="server" Label="申领人" EnableEdit="true" ForceSelection="true" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:HiddenField ID="hfd" runat="server" />
                                <f:DropDownList ID="lstDEPTOUT" runat="server" Label="出库部门" EnableEdit="true" ForceSelection="true" />
                                <f:DatePicker ID="lstLRRQ1" runat="server" Label="录入日期" />
                                <f:DatePicker ID="lstLRRQ2" runat="server" Label="　　至" LabelSeparator="" />
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>

                <f:Grid ID="GridList" AnchorValue="100% -95" ShowBorder="true" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server"
                    DataKeyNames="SEQNO" EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridList_RowDoubleClick">
                    <Columns>
                        <f:BoundField DataField="SEQNO" Hidden="true" />
                        <f:BoundField Width="100px" DataField="BILLNO" HeaderText="单据编号" TextAlign="Center" />
                        <f:BoundField Width="60px" DataField="FLAG" HeaderText="单据状态" TextAlign="Center" />
                        <f:BoundField Width="150px" DataField="DEPTOUT" HeaderText="出库部门" TextAlign="Center" />
                        <f:BoundField Width="150px" DataField="DEPTID" HeaderText="申领科室" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="XSRQ" HeaderText="申领日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="70px" DataField="SUBNUM" HeaderText="商品种类" TextAlign="Right" DataFormatString="{0:F2}" />
                        <f:BoundField Width="70px" DataField="SLR" HeaderText="申领人" TextAlign="Center" />
                        <f:BoundField Width="70px" DataField="LRY" HeaderText="录入员" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="LRRQ" HeaderText="录入日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="70px" DataField="SHR" HeaderText="审核员" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="SHRQ" HeaderText="审核日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="100px" DataField="MEMO" HeaderText="备注" TextAlign="Center" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
</body>
</html>