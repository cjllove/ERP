<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TraceBackFm.aspx.cs" Inherits="ERPProject.ERPQuery.TraceBackFm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="../res/bootstrap/bootstrap.css" rel="stylesheet" />
    <link href="../res/bootstrap/bootstrap-responsive.css" rel="stylesheet" />
    <link href="../res/bootstrap/docs.css" rel="stylesheet" />
    <style>
        body {
            padding: 0 10%;
            font-family: Arial;
            overflow-x: hidden;
            background-color:#F2F5F7;
            /*background-image: url(../res/images/square.gif);*/
        }

        h1 {
            font-family: 微软雅黑,黑体;
            font-weight:normal;
            font-size:18px;
            text-align:center;
            border-bottom:1px solid #ccc;
            padding-bottom:5px;
        }

            h1 span.lead {
                font-size: 18px;
                padding-left: 20px;
                color: #333;
                font-weight: bold;
            }

        ul.timeline {
                list-style-type: none;
                background: url("../ERPUpload/Wiki/images/version_line.png") repeat-y scroll 120px 0 transparent;
                margin: 8px 0;
                padding: 0;
            }

                ul.timeline li {
                    position: relative;
                    margin-bottom: 40px;
                }

                    ul.timeline li .time {
                        position: absolute;
                        width: 120px;
                        text-align: right;
                        left: -32px;
                        top: 10px;
                        color: #999;
                    }

                    ul.timeline li .version {
                        position: absolute;
                        width: 290px;
                        text-align: right;
                        left: -200px;
                        top: 30px;
                        font-size: 20px;
                        line-height: 50px;
                        color: #3594cb;
                        overflow: hidden;
                    }

                        ul.timeline li .version span {
                            color: #666;
                            font-size: 12px;
                            line-height: 20px;
                            display: block;
                            text-align: right;
                        }

                            ul.timeline li .version span i {
                                color: #333;
                                font-style: normal;
                            }

                    ul.timeline li .number {
                        position: absolute;
                        background: url("../ERPUpload/Wiki/images/version_dot.png") no-repeat scroll 0 0 transparent;
                        width: 56px;
                        height: 56px;
                        left: 97px;
                        line-height: 56px;
                        text-align: center;
                        color: #fff;
                        font-size: 18px;
                    }

                    ul.timeline li.alt .number {
                        background-image: url("../ERPUpload/Wiki/images/version_dot_alt.png");
                    }

                    ul.timeline li .content {
                        margin-left: 180px;
                    }

                        ul.timeline li .content pre {
                            background-color: #3594cb;
                            padding: 20px;
                            color: #fff;
                            font-size: 13px;
                            line-height: 20px;
                        }

                    ul.timeline li.alt .content pre {
                        background-color: #43B1F1;
                    }

            
    </style>
</head>
<body>

    <div>
        <div>
            <h1><%=GDNAME %><span class="lead"><%=USERNAME %></span></h1>
        </div>
    </div>

    <div class="container">

        <ul class="timeline">
            <asp:Repeater ID="RepeaterVersion" runat="server">
                <ItemTemplate>
                    <li>
                        <div class="time"><%#Eval("INSTIME","{0:yyyy-MM-dd hh:mm}") %></div>
                        <div class="version" style="display:none;"><%#Eval("USERNAME") %></div>
                        <div class="number"></div>
                        <div class="content">
                            <pre><%#Eval("MEMO") %></pre>
                        </div>
                    </li>
                </ItemTemplate>
            </asp:Repeater>
        </ul>
    </div>
    <script src="../ERPUpload/Wiki/js/jquery.min.js"></script>
    <script src="../ERPUpload/Wiki/js/bootstrap.min.js"></script>

    <script>
        $(function () {
            var urlSearch = window.location.search;
            if (urlSearch && urlSearch.indexOf('from=demo') >= 0) {
                $(document.body).addClass('from-demo');
            }

            var nextDataNumber = 5;
            var ajaxLoading = false;

            var ulNode = $('ul.timeline');

            function initLiNodes() {
                var liNodes = ulNode.find('li'), count = liNodes.length, i, liNode, leftCount = nextDataNumber * 20;
                for (i = 0; i < count; i++) {
                    liNode = $(liNodes.get(i));
                    if (i % 2 !== 0) {
                        liNode.addClass('alt');
                    } else {
                        liNode.removeClass('alt');
                    }
                    liNode.find('.number').text(count - i);
                }
            }
            initLiNodes();
        });
    </script>

</body>
</html>
