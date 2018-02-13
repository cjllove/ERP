﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoodsStatus.aspx.cs" Inherits="ERPProject.ERPDictionary.GoodsStatus" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>商品状态维护</title>
    <style type="text/css">
        .f-grid-row[data-color=color1],
        .f-grid-row[data-color=color1] .ui-icon,
        .f-grid-row[data-color=color1] a {
            background-color: #3AA02C;
            color: #fff;
        }

        .f-grid-row[data-color=color3],
        .f-grid-row[data-color=color3] .ui-icon,
        .f-grid-row[data-color=color3] a {
            background-color: #AF5553;
            color: #fff;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="PanelDetail"
            runat="server" />
        <%--<f:Panel ID="PanelDetail" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor" ShowHeader="False">
            <Items>--%>
        <f:Panel ID="PanelDetail" ShowBorder="false" BodyPadding="0px"
            Layout="Anchor" ShowHeader="False" runat="server">
            <Items>
                <f:Panel runat="server" Layout="Anchor" ShowBorder="false" ShowHeader="false">
                    <Items>
                        <f:Form runat="server" ShowBorder="false" ShowHeader="false" LabelWidth="70px">
                            <Toolbars>
                                <f:Toolbar runat="server">
                                    <Items>
                                        <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                        <f:Button ID="bntAudit" Icon="UserTick" Text="启 用" EnablePostBack="true" ConfirmText="是否确定将选中商品审核？" runat="server" OnClick="bntAudit_Click" EnableDefaultState="false" />
                                        <f:ToolbarSeparator runat="server"></f:ToolbarSeparator>
                                        <f:Button ID="btnStopUse" OnClick="btnStopUse_Click" Icon="UserAlert" EnableDefaultState="false" ConfirmText="是否确定将选中商品停用？"
                                            Text="停 用" EnablePostBack="true" runat="server">
                                        </f:Button>
                                        <f:Button ID="btStopBuy" OnClick="btStopBuy_Click" EnableDefaultState="false" ConfirmText="是否确定将选中商品停购？"
                                            Icon="UserDelete" Text="停 购" DisableControlBeforePostBack="false" runat="server" />
                                        <f:Button ID="btnEliminated" OnClick="btnEliminated_Click" Icon="UserCross" EnableDefaultState="false"
                                            Text="淘 汰" DisableControlBeforePostBack="false" ConfirmText="是否确定将选中商品淘汰?" runat="server">
                                        </f:Button>
                                        <f:ToolbarSeparator runat="server"></f:ToolbarSeparator>
                                        <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="bntSearch_Click" />
                                    </Items>
                                </f:Toolbar>
                            </Toolbars>
                            <Rows>
                                <f:FormRow CssStyle="padding:10px 10px 5px 10px">
                                    <Items>
                                        <f:TriggerBox ID="trbSearch" runat="server" Label="商品信息" EmptyText="商品编码或者商品名称" ShowTrigger="false" OnTriggerClick="bntSearch_Click" />
                                        <f:DropDownList ID="srhFLAG" Label="商品状态" runat="server" ForceSelection="true" />
                                        <f:DropDownList ID="srhTYPE" Label="操作类型" runat="server" EnableEdit="true" ForceSelection="true">
                                            <f:ListItem Text="--请选择--" Value="" />
                                            <f:ListItem Text="新增" Value="NEW" />
                                            <f:ListItem Text="修改" Value="MOD" />
                                            <f:ListItem Text="正常" Value="NOW" />
                                            <f:ListItem Text="变更" Value="ALL" Selected="true" />
                                        </f:DropDownList>
                                    </Items>
                                </f:FormRow>
                            </Rows>
                        </f:Form>
                    </Items>
                </f:Panel>
                <f:Grid ID="GridGoods" AnchorValue="100% -80px" ShowBorder="false" ShowHeader="false" AllowSorting="false" CssStyle="border-top: 1px solid #99bce8;" EnableCheckBoxSelect="true" KeepCurrentSelection="true"
                    AutoScroll="true" runat="server" DataKeyNames="GDSEQ,FLAG,TYPENAME,CATID,CATID0,HSJJ,UNIT,GDNAME,PRODUCER" OnRowDataBound="GridGoods_RowDataBound" EnableCollapse="true"
                    PageSize="50" IsDatabasePaging="true" AllowPaging="true" OnPageIndexChange="GridGoods_PageIndexChange" EnableColumnLines="true" OnRowCommand="GridGoods_RowCommand" >
                    <Columns>
                        <f:RowNumberField runat="server" Width="35px" TextAlign="Center" />
                        <f:LinkButtonField Width="80px" TextAlign="Center" CommandName="Action1" Text="商品调整"  />
                        <f:BoundField DataField="TYPENAME" ColumnID="TYPENAME" Width="60px" HeaderText="操作类型" EnableHeaderMenu="false" TextAlign="Center" />
                        <f:BoundField Width="60px" DataField="FLAGNAME" ColumnID="FLAGNAME" HeaderText="商品状态" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="120px" DataField="GDSEQ" HeaderText="商品编码" ColumnID="GDSEQ" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="180px" DataField="GDNAME" HeaderText="商品名称" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="120px" DataField="CATID" Hidden="true" HeaderText="商品分类" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="120px" DataField="CATIDNAME" HeaderText="商品分类" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="120px" DataField="CATID0" Hidden="true" HeaderText="商品种类" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="120px" DataField="CATID0NAME" HeaderText="商品种类" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="80px" DataField="HSJJ" HeaderText="含税进价" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="140px" DataField="GDSPEC" HeaderText="规格·容量" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="60px" DataField="UNIT" HeaderText="单位" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="180px" DataField="PRODUCER" HeaderText="生产厂家" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="175px" DataField="PIZNO" HeaderText="注册证号" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="60px" DataField="BZHL" HeaderText="包装含量" Hidden="true" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="60px" DataField="ISGZ" HeaderText="是否高值" Hidden="true" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="60px" DataField="ISGZ_CN" HeaderText="是否高值" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="80px" DataField="UPTUSER" HeaderText="修改人" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="90px" DataField="UPTRQ" HeaderText="修改时间" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:yyyy-MM-dd}" TextAlign="Center" />
                        <f:TemplateField ColumnID="expander" RenderAsRowExpander="true">
                            <ItemTemplate>
                            </ItemTemplate>
                        </f:TemplateField>
                    </Columns>
                    <Listeners>
                        <f:Listener Event="rowexpanderexpand" Handler="onRowExpanderExpand" />
                        <f:Listener Event="rowexpandercollapse" Handler="onRowExpanderCollapse" />
                    </Listeners>
                </f:Grid>
            </Items>
        </f:Panel>
        <f:HiddenField ID="hfdGDSEQ" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdCATID" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdCATID0" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdHSJJ" runat="server"></f:HiddenField>
        <f:Window ID="WinSure" Title="商品审核" Hidden="true" EnableIFrame="false" runat="server" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="510px" Height="320px">
            <Items>
                <f:Grid ID="GridSure" ShowBorder="false" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server" ClicksToEdit="1" AllowCellEditing="true" EnableColumnLines="true"
                    DataKeyNames="GDSEQ">
                    <Columns>
                        <f:RowNumberField runat="server" Width="30px"></f:RowNumberField>
                        <f:RenderField Hidden="true" DataField="GDSEQ" ColumnID="GDSEQ" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false" TextAlign="Center"
                            HeaderText="商品名称">
                            <Editor>
                                <f:Label runat="server"></f:Label>
                            </Editor>
                        </f:RenderField>
                        <f:RenderField Width="170px" DataField="GDNAME" ColumnID="GDNAME" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false" TextAlign="Center"
                            HeaderText="商品名称">
                            <Editor>
                                <f:Label runat="server"></f:Label>
                            </Editor>
                        </f:RenderField>
                        <f:RenderField Width="160px" DataField="CATID" ColumnID="CATID" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false" RendererFunction="renderGender" TextAlign="Center"
                            HeaderText="商品分类<font color='red'>*</font>">
                            <Editor>
                                <f:DropDownList runat="server" Required="true" ID="ddlCATID" AutoPostBack="true"  OnSelectedIndexChanged="ddlCATID_SelectedIndexChanged"></f:DropDownList>
                            </Editor>
                        </f:RenderField>
                        <f:RenderField Width="110px" DataField="CATID0" ColumnID="CATID0" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false" RendererFunction="renderGender1" TextAlign="Center"
                            HeaderText="商品种类">
                            <Editor>
                                <f:DropDownList runat="server" Required="true" ID="ddlCATID0" Enabled="false"></f:DropDownList>
                            </Editor>
                        </f:RenderField>
                        <f:RenderField Width="110px" DataField="HSJJ" ColumnID="HSJJ" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false" TextAlign="Right"
                            HeaderText="含税进价<font color='red'>*</font>">
                            <Editor>
                                <f:NumberBox runat="server" ID="nbxHSJJ" Required="true" MinValue="0" DecimalPrecision="6" MaxLength="16"></f:NumberBox>
                            </Editor>
                        </f:RenderField>
                        <f:BoundField Width="180px" DataField="PRODUCER" HeaderText="生产厂家" Hidden="true" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="60px" DataField="UNIT" HeaderText="单位" Hidden="true" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                    </Columns>
                </f:Grid>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar1" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:HiddenField ID="hfdRowIndex" runat="server" />
                        <f:Button ID="btnClosePostBack" Text="确定" EnableDefaultState="false" Icon="SystemSave" OnClick="btnClosePostBack_Click" runat="server">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
        
        <f:Window ID="WindowCatID" Title="调整商品分类" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="370px" Height="180px">
            <Items>
                <f:Form ID="Form2" ShowBorder="false" AutoScroll="false" BodyPadding="5px 10px"
                    ShowHeader="False" LabelWidth="80px" runat="server">
                    <Rows>
                        <f:FormRow Hidden="true">
                            <Items>
                                <f:TextBox ID="winGDSEQ" runat="server" Label="商品编码"></f:TextBox>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:DropDownList ID="winCATID" runat="server" Label="商品分类" Required="true" ShowRedStar="true" AutoPostBack="true" Enabled="true" EnableEdit="true" ForceSelection="true" OnSelectedIndexChanged="winCATID_SelectedIndexChanged"></f:DropDownList>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:DropDownList ID="winCATID0" runat="server" Label="商品种类" EnableEdit="true" ForceSelection="true" Required="true" ShowRedStar="true" Enabled="false"></f:DropDownList>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:TextBox ID="winHSJJ" runat="server" Label="含税进价" Required="true" ShowRedStar="true" Enabled="true"></f:TextBox>
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar4" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="btnCatIDSubmit" Text="确定" EnableDefaultState="false" Icon="SystemSave" runat="server" OnClick="btnCatIDSubmit_Click">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
    </form>
    <script>
        var ddlCATID = '<%= ddlCATID.ClientID %>';
        var ddlCATID0 = '<%= ddlCATID0.ClientID %>';
        function renderGender(value) {
            return F(ddlCATID).getTextByValue(value);
        }
        function renderGender1(value) {

            return F(ddlCATID0).getTextByValue(value);
        }

        var grid1 = '<%= GridGoods.ClientID %>';
        function onRowExpanderExpand(event, rowId) {
            var grid = this, rowEl = grid.getRowEl(rowId), rowData = grid.getRowData(rowId);

            var tplEl = rowEl.find('.f-grid-rowexpander-details .f-grid-tpl');
            if (!tplEl.text()) {

                F.create({
                    type: 'grid',
                    renderTo: tplEl,
                    header: false,
                    columnMenu: false,
                    columnResizing: false,
                    cls: 'gridinrowexpander',
                    fields: ['TYPE', 'GDNAME', 'GDSPEC', 'UNIT', 'PRODUCER', 'PIZNO'],
                    columns: [{
                        text: '', field: 'TYPE', width: 60
                    }, {
                        text: '商品名称', field: 'GDNAME', width: 120
                    }, {
                        text: '规格·容量', field: 'GDSPEC', width: 120
                    }, {
                        text: '单位', field: 'UNIT', width: 60
                    }, {
                        text: '生产厂家', field: 'PRODUCERNAME', width: 140
                    }, {
                        text: '注册证号', field: 'PIZNO', width: 140
                    }],
                    dataUrl: '../captcha/GetModGoods.ashx?gdseq=' + encodeURIComponent(rowData.values['GDSEQ']), // 这里可传递行中任意数据（rowData）
                    listeners: {
                        dataload: function () {
                            rowExpandersDoLayout();
                        }
                    }
                });

            }
        }
        function onRowExpanderCollapse(event, rowId) {
            rowExpandersDoLayout();
        }
        // 重新布局表格和行扩展列中的表格（解决出现纵向滚动条时的布局问题）
        function rowExpandersDoLayout() {
            var grid1Cmp = F(grid1);
            grid1Cmp.doLayout();
            $('.f-grid-row:not(.f-grid-rowexpander-collapsed) .gridinrowexpander').each(function () {
                var gridInside = F($(this).attr('id'));
                gridInside.doLayout();
            });
        }
    </script>
</body>
</html>