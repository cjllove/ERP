using XTBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections.Specialized;
using FineUIPro;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;

namespace ERPProject.ERPQuery
{
    public partial class WorkShow : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 在页面第一次加载时 
                getEcharsData_PSLY();//配送来源图
                getEcharsData_PSSL();//周配送科室量
                getEcharsData_WCQK();//配送完成情况
                getEcharsData_PSQK();//配送员配送情况
            }
        }
        #region 仪表盘
       
        protected void getEcharsData_WCQK()
        {
            String wcl = "";
            String sql = @"WITH 
                            SQLWC AS 
                            (SELECT SUM(DCC.DHSL) WC FROM DAT_CK_DOC DCD, DAT_CK_COM DCC,SYS_DEPT SD
                            WHERE DCD.BILLNO=DCC.SEQNO(+) AND DCD.DEPTID=SD.CODE(+) AND SD.TYPE<>'1' AND SD.FLAG='Y' AND TO_CHAR(DCD.LRRQ,'YYYY-MM-DD')=TO_CHAR(SYSDATE,'YYYY-MM-DD')
                            AND DCD.FLAG NOT IN('Y','G') AND DECODE(to_char(sysdate,'day'),'星期一',SD.DHZQ1,'星期二',SD.DHZQ2,'星期三',SD.DHZQ3,'星期四',SD.DHZQ4,'星期五',SD.DHZQ5,'星期六',SD.DHZQ6,SD.DHZQ7) <> 'N')
                            ,
                            SQLYC AS
                            (SELECT  SUM(DGJ.SL) YC
                            FROM DAT_GOODSJXC DGJ,SYS_DEPT SD
                            WHERE DGJ.DEPTID=SD.CODE(+) AND SD.FLAG='Y' AND SD.TYPE<>'1' AND TO_CHAR(DGJ.RQSJ,'YYYY-MM-DD')=TO_CHAR(SYSDATE,'YYYY-MM-DD')
                            AND DGJ.BILLTYPE IN('LCD','CKD','LTD') AND  
                             DECODE(to_char(sysdate,'day'),'星期一',SD.DHZQ1,'星期二',SD.DHZQ2,'星期三',SD.DHZQ3,'星期四',SD.DHZQ4,'星期五',SD.DHZQ5,'星期六',SD.DHZQ6,SD.DHZQ7) <> 'N')
                             SELECT ROUND(YC/WC*100,2) FROM SQLWC,SQLYC
";
            try
            {
                wcl = DbHelperOra.GetSingle(sql).ToString() + "";
            }
            catch
            {
                wcl = "0.00";
            }
            hfd3.Text = wcl;
            PageContext.RegisterStartupScript("getEcharsData_WCQK();");
        }
        #endregion
        public static DataTable DistinctDB(DataTable dt, string[] filedNames)
        {
            DataView dv = dt.DefaultView;
            DataTable DistTable = dv.ToTable( true, filedNames);
            return DistTable;
        }
        #region 列表
        protected void getEcharsData_PSQK()
        {
            string datauser="";
            string datadept = "";
            string dmap = "";
            string strsql = @" SELECT SUM(DCC.HSJE)JE,SD.NAME DEPTNAME
                              FROM DAT_CK_DOC D, DAT_CK_COM DCC, SYS_DEPT SD
                             WHERE D.BILLNO = DCC.SEQNO
                               AND D.DEPTID = SD.CODE(+)
                               AND D.BILLTYPE NOT IN ('Y', 'G')
                               AND SD.FLAG = 'Y' AND SD.STR4='{0}'
                               AND SD.TYPE <> '1' 
                               AND DECODE(TO_CHAR(SYSDATE-1, 'day'),
                                          '星期一',
                                          SD.DHZQ1,
                                          '星期二',
                                          SD.DHZQ2,
                                          '星期三',
                                          SD.DHZQ3,
                                          '星期四',
                                          SD.DHZQ4,
                                          '星期五',
                                          SD.DHZQ5,
                                          '星期六',
                                          SD.DHZQ6,
                                          SD.DHZQ7) <> 'N'
                                          GROUP BY SD.NAME ";
            DataTable dtuser = DbHelperOra.QueryForTable("SELECT DISTINCT(SD.STR4)USERID,F_GETUSERNAME(SD.STR4)USERNAME,SD.NAME DEPTNAME FROM SYS_DEPT SD,SYS_OPERUSER OU WHERE SD.STR4=OU.USERID AND OU.ISDELETE<>'Y' AND SD.FLAG='Y'");
            DataTable dt = new DataTable();
            DataTable dtuserdist = new DataTable();
            //int i=1;
            dtuserdist = DistinctDB(dtuser, new string[] { "USERID" });
            foreach (DataRow druser in dtuserdist.Rows)
            {
                
                dt = DbHelperOra.QueryForTable(string.Format(strsql,druser[0].ToString()));
                if (dt.Rows.Count > 0)
                {
                    //dmap += i + " :[";
                    dmap += " [";
                    foreach (DataRow drmap in dt.Rows)
                    {
                        dmap += drmap[0].ToString() + ",";
                    }
                    dmap =dmap.Substring(0,dmap.Count()-1)+ "]$";

                    //i++;
                }
            }


            dtuserdist = DistinctDB(dtuser, new string[] { "USERNAME" });
            foreach (DataRow drdept in dtuserdist.Rows)
            {
                datauser += drdept[0].ToString() + ",";
            }
            dtuserdist = DistinctDB(dtuser, new string[] { "DEPTNAME" });
            foreach (DataRow drdept in dtuserdist.Rows)
            {
                datadept += drdept[0].ToString() + ",";

            }
            if (dmap.Length > 0)
            {
                dmap = dmap.Substring(0, dmap.Count() - 1);

            }
            datauser = datauser.Substring(0, datauser.Count()-1);
            datadept = datadept.Substring(0, datadept.Count() - 1);
                hfd4.Text=dmap;

             PageContext.RegisterStartupScript(string.Format("getEcharsData_PSQK('{0}','{1}','{2}');",dmap,datauser,datadept));

            String sql3 = @"SELECT A.*,DECODE(A.DHZQ,'A','上午','下午') PSTIME,'配送延迟' TYPE,'../res/images/work/ps.gif' IMAGEURL
                    FROM V_PS A";
            string sql = @"WITH SQLWC AS
                             (SELECT SUM(DCC.DHSL) WC,
                                     DCD.DEPTID,SD.NAME DEPTNAME,
                                     DECODE(TO_CHAR(SYSDATE, 'day'),
                                            '星期一',
                                            SD.DHZQ1,
                                            '星期二',
                                            SD.DHZQ2,
                                            '星期三',
                                            SD.DHZQ3,
                                            '星期四',
                                            SD.DHZQ4,
                                            '星期五',
                                            SD.DHZQ5,
                                            '星期六',
                                            SD.DHZQ6,
                                            SD.DHZQ7) PSTIME
                                FROM DAT_CK_DOC DCD, DAT_CK_COM DCC, SYS_DEPT SD
                               WHERE DCD.BILLNO = DCC.SEQNO(+)
                                 AND DCD.DEPTID = SD.CODE(+)
                                 AND SD.TYPE <> '1'
                                 AND SD.FLAG = 'Y'
                                   -- AND TO_CHAR(DCD.LRRQ, 'YYYY-MM-DD') = TO_CHAR(SYSDATE, 'YYYY-MM-DD')
                                 AND DCD.FLAG NOT IN ('Y', 'G')
                                 AND DECODE(TO_CHAR(SYSDATE, 'day'),
                                            '星期一',
                                            SD.DHZQ1,
                                            '星期二',
                                            SD.DHZQ2,
                                            '星期三',
                                            SD.DHZQ3,
                                            '星期四',
                                            SD.DHZQ4,
                                            '星期五',
                                            SD.DHZQ5,
                                            '星期六',
                                            SD.DHZQ6,
                                            SD.DHZQ7) <> 'N'
                               GROUP BY DCD.DEPTID,SD.NAME,
                                        SD.DHZQ1,
                                        SD.DHZQ2,
                                        SD.DHZQ3,
                                        SD.DHZQ4,
                                        SD.DHZQ5,
                                        SD.DHZQ6,
                                        SD.DHZQ7),
                            SQLYC AS
                             (SELECT SUM(DGJ.SL) YC, SD.CODE DEPTID,SD.NAME DEPTNAME
                                FROM DAT_GOODSJXC DGJ, SYS_DEPT SD
                               WHERE DGJ.DEPTID = SD.CODE(+)
                                 AND SD.FLAG = 'Y'
                                 AND SD.TYPE <> '1'
                                 AND TO_CHAR(DGJ.RQSJ, 'YYYY-MM-DD') = TO_CHAR(SYSDATE, 'YYYY-MM-DD')
                                 AND DGJ.BILLTYPE IN ('LCD', 'CKD', 'LTD')
                                 AND DECODE(TO_CHAR(SYSDATE, 'day'),
                                            '星期一',
                                            SD.DHZQ1,
                                            '星期二',
                                            SD.DHZQ2,
                                            '星期三',
                                            SD.DHZQ3,
                                            '星期四',
                                            SD.DHZQ4,
                                            '星期五',
                                            SD.DHZQ5,
                                            '星期六',
                                            SD.DHZQ6,
                                            SD.DHZQ7) <> 'N'
                               GROUP BY SD.CODE,SD.NAME)
                            SELECT WC.WC,
                                   WC.DEPTID,WC.DEPTNAME,
                                   DECODE(WC.PSTIME, 'A', '上午', '下午') PSTIME,
                                   YC.YC,
                                   ROUND(YC / WC * 100, 2),
                                   CASE
                                     WHEN YC IS NULL THEN
                                      '未配送'
                                     WHEN YC < WC THEN
                                      '配送中'
                                     WHEN YC = WC THEN
                                      '配送完成'
                                     WHEN YC > WC THEN
                                      '异常'
                                   END PRESULT
                              FROM SQLWC WC, SQLYC YC
                             WHERE WC.DEPTID = YC.DEPTID(+)";
            gridPs.DataSource = DbHelperOra.Query(sql).Tables[0];
            gridPs.DataBind();
            PageContext.RegisterStartupScript("updatedata();");
        }
      
        #endregion
        #region 三图
        private void getEcharsData_PSLY()
        {
            String sql = @"SELECT SUM(DGJ.SL)SL,f_getproducername(DGJ.PSSID)PSSNAME
                          FROM DAT_GOODSJXC DGJ
                         WHERE DGJ.BILLTYPE IN ('RKD','THD')
                          AND DGJ.RQSJ between trunc(sysdate, 'mm') and last_day(sysdate) + 1
                           GROUP BY PSSID";
            DataTable dt = DbHelperOra.Query(sql).Tables[0];
            Decimal SL = 0, total = 0;
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                SL += Convert.ToDecimal(dr["SL"]);
                if (i > 9)
                {
                    total += Convert.ToDecimal(dr["SL"].ToString());
                }
                else
                {
                    hfd1.Text += dr["SL"] + "$" + dr["PSSNAME"] + ",";
                }
                i++;

            }
            if (total > 0)
            {
                hfd1.Text += total.ToString() + "$其他,";
            }

            hfd1.Text = hfd1.Text.TrimEnd(',');
            PageContext.RegisterStartupScript("getEcharsData_PSLY();");
        }
        #endregion
        private void getEcharsData_PSSL()
        {
            List<CommandInfo> cmdList = new List<CommandInfo>();
            OracleParameter[] parameters = {
                                               new OracleParameter("V_TYPE", OracleDbType.Varchar2,50),
                                               new OracleParameter("RES", OracleDbType.Varchar2,2000)
                                                };
            parameters[0].Value = "RES_JH";
            parameters[1].Direction = ParameterDirection.Output;

            cmdList.Add(new CommandInfo("P_GETPIE_PSSL", parameters, CommandType.StoredProcedure));
            DbHelperOra.ExecuteSqlTran(cmdList);
            hfd21.Text = parameters[1].Value.ToString() ;
            List<CommandInfo> cmdList2 = new List<CommandInfo>();
            OracleParameter[] parameters2 = {
                                               new OracleParameter("V_TYPE", OracleDbType.Varchar2,50),
                                               new OracleParameter("RES", OracleDbType.Varchar2,2000)
                                                };
            parameters2[0].Value = "RES_SJ";
            parameters2[1].Direction = ParameterDirection.Output;

            cmdList2.Add(new CommandInfo("P_GETPIE_PSSL", parameters2, CommandType.StoredProcedure));
            DbHelperOra.ExecuteSqlTran(cmdList2);
            hfd22.Text = parameters2[1].Value.ToString();
            PageContext.RegisterStartupScript("getEcharsData_PSSL();");

        }
    }
}