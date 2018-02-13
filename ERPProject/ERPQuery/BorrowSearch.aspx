<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BorrowSearch.aspx.cs" Inherits="ERPProject.ERPQuery.BorrowSearch" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>赠品信息查询</title>
    <script src="../res/js/CreateControl.js" type="text/javascript"></script>
    <style type="text/css" media="all">
        .ColBlue {
            font-size: 12px;
            color: blue;
        }

        .x-grid-row-summary .x-grid-cell-inner {
            font-weight: bold;
            color: blue;
        }

        .x-grid-row-summary .x-grid-cell {
            background-color: #fff !important;
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
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="TabStrip1" runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right" EnableTabCloseMenu="false" ActiveTabIndex="0" runat="server">
            <Tabs>
                <f:Tab Title="赠品进销存明细" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel1" runat="server" ShowBorder="False" BodyPadding="0px" Layout="Anchor" ShowHeader="false">
                            <Items>
                                <f:Panel ID="PanelBody" ShowBorder="false" BodyPadding="0px" Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar1" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText" runat="server" Text="备注：查询赠品进销存信息！"></f:ToolbarText>
                                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                <f:Button ID="btnAudit" Icon="ArrowRotateClockwise" Text="清 空" OnClick="btClear_Click" EnablePostBack="true" runat="server" EnableDefaultState="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnExp" runat="server" CssStyle="margin-left: 10px;" Icon="DatabaseGo" EnableAjax="false" EnableDefaultState="false"
                                                    OnClick="btExport_Click" EnablePostBack="true" Text="导 出" ConfirmText="是否确定导出科室分类结算信息？" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="BtnSearch" Icon="Magnifier" Text="查 询" ValidateForms="FormUser" OnClick="BtnSearch_Click" runat="server" EnableDefaultState="false" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="10px 0px 10px 10px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="ddlDEPTOUT" runat="server" Label="库房" EnableEdit="true" ForceSelection="true"></f:DropDownList>
                                                        <f:DropDownList ID="ddlTYPE" runat="server" Label="单据类型">
                                                            <f:ListItem Text="--请选择--" Value="" Selected="true" />
                                                            <f:ListItem Text="入库单" Value="RK" />
                                                            <f:ListItem Text="出库单" Value="CK" />
                                                            <f:ListItem Text="销售单" Value="XS" />
                                                            <f:ListItem Text="退货单" Value="TH" />
                                                            <f:ListItem Text="销退单" Value="XT" />
                                                        </f:DropDownList>
                                                        <f:TriggerBox ID="tgbSch" runat="server" Label="商品信息" EmptyText="可输入商品名称、编码或助记码" ShowTrigger="false" OnTriggerClick="BtnSearch_Click"></f:TriggerBox>
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="ddlDEPTID" runat="server" Label="科室" EnableEdit="true" ForceSelection="true"></f:DropDownList>
                                                        <f:DatePicker ID="dpkDATE1" runat="server" Label="日期" Required="true" ShowRedStar="true"></f:DatePicker>
                                                        <f:DatePicker ID="dpkDATE2" runat="server" Label="　至" Required="true" ShowRedStar="true" CompareOperator="GreaterThanEqual" CompareType="String" CompareControl="dpkDATE1"></f:DatePicker>
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridGoods" AnchorValue="100% -100px" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;" EnableColumnLines="true" EnableTextSelection="true"
                                    AllowSorting="false" AutoScroll="true" runat="server">
                                    <Columns>
                                        <f:RowNumberField Width="30px"></f:RowNumberField>
                                        <f:BoundField Width="60px" DataField="TYPENAME" HeaderText="操作类型" TextAlign="Center" />
                                        <f:BoundField Width="110px" DataField="SEQNO" HeaderText="单号" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="DEPTOUTNAME" HeaderText="库房" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="DEPTIDNAME" HeaderText="科室名称" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="LRYNAME" HeaderText="录入员" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="LRRQ" HeaderText="录入日期" DataFormatString="{0:yyyy-MM-dd}" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="SHRNAME" HeaderText="审核员" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="LRRQ" HeaderText="审核日期" DataFormatString="{0:yyyy-MM-dd}" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                                        <f:BoundField Width="160px" DataField="GDNAME" HeaderText="商品名称" />
                                        <f:BoundField Width="120px" DataField="GDSPEC" HeaderText="规格" />
                                        <f:BoundField Width="50px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="SSSL" HeaderText="数量" TextAlign="Center" />
                                        <f:BoundField Width="60px" DataField="HSJJ" HeaderText="单价" TextAlign="Right" DataFormatString="{0:F2}" />
                                        <f:BoundField Width="180px" DataField="PRODUCERNAME" HeaderText="生产厂家" />
                                        <f:BoundField Width="220px" DataField="PIZNO" HeaderText="注册证号" />
                                        <f:BoundField Width="110px" DataField="BAR3" HeaderText="ERP编码" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="HISCODE" HeaderText="HIS编码" TextAlign="Center" />
                                        <f:BoundField Width="160px" DataField="HISNAME" HeaderText="HIS名称" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="赠品库存查询" Icon="PageWord" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel6" runat="server" ShowBorder="False" BodyPadding="0px" Layout="Anchor" ShowHeader="false">
                            <Items>
                                <f:Panel ID="Panel7" ShowBorder="false" BodyPadding="0px" Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar4" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText1" runat="server" Text="备注：查询赠品库存信息！"></f:ToolbarText>
                                                <f:ToolbarFill ID="ToolbarFill4" runat="server" />
                                                <f:Button ID="Button1" Icon="ArrowRotateClockwise" Text="清 空" OnClick="btClear_Click" EnablePostBack="true" runat="server" EnableDefaultState="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="Button3" runat="server" CssStyle="margin-left: 10px;" Icon="DatabaseGo" EnableAjax="false" OnClick="btExport_Click" EnablePostBack="true" Text="导出" EnableDefaultState="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnlis" Icon="Magnifier" Text="查 询" runat="server" OnClick="btnlis_Click" EnableDefaultState="false" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="Form4" ShowBorder="false" AutoScroll="false" BodyPadding="10px 0px 10px 10px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TriggerBox ID="tbxGDSEQ" runat="server" Label="商品信息" EmptyText="可输入商品名称、编码或助记码" ShowTrigger="false" OnTriggerClick="btnlis_Click"></f:TriggerBox>
                                                        <f:DropDownList ID="ddlDEPT" runat="server" Label="科室" EnableEdit="true" ForceSelection="true"></f:DropDownList>
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="Grdlist" AnchorValue="100% -73px" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;" EnableTextSelection="true"
                                    AllowSorting="false" AutoScroll="true" runat="server" DataKeyNames="GDSEQ" EnableColumnLines="true">
                                    <Columns>
                                        <f:RowNumberField Width="30px"></f:RowNumberField>
                                        <f:BoundField Width="100px" DataField="DEPTOUTNAME" HeaderText="部门/科室" TextAlign="Center" />
                                        <f:BoundField Width="110px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                                        <f:BoundField Width="180px" DataField="GDNAME" HeaderText="商品名称" />
                                        <f:BoundField Width="120px" DataField="GDSPEC" HeaderText="规格" />
                                        <f:BoundField Width="50px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="SL" HeaderText="可出赠品量" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="HSJJ" HeaderText="单价" TextAlign="Right" DataFormatString="{0:F2}" />
                                        <f:BoundField Width="180px" DataField="PRODUCERNAME" HeaderText="生产厂家" />
                                        <f:BoundField Width="220px" DataField="PIZNO" HeaderText="注册证号" />
                                        <f:BoundField Width="110px" DataField="BAR3" HeaderText="ERP编码" TextAlign="Center" />
                                        <f:BoundField Width="120px" DataField="HISCODE" HeaderText="HIS编码" TextAlign="Center" />
                                        <f:BoundField Width="160px" DataField="HISNAME" HeaderText="HIS名称" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
    </form>
</body>
</html>
