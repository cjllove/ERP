﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DepartmentMonthBudgetControl1.aspx.cs" Inherits="ERPProject.ERPApply.DepartmentMonthBudgetControl1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="../res/css/tooltipster.css" rel="stylesheet">
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
        function reloaddata(labelarr, systit, syssubtit, footLabel, data, iMax) {
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
                fit: true,
                subtitle: syssubtit,
                border: {
                    enable: false
                },
                footnote: footLabel,

                width: 400,

                height: 300,

                background_color: '#F2F5F7',

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

                        end_scale: iMax,

                        scale_space: (iMax / 10).toFixed()

                    }],

                    width: 270,

                    height: 260

                }

            });
            chart.draw();
        }

        // console.log(basePath);
        function addTab(id, navurl, tabname) {
            parent.parent.addExampleTab.apply(null, ['hello_FineUIPro_tab' + id, navurl, tabname, '../../res/images/filetype/vs_aspx.png', true]);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
            <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
                ShowHeader="false">
                <Content>
                    <div id="canvasDiv"></div>
                </Content>
            </f:Panel>
        </div>
    </form>
</body>
</html>

