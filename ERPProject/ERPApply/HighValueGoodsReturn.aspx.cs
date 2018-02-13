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
    public partial class HighValueGoodsReturn : BillBase
    {
        private string strDocSql = "SELECT * FROM DAT_XS_DOC WHERE SEQNO ='{0}' AND BILLTYPE = 'XST'";
        private string strComSql = @"SELECT A.*,F_GETUNITNAME(A.UNIT) UNITNAME,F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME,f_getunitname(B.UNIT) UNITSMALLNAME,DECODE(NVL(A.NUM3,0),0,'非赠品','赠品') NUM3NAME 
                FROM DAT_XS_COM A,DOC_GOODS B WHERE SEQNO ='{0}' AND A.GDSEQ = B.GDSEQ  ORDER BY ROWNO";
        public override Field[] LockControl
        {
            get { return new Field[] { docXSRQ }; }
        }

        public HighValueGoodsReturn()
        {
            BillType = "XST";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //屏蔽不需要的操作按钮
                //if (Request.QueryString["oper"] != null)
                //{
                //    if (Request.QueryString["oper"].ToString() == "input")
                //    {
                //        ButtonHidden(btnAudit, btnExport, btnCopy, btnNext, btnBef, btnPrint, btnCancel, btnAdt, btnAddRow);
                //    }
                //    else if (Request.QueryString["oper"].ToString() == "audit")
                //    {
                ButtonHidden(btnExport, btnCopy, btnNext, btnBef, btnAddRow, btnAdt, btnDelRow, btnCancel, btnGoods);
                //        TabStrip1.ActiveTabIndex = 0;
                //    }
                //    else if (Request.QueryString["oper"].ToString() == "receive")
                //    {
                //        ButtonHidden(btnExport, btnCopy, btnNext, btnBef, btnNew, btnDel, btnAddRow, btnDelRow, btnGoods, btnAdt, btnSave, btnCommit);
                //        TabStrip1.ActiveTabIndex = 0;
                //    }
                //}
                DataInit();
                billNew();
            }
        }

        private void DataInit()
        {
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
            PubFunc.DdlDataGet("DDL_USER", ddlSPR, docSHR, docLRY);
            //if (Request.QueryString["oper"].ToString() == "input")
            //{
            DepartmentBind.BindDDL("DDL_SYS_DEPTRANGE", UserAction.UserID, docDEPTID, lstDEPTID);
            //}
            //else
            //{
            //    PubFunc.DdlDataGet("DDL_SYS_DEPTDEF", docDEPTID, lstDEPTID);
            //}
            //PubFunc.DdlDataGet("DDL_SYS_DEPOTRANGE", UserAction.UserID, docDEPTOUT);
            //PubFunc.DdlDataGet("DDL_SYS_DEPOT", docDEPTOUT);
            PubFunc.DdlDataGet("DDL_BILL_STATUS", lstTYPE, docFLAG);
            PubFunc.DdlDataGet(ddlReject, "DDL_BILL_REASON");
            PubFunc.DdlDataGet(docSTR2, "DDL_RETURNREASON");
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
            //原单据保存判断
            string strDept = docDEPTID.SelectedValue;
            PubFunc.FormDataClear(FormDoc);

            docFLAG.SelectedValue = "M";
            docLRY.SelectedValue = UserAction.UserID;
            docLRRQ.SelectedDate = DateTime.Now;
            docXSRQ.SelectedDate = DateTime.Now;
            docDEPTID.SelectedValue = strDept;
            if (docDEPTID.SelectedValue.Length < 1)
            {
                docDEPTID.SelectedIndex = 1;
            }
            billLockDoc(false);
            docMEMO.Enabled = true;
            docSTR2.Enabled = true;
            docDEPTID.Enabled = true;
            //docDEPTOUT.Enabled = true;
            tgbSTR1.Enabled = true;
            //清空Grid行
            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            btnDel.Enabled = false;
            btnSave.Enabled = true;
            btnCommit.Enabled = false;
            btnDelRow.Enabled = true;
            btnAdt.Enabled = false;
            btnAudit.Enabled = false;
            btnCancel.Enabled = false;
            btnPrint.Enabled = false;


            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("BZSL", "0");
            summary.Add("HSJE", "0");
            GridGoods.SummaryData = summary;
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
            //defaultObj.Add("HSJE", rs * jg);

            //处理金额格式
            string jingdu = "";
            if (tgbSTR1.Text.Substring(0, 2) == "DS")
            {
                jingdu = (rs * jg * hl).ToString("F2");
            }
            else
            {
                jingdu = (rs * jg).ToString("F2");
            }

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
                //if (newDict[e.RowIndex]["BZSL"] == null)
                //{
                //    Alert.Show("请填写退货数量!");
                //    return;
                //}
                //if (newDict[e.RowIndex]["HSJJ"] == null)
                //{
                //    Alert.Show("请填写含税进价!");
                //    return;
                //}
                //if (!PubFunc.isNumeric(newDict[e.RowIndex]["BZHL"].ToString()) || !PubFunc.isNumeric(newDict[e.RowIndex]["BZSL"].ToString()) || !PubFunc.isNumeric(newDict[e.RowIndex]["HSJJ"].ToString()))
                //{
                //    Alert.Show("商品信息异常，请详细检查商品信息：包装含量、价格或数量！", "异常信息", MessageBoxIcon.Warning);
                //    return;
                //}
                if (Doc.GetGridInf(GridGoods, e.RowID, "BZSL").Length < 1)
                {
                    Alert.Show("请填写退货数量!");
                    return;
                }
                if (Doc.GetGridInf(GridGoods, e.RowID, "HSJJ").Length < 1)
                {
                    Alert.Show("请填写含税进价!");
                    return;
                }
                if (Doc.GetGridInf(GridGoods, e.RowID, "BZHL").Length < 1|| Doc.GetGridInf(GridGoods, e.RowID, "BZSL").Length <1||Doc.GetGridInf(GridGoods, e.RowID, "HSJJ").Length < 1)
                {
                    Alert.Show("商品信息异常，请详细检查商品信息：包装含量！", "异常信息", MessageBoxIcon.Warning);
                    return;
                }
                //PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(strCell[0], strCell[1], GetJObject(newDict[e.RowIndex]).ToString()));
                JObject defaultObj = Doc.GetJObject(GridGoods, e.RowID);
                PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(e.RowID, defaultObj));
                //计算合计数量
                decimal bzslTotal = 0, feeTotal = 0;
                foreach (Dictionary<string, object> dic in newDict)
                {
                    bzslTotal += Convert.ToDecimal(dic["BZSL"]);
                    if (tgbSTR1.Text.Substring(0, 2) == "DS")
                    {
                        feeTotal += Convert.ToDecimal(dic["HSJJ"]) * Convert.ToDecimal(dic["DHSL"]);
                    }
                    else
                    {
                        feeTotal += Convert.ToDecimal(dic["HSJJ"]) * Convert.ToDecimal(dic["BZSL"]);
                    }
                }
                JObject summary = new JObject();
                summary.Add("GDNAME", "本页合计");
                summary.Add("BZSL", bzslTotal.ToString());
                summary.Add("HSJE", feeTotal.ToString("F2"));
                GridGoods.SummaryData = summary;
            }
            else if (e.ColumnID == "PH")
            {
                #region
                //if (newDict[e.RowIndex]["GDSEQ"] == null)
                //{
                //    Alert.Show("请先选择商品信息！", "异常信息", MessageBoxIcon.Warning);
                //    return;
                //}
                //if (newDict[e.RowIndex]["PH"] == null)
                //{
                //    Alert.Show("请填写批次信息！", "异常信息", MessageBoxIcon.Warning);
                //    return;
                //}
                //if (PubFunc.StrIsEmpty(newDict[e.RowIndex]["GDSEQ"].ToString()))
                //{
                //    Alert.Show("请先选择商品信息！", "异常信息", MessageBoxIcon.Warning);
                //    return;
                //}
                //if (PubFunc.StrIsEmpty(newDict[e.RowIndex]["PH"].ToString()))
                //{
                //    Alert.Show("请填写批次信息！", "异常信息", MessageBoxIcon.Warning);
                //    return;
                //}
                ////if (newDict[e.RowIndex]["PH"].ToString() == "\\")
                ////{
                ////    DataTable dtPH = Doc.GetGoodsPH_New(newDict[e.RowIndex]["GDSEQ"].ToString(), docDEPTID.SelectedValue, true);
                ////    if (dtPH != null && dtPH.Rows.Count > 0)
                ////    {
                ////        hfdRowIndex.Text = GridGoods.SelectedCell[0].ToString();

                ////        GridLot.DataSource = dtPH;
                ////        GridLot.DataBind();
                ////        WindowLot.Hidden = false;
                ////    }
                ////    else
                ////    {
                ////        Alert.Show("此商品已无库存,请检查！", MessageBoxIcon.Warning);
                ////    }
                ////}

                //// 验证批次信息
                //DataTable dtPH = DbHelperOra.Query(string.Format("SELECT A.* FROM DAT_GOODSSTOCK A WHERE A.DEPTID = '{0}' AND A.GDSEQ ='{1}' AND A.PH = '{2}'", docDEPTID.SelectedValue, newDict[e.RowIndex]["GDSEQ"].ToString(), newDict[e.RowIndex]["PH"].ToString())).Tables[0];
                //DataTable dtPHOUT = DbHelperOra.Query(string.Format("SELECT 1 FROM DAT_GOODSSTOCK A WHERE A.DEPTID = '{0}' AND A.GDSEQ ='{1}' AND A.PH = '{2}'", docDEPTOUT.SelectedValue, newDict[e.RowIndex]["GDSEQ"].ToString(), newDict[e.RowIndex]["PH"].ToString())).Tables[0];
                //if (dtPH != null && dtPH.Rows.Count > 0 && dtPHOUT != null && dtPHOUT.Rows.Count > 0)
                //{
                //    if (dtPH.Rows.Count == 1)
                //    {
                //        newDict[e.RowIndex]["PH"] = dtPH.Rows[0]["PH"];
                //        newDict[e.RowIndex]["RQ_SC"] = dtPH.Rows[0]["RQ_SC"];
                //        newDict[e.RowIndex]["YXQZ"] = dtPH.Rows[0]["YXQZ"];
                //        int rowIndex = GridGoods.SelectedCell[0];
                //        string deleteScript = GridGoods.GetDeleteIndexReference(rowIndex);
                //        PageContext.RegisterStartupScript(deleteScript + GridGoods.GetAddNewRecordReferenceByIndex(GetJObject(newDict[e.RowIndex]), rowIndex));
                //    }
                //}
                //else
                //{
                //    newDict[e.RowIndex]["PH"] = "";
                //    newDict[e.RowIndex]["RQ_SC"] = "";
                //    newDict[e.RowIndex]["YXQZ"] = "";
                //    int rowIndex = GridGoods.SelectedCell[0];
                //    string deleteScript = GridGoods.GetDeleteIndexReference(rowIndex);
                //    PageContext.RegisterStartupScript(deleteScript + GridGoods.GetAddNewRecordReferenceByIndex(GetJObject(newDict[e.RowIndex]), rowIndex));
                //    Alert.Show("填写的批次信息不存在,请重新填写!", "异常信息", MessageBoxIcon.Warning);
                //    return;
                //}
                #endregion
            }
        }

        protected override void billClear()
        {
            PubFunc.FormDataClear(Formlist);
            lstLRRQ1.SelectedDate = DateTime.Now;
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
            if (GridGoods.SelectedRowID == null)
            {
                Alert.Show("请选择数据行删除！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!string.IsNullOrWhiteSpace(lstBILLNO.Text.Trim()))
            {
                string flag = DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M')  FROM DAT_XS_DOC WHERE BILLNO='{0}'", lstBILLNO.Text.Trim())).ToString();

                if (!string.IsNullOrWhiteSpace(flag) && (",M,R").IndexOf(docFLAG.SelectedValue) < 0)
                {
                    Alert.Show("非新单，不能删行！", "消息提示", MessageBoxIcon.Warning);
                    return;
                }
            }
            GridGoods.DeleteSelectedRows();
        }

        protected override void billGoods()
        {
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
            PubFunc.FormLock(FormDoc, true, "");
            docMEMO.Enabled = true;
            //参数说明：cx-查询内容，bm-商品配置部门,su-供应商
            string url = "~/ERPQuery/GoodsWindow_Gather.aspx?bm=" + docDEPTID.SelectedValue + "&cx=&su=";
            PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "商品信息查询"));
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

            string strSql = @"SELECT A.SEQNO,A.BILLNO,B.NAME FLAG,F_GETDEPTNAME(A.DEPTID) DEPTID,A.XSRQ,F_GETDEPTNAME(A.DEPTOUT) DEPTOUT,A.SUBSUM,
                                     A.SUBNUM,F_GETUSERNAME(A.LRY) LRY,A.LRRQ,F_GETUSERNAME(A.SHR) SHR,A.SHRQ,F_GETUSERNAME(A.SPR) SPR,A.SPRQ,A.MEMO ,A.STR1
                                from DAT_XS_DOC A, SYS_CODEVALUE B 
                                WHERE A.FLAG = B.CODE AND B.TYPE  ='DIC_BILLSTATUS' AND BILLTYPE='XST' AND XSTYPE='2' and substr(a.str1,'0','3')='XSG' ";
            string strSearch = "";


            if (lstBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND TRIM(UPPER(A.BILLNO))  LIKE '%{0}%'", lstBILLNO.Text.Trim().ToUpper());
            }
            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedItem.Value);
            }
            if (tgbXSbill.Text.Trim().Length > 0)
            {
                strSearch += string.Format(" AND A.STR1  LIKE '%{0}%'", tgbXSbill.Text.Trim());
            }
            strSearch += string.Format(" AND deptid in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            strSearch += string.Format(" AND A.XSRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.XSRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            //if (!isDg())
            //{
            //    strSearch += string.Format(" AND A.SUPID IN (SELECT SUPID FROM DOC_SUPPLIER WHERE ISDG = 'N')");
            //}
            //else
            //{
            //    strSearch += string.Format(" AND A.SUPID IN (SELECT SUPID FROM DOC_SUPPLIER WHERE ISDG = 'Y')");
            //}
            //if (Request.QueryString["oper"] != null)
            //{
            //    if (Request.QueryString["oper"].ToString() == "audit")
            //    {
            //        //审批
            //        strSearch += " AND A.FLAG <> 'M'";
            //    }
            //    else if (Request.QueryString["oper"].ToString() == "receive")
            //    {
            //        strSearch += " AND A.FLAG <>'M' AND A.FLAG <>'N' ";
            //    }
            //}

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY A.BILLNO DESC";
            highlightRows.Text = "";
            highlightRowYellow.Text = "";
            highlightRowRed.Text = "";
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
            decimal bzslTotal = 0, feeTotal = 0;
            DataTable dtBill = DbHelperOra.Query(string.Format(strComSql, strBillno)).Tables[0];
            string temp = string.Empty;
            if (dtBill != null && dtBill.Rows.Count > 0)
            {
                foreach (DataRow row in dtBill.Rows)
                {
                    //LoadGridRow(row, false, "OLD");
                    bzslTotal += Convert.ToDecimal(row["BZSL"]);
                    feeTotal += Convert.ToDecimal(row["HSJJ"]) * Convert.ToDecimal(row["BZSL"]);
                    temp += row["GDSEQ"].ToString() + ",";
                }
                /*
                *  修 改 人 ：袁鹏    修改时间：2015-03-20
                *  信息说明：这种加载方法比LoadGridRow(row, false, "OLD")更高效
                *  研发组织：威高讯通信息科技有限公司
                */
                hfdGDSEQ.Text = temp.TrimEnd(',');
                PubFunc.GridRowAdd(GridGoods, dtBill);
            }
            //计算合计数量
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("BZSL", bzslTotal.ToString());
            summary.Add("HSJE", feeTotal.ToString("F2"));
            GridGoods.SummaryData = summary;
            PubFunc.FormLock(FormDoc, true, "");
            //按钮显示
            if (docFLAG.SelectedValue == "M" || docFLAG.SelectedValue == "R")
            {
                btnDel.Enabled = true;
                btnSave.Enabled = true;
                btnCommit.Enabled = true;
                btnDelRow.Enabled = true;
                docMEMO.Enabled = true;
                btnAdt.Enabled = false;
                btnAudit.Enabled = false;
                btnCancel.Enabled = false;
                btnPrint.Enabled = false;
            }
            else if (docFLAG.SelectedValue == "N")
            {
                btnDel.Enabled = false;
                btnSave.Enabled = false;
                btnCommit.Enabled = false;
                btnDelRow.Enabled = false;
                btnAdt.Enabled = true;
                btnAudit.Enabled = true;
                btnCancel.Enabled = false;
                btnPrint.Enabled = false;
            }
            else if (docFLAG.SelectedValue == "S")
            {
                btnDel.Enabled = false;
                btnSave.Enabled = false;
                btnCommit.Enabled = false;
                btnDelRow.Enabled = false;
                btnAdt.Enabled = false;
                btnAudit.Enabled = true;
                btnCancel.Enabled = true;
                btnPrint.Enabled = false;
            }
            else
            {
                btnDel.Enabled = false;
                btnSave.Enabled = false;
                btnCommit.Enabled = false;
                btnDelRow.Enabled = false;
                btnAdt.Enabled = false;
                btnAudit.Enabled = false;
                btnCancel.Enabled = false;
                btnPrint.Enabled = true;
            }
            TabStrip1.ActiveTabIndex = 1;
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
                    if (newDict[i]["BZSL"] == null)
                    {
                        Alert.Show("商品【" + newDict[i]["GDNAME"].ToString() + "】退货数量不能为空！！！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(newDict[i]["BZSL"].ToString()) || newDict[i]["BZSL"].ToString() == "0")
                    {
                        Alert.Show("商品【" + newDict[i]["GDNAME"].ToString() + "】退货数量不能为空或者为0！！！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(newDict[i]["HSJJ"].ToString()))
                    {
                        Alert.Show("商品【" + newDict[i]["GDNAME"].ToString() + "】含税进价不能为空！！！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    if (newDict[i]["ISLOT"].ToString() == "2" && string.IsNullOrWhiteSpace(newDict[i]["PH"].ToString()))
                    {
                        Alert.Show("商品【" + newDict[i]["GDNAME"].ToString() + "】批号不能为空！！！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    if (Math.Abs(Convert.ToDecimal(newDict[i]["BZSL"].ToString())) > Math.Abs(Convert.ToDecimal(newDict[i]["NUM2"].ToString())))
                    {
                        Alert.Show("商品【" + newDict[i]["GDNAME"].ToString() + "】退货数量大于可退货数量！！！", "消息提示", MessageBoxIcon.Warning);
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
                string flag = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M') FROM DAT_XS_DOC WHERE SEQNO='{0}'", docBILLNO.Text));
                if (!string.IsNullOrWhiteSpace(flag) && (",M,R").IndexOf(flag) < 0)
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
            MyTable mtType = new MyTable("DAT_XS_DOC");
            mtType.ColRow = PubFunc.FormDataHT(FormDoc);
            mtType.ColRow["SEQNO"] = docBILLNO.Text;
            mtType.ColRow.Add("BILLTYPE", BillType);
            mtType.ColRow.Add("SUBNUM", goodsData.Count);
            mtType.ColRow.Add("XSTYPE", "2");
            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtTypeMx = new MyTable("DAT_XS_COM");
            decimal subNum = 0;//总金额
            //先删除单据信息在插入
            cmdList.Add(new CommandInfo("delete DAT_XS_DOC where seqno='" + docBILLNO.Text + "'", null));//删除单据台头
            cmdList.Add(new CommandInfo("delete DAT_XS_COM where seqno='" + docBILLNO.Text + "'", null));//删除单据明细
            for (int i = 0; i < goodsData.Count; i++)
            {
                mtTypeMx.ColRow = PubFunc.GridDataGet(goodsData[i]);

                if (PubFunc.StrIsEmpty(mtTypeMx.ColRow["BZSL"].ToString()) || mtTypeMx.ColRow["BZSL"].ToString() == "0")
                {
                    Alert.Show("商品【" + mtTypeMx.ColRow["GDSEQ"] + " | " + mtTypeMx.ColRow["GDNAME"] + "】【退货数】为0或空，无法进行【商品退货申请】操作。");
                    return;
                }

                mtTypeMx.ColRow.Add("SEQNO", docBILLNO.Text);
                mtTypeMx.ColRow.Add("ROWNO", i + 1);
                mtTypeMx.ColRow.Add("PHID", mtTypeMx.ColRow["PH"]);
                //if (PubFunc.StrIsEmpty(mtTypeMx.ColRow["DHSL"].ToString()) || mtTypeMx.ColRow["DHSL"].ToString() == "0")
                //{
                //    mtTypeMx.ColRow["DHSL"] = decimal.Parse(mtTypeMx.ColRow["BZHL"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                //}
                //if (PubFunc.StrIsEmpty(mtTypeMx.ColRow["HSJE"].ToString()) || mtTypeMx.ColRow["HSJE"].ToString() == "0")
                //{
                //    mtTypeMx.ColRow["HSJE"] = decimal.Parse(mtTypeMx.ColRow["HSJJ"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZHL"].ToString());
                //}

                mtTypeMx.ColRow["DHSL"] = decimal.Parse(mtTypeMx.ColRow["BZHL"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                if (tgbSTR1.Text.Substring(0, 2) == "DS")
                {
                    mtTypeMx.ColRow["HSJE"] = decimal.Parse(mtTypeMx.ColRow["HSJJ"].ToString()) * decimal.Parse(mtTypeMx.ColRow["DHSL"].ToString());
                }
                else
                {
                    mtTypeMx.ColRow["HSJE"] = decimal.Parse(mtTypeMx.ColRow["HSJJ"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                }
                subNum = subNum + decimal.Parse(mtTypeMx.ColRow["HSJE"].ToString());
                mtTypeMx.ColRow["XSSL"] = mtTypeMx.ColRow["DHSL"];
                mtTypeMx.ColRow["MEMO"] = "高值销售退货";

                //mtTypeMx.ColRow.Add("XSSL", goodsData[i]["DHSL"].ToString());
                mtTypeMx.ColRow.Add("BHSJJ", 0);
                mtTypeMx.ColRow.Add("BHSJE", 0);
                mtTypeMx.ColRow.Add("STR4", mtTypeMx.ColRow["ONECODE"]);
                mtTypeMx.ColRow.Remove("UNITNAME");
                mtTypeMx.ColRow.Remove("PRODUCERNAME");
                cmdList.Add(mtTypeMx.Insert());
            }
            mtType.ColRow.Add("SUBSUM", subNum);
            cmdList.AddRange(mtType.InsertCommand());
            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                Alert.Show("商品退货信息保存成功！");
                OperLog("销售退货", "修改单据【" + docBILLNO.Text + "】");
                billLockDoc(true);
                billOpen(docBILLNO.Text);
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
            PubFunc.GridRowAdd(GridGoods, row, firstRow);
        }

        protected void Window1_Close(object sender, WindowCloseEventArgs e)
        {
            DataTable dt = GetGoods(hfdValue.Text);
            //dt.Columns.Remove(dt.Columns["BZHL"]);
            //string msg = "";
            string msg1 = "";
            if (dt != null && dt.Rows.Count > 0)
            {
                dt.Columns["PIZNO"].ColumnName = "PZWH";
                //dt.Columns["UNITNAME"].ColumnName = "UNITSMALLNAME";
                //dt.Columns["UNIT_SELL_NAME"].ColumnName = "UNITNAME";
                //dt.Columns["BZHL_SELL"].ColumnName = "BZHL";

                dt.Columns.Add("PH", Type.GetType("System.String"));
                dt.Columns.Add("RQ_SC", Type.GetType("System.String"));
                dt.Columns.Add("YXQZ", Type.GetType("System.String"));
                dt.Columns.Add("MEMO", Type.GetType("System.String"));
                dt.Columns.Add("BZSL", Type.GetType("System.Int32"));
                dt.Columns.Add("DHS", Type.GetType("System.Int32"));
                dt.Columns.Add("DHSL", Type.GetType("System.Int32"));
                dt.Columns.Add("HSJE", Type.GetType("System.Double"));
                dt.Columns.Add("SPZTSL", Type.GetType("System.Int32"));
                dt.Columns.Add("KCSL", Type.GetType("System.Int32"));

                foreach (DataRow row in dt.Rows)
                {
                    row["BZSL"] = "0";
                    row["DHSL"] = "0";
                    row["DHS"] = "0";
                    row["HSJE"] = "0";
                    row["KCSL"] = "0";
                    //查询代管库存
                    //DataTable Temp = DbHelperOra.Query(string.Format("SELECT A.KCSL,A.PH,A.YXQZ,A.RQ_SC,B.ISGZ,B.GDNAME FROM DAT_GOODSSTOCK A ,DOC_GOODS B WHERE A.DEPTID ='{0}' AND A.GDSEQ = '{1}'  AND A.GDSEQ = B.GDSEQ AND A.KCSL >0 AND ROWNUM = 1 AND A.SUPID ='00002' ORDER BY A.PICINO ASC", docDEPTID.SelectedValue, row["GDSEQ"].ToString())).Tables[0];
                    //if (Temp.Rows.Count > 0)
                    //{
                    //    row["KCSL"] = Temp.Rows[0]["KCSL"];
                    //    row["PH"] = Temp.Rows[0]["PH"];
                    //    row["YXQZ"] = Temp.Rows[0]["YXQZ"];
                    //    row["RQ_SC"] = Temp.Rows[0]["RQ_SC"];
                    //}
                    //else
                    //{
                    //    msg += row["GDNAME"] + ",";
                    //    continue;
                    //}
                    if (string.IsNullOrWhiteSpace(row["HSJJ"].ToString()) || row["HSJJ"].ToString() == "0")
                    {
                        msg1 += "【" + row["GDSEQ"].ToString() + " | " + row["GDNAME"].ToString() + "】,";
                        continue;
                    }
                    LoadGridRow(row, false);
                }

                //if (!string.IsNullOrWhiteSpace(msg))
                //{
                //    String strNostock = "";
                //    strNostock = string.Format("商品【{0}】在部门『{1}』中没有库存,不能进行录入！", msg, docDEPTID.SelectedText);
                //    Alert.Show(strNostock, "消息提示", MessageBoxIcon.Warning);
                //}
                if (!string.IsNullOrWhiteSpace(msg1))
                {
                    String strNostock = "";
                    strNostock = string.Format("商品【{0}】【含税进价】为空,不能进行【商品退货审批】操作。", msg1);
                    Alert.Show(strNostock, "消息提示", MessageBoxIcon.Warning);
                }
            }
            else
            {
                Alert.Show("系统传值错误！！！", "消息提示", MessageBoxIcon.Warning);
            }
        }

        protected void trbEditorGDSEQ_TriggerClick(object sender, EventArgs e)
        {
            string code = "";
            string dept = docDEPTID.SelectedValue;

            if (!string.IsNullOrWhiteSpace(code) && code.Trim().Length >= 2)
            {
                DataTable dt_goods = Doc.GetGoods(code, "", dept);
                if (dt_goods != null && dt_goods.Rows.Count > 0)
                {
                    dt_goods.Columns.Add("BZSL", Type.GetType("System.Int32"));
                    dt_goods.Columns.Add("DHS", Type.GetType("System.Int32"));
                    dt_goods.Columns.Add("DHSL", Type.GetType("System.Int32"));
                    dt_goods.Columns.Add("HSJE", Type.GetType("System.Double"));
                    dt_goods.Columns.Add("SPZTSL", Type.GetType("System.Int32"));
                    DataRow dr_goods = dt_goods.Rows[0];
                    LoadGridRow(dr_goods);
                }
                else
                {
                    Alert.Show(string.Format("{0}尚未配置商品【{1}】！！！", docDEPTID.SelectedText, code), MessageBoxIcon.Warning);
                    PubFunc.GridRowAdd(GridGoods, "CLEAR");
                }
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
                    Dictionary<string, object> newDict = GridGoods.GetNewAddedList()[Convert.ToInt16(strCell[0])];
                    newDict["PH"] = row.Values[3];
                    newDict["YXQZ"] = row.Values[4];
                    newDict["PZWH"] = row.Values[9];
                    newDict["RQ_SC"] = row.Values[5];
                    newDict["KCSL"] = row.Values[6];
                    newDict["BZSL"] = tbxNumber.Text;
                    if (firstRow)
                    {
                        firstRow = false;
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
                Alert.Show("请选择需要删除的单据");
                return;
            }
            if ((",M,R").IndexOf(docFLAG.SelectedValue) < 0)
            {
                Alert.Show("非新单不能删除！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            if (!string.IsNullOrWhiteSpace(lstBILLNO.Text.Trim()))
            {
                string flag = DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M')  FROM DAT_XS_DOC WHERE BILLNO='{0}'", lstBILLNO.Text.Trim())).ToString();
                if (!string.IsNullOrWhiteSpace(flag) && (",M,R").IndexOf(docFLAG.SelectedValue) < 0)
                {
                    Alert.Show("非新单，不能删除！", "消息提示", MessageBoxIcon.Warning);
                    return;
                }
            }

            DbHelperOra.ExecuteSql("Delete from DAT_XS_DOC t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            DbHelperOra.ExecuteSql("Delete from DAT_XS_COM t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            DbHelperOra.ExecuteSql("Delete from DAT_XS_EXT t WHERE T.BILLNO ='" + docBILLNO.Text.Trim() + "'");
            Alert.Show("单据【" + docBILLNO.Text + "】删除成功!");
            OperLog("销售退货", "删除单据【" + docBILLNO.Text + "】");
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
        protected override void billAudit()
        {
            if (Doc.DbGetSysPara("LOCKSTOCK") == "Y")
            {
                Alert.Show("系统库存已被锁定，请等待物资管理科结存处理完毕再做审核处理！", "消息提醒", MessageBoxIcon.Warning);
                return;
            }
            if (!string.IsNullOrWhiteSpace(docSEQNO.Text))
            {
                List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().ToList();

                //for (int i = 0; i < newDict.Count; i++)
                //{
                //    string strGdseq = newDict[i]["GDSEQ"].ToString();
                //    string strGdname = newDict[i]["GDNAME"].ToString();
                //    //if (!DbHelperOra.Exists(string.Format("select 1 from doc_goodscfg where gdseq = '{0}' ", strGdseq /*docDEPTOUT.SelectedValue*/)))
                //    //{
                //    //    Alert.Show("第【" + (i + 1) + "】行商品【" + strGdseq + " | " + strGdname + "】在科室【" + docDEPTOUT.SelectedValue + "】未配置", "消息提示", MessageBoxIcon.Warning);
                //    //    return;
                //    //}
                //}
                string msg = "";
                DataTable dtCom = DbHelperOra.Query(string.Format("SELECT SUM(ABS(A.DHSL)) DHSL,A.GDSEQ,B.GDNAME FROM DAT_XS_COM A,DOC_GOODS B WHERE A.GDSEQ = B.GDSEQ AND B.ISGZ = 'Y' AND SEQNO = '{0}' GROUP BY A.GDSEQ,B.GDNAME", docSEQNO.Text)).Tables[0];
                if (dtCom != null && dtCom.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtCom.Rows)
                    {
                        string checkCount = DbHelperOra.GetSingle(string.Format("SELECT COUNT(1) FROM DAT_XS_EXT WHERE BILLNO='{0}' AND GDSEQ ='{1}'", docSEQNO.Text, dr["GDSEQ"].ToString())).ToString();
                        if (int.Parse(dr["DHSL"].ToString()) > int.Parse(checkCount))
                        {
                            msg += "【" + dr["GDSEQ"] + "-" + dr["GDNAME"] + "】,";
                            continue;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(msg))
                    {
                        Alert.Show("高值商品中 " + msg + "存在追溯码未扫描", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                }

                string strBillno = docSEQNO.Text;
                if (BillOper(strBillno, "PASS1") == 1)
                {
                    billLockDoc(true);
                    Alert.Show("单据【" + strBillno + "】审核成功！");
                    OperLog("销售退货", "审核单据【" + strBillno + "】");
                    billOpen(strBillno);
                    docFLAG.SelectedValue = "Y";
                }
            }
        }

        protected void btnAdt_Click(object sender, EventArgs e)
        {
            //设备科审核
            if (!string.IsNullOrWhiteSpace(docSEQNO.Text))
            {
                string flag = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M') FROM DAT_XS_DOC WHERE SEQNO='{0}'", docSEQNO.Text));
                if (!string.IsNullOrWhiteSpace(flag) && (",M,R,N").IndexOf(docFLAG.SelectedValue) < 0)
                {
                    Alert.Show("已审批或者已审核,不能审批！", "消息提示", MessageBoxIcon.Warning);
                    return;
                }

                bool flg = true;
                if ((",M,R").IndexOf(flag) > 0)
                {
                    flg = CommitData();
                }

                if (flg)
                {
                    if (DbHelperOra.Exists("SELECT 1 FROM DAT_XS_DOC WHERE BILLTYPE = 'XST' AND SEQNO = '" + docSEQNO.Text + "'"))
                    {
                        string Sql = "UPDATE DAT_XS_DOC SET FLAG = 'S',SPR='{0}',SPRQ=sysdate WHERE BILLTYPE = 'XST' AND SEQNO = '{1}'";
                        DbHelperOra.ExecuteSql(string.Format(Sql, UserAction.UserID, docSEQNO.Text));
                        billOpen(docSEQNO.Text);
                        Alert.Show("单据【" + docSEQNO.Text + "】审批成功！");
                        OperLog("销售退货", "审批单据【" + docSEQNO.Text + "】");
                        docFLAG.SelectedValue = "S";
                    }
                    else
                    {
                        Alert.Show("请重新确认单据信息！");
                    }
                }
            }
        }

        protected override void billCancel()
        {
            //将选中单据驳回
            if ((",S").IndexOf(docFLAG.SelectedValue) < 0)
            {
                Alert.Show("不是已审批单据,不能驳回", "操作提示", MessageBoxIcon.Warning);
                return;
            }
            WindowReject.Hidden = false;
        }
        protected void btnRejectSubmit_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(docSEQNO.Text.Trim()))
            {
                string flag = DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M')  FROM DAT_XS_DOC WHERE BILLNO='{0}'", docSEQNO.Text.Trim())).ToString();

                if (flag == "Y")
                {
                    Alert.Show("已审核单据,不能驳回", "操作提示", MessageBoxIcon.Warning);
                    return;
                }
                string strMemo = "";
                if (!string.IsNullOrWhiteSpace(ddlReject.SelectedValue))
                {
                    strMemo = "驳回原因：" + ddlReject.SelectedText;
                }
                else
                {
                    Alert.Show("请选择【驳回原因】");
                    return;
                }
                if (!string.IsNullOrWhiteSpace(txaMemo.Text.Trim()))
                {
                    strMemo += "；详细说明：" + txaMemo.Text;
                }
                List<CommandInfo> cmdList = new List<CommandInfo>();
                string sSQL = "update DAT_XS_DOC set flag='R',SHRQ=sysdate,memo ='" + strMemo + "',SHR='" + UserAction.UserID + "' where seqno ='" + docSEQNO.Text + "'";
                string sSQL1 = " DELETE  FROM DAT_XS_EXT WHERE BILLNO ='" + docSEQNO.Text + "'";
                cmdList.Add(new CommandInfo(sSQL, null));
                cmdList.Add(new CommandInfo(sSQL1, null));
                if (DbHelperOra.ExecuteSqlTran(cmdList))
                {
                    WindowReject.Hidden = true;
                    billSearch();
                    billOpen(docSEQNO.Text);
                    docFLAG.SelectedValue = "R";
                    OperLog("销售退货", "驳回单据【" + docSEQNO.Text + "】");
                }
            }
        }
        #region 高值退货
        protected void btnScan_Click(object sender, EventArgs e)
        {
            if (PubFunc.StrIsEmpty(docSEQNO.Text))
            {
                Alert.Show("请保存单据后进行扫描追溯码操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_XS_COM A,DOC_GOODS B WHERE A.GDSEQ = B.GDSEQ AND B.ISGZ = 'Y' AND SEQNO = '{0}'", docSEQNO.Text)))
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
                sql = "SELECT A.*,f_getunitname(A.UNIT) UNITNAME FROM DAT_XS_EXT A WHERE A.BILLNO = '{0}' ORDER BY A.GDSEQ,A.INSTIME DESC";
            }
            else
            {
                sql = "SELECT A.*,f_getunitname(A.UNIT) UNITNAME FROM DAT_XS_EXT A WHERE A.BILLNO = '{0}' ORDER BY A.INSTIME DESC";
            }
            DataTable dtScan = DbHelperOra.Query(string.Format(sql, docSEQNO.Text)).Tables[0];
            //PubFunc.GridRowAdd(GridSacn, dtScan);
            GridSacn.DataSource = dtScan;
            GridSacn.DataBind();
            zsmScan.Text = String.Empty;
            zsmScan.Focus();
            if (docFLAG.SelectedValue == "M" || docFLAG.SelectedValue == "R")
            {
                zsmScan.Enabled = false;
                zsmDelete.Enabled = false;
            }
            else
            {
                zsmScan.Enabled = true;
                zsmDelete.Enabled = true;
            }
        }
        protected void zsmScan_TextChanged(object sender, EventArgs e)
        {
            string flag = DbHelperOra.GetSingle("SELECT FLAG FROM DAT_XS_DOC WHERE BILLNO ='" + docBILLNO.Text.Trim() + "'").ToString();
            if ((",S,N").IndexOf(docFLAG.SelectedValue) < 0)
            {
                Alert.Show("非『已提交』单据不允许操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
       
            if (DbHelperOra.Exists(string.Format("SELECT COUNT(1) FROM DAT_XS_EXT WHERE  BILLNO='{0}' AND UPPER(ONECODE) = UPPER('{1}')", docBILLNO.Text.Trim(), zsmScan.Text)))
            {
                Alert.Show("您输入的追溯码已销退,请检查！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            if (zsmScan.Text.Length<1)
            {
                Alert.Show("请输入追溯码！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            if (zsmScan.Text.Substring(0, 1) != "2")
            {
                Alert.Show("您扫描的条码不是贵重码,请检查！", "提示信息", MessageBoxIcon.Warning);
                zsmScan.Text = string.Empty;
                zsmScan.Focus();
                return;
            }
            String S = string.Format("SELECT 1 FROM DAT_XS_EXT WHERE BILLNO='{0}' AND UPPER(ONECODE) = UPPER('{1}')", tgbSTR1.Text.Trim(), zsmScan.Text.Trim());
            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_XS_EXT WHERE BILLNO='{0}' AND UPPER(ONECODE) = UPPER('{1}') ", tgbSTR1.Text.Trim(), zsmScan.Text.Trim())))
            {
                Alert.Show("您输入的追溯码不包含在该销售单列表中,请检查！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_GZ_EXT DGE,DAT_XS_COM DXC WHERE DGE.GDSEQ=DXC.GDSEQ  AND DGE.PH=DXC.PH AND DXC.SEQNO='{0}' AND DGE.ONECODE='{1}'", docBILLNO.Text.Trim(), zsmScan.Text.Trim())))
            {
                Alert.Show("您输入的追溯码不包含在该销售单列表中,请检查！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_XS_COM DXC WHERE  DXC.SEQNO='{0}' AND DXC.STR4='{1}'", docBILLNO.Text.Trim(), zsmScan.Text.Trim())))
            {
                Alert.Show("您输入的追溯码不包含在该销售退单列表中,请检查！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            if (hfdGDSEQ.Text.Trim().Length > 0)
            {
                string[] strSplit = hfdGDSEQ.Text.Split(',');
                bool b = true;
                if (strSplit.Length > 0)
                {
                    for (int i = 0; i < strSplit.Length; i++)
                    {
                        if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_GZ_EXT WHERE GDSEQ='{0}' AND ONECODE='{1}'", strSplit[i], zsmScan.Text)))
                        {
                            b = false;
                            break;
                        }
                    }
                    if (b)
                    {
                        Alert.Show("您输入的追溯码对应的商品不在列表中,请检查！", "提示信息", MessageBoxIcon.Warning);
                        return;
                    }
                }
            }
            if (GridSacn.Rows.Count > 0)
            {

                for (int index = 0; index < GridSacn.Rows.Count; index++)
                {
                    if (GridSacn.Rows[index].DataKeys[0].ToString().ToUpper() == zsmScan.Text.ToUpper())
                    {
                        Alert.Show("您输入的追溯码与扫描列表中第【" + (index + 1).ToString() + "】行的追溯码重复,请检查！", "提示信息", MessageBoxIcon.Warning);
                        zsmScan.Text = string.Empty;
                        zsmScan.Focus();
                        return;
                    }
                }

            }
            if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_GZ_EXT WHERE UPPER(ONECODE) = UPPER('{0}') AND FLAG <> 'G'", zsmScan.Text)))
            {
                Alert.Show("您输入的追溯码未被使用或已退货,请检查！", "提示信息", MessageBoxIcon.Warning);
                zsmScan.Text = string.Empty;
                zsmScan.Focus();
                return;
            }

            //写入数据库中
            string sSQL = string.Format(@"INSERT INTO DAT_XS_EXT(DEPTID,BILLNO,ROWNO,ONECODE,GDSEQ,GDNAME,BARCODE,UNIT,GDSPEC,DEPTCUR,BZHL,PH,RQ_SC,YXQZ,INSTIME)
                    SELECT '{0}','{1}',NVL((SELECT MAX(ROWNO)+1 FROM DAT_XS_EXT WHERE BILLNO = '{1}'),1),'{2}',GDSEQ,GDNAME,BARCODE,UNIT,GDSPEC,DEPTCUR,BZHL,PH,RQ_SC,YXQZ,SYSDATE
                    FROM DAT_XS_EXT A
                    WHERE BILLNO='{3}' AND A.ONECODE = '{2}'", docDEPTID.SelectedValue, docBILLNO.Text, zsmScan.Text.Trim().ToUpper(), tgbSTR1.Text.Trim());

            DbHelperOra.ExecuteSql(sSQL);

            string str5 = "";
            DataTable dt = null;
            try
            {
                dt = DbHelperOra.Query("SELECT t.ROWNO,t.gdseq FROM DAT_XS_ext T WHERE T.ONECODE='" + zsmScan.Text.ToUpper() + "' and t.billno='" + tgbSTR1.Text + "'").Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    str5 = DbHelperOra.GetSingle(string.Format("SELECT T.SEQNO FROM DAT_GOODSJXC T WHERE T.BILLNO='{0}' AND T.ROWNO='{1}' AND T.GDSEQ='{2}'", tgbSTR1.Text, dt.Rows[0]["rowno"], dt.Rows[0]["gdseq"])).ToString();
                }
            }
            catch
            {
                str5 = tgbSTR1.Text;
            }
            DbHelperOra.ExecuteSql("UPDATE DAT_XS_com T SET T.str1='" + str5 + "' WHERE T.seqno='" + docBILLNO.Text + "' and length(nvl(str5,' '))<2 AND T.GDSEQ='" + dt.Rows[0]["gdseq"] + "'");

            ScanSearch("");
        }

        protected void zsmDelete_Click(object sender, EventArgs e)
        {
            if ((",M,S").IndexOf(docFLAG.SelectedValue) < 0)
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
            DbHelperOra.ExecuteSql(string.Format("DELETE FROM DAT_XS_EXT T WHERE ONECODE='{0}' AND BILLNO LIKE 'XST%'", onecode));
            ScanSearch("");
        }
        #endregion
        protected void GridList_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                string flag = row["FLAG"].ToString();

                if (flag == "新单")
                {
                    highlightRows.Text += e.RowIndex.ToString() + ",";
                }
                if (flag == "已提交")
                {
                    highlightRowYellow.Text += e.RowIndex.ToString() + ",";
                }
                if (flag == "已驳回")
                {
                    highlightRowRed.Text += e.RowIndex.ToString() + ",";
                }
                //if (Request.QueryString["oper"].ToString() == "receive" && flag == "已审批")
                //{
                //    highlightRowYellow.Text += e.RowIndex.ToString() + ",";
                //}
            }
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            WindowLot.Hidden = true;
        }

        protected void btnCommit_Click(object sender, EventArgs e)
        {
            CommitData();
        }

        private bool CommitData()
        {
            if (PubFunc.StrIsEmpty(docSEQNO.Text))
            {
                Alert.Show("请保存单据后提交！", "消息提示", MessageBoxIcon.Warning);
                return false;
            }
            string flag = DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M')  FROM DAT_XS_DOC WHERE BILLNO='{0}'", docSEQNO.Text.Trim())).ToString();
            if (("M,R").IndexOf(flag) < 0)
            {
                Alert.Show("非新单，不能提交！", "消息提示", MessageBoxIcon.Warning);
                return false;
            }
            //验证单据数量是否符合要求
            object gdseq = DbHelperOra.GetSingle(string.Format(@"SELECT A.GDSEQ
                    FROM DAT_XS_COM A,DAT_XS_COM B
                    WHERE A.SEQNO = '{0}' AND B.SEQNO = '{1}' AND A.STR1 = B.ROWNO
                    AND ABS(A.BZSL) > (B.XSSL - NVL(B.NUM1,0)) AND ROWNUM =1", docBILLNO.Text, tgbSTR1.Text));
            //string msg = "";
            List<CommandInfo> cmdList = new List<CommandInfo>();

            DataTable dtCom = DbHelperOra.Query(string.Format("SELECT SUM(ABS(A.DHSL)) DHSL,A.GDSEQ,B.GDNAME FROM DAT_XS_COM A,DOC_GOODS B WHERE A.GDSEQ = B.GDSEQ AND B.ISGZ = 'Y' AND SEQNO = '{0}' GROUP BY A.GDSEQ,B.GDNAME", docSEQNO.Text)).Tables[0];
            if (dtCom != null && dtCom.Rows.Count > 0)
            {
                //foreach (DataRow dr in dtCom.Rows)
                //{
                //    string checkCount = DbHelperOra.GetSingle(string.Format("SELECT COUNT(1) FROM DAT_XS_EXT WHERE BILLNO='{0}' AND GDSEQ ='{1}'", docSEQNO.Text, dr["GDSEQ"].ToString())).ToString();
                //    if (int.Parse(dr["DHSL"].ToString()) > int.Parse(checkCount))
                //    {
                //        msg += "【" + dr["GDSEQ"] + "," + dr["GDNAME"] + "】,";
                //        continue;
                //    }
                //}

                //if (!string.IsNullOrWhiteSpace(msg))
                //{
                //    Alert.Show("高值商品中 " + msg + "存在追溯码未扫描", "消息提示", MessageBoxIcon.Warning);
                //    return false;
                //}

                string sSQL1 = "CALL P_EXE_XSD('" + docSEQNO.Text + "')";
                cmdList.Add(new CommandInfo(sSQL1, null));
            }

            string sSQL2 = string.Format("UPDATE DAT_XS_DOC SET FLAG = 'S',SPR='{0}',SPRQ=sysdate WHERE BILLTYPE = 'XST' AND SEQNO = '{1}'", UserAction.UserID, docSEQNO.Text);
            cmdList.Add(new CommandInfo(sSQL2, null));

            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                billOpen(docSEQNO.Text);
                Alert.Show("单据【" + docSEQNO.Text + "】提交成功！");
                docFLAG.SelectedValue = "N";
                return true;
            }
            else
            {
                return false;
            }
        }

        protected void GridList_Sort(object sender, GridSortEventArgs e)
        {
            highlightRows.Text = "";
            highlightRowYellow.Text = "";
            highlightRowRed.Text = "";
            GridList.SortDirection = e.SortDirection;
            GridList.SortField = e.SortField;

            DataTable table = PubFunc.GridDataGet(GridList);
            DataView view1 = table.DefaultView;
            view1.Sort = String.Format("{0} {1}", GridList.SortField, GridList.SortDirection);
            GridList.DataSource = view1;
            GridList.DataBind();
        }

        protected void tgbTHDH_TriggerClick(object sender, EventArgs e)
        {
            if (PubFunc.StrIsEmpty(tgbSTR1.Text)) return;
            //验证表头信息是否符合规范
            if (/*PubFunc.StrIsEmpty(docDEPTOUT.SelectedValue) || */PubFunc.StrIsEmpty(docSTR2.SelectedValue))
            {
                Alert.Show("单据头未维护完整,请检查!!", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            if(tgbSTR1.Text.IndexOf("XSG")<0)
            {
                Alert.Show("只支持高值销售单进行退货，请检查!!","提示信息", MessageBoxIcon.Warning);
                return;
            }
            //判断从GOODS表里取哪个NAME
            string sqls = "";
            if (DbHelperOra.Exists("SELECT 1 FROM SYS_PARA WHERE CODE = 'ShowName' AND VALUE = 'HIS'"))
            {
                sqls = @"SELECT A.GDSEQ,
                           DECODE(B.HISNAME,B.HISNAME,B.GDNAME) GDNAME,
                           DECODE(B.HISCODE,B.HISCODE,B.GDSPEC) GDSPEC,";

            }
            else
            {
                sqls = @"SELECT A.GDSEQ,
                           B.GDNAME,
                           B.GDSPEC,";

            }
            sqls = sqls + @"f_getunitname(B.UNIT) UNITNAME,
                           B.BZHL,D.ONECODE,
                           ABS(A.SL) - NVL(C.NUM1,0) KCSL,
                           '0' SL,  A.PSSID,F_GETSUPNAME(A.PSSID) PSSNAME,     
                           DECODE(NVL(C.NUM3, 0), 1, '赠品', '非赠品') NUM3NAME,
                           F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                           A.PZWH,A.PH,A.YXQZ,A.RQ_SC,A.ROWNO,A.DEPTID,A.SEQNO JXCNO
                      FROM DAT_GOODSJXC A, DOC_GOODS B, DAT_XS_COM C,DAT_XS_EXT D
                     WHERE A.GDSEQ = B.GDSEQ
                       AND A.GDSEQ = C.GDSEQ
                       AND A.BILLNO = C.SEQNO
                       AND A.ROWNO = C.ROWNO AND C.SEQNO=D.BILLNO AND C.ROWNO=D.ROWNO
                       AND A.BILLNO = '{0}'";
            DataTable dt = DbHelperOra.Query(string.Format(sqls, tgbSTR1.Text)).Tables[0];

            //绑定数据源
            //DataTable dt = DbHelperOra.Query(string.Format(@"SELECT B.*,A.DEPTID,(ABS(B.BZSL) - NVL(B.NUM1,0)) KCSL,'0' SL,
            //           f_getunitname(B.UNIT) UNITNAME,f_getproducername(B.PRODUCER) PRODUCERNAME,DECODE(NVL(B.NUM3,0),1,'赠品','非赠品') NUM3NAME
            //            FROM DAT_XS_DOC A,DAT_XS_COM B
            //            WHERE A.SEQNO = B.SEQNO AND B.SEQNO = '{0}' AND ABS(B.BZSL) > NVL(B.NUM1,0)", tgbSTR1.Text)).Tables[0];

            if (dt.Rows.Count < 1)
            {
                Alert.Show("单号【" + tgbSTR1.Text + "】已被退货或输入的单号不正确,请检查！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            if (PubFunc.StrIsEmpty(docDEPTID.SelectedValue))
            {
                docDEPTID.SelectedValue = dt.Rows[0]["DEPTID"].ToString();
            }
            else if (docDEPTID.SelectedValue != dt.Rows[0]["DEPTID"].ToString())
            {
                Alert.Show("此单据退货科室【" + dt.Rows[0]["DEPTID"].ToString() + "】与单据头定义的科室【" + docDEPTID.SelectedValue + "】不一致，请检查！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            GrdRtn.DataSource = dt;
            GrdRtn.DataBind();
            //增加单据显示
            Win_Rtn.Title = "退货单据信息 - " + tgbSTR1.Text;
            Win_Rtn.Hidden = false;
            PubFunc.FormLock(FormDoc, true, "");
            docMEMO.Enabled = true;
        }

        protected void GodClose_Click(object sender, EventArgs e)
        {
            Win_Rtn.Hidden = true;
        }

        protected void GodSure_Click(object sender, EventArgs e)
        {
            foreach (GridRow row in GrdRtn.Rows)
            {
                int rowIndex = row.RowIndex;
                System.Web.UI.WebControls.TextBox tbxNumber = (System.Web.UI.WebControls.TextBox)GrdRtn.Rows[rowIndex].FindControl("tbxNum");
                if (!string.IsNullOrWhiteSpace(tbxNumber.Text) && tbxNumber.Text != "0")
                {
                    //DataTable dt = DbHelperOra.Query(string.Format(@"SELECT A.*,f_getunitname(A.UNIT) UNITNAME,B.PRODUCER,f_getproducername(B.PRODUCER) PRODUCERNAME,'' STR1,DECODE(NVL(A.NUM3,0),0,'非赠品','赠品') NUM3NAME
                    //    FROM DAT_XS_COM A,DOC_GOODS B
                    //    WHERE A.SEQNO = '{0}' AND A.GDSEQ = B.GDSEQ AND A.GDSEQ = '{1}' AND A.ROWNO = '{2}'", tgbSTR1.Text, GrdRtn.DataKeys[rowIndex][0].ToString(), GrdRtn.DataKeys[rowIndex][1])).Tables[0];

                    //判断从GOODS表里取哪个NAME
                    string sqls = "";
                    if (DbHelperOra.Exists("SELECT 1 FROM SYS_PARA WHERE CODE = 'ShowName' AND VALUE = 'HIS'"))
                    {
                        sqls = @"SELECT A.GDSEQ,
                           DECODE(B.HISNAME,B.HISNAME,B.GDNAME) GDNAME,
                           DECODE(B.HISCODE,B.HISCODE,B.GDSPEC) GDSPEC,";

                    }
                    else
                    {
                        sqls = @"SELECT A.GDSEQ,
                           B.GDNAME,
                           B.GDSPEC,";

                    }
                    sqls = sqls + @"f_getunitname(B.UNIT) UNITNAME,
                           B.BZHL,
                           ABS(A.SL) KCSL,
                           '0' SL, D.ONECODE,
                           DECODE(NVL(C.NUM3, 0), 1, '赠品', '非赠品') NUM3NAME,
                           f_getproducername(B.PRODUCER) PRODUCERNAME,
                           A.PZWH,A.PH,A.YXQZ,A.RQ_SC,A.ROWNO,A.DEPTID,C.*,A.SEQNO JXCNO
                      FROM DAT_GOODSJXC A, DOC_GOODS B, DAT_XS_COM C,DAT_XS_EXT D
                     WHERE A.GDSEQ = B.GDSEQ
                       AND A.GDSEQ = C.GDSEQ
                       AND A.BILLNO = C.SEQNO
                       AND A.ROWNO = C.ROWNO AND C.SEQNO=D.BILLNO AND C.ROWNO=D.ROWNO
                       AND A.BILLNO = '{0}' AND A.GDSEQ = '{1}' AND A.ROWNO = '{2}' AND A.PSSID = '{3}'";
                    DataTable dt = DbHelperOra.Query(string.Format(sqls, tgbSTR1.Text, GrdRtn.DataKeys[rowIndex][0].ToString(), GrdRtn.DataKeys[rowIndex][1], GrdRtn.DataKeys[rowIndex][4])).Tables[0];


                    foreach (DataRow rows in dt.Rows)
                    {
                        //rows["STR1"] = GrdRtn.DataKeys[rowIndex][1];
                        rows["STR1"] = GrdRtn.DataKeys[rowIndex][5];
                        rows["STR3"] = GrdRtn.DataKeys[rowIndex][4];
                        rows["NUM2"] = GrdRtn.DataKeys[rowIndex][2];
                        rows["BZSL"] = -Math.Abs(Convert.ToDecimal(tbxNumber.Text));
                        if (tgbSTR1.Text.Substring(0, 2) == "DS")
                        {
                            rows["HSJE"] = decimal.Parse(rows["HSJJ"].ToString()) * decimal.Parse(rows["DHSL"].ToString());
                        }
                        else
                        {
                            rows["HSJE"] = decimal.Parse(rows["HSJJ"].ToString()) * decimal.Parse(rows["BZSL"].ToString());
                        }
                        LoadGridRow(rows, false);
                    }
                }
            }
            Win_Rtn.Hidden = true;
        }

        protected void lstBILLNO_TriggerClick(object sender, EventArgs e)
        {
            billSearch();
        }
    }
}