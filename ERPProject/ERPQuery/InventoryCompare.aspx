<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InventoryCompare.aspx.cs" Inherits="ERPProject.ERPReport.InventoryCompare" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
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
    <form id="form2" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" AjaxLoadingType="Mask" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
            ShowHeader="false">
            <Items>
                <f:Panel ID="PanelBody" ShowBorder="false" BodyPadding="0px"
                    Layout="Anchor" ShowHeader="False" runat="server">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar1" runat="server">
                            <Items>
                                <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="先点击EAS库存下载，将EAS库存下载到本地。[比较]本地库存与EAS库存比较,红色不同，绿色相同，白色仅本地，黄色仅EAS" runat="server" />
                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                <f:Button ID="btnAudit" Icon="ArrowRotateClockwise" Text="清 空" OnClick="btClear_Click" EnablePostBack="true" runat="server" EnableDefaultState="false" />
                                <f:Button ID="btnPrint" Icon="Printer" Text="打 印" DisableControlBeforePostBack="true" runat="server" Hidden="true" EnableDefaultState="false" />
                                <f:Button ID="btnExport" Icon="PageExcel" Text="导 出" OnClick="btExport_Click" ConfirmText="是否导出当前商品库存信息?" DisableControlBeforePostBack="false" EnableDefaultState="false"
                                    EnablePostBack="true" EnableAjax="false" EnableAjaxLoading="true" AjaxLoadingType="Mask" runat="server" Hidden="true"  />
                                <f:Button ID="btSearch" Icon="Magnifier" Text="下载EAS库存" OnClick="btSearch_Click" runat="server" EnableDefaultState="false" />
                                <f:Button ID="btnCompare" Icon="ArrowRefresh" Text="比 较" OnClick="btnCompare_Click" runat="server" EnableDefaultState="false" />
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Items>
                        <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="5px"
                            ShowHeader="False" LabelWidth="80px" runat="server">
                            <Rows>
                                <f:FormRow>
                                    <Items>
                                        <%--<f:DropDownList ID="ddlDEPTID" runat="server" Label="库房/科室" EnableEdit="true" ForceSelection="true" />--%>
                                        <f:TextBox ID="tbxGOODS" runat="server" Label="商品" EmptyText="可输入编码、名称或助记码" />
                                        <f:DropDownList ID="ddlCATID" runat="server" Label="商品分类" EnableEdit="true" ForceSelection="true" />
                                        <%--<f:DropDownList ID="ddlSUPID" runat="server" Label="供应商" EnableEdit="true" ForceSelection="true" />
                                        <f:TextBox ID="tbxZPBH" runat="server" Label="制品编号" EmptyText="请输入制品编码" />--%>
                                        <f:ToolbarFill ID="ToolbarFill2" runat="server" />

                                    </Items>
                                </f:FormRow>

                            </Rows>
                        </f:Form>
                    </Items>
                </f:Panel>
                <f:Grid ID="GridGoods" AnchorValue="100% -75" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;"
                    PageSize="100" IsDatabasePaging="true" AllowPaging="true" OnPageIndexChange="GridGoods_PageIndexChange" EnableColumnLines="true"
                    AllowSorting="true" AutoScroll="true" runat="server" DataKeyNames="GDSEQ" OnSort="GridGoods_Sort" SortField="GDSEQ" SortDirection="ASC" EnableTextSelection="true">
                    <Columns>
                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                        <%--<f:BoundField Width="130px" DataField="DEPTID" SortField="DEPTID" HeaderText="存货地点" />--%>
                        <f:BoundField Width="105px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" />
                        <f:BoundField Width="105px" DataField="BAR3" SortField="BAR3" HeaderText="EAS商品编码" ColumnID="BAR3" />
                        <f:BoundField Width="180px" DataField="GDNAME" SortField="GDNAME" HeaderText="商品名称" />
                        <f:BoundField Width="100px" DataField="GDSPEC" SortField="GDSPEC" HeaderText="规格" />
                        <f:BoundField Width="50px" DataField="UNIT" SortField="UNIT" HeaderText="单位" />
                        <f:BoundField Width="80px" DataField="PHID" SortField="PHID" HeaderText="批号" />
                        <f:BoundField Width="80px" DataField="PHID_EAS" HeaderText="EAS批号" ColumnID="PHID_EAS" />
                        <%--                        <f:BoundField Width="120px" DataField="CATNAME" SortField="CATID" HeaderText="商品分类" />
                        <f:BoundField Width="60px" DataField="BZHL" Hidden="true" HeaderText="包装含量" />--%>
                        <%--<f:BoundField Width="80px" DataField="HWID" SortField="HWID" HeaderText="货位" />--%>
                        <f:BoundField Width="70px" DataField="sumkcsl" SortField="SL" HeaderText="库存数" TextAlign="Right" />
                        <f:BoundField Width="70px" DataField="SL_EAS" HeaderText="EAS库存数" TextAlign="Right" ColumnID="SL_EAS" />
                        <%--              <f:BoundField Width="170px" DataField="SUPNAME" SortField="SUPID" HeaderText="供应商" />--%>
                        <%--                        <f:BoundField Width="70px" DataField="KCHSJJ" SortField="KCHSJJ" HeaderText="含税进价" DataFormatString="{0:F2}" TextAlign="Right" />--%>
                        <%--                        <f:BoundField Width="80px" DataField="SUMKCHSJE" SortField="SUMKCHSJE" HeaderText="含税金额" DataFormatString="{0:F2}" TextAlign="Right" />
                        <f:BoundField Width="100px" DataField="ZPBH" SortField="ZPBH" HeaderText="制品编号" />--%>
                        <%--<f:BoundField Width="100px" DataField="BILLNO" SortField="BILLNO" HeaderText="单据编号" />--%>

                        <%--                        <f:BoundField Width="80px" DataField="RQ_SC" SortField="RQ_SC" HeaderText="生产日期" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="80px" DataField="YXQZ" SortField="YXQZ" HeaderText="效期" DataFormatString="{0:yyyy-MM-dd}" />--%>

                    </Columns>
                </f:Grid>
                <f:HiddenField ID="highLightedRows" runat="server" />
                <f:HiddenField ID="yellowRows" runat="server" />
                <f:HiddenField ID="greenRows" runat="server" />
            </Items>
        </f:Panel>
    </form>
</body>
<%--<script src="../res/js/jquery-1.11.1.min.js"></script>--%>
<script>

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
</html>
