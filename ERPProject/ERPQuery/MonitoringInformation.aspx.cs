using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using FineUIPro;
using XTBase;

namespace ERPProject.ERPQuery
{
    public partial class MonitoringInformation : PageBase
    {
        private string strSql_OLD = @"select *
                                    from (select SEQNO,rqsj, memo,
                                                decode(TYPE,'LOG', '历史记录', 'ERR', '错误类型', '历史记录1') typename,type,STR3 FLAG, 'LOG' TAB
                                            from SYS_OPERLOG 
                                        union ALL
                                        select SEQNO, execrq, nvl(memo, '接口执行成功！'),
                                                decode(errortype,'ERR', '错误记录','WAR','警告信息','接口信息') typename,errortype type,FLAG, 'INF' TAB
                                            from DAT_INFDATA_LOG  
                                        )
                                    where 1 = 1";
        private string strSql = @"select SEQNO, execrq rqsj, nvl(memo, '接口执行成功！') memo,
                                         decode(errortype,'ERR', '错误记录','WAR','警告信息','接口信息') typename,
                                         NVL(errortype,'LOG') type,decode(FLAG,'N','未处理','Y','已处理') FLAG, 'INF' TAB
                                    from DAT_INFDATA_LOG  
                                   where NVL(errortype,'LOG') IN ('ERR','WRI') ";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
//                PubFunc.DdlDataSql(ddlTYPE, @"SELECT '' CODE ,'--请选择--' NAME  FROM dual
//                                        union all
//                                        SELECT 'ERR' CODE ,'错误记录' NAME  FROM dual
//                                        union all
//                                        SELECT 'WRI' CODE ,'警告记录' NAME  FROM dual
//                                        union all
//                                        SELECT 'LOG' CODE ,'历史记录' NAME  FROM dual
//                                        ");
                dpkKSRQ.SelectedDate = DateTime.Now.AddDays(-1);
                dpkJSRQ.SelectedDate = DateTime.Now;
            }
        }

        private void DataSearch()
        {
            if (PubFunc.StrIsEmpty(dpkKSRQ.SelectedDate.ToString()) || PubFunc.StrIsEmpty(dpkJSRQ.SelectedDate.ToString()))
            {
                Alert.Show("输入日期不正确,请检查！");
                return;
            }
            if (dpkKSRQ.SelectedDate > dpkJSRQ.SelectedDate)
            {
                Alert.Show("开始日期不能大于结束日期！");
                return;
            }
            string strMonitor = " ";
            strMonitor += " AND execrq BETWEEN TO_DATE('" + dpkKSRQ.Text + "','YYYY/MM/DD') and TO_DATE('" + dpkJSRQ.Text + "','YYYY/MM/DD') + 1";

            if (chkCHULI.Checked )
            {
                strMonitor += " AND FLAG  = 'N' ";
            }
            strSql = strSql + strMonitor;

            int total = 0;

            DataTable dtData = PubFunc.DbGetPage(GridList.PageIndex, GridList.PageSize, strSql, ref total);
            GridList.RecordCount = total;
            GridList.DataSource = dtData;
            GridList.DataBind();
        }

        protected void GridList_RowCommand(object sender, FineUIPro.GridCommandEventArgs e)
        {
            if (e.CommandName == "EXEC")
            {
                string menuId = GridList.DataKeys[e.RowIndex][0].ToString();
                DbHelperOra.ExecuteSql("update DAT_INFDATA_LOG set flag='Y' where seqno='" + menuId + "' ");
                DataSearch();
            }
        }

        protected void btSearch_Click(object sender, EventArgs e)
        {
            DataSearch();
        }

        protected void GridList_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridList.PageIndex = e.NewPageIndex;
            DataSearch();
        }
    }
}