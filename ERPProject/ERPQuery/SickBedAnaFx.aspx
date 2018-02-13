<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SickBedAnaFx.aspx.cs" Inherits="ERPProject.ERPQuery.SickBedAnaFx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>科室病房情况分析</title>
     <script src="../res/my97/WdatePicker.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="TabStrip1" runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="0" runat="server">
            <Tabs>
                <f:Tab Title="科室病房排行" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel1" runat="server" ShowBorder="False" BodyPadding="0px" Layout="Anchor"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="PanelBody" ShowBorder="false" BodyPadding="0px"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar1" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText" runat="server" Text=""></f:ToolbarText>
                                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                <f:Button ID="btnAudit" Icon="ArrowRotateClockwise" Text="清 空" OnClick="btClear_Click" EnablePostBack="true" runat="server" EnableDefaultState="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnExp" runat="server" CssStyle="margin-left: 10px;" Icon="DatabaseGo" EnableAjax="false" EnableDefaultState="false"
                                                    OnClick="btExport_Click" EnablePostBack="true" Text="导出" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" OnClick="btSearch_Click" runat="server" EnableDefaultState="false" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="10px 0px 10px 10px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>      
                                                                                                         
                                                        <f:DropDownList ID="lstDEPTID" runat="server" Label="科室" EnableEdit="true" ForceSelection="true">
                                                        </f:DropDownList>
                                                                                     
                                                        <f:TriggerBox ID="trbBEGRQ" runat="server" Label="查询期间" Required="true" TriggerIcon="Date" ShowRedStar="true"></f:TriggerBox>
                                                        <f:TriggerBox ID="trbENDRQ" runat="server" Label="至" ShowRedStar="true" TriggerIcon="Date" Required="true"></f:TriggerBox>
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Panel runat="server" ShowBorder="false" Height="200px" ShowHeader="false" BodyPadding="5px" Layout="HBox">
                                    <Items>
                                        <f:Panel runat="server" ShowBorder="false" ShowHeader="false" BoxFlex="3">
                                            <Items>
                                                <f:ContentPanel runat="server" ShowBorder="false" ShowHeader="false">
                                                    <div style="width: 100%; height: 190px;" id="echart"></div>
                                                </f:ContentPanel>
                                            </Items>
                                        </f:Panel>
                                          <f:Panel runat="server" ShowBorder="false" ShowHeader="false" BoxFlex="2">
                                            <Items>
                                                <f:ContentPanel runat="server" ShowBorder="false" ShowHeader="false">
                                                    <div style="width: 100%; height: 190px;" id="echart2"></div>
                                                </f:ContentPanel>
                                            </Items>
                                        </f:Panel>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridGoods" AnchorValue="100% -284" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;" SummaryPosition="Bottom" EnableSummary="true"
                                    EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridGoods_RowDoubleClick" OnSort="GridGoods_Sort" SortDirection="DESC" SortField="HCSYJE"
                                    AllowSorting="true" AutoScroll="true" runat="server" DataKeyNames="DEPTID" EnableColumnLines="true">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField DataField="DEPTID" ColumnID="DEPTID" SortField="DEPTID" HeaderText="科室编码" Hidden="true" />
                                        <f:BoundField DataField="DEPTNAME" ColumnID="DEPTNAME" SortField="DEPTNAME" HeaderText="科室" ExpandUnusedSpace="true" MinWidth="150px" />
                                          <f:GroupField runat="server" HeaderText="耗材使用数量" TextAlign="Center">
                                            <Columns>
                                                <f:BoundField Width="100px" DataField="HCSYSL" SortField="HCSYSL" ColumnID="HCSYSL" HeaderText="消耗金额数量" TextAlign="Right" DataFormatString="{0:f2}" />
                                                <%--<f:BoundField Width="90px" DataField="HCSLZB" SortField="HCSLZB" ColumnID="HCSLZB" HeaderText="占比" DataFormatString="{0:p2}" TextAlign="Center" />--%>
                                                <f:BoundField Width="90px" DataField="HCSLZB" SortField="HCSYSL" ColumnID="HCSLZB" HeaderText="占比" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField  DataField="HCSYHBZ" ColumnID="HCSYHBZ" Hidden="true"></f:BoundField>
                                                <f:BoundField DataField="HCSYTBZ" ColumnID="HCSYTBZ"  Hidden="true"></f:BoundField>
                                                <%--<f:BoundField Width="90px" DataField="HCSLHB" SortField="HCSLHB" ColumnID="HCSLHB"  HeaderText="环比增长" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField Width="90px" DataField="HCSLTB" SortField="HCSLTB" ColumnID="HCSLTB" HeaderText="同比增长" DataFormatString="{0:p2}" TextAlign="Center" />--%>
                                                <f:BoundField Width="90px" DataField="HCSLHB" SortField="HCSYHBZ" ColumnID="HCSLHB"  HeaderText="环比增长" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField Width="90px" DataField="HCSLTB" SortField="HCSYTBZ" ColumnID="HCSLTB" HeaderText="同比增长" DataFormatString="{0:p2}" TextAlign="Center" />
                                            </Columns>
                                        </f:GroupField>
                                          <f:GroupField runat="server" HeaderText="耗材使用金额" TextAlign="Center">
                                            <Columns>
                                                <f:BoundField Width="100px" DataField="HCSYJE" SortField="HCSYJE" ColumnID="HCSYJE" HeaderText="消耗金额" TextAlign="Right" DataFormatString="{0:f2}" />
                                                <%--<f:BoundField Width="90px" DataField="HCJEZB" SortField="HCJEZB" ColumnID="HCJEZB" HeaderText="占比" DataFormatString="{0:p2}" TextAlign="Center" />--%>
                                                <f:BoundField Width="90px" DataField="HCJEZB" SortField="HCSYJE" ColumnID="HCJEZB" HeaderText="占比" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField DataField="HCJEHBZ" ColumnID="HCJEHBZ" Hidden="true"></f:BoundField>
                                                <f:BoundField DataField="HCJETBZ" ColumnID="HCJETBZ" Hidden="true"></f:BoundField>
                                                <%--<f:BoundField Width="90px" DataField="HCJEHB" SortField="HCJEHB" ColumnID="HCJEHB" HeaderText="环比增长" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField Width="90px" DataField="HCJETB" SortField="HCJETB" ColumnID="HCJETB" HeaderText="同比增长" DataFormatString="{0:p2}" TextAlign="Center" />--%>
                                                <f:BoundField Width="90px" DataField="HCJEHB" SortField="HCJEHBZ" ColumnID="HCJEHB" HeaderText="环比增长" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField Width="90px" DataField="HCJETB" SortField="HCJETBZ" ColumnID="HCJETB" HeaderText="同比增长" DataFormatString="{0:p2}" TextAlign="Center" />
                                            </Columns>
                                        </f:GroupField>                                     
                                       
                                          <f:GroupField runat="server" HeaderText="高耗使用数量" TextAlign="Center">
                                            <Columns>
                                                <f:BoundField Width="100px" DataField="GHSYSL" SortField="GHSYSL" ColumnID="GHSYSL" HeaderText="消耗金额" TextAlign="Right" DataFormatString="{0:f2}" />
                                                <%--<f:BoundField Width="90px" DataField="GHSLZB" SortField="GHSLZB" ColumnID="GHSLZB" HeaderText="占比" DataFormatString="{0:p2}" TextAlign="Center" />--%>
                                                <f:BoundField Width="90px" DataField="GHSLZB" SortField="GHSYSL" ColumnID="GHSLZB" HeaderText="占比" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField DataField="GHSLHBZ" ColumnID="GHSLHBZ" Hidden="true"></f:BoundField>
                                                <f:BoundField DataField="GHSLTBZ" ColumnID="GHSLTBZ" Hidden="true"></f:BoundField>
                                                <%--<f:BoundField Width="90px" DataField="GHSLHB" SortField="GHSLHB" ColumnID="GHSLHB" HeaderText="环比增长" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField Width="90px" DataField="GHSLTB" SortField="GHSLTB" ColumnID="GHSLTB"  HeaderText="同比增长" DataFormatString="{0:p2}" TextAlign="Center" />--%>
                                                <f:BoundField Width="90px" DataField="GHSLHB" SortField="GHSLHBZ" ColumnID="GHSLHB" HeaderText="环比增长" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField Width="90px" DataField="GHSLTB" SortField="GHSLTBZ" ColumnID="GHSLTB"  HeaderText="同比增长" DataFormatString="{0:p2}" TextAlign="Center" />
                                            </Columns>
                                        </f:GroupField>
                                          <f:GroupField runat="server" HeaderText="高耗使用金额" TextAlign="Center">
                                            <Columns>
                                                <f:BoundField Width="100px" DataField="GHSYJE" SortField="SGHYJE" ColumnID="GHSYJE" HeaderText="消耗金额" TextAlign="Right" DataFormatString="{0:f2}" />
                                                <%--<f:BoundField Width="90px" DataField="GHJEZB" SortField="GHJEZB" ColumnID="GHJEZB"  HeaderText="占比" DataFormatString="{0:p2}" TextAlign="Center" />--%>
                                                <f:BoundField Width="90px" DataField="GHJEZB" SortField="SGHYJE" ColumnID="GHJEZB"  HeaderText="占比" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField DataField="GHJEHBZ" ColumnID="GHJEHBZ" Hidden="true"></f:BoundField>
                                                <f:BoundField DataField="GHJETBZ" ColumnID="GHJETBZ" Hidden="true"></f:BoundField>
                                                <%--<f:BoundField Width="90px" DataField="GHJEHB" SortField="GHJEHB"  ColumnID="GHJEHB" HeaderText="环比增长" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField Width="90px" DataField="GHJETB" SortField="GHJETB" ColumnID="GHJETB"  HeaderText="同比增长" DataFormatString="{0:p2}" TextAlign="Center" />--%>
                                                <f:BoundField Width="90px" DataField="GHJEHB" SortField="GHJEHBZ"  ColumnID="GHJEHB" HeaderText="环比增长" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField Width="90px" DataField="GHJETB" SortField="GHJETBZ" ColumnID="GHJETB"  HeaderText="同比增长" DataFormatString="{0:p2}" TextAlign="Center" />
                                            </Columns>
                                        </f:GroupField>
                                           <f:GroupField runat="server" HeaderText="药品消耗数量" TextAlign="Center">
                                            <Columns>
                                                <f:BoundField Width="100px" DataField="YPSYSL" SortField="YPSYSL"  ColumnID="YPSYSL" HeaderText="消耗数量" TextAlign="Right" DataFormatString="{0:f2}" />
                                                <%--<f:BoundField Width="90px" DataField="YPSLZB"  HeaderText="占比" ColumnID="YPSLZB"  DataFormatString="{0:p2}" TextAlign="Center" />--%>
                                                <f:BoundField Width="90px" DataField="YPSLZB"  SortField="YPSYSL" HeaderText="占比" ColumnID="YPSLZB"  DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField DataField="YPSLHBZ" ColumnID="YPSLHBZ" Hidden="true"></f:BoundField>
                                                <f:BoundField DataField="YPSLTBZ" ColumnID="YPSLTBZ" Hidden="true"></f:BoundField>
                                                <%--<f:BoundField Width="90px" DataField="YPSLHB" SortField="YPSLHB" ColumnID="YPSLHB" HeaderText="环比增长" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField Width="90px" DataField="YPSLTB" SortField="YPSLTB" ColumnID="YPSLTB"  HeaderText="同比增长" DataFormatString="{0:p2}" TextAlign="Center" />--%>
                                                <f:BoundField Width="90px" DataField="YPSLHB" SortField="YPSLHBZ" ColumnID="YPSLHB" HeaderText="环比增长" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField Width="90px" DataField="YPSLTB" SortField="YPSLTBZ" ColumnID="YPSLTB"  HeaderText="同比增长" DataFormatString="{0:p2}" TextAlign="Center" />
                                            </Columns>
                                        </f:GroupField>
                                        <f:GroupField runat="server" HeaderText="药品消耗金额" TextAlign="Center">
                                            <Columns>
                                                <f:BoundField Width="100px" DataField="YPSYJE" SortField="YPSYJE" ColumnID="YPSYJE" HeaderText="消耗金额" TextAlign="Right" DataFormatString="{0:f2}" />
                                                <%--<f:BoundField Width="90px" DataField="YPJEZB" SortField="YPJEZB" ColumnID="YPJEZB"  HeaderText="占比" DataFormatString="{0:p2}" TextAlign="Center" />--%>
                                                <f:BoundField Width="90px" DataField="YPJEZB" SortField="YPSYJE" ColumnID="YPJEZB"  HeaderText="占比" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField DataField="YPJEHBZ" ColumnID="YPJEHBZ" Hidden="true"></f:BoundField>
                                                <f:BoundField DataField="YPJETBZ" ColumnID="YPJETBZ" Hidden="true"></f:BoundField>
                                                <%--<f:BoundField Width="90px" DataField="YPJEHB" SortField="YPJEHB" ColumnID="YPJEHB"  HeaderText="环比增长" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField Width="90px" DataField="YPJETB" SortField="YPJETB" ColumnID="YPJETB" HeaderText="铜板增长"  DataFormatString="{0:p2}" TextAlign="Center"></f:BoundField>--%>
                                                <f:BoundField Width="90px" DataField="YPJEHB" SortField="YPJEHBZ" ColumnID="YPJEHB"  HeaderText="环比增长" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField Width="90px" DataField="YPJETB" SortField="YPJETBZ" ColumnID="YPJETB" HeaderText="铜板增长"  DataFormatString="{0:p2}" TextAlign="Center"></f:BoundField>
                                            </Columns>
                                        </f:GroupField>
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="科室病房明细排行" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel2" runat="server" ShowBorder="False" BodyPadding="0px" Layout="Anchor"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="Panel3" ShowBorder="false" BodyPadding="0px"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar2" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="操作信息：按订货日期倒序排列！" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="btnCl" Icon="ArrowRotateClockwise" Text="清 空" OnClick="btClear_Click" EnablePostBack="true" runat="server" EnableDefaultState="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnExpt" runat="server" CssStyle="margin-left: 10px;" Icon="DatabaseGo" EnableAjax="false" EnableDefaultState="false"
                                                    OnClick="btExport_Click" EnablePostBack="true" Text="导出" DisableControlBeforePostBack="false"/>
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnSch" Icon="Magnifier" Text="查 询" OnClick="btnSch_Click" runat="server" EnableDefaultState="false" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="FormList" ShowBorder="false" AutoScroll="false" BodyPadding="10px 0px 10px 10px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="ddlDEPTID" runat="server" Label="科室" EnableEdit="true" ForceSelection="true">
                                                        </f:DropDownList>                                                      
                                                          <f:TriggerBox ID="tbxBEGRQ" runat="server" Label="查询期间" Required="true" TriggerIcon="Date" ShowRedStar="true"></f:TriggerBox>
                                                        <f:TriggerBox ID="tbxENDRQ" runat="server" Label="至" ShowRedStar="true" TriggerIcon="Date" Required="true"></f:TriggerBox>
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                     <f:Panel runat="server" ShowBorder="false" Height="200px" ShowHeader="false" BodyPadding="5px" Layout="HBox">
                                    <Items>
                                        <f:Panel runat="server" ShowBorder="false" ShowHeader="false" BoxFlex="1">
                                            <Items>
                                                <f:ContentPanel runat="server" ShowBorder="false" ShowHeader="false">
                                                    <div style="width: 100%; height: 190px;" id="echart3"></div>
                                                </f:ContentPanel>
                                            </Items>
                                        </f:Panel>     
                                         <%--<f:Panel runat="server" ShowBorder="false" ShowHeader="false" BoxFlex="1">
                                            <Items>
                                                <f:ContentPanel runat="server" ShowBorder="false" ShowHeader="false">
                                                    <div style="width: 100%; height: 190px;" id="echart4"></div>
                                                </f:ContentPanel>
                                            </Items>
                                        </f:Panel>       --%>                                    
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" AnchorValue="100% -282" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;" SummaryPosition="Bottom" EnableSummary="true"
                                    AllowPaging="true" IsDatabasePaging="true" SortField="HSJE" SortDirection="DESC" PageSize="50" OnSort="GridList_Sort" OnPageIndexChange="GridGoods_PageIndexChange"
                                    AllowSorting="true" AutoScroll="true" runat="server" DataKeyNames="SUPID" EnableColumnLines="true">
                                    <Columns>
                                        <f:RowNumberField Width="40px"></f:RowNumberField>
                                        <f:BoundField Width="110px" DataField="MONTHID" SortField="MONTHID" HeaderText="月份" />
                                        <f:BoundField Width="220px" DataField="DEPTID" ColumnID="DEPTID" SortField="DEPTID" Hidden="true" HeaderText="部门编码" />
                                        <f:BoundField Width="110px" DataField="DEPTNAME" ColumnID="DEPTNAME" SortField="DEPTNAME" HeaderText="科室" />                                       
                                        <f:GroupField runat="server" HeaderText="数量分析" TextAlign="Center">
                                            <Columns>
                                                <%--<f:BoundField Width="100px" DataField="SL" ColumnID="SL2" SortField="SYSL" HeaderText="消耗数量" DataFormatString="{0:f2}" TextAlign="Right" />--%>
                                                <f:BoundField Width="100px" DataField="SL" ColumnID="SL2" SortField="SLZB" HeaderText="消耗数量" DataFormatString="{0:f2}" TextAlign="Right" />
                                                <f:BoundField Width="90px" DataField="SLZB" SortField="SLZB" HeaderText="数量占比" TextAlign="Right" DataFormatString="{0:p}" />
                                                <f:BoundField Width="110px" DataField="SLTB" SortField="SLTB" HeaderText="同比" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField Width="110px" DataField="SLHB" SortField="SLHB" HeaderText="环比" DataFormatString="{0:p2}" TextAlign="Center" />
                                            </Columns>
                                        </f:GroupField>
                                        <f:GroupField runat="server" HeaderText="金额分析" TextAlign="Center">
                                            <Columns>
                                                <%--<f:BoundField Width="110px" DataField="HSJE" ColumnID="HSJE2" HeaderText="消耗金额" DataFormatString="{0:f2}" TextAlign="Right" />--%>

                                                <f:BoundField Width="110px" DataField="HSJE" SortField="JEZB" ColumnID="HSJE2" HeaderText="消耗金额" DataFormatString="{0:f2}" TextAlign="Right" />
                                                <f:BoundField Width="100px" DataField="JEZB" SortField="JEZB" HeaderText="金额占比" TextAlign="Right" DataFormatString="{0:p}" />
                                                <f:BoundField Width="110px" DataField="JETB" SortField="JETB" HeaderText="同比" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField Width="110px" DataField="JEHB" SortField="JEHB" HeaderText="环比" DataFormatString="{0:p2}" TextAlign="Center" />
                                            </Columns>
                                        </f:GroupField>                                     
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
        <f:HiddenField ID="hdfdeptid" runat="server"></f:HiddenField>
          <f:HiddenField ID="hdfsupid" runat="server"></f:HiddenField>
         <f:HiddenField ID="Totalsl" runat="server"></f:HiddenField>
        <f:HiddenField ID="Totalje" runat="server"></f:HiddenField>
        <f:HiddenField ID="totalhcsl" runat="server"></f:HiddenField>
        <f:HiddenField ID="totalhcje" runat="server"></f:HiddenField>
        <f:HiddenField ID="totalghsl" runat="server"></f:HiddenField>
        <f:HiddenField ID="totalghje" runat="server"></f:HiddenField>
         <f:HiddenField ID="hfdArray" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdArrayVal" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdArrayVal2" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdArray3" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdArrayVal3" runat="server"></f:HiddenField>
    </form>
    <script src="/res/js/echarts.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        function updateDate() {
            var me = F('<%= GridGoods.ClientID %>'), Total = 0, TbTotal = 0, HbTotal = 0;
             var Totalsl = F('<%= Totalsl.ClientID%>').getValue();
            var Totalje = F('<%= Totalje.ClientID%>').getValue();
            var totalhcsl = F('<%= totalhcsl.ClientID%>').getValue();
            var totalhcje = F('<%= totalhcje.ClientID%>').getValue(); 
            var totalghsl = F('<%= totalghsl.ClientID%>').getValue();
            var totalghje = F('<%= totalghje.ClientID%>').getValue();

             me.getRowEls().each(function (index, tr) {
                 Total = me.getCellValue(tr, 'YPSYSL');
                 TbTotal = me.getCellValue(tr, 'YPSLTBZ');
                 HbTotal = me.getCellValue(tr, 'YPSLHBZ');               
                 me.updateCellValue(index, 'YPSLTB', TbTotal == 0 ? '0%' : ((Total / TbTotal - 1) * 100).toFixed(2) + '%', true);
                 me.updateCellValue(index, 'YPSLHB', HbTotal == 0 ? '0%' : ((Total / HbTotal - 1) * 100).toFixed(2) + '%', true);
                 me.updateCellValue(index, 'YPSLZB', Total == 0 ? '0%' : ((Total / Totalsl) * 100).toFixed(2) + '%', true);
                 Total = me.getCellValue(tr, 'YPSYJE');
                 TbTotal = me.getCellValue(tr, 'YPJETBZ');
                 HbTotal = me.getCellValue(tr, 'YPJEHBZ');               
                 me.updateCellValue(index, 'YPJETB', TbTotal == 0 ? '0%' : ((Total / TbTotal - 1) * 100).toFixed(2) + '%', true);
                 me.updateCellValue(index, 'YPJEHB', HbTotal == 0 ? '0%' : ((Total / HbTotal - 1) * 100).toFixed(2) + '%', true);
                 me.updateCellValue(index, 'YPJEZB', Total == 0 ? '0%' : ((Total / Totalje) * 100).toFixed(2) + '%', true);
                 Total = me.getCellValue(tr, 'HCSYSL');
                 TbTotal = me.getCellValue(tr, 'HCSYTBZ');
                 HbTotal = me.getCellValue(tr, 'HCSYHBZ');                
                 me.updateCellValue(index, 'HCSLTB', TbTotal == 0 ? '0%' : ((Total / TbTotal - 1) * 100).toFixed(2) + '%', true);
                 me.updateCellValue(index, 'HCSLHB', HbTotal == 0 ? '0%' : ((Total / HbTotal - 1) * 100).toFixed(2) + '%', true);
                 me.updateCellValue(index, 'HCSLZB', Total == 0 ? '0%' : ((Total / totalhcsl) * 100).toFixed(2) + '%', true);
                 Total = me.getCellValue(tr, 'HCSYJE');
                 TbTotal = me.getCellValue(tr, 'HCJETBZ');
                 HbTotal = me.getCellValue(tr, 'HCJEHBZ');              
                 me.updateCellValue(index, 'HCJETB', TbTotal == 0 ? '0%' : ((Total / TbTotal - 1) * 100).toFixed(2) + '%', true);
                 me.updateCellValue(index, 'HCJEHB', HbTotal == 0 ? '0%' : ((Total / HbTotal - 1) * 100).toFixed(2) + '%', true);
                 me.updateCellValue(index, 'HCJEZB', Total == 0 ? '0%' : ((Total / totalhcje) * 100).toFixed(2) + '%', true);
               
                 Total = me.getCellValue(tr, 'GHSYSL');
                 TbTotal = me.getCellValue(tr, 'GHSLTBZ');
                 HbTotal = me.getCellValue(tr, 'GHSLHBZ');                
                 me.updateCellValue(index, 'GHSLTB', TbTotal == 0 ? '0%' : ((Total / TbTotal - 1) * 100).toFixed(2) + '%', true);
                 me.updateCellValue(index, 'GHSLHB', HbTotal == 0 ? '0%' : ((Total / HbTotal - 1) * 100).toFixed(2) + '%', true);
                 me.updateCellValue(index, 'GHSLZB', Total == 0 ? '0%' : ((Total / totalghsl) * 100).toFixed(2) + '%', true);
                 Total = me.getCellValue(tr, 'GHSYJE');
                 TbTotal = me.getCellValue(tr, 'GHJETBZ');
                 HbTotal = me.getCellValue(tr, 'GHJEHBZ');               
                 me.updateCellValue(index, 'GHJETB', TbTotal == 0 ? '0%' : ((Total / TbTotal - 1) * 100).toFixed(2) + '%', true);
                 me.updateCellValue(index, 'GHJEHB', HbTotal == 0 ? '0%' : ((Total / HbTotal - 1) * 100).toFixed(2) + '%', true);
                 me.updateCellValue(index, 'GHJEZB', Total == 0 ? '0%' : ((Total / totalghje) * 100).toFixed(2) + '%', true);            
             });
         }
        function showbar(data1, datamonth, datayp, datahc, datagh, datapj, data_max_je, data_int_je, data_max_rc, data_int_rc) {
            var myChart = echarts.init(document.getElementById('echart'));
            option = {
                tooltip: {
                    trigger: 'axis'
                },
                toolbox: {
                    feature: {
                       
                    }
                },
                legend: {
                    data: data1
                },
                xAxis: [
                    {
                        type: 'category',
                        data: datamonth
                    }
                ],
                yAxis: [
                    {
                        type: 'value',
                        name: '金额',
                        min: 0,
                        max: data_max_je,
                        interval: data_int_je,
                        axisLabel: {
                            formatter: '{value} 元'
                        }
                    },
                    {
                        type: 'value',
                        name: '人床数',
                        min: 0,
                        max: data_max_rc,
                        interval: data_int_rc,
                        axisLabel: {
                            formatter: '{value} 个'
                        }
                    }
                ],
                series: [
                    {
                        name: '药品使用金额',
                        type: 'bar',
                        data: datayp
                    },
                    {
                        name: '耗材使用金额',
                        type: 'bar',
                        data: datahc
                    },
                     {
                         name: '高耗使用金额',
                         type: 'bar',
                         data: datagh
                     },
                    {
                        name: '人床数',
                        type: 'line',
                        yAxisIndex: 1,
                        data: datapj
                    }
                ]
            };
            myChart.setOption(option);
        };
        function showpie() {
            var datatitl = F('<%= hfdArray.ClientID%>').getValue().split(",");
            var hfdArrayVal = F('<%= hfdArrayVal.ClientID%>').getValue().split(",");
            var dataVal = new Array();
            for (var i = 0; i < hfdArrayVal.length; i++) {
                dataVal.push(eval('(' + '{"value":"' + hfdArrayVal[i].split("$")[0] + '", "name":"' + hfdArrayVal[i].split("$")[1] + '"}' + ')'));
            }
            if (hfdArrayVal.length < 1) { dataVal.push(eval('(' + '{"value":"' + "无数据" + '", "name":"' + "无数据" + ')')); }
            var myChart = echarts.init(document.getElementById('echart2'));
            var option = {
                title: {
                    text: '科室人床数量占比',
                    subtext: '',
                    x: 'center'
                },
                tooltip: {
                    trigger: 'item',
                    formatter: "{a} <br/>{b} : {c} ({d}%)"
                },
                legend: {
                    orient: 'vertical',
                    left: 'left',
                    data: datatitl
                },
                series: [
                    {
                        name: '来源',
                        type: 'pie',
                        radius: '55%',
                        center: ['70%', '60%'],
                        data: dataVal,
                        itemStyle: {
                            emphasis: {
                                shadowBlur: 10,
                                shadowOffsetX: 0,
                                shadowColor: 'rgba(0, 0, 0, 0.5)'
                            }
                        }
                    }
                ]
            };
            myChart.setOption(option);
        };
        function showpie3(data1, data2) {
            var datatitl = F('<%= hfdArray3.ClientID%>').getValue().split(",");
            var hfdArrayVal = F('<%= hfdArrayVal3.ClientID%>').getValue().split(",");
            var dataVal = new Array();
            for (var i = 0; i < hfdArrayVal.length; i++) {
                dataVal.push(eval('(' + '{"value":"' + hfdArrayVal[i].split("$")[0] + '", "name":"' + hfdArrayVal[i].split("$")[1] + '"}' + ')'));
            }

            var myChart = echarts.init(document.getElementById('echart3'));
            var option = {
                title: {
                    text: '科室月消耗金额占比',
                    subtext: '',
                    x: 'center'
                },
                tooltip: {
                    trigger: 'item',
                    formatter: "{a} <br/>{b} : {c} ({d}%)"
                },
                legend: {
                    orient: 'vertical',
                    left: 'left',
                    data: datatitl
                },
                series: [
                    {
                        name: '来源',
                        type: 'pie',
                        radius: '55%',
                        center: ['70%', '60%'],
                        data: dataVal,
                        itemStyle: {
                            emphasis: {
                                shadowBlur: 10,
                                shadowOffsetX: 0,
                                shadowColor: 'rgba(0, 0, 0, 0.5)'
                            }
                        }
                    }
                ]
            };
            myChart.setOption(option);
        };
    </script>
    <script type="text/javascript">    
        var trbBEGRQClientID = '<%= trbBEGRQ.ClientID %>';
        var trbENDRQClientID = '<%= trbENDRQ.ClientID %>';
        var tbxBEGRQClientID = '<%= tbxBEGRQ.ClientID %>';
        var tbxENDRQClientID = '<%= tbxENDRQ.ClientID %>';
        F.ready(function () {
            var trbBEGRQ = F(trbBEGRQClientID);
            var trbENDRQ = F(trbENDRQClientID);
            var tbxBEGRQ = F(tbxBEGRQClientID);
            var tbxENDRQ = F(tbxENDRQClientID);

            trbBEGRQ.onTriggerClick = function () {
                WdatePicker({
                    el: trbBEGRQClientID + '-inputEl',
                    //dateFmt: 'yyyy-MM-dd HH:mm:ss',
                    dateFmt: 'yyyy-MM',
                    onpicked: function () {
                        // 确认选择后，执行触发器输入框的客户端验证
                        trbBEGRQ.validate();
                    }
                });
            };
            trbENDRQ.onTriggerClick = function () {
                WdatePicker({
                    el: trbENDRQClientID + '-inputEl',
                    //dateFmt: 'yyyy-MM-dd HH:mm:ss',
                    dateFmt: 'yyyy-MM',
                    onpicked: function () {
                        // 确认选择后，执行触发器输入框的客户端验证
                        trbENDRQ.validate();
                    }
                });
            };
            tbxBEGRQ.onTriggerClick = function () {
                WdatePicker({
                    el: tbxBEGRQClientID + '-inputEl',
                    //dateFmt: 'yyyy-MM-dd HH:mm:ss',
                    dateFmt: 'yyyy-MM',
                    onpicked: function () {
                        // 确认选择后，执行触发器输入框的客户端验证
                        tbxBEGRQ.validate();
                    }
                });
            };
            tbxENDRQ.onTriggerClick = function () {
                WdatePicker({
                    el: tbxENDRQClientID + '-inputEl',
                    //dateFmt: 'yyyy-MM-dd HH:mm:ss',
                    dateFmt: 'yyyy-MM',
                    onpicked: function () {
                        // 确认选择后，执行触发器输入框的客户端验证
                        tbxENDRQ.validate();
                    }
                });
            };
        })
    </script>
</body>
</html>
