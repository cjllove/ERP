<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConstantWarning.aspx.cs" Inherits="ERPProject.ERPApply.ConstantWarning" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
	<title>定数缺货转出库审核</title>
	<script src="/res/js/CreateControl.js" type="text/javascript"></script>
	<script src="/res/js/GRInstall.js" type="text/javascript"></script>
	<script src="/res/js/jquery.ymh.js" type="text/javascript"></script>
	<style type="text/css" media="all">
		.x-grid-row-summary .x-grid-cell-inner {
			font-weight: bold;
			color: blue;
		}

		.x-grid-row-summary .x-grid-cell {
			background-color: #fff !important;
		}

		.x-grid-row.highlight td {
			background-color: lightgreen;
			background-image: none;
		}

		.x-grid-row.highlightyel td {
			background-color: yellow;
			background-image: none;
		}

		.x-grid-row.SelectColor td {
			background-color: #B8CFEE;
			background-image: none;
		}
	</style>
</head>
<body>
	<form id="form1" runat="server">
		<f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="PanelMain"
			runat="server" />
		<f:Panel ID="PanelMain" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBox"
			ShowHeader="false">
			<Toolbars>
				<f:Toolbar ID="Toolbar2" runat="server">
					<Items>
						<f:ToolbarText ID="ToolbarText2" CssStyle="" Text="公式说明：建议申领定数  = 定数数量 - 待出定数 - 库存定数（定数+非定数/定数含量）" runat="server" />
						<f:ToolbarFill ID="ToolbarFill1" runat="server" />
						<f:ToolbarSeparator runat="server" />
						<f:Button ID="btnClear" Icon="Erase" Text="清 除" EnablePostBack="true" runat="server" OnClick="btnClear_Click" />
						<f:ToolbarSeparator runat="server" />
						<f:Button ID="btnAudit" Icon="UserTick" Text="审 核" EnablePostBack="true" runat="server" OnClick="btnAudit_Click" DisableControlBeforePostBack="true" />
						<f:ToolbarSeparator runat="server" />
						<f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnablePostBack="true" runat="server" OnClick="bntSearch_Click" />
					</Items>
				</f:Toolbar>
			</Toolbars>
			<Items>
				<f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
					ShowHeader="False" LabelWidth="75px" runat="server">
					<Rows>
						<f:FormRow>
							<Items>
								<f:DropDownList ID="ddlDEPTOUT" runat="server" Label="出库部门" EnableEdit="true" ForceSelection="true" ShowRedStar="true" />
								<f:DropDownList ID="ddlDEPTID" runat="server" Label="申领科室" EnableEdit="true" ForceSelection="true">
								</f:DropDownList>
								<f:TextBox ID="tbxGDSEQ" runat="server" Label="商品信息" EmptyText="请输入商品编码、名称或助记码"></f:TextBox>
							</Items>
						</f:FormRow>
					</Rows>
				</f:Form>
				<f:Grid ID="GridList" BoxFlex="1" ShowBorder="false" ShowHeader="false" EnableMultiSelect="true" CheckBoxSelectOnly="true"
					AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" DataKeyNames="DEPTID,GDSEQ"
					EnableColumnLines="true" EnableHeaderMenu="true" SortField="DEPTID" EnableCheckBoxSelect="true"
					AllowPaging="true" IsDatabasePaging="true" PageSize="100" OnPageIndexChange="GridList_PageIndexChange"		>
					<Columns>
						<f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
						<f:BoundField DataField="DEPTID" Hidden="true" SortField="DEPTID" />
						<f:BoundField Width="120px" DataField="DEPTNAME" SortField="DEPTNAME" HeaderText="科室" />
						<f:BoundField Width="90px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
						<f:BoundField Width="150px" DataField="GDNAME" SortField="GDNAME" HeaderText="商品名称" />
						<f:BoundField Width="100px" DataField="GDSPEC" SortField="GDSPEC" HeaderText="商品规格" />
						<f:BoundField Width="70px" DataField="NUM1" SortField="NUM1" HeaderText="定数含量" TextAlign="Center" />	  
						<f:BoundField Width="60px" DataField="DSNUM" SortField="DSNUM" HeaderText="定数数量" TextAlign="Center" />
						<f:BoundField Width="60px" DataField="NUM2" SortField="NUM2" HeaderText="待出定数" TextAlign="Center" />
						<f:BoundField Width="70px" DataField="KCSL" SortField="KCSL" HeaderText="库存定数" TextAlign="Right" />
						<f:BoundField Width="100px" DataField="YCDS" SortField="YCDS" HeaderText="建议申领定数" TextAlign="Center" />
						<f:BoundField Width="70px" DataField="HSJJ" SortField="HSJJ" HeaderText="含税进价" TextAlign="Right" DataFormatString="{0:F2}" />
						<f:BoundField Width="90px" DataField="HSJE" SortField="HSJE" HeaderText="含税金额" TextAlign="Right" DataFormatString="{0:F2}" />
						<f:BoundField Width="70px" DataField="BYXHS" SortField="BYXHS" HeaderText="本月消耗" TextAlign="Right" />
						<f:BoundField Width="70px" DataField="ZDKC" SortField="ZDKC" HeaderText="最低库存" TextAlign="Right" />
						<f:BoundField Width="70px" DataField="ZGKC" SortField="ZGKC" HeaderText="最高库存" TextAlign="Right" />
						<f:BoundField Width="70px" DataField="SUPID" SortField="SUPID" HeaderText="供应商编码" Hidden="true" />
						<f:BoundField Width="210px" DataField="SUPNAME" SortField="SUPNAME" HeaderText="供应商" />
						<f:BoundField Width="230px" DataField="PZWH" SortField="PZWH" HeaderText="批准文号" />
						<f:BoundField Width="70px" DataField="PRODUCER" SortField="PRODUCER" HeaderText="生产厂家编码" Hidden="true" />
						<f:BoundField Width="210px" DataField="PRODUCERNAME" SortField="PRODUCERNAME" HeaderText="生产厂家" />
						<f:BoundField Width="50px" DataField="PRINTNUM" HeaderText="打印次数" Hidden="true" />
						<f:BoundField DataField="BARCODE" Hidden="true" />
						<f:BoundField DataField="UNIT" Hidden="true" />
						<f:BoundField DataField="GDMODE" Hidden="true" />
						<f:BoundField DataField="HWID" Hidden="true" />
						<f:BoundField DataField="JXTAX" Hidden="true" />
						<f:BoundField DataField="BHSJJ" Hidden="true" />
						<f:BoundField DataField="BHSJE" Hidden="true" />
						<f:BoundField DataField="LSJ" Hidden="true" />
						<f:BoundField DataField="LSJE" Hidden="true" />
						<f:BoundField DataField="ISGZ" Hidden="true" />
						<f:BoundField DataField="ISLOT" Hidden="true" />
					</Columns>
				</f:Grid>
			</Items>
		</f:Panel>
		<f:HiddenField ID="highlightRows" runat="server"></f:HiddenField>
		<f:HiddenField ID="highlightRowsGreen" runat="server"></f:HiddenField>
	</form>
</body>
</html>