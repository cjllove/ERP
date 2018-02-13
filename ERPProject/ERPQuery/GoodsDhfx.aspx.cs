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
    public partial class GoodsDhfx : BillBase
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
            lisDATE1.SelectedDate = DateTime.Now.AddMonths(-1);
            lisDATE2.SelectedDate = DateTime.Now;
            DatePicker1.SelectedDate = DateTime.Now.AddMonths(-1);
            DatePicker2.SelectedDate = DateTime.Now;
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

            //int total = 0;
            string strSql = @"select sp.gdseq,
       F_GETHISINFO(sp.gdseq, 'GDNAME') GDNAME,
       f_getsupname(sj.supplier) supName,
       F_GETUNITNAME(sj.unit) unitname,
       F_GETHISINFO(sp.gdseq, 'gdspec') GDSPEC,
       F_GETPRODUCERNAME(sj.producer) producername,
       sp.dhs,
       nvl(sq.rks, 0) RKS,
       (sp.dhs - nvl(sq.rks, 0)) CYS,
       (sj.HSJJ*sp.dhs) dhje,
       nvl(sj.HSJJ,0)*nvl(sq.rks,0) RKJE,
       (nvl(sj.HSJJ,0)*nvl(sp.dhs,0)) - nvl(sj.HSJJ,0)*nvl(sq.rks,0) CYJE,
       decode(nvl(sp.dhs, 0), 0, 0, round(nvl(sq.rks, 0) / sp.dhs, 4)) ZB,
       (case sj.ISFLAG3
         when 'Y' then
          '直送商品'
         when 'N' then
          '库存品'
         else
          '维护信息'
       end) ISFLAG3,
       sj.hsjj,
decode(sj.ISGZ,'Y','是','否') ISGZ
  from (select b.gdseq, nvl(sum(b.dhs), 0) dhs, nvl(sum(b.hsje), 0) dhje
          from dat_dd_doc a, dat_dd_com b
         where a.seqno = b.seqno
           and a.flag in ('Y', 'G')
           and a.shrq between to_date('{0}', 'yyyy-mm-dd') and
               to_date('{1}', 'yyyy-mm-dd')+1
         group by b.gdseq) sp,
       (select b.gdseq, nvl(sum(b.sssl), 0) rks, nvl(sum(b.hsje), 0) rkje
          from dat_rk_doc a, dat_rk_com b
         where a.seqno = b.seqno
           and a.flag in ('Y', 'G')
           and a.shrq between to_date('{0}', 'yyyy-mm-dd') and
               to_date('{1}', 'yyyy-mm-dd')+1
         group by b.gdseq) sq,
       doc_goods sj
 where sp.gdseq = sq.gdseq(+)
   and sp.gdseq = sj.gdseq";
            string strWhere = "";
            if (!PubFunc.StrIsEmpty(ddlISGZ.SelectedValue)) strWhere += " and sj.ISGZ = '" + ddlISGZ.SelectedValue + "'";
            if (!PubFunc.StrIsEmpty(txbGDSEQ.Text)) strWhere += " and (sp.gdseq like '%" + txbGDSEQ.Text + "%' or sj.gdname like '%" + txbGDSEQ.Text + "%' or sj.zjm like '%" + txbGDSEQ.Text + "%')";
            if (strWhere.Trim().Length > 0) strSql = strSql + strWhere;
            strSql += " order by ZB";
            //DataTable dtData = PubFunc.DbGetPage(GridGoods.PageIndex, GridGoods.PageSize, string.Format(strSql, dpkDATE1.Text, dpkDATE2.Text), ref total);
            //GridGoods.RecordCount = total;
            //GridGoods.DataSource = dtData;
            GridGoods.DataSource = DbHelperOra.QueryForTable(string.Format(strSql, dpkDATE1.Text, dpkDATE2.Text));
            GridGoods.DataBind();
        }
        protected void btnSch_Click(object sender, EventArgs e)
        {
            if (PubFunc.StrIsEmpty(lisDATE1.SelectedDate.ToString()) || PubFunc.StrIsEmpty(lisDATE2.SelectedDate.ToString()))
            {
                Alert.Show("输入日期不正确,请检查！");
                return;
            }
            if (lisDATE1.SelectedDate > lisDATE2.SelectedDate)
            {
                Alert.Show("开始日期不能大于结束日期！");
                return;
            }
            string strSql = @"SELECT A.SEQNO,DECODE(nvl(B.SEQNO,'#'),'#','未入库','已入库') FLAGNAME,F_GETDEPTNAME(A.DEPTDH) DEPTDHNAME,F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,A.SUBNUM NUM_DH,F_GETUSERNAME(A.LRY) LRYNAME,A.SHRQ,B.SEQNO SEQNO_RK,B.NUM_RK,B.SHRQ_RK,F_GETUSERNAME(B.SHR) SHRNAME
                FROM DAT_DD_DOC A,(SELECT SEQNO,DDBH,SUM(SUBNUM) NUM_RK,MAX(SHRQ) SHRQ_RK,MAX(SHR) SHR FROM DAT_RK_DOC WHERE FLAG IN ('Y','G') GROUP BY SEQNO,DDBH) B
                WHERE A.SEQNO = B.DDBH(+) AND A.FLAG IN ('Y','G') AND A.SHRQ BETWEEN TO_DATE('{0}','yyyy-mm-dd') AND TO_DATE('{1}','yyyy-mm-dd')";
            string strWhere = "";
            if (!PubFunc.StrIsEmpty(lisBILL.Text.Trim())) strWhere += " AND A.SEQNO LIKE '%" + lisBILL.Text.Trim() + "%'";
            if (!PubFunc.StrIsEmpty(ddlFLAG.SelectedValue.Trim()))
            {
                if (ddlFLAG.SelectedValue == "N") strWhere += " AND NVL(B.SEQNO,'#') = '#'";
                if (ddlFLAG.SelectedValue == "Y") strWhere += " AND NVL(B.SEQNO,'#') <>'#'";
            }
            if (strWhere.Trim().Length > 0) strSql = strSql + strWhere;
            strSql += " order by SHRQ DESC";
            GridList.DataSource = DbHelperOra.QueryForTable(string.Format(strSql, dpkDATE1.Text, dpkDATE2.Text));
            GridList.DataBind();
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
            if (TabStrip1.ActiveTabIndex == 0)
            {
                txbGDSEQ.Text = string.Empty;
                dpkDATE1.SelectedDate = DateTime.Now.AddMonths(-1);
                dpkDATE2.SelectedDate = DateTime.Now;
            }
            if (TabStrip1.ActiveTabIndex == 1)
            {
                lisBILL.Text = string.Empty;
                lisDATE1.SelectedDate = DateTime.Now.AddMonths(-1);
                lisDATE2.SelectedDate = DateTime.Now;
            }
            if (TabStrip1.ActiveTabIndex == 2)
            {
                TextBox1.Text = string.Empty;
                DropDownList1.SelectedIndex = 0;
                DatePicker1.SelectedDate = DateTime.Now.AddMonths(-1);
                DatePicker2.SelectedDate = DateTime.Now;
            }
        }
        protected void btExport_Click(object sender, EventArgs e)
        {
            if (TabStrip1.ActiveTabIndex == 0)
            {
                if (GridGoods.Rows.Count < 1)
                {
                    Alert.Show("没有数据,无法导出！");
                    return;
                }
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment; filename=单品到货分析.xls");
                Response.ContentType = "application/excel";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.Write(PubFunc.GridToHtml(GridGoods));
                Response.End();
                btnExp.Enabled = true;
            }
            if (TabStrip1.ActiveTabIndex == 1)
            {
                if (GridList.Rows.Count < 1)
                {
                    Alert.Show("没有数据,无法导出！");
                    return;
                }
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment; filename=单据到货分析.xls");
                Response.ContentType = "application/excel";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.Write(PubFunc.GridToHtml(GridList));
                Response.End();
                btnExp.Enabled = true;
            }
        }
        protected void btnSch3_Click(object sender, EventArgs e)
        {
            if (PubFunc.StrIsEmpty(DatePicker1.SelectedDate.ToString()) || PubFunc.StrIsEmpty(DatePicker2.SelectedDate.ToString()))
            {
                Alert.Show("输入日期不正确,请检查！");
                return;
            }
            if (DatePicker1.SelectedDate > DatePicker2.SelectedDate)
            {
                Alert.Show("开始日期不能大于结束日期！");
                return;
            }
            string strSql = @"select A.SEQNO,
                                       (CASE WHEN  A.SEQNO IN (select DISTINCT (seqno) from DAT_DD_COM t where dhs > dhsl) THEN '未完成' 
                                       else '已完成' END) FLAGNAME,
                                       A.DEPTDH,
                                       f_getdeptname(A.DEPTDH) DEPTDHNAME,
                                       A.DEPTID,
                                       f_getdeptname(A.DEPTID) DEPTIDNAME,
                                       A.LRRQ,
                                       A.LRY,
                                       f_getusername(A.LRY) LRYNAME
                                  from dat_dd_doc A";
            string strWhere = " where 1=1 ";
            if (!PubFunc.StrIsEmpty(TextBox1.Text.Trim())) strWhere += " AND A.SEQNO LIKE '%" + lisBILL.Text.Trim() + "%'";
            if (!PubFunc.StrIsEmpty(DropDownList1.SelectedValue.Trim()))
            {
                if (DropDownList1.SelectedValue == "N") strWhere += " AND (CASE WHEN  A.SEQNO IN (select DISTINCT (seqno) from DAT_DD_COM t where dhs > dhsl) THEN '未完成' else '已完成' END) = '未完成'";
                if (DropDownList1.SelectedValue == "Y") strWhere += " AND (CASE WHEN  A.SEQNO IN (select DISTINCT (seqno) from DAT_DD_COM t where dhs > dhsl) THEN '未完成' else '已完成' END) = '已完成'";
            }
            strWhere += " AND A.LRRQ>=TO_DATE('{0}','YYYY-MM-DD')";
            strWhere += " AND A.LRRQ <TO_DATE('{1}','YYYY-MM-DD') + 1";
            strSql += strWhere + " order by LRRQ DESC";
            Grid1.DataSource = DbHelperOra.QueryForTable(string.Format(strSql, DatePicker1.Text, DatePicker2.Text));
            Grid1.DataBind();
        }
        protected void Grid1_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                string flag = row["FLAGNAME"].ToString();
                FineUIPro.BoundField flagcol = Grid1.FindColumn("FLAGNAME") as FineUIPro.BoundField;
                if (flag == "未完成")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color3";
                }
            }
        }
        protected  void Grid1_DoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            string strBillno = Grid1.Rows[e.RowIndex].Values[1].ToString();
            billOpen(strBillno);
            WindowGoodsDD.Hidden = false;
        }
        protected  void billOpen(string strBillno)
        {
            string Sql = @"SELECT A.*,
                                               F_GETUNITNAME(A.UNIT) UNITNAME,
                                               F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME,
                                               F_GETSUPNAME(A.SUPID) SUPNAME,
                                               F_GETUNITNAME(B.UNIT) UNITSMALLNAME
                                          FROM DAT_DD_COM A, DOC_GOODS B
                                         WHERE A.SEQNO = '{0}'
                                           AND A.GDSEQ = B.GDSEQ";
            DataTable dtDoc = DbHelperOra.Query(string.Format(Sql, strBillno)).Tables[0];
            DDlist.DataSource = dtDoc;
            DDlist.DataBind();
        }
    }
}