﻿using FineUIPro;
using Newtonsoft.Json.Linq;
using XTBase;
using ERPProject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPProject.ERPInventory
{
    public partial class DecreaseOverflow : BillBase
    {
        private string strDocSql = "SELECT A.* FROM DAT_SY_DOC A WHERE A.SEQNO ='{0}'";
        private string strLisSQL = "SELECT A.*,F_GETUNITNAME(A.UNIT) UNITNAME,F_GETSUPPLIERNAME(A.PSSID) PSSNAME,F_GETSUPNAME(A.SUPID)SUPNAME,G.NAMEJC FROM DAT_SY_COM A,DOC_GOODS G WHERE A.GDSEQ=G.GDSEQ AND SEQNO = '{0}' ORDER BY ROWNO";
        public DecreaseOverflow()
        {
            BillType = "SYD";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
                billNew();
            }
        }

        public override Field[] LockControl
        {
            get { return new Field[] { tbxBILLNO, ddlFLAG, ddlLRY, dpkLRRQ, ddlDEPTID, ddlKCTYPE }; }
        }

        private void DataInit()
        {
            ButtonHidden(btnCopy, btnAddRow, btnCancel);
            PubFunc.DdlDataGet("DDL_USER", ddlLRY, ddlSHR);
            PubFunc.DdlDataGet("DDL_SYS_DEPT", ddlDEPTID);
            PubFunc.DdlDataGet("DDL_BILL_FLAG_MNYR", ddlFLAG, lisFLAG);
            PubFunc.DdlDataGet("DDL_DAT_KCTYPE", ddlKCTYPE);
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
            dpkLRRQ.SelectedDate = DateTime.Now;
            ddlLRY.SelectedValue = UserAction.UserID;
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
            ddlFLAG.SelectedValue = "M";
            ddlLRY.SelectedValue = UserAction.UserID;
            dpkLRRQ.SelectedDate = DateTime.Now;
            ddlDEPTID.Enabled = true;
            ddlKCTYPE.Enabled = true;
            tbxBILLNO.Enabled = true;
            tbxBILLNO.Text = string.Empty;
            tbxMEMO.Enabled = true;
            tbxMEMO.Text = string.Empty;
            GridGoods.SummaryData = null;
            nbxSUBSUM.Text = String.Empty;
            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            //按钮状态
            btnSave.Enabled = true;
            btnDel.Enabled = false;
            btnPrint.Enabled = false;
            btnDelRow.Enabled = true;
            btnTj.Enabled = false;
            btnGoods.Enabled = true;
            //计算合计数量
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            GridGoods.SummaryData = summary;
        }
        protected void GridGoods_AfterEdit(object sender, FineUIPro.GridAfterEditEventArgs e)
        {
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
            if (newDict.Count == 0) return;
            if (e.ColumnID == "PH")
            {

                if (!PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "BZHL")))
                {
                    Alert.Show("请先选择商品信息！", "异常信息", MessageBoxIcon.Warning);
                    return;
                }
                JObject defaultObj = Doc.GetJObject(GridGoods, e.RowID);
                DataTable dtPH = new DataTable();
                dtPH = DbHelperOra.Query(string.Format("SELECT A.* FROM DAT_GOODSSTOCK A WHERE A.DEPTID = '{0}' AND A.GDSEQ ='{1}' AND A.PH = '{2}' AND A.KCSL>0 ", ddlDEPTID.SelectedValue, defaultObj["GDSEQ"].ToString(), defaultObj["PH"].ToString())).Tables[0];

                if (dtPH != null && dtPH.Rows.Count > 0)
                {
                    if (dtPH.Rows.Count == 1)
                    {
                        defaultObj["RQ_SC"] = dtPH.Rows[0]["RQ_SC"].ToString();
                        defaultObj["YXQZ"] = dtPH.Rows[0]["RQ_SC"].ToString();

                        PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(e.RowID, defaultObj));
                        if (e.RowID != GridGoods.SelectedRowID)
                        {
                            PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(e.RowID, defaultObj));
                        }
                    }
                    else
                    {
                        Alert.Show("批次在原系统中不存在,请检查！", "提示信息", MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    defaultObj["RQ_SC"] = null;
                    defaultObj["YXQZ"] = null;
                    //PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(GetJObject(newDict[e.RowIndex]), cell));
                    PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(e.RowID, defaultObj));
                    if (e.RowID == GridGoods.SelectedRowID)
                    {
                        //PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(GetJObject(newDict[intCell[0]]), string.Format("[{0},{1}]", intCell[0], intCell[1])));
                        PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(e.RowID, defaultObj));
                    }
                    else {
                        Alert.Show("请维护批次信息！", "提示信息", MessageBoxIcon.Warning);

                    }
                }
            }
        }
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
            return defaultObj;
        }
        protected override void billDel()
        {
            if (tbxBILLNO.Text.Trim() == "" || tbxBILLNO.Enabled == true)
            {
                Alert.Show("请选择需要删除的单据");
                return;
            }
            if (ddlFLAG.SelectedValue != "M" && ddlFLAG.SelectedValue != "R")
            {
                Alert.Show("非新单不能删除!");
                return;
            }
            DbHelperOra.ExecuteSql("Delete from dat_sy_com t WHERE T.SEQNO ='" + tbxBILLNO.Text.Trim() + "'");
            DbHelperOra.ExecuteSql("Delete from dat_sy_doc t WHERE T.SEQNO ='" + tbxBILLNO.Text.Trim() + "'");
            DbHelperOra.ExecuteSql("Delete from dat_sy_ext t WHERE T.BILLNO ='" + tbxBILLNO.Text.Trim() + "'");
            Alert.Show("单据删除成功!");
            billNew();
            billSearch();
            OperLog("商品损益", "删除单据【" + tbxBILLNO.Text + "】");
        }
        protected override void billDelRow()
        {
            if (ddlFLAG.SelectedValue != "M" && ddlFLAG.SelectedValue != "R")
            {
                Alert.Show("非新增单据不能删除！");
                return;
            }
            if (GridGoods.SelectedRowID.Length < 0) return;
            // string RowID = GridGoods.SelectedRowID;
            //GridGoods.DeleteRow(RowID);
            GridGoods.DeleteSelectedRows();
            //  int rowIndex = GridGoods.SelectedRowIndex;
            // PageContext.RegisterStartupScript(GridGoods.DeleteRow(rowIndex));
        }
        protected override void billClear()
        {
            PubFunc.FormDataClear(Formlist);
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
        }
        protected override void billSearch()
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【损益日期】！");
                return;
            }
            string strSql = @"select a.*,F_GETUSERNAME(LRY) LRYNAME,F_GETUSERNAME(SHR) SHRNAME,F_GETDEPTNAME(DEPTID) DEPTIDNAME,F_GETSYTYPENAME(SYTYPE) SYTYPENAME,decode(FLAG,'M','新单','N','已提交','Y','已审核','R','已驳回','已执行') FLAGNAME,F_GETSYTYPENAME(SYTYPE) SYTYPENAME FROM DAT_SY_DOC A where 1=1 ";
            string strSearch = "";
            if (lstBILLNO.Text.Length > 0)
            { strSearch += string.Format(" AND SEQNO like '%{0}%'", lstBILLNO.Text); }
            if (lisFLAG.SelectedValue.Length > 0)
            { strSearch += string.Format(" AND FLAG = '{0}'", lisFLAG.SelectedValue); }
            strSearch += string.Format(" AND (LRRQ between TO_DATE('{0}','YYYY-MM-DD') and (TO_DATE('{1}','YYYY-MM-DD')) + 1)", lstLRRQ1.Text, lstLRRQ2.Text);
            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY DECODE(FLAG,'M','1','N','2','R','3','4'),LRRQ DESC";
            DataTable dtBill = new DataTable();
            dtBill = DbHelperOra.Query(strSql).Tables[0];
            GridList.DataSource = dtBill;
            GridList.DataBind();
        }
        protected override void billCancel()
        {
            if (ddlFLAG.SelectedValue != "Y")
            {
                Alert.Show("非盘点完成单据不能驳回！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            WindowReject.Hidden = false;
        }
        protected void btnRejectSubmit_Click(object sender, EventArgs e)
        {
        }
        protected override void billAudit()
        {
            if (tbxBILLNO.Text.Length < 1 || ddlFLAG.SelectedValue == "S")
            {
                Alert.Show("请选择需要审核的单据!");
                return;
            }
            if (ddlFLAG.SelectedValue == "N")
            {

                if (BillOper(tbxBILLNO.Text, "AUDIT") == 1)
                {
                    billLockDoc(true);
                    Alert.Show("单据【" + tbxBILLNO.Text + "】审核成功！");
                    OperLog("商品损益", "审核单据【" + tbxBILLNO.Text + "】");
                    billOpen(tbxBILLNO.Text);
                }
            }
        }
        protected override void listRow_DoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            //dpkPDRQ.Enabled = false;
            tbxBILLNO.Enabled = false;
            billOpen(GridList.Rows[e.RowIndex].Values[1].ToString());
        }

        protected override void billOpen(string strBillno)
        {
            //表头进行赋值
            DataTable dtDoc = DbHelperOra.Query(string.Format(strDocSql, strBillno)).Tables[0];
            PubFunc.FormDataSet(FormDoc, dtDoc.Rows[0]);
            //表体赋值
            DataTable dtBill = DbHelperOra.Query(string.Format(strLisSQL, strBillno)).Tables[0];
            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            decimal bzslTotal = 0, feeTotal = 0, ddslTotal = 0, je1 = 0, je2 = 0, je3 = 0;
            if (dtBill != null && dtBill.Rows.Count > 0)
            {
                foreach (DataRow row in dtBill.Rows)
                {
                    //PubFunc.GridRowAdd(GridGoods, row, false);
                    row["KCSL"] = row["KCSL"].ToString() == "" ? "0" : row["KCSL"];
                    row["KCHSJE"] = row["KCHSJE"].ToString() == "" ? "0" : row["KCHSJE"];
                    row["LSJE"] = row["LSJE"].ToString() == "" ? "0" : row["LSJE"];
                    ddslTotal += Convert.ToDecimal((row["KCSL"].ToString()));
                    bzslTotal += Convert.ToDecimal(row["SYSL"]);
                    feeTotal += Convert.ToDecimal(row["KCHSJE"]);
                    je1 += Convert.ToDecimal(row["HSJE"]);
                    je2 += Convert.ToDecimal(row["BHSJE"]);
                    je3 += Convert.ToDecimal(row["LSJE"]);
                }
                /*
                *  修 改 人 ：袁鹏    修改时间：2015-04-24
                *  研发组织：威高讯通信息科技有限公司
                */
                Doc.GridRowAdd(GridGoods, dtBill);
            }
            PubFunc.FormLock(FormDoc, true, "");
            TabStrip1.ActiveTabIndex = 1;
            //判断单据状态
            if (ddlFLAG.SelectedValue == "M" || ddlFLAG.SelectedValue == "R")
            {
                //按钮状态
                btnSave.Enabled = true;
                btnDel.Enabled = true;
                btnPrint.Enabled = false;
                btnDelRow.Enabled = true;
                btnTj.Enabled = true;
                btnGoods.Enabled = true;
                tbxMEMO.Enabled = true;
            }
            else if (ddlFLAG.SelectedValue == "N")
            {
                btnSave.Enabled = false;
                btnDel.Enabled = false;
                btnPrint.Enabled = false;
                btnDelRow.Enabled = false;
                btnTj.Enabled = false;
                btnGoods.Enabled = false;
                btnAudit.Enabled = true;
                btnCancel.Enabled = true;
            }
            else if (ddlFLAG.SelectedValue.Equals("Y"))
            {
                btnDel.Enabled = false;
                btnDelRow.Enabled = false;
                btnSave.Enabled = false;
                btnTj.Enabled = false;
                btnGoods.Enabled = false;
                btnPrint.Enabled = true;
            }
            else
            {
                btnPrint.Enabled = true;
                btnAudit.Enabled = false;
                btnCancel.Enabled = false;
            }
            //增加合计
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("KCSL", ddslTotal.ToString());
            summary.Add("SYSL", bzslTotal.ToString());
            summary.Add("KCHSJE", feeTotal.ToString("F2"));
            summary.Add("HSJE", je1.ToString("F2"));
            summary.Add("BHSJE", je2.ToString("F2"));
            summary.Add("LSJE", je3.ToString("F2"));
            GridGoods.SummaryData = summary;
        }
        protected override void billGoods()
        {
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
            billLockDoc(true);
            tbxMEMO.Enabled = true;
            //绑定库存,区分代管和非代管的供应商
            string strStrch = "";
            if (!PubFunc.StrIsEmpty(tgbStrch.Text))
            {
                strStrch = string.Format(" AND (B.GDSEQ LIKE '%{0}%' OR B.GDNAME LIKE '%{0}%' OR B.ZJM LIKE '%{0}%' OR B.BARCODE LIKE '%{0}%' OR B.HISCODE LIKE '%{0}%' OR B.HISNAME LIKE '%{0}%' OR B.STR4 LIKE '%{0}%')", tgbStrch.Text);
            }
            /*不管损益类别，都默认选择有库存商品，因此将此段判断注释掉 by congwm 2016/10/18
            *if (ddlKCTYPE.SelectedValue == "A")
            {
                CbxShowAll.Checked = true;
            }*/
            if (CbxShowAll.Checked)
            {
                strStrch += " ";
            }
            else
            {
                strStrch += " AND A.KCSL>0 AND A.KCSL-A.LOCKSL>0 ";
            }
            //strStrch += " GROUP BY A.GDSEQ,PH,A.UNIT,A.PH,A.RQ_SC,A.YXQZ,B.GDNAME,B.PIZNO,A.PSSID,A.SUPID ORDER BY PH DESC";
            strStrch += " GROUP BY B.GDSEQ,B.GDNAME,B.GDSPEC,B.PIZNO,NVL(A.UNIT, B.UNIT),B.PRODUCER ORDER BY GDSEQ DESC";

            string strSql = "";
            #region
            //if (isDg())
            //{
            //    strSql = @"SELECT A.*,B.GDNAME,b.PIZNO PZWH,F_GETUNITNAME(A.UNIT) UNITNAME,0 SL,DECODE(A.SUPID,'00002','非代管','代管') TYPENAME 
            //                FROM DAT_GOODSSTOCK A, DOC_GOODS B 
            //                WHERE A.GDSEQ = B.GDSEQ AND A.DEPTID = '" + ddlDEPTID.SelectedValue + "' ";
            //}
            //else
            //{
            #endregion
//            strSql = @"SELECT A.GDSEQ,A.UNIT,
//                                   SUM(A.KCSL-A.LOCKSL) KCSL,
//                                   A.PH,A.RQ_SC,A.YXQZ,
//                                   B.GDNAME,
//                                   B.PIZNO PZWH,
//                                   F_GETUNITNAME(A.UNIT) UNITNAME,
//                                   F_GETSUPNAME(A.PSSID) PSSNAME,
//                                   F_GETSUPNAME(A.SUPID) SUPNAME,
//                                   0 SL,
//                                   A.PSSID,
//                                   A.SUPID,
//                                   (CASE (SELECT DISTINCT S.TYPE
//                                        FROM DOC_GOODSSUP S
//                                       WHERE S.SUPID = A.SUPID
//                                         AND S.GDSEQ = A.GDSEQ AND FLAG='Y' 
//                                        )
//                                     WHEN '0' THEN
//                                      '托管'
//                                     WHEN '1' THEN
//                                      '代管'
//                                     WHEN 'Z' THEN
//                                      '直供'
//                                     ELSE
//                                      (CASE A.PSSID
//                                        WHEN '00001' THEN
//                                         '代管'
//                                        ELSE
//                                         '未知模式'
//                                      END)
//                                    END) TYPENAME
//                              FROM DAT_GOODSSTOCK A, DOC_GOODS B
//                             WHERE A.GDSEQ = B.GDSEQ   AND EXISTS(SELECT 1 FROM DOC_GOODSCFG WHERE DEPTID='" + ddlDEPTID.SelectedValue + "' AND GDSEQ=B.GDSEQ) AND A.DEPTID = '" + ddlDEPTID.SelectedValue + "' ";
//            //}
            //增加分页
            strSql = @"SELECT B.GDSEQ,
                                           NVL(A.UNIT, B.UNIT) UNIT,
                                           NVL(SUM(A.KCSL - A.LOCKSL), 0) KCSL,
                                           B.GDNAME,B.GDSPEC,
                                           B.PIZNO PZWH,
                                           F_GETUNITNAME(NVL(A.UNIT, B.UNIT)) UNITNAME,
                                           0 SL,
                                           B.PRODUCER,
                                           F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME
                                      FROM (SELECT *
                                              FROM DAT_GOODSSTOCK
                                             WHERE DEPTID = '" + ddlDEPTID.SelectedValue + @"') A,
                                           (SELECT G.GDSEQ,
                                                   G.GDNAME,
                                                   G.GDSPEC,
                                                   G.PIZNO,
                                                   G.PRODUCER,
                                                   GF.DEPTID,
                                                   G.UNIT,
                                                   G.STR4,
                                                   G.HISNAME,
                                                   G.HISCODE,
                                                   G.BARCODE,
                                                   G.ZJM
                                              FROM DOC_GOODS G, DOC_GOODSCFG GF
                                             WHERE G.GDSEQ = GF.GDSEQ
                                               AND G.ISGZ = 'N'
                                               AND GF.DEPTID = '" + ddlDEPTID.SelectedValue + @"') B
                                     WHERE A.GDSEQ(+) = B.GDSEQ
                                       AND A.DEPTID(+) = B.DEPTID";
            int total = 0;
            DataTable dtLot = PubFunc.DbGetPage(GridLot.PageIndex, GridLot.PageSize, strSql + strStrch, ref total);
            GridLot.RecordCount = total;
            GridLot.DataSource = dtLot;
            GridLot.DataBind();
            WindowLot.Hidden = false;
        }
        protected void tgbStrch_TriggerClick(object sender, EventArgs e)
        {
            billGoods();
        }
//        protected void btnClosePostBack_Click(object sender, EventArgs e)
//        {
//            foreach (GridRow row in GridLot.Rows)
//            {
//                int rowIndex = row.RowIndex;
//                System.Web.UI.WebControls.TextBox tbxNumber = (System.Web.UI.WebControls.TextBox)GridLot.Rows[rowIndex].FindControl("tbxNumber");
//                if (!string.IsNullOrWhiteSpace(tbxNumber.Text) && tbxNumber.Text != "0")
//                {
//                    if (ddlKCTYPE.SelectedValue == "A" && tbxNumber.Text.Trim().StartsWith("-"))
//                    {
//                        tbxNumber.Text = tbxNumber.Text.Substring(1, tbxNumber.Text.Length - 1);
//                    }
//                    else if (ddlKCTYPE.SelectedValue == "M" && !tbxNumber.Text.Trim().StartsWith("-"))
//                    {
//                        tbxNumber.Text = Math.Abs(Convert.ToDecimal(tbxNumber.Text)).ToString();
//                    }
//                }

//                if (!string.IsNullOrWhiteSpace(tbxNumber.Text) && tbxNumber.Text != "0")
//                {
//                    //string PICINO = GridLot.DataKeys[rowIndex][0].ToString();//
//                    string PH = row.Values[8].ToString();
//                    string gdseq = row.Values[2].ToString();
//                    string pssid = row.Values[11].ToString();
//                    string supid = row.Values[13].ToString();
//                    //表体赋值
//                    //金额取批次中的价格A.KCHSJJ,A.KCBHSJJ,a.KCLSJ
//                    string strSql = @"SELECT 0 SYSL,0 HSJE,0 LSJE,0 BHSJE,b.isflag5 ISFLOAT,B.GDSPEC,A.GDSEQ,B.GDNAME,B.NAMEJC,B.PIZNO PZWH,B.BARCODE,B.ISLOT,
//                                    A.PSSID,F_GETSUPNAME(A.SUPID) SUPNAME,A.HWID,A.SUPID,F_GETSUPPLIERNAME(A.PSSID) PSSNAME,B.GDMODE,A.BZHL,SUM(A.KCSL - A.LOCKSL) KCSL,SUM(A.KCHSJE) KCHSJE,A.JXTAX,
//                                    B.HSJJ,B.BHSJJ,A.KCLSJ LSJ,A.PH,B.PIZNO,A.RQ_SC,A.YXQZ,A.ZPBH,A.UNIT,A.PH,F_GETUNITNAME(A.UNIT) UNITNAME,b.isgz
//                                    FROM DAT_GOODSSTOCK A, DOC_GOODS B
//                                    WHERE A.GDSEQ = B.GDSEQ AND A.PH = '{0}' AND A.DEPTID ='{1}' AND A.GDSEQ = '{2}' -- A.KCSL>0 AND A.KCSL-A.LOCKSL>0
//                                    AND A.PSSID='{3}' AND A.SUPID='{4}'
//                                    GROUP BY A.KCLSJ,A.PSSID,b.isflag5,B.GDSPEC,A.GDSEQ,B.GDNAME,B.NAMEJC,B.PIZNO,B.BARCODE,B.ISLOT,A.PSSID,A.HWID,A.SUPID,B.GDMODE,A.BZHL,A.JXTAX,B.HSJJ,B.BHSJJ,LSJ,A.PH,B.PIZNO,A.RQ_SC,A.YXQZ,A.ZPBH,A.UNIT,PH,b.isgz";
//                    DataTable dtBill = DbHelperOra.Query(string.Format(strSql, PH, ddlDEPTID.SelectedValue, gdseq, pssid, supid)).Tables[0];
//                    //PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
//                    if (dtBill != null && dtBill.Rows.Count > 0)
//                    {
//                        decimal bzslTotal = 0, feeTotal = 0, ddslTotal = 0, je1 = 0, je2 = 0, je3 = 0;
//                        foreach (DataRow AddRow in dtBill.Rows)
//                        {
//                            AddRow["SYSL"] = tbxNumber.Text;
//                            AddRow["HSJE"] = Convert.ToDecimal(tbxNumber.Text) * Convert.ToDecimal(AddRow["HSJJ"]);
//                            AddRow["BHSJE"] = Convert.ToDecimal(tbxNumber.Text) * Convert.ToDecimal(AddRow["BHSJJ"]);
//                            AddRow["LSJE"] = Convert.ToDecimal(tbxNumber.Text) * Convert.ToDecimal(AddRow["LSJ"]);
//                            if (AddRow["RQ_SC"] != DBNull.Value)
//                            {
//                                AddRow["RQ_SC"] = DateTime.Parse(AddRow["RQ_SC"].ToString()).ToString("yyyy-MM-dd");
//                            }
//                            if (AddRow["YXQZ"] != DBNull.Value)
//                            {
//                                AddRow["YXQZ"] = DateTime.Parse(AddRow["YXQZ"].ToString()).ToString("yyyy-MM-dd");
//                            }
//                            ddslTotal += Convert.ToDecimal(AddRow["KCSL"]);
//                            bzslTotal += Convert.ToDecimal(AddRow["SYSL"]);
//                            feeTotal += Convert.ToDecimal(AddRow["KCHSJE"]);
//                            je1 += Convert.ToDecimal(AddRow["HSJE"]);
//                            je2 += Convert.ToDecimal(AddRow["BHSJE"]);
//                            je3 += Convert.ToDecimal(AddRow["LSJE"]);
//                            //严格校验数量，减去锁定库存cjl

//                            if (ddlKCTYPE.SelectedValue == "M" && decimal.Parse(AddRow["KCSL"].ToString()) < Math.Abs(decimal.Parse(tbxNumber.Text)))
//                            {
//                                Alert.Show("损益数量不能大于库存数量！", MessageBoxIcon.Warning);
//                                return;
//                            }
//                            //严格校验数量，判断商品cjl
//                            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().ToList();
//                            int sameRowCount = newDict.Where(a => a["GDSEQ"].ToString() == AddRow["GDSEQ"].ToString() && a["PH"].ToString() == AddRow["PH"].ToString()).Count();
//                            if (sameRowCount > 0)
//                            {
//                                Alert.Show("商品编码为【" + row.DataKeys[0].ToString() + "】的商品已经存在，无需重复填加！", MessageBoxIcon.Warning);
//                                return;
//                            }
//                            PubFunc.GridRowAdd(GridGoods, AddRow, false);
//                        }

//                        JObject summary = new JObject();
//                        summary.Add("GDNAME", "本页合计");
//                        summary.Add("KCSL", ddslTotal.ToString());
//                        summary.Add("SYSL", bzslTotal.ToString());
//                        summary.Add("KCHSJE", feeTotal.ToString("F2"));
//                        summary.Add("HSJE", je1.ToString("F2"));
//                        summary.Add("BHSJE", je2.ToString("F2"));
//                        summary.Add("LSJE", je3.ToString("F2"));
//                        GridGoods.SummaryData = summary;
//                    }
//                }
//            }
//            WindowLot.Hidden = true;
//        }
        protected void btnClosePostBack_Click(object sender, EventArgs e)
        {
            foreach (GridRow row in GridLot.Rows)
            {
                int rowIndex = row.RowIndex;
                System.Web.UI.WebControls.TextBox tbxNumber = (System.Web.UI.WebControls.TextBox)GridLot.Rows[rowIndex].FindControl("tbxNumber");
                //if (!string.IsNullOrWhiteSpace(tbxNumber.Text) && tbxNumber.Text != "0")
                //{
                //    if (ddlKCTYPE.SelectedValue == "A" && tbxNumber.Text.Trim().StartsWith("-"))
                //    {
                //        tbxNumber.Text = tbxNumber.Text.Substring(1, tbxNumber.Text.Length - 1);
                //    }
                //    else if (ddlKCTYPE.SelectedValue == "M" && !tbxNumber.Text.Trim().StartsWith("-"))
                //    {
                //        tbxNumber.Text = Math.Abs(Convert.ToDecimal(tbxNumber.Text)).ToString();
                //    }
                //}

                if (!string.IsNullOrWhiteSpace(tbxNumber.Text) && tbxNumber.Text != "0")
                {
                    //string PICINO = GridLot.DataKeys[rowIndex][0].ToString();//

                    string gdseq = row.Values[2].ToString();

                    //表体赋值
                    //金额取批次中的价格A.KCHSJJ,A.KCBHSJJ,a.KCLSJ
                    string strSql = @"select B.GDSEQ,B.NAMEJC,B.GDNAME,NVL(A.UNIT, B.UNIT) UNIT,F_GETUNITNAME(NVL(A.UNIT, B.UNIT)) UNITNAME,
                            B.GDSPEC,B.ZDKC,B.ZGKC, NVL(SUM(A.KCSL - A.LOCKSL), 0) KCSL, B.BZHL,
                            0 SYSL,0 HSJE,B.HSJJ,B.PIZNO PZWH,A.HWID,0 BHSJJ,0 BHSJE,0 LSJ,0 LSJE
                            from (select * from dat_goodsstock where DEPTID ='{0}')A,
                             (SELECT G.GDSEQ, G.NAMEJC,G.GDNAME,G.UNIT,G.GDSPEC,G.HSJJ,G.PIZNO, GF.DEPTID,GF.ZDKC,GF.ZGKC,G.BZHL
                                      FROM DOC_GOODS G, DOC_GOODSCFG GF
                                     WHERE G.GDSEQ = GF.GDSEQ
                                       AND G.ISGZ = 'N'
                                       AND GF.DEPTID ='{0}') B
                             WHERE A.GDSEQ(+) = B.GDSEQ
                               AND A.DEPTID(+) = B.DEPTID
                                AND B.GDSEQ = '{1}'
                             GROUP BY B.GDSEQ, B.NAMEJC,B.GDNAME, B.PIZNO, NVL(A.UNIT, B.UNIT),B.GDSPEC,B.ZDKC,B.ZGKC,A.HWID,B.HSJJ,B.BZHL ";
                    DataTable dtBill = DbHelperOra.Query(string.Format(strSql, ddlDEPTID.SelectedValue, gdseq)).Tables[0];
                    //PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
                    if (dtBill != null && dtBill.Rows.Count > 0)
                    {
                        decimal bzslTotal = 0, ddslTotal = 0, je1 = 0;
                        foreach (DataRow AddRow in dtBill.Rows)
                        {
                            AddRow["SYSL"] = tbxNumber.Text;
                            AddRow["HSJE"] = Convert.ToDecimal(tbxNumber.Text) * Convert.ToDecimal(AddRow["HSJJ"]);
                            AddRow["BHSJE"] = Convert.ToDecimal(tbxNumber.Text) * Convert.ToDecimal(AddRow["BHSJJ"]);
                            AddRow["LSJE"] = Convert.ToDecimal(tbxNumber.Text) * Convert.ToDecimal(AddRow["LSJ"]);

                            ddslTotal += Convert.ToDecimal(AddRow["KCSL"]);
                            bzslTotal += Convert.ToDecimal(AddRow["SYSL"]);
                            //feeTotal += Convert.ToDecimal(AddRow["KCHSJE"]);
                            je1 += Convert.ToDecimal(AddRow["HSJE"]);
                            //je2 += Convert.ToDecimal(AddRow["BHSJE"]);
                            //je3 += Convert.ToDecimal(AddRow["LSJE"]);
                            //严格校验数量，减去锁定库存cjl

                            //if (ddlKCTYPE.SelectedValue == "M" && decimal.Parse(AddRow["KCSL"].ToString()) < Math.Abs(decimal.Parse(tbxNumber.Text)))
                            //{
                            //    Alert.Show("损益数量不能大于库存数量！", MessageBoxIcon.Warning);
                            //    return;
                            //}
                            //严格校验数量，判断商品cjl
                            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().ToList();
                            int sameRowCount = newDict.Where(a => a["GDSEQ"].ToString() == AddRow["GDSEQ"].ToString()).Count();
                            if (sameRowCount > 0)
                            {
                                Alert.Show("商品编码为【" + row.Values[2].ToString() + "】的商品已经存在，无需重复填加！", MessageBoxIcon.Warning);
                                return;
                            }
                            JObject defaultObj = new JObject();
                            foreach (DataColumn col in AddRow.Table.Columns)
                            {
                                defaultObj.Add(col.ColumnName.ToUpper(), AddRow[col.ColumnName].ToString());
                            }
                            PageContext.RegisterStartupScript(GridGoods.GetAddNewRecordReference(defaultObj, true));
                        }

                        JObject summary = new JObject();
                        summary.Add("GDNAME", "本页合计");
                        summary.Add("KCSL", ddslTotal.ToString());
                        summary.Add("SYSL", bzslTotal.ToString());
                        //summary.Add("KCHSJE", feeTotal.ToString("F2"));
                        summary.Add("HSJE", je1.ToString("F2"));
                        //summary.Add("BHSJE", je2.ToString("F2"));
                        //summary.Add("LSJE", je3.ToString("F2"));
                        GridGoods.SummaryData = summary;
                    }
                }
            }
            WindowLot.Hidden = true;
        }
        protected void btnClose_Click(object sender, EventArgs e)
        {
            WindowLot.Hidden = true;
        }
        protected override void billSave()
        {
            save();
        }

        private void save(string flag="N")
        {
            #region 数据有效性验证
            if (ddlFLAG.SelectedValue != "M" && ddlFLAG.SelectedValue != "R")
            {
                Alert.Show("非新单不能保存！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (ddlKCTYPE.SelectedValue.Length < 1)
            {
                Alert.Show("请选择【损益类别】");
                return;
            }
            if (PubFunc.FormDataCheck(FormDoc).Length > 0) return;

            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().ToList();
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
                    goodsData.Add(newDict[i]);
                }
                if (string.IsNullOrWhiteSpace(newDict[i]["BZHL"].ToString()) || string.IsNullOrWhiteSpace(newDict[i]["UNIT"].ToString()))
                {
                    Alert.Show("商品包装单位信息错误，请联系管理员进行维护！", "消息提示", MessageBoxIcon.Warning);
                    return;
                }
                //if (newDict[i]["ISLOT"].ToString() == "2" && string.IsNullOrWhiteSpace(newDict[i]["PH"].ToString()))
                //{
                //    Alert.Show("商品批号不能为空，请进行维护！", "消息提示", MessageBoxIcon.Warning);
                //    return;
                //}
                if (string.IsNullOrWhiteSpace(newDict[i]["SYSL"].ToString()))
                {
                    Alert.Show("请填写[" + newDict[i]["GDSEQ"] + "]正确的损益数量!", "提示信息", MessageBoxIcon.Warning);
                    return;
                }
                if (ddlKCTYPE.SelectedValue == "M" && Convert.ToInt32(newDict[i]["SYSL"]) > 0)
                {
                    Alert.Show("单据为损耗单，商品[" + newDict[i]["GDSEQ"] + "]损益数量不能大于0!", "提示信息", MessageBoxIcon.Warning);
                    return;
                }

                else if (ddlKCTYPE.SelectedValue == "A" && Convert.ToInt32(newDict[i]["SYSL"]) < 0)
                {
                    Alert.Show("单据为溢余单，商品[" + newDict[i]["GDSEQ"] + "]损益数量不能小于0!", "提示信息", MessageBoxIcon.Warning);
                    return;
                }

                //if (ddlKCTYPE.SelectedValue == "A" && newDict[i]["ISGZ"].ToString() == "Y")
                //{
                //    Alert.Show("单据为溢余单，商品[" + newDict[i]["GDSEQ"] + "]为高值商品，不能溢余!", "提示信息", MessageBoxIcon.Warning);
                //    return;
                //}
                //if (ddlKCTYPE.SelectedValue == "M")
                //{
                //    decimal kc, sy;
                //    Decimal.TryParse(newDict[i]["KCSL"].ToString(), out kc);
                //    Decimal.TryParse(newDict[i]["SYSL"].ToString(), out sy);
                //    if (kc < Math.Abs(sy))
                //    {
                //        Alert.Show("【损耗数量】不能大于【库存数量】！", "消息提示", MessageBoxIcon.Warning);
                //        return;
                //    }
                //}
            }
            if (goodsData.Count == 0)//所有Gird行都为空行时
            {
                Alert.Show("商品信息不能为空", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            #endregion
            if (ddlFLAG.SelectedValue == "R")
            {
                ddlFLAG.SelectedValue = "M";
            }
            if (PubFunc.StrIsEmpty(tbxBILLNO.Text))
            {
                tbxBILLNO.Text = BillSeqGet();
                tbxBILLNO.Enabled = false;
            }
            MyTable mtType = new MyTable("DAT_SY_DOC");
            mtType.ColRow = PubFunc.FormDataHT(FormDoc);
            mtType.ColRow["SEQNO"] = tbxBILLNO.Text;
            mtType.ColRow["FLAG"] = "M";
            mtType.ColRow.Add("BILLTYPE", BillType);
            mtType.ColRow.Add("SUBNUM", goodsData.Count);
            if (ddlKCTYPE.SelectedValue == "A")
            {
                mtType.ColRow.Add("SYTYPE", "0");
            }
            else if (ddlKCTYPE.SelectedValue == "M")
            {
                mtType.ColRow.Add("SYTYPE", "1");
            }
            //如果是代管商品，把发送标志设为G-即不发送
            if (isDg())
            {
                mtType.ColRow.Add("ISSEND", "G");
            }

            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtTypeMx = new MyTable("DAT_SY_COM");
            decimal subNum = 0;//总金额
            //先删除单据信息在插入
            cmdList.Add(mtType.DeleteCommand(""));//删除单据台头
            cmdList.Add(new CommandInfo("delete DAT_SY_COM where seqno='" + tbxBILLNO.Text + "'", null));//删除单据明细
            for (int i = 0; i < goodsData.Count; i++)
            {
                mtTypeMx.ColRow = PubFunc.GridDataGet(goodsData[i]);

                //判断含税进价，为0时不能保存
                string isJiFei = string.Format("select 1 from DOC_GOODS t where iscf = 'N' and gdseq = '{0}'", mtTypeMx.ColRow["GDSEQ"].ToString());
                if (DbHelperOra.Exists(isJiFei))
                {
                    if (PubFunc.StrIsEmpty(mtTypeMx.ColRow["HSJJ"].ToString()) || mtTypeMx.ColRow["HSJJ"].ToString() == "0")
                    {
                        Alert.Show("商品【含税进价】为0或空，无法进行【商品损益管理】操作。");
                        return;
                    }


                }
                //重新计算金额

                mtTypeMx.ColRow["HSJE"] = Convert.ToDecimal(mtTypeMx.ColRow["SYSL"]) * Convert.ToDecimal(mtTypeMx.ColRow["HSJJ"]);
                mtTypeMx.ColRow["BHSJE"] = Convert.ToDecimal(mtTypeMx.ColRow["SYSL"]) * Convert.ToDecimal(string.IsNullOrWhiteSpace(mtTypeMx.ColRow["BHSJJ"].ToString()) ? "0" : mtTypeMx.ColRow["BHSJJ"].ToString());
                mtTypeMx.ColRow["LSJE"] = Convert.ToDecimal(mtTypeMx.ColRow["SYSL"]) * Convert.ToDecimal(string.IsNullOrWhiteSpace(mtTypeMx.ColRow["LSJ"].ToString()) ? "0" : mtTypeMx.ColRow["LSJ"].ToString());
                //mtTypeMx.ColRow["SYSL"] = Convert.ToDecimal(mtTypeMx.ColRow["SYSL"]) - Convert.ToDecimal(mtTypeMx.ColRow["KCSL"]);
                subNum = subNum + decimal.Parse(mtTypeMx.ColRow["HSJE"].ToString());
                mtTypeMx.ColRow.Add("SEQNO", tbxBILLNO.Text);
                mtTypeMx.ColRow["BZSL"] = mtTypeMx.ColRow["SYSL"];
                mtTypeMx.ColRow["BZHL"] = "1";
                mtTypeMx.ColRow.Remove("KCHSJE");
                mtTypeMx.ColRow.Add("KCHSJE",0);
                mtTypeMx.ColRow["ROWNO"] = i + 1;
                mtTypeMx.ColRow.Add("PHID", "ERPCSKC");
                mtTypeMx.ColRow["PH"] = "ERPCSKC";
                mtTypeMx.ColRow["RQ_SC"] = "2017-01-01";
                mtTypeMx.ColRow["YXQZ"] = "2019-01-01";
                mtTypeMx.ColRow["SUPID"] = "00001";
                mtTypeMx.ColRow["STR1"] = "00001";
                mtTypeMx.ColRow.Remove("UNITNAME");
                cmdList.Add(mtTypeMx.Insert());
            }
            mtType.ColRow["SUBSUM"] = subNum;
            //mtType.ColRow.Add("SUBSUM", subNum);
            cmdList.AddRange(mtType.InsertCommand());

            DbHelperOra.ExecuteSqlTran(cmdList);
         
                Alert.Show("损益单信息保存成功！");
            //billNew();
            billOpen(tbxBILLNO.Text);
            billLockDoc(true);
            OperLog("商品损益", "修改单据【" + tbxBILLNO.Text + "】");
            //string strSql = @"SELECT 1 FROM DAT_SY_COM A
            //                         WHERE A.SEQNO = '{0}' AND A.ISGZ = 'Y'AND A.GDSEQ NOT IN (SELECT GDSEQ FROM DAT_SY_EXT WHERE BILLNO = '{0}')";

            //if (DbHelperOra.Exists(string.Format(strSql, tbxBILLNO.Text)))
            //{
            //    btnScan_Click(null, null);
            //    return;
            //}
            SaveSuccess = true;
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
        protected void GridLot_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridLot.PageIndex = e.NewPageIndex;
            billGoods();
        }
        private bool SaveSuccess = false;
        protected void btnTj_Click(object sender, EventArgs e)
        {
            //提交单据
            string billno = tbxBILLNO.Text;
            if (PubFunc.StrIsEmpty(billno))
            {
                Alert.Show("请打开需要提交的单据！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            if (ddlFLAG.SelectedValue != "M")
            {
                Alert.Show("只有保存后的单据，才能提交！");
                return;
            }
            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_SY_DOC WHERE SEQNO = '{0}' AND FLAG IN ('M','R')", billno)))
            {
                Alert.Show("单据【" + billno + "】不存在或已被提交审核,请检查！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
           // SaveSuccess = false;
           //// save("Y");
           // if (SaveSuccess == false)
           //     return;
           // SaveSuccess = false;

            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().OrderBy(x => x["GDSEQ"]).ToList();
            //object obj = DbHelperOra.GetSingle(String.Format("SELECT gdseq FROM DAT_SY_com A WHERE A.SEQNO = '{0}' AND (GDSEQ,PH) NOT IN(SELECT GDSEQ,PH FROM DAT_GOODSSTOCK) AND ROWNUM = 1", billno));
            //if ((obj ?? "").ToString().Length > 0)
            //{
            //    Alert.Show("商品【" + obj + "】批次维护错误,请检查！", "提示信息", MessageBoxIcon.Warning);
            //    return;
            //}
            //判断是否有空行、批号填写是否符合要求
            for (int i = 0; i < newDict.Count; i++)
            {
                string strGdseq = newDict[i]["GDSEQ"].ToString();

                if (string.IsNullOrWhiteSpace(newDict[i]["BZHL"].ToString()) || string.IsNullOrWhiteSpace(newDict[i]["UNIT"].ToString()))
                {
                    Alert.Show("商品包装单位信息错误，请联系管理员进行维护！", "消息提示", MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(newDict[i]["SYSL"].ToString()))
                {
                    Alert.Show("请填写[" + newDict[i]["GDSEQ"] + "]正确的损益数量!", "提示信息", MessageBoxIcon.Warning);
                    return;
                }
                //if (ddlKCTYPE.SelectedValue == "M" && Convert.ToInt32(newDict[i]["SYSL"]) > 0)
                //{
                //    Alert.Show("单据为损耗单，商品[" + newDict[i]["GDSEQ"] + "]损益数量不能大于0!", "提示信息", MessageBoxIcon.Warning);
                //    return;
                //}
                //else if (ddlKCTYPE.SelectedValue == "A" && Convert.ToInt32(newDict[i]["SYSL"]) < 0)
                //{
                //    Alert.Show("单据为溢余单，商品[" + newDict[i]["GDSEQ"] + "]损益数量不能小于0!", "提示信息", MessageBoxIcon.Warning);
                //    return;
                //}
            }
//            if (ddlKCTYPE.SelectedValue == "M")
//            {
//            object COM = DbHelperOra.GetSingle(String.Format("SELECT ABS(SUM(SYSL)) FROM DAT_SY_COM A,DOC_GOODS B WHERE A.SEQNO = '{0}' AND A.GDSEQ=B.GDSEQ AND B.ISGZ='Y'", billno));
//                //object EXT = DbHelperOra.GetSingle(String.Format(@"SELECT NVL(count(1), 0)
//                //                      FROM DAT_SY_EXT A, DAT_SY_COM B
//                //                     WHERE a.BILLNO = '{0}'
//                //                       and a.billno = b.seqno
//                //                       AND A.GDSEQ = B.GDSEQ
//                //                       AND A.ROWNO = B.ROWNO", billno));
//                object EXT = DbHelperOra.GetSingle(String.Format(@"SELECT NVL(count(1), 0)
//                                  FROM DAT_SY_EXT A
//                                 WHERE a.BILLNO = '{0}'", billno));
//                int comnum= Convert.ToInt32(COM ?? "0");
//            int extnum = Convert.ToInt32(EXT ?? "0");
//                if (comnum != extnum && extnum == 0)
//                {
//                    //Alert.Show("单据【" + tbxBILLNO.Text + "】追溯码数量不正确，请维护！","提示信息", MessageBoxIcon.Warning);
//                    btnScan_Click(null, null);
//                    return;
//                }
//                else if (comnum != extnum && extnum != 0)
//                {
//                    Alert.Show("单据【" + tbxBILLNO.Text + "】追溯码数量不正确，请维护！", "提示信息", MessageBoxIcon.Warning);
//                    return;
//                }
//            }

            if (BillOper(tbxBILLNO.Text, "DECLARE") == 1)
            {
                billLockDoc(true);
                Alert.Show("单据【" + tbxBILLNO.Text + "】提交成功！");
                billOpen(billno);
                OperLog("商品损益", "提交单据【" + tbxBILLNO.Text + "】");
            }
        }
        protected void btnScan_Click(object sender, EventArgs e)
        {
            if (PubFunc.StrIsEmpty(tbxBILLNO.Text))
            {
                Alert.Show("请保存单据后进行扫描追溯码操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_SY_COM A,DOC_GOODS B WHERE A.GDSEQ = B.GDSEQ AND B.ISGZ = 'Y' AND SEQNO = '{0}'", tbxBILLNO.Text)))
            {
                Alert.Show("此单据中没有已经保存的高值商品,请检查！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            WindowScan.Hidden = false;
            ScanSearch("SHOW");
        }
        protected void ScanSearch(string type)
        {
            string sql = "";
            if (type == "SHOW")
            {
                sql = "SELECT A.*,f_getunitname(A.UNIT) UNITNAME FROM DAT_SY_EXT A WHERE A.BILLNO = '{0}' ORDER BY A.GDSEQ,A.INSTIME DESC";
            }
            else
            {
                sql = "SELECT A.*,f_getunitname(A.UNIT) UNITNAME FROM DAT_SY_EXT A WHERE A.BILLNO = '{0}' ORDER BY A.INSTIME DESC";
            }
            DataTable dtScan = DbHelperOra.Query(string.Format(sql, tbxBILLNO.Text)).Tables[0];
            //PubFunc.GridRowAdd(GridSacn, dtScan);
            GridSacn.DataSource = dtScan;
            GridSacn.DataBind();
            zsmScan.Text = String.Empty;
            zsmScan.Focus();
        }
        protected void zsmScan_TextChanged(object sender, EventArgs e)
        {
            if (ddlFLAG.SelectedValue != "M")
            {
                Alert.Show("非『新单』单据不允许操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (zsmScan.Text.Length < 28) return;
            if (zsmScan.Text.Substring(0, 1) != "2")
            {
                Alert.Show("您扫描的条码不是贵重码,请检查！", "提示信息", MessageBoxIcon.Warning);
                zsmScan.Text = string.Empty;
                zsmScan.Focus();
                return;
            }
            if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_SY_DOC WHERE SEQNO = '{0}' AND DEPTID IN(SELECT CODE FROM SYS_DEPT WHERE TYPE='1')", tbxBILLNO.Text)))
              {
                if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_GZ_EXT WHERE ONECODE = '{0}' AND (FLAG ='Y' OR FLAG='R')", zsmScan.Text)))
                {
                    Alert.Show("您扫描的二维码不存在或未入库审核或已损耗，请检查！", "提示信息", MessageBoxIcon.Warning);
                    zsmScan.Text = string.Empty;
                    zsmScan.Focus();
                    return;
                }
            }
            else if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_SY_DOC WHERE SEQNO = '{0}' AND DEPTID IN(SELECT CODE FROM SYS_DEPT WHERE TYPE='3')", tbxBILLNO.Text)))
                {
                if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_GZ_EXT WHERE ONECODE = '{0}' AND FLAG ='C'", zsmScan.Text)))
                {
                    Alert.Show("您扫描的二维码不存在或已使用或已损耗，请检查！", "提示信息", MessageBoxIcon.Warning);
                    zsmScan.Text = string.Empty;
                    zsmScan.Focus();
                    return;
                }
            }
            
            //判断条码是否已经退货
            if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_TH_EXT WHERE ONECODE = '{0}' AND FLAG ='Y'", zsmScan.Text)))
            {
                Alert.Show("您扫描的二维码已经退货，请检查！", "提示信息", MessageBoxIcon.Warning);
                zsmScan.Text = string.Empty;
                zsmScan.Focus();
                return;
            }

            if (!DbHelperOra.Exists(string.Format(@"select 1
  from DAT_SY_com sy, dat_gz_ext ext
 where
    ext.gdseq = sy.gdseq
   and sy.supid = ext.supid
    and sy.pssid = ext.pssid
    AND SY.PH=EXT.PH
   and sy.seqno = '{0}'
   --and ext.flag <>'T' and ext.flag<>'S'
   and onecode = '{1}'", tbxBILLNO.Text, zsmScan.Text))) {
                Alert.Show("您扫描的二维码不存在，或不是本配送商/供应商的，或不是对应批号！", "提示信息", MessageBoxIcon.Warning);
                zsmScan.Text = string.Empty;
                zsmScan.Focus();
                return;
            }
            //String exis = (DbHelperOra.GetSingle(String.Format("SELECT BILLNO FROM DAT_CK_EXT WHERE ONECODE = '{0}' AND FLAG <> 'T'", zsmScan.Text)) ?? "").ToString();
            //if (!PubFunc.StrIsEmpty(exis))
            //{
            //    Alert.Show("您输入的追溯码已被单据【" + exis + "】使用,请检查！", "提示信息", MessageBoxIcon.Warning);
            //    zsmScan.Text = string.Empty;
            //    zsmScan.Focus();
            //    return;
            //}
            //写入数据库中
            string msg = "";
            DataTable dtt = DbHelperOra.Query(string.Format("SELECT ONECODE,BILLNO,DEPTID,F_GETDEPTNAME(DEPTID) DEPTNAME FROM DAT_SY_EXT WHERE ONECODE = '{0}'", zsmScan.Text.Trim())).Tables[0];
            if (dtt.Rows.Count > 0)
            {
                msg = "追溯码【" + dtt.Rows[0]["ONECODE"] + "】在【[" + dtt.Rows[0]["DEPTID"] + "]" + dtt.Rows[0]["DEPTNAME"] + "】被单据【" + dtt.Rows[0]["BILLNO"] + "】使用！";
                Alert.Show(msg, "提示信息", MessageBoxIcon.Warning);
                zsmScan.Text = string.Empty;
                zsmScan.Focus();
                return;
            }
            else
            { 
            DbHelperOra.ExecuteSql(string.Format(@"INSERT INTO DAT_SY_EXT(DEPTID,BILLNO,ROWNO,ONECODE,GDSEQ,GDNAME,BARCODE,UNIT,GDSPEC,DEPTCUR,BZHL,INSTIME,PH,RQ_SC,YXQZ)
                    SELECT '{0}','{1}',NVL((SELECT MAX(ROWNO)+1 FROM DAT_CK_EXT WHERE BILLNO = '{1}'),1),'{2}',GDSEQ,GDNAME,BARCODE,UNIT,GDSPEC,DEPTCUR,BZHL,SYSDATE,PH,RQ_SC,YXQZ
                    FROM DAT_RK_EXT A
                    WHERE A.ONECODE = '{2}'", ddlDEPTID.SelectedValue, tbxBILLNO.Text, zsmScan.Text));
            }
            ScanSearch("");
        }
        protected void zsmDelete_Click(object sender, EventArgs e)
        {
            if (ddlFLAG.SelectedValue != "M")
            {
                Alert.Show("非『新单』单据不允许操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (GridSacn.SelectedCell == null)
            {
                Alert.Show("请选择您需要删除的数据！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if ((Convert.ToInt32(GridSacn.SelectedRowIndex)) < 0)
            {
                Alert.Show("请选择您需要删除的数据！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string barcode = "";
            foreach (int rowIndex in GridSacn.SelectedRowIndexArray)
            {

                barcode += GridSacn.DataKeys[rowIndex][0].ToString() + ",";
            }
            string onecode= barcode.Replace(",", "','");
            //string onecode = (GridSacn.DataKeys[GridSacn.SelectedRowIndex][0]).ToString();
            DbHelperOra.ExecuteSql(string.Format("DELETE FROM DAT_SY_EXT WHERE ONECODE in ('{0}')", onecode));
            ScanSearch("");
            OperLog("商品损益", "删除单据【" + tbxBILLNO.Text + "】唯一码【" + onecode + "】");
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

        protected void GridLot_AfterEdit(object sender, GridAfterEditEventArgs e)
        {
            string ss = GridLot.Rows[GridLot.SelectedRowIndex].Values[3].ToString();
        }
    }
}