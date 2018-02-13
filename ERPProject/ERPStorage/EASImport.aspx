<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EASImport.aspx.cs" Inherits="ERPProject.ERPStorage.EASImport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>EAS订单导入</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="TabStripEAS" runat="server" />
        <f:TabStrip ID="TabStripEAS" runat="server" ShowBorder="false" TabPosition="Right" EnableFrame="false" EnableTabCloseMenu="false" ActiveTabIndex="0">
            <Tabs>
                <f:Tab Title="订单导入" BodyPadding="0px" Layout="Fit" runat="server">
                    <Items>
                        <f:Grid ID="GridShelf" ShowBorder="true" ShowHeader="false" AllowSorting="false"
                            AutoScroll="true" runat="server" DataKeyNames="userid">
                            <Toolbars>
                                <f:Toolbar ID="Toolbar1" runat="server">
                                    <Items>
                                        <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                        <f:Button ID="btnSelect" CssStyle="margin-left: 15px;" Icon="Add" Text="选择文件" runat="server" OnClick="btnSelect_Click">
                                        </f:Button>
                                        <f:Button ID="btnDelete" CssStyle="margin-left: 15px;" ConfirmText="是否确认删除此单信息?" Icon="Delete" OnClick="btnDelete_Click"
                                            Text="删除" EnablePostBack="true" runat="server">
                                        </f:Button>
                                        <f:Button ID="btImport" CssStyle="margin-left: 15px;margin-right: 11px;" OnClick="btImport_Click"
                                            Icon="Disk" Text="订单导入" DisableControlBeforePostBack="false" runat="server" />
                                    </Items>
                                </f:Toolbar>
                            </Toolbars>
                            <Columns>
                                <f:BoundField Width="100px" DataField="IMPSEQ" HeaderText="导入系号" />
                                <f:BoundField Width="100px" DataField="IMPFLAG" HeaderText="处理标志" />
                                <f:BoundField Width="100px" DataField="FMATERIALID" HeaderText="商品编码" />
                                <f:BoundField Width="100px" DataField="FNAME_L2" HeaderText="商品名称" />
                                <f:BoundField Width="100px" DataField="FMODEL" HeaderText="规格型号" />
                                <f:BoundField Width="100px" DataField="FASSISTATTR" HeaderText="辅助属性" />
                                <f:BoundField Width="100px" DataField="CFCLDLID" HeaderText="产量大类" />
                                <f:BoundField Width="100px" DataField="CFCPDLID" HeaderText="产品大类" />
                                <f:BoundField Width="100px" DataField="CFXCPXMID" HeaderText="项目号" />
                                <f:BoundField Width="100px" DataField="FLOT" HeaderText="批次" />
                                <f:BoundField Width="100px" DataField="FMFG" HeaderText="生产日期" />
                                <f:BoundField Width="100px" DataField="FEXP" HeaderText="到期日期" />
                                <f:BoundField Width="100px" DataField="FUNITID" HeaderText="计量单位" />
                                <f:BoundField Width="100px" DataField="CFWGSHOULDQTY" HeaderText="应发数量" />
                                <f:BoundField Width="100px" DataField="FQTY" HeaderText="数量" />
                                <f:BoundField Width="100px" DataField="FBASEUNITID" HeaderText="基本计量单位" />
                                <f:BoundField Width="100px" DataField="EBASEQTY" HeaderText="基本数量" />
                                <f:BoundField Width="100px" DataField="FASSISTUNITID" HeaderText="辅助计量单位" />
                                <f:BoundField Width="100px" DataField="FASSISTQTY" HeaderText="辅助数量" />
                                <f:BoundField Width="100px" DataField="FWAREHOUSEID" HeaderText="仓库" />
                                <f:BoundField Width="100px" DataField="FSTOCKERID" HeaderText="仓管员" />
                                <f:BoundField Width="100px" DataField="FLOCATIONID" HeaderText="库位" />
                                <f:BoundField Width="100px" DataField="FSUPPLIERID" HeaderText="供应商" />
                                <f:BoundField Width="100px" DataField="FISPRESENT" HeaderText="是否赠品" />
                                <f:BoundField Width="100px" DataField="FUNITSTANDARDCOST" HeaderText="单位标准成本" />
                                <f:BoundField Width="100px" DataField="FSTANDARDCOST" HeaderText="标准成本" />
                                <f:BoundField Width="100px" DataField="FUNITACTUALCOST" HeaderText="单位实际成本" />
                                <f:BoundField Width="100px" DataField="FACTUALCOST" HeaderText="实际成本" />
                                <f:BoundField Width="100px" DataField="FSALEPRICE" HeaderText="单价" />
                                <f:BoundField Width="100px" DataField="FTAXRATE" HeaderText="税率" />
                                <f:BoundField Width="100px" DataField="FTAXPRICE" HeaderText="含税单价" />
                                <f:BoundField Width="100px" DataField="FDISCOUNTTYPE" HeaderText="折扣方式" />
                                <f:BoundField Width="100px" DataField="FDISCOUNTAMOUNT" HeaderText="折扣额" />
                                <f:BoundField Width="100px" DataField="FDISCOUNT" HeaderText="单位折扣(率)" />
                                <f:BoundField Width="100px" DataField="FACTUALPRICE" HeaderText="实际单价" />
                                <f:BoundField Width="100px" DataField="FPRICE" HeaderText="实际含税单价" />
                                <f:BoundField Width="100px" DataField="FNONTAXAMOUNT" HeaderText="金额" />
                                <f:BoundField Width="100px" DataField="FLOCALNONTAXAMOUNT" HeaderText="金额本位币" />
                                <f:BoundField Width="100px" DataField="FTAX" HeaderText="税额" />
                                <f:BoundField Width="100px" DataField="FLOCALTAX" HeaderText="本位币税额" />
                                <f:BoundField Width="100px" DataField="FAMOUNT" HeaderText="价税合计" />
                                <f:BoundField Width="100px" DataField="FLOCALAMOUNT" HeaderText="价税合计本位币" />
                                <f:BoundField Width="100px" DataField="FWRITTENOFFQTY" HeaderText="已核销数量" />
                                <f:BoundField Width="100px" DataField="FWRITTENOFFAMOUNT" HeaderText="已核销金额" />
                                <f:BoundField Width="100px" DataField="FUNWRITEOFFAMOUNT" HeaderText="未核销金额" />
                                <f:BoundField Width="100px" DataField="FCOREBILLTYPEID" HeaderText="核心单据类型" />
                                <f:BoundField Width="100px" DataField="FSALEORDERNUMBER" HeaderText="核心单据号" />
                                <f:BoundField Width="100px" DataField="FSALEORDERENTRYSEQ" HeaderText="核心单据行行号" />
                                <f:BoundField Width="100px" DataField="FPROJECTID" HeaderText="项目号" />
                                <f:BoundField Width="100px" DataField="FTRACKNUMBER" HeaderText="跟踪号" />
                                <f:BoundField Width="100px" DataField="FCONTRACTNUMBER" HeaderText="合同号" />
                                <f:BoundField Width="100px" DataField="FSALEORGUNITID" HeaderText="销售组织" />
                                <f:BoundField Width="100px" DataField="FSALEGROUPID" HeaderText="销售组" />
                                <f:BoundField Width="100px" DataField="FSALEPERSONID" HeaderText="销售员" />
                                <f:BoundField Width="100px" DataField="FSENDADDRESS" HeaderText="送货地址" />
                                <f:BoundField Width="100px" DataField="FBALANCECUSTOMERID" HeaderText="应收客户" />
                                <f:BoundField Width="100px" DataField="FCONFIRMQTY" HeaderText="确认签收数量" />
                                <f:BoundField Width="100px" DataField="FTOTALINWAREHSQTY" HeaderText="累计入库数量" />
                                <f:BoundField Width="100px" DataField="FCONFIRMDATE" HeaderText="确认签收时间" />
                                <f:BoundField Width="100px" DataField="CFPIECENUM" HeaderText="件数" />
                                <f:BoundField Width="100px" DataField="CFLOADNUM" HeaderText="装量" />
                                <f:BoundField Width="100px" DataField="CFWGTRANSFERTYPEID" HeaderText="业务类型" />
                                <f:BoundField Width="100px" DataField="FREMARK" HeaderText="备注" />
                                <f:BoundField Width="100px" DataField="FISFULLWRITEOFF" HeaderText="是否完全核销" />
                                <f:BoundField Width="100px" DataField="CFWGORDERDATE" HeaderText="订单日期" />
                                <f:BoundField Width="100px" DataField="FSTOCKLOT" HeaderText="批次库存" />
                                <f:BoundField Width="100px" DataField="CFVERIFICATIONDATE" HeaderText="产品检验日期" />
                                <f:BoundField Width="100px" DataField="CFSCANQTY" HeaderText="扫描数量" />
                                <f:BoundField Width="100px" DataField="CFSCANPERSOM" HeaderText="扫描人" />
                                <f:BoundField Width="100px" DataField="FSOURCEBILLID" HeaderText="源单据Id" />
                                <f:BoundField Width="100px" DataField="SEQNO" HeaderText="序号" />
                                <f:BoundField Width="100px" DataField="CFALIAS" HeaderText="商品别名" />
                                <f:BoundField Width="100px" DataField="CFMANUFACTURER" HeaderText="生产厂家" />
                                <f:BoundField Width="100px" DataField="CFMIEJUNDATE" HeaderText="灭菌日期" />
                                <f:BoundField Width="100px" DataField="CFMIDLEPACK" HeaderText="中包装装量" />
                                <f:BoundField Width="100px" DataField="CFMIDLEUNITID" HeaderText="中包装单位" />
                                <f:BoundField Width="100px" DataField="CFLARGPACKQUTY" HeaderText="大包装数量" />
                                <f:BoundField Width="100px" DataField="CFMIDLEPACKQUTY" HeaderText="中包装数量" />
                                <f:BoundField Width="100px" DataField="CFLARGPACK" HeaderText="大包装装量" />
                                <f:BoundField Width="100px" DataField="FWARRANTNUMBER" HeaderText="注册证号" />
                                <f:BoundField Width="100px" DataField="CFFREEADVICE" HeaderText="运费发票" />
                                <f:BoundField Width="100px" DataField="CFFORMULATION" HeaderText="剂型" />
                                <f:BoundField Width="100px" DataField="CFLIMITEDPRICE" HeaderText="限价" />
                                <f:BoundField Width="100px" DataField="CFCLASSICID" HeaderText="套系" />
                            </Columns>
                        </f:Grid>
                    </Items>
                </f:Tab>
                <f:Tab Title="信息查询" BodyPadding="0px" Layout="Anchor" runat="server">
                    <Items>
                        <f:Form ID="FormSearch" ShowBorder="false" AutoScroll="false" ShowHeader="False" BodyPadding="5px 10px 0px 10px" LabelWidth="80px" runat="server">
                            <Rows>
                                <f:FormRow>
                                    <Items>
                                        <f:TriggerBox ID="trbSearch" runat="server" Label="查询信息" TriggerIcon="Search" OnTriggerClick="trbSearch_TriggerClick" />
                                    </Items>
                                </f:FormRow>
                            </Rows>
                        </f:Form>
                        <f:Grid ID="Grid1" AnchorValue="100% -45" ShowBorder="true" ShowHeader="false" AllowSorting="false"
                            AutoScroll="true" runat="server" DataKeyNames="userid">
                            <Columns>
                                <f:BoundField Width="100px" DataField="IMPSEQ" HeaderText="导入系号" />
                                <f:BoundField Width="100px" DataField="IMPFLAG" HeaderText="处理标志" />
                                <f:BoundField Width="100px" DataField="FMATERIALID" HeaderText="商品编码" />
                                <f:BoundField Width="100px" DataField="FNAME_L2" HeaderText="商品名称" />
                                <f:BoundField Width="100px" DataField="FMODEL" HeaderText="规格型号" />
                                <f:BoundField Width="100px" DataField="FASSISTATTR" HeaderText="辅助属性" />
                                <f:BoundField Width="100px" DataField="CFCLDLID" HeaderText="产量大类" />
                                <f:BoundField Width="100px" DataField="CFCPDLID" HeaderText="产品大类" />
                                <f:BoundField Width="100px" DataField="CFXCPXMID" HeaderText="项目号" />
                                <f:BoundField Width="100px" DataField="FLOT" HeaderText="批次" />
                                <f:BoundField Width="100px" DataField="FMFG" HeaderText="生产日期" />
                                <f:BoundField Width="100px" DataField="FEXP" HeaderText="到期日期" />
                                <f:BoundField Width="100px" DataField="FUNITID" HeaderText="计量单位" />
                                <f:BoundField Width="100px" DataField="CFWGSHOULDQTY" HeaderText="应发数量" />
                                <f:BoundField Width="100px" DataField="FQTY" HeaderText="数量" />
                                <f:BoundField Width="100px" DataField="FBASEUNITID" HeaderText="基本计量单位" />
                                <f:BoundField Width="100px" DataField="EBASEQTY" HeaderText="基本数量" />
                                <f:BoundField Width="100px" DataField="FASSISTUNITID" HeaderText="辅助计量单位" />
                                <f:BoundField Width="100px" DataField="FASSISTQTY" HeaderText="辅助数量" />
                                <f:BoundField Width="100px" DataField="FWAREHOUSEID" HeaderText="仓库" />
                                <f:BoundField Width="100px" DataField="FSTOCKERID" HeaderText="仓管员" />
                                <f:BoundField Width="100px" DataField="FLOCATIONID" HeaderText="库位" />
                                <f:BoundField Width="100px" DataField="FSUPPLIERID" HeaderText="供应商" />
                                <f:BoundField Width="100px" DataField="FISPRESENT" HeaderText="是否赠品" />
                                <f:BoundField Width="100px" DataField="FUNITSTANDARDCOST" HeaderText="单位标准成本" />
                                <f:BoundField Width="100px" DataField="FSTANDARDCOST" HeaderText="标准成本" />
                                <f:BoundField Width="100px" DataField="FUNITACTUALCOST" HeaderText="单位实际成本" />
                                <f:BoundField Width="100px" DataField="FACTUALCOST" HeaderText="实际成本" />
                                <f:BoundField Width="100px" DataField="FSALEPRICE" HeaderText="单价" />
                                <f:BoundField Width="100px" DataField="FTAXRATE" HeaderText="税率" />
                                <f:BoundField Width="100px" DataField="FTAXPRICE" HeaderText="含税单价" />
                                <f:BoundField Width="100px" DataField="FDISCOUNTTYPE" HeaderText="折扣方式" />
                                <f:BoundField Width="100px" DataField="FDISCOUNTAMOUNT" HeaderText="折扣额" />
                                <f:BoundField Width="100px" DataField="FDISCOUNT" HeaderText="单位折扣(率)" />
                                <f:BoundField Width="100px" DataField="FACTUALPRICE" HeaderText="实际单价" />
                                <f:BoundField Width="100px" DataField="FPRICE" HeaderText="实际含税单价" />
                                <f:BoundField Width="100px" DataField="FNONTAXAMOUNT" HeaderText="金额" />
                                <f:BoundField Width="100px" DataField="FLOCALNONTAXAMOUNT" HeaderText="金额本位币" />
                                <f:BoundField Width="100px" DataField="FTAX" HeaderText="税额" />
                                <f:BoundField Width="100px" DataField="FLOCALTAX" HeaderText="本位币税额" />
                                <f:BoundField Width="100px" DataField="FAMOUNT" HeaderText="价税合计" />
                                <f:BoundField Width="100px" DataField="FLOCALAMOUNT" HeaderText="价税合计本位币" />
                                <f:BoundField Width="100px" DataField="FWRITTENOFFQTY" HeaderText="已核销数量" />
                                <f:BoundField Width="100px" DataField="FWRITTENOFFAMOUNT" HeaderText="已核销金额" />
                                <f:BoundField Width="100px" DataField="FUNWRITEOFFAMOUNT" HeaderText="未核销金额" />
                                <f:BoundField Width="100px" DataField="FCOREBILLTYPEID" HeaderText="核心单据类型" />
                                <f:BoundField Width="100px" DataField="FSALEORDERNUMBER" HeaderText="核心单据号" />
                                <f:BoundField Width="100px" DataField="FSALEORDERENTRYSEQ" HeaderText="核心单据行行号" />
                                <f:BoundField Width="100px" DataField="FPROJECTID" HeaderText="项目号" />
                                <f:BoundField Width="100px" DataField="FTRACKNUMBER" HeaderText="跟踪号" />
                                <f:BoundField Width="100px" DataField="FCONTRACTNUMBER" HeaderText="合同号" />
                                <f:BoundField Width="100px" DataField="FSALEORGUNITID" HeaderText="销售组织" />
                                <f:BoundField Width="100px" DataField="FSALEGROUPID" HeaderText="销售组" />
                                <f:BoundField Width="100px" DataField="FSALEPERSONID" HeaderText="销售员" />
                                <f:BoundField Width="100px" DataField="FSENDADDRESS" HeaderText="送货地址" />
                                <f:BoundField Width="100px" DataField="FBALANCECUSTOMERID" HeaderText="应收客户" />
                                <f:BoundField Width="100px" DataField="FCONFIRMQTY" HeaderText="确认签收数量" />
                                <f:BoundField Width="100px" DataField="FTOTALINWAREHSQTY" HeaderText="累计入库数量" />
                                <f:BoundField Width="100px" DataField="FCONFIRMDATE" HeaderText="确认签收时间" />
                                <f:BoundField Width="100px" DataField="CFPIECENUM" HeaderText="件数" />
                                <f:BoundField Width="100px" DataField="CFLOADNUM" HeaderText="装量" />
                                <f:BoundField Width="100px" DataField="CFWGTRANSFERTYPEID" HeaderText="业务类型" />
                                <f:BoundField Width="100px" DataField="FREMARK" HeaderText="备注" />
                                <f:BoundField Width="100px" DataField="FISFULLWRITEOFF" HeaderText="是否完全核销" />
                                <f:BoundField Width="100px" DataField="CFWGORDERDATE" HeaderText="订单日期" />
                                <f:BoundField Width="100px" DataField="FSTOCKLOT" HeaderText="批次库存" />
                                <f:BoundField Width="100px" DataField="CFVERIFICATIONDATE" HeaderText="产品检验日期" />
                                <f:BoundField Width="100px" DataField="CFSCANQTY" HeaderText="扫描数量" />
                                <f:BoundField Width="100px" DataField="CFSCANPERSOM" HeaderText="扫描人" />
                                <f:BoundField Width="100px" DataField="FSOURCEBILLID" HeaderText="源单据Id" />
                                <f:BoundField Width="100px" DataField="SEQNO" HeaderText="序号" />
                                <f:BoundField Width="100px" DataField="CFALIAS" HeaderText="商品别名" />
                                <f:BoundField Width="100px" DataField="CFMANUFACTURER" HeaderText="生产厂家" />
                                <f:BoundField Width="100px" DataField="CFMIEJUNDATE" HeaderText="灭菌日期" />
                                <f:BoundField Width="100px" DataField="CFMIDLEPACK" HeaderText="中包装装量" />
                                <f:BoundField Width="100px" DataField="CFMIDLEUNITID" HeaderText="中包装单位" />
                                <f:BoundField Width="100px" DataField="CFLARGPACKQUTY" HeaderText="大包装数量" />
                                <f:BoundField Width="100px" DataField="CFMIDLEPACKQUTY" HeaderText="中包装数量" />
                                <f:BoundField Width="100px" DataField="CFLARGPACK" HeaderText="大包装装量" />
                                <f:BoundField Width="100px" DataField="FWARRANTNUMBER" HeaderText="注册证号" />
                                <f:BoundField Width="100px" DataField="CFFREEADVICE" HeaderText="运费发票" />
                                <f:BoundField Width="100px" DataField="CFFORMULATION" HeaderText="剂型" />
                                <f:BoundField Width="100px" DataField="CFLIMITEDPRICE" HeaderText="限价" />
                                <f:BoundField Width="100px" DataField="CFCLASSICID" HeaderText="套系" />
                            </Columns>
                        </f:Grid>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
    </form>
</body>
</html>
