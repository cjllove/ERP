<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OutStrategy.aspx.cs" Inherits="ERPProject.ERPDictionary.OutStrategy" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>出库群组管理</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="MainPanel" runat="server" OnCustomEvent="PageManager1_CustomEvent" />
        <f:Panel ID="MainPanel" runat="server" Layout="HBox" BoxConfigAlign="Stretch" BoxConfigPosition="Start" ShowHeader="False" ShowBorder="false"
            CssStyle="border-bottom: 1px solid #99bce8;">
            <Toolbars>
                <f:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                        <f:TriggerBox ID="trbSearch" runat="server" EmptyText="请输入模板名、称科室编码" TriggerIcon="Search" Width="320px" OnTriggerClick="trbSearch_TriggerClick"></f:TriggerBox>
                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                        <f:Button ID="btnNew" Icon="PageAdd" Text="新 增" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnNew_Click" />
                        <f:Button ID="btnDel" Icon="PageCancel" Text="删除" EnablePostBack="true" EnableDefaultState="false" ConfirmText="是否确认删除此模板?" runat="server" OnClick="btnDel_Click" />
                        <f:ToolbarSeparator runat="server" />
                        <f:Button ID="btnDelRow" Icon="Delete" Text="删 行" EnablePostBack="true" runat="server" EnableDefaultState="false" ConfirmText="是否确认删除选中行?" OnClick="btnDelRow_Click" />
                        <f:Button ID="btnSave" Icon="Disk" Text="保存" EnablePostBack="true" EnableDefaultState="false" runat="server" ValidateForms="FormCond" OnClick="btnSave_Click" />
                        <f:ToolbarSeparator runat="server" />
                        <f:Button ID="mbtnPrint" runat="server" Icon="PageAdd" Text="启用/作废" EnableDefaultState="false">
                            <Menu runat="server">
                                <f:MenuButton ID="btnAdd" Icon="PageAdd" Text="启用"  runat="server" OnClick="btnAdd_Bill"/>
                                <f:MenuButton ID="btnDelete" Icon="PageCancel" Text="作废" runat="server" OnClick="btnDelete_Bill" />
                            </Menu>
                        </f:Button>
                        <f:Button ID="mbtnExcel" runat="server" Icon="PageExcel" Text="导出/导入" EnableDefaultState="false">
                            <Menu runat="server">
                                <f:MenuButton ID="btnExport" Icon="PageExcel" Text="导 出" OnClick="btnExport_Click" ConfirmText="是否确认导出此模板信息?" DisableControlBeforePostBack="false" EnablePostBack="true" EnableAjax="false" EnableAjaxLoading="true" AjaxLoadingType="Mask" runat="server" />
                                <f:MenuButton ID="btnImport" Icon="DatabaseGo" Text="导 入" EnablePostBack="true" runat="server" OnClick="btnImport_Click" />
                            </Menu>
                        </f:Button>
                        <f:ToolbarSeparator runat="server" />
                        <f:Button ID="btnGoods" Icon="Magnifier" Text="追加商品" EnableDefaultState="false" EnablePostBack="true" ValidateForms="FormCond" runat="server" OnClick="btnGoods_Click" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Panel ID="PanelRight" ShowBorder="false" BoxFlex="3" BodyPadding="0px"
                    Layout="Fit" ShowHeader="False" runat="server">
                    <Items>
                        <f:Grid ID="GridStrategy" ShowBorder="false" ShowHeader="false" runat="server" DataKeyNames="DEPTID,CATID,GROUPID,GROUPNAME,TYPE" EnableColumnLines="true" EnableMultiSelect="true" CheckBoxSelectOnly="true"
                            EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridStrategy_RowDoubleClick" CssStyle="border-right: 1px solid #99bce8;">
                            <Columns>
                                <f:RowNumberField Width="35px" TextAlign="Center"></f:RowNumberField>
                                <f:BoundField Hidden="true" DataField="GROUPID" />
                                <f:BoundField Hidden="true" DataField="DEPTID" />
                                <f:BoundField Hidden="true" DataField="TYPE" />
                                <f:BoundField Width="80px" DataField="DEPTNAME" HeaderText="所属科室" />
                                <f:BoundField Hidden="true" DataField="CATID" HeaderText="商品类别" TextAlign="Center" />
                                <%--<f:BoundField Width="80px" DataField="CATIDNAME" HeaderText="商品类别" TextAlign="Center" />--%>
                                <f:BoundField Width="120px" DataField="GROUPNAME" HeaderText="模板名称" ExpandUnusedSpace="true" />
                                <f:BoundField Width="75px" DataField="FLAGNAME" HeaderText="模板状态"  TextAlign="Center"  />
                            </Columns>
                            <%--<Listeners>--%>
                            <%-- <f:Listener Event="cellkeydown" Handler="onCellClick" />--%>
                            <%-- <f:Listener Event="cellmousedown" Handler="onMouseDown" />--%>
                            <%--</Listeners>--%>
                        </f:Grid>
                    </Items>
                </f:Panel>
                <f:Panel ID="PanelLeft" ShowBorder="false" BoxFlex="7" BodyPadding="0px" Layout="VBox" ShowHeader="False" runat="server">
                    <Items>
                        <f:Form ID="FormCond" ShowBorder="false" BodyPadding="10px"
                            ShowHeader="False" runat="server" LabelWidth="75px">
                            <Rows>
                                <f:FormRow>
                                    <Items>
                                        <f:HiddenField ID="hfdGROUPID" runat="server"></f:HiddenField>
                                    </Items>
                                </f:FormRow>
                                <f:FormRow>
                                    <Items>
                                        <f:DropDownList ID="ddlTYPE" runat="server" Label="模板选择" LabelWidth="80" EnableEdit="false" Required="true" ShowRedStar="true" Hidden="false">
                                            <f:ListItem Text="---请选择---" Value="" />
                                            <f:ListItem Text="申领模板" Value="K" />
                                            <f:ListItem Text="订货模板" Value="D" />
                                            <f:ListItem Text="手术套包" Value="S" />
                                        </f:DropDownList>
                                        <%--<f:DropDownList ID="docDEPTOUT" runat="server" Label="出库部门" Required="true" ShowRedStar="true" />--%>
                                        <f:DropDownList ID="docDEPTID" runat="server" Label="部门科室" Required="true" ShowRedStar="true" Hidden="false" EnableEdit="true" ForceSelection="true" />
                                        <%-- <f:DropDownList ID="docCATID" runat="server" Label="商品类别" ShowRedStar="true" Required="true" />--%>
                                        <f:TextBox ID="docGROUPNAME" Label="模板名称" runat="server" Required="true" ShowRedStar="true" Hidden="false" />
                                    </Items>
                                </f:FormRow>
                            </Rows>
                        </f:Form>
                        <f:Grid ID="GridGoods" BoxFlex="1" ShowBorder="false" ShowHeader="false" AutoScroll="true" runat="server" DataKeyNames="GDSEQ,nbxDHSL" AllowCellEditing="true" EnableColumnLines="true" CssStyle="border-top: 1px solid #99bce8;" ClicksToEdit="1">
                            <Columns>
                                <f:RenderField Width="40px" ColumnID="ROWNO" DataField="ROWNO" FieldType="String"
                                    HeaderText="" EnableHeaderMenu="false" TextAlign="Center">
                                    <Editor>
                                        <f:Label ID="lblEditorROWNO" runat="server"></f:Label>
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="105px" ColumnID="GDSEQ" DataField="GDSEQ" FieldType="String"
                                    HeaderText="商品编码" EnableHeaderMenu="false">
                                    <Editor>
                                        <f:Label ID="lblEditorGDSEQ" runat="server"></f:Label>
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="180px" ColumnID="GDNAME" DataField="GDNAME" FieldType="String" EnableHeaderMenu="false"
                                    HeaderText="商品名称" EnableLock="true" Locked="true">
                                    <Editor>
                                        <f:Label ID="lblEditorName" runat="server" />
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="100px" ColumnID="GDSPEC" DataField="GDSPEC" FieldType="String" EnableHeaderMenu="false"
                                    HeaderText="商品规格">
                                    <Editor>
                                        <f:Label ID="lblEditorGDSPEC" runat="server" />
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="100px" ColumnID="SL" DataField="SL" FieldType="Int" EnableHeaderMenu="false" EnableColumnHide="false" TextAlign="Center"
                                    HeaderText="申领数量<font color='red'>*</font>">
                                    <Editor>
                                        <f:NumberBox ID="nbxDHSL" runat="server" MinValue="0" MaxValue="99999999" DecimalPrecision="6" />
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="60px" ColumnID="UNITNAME" DataField="UNITNAME" FieldType="String" EnableHeaderMenu="false"
                                    HeaderText="申领单位" TextAlign="Center">
                                    <Editor>
                                        <f:Label ID="lblEditorUNITNAME" runat="server" />
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="70px" ColumnID="BZHL" DataField="BZHL" FieldType="String" EnableHeaderMenu="false"
                                    HeaderText="包装含量" TextAlign="Center">
                                    <Editor>
                                        <f:Label ID="lblEditorBZHL" runat="server" />
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="70px" ColumnID="HSJJ" DataField="HSJJ" HeaderText="含税进价" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false">
                                    <Editor>
                                        <f:Label ID="Label4" runat="server" />
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="70px" ColumnID="HSJE" DataField="HSJE" HeaderText="含税金额" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false">
                                    <Editor>
                                        <f:Label ID="Label5" runat="server" />
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="180px" ColumnID="PRODUCERNAME" DataField="PRODUCERNAME" FieldType="String" EnableHeaderMenu="false"
                                    HeaderText="生产厂家">
                                    <Editor>
                                        <f:Label ID="lblEditorPRODUCERNAME" runat="server" />
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="100px" ColumnID="PZWH" DataField="PZWH" FieldType="String" EnableHeaderMenu="false"
                                    HeaderText="注册证号" TextAlign="Center" Hidden="true">
                                    <Editor>
                                        <f:Label ID="lblEditorPZWH" runat="server" />
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="200px" ColumnID="MEMO" DataField="MEMO" FieldType="String" EnableHeaderMenu="false"
                                    HeaderText="备注<font color='red'>*</font>" TextAlign="Center" ExpandUnusedSpace="true">
                                    <Editor>
                                        <f:TextBox ID="tbxEditorMEMO" runat="server" MaxLength="80" />
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="0px" ColumnID="UNIT" DataField="UNIT" FieldType="String" EnableHeaderMenu="false"
                                    HeaderText="单位" Hidden="true">
                                    <Editor>
                                        <f:Label ID="Label1" runat="server" />
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="0px" ColumnID="PRODUCER" DataField="PRODUCER" FieldType="String" EnableHeaderMenu="false"
                                    HeaderText="生产厂家" Hidden="true">
                                    <Editor>
                                        <f:Label ID="Label2" runat="server" />
                                    </Editor>
                                </f:RenderField>
                            </Columns>
                            <Listeners>
                                <f:Listener Event="afteredit" Handler="onGridAfterEdit" />
                            </Listeners>
                        </f:Grid>
                    </Items>
                </f:Panel>
            </Items>
        </f:Panel>
        <f:HiddenField ID="hfdValue" runat="server" />
        <f:Window ID="winGoodsQuery" Title="商品信息" Hidden="true" EnableIFrame="true" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Self" IsModal="True"
            Width="820px" Height="480px" OnClose="winGoodsQuery_Close">
        </f:Window>
        <f:Window ID="WindowImport" Title="模板信息导入" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="370px" Height="200px">
            <Items>
                <f:Form ID="FormFile" ShowBorder="false" AutoScroll="false" BodyPadding="30px"
                    ShowHeader="False" LabelWidth="100px" runat="server">
                    <Rows>
                        <f:FormRow Hidden="false">
                            <Items>
                                <f:FileUpload ID="fuImport" runat="server" Label="请选择模板"></f:FileUpload>
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar4" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Label ID="Label3" EncodeText="false" Text="<a href='../ERPUpload/申领导入模板.xlsx' style='color: #1ea3d8' target='_blank'>点击下载模板</a></p>"
                            runat="server">
                        </f:Label>
                        <f:Button ID="btnClosePostBack" Text="确定" EnableDefaultState="false" Icon="SystemSave" runat="server" OnClick="btnClosePostBack_Click">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
    </form>
    <script language="javascript" type="text/javascript">
        function onEditorCellClick(grid, rowIndex, columnIndex, e) {
            if (columnIndex == 4) {
                return true;
            }
            else {
                return false;
            }
        }
        function PosGoodsQt(Info) {
            F('<%= hfdValue.ClientID%>').setValue(Info);
            F.customEvent('GoodsAdd');
        }
        function onGridAfterEdit(event, value, params) {
            var me = this, columnId = params.columnId, rowId = params.rowId;
            if (columnId === 'SL') {
                var SL = me.getCellValue(rowId, 'SL');
                var HSJJ = me.getCellValue(rowId, 'HSJJ');
                me.updateCellValue(rowId, 'HSJE', SL * HSJJ);
            }
        }
    </script>
</body>
</html>
