<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DepartmentSingleBudget.aspx.cs" Inherits="ERPProject.ERPApply.DepartmentSingleBudget" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>科室预算三个月对比图</title>
    <link href="../res/css/tooltipster-shadow.css" rel="stylesheet" />
    <link href="../res/css/tooltipster.css" rel="stylesheet" />
    <script src="../res/js/ichart.1.2.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        var labelarr;
        var systit;
        var syssubtit;
        var footLabel;
        function clearcurrdata() {
            labelarr = "";
            systit = "";
            syssubtit = "";
            footLabel = "";
        }
        function reloaddata(labelarr, systit, syssubtit, footLabel, data, Imax) {
            clearcurrdata();
            labelarr = labelarr;
            systit = systit;
            syssubtit = syssubtit;
            footLabel = footLabel;
            data = eval('(' + data + ')');
            var chart = new iChart.ColumnMulti2D({
                render: 'canvasDiv',
                data: data,
                labels: labelarr.split(','),
                title: systit,

                subtitle: syssubtit,

                footnote: footLabel,

                fit: true,

                background_color: '#ffffff',

                legend: {

                    enable: true,

                    background_color: null,

                    border: {

                        enable: false

                    }

                },

                coordinate: {

                    background_color: '#f1f1f1',

                    scale: [{

                        position: 'left',

                        start_scale: 0,

                        end_scale: Imax,

                        scale_space: (Imax / 10).toFixed()

                    }],

                    width: 500,

                    height: 300

                }

            });
            chart.draw();
        }

    </script>

</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
            ShowHeader="false">
           <%-- <Toolbars>
                <f:Toolbar runat="server">
                    <Items>
                        <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="" runat="server" />
                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                        <f:ToolbarSeparator runat="server" />
                        <f:Button ID="bntClear" Icon="Erase" Text="清 空" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                        <f:ToolbarSeparator runat="server" />
                        <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                    </Items>
                </f:Toolbar>
            </Toolbars>--%>
            <Content>
                <div id="canvasDiv"></div>
            </Content>
        </f:Panel>
        <f:HiddenField ID="hfgdSeq" runat="server" />
        <f:HiddenField ID="hfDeptID" runat="server" />
    </form>
</body>
</html>

