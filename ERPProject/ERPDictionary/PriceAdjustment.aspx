<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PriceAdjustment.aspx.cs" Inherits="ERPProject.ERPDictionary.PriceAdjustment" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>商品价格调整</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="TabStrip1" OnCustomEvent="PageManager1_CustomEvent"
            runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="1" runat="server">
            <Tabs>
                <f:Tab Title="单据列表" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel2" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="Panel3" ShowBorder="false" BodyPadding="0px"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar2" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="操作信息：双击打开单据明细！" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                <f:Button ID="bntClear" Icon="Erase" Text="清 除" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TextBox ID="lstBILLNO" runat="server" Label="单据编号" MaxLength="16" EmptyText="单据编号信息" />
                                                        <f:TextBox ID="lstGDSEQ" runat="server" Label="商品信息" MaxLength="20" EmptyText="商品信息" />
                                                        <f:DatePicker ID="lstLRRQ1" runat="server" Label="调价日期" Required="true" ShowRedStar="true" />
                                                        <f:DatePicker ID="lstLRRQ2" runat="server" Label="　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" AnchorValue="100% -75" ShowBorder="false" ShowHeader="false" OnRowDataBound="GridList_RowDataBound"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableTextSelection="true"
                                    DataKeyNames="SEQNO" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" HeaderText="序号"></f:RowNumberField>
                                        <f:BoundField DataField="SEQNO" Hidden="true" />
                                        <f:BoundField Width="115px" DataField="BILLNO" HeaderText="单据编号" TextAlign="Center" />
                                        <f:BoundField DataField="FLAG" Hidden="true" />
                                        <f:BoundField Width="65px" DataField="FLAG_CN" ColumnID="FLAG_CN" HeaderText="单据状态" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="TJRQ" HeaderText="调整日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="140px" DataField="TJREASON" HeaderText="调整原因" />
                                        <f:BoundField Width="80px" DataField="SUBNUM" HeaderText="明细条数" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="SQRNAME" HeaderText="申请人" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="LRYNAME" HeaderText="录入员" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="LRRQ" HeaderText="录入日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="80px" DataField="SHRNAME" HeaderText="审核员" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="SHRQ" HeaderText="审核日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="110px" DataField="MEMO" HeaderText="备注" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="单据信息" Icon="PageWord" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="PanelMain" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Region" ShowHeader="false">
                            <Items>
                                <f:Panel ID="Panel8" BodyPadding="0px" RegionSplit="false" EnableCollapse="false" RegionPosition="Top" ShowBorder="false"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar1" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="操作信息：商品调价申请" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="btnNew" Icon="PageAdd" Text="新 单" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnDelRow" Icon="Delete" Text="删 行" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否确认删除选中行?" OnClick="btnBill_Click" />
                                                <f:Button ID="btnSave" Icon="Disk" Text="保 存" EnableDefaultState="false" EnablePostBack="true" runat="server" ValidateForms="FormPrice" OnClick="btnBill_Click" />
                                                <f:Button ID="btnSubmit" Icon="UserTick" Text="提 交" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否确认提交此单据?" OnClick="btnSubmit_Click" Enabled="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnAudit" Icon="UserTick" Text="审 核" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认审核此单据?" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnGoods" Icon="Magnifier" Text="追加商品" EnableDefaultState="false" ValidateForms="FormPrice" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                </f:Panel>
                                <f:Panel ID="Panel4" BodyPadding="0px" RegionSplit="true" EnableCollapse="true" RegionPosition="Top" ShowBorder="false"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Items>
                                        <f:Form ID="FormPrice" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DatePicker ID="dpkTJRQ" runat="server" Label="调价日期" ShowRedStar="true" />
                                                        <f:DropDownList ID="docSQR" runat="server" Label="申请人" Enabled="false" EnableEdit="true" ForceSelection="true">
                                                        </f:DropDownList>
                                                        <f:TextBox ID="docBILLNO" runat="server" Label="调价单号" Enabled="false" EmptyText="自动生成" />
                                                        <f:DropDownList ID="docFLAG" runat="server" Label="单据状态" Enabled="false" EnableEdit="true" ForceSelection="true">
                                                            <f:ListItem Text="新单" Value="M" />
                                                            <f:ListItem Text="已提交" Value="N" />
                                                            <f:ListItem Text="已审核" Value="Y" />
                                                            <f:ListItem Text="已完结" Value="G" />
                                                        </f:DropDownList>
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TextBox ID="docTJREASON" runat="server" Label="调价原因" Required="true" ShowRedStar="true" MaxLength="40" EmptyText="请填写调整价格原因" />
                                                        <f:DropDownList ID="docTJTYPE" runat="server" Label="调整类型" Enabled="false">
                                                            <f:ListItem Text="库存同调" Value="A" Selected="true" />
                                                        </f:DropDownList>
                                                        <f:DropDownList ID="docLRY" runat="server" Label="录入员" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="docLRRQ" runat="server" Label="录入日期" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow ColumnWidths="50% 25% 25%">
                                                    <Items>
                                                        <f:TextBox ID="doxMEMO" runat="server" Label="备注" MaxLength="80" EmptyText="请输入备注信息" />
                                                        <f:DropDownList ID="docSHR" runat="server" Label="审核人" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="docSHRQ" runat="server" Label="审核日期" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow Hidden="true">
                                                    <Items>
                                                        <f:HiddenField ID="hfdSEQNO" runat="server" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridGoods" AnchorValue="100% -140" ShowBorder="false" ShowHeader="false" AutoScroll="true" runat="server"
                                    CssStyle="border-top: 1px solid #99bce8;" DataKeyNames="GDSEQ" AllowCellEditing="true" ClicksToEdit="1">
                                    <Columns>
                                        <f:RenderField Width="35px" DataField="ROWNO" ColumnID="ROWNO" TextAlign="Center" FieldType="String" EnableHeaderMenu="false">
                                        </f:RenderField>
                                        <f:RenderField Width="100px" DataField="GDSEQ" ColumnID="GDSEQ" HeaderText="商品编码" FieldType="String" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comGDSEQ" BoxConfigAlign="Middle" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="230px" DataField="GDNAME" ColumnID="GDNAME" HeaderText="商品名称" FieldType="String" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comGDNAME" BoxConfigAlign="Middle" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="170px" DataField="GDSPEC" ColumnID="GDSPEC" HeaderText="商品规格" FieldType="String" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comGDSPEC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="60px" ColumnID="UNITNAME" DataField="UNITNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="单位" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="comUNITNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="HSJJ" DataField="HSJJ" EnableHeaderMenu="false"
                                            HeaderText="原价格" TextAlign="Right">
                                            <Editor>
                                                <f:Label ID="comHSJJ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="XHSJJ" DataField="XHSJJ" EnableHeaderMenu="false"
                                            HeaderText="修改后价格<font color='red'>*</font>" TextAlign="Right">
                                            <Editor>
                                                <f:NumberBox ID="nbxXHSJJ" runat="server" MinValue="0" MaxValue="99999999" DecimalPrecision="6" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="160px" ColumnID="PRODUCERNAME" DataField="PRODUCERNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="生产厂家">
                                            <Editor>
                                                <f:Label ID="comPRODUCERNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="210px" ColumnID="PIZNO" DataField="PIZNO" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="注册证号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblPIZNO" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="UNIT" DataField="UNIT" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="true"
                                            HeaderText="单位" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblUNIT" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="PRODUCER" DataField="PRODUCER" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="true"
                                            HeaderText="生产厂家">
                                            <Editor>
                                                <f:Label ID="lblPRODUCER" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
        <f:HiddenField ID="hfdDEPTID" runat="server" />
        <f:HiddenField ID="hfdValue" runat="server" />
        <f:Window ID="Window1" Title="商品信息" Hidden="true" EnableIFrame="true" runat="server"
            EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True"
            Width="820px" Height="480px" OnClose="Window1_Close">
        </f:Window>
    </form>
    <script language="javascript" type="text/javascript">
        function PosGoodsQt(Info) {
            F('<%= hfdValue.ClientID%>').setValue(Info);
            F.customEvent('GoodsAdd');
        }
    </script>
</body>
</html>
