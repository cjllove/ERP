using XTBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FineUIPro;
using System.Data;
using Newtonsoft.Json.Linq;

namespace ERPProject.ERPQuery
{
    public partial class DeptSy : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 在页面第一次加载时 
                BindDDL();
                DataSearch();
            }
        }
        private void BindDDL()
        {
            dpkDATE1.SelectedDate = DateTime.Now.AddMonths(-3);
            dpkDATE2.SelectedDate = DateTime.Now;
            lisDATE1.SelectedDate = DateTime.Now.AddMonths(-3);
            lisDATE2.SelectedDate = DateTime.Now;
            PubFunc.DdlDataGet("DDL_SYS_DEPTALLRANGE", UserAction.UserID, lstDEPTID, ddlDEPTID);
        }

        private void DataSearch()
        {
            if (PubFunc.StrIsEmpty(dpkDATE1.SelectedDate.ToString()) || PubFunc.StrIsEmpty(dpkDATE2.SelectedDate.ToString()))
            {
                Alert.Show("【输入日期】不正确,请检查！", MessageBoxIcon.Warning);
                return;
            }
            if (dpkDATE1.SelectedDate > dpkDATE2.SelectedDate)
            {
                Alert.Show("【开始日期】不能大于【结束日期】！", MessageBoxIcon.Warning);
                return;
            }
            //string strSql2 = @"(SELECT SUM(B.SL*B.KCADD)
            //            FROM SYS_DEPT A,DAT_GOODSJXC B,DOC_GOODS C
            //            WHERE A.CODE = B.DEPTID  AND B.BILLTYPE = 'SYD' AND B.KCADD < 0
            //            AND B.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd') + 1
            //            AND B.GDSEQ = C.GDSEQ AND A.CODE LIKE NVL('{2}','%')";
            //string strWhere2 = ")"; 
            //if (!PubFunc.StrIsEmpty(ddlISGZ.SelectedValue)) strWhere2 = " AND C.ISGZ = '" + ddlISGZ.SelectedValue + "'" + strWhere2;
            //if (strWhere2.Trim().Length > 0) strSql2 = string.Format(strSql2, dpkDATE1.Text, dpkDATE2.Text, lstDEPTID.SelectedValue) + strWhere2;

            //string strSql3 = @"(SELECT SUM(B.HSJE*B.KCADD)
            //            FROM SYS_DEPT A,DAT_GOODSJXC B,DOC_GOODS C
            //            WHERE A.CODE = B.DEPTID  AND B.BILLTYPE = 'SYD' AND B.KCADD < 0
            //            AND B.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd') + 1
            //             AND B.GDSEQ = C.GDSEQ AND A.CODE LIKE NVL('{2}','%')";
            //string strWhere3 = ")";  
            //if (!PubFunc.StrIsEmpty(ddlISGZ.SelectedValue)) strWhere3 = " AND C.ISGZ = '" + ddlISGZ.SelectedValue + "'" + strWhere3;
            //if (strWhere3.Trim().Length > 0) strSql3 = string.Format(strSql3, dpkDATE1.Text, dpkDATE2.Text, lstDEPTID.SelectedValue) + strWhere3;

            //string strSql = @"SELECT A.CODE,A.NAME,f_getusername(A.MANAGER) USERNAME,SUM(B.SL*B.KCADD) SL
            //                ,SUM(B.HSJE*B.KCADD) HSJE,
            //                ROUND(SUM(B.SL*B.KCADD)/({2}),4) ZBSL
            //                ,ROUND(SUM(B.HSJE*B.KCADD)/({3}),4) ZBJE,
            //                F_GETBL('SYJEHB','{0}','{1}',A.CODE,'','','','{4}') JEHB,F_GETBL('SYJETB','{0}','{1}',A.CODE,'','','','{4}') JETB,
            //                F_GETBL('SYSLHB','{0}','{1}',A.CODE,'','','','{4}') SLHB,F_GETBL('SYSLTB','{0}','{1}',A.CODE,'','','','{4}') SLTB
            //            FROM SYS_DEPT A,DAT_GOODSJXC B,DOC_GOODS C
            //            WHERE A.CODE = B.DEPTID  AND B.BILLTYPE = 'SYD' AND B.KCADD < 0
            //            AND B.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd') + 1
            //             AND B.GDSEQ = C.GDSEQ";
            //string strWhere = "";
            //if (lstDEPTID.SelectedValue.Length > 0) strWhere += " AND A.CODE = '" + lstDEPTID.SelectedValue + "'";
            //if (!PubFunc.StrIsEmpty(ddlISGZ.SelectedValue)) strWhere += " AND C.ISGZ = '" + ddlISGZ.SelectedValue + "'";
            //if (strWhere.Trim().Length > 0) strSql = strSql + strWhere;
            //strSql += " GROUP BY A.CODE,A.NAME,A.MANAGER";
            string strSql = @"SELECT A.*,NVL(B.HBJE,0) HBJE,NVL(B.HBSL,0) HBSL,
NVL(C.TBJE,0)TBJE,NVL(C.TBSL,0) TBSL
 FROM 
(SELECT A.CODE,
       A.NAME,
       f_getusername(A.MANAGER) USERNAME,
       SUM(B.SL * B.KCADD) SL,
       SUM(B.HSJE * B.KCADD) HSJE,
       ROUND(SUM(B.SL * B.KCADD) /
             ((SELECT SUM(B.SL * B.KCADD)
                FROM SYS_DEPT A, DAT_GOODSJXC B, DOC_GOODS C
               WHERE A.CODE = B.DEPTID
                 AND B.BILLTYPE = 'SYD'
                 AND B.KCADD < 0
                 AND B.RQSJ BETWEEN TO_DATE('{0}', 'yyyy-MM-dd') AND
                     TO_DATE('{1}', 'yyyy-MM-dd') + 1
                 AND B.GDSEQ = C.GDSEQ
                 AND A.CODE LIKE NVL('{2}', '%')
                 AND C.ISGZ LIKE NVL('{3}', '%'))),
             4) ZBSL,
       ROUND(SUM(B.HSJE * B.KCADD) /
             ((SELECT SUM(B.HSJE * B.KCADD)
                FROM SYS_DEPT A, DAT_GOODSJXC B, DOC_GOODS C
               WHERE A.CODE = B.DEPTID
                 AND B.BILLTYPE = 'SYD'
                 AND B.KCADD < 0
                 AND B.RQSJ BETWEEN TO_DATE('{0}', 'yyyy-MM-dd') AND
                     TO_DATE('{1}', 'yyyy-MM-dd') + 1
                 AND B.GDSEQ = C.GDSEQ
                 AND A.CODE LIKE NVL('{2}', '%')
                 AND C.ISGZ LIKE NVL('{3}', '%'))),
             4) ZBJE,
       F_GETBL('SYJEHB', '{0}', '{1}', A.CODE, '', '', '', '') JEHB,
       F_GETBL('SYJETB', '{0}', '{1}', A.CODE, '', '', '', '') JETB,
       F_GETBL('SYSLHB', '{0}', '{1}', A.CODE, '', '', '', '') SLHB,
       F_GETBL('SYSLTB', '{0}', '{1}', A.CODE, '', '', '', '') SLTB
  FROM SYS_DEPT A, DAT_GOODSJXC B, DOC_GOODS C
 WHERE A.CODE = B.DEPTID
   AND B.BILLTYPE = 'SYD'
   AND B.KCADD < 0
   AND B.RQSJ BETWEEN TO_DATE('{0}', 'yyyy-MM-dd') AND
       TO_DATE('{1}', 'yyyy-MM-dd') + 1
   AND B.GDSEQ = C.GDSEQ
    AND A.CODE LIKE NVL('{2}', '%')
    AND C.ISGZ LIKE NVL('{3}', '%')
 GROUP BY A.CODE, A.NAME, A.MANAGER)A,
 
 (SELECT A.CODE,
       A.NAME,
       f_getusername(A.MANAGER) USERNAME,
               SUM(B.HSJE * B.KCADD) HBJE,
               SUM(B.SL* B.KCADD) HBSL
          FROM SYS_DEPT A, DAT_GOODSJXC B,DOC_GOODS C
           WHERE A.CODE = B.DEPTID
            AND B.BILLTYPE ='SYD'
            AND B.KCADD<0
            AND B.GDSEQ=C.GDSEQ
           AND B.RQSJ >= TO_DATE('{0}', 'yyyy-MM-dd')-(TO_DATE('{1}', 'yyyy-MM-dd')-TO_DATE('{0}', 'yyyy-MM-dd'))-1 
           AND B.RQSJ < TO_DATE('{0}', 'yyyy-MM-dd') 
           AND B.KCADD < 0
            AND A.CODE LIKE NVL('{2}', '%')
            AND C.ISGZ LIKE NVL('{3}', '%')
         GROUP BY A.CODE, A.NAME, A.MANAGER) B,
         
         (SELECT A.CODE,
       A.NAME,
       f_getusername(A.MANAGER) USERNAME,
               SUM(B.HSJE* B.KCADD) TBJE,
               SUM(B.SL* B.KCADD) TBSL
          FROM SYS_DEPT A, DAT_GOODSJXC B,DOC_GOODS C
           WHERE A.CODE = B.DEPTID
            AND B.BILLTYPE ='SYD'
            AND B.KCADD<0
            AND B.GDSEQ=C.GDSEQ
           AND B.RQSJ >=
               ADD_MONTHS(TO_DATE('{0}', 'yyyy-MM-dd'), -12)
           AND B.RQSJ <
               ADD_MONTHS((TO_DATE('{1}', 'yyyy-MM-dd') + 1), -12)
           AND B.KCADD < 0
            AND A.CODE LIKE NVL('{2}', '%')
            AND C.ISGZ LIKE NVL('{3}', '%')
         GROUP BY A.CODE, A.NAME, A.MANAGER) C
  WHERE A.CODE=B.CODE(+) AND A.CODE=C.CODE(+)  ";
            string sortField = GridGoods.SortField;
            string sortDirection = GridGoods.SortDirection;
            string ss = string.Format(strSql, dpkDATE1.Text, dpkDATE2.Text, lstDEPTID.SelectedValue,ddlISGZ.SelectedValue);

            DataTable dt = DbHelperOra.QueryForTable(string.Format(strSql, dpkDATE1.Text, dpkDATE2.Text, lstDEPTID.SelectedValue, ddlISGZ.SelectedValue) + String.Format(" ORDER BY {0} {1}", sortField, sortDirection));
            GridGoods.DataSource = dt;
            GridGoods.DataBind();

            JObject summary = new JObject();
            hfdArray.Text = "";
            hfdArrayVal.Text = "";
            hfdArrayVal2.Text = "";
            if (dt.Rows.Count == 0)
            {
                hfdArrayVal.Text = "无数据" + "$" + "无数据" + ",";
            }
            Decimal SL = 0, HSJE = 0, total = 0, totaltb = 0, totalhb = 0;
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                SL += Convert.ToDecimal(dr["SL"]);
                HSJE += Convert.ToDecimal(dr["HSJE"]);
                //totaltb += Convert.ToDecimal(dr["HSJE"]) * Convert.ToDecimal(dr["JETB"]);
                //totalhb += Convert.ToDecimal(dr["HSJE"]) * Convert.ToDecimal(dr["JEHB"]);
                totaltb += Convert.ToDecimal(dr["TBJE"]);
                totalhb += Convert.ToDecimal(dr["HBJE"]);
                if (i > 9)
                {
                    total += Convert.ToDecimal(dr["HSJE"].ToString());
                }
                else
                {
                    hfdArray.Text += dr["NAME"] + ",";
                    hfdArrayVal.Text += dr["HSJE"] + "$" + dr["NAME"] + ",";
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
            hfdArrayVal2.Text = HSJE.ToString() + "," + (Math.Round(totaltb, 2)).ToString() + "," + (Math.Round(totalhb, 2)).ToString();
            summary.Add("NAME", "本页合计");
            summary.Add("SL", SL.ToString("F2"));
            summary.Add("HSJE", HSJE.ToString("F2"));
            GridGoods.SummaryData = summary;
            PageContext.RegisterStartupScript("getEcharsData2();getEcharsData();");
        }
        protected void btnSch_Click(object sender, EventArgs e)
        {
            if (PubFunc.StrIsEmpty(lisDATE1.SelectedDate.ToString()) || PubFunc.StrIsEmpty(lisDATE2.SelectedDate.ToString()))
            {
                Alert.Show("【输入日期】不正确,请检查！", MessageBoxIcon.Warning);
                return;
            }
            if (lisDATE1.SelectedDate > lisDATE2.SelectedDate)
            {
                Alert.Show("【开始日期】不能大于【结束日期】！", MessageBoxIcon.Warning);
                return;
            }
            string strSql = @"SELECT A.SL SL,A.HSJE HSJE,B.GDSEQ,B.GDNAME,B.GDSPEC,f_getunitname(B.UNIT) UNITNAME,f_getproducername(B.PRODUCER) PRODUCERNAME,
                                   B.HSJJ,B.PIZNO,
                                   ROUND((A.SL/(SELECT SUM(AA.SL*AA.KCADD)
                                    FROM DAT_GOODSJXC AA
                                    WHERE AA.RQSJ >=TO_DATE('{0}','yyyy-MM-dd') AND AA.RQSJ<TO_DATE('{1}','yyyy-MM-dd') + 1 AND AA.BILLTYPE='SYD' AND AA.KCADD < 0  AND AA.DEPTID = A.DEPTID)),4) SLZB,
                                    ROUND((A.HSJE/(SELECT SUM(AA.HSJE*AA.KCADD)
                                    FROM DAT_GOODSJXC AA
                                    WHERE AA.RQSJ >= TO_DATE('{0}','yyyy-MM-dd') AND AA.RQSJ<TO_DATE('{1}','yyyy-MM-dd') + 1 AND AA.BILLTYPE='SYD' AND AA.KCADD < 1  AND AA.DEPTID = A.DEPTID)),4) JEZB,
                                    F_GETBL('SYJEHB','{0}','{1}',A.DEPTID,B.GDSEQ,'','','{3}') JEHB,F_GETBL('SYJETB','{0}','{1}',A.DEPTID,B.GDSEQ,'','','{3}') JETB,
                                    F_GETBL('SYSLHB','{0}','{1}',A.DEPTID,B.GDSEQ,'','','{3}') SLHB,F_GETBL('SYSLTB','{0}','{1}',A.DEPTID,B.GDSEQ,'','','{3}') SLTB
                            FROM (SELECT A.DEPTID,A.GDSEQ,SUM(A.SL*A.KCADD) SL
                                    ,SUM(A.HSJE*A.KCADD) HSJE
                            FROM DAT_GOODSJXC A
                            WHERE A.RQSJ >=TO_DATE('{0}','yyyy-MM-dd') AND A.RQSJ<TO_DATE('{1}','yyyy-MM-dd') + 1 AND A.BILLTYPE = 'SYD' AND A.KCADD < 0
                            {2}
                            GROUP BY A.DEPTID,A.GDSEQ) A,DOC_GOODS B
                            WHERE A.GDSEQ = B.GDSEQ";
            string strWhere = "";
            string strWhere2 = "";
            if (ddlDEPTID.SelectedValue.Length > 1) strWhere2 += " AND A.DEPTID = '" + ddlDEPTID.SelectedValue + "'";
            if (lisGDSEQ.Text.Trim().Length > 0)
            {
                strWhere += String.Format(" AND (B.GDSEQ LIKE '%{0}%' OR B.GDNAME LIKE '%{0}%' OR B.ZJM LIKE '%{0}%')", lisGDSEQ.Text.Trim());
            }
            if (lstISGZ.SelectedValue.Length > 0) strWhere += " AND B.ISGZ = '" + lstISGZ.SelectedValue + "'";
            if (strWhere.Trim().Length > 0) strSql = strSql + strWhere;
            int total = 0;
            string sortField = GridList.SortField;
            string sortDirection = GridList.SortDirection;
            string SS = string.Format(strSql, dpkDATE1.Text, dpkDATE2.Text, strWhere2, lstISGZ.SelectedValue);
            DataTable dtData = PubFunc.DbGetPage(GridList.PageIndex, GridList.PageSize, string.Format(strSql, dpkDATE1.Text, dpkDATE2.Text, strWhere2, lstISGZ.SelectedValue) + String.Format(" ORDER BY {0} {1}", sortField, sortDirection), ref total);
            GridList.RecordCount = total;
            GridList.DataSource = dtData;
            GridList.DataBind();

            Decimal SL = 0, HSJE = 0;
            JObject summary = new JObject();
            foreach (DataRow dr in dtData.Rows)
            {
                SL += Convert.ToDecimal(dr["SL"]);
                HSJE += Convert.ToDecimal(dr["HSJE"]);
            }
            summary.Add("GDNAME", "本页合计");
            summary.Add("SL2", SL.ToString("F2"));
            summary.Add("HSJE2", HSJE.ToString("F2"));
            GridList.SummaryData = summary;
        }
        protected void GridGoods_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridGoods.PageIndex = e.NewPageIndex;
            DataSearch();
        }
        protected void btSearch_Click(object sender, EventArgs e)
        {
            DataSearch();
        }
        protected void btClear_Click(object sender, EventArgs e)
        {
            if (TabStrip1.ActiveTabIndex == 0)
            {
                dpkDATE1.SelectedDate = DateTime.Now.AddMonths(-3);
                dpkDATE2.SelectedDate = DateTime.Now;
                lstDEPTID.SelectedValue = "";
                ddlISGZ.SelectedValue = "";
            }
            if (TabStrip1.ActiveTabIndex == 1)
            {
                lisDATE1.SelectedDate = DateTime.Now.AddMonths(-3);
                lisDATE2.SelectedDate = DateTime.Now;
                lisGDSEQ.Text = String.Empty;
                lstISGZ.SelectedValue = "";
            }
        }
        protected void btExport_Click(object sender, EventArgs e)
        {
            if (TabStrip1.ActiveTabIndex == 0)
            {
                if (GridGoods.Rows.Count < 1)
                {
                    Alert.Show("没有数据,无法导出！");
                    return;
                }
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment; filename=科室排行.xls");
                Response.ContentType = "application/excel";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.Write(PubFunc.GridToHtml(GridGoods));
                Response.End();
                btnExp.Enabled = true;
            }
            if (TabStrip1.ActiveTabIndex == 1)
            {
                if (GridList.Rows.Count < 1)
                {
                    Alert.Show("没有数据,无法导出！");
                    return;
                }
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment; filename=科室明细排行.xls");
                Response.ContentType = "application/excel";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.Write(PubFunc.GridToHtml(GridList));
                Response.End();
                btnExp.Enabled = true;
            }
        }

        protected void GridGoods_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            ddlDEPTID.SelectedValue = GridGoods.DataKeys[e.RowIndex][0].ToString();
            lstISGZ.SelectedValue = ddlISGZ.SelectedValue;
            lisDATE1.SelectedDate = dpkDATE1.SelectedDate;
            lisDATE2.SelectedDate = dpkDATE2.SelectedDate;
            TabStrip1.ActiveTabIndex = 1;
            lisGDSEQ.Text = String.Empty;
            btnSch_Click(null, null);
        }

        protected void GridList_Sort(object sender, GridSortEventArgs e)
        {
            btnSch_Click(null, null);
        }

        protected void GridGoods_Sort(object sender, GridSortEventArgs e)
        {
            DataSearch();
        }
    }
}