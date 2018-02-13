using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FineUIPro;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using XTBase;
using System.Data;
using System.Collections.Specialized;
using System.Text;



namespace ERPProject.ERPEntrust
{
    public partial class EntrustAudit : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
            }
        }


        protected void billSearch()
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【使用时间】！");
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                Alert.Show("【开始日期】大于【结束日期】，请重新输入！");
                return;
            }

            string strSql = @"SELECT A.*,F_GETUSERNAME(A.OPERUSER) USERNAME,G.GDNAME,F_GETSUPNAME(A.SUPID) SUPNAME,
                                     DECODE(A.FLAG,'N','新生成','Y','已下单','已完成') FLAG_CN,F_GETUSERNAME(A.ORDUSER) ORDUSERNAME,
                                     F_GETUNITNAME(G.UNIT) UNIT FROM DAT_USE_DET A,DOC_GOODS G WHERE A.GDSEQ=G.GDSEQ ";
            string strSearch = "";
            strSearch += string.Format(" AND A.USERTIME>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.USERTIME <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);
            if (tbxBILLNO.Text.Trim().Length > 0)
            {
                strSearch += string.Format(" AND (A.BILLNO  LIKE '%{0}%' OR A.GDSEQ  LIKE '%{0}%' OR G.GDNAME  LIKE '%{0}%')", tbxBILLNO.Text.Trim());
            }
            if (lstFLAG.SelectedItem != null && lstFLAG.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.FLAG  = '{0}'", lstFLAG.SelectedItem.Value);
            }
            if (lstLRY.SelectedItem != null && lstLRY.SelectedItem.Value.Length > 0)
            {//判断使用人
                strSearch += string.Format(" AND A.OPERUSER  LIKE '%{0}%'", lstLRY.SelectedItem.Value);
            }
            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedItem.Value.Length > 0)
            {//判断部门
                strSearch += string.Format(" AND A.DEPTID  LIKE '%{0}%'", lstDEPTID.SelectedItem.Value);
            }
            if (lstSUPID.SelectedItem != null && lstSUPID.SelectedItem.Value.Length > 0)
            {//判断供应商
                strSearch += string.Format(" AND A.SUPID  LIKE '%{0}%'", lstSUPID.SelectedItem.Value);
            }
            strSql += strSearch;

            int total = 0;
            DataTable dt = PubFunc.DbGetPage(GridList.PageIndex, GridList.PageSize, strSql, ref total);
            GridList.DataSource = dt;
            GridList.RecordCount = total;
            GridList.DataBind();


        }
        protected void GridList_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                string flag = row["FLAG_CN"].ToString();

                if (flag != "新生成")
                {
                    e.RowSelectable = false;
                }
            }
        }

        private void DataInit()
        {
            //录入员下拉表
            PubFunc.DdlDataGet("DDL_USER", lstLRY);
            //使用部门下拉表
            PubFunc.DdlDataGet("DDL_SYS_DEPTDEF", lstDEPTID);
            //供应商下拉表
            PubFunc.DdlDataGet("DDL_DOC_SUPPLIER_DG", lstSUPID);
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
        }

        protected void btnAudit_Click(object sender, EventArgs e)
        {

            int[] selections = GridList.SelectedRowIndexArray;
            if (selections.Length == 0)
            {
                Alert.Show("您没有选择任何信息", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string YBILLNO = "";
            string ROWNO = "";

            string strFPSeq = DbHelperOra.GetSingle("SELECT SEQ_PUBLIC.NEXTVAL FROM DUAL").ToString();
            List<CommandInfo> cmdList = new List<CommandInfo>();
            foreach (int rowIndex in selections)
            {
                YBILLNO = GridList.DataKeys[rowIndex][0].ToString();
                ROWNO = GridList.DataKeys[rowIndex][1].ToString();
                cmdList.Add(new CommandInfo("UPDATE DAT_USE_DET SET DEALWITHSEQ='" + strFPSeq + "' where BILLNO='" + YBILLNO + "' and ROWNO='" + ROWNO + "'", null));

            }
            OracleParameter[] parameters = {
                                               new OracleParameter("VI_SEQ", OracleDbType.Int32),
                                               new OracleParameter("VI_USER", OracleDbType.Varchar2) };
            parameters[0].Value = strFPSeq;
            parameters[1].Value = UserAction.UserID;
            cmdList.Add(new CommandInfo("STORE.P_DG_AUDIT", parameters, CommandType.StoredProcedure));
            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {

                Alert.Show("代管商品使用信息确认成功！", "消息提示", MessageBoxIcon.Information);
                billSearch();
            }
            else
            {
                Alert.Show("代管商品使用信息确认失败！", "错误提示", MessageBoxIcon.Error);
            }
        }

        protected void bntSearch_Click(object sender, EventArgs e)
        {
            billSearch();
        }

        protected void GridList_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridList.PageIndex = e.NewPageIndex;
            billSearch();
        }
    }
}