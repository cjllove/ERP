﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoodsQuery.aspx.cs" Inherits="ERPProject.ERPQuery.GoodsQuery" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>商品资料查询</title>
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
                        <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="提示信息：商品资料查询主界面！" runat="server" />
                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                        <%--<f:Button ID="btnPrint" Icon="Printer" Text="打 印" DisableControlBeforePostBack="true" runat="server" />
                        <f:ToolbarSeparator runat="server" />--%>
                        <f:Button ID="btnExport" Icon="PageExcel" Text="导 出" DisableControlBeforePostBack="false" ConfirmText="是否确认导出此商品资料信息?" runat="server" EnableAjax="false" OnClick="btnExport_Click" EnableDefaultState="false" />
                        <f:ToolbarSeparator runat="server" />
                        <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" DisableControlBeforePostBack="true" runat="server" OnClick="btSearch_Click" EnableDefaultState="false" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="5px"
                    ShowHeader="False" LabelWidth="75px" runat="server">
                    <Rows>
                        <f:FormRow>
                            <Items>
                                <f:TriggerBox ID="trbxSearch" runat="server" Label="商品信息" ShowTrigger="false" MaxLength="20" EmptyText="可输入编码、名称或助记码" TriggerIcon="Search" OnTriggerClick="btSearch_Click"></f:TriggerBox>
                                <f:DropDownList ID="ddlCATID0" runat="server" Label="商品分类" EnableEdit="true" ForceSelection="true">
                                </f:DropDownList>
                                <f:DropDownList ID="ddlFLAG" runat="server" Label="商品状态" EnableEdit="false">
                                    <f:ListItem Text="---请选择---" Value="" />
                                    <f:ListItem Text="新增" Value="N" />
                                    <f:ListItem Text="正常" Value="Y" />
                                    <f:ListItem Text="停用" Value="S" />
                                    <f:ListItem Text="停购" Value="T" />
                                    <f:ListItem Text="淘汰" Value="E" />
                                </f:DropDownList>
                                <f:DropDownList runat="server" ID="ddlISFLAG7" Label="商品类型">
                                    <f:ListItem Text="-- 全部 --" Value="" />
                                    <f:ListItem Text="下传商品" Value="N" />
                                    <f:ListItem Text="本地商品" Value="Y" />
                                </f:DropDownList>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:DropDownList ID="ddlSUPID" runat="server" Label="供应商" EnableEdit="true" ForceSelection="true">
                                </f:DropDownList>
                                <f:DropDownList ID="ddlPASSID" runat="server" Label="配送商" EnableEdit="true" ForceSelection="true">
                                </f:DropDownList>
                                <f:TriggerBox ShowTrigger="false" runat="server" ID="tgbPRO" Label="生产厂家" MaxLength="20" EmptyText="输入厂家信息"></f:TriggerBox>
                                <f:DropDownList runat="server" ID="ddlISGZ" Label="是否高值">
                                    <f:ListItem Text="-- 全部 --" Value="" />
                                    <f:ListItem Text="是" Value="Y" />
                                    <f:ListItem Text="否" Value="N" />
                                </f:DropDownList>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                
                                <f:DropDownList ID="JSMODE" runat="server" Label="结算模式">
                                    <f:ListItem Text="--请选择--" Value="" />
                                    <f:ListItem Text="入库结" Value="R" />
                                    <f:ListItem Text="出库结" Value="C" />
                                    <f:ListItem Text="销售结" Value="X" />
                                </f:DropDownList>
                                <f:DropDownList ID="GoodsMode" runat="server" Label="供应模式">
                                    <f:ListItem Text="--请选择--" Value="" />
                                    <f:ListItem Text="托管" Value="0" />
                                    <f:ListItem Text="直供" Value="Z" />
                                </f:DropDownList>
                                <f:DropDownList runat="server" ID="ddlISTY" Label="是否停用">
                                    <f:ListItem Text="-- 全部 --" Value="" />
                                    <f:ListItem Text="正常" Value="Y" />
                                    <f:ListItem Text="停购" Value="T" />
                                    <f:ListItem Text="停销" Value="S" />
                                    <f:ListItem Text="新品" Value="N" />
                                    <f:ListItem Text="淘汰" Value="E" />
                                </f:DropDownList>
                                <f:DropDownList runat="server" ID="ddlSTR6" Label="是否计费">
                                    <f:ListItem Text="--请选择--" Value="" />
                                    <f:ListItem Text="是" Value="Y" />
                                    <f:ListItem Text="否" Value="N" />
                                </f:DropDownList>
 
                                                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
                <f:Grid ID="GridGoods" BoxFlex="1" ShowBorder="false" ShowHeader="false"
                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableTextSelection="true"
                    PageSize="100" DataKeyNames="GDSEQ" IsDatabasePaging="true" AllowPaging="true" OnPageIndexChange="GridGoods_PageIndexChange" EnableColumnLines="true">
                    <Columns>
                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                        <f:BoundField Width="130px" DataField="KFCFG" HeaderText="已配置库房" />
                        <f:BoundField Width="105px" DataField="GDID" HeaderText="商品编码" TextAlign="Center" />
                        <f:BoundField Width="105px" DataField="BAR3" HeaderText="ERP编码" TextAlign="Center" />
                        <f:BoundField Width="190px" DataField="GDNAME" HeaderText="商品名称" />
                        <f:BoundField Width="90px" DataField="BARCODE" HeaderText="商品条码" Hidden="true" />
                        <f:BoundField Width="60px" DataField="FLAGNAME" HeaderText="状态" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="CATID0NAME" HeaderText="商品种类" TextAlign="Center" />
                        <f:BoundField Width="130px" DataField="CATIDNAME" HeaderText="商品类别" TextAlign="Center" />
                        <f:BoundField Width="0px" Hidden="true" DataField="ISFLAG7" HeaderText="商品类型" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="ISFLAG7_CN" HeaderText="商品类型" TextAlign="Center" />
                        <f:BoundField Width="60px" DataField="JXNAME" HeaderText="剂形" Hidden="true" />
                        <f:BoundField Width="60px" DataField="YXNAME" HeaderText="药效" Hidden="true" />
                        <f:BoundField Width="120px" DataField="GDSPEC" HeaderText="规格·容量" />
                        <f:BoundField Width="90px" DataField="UNITNAME" HeaderText="最小单位" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="HSJJ" HeaderText="含税进价" TextAlign="Right" DataFormatString="{0:F4}" />
                        <f:BoundField Width="80px" DataField="LSJ" HeaderText="售价" TextAlign="Right" DataFormatString="{0:F4}" Hidden="true" />
                        <f:BoundField Width="130px" DataField="UNITORDERNAME" HeaderText="订货单位" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="NUMORDER" HeaderText="订货单位含量" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="UNITSELLNAME" HeaderText="出库单位" TextAlign="Center" />
                        <f:BoundField Width="130px" DataField="NUMSELL" HeaderText="出库单位含量" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="STR3NAME" HeaderText="结算模式" />
                        <f:BoundField Width="80px" DataField="TYPE" HeaderText="供应模式" />
                        <f:BoundField Width="180px" DataField="CD" HeaderText="产地" />
                        <f:BoundField Width="180px" DataField="SUPNAME" HeaderText="供应商" />
                        <f:BoundField Width="180px" DataField="PSSNAME" HeaderText="配送商" />
                        <f:BoundField Width="180px" DataField="PIZNO" HeaderText="注册证号" />
                        <f:BoundField Width="105px" DataField="HISCODE" HeaderText="HIS编码" TextAlign="Center" />
                        <f:BoundField Width="190px" DataField="HISNAME" HeaderText="HIS名称" TextAlign="Left" />
                        <f:BoundField Width="120px" DataField="STR3" HeaderText="HIS规格" TextAlign="Left" />
                        <f:BoundField Width="0px" DataField="ZPBH" HeaderText="制品编号" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
</body>
</html>
