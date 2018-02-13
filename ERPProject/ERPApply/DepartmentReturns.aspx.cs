using FineUIPro;
using Newtonsoft.Json.Linq;
using XTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPProject.ERPApply
{
    public partial class DepartmentReturns : BillBase
    {
        private string strDocSql = "SELECT * FROM DAT_CK_DOC WHERE SEQNO ='{0}'";
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
                        ButtonHidden(btnAudit, btnCancel);
                    }
                    else if (Request.QueryString["oper"].ToString() == "audit")
                    {
                        billLockDoc(true);
                        ButtonHidden(btnNew, btnSave, btnCopy, btnAddRow, btnDelRow, btnSumit, btnDel, btnGoods);
                        TabStrip1.ActiveTabIndex = 0;
                        //billSearch();
                    }
                }
            }
        }

        private void DataInit()
        {
            //DepartmentBind.BindDDL("DDL_SYS_DEPOTRANGE", UserAction.UserID, lstDEPTOUT, docDEPTOUT);
            //PubFunc.DdlDataGet("DDL_SYS_DEPTRANGE", UserAction.UserID, lstDEPTOUT, docDEPTOUT);
            DepartmentBind.BindDDL("DDL_SYS_DEPTRANGE", UserAction.UserID, lstDEPTID);
            DepartmentBind.BindDDL("DDL_SYS_DEPTRANGE", UserAction.UserID, docDEPTID);
            PubFunc.DdlDataGet("DDL_USER", lstLRY, lstSLR, docLRY, docSLR);
            //PubFunc.DdlDataGet("DDL_SYS_DEPOTRANGE", UserAction.UserID, lstDEPTID, docDEPTID);
            PubFunc.DdlDataGet("DDL_SYS_DEPOT", lstDEPTOUT, docDEPTOUT);
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

            JObject defaultObj = Doc.GetJObject(GridGoods, e.RowID);
            if (e.ColumnID == "BZSL")
            {
                //string cell = string.Format("[{0},{1}]", e.RowIndex, intCell[1]);
                if (!PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "BZHL")) || !PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "BZSL")) || !PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "HSJJ")))
                {
                    Alert.Show("商品信息异常，请详细检查商品信息：包装含量、价格或数量！", "异常信息", MessageBoxIcon.Warning);
                    return;
                }
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
            if (e.ColumnID == "PH")
            {
                if (defaultObj["GDSEQ"] == null || PubFunc.StrIsEmpty(defaultObj["GDSEQ"].ToString()))
                {
                    Alert.Show("请先选择商品信息！", "异常信息", MessageBoxIcon.Warning);
                    return;
                }
                if (defaultObj["PH"] == null || PubFunc.StrIsEmpty(defaultObj["PH"].ToString()))
                {
                    Alert.Show("请填写批次信息！", "异常信息", MessageBoxIcon.Warning);
                    return;
                }

                if (defaultObj["PH"].ToString() == "\\")
                {
                    DataTable dtPH = Doc.GetGoodsPH_New(defaultObj["GDSEQ"].ToString(), docDEPTID.SelectedValue);
                    if (dtPH != null && dtPH.Rows.Count > 0)
                    {
                        hfdRowIndex.Text = e.RowID;
                        GridLot.DataSource = dtPH;
                        GridLot.DataBind();
                        WindowLot.Hidden = false;
                    }
                    else
                    {
                        Alert.Show("此商品已无库存,请检查！", MessageBoxIcon.Warning);
                    }
                }
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
            string url = "~/ERPQuery/GoodsWindow_New.aspx?Deptout=" + docDEPTOUT.SelectedValue + "&DeptIn=" + docDEPTID.SelectedValue + "&GoodsState=YTS";
            PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "商品信息查询"));
        }
        protected void PageManager1_CustomEvent(object sender, CustomEventArgs e)
        {
            switch (e.EventArgument)
            {
                case "GoodsAdd": Window1_Close(null, null); break;
                    // case "CONTROLM_ENTER": billGoods(); break;
            }
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
            if (lstSLR.SelectedItem != null && lstSLR.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.SLR='{0}'", lstSLR.SelectedItem.Value);
            }
            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedItem.Value);
            }
            if (lstDEPTOUT.SelectedItem != null && lstDEPTOUT.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND DEPTOUT='{0}'", lstDEPTOUT.SelectedItem.Value);
            }
            strSearch += string.Format(" AND deptid in( select code FROM SYS_DEPT where type <>'1' and  F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            strSearch += string.Format(" AND A.LRRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
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
            string strBillno = docSEQNO.Text;
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
            if (DbHelperOra.ExecuteSql(string.Format("UPDATE DAT_CK_DOC SET FLAG = 'R',SHR='{1}',SHRQ = SYSDATE WHERE SEQNO = '{0}'", docBILLNO.Text.Trim(), UserAction.UserID)) > 0)
            {
                Alert.Show("单据驳回成功!", "消息提示", MessageBoxIcon.Information);
                OperLog("科室申退", "驳回单据【" + docBILLNO.Text + "】");
                billOpen(docBILLNO.Text.Trim());
            }
            else
            {
                Alert.Show("单据驳回失败!", "错误提示", MessageBoxIcon.Information);
            }
        }

        protected override void listRow_DoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            billOpen(GridList.Rows[e.RowIndex].Values[1].ToString());
        }

        protected override void billOpen(string strBillno)
        {
            string sql = @"SELECT  A.*,
                                   F_GETUNITNAME(C.UNIT) UNITNAME,
                                   F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME,
                                   --F_GETKCGDSEQ(B.DEPTID,A.GDSEQ,A.PH) KCSL,
                                   NVL2(A.STR2,A.DHSL,F_GETKCGDSEQ(B.DEPTID,A.GDSEQ,A.PH))KCSL,
                                   f_getunitname(C.UNIT) UNITSMALLNAME,A.STR2
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
            summary.Add("HSJE", feeTotal.ToString("F6"));
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
                zsmScan.Enabled = false;
                zsmDelete.Enabled = true;

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
                zsmScan.Enabled = true;
                zsmDelete.Enabled = false;

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
                zsmScan.Enabled = true;
                zsmDelete.Enabled = false;

            }
        }

        protected override void billSave()
        {
            save();
        }

        private void save(string flag="N")
        {
            #region 数据有效性验证
            if ((",M,R").IndexOf(docFLAG.SelectedValue) < 0)
            {
                Alert.Show("非『" + docFLAG.SelectedText + "』单据不能保存！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().ToList();
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
                    if (!string.IsNullOrWhiteSpace(newDict[i]["STR2"].ToString()))
                    {
                        for (int k = 1 + i; k < newDict.Count; k++)
                        {
                            if ((newDict[i]["STR2"].ToString()) == (newDict[k]["STR2"].ToString()))
                            {
                                Alert.Show("商品『" + newDict[k]["GDNAME"].ToString() + "』条码『" + newDict[k]["STR2"].ToString() + "』重复，请维护！", "消息提示", MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    if (string.IsNullOrWhiteSpace(newDict[i]["STR2"].ToString()))
                    {
                        for (int j = 1 + i; j < newDict.Count; j++)
                        {
                            if (decimal.Parse(newDict[i]["GDSEQ"].ToString()) == decimal.Parse(newDict[j]["GDSEQ"].ToString()) && newDict[i]["PH"].ToString() == newDict[j]["PH"].ToString())
                            {
                                Alert.Show("同批号商品『" + newDict[j]["GDNAME"].ToString() + "』在明细中已存在，请维护！", "消息提示", MessageBoxIcon.Warning);
                                return;
                            }
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
            mtType.ColRow["FLAG"] = "M";
            mtType.ColRow.Add("BILLTYPE", BillType);
            mtType.ColRow.Add("SUBNUM", goodsData.Count);
            mtType.ColRow.Add("XSTYPE", "2");
            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtTypeMx = new MyTable("DAT_CK_COM");
            //先删除单据信息在插入
            cmdList.Add(new CommandInfo("delete DAT_CK_DOC where seqno='" + docBILLNO.Text + "'", null));//删除单据台头
            cmdList.Add(new CommandInfo("delete DAT_CK_COM where seqno='" + docBILLNO.Text + "'", null));//删除单据明细 
            decimal subSum = 0;//总金额
            for (int i = 0; i < goodsData.Count; i++)
            {
                mtTypeMx.ColRow = PubFunc.GridDataGet(goodsData[i]);
                mtTypeMx.ColRow.Add("SEQNO", docBILLNO.Text);
                mtTypeMx.ColRow["ROWNO"] = i + 1;
                mtTypeMx.ColRow.Add("PHID", mtTypeMx.ColRow["PH"]);
                mtTypeMx.ColRow["DHSL"] = decimal.Parse(mtTypeMx.ColRow["BZHL"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                mtTypeMx.ColRow["HSJE"] = decimal.Parse(mtTypeMx.ColRow["HSJJ"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                subSum = subSum + decimal.Parse(mtTypeMx.ColRow["HSJE"].ToString());
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
            mtType.ColRow.Add("SUBSUM", subSum);
            cmdList.AddRange(mtType.InsertCommand());
            DbHelperOra.ExecuteSqlTran(cmdList);
            if(flag == "N")
                Alert.Show("商品申退信息保存成功！");
            OperLog("科室申退", "修改单据【" + docBILLNO.Text + "】");
            btnSumit.Enabled = true;
            btnSave.Enabled = false;
            billOpen(docSEQNO.Text);
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
                dt.Columns.Add("STR2", Type.GetType("System.String"));
                string msg1 = "";
                string msg2 = "";
                foreach (DataRow row in dt.Rows)
                {
                    row["STR2"] = "";
                    row["BZSL"] = "0";
                    row["DHSL"] = "0";
                    row["KCSL"] = "0";
                    row["HSJE"] = "0";
                    row["UNITNAME"] = row["UNITSMALLNAME"];
                    row["BZHL"] = "1";
                    string sSQL = string.Format("SELECT * FROM (SELECT A.HWID,A.PH,A.YXQZ,A.RQ_SC,B.ISGZ,B.GDNAME FROM DAT_GOODSSTOCK A, DOC_GOODS B WHERE A.DEPTID ='{0}' AND A.GDSEQ = '{1}'  AND A.GDSEQ = B.GDSEQ AND A.KCSL >0  ORDER BY A.PSSID DESC,A.SUPID DESC) WHERE ROWNUM = 1 ", docDEPTID.SelectedValue, row["GDSEQ"].ToString());
                    DataTable Temp = DbHelperOra.Query(sSQL).Tables[0];
                    if (Temp.Rows.Count > 0)
                    {
                        object objKCSL = DbHelperOra.GetSingle(string.Format("SELECT SUM(KCSL-LOCKSL) FROM DAT_GOODSSTOCK WHERE GDSEQ ='{0}' AND DEPTID='{1}'AND PH='{2}'", row["GDSEQ"].ToString(), docDEPTID.SelectedValue, Temp.Rows[0]["PH"]));
                        int initNum = Convert.ToInt32(objKCSL ?? "0");
                        //row["KCSL"] = (Temp.Rows[0]["KCSL"] ?? "");
                        row["KCSL"] = (objKCSL ?? 0).ToString();
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
                    Alert.ShowInTop(strNostock, "消息提示", MessageBoxIcon.Warning);
                }

                if (!string.IsNullOrWhiteSpace(msg2))
                {
                    String strNostock = "";
                    strNostock = string.Format("商品【{0}】【含税进价】为空,不能进行【商品退货审批】操作。", msg2);
                    Alert.ShowInTop(strNostock, "消息提示", MessageBoxIcon.Warning);
                }
            }
            else
            {
                Alert.ShowInTop("系统传值错误！！！", "消息提示", MessageBoxIcon.Warning);
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
                    if (!PubFunc.isNumeric(tbxNumber.Text))
                    {
                        Alert.Show("请输入正确的数字信息!", "操作提示", MessageBoxIcon.Warning);
                        return;
                    }
                    //JObject defaultObj = Doc.GetJObject(GridGoods, hfdRowIndex.Text);

                    string[] strCell = GridGoods.SelectedCell;
                    JObject defaultObj = Doc.GetJObject(GridGoods, strCell[0]);

                    //Dictionary<string, object> newDict = GridGoods.GetNewAddedList()[Convert.ToInt16(hfdRowIndex.Text)];
                    //  string sSQL = string.Format("SELECT A.HWID,A.KCSL - A.LOCKSL KCSL,A.PH,A.YXQZ,A.RQ_SC,B.ISGZ,B.GDNAME FROM DAT_GOODSSTOCK A ,DOC_GOODS B WHERE A.DEPTID ='{0}' AND A.GDSEQ = '{1}'  AND A.GDSEQ = B.GDSEQ AND A.KCSL >0 AND ROWNUM = 1 ORDER BY A.PICINO ASC", docDEPTID.SelectedValue, row["GDSEQ"].ToString());
                    defaultObj["PH"] = row.Values[1].ToString();
                    defaultObj["YXQZ"] = row.Values[2].ToString();
                    defaultObj["RQ_SC"] = row.Values[3].ToString();
                    defaultObj["KCSL"] = row.Values[4].ToString();
                    defaultObj["BZSL"] = tbxNumber.Text;
                    defaultObj["DHSL"] = tbxNumber.Text;
                    defaultObj["HSJE"] = (Convert.ToInt16(tbxNumber.Text ?? "0") * Convert.ToDecimal(defaultObj["HSJJ"])).ToString();
                    //defaultObj["DHSL"] = Convert.ToInt32(defaultObj["BZSL"]) * Convert.ToInt32(defaultObj["BZHL"]);
                    if (firstRow)
                    {
                        firstRow = false;
                        PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(hfdRowIndex.Text, defaultObj));
                    }
                    else
                    {
                        PageContext.RegisterStartupScript(GridGoods.GetAddNewRecordReference(defaultObj));
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
            if (docFLAG.SelectedValue == "Y")
            {
                Alert.Show("非『新单』单据不允许操作！", "消息提示", MessageBoxIcon.Warning);
                return;
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
            #region 原逻辑
            //if (docFLAG.SelectedValue != "N" && docFLAG.SelectedValue != "R")
            //{
            //    Alert.Show("非新单据不允许扫描追溯码");
            //    return;
            //}
            //bool test = false;
            //PageContext.RegisterStartupScript(GridSacn.GetRejectChangesReference());
            //List<Dictionary<string, object>> goodsData = GridGoods.GetNewAddedList();
            //DataTable dtScan = new DataTable();
            //dtScan.Columns.Add(new DataColumn("LISGDSEQ", typeof(string)));
            //dtScan.Columns.Add(new DataColumn("LISGDNAME", typeof(string)));
            //dtScan.Columns.Add(new DataColumn("LISGDSPEC", typeof(string)));
            //dtScan.Columns.Add(new DataColumn("LISUNIT", typeof(string)));
            //dtScan.Columns.Add(new DataColumn("LISONECODE", typeof(string)));
            //for (int i = 0; i < goodsData.Count; i++)
            //{
            //    //测试
            //    if (DbHelperOra.Exists("select 1 from doc_goods where GDSEQ = '" + goodsData[i]["GDSEQ"] + "' and ISGZ in('Y','1')"))
            //    {
            //        for (int count = 0; count < Convert.ToInt16(goodsData[i]["BZSL"]); count++)
            //        {
            //            DataRow dr = dtScan.NewRow();
            //            dr["LISGDSEQ"] = goodsData[i]["GDSEQ"].ToString();
            //            dr["LISGDNAME"] = goodsData[i]["GDNAME"].ToString();
            //            dr["LISGDSPEC"] = goodsData[i]["GDSPEC"].ToString();
            //            dr["LISUNIT"] = goodsData[i]["UNITNAME"].ToString();
            //            dr["LISONECODE"] = "";
            //            //dtScan.Rows.Add(dr);
            //            PubFunc.GridRowAdd(GridSacn, dr, false);
            //            test = true;
            //        }
            //    }
            //}
            //if (!test)
            //{
            //    Alert.Show("单据中无贵重物品,请检查!");
            //    return;
            //}
            //WindowScan.Hidden = false;
            //lisonecode.Focus();
            #endregion
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
            WindowScan.Hidden = true;
        }
        #region 追溯码
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
                DbHelperOra.ExecuteSql(string.Format("DELETE FROM DAT_CK_EXT WHERE ONECODE = '{0}' AND SEQNO = '{1}'", onecode, docBILLNO.Text));
                ScanSearch("");
            }
        }
        protected void ScanSearch(string type)
        {
            string sql = "";
            if (type == "SHOW")
            {
                sql = "SELECT A.*,f_getunitname(A.UNIT) UNITNAME FROM DAT_CK_EXT A WHERE A.BILLNO = '{0}' ORDER BY A.GDSEQ,A.INSTIME DESC";
            }
            else
            {
                sql = "SELECT A.*,f_getunitname(A.UNIT) UNITNAME FROM DAT_CK_EXT A WHERE A.BILLNO = '{0}' ORDER BY A.INSTIME DESC";
            }
            DataTable dtScan = DbHelperOra.Query(string.Format(sql, docSEQNO.Text)).Tables[0];
            GridSacn.DataSource = dtScan;
            GridSacn.DataBind();
            zsmScan.Text = String.Empty;
            zsmScan.Focus();
        }
        #endregion
        private bool SaveSuccess = false;
        protected void btnSumit_Click(object sender, EventArgs e)
        {
            if (docSEQNO.Text.Length < 1)
            {
                return;
            }

            SaveSuccess = false;
            save("Y");
            if (SaveSuccess == false)
                return;
            SaveSuccess = false;

            //将高值信息写入从表中
            DbHelperOra.ExecuteSql(string.Format("delete from dat_ck_ext where billno='{0}' ", docSEQNO.Text));
            String Sql = String.Format(@"INSERT INTO DAT_CK_EXT(DEPTID,BILLNO,ROWNO,ONECODE,GDSEQ,GDNAME,BARCODE,UNIT,GDSPEC,DEPTCUR,BZHL,INSTIME,PH,RQ_SC,YXQZ)
                    SELECT '{0}','{1}',rownum,A.ONECODE,A.GDSEQ,A.GDNAME,A.BARCODE,A.UNIT,A.GDSPEC,A.DEPTCUR,A.BZHL,SYSDATE,A.PH,A.RQ_SC,A.YXQZ
                    FROM DAT_RK_EXT A,DAT_CK_COM B
                    WHERE A.ONECODE = B.STR2 AND B.SEQNO = '{1}'", docDEPTID.SelectedValue, docSEQNO.Text);
            string flg = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M') FROM DAT_CK_DOC WHERE SEQNO='{0}'", docSEQNO.Text));
            if (flg=="R")
            {
                Alert.Show("申领单:" + docSEQNO.Text + "是驳回单据，请先修改保存后再提交!", "消息提示", MessageBoxIcon.Warning);
                return;
            }
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

                string deptid = dtCode.Rows[0]["DEPTOUT"].ToString();
                if (string.IsNullOrWhiteSpace(docDEPTOUT.SelectedValue))
                {
                    docDEPTOUT.SelectedValue = deptid;
                }
                else
                {
                    if (docDEPTOUT.SelectedValue != deptid)
                    {
                        Alert.Show("该扫描条码不属于【" + docDEPTOUT.SelectedText + "】,请检查!", "操作提示", MessageBoxIcon.Warning);
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

                string kf = dtCode.Rows[0]["DEPTIN"].ToString();
                if (string.IsNullOrWhiteSpace(docDEPTID.SelectedValue))
                {
                    docDEPTID.SelectedValue = kf;
                }
                else
                {
                    if (docDEPTID.SelectedValue != kf)
                    {
                        Alert.Show("扫描条码非【" + docDEPTID.SelectedText + "】科室条码,请检查!", "操作提示", MessageBoxIcon.Warning);
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
                        dr_goods["HSJJ"] = DbHelperOra.GetSingle("SELECT HSJJ FROM DAT_CK_BARCODE WHERE BARCODE = '" + tbxBARCODE.Text + "'");
                        dr_goods["HSJE"] = Convert.ToDecimal(barcode.Rows[0]["BZSL"]) * Convert.ToDecimal(dr_goods["HSJJ"]) * Convert.ToDecimal(barcode.Rows[0]["BZHL"]);
                        dr_goods["PH"] = barcode.Rows[0]["PH"];
                        dr_goods["PZWH"] = barcode.Rows[0]["PIZNO"];
                        dr_goods["RQ_SC"] = barcode.Rows[0]["RQ_SC"];
                        dr_goods["YXQZ"] = barcode.Rows[0]["YXQZ"];
                        //货位使用科室编码
                        dr_goods["HWID"] = dept;
                        dr_goods["STR2"] = tbxBARCODE.Text;
                        Object obj = DbHelperOra.GetSingle("select count(1) from (select column_value from table(f_split('" + barcode.Rows[0]["PH"] + "')))");
                        int phnum = Convert.ToInt32(obj ?? "0");
                        if (phnum > 1)
                        {
                            dr_goods["PH"] = "";
                        }
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
                DataTable dtOneCode = DbHelperOra.Query(string.Format(@"SELECT A.SEQNO, f_getusername(A.LRY) LRYNAME FROM DAT_CK_DOC A, DAT_CK_COM B
WHERE A.SEQNO = B.SEQNO AND B.STR2 = '{0}' AND FLAG IN('M', 'N')", tbxBARCODE.Text)).Tables[0];
                if (dtCode == null || dtCode.Rows.Count != 1)
                {
                    Alert.Show("条码信息已被使用或条码输入错误，请检查！！！", "异常提示", MessageBoxIcon.Warning);
                    return;
                }
                else if (dtOneCode != null && dtOneCode.Rows.Count == 1)
                {
                    Alert.Show("条码信息已被操作员【" + dtOneCode.Rows[0][1] + "】使用，单据号【" + dtOneCode.Rows[0][0] + "】，请检查！！！", "异常提示", MessageBoxIcon.Warning);
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
                        Alert.Show("该扫描条码不属于【" + docDEPTID.SelectedText + "】,请检查!", "操作提示", MessageBoxIcon.Warning);
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
                        //退货取历史价格
                        dr_goods["HSJJ"] = DbHelperOra.GetSingle(String.Format(@"SELECT HSJJ FROM
                                                (SELECT HSJJ
                                                FROM DAT_ONECODEJXC 
                                                WHERE ONECODE ='{0}' ORDER BY SEQNO DESC) 
                                                WHERE ROWNUM = 1", tbxBARCODE.Text));
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


    }
}