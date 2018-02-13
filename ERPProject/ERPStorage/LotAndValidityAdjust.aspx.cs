﻿using FineUIPro;
using Newtonsoft.Json.Linq;
using XTBase;
using XTFramework.XTFrame;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System.Text;
using System.Collections.Specialized;
using XTBase.Utilities;

namespace ERPProject.ERPStorage
{
    public partial class LotAndValidityAdjust : BillBase
    {

        string strDocSql = "SELECT * FROM DAT_PHTZ_DOC WHERE SEQNO ='{0}'";
        string strComSql = @" SELECT A.GDSEQ,ROWNO,A.GDNAME,f_getunitname(A.UNIT) UNITNAME,A.GDSPEC,A.HSJJ,A.HSJE,A.HWID,A.BZHL,A.KCSL,A.SL,A.PH,A.NEWPH,A.PZWH,A.YXQZ,A.Newyxqz,A.PRODUCER
                             FROM DAT_PHTZ_COM A,DOC_GOODS B  WHERE A.GDSEQ=B.GDSEQ  AND  A.SEQNO = '{0}'  ";

        private string strSql = @"SELECT A.SEQNO,A.BILLNO,F_GETBILLFLAG(FLAG) FLAG_CN,A.FLAG,F_GETDEPTNAME(A.DEPTID) DEPTID,
                                A.TPRQ,A.SUBSUM,F_GETUSERNAME(A.CGY) CGY,
                                F_GETUSERNAME(A.LRY) LRY,A.LRRQ,F_GETUSERNAME(A.SHR) SHR,A.SHRQ,A.MEMO 
                                FROM DAT_PHTZ_DOC A  ";

        protected string GoodOrder = "/grf/GoodsOrder.grf";
        public override Field[] LockControl
        {
            get { return new Field[] { docBILLNO, docCGY, docDEPTID, docTPRQ, docMEMO }; }
        }


        public LotAndValidityAdjust()
        {
            BillType = "TPH";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
                billNew();
                //屏蔽不需要的操作按钮
                if (Request.QueryString["oper"] != null)
                {
                    //获取oper的值
                    hfdOper.Text = Request.QueryString["oper"].ToString();

                    if (Request.QueryString["oper"].ToString() == "input")
                    {
                        ButtonHidden(btnAudit, btnAuditBatch, btnCancel);
                        WebLine3.Hidden = true;
                        PubFunc.DdlDataGet("DDL_BILL_STATUSDHD", lstFLAG);
                    }
                    else if (Request.QueryString["oper"].ToString() == "audit")
                    {
                        PubFunc.DdlDataGet("DDL_BILL_STATUSFCH", lstFLAG);
                        billLockDoc(true);
                        ButtonHidden(btnNew, btnDelRow, btnSave, btnGoods, btnCommit, btnAllCommit, btnDel);
                        ListLine2.Hidden = true;
                        WebLine3.Hidden = true;
                        TabStrip1.ActiveTabIndex = 0;
                        if (Request.QueryString["pid"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["pid"].ToString()))
                        {
                            lstBILLNO.Text = Request.QueryString["pid"].ToString();
                            string date = "20" + lstBILLNO.Text.Substring(3, 2) + "-" + lstBILLNO.Text.Substring(5, 2) + "-" + lstBILLNO.Text.Substring(7, 2);
                            lstLRRQ1.SelectedDate = DateTime.Parse(date).AddDays(-1);
                            billOpen(lstBILLNO.Text);
                        }
                        else
                        {
                            billSearch();
                        }
                    }
                }

            }
        }

        private Boolean isDg()
        {
            if (Request.QueryString["dg"] != null && Request.QueryString["dg"].ToString() == "1")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void DataInit()
        {
            //代管需要绑定代管供应商，其他绑定非代管供应商
            if (!isDg())
            {
                PubFunc.DdlDataGet("DDL_DOC_SUPPLIER", lstPSSID, docPSSID);
            }
            else
            {
                //TODO 增加sys_report 代管供应商
                string sql = @"select  CODE,NAME from (
                                SELECT '--请选择--' NAME,'' CODE   FROM dual
                                union all
                                SELECT SUPNAME NAME,SUPID CODE FROM DOC_SUPPLIER where STR1 = 'N' AND 
                                SUPID IN (SELECT DISTINCT NVL(PSSID,SUPID) FROM DOC_GOODSSUP WHERE TYPE = '1')
                                )
                                ORDER BY DECODE(CODE,'',99,0) DESC ,CODE ASC ";
                PubFunc.DdlDataSql(docPSSID, sql);
                PubFunc.DdlDataSql(lstPSSID, sql);
                //PubFunc.DdlDataGet("DDL_DOC_SUPPLIER_DG", docPSSID, lstPSSID);

            }
            // docPSSID.SelectedIndex = 1;
            PubFunc.DdlDataGet(ddlReject, "DDL_BILL_REASON");
            PubFunc.DdlDataGet("DDL_SYS_DEPOTRANGE_ORDER", UserAction.UserID, docDEPTID, lstDEPTID);
            PubFunc.DdlDataGet("DDL_GOODSTYPE", docDHLX);
            PubFunc.DdlDataGet("DDL_BILL_STATUSDHD", docFLAG, lstFLAG);
            PubFunc.DdlDataGet("DDL_USER", lstLRY, lstCGY, docLRY, docCGY, docSHR);
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
            docDEPTID.SelectedValue = Doc.DbGetSysPara("DEFDEPT");
            if (Request.QueryString["tp"] != null)
            {
                docDHLX.SelectedValue = Request.QueryString["tp"].ToString();
            }
            else
            {
                docDHLX.SelectedValue = "2";
            }
        }

        protected override void billNew()
        {
            string strSup = strSup = Doc.DbGetSysPara("SUPPLIER");
            string strDept = docDEPTID.SelectedValue;
            //原单据保存判断
            PubFunc.FormDataClear(FormDoc);

            if (PubFunc.StrIsEmpty(strDept))
            {
                if (docDEPTID.Items.Count > 2 && !isDg())
                    strDept = docDEPTID.Items[1].Value;
            }
            docFLAG.SelectedValue = "M";
            docCGY.SelectedValue = UserAction.UserID;
            docLRY.SelectedValue = UserAction.UserID;
            docLRRQ.SelectedDate = DateTime.Now;
            docTPRQ.SelectedDate = DateTime.Now;
            docDEPTID.SelectedValue = strDept;
            docPSSID.Enabled = true;
            docPSSID.SelectedValue = strSup;
            //  nbxBZSL.Enabled = true;
            comMEMO.Enabled = true;
            billLockDoc(false);
            docFLAG.Enabled = false;
            docTPRQ.Enabled = true;
            btnGoods.Enabled = true;
            //cbxISYX.Enabled = true;
            docDHLX.Enabled = true;
            GridGoods.SummaryData = null;
            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());

            btnDel.Enabled = false;
            btnSave.Enabled = true;
            btnCommit.Enabled = false;
            btnAudit.Enabled = false;
            btnCancel.Enabled = false;
            btnPrint.Enabled = false;
            btnDelRow.Enabled = true;
            btnGoods.Enabled = true;
            if (Request.QueryString["tp"] != null)
            {
                docDHLX.SelectedValue = Request.QueryString["tp"].ToString();
            }
            else
            {
                docDHLX.SelectedValue = "2";
            }
        }

        protected void GridGoods_AfterEdit(object sender, FineUIPro.GridAfterEditEventArgs e)
        {
            if (e.ColumnID == "SL" || e.ColumnID == "HSJJ")
            {
                string[] strCell = GridGoods.SelectedCell;
                List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
                if (newDict.Count == 0) return;
                JObject defaultObj = Doc.GetJObject(GridGoods, e.RowID);

                if (!PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "SL")) || !PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "HSJJ")))
                {
                    Alert.Show("商品信息异常，请详细检查商品信息：调整数量或价格！", "异常信息", MessageBoxIcon.Warning);
                    return;
                }

                decimal hl = 0, rs = 0, jg = 0;
                //  decimal.TryParse((defaultObj["BZHL"] ?? "0").ToString(), out hl);
                decimal.TryParse((defaultObj["SL"] ?? "0").ToString(), out rs);
                decimal.TryParse((defaultObj["HSJJ"] ?? "0").ToString(), out jg);
                // defaultObj["DHS"] = rs * hl;
                defaultObj["HSJE"] = Math.Round(rs * jg, 2).ToString("F2");
                PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(e.RowID, defaultObj));

                #region //计算合计数量
                decimal bzslTotal = 0, feeTotal = 0;
                foreach (Dictionary<string, object> dic in newDict)
                {
                    bzslTotal += Convert.ToDecimal(dic["SL"] ?? "0");
                    feeTotal += (Convert.ToDecimal(dic["HSJJ"] ?? "0") * Convert.ToDecimal((dic["SL"] ?? "0")));
                }
                JObject summary = new JObject();
                summary.Add("GDNAME", "本页合计");
                summary.Add("SL", bzslTotal.ToString("F2"));
                summary.Add("HSJE", feeTotal.ToString("F2"));

                GridGoods.SummaryData = summary;
                //后调函数验证数据库
                hdfBH.Text = "1";
                #endregion
            }
        }

        protected override void billClear()
        {
            PubFunc.FormDataClear(Formlist);
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;

            //还原按钮显示状态

        }
        private void buttonClear(string strBillno)
        {//还原按钮显示状态
            bntClear.Enabled = true;
            btnAllCommit.Enabled = true;
            btnAuditBatch.Enabled = true;
            bntSearch.Enabled = true;
            btnNew.Enabled = true;
            btnDel.Enabled = true;
            btnSave.Enabled = true;
            btnCommit.Enabled = true;
            btnAudit.Enabled = true;
            btnCancel.Enabled = true;
            btnPrint.Enabled = true;
            btnExport.Enabled = true;
            btnDelRow.Enabled = true;
            btnGoods.Enabled = true;
            if (DbHelperOra.Exists("select 1 from DAT_PHTZ_DOC where SEQNO = '" + strBillno + "' AND (FLAG='N' or FLAG='M')"))
            {
                btnPrint.Enabled = false;
            }
            if (DbHelperOra.Exists("select 1 from DAT_PHTZ_DOC where SEQNO = '" + strBillno + "' AND FLAG='Y'"))
            {
                btnDel.Enabled = false;
                btnSave.Enabled = false;
                btnCommit.Enabled = false;
                btnDelRow.Enabled = false;
                btnGoods.Enabled = false;
            }
        }



        protected override void billCancel()
        {
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("只有已提交的单据才能驳回！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            WindowReject.Hidden = false;
        }
        protected void btnRejectSubmit_Click(object sender, EventArgs e)
        {
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("已提交的单据不能驳回！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            //增加待办事宜
            // List<CommandInfo> cmdList = new List<CommandInfo>();
            //cmdList.Add(new CommandInfo("UPDATE DAT_DO_LIST SET FLAG = 'Y' WHERE PARA='" + docBILLNO.Text.Trim() + "'", null));
            //if (docBILLNO.Text.Length < 1)
            //{ return; }

            if (string.IsNullOrWhiteSpace(ddlReject.SelectedValue))
            {
                Alert.Show("请选择驳回原因", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string strMemo = "驳回原因：" + ddlReject.SelectedText;
            if (!string.IsNullOrWhiteSpace(txaMemo.Text.Trim()))
            {
                strMemo += "；详细说明：" + txaMemo.Text;
            }
            if (DbHelperOra.ExecuteSql(string.Format("UPDATE DAT_PHTZ_DOC set flag='R',memo='{0}' where seqno='{1}' and flag='N'", strMemo, docBILLNO.Text)) == 1)
            {
                //  DbHelperOra.ExecuteSqlTran(cmdList);
                WindowReject.Hidden = true;
                billOpen(docBILLNO.Text);
                Alert.Show("单据驳回成功！");
            };
        }
        protected override void billDelRow()
        {
            if (docFLAG.SelectedValue != "M" && docFLAG.SelectedValue != "R")
            {
                Alert.Show("非新增单据不能删除！");
                return;
            }
            if (!CheckFlag(docFLAG.SelectedValue))
            {
                Alert.Show("此单据已经被别人操作，请等待操作!");
                return;
            }
            if (GridGoods.SelectedRowID == null)
            {
                Alert.Show("未选中任何行，无法进行【删行】操作!");
                return;
            }
            GridGoods.DeleteSelectedRows();
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
            UpdateSum(newDict);
        }

        protected override void billGoods()
        {
            //ymh取消后台调用商品界面
            if (docFLAG.SelectedValue != "M" && docFLAG.SelectedValue != "R")
            {
                Alert.Show("非新单不能增加商品");
                return;
            }
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
            billLockDoc(true);
            docMEMO.Enabled = true;
            docDHLX.Enabled = false;
            docPSSID.Enabled = false;
            DataSearch();
            WindowLot.Hidden = false;
            //string url;
            //if (isDg())
            //{
            //    //供应商不决定商品
            //    url = "~/ERPQuery/GoodsWindow_New.aspx?Deptout=" + docDEPTID.SelectedValue + "&shs="+docPSSID.SelectedValue+ "&goodsType="+docDHLX.SelectedValue+"&isdg=Y&isbd=N&GoodsState=Y";
            //}
            //else
            //{
            //    url = "~/ERPQuery/GoodsWindow_New.aspx?Deptout=" + docDEPTID.SelectedValue + "&shs = "+ docPSSID.SelectedValue + "&goodsType=" + docDHLX.SelectedValue + "&isdg=N&isbd=N&GoodsState=Y";
            //}
            //PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "商品信息查询"));
        }

        protected override void billSearch()
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【订货日期】！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                Alert.Show("【开始日期】大于【结束日期】，请重新输入！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            string strSql = @"SELECT A.SEQNO,A.BILLNO,A.FLAG,B.NAME FLAG_CN,F_GETDEPTNAME(A.DEPTID) DEPTID,A.TPRQ,F_GETSUPNAME(A.PSSID) SUPNAME,A.SUBSUM,
                                     A.SUBNUM,F_GETUSERNAME(A.LRY) LRY,A.LRRQ,F_GETUSERNAME(A.SHR) SHR,A.SHRQ,A.MEMO,
                                     DECODE(A.ISSEND,'N','等待传输','Y','平台处理中','E','平台处理错误','S','平台处理完成','入库完成') ISSENDNAME
                                from DAT_PHTZ_DOC A, SYS_CODEVALUE B 
                                WHERE A.FLAG = B.CODE AND B.TYPE  ='DIC_BILLSTATUS' 
                                AND A.DEPTID in(select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '" + UserAction.UserID + "') = 'Y' ) ";
            string strSearch = "";

            if (lstBILLNO.Text.Trim().Length > 0)
            {
                strSearch += string.Format(" AND A.BILLNO  LIKE '%{0}%'", lstBILLNO.Text.Trim());
            }
            if (lstFLAG.SelectedItem != null && lstFLAG.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.FLAG='{0}'", lstFLAG.SelectedItem.Value);
            }
            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedItem.Value);
            }
            if (lstLRY.SelectedItem != null && lstLRY.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.LRY='{0}'", lstLRY.SelectedItem.Value);
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
            //代管筛选条件修改
            if (isDg())
            {
                //2-代管
                //系统默认00001表示医院，并且为代管模式（主要是针对医院初始库存导入的情况） By c 20160116
                strSearch += " AND (PSSID = '00001' OR PSSID IN (SELECT DISTINCT NVL(A.PSSID, A.SUPID) PSSID FROM DOC_GOODSSUP A WHERE A.TYPE ='1')) AND A.DHFS='2' ";
            }
            else
            {
                //1-托管，3-代管转正常
                strSearch += " AND  PSSID IN (SELECT DISTINCT NVL(A.PSSID, A.SUPID) PSSID FROM DOC_GOODSSUP A WHERE A.TYPE IN ('0','Z'))";
            }

            strSearch += string.Format(" AND A.LRRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.LRRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY A.LRRQ DESC,A.BILLNO DESC";
            highlightRows.Text = "";
            highlightRowYellow.Text = "";
            highlightRowRed.Text = "";
            GridList.DataSource = DbHelperOra.Query(strSql).Tables[0];
            GridList.DataBind();
        }

        protected override void billAudit()
        {
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("非提交单据不能审核！");
                return;
            }
            string strBillno = docSEQNO.Text;

            if (!Doc.getFlag(strBillno, "N", BillType))
            {
                Alert.Show("此单据已经被别人操作，不能审核！");
                return;
            }
            //审核的时候允许修改订单信息 如果允许需要重新保存．
            //DataSave(false);
            if (BillOper(strBillno, "AUDIT") == 1)
            {
                billLockDoc(true);
                Alert.Show("单据【" + strBillno + "】审核成功！");
                billOpen(strBillno);
                OperLog("商品订货", "审核单据【" + docBILLNO.Text + "】");
            }
        }

        protected override void listRow_DoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            string strBillno = GridList.Rows[e.RowIndex].Values[1].ToString();
            billClear();
            buttonClear(strBillno);
            billOpen(strBillno);

            if ((",M,R").IndexOf(docFLAG.SelectedValue) > 0)
            {
                PubFunc.FormLock(FormDoc, true);
                comMEMO.Enabled = true;
                //nbxBZSL.Enabled = true;
                //dpkDHRQ.Enabled = true;
                docMEMO.Enabled = true;
                docDHLX.Enabled = false;
                //cbxISYX.Enabled = true;
                //docDHLX.Enabled = true;
            }
            else
            {
                PubFunc.FormLock(FormDoc, true);
                //  nbxBZSL.Enabled = false;
                comMEMO.Enabled = false;
            }
        }

        protected override void billOpen(string strBillno)
        {
            DataTable dtDoc = DbHelperOra.Query(string.Format(strDocSql, strBillno)).Tables[0];
            if (dtDoc != null && dtDoc.Rows.Count > 0)
            {
                PubFunc.FormDataSet(FormDoc, dtDoc.Rows[0]);
                page(strBillno, 0);
            }
            //增加按钮控制
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
                //  btnNew.Enabled = false;
                btnSave.Enabled = false;
                btnDelRow.Enabled = false;
                btnPrint.Enabled = false;
                btnExport.Enabled = false;

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
            }

        }
        protected void page(string billNo, int PageIndex)
        {
            if (PageIndex > 0)
            {
                GridGoods.PageIndex = Convert.ToInt32(hdftest.Text);
            }
            string strWhere = "";

            int total = 0;
            DataTable dtBill = DbHelperOra.Query(string.Format(strComSql + strWhere + " order by rowno asc", billNo)).Tables[0];

            decimal ddslTotal = 0, bzslTotal = 0, feeTotal = 0;
            foreach (DataRow row in dtBill.Rows)
            {
                //ddslTotal += Convert.ToDecimal(string.IsNullOrWhiteSpace(row["DHSL"].ToString()) ? "0" : row["DHSL"].ToString());
                bzslTotal += Convert.ToDecimal(string.IsNullOrWhiteSpace(row["SL"].ToString()) ? "0" : row["SL"].ToString());
                feeTotal += (Convert.ToDecimal(row["HSJJ"]) * Convert.ToDecimal(row["SL"] ?? "0"));
            }
            GridGoods.RecordCount = total;
            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            //PubFunc.GridRowAdd(GridGoods, dtBill);
            Doc.GridRowAdd(GridGoods, dtBill);

            //计算合计数量
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("SL", bzslTotal.ToString("F2"));
            summary.Add("HSJE", feeTotal.ToString("F2"));
            GridGoods.SummaryData = summary;
            hdfBH.Text = "0";
            PubFunc.FormLock(FormDoc, true, "");
            if ((",M,R").IndexOf(docFLAG.SelectedValue) > 0) { docMEMO.Enabled = true; }
            TabStrip1.ActiveTabIndex = 1;
        }
        protected override void billSave()
        {
            DataSave(true);
        }
        /// <summary>
        /// 该功能是适应订货页面分页时使用，前台页面已经改为不分页，故该功能作废改用save() By c
        /// </summary>
        /// <param name="flag"></param>
        private void pagesave(bool flag)
        {
            #region 数据有效性验证
            if (docFLAG.SelectedValue != "M" && docFLAG.SelectedValue != "R")
            {
                Alert.Show("非新单不能保存！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!CheckFlag(docFLAG.SelectedValue))
            {
                Alert.Show("此单据已经被别人操作，请等待操作!");
                return;
            }

            if (PubFunc.FormDataCheck(FormDoc).Length > 0) return;
            string GoodsFlag = "Y";
            List<Dictionary<string, object>> newDict = new List<Dictionary<string, object>>();

            if (DbHelperOra.Exists("SELECT 1 FROM SYS_PARA WHERE CODE = 'GOODSMODE' AND VALUE = 'N'"))
            { GoodsFlag = "M"; }
            if (GoodsFlag == "Y")
            {
                //不允许排序变动 王阿磊 2015年9月29日 14:48:44
                //newDict = GridGoods.GetNewAddedList().OrderBy(x => x["GDSEQ"]).ToList();
                newDict = GridGoods.GetNewAddedList().ToList();
            }
            else
            { newDict = GridGoods.GetNewAddedList(); }
            if (newDict.Count == 0)
            {
                Alert.Show("请输入商品信息", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string gdseq = "";
            List<Dictionary<string, object>> goodsData = new List<Dictionary<string, object>>();
            //判断是否有空行、批号填写是否符合要求
            for (int i = 0; i < newDict.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(newDict[i]["GDSEQ"].ToString()) && !string.IsNullOrWhiteSpace(newDict[i]["GDNAME"].ToString()))
                {

                    //if (newDict[i]["BZSL"] ==null )
                    //{
                    //    Alert.Show("请填写商品[" + newDict[i]["GDSEQ"] + "]订货数量！", "消息提示", MessageBoxIcon.Warning);
                    //    return;
                    //}
                    if ((newDict[i]["BZSL"] ?? "").ToString() == "" || (newDict[i]["BZSL"] ?? "").ToString() == "0")
                    {
                        Alert.Show("请填写商品[" + newDict[i]["GDSEQ"] + "]订货数量！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    if ((newDict[i]["HSJJ"] ?? "").ToString() == "" || (newDict[i]["HSJJ"] ?? "").ToString() == "0" || Convert.ToDecimal(newDict[i]["HSJJ"]) == 0)
                    {
                        Alert.Show("商品[" + newDict[i]["GDSEQ"] + "]的【含税进价】不能为零,请维护好商品的含税进价，再次保存！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(newDict[i]["BZHL"].ToString()) || string.IsNullOrWhiteSpace(newDict[i]["UNIT"].ToString()))
                    {
                        Alert.Show("商品[" + newDict[i]["GDSEQ"] + "]包装单位信息错误，请联系管理员维护！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    //重新计算订货数/金额
                    newDict[i]["DHS"] = decimal.Parse(newDict[i]["BZSL"].ToString()) * decimal.Parse(newDict[i]["BZHL"].ToString());
                    newDict[i]["HSJE"] = decimal.Parse(newDict[i]["HSJJ"].ToString()) * decimal.Parse(newDict[i]["BZSL"].ToString());
                    if (gdseq != newDict[i]["GDSEQ"].ToString() || GoodsFlag == "M")
                    {
                        gdseq = newDict[i]["GDSEQ"].ToString();
                        goodsData.Add(newDict[i]);
                    }
                    else
                    {
                        if (goodsData[goodsData.Count - 1]["GDSEQ"].ToString() == gdseq)
                        {
                            goodsData[goodsData.Count - 1]["BZSL"] = decimal.Parse(goodsData[goodsData.Count - 1]["BZSL"].ToString()) + decimal.Parse(newDict[i]["BZSL"] == null ? "0" : newDict[i]["BZSL"].ToString());
                            //取消计算到货数
                            //goodsData[goodsData.Count - 1]["DHSL"] = decimal.Parse(goodsData[goodsData.Count - 1]["DHSL"].ToString()) + decimal.Parse(newDict[i]["DHSL"].ToString());
                            goodsData[goodsData.Count - 1]["HSJE"] = decimal.Parse(goodsData[goodsData.Count - 1]["HSJE"].ToString()) + decimal.Parse(newDict[i]["HSJE"] == null ? "0" : newDict[i]["HSJE"].ToString());
                            //由于界面问题,如果未计算订货数，保存时自动计算
                            goodsData[goodsData.Count - 1]["DHS"] = decimal.Parse(goodsData[goodsData.Count - 1]["DHS"].ToString()) + decimal.Parse(newDict[i]["DHS"].ToString());
                        }
                    }
                }
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
                string flg = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M') FROM DAT_DD_DOC WHERE SEQNO='{0}'", docBILLNO.Text));
                if (!PubFunc.StrIsEmpty(flg) && (",M,R").IndexOf(flg) < 0)
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
            MyTable mtType = new MyTable("DAT_PHTZ _DOC");
            mtType.ColRow = PubFunc.FormDataHT(FormDoc);
            mtType.ColRow["SEQNO"] = docBILLNO.Text;
            mtType.ColRow["FLAG"] = "M";
            mtType.ColRow.Add("BILLTYPE", BillType);
            mtType.ColRow.Add("DEPTID", mtType.ColRow["DEPTID"]);

            if (isDg())
            {
                mtType.ColRow["DHFS"] = "2";
            }
            if (Grid.PageInsert(docSEQNO.Text, hdfGL.Text, "DAT_DD_COM", GridGoods.PageIndex, GridGoods.PageSize, goodsData))
            {
                if (Grid.saveDoc(mtType, "DAT_DD_DOC", docSEQNO.Text))
                {
                    if (flag)
                    {
                        Alert.Show("调整信息保存成功！");
                        billOpen(docBILLNO.Text);
                        billSearch();
                        OperLog("商品订货", "修改单据【" + docBILLNO.Text + "】");
                    }
                }
                else
                {
                    Alert.Show("调整信息保存失败，请联系管理员检查原因！");
                }
            }
            else
            {
                Alert.Show("调整信息保存失败，请联系管理员检查原因！");
            }
        }
        private void DataSave(bool flag)
        {
            #region 数据有效性验证
            if (flag)
            {
                if (docFLAG.SelectedValue != "M" && docFLAG.SelectedValue != "R")
                {
                    Alert.Show("非新单不能保存！", "消息提示", MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                if (docFLAG.SelectedValue != "N")
                {
                    Alert.Show("非已提交单据不能审核！", "消息提示", MessageBoxIcon.Warning);
                    return;
                }
            }
            if (!CheckFlag(docFLAG.SelectedValue))
            {
                Alert.Show("此单据已经被别人操作，请等待操作!");
                return;
            }
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().ToList();

            if (newDict.Count == 0)
            {
                Alert.Show("请输入商品信息", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string gdseq = "";
            List<Dictionary<string, object>> goodsData = new List<Dictionary<string, object>>();
            //判断是否有空行、批号填写是否符合要求
            for (int i = 0; i < newDict.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(newDict[i]["GDSEQ"].ToString()) && !string.IsNullOrWhiteSpace(newDict[i]["GDNAME"].ToString()))
                {
                    if ((newDict[i]["SL"] ?? "").ToString() == "" || (newDict[i]["SL"] ?? "").ToString() == "0")
                    {
                        Alert.Show("请填写商品[" + newDict[i]["GDSEQ"] + "]调整数量！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    if ((newDict[i]["SL"] ?? "").ToString() != "0" && Convert.ToDecimal(newDict[i]["SL"]) > Convert.ToDecimal(newDict[i]["KCSL"]))
                    {
                        Alert.Show("商品[" + newDict[i]["GDSEQ"] + "]调整数量不能大于库存！", "消息提示", MessageBoxIcon.Warning);
                        return;

                    }
                    if (newDict[i].Keys.Contains("NEWPH") == false && !newDict[i].Keys.Contains("NEWYXQZ") == false)
                    {
                        Alert.Show("商品[" + newDict[i]["GDSEQ"] + "]调整批号和效期至少一项不为空！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    if (newDict[i].Keys.Contains("NEWPH") == true && newDict[i]["NEWPH"] == newDict[i]["PH"])
                    {
                        Alert.Show("商品[" + newDict[i]["GDSEQ"] + "]调整新批号和原批号不能一致！", "消息提示", MessageBoxIcon.Warning);
                        return;

                    }
                    if (newDict[i].Keys.Contains("NEWYXQZ") == true && newDict[i]["NEWYXQZ"] == newDict[i]["YXQZ"])
                    {
                        Alert.Show("商品[" + newDict[i]["GDSEQ"] + "]调整新有效期和原有效期不能一致！", "消息提示", MessageBoxIcon.Warning);
                        return;

                    }
                    if ((newDict[i]["HSJJ"] ?? "").ToString() == "" || (newDict[i]["HSJJ"] ?? "").ToString() == "0" || Convert.ToDecimal(newDict[i]["HSJJ"]) == 0)
                    {
                        Alert.Show("商品[" + newDict[i]["GDSEQ"] + "]的【含税进价】不能为零,请维护好商品的含税进价，再次保存！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    if (gdseq != newDict[i]["GDSEQ"].ToString())
                    {
                        gdseq = newDict[i]["GDSEQ"].ToString();
                        goodsData.Add(newDict[i]);
                    }
                    //else
                    //{
                    //    if (goodsData[goodsData.Count - 1]["GDSEQ"].ToString() == gdseq)
                    //    {
                    //        goodsData[goodsData.Count - 1]["BZSL"] = decimal.Parse(goodsData[goodsData.Count - 1]["BZSL"].ToString()) + decimal.Parse(newDict[i]["BZSL"] == null ? "0" : newDict[i]["BZSL"].ToString());
                    //        //取消计算到货数
                    //        //goodsData[goodsData.Count - 1]["DHSL"] = decimal.Parse(goodsData[goodsData.Count - 1]["DHSL"].ToString()) + decimal.Parse(newDict[i]["DHSL"].ToString());
                    //        goodsData[goodsData.Count - 1]["HSJE"] = decimal.Parse(goodsData[goodsData.Count - 1]["HSJE"].ToString()) + decimal.Parse(newDict[i]["HSJE"] == null ? "0" : newDict[i]["HSJE"].ToString());
                    //        //由于界面问题,如果未计算订货数，保存时自动计算
                    //        goodsData[goodsData.Count - 1]["DHS"] = decimal.Parse(goodsData[goodsData.Count - 1]["DHS"].ToString()) + decimal.Parse(newDict[i]["DHS"].ToString());
                    //    }
                    //}
                }
            }
            if (goodsData.Count == 0)
            {
                Alert.Show("商品商品信息不能为空", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            #endregion

            if (flag)
            {
                if (PubFunc.StrIsEmpty(docBILLNO.Text))
                {
                    docSEQNO.Text = BillSeqGet();
                    docBILLNO.Text = docSEQNO.Text;
                    docBILLNO.Enabled = false;
                }
                else
                {
                    string flg = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M') FROM DAT_DD_DOC WHERE SEQNO='{0}'", docBILLNO.Text));
                    if (!PubFunc.StrIsEmpty(flg) && (",M,R").IndexOf(flg) < 0)
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
            }
            MyTable mtType = new MyTable("DAT_PHTZ_DOC");
            mtType.ColRow = PubFunc.FormDataHT(FormDoc);
            mtType.ColRow["SEQNO"] = docBILLNO.Text;
            if (flag)
            {
                mtType.ColRow["FLAG"] = "M";
            }
            mtType.ColRow.Add("BILLTYPE", BillType);

            mtType.ColRow.Add("SUBNUM", newDict.Count);
            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtTypeMx = new MyTable("DAT_PHTZ_COM");
            decimal subNum = 0;//总金额
            //先删除单据信息在插入
            cmdList.Add(new CommandInfo("DELETE DAT_PHTZ_DOC WHERE SEQNO='" + docBILLNO.Text + "'", null));//删除单据台头
            cmdList.Add(new CommandInfo("DELETE DAT_PHTZ_COM WHERE SEQNO='" + docBILLNO.Text + "'", null));//删除单据明细
            newDict = newDict.OrderBy(x => x["GDSEQ"]).ToList();//按照商品编码重新排序
            for (int i = 0; i < newDict.Count; i++)
            {
                mtTypeMx.ColRow = PubFunc.GridDataGet(newDict[i]);

                mtTypeMx.ColRow.Add("SEQNO", docBILLNO.Text);

                mtTypeMx.ColRow["ROWNO"] = i + 1;
                mtTypeMx.ColRow.Add("BHSJJ", 0);
                mtTypeMx.ColRow.Add("BHSJE", 0);
                //  mtTypeMx.ColRow["DHS"] = decimal.Parse(mtTypeMx.ColRow["BZHL"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                mtTypeMx.ColRow["HSJE"] = (decimal.Parse(mtTypeMx.ColRow["HSJJ"].ToString()) * decimal.Parse(mtTypeMx.ColRow["SL"].ToString()));
                mtTypeMx.ColRow["PHID"] = mtTypeMx.ColRow["PH"];
                if (mtTypeMx.ColRow.Contains("NEWPH") == true && mtTypeMx.ColRow["NEWPH"] == mtTypeMx.ColRow["PH"])
                {
                    mtTypeMx.ColRow["NEWPH"] = "";
                }
                if (mtTypeMx.ColRow.Contains("NEWYXQZ") == true && mtTypeMx.ColRow["NEWYXQZ"] == mtTypeMx.ColRow["YXQZ"])
                {
                    mtTypeMx.ColRow["NEWYXQZ"] = "";
                }
                mtTypeMx.ColRow["DEPTID"] = mtType.ColRow["DEPTID"];
                subNum = subNum + decimal.Parse(mtTypeMx.ColRow["HSJE"].ToString());
                cmdList.Add(mtTypeMx.Insert());
            }
            mtType.ColRow.Add("SUBSUM", subNum);
            cmdList.Add(mtType.Insert());
            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                if (flag)
                {
                    Alert.Show("商品调整数据保存成功！");
                    OperLog("商品调整", "修改单据【" + docBILLNO.Text + "】");
                    billOpen(docBILLNO.Text);
                }
            }
            else
            {
                Alert.Show("商品调整数据保存失败！", "错误提示", MessageBoxIcon.Error);
            }
        }
        public void DataSearch()
        {

            int total = 0;
            string strSql = @"SELECT GD.GDSEQ,
                               GD.GDNAME,
                               GD.GDSPEC,
                               GD.UNIT,
                               F_GETUNITNAME(GD.UNIT) UNITNAME,
                               GS.YXQZ,
                               GS.PHID,
                               GS.PH,
                               GS.RQ_SC,
                               GD.BARCODE,
                               GD.GDMODE,
                               GD.BZHL,
                               GS.HWID,
                               GD.HSJJ,
                               GS.KCSL,
                               GD.PIZNO,
                               F_GETPRODUCERNAME(GD.PRODUCER)  PRODUCER
                          FROM DAT_GOODSSTOCK GS, DOC_GOODS GD
                         WHERE GS.GDSEQ = GD.GDSEQ
                           AND GD.ISGZ = 'N'  AND GS.KCSL>0";
            if (!string.IsNullOrEmpty(docDEPTID.SelectedValue))
            {

                strSql += " AND GS.DEPTID='" + docDEPTID.SelectedValue + "'";
            }
            if (!string.IsNullOrEmpty(docDHLX.SelectedValue))
            {
                strSql += " AND GD.CATID0='" + docDHLX.SelectedValue + "'";

            }
            if (!string.IsNullOrEmpty(docPSSID.SelectedValue))
            {
                strSql += " AND GS.SUPID='" + docPSSID.SelectedValue + "'";

            }

            GridLot.DataSource = PubFunc.DbGetPage(GridLot.PageIndex, GridLot.PageSize, strSql, ref total);
            GridLot.RecordCount = total;
            GridLot.DataBind();
        }
        private DataTable LoadGridData(DataTable dt, ref string msg)
        {
            DataTable mydt = dt.Clone();
            foreach (DataRow row in dt.Rows)
            {
                //处理金额格式
                decimal jingdu = 0;
                if (decimal.TryParse(row["HSJJ"].ToString(), out jingdu)) { row["HSJJ"] = jingdu.ToString("F6"); }
                if (decimal.TryParse(row["YBJ"].ToString(), out jingdu)) { row["YBJ"] = jingdu.ToString("F6"); }
                if (decimal.TryParse(row["DHS"].ToString(), out jingdu)) { row["DHS"] = jingdu.ToString("F6"); }
                if (decimal.TryParse(row["DHSL"].ToString(), out jingdu)) { row["DHSL"] = jingdu.ToString("F6"); }
                if (decimal.TryParse(row["KCSL"].ToString(), out jingdu)) { row["KCSL"] = jingdu.ToString("F6"); }
                if (decimal.TryParse(row["SPZTSL"].ToString(), out jingdu)) { row["SPZTSL"] = jingdu.ToString("F6"); }

                row["MEMO"] = row["MEMO"].ToString() + row["STR0"].ToString();

                if (row["BZHL"].ToString() == "0" || row["BZHL"].ToString() == "")
                {
                    msg += row["GDNAME"].ToString() + ",";
                }
                else
                {
                    mydt.Rows.Add(row.ItemArray);
                }
            }
            return mydt;
        }

        protected void Window1_Close(object sender, WindowCloseEventArgs e)
        {
            DataTable dt = GetGoods(hfdValue.Text);
            dt.Columns.Remove(dt.Columns["BZHL"]);
            dt.Columns.Remove(dt.Columns["UNIT"]);

            if (dt != null && dt.Rows.Count > 0)
            {
                dt.Columns["UNITNAME"].ColumnName = "UNITSMALLNAME";
                dt.Columns["BZHL_ORDER"].ColumnName = "BZHL";
                dt.Columns["UNIT_ORDER_NAME"].ColumnName = "UNITNAME";
                dt.Columns["UNIT_ORDER"].ColumnName = "UNIT";

                dt.Columns.Add("PH", Type.GetType("System.String"));
                dt.Columns.Add("RQ_SC", Type.GetType("System.String"));
                dt.Columns.Add("YXQZ", Type.GetType("System.String"));
                dt.Columns.Add("MEMO", Type.GetType("System.String"));
                dt.Columns.Add("BZSL", Type.GetType("System.Int32"));
                dt.Columns.Add("DHS", Type.GetType("System.Int32"));
                dt.Columns.Add("DHSL", Type.GetType("System.Int32"));
                dt.Columns.Add("HSJE", Type.GetType("System.Double"));
                dt.Columns.Add("KCSL", Type.GetType("System.Int32"));
                dt.Columns.Add("SPZTSL", Type.GetType("System.Int32"));
                string msg = "";
                string someDjbh = string.Empty;
                DataTable dtBill = LoadGridData(dt, ref msg);
                if (dtBill != null && dtBill.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        row["BZSL"] = "0";
                        row["DHSL"] = "0";
                        row["HSJE"] = "0";
                        row["KCSL"] = row["CKKCSL"];
                        //row["HSJJ"].ToString();
                        if (string.IsNullOrWhiteSpace(row["HSJJ"].ToString()) || row["HSJJ"].ToString() == "0")
                        {
                            msg += "【" + row["GDSEQ"].ToString() + " | " + row["GDNAME"].ToString() + "】,";
                            Alert.Show("商品" + msg + "【含税进价】为空，不能进行【商品补货管理】操作。", "消息提示", MessageBoxIcon.Warning);
                            continue;
                        }
                        //LoadGridRow(row, false);
                        //处理金额格式
                        decimal jingdu = 0;
                        decimal bzhl = 0;
                        if (decimal.TryParse(row["HSJJ"].ToString(), out jingdu) && decimal.TryParse(row["BZHL"].ToString(), out bzhl)) { row["HSJJ"] = Math.Round(jingdu * bzhl, 4).ToString("F4"); }
                        if (decimal.TryParse(row["YBJ"].ToString(), out jingdu)) { row["YBJ"] = jingdu.ToString("F4"); }
                        if (decimal.TryParse(row["HSJE"].ToString(), out jingdu)) { row["HSJE"] = Math.Round(jingdu, 2).ToString("F2"); }
                        //if (isDg())
                        //{
                        //    row["NUM2"] = DbHelperOra.GetSingle(string.Format("SELECT NVL(ORDERZQ,7) FROM DOC_GOODSSUP WHERE GDSEQ = '{0}' AND NVL(PSSID,SUPID) = '{1}'", row["GDSEQ"], docPSSID.SelectedValue));
                        //}
                        //取得商品供应商
                        row["SUPID"] = (DbHelperOra.GetSingle(string.Format("SELECT SUPID FROM DOC_GOODSSUP WHERE GDSEQ='{0}'", row["GDSEQ"])) ?? "");

                        List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().OrderBy(x => x["GDSEQ"]).ToList();
                        int sameRowCount = newDict.Where(a => a["GDSEQ"].ToString() == row["GDSEQ"].ToString()).Count();
                        if (sameRowCount > 0)
                        {
                            someDjbh += "【" + row["GDNAME"].ToString() + "】";
                        }
                        else
                        {
                            PubFunc.GridRowAdd(GridGoods, row, false);
                            docDEPTID.Enabled = false;
                        }
                    }
                    //PubFunc.GridRowAdd(GridGoods, dtBill);
                }
                if (!string.IsNullOrWhiteSpace(someDjbh))
                {
                    Alert.Show("商品名称：" + someDjbh + "申请明细中已存在", "消息提示", MessageBoxIcon.Warning);

                }
                if (!string.IsNullOrWhiteSpace(msg))
                {
                    Alert.Show(string.Format("请先维护商品【{0}】的订货包装含量", msg.TrimEnd(',')), "异常提示", MessageBoxIcon.Warning);

                }
            }
            else
            {
                Alert.Show("请选择商品！", "消息提示", MessageBoxIcon.Warning);
            }
        }


        protected void btnClosePostBack_Click(object sender, EventArgs e)
        {
            string strGDSEQS = string.Empty;
            string strPHS = string.Empty;
            if (GridLot.SelectedRowIndexArray.Length == 0)
            {
                Alert.Show("请选择要添加的数据");
                return;
            }
            else
            {
                foreach (int iRow in GridLot.SelectedRowIndexArray)
                {
                    strGDSEQS += GridLot.Rows[iRow].DataKeys[0].ToString() + ",";
                    if (GridLot.Rows[iRow].DataKeys[1] != null)
                    {
                        strPHS += GridLot.Rows[iRow].DataKeys[1].ToString() + ",";
                    }
                    else
                    {
                        strPHS += " ,";
                    }


                }
            }
            if (strGDSEQS.Length > 0)
            {
                string strSql = string.Format(@"SELECT GD.GDSEQ,
                               GD.GDNAME,
                               GD.GDSPEC,
                               GD.UNIT,
                               F_GETUNITNAME(GD.UNIT) UNITNAME,
                               GS.YXQZ,
                               GS.PHID,
                               GS.PH,
                               GS.RQ_SC,
                               GD.BARCODE,
                               GD.GDMODE,
                               GD.BZHL,
                               GS.HWID,
                               GD.HSJJ,
                               0 HSJE,
                               GS.KCSL,
                               0 SL,
                               GD.PIZNO PZWH,
                               GD.ZPBH,
                               GS.YXQZ NewYXQZ,
                               GD.PRODUCER,
                               F_GETPRODUCERNAME(GD.PRODUCER)  PRODUCERNAME,
                               GS.MEMO
                          FROM DAT_GOODSSTOCK GS, DOC_GOODS GD
                         WHERE GS.GDSEQ = GD.GDSEQ
                           AND GD.ISGZ = 'N' AND GS.KCSL>0 AND GD.GDSEQ IN ('{0}') AND  GS.PH IN ('{1}')", strGDSEQS.TrimEnd(',').Replace(",", "','"), strPHS.TrimEnd(',').Replace(",", "','"));
                if (!string.IsNullOrEmpty(docDEPTID.SelectedValue))
                {

                    strSql += " AND GS.DEPTID='" + docDEPTID.SelectedValue + "'";
                }
                if (!string.IsNullOrEmpty(docDHLX.SelectedValue))
                {
                    strSql += " AND GD.CATID0='" + docDHLX.SelectedValue + "'";

                }
                if (!string.IsNullOrEmpty(docPSSID.SelectedValue))
                {
                    strSql += " AND GS.SUPID='" + docPSSID.SelectedValue + "'";

                }
                DataTable dtAdd = DbHelperOra.QueryForTable(strSql);
                PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
                Doc.GridRowAdd(GridGoods, dtAdd);
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
            if (docFLAG.SelectedValue != "M" && docFLAG.SelectedValue != "R")
            {
                Alert.Show("非新单不能删除!");
                return;
            }
            if (!CheckFlag(docFLAG.SelectedValue))
            {
                Alert.Show("此单据已经被别人操作，请等待操作!");
                return;
            }
            List<CommandInfo> cmdList = new List<CommandInfo>();
            cmdList.Add(new CommandInfo("Delete from DAT_PHTZ_DOC t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'", null));
            cmdList.Add(new CommandInfo("Delete from DAT_PHTZ_COM t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'", null));
            cmdList.Add(new CommandInfo(" UPDATE DAT_DO_LIST SET DOUSER='" + UserAction.UserID + "',DORQ=SYSDATE,FLAG='Y' WHERE  PARA='" + docBILLNO.Text.Trim() + "'", null));
            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                Alert.Show("单据删除成功!");
                OperLog("商品订货", "删除单据【" + docBILLNO.Text + "】");
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
            else
            {
                Alert.Show("单据删除失败!", "错误提示", MessageBoxIcon.Information);
            }
        }
        protected override void billExport()
        {
            if (string.IsNullOrWhiteSpace(docSEQNO.Text))
            {
                Alert.Show("请先选择要导出的订单信息！", "提示信息", MessageBoxIcon.Warning);
                Window1.Hidden = true;
                return;
            }
            string sql = @"SELECT A.BILLNO 单据编号,
                                   F_GETDEPTNAME(A.DEPTID) 订货部门,
                                   F_GETSUPNAME(B.SUPID) 供应商,
                                   TO_CHAR(A.XDRQ, 'YYYY-MM-DD') 订货日期,
                                   F_GETUSERNAME(A.CGY) 操作员,
                                   A.MEMO 订单备注说明,
                                   ' '||B.GDSEQ 商品编码,
                                   G.BAR3 ERP编码,
                                   B.ROWNO 行号,
                                   B.GDNAME 商品名称,
                                   B.GDSPEC 商品规格,
                                   F_GETUNITNAME(B.UNIT) 订货单位,
                                   B.BZHL 包装含量,
                                   B.BZSL 订货包装数,
                                   B.DHS 订货数,
                                   B.HSJJ 含税进价,
                                   B.HSJE 含税金额,
                                   B.DHSL 入库数,
                                   B.PZWH 注册证号,
                                   F_GETPRODUCERNAME(B.PRODUCER) 生产厂家,
                                   G.HISCODE HIS编码,
                                   B.MEMO 商品明细备注
                              FROM DAT_PHTZ_DOC A, DAT_PHTZ_COM B,DOC_GOODS G
                             WHERE B.SEQNO = '{0}' AND B.GDSEQ=G.GDSEQ
                               AND A.SEQNO = B.SEQNO
                             ORDER BY ROWNO";
            DataTable dt = DbHelperOra.Query(string.Format(sql, docSEQNO.Text)).Tables[0];
            if (dt.Rows.Count < 1)
            {
                Alert.Show("请先选择要导出的订单信息！", "提示信息", MessageBoxIcon.Warning);
                Window1.Hidden = true;
                return;
            }
            ExcelHelper.ExportByWeb(dt, string.Format("【{0}】订货信息", docDEPTID.SelectedText), "订货信息导出_" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
        }

        protected void btnAuditBatch_Click(object sender, EventArgs e)
        {
            int[] selections = GridList.SelectedRowIndexArray;
            if (selections.Length == 0)
            {
                Alert.Show("请选择要审核的科室申领信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string succeed = "";
            string billno = "";
            string msgUpdate = "";
            bool flagUpdate = false;

            // string shs = docPSSID.SelectedValue;    //供应商

            foreach (int rowIndex in selections)
            {
                //添加拦截判断
                if (GridList.DataKeys[rowIndex][1].ToString() == "N")
                {
                    billno = GridList.DataKeys[rowIndex][0].ToString();

                    //DataTable dtDoc = DbHelperOra.Query(string.Format(strDocSql, billno)).Tables[0];
                    //string supid = dtDoc.Rows[0]["PSSID"].ToString();

                    //DataTable ddComDT = DbHelperOra.Query("SELECT GDSEQ FROM DAT_DD_COM WHERE SEQNO='" + billno + "'").Tables[0];
                    //if (ddComDT.Rows.Count > 0)
                    //{
                    //    string lisResult = "";
                    //    for (int i = 0; i < ddComDT.Rows.Count; i++)
                    //    {
                    //        lisResult = lisResult + DbHelperOra.GetSingle("select F_LICENSE_ISEFFECTIVE('" + supid + "','" + ddComDT.Rows[i]["GDSEQ"].ToString() + "') from dual").ToString() + ",";
                    //    }

                    //    lisResult = lisResult.Substring(0, lisResult.Length - 1).Split(',')[0];
                    //    if (lisResult.Equals("验证通过"))
                    //    {
                    if (!Doc.getFlag(billno, "N", BillType))
                    {
                        msgUpdate += billno + ";";
                        flagUpdate = true;
                    }
                    if (Doc.getFlag(billno, "N", BillType))
                    {
                        if (BillOper(billno, "AUDIT") == 1)
                        {
                            succeed = succeed + "【" + billno + "】";
                        }
                    }
                    if (flagUpdate)
                    {
                        Alert.Show("单据编号【" + msgUpdate + "】被别人在操作，不能执行审核操作！");
                        return;
                    }

                    if (succeed.Length > 0)
                    {
                        Alert.Show("单据" + succeed + "审核成功！");
                        billSearch();
                    }

                }
            }

        }

        protected void PageManager1_CustomEvent(object sender, CustomEventArgs e)
        {


            if (e.EventArgument.IndexOf("GoodsAdd") >= 0)
            {
                Window1_Close(null, null);
            }
            switch (e.EventArgument)
            {
                //删行
                case "billDelRow_Cancel":
                    break;
                case "Confirm_OK":
                    pagesave(false);
                    page(docSEQNO.Text, 1);
                    break;
                case "Confirm_Cancel":
                    page(docSEQNO.Text, 1);
                    break;
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

        protected void lstBILLNO_TriggerClick(object sender, EventArgs e)
        {
            //查询信息统一触发
            billSearch();
        }
        /// <summary>
        ///  20150510 liuz  解决带出信息更新汇总信息显示
        /// </summary>
        /// <param name="newDict"></param>
        private void UpdateSum(List<Dictionary<string, object>> newDict)
        {
            //计算合计数量
            decimal bzslTotal = 0, feeTotal = 0;
            foreach (Dictionary<string, object> dic in newDict)
            {
                bzslTotal += Convert.ToDecimal(dic["SL"] ?? "0");
                feeTotal += (Convert.ToDecimal(dic["HSJJ"]) * Convert.ToDecimal(dic["SL"] ?? "0"));
            }
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("SL", bzslTotal.ToString());
            summary.Add("HSJE", feeTotal.ToString("F2"));

            GridGoods.SummaryData = summary;
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
            string strBillno = docSEQNO.Text;
            if (!Doc.getFlag(strBillno, "M", BillType))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                return;
            }
            sqlList.Add(new CommandInfo("update DAT_PHTZ_DOC set flag='N' where BILLNO='" + strBILLNO + "' ", null));
            //增加待办事宜
            //  sqlList.Add(Doc.GETDOADD("DO_7", docDEPTID.SelectedValue, docLRY.SelectedValue, docBILLNO.Text));

            if (DbHelperOra.ExecuteSqlTran(sqlList))
            {
                Alert.Show("此单据提交成功！");
                OperLog("商品调整", "提交单据【" + docBILLNO.Text + "】");
                billSearch();
                billOpen(docBILLNO.Text);
                billLockDoc(true);
            }
            else
            {
                Alert.Show("此单据提交失败，请联系系统管理人员！");
            }
        }
        protected void btnAllCommit_Click(object sender, EventArgs e)
        {
            List<CommandInfo> sqlList = new List<CommandInfo>();
            string succeed = string.Empty;
            if (GridList.SelectedRowIndexArray.Length == 0)
            {
                Alert.Show("你选中的单据中，没有要提交的单据");
                return;
            }
            for (int i = 0; i < GridList.SelectedRowIndexArray.Length; i++)
            {
                int rowIndex = GridList.SelectedRowIndexArray[i];
                if (GridList.DataKeys[rowIndex][1].ToString() == "M")
                {
                    string strBILLNO = GridList.DataKeys[rowIndex][0].ToString();
                    sqlList.Add(new CommandInfo("update DAT_DD_DOC set flag='N' where BILLNO='" + strBILLNO + "' ", null));
                    //增加待办事宜
                    //  sqlList.Add(Doc.GETDOADD("DO_7", GridList.DataKeys[rowIndex][2].ToString(), GridList.DataKeys[rowIndex][3].ToString(), strBILLNO));
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
            }
        }
        private bool CheckFlag(string flag)
        {
            if (docBILLNO.Text.Length > 0)
            {
                return Doc.getFlag(docBILLNO.Text, flag, BillType);
            }
            return true;
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
        protected void GridGoods_PageIndexChange(object sender, GridPageEventArgs e)
        {
            if (hdfBH.Text == "1" && modif())
            {
                PageContext.RegisterStartupScript(Confirm.GetShowReference("数据已被修改是否保存？", String.Empty, MessageBoxIcon.Question, PageManager1.GetCustomEventReference("Confirm_OK"), PageManager1.GetCustomEventReference("Confirm_Cancel")));
                hdftest.Text = e.NewPageIndex.ToString();
                hdfBH.Text = "0";
            }
            else
            {
                GridGoods.PageIndex = e.NewPageIndex;
                page(docSEQNO.Text, 0);
            }
        }

        protected bool modif()
        {
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
            for (int i = 0; i < newDict.Count; i++)
            {
                if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_DD_COM WHERE SEQNO = '{0}' AND GDSEQ = '{1}' AND BZSL = {2} AND MEMO = '{3}'", docBILLNO.Text, newDict[i]["GDSEQ"], newDict[i]["BZSL"], newDict[i]["MEMO"])))
                {
                    return true;
                }
            }
            return false;
        }
        protected void GridList_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridList.PageIndex = e.NewPageIndex;
            billSearch();
        }

    }
}