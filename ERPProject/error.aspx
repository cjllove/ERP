﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="error.aspx.cs" Inherits="ERPProject.Error" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>异常处理页面</title>
</head>
<body>
    <form id="form1" runat="server">
    <h2>系统错误</h2>
<div style="text-align:center;">
        <table width="100%" class="blueTab" border="0" cellspacing="1" cellpadding="1">
            <tr>
                <td colspan="3">
                    <table cellspacing="0" cellpadding="0" width="100%" border="0">
                        <tbody>
                            <tr>
                                <td >
                                    &nbsp;错误处理页面</td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
            <tr id="youhaotishi" >
                <td colspan="2" align="left">
                    &nbsp;欢迎您光临本网站！网站运行发生错误，请与管理员联系。错误原因可能如下:
                    <br />
                    &nbsp;&nbsp;&nbsp;1.非法访问页面.
                    <br />
                    &nbsp;&nbsp;&nbsp;2.您输入的数据错误.
                    <br />
                    &nbsp;&nbsp;&nbsp;3.您访问的页面不存在.
                    <br />
                    &nbsp;&nbsp;&nbsp;4.内容不存在,或已被删除.
                    <br />
                    &nbsp;&nbsp;&nbsp;5.系统忙,请稍候再试.
                </td>
            </tr>
            <tbody id="detailInformation" style="display: none;">
                    <tr>
                <td width="20%" class="alignRight" nowrap>
                    <strong>出错页面：</strong>
                </td>
                <td  class="alignLeft">
                    @Html.DisplayFor(model => model.ErrorPageUrl) </td>
                </tr>
                <tr>
                    <td class="alignRight" nowrap>
                        <strong>异常名称：</strong>
                    </td>
                    <td class="alignLeft">
                        @Html.DisplayFor(model => model.ExceptionName) </td>
                </tr>
                <tr>
                    <td class="alignRight" nowrap>
                        <strong>异常信息：</strong>
                    </td>
                    <td class="alignLeft">
                        @Html.DisplayFor(model => model.ExceptionMessage) </td>
                </tr>
                <tr id="trInnerExceptionName" runat="server">
                    <td class="alignRight" nowrap>
                        <strong>内部异常名称：</strong>
                    </td>
                    <td class="alignLeft">
                        @Html.DisplayFor(model => model.InnerExceptionName) </td>
                </tr>
                <tr id="trInnerExceptionMessage" runat="server">
                    <td class="alignRight" nowrap>
                        <strong>内部异常信息：</strong>
                    </td>
                    <td class="alignLeft">
                        @Html.DisplayFor(model => model.InnerExceptionMessage) 
                        </td>
                </tr>
                <tr id="trExceptionMethod" runat="server">
                    <td class="alignRight" nowrap>
                        <strong>方法名称：</strong>
                    </td>
                    <td class="alignLeft" style="background-color: #ffffcc;">
                        &nbsp;@Html.DisplayFor(model => model.TargetSite) </td>
                </tr>
                <tr id="trExceptionSource" runat="server">
                    <td class="alignRight" nowrap>
                        <strong>源文件：</strong>
                    </td>
                    <td class="alignLeft" style="background-color: #ffffcc;">
                        @Html.DisplayFor(model => model.SourceErrorFile) 
                    </td>
                </tr>
                <tr id="trExceptionRowId" runat="server">
                    <td class="alignRight" nowrap>
                        <strong>行号：</strong>
                    </td>
                    <td class="alignLeft" style="background-color: #ffffcc; color: Red">
                        &nbsp;@Html.DisplayFor(model => model.SourceErrorRowID) </td>
                </tr>
                <tr runat="server" id="trStack" visible="false">
                    <td class="alignRight">
                        <strong>堆栈跟踪：</strong>
                    </td>
                    <td class="alignLeft" style="background-color: #ffffcc;">
                        <code>
                            <pre id="litStack"><textarea name="errormsg" cols="80" rows="30" readonly="readonly">@Html.DisplayFor(model => model.StackInfo) </textarea> </pre>
                        </code>
                    </td>
                </tr>
            </tbody>            
        </table>
        <a id="showMessage" href="#" onclick="ShowErrorMessage();return false;">显示详细信息</a>
    </div>

<script type="text/javascript">

    var isShowMessage = true;

    function ShowErrorMessage() {

        var obj = document.getElementById("showMessage")
        var detailInformation = document.getElementById("detailInformation");
        var youhaotishi = document.getElementById("youhaotishi");

        if (isShowMessage) {
            obj.innerText = "隐藏详细信息";
            isShowMessage = false;
            detailInformation.style.display = "block";
            youhaotishi.style.display = "none";
        }
        else {
            obj.innerText = "显示详细信息";
            isShowMessage = true;
            detailInformation.style.display = "none";
            youhaotishi.style.display = "block";
        }

    }
</script>
    </form>
</body>
</html>
