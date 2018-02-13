﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Departments.aspx.cs" Inherits="ERPProject.ERPDictionary.Departments" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>科室部门信息管理</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="Panel1"
            runat="server" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="True" BodyPadding="3px" Layout="Anchor"
            ShowHeader="false">
            <Toolbars>
                <f:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                        <f:Button ID="btnAudit" Icon="ArrowRotateClockwise" Text="清 空" EnablePostBack="true" runat="server" />
                        <f:ToolbarSeparator runat="server" />
                        <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" runat="server" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Panel ID="PanelHeader" ShowBorder="false" BodyPadding="0px" Layout="HBox" Height="200px" ShowHeader="False" runat="server">
                    <Items>
                        <f:Panel ID="Panel3" Title="面板1" BoxFlex="3" runat="server" BodyPadding="0px" ShowBorder="false" ShowHeader="false" Layout="Anchor">
                            <Items>
                                <f:GroupPanel ID="GroupPanel1" runat="server" Height="65px" Title="查询条件">
                                    <Items>
                                        <f:Form ID="FormCond" ShowBorder="false" BoxFlex="1" BodyPadding="10px"
                                            ShowHeader="False" runat="server" LabelWidth="65px">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TriggerBox ID="trbSearch" Label="查询信息" TriggerIcon="Search" runat="server"></f:TriggerBox>
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:GroupPanel>
                                <f:GroupPanel ID="GroupPanel2" runat="server" AnchorValue="100% -65" Title="部门编辑">
                                    <Items>
                                        <f:Form ID="FormMain" ShowBorder="false" AutoScroll="false" BodyPadding="10px" ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TextBox ID="tbxDeptCode" runat="server" Label="科室编码" EmptyText="设定后不可更改" Required="true"
                                                            ShowRedStar="true">
                                                        </f:TextBox>
                                                        <f:DropDownList ID="ddlDeptType" runat="server" Label="部门分类" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true">
                                                            <f:ListItem Text="管理部门" Value="1" />
                                                            <f:ListItem Text="诊疗部门" Value="2" />
                                                            <f:ListItem Text="事物部门" Value="3" />
                                                        </f:DropDownList>
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TextBox ID="tbxDeptName" runat="server" Label="科室名称">
                                                        </f:TextBox>
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TextBox ID="tbxABBREVIATION" runat="server" Label="简称编码">
                                                        </f:TextBox>
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:GroupPanel>
                            </Items>
                        </f:Panel>
                        <f:GroupPanel ID="GroupPanel3" runat="server" BoxFlex="2" Title="信息列表" Layout="Fit">
                            <Items>
                                <f:Grid ID="GridDept" runat="server" ShowBorder="true" ShowHeader="false" BoxFlex="1" AutoScroll="true" Height="176px">
                                    <Columns>
                                        <f:BoundField Width="100px" DataField="CODE" HeaderText="部门编码" />
                                        <f:BoundField Width="150px" DataField="NAME" HeaderText="部门名称" ExpandUnusedSpace="true" />
                                        <f:BoundField Width="130px" DataField="Month1" HeaderText="商品分类" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:GroupPanel>
                    </Items>
                </f:Panel>
                <f:GroupPanel ID="GroupPanel4" runat="server" AnchorValue="100% -200" Layout="HBox" Title="部门设定" Height="450px">
                    <Items>
                        <f:Panel ID="Panel2" BoxFlex="1" runat="server" BodyPadding="5px" ShowBorder="false" ShowHeader="false">
                            <Items>
                                <f:Form ID="Form2" AnchorValue="100% 27%" ShowBorder="false" AutoScroll="false" BodyPadding="10px" ShowHeader="False" LabelWidth="80px" runat="server">
                                    <Rows>
                                        <f:FormRow>
                                            <Items>
                                                <f:DropDownList ID="ddlGoodsType" runat="server" Label="商品分类" Required="true" ShowRedStar="true">
                                                </f:DropDownList>
                                                <f:DropDownList ID="DropDownList4" runat="server" Label="部门类型" Required="true" ShowRedStar="true">
                                                    <f:ListItem Text="普通科室" Value="0" />
                                                    <f:ListItem Text="仓库库房" Value="1" />
                                                </f:DropDownList>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:DatePicker ID="DatePicker1" runat="server" Label="启用日期" Required="true" ShowRedStar="true">
                                                </f:DatePicker>
                                                <f:DropDownList ID="DropDownList5" runat="server" Label="订货权限">
                                                    <f:ListItem Text="无" Value="0" />
                                                    <f:ListItem Text="有" Value="1" />
                                                </f:DropDownList>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:DatePicker ID="DatePicker2" runat="server" Label="有效日期" Required="true" ShowRedStar="true">
                                                </f:DatePicker>
                                                <f:DropDownList ID="DropDownList2" runat="server" Label="订货确定">
                                                    <f:ListItem Text="可以" Value="0" />
                                                    <f:ListItem Text="不可以" Value="1" />
                                                </f:DropDownList>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:TextBox ID="TextBox4" runat="server" Label="日均用量">
                                                </f:TextBox>
                                                <f:DropDownList ID="DropDownList3" runat="server" Label="复核权限" EnableEdit="true" ForceSelection="true">
                                                    <f:ListItem Text="无" Value="0" />
                                                    <f:ListItem Text="有" Value="1" />
                                                </f:DropDownList>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:TextBox ID="TextBox1" runat="server" Label="备用编码">
                                                </f:TextBox>
                                                <f:DropDownList ID="DropDownList6" runat="server" Label="受理权限" EnableEdit="true" ForceSelection="true">
                                                    <f:ListItem Text="可以受理" Value="0" />
                                                    <f:ListItem Text="不可受理" Value="1" />
                                                </f:DropDownList>
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                            </Items>
                        </f:Panel>
                        <f:Grid ID="Grid1" runat="server" ShowBorder="true" ShowHeader="false" BoxFlex="1" Height="180px">
                            <Columns>
                                <f:BoundField Width="100px" DataField="Month1" HeaderText="开始日期" />
                                <f:BoundField Width="100px" DataField="Month2" HeaderText="结束日期" />
                                <f:BoundField Width="80px" DataField="Month3" HeaderText="库存管理" />
                                <f:BoundField Width="120px" DataField="Month4" HeaderText="主管部门" ExpandUnusedSpace="true" />
                            </Columns>
                        </f:Grid>
                    </Items>
                </f:GroupPanel>
            </Items>
        </f:Panel>
    </form>
</body>
</html>
