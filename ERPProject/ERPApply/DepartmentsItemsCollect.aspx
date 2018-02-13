﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DepartmentsItemsCollect.aspx.cs" Inherits="ERPProject.ERPApply.DepartmentsItemsCollect" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>科室收藏商品</title>
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
                                <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="操作提示信息：默认数量为申领数,追加收藏的商品需要点击保存" runat="server" />
                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button runat="server" ID="btnSave" CssStyle="margin-left: 10px;margin-right: 10px;" Icon="Disk" Text="保 存" DisableControlBeforePostBack="false" OnClick="btnBill_Click" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="btnExport" Icon="PageExcel" Text="导 出" DisableControlBeforePostBack="false" ConfirmText="是否确认导出此商品资料信息?" runat="server" EnableAjax="false" OnClick="btnExport_Click" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnablePostBack="true" runat="server" OnClick="btnBill_Click"/>
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="btnDelRow" Icon="Delete" Text="删 行" EnablePostBack="true" runat="server" ConfirmText="确定要取消收藏该商品？" OnClick="btnBill_Click" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="btnGoods" Icon="Magnifier" Text="追加收藏" EnablePostBack="true" runat="server" OnClick="btnBill_Click"  />
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Rows>
                        <f:FormRow ColumnWidths="30% 50% 20%">
                            <Items>
                                <f:DropDownList ID="lstDEPTID" runat="server" Label="当前科室" />
                                <f:TriggerBox ID="tgbGDSEQ" runat="server" Label="商品信息" ShowTrigger="false" OnTriggerClick="tgbGDSEQ_TriggerClick"></f:TriggerBox>
                                <f:RadioButtonList ID="rblRange" runat="server" Label="查询范围">
                                    <f:RadioItem Text="全部人员" Value="0" />
                                    <f:RadioItem Text="当前操作员" Value="1" Selected="true" />
                                </f:RadioButtonList>
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
                <f:Grid ID="GridList" AnchorValue="100% -65" ShowBorder="false" ShowHeader="false"
                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;"
                    DataKeyNames="DEPTID,GDSEQ,MRSL" AllowCellEditing="true" ClicksToEdit="1">
                    <Columns>
                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                        <f:RenderField Width="100px" ColumnID="DEPTIDNAME" DataField="DEPTIDNAME" FieldType="String" EnableHeaderMenu="false"
                            HeaderText="管理科室" TextAlign="Center">
                            <Editor>
                                <f:Label ID="lblDEPTIDname" runat="server" />
                            </Editor>
                        </f:RenderField>
                        <f:RenderField Width="110px" ColumnID="GDSEQ" DataField="GDSEQ" FieldType="String" EnableHeaderMenu="false"
                            HeaderText="商品编码" TextAlign="Center">
                            <Editor>
                                <f:Label ID="lblauto" runat="server" />
                            </Editor>
                        </f:RenderField>
                        <f:RenderField Width="200px" ColumnID="GDNAME" DataField="GDNAME" FieldType="String" EnableHeaderMenu="false"
                            HeaderText="商品名称" TextAlign="Left">
                            <Editor>
                                <f:Label ID="lblGDNAME" runat="server" />
                            </Editor>
                        </f:RenderField>
                        <f:RenderField Width="80px" ColumnID="GDSPEC" DataField="GDSPEC" FieldType="String" EnableHeaderMenu="false"
                            HeaderText="商品规格" TextAlign="Left">
                            <Editor>
                                <f:Label ID="lblGDSPEC" runat="server" />
                            </Editor>
                        </f:RenderField>
                        <f:RenderField Width="90px" DataField="DEFSL" ColumnID="DEFSL" HeaderText="默认数量" TextAlign="Center" FieldType="Auto" EnableHeaderMenu="false">
                            <Editor>
                                <f:NumberBox ID="nbxDEFSL" runat="server" MinValue="0" MaxValue="99999999" DecimalPrecision="2" />
                            </Editor>
                        </f:RenderField>
                        <f:RenderField Width="50px" ColumnID="UNITNAME" DataField="UNITNAME" FieldType="String" EnableHeaderMenu="false"
                            HeaderText="单位" TextAlign="Center">
                            <Editor>
                                <f:Label ID="lblUNITNAME" runat="server" />
                            </Editor>
                        </f:RenderField>
                        <f:RenderField Width="240px" ColumnID="PRODUCERNAME" DataField="PRODUCERNAME" FieldType="String" EnableHeaderMenu="false"
                            HeaderText="生产商" TextAlign="Left">
                            <Editor>
                                <f:Label ID="lblPRODUCERNAME" runat="server" />
                            </Editor>
                        </f:RenderField>
                        <f:RenderField Width="210" ColumnID="PIZNO" DataField="PIZNO" FieldType="String" EnableHeaderMenu="false"
                            HeaderText="注册证号" TextAlign="Left">
                            <Editor>
                                <f:Label ID="lblPIZNO" runat="server" />
                            </Editor>
                        </f:RenderField>
                        <f:RenderField Width="80" DataField="ZDKC" ColumnID="ZDKC" HeaderText="最低库存" TextAlign="Center" EnableHeaderMenu="false" EnableColumnHide="false" hidden="true">
                            <Editor>
                                <f:Label ID="lblZDKC" runat="server" />
                            </Editor>
                        </f:RenderField>
                        <f:RenderField Width="80px" DataField="ZGKC" ColumnID="ZGKC" HeaderText="最高库存" TextAlign="Center" EnableHeaderMenu="false" EnableColumnHide="false" hidden="true">
                            <Editor>
                                <f:Label ID="lblZGKC" runat="server" />
                            </Editor>
                        </f:RenderField>
                        <f:RenderField Width="80px" DataField="DSNUM" ColumnID="DSNUM" HeaderText="定数数量" TextAlign="Center" EnableHeaderMenu="false" EnableColumnHide="false" hidden="true">
                            <Editor>
                                <f:Label ID="lblDSNUM" runat="server" />
                            </Editor>
                        </f:RenderField>
                        <f:RenderField Width="80px" DataField="NUM1" ColumnID="NUM1" HeaderText="定数含量" TextAlign="Center" EnableHeaderMenu="false" EnableColumnHide="false" hidden="true">
                            <Editor>
                                <f:Label ID="lblNUM1" runat="server" />
                            </Editor>
                        </f:RenderField>
                        <f:RenderField Width="80" DataField="NUM2" ColumnID="NUM2" HeaderText="定数流水" TextAlign="Center" EnableHeaderMenu="false" EnableColumnHide="false" Hidden="true">
                            <Editor>
                                <f:Label ID="lblNUM2" runat="server" />
                            </Editor>
                        </f:RenderField>
                        <f:RenderField Width="80px" DataField="NUM3" ColumnID="NUM3" HeaderText="待收定数" TextAlign="Center" EnableHeaderMenu="false" EnableColumnHide="false" hidden="true">
                            <Editor>
                                <f:Label ID="lblNUM3" runat="server" />
                            </Editor>
                        </f:RenderField>
                        <f:RenderField Width="80px" ColumnID="OPERUSER" DataField="OPERUSER" FieldType="String" EnableHeaderMenu="false"
                            HeaderText="操作人" TextAlign="Center">
                            <Editor>
                                <f:Label ID="lblOPERUSER" runat="server" />
                            </Editor>
                        </f:RenderField>
                        <f:RenderField Width="130px" ColumnID="OPERDATE" DataField="OPERDATE" FieldType="String" EnableHeaderMenu="false" 
                            HeaderText="操作时间" TextAlign="Center">
                            <Editor>
                                <f:Label ID="lblOPERDATE" runat="server" />
                            </Editor>
                        </f:RenderField>
                        <f:RenderField Width="0" ColumnID="DEPTID" DataField="DEPTID" FieldType="String" EnableHeaderMenu="false"
                            HeaderText="管理科室" TextAlign="Center" >
                            <Editor>
                                <f:Label ID="lblDEPTID" runat="server" />
                            </Editor>
                        </f:RenderField>
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
        <f:HiddenField ID="hfdValue" runat="server" />
        <f:Window ID="Window1" Title="商品信息" Hidden="true" EnableIFrame="true" runat="server"
            EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True"
            Width="820px" Height="480px" OnClose="Window1_Close">
        </f:Window>
    </form>
</body>
</html>
