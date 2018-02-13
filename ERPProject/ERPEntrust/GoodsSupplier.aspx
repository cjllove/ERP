﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoodsSupplier.aspx.cs" Inherits="ERPProject.ERPEntrust.GoodsSupplier" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>代管供应商商品</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager runat="server" AutoSizePanelID="PanelDetail"></f:PageManager>
        <f:Panel runat="server" ID="PanelDetail" ShowBorder="false" BodyPadding="0px" Layout="Anchor" ShowHeader="false">
            <Items>
                <f:Panel ID="Panel3" ShowBorder="false" BodyPadding="0px" Layout="Anchor" ShowHeader="False" runat="server">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar2" runat="server">
                            <Items>
                                
                                <f:TriggerBox runat="server" ID="trbSearch" LabelWidth="70px" Width="350px" CssStyle="margin-left:15px;" Label="查询信息" EmptyText="可按编号,名称,助记码" TriggerIcon="Search"  OnTriggerClick="trbSearch_TriggerClick" ></f:TriggerBox>
                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                <f:ToolbarSeparator runat="server" />
                                
                                 <f:Button runat="server" ID="btnSearch" CssStyle="margin-left: 10px;margin-right: 10px;" Icon="Disk" Text="查 询" DisableControlBeforePostBack="false" ValidateForms="FormMain,FormAssist"  OnClick="btnSearch_Click"></f:Button>
                                <f:Button runat="server" ID="btnSave" CssStyle="margin-left: 10px;margin-right: 10px;" Icon="Disk" Text="保 存" DisableControlBeforePostBack="false" ValidateForms="FormMain,FormAssist" OnClick="btnSave_Click" ></f:Button>
               
          
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                </f:Panel>

        <f:Grid ID="GridGoods" AnchorValue="100% 95%" ShowBorder="false" ShowHeader="false" AllowSorting="false"
                    AutoScroll="true" runat="server" DataKeyNames="GDSEQ"  EnableMultiSelect="true" EnableCheckBoxSelect="true" 
                    PageSize="50" IsDatabasePaging="true" AllowPaging="true" OnPageIndexChange="GridGoods_PageIndexChange"  EnableRowDoubleClickEvent="true"   OnRowDoubleClick="GridGoods_RowDoubleClick"  > 
             <Columns>
                
                <f:RenderField Width="115px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center"/>
                <f:RenderField Width="140px" DataField="GDNAME" HeaderText="商品名称" />
                <f:RenderField Width="130px" DataField="GDSPEC" HeaderText="商品规格" />
                 <f:RenderField Width="180px" DataField="SUPNAME" HeaderText="供应商" TextAlign="Center">

                           
                 </f:RenderField>
                    
                <f:RenderField Width="180px" DataField="NEWSUPNAME" HeaderText="双击选择新供应商" TextAlign="Center" />
                       
               
                 <f:RenderField Width="180px" DataField="NEWSUPID"  Hidden="true" TextAlign="Center" />
                <f:RenderField Width="140px" DataField="ZJM" HeaderText="助记码" TextAlign="Center" />
                <f:RenderField Width="100px" DataField="CATID0NAME" HeaderText="大类" TextAlign="Center" />
                <f:RenderField Width="100px" DataField="CATIDNAME" HeaderText="类别" TextAlign="Center" />
                <f:RenderField Width="140px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                <f:RenderField Width="130px" DataField="PRODUCERNAME" HeaderText="厂商" TextAlign="Center" />
              
                <f:RenderField Width="175px" DataField="PIZNO" HeaderText="注册证号" TextAlign="Center" />
                <f:RenderField Width="140px" DataField="FLAG" HeaderText="当前状态" TextAlign="Center" />
               
                
                     
            </Columns>
        </f:Grid>


            </Items>
        </f:Panel>
        <f:Window ID="WindowSup" Title="供应商信息" EnableIFrame="false" runat="server" Hidden="true"
            EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True"  Width="600px" Height="400px" Layout="Anchor">
            <Items> 
                <f:Form ID="FormHis" ShowBorder="false" AutoScroll="false" BodyPadding="5px 30px 0px 30px"
                    ShowHeader="False" LabelWidth="60px" runat="server">
                    <Rows>
                        <f:FormRow>
                            <Items>
                                <f:TriggerBox ID="TriggerBox1" runat="server" Label="查询信息"  EmptyText="可输入供应商编码、供应商名称或助记码" TriggerIcon="Search"    OnTriggerClick="TriggerBox1_TriggerClick"  />
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form></Items>

             <Items>
                  
                  <f:Grid ID="GridSu"  ShowBorder="true" AnchorValue="100% -30" ShowHeader="false" EnableRowDoubleClickEvent="true" 
                    AllowSorting="true" AutoScroll="true" runat="server" EnableCheckBoxSelect="true" IsDatabasePaging="true"
                    PageSize="70" AllowPaging="true"  DataKeyNames="SUPID,SUPNAME"    EnableMultiSelect="false" OnPageIndexChange="GridSu_PageIndexChange"   OnRowDoubleClick="GridSu_RowDoubleClick" >
                    <Columns>
                      
                        <f:BoundField Width="100px" DataField="SUPID"    HeaderText ="供应商编号"    EnableColumnHide ="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="400px" DataField="SUPNAME" HeaderText="供应商名称"  EnableColumnHide="true" EnableHeaderMenu="false"  />
                        
                    </Columns>
                </f:Grid>
               
               
            </Items>

            <Toolbars>
                <f:Toolbar ID="Toolbar3" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="btnClosePostBack" Text="确 定" Icon="SystemSave" runat="server"   OnClick="btnClosePostBack_Click">
                        </f:Button>
                        <f:Button ID="btnClose" Text="关 闭" Icon="SystemClose" runat="server"  OnClick="btnClose_Click">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
        <f:HiddenField ID="hfdValue" runat="server" />
        <f:HiddenField ID="Rowid" runat="server" />
          <f:HiddenField ID="hdfgood" runat="server" />
    </form>
</body>
</html>
