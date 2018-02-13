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

namespace ERPProject.Certificate
{
    public partial class CertificateWindow : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataSearch();
                btnClose.OnClientClick = ActiveWindow.GetHideReference();
                BillBase.Grid_Goods = GridGoods;
            }
        }

        private void DataSearch()
        {
            int total = 0;
            string sql = @"SELECT sg.GDSEQ,sg.GDNAME,sg.GDSPEC,sg.PIZNO,sg.PRODUCER,sg.ZPBH,dg.NAME,sg.UNIT,ds.SUPNAME FROM DOC_GOODS sg ,doc_supplier ds ,DOC_GOODSUNIT dg WHERE ds.SUPID = sg.SUPPLIER and dg.CODE=sg.UNIT ";
            StringBuilder strSql = new StringBuilder(sql);
            if (!string.IsNullOrWhiteSpace(hfdSearch.Text))
            {
                strSql.AppendFormat(" AND (sg.GDSEQ LIKE '%{0}%' OR sg.GDNAME LIKE '%{0}%' OR sg.GDSPEC LIKE '%{0}%' OR sg.PIZNO LIKE '%{0}%' OR ds.SUPNAME LIKE '%{0}%')", hfdSearch.Text);
            }
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
            if (row.Length == 0)
            {
                Alert.Show("请至少选择一个商品!");
                return;
            }
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