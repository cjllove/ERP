using XTBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FineUIPro;
using System.Data;
using Newtonsoft.Json.Linq;


namespace ERPProject.ERPQuery
{
    public partial class BorrowSearch : PageBase
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
            dpkDATE1.SelectedDate = DateTime.Now.AddDays(-1);
            dpkDATE2.SelectedDate = DateTime.Now;
            PubFunc.DdlDataGet("DDL_SYS_DEPTRANGE", UserAction.UserID, ddlDEPTID);
            //PubFunc.DdlDataGet("DDL_SYS_DEPOTRANGE", UserAction.UserID, ddlDEPTOUT);
            PubFunc.DdlDataGet("DDL_SYS_DEPOT", ddlDEPTOUT);
            PubFunc.DdlDataGet("DDL_SYS_DEPTDEF", ddlDEPT);
        }

        private void DataSearch()
        {

        }

        protected void GridGoods_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridGoods.PageIndex = e.NewPageIndex;
            DataSearch();
        }
        protected void btClear_Click(object sender, EventArgs e)
        {
            if (TabStrip1.ActiveTabIndex == 0)
            {
                PubFunc.FormDataClear(FormUser);
                dpkDATE1.SelectedDate = DateTime.Now.AddMonths(-1);
                dpkDATE2.SelectedDate = DateTime.Now;
            }
            if (TabStrip1.ActiveTabIndex == 3)
            {
                tbxGDSEQ.Text = string.Empty;
            }
        }
        protected void btExport_Click(object sender, EventArgs e)
        {
            if (TabStrip1.ActiveTabIndex == 0)
            {
                //科室分类
                if (GridGoods.Rows.Count < 1)
                {
                    Alert.Show("没有数据,无法导出！");
                    return;
                }
            }
            if (TabStrip1.ActiveTabIndex == 1)
            {

            }

        }

        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            string Sql = @"SELECT A.*,DECODE(A.TYPE,'RK','入库单','CK','出库单','XS','销售单','XT','销退单','退货单') TYPENAME,f_getdeptname(A.DEPTOUT) DEPTOUTNAME,f_getdeptname(A.DEPTID) DEPTIDNAME,
                   f_getusername(A.LRY) LRYNAME,f_getusername(A.SHR) SHRNAME,f_getproducername(B.PRODUCER) PRODUCERNAME,B.PIZNO,f_getunitname(B.UNIT) UNITNAME,B.BAR3,B.HISCODE,B.HISNAME
            FROM VIEW_ZP A,DOC_GOODS B
            WHERE A.GDSEQ = B.GDSEQ AND A.SHRQ BETWEEN TO_DATE('{0}','YYYY-MM-DD') AND TO_DATE('{1}','YYYY-MM-DD')+1 ";
            string streach = "";
            if (ddlDEPTOUT.SelectedValue != "")
            {
                streach = string.Format(" AND A.DEPTOUT = '{0}'", ddlDEPTOUT.SelectedValue);
            }
            if (ddlDEPTID.SelectedValue != "")
            {
                streach += string.Format(" AND A.DEPTID = '{0}'", ddlDEPTID.SelectedValue);
            }
            if (ddlTYPE.SelectedValue != "")
            {
                streach += string.Format(" AND A.TYPE = '{0}'", ddlTYPE.SelectedValue);
            }
            if (tgbSch.Text.Trim().Length > 0)
            {
                streach += string.Format(" AND (B.GDSEQ LIKE '%{0}%' OR B.GDNAME LIKE '%{0}%' OR B.BAR3 LIKE '%{0}%' OR B.HISCODE LIKE '%{0}%' OR B.ZJM LIKE '%{0}%' OR B.STR4 LIKE '%{0}%')", tgbSch.Text.Trim().ToUpper());
            }
            streach += " ORDER BY A.SHRQ DESC";
            GridGoods.DataSource = DbHelperOra.Query(string.Format(Sql, dpkDATE1.Text, dpkDATE2.Text) + streach).Tables[0];
            GridGoods.DataBind();
        }

        protected void btnlis_Click(object sender, EventArgs e)
        {
            string Sql = @"SELECT A.*,f_getdeptname(A.DEPTOUT) DEPTOUTNAME,f_getproducername(B.PRODUCER) PRODUCERNAME,B.PIZNO,f_getunitname(B.UNIT) UNITNAME,
                               B.BAR3,B.HISCODE,B.HISNAME,B.GDNAME,B.GDSPEC, 0 HSJJ
                        FROM VIEW_ZPKC A,DOC_GOODS B
                        WHERE A.GDSEQ = B.GDSEQ";

            string Streach = "";
            if (tbxGDSEQ.Text.Trim().Length > 0)
            {
                Streach = string.Format(" AND (B.GDSEQ LIKE '%{0}%' OR B.GDNAME LIKE '%{0}%' OR B.BAR3 LIKE '%{0}%' OR B.HISCODE LIKE '%{0}%' OR B.ZJM LIKE '%{0}%' OR B.STR4 LIKE '%{0}%')", tbxGDSEQ.Text.Trim().ToUpper());
            }
            if (ddlDEPT.SelectedValue != "")
            {
                Streach += string.Format(" AND A.DEPTOUT = '{0}'", ddlDEPT.SelectedValue);
            }
            Streach += " ORDER BY A.DEPTOUT,B.GDNAME";
            Grdlist.DataSource = DbHelperOra.Query(Sql + Streach).Tables[0];
            Grdlist.DataBind();
        }
    }
}