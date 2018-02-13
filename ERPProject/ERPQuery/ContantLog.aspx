﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ContantLog.aspx.cs" Inherits="ERPProject.ERPQuery.ContantLog" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>商品代替关系</title>
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
                        <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="备注：商品替代查询主界面！" runat="server" />
                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                        <f:Button ID="btnPrint" Icon="Printer" Text="打 印" DisableControlBeforePostBack="true" runat="server" Hidden="true" />
                        <f:ToolbarSeparator runat="server" />
                        <f:Button ID="btnExport" Icon="PageExcel" Text="导 出" Enabled="false" DisableControlBeforePostBack="true" ConfirmText="是否确认导出此商品资料信息?" runat="server" EnableAjax="false" OnClick="btnExport_Click" />
                        <f:ToolbarSeparator runat="server" />
                        <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" DisableControlBeforePostBack="true" ValidateForms="FormUser" runat="server" OnClick="btSearch_Click" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="5px 10px 0px 10px"
                    ShowHeader="False" LabelWidth="80px" runat="server">
                    <Rows>
                        <f:FormRow>
                            <Items>
                                <f:TextBox ID="tbxSEQNO" runat="server" Label="单据编号" EmptyText="" MaxLength="20" />
                                <f:DropDownList ID="ddlDEPTID" runat="server" Label="科室" EnableEdit="true" ForceSelection="true" />
                                <f:TextBox ID="tbxGDSEQ_OLD" runat="server" Label="旧商品编码" EmptyText="商品编码或ERP编码或HIS编码或助记码" MaxLength="20" />
                                <f:TextBox ID="tbxGDSEQ" runat="server" Label="商品编码" EmptyText="商品编码或ERP编码或HIS编码或助记码" MaxLength="20" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:DropDownList ID="ddlSTR1" runat="server" Label="标志">
                                    <f:ListItem Text="--请选择--" Value="" Selected="true" />
                                    <f:ListItem Text="生效" Value="生效" />
                                    <f:ListItem Text="作废" Value="作废" Selected="true" />
                                </f:DropDownList>
                                <f:DropDownList ID="ddlTYPE" runat="server" Label="类型"></f:DropDownList>
                                <f:DatePicker ID="dpkbegin" runat="server" Label="开始日期" Required="true" ShowRedStar="true"></f:DatePicker>
                                <f:DatePicker ID="dpkend" runat="server" Label="结束日期" Required="true" ShowRedStar="true"></f:DatePicker>
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
                <f:Grid ID="GridGoods" AnchorValue="100% -60" ShowBorder="false" ShowHeader="false"
                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableTextSelection="true"
                    PageSize="70" DataKeyNames="SEQNO,DEPTID" IsDatabasePaging="true" AllowPaging="true" OnPageIndexChange="GridGoods_PageIndexChange" EnableColumnLines="true">
                    <Columns>
                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                        <f:BoundField Width="150px" DataField="INSTIME" HeaderText="业务时间" TextAlign="Center" />
                        <f:BoundField Width="40px" DataField="STR1" HeaderText="标志" TextAlign="Center" />
                        <f:BoundField Width="130px" DataField="SEQNO" HeaderText="单据编号" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="TYPENAME" HeaderText="类型" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="DEPTID" HeaderText="科室" Hidden="true" />
                        <f:BoundField Width="140px" DataField="DEPTIDNAME" HeaderText="科室" />

                        <f:BoundField Width="105px" DataField="GDSEQ_OLD" HeaderText="旧商品编码" TextAlign="Center" />
                        <f:BoundField Width="190px" DataField="GDNAME_OLD" HeaderText="旧商品名称" />
                        <f:BoundField Width="90px" DataField="GDSPEC_OLD" HeaderText="旧规格" />
                        <f:BoundField Width="60px" DataField="UNITNAME_OLD" HeaderText="旧单位" TextAlign="Center" />
                        <f:BoundField Width="60px" DataField="HSJJ_OLD" HeaderText="旧价格" TextAlign="Center" />
                        <f:BoundField Width="70px" DataField="DSSL_OLD" HeaderText="旧定数数量" TextAlign="Center" />
                        <f:BoundField Width="70px" DataField="DSHL_OLD" HeaderText="旧定数含量" TextAlign="Center" />
                        <f:BoundField Width="160px" DataField="PRODUCER_OLDNAME" HeaderText="旧厂家" TextAlign="Left" />
                        <f:BoundField Width="160px" DataField="PZWH_OLD" HeaderText="旧注册证号" TextAlign="Left" />

                        <f:BoundField Width="105px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                        <f:BoundField Width="190px" DataField="GDNAME" HeaderText="商品名称" />
                        <f:BoundField Width="90px" DataField="GDSPEC" HeaderText="规格" />
                        <f:BoundField Width="60px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                        <f:BoundField Width="60px" DataField="HSJJ" HeaderText="价格" TextAlign="Center" />
                        <f:BoundField Width="70px" DataField="DSSL" HeaderText="定数数量" TextAlign="Center" />
                        <f:BoundField Width="70px" DataField="DSHL" HeaderText="定数含量" TextAlign="Center" />
                        <f:BoundField Width="160px" DataField="PRODUCERNAME" HeaderText="厂家" TextAlign="Left" />
                        <f:BoundField Width="160px" DataField="PZWH" HeaderText="注册证号" TextAlign="Left" />
                        <f:BoundField Width="160px" DataField="MEMO" HeaderText="备注" TextAlign="Left" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
</body>
</html>
