<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderUpload2.aspx.cs" Inherits="ERPProject.ERPUpload.OrderUpload2" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>EXCEL导入生成订单</title>
    <style type="text/css" media="all">
        .x-grid-row.highlight td {
            background-color: red;
            background-image: none;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="TabStrip1"
            runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right" EnableTabCloseMenu="false" ActiveTabIndex="0" runat="server">
            <Tabs>
                <f:Tab Title="原始数据分析" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel5" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBOX"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="Panel6" ShowBorder="false" BodyPadding="0px" Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar3" runat="server" Position="Bottom">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText3" CssStyle="" Text="定数公式：定数数量(2-4个)=日均用量*科室送货周期(未定义则默认7日)/出库单位含量     定数含量=出库单位含量倍数" runat="server" />
                                                <f:ToolbarFill runat="server"></f:ToolbarFill>
                                                <f:Button ID="btnRecy" Icon="ArrowRotateClockwise" Text="重置计算数据" ConfirmText="是否重新计算导入数据?" EnablePostBack="true" runat="server" OnClick="btnRecy_Click" EnableDefaultState="false" />
                                                <f:Button ID="bunClear" Icon="Erase" Text="清空记录" ConfirmText="是否清空导入数据?" EnablePostBack="true" runat="server" OnClick="bunClear_Click"  EnableDefaultState="false"/>
                                                <f:ToolbarSeparator runat="server"></f:ToolbarSeparator>
                                                <f:Button ID="btnDH" Icon="PageExcel" ConfirmText="是否导出订货信息?" Text="Excel订货导出" runat="server" OnClick="btnDH_Click" DisableControlBeforePostBack="false" EnableDefaultState="false">
                                                </f:Button>
                                                <f:Button ID="btnDS" Icon="PageExcel" ConfirmText="是否导出科室定数信息?" Text="Excel定数导出" runat="server" OnClick="btnDS_Click" DisableControlBeforePostBack="false" EnableDefaultState="false">
                                                </f:Button>
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:GroupPanel runat="server" Title="医院匹配信息导入" Hidden="true">
                                            <Items>
                                                <f:Form ID="Form3" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px" ShowHeader="False" LabelWidth="90px" runat="server">
                                                    <Rows>
                                                        <f:FormRow ColumnWidths="44% 11% 3% 11% 11% 11% 9%">
                                                            <Items>
                                                                <f:FileUpload runat="server" ID="fpdExcelJC" Label="选择Excel" LabelWidth="110px" EmptyText="请选择已经匹配信息,已匹配则不需要导入" ShowRedStar="true"></f:FileUpload>
                                                                <f:CheckBox runat="server" ID="CbxAll" CssStyle="margin-left:20px" Label="强制导入" ShowRedStar="true"></f:CheckBox>
                                                                <f:Label runat="server"></f:Label>
                                                                <f:Button ID="btnJCXX" Icon="PageBreakInsert" runat="server" Text="Excel导入系统" OnClick="btnJCXX_Click" EnableDefaultState="false"></f:Button>
                                                                <f:Button ID="BtnGoodsErr" runat="server" Icon="PageExcel" Text="Excel错误信息" OnClick="BtnGoodsErr_Click" EnableDefaultState="false"></f:Button>
                                                                <f:Button ID="btnGoodsSed" runat="server" Text="Excel导入信息" EnablePostBack="true" Icon="SystemSearch" OnClick="btnGoodsSed_Click" EnableDefaultState="false"></f:Button>
                                                                <f:Button ID="btnInssys" runat="server" CssStyle="margin-left:20px" Icon="PackageStart" ConfirmText="是否确定将数据更新至系统中?" Text="导入系统" OnClick="btnInssys_Click" EnableDefaultState="false"></f:Button>
                                                            </Items>
                                                        </f:FormRow>
                                                    </Rows>
                                                </f:Form>
                                            </Items>
                                        </f:GroupPanel>
                                    </Items>
                                    <Items>
                                        <f:GroupPanel runat="server" Title="医院销售信息导入">
                                            <Items>
                                                <f:Form ID="Form2" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px" ShowHeader="False" LabelWidth="70px" runat="server">
                                                    <Rows>
                                                        <f:FormRow ColumnWidths="62% 12% 9% 9%">
                                                            <Items>
                                                                <f:FileUpload runat="server" ID="fueExl" Label="选择Excel" LabelWidth="110px" EmptyText="请选择需要进行数据分析的Excel原始表格" ShowRedStar="true"></f:FileUpload>
                                                                <f:Button ID="ExlIn" CssStyle="margin-left:35px" Icon="PageBreakInsert" runat="server" Text="Excel导入系统" OnClick="ExlIn_Click" EnableDefaultState="false"></f:Button>
                                                                <f:Button ID="ErrSrch" runat="server" Icon="SystemSearch" Text="Excel错误查询" OnClick="ErrSrch_Click" EnableDefaultState="false"></f:Button>
                                                                <f:Button ID="btnExp" runat="server" Icon="DiskDownload" Text="样表下载" ConfirmText="是否下载匹配信息样表?" OnClick="btnExp_Click" DisableControlBeforePostBack="false" EnableDefaultState="false"></f:Button>
                                                            </Items>
                                                        </f:FormRow>
                                                    </Rows>
                                                </f:Form>
                                            </Items>
                                        </f:GroupPanel>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridIns" BoxFlex="1" AnchorValue="100% -98" ShowBorder="false" ShowHeader="false" EnableColumnLines="true"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;"
                                    DataKeyNames="MEMO">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar4" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText4" CssStyle="" Text="订货公式：订货数(不超过科室库存周期3倍)=(未经营品种：定数量*3)或(已经营品种：定数量*3-(库存-已存在定数量合计*2))" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                <f:TriggerBox ID="trbSearch" LabelWidth="90px" Width="350px" MaxLength="20" Label="商品信息" runat="server" EmptyText="可输入ERP编码或ERP编码或商品名称" TriggerIcon="Search" OnTriggerClick="trbSearch_TriggerClick" />
                                                <f:Button ID="btnSrchDh" Icon="SystemSearch" CssStyle="margin-left:50px" Text="订货查询" EnablePostBack="true" runat="server" OnClick="btnSrchDh_Click" EnableDefaultState="false"/>
                                                <f:Button ID="btnSrchDs" Icon="SystemSearch" Text="定数查询" DisableControlBeforePostBack="false" runat="server" OnClick="btnSrchDs_Click" EnableDefaultState="false"/>
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" />
                                        <f:BoundField Width="170px" DataField="MEMO" ColumnID="MEMO" HeaderText="错误信息" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="120px" DataField="DEPTIDNAME" ColumnID="DEPTIDNAME" HeaderText="科室名称" TextAlign="Center" />

                                        <f:BoundField Width="100px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                                        <f:BoundField Width="180px" DataField="GDNAME" HeaderText="商品名称" TextAlign="Left" />
                                        <f:BoundField Width="100px" DataField="GDSPEC" HeaderText="商品规格" TextAlign="Center" />
                                        <f:BoundField Width="50px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="HSJJ" HeaderText="价格" TextAlign="Right" />
                                        <f:BoundField Width="160px" DataField="PROCEDURENAME" HeaderText="生产厂家" TextAlign="Center" />
                                        <f:BoundField Width="160px" DataField="PIZNO" HeaderText="注册证号" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="BAR3" HeaderText="ERP编码" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="HISCODE" HeaderText="HIS编码" TextAlign="Center" />
                                        <f:BoundField Width="150px" DataField="HISNAME" HeaderText="HIS名称" TextAlign="Center" />

                                        <f:BoundField Width="70px" DataField="NUM_ZHONGBZ" ColumnID="NUM_ZHONGBZ" HeaderText="中包装数" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="UNITNAME_Z" ColumnID="UNITNAME_Z" HeaderText="中包装名" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="NUM_DABZ" ColumnID="NUM_DABZ" HeaderText="大包装数" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="UNITNAME_D" ColumnID="UNITNAME_D" HeaderText="大包装名" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="UNITNAME_O" ColumnID="UNITNAME_O" HeaderText="订货单位" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="UNITNAME_C" ColumnID="UNITNAME_C" HeaderText="出库单位" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="SL" ColumnID="SL" HeaderText="日均用量" TextAlign="Right" />
                                        <f:BoundField Width="80px" DataField="ORDERNUM" ColumnID="ORDERNUM" HeaderText="订货数" TextAlign="Center" />

                                        <f:BoundField Width="100px" DataField="DSHL" ColumnID="DSHL" HeaderText="定数含量(参考)" TextAlign="Right" />
                                        <f:BoundField Width="100px" DataField="DSSL" ColumnID="DSSL" HeaderText="定数数量(参考)" TextAlign="Right" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="订单信息导入" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel2" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBOX"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="Panel3" ShowBorder="false" BodyPadding="0px" Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar2" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="操作信息：首选选择需要导入的Excel文件！" runat="server" />
                                                <f:ToolbarFill runat="server"></f:ToolbarFill>
                                                <f:Button ID="btnSel" CssStyle="margin-left:85px;" Icon="PageBreakInsert" ConfirmText="请先选择导入的Excel文件,并确认文件已关闭?" Text="Excel导入" runat="server" OnClick="btnSelect_Click" EnableDefaultState="false"></f:Button>
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="bntClr" Icon="Erase" Text="清空记录" ConfirmText="是否清空导入数据?" EnablePostBack="true" runat="server" OnClick="bntClr_Click" EnableDefaultState="false"/>
                                                <f:Button ID="btnInsert" Icon="PackageStart" Text="导入系统" ConfirmText="是否确认将此数据导入系统?" ValidateForms="Formlist" EnablePostBack="true" runat="server" OnClick="btnInsert_Click" EnableDefaultState="false"/>
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnError" Icon="SystemSearch" Text="查询错误信息" EnablePostBack="true" runat="server" OnClick="btnError_Click" EnableDefaultState="false"/>
                                                <f:Button ID="btnSearch" Icon="SystemSearch" Text="查询订货信息" EnablePostBack="true" runat="server" OnClick="btnSearch_Click" EnableDefaultState="false" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="90px" runat="server">
                                            <Rows>
                                                <f:FormRow ColumnWidths="50% 25% 25%">
                                                    <Items>
                                                        <f:FileUpload runat="server" ID="fuDocument" Label="选择Excel" LabelWidth="110px" EmptyText="" CssStyle="" ShowRedStar="true"></f:FileUpload>
                                                        <f:DropDownList runat="server" Label="订货部门" ShowRedStar="true" Required="true" ID="ddlDEPTID"></f:DropDownList>
                                                        <f:DropDownList runat="server" Label="录入员" Required="true" ID="ddlLRY" Enabled="false"></f:DropDownList>
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow ColumnWidths="25% 25% 25% 25%">
                                                    <Items>
                                                        <f:Label runat="server"></f:Label>
                                                        <f:DropDownList runat="server" CssStyle="margin-left:30px" LabelWidth="90px" Label="供应商" ShowRedStar="true" Required="true" ID="ddlSUPID"></f:DropDownList>
                                                        <f:DatePicker runat="server" Label="订货日期" ShowRedStar="true" Required="true" ID="dpkDhrq"></f:DatePicker>
                                                        <f:DatePicker runat="server" Label="录入日期" Required="true" ID="dpkLRRQ" Enabled="false"></f:DatePicker>
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" BoxFlex="1" AnchorValue="100% -95" ShowBorder="false" ShowHeader="false" EnableColumnLines="true"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;"
                                    DataKeyNames="SEQNO">
                                    <Columns>
                                        <f:RowNumberField Width="35px" />
                                        <f:BoundField Width="100px" DataField="CODE" HeaderText="商品编码" Hidden="true" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="MEMO" ColumnID="MEMOOrder" HeaderText="错误信息" Hidden="true" TextAlign="Center" />
                                        <f:BoundField Width="180px" DataField="GDNAME" HeaderText="商品名称" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="GDSPEC" HeaderText="商品规格" TextAlign="Center" />
                                        <f:BoundField Width="50px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                                        <f:BoundField Width="60px" DataField="HSJJ" HeaderText="单价" TextAlign="Right" />
                                        <f:BoundField Width="160px" DataField="PROCEDURENAME" HeaderText="生产厂家" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="BAR3" HeaderText="ERP编码" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="HISCODE" HeaderText="HIS编码" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="HISNAME" HeaderText="HIS名称" TextAlign="Center" />
                                        <%--                                        <f:BoundField Width="70px" DataField="NUM_Z" HeaderText="中包装数" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="UNITNAME_Z" HeaderText="中包装名" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="NUM_D" HeaderText="大包装数" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="UNITNAME_D" HeaderText="大包装名" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="UNITNAME_S" HeaderText="订货单位" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="UNITNAME_C" HeaderText="出库单位" TextAlign="Center" />--%>
                                        <f:BoundField Width="80px" DataField="ORDERNUM" HeaderText="订货数" TextAlign="Center" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="定数信息导入" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBOX"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="Panel4" ShowBorder="false" BodyPadding="0px" Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar1" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="操作信息：首选选择需要导入的Excel文件！" runat="server" />
                                                <f:ToolbarFill runat="server"></f:ToolbarFill>
                                                <f:Button ID="btnExport" CssStyle="margin-left:85px;" Icon="PageBreakInsert" ConfirmText="请先选择导入的Excel文件,并确认文件已关闭?" Text="Excel导入" runat="server" OnClick="btnExport_Click" EnableDefaultState="false">
                                                </f:Button>
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="Btnclr" Icon="Erase" Text="清空记录" ConfirmText="是否清空导入数据?" EnablePostBack="true" runat="server" OnClick="Btnclr_Click" EnableDefaultState="false"/>
                                                <f:Button ID="btnINPUT" Icon="PackageStart" Text="导入系统" ConfirmText="是否确认将此数据导入系统?" ValidateForms="FmList" EnablePostBack="true" runat="server" OnClick="btnINPUT_Click" EnableDefaultState="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnerr" Icon="SystemSearch" Text="查询错误信息" EnablePostBack="true" runat="server" OnClick="btnerr_Click" EnableDefaultState="false"/>
                                                <f:Button ID="btnSearchDS" Icon="SystemSearch" Text="查询定数信息" EnablePostBack="true" runat="server" OnClick="btnSearchDS_Click" EnableDefaultState="false"/>
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="FmList" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="90px" runat="server">
                                            <Rows>
                                                <f:FormRow ColumnWidths="39% 18% 18% 25%">
                                                    <Items>
                                                        <f:FileUpload runat="server" ID="FileUpd" Label="选择Excel" LabelWidth="110px" EmptyText="" CssStyle="" ShowRedStar="true"></f:FileUpload>
                                                        <f:CheckBox runat="server" ID="cbxDS" Label="修改定数" CssStyle="margin-left:30px" ShowRedStar="true" Checked="true"></f:CheckBox>
                                                        <f:NumberBox runat="server" ID="nbxKCXX" Enabled="false" Width="180px" Label="库存下限基数" MinValue="0" LabelWidth="110px" DecimalPrecision="2" ShowRedStar="true" Text="1"></f:NumberBox>
                                                        <f:DropDownList runat="server" Label="录入员" Required="true" ID="lisLRY" Enabled="false"></f:DropDownList>
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow ColumnWidths="39% 18% 18% 25%">
                                                    <Items>
                                                        <f:Label runat="server"></f:Label>
                                                        <f:CheckBox runat="server" ID="cbxKC" Label="修改库存上下限" CssStyle="margin-left:30px" LabelWidth="140px" ShowRedStar="true" AutoPostBack="true" OnCheckedChanged="cbxKC_CheckedChanged"></f:CheckBox>
                                                        <f:NumberBox runat="server" ID="nbxKCSX" Enabled="false" Width="180px" Label="库存上限基数" MinValue="0" LabelWidth="110px" DecimalPrecision="2" ShowRedStar="true" Text="2"></f:NumberBox>
                                                        <f:DatePicker runat="server" Label="录入日期" Required="true" ID="lisLRRQ" Enabled="false"></f:DatePicker>
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridDs" BoxFlex="1" AnchorValue="100% -95" ShowBorder="false" ShowHeader="false" EnableColumnLines="true"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar5" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText5" CssStyle="" Text="操作信息：(科室库存下限=定数量*库存下限基数)(科室库存上限=定数量*库存上限基数)" runat="server" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Columns>
                                        <f:RowNumberField Width="35px" />
                                        <f:BoundField Width="160px" DataField="MEMO" ColumnID="MEMODS" HeaderText="错误信息" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="60px" DataField="DEPTID" HeaderText="科室编码" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="100px" DataField="DEPTNAME" HeaderText="科室名称" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                                        <f:BoundField Width="180px" DataField="GDNAME" HeaderText="商品名称" TextAlign="Center" />
                                        <f:BoundField Width="120px" DataField="GDSPEC" HeaderText="商品规格" TextAlign="Center" />
                                        <f:BoundField Width="160px" DataField="PROCEDURENAME" HeaderText="生产厂家" TextAlign="Center" />
                                        <f:BoundField Width="50px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="HSJJ" HeaderText="单价" TextAlign="Right" />
                                        <f:BoundField Width="70px" DataField="NUM_ZHONGBZ" HeaderText="中包装数" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="UNITNAME_Z" HeaderText="中包装名" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="NUM_DABZ" HeaderText="大包装数" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="UNITNAME_D" HeaderText="大包装名" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="UNITNAME_O" HeaderText="订货单位" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="UNITNAME_C" HeaderText="出库单位" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="DSSL_CK" HeaderText="参考定数数量" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="DSHL_CK" HeaderText="参考定数含量" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="DSSL" HeaderText="定数数量" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="DSHL" HeaderText="定数含量" TextAlign="Center" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
    </form>
</body>
</html>
