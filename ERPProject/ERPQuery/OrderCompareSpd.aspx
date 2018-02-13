<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderCompareERP.aspx.cs" Inherits="ERPProject.ERPQuery.OrderCompareERP" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>订单/入库单查询核对</title>
    <script src="../res/js/CreateControl.js" type="text/javascript"></script>
    <script src="../res/js/GRInstall.js" type="text/javascript"></script>
    <style type="text/css" media="all">
        .x-grid-row-summary .x-grid-cell-inner {
            font-weight: bold;
            color: blue;
        }

        .x-grid-row-summary .x-grid-cell {
            background-color: #fff !important;
        }

        .x-grid-row.highlight td {
            background-color: red;
            background-image: none;
        }
 
        .x-grid-row-selected.highlight td {
            background-color: red;
            background-image: none;
        }
        
        .x-grid-row.green td {
            background-color: lightgreen;
            background-image: none;
        }
 
        .x-grid-row-selected.green td {
            background-color: lightgreen;
            background-image: none;
        }

        .x-grid-row.white td {
            background-color: white;
            background-image: none;
        }
 
        .x-grid-row-selected.white td {
            background-color: white;
            background-image: none;
        }

        .x-grid-row.yellow td {
            background-color: yellow;
            background-image: none;
        }
 
        .x-grid-row-selected.yellow td {
            background-color: yellow;
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
        <f:PageManager EnableAjaxLoading="true" ID="PageManager1" AutoSizePanelID="TabStrip1" runat="server" AjaxLoadingType="Mask" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right" EnableTabCloseMenu="false" ActiveTabIndex="0" runat="server">
            <Tabs>
                <f:Tab Title="订单对账" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor" ShowHeader="false">
                            <Items>
                                <f:Form ID="FormCond" ShowBorder="false" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="操作信息：双击打开单据明细！" runat="server" />
                                                <f:ToolbarText ID="ToolbarText5" CssStyle="" Text="绿色表示字段完全匹配，红色表示至少一个字段不匹配，白色表示EAS上没有信息" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill1"  runat="server" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnClear" Icon="Erase" Text="清 除" EnablePostBack="true" runat="server" OnClick="btnClear_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnSearch" Icon="Magnifier" Text="查 询" EnablePostBack="true" runat="server" OnClick="btnSearch_Click" />    
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnCompare" Icon="Magnifier" Text="比 较" EnablePostBack="true" runat="server" OnClick="btnCompare_Click" />                             
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                </f:Form>
                                <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px" ShowHeader="False" LabelWidth="70px" runat="server">
                                    <Rows>
                                        <f:FormRow>
                                            <Items>
                                                <f:DatePicker ID="lstLRRQ1" runat="server" Label="开始日期" Required="true" ShowRedStar="true" />
                                                <f:DatePicker ID="lstLRRQ2" runat="server" Label="结束日期" LabelSeparator="" Required="true" ShowRedStar="true" />
                                                <f:TextBox ID="txtBillNo" runat="server" Label="单据编号" MaxLength="15" />
                                                <f:ToolbarFill ID="ToolbarFill3" runat="server" />     
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                                <f:Panel ID="PanelDetail" runat="server" ShowBorder="false" AnchorValue="100% -30"
                                    BodyPadding="0px" Layout="Anchor" ShowHeader="False">
                                    <Items>
                                        <f:Grid ID="GridList" AnchorValue="100% -260px" ShowBorder="false" ShowHeader="false" AllowSorting="false"
                                            CssStyle="border-bottom: 1px solid #99bce8;"
                                            AutoScroll="true" runat="server" DataKeyNames="BILLNO"  EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridList_RowDoubleClick">
                                            <Columns>
                                                <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" />
                                                <f:BoundField Width="70px" DataField="BILLNO" Hidden="true" />
                                                <f:BoundField Width="70px" DataField="CUSTID" Hidden="true" />
                                                <f:BoundField Width="115px" DataField="MEMO" HeaderText="状态说明" TextAlign="Center" />
                                                <f:BoundField Width="120px" DataField="BILLNO" HeaderText="单据编号" EnableHeaderMenu="false" />
                                                <f:BoundField Width="100px" DataField="XDRQ" HeaderText="单据日期" DataFormatString="{0:yyyy-MM-dd}" EnableHeaderMenu="false" EnableColumnHide="true" TextAlign="Center" />
                                                <%--<f:BoundField Width="100px" DataField="XDRQ_EAS" HeaderText="EAS单据日期" DataFormatString="{0:yyyy-MM-dd}" EnableHeaderMenu="false" EnableColumnHide="true" TextAlign="Center" />--%>
                                                <f:BoundField Width="80px" DataField="SUBNUM" HeaderText="ERP条目数" EnableHeaderMenu="false" EnableColumnHide="true" TextAlign="Center" />
                                                <f:BoundField Width="80px" DataField="SUBNUM_EAS" HeaderText="EAS条目数" EnableHeaderMenu="false" EnableColumnHide="true" TextAlign="Center" />
                                                <f:BoundField Width="80px" DataField="SUBSUM" HeaderText="总价" EnableHeaderMenu="false" EnableColumnHide="true" TextAlign="Center" />
                                                <f:BoundField Width="80px" DataField="SHR" HeaderText="审核人" EnableHeaderMenu="false" EnableColumnHide="true" TextAlign="Center" />
                                                <f:BoundField Width="100px" DataField="SHRQ" HeaderText="审核日期" DataFormatString="{0:yyyy-MM-dd}" EnableHeaderMenu="false" EnableColumnHide="true" TextAlign="Center" />
                                            </Columns>
                                        </f:Grid>
                                        <f:Panel ID="Panel2" runat="server" ShowBorder="false" Height="260px"
                                            BodyPadding="0px" Layout="Anchor" ShowHeader="False">
                                            <Toolbars>
                                                <f:Toolbar ID="Toolbar1" runat="server">
                                                    <Items>
                                                        <f:ToolbarText ID="ToolbarText1" Text="单据" runat="server" />
                                                        <f:ToolbarText ID="ToolbarBillno" Text="" runat="server" />
                                                        <f:ToolbarText ID="ToolbarText7" Text="明细：" runat="server" />
                                                        <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                    </Items>
                                                </f:Toolbar>
                                            </Toolbars>
                                            <Items>
                                                  <f:Grid ID="GridDetail" AnchorValue="100% -60px" ShowBorder="false" ShowHeader="false" AllowSorting="false"
                                                    CssStyle="border-bottom: 1px solid #99bce8;" AutoScroll="true" runat="server" DataKeyNames="GDSEQ"  >
                                                    <Columns>
                                                        <f:RowNumberField Width="30px" TextAlign="Center" />
                                                        <f:BoundField Width="115px" DataField="MEMO" HeaderText="状态说明" TextAlign="Center" />
                                                        <f:BoundField Width="115px" DataField="GDSEQ" HeaderText="ERP上传编码" TextAlign="Center" />
                                                        <f:BoundField Width="115px" DataField="GDSEQ_EAS" HeaderText="EAS下传编码" TextAlign="Center" />
                                                        <f:BoundField Width="150px" DataField="GDNAME" HeaderText="商品名称" TextAlign="Center" />
                                                        <f:BoundField Width="100px" DataField="UNIT" HeaderText="包装单位" TextAlign="Center" />
                                                        <f:BoundField Width="90px" DataField="GDSPEC" HeaderText="商品规格" TextAlign="Center" />
                                                        <f:BoundField Width="90px" DataField="DHS" HeaderText="订单数量" TextAlign="Center" />
                                                        <f:BoundField Width="90px" DataField="DHS_EAS" HeaderText="EAS订单数量" TextAlign="Center" />
                                                    </Columns>
                                                </f:Grid>
                                            </Items>
                                        </f:Panel>
                                    </Items>
                                </f:Panel>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>

                <f:Tab Title="入库单对账" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel3" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor" ShowHeader="false">
                            <Items>
                                <f:Form ID="Form2" ShowBorder="false" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText3" CssStyle="" Text="操作信息：双击打开单据明细！" runat="server" />
                                                <f:ToolbarText ID="ToolbarText8" CssStyle="" Text="绿色表示字段完全匹配，红色表示至少一个字段不匹配，黄色表示ERP没有信息" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill4" runat="server" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnClear2" Icon="Erase" Text="清 除" EnablePostBack="true" runat="server" OnClick="btnClear2_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnSearch2" Icon="Magnifier" Text="查 询" EnablePostBack="true" runat="server" OnClick="btnSearch2_Click" />    
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnCompare2" Icon="Magnifier" Text="比 较" EnablePostBack="true" runat="server" OnClick="btnCompare2_Click" />                              
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                </f:Form>
                                <f:Form ID="Form3" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px" ShowHeader="False" LabelWidth="70px" runat="server">
                                    <Rows>
                                        <f:FormRow>
                                            <Items>
                                                <f:DatePicker ID="lstLRRQ11" runat="server" Label="开始日期" Required="true" ShowRedStar="true" />
                                                <f:DatePicker ID="lstLRRQ22" runat="server" Label="结束日期" LabelSeparator="" Required="true" ShowRedStar="true" />
                                                <f:TextBox ID="txtBillNo2" runat="server" Label="单据编号" MaxLength="15" />
                                                <f:ToolbarFill ID="ToolbarFill5" runat="server" />     
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                                <f:Panel ID="Panel4" runat="server" ShowBorder="false" AnchorValue="100% -30"
                                    BodyPadding="0px" Layout="Anchor" ShowHeader="False">
                                    <Items>
                                        <f:Grid ID="GridList2" AnchorValue="100% -260px" ShowBorder="false" ShowHeader="false" AllowSorting="false"
                                            CssStyle="border-bottom: 1px solid #99bce8;"
                                            AutoScroll="true" runat="server" DataKeyNames="SEQNO"  EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridList2_RowDoubleClick">
                                            <Columns>
                                                <f:RowNumberField Width="30px" TextAlign="Center" />
                                                <f:BoundField Width="70px" DataField="SEQNO" Hidden="true" />
                                                <f:BoundField Width="115px" DataField="MEMO" HeaderText="状态说明" TextAlign="Center" />
                                                <f:BoundField Width="120px" DataField="SEQNO" HeaderText="单据编号" EnableHeaderMenu="false" />
                                                <f:BoundField Width="100px" DataField="DHRQ" HeaderText="ERP单据日期" DataFormatString="{0:yyyy-MM-dd}" EnableHeaderMenu="false" EnableColumnHide="true" TextAlign="Center" />
                                                <f:BoundField Width="100px" DataField="DHRQ_EAS" HeaderText="EAS单据日期" DataFormatString="{0:yyyy-MM-dd}" EnableHeaderMenu="false" EnableColumnHide="true" TextAlign="Center" />
                                                <f:BoundField Width="80px" DataField="SUBNUM" HeaderText="ERP条目数" EnableHeaderMenu="false" EnableColumnHide="true" TextAlign="Center" />
                                                <f:BoundField Width="80px" DataField="SUBNUM_EAS" HeaderText="EAS条目数" EnableHeaderMenu="false" EnableColumnHide="true" TextAlign="Center" />
                                            </Columns>
                                        </f:Grid>
                                        <f:Panel ID="Panel5" runat="server" ShowBorder="false" Height="260px"
                                            BodyPadding="0px" Layout="Anchor" ShowHeader="False">
                                            <Toolbars>
                                                <f:Toolbar ID="Toolbar2" runat="server">
                                                    <Items>
                                                        <f:ToolbarText ID="ToolbarText4" Text="单据" runat="server" />
                                                        <f:ToolbarText ID="ToolbarSeqno" Text="" runat="server" />
                                                        <f:ToolbarText ID="ToolbarText6" Text="明细：" runat="server" />
                                                        <f:ToolbarFill ID="ToolbarFill6" runat="server" />
                                                    </Items>
                                                </f:Toolbar>
                                            </Toolbars>
                                            <Items>
                                                  <f:Grid ID="GridDetail2" AnchorValue="100% -60px" ShowBorder="false" ShowHeader="false" AllowSorting="false"
                                                    CssStyle="border-bottom: 1px solid #99bce8;" AutoScroll="true" runat="server" DataKeyNames="Seqno,GDSEQ"  >
                                                    <Columns>
                                                        <f:BoundField Width="70px" DataField="SEQNO" Hidden="true" />
                                                        <f:RowNumberField Width="30px" TextAlign="Center" />
                                                        <f:BoundField Width="115px" DataField="MEMO" HeaderText="状态说明" TextAlign="Center" />
                                                        <f:BoundField Width="115px" DataField="GDSEQ" HeaderText="ERP上传编码" TextAlign="Center" />
                                                        <f:BoundField Width="115px" DataField="GDSEQ_EAS" HeaderText="EAS下传编码" TextAlign="Center" />
                                                        <f:BoundField Width="100px" DataField="GDNAME" HeaderText="商品名称" TextAlign="Center" />
                                                        <f:BoundField Width="100px" DataField="UNIT" HeaderText="包装单位" TextAlign="Center" />
                                                        <f:BoundField Width="90px" DataField="GDSPEC" HeaderText="商品规格" TextAlign="Center" />
                                                        <f:BoundField Width="90px" DataField="SSSL" HeaderText="ERP订单数量" TextAlign="Center" />
                                                        <f:BoundField Width="90px" DataField="SSSL_EAS" HeaderText="EAS订单数量" TextAlign="Center" />
                                                        <f:BoundField Width="90px" DataField="PH" HeaderText="ERP批号" TextAlign="Center" />
                                                        <f:BoundField Width="90px" DataField="PH_EAS" HeaderText="EAS批号" TextAlign="Center" />
                                                    </Columns>
                                                </f:Grid>
                                            </Items>
                                        </f:Panel>
                                    </Items>
                                </f:Panel>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
    </form>
        <script src="../res/js/jquery-1.11.1.min.js"></script>
    <script type="text/javascript">

        function onGridDataLoad(highlightRows,greenRows,gridClientID)
        {
            var grid = F(gridClientID);
            $(grid.el.dom).find('.x-grid-row.highlight').removeClass('highlight');
            $(grid.el.dom).find('.x-grid-row.green').removeClass('green');
            $.each(highlightRows.split(','), function (index, item) {
                if (item !== '') {
                    var row = grid.getView().getNode(parseInt(item, 10));
                    $(row).addClass('highlight');
                }
            });
            $.each(greenRows.split(','), function (index, item) {
                if (item !== '') {
                    var row = grid.getView().getNode(parseInt(item, 10));
                    $(row).addClass('green');
                }
            });
        }

        function onGridDataLoad2(highlightRows, greenRows, yellowRows, gridClientID) {
            var grid = F(gridClientID);
            $(grid.el.dom).find('.x-grid-row.highlight').removeClass('highlight');
            $(grid.el.dom).find('.x-grid-row.green').removeClass('green');
            $(grid.el.dom).find('.x-grid-row.yellow').removeClass('yellow');
            $.each(highlightRows.split(','), function (index, item) {
                if (item !== '') {
                    var row = grid.getView().getNode(parseInt(item, 10));
                    $(row).addClass('highlight');
                }
            });
            $.each(greenRows.split(','), function (index, item) {
                if (item !== '') {
                    var row = grid.getView().getNode(parseInt(item, 10));
                    $(row).addClass('green');
                }
            });
            $.each(yellowRows.split(','), function (index, item) {
                if (item !== '') {
                    var row = grid.getView().getNode(parseInt(item, 10));
                    $(row).addClass('yellow');
                }
            });
        }

    </script>
</body>
</html>
