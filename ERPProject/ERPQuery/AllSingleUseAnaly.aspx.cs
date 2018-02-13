﻿using XTBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FineUIPro;
using System.Data;
using Newtonsoft.Json.Linq;
using XTBase.Utilities;
using System.Text;

namespace ERPProject.ERPQuery
{
    public partial class AllSingleUseAnaly : PageBase
    {
        decimal firtotal = 0;
        decimal sectotal = 0;
        public AllSingleUseAnaly()
        {
            ISCHECK = false;
        }
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
            PubFunc.DdlDataGet("DDL_SYS_DEPTRANGE", UserAction.UserID, lstDEPTID, ddlDEPTID);
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
            string strSql2 = @"(SELECT SUM(
                                        DECODE(B.BILLTYPE,
                                                 'XST',
                                                -ABS(B.SL),
                                              /*  'LTD',-ABS(B.SL),'DST',-ABS(B.SL),*/
                                                ABS(B.SL)
                                                )
                                           )
                        FROM SYS_DEPT A,DAT_GOODSJXC B,DOC_GOODS C
                        WHERE A.CODE = B.DEPTID AND A.TYPE = '3' 
                        AND B.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd') + 1
                        AND B.KCADD < 0 AND B.GDSEQ = C.GDSEQ";
            string strWhere2 = ")";
            if (strWhere2.Trim().Length > 0) strSql2 = string.Format(strSql2, dpkDATE1.Text, dpkDATE2.Text) + strWhere2;

            string strSql3 = @"(SELECT SUM(
                                        DECODE(B.BILLTYPE,
                                                'XST',
                                                -ABS(B.HSJE),
                                               /* 'LTD',-ABS(B.HSJE),'DST',-ABS(B.HSJE),*/
                                                ABS(B.HSJE)
                                               )
                                           )
                        FROM SYS_DEPT A,DAT_GOODSJXC B,DOC_GOODS C
                        WHERE A.CODE = B.DEPTID AND A.TYPE = '3' 
                        AND B.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd') + 1
                        AND B.KCADD < 0 AND B.GDSEQ = C.GDSEQ";
            string strWhere3 = ")";
            if (strWhere3.Trim().Length > 0) strSql3 = string.Format(strSql3, dpkDATE1.Text, dpkDATE2.Text) + strWhere3;

            //            string strSql = @"SELECT  C.GDSEQ,   C.GDNAME,SUM(DECODE(B.BILLTYPE,'XST',-ABS(B.SL),'LTD',-ABS(B.SL),'DST',-ABS(B.SL),ABS(B.SL))) SL
            //                            ,SUM(DECODE(B.BILLTYPE,'XST',-ABS(B.HSJE),'LTD',-ABS(B.HSJE),'DST',-ABS(B.HSJE),ABS(B.HSJE))) HSJE,
            //                            ROUND(SUM(DECODE(B.BILLTYPE,'XST',-ABS(B.SL),'LTD',-ABS(B.SL),'DST',-ABS(B.SL),ABS(B.SL)))/{2},4) ZBSL
            //                            ,ROUND(SUM(DECODE(B.BILLTYPE,'XST',-ABS(B.HSJE),'LTD',-ABS(B.HSJE),'DST',-ABS(B.HSJE),ABS(B.HSJE)))/{3},4) ZBJE,
            //                            F_GETBL('DPJEHB','{0}','{1}','{4}',C.GDSEQ,'','','') JEHB,F_GETBL('DPJETB','{0}','{1}','{4}',C.GDSEQ,'','','') JETB,
            //                            F_GETBL('DPSLHB','{0}','{1}','{4}',C.GDSEQ,'','','') SLHB,F_GETBL('DPSLTB','{0}','{1}','{4}',C.GDSEQ,'','','') SLTB
            //                        FROM SYS_DEPT A,DAT_GOODSJXC B,DOC_GOODS C
            //                        WHERE A.CODE = B.DEPTID AND A.TYPE = '3' 
            //                        AND B.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd') + 1
            //                        AND B.KCADD < 0 AND B.GDSEQ = C.GDSEQ";
            string strSql = @"SELECT A.*,NVL(B.HBSLZ,0)HBSLZ,NVL(B.HBJEZ,0)HBJEZ,NVL(C.TBSLZ,0)TBSLZ,NVL(C.TBJEZ,0)TBJEZ FROM (SELECT C.GDSEQ,
                                           C.GDNAME,
                                           SUM(DECODE(B.BILLTYPE,
                                                      'XST',
                                                      -ABS(B.SL),
                                                     /* 'LTD',
                                                      -ABS(B.SL),
                                                      'DST',
                                                      -ABS(B.SL),*/
                                                      ABS(B.SL))) SL,
                                           SUM(DECODE(B.BILLTYPE,
                                                      'XST',
                                                      -ABS(B.HSJE),
                                                    /*  'LTD',
                                                      -ABS(B.HSJE),
                                                      'DST',
                                                      -ABS(B.HSJE),*/
                                                      ABS(B.HSJE))) HSJE,
                                           ROUND(SUM(DECODE(B.BILLTYPE,
                                                            'XST',
                                                            -ABS(B.SL),
                                                         /*   'LTD',
                                                            -ABS(B.SL),
                                                            'DST',
                                                            -ABS(B.SL),*/
                                                            ABS(B.SL))) /
                                                 (SELECT SUM(DECODE(B.BILLTYPE,
                                                                    'XST',
                                                                    -ABS(B.SL),
                                                                   /* 'LTD',
                                                                    -ABS(B.SL),
                                                                    'DST',
                                                                    -ABS(B.SL),*/
                                                                    ABS(B.SL)))
                                                    FROM SYS_DEPT A, DAT_GOODSJXC B, DOC_GOODS C
                                                   WHERE A.CODE = B.DEPTID
                                                     AND A.TYPE = '3'
                                                     AND B.RQSJ BETWEEN TO_DATE('{0}', 'yyyy-MM-dd') AND
                                                         TO_DATE('{1}', 'yyyy-MM-dd') + 1
                                                     AND B.KCADD < 0
                                                     AND B.GDSEQ = C.GDSEQ
                                                      AND A.CODE LIKE NVL('{2}','%')
                                                     AND B.BILLTYPE IN ('XSD', 'XSG', 'DSH', 'XST')),
                                                 4) ZBSL,
                                           ROUND(SUM(DECODE(B.BILLTYPE,
                                                            'XST',
                                                            -ABS(B.HSJE),
                                                          /*  'LTD',
                                                            -ABS(B.HSJE),
                                                            'DST',
                                                            -ABS(B.HSJE),*/
                                                            ABS(B.HSJE))) /
                                                 (SELECT SUM(DECODE(B.BILLTYPE,
                                                                    'XST',
                                                                    -ABS(B.HSJE),
                                                                  /*  'LTD',
                                                                    -ABS(B.HSJE),
                                                                    'DST',
                                                                    -ABS(B.HSJE),*/
                                                                    ABS(B.HSJE)))
                                                    FROM SYS_DEPT A, DAT_GOODSJXC B, DOC_GOODS C
                                                   WHERE A.CODE = B.DEPTID
                                                     AND A.TYPE = '3'
                                                     AND B.RQSJ BETWEEN TO_DATE('{0}', 'yyyy-MM-dd') AND
                                                         TO_DATE('{1}', 'yyyy-MM-dd') + 1
                                                     AND B.KCADD < 0
                                                     AND B.GDSEQ = C.GDSEQ
                                                     AND A.CODE LIKE NVL('{2}','%')
                                                    AND B.BILLTYPE IN ('XSD', 'XSG', 'DSH', 'XST')),
                                                 4) ZBJE      
                                      FROM SYS_DEPT A, DAT_GOODSJXC B, DOC_GOODS C
                                     WHERE A.CODE = B.DEPTID
                                       AND A.TYPE = '3'
                                       AND B.RQSJ BETWEEN TO_DATE('{0}', 'yyyy-MM-dd') AND
                                           TO_DATE('{1}', 'yyyy-MM-dd') + 1
                                       AND B.KCADD < 0
                                       AND B.GDSEQ = C.GDSEQ
                                         AND A.CODE LIKE NVL('{2}','%')                                       
                                       AND B.BILLTYPE IN ('XSD','XSG','DSH','XST')
                                        GROUP BY C.GDSEQ, C.GDNAME) A,  -------------============================
                                       (SELECT C.GDSEQ, SUM(DECODE(B.BILLTYPE,
                                                      'XST',
                                                      -ABS(B.SL),
                                                    /*  'LTD',
                                                      -ABS(B.SL),
                                                      'DST',
                                                      -ABS(B.SL),*/
                                                      ABS(B.SL))) HBSLZ, SUM(DECODE(B.BILLTYPE,
                                                      'XST',
                                                      -ABS(B.HSJE),
                                                    /*  'LTD',
                                                      -ABS(B.HSJE),
                                                      'DST',
                                                      -ABS(B.HSJE),*/
                                                      ABS(B.HSJE))) HBJEZ
                                                       FROM SYS_DEPT A, DAT_GOODSJXC B, DOC_GOODS C
                                     WHERE A.CODE = B.DEPTID
                                       AND A.TYPE = '3'
                                       AND B.RQSJ BETWEEN TO_DATE('{0}', 'yyyy-MM-dd')-(TO_DATE('{1}','yyyy-MM-dd')-TO_DATE('{0}','yyyy-MM-dd'))-1 AND
                                           TO_DATE('{0}', 'yyyy-MM-dd')
                                       AND B.KCADD < 0
                                       AND B.GDSEQ = C.GDSEQ
                                       AND A.CODE LIKE NVL('{2}','%')                                       
                                       AND B.BILLTYPE IN ('XSD','XSG','DSH','XST')
                                       GROUP BY C.GDSEQ) B,  -------------============================
                                       (SELECT C.GDSEQ,SUM(DECODE(B.BILLTYPE,
                                                      'XST',
                                                      -ABS(B.SL),
                                                    /*  'LTD',
                                                      -ABS(B.SL),
                                                      'DST',
                                                      -ABS(B.SL),*/
                                                      ABS(B.SL))) TBSLZ, SUM(DECODE(B.BILLTYPE,
                                                      'XST',
                                                      -ABS(B.HSJE),
                                                    /*  'LTD',
                                                      -ABS(B.HSJE),
                                                      'DST',
                                                      -ABS(B.HSJE),*/
                                                      ABS(B.HSJE))) TBJEZ
                                                       FROM SYS_DEPT A, DAT_GOODSJXC B, DOC_GOODS C
                                     WHERE A.CODE = B.DEPTID
                                       AND A.TYPE = '3'
                                       AND B.RQSJ BETWEEN ADD_MONTHS(TO_DATE('{0}', 'yyyy-MM-dd'),-12) AND
                                           ADD_MONTHS(TO_DATE('{1}', 'yyyy-MM-dd') + 1,-12)
                                       AND B.KCADD < 0
                                       AND B.GDSEQ = C.GDSEQ
                                       AND A.CODE LIKE NVL('{2}','%')                                       
                                       AND B.BILLTYPE IN ('XSD','XSG','DSH','XST')
                                       GROUP BY C.GDSEQ
                                              ) C  -------------============================
                                       WHERE A.GDSEQ=B.GDSEQ(+) AND A.GDSEQ=C.GDSEQ(+)";
            string strWhere = "";

            if (strWhere.Trim().Length > 0) strSql = strSql + strWhere;
            //strSql += " GROUP BY C.GDSEQ,   C.GDNAME";
            string sortField = ddlColumnID.SelectedValue.ToString();
            string sortDirection = rblOrder.SelectedValue.ToString();
            // string tempsql = string.Format(strSql, dpkDATE1.Text, dpkDATE2.Text, strSql2, strSql3, lstDEPTID.SelectedValue.ToString()) + String.Format(" ORDER BY {0} {1}", sortField, sortDirection);
            string tempsql = string.Format(strSql, dpkDATE1.Text, dpkDATE2.Text, lstDEPTID.SelectedValue.ToString()) + String.Format(" ORDER BY {0} {1}", sortField, sortDirection);

            DataTable dt = DbHelperOra.QueryForTable(tempsql);
            GridGoods.DataSource = dt;
            GridGoods.DataBind();

            Decimal SL = 0, HSJE = 0;
            Decimal SLOther = 0, HSJEOther = 0;
            JObject summary = new JObject();
            hfdArray.Text = "";
            hfdArrayVal.Text = "";
            hfdArrayVal2.Text = "";

            for (int iRow = 0; iRow < dt.Rows.Count; iRow++)
            {

                SL += Convert.ToDecimal(dt.Rows[iRow]["SL"]);
                HSJE += Convert.ToDecimal(dt.Rows[iRow]["HSJE"]);

                if (iRow < 10)
                {

                    hfdArray.Text += dt.Rows[iRow]["GDNAME"] + ",";
                    string strGDNAME = "";
                    if (dt.Rows[iRow]["GDNAME"].ToString().Length > 10)
                    {
                        strGDNAME = dt.Rows[iRow]["GDNAME"].ToString().Substring(0, 10);
                    }
                    strGDNAME = dt.Rows[iRow]["GDNAME"].ToString();
                    hfdArrayVal.Text += dt.Rows[iRow]["HSJE"] + "$" + strGDNAME + ",";
                    hfdArrayVal2.Text += dt.Rows[iRow]["SL"] + "$" + strGDNAME + ",";

                }
                else
                {
                    SLOther += Convert.ToDecimal(dt.Rows[iRow]["SL"]);
                    HSJEOther += Convert.ToDecimal(dt.Rows[iRow]["HSJE"]);

                }


            }
            Totalsl.Text = SL.ToString();
            Totalje.Text = HSJE.ToString();
            hfdArray.Text += "其它,";
            hfdArrayVal.Text += HSJEOther + "$其它,";
            hfdArrayVal2.Text += SLOther + "$其它,";
            hfdArray.Text = hfdArray.Text.TrimEnd(',');
            hfdArrayVal.Text = hfdArrayVal.Text.TrimEnd(',');
            hfdArrayVal2.Text = hfdArrayVal2.Text.TrimEnd(',');
            summary.Add("NAME", "本页合计");
            summary.Add("SL", SL.ToString("F2"));
            summary.Add("HSJE", HSJE.ToString("F2"));
            GridGoods.SummaryData = summary;
            PageContext.RegisterStartupScript("updateDate();");

            //  PageContext.RegisterStartupScript("getEcharsData2();");
            //PageContext.RegisterStartupScript("getEcharsData();");
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
            string strSql = @"SELECT A.SL,A.HSJE,B.GDSEQ,B.GDNAME,B.GDSPEC,f_getunitname(B.UNIT) UNITNAME,f_getproducername(B.PRODUCER) PRODUCERNAME,
                                   B.HSJJ,B.PIZNO,
                                   ROUND(
DECODE(A.SL,0,0,
(A.SL/(SELECT SUM(DECODE(AA.BILLTYPE,'XST',-ABS(AA.SL),
/*'LTD',-ABS(AA.SL),'DST',-ABS(AA.SL),*/
ABS(AA.SL)))
                                    FROM DAT_GOODSJXC AA
                                    WHERE AA.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd') + 1 AND AA.DEPTID LIKE NVL('{3}','%'))),4)
) SLZB,
                                    ROUND(
DECODE(A.SL,0,0,
(A.HSJE/(SELECT SUM(DECODE(AA.BILLTYPE,'XST',-ABS(AA.HSJE),
/*'LTD',-ABS(AA.HSJE),'DST',-ABS(AA.HSJE),*/
ABS(AA.HSJE)))
                                    FROM DAT_GOODSJXC AA
                                    WHERE AA.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd') + 1 AND AA.DEPTID LIKE NVL('{3}','%'))),4) 
) JEZB,
                                    F_GETBL('DPJEHB','{0}','{1}','{3}',B.GDSEQ,'','','') JEHB,F_GETBL('DPJETB','{0}','{1}','{3}',B.GDSEQ,'','','') JETB,
                                    F_GETBL('DPSLHB','{0}','{1}','{3}',B.GDSEQ,'','','') SLHB,F_GETBL('DPSLTB','{0}','{1}','{3}',B.GDSEQ,'','','') SLTB
                            FROM (SELECT A.GDSEQ,SUM(DECODE(A.BILLTYPE,'XST',-ABS(A.SL),
/*'LTD',-ABS(A.SL),'DST',-ABS(A.SL),*/
ABS(A.SL))) SL
                                    ,SUM(DECODE(A.BILLTYPE,'XST',-ABS(A.HSJE),
/*'LTD',-ABS(A.HSJE),'DST',-ABS(A.HSJE),*/
ABS(A.HSJE))) HSJE
                            FROM DAT_GOODSJXC A
                            WHERE A.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd') + 1
                             AND KCADD = -1 {2}                                       
                             AND A.BILLTYPE IN ('XSD','XSG','DSH','XST')

                            GROUP BY A.GDSEQ) A,DOC_GOODS B
                            WHERE A.GDSEQ = B.GDSEQ";
            string strWhere = "";
            string strWhere2 = "";
            if (ddlDEPTID.SelectedValue.Length > 1) strWhere2 += " AND A.DEPTID = '" + ddlDEPTID.SelectedValue + "'";
            if (lisGDSEQ.Text.Trim().Length > 0)
            {
                strWhere += String.Format(" AND (B.GDSEQ LIKE '%{0}%' OR B.GDNAME LIKE '%{0}%' OR B.ZJM LIKE '%{0}%')", lisGDSEQ.Text.Trim());
            }
            if (!string.IsNullOrWhiteSpace(lstISGZ.SelectedValue))
            {
                strWhere += string.Format(" AND B.ISGZ = '{0}'", lstISGZ.SelectedValue);
            }
            if (strWhere.Trim().Length > 0) strSql = strSql + strWhere;
            int total = 0;
            string sortField = GridList.SortField;
            string sortDirection = GridList.SortDirection;

            string tempSql = string.Format(strSql, dpkDATE1.Text, dpkDATE2.Text, strWhere2, ddlDEPTID.SelectedValue.ToString()) + String.Format(" ORDER BY {0} {1}", sortField, sortDirection);
            DataTable dtData = PubFunc.DbGetPage(GridList.PageIndex, GridList.PageSize, tempSql, ref total);
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

        protected void GridGoods_RowDataBound(object sender, GridRowEventArgs e)
        {


            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                //  string colName = ddlColumnID.SelectedValue.ToString();

                if (ddlColumnID.SelectedValue.ToString() == "ZBSL")
                    e.CellAttributes[4]["data-color"] = GetColorCss(e.RowIndex);
                else
                    e.CellAttributes[9]["data-color"] = GetColorCss(e.RowIndex);
                /* if (row["FLAG"].ToString() == "S")
                 {
                     e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color1";
                 }*/
            }


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
                // lstDEPTID.SelectedValue = "";
                //ddlISGZ.SelectedValue = "";

                GridGoods.DataBind();
            }
            if (TabStrip1.ActiveTabIndex == 1)
            {
                lisDATE1.SelectedDate = DateTime.Now.AddMonths(-3);
                lisDATE2.SelectedDate = DateTime.Now;
                lisGDSEQ.Text = String.Empty;
                lstISGZ.SelectedValue = "";
                GridList.DataBind();
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
                string strSql2 = @"(SELECT SUM(
                                        DECODE(B.BILLTYPE,
                                                 'XST',
                                                -ABS(B.SL),
                                              /*  'LTD',-ABS(B.SL),'DST',-ABS(B.SL),*/
                                                ABS(B.SL)
                                                )
                                           )
                        FROM SYS_DEPT A,DAT_GOODSJXC B,DOC_GOODS C
                        WHERE A.CODE = B.DEPTID AND A.TYPE = '3' 
                        AND B.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd') + 1
                        AND B.KCADD < 0 AND B.GDSEQ = C.GDSEQ";
                string strWhere2 = ")";
                if (strWhere2.Trim().Length > 0) strSql2 = string.Format(strSql2, dpkDATE1.Text, dpkDATE2.Text) + strWhere2;

                string strSql3 = @"(SELECT SUM(
                                        DECODE(B.BILLTYPE,
                                                'XST',
                                                -ABS(B.HSJE),
                                               /* 'LTD',-ABS(B.HSJE),'DST',-ABS(B.HSJE),*/
                                                ABS(B.HSJE)
                                               )
                                           )
                        FROM SYS_DEPT A,DAT_GOODSJXC B,DOC_GOODS C
                        WHERE A.CODE = B.DEPTID AND A.TYPE = '3' 
                        AND B.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd') + 1
                        AND B.KCADD < 0 AND B.GDSEQ = C.GDSEQ";
                string strWhere3 = ")";
                if (strWhere3.Trim().Length > 0) strSql3 = string.Format(strSql3, dpkDATE1.Text, dpkDATE2.Text) + strWhere3;

                //            string strSql = @"SELECT  C.GDSEQ,   C.GDNAME,SUM(DECODE(B.BILLTYPE,'XST',-ABS(B.SL),'LTD',-ABS(B.SL),'DST',-ABS(B.SL),ABS(B.SL))) SL
                //                            ,SUM(DECODE(B.BILLTYPE,'XST',-ABS(B.HSJE),'LTD',-ABS(B.HSJE),'DST',-ABS(B.HSJE),ABS(B.HSJE))) HSJE,
                //                            ROUND(SUM(DECODE(B.BILLTYPE,'XST',-ABS(B.SL),'LTD',-ABS(B.SL),'DST',-ABS(B.SL),ABS(B.SL)))/{2},4) ZBSL
                //                            ,ROUND(SUM(DECODE(B.BILLTYPE,'XST',-ABS(B.HSJE),'LTD',-ABS(B.HSJE),'DST',-ABS(B.HSJE),ABS(B.HSJE)))/{3},4) ZBJE,
                //                            F_GETBL('DPJEHB','{0}','{1}','{4}',C.GDSEQ,'','','') JEHB,F_GETBL('DPJETB','{0}','{1}','{4}',C.GDSEQ,'','','') JETB,
                //                            F_GETBL('DPSLHB','{0}','{1}','{4}',C.GDSEQ,'','','') SLHB,F_GETBL('DPSLTB','{0}','{1}','{4}',C.GDSEQ,'','','') SLTB
                //                        FROM SYS_DEPT A,DAT_GOODSJXC B,DOC_GOODS C
                //                        WHERE A.CODE = B.DEPTID AND A.TYPE = '3' 
                //                        AND B.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd') + 1
                //                        AND B.KCADD < 0 AND B.GDSEQ = C.GDSEQ";
                string strSql = @"SELECT ' '||A.GDSEQ 商品编码,A.GDNAME 商品名称,A.SL 使用数量,A.ZBSL*100||'%' 数量占比,NVL(C.TBSLZ,0)*100||'%'数量同比,NVL(B.HBSLZ,0)*100||'%'数量环比,
                                        A.HSJE 使用金额,A.ZBJE*100||'%' 金额占比,NVL(C.TBJEZ,0)*100||'%'金额同比,NVL(B.HBJEZ,0)*100||'%'金额环比 
                                        
                                         FROM (SELECT C.GDSEQ,
                                           C.GDNAME,
                                           SUM(DECODE(B.BILLTYPE,
                                                      'XST',
                                                      -ABS(B.SL),
                                                     /* 'LTD',
                                                      -ABS(B.SL),
                                                      'DST',
                                                      -ABS(B.SL),*/
                                                      ABS(B.SL))) SL,
                                           SUM(DECODE(B.BILLTYPE,
                                                      'XST',
                                                      -ABS(B.HSJE),
                                                    /*  'LTD',
                                                      -ABS(B.HSJE),
                                                      'DST',
                                                      -ABS(B.HSJE),*/
                                                      ABS(B.HSJE))) HSJE,
                                           ROUND(SUM(DECODE(B.BILLTYPE,
                                                            'XST',
                                                            -ABS(B.SL),
                                                         /*   'LTD',
                                                            -ABS(B.SL),
                                                            'DST',
                                                            -ABS(B.SL),*/
                                                            ABS(B.SL))) /
                                                 (SELECT SUM(DECODE(B.BILLTYPE,
                                                                    'XST',
                                                                    -ABS(B.SL),
                                                                   /* 'LTD',
                                                                    -ABS(B.SL),
                                                                    'DST',
                                                                    -ABS(B.SL),*/
                                                                    ABS(B.SL)))
                                                    FROM SYS_DEPT A, DAT_GOODSJXC B, DOC_GOODS C
                                                   WHERE A.CODE = B.DEPTID
                                                     AND A.TYPE = '3'
                                                     AND B.RQSJ BETWEEN TO_DATE('{0}', 'yyyy-MM-dd') AND
                                                         TO_DATE('{1}', 'yyyy-MM-dd') + 1
                                                     AND B.KCADD < 0
                                                     AND B.GDSEQ = C.GDSEQ
                                                      AND A.CODE LIKE NVL('{2}','%')
                                                     AND B.BILLTYPE IN ('XSD', 'XSG', 'DSH', 'XST')),
                                                 4) ZBSL,
                                           ROUND(SUM(DECODE(B.BILLTYPE,
                                                            'XST',
                                                            -ABS(B.HSJE),
                                                          /*  'LTD',
                                                            -ABS(B.HSJE),
                                                            'DST',
                                                            -ABS(B.HSJE),*/
                                                            ABS(B.HSJE))) /
                                                 (SELECT SUM(DECODE(B.BILLTYPE,
                                                                    'XST',
                                                                    -ABS(B.HSJE),
                                                                  /*  'LTD',
                                                                    -ABS(B.HSJE),
                                                                    'DST',
                                                                    -ABS(B.HSJE),*/
                                                                    ABS(B.HSJE)))
                                                    FROM SYS_DEPT A, DAT_GOODSJXC B, DOC_GOODS C
                                                   WHERE A.CODE = B.DEPTID
                                                     AND A.TYPE = '3'
                                                     AND B.RQSJ BETWEEN TO_DATE('{0}', 'yyyy-MM-dd')-365 AND
                                                         TO_DATE('{1}', 'yyyy-MM-dd') + 1
                                                     AND B.KCADD < 0
                                                     AND B.GDSEQ = C.GDSEQ
                                                     AND A.CODE LIKE NVL('{2}','%')
                                                     AND B.BILLTYPE IN ('XSD', 'XSG', 'DSH', 'XST')),
                                                 4) ZBJE      
                                      FROM SYS_DEPT A, DAT_GOODSJXC B, DOC_GOODS C
                                     WHERE A.CODE = B.DEPTID
                                       AND A.TYPE = '3'
                                       AND B.RQSJ BETWEEN TO_DATE('{0}', 'yyyy-MM-dd') AND
                                           TO_DATE('{1}', 'yyyy-MM-dd') + 1
                                       AND B.KCADD < 0
                                       AND B.GDSEQ = C.GDSEQ
                                         AND A.CODE LIKE NVL('{2}','%')                                       
                                       AND B.BILLTYPE IN ('XSD','XSG','DSH','XST')
                                        GROUP BY C.GDSEQ, C.GDNAME) A,  -------------============================
                                       (SELECT C.GDSEQ, SUM(DECODE(B.BILLTYPE,
                                                      'XST',
                                                      -ABS(B.SL),
                                                    /*  'LTD',
                                                      -ABS(B.SL),
                                                      'DST',
                                                      -ABS(B.SL),*/
                                                      ABS(B.SL))) HBSLZ, SUM(DECODE(B.BILLTYPE,
                                                      'XST',
                                                      -ABS(B.HSJE),
                                                    /*  'LTD',
                                                      -ABS(B.HSJE),
                                                      'DST',
                                                      -ABS(B.HSJE),*/
                                                      ABS(B.HSJE))) HBJEZ
                                                       FROM SYS_DEPT A, DAT_GOODSJXC B, DOC_GOODS C
                                     WHERE A.CODE = B.DEPTID
                                       AND A.TYPE = '3'
                                       AND B.RQSJ >=( TO_DATE('{0}', 'yyyy-MM-dd')-TO_DATE('{1}','yyyy-MM-dd')+TO_DATE('{0}','yyyy-MM-dd'))
                                       AND B.RQSJ <  TO_DATE('{0}', 'yyyy-MM-dd') 
                                       AND B.KCADD < 0
                                       AND B.GDSEQ = C.GDSEQ
                                       AND A.CODE LIKE NVL('{2}','%')                                       
                                       AND B.BILLTYPE IN ('XSD','XSG','DSH','XST')
                                       GROUP BY C.GDSEQ) B,  -------------============================
                                       (SELECT C.GDSEQ,SUM(DECODE(B.BILLTYPE,
                                                      'XST',
                                                      -ABS(B.SL),
                                                    /*  'LTD',
                                                      -ABS(B.SL),
                                                      'DST',
                                                      -ABS(B.SL),*/
                                                      ABS(B.SL))) TBSLZ, SUM(DECODE(B.BILLTYPE,
                                                      'XST',
                                                      -ABS(B.HSJE),
                                                    /*  'LTD',
                                                      -ABS(B.HSJE),
                                                      'DST',
                                                      -ABS(B.HSJE),*/
                                                      ABS(B.HSJE))) TBJEZ
                                                       FROM SYS_DEPT A, DAT_GOODSJXC B, DOC_GOODS C
                                     WHERE A.CODE = B.DEPTID
                                       AND A.TYPE = '3'
                                       AND TRUNC(B.RQSJ, 'DD') >=TRUNC(ADD_MONTHS(TO_DATE('{0}', 'yyyy-MM-dd'), -12),'DD')
                                       AND TRUNC(B.RQSJ, 'DD') <TRUNC(ADD_MONTHS(TO_DATE('{1}', 'yyyy-MM-dd') + 1, -12),'DD')
                                       AND B.KCADD < 0
                                       AND B.GDSEQ = C.GDSEQ
                                       AND A.CODE LIKE NVL('{2}','%')                                       
                                       AND B.BILLTYPE IN ('XSD','XSG','DSH','XST')
                                       GROUP BY C.GDSEQ
                                              ) C  -------------============================
                                       WHERE A.GDSEQ=B.GDSEQ(+) AND A.GDSEQ=C.GDSEQ(+)";
                string strWhere = "";

                if (strWhere.Trim().Length > 0) strSql = strSql + strWhere;
                //strSql += " GROUP BY C.GDSEQ,   C.GDNAME";
                string sortField = ddlColumnID.SelectedValue.ToString();
                string sortDirection = rblOrder.SelectedValue.ToString();
                // string tempsql = string.Format(strSql, dpkDATE1.Text, dpkDATE2.Text, strSql2, strSql3, lstDEPTID.SelectedValue.ToString()) + String.Format(" ORDER BY {0} {1}", sortField, sortDirection);
                string tempsql = string.Format(strSql, dpkDATE1.Text, dpkDATE2.Text, lstDEPTID.SelectedValue.ToString()) + String.Format(" ORDER BY {0} {1}", sortField, sortDirection);
                DataTable dt = DbHelperOra.QueryForTable(tempsql);
                ExcelHelper.ExportByWeb(dt, string.Format("商品使用排行"), "商品使用排行" + DateTime.Now.ToString("yyyyMMdd") + ".xls");

            }
            if (TabStrip1.ActiveTabIndex == 1)
            {
                if (GridList.Rows.Count < 1)
                {
                    Alert.Show("没有数据,无法导出！");
                    return;
                }
                string strSql = @"SELECT ' '||B.GDSEQ 商品编码,B.GDNAME 商品名称,B.GDSPEC 规格容量,f_getunitname(B.UNIT) 单位,B.HSJJ 单价,A.SL 使用数,A.HSJE 使用金额,f_getproducername(B.PRODUCER) 生成厂家,B.PIZNO 批准文号,
                                  NVL(ROUND(DECODE(A.SL,0,0,(A.SL/(SELECT SUM(DECODE(AA.BILLTYPE,'XST',-ABS(AA.SL),ABS(AA.SL)))
                                    FROM DAT_GOODSJXC AA
                                    WHERE AA.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd') + 1 AND AA.DEPTID = A.DEPTID AND AA.BILLTYPE IN ('XSD', 'XSG', 'DSH', 'XST'))),4)),0)数量占比,
                                    NVL(ROUND(DECODE(A.SL,0,0,(A.HSJE/(SELECT SUM(DECODE(AA.BILLTYPE,'XST',-ABS(AA.HSJE),ABS(AA.HSJE)))
                                    FROM DAT_GOODSJXC AA
                                    WHERE AA.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd') + 1 AND AA.DEPTID = A.DEPTID AND AA.BILLTYPE IN ('XSD', 'XSG', 'DSH', 'XST') )),4) ),0) 金额占比,
                                    F_GETBL('DPJEHB','{0}','{1}','{3}',B.GDSEQ,'','','') 金额环比,F_GETBL('DPJETB','{0}','{1}','{3}',B.GDSEQ,'','','') 金额同比,
                                    F_GETBL('DPSLHB','{0}','{1}','{3}',B.GDSEQ,'','','') 数量环比,F_GETBL('DPSLTB','{0}','{1}','{3}',B.GDSEQ,'','','') 数量同比
                            FROM (SELECT A.DEPTID,A.GDSEQ,SUM(DECODE(A.BILLTYPE,'XST',-ABS(A.SL),
/*'LTD',-ABS(A.SL),'DST',-ABS(A.SL),*/
ABS(A.SL))) SL
                                    ,SUM(DECODE(A.BILLTYPE,'XST',-ABS(A.HSJE),
/*'LTD',-ABS(A.HSJE),'DST',-ABS(A.HSJE),*/
ABS(A.HSJE))) HSJE
                            FROM DAT_GOODSJXC A
                            WHERE A.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd') + 1
                             AND KCADD = -1 {2}                                       
                             AND A.BILLTYPE IN ('XSD','XSG','DSH','XST')

                            GROUP BY A.DEPTID,A.GDSEQ) A,DOC_GOODS B
                            WHERE A.GDSEQ = B.GDSEQ";
                string strWhere = "";
                string strWhere2 = "";
                if (ddlDEPTID.SelectedValue.Length > 1) strWhere2 += " AND A.DEPTID = '" + ddlDEPTID.SelectedValue + "'";
                if (lisGDSEQ.Text.Trim().Length > 0)
                {
                    strWhere += String.Format(" AND (B.GDSEQ LIKE '%{0}%' OR B.GDNAME LIKE '%{0}%' OR B.ZJM LIKE '%{0}%')", lisGDSEQ.Text.Trim());
                }
                if (!string.IsNullOrWhiteSpace(lstISGZ.SelectedValue))
                {
                    strWhere += string.Format(" AND B.ISGZ = '{0}'", lstISGZ.SelectedValue);
                }
                if (strWhere.Trim().Length > 0) strSql = strSql + strWhere;

                string sortField = GridList.SortField;
                string sortDirection = GridList.SortDirection;

                string tempSql = string.Format(strSql, dpkDATE1.Text, dpkDATE2.Text, strWhere2, ddlDEPTID.SelectedValue.ToString()) + String.Format(" ORDER BY {0} {1}", sortField, sortDirection);
                DataTable dt = DbHelperOra.QueryForTable(tempSql);
                ExcelHelper.ExportByWeb(dt, string.Format("科室明细排行"), "科室明细排行" + DateTime.Now.ToString("yyyyMMdd") + ".xls");

            }
        }




        protected void GridGoods_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            //ddlDEPTID.SelectedValue = GridGoods.DataKeys[e.RowIndex][0].ToString();
            //  lstISGZ.SelectedValue = ddlISGZ.SelectedValue;
            lisDATE1.SelectedDate = dpkDATE1.SelectedDate;
            lisDATE2.SelectedDate = dpkDATE2.SelectedDate;
            ddlDEPTID.SelectedValue = lstDEPTID.SelectedValue;
            lstISGZ.SelectedValue = "";
            TabStrip1.ActiveTabIndex = 1;
            lisGDSEQ.Text = GridGoods.DataKeys[e.RowIndex][0].ToString();
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
        protected void ddlA_SelectedIndexChanged(object sender, EventArgs e)
        {
            int cNum = Convert.ToInt32(ddlc.Text);
            // int bNum = Convert.ToInt32(ddlB.SelectedValue);
            int aNum = Convert.ToInt32(ddlA.SelectedValue);
            int CurrNum = 100 - aNum - cNum;
            if (CurrNum < 0)
            {

                Alert.Show("ABC比例设置有问题！");
                ddlA.Focus();
                return;
            }

            ddlB.SelectedValue = (100 - aNum - cNum).ToString();

        }
        protected void ddlB_SelectedIndexChanged(object sender, EventArgs e)
        {
            int cNum = Convert.ToInt32(ddlc.Text);
            int bNum = Convert.ToInt32(ddlB.SelectedValue);
            int aNum = Convert.ToInt32(ddlA.SelectedValue);
            int CurrNum = 100 - aNum - bNum;
            if (CurrNum < 0)
            {
                Alert.Show("ABC比例设置有问题！");
                ddlB.Focus();
                return;
            }
            else
            {
                ddlc.Text = CurrNum.ToString();


            }
        }
        public string GetColorCss(int rowIndex)
        {


            
           if (firtotal < Convert.ToDecimal(ddlA.SelectedValue))
            
            {
                //  e.RowAttributes["data-color"] = "color1";
                //FineUIPro.BoundField Bfile = (FineUIPro.BoundField)GridGoods.fi

                if (ddlColumnID.SelectedValue.ToString() == "ZBSL")
                {

                    firtotal += Convert.ToDecimal(GridGoods.Rows[rowIndex].Values[4].ToString().Replace("%", ""));

                }
                else
                {
                    firtotal += Convert.ToDecimal(GridGoods.Rows[rowIndex].Values[9].ToString().Replace("%", ""));
                }
                return "color1";

            }
            else
            {
                if (firtotal < Convert.ToDecimal(ddlB.SelectedValue) + Convert.ToDecimal(ddlA.SelectedValue))
                {

                    //  e.CellAttributes["data-color"] = "color2";
                    if (ddlColumnID.SelectedValue.ToString() == "ZBSL")
                    {
                        firtotal += Convert.ToDecimal(GridGoods.Rows[rowIndex].Values[4].ToString().Replace("%", ""));
                    }
                    else
                    {
                        firtotal += Convert.ToDecimal(GridGoods.Rows[rowIndex].Values[9].ToString().Replace("%", ""));
                    }
                    return "color2";
                }

            }
            return "";

        }


    }
}