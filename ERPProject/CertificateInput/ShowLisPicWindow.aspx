<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowLisPicWindow.aspx.cs" Inherits="ERPProject.CertificateInput.ShowLisPicWindow" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>

    <style type="text/css">
        .w1000 {
            margin: 0 auto;
            width: 100%;
        }

            .w1000 img {
                width: 100%;
                height:600px;
            }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="True" BodyPadding="0px" Layout="Anchor"
            ShowHeader="false">
            <Toolbars>
                <f:Toolbar ID="Toolbar2" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="btnClosePostBack" Text="确 定" Icon="SystemSave" runat="server" OnClick="btnClosePostBack_Click">
                        </f:Button>
                        <f:Button ID="btnClose" Text="关 闭" Icon="SystemClose" runat="server">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Panel ID="sfmImage" runat="server" AnchorValue="100% -2" CssStyle="border-left: 1px solid #99bce8;" ColumnWidth="25%"
                    Height="640px" AutoScroll="true" ShowBorder="false" ShowHeader="false" BodyPadding="0px 0px 0px 0px">
                    <Items>
                        <f:Form ID="FormPic" ShowBorder="false" AutoScroll="false" Height="500px" CssStyle="padding:5px;" ShowHeader="False" LabelWidth="80px" runat="server">
                            <Rows>
                                <f:FormRow>
                                    <Items>
                                        <f:Label ID="imglbl" Hidden="true" runat="server"></f:Label>
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
                                        <f:Button ID="btnLeft" runat="server" Text="< <上一张" Icon="Accept" CssStyle="width:90%;margin-left:10%;height:30px;margin-top:15px;padding-top:6px"  BoxConfigAlign="Center" OnClick="prePageBtn_Click" />
                                        <f:Button ID="btnRight" runat="server" Text="> >下一张" Icon="Accept" CssStyle="width:90%;margin-right:10%;height:30px;margin-top:15px;padding-top:6px" BoxConfigAlign="Center" OnClick="nextPageBtn_Click" />
                                    </Items>
                                </f:FormRow>
                            </Rows>
                        </f:Form>
                    </Items>
                </f:Panel>
            </Items>
        </f:Panel>
    </form>

    <f:HiddenField ID="seqnohfd" runat="server"></f:HiddenField>
    <f:HiddenField ID="hfdpicnum" runat="server"></f:HiddenField>
    <f:HiddenField ID="hfdlisid" runat="server"></f:HiddenField>
</body>
</html>
