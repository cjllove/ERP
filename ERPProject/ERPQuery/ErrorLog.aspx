﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ErrorLog.aspx.cs" Inherits="ERPProject.ERPQuery.ErrorLog" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="Panel1"
            runat="server" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBOX"
            ShowHeader="false">
            <Items>
                <f:Form ID="FormCond" runat="server" ShowBorder="false" BodyPadding="10px" ShowHeader="False" LabelWidth="90px">
                    <Rows>
                        <f:FormRow>
                            <Items>
                                <f:DropDownList ID="lstType" runat="server" Label="错误类型" EnableEdit="true" ForceSelection="true"/>
                                <f:DropDownList ID="lstAccount" runat="server" Label="登陆账号" EnableEdit="true" ForceSelection="true"/>
                                <f:DatePicker ID="lstLRRQ1" runat="server" Label="开始日期" />
                                <f:DatePicker ID="lstLRRQ2" runat="server" Label="结束日期" />
                                <f:Button ID="btnSearch" CssStyle="float:right;" OnClick="btnSearch_Click" Icon="Magnifier" EnableDefaultState="false"
                                    Text="查 询" runat="server">
                                </f:Button>
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
                <f:Grid ID="GridLog" BoxFlex="1" AnchorValue="100% -70" ShowBorder="false" ShowHeader="false"  CssStyle="border-top: 1px solid #99bce8;"
                    AllowSorting="true" OnSort="GridLog_Sort" AutoScroll="true" runat="server" DataKeyNames="userid" EnableColumnLines="true" >
                    <Columns>
                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                        <f:BoundField Width="120px" DataField="USERID" HeaderText="登录账号" />
                        <f:BoundField Width="180px" DataField="MEMO" HeaderText="错误描述" ExpandUnusedSpace="true" />
                        <f:BoundField Width="230px" DataField="STATION" HeaderText="IP地址" />
                        <f:BoundField Width="180px" DataField="RQSJ" HeaderText="操作时间" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
        <f:Window ID="Window1" runat="server" IsModal="true" Hidden="true" Target="Parent"
            EnableIFrame="true" IFrameUrl="about:blank" Width="820px" Height="313px" AutoScroll="false"
            OnClose="Window1_Close">
        </f:Window>
    </form>
</body>
</html>