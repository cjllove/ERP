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
    public partial class SupghFx : PageBase
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
            dpkDATE1.SelectedDate = DateTime.Now.AddMonths(-1);
            dpkDATE2.SelectedDate = DateTime.Now;
            lisDATE1.SelectedDate = DateTime.Now.AddMonths(-1);
            lisDATE2.SelectedDate = DateTime.Now;
            PubFunc.DdlDataGet("DDL_SYS_DEPOTRANGE", UserAction.UserID, lstDEPTID, ddlDEPTID);
            PubFunc.DdlDataGet("DDL_DOC_SUPPLIERNULL",lstSUPPLIER,ddlSUPID);
        }
      
        private void DataSearch()
        {
            string strsql = "";
            string strwhere = "";
            string sortField = GridGoods.SortField;
            string sortDirection = GridGoods.SortDirection;
             DateTime begrq = Convert.ToDateTime(dpkDATE1.Text);
            DateTime endrq = Convert.ToDateTime(dpkDATE2.Text);
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
            strsql = string.Format(@"SELECT A.*,
       NVL(B.SLHBZ，0) SLHBZ,
       NVL(B.JEHBZ，0) JEHBZ,
       NVL(C.SLTBZ, 0) SLTBZ,
       NVL(C.JETBZ, 0) JETBZ,
(SELECT COUNT(DISTINCT(DGS.GDSEQ))
FROM DAT_GOODSSTOCK DGS
WHERE DGS.SUPID LIKE NVL(A.SUPID,'%') AND DGS.JHRQ>=TO_DATE('{0}','yyyy-MM-dd') AND DGS.JHRQ<=TO_DATE('{1}','yyyy-MM-dd'))SUMGDSEQ,
(SELECT NVL(SUM(NVL(DGS.KCSL,0)),0)
FROM DAT_GOODSSTOCK DGS
WHERE DGS.SUPID LIKE NVL(A.SUPID,'%') 
AND   DGS.KCSL>0 AND DGS.JHRQ>=TO_DATE('{0}','yyyy-MM-dd') AND DGS.JHRQ<=TO_DATE('{1}','yyyy-MM-dd'))SUMKCSL,
(SELECT NVL(SUM(NVL(DGS.KCHSJE,0)),0) 
FROM DAT_GOODSSTOCK DGS
WHERE DGS.SUPID LIKE NVL(A.SUPID,'%')
AND   DGS.KCSL>0 AND DGS.JHRQ>=TO_DATE('{0}','yyyy-MM-dd') AND DGS.JHRQ<=TO_DATE('{1}','yyyy-MM-dd'))SUMKCJE
  FROM （SELECT SUPID,
        SUPNAME,
       SUM(NVL(TSL, 0)) SYSL,
       ROUND(NVL(SUM(ZJ), 0), 2) SYJE
  FROM (SELECT DGJ.SUPID,DS.SUPNAME, SUM(DGJ.SL) TSL, SUM(DGJ.HSJJ * DGJ.SL) ZJ
          FROM DAT_GOODSJXC DGJ, DOC_GOODS DG,DOC_SUPPLIER DS
         WHERE DGJ.GDSEQ = DG.GDSEQ(+) AND DGJ.SUPID=DS.SUPID(+)
           AND TRUNC(DGJ.RQSJ, 'DD') BETWEEN TRUNC(TO_DATE('{0}','yyyy-MM-dd'), 'DD') AND
               TRUNC(TO_DATE('{1}','yyyy-MM-dd'), 'DD')
            AND DGJ.BILLTYPE = 'RKD'
           AND DGJ.SL > 0
           AND DG.ISGZ LIKE NVL('{2}','%')
           AND DGJ.DEPTID LIKE NVL('{3}','%')
         GROUP BY DGJ.SUPID,DS.SUPNAME)
 GROUP BY SUPID,SUPNAME) A,

 (SELECT SUPID, SUM(NVL(TSL, 0)) SLHBZ, ROUND(NVL(SUM(ZJ), 0), 2) JEHBZ
    FROM (SELECT DGJ.SUPID, SUM(DGJ.SL) TSL, SUM(DGJ.HSJJ * DGJ.SL) ZJ
            FROM DAT_GOODSJXC DGJ, DOC_GOODS DG
           WHERE DGJ.GDSEQ = DG.GDSEQ(+)
             AND TRUNC(DGJ.RQSJ, 'DD') BETWEEN TRUNC(TO_DATE('{0}','yyyy-MM-dd')-(TO_DATE('{1}','yyyy-MM-dd')-TO_DATE('{0}','yyyy-MM-dd')), 'DD') AND
                 TRUNC(TO_DATE('{0}','yyyy-MM-dd'), 'DD')
             AND DGJ.BILLTYPE = 'RKD'
             AND DGJ.SL > 0
           GROUP BY SUPID)
   GROUP BY SUPID) B,

 (SELECT SUPID, SUM(NVL(TSL, 0)) SLTBZ, ROUND(NVL(SUM(ZJ), 0), 2) JETBZ
    FROM (SELECT DGJ.SUPID, SUM(DGJ.SL) TSL, SUM(DGJ.HSJJ * DGJ.SL) ZJ
            FROM DAT_GOODSJXC DGJ, DOC_GOODS DG
           WHERE DGJ.GDSEQ = DG.GDSEQ(+)
             AND TRUNC(DGJ.RQSJ, 'DD') BETWEEN TRUNC(TO_DATE('{0}','yyyy-MM-dd')-365, 'DD') AND
                 TRUNC(TO_DATE('{1}','yyyy-MM-dd')-365, 'DD')
              AND DGJ.BILLTYPE = 'RKD'
             AND DGJ.SL > 0
           GROUP BY SUPID)
   GROUP BY SUPID) C

 WHERE A.SUPID = B.SUPID(+)
   AND A.SUPID = C.SUPID(+)", begrq.ToShortDateString(), endrq.ToShortDateString(), ddlISGZ.SelectedValue,lstDEPTID.SelectedValue);
            if (!string.IsNullOrEmpty(lstSUPPLIER.SelectedValue))
            {
                strwhere += " AND A.SUPID='" + lstSUPPLIER.SelectedValue + "' ";
            }
            //if (!string.IsNullOrEmpty(lstDEPTID.SelectedValue))
            //{
            //    strwhere += " AND A.DEPTID='" + lstDEPTID.SelectedValue + "' ";
            //}
            //if (!string.IsNullOrEmpty(ddlISGZ.SelectedValue))
            //{
            //    strwhere += " AND A.ISGZ='" + ddlISGZ.SelectedValue + "' ";
            //}


            //strsql += strwhere+" ORDER BY A.SYSL DESC ";
            //DataTable dt = DbHelperOra.QueryForTable(strsql);
            //GridGoods.DataSource = dt;
            //GridGoods.DataBind();

            //解决排序问题
            int total1 = 0;
            strsql += strwhere ;
            DataTable dt = PubFunc.DbGetPage(GridGoods.PageIndex, GridGoods.PageSize, strsql + String.Format(" ORDER BY {0} {1}", sortField, sortDirection), ref total1);
            GridGoods.RecordCount = total1;
            GridGoods.DataSource = dt;
            GridGoods.DataBind();

            JObject summary = new JObject();          
            hfdArrayVal.Text = "";
            hfdArrayVal2.Text = "";
            Decimal SL = 0, HSJE = 0, total = 0, totaltb = 0, totalhb = 0;
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                SL += Convert.ToDecimal(dr["SYSL"]);
                HSJE += Convert.ToDecimal(dr["SYJE"]);
                if (i > 8)
                {
                    total += Convert.ToDecimal(dr["SYSL"].ToString());
                }
                else
                {
                    hfdArray.Text += dr["SUPNAME"] + ",";
                    hfdArrayVal.Text += dr["SYSL"] + "$" + dr["SUPNAME"] + ",";
                }
                i++;
               
            }
            if (total > 0)
            {
                hfdArray.Text += "其他,";
                hfdArrayVal.Text += total.ToString() + "$其他,";
            }
            Totalsl.Text = SL.ToString();
            Totalje.Text = HSJE.ToString();
            hfdArrayVal.Text = hfdArrayVal.Text.TrimEnd(',');
            hfdArray.Text = hfdArray.Text.TrimEnd(',');
            hfdArrayVal2.Text = HSJE.ToString() + "," + (Math.Round(totaltb, 2)).ToString() + "," + (Math.Round(totalhb, 2)).ToString();
            summary.Add("SUPNAME", "本页合计");
            summary.Add("SYSL", SL.ToString("F2"));
            summary.Add("SYJE", HSJE.ToString("F2"));
            GridGoods.SummaryData = summary;
            PageContext.RegisterStartupScript("showpie();updateDate()");

        }
      
        protected void init_pie(DataTable dt, DateTime begrq, DateTime endrq)
        {
            string sbeg = "["; //开始标志
            string send = "]"; //结束标志
            string scon = "";
            string dbeg = "{";  //开始标志
            string dend = "}";  //结束标志
            string dcon = "";
            string sysl = "";
            int shownum = 9;    //显示前9个
            decimal showsum = 0;  //除了前9个其他的合数
            string syje = "";
            string je = "";
            decimal dqje = 0;
            decimal tbje = 0;
            decimal hbje = 0;
            int i = 0;
            string data1 = ""; //第一个参数
            string data2 = ""; //第二个参数
            if (dt.Rows.Count > 0)
            {

                foreach (DataRow dr in dt.Rows)
                {
                    if (i < shownum)
                    {
                        TimeSpan ts1 = new TimeSpan(begrq.Ticks);
                        TimeSpan ts2 = new TimeSpan(endrq.Ticks);
                        TimeSpan ts = ts1.Subtract(ts2).Duration();
                        sysl = DbHelperOra.GetSingle(string.Format("select F_GET_SUPPLIER_FX('GH','SYSL','{0}','{1}',TO_DATE('{2}','yyyy-MM-dd'),TO_DATE('{3}','yyyy-MM-dd'),'{4}')SYSL from dual", ddlDEPTID.SelectedValue, dr[1].ToString(), begrq.ToShortDateString(), endrq.ToShortDateString(), ddlISGZ.SelectedValue)).ToString();
                        scon = scon + "'" + dr[0].ToString().Replace("有限公司", "") + "',";
                        dcon = dcon + dbeg + "value:" + sysl + ",name:" + "'" + dr[0].ToString().Replace("有限公司", "") + "'" + dend + ",";
                        //  dqje += Convert.ToDecimal(DbHelperOra.GetSingle(string.Format(@"select F_GET_SUPPLIER_FX('GH','SYJE','{0}','{1}',TO_DATE('{2}','yyyy-MM-dd'),TO_DATE('{3}','yyyy-MM-dd'),'{4}')SYJE  from dual", ddlDEPTID.SelectedValue, dr[1].ToString(), begrq.ToShortDateString(), endrq.ToShortDateString(), ddlISGZ.SelectedValue)).ToString());
                        //  tbje += Convert.ToDecimal(DbHelperOra.GetSingle(string.Format(@" select F_GET_SUPPLIER_FX('GH','SYJE','{0}','{1}',TO_DATE('{2}','yyyy-MM-dd'),TO_DATE('{3}','yyyy-MM-dd'),'{4}')JETBSL from dual", ddlDEPTID.SelectedValue, dr[1].ToString(), begrq.AddYears(-1).ToShortDateString(), endrq.AddYears(-1).ToShortDateString(), ddlISGZ.SelectedValue)).ToString());
                        //  hbje += Convert.ToDecimal(DbHelperOra.GetSingle(string.Format(@"select F_GET_SUPPLIER_FX('GH','SYJE','{0}','{1}',TO_DATE('{2}','yyyy-MM-dd'),TO_DATE('{3}','yyyy-MM-dd'),'{4}')SYJE  from dual", ddlDEPTID.SelectedValue, dr[1].ToString(), (begrq.AddDays(ts.Days)).ToShortDateString(), begrq.ToShortDateString(), ddlISGZ.SelectedValue)).ToString());
                        // je = je + dbeg + "value:" + syje + ",name:" + "'" + dr[1].ToString() + "'" + dend + ",";
                        //  string data22 = "[" + je.Substring(0, je.Length - 1) + "]";                           
                        i++;
                    }
                    else
                    {
                        showsum += Convert.ToDecimal(dr[2].ToString());
                    }

                }
                if (showsum > 0)
                {
                    scon = scon + "'其他',";
                    dcon = dcon + dbeg + "value:" + showsum + ",name:" + "'其他'" + dend + ",";
                }
                data1 = sbeg + scon.Substring(0, scon.Length - 1) + send;
                data2 = "[" + dcon.Substring(0, dcon.Length - 1) + "]";
                //PageContext.RegisterStartupScript("getEcharsData2("+dqje+","+tbje+","+hbje+");");
                PageContext.RegisterStartupScript("showpie(" + data1 + "," + data2 + ");");
            }
            else
            {
                data1 = sbeg + scon + send;
                data2 = "[" + dcon + "]";
                PageContext.RegisterStartupScript("showpie(" + data1 + "," + data2 + ");");
            }
           
        }

        
             protected void btnSch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdfsupid.Text))
            {
                ddlSUPID.SelectedValue = hdfsupid.Text;
            }
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
//            string strSql = @"SELECT A.SL,A.HSJE,B.GDSEQ,B.GDNAME,B.GDSPEC,f_getunitname(B.UNIT) UNITNAME,f_getproducername(B.PRODUCER) PRODUCERNAME,
//                                   B.HSJJ,B.PIZNO,
//                                   ROUND((A.SL/(SELECT SUM(DECODE(AA.BILLTYPE,'XST',-ABS(AA.SL),'LTD',-ABS(AA.SL),'DST',-ABS(AA.SL),ABS(AA.SL)))
//                                    FROM DAT_GOODSJXC AA
//                                    WHERE AA.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd') + 1 AND AA.DEPTID = A.DEPTID)),4) SLZB,
//                                    ROUND((A.HSJE/(SELECT SUM(DECODE(AA.BILLTYPE,'XST',-ABS(AA.HSJE),'LTD',-ABS(AA.HSJE),'DST',-ABS(AA.HSJE),ABS(AA.HSJE)))
//                                    FROM DAT_GOODSJXC AA
//                                    WHERE AA.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd') + 1 AND AA.DEPTID = A.DEPTID)),4) JEZB,
//                                    F_GETBL('KSJEHB','{0}','{1}',A.DEPTID,B.GDSEQ,'','','') JEHB,F_GETBL('KSJETB','{0}','{1}',A.DEPTID,B.GDSEQ,'','','') JETB,
//                                    F_GETBL('KSSLHB','{0}','{1}',A.DEPTID,B.GDSEQ,'','','') SLHB,F_GETBL('KSSLTB','{0}','{1}',A.DEPTID,B.GDSEQ,'','','') SLTB
//                            FROM (SELECT A.DEPTID,A.GDSEQ,SUM(DECODE(A.BILLTYPE,'XST',-ABS(A.SL),'LTD',-ABS(A.SL),'DST',-ABS(A.SL),ABS(A.SL))) SL
//                                    ,SUM(DECODE(A.BILLTYPE,'XST',-ABS(A.HSJE),'LTD',-ABS(A.HSJE),'DST',-ABS(A.HSJE),ABS(A.HSJE))) HSJE
//                            FROM DAT_GOODSJXC A
//                            WHERE A.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd') + 1
//                         AND A.SL>0 AND A.SUPID LIKE NVL('{3}','%')  AND BILLTYPE ='RKD'       
//                            {2}
//                            GROUP BY A.DEPTID,A.GDSEQ) A,DOC_GOODS B
//                            WHERE A.GDSEQ = B.GDSEQ";
            string strSql = string.Format(@"SELECT A.*,
                                                           NVL(B.SLHBZ，0) SLHBZ,
                                                           NVL(B.JEHBZ，0) JEHBZ,
                                                           NVL(C.SLTBZ, 0) SLTBZ,
                                                           NVL(C.JETBZ, 0) JETBZ                                           
                                                  
                                                      FROM （SELECT GDSEQ,
                                                            GDNAME,
                                                           TSL SYSL,ZJ SYJE,PIZNO,UNITNAME,PRODUCERNAME,GDSPEC
                                                      FROM (SELECT DG.GDSEQ,DG.GDNAME, SUM(DGJ.SL) TSL, SUM(DGJ.HSJJ * DGJ.SL) ZJ,
                                                           DG.GDSPEC,f_getunitname(DG.UNIT) UNITNAME,f_getproducername(DG.PRODUCER) PRODUCERNAME,
                                                         DG.PIZNO
                                                              FROM DAT_GOODSJXC DGJ, DOC_GOODS DG,DOC_SUPPLIER DS
                                                             WHERE DGJ.GDSEQ = DG.GDSEQ(+) AND DGJ.SUPID=DS.SUPID(+)
                                                               AND TRUNC(DGJ.RQSJ, 'DD') BETWEEN TRUNC(TO_DATE('{0}','yyyy-MM-dd'), 'DD') AND
                                                                   TRUNC(TO_DATE('{1}','yyyy-MM-dd'), 'DD')
                                                                AND DGJ.BILLTYPE = 'RKD'
                                                               AND DGJ.SL > 0 
                                                               AND DGJ.SUPID LIKE(NVL('{2}','%'))
                                                             GROUP BY DG.GDSEQ,DG.GDNAME,DG.UNIT,DG.PRODUCER,DG.PIZNO,DG.GDSPEC)
                                                     ) A,

                                                     (SELECT GDSEQ, TSL SLHBZ, ROUND(NVL(ZJ, 0), 2) JEHBZ
                                                        FROM (SELECT DG.GDSEQ,SUM(DGJ.SL) TSL, SUM(DGJ.HSJJ * DGJ.SL) ZJ
                                                                FROM DAT_GOODSJXC DGJ, DOC_GOODS DG
                                                               WHERE DGJ.GDSEQ = DG.GDSEQ(+)
                                                                 AND TRUNC(DGJ.RQSJ, 'DD') BETWEEN TRUNC(TO_DATE('{0}','yyyy-MM-dd')-(TO_DATE('{1}','yyyy-MM-dd')-TO_DATE('{0}','yyyy-MM-dd')), 'DD') AND
                                                                     TRUNC(TO_DATE('{0}','yyyy-MM-dd'), 'DD')
                                                                 AND DGJ.BILLTYPE = 'RKD'
                                                                 AND DGJ.SL > 0
                                                               AND DGJ.SUPID LIKE(NVL('{2}','%'))
                                                               GROUP BY DG.GDSEQ)
                                                       ) B,

                                                     (SELECT GDSEQ, TSL SLTBZ, ROUND(ZJ, 2) JETBZ
                                                        FROM (SELECT DG.GDSEQ, SUM(NVL(DGJ.SL,0)) TSL, SUM(NVL(DGJ.HSJJ * DGJ.SL,0)) ZJ
                                                                FROM DAT_GOODSJXC DGJ, DOC_GOODS DG
                                                               WHERE DGJ.GDSEQ = DG.GDSEQ(+)
                                                                 AND TRUNC(DGJ.RQSJ, 'DD') BETWEEN TRUNC(TO_DATE('{0}','yyyy-MM-dd')-365, 'DD') AND
                                                                     TRUNC(TO_DATE('{1}','yyyy-MM-dd')-365, 'DD')
                                                                  AND DGJ.BILLTYPE = 'RKD'
                                                                 AND DGJ.SL > 0
                                                               AND DGJ.SUPID LIKE(NVL('{2}','%'))
                                                               GROUP BY DG.GDSEQ)
                                                       ) C

                                                     WHERE A.GDSEQ = B.GDSEQ(+)
                                                       AND A.GDSEQ = C.GDSEQ(+)", lisDATE1.Text, lisDATE2.Text,ddlSUPID.SelectedValue);
        
            int total = 0;
            string sortField = GridList.SortField;
            string sortDirection = GridList.SortDirection;

            //DataTable dtData = PubFunc.DbGetPage(GridList.PageIndex, GridList.PageSize,strSql, ref total);
            //GridList.RecordCount = total;

            //解决排序问题
            DataTable dtData = PubFunc.DbGetPage(GridList.PageIndex, GridList.PageSize, strSql + String.Format(" ORDER BY {0} {1}", sortField, sortDirection), ref total);
            GridList.RecordCount = total;
            GridList.DataSource = dtData;
            GridList.DataBind();
         

            Decimal SL = 0, HSJE = 0;
            JObject summary = new JObject();
            foreach (DataRow dr in dtData.Rows)
            {
                SL += Convert.ToDecimal(dr["SYSL"]);
                HSJE += Convert.ToDecimal(dr["SYJE"]);
            }
            TotalslMX.Text = SL.ToString();
            TotaljeMX.Text = HSJE.ToString();
            summary.Add("SUPNAME", "本页合计");
            summary.Add("SYSL", SL.ToString("F2"));
            summary.Add("HSJE", HSJE.ToString("F2"));
            GridList.SummaryData = summary;
            PageContext.RegisterStartupScript("updateDateMX();");
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
                Response.AddHeader("content-disposition", "attachment; filename=供应商供货汇总.xls");
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
                    Alert.Show("没有数据,无法导出！");
                    return;
                }
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment; filename=供应商供货明细.xls");
                Response.ContentType = "application/excel";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.Write(Doc.GetGridTableHtml(GridList));
                Response.End();
                btnExp.Enabled = true;
            }
        }

        protected void GridGoods_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            ddlDEPTID.SelectedValue = lstDEPTID.SelectedValue;
            lstISGZ.SelectedValue = ddlISGZ.SelectedValue;
            lisDATE1.SelectedDate = dpkDATE1.SelectedDate;
            lisDATE2.SelectedDate = dpkDATE2.SelectedDate;
            hdfsupid.Text = GridGoods.DataKeys[e.RowIndex][0].ToString();
            TabStrip1.ActiveTabIndex = 1;
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