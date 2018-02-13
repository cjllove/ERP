<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InventorySheet.aspx.cs" Inherits="ERPProject.pad.InventorySheet" %>

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
        
        .item-table .item-code {
            min-width:120px;
        }
        .item-table .item-status {
            min-width:50px;
            text-align:center;
        }
        .item-table .item-lry {
            min-width:190px;
        }
        
        .item-table .item-status.M {
            color:white;
            background-color:#3AA02C;  
        }
        .item-table .item-status.R {
            color:white;
            background-color:#AF5553;  
        }
        .item-table .item-status.N {
            color:white;
            background-color:#3AA02C;  
        }
        .actions-goods input {
            width:100px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1"  runat="server" OnCustomEvent="PageManager1_CustomEvent" />
        <f:Panel ID="Panel1" ShowBorder="false" ShowHeader="false" Layout="Fit" BoxConfigChildMargin="0 0 5px 0" BodyPadding="10px" runat="server" AutoScroll="true">
            <Toolbars>
                <f:Toolbar runat="server" HeaderStyle="true" >
                    <Items>
                        <f:Button runat="server" Text="" Size="Large" EnablePostBack="false" EnableDefaultState="false" IconFont="Home"  ID="Button1">
                            <Listeners>
                                <f:Listener Event="click" Handler="home" />
                            </Listeners>
                        </f:Button>
                        <f:Label runat="server" Text="盘点单列表" CssClass="large-font ml-10"></f:Label>
                        <f:ToolbarFill runat="server"></f:ToolbarFill>
                        <f:Button runat="server" Text="条件查询" Size="Large" EnablePostBack="false" EnableDefaultState="false" IconFont="Search" >
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
                            <p> 如需查询数据或更改查询条件请点击 [条件查询] 进行筛选查询</p>
                        </f:ContentPanel>
                    </Items>
                </f:Panel>
                <f:DataList runat="server" ID="BillList" Hidden="true" >
                </f:DataList>
            </Items>
        </f:Panel>
        <f:Panel runat="server" Layout="VBOX" ID="Panel2" ShowHeader="false"  ShowBorder="false" Hidden="true" AutoScroll="true" IsViewPort="true">
            <Toolbars>
                        <f:Toolbar runat="server" Position="Bottom">
                            <Items>
                                <f:ToolbarFill runat="server"></f:ToolbarFill>
                                <f:TextBox ID="sumKCSL" runat="server" Width="180px" Label="合计库存" LabelWidth="80px" Readonly="true"></f:TextBox>
                                <f:TextBox ID="sumPDSL" runat="server" Width="180px" Label="合计盘点" LabelWidth="80px" Readonly="true"></f:TextBox>
                            </Items>
                        </f:Toolbar>
                        <f:Toolbar runat="server" HeaderStyle="true" >
                        
                                <Items>
                                    <f:Button runat="server" Text="返回" Size="Large" EnablePostBack="false" EnableDefaultState="false" IconFont="ArrowLeft" >
                                        <Listeners>
                                            <f:Listener Event="click" Handler="onSlideLeftClick" />
                                        </Listeners>
                                    </f:Button>
                                    <f:Label runat="server" Text="盘点单明细" CssClass="large-font ml-10"></f:Label>
                                    <f:ToolbarFill runat="server"></f:ToolbarFill>
                                    <f:Button runat="server" ID="btnLock" Text="锁定库存" Size="Large" EnablePostBack="false" EnableDefaultState="false" IconFont="Lock">
                                        <Listeners>
                                            <f:Listener Event="click" Handler="lockStorage" />
                                        </Listeners>
                                    </f:Button>
                                    
                                    
                                    <f:Button runat="server" ID="btnSave" Text="保存" Size="Large" EnablePostBack="false" EnableDefaultState="false" IconFont="Save">
                                            <Listeners>
                                                <f:Listener Event="click" Handler="save" />
                                            </Listeners>
                                        </f:Button>
                                    <f:Button runat="server" ID="btnSubmit" Text="审核" Size="Large" EnablePostBack="false" EnableDefaultState="false" IconFont="Check">
                                            <Listeners>
                                                <f:Listener Event="click" Handler="submitBill" />
                                            </Listeners>
                                        </f:Button>
                                    <f:Button runat="server" ID="btnCancel" Text="驳回" Size="Large" EnablePostBack="false" EnableDefaultState="false" IconFont="Close">
                                        <Listeners>
                                            <f:Listener Event="click" Handler="rejectBill" />
                                        </Listeners>
                                    </f:Button>
                                </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Items>
                        <f:form runat="server" ID="DetailForm" ShowBorder="false" ShowHeader="false" BodyPadding="10" LabelWidth="90px" CssStyle="border-bottom:1px solid #99bce8;">
                            <Rows>
                                    <f:FormRow>
                                        <Items>
                                            <f:TextBox ID="tbxPDPLAN" runat="server" Label="计划单号" >
                                            </f:TextBox>
                                            <f:DatePicker ID="dpkPDRQ" runat="server" Label="盘点日期"></f:DatePicker>
                                            <f:TextBox ID="tbxSEQNO" runat="server" Label="盘点单号"></f:TextBox>
                                            <f:DropDownList ID="ddlFLAG" runat="server" Label="单据状态"  ForceSelection="true">
                                            </f:DropDownList>
                                        </Items>
                                    </f:FormRow>
                                    <f:FormRow>
                                        <Items>
                                            <f:DropDownList ID="ddlPDTYPE" runat="server" Label="盘点类别" AutoPostBack="true">
                                                <f:ListItem Value="1" Text="1-普通盘点" Selected="true" />
                                                <f:ListItem Value="2" Text="2-动销盘点" />
                                                <%--<f:ListItem Value="3" Text="3-月末盘点" />
                                                <f:ListItem Value="4" Text="4-APP盘点" EnableSelect="false" />--%>
                                            </f:DropDownList>
                                            <f:DropDownList ID="ddlISPH" runat="server" Label="批次管理">
                                                <f:ListItem Text="是" Value="Y" Selected="true" />
                                                <f:ListItem Text="否" Value="N" />
                                            </f:DropDownList>
                                            <f:DropDownList ID="ddlLRY" runat="server" Label="录入员" ForceSelection="true">
                                            </f:DropDownList>
                                            <f:DatePicker ID="dpkLRRQ" runat="server" Label="录入日期"></f:DatePicker>
                                        </Items>
                                    </f:FormRow>
                                    <f:FormRow>
                                        <Items>
                                            <f:DropDownList runat="server" Label="锁定库存" ID="ddlISSD" Enabled="false">
                                                <f:ListItem Text="未锁定" Value="N" />
                                                <f:ListItem Text="已锁定" Value="Y" />
                                            </f:DropDownList>
                                            <f:DropDownList ID="ddlDEPTID" runat="server" Label="部　　门"></f:DropDownList>
                                            <f:DropDownList ID="ddlSPR" runat="server" Label="审核员"  ForceSelection="true">
                                            </f:DropDownList>
                                            <f:DatePicker ID="dpkSPRQ" runat="server" Label="审核日期"></f:DatePicker>
                                        </Items>
                                    </f:FormRow>
                                    <f:FormRow Hidden="true">
                                        <Items>
                                            <f:TextBox ID="tbxSYDBILL" runat="server" Label="损益单号" Hidden="true"></f:TextBox>
                                            <f:TextBox ID="tbxMEMO" runat="server" Label="备　注" Hidden="true"></f:TextBox>
                                        </Items>
                                    </f:FormRow>
                                    <f:FormRow Hidden="true">
                                        <Items>
                                            <f:DatePicker ID="dpkENDRQ" runat="server" Hidden="true"></f:DatePicker>
                                            <f:DatePicker ID="dpkBEGINRQ" runat="server" Hidden="true"></f:DatePicker>
                                            <f:TextBox ID="tbxBILLNO" runat="server" Hidden="true"></f:TextBox>
                                        </Items>
                                    </f:FormRow>
                                </Rows>
                        </f:form>


                        <f:DataList runat="server" ID="GoodsList" BoxFlex="1">
                        </f:DataList>
                
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
                                        <f:FormRow ColumnWidths="50% 50%">
                                            <Items>
                                                <f:TriggerBox ID="lstBILLNO" runat="server" Label="盘点单号" MaxLength="20" EmptyText="输入盘点编号查询"  />
                                                <f:TriggerBox ID="lstBILLPLAN" runat="server" Label="计划盘点单号" MaxLength="20" EmptyText="输入盘点计划单号查询" />
                                                
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow ColumnWidths="50% 50%">
                                            <Items>
                                                <f:DatePicker ID="lstLRRQ1" runat="server" Label="盘点日期" Required="true" ShowRedStar="true" />
                                                <f:DatePicker ID="lstLRRQ2" runat="server" Label="　　至" Required="true" CompareType="String" />
                                                <%--CompareControl="lstLRRQ1" CompareOperator="GreaterThanEqual" CompareMessage="结束日期应该大于开始日期！" --%>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow ColumnWidths="50% 50%">
                                            <Items>
                                                <f:DropDownList ID="lstDEPTOUT" runat="server" Label="生成科室" ForceSelection="true" />
                                                <f:TextBox Hidden="true" runat="server"></f:TextBox>
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
        var EmptyPanel = '<%=EmptyPanel.ClientID%>';
        var billList = '<%=BillList.ClientID%>';
        var FormQuery = '<%=FormQuery.ClientID%>';
        var lstLRRQ1 = '<%=lstLRRQ1.ClientID%>';
        var lstLRRQ2 = '<%=lstLRRQ2.ClientID%>';
        var Panel1 = '<%=Panel1.ClientID%>';
        var Panel2 = '<%=Panel2.ClientID%>';
        var ddlFLAG = '<%=ddlFLAG.ClientID%>';
        var GoodsList = '<%=GoodsList.ClientID%>';
        var DetailForm = '<%=DetailForm.ClientID%>';  
        var btnSave = '<%=btnSave.ClientID%>';
        var btnCancel = '<%=btnCancel.ClientID%>';
        var btnLock = '<%=btnLock.ClientID%>';
        var btnSubmit = '<%=btnSubmit.ClientID%>';
        var ddlISSD = '<%=ddlISSD.ClientID%>';
        var tbxSEQNO = '<%=tbxSEQNO.ClientID%>';
        var sumKCSL = '<%=sumKCSL.ClientID%>';
        var sumPDSL = '<%=sumPDSL.ClientID%>';
        function calcSum() {
            var ERPSL = 0;
            var sKCSL = 0;
            F(GoodsList).el.find('.f-datalist-item-inner .item-table .item-rowcode').each(function (i) {
                var $this = $(this)
                var rowcode = $this.text();

                var innerEl = $this.parents('.f-datalist-item-inner');
                var pdsl = innerEl.find('.actions-goods').attr('data-qty');

                ERPSL += parseFloat(pdsl == "" ? 0 : pdsl)
                var kcsl = innerEl.find('.item-kcsl-value').text();
                sKCSL += parseFloat(kcsl == "" ? 0 : kcsl)
            })
            F(sumKCSL).setValue(sKCSL)
            F(sumPDSL).setValue(ERPSL)
            
        }

        function rejectBill() {
            F.confirm('_top', '确认要驳回吗？', '提示', 'question', confirmed)
            function confirmed() {
                var data =
                        formToJson(DetailForm);
                toggleMask()
                $.ajax('/ERPInventory/InventorySheet.aspx?osid=cancel', {
                    type: 'POST',
                    data: data,
                    success: function (data) {
                        toggleMask(true)
                        var resultData = eval('(' + data + ')');
                        var objData = resultData["data"];
                        var result = resultData["result"];
                        if (result == "fail") {
                            F.alert(objData);
                            return;
                        } else {
                            var billno = objData;
                            billOpen(billno);
                        }
                        F.alert('驳回成功')
                    }
                })
            }
            
        }
        function submitBill() {
            F.confirm('_top', '确认要审核吗？', '提示', 'question', confirmed)
            function confirmed() {
                var data =
                        formToJson(DetailForm);
                toggleMask()
                $.ajax('/ERPInventory/InventorySheet.aspx?osid=audit', {
                    type: 'POST',
                    data: data,
                    success: function (data) {
                        toggleMask(true)
                        var resultData = eval('(' + data + ')');
                        var objData = resultData["data"];
                        var result = resultData["result"];
                        if (result == "fail") {
                            F.alert(objData);
                            return;
                        } else {
                            var billno = objData;
                            billOpen(billno);
                        }
                        F.alert('审核成功')
                    }
                })
            }
        }
        function save() {
            var newData = new Array();
            F(GoodsList).el.find('.f-datalist-item-inner .item-table .item-rowcode').each(function (i) {
                var $this = $(this)
                var rowcode = $this.text();
                //newData.push($this.text())

                var innerEl = $this.parents('.f-datalist-item-inner');
                var pdsl = innerEl.find('.actions-goods input').val();
                var mydata = rowcode + ":" + pdsl;
                newData.push(mydata);
                
            })
            var data =
                    formToJson(DetailForm);
            data["com"] = newData;
            toggleMask()
            $.ajax('/ERPInventory/InventorySheet.aspx?osid=save', {
                type: 'POST',
                data: data,
                success: function (data) {
                    toggleMask(true)
                    var resultData = eval('(' + data + ')');
                    var objData = resultData["data"];
                    var result = resultData["result"];
                    if (result == "fail") {
                        F.alert(objData);
                        return;
                    } else {
                        var billno = objData;
                        billOpen(billno);
                    }
                    F.alert('保存成功')
                }
            })
        }
        function lockStorage() {
            var billno = F(tbxSEQNO).getValue();
            var flag = F(ddlFLAG).getValue();
            toggleMask()
            $.ajax('/ERPInventory/InventorySheet.aspx?osid=lock&billno=' + billno+'&flag='+flag, {
                success: function (data) {
                    toggleMask(true)
                    var resultData = eval('(' + data + ')');
                    var objData = resultData["data"];
                    var result = resultData["result"];
                    if (result == "fail") {
                        F.alert(objData);
                        return;
                    }
                    F.alert(objData);
                    billOpen(billno);
                    //objData = eval('(' + objData + ')');
                }
            });
        }


        function changeEditStatus(canEdit) {
            if (!canEdit) {
                //F(docDEPTID).disable();
                //F(docOPTTABLE).disable();
                //F(docCUSTID).disable();
                //F(docOPTID).disable();
                //F(docMEMO).disable();
                //F(docXSRQ).disable();
                //F(docONECODE).disable();
                //F(docOPTDATE).disable();

            } else {
                //F(docDEPTID).enable();
                //F(docOPTTABLE).enable();
                //F(docCUSTID).enable();
                //F(docOPTID).enable();
                //F(docMEMO).enable();
                //F(docXSRQ).enable();
                //F(docONECODE).enable();
                //F(docOPTDATE).enable();

            }
        }
        function newBill() {
            changeEditStatus(true)
            F(DetailForm).reset();
            //var newData = new Array();
            //var currDate = new Date();
            //F(docLRRQ).setValue(currDate);
            //F(docLRY).setValue(UserAction.UserID);
            //F(docFLAG).setValue("M");
            //renderCom(newData);
            //F(btnDel).disable();
            //F(btnNew).enable();
            //F(btnSave).enable();
            //F(btnSubmit).disable();
        }

        function billOpen(billno) {
            newBill();
            toggleMask()
            $.ajax('/ERPInventory/InventorySheet.aspx?osid=queryBill&billno=' + billno, {
                //async:false,
                success: function (data) {
                    toggleMask(true)
                    var resultData = eval('(' + data + ')');

                    var docData = resultData["doc"];
                    var comData = resultData["com"];
                    var result = resultData["result"];
                    if (result == "fail") {
                        F.alert("获取数据失败！");
                        return;
                    }
                    docData = eval('(' + docData + ')');
                    comData = eval('(' + comData + ')');
                    var canEdit = true;
                    var noBtn = false;
                    /*
                    if (ddlISSD.SelectedValue.Equals("N")) {
                        btnSave.Enabled = false;
                        btnAudit.Enabled = false;
                        btnCancel.Enabled = false;
                        btnPrint.Enabled = false;
                        btnSD.Enabled = true;
                        btnGoods.Enabled = false;
                        btnRept.Enabled = false;
                    }
                    else if (ddlFLAG.SelectedValue == "N") {
                        btnSave.Enabled = true;
                        btnAudit.Enabled = true;
                        btnCancel.Enabled = true;
                        btnPrint.Enabled = true;
                        btnSD.Enabled = false;
                        btnGoods.Enabled = true;
                        btnRept.Enabled = true;
                    }
                    else {
                        btnSave.Enabled = false;
                        btnAudit.Enabled = false;
                        btnCancel.Enabled = false;
                        btnPrint.Enabled = false;
                        btnSD.Enabled = false;
                        btnGoods.Enabled = false;
                        btnRept.Enabled = false;
                    }
                    */

                   
                    
                    //F(ListData).setValue(objData);
                    setFormValues(DetailForm, docData)
                    if (F(ddlISSD).getValue() == "N") {
                        F(btnSave).disable();
                        F(btnSubmit).disable();
                        F(btnCancel).disable();
                        F(btnLock).enable();
                        noBtn = true;
                    }
                    else if (docData[0]["FLAG"] == "N" ) {
                        noBtn = false;
                        F(btnSave).enable();
                        F(btnSubmit).enable();
                        F(btnCancel).enable();
                        F(btnLock).disable();
                        //F(btnDel).disable();
                        //F(btnNew).enable();
                        //F(btnSave).disable();
                        //F(btnSubmit).disable();
                    } else {
                        noBtn = true;
                        F(btnSave).disable();
                        F(btnSubmit).disable();
                        F(btnCancel).disable();
                        F(btnLock).disable();
                        //F(btnDel).enable();
                        //F(btnNew).enable();
                        //F(btnSave).enable();
                        //F(btnSubmit).enable();
                    }
                    canEdit = !noBtn;
                    changeEditStatus(canEdit);
                    var newData = new Array();
                    for (var i = 0; i < comData.length; i++) {
                        var content = DATAGOODS_TEMPLATE
                           .replace(/\{0\}/ig, convertEmptyStr(comData[i]["HWID"]))
                           .replace(/\{1\}/ig, guid())
                        .replace(/\{2\}/ig, convertEmptyStr(comData[i]["GDSEQ"]))
                        .replace(/\{3\}/ig, convertEmptyStr(comData[i]["GDNAME"]))
                        .replace(/\{4\}/ig, convertEmptyStr(comData[i]["GDSPEC"]))
                         .replace(/\{5\}/ig, convertEmptyStr(comData[i]["UNITNAME"]))
                        .replace(/\{6\}/ig, comData[i]["BZSL"])
                        .replace(/\{7\}/ig, comData[i]["HSJJ"])
                        .replace(/\{8\}/ig, comData[i]["HSJE"])
                         .replace(/\{9\}/ig, convertEmptyStr(comData[i]["PH"]))
                        .replace(/\{10\}/ig, splitDate(comData[i]["RQ_SC"]))
                        .replace(/\{11\}/ig, splitDate(comData[i]["YXQZ"]))
                        .replace(/\{12\}/ig, convertEmptyStr(comData[i]["PZWH"]))
                         .replace(/\{13\}/ig, convertEmptyStr(comData[i]["PRODUCERNAME"]))
                         .replace(/\{14\}/ig, convertEmptyStr(comData[i]["PDSL"]))
                        .replace(/\{15\}/ig, convertEmptyStr(comData[i]["KCSL"]))
                        .replace(/\{16\}/ig, convertEmptyStr(comData[i]["ROWCODE"]))
                        newData.push(content);
                    }
                    renderCom(newData, noBtn)
                    calcSum();
                }
            });
            //console.log(code)
        }

        function renderCom(newData, noBtn) {
            F(GoodsList).loadData(newData);
            if (!noBtn) {
                F(GoodsList).el.find('.f-datalist-item-inner .item-table .actions-goods').each(function () {
                    $(this).html('');
                    var input = $('<input class="f-field-textbox ui-corner-all ui-widget-content" placeholder="盘点数量" type="number" >')
                    if ($(this).attr('data-qty')!='') {
                        input.val($(this).attr('data-qty'))
                    }
                    $(this).append(input);
                })
                //F(GoodsList).el.find('.f-datalist-item-inner .item-table .actions-goods').each(function () {
                //    F.create({
                //        type: 'button',
                //        text: '删除',
                //        scale: 'large',
                //        cls: 'goodsbtn',
                //        renderTo: this,
                //        iconFont: 'close'
                //    });
                //});
            } 
        }

        //向右滑动
        function onSlideRightClick(event, code) {
            newBill();
            F.slideLeft(Panel1, Panel2);
            if (code) {
                billOpen(code);
            } 
        }
        //向左滑动
        function onSlideLeftClick() {
            var sr = function () {
                F.slideRight(Panel2, Panel1);
                setTimeout(function () { bindList(); }, 500)
            }
            if (F(ddlFLAG).getValue() == "N" ) {
                F.confirm('_top', '确认要返回上级页面吗？未保存的修改将丢失', '提示', 'question', sr)
            } else {
                sr();
            }
        }

        //列表页模板
        var DATALIST_TEMPLATE = "<table class=\"item-table\"><tr><td width='93%'><div><span class=\"item-code\">{0}</span><span class=\"item-status {2}\">{1}</span><span>盘点日期：{4}</span><span>盘点科室：{3}</span></div>"
            + "<div><span class='item-lry'>录入员：{5}</span><span >录入日期：{6}</span><span class='item-shy'>审核员：{7}</span><span >审核日期：{8}</span></div>"
            + "<div><span >计划单号：{9}</span></div>"
            + "</td><td class=\"actions\"></td></tr></table>";
        var DATAGOODS_TEMPLATE = "<table class=\"item-table\"><tr><td width='70%'><div><span style='display:none' class='item-guid'>{1}</span><span style='display:none' class='item-rowcode'>{16}</span><span class='item-gdname'>{3}</span><span class='item-gdseq'>商品编码：{2}</span><span class='item-gdspec'>规格：{4}</span></div>"
            + "<div><span>货位：{0}</span><span class='item-unit'>单位：{5}</span><span class='item-hsjj'>价格：{7}</span><span >生产厂家：{13}</span></div>"
            + "<div><span class='item-ph'>批号：{9}</span><span class='item-rqsc'>生产日期：{10}</span><span class='item-yxqz'>有效期至：{11}</span><span class='item-pzwh'>批准文号：{12}</span></div>"
            + "</td width='15%'><td><div><span class='item-kcsl'>库存：<b class='item-kcsl-value'>{15}</b></span></div>"
            + "</td><td width='15%' class=\"actions-goods\" data-qty='{14}'><span>盘点数：{14}</span></td></tr></table>";
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
            bindList();
            F(WindowQuery).hide();
        }

        function bindList() {
            toggleMask()
            $.ajax('/ERPInventory/InventorySheet.aspx?osid=querylist', {
                data: formToJson(FormQuery),
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
                    for (var i = 0; i < objData.length; i++) {
                        var content = DATALIST_TEMPLATE
                            .replace(/\{0\}/ig, convertEmptyStr(objData[i]["SEQNO"]))
                            .replace(/\{1\}/ig, convertEmptyStr(objData[i]["FLAGNAME"]))
                            .replace(/\{2\}/ig, convertEmptyStr(objData[i]["FLAG"]))
                            .replace(/\{3\}/ig, convertEmptyStr(objData[i]["DEPTIDNAME"]))
                            .replace(/\{4\}/ig, splitDate(objData[i]["PDRQ"]))
                            .replace(/\{5\}/ig, convertEmptyStr(objData[i]["LRYNAME"]))
                            .replace(/\{6\}/ig, splitDate(objData[i]["LRRQ"]))
                            .replace(/\{7\}/ig, convertEmptyStr(objData[i]["SPRNAME"]))
                            .replace(/\{8\}/ig, splitDate(objData[i]["SPRQ"]))
                            .replace(/\{9\}/ig, convertEmptyStr(objData[i]["PDPLAN"]))
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

        F.ready(function () {
            F(billList).el.on('touchend', '.f-datalist-item-inner .mybtn', function (e) {
                var btnEl = $(this), innerEl = btnEl.parents('.f-datalist-item-inner');
                var code = innerEl.find('.item-code').text();
                onSlideRightClick(e, code);
            });
            var currDate = new Date();
            var yestedayDate = new Date();
            yestedayDate.setTime(currDate.getTime() - 24 * 60 * 60 * 1000 * 30)
            F(lstLRRQ1).setValue(yestedayDate)
            F(lstLRRQ2).setValue(currDate)
            replaceDateControl(lstLRRQ1)
            replaceDateControl(lstLRRQ2)
            bindList();
        })

    </script>
</body>
</html>
