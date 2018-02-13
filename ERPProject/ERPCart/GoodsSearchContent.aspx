<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoodsSearchContent.aspx.cs" Inherits="ERPProject.ERPCart.GoodsSearchContent" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link rel="stylesheet" href="../res/css/shoufengqin.css" />
    <link href="../res/css/gwNumberBox.css" rel="stylesheet" type="text/css" />
    <title></title>
    <style type="text/css">
        h1, h2, h3, h4, div, p, ul, li, img {
            margin: 0;
            padding: 0;
        }

        li {
            list-style: none;
        }

        a:link, a:visited {
            color: #0440A5;
            text-decoration: inherit;
        }

        a:hover {
            text-decoration: none;
        }

        /*ul.m_c li{padding:15px 20px; border-bottom:#ebebeb 1px solid; height:124px;}
        ul.m_c li:hover{ background: #F7FBFF;}
        ul.m_c li .mcdd{ line-height:25px; color:#999; float:left; margin-left:10px; margin-top:15px;}
        ul.m_c li .mcdd strong{ font-size:16px; color:#6184BE; width:400px; height:35px; overflow:hidden; display: inline-block;}
        ul.m_c li .jgmj{ float:right; margin-top:10px; text-align:center; width:150px; line-height:30px; font-size:14px; color:#666; font-family:Arial;}
        ul.m_c li .jgmj strong{ font-size:30px; color:#FF541F; padding-right:6px}
        ul.m_c li .tupian{ float:left;}*/


        .r_cont .bt {
            padding: 0 20px;
            margin-bottom: 20px;
            height: 30px;
            line-height: 30px;
            background: #F5F5F5;
            text-align: right;
            font-size: 14px;
        }

        .r_cont p.title {
            float: left;
        }

        ul.m_c {
        }

        .goods_li {
            position: relative;
            height: 120px;
            border-bottom: 1px solid #F5F5F5;
            padding: 15px;
        }

            .goods_li:hover {
                background: #FDFDFD;
                border-bottom: 1px solid #ddd;
            }

            .goods_li .goods_img {
                float: left;
                width: 120px;
                height: 120px;
                margin-right: 15px;
                margin-bottom: 0;
            }

            .goods_li h2 {
                font-size: 16px;
                line-height: 24px;
                margin: 5px 0 10px;
            }

            .goods_li .goods_price {
                position: absolute;
                top: 70px;
                right: 50px;
            }

            .goods_li p {
                font-size: 12px;
                line-height: 20px;
                color: #666;
            }

            .goods_li .goods_price {
                color: #333;
            }

                .goods_li .goods_price .m_price {
                    font-weight: bold;
                    color: #D70000;
                    font-size: 14px;
                }

        .sort_page {
            float: right;
            font-size: 14px;
        }

        .r_cont .sort_page_txt {
            float: left;
        }

            .r_cont .sort_page_txt b {
                color: #d70000;
                margin: 0 2px;
            }

        .r_cont .sort_page_num {
            float: left;
            margin-left: 20px;
        }

        .yema {
            padding: 15px 0 20px;
            text-align: right;
        }

        /*xm页码--start*/
        .manu {
            MARGIN: 3px;
            TEXT-ALIGN: center;
        }

            .manu A {
                background-color:#1ea3d8;
                padding: 7px;
                padding-left: 10px;
                padding-right: 10px;
                MARGIN: 5px;
                COLOR: white;
                font-size:10px;
                TEXT-DECORATION: none;
                border-radius:3px;
                -webkit-border-radius:3px;
                -moz-border-radius:3px;
                -o-border-radius:3px;
                font-size:11px;
            }

                .manu A:hover {
                    COLOR: black;
                    background-color:#d7ebf9;
                    cursor:pointer;
                }

                .manu A:active {
                    BORDER-RIGHT: #666 1px solid;
                    BORDER-TOP: #666 1px solid;
                    BORDER-LEFT: #666 1px solid;
                    COLOR: #666;
                    BORDER-BOTTOM: #666 1px solid;
                }

            .manu .current {
                BORDER-RIGHT: #666 1px solid;
                padding: 5px;
                padding-left: 10px;
                padding-right: 10px;
                BORDER-TOP: #666 1px solid;
                MARGIN: 5px;
                BORDER-LEFT: #666 1px solid;
                COLOR: #fff;
                BORDER-BOTTOM: #666 1px solid;
                BACKGROUND-COLOR: #666;
            }

            .manu .disabled {
                BORDER-RIGHT: #eee 1px solid;
                padding: 5px;
                padding-left: 10px;
                padding-right: 10px;
                BORDER-TOP: #eee 1px solid;
                MARGIN: 5px;
                BORDER-LEFT: #eee 1px solid;
                COLOR: #666;
                background-color: #f9f9f9;
                BORDER-BOTTOM: #eee 1px solid;
            }
            span.cpb{
                border:1px solid #1ea3d8;
                padding:5px;
                font-size:14px;
                margin-left:4px;
                padding-left:8px;
                padding-right:8px;
                color:#1ea3d8;
                font-weight:bold;
                border-radius:3px;
                -webkit-border-radius:3px;
                -moz-border-radius:3px;
                -o-border-radius:3px;
            }
        /*xm页码--End*/
        .gw-number-box {
            text-align: center;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="r_cont">

            <div class="bt">
                <p class="title">商品列表：</p>
                <div class="sort_page">
                    <a class="gwc_goods"></a>
                    <div class="sort_page_txt">共<b><asp:Literal ID="LiteralNum" runat="server"></asp:Literal></b>件商品</div>
                    <div class="sort_page_num">
                        <asp:Literal ID="LiteralLastPage" runat="server"></asp:Literal>
                        <span><b><%=strPage %></b>/<asp:Literal ID="LiteralPageNum" runat="server"></asp:Literal></span>
                        <asp:Literal ID="LiteralNextPage" runat="server"></asp:Literal>
                    </div>
                </div>
            </div>

            <ul class="m_c">
                <asp:Repeater ID="RepeaterGoods" runat="server">
                    <ItemTemplate>
                        <li class="goods_li">
                            <div class="goods_img">
                                <a href="javascript:void(0);" onclick="window.parent.ShowPic('<%#Eval("gdseq") %>');">
                                    <%--<img src="<%#Eval("picpath").ToString()==""?"../res/images/noPic.jpg":("../captcha/GoodsPicture.aspx?picPath="+Eval("picpath").ToString().Substring(2)) %>" width="120" height="120" />--%>
                                    <img src="<%#Eval("picpath").ToString()==""?"../res/images/noPic.jpg":(Eval("picpath").ToString()) %>" width="120" height="120" />
                                    <%--<img class="img" data-gdseq="<%#Eval("picpath") %>" src="/res/images/noPic.jpg" width="120" height="120" />--%>
                                </a>
                            </div>
                            <div class="goods_info">
                                <h2><a href="javascript:void(0);" onclick="window.parent.showOverlay('<%#Eval("gdseq") %>');"><%#Eval("gdname") %></a></h2>
                                <p class="goods_gdseq">商品编码：<%#Eval("gdseq") %></p>
                                <p class="goods_gdspec">商品规格：<%#Eval("gdspec") %></p>
                                <p class="goods_pizno">生产厂家：<%#Eval("producername") %></p>
                                <p style="position: fixed; right: 50px;">
                                    <input id="F<%#Eval("gdseq") %>" data-isdecimal="<%#Eval("isflag5") %>" type="text" class="gw-number-box" style="width: 80px; height: 20px; line-height: 24px; float: left; padding: 0" />
                                    <a href="javascript:void(0);" class="left_btn" onclick="addCartN('<%#Eval("gdseq") %>');">
                                        <img src="../res/images/addCart-small.jpg" style="margin-left: 5px" runat="server" /></a>
                                </p>
                                <p class="goods_price">
                                    <span class="m_price">￥<%#Eval("HSJJ") %></span>
                                    <span>/<%#Eval("UNITNAME") %></span>
                                </p>
                            </div>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>

            </ul>
            <div class="manu">
                <webdiyer:AspNetPager ID="AspNetPager1" CssClass="yema" CurrentPageButtonClass="cpb" runat="server" AlwaysShow="True" UrlPaging="true"
                    NextPageText="下一页" PageSize="10" PrevPageText="上一页" FirstPageText="首页" LastPageText="尾页"
                    ShowInputBox="Never" OnPageChanged="AspNetPager1_PageChanged" NumericButtonCount="10" ShowBoxThreshold="1000">
                </webdiyer:AspNetPager>
            </div>
        </div>
    </form>
    <script src="../res/js/jquery-1.11.1.min.js"></script>
    <script src="../res/js/jquery.gwNumberBox.js" type="text/javascript"></script>
    <script src="../res/js/jquery.fly.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            getImg();
        });
        function getImg() {
            var gdseqs = '';
            $('.img').each(function (i) {
                gdseqs += $(this).attr('data-gdseq') + ';';
            });
            if (gdseqs != '') {
                $.ajax({
                    url: '/captcha/ImageHelper.ashx?gdseqs=' + gdseqs,
                    success: function (result) {
                        console.log(result)
                        ra = result.split(";");
                        $('.img').each(function (i) {
                            if (ra[i] && ra[i] != 'img_') {
                                $(this).attr('src', '/captcha/ImageServer.aspx?img=' + ra[i]);
                            }
                        });
                    }
                });
            }

        }
        $('.gw-number-box').gwNumberBox({ min: 0, max: 10000, defaultVal: 1 });
        /* 把商品添加到购物车 */
        function addCartN(gdseq) {
            if ($("#F" + gdseq).val() <= 0) {
                top.F.util.alert({
                    message: '请填写商品数量！',
                    closable: true
                });
            }
            addProduct(event);
            window.parent.PosaddCart(gdseq, $("#F" + gdseq).val());
        }
        function addProduct(event) {
            var offset = $(".gwc_goods").offset(),
              flyer = $('<img src="../res/images/gwcfly-icon.png" width="26" height="26">');
            flyer.fly({
                start: {
                    left: event.pageX,
                    top: event.pageY
                },
                end: {
                    left: offset.left - 80,
                    top: offset.top,
                    width: 0, height: 0
                }
            });
        }

    </script>
</body>
</html>
