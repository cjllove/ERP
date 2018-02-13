using FineUIPro;
using XTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;



namespace ERPProject.ERPQuery
{
    public partial class ToDoQuery : PageBase
    {

        public ToDoQuery() {
            ISCHECK = false;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
            }
        }

        private void DataInit()
        {
            lstSCRQ1.SelectedDate = DateTime.Now.AddDays(-7);
            lstSCRQ2.SelectedDate = DateTime.Now;
            PubFunc.DdlDataGet("DDL_DOTYPE", lstDOTYPE);
        }

        private void DataQuery()
        {
            if (lstSCRQ1.SelectedDate == null || lstSCRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【查询日期】！");
                return;
            }
            else if (lstSCRQ1.SelectedDate > lstSCRQ2.SelectedDate)
            {
                Alert.Show("开始日期大于结束日期，请重新输入！");
                return;
            }

            //string sql = "select A.*,B.DONAME from DAT_DO_LIST A,DAT_DO_TYPE B WHERE A.DOTYPE=B.DOTYPE ";
            string sql = @"select A.*,
                                   f_getroleid(a.rolelist) ROLELISTNAME,
                                   B.DONAME,
                                   (case A.Flag
                                     when 'Y' then
                                      '信息已处理，不显示'
                                     when 'N' then
                                      '信息未处理，显示'
                                     else
                                      '维护信息'
                                   end) FLAGNAME,
                                   f_getusername(A.USERID) USERNAME,
                                   f_getusername(A.DOUSER) DOUSERNAME,
                                   C.FUNCNAME
                              from DAT_DO_LIST A, DAT_DO_TYPE B, SYS_FUNCTION C
                             WHERE A.DOTYPE = B.DOTYPE
                               AND A.FUNCID = C.FUNCID(+) ";
            int total = 0;
            if (lstDOTYPE.SelectedItem != null && lstDOTYPE.SelectedItem.Value.Length > 0)
            {
                sql += string.Format(" AND A.DOTYPE='{0}'", lstDOTYPE.SelectedItem.Value);
            }
            if (lstFLAG.SelectedItem != null && lstFLAG.SelectedItem.Value.Length > 0)
            {
                sql += string.Format(" AND A.FLAG='{0}'", lstFLAG.SelectedItem.Value);
            }


            sql += string.Format(" AND a.scrq>=TO_DATE('{0}','yyyy-mm-dd hh24:mi:ss')", lstSCRQ1.SelectedDate);
            sql += string.Format(" AND a.scrq <=TO_DATE('{0}','yyyy-mm-dd hh24:mi:ss')+1 ", lstSCRQ2.SelectedDate);
          //  sql += string.Format("  AND F_CHK_ROLELIST(B.ROLELIST, '{0}') = 'Y'" , UserAction.UserRole);


            sql += " ORDER BY A.SCRQ DESC";

            GridList.DataSource = PubFunc.DbGetPage(GridList.PageIndex, GridList.PageSize, sql, ref total);
            GridList.RecordCount = total;
            GridList.DataBind();
        }

        protected void bntSearch_Click(object sender, EventArgs e)
        {
            DataQuery();
        }

        protected void GridList_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridList.PageIndex = e.NewPageIndex;
            DataQuery();
        }
        protected void Grid1_RowCommand(object sender, FineUIPro.GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                string seqno = GridList.DataKeys[e.RowIndex][0].ToString();
                if (DbHelperOra.ExecuteSql("UPDATE DAT_DO_LIST SET FLAG = 'Y' WHERE FLAG = 'N' AND SEQNO ='" + seqno + "'") > 0)
                {
                    Alert.Show("清理完成");
                    return;
                }
                else
                {
                    Alert.Show("清理失败，请刷新页面或确认是否为显示状态");
                    return;
                }


            }
        }
       
        protected void Grid1_PreRowDataBound(object sender, GridPreRowEventArgs e)
        {
            LinkButtonField lbfDelete = GridList.FindColumn("Delete") as LinkButtonField;
            DataRowView row = e.DataItem as DataRowView;

            if (row["FLAG"].ToString() == "Y")
            {
                lbfDelete.Enabled = false;
            }
            else
            {
                lbfDelete.Enabled = true;
            }
        }
    }
}