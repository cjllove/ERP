using FineUIPro;
using Newtonsoft.Json.Linq;
using XTBase;
using XTBase.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPProject.ERPQuery
{
    public partial class StockOutQuery : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 在页面第一次加载时 
                BindDDL();
            }
        }

        private void BindDDL()
        {
            PubFunc.DdlDataGet(ddlDEPTID, "DDL_SYS_DEPTDEF");
            lstLRRQ1.SelectedDate = DateTime.Now;
            lstLRRQ2.SelectedDate = DateTime.Now;
            dpkBegRQ.SelectedDate = DateTime.Now;
            dpkEndRQ.SelectedDate = DateTime.Now;
        }

        private string GetSearchSql()
        {
            string strSql = @"SELECT SD.NAME DEPTNAME,B.GDSEQ,B.GDNAME,B.GDSPEC,F_GETUNITNAME(B.UNIT) UNIT,B.HSJJ,SUM(B.XSSL) SL,SUM(B.HSJE) JE,G.HISCODE
                                       FROM DAT_CK_DOC A, DAT_CK_COM B, SYS_DEPT SD, DOC_GOODS G
                                      WHERE A.SEQNO = B.SEQNO AND B.GDSEQ = G.GDSEQ AND A.DEPTID = SD.CODE  AND A.FLAG='Y' ";
            string strWhere = " ";

            //if (!PubFunc.StrIsEmpty(SELECTSUPID.Text.Trim())) strWhere += " AND A.SUPID = '" + SELECTSUPID.Text.Trim() + "'";
            if (!PubFunc.StrIsEmpty(ddlDEPTID.SelectedValue)) strWhere += " AND A.DEPTID = '" + ddlDEPTID.SelectedValue + "'";
            if (!PubFunc.StrIsEmpty(tbxGOODS.Text)) strWhere += " and (G.GDSEQ like '%" + tbxGOODS.Text + "%' or g.zjm like '%" + tbxGOODS.Text + "%' or g.gdname like '%" + tbxGOODS.Text + "%')";

            //if (!PubFunc.StrIsEmpty(SELECTPRODUCERID.Text.Trim())) strWhere += " and g.producer = '" + SELECTPRODUCERID.Text.Trim() + "'";
            strWhere += string.Format(" and A.XSRQ>=TO_DATE('{0}','YYYY-MM-DD') and A.XSRQ < TO_DATE('{1}','YYYY-MM-DD')+1 ", lstLRRQ1.Text, lstLRRQ2.Text);
            //strWhere += string.Format(" AND a.deptid in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            if (strWhere != " ") strSql = strSql + strWhere;

            strSql += string.Format(" GROUP BY SD.NAME, B.GDSEQ, B.GDNAME, B.GDSPEC, B.UNIT, B.HSJJ, G.HISCODE ORDER BY {0} {1}", GridGoods.SortField, GridGoods.SortDirection);
            return strSql;
        }

        private string GetSearchSql1()
        {
            string strSql = @"SELECT B.GDSEQ,B.GDNAME,B.GDSPEC,F_GETUNITNAME(B.UNIT) UNIT,B.HSJJ,SUM(B.XSSL) SL,SUM(B.HSJE) JE,G.HISCODE
                                       FROM DAT_CK_DOC A, DAT_CK_COM B, DOC_GOODS G
                                      WHERE A.SEQNO = B.SEQNO AND B.GDSEQ = G.GDSEQ AND A.FLAG='Y' ";
            string strWhere = " ";

            if (!PubFunc.StrIsEmpty(trbSearch.Text)) strWhere += " and (G.GDSEQ like '%" + trbSearch.Text + "%' or g.zjm like '%" + trbSearch.Text + "%' or g.gdname like '%" + trbSearch.Text + "%')";

            strWhere += string.Format(" and A.XSRQ>=TO_DATE('{0}','YYYY-MM-DD') and A.XSRQ < TO_DATE('{1}','YYYY-MM-DD')+1 ", dpkBegRQ.Text, dpkEndRQ.Text);
            if (strWhere != " ") strSql = strSql + strWhere;

            strSql += " GROUP BY B.GDSEQ, B.GDNAME, B.GDSPEC, B.UNIT, B.HSJJ, G.HISCODE";
            return strSql;
        }

        private void DataSearch()
        {
            int total = 0;

            DataTable dtData = PubFunc.DbGetPage(GridGoods.PageIndex, GridGoods.PageSize, GetSearchSql(), ref total);
            OutputSummaryData(dtData);
            GridGoods.RecordCount = total;
            GridGoods.DataSource = dtData;
            GridGoods.DataBind();
        }

        protected void GridGoods_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridGoods.PageIndex = e.NewPageIndex;
            DataSearch();
        }
        protected void btSearch_Click(object sender, EventArgs e)
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【查询期间】！");
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                Alert.Show("开始日期大于结束日期，请重新输入！");
                return;
            }
            DataSearch();
        }
        protected void btClear_Click(object sender, EventArgs e)
        {
            PubFunc.FormDataClear(FormUser);
            lstLRRQ1.SelectedDate = DateTime.Now;
            lstLRRQ2.SelectedDate = DateTime.Now;
        }
        protected void btExport_Click(object sender, EventArgs e)
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【查询期间】！");
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                Alert.Show("开始日期大于结束日期，请重新输入！");
                return;
            }
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
                if (column is FineUIPro.BoundField)
                {
                    //dtData.Columns[((FineUIPro.BoundField)(column)).DataField.ToUpper()].DataType = Type.GetType("System.String");
                    dtData.Columns[((FineUIPro.BoundField)(column)).DataField.ToUpper()].ColumnName = column.HeaderText;
                    columnNames[index - 1] = column.HeaderText;
                }
            }
            //((FineUIPro.Button)sender).Enabled = false;
            ExcelHelper.ExportByWeb(dtData.DefaultView.ToTable(true, columnNames), "商品出库信息汇总", string.Format("商品出库信息汇总_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
            //((FineUIPro.Button)sender).Enabled = true;
        }

        protected void GridGoods_Sort(object sender, GridSortEventArgs e)
        {
            GridGoods.SortDirection = e.SortDirection;
            GridGoods.SortField = e.SortField;

            DataSearch();
        }

        //protected void SELECTSUPNAME_TextChanged(object sender, EventArgs e)
        //{
        //    if (SELECTSUPNAME.Text.Length == 0)
        //    {
        //        SELECTSUPID.Text = "";
        //    }
        //}

        //protected void SELECTPRODUCERNAME_TextChanged(object sender, EventArgs e)
        //{
        //    if (SELECTPRODUCERNAME.Text.Length == 0)
        //    {
        //        SELECTPRODUCERID.Text = "";
        //    }
        //}


        private void OutputSummaryData(DataTable source)
        {
            decimal HSJJTotal = 0, HSJETotal = 0;
            foreach (DataRow row in source.Rows)
            {
                HSJJTotal += Convert.ToInt32(row["SL"]);
                HSJETotal += Convert.ToDecimal(row["JE"]);
            }
            JObject summary = new JObject();
            summary.Add("GDNAME", "全部合计");
            summary.Add("SL", HSJJTotal.ToString("F2"));
            summary.Add("JE", HSJETotal.ToString("F2"));
            GridGoods.SummaryData = summary;
        }

        private void OutputSummaryData1(DataTable source)
        {
            decimal HSJJTotal = 0, HSJETotal = 0;
            foreach (DataRow row in source.Rows)
            {
                HSJJTotal += Convert.ToInt32(row["SL"]);
                HSJETotal += Convert.ToDecimal(row["JE"]);
            }
            JObject summary = new JObject();
            summary.Add("COUNTTITLE", "全部合计");
            summary.Add("HJSL", HSJJTotal.ToString("F2"));
            summary.Add("HJJE", HSJETotal.ToString("F2"));
            GridCom.SummaryData = summary;
        }

        protected void btnClear1_Click(object sender, EventArgs e)
        {
            PubFunc.FormDataClear(FormSearch);
            dpkBegRQ.SelectedDate = DateTime.Now;
            dpkEndRQ.SelectedDate = DateTime.Now;
        }

        protected void btnExport1_Click(object sender, EventArgs e)
        {
            if (dpkBegRQ.SelectedDate == null || dpkEndRQ.SelectedDate == null)
            {
                Alert.Show("请输入条件【查询期间】！");
                return;
            }
            else if (dpkBegRQ.SelectedDate > dpkEndRQ.SelectedDate)
            {
                Alert.Show("开始日期大于结束日期，请重新输入！");
                return;
            }
            DataTable dtData = DbHelperOra.Query(GetSearchSql1()).Tables[0];
            if (dtData == null || dtData.Rows.Count == 0)
            {
                Alert.Show("没有数据,无法导出！");
                return;
            }
            string[] columnNames = new string[GridCom.Columns.Count - 1];
            for (int index = 1; index < GridCom.Columns.Count; index++)
            {
                GridColumn column = GridCom.Columns[index];
                if (column is FineUIPro.BoundField)
                {
                    dtData.Columns[((FineUIPro.BoundField)(column)).DataField.ToUpper()].ColumnName = column.HeaderText;
                    columnNames[index - 1] = column.HeaderText;
                }
            }

            ExcelHelper.ExportByWeb(dtData.DefaultView.ToTable(true, columnNames), "商品出库信息汇总", string.Format("商品出库信息汇总_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }

        protected void btnSearch1_Click(object sender, EventArgs e)
        {
            if (dpkBegRQ.SelectedDate == null || dpkEndRQ.SelectedDate == null)
            {
                Alert.Show("请输入条件【查询期间】！");
                return;
            }
            else if (dpkBegRQ.SelectedDate > dpkEndRQ.SelectedDate)
            {
                Alert.Show("开始日期大于结束日期，请重新输入！");
                return;
            }

            DataTable dtData = DbHelperOra.Query(GetSearchSql1()).Tables[0];
            OutputSummaryData1(dtData);
            GridCom.DataSource = dtData;
            GridCom.DataBind();
        }
    }
}