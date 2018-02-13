<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EarlyWarningInput.aspx.cs" Inherits="ERPProject.CertificateInput.EarlyWarningInput" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>证照到期提醒</title>
    <style type="text/css">
        .color1 {
            background-color: #DCDCDC;
            color: #000;
        }

        .color2 {
            background-color: red;
            color: #fff;
            /*filter:alpha(opacity=50);*/
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
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" runat="server" AutoScroll="false" BodyPadding="0px" Layout="Anchor" ShowBorder="false" ShowHeader="false">
            <Items>
                <f:Grid ID="GridCertype" ShowBorder="false" ShowHeader="false" AnchorValue="100% -1" OnPageIndexChange="GridCertype_PageIndexChange"
                    AutoScroll="false" EnableCheckBoxSelect="false" DataKeyNames="SEQNO" OnRowDataBound="GridCertype_RowDataBound"
                    EnableColumnLines="true" runat="server" PageSize="30" IsDatabasePaging="true" AllowPaging="true">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar1" runat="server">
                            <Items>
                                <f:TextBox ID="tbxDOCID" runat="server" Label="档案编号" LabelWidth="80px"></f:TextBox>
                                <f:DropDownList ID="ddlIsWarn" runat="server" LabelWidth="80px" Width="170px" Label="证照状态">
                                    <f:ListItem Value="ALLExpire" Text="全部" Selected="true" />
                                    <f:ListItem Value="ISExpire" Text="已过期" />
                                    <f:ListItem Value="NOExpire" Text="即将过期" />
                                </f:DropDownList>
                                <f:NumberBox runat="server" LabelWidth="100px" Width="190px" MinValue="0" ID="nbxExpire" Label="过期剩余天数" Text="30"></f:NumberBox>
                                <f:Button ID="btnSearch" runat="server" Icon="Magnifier" OnClick="btnSearch_Click" Text="查询"></f:Button>
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Columns>
                        <f:RowNumberField Width="60px" TextAlign="Center"></f:RowNumberField>
                        <f:BoundField Width="150px" DataField="TISHI" HeaderText="提醒"></f:BoundField>
                        <f:BoundField Width="160px" DataField="SEQNO" HeaderText="证照流水" />
                        <f:BoundField Width="120px" DataField="LICTYPE" HeaderText="证照类别" TextAlign="Center" />
                        <f:BoundField Width="150px" DataField="BEGRQ" HeaderText="开始日期" DataFormatString="{0:yyyy-MM-dd}" TextAlign="Center" />
                        <f:BoundField Width="150px" DataField="ENDRQ" HeaderText="截止日期" DataFormatString="{0:yyyy-MM-dd}" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="DOCID" HeaderText="档案编号" />
                        <f:BoundField Width="100px" DataField="MEMO" HeaderText="备注" ExpandUnusedSpace="true" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>

        <f:HiddenField ID="hfdValue" runat="server"></f:HiddenField>
    </form>
</body>
</html>
