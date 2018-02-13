<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderEntry.aspx.cs" Inherits="ERPProject.ERPStorage.OrderEntry" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>商品订货管理</title>
    <meta name="sourcefiles" content="~/ERPQuery/GoodsWindow.aspx" />
    <link href="../res/css/jquery-ui.min.css" rel="stylesheet" />
    <link href="../res/css/jquery-ui.theme.min.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="True" BodyPadding="0px" Layout="Anchor"
            ShowHeader="false">
            <Items>
                <f:Panel ID="PanelHeader" ShowBorder="false" AnchorValue="100% 20%" BodyPadding="0px"
                    Layout="Anchor" ShowHeader="False" runat="server">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar1" runat="server">
                            <Items>
                                <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="操作信息：" runat="server" />
                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                <f:Button ID="btnClear" Icon="ArrowRotateClockwise" Text="清 空" EnablePostBack="true" runat="server" OnClick="btnClear_Click" />
                                <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" runat="server" EnablePostBack="true" OnClick="btSearch_Click" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="btnAddRow" CssStyle="margin-left: 15px;" OnClick="btnAddRow_Click" Icon="Add"
                                    Text="增 行" EnablePostBack="true" runat="server" />
                                <f:Button ID="btnDelRow" CssStyle="margin-left: 15px;" OnClick="btnDelRow_Click" ConfirmText="是否确认删除选中行信息?" Icon="Delete"
                                    Text="删 行" EnablePostBack="true" runat="server" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="btnAuto" CssStyle="margin-left: 15px;" OnClick="btnAuto_Click" Icon="Add"
                                    Text="自动订货" EnablePostBack="true" runat="server" />
                                <f:Button ID="btnPrint" CssStyle="margin-left: 15px;" OnClick="btnPrint_Click" Icon="Printer"
                                    Text="订单打印" EnablePostBack="true" runat="server" />
                                <f:Button ID="btSave" CssStyle="margin-left: 15px;margin-right: 11px;" OnClick="btSave_Click"
                                    Icon="Disk" Text="保 存" DisableControlBeforePostBack="false" runat="server" />
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Items>
                        <f:Form ID="FormOrder" ShowBorder="false" AutoScroll="false" BodyPadding="5px 10px 0px 10px"
                            ShowHeader="False" LabelWidth="80px" runat="server">
                            <Rows>
                                <f:FormRow>
                                    <Items>
                                        <f:TextBox ID="tbxDept" runat="server" Label="部门">
                                        </f:TextBox>
                                        <f:DropDownList ID="ddlGoodsType" runat="server" Label="商品分类" EnableEdit="true" ForceSelection="true">
                                        </f:DropDownList>
                                        <f:DropDownList ID="ddlOperation" runat="server" Label="处理" EnableEdit="true" ForceSelection="true">
                                            <f:ListItem Text="新增" Value="1" />
                                            <f:ListItem Text="指定输入" Value="2" />
                                            <f:ListItem Text="选择输入" Value="3" />
                                        </f:DropDownList>
                                        <f:DropDownList ID="ddlCategory" runat="server" Label="库存分类" EnableEdit="true" ForceSelection="true">
                                            <f:ListItem Text="全部商品" Value="A" />
                                            <f:ListItem Text="库存商品" Value="0" />
                                            <f:ListItem Text="直送商品" Value="1" />
                                        </f:DropDownList>
                                    </Items>
                                </f:FormRow>
                                <f:FormRow ColumnWidths="25% 25% 25%">
                                    <Items>
                                        <f:TextBox ID="tbxOrderingDept" runat="server" Label="出库对象">
                                        </f:TextBox>
                                        <f:DatePicker ID="dpkOrderDate" runat="server" Label="订货时间"></f:DatePicker>
                                        <f:DropDownList ID="ddlOrderingCategory" runat="server" Label="订货分类" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true">
                                            <f:ListItem Text="定时" Value="1" />
                                            <f:ListItem Text="临时" Value="2" />
                                            <f:ListItem Text="批量" Value="3" />
                                        </f:DropDownList>
                                    </Items>
                                </f:FormRow>
                                <f:FormRow Hidden="true">
                                    <Items>
                                        <f:HiddenField ID="hfdCacheKey" runat="server" />
                                        <f:HiddenField ID="hfdValue" runat="server" />
                                    </Items>
                                </f:FormRow>
                            </Rows>
                        </f:Form>
                    </Items>
                </f:Panel>
                <f:Grid ID="GridGoods" AnchorValue="100% 55%" ShowBorder="true" AllowCellEditing="true" ClicksToEdit="2" ShowHeader="false"
                    AllowSorting="false" AutoScroll="true" runat="server" DataKeyNames="GDSEQ" OnRowCommand="GridGoods_RowCommand">
                    <Columns>
                        <f:RowNumberField />
                        <f:BoundField Width="70px" ColumnID="GDSEQ" DataField="GDSEQ" Hidden="true" />
                        <f:RenderField Width="180px" ColumnID="NAME" DataField="NAME" FieldType="String"
                            HeaderText="商品名称">
                            <Editor>
                                <f:TextBox ID="tbxEditorName" Required="true" runat="server">
                                </f:TextBox>
                            </Editor>
                        </f:RenderField>
                        <%--<f:RenderField Width="100px" ColumnID="GDSPEC" DataField="GDSPEC" FieldType="String"
                            HeaderText="规格·容量">
                            <Editor>
                                <f:Label ID="lblEditorSpgg" runat="server" />
                            </Editor>
                        </f:RenderField>
                        <f:RenderField Width="70px" ColumnID="UNIT" DataField="UNIT" FieldType="String"
                            HeaderText="包装单位">
                            <Editor>
                                <f:Label ID="lblUnit" runat="server" />
                            </Editor>
                        </f:RenderField>--%>
                        <f:BoundField Width="100px" ColumnID="GDSPEC" DataField="GDSPEC" HeaderText="规格·容量" />
                        <f:BoundField Width="70px" ColumnID="UNIT" DataField="UNIT" HeaderText="包装单位" />
                        <f:BoundField Width="70px" ColumnID="BZHL" DataField="BZHL" HeaderText="包装数量" />
                        <f:BoundField Width="100px" ColumnID="ZPBH" DataField="ZPBH" HeaderText="制品编号" />
                        <f:BoundField Width="70px" ColumnID="SPLB" DataField="SPLB" HeaderText="库存分类" />
                        <f:BoundField Width="130px" ColumnID="CDID" DataField="CDID" HeaderText="产地" />
                        <f:BoundField Width="180px" ColumnID="SUPID" DataField="SUPID" ExpandUnusedSpace="true" HeaderText="供应商" />
                        <f:BoundField Width="70px" ColumnID="HSJJ" DataField="HSJJ" HeaderText="单价" />
                        <f:TemplateField HeaderText="数量" Width="90px">
                            <ItemTemplate>
                                <input type="hidden" class="price" runat="server" value='<%# Eval("HSJJ") %>' />
                                <asp:TextBox runat="server" Width="98%" ID="tbxNumber" CssClass="number" Text='<%# Eval("DHS") %>'></asp:TextBox>
                            </ItemTemplate>
                        </f:TemplateField>
                        <f:TemplateField HeaderText="订货金额" Width="90px">
                            <ItemTemplate>
                                <asp:Label runat="server" CssClass="xiaoji" Text='<%# "¥" + GetXiaoji(Eval("HSJJ"), Eval("DHS")) %>'></asp:Label>
                            </ItemTemplate>
                        </f:TemplateField>
                        <%--<f:BoundField Width="90px" ColumnID="HSJE" DataField="HSJE" HeaderText="订货金额" />--%>
                        <f:BoundField Width="90px" ColumnID="KCSL" DataField="KCSL" HeaderText="库存数" />
                        <f:BoundField Width="90px" ColumnID="CKBM" DataField="CKBM" HeaderText="出库对象" />
                        <f:BoundField Width="100px" ColumnID="LOT" DataField="LOT" HeaderText="批号" />
                        <f:BoundField Width="100px" ColumnID="BZRQ" DataField="BZRQ" HeaderText="效期" />
                    </Columns>
                    <Listeners>
                        <f:Listener Event="beforeitemcontextmenu" Handler="onRowContextMenu" />
                    </Listeners>
                </f:Grid>
                <f:Menu ID="Menu1" runat="server">
                    <f:MenuButton ID="btnSelectRows" EnablePostBack="false" runat="server" Text="全选行">
                        <Listeners>
                            <f:Listener Event="click" Handler="onSelectRows" />
                        </Listeners>
                    </f:MenuButton>
                    <f:MenuButton ID="btnUnselectRows" EnablePostBack="false" runat="server" Text="取消行">
                        <Listeners>
                            <f:Listener Event="click" Handler="onUnselectRows" />
                        </Listeners>
                    </f:MenuButton>
                </f:Menu>

                <f:GroupPanel ID="GroupPanel1" AnchorValue="100% -400" Layout="Table" TableConfigColumns="2" Height="150px" Title="信息说明" runat="server">
                    <Items>
                        <f:Grid ID="Grid1" runat="server" ShowBorder="true" ShowHeader="false" Width="600px" Height="70px">
                            <Columns>
                                <f:BoundField Width="70px" ColumnID="Class" DataField="Class" />
                                <f:BoundField Width="70px" ColumnID="Month1" DataField="Month1" HeaderText="2014/02" />
                                <f:BoundField Width="70px" ColumnID="Month2" DataField="Month2" HeaderText="2014/03" />
                                <f:BoundField Width="70px" ColumnID="Month3" DataField="Month3" HeaderText="2014/04" />
                                <f:BoundField Width="70px" ColumnID="Month4" DataField="Month4" HeaderText="2014/05" />
                                <f:BoundField Width="70px" ColumnID="Month5" DataField="Month5" HeaderText="2014/06" />
                                <f:BoundField Width="70px" ColumnID="Month6" DataField="Month6" HeaderText="2014/07" />
                                <f:BoundField Width="70px" ColumnID="Month7" DataField="Month7" HeaderText="2014/08" />
                            </Columns>
                        </f:Grid>
                        <f:Grid ID="Grid2" runat="server" TableRowspan="2" ShowBorder="true" ShowHeader="false" Width="440px" Height="140px">
                            <Columns>
                                <f:BoundField Width="100px" ColumnID="Class" DataField="Class" />
                                <f:BoundField Width="110px" ColumnID="DayInfo" DataField="DayInfo" HeaderText="平日自动订货" />
                                <f:BoundField Width="110px" ColumnID="WeekInfo" DataField="WeekInfo" HeaderText="周末自动订货" />
                                <f:BoundField Width="110px" ColumnID="MonthInfo" DataField="MonthInfo" HeaderText="月末自动订货" />
                            </Columns>
                        </f:Grid>
                        <f:Grid ID="Grid3" runat="server" ShowBorder="true" ShowHeader="false" Width="600px" Height="70px">
                            <Columns>
                                <f:BoundField Width="110px" ColumnID="Storeing" DataField="Storeing" HeaderText="未入库数" />
                                <f:BoundField Width="120px" ColumnID="Stored" DataField="Stored" HeaderText="商品库存数" />
                                <f:BoundField Width="120px" ColumnID="Coefficient" DataField="Coefficient" HeaderText="订货系数" />
                                <f:BoundField Width="120px" ColumnID="PurchaseDay" DataField="PurchaseDay" HeaderText="上次采购日期" />
                                <f:BoundField Width="120px" ColumnID="PurchaseSum" DataField="PurchaseSum" HeaderText="上次采购数" />
                            </Columns>
                        </f:Grid>
                    </Items>
                </f:GroupPanel>
            </Items>
        </f:Panel>
        <f:Window ID="Window1" runat="server" IsModal="true" Hidden="true" Target="Parent"
            EnableIFrame="true" Width="820px" Height="480px" AutoScroll="false" CloseAction="HidePostBack"
            OnClose="Window1_Close">
        </f:Window>
    </form>
    <script type="text/javascript" src="../res/js/jquery-1.11.1.min.js"></script>
    <script type="text/javascript" src="../res/js/jquery-ui.min.js"></script>
    <script type="text/javascript">
        var tbxDept = '<%= tbxDept.ClientID %>';
        var tbxOrderDept = '<%= tbxOrderingDept.ClientID %>'
        var menuID = '<%= Menu1.ClientID %>';
        var gridClientID = '<%= GridGoods.ClientID %>';
        var numberSelector = '.f-grid-tpl input.number';
        var priceSelector = '.f-grid-tpl input.price';

        function getRowNumber(row) {
            return parseInt(row.find(numberSelector).val(), 10);
        }
        function getRowPrice(row) {
            return parseFloat(row.find(priceSelector).val());
        }

        function updateTotal() {
            var grid = F(gridClientID);
            var selection = grid.getSelectionModel().getSelection();
            var store = grid.getStore();

            var total = 0;
            $.each(selection, function (index, item) {
                var rowIndex = store.indexOf(item);
                var row = $(grid.body.el.dom).find('.x-grid-row').eq(rowIndex);
                total += getRowNumber(row) * getRowPrice(row);
            });

            $('#totalNumber').text(selection.length);
            $('#totalPrice').text("¥" + total.toFixed(2));

            $('#TOTAL_NUMBER').val(selection.length);
            $('#TOTAL_PRICE').val(total.toFixed(2));

            var gotoPayBtn = F(btnGotoPayClientID);
            if (total === 0) {
                gotoPayBtn.disable();
            } else {
                gotoPayBtn.enable();
            }
        }

        function registerNumberChangeEvents() {
            var grid = F(gridClientID);

            // 数量改变事件
            // http://stackoverflow.com/questions/17384218/jquery-input-event
            $(grid.el.dom).find(numberSelector).on('input propertychange', function (evt) {
                var $this = $(this);

                var row = $this.parents('.x-grid-row');
                var number = getRowNumber(row);
                var price = getRowPrice(row);
                var resultNode = row.find('.f-grid-tpl span.xiaoji');

                resultNode.text("¥" + (number * price).toFixed(2));

                //updateTotal();
            });
        }

        function registerSelectionChangeEvents() {
            var grid = F(gridClientID);

            grid.on('selectionchange', function (cmp, selected) {
                updateTotal();
            });
        }

        function onRowContextMenu(view, record, item, index, event) {
            F(menuID).showAt(event.getXY());
            event.stopEvent();
        }

        F.ready(function () {
            var cache = {};

            $('#' + tbxDept + ' input').autocomplete({
                minLength: 2,
                source: function (request, response) {
                    var term = request.term;
                    if (term in cache) {
                        response(cache[term]);
                        return;
                    }

                    $.getJSON("../captcha/AutoComplete.ashx", request, function (data, status, xhr) {
                        cache[term] = data;
                        response(data);
                    });
                }
            });

            $('#' + tbxOrderDept + ' input').autocomplete({
                minLength: 2,
                source: function (request, response) {
                    var term = request.term;
                    if (term in cache) {
                        response(cache[term]);
                        return;
                    }

                    $.getJSON("../captcha/AutoComplete.ashx", request, function (data, status, xhr) {
                        cache[term] = data;
                        response(data);
                    });
                }
            });
            registerNumberChangeEvents();
            //registerSelectionChangeEvents();
            //updateTotal();
        });
    </script>
</body>
</html>
