﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BasicInfoSetup.aspx.cs" Inherits="ERPProject.ERPBasic.BasicInfoSetup" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>配送日期设置</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="true" ID="PageManager1" AutoSizePanelID="PanelMain" runat="server" />
        <f:Panel ID="PanelMain" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBOX" ShowHeader="false">
            <Items>
                <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                    ShowHeader="False" LabelWidth="70px" runat="server">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar2" runat="server">
                            <Items>
                                <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="操作信息：批量设置科室配送时间信息！" runat="server" />
                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                <f:Button ID="bntClear" Icon="Erase" Text="清 空" EnablePostBack="true" runat="server" OnClick="bntClear_Click" EnableDefaultState="false" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="bntSave" Icon="Disk" Text="保 存" EnablePostBack="true" runat="server" OnClick="bntSave_Click" EnableDefaultState="false" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnablePostBack="true" runat="server" OnClick="bntSearch_Click" EnableDefaultState="false" />
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Rows>
                        <f:FormRow>
                            <Items>
                                <%--<f:TextBox ID="tbxGoodsName" runat="server" Label="品名/编码" />--%>
                                <f:DropDownList ID="ddlDEPTID" runat="server" Label="科室" EnableEdit="true" ForceSelection="true" />
                                <f:DropDownList ID="ddlPSY" runat="server" Label="配送员" EnableEdit="true" ForceSelection="true" />
                                <f:DropDownList ID="ddlPSTIME" runat="server" Label="配送时间" EnableEdit="true" ForceSelection="true">
                                    <f:ListItem Text="--请选择--" Value="" />
                                    <f:ListItem Text="星期一" Value="DHZQ1" />
                                    <f:ListItem Text="星期二" Value="DHZQ2" />
                                    <f:ListItem Text="星期三" Value="DHZQ3" />
                                    <f:ListItem Text="星期四" Value="DHZQ4" />
                                    <f:ListItem Text="星期五" Value="DHZQ5" />
                                    <f:ListItem Text="星期六" Value="DHZQ6" />
                                    <f:ListItem Text="星期日" Value="DHZQ7" />
                                </f:DropDownList>
                                <f:DropDownList ID="ddlFLAG" runat="server" Label="配送状态" EnableEdit="true" ForceSelection="true" />
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
                <f:Grid ID="GridGoods" BoxFlex="1" AnchorValue="100% -65" ShowBorder="false" ShowHeader="false" ClicksToEdit="1" OnRowDataBound="GridGoods_RowDataBound"
                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableColumnLines="true" AllowCellEditing="true"
                    DataKeyNames="CODE,STR4" AllowSorting="true" EnableHeaderMenu="true">
                    <Columns>
                        <f:RowNumberField runat="server" Width="35px"></f:RowNumberField>
                        <f:BoundField Width="120px" DataField="CODE" HeaderText="编码" Hidden="true" />
                        <f:BoundField MinWidth="100px" ExpandUnusedSpace="true" DataField="NAME" HeaderText="科室" />
                        <f:RenderField Width="120px" DataField="STR4" ColumnID="STR4" HeaderText="配送员" RendererFunction="renderCode" TextAlign="Center">
                            <Editor>
                                <f:DropDownBox runat="server" ID="ddlUser" EmptyText="" AutoShowClearIcon="true">
                                    <PopPanel>
                                        <f:Grid ID="grdUser" ShowBorder="true" ShowHeader="false" runat="server" ShowGridHeader="false"
                                            Hidden="true" DataIDField="USERID" DataTextField="USERNAME" EnableMultiSelect="false">
                                            <Columns>
                                                <f:RowNumberField></f:RowNumberField>
                                                <f:RenderField ColumnID="USERID" Width="100px" DataField="USERID" Hidden="true" />
                                                <f:RenderField ColumnID="USERNAME" ExpandUnusedSpace="true" DataField="USERNAME" HeaderText="" />
                                            </Columns>
                                        </f:Grid>
                                    </PopPanel>
                                </f:DropDownBox>
                            </Editor>
                        </f:RenderField>
                        <f:RenderField Width="100px" DataField="DHZQ1" ColumnID="DHZQ1" HeaderText="星期一" RendererFunction="renderGender" TextAlign="Center">
                            <Editor>
                                <f:DropDownList runat="server">
                                    <f:ListItem Text="上午送" Value="A"></f:ListItem>
                                    <f:ListItem Text="下午送" Value="P"></f:ListItem>
                                    <f:ListItem Text="不送" Value="N"></f:ListItem>
                                </f:DropDownList>
                            </Editor>
                        </f:RenderField>
                        <f:RenderField Width="100px" HeaderText="星期二" DataField="DHZQ2" ColumnID="DHZQ2" RendererFunction="renderGender" TextAlign="Center">
                            <Editor>
                                <f:DropDownList runat="server">
                                    <f:ListItem Text="上午送" Value="A"></f:ListItem>
                                    <f:ListItem Text="下午送" Value="P"></f:ListItem>
                                    <f:ListItem Text="不送" Value="N"></f:ListItem>
                                </f:DropDownList>
                            </Editor>
                        </f:RenderField>
                        <f:RenderField Width="100px" HeaderText="星期三" ColumnID="DHZQ3" DataField="DHZQ3" RendererFunction="renderGender" TextAlign="Center">
                            <Editor>
                                <f:DropDownList runat="server">
                                    <f:ListItem Text="上午送" Value="A"></f:ListItem>
                                    <f:ListItem Text="下午送" Value="P"></f:ListItem>
                                    <f:ListItem Text="不送" Value="N"></f:ListItem>
                                </f:DropDownList>
                            </Editor>
                        </f:RenderField>
                        <f:RenderField Width="100px" HeaderText="星期四" ColumnID="DHZQ4" DataField="DHZQ4" RendererFunction="renderGender" TextAlign="Center">
                            <Editor>
                                <f:DropDownList runat="server">
                                    <f:ListItem Text="上午送" Value="A"></f:ListItem>
                                    <f:ListItem Text="下午送" Value="P"></f:ListItem>
                                    <f:ListItem Text="不送" Value="N"></f:ListItem>
                                </f:DropDownList>
                            </Editor>
                        </f:RenderField>
                        <f:RenderField Width="100px" HeaderText="星期五" ColumnID="DHZQ5" DataField="DHZQ5" RendererFunction="renderGender" TextAlign="Center">
                            <Editor>
                                <f:DropDownList runat="server">
                                    <f:ListItem Text="上午送" Value="A"></f:ListItem>
                                    <f:ListItem Text="下午送" Value="P"></f:ListItem>
                                    <f:ListItem Text="不送" Value="N"></f:ListItem>
                                </f:DropDownList>
                            </Editor>
                        </f:RenderField>
                        <f:RenderField Width="100px" HeaderText="星期六" ColumnID="DHZQ6" DataField="DHZQ6" RendererFunction="renderGender" TextAlign="Center">
                            <Editor>
                                <f:DropDownList runat="server">
                                    <f:ListItem Text="上午送" Value="A"></f:ListItem>
                                    <f:ListItem Text="下午送" Value="P"></f:ListItem>
                                    <f:ListItem Text="不送" Value="N"></f:ListItem>
                                </f:DropDownList>
                            </Editor>
                        </f:RenderField>
                        <f:RenderField Width="100px" HeaderText="星期日" ColumnID="DHZQ7" DataField="DHZQ7" RendererFunction="renderGender" TextAlign="Center">
                            <Editor>
                                <f:DropDownList runat="server">
                                    <f:ListItem Text="上午送" Value="A"></f:ListItem>
                                    <f:ListItem Text="下午送" Value="P"></f:ListItem>
                                    <f:ListItem Text="不送" Value="N"></f:ListItem>
                                </f:DropDownList>
                            </Editor>
                        </f:RenderField>
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
    <script type="text/javascript">
        function renderGender(value) {
            switch (value) {
                case "A":
                    return "上午送";
                    break;
                case "P":
                    return "下午送";
                    break;
                case "N":
                    return "不送";
                    break;
                default:
                    return "";
                    break;

            }
        }
        var grdUserClientID = '<%= grdUser.ClientID %>';
        function renderCode(value) {
            if (!value) {
                return '';
            }

            var grid2 = F(grdUserClientID);
            return grid2.getRowData(value).text;
        }
        var grid1ClientID = '<%= GridGoods.ClientID %>';
        function updateStyle() {
            var me = F(grid1ClientID);
            me.getRowEls().each(function (index, tr) {
                for (var i = 1; i < 8; i++) {
                    if (me.getCellValue(tr, 'DHZQ' + i) == "A") {
                        $(this).children('td').eq(2 + i).css("background-color", "antiquewhite");
                    }
                    if (me.getCellValue(tr, 'DHZQ' + i) == "P") {
                        $(this).children('td').eq(2 + i).css("background-color", "aquamarine");
                    }
                }
            });
        }
    </script>
</body>
</html>