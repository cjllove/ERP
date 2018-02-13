﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoodsDepartmentInquiry.aspx.cs" Inherits="ERPProject.ERPApply.GoodsDepartmentInquiry" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>科室商品查询</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" ShowBorder="false" BodyPadding="0px" Layout="Anchor" ShowHeader="False" runat="server">
            <Items>
                <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                    ShowHeader="False" LabelWidth="80px" runat="server">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar1" runat="server">
                            <Items>
                                <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="操作提示信息：完全显示会将科室未配置定数的商品一同显示" runat="server" />
                                <f:CheckBox ID="ShowMode" runat="server" Text="完全显示" LabelWidth="75px" ShowRedStar="true" CssStyle="margin-left:50px"></f:CheckBox>
                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                <f:Button ID="btExport" Icon="DatabaseGo" Text="导 出" EnableDefaultState="false" ConfirmText="是否确认导出商品定数数据?" EnablePostBack="true" runat="server" OnClick="btExport_Click" EnableAjax="false" DisableControlBeforePostBack="false" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="btnCollect" Icon="UserTick" Text="收 藏" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否确定收藏选中商品？" OnClick="bntCollect_Click" />
                                <f:Button ID="btnDelCollect" Icon="UserCross" Text="取消收藏" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否确定取消该商品收藏？" OnClick="bntDelCollect_Click" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="bntSearch_Click" />
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Rows>
                        <f:FormRow ColumnWidths="25% 25% 25% 25%">
                            <Items>
                                <f:TextBox ID="tbxGDSEQ" runat="server" Label="商品信息" MaxLength="20" EmptyText="请输入商品编码或商品名称" />
                                <f:DropDownList ID="lstDEPTID" runat="server" Label="管理科室" EnableEdit="true" ForceSelection="true">
                                </f:DropDownList>
                                <f:TextBox ID="CZR" Label="操作人" runat="server"></f:TextBox>
                                <f:DatePicker ID="CZSJ" Label="操作时间" runat="server"></f:DatePicker>
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
                <f:Grid ID="GridList" AnchorValue="100% -83" ShowBorder="false" ShowHeader="false" EnableCheckBoxSelect="true"
                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;"
                    DataKeyNames="DEPTID,GDSEQ,DSNUM,NUM1" EnableColumnLines="true" EnableTextSelection="true"
                    IsDatabasePaging="true" AllowPaging="true" PageSize="100" OnPageIndexChange="GridList_PageIndexChange"
                    OnRowDataBound="GridList_RowDataBound" EnableSummary="true" SummaryPosition="Bottom"
                    EnableMultiSelect="true" CheckBoxSelectOnly="true">
                    <Columns>
                        <f:BoundField Width="100px" DataField="DEPTIDname" HeaderText="管理科室" TextAlign="Center" />
                        <f:BoundField Width="0" DataField="DEPTID" HeaderText="" TextAlign="Center" Hidden="true" />
                        <f:BoundField Width="110px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                        <f:BoundField Width="200px" DataField="GDNAME" ColumnID="GDNAME" HeaderText="商品名称" TextAlign="Left" />
                        <f:BoundField Width="80px" DataField="GDSPEC" HeaderText="商品规格" TextAlign="Left" />
                        <f:BoundField Width="50px" DataField="UNIT" HeaderText="单位" TextAlign="Center" />
                        <f:BoundField Width="240px" DataField="PRODUCERNAME" HeaderText="生产商" TextAlign="Left" />
                        <f:BoundField Width="210px" DataField="PIZNO" HeaderText="注册证号" TextAlign="Left" />
                        <f:BoundField Width="100px" DataField="COLLECT" HeaderText="收藏状态" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="DSNUM" HeaderText="定数数量" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="NUM1" HeaderText="定数含量" TextAlign="Center" />
                        <f:BoundField Width="0px" DataField="NUM2" HeaderText="定数流水" TextAlign="Center" Hidden="true" />
                        <f:BoundField Width="80px" DataField="NUM3" HeaderText="待收定数" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="ZDKC" HeaderText="最低库存" TextAlign="Center" Hidden="true" />
                        <f:BoundField Width="80px" DataField="ZGKC" HeaderText="最高库存" TextAlign="Center" Hidden="true" />
                        <f:BoundField Width="80px" DataField="DSPOOL" ColumnID="DSPOOL" HeaderText="定数预占" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="YCDS" ColumnID="YCDS" HeaderText="应出定数" TextAlign="Center" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
</body>
</html>