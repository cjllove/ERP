using FineUIPro;
using XTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPProject.ERPQuery
{
    public partial class PSManage : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //打开界面不加载
                DataInit();
                btnSch_Click(null,null);
            }
        }
        //protected void Page_Init(object sender, EventArgs e)
        //{
        //    ColumnShow();
        //}
        private void DataInit()
        {
            PubFunc.DdlDataGet("DDL_SYS_DEPTDEF", ddlDEPTID, StrDeptid, ddlDept);
            PubFunc.DdlDataGet("DDL_WORK_TYPE", ddlTYPE, StrType);
            PubFunc.DdlDataGet("DDL_USER", ddlMANAGER);
            //加载初始数据
            //dpkBegin.SelectedDate = DateTime.Now.AddDays(-1);
            //dpkEnd.SelectedDate = DateTime.Now.AddDays(5);
        }
        #region 查询维护界面
        private void dataSearch()
        {
            string Streach = "";
            string sql = @"SELECT A.*,B.NAME TYPENAME,f_getdeptname(A.DEPTID) DEPTNAME,f_getusername(A.MANAGER) MANAGERNAME
                        FROM DAT_WORK A ,SYS_CODEVALUE B
                        WHERE B.TYPE = 'DAT_WORK_TYPE' 
                        AND A.TYPE = B.CODE(+)";
            if (!PubFunc.StrIsEmpty(StrDeptid.SelectedValue))
            {
                Streach += string.Format(" AND A.DEPTID = '{0}'", StrDeptid.SelectedValue);
            }
            if (!PubFunc.StrIsEmpty(StrType.SelectedValue))
            {
                Streach += string.Format(" AND A.TYPE = '{0}'", StrType.SelectedValue);
            }
            int total = 0;
            DataTable dtData = PubFunc.DbGetPage(GridMange.PageIndex, GridMange.PageSize, (sql + Streach), ref total);
            GridMange.RecordCount = total;
            GridMange.DataSource = dtData;
            GridMange.DataBind();
        }

        protected void trbSearch_TriggerClick(object sender, EventArgs e)
        {
            dataSearch();
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            PubFunc.FormDataClear(FormMange);
            //dpkBegin.SelectedDate = DateTime.Now.AddDays(-1);
            //dpkEnd.SelectedDate = DateTime.Now.AddDays(5);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!chkMONDAY.Checked && !chkTUESDAY.Checked && !chkWEDNESDAY.Checked && !chkTHURSDAY.Checked && !chkFRIDAY.Checked && !chkSATURDAY.Checked && !chkSUNDAY.Checked)
            {
                Alert.Show("请选择业务操作的周期！", "操作提示", MessageBoxIcon.Warning);
                return;
            }
            MyTable mtType = new MyTable("DAT_WORK");
            mtType.ColRow = PubFunc.FormDataHT(FormMange);
            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_WORK WHERE DEPTID = '{0}' AND TYPE = '{1}'", ddlDEPTID.SelectedValue, ddlTYPE.SelectedValue)))
            {
                mtType.InsertExec();
            }
            else
            {
                mtType.UpdateExec("");
            }

            Alert.Show("数据保存成功！");
            dataSearch();
            PubFunc.FormDataClear(FormMange);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            int[] rows = GridMange.SelectedRowIndexArray;
            if (rows.Length > 0)
            {
                List<CommandInfo> cmdList = new List<CommandInfo>();
                foreach (int index in rows)
                {
                    cmdList.Add(new CommandInfo(string.Format("DELETE DAT_WORK WHERE DEPTID='{0}' AND TYPE = '{1}'", GridMange.DataKeys[index][0], GridMange.DataKeys[index][1]), null));
                }
                DbHelperOra.ExecuteSqlTran(cmdList);
                Alert.Show("生产厂家信息删除成功！");
                dataSearch();
            }
            else
            {
                Alert.Show("请选择要删除的信息！");
            }
        }
        protected void GridMange_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridMange.PageIndex = e.NewPageIndex;
            dataSearch();
        }

        protected void GridMange_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            string strDept = GridMange.Rows[e.RowIndex].Values[1].ToString();
            string strType = GridMange.Rows[e.RowIndex].Values[3].ToString();
            string strSql = string.Format("SELECT A.* FROM DAT_WORK A WHERE A.DEPTID='{0}' AND A.TYPE = '{1}'", strDept, strType);
            DataTable dtMange = DbHelperOra.Query(strSql).Tables[0];
            if (dtMange.Rows.Count > 0)
            {
                PubFunc.FormDataSet(FormMange, dtMange.Rows[0]);
            }
        }
        #endregion

        #region 展示界面
        #region 模板列
        //private void ColumnShow()
        //{
        //    FineUIPro.BoundField bf;
        //    bf = new FineUIPro.BoundField();
        //    bf.DataField = "DEPTNAME";
        //    bf.DataFormatString = "{0}";
        //    bf.HeaderText = "科室名称";
        //    GridView.Columns.Add(bf);

        //    bf = new FineUIPro.BoundField();
        //    bf.DataField = "TYPENAME";
        //    bf.DataFormatString = "{0}";
        //    bf.HeaderText = "作业类型";
        //    GridView.Columns.Add(bf);

        //    bf = new FineUIPro.BoundField();
        //    bf.DataField = "MANAGER";
        //    bf.DataFormatString = "{0}";
        //    bf.HeaderText = "负责人";
        //    GridView.Columns.Add(bf);

        //    FineUIPro.ImageField bfimage = new FineUIPro.ImageField();
        //    //bf = new FineUIPro.ImageField();
        //    bf.DataField = "MONDAY";
        //    bf.DataFormatString = "{0}";
        //    bf.HeaderText = "星期一";
        //    GridView.Columns.Add(bf);

        //    bf = new FineUIPro.BoundField();
        //    bf.DataField = "TUESDAY";
        //    bf.DataFormatString = "{0}";
        //    bf.HeaderText = "星期二";
        //    GridView.Columns.Add(bf);
        //    bf = new FineUIPro.BoundField();
        //    bf.DataField = "WEDNESDAY";
        //    bf.DataFormatString = "{0}";
        //    bf.HeaderText = "星期三";
        //    GridView.Columns.Add(bf);
        //    bf = new FineUIPro.BoundField();
        //    bf.DataField = "THURSDAY";
        //    bf.DataFormatString = "{0}";
        //    bf.HeaderText = "星期四";
        //    GridView.Columns.Add(bf);
        //    bf = new FineUIPro.BoundField();
        //    bf.DataField = "FRIDAY";
        //    bf.DataFormatString = "{0}";
        //    bf.HeaderText = "星期五";
        //    GridView.Columns.Add(bf);
        //    bf = new FineUIPro.BoundField();
        //    bf.DataField = "FRIDAY";
        //    bf.DataFormatString = "{0}";
        //    bf.HeaderText = "星期六";
        //    GridView.Columns.Add(bf);
        //    bf = new FineUIPro.BoundField();
        //    bf.DataField = "FRIDAY";
        //    bf.DataFormatString = "{0}";
        //    bf.HeaderText = "星期日";
        //    GridView.Columns.Add(bf);
        //}

        #endregion
        protected void btnSch_Click(object sender, EventArgs e)
        {
            string strSch = "";
            if (!PubFunc.StrIsEmpty(ddlDept.SelectedValue))
            {
                strSch = string.Format(" AND A.DEPTID = '{0}'", ddlDept.SelectedValue);
            }
            string sql = @"SELECT A.*,B.NAME TYPENAME,f_getdeptname(A.DEPTID) DEPTNAME,f_getusername(A.MANAGER) MANAGERNAME
                        FROM DAT_WORK A ,SYS_CODEVALUE B
                        WHERE B.TYPE = 'DAT_WORK_TYPE' 
                        AND A.TYPE = B.CODE(+)";
            GridView.DataSource = DbHelperOra.Query(sql).Tables[0];
            GridView.DataBind();
        }
        #endregion
    }
}