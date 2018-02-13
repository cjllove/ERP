﻿using FineUIPro;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using XTBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPProject.ERPApply
{
    public partial class High_valueConsumable : BillBase
    {
        private string lstSql = @"SELECT A.SEQNO,A.BILLNO,A.FLAG,B.NAME FLAGNAME,F_GETDEPTNAME(A.DEPTID) DEPTID,A.XSRQ,F_GETDEPTNAME(A.DEPTOUT) DEPTOUT,
                                     A.SUBNUM,F_GETUSERNAME(A.LRY) LRY,A.LRRQ,A.SHR,A.SHRQ,A.MEMO,A.DOCTOR,A.OPTTABLE,A.OPTDATE,A.SUBSUM
                                from DAT_XS_DOC A, SYS_CODEVALUE B 
                                WHERE A.FLAG = B.CODE AND B.TYPE  ='DIC_BILLSTATUS' AND BILLTYPE='XSG' AND XSTYPE='1' ";
        private string strDocSql = "SELECT * FROM DAT_XS_DOC WHERE SEQNO ='{0}'";
        private string strComSql = "SELECT A.*,F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME,f_getunitname(A.UNIT) UNITNAME,f_getunitname(B.UNIT) UNITSMALLNAME  FROM DAT_XS_COM A,DOC_GOODS B WHERE A.SEQNO ='{0}' AND A.GDSEQ = B.GDSEQ ORDER BY ROWNO";
        public override Field[] LockControl
        {
            get { return new Field[] { trbBARCODE, docBILLNO, docDEPTID, docXSRQ, docCUSTID, docOPTID, docOPTTABLE, docOPTDATE, docDOCTOR, docSTR2 }; }
        }

        public High_valueConsumable()
        {
            BillType = "XSG";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //屏蔽不需要的操作按钮
                if (Request.QueryString["osid"] != null)
                {
                    if (Request.QueryString["osid"].ToString() == "input")
                    {
                        ButtonHidden(btnExport, btnAudit, btnCancel);
                    }
                    else if (Request.QueryString["osid"].ToString() == "audit")
                    {
                        //打印高值销售单
                        btnPrint.Text = "打 印";
                        btnPrint.OnClientClick = "btnPrint_Bill()";
                        billLockDoc(true);
                        ButtonHidden(btnExport, btnSave, btnDelRow, btnNew, btnTJ, btnDel);
                    }
                    else if (Request.QueryString["osid"].ToString() == "querylist")
                    {
                        string result = listSearchForPad();
                        Response.ContentType = "text/plain";
                        Response.Write(result);
                        Response.End();
                    }
                    else if (Request.QueryString.Get("osid") == "queryBill" && !string.IsNullOrEmpty(Request.QueryString.Get("billno")))
                    {
                        string result = billSearchForPad();
                        Response.ContentType = "text/plain";
                        Response.Write(result);
                        Response.End();
                    }
                    else if (Request.QueryString.Get("osid") == "scan" && !string.IsNullOrEmpty(Request.QueryString.Get("onecode")) && !string.IsNullOrEmpty(Request.QueryString.Get("deptid")))
                    {
                        string result = scanForPad();
                        Response.ContentType = "text/plain";
                        Response.Write(result);
                        Response.End();
                    }
                    else if (Request.QueryString.Get("osid") == "save")
                    {
                        string result = billSaveForPad();
                        Response.ContentType = "text/plain";
                        Response.Write(result);
                        Response.End();
                    }
                    else if (Request.QueryString.Get("osid") == "submit")
                    {
                        string result = billSubmitForPad();
                        Response.ContentType = "text/plain";
                        Response.Write(result);
                        Response.End();
                    }
                    else if (Request.QueryString.Get("osid") == "delete")
                    {
                        string result = billDeleteForPad();
                        Response.ContentType = "text/plain";
                        Response.Write(result);
                        Response.End();
                    }
                }
                DataInit();
                #region 读取客户标记，更改客户化需求
                string USERXMID = "";
                USERXMID = (DbHelperOra.GetSingle("select VALUE from sys_para WHERE CODE='USERXMID'")).ToString();
                if (USERXMID == "TJ_YKGZ") TJ_YKGZ();
                #endregion
                billNew();
            }
        }
        protected bool state()
        {
            if (Request.QueryString["osid"].ToString() == "input")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private void DataInit()
        {
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;

            PubFunc.DdlDataGet("DDL_USER", docSHR, docLRY);
            //PubFunc.DdlDataGet("DDL_SYS_DEPTRANGE", UserAction.UserID, docDEPTID, lstDEPTID);
            DepartmentBind.BindDDL("DDL_SYS_DEPTRANGE", UserAction.UserID, docDEPTID, lstDEPTID);
            PubFunc.DdlDataGet(docFLAG, "DDL_BILL_STATUS");
        }

        protected override void billNew()
        {
            //原单据保存判断
            string strDept = docDEPTID.SelectedValue;
            PubFunc.FormDataClear(FormDoc);

            docFLAG.SelectedValue = "M";
            docLRY.SelectedValue = UserAction.UserID;
            //docSHR.SelectedValue = UserAction.UserID;
            docLRRQ.SelectedDate = DateTime.Now;
            docXSRQ.SelectedDate = DateTime.Now;
            docOPTDATE.SelectedDate = DateTime.Now;
            //docSHRQ.SelectedDate = DateTime.Now;
            docDEPTID.SelectedValue = strDept;
            if (docDEPTID.SelectedValue.Length < 1)
            {
                docDEPTID.SelectedIndex = 1;
            }
            docSTR9.Text = "";//清空条码回收版编号
            billLockDoc(false);
            docMEMO.Enabled = true;
            btnSave.Enabled = true;
            btnAudit.Enabled = false;
            btnPrint.Enabled = false;
            btnDelRow.Enabled = true;
            btnTJ.Enabled = false;
            btnDel.Enabled = false;
            btnDJ.Enabled = false;
            //清空Grid行
            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
        }

        private JObject GetJObject(Dictionary<string, object> dicRecord)
        {
            JObject defaultObj = new JObject();
            foreach (string key in dicRecord.Keys)
            {
                defaultObj.Add(key, dicRecord[key].ToString());
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
            string jingdu = Math.Round(rs * jg, 2).ToString("F2");
            defaultObj.Add("HSJE", jingdu);

            return defaultObj;
        }
        protected void GridGoods_AfterEdit(object sender, FineUIPro.GridAfterEditEventArgs e)
        {
            //不需处理
            string[] strCell = GridGoods.SelectedCell;
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
            if (newDict.Count == 0) return;
            if (e.ColumnID == "BZSL")
            {
                if (Doc.GetGridInf(GridGoods, e.RowID, "BZHL").Length < 1 || Doc.GetGridInf(GridGoods, e.RowID, "BZSL").Length < 1 || Doc.GetGridInf(GridGoods, e.RowID, "HSJJ").Length < 1)
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
                decimal bzslTotal = 0, feeTotal = 0;
                foreach (Dictionary<string, object> dic in newDict)
                {
                    bzslTotal += Convert.ToDecimal(dic["BZSL"]);
                    feeTotal += Convert.ToDecimal(dic["HSJJ"]) * Convert.ToDecimal(dic["BZSL"]);
                }
                JObject summary = new JObject();
                summary.Add("GDNAME", "本页合计");
                summary.Add("BZSL", bzslTotal.ToString());
                summary.Add("HSJE", feeTotal.ToString("F2"));
                GridGoods.SummaryData = summary;
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
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("非『新增』单据不能增行！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
            billLockDoc(true);
            PubFunc.GridRowAdd(GridGoods, "INIT");
        }
        private string billDeleteForPad()
        {
            string result = "";
            MyTable mtType = new MyTable("DAT_XS_DOC");
            //TODO
            foreach (string a in Request.Form)
            {
                if (a != "com[]")
                {
                    if (!mtType.ColRow.ContainsKey(a))
                    {
                        mtType.ColRow.Add(a, Request.Form[a]);
                    }
                    else
                    {
                        mtType.ColRow[a] = Request.Form[a];
                    }
                }
            }

            JObject jo = new JObject();
            if (mtType.ColRow["BILLNO"] == null)
            {
                jo.Add("result", "fail");
                jo.Add("data", "请选择需要删除的单据!");
                return JsonConvert.SerializeObject(jo);
            }
            string billno = mtType.ColRow["BILLNO"].ToString();
            string flag = mtType.ColRow["FLAG"].ToString();
            if (flag != "M" && flag != "R")
            {
                jo.Add("result", "fail");
                jo.Add("data", "非【新单】不能删除!");
                return JsonConvert.SerializeObject(jo);
            }
            List<CommandInfo> cmdList = new List<CommandInfo>();
            cmdList.Add(new CommandInfo("Delete from DAT_XS_DOC t WHERE T.SEQNO ='" + billno + "'", null));
            cmdList.Add(new CommandInfo("Delete from DAT_XS_COM t WHERE T.SEQNO ='" + billno + "'", null));
            try
            {
                if (DbHelperOra.ExecuteSqlTran(cmdList))
                {
                    jo.Add("result", "success");
                    jo.Add("data", billno);
                    result = JsonConvert.SerializeObject(jo);
                }
            }
            catch (Exception ex)
            {
                jo.Add("result", "fail");
                jo.Add("data", ex.Message);
                return JsonConvert.SerializeObject(jo);
            }

            return result;
        }
        protected override void billDel()
        {
            if (docBILLNO.Text.Trim() == "")
            {
                Alert.Show("请选择需要删除的单据");
                return;
            }
            if (docFLAG.SelectedValue != "M" && docFLAG.SelectedValue != "R")
            {
                Alert.Show("非【新单】不能删除!");
                return;
            }
            List<CommandInfo> cmdList = new List<CommandInfo>();
            cmdList.Add(new CommandInfo("Delete from DAT_XS_DOC t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'", null));
            cmdList.Add(new CommandInfo("Delete from DAT_XS_COM t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'", null));
            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                Alert.Show("单据删除成功!");
                OperLog("高值使用", "删除单据【" + docBILLNO.Text + "】");
                billSearch();
                billNew();
            }
            else
            {
                Alert.Show("单据删除失败!", "错误提示", MessageBoxIcon.Information);
            }
        }
        protected override void billDelRow()
        {
            if (docFLAG.SelectedValue != "M" && docFLAG.SelectedValue != "R")
            {
                Alert.Show("非『新增』单据不能删行！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (GridGoods.SelectedRowID == null)
            {
                Alert.Show("请选择数据行删除！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            GridGoods.DeleteSelectedRows();
        }

        protected override void billGoods()
        {
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
            billLockDoc(true);
            //参数说明：cx-查询内容，bm-商品配置部门,su-供应商
            string url = "~/ERPQuery/GoodsWindow_Gather.aspx?bm=" + docDEPTID.SelectedValue + "&cx=&su=";
            PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "商品信息查询"));
        }

        private string billSearchForPad()
        {
            string result = "";
            string billno = Request.QueryString.Get("billno");
            JObject jo = new JObject();
            DataTable dtDoc = DbHelperOra.Query(string.Format(strDocSql, billno)).Tables[0];
            jo.Add("result", "success");
            jo.Add("doc", JsonConvert.SerializeObject(dtDoc));
            DataTable dtBill = DbHelperOra.Query(string.Format(strComSql, billno)).Tables[0];
            jo.Add("com", JsonConvert.SerializeObject(dtBill));
            result = JsonConvert.SerializeObject(jo);
            return result;
        }

        private string listSearchForPad()
        {
            string result = "";
            string strSql = lstSql;
            JObject jo = new JObject();
            string strSearch = "";
            string lrrq1 = Request.QueryString.Get("LRRQ1");
            string lrrq2 = Request.QueryString.Get("LRRQ2");
            string billno = Request.QueryString.Get("BILLNO");
            string deptid = Request.QueryString.Get("DEPTID");
            string doctor = Request.QueryString.Get("DOCTOR");
            string opttable = Request.QueryString.Get("OPTTABLE");
            string optdate1 = Request.QueryString.Get("OPTDTAE1");
            string optdate2 = Request.QueryString.Get("OPTDTAE2");
            if (string.IsNullOrWhiteSpace(lrrq1) || string.IsNullOrWhiteSpace(lrrq2))
            {
                jo.Add("result", "fail");
                jo.Add("data", "请输入条件【使用日期】！");
                return JsonConvert.SerializeObject(jo);
            }
            if (Convert.ToDateTime(lrrq1) > Convert.ToDateTime(lrrq2))
            {
                jo.Add("result", "fail");
                jo.Add("data", "开始日期大于结束日期，请重新输入！");
                return JsonConvert.SerializeObject(jo);
            }
            if (!string.IsNullOrWhiteSpace(billno))
            {
                strSearch += string.Format(" AND  TRIM(UPPER(A.BILLNO))  LIKE '%{0}%'", billno.ToUpper());
            }
            if (!string.IsNullOrWhiteSpace(deptid))
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", deptid);
            }
            if (!string.IsNullOrWhiteSpace(doctor))
            {
                strSearch += string.Format(" AND  A.DOCTOR  LIKE '%{0}%'", doctor.Trim());
            }
            if (!string.IsNullOrWhiteSpace(opttable))
            {
                strSearch += string.Format(" AND  A.OPTTABLE  LIKE '%{0}%'", opttable.Trim());
            }
            if (!string.IsNullOrWhiteSpace(optdate1))
            {
                strSearch += string.Format(" AND A.OPTDATE>=TO_DATE('{0}','YYYY-MM-DD')", optdate1.Trim());
            }
            if (!string.IsNullOrWhiteSpace(optdate2))
            {
                strSearch += string.Format(" AND A.OPTDATE<=TO_DATE('{0}','YYYY-MM-DD')", optdate2.Trim());
            }


            strSearch += string.Format(" AND deptid in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            strSearch += string.Format(" AND A.XSRQ>=TO_DATE('{0}','YYYY-MM-DD')", lrrq1);
            strSearch += string.Format(" AND A.XSRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lrrq2);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY A.BILLNO DESC";

            jo.Add("result", "success");
            jo.Add("data", getSerilizedData(strSql));
            result = JsonConvert.SerializeObject(jo);
            return result;
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

            string strSql = lstSql;
            string strSearch = "";

            if (state())
            {
                strSearch += "AND A.FLAG <> 'M'";
            }
            if (lstBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND  TRIM(UPPER(A.BILLNO))  LIKE '%{0}%'", lstBILLNO.Text.Trim().ToUpper());
            }
            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedItem.Value);
            }
            strSearch += string.Format(" AND deptid in( select code FROM SYS_DEPT where  F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            strSearch += string.Format(" AND A.XSRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.XSRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);
            if (lstDOCTOR.Text.Trim().Length > 0)
            {
                strSearch += string.Format(" AND  A.DOCTOR  LIKE '%{0}%'", lstDOCTOR.Text.Trim());
            }
            if (lstOPTTABLE.Text.Trim().Length > 0)
            {
                strSearch += string.Format(" AND  A.OPTTABLE  LIKE '%{0}%'", lstOPTTABLE.Text.Trim());
            }
            if (lstOPTDATE1.Text.Trim().Length > 0)
            {
                strSearch += string.Format(" AND A.OPTDATE>=TO_DATE('{0}','YYYY-MM-DD')", lstOPTDATE1.Text.Trim());
            }
            if (lstOPTDATE2.Text.Trim().Length > 0)
            {
                strSearch += string.Format(" AND A.OPTDATE<=TO_DATE('{0}','YYYY-MM-DD')", lstOPTDATE2.Text.Trim());
            }

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY A.BILLNO DESC";
            GridList.DataSource = DbHelperOra.Query(strSql).Tables[0];
            GridList.DataBind();
        }

        /// <summary>
        /// 返回序列化了的DataTable
        /// </summary>
        /// <returns></returns>
        private string getSerilizedData(string sql)
        {
            return JsonConvert.SerializeObject(DbHelperOra.Query(sql).Tables[0]);
        }

        protected override void billAudit()
        {
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("非新单不能审核！");
                return;
            }
            //验证是否盘点
            if (DbHelperOra.Exists("SELECT 1 FROM DAT_PD_LOCK WHERE DEPTID = '" + docDEPTID.SelectedValue + "' AND FLAG='N'"))
            {
                Alert.Show(string.Format("部门【{0}】正在盘点,请盘点后进行操作！", docDEPTID.SelectedText), "警告提示", MessageBoxIcon.Warning);
                return;
            }
            
            string strBillno = docSEQNO.Text;
            if (BillOper(strBillno, "PASS2") == 1)
            {
                billLockDoc(true);
                Alert.Show("单据【" + strBillno + "】审核成功！");
                OperLog("高值使用", "审核单据【" + strBillno + "】");
                billOpen(strBillno);
            }
        }
        protected void btnTjAll_Click(object sender, EventArgs e)
        {
            //结算登记
            int[] selectAll = GridList.SelectedRowIndexArray;
            if (selectAll.Count() < 1)
            {
                Alert.Show("请选择需要登记的单据！", MessageBoxIcon.Warning);
                return;
            }
            String billno = "";
            foreach (int select in selectAll)
            {
                if (GridList.DataKeys[select][1].ToString() == "Y")
                {
                    billno += "'" + GridList.DataKeys[select][0] + "',";
                }
            }
            if (billno.Length < 1)
            {
                Alert.Show("选择登记的单据状态不正确！", MessageBoxIcon.Warning);
                return;
            }
            String Sql = "UPDATE DAT_XS_DOC SET FLAG = 'J' WHERE SEQNO in ({0}) AND FLAG = 'Y'";
            DbHelperOra.ExecuteSql(String.Format(Sql, billno.TrimEnd(',')));
            Alert.Show("单据【" + billno.TrimEnd(',') + "】登记成功！");
            billSearch();
        }
        protected void btnDJ_Click(object sender, EventArgs e)
        {
            if (docSEQNO.Text.Length < 1)
            {
                Alert.Show("请选择需要登记的单据信息！", MessageBoxIcon.Warning);
                return;
            }
            if (!DbHelperOra.Exists("SELECT 1 FROM DAT_XS_DOC WHERE FLAG ='Y' AND SEQNO = '" + docSEQNO.Text + "'"))
            {
                Alert.Show("单据【" + docSEQNO.Text + "】不存在或者状态不正确，请检查！", MessageBoxIcon.Warning);
                billOpen(docSEQNO.Text);
                return;
            }
            String Sql = "UPDATE DAT_XS_DOC SET FLAG = 'J' WHERE SEQNO = '{0}' AND FLAG = 'Y'";
            DbHelperOra.ExecuteSql(String.Format(Sql, docSEQNO.Text));
            Alert.Show("单据【" + docSEQNO.Text + "】登记成功！");
            billOpen(docSEQNO.Text);
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
            if (dtBill != null && dtBill.Rows.Count > 0)
            {
                Doc.GridRowAdd(GridGoods, dtBill);
                foreach (DataRow row in dtBill.Rows)
                {
                    bzslTotal += Convert.ToDecimal(row["BZSL"]);
                    feeTotal += Convert.ToDecimal(row["HSJJ"]) * Convert.ToDecimal(row["BZSL"]);
                }
            }
            //计算合计数量
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("BZSL", bzslTotal.ToString());
            summary.Add("HSJE", feeTotal.ToString("F2"));


            GridGoods.SummaryData = summary;
            PubFunc.FormLock(FormDoc, true, "");
            //if (string.IsNullOrWhiteSpace(docOPTID.Text) || string.IsNullOrWhiteSpace(docDOCTOR.Text) || string.IsNullOrWhiteSpace(docOPTTABLE.Text) || string.IsNullOrWhiteSpace(docCUSTID.Text) || string.IsNullOrWhiteSpace(docSTR2.Text))
            //{

            //}
            TabStrip1.ActiveTabIndex = 1;
            if (docFLAG.SelectedValue == "M" || docFLAG.SelectedValue == "R")
            {
                docOPTID.Enabled = true;
                docDOCTOR.Enabled = true;
                docOPTTABLE.Enabled = true;
                docCUSTID.Enabled = true;
                docSTR2.Enabled = true;
                docSTR7.Enabled = true;
                docSTR8.Enabled = true;
                docSTR9.Enabled = true;
                docOPTDATE.Enabled = true;
                docMEMO.Enabled = true;
            }
            else
            {
                docOPTID.Enabled = false;
                docDOCTOR.Enabled = false;
                docOPTTABLE.Enabled = false;
                docCUSTID.Enabled = false;
                docSTR2.Enabled = false;
                docSTR7.Enabled = false;
                docSTR8.Enabled = false;
                docSTR9.Enabled = false;
                docOPTDATE.Enabled = false;
                docMEMO.Enabled = false;
            }

            if (docFLAG.SelectedValue == "M" || docFLAG.SelectedValue == "R")
            {
                btnSave.Enabled = true;
                btnAudit.Enabled = true;
                btnPrint.Enabled = false;
                btnDelRow.Enabled = true;
                btnTJ.Enabled = true;
                btnDel.Enabled = true;
                trbBARCODE.Enabled = true;
                btnDJ.Enabled = false;

            }
            else if (docFLAG.SelectedValue == "N")
            {
                btnSave.Enabled = false;
                btnAudit.Enabled = true;
                btnPrint.Enabled = false;
                btnDelRow.Enabled = false;
                btnTJ.Enabled = false;
                btnCancel.Enabled = true;
                btnDel.Enabled = false;
                trbBARCODE.Enabled = false;
                btnDJ.Enabled = false;
            }
            else if (docFLAG.SelectedValue == "J" || docFLAG.SelectedValue == "D")
            {
                btnSave.Enabled = false;
                btnAudit.Enabled = false;
                btnPrint.Enabled = true;
                btnDelRow.Enabled = false;
                btnTJ.Enabled = false;
                btnCancel.Enabled = false;
                btnDel.Enabled = false;
                trbBARCODE.Enabled = false;
                btnDJ.Enabled = false;
            }
            else
            {
                btnSave.Enabled = false;
                btnAudit.Enabled = false;
                btnPrint.Enabled = true;
                btnDelRow.Enabled = false;
                btnTJ.Enabled = false;
                btnCancel.Enabled = false;
                btnDel.Enabled = false;
                trbBARCODE.Enabled = false;
                btnDJ.Enabled = true; ;
            }
        }
        protected override void billCancel()
        {
            if (PubFunc.StrIsEmpty(docSEQNO.Text))
            {
                Alert.Show("请选择需要操作的单据！", "操作提示", MessageBoxIcon.Warning);
                return;
            }
            if (docFLAG.SelectedValue == "N")
            {
                if (DbHelperOra.ExecuteSql(string.Format("UPDATE DAT_XS_DOC SET FLAG = 'R' WHERE SEQNO = '{0}' AND FLAG = 'N'", docSEQNO.Text)) > 0)
                {
                    Alert.Show("单据【" + docSEQNO.Text + "】驳回成功！");
                    OperLog("高值使用", "驳回单据【" + docSEQNO.Text + "】");
                    billOpen(docSEQNO.Text);
                    return;
                }
                else
                {
                    Alert.Show("请刷新界面后重试！", "操作提示", MessageBoxIcon.Warning);
                }
            }
            else
            {
                Alert.Show("单据状态不正确，请检查！", "操作提示", MessageBoxIcon.Warning);
                return;
            }
        }

        private string billSaveForPad()
        {
            string result = "";
            JObject jo = new JObject();
            String[] comData = Request.Form["com[]"].Split(',');
            MyTable mtType = new MyTable("DAT_XS_DOC");
            //TODO
            foreach (string a in Request.Form)
            {
                if (a != "com[]")
                {
                    if (!mtType.ColRow.ContainsKey(a))
                    {
                        mtType.ColRow.Add(a, Request.Form[a]);
                    }
                    else
                    {
                        mtType.ColRow[a] = Request.Form[a];
                    }


                }
            }



            //Dictionary<string, object> newDict = new Dictionary<string, object>();
            //foreach (string a in Request.Form) {
            //    if (a != "com[]") {
            //        newDict.Add(a, Request.Form[a]);
            //    }
            //}

            //string deptid = Request.Form["deptid"];
            //string opttable = Request.Form["opttable"];
            //string custid = Request.Form["custid"];
            //string optid = Request.Form["optid"];
            //string memo = Request.Form["memo"];
            //string xsrq = Request.Form["xsrq"];
            //string optdate = Request.Form["optdate"];
            //string flag = Request.Form["flag"];
            //string billno = Request.Form["billno"];
            if (mtType.ColRow["FLAG"].ToString() != "M" && mtType.ColRow["FLAG"].ToString() != "R")
            {
                jo.Add("result", "fail");
                jo.Add("data", "非[新单]不能保存！");
                return JsonConvert.SerializeObject(jo);
            }
            if (comData.Length == 0)
            {
                jo.Add("result", "fail");
                jo.Add("data", "请输入商品信息！");
                return JsonConvert.SerializeObject(jo);
            }
            List<Dictionary<string, object>> goodsData = new List<Dictionary<string, object>>();
            try
            {
                foreach (string oc in comData)
                {
                    string onecode = oc.Trim();
                    string gdseq = Doc.ONECODE_GZ(onecode, "GDSEQ");
                    DataTable dt_goods = new DataTable();
                    dt_goods = Doc.GetGoods_Gather(gdseq, "", mtType.ColRow["DEPTID"].ToString());
                    string ph = Doc.ONECODE_GZ(onecode, "PH");
                    DataRow dr_goods = dt_goods.Rows[0];
                    string gdname = dr_goods["GDNAME"].ToString();
                    if (!string.IsNullOrWhiteSpace(gdseq) && !string.IsNullOrWhiteSpace(gdname))
                    {
                        if (string.IsNullOrWhiteSpace(ph) || ph == "\\")
                        {
                            jo.Add("result", "fail");
                            jo.Add("data", "商品【" + gdname + "】批号不能为空！");
                            return JsonConvert.SerializeObject(jo);
                        }
                        else
                        {
                            Dictionary<string, object> oneGoodsData = new Dictionary<string, object>();
                            foreach (DataColumn dc in dt_goods.Columns)
                            {
                                oneGoodsData.Add(dc.ColumnName, dr_goods[dc.ColumnName]);
                            }
                            oneGoodsData["BZSL"] = 1;
                            oneGoodsData["DHSL"] = 1;
                            oneGoodsData["ONECODE"] = onecode;
                            oneGoodsData["PH"] = ph;
                            oneGoodsData["RQ_SC"] = Doc.ONECODE_GZ(onecode, "RQ_SC");
                            oneGoodsData["YXQZ"] = Doc.ONECODE_GZ(onecode, "YXQZ");
                            //oneGoodsData.Add("GDSEQ", gdseq);
                            //oneGoodsData.Add("GDNAME", gdname);
                            //oneGoodsData.Add("ONECODE", onecode);
                            //todo
                            goodsData.Add(oneGoodsData);
                        }
                    }
                }
                if (goodsData.Count == 0)
                {
                    jo.Add("result", "fail");
                    jo.Add("data", "商品信息不能为空！");
                    return JsonConvert.SerializeObject(jo);
                }

                if (string.IsNullOrWhiteSpace(mtType.ColRow["BILLNO"].ToString()))
                {
                    mtType.ColRow["BILLNO"] = BillSeqGet();
                }
                else
                {
                    string flg = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M') FROM DAT_XS_DOC WHERE SEQNO='{0}'", mtType.ColRow["BILLNO"].ToString()));
                    if (!string.IsNullOrWhiteSpace(flg) && (",M,R").IndexOf(flg) < 0)
                    {
                        jo.Add("result", "fail");
                        jo.Add("data", "非新增、驳回状态不能保存！");
                        return JsonConvert.SerializeObject(jo);
                    }
                }

                //mtType.ColRow = PubFunc.FormDataHT(FormDoc);
                mtType.ColRow["SEQNO"] = mtType.ColRow["BILLNO"];
                mtType.ColRow["BILLTYPE"] = BillType;
                mtType.ColRow["SUBNUM"] = goodsData.Count;
                mtType.ColRow["XSTYPE"] = "1";
                mtType.ColRow["FLAG"] = "M";
                List<CommandInfo> cmdList = new List<CommandInfo>();
                MyTable mtTypeMx = new MyTable("DAT_XS_COM");
                MyTable mtTypeExt = new MyTable("DAT_XS_EXT");

                //先删除单据信息在插入
                cmdList.Add(new CommandInfo("delete DAT_XS_DOC where seqno='" + mtType.ColRow["BILLNO"].ToString() + "'", null));//删除单据台头
                cmdList.Add(new CommandInfo("delete DAT_XS_COM where seqno='" + mtType.ColRow["BILLNO"].ToString() + "'", null));//删除单据明细
                cmdList.AddRange(mtType.InsertCommand());
                for (int i = 0; i < goodsData.Count; i++)
                {
                    //todo
                    mtTypeMx.ColRow = PubFunc.GridDataGet(goodsData[i]);

                    if (PubFunc.StrIsEmpty(mtTypeMx.ColRow["BZSL"].ToString()) || mtTypeMx.ColRow["BZSL"].ToString() == "0")
                    {
                        jo.Add("result", "fail");
                        jo.Add("data", "商品【" + mtTypeMx.ColRow["GDSEQ"] + " | " + mtTypeMx.ColRow["GDNAME"] + "】【使用数】为0或空，无法进行【高值商品使用】操作。");
                        return JsonConvert.SerializeObject(jo);

                    }

                    mtTypeMx.ColRow.Add("SEQNO", mtType.ColRow["BILLNO"].ToString());
                    mtTypeMx.ColRow["ROWNO"] = i + 1;
                    mtTypeMx.ColRow.Add("PHID", mtTypeMx.ColRow["PH"]);
                    mtTypeMx.ColRow["DHSL"] = decimal.Parse(mtTypeMx.ColRow["BZHL"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                    mtTypeMx.ColRow["HSJE"] = decimal.Parse(mtTypeMx.ColRow["HSJJ"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZHL"].ToString());

                    mtTypeMx.ColRow.Add("XSSL", goodsData[i]["DHSL"].ToString());
                    mtTypeMx.ColRow["BHSJJ"] = 0;
                    mtTypeMx.ColRow["BHSJE"] = 0;

                    mtTypeMx.ColRow.Remove("UNITNAME");
                    mtTypeMx.ColRow.Remove("PRODUCERNAME");
                    cmdList.Add(mtTypeMx.Insert());
                }
                if (DbHelperOra.ExecuteSqlTran(cmdList))
                {
                    //Alert.Show("商品使用信息保存成功！");
                    //OperLog("高值使用", "修改单据【" + docBILLNO.Text + "】");
                    //billNew();
                    //billLockDoc(true);
                    //billOpen(docSEQNO.Text);
                    jo.Add("result", "success");
                    jo.Add("data", mtType.ColRow["BILLNO"].ToString());
                    result = JsonConvert.SerializeObject(jo);
                }

            }
            catch (Exception ex)
            {
                jo.Add("result", "fail");
                jo.Add("data", ex.Message);
                return JsonConvert.SerializeObject(jo);
            }


            return result;
        }

        protected override void billSave()
        {
            saveEx();
        }

        private void saveEx(string flag="N")
        {
            #region 数据有效性验证
            if (docFLAG.SelectedValue != "M" && docFLAG.SelectedValue != "R")
            {
                Alert.Show("非[新单]不能保存！", "消息提示", MessageBoxIcon.Warning);
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
                    if (string.IsNullOrWhiteSpace(newDict[i]["PH"].ToString()) || newDict[i]["PH"].ToString() == "\\")
                    {
                        //GridGoods.SelectedCell = new int[] { i, 8 };
                        string[] selectedCell = GridGoods.SelectedCell;
                        PageContext.RegisterStartupScript(String.Format("F('{0}').selectCell('{1}','{2}');", GridGoods.ClientID, selectedCell[0], "KCSL"));
                        Alert.Show("商品【" + newDict[i]["GDNAME"].ToString() + "】批号不能为空！！！", "消息提示", MessageBoxIcon.Warning);

                    }
                    if (!string.IsNullOrWhiteSpace(newDict[i]["ONECODE"].ToString()))
                    {
                        for (int k = 1 + i; k < newDict.Count; k++)
                        {
                            if ((newDict[i]["ONECODE"].ToString()) == (newDict[k]["ONECODE"].ToString()))
                            {
                                Alert.Show("商品『" + newDict[k]["GDNAME"].ToString() + "』条码『" + newDict[k]["ONECODE"].ToString() + "』重复，请维护！", "消息提示", MessageBoxIcon.Warning);
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
            if (DbHelperOra.Exists("SELECT 1 FROM dat_xs_doc where seqno = '" + docBILLNO.Text + "'") && docBILLNO.Enabled)
            {
                Alert.Show("您输入的单号已存在,请检查!");
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
                string flg = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M') FROM DAT_XS_DOC WHERE SEQNO='{0}'", docBILLNO.Text));
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
            MyTable mtType = new MyTable("DAT_XS_DOC");
            mtType.ColRow = PubFunc.FormDataHT(FormDoc);
            mtType.ColRow["SEQNO"] = docBILLNO.Text;
            mtType.ColRow.Add("BILLTYPE", BillType);
            mtType.ColRow.Add("SUBNUM", goodsData.Count);
            mtType.ColRow.Add("SUBSUM", "0");
            mtType.ColRow.Add("XSTYPE", "1");
            mtType.ColRow["FLAG"] = "M";
            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtTypeMx = new MyTable("DAT_XS_COM");
            MyTable mtTypeExt = new MyTable("DAT_XS_EXT");
            Decimal sum = 0;
            //先删除单据信息在插入
            cmdList.Add(new CommandInfo("delete DAT_XS_DOC where seqno='" + docBILLNO.Text + "'", null));//删除单据台头
            cmdList.Add(new CommandInfo("delete DAT_XS_COM where seqno='" + docBILLNO.Text + "'", null));//删除单据明细
            //cmdList.Add(new CommandInfo("delete DAT_XS_EXT where  BILLNO='" + docBILLNO.Text + "'", null));//删除单据台头
            for (int i = 0; i < goodsData.Count; i++)
            {
                mtTypeMx.ColRow = PubFunc.GridDataGet(goodsData[i]);
                if (PubFunc.StrIsEmpty(mtTypeMx.ColRow["BZSL"].ToString()) || mtTypeMx.ColRow["BZSL"].ToString() == "0")
                {
                    Alert.Show("商品【" + mtTypeMx.ColRow["GDSEQ"] + " | " + mtTypeMx.ColRow["GDNAME"] + "】【使用数】为0或空，无法进行【高值商品使用】操作。");
                    return;
                }
                mtTypeMx.ColRow.Add("SEQNO", docBILLNO.Text);
                mtTypeMx.ColRow["ROWNO"] = i + 1;
                mtTypeMx.ColRow.Add("PHID", mtTypeMx.ColRow["PH"]);
                mtTypeMx.ColRow["DHSL"] = decimal.Parse(mtTypeMx.ColRow["BZHL"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                mtTypeMx.ColRow["HSJE"] = decimal.Parse(mtTypeMx.ColRow["HSJJ"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZHL"].ToString());
                sum = sum + decimal.Parse(mtTypeMx.ColRow["HSJE"].ToString());
                mtTypeMx.ColRow.Add("XSSL", goodsData[i]["DHSL"].ToString());
                mtTypeMx.ColRow.Add("BHSJJ", 0);
                mtTypeMx.ColRow.Add("BHSJE", 0);

                mtTypeMx.ColRow.Remove("UNITNAME");
                mtTypeMx.ColRow.Remove("PRODUCERNAME");
                cmdList.Add(mtTypeMx.Insert());

                //mtTypeExt.ColRow["DEPTID"] = docDEPTID.SelectedValue;
                //mtTypeExt.ColRow["BILLNO"] = docBILLNO.Text;
                //mtTypeExt.ColRow["ROWNO"] = i + 1;
                //mtTypeExt.ColRow["ONECODE"] = goodsData[i]["ONECODE"].ToString();
                //mtTypeExt.ColRow["GDSEQ"] = goodsData[i]["GDSEQ"].ToString();
                //mtTypeExt.ColRow["GDNAME"] = goodsData[i]["GDNAME"].ToString();
                //// mtTypeExt.ColRow.Add("BARCODE", goodsData[i]["GDNAME"].ToString());
                //mtTypeExt.ColRow["UNIT"] = goodsData[i]["UNIT"];
                //mtTypeExt.ColRow["GDSPEC"] = goodsData[i]["GDSPEC"];
                //mtTypeExt.ColRow["PH"] = goodsData[i]["PH"];
                //if (goodsData[i]["RQ_SC"] != null && !string.IsNullOrWhiteSpace(goodsData[i]["RQ_SC"].ToString()))
                //{
                //    mtTypeExt.ColRow["RQ_SC"] = DateTime.Parse(goodsData[i]["RQ_SC"].ToString()).ToString("yyyy-MM-dd");
                //}
                //if (goodsData[i]["YXQZ"] != null && !string.IsNullOrWhiteSpace(goodsData[i]["YXQZ"].ToString()))
                //{
                //    mtTypeExt.ColRow["YXQZ"] = DateTime.Parse(goodsData[i]["YXQZ"].ToString()).ToString("yyyy-MM-dd");
                //}
                //mtTypeExt.ColRow["DEPTCUR"] = docDEPTID.SelectedValue;
                //mtTypeExt.ColRow["FLAG"] = "N";
                //mtTypeExt.ColRow["OPERUSER"] = UserAction.UserID;
                //mtTypeExt.ColRow["OPERDATE"] = DateTime.Now.ToString();
                //mtTypeExt.ColRow["PATIENT"] = goodsData[i]["CUSTID"];
                //mtTypeExt.ColRow["OPTDATE"] = DateTime.Now.ToString();
                //mtTypeExt.ColRow["OPTID"] = docOPTID.Text.Trim();
                //mtTypeExt.ColRow["OPTDOCTOR"] = docDOCTOR.Text.Trim();
                //mtTypeExt.ColRow["INSTIME"] = DateTime.Now.ToString();
                //mtTypeExt.ColRow["BZHL"] = goodsData[i]["BZHL"];
                //cmdList.Add(mtTypeExt.Insert());
            }
            mtType.ColRow["SUBSUM"] = sum;
            cmdList.AddRange(mtType.InsertCommand());
            //商品使用信息在保存之后即进行审核操作
            //OracleParameter[] parameters = {
            //                                   new OracleParameter("VTASKID", OracleDbType.Varchar2,20),
            //                                   new OracleParameter("VPARA", OracleDbType.Varchar2,800) };
            //parameters[0].Value = BillType;
            //parameters[1].Value = "'" + docBILLNO.Text + "','" + BillType + "','" + UserAction.UserID + "','AUDIT'";
            //cmdList.Add(new CommandInfo("P_EXECTASK", parameters, CommandType.StoredProcedure));

            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                if(flag == "N")
                    Alert.Show("商品使用信息保存成功！");
                OperLog("高值使用", "修改单据【" + docBILLNO.Text + "】");
                //billNew();
                //billLockDoc(true);
                billOpen(docSEQNO.Text);
                if (!string.IsNullOrWhiteSpace(trbBARCODE.Text))
                {
                    trbBARCODE.Text = "";
                }
            }
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
            //dt.Columns.Remove(dt.Columns["BZHL"]);
            if (dt != null && dt.Rows.Count > 0)
            {
                dt.Columns["PIZNO"].ColumnName = "PZWH";
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
                string msg = "";
                string msg1 = "";
                foreach (DataRow row in dt.Rows)
                {
                    row["BZSL"] = "1";
                    row["DHSL"] = "1";
                    row["KCSL"] = "0";
                    row["HSJE"] = Convert.ToDecimal(row["BZSL"]) * Convert.ToDecimal(row["BZHL"]) * Convert.ToDecimal(row["HSJJ"]);
                    ////越库商品不进行库存、批号带入
                    //if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DOC_GOODS WHERE GDSEQ = '{0}' AND ISFLAG3 = 'N'", row["GDSEQ"].ToString())))
                    //{
                    DataTable dtPH = Doc.GetGoodsPHKC(row["GDSEQ"].ToString(), docDEPTID.SelectedValue);
                    if (dtPH != null && dtPH.Rows.Count > 0)
                    {
                        if (dtPH.Rows[0]["KCSL"].ToString() == "0")
                        {
                            msg += row["GDNAME"] + ",";
                            continue;
                        }
                        row["PH"] = dtPH.Rows[0]["PH"];
                        row["PZWH"] = dtPH.Rows[0]["PZWH"];
                        row["RQ_SC"] = dtPH.Rows[0]["RQ_SC"];
                        row["YXQZ"] = dtPH.Rows[0]["YXQZ"];
                        row["KCSL"] = dtPH.Rows[0]["KCSL"];
                        //row["STR2"] = (dtPH.Rows[0]["ISFLAG3"] ?? "");
                    }
                    else
                    {
                        msg += row["GDNAME"] + ",";
                        continue;
                    }
                    //}
                    //else
                    //{
                    //    row["STR2"] = "Y";
                    //}
                    if (string.IsNullOrWhiteSpace(row["HSJJ"].ToString()) || row["HSJJ"].ToString() == "0")
                    {
                        msg1 += "【" + row["GDSEQ"].ToString() + " | " + row["GDNAME"].ToString() + "】,";
                        Alert.Show("商品" + msg1 + "【含税进价】为空，不能进行【高值商品使用】操作。", "消息提示", MessageBoxIcon.Warning);
                        continue;
                    }
                    row["HSJJ"] = Math.Round(Convert.ToDecimal(row["HSJJ"]) * Convert.ToDecimal(row["BZHL"]), 4);
                    LoadGridRow(row, false);
                }
                if (!string.IsNullOrWhiteSpace(msg))
                {
                    Alert.Show(string.Format("商品【{0}】在部门『{1}』中没有库存,不能进行录入！", msg, docDEPTID.SelectedText), "消息提示", MessageBoxIcon.Warning);
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
                    newDict["PH"] = row.Values[3];
                    newDict["YXQZ"] = row.Values[4];
                    newDict["PZWH"] = row.Values[6];
                    newDict["RQ_SC"] = row.Values[5];
                    newDict["KCSL"] = row.Values[7];
                    newDict["BZSL"] = tbxNumber.Text;
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


        private string scanForPad()
        {
            string dept = Request.QueryString.Get("deptid");
            string onecode = Request.QueryString.Get("onecode");
            string result = "";
            JObject jo = new JObject();
            DataTable dt = new DataTable();
            string DEPT = (DbHelperOra.GetSingle(String.Format("SELECT DEPTCUR FROM DAT_GZ_EXT WHERE (ONECODE = '{0}' OR STR1 = '{0}') AND FLAG = 'C'", onecode.Trim())) ?? "").ToString();
            if (string.IsNullOrWhiteSpace(DEPT))
            {
                jo.Add("result", "fail");
                jo.Add("data", "您输入的追溯码未出库或已被使用,请检查！");
                return JsonConvert.SerializeObject(jo);
            }
            if (string.IsNullOrWhiteSpace(dept))
            {
                dept = DEPT;
            }
            //TODO 检查已扫描唯一码

            string gdseq = Doc.ONECODE_GZ(onecode, "GDSEQ");
            if (!string.IsNullOrWhiteSpace(gdseq) && gdseq.Trim().Length >= 2)
            {
                DataTable dt_goods = new DataTable();
                dt_goods = Doc.GetGoods_Gather(gdseq, "", dept);
                if (dt_goods != null && dt_goods.Rows.Count > 0)
                {
                    dt_goods.Columns.Add("ONECODE", Type.GetType("System.String"));
                    dt_goods.Columns.Add("BZSL", Type.GetType("System.Int32"));
                    dt_goods.Columns.Add("DHSL", Type.GetType("System.Int32"));

                    dt_goods.Columns.Add("KCSL", Type.GetType("System.Int32"));
                    dt_goods.Columns.Add("HSJE", Type.GetType("System.Double"));
                    DataRow dr_goods = dt_goods.Rows[0];
                    dr_goods["ONECODE"] = onecode.Trim();
                    dr_goods["DHSL"] = Doc.ONECODE_GZ(onecode, "BZHL");
                    dr_goods["BZSL"] = dr_goods["DHSL"];
                    dr_goods["KCSL"] = "0";
                    dr_goods["HSJE"] = Convert.ToDecimal(dr_goods["HSJJ"]) * Convert.ToDecimal(dr_goods["BZSL"]) * Convert.ToDecimal(dr_goods["BZHL"]);

                    //直接从唯一码跟踪明细表 去批号和库存数量
                    dr_goods["PH"] = Doc.ONECODE_GZ(onecode.Trim(), "PH");
                    dr_goods["YXQZ"] = Doc.ONECODE_GZ(onecode.Trim(), "YXQZ");
                    dr_goods["RQ_SC"] = Doc.ONECODE_GZ(onecode.Trim(), "RQ_SC");
                    dt = dt_goods.Copy();
                    //LoadGridRow(dr_goods, false, "DECLARE");
                }
                else
                {
                    jo.Add("result", "fail");
                    jo.Add("data", "科室尚未配置商品【" + gdseq + "】");
                    return JsonConvert.SerializeObject(jo);

                }
            }
            jo.Add("result", "success");
            jo.Add("data", JsonConvert.SerializeObject(dt));
            result = JsonConvert.SerializeObject(jo);
            return result;
        }
        protected void trbBARCODE_TriggerClick(object sender, EventArgs e)
        {
            //  因本位码隐藏此判断
            //if (trbBARCODE.Text.Trim().Length < Doc.LENCODE()) return;
            //if (trbBARCODE.Text.Substring(0, 1) != "2")
            //{
            //    Alert.Show("您扫描或输入的高值码信息非高值商品,请检查！", "提示信息", MessageBoxIcon.Warning);
            //    trbBARCODE.Text = string.Empty;
            //    trbBARCODE.Focus();
            //    return;
            //}
            if (string.IsNullOrWhiteSpace(docSTR9.Text)&& !string.IsNullOrWhiteSpace(trbBARCODE.Text))
            {
                if ((",JG,HG").IndexOf(trbBARCODE.Text.Substring(0, 2).ToUpper()) > 0 || trbBARCODE.Text.Length == 10)
                {
                    docSTR9.Text = trbBARCODE.Text.ToUpper();
                    trbBARCODE.Text = string.Empty;
                    trbBARCODE.Focus();
                    return;
                }
            }

            //验证唯一码是否可用
            string DEPT = (DbHelperOra.GetSingle(String.Format("SELECT DEPTCUR FROM DAT_GZ_EXT WHERE (ONECODE = '{0}' OR STR1 = '{0}') AND FLAG = 'C'", trbBARCODE.Text.Trim())) ?? "").ToString();
            if (PubFunc.StrIsEmpty(DEPT))
            {
                Alert.Show("您输入的追溯码未出库或已被使用,请检查！", "提示信息", MessageBoxIcon.Warning);
                trbBARCODE.Text = string.Empty;
                trbBARCODE.Focus();
                return;
            }
            //else if (docDEPTID.SelectedValue != DEPT&& docDEPTID.SelectedValue!="0")
            //{
            //    Alert.Show("您扫描的条码【" + trbBARCODE.Text.Trim() + "】不属于科室【" + docDEPTID.SelectedText + "】，请确认！", "操作提示", MessageBoxIcon.Warning);
            //    //trbBARCODE.Text = string.Empty;
            //    //trbBARCODE.Focus();
            //    return;
            //}
            //else
            //{
            //    docDEPTID.SelectedValue = DEPT;
            //}

            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().ToList();
            for (int i = 0; i < newDict.Count; i++)
            {
                string barcode_old = newDict[i]["ONECODE"].ToString();
                if (barcode_old == trbBARCODE.Text.Trim())
                {
                    Alert.Show("扫描条码已扫描!", "提示信息", MessageBoxIcon.Warning);
                    trbBARCODE.Text = "";
                    trbBARCODE.Focus();
                    return;
                }
                //增加高值条码是否属于同一科室的判断
                string deptcur_old = Doc.ONECODE_GZ(barcode_old, "DEPTCUR");
                if (deptcur_old != DEPT)
                {
                    Alert.Show("您扫描的条码【" + trbBARCODE.Text.Trim() + "】不属于科室【" + docDEPTID.SelectedText + "】，请确认！", "操作提示", MessageBoxIcon.Warning);
                    return;
                }

            }

            docDEPTID.SelectedValue = DEPT;
            //增加表体信息
            docDEPTID.Enabled = false;
            string gdseq = Doc.ONECODE_GZ(trbBARCODE.Text.Trim(), "GDSEQ");
            if (!string.IsNullOrWhiteSpace(gdseq) && gdseq.Trim().Length >= 2)
            {
                DataTable dt_goods = new DataTable();
                dt_goods = Doc.GetGoods_Gather(gdseq, "", docDEPTID.SelectedValue);
                if (dt_goods != null && dt_goods.Rows.Count > 0)
                {
                    dt_goods.Columns.Add("ONECODE", Type.GetType("System.String"));
                    dt_goods.Columns.Add("BZSL", Type.GetType("System.Int32"));
                    dt_goods.Columns.Add("DHSL", Type.GetType("System.Int32"));

                    dt_goods.Columns.Add("KCSL", Type.GetType("System.Int32"));
                    dt_goods.Columns.Add("HSJE", Type.GetType("System.Double"));
                    DataRow dr_goods = dt_goods.Rows[0];
                    dr_goods["ONECODE"] = trbBARCODE.Text.Trim();
                    dr_goods["DHSL"] = Doc.ONECODE_GZ(trbBARCODE.Text.Trim(), "BZHL");
                    dr_goods["BZSL"] = dr_goods["DHSL"];
                    dr_goods["KCSL"] = "0";
                    dr_goods["HSJE"] = Convert.ToDecimal(dr_goods["HSJJ"]) * Convert.ToDecimal(dr_goods["BZSL"]) * Convert.ToDecimal(dr_goods["BZHL"]);

                    //直接从唯一码跟踪明细表 去批号和库存数量
                    dr_goods["PH"] = Doc.ONECODE_GZ(trbBARCODE.Text.Trim(), "PH");
                    dr_goods["YXQZ"] = Doc.ONECODE_GZ(trbBARCODE.Text.Trim(), "YXQZ");
                    dr_goods["RQ_SC"] = Doc.ONECODE_GZ(trbBARCODE.Text.Trim(), "RQ_SC");
                    LoadGridRow(dr_goods, false, "DECLARE");
                }
                else
                {
                    Alert.Show(string.Format("{0}尚未配置商品【{1}】！！！", docDEPTID.SelectedText, gdseq), MessageBoxIcon.Warning);
                    trbBARCODE.Text = string.Empty;
                    trbBARCODE.Focus();
                    return;
                }
            }
            trbBARCODE.Text = string.Empty;
            trbBARCODE.Focus();
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            WindowLot.Hidden = true;
        }

        protected void GridLot_RowDoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            DataTable dt_goods = new DataTable();
            dt_goods = Doc.GetGoods_Gather(GridLot.DataKeys[e.RowIndex][4].ToString(), "", docDEPTID.SelectedValue);
            if (dt_goods.Rows.Count <= 0)
            {
                Alert.Show("查询数据更新失败!", "提示信息", MessageBoxIcon.Warning);
                return;
            }

            dt_goods.Columns.Add("ONECODE", Type.GetType("System.String"));
            dt_goods.Columns.Add("CUSTID", Type.GetType("System.String"));
            dt_goods.Columns.Add("BZSL", Type.GetType("System.Int32"));
            dt_goods.Columns.Add("DHSL", Type.GetType("System.Int32"));
            dt_goods.Columns.Add("KCSL", Type.GetType("System.Int32"));
            dt_goods.Columns.Add("HSJE", Type.GetType("System.Double"));

            dt_goods.Rows[0]["ONECODE"] = saveOnecode.Text;
            dt_goods.Rows[0]["CUSTID"] = savecustid.Text;
            dt_goods.Rows[0]["DHSL"] = DbHelperOra.GetSingle(string.Format("SELECT BZHL FROM DAT_DD_EXT WHERE ONECODE = '{0}'", saveOnecode.Text)).ToString();
            dt_goods.Rows[0]["BZSL"] = dt_goods.Rows[0]["DHSL"];
            dt_goods.Rows[0]["KCSL"] = (GridLot.DataKeys[e.RowIndex][3].ToString());
            if (GridLot.DataKeys[e.RowIndex][0] == null || string.IsNullOrEmpty(GridLot.DataKeys[e.RowIndex][0].ToString()))
            {
                dt_goods.Rows[0]["PH"] = "";
            }
            else
            {
                dt_goods.Rows[0]["PH"] = (GridLot.DataKeys[e.RowIndex][0].ToString());
            }
            if (GridLot.DataKeys[e.RowIndex][1] == null || string.IsNullOrEmpty(GridLot.DataKeys[e.RowIndex][1].ToString()))
            {
                dt_goods.Rows[0]["YXQZ"] = "";
            }
            else
            {
                dt_goods.Rows[0]["YXQZ"] = (GridLot.DataKeys[e.RowIndex][1].ToString());
            }
            if (GridLot.DataKeys[e.RowIndex][2] == null || string.IsNullOrEmpty(GridLot.DataKeys[e.RowIndex][2].ToString()))
            {
                dt_goods.Rows[0]["RQ_SC"] = "";
            }
            else
            {
                dt_goods.Rows[0]["RQ_SC"] = (GridLot.DataKeys[e.RowIndex][2].ToString());
            }

            dt_goods.Rows[0]["HSJE"] = Convert.ToDecimal(dt_goods.Rows[0]["HSJJ"]) * Convert.ToDecimal(dt_goods.Rows[0]["BZSL"]) * Convert.ToDecimal(dt_goods.Rows[0]["BZHL"]);

            LoadGridRow(dt_goods.Rows[0], true, "OLD");
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

        #region 高值条码处理
        protected void btnScan_Click(object sender, EventArgs e)
        {
            //越库商品不允许退货？
            if ((",N").IndexOf(docFLAG.SelectedValue) < 0)
            {
                Alert.Show("非『新单』单据不允许操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
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
        }
        protected void zsmScan_TextChanged(object sender, EventArgs e)
        {
            string flag = DbHelperOra.GetSingle("SELECT FLAG FROM DAT_XS_DOC WHERE BILLNO ='" + docBILLNO.Text.Trim() + "'").ToString();
            if ((",M,R").IndexOf(docFLAG.SelectedValue) < 0)
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
            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_XS_EXT WHERE ONECODE = '{0}' AND FLAG = 'Y'", zsmScan.Text)))
            {
                Alert.Show("您输入的追溯码未被使用或已退货,请检查！", "提示信息", MessageBoxIcon.Warning);
                zsmScan.Text = string.Empty;
                zsmScan.Focus();
                return;
            }
            //写入数据库中
            string sSQL = string.Format(@"INSERT INTO DAT_XS_EXT(DEPTID,BILLNO,ROWNO,ONECODE,GDSEQ,GDNAME,BARCODE,UNIT,GDSPEC,DEPTCUR,BZHL,INSTIME,PH,RQ_SC,YXQZ)
                    SELECT '{0}','{1}',NVL((SELECT MAX(ROWNO)+1 FROM DAT_CK_EXT WHERE BILLNO = '{1}'),1),'{2}',GDSEQ,GDNAME,BARCODE,UNIT,GDSPEC,DEPTCUR,BZHL,SYSDATE,PH,RQ_SC,YXQZ
                    FROM DAT_CK_EXT A
                    WHERE A.ONECODE = '{2}' AND ROWNO = 1", docDEPTID.SelectedValue, docBILLNO.Text, zsmScan.Text.Trim());
            DbHelperOra.ExecuteSql(sSQL);
            ScanSearch("");
        }

        protected void zsmDelete_Click(object sender, EventArgs e)
        {
            if ((",M").IndexOf(docFLAG.SelectedValue) < 0)
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
            DbHelperOra.ExecuteSql(string.Format("DELETE FROM DAT_XS_EXT WHERE ONECODE = '{0}'", onecode));
            ScanSearch("");
        }
        #endregion

        private string billSubmitForPad()
        {
            string result = "";
            MyTable mtType = new MyTable("DAT_XS_DOC");
            //TODO
            foreach (string a in Request.Form)
            {
                if (a != "com[]")
                {
                    if (!mtType.ColRow.ContainsKey(a))
                    {
                        mtType.ColRow.Add(a, Request.Form[a]);
                    }
                    else
                    {
                        mtType.ColRow[a] = Request.Form[a];
                    }
                }
            }

            JObject jo = new JObject();
            if (mtType.ColRow["BILLNO"] == null)
            {
                jo.Add("result", "fail");
                jo.Add("data", "请选择需要操作的单据!");
                return JsonConvert.SerializeObject(jo);
            }
            string billno = mtType.ColRow["BILLNO"].ToString();
            string flag = mtType.ColRow["FLAG"].ToString();
            try
            {
                if (flag == "M")
                {
                    if (DbHelperOra.ExecuteSql(string.Format("UPDATE DAT_XS_DOC SET FLAG = 'N' WHERE SEQNO = '{0}' AND FLAG = 'M'", billno)) > 0)
                    {
                        jo.Add("result", "success");
                        jo.Add("data", billno);
                        result = JsonConvert.SerializeObject(jo);
                    }
                }
                else
                {
                    jo.Add("result", "fail");
                    jo.Add("data", "单据状态不正确，请检查！");
                    return JsonConvert.SerializeObject(jo);
                }
            }
            catch (Exception ex)
            {
                jo.Add("result", "fail");
                jo.Add("data", ex.Message);
                return JsonConvert.SerializeObject(jo);
            }
            return result;
        }
        private bool SaveSuccess = false;
        protected void btnTJ_Click(object sender, EventArgs e)
        {
            if (PubFunc.StrIsEmpty(docSEQNO.Text))
            {
                Alert.Show("请选择需要操作的单据！", "操作提示", MessageBoxIcon.Warning);
                return;
            }
            SaveSuccess = false;
            saveEx("Y");
            if (SaveSuccess == false)
                return;
            SaveSuccess = false;
            if (docFLAG.SelectedValue == "M")
            {
                if (DbHelperOra.ExecuteSql(string.Format("UPDATE DAT_XS_DOC SET FLAG = 'N' WHERE SEQNO = '{0}' AND FLAG = 'M'", docSEQNO.Text)) > 0)
                {
                    Alert.Show("单据【" + docSEQNO.Text + "】提交成功！");
                    OperLog("高值使用", "提交单据【" + docSEQNO.Text + "】");
                    billOpen(docSEQNO.Text);
                    return;
                }
                else
                {
                    Alert.Show("请刷新界面后重试！", "操作提示", MessageBoxIcon.Warning);
                }
            }
            else
            {
                Alert.Show("非【新单】状态无法提交，请检查！", "操作提示", MessageBoxIcon.Warning);
                return;
            }
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
        //测试使用his_inp_bill_detail_1，his_pat_master_index_1
        string optSql = @"SELECT A.ITEM_CODE,A.OPER_CODE,B.NAME,B.ID_NO,A.DOCTOR_USER||' - '||A.PERFORM_DOCTOR DOCTOR FROM his_inp_bill_detail_1 A,his_pat_master_index_1 B 
            WHERE A.PATIENT_ID = B.PATIENT_ID";
        protected void docOPTID_TriggerClick(object sender, EventArgs e)
        {
            //手术ID
            if (docOPTID.Text.Trim().Length < 1) return;
            string Stch = string.Format(" AND A.ITEM_CODE LIKE '%{0}%'", docOPTID.Text);
            DataTable Dt = DbHelperOra.Query(optSql + Stch).Tables[0];
            if (Dt.Rows.Count < 1) return;
            GridHis.DataSource = Dt;
            GridHis.DataBind();
            WindowHis.Hidden = false;
        }
        protected void docDOCTOR_TriggerClick(object sender, EventArgs e)
        {
            //医生
            if (docDOCTOR.Text.Trim().Length < 1) return;
            string Stch = string.Format(" AND (A.DOCTOR_USER LIKE '%{0}%' OR A.PERFORM_DOCTOR LIKE '%{0}%')", docDOCTOR.Text);
            DataTable Dt = DbHelperOra.Query(optSql + Stch).Tables[0];
            if (Dt.Rows.Count < 1) return;
            GridHis.DataSource = Dt;
            GridHis.DataBind();
            WindowHis.Hidden = false;
        }

        protected void docOPTTABLE_TriggerClick(object sender, EventArgs e)
        {
            //手术台号
            if (docOPTTABLE.Text.Trim().Length < 1) return;
            string Stch = string.Format(" AND A.OPER_CODE LIKE '%{0}%'", docOPTTABLE.Text);
            DataTable Dt = DbHelperOra.Query(optSql + Stch).Tables[0];
            if (Dt.Rows.Count < 1) return;
            GridHis.DataSource = Dt;
            GridHis.DataBind();
            WindowHis.Hidden = false;
        }

        protected void docCUSTID_TriggerClick(object sender, EventArgs e)
        {
            //患者
            if (docCUSTID.Text.Trim().Length < 1) return;
            string Stch = string.Format(" AND B.NAME LIKE '%{0}%'", docCUSTID.Text);
            DataTable Dt = DbHelperOra.Query(optSql + Stch).Tables[0];
            if (Dt.Rows.Count < 1) return;
            GridHis.DataSource = Dt;
            GridHis.DataBind();
            WindowHis.Hidden = false;
        }

        protected void docSTR2_TriggerClick(object sender, EventArgs e)
        {
            //身份证
            if (docSTR2.Text.Trim().Length < 1) return;
            string Stch = string.Format(" AND B.ID_NO LIKE '%{0}%'", docSTR2.Text);
            DataTable Dt = DbHelperOra.Query(optSql + Stch).Tables[0];
            if (Dt.Rows.Count < 1) return;
            GridHis.DataSource = Dt;
            GridHis.DataBind();
            WindowHis.Hidden = false;
        }

        protected void GridHis_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            //赋值
            if (e.RowIndex < 0) return;
            docOPTID.Text = (GridHis.DataKeys[e.RowIndex][0] ?? "").ToString();
            docDOCTOR.Text = (GridHis.DataKeys[e.RowIndex][4] ?? "").ToString();
            docOPTTABLE.Text = (GridHis.DataKeys[e.RowIndex][1] ?? "").ToString();
            docCUSTID.Text = (GridHis.DataKeys[e.RowIndex][2] ?? "").ToString();
            docSTR2.Text = (GridHis.DataKeys[e.RowIndex][3] ?? "").ToString();
            WindowHis.Hidden = true;
        }

        protected void btnCse_Click(object sender, EventArgs e)
        {
            WindowHis.Hidden = true;
        }
        private void TJ_YKGZ()
        {//天津医科大学总医院高值 客户化需求 2016年6月20日 17:16:25
            docSTR8.Label = "性  别";
            docDOCTOR.Label = "年  龄";
            docSTR9.Label = "身份证号";
            docSTR7.Required = true;
            docSTR7.ShowRedStar = true;
            docCUSTID.Required = true;
            docCUSTID.ShowRedStar = true;
        }
    }
}