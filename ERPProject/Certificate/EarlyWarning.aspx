﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EarlyWarning.aspx.cs" Inherits="ERPProject.Certificate.EarlyWarning" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>证照到期预警</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="PanelMain" runat="server" />
        <f:Panel ID="PanelMain" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
            ShowHeader="false">
            <Items>
                <f:Panel ID="Panel3" ShowBorder="false" BodyPadding="0px"
                    Layout="Anchor" ShowHeader="False" runat="server">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar2" runat="server">
                            <Items>
                                <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="提示信息：证照预警提醒主界面！" runat="server" />
                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="bntClear" Icon="Erase" Text="清 除" EnablePostBack="true" runat="server" OnClick="bntClear_Click" EnableDefaultState="false" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="btnSendMessage" Icon="BellStart" Text="提 醒" EnablePostBack="true" runat="server" OnClick="btnSendMessage_Click" EnableDefaultState="false" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnablePostBack="true" runat="server" OnClick="bntSearch_Click" EnableDefaultState="false" />
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Items>
                        <f:Form ID="FormQuery" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                            ShowHeader="False" LabelWidth="80px" runat="server">
                            <Rows>
                                <f:FormRow ColumnWidths="35% 25% 20% 20%">
                                    <Items>
                                        <f:DropDownList ID="lstSUPID" runat="server" Label="供应商" EnableEdit="true" ForceSelection="true" />
                                        <f:DropDownList ID="lstLICENSETYPE" runat="server" Label="证照类型" EnableEdit="true" ForceSelection="true" />
                                        <f:DatePicker ID="lstLRRQ1" runat="server" Label="查询日期" Required="true" ShowRedStar="true" />
                                        <f:DatePicker ID="lstLRRQ2" runat="server" Label="　　至" LabelSeparator="" />
                                    </Items>
                                </f:FormRow>
                            </Rows>
                        </f:Form>
                    </Items>
                </f:Panel>
                <f:Grid ID="GridList" AnchorValue="100% -75" ShowBorder="false" ShowHeader="false"
                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" AllowPaging="true" PageSize="100"
                    DataKeyNames="SEQNO" EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridList_RowDoubleClick" OnPageIndexChange="GridList_PageIndexChange">
                    <Columns>
                        <f:BoundField DataField="SEQNO" Hidden="true" />
                        <f:BoundField DataField="SUPID" Hidden="true" />
                        <f:BoundField DataField="LICENSEID" Hidden="true" />
                        <f:BoundField DataField="SUPNAME" HeaderText="供应商" MinWidth="150px" ExpandUnusedSpace="true" />
                        <f:BoundField Width="100px" DataField="LICENSENAME" HeaderText="证照类型" />
                        <f:BoundField Width="80px" DataField="FLAG" HeaderText="证照状态" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="BEGRQ" HeaderText="发证日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="80px" DataField="ENDRQ" HeaderText="有效期至" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="60px" DataField="LRY" HeaderText="录入员" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="LRRQ" HeaderText="录入日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="60px" DataField="SHR" HeaderText="审核员" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="SHRQ" HeaderText="审核日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="100px" DataField="MEMO" HeaderText="备注" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
        <f:Window ID="WindowMessage" Title="证照到期提醒" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="490px" Height="230px">
            <Items>
                <f:Form ID="Form2" ShowBorder="false" AutoScroll="false" BodyPadding="5px 10px"
                    ShowHeader="False" LabelWidth="70px" runat="server">
                    <Rows>
                        <f:FormRow Hidden="true">
                            <Items>
                                <f:Label ID="lblMessage" runat="server" Label="基本信息" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow Hidden="true">
                            <Items>
                                <f:TextArea ID="txaMemo" runat="server" Label="附加信息" Height="100px" MaxLength="80" />
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar4" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="btnSubmit" Text="确定" Icon="SystemSave" runat="server" OnClick="btnSubmit_Click" EnableDefaultState="false">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
    </form>
</body>
</html>