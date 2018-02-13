﻿using XTBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FineUIPro;
using System.Data;


namespace ERPProject.ERPQuery
{
    public partial class StorageAcnFx : PageBase
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
            dpkDATE1.SelectedDate = DateTime.Now.AddMonths(-1);
            dpkDATE2.SelectedDate = DateTime.Now;
            PubFunc.DdlDataGet("DDL_SYS_DEPOTRANGE", UserAction.UserID, ddlDEPTID, ddlDEPTOUT);
        }

        private void DataSearch()
        {
            if (PubFunc.StrIsEmpty(dpkDATE1.SelectedDate.ToString()) || PubFunc.StrIsEmpty(dpkDATE2.SelectedDate.ToString()))
            {
                Alert.Show("输入日期不正确,请检查！");
                return;
            }
            if (dpkDATE1.SelectedDate > dpkDATE2.SelectedDate)
            {
                Alert.Show("开始日期不能大于结束日期！");
                return;
            }

            int total = 0;
            string strSql = @"select ap.*,sp.gdname,sp.gdspec,f_getunitname(sp.unit) UNITNAME,f_getproducername(sp.producer) PRODUCERNAME,sp.hsjj,sp.hsjj*dhs HSJE,DECODE(ap.flag,'N','新单','A','已提交','W','已出库','Y','已收货','已驳回') FLAGNAME
                        from (select a.flag,a.billno,b.gdseq,round(sum(decode(b.memo,'批次拆分',b.dhsl/(select count(1) from dat_db_com c where c.gdseq = b.gdseq and c.seqno= b.seqno),b.dhsl)),0) dhsl,sum(b.xssl) dhs,f_getdeptname(a.deptid) DEPTIDNAME,f_getdeptname(a.deptout) DEPTOUTNAME
                        from dat_db_doc a,dat_db_com b
                        where a.seqno = b.seqno";
            string strWhere = "";
            if (!PubFunc.StrIsEmpty(ddlDEPTID.SelectedValue)) strWhere += " and a.deptid = '" + ddlDEPTID.SelectedValue + "'";
            if (!PubFunc.StrIsEmpty(ddlDEPTOUT.SelectedValue)) strWhere += " and a.deptout = '" + ddlDEPTOUT.SelectedValue + "'";
            if (!PubFunc.StrIsEmpty(tbxBILL.Text)) strWhere += " and a.seqno like '%" + tbxBILL.Text + "%'";
            strWhere += " and a.xsrq >= TO_DATE('" + dpkDATE1.Text + "','YYYY-MM-DD') and a.xsrq < TO_DATE('" + dpkDATE2.Text + "','YYYY-MM-DD')+1";
            
            strSql = strSql +strWhere+ @" group by a.flag,a.billno,b.gdseq,a.deptid,a.deptout) ap,doc_goods sp where ap.gdseq = sp.gdseq";
            if (!PubFunc.StrIsEmpty(txbGDSEQ.Text)) strSql += " and (sp.gdseq like '%" + txbGDSEQ.Text + "%' or sp.gdname like '%" + txbGDSEQ.Text + "%' or sp.zjm like '%" + txbGDSEQ.Text + "%')";
            strSql += "  order by decode (ap.flag,'R','1','N','2','A','3','W','4','Y','5')";
            DataTable dtData = PubFunc.DbGetPage(GridGoods.PageIndex, GridGoods.PageSize, strSql, ref total);
            GridGoods.RecordCount = total;
            GridGoods.DataSource = dtData;
            GridGoods.DataBind();
        }
        protected void btSearch_Click(object sender, EventArgs e)
        {
            DataSearch();
        }
        protected void btClear_Click(object sender, EventArgs e)
        {
            PubFunc.FormDataClear(FormUser);
            dpkDATE1.SelectedDate = DateTime.Now.AddMonths(-1);
            dpkDATE2.SelectedDate = DateTime.Now;
        }
       /*protected void btExport_Click(object sender, EventArgs e)
        {
            if (GridGoods.Rows.Count<1 )
            {
                Alert.Show("没有数据,无法导出！");
                return;
            }
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=商品进销存汇总.xls");
            Response.ContentType = "application/excel";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.Write(PubFunc.GridToHtml(GridGoods));
            Response.End();
            btnExp.Enabled = true;
        }*/
        protected void btExport_Click(object sender, EventArgs e)
        {
            if (GridGoods.Rows.Count < 1)
            {
                Alert.Show("没有数据,无法导出！");
                return;
            }
            string strSql = @"select ap.billno 调拨单号,DECODE(ap.flag,'N', '新单', 'A', '已提交', 'W', '已出库', 'Y', '已收货', '已驳回') 商品状态,
       ' '||ap.gdseq 商品编码, sp.gdname 商品名称, sp.gdspec 规格容量,f_getunitname(sp.unit) 单位, f_getproducername(sp.producer) 生产厂家,
       ap.DEPTIDNAME 申请库房,ap.DEPTOUTNAME 出库库房, ap.dhsl 申请数, ap.dhs 调拨数,sp.hsjj 含税进价, sp.hsjj * dhs 含税金额
       from (select a.flag, a.billno,b.gdseq, round(sum(decode(b.memo, '批次拆分', b.dhsl / (select count(1) from dat_db_com c where c.gdseq = b.gdseq and c.seqno = b.seqno),b.dhsl)), 0) dhsl,
               sum(b.xssl) dhs, f_getdeptname(a.deptid) DEPTIDNAME, f_getdeptname(a.deptout) DEPTOUTNAME
                      from dat_db_doc a, dat_db_com b where a.seqno = b.seqno";
            string strWhere = "";
            if (!PubFunc.StrIsEmpty(ddlDEPTID.SelectedValue)) strWhere += " and a.deptid = '" + ddlDEPTID.SelectedValue + "'";
            if (!PubFunc.StrIsEmpty(ddlDEPTOUT.SelectedValue)) strWhere += " and a.deptout = '" + ddlDEPTOUT.SelectedValue + "'";
            if (!PubFunc.StrIsEmpty(tbxBILL.Text)) strWhere += " and a.seqno like '%" + tbxBILL.Text + "%'";
            strWhere += " and a.xsrq >= TO_DATE('" + dpkDATE1.Text + "','YYYY-MM-DD') and a.xsrq < TO_DATE('" + dpkDATE2.Text + "','YYYY-MM-DD')+1";

            strSql = strSql + strWhere + @" group by a.flag,a.billno,b.gdseq,a.deptid,a.deptout) ap,doc_goods sp where ap.gdseq = sp.gdseq";
            if (!PubFunc.StrIsEmpty(txbGDSEQ.Text)) strSql += " and (sp.gdseq like '%" + txbGDSEQ.Text + "%' or sp.gdname like '%" + txbGDSEQ.Text + "%' or sp.zjm like '%" + txbGDSEQ.Text + "%')";
            strSql += "  order by decode (ap.flag,'R','1','N','2','A','3','W','4','Y','5')";
            DataTable dt = DbHelperOra.Query(strSql).Tables[0];
            XTBase.Utilities.ExcelHelper.ExportByWeb(dt, "商品调拨信息", string.Format("商品调拨信息_{0}.xls", DateTime.Now.ToString("yyyyMMdd")));
        }

        protected void GridGoods_Sort(object sender, GridSortEventArgs e)
        {
            GridGoods.SortDirection = e.SortDirection;
            GridGoods.SortField = e.SortField;

            DataTable table = PubFunc.GridDataGet(GridGoods);
            DataView view1 = table.DefaultView;
            view1.Sort = String.Format("{0} {1}", GridGoods.SortField, GridGoods.SortDirection);
            GridGoods.DataSource = view1;
            GridGoods.DataBind();
        }
        protected void GridCom_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridGoods.PageIndex = e.NewPageIndex;
            DataSearch();
        }

        protected void GridList_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                string flag = row["FLAGNAME"].ToString();
                FineUIPro.BoundField flagcol = GridGoods.FindColumn("FLAGNAME") as FineUIPro.BoundField;
                if (flag == "新单")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color1";
                }
                if (flag == "已提交")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color2";
                }
                if (flag == "已驳回")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color3";
                }
            }
        }
    }
}