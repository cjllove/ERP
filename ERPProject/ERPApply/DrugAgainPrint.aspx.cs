using XTBase;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FineUIPro;

namespace ERPProject.ERPApply
{
    public partial class DrugAgainPrint : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //PubFunc.DdlDataGet(ddlDEPTINT, "DDL_SYS_DEPTNULL");
                DepartmentBind.BindDDL("DDL_SYS_DEPTRANGE", UserAction.UserID, ddlDEPTINT);
                hfdCurrent.Text = UserAction.UserID;
                dpkout1.SelectedDate = DateTime.Now.AddMonths(-1);
                dpkout2.SelectedDate = DateTime.Now;
            }
        }
        private void DataSearch()
        {
            highlightRows.Text = "";
            int total = 0;
            DataTable dtData = GetDataTable(GridGoods.PageIndex, GridGoods.PageSize, GetSearchSql(), ref total);
            GridGoods.RecordCount = total;
            GridGoods.DataSource = dtData;
            GridGoods.DataBind();
        }

        protected void btSearch_Click(object sender, EventArgs e)
        {
            DataSearch();
        }

        protected void GridGoods_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridGoods.PageIndex = e.NewPageIndex;
            DataSearch();
        }

        protected void GridGoods_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                string flag = row["FLAG"].ToString();

                if (flag == "N")
                {
                    highlightRows.Text += e.RowIndex.ToString() + ",";
                }
            }
        }

        private string GetSearchSql()
        {
            string strSearch = "";
            if (trbSearch.Text.Trim().Length > 0)
            {
                strSearch += string.Format(" AND (A.GDSEQ LIKE '%{0}%' OR B.GDNAME LIKE '%{0}%' OR UPPER(B.ZJM) LIKE UPPER('%{0}%')  OR UPPER(B.GDSPEC) LIKE UPPER('%{0}%')  OR UPPER(A.BARCODE) LIKE UPPER('%{0}%'))", trbSearch.Text.Trim());
            }
            if (ddlDEPTINT.SelectedValue.Length > 0) { strSearch += " AND A.DEPTIN ='" + ddlDEPTINT.SelectedValue + "'"; }
            if (!string.IsNullOrWhiteSpace(ddlFlag.SelectedValue)) { strSearch += " AND A.FLAG ='" + ddlFlag.SelectedValue + "'"; }
            if (tbxBILL.Text.Trim().Length > 0) { strSearch += " AND A.SEQNO='" + tbxBILL.Text + "'"; }
            if (dpkout1.SelectedDate != null) { strSearch += string.Format(" AND A.INS_TIME>=TO_DATE('{0}','YYYY-MM-DD')", dpkout1.Text); }
            if (dpkout2.SelectedDate != null) { strSearch += string.Format(" AND A.INS_TIME<TO_DATE('{0}','YYYY-MM-DD')+1", dpkout2.Text); }
            strSearch += string.Format(" AND A.DEPTIN IN( SELECT CODE FROM SYS_DEPT WHERE TYPE <>'1' AND  F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            strSearch += " ORDER BY A.INS_TIME DESC,A.SEQNO DESC,B.GDNAME";
            string strGoods = @"SELECT A.GDSEQ,B.GDNAME,B.GDSPEC,A.DHSL,F_GETUNITNAME(B.UNIT) UNITNAME,F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                                                   F_GETDEPTNAME(A.DEPTOUT) DEPTOUT,F_GETDEPTNAME(A.DEPTIN) DEPTIN,A.BARCODE,A.INS_TIME,A.SEQNO,A.FLAG,DECODE(A.FLAG,'Y','已回收','C','已作废','R','已退货','未回收') FLAGCH
                                           FROM DAT_CK_BARCODE A,DOC_GOODS B  WHERE A.GDSEQ = B.GDSEQ ";
            StringBuilder strSql = new StringBuilder(strGoods);
            strSql.Append(strSearch);
            return strSql.ToString();
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            int[] selections = GridGoods.SelectedRowIndexArray;
            string no = "";
            foreach (int rowIndex in selections)
            {
                no = no + "'" + GridGoods.DataKeys[rowIndex][0].ToString() + "',";
            }
            echo.Text = no.TrimEnd(',');
            PageContext.RegisterStartupScript("PrintClick();");
        }

        protected void trbSearch_TriggerClick(object sender, EventArgs e)
        {
            DataSearch();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            DataTable dtData = DbHelperOra.Query(GetSearchSql()).Tables[0];
            if (dtData == null || dtData.Rows.Count == 0)
            {
                Alert.Show("没有数据,无法导出！");
                return;
            }
            string[] columnNames = new string[GridGoods.Columns.Count - 1];
            for (int index = 1; index < GridGoods.Columns.Count; index++)
            {
                GridColumn column = GridGoods.Columns[index];

                if (column is FineUIPro.LinkButtonField)
                {
                    dtData.Columns[((FineUIPro.LinkButtonField)(column)).DataTextField.ToUpper()].ColumnName = column.HeaderText;
                    columnNames[index - 1] = column.HeaderText;
                }
                if (column is FineUIPro.BoundField)
                {
                    dtData.Columns[((FineUIPro.BoundField)(column)).DataField.ToUpper()].ColumnName = column.HeaderText;
                    columnNames[index - 1] = column.HeaderText;
                }
            }
            XTBase.Utilities.ExcelHelper.ExportByWeb(dtData.DefaultView.ToTable(true, columnNames), "非定数条码信息", string.Format("非定数条码信息_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }
    }
}