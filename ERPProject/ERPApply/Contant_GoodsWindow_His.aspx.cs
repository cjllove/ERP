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
    public partial class Contant_GoodsWindow_His : PageBase
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
            string sql = "";
            if (DbHelperOra.Exists("SELECT 1 FROM SYS_PARA WHERE CODE = 'ShowName' AND VALUE = 'HIS'"))
            {
                sql = @"SELECT  SP.GDSEQ,SP.GDID,SP.BARCODE,SP.ZJM,SP.YCODE,SP.NAMEJC,SP.NAMEEN,SP.GDMODE,SP.STRUCT,SP.BZHL,SP.UNIT,SP.FLAG,SP.CATID,SP.JX,SP.YX,SP.PIZNO,SP.BAR1,SP.BAR2,SP.BAR3,SP.DEPTID,SP.SUPPLIER,SP.LOGINLABEL,SP.PRODUCER,SP.ZPBH,SP.PPID,SP.CDID,SP.JXTAX,SP.XXTAX,SP.BHSJJ,SP.HSJJ,SP.LSJ,SP.YBJ,SP.HSID,SP.HSJ,SP.JHZQ,SP.ZDKC,
                                        SP.HLKC,SP.ZGKC,SP.SPZT,SP.DAYXS,SP.MANAGER,SP.INPER,SP.INRQ,SP.BEGRQ,SP.ENDRQ,SP.UPTRQ,SP.UPTUSER,SP.MEMO,DISABLEORG,SP.ISLOT,SP.ISJB,SP.ISFZ,SP.ISGZ,SP.ISIN,SP.ISJG,SP.ISDM,SP.ISCF,SP.ISYNZJ,SP.ISFLAG1,nvl(SP.STR3,SP.GDSPEC) GDSPEC,SP.UNIT_DABZ,SP.UNIT_ZHONGBZ,SP.BARCODE_DABZ,SP.NUM_DABZ,SP.NUM_ZHONGBZ,SP.UNIT_ORDER,SP.UNIT_SELL,SP.HISCODE,nvl(SP.HISNAME,SP.GDNAME) GDNAME,SP.CATID0,
                                        F_GETUNITNAME(UNIT) UNITNAME,F_GETPRODUCERNAME(SP.PRODUCER) PRODUCERNAME,F_GETSUPNAME(SUPPLIER) SUPPLIERNAME,(nvl(PZ.DSNUM,0) - nvl(PZ.NUM3,0)- nvl(PZ.DSPOOL,0)) sum_num,PZ.DSNUM,nvl(PZ.NUM1,0) NUM_XS,nvl(PZ.NUM3,0) NUM_DS,
                                       F_GETUNITNAME(UNIT_ORDER) UNIT_ORDER_NAME,F_GETUNITNAME(UNIT_SELL) UNIT_SELL_NAME,F_GETUNITNAME(UNIT_DABZ) UNIT_DABZ_NAME,NVL(PZ.ISJF,'Y') ISJF,
                                       F_GETUNITNAME(UNIT_ZHONGBZ) UNIT_ZHONGBZ_NAME,f_gethwid('{1}',SP.GDSEQ) HWID,NVL((SELECT wmsys.wm_concat(gdseq) FROM VIEW_DS WHERE GDSEQ_OLD = SP.GDSEQ AND DEPTID = '{0}'),'不存在') DT";
            }
            else
            {
                sql = @"SELECT  SP.GDSEQ,SP.GDID,SP.BARCODE,SP.ZJM,SP.YCODE,SP.NAMEJC,SP.NAMEEN,SP.GDMODE,SP.STRUCT,SP.BZHL,SP.UNIT,SP.FLAG,SP.CATID,SP.JX,SP.YX,SP.PIZNO,SP.BAR1,SP.BAR2,SP.BAR3,SP.DEPTID,SP.SUPPLIER,SP.LOGINLABEL,SP.PRODUCER,SP.ZPBH,SP.PPID,SP.CDID,SP.JXTAX,SP.XXTAX,SP.BHSJJ,SP.HSJJ,SP.LSJ,SP.YBJ,SP.HSID,SP.HSJ,SP.JHZQ,SP.ZDKC,
                                        SP.HLKC,SP.ZGKC,SP.SPZT,SP.DAYXS,SP.MANAGER,SP.INPER,SP.INRQ,SP.BEGRQ,SP.ENDRQ,SP.UPTRQ,SP.UPTUSER,SP.MEMO,DISABLEORG,SP.ISLOT,SP.ISJB,SP.ISFZ,SP.ISGZ,SP.ISIN,SP.ISJG,SP.ISDM,SP.ISCF,SP.ISYNZJ,SP.ISFLAG1,nvl(SP.STR3,SP.GDSPEC) GDSPEC,SP.UNIT_DABZ,SP.UNIT_ZHONGBZ,SP.BARCODE_DABZ,SP.NUM_DABZ,SP.NUM_ZHONGBZ,SP.UNIT_ORDER,SP.UNIT_SELL,SP.HISCODE,SP.GDNAME,SP.CATID0,
                                        F_GETUNITNAME(UNIT) UNITNAME,F_GETPRODUCERNAME(SP.PRODUCER) PRODUCERNAME,F_GETSUPNAME(SUPPLIER) SUPPLIERNAME,(nvl(PZ.DSNUM,0) - nvl(PZ.NUM3,0)- nvl(PZ.DSPOOL,0)) sum_num,PZ.DSNUM,nvl(PZ.NUM1,0) NUM_XS,nvl(PZ.NUM3,0) NUM_DS,
                                       F_GETUNITNAME(UNIT_ORDER) UNIT_ORDER_NAME,F_GETUNITNAME(UNIT_SELL) UNIT_SELL_NAME,F_GETUNITNAME(UNIT_DABZ) UNIT_DABZ_NAME,NVL(PZ.ISJF,'Y') ISJF,
                                       F_GETUNITNAME(UNIT_ZHONGBZ) UNIT_ZHONGBZ_NAME,f_gethwid('{1}',SP.GDSEQ) HWID,NVL((SELECT wmsys.wm_concat(gdseq) FROM VIEW_DS WHERE GDSEQ_OLD = SP.GDSEQ AND DEPTID = '{1}'),'不存在') DT";
            }
            StringBuilder strSql = new StringBuilder(string.Format(sql, hfdDept.Text, hfdSearch.Text));
            if (!string.IsNullOrWhiteSpace(hfdSearch.Text))
            {
                strSql.AppendFormat(" ,(select nvl(sum(KCSL -LOCKSL),0) from DAT_GOODSSTOCK a where a.gdseq = SP.GDSEQ and a.deptid = '{0}') KCSL,floor((select nvl(sum(KCSL - LOCKSL),0) from DAT_GOODSSTOCK a where a.gdseq = SP.GDSEQ and a.deptid = '{0}')/PZ.NUM1) SL", hfdSearch.Text);
            }
            if (!string.IsNullOrWhiteSpace(hfdDept.Text))
            {
                strSql.AppendFormat(" from  DOC_GOODS SP,DOC_GOODSCFG PZ WHERE SP.FLAG IN('Y','T') AND ISDELETE='N' AND SP.GDSEQ=PZ.GDSEQ AND PZ.DSNUM > 0 AND nvl(PZ.NUM3,0) + nvl(PZ.DSPOOL,0) <= nvl(PZ.DSNUM,0) and nvl(PZ.NUM1,0) > 0 AND PZ.DEPTID='{0}' AND PZ.ISCFG IN ('1','Y') and (nvl(PZ.DSNUM, 0) - nvl(PZ.NUM3, 0) - nvl(PZ.DSPOOL, 0))>0 AND EXISTS(SELECT 1 FROM DOC_GOODSSUP GS WHERE GS.GDSEQ=SP.GDSEQ AND GS.SUPID IS NOT NULL) ", hfdDept.Text);
            }
            if (!string.IsNullOrWhiteSpace(trbSearch.Text))
            {
                strSql.AppendFormat(" AND (SP.GDSEQ LIKE '%{0}%' OR SP.GDNAME LIKE '%{0}%' OR SP.ZJM LIKE '%{0}%' OR SP.BARCODE LIKE '%{0}%')", trbSearch.Text.Trim().ToUpper());
            }
            if (!string.IsNullOrWhiteSpace(hfdSearch.Text))
            {
                strSql.AppendFormat(" AND EXISTS(SELECT 1 FROM DOC_GOODSCFG K WHERE K.DEPTID = '{0}' AND K.GDSEQ = SP.GDSEQ)", hfdSearch.Text.Trim());
            }
            strSql.Append("    ORDER BY SP.GDNAME,KCSL");
            GridGoods.DataSource = DbHelperOra.Query(strSql.ToString()).Tables[0];
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
            //hfdSearch.Text = trbSearch.Text;
            DataSearch();
        }

        protected void GridGoods_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridGoods.PageIndex = e.NewPageIndex;
            DataSearch();
        }
    }
}