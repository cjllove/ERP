using FineUIPro;
using XTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace ERPProject.ERPAssist
{
    public partial class VersionInput : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
            }

        }

        #region page load event

        public void DataInit()
        {
            PubFunc.DdlDataGet("DDL_USER", ddlUPTPER, ddlTESTPER, ddlCHECKPER);

            tbxSYSNAME.Text = ConfigurationManager.AppSettings["APPNAME"];
            dptUPTDATE.SelectedDate = DateTime.Now;
            ddlUPTPER.SelectedValue = UserAction.UserID;
            ddlCHECKPER.SelectedValue = "xiadl";

            object obj = DbHelperOra.GetSingle(@"select version
                                                from  (select  t.*, f_getusername(t.uptper)  from sys_version t  order by  uptdate desc) 
                                            where rownum<2");
            if (!string.IsNullOrWhiteSpace((obj ?? "").ToString()))
            {
                string[] s = obj.ToString().Split('.');
                s[2] = (Convert.ToInt16(s[0]) + 1).ToString();
                string str1 = string.Join(".", s);
                tbxVERSION.Text = str1;
            }



            //BindWinChkToPersons();
            btnSearch();

        }

        protected void btnClear_Click(object sender, EventArgs e)
        {


        }
        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            btnSave.Enabled = true;
            PubFunc.FormDataClear(FormList);
            tbxUPTMEMO.Text = "";
            DataInit();
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbxUPTMEMO.Text))
            {
                Alert.Show("更新内容为空，请填写");
                return;
            }
            if (hfdValue.Text == "N")
            {
                if (DbHelperOra.Exists(string.Format("SELECT 1 FROM SYS_VERSION WHERE FLAG = 'Y' AND VERSION = '{0}'", tbxVERSION.Text)))
                {
                    Alert.Show("当前输入的【版本号】已存在，请重新输入");
                    return;
                }

                string strSYSNAME = tbxSYSNAME.Text.Trim();
                string strVERSION = tbxVERSION.Text.Trim();
                string strUPTDATE = Convert.ToDateTime(dptUPTDATE.SelectedDate).ToString("yyyy-MM-dd");
                string strUPTPER = ddlUPTPER.SelectedValue;
                string strTESTPER = ddlTESTPER.SelectedValue;
                string strCHECKPER = ddlCHECKPER.SelectedValue;
                string strUPTMEMO = tbxUPTMEMO.Text;

                string strSql = "";
                strSql = "INSERT INTO  SYS_VERSION (SYSNAME,VERSION,UPTDATE,FLAG,UPTPER,TESTPER,CHECKPER,UPTMEMO) VALUES('" + strSYSNAME + "','" + strVERSION + "',to_date('" + strUPTDATE + "','yyyy-mm-dd'),'Y','" + strUPTPER + "','" + strTESTPER + "','" + strCHECKPER + "','" + strUPTMEMO + "' )";
                if (DbHelperOra.ExecuteSql(strSql) > 0)
                {
                    Alert.Show("数据保存成功！");
                    btnSave.Enabled = false;
                    btnSearch();
                }
            }
            else
            {
                string strSYSNAME = tbxSYSNAME.Text.Trim();
                string strVERSION = tbxVERSION.Text.Trim();
                string strUPTDATE = Convert.ToDateTime(dptUPTDATE.SelectedDate).ToString("yyyy-MM-dd");
                string strUPTPER = ddlUPTPER.SelectedValue;
                string strTESTPER = ddlTESTPER.SelectedValue;
                string strCHECKPER = ddlCHECKPER.SelectedValue;
                string strUPTMEMO = tbxUPTMEMO.Text;

                string strSql = "";
                strSql = "UPDATE SYS_VERSION SET SYSNAME = '" + strSYSNAME + "',UPTPER='" + strUPTPER + "',TESTPER='" + strTESTPER + "',CHECKPER='" + strCHECKPER + "',UPTMEMO='" + strUPTMEMO + "' WHERE VERSION='" + strVERSION + "'";
                if (DbHelperOra.ExecuteSql(strSql) > 0)
                {
                    Alert.Show("数据更新成功！");
                    btnSave.Enabled = false;
                    btnSearch();
                    hfdValue.Text = "N";
                }
            }

        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            btnSearch();
        }

        protected void btnSearch()
        {
            string strSQL = "SELECT SYSNAME,VERSION,UPTDATE,TO_CHAR(UPTMEMO) UPTMEMO,f_getusername(UPTPER) UPTPER, f_getusername(TESTPER) TESTPER,f_getusername(checkper) checkper FROM SYS_VERSION WHERE FLAG = 'Y' ORDER BY uptdate DESC";
            int total = 0;
            DataTable dt = PubFunc.DbGetPage(GridList.PageIndex, GridList.PageSize, strSQL, ref total);
            GridList.DataSource = dt;
            GridList.RecordCount = total;
            GridList.DataBind();
        }

        #endregion


        protected void GridList_Sort(object sender, GridSortEventArgs e)
        {
            GridList.SortDirection = e.SortDirection;
            GridList.SortField = e.SortField;

            DataTable table = PubFunc.GridDataGet(GridList);
            DataView view1 = table.DefaultView;
            view1.Sort = String.Format("{0} {1}", GridList.SortField, GridList.SortDirection);
            GridList.DataSource = view1;
            GridList.DataBind();
        }

        protected void GridList_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridList.PageIndex = e.NewPageIndex;
            btnSearch();
        }

        protected void GridList_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            string strVersion = GridList.Rows[e.RowIndex].Values[2].ToString();
            string strSql = "SELECT A.*,TO_CHAR(UPTMEMO) UPTMEMO2 FROM SYS_VERSION A WHERE FLAG = 'Y' AND VERSION = '{0}'";
            DataTable dt = DbHelperOra.Query(string.Format(strSql, strVersion)).Tables[0];
            //dt.Columns.Remove("UPTMEMO");
            //dt.Columns["UPTMEMO2"].ColumnName = "UPTMEMO";
            PubFunc.FormDataSet(FormList, dt.Rows[0]);
            hfdValue.Text = "Y";
            tbxUPTMEMO.Text = dt.Rows[0]["UPTMEMO2"].ToString();
        }
    }
}