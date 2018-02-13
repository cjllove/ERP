﻿using FineUIPro;
using Newtonsoft.Json.Linq;
using XTBase;
using XTBase.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPProject.ERPApply
{
    public partial class DepartmentAllocate : BillBase
    {
        private string strDocSql = "SELECT * FROM DAT_KD_DOC WHERE BILLTYPE = 'KSD' AND SEQNO ='{0}'";
        private string strComSql = "SELECT t.*,F_GETUNITNAME(UNIT) UNITNAME,F_GETPRODUCERNAME(PRODUCER) PRODUCERNAME FROM DAT_KD_COM t WHERE SEQNO ='{0}' ORDER BY ROWNO";
        public DepartmentAllocate()
        {
            BillType = "KSD";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //屏蔽不需要的操作按钮
                //ButtonHidden( btnExport, btnCopy);
                if (Request.QueryString["oper"] != null)
                {
                    if (Request.QueryString["oper"].ToString() == "input")
                    {
                        ButtonHidden(btnAudit);
                    }
                    else if (Request.QueryString["oper"].ToString() == "audit")
                    {
                        billLockDoc(true);
                        //ButtonHidden(btnNew, btnSave, btnAddRow, btnDelRow);
                    }
                }
                DataInit();
                billNew();
                btnDel.Enabled = false;
            }
        }

        private void DataInit()
        {
            ButtonHidden(btnCopy);
            //PubFunc.DdlDataGet("DDL_SYS_DEPOT", lstDEPTOUT, docDEPTOUT);
            PubFunc.DdlDataGet("DDL_SYS_DEPTDEF", lstDEPTOUT, docDEPTOUT);
            DepartmentBind.BindDDL("DDL_SYS_DEPARTMENTRANGE", UserAction.UserID, lstDEPTID, docDEPTID);
            //DepartmentBind.BindDDL("DDL_SYS_DEPTRANGE", UserAction.UserID, lstDEPTID, docDEPTID);
            PubFunc.DdlDataGet("DDL_USER", lstSLR, docLRY, docSLR, docSHR);
            //PubFunc.DdlDataGet("DDL_SYS_DEPTNULL", lstDEPTID, docDEPTID);
            //PubFunc.DdlDataGet("DDL_BILL_STATUSDHD", docFLAG);
            PubFunc.DdlDataSql(lstFLAG, @"SELECT '' CODE ,'--请选择--' NAME  FROM dual
                                            union all
                                            SELECT 'N' CODE ,'新 单' NAME  FROM dual
                                            union all
                                            SELECT 'Y' CODE ,'已审核' NAME  FROM dual
                                            ");
            PubFunc.DdlDataSql(docFLAG, @"SELECT '' CODE ,'--请选择--' NAME  FROM dual
                                            union all
                                            SELECT 'N' CODE ,'新 单' NAME  FROM dual
                                            union all
                                            SELECT 'Y' CODE ,'已审核' NAME  FROM dual
                                            ");
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
        }

        protected override void billNew()
        {
            PubFunc.FormDataClear(FormDoc);
            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            docFLAG.SelectedValue = "N";
            docSLR.SelectedValue = UserAction.UserID;
            docLRY.SelectedValue = UserAction.UserID;
            docLRRQ.SelectedDate = DateTime.Now;
            docXSRQ.SelectedDate = DateTime.Now;
            docDEPTOUT.SelectedValue = "";
            docDEPTID.SelectedValue = "";

            btnDel.Enabled = false;
            docDEPTOUT.Enabled = true;
            docSLR.Enabled = true;
            docDEPTID.Enabled = true;
            docXSRQ.Enabled = true;
            docMEMO.Enabled = true;
            docSLR.Enabled = true;
            //tbxINSERT.Enabled = true;
            //改变按钮状态
            btnPrint.Enabled = false;
            btnSave.Enabled = true;
            btnAudit.Enabled = false;
            btnDelRow.Enabled = true;
            btnGoods.Enabled = true;
            //tbxINSERT.Focus();
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("BZSL", "0");
            summary.Add("HSJE", "0");
            GridGoods.SummaryData = summary;
        }
        protected void GridList_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                string flag = row["FLAG"].ToString();
                FineUIPro.BoundField flagcol = GridList.FindColumn("FLAGNAME") as FineUIPro.BoundField;
                if (flag == "N")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color1";
                }
            }
        }
        protected void GridGoods_AfterEdit(object sender, FineUIPro.GridAfterEditEventArgs e)
        {
            string[] strCell = GridGoods.SelectedCell;
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
            if (newDict.Count == 0) return;
            if (e.ColumnID == "BZSL")
            {
                JObject defaultObj = Doc.GetJObject(GridGoods, e.RowID);
                decimal hl = 0, rs = 0, jg = 0;
                decimal.TryParse((defaultObj["BZHL"] ?? "0").ToString(), out hl);
                decimal.TryParse((defaultObj["BZSL"] ?? "0").ToString(), out rs);
                decimal.TryParse((defaultObj["HSJJ"] ?? "0").ToString(), out jg);
                defaultObj["DHSL"] = rs * hl;
                defaultObj["HSJE"] = Math.Round(rs * jg, 2).ToString("F2");
                PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(e.RowID, defaultObj));
                //计算合计数量
                decimal bzslTotal = 0, feeTotal = 0;
                foreach (Dictionary<string, object> dic in newDict)
                {
                    decimal.TryParse(dic["BZSL"].ToString(), out jg);
                    bzslTotal += jg;

                    feeTotal += Convert.ToDecimal(dic["HSJJ"] ?? "0") * Convert.ToDecimal(dic["BZSL"] ?? "0") * Convert.ToDecimal(dic["BZHL"]);
                }
                JObject summary = new JObject();
                summary.Add("GDNAME", "本页合计");
                summary.Add("BZSL", bzslTotal.ToString());
                summary.Add("HSJE", feeTotal.ToString("F2"));
                GridGoods.SummaryData = summary;
            }
            else if (e.ColumnID == "PH")
            {
                JObject defaultObj = Doc.GetJObject(GridGoods, e.RowID);
                String Sql = string.Format("SELECT SUM(A.KCSL),A.PH,A.YXQZ,A.RQ_SC,B.PIZNO PZWH FROM DAT_GOODSSTOCK A,DOC_GOODS B WHERE A.DEPTID = '{0}' AND A.GDSEQ ='{1}' AND A.GDSEQ=B.GDSEQ  AND KCSL > LOCKSL GROUP BY PH,YXQZ,B.PIZNO,A.RQ_SC ", docDEPTOUT.SelectedValue, defaultObj["GDSEQ"].ToString());
                if (defaultObj["PH"].ToString() != "\\")
                {
                    Sql = string.Format("SELECT SUM(A.KCSL),A.PH,A.YXQZ,A.RQ_SC FROM DAT_GOODSSTOCK A WHERE A.DEPTID = '{0}' AND A.GDSEQ ='{1}'AND PH = '{2}'  AND KCSL > LOCKSL GROUP BY PH,YXQZ,A.RQ_SC", docDEPTOUT.SelectedValue, defaultObj["GDSEQ"].ToString(), defaultObj["PH"]);
                }
                DataTable dtPH = DbHelperOra.Query(Sql).Tables[0];
                if (dtPH != null && dtPH.Rows.Count > 0)
                {
                    if (dtPH.Rows.Count == 1)
                    {
                        defaultObj["PH"] = dtPH.Rows[0]["PH"].ToString();
                        defaultObj["YXQZ"] = dtPH.Rows[0]["YXQZ"].ToString();
                        defaultObj["RQ_SC"] = dtPH.Rows[0]["RQ_SC"].ToString();
                        PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(strCell[0], defaultObj));
                    }
                    else
                    {
                        GridLot.DataSource = dtPH;
                        GridLot.DataBind();
                        WindowLot.Hidden = false;
                    }
                }
                //else
                //{
                //    Alert.Show("科室无可调出库存！", "提示信息", MessageBoxIcon.Warning);
                //    return;
                //}
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
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;

            string strBillno = docSEQNO.Text;
            // 新增数据初始值
            JObject defaultObj = new JObject();
            defaultObj.Add("GDSEQ", "");
            defaultObj.Add("BarCode", "");
            defaultObj.Add("GDNAME", "");
            defaultObj.Add("GDSPEC", "");
            defaultObj.Add("UNIT", "");
            defaultObj.Add("UNITNAME", "");
            defaultObj.Add("BZHL", "");
            defaultObj.Add("BZSL", "1");
            defaultObj.Add("DHSL", "");
            defaultObj.Add("XSSL", "");
            defaultObj.Add("JXTAX", "");
            defaultObj.Add("HSJJ", "");
            defaultObj.Add("HSJE", "");
            defaultObj.Add("ZPBH", "");
            defaultObj.Add("PRODUCER", "");
            defaultObj.Add("PRODUCERNAME", "");
            defaultObj.Add("HWID", "");
            defaultObj.Add("PH", "");
            defaultObj.Add("PZWH", "");
            defaultObj.Add("RQ_SC", "");
            defaultObj.Add("YXQZ", "");
            defaultObj.Add("MEMO", "");

            PageContext.RegisterStartupScript(GridGoods.GetAddNewRecordReference(defaultObj, true));
        }

        protected override void billDelRow()
        {
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("非新增单据不能删除！");
                return;
            }
            if (GridGoods.SelectedCell == null) return;

            //string rowIndex = GridGoods.SelectedRowID;  // GridGoods.SelectedRowIndex动态行不能删除行索引，只能删除行ID
            //PageContext.RegisterStartupScript(GridGoods.GetDeleteRowReference(rowIndex));
            GridGoods.DeleteSelectedRows();
            PubFunc.FormLock(FormDoc, true, "");
            //tbxINSERT.Enabled = true;
        }

        protected override void billGoods()
        {
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
            //参数说明：cx-查询内容，dr-调入科室,dc-调出科室
            string url = "~/ERPQuery/GoodsWindow_KSD.aspx?dc=" + docDEPTOUT.SelectedValue + "&cx=&dr=" + docDEPTID.SelectedValue;
            PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "商品信息查询"));
        }

        protected override void billSearch()
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("输入条件录入日期不正确！");
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                Alert.Show("开始日期大于结束日期，请重新输入！");
                return;
            }

            string strSql = @"select A.SEQNO,
                                   A.BILLNO,
                                   A.FLAG,
                                   decode(A.FLAG,'N','新单','Y','已审核','状态未定义') FLAGNAME,
                                   F_GETDEPTNAME(A.DEPTOUT) DEPTOUT,
                                   F_GETDEPTNAME(A.DEPTID) DEPTID,
                                   A.XSRQ,
                                   A.SUBSUM,
                                   A.SUBNUM,
                                   F_GETUSERNAME(A.SLR) SLR,
                                   F_GETUSERNAME(A.LRY) LRY,
                                   A.LRRQ,
                                   F_GETUSERNAME(A.SHR) SHR,
                                   A.SHRQ,
                                   A.MEMO
                              from DAT_KD_DOC A, SYS_CODEVALUE B
                             WHERE A.TYPE = '1'
                               AND A.FLAG = B.CODE
                               AND A.BILLTYPE = 'KSD'
                               AND B.TYPE = 'DIC_BILLSTATUS'";
            string strSearch = "";


            if (lstBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND A.BILLNO  LIKE '%{0}%'", lstBILLNO.Text);
            }
            if (lstFLAG.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.FLAG='{0}'", lstFLAG.SelectedItem.Value);
            }
            if (lstSLR.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.SLR='{0}'", lstSLR.SelectedItem.Value);
            }
            if (lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedItem.Value);
            }
            if (lstDEPTOUT.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTOUT='{0}'", lstDEPTOUT.SelectedItem.Value);
            }
            if (tbxGDSEQ.Text.Length > 0)
            {
                strSearch += string.Format("AND EXISTS (SELECT 1 FROM DAT_KD_COM T WHERE T.SEQNO = A.SEQNO AND (T.GDSEQ  LIKE '%{0}%' OR T.GDNAME  LIKE '%{0}%'))", tbxGDSEQ.Text);
            }
            strSearch += string.Format(" AND deptid in( select code FROM SYS_DEPT where type <>'1' and  F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            strSearch += string.Format(" AND A.LRRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.LRRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += "  ORDER BY decode(A.FLAG,'N','1','2'),A.BILLNO DESC";
            GridList.DataSource = DbHelperOra.Query(strSql).Tables[0];
            GridList.DataBind();
        }
        private bool SaveSuccess = false;
        protected override void billAudit()
        {
            string strBill = docSEQNO.Text.Trim();
            object objFlag = DbHelperOra.GetSingle(string.Format("SELECT FLAG FROM DAT_KD_DOC WHERE SEQNO = '{0}'", strBill));
            if ((objFlag ?? "").ToString() != "N")
            {
                Alert.Show("非新单不能审核！");
                return;
            }
            //判断定数是否被退货
            if (DbHelperOra.Exists("SELECT 1 FROM DAT_GOODSDS_LOG WHERE SL < 1 AND BARCODE IN(SELECT DSCODE FROM DAT_KD_COM WHERE SEQNO = '" + strBill + "' AND NVL(DSCODE,'#') <> '#')"))
            {
                Alert.Show("定数调拨使用的部分定数码已被调拨，请检查！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            SaveSuccess = false;
            save("Y");
            if (SaveSuccess == false)
                return;
            SaveSuccess = false;
            //验证是否盘点
            if (DbHelperOra.Exists("SELECT 1 FROM DAT_PD_LOCK WHERE DEPTID = '" + docDEPTOUT.SelectedValue + "' AND FLAG='N'"))
            {
                Alert.Show(string.Format("部门【{0}】正在盘点,请盘点后进行操作！", docDEPTOUT.SelectedText), "警告提示", MessageBoxIcon.Warning);
                return;
            }
            object obj = DbHelperOra.GetSingle(string.Format(@"SELECT A.GDSEQ FROM
                            (SELECT GDSEQ,SUM(XSSL) SL 
                            FROM DAT_KD_COM WHERE SEQNO = '{1}' AND NVL(DSCODE,'#') = '#' 
                            GROUP BY GDSEQ) A,
                            (SELECT GDSEQ,SUM(KCSL) SL
                            FROM DAT_GOODSSTOCK WHERE DEPTID = '{0}' AND GDSEQ IN(SELECT GDSEQ
                            FROM DAT_KD_COM WHERE SEQNO = '{1}' AND NVL(DSCODE,'#') = '#') AND KCSL > 0
                            GROUP BY GDSEQ) B,
                            (SELECT GDSEQ,SUM(SL) SL
                            FROM Dat_Goodsds_Log WHERE DEPTIN = '{0}' AND FLAG = 'N'
                            GROUP BY GDSEQ) C
                            WHERE A.GDSEQ = B.GDSEQ AND A.GDSEQ = C.GDSEQ(+) AND A.SL > B.SL - NVL(C.SL,0) AND ROWNUM = 1", docDEPTOUT.SelectedValue, strBill));
            if ((obj ?? "").ToString().Length > 0)
            {
                Alert.Show("商品【" + obj.ToString() + "】非定数数量超出范围,请检查！", "提示信息", MessageBoxIcon.Warning);
                return;
            }

            if (DbHelperOra.Exists("SELECT 1 FROM DAT_KD_COM A,DAT_GOODSDS_LOG B WHERE A.SEQNO = '" + strBill + "' AND NVL(A.DSCODE,'#') <> '#' AND A.DSCODE = B.BARCODE AND B.FLAG <> 'N'"))
            {
                Alert.Show("定数调拨使用的部分定数码已被使用，请检查！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            
            if (BillOper(strBill, "AUDIT") == 1)
            {
                billLockDoc(true);
                Alert.Show("单据【" + strBill + "】审核成功！");
                OperLog("科室调拨", "审核单据【" + docBILLNO.Text.Trim() + "】");
                billOpen(strBill);
            }
        }

        protected override void listRow_DoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            billOpen(GridList.Rows[e.RowIndex].Values[1].ToString());
            PubFunc.FormLock(FormDoc, true, "");
            if (docFLAG.SelectedValue == "N")
            {
                //tbxINSERT.Enabled = true;
                docMEMO.Enabled = true;
            }
        }

        protected override void billOpen(string strBillno)
        {
            DataTable dtDoc = DbHelperOra.Query(string.Format(strDocSql, strBillno)).Tables[0];

            PubFunc.FormDataSet(FormDoc, dtDoc.Rows[0]);

            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            DataTable dtBill = DbHelperOra.Query(string.Format(strComSql, strBillno)).Tables[0];
            decimal bzslTotal = 0, feeTotal = 0;
            if (dtBill != null && dtBill.Rows.Count > 0)
            {
                Doc.GridRowAdd(GridGoods, dtBill);
                foreach (DataRow row in dtBill.Rows)
                {
                    bzslTotal += Convert.ToDecimal(row["BZSL"]);
                    feeTotal += Convert.ToDecimal(row["HSJJ"]) * Convert.ToDecimal(row["BZSL"]) * Convert.ToDecimal(row["BZHL"]);
                }
            }
            //计算合计数量
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("BZSL", bzslTotal.ToString());
            summary.Add("HSJE", feeTotal.ToString("F2"));
            GridGoods.SummaryData = summary;
            TabStrip1.ActiveTabIndex = 1;
            //根据状态屏蔽按钮
            if (docFLAG.SelectedValue == "N")
            {
                docMEMO.Enabled = true;
                btnPrint.Enabled = false;
                btnDel.Enabled = true;
                btnSave.Enabled = true;
                btnAudit.Enabled = true;
                btnDelRow.Enabled = true;
                btnGoods.Enabled = true;
            }
            else if (docFLAG.SelectedValue == "Y")
            {
                btnPrint.Enabled = true;
                btnDel.Enabled = false;
                btnSave.Enabled = false;
                btnAudit.Enabled = false;
                btnDelRow.Enabled = false;
                btnGoods.Enabled = false;
            }
            else
            {
                btnPrint.Enabled = false;
                btnDel.Enabled = false;
                btnSave.Enabled = false;
                btnAudit.Enabled = false;
                btnDelRow.Enabled = false;
            }
        }

        protected override void billSave()
        {
            save();
        }

        private void save(string flag = "N")
        {
            #region 数据有效性验证
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("非新单不能保存！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().OrderBy(x => x["GDSEQ"]).ToList();
            if (newDict.Count == 0)
            {
                Alert.Show("请输入商品信息", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            List<Dictionary<string, object>> goodsData = new List<Dictionary<string, object>>();
            //判断是否有空行
            for (int i = 0; i < newDict.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(newDict[i]["GDSEQ"].ToString()))
                {
                    if (newDict[i]["BZSL"].ToString() == "0" || string.IsNullOrWhiteSpace(newDict[i]["BZSL"].ToString()))
                    {
                        Alert.Show("第【" + (i + 1) + "】行【调拨数量】为空，请填写！");
                        return;
                    }

                    object DSBZSL = DbHelperOra.GetSingle(string.Format(@"SELECT DSHL FROM DAT_GOODSDS_LOG 
                    WHERE GDSEQ = '{0}' AND DEPTIN = '{1}' AND BARCODE = '{2}'", newDict[i]["GDSEQ"].ToString(), docDEPTOUT.SelectedValue, newDict[i]["DSCODE"].ToString()));

                    object FDSKC = DbHelperOra.GetSingle(string.Format(@"SELECT (A.KCSL - NVL(B.DSSL, 0)) KSSPSL
                          FROM (SELECT GDSEQ, SUM(KCSL) KCSL
                                  FROM DAT_GOODSSTOCK A
                                 WHERE KCSL > 0
                                   AND A.DEPTID = '{1}'
                                   AND A.GDSEQ = '{0}'
                                 GROUP BY GDSEQ) A,
                               (SELECT GDSEQ, SUM(DSHL) DSSL
                                  FROM DAT_GOODSDS_LOG
                                 WHERE DEPTIN = '{1}'
                                   AND FLAG = 'N'
                                   AND SL > 0
                                   AND DSHL > 0           
                                   AND GDSEQ = '{0}'
                                 GROUP BY GDSEQ) B
                                 WHERE A.GDSEQ = B.GDSEQ(+)", newDict[i]["GDSEQ"].ToString(), docDEPTOUT.SelectedValue));
                    if (string.IsNullOrWhiteSpace(newDict[i]["PH"].ToString()))
                    {
                        Alert.Show("第【" + (i + 1) + "】行【批号】为空，请填写！");
                        return;
                    }
                    //if (newDict[i]["ISDS"].ToString() == "定数")
                    if (!string.IsNullOrWhiteSpace(newDict[i]["DSCODE"].ToString()))
                    {
                        //if (newDict[i]["BZSL"].ToString() != newDict[i]["DSBZSL"].ToString())
                        if (newDict[i]["BZSL"].ToString() != DSBZSL.ToString())
                        {
                            //newDict[i]["BZSL"] = newDict[i]["DSBZSL"].ToString();
                            Alert.Show("第【" + (i + 1) + "】行为定数，【调拨数量】不允许由【" + DSBZSL.ToString() + "】调整为【" + newDict[i]["BZSL"].ToString() + "】，请修改！");
                            return;
                        }
                    }
                    else
                    {
                        if (Convert.ToDecimal(newDict[i]["BZSL"].ToString()) > Convert.ToDecimal(FDSKC.ToString()))
                        {
                            //newDict[i]["BZSL"] = newDict[i]["DSBZSL"].ToString();
                            Alert.Show("第【" + (i + 1) + "】行为非定数，不允许大于非定数库存数量【" + FDSKC.ToString() + "】，请修改！");
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
            //验证单据信息
            if (DbHelperOra.Exists("SELECT 1 FROM dat_kd_doc where seqno = '" + docBILLNO.Text + "'") && docBILLNO.Enabled)
            {
                Alert.Show("您输入的单号已存在,请检查!");
                return;
            }
            #endregion

            if (PubFunc.StrIsEmpty(docBILLNO.Text))
            {
                docSEQNO.Text = BillSeqGet();
                //重新拼单号
                docSEQNO.Text = "KSD" + docSEQNO.Text.Substring(3, docSEQNO.Text.Length - 3);
                docBILLNO.Text = docSEQNO.Text;
                docBILLNO.Enabled = false;
            }
            MyTable mtType = new MyTable("DAT_KD_DOC");
            mtType.ColRow = PubFunc.FormDataHT(FormDoc);
            mtType.ColRow["SEQNO"] = docBILLNO.Text;
            mtType.ColRow.Add("BILLTYPE", "KSD");
            mtType.ColRow.Add("SUBNUM", goodsData.Count);
            mtType.ColRow.Add("XSTYPE", "1");
            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtTypeMx = new MyTable("DAT_KD_COM");
            decimal subNum = 0;//总金额
            //先删除单据信息在插入
            cmdList.Add(mtType.DeleteCommand(""));//删除单据台头
            cmdList.Add(new CommandInfo("DELETE DAT_KD_COM WHERE SEQNO='" + docBILLNO.Text + "'", null));//删除单据明细
            for (int i = 0; i < goodsData.Count; i++)
            {
                mtTypeMx.ColRow = PubFunc.GridDataGet(goodsData[i]);
                mtTypeMx.ColRow.Add("SEQNO", docBILLNO.Text);
                mtTypeMx.ColRow["ROWNO"] = i + 1;
                mtTypeMx.ColRow.Add("PHID", mtTypeMx.ColRow["PH"]);
                mtTypeMx.ColRow.Add("XSSL", Convert.ToInt32(mtTypeMx.ColRow["BZHL"]) * Convert.ToInt32(mtTypeMx.ColRow["BZSL"]));
                mtTypeMx.ColRow["HSJE"] = Convert.ToDecimal(mtTypeMx.ColRow["XSSL"]) * Convert.ToDecimal(mtTypeMx.ColRow["HSJJ"]);
                mtTypeMx.ColRow.Add("BHSJJ", 0);
                mtTypeMx.ColRow.Add("BHSJE", 0);
                mtTypeMx.ColRow.Remove("UNITNAME");
                mtTypeMx.ColRow.Remove("PRODUCERNAME");
                subNum = subNum + decimal.Parse(mtTypeMx.ColRow["HSJE"].ToString());
                cmdList.Add(mtTypeMx.Insert());
            }
            mtType.ColRow.Add("SUBSUM", subNum);
            cmdList.AddRange(mtType.InsertCommand());
            DbHelperOra.ExecuteSqlTran(cmdList);
            if(flag == "N")
                Alert.Show("科室商品调拨信息保存成功！");
            //billNew();
            OperLog("科室调拨", "修改单据【" + docBILLNO.Text.Trim() + "】");
            billOpen(docBILLNO.Text);
            btnDel.Enabled = true;
            billLockDoc(true);
            SaveSuccess = true;
        }

        /// <summary>
        /// FineUIPro.Grid控件数据加载
        /// </summary>
        /// <param name="row">要加载的行数据</param>
        /// <param name="firstRow">是否插入指定行</param>
        /// <param name="flag">数据来源：NEW-从数据库中获得，用于商品新增时；OLD-从销售单据明细中获得，用于修改或审核时</param>
        private void LoadGridRow(DataRow row, bool firstRow = true, string flag = "NEW")
        {
            row["HSJE"] = Convert.ToDecimal(row["HSJJ"]) * Convert.ToDecimal(row["BZSL"]) * Convert.ToDecimal(row["BZHL"]);
            PubFunc.GridRowAdd(GridGoods, row, false);
        }

        protected void Window1_Close(object sender, WindowCloseEventArgs e)
        {
            DataTable dt = GetGoods(hfdValue.Text);
            if (dt != null && dt.Rows.Count > 0)
            {
                dt.Columns["PIZNO"].ColumnName = "PZWH";

                dt.Columns.Add("PH", Type.GetType("System.String"));
                dt.Columns.Add("RQ_SC", Type.GetType("System.String"));
                dt.Columns.Add("YXQZ", Type.GetType("System.String"));
                dt.Columns.Add("MEMO", Type.GetType("System.String"));
                dt.Columns.Add("BZSL", Type.GetType("System.Int32"));
                dt.Columns.Add("DHSL", Type.GetType("System.Int32"));
                dt.Columns.Add("HSJE", Type.GetType("System.Double"));


                foreach (DataRow row in dt.Rows)
                {
                    row["BZSL"] = "1";
                    row["DHSL"] = "0";
                    row["HSJE"] = "0";
                    LoadGridRow(row, false);
                }
            }
            else
            {
                Alert.Show("系统传值错误！！！", "消息提示", MessageBoxIcon.Warning);
            }
        }
        protected void btnClosePostBack_Click(object sender, EventArgs e)
        {
            int[] selections = GridLot.SelectedRowIndexArray;
            foreach (int i in selections)
            {
                string[] strCell = GridGoods.SelectedCell;
                JObject defaultObj = Doc.GetJObject(GridGoods, strCell[0]);
                defaultObj["PH"] = GridLot.DataKeys[i][0].ToString();
                defaultObj["YXQZ"] = GridLot.DataKeys[i][1].ToString();
                defaultObj["RQ_SC"] = GridLot.DataKeys[i][2].ToString();
                PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(strCell[0], defaultObj));
            }
            WindowLot.Hidden = true;
        }
        protected override void billDel()
        {
            if (docBILLNO.Text.Trim() == "")
            {
                Alert.Show("请选择需要删除的单据");
                return;
            }
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("非新单不能删除!");
                return;
            }
            DbHelperOra.ExecuteSql("Delete from DAT_KD_DOC t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            DbHelperOra.ExecuteSql("Delete from DAT_KD_COM t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            Alert.Show("单据删除成功!");
            OperLog("科室调拨", "删除单据【" + docBILLNO.Text.Trim() + "】");
            PubFunc.FormDataClear(FormDoc);
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                return;
            }
            billSearch();
            billNew();
        }

        protected override void billExport()
        {
            if (string.IsNullOrWhiteSpace(docSEQNO.Text))
            {
                Alert.Show("请先选择要导出的订单信息！");
                return;
            }
            string sql = @"SELECT A.BILLNO 单据编号,
                                   F_GETDEPTNAME(A.DEPTOUT) 调出科室,
                                   F_GETDEPTNAME(A.DEPTID) 调入科室,                                   
                                   F_GETUSERNAME(A.LRY) 操作员,
                                   TO_CHAR(A.XSRQ, 'YYYY-MM-DD') 调拨日期,
                                   A.MEMO 备注说明,
                                   B.GDSEQ 商品编码,
                                   B.ROWNO 行号,
                                   B.GDNAME 商品名称,
                                   B.GDSPEC 商品规格,
                                   B.BZSL 包装数量,
                                   F_GETUNITNAME(B.UNIT) 包装单位,
                                   B.BZHL 包装含量,
                                   B.DSCODE 扫描条码,
                                   B.HSJJ 含税进价,
                                   B.HSJE 含税金额,
                                   B.ZPBH 制品编号,
                                   F_GETPRODUCERNAME(B.PRODUCER) 生产厂家,
                                   B.HWID 货位,
                                   B.PHID 批号,
                                   B.YXQZ 有效期至,                                   
                                   B.PZWH 注册证号,
                                   B.RQ_SC 生产日期,
                                   B.MEMO 商品明细备注
                              FROM DAT_KD_DOC A, DAT_KD_COM B,DOC_GOODS G
                             WHERE B.SEQNO = '{0}' AND B.GDSEQ=G.GDSEQ
                               AND A.SEQNO = B.SEQNO
                             ORDER BY ROWNO";
            DataTable dt = DbHelperOra.Query(string.Format(sql, docSEQNO.Text)).Tables[0];
            ExcelHelper.ExportByWeb(dt, string.Format("【{0}】科室商品调拨信息", docDEPTID.SelectedText), "科室商品调拨信息导出_" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
        }

        protected override void billCopy()
        {
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

        private void DataSearch()
        {
            int total = 0;
            string strSearch = "";
            //非定数
            string sql = string.Format(@"SELECT * FROM
                                        (SELECT (A.KCSL - NVL(B.DSSL,0)) KSSPSL,'非定数' isds,
                                               F_GETUNITNAME(C.UNIT) UNITNAME,
                                               F_GETPRODUCERNAME(C.PRODUCER) PRODUCERNAME,
                                               F_GETSUPNAME(C.SUPPLIER) SUPPLIERNAME,
                                               '' DSCODE,C.GDSEQ,C.GDNAME,C.GDSPEC,C.BZHL,C.HSJJ,C.ZPBH,'{0}' HWID,C.JXTAX,C.PIZNO PZWH,C.PRODUCER,C.UNIT,C.ZJM,C.BARCODE
                                        FROM (SELECT GDSEQ,SUM(KCSL) KCSL FROM DAT_GOODSSTOCK A WHERE KCSL>0 AND A.DEPTID ='{0}' GROUP BY GDSEQ) A,
                                            (SELECT GDSEQ,SUM(DSHL) DSSL FROM DAT_GOODSDS_LOG WHERE DEPTIN = '{0}' AND FLAG = 'N' AND SL > 0 AND DSHL>0 GROUP BY GDSEQ) B,
                                            DOC_GOODS C
                                        WHERE A.GDSEQ = B.GDSEQ(+) AND C.GDSEQ = A.GDSEQ  AND C.FLAG IN('Y','T') AND C.ISGZ = 'N' AND A.KCSL> NVL(B.DSSL,0)
                                        AND EXISTS (SELECT 1 FROM DOC_GOODSCFG WHERE DEPTID = '{1}' AND GDSEQ = C.GDSEQ)", docDEPTOUT.SelectedValue, docDEPTID.SelectedValue);


            sql += string.Format(@" union all
                select PZ.DSHL,'定数',
                                    F_GETUNITNAME(SP.UNIT) UNITNAME,
                                    F_GETPRODUCERNAME(SP.PRODUCER) PRODUCERNAME,
                                    F_GETSUPNAME(SUPPLIER) SUPPLIERNAME,
                                    PZ.BARCODE DSCODE,SP.GDSEQ,SP.GDNAME,SP.GDSPEC,SP.BZHL,SP.HSJJ,SP.ZPBH,'{0}' HWID,SP.JXTAX,SP.PIZNO PZWH,SP.PRODUCER,SP.UNIT,SP.ZJM,SP.BARCODE
                               from DOC_GOODS SP, DAT_GOODSDS_LOG PZ
                               where SP.gdseq = PZ.gdseq  AND SP.ISGZ = 'N' AND SP.FLAG IN('Y','T') AND PZ.FLAG='N' AND PZ.DEPTIN = '{0}' AND PZ.SL > 0 AND PZ.DSHL>0
                               AND EXISTS(SELECT 1 FROM DOC_GOODSCFG WHERE DEPTID = '{1}' AND GDSEQ = SP.GDSEQ))", docDEPTOUT.SelectedValue, docDEPTID.SelectedValue);
            if (!string.IsNullOrWhiteSpace(trbSearch.Text))
            {
                strSearch = string.Format(" WHERE (GDSEQ LIKE '%{0}%' OR GDNAME LIKE '%{0}%' OR ZJM LIKE '%{0}%' OR BARCODE LIKE '%{0}%')", trbSearch.Text.ToUpper());
            }
            sql += strSearch;

            StringBuilder strSql = new StringBuilder(sql);
            strSql.AppendFormat(" ORDER BY {0} {1}", GoodsInfo.SortField, GridGoods.SortDirection);
            DataTable dtData = PubFunc.DbGetPage(GoodsInfo.PageIndex, GoodsInfo.PageSize, strSql.ToString(), ref total);
            GoodsInfo.RecordCount = total;
            GoodsInfo.DataSource = dtData;
            GoodsInfo.DataBind();
        }

        protected void btnGoods_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(docDEPTOUT.SelectedValue) || string.IsNullOrWhiteSpace(docDEPTID.SelectedValue))
            {
                Alert.Show("表单字段为空，请补充完整后再追加商品！");
                return;
            }
            if (docDEPTOUT.SelectedValue == docDEPTID.SelectedValue)
            {
                Alert.Show("调出科室和调入科室不能为同一个，请重新选择！");
                return;
            }
            PubFunc.FormLock(FormDoc, true, "");
            WindowGoods.Hidden = false;
            DataSearch();
        }

        protected void GoodsInfo_PageIndexChange(object sender, GridPageEventArgs e)
        {

        }

        protected void GoodsInfo_Sort(object sender, GridSortEventArgs e)
        {

        }

        protected void GoodsInfo_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            string strIsds = GoodsInfo.Rows[e.RowIndex].Values[1].ToString();
            string strCode = GoodsInfo.Rows[e.RowIndex].Values[2].ToString();
            string strDsCode = GoodsInfo.Rows[e.RowIndex].Values[6].ToString();
            DataTable table = PubFunc.GridDataGet(GoodsInfo);
            DataView dv = table.DefaultView;
            if (string.IsNullOrWhiteSpace(strDsCode))
            {
                dv.RowFilter = "GDSEQ = '" + strCode + "' AND ISDS = '" + strIsds + "'";
            }
            else
            {
                dv.RowFilter = "GDSEQ = '" + strCode + "' AND ISDS = '" + strIsds + "' AND DSCODE = '" + strDsCode + "'";
            }

            //DataTable dt = DbHelperOra.Query(strSql).Tables[0];
            DataGridBack(dv.ToTable());
        }

        protected void trbSearch_TriggerClick(object sender, EventArgs e)
        {
            DataSearch();
        }

        private void DataGridBack(DataTable dt)
        {
            string msg = "";

            if (dt != null && dt.Rows.Count > 0)
            {
                string someDjbh = string.Empty;
                bool getDjbh = false;

                dt.Columns.Add("BZSL", Type.GetType("System.Int32"));
                dt.Columns.Add("PH", Type.GetType("System.String"));
                dt.Columns.Add("YXQZ", Type.GetType("System.String"));
                dt.Columns.Add("RQ_SC", Type.GetType("System.String"));
                foreach (DataRow row in dt.Rows)
                {
                    if (row["ISDS"].ToString() == "定数")
                    {
                        string sSQL = string.Format("SELECT * FROM DAT_CK_COM WHERE STR2='{0}'",row["DSCODE"].ToString());
                        DataTable Temp = DbHelperOra.Query(sSQL).Tables[0];
                        if (Temp.Rows.Count > 0)
                        {

                            row["PH"] = (Temp.Rows[0]["PH"] ?? "");
                            row["YXQZ"] = (Temp.Rows[0]["YXQZ"] ?? "");
                            row["RQ_SC"] = (Temp.Rows[0]["RQ_SC"] ?? "");
                            row["HWID"] = (Temp.Rows[0]["HWID"] ?? "");
                        }
                        row["BZSL"] = row["KSSPSL"];
                        row["DSBZSL"] = row["KSSPSL"];
                        row["HSJE"] = Convert.ToDecimal(row["HSJJ"]) * Convert.ToDecimal(row["BZSL"]) * Convert.ToDecimal(row["BZHL"]);
                        decimal jingdu = 0;
                        decimal bzhl = 0;
                        if (decimal.TryParse(row["HSJJ"].ToString(), out jingdu) && decimal.TryParse(row["BZHL"].ToString(), out bzhl)) { row["HSJJ"] = Math.Round(jingdu * bzhl, 4).ToString("F4"); }

                        if (decimal.TryParse(row["HSJE"].ToString(), out jingdu)) { row["HSJE"] = Math.Round(jingdu, 2).ToString("F2"); }
                        docMEMO.Enabled = true;
                        List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().OrderBy(x => x["GDSEQ"]).ToList();
                        int sameRowCount = newDict.Where(a => a["DSCODE"].ToString() == row["DSCODE"].ToString()).Count();
                        if (sameRowCount > 0)
                        {
                            someDjbh += "【" + row["GDNAME"].ToString() + "】";
                            getDjbh = true;
                        }
                        else
                        {
                            PubFunc.GridRowAdd(GridGoods, row, false);
                        }
                    }
                    else
                    {
                        string sSQL = string.Format("SELECT * FROM (SELECT A.HWID,SUM((A.KCSL - A.LOCKSL)) KCSL,A.PH,A.YXQZ,A.RQ_SC,B.ISGZ,B.GDNAME FROM DAT_GOODSSTOCK A ,DOC_GOODS B WHERE A.DEPTID ='{0}' AND A.GDSEQ = '{1}'  AND A.GDSEQ = B.GDSEQ AND A.KCSL >0 GROUP BY HWID,PH,YXQZ,RQ_SC,ISGZ,GDNAME ORDER BY PH ASC) WHERE ROWNUM = 1 ", docDEPTOUT.SelectedValue, row["GDSEQ"].ToString());
                        DataTable Temp = DbHelperOra.Query(sSQL).Tables[0];
                        row["BZSL"] = "0";
                        row["DSBZSL"] = "0";
                        
                        if (Temp.Rows.Count > 0)
                        {
                            
                            row["PH"] = (Temp.Rows[0]["PH"] ?? "");
                            row["YXQZ"] = (Temp.Rows[0]["YXQZ"] ?? "");
                            row["RQ_SC"] = (Temp.Rows[0]["RQ_SC"] ?? "");
                            row["HWID"] = (Temp.Rows[0]["HWID"] ?? "");
                        }
                        decimal jingdu = 0;
                        decimal bzhl = 0;
                        if (decimal.TryParse(row["HSJJ"].ToString(), out jingdu) && decimal.TryParse(row["BZHL"].ToString(), out bzhl)) { row["HSJJ"] = Math.Round(jingdu * bzhl, 4).ToString("F4"); }

                        if (decimal.TryParse(row["HSJE"].ToString(), out jingdu)) { row["HSJE"] = Math.Round(jingdu, 2).ToString("F2"); }
                        docMEMO.Enabled = true;
                        List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().OrderBy(x => x["GDSEQ"]).ToList();
                        int sameRowCount = newDict.Where(a => a["GDSEQ"].ToString() == row["GDSEQ"].ToString() && a["DSCODE"].ToString().Length == 0).Count();
                        if (sameRowCount > 0)
                        {
                            someDjbh += "【" + row["GDNAME"].ToString() + "】";
                            getDjbh = true;
                        }
                        else
                        {
                            PubFunc.GridRowAdd(GridGoods, row, false);
                        }
                    }

                }
                if (getDjbh)
                {
                    Alert.Show("商品名称：" + someDjbh + "申请明细中已存在", "消息提示", MessageBoxIcon.Warning);
                }
            }
            else
            {
                Alert.Show("请先选择要加载的商品信息！", "消息提示", MessageBoxIcon.Warning);
            }
            WindowGoods.Hidden = true;
        }


        protected void btnSure_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            if (GoodsInfo.SelectedRowIndexArray.Length == 0)
            {
                Alert.Show("请选择商品信息！", "警告提醒", MessageBoxIcon.Warning);
                return;
            }
            FineUIPro.GridRowCollection rows = new FineUIPro.GridRowCollection();
            for (int i = GoodsInfo.SelectedRowIndexArray.Length - 1; i > -1; i--)
            {
                rows.Add(GoodsInfo.Rows[GoodsInfo.SelectedRowIndexArray[i]]);
            }
            dt = GridDataGet(GoodsInfo, rows);

            DataGridBack(dt);
        }

        /// <summary>
        /// FineUIPro.Grid控件的数据转换成DataTable
        /// </summary>
        /// <param name="grid">Grid控件</param>
        /// <returns>DataTable数据源</returns>
        public static DataTable GridDataGet(FineUIPro.Grid grid, FineUIPro.GridRowCollection rows)
        {
            int rowCount = 0;
            DataTable dtGrid = new DataTable();
            foreach (GridColumn gc in grid.Columns)
            {
                if (gc is FineUIPro.BoundField)
                {
                    dtGrid.Columns.Add(new DataColumn(((FineUIPro.BoundField)(gc)).DataField.ToUpper(), typeof(string)));
                }
                else if (gc is FineUIPro.RenderField)
                {
                    dtGrid.Columns.Add(new DataColumn(((FineUIPro.RenderField)(gc)).DataField.ToUpper(), typeof(string)));
                }
                else if (gc is FineUIPro.TemplateField)
                {
                    dtGrid.Columns.Add(new DataColumn(((FineUIPro.TemplateField)(gc)).ColumnID.ToUpper(), typeof(string)));
                }
                rowCount++;
            }
            if (rowCount > 0)
            {
                foreach (GridRow gr in rows)
                {
                    DataRow row = dtGrid.NewRow();
                    foreach (GridColumn gc in grid.Columns)
                    {
                        if (gc is FineUIPro.BoundField)
                        {
                            row[((FineUIPro.BoundField)(gc)).DataField.ToUpper()] = gr.Values[gc.ColumnIndex];
                        }
                        if (gc is FineUIPro.RenderField)
                        {
                            row[((FineUIPro.RenderField)(gc)).DataField.ToUpper()] = gr.Values[gc.ColumnIndex];
                        }
                        else if (gc is FineUIPro.TemplateField)
                        {
                            row[((FineUIPro.TemplateField)(gc)).ColumnID.ToUpper()] = gr.Values[gc.ColumnIndex];
                        }
                    }
                    dtGrid.Rows.Add(row);
                }
            }
            return dtGrid;
        }

        protected void btnClose1_Click(object sender, EventArgs e)
        {
            WindowGoods.Hidden = true;
        }

        //protected void btnPostBack_Click(object sender, EventArgs e)
        //{
        //    string GoodsInfo = "";
        //    int[] row = GridGoods.SelectedRowIndexArray;
        //    foreach (int index in row)
        //    {
        //        GoodsInfo += GetRowValue(GridGoods.Rows[index]) + "♂";
        //    }
        //    GridGoods.SelectedRowIndexArray = new int[] { };
        //    FineUIPro.PageContext.RegisterStartupScript("PosGoods('" + GoodsInfo.TrimEnd(';') + "')");
        //}
    }
}