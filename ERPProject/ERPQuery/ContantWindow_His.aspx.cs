﻿using FineUIPro;
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
    public partial class ContantWindow_His : PageBase
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
                    //是否高值使用
                    hfdSupplier.Text = Request.QueryString["su"].ToString();
                    if (hfdSupplier.Text == "Y")
                    {
                        
                        GridColumn DSNUMColumn = GridGoods.FindColumn("DSNUM");
                        DSNUMColumn.Hidden = true;
                        GridColumn NUM1Column = GridGoods.FindColumn("NUM1");
                        NUM1Column.Hidden = true;
                        GridColumn NUM3Column = GridGoods.FindColumn("NUM3");
                        NUM3Column.Hidden = true;

                    }
                }
                DataSearch();
                btnClose.OnClientClick = ActiveWindow.GetHideReference();

                BillBase.Grid_Goods = GridGoods;
            }
        }

        private void DataSearch()
        {
            int total = 0;
            //使用his名称、规格,SP.GDNAME,SP.GDSPEC
            string sql = @"select  SP.GDSEQ,SP.GDID,SP.BARCODE,SP.ZJM,SP.YCODE,SP.NAMEJC,SP.NAMEEN,SP.GDMODE,SP.STRUCT,SP.BZHL,SP.UNIT,SP.FLAG,SP.CATID,SP.JX,SP.YX,SP.PIZNO,SP.BAR1,SP.BAR2,SP.BAR3,SP.DEPTID,SP.SUPPLIER,SP.LOGINLABEL,SP.PRODUCER,SP.ZPBH,SP.PPID,SP.CDID,SP.JXTAX,SP.XXTAX,SP.BHSJJ,SP.HSJJ,SP.LSJ,SP.YBJ,SP.HSID,SP.HSJ,SP.JHZQ,PZ.ZDKC,
                                    SP.HLKC,SP.ZGKC,SP.SPZT,SP.DAYXS,SP.MANAGER,SP.INPER,SP.INRQ,SP.BEGRQ,SP.ENDRQ,SP.UPTRQ,SP.UPTUSER,SP.MEMO,DISABLEORG,SP.ISLOT,SP.ISJB,SP.ISFZ,SP.ISGZ,SP.ISIN,SP.ISJG,SP.ISDM,SP.ISCF,SP.ISYNZJ,SP.ISFLAG1,nvl(SP.STR3,SP.GDSPEC) GDSPEC,SP.UNIT_DABZ,SP.UNIT_ZHONGBZ,SP.BARCODE_DABZ,SP.NUM_DABZ,SP.NUM_ZHONGBZ,SP.UNIT_ORDER,SP.UNIT_SELL,SP.HISCODE,nvl(SP.HISNAME,SP.GDNAME) GDNAME,SP.CATID0,
                                    F_GETUNITNAME(UNIT) UNITNAME,F_GETPRODUCERNAME(SP.PRODUCER) PRODUCERNAME,F_GETSUPNAME(SUPPLIER) SUPPLIERNAME, 
                                   F_GETUNITNAME(UNIT_ORDER) UNIT_ORDER_NAME,F_GETUNITNAME(UNIT_SELL) UNIT_SELL_NAME,F_GETUNITNAME(UNIT_DABZ) UNIT_DABZ_NAME,
                                   F_GETUNITNAME(UNIT_ZHONGBZ) UNIT_ZHONGBZ_NAME,PZ.HJCODE1 HWID,NVL(PZ.NUM1,0) NUM1,NVL(PZ.NUM3,0) NUM3,NVL(PZ.DSNUM,0) DSNUM
                             from  DOC_GOODS SP,DOC_GOODSCFG PZ WHERE ISDELETE='N' and sp.flag='Y' AND SP.GDSEQ=PZ.GDSEQ(+) ";
            StringBuilder strSql = new StringBuilder(sql);
            if (!string.IsNullOrWhiteSpace(hfdSearch.Text))
            {
                strSql.AppendFormat(" AND (SP.GDSEQ LIKE '%{0}%' OR SP.GDNAME LIKE '%{0}%' OR SP.ZJM LIKE '%{0}%' OR SP.BARCODE LIKE '%{0}%')", hfdSearch.Text.ToUpper());
            }
            if (!string.IsNullOrWhiteSpace(hfdDept.Text))
            {
                strSql.AppendFormat(" AND PZ.DEPTID='{0}' AND PZ.ISCFG IN ('1','Y')", hfdDept.Text);
            }
            if (hfdSupplier.Text == "N")
            {
                strSql.AppendFormat(" AND SP.ISGZ = 'N'");
            }
            else
            {
                strSql.AppendFormat(" AND SP.ISGZ = 'Y'");
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