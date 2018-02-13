using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FineUIPro;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using XTBase;
using System.Data;
using System.Collections.Specialized;
using System.Text;

namespace ERPProject.ERPQuery
{
    public partial class SystemInventoryComparison : BillBase
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
            lstSJ.SelectedDate = DateTime.Now;
        }
        protected override void billClear()
        {
            lstSJ.SelectedDate = DateTime.Now;
            tbxGDSEQ.Text = "";
            GridList.Rows.Clear();
        }
        private string GetQuerySql()
        {
            string strSql = @"SELECT A.GDSEQ,
                                                   TA.BAR3,
                                                   TA.GDNAME,
                                                   F_GETUNITNAME(TA.UNIT) UNITNAME,
                                                   A.KCSL KFKC,
                                                   B.KSKCSL KEKCSL,
                                                   C.XSSL WJSSL,
                                                   F.SYSL SYSL,
                                                   E.EASSL EASSL,
                                                   NVL(A.KCSL, 0) + NVL(B.KSKCSL, 0) + NVL(C.XSSL, 0) - NVL(E.EASSL, 0) CY,
                                                   decode(TA.ISGZ,'Y','是','否') ISGZ
                                              FROM (SELECT GDSEQ, SUM(KCSL) KCSL
                                                      FROM DAT_GOODSSTOCK
                                                     WHERE DEPTID IN (SELECT CODE FROM SYS_DEPT WHERE TYPE = '1')
                                                     GROUP BY GDSEQ) A,
                                                   (SELECT GDSEQ, SUM(KCSL) KSKCSL
                                                      FROM DAT_GOODSSTOCK
                                                     WHERE DEPTID IN (SELECT CODE FROM SYS_DEPT WHERE TYPE = '3')
                                                     GROUP BY GDSEQ) B,
                                                   (SELECT GDSEQ, SUM(XSSL) XSSL
                                                      FROM DAT_XS_DOC A, DAT_XS_COM B
                                                     WHERE A.SEQNO = B.SEQNO
                                                       AND FLAG = 'Y'
                                                       AND A.ISSEND = 'N'
                                                     GROUP BY B.GDSEQ) C,
                                                   (SELECT GDSEQ, SUM(SL) EASSL
                                                      FROM EAS_STOCK
                                                     WHERE TO_CHAR(RQ, 'YYYY-MM-DD') = '" + lstSJ.Text + @"'
                                                     GROUP BY GDSEQ) E,
                                                   (SELECT GDSEQ, SUM(SYSL)SYSL
                                                      FROM DAT_SY_DOC A, DAT_SY_COM B
                                                     WHERE A.SEQNO = B.SEQNO
                                                       AND FLAG = 'Y'
                                                     GROUP BY B.GDSEQ) F,
                                                   DOC_GOODS TA
                                             WHERE A.GDSEQ = B.GDSEQ(+)
                                               AND A.GDSEQ = C.GDSEQ(+)
                                               AND A.GDSEQ = TA.GDSEQ
                                               AND TA.GDSEQ = E.GDSEQ(+)
                                               AND A.GDSEQ = F.GDSEQ(+) ";
            string strSearch = "";
            if (!PubFunc.StrIsEmpty(ddlISGZ.SelectedValue)) strSearch += " and TA.ISGZ = '" + ddlISGZ.SelectedValue + "'";
            if (ckbOnlyDifference.Checked)
            {
                strSearch += " AND (NVL(A.KCSL, 0) + NVL(B.KSKCSL, 0) + NVL(C.XSSL, 0) + NVL(-F.SYSL, 0) <> NVL(E.EASSL, 0)) ";
            }
            if (tbxGDSEQ.Text.Trim().Length > 0)
            {
                strSearch += " AND (TA.GDSEQ LIKE '%" + tbxGDSEQ.Text.Trim() + "%' or TA.BAR3 LIKE '%" + tbxGDSEQ.Text.Trim() + "%' or TA.GDNAME LIKE '%" + tbxGDSEQ.Text.Trim() + "%' or TA.ZJM LIKE '%" + tbxGDSEQ.Text.Trim() + "%')";
            }

            return strSql + strSearch;
        }
        protected override void billSearch()
        {
            int total = 0;
            DataTable dt = PubFunc.DbGetPage(GridList.PageIndex, GridList.PageSize, GetQuerySql(), ref total);
            GridList.DataSource = dt;
            GridList.RecordCount = total;
            GridList.DataBind();
        }
        protected void GridList_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridList.PageIndex = e.NewPageIndex;
            billSearch();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            if (GridList.Rows.Count < 1)
            {
                Alert.Show("没有数据,无法导出！","警告提醒",MessageBoxIcon.Warning);
            }
            else
            {
                DataTable dt = DbHelperOra.Query(GetQuerySql()).Tables[0];

                foreach (FineUIPro.GridColumn column in GridList.Columns)
                {
                    if (column is FineUIPro.BoundField)
                    {
                        dt.Columns[((FineUIPro.BoundField)(column)).DataField.ToUpper()].ColumnName = column.HeaderText;
                    }
                }

                XTBase.Utilities.ExcelHelper.ExportByWeb(dt, "系统库存对比导出", "系统库存对比导出_" + DateTime.Now.ToString("yyyyMMddHH") + ".xls");
            }
        }
    }
}