<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoodsZKYH.aspx.cs" Inherits="ERPProject.ERPQuery.GoodsZKYH" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>在库养护管理</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
            ShowHeader="false">
            <Items>
                <f:Panel ID="PanelBody" ShowBorder="false" BodyPadding="0px"
                    Layout="Anchor" ShowHeader="False" runat="server">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar1" runat="server">
                            <Items>
                                <%--<f:CheckBox ID="chkISDG" runat="server" Text="包含代管商品" CssStyle="margin-left:10px;"></f:CheckBox>--%>
                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                <f:Button ID="btnClear" Icon="ArrowRotateClockwise" Text="清 空" OnClick="btnClear_Click" EnablePostBack="true" runat="server" EnableDefaultState="false" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="btnPrint" Icon="Printer" Text="打 印" DisableControlBeforePostBack="true" runat="server" Hidden="true" EnableDefaultState="false" />
                                <%--<f:ToolbarSeparator runat="server" />--%>
                                <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" OnClick="btnExport_Click" DisableControlBeforePostBack="false" EnableDefaultState="false"
                                    EnablePostBack="true" EnableAjax="false" EnableAjaxLoading="true" AjaxLoadingType="Mask" runat="server" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" OnClick="btSearch_Click" runat="server" EnableDefaultState="false" />
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Items>
                        <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="5px"
                            ShowHeader="False" LabelWidth="80px" runat="server">
                            <Rows>
                                <f:FormRow>
                                    <Items>
                                        <%--<f:TriggerBox ID="tbxGDSEQ" runat="server" Label="商品" EmptyText="可输入编码、名称或助记码" TriggerIcon="Search"></f:TriggerBox>--%> 
                                        <f:TextBox ID="tbxGDSEQ" runat="server" Label="商品" EmptyText="可输入编码、名称或助记码"></f:TextBox>
                                        <f:DropDownList ID="lstLRY" runat="server" Label="录入员" Required="true" EnableEdit="true" ForceSelection="true" />
                                        <f:DatePicker ID="dpkDATE1" runat="server" Label="日期" Required="true" />
                                        <f:DatePicker ID="dpkDATE2" runat="server" Label="　　至" LabelSeparator="" Required="true" />
                                    </Items>
                                </f:FormRow>
                            </Rows>
                            <Rows>
                                <f:FormRow>
                                    <Items>
                                        <f:TextBox ID="tbxBILLNO" runat="server" Label="养护单号" EmptyText="请输入单据编号"></f:TextBox>
                                        <f:DropDownList ID="ddlYHTYPE" runat="server" Label="养护标准" EnableEdit="true" ForceSelection="true" />
                                        <f:DropDownList ID="ddlDEPTID" runat="server" Label="养护仓库" EnableEdit="true" ForceSelection="true" />
                                        <f:DropDownList ID="ddlISGZ" runat="server" Label="是否高值">
                                            <f:ListItem Text="--请选择--" Value="" /> 
                                            <f:ListItem Text="是" Value="Y" /> 
                                            <f:ListItem Text="否" Value="N" /> 
                                        </f:DropDownList>

                                    </Items>
                                </f:FormRow>
                            </Rows>
                        </f:Form>
                    </Items>
                </f:Panel>
                <f:Grid ID="GridGoods" AnchorValue="100% -130" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;"
                    PageSize="100" IsDatabasePaging="true" AllowPaging="false" OnPageIndexChange="GridGoods_PageIndexChange"
                    EnableColumnLines="true" EnableTextSelection="true"
                    AllowSorting="true" AutoScroll="true" runat="server" DataKeyNames="GDSEQ" OnSort="GridGoods_Sort" SortField="INSTIME" SortDirection="DESC">
                    <Columns>
                       <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                <f:BoundField Width="120px" DataField="SEQNO" SortField="SEQNO" HeaderText="养护单号" TextAlign="Center" />
                                <f:BoundField Width="70px" DataField="YHTYPENAME" SortField="YHTYPENAME" HeaderText="养护标准" TextAlign="Center" />                        
                                <f:BoundField Width="120px" DataField="REASON" SortField="REASON" HeaderText="原因说明" TextAlign="Center" />
                                <f:BoundField Width="70px" DataField="YHY" SortField="YHY" HeaderText="养护人" TextAlign="Center" />
                                <f:BoundField Width="150px" DataField="DEPTNAME" SortField="DEPTNAME" HeaderText="养护仓库" />
                                <f:BoundField Width="110px" DataField="YHRQ" SortField="YHRQ" HeaderText="养护日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                <f:BoundField Width="70px" DataField="LRY" SortField="LRY" HeaderText="录入员" TextAlign="Center" />
                                <f:BoundField Width="110px" DataField="LRRQ" SortField="LRRQ" HeaderText="录入日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                <f:BoundField Width="70px" DataField="SHR" SortField="SHR" HeaderText="审核人" TextAlign="Center" />
                                <f:BoundField Width="110px" DataField="SHRQ" SortField="SHRQ" HeaderText="审核日期" TextAlign="Center" />

                                <f:BoundField Width="110px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                                <f:BoundField Width="100px" DataField="GDID" HeaderText="商品条码" TextAlign="Center" Hidden="true" />
                                <f:BoundField Width="130px" DataField="GDNAME" HeaderText="商品名称" />
                                <f:BoundField Width="120px" DataField="GDSPEC" HeaderText="商品规格" />
                                <f:BoundField Width="50px" DataField="UNIT" HeaderText="单位" Hidden="true" />
                                <f:BoundField Width="50px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                                <f:BoundField Width="70px" DataField="ISGZ" HeaderText="是否贵重" TextAlign="Center" Hidden="true" />
                                <f:BoundField Width="80px" DataField="ISGZNAME" HeaderText="是否贵重" TextAlign="Center" />
                                <f:BoundField Width="80px" DataField="ISLOT" HeaderText="批号管理" TextAlign="Center" />
                                <f:BoundField Width="90px" DataField="HSJJ" HeaderText="含税进价" TextAlign="Right" DataFormatString="{0:F4}" />
                                <f:BoundField Width="90px" DataField="KCSL" HeaderText="库存数量" TextAlign="Right" />
                                <f:BoundField Width="90px" DataField="HSJE" HeaderText="含税金额" TextAlign="Right" DataFormatString="{0:F2}" />
                                <f:BoundField Width="90px" DataField="HWID" HeaderText="货位" TextAlign="Center" />
                                <f:BoundField Width="110px" DataField="PHID" HeaderText="批号" TextAlign="Center" />
                                <f:BoundField Width="110px" DataField="YXQZ" HeaderText="有效期至" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                <f:BoundField Width="110px" DataField="RQ_SC" HeaderText="生产日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                <%--<f:BoundField Width="110px" DataField="GDMODE" HeaderText="商品型号" TextAlign="Center" />--%>
                                <f:BoundField Width="80px" DataField="BZHL" HeaderText="包装含量" TextAlign="Center" />
                                <f:BoundField Width="120px" DataField="PIZNO" HeaderText="注册证号" TextAlign="Center" />
                                <f:BoundField Width="110px" DataField="PRODUCER" HeaderText="生产厂家" Hidden="true" />
                                <f:BoundField Width="120px" DataField="PRODUCERNAME" HeaderText="生产厂家" TextAlign="Center" />
                                <f:BoundField Width="0px" DataField="ZPBH" HeaderText="制品编码" TextAlign="Center" Hidden="true" />
                                <f:BoundField Width="110px" DataField="PICINO" HeaderText="批次编号" TextAlign="Center" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
</body>
</html>
