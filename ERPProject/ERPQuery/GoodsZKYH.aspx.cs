using XTBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections.Specialized;
using FineUIPro;

namespace ERPProject.ERPQuery
{
    public partial class GoodsZKYH : PageBase
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
            PubFunc.DdlDataGet("DDL_SYS_DEPOT", ddlDEPTID);
            PubFunc.DdlDataGet("DDL_USER",  lstLRY);
            string strSql1 = @"SELECT CODE,NAME FROM (SELECT '' CODE ,'--请选择--' NAME  FROM dual
                                            union all
                                            SELECT '0' CODE ,'正常' NAME  FROM dual
                                            union all
                                            SELECT '1' CODE ,'破损' NAME  FROM dual
                                            union all
                                            SELECT '2' CODE ,'失效' NAME  FROM dual
                                            union all
                                            SELECT '3' CODE ,'发霉' NAME  FROM dual)";
            PubFunc.DdlDataSql(ddlYHTYPE, strSql1);
            dpkDATE1.SelectedDate = DateTime.Now.AddDays(-1);
            dpkDATE2.SelectedDate = DateTime.Now;
        }


        protected void btnExport_Click(object sender, EventArgs e)
        {
            //DataTable dtData = DbHelperOra.Query(GetSearchSql()).Tables[0];
            //if (dtData == null || dtData.Rows.Count == 0)
            //{
            //    Alert.Show("没有数据,无法导出！");
            //    return;
            //}
            //string[] columnNames = new string[GridGoods.Columns.Count - 1];
            //for (int index = 1; index < GridGoods.Columns.Count; index++)
            //{
            //    GridColumn column = GridGoods.Columns[index];
            //    if (column is FineUIPro.BoundField)
            //    {
            //        dtData.Columns[((FineUIPro.BoundField)(column)).DataField.ToUpper()].ColumnName = column.HeaderText;
            //        columnNames[index - 1] = column.HeaderText;
            //    }
            //}
            //ExcelHelper.ExportByWeb(dtData.DefaultView.ToTable(true, columnNames), "在库养护信息", string.Format("在库养护信息报表_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));

            if (GridGoods.Rows.Count < 1)
            {
                Alert.Show("没有数据,无法导出！");
                return;
            }
            string strSql = @"SELECT 
  B.SEQNO 养护单号,
  decode(B.YHTYPE,'0','正常','1','破损','2','失效','3','发霉','未定义') 养护标准,
  B.REASON 原因说明,
  A.YHY 养护人,
  f_getdeptname(A.DEPTID) 养护仓库,
  A.YHRQ 养护日期, 
  A.LRY 录入员, 
  A.LRRQ 录入日期, 
  A.SHR 审核人, 
  A.SHRQ 审核日期,
  B.GDSEQ 商品编码,
  B.GDNAME 商品名称,
  B.GDSPEC 商品规格, 
  f_getunitname(B.UNIT) 单位, 
  decode(B.ISGZ,'N','否','Y','是','未定义') 是否贵重,
  B.ISLOT 批号管理,
  B.HSJJ 含税进价,
  B.KCSL 库存数量,
  B.HSJE 含税金额,
  B.PHID 批号,
  B.YXQZ 有效期至,
  B.RQ_SC 生产日期,
  B.BZHL 包装含量,
  f_getproducername(B.PRODUCER) 生产厂家,
  B.ZPBH 制品编号,
  B.PICINO 批次编号 
FROM DAT_YH_DOC A,DAT_YH_COM B
WHERE A.SEQNO = B.SEQNO AND A.FLAG = 'Y'";
            string strSearch = "";
            if (!PubFunc.StrIsEmpty(tbxGDSEQ.Text.Trim()))
            {
                strSearch += " AND b.GDSEQ like '%" + tbxGDSEQ.Text.Trim() + "%'";
            }

            if (!PubFunc.StrIsEmpty(tbxBILLNO.Text.Trim()))
            {
                strSearch += " AND A.SEQNO LIKE '%" + tbxBILLNO.Text.Trim().ToUpper() + "%'";
            }
            if (!string.IsNullOrWhiteSpace(dpkDATE1.Text))
            {
                strSearch += string.Format(" AND TRUNC(A.YHRQ) >= TO_DATE('{0}','YYYY-MM-DD')", dpkDATE1.Text);
            }
            if (!string.IsNullOrWhiteSpace(dpkDATE2.Text))
            {
                strSearch += string.Format(" AND TRUNC(A.YHRQ) <= TO_DATE('{0}','YYYY-MM-DD')", dpkDATE2.Text);
            }
            if (lstLRY.SelectedItem != null && lstLRY.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.LRY='{0}'", lstLRY.SelectedItem.Value);
            }
            strSql += strSearch;

            strSql += " ORDER BY  A.BILLNO DESC  ";
            DataTable dt = DbHelperOra.Query(strSql).Tables[0];
            //if (GridShelf.DataSource == null)
            //{
            //    Alert.Show("没有数据,无法导出！");
            //    return;
            //}
            XTBase.Utilities.ExcelHelper.ExportByWeb(dt, "在库养护信息", "在库养护信息报表_" + DateTime.Now.ToString("yyyyMMddHH") + ".xls");
        }

        protected void btSearch_Click(object sender, EventArgs e)
        {
            DataSearch();
        }

        private void DataSearch()
        {
            int total = 0;

            DataTable dtData = PubFunc.DbGetPage(GridGoods.PageIndex, GridGoods.PageSize, GetSearchSql(), ref total);
            if (dtData.Rows.Count == 0)
            {
                Alert.Show("当前条件无数据，请重新查询。");
                return;
            }
            GridGoods.RecordCount = total;
            GridGoods.DataSource = dtData;
            GridGoods.DataBind();
        }

        private string GetSearchSql()
        {
            string sSQL = @"SELECT B.*,C.GDID, decode(B.YHTYPE,'0','正常','1','破损','2','失效','3','发霉','未定义') YHTYPENAME,f_getusername(A.YHY) YHY, f_getdeptname(A.DEPTID) DEPTNAME,A.YHRQ, f_getusername(A.LRY) LRY, A.LRRQ, f_getusername(A.SHR) SHR, A.SHRQ, f_getunitname(B.UNIT) UNITNAME, decode(B.ISGZ,'N','否','Y','是','未定义') ISGZNAME,  f_getproducername(B.PRODUCER) PRODUCERNAME
FROM DAT_YH_DOC A,DAT_YH_COM B,DOC_GOODS C
WHERE A.SEQNO = B.SEQNO AND B.GDSEQ=C.GDSEQ AND A.FLAG = 'Y'";

            if (!PubFunc.StrIsEmpty(ddlISGZ.SelectedValue)) sSQL += " and C.ISGZ = '" + ddlISGZ.SelectedValue + "'";
            if (!PubFunc.StrIsEmpty(tbxGDSEQ.Text.Trim()))
            {
                sSQL += " AND b.GDSEQ like '%" + tbxGDSEQ.Text.Trim() + "%'";
            }

            if (!PubFunc.StrIsEmpty(tbxBILLNO.Text.Trim()))
            {
                sSQL += " AND A.SEQNO LIKE '%" + tbxBILLNO.Text.Trim().ToUpper() + "%'";
            }
            if (!string.IsNullOrWhiteSpace(dpkDATE1.Text))
            {
                sSQL += string.Format(" AND TRUNC(A.YHRQ) >= TO_DATE('{0}','YYYY-MM-DD')", dpkDATE1.Text);
            }
            if (!string.IsNullOrWhiteSpace(dpkDATE2.Text))
            {
                sSQL += string.Format(" AND TRUNC(A.YHRQ) <= TO_DATE('{0}','YYYY-MM-DD')", dpkDATE2.Text);
            }
            if (!string.IsNullOrWhiteSpace(ddlYHTYPE.SelectedValue))
            {
                sSQL += string.Format(" AND B.YHTYPE = {0}", ddlYHTYPE.SelectedValue);
            }
            if (!string.IsNullOrWhiteSpace(ddlDEPTID.SelectedValue))
            {
                sSQL += string.Format(" AND A.DEPTID = {0}", ddlDEPTID.SelectedValue);
            }

            sSQL += " ORDER BY  A.BILLNO DESC  ";
            return sSQL;
        }


        protected void GridGoods_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridGoods.PageIndex = e.NewPageIndex;
            DataSearch();
        }

        protected void GridGoods_Sort(object sender, FineUIPro.GridSortEventArgs e)
        {
            GridGoods.SortDirection = e.SortDirection;
            GridGoods.SortField = e.SortField;

            DataSearch();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            PubFunc.FormDataClear(FormUser);
            BindDDL();
            GridGoods.DataSource = null;
            GridGoods.DataBind();
        }

    }
}