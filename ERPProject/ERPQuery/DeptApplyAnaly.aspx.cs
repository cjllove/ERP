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

namespace ERPProject.ERPQuery
{
    public partial class DeptApplyAnaly : PageBase
    {
        public DeptApplyAnaly()
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
            PubFunc.DdlDataGet("DDL_SYS_DEPT", lstDEPTID, ddlDEPTID);
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


            string strSql = @"SELECT  MM.*,NVL(NN.GDNUM,0)GDNUM
                                FROM(
                                SELECT DEPTID,
                                       DEPTNAME,
                                       NVL(SUM(BILLNUM), 0) BILLNUM,
                                       NVL(SUM(APPLYNUM), 0) APPLYNUM,
                                       NVL(SUM(APPGDNUM) / COUNT(X.DEPTID), 0) APPGDNUM,
                                       NVL(SUM(HSJE) / COUNT(X.DEPTID), 0) HSJE,
                                       NVL(SUM(PASSMONEY) / COUNT(X.DEPTID), 0) PASSMONEY
                                  FROM (SELECT A.DEPTID,
                                               F_GETDEPTNAME(A.DEPTID) DEPTNAME,A.SEQNO,
                                               (SELECT COUNT(SEQNO)
                                                  FROM DAT_CK_DOC
                                                 WHERE FLAG = 'Y'
                                                   AND SEQNO IN (SELECT STR2
                                                                   FROM DAT_SL_DOC
                                                                  WHERE SEQNO = A.SEQNO
                                                                    AND STR2 IS NOT NULL)) BILLNUM,
                                               (SELECT COUNT(SEQNO)
                                                  FROM DAT_SL_DOC
                                                 WHERE FLAG IN ('Y', 'M', 'W', 'N', 'B', 'D', 'S')
                                                   AND SEQNO = A.SEQNO) APPLYNUM,
               
                                               (SELECT COUNT(DISTINCT DSC.GDSEQ)
                                                  FROM DAT_SL_COM DSC, DAT_SL_DOC DSD, DOC_GOODS DG
                                                 WHERE DSC.SEQNO = DSD.SEQNO(+)
                                                   AND DSC.GDSEQ = DG.GDSEQ
                                                   AND DG.FLAG IN ('Y', 'T')
                                                   AND DSD.DEPTID = A.DEPTID
                                                   AND DG.ISGZ LIKE (NVL('{0}', '%'))
                                                   AND DSD.DEPTID LIKE NVL('{1}', '%')
                                                   AND DSD.FLAG <> 'R'
                                                   AND DSD.XSRQ BETWEEN TO_DATE('{2}', 'YYYY-MM-DD') AND
                                                       TO_DATE('{3}', 'YYYY-MM-DD')) APPGDNUM,
                                               (SELECT SUM(DSC.HSJE)
                                                  FROM DAT_SL_COM DSC, DAT_SL_DOC DSD, DOC_GOODS DG
                                                 WHERE DSC.SEQNO = DSD.SEQNO(+)
                                                   AND DSC.GDSEQ = DG.GDSEQ
                                                   AND DSD.DEPTID = A.DEPTID
                                                   AND DG.FLAG IN ('Y', 'T')
                                                   AND DSD.FLAG <> 'R'
                                                   AND DG.ISGZ LIKE (NVL('{0}', '%'))
                                                   AND DSD.DEPTID LIKE NVL('{1}', '%')
                                                   AND DSD.XSRQ BETWEEN TO_DATE('{2}', 'YYYY-MM-DD') AND
                                                       TO_DATE('{3}', 'YYYY-MM-DD')) HSJE,
               
                                               (SELECT ((SELECT NVL(SUM(CK_COM.HSJE), 0)
                                                           FROM DAT_SL_DOC DOC,
                                                                DAT_CK_DOC CK,
                                                                DAT_CK_COM CK_COM,
                                                                DOC_GOODS  DG
                                                          WHERE CK.FLAG = 'Y'
                                                            AND DOC.STR2 = CK.SEQNO
                                                            AND CK_COM.GDSEQ = DG.GDSEQ
                                                            AND DG.FLAG IN ('Y', 'T')
                                                            AND CK_COM.SEQNO = CK.SEQNO
                                                            AND DG.ISGZ LIKE (NVL('{0}', '%'))
                                                            AND DOC.STR2 IS NOT NULL
                                                            AND DOC.DEPTID LIKE NVL('{1}', '%')
                                                            AND DOC.DEPTID = A.DEPTID
                                                            AND CK.FLAG = 'Y'
                                                            AND DOC.FLAG IN ('Y', 'B')
                                                            AND DOC.XSRQ BETWEEN
                                                                TO_DATE('{2}', 'YYYY-MM-DD') AND
                                                                TO_DATE('{3}', 'YYYY-MM-DD')) +
                                                       (SELECT NVL(SUM(SLCOM.HSJE), 0)
                                                           FROM DAT_SL_DOC       SLDOC,
                                                                DAT_NOSTOCK_LIST NOSTOCK,
                                                                DAT_CK_COM       SLCOM,
                                                                DOC_GOODS        DG
                                                          WHERE NOSTOCK.BILLNO_SL = SLDOC.BILLNO
                                                            AND SLCOM.GDSEQ = DG.GDSEQ
                                                            AND DG.FLAG IN ('Y', 'T')
                                                            AND ',' || SLCOM.SEQNO = NOSTOCK.BILLNO_CK
                                                            AND DG.ISGZ LIKE (NVL('{0}', '%'))
                                                            AND NOSTOCK.BILLNO_CK IS NOT NULL
                                                            AND SLDOC.DEPTID LIKE NVL('{1}', '%')
                                                            AND SLDOC.DEPTID = A.DEPTID
                                                            AND SLDOC.XSRQ BETWEEN
                                                                TO_DATE('{2}', 'YYYY-MM-DD') AND
                                                                TO_DATE('{3}', 'YYYY-MM-DD')))
                                                  FROM DUAL) PASSMONEY
        
                                          FROM DAT_SL_DOC A, DAT_SL_COM B, SYS_DEPT SD, DOC_GOODS DG
                                         WHERE XSRQ BETWEEN TO_DATE('{2}', 'YYYY-MM_DD') AND
                                               TO_DATE('{3}', 'YYYY-MM_DD')
                                           AND A.DEPTID = SD.CODE(+)
                                           AND B.GDSEQ = DG.GDSEQ
                                           AND SD.TYPE <> '1'
                                           AND A.SEQNO = B.SEQNO(+)
                                           AND B.GDSEQ = DG.GDSEQ(+)
                                           AND DG.FLAG IN ('Y', 'T')
                                           AND DG.ISGZ LIKE (NVL('{0}', '%'))
                                           AND A.DEPTID LIKE NVL('{1}', '%')
                                         GROUP BY A.DEPTID, A.FLAG, A.SEQNO) X
                                 GROUP BY DEPTID, DEPTNAME
                                 ORDER BY HSJE DESC) MM,((SELECT COUNT(DISTINCT(GDSEQ))GDNUM,DEPTID
                                                  FROM (SELECT CKCOM.GDSEQ, DSD.DEPTID, DSD.SEQNO
                                                          FROM DAT_SL_DOC DSD,
                                                               DOC_GOODS  DG,
                                                               DAT_CK_DOC CK,
                                                               DAT_CK_COM CKCOM
                                                         WHERE DSD.DEPTID LIKE (NVL('{1}', '%'))
                                                           AND DG.ISGZ LIKE (NVL('{0}', '%'))
                                                           AND CK.SEQNO = DSD.STR2
                                                           AND CKCOM.SEQNO = CK.SEQNO
                                                           AND DSD.STR2 IS NOT NULL
                                                           AND CK.FLAG = 'Y'
                                                           AND DSD.FLAG IN ('Y', 'B')
                                                           AND DG.FLAG IN ('Y', 'T')
                                                           AND DSD.XSRQ BETWEEN
                                                               TO_DATE('{2}', 'YYYY-MM-DD') AND
                                                               TO_DATE('{3}', 'YYYY-MM-DD')
                                                        UNION ALL
                                                        SELECT NOSTOCK.GDSEQ, SLDOC.DEPTID, SLDOC.SEQNO
                                                          FROM DAT_SL_DOC SLDOC, DAT_NOSTOCK_LIST NOSTOCK,DOC_GOODS DG
                                                         WHERE NOSTOCK.BILLNO_SL = SLDOC.BILLNO
                                                           AND NOSTOCK.BILLNO_CK IS NOT NULL
                                                           AND NOSTOCK.GDSEQ=DG.GDSEQ AND DG.ISGZ LIKE NVL('{0}','%')
                                                           AND SLDOC.DEPTID LIKE NVL('{1}', '%')
                                                           AND SLDOC.XSRQ BETWEEN
                                                               TO_DATE('{2}', 'YYYY-MM-DD') AND
                                                               TO_DATE('{3}', 'YYYY-MM-DD'))
                                                GROUP BY DEPTID))NN
                   WHERE MM.DEPTID=NN.DEPTID(+) ";
            string strWhere = "";
            // if (lstDEPTID.SelectedValue.Length > 0) strWhere += " AND A.DEPTID = '" + lstDEPTID.SelectedValue + "'";
            // if (!PubFunc.StrIsEmpty(ddlISGZ.SelectedValue)) strWhere += " AND C.ISGZ = '" + ddlISGZ.SelectedValue + "'";
            if (strWhere.Trim().Length > 0) strSql = strSql + strWhere;
            string sortField = GridGoods.SortField;
            string sortDirection = GridGoods.SortDirection;
            string tempsql = string.Format(strSql, ddlISGZ.SelectedValue, lstDEPTID.SelectedValue, dpkDATE1.Text, dpkDATE2.Text) + String.Format(" ORDER BY {0} {1}", sortField, sortDirection);
            DataTable dt = DbHelperOra.QueryForTable(tempsql);
            GridGoods.DataSource = dt;
            GridGoods.DataBind();

            JObject summary = new JObject();
            hfdArray.Text = "";
            hfdArrayVal.Text = "";
            hfdArrayVal2.Text = "";
            Decimal SL = 0, HSJE = 0, total = 0, totaltb = 0, totalhb = 0;
            int i = 0;
            if (dt.Rows.Count < 1)
            {
                //hfdArray.Text = "无查询数据信息,";
                hfdArrayVal.Text = "0$无查询数据信息,";
                hfdArrayVal2.Text += "00$" + "0$无查询数据信息" + ",";
            }
            foreach (DataRow dr in dt.Rows)
            {
                SL += Convert.ToDecimal(dr["BILLNUM"]);
                HSJE += Convert.ToDecimal(dr["HSJE"]);
                totaltb += Convert.ToDecimal(dr["HSJE"]);    //* Convert.ToDecimal(dr["JETB"]);
                totalhb += Convert.ToDecimal(dr["HSJE"]); ///* Convert.ToDecimal(dr["JEHB"]);
                if (Convert.ToDecimal(dr["HSJE"]) > 0)
                {
                    if (i > 9)
                    {
                        total += Convert.ToDecimal(dr["HSJE"].ToString());
                    }
                    else
                    {
                        hfdArray.Text += dr["DEPTNAME"] + ",";
                        hfdArrayVal.Text += dr["HSJE"] + "$" + dr["DEPTNAME"] + ",";
                        hfdArrayVal2.Text += dr["HSJE"] + "|" + dr["PASSMONEY"] + "$" + dr["DEPTNAME"] + ",";
                    }
                    i++;
                }
            }
            if (total > 0)
            {
                hfdArray.Text += "其他,";
                hfdArrayVal.Text += total.ToString() + "$其他,";
                hfdArrayVal2.Text += "$其他,";
            }
            hfdArray.Text = hfdArray.Text.TrimEnd(',');
            hfdArrayVal.Text = hfdArrayVal.Text.TrimEnd(',');
            hfdArrayVal2.Text = hfdArrayVal2.Text.TrimEnd(',');
            //  hfdArrayVal2.Text = HSJE.ToString() + "," + (Math.Round(totaltb, 2)).ToString() + "," + (Math.Round(totalhb, 2)).ToString();
            summary.Add("DEPTNAME", "本页合计");
            summary.Add("BILLNUM", SL.ToString("F2"));
            summary.Add("HSJE", HSJE.ToString("F2"));
            GridGoods.SummaryData = summary;
            PageContext.RegisterStartupScript("getEcharsData2();getEcharsData();updateDate();");
        }
        protected void btnSch_Click(object sender, EventArgs e)
        {
            DataMXSearch();
        }
            private void DataMXSearch()
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
            string strSql = @"SELECT MM.*,
                           NVL(NN.PSSL, 0) PSSL,
                           ROUND(NVL(PSSL, 0) / NVL(MM.DHSL, 0), 4) ARRIVERATE
                      FROM (SELECT DEPTID,
                                   DEPTNAME,
                                   GDSEQ,
                                   GDNAME,
                                   NVL(SUM(DHSL) / COUNT(X.DEPTID), 0) DHSL        
                              FROM (SELECT A.DEPTID,
                                           F_GETDEPTNAME(A.DEPTID) DEPTNAME,
                                           B.GDSEQ,
                                           DG.GDNAME,
                                           (SELECT SUM(DSC.DHSL)
                                              FROM DAT_SL_COM DSC, DAT_SL_DOC DSD, DOC_GOODS DG
                                             WHERE DSC.SEQNO = DSD.SEQNO(+)
                                               AND DSC.GDSEQ = DG.GDSEQ
                                               AND DSD.DEPTID = A.DEPTID
                                               AND DSC.GDSEQ = B.GDSEQ
                                               AND DG.FLAG IN ('Y', 'T')
                                               AND DSD.FLAG <> 'R'
                                               AND DG.ISGZ LIKE (NVL('{0}', '%'))
                                               AND DSD.DEPTID LIKE NVL('{1}', '%')
                                               AND DSD.XSRQ BETWEEN
                                                   TO_DATE('{2}', 'YYYY-MM-DD') AND
                                                   TO_DATE('{3}', 'YYYY-MM-DD')
                                               AND  (DG.GDSEQ LIKE '%{4}%' OR DG.GDNAME LIKE '%{4}%' OR
                                                  DG.ZJM LIKE '%{4}%')) DHSL
                
                                      FROM DAT_SL_DOC A, DAT_SL_COM B, SYS_DEPT SD, DOC_GOODS DG
                                     WHERE XSRQ BETWEEN TO_DATE('{2}', 'YYYY-MM_DD') AND
                                           TO_DATE('{3}', 'YYYY-MM_DD')
                                       AND A.DEPTID = SD.CODE(+)
                                       AND B.GDSEQ = DG.GDSEQ
                                       AND SD.TYPE <> '1'
                                       AND A.SEQNO = B.SEQNO(+)
                                       AND B.GDSEQ = DG.GDSEQ(+)
                                       AND DG.FLAG IN ('Y', 'T')
                                       AND DG.ISGZ LIKE (NVL('{0}', '%'))
                                       AND A.DEPTID LIKE NVL('{1}', '%')
                                       AND  (DG.GDSEQ LIKE '%{4}%' OR DG.GDNAME LIKE '%{4}%' OR
                                     DG.ZJM LIKE '%{4}%')
                                     GROUP BY A.DEPTID, A.FLAG, A.SEQNO, B.GDSEQ, DG.GDNAME) X
                             GROUP BY DEPTID, DEPTNAME, GDSEQ, GDNAME
                             ORDER BY DEPTID DESC) MM,
                           (((SELECT NVL(SUM(CK_COM.DHSL), 0) PSSL, DOC.DEPTID, CK_COM.GDSEQ
                                FROM DAT_SL_DOC DOC,
                                     DAT_CK_DOC CK,
                                     DAT_CK_COM CK_COM,
                                     DOC_GOODS  DG
                               WHERE CK.FLAG = 'Y'
                                 AND DOC.STR2 = CK.SEQNO
                                 AND CK_COM.GDSEQ = DG.GDSEQ
                                 AND DG.FLAG IN ('Y', 'T')
                                 AND CK_COM.SEQNO = CK.SEQNO
                                 AND DG.ISGZ LIKE (NVL('{0}', '%'))
                                 AND DOC.STR2 IS NOT NULL
                                 AND DOC.DEPTID LIKE NVL('{1}', '%')
                                 AND CK.FLAG = 'Y'
                                 AND DOC.FLAG IN ('Y', 'B')
                                 AND DOC.XSRQ BETWEEN TO_DATE('{2}', 'YYYY-MM-DD') AND
                                     TO_DATE('{3}', 'YYYY-MM-DD')
                                 AND  (DG.GDSEQ LIKE '%{4}%' OR DG.GDNAME LIKE '%{4}%' OR
                                     DG.ZJM LIKE '%{4}%')
                               GROUP BY DOC.DEPTID, CK_COM.GDSEQ) UNION ALL
                            (SELECT NVL(SUM(NOSTOCK.SLSL), 0) PSSL, SLDOC.DEPTID, SLCOM.GDSEQ
                                FROM DAT_SL_DOC       SLDOC,
                                     DAT_NOSTOCK_LIST NOSTOCK,
                                     DAT_CK_COM       SLCOM,
                                     DOC_GOODS        DG
                               WHERE NOSTOCK.BILLNO_SL = SLDOC.BILLNO
                                 AND SLCOM.GDSEQ = DG.GDSEQ
                                 AND DG.FLAG IN ('Y', 'T')
                                 AND SLCOM.SEQNO = NOSTOCK.BILLNO_CK
                                 AND DG.ISGZ LIKE (NVL('{0}', '%'))
                                 AND NOSTOCK.BILLNO_CK IS NOT NULL
                                 AND SLDOC.DEPTID LIKE NVL('{1}', '%')
                                 AND SLDOC.XSRQ BETWEEN TO_DATE('{2}', 'YYYY-MM-DD') AND
                                     TO_DATE('{3}', 'YYYY-MM-DD')
                                 AND (DG.GDSEQ LIKE '%{4}%' OR DG.GDNAME LIKE '%{4}%' OR
                                     DG.ZJM LIKE '%{4}%')
                               GROUP BY SLDOC.DEPTID, SLCOM.GDSEQ))) NN
                     WHERE MM.DEPTID = NN.DEPTID(+)
                       AND MM.GDSEQ = NN.GDSEQ(+) ";
            int total = 0;
            string sortField = GridList.SortField;
            string sortDirection = GridList.SortDirection;
            string tempsql = string.Format(strSql, lstISGZ.SelectedValue, ddlDEPTID.SelectedValue, lisDATE1.Text, lisDATE2.Text, lisGDSEQ.Text.Trim()) + String.Format(" ORDER BY {0} {1}", sortField, sortDirection);
            DataTable dtData = PubFunc.DbGetPage(GridList.PageIndex, GridList.PageSize, tempsql, ref total);
            GridList.RecordCount = total;
            GridList.DataSource = dtData;
            GridList.DataBind();

            Decimal DHSL = 0, ARRIVESL = 0;
            JObject summary = new JObject();
            foreach (DataRow dr in dtData.Rows)
            {
                DHSL += Convert.ToDecimal(dr["DHSL"]);
                ARRIVESL += Convert.ToDecimal(dr["PSSL"]);
            }
            summary.Add("GDNAME", "本页合计");
            summary.Add("DHSL", DHSL.ToString("F2"));
            summary.Add("PSSL", ARRIVESL.ToString("F2"));
            GridList.SummaryData = summary;
        }
        protected void GridGoods_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridList.PageIndex = e.NewPageIndex;
            DataMXSearch();
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

                Response.Write(Doc.GetGridTableHtml(GridGoods));

                Response.End();
                btnExp.Enabled = true;
            }
            if (TabStrip1.ActiveTabIndex == 1)
            {
                if (GridList.Rows.Count < 1)
                {
                    Alert.Show("没有数据,无法导出！", MessageBoxIcon.Warning);
                    return;
                }
                string strSql = @"SELECT MM.DEPTNAME 科室名称,' '||MM.GDSEQ 商品编码,MM.GDNAME 商品名称,MM.DHSL 申请数量,
                           NVL(NN.PSSL, 0) 配送数量,
                           ROUND(NVL(PSSL, 0) / NVL(MM.DHSL, 0), 4) 满足率
                      FROM (SELECT DEPTID,
                                   DEPTNAME,
                                   GDSEQ,
                                   GDNAME,
                                   NVL(SUM(DHSL) / COUNT(X.DEPTID), 0) DHSL        
                              FROM (SELECT A.DEPTID,
                                           F_GETDEPTNAME(A.DEPTID) DEPTNAME,
                                           B.GDSEQ,
                                           DG.GDNAME,
                                           (SELECT SUM(DSC.DHSL)
                                              FROM DAT_SL_COM DSC, DAT_SL_DOC DSD, DOC_GOODS DG
                                             WHERE DSC.SEQNO = DSD.SEQNO(+)
                                               AND DSC.GDSEQ = DG.GDSEQ
                                               AND DSD.DEPTID = A.DEPTID
                                               AND DSC.GDSEQ = B.GDSEQ
                                               AND DG.FLAG IN ('Y', 'T')
                                               AND DSD.FLAG <> 'R'
                                               AND DG.ISGZ LIKE (NVL('{0}', '%'))
                                               AND DSD.DEPTID LIKE NVL('{1}', '%')
                                               AND DSD.XSRQ BETWEEN
                                                   TO_DATE('{2}', 'YYYY-MM-DD') AND
                                                   TO_DATE('{3}', 'YYYY-MM-DD')
                                               AND  (DG.GDSEQ LIKE '%{4}%' OR DG.GDNAME LIKE '%{4}%' OR
                                                  DG.ZJM LIKE '%{4}%')) DHSL
                
                                      FROM DAT_SL_DOC A, DAT_SL_COM B, SYS_DEPT SD, DOC_GOODS DG
                                     WHERE XSRQ BETWEEN TO_DATE('{2}', 'YYYY-MM_DD') AND
                                           TO_DATE('{3}', 'YYYY-MM_DD')
                                       AND A.DEPTID = SD.CODE(+)
                                       AND B.GDSEQ = DG.GDSEQ
                                       AND SD.TYPE <> '1'
                                       AND A.SEQNO = B.SEQNO(+)
                                       AND B.GDSEQ = DG.GDSEQ(+)
                                       AND DG.FLAG IN ('Y', 'T')
                                       AND DG.ISGZ LIKE (NVL('{0}', '%'))
                                       AND A.DEPTID LIKE NVL('{1}', '%')
                                       AND  (DG.GDSEQ LIKE '%{4}%' OR DG.GDNAME LIKE '%{4}%' OR
                                     DG.ZJM LIKE '%{4}%')
                                     GROUP BY A.DEPTID, A.FLAG, A.SEQNO, B.GDSEQ, DG.GDNAME) X
                             GROUP BY DEPTID, DEPTNAME, GDSEQ, GDNAME
                             ORDER BY DEPTID DESC) MM,
                           (((SELECT NVL(SUM(CK_COM.DHSL), 0) PSSL, DOC.DEPTID, CK_COM.GDSEQ
                                FROM DAT_SL_DOC DOC,
                                     DAT_CK_DOC CK,
                                     DAT_CK_COM CK_COM,
                                     DOC_GOODS  DG
                               WHERE CK.FLAG = 'Y'
                                 AND DOC.STR2 = CK.SEQNO
                                 AND CK_COM.GDSEQ = DG.GDSEQ
                                 AND DG.FLAG IN ('Y', 'T')
                                 AND CK_COM.SEQNO = CK.SEQNO
                                 AND DG.ISGZ LIKE (NVL('{0}', '%'))
                                 AND DOC.STR2 IS NOT NULL
                                 AND DOC.DEPTID LIKE NVL('{1}', '%')
                                 AND CK.FLAG = 'Y'
                                 AND DOC.FLAG IN ('Y', 'B')
                                 AND DOC.XSRQ BETWEEN TO_DATE('{2}', 'YYYY-MM-DD') AND
                                     TO_DATE('{3}', 'YYYY-MM-DD')
                                 AND  (DG.GDSEQ LIKE '%{4}%' OR DG.GDNAME LIKE '%{4}%' OR
                                     DG.ZJM LIKE '%{4}%')
                               GROUP BY DOC.DEPTID, CK_COM.GDSEQ) UNION ALL
                            (SELECT NVL(SUM(NOSTOCK.SLSL), 0) PSSL, SLDOC.DEPTID, SLCOM.GDSEQ
                                FROM DAT_SL_DOC       SLDOC,
                                     DAT_NOSTOCK_LIST NOSTOCK,
                                     DAT_CK_COM       SLCOM,
                                     DOC_GOODS        DG
                               WHERE NOSTOCK.BILLNO_SL = SLDOC.BILLNO
                                 AND SLCOM.GDSEQ = DG.GDSEQ
                                 AND DG.FLAG IN ('Y', 'T')
                                 AND SLCOM.SEQNO = NOSTOCK.BILLNO_CK
                                 AND DG.ISGZ LIKE (NVL('{0}', '%'))
                                 AND NOSTOCK.BILLNO_CK IS NOT NULL
                                 AND SLDOC.DEPTID LIKE NVL('{1}', '%')
                                 AND SLDOC.XSRQ BETWEEN TO_DATE('{2}', 'YYYY-MM-DD') AND
                                     TO_DATE('{3}', 'YYYY-MM-DD')
                                 AND (DG.GDSEQ LIKE '%{4}%' OR DG.GDNAME LIKE '%{4}%' OR
                                     DG.ZJM LIKE '%{4}%')
                               GROUP BY SLDOC.DEPTID, SLCOM.GDSEQ))) NN
                     WHERE MM.DEPTID = NN.DEPTID(+)
                       AND MM.GDSEQ = NN.GDSEQ(+) ";
                int total = 0;
                string sortField = GridList.SortField;
                string sortDirection = GridList.SortDirection;
                string tempsql = string.Format(strSql, lstISGZ.SelectedValue, ddlDEPTID.SelectedValue, lisDATE1.Text, lisDATE2.Text, lisGDSEQ.Text.Trim()) + String.Format(" ORDER BY {0} {1}", sortField, sortDirection);
                DataTable dtData = DbHelperOra.QueryForTable(tempsql);
                if (dtData.Rows.Count < 1)
                {
                    Alert.Show("无需要导出的数据信息!", MessageBoxIcon.Warning);
                    return;
                }
                ExcelHelper.ExportByWeb(dtData, "科室申领分析", "科室申领分析" + DateTime.Now.ToShortDateString() + ".xls");
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
