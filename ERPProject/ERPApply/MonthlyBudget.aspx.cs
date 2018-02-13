﻿using FineUIPro;
using Oracle.ManagedDataAccess.Client;
using XTBase;
using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XTBase.Utilities;

namespace ERPProject.ERPApply
{
    public partial class MonthlyBudget : BillBase
    {
        private DateTime startDate = new DateTime();
        private DateTime endDate = new DateTime();
        private string strDocSql = "SELECT to_char(c.ysrq,'YYYY-MM'),SEQNO,BILLNO,BILLTYPE,FLAG,DEPTID,YSRANGE,BEGRQ,ENDRQ,SUBNUM,SUBSUM,YSY,LRY,LRRQ,SPR,SPRQ,SHR,SHRQ,MEMO FROM DAT_YS_DOC c WHERE SEQNO ='{0}'";
        private string strComSql = "SELECT c.*,F_GETUNITNAME(C.UNIT) UNITNAME,F_GETPRODUCERNAME(C.PRODUCER) PRODUCERNAME,g.pizno,g.hisname FROM DAT_YS_COM c,doc_goods g where g.gdseq = c.gdseq and c.seqno = '{0}'";

        public MonthlyBudget()
        {
            BillType = "YSD";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                initData();
                newData();

                if (Request.QueryString["oper"] != null)
                {
                    if (Request.QueryString["oper"].ToString() == "input")
                    {
                        ButtonHidden(btnAudit, btnCancel);
                        hfdoper.Text = "input";
                    }
                    else if (Request.QueryString["oper"].ToString() == "audit")
                    {
                        ToolbarText1.Text = "操作信息:月度预算审批界面";
                        billLockDoc(true);
                        ButtonHidden(btnNew, btnGenerate, btnSave, btnSumbit, btnDelRow, btnDelRow);
                        hfdoper.Text = "audit";
                        TabStrip1.ActiveTabIndex = 0;
                        //billSearch();
                    }
                }
            }
        }

        private void initData()
        {
            PubFunc.DdlDataGet("DDL_SYS_DEPOTRANGE", UserAction.UserID, ddlDEPTID);
            PubFunc.DdlDataGet("DDL_USER", docLRY, docSHR);
            PubFunc.DdlDataGet("DDL_SYS_DEPTRANGE", UserAction.UserID, ddlDEPTID, lstDEPTID);
            PubFunc.DdlDataGet("DDL_YS_DEPT", lstFLAG, docFLAG);
            //201509191548  add by zhanghaicheng 
            PubFunc.DdlDataGet("DDL_SYS_DEPTRANGE", UserAction.UserID, ddlDeptYs);
            //   dpkRQSJ1.SelectedDate = DateTime.Now.AddDays(-30);
            PubFunc.DdlDataGet(ddlReject, "DDL_BILL_REASON");
            docFLAG.SelectedValue = "M";   //默认是新单状态（这个后续还要更改，先写成这样）
                                           //    dpkRQSJ2.SelectedDate = DateTime.Now;
                                           //docSHRQ.SelectedDate = DateTime.Now;
            docLRRQ.SelectedDate = DateTime.Now;
            docLRY.SelectedValue = UserAction.UserID;
            //docSHR.SelectedValue = UserAction.UserID;
            //lstBEGRQ.SelectedDate = DateTime.Now.AddMonths(-1);
            //lstENDRQ.SelectedDate = DateTime.Now;
            ksyf.Text = DateTime.Now.ToString("yyyy-MM");
            jsyf.Text = DateTime.Now.AddMonths(1).ToString("yyyy-MM");
            ddlDEPTID.SelectedValue = UserAction.UserDept;
            ddlDeptYs.SelectedValue = UserAction.UserDept;
            //ddlDEPTID.Enabled = false;

            dpEndDate.SelectedDate = DateTime.Now.AddDays(-1);
            dpEndDate.Enabled = true;
            dpStartDate.SelectedDate = DateTime.Now.AddMonths(-1);
            //  dpStartDate.MaxDate = DateTime.Now.AddMonths(-1);

            //docYSRQ.Text = Convert.ToDateTime(dpkRQSJ2.Text).Year.ToString() + "-" + Convert.ToDateTime(dpkRQSJ2.Text).AddMonths(1).Month.ToString();
            // docYSRQ.Text = DateTime.Now.ToString("yyyy") + "-" + ddlMonth.SelectedValue + "-" + "01";
            docYSRQ.Text = DateTime.Now.AddMonths(1).ToString("yyyy-MM");
            docYSRQ.Enabled = false;
        }

        private void newData()
        {
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("YSSL", "0");
            summary.Add("HSJE", "0");
            GridGoods.SummaryData = summary;
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            PubFunc.FormDataClear(Formlist);
            //lstBEGRQ.SelectedDate = DateTime.Now.AddDays(-30);
            //lstENDRQ.SelectedDate = DateTime.Now;
            ksyf.Text = DateTime.Now.ToString("yyyy-MM");
            jsyf.Text = DateTime.Now.AddMonths(1).ToString("yyyy-MM");
        }

        protected void btnAuditBatch_Click(object sender, EventArgs e)
        {
            int[] selections = GridList.SelectedRowIndexArray;
            bool isflag = true;
            if (selections.Length == 0)
            {
                Alert.Show("请选择要操作的单据！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            List<CommandInfo> cmdList = new List<CommandInfo>();
            for (int i = 0; i < selections.Length; i++)
            {
                int rowNum = GridList.SelectedRowIndexArray[i];
                Alert.Show(GridList.DataKeys[rowNum][3].ToString());
                if (GridList.DataKeys[rowNum][3].ToString().Equals("已审批"))
                {
                    isflag = false;
                }
                else
                {
                    cmdList.Add(new CommandInfo(string.Format(@"update dat_ys_doc set flag='S',SHR='{0}',SHRQ=SYSDATE where seqno='{1}'", UserAction.UserID, GridList.DataKeys[rowNum][2].ToString()), null));
                }
            }
            if (isflag)
            {
                if (DbHelperOra.ExecuteSqlTran(cmdList))
                {
                    Alert.Show("预算单据审批成功！", "操作提示", MessageBoxIcon.Information);
                    dataSearch();
                }
                else
                {
                    Alert.Show("预算单据审批失败，请检查原因！", "操作提示", MessageBoxIcon.Information);
                    dataSearch();
                }
            }
            else
            {
                Alert.Show("您不能选择【审批】状态的单据，执行撤回操作！");
            }
        }

        private void dataSearch()
        {

            if (ksyf.Text.Trim() == "" || ksyf.Text.Length != 7)
            {
                Alert.Show("请选择有效的开始时间！");
                return;
            }
            if (jsyf.Text.Trim() == "" || jsyf.Text.Length != 7)
            {
                Alert.Show("请选择有效的结束时间！");
                return;
            }

            //if (lstBEGRQ.SelectedDate == null || lstENDRQ.SelectedDate == null)
            //{
            //    Alert.Show("请输入条件【申领日期】！");
            //    return;
            //}
            if (string.Compare(ksyf.Text, jsyf.Text) == 1)
            {
                Alert.Show("开始日期大于结束日期，请重新输入！");
                return;
            }
            DateTime lstBEGRQ = DateTime.Parse(ksyf.Text + "-01");
            DateTime lstENDRQ = DateTime.Parse(jsyf.Text + "-01").AddMonths(1).AddDays(-1);

            string strSql = "";
            if (hfdoper.Text.Equals("input"))
            {
                strSql = @"SELECT decode(t.flag,'M','新单','N','已提交','S','已审批','R','已驳回') flag,t.BILLNO,t.BILLTYPE,F_GETDEPTNAME(t.DEPTID) DEPTID,t.YSRANGE,t.BEGRQ,t.ENDRQ,t.YSRQ,t.SUBNUM,t.SUBSUM,F_GETUSERNAME(t.YSY) YSY,F_GETUSERNAME(t.LRY) LRY,t.LRRQ,F_GETUSERNAME(t.SPR) SPR,t.SPRQ,t.MEMO FROM DAT_YS_DOC t where t.flag IN ('M','N','R','S')";
            }
            if (hfdoper.Text.Equals("audit"))
            {
                strSql = @"SELECT decode(t.flag,'M','新单','N','已提交','S','已审批','R','已驳回') flag,t.BILLNO,t.BILLTYPE,F_GETDEPTNAME(t.DEPTID) DEPTID,t.YSRANGE,t.BEGRQ,t.ENDRQ,t.YSRQ,t.SUBNUM,t.SUBSUM,F_GETUSERNAME(t.YSY) YSY,F_GETUSERNAME(t.LRY) LRY,t.LRRQ,F_GETUSERNAME(t.SPR) SPR,t.SPRQ,t.MEMO FROM DAT_YS_DOC t where t.flag in ('N','S','R')";
            }

            string strSearch = "";

            if (lstBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND t.BILLNO  LIKE '%{0}%'", lstBILLNO.Text);
            }
            if (lstFLAG.SelectedItem != null && lstFLAG.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND t.FLAG='{0}'", lstFLAG.SelectedItem.Value);
            }
            if (lstDEPTID.SelectedValue != null && lstDEPTID.SelectedValue.Length > 0)
            {
                strSearch += string.Format(" AND t.DEPTID='{0}'", lstDEPTID.SelectedValue);
            }
            strSearch += string.Format(" AND t.YSRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstBEGRQ.ToString("yyyy-MM-dd"));
            strSearch += string.Format(" AND t.YSRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstENDRQ.ToString("yyyy-MM-dd"));

            strSearch += string.Format(" AND t.deptid in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY t.LRRQ DESC";
            int total = 0;
            DataTable dt = PubFunc.DbGetPage(GridList.PageIndex, GridList.PageSize, strSql, ref total);
            GridList.DataSource = dt;
            GridList.RecordCount = total;
            GridList.DataBind();
        }


        protected void btnMySerarch_Click(object sender, EventArgs e)
        {
            dataSearch();
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {

            PubFunc.FormDataClear(FormDoc);
            // 20150919 modify by zhanghaicheng
            //20150921 6448版本缺少　　add by zhanghaicheng
            docLRY.SelectedValue = UserAction.UserID;
            //docSHR.SelectedValue = UserAction.UserID;

            docLRRQ.SelectedDate = DateTime.Now;
            docSHRQ.SelectedDate = null;
            nbxYSSL.Text = "1";
            docFLAG.SelectedValue = "M";

            // docYSRQ.Text = DateTime.Now.ToString("yyyy") + "-" + ddlMonth.SelectedValue + "-" + "01";

            //  docYSRQ.Text = Convert.ToDateTime(docYSRQ.Text).AddMonths(1).ToString("YYYY-MM");

            docYSRQ.Text = DateTime.Now.AddMonths(1).ToString("yyyy-MM");

            //20150919 modify by zhanghaicheng
            // dpkRQSJ1.Enabled = true;
            // dpkRQSJ2.Enabled = true;
            docMEMO.Enabled = true;
            btnGenerate.Enabled = true;
            btnSave.Enabled = false;
            btnSumbit.Enabled = false;
            btnAudit.Enabled = false;
            btnCancel.Enabled = false;
            btnPrint.Enabled = false;
            btnExport.Enabled = false;
            btnDelRow.Enabled = true;


            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("YSSL", "0");
            summary.Add("HSJE", "0");
            GridGoods.SummaryData = summary;
            //20150919 modify by zhanghaicheng
            //docYSRQ.Text = DateTime.Now.ToString("yyyy") + "-" + ddlMonth.SelectedValue.ToString();
            //Convert.ToDateTime(dpkRQSJ2.Text).Year.ToString() + "-" + Convert.ToDateTime(dpkRQSJ2.Text).AddMonths(1).Month.ToString();
            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
        }

        protected override void billNew()
        {
            PubFunc.FormDataClear(FormDoc);
            // 20150919 modify by zhanghaicheng

            // dpkRQSJ1.SelectedDate = DateTime.Now.AddDays(-30);
            // dpkRQSJ2.SelectedDate = DateTime.Now;
            docLRY.SelectedValue = UserAction.UserID;
            //docSHR.SelectedValue = UserAction.UserID;
            docLRRQ.SelectedDate = DateTime.Now;
            //docSHRQ.SelectedDate = DateTime.Now;
            nbxYSSL.Text = "1";
            docFLAG.SelectedValue = "M"; ;
            //20150919 modify by zhanghaicheng
            // dpkRQSJ1.Enabled = true;
            // dpkRQSJ2.Enabled = true;
            ddlDEPTID.Enabled = true;
            nbxYSSL.Enabled = true;
            docMEMO.Enabled = true;
            btnGenerate.Enabled = true;
            btnSave.Enabled = false;
            btnSumbit.Enabled = false;
            btnAudit.Enabled = false;
            btnCancel.Enabled = false;
            btnPrint.Enabled = false;
            btnExport.Enabled = false;
            btnDelRow.Enabled = false;

            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("YSSL", "0");
            summary.Add("HSJE", "0");
            GridGoods.SummaryData = summary;
            //20150919 modify by zhanghaicheng
            docYSRQ.Text = DateTime.Now.AddMonths(1).ToString("yyyy-MM");
            //Convert.ToDateTime(dpkRQSJ2.Text).Year.ToString() + "-" + Convert.ToDateTime(dpkRQSJ2.Text).AddMonths(1).Month.ToString();
            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
        }
        protected void btnSureDate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ddlDeptYs.SelectedValue))
            {
                Alert.Show("请选择科室生成月度预算！", "操作提示", MessageBoxIcon.Information);
                return;
            }
            if (dpStartDate.SelectedDate == null || Convert.ToDateTime(dpStartDate.SelectedDate).ToString("yyyy-MM-dd") == "0001-01-01")
            {

                Alert.Show("请选择正确开始日期！", "操作提示", MessageBoxIcon.Information);
                return;
            }
            if (dpEndDate.SelectedDate == null)
            {

                Alert.Show("请选择正确结束日期！", "操作提示", MessageBoxIcon.Information);
                return;

            }
            if (Convert.ToDateTime(dpEndDate.SelectedDate) < Convert.ToDateTime(dpStartDate.SelectedDate))
            {
                Alert.Show("开始日期不能大于结束日期！", "操作提示", MessageBoxIcon.Information);
                return;

            }


            string sql = string.Format(@"select * from dat_ys_doc where TO_CHAR(YSRQ,'YYYY-MM')='{0}' and deptid='{1}'", docYSRQ.Text, ddlDeptYs.SelectedValue);
            DataTable dt = DbHelperOra.Query(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                Alert.Show("已经有该月份的月度预算，无法生成，请生成其他月份的月度预算！", "操作提示", MessageBoxIcon.Information);
                return;
            }
            else
            {
                winDataSelectDate.Hidden = false;

            }
            ddlDEPTID.SelectedValue = ddlDeptYs.SelectedValue;
            nbxYSSL.Text = nbxTz.Text;
            startDate = Convert.ToDateTime(dpStartDate.SelectedDate);
            endDate = Convert.ToDateTime(dpEndDate.SelectedDate);
            billSearch();
            btnSave.Enabled = true;
            btnDelRow.Enabled = true;
            winDataSelectDate.Hidden = true;

        }
        protected override void billSearch()
        {

            // modify by zhanghaicheng  201509191553
            /*  if (string.IsNullOrWhiteSpace(dpkRQSJ1.SelectedDate.ToString()) && string.IsNullOrWhiteSpace(dpkRQSJ1.SelectedDate.ToString()))
              {
                  Alert.Show("【开始日期】和【结束日期】都不得为空", "操作提示", MessageBoxIcon.Information);
                  return;
              }
              else if (dpkRQSJ1.SelectedDate > dpkRQSJ2.SelectedDate)
              {
                  Alert.Show("【开始日期】大于【结束日期】，请重新输入！", "操作提示", MessageBoxIcon.Information);
                  return;
              }
              if (string.IsNullOrWhiteSpace(docYSRQ.Text))
              {
                  Alert.Show("【预算月份】不得为空！", "操作提示", MessageBoxIcon.Information);
                  return;
              }
              if (Convert.ToDateTime(docYSRQ.Text) < dpkRQSJ2.SelectedDate)
              {
                  Alert.Show("【预算月份】不能小于历史【结束月份】！！！", "操作月份", MessageBoxIcon.Information);
              }
              */
            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());

            /*   string ACCOUNTDAY = Doc.DbGetSysPara("ACCOUNTDAY");
              DateTime startDate = new DateTime();
              DateTime endDate = new DateTime();
              if (ACCOUNTDAY == "31")
              {
                  startDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy") + "-" + ddlMonth.SelectedValue.ToString() + "-01");
                  endDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy") + "-" + ddlMonth.SelectedValue.ToString() + "-01").AddMonths(1);
              }
              else
              {
                  startDate = Convert.ToDateTime(DateTime.Now.AddMonths(-1).ToString("yyyy") + "-" + ddlMonth.SelectedValue.ToString() + "-" + ACCOUNTDAY);
                  endDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy") + "-" + ddlMonth.SelectedValue.ToString() + "-" + ACCOUNTDAY);

              }*/

            //string strSql = @"select a.fd YSSL,a.sl XHSL,b.GDSEQ,b.HSJJ*a.fd HSJE,decode(b.FLAG,'Y','正常','T','停购','N','新品','E','淘汰') FLAG, b.GDNAME,b.GDSPEC,f_getunitname(b.UNIT) UNIT,b.CATID,b.PIZNO,F_GETPRODUCERNAME(b.PRODUCER) PRODUCER,b.HSJJ,b.HISCODE,b.HISNAME from (select t.gdseq, ";

            Int32 dayDiff = new TimeSpan(Convert.ToDateTime(dpEndDate.SelectedDate).Ticks - Convert.ToDateTime(dpStartDate.SelectedDate).Ticks).Days + 1;


            string strSql = string.Format(@"SELECT B.GDSEQ,
                               B.GDNAME,
                               B.GDSPEC,
                               CEIL(NVL(A.FD, 0)) YSSL,
                               NVL(A.SL, 0) XHSL,
                               B.UNIT,
                               F_GETUNITNAME(B.UNIT) UNITNAME,
                               B.HSJJ,
                               B.HSJJ * CEIL(NVL(A.FD, 0)) HSJE,
                               B.PIZNO,
                               B.PRODUCER,
                               F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                               B.HISCODE,
                               B.HISNAME", dayDiff.ToString());
            string strSearch = "";
            strSearch += string.Format(@"  FROM (SELECT T.GDSEQ, ABS(SUM(T.SL)) SL,  CEIL(ABS(SUM(T.SL))*30 /{0}) * {1} FD FROM DAT_GOODSJXC T WHERE T.KCADD='-1'", dayDiff.ToString(), Convert.ToDecimal(nbxYSSL.Text));
            if (!string.IsNullOrWhiteSpace(ddlDEPTID.SelectedValue))
            {
                strSearch += string.Format(" AND T.DEPTID = '{0}' ", ddlDEPTID.SelectedValue);
            }
            strSearch += string.Format(" AND T.RQSJ>=TO_DATE('{0}','YYYY-MM-DD')", startDate.ToString("yyyy-MM-dd"));
            strSearch += string.Format(" AND T.RQSJ <TO_DATE('{0}','YYYY-MM-DD') + 1", endDate.ToString("yyyy-MM-dd"));
            strSearch += string.Format(@" GROUP BY T.GDSEQ) A,DOC_GOODS B,DOC_GOODSCFG C WHERE B.FLAG = 'Y'  AND B.GDSEQ = C.GDSEQ AND C.GDSEQ = A.GDSEQ(+)");
            strSearch += string.Format(" AND C.DEPTID = '{0}' ", ddlDEPTID.SelectedValue);
            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            //int total = 0;
            DataTable billDT = new DataTable();
            billDT = DbHelperOra.Query(strSql).Tables[0];
            // GridGoods.DataBind();

            // billDT = GetDataTable(GridGoods.PageIndex, GridGoods.PageSize, strSql, ref total);
            Doc.GridRowAdd(GridGoods, billDT);

        }

        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            winDataSelectDate.Hidden = false;
            btnGenerate.Enabled = false;
            ddlDEPTID.Enabled = false;
        }

        protected void GridGoods_AfterEdit(object sender, FineUIPro.GridAfterEditEventArgs e)
        {
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
            if (newDict.Count == 0) return;
            if (e.ColumnID == "YSSL")
            {
                if (!PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "YSSL")))
                {
                    Alert.Show("商品信息异常，请详细检查预算值！", "异常信息", MessageBoxIcon.Warning);
                    return;
                }
                JObject defaultObj = Doc.GetJObject(GridGoods, e.RowID);
                decimal rs = 0, jg = 0;
                decimal.TryParse((defaultObj["YSSL"] ?? "0").ToString(), out rs);
                decimal.TryParse((defaultObj["HSJJ"] ?? "0").ToString(), out jg);
                defaultObj["HSJE"] = Math.Round(rs * jg, 2).ToString("F2");
                PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(e.RowID, defaultObj));
                //计算合计数量
                decimal bzslTotal = 0, feeTotal = 0;
                foreach (Dictionary<string, object> dic in newDict)
                {
                    if ((dic["YSSL"] ?? "0").ToString().Length > 0 && (dic["HSJJ"] ?? "0").ToString().Length > 0)
                    {
                        bzslTotal += Convert.ToDecimal(dic["YSSL"] ?? "0");
                        feeTotal += Convert.ToDecimal(dic["HSJJ"] ?? "0") * Convert.ToDecimal(dic["YSSL"] ?? "0");
                    }
                }
                JObject summary = new JObject();
                summary.Add("GDNAME", "本页合计");
                summary.Add("YSSL", bzslTotal.ToString());
                summary.Add("HSJE", feeTotal.ToString("F2"));
                GridGoods.SummaryData = summary;
            }
        }


        protected override void billSave()
        {
            save();
        }

        private void save(string flag = "N")
        {
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().ToList();
            if (newDict.Count == 0)
            {
                Alert.Show("没有生成任何数据！", "消息提示", MessageBoxIcon.Information);
                return;
            }
            int rowIndex = 0;
            foreach (Dictionary<string, object> dic in newDict)
            {
                rowIndex++;
                if ((dic["YSSL"] ?? "0").ToString() == "0")
                {
                    PageContext.RegisterStartupScript(Confirm.GetShowReference("第【" + rowIndex + "】行预算数量为空或0，确认执行操作？", "操作提示", MessageBoxIcon.Question, PageManager1.GetCustomEventReference("Confirm_OK"), PageManager1.GetCustomEventReference("Confirm_Cancel")));
                    return;
                }
            }

            DataSave(flag);
            billSearch();
            //SaveSuccess = true;
        }

        private bool saveTest(string flag = "N")
        {
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().ToList();
            if (newDict.Count == 0)
            {
                Alert.Show("没有生成任何数据！", "消息提示", MessageBoxIcon.Information);
                return false;
            }
            int rowIndex = 0;
            foreach (Dictionary<string, object> dic in newDict)
            {
                rowIndex++;
                if ((dic["YSSL"] ?? "0").ToString() == "0")
                {
                    PageContext.RegisterStartupScript(Confirm.GetShowReference("第【" + rowIndex + "】行预算数量为空或0，确认执行操作？", "操作提示", MessageBoxIcon.Question, PageManager1.GetCustomEventReference("Confirm_OKSumbit"), PageManager1.GetCustomEventReference("Confirm_Cancel")));
                    return false;
                }
            }
            DataSave(flag);
            billSearch();
            return true;
            //SaveSuccess = true;
        }

        private void DataSave(string flag)
        {
            List<Dictionary<string, object>> goodsData = GridGoods.GetNewAddedList().ToList();
            if (goodsData.Count == 0)//所有Gird行都为空行时
            {
                Alert.Show("没有任何数据！", "消息提示", MessageBoxIcon.Information);
                return;
            }

            if (!string.IsNullOrEmpty(docYSRQ.Text) && Convert.ToDateTime(docYSRQ.Text + "-01") < Convert.ToDateTime(DateTime.Now.AddMonths(1).ToString("yyyy-MM") + "-01"))
            {
                Alert.Show("当前时间只能做下一个月预算！", "消息提示", MessageBoxIcon.Information);
                return;

            }


            if (string.IsNullOrWhiteSpace(docSEQNO.Text))
            {
                if (PubFunc.StrIsEmpty(docBILLNO.Text))
                {
                    docSEQNO.Text = BillSeqGet();
                    docBILLNO.Text = docSEQNO.Text;
                    docSEQNO.Enabled = false;
                }

                decimal bzslTotal = 0, feeTotal = 0;

                bzslTotal = goodsData.Count();
                foreach (Dictionary<string, object> dic in goodsData)
                {
                    if ((dic["HSJJ"] ?? "0").ToString().Length > 0)
                    {

                        feeTotal += Convert.ToDecimal(dic["HSJJ"] ?? "0") * Convert.ToDecimal(dic["YSSL"] ?? "0");
                    }
                }


                MyTable mtType = new MyTable("DAT_YS_DOC");
                mtType.ColRow["SEQNO"] = docBILLNO.Text;
                mtType.ColRow["BILLNO"] = docBILLNO.Text;
                mtType.ColRow["FLAG"] = "M";
                mtType.ColRow["BILLTYPE"] = "YSD";  //单据类别为预算单
                mtType.ColRow["DEPTID"] = ddlDEPTID.SelectedValue;
                mtType.ColRow["YSRANGE"] = Convert.ToDecimal(nbxYSSL.Text);//插入预算调整幅度
                mtType.ColRow["BEGRQ"] = dpStartDate.SelectedDate;
                mtType.ColRow["ENDRQ"] = dpEndDate.SelectedDate;
                mtType.ColRow["YSRQ"] = docYSRQ.Text;
                mtType.ColRow["SUBNUM"] = bzslTotal.ToString();
                mtType.ColRow["SUBSUM"] = feeTotal.ToString("F2");
                mtType.ColRow["YSY"] = UserAction.UserID;
                mtType.ColRow["LRY"] = UserAction.UserID;
                mtType.ColRow["LRRQ"] = DateTime.Now;
                mtType.ColRow["SPR"] = UserAction.UserID;
                mtType.ColRow["SPRQ"] = DateTime.Now;
                mtType.ColRow["SHR"] = "";
                mtType.ColRow["SHRQ"] = "";
                mtType.ColRow["MEMO"] = docMEMO.Text;

                List<CommandInfo> cmdList = new List<CommandInfo>();
                MyTable mtTypeMx = new MyTable("DAT_YS_COM");
                cmdList.Add(new CommandInfo("DELETE DAT_YS_COM WHERE SEQNO='" + docBILLNO.Text + "'", null));//删除单据台头
                cmdList.Add(new CommandInfo("DELETE DAT_YS_COM WHERE SEQNO='" + docBILLNO.Text + "'", null));//删除单据明细
                cmdList.AddRange(mtType.InsertCommand());

                for (int i = 0; i < goodsData.Count; i++)
                {
                    mtTypeMx.ColRow = PubFunc.GridDataGet(goodsData[i]);
                    mtTypeMx.ColRow.Add("SEQNO", docBILLNO.Text);
                    mtTypeMx.ColRow.Add("ROWNO", i + 1);
                    mtTypeMx.ColRow.Add("BZHL", 0);
                    mtTypeMx.ColRow.Remove("HSJE");
                    mtTypeMx.ColRow.Add("HSJE", Convert.ToDecimal(goodsData[i]["HSJJ"] ?? "0") * Convert.ToDecimal(goodsData[i]["YSSL"] ?? "0"));
                    cmdList.Add(mtTypeMx.Insert());
                }


                if (DbHelperOra.ExecuteSqlTran(cmdList))
                {
                    Alert.Show("预算单生成成功！!", "消息提示", MessageBoxIcon.Information);
                    billOpen(docBILLNO.Text);
                    OperLog("科室预算", "生成单据【" + docBILLNO.Text + "】");
                    btnSave.Enabled = true;
                    btnSumbit.Enabled = true;
                    btnPrint.Enabled = false;
                    btnExport.Enabled = false;
                    //btnDelRow.Enabled = false;
                }
                else
                {
                    Alert.Show("预算单生成失败!", "消息提示", MessageBoxIcon.Information);
                    btnSave.Enabled = true;
                    return;
                }
            }
            else
            {
                int mycount = 0;
                int RowNum = goodsData.Count;
                Decimal CountNum = 0;
                for (int i = 0; i < goodsData.Count; i++)
                {
                    string ss = goodsData[i]["YSSL"].ToString();
                    string hsje = goodsData[i]["HSJE"].ToString();
                    CountNum += Convert.ToDecimal(goodsData[i]["HSJE"] ?? 0);
                    string comsql = string.Format("update dat_ys_COM com set YSSL={0},HSJE={1} where com.seqno='{2}' and com.rowno={3}", Convert.ToDecimal(ss), Convert.ToDecimal(hsje), docSEQNO.Text, i + 1);
                    mycount = DbHelperOra.ExecuteSql(comsql);
                }
                string mysql = string.Format("update dat_ys_doc set DEPTID='{0}',YSRANGE={1},memo='{2}' ,SubNum={3},SubSum={4} where seqno='{5}'", ddlDEPTID.SelectedValue, Convert.ToDecimal(nbxYSSL.Text), docMEMO.Text, RowNum, CountNum, docSEQNO.Text);
                int count = DbHelperOra.ExecuteSql(mysql);
                if (count > 0 && mycount > 0)
                {
                    if (flag == "N")
                        Alert.Show("预算单修改成功!", "消息提示", MessageBoxIcon.Information);
                    OperLog("科室预算", "修改单据【" + docBILLNO.Text + "】");
                    billOpen(docBILLNO.Text);
                    btnSave.Enabled = true;
                    btnSumbit.Enabled = true;
                    btnPrint.Enabled = false;
                    btnExport.Enabled = false;
                    btnDelRow.Enabled = false;
                }
                else
                {
                    Alert.Show("预算单修改失败，请检查原因!", "消息提示", MessageBoxIcon.Information);
                    btnSave.Enabled = true;
                    hfdFlag.Text = "false";
                }
            }
        }
        //private bool SaveSuccess = false;
        protected void btnSumbit_Click(object sender, EventArgs e)
        {
            //SaveSuccess = false;
            btnSumbit.Enabled = false;
            if (hfdFlag.Text != "true")
            {
                if (hfdFlag.Text != "false")
                {
                    if (!saveTest("Y"))
                    {
                        return;
                    }
                }
                return;

            }
            hfdFlag.Text = "";
            //if (SaveSuccess == false)
            //    return;
            //SaveSuccess = false;
            if (docFLAG.SelectedValue.Equals("R"))
            {
                docMEMO.Text = "";
                int count = DbHelperOra.ExecuteSql(string.Format("update DAT_YS_DOC set flag='N',memo='' where seqno='{0}' and flag='R'", docSEQNO.Text));
                if (count > 0)
                {
                    Alert.Show("已重新提交！!", "消息提示", MessageBoxIcon.Information);
                    OperLog("科室预算", "提交单据【" + docSEQNO.Text + "】");
                    billOpen(docSEQNO.Text);
                    btnSave.Enabled = false;
                    btnSumbit.Enabled = false;
                    btnAudit.Enabled = true;
                    btnCancel.Enabled = true;
                    btnGenerate.Enabled = false;
                }
                else
                {
                    Alert.Show("提交失败！", "操作提示", MessageBoxIcon.Information);
                    btnSumbit.Enabled = true;
                }
            }
            else
            {
                string sql = string.Format("update DAT_YS_DOC set flag='N' where seqno='{0}' and flag='M'", docSEQNO.Text);
                int count = DbHelperOra.ExecuteSql(sql);
                if (count > 0)
                {
                    Alert.Show("预算单提交成功！!", "消息提示", MessageBoxIcon.Information);
                    OperLog("科室预算", "提交单据【" + docBILLNO.Text + "】");
                    billOpen(docBILLNO.Text);
                    btnSave.Enabled = false;
                    btnSumbit.Enabled = false;
                    btnAudit.Enabled = true;
                    btnCancel.Enabled = true;
                    btnGenerate.Enabled = false;
                }
                else
                {
                    Alert.Show("预算单提交失败！", "操作提示", MessageBoxIcon.Information);
                    btnSumbit.Enabled = true;
                }
            }
        }

        protected override void billOpen(string strBillno)
        {
            DataTable dtDoc = DbHelperOra.Query(string.Format(strDocSql, strBillno)).Tables[0];
            if (dtDoc != null && dtDoc.Rows.Count > 0)
            {
                PubFunc.FormDataSet(FormDoc, dtDoc.Rows[0]);
            }
            else
            {
                Alert.Show("单据信息获取失败！！！", "警告提示", MessageBoxIcon.Warning);
                return;
            }

            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            DataTable dtBill = DbHelperOra.Query(string.Format(strComSql, strBillno)).Tables[0];
            decimal bzslTotal = 0, feeTotal = 0;
            if (dtBill != null && dtBill.Rows.Count > 0)
            {
                foreach (DataRow row in dtBill.Rows)
                {
                    bzslTotal += Convert.ToDecimal(row["YSSL"] ?? "0");
                    feeTotal += Convert.ToDecimal(row["YSSL"] ?? "0") * Convert.ToDecimal(row["HSJJ"] ?? "0");
                }
                Doc.GridRowAdd(GridGoods, dtBill);
            }

            //增加合计
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("YSSL", bzslTotal.ToString());
            summary.Add("HSJE", feeTotal.ToString("F2"));
            GridGoods.SummaryData = summary;
            PubFunc.FormLock(FormDoc, true, "");
            TabStrip1.ActiveTabIndex = 1;
            //按钮状态转换
            if (docFLAG.SelectedValue == "N")
            {
                //提交
                btnNew.Enabled = true;
                btnGenerate.Enabled = false;
                btnSave.Enabled = false;
                btnSumbit.Enabled = false;
                btnAudit.Enabled = true;
                btnCancel.Enabled = true;
                btnPrint.Enabled = true;
                btnExport.Enabled = true;
                btnDelRow.Enabled = false;
            }
            else if (docFLAG.SelectedValue.Equals("R"))
            {
                btnSave.Enabled = true;
                btnSumbit.Enabled = true;
                btnDelRow.Enabled = true;
                btnGenerate.Enabled = false;
                //   dpkRQSJ1.Enabled = true;
                //  dpkRQSJ2.Enabled = true;
                // ddlDEPTID.Enabled = true;
                // nbxYSSL.Enabled = true;
                docMEMO.Enabled = true;
            }
            else if (docFLAG.SelectedValue.Equals("M"))
            {
                btnSave.Enabled = true;
                btnSumbit.Enabled = true;
                btnDelRow.Enabled = true;
                btnGenerate.Enabled = false;
                btnExport.Enabled = false;
                btnPrint.Enabled = false;
                btnCancel.Enabled = false;
                btnAudit.Enabled = false;
                //   dpkRQSJ1.Enabled = true;
                //  dpkRQSJ2.Enabled = true;
                //   ddlDEPTID.Enabled = true;
                //  nbxYSSL.Enabled = true;
                docMEMO.Enabled = true;
            }
            else
            {
                //审核
                btnNew.Enabled = true;
                btnGenerate.Enabled = false;
                btnSave.Enabled = false;
                btnSumbit.Enabled = false;
                btnAudit.Enabled = false;
                btnPrint.Enabled = true;
                btnExport.Enabled = true;
                btnDelRow.Enabled = false;
            }
        }

        protected override void listRow_DoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            billOpen(GridList.Rows[e.RowIndex].Values[2].ToString());
        }

        protected override void billAudit()
        {
            string upsql = string.Format(@"update dat_ys_doc set flag='S',SHR='{0}',SHRQ=SYSDATE WHERE seqno='{1}'", UserAction.UserID, docSEQNO.Text);
            int count = DbHelperOra.ExecuteSql(upsql);
            if (count > 0)
            {
                Alert.Show("审批成功！", "操作提示", MessageBoxIcon.Information);
                btnCancel.Enabled = false;
                btnAudit.Enabled = false;
                btnPrint.Enabled = true;
                btnExport.Enabled = true;
                billOpen(docBILLNO.Text);
            }
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            string sql = string.Format("update dat_ys_com set dyy='{0}' and dyrq='{1}'", UserAction.UserID, DateTime.Now);
            DbHelperOra.ExecuteSql(sql);
        }

        private bool CheckFlagForM()
        {
            if (docBILLNO.Text.Length > 0)
            {
                return Doc.getFlag(docBILLNO.Text, "M", BillType);
            }
            return true;
        }

        protected void btnDelRow_Click(object sender, EventArgs e)
        {
            //201509211429 6448版本缺失　add by zhanghaicheng
            if (docFLAG.SelectedValue != "M")
            {
                Alert.Show("非『新增』单据不能删行！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (CheckFlagForM() && docBILLNO.Text.Length > 0)
            {
                Alert.Show("此单据已经被别人操作，请等待操作!");
                return;
            }
            if (GridGoods.SelectedCell == null)
            {
                Alert.Show("当前未选中单元行，无法进行操作!", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            GridGoods.DeleteSelectedRows();

            PubFunc.FormLock(FormDoc, true, "");
            docMEMO.Enabled = true;
        }
        protected void btnExport_Click(object sender, EventArgs e)
        {
            if (GridList == null)
            {
                Alert.Show("没有数据,无法导出！");
                return;
            }
            string sql = @"SELECT a.SEQNO 单据编号,
                            a.ROWNO 行号,
                            ''''||a.GDSEQ 商品编码,
                            a.GDNAME 商品名称,
                            a.XHSL 历史消耗,
                            a.YSSL 预算数量,
                            a.GDSPEC 商品规格,
                            f_getunitname(a.UNIT) 商品单位,
                            a.HSJJ 含税进价,
                            a.CATID 商品种类,
                            f_getproducername(a.PRODUCER) 生产商
                            --,a.PIZNO 注册证号
                            --,a.HISCODE HIS编码
                            --,a.HISNAME HIS名称 
                            FROM DAT_YS_COM a,DAT_YS_DOC b WHERE a.seqno = b.seqno";
            string strSearch = "";
            //strSearch += string.Format("  AND TO_CHAR(b.YSRQ,'YYYY-MM-DD')='{0}'", Convert.ToDateTime(docYSRQ.Text).ToString("yyyy-MM-dd"));
            strSearch += string.Format(" AND A.SEQNO = '{0}'", docBILLNO.Text);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                sql += strSearch;
            }

            DataTable dt = DbHelperOra.Query(sql).Tables[0];

            ExcelHelper.ExportByWeb(dt, "预算单信息", "预算单导出_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
        }

        protected void dpkRQSJ2_DateSelect(object sender, EventArgs e)
        {
            docYSRQ.Text = DateTime.Now.AddMonths(1).ToString("yyyy-MM");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (docBILLNO.Text.Length < 1)
            {
                Alert.Show("请选择需要驳回的单据!", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (docFLAG.SelectedValue.ToString() != "N")
            {
                Alert.Show("已审批的单据不能驳回！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            WindowReject.Hidden = false;
        }

        protected void btnRejectSubmit_Click(object sender, EventArgs e)
        {
            if (ddlReject.SelectedText == "--请选择--")
            {
                Alert.Show("请选择驳回原因");
                return;
            }
            if (txaMemo.Text.Length > 30)
            {
                Alert.Show("驳回原因超长！", MessageBoxIcon.Warning);
                return;
            }
            PubFunc.FormDataCheck(Form2);

            //增加待办事宜
            List<CommandInfo> cmdList = new List<CommandInfo>();

            string strMemo = docMEMO.Text + ";驳回原因:" + ddlReject.SelectedText;
            if (!string.IsNullOrWhiteSpace(txaMemo.Text.Trim()))
            {
                strMemo += ";详细说明;" + txaMemo.Text;
            }
            cmdList.Add(new CommandInfo(string.Format("update DAT_YS_DOC SET FLAG='R',MEMO='{0}',SHR={1},SHRQ=SYSDATE WHERE SEQNO='{2}' AND FLAG='N'", strMemo, "'" + UserAction.UserID + "'", docBILLNO.Text), null));
            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                WindowReject.Hidden = true;
                billOpen(docBILLNO.Text);
                Alert.Show("驳回成功");
                btnAudit.Enabled = false;
                txaMemo.Text = "";
            }
        }

        protected void GridList_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridList.PageIndex = e.NewPageIndex;
            dataSearch();
        }

        protected void PageManager1_CustomEvent(object sender, CustomEventArgs e)
        {
            if (e.EventArgument == "Confirm_OK")
            {
                btnSave.Enabled = false;
                DataSave("N");
            }
            if (e.EventArgument == "Confirm_OKSumbit")
            {
                DataSave("Y");
                hfdFlag.Text = "true";
                btnSumbit_Click(null, null);
            }

        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            //201509211433 6448版本缺失　　add by zhanghaicheng
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().ToList();
            if (newDict.Count == 0)
            {
                Alert.Show("没有生成任何数据！", "消息提示", MessageBoxIcon.Information);
                return;
            }
            int rowIndex = 0;
            foreach (Dictionary<string, object> dic in newDict)
            {
                rowIndex++;
                if ((dic["YSSL"] ?? "0").ToString() == "0")
                {
                    PageContext.RegisterStartupScript(Confirm.GetShowReference("第【" + rowIndex + "】行预算数量为空或0，确认执行操作？", "操作提示", MessageBoxIcon.Question, PageManager1.GetCustomEventReference("Confirm_OK"), PageManager1.GetCustomEventReference("Confirm_Cancel")));
                    return;
                }
            }
            btnSave.Enabled = false;
            DataSave("N");
        }

        protected void btnAudit_Click(object sender, EventArgs e)
        {
            //201509211435  6448版本缺失　add by zhanghaicheng
            string upsql = string.Format(@"update dat_ys_doc set flag='S',SHR='{0}',SHRQ=SYSDATE WHERE seqno='{1}'", UserAction.UserID, docSEQNO.Text);
            int count = DbHelperOra.ExecuteSql(upsql);
            if (count > 0)
            {
                Alert.Show("审批成功！", "操作提示", MessageBoxIcon.Information);
                btnCancel.Enabled = false;
                btnAudit.Enabled = false;
                btnPrint.Enabled = true;
                btnExport.Enabled = true;
                billOpen(docBILLNO.Text);
            }
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
                if (flag == "已提交")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color2";
                }
                if (flag == "已驳回")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color3";
                }
            }
        }



    }
}