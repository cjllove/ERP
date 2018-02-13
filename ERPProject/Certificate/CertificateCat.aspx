﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CertificateCat.aspx.cs" Inherits="ERPProject.Certificate.CertificateCat" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>证照类别管理</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="MainPanel"
            runat="server" />
        <f:Panel ID="MainPanel" runat="server" AutoScroll="false" BodyPadding="0px" Layout="Anchor" ShowBorder="false" ShowHeader="false">
            <Items>
                <f:Grid ID="GridSupplier" ShowBorder="false" ShowHeader="false" AnchorValue="100% -100px"
                    AutoScroll="true" EnableCheckBoxSelect="true" DataKeyNames="SUPID"
                    EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridSupplier_RowDoubleClick"
                    runat="server" OnPageIndexChange="Grid1_PageIndexChange" PageSize="50" IsDatabasePaging="true" AllowPaging="true">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar1" runat="server">
                            <Items>                                
                                <f:Label runat="server" Text="信息查询：" CssStyle="padding-left:10px; padding-right:10px;"></f:Label>
                                <f:DropDownList runat="server" ID="lstFLAG" Label="状态" ShowLabel="false" ForceSelection="true"></f:DropDownList>
                                <f:DropDownList ID="lstOBJUSER" Label="分类" runat="server" ShowLabel="false"/>
                                <f:TriggerBox ID="tgbSearch" Label="　信息查询" ShowLabel="false" TriggerIcon="Search" runat="server" OnTriggerClick="btnSearch_Click"></f:TriggerBox>
                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                <f:Button ID="btnNew" Text="新增" Icon="Add" runat="server" OnClick="btnNew_Click"></f:Button>
                                <f:Button ID="btnDelete" Text="删除" Icon="Delete" runat="server" ConfirmText="是否确认删除此信息?" OnClick="btnDelete_Click"></f:Button>
                                <f:Button ID="btnSave" Text="保存" Icon="Disk" runat="server" ValidateForms="FormProducer" OnClick="btnSave_Click"></f:Button>
                                <f:ToolbarSeparator runat="server"></f:ToolbarSeparator>
                                <f:Button ID="btnSearch" OnClick="btnSearch_Click" Icon="Magnifier" Text="查 询" runat="server"></f:Button>
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Columns>
                        <f:BoundField Width="130px" DataField="CODE" HeaderText="证照类别编码" />
                        <f:BoundField Width="130px" DataField="NAME"  HeaderText="证照类别名称" />
                        <f:BoundField Width="130px" DataField="FLAG" HeaderText="状态" />
                        <f:BoundField Width="90px" DataField="OBJUSER" HeaderText="分类" />
                        <f:BoundField Width="90px" DataField="ISNEED" HeaderText="是否必须" />
                        <f:BoundField Width="90px" DataField="ISDATE" HeaderText="是否有期限" />
                        <f:BoundField Width="90px" DataField="ISDUP" HeaderText="是否可以多份" />
                        <f:BoundField Width="230px" DataField="MEMO" HeaderText="备注" ExpandUnusedSpace="true"/>
                        <f:BoundField Width="60px" DataField="SORT" HeaderText="排序"/>
                    </Columns>
                </f:Grid>
                <f:Panel ID="PanelCond" ShowBorder="false" BodyPadding="0px" Height="100px"
                    ShowHeader="false" runat="server">
                    <Items>
                        <f:Form ID="FormProducer" ShowBorder="false" BodyPadding="10px 0px 10px 10px"
                            CssStyle="border-top: 1px solid #99bce8;" ShowHeader="False" runat="server" LabelWidth="80px">
                            <Rows>
                                <f:FormRow ColumnWidths="33% 33% 33%">
                                    <Items>
                                        <f:TextBox ID="tbxCODE" Label="类别编码" EmptyText="设定后不可更改" runat="server" Required="true" ShowRedStar="true" MaxLength="50" />
                                        <f:TextBox ID="tbxNAME" Label="类别名称" runat="server" Required="true" ShowRedStar="true" MaxLength="60" />
                                        <f:DropDownList ID="ddlOBJUSER" Label="分类" runat="server" Required="true" ShowRedStar="true"/>
                                    </Items>
                                </f:FormRow>
                                <f:FormRow ColumnWidths="33% 66%">
                                    <Items>
                                        <f:DropDownList runat="server" ID="ddlFLAG" Label="状态" ForceSelection="true">
                                        </f:DropDownList>
                                         <f:TextBox ID="tbxMEMO" Label="备注" runat="server" MaxLength="20" />
                                        <f:NumberBox ID="tbxSORT" Label="证照排序：" runat="server" EmptyText="排序，数字小的排在上面" NoDecimal="True" NoNegative="True" />
                                    </Items>
                                </f:FormRow>
                                <f:FormRow ColumnWidths="10% 10% 10%">
                                    <Items>
                                       
                                        <f:CheckBox ID="chkISDATE" Label="是否有期限" runat="server"  />
                                        <f:CheckBox runat="server" ID="chkISNEED" Label="是否必须">
                                        </f:CheckBox>
                                        <f:CheckBox ID="chkISDUP" Label="是否可多份" runat="server" />
                                    </Items>
                                </f:FormRow>
                                <f:FormRow Hidden="true">
                                    <Items>
                                        <f:HiddenField ID="hfdIsNew" runat="server" Text="Y" />
                                    </Items>
                                </f:FormRow>
                            </Rows>
                        </f:Form>
                    </Items>
                </f:Panel>
            </Items>
        </f:Panel>
    </form>
</body>
</html>