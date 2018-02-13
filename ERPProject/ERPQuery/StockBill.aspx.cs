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

namespace ERPProject.ERPQuery
{
    public partial class StockBill : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 在页面第一次加载时 
                BindDDL();
            }
        }

        private Boolean isDg()
        {
            if (Request.QueryString["dg"] == null)
            {
                return false;
            }
            else if (Request.QueryString["dg"].ToString() == "1")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void BindDDL()
        {
            // 绑定到下拉列表（启用模拟树功能）
            PubFunc.DdlDataGet(ddlDEPTID, "DDL_SYS_DEPOT");
            DepartmentBind.BindDDL("DDL_SYS_DEPTHASATH", UserAction.UserID);  //ddlDEPTID
        }

        private string GetSearchSql()
        {
            string strSql = @"select a.lockbillno,a.lockrowno,b.gdseq, b.gdspec,b.gdname,f_getunitname(b.unit) unitname,sum(abs(a.lockkcsl)) lockkcsl,f_getdeptname(a.deptid) deptidname,
                       b.hsjj,sum(b.hsjj*abs(a.lockkcsl)) hsje,b.zpbh,a.phid,b.bar3,f_getproducername(b.producer) PRODUCERNAME,b.pizno
                from dat_stocklock a,doc_goods b
                where a.gdseq = b.gdseq";
            string strWhere = " ";
            if (!PubFunc.StrIsEmpty(ddlDEPTID.SelectedValue)) strWhere += " and A.DEPTID = '" + ddlDEPTID.SelectedValue + "'";

            if (!PubFunc.StrIsEmpty(tbxGOODS.Text)) strWhere += " and (b.gdseq like '%" + tbxGOODS.Text.ToUpper() + "%' or b.zjm like '%" + tbxGOODS.Text.ToUpper() + "%' or b.gdname like '%" + tbxGOODS.Text.ToUpper() + "%')";

            if (!PubFunc.StrIsEmpty(tbxPHID.Text)) strWhere += " and A.PHID = '" + tbxPHID.Text + "'";

            if (!PubFunc.StrIsEmpty(tbxBILLNO.Text)) strWhere += " and A.lockbillno like '%" + tbxBILLNO.Text + "%'";
            if (strWhere != " ") strSql = strSql + strWhere;
            strSql += string.Format(" group by a.lockbillno,a.lockrowno,b.gdseq,b.gdspec,b.gdname,b.unit,a.deptid,b.hsjj,b.zpbh,a.phid,b.bar3,b.producer,b.pizno");
            strSql += string.Format(" ORDER BY {0} {1}", GridGoods.SortField, GridGoods.SortDirection);
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
            DataSearch();
        }
        protected void btClear_Click(object sender, EventArgs e)
        {
            PubFunc.FormDataClear(FormUser);
        }
        protected void btExport_Click(object sender, EventArgs e)
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
                if (column is FineUIPro.BoundField)
                {
                    //dtData.Columns[((FineUIPro.BoundField)(column)).DataField.ToUpper()].DataType = Type.GetType("System.String");
                    dtData.Columns[((FineUIPro.BoundField)(column)).DataField.ToUpper()].ColumnName = column.HeaderText;
                    columnNames[index - 1] = column.HeaderText;
                }
            }
            //((FineUIPro.Button)sender).Enabled = false;
            XTBase.Utilities.ExcelHelper.ExportByWeb(dtData.DefaultView.ToTable(true, columnNames), "商品库存信息", string.Format("商品库存报表_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
            //((FineUIPro.Button)sender).Enabled = true;
        }

        protected void GridGoods_Sort(object sender, GridSortEventArgs e)
        {
            GridGoods.SortDirection = e.SortDirection;
            GridGoods.SortField = e.SortField;

            DataSearch();
        }
        private void OutputSummaryData(DataTable source)
        {
            decimal Total = 0, HSJETotal = 0;
            foreach (DataRow row in source.Rows)
            {
                Total += Convert.ToInt32(row["lockkcsl"]);
                HSJETotal += Convert.ToDecimal(row["HSJE"]);
            }
            JObject summary = new JObject();
            summary.Add("GDNAME", "全部合计");
            summary.Add("LOCKKCSL", Total.ToString("F2"));
            summary.Add("HSJE", HSJETotal.ToString("F2"));
            GridGoods.SummaryData = summary;
        }
    }
}