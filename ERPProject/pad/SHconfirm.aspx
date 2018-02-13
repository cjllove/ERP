﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SHconfirm.aspx.cs" Inherits="ERPProject.pad.SHconfirm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="minimal-ui" />
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link rel="stylesheet" href="/res/css/pad.css" />
    <style>
        .item-table .item-code {
            min-width: 120px;
        }

        .item-table .item-status {
            min-width: 50px;
            text-align: center;
        }

        .item-table .item-lry {
            min-width: 190px;
        }

        .item-table .item-gdname, .item-table .item-onecode, .item-table .item-gdspec, .item-table .item-gdseq {
            /*color:#5da2d7;*/
            font-weight: bold;
        }

        .item-table .item-hsjj, .item-table .item-hsje {
            min-width: 160px;
        }

        .item-table .item-rqsc, .item-table .item-yxqz {
            min-width: 160px;
        }

        .item-table .item-ph, .item-table .item-unit {
            min-width: 140px;
        }

        .item-table .item-status.M {
            color: white;
            background-color: #3AA02C;
        }

        .item-table .item-status.R {
            color: white;
            background-color: #AF5553;
        }
        .item-table .item-status.FALSE {
            color: white;
            background-color: #AF5553;
        }

        .item-table .item-status.N {
            color: white;
            background-color: #BFBE24;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" OnCustomEvent="PageManager1_CustomEvent" />
        <%--<f:Panel runat="server" ShowBorder="false" ShowHeader="false" ID="PanelMain" Layout="Fit">
            <Items>--%>
        <f:Panel ID="Panel1" ShowBorder="false" ShowHeader="false" Layout="Fit" BoxConfigChildMargin="0 0 5px 0" BodyPadding="10px" runat="server" AutoScroll="true">
            <Toolbars>
                <f:Toolbar runat="server" HeaderStyle="true">
                    <Items>
                        <f:Button runat="server" Text="" Size="Large" EnablePostBack="false" EnableDefaultState="false" IconFont="Home" ID="Button1">
                            <Listeners>
                                <f:Listener Event="click" Handler="home" />
                            </Listeners>
                        </f:Button>
                        <f:Label runat="server" Text="高值验收入库" CssClass="large-font ml-10"></f:Label>
                        <f:ToolbarFill runat="server"></f:ToolbarFill>
                        <%--<f:Button runat="server" Text="验收审核" Size="Large" EnablePostBack="false" EnableDefaultState="false" IconFont="Plus" ID="useBtn">
                            <Listeners>
                                <f:Listener Event="click" Handler="onSlideRightClick" />
                            </Listeners>
                        </f:Button>--%>
                        <f:Button runat="server" Text="条件查询" Size="Large" EnablePostBack="false" EnableDefaultState="false" IconFont="Search">
                            <Listeners>
                                <f:Listener Event="click" Handler="onOpenWindowQuery" />
                            </Listeners>
                        </f:Button>

                    </Items>
                </f:Toolbar>

            </Toolbars>
            <Items>
                <f:Panel runat="server" Hidden="false" ShowBorder="false" ShowHeader="false" ID="EmptyPanel">
                    <Items>
                        <f:ContentPanel runat="server" ShowBorder="false" ShowHeader="false">

                            <h2><i class="f-btn-icon ui-icon f-icon-info-circle" style="display: inline-block;"></i>未查询到数据</h2>
                            <p> [条件查询] 可以进行进行筛选查询</p>
                        </f:ContentPanel>
                    </Items>
                </f:Panel>
                <f:DataList runat="server" ID="BillList" Hidden="true">
                </f:DataList>
            </Items>
        </f:Panel>
        <f:Panel runat="server" Layout="VBOX" ID="Panel2" ShowHeader="false" ShowBorder="false" Hidden="true" AutoScroll="true" IsViewPort="true">
            <Toolbars>
                <f:Toolbar runat="server" HeaderStyle="true">

                    <Items>
                        <f:Button runat="server" Text="返回" Size="Large" EnablePostBack="false" EnableDefaultState="false" IconFont="ArrowLeft">
                            <Listeners>
                                <f:Listener Event="click" Handler="onSlideLeftClick" />
                            </Listeners>
                        </f:Button>
                        <f:Label runat="server" Text="验收入库单明细" CssClass="large-font ml-10"></f:Label>
                        <f:ToolbarFill runat="server"></f:ToolbarFill>
                        <%--                        
                        <f:Button runat="server" ID="btnNew" Text="新单" Size="Large" EnablePostBack="false" EnableDefaultState="false" IconFont="FileO">
                            <Listeners>
                                <f:Listener Event="click" Handler="newBill" />
                            </Listeners>
                        </f:Button>
                        
                        <f:Button runat="server" ID="btnSave" Text="保存" Size="Large" EnablePostBack="false" EnableDefaultState="false" IconFont="Save">
                            <Listeners>
                                <f:Listener Event="click" Handler="save" />
                            </Listeners>
                        </f:Button>
                            --%>
                        <f:Button runat="server" ID="btnSubmit" Text="审核" Size="Large" EnablePostBack="false" EnableDefaultState="false" IconFont="Check">
                            <Listeners>
                                <f:Listener Event="click" Handler="submitBill" />
                            </Listeners>
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Form runat="server" ID="DetailForm" ShowBorder="false" ShowHeader="false" BodyPadding="10" LabelWidth="90px" CssStyle="border-bottom:1px solid #99bce8;">
                    <Rows>
                        <f:FormRow Hidden="true">
                            <Items>
                                <f:TextBox ID="docSEQNO" runat="server" Hidden="true" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow ColumnWidths="25% 25% 25% 25%">
                            <Items>
                                <f:TextBox ID="docDDBH" runat="server" Label="订单号" MaxLength="15" EmptyText="可输入订货单号" />
                                <f:DropDownList ID="docCGY" runat="server" Label="操作员" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                                <f:TextBox ID="docBILLNO" runat="server" Label="单据编号" EmptyText="自动生成" MaxLength="20" Enabled="false" />
                                <f:DropDownList ID="docFLAG" runat="server" Label="单据状态" Enabled="false" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow ColumnWidths="25% 25% 25% 25%">
                            <Items>
                                <f:DropDownList ID="docPSSID" runat="server" Label="送货商" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                                <f:TextBox ID="docINVOICENUMBER" runat="server" Label="发票号" EmptyText="多个发票号用逗号分隔" MaxLength="500" />
                                <f:DropDownList ID="docLRY" runat="server" Label="录入员" Enabled="false" ForceSelection="true" />
                                <f:DatePicker ID="docLRRQ" runat="server" Label="录入日期" Enabled="false" Required="true" ShowRedStar="true" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow ColumnWidths="25% 25% 25% 25%">
                            <Items>
                                <f:DropDownList ID="docDEPTID" runat="server" Label="收货地点" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                                <f:DatePicker ID="docDHRQ" runat="server" Label="收货日期" Required="true" ShowRedStar="true" />
                                <f:DropDownList ID="docSHR" runat="server" Label="审核人" Enabled="false" ForceSelection="true" />
                                <f:DatePicker ID="docSHRQ" runat="server" Label="审核日期" Enabled="false" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow ColumnWidths="50% 50%">
                            <Items>
                                <f:TextBox runat="server" ID="docONECODE" EmptyText="商品唯一码扫描框,扫描高值码信息"></f:TextBox>
                                <f:TextBox ID="docMEMO" runat="server" Label="备注" MaxLength="40"></f:TextBox>
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
                <f:Panel ID="Panel3" runat="server" ShowBorder="True" EnableCollapse="true" BoxFlex="1" Layout="HBox" BodyPadding="0px" BoxConfigChildMargin="0 0 0 0" ShowHeader="false">
                    <Items>
                        <f:Panel ID="Panel4" AutoScroll="true"  Title="面板1" BoxFlex="2" runat="server" BodyPadding="0px" ShowBorder="true" ShowHeader="false">
                            <Items>
                                <f:DataList runat="server" ID="GoodsList" BoxFlex="1"></f:DataList>
                            </Items>
                        </f:Panel>
                        <f:Panel ID="Panel5" AutoScroll="true"  Title="面板3" BoxFlex="1" runat="server" BodyPadding="0px" Margin="0" ShowBorder="true" ShowHeader="false">
                            <Items>
                                <f:DataList runat="server" ID="DataList1" BoxFlex="2"></f:DataList>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Panel>
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
                                <f:FormRow runat="server">
                                    <Items>
                                        <f:TextBox ID="lstBILLNO" runat="server" Label="单据编号" />
                                        <f:DropDownList ID="lstDEPTID" runat="server" Label="收货地点" ForceSelection="true" />
                                    </Items>
                                </f:FormRow>
                                <f:FormRow runat="server">
                                    <Items>
                                        <f:TextBox ID="lstDDBH" runat="server" Label="订单号" />
                                        <f:DropDownList ID="lstSUPID" runat="server" Label="送货商" ForceSelection="true" />
                                    </Items>
                                </f:FormRow>
                                <f:FormRow runat="server">
                                    <Items>
                                        <f:DropDownList ID="lstFLAG" runat="server" Label="单据状态" ForceSelection="true" />
                                        <f:DropDownList ID="lstLRY" runat="server" Label="录入员" ForceSelection="true" />
                                    </Items>
                                </f:FormRow>
                                <f:FormRow runat="server">
                                    <Items>
                                        <f:DatePicker ID="lstLRRQ1" runat="server" Label="录入日期" Required="true" ShowRedStar="true" />
                                        <f:DatePicker ID="lstLRRQ2" runat="server" Label="　　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                                    </Items>
                                </f:FormRow>
                            </Rows>
                        </f:Form>
                    </Items>
                </f:Panel>
            </Items>
        </f:Window>
        <f:HiddenField runat="server" ID="ListData"></f:HiddenField>
    </form>
    <script src="/res/js/pad.js" type="text/javascript"></script>
    <script>
        var WindowQuery = '<%=WindowQuery.ClientID%>';
        var billList = '<%=BillList.ClientID%>';
        var ListData = '<%=ListData.ClientID%>';
        var DataList1 = '<%=DataList1.ClientID%>';
        var Panel1 = '<%=Panel1.ClientID%>';
        var Panel2 = '<%=Panel2.ClientID%>';
        var FormQuery = '<%=FormQuery.ClientID%>';
        var lstLRRQ1 = '<%=lstLRRQ1.ClientID%>';
        var lstLRRQ2 = '<%=lstLRRQ2.ClientID%>';
        var EmptyPanel = '<%=EmptyPanel.ClientID%>';
        var docONECODE = '<%=docONECODE.ClientID%>';
        var docDDBH = '<%=docDDBH.ClientID%>';
        var docCGY = '<%=docCGY.ClientID%>';
        var GoodsList = '<%=GoodsList.ClientID%>';
        var docLRRQ = '<%=docLRRQ.ClientID%>';
        var docSHRQ = '<%=docSHRQ.ClientID%>';
        var DetailForm = '<%=DetailForm.ClientID%>';
        var docPSSID = '<%=docPSSID.ClientID%>';
        var docINVOICENUMBER = '<%=docINVOICENUMBER.ClientID%>';
        var docDEPTID = '<%=docDEPTID.ClientID%>';
        var docMEMO = '<%=docMEMO.ClientID%>';
        var docFLAG = '<%=docFLAG.ClientID%>';
        var docBILLNO = '<%=docBILLNO.ClientID%>';
        var docLRY = '<%=docLRY.ClientID%>';
        var lstDEPTID = '<%=lstDEPTID.ClientID%>';
        var UserAction = <%=MyUser%>;

        var btnSubmit = '<%=btnSubmit.ClientID%>';
        function onOpenWindowQuery() {
            F(WindowQuery).show();
        }
        function home(){
            //if(parent&& parent.home){
            //    parent.home();
            //}
            location.href='/pad/index.aspx'
        }
        
        //向右滑动
        function onSlideRightClick(event,code) {
            newBill();
            F.slideLeft(Panel1, Panel2);
            if (code) {
                billOpen(code);
            } else {
                document.getElementById(docONECODE + "-inputEl").focus()
            }
        }
        //向左滑动
        function onSlideLeftClick() {
            var sr = function(){
                F.slideRight(Panel2, Panel1);
                bindList();
            }
            if(F(docFLAG).getValue()=="M"||F(docFLAG).getValue()=="R"){
                F.confirm('_top', '确认要返回上级页面吗？未保存的修改将丢失', '提示', 'question',sr)
            }else{
                sr();
            }
        }

        function onQuery() {
            bindList();
            F(WindowQuery).hide();
            //查询按钮
        }

        //列表页模板
        var DATALIST_TEMPLATE = "<table class=\"item-table\"><tr><td width='93%'><div><span class=\"item-code\">{0}</span><span class=\"item-status {2}\">{1}</span><span>收获日期：{4}</span><span>收货地点：{3}</span></div>"
            + "<div><span class='item-lry'>录入员：{5}</span><span >录入日期：{6}</span><span >审核日期：{7}</span></div>"
            + "<div><span >备注：{8}</span></div>"
            + "</td><td class=\"actions\"></td></tr></table>";
        //详情页商品模板
        var DATAGOODS_TEMPLATE = "<table class=\"item-table\"><tr><td width='93%'><div><span style='display:none' class='item-guid'>{1}</span><span class='item-onecode'>{0}</span><span class='item-gdname'>{3}</span><span class='item-gdseq'>商品编码：{2}</span><span class='item-gdspec'>规格：{4}</span><span class='item-sssl'>入库数：{5}</span></div>"
            + "<div><span class='item-unit'>单位：{6}</span><span class='item-hsjj'>价格：{8}</span><span >生产厂家：{14}</span></div>"
            + "<div><span class='item-ph'>批号：{10}</span><span >生产日期：{11}</span><span >有效期至：{12}</span><span class='item-pzwh'>批准文号：{13}</span></div>"
            + "</td><td class=\"actions-goods\"></td></tr></table>";
        var DATAZSM = "<table class=\"item-table\"><tr><td width='93%'><div><span class=\"item-status {0}\">{1}</span><span class=\"item-code\">{2}</span></div>"
            + "</td><td class=\"actions\"></td></tr></table>";

        //绑定查询列表数据
        function bindList() {
            toggleMask()
            $.ajax('/ERPStorage/GoodsStorage.aspx?oper=querylist', {
                data:formToJson(FormQuery),
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
                    //F(ListData).setValue(objData);
                    
                    var newData = new Array();
                    for(var i=0;i<objData.length;i++){
                        var content = DATALIST_TEMPLATE
                            .replace(/\{0\}/ig, convertEmptyStr(objData[i]["BILLNO"]))
                            .replace(/\{1\}/ig, convertEmptyStr(objData[i]["FLAG_CN"]))
                            .replace(/\{2\}/ig, convertEmptyStr(objData[i]["FLAG"]))
                            .replace(/\{3\}/ig, convertEmptyStr(objData[i]["DEPTID"]))
                            .replace(/\{4\}/ig, splitDate(objData[i]["DHRQ"]))
                            .replace(/\{5\}/ig, convertEmptyStr(objData[i]["LRY"]))
                            .replace(/\{6\}/ig, splitDate(objData[i]["LRRQ"]))
                            .replace(/\{7\}/ig, splitDate(objData[i]["SHRQ"]))
                            .replace(/\{8\}/ig, convertEmptyStr(objData[i]["MEMO"]))
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
                            iconFont:'file-o'
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

        function changeEditStatus(canEdit) {
            if (!canEdit) {
                F(docDEPTID).disable();
                F(docMEMO).disable();
               // F(docONECODE).disable();
                F(docDDBH).disable();
                F(docCGY).disable();
                F(docPSSID).disable();
                F(docINVOICENUMBER).disable();
                
            } else {
                F(docDEPTID).enable();
                F(docMEMO).enable();
                F(docONECODE).enable();
                F(docDDBH).enable();
                F(docCGY).enable();
                F(docPSSID).enable();
                F(docINVOICENUMBER).enable();
                
            }
        }

        //打开单据明细
        function billOpen(billno) {
            newBill();
            toggleMask();
            $.ajax('/ERPStorage/GoodsStorage.aspx?oper=queryBill&billno=' + billno, {
                //async:false,
                success: function (data) {
                    toggleMask(true)
                    var resultData = eval('(' + data + ')');
                    
                    var docData = resultData["doc"];
                    var comData = resultData["com"];
                    var zsmData = resultData["zsm"];
                    var result = resultData["result"];
                    if (result == "fail") {
                        F.alert("获取数据失败！");
                        return;
                    }
                    docData = eval('(' + docData + ')');
                    comData = eval('(' + comData + ')');
                    zsmData = eval('(' + zsmData + ')');
                    var canEdit = true;
                    var noBtn = false;
                    if (docData[0]["FLAG"] != "M" && docData[0]["FLAG"] != "R") {
                        noBtn = true;
                        //F(btnNew).enable();
                        //F(btnSave).disable();
                        //F(btnSubmit).disable();
                    }else{
                        //F(btnNew).enable();
                        //F(btnSave).enable();
                        F(btnSubmit).enable();
                    }
                    canEdit = !noBtn;
                    changeEditStatus(canEdit);
                    setFormValues(DetailForm, docData)
                    
                    var newData = new Array();
                    for (var i = 0; i < comData.length; i++) {
                        var content = DATAGOODS_TEMPLATE
                        .replace(/\{0\}/ig, convertEmptyStr(comData[i]["ONECODE"]))
                        .replace(/\{1\}/ig, guid())
                        .replace(/\{2\}/ig, convertEmptyStr(comData[i]["GDSEQ"]))
                        .replace(/\{3\}/ig, convertEmptyStr(comData[i]["GDNAME"]))
                        .replace(/\{4\}/ig, convertEmptyStr(comData[i]["GDSPEC"]))
                        .replace(/\{5\}/ig, convertEmptyStr(comData[i]["SSSL"]))
                        .replace(/\{6\}/ig, convertEmptyStr(comData[i]["UNITNAME"]))
                        .replace(/\{7\}/ig, comData[i]["BZSL"])
                        .replace(/\{8\}/ig, comData[i]["HSJJ"])
                        .replace(/\{9\}/ig, comData[i]["HSJE"])
                        .replace(/\{10\}/ig, convertEmptyStr(comData[i]["PH"]))
                        .replace(/\{11\}/ig, convertEmptyStr(comData[i]["RQ_SC"]))
                        .replace(/\{12\}/ig, convertEmptyStr(comData[i]["YXQZ"]))
                        .replace(/\{13\}/ig, convertEmptyStr(comData[i]["PZWH"]))
                        .replace(/\{14\}/ig, convertEmptyStr(comData[i]["PRODUCERNAME"]))
                        .replace(/\{15\}/ig, convertEmptyStr(comData[i]["MEMO"]))
                        newData.push(content);
                    }
                    renderCom(newData, noBtn)

                    var newZsmData = new Array();
                    for (var j = 0; j < zsmData.length; j++) {
                        var content = DATAZSM
                        .replace(/\{0\}/ig, convertEmptyStr(zsmData[j]["FLAGNAME"]))
                        .replace(/\{1\}/ig, convertEmptyStr(zsmData[j]["GDNAME"]))
                        .replace(/\{2\}/ig, convertEmptyStr(zsmData[j]["ONECODE"]))
                        newZsmData.push(content);
                    }
                    
                    F(DataList1).loadData(newZsmData);

                }
            });
        }

        //扫描唯一码事件
        function onScan(e){
            var ONECODE = F(docONECODE).getValue();
            var BILLNO = F(docBILLNO).getValue()
            var newData = F(GoodsList).data || new Array();

            toggleMask()
            $.ajax('/ERPStorage/GoodsStorage.aspx?oper=scan&onecode=' + ONECODE + '&billno=' + BILLNO, {
                success: function (data) {
                    toggleMask(true)
                    var resultData = eval('(' + data + ')');
                    var objData = resultData["data"];
                    var result = resultData["result"];
                    var zsmData = resultData["zsm"];
                    if (result == "fail") {
                        F.alert(objData);
                        F(docONECODE).setValue('');
                        return;
                    }
                    zsmData = eval('(' + zsmData + ')');
                        var newZsmData = new Array();
                        for (var j = 0; j < zsmData.length; j++) {
                            var content = DATAZSM
                            .replace(/\{0\}/ig, convertEmptyStr(zsmData[j]["FLAGNAME"]))
                            .replace(/\{1\}/ig, convertEmptyStr(zsmData[j]["GDNAME"]))
                            .replace(/\{2\}/ig, convertEmptyStr(zsmData[j]["ONECODE"]))
                            newZsmData.push(content);
                        }

                    F(DataList1).loadData(newZsmData);
                    F(docONECODE).setValue('');
                }
            });
        }

        //生成单据详情表体事件
        function renderCom(newData,noBtn) {
            F(GoodsList).loadData(newData);
            if (!noBtn) {
                F(GoodsList).el.find('.f-datalist-item-inner .item-table .actions-goods').each(function () {
                    F.create({
                        type: 'button',
                        text: '删除',
                        scale: 'large',
                        cls: 'goodsbtn',
                        renderTo: this,
                        iconFont: 'close'
                    });
                });
            }
        }
        function newBill() {
            changeEditStatus(true)
            F(DetailForm).reset();
            var newData = new Array();
            var currDate = new Date();
            F(docLRRQ).setValue(currDate);
            F(docLRY).setValue(UserAction.UserID);
            F(docFLAG).setValue("M");
            renderCom(newData);
            //F(btnNew).enable();
            //F(btnSave).enable();
            //F(btnSubmit).disable();
        }

        function submitBill() {
            var DEPTID = F(lstDEPTID).getValue();
            var BILLNO = F(docBILLNO).getValue();
            var DDBH = F(docDDBH).getValue();
            F.confirm('_top', '是否确认审核该单据？', '提示', 'question',function(){
                if(F.validateForm(DetailForm)){
                    if(F(GoodsList).data.length>0){
                        var data = 
                        formToJson(DetailForm);
                        toggleMask()
                        $.ajax('/ERPStorage/GoodsStorage.aspx?oper=submit&DEPTID=' + DEPTID + '&billno=' + BILLNO+ '&DDBH='+DDBH,{
                            type:'POST',
                            data:data,
                            success:function(data){
                                toggleMask(true)
                                var resultData = eval('(' + data + ')');
                                var objData = resultData["data"];
                                var result = resultData["result"];
                                if (result == "fail") {
                                    F.alert(objData);
                                    return;
                                }else{
                                    var billno = objData;
                                    billOpen(billno);
                                }
                                F.alert('单据【'+billno+'】审核成功')
                            }
                        })
                    }else{
                        F.alert('单据明细不能为空！');
                    }
                }
            })
        }



        F.ready(function () {
            F(GoodsList).el.on('touchend', '.f-datalist-item-inner .goodsbtn', function (e) {
                var btnEl = $(this), innerEl = btnEl.parents('.f-datalist-item-inner');
                var myguid = innerEl.find('.item-guid').text();
                var newData = new Array();
                for (var i = 0; i < F(GoodsList).data.length ; i++) {
                    if (F(GoodsList).data[i].indexOf(myguid) == -1) {
                        newData.push(F(GoodsList).data[i])
                    }
                }
                renderCom(newData)
            });
            F(billList).el.on('touchend', '.f-datalist-item-inner .mybtn', function (e) {
                var btnEl = $(this), innerEl = btnEl.parents('.f-datalist-item-inner');
                var code = innerEl.find('.item-code').text();
                onSlideRightClick(e,code);

            });
            $(document).unbind('keydown');
            F(docONECODE).el.find('input').unbind('input');
            F(docONECODE).el.find('input').unbind('keydown');
            var _onecode = '';
            $('#' + docONECODE).find('input[type=text]').on('keydown', function (e) {
                e.preventDefault();
                e.stopPropagation()
                var key = e.which ||e.keyCode;
                
                if (key == "13") {
                   
                    //
                    $(e.currentTarget).val(_onecode)
                    onScan(e);
                    _onecode='';
                }else{
                    _onecode+=String.fromCharCode(e.keyCode)
                }
            });
            var currDate = new Date();
            var yestedayDate = new Date();
            yestedayDate.setTime(currDate.getTime() - 24 * 60 * 60 * 1000*30)
            F(lstLRRQ1).setValue(yestedayDate)
            F(lstLRRQ2).setValue(currDate)
            replaceDateControl(lstLRRQ1)
            replaceDateControl(lstLRRQ2)
            if (!F(docLRRQ).el.hasClass('ui-state-disabled')) {
                replaceDateControl(docLRRQ)
            }

            if (!F(docSHRQ).el.hasClass('ui-state-disabled')) {
                replaceDateControl(docSHRQ)
            }
            
            bindList();
        })
       
    </script>
</body>
</html>
