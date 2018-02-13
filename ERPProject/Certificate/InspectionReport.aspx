<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InspectionReport.aspx.cs" Inherits="ERPProject.Certificate.InspectionReport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>检验报告浏览</title>
        <style>
    .w1000 {
        margin:0 auto;
        width:90%;
    }
    .w1000 img {
        width:100%;
    }
    </style>
</head>
<body>
    <form id="form1" runat="server">
          <f:PageManager EnableAjaxLoading="true" ID="PageManager1" AjaxLoadingType="Mask" AutoSizePanelID="TabStrip1" runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="1" runat="server">
            <Tabs>
                <f:Tab Title="检验报告查询" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel3" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="Panel4" ShowBorder="false" BodyPadding="0px"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar2" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText1" Text="检验报告查询：" runat="server"></f:ToolbarText>
                                                <f:TriggerBox ID="tgbSearch" Width="400px" runat="server" EmptyText="按流水号或物料编号查询" TriggerIcon="Search" OnTriggerClick="btnSearch_Click" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                </f:Panel>
                                <f:Grid ID="GridList" AnchorValue="100% -25" ShowBorder="false" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server"
                                     EnableCheckBoxSelect="false" AllowPaging="true"   OnPageIndexChange="GridList_PageIndexChange"
                                    DataKeyNames="SEQNO,FLAG" EnableRowDoubleClickEvent="true" EnableColumnLines="true" OnRowDoubleClick="GridList_RowDoubleClick" PageSize="20">
                                    <Columns>
                                        <f:RowNumberField Width="30px" TextAlign="Center"></f:RowNumberField>
                                        <f:BoundField Width="130px" DataField="SEQNO" HeaderText="流水号" />
                                        <f:BoundField Width="90px" DataField="WLSEQ" HeaderText="物料编码" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="FLAG" HeaderText="状态" TextAlign="Center" />
                                        <f:BoundField Width="210px" DataField="GDNAME" HeaderText="商品名称" TextAlign="Center" />
                                        <f:BoundField Width="110px" DataField="GDSPEC" HeaderText="商品规格" TextAlign="Center" />
                                        <%--<f:BoundField Width="90px" DataField="CDID" HeaderText="产地" TextAlign="Center" />--%>
                                        <f:BoundField Width="90px" DataField="PH" HeaderText="批号" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="MJPH" HeaderText="灭菌批号" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="ENDRQ" HeaderText="有效期至" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="80px" DataField="LRY" HeaderText="录入员" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="LRRQ" HeaderText="录入日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
                                        <f:BoundField Width="80px" DataField="SHR" HeaderText="审核员" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="SHRQ" HeaderText="审核日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="100px" DataField="MEMO" HeaderText="备注" TextAlign="Center" ExpandUnusedSpace="true" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="检验报告录入" Icon="PageWord" Layout="Fit" runat="server">
                       <Items>
                           <f:Panel ID="panelPicture" runat="server" ShowBorder="false" BodyPadding="0px" Layout="HBox" BoxConfigAlign="Stretch" BoxConfigPosition="Start" BoxConfigChildMargin="0" ShowHeader="False">
                           <Items>
                               <f:Panel ID="gridPicList" runat="server" ShowBorder="false" Width="450px" BodyPadding="0" ShowHeader="false" Layout="Fit">
                                                 <Items>
                                                     <f:Grid ID="gridPic"  ShowBorder="false" ShowHeader="false" AllowSorting="false" EnableCheckBoxSelect="false"
                                                            AutoScroll="true"  runat="server" PageSize="10" EnableRowClickEvent="true"
                                                                 DataKeyNames="SEQNO,ROWNO"  OnRowClick ="gridPic_RowClick">
                                                         <Columns>
                                                            <f:RowNumberField Width="25" TextAlign="Center" />
                                                            <f:BoundField DataField="LSNO" Width="130px" HeaderText="证照流水" TextAlign="Center"/>
                                                            <f:BoundField DataField="ROWNO" Width="50px" HeaderText="系数" TextAlign="Center"/>
                                                            <f:BoundField DataField="LIS_PICT" Width="120px" HeaderText="图片名" TextAlign="Center" />
                                                            <f:BoundField DataField="PICPATH" Width="103px" HeaderText="图片路径" TextAlign="Center" ExpandUnusedSpace="true" />
                                                         </Columns>
                                                     </f:Grid>
                                                 </Items>
                                             </f:Panel>
                                             
                                             <f:Panel ID="Panelp" runat="server" ShowBorder="false" ShowHeader="false" BoxFlex="2"
                                                  BodyPadding="0" Layout="VBox" BoxConfigAlign="Stretch" BoxConfigPosition="Start" BoxConfigChildMargin="0">
                                                 <Items>
                                                    <f:Panel ID="panelPic" runat="server" BoxFlex="2" ShowBorder="false" CssStyle="border-left: 1px solid #99bce8; border-top:0px;"  ShowHeader="false" Width="500px" BodyPadding="0" Layout="Fit">
                                                     <Items>
                                                       <f:Form ID="FormPic" ShowBorder="false" AutoScroll="false" CssStyle="padding:0px;" ShowHeader="False" LabelWidth="80px" runat="server">
                                                            <Rows>
                                                              <f:FormRow>
                                                                <Items>
                                                                    <f:Image ID="imgBMPPATH" CssClass="w1000" Height="250px" runat="server" ImageUrl="~/res/images/model.png"/>
                                                                </Items>
                                                              </f:FormRow>
                                                            </Rows>
                                                       </f:Form>
                                                     </Items>
                                                   </f:Panel>
                                                 </Items>
                                             </f:Panel>
                           </Items>
                       </f:Panel>
                       </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
    </form>

<%--            <script>
                var imgBMPPATH = '<%= imgBMPPATH.ClientID%>'; //图片点击看大图
                F.ready(function () {
                    $('#' + imgBMPPATH).on('click', function (e) { window.open($(e.currentTarget).find('img').eq(0).attr('src')) });
                });
    </script>--%>
</body>
</html>
