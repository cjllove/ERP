<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TraceBack.aspx.cs" Inherits="ERPProject.ERPQuery.TraceBack" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>高值商品追溯管理</title>
    <style type="text/css">
        .ui-state-disabled {
            opacity: .5;
            filter: alpha(opacity=50);
            background-image: none;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="Panel1"
            runat="server" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBox"
            ShowHeader="false">
            <Toolbars>
                <f:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                        <f:ToolbarText runat="server" Text="操作信息：查询高值商品的追溯信息"></f:ToolbarText>
                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                        <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" DisableControlBeforePostBack="true" runat="server" EnableDefaultState="false" ValidateForms="FormUser" OnClick="btSearch_Click" />
                        <f:ToolbarSeparator runat="server" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="5px 10px 0px 10px"
                    ShowHeader="False" LabelWidth="80px" runat="server">
                    <Rows>
                        <f:FormRow>
                            <Items>
                                <f:TriggerBox ID="tbxPatient" runat="server" Label="患者信息" ShowTrigger="false" EmptyText="请输入患者住院号、姓名或身份证号" TriggerIcon="Search" OnTriggerClick="btSearch_Click"></f:TriggerBox>
                                <f:DropDownList ID="lstDEPTID" runat="server" Label="使用科室" EnableEdit="true" ForceSelection="true" />
                                <f:TriggerBox ID="trbGDSEQ" runat="server" Label="商品信息" ShowTrigger="false" EmptyText="请输入商品编码、名称或助记码" TriggerIcon="Search" OnTriggerClick="btSearch_Click"></f:TriggerBox>
                                <%--                                <f:DropDownList ID="ddlFLAG" runat="server" Label="商品状态" EnableEdit="true" ForceSelection="true">
                                    <f:ListItem Text="--请选择--" Value="" />
                                    <f:ListItem Text="未使用" Value="N" />
                                    <f:ListItem Text="已入库" Value="Y" />
                                    <f:ListItem Text="已库房退货" Value="T" />
                                    <f:ListItem Text="已出库" Value="C" />
                                    <f:ListItem Text="库房退货" Value="R" />
                                    <f:ListItem Text="已使用" Value="G" />
                                </f:DropDownList>--%>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:TriggerBox ID="tbxONECODE" runat="server" Label="追 溯 码" ShowTrigger="false" EmptyText="商品追溯码" MaxLength="36" TriggerIcon="Search" OnTriggerClick="tbxONECODE_TriggerClick"></f:TriggerBox>
                                <f:DatePicker ID="dpkout1" runat="server" Label="查询期间" Required="true"></f:DatePicker>
                                <f:DatePicker ID="dpkout2" runat="server" Label="     至" Required="true" CompareControl="dpkout1" CompareOperator="GreaterThanEqual" CompareMessage="结束日期应大于开始日期!"></f:DatePicker>
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
                <f:Grid ID="GridGoods" BoxFlex="1" ShowBorder="false" ShowHeader="false" EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridGoods_RowDoubleClick"
                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableColumnLines="true"
                    PageSize="100" DataKeyNames="DEPTID,CUSTID" IsDatabasePaging="true" AllowPaging="true" OnPageIndexChange="GridGoods_PageIndexChange" EnableTextSelection="true">
                    <Columns>
                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                        <f:BoundField Width="120px" DataField="DEPTNAME" HeaderText="使用部门" />
                        <f:BoundField Width="80px" DataField="CUSTID" HeaderText="患者姓名" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="SL" HeaderText="数量" TextAlign="Center" />
                        <f:BoundField Width="110px" DataField="JE" HeaderText="金额" TextAlign="Center" DataFormatString="{0:F2}" />
                        <f:BoundField Width="110px" DataField="OPTID" HeaderText="手术ID" TextAlign="Center" />
                        <f:BoundField Width="110px" DataField="DOCTOR" HeaderText="主治医生" TextAlign="Center" />
                        <f:BoundField Width="110px" DataField="STR9" HeaderText="手术医生" TextAlign="Center" />
                        <f:BoundField Width="110px" DataField="STR7" HeaderText="患者住院号" TextAlign="Center" />
                        <f:BoundField Width="110px" DataField="STR8" HeaderText="患者流水号" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="NUM4" HeaderText="病床号" TextAlign="Center" />
                        <f:BoundField Width="110px" DataField="OPTDATE" HeaderText="手术日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" Hidden="true" />
                        <f:BoundField Width="110px" DataField="OPTTABLE" HeaderText="手术台号" Hidden="true" TextAlign="Center" />
                        <f:BoundField Width="0px" DataField="DEPTID" HeaderText="使用部门" Hidden="true" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
        <f:Window ID="winXq" Title="详细信息" Hidden="true" EnableIFrame="false" runat="server" ShowHeader="false"
            EnableMaximize="false" EnableResize="true" Target="Self" IsModal="True" Layout="Fit" Width="800px" Height="600px">
            <Items>
                <f:Form ID="ForXq" ShowBorder="false" AutoScroll="false" Layout="Fit"
                    ShowHeader="False" LabelWidth="70px" runat="server">
                    <Items>
                        <f:TabStrip ID="TspPage" runat="server" ShowBorder="false" TabPosition="Right">
                            <Tabs>
                                <f:Tab runat="server" Title="详细信息" Icon="Table" Layout="Fit">
                                    <Items>
                                        <f:Grid ID="GridXq" ShowBorder="false" ShowHeader="false" EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridXq_RowDoubleClick" DataKeyNames="ONECODE"
                                            AutoScroll="true" runat="server" EnableColumnLines="true">
                                            <Columns>
                                                <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                                <f:BoundField Width="210px" DataField="ONECODE" ColumnID="ONECODE" HeaderText="追溯码" TextAlign="Center" />
                                                <f:BoundField Width="100px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                                                <f:BoundField Width="170px" DataField="GDNAME" HeaderText="商品名称" />
                                                <f:BoundField Width="100px" DataField="GDSPEC" HeaderText="商品规格" />
                                                <f:BoundField Width="50px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                                                <f:BoundField Width="60px" DataField="HSJJ" HeaderText="价格" TextAlign="Right" />
                                                <f:BoundField Width="120px" DataField="PH" HeaderText="批号" TextAlign="Center" />
                                                <f:BoundField Width="90px" DataField="YXQZ" HeaderText="效期" DataFormatString="{0:yyyy-MM-dd}" TextAlign="Center" />
                                                <f:BoundField Width="90px" DataField="RQ_SC" HeaderText="生产日期" DataFormatString="{0:yyyy-MM-dd}" TextAlign="Center" />
                                                <f:BoundField Width="190px" DataField="SUPNAME" HeaderText="供应商" Hidden="true" />
                                                <f:BoundField Width="70px" Hidden="true" DataField="USERNAME" HeaderText="操作员" TextAlign="Center" />
                                                <f:BoundField Width="190px" DataField="PRODUCERNAME" HeaderText="生产厂家" />
                                                <f:BoundField Width="200px" DataField="PIZNO" HeaderText="注册证号" />
                                                <f:BoundField Width="90px" DataField="SHRQ" HeaderText="操作日期" DataFormatString="{0:yyyy-MM-dd}" TextAlign="Center" />
                                            </Columns>
                                        </f:Grid>
                                    </Items>
                                </f:Tab>
                                <f:Tab runat="server" Title="流程追溯" ID="tabFrm" Icon="Table" EnableIFrame="true" IFrameUrl="TraceBackFm.aspx" Layout="Fit">
                                    <Items>
                                    </Items>
                                </f:Tab>
                            </Tabs>
                        </f:TabStrip>
                    </Items>
                </f:Form>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar3" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="btnSureDate" Text="关闭" EnableDefaultState="false" ValidateForms="ForXq" Icon="SystemSave" runat="server" OnClick="btnSureDate_Click">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
    </form>
    <script type="text/javascript">
        var tabID = '<%= tabFrm.ClientID %>';
        function reFrame(code) {
            F(tabID).setIFrameUrl('TraceBackFm.aspx?onecode=' + code);
        }

    </script>
</body>
</html>
