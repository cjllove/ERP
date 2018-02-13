<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DepartmentMonthBudgetControl.aspx.cs" Inherits="ERPProject.ERPApply.DepartmentMonthBudgetControl" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="../res/css/tooltipster.css" rel="stylesheet" />
    <link href="../res/css/themes/tooltipster-shadow.css" rel="stylesheet" />
    <script src="../../res/js/jquery.ymh.js" type="text/javascript"></script>
    <script src="../res/js/ichart.1.2.min.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
            ShowHeader="false">
            <Content>
                <div id="canvasDiv"></div>
            </Content>
        </f:Panel>
        <f:HiddenField ID="hfMonth" runat="server"></f:HiddenField>
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
                    width: 370,
                    height: 270,
                    background_color: '#ffffff',
                    legend: {
                        enable: true,
                        background_color: null,
                        border: {
                            enable: false
                        }
                    },
                    sub_option: {
                        listeners: {
                            /**
                             * r:iChart.Rectangle2D对象
                             * e:eventObject对象
                             * m:额外参数
                             */
                            click: function (r, e, m) {
                                if (r.get('name') == "执行" && e.x > 280) {
                                    addTab('', '/ERPapply/DepartmentBudget.aspx', '科室月度预算执行分析');
                                } else {
                                    addTab('', '/ERPapply/DepartmentMonthBudgetBig.aspx', '科室月度预算执行情况');
                                }
                            }
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
                        width: 240,
                        height: 230
                    }
                });
                chart.draw();
            }

            function addTab(id, navurl, tabname) {
                parent.parent.addExampleTab.apply(null, ['hello_FineUIPro_tab' + id, navurl, tabname, '../../res/images/filetype/vs_aspx.png', true]);
            }
        </script>
    </form>
</body>
</html>
