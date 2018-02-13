<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Autoship.aspx.cs" Inherits="ERPProject.ERPStorage.Autoship" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" runat="server" AutoSizePanelID="Panel2" />
        <f:Panel ID="Panel2" runat="server" AutoScroll="false" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
            ShowHeader="false">
            <Items>
                <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                    ShowHeader="False" LabelWidth="80px" runat="server">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar2" runat="server">
                            <Items>
                                <f:ToolbarText ID="ToolbarText2" Text="操作信息：自动订货可按【库存上下限参数】/【历史销售数据】/【按定数数量】三种方式生成！" runat="server" />
                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                <f:Button ID="bntClear" Icon="Erase" Text="清 空" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="bntClear_Click" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="btnAutoOrder" Icon="UserTick" Text="生成订单" EnableDefaultState="false" EnablePostBack="true" ValidateForms="Formlist" runat="server" ConfirmText="是否生成订单？" OnClick="btnAutoOrder_Click" Enabled="false" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="btExp" Icon="PageExcel" Text="导 出" EnableDefaultState="false" OnClick="btExp_Click" ConfirmText="是否导出当前订货建议?" DisableControlBeforePostBack="false"
                                    EnablePostBack="true" EnableAjax="false" EnableAjaxLoading="true" AjaxLoadingType="Mask" runat="server" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnableDefaultState="false" ValidateForms="Formlist" EnablePostBack="true" runat="server" OnClick="bntSearch_Click" />
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Rows>
                        <f:FormRow ColumnWidths="25% 30% 20% 20%">
                            <Items>
                                <f:DropDownList ID="docDEPTID" runat="server" Label="订货部门" ForceSelection="true" Required="true" ShowRedStar="true" EnableEdit="true" />
                                <f:RadioButtonList ID="rblTYPE" Label="" AutoPostBack="true" OnSelectedIndexChanged="rblTYPE_SelectedIndexChanged" runat="server">
                                    <f:RadioItem Text="按库存参数生成" Value="KC" />
                                    <f:RadioItem Text="按历史销售生成" Value="XS" />
                                    <f:RadioItem Text="按定数量生成" Value="DS" Selected="true" />
                                </f:RadioButtonList>
                                <f:DatePicker ID="lstLRRQ1" runat="server" AutoPostBack="true" Label="销售日期" Required="true" ShowRedStar="true" />
                                <f:DatePicker ID="lstLRRQ2" runat="server" AutoPostBack="true" Label="　至" LabelSeparator="" Required="true" ShowRedStar="true" CompareOperator="GreaterThanEqual" CompareControl="lstLRRQ1" CompareType="String" />
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
                <f:Grid ID="GridList" AnchorValue="100% -80" ShowBorder="false" ShowHeader="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableHeaderMenu="true"
                    DataKeyNames="" SortField="GDSEQ" EnableColumnLines="true" EnableTextSelection="true">
                    <Columns>
                        <f:RowNumberField Width="30px" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                        <f:BoundField Width="160px" DataField="GDNAME" HeaderText="商品名称" />
                        <f:BoundField Width="150px" DataField="GDSPEC" HeaderText="规格容量" />
                        <f:BoundField Width="80px" DataField="UNITNAME" HeaderText="包装单位" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="HSJJ" HeaderText="含税进价" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="KCSL" HeaderText="当前库存" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="ZTKC" HeaderText="在途库存" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="ZGKC" ColumnID="ZGKC" HeaderText="库存上限" TextAlign="Center" Hidden="true" />
                        <f:BoundField Width="80px" DataField="ZDKC" ColumnID="ZDKC" HeaderText="库存下限" TextAlign="Center" Hidden="true" />
                        <f:BoundField Width="80px" DataField="DAYSL" ColumnID="DAYSL" HeaderText="备货天数" TextAlign="Center" Hidden="true" />
                        <f:BoundField Width="80px" DataField="DHDAY" ColumnID="DHDAY" HeaderText="送货天数" TextAlign="Center" Hidden="true" />
                        <f:BoundField Width="80px" DataField="DAYUSE" ColumnID="DAYUSE" HeaderText="日均用量" TextAlign="Center" />
                        <f:BoundField Width="70px" DataField="DSSL" ColumnID="DSSL" HeaderText="定数量" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="DHSL" HeaderText="建议订货量" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="BAR3" HeaderText="ERP编码" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="HISCODE" HeaderText="HIS编码" TextAlign="Center" />
                        <f:BoundField Width="250px" DataField="HISNAME" HeaderText="HIS名称" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
</body>
</html>
