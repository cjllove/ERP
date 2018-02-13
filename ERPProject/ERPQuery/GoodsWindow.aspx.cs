using FineUIPro;
using XTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPProject.ERPQuery
{
    public partial class GoodsWindow : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["cx"] != null && Request.QueryString["cx"].ToString() != "")
                {
                    hfdSearch.Text = Request.QueryString["cx"].ToString();
                }
                if (Request.QueryString["bm"] != null && Request.QueryString["bm"].ToString() != "")
                {
                    hfdDept.Text = Request.QueryString["bm"].ToString();
                }
                if (Request.QueryString["su"] != null && Request.QueryString["su"].ToString() != "")
                {
                    hfdSupplier.Text = Request.QueryString["su"].ToString();
                }
                DataSearch();
                btnClose.OnClientClick = ActiveWindow.GetHideReference();

                BillBase.Grid_Goods = GridGoods;
            }
        }

        private void DataSearch()
        {
            int total = 0;
            string sql = @"select  SP.*,F_GETUNITNAME(UNIT) UNITNAME,F_GETPRODUCERNAME(SP.PRODUCER) PRODUCERNAME,F_GETSUPNAME(SUPPLIER) SUPPLIERNAME, 
                                   F_GETUNITNAME(UNIT_ORDER) UNIT_ORDER_NAME,F_GETUNITNAME(UNIT_SELL) UNIT_SELL_NAME,F_GETUNITNAME(UNIT_DABZ) UNIT_DABZ_NAME,
                                   F_GETUNITNAME(UNIT_ZHONGBZ) UNIT_ZHONGBZ_NAME,PZ.HJCODE1 HWID
                             from  DOC_GOODS SP,DOC_GOODSCFG PZ WHERE ISDELETE='N' and sp.flag='Y' AND SP.GDSEQ=PZ.GDSEQ(+) ";
            StringBuilder strSql = new StringBuilder(sql);
            if (!string.IsNullOrWhiteSpace(hfdSearch.Text))
            {
                strSql.AppendFormat(" AND (SP.GDSEQ LIKE '%{0}%' OR SP.GDNAME LIKE '%{0}%' OR SP.ZJM LIKE '%{0}%' OR SP.BARCODE LIKE '%{0}%')", hfdSearch.Text.ToUpper());
            }
            if (!string.IsNullOrWhiteSpace(hfdDept.Text))
            {
                strSql.AppendFormat(" AND PZ.DEPTID='{0}' AND PZ.ISCFG='1'", hfdDept.Text);
            }
            if (!string.IsNullOrWhiteSpace(hfdSupplier.Text))
            {
                strSql.AppendFormat(" AND SP.SUPPLIER = '{0}'", hfdSupplier.Text);
            }
            strSql.AppendFormat(" ORDER BY SP.{0} {1}", GridGoods.SortField, GridGoods.SortDirection);

            DataTable dtData = PubFunc.DbGetPage(GridGoods.PageIndex, GridGoods.PageSize, strSql.ToString(), ref total);
            GridGoods.RecordCount = total;
            GridGoods.DataSource = dtData;
            GridGoods.DataBind();
        }

        protected void GridGoods_RowDoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            string GoodsInfo = GetRowValue(GridGoods.Rows[e.RowIndex]);
            FineUIPro.PageContext.RegisterStartupScript(FineUIPro.ActiveWindow.GetWriteBackValueReference(GoodsInfo) + FineUIPro.ActiveWindow.GetHidePostBackReference());
        }

        private string GetRowValue(GridRow row)
        {
            string strValue = "";
            for (int i = 0; i < GridGoods.Columns.Count; i++)
            {
                strValue += row.Values[i].ToString() == "" ? "★♀" : row.Values[i].ToString() + "♀";
            }
            return strValue.TrimEnd('♀');
        }

        protected void btnClosePostBack_Click(object sender, EventArgs e)
        {
            string GoodsInfo = "";
            int[] row = GridGoods.SelectedRowIndexArray;
            foreach (int index in row)
            {
                GoodsInfo += GetRowValue(GridGoods.Rows[index]) + "♂";
            }
            FineUIPro.PageContext.RegisterStartupScript(FineUIPro.ActiveWindow.GetWriteBackValueReference(GoodsInfo.TrimEnd(';')) + FineUIPro.ActiveWindow.GetHidePostBackReference());
        }

        protected void trbSearch_TriggerClick(object sender, EventArgs e)
        {
            hfdSearch.Text = trbSearch.Text;
            DataSearch();
        }

        protected void GridGoods_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridGoods.PageIndex = e.NewPageIndex;
            DataSearch();
        }

        protected void GridGoods_Sort(object sender, GridSortEventArgs e)
        {
            GridGoods.SortDirection = e.SortDirection;
            GridGoods.SortField = e.SortField;

            DataSearch();
        }
    }
}