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
using Oracle.ManagedDataAccess.Client;

namespace ERPProject.ERPReapt
{
    public partial class DepartmentReturns : BillBase
    {
        private string strDocSql = "SELECT A.* FROM DAT_CK_DOC A WHERE SEQNO ='{0}'";
        public override Field[] LockControl
        {
            get { return new Field[] { docDEPTID, docSLR, docTHTYPE, docDEPTOUT, docXSRQ, docMEMO }; }
        }

        public DepartmentReturns()
        {
            BillType = "LTD";
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
                    if (Request.QueryString["oper"].ToString() == "input")
                    {
                        ButtonHidden(btnAudit, btnCancel, btnBack);
                    }
                    else if (Request.QueryString["oper"].ToString() == "audit")
                    {
                        billLockDoc(true);
                        ButtonHidden(btnNew, btnSave, btnCopy, btnAddRow, btnDelRow, btnSumit, btnDel, btnGoods);
                        TabStrip1.ActiveTabIndex = 0;
                    }
                }
            }
        }
        private void DataInit()
        {
            DepartmentBind.BindDDL("DDL_SYS_DEPTRANGE", UserAction.UserID, lstDEPTID);
            DepartmentBind.BindDDL("DDL_SYS_DEPTRANGE", UserAction.UserID, docDEPTID);
            PubFunc.DdlDataGet("DDL_USER", lstLRY, lstSLR, docLRY, docSLR);
            PubFunc.DdlDataGet("DDL_SYS_DEPOT", lstDEPTOUT, docDEPTOUT);
            PubFunc.DdlDataGet(ddlReject, "DDL_BILL_REASON");
            PubFunc.DdlDataGet("DDL_KSST", lstFLAG, docFLAG);
            PubFunc.DdlDataGet(docTHTYPE, "DDL_RETURNREASON");


            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
        }

        protected override void billNew()
        {
            string strDept = docDEPTID.SelectedValue;
            string strDeptOut = docDEPTOUT.SelectedValue;
            //lstDEPTID.SelectedIndex = 1;
            //lstDEPTOUT.SelectedIndex = 1;
            docSEQNO.Enabled = true;
            docBILLNO.Text = docSEQNO.Text = "";
            PubFunc.FormDataClear(FormDoc);
            if (PubFunc.StrIsEmpty(strDept))
            {
                if (docDEPTID.Items.Count > 1)
                    strDept = docDEPTID.Items[1].Value;
            }
            if (PubFunc.StrIsEmpty(strDeptOut))
            {
                if (docDEPTOUT.Items.Count > 1)
                    strDeptOut = docDEPTOUT.Items[1].Value;
            }
            docFLAG.SelectedValue = "M";
            docSLR.SelectedValue = UserAction.UserID;
            docLRY.SelectedValue = UserAction.UserID;
            docLRRQ.SelectedDate = DateTime.Now;
            docXSRQ.SelectedDate = DateTime.Now;
            docDEPTID.SelectedValue = strDept;
            docDEPTOUT.SelectedValue = strDeptOut;

            billLockDoc(false);
            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            btnGoods.Enabled = true;
            btnSave.Enabled = true;
            btnDel.Enabled = false;
            btnSumit.Enabled = false;
            btnAudit.Enabled = false;
            btnCancel.Enabled = false;
            btnPrint.Enabled = false;
            btnDelRow.Enabled = true;
            zsmScan.Enabled = true;
            zsmDelete.Enabled = true;
            tbxBARCODE.Enabled = true;
            btnBack.Enabled = false;
            tbxBARCODE.Focus();
        }
        protected void GridList_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                string flag = row["FLAG"].ToString();
                FineUIPro.BoundField flagcol = GridList.FindColumn("FLAG") as FineUIPro.BoundField;
                if (flag == "新单")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color1";
                }
                else if (flag == "已提交")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color2";
                }
                else if (flag == "已驳回")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color3";
                }
            }
        }

        private JObject GetJObject(Dictionary<string, object> dicRecord)
        {
            JObject defaultObj = new JObject();
            foreach (string key in dicRecord.Keys)
            {
                defaultObj.Add(key, (dicRecord[key] ?? "").ToString());
            }

            decimal hl = 0, rs = 0, jg = 0;
            decimal.TryParse(dicRecord["BZHL"].ToString(), out hl);//包装含量
            decimal.TryParse(dicRecord["BZSL"].ToString(), out rs);//订货数
            decimal.TryParse(dicRecord["HSJJ"].ToString(), out jg);//价格

            defaultObj.Remove("DHSL");
            defaultObj.Remove("HSJE");
            defaultObj.Add("DHSL", rs * hl);


            //处理金额格式
            string jingdu = Math.Round(rs * jg, 4).ToString("F2");
            defaultObj.Add("HSJE", jingdu);

            return defaultObj;
        }
        protected void GridGoods_AfterEdit(object sender, FineUIPro.GridAfterEditEventArgs e)
        {
            string[] strCell = GridGoods.SelectedCell;
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
            if (newDict.Count == 0) return;
            if (e.ColumnID == "BZSL")
            {
                //string cell = string.Format("[{0},{1}]", e.RowIndex, intCell[1]);
                if (!PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "BZHL")) || !PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "BZSL")) || !PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "HSJJ")))
                {
                    Alert.Show("商品信息异常，请详细检查商品信息：包装含量、价格或数量！", "异常信息", MessageBoxIcon.Warning);
                    return;
                }
                JObject defaultObj = Doc.GetJObject(GridGoods, e.RowID);
                decimal hl = 0, rs = 0, jg = 0;
                decimal.TryParse((defaultObj["BZHL"] ?? "0").ToString(), out hl);
                decimal.TryParse((defaultObj["BZSL"] ?? "0").ToString(), out rs);
                decimal.TryParse((defaultObj["HSJJ"] ?? "0").ToString(), out jg);
                defaultObj["DHSL"] = rs * hl;
                defaultObj["HSJE"] = Math.Round(rs * jg, 2).ToString("F2");
                PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(e.RowID, defaultObj));
                //计算合计数量
                decimal bzslTotal = 0, feeTotal = 0, dhslTotal = 0, kcslTotal = 0;
                foreach (Dictionary<string, object> dic in newDict)
                {
                    bzslTotal += Convert.ToDecimal(dic["BZSL"] ?? "0");
                    dhslTotal += Convert.ToDecimal(dic["BZHL"] ?? "0") * Convert.ToDecimal(dic["BZSL"] ?? "0");
                    kcslTotal += Convert.ToDecimal(dic["KCSL"] ?? "0");
                    feeTotal += Convert.ToDecimal(dic["HSJJ"] ?? "0") * Convert.ToDecimal(dic["BZSL"] ?? "0");
                }
                JObject summary = new JObject();
                summary.Add("GDNAME", "本页合计");
                summary.Add("BZSL", bzslTotal.ToString());
                summary.Add("DHSL", dhslTotal.ToString());
                summary.Add("KCSL", kcslTotal.ToString());
                summary.Add("HSJE", feeTotal.ToString("F2"));

                GridGoods.SummaryData = summary;
            }
        }

        protected override void billClear()
        {
            PubFunc.FormDataClear(Formlist);
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
            GridList.DataSource = null;
            GridList.DataBind();
        }

        protected override void billAddRow()
        {
            if ((",R,M").IndexOf(docFLAG.SelectedValue) < 0)
            {
                Alert.Show("非『" + docFLAG.SelectedText + "』单据不能删行！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
            billLockDoc(true);
            PubFunc.GridRowAdd(GridGoods, "INIT");
        }

        protected override void billDelRow()
        {

            if ((",R,M").IndexOf(docFLAG.SelectedValue) < 0)
            {
                Alert.Show("非『" + docFLAG.SelectedText + "』单据不能删行！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            GridGoods.DeleteSelectedRows();
        }

        protected override void billGoods()
        {
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
            PubFunc.FormLock(FormDoc, true, "docMEMO,tbxBARCODE");
            //参数说明：cx-查询内容，bm-商品配置部门,su-供应商
            string url = "~/ERPQuery/GoodsWindow_New.aspx?Deptout=" + docDEPTOUT.SelectedValue + "&DeptIn" + docDEPTID.SelectedValue;
            PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "商品信息查询"));
        }

        protected override void billSearch()
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【申领日期】！");
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                Alert.Show("【开始日期】大于【结束日期】，请重新输入！");
                return;
            }

            string strSql = @"SELECT A.SEQNO,A.BILLNO,B.NAME FLAG,F_GETDEPTNAME(A.DEPTID) DEPTID,A.XSRQ,F_GETDEPTNAME(A.DEPTOUT) DEPTOUT,
                                     A.SUBNUM,F_GETUSERNAME(A.SLR) SLR,F_GETUSERNAME(A.LRY) LRY,A.LRRQ,F_GETUSERNAME(A.SHR) SHR,A.SHRQ,A.MEMO 
                                from DAT_CK_DOC A, SYS_CODEVALUE B 
                                WHERE A.FLAG = B.CODE AND B.TYPE  ='DIC_BILLSTATUS' AND BILLTYPE='LTD' AND XSTYPE='2' ";
            string strSearch = "";

            if (Request.QueryString["oper"].ToString() == "audit")
            {

                strSql = strSql + " AND A.FLAG<>'M'";

            }
            else { strSql = strSql + " AND A.FLAG  IN ('M','N','R')"; }

            if (lstBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND A.BILLNO  LIKE '%{0}%'", lstBILLNO.Text);
            }
            if (lstFLAG.SelectedItem != null && lstFLAG.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.FLAG='{0}'", lstFLAG.SelectedItem.Value);
            }
            if (lstLRY.SelectedItem != null && lstLRY.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.lry='{0}'", lstLRY.SelectedItem.Value);
            }
            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedItem.Value);
            }
            if (lstDEPTOUT.SelectedItem != null && lstDEPTOUT.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND DEPTOUT='{0}'", lstDEPTOUT.SelectedItem.Value);
            }
            strSearch += string.Format(" AND deptid in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            strSearch += string.Format(" AND A.LRRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);//type <>'1' and
            strSearch += string.Format(" AND A.LRRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY DECODE(A.FLAG,'M','1','N','2','Y','3','4'),A.BILLNO DESC";
            GridList.DataSource = DbHelperOra.Query(strSql).Tables[0];
            GridList.DataBind();
        }

        protected override void billAudit()
        {
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("非新单不能审核！");
                return;
            }
            //验证是否盘点
            if (DbHelperOra.Exists("SELECT 1 FROM DAT_PD_LOCK WHERE DEPTID = '" + docDEPTOUT.SelectedValue + "' AND FLAG='N'"))
            {
                Alert.Show(string.Format("部门【{0}】正在盘点,请盘点后进行操作！", docDEPTOUT.SelectedText), "警告提示", MessageBoxIcon.Warning);
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
            if (BillOper(strBillno, "AUDIT") == 1)
            {
                billLockDoc(true);
                Alert.Show("单据【" + strBillno + "】审核成功！");
                OperLog("科室申退", "审核单据【" + strBillno + "】");
                billOpen(strBillno);
            }
        }

        protected override void billCancel()
        {
            if (docBILLNO.Text.Trim() == "")
            {
                Alert.Show("请选择需要驳回的单据！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            object flag = DbHelperOra.GetSingle("SELECT NVL(FLAG,'M') FROM DAT_CK_DOC WHERE SEQNO='" + docBILLNO.Text.Trim() + "'");
            if (flag != null && (",N").IndexOf(flag.ToString()) < 0)
            {
                Alert.Show("非『已提交』单据不能驳回！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            WindowReject.Hidden = false;


        }

        protected override void listRow_DoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            billOpen(GridList.Rows[e.RowIndex].Values[1].ToString());
        }

        protected override void billOpen(string strBillno)
        {
            string sql = @"SELECT A.SEQNO,A.ROWNO,A.GDSEQ,A.BARCODE,A.GDNAME,A.UNIT,A.GDSPEC,A.GDMODE,A.HWID,A.BZHL,A.BZSL,F_GETKCGDSEQ(B.DEPTID, A.GDSEQ, A.PH, 'Y')DHSL,A.XSSL,A.JXTAX,A.HSJJ,A.BHSJJ,
                           A.HSJE,A.BHSJE,A.LSJ,A.LSJE,A.ISGZ,A.ISLOT,A.PHID,A.PH,A.PZWH,A.RQ_SC,A.YXQZ,A.PRODUCER,A.ZPBH,A.STR1,A.STR2,
                            A.STR3，A.NUM1，A.NUM2，A.NUM3，A.MEMO,A.STR4，A.STR5，A.NUM4，A.NUM5，A.ISJF,A.FPSL,A.FPUSER,A.FPDATE,A.STR6,A.STR7,A.STR7,A.STR8,A.STR9，A.STR10，A.SUPID,A.PSSID,
                                   F_GETUNITNAME(C.UNIT) UNITNAME,
                                   F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME,
                                   F_GETKCGDSEQ(B.DEPTID,A.GDSEQ,A.PH)KCSL,
                                   f_getunitname(C.UNIT) UNITSMALLNAME
                              FROM DAT_CK_COM A, DAT_CK_DOC B, DOC_GOODS C
                             WHERE A.SEQNO = B.SEQNO
                               AND A.SEQNO = '{0}'
                               AND A.GDSEQ = C.GDSEQ
                             ORDER BY A.ROWNO";
            DataTable dtDoc = DbHelperOra.Query(string.Format(strDocSql, strBillno)).Tables[0];
            PubFunc.FormDataSet(FormDoc, dtDoc.Rows[0]);

            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());

            decimal bzslTotal = 0, feeTotal = 0, dhslTotal = 0, kcslTotal = 0;
            DataTable dtBill = DbHelperOra.Query(string.Format(sql, strBillno)).Tables[0];
            if (dtBill != null && dtBill.Rows.Count > 0)
            {
                foreach (DataRow row in dtBill.Rows)
                {
                    //LoadGridRow(row, false, "OLD");
                    bzslTotal += Convert.ToDecimal(row["BZSL"] ?? "0");
                    dhslTotal += Convert.ToDecimal(row["BZHL"] ?? "0") * Convert.ToDecimal(row["BZSL"] ?? "0");
                    kcslTotal += Convert.ToDecimal(row["KCSL"] ?? "0");
                    feeTotal += Convert.ToDecimal(row["HSJJ"] ?? "0") * Convert.ToDecimal(row["BZSL"] ?? "0");
                }
                Doc.GridRowAdd(GridGoods, dtBill);
            }
            //计算合计数量
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("BZSL", bzslTotal.ToString());
            summary.Add("HSJE", feeTotal.ToString("F2"));
            summary.Add("KCSL", kcslTotal.ToString());
            summary.Add("DHSL", dhslTotal.ToString());
            GridGoods.SummaryData = summary;
            PubFunc.FormLock(FormDoc, true);
            if ((",M,R").IndexOf(docFLAG.SelectedValue) > 0)
            {
                tbxBARCODE.Enabled = true;
                docMEMO.Enabled = true;
            }
            TabStrip1.ActiveTabIndex = 1;

            string strFLAG = dtDoc.Rows[0]["FLAG"].ToString();

            if ((",R,M").IndexOf(strFLAG) > 0)
            {
                btnGoods.Enabled = true;
                btnSave.Enabled = true;
                btnDel.Enabled = true;
                btnSumit.Enabled = true;
                btnAudit.Enabled = false;
                btnCancel.Enabled = false;
                btnPrint.Enabled = false;
                btnDelRow.Enabled = true;
                zsmScan.Enabled = true;
                zsmDelete.Enabled = true;
                btnBack.Enabled = false;
            }
            else if (strFLAG.Equals("N"))
            {
                btnGoods.Enabled = false;
                btnSave.Enabled = false;
                btnDel.Enabled = false;
                btnSumit.Enabled = false;
                btnAudit.Enabled = true;
                btnCancel.Enabled = true;
                btnPrint.Enabled = false;
                btnDelRow.Enabled = false;
                zsmScan.Enabled = false;
                zsmDelete.Enabled = false;
                btnBack.Enabled = false;
            }
            else
            {
                btnGoods.Enabled = false;
                btnSave.Enabled = false;
                btnDel.Enabled = false;
                btnSumit.Enabled = false;
                btnAudit.Enabled = false;
                btnCancel.Enabled = false;
                btnPrint.Enabled = true;
                btnDelRow.Enabled = false;
                zsmScan.Enabled = false;
                zsmDelete.Enabled = false;
                btnBack.Enabled = true;
            }
        }

        protected override void billSave()
        {
            #region 数据有效性验证
            if ((",M,R").IndexOf(docFLAG.SelectedValue) < 0)
            {
                Alert.Show("非『" + docFLAG.SelectedText + "』单据不能保存！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().OrderBy(x => x["GDSEQ"]).ToList();
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
                    if (decimal.Parse(newDict[i]["KCSL"].ToString()) < decimal.Parse(newDict[i]["BZSL"].ToString()))
                    {
                        Alert.Show("商品『" + newDict[i]["GDNAME"].ToString() + "』退货数大于库存数，不能退货！", "消息提示", MessageBoxIcon.Warning);
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
                string flg = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M') FROM DAT_CK_DOC WHERE SEQNO='{0}'", docBILLNO.Text));
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
            mtType.ColRow.Add("XSTYPE", "2");
            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtTypeMx = new MyTable("DAT_CK_COM");
            //先删除单据信息在插入
            cmdList.Add(new CommandInfo("delete DAT_CK_DOC where seqno='" + docBILLNO.Text + "'", null));//删除单据台头
            cmdList.Add(new CommandInfo("delete DAT_CK_COM where seqno='" + docBILLNO.Text + "'", null));//删除单据明细
            cmdList.AddRange(mtType.InsertCommand());
            for (int i = 0; i < goodsData.Count; i++)
            {
                mtTypeMx.ColRow = PubFunc.GridDataGet(goodsData[i]);
                mtTypeMx.ColRow.Add("SEQNO", docBILLNO.Text);
                mtTypeMx.ColRow["ROWNO"] = i + 1;
                mtTypeMx.ColRow.Add("PHID", mtTypeMx.ColRow["PH"]);
                mtTypeMx.ColRow["DHSL"] = decimal.Parse(mtTypeMx.ColRow["BZHL"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                mtTypeMx.ColRow["HSJE"] = decimal.Parse(mtTypeMx.ColRow["HSJJ"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                //判断 申退数，为0时不能保存
                if (PubFunc.StrIsEmpty(mtTypeMx.ColRow["BZSL"].ToString()) || mtTypeMx.ColRow["BZSL"].ToString() == "0")
                {
                    Alert.Show("商品【" + mtTypeMx.ColRow["GDSEQ"] + " | " + mtTypeMx.ColRow["GDNAME"] + "】【申退数】为0或空，无法进行【申退】操作。");
                    return;
                }

                mtTypeMx.ColRow.Add("XSSL", mtTypeMx.ColRow["DHSL"]);
                mtTypeMx.ColRow.Add("BHSJJ", 0);
                mtTypeMx.ColRow.Add("BHSJE", 0);

                mtTypeMx.ColRow.Remove("KCSL");
                mtTypeMx.ColRow.Remove("UNITNAME");
                mtTypeMx.ColRow.Remove("PRODUCERNAME");
                cmdList.Add(mtTypeMx.Insert());
            }
            DbHelperOra.ExecuteSqlTran(cmdList);

            Alert.Show("商品申退信息保存成功！");
            OperLog("科室申退", "修改单据【" + docBILLNO.Text + "】");
            btnSumit.Enabled = true;
            btnSave.Enabled = false;
            billOpen(docSEQNO.Text);
        }

        /// <summary>
        /// FineUIPro.Grid控件数据加载
        /// </summary>
        /// <param name="row">要加载的行数据</param>
        /// <param name="firstRow">是否插入指定行</param>
        /// <param name="flag">数据来源：NEW-从数据库中获得，用于商品新增时；OLD-从销售单据明细中获得，用于修改或审核时</param>
        private void LoadGridRow(DataRow row, bool firstRow = true, string flag = "NEW")
        {
            PubFunc.GridRowAdd(GridGoods, row, firstRow);
        }

        protected void Window1_Close(object sender, WindowCloseEventArgs e)
        {
            DataTable dt = GetGoods(hfdValue.Text);
            dt.Columns.Remove(dt.Columns["BZHL"]);
            dt.Columns.Remove(dt.Columns["UNIT"]);
            if (dt != null && dt.Rows.Count > 0)
            {
                dt.Columns["UNITNAME"].ColumnName = "UNITSMALLNAME";
                dt.Columns["UNIT_SELL_NAME"].ColumnName = "UNITNAME";
                dt.Columns["UNIT_SELL"].ColumnName = "UNIT";
                dt.Columns["BZHL_SELL"].ColumnName = "BZHL";

                dt.Columns.Add("PH", Type.GetType("System.String"));
                dt.Columns.Add("RQ_SC", Type.GetType("System.String"));
                dt.Columns.Add("YXQZ", Type.GetType("System.String"));
                dt.Columns.Add("MEMO", Type.GetType("System.String"));
                dt.Columns.Add("BZSL", Type.GetType("System.Int32"));
                dt.Columns.Add("DHSL", Type.GetType("System.Int32"));
                dt.Columns.Add("KCSL", Type.GetType("System.Int32"));
                dt.Columns.Add("HSJE", Type.GetType("System.Double"));
                string msg1 = "";
                string msg2 = "";
                foreach (DataRow row in dt.Rows)
                {
                    row["BZSL"] = "0";
                    row["DHSL"] = "0";
                    row["KCSL"] = "0";
                    row["HSJE"] = "0";
                    string sSQL = string.Format("SELECT A.HWID,A.KCSL - A.LOCKSL KCSL,A.PH,A.YXQZ,A.RQ_SC,B.ISGZ,B.GDNAME FROM DAT_GOODSSTOCK A ,DOC_GOODS B WHERE A.DEPTID ='{0}' AND A.GDSEQ = '{1}'  AND A.GDSEQ = B.GDSEQ AND A.KCSL >0 AND ROWNUM = 1 ORDER BY A.PICINO ASC", docDEPTID.SelectedValue, row["GDSEQ"].ToString());
                    DataTable Temp = DbHelperOra.Query(sSQL).Tables[0];
                    if (Temp.Rows.Count > 0)
                    {
                        row["KCSL"] = (Temp.Rows[0]["KCSL"] ?? "");
                        row["PH"] = (Temp.Rows[0]["PH"] ?? "");
                        row["YXQZ"] = (Temp.Rows[0]["YXQZ"] ?? "");
                        row["RQ_SC"] = (Temp.Rows[0]["RQ_SC"] ?? "");
                        row["HWID"] = (Temp.Rows[0]["HWID"] ?? "");
                    }
                    else
                    {
                        msg1 += row["GDNAME"] + ",";
                        continue;
                    }


                    if (string.IsNullOrWhiteSpace(row["HSJJ"].ToString()) || row["HSJJ"].ToString() == "0")
                    {
                        msg2 += "【" + row["GDSEQ"].ToString() + " | " + row["GDNAME"].ToString() + "】,";
                        continue;
                    }
                    //换算价格
                    //row["HSJJ"] = Math.Round(Convert.ToDecimal(row["HSJJ"]) * Convert.ToDecimal(row["BZHL"]), 4);
                    //处理金额格式
                    decimal jingdu = 0;
                    decimal bzhl = 0;
                    if (decimal.TryParse(row["HSJJ"].ToString(), out jingdu) && decimal.TryParse(row["BZHL"].ToString(), out bzhl)) { row["HSJJ"] = Math.Round(jingdu * bzhl, 4).ToString("F4"); }
                    if (decimal.TryParse(row["YBJ"].ToString(), out jingdu)) { row["YBJ"] = jingdu.ToString("F4"); }
                    if (decimal.TryParse(row["HSJE"].ToString(), out jingdu)) { row["HSJE"] = Math.Round(jingdu, 2).ToString("F2"); }

                    LoadGridRow(row, false);
                }
                if (!string.IsNullOrWhiteSpace(msg1))
                {
                    String strNostock = "";
                    strNostock = string.Format("商品【{0}】在部门『{1}』中没有库存,不能进行录入！", msg1, docDEPTID.SelectedText);
                    Alert.Show(strNostock, "消息提示", MessageBoxIcon.Warning);
                }

                if (!string.IsNullOrWhiteSpace(msg2))
                {
                    String strNostock = "";
                    strNostock = string.Format("商品【{0}】【含税进价】为空,不能进行【商品退货审批】操作。", msg2);
                    Alert.Show(strNostock, "消息提示", MessageBoxIcon.Warning);
                }
            }
            else
            {
                Alert.Show("系统传值错误！！！", "消息提示", MessageBoxIcon.Warning);
            }
        }

        protected void btnClosePostBack_Click(object sender, EventArgs e)
        {
            bool firstRow = true;
            foreach (GridRow row in GridLot.Rows)
            {
                int rowIndex = row.RowIndex;
                System.Web.UI.WebControls.TextBox tbxNumber = (System.Web.UI.WebControls.TextBox)GridLot.Rows[rowIndex].FindControl("tbxNumber");
                if (!string.IsNullOrWhiteSpace(tbxNumber.Text) && tbxNumber.Text != "0")
                {
                    string[] strCell = GridGoods.SelectedCell;
                    Dictionary<string, object> newDict = GridGoods.GetNewAddedList()[Convert.ToInt32(strCell[0])];
                    newDict["PH"] = row.Values[1];
                    newDict["YXQZ"] = row.Values[2];
                    newDict["PZWH"] = row.Values[4];
                    newDict["RQ_SC"] = row.Values[3];
                    newDict["BZSL"] = tbxNumber.Text;
                    newDict["KCSL"] = row.Values[6];
                    if (firstRow)
                    {
                        firstRow = false;
                        //string cell = string.Format("[{0},{1}]", intCell[0], intCell[1]);
                        //PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(GetJObject(newDict), cell));
                        PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(strCell[0], strCell[1], GetJObject(newDict).ToString()));
                    }
                    else
                    {
                        PageContext.RegisterStartupScript(GridGoods.GetAddNewRecordReference(GetJObject(newDict)));
                    }
                }
            }
            WindowLot.Hidden = true;
        }
        protected override void billDel()
        {
            if (docBILLNO.Text.Trim() == "")
            {
                Alert.Show("当前单据未保存，不需要删除", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string flag = DbHelperOra.GetSingle("SELECT NVL(FLAG,'M') FROM DAT_CK_DOC WHERE SEQNO='" + docBILLNO.Text.Trim() + "'").ToString();
            if (!string.IsNullOrWhiteSpace(flag) && (",M,R").IndexOf(flag) < 0)
            {
                if (docFLAG.SelectedValue != flag)
                {
                    Alert.Show("不能删除：当前单据状态已经发生改变，请刷新页面更新单据状态！", "消息提示", MessageBoxIcon.Warning);
                }
                else
                {
                    Alert.Show("非『新增』单据不能删除！", "消息提示", MessageBoxIcon.Warning);
                }
                return;
            }
            List<string> listSql = new List<string>();
            listSql.Add(string.Format("DELETE FROM DAT_CK_DOC WHERE SEQNO = '{0}'", docBILLNO.Text.Trim()));
            listSql.Add(string.Format("DELETE FROM DAT_CK_COM WHERE SEQNO = '{0}'", docBILLNO.Text.Trim()));
            listSql.Add(string.Format("DELETE FROM DAT_CK_EXT WHERE BILLNO = '{0}'", docBILLNO.Text.Trim()));
            if (DbHelperOra.ExecuteSqlTran(listSql))
            {
                Alert.Show("单据删除成功!", "消息提示", MessageBoxIcon.Information);
                OperLog("科室申退", "删除单据【" + docBILLNO.Text + "】");
                billNew();
                billSearch();
            }
            else
            {
                Alert.Show("单据删除失败!", "错误提示", MessageBoxIcon.Information);
            }
        }
        protected void btnScan_Click(object sender, EventArgs e)
        {
            if (docFLAG.SelectedValue == "Y" || docFLAG.SelectedValue == "N")
            {
                zsmScan.Enabled = true;
                zsmALL.Enabled = true;
            }
            else
            {
                zsmScan.Enabled = false;
                zsmALL.Enabled = false;
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
        }
        protected void btnScanClose_Click(object sender, EventArgs e)
        {
            WindowScan.Hidden = true;
        }
        #region 追溯码
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
        //库房使用退收货扫码
        protected void zsmScan_TriggerClick(object sender, EventArgs e)
        {
            if (zsmScan.Text.Length < Doc.LENCODE()) return;
            if (zsmScan.Text.Substring(0, 1) != "2")
            {
                Alert.Show("您扫描的条码不是贵重码,请检查！", "提示信息", MessageBoxIcon.Warning);
                zsmScan.Text = string.Empty;
                zsmScan.Focus();
                return;
            }
            //增加输入二维码验证
            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_GZ_EXT WHERE ONECODE = '{0}' AND FLAG ='C' AND DEPTCUR = '{1}'", zsmScan.Text.Trim(), docDEPTID.SelectedValue)))
            {
                Alert.Show("您扫描的追溯码已被使用或已被退货，请检查！", "提示信息", MessageBoxIcon.Warning);
                zsmScan.Text = string.Empty;
                zsmScan.Focus();
                return;
            }
            //校验追溯码是否存在
            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_CK_EXT WHERE ONECODE = '{0}' AND BILLNO = '{1}'", zsmScan.Text.Trim(), docBILLNO.Text)))
            {
                Alert.Show("您扫描的追溯码已被扫描，请检查！", "提示信息", MessageBoxIcon.Warning);
                zsmScan.Text = string.Empty;
                zsmScan.Focus();
                return;
            }
            //写入数据库中
            String Sql = @"UPDATE DAT_CK_EXT SET FLAG ='Y' WHERE BILLNO = '{0}' AND ONECODE = '{1}'";
            DbHelperOra.ExecuteSql(string.Format(Sql, docBILLNO.Text, zsmScan.Text));
            ScanSearch("");
        }
        protected void zsmScan_TextChanged(object sender, EventArgs e)
        {
            if (docFLAG.SelectedValue == "Y")
            {
                Alert.Show("非『新单』单据不允许操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (zsmScan.Text.Length < Doc.LENCODE()) return;
            if (zsmScan.Text.Substring(0, 1) != "2")
            {
                Alert.Show("您扫描的条码不是贵重码,请检查！", "提示信息", MessageBoxIcon.Warning);
                zsmScan.Text = string.Empty;
                zsmScan.Focus();
                return;
            }
            //增加输入二维码验证
            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_GZ_EXT WHERE ONECODE = '{0}' AND FLAG ='C' AND DEPTCUR = '{1}'", zsmScan.Text.Trim(), docDEPTID.SelectedValue)))
            {
                Alert.Show("您扫描的追溯码已被使用或已被退货，请检查！", "提示信息", MessageBoxIcon.Warning);
                zsmScan.Text = string.Empty;
                zsmScan.Focus();
                return;
            }
            //校验追溯码是否存在
            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_CK_EXT WHERE ONECODE = '{0}' AND BILLNO = '{1}'", zsmScan.Text.Trim(), docBILLNO.Text)))
            {
                Alert.Show("您扫描的追溯码已被扫描，请检查！", "提示信息", MessageBoxIcon.Warning);
                zsmScan.Text = string.Empty;
                zsmScan.Focus();
                return;
            }
            //写入数据库中
            DbHelperOra.ExecuteSql(string.Format(@"INSERT INTO DAT_CK_EXT(DEPTID,BILLNO,ROWNO,ONECODE,GDSEQ,GDNAME,BARCODE,UNIT,GDSPEC,DEPTCUR,BZHL,INSTIME,PH,RQ_SC,YXQZ)
                    SELECT '{0}','{1}',NVL((SELECT MAX(ROWNO)+1 FROM DAT_CK_EXT WHERE BILLNO = '{1}'),1),'{2}',GDSEQ,GDNAME,BARCODE,UNIT,GDSPEC,DEPTCUR,BZHL,SYSDATE,PH,RQ_SC,YXQZ
                    FROM DAT_RK_EXT A
                    WHERE A.ONECODE = '{2}' AND ROWNUM = 1", docDEPTOUT.SelectedValue, docBILLNO.Text, zsmScan.Text));
            ScanSearch("");
        }

        protected void zsmDelete_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(docBILLNO.Text.Trim()))
            {
                string flag = DbHelperOra.GetSingle("SELECT FLAG FROM DAT_CK_DOC WHERE BILLNO ='" + docBILLNO.Text.Trim() + "'").ToString();

                if ((",M,R").IndexOf(flag) < 0)
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
                //DbHelperOra.ExecuteSql(string.Format("DELETE FROM DAT_CK_EXT WHERE ONECODE = '{0}' AND SEQNO = '{1}'", onecode, docBILLNO.Text));
                DbHelperOra.ExecuteSql(string.Format("UPDATE DAT_CK_EXT SET FLAG = 'N' WHERE ONECODE = '{0}' AND SEQNO = '{1}'", onecode, docBILLNO.Text));
                ScanSearch("");
            }
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
            zsmScan.Text = String.Empty;
            zsmScan.Focus();
        }
        #endregion

        protected void btnSumit_Click(object sender, EventArgs e)
        {
            if (docSEQNO.Text.Length < 1)
            {
                return;
            }
            //将高值信息写入从表中
            String Sql = String.Format(@"INSERT INTO DAT_CK_EXT(DEPTID,BILLNO,ROWNO,ONECODE,GDSEQ,GDNAME,BARCODE,UNIT,GDSPEC,DEPTCUR,BZHL,INSTIME,PH,RQ_SC,YXQZ)
                    SELECT '{0}','{1}',rownum,A.ONECODE,A.GDSEQ,A.GDNAME,A.BARCODE,A.UNIT,A.GDSPEC,A.DEPTCUR,A.BZHL,SYSDATE,A.PH,A.RQ_SC,A.YXQZ
                    FROM DAT_RK_EXT A,DAT_CK_COM B
                    WHERE A.ONECODE = B.STR2 AND B.SEQNO = '{1}'", docDEPTID.SelectedValue, docSEQNO.Text);
            string flg = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M') FROM DAT_CK_DOC WHERE SEQNO='{0}'", docSEQNO.Text));
            if (!PubFunc.StrIsEmpty(flg) && (",M,R").IndexOf(flg) < 0)
            {
                Alert.Show("申领单:" + docSEQNO.Text + "不是新增单据，不能提交!", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            DbHelperOra.ExecuteSql(Sql);
            //增加高值判断
            if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_CK_COM A,DOC_GOODS B WHERE A.GDSEQ = B.GDSEQ AND B.ISGZ = 'Y' AND A.SEQNO = '{0}' AND (A.GDSEQ,A.PH) NOT IN(SELECT GDSEQ,PH FROM DAT_CK_EXT WHERE BILLNO = '{0}')", docSEQNO.Text)))
            {
                Alert.Show("申领单:" + docSEQNO.Text + "存在未扫描条码的高值商品或扫描的高值批次不正确，请检查!", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (DbHelperOra.ExecuteSql(string.Format("UPDATE  DAT_CK_DOC SET FLAG='N' WHERE SEQNO='{0}'", docBILLNO.Text)) == 1)
            {
                Alert.Show("单据提交成功！");
                OperLog("科室申退", "提交单据【" + docSEQNO.Text + "】");
                billOpen(docSEQNO.Text);
                docFLAG.SelectedValue = "N";
                btnSumit.Enabled = false;
            }
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            WindowLot.Hidden = true;
            WindowBack.Hidden = true;
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

        protected void tbxBARCODE_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(docTHTYPE.SelectedValue))
            {
                Alert.Show("请选择申退原因！！！", "异常提示", MessageBoxIcon.Warning);
                tbxBARCODE.Text = "";
                tbxBARCODE.Focus();
                return;
            }
            int len = Doc.LENCODE();
            if (tbxBARCODE.Text.Trim().Length < len)
            { return; }
            if (tbxBARCODE.Text.Trim().Substring(0, 1) == "0")
            {
                #region
                DataTable dtCode = DbHelperOra.Query(string.Format("SELECT * FROM DAT_CK_BARCODE WHERE BARCODE = '{0}'", tbxBARCODE.Text)).Tables[0];
                if (dtCode == null || dtCode.Rows.Count != 1)
                {
                    Alert.Show("条码信息读取异常，请检查！！！", "异常提示", MessageBoxIcon.Warning);
                    return;
                }

                string deptid = dtCode.Rows[0]["DEPTIN"].ToString();
                if (string.IsNullOrWhiteSpace(docDEPTID.SelectedValue))
                {
                    docDEPTID.SelectedValue = deptid;
                }
                else
                {
                    if (docDEPTOUT.SelectedValue != deptid)
                    {
                        Alert.Show("扫描条码不是从【" + docDEPTOUT.SelectedText + "】申领的,请检查!", "操作提示", MessageBoxIcon.Warning);
                        tbxBARCODE.Text = "";
                        tbxBARCODE.Focus();
                        return;
                    }
                }

                //判断该条码是否在单据里使用了，限定条件为“提交”状态单据
                string sqls = string.Format("SELECT 1 FROM DAT_CK_DOC D, DAT_CK_COM C WHERE D.SEQNO = C.SEQNO AND C.STR2 = '{0}' AND FLAG = 'N'", tbxBARCODE.Text);
                if (DbHelperOra.Exists(sqls))
                {
                    Alert.Show("该条码在其他【提交】状态的单据里已被使用");
                    return;
                }
                
                string kf = dtCode.Rows[0]["DEPTOUT"].ToString();
                if (string.IsNullOrWhiteSpace(docDEPTOUT.SelectedValue))
                {
                    docDEPTOUT.SelectedValue = kf;
                }
                else
                {
                    if (docDEPTOUT.SelectedValue != kf)
                    {
                        Alert.Show("扫描条码非【" + docDEPTOUT.SelectedText + "】科室条码,请检查!", "操作提示", MessageBoxIcon.Warning);
                        tbxBARCODE.Text = "";
                        tbxBARCODE.Focus();
                        return;
                    }
                }
                PubFunc.FormLock(FormDoc, true, "docMEMO,tbxBARCODE");

                if (tbxBARCODE.Text.Substring(0, 1) != "0")
                {
                    //增加定数条码提示
                    if (tbxBARCODE.Text.Substring(0, 1) == "1")
                    {
                        Alert.Show("扫描定数条码为定数条码,请到【使定数条码回收】界面录入!", "异常提示", MessageBoxIcon.Warning);
                    }
                    else
                    {
                        Alert.Show(string.Format("条码【{0}】不是非定数条码,不能在本页面进行回收扫描!", tbxBARCODE.Text), "异常提示", MessageBoxIcon.Warning);
                    }
                    tbxBARCODE.Text = "";
                    tbxBARCODE.Focus();
                    return;
                }

                //重新取得数量
                DataTable barcode = DbHelperOra.Query("select A.*,B.PIZNO from dat_ck_barcode A,DOC_GOODS B where A.GDSEQ = B.GDSEQ AND A.BARCODE = '" + tbxBARCODE.Text + "' and A.FLAG = 'N'").Tables[0];
                if (barcode.Rows.Count < 1)
                {
                    Alert.Show("扫描条码不存在或已经被回收请检查!");
                    tbxBARCODE.Text = "";
                    tbxBARCODE.Focus();
                    return;
                }
                List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().ToList();
                if (newDict.Count > 0)
                {
                    for (int i = 0; i < newDict.Count; i++)
                    {
                        string barcode_old = (newDict[i]["STR2"] ?? "").ToString();
                        if (barcode_old == tbxBARCODE.Text)
                        {
                            Alert.Show("扫描定数条码已存在!");
                            tbxBARCODE.Text = "";
                            tbxBARCODE.Focus();
                            return;
                        }
                    }
                }

                string code = dtCode.Rows[0]["GDSEQ"].ToString(); //tbxBARCODE.Text.Trim().Substring(13, 12);
                string dept = docDEPTID.SelectedValue;

                if (!string.IsNullOrWhiteSpace(code) && code.Trim().Length >= 2)
                {
                    DataTable dt_goods = Doc.GetGoods_His(code, "", dept);

                    if (dt_goods != null && dt_goods.Rows.Count > 0)
                    {
                        dt_goods.Columns.Add("ONECODE", Type.GetType("System.String"));
                        dt_goods.Columns.Add("STR2", Type.GetType("System.String"));
                        dt_goods.Columns.Add("BZSL", Type.GetType("System.Int32"));
                        dt_goods.Columns.Add("DHSL", Type.GetType("System.Int32"));
                        dt_goods.Columns.Add("KCSL", Type.GetType("System.Int32"));
                        dt_goods.Columns.Add("HSJE", Type.GetType("System.Double"));
                        DataRow dr_goods = dt_goods.Rows[0];
                        dr_goods["BZSL"] = barcode.Rows[0]["BZSL"];
                        dr_goods["DHSL"] = barcode.Rows[0]["DHSL"];
                        dr_goods["KCSL"] = barcode.Rows[0]["DHSL"];
                        dr_goods["HSJE"] = Convert.ToDecimal(barcode.Rows[0]["BZSL"]) * Convert.ToDecimal(barcode.Rows[0]["HSJJ"]) * Convert.ToDecimal(barcode.Rows[0]["BZHL"]);
                        dr_goods["PH"] = barcode.Rows[0]["PH"];
                        dr_goods["PZWH"] = barcode.Rows[0]["PIZNO"];
                        dr_goods["RQ_SC"] = barcode.Rows[0]["RQ_SC"];
                        dr_goods["YXQZ"] = barcode.Rows[0]["YXQZ"];
                        //货位使用科室编码
                        dr_goods["HWID"] = dept;
                        dr_goods["STR2"] = tbxBARCODE.Text;
                        LoadGridRow(dr_goods, false);
                        tbxBARCODE.Text = "";
                        tbxBARCODE.Focus();
                    }
                    else
                    {
                        Alert.Show(string.Format("{0}尚未配置商品【{1}】！！！", docDEPTID.SelectedText, code), MessageBoxIcon.Warning);
                        PubFunc.GridRowAdd(GridGoods, "CLEAR");
                    }
                }
                #endregion
            }
            else
            {
                #region
                DataTable dtCode = DbHelperOra.Query(string.Format("SELECT * FROM DAT_GZ_EXT WHERE ONECODE = '{0}' AND FLAG = 'C'", tbxBARCODE.Text)).Tables[0];
                if (dtCode == null || dtCode.Rows.Count != 1)
                {
                    Alert.Show("条码信息已被使用或条码输入错误，请检查！！！", "异常提示", MessageBoxIcon.Warning);
                    return;
                }

                string deptid = dtCode.Rows[0]["DEPTCUR"].ToString();
                if (string.IsNullOrWhiteSpace(docDEPTID.SelectedValue))
                {
                    docDEPTID.SelectedValue = deptid;
                }
                else
                {
                    if (docDEPTID.SelectedValue != deptid)
                    {
                        Alert.Show("扫描条码不是从【" + docDEPTID.SelectedText + "】申领的,请检查!", "操作提示", MessageBoxIcon.Warning);
                        tbxBARCODE.Text = "";
                        tbxBARCODE.Focus();
                        return;
                    }
                }

                PubFunc.FormLock(FormDoc, true, "docMEMO,tbxBARCODE");

                //重新取得数量
                DataTable barcode = DbHelperOra.Query("SELECT A.*,B.PIZNO FROM DAT_GZ_EXT A,DOC_GOODS B,DOC_GOODSCFG C WHERE A.GDSEQ = B.GDSEQ AND A.GDSEQ = C.GDSEQ AND C.DEPTID ='" + docDEPTOUT.SelectedValue + "' AND A.ONECODE = '" + tbxBARCODE.Text + "' AND A.FLAG = 'C'").Tables[0];
                if (barcode.Rows.Count < 1)
                {
                    Alert.Show("扫描的条码收货部门[" + docDEPTOUT.SelectedText + "]未配置!", MessageBoxIcon.Warning);
                    tbxBARCODE.Text = "";
                    tbxBARCODE.Focus();
                    return;
                }
                List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().ToList();
                if (newDict.Count > 0)
                {
                    for (int i = 0; i < newDict.Count; i++)
                    {
                        string barcode_old = (newDict[i]["STR2"] ?? "").ToString();
                        if (barcode_old == tbxBARCODE.Text)
                        {
                            Alert.Show("扫描条码已存在!", MessageBoxIcon.Warning);
                            tbxBARCODE.Text = "";
                            tbxBARCODE.Focus();
                            return;
                        }
                    }
                }

                string code = dtCode.Rows[0]["GDSEQ"].ToString(); //tbxBARCODE.Text.Trim().Substring(13, 12);
                string dept = docDEPTID.SelectedValue;

                if (!string.IsNullOrWhiteSpace(code) && code.Trim().Length >= 2)
                {
                    DataTable dt_goods = Doc.GetGoods_His(code, "", dept);

                    if (dt_goods != null && dt_goods.Rows.Count > 0)
                    {
                        dt_goods.Columns.Add("ONECODE", Type.GetType("System.String"));
                        dt_goods.Columns.Add("STR2", Type.GetType("System.String"));
                        dt_goods.Columns.Add("BZSL", Type.GetType("System.Int32"));
                        dt_goods.Columns.Add("DHSL", Type.GetType("System.Int32"));
                        dt_goods.Columns.Add("KCSL", Type.GetType("System.Int32"));
                        dt_goods.Columns.Add("HSJE", Type.GetType("System.Double"));
                        DataRow dr_goods = dt_goods.Rows[0];
                        dr_goods["BZSL"] = "1";
                        dr_goods["DHSL"] = "1";
                        dr_goods["KCSL"] = "1";
                        dr_goods["HSJE"] = dr_goods["HSJJ"];
                        dr_goods["PH"] = barcode.Rows[0]["PH"];
                        dr_goods["PZWH"] = barcode.Rows[0]["PIZNO"];
                        dr_goods["RQ_SC"] = barcode.Rows[0]["RQ_SC"];
                        dr_goods["YXQZ"] = barcode.Rows[0]["YXQZ"];
                        //货位使用科室编码
                        dr_goods["HWID"] = dept;
                        dr_goods["STR2"] = tbxBARCODE.Text;
                        LoadGridRow(dr_goods, false);
                        tbxBARCODE.Text = "";
                        tbxBARCODE.Focus();
                    }
                    else
                    {
                        Alert.Show(string.Format("{0}尚未配置商品【{1}】！！！", docDEPTID.SelectedText, code), MessageBoxIcon.Warning);
                        PubFunc.GridRowAdd(GridGoods, "CLEAR");
                    }
                }
                #endregion
            }
        }
        protected void btnRejectSubmit_Click(object sender, EventArgs e)
        {
            string strMemo = "驳回原因：" + ddlReject.SelectedText;
            if (string.IsNullOrEmpty(ddlReject.SelectedValue))
            {
                Alert.Show("驳回原因不能为空！","错误提示",MessageBoxIcon.Error);
                return;
            }
            if (!string.IsNullOrWhiteSpace(txaMemo.Text.Trim()))
            {
                strMemo += "；详细说明：" + txaMemo.Text;
               
            }

            if (DbHelperOra.ExecuteSql(string.Format("UPDATE DAT_CK_DOC SET FLAG = 'R',SHR='{1}',SHRQ = SYSDATE,MEMO='{2}' WHERE SEQNO = '{0}'", docBILLNO.Text.Trim(), UserAction.UserID, strMemo)) > 0)
            {
                Alert.Show("单据驳回成功!", "消息提示", MessageBoxIcon.Information);
                OperLog("科室申退", "驳回单据【" + docBILLNO.Text + "】");
                WindowReject.Hidden = true;
                billOpen(docBILLNO.Text.Trim());
            }
            else
            {
                Alert.Show("单据驳回失败!", "错误提示", MessageBoxIcon.Information);
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            //GridReturn.DataSource = DbHelperOra.Query(String.Format(@"SELECT A.*,F_GETUNITNAME(A.UNIT) UNITNAME,
            //                            F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME
            //                            FROM DAT_CK_COM A WHERE A.SEQNO='{0}'", docSEQNO.Text)).Tables[0];
            //查询该单据非代管商品
            //GridReturn.DataSource = DbHelperOra.Query(String.Format(@"SELECT A.SEQNO,A.ROWNO,A.GDSEQ,A.GDNAME,A.GDSPEC,SUM(B.SL) BZSL,B.HSJJ,SUM(B.HSJE),A.PH,A.STR2,A.YXQZ,A.RQ_SC,
            //                        F_GETUNITNAME(A.UNIT) UNITNAME,F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME
            //            FROM DAT_CK_COM A,DAT_GOODSJXC B WHERE A.SEQNO=B.BILLNO AND A.ROWNO=B.ROWNO AND KCADD=1 AND A.SEQNO='{0}'
            //            AND B.SEQNO NOT IN (SELECT SEQNO FROM DAT_GOODSJXC WHERE PSSID='00001' AND SUPID='00001')
            //            GROUP BY A.SEQNO,A.ROWNO,A.GDSEQ,A.GDNAME,A.GDSPEC,B.HSJJ,A.PH,A.STR2,A.YXQZ,A.RQ_SC,A.UNIT,A.PRODUCER", docSEQNO.Text)).Tables[0];


            GridReturn.DataSource = DbHelperOra.Query(String.Format(@"SELECT A.ROWNO,rownum,B.PH, A.GDSEQ,A.GDNAME,A.GDSPEC,A.HSJJ,A.STR2，A.YXQZ,A.RQ_SC,F_GETUNITNAME(A.UNIT) UNITNAME,

                                       F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME,B.SL,B.SUMHSJE
                                      FROM DAT_CK_COM A ,
                                      (SELECT BILLNO,ROWNO,SUM(SL) SL,SUM(HSJE) SUMHSJE,PH FROM DAT_GOODSJXC WHERE KCADD=1 AND SEQNO NOT IN 
                                            (SELECT SEQNO FROM DAT_GOODSJXC WHERE PSSID='00001' AND SUPID='00001') GROUP BY BILLNO,ROWNO,PH)B
                                      WHERE A.SEQNO=B.BILLNO AND A.ROWNO=B.ROWNO AND A.SEQNO='{0}'", docSEQNO.Text)).Tables[0];
            DataTable dt = DbHelperOra.Query(String.Format(@"SELECT 1 FROM DAT_CK_COM A,DAT_GOODSJXC B 
                                    WHERE A.SEQNO=B.BILLNO AND A.ROWNO=B.ROWNO AND KCADD=1 AND A.SEQNO='{0}' 
                        AND B.SEQNO NOT IN (SELECT SEQNO FROM DAT_GOODSJXC WHERE PSSID='00001' AND SUPID='00001')", docSEQNO.Text)).Tables[0];
            if (dt.Rows.Count == 0)
            {
                Alert.Show("该单据没有可退至供应商的商品！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            GridReturn.DataBind();
            WindowBack.Hidden = false;
        }
        protected void btnReturn_Click(object sender, EventArgs e)
        {
            if (GridReturn.SelectedRowIDArray.Length < 1)
            {
                Alert.Show("请选择需要退至供应商的商品！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            //验证是否盘点
            if (DbHelperOra.Exists("SELECT 1 FROM DAT_PD_LOCK WHERE DEPTID = '" + docDEPTOUT.SelectedValue + "' AND FLAG='N'"))
            {
                Alert.Show(string.Format("部门【{0}】正在盘点,请盘点后进行操作！", docDEPTOUT.SelectedText), "警告提示", MessageBoxIcon.Warning);
                return;
            }
            String strBill = "";
            int[] selects = GridReturn.SelectedRowIndexArray;
            foreach (int index in selects)
            {
                strBill = strBill + GridReturn.DataKeys[index][0] + ",";
            }
            //DataTable dtScan = DbHelperOra.Query(string.Format("SELECT ROWNO FROM DAT_CK_COM WHERE ROWNO IN ({0}) AND STR4 IS NULL", strBill.TrimEnd(',').Replace("'", ""))).Tables[0];
            //if (dtScan.Rows.Count<1)
            //{
            //    Alert.Show(string.Format("所选商品已全部退至供应商，无需重复操作！" ), "警告提示", MessageBoxIcon.Warning);
            //    return;
            //}
            //DataTable dt = DbHelperOra.Query(string.Format("SELECT ROWNO,GDNAME FROM DAT_CK_COM WHERE SEQNO = '{0}' AND ROWNO IN ({1}) AND STR4 IS NOT NULL", docSEQNO.Text, strBill.TrimEnd(',').Replace("'", ""))).Tables[0];
            //if (dt.Rows.Count > 0)
            //{
            //    String rowno = "";
            //    for (int i = 0; i < dt.Rows.Count; i++)
            //    {
            //        rowno = rowno + dt.Rows[i][0] + ",";
            //    }
            //    Alert.Show(string.Format("第【"+ rowno.TrimEnd(',')+"】行商品已退至供应商，无需重复操作！"), "警告提示", MessageBoxIcon.Warning);
            //    return;
            //}
            String msg = "";
            foreach (int index in selects)
            {
                DataTable dt = DbHelperOra.Query(string.Format("SELECT ROWNO,GDNAME FROM DAT_CK_COM WHERE SEQNO = '{0}' AND ROWNO IN ({1}) AND STR4 IS NOT NULL", docSEQNO.Text, GridReturn.DataKeys[index][0])).Tables[0];
                if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_CK_COM WHERE SEQNO = '{0}' AND ROWNO = '{1}' AND STR4 IS NOT NULL", docSEQNO.Text, GridReturn.DataKeys[index][0])))
                {
                    msg = msg + GridReturn.DataKeys[index][2] + ",";
                }
            }
            if (!PubFunc.StrIsEmpty(msg))
            {
                Alert.Show(string.Format("第【" + msg.TrimEnd(',') + "】行商品已退至供应商，无需重复操作！"), "警告提示", MessageBoxIcon.Warning);
                return;
            }
            OracleParameter[] parameters ={
                                            new OracleParameter("VIN_BILLNO" ,OracleDbType.Varchar2,2000),
                                            new OracleParameter("VIN_ROWNO" ,OracleDbType.Varchar2,200),
                                            new OracleParameter("VIN_OPERUSER" ,OracleDbType.Varchar2,20),
                                            new OracleParameter("VO_BILLNUM",OracleDbType.Varchar2,20)
                                           };
                parameters[0].Value = docSEQNO.Text;
                parameters[1].Value = strBill.TrimEnd(',').Replace("'", "");
                parameters[2].Value = UserAction.UserID;

                parameters[0].Direction = ParameterDirection.Input;
                parameters[1].Direction = ParameterDirection.Input;
                parameters[2].Direction = ParameterDirection.Input;
                parameters[3].Direction = ParameterDirection.Output;
            try
            {
                DbHelperOra.RunProcedure("P_THD", parameters);
                if (parameters[3].Value.ToString() != "null")
                {
                    Alert.Show("商品退至供应商失败！", "消息提示", MessageBoxIcon.Information);
                }
                else
                {
                    Alert.Show("商品退至供应商成功!", "消息提示", MessageBoxIcon.Information);
                    WindowBack.Hidden = true;
                    billOpen(docBILLNO.Text.Trim());
                }
            }
            catch (Exception err)
            {
                if (err.Message.IndexOf("ORA-20001") > -1)
                {
                    string message = err.Message.Substring(0, err.Message.IndexOf("\n"));
                    Alert.Show(message.Substring(message.IndexOf("ORA-20001") + 10), "错误信息", MessageBoxIcon.Warning);
                }
                else
                {
                    Alert.Show(err.Message + "", "错误信息", MessageBoxIcon.Question);
                }
                //Alert.Show(err.Message);
            }


        }
    }
}