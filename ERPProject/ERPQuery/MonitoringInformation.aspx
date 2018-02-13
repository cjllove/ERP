﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MonitoringInformation.aspx.cs" Inherits="ERPProject.ERPQuery.MonitoringInformation" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" AjaxLoadingType="Mask" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
            ShowHeader="false">
            <Items>
                <f:Panel ID="PanelBody" ShowBorder="false" BodyPadding="0px"
                    Layout="Anchor" ShowHeader="False" runat="server">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar11" runat="server">
                            <Items>
                                <f:ToolbarText ID="summsg" EnableAjax="true" CssStyle="" Text="统计信息：暂无统计数据！" runat="server" />
                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
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
                                        <f:DatePicker ID="dpkKSRQ" runat="server" Label="开始日期" Required="true" ShowRedStar="true"></f:DatePicker>
                                        <f:DatePicker ID="dpkJSRQ" runat="server" Label="结束日期" Required="true" ShowRedStar="true"></f:DatePicker>
                                        
                                        <f:CheckBox ID="chkCHULI" runat="server" Label="" Text="是否处理"></f:CheckBox>
                                    </Items>
                                </f:FormRow>
                              
                            </Rows>
                        </f:Form>
                    </Items>
                </f:Panel>
                <f:Grid ID="GridList" runat="server" AnchorValue="100% -64" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;"
                    PageSize="100" IsDatabasePaging="true" AllowPaging="true" OnPageIndexChange="GridList_PageIndexChange"
                    AllowSorting="true" AutoScroll="true" SortDirection="ASC" EnableTextSelection="true"
                    DataKeyNames="SEQNO" OnRowCommand="GridList_RowCommand">
                    <Columns>
                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                        <f:BoundField Width="150px" DataField="RQSJ" SortField="RQSJ" HeaderText="发生日期" />
                        <f:BoundField Width="250px" DataField="MEMO" SortField="MEMO" HeaderText="信息说明" ExpandUnusedSpace="true" />
                        <f:BoundField Width="130px" DataField="TYPENAME" SortField="TYPENAME" HeaderText="信息类别" />
                        <f:BoundField Width="130px" DataField="FLAG" SortField="FLAG" TextAlign="Center" HeaderText="处理状态" />
                        
                        <f:LinkButtonField Text="处理"  ConfirmTarget="Top" CommandName="EXEC" 
                            Width="50px" />
                        <f:BoundField Width="0px" DataField="SEQNO" SortField="SEQNO" HeaderText="SEQNO"/>
                        <f:BoundField Width="0px" DataField="TAB" SortField="TAB" HeaderText="TAB"/>
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
</body>
</html>