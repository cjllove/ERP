using XTBase;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FineUIPro;

namespace ERPProject.ERPApply
{
    public partial class ConstantAgainPrint : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //PubFunc.DdlDataGet(ddlDEPTINT, "DDL_SYS_DEPTNULL");
                DepartmentBind.BindDDL("DDL_SYS_DEPTRANGE", UserAction.UserID, ddlDEPTINT);
                dpkout1.SelectedDate = DateTime.Now.AddDays(-7);
                dpkout2.SelectedDate = DateTime.Now;
            }
        }
        private void DataSearch()
        {
            int total = 0;
            string msg = "";
            NameValueCollection nvc = new NameValueCollection();

            DataTable dtData = GetGoodsList(GridGoods.PageIndex, GridGoods.PageSize, nvc, ref total, ref msg);
            GridGoods.RecordCount = total;
            GridGoods.DataSource = dtData;
            GridGoods.DataBind();
        }

        protected void btSearch_Click(object sender, EventArgs e)
        {
            DataSearch();
        }

        protected void GridGoods_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridGoods.PageIndex = e.NewPageIndex;
            DataSearch();
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
        public DataTable GetGoodsList(int pageNum, int pageSize, NameValueCollection nvc, ref int total, ref string errMsg)
        {
            string strSearch = "";
            if (tbxGDSEQ.Text.Trim().Length > 0) { strSearch += " and ( a.GDSEQ like '%" + tbxGDSEQ.Text + "%' or b.gdname like '%" + tbxGDSEQ.Text + "%'  )"; }
            if (ddlDEPTINT.SelectedValue.Length > 0) { strSearch += " and a.DEPTIN ='" + ddlDEPTINT.SelectedValue.ToString() + "'"; }
            if (tbxBILL.Text.Trim().Length > 0) { strSearch += " and a.OUTBILLNO='" + tbxBILL.Text + "'"; }
            if (dpkout1.SelectedDate != null) { strSearch += string.Format(" AND a.OUTRQ>=TO_DATE('{0}','YYYY-MM-DD')", dpkout1.Text); }
            if (dpkout2.SelectedDate != null) { strSearch += string.Format(" AND a.OUTRQ<TO_DATE('{0}','YYYY-MM-DD')+1", dpkout2.Text); }
            strSearch += string.Format(" AND deptin in( select code FROM SYS_DEPT where type <>'1' and  F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            strSearch += " order by a.OUTRQ desc,a.OUTBILLNO desc,b.gdname";
            string strGoods = @"select a.GDSEQ,b.gdname,b.gdspec,F_GETUNITNAME(b.unit) unitname,F_GETPRODUCERNAME(b.producer) producername,F_GETDEPTNAME(a.DEPTOUT) DEPTOUT,F_GETDEPTNAME(a.DEPTIN) DEPTIN,a.DSHL,a.SL,a.BARCODE,a.OUTRQ,a.OUTBILLNO  from DAT_GOODSDS_LOG a,doc_goods b  WHERE a.gdseq = b.gdseq and a.FLAG = 'N'";
            StringBuilder strSql = new StringBuilder(strGoods);
            strSql.Append(strSearch);
            return GetDataTable(pageNum, pageSize, strSql, ref total);
        }

        protected void GridGoods_RowSelect(object sender, GridRowSelectEventArgs e)
        {
            //int[] selections = GridGoods.SelectedRowIndexArray;
            //string no = "";
            //echo.Text = "";
            //foreach (int rowIndex in selections)
            //{
            //    no = no + "'" + GridGoods.DataKeys[rowIndex][0].ToString() + "',"; 
            //}
            //echo.Text = no.TrimEnd(',');
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            int[] selections = GridGoods.SelectedRowIndexArray;
            string no = "";
            echo.Text = "";
            foreach (int rowIndex in selections)
            {
                no = no + "'" + GridGoods.DataKeys[rowIndex][0].ToString() + "',";
            }
            echo.Text = no.TrimEnd(',');

            PageContext.RegisterStartupScript("btnPrint_onclick()");
        }

        protected void btnExp_Click(object sender, EventArgs e)
        {
            string strSearch = "";
            if (tbxGDSEQ.Text.Trim().Length > 0) { strSearch += " and ( a.GDSEQ like '%" + tbxGDSEQ.Text + "%' or b.gdname like '%" + tbxGDSEQ.Text + "%'  )"; }
            if (ddlDEPTINT.SelectedValue.Length > 0) { strSearch += " and a.DEPTIN ='" + ddlDEPTINT.SelectedValue.ToString() + "'"; }
            if (tbxBILL.Text.Trim().Length > 0) { strSearch += " and a.OUTBILLNO='" + tbxBILL.Text + "'"; }
            if (dpkout1.SelectedDate != null) { strSearch += string.Format(" AND a.OUTRQ>=TO_DATE('{0}','YYYY-MM-DD')", dpkout1.Text); }
            if (dpkout2.SelectedDate != null) { strSearch += string.Format(" AND a.OUTRQ<TO_DATE('{0}','YYYY-MM-DD')+1", dpkout2.Text); }
            strSearch += string.Format(" AND deptin in( select code FROM SYS_DEPT where type <>'1' and  F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            strSearch += " order by a.OUTRQ desc,a.OUTBILLNO desc,b.gdname";
            string strGoods = @"SELECT A.GDSEQ 商品编码,B.GDNAME 商品名称,B.GDSPEC 商品规格,F_GETUNITNAME(B.UNIT) 单位,F_GETPRODUCERNAME(B.PRODUCER) 生产厂家,F_GETDEPTNAME(A.DEPTOUT) 出库部门,F_GETDEPTNAME(A.DEPTIN) 科室名称,A.DSHL 定数量,A.BARCODE||' ' 定数条码,A.OUTRQ 出库日期,A.OUTBILLNO 单据号 FROM DAT_GOODSDS_LOG A,DOC_GOODS B  WHERE A.GDSEQ = B.GDSEQ AND A.FLAG = 'N'";
            StringBuilder strSql = new StringBuilder(strGoods);
            strSql.Append(strSearch);
            DataTable dt = DbHelperOra.Query(strSql.ToString()).Tables[0];
            if (dt.Rows.Count < 1)
            {
                Alert.Show("请查询需要导出的数据！", MessageBoxIcon.Warning);
                return;
            }
            XTBase.Utilities.ExcelHelper.ExportByWeb(dt, "科室申领信息", string.Format("科室申领信息_{0}.xls", DateTime.Now.ToString("yyyyMMdd")));
        }
    }
}