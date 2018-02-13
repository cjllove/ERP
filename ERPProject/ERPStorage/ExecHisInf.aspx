<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExecHisInf.aspx.cs" Inherits="ERPProject.ERPStorage.ExecHisInf" %>

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
			<Items>
				<f:Grid ID="GridGoods" BoxFlex="1" ShowBorder="false" ShowHeader="false" AllowSorting="false"
					AutoScroll="true" runat="server" DataKeyNames="YAZ01,FLAG"   EnableMultiSelect="true" EnableRowSelectEvent="true"  EnableCheckBoxSelect="true"
					PageSize="50" IsDatabasePaging="true" AllowPaging="true" OnPageIndexChange="GridGoods_PageIndexChange" EnableColumnLines="true">
					<Toolbars>
                        <f:Toolbar runat="server">
                            <Items>
                     <f:ToolbarText runat="server" Text="注释：【手动执行】按照HIS申领数据生成申领单"></f:ToolbarText>
                            </Items>
                        </f:Toolbar>
						<f:Toolbar ID="Toolbar1" runat="server">
							<Items>
								<f:DropDownList ID="srhFLAG" LabelWidth="70px" Width="160px" Label="是否异常" runat="server" CssStyle="margin-left:5px;" EnableEdit="true" ForceSelection="true">
									<f:ListItem Text="--请选择--" Value=""  Selected="true"/>
									<f:ListItem Text="是" Value="Y" />
									<f:ListItem Text="否" Value="N" />
								</f:DropDownList>
						       <f:DatePicker runat="server" ID="dpkBEGRQ" Label="开始时间"></f:DatePicker>
                                <f:DatePicker runat="server" ID="dpkENDRQ" Label="结束时间"></f:DatePicker>
								<f:ToolbarSeparator runat="server"></f:ToolbarSeparator>	
                                                <f:Button ID="btnINIT" EnablePostBack="false" Icon="BookOpen"  EnableDefaultState="false" runat="server" Text="初始化">
                                                    <Menu ID="Menu1" runat="server">
                                                        <f:MenuButton ID="btnInitGoods" Icon="TableAdd" EnablePostBack="true" OnClick="btnInitGoods_Click" runat="server"  Text="商品资料">
                                                        </f:MenuButton>
                                                        <f:MenuButton ID="btnInitUser" Icon="TableAdd" EnablePostBack="true" OnClick="btnInitUser_Click" runat="server"  Text="人员">
                                                        </f:MenuButton>
                                                          <f:MenuButton ID="btnInitDept" Icon="TableAdd" EnablePostBack="true" OnClick="btnInitDept_Click" runat="server"  Text="部门">
                                                        </f:MenuButton>
                                                    </Menu>
                                                </f:Button>
								
                                <f:Button ID="btnNoExec" runat="server" Text="忽略异常" EnableDefaultState="false" OnClick="btnNoExec_Click"></f:Button>						
								<f:Button ID="btExp" Icon="DatabaseGo" Text="导 出" OnClick="btExp_Click" ConfirmText="是否导出当前商品信息?" DisableControlBeforePostBack="false" EnableDefaultState="false"
									EnablePostBack="true" EnableAjax="false" EnableAjaxLoading="true" AjaxLoadingType="Mask" runat="server" />
								<f:ToolbarSeparator runat="server"></f:ToolbarSeparator>
                                <f:Button ID="btnEXEC" Text="手动执行" runat="server"  EnableDefaultState="false" OnClick="btnEXEC_Click"></f:Button>
								<f:Button ID="btnSearch" Icon="Magnifier" Text="查 询" EnablePostBack="true" runat="server" OnClick="btnSearch_Click" EnableDefaultState="false" />
							</Items>
						</f:Toolbar>
					</Toolbars>
					<Columns>
						<f:RowNumberField Width="35" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                        <f:BoundField Width="100px" DataField="YAZ01" Hidden="true" HeaderText="序列号" ></f:BoundField>
                         <f:BoundField Width="100px" DataField="DEPTID"  HeaderText="领取部门编号"></f:BoundField>
                        <f:BoundField DataField="DEPTNAME" HeaderText="领取部门名称"></f:BoundField>
                        <f:BoundField DataField="FLAG"  Hidden="true"></f:BoundField>
                        <f:BoundField Width="80px" DataField="FLAGNAME" HeaderText="状态"></f:BoundField>
                        <f:BoundField DataField="BBY04" HeaderText="商品编码"></f:BoundField>						
						<f:BoundField Width="140px" DataField="GDNAME" HeaderText="商品名称" TextAlign="Left" ExpandUnusedSpace="true" />
						<f:BoundField Width="45px" DataField="VAJ25" HeaderText="数量" />	
                        <f:BoundField Width="115px" DataField="YAZ20" HeaderText="数据生成时间"></f:BoundField>	
                        <f:BoundField Width="115px" DataField="PRODATE" HeaderText="处理生成时间"    ></f:BoundField>			
				        <f:BoundField  ExpandUnusedSpace="true" DataField="MEMO" HeaderText="备注"></f:BoundField>
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
