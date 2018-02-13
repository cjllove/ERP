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
    public partial class DeptReturn : PageBase
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
            //string strSql2 = string.Format(@"(SELECT SUM(ABS(B.SL))
            //            FROM SYS_DEPT A,DAT_GOODSJXC B,DOC_GOODS C
            //            WHERE A.CODE = B.DEPTID AND A.TYPE = '3' AND B.BILLTYPE IN('DST','LTD','XST')
            //            AND B.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd') + 1
            //            AND B.KCADD < 0 AND B.GDSEQ = C.GDSEQ AND A.CODE LIKE NVL('{2}','%')", dpkDATE1.Text, dpkDATE2.Text, lstDEPTID.SelectedValue);
            //string strWhere2 = "";
            //if (!PubFunc.StrIsEmpty(ddlISGZ.SelectedValue))
            //{
            //    strWhere2 += " AND C.ISGZ = '" + ddlISGZ.SelectedValue + "')";

            //}
            //else strWhere2 += " AND 1=1)";
            //if (strWhere2.Trim().Length > 0) strSql2 += strWhere2;

            //string strSql3 = string.Format(@"(SELECT SUM(ABS(B.HSJE))
            //            FROM SYS_DEPT A,DAT_GOODSJXC B,DOC_GOODS C
            //            WHERE A.CODE = B.DEPTID AND A.TYPE = '3'  AND B.BILLTYPE IN('DST','LTD','XST')
            //            AND B.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd') + 1
            //            AND B.KCADD < 0 AND B.GDSEQ = C.GDSEQ AND A.CODE LIKE NVL('{2}','%')", dpkDATE1.Text, dpkDATE2.Text, lstDEPTID.SelectedValue);
            //string strWhere3 = "";
            //if (!PubFunc.StrIsEmpty(ddlISGZ.SelectedValue)) strWhere3 += " AND C.ISGZ = '" + ddlISGZ.SelectedValue + "')";
            //else strWhere3 += " AND 1=1)";
            //if (strWhere3.Trim().Length > 0) strSql3 += strWhere3;

            ////string strSql = @"SELECT A.CODE,A.NAME,f_getusername(A.MANAGER) USERNAME,SUM(ABS(B.SL)) SL
            ////                ,SUM(ABS(B.HSJE)) HSJE,
            ////                ROUND(SUM(ABS(B.SL))/{2},4) ZBSL
            ////                ,ROUND(SUM(ABS(B.HSJE))/{3},4) ZBJE,
            ////                F_GETBL('KSJEHB','{0}','{1}',A.CODE,'','','','') JEHB,F_GETBL('KSJETB','{0}','{1}',A.CODE,'','','','') JETB,
            ////                F_GETBL('KSSLHB','{0}','{1}',A.CODE,'','','','') SLHB,F_GETBL('KSSLTB','{0}','{1}',A.CODE,'','','','') SLTB
            ////            FROM SYS_DEPT A,DAT_GOODSJXC B,DOC_GOODS C
            ////            WHERE A.CODE = B.DEPTID AND A.TYPE = '3'  AND B.BILLTYPE IN('DST','LTD','XST')
            ////            AND B.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd') + 1
            ////            AND B.KCADD < 0 AND B.GDSEQ = C.GDSEQ";
            //string strSql = @"SELECT A.CODE,A.NAME,f_getusername(A.MANAGER) USERNAME,SUM(ABS(B.SL)) SL
            //                ,SUM(ABS(B.HSJE)) HSJE,
            //                ROUND(SUM(ABS(B.SL))/{2},4) ZBSL
            //                ,ROUND(SUM(ABS(B.HSJE))/{3},4) ZBJE,
            //                F_GETTHHB('DPJEHB','{0}','{1}',A.CODE,'','','','') JEHB,F_GETTHHB('DPJETB','{0}','{1}',A.CODE,'','','','') JETB,
            //                F_GETTHHB('DPSLHB','{0}','{1}',A.CODE,'','','','') SLHB,F_GETTHHB('DPSLTB','{0}','{1}',A.CODE,'','','','') SLTB
            //            FROM SYS_DEPT A,DAT_GOODSJXC B,DOC_GOODS C
            //            WHERE A.CODE = B.DEPTID AND A.TYPE = '3'  AND B.BILLTYPE IN('DST','LTD','XST')
            //            AND B.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd') + 1
            //            AND B.KCADD < 0 AND B.GDSEQ = C.GDSEQ";
            //string strWhere = "";
            //string strhfd = strSql;
            //if (lstDEPTID.SelectedValue.Length > 0) strWhere += " AND A.CODE = '" + lstDEPTID.SelectedValue + "'";
            //if (!PubFunc.StrIsEmpty(ddlISGZ.SelectedValue)) strWhere += " AND C.ISGZ = '" + ddlISGZ.SelectedValue + "'";
            //if (strWhere.Trim().Length > 0) { strSql = strSql + strWhere; strhfd = strhfd + strWhere; }
            //strSql += " GROUP BY A.CODE,A.NAME,A.MANAGER";
            //strhfd += " GROUP BY A.CODE,A.NAME,A.MANAGER";
            string strSql = @"SELECT A.*,NVL(B.HBJE,0) HBJE,NVL(B.HBSL,0) HBSL,
NVL(C.TBJE,0)TBJE,NVL(C.TBSL,0) TBSL
FROM
(SELECT A.CODE,
       A.NAME,
       f_getusername(A.MANAGER) USERNAME,
       SUM(ABS(B.SL)) SL,
       SUM(ABS(B.HSJE)) HSJE,
       ROUND(SUM(ABS(B.SL)) /
             (SELECT SUM(ABS(B.SL))
                FROM SYS_DEPT A, DAT_GOODSJXC B, DOC_GOODS C
               WHERE A.CODE = B.DEPTID
                 AND A.TYPE = '3'
                 AND B.BILLTYPE IN ('DST', 'LTD', 'XST')
                 AND B.RQSJ BETWEEN TO_DATE('{0}', 'yyyy-MM-dd') AND
                     TO_DATE('{1}', 'yyyy-MM-dd') + 1
                 AND B.KCADD < 0
                 AND B.GDSEQ = C.GDSEQ
                 AND A.CODE LIKE NVL('{2}', '%')
                 AND C.ISGZ LIKE NVL('{3}','%')),
             4) ZBSL,
       ROUND(SUM(ABS(B.HSJE)) /
             (SELECT SUM(ABS(B.HSJE))
                FROM SYS_DEPT A, DAT_GOODSJXC B, DOC_GOODS C
               WHERE A.CODE = B.DEPTID
                 AND A.TYPE = '3'
                 AND B.BILLTYPE IN ('DST', 'LTD', 'XST')
                 AND B.RQSJ BETWEEN TO_DATE('{0}', 'yyyy-MM-dd') AND
                     TO_DATE('{1}', 'yyyy-MM-dd') + 1
                 AND B.KCADD < 0
                 AND B.GDSEQ = C.GDSEQ
                 AND A.CODE LIKE NVL('{2}', '%')
                 AND C.ISGZ LIKE NVL('{3}','%')),
             4) ZBJE,
       F_GETTHHB('DPJEHB',
                 '{0}',
                 '{1}',
                 A.CODE,
                 '',
                 '',
                 '',
                 '') JEHB,
       F_GETTHHB('DPJETB',
                 '{0}',
                 '{1}',
                 A.CODE,
                 '',
                 '',
                 '',
                 '') JETB,
       F_GETTHHB('DPSLHB',
                 '{0}',
                 '{1}',
                 A.CODE,
                 '',
                 '',
                 '',
                 '') SLHB,
       F_GETTHHB('DPSLTB',
                 '{0}',
                 '{1}',
                 A.CODE,
                 '',
                 '',
                 '',
                 '') SLTB
  FROM SYS_DEPT A, DAT_GOODSJXC B, DOC_GOODS C
 WHERE A.CODE = B.DEPTID
   AND A.TYPE = '3'
   AND B.BILLTYPE IN ('DST', 'LTD', 'XST')
   AND B.RQSJ BETWEEN TO_DATE('{0}', 'yyyy-MM-dd') AND
       TO_DATE('{1}', 'yyyy-MM-dd') + 1
   AND B.KCADD < 0
   AND B.GDSEQ = C.GDSEQ
    AND A.CODE LIKE NVL('{2}', '%')
   AND C.ISGZ LIKE NVL('{3}','%')
 GROUP BY A.CODE, A.NAME, A.MANAGER)A,
 
 
  (SELECT A.CODE,
       A.NAME,
       f_getusername(A.MANAGER) USERNAME,
               SUM(DECODE(B.BILLTYPE,
                          'XST',
                          ABS(B.HSJE),
                          'DST',
                          ABS(B.HSJE),
                          'LTD',
                          ABS(B.HSJE))) HBJE,
               SUM(DECODE(B.BILLTYPE,
                          'XST',
                          ABS(B.SL),
                          'DST',
                          ABS(B.SL),
                          'LTD',
                          ABS(B.SL))) HBSL
          FROM SYS_DEPT A, DAT_GOODSJXC B,DOC_GOODS C
           WHERE A.CODE = B.DEPTID AND B.GDSEQ=C.GDSEQ
           AND A.TYPE = '3'
            AND B.BILLTYPE IN ('DST', 'LTD', 'XST')
           AND B.RQSJ >= TO_DATE('{0}', 'yyyy-MM-dd')-(TO_DATE('{1}', 'yyyy-MM-dd')-TO_DATE('{0}', 'yyyy-MM-dd'))-1 
           AND B.RQSJ < TO_DATE('{0}', 'yyyy-MM-dd') 
           AND B.KCADD < 0
            AND A.CODE LIKE NVL('{2}', '%')
            AND C.ISGZ LIKE NVL('{3}', '%')
         GROUP BY A.CODE, A.NAME, A.MANAGER) B,
 (SELECT A.CODE,
       A.NAME,
       f_getusername(A.MANAGER) USERNAME,
               SUM(DECODE(B.BILLTYPE,
                          'XST',
                          ABS(B.HSJE),
                          'DST',
                          ABS(B.HSJE),
                          'LTD',
                          ABS(B.HSJE))) TBJE,
               SUM(DECODE(B.BILLTYPE,
                          'XST',
                          ABS(B.SL),
                          'DST',
                          ABS(B.SL),
                          'LTD',
                          ABS(B.SL))) TBSL
          FROM SYS_DEPT A, DAT_GOODSJXC B,DOC_GOODS C
         WHERE A.CODE = B.DEPTID AND B.GDSEQ=C.GDSEQ
           AND A.TYPE = '3'
           AND B.BILLTYPE IN ('DST', 'LTD', 'XST')
           AND B.RQSJ >=
               ADD_MONTHS(TO_DATE('{0}', 'yyyy-MM-dd'), -12)
           AND B.RQSJ <
               ADD_MONTHS((TO_DATE('{1}', 'yyyy-MM-dd') + 1), -12)
           AND B.KCADD < 0
           AND A.CODE LIKE NVL('{2}', '%')
           AND C.ISGZ LIKE NVL('{3}', '%')
      GROUP BY   A.CODE, A.NAME, A.MANAGER) C        
  WHERE A.CODE=B.CODE(+) AND A.CODE=C.CODE(+) ";
            string sortField = GridGoods.SortField;
            string sortDirection = GridGoods.SortDirection;
            DataTable dt = DbHelperOra.QueryForTable(string.Format(strSql, dpkDATE1.Text, dpkDATE2.Text, lstDEPTID.SelectedValue, ddlISGZ.SelectedValue) + String.Format(" ORDER BY {0} {1}", sortField, sortDirection));
            GridGoods.DataSource = dt;
            GridGoods.DataBind();
            DataTable dtchar = DbHelperOra.QueryForTable(string.Format(strSql, dpkDATE1.Text, dpkDATE2.Text, lstDEPTID.SelectedValue, ddlISGZ.SelectedValue) + String.Format(" ORDER BY HSJE DESC"));
            JObject summary = new JObject();
            hfdArray.Text = "";
            hfdArrayVal.Text = "";
            hfdArrayVal2.Text = "";
            Decimal SL = 0, HSJE = 0, total = 0, totaltb = 0, totalhb = 0;
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                SL += Convert.ToDecimal(dr["SL"]);
                HSJE += Convert.ToDecimal(dr["HSJE"]);
                //totaltb += Convert.ToDecimal(dr["HSJE"]) /( Convert.ToDecimal(dr["JETB"])+1);
                //totalhb += Convert.ToDecimal(dr["HSJE"]) /( Convert.ToDecimal(dr["JEHB"])+1);
                totaltb += Convert.ToDecimal(dr["TBJE"]);
                totalhb += Convert.ToDecimal(dr["HBJE"]);
            }
            if (dtchar.Rows.Count < 1)
            {
                hfdArray.Text = "无查询数据信息,";
                hfdArrayVal.Text = "0$无查询数据信息,";
            }
            foreach (DataRow dr in dtchar.Rows)
            {
                if (i >5)
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
            string strWhere = "";
            string strSql = "";
            if (ddlDEPTID.SelectedValue.Length > 1)
            {
                strSql = @"SELECT A.SL,A.HSJE,B.GDSEQ,B.GDNAME,B.GDSPEC,f_getunitname(B.UNIT) UNITNAME,f_getproducername(B.PRODUCER) PRODUCERNAME,
                                   B.HSJJ,B.PIZNO,
                                   ROUND((A.SL/(SELECT SUM(ABS(AA.SL))
                                    FROM DAT_GOODSJXC AA
                                    WHERE AA.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd') + 1 AND AA.DEPTID = A.DEPTID AND AA.KCADD = -1 AND AA.BILLTYPE IN('DST','LTD','XST')
                                    AND EXISTS(SELECT 1 FROM SYS_DEPT BB WHERE BB.CODE = AA.DEPTID AND BB.TYPE IN('3','4')))),4) SLZB,
                                    ROUND((A.HSJE/(SELECT SUM(ABS(AA.HSJE))
                                    FROM DAT_GOODSJXC AA
                                    WHERE AA.KCADD = -1 AND EXISTS(SELECT 1 FROM SYS_DEPT BB WHERE BB.CODE = AA.DEPTID AND BB.TYPE IN('3','4')) AND
                                    AA.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd') + 1 AND AA.DEPTID = A.DEPTID  AND AA.BILLTYPE IN('DST','LTD','XST'))),4) JEZB,
                                    F_GETTHHB('KSJEHB','{0}','{1}',A.DEPTID,B.GDSEQ,'','','') JEHB,F_GETTHHB('KSJETB','{0}','{1}',A.DEPTID,B.GDSEQ,'','','') JETB,
                                    F_GETTHHB('KSSLHB','{0}','{1}',A.DEPTID,B.GDSEQ,'','','') SLHB,F_GETTHHB('KSSLTB','{0}','{1}',A.DEPTID,B.GDSEQ,'','','') SLTB
                            FROM (SELECT A.DEPTID,A.GDSEQ,SUM(ABS(A.SL)) SL
                                    ,SUM(ABS(A.HSJE)) HSJE
                            FROM DAT_GOODSJXC A,SYS_DEPT B
                            WHERE A.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd') + 1
                            {2}  AND A.BILLTYPE IN('DST','LTD','XST') AND A.DEPTID = B.CODE AND B.TYPE IN('3','4')  AND A.KCADD = -1
                            GROUP BY A.DEPTID,A.GDSEQ) A,DOC_GOODS B
                            WHERE A.GDSEQ = B.GDSEQ";
                strWhere += " AND A.DEPTID = '" + ddlDEPTID.SelectedValue + "'";
            }
            else
            {
                strSql = @"SELECT A.SL,A.HSJE,B.GDSEQ,B.GDNAME,B.GDSPEC,f_getunitname(B.UNIT) UNITNAME,f_getproducername(B.PRODUCER) PRODUCERNAME,
                                   B.HSJJ,B.PIZNO,
                                   ROUND((A.SL/(SELECT SUM(ABS(AA.SL))
                                    FROM DAT_GOODSJXC AA
                                    WHERE AA.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd') + 1 AND AA.KCADD = -1
                                    AND EXISTS(SELECT 1 FROM SYS_DEPT BB WHERE BB.CODE = AA.DEPTID AND BB.TYPE IN('3','4') AND AA.BILLTYPE IN ('DST', 'LTD', 'XST')))),4) SLZB,
                                    ROUND((A.HSJE/(SELECT SUM(ABS(AA.HSJE))
                                    FROM DAT_GOODSJXC AA
                                    WHERE AA.KCADD = -1 AND EXISTS(SELECT 1 FROM SYS_DEPT BB WHERE BB.CODE = AA.DEPTID AND BB.TYPE IN('3','4')) AND
                                    AA.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd') + 1 AND AA.BILLTYPE IN('DST','LTD','XST'))),4) JEZB,
                                    F_GETTHHB('DPJEHB','{0}','{1}','',B.GDSEQ,'','','') JEHB,F_GETTHHB('DPJETB','{0}','{1}','',B.GDSEQ,'','','') JETB,
                                    F_GETTHHB('DPSLHB','{0}','{1}','',B.GDSEQ,'','','') SLHB,F_GETTHHB('DPSLTB','{0}','{1}','',B.GDSEQ,'','','') SLTB
                            FROM (SELECT A.GDSEQ,SUM(ABS(A.SL)) SL
                                    ,SUM(ABS(A.HSJE)) HSJE
                            FROM DAT_GOODSJXC A,SYS_DEPT B
                            WHERE A.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd') + 1
                            {2}  AND A.BILLTYPE IN('DST','LTD','XST') AND A.DEPTID = B.CODE AND B.TYPE IN('3','4')  AND A.KCADD = -1
                            GROUP BY A.GDSEQ) A,DOC_GOODS B
                            WHERE A.GDSEQ = B.GDSEQ";
            }
            //strSql = @"SELECT A.SL,A.HSJE,B.GDSEQ,B.GDNAME,B.GDSPEC,f_getunitname(B.UNIT) UNITNAME,f_getproducername(B.PRODUCER) PRODUCERNAME,
            //                       B.HSJJ,B.PIZNO,
            //                       ROUND((A.SL/(SELECT SUM(ABS(AA.SL))
            //                        FROM DAT_GOODSJXC AA
            //                        WHERE AA.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd') + 1 AND AA.DEPTID = A.DEPTID AND AA.KCADD = -1
            //                        AND EXISTS(SELECT 1 FROM SYS_DEPT BB WHERE BB.CODE = AA.DEPTID AND BB.TYPE IN('3','4')))),4) SLZB,
            //                        ROUND((A.HSJE/(SELECT SUM(ABS(AA.HSJE))
            //                        FROM DAT_GOODSJXC AA
            //                        WHERE AA.KCADD = -1 AND EXISTS(SELECT 1 FROM SYS_DEPT BB WHERE BB.CODE = AA.DEPTID AND BB.TYPE IN('3','4')) AND
            //                        AA.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd') + 1 AND AA.DEPTID = A.DEPTID  AND AA.BILLTYPE IN('DST','LTD','XST'))),4) JEZB,
            //                        F_GETBL('KSJEHB','{0}','{1}',A.DEPTID,B.GDSEQ,'','','') JEHB,F_GETBL('KSJETB','{0}','{1}',A.DEPTID,B.GDSEQ,'','','') JETB,
            //                        F_GETBL('KSSLHB','{0}','{1}',A.DEPTID,B.GDSEQ,'','','') SLHB,F_GETBL('KSSLTB','{0}','{1}',A.DEPTID,B.GDSEQ,'','','') SLTB
            //                FROM (SELECT A.DEPTID,A.GDSEQ,SUM(ABS(A.SL)) SL
            //                        ,SUM(ABS(A.HSJE)) HSJE
            //                FROM DAT_GOODSJXC A,SYS_DEPT B
            //                WHERE A.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd') + 1
            //                {2}  AND A.BILLTYPE IN('DST','LTD','XST') AND A.DEPTID = B.CODE AND B.TYPE IN('3','4')  AND A.KCADD = -1
            //                GROUP BY A.DEPTID,A.GDSEQ) A,DOC_GOODS B
            //                WHERE A.GDSEQ = B.GDSEQ";
            //string strWhere = "";
            //if (ddlDEPTID.SelectedValue.Length > 1) strWhere += " AND A.DEPTID = '" + ddlDEPTID.SelectedValue + "'";
            if (lisGDSEQ.Text.Trim().Length > 0)
            {
                strWhere += String.Format(" AND EXISTS (SELECT 1 FROM DOC_GOODS K WHERE K.GDSEQ = A.GDSEQ AND (K.GDSEQ LIKE '%{0}%' OR K.GDNAME LIKE '%{0}%' OR K.ZJM LIKE '%{0}%'))", lisGDSEQ.Text.Trim());
            }
            if (lstISGZ.SelectedValue.Length > 0) strWhere += String.Format(" AND EXISTS (SELECT 1 FROM DOC_GOODS KK WHERE KK.GDSEQ = A.GDSEQ AND KK.ISGZ = '{0}')", lstISGZ.SelectedValue);
            int total = 0;
            string sortField = GridList.SortField;
            string sortDirection = GridList.SortDirection;

            DataTable dtData = PubFunc.DbGetPage(GridList.PageIndex, GridList.PageSize, string.Format(strSql, lisDATE1.Text, lisDATE2.Text, strWhere) + String.Format(" ORDER BY {0} {1}", sortField, sortDirection), ref total);
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
            String sql = @"SELECT B.NAME,SUM(ABS(A.SL)) SL 
                            FROM
                            (SELECT A.STR2 THTYPE,B.SL
                            FROM DAT_XS_DOC A,DAT_GOODSJXC B,DOC_GOODS C
                            WHERE A.SEQNO = B.BILLNO AND B.BILLTYPE = 'XST'
                            AND A.DEPTID = B.DEPTID AND B.KCADD = - 1 AND B.GDSEQ = C.GDSEQ AND
                            B.RQSJ BETWEEN TO_DATE('{1}', 'yyyy-MM-dd') AND TO_DATE('{2}', 'yyyy-MM-dd') + 1
                            AND A.DEPTID LIKE NVL('{0}','%') AND C.ISGZ LIKE NVL('{3}','%') AND (C.GDNAME LIKE '%{4}%' OR C.GDSEQ LIKE '%{4}%')
                            UNION ALL
                            SELECT A.THTYPE,B.SL
                            FROM DAT_CK_DOC A,DAT_GOODSJXC B,DOC_GOODS C
                            WHERE A.SEQNO = B.BILLNO AND B.BILLTYPE IN ('DST','LTD')
                            AND A.DEPTID = B.DEPTID AND B.KCADD = - 1 AND B.GDSEQ = C.GDSEQ AND C.ISGZ LIKE NVL('{3}','%') AND
                            B.RQSJ BETWEEN TO_DATE('{1}', 'yyyy-MM-dd') AND TO_DATE('{2}', 'yyyy-MM-dd') + 1
                            AND A.DEPTID LIKE NVL('{0}','%') AND (C.GDNAME LIKE '%{4}%' OR C.GDSEQ LIKE '%{4}%')) A,SYS_CODEVALUE B
                            WHERE A.THTYPE = B.CODE AND B.TYPE='SUP_RETURNCAUSE'
                            GROUP BY B.NAME";
            DataTable dt = DbHelperOra.Query(String.Format(sql, ddlDEPTID.SelectedValue, lisDATE1.Text, lisDATE2.Text, lstISGZ.SelectedValue, lisGDSEQ.Text)).Tables[0];
            hfdArrayRtn.Text = "";
            hfdArrayRtnVal.Text = "";
            if (dt.Rows.Count < 1)
            {
                //hfdArrayRtn.Text = "无查询数据信息,";
                hfdArrayRtnVal.Text =100 + "$" + "无查询数据信息"+ ",";
            }
            foreach (DataRow dr in dt.Rows)
            {
                hfdArrayRtn.Text += dr["NAME"] + ",";
                hfdArrayRtnVal.Text += dr["SL"] + "$" + dr["NAME"] + ",";
            }
            PageContext.RegisterStartupScript("getEcharsData3();");
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