using System;
using FineUIPro;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XTBase;


namespace ERPProject.CertificateInput
{
    public partial class SupplierList : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                string listype = Request.QueryString["bm"].ToString();
                hfdLicType.Text = Request.QueryString["bm"].ToString();
                if (listype.Equals("producer"))
                {
                    dataSearch();
                }
                if (listype.Equals("agent"))
                {
                    dataSearch1();
                }
                if(listype.Equals("supplier"))
                {
                    dataSearch2();
                }
            }
        }

        protected void trbSearch_TriggerClick(object sender, EventArgs e)
        {
            if (hfdLicType.Text.Equals("producer"))
            {
                dataSearch();
            }
            if (hfdLicType.Text.Equals("agent"))
            {
                dataSearch1();
            }
            if(hfdLicType.Text.Equals("supplier"))
            {
                dataSearch2();
            }
        }

        protected void GridSupplier_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridSupplier.PageIndex = e.NewPageIndex;
            dataSearch();
        }

        private void dataSearch()
        {
            //根据授权书里的商品查询生产厂商
            string sql = @"select supid,supname,supcat,regid from doc_supplier t where isproducer='Y' ";
            string query = "";
            StringBuilder strSql = new StringBuilder(sql);

            if (!string.IsNullOrWhiteSpace(trbSearch.Text))
            {
                query = trbSearch.Text.Trim();
                strSql.AppendFormat(" and (supid LIKE '%{0}%' or supname like '%{0}%')", query);
            }
            int total = 0;

            DataTable dtData = PubFunc.DbGetPage(GridSupplier.PageIndex, GridSupplier.PageSize, strSql.ToString(), ref total);
            GridSupplier.RecordCount = total;
            GridSupplier.DataSource = dtData;
            GridSupplier.DataBind();
        }

        private void dataSearch1()
        {
            //根据授权书里的商品查询生产厂商
            string sql = @"  select supid,supname,supcat,regid from doc_supplier t where isagent='Y' and flag='Y' union all select supid,supname,supcat,regid from doc_supplier t where issupplier='Y' ";
            string query = "";
            StringBuilder strSql = new StringBuilder(sql);

            if (!string.IsNullOrWhiteSpace(trbSearch.Text))
            {
                query = trbSearch.Text.Trim();
                strSql.AppendFormat(" and (supid LIKE '%{0}%' or supname like '%{0}%')", query);
            }
            int total = 0;
            
            DataTable dtData = PubFunc.DbGetPage(GridSupplier.PageIndex, GridSupplier.PageSize, strSql.ToString(), ref total);
            GridSupplier.RecordCount = total;
            GridSupplier.DataSource = dtData;
            GridSupplier.DataBind();
        }

         private void dataSearch2()
        {
            //根据授权书里的商品查询生产厂商
            string sql = @"select supid,supname,supcat,regid from doc_supplier t where issupplier='Y' ";
            string query = "";
            StringBuilder strSql = new StringBuilder(sql);

            if (!string.IsNullOrWhiteSpace(trbSearch.Text))
            {
                query = trbSearch.Text.Trim();
                strSql.AppendFormat(" and (supid LIKE '%{0}%' or supname like '%{0}%')", query);
            }
            int total = 0;
            
            DataTable dtData = PubFunc.DbGetPage(GridSupplier.PageIndex, GridSupplier.PageSize, strSql.ToString(), ref total);
            GridSupplier.RecordCount = total;
            GridSupplier.DataSource = dtData;
            GridSupplier.DataBind();
        }

        protected void btnClosePostBack_Click(object sender, EventArgs e)
        {
            string GoodsInfo = "";
            int[] row = GridSupplier.SelectedRowIndexArray;
            JArray ja = new JArray();

            foreach (int index in row)
            {
                //GoodsInfo += GetRowValue(GridGoods.Rows[index]) + "♂";
                JArray rowJa = new JArray();
                rowJa = JsonConvert.DeserializeObject<JArray>(JsonConvert.SerializeObject(GridSupplier.Rows[index].Values));
                ja.Add(rowJa);
            }
            GoodsInfo = JsonConvert.SerializeObject(ja);
            PageContext.RegisterStartupScript(FineUIPro.ActiveWindow.GetWriteBackValueReference(GoodsInfo.TrimEnd(';')) + FineUIPro.ActiveWindow.GetHidePostBackReference());
        }

        protected void GridSupplier_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            string GoodsInfo = "";
            int[] row = GridSupplier.SelectedRowIndexArray;
            JArray ja = new JArray();

            foreach (int index in row)
            {
                //GoodsInfo += GetRowValue(GridGoods.Rows[index]) + "♂";
                JArray rowJa = new JArray();
                rowJa = JsonConvert.DeserializeObject<JArray>(JsonConvert.SerializeObject(GridSupplier.Rows[index].Values));
                ja.Add(rowJa);
            }
            GoodsInfo = JsonConvert.SerializeObject(ja);
            PageContext.RegisterStartupScript(FineUIPro.ActiveWindow.GetWriteBackValueReference(GoodsInfo.TrimEnd(';')) + FineUIPro.ActiveWindow.GetHidePostBackReference());
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            //PageContext.RegisterStartupScript(ActiveWindow.GetHideRefreshReference()); //关闭窗体并刷新父页面
            PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());  //关闭窗体并回发父窗体
        }
    }
}