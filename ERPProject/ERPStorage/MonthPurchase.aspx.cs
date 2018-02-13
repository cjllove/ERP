using FineUIPro;
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

namespace ERPProject.ERPStorage
{
    public partial class MonthPurchase : BillBase
    {
        private DateTime startDate = new DateTime();
        private DateTime endDate = new DateTime();
        private string strDocSql = "SELECT to_char(c.XDRQ,'YYYY-MM') YSRQ,SEQNO,BILLNO,BILLTYPE,FLAG,DEPTID,BEGINTIME,ENDTIME,SUBNUM,SUBSUM,LRY,LRRQ,SPR,SPRQ,SHR,SHRQ,MEMO FROM DAT_DDPLAN_DOC c WHERE SEQNO ='{0}' AND BILLTYPE ='CJD'";
        private string strComSql = "SELECT * FROM DAT_DDPLAN_COM c where c.seqno = '{0}'";

        public MonthPurchase()
        {
            BillType = "CJD";
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
                        ToolbarText1.Text = "操作信息:月度采购审批界面";
                        billLockDoc(true);
                        ButtonHidden(btnNew, btnGenerate, btnSave, btnSumbit, btnDelRow, btnDelRow);
                        hfdoper.Text = "audit";
                        TabStrip1.ActiveTabIndex = 0;
                        billSearch();
                    }
                }
            }

        }
        private void initData()
        {

            PubFunc.DdlDataGet("DDL_USER", docLRY, docSHR);

            PubFunc.DdlDataGet("DDL_SYS_DEPOT", UserAction.UserID, ddlDEPTID, lstDEPTID);
            PubFunc.DdlDataGet("DDL_DD_STATUSDD", lstFLAG, docFLAG);
            PubFunc.DdlDataGet("DDL_SYS_DEPOT", UserAction.UserID, ddlDeptYs);
            PubFunc.DdlDataGet(ddlReject, "DDL_BILL_REASON");
            docFLAG.SelectedValue = "M";   //默认是新单状态M（这个后续还要更改，先写成这样）

            docLRRQ.SelectedDate = DateTime.Now;
            docLRY.SelectedValue = UserAction.UserID;


            tbxJHYF.Text = DateTime.Now.AddMonths(1).ToString("yyyy-MM");
            ddlDEPTID.SelectedValue = UserAction.UserDept;
            ddlDeptYs.SelectedValue = UserAction.UserDept;
            //ddlDEPTID.Enabled = false;

            dpEndDate.SelectedDate = DateTime.Now.AddDays(-1);
            dpEndDate.Enabled = true;
            dpStartDate.SelectedDate = DateTime.Now.AddMonths(-2);
            docYSRQ.Text = DateTime.Now.AddMonths(1).ToString("yyyy-MM");
            docYSRQ.Enabled = false;
        }

        private void newData()
        {
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("DHS", "0");
            summary.Add("HSJE", "0");
            GridGoods.SummaryData = summary;
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            PubFunc.FormDataClear(Formlist);
            tbxJHYF.Text = DateTime.Now.AddMonths(1).ToString("yyyy-MM");
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
                    cmdList.Add(new CommandInfo(string.Format(@"update DAT_DDPLAN_DOC set flag='Y',SHR='{0}',SHRQ=SYSDATE where  FLAG='N' AND seqno='{1}' AND BILLTYPE='CJD'", UserAction.UserID, GridList.DataKeys[rowNum][2].ToString()), null));
                }
            }
            if (isflag)
            {
                if (DbHelperOra.ExecuteSqlTran(cmdList))
                {
                    Alert.Show("采购单据审批成功！", "操作提示", MessageBoxIcon.Information);
                    dataSearch();
                }
                else
                {
                    Alert.Show("采购单据审批失败，请检查原因！", "操作提示", MessageBoxIcon.Information);
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
            if (string.IsNullOrWhiteSpace(tbxJHYF.Text))
            {
                Alert.Show("【计划月份】不能为空，请选择！");
                return;
            }
            string strSql = "";
            if (hfdoper.Text.Equals("input"))
            {
                strSql = @"SELECT decode(t.flag,'M','新单','N','已提交','Y','已审核','R','已驳回') flag,t.BILLNO,t.BILLTYPE,F_GETDEPTNAME(t.DEPTID) DEPTID,t.BEGINTIME,t.ENDTIME,t.XDRQ,t.SUBNUM,t.SUBSUM,F_GETUSERNAME(t.CGY) CGY,F_GETUSERNAME(t.LRY) LRY,t.LRRQ,F_GETUSERNAME(t.SHR) SHR,t.SPRQ,t.MEMO FROM DAT_DDPLAN_DOC t where BILLTYPE='CJD' AND t.flag IN ('M','N','R','Y')";
            }
            if (hfdoper.Text.Equals("audit"))
            {
                strSql = @"SELECT decode(t.flag,'M','新单','N','已提交','Y','已审核','R','已驳回') flag,t.BILLNO,t.BILLTYPE,F_GETDEPTNAME(t.DEPTID) DEPTID,t.BEGINTIME,t.ENDTIME,t.XDRQ,t.SUBNUM,t.SUBSUM,F_GETUSERNAME(t.CGY) CGY,F_GETUSERNAME(t.LRY) LRY,t.LRRQ,F_GETUSERNAME(t.SHR) SHR,t.SHRQ,t.MEMO FROM DAT_DDPLAN_DOC t where BILLTYPE='CJD' AND t.flag in ('N','Y','R')";
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
            strSearch += string.Format(" AND TO_CHAR(t.XDRQ,'YYYY-MM')=TO_CHAR(TO_DATE('{0}','YYYY-MM-DD'),'YYYY-MM')", tbxJHYF.Text + "-01");
            // strSearch += string.Format(" AND t.XDRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstENDRQ.ToString("yyyy-MM-dd"));

            strSearch += string.Format(" AND t.deptid in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY decode(flag,'新单','1','已提交','2','已驳回','3','4'),t.XDRQ DESC";
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
            //// 20150919 modify by zhanghaicheng
            ////20150921 6448版本缺少　　add by zhanghaicheng
            //docLRY.SelectedValue = UserAction.UserID;
            ////docSHR.SelectedValue = UserAction.UserID;

            //docLRRQ.SelectedDate = DateTime.Now;
            //docSHRQ.SelectedDate = null;
            //nbxYSSL.Text = "1";
            //docFLAG.SelectedValue = "M";

            //// docYSRQ.Text = DateTime.Now.ToString("yyyy") + "-" + ddlMonth.SelectedValue + "-" + "01";

            ////  docYSRQ.Text = Convert.ToDateTime(docYSRQ.Text).AddMonths(1).ToString("YYYY-MM");

            //docYSRQ.Text = DateTime.Now.AddMonths(1).ToString("yyyy-MM");

            initData();

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
            btnDelRow.Enabled = false;


            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("DHS", "0");
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
            nbxYSSL.Text = "1";
            docFLAG.SelectedValue = "M"; ;
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
            summary.Add("DHS", "0");
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
                Alert.Show("请选择科室生成月度采购！", "操作提示", MessageBoxIcon.Information);
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


            string sql = string.Format(@"select * from DAT_DDPLAN_DOC where BILLTYPE='CJD' AND TO_CHAR(XDRQ,'YYYY-MM')='{0}' and deptid='{1}'", docYSRQ.Text, ddlDeptYs.SelectedValue);
            DataTable dt = DbHelperOra.Query(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                Alert.Show("已经有该月份的月度采购，无法生成，请生成其他月份的月度采购！", "操作提示", MessageBoxIcon.Information);
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
            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat(@"SELECT GD.GDNAME,GD.GDSEQ,GD.GDSPEC,GD.CATID,GD.SUPPLIER SUPID,
                                                                   F_GETUNITNAME(DECODE(GD.UNIT_ORDER, 'D', GD.UNIT_DABZ, 'Z', GD.UNIT_ZHONGBZ, GD.UNIT)) UNIT,
                                                                   GD.BZHL,NVL(GD.ISLOT,'2') ISLOT,GD.HSJJ,GD.PIZNO PZWH,
                                                                   F_GETPRODUCERNAME(GD.Producer) PRODUCER,
                                                                   NVL(GS.KCSL, 0) KCSL,
                                                                   NVL(CFG.ZGKC, 0) ZGKC,
                                                                   NVL(CFG.ZDKC, 0) ZDKC,
                                                                   DECODE(GD.UNIT_ORDER, 'D', GD.NUM_DABZ, 'Z', GD.NUM_ZHONGBZ, GD.BZHL) BZHL,
                                                                   F_GETZTKC('{3}', GD.GDSEQ) *
                                                                   DECODE(GD.UNIT_ORDER, 'D', GD.NUM_DABZ, 'Z', GD.NUM_ZHONGBZ, GD.BZHL) ZTSL,
                                                                   F_GETJYDH('{3}', GD.GDSEQ, 'DAY', '{1}', '{2}') AVGSL,
                                                                   F_GETJYDH('{3}', GD.GDSEQ, 'DS', '{1}', '{2}') *
                                                                   DECODE(GD.UNIT_ORDER, 'D', GD.NUM_DABZ, 'Z', GD.NUM_ZHONGBZ, GD.BZHL) ADVSL,
                                                                   F_GETJYDH('{3}', GD.GDSEQ, 'DS', '{1}', '{2}') *
                                                                   DECODE(GD.UNIT_ORDER, 'D', GD.NUM_DABZ, 'Z', GD.NUM_ZHONGBZ, GD.BZHL) * {0} DHS,
                                                                   (F_GETJYDH('{3}', GD.GDSEQ, 'DS', '{1}', '{2}') *
                                                                   DECODE(GD.UNIT_ORDER,
                                                                           'D',
                                                                           GD.NUM_DABZ,
                                                                           'Z',
                                                                           GD.NUM_ZHONGBZ,
                                                                           GD.BZHL)) * GD.HSJJ HSJE
                                                              FROM (SELECT DISTINCT GDSEQ
                                                                      FROM DAT_GOODSJXC A
                                                                     WHERE RQSJ BETWEEN TO_DATE('{1}', 'YYYY-MM-DD') AND
                                                                           TO_DATE('{2}', 'YYYY-MM-DD')
                                                                       AND EXISTS (SELECT 1
                                                                              FROM SYS_DEPT
                                                                             WHERE TYPE = '3'
                                                                               AND CODE = A.DEPTID)) JXC,
                                                                   (SELECT GDSEQ, SUM(KCSL) KCSL
                                                                      FROM DAT_GOODSSTOCK S
                                                                     WHERE EXISTS (SELECT 1
                                                                              FROM SYS_DEPT
                                                                             WHERE TYPE = '3'
                                                                               AND CODE = S.DEPTID)
                                                                     GROUP BY GDSEQ) GS,
                                                                   (SELECT GDNAME,
                                                                           GDSEQ,
                                                                           GDSPEC,
                                                                           UNIT,
                                                                           BZHL,
                                                                           PIZNO,
                                                                           HSJJ,ISLOT,
                                                                           PRODUCER,
                                                                           NUM_ZHONGBZ,
                                                                           NUM_DABZ,
                                                                           UNIT_ORDER,UNIT_ZHONGBZ, UNIT_DABZ, 
                                                                           F_GETSUPID(GDSEQ) SUPPLIER,CATID
                                                                      FROM DOC_GOODS
                                                                     WHERE ISGZ = 'N') GD,
                                                                   (SELECT ZGKC, ZDKC, GDSEQ FROM DOC_GOODSCFG WHERE DEPTID = '{3}') CFG
                                                             WHERE JXC.GDSEQ = GS.GDSEQ(+)
                                                               AND JXC.GDSEQ = GD.GDSEQ
                                                               AND JXC.GDSEQ = CFG.GDSEQ", Convert.ToDecimal(nbxYSSL.Text), startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"), UserAction.UserDept);

            DataTable billDT = new DataTable();
            billDT = DbHelperOra.Query(strSql.ToString()).Tables[0];
            Doc.GridRowAdd(GridGoods, billDT);
        }

        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            winDataSelectDate.Hidden = false;
            btnGenerate.Enabled = false;
            ddlDEPTID.Enabled = false;
        }

        private JObject GetJObject(Dictionary<string, object> dicRecord)
        {
            JObject defaultObj = new JObject();
            foreach (string key in dicRecord.Keys)
            {
                defaultObj.Add(key, (dicRecord[key] ?? "").ToString());
            }

            decimal rs = 0, jg = 0;
            decimal.TryParse((dicRecord["DHS"] ?? "0").ToString(), out rs);
            decimal.TryParse((dicRecord["HSJJ"] ?? "0").ToString(), out jg);

            defaultObj.Remove("HSJE");
            //处理金额格式
            string jingdu = Math.Round(rs * jg, 2).ToString("F2");
            defaultObj.Add("HSJE", jingdu);

            return defaultObj;
        }

        protected void GridGoods_AfterEdit(object sender, FineUIPro.GridAfterEditEventArgs e)
        {
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
            if (newDict.Count == 0) return;
            if (e.ColumnID == "DHS")
            {
                string[] strCell = GridGoods.SelectedCell;
                if (!PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "DHS")))
                {
                    Alert.Show("商品信息异常，请详细检查采购值！", "异常信息", MessageBoxIcon.Warning);
                    return;
                }
                JObject defaultObj = Doc.GetJObject(GridGoods, e.RowID);
                decimal hl = 0, jg = 0;
                decimal.TryParse((defaultObj["DHS"] ?? "0").ToString(), out hl);
                decimal.TryParse((defaultObj["HSJJ"] ?? "0").ToString(), out jg);
                defaultObj["HSJE"] = Math.Round(hl * jg, 2).ToString("F2");

                PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(e.RowID, defaultObj));
                //计算合计数量
                decimal bzslTotal = 0, feeTotal = 0;
                foreach (Dictionary<string, object> dic in newDict)
                {
                    if ((dic["DHS"] ?? "0").ToString().Length > 0 && (dic["HSJJ"] ?? "0").ToString().Length > 0)
                    {
                        bzslTotal += Convert.ToDecimal(dic["DHS"] ?? "0");
                        feeTotal += Convert.ToDecimal(dic["HSJJ"] ?? "0") * Convert.ToDecimal(dic["DHS"] ?? "0");
                    }
                }
                JObject summary = new JObject();
                summary.Add("GDNAME", "本页合计");
                summary.Add("DHS", bzslTotal.ToString());
                summary.Add("HSJE", feeTotal.ToString("F2"));
                GridGoods.SummaryData = summary;
            }
        }


        protected override void billSave()
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
                if ((dic["DHS"] ?? "0").ToString() == "0")
                {
                    PageContext.RegisterStartupScript(Confirm.GetShowReference("第【" + rowIndex + "】行采购数量为空或0，确认执行操作？", "操作提示", MessageBoxIcon.Question, PageManager1.GetCustomEventReference("Confirm_OK"), PageManager1.GetCustomEventReference("Confirm_Cancel")));
                    return;
                }
            }

            DataSave();
            billSearch();
        }

        private void DataSave()
        {

            List<Dictionary<string, object>> goodsData = GridGoods.GetNewAddedList().ToList();
            if (goodsData.Count == 0)//所有Gird行都为空行时
            {
                Alert.Show("没有任何数据！", "消息提示", MessageBoxIcon.Information);
                return;
            }

            if (!string.IsNullOrEmpty(docYSRQ.Text) && Convert.ToDateTime(docYSRQ.Text + "-01") < Convert.ToDateTime(DateTime.Now.AddMonths(1).ToString("yyyy-MM") + "-01"))
            {
                Alert.Show("当前时间只能做下一个月采购！", "消息提示", MessageBoxIcon.Information);
                return;

            }
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
                    feeTotal += Convert.ToDecimal(dic["HSJJ"] ?? "0") * Convert.ToDecimal(dic["DHS"] ?? "0");
                }
            }

            MyTable mtType = new MyTable("DAT_DDPLAN_DOC");
            mtType.ColRow["SEQNO"] = docBILLNO.Text;
            mtType.ColRow["BILLNO"] = docBILLNO.Text;
            mtType.ColRow["FLAG"] = "M";
            mtType.ColRow["BILLTYPE"] = "CJD";  //单据类别为采购单
            mtType.ColRow["DEPTID"] = ddlDEPTID.SelectedValue;
            mtType.ColRow["NUM1"] = Convert.ToDecimal(nbxYSSL.Text);//插入采购调整幅度
            mtType.ColRow["BEGINTIME"] = Convert.ToDateTime(dpStartDate.SelectedDate).ToString("yyyy-MM-dd");
            mtType.ColRow["ENDTIME"] = Convert.ToDateTime(dpEndDate.SelectedDate).ToString("yyyy-MM-dd"); //dpEndDate.SelectedDate;
            mtType.ColRow["XDRQ"] = docYSRQ.Text;
            mtType.ColRow["SUBNUM"] = bzslTotal.ToString();
            mtType.ColRow["SUBSUM"] = feeTotal.ToString("F2");
            mtType.ColRow["CGY"] = UserAction.UserID;  //采购员
            mtType.ColRow["LRY"] = UserAction.UserID;
            mtType.ColRow["LRRQ"] = DateTime.Now;
            mtType.ColRow["SPR"] = UserAction.UserID;
            mtType.ColRow["SPRQ"] = DateTime.Now;
            mtType.ColRow["SHR"] = "";
            mtType.ColRow["SHRQ"] = "";
            mtType.ColRow["MEMO"] = docMEMO.Text;

            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtTypeMx = new MyTable("DAT_DDPLAN_COM");
            cmdList.Add(new CommandInfo("DELETE DAT_DDPLAN_DOC WHERE SEQNO='" + docBILLNO.Text + "'", null));//删除单据台头
            cmdList.Add(new CommandInfo("DELETE DAT_DDPLAN_COM WHERE SEQNO='" + docBILLNO.Text + "'", null));//删除单据明细
            cmdList.AddRange(mtType.InsertCommand());

            for (int i = 0; i < goodsData.Count; i++)
            {
                mtTypeMx.ColRow = PubFunc.GridDataGet(goodsData[i]);
                mtTypeMx.ColRow.Add("SEQNO", docBILLNO.Text);
                mtTypeMx.ColRow["ROWNO"] = i + 1;
                mtTypeMx.ColRow.Add("DEPTID", lstDEPTID.SelectedValue.ToString());
                mtTypeMx.ColRow.Add("NUM1", nbxYSSL.Text);
                mtTypeMx.ColRow.Remove("HSJE");
                mtTypeMx.ColRow.Add("HSJE", Convert.ToDecimal(goodsData[i]["HSJJ"] ?? "0") * Convert.ToDecimal(goodsData[i]["DHS"] ?? "0"));
                mtTypeMx.ColRow.Add("ISGZ", "N");
                mtTypeMx.ColRow.Add("HISNAME", goodsData[i]["GDNAME"]);
                cmdList.Add(mtTypeMx.Insert());
            }


            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                Alert.Show("采购单生成成功！!", "消息提示", MessageBoxIcon.Information);
                billOpen(docBILLNO.Text);
                btnSave.Enabled = true;
                btnSumbit.Enabled = true;
                btnPrint.Enabled = false;
                btnExport.Enabled = false;
                btnDelRow.Enabled = false;
            }
            else
            {
                Alert.Show("采购单生成失败!", "消息提示", MessageBoxIcon.Information);
                return;
            }
        }

        protected void btnSumbit_Click(object sender, EventArgs e)
        {
            if (docFLAG.SelectedValue != "M")
            {
                Alert.Show("非【新单】的单据无法提交");
                return;
            }
            string sql = string.Format("update DAT_DDPLAN_DOC set flag='N' where BILLTYPE='CJD' AND seqno='{0}' and flag='M'", docSEQNO.Text);
            int count = DbHelperOra.ExecuteSql(sql);
            if (count > 0)
            {
                Alert.Show("采购单提交成功！!", "消息提示", MessageBoxIcon.Information);
                OperLog("采购计划", "提交单据【" + docSEQNO.Text + "】");
                billOpen(docBILLNO.Text);
                btnSave.Enabled = false;
                btnSumbit.Enabled = false;
                btnAudit.Enabled = true;
                btnCancel.Enabled = true;
                btnGenerate.Enabled = false;
            }
            else
            {
                Alert.Show("采购单提交失败！", "操作提示", MessageBoxIcon.Information);
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
                    bzslTotal += Convert.ToDecimal(row["DHS"] ?? "0");
                    feeTotal += Convert.ToDecimal(row["DHS"] ?? "0") * Convert.ToDecimal(row["HSJJ"] ?? "0");
                }
                Doc.GridRowAdd(GridGoods, dtBill);
            }

            //增加合计
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("DHS", bzslTotal.ToString());
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
                btnExport.Enabled = false;
            }
            else if (docFLAG.SelectedValue == "R")
            {
                //已驳回 可以修改保存
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
            else if (docFLAG.SelectedValue == "M")
            {
                btnSave.Enabled = true;
                btnSumbit.Enabled = true;
                btnDelRow.Enabled = false;
                btnGenerate.Enabled = false;
                btnExport.Enabled = false;
                btnPrint.Enabled = false;
                btnCancel.Enabled = false;
                btnAudit.Enabled = false;
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
            }
        }

        protected override void listRow_DoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            billOpen(GridList.Rows[e.RowIndex].Values[2].ToString());
        }

        protected override void billAudit()
        {
            string upsql = string.Format(@"UPDATE DAT_DDPLAN_DOC SET FLAG='Y',SHR='{0}',SHRQ=SYSDATE WHERE  BILLTYPE='CJD' AND FLAG＝'N' AND SEQNO='{1}'", UserAction.UserID, docSEQNO.Text);
            int count = DbHelperOra.ExecuteSql(upsql);
            if (count > 0)
            {
                Alert.Show("审批成功！", "操作提示", MessageBoxIcon.Information);
                OperLog("采购计划", "审核单据【" + docSEQNO.Text + "】");
                btnCancel.Enabled = false;
                btnAudit.Enabled = false;
                btnPrint.Enabled = true;
                btnExport.Enabled = true;
                billOpen(docBILLNO.Text);
            }
        }

        private bool CheckFlagForM()
        {
            if (docBILLNO.Text.Length > 0)
            {
                return Doc.getFlag(docBILLNO.Text, "M", BillType);
            }
            return true;
        }

        protected override void billDelRow()
        {
            if (docFLAG.SelectedValue != "M")
            {
                Alert.Show("非『新增』单据不能删行！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!CheckFlagForM())
            {
                Alert.Show("此单据已经被别人操作，请等待操作!");
                return;
            }
            if (GridGoods.SelectedCell == null)
            {
                Alert.Show("当前未选中单元行，无法进行操作!", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            int rowIndex = GridGoods.SelectedRowIndex;
            //PageContext.RegisterStartupScript(GridGoods.GetDeleteRowReference(rowIndex) + Grid_Goods.GetSelectedCellReference());
            PageContext.RegisterStartupScript(GridGoods.GetDeleteRowReference(rowIndex));

            PubFunc.FormLock(FormDoc, true, "");
            docMEMO.Enabled = true;
        }

        protected override void billExport()
        {
            if (GridList == null || docBILLNO.Text == "")
            {
                Alert.Show("没有数据无法导出，请先生成采购计划！", "异常提示", MessageBoxIcon.Warning);
                return;
            }
            //string strSql = string.Format(@"SELECT 
            //                           GD.GDNAME 商品名称,
            //                           GD.GDSEQ  商品编码,
            //                           GD.GDSPEC  商品规格,
            //                            F_GETUNITNAME(GD.Unit) 商品单位,
            //                           GD.BZHL 包装数量,
            //                           GD.PIZNO 批号,
            //                           GD.HSJJ  含税进价,
            //                           F_GETPRODUCERNAME(GD.Producer) 生产厂商,
            //                           NVL(CFG.ZGKC,0) 最高库存,
            //                           NVL(CFG.ZDKC,0) 最低库存,
            //                           DECODE(GD.UNIT_ORDER,'D',GD.NUM_DABZ,'Z',GD.NUM_ZHONGBZ,GD.BZHL)  最小包装数量,
            //                           F_GETZTKC('{3}',GD.GDSEQ) * DECODE(GD.UNIT_ORDER,'D',GD.NUM_DABZ,'Z',GD.NUM_ZHONGBZ,GD.BZHL) 在途数量,
            //                           F_GETJYDH('{3}',GD.GDSEQ,'DAY', '{1}','{2}')  日均用量,
            //                           F_GETJYDH('{3}',GD.GDSEQ,'DS', '{1}','{2}')* DECODE(GD.UNIT_ORDER,'D',GD.NUM_DABZ,'Z',GD.NUM_ZHONGBZ,GD.BZHL)*{0}  建议采购量,
            //                           F_GETJYDH('{3}',GD.GDSEQ,'DS', '{1}','{2}')* DECODE(GD.UNIT_ORDER,'D',GD.NUM_DABZ,'Z',GD.NUM_ZHONGBZ,GD.BZHL)*{0}*GD.HSJJ 含税金额
            //                      FROM 
            //                             (SELECT GDSEQ, ABS(SUM(SL)) SL
            //                              FROM DAT_GOODSJXC A
            //                             WHERE RQSJ BETWEEN TO_DATE('{1}', 'YYYY-MM-DD') AND
            //                                   TO_DATE('{2}', 'YYYY-MM-DD')
            //                               AND EXISTS (SELECT 1
            //                                      FROM SYS_DEPT
            //                                     WHERE TYPE = '3'
            //                                       AND CODE = A.DEPTID)
            //                             GROUP BY GDSEQ) JXC,

            //                           (SELECT GDSEQ, SUM(KCSL) KCSL
            //                              FROM DAT_GOODSSTOCK S
            //                             WHERE EXISTS (SELECT 1
            //                                      FROM SYS_DEPT
            //                                     WHERE TYPE = '3'
            //                                       AND CODE = S.DEPTID)
            //                             GROUP BY GDSEQ) GS,
            //                           (SELECT GDNAME, GDSEQ, GDSPEC, UNIT, BZHL, PIZNO, HSJJ, PRODUCER,UNIT_ORDER,NUM_DABZ,NUM_ZHONGBZ
            //                              FROM DOC_GOODS
            //                             WHERE ISGZ = 'N') GD,

            //                           (SELECT ZGKC, ZDKC, GDSEQ
            //                             FROM DOC_GOODSCFG WHERE DEPTID={3}) CFG
            //                            WHERE JXC.GDSEQ = GS.GDSEQ(+)
            //                              AND JXC.GDSEQ = GD.GDSEQ
            //                              AND JXC.GDSEQ = CFG.GDSEQ", Convert.ToDecimal(nbxYSSL.Text), dpStartDate.Text, dpEndDate.Text, UserAction.UserDept);
            //DataTable dt = DbHelperOra.Query(strSql).Tables[0];
            DataTable dtData = DbHelperOra.Query(string.Format("SELECT * FROM DAT_DDPLAN_COM WHERE BILLTYPE='CJD' AND SEQNO='{0}'", docBILLNO.Text)).Tables[0];
            if (dtData == null || dtData.Rows.Count == 0)
            {
                Alert.Show("未获取到单据【" + docBILLNO.Text + "】的明细信息，无法导出！", "异常提示", MessageBoxIcon.Warning);
                return;
            }
            ExcelHelper.ExportByWeb(ERPUtility.ExportDataTable(GridGoods, dtData), "采购单信息", "采购单导出_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
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
                Alert.Show("非【已提交】的单据不能驳回！", "消息提示", MessageBoxIcon.Warning);
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
            if (!string.IsNullOrWhiteSpace(txaMemo.Text) && txaMemo.Text.Length > 40)
            {
                Alert.Show("【驳回原因】字符超长");
                return;
            }

            PubFunc.FormDataCheck(Form2);

            //增加待办事宜
            List<CommandInfo> cmdList = new List<CommandInfo>();

            string strMemo = docMEMO.Text + "；驳回原因：" + ddlReject.SelectedText;
            if (!string.IsNullOrWhiteSpace(txaMemo.Text.Trim()))
            {
                strMemo += "；详细说明：" + txaMemo.Text;
            }

            //cmdList.Add(new CommandInfo(string.Format("update DAT_SL_DOC set flag='R',memo='{0}',SHR={1}，SHRQ=SYSDATE where seqno='{2}' and flag='N'", strMemo, "'" + UserAction.UserID + "'", docBILLNO.Text), null));
            cmdList.Add(new CommandInfo(string.Format("update DAT_DDPLAN_DOC SET FLAG='R',MEMO='{0}',SHR={1},SHRQ=SYSDATE WHERE SEQNO='{2}' AND BILLTYPE='CJD' AND FLAG='N'", strMemo, "'" + UserAction.UserID + "'", docBILLNO.Text), null));
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
                DataSave();
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
    }
}
