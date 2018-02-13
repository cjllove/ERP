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
    public partial class DeptWorkShow : BillBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 在页面第一次加载时 
                getEcharsData();
                getEcharsData3();
                calendar();
            }
        }
        #region 列表
        protected void getEcharsData3()
        {
            String sql = @"SELECT B.GDNAME,B.GDSPEC,A.DSNUM,A.NUM1,A.ZDKC,A.ZGKC,
                 NVL((SELECT SUM(KCSL) FROM DAT_GOODSSTOCK K WHERE K.GDSEQ = A.GDSEQ AND K.DEPTID = A.DEPTID),0) KCSL
                FROM DOC_GOODSCFG A,DOC_GOODS B
                WHERE A.GDSEQ = B.GDSEQ AND A.DEPTID = '{0}'";
            GridPs.DataSource = DbHelperOra.Query(String.Format(sql, UserAction.UserDept)).Tables[0];
            GridPs.DataBind();
        }
        protected void gridPs_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                Decimal kcsl = Convert.ToDecimal(row["KCSL"].ToString());
                Decimal zgkc = Convert.ToDecimal(row["ZGKC"].ToString());
                Decimal zdkc = Convert.ToDecimal(row["ZDKC"].ToString());
                if (zgkc > 0 && zgkc < kcsl)
                {
                    e.RowCssClass = "color2";
                }
                else if (zdkc > 0 && zdkc > kcsl)
                {
                    e.RowCssClass = "color2";
                }
            }
        }
        #endregion
        #region 三图
        private void getEcharsData()
        {
            List<CommandInfo> cmdList = new List<CommandInfo>();
            OracleParameter[] parameters = {
                                               new OracleParameter("V_TYPE", OracleDbType.Varchar2,50),
                                               new OracleParameter("RES", OracleDbType.Varchar2,2000)
                                                };
            parameters[0].Value = "RES_JH";
            parameters[1].Direction = ParameterDirection.Output;

            cmdList.Add(new CommandInfo("P_GETBAR_SL", parameters, CommandType.StoredProcedure));
            DbHelperOra.ExecuteSqlTran(cmdList);
            hfdYSXH.Text = parameters[1].Value.ToString();
            List<CommandInfo> cmdList2 = new List<CommandInfo>();
            OracleParameter[] parameters2 = {
                                               new OracleParameter("V_TYPE", OracleDbType.Varchar2,50),
                                               new OracleParameter("RES", OracleDbType.Varchar2,2000)
                                                };
            parameters2[0].Value = "RES_SJ";
            parameters2[1].Direction = ParameterDirection.Output;

            cmdList2.Add(new CommandInfo("P_GETBAR_SL", parameters2, CommandType.StoredProcedure));
            DbHelperOra.ExecuteSqlTran(cmdList2);
            hfdSJXH.Text = parameters2[1].Value.ToString();


            PageContext.RegisterStartupScript("getEcharsData();");
        }
        #endregion
        #region 日历
        private void calendar()
        {
            PageContext.RegisterStartupScript("calendar();");
        }
        #endregion
    }
}