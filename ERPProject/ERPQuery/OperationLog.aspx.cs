using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FineUIPro;
using System.Data;
using XTBase;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ERPProject.ERPQuery
{
    public partial class OperationLog : PageBase
    {
        private static DataTable dtData;
        private String strSql = @"select F_GETUSERNAME(USERID) USERID,
                           FUNCID,
                           MEMO,
                           STATION,
                           to_char(RQSJ, 'yyyy-mm-dd hh24:mi:ss') RQSJ
              from (SELECT *
                      FROM SYS_OPERLOG
                     WHERE TYPE = 'LOG'
                       --and RQSJ >= trunc(sysdate) - 1
                     order by RQSJ desc)
             where type='LOG'";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString.Count > 0 && !string.IsNullOrWhiteSpace(Request.QueryString.Get("data")))
                {
                    string result = GetResultData();
                    Response.ContentType = "text/plain";
                    Response.Write(result);
                    Response.End();
                }
                else
                {
                    BindDDL();
                }
            }
        }

        //为工作台提供数据
        private string GetResultData()
        {

            DataTable table = getData();
            string result = JsonConvert.SerializeObject(table);
            return result;
        }

        private void BindDDL()
        {
            lstLRRQ1.SelectedDate = DateTime.Now;
            lstLRRQ2.SelectedDate = DateTime.Now;

            lstFuncid.DataSource = DbHelperOra.Query(@"SELECT '' CODE ,'--请选择--' NAME  FROM dual
                                                        union all
                                                        select distinct t.funcid NAME,t.funcid CODE from sys_operlog t");
            lstFuncid.DataTextField = "NAME";
            lstFuncid.DataValueField = "CODE";
            lstFuncid.DataBind();


        }
        protected void GridLog_Sort(object sender, FineUIPro.GridSortEventArgs e)
        {
            DataView view1 = dtData.DefaultView;
            view1.Sort = String.Format("{0} {1}", e.SortField, e.SortDirection);

            GridLog.DataSource = view1;
            GridLog.DataBind();
        }

        private DataTable getData()
        {
            DataTable dt = new DataTable();
            //DateTime dtNow = DateTime.Now;
            //DateTime dtYes = dtNow.AddDays(-1);
            // string strOrder = "ORDER BY SEQNO DESC";
            //string strSearch = "";

            //strSearch += string.Format(" AND  RQSJ>=to_date('{0}','yyyy-mm-dd') and RQSJ< to_date('{1}','yyyy-mm-dd') + 1", dtYes.ToString("yyyy-MM-dd"), dtNow.ToString("yyyy-MM-dd"));


            //strSql += strSearch;
            //strSql += strOrder;
            string sql = "SELECT * FROM (" + strSql + ") WHERE ROWNUM<21";
            dt = DbHelperOra.Query(sql).Tables[0];
            return dt;
        }
        private void dataSearch()
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【录入日期】！");
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                Alert.Show("开始日期大于结束日期，请重新输入！");
                return;
            }
            string strOrder = " ORDER BY RQSJ DESC";

            string strSearch = "";
            if (!string.IsNullOrWhiteSpace(trbSearch.Text))
            {
                strSearch += string.Format(" AND DBMS_LOB.INSTR(MEMO, '{0}', 1, 1) > 0", trbSearch.Text);
            }
            if (!string.IsNullOrWhiteSpace(lstFuncid.SelectedValue))
            {
                strSearch += string.Format(" AND FUNCID='{0}' ", lstFuncid.SelectedValue);
            }
            if (lstLRRQ1.SelectedDate != null || lstLRRQ2.SelectedDate != null)
            {
                strSearch += string.Format(" AND  RQSJ>=to_date('{0}','yyyy-mm-dd') and RQSJ< to_date('{1}','yyyy-mm-dd') + 1", lstLRRQ1.Text, lstLRRQ2.Text);
            }

            strSql += strSearch;
            strSql += strOrder;
            PageContext.RegisterStartupScript(GridLog.GetRejectChangesReference());
            int total = 0;
            DataTable dt = PubFunc.DbGetPage(GridLog.PageIndex, GridLog.PageSize, strSql, ref total);
            GridLog.DataSource = dt;
            GridLog.RecordCount = total;
            //GridLog.DataSource = DbHelperOra.Query(strSql);
            GridLog.DataBind();
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            dataSearch();
        }
        protected void Window1_Close(object sender, EventArgs e)
        {
            dataSearch();
        }
        protected void GridLog_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridLog.PageIndex = e.NewPageIndex;
            dataSearch();
        }
    }
}