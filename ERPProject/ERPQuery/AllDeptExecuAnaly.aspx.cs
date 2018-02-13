﻿using System;
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

namespace ERPProject.ERPQuery
{
    public partial class AllDeptExecuAnaly : PageBase
    {
        public AllDeptExecuAnaly()
        {
            ISCHECK = false;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
                // DataSearch();
                //  billSearch();

            }

            //屏蔽不需要的操作按钮



        }
        #region page load event

        private void DataInit()
        {
            DepartmentBind.BindDDL("DDL_SYS_DEPTRANGE", UserAction.UserID, lstDEPTID, ddlDEPTID);
            lstDEPTID.SelectedValue = UserAction.UserDept;
            tbxMonth.Text = DateTime.Now.ToString("yyyy-MM");
            tbxMonth2.Text = DateTime.Now.ToString("yyyy-MM");
        }
        #endregion
        #region gridlist event

        protected void GridList_Sort(object sender, GridSortEventArgs e)
        {
            billSearch();
        }

        protected void GridList_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridList.PageIndex = e.NewPageIndex;
            billSearch();
        }
        #endregion

        protected void GridList_RowClick(object sender, GridRowClickEventArgs e)
        {
            string deptID = GridList.Rows[e.RowIndex].DataKeys[0].ToString();
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
            string strSql = string.Format(@"SELECT A.*,F_GETDEPTNAME(A.DEPTID)DEPTNAME,
ROUND(DECODE(SUM(NVL(YSTOTAL, 0)),0,0,sum(NVL(TOTAL, 0)) / sum(NVL(YSTOTAL, 0))),4) PERRATE      
FROM (SELECT 
        DEPTID,
       SUM(NVL(YSJE, 0)) YSTOTAL,
       ROUND(NVL(SUM(SJJE), 0), 2) TOTAL
       FROM (SELECT A.DEPTID,F_GETYSTOTAL(A.DEPTID, '{0}', '{1}') YSJE,ABS(NVL(SUM(B.HSJE), 0)) AS SJJE
                    FROM DAT_XS_DOC A, DAT_XS_COM B
                    WHERE A.SEQNO = B.SEQNO
                          AND A.FLAG IN ('Y', 'G')
                          AND A.SHRQ >= TO_DATE('{0}', 'YYYY-MM-dd') 
                          AND A.SHRQ<TO_DATE('{1}', 'YYYY-MM-dd') + 1
                          GROUP BY DEPTID)
       GROUP BY DEPTID) A
 WHERE 1=1", startDate.ToString("yyyy -MM-dd"), endDate.ToString("yyyy-MM-dd"));
            if (deptID.Length > 0)
            {
                strSql += " AND a.deptid='" + deptID + "'";
            }
            strSql += " Group by a.DEPTID,YSTOTAL,TOTAL  ";

            DataTable dt = DbHelperOra.QueryForTable(strSql);
            hfdArray.Text = "";
            hfdArrayVal.Text = "";
            hfdArrayVal2.Text = "";
            Decimal YSJE = 0, HSJE = 0, total = 0, totaltb = 0, totalhb = 0;
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                YSJE += Convert.ToDecimal(dr["YSTOTAL"]);
                HSJE += Convert.ToDecimal(dr["TOTAL"]);

                if (i > 9)
                {
                    totaltb += Convert.ToDecimal(dr["YSTOTAL"]) * Convert.ToDecimal(dr["YSTOTAL"]);
                    totalhb += Convert.ToDecimal(dr["TOTAL"]) * Convert.ToDecimal(dr["TOTAL"]);
                    //total += Convert.ToDecimal(dr["HSJE"].ToString());
                }
                else
                {
                    hfdArray.Text += dr["DEPTNAME"] + ",";
                    //   hfdArrayVal.Text += dr["YSTOTAL"] + "$" + dr["TOTAL"] + ",";
                    hfdArrayVal.Text += dr["YSTOTAL"] + "|" + dr["TOTAL"] + ",";
                }
                i++;
            }
            if (total > 0)
            {
                hfdArray.Text += "其他,";
                hfdArrayVal.Text += total.ToString() + "$其他,";
            }
            hfdArray.Text = hfdArray.Text.TrimEnd(',');

            hfdArrayVal.Text = hfdArrayVal.Text.TrimEnd(',');
            Totalsl.Text = YSJE.ToString();

            PageContext.RegisterStartupScript("getEcharsData();updateDate();");
        }

        #region button event


        protected void btExpor_Click(object sender, EventArgs e)
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
            string strSql = string.Format(@"SELECT DEPTNAME 部门名称, YSTOTAL 预算金额,TYSTOTAL 同比预算金额,HYSTOTAL 环比预算金额,TYSPERRATE|| '%' 预算同比, HYSPERRATE|| '%' 预算环比,
TOTAL 执行金额,TSJJE 同比执行金额,HSJJE 环比执行金额,TPERRATE || '%' 执行同比,HPERRATE || '%' 执行环比,PERRATE*100 || '%' 占比
FROM
(SELECT A.*,F_GETDEPTNAME(A.DEPTID)DEPTNAME,
NVL(B.HYSTOTAL,0)HYSTOTAL,
NVL(B.HSJJE,0)HSJJE,
NVL(C.TSJJE,0)TSJJE,
NVL(C.TYSTOTAL,0)TYSTOTAL,
ROUND(DECODE(SUM(NVL(YSTOTAL, 0)),0,0,sum(NVL(TOTAL, 0)) / sum(NVL(YSTOTAL, 0))),4) PERRATE,
DECODE(C.TYSTOTAL,0,0,NVL(ROUND(A.YSTOTAL /C.TYSTOTAL - 1, 4) * 100, 0))TYSPERRATE,
DECODE(B.HYSTOTAL,0,0,NVL(ROUND(A.YSTOTAL /B.HYSTOTAL - 1, 4) * 100, 0))HYSPERRATE,
DECODE(C.TSJJE,0,0,NVL(ROUND(A.TOTAL /C.TSJJE - 1, 4) * 100, 0)) TPERRATE,
DECODE(B.HSJJE,0,0,NVL(ROUND(A.TOTAL /B.HSJJE - 1, 4) * 100, 0))HPERRATE       
FROM (SELECT 
        DEPTID,
       SUM(NVL(YSJE, 0)) YSTOTAL,
       ROUND(NVL(SUM(SJJE), 0), 2) TOTAL
       FROM (SELECT A.DEPTID,F_GETYSTOTAL(A.DEPTID, '{0}', '{1}') YSJE,ABS(NVL(SUM(B.HSJE), 0)) AS SJJE
                    FROM DAT_XS_DOC A, DAT_XS_COM B
                    WHERE A.SEQNO = B.SEQNO
                          AND A.FLAG IN ('Y', 'G')
                          AND A.SHRQ >= TO_DATE('{0}', 'YYYY-MM-dd') 
                          AND A.SHRQ<TO_DATE('{1}', 'YYYY-MM-dd') + 1
                          GROUP BY DEPTID)
       GROUP BY DEPTID) A,

           (SELECT DEPTID, SUM(NVL(YSJE, 0)) HYSTOTAL, ROUND(NVL(SUM(SJJE), 0), 2) HSJJE
                   FROM (SELECT A.DEPTID,F_GETYSTOTAL(A.DEPTID, '{4}', '{5}') YSJE,ABS(NVL(SUM(B.HSJE), 0)) AS SJJE
                                FROM DAT_XS_DOC A, DAT_XS_COM B
                                WHERE A.SEQNO = B.SEQNO
                                      AND A.FLAG IN ('Y', 'G')
                                      AND A.SHRQ >= TO_DATE('{4}', 'YYYY-MM-dd') 
                                      AND A.SHRQ <TO_DATE('{5}', 'YYYY-MM-dd') + 1
                                     GROUP BY DEPTID)
             GROUP BY DEPTID) B,

          (SELECT DEPTID, SUM(NVL(YSJE, 0)) TYSTOTAL, ROUND(NVL(SUM(SJJE), 0), 2) TSJJE
                  FROM ( SELECT A.DEPTID,F_GETYSTOTAL(A.DEPTID, '{2}', '{3}') YSJE,ABS(NVL(SUM(B.HSJE), 0)) AS SJJE
                                FROM DAT_XS_DOC A, DAT_XS_COM B
                                WHERE A.SEQNO = B.SEQNO
                                      AND A.FLAG IN ('Y', 'G')
                                      AND A.SHRQ >= TO_DATE('{2}', 'YYYY-MM-dd')
                                      AND A.SHRQ <TO_DATE('{3}', 'YYYY-MM-dd') + 1
                                     GROUP BY DEPTID)
           GROUP BY DEPTID) C
 WHERE A.DEPTID = B.DEPTID(+)
       AND A.DEPTID = C.DEPTID(+) ", startDate.ToString("yyyy -MM-dd"), endDate.ToString("yyyy -MM-dd"), startDate.AddYears(-1).ToString("yyyy-MM-dd"), endDate.AddYears(-1).ToString("yyyy-MM-dd"), startDate.AddMonths(-1).ToString("yyyy-MM-dd"), endDate.AddMonths(-1).ToString("yyyy-MM-dd"));
            if (!string.IsNullOrEmpty(lstDEPTID.SelectedValue))
            {

                strSql += " AND A.DeptID='" + lstDEPTID.SelectedValue + "'";
            }
            strSql += "GROUP BY A.DEPTID,A.YSTOTAL,A.TOTAL,HYSTOTAL,HSJJE,TYSTOTAL,TSJJE";
            strSql += String.Format(" ORDER BY A.TOTAL DESC)");
            ExcelHelper.ExportByWeb(DbHelperOra.Query(strSql).Tables[0], "全院科室" + startDate.ToString("MM") + "月预算执行情况报告", string.Format("全院科室" + startDate.ToString("MM") + "月预算执行情况分析表_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));


        }
        protected void btnBill_Click(object sender, EventArgs e)
        {

            billSearch();
        }
        public void billSearch()
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


            int Total = 0;
            string strSQL = GetDiffDeptPeriodVal(startDate, endDate);
            DataTable dt = PubFunc.DbGetPage(GridList.PageIndex, GridList.PageSize, strSQL, ref Total);
            GridList.DataSource = dt;
            GridList.RecordCount = Total;
            GridList.DataBind();
            JObject summary = new JObject();
            hfdArray.Text = "";
            hfdArrayVal.Text = "";
            hfdArrayVal2.Text = "";
            Decimal YSJE = 0, HSJE = 0, total = 0, totaltb = 0, totalhb = 0;
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                YSJE += Convert.ToDecimal(dr["YSTOTAL"]);
                HSJE += Convert.ToDecimal(dr["TOTAL"]);

                if (i > 9)
                {
                    totaltb += Convert.ToDecimal(dr["YSTOTAL"]) * Convert.ToDecimal(dr["YSTOTAL"]);
                    totalhb += Convert.ToDecimal(dr["TOTAL"]) * Convert.ToDecimal(dr["TOTAL"]);
                    //total += Convert.ToDecimal(dr["HSJE"].ToString());
                }
                else
                {
                    hfdArray.Text += dr["DEPTNAME"] + ",";
                    //   hfdArrayVal.Text += dr["YSTOTAL"] + "$" + dr["TOTAL"] + ",";
                    hfdArrayVal.Text += dr["YSTOTAL"] + "|" + dr["TOTAL"] + ",";
                }
                i++;
            }
            if (total > 0)
            {
                hfdArray.Text += "其他,";
                hfdArrayVal.Text += total.ToString() + "$其他,";
            }
            hfdArray.Text = hfdArray.Text.TrimEnd(',');

            hfdArrayVal.Text = hfdArrayVal.Text.TrimEnd(','); ///(Math.Round(totaltb, 2)).ToString() + "," + (Math.Round(totalhb, 2)).ToString(); //    HSJE.ToString() + ","
            Totalsl.Text = YSJE.ToString();
            summary.Add("DEPTNAME", "本页合计");
            summary.Add("YSTOTAL", YSJE.ToString("F2"));
            summary.Add("TOTAL", HSJE.ToString("F2"));
            GridList.SummaryData = summary;
            PageContext.RegisterStartupScript("getEcharsData();updateDate();");
            /* JObject summary = new JObject();
             summary.Add("DEPTNAME", "本页合计");
             summary.Add("YSJE", ddslTotal.ToString("F2"));
             summary.Add("TOTAL", bzslTotal.ToString("F2"));
             //   summary.Add("HSJE", feeTotal.ToString("F2"));
             GridList.SummaryData = summary;*/
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

        protected void GridList_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            string deptID = GridList.Rows[e.RowIndex].DataKeys[0].ToString();
            ddlDEPTID.SelectedValue = deptID;
            tbxMonth2.Text = tbxMonth.Text;
            TabStrip1.ActiveTabIndex = 1;
            btnSch_Click(sender, e);
            //PageContext.RegisterStartupScript("getEcharsData2();");
        }

        private void DataSearch()
        {
            if (PubFunc.StrIsEmpty(tbxMonth2.Text.Trim()))
            {

                Alert.Show("请选择[查询月份]！", MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(ddlDEPTID.SelectedValue))
            {
                Alert.Show("请选择[查询科室]！", MessageBoxIcon.Warning);
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
                startDate = Convert.ToDateTime(tbxMonth2.Text + "-01");
                endDate = Convert.ToDateTime(tbxMonth2.Text + "-01").AddMonths(1).AddDays(-1);
            }
            else
            {
                //startDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy") + "-" + ddlMonth.SelectedValue.ToString() + "-" + ACCOUNTDAY).AddMonths(-1);
                //endDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy") + "-" + ddlMonth.SelectedValue.ToString() + "-" + ACCOUNTDAY).AddDays(-1);
                startDate = Convert.ToDateTime(tbxMonth2.Text + "-" + ACCOUNTDAY).AddMonths(-1);
                endDate = Convert.ToDateTime(tbxMonth2.Text + "-" + ACCOUNTDAY).AddDays(-1);
            }
            //, (case whe

            int Total = 0;
            string strSQL = GetDiffShpPeriodVal(startDate, endDate);
            DataTable dt = PubFunc.DbGetPage(GridList.PageIndex, GridList.PageSize, strSQL, ref Total);
            GridGoods.DataSource = dt;
            GridGoods.RecordCount = Total;
            GridGoods.DataBind();
            JObject summary = new JObject();
            hfdArray2.Text = "";
            hfdArrayVal.Text = "";
            hfdArrayVal2.Text = "";
            Decimal YSJE = 0, HSJE = 0, total = 0, totaltb = 0, totalhb = 0;
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                YSJE += Convert.ToDecimal(dr["YSTOTAL"]);
                HSJE += Convert.ToDecimal(dr["TOTAL"]);

                if (i > 9)
                {
                    totaltb += Convert.ToDecimal(dr["YSTOTAL"]) * Convert.ToDecimal(dr["YSTOTAL"]);
                    totalhb += Convert.ToDecimal(dr["TOTAL"]) * Convert.ToDecimal(dr["TOTAL"]);
                    total += i;
                }
                else
                {
                    //hfdArray2.Text += dr["GDNAME"].ToString().Length > 10 ? dr["GDNAME"].ToString().Substring(0, 10) + "," : dr["GDNAME"].ToString() + ",";
                    //   hfdArrayVal.Text += dr["YSTOTAL"] + "$" + dr["TOTAL"] + ",";
                    hfdArray2.Text += dr["GDNAME"].ToString() + ",";
                    hfdArrayVal2.Text += dr["YSTOTAL"] + "|" + dr["TOTAL"] + ",";
                }
                i++;
            }
            if (total > 0)
            {
                hfdArray2.Text += "其他,";
                hfdArrayVal.Text += total.ToString() + "$其他,";
                hfdArrayVal2.Text += totaltb + "|" + totalhb + ",";
            }
            hfdArray2.Text = hfdArray2.Text.TrimEnd(',');
            hfdArrayVal2.Text = hfdArrayVal2.Text.TrimEnd(','); ///(Math.Round(totaltb, 2)).ToString() + "," + (Math.Round(totalhb, 2)).ToString(); //    HSJE.ToString() + ","
            summary.Add("GDNAME", "本页合计");
            summary.Add("YSJE", YSJE.ToString("F2"));
            summary.Add("TOTAL", HSJE.ToString("F2"));
            GridGoods.SummaryData = summary;
            PageContext.RegisterStartupScript("getEcharsData2();");

        }
        protected void btClear_Click(object sender, EventArgs e)
        {

            if (TabStrip1.ActiveTabIndex == 0)
            {

                lstDEPTID.SelectedValue = "";
                tbxMonth.Text = DateTime.Now.ToString("yyyy-MM");
            }
            if (TabStrip1.ActiveTabIndex == 1)
            {

                ddlDEPTID.SelectedValue = "";
                tbxMonth2.Text = DateTime.Now.ToString("yyyy-MM");
            }
        }

        protected void GridGoods_Sort(object sender, GridSortEventArgs e)
        {
            GridGoods.SortDirection = e.SortDirection;
            GridGoods.SortField = e.SortField;

            DataTable table = PubFunc.GridDataGet(GridGoods);
            DataView view1 = table.DefaultView;
            view1.Sort = String.Format("{0} {1}", GridGoods.SortField, GridGoods.SortDirection);
            GridGoods.DataSource = view1;
            GridGoods.DataBind();
        }
        protected void GridGoods_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridList.PageIndex = e.NewPageIndex;
            billSearch();
        }
        public void btnSch_Click(object sender, EventArgs e)
        {
            DataSearch();


        }

        protected void GridGoods_RowClick(object sender, GridRowClickEventArgs e)
        {
            string GDSEQ = GridGoods.Rows[e.RowIndex].DataKeys[0].ToString();
            if (PubFunc.StrIsEmpty(tbxMonth2.Text.Trim()))
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
                startDate = Convert.ToDateTime(tbxMonth2.Text + "-01");
                endDate = Convert.ToDateTime(tbxMonth2.Text + "-01").AddMonths(1).AddDays(-1);
            }
            else
            {
                //startDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy") + "-" + ddlMonth.SelectedValue.ToString() + "-" + ACCOUNTDAY).AddMonths(-1);
                //endDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy") + "-" + ddlMonth.SelectedValue.ToString() + "-" + ACCOUNTDAY).AddDays(-1);
                startDate = Convert.ToDateTime(tbxMonth2.Text + "-" + ACCOUNTDAY).AddMonths(-1);
                endDate = Convert.ToDateTime(tbxMonth2.Text + "-" + ACCOUNTDAY).AddDays(-1);
            }
            //, (case whe
            //string strSql = string.Format(@"SELECT GDSEQ,GDNAME, SUM(YSJE) YSTOTAL,SUM(SJJE) TOTAL, ROUND(DECODE(SUM(NVL(YSJE, 0)),
            //                                            0,
            //                                            0,
            //                                            sum(NVL(SJJE, 0)) / sum(NVL(YSJE, 0))),
            //                                     4) PERRATE FROM (SELECT 

            //                                       SD.GDSEQ,
            //                                       SD.GdNAME,

            //                                       NVL(SC.YSSL, 0) YSSL,
            //                                       NVL(SC.YSJE, 0) YSJE,
            //                                      NVL((NVL(F_GETXHSL(SA.DEPTID, SA.GDSEQ, '{0}', '{1}', '0'), 0) -
            //                                           NVL(SB.PDSL, 0) + F_GETKC(SA.DEPTID,SD.GDSEQ,'{1}')),
            //                                           0) * SD.HSJJ SJJE,
            //                                       SD.HISCODE HISCODE,
            //                                       UPPER (SD.ZJM) ZJM
            //                                  FROM (SELECT DISTINCT DEPTID, GDSEQ
            //                                          FROM (SELECT DEPTID, GDSEQ
            //                                                  FROM DAT_GOODSJXC A
            //                                                 WHERE RQSJ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
            //                                                       TO_DATE('{1}', 'YYYY-MM-DD') 
            //                                                   AND EXISTS (SELECT 1
            //                                                          FROM SYS_DEPT
            //                                                         WHERE TYPE = '3'
            //                                                           AND CODE = A.DEPTID)
            //                                                UNION
            //                                                SELECT A.DEPTID, B.GDSEQ
            //                                                  FROM DAT_PD_DOC A, DAT_PD_COM B
            //                                                 WHERE A.SEQNO = B.SEQNO
            //                                                   AND A.PDTYPE = '3' AND A.FLAG = 'Y'
            //                                                   AND A.SPRQ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
            //                                                       TO_DATE('{1}', 'YYYY-MM-DD') 
            //                                                UNION
            //                                                SELECT A.DEPTID, B.GDSEQ
            //                                                  FROM DAT_YS_DOC A, DAT_YS_COM B
            //                                                 WHERE A.SEQNO = B.SEQNO AND A.FLAG = 'S' AND B.YSSL > 0
            //                                                   AND A.YSRQ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
            //                                                       TO_DATE('{1}', 'YYYY-MM-DD') 
            //                                               )) SA,
            //                                       (SELECT A.DEPTID, B.GDSEQ, SUM(B.PDSL) PDSL, SUM(B.HSJE) PDJE
            //                                          FROM DAT_PD_DOC A, DAT_PD_COM B
            //                                         WHERE A.SEQNO = B.SEQNO
            //                                           AND A.PDTYPE = '3' AND A.FLAG = 'Y'
            //                                           AND A.SPRQ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
            //                                               TO_DATE('{1}', 'YYYY-MM-DD')
            //                                         GROUP BY A.DEPTID, B.GDSEQ) SB,
            //                                       (SELECT A.DEPTID, B.GDSEQ, SUM(B.YSSL) YsSL, SUM(B.HSJE) YSJE
            //                                          FROM DAT_YS_DOC A, DAT_YS_COM B
            //                                         WHERE A.SEQNO = B.SEQNO AND A.FLAG = 'S' AND B.YSSL > 0
            //                                           AND A.YSRQ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
            //                                               TO_DATE('{1}', 'YYYY-MM-DD') 
            //                                         GROUP BY A.DEPTID, B.GDSEQ) SC,
            //                                       DOC_GOODS SD
            //                                 WHERE SA.DEPTID = SB.DEPTID(+)
            //                                   AND SA.GDSEQ = SB.GDSEQ(+)
            //                                   AND SA.DEPTID = SC.DEPTID(+)
            //                                   AND SA.GDSEQ = SC.GDSEQ(+)
            //                                   AND SA.GDSEQ = SD.GDSEQ", startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
            //if (!string.IsNullOrEmpty(ddlDEPTID.SelectedValue))
            //{

            //    strSql += " AND SA.DeptID='" + ddlDEPTID.SelectedValue + "'";
            //}
            string strSqlAll = string.Format(@"SELECT  E.GDSEQ,E.GDNAME,E.DEPTID,NVL(D.YSTOTAL,0) YSTOTAL,NVL(A.TOTAL,0) TOTAL,
       F_GETDEPTNAME(E.DEPTID) DEPTNAME,
NVL(B.HYSTOTAL, 0) HYSTOTAL,
       NVL(B.HSJJE, 0) HSJJE,
       NVL(C.TSJJE, 0) TSJJE,
       NVL(C.TYSTOTAL, 0) TYSTOTAL,
       ROUND(DECODE(SUM(NVL(D.YSTOTAL, 0)),
                    0,
                    0,
                    sum(NVL(TOTAL, 0)) / sum(NVL(D.YSTOTAL, 0))),
             4) PERRATE,
       DECODE(C.TYSTOTAL,
              0,
              0,
              NVL(ROUND(D.YSTOTAL / C.TYSTOTAL - 1, 4) * 100, 0)) TYSPERRATE,
       DECODE(B.HYSTOTAL,
              0,
              0,
              NVL(ROUND(D.YSTOTAL / B.HYSTOTAL - 1, 4) * 100, 0)) HYSPERRATE,
       DECODE(C.TSJJE, 0, 0, NVL(ROUND(A.TOTAL / C.TSJJE - 1, 4) * 100, 0)) TPERRATE,
       DECODE(B.HSJJE, 0, 0, NVL(ROUND(A.TOTAL / B.HSJJE - 1, 4) * 100, 0)) HPERRATE      
FROM (SELECT GDSEQ,GDNAME,DEPTID,SUM(NVL(YSJE, 0)) YSTOTAL,ROUND(NVL(SUM(SJJE), 0), 2) TOTAL
       FROM (SELECT B.GDSEQ,B.GDNAME,A.DEPTID,F_GETGDYS(A.DEPTID,B.GDSEQ,'{0}', '{1}') YSJE,ABS(NVL(SUM(B.HSJE), 0)) AS SJJE
                    FROM DAT_XS_DOC A, DAT_XS_COM B
                    WHERE A.SEQNO = B.SEQNO
                          AND A.FLAG IN ('Y', 'G')
                          AND A.SHRQ >= TO_DATE('{0}', 'YYYY-MM-dd') 
                          AND A.SHRQ<TO_DATE('{1}', 'YYYY-MM-dd') + 1  AND A.DEPTID = '{6}' 
                          GROUP BY GDSEQ,GDNAME,DEPTID)
       GROUP BY GDSEQ,GDNAME,DEPTID) A,

           (SELECT GDSEQ,GDNAME,DEPTID, SUM(NVL(YSJE, 0)) HYSTOTAL, ROUND(NVL(SUM(SJJE), 0), 2) HSJJE
                   FROM (SELECT  GDSEQ,GDNAME,A.DEPTID,F_GETGDYS(A.DEPTID,B.GDSEQ,'{4}', '{5}') YSJE,ABS(NVL(SUM(B.HSJE), 0)) AS SJJE
                                FROM DAT_XS_DOC A, DAT_XS_COM B
                                WHERE A.SEQNO = B.SEQNO
                                      AND A.FLAG IN ('Y', 'G')
                                      AND A.SHRQ >= TO_DATE('{4}', 'YYYY-MM-dd') 
                                      AND A.SHRQ <TO_DATE('{5}', 'YYYY-MM-dd') + 1  AND A.DEPTID = '{6}' 
                                     GROUP BY GDSEQ,GDNAME,DEPTID)
             GROUP BY GDSEQ,GDNAME,DEPTID) B,

          (SELECT GDSEQ,GDNAME,DEPTID, SUM(NVL(YSJE, 0)) TYSTOTAL, ROUND(NVL(SUM(SJJE), 0), 2) TSJJE
                  FROM ( SELECT GDSEQ,GDNAME,A.DEPTID,F_GETGDYS(A.DEPTID,B.GDSEQ, '{2}', '{3}') YSJE,ABS(NVL(SUM(B.HSJE), 0)) AS SJJE
                                FROM DAT_XS_DOC A, DAT_XS_COM B
                                WHERE A.SEQNO = B.SEQNO
                                      AND A.FLAG IN ('Y', 'G')
                                      AND A.SHRQ >= TO_DATE('{2}', 'YYYY-MM-dd')
                                      AND A.SHRQ <TO_DATE('{3}', 'YYYY-MM-dd') + 1 AND A.DEPTID = '{6}' 
                                     GROUP BY GDSEQ,GDNAME,DEPTID)
           GROUP BY GDSEQ,GDNAME,DEPTID) C,
(SELECT  A.DEPTID,B.GDSEQ,B.GDNAME,SUM(B.HSJE) YSTOTAL
          FROM DAT_YS_DOC A, DAT_YS_COM B
         WHERE A.FLAG = 'S'
           AND A.SEQNO = B.SEQNO
           AND A.SHRQ BETWEEN TO_DATE('{0}', 'YYYY-MM-dd') AND
               TO_DATE('{1}', 'YYYY-MM-dd') - 1  AND A.DEPTID = '{6}' 
               GROUP BY A.DEPTID,B.GDSEQ,B.GDNAME) D,
         (SELECT DISTINCT A.GDSEQ,B.GDNAME, A.DEPTID FROM DOC_GOODSCFG A,DOC_GOODS B WHERE A.GDSEQ=B.GDSEQ) E
 WHERE E.GDSEQ=D.GDSEQ(+)
   AND E.GDSEQ=A.GDSEQ(+)
   AND E.GDSEQ = B.GDSEQ(+)
   AND E.GDSEQ = C.GDSEQ(+)
   AND E.DEPTID = '{6}'
   AND (D.YSTOTAL>0 OR A.TOTAL>0 OR HYSTOTAL>0 OR HSJJE>0 OR TYSTOTAL>0 OR TSJJE>0) ", startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"), startDate.AddYears(-1).ToString("yyyy-MM-dd"), endDate.AddYears(-1).ToString("yyyy-MM-dd"), startDate.AddMonths(-1).ToString("yyyy-MM-dd"), endDate.AddMonths(-1).ToString("yyyy-MM-dd"), ddlDEPTID.SelectedValue);

            if (GDSEQ.Length > 0)
            {
                strSqlAll += " AND E.GDSEQ='" + GDSEQ + "'";
            }
            strSqlAll += @"GROUP BY E.DEPTID,
          D.YSTOTAL,
          A.TOTAL,
          HYSTOTAL,
          HSJJE,
          TYSTOTAL,
          TSJJE,
          E.GDSEQ,
          E.GDNAME ORDER BY NVL(A.TOTAL,0) DESC";
            DataTable dt = DbHelperOra.QueryForTable(strSqlAll);

            hfdArray2.Text = "";
            hfdArrayVal.Text = "";
            hfdArrayVal2.Text = "";
            Decimal YSJE = 0, HSJE = 0, total = 0, totaltb = 0, totalhb = 0;
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                YSJE += Convert.ToDecimal(dr["YSTOTAL"]);
                HSJE += Convert.ToDecimal(dr["TOTAL"]);

                if (i > 9)
                {
                    totaltb += Convert.ToDecimal(dr["YSTOTAL"]) * Convert.ToDecimal(dr["YSTOTAL"]);
                    totalhb += Convert.ToDecimal(dr["TOTAL"]) * Convert.ToDecimal(dr["TOTAL"]);
                    //total += Convert.ToDecimal(dr["HSJE"].ToString());
                }
                else
                {
                    hfdArray2.Text += dr["GDNAME"].ToString() + ",";
                    //   hfdArrayVal.Text += dr["YSTOTAL"] + "$" + dr["TOTAL"] + ",";
                    hfdArrayVal2.Text += dr["YSTOTAL"] + "|" + dr["TOTAL"] + ",";
                }
                i++;
            }
            if (total > 0)
            {
                hfdArray2.Text += "其他,";
                hfdArrayVal.Text += total.ToString() + "$其他,";
            }
            hfdArray2.Text = hfdArray2.Text.TrimEnd(',');

            hfdArrayVal2.Text = hfdArrayVal2.Text.TrimEnd(','); ///(Math.Round(totaltb, 2)).ToString() + "," + (Math.Round(totalhb, 2)).ToString(); //    HSJE.ToString() + ","

            PageContext.RegisterStartupScript("getEcharsData2();");
        }
        protected void btExport_Click(object sender, EventArgs e)
        {

            btnExpt.Enabled = false;
            if (PubFunc.StrIsEmpty(tbxMonth2.Text.Trim()))
            {
                Alert.Show("请选择[查询月份]！", MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(ddlDEPTID.SelectedValue))
            {
                Alert.Show("请选择[查询科室]！", MessageBoxIcon.Warning);
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
                startDate = Convert.ToDateTime(tbxMonth2.Text + "-01");
                endDate = Convert.ToDateTime(tbxMonth2.Text + "-01").AddMonths(1).AddDays(-1);
            }
            else
            {
                //startDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy") + "-" + ddlMonth.SelectedValue.ToString() + "-" + ACCOUNTDAY).AddMonths(-1);
                //endDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy") + "-" + ddlMonth.SelectedValue.ToString() + "-" + ACCOUNTDAY).AddDays(-1);
                startDate = Convert.ToDateTime(tbxMonth2.Text + "-" + ACCOUNTDAY).AddMonths(-1);
                endDate = Convert.ToDateTime(tbxMonth2.Text + "-" + ACCOUNTDAY).AddDays(-1);
            }
            //, (case whe
            //            string strSql = string.Format(@"SELECT ' '||GDSEQ 商品编码,GDNAME 商品名称,DEPTNAME 科室,YSTOTAL 预算金额,TYSPERRATE 预算同比金额,HYSPERRATE 预算环比金额,
            //TOTAL 执行金额,TPERRATE 执行同比金额,HPERRATE 执行环比金额,perRate 占比
            //FROM(SELECT A.*, F_GETDEPTNAME(A.DEPTID)DEPTNAME,
            //DECODE(B.HYSTOTAL, 0, 0, NVL(ROUND(A.YSTOTAL / B.HYSTOTAL - 1, 4) * 100, 0)) HYSTOTAL,
            //DECODE(B.HSJJE, 0, 0, NVL(ROUND(A.TOTAL / B.HSJJE - 1, 4) * 100, 0)) HSJJE,
            //DECODE(C.TYSTOTAL, 0, 0, NVL(ROUND(A.YSTOTAL / C.TYSTOTAL - 1, 4) * 100, 0)) TYSTOTAL,
            //DECODE(C.TSJJE, 0, 0, NVL(ROUND(A.TOTAL / C.TSJJE - 1, 4) * 100, 0)) TSJJE,
            //ROUND(DECODE(SUM(NVL(YSTOTAL, 0)), 0, 0, sum(NVL(TOTAL, 0)) / sum(NVL(YSTOTAL, 0))), 4) PERRATE,
            //ROUND(DECODE(SUM(NVL(TYSTOTAL, 0)), 0, 0, sum(NVL(YSTOTAL, 0)) / sum(NVL(TYSTOTAL, 0))), 4) TYSPERRATE,
            //ROUND(DECODE(SUM(NVL(HYSTOTAL, 0)), 0, 0, sum(NVL(YSTOTAL, 0)) / sum(NVL(HYSTOTAL, 0))), 4) HYSPERRATE,
            //ROUND(DECODE(SUM(NVL(TSJJE, 0)), 0, 0, sum(NVL(TOTAL, 0)) / sum(NVL(TSJJE, 0))), 4) TPERRATE,
            //ROUND(DECODE(SUM(NVL(HSJJE, 0)), 0, 0, sum(NVL(TOTAL, 0)) / sum(NVL(HSJJE, 0))), 4) HPERRATE
            //FROM(SELECT GDSEQ, GDNAME, DEPTID, SUM(NVL(YSJE, 0)) YSTOTAL, ROUND(NVL(SUM(SJJE), 0), 2) TOTAL
            //       FROM(SELECT B.GDSEQ, B.GDNAME, A.DEPTID, F_GETYSTOTAL(A.DEPTID, '{0}', '{1}') YSJE, ABS(NVL(SUM(B.HSJE), 0)) AS SJJE
            //                    FROM DAT_XS_DOC A, DAT_XS_COM B
            //                    WHERE A.SEQNO = B.SEQNO
            //                          AND A.FLAG IN('Y', 'G')
            //                          AND A.SHRQ >= TO_DATE('{0}', 'YYYY-MM-dd')
            //                          AND A.SHRQ < TO_DATE('{1}', 'YYYY-MM-dd') + 1
            //                          GROUP BY GDSEQ, GDNAME, DEPTID)
            //       GROUP BY GDSEQ, GDNAME, DEPTID) A,

            //           (SELECT GDSEQ, GDNAME, DEPTID, SUM(NVL(YSJE, 0)) HYSTOTAL, ROUND(NVL(SUM(SJJE), 0), 2) HSJJE
            //                   FROM(SELECT  GDSEQ, GDNAME, A.DEPTID, F_GETYSTOTAL(A.DEPTID, '{4}', '{5}') YSJE, ABS(NVL(SUM(B.HSJE), 0)) AS SJJE
            //                                FROM DAT_XS_DOC A, DAT_XS_COM B
            //                                WHERE A.SEQNO = B.SEQNO
            //                                      AND A.FLAG IN('Y', 'G')
            //                                      AND A.SHRQ >= TO_DATE('{4}', 'YYYY-MM-dd')
            //                                      AND A.SHRQ < TO_DATE('{5}', 'YYYY-MM-dd') + 1
            //                                     GROUP BY GDSEQ, GDNAME, DEPTID)
            //             GROUP BY GDSEQ, GDNAME, DEPTID) B,

            //          (SELECT GDSEQ, GDNAME, DEPTID, SUM(NVL(YSJE, 0)) TYSTOTAL, ROUND(NVL(SUM(SJJE), 0), 2) TSJJE
            //                  FROM(SELECT GDSEQ, GDNAME, A.DEPTID, F_GETYSTOTAL(A.DEPTID, '{2}', '{3}') YSJE, ABS(NVL(SUM(B.HSJE), 0)) AS SJJE
            //                                FROM DAT_XS_DOC A, DAT_XS_COM B
            //                                WHERE A.SEQNO = B.SEQNO
            //                                      AND A.FLAG IN('Y', 'G')
            //                                      AND A.SHRQ >= TO_DATE('{2}', 'YYYY-MM-dd')
            //                                      AND A.SHRQ < TO_DATE('{3}', 'YYYY-MM-dd') + 1
            //                                     GROUP BY GDSEQ, GDNAME, DEPTID)
            //           GROUP BY GDSEQ,GDNAME,DEPTID) C
            // WHERE A.DEPTID = B.DEPTID(+)
            //       AND A.DEPTID = C.DEPTID(+)", startDate.ToString("yyyy - MM - dd"), endDate.ToString("yyyy - MM - dd"), startDate.AddYears(-1).ToString("yyyy - MM - dd"), endDate.AddYears(-1).ToString("yyyy - MM - dd"), startDate.AddMonths(-1).ToString("yyyy - MM - dd"), endDate.AddMonths(-1).ToString("yyyy - MM - dd"));
            //            if (!string.IsNullOrEmpty(ddlDEPTID.SelectedValue))
            //            {

            //                strSql += " AND A.DeptID='" + ddlDEPTID.SelectedValue + "'";
            //            }
            //            strSql += "GROUP BY A.DEPTID,A.YSTOTAL,A.TOTAL,HYSTOTAL,HSJJE,TYSTOTAL,TSJJE,A.GDSEQ,A.GDNAME ORDER BY A.TOTAL DESC)";

            string strSqlAll = string.Format(@"SELECT ' '||GDSEQ 商品编码,GDNAME 商品名称,DEPTNAME 科室,YSTOTAL 预算金额,TYSPERRATE|| '%' 预算同比金额,
       HYSPERRATE 预算环比金额,
       TOTAL 执行金额,
       TPERRATE || '%' 执行同比金额,
       HPERRATE || '%' 执行环比金额,
       perRate*100|| '%' 占比
FROM （SELECT  E.GDSEQ ,E.GDNAME,E.DEPTID,NVL(D.YSTOTAL,0) YSTOTAL,NVL(A.TOTAL,0) TOTAL,
       F_GETDEPTNAME(E.DEPTID) DEPTNAME,
NVL(B.HYSTOTAL, 0) HYSTOTAL,
       NVL(B.HSJJE, 0) HSJJE,
       NVL(C.TSJJE, 0) TSJJE,
       NVL(C.TYSTOTAL, 0) TYSTOTAL,
       ROUND(DECODE(SUM(NVL(D.YSTOTAL, 0)),
                    0,
                    0,
                    sum(NVL(TOTAL, 0)) / sum(NVL(D.YSTOTAL, 0))),
             4) PERRATE,
       DECODE(C.TYSTOTAL,
              0,
              0,
              NVL(ROUND(D.YSTOTAL / C.TYSTOTAL - 1, 4) * 100, 0)) TYSPERRATE,
       DECODE(B.HYSTOTAL,
              0,
              0,
              NVL(ROUND(D.YSTOTAL / B.HYSTOTAL - 1, 4) * 100, 0)) HYSPERRATE,
       DECODE(C.TSJJE, 0, 0, NVL(ROUND(A.TOTAL / C.TSJJE - 1, 4) * 100, 0)) TPERRATE,
       DECODE(B.HSJJE, 0, 0, NVL(ROUND(A.TOTAL / B.HSJJE - 1, 4) * 100, 0)) HPERRATE      
FROM (SELECT GDSEQ,GDNAME,DEPTID,SUM(NVL(YSJE, 0)) YSTOTAL,ROUND(NVL(SUM(SJJE), 0), 2) TOTAL
       FROM (SELECT B.GDSEQ,B.GDNAME,A.DEPTID,F_GETGDYS(A.DEPTID,B.GDSEQ,'{0}', '{1}') YSJE,ABS(NVL(SUM(B.HSJE), 0)) AS SJJE
                    FROM DAT_XS_DOC A, DAT_XS_COM B
                    WHERE A.SEQNO = B.SEQNO
                          AND A.FLAG IN ('Y', 'G')
                          AND A.SHRQ >= TO_DATE('{0}', 'YYYY-MM-dd') 
                          AND A.SHRQ<TO_DATE('{1}', 'YYYY-MM-dd') + 1  AND A.DEPTID = '{6}' 
                          GROUP BY GDSEQ,GDNAME,DEPTID)
       GROUP BY GDSEQ,GDNAME,DEPTID) A,

           (SELECT GDSEQ,GDNAME,DEPTID, SUM(NVL(YSJE, 0)) HYSTOTAL, ROUND(NVL(SUM(SJJE), 0), 2) HSJJE
                   FROM (SELECT  GDSEQ,GDNAME,A.DEPTID,F_GETGDYS(A.DEPTID,B.GDSEQ,'{4}', '{5}') YSJE,ABS(NVL(SUM(B.HSJE), 0)) AS SJJE
                                FROM DAT_XS_DOC A, DAT_XS_COM B
                                WHERE A.SEQNO = B.SEQNO
                                      AND A.FLAG IN ('Y', 'G')
                                      AND A.SHRQ >= TO_DATE('{4}', 'YYYY-MM-dd') 
                                      AND A.SHRQ <TO_DATE('{5}', 'YYYY-MM-dd') + 1  AND A.DEPTID = '{6}' 
                                     GROUP BY GDSEQ,GDNAME,DEPTID)
             GROUP BY GDSEQ,GDNAME,DEPTID) B,

          (SELECT GDSEQ,GDNAME,DEPTID, SUM(NVL(YSJE, 0)) TYSTOTAL, ROUND(NVL(SUM(SJJE), 0), 2) TSJJE
                  FROM ( SELECT GDSEQ,GDNAME,A.DEPTID,F_GETGDYS(A.DEPTID,B.GDSEQ, '{2}', '{3}') YSJE,ABS(NVL(SUM(B.HSJE), 0)) AS SJJE
                                FROM DAT_XS_DOC A, DAT_XS_COM B
                                WHERE A.SEQNO = B.SEQNO
                                      AND A.FLAG IN ('Y', 'G')
                                      AND A.SHRQ >= TO_DATE('{2}', 'YYYY-MM-dd')
                                      AND A.SHRQ <TO_DATE('{3}', 'YYYY-MM-dd') + 1 AND A.DEPTID = '{6}' 
                                     GROUP BY GDSEQ,GDNAME,DEPTID)
           GROUP BY GDSEQ,GDNAME,DEPTID) C,
(SELECT  A.DEPTID,B.GDSEQ,B.GDNAME,SUM(B.HSJE) YSTOTAL
          FROM DAT_YS_DOC A, DAT_YS_COM B
         WHERE A.FLAG = 'S'
           AND A.SEQNO = B.SEQNO
           AND A.SHRQ BETWEEN TO_DATE('{0}', 'YYYY-MM-dd') AND
               TO_DATE('{1}', 'YYYY-MM-dd') - 1  AND A.DEPTID = '{6}' 
               GROUP BY A.DEPTID,B.GDSEQ,B.GDNAME) D,
         (SELECT DISTINCT A.GDSEQ,B.GDNAME, A.DEPTID FROM DOC_GOODSCFG A,DOC_GOODS B WHERE A.GDSEQ=B.GDSEQ) E
 WHERE E.GDSEQ=D.GDSEQ(+)
   AND E.GDSEQ=A.GDSEQ(+)
   AND E.GDSEQ = B.GDSEQ(+)
   AND E.GDSEQ = C.GDSEQ(+)
   AND E.DEPTID = '{6}'
   AND (D.YSTOTAL>0 OR A.TOTAL>0 OR HYSTOTAL>0 OR HSJJE>0 OR TYSTOTAL>0 OR TSJJE>0) ", startDate.ToString("yyyy - MM - dd"), endDate.ToString("yyyy - MM - dd"), startDate.AddYears(-1).ToString("yyyy - MM - dd"), endDate.AddYears(-1).ToString("yyyy - MM - dd"), startDate.AddMonths(-1).ToString("yyyy - MM - dd"), endDate.AddMonths(-1).ToString("yyyy - MM - dd"), ddlDEPTID.SelectedValue);
            strSqlAll += @"GROUP BY E.DEPTID,
          D.YSTOTAL,
          A.TOTAL,
          HYSTOTAL,
          HSJJE,
          TYSTOTAL,
          TSJJE,
          E.GDSEQ,
          E.GDNAME ORDER BY NVL(A.TOTAL,0) DESC )";
            ExcelHelper.ExportByWeb(DbHelperOra.Query(strSqlAll).Tables[0], "单品" + tbxMonth2.Text + "月期间预算执行情况分析表", string.Format("单品" + tbxMonth2.Text + "月预算执行情况分析表_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));

        }
        public string GetDiffDeptPeriodVal(DateTime dt1, DateTime dt2)
        {


            string strSqlAll = string.Format(@"SELECT A.*,F_GETDEPTNAME(A.DEPTID)DEPTNAME,
NVL(B.HYSTOTAL,0)HYSTOTAL,
NVL(B.HSJJE,0)HSJJE,
NVL(C.TSJJE,0)TSJJE,
NVL(C.TYSTOTAL,0) TYSTOTAL,
ROUND(DECODE(SUM(NVL(YSTOTAL, 0)),0,0,sum(NVL(TOTAL, 0)) / sum(NVL(YSTOTAL, 0))),4) PERRATE,
DECODE(C.TYSTOTAL,0,0,NVL(ROUND(A.YSTOTAL /C.TYSTOTAL - 1, 4) , 0))TYSPERRATE,
DECODE(B.HYSTOTAL,0,0,NVL(ROUND(A.YSTOTAL /B.HYSTOTAL - 1, 4), 0))HYSPERRATE,
DECODE(C.TSJJE,0,0,NVL(ROUND(A.TOTAL /C.TSJJE - 1, 4) , 0)) TPERRATE,
DECODE(B.HSJJE,0,0,NVL(ROUND(A.TOTAL /B.HSJJE - 1, 4) , 0))HPERRATE       
FROM (SELECT 
        DEPTID,
       SUM(NVL(YSJE, 0)) YSTOTAL,
       ROUND(NVL(SUM(SJJE), 0), 2) TOTAL
       FROM (SELECT A.DEPTID,F_GETYSTOTAL(A.DEPTID, '{0}', '{1}') YSJE,ABS(NVL(SUM(B.HSJE), 0)) AS SJJE
                    FROM DAT_XS_DOC A, DAT_XS_COM B
                    WHERE A.SEQNO = B.SEQNO
                          AND A.FLAG IN ('Y', 'G')
                          AND A.SHRQ >= TO_DATE('{0}', 'YYYY-MM-dd') 
                          AND A.SHRQ<TO_DATE('{1}', 'YYYY-MM-dd') + 1
                          GROUP BY DEPTID)
       GROUP BY DEPTID) A,

           (SELECT DEPTID, SUM(NVL(YSJE, 0)) HYSTOTAL, ROUND(NVL(SUM(SJJE), 0), 2) HSJJE
                   FROM (SELECT A.DEPTID,F_GETYSTOTAL(A.DEPTID, '{4}', '{5}') YSJE,ABS(NVL(SUM(B.HSJE), 0)) AS SJJE
                                FROM DAT_XS_DOC A, DAT_XS_COM B
                                WHERE A.SEQNO = B.SEQNO
                                      AND A.FLAG IN ('Y', 'G')
                                      AND A.SHRQ >= TO_DATE('{4}', 'YYYY-MM-dd') 
                                      AND A.SHRQ <TO_DATE('{5}', 'YYYY-MM-dd') + 1
                                     GROUP BY DEPTID)
             GROUP BY DEPTID) B,

          (SELECT DEPTID, SUM(NVL(YSJE, 0)) TYSTOTAL, ROUND(NVL(SUM(SJJE), 0), 2) TSJJE
                  FROM ( SELECT A.DEPTID,F_GETYSTOTAL(A.DEPTID, '{2}', '{3}') YSJE,ABS(NVL(SUM(B.HSJE), 0)) AS SJJE
                                FROM DAT_XS_DOC A, DAT_XS_COM B
                                WHERE A.SEQNO = B.SEQNO
                                      AND A.FLAG IN ('Y', 'G')
                                      AND A.SHRQ >= TO_DATE('{2}', 'YYYY-MM-dd')
                                      AND A.SHRQ <TO_DATE('{3}', 'YYYY-MM-dd') + 1
                                     GROUP BY DEPTID)
           GROUP BY DEPTID) C
 WHERE A.DEPTID = B.DEPTID(+)
       AND A.DEPTID = C.DEPTID(+) ", dt1.ToString("yyyy-MM-dd"), dt2.ToString("yyyy-MM-dd"), dt1.AddYears(-1).ToString("yyyy-MM-dd"), dt2.AddYears(-1).ToString("yyyy-MM-dd"), dt1.AddMonths(-1).ToString("yyyy-MM-dd"), dt2.AddMonths(-1).ToString("yyyy-MM-dd"));
            if (!string.IsNullOrEmpty(lstDEPTID.SelectedValue))
            {

                strSqlAll += " AND A.DeptID='" + lstDEPTID.SelectedValue + "'";
            }
            strSqlAll += "GROUP BY A.DEPTID,A.YSTOTAL,A.TOTAL,HYSTOTAL,HSJJE,TYSTOTAL,TSJJE";
            strSqlAll += String.Format(" ORDER BY A.TOTAL DESC");

            return strSqlAll;


        }
        public string GetDiffShpPeriodVal(DateTime dt1, DateTime dt2)
        {
            string strSqlAll = string.Format(@"SELECT  E.GDSEQ,E.GDNAME,E.DEPTID,NVL(D.YSTOTAL,0) YSTOTAL,NVL(A.TOTAL,0) TOTAL,
       F_GETDEPTNAME(E.DEPTID) DEPTNAME,
NVL(B.HYSTOTAL, 0) HYSTOTAL,
       NVL(B.HSJJE, 0) HSJJE,
       NVL(C.TSJJE, 0) TSJJE,
       NVL(C.TYSTOTAL, 0) TYSTOTAL,
       ROUND(DECODE(SUM(NVL(D.YSTOTAL, 0)),
                    0,
                    0,
                    sum(NVL(TOTAL, 0)) / sum(NVL(D.YSTOTAL, 0))),
             4) PERRATE,
       DECODE(C.TYSTOTAL,
              0,
              0,
              NVL(ROUND(D.YSTOTAL / C.TYSTOTAL - 1, 4) , 0)) TYSPERRATE,
       DECODE(B.HYSTOTAL,
              0,
              0,
              NVL(ROUND(D.YSTOTAL / B.HYSTOTAL - 1, 4) , 0)) HYSPERRATE,
       DECODE(C.TSJJE, 0, 0, NVL(ROUND(A.TOTAL / C.TSJJE - 1, 4), 0)) TPERRATE,
       DECODE(B.HSJJE, 0, 0, NVL(ROUND(A.TOTAL / B.HSJJE - 1, 4), 0)) HPERRATE      
FROM (SELECT GDSEQ,GDNAME,DEPTID,SUM(NVL(YSJE, 0)) YSTOTAL,ROUND(NVL(SUM(SJJE), 0), 2) TOTAL
       FROM (SELECT B.GDSEQ,B.GDNAME,A.DEPTID,F_GETGDYS(A.DEPTID,B.GDSEQ,'{0}', '{1}') YSJE,ABS(NVL(SUM(B.HSJE), 0)) AS SJJE
                    FROM DAT_XS_DOC A, DAT_XS_COM B
                    WHERE A.SEQNO = B.SEQNO
                          AND A.FLAG IN ('Y', 'G')
                          AND A.SHRQ >= TO_DATE('{0}', 'YYYY-MM-dd') 
                          AND A.SHRQ<TO_DATE('{1}', 'YYYY-MM-dd') + 1  AND A.DEPTID = '{6}' 
                          GROUP BY GDSEQ,GDNAME,DEPTID)
       GROUP BY GDSEQ,GDNAME,DEPTID) A,

           (SELECT GDSEQ,GDNAME,DEPTID, SUM(NVL(YSJE, 0)) HYSTOTAL, ROUND(NVL(SUM(SJJE), 0), 2) HSJJE
                   FROM (SELECT  GDSEQ,GDNAME,A.DEPTID,F_GETGDYS(A.DEPTID,B.GDSEQ,'{4}', '{5}') YSJE,ABS(NVL(SUM(B.HSJE), 0)) AS SJJE
                                FROM DAT_XS_DOC A, DAT_XS_COM B
                                WHERE A.SEQNO = B.SEQNO
                                      AND A.FLAG IN ('Y', 'G')
                                      AND A.SHRQ >= TO_DATE('{4}', 'YYYY-MM-dd') 
                                      AND A.SHRQ <TO_DATE('{5}', 'YYYY-MM-dd') + 1  AND A.DEPTID = '{6}' 
                                     GROUP BY GDSEQ,GDNAME,DEPTID)
             GROUP BY GDSEQ,GDNAME,DEPTID) B,

          (SELECT GDSEQ,GDNAME,DEPTID, SUM(NVL(YSJE, 0)) TYSTOTAL, ROUND(NVL(SUM(SJJE), 0), 2) TSJJE
                  FROM ( SELECT GDSEQ,GDNAME,A.DEPTID,F_GETGDYS(A.DEPTID,B.GDSEQ, '{2}', '{3}') YSJE,ABS(NVL(SUM(B.HSJE), 0)) AS SJJE
                                FROM DAT_XS_DOC A, DAT_XS_COM B
                                WHERE A.SEQNO = B.SEQNO
                                      AND A.FLAG IN ('Y', 'G')
                                      AND A.SHRQ >= TO_DATE('{2}', 'YYYY-MM-dd')
                                      AND A.SHRQ <TO_DATE('{3}', 'YYYY-MM-dd') + 1 AND A.DEPTID = '{6}' 
                                     GROUP BY GDSEQ,GDNAME,DEPTID)
           GROUP BY GDSEQ,GDNAME,DEPTID) C,
(SELECT  A.DEPTID,B.GDSEQ,B.GDNAME,SUM(B.HSJE) YSTOTAL
          FROM DAT_YS_DOC A, DAT_YS_COM B
         WHERE A.FLAG = 'S'
           AND A.SEQNO = B.SEQNO
           AND A.SHRQ BETWEEN TO_DATE('{0}', 'YYYY-MM-dd') AND
               TO_DATE('{1}', 'YYYY-MM-dd') - 1  AND A.DEPTID = '{6}' 
               GROUP BY A.DEPTID,B.GDSEQ,B.GDNAME) D,
         (SELECT DISTINCT A.GDSEQ,B.GDNAME, A.DEPTID FROM DOC_GOODSCFG A,DOC_GOODS B WHERE A.GDSEQ=B.GDSEQ) E
 WHERE E.GDSEQ=D.GDSEQ(+)
   AND E.GDSEQ=A.GDSEQ(+)
   AND E.GDSEQ = B.GDSEQ(+)
   AND E.GDSEQ = C.GDSEQ(+)
   AND E.DEPTID = '{6}'
   AND (D.YSTOTAL>0 OR A.TOTAL>0 OR HYSTOTAL>0 OR HSJJE>0 OR TYSTOTAL>0 OR TSJJE>0) ", dt1.ToString("yyyy-MM-dd"), dt2.ToString("yyyy-MM-dd"), dt1.AddYears(-1).ToString("yyyy-MM-dd"), dt2.AddYears(-1).ToString("yyyy-MM-dd"), dt1.AddMonths(-1).ToString("yyyy-MM-dd"), dt2.AddMonths(-1).ToString("yyyy-MM-dd"), ddlDEPTID.SelectedValue);
            strSqlAll += @"GROUP BY E.DEPTID,
          D.YSTOTAL,
          A.TOTAL,
          HYSTOTAL,
          HSJJE,
          TYSTOTAL,
          TSJJE,
          E.GDSEQ,
          E.GDNAME ORDER BY NVL(A.TOTAL,0) DESC";
            return strSqlAll;

        }

        protected void GridList_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                for (int icol = 4; icol < 10; icol++)
                {
                    if (e.Values[icol].ToString().Trim().Length == 0)
                    {
                        e.Values[icol] = 0.00;
                    }
                }
            }
        }
        protected void GridGoods_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                for (int icol = 4; icol < 11; icol++)
                {
                    if (e.Values[icol].ToString().Trim().Length == 0)
                    {
                        e.Values[icol] = 0.00;
                    }
                }
            }
        }
    }
}