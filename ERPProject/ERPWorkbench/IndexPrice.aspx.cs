using FineUIPro;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using XTBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ERPProject.ERPWorkbench
{
    public partial class IndexPrice : PageBase
    {
        protected string MyMemo = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GridLoadData();
                YsSearch();
                search_sy();
                search_dept();
            }
        }

        private void GridLoadData()
        {
            //结算信息
            String Sql = @"SELECT TO_CHAR(A.RQSJ,'YYYY-MM') JSYF,ABS(SUM(A.HSJE)) JSJE
                    FROM VIEW_JS A
                    WHERE TO_CHAR(A.RQSJ,'YYYY') = TO_CHAR(SYSDATE,'YYYY')
                    GROUP BY TO_CHAR(A.RQSJ,'YYYY-MM')";
            DataTable Dt = DbHelperOra.Query(Sql).Tables[0];
            String str1 = "", str2 = "";
            foreach (DataRow dr in Dt.Rows)
            {
                str1 += dr["JSYF"].ToString() + ",";
                str2 += dr["JSJE"].ToString() + ",";
            }
            hfdjs1.Text = str1.TrimEnd(',');
            hfdjs2.Text = str2.TrimEnd(',');
            PageContext.RegisterStartupScript("getEcharsData3();");
        }
        private void search_sy()
        {
            //损益信息
            String Sql = @"SELECT TO_CHAR(A.RQSJ,'YYYY-MM') SYYF,(SUM(DECODE(A.KCADD,-1,0,A.HSJE))) SYJE1,(SUM(DECODE(A.KCADD,-1,A.HSJE,0))) SYJE2
                FROM DAT_GOODSJXC A
                WHERE A.BILLTYPE = 'SYD' AND TO_CHAR(A.RQSJ,'YYYY') = TO_CHAR(SYSDATE,'YYYY')
                GROUP BY TO_CHAR(A.RQSJ,'YYYY-MM')";
            DataTable Dt = DbHelperOra.Query(Sql).Tables[0];
            String str1 = "", str2 = "", str3 = "";
            foreach (DataRow dr in Dt.Rows)
            {
                str1 += dr["SYYF"].ToString() + ",";
                str2 += dr["SYJE1"].ToString() + ",";
                str3 += dr["SYJE2"].ToString() + ",";
            }
            hfdsy1.Text = str1.TrimEnd(',');
            hfdsy2.Text = str2.TrimEnd(',');
            hfdsy3.Text = str3.TrimEnd(',');
            PageContext.RegisterStartupScript("getEcharsData();");
        }
        private void search_dept()
        {
            //预算信息
            String Sql = @"SELECT SA.*,NVL(SB.YSJE,0) YSJE FROM
                        (SELECT B.CODE,B.NAME,ABS(SUM(NVL(A.HSJE,0))) SYJE
                        FROM VIEW_JS A,SYS_DEPT B
                        WHERE TO_CHAR(A.RQSJ,'YYYY-MM') = TO_CHAR(SYSDATE,'YYYY-MM')
                        AND A.DEPTID(+) = B.CODE AND B.TYPE IN('3','4')
                        GROUP BY B.CODE,B.NAME) SA,
                        (SELECT DEPTID,SUM(SUBSUM) YSJE
                        FROM DAT_YS_DOC
                        WHERE FLAG = 'Y' AND SYSDATE BETWEEN BEGRQ AND ENDRQ + 1
                        GROUP BY DEPTID) SB
                        WHERE SA.CODE = SB.DEPTID(+)";
            DataTable Dt = DbHelperOra.Query(Sql).Tables[0];
            GridDept.DataSource = Dt;
            GridDept.DataBind();
        }
        protected void Timer1_Tick(object sender, EventArgs e)
        {
            GridLoadData();
        }

        protected void Search()
        {
            String wcl = "";
            String sql = @"SELECT round(SUM(DECODE(NVL(SB.DEPTID,'#'),'#',0,1))/COUNT(1),4)*100
                        FROM
                        (SELECT A.CODE
                        FROM SYS_DEPT A
                        WHERE DECODE(to_char(TO_DATE(sysdate,'yyyy-MM-dd'),'day'),'星期一',A.DHZQ1,'星期二',A.DHZQ2,'星期三',A.DHZQ3,'星期四',A.DHZQ4,'星期五',A.DHZQ5,'星期六',A.DHZQ6,A.DHZQ7) <> 'N')
                        SA,
                        (
                        SELECT DISTINCT B.DEPTID
                        FROM DAT_GOODSJXC B
                        WHERE B.RQSJ > TO_DATE(sysdate,'yyyy-MM-dd')
                        ) SB
                        WHERE SA.CODE = SB.DEPTID(+)";
            try
            {
                wcl = DbHelperOra.GetSingle(sql).ToString() + "";
            }
            catch
            {
                wcl = "0.00";
            }
            hfdWcl.Text = wcl;
            PageContext.RegisterStartupScript("getEcharsData();");
        }
        protected void YsSearch()
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
            string strSql = string.Format(@"SELECT TO_CHAR(A.RQSJ, 'YYYY-MM') Monthly, SUM(JSJE + THJE) TOTAL,SUM(NVL(B.YSJE, 0)) YSTOTAL
                                                    FROM VIEW_JS A,
                                                        (SELECT to_char(A.YSRQ, 'YYYY-MM') Monthly, SUM(B.HSJE) YSJE
                                                            FROM DAT_YS_DOC A, DAT_YS_COM B
                                                            WHERE A.SEQNO = B.SEQNO
                                                            AND A.FLAG = 'Y'
                                                            AND A.YSRQ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                                TO_DATE('{1}', 'YYYY-MM-DD')
                                                            GROUP BY to_char(A.YSRQ, 'YYYY-MM')) B
                                                    WHERE  RQSJ BETWEEN TO_DATE('{0}', 'yyyy-MM-dd') AND
                                                        TO_DATE('{1}', 'yyyy-MM-dd') + 1
                                                        AND TO_CHAR(A.RQSJ, 'YYYY-MM') = B.Monthly(+)
                                                    GROUP BY TO_CHAR(A.RQSJ, 'YYYY-MM')
                                                ", startDate.AddMonths(-2).ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
            DataTable dt = DbHelperOra.Query(strSql).Tables[0];
            String ysje, xsje;
            if (dt != null && dt.Rows.Count > 0)
            {
                ysje = dt.Rows[0]["YSTOTAL"].ToString();
                xsje = dt.Rows[0]["TOTAL"].ToString();
            }
            else
            {
                ysje = "0";
                xsje = "0";
            }
            PageContext.RegisterStartupScript("reloaddata(" + ysje + "," + xsje + ");");
        }
        protected void btnClose_Click(object sender, EventArgs e)
        {
            DbHelperOra.ExecuteSql(string.Format("UPDATE SYS_MYMEMO_USER SET FLAG='C',LOOKRQ=SYSDATE WHERE SEQNO='{0}' AND LOOKPER='{1}'", hfdSeqNo.Text, UserAction.UserID));
            WindowMemo.Hidden = true;
        }
    }
}