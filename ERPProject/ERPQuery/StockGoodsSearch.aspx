<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StockGoodsSearch.aspx.cs" Inherits="ERPProject.ERPQuery.StockGoodsSearch" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>库存进销存明细查询</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="False" BodyPadding="0px" Layout="VBox"
            ShowHeader="false">
            <Toolbars>
                <f:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                        <f:ToolbarText ID="ToolbarText" runat="server" Text="操作信息：查询商品的进销存信息！"></f:ToolbarText>

                        <%--<f:CheckBox ID="chkISDG" runat="server" Text="包含代管商品" CssStyle="margin-left:10px;"></f:CheckBox>--%>
                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                        <f:Button ID="btnAudit" Icon="ArrowRotateClockwise" Text="清 空" OnClick="btClear_Click" EnablePostBack="true" runat="server" EnableDefaultState="false" />
                        <f:ToolbarSeparator runat="server" />
                        <f:Button ID="btnExp" runat="server" CssStyle="margin-left: 10px;" Icon="DatabaseGo" EnableAjax="false" EnableDefaultState="false"
                            OnClick="btExport_Click" EnablePostBack="true" Text="导出" ConfirmText="是否确认导出此进销存信息?" DisableControlBeforePostBack="false" />
                        <f:ToolbarSeparator runat="server" />
                        <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" OnClick="btSearch_Click" runat="server" EnableDefaultState="false" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="5px"
                    ShowHeader="False" LabelWidth="70px" runat="server">
                    <Rows>
                        <f:FormRow>
                            <Items>
                                
                                <f:TextBox ID="txbGDSEQ" runat="server" Label="商品信息" EmptyText="输入商品编码查询" />
                                <f:DropDownList ID="ddlDEPTID" runat="server" Label="部门" EnableEdit="true" ForceSelection="true" />
                                <f:DropDownList ID="ddlSUPID" runat="server" Label="配送商" EnableEdit="true" ForceSelection="true" />
                                <f:DatePicker ID="dpkDATE1" runat="server" Label="查询日期" Required="true"></f:DatePicker>
                                <f:DatePicker ID="dpkDATE2" ShowRedStar="true" Required="true" runat="server" Label="　至" LabelSeparator=""></f:DatePicker>
                                <%-- <f:TriggerBox ID="tgbPH" runat="server" Label="批次" EmptyText="批次信息" MaxLength="20"></f:TriggerBox>--%>
                            </Items>
                        </f:FormRow> 
                    </Rows>
                </f:Form>
                <f:Grid ID="GridGoods" BoxFlex="1" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;" AllowColumnLocking="true"
                    AllowSorting="true" AutoScroll="true" runat="server" DataKeyNames="GDSEQ" IsDatabasePaging="true" AllowPaging="true" SummaryPosition="Bottom" EnableSummary="true"
                    PageSize="100" OnPageIndexChange="GridGoods_PageIndexChange" OnSort="GridGoods_Sort" SortField="GDSEQ" SortDirection="ASC" EnableColumnLines="true" EnableTextSelection="true">
                    <Columns>
                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" EnableLock="true" Locked="true" />     
                        <f:BoundField Width="115px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="true" />
                        <f:BoundField Width="150px" DataField="GDNAME" SortField="GDNAME" ColumnID="GDNAME" HeaderText="商品名称" EnableLock="true" Locked="true" />
                        <f:BoundField Width="115px" DataField="HISCODE" SortField="HISCODE" HeaderText="HIS编码" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="true" Hidden="true"/>
                        <f:BoundField Width="115px" DataField="EASCODE" SortField="EASCODE" HeaderText="EAS编码" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="true" Hidden="true"/>
                        <f:BoundField Width="100px" DataField="DEPTID" SortField="DEPTID" HeaderText="部门" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false"  EnableLock="true" Locked="true" />
                        <f:BoundField Width="170px" DataField="SUPPLIERNAME" SortField="SUPPLIERNAME" ColumnID="SUPPLIERNAME" HeaderText="配送商"  EnableLock="true" Locked="true"/>
                        <f:BoundField Width="60px" DataField="HSJJ" SortField="HSJJ" HeaderText="价格" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false"  />  
                        <f:BoundField Width="70px" DataField="GDSPEC" SortField="GDSPEC" HeaderText="规格·容量" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="50px" DataField="UNIT" HeaderText="单位" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false"></f:BoundField>
                        <f:BoundField Width="80px" DataField="QCKCSL" SortField="QCKCSL" ColumnID="QCKCSL" HeaderText="期初库存数" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />                  
                        <f:BoundField Width="90px" DataField="QCKCHSJE" SortField="QCKCHSJE" ColumnID="QCKCHSJE" HeaderText="期初库存金额" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:F2}" /> 
                        
                         <f:GroupField HeaderText="数量" TextAlign="Center">
                          <Columns>
                             <f:BoundField Width="70px" DataField="CGRK" SortField="CGRK" ColumnID="CGRK" HeaderText="采购数" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                             <f:BoundField Width="70px" DataField="KFCK" SortField="KFCK" ColumnID="KFCK" HeaderText="出库数" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                             <f:BoundField Width="70px" DataField="THCK" SortField="THCK" ColumnID="THCK" HeaderText="采购退" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                             <f:BoundField Width="70px" DataField="KSTH" SortField="KSTH" ColumnID="KSTH" HeaderText="科室退" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />                  
                             <f:BoundField Width="70px" DataField="DBRK" SortField="DBRK" ColumnID="DBRK" HeaderText="调拨入" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                             <f:BoundField Width="70px" DataField="DBCK" SortField="DBCK" ColumnID="DBCK" HeaderText="调拨出" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" /> 
                              <f:BoundField Width="70px" DataField="PYSL" SortField="PYSL" ColumnID="PYSL" HeaderText="损益数" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />                                                      
                          </Columns> 
                        </f:GroupField>

                        <f:GroupField HeaderText="金额" TextAlign="Center">
                          <Columns>
                            <f:BoundField Width="70px" DataField="CGRKHSJE" SortField="CGRKHSJE" ColumnID="CGRKHSJE" HeaderText="采购金额" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:F2}"/>
                            <f:BoundField Width="70px" DataField="KFCKHSJE" SortField="KFCKHSJE" ColumnID="KFCKHSJE" HeaderText="出库金额" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:F2}" />
                            <f:BoundField Width="70px" DataField="THCKHSJE" SortField="THCKHSJE" ColumnID="THCKHSJE" HeaderText="采购退" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:F2}"/>
                            <f:BoundField Width="70px" DataField="KSTHHSJE" SortField="KSTHHSJE" ColumnID="KSTHHSJE" HeaderText="科室退" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:F2}"/>
                            <f:BoundField Width="70px" DataField="DBRKHSJE" SortField="DBRKHSJE" ColumnID="DBRKHSJE" HeaderText="调拨入" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:F2}"/>
                            <f:BoundField Width="70px" DataField="DBCKHSJE" SortField="DBCKHSJE" ColumnID="DBCKHSJE" HeaderText="调拨出" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:F2}"/>
                            <f:BoundField Width="70px" DataField="PYHSJE" SortField="PYHSJE" ColumnID="PYHSJE" HeaderText="损益金额" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:F2}"/>
                            <f:BoundField Width="70px" DataField="TJJE" SortField="TJJE" ColumnID="TJJE" HeaderText="调价金额" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:F2}"/>    
                          </Columns> 
                        </f:GroupField>
                       
                       <%-- <f:GroupField HeaderText="合计" TextAlign="Center">
                          <Columns>
                       <f:BoundField Width="60px" DataField="CGRK" SortField="CGRK" ColumnID="CGRK" HeaderText="入库数" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />                  
                       <f:BoundField Width="70px" DataField="CGRKHSJE" SortField="CGRKHSJE" ColumnID="CGRKHSJE" HeaderText="入库金额" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:F2}"/>
                       <f:BoundField Width="60px" DataField="KFCK" SortField="KFCK" ColumnID="KFCK" HeaderText="出库数" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />                  
                       <f:BoundField Width="70px" DataField="KFCKHSJE" SortField="KFCKHSJE" ColumnID="KFCKHSJE" HeaderText="出库金额" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:F2}"/>
                       <f:BoundField Width="60px" DataField="THCK" SortField="THCK" ColumnID="THCK" HeaderText="退货数" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />                  
                       <f:BoundField Width="70px" DataField="THCKHSJE" SortField="THCKHSJE" ColumnID="THCKHSJE" HeaderText="退货金额" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:F2}"/>
                       <f:BoundField Width="60px" DataField="OTHERSL" SortField="OTHERSL" ColumnID="OTHERSL" HeaderText="其他" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />                  
                       <f:BoundField Width="70px" DataField="OTHERHSJE" SortField="OTHERHSJE" ColumnID="OTHERHSJE" HeaderText="其他金额" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:F2}"/>
                          </Columns> 
                        </f:GroupField>--%>
                        <f:BoundField Width="70px" DataField="QMKCSL" SortField="QMKCSL" ColumnID="QMKCSL" HeaderText="期末数" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />                  
                        <f:BoundField Width="70px" DataField="QMKCHSJE" SortField="QMKCHSJE" ColumnID="QMKCHSJE" HeaderText="期末金额" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:F2}"/>
                        <f:BoundField Width="70px" DataField="KCSL" SortField="KCSL" ColumnID="KCSL" HeaderText="库存数" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />                  
                        <f:BoundField Width="70px" DataField="KCHSJE" SortField="KCHSJE" ColumnID="KCHSJE" HeaderText="库存金额" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:F2}"/>
                      
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
</body>
</html>
