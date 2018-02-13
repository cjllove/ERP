<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GZJXC.aspx.cs" Inherits="ERPProject.ERPQuery.GZJXC" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>高值条码管理</title>
    <script src="../res/js/CreateControl.js" type="text/javascript"></script>
    <style type="text/css">
        .ui-state-disabled {
            opacity: .5;
            filter: alpha(opacity=50);
            background-image: none;
        }
    </style>
</head>
<body>
    <div style="width: 0px; height: 0px">
        <script type="text/javascript">
            CreateDisplayViewerEx("0%", "0%", "", "", false, "");
        </script>
    </div>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="Panel1"
            runat="server" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
            ShowHeader="false">
            <Toolbars>
                <f:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                        <f:ToolbarText runat="server" Text="操作信息：查询高值商品的进销存信息"></f:ToolbarText>
                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                        <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" EnableDefaultState="false" DisableControlBeforePostBack="true" runat="server" ValidateForms="FormUser" OnClick="btSearch_Click" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="5px 10px 0px 10px"
                    ShowHeader="False" LabelWidth="80px" runat="server">
                    <Rows>
                        <f:FormRow ColumnWidths="25% 25% 50%">
                            <Items>
                                <f:DropDownList ID="lstDEPTID" runat="server" Label="使用科室" EnableEdit="true" ForceSelection="true" />
                                <f:TriggerBox ID="tbxGDSEQ" runat="server" Label="商品" ShowTrigger="false" MaxLength="20" EmptyText="请输入商品编码或商品名称" TriggerIcon="Search" OnTriggerClick="btSearch_Click"></f:TriggerBox>
                                <f:TriggerBox ID="tbxONECODE" runat="server" Label="追溯码" ShowTrigger="false" EmptyText="商品追溯码" MaxLength="36" TriggerIcon="Search" OnTriggerClick="btSearch_Click"></f:TriggerBox>
                            </Items>
                        </f:FormRow>
                        <f:FormRow ColumnWidths="25% 25% 50%">
                            <Items>
                                <f:DatePicker ID="dpkout1" runat="server" Label="操作日期" Required="true" ShowRedStar="true"></f:DatePicker>
                                <f:DatePicker ID="dpkout2" runat="server" Label="&nbsp;&nbsp;至" Required="true" CompareControl="dpkout1" CompareOperator="GreaterThanEqual" CompareMessage="结束日期应大于开始日期!" ShowRedStar="true"></f:DatePicker>
                                <f:Label runat="server" ></f:Label>
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
                <f:Grid ID="GridGoods" AnchorValue="100% -66" ShowBorder="false" ShowHeader="false"
                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableColumnLines="true"
                    PageSize="50" DataKeyNames="ONECODE" IsDatabasePaging="true" AllowPaging="true" CheckBoxSelectOnly="true" OnPageIndexChange="GridGoods_PageIndexChange" EnableTextSelection="true">
                    <Columns>
                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                        <f:BoundField Width="140px" DataField="CREATETIME" HeaderText="操作时间" TextAlign="Center" />
                        <f:BoundField Width="260px" DataField="ONECODE" HeaderText="唯一码" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                        <f:BoundField Width="150px" DataField="GDNAME" HeaderText="商品名称" />
                        <f:BoundField Width="100px" DataField="GDSPEC" HeaderText="商品规格" TextAlign="Center" />
                        <f:BoundField Width="50px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                        <f:BoundField Width="70px" DataField="OPERTYEPNAME" HeaderText="操作" TextAlign="Center" />
                        <f:BoundField Width="60px" DataField="SL" HeaderText="数量" TextAlign="Center" />
                        <f:BoundField Width="60px" DataField="HSJJ_GOD" HeaderText="价格" TextAlign="Right" />
                        <f:BoundField Width="150px" DataField="PRODUCERNAME" HeaderText="生产厂家" />
                        <f:BoundField Width="100px" DataField="DEPTNAME" HeaderText="使用科室" TextAlign="Center" />
                        <f:BoundField Width="120px" DataField="BILLNO" HeaderText="单号" TextAlign="Center" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
</body>
</html>
