<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SupEvaluationQuery.aspx.cs" Inherits="ERPProject.ERPQuery.SupEvaluationQuery" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" EnableAjaxLoading="false" AjaxLoadingType="Mask" runat="server" />
        <f:Panel runat="server" ShowBorder="false" ShowHeader="false" Layout="Anchor" ID="Panel1">
            <Toolbars>
                <f:Toolbar ID="Toolbar2" runat="server" Position="Top" ToolbarAlign="RIGHT">
                    <Items>
                        <f:Button ID="btnClear" Text="清 除" EnableDefaultState="false" Icon="Erase" runat="server" OnClick="btnClear_Click">
                        </f:Button>
                        <f:ToolbarSeparator runat="server" />
                        <f:Button ID="btnExport" Text="导 出" EnableDefaultState="false" Icon="DatabaseGo" ConfirmText="是否确认导出供应商评价?" EnablePostBack="true" runat="server" OnClick="btnExport_Click" EnableAjax="false" DisableControlBeforePostBack="false">
                        </f:Button>
                        <f:ToolbarSeparator runat="server" />
                        <f:Button ID="btnClosePostBack" Text="查 询" ValidateForms="FormUser" EnableDefaultState="false" Icon="Magnifier" runat="server" OnClick="btnClosePostBack_Click">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Form ID="FormUser" runat="server" LabelWidth="80px" ShowBorder="false" AnchorValue="100%" ShowHeader="false" BodyPadding="10px 5px 5px 5px">
                    <Rows>
                        <f:FormRow ColumnWidths="25% 25% 50%">
                            <Items>
                                <f:TriggerBox ID="txtSEQNO" runat="server" Label="入库单号" EmptyText="可模糊输入单号,回车确认" MaxLength="15" OnTriggerClick="btnClosePostBack_Click"></f:TriggerBox>
                                <f:DropDownList ID="lstPJR" runat="server" Label="评价人" ForceSelection="true" EnableEdit="true">
                                </f:DropDownList>
                                <f:DropDownList ID="lstSUPID" runat="server" Label="供应商" ForceSelection="true" EnableEdit="true">
                                </f:DropDownList>
                                <%-- <f:TextBox ID="txtSupName" runat="server" Label="供应商" EmptyText="供应商编码、名称、助记码" ></f:TextBox>--%>
                            </Items>
                        </f:FormRow>
                        <f:FormRow ColumnWidths="25% 25% 25% 25%">
                            <Items>
                                <f:DropDownList ID="lstPJTYPENAME" runat="server" Label="评价状态" EnableEdit="true" ForceSelection="true">
                                    <f:ListItem Text="--请选择--" Value="" />
                                    <f:ListItem Text="已评价" Value="1" />
                                    <f:ListItem Text="未评价" Value="0" />
                                </f:DropDownList>
                                <f:DropDownList ID="lstGRADE" runat="server" Label="评价等级" EnableEdit="true" ForceSelection="true">
                                    <f:ListItem Text="--请选择--" Value="" />
                                    <f:ListItem Text="五星" Value="5" />
                                    <f:ListItem Text="四星" Value="4" />
                                    <f:ListItem Text="三星" Value="3" />
                                    <f:ListItem Text="二星" Value="2" />
                                    <f:ListItem Text="一星" Value="1" />
                                </f:DropDownList>
                                <f:DatePicker ID="lstLRRQ1" runat="server" Label="到货日期" ShowRedStar="true" Required="true" />
                                <f:DatePicker ID="lstLRRQ2" runat="server" Label=" 至" ShowRedStar="true" Required="true" />
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
                <f:Grid ID="GridGoods" runat="server" ShowBorder="false" ShowHeader="false" AnchorValue="100% -80" CssStyle="border-top: 1px solid #ccc;" OnRowDataBound="GridGoods_RowDataBound"
                    EnableTextSelection="true" EnableMultiSelect="true" EnableCheckBoxSelect="FALSE" CheckBoxSelectOnly="true" IsDatabasePaging="true" EnableColumnLines="true"
                    PageSize="70" DataKeyNames="SEQNO,PSSID,PSSNAME" AllowPaging="true" OnPageIndexChange="GridGoods_PageIndexChange" EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridGoods_RowDoubleClick">
                    <%--DataKeyNames="SUPCODE,SUPNAME；PageSize决定每页显示数据数量；EnableTextSelection决定内容是否可以复制，AnchorValue决定Grid的大小，从而影响分页信息的显示效果--%>
                    <Columns>
                        <f:RowNumberField HeaderText="序号" Width="35px" TextAlign="Center" EnablePagingNumber="true" />
                        <f:BoundField ColumnID="SEQNO" DataField="SEQNO" HeaderText="入库单号" Width="120px" TextAlign="Center"></f:BoundField>
                        <f:BoundField ColumnID="PSSID" DataField="PSSID" HeaderText="供应商编码" Width="200px" Hidden="true" TextAlign="Center"></f:BoundField>
                        <f:BoundField ColumnID="PSSNAME" DataField="PSSNAME" HeaderText="供应商名称" Width="200px" TextAlign="left"></f:BoundField>
                        <f:BoundField ColumnID="DHRQ" DataField="DHRQ" HeaderText="到货日期" Width="100px" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}"></f:BoundField>
                        <f:BoundField ColumnID="PJTYPENAME" DataField="PJTYPENAME" HeaderText="评价状态" Width="80px" TextAlign="Center"></f:BoundField>
                        <f:BoundField ColumnID="GRADENAME" DataField="GRADENAME" HeaderText="评价等级" Width="60px" TextAlign="Center"></f:BoundField>
                        <f:BoundField ColumnID="SCORE" DataField="SCORE" HeaderText="评价给分" Width="60px" TextAlign="Center" Hidden="true"></f:BoundField>
                        <f:BoundField ColumnID="NUM2NAME" DataField="NUM2NAME" HeaderText="包装外观" Width="60px" TextAlign="Center"></f:BoundField>
                        <f:BoundField ColumnID="NUM3NAME" DataField="NUM3NAME" HeaderText="数  量" Width="60px" TextAlign="Center"></f:BoundField>
                        <f:BoundField ColumnID="NUM1NAME" DataField="NUM1NAME" HeaderText="抽检结果" Width="90px" TextAlign="Center"></f:BoundField>
                        <f:BoundField ColumnID="STR5NAME" DataField="STR5NAME" HeaderText="供货及时率" Width="90px" TextAlign="left"></f:BoundField>
                        <f:BoundField ColumnID="PJRNAME" DataField="PJRNAME" HeaderText="评价人" Width="100px" TextAlign="Center"></f:BoundField>
                        <f:BoundField ColumnID="PJSJ" DataField="PJSJ" HeaderText="评价时间" Width="100px" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}"></f:BoundField>
                        <f:BoundField ColumnID="MEMO" DataField="MEMO" HeaderText="评价说明" Width="380px" TextAlign="left"></f:BoundField>
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
        <f:Window ID="winRKD" Title="入库单信息档" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="1000px" Height="600px">
            <Items>
                <f:Panel ID="Panel2" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Region" ShowHeader="false">
                    <Items>
                        <f:Panel ID="Panel4" BodyPadding="0px" RegionSplit="true" EnableCollapse="true" RegionPosition="Top" ShowBorder="false"
                            Layout="Anchor" ShowHeader="False" runat="server">
                            <Items>
                                <f:Form ID="FormDoc" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                    ShowHeader="False" LabelWidth="80px" runat="server">
                                    <Rows>
                                        <f:FormRow Hidden="true">
                                            <Items>
                                                <f:TextBox ID="docSEQNO" runat="server" Hidden="true" />
                                                <f:TextBox ID="docDNLSEQNO" runat="server" Hidden="true" />
                                                <f:TextBox ID="docNUM1" runat="server" Hidden="true" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:TriggerBox ID="docDDBH" runat="server" Label="订单号" LabelWidth="80px" MaxLength="28" Enabled="false" TriggerIcon="Search" ShowTrigger="false"></f:TriggerBox>
                                                <f:DropDownList ID="docCGY" runat="server" Label="操作员" Enabled="false" EnableEdit="true" ForceSelection="true"></f:DropDownList>
                                                <f:TextBox ID="docBILLNO" runat="server" Label="单据编号" Enabled="false" MaxLength="15" />
                                                <f:DropDownList ID="docFLAG" runat="server" Label="单据状态" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                            </Items>
                                        </f:FormRow>

                                        <f:FormRow ColumnWidths="25% 25% 25% 25%">
                                            <Items>
                                                <f:DropDownList ID="docPSSID" runat="server" Label="送货商" Enabled="false" EnableEdit="true" ForceSelection="true">
                                                </f:DropDownList>
                                                <f:TextBox ID="docINVOICENUMBER" runat="server" Label="发票号" Enabled="false" EmptyText="多个发票号用逗号分隔" MaxLength="500" />
                                                <f:DropDownList ID="docLRY" runat="server" Label="录入员" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                <f:DatePicker ID="docLRRQ" runat="server" Label="录入日期" Enabled="false" Required="true" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:DropDownList ID="docDEPTID" runat="server" Label="收货地点" Enabled="false" EnableEdit="true" ForceSelection="true">
                                                </f:DropDownList>
                                                <f:DatePicker ID="docDHRQ" runat="server" Label="收货日期" Enabled="false" />
                                                <f:DropDownList ID="docSHR" runat="server" Label="审核员" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                <f:DatePicker ID="docSHRQ" runat="server" Label="审核日期" Enabled="false" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:TextBox ID="docMEMO" Label="备 注" runat="server" Enabled="false" />
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                            </Items>
                        </f:Panel>
                        <f:Grid ID="GridCom" AnchorValue="100% -144" ShowBorder="false" ShowHeader="false" EnableColumnLines="true"
                            AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;"
                            DataKeyNames="GDSEQ,MEMO,BZSL,YRKSL,DDSL" AllowCellEditing="false" ClicksToEdit="1" EnableAfterEditEvent="false"
                            EnableSummary="true" SummaryPosition="Bottom" AllowColumnLocking="true">
                            <Columns>
                                <f:RenderField Width="35px" EnableLock="true" Locked="true" ColumnID="ROWNO" DataField="ROWNO" FieldType="String" EnableHeaderMenu="false" TextAlign="Center">
                                    <Editor>
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="120px" DataField="GDSEQ" ColumnID="GDSEQ" HeaderText="商品编码" FieldType="String" EnableHeaderMenu="false" EnableLock="true" Locked="true">
                                    <Editor>
                                        <f:Label ID="labGDSEQ" runat="server" BoxConfigAlign="Middle">
                                        </f:Label>
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="180px" DataField="GDNAME" ColumnID="GDNAME" HeaderText="商品名称" FieldType="String" EnableLock="true" Locked="true">
                                    <Editor>
                                        <f:Label ID="comGDNAME" BoxConfigAlign="Middle" runat="server" />
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="100px" DataField="GDSPEC" ColumnID="GDSPEC" HeaderText="商品规格" FieldType="String" EnableLock="true" Locked="true">
                                    <Editor>
                                        <f:Label ID="comGDSPEC" runat="server" />
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="70px" DataField="BZSL" ColumnID="BZSL" HeaderText="入库数" Enabled="false" TextAlign="Center" FieldType="Int">
                                    <Editor>
                                        <f:NumberBox ID="comBZSL" CssClass="ColBlue" Required="true" runat="server" MinValue="0" DecimalPrecision="2" MaxValue="99999999" />
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="70px" DataField="YRKSL" ColumnID="YRKSL" HeaderText="已入库数" EnableLock="true" Locked="true" TextAlign="Center" FieldType="Int">
                                    <Editor>
                                        <f:NumberBox ID="comYRKSL" CssClass="ColBlue" Required="true" runat="server" MinValue="0" DecimalPrecision="2" MaxValue="99999999" />
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="70px" DataField="DDSL" ColumnID="DDSL" HeaderText="订货数" TextAlign="Center" FieldType="Int">
                                    <Editor>
                                        <f:Label ID="comDDSL" runat="server" />
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="90px" ColumnID="UNITNAME" DataField="UNITNAME" FieldType="String" HeaderText="入库单位" TextAlign="Center">
                                    <Editor>
                                        <f:Label ID="comUNITNAME" runat="server" />
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="95px" DataField="PH" ColumnID="PH" HeaderText="批号" TextAlign="Center">
                                    <Editor>
                                        <f:TextBox ID="comPH" Required="true" runat="server" MaxLength="20" />
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="90px" DataField="RQ_SC" ColumnID="RQ_SC" HeaderText="生产日期" TextAlign="Center" FieldType="Date" Renderer="Date" RendererArgument="yyyy-MM-dd" NullDisplayText="">
                                    <Editor>
                                        <f:DatePicker ID="comRQ_SC" Required="true" runat="server" />
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="90px" DataField="YXQZ" ColumnID="YXQZ" HeaderText="有效期至" TextAlign="Center" FieldType="Date" Renderer="Date" RendererArgument="yyyy-MM-dd" NullDisplayText="">
                                    <Editor>
                                        <f:DatePicker ID="comYXQZ" Required="true" runat="server" />
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="250px" DataField="PZWH" ColumnID="PZWH" HeaderText="注册证号" TextAlign="Center">
                                    <Editor>
                                        <%--<f:Label ID="comPZWH" runat="server" />--%>
                                        <f:TextBox ID="comPZWH" runat="server" />
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="90px" DataField="BZHL" ColumnID="BZHL" HeaderText="包装含量" TextAlign="Center" EnableColumnHide="false" EnableHeaderMenu="false">
                                    <Editor>
                                        <f:Label ID="comBZHL" runat="server" />
                                    </Editor>
                                </f:RenderField>

                                <f:RenderField Width="110px" DataField="SSSL" ColumnID="SSSL" HeaderText="入库数(最小单位)" TextAlign="Center" FieldType="Int" EnableColumnHide="false" EnableHeaderMenu="false">
                                    <Editor>
                                        <f:Label ID="comSSSL" runat="server" />
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="110px" DataField="UNITSMALLNAME" ColumnID="UNITSMALLNAME" HeaderText="最小包装单位" FieldType="Auto" TextAlign="Center" EnableHeaderMenu="false">
                                    <Editor>
                                        <f:Label ID="labUNITSMALLNAME" runat="server" />
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="70px" DataField="HWID" ColumnID="HWID" HeaderText="货位" TextAlign="Center">
                                    <Editor>
                                        <f:Label ID="comHWID" runat="server" />
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField DataField="JXTAX" ColumnID="JXTAX" HeaderText="税率" TextAlign="Center" Width="0px" Hidden="true">
                                    <Editor>
                                        <f:Label ID="comJXTAX" runat="server" />
                                    </Editor>
                                </f:RenderField>
                                <%--<f:RenderField Width="80px" DataField="HSJJ" ColumnID="HSJJ" HeaderText="含税进价" TextAlign="Right" FieldType="Auto">
                                            <Editor>
                                                 <f:NumberBox ID="comHSJJ" CssClass="ColBlue" DecimalPrecision="6" NoNegative="True" Required="True" runat="server" MinValue="0" MaxValue="99999999" />
                                             </Editor>
                                        </f:RenderField>--%>
                                <f:RenderField Width="90px" DataField="HSJJ" ColumnID="HSJJ" HeaderText="含税进价" TextAlign="Right" FieldType="String" RendererFunction="round4">
                                    <Editor>
                                        <f:Label ID="Label1" runat="server" />
                                    </Editor>
                                </f:RenderField>

                                <f:RenderField Width="90px" DataField="HSJE" ColumnID="HSJE" HeaderText="含税金额" TextAlign="Right" RendererFunction="round2">
                                    <Editor>
                                        <f:Label ID="comHSJE" runat="server" />
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="180px" ColumnID="PRODUCERNAME" DataField="PRODUCERNAME" FieldType="String" EnableHeaderMenu="false"
                                    HeaderText="生产厂家">
                                    <Editor>
                                        <f:Label ID="lblEditorPRODUCERNAME" runat="server" />
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="95px" DataField="MJPH" ColumnID="MJPH" HeaderText="灭菌批号" TextAlign="Center">
                                    <Editor>
                                        <f:TextBox ID="tbxMJPH" Required="true" runat="server" MaxLength="20" />
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="90px" DataField="MJRQ" ColumnID="MJRQ" HeaderText="灭菌日期" TextAlign="Center" FieldType="Date" Renderer="Date" RendererArgument="yyyy-MM-dd">
                                    <Editor>
                                        <f:DatePicker ID="dpkMJRQ" Required="true" runat="server" />
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="90px" DataField="MJXQ" ColumnID="MJXQ" HeaderText="灭菌效期" TextAlign="Center" FieldType="Date" Renderer="Date" RendererArgument="yyyy-MM-dd">
                                    <Editor>
                                        <f:DatePicker ID="dpkMJXQ" Required="true" runat="server" />
                                    </Editor>
                                </f:RenderField>
                                <%--<f:RenderField Width="180px" ColumnID="SUPNAME" DataField="SUPNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="供应商">
                                            <Editor>
                                                <f:Label ID="lblSUPNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>--%>
                                <f:RenderField Width="0px" Hidden="true" DataField="ZPBH" ColumnID="ZPBH" HeaderText="制品编号" TextAlign="Center">
                                    <Editor>
                                        <f:Label ID="comZPBH" runat="server" />
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="100px" DataField="MEMO" ColumnID="MEMO" HeaderText="备注" TextAlign="Center">
                                    <Editor>
                                        <f:TextBox ID="comMEMO" runat="server" MaxLength="80" />
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="0px" Hidden="true" DataField="BARCODE" ColumnID="BARCODE" HeaderText="商品条码" TextAlign="Center"
                                    FieldType="String" EnableColumnHide="false" EnableHeaderMenu="false">
                                    <Editor>
                                        <f:Label ID="comBARCODE" runat="server" />
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="0px" Hidden="true" ColumnID="UNIT" DataField="UNIT" FieldType="String" HeaderText="包装单位编码" TextAlign="Center"
                                    EnableColumnHide="false" EnableHeaderMenu="false">
                                    <Editor>
                                        <f:Label ID="comUNIT" runat="server" />
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="0px" Hidden="true" DataField="ISLOT" ColumnID="ISLOT" HeaderText="批号管理" EnableColumnHide="false" EnableHeaderMenu="false">
                                    <Editor>
                                        <f:Label ID="comISLOT" runat="server" />
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="0px" Hidden="true" DataField="ISGZ" ColumnID="ISGZ" HeaderText="是否贵重" EnableColumnHide="false" EnableHeaderMenu="false">
                                    <Editor>
                                        <f:Label ID="comISGZ" runat="server" />
                                    </Editor>
                                </f:RenderField>
                                <%--<f:RenderField Width="0px" DataField="CODEINFO" ColumnID="CODEINFO" HeaderText="商品赋码信息" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comCODEINFO" runat="server" />
                                            </Editor>
                                        </f:RenderField>--%>
                                <f:RenderField Width="0px" Hidden="true" ColumnID="SUPID" FieldType="String" EnableHeaderMenu="false"
                                    HeaderText="供应商">
                                    <Editor>
                                        <f:Label ID="lblSUPID" runat="server" />
                                    </Editor>
                                </f:RenderField>
                                <f:RenderField Width="0px" Hidden="true" ColumnID="PRODUCER" DataField="PRODUCER" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                    HeaderText="生产厂家编码" TextAlign="Center">
                                    <Editor>
                                        <f:Label ID="lblEditorPRODUCER" runat="server" />
                                    </Editor>
                                </f:RenderField>
                            </Columns>
                        </f:Grid>
                    </Items>
                </f:Panel>
            </Items>
        </f:Window>
    </form>
</body>
</html>
