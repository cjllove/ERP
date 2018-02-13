<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SQEditWindow.aspx.cs" Inherits="SPDProject.CertificateInput.SQEditWindow" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link rel="stylesheet" type="text/css" href="/res/webuploader/webuploader.css" />
    <link rel="stylesheet" type="text/css" href="/res/webuploader/detail/style.css" />
    <link rel="stylesheet" type="text/css" href="/res/fancybox/jquery.fancybox.css" />
    <title></title>
    <style type="text/css">
        .w1000 {
            margin: 0 auto;
            width: 100%;
        }

            .w1000 img {
                width: 100%;
                height: 600px;
            }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="TabStrip1" runat="server" OnCustomEvent="PageManager1_CustomEvent" />
        <f:Panel ID="Panel4" runat="server" ShowBorder="false" EnableCollapse="true"
            Layout="HBox" AutoScroll="true" ShowHeader="false" Title="面板"
            BoxConfigChildMargin="0 5 0 0" BodyPadding="5">
            <Toolbars>
                <f:Toolbar ID="Toolbar2" runat="server">
                    <Items>
                        <f:Label runat="server" ID="flagLbl" Text="授权资料查看" CssStyle="color:white;width:100px; height:30px; background-color:#5CACEE;text-align:center;padding-top:3px;border-radius:5px;margin-left:10px" />
                        <f:Label runat="server" ID="flagLbl1" Hidden="true" Text="授权资料查看" CssStyle="color:white;width:100px; height:30px; background-color:#FF4040;text-align:center;padding-top:3px;border-radius:5px;margin-left:10px" />
                        <f:ToolbarFill ID="ToolbarFill3" runat="server" />
                        <f:Button ID="btnChange" Icon="Add" Text="换 证" EnablePostBack="true" runat="server" OnClick="btnChange_Click" />
                        <f:Button ID="btnSave" Icon="PageSave" Text="保 存" EnablePostBack="true" runat="server" OnClick="btnSave_Click" />
                        <f:Button ID="btnDelete" Icon="Delete" Text="删 除" Hidden="true" EnablePostBack="true" runat="server" OnClick="btnDelete_Click"></f:Button>
                        <f:Button ID="btnSumbit" Icon="Accept" Text="提 交" Hidden="true" ConfirmText="是否确定要提交授权数据?" EnablePostBack="true" runat="server" OnClick="btnSumbit_Click" />
                        <f:Button ID="btnRollBack" Icon="Accept" Text="撤 回" Hidden="true" EnablePostBack="true" runat="server" OnClick="btnRollBack_Click" />
                        <f:Button ID="btnAudit" Icon="UserTick" Text="审 核" Hidden="true" ConfirmText="是否确定要审核授权数据?" EnablePostBack="true" runat="server" OnClick="btnAudit_Click" />
                        <f:Button ID="btnReject" Icon="UserCross" Text="驳 回" Hidden="true" ConfirmText="是否确定要驳回授权数据?" EnablePostBack="true" runat="server" OnClick="btnReject_Click" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Panel ID="Panel6" Title="面板1" Width="600px" Height="400px" runat="server" AnchorValue="100%"
                    BodyPadding="0px" ShowBorder="true" ShowHeader="false" Layout="VBox">
                    <Items>
                        <f:Form ID="FormDoc" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                            ShowHeader="False" LabelWidth="70px" runat="server">
                            <Rows>
                                <f:FormRow ColumnWidths="50% 20% 30%">
                                    <Items>
                                        <f:TriggerBox ID="trgPRODUCER" Enabled="false" runat="server" ShowRedStar="true" Required="true" LabelWidth="90px" Label="生产厂商" TriggerIcon="Search" AutoPostBack="true"></f:TriggerBox>
                                        <f:CheckBox ID="chkisLR" Label="厂家直接授权" OnCheckedChanged="chkisLR_CheckedChanged" AutoPostBack="true" LabelWidth="97px" Checked="true" runat="server"></f:CheckBox>
                                        <f:CheckBox ID="chknoLR" Label="代理商授权" OnCheckedChanged="chknoLR_CheckedChanged" AutoPostBack="true" LabelWidth="90px" runat="server"></f:CheckBox>
                                    </Items>
                                </f:FormRow>
                                <f:FormRow ColumnWidths="50% 20% 30%">
                                    <Items>
                                        <f:TriggerBox ID="trgAGENT" runat="server" LabelWidth="90px" Label="上级代理商" OnTriggerClick="trgAGENT_TriggerClick" TriggerIcon="Search" AutoPostBack="true"></f:TriggerBox>
                                        <f:TriggerBox ID="trgSUPID1" runat="server" LabelWidth="90px" ShowRedStar="true" OnTriggerClick="trgSUPID1_TriggerClick" Required="true" Label="被授权机构" TriggerIcon="Search" AutoPostBack="true"></f:TriggerBox>
                                    </Items>
                                </f:FormRow>
                                <f:FormRow>
                                    <Items>
                                        <f:DatePicker ID="dpkBEGRQ" LabelWidth="90px" runat="server" Label="授权日期" ShowRedStar="true" />
                                        <f:DatePicker ID="dpkENDRQ" runat="server" LabelWidth="90px" Label="截止日期" ShowRedStar="true" />
                                    </Items>
                                </f:FormRow>
                                <f:FormRow>
                                    <Items>
                                        <f:TextArea runat="server" ID="tbxSQREGION" LabelWidth="90px" EmptyText="授权区域" Label="授权区域(最多250个汉字)"
                                            AutoGrowHeight="true" AutoGrowHeightMin="80" AutoGrowHeightMax="100">
                                        </f:TextArea>
                                    </Items>
                                </f:FormRow>
                                <f:FormRow ColumnWidths="50% 50%">
                                    <Items>
                                        <f:TextBox ID="tbxDOCID" runat="server" LabelWidth="90px" Label="档案编号"></f:TextBox>
                                        <f:TextBox ID="tbxMEMO" runat="server" LabelWidth="90px" Label="备注"></f:TextBox>
                                    </Items>
                                </f:FormRow>
                            </Rows>
                        </f:Form>
                        <f:Grid ID="GridList" BoxFlex="1" ShowBorder="false" ShowHeader="false" EnableColumnLines="true"
                            AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;"
                            EnableTextSelection="true" EnableCheckBoxSelect="true"
                            DataKeyNames="GDSEQ,GDNAME,GDSPEC,PRODUCER" EnableHeaderMenu="true" SortField="SEQNO" SortDirection="ASC" AllowPaging="false"
                            AllowSorting="true" EnableMultiSelect="true" KeepCurrentSelection="true">
                            <Columns>
                                <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                <f:BoundField Width="100px" DataField="GDSEQ" HeaderText="商品编号" SortField="GDSEQ" TextAlign="Center" />
                                <f:BoundField Width="210px" DataField="GDNAME" HeaderText="商品名称" SortField="GDNAME" TextAlign="left" />
                                <f:BoundField Width="100px" DataField="GDSPEC" ColumnID="GDSPEC" HeaderText="规格" TextAlign="Center" />
                                <f:BoundField Width="200px" DataField="PRODUCER" ColumnID="PRODUCER" HeaderText="生产厂家" TextAlign="left" ExpandUnusedSpace="true" />
                            </Columns>
                        </f:Grid>
                    </Items>
                </f:Panel>

                <f:Panel ID="Panel7" Title="面板2" BoxFlex="1" Margin="0"
                    runat="server" BodyPadding="5px" ShowBorder="true" ShowHeader="false">
                    <Items>
                        <f:ContentPanel runat="server" ID="ContentPanel1" ShowHeader="false" ShowBorder="false" BoxFlex="1" Height="586px">
                            <div id="wrapper">
                                <div id="container">
                                    <!--头部，相册选择和格式选择-->
                                    <div id="uploader">
                                        <div class="queueList">
                                            <div id="dndArea" class="placeholder">
                                                <div id="filePicker"></div>
                                                <p>或将照片拖到这里，单次最多可选10张</p>
                                            </div>
                                        </div>
                                        <div class="statusBar" style="display: none;">
                                            <div class="progress">
                                                <span class="text">0%</span>
                                                <span class="percentage"></span>
                                            </div>
                                            <div class="info"></div>
                                            <div class="btns">
                                                <div id="filePicker2"></div>
                                                <div class="uploadBtn">开始上传</div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </f:ContentPanel>
                    </Items>
                </f:Panel>

            </Items>
        </f:Panel>

        <f:Window ID="Window2" Title="上级代理商" Hidden="true" EnableIFrame="true" runat="server"
            EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True"
            Width="640px" Height="480px" OnClose="Window2_Close">
        </f:Window>

        <f:Window ID="Window3" Title="被授权机构" Hidden="true" EnableIFrame="true" runat="server"
            EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True"
            Width="640px" Height="480px" OnClose="Window3_Close">
        </f:Window>

        <f:HiddenField ID="hfdGoodsIndex" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdbesup" runat="server" Text="" />
        <f:HiddenField ID="hfdagent" runat="server" Text="" />
        <f:HiddenField ID="hfdValue1" runat="server" Text="" />
        <f:HiddenField ID="hfdValue2" runat="server" Text="" />
        <f:HiddenField ID="hfdURL" runat="server" />
        <f:HiddenField ID="hfdGrantid" runat="server" />
        <f:HiddenField ID="hfdSuptoid" runat="server" />
        <f:HiddenField ID="isChange" runat="server" />
        <f:HiddenField ID="hfdsupid" runat="server" />
    </form>

    <script type="text/javascript" src="/res/webuploader/webuploader.nolog.min.js"></script>
    <script type="text/javascript" src="/res/webuploader/detail/myupload.js"></script>
    <script type="text/javascript" src="/res/fancybox/jquery.fancybox.js"></script>
    <script>
        function save() {
            F('<%= hfdURL.ClientID%>').setValue(JSON.stringify(getData()));
            F.customEvent("mysave");
        }
        function clearUpload() {
            $('#uploader').myUpload("destroy");
        }

        function initUpload(id) {
            $('#uploader').myUpload("destroy");
            $('#uploader').myUpload({
                server: '/captcha/LicenseUploadPictures.ashx',
                formData: {
                    owner: id
                },
                fileNumLimit: 100
            });
        }

        function initUpload1(id) {
            $('#uploader').myUpload("destroy");
            $('#uploader').myUpload("rebind", {
                server: '/captcha/LicenseUploadPictures.ashx',
                formData: {
                    owner: id
                },
                fileNumLimit: 100
            });
        }

        function getData() {
            return $('#uploader').myUpload("data");
        }
    </script>
</body>
</html>
