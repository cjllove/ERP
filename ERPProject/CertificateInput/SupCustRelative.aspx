<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SupCustRelative.aspx.cs" Inherits="ERPProject.CertificateInput.SupCustRelative" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link rel="stylesheet" type="text/css" href="/res/webuploader/webuploader.css" />
    <link rel="stylesheet" type="text/css" href="/res/webuploader/detail/style.css" />
    <link rel="stylesheet" type="text/css" href="/res/fancybox/jquery.fancybox.css" />
    <title>供货商证照管理</title>
    <style type="text/css">
        .w1000 {
            margin: 0 auto;
            width: 100%;
        }

            .w1000 img {
                width: 100%;
                height: 300px;
            }
    </style>

    <style type="text/css">
        .color1 {
            color: #000;
        }

        .color2 {
            color: red;
            filter: alpha(opacity=50);
        }

        .color3 {
            background-color: #AF5553;
            color: #fff;
        }

        .color4 {
            background-color: #F8B551;
            color: #fff;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="TabStrip1" runat="server" OnCustomEvent="PageManager1_CustomEvent" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="1" runat="server">
            <Tabs>
                <f:Tab Title="证照信息" Icon="PageWord" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" BoxFlex="2" Layout="VBOX"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="Panel3" ShowBorder="false" BodyPadding="0px" Layout="Fit" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar1" runat="server">
                                            <Items>
                                                <f:Label ID="lblTime" CssStyle="font-size:20px;font-weight:Black;" runat="server"></f:Label>
                                                <f:TextBox ID="txtName" Label="机构查询" LabelWidth="70px" runat="server" EmptyText="请按照机构编号或名称查询" />
                                                <f:DropDownList ID="ddlFLAG" Hidden="true" runat="server" Label="状态" LabelWidth="45px" Width="127px">
                                                    <f:ListItem Value="" Selected="true" Text="--请选择--" />
                                                    <f:ListItem Value="N" Text="已保存" />
                                                    <f:ListItem Value="S" Text="已提交" />
                                                    <f:ListItem Value="Y" Text="已审核" />
                                                    <f:ListItem Value="R" Text="已驳回" />
                                                </f:DropDownList>
                                                <f:CheckBox ID="chkisLR" Label="已上传" AutoPostBack="true" LabelWidth="60px" OnCheckedChanged="chkisLR_CheckedChanged" runat="server"></f:CheckBox>
                                                <f:CheckBox ID="chknoLR" Label="未上传" AutoPostBack="true" LabelWidth="60px" OnCheckedChanged="chknoLR_CheckedChanged" runat="server"></f:CheckBox>
                                                <f:Button ID="btnSearch" Icon="Magnifier" Text="查 询" runat="server" OnClick="btnSearch_Click"></f:Button>
                                                <f:Button ID="btnHistory" Hidden="true" Icon="BookMagnify" Text="历史证照查看" runat="server" OnClick="btnHistory_Click"></f:Button>
                                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                <f:Button ID="mybtnAudit" Icon="UserTick" Text="审 核" EnablePostBack="true" runat="server" OnClick="mybtnAudit_Click" />
                                                <f:Button ID="mybtnReject" Icon="UserCross" Text="驳 回" EnablePostBack="true" runat="server" OnClick="mybtnReject_Click" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                </f:Panel>

                                <f:Panel ID="Panel7" BoxFlex="1" runat="server" AnchorValue="100%" ShowBorder="false" BodyPadding="0px" Layout="HBox" BoxConfigAlign="Stretch" BoxConfigPosition="Start" BoxConfigChildMargin="0" ShowHeader="False">
                                    <Items>
                                        <f:Panel ID="panel8" runat="server" BoxFlex="2" ShowBorder="false" ShowHeader="false" Layout="Fit" Height="635px">
                                            <Items>
                                                <f:Grid ID="GridLIS" AnchorValue="100% -20" ShowBorder="false" ShowHeader="false" EnableTextSelection="true" EnableCheckBoxSelect="true"
                                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;border-right: 1px solid #99bce8;"
                                                    PageSize="30" DataKeyNames="NAME,SUPID,SUPNAME,SEQNO" IsDatabasePaging="true" AllowPaging="true" EnableRowClickEvent="true" OnRowClick="GridLIS_RowClick"
                                                    OnPageIndexChange="GridLIS_PageIndexChange" EnableColumnLines="true"
                                                    OnRowDataBound="GridLIS_RowDataBound" EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridLIS_RowDoubleClick">
                                                    <Columns>
                                                        <f:RowNumberField Width="30px" TextAlign="Center" />
                                                        <f:BoundField Width="80px" DataField="SUPPLIER" HeaderText="客户类型" TextAlign="center" />
                                                        <f:BoundField Width="80px" DataField="SUPID" HeaderText="编号" TextAlign="left" />
                                                        <f:BoundField Width="220px" DataField="SUPNAME" HeaderText="名称" TextAlign="left" />
                                                        <f:BoundField Width="80px" DataField="PICNUM" HeaderText="承诺书" TextAlign="center" />
                                                        <f:BoundField Width="80px" DataField="FLAG" HeaderText="承诺书状态" TextAlign="center" />
                                                        <f:BoundField Width="80px" DataField="HTPICNUM" HeaderText="合同书" TextAlign="center" />
                                                        <f:BoundField Width="80px" DataField="FLAG1" HeaderText="合同书状态" TextAlign="center" />
                                                    </Columns>
                                                </f:Grid>
                                            </Items>
                                        </f:Panel>

                                        <f:Panel ID="Panel5" runat="server" Width="410px" BoxConfigAlign="Stretch" CssStyle="border-top: 1px solid #99bce8;" Layout="VBox" AutoScroll="true" ShowBorder="false" ShowHeader="false" BodyPadding="0px">
                                            <Items>
                                                <f:Grid ID="GridCertype" ShowBorder="false" ShowHeader="false" runat="server"
                                                    DataKeyNames="SEQNO,SUPID,SUPNAME,LICENSEID,CODE,CASE,OPERTIME,FLAG,CASE"
                                                    Height="400px" AutoScroll="true" EnableColumnLines="true" OnRowCommand="GridCertype_RowCommand"
                                                    CssStyle="border-right: 1px solid #99bce8;border-top:1px solid #99bce8;">
                                                    <Columns>
                                                        <f:RowNumberField Width="20px" TextAlign="Center" EnablePagingNumber="true" />
                                                        <f:BoundField Width="160px" DataField="CASE" HeaderText="证照名称" TextAlign="center" />
                                                        <f:BoundField Width="120px" DataField="OPERTIME" HeaderText="最新上传时间" TextAlign="center" />
                                                        <f:LinkButtonField Text="查看" CommandName="seepic" Width="100px" TextAlign="Center" ExpandUnusedSpace="true" />
                                                        <f:BoundField Width="90px" DataField="FLAG" HeaderText="状态" Hidden="true" TextAlign="center" ExpandUnusedSpace="false" />
                                                    </Columns>
                                                </f:Grid>
                                            </Items>
                                        </f:Panel>

                                    </Items>
                                </f:Panel>

                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>

                <f:Tab Title="证照录入" Icon="Table" Layout="Fit" runat="server" ID="tab1">
                    <Items>
                        <f:Panel ID="Panel2" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
                            ShowHeader="false">
                            <Items>

                                <f:Panel ID="PaneLis" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBOX" BoxFlex="2"
                                    ShowHeader="false">
                                    <Items>
                                        <f:Panel ID="PanelBody" ShowBorder="false" BodyPadding="0px"
                                            Layout="Fit" ShowHeader="False" runat="server">
                                            <Toolbars>
                                                <f:Toolbar ID="Toolbar3" runat="server">
                                                    <Items>
                                                        <f:Label runat="server" Hidden="true" ID="lblLicenseID" Text="操作" />
                                                        <f:Button ID="btnSave" Icon="PageSave" Text="保 存" EnablePostBack="true" runat="server" OnClick="btnSave_Click" />
                                                        <f:Button ID="btnSubmit" Icon="Accept" Text="提 交" ConfirmText="确认证照类别都已填写完整，并且提交吗？" Enabled="false" EnablePostBack="true" runat="server" OnClick="btnSubmit_Click" />
                                                        <f:Button ID="btnRollBack" Icon="Accept" Text="撤 回" EnablePostBack="true" runat="server" OnClick="btnRollBack_Click" />
                                                        <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                        <f:Button ID="btnAudit" Icon="UserTick" Text="审 核" EnablePostBack="true" runat="server" OnClick="btnAudit_Click" />
                                                        <f:Button ID="btnReject" Icon="UserCross" Text="驳 回" EnablePostBack="true" runat="server" OnClick="btnReject_Click" />
                                                    </Items>
                                                </f:Toolbar>
                                            </Toolbars>
                                        </f:Panel>

                                        <f:Panel ID="Panel4" runat="server" AnchorValue="100%" ShowBorder="false" BodyPadding="0px" Layout="HBox" BoxConfigAlign="Stretch" BoxConfigPosition="Start" BoxConfigChildMargin="0" ShowHeader="False">
                                            <Items>
                                                <f:Panel ID="panelList" runat="server" Width="455px" ShowBorder="false" ShowHeader="false" Layout="Fit" Height="600px">
                                                    <Items>
                                                        <f:Grid ID="GridLicense" ShowBorder="false" ShowHeader="false" runat="server" EnableCheckBoxSelect="true"
                                                            DataKeyNames="SEQNO,SUPID,SUPNAME,LICENSEID,CODE,CASE,OPERTIME,FLAG,CASE" Height="500px"
                                                            AutoScroll="true" EnableColumnLines="true" CssStyle="border-right: 1px solid #99bce8;border-top:1px solid #99bce8;"
                                                            EnableRowClickEvent="true" OnRowClick="GridLicense_RowClick" OnRowCommand="GridLicense_RowCommand"
                                                            EnableMultiSelect="true" AllowSorting="false">
                                                            <Columns>
                                                                <f:RowNumberField Width="20" TextAlign="Center" />
                                                                <f:BoundField Width="100px" DataField="CASE" HeaderText="证照名称" TextAlign="center" />
                                                                <f:BoundField Width="114px" DataField="OPERTIME" HeaderText="最新上传时间" TextAlign="center" />
                                                                <f:BoundField Width="50px" DataField="FLAG" HeaderText="状态" TextAlign="center" />
                                                                <f:LinkButtonField Text="查看" CommandName="htpic" Width="50px" TextAlign="Center" />
                                                                <f:LinkButtonField Text="换证" ID="lbfChCerts" CommandName="changeCert" Width="50px" TextAlign="Center" />
                                                            </Columns>
                                                        </f:Grid>
                                                    </Items>
                                                </f:Panel>


                                                <f:Panel ID="sfmImage" runat="server" BoxFlex="2" CssStyle="border-top: 1px solid #99bce8;" Layout="VBox" AutoScroll="true" ShowBorder="false" ShowHeader="false" BodyPadding="0px">
                                                    <Items>
                                                        <f:Form ID="FormDoc" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                                            ShowHeader="False" LabelWidth="70px" runat="server">
                                                            <Rows>
                                                                <f:FormRow ColumnWidths="31% 25% 42%">
                                                                    <Items>
                                                                        <f:TextBox ID="docLISNAME" runat="server" Label="当前证照" LabelWidth="90px" Enabled="false" />
                                                                        <f:TextBox ID="docDOCID" runat="server" Label="档案编号" LabelWidth="80px"></f:TextBox>
                                                                        <f:TextBox ID="docMEMO" runat="server" Label="备注" LabelWidth="80px" />
                                                                    </Items>
                                                                </f:FormRow>
                                                                <f:FormRow ColumnWidths="31% 25% 25% 19%">
                                                                    <Items>
                                                                        <f:TextBox ID="docSUPNAME" runat="server" Label="机构名称" LabelWidth="90px" Enabled="false" />
                                                                        <f:DatePicker ID="dpkBEGRQ" runat="server" LabelWidth="80px" Label="注册日期" ShowRedStar="true" />
                                                                        <f:DatePicker ID="dpkENDRQ" runat="server" LabelWidth="80px" Label="到期日期" ShowRedStar="true" />
                                                                        <f:CheckBox ID="ischk" runat="server" LabelWidth="80" Label="是否长期" AutoPostBack="true" OnCheckedChanged="ischk_CheckedChanged"></f:CheckBox>
                                                                    </Items>
                                                                </f:FormRow>
                                                            </Rows>
                                                        </f:Form>
                                                        <f:ContentPanel runat="server" ID="contentPel1" ShowHeader="false" ShowBorder="false" BoxFlex="1">
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

                                    </Items>
                                </f:Panel>

                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>


            </Tabs>
        </f:TabStrip>
        <f:HiddenField ID="hfdLISID" runat="server" />
        <f:HiddenField ID="hfdLISNAME" runat="server" />
        <f:HiddenField ID="hfdSUPID" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdSEQNO" runat="server" />
        <f:HiddenField ID="hfdHSEQNO" runat="server" />
        <f:HiddenField ID="hfdURL" runat="server" />
        <f:HiddenField ID="hfdisChange" runat="server" />
        <f:HiddenField ID="hfdFLAG" runat="server"></f:HiddenField>

        <f:Window ID="Window1" Title="证照图片展示" Hidden="true" EnableIFrame="true" runat="server"
            EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True"
            Width="940px" Height="640px">
        </f:Window>
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
