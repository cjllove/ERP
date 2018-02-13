﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DepartmentApplyAuto.aspx.cs" Inherits="ERPProject.ERPApply.DepartmentApplyAuto" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>耗材自动申领</title>
    <script src="../res/js/CreateControl.js" type="text/javascript"></script>
    <script src="../res/js/GRInstall.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" autosizepanelid="Panel1" runat="server" />
               <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
                            ShowHeader="false">
                   <Items>
                        <f:Panel ID="PanelBody" ShowBorder="false" BodyPadding="0px" Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar1" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="操作信息：勾选商品生成申领单" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />

                                                <f:Button ID="bntClear" Icon="Erase" Text="清 空" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="bntSearch" Icon="Magnifier" EnableDefaultState="false" Text="查 询" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnAut" Icon="Magnifier" EnableDefaultState="false" Text="生成申领单" EnablePostBack="true" runat="server" OnClick="btnAut_Click" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow ColumnWidths="25% 25% 25% 25%">
                                                    <Items>
                                                        <f:DropDownList ID="ddlDEPTIN" runat="server" Label="科室名称" EnableEdit="true" ForceSelection="true" />
														 <f:DropDownList ID="ddlISGZ" runat="server" Label="是否高值">
                                                            <f:ListItem Text="--请选择--" Value="" />
                                                            <f:ListItem Text="是" Value="Y" />
                                                            <f:ListItem Text="否" Value="N" />
                                                        </f:DropDownList>
                                                        <f:DatePicker ID="dpkLRRQ1" runat="server" Label="起始日期" Required="true" ShowRedStar="true" />
                                                        <f:DatePicker ID="dpkLRRQ2" runat="server" Label="终止日期" LabelSeparator="" Required="true" ShowRedStar="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow ColumnWidths="25% 25% 25% 25%">
                                                    <Items>                                                       
                                                        <f:TextBox ID="tbxGDSEQ" runat="server" Label="商品信息" MaxLength="15" EmptyText="输入商品信息查询" />
														<f:Label runat="server" />
																												<f:Label runat="server" />
														<f:Label runat="server" />

                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridGoods" AnchorValue="100% -115" ShowBorder="false" ShowHeader="false" EnableCheckBoxSelect="true" AllowCellEditing="true" ClicksToEdit="1"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" AllowPaging="true" IsDatabasePaging="true" OnPageIndexChange="GridGoods_PageIndexChange"
                                    DataKeyNames="SEQ,FLAG,SLSL,GDSEQ,GDNAME" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick" EnableTextSelection="true"
                                    EnableColumnLines="true" EnableMultiSelect="true" 
                                    AllowSorting="true" EnableHeaderMenu="true" SortField="DJRQ" OnSort="GridGoods_Sort" SortDirection="DESC" KeepCurrentSelection="true">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField Width="130px" DataField="SEQ" SortField="SEQ" HeaderText="编号" TextAlign="Center" Hidden="true"/>
                                        <f:BoundField Width="130px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编号" TextAlign="Center" />
                                        <f:BoundField DataField="FLAG" SortField="FLAG" Hidden="true" />
                                        <f:BoundField Width="160px" DataField="GDNAME" ColumnID="GDNAME" SortField="GDNAME" HeaderText="商品名称" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="GDSPEC" SortField="GDSPEC" HeaderText="商品规格" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="XHSL" SortField="XHSL" HeaderText="消耗数量" TextAlign="Center"/>
                                        <%--<f:BoundField Width="90px" DataField="SLSL" SortField="SLSL" HeaderText="申领数量" TextAlign="Center" />--%>
                                        <f:TemplateField Width="70px" ColumnID="SLSL" EnableLock="true" EnableHeaderMenu="false"
                                            HeaderText="申领数量" TextAlign="Center">
                                            <%--<Editor>
                                                <f:NumberBox ID="nbxSLSL" runat="server" Required="true" MinValue="0" MaxValue="99999999" DecimalPrecision="2" />
                                            </Editor>--%>
                                            <ItemTemplate>
                                                <asp:TextBox runat="server" Width="98%" ID="tbxNum" CssClass="number"
                                                    TabIndex='<%# Container.DataItemIndex + 100 %>' Text='<%# Eval("XHSL") %>' FieldType="Int"></asp:TextBox>
                                            </ItemTemplate>
                                        </f:TemplateField>
                                        <f:BoundField Width="70px" DataField="HSJJ" HeaderText="含税进价" TextAlign="Center"/>
                                        <f:BoundField Width="70px" DataField="DEPTID" ColumnID="DEPTID" SortField="DEPTID" HeaderText="科室" TextAlign="Center"/>
                                        <f:BoundField Width="90px" DataField="PRODUCER_CN" SortField="PRODUCER_CN" HeaderText="生产厂家" TextAlign="Center" />
                                        <f:BoundField Width="0px" DataField="DJRQ" SortField="DJRQ" HeaderText="登记日期" TextAlign="Center" Hidden="true"/>
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
            </form>
</body>
</html>
