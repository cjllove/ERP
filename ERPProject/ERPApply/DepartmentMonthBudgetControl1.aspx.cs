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
using System.Collections;

namespace ERPProject.ERPApply
{
    public partial class DepartmentMonthBudgetControl1 : BillBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                billSearch();
            }
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
                endDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy") + "-" + currentMonth + "-01").AddMonths(1).AddDays(-1);
            }
            else
            {
                startDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy") + "-" + currentMonth + "-" + ACCOUNTDAY).AddMonths(-1);
                endDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy") + "-" + currentMonth + "-" + ACCOUNTDAY);

            }
            string strSql = string.Format(@"SELECT SUM(NVL(ysje, 0)) YSTOTAL, SUM(NVL(sjje, 0)) as TOTAL, Monthly
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
                                               NVL(F_GETXHSLBYMONTH(SA.DEPTID, SA.GDSEQ, SA.MONTHLY, '{2}', '0'), 0) ERPSL,
                                               NVL(F_GETXHSLBYMONTH(SA.DEPTID, SA.GDSEQ, SA.MONTHLY, '{2}', '1'), 0)            
                                               ERPJE,
                                                F_GETKC(SA.DEPTID,SD.GDSEQ,ADD_MONTHS(TO_DATE(SA.MONTHLY||'-01','YYYY-MM-DD')-1,1)) STOCKSL,
                                                   NVL((NVL(F_GETXHSLBYMONTH(SA.DEPTID, SA.GDSEQ,  SA.MONTHLY, '{2}', '0'), 0) -
                                                       NVL(SB.PDSL, 0) + F_GETKC(SA.DEPTID,SD.GDSEQ,ADD_MONTHS(TO_DATE(SA.MONTHLY||'-01','YYYY-MM-DD')-1,1))),
                                                       0) SJSL,
                                                   NVL((NVL(F_GETXHSLBYMONTH(SA.DEPTID, SA.GDSEQ,  SA.MONTHLY, '{2}', '0'), 0) -
                                                       NVL(SB.PDSL, 0) + F_GETKC(SA.DEPTID,SD.GDSEQ,ADD_MONTHS(TO_DATE(SA.MONTHLY||'-01','YYYY-MM-DD')-1,1))),
                                                       0) * SD.HSJJ SJJE
               
                                              ,
                                               SA.MONTHLY
                                          FROM (SELECT DISTINCT DEPTID, GDSEQ, MONTHLY
                                                  FROM (SELECT DEPTID, GDSEQ, to_char(RQSJ, 'YYYY-MM') as MONTHLY
                                                          FROM DAT_GOODSJXC A
                                                         WHERE RQSJ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                               TO_DATE('{1}', 'YYYY-MM-DD') 
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
                                                               TO_DATE('{1}', 'YYYY-MM-DD') 
                                                        UNION
                                                        SELECT A.DEPTID,
                                                               B.GDSEQ,
                                                               to_char(A.SHRQ, 'YYYY-MM') as Monthly
                                                          FROM DAT_YS_DOC A, DAT_YS_COM B
                                                         WHERE A.SEQNO = B.SEQNO
                                                           AND A.YSRQ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                               TO_DATE('{1}', 'YYYY-MM-DD') 


                                                         UNION
                                            (SELECT DEPTID, GDSEQ, to_Char(RQ, 'YYYY-MM') as MONTHLY
                                              FROM DAT_STOCKDAY
                                             WHERE DEPTID IN (SELECT CODE FROM SYS_DEPT WHERE TYPE = '3')
                                               AND RQ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                   TO_DATE('{1}', 'YYYY-MM-DD')))) SA,
                                               (SELECT A.DEPTID,
                                                   B.GDSEQ,SUM(B.PDSL) PDSL,
                                                       SUM(B.HSJE) PDJE,
                                                       to_char(A.SPRQ, 'YYYY-MM') as Monthly
                                                  FROM DAT_PD_DOC A, DAT_PD_COM B
                                                 WHERE A.SEQNO = B.SEQNO
                                                   AND A.PDTYPE = '3'
                                                   AND A.SPRQ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                       TO_DATE('{1}', 'YYYY-MM-DD') 
                                                 GROUP BY  A.DEPTID,
                                                   B.GDSEQ,to_char(A.SPRQ, 'YYYY-MM')) SB,
                                               (SELECT  A.DEPTID,
                                                   B.GDSEQ,SUM(B.YSSL) YsSL,
                                                       SUM(B.HSJE) YSJE,
                                                       to_char(A.YSRQ, 'YYYY-MM') as Monthly
                                                  FROM DAT_YS_DOC A, DAT_YS_COM B
                                                 WHERE A.SEQNO = B.SEQNO
                                                   AND A.YSRQ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                       TO_DATE('{1}', 'YYYY-MM-DD') 
                                                 GROUP BY A.DEPTID, 
                                                   B.GDSEQ,to_char(A.YSRQ, 'YYYY-MM')) SC,
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
                                                       TO_DATE('{1}', 'YYYY-MM-DD') GROUP BY  to_Char(RQ, 'YYYY-MM') ,
                                                  T.DEPTID, 
                                                 T.GDSEQ) SE
                                         WHERE SA.MONTHLY = SB.MONTHLY(+)
                                           AND SA.MONTHLY = SB.MONTHLY(+)
                                          AND SA.GDSEQ = SB.GDSEQ(+)
                                         AND SA.MONTHLY = SB.MONTHLY(+)
                                         AND SA.DEPTID = SC.DEPTID(+)
                                         AND SA.GDSEQ = SC.GDSEQ(+)
                                         AND SA.MONTHLY = SC.MONTHLY(+)
                                         AND SA.GDSEQ = SD.GDSEQ
                                        AND SA.DEPTID = SE.DEPTID(+)
                                        AND SA.GDSEQ = SE.GDSEQ(+)
                                         AND SA.MONTHLY = SE.MONTHLY(+)
                                           AND SA.GDSEQ = SE.GDSEQ(+))
                                 GROUP BY MONTHLY  ", startDate.AddMonths(-2).ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"), ACCOUNTDAY);
            DataTable dt = DbHelperOra.Query(strSql).Tables[0];

            string sbBuget = string.Empty;
            string sbActual = string.Empty;

            string sysTit = "全院科室月度预算执行情况";
            string sysSubTit = startDate.ToString("yyyy年MM月") + "预算使用情况,  单位：（元）";
            string footLabel = "来源：系统数据库";
            string labelarr = string.Empty;// startDate.AddMonths(-2).ToString("MM") + "月," + startDate.AddMonths(-1).ToString("MM") + "月," + startDate.ToString("MM") + "月";
            DataView view = dt.DefaultView;
            decimal[] iBudget = new decimal[3];
            decimal[] iActival = new decimal[3];
            if (dt != null && dt.Rows.Count > 0)
            {
                int iRow = dt.Rows.Count < 3 ? dt.Rows.Count : 3;
                DataRow[] RowArr = dt.Select("1=1", "Monthly ASC");

                for (int i = 0; i < iRow; i++)
                {
                    labelarr += RowArr[i]["Monthly"].ToString() + "月,";
                    iBudget[i] = Convert.ToDecimal(RowArr[i]["ystotal"] != DBNull.Value ? RowArr[i]["ystotal"] : 0);
                    iActival[i] = Convert.ToDecimal(RowArr[i]["total"] != DBNull.Value ? RowArr[i]["total"] : 0);
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
    }
}