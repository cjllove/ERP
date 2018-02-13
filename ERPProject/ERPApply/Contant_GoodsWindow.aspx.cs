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
    public partial class Contant_GoodsWindow : PageBase
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
            string strMsg = "";
            GridGoods.DataSource = GetGoodsList(GridGoods.PageIndex, GridGoods.PageSize, ref total, ref strMsg);
            GridGoods.RecordCount = total;
            GridGoods.DataBind();
        }

        /// <summary>
        /// 获取商品数据信息
        /// </summary>
        /// <param name="pageNum">第几页</param>
        /// <param name="pageSize">每页显示天数</param>
        /// <param name="nvc">查询条件</param>
        /// <param name="total">总的条目数</param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public DataTable GetGoodsList(int pageNum, int pageSize, ref int total, ref string errMsg)
        {
            string sql = @"select  SP.*,F_GETUNITNAME(UNIT) UNITNAME,F_GETPRODUCERNAME(SP.PRODUCER) PRODUCERNAME,F_GETSUPNAME(SUPPLIER) SUPPLIERNAME,(nvl(PZ.DSNUM,0) - nvl(PZ.NUM3,0)) sum_num,PZ.DSNUM,nvl(PZ.NUM1,0) NUM_XS,nvl(PZ.NUM3,0) NUM_DS,
                                   F_GETUNITNAME(UNIT_ORDER) UNIT_ORDER_NAME,F_GETUNITNAME(UNIT_SELL) UNIT_SELL_NAME,F_GETUNITNAME(UNIT_DABZ) UNIT_DABZ_NAME,
                                   F_GETUNITNAME(UNIT_ZHONGBZ) UNIT_ZHONGBZ_NAME,PZ.HJCODE1 HWID";
            StringBuilder strSql = new StringBuilder(sql);
            if (!string.IsNullOrWhiteSpace(hfdSearch.Text))
            {
                strSql.AppendFormat(" ,(select nvl(sum(KCSL),0) from DAT_GOODSSTOCK a where a.gdseq = SP.GDSEQ and a.deptid = '{0}') KCSL,floor((select nvl(sum(KCSL),0) from DAT_GOODSSTOCK a where a.gdseq = SP.GDSEQ and a.deptid = '{0}')/PZ.NUM1) SL", hfdSearch.Text);
            }
            if (!string.IsNullOrWhiteSpace(hfdDept.Text))
            {
                strSql.AppendFormat(" from  DOC_GOODS SP,DOC_GOODSCFG PZ WHERE ISDELETE='N' AND SP.GDSEQ=PZ.GDSEQ AND PZ.DSNUM > 0 AND nvl(PZ.NUM3,0) < nvl(PZ.DSNUM,0) and nvl(PZ.NUM1,0) > 0 AND PZ.DEPTID='{0}' AND PZ. ISCFG IN ('1','Y')", hfdDept.Text);
            }
            strSql.Append(" ORDER BY SP.GDNAME,KCSL");
            return GetDataTable(pageNum, pageSize, strSql, ref total);
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
    }
}