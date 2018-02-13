<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StorageStrategy.aspx.cs" Inherits="ERPProject.ERPDictionary.StorageStrategy" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>订货群组管理</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="Panel1"
            runat="server" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
            ShowHeader="false">
            <Toolbars>
                <f:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                        <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="备注：供应商查询主界面！" runat="server" />
                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                        <%--<f:Button ID="btnPrint" Icon="Printer" Text="打 印" DisableControlBeforePostBack="true" runat="server" />
                        <f:ToolbarSeparator runat="server" />--%>
                        <f:Button ID="btnExport" Icon="PageExcel" Text="导 出" DisableControlBeforePostBack="false" ConfirmText="是否确认导出供应商资料信息?" runat="server" EnableAjax="false" OnClick="btnExport_Click" EnableDefaultState="false" />
                        <f:ToolbarSeparator runat="server" />
                        <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" DisableControlBeforePostBack="true" runat="server" OnClick="btSearch_Click" EnableDefaultState="false" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="5px 10px 0px 10px"
                    ShowHeader="False" LabelWidth="80px" runat="server">
                    <Rows>
                        <f:FormRow ColumnWidths="50% 25% 25%">
                            <Items>
                                <f:DropDownList ID="ddlSUPID" runat="server" Label="厂家信息" EnableEdit="true" ForceSelection="true"></f:DropDownList>
                                <f:DropDownList ID="ddlISSUPID" runat="server" Label="供应标记" EnableEdit="true">
                                    <f:ListItem Text="--请选择--" Value="" Selected="true" />
                                    <f:ListItem Text="[Y]是" Value="Y" />
                                    <f:ListItem Text="[N]否" Value="N" />
                                </f:DropDownList>
                                 <f:DropDownList ID="ddlPRO" runat="server" Label="生产标志" EnableEdit="true">
                                    <f:ListItem Text="--请选择--" Value="" Selected="true" />
                                    <f:ListItem Text="[Y]是" Value="Y" />
                                    <f:ListItem Text="[N]否" Value="N" />
                                </f:DropDownList>
                                 <f:DropDownList ID="ddlPASSID" runat="server" Label="配送标志" EnableEdit="true">
                                    <f:ListItem Text="--请选择--" Value="" />
                                    <f:ListItem Text="[Y]是" Value="Y" />
                                    <f:ListItem Text="[N]否" Value="N" />
                                </f:DropDownList>
                            </Items>
                        </f:FormRow>
                       

                    </Rows>
                </f:Form>
                <f:Grid ID="GridGoods" AnchorValue="100% -35" ShowBorder="false" ShowHeader="false"
                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableTextSelection="true"
                    PageSize="70" DataKeyNames="SUPNAME" IsDatabasePaging="true" AllowPaging="true" OnPageIndexChange="GridGoods_PageIndexChange" EnableColumnLines="true">
                    <Columns>
                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                         <f:BoundField Width="105px" DataField="SUPID" HeaderText="供应商编号" TextAlign="Center" />
                        <f:BoundField Width="105px" DataField="SUPNAME" HeaderText="供应商" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="SUPENAME" HeaderText="英文名称" TextAlign="Center" />
                        <f:BoundField Width="60px" DataField="SUPSIMNAME" HeaderText="简称" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="SUPZJM" HeaderText="供应商助记码" />
                        <f:BoundField Width="90px" DataField="SUPCAT" HeaderText="供应商分类" Hidden="true" />
                        <f:BoundField Width="60px" DataField="REGID" HeaderText="地区" TextAlign="Center" />
                        <f:BoundField Width="130px" DataField="YYZZNO" HeaderText="营业执照" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="LOGINRQ" HeaderText="注册日期" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="LOGINLABEL" HeaderText="注册商标" TextAlign="Center" />
                        <f:BoundField Width="120px" DataField="LOGINFUND" HeaderText="注册资本" />
                        <f:BoundField Width="80px" DataField="JYFW" HeaderText="经营范围" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="BANK" HeaderText="开户银行" TextAlign="Right" />
                        <f:BoundField Width="90px" DataField="ACCNTNO" HeaderText="银行账号" TextAlign="Right" />
                        <f:BoundField Width="160px" DataField="LOGINADDR" HeaderText="注册地址" TextAlign="Left" />
                        <f:BoundField Width="80px" DataField="LEADER" HeaderText="法人代表" TextAlign="Center" />
                        <f:BoundField Width="140px" DataField="LEADERIDCARD" HeaderText="法人代表身份证" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="TEL" HeaderText="公司电话" TextAlign="Center" />
                        <f:BoundField Width="180px" DataField="FAX" HeaderText="公司传真" TextAlign="Center" />
                        <f:BoundField Width="120px" DataField="TELSERVICE" HeaderText="服务电话" />
                        <f:BoundField Width="120px" DataField="ZIP" HeaderText="邮政编码" />
                        <f:BoundField Width="105px" DataField="EMAIL" HeaderText="E-MAIL" TextAlign="Center" />
                        <f:BoundField Width="190px" DataField="UPTTIME" HeaderText="最后修改时间" TextAlign="Left" />
                         <f:BoundField Width="105px" DataField="ISSUPPLIER" HeaderText="是否是供应商" TextAlign="Center" />
                         <f:BoundField Width="105px" DataField="ISPRODUCER" HeaderText="是否是生产商" TextAlign="Center" />
                         <f:BoundField Width="105px" DataField="ISPSS" HeaderText="是否是配送商" TextAlign="Center" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
</body>
</html>
