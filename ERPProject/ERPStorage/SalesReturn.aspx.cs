﻿using FineUIPro;
using Newtonsoft.Json.Linq;
using XTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERPProject;
using XTBase.Utilities;

namespace ERPProject.ERPStorage
{
    public partial class SalesReturn : BillBase
    {
        private string strDocSql = "SELECT * FROM DAT_TH_DOC WHERE SEQNO ='{0}'";

        protected string CGTHD = "/grf/GoodsRtn.grf";
        public override Field[] LockControl
        {
            get { return new Field[] { docDEPTID, docBILLNO, docCGY, tgbRKDH, docPSSID, docTHRQ, docTHTYPE, docMEMO }; }
        }

        public SalesReturn()
        {
            BillType = "THD";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
                billNew();
                //屏蔽不需要的操作按钮
                ButtonHidden(btnCopy, btnNext, btnBef, btnExport, btnAddRow);
                if (Request.QueryString["oper"] != null)
                {
                    //获取oper的值
                    hfdOper.Text = Request.QueryString["oper"].ToString();
                    if (Request.QueryString["oper"].ToString() == "input")
                    {
                        ButtonHidden(btnAudit, btnCancel);
                        WebLine3.Hidden = true;
                    }
                    else if (Request.QueryString["oper"].ToString() == "audit")
                    {
                        ButtonHidden(btnNew, btnCopy, btnAddRow, btnDelRow, btnDel, btnSave, btnGoods, btnAllCommit, btnCommit);
                        WebLine1.Hidden = true;
                        WebLine2.Hidden = true;
                        TabStrip1.ActiveTabIndex = 0;
                        billSearch();
                    }
                }
            }
        }

        private void DataInit()
        {
            if (!isDg())
            {
                PubFunc.DdlDataGet("DDL_DOC_SUPPLIER", lstPSSID, docPSSID);
            }
            else
            {
                PubFunc.DdlDataGet("DDL_DOC_SUPPLIER_DG", lstPSSID, docPSSID);
                PubFunc.DdlDataGet(ddlReject, "DDL_BILL_REASON");
            }
            PubFunc.DdlDataGet("DDL_SYS_DEPOTRANGE", UserAction.UserID, ddlDEPTIN);
            //天津医科大学总医院 库房按照登录用户权限来加载 c 20151024
            //PubFunc.DdlDataGet("DDL_SYS_DEPOTRANGE", UserAction.UserID, docDEPTID, lstDEPTID);
            DepartmentBind.BindDDL("DDL_SYS_DEPOTRANGE", UserAction.UserID, docDEPTID, lstDEPTID);
            PubFunc.DdlDataGet("DDL_USER", lstCGY, docLRY, docSHR, docCGY);
            //PubFunc.DdlDataGet("DDL_SYS_DEPOT", lstDEPTID, docDEPTID);
            PubFunc.DdlDataGet(ddlReject, "DDL_BILL_REASON");
            PubFunc.DdlDataGet("DDL_BILL_FLAG_MNYR", docFLAG, lstFLAG);
            PubFunc.DdlDataGet(docTHTYPE, "DDL_RETURNREASON");

            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
            //获取客户化GRF文件地址  By c 2016年1月21日12:18:29 At 威海509
            string grf = Doc.DbGetGrf("CGTHD");
            if (!string.IsNullOrWhiteSpace(grf))
            {
                CGTHD = grf;
            }
        }
        private Boolean isDg()
        {
            if (Request.QueryString["dg"] == null)
            {
                return false;
            }
            else if (Request.QueryString["dg"].ToString() == "1")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        protected override void billNew()
        {
            string strSup = docPSSID.SelectedValue;
            string strDept = docDEPTID.SelectedValue;
            string strThyy = docTHTYPE.SelectedValue;

            //原单据保存判断
            PubFunc.FormDataClear(FormDoc);
            if (PubFunc.StrIsEmpty(strDept) && !isDg())
            {
                if (docDEPTID.Items.Count > 2)
                    strDept = docDEPTID.Items[1].Value;
            }
            if (PubFunc.StrIsEmpty(strSup))
            {
                strSup = Doc.DbGetSysPara("SUPPLIER");
            }
            docFLAG.SelectedValue = "M";
            docLRY.SelectedValue = UserAction.UserID;
            docCGY.SelectedValue = UserAction.UserID;
            docLRRQ.SelectedDate = DateTime.Now;
            docTHRQ.SelectedDate = DateTime.Now;
            docDEPTID.SelectedValue = strDept;
            docPSSID.SelectedValue = strSup;
            docTHTYPE.SelectedValue = strThyy;

            billLockDoc(false);
            GridGoods.SummaryData = null;
            comBZSL.Enabled = true;
            comMEMO.Enabled = true;
            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            btnDel.Enabled = false;
            btnSave.Enabled = true;
            btnCommit.Enabled = false;
            btnAudit.Enabled = false;
            btnCancel.Enabled = false;
            btnPrint.Enabled = false;
            btnDelRow.Enabled = true;
            btnGoods.Enabled = true;
            zsmScan.Enabled = true;
            zsmDelete.Enabled = true;
            if (Request.QueryString["tp"] != null && Request.QueryString["tp"].ToString().Trim().Length > 0)
            {
                docDEPTID.SelectedValue = DbHelperOra.GetSingle("SELECT T.STR2 FROM DOC_GOODSTYPE T WHERE T.CODE=" + Request.QueryString["tp"].ToString()).ToString();
            }
            else
            {
                docDEPTID.SelectedValue = DbHelperOra.GetSingle("SELECT T.STR2 FROM DOC_GOODSTYPE T WHERE T.CODE='2'").ToString();
            }
        }

        //protected void GridList_RowDataBound(object sender, GridRowEventArgs e)
        //{
        //    DataRowView row = e.DataItem as DataRowView;
        //    if (row != null)
        //    {
        //        string flag = row["FLAG"].ToString();
        //        if (flag == "M")
        //        {
        //            highlightRows.Text += e.RowIndex.ToString() + ",";
        //        }
        //        if (flag == "N")
        //        {
        //            highlightRowYellow.Text += e.RowIndex.ToString() + ",";
        //        }
        //    }
        //}


        private JObject GetJObject(Dictionary<string, object> dicRecord)
        {
            JObject defaultObj = new JObject();
            foreach (string key in dicRecord.Keys)
            {
                if (dicRecord[key] == null)
                {
                    defaultObj.Add(key, null);
                }
                else
                {
                    defaultObj.Add(key, dicRecord[key].ToString());
                }
                //defaultObj.Add(key, dicRecord[key].ToString());
            }

            decimal hl = 0, rs = 0, jg = 0;
            decimal.TryParse(dicRecord["BZHL"].ToString(), out hl);
            decimal.TryParse(dicRecord["BZSL"].ToString(), out rs);
            decimal.TryParse(dicRecord["HSJJ"].ToString(), out jg);

            defaultObj.Remove("THSL");
            defaultObj.Remove("HSJE");
            defaultObj.Remove("BZSL");
            defaultObj.Add("BZSL", Math.Abs(rs) * -1);
            defaultObj.Add("THSL", Math.Abs(rs) * -1 * hl);
            defaultObj.Add("HSJE", Math.Abs(rs) * -1 * jg);

            return defaultObj;
        }
        protected void GridGoods_AfterEdit(object sender, FineUIPro.GridAfterEditEventArgs e)
        {
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
            if (newDict.Count == 0) return;

            //处理返回jobject
            JObject defaultObj = Doc.GetJObject(GridGoods, e.RowID);
            //string cell = string.Format("[{0},{1}]", e.RowIndex, intCell[1]);
            if (e.ColumnID == "BZSL")
            {

                if (!PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "BZHL")) || !PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "BZSL")) || !PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "HSJJ")))
                {
                    Alert.Show("商品信息异常，请详细检查商品信息：包装含量或价格！", "异常信息", MessageBoxIcon.Warning);
                    return;
                }

                if ((Convert.ToInt32(defaultObj["BZSL"]) > Convert.ToInt32(defaultObj["KCSL"])) ||
                    (Convert.ToInt32(defaultObj["BZSL"]) * Convert.ToInt32(defaultObj["BZHL"]) > Convert.ToInt32(defaultObj["KCSL"])))
                {
                    Alert.Show("退货数量(最小包装)不能大于库存数量！", "异常信息", MessageBoxIcon.Warning);
                    defaultObj["BZSL"] = "0";
                }

                decimal hl = 0, rs = 0, jg = 0;
                decimal.TryParse((defaultObj["BZHL"] ?? "0").ToString(), out hl);
                decimal.TryParse((defaultObj["BZSL"] ?? "0").ToString(), out rs);
                decimal.TryParse((defaultObj["HSJJ"] ?? "0").ToString(), out jg);
                defaultObj["THSL"] = rs * hl;
                defaultObj["HSJE"] = Math.Round(rs * jg, 2).ToString("F2");
                PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(e.RowID, defaultObj));
                #region //计算合计数量
                decimal bzslTotal = 0, feeTotal = 0, thslTotal = 0;
                foreach (Dictionary<string, object> dic in newDict)
                {
                    if ((dic["BZSL"] ?? "0").ToString().Length > 0 && (dic["HSJJ"] ?? "0").ToString().Length > 0)
                    {
                        bzslTotal += Convert.ToDecimal(dic["BZSL"] ?? "0");
                        feeTotal += Convert.ToDecimal(dic["HSJJ"] ?? "0") * Convert.ToDecimal(dic["BZSL"] ?? "0");
                        thslTotal += Convert.ToDecimal(dic["BZHL"] ?? "0") * Convert.ToDecimal(dic["BZSL"] ?? "0");
                    }

                    object objISFLAG5 = DbHelperOra.GetSingle(string.Format("SELECT ISFLAG5 FROM DOC_GOODS WHERE GDSEQ = '{0}'", dic["GDSEQ"]));

                    if (objISFLAG5.ToString() == "N")
                    {
                        string str = Convert.ToString(Convert.ToDecimal(dic["BZSL"] ?? "0"));
                        if (Convert.ToDecimal(dic["BZSL"]) != (int)Convert.ToDecimal(dic["BZSL"]) && Convert.ToDecimal(dic["BZHL"] ?? "0") == 1)
                        {
                            Alert.Show("当前商品不支持申领数为小数，请调整", "消息提示", MessageBoxIcon.Warning);

                        }
                    }
                }

                JObject summary = new JObject();
                summary.Add("GDNAME", "本页合计");
                summary.Add("BZSL", bzslTotal.ToString());
                summary.Add("HSJE", feeTotal.ToString("F2"));
                summary.Add("THSL", thslTotal.ToString("F2"));

                GridGoods.SummaryData = summary;
                #endregion
            }
            else if (e.ColumnID == "PH")
            {
                if (PubFunc.StrIsEmpty(defaultObj["GDSEQ"].ToString()))
                {
                    Alert.Show("请先选择商品信息！", "异常信息", MessageBoxIcon.Warning);
                    return;
                }
                String sql = "";
                if (defaultObj["PH"].ToString().Length > 0 && defaultObj["PH"].ToString() != "\\")
                {
                    sql = string.Format("SELECT A.* FROM DAT_GOODSSTOCK A WHERE A.DEPTID = '{0}' AND A.GDSEQ ='{1}' AND A.PH = '{2}' AND KCSL > LOCKSL", docDEPTID.SelectedValue, defaultObj["GDSEQ"].ToString(), defaultObj["PH"].ToString());
                }
                else
                {
                    sql = string.Format("SELECT A.* FROM DAT_GOODSSTOCK A WHERE A.DEPTID = '{0}' AND A.GDSEQ ='{1}' AND KCSL > LOCKSL", docDEPTID.SelectedValue, defaultObj["GDSEQ"].ToString());
                }
                DataTable dtPH = DbHelperOra.Query(sql).Tables[0];
                if (dtPH != null && dtPH.Rows.Count > 0)
                {
                    if (dtPH.Rows.Count == 1)
                    {

                        defaultObj["PH"] = dtPH.Rows[0]["PH"].ToString();
                        defaultObj["RQ_SC"] = dtPH.Rows[0]["RQ_SC"].ToString();
                        defaultObj["YXQZ"] = dtPH.Rows[0]["YXQZ"].ToString();
                        defaultObj["KCSL"] = dtPH.Rows[0]["KCSL"].ToString();
                        defaultObj["HWID"] = dtPH.Rows[0]["HWID"].ToString();

                        PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(e.RowID, defaultObj));
                    }
                    else
                    {
                        hfdRowIndex.Text = GridGoods.SelectedRowIndex.ToString();
                        GridLot.DataSource = dtPH;
                        GridLot.DataBind();
                        WindowLot.Hidden = false;
                    }
                }
                else
                {
                    Alert.Show("填写的批次信息不存在,请重新填写!", "异常信息", MessageBoxIcon.Warning);
                    return;
                }
            }
        }

        protected override void billClear()
        {
            PubFunc.FormDataClear(Formlist);
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
        }

        protected override void billAddRow()
        {
            if (docFLAG.SelectedValue != "M" && docFLAG.SelectedValue != "R")
            {
                Alert.Show("非『新增』单据不能增行！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
            billLockDoc(true);
            PubFunc.GridRowAdd(GridGoods, "INIT");
        }

        protected override void billGoods()
        {
            if (docFLAG.SelectedValue != "M" && docFLAG.SelectedValue != "R")
            {
                Alert.Show("非新增单据不能追加商品！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
            PubFunc.FormLock(FormDoc, true);
            docMEMO.Enabled = true;
            if (tgbRKDH.Text.Trim().Length == 0)
            {
                Alert.Show("无入库单据不能退货！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            DataComSearch();
            WindowLot.Hidden = false;

            hdfZP.Text = "";
        }
        //追加赠品
        protected void btnGoodsZP_Click(object sender, EventArgs e)
        {
            if (docFLAG.SelectedValue != "M" && docFLAG.SelectedValue != "R")
            {
                Alert.Show("非新增单据不能追加商品！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
            PubFunc.FormLock(FormDoc, true);
            docMEMO.Enabled = true;
            string url = "~/ERPQuery/GoodsWindow_Zp.aspx?bm=" + docDEPTID.SelectedValue + "_" + docDEPTID.SelectedValue + "&cx=&su=";
            PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "赠品商品信息查询"));
            hdfZP.Text = "ZP";
        }

        protected override void billSearch()
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【退货日期】！");
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                Alert.Show("开始日期大于结束日期，请重新输入！");
                return;
            }

            string strSql = @"SELECT A.SEQNO,A.BILLNO, F_GETBILLFLAG(FLAG) FLAG_CN, FLAG,F_GETDEPTNAME(A.DEPTID) DEPTID,A.THRQ,F_GETSUPNAME(A.PSSID) SUPNAME,A.SUBSUM,
                                     A.SUBNUM,F_GETUSERNAME(A.CGY) CGY,F_GETUSERNAME(A.LRY) LRY,A.LRRQ,F_GETUSERNAME(A.SHR) SHR,A.SHRQ,A.MEMO,RKDH
                                from DAT_TH_DOC A
                                WHERE NVL(A.NUM1,0) = 0 AND A.DEPTID in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '" + UserAction.UserID + "') = 'Y' ) ";
            string strSearch = "";

            if (lstBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND A.BILLNO  LIKE '%{0}%'", lstBILLNO.Text);
            }
            if (lstFLAG.SelectedItem != null && lstFLAG.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.FLAG='{0}'", lstFLAG.SelectedItem.Value);
            }
            if (lstCGY.SelectedItem != null && lstCGY.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.CGY='{0}'", lstCGY.SelectedItem.Value);
            }
            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedItem.Value);
            }
            if (lstPSSID.SelectedItem != null && lstPSSID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.PSSID='{0}'", lstPSSID.SelectedItem.Value);
            }
            //过滤新单的单据，不能审核，提交后的才能审核
            if (Request.QueryString["oper"].ToString() == "audit")
            {
                strSearch += " AND A.FLAG<>'M'";
            }
            strSearch += string.Format(" AND A.THRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.THRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY A.FLAG,A.BILLNO DESC";
            highlightRows.Text = "";
            highlightRowYellow.Text = "";
            GridList.DataSource = DbHelperOra.Query(strSql).Tables[0];
            GridList.DataBind();
        }

        protected override void billAudit()
        {
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("非『已提交』单据不能审核！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            //验证是否盘点
            if (DbHelperOra.Exists("SELECT 1 FROM DAT_PD_LOCK WHERE DEPTID = '" + docDEPTID.SelectedValue + "' AND FLAG='N'"))
            {
                Alert.Show(string.Format("部门【{0}】正在盘点,请盘点后进行操作！", docDEPTID.SelectedText), "警告提示", MessageBoxIcon.Warning);
                return;
            }

            string strBillno = docSEQNO.Text;
            if (!Doc.getFlag(docBILLNO.Text, "N", BillType))
            {
                Alert.Show("此单据已经被别人操作,不能审核！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            try
            {
                if (BillOper(strBillno, "AUDIT") == 1)
                {
                    billLockDoc(true);
                    Alert.Show("单据【" + strBillno + "】审核成功！");
                    billOpen(strBillno);
                    OperLog("商品退货", "审核单据【" + strBillno + "】");
                }
            }
            catch (Exception ex)
            {
                Alert.Show(ex.Message);
            }
        }

        protected override void listRow_DoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            string strBillno = GridList.Rows[e.RowIndex].Values[1].ToString();
            billOpen(strBillno);
            if (DbHelperOra.Exists("select 1 from DAT_TH_DOC where SEQNO = '" + strBillno + "' AND FLAG='M'"))
            { //双击打开单据信息时，如果单据状态在数据库中依然是新单状态，到货日期允许修改
                docMEMO.Enabled = true;
            }
        }

        protected override void billOpen(string strBillno)
        {
            string sql = @"SELECT F_GETUNITNAME(A.UNIT) UNITNAME,
                                  F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME,
                                  F_GETSUPNAME(A.SUPID) SUPNAME,
                                  (select NVL(SUM(KCSL), 0)
                                      from DAT_GOODSSTOCK
                                      where GDSEQ = A.GDSEQ
                                      and DEPTID = B.DEPTID AND PH = A.PH AND BILLNO=B.RKDH) KC, D.SL-NVL(D.NUM1,0)KCSL,
                                A.SEQNO,A.ROWNO,A.DEPTID,A.GDSEQ,A.BARCODE,A.GDNAME,A.GDSPEC,A.GDMODE,A.HWID,A.BZHL,A.BZSL,A.NUM2,
       A.THSL,A.SSSL,A.JXTAX,A.HSJJ,A.HSJE,A.BHSJJ,A.BHSJE,A.ISLOT,A.PHID,A.PH,A.PZWH,A.RQ_SC,A.YXQZ,A.UNIT,
       A.MEMO,A.NUM1,f_getunitname(C.UNIT) UNITSMALLNAME ,DECODE(NVL(A.NUM1,0),0,'非赠品','赠品') NUM1NAME
                              FROM DAT_TH_COM A,DAT_TH_DOC B,DOC_GOODS C,(SELECT BILLNO,GDSEQ,PH,ROWNO,SUM(SL) SL, SUM(NUM1) NUM1 FROM DAT_GOODSJXC GROUP BY BILLNO,GDSEQ,PH,ROWNO) D
                              WHERE A.SEQNO = B.SEQNO AND A.SEQNO = '{0}' AND A.GDSEQ=C.GDSEQ AND A.ROWNO=D.ROWNO AND D.BILLNO=B.RKDH AND A.GDSEQ=D.GDSEQ ORDER BY A.ROWNO";

            DataTable dtDoc = DbHelperOra.Query(string.Format(strDocSql, strBillno)).Tables[0];
            PubFunc.FormDataSet(FormDoc, dtDoc.Rows[0]);

            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            DataTable dtBill = DbHelperOra.Query(string.Format(sql, strBillno)).Tables[0];
            if (dtBill != null && dtBill.Rows.Count > 0)
            {
                decimal bzslTotal = 0, feeTotal = 0, thslTotal = 0;
                foreach (DataRow row in dtBill.Rows)
                {
                    //LoadGridRow(row, false, "OLD");
                    bzslTotal += Convert.ToDecimal(row["BZSL"]);
                    feeTotal += Convert.ToDecimal(row["HSJJ"]) * Convert.ToDecimal(row["BZSL"] ?? "0");
                    thslTotal += Convert.ToDecimal(row["BZHL"] ?? "0") * Convert.ToDecimal(row["BZSL"] ?? "0");
                }
                /*
                *  修 改 人 ：袁鹏    修改时间：2015-03-20
                *  信息说明：这种加载方法比LoadGridRow(row, false, "OLD")更高效
                */
                Doc.GridRowAdd(GridGoods, dtBill);
                //计算合计数量
                JObject summary = new JObject();
                summary.Add("GDNAME", "本页合计");
                summary.Add("BZSL", bzslTotal.ToString());
                summary.Add("HSJE", feeTotal.ToString("F2"));
                summary.Add("THSL", thslTotal.ToString());
                GridGoods.SummaryData = summary;
            }

            PubFunc.FormLock(FormDoc, true);
            TabStrip1.ActiveTabIndex = 1;
            if (docFLAG.SelectedValue == "M" || docFLAG.SelectedValue == "R")
            {
                btnDel.Enabled = true;
                btnSave.Enabled = true;
                btnCommit.Enabled = true;
                btnAudit.Enabled = false;
                btnCancel.Enabled = false;
                btnPrint.Enabled = false;
                btnDelRow.Enabled = true;
                btnGoods.Enabled = true;
                zsmScan.Enabled = true;
                zsmDelete.Enabled = true;
            }
            else if (docFLAG.SelectedValue == "N")
            {
                btnDel.Enabled = false;
                btnSave.Enabled = false;
                btnCommit.Enabled = false;
                btnAudit.Enabled = true;
                btnCancel.Enabled = true;
                btnPrint.Enabled = false;
                btnDelRow.Enabled = false;
                btnGoods.Enabled = false;
                zsmScan.Enabled = false;
                zsmDelete.Enabled = false;
            }
            else
            {
                btnDel.Enabled = false;
                btnSave.Enabled = false;
                btnCommit.Enabled = false;
                btnAudit.Enabled = false;
                btnCancel.Enabled = false;
                btnPrint.Enabled = true;
                btnDelRow.Enabled = false;
                btnGoods.Enabled = false;
                zsmScan.Enabled = false;
                zsmDelete.Enabled = false;
            }
        }
        //导出
        protected void btExport_Click(object sender, EventArgs e)
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【录入日期】！");
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                Alert.Show("开始日期大于结束日期，请重新输入！");
                return;
            }

            string strSql = @"SELECT B.SEQNO 单据编号,
       F_GETBILLFLAG(B.FLAG) 单据状态,
       B.RKDH 入库单号,
       B.DEPTID 退货部门,
       F_GETSUPNAME(B.PSSID)送货商名称,
       B.THRQ 退货日期,
       F_GETUSERNAME(B.CGY)业务员,
       F_GETUSERNAME(B.LRY) 录入员,
       B.LRRQ 录入日期,
       F_GETUSERNAME(B.SHR) 审核员,
       B.SHRQ 审核日期,
       B.MEMO 备注,
       A.ROWNO 行号,
       ' '||A.GDSEQ 商品编码,
       A.GDNAME 商品名称,
       A.GDSPEC 商品规格,
       F_GETUNITNAME(A.UNIT) 单位,
       A.PH 批号,
       A.RQ_SC 生产日期,
       A.YXQZ 有效期至,
       A.HWID 货位,
       A.BZHL 包装含量,
       A.SSSL 退货数量,
       A.HSJJ 含税进价,
       A.HSJE 含税金额, 
       A.PZWH 注册证号,
       A.MEMO 备注
       
  FROM DAT_TH_COM A, DAT_TH_DOC B
 WHERE A.SEQNO = B.SEQNO";
            string strSearch = "";

            if (lstBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND A.BILLNO  LIKE '%{0}%'", lstBILLNO.Text);
            }
            if (lstFLAG.SelectedItem != null && lstFLAG.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND B.FLAG='{0}'", lstFLAG.SelectedItem.Value);
            }
            if (lstCGY.SelectedItem != null && lstCGY.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND B.CGY='{0}'", lstCGY.SelectedItem.Value);
            }
            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedItem.Value);
            }
            if (lstRKDH.Text.Length > 0)
            {
                strSearch += string.Format(" AND A.RKDH='{0}'", lstRKDH.Text);
            }
            if (lstPSSID.SelectedItem != null && lstPSSID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.PSSID='{0}'", lstPSSID.SelectedItem.Value);
            }
            strSearch += string.Format(" AND  B.LRRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND  B.LRRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY A.ROWNO";

            DataTable dt = DbHelperOra.Query(strSql).Tables[0];
            ExcelHelper.ExportByWeb(dt, string.Format("采购退货信息"), "采购退货信息导出_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
        }
        protected override void billSave()
        {
            save();
        }

        private void save(string flag = "N")
        {
            #region 数据有效性验证
            if (docFLAG.SelectedValue != "M" && docFLAG.SelectedValue != "R")
            {
                Alert.Show("非新单不能保存！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            //if (!CheckFlag("M") && !CheckFlag("R"))
            //{
            //    Alert.Show("此单据已经被别人操作，请等待操作!");
            //    return;
            //}此方法无法校验，只生成单据号，但没生成DOC和COM的地方cjl
            if (docFLAG.SelectedValue != "M" && docFLAG.SelectedValue != "R")
            {

                Alert.Show("此单据已经被别人操作，请等待操作!");
                return;

            }
            if (PubFunc.FormDataCheck(FormDoc).Length > 0) return;
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();//.OrderBy(x => x["GDSEQ"]).ToList(); //排序这里给关掉
            if (newDict.Count == 0)
            {
                Alert.Show("请输入商品信息", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            List<Dictionary<string, object>> goodsData = new List<Dictionary<string, object>>();
            //判断是否有空行、批号填写是否符合要求
            for (int i = 0; i < newDict.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(newDict[i]["GDSEQ"].ToString()) && !string.IsNullOrWhiteSpace(newDict[i]["GDNAME"].ToString()))
                {
                    if (string.IsNullOrWhiteSpace(newDict[i]["BZHL"].ToString()) || string.IsNullOrWhiteSpace(newDict[i]["UNIT"].ToString()))
                    {
                        Alert.Show("商品[" + newDict[i]["GDSEQ"] + "]包装单位信息错误，请联系管理员维护！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    if ((newDict[i]["BZSL"] ?? "").ToString() == "" || (newDict[i]["BZSL"] ?? "0").ToString() == "0")
                    {
                        Alert.Show("请填写商品[" + newDict[i]["GDSEQ"] + "]退货数！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    if (decimal.Parse(newDict[i]["KCSL"].ToString()) < decimal.Parse(newDict[i]["BZSL"].ToString()))
                    {
                        Alert.Show("第【" + (i + 1).ToString() + "】行商品『" + newDict[i]["GDNAME"].ToString() + "』退货数大于库存数，不能退货！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    if (((newDict[i]["HSJJ"] ?? "").ToString() == "" || (newDict[i]["HSJJ"] ?? "").ToString() == "0") && (newDict[i]["NUM1"] ?? "").ToString() != "1")
                    {
                        Alert.Show("商品[" + newDict[i]["GDSEQ"] + "]的【含税进价】不能为零,请维护好商品的含税进价，再次保存！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    if (newDict[i]["ISLOT"].ToString() == "1" || newDict[i]["ISLOT"].ToString() == "2")
                    {
                        if (string.IsNullOrWhiteSpace(newDict[i]["PH"].ToString()))
                        {
                            string[] selectedCell = GridGoods.SelectedCell;
                            PageContext.RegisterStartupScript(String.Format("F('{0}').selectCell('{1}','{2}');", GridGoods.ClientID, selectedCell[0], selectedCell[1]));
                            Alert.Show("商品【" + newDict[i]["GDNAME"].ToString() + "】批号不能为空！！！", "消息提示", MessageBoxIcon.Warning);
                            return;
                        }
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
                string flg = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'N') FROM DAT_TH_DOC WHERE SEQNO='{0}'", docBILLNO.Text));
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
            //DataTable dtrkd = DbHelperOra.QueryForTable("SELECT DRD.SEQNO,DRC.GDSEQ,DRC.BZSL FROM DAT_RK_DOC DRD,DAT_RK_COM DRC WHERE DRD.SEQNO=DRC.SEQNO(+) AND DRD.FLAG='Y' AND DRD.SEQNO='" + tgbRKDH.Text + "' ");
            DataTable dtrkd = DbHelperOra.QueryForTable("SELECT A.BILLNO,A.GDSEQ,(A.KCSL-A.LOCKSL) KC,A.PH FROM DAT_GOODSSTOCK A,DAT_RK_COM DRC WHERE A.GDSEQ=DRC.GDSEQ AND A.BILLNO=DRC.SEQNO AND A.PH=DRC.PH AND A.BILLNO='" + tgbRKDH.Text + "' AND A.DEPTID='" + docDEPTID.SelectedValue + "' ");
            string msg = "";
            foreach (DataRow dr in dtrkd.Rows)
            {
                for (int i = 0; i < newDict.Count; i++)
                {
                    if (newDict[i]["GDSEQ"].ToString().Equals(dr[1].ToString()) && newDict[i]["PH"].ToString().Equals(dr[3].ToString()))
                    {
                        if (Math.Abs(decimal.Parse(newDict[i]["BZSL"].ToString())) > decimal.Parse(dr[2].ToString()))
                        {
                            Alert.Show("商品【" + newDict[i]["GDNAME"].ToString() + "】退数量大于入库单【" + dr[0].ToString() + "】剩余库存数。", "消息提示", MessageBoxIcon.Warning);
                            return;
                            //msg = "商品【" + newDict[i]["GDNAME"].ToString() + "】可退数量大于入库单【" + dr[0].ToString() + "】中的入库数,但库存数满足退货数量。<br />";
                        }
                    }
                }
            }
            MyTable mtType = new MyTable("DAT_TH_DOC");
            mtType.ColRow = PubFunc.FormDataHT(FormDoc);
            mtType.ColRow["SEQNO"] = docBILLNO.Text;
            mtType.ColRow["FLAG"] = "M";
            mtType.ColRow.Add("BILLTYPE", BillType);
            mtType.ColRow.Add("SUBNUM", goodsData.Count);
            mtType.ColRow.Add("SUPNAME", docPSSID.SelectedText);
            mtType.ColRow.Add("DEPTDH", mtType.ColRow["DEPTID"]);

            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtTypeMx = new MyTable("DAT_TH_COM");
            decimal subNum = 0;//总金额
                               //先删除单据信息在插入
            cmdList.Add(new CommandInfo("delete DAT_TH_DOC where seqno='" + docBILLNO.Text + "'", null));//删除单据台头
            cmdList.Add(new CommandInfo("delete DAT_TH_COM where seqno='" + docBILLNO.Text + "'", null));//删除单据明细
            for (int i = 0; i < goodsData.Count; i++)
            {
                mtTypeMx.ColRow = PubFunc.GridDataGet(goodsData[i]);
                mtTypeMx.ColRow["BZSL"] = Math.Abs(decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString())) * -1;
                mtTypeMx.ColRow["HSJE"] = decimal.Parse(mtTypeMx.ColRow["HSJJ"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                subNum = subNum + decimal.Parse(mtTypeMx.ColRow["HSJE"].ToString());
                mtTypeMx.ColRow.Add("SEQNO", docBILLNO.Text);
                Object obj = DbHelperOra.GetSingle(string.Format("SELECT T.ROWNO FROM DAT_RK_COM T WHERE T.GDSEQ='{0}' AND T.SEQNO='{1}' AND T.PH='{2}' AND TO_CHAR(T.YXQZ,'YYYY-MM-DD')='{3}' AND ROWNUM=1", mtTypeMx.ColRow["GDSEQ"].ToString(), tgbRKDH.Text, mtTypeMx.ColRow["PH"], mtTypeMx.ColRow["YXQZ"]));
                if (obj != null && obj.ToString().Length > 0)
                {
                    mtTypeMx.ColRow["ROWNO"] = obj.ToString();
                }
                else
                {
                    mtTypeMx.ColRow["ROWNO"] = i + 1;
                }

                mtTypeMx.ColRow.Add("PHID", mtTypeMx.ColRow["PH"]);
                mtTypeMx.ColRow["THSL"] = decimal.Parse(mtTypeMx.ColRow["BZHL"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                mtTypeMx.ColRow.Add("SSSL", mtTypeMx.ColRow["THSL"]);
                mtTypeMx.ColRow.Add("DEPTID", docDEPTID.SelectedValue);
                mtTypeMx.ColRow.Add("BHSJJ", 0);
                mtTypeMx.ColRow.Add("BHSJE", 0);
                mtTypeMx.ColRow.Remove("UNITNAME");
                mtTypeMx.ColRow.Remove("PRODUCERNAME");
                //mtTypeMx.ColRow.Remove("SUPNAME");
                //mtTypeMx.ColRow.Remove("SUPID");//添加选择的供应商
                //mtTypeMx.ColRow.Add("SUPID", docPSSID.SelectedValue);
                cmdList.Add(mtTypeMx.Insert());
            }
            mtType.ColRow.Add("SUBSUM", subNum);
            cmdList.AddRange(mtType.InsertCommand());
            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                if (flag == "N")
                    Alert.Show(msg + "商品退货信息保存成功！");
                billOpen(docBILLNO.Text);
                billLockDoc(true);
                OperLog("商品退货", "修改单据【" + docBILLNO.Text + "】");
                SaveSuccess = true;
            }
            else
            {
                Alert.Show("商品退货信息保存失败，请联系管理员！");
            }
        }

        private bool SaveSuccess = false;


        protected void Window1_Close(object sender, WindowCloseEventArgs e)
        {
            DataTable dt = GetGoods(hfdValue.Text);
            if (dt != null && dt.Rows.Count > 0)
            {
                //dt.Columns.Add("PH", Type.GetType("System.String"));
                //dt.Columns.Add("RQ_SC", Type.GetType("System.String"));
                //dt.Columns.Add("YXQZ", Type.GetType("System.String"));
                dt.Columns.Add("MEMO", Type.GetType("System.String"));
                dt.Columns.Add("BZSL", Type.GetType("System.Int32"));
                dt.Columns.Add("THSL", Type.GetType("System.Int32"));
                dt.Columns.Add("SSSL", Type.GetType("System.Int32"));
                dt.Columns.Add("HSJE", Type.GetType("System.Double"));
                //dt.Columns.Add("KCSL", Type.GetType("System.Int32"));
                dt.Columns.Add("SPZTSL", Type.GetType("System.Int32"));
                dt.Columns.Add("NUM1", Type.GetType("System.Int32"));
                dt.Columns.Add("NUM1NAME", Type.GetType("System.String"));
                string msg = "", msg1 = "";

                foreach (DataRow row in dt.Rows)
                {
                    row["BZSL"] = "0";
                    row["THSL"] = "0";
                    row["SSSL"] = "0";
                    row["HSJE"] = "0";
                    //row["KCSL"] = "0";
                    if (hdfZP.Text == "ZP")
                    {
                        row["NUM1"] = "1";
                        row["NUM1NAME"] = "赠品";
                    }
                    else
                    {
                        row["NUM1"] = "0";
                        row["NUM1NAME"] = "非赠品";
                    }
                    List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
                    for (int i = 0; i < newDict.Count; i++)
                    {
                        if (newDict[i]["GDSEQ"].ToString() == row["GDSEQ"].ToString() && newDict[i]["PH"].ToString() == row["PH"].ToString())
                        {
                            msg1 += row["GDNAME"] + ",";
                        }
                    }

                    if (Convert.ToInt32(row["KCSL"]) <= 0)
                    {
                        msg += row["GDNAME"] + ",";
                        continue;
                    }
                    else
                    {
                        PubFunc.GridRowAdd(GridGoods, row, false);
                    }
                }

                if (!string.IsNullOrWhiteSpace(msg))
                {
                    Alert.Show(string.Format("商品【{0}】在部门『{1}』中没有库存,不能进行退货！", msg, docDEPTID.SelectedText), "消息提示", MessageBoxIcon.Warning);
                    return;
                }
                if (!string.IsNullOrWhiteSpace(msg1))
                {
                    Alert.Show(string.Format("商品【{0}】需要添加的批次已存在！", msg1, docDEPTID.SelectedText), "消息提示", MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                Alert.Show("请选择商品！", "消息提示", MessageBoxIcon.Warning);
            }

        }
        private DataTable LoadGridData(DataTable dt)
        {
            DataTable mydt = dt.Copy();

            foreach (DataRow row in mydt.Rows)
            {

                //处理金额格式
                decimal jingdu = 0;
                if (decimal.TryParse(row["HSJJ"].ToString(), out jingdu)) { row["HSJJ"] = jingdu.ToString("F6"); }
                if (decimal.TryParse(row["YBJ"].ToString(), out jingdu)) { row["YBJ"] = jingdu.ToString("F6"); }
                if (decimal.TryParse(row["THSL"].ToString(), out jingdu)) { row["THSL"] = jingdu.ToString("F0"); }
                if (decimal.TryParse(row["SSSL"].ToString(), out jingdu)) { row["SSSL"] = jingdu.ToString("F0"); }
                if (decimal.TryParse(row["KCSL"].ToString(), out jingdu)) { row["KCSL"] = jingdu.ToString("F0"); }
                if (decimal.TryParse(row["SPZTSL"].ToString(), out jingdu)) { row["SPZTSL"] = jingdu.ToString("F6"); }

                //PubFunc.GridRowAdd(GridGoods, row, firstRow);
            }
            return mydt;
        }

        #region 该内容由于业务变动废弃掉了 alei 20150510
        /// <summary>
        /// 该方法废弃 c 20150510
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void comGDSEQ_TriggerClick(object sender, EventArgs e)
        {
            string code = labGDSEQ.Text;
            string dept = docDEPTID.SelectedValue;

            if (!string.IsNullOrWhiteSpace(code) && code.Trim().Length >= 2)
            {
                DataTable dt_goods = Doc.GetGoods(code, "", dept);

                if (dt_goods != null && dt_goods.Rows.Count > 0)
                {
                    dt_goods.Columns.Add("BZSL", Type.GetType("System.Int32"));
                    dt_goods.Columns.Add("THSL", Type.GetType("System.Int32"));
                    dt_goods.Columns.Add("SSSL", Type.GetType("System.Int32"));
                    dt_goods.Columns.Add("HSJE", Type.GetType("System.Double"));
                    dt_goods.Columns.Add("KCSL", Type.GetType("System.Int32"));
                    dt_goods.Columns.Add("SPZTSL", Type.GetType("System.Int32"));

                    DataRow dr_goods = dt_goods.Rows[0];
                    dr_goods["BZSL"] = "0";
                    dr_goods["THSL"] = "0";
                    dr_goods["SSSL"] = "0";
                    dr_goods["HSJE"] = "0";
                    dr_goods["STR1"] = "";

                    DataTable dtPH = new DataTable();
                    if (!isDg())
                    {
                        dtPH = Doc.GetGoodsPHKC(code, docDEPTID.SelectedValue, docPSSID.SelectedValue);
                    }
                    else
                    {
                        dtPH = Doc.GetGoodsPHKC_DG(code, docDEPTID.SelectedValue, docPSSID.SelectedValue);
                    }

                    if (dtPH != null && dtPH.Rows.Count > 0)
                    {
                        if (dtPH.Rows[0]["KCSL"].ToString() == "0")
                        {
                            Alert.Show(string.Format("商品【{1}】在{0}中没有库存，不能进行退货！", docDEPTID.SelectedText, code), MessageBoxIcon.Warning);
                            PubFunc.GridRowAdd(GridGoods, "CLEAR");
                            return;
                        }
                        if (dtPH.Rows.Count == 1)
                        {
                            dr_goods["PH"] = dtPH.Rows[0]["PH"];
                            dr_goods["PZWH"] = dtPH.Rows[0]["PZWH"];
                            dr_goods["RQ_SC"] = dtPH.Rows[0]["RQ_SC"];
                            dr_goods["YXQZ"] = dtPH.Rows[0]["YXQZ"];
                            dr_goods["KCSL"] = dtPH.Rows[0]["KCSL"];
                        }
                        else
                        {
                            hfdRowIndex.Text = GridGoods.SelectedRowIndex.ToString();
                            GridLot.DataSource = dtPH;
                            GridLot.DataBind();
                            WindowLot.Hidden = false;

                        }
                    }
                    else
                    {
                        Alert.Show(string.Format("商品【{1}】在{0}中没有库存，不能进行退货！", docDEPTID.SelectedText, code), MessageBoxIcon.Warning);
                        PubFunc.GridRowAdd(GridGoods, "CLEAR");
                        return;
                    }
                    LoadGridRow(dr_goods);
                }
                else
                {
                    Alert.Show(string.Format("{0}尚未配置商品【{1}】！！！", docDEPTID.SelectedText, code), MessageBoxIcon.Warning);
                    PubFunc.GridRowAdd(GridGoods, "CLEAR");
                }
            }
        }
        /// <summary>
        /// FineUIPro.Grid控件数据加载
        /// </summary>
        /// <param name="row">要加载的行数据</param>
        /// <param name="firstRow">是否插入指定行</param>
        /// <param name="flag">数据来源：NEW-从数据库中获得，用于商品新增时；OLD-从销售单据明细中获得，用于修改或审核时</param>
        private void LoadGridRow(DataRow row, bool firstRow = true, string flag = "NEW")
        {
            if (flag == "NEW")
            {
                if (!string.IsNullOrWhiteSpace(row["UNIT_ORDER"].ToString()))
                {
                    if (row["UNIT_ORDER"].ToString() == "D")//订货单位为大包装时
                    {
                        if (!string.IsNullOrWhiteSpace(row["NUM_DABZ"].ToString()) && row["NUM_DABZ"].ToString() != "0")
                        {
                            row["UNIT"] = row["UNIT_DABZ"];
                            row["UNITNAME"] = row["UNIT_DABZ_NAME"];
                            row["BZHL"] = row["NUM_DABZ"];
                            int price = 0, number = 0;
                            int.TryParse(row["HSJJ"].ToString(), out price);
                            int.TryParse(row["NUM_DABZ"].ToString(), out number);
                            row["HSJJ"] = price * number;
                        }
                    }
                    else if (row["UNIT_ORDER"].ToString() == "Z")//订货单位为中包装时
                    {
                        if (!string.IsNullOrWhiteSpace(row["NUM_ZHONGBZ"].ToString()) && row["NUM_ZHONGBZ"].ToString() != "0")
                        {
                            row["UNIT"] = row["UNIT_ZHONGBZ"];
                            row["UNITNAME"] = row["UNIT_ZHONGBZ_NAME"];
                            row["BZHL"] = row["NUM_ZHONGBZ"];
                            int price = 0, number = 0;
                            int.TryParse(row["HSJJ"].ToString(), out price);
                            int.TryParse(row["NUM_ZHONGBZ"].ToString(), out number);
                            row["HSJJ"] = price * number;
                        }
                    }
                }
            }
            PubFunc.GridRowAdd(GridGoods, row, firstRow);
        }
        #endregion

        protected void btnClosePostBack_Click(object sender, EventArgs e)
        {
            foreach (GridRow row in GridLot.Rows)
            {
                int rowIndex = row.RowIndex;
                System.Web.UI.WebControls.TextBox tbxNumber = (System.Web.UI.WebControls.TextBox)GridLot.Rows[rowIndex].FindControl("tbxNumber");
                if (!string.IsNullOrWhiteSpace(tbxNumber.Text) && tbxNumber.Text != "0")
                {
                    List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().OrderBy(x => x["GDSEQ"]).ToList();
                    int sameRowCount = newDict.Where(a => a["GDSEQ"].ToString() == row.DataKeys[0].ToString() && a["PH"].ToString() == row.DataKeys[1].ToString()).Count();
                    if (sameRowCount > 0)
                    {
                        Alert.Show("商品编码为【" + row.DataKeys[0].ToString() + "】的商品已经存在，无需重复填加！", MessageBoxIcon.Warning);
                        return;
                    }

                    string sql = @"SELECT A.ROWNO,A.GDSEQ,A.GDNAME,A.GDSPEC,-ABS({3}) BZSL,A.BZHL,A.HWID,A.PH,A.YXQZ,A.RQ_SC,A.JXTAX,A.HSJJ,-ABS(A.HSJJ*ABS({3})) HSJE,A.PZWH,
                                    B.UNIT,A.SUPID,A.PRODUCER,A.ISGZ,A.ISLOT,
                                    F_GETUNITNAME(B.UNIT) UNITNAME,
                                    -ABS({3}) THSL,
                                    F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME,
                                    F_GETSUPNAME(A.SUPID) SUPNAME,
                                     SUM(A.SSSL - NVL(ABS(D.NUM1),0)) KCSL,
                                    0 SL,A.NUM1, D.SEQNO NUM2,C.KC,
                                    DECODE(NVL(A.NUM1, 0), 0, '非赠品', '赠品') NUM1NAME
                                FROM DAT_RK_COM A,DOC_GOODS B,(SELECT A.GDSEQ,A.DEPTID, A.PH,NVL(A.NUM1,0) NUM1,B.LOCKSL,(B.KCSL - B.LOCKSL) KC
                                                  FROM DAT_GOODSJXC A,DAT_GOODSSTOCK B
                                                  WHERE A.STR2 = '{0}'
                                                  AND A.DEPTID = '{1}'
                                                  AND A.GDSEQ=B.GDSEQ
                                                  AND A.DEPTID=B.DEPTID
                                                  AND A.BILLNO=B.BILLNO
                                                  --AND KCSL > LOCKSL
                                                  GROUP BY A.GDSEQ, A.PH,A.DEPTID,B.LOCKSL,A.NUM1,B.KCSL) C,DAT_GOODSJXC D
                                WHERE A.GDSEQ = B.GDSEQ AND A.GDSEQ = C.GDSEQ AND A.PH = C.PH AND A.SEQNO = '{0}' AND A.ROWNO = {2}  AND A.GDSEQ=D.GDSEQ
   AND A.SEQNO=D.BILLNO AND A.PH=D.PH AND A.ROWNO=D.ROWNO  GROUP BY A.ROWNO,A.GDSEQ, A.GDNAME,A.GDSPEC, A.BZHL,A.HWID,A.PH,A.YXQZ,A.RQ_SC,A.JXTAX,
       A.HSJJ, A.PZWH,B.UNIT,A.SUPID,A.PRODUCER,A.ISGZ,A.ISLOT, A.NUM1,D.SEQNO,C.KC";
                    DataTable dtBill = DbHelperOra.Query(string.Format(sql, tgbRKDH.Text.Trim(), docDEPTID.SelectedValue, row.DataKeys[2], tbxNumber.Text ?? "0")).Tables[0];
                    if (dtBill != null && dtBill.Rows.Count > 0)
                    {
                        foreach (DataRow irow in dtBill.Rows)
                        {
                            PubFunc.GridRowAdd(GridGoods, irow, false);
                        }
                    }
                }
            }

            WindowLot.Hidden = true;
            //更新显示的汇总信息
            UpdateSum(GridGoods.GetNewAddedList());
        }
        /// <summary>
        /// 更新汇总信息 20150510 liuz  解决带出信息更新汇总信息显示
        /// </summary>
        /// <param name="newDict"></param>
        private void UpdateSum(List<Dictionary<string, object>> newDict)
        {
            //计算合计数量
            decimal bzslTotal = 0, feeTotal = 0, thslTotal = 0;
            foreach (Dictionary<string, object> dic in newDict)
            {
                bzslTotal += Convert.ToDecimal(dic["BZSL"]);
                feeTotal += Convert.ToDecimal(dic["HSJJ"]) * Convert.ToDecimal(dic["BZSL"] ?? "0");
                thslTotal += Convert.ToDecimal(dic["BZHL"] ?? "0") * Convert.ToDecimal(dic["BZSL"] ?? "0");
            }
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("BZSL", bzslTotal.ToString());
            summary.Add("HSJE", feeTotal.ToString("F2"));
            summary.Add("THSL", thslTotal.ToString("F2"));

            GridGoods.SummaryData = summary;
        }
        protected override void billDel()
        {
            if (docBILLNO.Text.Trim() == "")
            {
                Alert.Show("请选择需要删除的单据", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            if (docFLAG.SelectedValue != "M" && docFLAG.SelectedValue != "R")
            {
                Alert.Show("非『新增』单据不能删除！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            if (!CheckFlag("M") && !CheckFlag("R"))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            List<string> listSql = new List<string>();
            listSql.Add("Delete from DAT_TH_DOC t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            listSql.Add("Delete from DAT_TH_COM t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            listSql.Add("Delete from DAT_TH_EXT t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");

            if (DbHelperOra.ExecuteSqlTran(listSql))
            {
                Alert.Show("单据删除成功!", "消息提示", MessageBoxIcon.Information);
                OperLog("商品退货", "删除单据【" + docBILLNO.Text + "】");
                PubFunc.FormDataClear(FormDoc);
                PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            }
            else
            {
                Alert.Show("单据删除失败!", "错误提示", MessageBoxIcon.Information);
            }
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            WindowLot.Hidden = true;
        }
        protected override void billDelRow()
        {
            if (docFLAG.SelectedValue != "M" && docFLAG.SelectedValue != "R")
            {
                Alert.Show("非『新增』单据不能删行！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            if (GridGoods.SelectedCell == null)
            {
                Alert.Show("未选择任何行，无法进行【删行】操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!CheckFlag("M") && !CheckFlag("R"))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            GridGoods.DeleteSelectedRows();
            //GridGoods.DeleteRow(GridGoods.SelectedRowID);
           
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
            UpdateSum(newDict);
        }
        // 根据行ID来删除行数据
        
        protected void btnScan_Click(object sender, EventArgs e)
        {
            if (PubFunc.StrIsEmpty(docSEQNO.Text))
            {
                Alert.Show("请保存单据后进行扫描追溯码操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_TH_COM A,DOC_GOODS B WHERE A.GDSEQ = B.GDSEQ AND B.ISGZ = 'Y' AND SEQNO = '{0}'", docSEQNO.Text)))
            {
                Alert.Show("此单据中没有已经保存的高值商品,请检查！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (docFLAG.SelectedValue != "M" && docFLAG.SelectedValue != "R")
            {
                zsmScan.Enabled = false;
                zsmDelete.Enabled = false;
            }
            else
            {
                zsmScan.Enabled = true;
                zsmDelete.Enabled = true;
            }
            WindowScan.Hidden = false;
            ScanSearch("SHOW");
        }
        protected void ScanSearch(string type)
        {
            string sql = "";
            if (type == "SHOW")
            {
                sql = "SELECT A.*,f_getunitname(A.UNIT) UNITNAME FROM DAT_TH_EXT A WHERE A.BILLNO = '{0}' ORDER BY A.GDSEQ,A.INSTIME DESC";
            }
            else
            {
                sql = "SELECT A.*,f_getunitname(A.UNIT) UNITNAME FROM DAT_TH_EXT A WHERE A.BILLNO = '{0}' ORDER BY A.INSTIME DESC";
            }
            DataTable dtScan = DbHelperOra.Query(string.Format(sql, docSEQNO.Text)).Tables[0];
            //PubFunc.GridRowAdd(GridSacn, dtScan);
            GridSacn.DataSource = dtScan;
            GridSacn.DataBind();
            zsmScan.Text = String.Empty;
            zsmScan.Focus();
        }
        protected void btnScanClose_Click(object sender, EventArgs e)
        {
            //List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
            //List<Dictionary<string, object>> newSacn = GridSacn.GetNewAddedList();
            //int rowNo = 0;
            //string onecode = "";
            //bool err = true;
            //foreach (Dictionary<string, object> dic in newDict)
            //{
            //    err = true;
            //    foreach (Dictionary<string, object> dic_Scan in newSacn)
            //    {
            //        if (dic_Scan["LISONECODE"].ToString() == "")
            //        {
            //            Alert.Show("追溯码未扫描完全,请检查!");
            //            lisonecode.Focus();
            //            return;
            //        }
            //        if (dic_Scan["LISGDSEQ"].ToString() == dic["GDSEQ"].ToString())
            //        {
            //            onecode = onecode + "'" + dic_Scan["LISONECODE"].ToString() + "',";
            //            err = false;
            //        }
            //    }
            //    if (!err)
            //    {
            //        dic["STR1"] = onecode.TrimEnd(',');
            //        PageContext.RegisterStartupScript(GridGoods.GetDeleteRowReference(rowNo) + GridGoods.GetAddNewRecordReference(GetJObject(dic), rowNo));
            //    }
            //    rowNo++;
            //}
            //WindowScan.Hidden = true;
        }
        /// <summary>
        /// 扫码改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void zsmScan_TextChanged(object sender, EventArgs e)
        {
            if (zsmScan.Text.Length < 28) return;
            if (docFLAG.SelectedValue != "M" && docFLAG.SelectedValue != "R")
            {
                Alert.Show("非『新增』单据不允许操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            //判断商品是否入库
            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_GZ_EXT WHERE ONECODE = '{0}' AND FLAG IN ('Y','R') AND DEPTCUR = '{1}' AND BILLNO = '{2}'", zsmScan.Text, docDEPTID.SelectedValue, tgbRKDH.Text)))
            {
                Alert.Show("高值商品条码状态不正确或不是此入库单入库条码，请检查！", "消息提示", MessageBoxIcon.Warning);
                zsmScan.Text = string.Empty;
                zsmScan.Focus();
                return;
            }

            if (DbHelperOra.Exists(string.Format("select 1 from dat_th_ext where onecode = '{0}'", zsmScan.Text)))
            {
                Alert.Show("该追溯码已经使用，请更换其他追溯码");
                return;
            }
            //写入数据库中
            DbHelperOra.ExecuteSql(string.Format(@"INSERT INTO DAT_TH_EXT(DEPTID,BILLNO,ROWNO,ONECODE,GDSEQ,GDNAME,BARCODE,UNIT,GDSPEC,DEPTCUR,BZHL,INSTIME,PH,RQ_SC,YXQZ)
                    SELECT '{0}','{1}',NVL((SELECT MAX(ROWNO)+1 FROM DAT_TH_EXT WHERE BILLNO = '{1}'),1),'{2}',GDSEQ,GDNAME,BARCODE,UNIT,GDSPEC,'{0}',BZHL,SYSDATE,PH,RQ_SC,YXQZ
                    FROM DAT_RK_EXT A
                    WHERE A.ONECODE = '{2}' AND ROWNUM=1 ", docDEPTID.SelectedValue, docBILLNO.Text, zsmScan.Text));
            ScanSearch("");
        }
        protected void zsmDelete_Click(object sender, EventArgs e)
        {
            if (docFLAG.SelectedValue != "M" && docFLAG.SelectedValue != "R")
            {
                Alert.Show("非『新增』单据不允许操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!CheckFlag("M") && !CheckFlag("R"))
            {
                Alert.Show("此单据已经被其他人操作，不能删除！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (GridSacn.SelectedCell == null)
            {
                Alert.Show("请选择您需要删除的数据！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if ((Convert.ToInt16(GridSacn.SelectedRowIndex)) < 0)
            {
                Alert.Show("请选择您需要删除的数据！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string onecode = (GridSacn.DataKeys[GridSacn.SelectedRowIndex][0]).ToString();
            DbHelperOra.ExecuteSql(string.Format("DELETE FROM DAT_TH_EXT WHERE ONECODE = '{0}'", onecode));
            OperLog("商品退货", "修改单据【" + docBILLNO.Text + "】高值码");
            ScanSearch("");
        }

        private bool CheckFlag(string flag)
        {
            if (docBILLNO.Text.Length > 0)
            {
                return Doc.getFlag(docBILLNO.Text, flag, BillType);
            }
            return true;
        }
        /// <summary>
        /// 20150510   liuz  增加提交操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCommit_Click(object sender, EventArgs e)
        {

            string strBILLNO = docBILLNO.Text;
            List<CommandInfo> sqlList = new List<CommandInfo>();
            if (strBILLNO.Length == 0)
            {
                Alert.Show("请先保存后，再次提交！");
                return;
            }
            if (docFLAG.SelectedValue != "M")
            {
                Alert.Show("只有保存后单据，才能提交！");
                return;
            }
            if (!Doc.getFlag(strBILLNO, "M", BillType))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                return;
            }

            //提交的时候增加高值商品的扫码
            Object exis = DbHelperOra.GetSingle(string.Format(@"SELECT GDSEQ 
                FROM (SELECT A.GDSEQ,A.PH,SUM(abs(A.THSL)) SL
                FROM DAT_TH_COM A,DOC_GOODS B
                WHERE A.GDSEQ = B.GDSEQ AND B.ISGZ IN ('Y','1') AND A.SEQNO = '{0}'
                GROUP BY A.GDSEQ,A.PH)
                WHERE (GDSEQ,PH,SL) NOT IN(SELECT GDSEQ,PH,COUNT(1) SL FROM DAT_TH_EXT WHERE BILLNO = '{0}' GROUP BY GDSEQ,PH) AND ROWNUM = 1", strBILLNO));
            if (!PubFunc.StrIsEmpty((exis ?? "").ToString()))
            {
                Alert.Show("提交失败，请检查商品【" + exis + "】高值码扫描的追溯码与退货数和批号是否不一致！");
                return;
            }
            //List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
            //for (int i = 0; i < newDict.Count; i++)
            //{
            //    //增加贵重物品验证   && newDict[i]["STR1"].ToString() == ""
            //    if (DbHelperOra.Exists("select 1 from doc_goods where GDSEQ = '" + newDict[i]["GDSEQ"] + "' and ISGZ in('Y','1')") && !DbHelperOra.Exists("select 1 from DAT_TH_EXT where billno ='" + strBILLNO + "' and GDSEQ = '" + newDict[i]["GDSEQ"] + "'"))
            //    {
            //        Alert.Show("单据中存在贵重物品,请进行追溯码扫描!");
            //        return;
            //    }
            //}
            SaveSuccess = false;
            save("Y");
            if (SaveSuccess == false)
                return;
            SaveSuccess = false;
            sqlList.Add(new CommandInfo("update DAT_TH_DOC set flag='N' where BILLNO='" + strBILLNO + "' ", null));


            if (DbHelperOra.ExecuteSqlTran(sqlList))
            {
                Alert.Show("此单据提交成功！");
                billSearch();
                billOpen(docBILLNO.Text);
                billLockDoc(true);
                OperLog("商品退货", "提交单据【" + strBILLNO + "】");
            }
            else
            {
                Alert.Show("此单据提交失败，请联系系统管理人员！");
            }

        }
        protected override void billCancel()
        {
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("只能驳回已提交的单据！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string strBillno = docSEQNO.Text;
            if (!Doc.getFlag(strBillno, "N", BillType))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                return;
            }
            WindowReject.Hidden = false;
        }
        protected void btnRejectSubmit_Click(object sender, EventArgs e)
        {

            if (!string.IsNullOrWhiteSpace(docBILLNO.Text.Trim()))
            {
                string flag = DbHelperOra.GetSingle("SELECT FLAG FROM DAT_TH_DOC WHERE BILLNO ='" + docBILLNO.Text.Trim() + "'").ToString();

                if ((",N").IndexOf(flag) < 0)
                {
                    Alert.Show("只能驳回已提交的单据！", "消息提示", MessageBoxIcon.Warning);
                    return;
                }
            }


            List<CommandInfo> cmdList = new List<CommandInfo>();


            if (docBILLNO.Text.Length < 1)
            { return; }

            if (string.IsNullOrWhiteSpace(ddlReject.SelectedValue))
            {
                Alert.Show("请选择驳回原因", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (txaMemo.Text.Length > 140)
            {
                Alert.Show("驳回原因超长！", MessageBoxIcon.Warning);
                return;
            }
            string strMemo = "驳回原因:" + ddlReject.SelectedText;
            if (!string.IsNullOrWhiteSpace(txaMemo.Text.Trim()))
            {
                strMemo += ";详细说明:" + txaMemo.Text;
            }
            if (DbHelperOra.ExecuteSql(string.Format("update DAT_TH_DOC set flag='R',memo='{0}' where seqno='{1}' and flag='N'", strMemo, docBILLNO.Text)) == 1)
            {
                WindowReject.Hidden = true;
                billOpen(docBILLNO.Text);
                OperLog("商品退货", "驳回单据【" + docBILLNO.Text + "】，" + strMemo);
                docFLAG.SelectedValue = "R";
            };
            //cmdList.Add(new CommandInfo("delete DAT_TH_EXT where BILLNO='" + docBILLNO.Text.Trim() + "'", null));
            DbHelperOra.ExecuteSqlTran(cmdList);
        }
        protected void btnAllCommit_Click(object sender, EventArgs e)
        {
            List<CommandInfo> sqlList = new List<CommandInfo>();
            string succeed = string.Empty;
            for (int i = 0; i < GridList.SelectedRowIndexArray.Length; i++)
            {
                int rowIndex = GridList.SelectedRowIndexArray[i];
                if (GridList.DataKeys[rowIndex][1].ToString() == "M")
                {
                    string strBILLNO = GridList.DataKeys[rowIndex][0].ToString();
                    sqlList.Add(new CommandInfo("update DAT_TH_DOC set flag='N' where BILLNO='" + strBILLNO + "' ", null));
                    if (DbHelperOra.Exists(string.Format(@"SELECT 1 
                                    FROM (SELECT A.GDSEQ,A.PH,SUM(A.THSL) SL
                                    FROM DAT_TH_COM A,DOC_GOODS B
                                    WHERE A.GDSEQ = B.GDSEQ AND B.ISGZ IN ('Y','1') AND A.SEQNO = '{0}'
                                    GROUP BY A.GDSEQ,A.PH)
                                    WHERE (GDSEQ,PH,SL) NOT IN(SELECT GDSEQ,PH,COUNT(1) SL FROM DAT_TH_EXT WHERE BILLNO = '{0}' GROUP BY GDSEQ,PH)", strBILLNO)))
                    {
                        Alert.Show("单据【" + strBILLNO + "】高值码扫描不正确，请检查！");
                        return;
                    }

                    if (DbHelperOra.ExecuteSqlTran(sqlList))
                    {
                        succeed = succeed + "【" + strBILLNO + "】";
                    }
                }
            }
            if (succeed.Length > 0)
            {
                Alert.Show("单据" + succeed + "提交成功！");
                billSearch();
                OperLog("商品退货", "提交单据" + succeed + "");
            }
            else
            {
                Alert.Show("你选中的单据中，没有要提交的单据");
            }


        }

        protected void GridList_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;

            if (row != null)
            {
                string flag = row["FLAG"].ToString();
                FineUIPro.BoundField flagcol = GridList.FindColumn("FLAG_CN") as FineUIPro.BoundField;
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
        protected void GridList_Sort(object sender, GridSortEventArgs e)
        {
            highlightRowYellow.Text = "";
            highlightRows.Text = "";
            GridList.SortDirection = e.SortDirection;
            GridList.SortField = e.SortField;

            DataTable table = PubFunc.GridDataGet(GridList);
            DataView view1 = table.DefaultView;
            view1.Sort = String.Format("{0} {1}", GridList.SortField, GridList.SortDirection);
            GridList.DataSource = view1;
            GridList.DataBind();

        }

        protected void PageManager1_CustomEvent(object sender, CustomEventArgs e)
        {
            if (e.EventArgument.IndexOf("GoodsAdd") >= 0)
            {
                Window1_Close(null, null);
            }
        }

        protected void tgbRKDH_TextChanged(object sender, EventArgs e)
        {
            if (tgbRKDH.Text.Trim().Length >= 11)
            {
                //对代管和非代管进行判断
                DataTable dtDoc = DbHelperOra.Query(string.Format("SELECT A.PSSID,A.SEQNO RKDH,A.DEPTID,B.PHID,A.FLAG AFLAG,A.STR2 FROM DAT_RK_DOC A,DAT_RK_COM B WHERE A.SEQNO=B.SEQNO AND A.SEQNO ='{0}'", tgbRKDH.Text.Trim())).Tables[0];
                if (dtDoc != null && dtDoc.Rows.Count > 0)
                {
                    PubFunc.FormDataSet(FormDoc, dtDoc.Rows[0]);
                    PubFunc.FormLock(FormDoc, true, "");
                    docTHTYPE.Enabled = true;
                    docMEMO.Enabled = true;

                    if (string.Compare(dtDoc.Rows[0][4].ToString(), "Y") == -1)
                    {
                        Alert.Show("入库单单据状态不正确！", "异常提醒", MessageBoxIcon.Warning);
                        return;
                    }
                    if (string.Compare(dtDoc.Rows[0][5].ToString(), "Y") != -1)
                    {
                        Alert.Show("由初始库存生成的入库单无法退货！", "异常提醒", MessageBoxIcon.Warning);
                        return;
                    }

                    PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
                    string sql = @"SELECT A.ROWNO,A.GDSEQ,A.GDNAME,A.GDSPEC,-ABS(BZSL)*A.BZHL BZSL,C.BZHL,A.HWID,A.PH,A.YXQZ,A.RQ_SC,A.JXTAX,A.HSJJ/A.BZHL HSJJ,-ABS(A.HSJE) HSJE,A.PZWH,
                                    C.UNIT,A.PRODUCER,A.ISGZ,A.ISLOT,
                                    F_GETUNITNAME(C.UNIT) UNITNAME,
                                    -ABS(SSSL) THSL,
                                    F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME,
                                    F_GETSUPNAME(A.SUPID) SUPNAME,
                                    --NVL(({0}),0) KCSL,
                                    --D.KCSL,
                                    SUM(A.SSSL-ABS(D.NUM1)) KCSL,
                                    E.SEQNO NUM2,
                                    D.KC,
                                    F_GETUNITNAME(C.UNIT) UNITSMALLNAME,A.NUM1,
                                    DECODE(NVL(A.NUM1, 0), 0, '非赠品', '赠品') NUM1NAME
                                FROM DAT_RK_COM A,
                                    (SELECT G.GDSEQ, G.TYPE
                                        FROM DOC_GOODSSUP G
                                        WHERE NVL(G.PSSID, G.SUPID) =
                                            (SELECT D.PSSID FROM DAT_RK_DOC D WHERE D.SEQNO = '{0}')) B,
                                    DOC_GOODS C,(SELECT A.GDSEQ,A.DEPTID, A.PH,NVL(A.NUM1,0) NUM1,B.LOCKSL,(B.KCSL - B.LOCKSL) KC
                                                  FROM DAT_GOODSJXC A,DAT_GOODSSTOCK B
                                                  WHERE A.STR2 = '{0}'
                                                  AND A.DEPTID = '{1}'
                                                  AND A.GDSEQ=B.GDSEQ
                                                  AND A.PH=B.PH
                                                  AND A.DEPTID=B.DEPTID
                                                  AND A.BILLNO=B.BILLNO
                                                  --AND KCSL > LOCKSL
                                                  GROUP BY A.GDSEQ, A.PH,A.DEPTID,B.LOCKSL,A.NUM1,B.KCSL) D,
                                    DAT_GOODSJXC E
                                WHERE A.GDSEQ = C.GDSEQ
                                AND C.GDSEQ = B.GDSEQ
                                AND A.GDSEQ = D.GDSEQ 
                                AND A.GDSEQ=E.GDSEQ
                                AND A.ROWNO=E.ROWNO
                                AND A.PH=D.PH
                                AND A.SEQNO = '{0}'
                                AND D.DEPTID = '{1}'
                                AND E.BILLNO='{0}'";
                    if (isDg())
                    {
                        sql += " AND B.TYPE = '0'";
                    }
                    else
                    {
                        sql += " AND B.TYPE <> '1'";
                    }
                    sql += " GROUP BY A.ROWNO,A.GDSEQ,A.GDNAME,A.GDSPEC,A.BZHL,A.HWID,A.PH,A.YXQZ,A.RQ_SC,A.JXTAX,A.HSJJ,A.PZWH, C.BZHL,A.PRODUCER,A.ISGZ, A.ISLOT,C.UNIT,A.SUPID,A.BZSL,A.HSJE,A.SSSL,A.NUM1,E.SEQNO,D.KC";
                    sql += " ORDER BY A.ROWNO";
                    string phSql = string.Empty;
                    //DataTable dtBill = DbHelperOra.Query(string.Format(sql, phSql, tgbRKDH.Text.Trim())).Tables[0];
                    DataTable dtBill = DbHelperOra.Query(string.Format(sql, tgbRKDH.Text.Trim(), docDEPTID.SelectedValue)).Tables[0];
                    string ss = string.Format(sql, tgbRKDH.Text.Trim(), docDEPTID.SelectedValue);
                    string sqlStr = string.Format(sql, docDEPTID.SelectedValue, tgbRKDH.Text.Trim());
                    if (dtBill != null && dtBill.Rows.Count > 0)
                    {
                        //dtBill.Columns.Remove(dtBill.Columns["KCSL"]);
                        //dtBill.Columns["KCSLNOW"].ColumnName = "KCSL";
                        //dtBill.Columns.Remove(dtBill.Columns["BZSL"]);
                        //dtBill.Columns["BZSLTH"].ColumnName = "BZSL";
                        decimal bzslTotal = 0, feeTotal = 0, thslTotal = 0;
                        foreach (DataRow row in dtBill.Rows)
                        {
                            bzslTotal += Convert.ToDecimal(row["BZSL"]);
                            feeTotal += Convert.ToDecimal(row["HSJJ"]) * Convert.ToDecimal(row["BZSL"] ?? "0");
                            thslTotal += Convert.ToDecimal(row["BZHL"] ?? "0") * Convert.ToDecimal(row["BZSL"] ?? "0");
                        }

                        Doc.GridRowAdd(GridGoods, dtBill);
                        JObject summary = new JObject();
                        summary.Add("GDNAME", "本页合计");
                        summary.Add("BZSL", bzslTotal.ToString());
                        summary.Add("HSJE", feeTotal.ToString("F2"));
                        summary.Add("THSL", thslTotal.ToString());
                        GridGoods.SummaryData = summary;
                        //btnGoods.Enabled = false;
                    }
                    else
                    {
                        if (isDg())
                        {
                            Alert.Show("此入库单号为非代管单号,请在【商品退货管理】中操作！", "异常提醒", MessageBoxIcon.Warning);
                            return;
                        }
                        //else
                        //{
                        //    Alert.Show("此入库单号为代管单号,请在【代管退货管理】中操作！", "异常提醒", MessageBoxIcon.Warning);
                        //    return;
                        //}
                    }

                }
                else
                {
                    Alert.Show("您输入的入库单信息不存在！", "异常提醒", MessageBoxIcon.Warning);
                    return;
                }

            }
        }
        public void DataComSearch()
        {
            //对代管和非代管进行判断
            DataTable dtDoc = DbHelperOra.Query(string.Format("SELECT A.PSSID,A.DEPTID FROM DAT_RK_DOC A WHERE A.SEQNO ='{0}'", tgbRKDH.Text.Trim())).Tables[0];
            docPSSID.SelectedValue = dtDoc.Rows[0]["PSSID"].ToString();
            if (dtDoc != null && dtDoc.Rows.Count > 0)
            {
                string sql = @"SELECT A.ROWNO,A.GDSEQ,A.GDNAME,A.GDSPEC,-ABS(BZSL) BZSL,A.BZHL,A.HWID,A.PH,A.YXQZ,A.RQ_SC,A.JXTAX,A.HSJJ,-ABS(A.HSJE) HSJE,A.PZWH,
                                    B.UNIT,A.PRODUCER,A.ISGZ,A.ISLOT,
                                    F_GETUNITNAME(B.UNIT) UNITNAME,
                                    -ABS(SSSL) THSL,
                                    F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME,
                                    F_GETSUPNAME(A.SUPID) SUPNAME,
                                    SUM(A.SSSL-ABS(C.NUM1)) KCSL,--C.KCSL,
                                    0 SL,A.NUM1,D.SEQNO NUM2,C.KC,
                                    DECODE(NVL(A.NUM1, 0), 0, '非赠品', '赠品') NUM1NAME
                                FROM DAT_RK_COM A,DOC_GOODS B,
                                      (SELECT A.GDSEQ,A.PH,NVL(A.NUM1,0)NUM1,B.LOCKSL,(B.KCSL - B.LOCKSL) KC
                                          FROM DAT_GOODSJXC A,DAT_GOODSSTOCK B
                                        WHERE A.STR2 = '{0}' AND A.DEPTID = '{1}' AND A.GDSEQ = B.GDSEQ AND A.DEPTID = B.DEPTID AND A.BILLNO = B.BILLNO
                                       GROUP BY A.GDSEQ,A.PH,A.NUM1，B.LOCKSL,B.KCSL) C,DAT_GOODSJXC D
                                WHERE A.GDSEQ = B.GDSEQ AND A.GDSEQ = C.GDSEQ AND A.GDSEQ=D.GDSEQ AND A.ROWNO=D.ROWNO AND A.PH = C.PH AND A.PH=D.PH AND A.SEQNO = '{0}'AND D.BILLNO='{0}'";
                if (tgblotGoods.Text.Trim().Length > 0)
                {
                    sql += string.Format(" AND (A.GDSEQ  LIKE '%{0}%'OR B.GDNAME LIKE '%{0}%' OR B.ZJM LIKE '%{0}%' OR B.BAR3 LIKE '%{0}%')", tgblotGoods.Text);
                }
                if (tgbPh.Text.Trim().Length > 0)
                {
                    sql += string.Format(" AND A.PH='{0}'", tgbPh.Text);
                }
                sql += " GROUP BY A.ROWNO,A.GDSEQ,A.GDNAME,A.GDSPEC,A.BZHL,A.HWID,A.PH,A.YXQZ,A.RQ_SC,A.JXTAX,A.HSJJ,A.PZWH,B.UNIT,A.PRODUCER,A.ISGZ,A.ISLOT, B.UNIT,A.SUPID,A.BZSL,A.HSJE,A.SSSL,A.NUM1,D.SEQNO,C.KC";
                sql += " ORDER BY A.ROWNO";
                DataTable dtBill = DbHelperOra.Query(string.Format(sql, tgbRKDH.Text.Trim(), docDEPTID.SelectedValue)).Tables[0];
                if (dtBill.Rows.Count < 1)
                {
                    Alert.Show("此单据无可退货信息，请检查！", MessageBoxIcon.Warning);
                    //WindowLot.Hidden = true;
                    return;
                }
                GridLot.DataSource = dtBill;
                GridLot.DataBind();
            }
            else
            {
                Alert.Show("此单据无可退货信息或退货部门错误，请检查！", MessageBoxIcon.Warning);
                WindowLot.Hidden = true;
                return;
            }
        }

        protected void tgbRKDH_TriggerClick(object sender, EventArgs e)
        {
            dbkTime1.SelectedDate = DateTime.Now.AddDays(-1);
            dbkTime2.SelectedDate = DateTime.Now;
            WinBillno.Hidden = false;
            btnSrchBill_Click(null, null);
        }
        protected void btnRkGoods_TriggerClick(object sender, EventArgs e)
        {
            DataComSearch();
            tgblotGoods.Text = "";
            tgbPh.Text = "";
        }
        protected void btnSrchBill_Click(object sender, EventArgs e)
        {
            //不关联商品配置表，可能会有商品信息不存在的单据 by congwm 16/11/17
            //String Sql = @"SELECT DISTINCT A.SEQNO,
            //                                f_getdeptname(A.DEPTID) DEPTNAME,
            //                                f_getsupname(A.PSSID) PSSNAME,
            //                                A.SHRQ
            //                  FROM DAT_RK_DOC A, DAT_RK_COM B, DOC_GOODSCFG C
            //                 WHERE A.SEQNO = B.SEQNO
            //                   AND B.GDSEQ = C.GDSEQ
            //                   AND A.FLAG = 'Y'";
            String Sql = @"SELECT DISTINCT A.SEQNO,
                                            f_getdeptname(A.DEPTID) DEPTNAME,
                                            f_getsupname(A.PSSID) PSSNAME,
                                            A.SHRQ
                              FROM DAT_RK_DOC A
                             WHERE A.FLAG = 'Y' ";//AND A.PSSID<>'00001'";
            if (tgbBillNo.Text.Trim().Length > 0)
            {
                Sql += string.Format(" AND UPPER(A.SEQNO) LIKE '%{0}%'", tgbBillNo.Text.Trim().ToUpper());
            }
            if (tgbGoods.Text.Trim().Length > 0)
            {
                Sql += string.Format(" AND A.SEQNO IN (SELECT SEQNO FROM DAT_RK_COM A, DOC_GOODS B WHERE A.GDSEQ = B.GDSEQ AND (UPPER(A.GDSEQ) LIKE '%{0}%' OR UPPER(A.GDNAME) LIKE '%{0}%' OR UPPER(B.ZJM) LIKE '%{0}%'))", tgbGoods.Text.Trim().ToUpper());
            }
            if (ddlDEPTIN.SelectedItem != null && ddlDEPTIN.SelectedItem.Value.Length > 0)
            {
                Sql += string.Format(" AND A.DEPTID='{0}'", ddlDEPTIN.SelectedItem.Value);
            }
            Sql += string.Format(" AND A.SHRQ>=TO_DATE('{0}','YYYY-MM-DD')", dbkTime1.Text);
            Sql += string.Format(" AND A.SHRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", dbkTime2.Text);
            int total = 0;

            DataTable dtData = PubFunc.DbGetPage(GridBill.PageIndex, GridBill.PageSize, String.Format(Sql), ref total);
            GridBill.RecordCount = total;
            GridBill.DataSource = dtData;
            GridBill.DataBind();
        }

        protected void btnBillSure_Click(object sender, EventArgs e)
        {
            int[] selects = GridBill.SelectedRowIndexArray;
            if (selects.Count() < 1)
            {
                Alert.Show("请选择入库单号！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            String Billno = "";
            foreach (int index in selects)
            {
                Billno += GridBill.DataKeys[index][0] + ",";
            }
            tgbRKDH.Text = Billno.TrimEnd(',');
            WinBillno.Hidden = true;
            tgbRKDH_TextChanged(null, null);
        }

        protected void GridBill_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridBill.PageIndex = e.NewPageIndex;
            btnSrchBill_Click(null, null);
        }

        protected void GridBill_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            String billNo = GridBill.DataKeys[e.RowIndex][0].ToString();
            tgbRKDH.Text = billNo;
            WinBillno.Hidden = true;
            tgbRKDH_TextChanged(null, null);
        }
    }
}