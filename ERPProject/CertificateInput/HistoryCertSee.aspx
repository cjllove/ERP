<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HistoryCertSee.aspx.cs" Inherits="SPDProject.CertificateInput.HistoryCertSee" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link rel="stylesheet" type="text/css" href="/res/webuploader/webuploader.css" />
    <link rel="stylesheet" type="text/css" href="/res/webuploader/detail/style.css" />
    <link rel="stylesheet" type="text/css" href="/res/fancybox/jquery.fancybox.css" />
    <title></title>
    <style type="text/css">
        .color1 {
            background-color: #3AA02C;
            color: #fff;
        }

        .color2 {
            background-color: #BFBE24;
            color: #fff;
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
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="TabStrip1" runat="server"/>
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="1" runat="server">
            <Tabs>
                <f:Tab Title="证照信息" Icon="PageWord" Layout="Fit" runat="server">
                    <Items>

                        <f:Panel ID="PaneL6" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBOX" BoxFlex="2"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="Panel1" ShowBorder="false" BodyPadding="0px" Layout="Fit" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar1" runat="server">
                                            <Items>
                                                <f:Label ID="lblTime" CssStyle="font-size:20px;font-weight:Black;" runat="server"></f:Label>
                                                <f:TextBox ID="txtName" Label="资料查询" LabelWidth="70px" runat="server" EmptyText="请按照商品编码或名称查询" />
                                                <f:Button ID="btnSearch" Icon="Magnifier" Text="查 询" runat="server"></f:Button>
                                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                </f:Panel>

                                <f:Panel ID="Panel7" runat="server" AnchorValue="100%" ShowBorder="false" BodyPadding="0px" Layout="HBox" BoxConfigAlign="Stretch" BoxConfigPosition="Start" BoxConfigChildMargin="0" ShowHeader="False">
                                    <Items>
                                        <f:Panel ID="panel8" runat="server" BoxFlex="2" ShowBorder="false" ShowHeader="false" Layout="Fit" Height="635px">
                                            <Items>
                                                <f:Grid ID="GridLIS" AnchorValue="100% -20" ShowBorder="false" ShowHeader="false" EnableTextSelection="true" EnableCheckBoxSelect="true"
                                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;border-right: 1px solid #99bce8;"
                                                    PageSize="30" DataKeyNames="NAME,GDSEQ,GDNAME" IsDatabasePaging="true" AllowPaging="true" EnableRowClickEvent="true" OnRowClick="GridLIS_RowClick"
                                                    OnPageIndexChange="GridLIS_PageIndexChange" EnableColumnLines="true" EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridLIS_RowDoubleClick">
                                                    <Columns>
                                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" />
                                                        <f:BoundField Width="130px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="left"></f:BoundField>
                                                        <f:BoundField Width="210px" DataField="GDNAME" HeaderText="商品名称" TextAlign="left"></f:BoundField>
                                                    </Columns>
                                                </f:Grid>
                                            </Items>
                                        </f:Panel>

                                        <f:Panel ID="Panel3" runat="server" Width="480px" BoxConfigAlign="Stretch" CssStyle="border-top: 1px solid #99bce8;" Layout="VBox" AutoScroll="true" ShowBorder="false" ShowHeader="false" BodyPadding="0px">
                                            <Items>
                                                <f:Panel ID="Panel5" runat="server" Layout="Fit"
                                                    BodyPadding="0px" ShowBorder="false" ShowHeader="false">
                                                    <Items>

                                                        <f:Grid ID="GridCertype" ShowBorder="false" ShowHeader="false" runat="server"
                                                            DataKeyNames="SEQNO,GDSEQ,GDNAME,LICENSEID,CODE,NAME,OPERTIME,FLAG"
                                                            Height="400px" AutoScroll="true" EnableColumnLines="true" OnRowCommand="GridCertype_RowCommand"
                                                            CssStyle="border-right: 1px solid #99bce8;border-top:1px solid #99bce8;">
                                                            <Columns>
                                                                <f:RowNumberField Width="20px" TextAlign="Center" EnablePagingNumber="true" />
                                                                <f:BoundField Width="100px" DataField="NAME" HeaderText="证照名称" TextAlign="center" />
                                                                <f:BoundField Width="120px" DataField="OPERTIME" HeaderText="最新上传时间" TextAlign="center" />
                                                                <f:BoundField Width="60px" DataField="PICNUM" HeaderText="图片数" TextAlign="center" />
                                                                <f:BoundField Width="60px" DataField="FLAG" HeaderText="状态" TextAlign="center" />
                                                                <f:LinkButtonField Text="查看证照" CommandName="seepic" Width="100px" TextAlign="Center" ExpandUnusedSpace="true" />
                                                            </Columns>
                                                        </f:Grid>
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
                                                        <f:Label runat="server" ID="lblLicenseID" Hidden="true" />

                                                    </Items>
                                                </f:Toolbar>
                                            </Toolbars>
                                        </f:Panel>

                                        <f:Panel ID="Panel4" runat="server" AnchorValue="100%" ShowBorder="false" BodyPadding="0px" Layout="HBox" BoxConfigAlign="Stretch" BoxConfigPosition="Start" BoxConfigChildMargin="0" ShowHeader="False">
                                            <Items>
                                                <f:Panel ID="panelList" runat="server" Width="480px" ShowBorder="false" ShowHeader="false" Layout="Fit" Height="600px">
                                                    <Items>
                                                        <f:Grid ID="GridLicense" ShowBorder="false" ShowHeader="false" runat="server" EnableRowSelectEvent="true"
                                                            DataKeyNames="SEQNO,GDSEQ,GDNAME,LICENSEID,CODE,NAME,OPERTIME,FLAG" Height="500px"
                                                            AutoScroll="true" EnableColumnLines="true" CssStyle="border-right: 1px solid #99bce8;border-top:1px solid #99bce8;"
                                                            EnableRowClickEvent="true" OnRowClick="GridLicense_RowClick"
                                                            EnableCheckBoxSelect="true" EnableMultiSelect="true" AllowSorting="false">
                                                            <Columns>
                                                                <f:RowNumberField Width="20" TextAlign="Center" />
                                                                <f:BoundField Width="100px" DataField="NAME" HeaderText="证照名称" TextAlign="center" />
                                                                <f:BoundField Width="114px" DataField="OPERTIME" HeaderText="最新上传时间" TextAlign="center" />
                                                                <f:BoundField Width="60px" DataField="PICNUM" HeaderText="图片数" TextAlign="center" />
                                                                <f:BoundField Width="50px" DataField="FLAG" HeaderText="状态" TextAlign="center" ExpandUnusedSpace="true" />
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
                                                                        <f:TextBox ID="docLISNAME" runat="server" Label="当前证照" ShowRedStar="true" Required="true" LabelWidth="90px" Enabled="false" />
                                                                        <f:TextBox ID="docDOCID" runat="server" Label="档案编号" LabelWidth="80px"></f:TextBox>
                                                                        <f:TextBox ID="docMEMO" runat="server" Label="备注" LabelWidth="80px" />
                                                                    </Items>
                                                                </f:FormRow>
                                                                <f:FormRow ColumnWidths="31% 25% 25% 19%">
                                                                    <Items>
                                                                        <f:TextBox ID="docGDNAME" runat="server" ShowRedStar="true" Required="true" Label="商品名称" LabelWidth="90px" Enabled="false" />
                                                                        <f:DatePicker ID="dpkBEGRQ" runat="server" LabelWidth="80px" Label="注册日期" ShowRedStar="true" />
                                                                        <f:DatePicker ID="dpkENDRQ" runat="server" LabelWidth="80px" Label="到期日期" ShowRedStar="true" />
                                                                        <f:CheckBox ID="ischk" runat="server" LabelWidth="80" Label="是否长期" AutoPostBack="true"></f:CheckBox>
                                                                    </Items>
                                                                </f:FormRow>
                                                            </Rows>
                                                        </f:Form>
                                                        <f:Panel ID="Panel9" runat="server" CssStyle="border-left: 1px solid #99bce8;" ColumnWidth="25%"
                                                            Height="640px" AutoScroll="true" ShowBorder="false" ShowHeader="false" BodyPadding="0px 0px 0px 0px">
                                                            <Items>
                                                                <f:Form ID="FormPic" ShowBorder="false" AutoScroll="false" Height="500px" CssStyle="padding:5px;" ShowHeader="False" LabelWidth="80px" runat="server">
                                                                    <Rows>
                                                                        <f:FormRow>
                                                                            <Items>
                                                                                <f:Label ID="imglbl" Hidden="false" runat="server"></f:Label>
                                                                                <f:Image ID="imgBMPPATH" CssClass="w1000" runat="server" ImageUrl="~/res/images/picc.jpg" />
                                                                            </Items>
                                                                        </f:FormRow>
                                                                    </Rows>
                                                                </f:Form>
                                                            </Items>
                                                            <Items>
                                                                <f:Form ID="btnForm" ShowBorder="false" AutoScroll="false" Height="60px" CssStyle="border-top:1px solid #99bce8;" ShowHeader="false" runat="server">
                                                                    <Rows>
                                                                        <f:FormRow ColumnWidth="50% 50%">
                                                                            <Items>
                                                                                <f:Button ID="btnLeft" runat="server" Text="< <上一张" Icon="Accept" CssStyle="width:90%;margin-left:10%;height:30px;margin-top:15px;padding-top:6px" BoxConfigAlign="Center" OnClick="btnLeft_Click" />
                                                                                <f:Button ID="btnRight" runat="server" Text="> >下一张" Icon="Accept" CssStyle="width:90%;margin-right:10%;height:30px;margin-top:15px;padding-top:6px" BoxConfigAlign="Center" OnClick="btnRight_Click" />
                                                                            </Items>
                                                                        </f:FormRow>
                                                                    </Rows>
                                                                </f:Form>
                                                            </Items>
                                                        </f:Panel>
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
                <f:Window ID="Window1" Title="证照图片展示" Hidden="true" EnableIFrame="true" runat="server"
            EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True"
            Width="940px" Height="640px" OnClose="Window1_Close">
        </f:Window>
        <f:HiddenField ID="hfdTempPic" runat="server" />
        <f:HiddenField ID="hfdLISID" runat="server" />
        <f:HiddenField ID="hfdLISNAME" runat="server" />
        <f:HiddenField ID="hfdGDSEQ" runat="server" />
        <f:HiddenField ID="hfdSEQNO" runat="server" />
        <f:HiddenField ID="hfdHSEQNO" runat="server" />
        <f:HiddenField ID="hfdURL" runat="server" />
        <f:HiddenField ID="hfdisChange" runat="server" />
    </form>
</body>
</html>
