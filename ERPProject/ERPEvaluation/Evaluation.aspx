<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Evaluation.aspx.cs" Inherits="ERPProject.ERPEvalution.Evaluation" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>科室评价管理</title>
    <script src="../res/js/start.js"></script>
    <style type="text/css" media="all">
        #rateStatus {
            float: left;
            clear: both;
            width: 100%;
            height: 30px;
        }

        #rateMe {
            clear: both;
            width: 100%;
            width: 100%;
            margin-left: 20px;
        }

            #rateMe li {
                float: left;
                list-style: none;
            }

                #rateMe li a:hover,
                #rateMe .on {
                    background: url(../res/images/2.png) no-repeat;
                    width: 40px;
                    height: 50px;
                }

            #rateMe a {
                float: left;
                background: url(../res/images/1.png) no-repeat;
                margin-left: 40px;
                width: 40px;
                height: 50px;
            }

        #ratingSaved {
            display: none;
        }

        .saved {
            color: red;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="TabStrip1" OnCustomEvent="PageManager1_CustomEvent"
            runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="0" runat="server">
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
                                                <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="bntSearch_Click" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="75px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="lstDEPTID" runat="server" Label="评价科室" EnableEdit="true" ForceSelection="true">
                                                        </f:DropDownList>
                                                        <f:DropDownList ID="lstFLAG" runat="server" Label="评价状态">
                                                            <f:ListItem Text="--请选择--" Value="" Selected="true" />
                                                            <f:ListItem Text="未评价" Value="N" />
                                                            <f:ListItem Text="已评价" Value="Y" />
                                                        </f:DropDownList>
                                                        <%--<f:DropDownList ID="lstPJDJ" runat="server" Label="评价等级" EnableEdit="true" ForceSelection="true" />--%>
                                                        <f:TriggerBox ID="tgbPJYF" runat="server" Label="评价月份" TriggerIcon="Date" EmptyText="请选择月份" ShowRedStar="true" Required="true"></f:TriggerBox>
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" AnchorValue="100% -82" ShowBorder="false" ShowHeader="false" EnableTextSelection="true"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" OnRowDoubleClick="GridList_RowDoubleClick"
                                    DataKeyNames="SEQNO" EnableRowDoubleClickEvent="true" EnableColumnLines="true"
                                    AllowSorting="true" EnableHeaderMenu="true">
                                    <Columns>
                                        <f:RowNumberField Width="30px" TextAlign="Center" />
                                        <f:BoundField DataField="DEPTID" Hidden="true" />
                                        <f:BoundField DataField="SEQNO" Hidden="true" />
                                        <f:BoundField Width="120px" DataField="DEPTIDNAME" HeaderText="评价科室" />
                                        <f:BoundField Width="80px" DataField="PJYF" HeaderText="评价月份" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="FLAGNAME" HeaderText="评价状态" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="RES" HeaderText="评价分数" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="PJRNAME" HeaderText="评价人" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="PJRQ" HeaderText="评价日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="80px" DataField="SHRNAME" HeaderText="提交人" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="SHRQ" HeaderText="提交日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="180px" DataField="MEMO" HeaderText="备注" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="单据信息" Icon="PageWord" runat="server">
                    <Items>
                        <f:Panel ID="Panel5" Height="980px" Layout="Fit" AnchorValue="100%" runat="server" Margin="0 0 0 0" AutoScroll="false" ShowBorder="false" ShowHeader="false" RegionPosition="Top" EnableIFrame="true" IFrameUrl="../ERPEvaluation/EvaluationFm.aspx">
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
        <f:HiddenField ID="hdfValue" runat="server"></f:HiddenField>
        <f:HiddenField ID="hdfcount" runat="server"></f:HiddenField>
    </form>
    <script src="../res/my97/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        var tbxMyBoxClientID = '<%= tgbPJYF.ClientID %>';
        F.ready(function () {
            var tbxMyBox = F(tbxMyBoxClientID);
            tbxMyBox.onTriggerClick = function () {
                WdatePicker({
                    el: tbxMyBoxClientID + '-inputEl',
                    dateFmt: 'yyyy-MM',
                    onpicked: function () {
                        tbxMyBox.validate();
                    }
                });
            };
        });
        function btnSave() {
            for (var i = 0; i < F('<%= hdfcount.ClientID%>').getValue() ; i++) {
                F('<%= hdfValue.ClientID%>').f_setValue(F('<%= hdfValue.ClientID%>').getValue() + document.getElementById("Project" + (i + 1)).innerHTML + "k" + document.getElementById("rateNow" + (i + 1)).innerHTML + ",");
            }
            F.customEvent('Save');
        }
        function refreshTab() {
            var mainTabStrip = parent.Ext.getCmp("TabStrip1");
            var myTab = mainTabStrip.getTab("tab_clientID");
            if (myTab != null) {
                //mainTabStrip.setActiveTab(myTab);
                var iframe = Ext.DomQuery.selectNode('iframe', myTab.body.dom);
                iframe.contentWindow.location.reload(false);
            }
        }
    </script>
</body>
</html>
