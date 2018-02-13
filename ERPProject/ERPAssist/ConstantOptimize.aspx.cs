using FineUIPro;
using XTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPProject.ERPAssist
{
    public partial class ConstantOptimize : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
            }
        }

        private void DataInit()
        {
            DepartmentBind.BindDDL("DDL_SYS_DEPARTMENTRANGE", UserAction.UserID, lstDEPTID);
            dpkBEGRQ.SelectedDate = DateTime.Now.AddMonths(-1);
            dpkENDRQ.SelectedDate = DateTime.Now;
        }

        private void DataQuery()
        {
            if (dpkBEGRQ.SelectedDate == null || dpkBEGRQ.SelectedDate == null)
            {
                Alert.Show("请输入条件【查询期间】！", "警告提醒", MessageBoxIcon.Warning);
                return;
            }
            else if (dpkENDRQ.SelectedDate > dpkENDRQ.SelectedDate)
            {
                Alert.Show("开始日期大于结束日期，请重新输入！", "警告提醒", MessageBoxIcon.Warning);
                return;
            }

            int total = 0;
            string sql = @"SELECT GC.DEPTNAME,G.GDSEQ,G.GDNAME,G.GDSPEC,GC.NUM1,GC.DSNUM,DSH.AVE DSNUM1,DSH.BACKTIMES,
                                             F_GETUNITNAME(G.UNIT) UNITNAME,G.PIZNO,F_GETPRODUCERNAME(G.PRODUCER) PRODUCER
                                   FROM (SELECT DG.*, D.NAME DEPTNAME FROM DOC_GOODSCFG DG, SYS_DEPT D
                                             WHERE DG.DEPTID = D.CODE AND D.TYPE = '3') GC,
                                            DOC_GOODS G,
                                            (SELECT DEPTID, GDSEQ, CEIL(SUM(HSGS) / COUNT(1)) AVE,COUNT(1) BACKTIMES
                                              FROM (SELECT A.DEPTID, A.SEQNO, B.GDSEQ, COUNT(1) HSGS
                                                          FROM DAT_XS_DOC A, DAT_XS_COM B
                                                        WHERE A.SEQNO = B.SEQNO AND A.BILLTYPE = 'DSH'
                                                             AND XSRQ >= TO_DATE('{0}', 'YYYY-MM-DD')
                                                             AND XSRQ < TO_DATE('{1}', 'YYYY-MM-DD')
                                                     GROUP BY A.DEPTID, A.SEQNO, B.GDSEQ)
                                             GROUP BY DEPTID, GDSEQ) DSH
                                 WHERE GC.GDSEQ = G.GDSEQ AND GC.GDSEQ = DSH.GDSEQ(+) AND GC.DEPTID = DSH.DEPTID(+) ";
            sql = string.Format(sql, dpkBEGRQ.Text, dpkENDRQ.Text);

            if (!string.IsNullOrWhiteSpace(tbxGOODS.Text.Trim()))
            {
                sql += string.Format(" AND (G.GDSEQ LIKE '%{0}%' OR G.GDNAME LIKE '%{0}%' OR G.ZJM LIKE '%{0}%')", tbxGOODS.Text.ToUpper());
            }
            if (!string.IsNullOrWhiteSpace(hfdDEPTID.Text))
            {
                sql += string.Format(" AND GC.DEPTID='{0}'", hfdDEPTID.Text.ToUpper());
            }
            else if (lstDEPTID.SelectedItem.Value.Length > 0)
            {
                sql += string.Format(" AND GC.deptid = '{0}'", lstDEPTID.SelectedItem.Value);
            }
            
            DataTable dt = GetDataTable(GridGoods.PageIndex, GridGoods.PageSize, sql, ref total);
            GridGoods.DataSource = dt;
            GridGoods.RecordCount = total;
            GridGoods.DataBind();
        }

        protected void bntSearch_Click(object sender, EventArgs e)
        {
            DataQuery();
        }

        protected void GridGoods_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridGoods.PageIndex = e.NewPageIndex;
            DataQuery();
        }
    }
}