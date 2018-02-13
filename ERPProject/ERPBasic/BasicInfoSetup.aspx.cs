using FineUIPro;
using XTBase;
using XTBase.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;

namespace ERPProject.ERPBasic
{
    public partial class BasicInfoSetup : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
                DataSearch();
            }
        }

        private void DataInit()
        {
            PubFunc.DdlDataGet(ddlDEPTID, "DDL_SYS_DEPTDEF");
            PubFunc.DdlDataGet("DDL_USER", ddlPSY);
            PubFunc.DdlDataGet("DDL_DHZQ", ddlFLAG);
            //绑定用户下拉
            String sql = @"SELECT USERNAME, USERID FROM SYS_OPERUSER WHERE STATUS = '01'";
            grdUser.DataSource = DbHelperOra.Query(sql).Tables[0];
            grdUser.DataBind();
        }

        private void DataSearch()
        {
            string strSql = @"SELECT A.CODE,A.NAME,
                              nvl(A.DHZQ1,'N') DHZQ1,
                              nvl(A.DHZQ2,'N') DHZQ2,
                              nvl(A.DHZQ3,'N') DHZQ3,
                              nvl(A.DHZQ4,'N') DHZQ4,
                              nvl(A.DHZQ5,'N') DHZQ5,
                              nvl(A.DHZQ6,'N') DHZQ6,
                              nvl(A.DHZQ7,'N') DHZQ7,STR4
                                FROM SYS_DEPT A WHERE 1=1";

            string strSearch = "";
            if (!string.IsNullOrWhiteSpace(ddlDEPTID.SelectedValue))
            {
                strSearch = string.Format(" AND A.CODE = '{0}'", ddlDEPTID.SelectedValue);
            }
            if (!string.IsNullOrWhiteSpace(ddlPSY.SelectedValue))
            {
                strSearch = string.Format(" AND A.STR4 = '{0}'", ddlPSY.SelectedValue);
            }
            if (!string.IsNullOrWhiteSpace(ddlPSTIME.SelectedValue) && string.IsNullOrWhiteSpace(ddlFLAG.SelectedValue))
            {
                Alert.Show("选择【配送时间】后需要同时选择【配送状态】才能查询到信息！", MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(ddlPSTIME.SelectedValue) && !string.IsNullOrWhiteSpace(ddlFLAG.SelectedValue))
            {
                strSearch = string.Format(" AND (A.DHZQ1 = '{1}' OR A.DHZQ2 = '{1}' OR  A.DHZQ3 = '{1}' OR  A.DHZQ4 = '{1}' OR A.DHZQ5 = '{1}' OR A.DHZQ6 = '{1}' OR A.DHZQ7 = '{1}')", ddlPSTIME.SelectedValue, ddlFLAG.SelectedValue);
            }
            if (!string.IsNullOrWhiteSpace(ddlPSTIME.SelectedValue) && !string.IsNullOrWhiteSpace(ddlFLAG.SelectedValue))
            {
                strSearch = string.Format(" AND A.{0} = '{1}'", ddlPSTIME.SelectedValue, ddlFLAG.SelectedValue);
            }
            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }

            //strSql.Append(" order by cfg.deptid(+),g.GDSEQ,g.GDNAME");
            strSql += " order by A.CODE";
            //int total = 0;
            GridGoods.DataSource = DbHelperOra.Query(strSql).Tables[0];//GetDataTable(GridGoods.PageIndex, GridGoods.PageSize, strSql, ref total);
            //GridGoods.RecordCount = total;
            GridGoods.DataBind();
            PageContext.RegisterStartupScript("updateStyle()");
        }
        protected void bntSearch_Click(object sender, EventArgs e)
        {
            DataSearch();
        }

        protected void bntSave_Click(object sender, EventArgs e)
        {
            List<CommandInfo> cmdList = new List<CommandInfo>();
            JArray jary = GridGoods.GetModifiedData();
            if (jary.Count < 1)
            {
                Alert.Show("未修改信息，不需要保存!", MessageBoxIcon.Warning);
                return;
            }
            foreach (JObject job in jary)
            {
                String deptid = GridGoods.DataKeys[Convert.ToInt16(job["index"])][0].ToString();
                String ddlSTR4 = (job.SelectToken(string.Format("$..values.{0}", "STR4")) ?? "").ToString();
                String ddlDHZQ1 = (job.SelectToken(string.Format("$..values.{0}", "DHZQ1")) ?? "").ToString();
                String ddlDHZQ2 = (job.SelectToken(string.Format("$..values.{0}", "DHZQ2")) ?? "").ToString();
                String ddlDHZQ3 = (job.SelectToken(string.Format("$..values.{0}", "DHZQ3")) ?? "").ToString();
                String ddlDHZQ4 = (job.SelectToken(string.Format("$..values.{0}", "DHZQ4")) ?? "").ToString();
                String ddlDHZQ5 = (job.SelectToken(string.Format("$..values.{0}", "DHZQ5")) ?? "").ToString();
                String ddlDHZQ6 = (job.SelectToken(string.Format("$..values.{0}", "DHZQ6")) ?? "").ToString();
                String ddlDHZQ7 = (job.SelectToken(string.Format("$..values.{0}", "DHZQ7")) ?? "").ToString();

                cmdList.Add(new CommandInfo(string.Format(@"UPDATE SYS_DEPT SET 
                            STR4 = NVL('{0}',STR4),DHZQ1 = NVL('{1}',DHZQ1), DHZQ2 = NVL('{2}',DHZQ2),DHZQ3 = NVL('{3}',DHZQ3), DHZQ4 = NVL('{4}',DHZQ4),DHZQ5 = NVL('{5}',DHZQ5), DHZQ6 = NVL('{6}',DHZQ6),DHZQ7 = NVL('{7}',DHZQ7) 
                            WHERE CODE = '{8}'", ddlSTR4, ddlDHZQ1, ddlDHZQ2, ddlDHZQ3, ddlDHZQ4, ddlDHZQ5, ddlDHZQ6, ddlDHZQ7, deptid), null));
            }
            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                Alert.Show("数据保存成功！");
                DataSearch();
            }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {

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

        protected void GridGoods_RowDataBound(object sender, GridRowEventArgs e)
        {
            //渲染颜色
            DataRowView row = e.DataItem as DataRowView;
            string DHZQ1 = row["DHZQ1"].ToString();
            RenderField rDHZQ1 = GridGoods.FindColumn("DHZQ1") as RenderField;
            if (DHZQ1 == "A")
            {
                e.CellCssClasses[rDHZQ1.ColumnIndex] = "color1";
            }
        }

        protected void bntClear_Click(object sender, EventArgs e)
        {
            ddlDEPTID.SelectedValue = "";
            ddlPSY.SelectedValue = "";
            ddlPSTIME.SelectedValue = "";
            ddlFLAG.SelectedValue = "";
            DataSearch();
        }
    }
}