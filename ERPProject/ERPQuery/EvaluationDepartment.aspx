<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EvaluationDepartment.aspx.cs" Inherits="ERPProject.ERPQuery.EvaluationDepartment" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="TabStrip1" runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right" EnableTabCloseMenu="false" ActiveTabIndex="1" runat="server">
            <Tabs>
                <f:Tab Title="评价列表" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel2" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="Panel3" ShowBorder="false" BodyPadding="0px" Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar2" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="操作信息：" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="bntClear" Icon="Erase" Text="清 除" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="70px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="lstDEPTID" runat="server" Label="科室" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="lstSUPID" runat="server" Label="评价等级" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="lstPJSJ1" runat="server" Label="评价时间" Required="true" ShowRedStar="true" />
                                                        <f:DatePicker ID="lstPJSJ2" runat="server" Label="　　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" AnchorValue="100% -95" ShowBorder="false" ShowHeader="false"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;"
                                    DataKeyNames="SEQNO,FLAG" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick"
                                    EnableMultiSelect="true" EnableCheckBoxSelect="true" CheckBoxSelectOnly="true">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField Width="110px" DataField="DEPTID" HeaderText="科室名称" TextAlign="Center" />
                                        <f:BoundField Width="110px" DataField="" HeaderText="评价等级" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="" HeaderText="评价人" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="" HeaderText="评价时间" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="150px" DataField="" HeaderText="评价内容" />
                                    </Columns>
                                    <Listeners>
                                        <f:Listener Event="cellkeydown" Handler="onCellClick" />
                                        <f:Listener Event="cellmousedown" Handler="onMouseDown" />
                                    </Listeners>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="评价信息" Icon="PageWord" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="PanelBody" ShowBorder="false" BodyPadding="0px"
                            Layout="Anchor" ShowHeader="False" runat="server">
                            <Toolbars>
                                <f:Toolbar ID="Toolbar1" runat="server">
                                    <Items>
                                        <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="感谢您留下宝贵的意见和评价" runat="server" />
                                        <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btnSave" Icon="Disk" Text="保 存" EnablePostBack="true" ValidateForms="FormDoc" runat="server" OnClick="btnBill_Click" />
                                    </Items>
                                </f:Toolbar>
                            </Toolbars>
                            <Items>
                                <f:DropDownList ID="OJXXDEPTID" runat="server" Label="管理科室" Width="300px"/>
                                <f:DatePicker ID="DQRI" Label="操作时间" runat="server" Width="300px" ></f:DatePicker>
                                <f:TextBox ID="docWeek" runat="server" Label="星期" Width="300px" />
                            </Items>

                            <Items>
                                <f:RadioButtonList ID="ZTPJ" Label="总体评价" runat="server">
                                    <f:RadioItem Text="好评" Value="好评" Selected="true" />
                                    <f:RadioItem Text="中评" Value="中评" />
                                    <f:RadioItem Text="差评" Value="差评" />
                                </f:RadioButtonList>
                            </Items>
                            <Items>
                                <f:TextArea runat="server" ID="FKJY" EmptyText="亲，感谢您留下评价，我们一定会做的更好。" Label="反馈建议"
                                    AutoGrowHeight="true" AutoGrowHeightMin="100" AutoGrowHeightMax="200">
                                </f:TextArea>
                            </Items>
                            <Items>
                                <f:RadioButtonList ID="SHJSX" Label="送货及时性" runat="server">
                                    <f:RadioItem Text="1" Value="1" />
                                    <f:RadioItem Text="2" Value="2" />
                                    <f:RadioItem Text="3" Value="3" />
                                    <f:RadioItem Text="4" Value="4" />
                                    <f:RadioItem Text="5" Value="5" Selected="true" />
                                </f:RadioButtonList>
                                <f:RadioButtonList ID="SHYTD" Label="送货员态度" runat="server">
                                    <f:RadioItem Text="1" Value="1" />
                                    <f:RadioItem Text="2" Value="2" />
                                    <f:RadioItem Text="3" Value="3" />
                                    <f:RadioItem Text="4" Value="4" />
                                    <f:RadioItem Text="5" Value="5" Selected="true" />
                                </f:RadioButtonList>
                                <f:RadioButtonList ID="SHMZL" Label="送货满足率" runat="server">
                                    <f:RadioItem Text="1" Value="1" />
                                    <f:RadioItem Text="2" Value="2" />
                                    <f:RadioItem Text="3" Value="3" />
                                    <f:RadioItem Text="4" Value="4" />
                                    <f:RadioItem Text="5" Value="5" Selected="true" />
                                </f:RadioButtonList>
                                <f:RadioButtonList ID="TMHSJSX" Label="条码回收及时性" runat="server">
                                    <f:RadioItem Text="1" Value="1" />
                                    <f:RadioItem Text="2" Value="2" />
                                    <f:RadioItem Text="3" Value="3" />
                                    <f:RadioItem Text="4" Value="4" />
                                    <f:RadioItem Text="5" Value="5" Selected="true" />
                                </f:RadioButtonList>
                                <f:RadioButtonList ID="CLWTJSX" Label="处理问题及时性" runat="server">
                                    <f:RadioItem Text="1" Value="1" />
                                    <f:RadioItem Text="2" Value="2" />
                                    <f:RadioItem Text="3" Value="3" />
                                    <f:RadioItem Text="4" Value="4" />
                                    <f:RadioItem Text="5" Value="5" Selected="true" />
                                </f:RadioButtonList>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
    </form>
</body>
</html>
