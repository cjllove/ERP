﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InitialStock.aspx.cs" Inherits="ERPProject.ERPStorage.InitialStock" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <style type="text/css" media="all">
        .f-grid-row[data-color=color1],
        .f-grid-row[data-color=color1] .ui-icon,
        .f-grid-row[data-color=color1] a {
            background-color: red;
            color: #fff;
        }
    </style>

</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="PanelDetail" runat="server" />
        <f:Panel ID="PanelDetail" runat="server" ShowBorder="false" BodyPadding="0px" Layout="HBox" ShowHeader="False">
            <Items>
                <f:Panel ID="PanelPicture" runat="server" ShowHeader="false" BoxFlex="2" ShowBorder="false"
                    BodyPadding="0px" Layout="Fit">
                    <Items>
                        <f:Grid ID="GridStock" ShowBorder="false" ShowHeader="false" EnableCheckBoxSelect="false" IsDatabasePaging="true"
                            AllowSorting="true" AutoScroll="true" runat="server" EnableColumnLines="true" AllowPaging="true" PageSize="50"
                            DataKeyNames="IMPSEQ" OnPageIndexChange="GridStock_PageIndexChange" OnRowDataBound="GridList_RowDataBound">
                            <Toolbars>
                                <f:Toolbar ID="Toolbar2" runat="server">
                                    <Items>
                                        <f:DropDownList ID="ddlTYPE" runat="server" Label="导入类型" LabelWidth="70px">
                                            <f:ListItem Text="入库单" Value="RKD" Selected="true" />
                                            <f:ListItem Text="损益单" Value="SYD" />
                                        </f:DropDownList>
                                        <f:ToolbarSeparator ID="ToolbarSeparator1" runat="server"></f:ToolbarSeparator>
                                        <f:ToolbarText ID="ToolbarText1" Text="导入初始库存excel文件:" EnableAjax="true" runat="server" />
                                        <f:FileUpload runat="server" ID="fuDocument" EmptyText="导入EXCEL文件" Width="300" ShowRedStar="true" AutoPostBack="true" OnFileSelected="btnSelect_Click" />

                                        <f:ToolbarFill runat="server"></f:ToolbarFill>
                                        <f:Button runat="server" ID="btnClear" Icon="Delete" Hidden="true" Text="清空列表数据" DisableControlBeforePostBack="false" Enabled="true" OnClick="btnClear_Click" EnableDefaultState="false"></f:Button>
                                        <f:ToolbarSeparator runat="server"></f:ToolbarSeparator>
                                        <f:Button runat="server" ID="btnSave" Icon="Disk" Enabled="false" ConfirmText="是否提交数据生成对应的单据？" EnableDefaultState="false"
                                            Text="提 交" DisableControlBeforePostBack="false" ValidateForms="FormMain" OnClick="btnSave_Click">
                                        </f:Button>
                                        <f:LinkButton ID="LinkButton1" Text="下载模板" EnablePostBack="false" OnClientClick="DownLoadModelclick();" runat="server" />
                                    </Items>
                                </f:Toolbar>
                            </Toolbars>
                            <Columns>
                                <f:RowNumberField Width="35px" EnablePagingNumber="true" />
                                <f:BoundField Width="160px" ColumnID="MEMO" DataField="MEMO" HeaderText="提示信息" />
                                <f:BoundField Width="110px" DataField="DEPTIDNAME" HeaderText="科室/库房" TextAlign="Center" />
                                <f:BoundField Width="100px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                                <f:BoundField Width="150px" DataField="GDNAME" HeaderText="商品名称" />
                                <f:BoundField Width="120px" DataField="GDSPEC" HeaderText="商品规格" />
                                <f:BoundField Width="60px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                                <f:BoundField Width="80px" DataField="SL" HeaderText="数量" TextAlign="Center" />
                                <f:BoundField Width="100px" DataField="PH" HeaderText="批号" TextAlign="Center" />
                                <f:BoundField Width="80px" DataField="HSJJ" HeaderText="价格" TextAlign="Center" />
                                <f:BoundField Width="100px" DataField="RQ_SC" HeaderText="生产日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                <f:BoundField Width="100px" DataField="YXQZ" HeaderText="有效期至" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                <f:BoundField Width="120px" DataField="SUPPLIERNAME" HeaderText="供应商" TextAlign="Center" />
                                <f:BoundField Width="120px" DataField="PRODUCERNAME" HeaderText="配送商" TextAlign="Center" />
                                <f:BoundField Width="130px" DataField="PIZNO" HeaderText="注册证号" TextAlign="Center" />
                                <f:BoundField Width="130px" DataField="FLAG" HeaderText="状态" TextAlign="Center" Hidden="true" />
                            </Columns>
                        </f:Grid>
                    </Items>
                </f:Panel>
            </Items>
        </f:Panel>

    </form>
    <script type="text/javascript">
        function DownLoadModelclick() {
            window.location.href = '初始库存模板.xlsx';
        }
    </script>
</body>
</html>
