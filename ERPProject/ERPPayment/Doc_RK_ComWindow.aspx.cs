﻿using FineUIPro;
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
    public partial class Doc_RK_ComWindow : PageBase
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
                if (Request.QueryString["tp"] != null && Request.QueryString["tp"].ToString() != "")
                {
                    type.Text = Request.QueryString["tp"].ToString();
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
                //if (hfdSupplier.Text == row["PSSID"].ToString())
                //{
                //    bzslTotal += Convert.ToDecimal(row["HSJE"]);
                //}
                feeTotal += Convert.ToDecimal(row["HSJE"]);
            }
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("BZSL", ddslTotal.ToString());
            //summary.Add("UNITNAME", "结算金额");
            //summary.Add("GDSPEC", bzslTotal.ToString("F2"));
            summary.Add("HSJE", feeTotal.ToString("F2"));
            GridGoods.SummaryData = summary;
        }
        public DataTable GetGoodsList(int pageNum, int pageSize, ref int total, ref string errMsg)
        {
            //string sql = @"SELECT SEQNO,GDSEQ,GDNAME,BARCODE,BZSL,JXTAX,HSJJ,HSJE,ZPBH,HWID,PZWH,RQ_SC,MEMO,F_GETUNITNAME(UNIT) UNITNAME,GDSPEC FROM DAT_RK_COM WHERE SEQNO ='{0}' union all
            //                SELECT SEQNO,GDSEQ,GDNAME,BARCODE,BZSL,JXTAX,HSJJ,HSJE,ZPBH,HWID,PZWH,RQ_SC,MEMO,F_GETUNITNAME(UNIT) UNITNAME,GDSPEC FROM DAT_TH_COM WHERE SEQNO ='{0}' ";
            string sql = @"SELECT * FROM (SELECT A.SEQNO,A.GDSEQ,A.GDNAME,A.BARCODE,A.BZSL,A.JXTAX,A.HSJJ,B.SL*B.HSJJ HSJE,A.ZPBH,A.HWID,A.PZWH,A.RQ_SC,A.MEMO,F_GETUNITNAME(A.UNIT) UNITNAME
                ,A.GDSPEC,B.SL,NVL(B.PSSID,B.SUPID) PSSID
                FROM DAT_RK_COM A,DAT_GOODSJXC B WHERE A.SEQNO = B.BILLNO AND A.GDSEQ = B.GDSEQ AND A.ROWNO = B.ROWNO AND SL > 0  AND A.SEQNO ='{0}'
                UNION ALL
                SELECT A.SEQNO,A.GDSEQ,A.GDNAME,A.BARCODE,A.BZSL,A.JXTAX,A.HSJJ,B.SL*B.HSJJ HSJE,A.ZPBH,A.HWID,A.PZWH,A.RQ_SC,A.MEMO,F_GETUNITNAME(A.UNIT) UNITNAME
                ,A.GDSPEC,B.SL,NVL(B.PSSID,B.SUPID) PSSID
                FROM DAT_TH_COM A,DAT_GOODSJXC B WHERE A.SEQNO = B.BILLNO AND A.GDSEQ = B.GDSEQ AND A.ROWNO = B.ROWNO AND SL < 0  AND A.SEQNO ='{0}') WHERE 1=1";
            if (trbSearch.Text.Length > 0)
            {
                sql += " AND (GDSEQ like '%" + trbSearch.Text + "%' or GDNAME like '%" + trbSearch.Text + "%' )";
            }
            if (hfdSupplier.Text.Length > 0 && type.Text.Length > 0)
            {
                sql += " AND GDSEQ IN (SELECT GDSEQ FROM DOC_GOODSSUP WHERE STR3 = '" + type.Text + "' AND NVL(PSSID,SUPID) = '" + hfdSupplier.Text + "')";
            }
            //sql += " ORDER BY ROWNO ";
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

        protected void GridGoods_RowDataBound(object sender, GridRowEventArgs e)
        {
            //DataRowView row = e.DataItem as DataRowView;
            //if (row != null)
            //{
            //    string flag = row["PSSID"].ToString();
            //    if (flag != hfdSupplier.Text)
            //    {
            //        highlightRows.Text += e.RowIndex.ToString() + ",";
            //    }
            //}
        }
    }
}