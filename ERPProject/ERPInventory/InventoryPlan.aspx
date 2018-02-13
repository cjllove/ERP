<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InventoryPlan.aspx.cs" Inherits="ERPProject.ERPInventory.InventoryPlan" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>盘点计划管理</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="TabStrip1"
            runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="1"
            runat="server">
            <Tabs>
                <f:Tab Title="单据列表" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel2" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBOX"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="Panel3" ShowBorder="false" BodyPadding="0px"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar2" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="操作信息：双击打开单据明细！" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                <f:Button ID="bntClear" Icon="Erase" Text="清 除" EnablePostBack="true" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnablePostBack="true" runat="server" ValidateForms="Formlist" OnClick="btnBill_Click" EnableDefaultState="false" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="90px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TextBox ID="lstBILLNO" runat="server" Label="单据编号" EmptyText="输入预盘单单号" MaxLength="20" />
                                                        <f:DropDownList ID="lstFLAG" runat="server" Label="单据状态" EnableEdit="true" ForceSelection="true">
                                                            <f:ListItem Text="--请选择--" Value="" Selected="true" />
                                                            <f:ListItem Text="新单" Value="N" />
                                                            <f:ListItem Text="已审核" Value="Y" />
                                                            <f:ListItem Text="已完结" Value="S" />
                                                        </f:DropDownList>
                                                        <f:DatePicker ID="lstLRRQ1" runat="server" Label="盘点日期" Required="true" ShowRedStar="true" />
                                                        <f:DatePicker ID="lstLRRQ2" runat="server" Label="至" Required="true" ShowRedStar="true" CompareType="String" CompareControl="lstLRRQ1" CompareOperator="GreaterThanEqual" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" BoxFlex="1" ShowBorder="false" ShowHeader="false"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableTextSelection="true"
                                    DataKeyNames="SEQNO" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick" EnableColumnLines="true" OnRowDataBound="GridList_RowDataBound"
                                    EnableHeaderMenu="true" SortField="SEQNO" OnSort="GridList_Sort" SortDirection="ASC" AllowSorting="true">
                                    <Columns>
                                        <f:RowNumberField />
                                        <f:BoundField Width="150px" DataField="SEQNO" HeaderText="预盘单单号" TextAlign="Center" SortField="SEQNO" />
                                        <f:BoundField Width="80px" DataField="BILLTYPE" HeaderText="单据类别" TextAlign="Center" Hidden="true" SortField="BILLTYPE" />
                                        <f:BoundField Width="130px" DataField="FLAG" HeaderText="单据状态" Hidden="true" SortField="FLAG" />
                                        <f:BoundField Width="80px" DataField="FLAGNAME" ColumnID="FLAGNAME" HeaderText="单据状态" TextAlign="Center" SortField="FLAGNAME" />
                                        <f:BoundField Width="130px" DataField="PDRQ" HeaderText="盘点日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="PDRQ" />
                                        <f:BoundField Width="120px" DataField="PDTYPE" HeaderText="盘点类别" TextAlign="Center" Hidden="true" SortField="PDTYPE" />
                                        <f:BoundField Width="120px" DataField="PDTYPENAME" HeaderText="盘点类别" TextAlign="Center" SortField="PDTYPENAME" />
                                        <f:BoundField Width="110px" DataField="LRY" HeaderText="录入员" TextAlign="Center" Hidden="true" SortField="LRY" />
                                        <f:BoundField Width="110px" DataField="LRYNAME" HeaderText="录入员" TextAlign="Center" SortField="LRYNAME" />
                                        <f:BoundField Width="130px" DataField="LRRQ" HeaderText="录入日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="LRRQ" />
                                        <f:BoundField Width="110px" DataField="SPR" HeaderText="审核员" TextAlign="Center" Hidden="true" SortField="SPR" />
                                        <f:BoundField Width="130px" DataField="SPRQ" HeaderText="审核日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="SPRQ" />
                                        <f:BoundField Width="110px" DataField="SHR" HeaderText="审批员" TextAlign="Center" Hidden="true" SortField="SHR" />
                                        <f:BoundField Width="110px" DataField="SPRNAME" HeaderText="审核员" TextAlign="Center" SortField="SPRNAME" />
                                        <f:BoundField Width="130px" DataField="SHRQ" HeaderText="审批日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="SHRQ" />
                                        <f:BoundField Width="170px" DataField="MEMO" HeaderText="备注" TextAlign="Center" SortField="MEMO" ExpandUnusedSpace="true" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="单据信息" Icon="PageWord" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBox"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="PanelBody" ShowBorder="false" BodyPadding="0px"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar1" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="操作信息：单据操作主界面！" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="btnNew" Icon="PageAdd" Text="新 单" EnablePostBack="true" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                                <f:Button ID="btnDel" Icon="PageCancel" Text="整单删除" EnablePostBack="true" ConfirmText="是否确认删除此单据?" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                                <f:Button ID="btnSave" Icon="Disk" Text="保 存" EnablePostBack="true" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" EnableDefaultState="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnAudit" Icon="UserTick" Text="审 核" EnablePostBack="true" ConfirmText="是否确认保存并审核此单据?" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                                <f:Button ID="btnCancel" Icon="UserCross" Text="驳 回" EnablePostBack="true" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                                <f:Button ID="btnCopy" Icon="PageCopy" Text="复 制" ConfirmText="是否确定已经保存数据并复制单据?" EnablePostBack="true" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnAddRow" Icon="Add" Text="增 行" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" EnableDefaultState="false" />
                                                <f:Button ID="btnDelRow" Icon="Delete" Text="删 行" EnablePostBack="true" ConfirmText="是否确认删除选中行信息?" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnPrint" Icon="Magnifier" Text="追加部门" EnablePostBack="true" runat="server" OnClick="btnadd_Click" ValidateForms="FormDoc" EnableDefaultState="false" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="FormDoc" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="90px" runat="server">
                                            <Rows>
                                                <f:FormRow ColumnWidths="50% 25% 25%">
                                                    <Items>
                                                        <f:DropDownList ID="ddlPDTYPE" runat="server" Label="盘点类别" AutoPostBack="true" ShowRedStar="true" Required="true" OnSelectedIndexChanged="ddlPDTYPE_SelectedIndexChanged">
                                                            <f:ListItem Value="1" Text="1-普通盘点" Selected="true" />
                                                            <f:ListItem Value="2" Text="2-动销盘点" />
                                                        </f:DropDownList>
                                                        <f:DatePicker ID="dpkPDRQ" runat="server" Label="盘点日期" ShowRedStar="true" Required="true"></f:DatePicker>
                                                        <f:TextBox ID="tbxBILLNO" runat="server" Label="预盘单号" EmptyText="自动生成盘点计划单号">
                                                        </f:TextBox>
                                                        <f:DropDownList ID="ddlFLAG" runat="server" Label="单据状态" Enabled="false">
                                                            <f:ListItem Text="新单" Value="N" Selected="true" />
                                                            <f:ListItem Text="已审核" Value="Y" />
                                                            <f:ListItem Text="已完结" Value="S" />
                                                        </f:DropDownList>
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="ddlISPH" runat="server" Label="批次管理" Required="true" ShowRedStar="true">
                                                            <f:ListItem Text="是" Value="Y" Selected="true" />
                                                            <f:ListItem Text="否" Value="N" />
                                                        </f:DropDownList>
                                                        <f:DropDownList ID="ddlCZY" runat="server" Label="操作员"></f:DropDownList>
                                                        <f:DropDownList ID="ddlLRY" runat="server" Label="录入员" Enabled="false"></f:DropDownList>
                                                        <f:DatePicker ID="dpkLRRQ" runat="server" Label="录入日期" Enabled="false"></f:DatePicker>
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DatePicker ID="dpkBEGINRQ" runat="server" Label="开始日期" Enabled="false"></f:DatePicker>
                                                        <f:DatePicker ID="dpkENDRQ" runat="server" Label="结束日期" Enabled="false"></f:DatePicker>
                                                        <f:DropDownList ID="ddlSPR" runat="server" Label="审批人" Enabled="false"></f:DropDownList>
                                                        <f:DatePicker ID="dpkSPRQ" runat="server" Label="审批日期" Enabled="false"></f:DatePicker>
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow ColumnWidths="50% 25% 25%">
                                                    <Items>
                                                        <f:TextBox ID="tbxMEMO" runat="server" Label="备注"></f:TextBox>
                                                        <f:DropDownList ID="ddlSHR" runat="server" Label="审核人" Enabled="false"></f:DropDownList>
                                                        <f:DatePicker ID="dpkSHRQ" runat="server" Label="审核日期" Enabled="false"></f:DatePicker>
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridGoods" BoxFlex="1" ShowBorder="false" ShowHeader="false" AllowCellEditing="true" ClicksToEdit="1" EnableAfterEditEvent="false" EnableColumnLines="true" EnableTextSelection="true"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" DataKeyNames="CODE" EnableSummary="true" SummaryPosition="Bottom">
                                    <Columns>
                                        <f:RenderField Width="35px" TextAlign="Center" EnableLock="true" Locked="true" ColumnID="ROWNO" DataField="ROWNO" FieldType="String" EnableHeaderMenu="false">
                                            <Editor>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" DataField="CODE" ColumnID="CODE" HeaderText="科室编码" Hidden="true">
                                            <Editor>
                                                <f:Label ID="lblCODE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" DataField="NAME" ColumnID="NAME" HeaderText="部门名称" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" DataField="FLAG" ColumnID="FLAG" HeaderText="盘点标志" TextAlign="Center" Hidden="true" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comFLAG" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="110px" DataField="PDDBILL" ColumnID="PDDBILL" HeaderText="盘点单号" TextAlign="Center" EnableColumnHide="true">
                                            <Editor>
                                                <f:Label ID="lblPDDBILL" runat="server" Enabled="false"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" DataField="FLAGNAME" HeaderText="盘点标志" TextAlign="Center" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comFLAGNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" DataField="PERSON" ColumnID="PERSON" HeaderText="负责人" TextAlign="Center" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comPERSON" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" DataField="SYSL" ColumnID="SYSL" HeaderText="损益数量" TextAlign="Right" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comSYSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="120px" DataField="SYHSJE" ColumnID="SYHSJE" HeaderText="损益含税金额" TextAlign="Right" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comSYHSJE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" DataField="SYBHSJE" ColumnID="SYBHSJE" HeaderText="损益不含税金额" TextAlign="Right" EnableColumnHide="false" EnableHeaderMenu="false" Hidden="true">
                                            <Editor>
                                                <f:Label ID="comSYBHSJE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="130px" DataField="KSWC" ColumnID="KSWC" HeaderText="科室盘点完成日期" FieldType="Date" Renderer="Date" RendererArgument="yyyy-MM-dd" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="comKSWC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" DataField="MEMO" ColumnID="MEMO" HeaderText="备注" TextAlign="Center" EnableColumnHide="false" EnableHeaderMenu="false" ExpandUnusedSpace="true">
                                            <Editor>
                                                <f:TextBox ID="ctbMEMO" runat="server" MaxLength="80"></f:TextBox>
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
        <f:HiddenField ID="hfdValue" runat="server" />
        <f:Window ID="WindowLot" Title="盘点部门信息" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="false" Layout="Anchor" Width="530px" Height="360px">
            <Items>
                <f:Panel ShowBorder="false" ShowHeader="false" RegionPosition="Top" Layout="Anchor" runat="server" Width="530px" Height="310px">
                    <Items>
                        <f:Grid ID="GridLot" ShowBorder="false" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server" EnableColumnLines="true" AnchorValue="100% 100%"
                            DataKeyNames="CODE" EnableCheckBoxSelect="true" EnableMultiSelect="true" KeepCurrentSelection="true" EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridGoods_RowDoubleClick">
                            <Columns>
                                <f:RowNumberField runat="server" Width="30px"></f:RowNumberField>
                                <f:BoundField Width="90px" DataField="CODE" HeaderText="部门编码" />
                                <f:BoundField Width="150px" DataField="NAME" HeaderText="部门名称" EnableHeaderMenu="false" />
                                <f:BoundField Width="110px" DataField="managername" HeaderText="负责人" EnableHeaderMenu="false" />
                                <f:BoundField Width="120px" DataField="TYPE" HeaderText="机构类型" Hidden="true" />
                                <f:BoundField DataField="BYNAME" HeaderText="机构别名" />
                            </Columns>
                        </f:Grid>
                    </Items>
                </f:Panel>
            </Items>
            <Toolbars>
                <f:Toolbar ToolbarAlign="Right" AnchorValue="100%" runat="server" Position="Top">
                    <Items>
                        <f:TriggerBox ID="tgbDept" runat="server" ShowTrigger="true" EmptyText="可输入部门编码或名称" MaxLength="20" AutoPostBack="true" OnTriggerClick="btnSch_Click"></f:TriggerBox>
                        <f:Button ID="btnSch" Icon="Magnifier" Text="查 询" EnablePostBack="true" runat="server" OnClick="btnSch_Click" EnableDefaultState="false" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Toolbars>
                <f:Toolbar ID="Toolbar3" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:HiddenField ID="hfdRowIndex" runat="server" />
                        <f:Button ID="btnClosePostBack" Text="确定" Icon="SystemSave" runat="server" OnClick="btnClosePostBack_Click" EnableDefaultState="false">
                        </f:Button>
                        <f:Button ID="btnClose" Text="关闭" Icon="SystemClose" runat="server" OnClick="btnClose_Click" EnableDefaultState="false">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
        <f:Window ID="WindowReject" Title="驳回信息" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="370px" Height="200px">
            <Items>
                <f:Form ID="Form2" ShowBorder="false" AutoScroll="false" BodyPadding="5px 10px"
                    ShowHeader="False" LabelWidth="70px" runat="server">
                    <Rows>
                        <f:FormRow Hidden="true">
                            <Items>
                                <f:DropDownList ID="ddlReject" runat="server" Label="驳回原因" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow Hidden="true">
                            <Items>
                                <f:TextArea ID="txaMemo" runat="server" Label="详细说明" Height="100px" />
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar4" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="btnRejectSubmit" Text="确定" Icon="SystemSave" runat="server" OnClick="btnRejectSubmit_Click" EnableDefaultState="false">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
    </form>
</body>
</html>
