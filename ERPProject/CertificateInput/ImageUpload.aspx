<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImageUpload.aspx.cs" Inherits="ERPProject.Example.ImageUpload" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link rel="stylesheet" type="text/css" href="/res/webuploader/webuploader.css" />
    <link rel="stylesheet" type="text/css" href="/res/webuploader/detail/style.css" />
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="PanelMain" runat="server" />
        <f:Panel runat="server" ShowHeader="false" ShowBorder="false">
            <Items>
                <f:ContentPanel runat="server" ShowHeader="false" ShowBorder="false">
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
                                <div class="statusBar" style="display:none;">
                                    <div class="progress">
                                        <span class="text">0%</span>
                                        <span class="percentage"></span>
                                    </div><div class="info"></div>
                                    <div class="btns">
                                        <div id="filePicker2"></div><div class="uploadBtn">开始上传</div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </f:ContentPanel>

            </Items>
        </f:Panel>
    </form>
    <script type="text/javascript" src="/res/webuploader/webuploader.nolog.min.js"></script>
    <script type="text/javascript" src="/res/webuploader/detail/myupload.js"></script>
    <script>
        $(function () {
            $('#uploader').myUpload("init", {
                    server:'xxx',
                    formData: {
                        owner: 111
                    },
                    fileNumLimit: 100,
                    fileQueuedCallBack: function () {

                        alert('完成')
                    }
            });

            //$('#uploader').myUpload("rebind", {
            //    formData: {
            //        owner: 111
            //    }
            //});
        });
    </script>
</body>
</html>
