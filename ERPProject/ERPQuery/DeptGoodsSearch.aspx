﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeptGoodsSearch.aspx.cs" Inherits="ERPProject.ERPQuery.DeptGoodsSearch" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>科室进销存查询</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="TabStrip1" runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="0" runat="server">
            <Tabs>
                <f:Tab Title="汇总信息" Icon="PageWord" Layout="VBox" runat="server">
                    <Items>
                        <f:Form ID="Form2" ShowBorder="false" AutoScroll="false" BodyPadding="5px"
                            ShowHeader="False" LabelWidth="70px" runat="server">
                            <Toolbars>
                                <f:Toolbar ID="Toolbar2" runat="server">
                                    <Items>
                                        <f:ToolbarText ID="ToolbarText1" runat="server" Text="操作信息："></f:ToolbarText>
                                        <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                        <f:Button ID="btnClear" Icon="ArrowRotateClockwise" Text="清 空" OnClick="btClear_Click" EnablePostBack="true" runat="server" EnableDefaultState="false" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btnExport" runat="server" CssStyle="margin-left: 10px;" Icon="DatabaseGo" EnableAjax="false" EnableDefaultState="false"
                                            OnClick="btnExport_Click" EnablePostBack="true" Text="导出" ConfirmText="是否确认导出此进销存信息?" DisableControlBeforePostBack="false" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btnQuery" Icon="Magnifier" Text="查 询" OnClick="btnQuery_Click" runat="server" EnableDefaultState="false" />
                                    </Items>
                                </f:Toolbar>
                            </Toolbars>
                            <Rows>
                                <f:FormRow ColumnWidths="50% 25% 25%">
                                    <Items>
                                        <f:DropDownList ID="lstDEPTID" runat="server" Label="科 室" EnableEdit="true" ForceSelection="true" />
                                        <f:DatePicker ID="lstDATE1" runat="server" Label="查询日期" Required="true"></f:DatePicker>
                                        <f:DatePicker ID="lstDATE2" runat="server" Required="true" Label="　至" LabelSeparator=""></f:DatePicker>
                                    </Items>
                                </f:FormRow>
                            </Rows>
                        </f:Form>
                        <f:Grid ID="GridSum" BoxFlex="1" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;" EnableTextSelection="true"
                            AllowSorting="true" AutoScroll="true" runat="server" DataKeyNames="DEPTID" IsDatabasePaging="true" AllowPaging="true" SummaryPosition="Bottom" EnableSummary="true"
                            PageSize="100" OnPageIndexChange="GridGoods_PageIndexChange" SortField="DEPTNAME" SortDirection="ASC" EnableColumnLines="true" EnableRowDoubleClickEvent="true"
                            OnRowDoubleClick="GridSum_RowDoubleClick">
                            <Columns>
                                <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" EnableLock="true" Locked="true" />
                                <f:BoundField Hidden="true" DataField="DEPTID" SortField="DEPTID" HeaderText="科室编码" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                                <f:BoundField Width="200px" DataField="DEPTNAME" SortField="DEPTNAME" ColumnID="DEPTNAME" HeaderText="科室名称" ExpandUnusedSpace="true" MinWidth="150px" />
                                <f:GroupField HeaderText="期初库存金额" TextAlign="Center">
                                    <Columns>
                                        <f:BoundField Width="110px" DataField="QCKCJFJE" SortField="QCKCJFJE" ColumnID="QCKCJFJE" HeaderText="计费类耗材" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:f2}" />
                                        <f:BoundField Width="110px" DataField="QCKCFJFJE" SortField="QCKCFJFJE" ColumnID="QCKCFJFJE" HeaderText="非计费类耗材" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:f2}" />
                                    </Columns>
                                </f:GroupField>
                                <f:GroupField HeaderText="科室收货金额" TextAlign="Center">
                                    <Columns>
                                        <f:BoundField Width="110px" DataField="JFSHJE" SortField="JFSHJE" ColumnID="JFSHJE" HeaderText="计费类耗材" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:f2}" />
                                        <f:BoundField Width="110px" DataField="FJFSHJE" SortField="FJFSHJE" ColumnID="FJFSHJE" HeaderText="非计费类耗材" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:f2}" />
                                    </Columns>
                                </f:GroupField>
                                <f:GroupField HeaderText="科室消耗金额" TextAlign="Center">
                                    <Columns>
                                        <f:BoundField Width="110px" DataField="JFXHJE" SortField="JFXHJE" ColumnID="JFXHJE" HeaderText="计费类耗材" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:f2}" />
                                        <f:BoundField Width="110px" DataField="FJFXHJE" SortField="FJFXHJE" ColumnID="FJFXHJE" HeaderText="非计费类耗材" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:f2}" />
                                    </Columns>
                                </f:GroupField>
                                <f:GroupField HeaderText="期末库存金额" TextAlign="Center">
                                    <Columns>
                                        <f:BoundField Width="110px" DataField="QMKCJFJE" SortField="QMKCJFJE" ColumnID="QMKCJFJE" HeaderText="计费类耗材" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:f2}" />
                                        <f:BoundField Width="110px" DataField="QMKCFJFJE" SortField="QMKCFJFJE" ColumnID="QMKCFJFJE" HeaderText="非计费类耗材" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:f2}" />
                                    </Columns>
                                </f:GroupField>
                            </Columns>
                        </f:Grid>
                    </Items>
                </f:Tab>
                <f:Tab Title="明细信息" Icon="Table" Layout="VBox" runat="server">
                    <Items>
                        <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="5px"
                            ShowHeader="False" LabelWidth="70px" runat="server">
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
                            <Rows>
                                <f:FormRow>
                                    <Items>
                                        <f:TextBox ID="txbGDSEQ" runat="server" Label="商品信息" EmptyText="输入商品编码查询" />
                                        <f:DropDownList ID="ddlDEPTID" runat="server" Label="部门" EnableEdit="true" ForceSelection="true" />
                                        <f:DropDownList ID="ddlSUPID" runat="server" Label="配送商" EnableEdit="true" ForceSelection="true" />
                                        <f:DatePicker ID="dpkDATE1" runat="server" Label="查询日期" Required="true"></f:DatePicker>
                                        <f:DatePicker ID="dpkDATE2" ShowRedStar="true" Required="true" runat="server" Label="　至" LabelSeparator=""></f:DatePicker>
                                    </Items>
                                </f:FormRow>
                            </Rows>
                        </f:Form>
                        <f:Grid ID="GridGoods" BoxFlex="1" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;" AllowColumnLocking="true"
                            AllowSorting="true" AutoScroll="true" runat="server" DataKeyNames="GDSEQ" IsDatabasePaging="true" AllowPaging="true" SummaryPosition="Bottom" EnableSummary="true"
                            PageSize="100" OnPageIndexChange="GridGoods_PageIndexChange" OnSort="GridGoods_Sort" SortField="GDSEQ" SortDirection="ASC" EnableColumnLines="true" EnableTextSelection="true">
                            <Columns>
                                <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" EnableLock="true" Locked="true" />
                                <f:BoundField Width="100px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="true" />
                                <f:BoundField Width="150px" DataField="GDNAME" SortField="GDNAME" ColumnID="GDNAME" HeaderText="商品名称" EnableLock="true" Locked="true" />
                                <f:BoundField Width="115px" DataField="HISCODE" SortField="HISCODE" HeaderText="HIS编码" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="true" Hidden="true" />
                                <f:BoundField Width="115px" DataField="EASCODE" SortField="EASCODE" HeaderText="EAS编码" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" EnableLock="true" Locked="true" Hidden="true" />
                                <f:BoundField Width="130px" DataField="DEPTID" SortField="DEPTID" HeaderText="部门" EnableLock="true" Locked="true" />
                                <f:BoundField Width="170px" DataField="SUPPLIERNAME" SortField="SUPPLIERNAME" ColumnID="SUPPLIERNAME" HeaderText="配送商" EnableLock="true" Locked="true" />
                                <f:BoundField Width="60px" DataField="HSJJ" SortField="HSJJ" HeaderText="价格" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                                <f:BoundField Width="70px" DataField="GDSPEC" SortField="GDSPEC" HeaderText="规格·容量" EnableColumnHide="true" EnableHeaderMenu="false" />
                                <f:BoundField Width="50px" DataField="UNIT" HeaderText="单位" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false"></f:BoundField>
                                <f:BoundField Width="70px" DataField="QCKCSL" SortField="QCKCSL" ColumnID="QCKCSL" HeaderText="期初数量" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                                <f:BoundField Width="70px" DataField="QCKCHSJE" SortField="QCKCHSJE" ColumnID="QCKCHSJE" HeaderText="期初金额" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:F2}" />

                                <f:GroupField HeaderText="数量" TextAlign="Center">
                                    <Columns>

                                        <f:GroupField HeaderText="非定数" TextAlign="Center">
                                            <Columns>
                                                <f:BoundField Width="70px" DataField="FDSCRK" SortField="FDSCRK" ColumnID="FDSCRK" HeaderText="入库" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                                                <f:BoundField Width="70px" DataField="XSDCK" SortField="XSDCK" ColumnID="XSDCK" HeaderText="出库" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                                                <f:BoundField Width="70px" DataField="FDSTCK" SortField="FDSTCK" ColumnID="FDSTCK" HeaderText="退货" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                                            </Columns>
                                        </f:GroupField>

                                        <f:GroupField HeaderText="定数" TextAlign="Center">
                                            <Columns>
                                                <f:BoundField Width="70px" DataField="DSCRK" SortField="DSCRK" ColumnID="DSCRK" HeaderText="入库" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                                                <f:BoundField Width="70px" DataField="DSHCK" SortField="DSHCK" ColumnID="DSHCK" HeaderText="出库" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                                                <f:BoundField Width="70px" DataField="DSTCK" SortField="DSTCK" ColumnID="DSTCK" HeaderText="退货" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                                            </Columns>
                                        </f:GroupField>

                                        <f:GroupField HeaderText="调拨" TextAlign="Center">
                                            <Columns>
                                                <f:BoundField Width="70px" DataField="DBRK" SortField="DBRK" ColumnID="DBRK" HeaderText="入库数" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                                                <f:BoundField Width="70px" DataField="DBCK" SortField="DBCK" ColumnID="DBCK" HeaderText="出库数" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                                            </Columns>
                                        </f:GroupField>
                                        <f:BoundField Width="70px" DataField="PYSL" SortField="PYSL" ColumnID="PYSL" HeaderText="损益数" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                                    </Columns>
                                </f:GroupField>

                                <f:GroupField HeaderText="金额" TextAlign="Center">
                                    <Columns>

                                        <f:GroupField HeaderText="非定数" TextAlign="Center">
                                            <Columns>
                                                <f:BoundField Width="70px" DataField="FDSCRKHSJE" SortField="FDSCRKHSJE" ColumnID="FDSCRKHSJE" HeaderText="入库" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:F2}" />
                                                <f:BoundField Width="70px" DataField="XSDCKHSJE" SortField="XSDCKHSJE" ColumnID="XSDCKHSJE" HeaderText="出库" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:F2}" />
                                                <f:BoundField Width="70px" DataField="FDSTCKHSJE" SortField="FDSTCKHSJE" ColumnID="FDSTCKHSJE" HeaderText="退货" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:F2}" />
                                            </Columns>
                                        </f:GroupField>

                                        <f:GroupField HeaderText="定数" TextAlign="Center">
                                            <Columns>
                                                <f:BoundField Width="70px" DataField="DSCRKHSJE" SortField="DSCRKHSJE" ColumnID="DSCRKHSJE" HeaderText="入库" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:F2}" />
                                                <f:BoundField Width="70px" DataField="DSHCKHSJE" SortField="DSHCKHSJE" ColumnID="DSHCKHSJE" HeaderText="出库" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:F2}" />
                                                <f:BoundField Width="70px" DataField="DSTCKHSJE" SortField="DSTCKHSJE" ColumnID="DSTCKHSJE" HeaderText="退货" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:F2}" />
                                            </Columns>
                                        </f:GroupField>

                                        <f:GroupField HeaderText="调拨" TextAlign="Center">
                                            <Columns>
                                                <f:BoundField Width="70px" DataField="DBRKHSJE" SortField="DBRKHSJE" ColumnID="DBRKHSJE" HeaderText="入库" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:F2}" />
                                                <f:BoundField Width="70px" DataField="DBCKHSJE" SortField="DBCKHSJE" ColumnID="DBCKHSJE" HeaderText="出库" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:F2}" />
                                            </Columns>
                                        </f:GroupField>
                                        <f:BoundField Width="70px" DataField="PYHSJE" SortField="PYHSJE" ColumnID="PYHSJE" HeaderText="损益金额" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:F2}" />
                                        <f:BoundField Width="70px" DataField="TJJE" SortField="TJJE" ColumnID="TJJE" HeaderText="调价金额" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:F2}" />
                                    </Columns>
                                </f:GroupField>

                                <%--<f:GroupField HeaderText="合计" TextAlign="Center">
                          <Columns>
                       <f:BoundField Width="70px" DataField="RKHJ" SortField="RKHJ" ColumnID="RKHJ" HeaderText="入库数" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />                  
                       <f:BoundField Width="70px" DataField="RKHJHSJE" SortField="RKHJHSJE" ColumnID="RKHJHSJE" HeaderText="入库金额" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:F2}"/>
                       <f:BoundField Width="70px" DataField="CKHJ" SortField="CKHJ" ColumnID="CKHJ" HeaderText="出库数" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />                  
                       <f:BoundField Width="70px" DataField="CKHJHSJE" SortField="CKHJHSJE" ColumnID="CKHJHSJE" HeaderText="出库金额" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:F2}"/>
                       <f:BoundField Width="60px" DataField="THCK" SortField="THCK" ColumnID="THCK" HeaderText="退货数" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />                  
                       <f:BoundField Width="70px" DataField="THCKHSJE" SortField="THCKHSJE" ColumnID="THCKHSJE" HeaderText="退货金额" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:F2}"/>
                       <f:BoundField Width="60px" DataField="OTHERSL" SortField="OTHERSL" ColumnID="OTHERSL" HeaderText="其他" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />                  
                       <f:BoundField Width="70px" DataField="OTHERHSJE" SortField="OTHERHSJE" ColumnID="OTHERHSJE" HeaderText="其他金额" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:F2}"/>
                          </Columns> 
                        </f:GroupField>--%>
                                <f:BoundField Width="70px" DataField="QMKCSL" SortField="QMKCSL" ColumnID="QMKCSL" HeaderText="期末数" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                                <f:BoundField Width="70px" DataField="QMKCHSJE" SortField="QMKCHSJE" ColumnID="QMKCHSJE" HeaderText="期末金额" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:F2}" />
                                <f:BoundField Width="70px" DataField="KCSL" SortField="KCSL" ColumnID="KCSL" HeaderText="库存数" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                                <f:BoundField Width="70px" DataField="KCHSJE" SortField="KCHSJE" ColumnID="KCHSJE" HeaderText="库存金额" TextAlign="Right" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:F2}" />
                            </Columns>
                        </f:Grid>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
    </form>
</body>
</html>
