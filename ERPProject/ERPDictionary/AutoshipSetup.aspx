﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AutoshipSetup.aspx.cs" Inherits="ERPProject.ERPDictionary.AutoshipSetup" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>自动订货设置</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="PanelMain" runat="server" />
        <f:Panel ID="PanelMain" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor" ShowHeader="false">
            <Items>
                <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                    ShowHeader="False" LabelWidth="70px" runat="server">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar2" runat="server">
                            <Items>
                                <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="操作信息：设置商品上下限！" runat="server" />
                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                <f:Button ID="bntSave" Icon="Disk" Text="保 存" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="bntSave_Click" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="bntSearch_Click" />
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Rows>
                        <f:FormRow>
                            <Items>
                                <f:TextBox ID="tbxGoodsName" runat="server" Label="商品信息" MaxLength="20" EmptyText="输入商品信息" />
                                <f:DropDownList ID="ddlDEPTID" runat="server" Label="库房" EnableEdit="true" ForceSelection="true" />
                                <f:DropDownList ID="ddlGoodsType" runat="server" Label="商品大类" EnableEdit="true" ForceSelection="true" />
                                <f:DropDownList ID="ddlCATID" runat="server" Label="商品分类" EnableEdit="true" ForceSelection="true" />
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
                <f:Grid ID="GridGoods" AnchorValue="100% -77" ShowBorder="false" ShowHeader="false" AllowCellEditing="true" ClicksToEdit="1" EnableTextSelection="true"
                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" IsDatabasePaging="true" EnableColumnLines="true"
                    DataKeyNames="GDSEQ,DEPTID,DHXS,DAYSL,ZDKC,ZGKC,ISAUTO" AllowPaging="true" PageSize="100" OnPageIndexChange="GridGoods_PageIndexChange"
                    EnableHeaderMenu="true" SortField="GDSEQ" OnSort="GridGoods_Sort" SortDirection="ASC" AllowSorting="true">
                    <Columns>
                        <f:RowNumberField Width="32px" TextAlign="Center" runat="server"></f:RowNumberField>
                        <f:BoundField Width="120px" DataField="DEPTNAME" HeaderText="库房" SortField="DEPTNAME" />
                        <f:BoundField Width="90px" DataField="TYPENAME" HeaderText="商品大类" TextAlign="Center" SortField="TYPENAME" />
                        <f:BoundField Width="160px" DataField="CATNAME" HeaderText="商品分类" TextAlign="Center" SortField="CATNAME" />
                        <f:BoundField Width="90px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" SortField="GDSEQ" />
                        <f:BoundField MinWidth="120px" DataField="GDNAME" HeaderText="商品名称" ExpandUnusedSpace="true" SortField="GDNAME" />
                        <f:BoundField Width="120px" DataField="GDSPEC" HeaderText="规格·容量" TextAlign="Center" SortField="GDSPEC" />
                        <f:BoundField Width="60px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" SortField="UNITNAME" />
                        <f:BoundField Width="90px" DataField="UNITNAME_ORDER" HeaderText="订货单位" TextAlign="Center" SortField="UNITNAME_ORDER" />
                        <%--<f:BoundField Width="70px" DataField="CATID" HeaderText="订货包装" TextAlign="Center" />--%>
                        <%--<f:CheckBoxField Width="60px" DataField="ISAUTO" ColumnID="cbxISAUTO" TextAlign="Center" RenderAsStaticField="false" HeaderText="自动订货"/>--%>
                        <%--<f:RenderField Width="70px" ColumnID="ISAUTO" DataField="ISAUTO" FieldType="Boolean" EnableFStateCompress="true"

                            HeaderText="自动订货" TextAlign="Center">
                            <Editor>
                                <f:Checkbox ID="ISAUTO" runat="server" />
                            </Editor>
                        </f:RenderField>--%>

                        <f:RenderCheckField Width="90px" ColumnID="ISAUTO" DataField="ISAUTO" HeaderText="自动订货<font color='red'>*</font>" TextAlign="Center" SortField="ISAUTO" />
                        <f:RenderField Width="90px" ColumnID="DHXS" DataField="DHXS" FieldType="Int"
                            HeaderText="订货系数<font color='red'>*</font>" TextAlign="Center" SortField="DHXS">
                            <Editor>
                                <f:NumberBox ID="nbxDHXS" runat="server" DecimalPrecision="2" MaxValue="100" MinValue="0" />
                            </Editor>
                        </f:RenderField>
                        <f:RenderField Width="90px" ColumnID="DAYSL" DataField="DAYSL" FieldType="Int"
                            HeaderText="备货天数<font color='red'>*</font>" TextAlign="Center" SortField="DAYSL">
                            <Editor>
                                <f:NumberBox ID="nbxDAYSL" runat="server" NoDecimal="true" MaxValue="365" MinValue="0" />
                            </Editor>
                        </f:RenderField>
                        <f:RenderField Width="90px" ColumnID="ZDKC" DataField="ZDKC" FieldType="Int"
                            HeaderText="最低库存<font color='red'>*</font>" TextAlign="Center" SortField="ZDKC">
                            <Editor>
                                <f:NumberBox ID="nbxZDKC" runat="server" NoDecimal="true" MaxValue="100000" MinValue="0" />
                            </Editor>
                        </f:RenderField>
                        <f:RenderField Width="90px" ColumnID="ZGKC" DataField="ZGKC" FieldType="Int"
                            HeaderText="最高库存<font color='red'>*</font>" TextAlign="Center" SortField="ZGKC">
                            <Editor>
                                <f:NumberBox ID="nbxZGKC" runat="server" NoDecimal="true" MaxValue="100000" MinValue="0" />
                            </Editor>
                        </f:RenderField>
                        <f:BoundField Width="90px" DataField="KCSL" HeaderText="当前库存" TextAlign="Center" SortField="KCSL" />

                        <%--                        <f:RenderField Width="70px" ColumnID="ISAUTO" DataField="ISAUTO" FieldType="Int" Hidden="true"
                            HeaderText="最高库存" TextAlign="Center">
                            <Editor>
                                <f:TextBox ID="tbxISAUTO" runat="server"/>
                            </Editor>
                        </f:RenderField>--%>
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
</body>
</html>