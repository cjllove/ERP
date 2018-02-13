﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SupplierEntrust.aspx.cs" Inherits="ERPProject.ERPEntrust.SupplierEntrust" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>供应商配置管理</title>
    <style type="text/css">
        .ui-state-disabled{
          opacity: .5;
          filter: alpha(opacity=50);
          background-image: none;
        }
    </style>
</head>

<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="true" AjaxLoadingType="Mask" EnableAjax="true" ID="PageManager1" AutoSizePanelID="PanelDetail" runat="server"/> 
        <f:Panel ID="PanelDetail" ShowBorder="false"  AnchorValue="100% " BodyPadding="0px" Layout="HBox"
            BoxConfigAlign="Stretch" BoxConfigPosition="Start" ShowHeader="False" runat="server">
            <Items>
                <f:Grid ID="GridSupplier" ShowBorder="false" ShowHeader="false" AllowSorting="false" CssStyle="border-right: 1px solid #99bce8;border-bottom: 1px solid #99bce8;"
                    BoxFlex="1" AutoScroll="true" runat="server" DataKeyNames="SUPID" EnableMultiSelect="true" EnableAjaxLoading="true" EnableCheckBoxSelect="true"
                    AllowPaging="true" IsDatabasePaging="true" OnPageIndexChange="GridSupplier_PageIndexChange" PageSize="100" KeepCurrentSelection="true">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar1" runat="server">
                            <Items>
                                <f:Label Width="200px" Text="数据中心供应商信息" runat="server" />
                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                <%--<f:Button ID="btnSearchLeft" Text="查询" runat="server" OnClick="btnSearch_Click" />--%>
                                <f:TriggerBox ID="tgbSearch" Width="200px" runat="server" EmptyText="供应商编码或名称" TriggerIcon="Search" OnTriggerClick="btnSearch_Click" />
                                
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Columns>
                        <f:BoundField Width="100px" DataField="SUPID"  HeaderText="供应商编码" />
                        <f:BoundField Width="230px" DataField="SUPNAME" HeaderText="供应商名称" ExpandUnusedSpace="true" />
                        <f:BoundField Width="90px" DataField="YYZZNO" HeaderText="营业执照" />
                        <f:BoundField Width="120px" DataField="LOGINRQ" HeaderText="注册日期" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="STR1" HeaderText="ERP编码" TextAlign="Center" />
                    </Columns>
                </f:Grid>
                <f:HiddenField ID="hfdGridSupplier" runat="server" />
                <f:HiddenField ID="hfdGridSupDisabled" runat="server"/>
                <f:HiddenField ID="hfdGridSupDisabled1" runat="server"/>
                <f:Panel ID="Panel3" Width="50px" runat="server" BodyPadding="5px" ShowBorder="false" ShowHeader="false"
                    Layout="VBox" BoxConfigAlign="Stretch" BoxConfigPosition="Start" BodyStyle="background-color:#d9e7f8;">
                    <Items>
                        <f:Panel ID="Panel2" Title="面板1" BoxFlex="1" runat="server" BodyStyle="background-color:#d9e7f8;"
                            BodyPadding="5px" ShowBorder="false" ShowHeader="false">
                            <Items>
                            </Items>
                        </f:Panel>
                        <f:Panel ID="Panel4" Height="70px" runat="server" BodyStyle="background-color:#d9e7f8;"
                            BodyPadding="4px" ShowBorder="false" ShowHeader="false">
                            <Items>
                                <f:Button ID="btnAddRight" runat="server" Text=">>" EnableDefaultState="false" CssStyle="margin-bottom:10px;" OnClick="btnAddRight_Click"></f:Button>
                                <f:Button ID="btnAddLeft" runat="server" Text="<<" EnableDefaultState="false" OnClick="btnAddLeft_Click"></f:Button>
                            </Items>
                        </f:Panel>
                        <f:Panel ID="Panel5" BoxFlex="1" Margin="0" BodyStyle="background-color:#d9e7f8;"
                            runat="server" BodyPadding="5px" ShowBorder="false" ShowHeader="false">
                            <Items>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Panel>
                <f:Grid ID="GridCFGSupplier" ShowBorder="false" ShowHeader="false" AllowSorting="false" BoxFlex="1" CssStyle="border-left: 1px solid #99bce8;border-bottom: 1px solid #99bce8;"
                    AutoScroll="true" runat="server" DataKeyNames="SUPID" EnableCheckBoxSelect="true"
                    PageSize="50" IsDatabasePaging="true" AllowPaging="true" OnPageIndexChange="GridCFGSupplier_PageIndexChange" EnableMultiSelect="true" KeepCurrentSelection="true">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar2" runat="server">
                            <Items>
                                <f:Label Width="200px" Text="本地代管供应商信息" runat="server" />
                                <f:ToolbarFill ID="ToolbarFill3" runat="server" />
                                <f:TriggerBox ID="trbSearch" Width="200px" runat="server" EmptyText="供应商编码或名称" TriggerIcon="Search" OnTriggerClick="trbSearch_TriggerClick" EnableAjaxLoading="true" />

                               <%-- <f:Button ID="btnSave" Hidden="true" CssStyle="margin-left: 15px;margin-right: 11px;" OnClick="btnSave_Click"
                                    Icon="Disk" Text="保 存" DisableControlBeforePostBack="false" runat="server" ValidateForms="FormConfig" />--%>
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Columns>
                        <%--<f:BoundField DataField="GDSEQ" Hidden="true" />--%>
                        <f:BoundField Width="100px" DataField="SUPID" HeaderText="供应商编码" ColumnID="SUPID"/>
                        <f:BoundField Width="230px" DataField="SUPNAME" HeaderText="供应商名称" ExpandUnusedSpace="true" />
                        <f:BoundField Width="90px" DataField="YYZZNO" HeaderText="营业执照" />
                        <f:BoundField Width="90px" DataField="LOGINRQ" HeaderText="注册日期" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="STR1" HeaderText="ERP编码" TextAlign="Center" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>

    </form>
</body>
<script type="text/javascript" src="../res/js/jquery.FineUIPro.disablegrid.js"></script>
<script type="text/javascript">
    var GridSupplier = '<%= GridSupplier.ClientID%>';
    var GridCFGSupplier = '<% = GridCFGSupplier.ClientID%>';
    var hfdGridSupDisabled = '<%= hfdGridSupDisabled.ClientID%>';
    var hfdGridSupDisabled1 = '<%= hfdGridSupDisabled1.ClientID%>';

    ////右侧窗口供应商需要禁止删除'00001'供应商
    function setDisableYiyuan() {
        $('#' + GridCFGSupplier).find(F(GridCFGSupplier).getView().itemSelector).each(function (i) {
            if (F(GridCFGSupplier).getStore().getAt(i).get('SUPID') == '00001') {
                Ext.get(F(GridCFGSupplier).getView().getNode(i)).addCls('ui-state-disabled');
            }
        });
    }

    function beforeSelectYiyuan(a, b, c) {
        if (this.getStore().getAt(c).get('SUPID') == '00001') {
            return false;
        }
    }
    $.disablegrid({ grid: GridSupplier, hfdDisabled: hfdGridSupDisabled });
    $.disablegrid({ grid: GridCFGSupplier, hfdDisabled: hfdGridSupDisabled1 });
</script>
</html>