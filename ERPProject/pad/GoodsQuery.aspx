<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoodsQuery.aspx.cs" Inherits="ERPProject.pad.GoodsQuery" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="minimal-ui"/>
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no"/>
    <meta name="apple-mobile-web-app-capable" content="yes" />
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link rel="stylesheet" href="/res/css/pad.css" />
    <style>
        .item-table .item-gdname,.firstcol {
            min-width:160px;
        }
        .item-table .item-gdseq,.secondcol {
        min-width:160px;
        }
        .item-table .thirdcol {
        min-width:160px;
        
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1"  runat="server" />
        <f:Panel ID="Panel1" ShowBorder="false" ShowHeader="false" Layout="Fit" BoxConfigChildMargin="0 0 5px 0" BodyPadding="10px" runat="server" AutoScroll="true">
            <Toolbars>
                        <f:Toolbar runat="server" HeaderStyle="true" >
                            <Items>
                                <f:Button runat="server" Text="" Size="Large" EnablePostBack="false" EnableDefaultState="false" IconFont="Home"  ID="Button1">
                                    <Listeners>
                                        <f:Listener Event="click" Handler="home" />
                                    </Listeners>
                                 </f:Button>
                                <f:Label runat="server" Text="商品查询检索" CssClass="large-font ml-10"></f:Label>
                                <f:ToolbarFill runat="server"></f:ToolbarFill>
                                <f:Button runat="server" Text="条件查询" Size="Large" EnablePostBack="false" EnableDefaultState="false" IconFont="Search" >
                                    <Listeners>
                                        <f:Listener Event="click" Handler="onOpenWindowQuery" />
                                    </Listeners>
                                </f:Button>
                        
                            </Items>
                        </f:Toolbar>
                        <f:Toolbar runat="server" Position="Bottom">
                            <Items>
                                <f:Button runat="server" Text="" Size="Large" EnablePostBack="false" EnableDefaultState="false" IconFont="StepBackward"  ID="btnStepBackward" >
                                    <Listeners>
                                        <f:Listener Event="click" Handler="stepBackward" />
                                    </Listeners>
                                 </f:Button>
                                <f:Button runat="server" Text="" Size="Large" EnablePostBack="false" EnableDefaultState="false" IconFont="ChevronLeft"  ID="btnBackward">
                                    <Listeners>
                                        <f:Listener Event="click" Handler="forward" />
                                    </Listeners>
                                 </f:Button>
                                <f:TextBox runat="server" ID="currPage" Readonly="true" Text="1" Width="50px"></f:TextBox>
                                <f:Label runat="server" Text="/"></f:Label>
                                <f:TextBox runat="server" ID="totalPage" Readonly="true" Width="50px"></f:TextBox>
                                <f:Button runat="server" Text="" Size="Large" EnablePostBack="false" EnableDefaultState="false" IconFont="ChevronRight"  ID="btnForward">
                                    <Listeners>
                                        <f:Listener Event="click" Handler="backward" />
                                    </Listeners>
                                 </f:Button>
                                <f:Button runat="server" Text="" Size="Large" EnablePostBack="false" EnableDefaultState="false" IconFont="StepForward"  ID="btnStepForward">
                                    <Listeners>
                                        <f:Listener Event="click" Handler="stepForward" />
                                    </Listeners>
                                 </f:Button>
                                <f:HiddenField runat="server" ID="pageSize" Text="20"></f:HiddenField>
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
            <Items>
                <f:Panel runat="server" Hidden="false" ShowBorder="false" ShowHeader="false" ID="EmptyPanel">
                            <Items>
                                <f:ContentPanel runat="server" ShowBorder="false" ShowHeader="false">
                            
                                    <h2><i class="f-btn-icon ui-icon f-icon-info-circle" style="display: inline-block;"></i>未查询到数据</h2>
                                    <p> 如需查询数据或更改查询条件请点击 [条件查询] 进行筛选查询</p>
                                </f:ContentPanel>
                            </Items>
                        </f:Panel>
                <f:DataList runat="server" ID="BillList" Hidden="true" >
                        </f:DataList>
            </Items>
        </f:Panel>
        <f:Panel runat="server" Layout="VBOX" ID="Panel2" ShowHeader="false"  ShowBorder="false" Hidden="true" AutoScroll="true" IsViewPort="true" BodyPadding="10px">
                    <Toolbars>
                        <f:Toolbar runat="server" HeaderStyle="true" >
                        
                                <Items>
                                    <f:Button runat="server" Text="返回" Size="Large" EnablePostBack="false" EnableDefaultState="false" IconFont="ArrowLeft" >
                                        <Listeners>
                                            <f:Listener Event="click" Handler="onSlideLeftClick" />
                                        </Listeners>
                                    </f:Button>
                                    <f:Label runat="server" Text="商品信息详情" CssClass="large-font ml-10"></f:Label>
                                </Items>
                        </f:Toolbar>
                    </Toolbars>
            <Items>
                <%--<f:ContentPanel runat="server" ShowBorder="false" ShowHeader="false" ID="GoodsPicPanel" Height="200px">
                    
                </f:ContentPanel>--%>
                <f:form runat="server" ID="DetailForm" ShowBorder="false" ShowHeader="false" BodyPadding="10" LabelWidth="90px" CssStyle="border-bottom:1px solid #99bce8;" AutoScroll="true" >
                    <Items>
                        <f:TextBox ID="tbxGDID" runat="server" Label="商品编码" EmptyText="设定后不可更改(为空则自动生成)" TabIndex="1" Enabled="false"
							ShowRedStar="true" MaxLength="20">
						</f:TextBox>
						<f:DropDownList ID="docFLAG" runat="server" Label="当前状态"  ShowRedStar="true" TabIndex="2"  Enabled="false" ForceSelection="true">
						</f:DropDownList>
						<f:DropDownList ID="ddlCATID" runat="server" Label="商品分类" ShowRedStar="true" AutoPostBack="true" Enabled="false" ForceSelection="true"  TabIndex="3">
						</f:DropDownList>
						<f:TextBox ID="tbxZJM" runat="server" Label="助记码" TabIndex="4" MaxLength="50">
						</f:TextBox>
                        <f:TextBox ID="tbsGDNAME" runat="server" Label="商品名称" AutoPostBack="true" Enabled="false"
							ShowRedStar="true" TabIndex="21" MaxLength="100" />
						<f:DropDownList ID="docCATID0" runat="server" Label="商品种类" ForceSelection="true" ShowRedStar="true" TabIndex="22" Enabled="false">
						</f:DropDownList>
						<f:DropDownList ID="ddlISLOT" runat="server" Label="批号管理" TabIndex="43" ForceSelection="true">
							<f:ListItem Text="不进行" Value="0" />
							<f:ListItem Text="只有入库" Value="1" />
							<f:ListItem Text="全部" Value="2" Selected="true" />
						</f:DropDownList>
                        <f:TextBox ID="tbxNAMEJC" runat="server" Label="通用名" ShowRedStar="true" TabIndex="31" MaxLength="100" />
						<f:TextBox ID="tbxHISNAME" runat="server" Label="HIS名称" AutoPostBack="true" MaxLength="100">
						</f:TextBox>
						<f:CheckBox ID="ckbISFLAG7" runat="server" Label="本地"  Checked="true" TabIndex="8" />
                        <f:TextBox ID="tbxGDSPEC" runat="server" Label="规格·容量" ShowRedStar="true" TabIndex="52" MaxLength="200" Enabled="false">
						</f:TextBox>
						<f:DropDownList ID="ddlUNIT" runat="server" Label="包装单位" ShowRedStar="true" TabIndex="42" ForceSelection="true" Enabled="false">
						</f:DropDownList>

						<f:TextBox ID="tbxSTR3" runat="server" Label="HIS规格">
						</f:TextBox>
						<f:NumberBox ID="nbxNUM1" runat="server" Label="HIS系数" NoDecimal="true"></f:NumberBox>
                        <f:NumberBox ID="nbbBHSJJ" runat="server" Label="不含税进价" TabIndex="30" DecimalPrecision="6" MinValue="0" MaxLength="12" Enabled="false"></f:NumberBox>
						<f:NumberBox ID="nbbHSJJ" runat="server" Label="含税进价" TabIndex="29" DecimalPrecision="6" MinValue="0" MaxLength="12" ShowRedStar="true" Enabled="false"></f:NumberBox>
						<f:TextBox ID="tbxHISCODE" runat="server" Label="HIS编码" MaxLength="20">
						</f:TextBox>

						<f:TextBox ID="tbxSTR4" runat="server" Label="HIS助记码" TabIndex="4" MaxLength="50">
						</f:TextBox>
                        <f:DropDownList ID="trbPRODUCER" Label="生产商" runat="server" Enabled="false" EnableEdit="true" ForceSelection="true" ShowRedStar="true" TabIndex="61">
						</f:DropDownList>
						<f:TextBox ID="tbxPIZNO" runat="server" Label="注册证号" TabIndex="74" MaxLength="50" Enabled="false">
						</f:TextBox>
                        <f:DropDownList ID="ddlUNIT_DABZ" runat="server" Label="大包装单位" TabIndex="81" EnableEdit="true" ForceSelection="true" Enabled="false">
						</f:DropDownList>
						<f:NumberBox ID="nbbNUM_DABZ" runat="server" Label="大包装数量" TabIndex="82" MinValue="0" NoDecimal="true" MaxLength="16" Enabled="false"></f:NumberBox>
						<f:NumberBox ID="nbbBARCODE_DABZ" runat="server" Label="大包装条码" TabIndex="83" MaxLength="20" NoDecimal="true" Enabled="false"></f:NumberBox>
						<f:DropDownList ID="ddlUNIT_ORDER" runat="server" Label="订货单位" TabIndex="84">
							<f:ListItem Text="大包装" Value="D" Selected="true" />
							<f:ListItem Text="中包装" Value="Z" />
							<f:ListItem Text="小包装" Value="X" />
						</f:DropDownList>
                        <f:DropDownList ID="ddlUNIT_ZHONGBZ" runat="server" Label="中包装单位" TabIndex="91" EnableEdit="true" ForceSelection="true" Enabled="false">
						</f:DropDownList>
						<f:NumberBox ID="nbbNUM_ZHONGBZ" runat="server" Label="中包装数量" TabIndex="92" MinValue="0" NoDecimal="true" MaxLength="16" Enabled="false"></f:NumberBox>
						<f:NumberBox ID="nbbBARCODE_ZHONGBZ" runat="server" Label="中包装条码" TabIndex="93" MaxLength="20" NoDecimal="true" Enabled="false"></f:NumberBox>
						<f:DropDownList ID="ddlUNIT_SELL" runat="server" Label="出库单位" TabIndex="94">
							<f:ListItem Text="大包装" Value="D" />
							<f:ListItem Text="中包装" Value="Z" />
							<f:ListItem Text="小包装" Value="X" Selected="true" />
						</f:DropDownList>
                        <f:NumberBox ID="tbxBZHL" runat="server" Label="包装含量" Hidden="true" NoDecimal="true" MaxLength="8">
						</f:NumberBox>
						<f:HiddenField ID="hfdIsNew" runat="server" Text="Y" />
						<f:HiddenField ID="hfdGDSEQ" runat="server" />
                        <f:TextBox ID="tbxINPER" runat="server" Label="引进人" TabIndex="63" MaxLength="10">
						</f:TextBox>
						<f:DatePicker ID="dpkINRQ" runat="server" Label="引进日期" TabIndex="53">
						</f:DatePicker>
						<f:TextBox ID="tbxMANAGER" runat="server" Label="主管人员" TabIndex="6" MaxLength="15">
						</f:TextBox>
						<f:NumberBox ID="nblJHZQ" runat="server" Label="进货周期" TabIndex="17" NoDecimal="true" MinValue="0" MaxLength="4"></f:NumberBox>
						
                        <f:DatePicker ID="dpkBEGRQ" runat="server" Label="使用日期" TabIndex="71">
						</f:DatePicker>
						<f:DatePicker ID="dpkENDRQ" runat="server" Label="停用日期" TabIndex="72">
						</f:DatePicker>
						<f:NumberBox ID="tbxZGKC" runat="server" Label="最高库存" TabIndex="10" DecimalPrecision="4" MinValue="0" MaxLength="14">
						</f:NumberBox>
						<f:NumberBox ID="tbxZDKC" runat="server" Label="最低库存" TabIndex="11" DecimalPrecision="4" MinValue="0" MaxLength="14">
						</f:NumberBox>
                       
                        <f:NumberBox ID="tbxHSJ" runat="server" Label="核算价" TabIndex="26" DecimalPrecision="4" MinValue="0" MaxLength="14">
						</f:NumberBox>
						<f:NumberBox ID="tbxLSJ" runat="server" Label="零售价" TabIndex="24" DecimalPrecision="4" MinValue="0" MaxLength="14">
						</f:NumberBox>
						<f:NumberBox ID="tbxJXTAX" runat="server" Label="商品税率" TabIndex="27" DecimalPrecision="4" MinValue="0" MaxLength="10">
						</f:NumberBox>
						<f:NumberBox ID="tbxYBJ" runat="server" Label="医保价" TabIndex="25" DecimalPrecision="4" MinValue="0" MaxLength="14">
						</f:NumberBox>
                         
                        <f:TextBox ID="tbxNAMEEN" runat="server" Label="英文名" TabIndex="41" MaxLength="100">
						</f:TextBox>

						<f:TextBox ID="tbxBAR1" runat="server" Label="药监码" TabIndex="34" MaxLength="20">
						</f:TextBox>
						<f:TextBox ID="tbxYCODE" runat="server" Label="原编码" TabIndex="22">
						</f:TextBox>
                        
                        <f:DropDownList ID="trbSUPPLIER" Label="供应商" runat="server" TabIndex="51"  ForceSelection="true" />

						<f:NumberBox ID="nbxNUM2" runat="server" Label="复用次数" TabIndex="25" DecimalPrecision="0" MinValue="0" MaxLength="14" />

						<f:TextBox ID="tbxBAR3" runat="server" Label="ERP编码" TabIndex="23" MaxLength="50" Enabled="false" />
                        
                        <f:TextBox ID="tbxBAR2" runat="server" Label="统计码" TabIndex="35" MaxLength="20">
						</f:TextBox>
						<f:NumberBox ID="nbxMJYXQ" runat="server" Label="灭菌效期" TabIndex="25" DecimalPrecision="0" MinValue="0" MaxLength="14" />
                        
                        <f:TextBox ID="tbxCATUSER" runat="server" Label="用户分类" TabIndex="35" MaxLength="20">
						</f:TextBox>
						<f:NumberBox ID="nbbBARCODE" runat="server" Label="商品条码" TabIndex="24" MaxLength="20" NoDecimal="true" Enabled="false"></f:NumberBox>
						<f:NumberBox ID="nbxKPYXQ" runat="server" Label="开瓶效期" TabIndex="25" DecimalPrecision="0" MinValue="0" MaxLength="14" />
                        <f:CheckBox ID="ckbISFLAG3" runat="server" Label="直送商品" TabIndex="5" />
						<f:CheckBox ID="ckbISIN" runat="server" Label="进口商品" TabIndex="23" />
						<f:CheckBox ID="ckbISYNZJ" runat="server" Label="是否医保" TabIndex="37" />
						<f:CheckBox ID="ckbISJG" runat="server" Label="监管药品" TabIndex="8" />
						<f:CheckBox ID="ckbISFLAG2" runat="server" Label="复用商品" TabIndex="28" />
						<f:CheckBox ID="ckbISGZ" runat="server" Label="高值商品" TabIndex="28" />   
						<f:CheckBox ID="ckbISFLAG5" runat="server" Label="是否小数" TabIndex="28" />
						<f:CheckBox ID="ckbISFLAG6" runat="server" Label="高值扫描" TabIndex="33" />
						<f:CheckBox ID="ckbISJB" runat="server" Label="基药商品" TabIndex="18" />
                    </Items>
                </f:form>
            </Items>
            </f:Panel>
         <f:Window runat="server" ID="WindowQuery" ShowHeader="false" Hidden="true" EnableAnimation="true"
                    IsModal="true" HideOnMaskClick="true" Layout="Fit" AutoScroll="true"
                    PercentWidth="100%" Height="250px" EnableDefaultCorner="false" PositionY="Top">
                    <Items>
                        <f:Panel runat="server" Layout="Fit" ID="PanelQuery" ShowHeader="false" ShowBorder="false">
                            <Toolbars>
                                <f:Toolbar ID="Toolbar2" runat="server" Position="Top" CssClass="ui-state-default">
                                    <Items>
                                        <f:ToolbarFill runat="server"></f:ToolbarFill>
                                        <f:Button runat="server" Text="查询" Size="Large" EnablePostBack="false" EnableDefaultState="false" IconFont="Search">
                                            <Listeners>
                                                <f:Listener Event="click" Handler="onQuery" />
                                            </Listeners>
                                        </f:Button>

                                    </Items>
                                </f:Toolbar>
                            </Toolbars>
                            <Items>
                                <f:Form ID="FormQuery" runat="server" ShowHeader="false" ShowBorder="false" BodyPadding="10px">
                                    <Rows>
                                        <f:FormRow>
                                            <Items>
                                                <f:TextBox ID="trbxSearch" runat="server" Label="商品信息" LabelWidth="80" MaxLength="20" EmptyText="可输入编码、名称或助记码"></f:TextBox>
                                                <f:DropDownList ID="ddlCATID0" runat="server" Label="商品分类" LabelWidth="80" ForceSelection="true">
                                                </f:DropDownList>
                                                <f:DropDownList ID="ddlFLAG" runat="server" Label="商品状态" LabelWidth="80" >
                                                    <f:ListItem Text="---请选择---" Value="" />
                                                    <f:ListItem Text="新增" Value="N" />
                                                    <f:ListItem Text="正常" Value="Y" />
                                                    <f:ListItem Text="停用" Value="S" />
                                                    <f:ListItem Text="停购" Value="T" />
                                                    <f:ListItem Text="淘汰" Value="E" />
                                                </f:DropDownList>

                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:DropDownList ID="ddlSUPID" runat="server" Label="供应商" LabelWidth="80" ForceSelection="true">
                                                </f:DropDownList>
                                                <f:DropDownList ID="ddlPASSID" runat="server" Label="配送商" LabelWidth="80" ForceSelection="true">
                                                </f:DropDownList>
                                                <f:TriggerBox ShowTrigger="false" runat="server" ID="tgbPRO" Label="生产厂家" LabelWidth="80" MaxLength="20" EmptyText="输入厂家信息"></f:TriggerBox>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:DropDownList runat="server" ID="ddlISFLAG7" Label="商品类型" LabelWidth="80px">
                                                    <f:ListItem Text="-- 全部 --" Value="" />
                                                    <f:ListItem Text="下传商品" Value="N" />
                                                    <f:ListItem Text="本地商品" Value="Y" />
                                                </f:DropDownList>
                                                <f:DropDownList ID="JSMODE" runat="server" Label="结算模式" LabelWidth="80">
                                                    <f:ListItem Text="--请选择--" Value="" />
                                                    <f:ListItem Text="入库结" Value="R" />
                                                    <f:ListItem Text="出库结" Value="C" />
                                                    <f:ListItem Text="销售结" Value="X" />
                                                </f:DropDownList>
                                                <f:DropDownList ID="GoodsMode" runat="server" Label="供应模式" LabelWidth="80">
                                                    <f:ListItem Text="--请选择--" Value="" />
                                                    <f:ListItem Text="托管" Value="0" />
                                                    <f:ListItem Text="代管" Value="1" />
                                                    <f:ListItem Text="直供" Value="Z" />
                                                </f:DropDownList>
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Window>
    </form>
    <script src="/res/js/pad.js" type="text/javascript"></script>
    <script>
        var WindowQuery = '<%=WindowQuery.ClientID%>';
        var currPage = '<%=currPage.ClientID%>';
        var totalPage = '<%=totalPage.ClientID%>';
        var pageSize = '<%=pageSize.ClientID%>';
        var billList = '<%=BillList.ClientID%>';
        var EmptyPanel = '<%=EmptyPanel.ClientID%>';
        var FormQuery = '<%=FormQuery.ClientID%>';
        var Panel1 = '<%=Panel1.ClientID%>';
        var Panel2 = '<%=Panel2.ClientID%>';
        var DetailForm = '<%=DetailForm.ClientID%>';
        var GoodsPicPanel = ''
        //向左滑动
        function onSlideLeftClick() {
                F.slideRight(Panel2, Panel1);
        }
        function stepBackward() {
            F(currPage).setValue('1')
            bindList();
        }
        function stepForward() {
            F(currPage).setValue(F(totalPage).getValue())
            bindList()
        }
        function forward() {
            if (parseInt(F(currPage).getValue()) > 1) {
                F(currPage).setValue(parseInt(F(currPage).getValue()) - 1)
                bindList()
            }
           
            
        }
        function backward() {
            if (parseInt(F(currPage).getValue()) <parseInt( F(totalPage).getValue())) {
                F(currPage).setValue(parseInt(F(currPage).getValue()) + 1)
                bindList()
            }
            
        }
        function home() {
            //if (parent && parent.home) {
            //    parent.home();
            //} else {
                location.href = '/pad/index.aspx';
            //}
        }
        function onOpenWindowQuery() {
            F(WindowQuery).show();
        }

        function onQuery() {
            bindList(true);
            F(WindowQuery).hide();
            //查询按钮
        }

        //列表页模板
        var DATALIST_TEMPLATE = "<table class=\"item-table\"><tr><td width='93%'><div><span style='display:none' class='item-guid'>{1}</span><span class='item-gdname'><b>{3}</b></span><span class='item-gdseq'>商品编码：<b class='item-gdseq-value'>{2}</b></span><span class='item-gdspec thirdcol'>规格：{4}</span><span>ERP编码：{9}</span></div>"
            + "<div><span class='firstcol'>状态：{10}</span><span class='item-unit secondcol'>单位：{5}</span><span class='item-hsjj thirdcol'>价格：{7}</span><span >注册证号：{24}</span></div>"
            + "<div><span class='firstcol'>商品种类：{11}</span><span class='secondcol'>商品类别：{12}</span><span class='thirdcol'>商品类型：{15}</span><span >结算模式：{16}</span></div>"
            + "<div><span class='firstcol'>订货单位：{17}</span><span class='secondcol'>订货单位含量：{18}</span><span class='thirdcol'>出库单位：{19}</span><span >出库单位含量：{20}</span></div>"
            + "<div><span class='firstcol'>生产厂家：{21}</span><span >供应商：{22}</span><span >配送商：{23}</span></div>"
            + "<div><span class='firstcol'>HIS编码：{25}</span><span class='secondcol'>HIS名称：{26}</span><span class='thirdcol'>HIS规格：{27}</span></div>"
            + "</td><td class=\"actions\"></td></tr></table>";

        function bindList(notfirst) {
            if (notfirst) { F(currPage).setValue("1"); }
            var pagesize = F(pageSize).getValue();
            var pageindex = F(currPage).getValue();
            var data = formToJson(FormQuery);
            data["pagesize"] = pagesize;
            data["pageindex"] = pageindex;
            toggleMask()
            $.ajax('/ERPquery/goodsquery.aspx?osid=querylist', {
                data: data,
                success: function (data) {
                    
                    toggleMask(true)
                    var resultData = eval('(' + data + ')');
                    var objData = resultData["data"];
                    var result = resultData["result"];
                    if (result == "fail") {
                        F.alert(objData);
                        return;
                    }
                    objData = eval('(' + objData + ')');
                    var total = resultData["total"];
                    var totalpage = resultData["totalpage"];
                    F(totalPage).setValue(totalpage)
                    //F(ListData).setValue(objData);

                    var newData = new Array();
                    for (var i = 0; i < objData.length; i++) {
                        var content = DATALIST_TEMPLATE
                           .replace(/\{1\}/ig, guid())
                        .replace(/\{2\}/ig, convertEmptyStr(objData[i]["GDSEQ"]))
                        .replace(/\{3\}/ig, convertEmptyStr(objData[i]["GDNAME"]))
                        .replace(/\{4\}/ig, convertEmptyStr(objData[i]["GDSPEC"]))
                         .replace(/\{5\}/ig, convertEmptyStr(objData[i]["UNITNAME"]))
                        .replace(/\{6\}/ig, objData[i]["BZSL"])
                        .replace(/\{7\}/ig, objData[i]["HSJJ"])
                        .replace(/\{8\}/ig, objData[i]["HSJE"])
                            .replace(/\{9\}/ig, convertEmptyStr(objData[i]["BAR3"]))
                            .replace(/\{10\}/ig, convertEmptyStr(objData[i]["FLAGNAME"]))
                             .replace(/\{11\}/ig, convertEmptyStr(objData[i]["CATID0NAME"]))
                             .replace(/\{12\}/ig, convertEmptyStr(objData[i]["CATIDNAME"]))
                         .replace(/\{13\}/ig, convertEmptyStr(objData[i]["TYPE"]))
                         .replace(/\{14\}/ig, convertEmptyStr(objData[i]["MEMO"]))
                        .replace(/\{15\}/ig, convertEmptyStr(objData[i]["ISFLAG7_CN"]))
                        .replace(/\{16\}/ig, convertEmptyStr(objData[i]["STR3NAME"]))
                        .replace(/\{17\}/ig, convertEmptyStr(objData[i]["UNITORDERNAME"]))
                            .replace(/\{18\}/ig, convertEmptyStr(objData[i]["NUMORDER"]))
                            .replace(/\{19\}/ig, convertEmptyStr(objData[i]["UNITSELLNAME"]))
                        .replace(/\{20\}/ig, convertEmptyStr(objData[i]["NUMSELL"]))
                        .replace(/\{21\}/ig, convertEmptyStr(objData[i]["CD"]))
                            .replace(/\{22\}/ig, convertEmptyStr(objData[i]["SUPNAME"]))
                            .replace(/\{23\}/ig, convertEmptyStr(objData[i]["PSSNAME"]))
                        .replace(/\{24\}/ig, convertEmptyStr(objData[i]["PIZNO"]))
                        .replace(/\{25\}/ig, convertEmptyStr(objData[i]["HISCODE"]))
                            .replace(/\{26\}/ig, convertEmptyStr(objData[i]["HISNAME"]))
                            .replace(/\{27\}/ig, convertEmptyStr(objData[i]["STR3"]))
                        .replace(/\{28\}/ig, convertEmptyStr(objData[i]["ZPBH"]))
                        newData.push(content);
                    }

                    F(billList).loadData(newData);
                    F(billList).el.find('.f-datalist-item-inner .item-table .actions').each(function () {
                        F.create({
                            type: 'button',
                            text: '详情',
                            scale: 'large',
                            cls: 'mybtn',
                            renderTo: this,
                            iconFont: 'file-o'
                        });
                    });

                    if (objData.length) {
                        F(EmptyPanel).hide();
                        F(billList).show();
                    } else {
                        F(EmptyPanel).show();
                        F(billList).hide();
                    }
                }
            })
        }

        function newBill() {
            F(DetailForm).reset();
        }

        function billOpen(code) {
            newBill();
            var data = {};
            data["gdseq"] = code;
            toggleMask()
            $.ajax('/ERPquery/goodsquery.aspx?osid=query', {
                data: data,
                success: function (data) {
                    toggleMask(true)
                    var resultData = eval('(' + data + ')');
                    var objData = resultData["data"];
                    var result = resultData["result"];
                    if (result == "fail") {
                        F.alert(objData);
                        return;
                    }
                    objData = eval('(' + objData + ')');
                    picData = resultData["picdata"];
                    picData = eval('(' + picData + ')');
                    //todo 图片轮播
                    //if (picData && picData.length > 0) {
                    //    F(GoodsPicPanel).show()
                    //} else {
                    //    F(GoodsPicPanel).hide ()
                    //}
                    setFormValues(DetailForm, objData)
                }
            });
        }

        //向右滑动
        function onSlideRightClick(event, code) {
            newBill();
            F.slideLeft(Panel1, Panel2);
            if (code) {
                billOpen(code);
            }
        }

        F.ready(function () {
            lockForm(DetailForm);
            F(billList).el.on('touchend', '.f-datalist-item-inner .mybtn', function (e) {
                var btnEl = $(this), innerEl = btnEl.parents('.f-datalist-item-inner');
                var code = innerEl.find('.item-gdseq-value').text();
                onSlideRightClick(e, code);

            });
            bindList();
        })

    </script>
</body>
</html>
