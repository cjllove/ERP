﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoodsStockout.aspx.cs" Inherits="ERPProject.ERPStorage.GoodsStockout" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>商品缺货管理</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="TabStrip1"
            runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="1" runat="server">
            <Tabs>
                <f:Tab Title="缺货处理查询" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel2" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="Panel3" ShowBorder="false" BodyPadding="0px"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar2" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="操作信息：" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                <f:Button ID="bntClear" Icon="Erase" Text="清 除" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="bntClear_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="bntSearch_Click" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="75px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TriggerBox ID="lstBILLNO" runat="server" Label="单据编号" MaxLength="20" EmptyText="输入缺货单号" ShowTrigger="false" OnTriggerClick="bntSearch_Click" />
                                                        <f:DropDownList ID="lstFLAG" runat="server" Label="单据状态">
                                                            <f:ListItem Text="--请选择--" Value="" />
                                                            <f:ListItem Text="已订货" Value="Y" />
                                                            <f:ListItem Text="已完成" Value="G" />
                                                            <f:ListItem Text="已取消" Value="C" />
                                                        </f:DropDownList>
                                                        <f:DropDownList ID="lstLRY" runat="server" Label="申领员" />
                                                        <f:DropDownList ID="lstDEPTID" runat="server" Label="收货地点" EnableEdit="true" ForceSelection="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TriggerBox ID="tgbGoods" runat="server" Label="商品信息" MaxLength="20" ShowTrigger="false" EmptyText="输入商品信息查询" OnTriggerClick="bntSearch_Click"></f:TriggerBox>
                                                        <f:DropDownList ID="lstPSSID" runat="server" Label="送货商" EnableEdit="true" ForceSelection="true">
                                                        </f:DropDownList>
                                                        <f:DatePicker ID="lstLRRQ1" runat="server" Label="查询日期" Required="true" ShowRedStar="true" />
                                                        <f:DatePicker ID="lstLRRQ2" runat="server" Label="　　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" AnchorValue="100% -110" ShowBorder="false" ShowHeader="false" PageSize="300" EnableColumnLines="true"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableTextSelection="true"
                                    DataKeyNames="GDSEQ" AllowPaging="true" IsDatabasePaging="true" OnPageIndexChange="GridList_PageIndexChange">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField Width="60px" DataField="FLAG_CN" HeaderText="当前状态" TextAlign="Center" />
                                        <f:BoundField Width="120px" DataField="BILLNO_SL" HeaderText="申领单号" TextAlign="Center" />
                                        <f:BoundField Hidden="true" DataField="ROWNO" HeaderText="行号" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="DATE_SL" HeaderText="申领日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Hidden="true" DataField="DEPTID" HeaderText="申领部门编码" TextAlign="Left" />
                                        <f:BoundField Width="120px" DataField="DEPTNAME" HeaderText="申领部门" TextAlign="Left" />
                                        <f:BoundField Width="100px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                                        <f:BoundField Width="150px" DataField="HISNAME" HeaderText="商品名称" TextAlign="Left" />
                                        <f:BoundField Width="120px" DataField="GDSPEC" HeaderText="商品规格" TextAlign="Left" /> 
                                        <f:BoundField Hidden="true" DataField="PSSID" HeaderText="配送商编码" TextAlign="Left" />
                                        <f:BoundField Width="150px" DataField="PSSNAME" HeaderText="配送商名称" TextAlign="Left" />
                                        <f:BoundField Width="120px" DataField="UNITNAME" HeaderText="最小包装单位" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="BZSL" HeaderText="申领数" TextAlign="Right" Hidden="true" />
                                        <f:BoundField Width="150px" DataField="SLSL" HeaderText="申领数量（最小单位）" TextAlign="Right" />
                                        <f:BoundField Width="150px" DataField="QHSL" HeaderText="缺货数量（最小单位）" TextAlign="Right" />
                                        <f:BoundField Width="80" DataField="BZUNITNAME" HeaderText="包装单位" TextAlign="Center" />
                                        <f:BoundField Width="150px" DataField="BZSLSL" HeaderText="申领数量（包装单位）" TextAlign="Right" />
                                        <f:BoundField Width="150px" DataField="BZQHSL" HeaderText="缺货数量（包装单位）" TextAlign="Right" />
                                        <f:BoundField Width="110px" DataField="BILLNO_DD" HeaderText="生成的订单号" TextAlign="Center" />
                                        <f:BoundField Width="110px" DataField="BILLNO_RK" HeaderText="匹配的入库单号" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="110px" DataField="BILLNO_CK" HeaderText="匹配的出库单号" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="70px" DataField="OPERUSER_CN" HeaderText="操作员" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="SLY" HeaderText="申领员" TextAlign="Center"></f:BoundField>

                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="待处理缺货" Icon="PageWord" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Region"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="Panel8" BodyPadding="0px" RegionSplit="false" EnableCollapse="false" RegionPosition="Top" ShowBorder="false"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar1" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="操作信息：查询科室缺货信息!" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="btnSup" Icon="ApplicationEdit" runat="server" EnableDefaultState="false" Text="修改配送商" OnClick="btnSup_Click" Hidden="true"></f:Button>
                                                <f:Button ID="btnAudit" Icon="UserTick" Text="订单生成" EnableDefaultState="false" ConfirmText="是否确认生成订单?" EnablePostBack="true" runat="server" OnClick="btnAudit_Click" />
                                                <f:Button ID="btnDb" Icon="UserTick" Text="调拨生成" EnableDefaultState="false" ConfirmText="是否确认生成自动调拨单?" EnablePostBack="true" runat="server" OnClick="btnDb_Click" />
                                                <f:Button ID="btnCancel" Icon="UserCross" Text="单据废弃" EnableDefaultState="false" ConfirmText="您确认将以下所选单据作废吗？" EnablePostBack="true" runat="server" OnClick="btnCancel_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" EnableDefaultState="false" OnClick="btExport_Click" ConfirmText="是否导出当前商品缺货信息？" DisableControlBeforePostBack="false"
                                                    EnablePostBack="true" EnableAjax="false" EnableAjaxLoading="true" AjaxLoadingType="Mask" runat="server" />
                                                <f:Button ID="btnGoodsSearch" Icon="Magnifier" Text="查 询" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnGoodsSearch_Click" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                </f:Panel>
                                <f:Panel ID="Panel4" BodyPadding="0px" RegionSplit="true" EnableCollapse="true" RegionPosition="Top" ShowBorder="false"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Items>
                                        <f:Form ID="FormDoc" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow ColumnWidths="25% 25% 50%">
                                                    <Items>
                                                        <f:DropDownList ID="ddlDEPTOUT" runat="server" Label="缺货库房" ShowRedStar="true" AutoPostBack="true" OnSelectedIndexChanged="ddlDEPTOUT_SelectedIndexChanged" Required="true" EnableEdit="true" ForceSelection="true">
                                                        </f:DropDownList>
                                                        <f:DropDownList ID="docSUPID" runat="server" Label="送货商" EnableEdit="true" ForceSelection="true">
                                                        </f:DropDownList>
                                                        <f:TriggerBox ID="docSearch" runat="server" Label="商品信息" EmptyText="可输入商品编码或名称" MaxLength="20" ShowTrigger="false" OnTriggerClick="btnGoodsSearch_Click" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="docDEPTID" runat="server" Label="申领科室" EnableEdit="true" ForceSelection="true">
                                                        </f:DropDownList>
                                                        <f:DropDownList ID="docCatid" runat="server" Label="商品类型">
                                                            <f:ListItem Value="" Text="--请选择--" Selected="true" />
                                                        </f:DropDownList>
                                                        <f:DropDownList ID="ddlISFLAG7" Label="本地商品" runat="server">
                                                            <f:ListItem Value="" Text="--请选择--" Selected="true" />
                                                            <f:ListItem Value="Y" Text="是" Selected="true" />
                                                            <f:ListItem Value="N" Text="否" Selected="true" />
                                                        </f:DropDownList>
                                                        <f:TriggerBox ID="docBILLNO" runat="server" Label="单据编号" MaxLength="20" EmptyText="申领单号" OnTriggerClick="btnGoodsSearch_Click" ShowTrigger="false" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="docISFLAG3" runat="server" Label="直送商品">
                                                            <f:ListItem Value="" Text="--请选择--" Selected="true" />
                                                            <f:ListItem Value="Y" Text="直送商品" />
                                                            <f:ListItem Value="N" Text="非直送" />
                                                        </f:DropDownList>
                                                        <f:DropDownList ID="docISGZ" runat="server" Label="高值商品">
                                                            <f:ListItem Value="" Text="--请选择--" Selected="true" />
                                                            <f:ListItem Value="Y" Text="高值商品" />
                                                            <f:ListItem Value="N" Text="非高值" />
                                                        </f:DropDownList>
                                                        <f:DatePicker ID="docDATE_SL1" runat="server" Label="申领日期" ShowRedStar="true" />
                                                        <f:DatePicker ID="docDATE_SL2" runat="server" Label="　至" LabelSeparator="" ShowRedStar="true" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridCom" AnchorValue="100% -138" ShowBorder="false" AllowCellEditing="false" ShowHeader="false" EnableColumnLines="true"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" PageSize="500" KeepCurrentSelection="true" EnableMultiSelect="true" EnableTextSelection="true"
                                    DataKeyNames="GDSEQ,BILLNO_SL,ROWNO,SUPID" EnableCheckBoxSelect="true" AllowPaging="true" IsDatabasePaging="true" OnPageIndexChange="GridCom_PageIndexChange"
                                    EnableRowClickEvent="true" OnRowClick="GridCom_RowClick"
                                    AllowSorting="true" EnableHeaderMenu="true" SortField="BILLNO_SL" OnSort="GridCom_Sort">
                                    <Columns>
                                        <f:RowNumberField Width="40px" TextAlign="Center" EnablePagingNumber="true" />
                                        <f:BoundField Width="0px" DataField="FLAG_CN" SortField="FLAG_CN" HeaderText="当前状态" Hidden="true" TextAlign="Center" />
                                        <f:BoundField Width="120px" DataField="BILLNO_SL" SortField="BILLNO_SL" HeaderText="申领单号" TextAlign="Center" />
                                        <f:BoundField Width="35px" DataField="ROWNO" SortField="ROWNO" HeaderText="行号" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="90px" DataField="DATE_SL" SortField="DATE_SL" HeaderText="申领日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Hidden="true" DataField="DEPTID" SortField="DEPTID" HeaderText="申领部门编码" TextAlign="Left" />
                                        <f:BoundField Width="100px" DataField="DEPTOUTNAME" SortField="DEPTOUT" HeaderText="缺货库房" TextAlign="Left" />
                                        <f:BoundField Width="150px" DataField="DEPTNAME" SortField="DEPTID" HeaderText="申领部门" TextAlign="Left" />
                                        <f:BoundField Width="100px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                                        <f:BoundField DataField="HISNAME" SortField="HISNAME" HeaderText="商品名称" TextAlign="Left" MinWidth="150px" ExpandUnusedSpace="true" />
                                        <f:BoundField Width="80px" DataField="CATID0NAME" SortField="CATID0NAME" HeaderText="商品类别" TextAlign="Center" />
                                        <f:BoundField Width="120px" DataField="GDSPEC" SortField="GDSPEC" HeaderText="商品规格" TextAlign="Left" />
                                        <f:BoundField Width="100px" DataField="UNITNAME" SortField="UNITNAME" HeaderText="最小包装单位" TextAlign="Center" />
                                        <f:BoundField Width="150px" DataField="SLSL" SortField="SLSL" HeaderText="申领数量（最小单位）" TextAlign="Right" />
                                        <f:BoundField Width="150px" DataField="QHSL" SortField="QHSL" HeaderText="缺货数（最小单位）" TextAlign="Right" />
                                        <f:BoundField Width="80px" DataField="BZUNITNAME" SortField="BZUNITNAME" HeaderText="包装单位" TextAlign="Center" />
                                        <f:BoundField Width="150px" DataField="BZSLSL" SortField="BZSLSL" HeaderText="申领数量（包装单位）" TextAlign="Right" />
                                        <f:BoundField Width="150px" DataField="BZQHSL" SortField="BZQHSL" HeaderText="缺货数（包装单位）" TextAlign="Right" />
                                        <f:BoundField Hidden="true" DataField="PSSID" SortField="PSSID" HeaderText="配送商编码" TextAlign="Left" />
                                        <f:BoundField Width="230px" DataField="PSSNAME" SortField="SUPNAME" HeaderText="送货商名称" TextAlign="Left" />
                                        <f:BoundField Width="50px" DataField="BZSL" SortField="BZSL" HeaderText="申领数" TextAlign="Right" Hidden="true" />
                                        <f:CheckBoxField Width="80px" RenderAsStaticField="true" DataField="ISGZ" SortField="ISGZ" HeaderText="高值商品" TextAlign="Center"></f:CheckBoxField>
                                        <f:CheckBoxField Width="80px" RenderAsStaticField="true" DataField="ISFLAG3" SortField="ISFLAG3" HeaderText="直送商品" TextAlign="Center"></f:CheckBoxField>
                                        <f:CheckBoxField Width="80px" RenderAsStaticField="true" DataField="ISFLAG7" SortField="ISFLAG7" HeaderText="本地商品" TextAlign="Center"></f:CheckBoxField>
                                        <f:BoundField Width="150px" DataField="QHMEMO" HeaderText="备注"></f:BoundField>
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
        <f:HiddenField ID="hfdDeptCode" runat="server" />
        <f:HiddenField ID="hdfIndex" runat="server"></f:HiddenField>
    </form>
    <f:HiddenField ID="hfdValue" runat="server" />
    <f:Window ID="WindowPH" Title="配送商信息" EnableIFrame="false" runat="server" Hidden="true"
        EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="300px" Height="100px">
        <Items>
            <f:Form ID="Form3" ShowBorder="false" AutoScroll="false" BodyPadding="5px 10px"
                ShowHeader="False" LabelWidth="70px" runat="server">
                <Rows>
                    <f:FormRow ColumnWidths="0 100%" Hidden="true">
                        <Items>
                            <f:HiddenField ID="tbxGDSEQ1" runat="server"></f:HiddenField>
                            <f:TextBox ID="tbxPH1" runat="server" EmptyText="代管商品手动输入批号" Label="批号" Required="true" ShowRedStar="True" Hidden="true"></f:TextBox>
                        </Items>
                    </f:FormRow>
                    <f:FormRow Hidden="true">
                        <Items>
                            <f:DropDownList ID="SUPNAME" runat="server" Label="配送商" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true">
                            </f:DropDownList>
                        </Items>
                    </f:FormRow>
                </Rows>
            </f:Form>
        </Items>
        <Toolbars>
            <f:Toolbar ID="Toolbar6" runat="server" Position="Bottom" ToolbarAlign="Right">
                <Items>
                    <f:Button ID="btnPHClose" Text="确定" Icon="SystemSave" OnClick="bntSave" runat="server"></f:Button>
                </Items>
            </f:Toolbar>
        </Toolbars>
    </f:Window>
</body>
</html>