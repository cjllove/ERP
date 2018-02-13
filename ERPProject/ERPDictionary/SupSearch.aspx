<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SupSearch.aspx.cs" Inherits="ERPProject.ERPDictionary.SupSearch" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>供应商进销存汇总</title>
    <style type="text/css">
        .x-grid-row-summary .x-grid-cell-inner {
            font-weight: bold;
            color: red;
        }

        .x-grid-row-summary .x-grid-cell {
            background-color: #fff !important;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="False" BodyPadding="0px" Layout="Anchor"
            ShowHeader="false">
            <Items>
                <f:Panel ID="PanelBody" ShowBorder="false" BodyPadding="0px"
                    Layout="Anchor" ShowHeader="False" runat="server">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar1" runat="server">
                            <Items>
                                <f:ToolbarText ID="ToolbarText" runat="server" Text="操作信息：分类查询进销存汇总信息！"></f:ToolbarText>
                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                <f:Button ID="btnAudit" Icon="ArrowRotateClockwise" Text="清 空" OnClick="btClear_Click" EnablePostBack="true" runat="server" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" OnClick="btSearch_Click" runat="server" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="btnExp" runat="server" CssStyle="margin-left: 10px;" Icon="DatabaseGo" EnableAjax="false"
                                    OnClick="btExport_Click" EnablePostBack="true" Text="导出" ConfirmText="是否确认导出此进销存信息?" />
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Items>
                        <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="10px 0px 10px 10px"
                            ShowHeader="False" LabelWidth="80px" runat="server">
                            <Rows>
                                <f:FormRow>
                                    <Items>
                                        <f:DatePicker ID="dpkDATE1" runat="server" Label="操作日期" Required="true" ShowRedStar="true"></f:DatePicker>
                                        <f:DatePicker ID="dpkDATE2" runat="server" Label="　至" LabelSeparator="" Required="true" ShowRedStar="true" LabelWidth="40px"></f:DatePicker>
                                        <f:DropDownList ID="ddlDEPTID" runat="server" Label="科室" EnableEdit="true" ForceSelection="true"/>
                                        <f:DropDownList ID="ddlSUPID" runat="server" Label="供应商" EnableEdit="true" ForceSelection="true"/>
                                    </Items>
                                </f:FormRow>
                            </Rows>
                        </f:Form>
                    </Items>
                </f:Panel>
                <f:Grid ID="GridGoods" AnchorValue="100% -73px" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;"
                    AllowSorting="false" AutoScroll="true" runat="server" DataKeyNames="SUPID" IsDatabasePaging="true" AllowPaging="true"
                    PageSize="100" OnPageIndexChange="GridGoods_PageIndexChange"
                    EnableSummary="true" SummaryPosition="Bottom" EnableCollapse="true">
                    <Columns>
                        <f:BoundField Width="0px" DataField="SUPID" HeaderText="供应商编码" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="DEPTNAME" HeaderText="科室" TextAlign="Center" />
                        <f:BoundField Width="110px" DataField="SUPNAME" HeaderText="供应商名称" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="KCADD" HeaderText="操作" TextAlign="Center" />
                        <f:BoundField Width="110px" DataField="SL" ColumnID="SL" HeaderText="合计数量" TextAlign="Center" />
                        <f:BoundField Width="130px" DataField="LSJE" ColumnID="LSJE" HeaderText="合计零售金额" TextAlign="Center" />
                        <f:BoundField Width="130px" DataField="HSJE" ColumnID="HSJE" HeaderText="合计含税金额" TextAlign="Center" />
                        <f:BoundField Width="130px" DataField="BHSJE" ColumnID="BHSJE" HeaderText="合计不含税金额" TextAlign="Center" />
                    </Columns>
                </f:Grid>
                <f:HiddenField runat="server" ID="hfGrid1Summary"></f:HiddenField>
            </Items>
        </f:Panel>
    </form>
</body>
</html>
