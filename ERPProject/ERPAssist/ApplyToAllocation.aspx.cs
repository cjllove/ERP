using FineUIPro;
using XTBase;
using XTBase.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPProject.ERPAssist
{
    public partial class ApplyToAllocation : PageBase
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
            DepartmentBind.BindDDL("DDL_SYS_DEPTRANGE", UserAction.UserID, lstDEPTID);
            PubFunc.DdlDataGet("DDL_BILL_STATUSSLD", lstFLAG);
            lstFLAG.SelectedValue = "S";
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
        }

        private string GetQuerySql()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendFormat(@"SELECT F_GETDEPTNAME(A.DEPTOUT) DEPTOUT,
                                                       F_GETDEPTNAME(A.DEPTID) DEPTID,
                                                       B.GDSEQ,
                                                       B.GDNAME,
                                                       B.GDSPEC,
                                                       F_GETUNITNAME(B.UNIT) UNIT,
                                                       F_GETPRODUCERNAME(B.PRODUCER) PRODUCER,
                                                       SUM(B.DHSL) SLS
                                                  FROM DAT_SL_DOC A, DAT_SL_COM B
                                                 WHERE A.SEQNO = B.SEQNO
                                                   AND A.FLAG = '{0}'", lstFLAG.SelectedValue);
            if (!string.IsNullOrWhiteSpace(lstDEPTID.SelectedValue))
            {
                sb.AppendFormat(" AND A.DEPTID='{0}'", lstDEPTID.SelectedValue);
            }
            sb.AppendFormat(" AND A.SHRQ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND TO_DATE('{1}', 'YYYY-MM-DD')+1",
                                         lstLRRQ1.Text, lstLRRQ2.Text);
            sb.Append(@"  GROUP BY A.DEPTOUT,
                                                  A.DEPTID,
                                                  B.GDSEQ,
                                                  B.GDNAME,
                                                  B.GDSPEC,
                                                  B.UNIT,
                                                  B.PRODUCER");
            return sb.ToString();
        }

        protected void bntSearch_Click(object sender, EventArgs e)
        {
            GridList.DataSource = DbHelperOra.Query(GetQuerySql()).Tables[0];
            GridList.DataBind();
        }

        protected void btnAudit_Click(object sender, EventArgs e)
        {

        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            DataTable dtData = DbHelperOra.Query(GetQuerySql()).Tables[0];
            if (dtData == null || dtData.Rows.Count == 0)
            {
                Alert.Show("没有数据,无法导出！");
                return;
            }
            string[] columnNames = new string[GridList.Columns.Count - 1];
            for (int index = 1; index < GridList.Columns.Count; index++)
            {
                GridColumn column = GridList.Columns[index];
                if (column is FineUIPro.BoundField)
                {
                    dtData.Columns[((FineUIPro.BoundField)(column)).DataField.ToUpper()].ColumnName = column.HeaderText;
                    columnNames[index - 1] = column.HeaderText;
                }
            }

            ExcelHelper.ExportByWeb(dtData.DefaultView.ToTable(true, columnNames), "科室商品请领信息汇总", string.Format("商品出库信息汇总_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }
    }
}