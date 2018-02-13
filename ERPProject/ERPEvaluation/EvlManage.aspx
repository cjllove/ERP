﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EvlManage.aspx.cs" Inherits="ERPProject.ERPEvalution.EvlManage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>科室评价设定</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="PanelDetail" EnableFormChangeConfirm="true"
            runat="server" />
        <f:Panel ID="PanelDetail" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor" ShowHeader="False">
            <Items>
                <f:Grid ID="GridGoods" AnchorValue="100% 0px" ShowBorder="false" ShowHeader="false" AllowSorting="false"
                    AutoScroll="true" runat="server" DataKeyNames="SEQNO" EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridGoods_RowDoubleClick" EnableColumnLines="true">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar1" runat="server">
                            <Items>
                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                <f:Button ID="btn_Auto" runat="server" EnablePostBack="true" EnableDefaultState="false" Icon="BasketEdit" Text="自动生成" ConfirmText="是否自动生成本月科室评价单？" OnClick="btn_Auto_Click"></f:Button>
                                <f:Button ID="btnNew" Icon="Add" Text="新 增" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnNew_Click">
                                </f:Button>
                                <f:ToolbarSeparator runat="server"></f:ToolbarSeparator>
                                <f:Button ID="btnDelete" Icon="Delete" Text="删 除" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否删除选中信息?" runat="server" OnClick="btnDelete_Click" Enabled="false">
                                </f:Button>
                                <f:ToolbarSeparator runat="server"></f:ToolbarSeparator>
                                <f:Button ID="btnAuditBatch" Icon="UserTick" Text="生 效" EnableDefaultState="false" ConfirmText="是否确认生效此评价项目名称？" EnablePostBack="true" runat="server" OnClick="btnAuditBatch_Click" Enabled="false" />
                                <f:Button ID="btnCancel" Icon="ArrowUndo" Text="回 撤" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否确认回撤此评价项目名称？" OnClick="btnCancel_Click" Enabled="false" />
                                <f:ToolbarSeparator runat="server"></f:ToolbarSeparator>
                                <f:Button ID="btnSearch" Icon="Magnifier" Text="查 询" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnSearch_Click" />
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Columns>
                        <f:RowNumberField Width="30" TextAlign="Center" />
                        <f:BoundField Width="105px" DataField="SEQNO" HeaderText="系号" Hidden="true" />
                        <f:BoundField Width="180px" DataField="PRONAME" HeaderText="项目名称" />
                        <f:BoundField Width="100px" DataField="FLAGNAME" HeaderText="状态" />
                        <f:BoundField Width="80px" DataField="QZNAME" HeaderText="权重" />
                        <f:BoundField Width="80px" DataField="CJRNAME" HeaderText="创建人" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="CJRQ" HeaderText="创建日期" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="90px" DataField="XGRNAME" HeaderText="最后修改人" />
                        <f:BoundField Width="100px" DataField="XGRQ" HeaderText="最后修改日期" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="180px" DataField="MEMO" HeaderText="备注" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
    <f:Window ID="WindowAdd" Title="评价项目" Hidden="true" EnableIFrame="false" runat="server"
        EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="450px" Height="230px">
        <Items>
            <f:Form ID="Form2" ShowBorder="false" AutoScroll="false" BodyPadding="5px 10px" ShowHeader="False" LabelWidth="75px" runat="server">
                <Rows>
                    <f:FormRow>
                        <Items>
                            <f:TextBox runat="server" ID="tbxPRONAME" Label="项目名称" Required="true" EmptyText="评价项目名称" MaxLength="20" ShowRedStar="true"></f:TextBox>
                            <f:DropDownList ID="ddlFLAG" runat="server" Label="状态" Required="true" Enabled="false">
                                <f:ListItem Text="生效" Value="Y" />
                                <f:ListItem Text="未生效" Value="N" Selected="true" />
                            </f:DropDownList>
                        </Items>
                    </f:FormRow>
                    <f:FormRow ColumnWidths="50% 45% 5%">
                        <Items>
                            <f:TextBox ID="tbxMemo" runat="server" Label="详细说明" MaxLength="80" />
                            <f:NumberBox ID="nbxQZ" runat="server" Label="权重" MaxValue="100" MinValue="1" EmptyText="权重占比" NoDecimal="true" Text="20" Required="true" ShowRedStar="true"></f:NumberBox>
                            <f:Label runat="server" Text="%"></f:Label>
                        </Items>
                    </f:FormRow>
                    <f:FormRow>
                        <Items>
                            <f:DropDownList ID="ddlCJR" runat="server" Label="创建人" Required="true" Enabled="false"></f:DropDownList>
                            <f:DatePicker ID="dpkCJRQ" runat="server" Label="创建日期" Required="true" Enabled="false"></f:DatePicker>
                        </Items>
                    </f:FormRow>
                    <f:FormRow>
                        <Items>
                            <f:DropDownList ID="ddlXGR" runat="server" LabelWidth="90px" Label="最后修改人" Required="true" Enabled="false"></f:DropDownList>
                            <f:DatePicker ID="dpkXGRQ" runat="server" LabelWidth="100px" Label="最后修改日期" Required="true" Enabled="false"></f:DatePicker>
                        </Items>
                    </f:FormRow>
                    <f:FormRow Hidden="true">
                        <Items>
                            <f:TextBox ID="tbxSEQNO" runat="server" Hidden="true" />
                        </Items>
                    </f:FormRow>
                </Rows>
            </f:Form>
        </Items>
        <Toolbars>
            <f:Toolbar ID="Toolbar4" runat="server" Position="Bottom" ToolbarAlign="Right">
                <Items>
                    <f:Button ID="btnClose" Text="关 闭" EnableDefaultState="false" Icon="SystemClose" runat="server" OnClick="btnClose_Click"></f:Button>
                    <f:Button ID="btnSubmit" Text="保 存" EnableDefaultState="false" Icon="SystemSave" ValidateForms="Form2" runat="server" OnClick="btnSubmit_Click"></f:Button>
                </Items>
            </f:Toolbar>
        </Toolbars>
    </f:Window>
</body>
</html>
