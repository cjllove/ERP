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
using XTBase.Utilities;

namespace ERPProject.ERPQuery
{
    public partial class AllSingleUseSort : PageBase
    {
        //  decimal firtotal = 0;
        // decimal sectotal = 0;
        public AllSingleUseSort()
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
            string strSql2 = @"(SELECT SUM(DECODE(B.BILLTYPE,'XST',-ABS(B.SL),'XSD',ABS(B.SL),'XSG',ABS(B.SL),'DSH',ABS(B.SL)))
                        FROM SYS_DEPT A,DAT_GOODSJXC B,DOC_GOODS C
                        WHERE A.CODE = B.DEPTID AND A.TYPE = '3'  AND  B.DEPTID LIKE NVL('{2}', '%')
                        AND B.RQSJ >= TO_DATE('{0}','yyyy-MM-dd') AND B.RQSJ <TO_DATE('{1}','yyyy-MM-dd') + 1
                        AND B.KCADD < 0 AND B.GDSEQ = C.GDSEQ";
            string strWhere2 = ")";
            // if (!PubFunc.StrIsEmpty(ddlISGZ.SelectedValue)) strWhere2 += " AND C.ISGZ = '" + ddlISGZ.SelectedValue + "'";
            if (strWhere2.Trim().Length > 0) strSql2 = string.Format(strSql2, dpkDATE1.Text, dpkDATE2.Text, lstDEPTID.SelectedValue.ToString()) + strWhere2;

            string strSql3 = @"(SELECT SUM(DECODE(B.BILLTYPE,'XST',-ABS(B.HSJE),'XSG',ABS(B.HSJE),'XSD',ABS(B.HSJE),'DSH',ABS(B.HSJE)))
                        FROM SYS_DEPT A,DAT_GOODSJXC B,DOC_GOODS C
                        WHERE A.CODE = B.DEPTID AND A.TYPE = '3' AND  B.DEPTID LIKE NVL('{2}', '%')
                        AND B.RQSJ >= TO_DATE('{0}','yyyy-MM-dd') AND B.RQSJ < TO_DATE('{1}','yyyy-MM-dd') + 1
                        AND B.KCADD < 0 AND B.GDSEQ = C.GDSEQ";
            string strWhere3 = ")";
            //if (!PubFunc.StrIsEmpty(ddlISGZ.SelectedValue)) strWhere3 += " AND C.ISGZ = '" + ddlISGZ.SelectedValue + "'";
            if (strWhere3.Trim().Length > 0) strSql3 = string.Format(strSql3, dpkDATE1.Text, dpkDATE2.Text, lstDEPTID.SelectedValue.ToString()) + strWhere3;

            string strSql = @"SELECT * FROM(SELECT  C.GDSEQ,   C.GDNAME,NVL(SUM(DECODE(B.BILLTYPE,'XST',-ABS(B.SL),'XSD',ABS(B.SL),'XSG',ABS(B.SL),'DSH',ABS(B.SL))),0) SL,
       NVL(SUM(DECODE(B.BILLTYPE,'XST',-ABS(B.HSJE),'XSG',ABS(B.HSJE),'XSD',ABS(B.HSJE),'DSH',ABS(B.HSJE))),0) HSJE,
                            ROUND(NVL(SUM(DECODE(B.BILLTYPE,'XST',-ABS(B.SL),'XSD',ABS(B.SL),'XSG',ABS(B.SL),'DSH',ABS(B.SL))),0)/{2},4) ZBSL,
                            ROUND(NVL(SUM(DECODE(B.BILLTYPE,'XST',-ABS(B.HSJE),'XSD',ABS(B.HSJE),'XSG',ABS(B.HSJE),'DSH',ABS(B.HSJE))),0)/{3},4) ZBJE,
                            F_GETBL('DPJEHB','{0}','{1}','{4}',C.GDSEQ,'','','') JEHB,F_GETBL('DPJETB','{0}','{1}','{4}',C.GDSEQ,'','','') JETB,
                            F_GETBL('DPSLHB','{0}','{1}','{4}',C.GDSEQ,'','','') SLHB,F_GETBL('DPSLTB','{0}','{1}','{4}',C.GDSEQ,'','','') SLTB
                        FROM SYS_DEPT A,DAT_GOODSJXC B,DOC_GOODS C
                        WHERE A.CODE = B.DEPTID AND A.TYPE = '3' 
                        AND B.RQSJ >=TO_DATE('{0}','yyyy-MM-dd')AND B.RQSJ<TO_DATE('{1}','yyyy-MM-dd') + 1
                        AND B.KCADD < 0 AND B.GDSEQ = C.GDSEQ";
            string strWhere = "";
            if (lstDEPTID.SelectedValue.Length > 0) strWhere += " AND B.DEPTID = '" + lstDEPTID.SelectedValue + "'";
            //  if (!PubFunc.StrIsEmpty(ddlISGZ.SelectedValue)) strWhere += " AND C.ISGZ = '" + ddlISGZ.SelectedValue + "'";
            if (strWhere.Trim().Length > 0) strSql = strSql + strWhere;
            strSql += " GROUP BY C.GDSEQ,   C.GDNAME) WHERE SL<>0";
            string sortField = GridGoods.SortField;
            string sortDirection = GridGoods.SortDirection;
            string tempsql = string.Format(strSql, dpkDATE1.Text, dpkDATE2.Text, strSql2, strSql3, lstDEPTID.SelectedValue.ToString()) + String.Format(" ORDER BY {0} {1}", sortField, sortDirection);
            //string.Format(strSql, dpkDATE1.Text, dpkDATE2.Text, strSql2, strSql3) + String.Format(" ORDER BY {0} {1}", sortField, sortDirection)
            DataTable dtt = DbHelperOra.QueryForTable(tempsql);
            //GridGoods.DataSource = dt;
            //GridGoods.DataBind();
            int total = 0;
            DataTable dt = PubFunc.DbGetPage(GridGoods.PageIndex, GridGoods.PageSize, tempsql, ref total);
            GridGoods.RecordCount = total;
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
                if (iRow > 5)
                {
                    SLOther += Convert.ToDecimal(dt.Rows[iRow]["SL"]);
                    HSJEOther += Convert.ToDecimal(dt.Rows[iRow]["HSJE"]);
                }
                SL += Convert.ToDecimal(dt.Rows[iRow]["SL"]);
                HSJE += Convert.ToDecimal(dt.Rows[iRow]["HSJE"]);
            }
            for (int iRow = 0; iRow < dtt.Rows.Count; iRow++)
            {
                if (iRow < 5)
                {
                    string strGDNAME = dt.Rows[iRow]["GDNAME"].ToString().Length < 14 ? dt.Rows[iRow]["GDNAME"].ToString() : dt.Rows[iRow]["GDNAME"].ToString().Substring(0, 13);
                    hfdArray.Text += strGDNAME + ",";
                    //string strGDNAME = dt.Rows[iRow]["GDNAME"].ToString().Length < 10 ? dt.Rows[iRow]["GDNAME"].ToString() : dt.Rows[iRow]["GDNAME"].ToString().Substring(1, dt.Rows[iRow]["GDNAME"].ToString().Length - 1);
                    hfdArrayVal.Text += dt.Rows[iRow]["HSJE"] + "$" + strGDNAME + ",";
                    hfdArrayVal2.Text += dt.Rows[iRow]["SL"] + "$" + strGDNAME + ",";

                }
            }
            hfdArray.Text += "其它,";
            hfdArrayVal.Text += HSJEOther + "$其它,";
            hfdArrayVal2.Text += SLOther + "$其它,";
            hfdArray.Text = hfdArray.Text.TrimEnd(',');
            hfdArrayVal.Text = hfdArrayVal.Text.TrimEnd(',');
            hfdArrayVal2.Text = hfdArrayVal2.Text.TrimEnd(',');
            summary.Add("NAME", "本页合计");
            summary.Add("SL", (SL).ToString("F2"));
            summary.Add("HSJE", HSJE.ToString("F2"));
            GridGoods.SummaryData = summary;
            if (GridGoods.PageIndex == 0)
            {
                PageContext.RegisterStartupScript("getEcharsData2();");
                PageContext.RegisterStartupScript("getEcharsData();");
            }

        }
        protected void btnSch_Click(object sender, EventArgs e)
        {
            DataPHSearch();
        }
        protected void DataPHSearch()
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
            string strSql = @"SELECT * FROM(SELECT nvl(A.SL,0)SL,NVL(A.HSJE,0)HSJE,B.GDSEQ,B.GDNAME,B.GDSPEC,A.DEPTID,f_getunitname(B.UNIT) UNITNAME,f_getproducername(B.PRODUCER) PRODUCERNAME,
                                   B.HSJJ,B.PIZNO,
                                   ROUND((NVL(A.SL,0)/(SELECT SUM(DECODE(AA.BILLTYPE,'XST',-ABS(AA.SL),'XSG',ABS(AA.SL),'XSD',ABS(AA.SL),'DSH',ABS(AA.SL)))
                                    FROM DAT_GOODSJXC AA
                                    WHERE AA.RQSJ >=TO_DATE('{0}','yyyy-MM-dd') AND AA.RQSJ < TO_DATE('{1}','yyyy-MM-dd') + 1 AND AA.DEPTID = A.DEPTID AND AA.KCADD='-1')),4) SLZB,
                                    ROUND((nvl(A.HSJE,0)/(SELECT SUM(DECODE(AA.BILLTYPE,'XST',-ABS(AA.HSJE),'XSD',ABS(AA.HSJE),'XSG',ABS(AA.HSJE),'DSH',ABS(AA.HSJE)))
                                    FROM DAT_GOODSJXC AA
                                    WHERE AA.RQSJ >=TO_DATE('{0}','yyyy-MM-dd') AND AA.RQSJ<TO_DATE('{1}','yyyy-MM-dd') + 1 AND AA.DEPTID = A.DEPTID AND AA.KCADD='-1')),4) JEZB,
                                    F_GETBL('KSJEHB','{0}','{1}',A.DEPTID,B.GDSEQ,'','','') JEHB,F_GETBL('KSJETB','{0}','{1}',A.DEPTID,B.GDSEQ,'','','') JETB,
                                    F_GETBL('KSSLHB','{0}','{1}',A.DEPTID,B.GDSEQ,'','','') SLHB,F_GETBL('KSSLTB','{0}','{1}',A.DEPTID,B.GDSEQ,'','','') SLTB
                            FROM (SELECT A.DEPTID,A.GDSEQ,SUM(DECODE(A.BILLTYPE,'XST',-ABS(A.SL),'XSG',ABS(A.SL),'XSD',ABS(A.SL),'DSH',ABS(A.SL))) SL
                                    ,SUM(DECODE(A.BILLTYPE,'XST',-ABS(A.HSJE),'XSD',ABS(A.HSJE),'XSG',ABS(A.HSJE),'DSH',ABS(A.HSJE))) HSJE
                            FROM DAT_GOODSJXC A
                            WHERE A.RQSJ >=TO_DATE('{0}','yyyy-MM-dd') AND A.RQSJ <TO_DATE('{1}','yyyy-MM-dd') + 1
                            {2} AND A.DEPTID IN(SELECT CODE FROM SYS_DEPT WHERE TYPE='3') AND A.KCADD<0
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
            strSql += ") WHERE SL<>0";
            int total = 0;
            string sortField = GridList.SortField;
            string sortDirection = GridList.SortDirection;

            DataTable dtData = PubFunc.DbGetPage(GridList.PageIndex, GridList.PageSize, string.Format(strSql, lisDATE1.Text, lisDATE2.Text, strWhere2) + String.Format(" ORDER BY {0} {1}", sortField, sortDirection), ref total);
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
        protected void GridList_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridList.PageIndex = e.NewPageIndex;
            DataPHSearch();
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
                //ddlISGZ.SelectedValue = "";
            }
            if (TabStrip1.ActiveTabIndex == 1)
            {
                lisDATE1.SelectedDate = DateTime.Now.AddMonths(-3);
                lisDATE2.SelectedDate = DateTime.Now;
                lisGDSEQ.Text = String.Empty;
                lstISGZ.SelectedValue = "";
                ddlDEPTID.SelectedValue = "";
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
                string strSql2 = @"(SELECT SUM(DECODE(B.BILLTYPE,'XST',-ABS(B.SL),'XSD',ABS(B.SL),'XSG',ABS(B.SL),'DSH',ABS(B.SL)))
                        FROM SYS_DEPT A,DAT_GOODSJXC B,DOC_GOODS C
                        WHERE A.CODE = B.DEPTID AND A.TYPE = '3' 
                        AND B.RQSJ >= TO_DATE('{0}','yyyy-MM-dd') AND B.RQSJ<TO_DATE('{1}','yyyy-MM-dd') + 1
                        AND B.KCADD < 0 AND B.GDSEQ = C.GDSEQ";
                string strWhere2 = ")";
                // if (!PubFunc.StrIsEmpty(ddlISGZ.SelectedValue)) strWhere2 += " AND C.ISGZ = '" + ddlISGZ.SelectedValue + "'";
                if (strWhere2.Trim().Length > 0) strSql2 = string.Format(strSql2, dpkDATE1.Text, dpkDATE2.Text) + strWhere2;

                string strSql3 = @"(SELECT SUM(DECODE(B.BILLTYPE,'XST',-ABS(B.HSJE),'XSG',ABS(B.HSJE),'XSD',ABS(B.HSJE),'DSH',ABS(B.HSJE)))
                        FROM SYS_DEPT A,DAT_GOODSJXC B,DOC_GOODS C
                        WHERE A.CODE = B.DEPTID AND A.TYPE = '3' 
                        AND B.RQSJ >= TO_DATE('{0}','yyyy-MM-dd') AND B.RQSJ< TO_DATE('{1}','yyyy-MM-dd') + 1
                        AND B.KCADD < 0 AND B.GDSEQ = C.GDSEQ";
                string strWhere3 = ")";
                // if (!PubFunc.StrIsEmpty(ddlISGZ.SelectedValue)) strWhere3 += " AND C.ISGZ = '" + ddlISGZ.SelectedValue + "'";
                if (strWhere3.Trim().Length > 0) strSql3 = string.Format(strSql3, dpkDATE1.Text, dpkDATE2.Text) + strWhere3;

                string strSql = @"SELECT ' '||GDSEQ 商品编码，GDNAME 商品名称,SL 使用数量,ROUND(SLZB*100,4) ||'%' 数量占比,ROUND(TBSL*100,4) ||'%' 同比数量,ROUND(HBSL*100,4) ||'%' 环比数量,SYJE 使用金额,ROUND(JEZB*100,4) ||'%' 金额占比,ROUND(TBJE*100,4) ||'%' 同比金额,ROUND(HBJE*100,4) ||'%' 环比金额 from(SELECT C.GDSEQ, C.GDNAME,NVL(SUM(DECODE(B.BILLTYPE,'XST',-ABS(B.SL),'XSD',ABS(B.SL),'XSG',ABS(B.SL),'DSH',ABS(B.SL))),0)SL,
                                  ROUND(NVL(SUM(DECODE(B.BILLTYPE,'XST',-ABS(B.SL),'XSD',ABS(B.SL),'XSG',ABS(B.SL),'DSH',ABS(B.SL))),0)/{2},4) SLZB,
                                  F_GETBL('DPSLTB','{0}','{1}','{4}',C.GDSEQ,'','','') TBSL，
                                  F_GETBL('DPSLHB','{0}','{1}','{4}',C.GDSEQ,'','','') HBSL,
                                  NVL(SUM(DECODE(B.BILLTYPE,'XST',-ABS(B.HSJE),'XSG',ABS(B.HSJE),'XSD',ABS(B.HSJE),'DSH',ABS(B.HSJE))),0) SYJE,  
                                  ROUND(NVL(SUM(DECODE(B.BILLTYPE,'XST',-ABS(B.HSJE),'XSD',ABS(B.HSJE),'XSG',ABS(B.HSJE),'DSH',ABS(B.HSJE))),0)/{3},4) JEZB,
                                  F_GETBL('DPJETB','{0}','{1}','{4}',C.GDSEQ,'','','') TBJE,                        
                                  F_GETBL('DPJEHB','{0}','{1}','{4}',C.GDSEQ,'','','') HBJE              
                        FROM SYS_DEPT A,DAT_GOODSJXC B,DOC_GOODS C
                        WHERE A.CODE = B.DEPTID AND A.TYPE = '3' 
                        AND B.RQSJ >= TO_DATE('{0}','yyyy-MM-dd') AND B.RQSJ<TO_DATE('{1}','yyyy-MM-dd') + 1
                        AND B.KCADD < 0 AND B.GDSEQ = C.GDSEQ";
                string strWhere = "";
                if (lstDEPTID.SelectedValue.Length > 0) strWhere += " AND B.DEPTID = '" + lstDEPTID.SelectedValue + "'";
                if (strWhere.Trim().Length > 0) strSql = strSql + strWhere;
                strSql += " GROUP BY C.GDSEQ,   C.GDNAME) where sl<>0";
                string sortField = GridGoods.SortField;
                string sortDirection = GridGoods.SortDirection;
                string tempsql = string.Format(strSql, dpkDATE1.Text, dpkDATE2.Text, strSql2, strSql3, lstDEPTID.SelectedValue.ToString()) + String.Format(" ORDER BY 使用金额 DESC");
                DataTable dt = DbHelperOra.QueryForTable(tempsql);
                ExcelHelper.ExportByWeb(dt, "单品消耗排行导出", "单品消耗排行导出_" + DateTime.Now.ToString("yyyyMMddHH") + ".xls");
            }
            if (TabStrip1.ActiveTabIndex == 1)
            {
                if (GridList.Rows.Count < 1)
                {
                    Alert.Show("没有数据,无法导出！");
                    return;
                }
                string strSql = @" SELECT ' '||B.GDSEQ 商品编码,B.GDNAME 商品名称,B.GDSPEC 规格,F_GETDEPTNAME(A.DEPTID) 部门,f_getunitname(B.UNIT) 单位, B.HSJJ 单价,nvl(A.SL,0)使用数量,NVL(A.HSJE,0)使用金额,
                                    f_getproducername(B.PRODUCER) 生产商,B.PIZNO 批准文号,
                                   ROUND((NVL(A.SL,0)/(SELECT SUM(DECODE(AA.BILLTYPE,'XST',-ABS(AA.SL),'XSG',ABS(AA.SL),'XSD',ABS(AA.SL),'DSH',ABS(AA.SL)))
                                    FROM DAT_GOODSJXC AA
                                    WHERE AA.RQSJ>=TO_DATE('{0}','yyyy-MM-dd') AND AA.RQSJ<TO_DATE('{1}','yyyy-MM-dd') + 1 AND AA.DEPTID = A.DEPTID AND AA.KCADD='-1')),4) 数量占比,
                                    ROUND((nvl(A.HSJE,0)/(SELECT SUM(DECODE(AA.BILLTYPE,'XST',-ABS(AA.HSJE),'XSD',ABS(AA.HSJE),'XSG',ABS(AA.HSJE),'DSH',ABS(AA.HSJE)))
                                    FROM DAT_GOODSJXC AA
                                    WHERE AA.RQSJ >= TO_DATE('{0}','yyyy-MM-dd') AND AA.RQSJ<TO_DATE('{1}','yyyy-MM-dd') + 1 AND AA.DEPTID = A.DEPTID AND AA.KCADD='-1')),4) 金额占比,
                                    F_GETBL('KSSLHB','{0}','{1}',A.DEPTID,B.GDSEQ,'','','') 数量环比,F_GETBL('KSSLTB','{0}','{1}',A.DEPTID,B.GDSEQ,'','','') 数量同比,
                                   F_GETBL('KSJEHB','{0}','{1}',A.DEPTID,B.GDSEQ,'','','') 金额环比,F_GETBL('KSJETB','{0}','{1}',A.DEPTID,B.GDSEQ,'','','') 金额同比
                            FROM (SELECT A.DEPTID,A.GDSEQ,SUM(DECODE(A.BILLTYPE,'XST',-ABS(A.SL),'XSG',ABS(A.SL),'XSD',ABS(A.SL),'DSH',ABS(A.SL))) SL
                                    ,SUM(DECODE(A.BILLTYPE,'XST',-ABS(A.HSJE),'XSD',ABS(A.HSJE),'XSG',ABS(A.HSJE),'DSH',ABS(A.HSJE))) HSJE
                            FROM DAT_GOODSJXC A
                            WHERE A.RQSJ>= TO_DATE('{0}','yyyy-MM-dd') AND A.RQSJ<TO_DATE('{1}','yyyy-MM-dd') + 1
                            AND A.DEPTID IN(SELECT CODE FROM SYS_DEPT WHERE TYPE='3') AND A.KCADD<0
                            GROUP BY A.DEPTID,A.GDSEQ) A,DOC_GOODS B
                            WHERE A.GDSEQ = B.GDSEQ";
                string strWhere = "";
                string strWhere2 = "";
                if (ddlDEPTID.SelectedValue.Length > 1) strWhere2 += " AND A.DEPTID = '" + ddlDEPTID.SelectedValue + "'";
                if (lisGDSEQ.Text.Trim().Length > 0)
                {
                    strWhere += String.Format(" AND (B.GDSEQ LIKE '%{0}%' OR B.GDNAME LIKE '%{0}%' OR B.ZJM LIKE '%{0}%')", lisGDSEQ.Text.Trim());
                }
                //  if (lstISGZ.SelectedValue.Length > 0) strWhere += " AND B.ISGZ = '" + lstISGZ.SelectedValue + "'";
                if (strWhere.Trim().Length > 0) strSql = strSql + strWhere;
                strSql += "  AND SL<>0";
                string tempsql = string.Format(strSql, lisDATE1.Text, lisDATE2.Text) + String.Format(" ORDER BY 使用金额 DESC");
                DataTable dtData = DbHelperOra.QueryForTable(tempsql);
                //DataTable dtData = PubFunc.DbGetPage(GridList.PageIndex, GridList.PageSize, string.Format(strSql, dpkDATE1.Text, dpkDATE2.Text, strWhere2) + String.Format(" ORDER BY 使用金额 DESC" ), ref total);
                ExcelHelper.ExportByWeb(dtData, "商品使用排行导出", "商品使用排行导出_" + DateTime.Now.ToString("yyyyMMddHH") + ".xls");
            }
        }

        protected void GridGoods_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            //ddlDEPTID.SelectedValue = GridGoods.DataKeys[e.RowIndex][0].ToString();
            //  lstISGZ.SelectedValue = ddlISGZ.SelectedValue;
            ddlDEPTID.SelectedValue = lstDEPTID.SelectedValue;
            lisDATE1.SelectedDate = dpkDATE1.SelectedDate;
            lisDATE2.SelectedDate = dpkDATE2.SelectedDate;
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



    }
}