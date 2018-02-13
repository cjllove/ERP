﻿using FineUIPro;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using XTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPProject.ERPReapt
{
    public partial class HighGoodsCk : BillBase
    {
        #region Load加载
        private string strDocSql = "SELECT * FROM DAT_CK_DOC WHERE SEQNO ='{0}' AND BILLTYPE = 'CKD' AND XSTYPE = 'G'";
        private string strComSql = @"SELECT A.*,
                                       F_GETUNITNAME(A.UNIT) UNITNAME,
                                       F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                                       F_GETUNITNAME(B.UNIT) UNITSMALLNAME
                                          FROM DAT_CK_COM A, DOC_GOODS B
                                         WHERE SEQNO = '{0}'
                                           AND A.GDSEQ = B.GDSEQ
                                         ORDER BY ROWNO";
        int CloseWindow = 0;
        public override Field[] LockControl
        {
            get { return new Field[] { docBILLNO, docDEPTID, docXSRQ }; }
        }

        public HighGoodsCk()
        {
            BillType = "CKD";
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
                DataInit();
                billNew();
                //对于不同界面展示不同信息
                if (Request.QueryString["oper"] != null)
                {
                    //审核过程
                    TabStrip1.ActiveTabIndex = 0;
                    btnScan.Hidden = false;
                    btnCancel.Hidden = false;
                    btnNew.Hidden = true;
                    btnDel.Hidden = true;
                    btnSave.Hidden = true;
                    btnAudit.Hidden = false;
                    btnDelRow.Hidden = true;
                    btnSubmit.Hidden = true;
                    docGZM.Enabled = false;
                }
                #region 读取客户标记，更改客户化需求
                //string USERXMID = "";
                //USERXMID = (DbHelperOra.GetSingle("select VALUE from sys_para WHERE CODE='USERXMID'")).ToString();
                //if (USERXMID == "TJ_YKGZ") TJ_YKGZ();
                #endregion
            }
        }
        private void DataInit()
        {
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
            PubFunc.DdlDataGet("DDL_USER", docSHR, docLRY, docSLR);
            DepartmentBind.BindDDL("DDL_SYS_DEPTRANGE", UserAction.UserID, docDEPTID, lstDEPTID);
            DepartmentBind.BindDDL("DDL_SYS_DEPOTRANGE", UserAction.UserID, docDEPTOUT, ddlDEPTOUT);
            PubFunc.DdlDataGet("DDL_BILL_StoreOut", docFLAG);
            docDEPTOUT.Items.RemoveAt(0);
        }
        protected void GridList_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                string flag = row["FLAG"].ToString();
                FineUIPro.BoundField flagcol = GridList.FindColumn("FLAGNAME") as FineUIPro.BoundField;
                if (flag == "M")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color1";
                }
                if (flag == "N")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color2";
                }
                if (flag == "R")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color3";
                }
            }
        }
        #endregion
        #region 增删改
        protected override void billNew()
        {
            //原单据保存判断
            string strDeptID = docDEPTID.SelectedValue;
            string strDeptOUT = docDEPTOUT.SelectedValue;
            PubFunc.FormDataClear(FormDoc);

            docFLAG.SelectedValue = "M";
            docLRY.SelectedValue = UserAction.UserID;
            docSLR.SelectedValue = UserAction.UserID;
            docLRRQ.SelectedDate = DateTime.Now;
            docXSRQ.SelectedDate = DateTime.Now;
            docDEPTID.SelectedValue = strDeptID;
            docDEPTOUT.SelectedValue = strDeptOUT;
            billLockDoc(false);
            docMEMO.Enabled = true;
            docDEPTOUT.Enabled = true;
            docSLR.Enabled = true;
            //清空Grid行
            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            btnDel.Enabled = false;
            btnSave.Enabled = true;
            btnAudit.Enabled = false;
            btnCancel.Enabled = false;
            btnPrt.Enabled = false;
            btnPrint.Enabled = false;
            btnDelRow.Enabled = true;
            btnGoods.Enabled = true;
            docGZM.Enabled = true;

            //计算合计数量
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("BZSL", "0");
            summary.Add("HSJE", "0");
            summary.Add("DHSL", "0");
            GridGoods.SummaryData = summary;
        }
        protected override void billClear()
        {
            PubFunc.FormDataClear(Formlist);
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
        }
        protected override void billAddRow()
        {
            if ((",M,R").IndexOf(docFLAG.SelectedValue) < 0)
            {
                Alert.Show("非『新增』单据不能增行！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
            billLockDoc(true);
            PubFunc.GridRowAdd(GridGoods, "INIT");
        }

        protected override void billDelRow()
        {
            if ((",M,R").IndexOf(docFLAG.SelectedValue) < 0)
            {
                Alert.Show("非『新增』单据不能删行！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (GridGoods.SelectedCell == null)
            {
                Alert.Show("空单据不能删行！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            else
            {
                GridGoods.DeleteSelectedRows();
            }

        }
        protected override void billSave()
        {
            #region 数据有效性验证
            if ((",M,R").IndexOf(docFLAG.SelectedValue) < 0)
            {
                Alert.Show("非新单不能保存！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
            if (newDict.Count == 0)
            {
                Alert.Show("请输入商品信息", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (PubFunc.FormDataCheck(FormDoc).Length > 0) return;
            List<Dictionary<string, object>> goodsData = new List<Dictionary<string, object>>();
            //判断是否有空行、批号填写是否符合要求
            for (int i = 0; i < newDict.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(newDict[i]["GDSEQ"].ToString()) && !string.IsNullOrWhiteSpace(newDict[i]["GDNAME"].ToString()))
                {
                    if ((newDict[i]["BZSL"] ?? "").ToString() == "" || (newDict[i]["BZSL"] ?? "").ToString() == "0")
                    {
                        Alert.Show("请填写商品[" + newDict[i]["GDSEQ"] + "]出库数！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(newDict[i]["BZHL"].ToString()) || string.IsNullOrWhiteSpace(newDict[i]["UNIT"].ToString()))
                    {
                        Alert.Show("商品[" + newDict[i]["GDSEQ"] + "]包装单位信息错误，请联系管理员维护！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(newDict[i]["HSJJ"].ToString()))
                    {
                        Alert.Show("商品【" + newDict[i]["GDNAME"].ToString() + "】含税进价不能为空！！！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(newDict[i]["UNIT"].ToString()))
                    {
                        Alert.Show("商品【" + newDict[i]["GDNAME"].ToString() + "】出库包装未维护！！！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    goodsData.Add(newDict[i]);
                }
            }

            if (goodsData.Count == 0)//所有Gird行都为空行时
            {
                Alert.Show("商品信息不能为空", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            #endregion

            if (PubFunc.StrIsEmpty(docBILLNO.Text))
            {
                docSEQNO.Text = BillSeqGet();
                docBILLNO.Text = docSEQNO.Text;
                docBILLNO.Enabled = false;
            }
            else
            {
                string flg = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'N') FROM DAT_CK_DOC WHERE SEQNO='{0}'", docBILLNO.Text));
                if (!string.IsNullOrWhiteSpace(flg) && (",M,R").IndexOf(flg) < 0)
                {
                    Alert.Show("您输入的单据号存在重复信息，请重新输入或置空！", "消息提示", MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    docSEQNO.Text = docBILLNO.Text;
                    docBILLNO.Enabled = false;
                }
            }
            MyTable mtType = new MyTable("DAT_CK_DOC");
            mtType.ColRow = PubFunc.FormDataHT(FormDoc);
            mtType.ColRow["SEQNO"] = docBILLNO.Text;
            mtType.ColRow.Add("BILLTYPE", BillType);
            mtType.ColRow.Add("SUBNUM", goodsData.Count);
            mtType.ColRow.Add("XSTYPE", "G");
            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtTypeMx = new MyTable("DAT_CK_COM");
            decimal subNum = 0;//总金额
            //先删除单据信息在插入
            cmdList.Add(new CommandInfo("delete DAT_CK_DOC where seqno='" + docBILLNO.Text + "' and flag = 'M'", null));//删除单据台头
            cmdList.Add(new CommandInfo("delete DAT_CK_COM where seqno='" + docBILLNO.Text + "'", null));//删除单据明细
            for (int i = 0; i < goodsData.Count; i++)
            {
                mtTypeMx.ColRow = PubFunc.GridDataGet(goodsData[i]);

                //判断含税进价，为0时不能保存
                if (PubFunc.StrIsEmpty(mtTypeMx.ColRow["HSJJ"].ToString()) || mtTypeMx.ColRow["HSJJ"].ToString() == "0")
                {
                    Alert.Show("商品【含税进价】为0或空，无法进行【库房出库管理】操作。");
                    return;
                }
                mtTypeMx.ColRow.Add("SEQNO", docBILLNO.Text);
                mtTypeMx.ColRow["ROWNO"] = i + 1;
                mtTypeMx.ColRow.Add("PHID", mtTypeMx.ColRow["PH"]);
                mtTypeMx.ColRow["DHSL"] = decimal.Parse(mtTypeMx.ColRow["BZHL"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                mtTypeMx.ColRow["HSJE"] = decimal.Parse(mtTypeMx.ColRow["HSJJ"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                subNum = subNum + decimal.Parse(mtTypeMx.ColRow["HSJE"].ToString());
                mtTypeMx.ColRow.Add("XSSL", mtTypeMx.ColRow["DHSL"].ToString());
                mtTypeMx.ColRow.Add("BHSJJ", 0);
                mtTypeMx.ColRow.Add("BHSJE", 0);

                mtTypeMx.ColRow.Remove("UNITNAME");
                mtTypeMx.ColRow.Remove("PRODUCERNAME");
                cmdList.Add(mtTypeMx.Insert());
            }
            mtType.ColRow.Add("SUBSUM", subNum);
            //写入追溯码表
            cmdList.Add(new CommandInfo("DELETE DAT_CK_EXT WHERE BILLNO='" + docBILLNO.Text + "'", null));//删除单据明细
            cmdList.Add(new CommandInfo(String.Format(@"INSERT INTO DAT_CK_EXT(DEPTID,BILLNO,ROWNO,ONECODE,GDSEQ,GDNAME,BARCODE,UNIT,GDSPEC,DEPTCUR,BZHL,FLAG,PH,RQ_SC,YXQZ)
                        SELECT '{1}','{0}',ROWNUM,A.ONECODE,A.GDSEQ,A.GDNAME,A.BARCODE,A.UNIT,A.GDSPEC,A.DEPTCUR,A.BZHL,'N',A.PH,A.RQ_SC,A.YXQZ
                          FROM DAT_RK_EXT A,DAT_CK_COM B
                         WHERE A.GDSEQ = B.GDSEQ AND B.SEQNO = '{0}' AND (A.ONECODE = B.STR2 OR A.STR1 = B.STR2)", docBILLNO.Text, docDEPTID.SelectedValue), null));//删除单据明细
            cmdList.AddRange(mtType.InsertCommand());
            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                Alert.Show("商品出库信息保存成功！");
                billLockDoc(true);
                billOpen(docBILLNO.Text);
            }
        }
        protected override void billDel()
        {
            if (docBILLNO.Text.Trim() == "")
            {
                Alert.Show("请选择需要删除的单据");
                return;
            }
            if ((",M,R").IndexOf(docFLAG.SelectedValue) < 0)
            {
                Alert.Show("非新单不能删除!");
                return;
            }
            DbHelperOra.ExecuteSql("Delete from DAT_CK_DOC t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            DbHelperOra.ExecuteSql("Delete from DAT_CK_COM t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            DbHelperOra.ExecuteSql("Delete from DAT_CK_EXT t WHERE T.BILLNO ='" + docBILLNO.Text.Trim() + "'");
            Alert.Show("单据删除成功!");
            OperLog("直接出库", "删除单据【" + docBILLNO.Text.Trim() + "】");
            billNew();
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                return;
            }
            billSearch();
        }
        #endregion
        #region 查询
        protected void tgbBILLNO_TriggerClick(object sender, EventArgs e)
        {
            billSearch();
        }
        protected override void billSearch()
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【使用日期】！");
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                Alert.Show("开始日期大于结束日期，请重新输入！");
                return;
            }

            string strSql = @"SELECT A.SEQNO,A.BILLNO,B.NAME FLAGNAME,A.FLAG,F_GETDEPTNAME(A.DEPTID) DEPTID,A.XSRQ,F_GETDEPTNAME(A.DEPTOUT) DEPTOUT,A.SUBSUM,
                                     A.SUBNUM,F_GETUSERNAME(A.LRY) LRY,A.LRRQ,A.SHRQ,A.MEMO,F_GETUSERNAME(A.SHR) SHR
                                from DAT_CK_DOC A, SYS_CODEVALUE B 
                                WHERE A.FLAG = B.CODE AND B.TYPE  ='DIC_BILLSTATUS' AND BILLTYPE='CKD' AND XSTYPE='G' ";
            string strSearch = "";

            if (Request.QueryString["oper"] != null && Request.QueryString["oper"].ToString().ToLower() == "audit")
            {
                strSearch = strSearch + " AND A.FLAG<>'M'";
            }
            if (tgbBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND UPPER(TRIM(A.BILLNO))  LIKE '%{0}%'", tgbBILLNO.Text.Trim().ToUpper());
            }
            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedItem.Value);
            }
            if (ddlDEPTOUT.SelectedValue.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTOUT='{0}'", ddlDEPTOUT.SelectedItem.Value);
            }
            if (!PubFunc.StrIsEmpty(lstFLAG.SelectedValue))
            {
                strSearch += string.Format(" AND A.FLAG='{0}'", lstFLAG.SelectedValue);
            }
            strSearch += string.Format(" AND deptid in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            strSearch += string.Format(" AND A.XSRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.XSRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY A.BILLNO DESC";
            GridList.DataSource = DbHelperOra.Query(strSql).Tables[0];
            GridList.DataBind();
        }
        protected override void listRow_DoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            billOpen(GridList.Rows[e.RowIndex].Values[1].ToString());
        }

        protected override void billOpen(string strBillno)
        {
            DataTable dtDoc = DbHelperOra.Query(string.Format(strDocSql, strBillno)).Tables[0];
            PubFunc.FormDataSet(FormDoc, dtDoc.Rows[0]);

            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            decimal bzslTotal = 0, feeTotal = 0, dhslTotal = 0;
            DataTable dtBill = DbHelperOra.Query(string.Format(strComSql, strBillno, docDEPTOUT.SelectedValue)).Tables[0];
            Doc.GridRowAdd(GridGoods, dtBill);
            if (dtBill != null && dtBill.Rows.Count > 0)
            {
                foreach (DataRow row in dtBill.Rows)
                {
                    bzslTotal += Convert.ToDecimal(row["BZSL"]);
                    feeTotal += Convert.ToDecimal(row["HSJE"]);
                    dhslTotal += Convert.ToDecimal(row["DHSL"]);
                }
            }
            //计算合计数量
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("BZSL", bzslTotal.ToString());
            summary.Add("HSJE", feeTotal.ToString("F2"));
            summary.Add("DHSL", dhslTotal.ToString());
            GridGoods.SummaryData = summary;
            PubFunc.FormLock(FormDoc, true, "");
            if ((",M,R").IndexOf(docFLAG.SelectedValue) > 0) docMEMO.Enabled = true;
            TabStrip1.ActiveTabIndex = 1;
            //增加按钮控制
            if (docFLAG.SelectedValue == "M" || docFLAG.SelectedValue == "R")
            {
                btnDel.Enabled = true;
                btnSave.Enabled = true;
                btnAudit.Enabled = true;
                btnCancel.Enabled = false;
                btnPrt.Enabled = false;
                btnPrint.Enabled = false;
                btnDelRow.Enabled = true;
                btnGoods.Enabled = true;
                docGZM.Enabled = true;
                btnSubmit.Enabled = true;
            }
            else if (docFLAG.SelectedValue == "N")
            {
                btnDel.Enabled = false;
                btnSave.Enabled = false;
                btnAudit.Enabled = true;
                btnCancel.Enabled = true;
                btnPrt.Enabled = false;
                btnPrint.Enabled = false;
                btnDelRow.Enabled = false;
                btnGoods.Enabled = false;
                btnSubmit.Enabled = false;
                docGZM.Enabled = false;
            }
            else
            {
                btnDel.Enabled = false;
                btnSave.Enabled = false;
                btnAudit.Enabled = false;
                btnCancel.Enabled = false;
                btnPrt.Enabled = true;
                btnPrint.Enabled = true;
                btnDelRow.Enabled = false;
                btnGoods.Enabled = false;
                btnSubmit.Enabled = false;
                docGZM.Enabled = false;
            }
        }
        private void LoadGridRow(DataRow row, bool firstRow = true, string flag = "NEW")
        {
            PubFunc.GridRowAdd(GridGoods, row, firstRow);
        }
        #endregion
        #region 界面传值
        protected override void billGoods()
        {
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
            if ((",M,R").IndexOf(docFLAG.SelectedValue) < 0) return;
            PubFunc.FormLock(FormDoc, true, "");
            docMEMO.Enabled = true;
            //参数说明：cx-查询内容，bm-商品配置部门,su-供应商
            string url = "~/ERPQuery/GoodsWindow_New.aspx?Deptout=" + docDEPTOUT.SelectedValue + "&DeptIn=" + docDEPTID.SelectedValue + "&su=&GoodsState=YT";
            PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "商品信息查询"));
            hdfZP.Text = "";
        }
        protected void Window1_Close(object sender, WindowCloseEventArgs e)
        {
            DataTable dt = GetGoods(hfdValue.Text);
            dt.Columns.Remove(dt.Columns["BZHL"]);
            dt.Columns.Remove(dt.Columns["UNIT"]);
            if (dt != null && dt.Rows.Count > 0)
            {
                //dt.Columns["PIZNO"].ColumnName = "PZWH";
                dt.Columns["UNITNAME"].ColumnName = "UNITSMALLNAME";
                dt.Columns["BZHL_SELL"].ColumnName = "BZHL";
                dt.Columns["UNIT_SELL_NAME"].ColumnName = "UNITNAME";
                dt.Columns["UNIT_SELL"].ColumnName = "UNIT";

                dt.Columns.Add("PH", Type.GetType("System.String"));
                dt.Columns.Add("RQ_SC", Type.GetType("System.String"));
                dt.Columns.Add("YXQZ", Type.GetType("System.String"));
                dt.Columns.Add("MEMO", Type.GetType("System.String"));
                dt.Columns.Add("BZSL", Type.GetType("System.Int32"));
                dt.Columns.Add("DHS", Type.GetType("System.Int32"));
                dt.Columns.Add("DHSL", Type.GetType("System.Int32"));
                dt.Columns.Add("HSJE", Type.GetType("System.Double"));
                dt.Columns.Add("SPZTSL", Type.GetType("System.Int32"));
                dt.Columns.Add("KCSL", Type.GetType("System.Double"));
                dt.Columns.Add("NUM1", Type.GetType("System.Int32"));
                dt.Columns.Add("NUM1NAME", Type.GetType("System.String"));

                foreach (DataRow row in dt.Rows)
                {
                    row["BZSL"] = "0";
                    row["DHSL"] = "0";
                    row["HSJE"] = "0";
                    //处理金额格式
                    decimal jingdu = 0;
                    decimal bzhl = 0;
                    if (decimal.TryParse(row["HSJJ"].ToString(), out jingdu) && decimal.TryParse(row["BZHL"].ToString(), out bzhl))
                    { row["HSJJ"] = Math.Round(jingdu * bzhl, 4).ToString("F4"); }
                    if (decimal.TryParse(row["YBJ"].ToString(), out jingdu)) { row["YBJ"] = jingdu.ToString("F4"); }
                    if (decimal.TryParse(row["HSJE"].ToString(), out jingdu)) { row["HSJE"] = Math.Round(jingdu, 2).ToString("F2"); }

                    LoadGridRow(row, false);
                }
            }
            else
            {
                Alert.Show("系统传值错误！！！", "消息提示", MessageBoxIcon.Warning);
            }
        }
        #endregion
        #region Grid计算
        #endregion
        #region 单据审核驳回
        protected override void billAudit()
        {
            //科室审核
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("单据未提交或者已审核！", "操作提示", MessageBoxIcon.Warning);
                return;
            }
            string strBillno = docSEQNO.Text;
            //唯一码是否扫描校验
            if (DbHelperOra.Exists(String.Format("SELECT 1 FROM DAT_CK_EXT WHERE FLAG <>'Y' AND BILLNO = '{0}'", strBillno)))
            {
                Alert.Show("存在未扫描的高值码，请检查！", MessageBoxIcon.Warning);
                btnScan_Click(null, null);
                return;
            }
            if (BillOper(strBillno, "PASS2") == 1)
            {
                Alert.Show("单据【" + strBillno + "】审核成功！");
                billOpen(strBillno);
                OperLog("直接出库", "审核单据【" + docSEQNO.Text + "】");
            }
        }
        protected void btnRejectSubmit_Click(object sender, EventArgs e)
        {
            string strBillno = docSEQNO.Text;
            //将备注保存到单据中
            String Sql = "UPDATE DAT_CK_DOC SET MEMO = MEMO||'{0}' WHERE SEQNO = '{1}'";
            DbHelperOra.ExecuteSql(String.Format(Sql, txaMemo.Text.Trim(), strBillno));
            if (BillOper(strBillno, "PASS3") == 1)
            {
                WindowReject.Hidden = true;
                Alert.Show("单据【" + strBillno + "】驳回成功！");
                billOpen(strBillno);
                OperLog("直接出库", "驳回单据【" + docSEQNO.Text + "】");
            }
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            //科室审核
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("单据未提交或者已审核！", "操作提示", MessageBoxIcon.Warning);
                return;
            }
            WindowReject.Hidden = false;
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            //住院办提交
            if (docFLAG.SelectedValue != "M")
            {
                Alert.Show("单据未提交或者已提交！", "操作提示", MessageBoxIcon.Warning);
                return;
            }
            string strBillno = docSEQNO.Text;
            if (BillOper(strBillno, "PASS1") == 1)
            {
                Alert.Show("单据【" + strBillno + "】提交成功！");
                billOpen(strBillno);
                OperLog("直接出库", "提交单据【" + docSEQNO.Text + "】");
            }
        }
        #endregion
        #region Excel导出
        protected void btnEpt_Click(object sender, EventArgs e)
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【申领日期】！");
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                Alert.Show("开始日期大于结束日期，请重新输入！");
                return;
            }

            string strSql = @"SELECT A.BILLNO 单据编号,
                                       F_GETDEPTNAME(A.DEPTID) 申领部门,
                                       A.XSRQ 申领日期,
                                       F_GETDEPTNAME(A.DEPTOUT) 出库部门,
                                       F_GETUSERNAME(A.SLR) 申领人,
                                       F_GETUSERNAME(A.LRY) 录入人,
                                       A.LRRQ 录入日期,
                                       B.ROWNO 行号,
                                       B.GDSEQ 商品编码,
                                       B.GDNAME 商品名称,
                                       B.GDSPEC 商品规格,
                                       B.PZWH 注册证号,
                                       F_GETUNITNAME(B.UNIT) 单位,
                                       F_GETPRODUCERNAME(B.PRODUCER) 生产厂家,
                                       B.STR2 高值条码,
                                       B.BZHL 包装含量,
                                       B.BZSL 申领包装数,
                                       B.XSSL 申领数,B.HSJJ 价格,B.PH 批号,B.RQ_SC 生产日期,B.YXQZ 有效期至
                                  FROM DAT_CK_DOC A, DAT_CK_COM B
                                 WHERE A.SEQNO=B.SEQNO
                                   AND A.BILLTYPE = '" + BillType + @"'
                                   AND A.XSTYPE = 'G' ";
            string strSearch = "";


            if (tgbBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND A.BILLNO  LIKE '%{0}%'", tgbBILLNO.Text);
            }
            if (lstFLAG.SelectedItem != null && lstFLAG.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.FLAG='{0}'", lstFLAG.SelectedItem.Value);
            }
            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedItem.Value);
            }
            if (ddlDEPTOUT.SelectedValue.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTOUT='{0}'", ddlDEPTOUT.SelectedItem.Value);
            }
            strSearch += string.Format(" AND A.deptid in( select code FROM SYS_DEPT where type <>'1' and  F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            strSearch += string.Format(" AND A.LRRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.LRRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY A.BILLNO DESC,B.ROWNO";

            DataTable dt = DbHelperOra.Query(strSql).Tables[0];
            if (dt == null || dt.Rows.Count == 0)
            {
                Alert.Show("暂时没有符合要求的数据，无法导出", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            XTBase.Utilities.ExcelHelper.ExportByWeb(dt, "科室出库信息", string.Format("科室申领信息_{0}.xls", DateTime.Now.ToString("yyyyMMdd")));
        }
        #endregion
        protected void btnScan_Click(object sender, EventArgs e)
        {
            if (docFLAG.SelectedValue == "M" && docFLAG.SelectedValue == "R")
            {
                zsmERP.Enabled = false;
                zsmALL.Enabled = false;
            }
            else
            {
                zsmERP.Enabled = true;
                zsmALL.Enabled = true;
            }
            if (PubFunc.StrIsEmpty(docSEQNO.Text))
            {
                Alert.Show("请保存单据后进行扫描追溯码操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_CK_COM A,DOC_GOODS B WHERE A.GDSEQ = B.GDSEQ AND B.ISGZ = 'Y' AND SEQNO = '{0}'", docSEQNO.Text)))
            {
                Alert.Show("此单据中没有已经保存的高值商品,请检查！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            WindowScan.Hidden = false;
            ScanSearch("SHOW");
            zsmERP.Focus();
        }

        protected void ScanSearch(string type)
        {
            string sql = "";
            if (type == "SHOW")
            {
                sql = "SELECT A.*,f_getunitname(A.UNIT) UNITNAME,DECODE(A.FLAG,'Y','true','false') FLAGNAME FROM DAT_CK_EXT A WHERE A.BILLNO = '{0}' ORDER BY A.GDSEQ,A.INSTIME DESC";
            }
            else
            {
                sql = "SELECT A.*,f_getunitname(A.UNIT) UNITNAME,DECODE(A.FLAG,'Y','true','false') FLAGNAME FROM DAT_CK_EXT A WHERE A.BILLNO = '{0}' ORDER BY A.INSTIME DESC";
            }
            DataTable dtScan = DbHelperOra.Query(string.Format(sql, docSEQNO.Text)).Tables[0];
            GridSacn.DataSource = dtScan;
            GridSacn.DataBind();
        }

        protected void zsmDelete_Click(object sender, EventArgs e)
        {
            if ((",M,R").IndexOf(docFLAG.SelectedValue) < 0)
            {
                Alert.Show("非『新增』单据不允许操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (GridSacn.SelectedCell == null)
            {
                Alert.Show("请选择您需要删除的数据！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string onecode = (GridSacn.DataKeys[GridSacn.SelectedRowIndex][0]).ToString();
            string sSQL = string.Format("DELETE FROM DAT_CK_EXT WHERE ONECODE = '{0}' AND BILLNO ='{1}'", onecode, docBILLNO.Text);
            DbHelperOra.ExecuteSql(sSQL);
            ScanSearch("");
            //int SelectedIndex = GridGoods.SelectedRowIndex;
            //PageContext.RegisterStartupScript(GridGoods.GetDeleteRowReference(SelectedIndex));
        }

        protected void GridList_Sort(object sender, GridSortEventArgs e)
        {
            GridList.SortDirection = e.SortDirection;
            GridList.SortField = e.SortField;
            DataTable table = PubFunc.GridDataGet(GridList);
            DataView view1 = table.DefaultView;
            view1.Sort = String.Format("{0} {1}", GridList.SortField, GridList.SortDirection);
            GridList.DataSource = view1;
            GridList.DataBind();
        }

        protected void GridList_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridList.PageIndex = e.NewPageIndex;
            billSearch();
        }

        protected void docGZM_TextChanged(object sender, EventArgs e)
        {
            //扫码出库
            //if (docGZM.Text.Length < Doc.LENCODE()) return;
            if (docDEPTID.SelectedValue.Length < 1)
            {
                Alert.Show("【申领科室】未维护", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
            for (int i = 0; i < newDict.Count; i++)
            {
                if (newDict[i]["STR2"].ToString() == docGZM.Text.Trim())
                {
                    Alert.Show("扫描条码已增加到单据中！", "提示信息", MessageBoxIcon.Warning);
                    docGZM.Text = "";
                    docGZM.Focus();
                    return;
                }
            }
            string dept = Doc.ONECODE_GZ(docGZM.Text.Trim(), "DEPTCUR");
            if (docDEPTOUT.SelectedValue.Length > 0)
            {
                if (docDEPTOUT.SelectedValue != dept)
                {
                    Alert.Show("条码【" + docGZM.Text.Trim() + "】不属于库房【" + docDEPTOUT.SelectedText + "】", "提示信息", MessageBoxIcon.Warning);
                    docGZM.Text = "";
                    docGZM.Focus();
                    return;
                }
            }
            else
            {
                docDEPTOUT.SelectedValue = dept;
                if (docDEPTOUT.Text.Length < 1)
                {
                    Alert.Show("扫描条码的库房不存在或条码已被使用！");
                    docDEPTOUT.SelectedValue = "";
                    docGZM.Text = "";
                    docGZM.Focus();
                    return;
                }
            }
            docDEPTOUT.Enabled = false;
            string gdseq = Doc.ONECODE_GZ(docGZM.Text.Trim(), "GDSEQ");
            //增加商品
            //信息赋值
            DataTable dt = DbHelperOra.Query("SELECT A.* FROM DAT_GZ_EXT A WHERE (A.ONECODE = '" + docGZM.Text.Trim() + "' OR A.STR1 = '" + docGZM.Text.Trim() + "') AND FLAG IN('Y','R')").Tables[0];
            if (dt.Rows.Count < 1)
            {
                Alert.Show("扫描条码未入库或已被使用!", "提示信息", MessageBoxIcon.Warning);
                docGZM.Text = "";
                docGZM.Focus();
                return;
            }
            DataTable dt_goods = Doc.GetGoods_His(gdseq, "", docDEPTID.SelectedValue);
            if (dt_goods != null && dt_goods.Rows.Count > 0)
            {
                dt_goods.Columns.Add("BZSL", Type.GetType("System.Int32"));
                dt_goods.Columns.Add("HSJE", Type.GetType("System.Double"));
                dt_goods.Columns.Add("STR2", Type.GetType("System.String"));
                DataRow dr_goods = dt_goods.Rows[0];
                dr_goods["BZSL"] = "1";
                dr_goods["BZHL"] = "1";
                dr_goods["STR2"] = docGZM.Text.Trim();
                dr_goods["HSJE"] = dr_goods["HSJJ"];
                dr_goods["PH"] = dt.Rows[0]["PH"];
                dr_goods["YXQZ"] = dt.Rows[0]["YXQZ"];
                dr_goods["RQ_SC"] = dt.Rows[0]["RQ_SC"];
                LoadGridRow(dr_goods, false);
                docDEPTID.Enabled = false;
            }
            else
            {
                Alert.Show(string.Format("{0}尚未配置商品【{1}】！！！", docDEPTID.SelectedText, gdseq), MessageBoxIcon.Warning);
            }
            docGZM.Text = "";
            docGZM.Focus();
        }

        protected void zsmALL_Click(object sender, EventArgs e)
        {
            String Sql = "UPDATE DAT_CK_EXT SET FLAG = 'Y',OPERUSER='{1}',OPERDATE = SYSDATE WHERE BILLNO = '{0}' AND FLAG = 'N'";
            if (DbHelperOra.ExecuteSql(String.Format(Sql, docBILLNO.Text, UserAction.UserID)) > 0)
            {
                Alert.Show("一键入库成功！");
            }
            else
            {
                Alert.Show("一键入库失败，请检查！", MessageBoxIcon.Error);
            }
            ScanSearch("SHOW");
        }

        protected void zsmERP_TriggerClick(object sender, EventArgs e)
        {
            String Sql = "UPDATE DAT_CK_EXT SET FLAG = 'Y',OPERUSER='{1}',OPERDATE = SYSDATE WHERE BILLNO = '{0}' AND ONECODE = '{2}' AND FLAG = 'N'";
            if (DbHelperOra.ExecuteSql(String.Format(Sql, docBILLNO.Text, UserAction.UserID, zsmERP.Text.Trim())) > 0)
            {
                ScanSearch("SHOW");
                zsmERP.Text = "";
                zsmERP.Focus();
                closeGZwindow();
            }
            else
            {
                Alert.Show("高值码【" + zsmERP.Text + "】输入有误！", MessageBoxIcon.Warning);
                zsmERP.Text = "";
                zsmERP.Focus();
            }
        }
        protected void GridSacn_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                string flag = row["FLAG"].ToString();
                if (flag == "N")
                {
                    CloseWindow++;
                }
            }
        }
        private void closeGZwindow()
        {
            //天津医科大学总医院高值 优化建议，条码全部扫描完成后，窗口直接关闭，不需要手动关闭。 阿磊 2016年7月15日 14:57:55
            if (CloseWindow == 0)
            {
                WindowScan.Hidden = true;
            }
            CloseWindow = 0;
        }
        private void TJ_YKGZ()
        {//天津医科大学总医院高值 客户化需求 2016年7月12日 13:21:19 。
            zsmALL.Hidden = true;
        }
    }
}