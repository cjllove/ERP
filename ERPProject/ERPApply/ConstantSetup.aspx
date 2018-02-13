<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConstantSetup.aspx.cs" Inherits="ERPProject.ERPApply.ConstantSetup" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>自动订货设置</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="true" ID="PageManager1" AutoSizePanelID="PanelMain" runat="server" />
        <f:Panel ID="PanelMain" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor" ShowHeader="false">
            <Items>
                <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                    ShowHeader="False" LabelWidth="75px" runat="server">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar2" runat="server">
                            <Items>
                                <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="操作信息：批量设置科室商品的定数信息！" runat="server" />
                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                
                                <f:Button ID="bntSave" Icon="Disk" Text="保 存" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="bntSave_Click" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="btnExport" Icon="DatabaseGo" AjaxLoadingType="Mask" Text="导 出" EnableDefaultState="false" EnablePostBack="true" EnableAjax="false" EnableAjaxLoading="true" runat="server" DisableControlBeforePostBack="false" OnClick="btnExport_Click" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="bntSearch_Click" />
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Rows>
                        <f:FormRow>
                            <Items>
                                <f:TextBox ID="tbxGoodsName" runat="server" Label="品名/编码" />
                                <f:DropDownList ID="ddlDEPTID" runat="server" Label="科室" EnableEdit="true" ForceSelection="true">
                                    
                                </f:DropDownList>
                                <f:DropDownList ID="ddlGoodsType" runat="server" Label="商品大类" EnableEdit="true" ForceSelection="true" />
                                <f:DropDownList ID="ddlCATID" runat="server" Label="商品分类" EnableEdit="true" ForceSelection="true" />
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
                <f:Grid ID="GridGoods" AnchorValue="100% -80" ShowBorder="false" ShowHeader="false" AllowCellEditing="true" ClicksToEdit="1"
                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" IsDatabasePaging="true" EnableTextSelection="true"
                    DataKeyNames="GDSEQ,DEPTID,DSHANL,DSNUM,GDNAME" AllowPaging="true" PageSize="100" OnPageIndexChange="GridGoods_PageIndexChange"
                    AllowSorting="true" EnableHeaderMenu="true" SortField="DEPTNAME" OnSort="GridGoods_Sort" EnableColumnLines="true" >
                    <Columns>
                        <f:BoundField Width="120px" DataField="DEPTNAME" SortField="DEPTNAME" HeaderText="科室" />
                        <f:BoundField Width="60px" DataField="TYPENAME" SortField="TYPENAME" HeaderText="商品大类" TextAlign="Center" Hidden="true" />
                        <f:BoundField Width="160px" DataField="CATNAME" SortField="CATNAME" HeaderText="商品分类" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                        <f:BoundField Width="160px" DataField="GDNAME" SortField="GDNAME" HeaderText="商品名称"/>
                        <f:BoundField Width="120px" DataField="GDSPEC" SortField="GDSPEC" HeaderText="规格·容量" />
                        <f:BoundField Width="230px" DataField="PRODUCER" SortField="PRODUCER" HeaderText="生产厂家" />
                        <%--<f:BoundField Width="60px" DataField="UNITNAME_ORDER" HeaderText="订货单位" TextAlign="Center" />--%>
                        <%--<f:BoundField Width="70px" DataField="CATID" HeaderText="订货包装" TextAlign="Center" />--%>
                        <%--            <f:CheckBoxField Width="60px" DataField="ISAUTO" ColumnID="cbxISAUTO" TextAlign="Center" RenderAsStaticField="false" HeaderText="自动订货" />--%>
                        <f:RenderField Width="90px" ColumnID="DSNUM" DataField="DSNUM" FieldType="Int" SortField="DSNUM"
                            HeaderText="定数数量<font color='red'>*</font>" TextAlign="Center">
                            <Editor>
                                <f:NumberBox ID="nbxDSNUM" runat="server" MaxLength="9" MinValue="0" />
                            </Editor>
                        </f:RenderField>
                        <f:RenderField Width="90px" ColumnID="DSHANL" SortField="DSHANL" DataField="DSHANL" FieldType="Int"
                            HeaderText="定数含量<font color='red'>*</font>" TextAlign="Center">
                            <Editor>
                                <f:NumberBox ID="nbxDSHANL" runat="server" MaxLength="9" MinValue="0" />
                            </Editor>
                        </f:RenderField>
                        <f:BoundField Width="70px" DataField="UNITNAME" HeaderText="单位" SortField="UNITNAME" TextAlign="Center" />
                        <f:BoundField Width="70px" DataField="DSLS" HeaderText="定数流水" SortField="DSLS" TextAlign="Center" Hidden="true" />
                        <f:BoundField Width="70px" DataField="DAISHOU" HeaderText="待收数量" SortField="DAISHOU" TextAlign="Center" Hidden="true" />
                        <%-- <f:RenderField Width="70px" ColumnID="ZDKC" DataField="ZDKC" FieldType="Int"
                            HeaderText="最低库存" TextAlign="Center">
                            <Editor>
                                <f:NumberBox ID="nbxZDKC" runat="server" MinValue="0" />
                            </Editor>
                        </f:RenderField>
                        <f:RenderField Width="70px" ColumnID="ZGKC" DataField="ZGKC" FieldType="Int"
                            HeaderText="最高库存" TextAlign="Center">
                            <Editor>
                                <f:NumberBox ID="nbxZGKC" runat="server" MinValue="0" />
                            </Editor>
                        </f:RenderField>--%>
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
</body>
</html>
