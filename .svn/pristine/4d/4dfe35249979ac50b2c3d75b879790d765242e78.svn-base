<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AgentInput.aspx.cs" Inherits="SPDProject.CertificateInput.AgentInput" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="MainPanel"
            runat="server" />
        <f:Panel ID="MainPanel" runat="server" AutoScroll="false" BodyPadding="0px" Layout="Anchor" ShowBorder="false" ShowHeader="false">
            <Items>
                <f:Grid ID="GridSupplier" ShowBorder="false" ShowHeader="false" AnchorValue="100% -130px" EnableRowDoubleClickEvent="true"
                    AutoScroll="true" EnableCheckBoxSelect="true" DataKeyNames="SUPID,SUPNAME" OnRowDoubleClick="GridSupplier_RowDoubleClick"
                    runat="server" OnPageIndexChange="GridSupplier_PageIndexChange" EnableMultiSelect="true"
                     PageSize="30" IsDatabasePaging="true" AllowPaging="true">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar1" runat="server">
                            <Items>
                                <f:TextBox ID="tbxSUPPNAME" Label="机构查询" LabelWidth="70px" runat="server" EmptyText="请按照机构编号或名称查询" />
                                <f:DropDownList ID="ddlFLAG" Hidden="true" runat="server" Label="状态" LabelWidth="45px" Width="127px">
                                    <f:ListItem Value="" Selected="true" Text="--请选择--" />
                                    <f:ListItem Value="N" Text="未审核" />
                                    <f:ListItem Value="Y" Text="审核通过" />
                                </f:DropDownList>
                                <f:Button ID="btnSearch" Icon="Magnifier" Text="查 询" runat="server" OnClick="btnSearch_Click" ></f:Button>
                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                <f:Button ID="btnNew" Text="新 增" Icon="Add" runat="server" OnClick="btnNew_Click"></f:Button>
                                <f:Button ID="btnDelete" Text="删 除" Icon="Delete" runat="server" ConfirmText="是否确认删除此信息?" OnClick="btnDelete_Click"></f:Button>
                                <f:Button ID="btnSave" Text="保 存" Icon="Disk" runat="server" ValidateForms="FormProducer" OnClick="btnSave_Click"></f:Button>
                                <f:Button ID="btnAudit" Icon="UserTick" Text="审 核" EnablePostBack="true" runat="server" OnClick="btnAudit_Click" />
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Columns>
                        <f:RowNumberField Width="40px" HeaderText="序号" TextAlign="center"></f:RowNumberField>
                        <f:BoundField Width="130px" DataField="SUPID" HeaderText="代理商编码" TextAlign="center" />
                        <f:BoundField Width="130px" DataField="SUPNAME" HeaderText="代理商名称" TextAlign="left" />
                        <f:BoundField Width="80px" DataField="FLAG" HeaderText="状态" TextAlign="center" />
                        <f:BoundField Width="90px" DataField="ISAGENT" HeaderText="机构" TextAlign="center" />
                        <f:BoundField Width="130px" DataField="TEL" HeaderText="公司电话" TextAlign="center" />
                        <f:BoundField Width="90px" DataField="FAX" HeaderText="公司传真" TextAlign="center" />
                        <f:BoundField Width="100px" DataField="ZZADDR" HeaderText="联系地址" TextAlign="left" />
                        <f:BoundField Width="80px" DataField="EMAIL" HeaderText="邮箱" TextAlign="center" />
                        <f:BoundField Width="230px" DataField="LOGINADDR" HeaderText="注册地址" TextAlign="left" />
                        <f:BoundField Width="80px" DataField="URL" HeaderText="网址" TextAlign="center" />
                        <f:BoundField Width="230px" DataField="LEADER" HeaderText="法人代表" ExpandUnusedSpace="true" TextAlign="center" />
                    </Columns>
                </f:Grid>
                <f:Panel ID="PanelCond" ShowBorder="false" BodyPadding="0px" Height="250px"
                    ShowHeader="false" runat="server">
                    <Items>
                        <f:Form ID="FormProducer" ShowBorder="false" BodyPadding="10px 0px 10px 10px"
                            CssStyle="border-top: 1px solid #99bce8;" ShowHeader="False" runat="server" LabelWidth="80px">
                            <Rows>
                                <f:FormRow ColumnWidths="33% 33% 33%">
                                    <Items>
                                        <f:TextBox ID="tbxSUPID" Label="代理商编码" LabelWidth="100px"  EmptyText="为空则自动生成" runat="server" MaxLength="50" />
                                        <f:TextBox ID="tbxSUPNAME" LabelWidth="90px" Label="代理商名称" runat="server" Required="true" ShowRedStar="true" MaxLength="60" />
                                        <f:TextBox ID="tbxTEL" Label="公司电话" ShowRedStar="true" Required="true" runat="server"></f:TextBox>
                                    </Items>
                                </f:FormRow>
                                <f:FormRow ColumnWidths="33% 33% 33%">
                                    <Items>
                                        <f:TextBox ID="tbxFAX" LabelWidth="100px" Label="公司传真" runat="server"></f:TextBox>
                                        <f:TextBox ID="tbxZZADDR" LabelWidth="90px"  Label="联系地址" runat="server"></f:TextBox>
                                        <f:TextBox ID="tbxLEADER" Label="法人代表" runat="server" ShowRedStar="true" Required="true"></f:TextBox>
                                    </Items>
                                </f:FormRow>
                                <f:FormRow>
                                    <Items>
                                        <f:TextBox ID="tbxLOGINADDR" LabelWidth="100px" Label="注册地址" runat="server"></f:TextBox>
                                        <f:TextBox ID="tbxURL" LabelWidth="90px" Label="网址" runat="server"></f:TextBox>
                                        <f:TextBox ID="tbxEMAIL" Label="邮箱" runat="server"></f:TextBox>
                                    </Items>
                                </f:FormRow>
                            </Rows>
                        </f:Form>
                    </Items>
                </f:Panel>
            </Items>
        </f:Panel>
        <f:HiddenField ID="hfdIsNew" runat="server" Text="Y" />
        <f:HiddenField ID="hfdsavecount" runat="server"></f:HiddenField>
    </form>
</body>
</html>
