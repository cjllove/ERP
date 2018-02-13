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

namespace ERPProject.ERPQuery
{
    public partial class GZJXC : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dpkout1.SelectedDate = DateTime.Now.AddDays(-1);
                dpkout2.SelectedDate = DateTime.Now;
                DepartmentBind.BindDDL("DDL_SYS_DEPTHASATH", UserAction.UserID, lstDEPTID);
                lstDEPTID.SelectedIndex = 1;
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
        public DataTable GetGoodsList(int pageNum, int pageSize, NameValueCollection nvc, ref int total, ref string errMsg)
        {
            string strSearch = "";
            if (tbxGDSEQ.Text.Trim().Length > 0) { strSearch += string.Format(" AND (B.GDSEQ LIKE '%{0}%' OR B.GDNAME LIKE '%{0}%' OR B.ZJM LIKE '%{0}%' OR B.BARCODE LIKE '%{0}%')", tbxGDSEQ.Text.Trim()); }
            if (tbxONECODE.Text.Trim().Length > 0) { strSearch += " AND A.ONECODE like '%" + tbxONECODE.Text + "%'"; }
            if (lstDEPTID.SelectedValue.Length > 0) { strSearch += " AND A.DEPTID ='" + lstDEPTID.SelectedValue + "'"; }
            if (dpkout1.SelectedDate != null) { strSearch += string.Format(" AND A.CREATETIME>=TO_DATE('{0}','YYYY-MM-DD')", dpkout1.Text); }
            if (dpkout2.SelectedDate != null) { strSearch += string.Format(" AND A.CREATETIME<TO_DATE('{0}','YYYY-MM-DD')+1", dpkout2.Text); }
            strSearch += " ORDER BY A.SEQNO DESC";
            string strGoods = @" SELECT A.*,B.GDNAME,B.GDSPEC,f_getunitname(B.UNIT) UNITNAME,f_getproducername(B.PRODUCER) PRODUCERNAME,B.HSJJ HSJJ_GOD,f_getdeptname(A.DEPTID) DEPTNAME,
                DECODE(A.OPERTYEP,'1','增库存','-1','减库存','0','订货','其他操作') OPERTYEPNAME
                FROM DAT_ONECODEJXC A,DOC_GOODS B
                WHERE A.GDSEQ = B.GDSEQ AND a.deptid in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '" + UserAction.UserID + "') = 'Y' )";
            StringBuilder strSql = new StringBuilder(strGoods);
            strSql.Append(strSearch);

            DataTable dt = new DataTable();
            dt = GetDataTable(pageNum, pageSize, strSql, ref total);
            return dt;
        }
    }
}