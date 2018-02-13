using FineUIPro;
using Newtonsoft.Json.Linq;
using XTBase;
using XTBase.Utilities;
using ERPProject;
using System;
using System.Data;
using System.Linq;

namespace ERPProject.ERPQuery
{
    public partial class TodayPs : PageBase
    {
        public TodayPs() {
            ISCHECK = false;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Bind();
                btnClear_Click(null, null);
                DataSearch();
            }
        }
        protected void Bind()
        {
            PubFunc.DdlDataGet("DDL_USER", ddlUser);
            DepartmentBind.BindDDL("DDL_SYS_DEPTRANGE", UserAction.UserID, ddlDept);
        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            ddlDept.SelectedValue = UserAction.UserDept;
            ddlUser.SelectedValue = "";
            //是否为配送员
            if (DbHelperOra.Exists(string.Format("SELECT 1 FROM SYS_DEPT WHERE STR4 = '{0}'", UserAction.UserID)))
            {
                ddlUser.SelectedValue = UserAction.UserID;
            }
            dpkTime.SelectedDate = DateTime.Now;
        }
        private string GetSearchSql()
        {
            string strNum = DbHelperOra.GetSingle(string.Format("select to_char(to_date('{0}','yyyy-mm-dd')-1,'d') from dual", dpkTime.Text)).ToString();
            string strSql = string.Format(@"SELECT A.*,F_GETDEPTPS(A.CODE,'{1}','FLAG') FLAGNAME,f_getusername(A.STR4) PSYNAME,F_GETDEPTPS(A.CODE,'{1}','TIME') TIMENAME
                                FROM SYS_DEPT A
                                WHERE A.DHZQ{0} <> 'N'  AND A.code in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '{2}') = 'Y' )  ", strNum, dpkTime.Text, UserAction.UserID);
            string strWhere = " ";
            if (ddlUser.SelectedValue != "")
            {
                strWhere += " AND A.STR4 = '" + ddlUser.SelectedValue + "'";
            }
            if (ddlDept.SelectedValue != "")
            {
                strWhere += " AND A.CODE = '" + ddlDept.SelectedValue + "'";
            }

            if (strWhere != " ") strSql = strSql + strWhere;

            strSql += string.Format(" ORDER BY {0} {1}", GridCom.SortField, GridCom.SortDirection);
            return strSql;
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            DataTable dtData = DbHelperOra.Query(GetSearchSql()).Tables[0];
            if (dtData == null || dtData.Rows.Count == 0)
            {
                Alert.Show("没有数据,无法导出！");
                return;
            }
            string[] columnNames = new string[GridCom.Columns.Count - 1];
            for (int index = 1; index < GridCom.Columns.Count; index++)
            {
                GridColumn column = GridCom.Columns[index];
                if (column is FineUIPro.BoundField)
                {
                    dtData.Columns[((FineUIPro.BoundField)(column)).DataField.ToUpper()].ColumnName = column.HeaderText;
                    columnNames[index - 1] = column.HeaderText;
                }
            }

            ExcelHelper.ExportByWeb(dtData.DefaultView.ToTable(true, columnNames), "科室定数信息", string.Format("科室定数信息_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            DataSearch();
        }

        private void DataSearch()
        {
            DataTable dtData = DbHelperOra.Query(GetSearchSql()).Tables[0];
            GridCom.DataSource = dtData;
            GridCom.DataBind();
            OutputSummaryData(dtData);
        }

        protected void GridCom_Sort(object sender, GridSortEventArgs e)
        {
            GridCom.SortDirection = e.SortDirection;
            GridCom.SortField = e.SortField;
            DataSearch();
        }

        private void OutputSummaryData(DataTable source)
        {
            JObject summary = new JObject();
            summary.Add("PSYNAME", string.Format("应配送科室量：{0}个", source.Rows.Count));
            summary.Add("NAME", string.Format("已配送科室量：{0}个", source.Select("FLAGNAME = '已配送'").Count()));
            summary.Add("FLAGNAME", string.Format("未配送科室量：{0}个", source.Select("FLAGNAME <> '已配送'").Count()));
            if (source.Rows.Count > 0)
            {
                decimal bf = source.Select("FLAGNAME = '已配送'").Count() * 100 / source.Rows.Count;
                summary.Add("TIMENAME", string.Format("已配送科室比例：{0}%", Math.Round(bf, 2)));
            }
            else
            {
                summary.Add("TIMENAME", string.Format("已配送科室比例：{0}", "0.00%"));
            }

            GridCom.SummaryData = summary;
        }
    }
}