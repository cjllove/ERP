<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrintTemplate.aspx.cs" Inherits="ERPProject.ERPAssist.PrintTemplate" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <script src="../res/js/CreateControl.js" type="text/javascript"></script>
    <script src="../res/js/GRInstall.js" type="text/javascript"></script>
</head>
<body>
    <div style="width: 0px; height: 0px">
        <script type="text/javascript">
            CreateDisplayViewerEx("0%", "0%", "", "", false, "");
        </script>
    </div>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" ShowBorder="false" Layout="Anchor" ShowHeader="False" runat="server" BodyPadding="10px">
            <Items>
                <f:Panel ID="Panel3" ShowBorder="false" BodyPadding="0px" Layout="Anchor" ShowHeader="False" runat="server">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar2" runat="server" ToolbarAlign="Right">
                            <Items>
                                <f:Button ID="btnBarcodeRecoveryBoard" Icon="Printer" Text="打 印" runat="server" OnClientClick="btnBarcode_onclick()" OnClick="btnBarcode_click"></f:Button>
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Items>
                        <f:GroupPanel ID="gpPanel" runat="server" Title="定数条码回收板打印">
                            <Items>
                                <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                    ShowHeader="False" LabelWidth="70px" runat="server">
                                    <Rows>
                                        <f:FormRow ColumnWidths="65% 25% 10%">
                                            <Items>
                                                <%--<f:TextBox ID="tbxHeader" runat="server" Label="表头"></f:TextBox>--%>
                                                <f:DropDownList ID="lstDEPTID" runat="server" Label="科室" EnableEdit="true" ForceSelection="true" AutoPostBack="true" OnSelectedIndexChanged="lstDEPTID_SelectedIndexChanged" ShowRedStar="true" />
                                                <f:NumberBox ID="NumberBox1" runat="server" Label="序号" MinValue="1" NoDecimal="true" Required="true" ShowRedStar="true" CompareType="Int"></f:NumberBox>
                                                <f:NumberBox ID="numCoding" runat="server" Label="至" MinValue="2" NoDecimal="true" Required="true" ShowRedStar="true" CompareControl="NumberBox1" CompareType="Float" CompareOperator="GreaterThanEqual"></f:NumberBox>
                                                <f:DropDownList ID="lstISGZ" runat="server" Label="是否高值" OnSelectedIndexChanged="lstISGZ_SelectedIndexChanged" AutoPostBack="true">
                                                    <f:ListItem Text="非高值" Value="N" Selected="true" />
                                                    <f:ListItem Text="高值" Value="Y" />
                                                </f:DropDownList>
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                            </Items>
                        </f:GroupPanel>
                        <f:GroupPanel ID="GroupPanel1" runat="server" Title="服务器CPU序列号" BodyPadding="10px">
                            <Items>
                                <f:Label ID="lblCPUID" runat="server" Label="CPU序列号"></f:Label>
                            </Items>
                        </f:GroupPanel>
                    </Items>
                </f:Panel>
            </Items>
        </f:Panel>
    </form>
    <script type="text/javascript">
        function btnBarcode_onclick() {
            var dept = F('<%= lstDEPTID.ClientID%>').getValue();
            if (dept == "") {
                F.alert("请选择要打印条码回收板的科室！");
                return;
            }
            var num1 = F('<%= NumberBox1.ClientID%>').getValue();
            var num = F('<%= numCoding.ClientID%>').getValue();
            var isgz = F('<%= lstISGZ.ClientID%>').getValue();
            var url = "/grf/phtmhsb.grf?timestamp=" + new Date().getTime();
            if (isgz == "Y") {
                url = "/grf/gztmhsb.grf?timestamp=" + new Date().getTime();
            }
            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = url;
            var dataurl = "/captcha/PrintReport.aspx?Method=GetGZBarCode&d=";
            dataurl = dataurl + dept + "&n=" + num + "&isgz=" + isgz + "&m=" + num1;
            console.log(dataurl);
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }

    </script>
</body>
</html>
