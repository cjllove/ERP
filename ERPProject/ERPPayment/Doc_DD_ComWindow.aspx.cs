using FineUIPro;
using Newtonsoft.Json.Linq;
using XTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using Oracle.ManagedDataAccess.Client;

namespace ERPProject.ERPQuery
{
    public partial class Doc_DD_ComWindow : PageBase
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
            DataTable dtBill = new DataTable();
            dtBill = GetGoodsList(GridGoods.PageIndex, GridGoods.PageSize, ref total, ref strMsg);
            GridGoods.DataSource = dtBill;
            GridGoods.DataBind();
            //计算合计数量
            decimal ddslTotal = 0, bzslTotal = 0, feeTotal = 0;
            foreach (DataRow row in dtBill.Rows)
            {
                ddslTotal += Convert.ToDecimal(row["BZSL"]);
                //bzslTotal += Convert.ToDecimal(row["HSJJ"]);
                feeTotal += Convert.ToDecimal(row["HSJE"]);
            }
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("BZSL", ddslTotal.ToString());
            //summary.Add("HSJJ", bzslTotal.ToString("F2"));
            summary.Add("HSJE", feeTotal.ToString("F2"));
            GridGoods.SummaryData = summary;
        }
        public DataTable GetGoodsList(int pageNum, int pageSize, ref int total, ref string errMsg)
        {
            string sql = "SELECT SEQNO,GDSEQ,GDNAME,BARCODE,BZSL,JXTAX,HSJJ,HSJE,ZPBH,PZWH,RQ_SC,MEMO,F_GETUNITNAME(UNIT) UNITNAME,F_GETPRODUCERNAME(PRODUCER) PRODUCERNAME,GDSPEC FROM DAT_DD_COM WHERE SEQNO ='{0}' ";
            if (trbSearch.Text.Length > 0)
            {
                sql += " and (GDSEQ like '%" + trbSearch.Text + "%' or GDNAME like '%" + trbSearch.Text + "%' )";
            }
            sql += " ORDER BY ROWNO ";
            StringBuilder strSql = new StringBuilder(string.Format(sql, hfdDept.Text));
            return GetDataTable(pageNum, pageSize, strSql, ref total);
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

        protected void trbSearch_TriggerClick(object sender, EventArgs e)
        {
            hfdSearch.Text = trbSearch.Text;
            DataSearch();
        }

        protected void GridGoods_PageIndexChange(object sender, GridPageEventArgs e)
        {
            DataSearch();
        }
    }
}