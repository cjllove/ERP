using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FineUIPro;
using Oracle.ManagedDataAccess.Client;
using XTBase;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using XTBase.Utilities;

namespace ERPProject.ERPApply
{
    public partial class DepartmentMonthBudgetReport : BillBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
                billNew();
                billSearch();
            }

            //屏蔽不需要的操作按钮



        }
        #region page load event

        private void DataInit()
        {
            DepartmentBind.BindDDL("DDL_SYS_DEPTRANGE", UserAction.UserID, lstDEPTID);
            lstDEPTID.SelectedValue = UserAction.UserDept;
            tbxMonth.Text = DateTime.Now.ToString("yyyy-MM");
        }
        #endregion
        #region gridlist event

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

        protected void GridList_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridList.PageIndex = e.NewPageIndex;
            billSearch();
        }
        #endregion

        #region button event
        protected override void billNew()
        {

        }
        protected override void billClear()
        {
            PubFunc.FormDataClear(Formlist);

        }
        protected void btExport_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbxMonth.Text))
            {
                Alert.Show("请选择[查询月份]！", MessageBoxIcon.Warning);
                return;
            }
            string startTime = string.Empty;
            string endTime = string.Empty;
            string ACCOUNTDAY = Doc.DbGetSysPara("ACCOUNTDAY");
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            if (ACCOUNTDAY == "31")
            {
                //startDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy") + "-" + ddlMonth.SelectedValue.ToString() + "-01");
                //endDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy") + "-" + ddlMonth.SelectedValue.ToString() + "-01").AddMonths(1).AddDays(-1);
                startDate = Convert.ToDateTime(tbxMonth.Text + "-01");
                endDate = Convert.ToDateTime(tbxMonth.Text + "-01").AddMonths(1).AddDays(-1);
            }
            else
            {
                //startDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy") + "-" + ddlMonth.SelectedValue.ToString() + "-" + ACCOUNTDAY).AddMonths(-1);
                //endDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy") + "-" + ddlMonth.SelectedValue.ToString() + "-" + ACCOUNTDAY).AddDays(-1);
                startDate = Convert.ToDateTime(tbxMonth.Text + "-" + ACCOUNTDAY).AddMonths(-1);
                endDate = Convert.ToDateTime(tbxMonth.Text + "-" + ACCOUNTDAY).AddDays(-1);
            }
            string strSql = string.Format(@"SELECT v.DEPTID 部门编码,
                                           f_GetDEPTNAME(v.DEPTID)　部门名称,
                                           SUM(YSJE) 预算金额,
                                           SUM(SJJE) 执行金额,
                                           ROUND(DECODE(SUM(NVL(SJJE, 0)),
                                                        0,
                                                        0,
                                                        sum(NVL(YSJE, 0)) / sum(NVL(SJJE, 0))),
                                                 4) 使用占比
                                      FROM (SELECT SA.DEPTID,
                                                   SD.GDSEQ,
                                                   SD.GdNAME,
                                                   SD.HSJJ,
                                                   SD.GDSPEC,
                                                   F_GETUNITNAME(SD.Unit) Unit,
                                                   F_GETPRODUCERNAME(SD.Producer) PRODUCTER,
                                                   NVL(SB.PDSL, 0) PDSL,
                                                   NVL(SB.PDJE, 0) PDJE,
                                                   NVL(SC.YSSL, 0) YSSL,
                                                   NVL(SC.YSJE, 0) YSJE,
                                                    NVL(F_GETXHSL(SA.DEPTID, SA.GDSEQ, '{0}', '{1}', '0'), 0) ERPSL,
                                                   NVL(F_GETXHSL(SA.DEPTID, SA.GDSEQ, '{0}', '{1}', '1'), 0) ERPJE,
                                                    F_GETKC(SA.DEPTID,SD.GDSEQ,'{1}') STOCKSL,
                                                   NVL((NVL(F_GETXHSL(SA.DEPTID, SA.GDSEQ, '{0}', '{1}', '0'), 0) -
                                                       NVL(SB.PDSL, 0) + F_GETKC(SA.DEPTID,SD.GDSEQ,'{1}')),
                                                       0) SJSL,
                                                   NVL((NVL(F_GETXHSL(SA.DEPTID, SA.GDSEQ, '{0}', '{1}', '0'), 0) -
                                                       NVL(SB.PDSL, 0) + F_GETKC(SA.DEPTID,SD.GDSEQ,'{1}')),
                                                       0) * SD.HSJJ SJJE
                                              FROM (SELECT DISTINCT DEPTID, GDSEQ
                                                      FROM (SELECT DEPTID, GDSEQ
                                                              FROM DAT_GOODSJXC A
                                                             WHERE RQSJ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                                   TO_DATE('{1}', 'YYYY-MM-DD') 
                                                               AND EXISTS (SELECT 1
                                                                      FROM SYS_DEPT
                                                                     WHERE TYPE = '3'
                                                                       AND CODE = A.DEPTID)
                                                            UNION
                                                            SELECT A.DEPTID, B.GDSEQ
                                                              FROM DAT_PD_DOC A, DAT_PD_COM B
                                                             WHERE A.SEQNO = B.SEQNO
                                                               AND A.PDTYPE = '3'
                                                               AND A.SHRQ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                                   TO_DATE('{1}', 'YYYY-MM-DD') 
                                                            UNION
                                                            SELECT A.DEPTID, B.GDSEQ
                                                              FROM DAT_YS_DOC A, DAT_YS_COM B
                                                             WHERE A.SEQNO = B.SEQNO
                                                               AND A.YSRQ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                                   TO_DATE('{1}', 'YYYY-MM-DD') )
                                                             UNION
                                                         (SELECT DEPTID, GDSEQ
                                                              FROM DAT_GOODSSTOCK
                                                             WHERE DEPTID IN (SELECT CODE FROM SYS_DEPT WHERE TYPE = '3'))) SA,
                                                   (SELECT A.DEPTID, B.GDSEQ, SUM(B.PDSL) PDSL, SUM(B.HSJE) PDJE
                                                      FROM DAT_PD_DOC A, DAT_PD_COM B
                                                     WHERE A.SEQNO = B.SEQNO
                                                       AND A.PDTYPE = '3'
                                                       AND A.SHRQ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                           TO_DATE('{1}', 'YYYY-MM-DD') 
                                                     GROUP BY A.DEPTID, B.GDSEQ) SB,
                                                   (SELECT A.DEPTID, B.GDSEQ, SUM(B.YSSL) YsSL, SUM(B.HSJE) YSJE
                                                      FROM DAT_YS_DOC A, DAT_YS_COM B
                                                     WHERE A.SEQNO = B.SEQNO
                                                       AND A.YSRQ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                           TO_DATE('{1}', 'YYYY-MM-DD') 
                                                     GROUP BY A.DEPTID, B.GDSEQ) SC,
                                                   DOC_GOODS SD,
                                                  (SELECT 
                                                 SUM(T.KCSL) STOCKSL,
                                                    T.DEPTID,
                                                    T.GDSEQ
                                      FROM DAT_STOCKDAY T
                                     WHERE NOT EXISTS
                                     (SELECT 1
                                              FROM DAT_STOCKDAY
                                             WHERE TO_CHAR(RQ, 'MM') = to_char(t.RQ, 'MM')
                                               AND RQ > t.RQ)
                 　
                                       AND RQ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                           TO_DATE('{1}', 'YYYY-MM-DD')  GROUP BY  T.DEPTID,
                                                    T.GDSEQ) SE
                                             WHERE SA.DEPTID = SB.DEPTID(+)
                                               AND SA.GDSEQ = SB.GDSEQ(+)
                                               AND SA.DEPTID = SC.DEPTID(+)
                                               AND SA.GDSEQ = SC.GDSEQ(+)
                                               AND SA.GDSEQ = SD.GDSEQ
                                               AND SA.DEPTID = SE.DEPTID(+)
                                               AND SA.GDSEQ = SE.GDSEQ(+)) v
                                     WHERE 1 = 1 ", startDate.ToString("yyyy -MM-dd"), endDate.ToString("yyyy-MM-dd"));
            //) v Group by DEPTID
            if (!string.IsNullOrEmpty(lstDEPTID.SelectedValue))
            {

                strSql += " AND v.deptid='" + lstDEPTID.SelectedValue + "'";
            }
            strSql += " Group by v.DEPTID  ";
            ExcelHelper.ExportByWeb(DbHelperOra.Query(strSql).Tables[0], "全院科室" + startDate.ToString("MM") + "月预算执行情况报告", string.Format("全院科室" + startDate.ToString("MM") + "月预算执行情况分析表_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));


        }

        protected override void billSearch()
        {
            if (string.IsNullOrEmpty(tbxMonth.Text))
            {
                Alert.Show("请选择[查询月份]！", MessageBoxIcon.Warning);
                return;
            }
            string startTime = string.Empty;
            string endTime = string.Empty;
            string ACCOUNTDAY = Doc.DbGetSysPara("ACCOUNTDAY");
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            if (ACCOUNTDAY == "31")
            {
                //startDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy") + "-" + ddlMonth.SelectedValue.ToString() + "-01");
                //endDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy") + "-" + ddlMonth.SelectedValue.ToString() + "-01").AddMonths(1).AddDays(-1);
                startDate = Convert.ToDateTime(tbxMonth.Text + "-01");
                endDate = Convert.ToDateTime(tbxMonth.Text + "-01").AddMonths(1).AddDays(-1);
            }
            else
            {
                //startDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy") + "-" + ddlMonth.SelectedValue.ToString() + "-" + ACCOUNTDAY).AddMonths(-1);
                //endDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy") + "-" + ddlMonth.SelectedValue.ToString() + "-" + ACCOUNTDAY).AddDays(-1);
                startDate = Convert.ToDateTime(tbxMonth.Text + "-" + ACCOUNTDAY).AddMonths(-1);
                endDate = Convert.ToDateTime(tbxMonth.Text + "-" + ACCOUNTDAY).AddDays(-1);
            }
            //, (case when SUM(SJJE)>0 then sum(YSJE)/sum(SJJE) else 0 end)  as PERRATE
            //,DECODE(SUM(NVL(SJJE,0)),0,0,sum(NVL(YSJE,0))/sum(NVL(SJJE,0)))  PERRATE
            string strSql = string.Format(@"SELECT v.DEPTID,
                                           f_GetDEPTNAME(v.DEPTID) as DEPTNAME,
                                           SUM(YSJE) YSTOTAL,
                                           SUM(SJJE) TOTAL,
                                           ROUND(DECODE(SUM(NVL(YSJE, 0)),
                                                        0,
                                                        0,
                                                        sum(NVL(SJJE, 0)) / sum(NVL(YSJE, 0))),
                                                 4) PERRATE
                                      FROM (SELECT SA.DEPTID,
                                                   SD.GDSEQ,
                                                   SD.GdNAME,
                                                   SD.HSJJ,
                                                   SD.GDSPEC,
                                                   F_GETUNITNAME(SD.Unit) Unit,
                                                   F_GETPRODUCERNAME(SD.Producer) PRODUCTER,
                                                   NVL(SB.PDSL, 0) PDSL,
                                                   NVL(SB.PDJE, 0) PDJE,
                                                   NVL(SC.YSSL, 0) YSSL,
                                                   NVL(SC.YSJE, 0) YSJE,
                                                    NVL(F_GETXHSL(SA.DEPTID, SA.GDSEQ, '{0}', '{1}', '0'), 0) ERPSL,
                                                   NVL(F_GETXHSL(SA.DEPTID, SA.GDSEQ, '{0}', '{1}', '1'), 0) ERPJE,
                                                   F_GETKC(SA.DEPTID,SD.GDSEQ,'{1}') STOCKSL,
                                                   NVL((NVL(F_GETXHSL(SA.DEPTID, SA.GDSEQ, '{0}', '{1}', '0'), 0) -
                                                       NVL(SB.PDSL, 0) + F_GETKC(SA.DEPTID,SD.GDSEQ,'{1}')),
                                                       0) SJSL,
                                                   NVL((NVL(F_GETXHSL(SA.DEPTID, SA.GDSEQ, '{0}', '{1}', '0'), 0) 
                                                       + F_GETKC(SA.DEPTID,SD.GDSEQ,'{1}')),
                                                       0) * SD.HSJJ SJJE
                                              FROM (SELECT DISTINCT DEPTID, GDSEQ
                                                      FROM (SELECT DEPTID, GDSEQ
                                                              FROM DAT_GOODSJXC A
                                                             WHERE RQSJ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                                   TO_DATE('{1}', 'YYYY-MM-DD') 
                                                               AND A.BILLTYPE<>'LCD'
                                                               AND EXISTS (SELECT 1
                                                                      FROM SYS_DEPT
                                                                     WHERE TYPE = '3'
                                                                       AND CODE = A.DEPTID)
                                                            UNION
                                                            SELECT A.DEPTID, B.GDSEQ
                                                              FROM DAT_PD_DOC A, DAT_PD_COM B
                                                             WHERE A.SEQNO = B.SEQNO
                                                               AND A.PDTYPE = '3' AND A.FLAG = 'Y'
                                                               AND A.SPRQ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                                   TO_DATE('{1}', 'YYYY-MM-DD')
                                                            UNION
                                                            SELECT A.DEPTID, B.GDSEQ
                                                              FROM DAT_YS_DOC A, DAT_YS_COM B
                                                             WHERE A.SEQNO = B.SEQNO AND B.YSSL > 0 AND A.FLAG = 'S'
                                                               AND A.YSRQ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                                   TO_DATE('{1}', 'YYYY-MM-DD'))) SA,
                                                   (SELECT A.DEPTID, B.GDSEQ, SUM(B.PDSL) PDSL, SUM(B.HSJE) PDJE
                                                      FROM DAT_PD_DOC A, DAT_PD_COM B
                                                     WHERE A.SEQNO = B.SEQNO
                                                       AND A.PDTYPE = '3' AND A.FLAG = 'Y'
                                                       AND A.SPRQ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                           TO_DATE('{1}', 'YYYY-MM-DD')
                                                     GROUP BY A.DEPTID, B.GDSEQ) SB,
                                                   (SELECT A.DEPTID, B.GDSEQ, SUM(B.YSSL) YsSL, SUM(B.HSJE) YSJE
                                                      FROM DAT_YS_DOC A, DAT_YS_COM B
                                                     WHERE A.SEQNO = B.SEQNO AND A.FLAG = 'S' AND B.YSSL > 0
                                                       AND A.YSRQ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                           TO_DATE('{1}', 'YYYY-MM-DD') 
                                                     GROUP BY A.DEPTID, B.GDSEQ) SC,
                                                   DOC_GOODS SD,
                                               (SELECT  to_Char(RQ, 'MM') as MONTHLY, 
                                                 SUM(T.KCSL) STOCKSL,
                                                  T.DEPTID, 
                                                 T.GDSEQ
                                                  FROM DAT_STOCKDAY T
                                                 WHERE NOT EXISTS (SELECT 1
                                                          FROM DAT_STOCKDAY
                                                         WHERE TO_CHAR(RQ, 'MM') = to_char(t.RQ, 'MM')
                                                           AND RQ > t.RQ)
 　
                                                   AND RQ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                       TO_DATE('{1}', 'YYYY-MM-DD')  GROUP BY  to_Char(RQ, 'MM') ,
                                                  T.DEPTID, 
                                                 T.GDSEQ) SE
                                             WHERE SA.DEPTID = SB.DEPTID(+)
                                               AND SA.GDSEQ = SB.GDSEQ(+)
                                               AND SA.DEPTID = SC.DEPTID(+)
                                               AND SA.GDSEQ = SC.GDSEQ(+)
                                               AND SA.GDSEQ = SD.GDSEQ(+)
                                              AND SA.GDSEQ = SE.GDSEQ(+)
                                              AND SA.DEPTID = SE.DEPTID(+)

                                           ) v
                                     WHERE 1 = 1 ", startDate.ToString("yyyy -MM-dd"), endDate.ToString("yyyy-MM-dd"));

            //) v Group by DEPTID
            if (!string.IsNullOrEmpty(lstDEPTID.SelectedValue))
            {
                strSql += " AND v.deptid='" + lstDEPTID.SelectedValue + "'";
            }
            strSql += " Group by v.DEPTID  ";
            int total = 0;
            DataTable dt = PubFunc.DbGetPage(GridList.PageIndex, GridList.PageSize, strSql, ref total);
            GridList.DataSource = dt;
            GridList.RecordCount = total;
            GridList.DataBind();
            decimal ddslTotal = 0, bzslTotal = 0;
            if (total > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ddslTotal += Convert.ToDecimal(dr["ystotal"] ?? 0);
                    bzslTotal += Convert.ToDecimal(dr["total"] ?? 0);

                }

            }
            JObject summary = new JObject();
            summary.Add("DEPTNAME", "本页合计");
            summary.Add("YSJE", ddslTotal.ToString("F2"));
            summary.Add("TOTAL", bzslTotal.ToString("F2"));
            //   summary.Add("HSJE", feeTotal.ToString("F2"));
            GridList.SummaryData = summary;
        }
        private JObject GetJObject(Dictionary<string, object> dicRecord)
        {
            JObject defaultObj = new JObject();
            foreach (string key in dicRecord.Keys)
            {
                defaultObj.Add(key, (dicRecord[key] ?? "").ToString());
            }

            decimal hl = 0, rs = 0, jg = 0;
            decimal.TryParse((dicRecord["YSJE"] ?? "0").ToString(), out hl);
            decimal.TryParse((dicRecord["ZXJE"] ?? "0").ToString(), out rs);
            //  decimal.TryParse((dicRecord["HSJJ"] ?? "0").ToString(), out jg);

            defaultObj.Remove("YSJE");
            defaultObj.Remove("ZXJE");
            // defaultObj.Add("DHSL", rs * hl);

            //处理金额格式
            /* string jingdu = Math.Round(rs * jg, 2).ToString("F2");
             defaultObj.Add("HSJE", jingdu);*/

            return defaultObj;
        }
        #endregion
    }
}