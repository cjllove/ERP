using FineUIPro;
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

namespace ERPProject.ERPQuery
{
    public partial class TraceBack : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //PubFunc.DdlDataGet(ddlDEPTINT, "DDL_SYS_DEPTNULL");
                //DepartmentBind.BindDDL("DDL_SYS_DEPTRANGE", UserAction.UserID, ddlDEPTINT);
                dpkout1.SelectedDate = DateTime.Now.AddDays(-30);
                dpkout2.SelectedDate = DateTime.Now;
                //PubFunc.DdlDataGet("DDL_SYS_DEPTRANGE", UserAction.UserID, lstDEPTID);
                DepartmentBind.BindDDL("DDL_SYS_DEPTRANGE", UserAction.UserID, lstDEPTID);
            }
        }
        private void DataSearch()
        {
            int total = 0;
            String Sql = @"SELECT A.CUSTID,A.OPTID,A.DOCTOR,A.STR9,A.STR7,A.STR8,A.OPTTABLE,A.DEPTID,f_getdeptname(A.DEPTID) DEPTNAME,COUNT(1) SL,A.SUBSUM JE,A.NUM4
                            FROM DAT_XS_DOC A,DAT_XS_EXT C,DOC_GOODS D
                            WHERE A.SEQNO = C.BILLNO AND C.GDSEQ = D.GDSEQ
                            AND NVL(A.CUSTID,'#') <> '#' AND A.FLAG <> 'M'";
            string strSearch = "";
            if (trbGDSEQ.Text.Trim().Length > 0) { strSearch += string.Format(" AND (D.GDSEQ LIKE '%{0}%' OR D.GDNAME LIKE '%{0}%' OR D.ZJM LIKE '%{0}%' OR D.BARCODE LIKE '%{0}%')", trbGDSEQ.Text.Trim().ToUpper()); }
            if (tbxPatient.Text.Trim().Length > 0) { strSearch += string.Format(" AND (A.CUSTID LIKE '%{0}%' OR A.OPTID LIKE '%{0}%' OR A.DOCTOR LIKE '%{0}%' OR A.STR9 LIKE '%{0}%' OR A.STR7 LIKE '%{0}%' OR A.STR8 LIKE '%{0}%')", tbxPatient.Text.Trim()); }
            if (lstDEPTID.SelectedValue.Length > 0) { strSearch += " AND A.DEPTID ='" + lstDEPTID.SelectedValue + "'"; }
            if (tbxONECODE.Text.Trim().Length > 0) { strSearch += " AND C.ONECODE ='" + tbxONECODE.Text + "'"; }
            if (dpkout1.SelectedDate != null) { strSearch += string.Format(" AND A.SHRQ >= TO_DATE('{0}','YYYY-MM-DD')", dpkout1.Text); }
            if (dpkout2.SelectedDate != null) { strSearch += string.Format(" AND A.SHRQ < TO_DATE('{0}','YYYY-MM-DD')+1", dpkout2.Text); }
            strSearch += " GROUP BY A.CUSTID,A.OPTID,A.DOCTOR,A.STR9,A.STR7,A.STR8,A.OPTTABLE,A.DEPTID,A.SUBSUM,A.NUM4";
            DataTable dtData = PubFunc.DbGetPage(GridGoods.PageIndex, GridGoods.PageSize, Sql + strSearch, ref total);
            GridGoods.RecordCount = total;
            GridGoods.DataSource = dtData;
            GridGoods.DataBind();
        }

        protected void btSearch_Click(object sender, EventArgs e)
        {
            DataSearch();
        }

        protected void GridGoods_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridGoods.PageIndex = e.NewPageIndex;
            DataSearch();
        }

        protected void GridGoods_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            dataBind(GridGoods.DataKeys[e.RowIndex][1].ToString(), GridGoods.DataKeys[e.RowIndex][0].ToString());
            TspPage.ActiveTabIndex = 0;
            winXq.Hidden = false;
        }
        protected void dataBind(String custid, String deptid)
        {
            String Sql = @"SELECT C.ONECODE,C.PH,C.YXQZ,C.RQ_SC,D.GDSEQ,D.GDNAME,D.GDSPEC,f_getunitname(D.UNIT) UNITNAME,
                            D.HSJJ,D.PIZNO,f_getproducername(D.PRODUCER) PRODUCERNAME,A.SHRQ,f_getusername(A.SHR) USERNAME
                            FROM DAT_XS_DOC A,DAT_XS_EXT C,DOC_GOODS D
                            WHERE A.SEQNO = C.BILLNO AND C.GDSEQ = D.GDSEQ
                            AND A.CUSTID = '{0}' AND A.DEPTID = '{1}'";
            GridXq.DataSource = DbHelperOra.Query(String.Format(Sql, custid, deptid)).Tables[0];
            GridXq.DataBind();
        }
        protected void btnSureDate_Click(object sender, EventArgs e)
        {
            winXq.Hidden = true;
        }

        protected void GridXq_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            dataBind2(GridXq.DataKeys[e.RowIndex][0].ToString());
        }
        protected void dataBind2(String onecode)
        {
            winXq.Hidden = false;
            tabFrm.IFrameUrl = "TraceBackFm.aspx?onecode=" + onecode;
            TspPage.ActiveTabIndex = 1;
            PageContext.RegisterStartupScript("reFrame('" + onecode + "');");
        }

        protected void tbxONECODE_TriggerClick(object sender, EventArgs e)
        {
            if (!DbHelperOra.Exists("SELECT 1 FROM DAT_GZ_EXT WHERE ONECODE = '" + tbxONECODE.Text + "'"))
            {
                Alert.Show("高值【" + tbxONECODE.Text + "】不存在！", MessageBoxIcon.Warning);
                tbxONECODE.Text = "";
                return;
            }
            dataBind2(tbxONECODE.Text);
        }
    }

}