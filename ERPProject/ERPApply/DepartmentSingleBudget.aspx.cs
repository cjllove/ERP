﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using FineUIPro;
using Oracle.ManagedDataAccess.Client;
using XTBase;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Text;
using System.Collections;

namespace ERPProject.ERPApply
{
    public partial class DepartmentSingleBudget : BillBase
    {
        string gdSeq = string.Empty;


        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                if (Request.QueryString["gdseq"].ToString() != null)
                {

                    gdSeq = Request.QueryString["gdseq"].ToString();
                    hfgdSeq.Text = gdSeq;
                }
                if (Request.QueryString["deptid"].ToString() != null)
                {
                    hfDeptID.Text = Request.QueryString["deptid"].ToString();


                }
                DataInit();
                billNew();
                billSearch();


            }

        }

        #region  page load event

        public void DataInit()
        {

        }
        #endregion


        #region button envet 

        protected override void billNew()
        {


        }
        protected override void billSearch()
        {
            string currentMonth = DateTime.Now.ToString("MM");

            string startTime = string.Empty;
            string endTime = string.Empty;
            string ACCOUNTDAY = Doc.DbGetSysPara("ACCOUNTDAY");
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            if (ACCOUNTDAY == "31")
            {
                startDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy") + "-" + currentMonth + "-01");
                endDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy") + "-" + currentMonth + "-01").AddMonths(1);
            }
            else
            {
                startDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy") + "-" + currentMonth + "-" + ACCOUNTDAY).AddMonths(-1);
                endDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy") + "-" + currentMonth + "-" + ACCOUNTDAY);

            }


            //公式：科室本月实际消耗数量=ERP中科室使用消耗数-库存盘点数+系统库存数，
            //公式：ERP中科室使用消耗数=科室领用数-科室退货数。天津项目是没有定数的
            string strSql = string.Format(@"SELECT DISTINCT DEPTID,
                                   NVL(YSJE, 0) YSTOTAL,
                                   NVL(SJJE, 0) as TOTAL,
                                   Monthly
                              FROM (SELECT SA.DEPTID,
                                           NVL(SC.YSJE, 0) YSJE,
                                           NVL((NVL(F_GETXHSLBYMONTH(SA.DEPTID, SA.GDSEQ, SA.MONTHLY, '{4}', '0'), 0) -
                                                       NVL(SB.PDSL, 0) + F_GETKC(SA.DEPTID,SD.GDSEQ,ADD_MONTHS(TO_DATE(SA.MONTHLY||'-01','YYYY-MM-DD')-1,1))),
                                                       0) * SD.HSJJ SJJE,
                                           SA.MONTHLY
                                      FROM (SELECT DISTINCT DEPTID, GDSEQ, MONTHLY
                                              FROM (SELECT DEPTID, GDSEQ, to_char(RQSJ, 'YYYY-MM') as MONTHLY
                                                      FROM DAT_GOODSJXC A
                                                     WHERE RQSJ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                           TO_DATE('{1}', 'YYYY-MM-DD') + 1
                                                       AND EXISTS (SELECT 1
                                                              FROM SYS_DEPT
                                                             WHERE TYPE = '3'
                                                               AND CODE = A.DEPTID)
                                                    UNION
                                                    SELECT A.DEPTID,
                                                           B.GDSEQ,
                                                           to_char(A.SHRQ, 'YYYY-MM') as Monthly
                                                      FROM DAT_PD_DOC A, DAT_PD_COM B
                                                     WHERE A.SEQNO = B.SEQNO
                                                       AND A.PDTYPE = '3'
                                                       AND A.SPRQ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                           TO_DATE('{1}', 'YYYY-MM-DD') + 1
                                                    UNION
                                                    SELECT A.DEPTID,
                                                           B.GDSEQ,
                                                           to_char(A.SHRQ, 'YYYY-MM') as Monthly
                                                      FROM DAT_YS_DOC A, DAT_YS_COM B
                                                     WHERE A.SEQNO = B.SEQNO
                                                       AND A.YSRQ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                           TO_DATE('{1}', 'YYYY-MM-DD') + 1
                                            UNION
                                            (SELECT DEPTID, GDSEQ, to_Char(RQ, 'YYYY-MM') as MONTHLY
                                              FROM DAT_STOCKDAY
                                             WHERE DEPTID IN (SELECT CODE FROM SYS_DEPT WHERE TYPE = '3')
                                               AND RQ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                   TO_DATE('{1}', 'YYYY-MM-DD') + 1)))SA,
                                           (SELECT A.DEPTID,
                                                   B.GDSEQ,
                                                   SUM(B.PDSL) PDSL,
                                                   to_char(A.SPRQ, 'YYYY-MM') as Monthly
                                              FROM DAT_PD_DOC A, DAT_PD_COM B
                                             WHERE A.SEQNO = B.SEQNO
                                               AND A.PDTYPE = '3'
                                               AND A.SPRQ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                   TO_DATE('{1}', 'YYYY-MM-DD') + 1
                                             GROUP BY A.DEPTID, GDSEQ, to_char(A.SPRQ, 'YYYY-MM')) SB,
                                           (SELECT A.DEPTID,
                                                   GDSEQ,
                                                   SUM(B.HSJE) YSJE,
                                                   to_char(A.YSRQ, 'YYYY-MM') as Monthly
                                              FROM DAT_YS_DOC A, DAT_YS_COM B
                                             WHERE A.SEQNO = B.SEQNO
                                               AND A.YSRQ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                   TO_DATE('{1}', 'YYYY-MM-DD') + 1
                                             GROUP BY A.DEPTID, B.GDSEQ, to_char(A.YSRQ, 'YYYY-MM')) SC,
                                           DOC_GOODS SD,
                                           (SELECT  to_Char(RQ, 'YYYY-MM') as MONTHLY, 
                                                 SUM(T.KCSL) STOCKSL,
                                                  T.DEPTID, 
                                                 T.GDSEQ
                                                  FROM DAT_STOCKDAY T
                                                 WHERE NOT EXISTS (SELECT 1
                                                          FROM DAT_STOCKDAY
                                                         WHERE TO_CHAR(RQ, 'YYYY-MM') = to_char(t.RQ, 'YYYY-MM')
                                                           AND RQ > t.RQ)
 　
                                                   AND RQ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                       TO_DATE('{1}', 'YYYY-MM-DD') + 1 GROUP BY  to_Char(RQ, 'YYYY-MM') ,
                                                  T.DEPTID, 
                                                 T.GDSEQ) SE
                                     WHERE SA.DEPTID = SB.DEPTID(+)
                                       AND SA.GDSEQ = SB.GDSEQ(+)
                                       AND SA.MONTHLY = SB.MONTHLY(+)
                                       AND SA.DEPTID = SC.DEPTID(+)
                                       AND SA.GDSEQ = SC.GDSEQ(+)
                                       AND SA.MONTHLY = SC.MONTHLY(+)
                                       AND SA.GDSEQ = SD.GDSEQ
                                       AND SA.DEPTID = SE.DEPTID(+)
                                       AND SA.GDSEQ = SE.GDSEQ(+)
                                       AND SA.MONTHLY = SE.MONTHLY(+)
                                       AND SA.DeptID = '{2}'
                                       AND SA.GDSEQ = '{3}')
                                       ", startDate.AddMonths(-2).ToString("yyyy - MM - dd"), endDate.AddDays(-1).ToString("yyyy - MM - dd"), hfDeptID.Text, hfgdSeq.Text.Trim(), ACCOUNTDAY);

            //strSql += " group by to_char(rqsj, 'mm'), deptid, gdseq ";
            DataTable dt = DbHelperOra.Query(strSql).Tables[0];
            string sbBuget = string.Empty;
            string sbActual = string.Empty;

            string sysTit = "单个商品月度执行情况分析";

            string strGDSql = "select gdname from doc_goods where gdseq='" + hfgdSeq.Text + "'";
            object objName = DbHelperOra.GetSingle(strGDSql);
            string sysSubTit = startDate.ToString("yyyy年MM月") + objName.ToString() + "使用情况比较,  单位：（元）";
            string footLabel = "来源：系统数据库";
            string labelarr = string.Empty;// startDate.AddMonths(-2).ToString("MM") + "月," + startDate.AddMonths(-1).ToString("MM") + "月," + startDate.ToString("MM") + "月";
            decimal[] iBudget = new decimal[3];
            decimal[] iActival = new decimal[3];
            if (dt != null && dt.Rows.Count > 0)
            {
                int iRow = dt.Rows.Count < 3 ? dt.Rows.Count : 3;
                DataRow[] RowArr = dt.Select("1=1", "Monthly Asc");

                for (int i = 0; i < iRow; i++)
                {
                    labelarr += RowArr[i]["Monthly"].ToString() + "月,";
                    iBudget[i] = Convert.ToDecimal(RowArr[i]["ystotal"] != DBNull.Value ? RowArr[i]["ystotal"] : 0);
                    iActival[i] = Convert.ToDecimal(RowArr[i]["total"] != DBNull.Value ? RowArr[i]["total"] : 0);
                    //   iBudget[i] = 150;
                    //  iActival[i] =200;



                }

            }
            else
            {
                iBudget[0] = 0;
                iBudget[1] = 0;
                iBudget[2] = 0;
                iActival[0] = 0;
                iActival[1] = 0;
                iActival[2] = 0;

            }
            labelarr = labelarr.TrimEnd(',');

            var data = new ArrayList()
            {


              new { name = "执行",
                  value = iActival,
                  color = "#1385a5",
              },
              new  {
                 name = "预算",
                 value=iBudget,
                 color="#c56966",
                }
            };
            decimal aMax = iActival.Max();
            decimal bMax = iBudget.Max();
            decimal iMax = aMax > bMax ? aMax + aMax / 5 : bMax + bMax / 5;
            PageContext.RegisterStartupScript("reloaddata('" + labelarr + "','" + sysTit + "','" + sysSubTit + "','" + footLabel + "','" + JsonConvert.SerializeObject(data) + "'," + iMax + ");");


        }

        #endregion


    }
}