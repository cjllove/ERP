﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="His_PHMZ_Search.aspx.cs" Inherits="ERPProject.ERPStorage.His_PHMZ_Search" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>商品资料管理</title>
</head>
<body>
	<form id="form1" runat="server">
		<f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="PanelDetail" EnableFormChangeConfirm="true"
			runat="server" />
		<f:Panel ID="PanelDetail" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBOX" ShowHeader="False">
			<Items> <f:Panel ID="Panel3" ShowBorder="false" BodyPadding="0px"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                 <Toolbars>
                        <f:Toolbar runat="server">
                            <Items>
                     <f:ToolbarText runat="server" Text="注：【手动同步】同步HIS计费信息【手动计费】处理HIS计费【手动退费】处理HIS退费"></f:ToolbarText>
                                                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />

                              
                                  <f:Button ID="btnNoExec" runat="server" Text="忽略异常" EnableDefaultState="false" OnClick="btnNoExec_Click"></f:Button>						
														<f:ToolbarSeparator runat="server"></f:ToolbarSeparator>
                                <f:Button ID="btnSYNC" Text="手动同步" runat="server" EnableDefaultState="false" OnClick="btnSYNC_Click" ConfirmText="确定要进行手动同步操作吗？"></f:Button>
                                <f:Button ID="btnEXEC" Text="手动计费" runat="server"  EnableDefaultState="false" OnClick="btnEXEC_Click" ConfirmText="确定要进行手动计费操作吗？"></f:Button>
                               <f:Button ID="btnEXECTF" Text="手动退费" runat="server"  EnableDefaultState="false" OnClick="btnEXECTF_Click" ConfirmText="确定要进行手动退费操作吗？"></f:Button>

                                <f:Button ID="btnExp"  Text="导出" runat="server" EnableDefaultState="false" OnClick="btExp_Click" EnablePostBack="true" EnableAjax="false" EnableAjaxLoading="true"  DisableControlBeforePostBack="false"></f:Button>
								<f:Button ID="btnSearch" Icon="Magnifier" Text="查 询" EnablePostBack="true" runat="server" OnClick="btnSearch_Click" EnableDefaultState="false" />
	
                            </Items>
                        </f:Toolbar>
                         </Toolbars>
                <Items>
						 <f:Form ID="FormSearch" ShowBorder="false" AutoScroll="false" BodyPadding="5px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow ColumnWidths="25% 25% 25% 25%">
                                                    <Items>
								<f:DropDownList ID="srhFLAG" LabelWidth="100px" Width="160px" Label="处理状态" runat="server"  EnableEdit="true" ForceSelection="true">
									<f:ListItem Text="--请选择--" Value=""  Selected="true"/>
									<f:ListItem Text="已处理" Value="G" />
									<f:ListItem Text="待处理" Value="N" />
                                    <f:ListItem Text="异常" Value="X" />
                                    <f:ListItem Text="已忽略" Value="H" />
								</f:DropDownList>
                                <f:DropDownList runat="server" ID="ddlDEPTID" Label="科室名称" EnableEdit="true"></f:DropDownList>
						       <f:DatePicker runat="server" ID="dpkBEGRQ" Label="开始时间"></f:DatePicker>
                                <f:DatePicker runat="server" ID="dpkENDRQ" Label="结束时间"></f:DatePicker>
                                     </Items>  

                                                </f:FormRow></Rows>
                             <Rows>
                                 <f:FormRow>
                                     <Items>
                        <f:TextBox runat="server" ID="txtPATIENT" LabelWidth="100px" Label="病人信息" EmptyText="输入病人编号"></f:TextBox>
                                         <f:TextBox runat="server" ID="txbGDSEQ" Label="商品信息" EmptyText="输入商品编码、名称"></f:TextBox>
                                         <f:DropDownList runat="server" ID="ddlXSTYPE" Label="计费类别">
                                            <f:ListItem Text="--请选择--" Value=""  Selected="true"/>
                                             <f:ListItem  Text="计费" Value="1"/>
                                             <f:ListItem Text="退费" Value="0" />
                                         </f:DropDownList>
                                         <f:DropDownList runat="server" ID="ddlYCTYPE" Label="异常类型">
                                             <f:ListItem Text="--请选择--" Value="" Selected="true" />
                                             <f:ListItem Text="商品编码或名称不符" Value="1" />
                                             <f:ListItem Text="价格不符" Value="2" />
                                             <f:ListItem Text="单位不符" Value="3" />
                                             <f:ListItem Text="库存不足" Value="4" />
                                             <f:ListItem Text="是高值商品" Value="5" />
                                             <f:ListItem Text="规格不符" Value="6" />
                                             <f:ListItem Text="数量是小数" Value="7" />
                                         </f:DropDownList>
                                      </Items>
                                 </f:FormRow>
                             </Rows>
                             
						 </f:Form></Items>
                      </f:Panel>
                      
            <f:Grid ID="GridGoods" BoxFlex="1" ShowBorder="false" ShowHeader="false" AllowSorting="false" CssStyle="border-top: 1px solid #99bce8;"  EnableSummary="true" SummaryPosition="Bottom"
					AutoScroll="true" runat="server" DataKeyNames="SEQNO,FLAG"   EnableMultiSelect="true" EnableRowSelectEvent="true"  EnableCheckBoxSelect="true"
					PageSize="100" IsDatabasePaging="true" AllowPaging="true" OnPageIndexChange="GridGoods_PageIndexChange" EnableColumnLines="true" >
				
					<Columns>
                      <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                        <f:BoundField DataField="FLAG"  Hidden="true"></f:BoundField>
                        <f:BoundField Width="110px" DataField="SEQNO" HeaderText="唯一键" ColumnID="SEQNO"></f:BoundField>
                        <f:BoundField Width="50px" DataField="FLAGNAME" HeaderText="状态"></f:BoundField>
                        <f:BoundField Width="50px" DataField="YCTYPE" HeaderText="异常类别" Hidden="true"></f:BoundField>
                        <f:BoundField Width="105px" DataField="YCTYPENAME" HeaderText="异常类别"></f:BoundField>
                          <f:BoundField Width="110px" DataField="BILLNO"  HeaderText="销售单号"></f:BoundField>
                        <f:BoundField Width="95px" DataField="GDSEQ" HeaderText="商品编码"></f:BoundField>						
						<f:BoundField Width="180px" DataField="GDNAME" HeaderText="商品名称" ColumnID="GDNAME" TextAlign="Left"  />
                        <f:BoundField Width="90px" DataField="GDSPEC" HeaderText="规格"></f:BoundField>
                        <f:BoundField Width="50px" DataField="UNIT" HeaderText="单位"></f:BoundField>
                        <f:BoundField Width="50px" DataField="HSJJ" HeaderText="价格"></f:BoundField>
                        <f:BoundField Width="65px" DataField="SL" HeaderText="数量" ColumnID="SL"></f:BoundField>
                        <f:BoundField Width="70px" DataField="HSJE" HeaderText="金额" ColumnID="HSJE"></f:BoundField>
                        <f:BoundField Width="80px" DataField="PRODUCER" HeaderText="生产厂家"></f:BoundField>
                        <f:BoundField DataField="XSTYPE" Hidden="true"></f:BoundField>
                        <f:BoundField Width="80px" DataField="XSTYPENAME" HeaderText="计费或退费"></f:BoundField>
                        <f:BoundField Width="90px" DataField="DEPTID" HeaderText="科室编码" Hidden="true"></f:BoundField>
                        <f:BoundField Width="90px" DataField="DEPTNAME" HeaderText="科室名称"></f:BoundField>
                        <f:BoundField Width="70px" DataField="PATIENT" HeaderText="病人信息"></f:BoundField>                       
                        <f:BoundField Width="95px" DataField="UPTTIME" HeaderText="数据生成时间" Hidden="true"></f:BoundField>	
                        <f:BoundField Width="110px" DataField="EXECTIME" HeaderText="处理生成时间"    ></f:BoundField>			
				        <f:BoundField  Width="100px" DataField="MEMO" HeaderText="备注">                          
				        </f:BoundField>
					</Columns>
				</f:Grid>			
			</Items>
		</f:Panel>
		<f:HiddenField ID="hfdConfig" runat="server"></f:HiddenField>
		<f:Window ID="Window1" runat="server" IsModal="true" Hidden="true" Target="Parent"
			Width="820px" Height="313px" AutoScroll="false"
			OnClose="Window1_Close">
		</f:Window>
	</form>
</body>
</html>
