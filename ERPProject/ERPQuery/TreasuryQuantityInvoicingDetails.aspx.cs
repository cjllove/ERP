using FineUIPro;
using Newtonsoft.Json.Linq;
using XTBase;
using XTBase.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPProject.ERPQuery
{
    public partial class TreasuryQuantityInvoicingDetails : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindDDL();
            }
        }
        private void BindDDL()
        {
            RQXZ.Text = DateTime.Today.ToString("yyyy-MM");
            RQXZ2.Text = DateTime.Today.ToString("yyyy-MM");
            DepartmentBind.BindDDL("DDL_SYS_DEPOTRANGE", UserAction.UserID, ddlDEPTID);
            CleanItemForDDL("--请选择--", ddlDEPTID);
            ddlDEPTID.SelectedIndex = 0;

            DepartmentBind.BindDDL("DDL_SYS_DEPTRANGE", UserAction.UserID, ddlDEPTID2);
            CleanItemForDDL("--请选择--", ddlDEPTID2);
            ddlDEPTID2.SelectedIndex = 0;

            string strSql = @"select * from 
                                    (SELECT '' CODE,'--请选择--' NAME,0 TreeLevel,0 islast FROM dual
                                    union all
                                    select code,
                                           '【' || code || '】' || name name,
                                           class TreeLevel,
                                           decode(islast, 'Y', 1, 0) islast
                                      from sys_category
                                     ORDER BY code)
                                     ORDER BY DECODE(CODE,'',99,0) DESC ,CODE ASC ";
            List<CategoryTreeBean> myList = new List<CategoryTreeBean>();
            DataTable categoryTreeTable = DbHelperOra.Query(strSql).Tables[0];
            foreach (DataRow dr in categoryTreeTable.Rows)
            {
                myList.Add(new CategoryTreeBean(dr["code"].ToString(), dr["name"].ToString(), Convert.ToInt16(dr["TreeLevel"]), Convert.ToInt16(dr["islast"]) == 1));
            }
            ddlCATID.EnableSimulateTree = true;
            ddlCATID.DataTextField = "Name";
            ddlCATID.DataValueField = "Id";
            ddlCATID.DataSimulateTreeLevelField = "Level";
            ddlCATID.DataSource = myList;
            ddlCATID.DataBind();

            ddlCATID2.EnableSimulateTree = true;
            ddlCATID2.DataTextField = "Name";
            ddlCATID2.DataValueField = "Id";
            ddlCATID2.DataSimulateTreeLevelField = "Level";
            ddlCATID2.DataSource = myList;
            ddlCATID2.DataBind();


            USERID.Text = UserAction.UserID;
        }
        protected void btClear_Click(object sender, EventArgs e)
        {
            ddlDEPTID.SelectedIndex = 0;
            tbxGOODS.Text = "";
            ddlCATID.SelectedIndex = 0;
        }
        private string GetSearchSql(string StartTime, string EndTime)
        {
            string strSql = "";
            string strDeptOut = @"SELECT TB.GDSEQ, 
                                            F_GETHISINFO(TB.GDSEQ, 'GDNAME') GDNAME, 
                                            F_GETHISINFO(TB.GDSEQ, 'GDSPEC') GDSPEC, 
                                            F_GETUNITNAME(DG.UNIT) UNIT, 
                                            F_GETDEPTNAME(TB.DEPTID) DEPTID, 
                                            NVL(TA.QCKCSL, 0) QCKCSL, 
                                            NVL(TA.QCKCHSJE, 0) QCKCHSJE, 
                                            NVL(TB.CGRK, 0) CGRK, 
                                            NVL(TB.KSTH, 0) KSTH, 
                                            NVL(TB.PYRK, 0) PYRK, 
                                            NVL(TB.DBRK, 0) DBRK,  
                                            NVL(TB.CGRK, 0) + NVL(TB.KSTH, 0) + NVL(TB.PYRK, 0) + NVL(TB.DBRK, 0) RKHJ,
                                            NVL(TB.KFCK, 0) KFCK, 
                                            NVL(TB.THCK, 0)+NVL(TB.XSTHCK, 0) THCK, 
                                            NVL(TB.PKCK, 0) PKCK, 
                                            NVL(TB.DBCK, 0) DBCK, 
                                            NVL(TB.KFCK, 0) + (NVL(TB.THCK, 0)+NVL(TB.XSTHCK, 0)) + NVL(TB.PKCK, 0) + NVL(TB.DBCK, 0) CKHJ,
                                            (NVL(TB.CGRK, 0) + NVL(TB.KSTH, 0) + NVL(TB.PYRK, 0) + NVL(TB.DBRK, 0)) - 
                                            (NVL(TB.KFCK, 0) + (NVL(TB.THCK, 0)+NVL(TB.XSTHCK, 0)) + NVL(TB.PKCK, 0) + NVL(TB.DBCK, 0))  QMKCSL
                                            FROM (SELECT GDSEQ, 
                                            DEPTID, 
                                            SUM(DECODE(TO_CHAR(RQ, 'YYYYMMDD'), '{2}', KCSL, 0)) QCKCSL, 
                                            SUM(DECODE(TO_CHAR(RQ, 'YYYYMMDD'), '{2}', KCHSJE, 0)) QCKCHSJE 
                                            FROM (SELECT GDSEQ, DEPTID, KCSL, KCHSJE, RQ 
                                            FROM DAT_STOCKDAY 
                                            UNION ALL 
                                            SELECT GDSEQ, DEPTID, KCSL, KCHSJE, TRUNC(SYSDATE) 
                                            FROM DAT_GOODSSTOCK) T 
                                            WHERE T.RQ = TO_DATE('{2}', 'YYYYMMDD') 
                                            GROUP BY GDSEQ, DEPTID) TA, 
                                            (SELECT GDSEQ, 
                                            DEPTID, 
                                            SUM(DECODE(A.KCADD, '1', DECODE(A.BILLTYPE,'RKD',SL,0), 0)) CGRK, 
                                            SUM(DECODE(A.KCADD, '1', DECODE(A.BILLTYPE,'LTD',SL,'XST',SL,'DST',SL,0), 0)) KSTH, 
                                            SUM(DECODE(A.KCADD, '1', DECODE(A.BILLTYPE,'SYD',SL,0), 0)) PYRK,
                                            SUM(DECODE(A.KCADD, '1', DECODE(A.BILLTYPE,'DBD',SL,0), 0)) DBRK, --调拨入库
                                            ABS(SUM(DECODE(A.KCADD, '-1', DECODE(A.BILLTYPE,'LCD',SL,'CKD',SL,'DSC',SL,0), 0))) KFCK, 
                                            ABS(SUM(DECODE(A.KCADD, '-1', DECODE(A.BILLTYPE,'THD',SL,0), 0))) THCK, 
                                            ABS(SUM(DECODE(A.KCADD, '-1', DECODE(A.BILLTYPE, 'XST', SL, 0), 0))) XSTHCK,
                                            ABS(SUM(DECODE(A.KCADD, '-1', DECODE(A.BILLTYPE,'SYD',SL,0), 0))) PKCK, 
                                            ABS(SUM(DECODE(A.KCADD, '-1', DECODE(A.BILLTYPE,'DBD',SL,0), 0))) DBCK --调拨出库
                                            FROM DAT_GOODSJXC A 
                                            WHERE A.RQSJ > （TO_DATE('{0}', 'YYYYMMDD')+1） 
                                            AND A.RQSJ <= ( TO_DATE('{1}', 'YYYYMMDD')  )
                                            GROUP BY GDSEQ, DEPTID) TB,  
                                            DOC_GOODS DG 
                                            WHERE TA.GDSEQ(+) = TB.GDSEQ 
                                            AND TA.DEPTID(+) = TB.DEPTID 
                                            AND TB.GDSEQ = DG.GDSEQ ";
            string strDeptIn = @"SELECT TB.GDSEQ, 
                                            F_GETHISINFO(TB.GDSEQ, 'GDNAME') GDNAME, 
                                            F_GETHISINFO(TB.GDSEQ, 'GDSPEC') GDSPEC, 
                                            F_GETUNITNAME(DG.UNIT) UNIT, 
                                            F_GETDEPTNAME(TB.DEPTID) DEPTID, 
                                            NVL(TA.QCKCSL, 0) QCKCSL, 
                                            NVL(TA.QCKCHSJE, 0) QCKCHSJE,
                                            NVL(TB.LCDRK, 0) LCDRK, 
                                            NVL(TB.CKDRK, 0) CKDRK, 
                                            NVL(TB.DSCRK, 0) DSCRK,  
                                            NVL(TB.DBRK, 0) DBRK,
                                            NVL(TB.PYRK, 0) PYRK,  
                                            NVL(TB.LCDRK, 0) + NVL(TB.CKDRK, 0) + NVL(TB.DSCRK, 0) + NVL(TB.DBRK, 0) + NVL(TB.PYRK, 0) RKHJ,
                                            NVL(TB.LTDCK, 0) LTDCK, 
                                            NVL(TB.XSTCK, 0) XSTCK,  
                                            NVL(TB.DBCK, 0) DBCK, 
                                            NVL(TB.PKCK, 0) PKCK,
                                            NVL(TB.LTDCK, 0) + NVL(TB.XSTCK, 0) + NVL(TB.PKCK, 0) + NVL(TB.DBCK, 0)  CKHJ,
                                            NVL(TA.QCKCSL, 0)+ NVL(TB.LCDRK, 0) + NVL(TB.CKDRK, 0) + NVL(TB.DSCRK, 0) +NVL(TB.DBRK, 0)+NVL(TB.PYRK, 0) - 
                                            (NVL(TB.LTDCK, 0) + NVL(TB.XSTCK, 0) + NVL(TB.PKCK, 0) + NVL(TB.DBCK, 0)) QMKCSL
                                            FROM (SELECT GDSEQ, 
                                            DEPTID, 
                                            SUM(DECODE(TO_CHAR(RQ, 'YYYYMMDD'), '{2}', KCSL, 0)) QCKCSL, 
                                            SUM(DECODE(TO_CHAR(RQ, 'YYYYMMDD'), '{2}', KCHSJE, 0)) QCKCHSJE 
                                            FROM (SELECT GDSEQ, DEPTID, KCSL, KCHSJE, RQ 
                                            FROM DAT_STOCKDAY 
                                            UNION ALL 
                                            SELECT GDSEQ, DEPTID, KCSL, KCHSJE, TRUNC(SYSDATE) 
                                            FROM DAT_GOODSSTOCK) T 
                                            WHERE T.RQ = TO_DATE('{2}', 'YYYYMMDD') 
                                            GROUP BY GDSEQ, DEPTID) TA, 
                                            (SELECT GDSEQ, 
                                            DEPTID, 
                                            SUM(DECODE(A.KCADD, '1', DECODE(A.BILLTYPE,'LCD',SL,0), 0)) LCDRK, 
                                            SUM(DECODE(A.KCADD, '1', DECODE(A.BILLTYPE,'CKD',SL,0), 0)) CKDRK, 
                                            SUM(DECODE(A.KCADD, '1', DECODE(A.BILLTYPE,'DSC',SL,0), 0)) DSCRK,  
                                            SUM(DECODE(A.KCADD, '1', DECODE(A.BILLTYPE,'DBD',SL,0), 0)) DBRK,
                                            SUM(DECODE(A.KCADD, '1', DECODE(A.BILLTYPE,'SYD',SL,0), 0)) PYRK, 
                                            SUM(DECODE(A.KCADD, '-1', DECODE(A.BILLTYPE,'LTD',SL,0), 0)) LTDCK, 
                                            SUM(DECODE(A.KCADD, '-1', DECODE(A.BILLTYPE,'XST',SL,0), 0)) XSTCK, 
                                            SUM(DECODE(A.KCADD, '-1', DECODE(A.BILLTYPE,'DBD',SL,0), 0)) DBCK, 
                                            SUM(DECODE(A.KCADD, '-1', DECODE(A.BILLTYPE,'SYD',SL,0), 0)) PKCK 
                                            FROM DAT_GOODSJXC A 
                                            WHERE A.RQSJ > （TO_DATE('{0}', 'YYYYMMDD')+1） 
                                            AND A.RQSJ <= ( TO_DATE('{1}', 'YYYYMMDD')  )
                                            GROUP BY GDSEQ, DEPTID) TB,  
                                            DOC_GOODS DG 
                                            WHERE TA.GDSEQ(+) = TB.GDSEQ 
                                            AND TA.DEPTID(+) = TB.DEPTID 
                                            AND TB.GDSEQ = DG.GDSEQ ";
            string strWhere = " ";
            
            if(TabStrip1.ActiveTabIndex == 0 )
            {
                if (!PubFunc.StrIsEmpty(ddlDEPTID.SelectedValue)) strWhere += " and TB.DEPTID = '" + ddlDEPTID.SelectedValue + "'";

                if (!PubFunc.StrIsEmpty(ddlCATID.SelectedValue)) strWhere += " and DG.CATID = '" + ddlCATID.SelectedValue + "'";

                if (!PubFunc.StrIsEmpty(tbxGOODS.Text)) strWhere += " and (DG.gdseq like '%" + tbxGOODS.Text + "%' or DG.zjm like '%" + tbxGOODS.Text + "%' or DG.gdname like '%" + tbxGOODS.Text + "%')";

                strWhere += string.Format(" AND TB.DEPTID in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);

                if (strWhere != " ") strDeptOut = strDeptOut + strWhere;
                strSql = string.Format(strDeptOut, StartTime, EndTime, StartTime);
            }
            else if(TabStrip1.ActiveTabIndex == 1)
            {
                if (!PubFunc.StrIsEmpty(ddlDEPTID.SelectedValue)) strWhere += " and TB.DEPTID = '" + ddlDEPTID2.SelectedValue + "'";

                if (!PubFunc.StrIsEmpty(ddlCATID.SelectedValue)) strWhere += " and DG.CATID = '" + ddlCATID2.SelectedValue + "'";

                if (!PubFunc.StrIsEmpty(tbxGOODS.Text)) strWhere += " and (DG.gdseq like '%" + tbxGOODS2.Text + "%' or DG.zjm like '%" + tbxGOODS2.Text + "%' or DG.gdname like '%" + tbxGOODS2.Text + "%')";

                strWhere += string.Format(" AND TB.DEPTID in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);

                if (strWhere != " ") strDeptIn = strDeptIn + strWhere;
                strSql = string.Format(strDeptIn, StartTime, EndTime, StartTime);

            }

            //strSql += string.Format(" ORDER BY {0} {1}", GridList.SortField, GridList.SortDirection);
            strSql += " ORDER BY GDNAME ASC";
            return strSql;
        }

        protected void btSearch_Click(object sender, EventArgs e)
        {
            DataSearch();
        }
        private void DataSearch()
        {

            if (RQXZ.Text.Trim() == "" || RQXZ.Text.Length != 7)
            {
                Alert.Show("请选择有效时间！");
                return;
            }

            DateTime time = DateTime.Parse(RQXZ.Text + "-01");
            int total = 0;
            DataTable dtData = PubFunc.DbGetPage(GridList.PageIndex, GridList.PageSize, GetSearchSql(time.AddDays(-1).ToString("yyyyMMdd"), time.AddMonths(1).AddDays(-1).ToString("yyyyMMdd")), ref total);
            GridList.RecordCount = total;
            GridList.DataSource = dtData;
            GridList.DataBind();
            OutputSummaryData(dtData);
        }
        protected void GridList_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridList.PageIndex = e.NewPageIndex;
            DataSearch();
        }
        public static void CleanItemForDDL(string CleanName, FineUIPro.DropDownList dropDownList)
        {
            //用于取消下拉表中不需要的选项
            int GetItem = 0;
            foreach (FineUIPro.ListItem item in dropDownList.Items)
            {
                if (item.Text == CleanName)
                {
                    dropDownList.Items.RemoveAt(GetItem);
                    break;
                }
                else
                {
                    if (item.Value == CleanName)
                    {
                        dropDownList.Items.RemoveAt(GetItem);
                        break;
                    }
                }
                GetItem++;
            }
        }
        protected void btExport_Click(object sender, EventArgs e)
        {
            if (RQXZ.Text.Trim() == "" || RQXZ.Text.Length != 7)
            {
                Alert.Show("请选择有效时间！");
                return;
            }
            string time = RQXZ.Text;
            string year = time.Substring(0, 4);
            int yearnum = Convert.ToInt32(year);
            string month = time.Substring(5, 2);
            int monthnum = Convert.ToInt32(month);
            if (yearnum < 2000 || yearnum > 9999 || monthnum < 1 || monthnum > 12)
            {
                Alert.Show("请选择有效时间！");
                return;
            }
            DateTime dtSj = DateTime.Parse(RQXZ.Text + "-01");
            DataTable dt = DbHelperOra.Query(GetSearchSql(dtSj.AddDays(-1).ToString("yyyyMMdd"), dtSj.AddMonths(1).AddDays(-1).ToString("yyyyMMdd"))).Tables[0];

            if (dt == null || dt.Rows.Count == 0)
            {
                Alert.Show("暂时没有符合要求的数据，无法导出", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            ExcelHelper.ExportByWeb(dt, "库房商品数量进销存明细报表", string.Format("库房商品数量进销存明细报表_{0}.xls", DateTime.Now.ToString("yyyyMMdd")));
            btnExport.Enabled = true;
        }

        protected void GridList_Sort(object sender, GridSortEventArgs e)
        {
            GridList.SortDirection = e.SortDirection;
            GridList.SortField = e.SortField;

            DataSearch();
        }

        private void OutputSummaryData(DataTable source)
        {
            int QCKCSLSUM = 0;
            int CGRKSUM = 0;
            int KSTHSUM = 0;
            int PYRKSUM = 0;
            int DBRKSUM = 0;
            int KFCKSUM = 0;
            int THCKSUM = 0;
            int PKCKSUM = 0;
            int DBCKSUM = 0;
            int QMKCSLSUM = 0;
            foreach (DataRow row in source.Rows)
            {
                QCKCSLSUM += Convert.ToInt32(row["QCKCSL"]);
                CGRKSUM += Convert.ToInt32(row["CGRK"]);
                KSTHSUM += Convert.ToInt32(row["KSTH"]);
                PYRKSUM += Convert.ToInt32(row["PYRK"]);
                DBRKSUM += Convert.ToInt32(row["DBRK"]);
                KFCKSUM += Convert.ToInt32(row["KFCK"]);
                THCKSUM += Convert.ToInt32(row["THCK"]);
                PKCKSUM += Convert.ToInt32(row["PKCK"]);
                DBCKSUM += Convert.ToInt32(row["DBCK"]);
                QMKCSLSUM += Convert.ToInt32(row["QMKCSL"]);
            }
            JObject summary = new JObject();
            summary.Add("GDSEQ", "汇总：");
            summary.Add("QCKCSL", QCKCSLSUM.ToString());
            summary.Add("CGRK", CGRKSUM.ToString());
            summary.Add("KSTH", KSTHSUM.ToString());
            summary.Add("PYRK", PYRKSUM.ToString());
            summary.Add("DBRK", DBRKSUM.ToString());
            summary.Add("KFCK", KFCKSUM.ToString());
            summary.Add("THCK", THCKSUM.ToString());
            summary.Add("PKCK", PKCKSUM.ToString());
            summary.Add("DBCK", DBCKSUM.ToString());
            summary.Add("QMKCSL", QMKCSLSUM.ToString());
            GridList.SummaryData = summary;

        }

        protected void btnClearDept_Click(object sender, EventArgs e)
        {

        }

        protected void btnExportDept_Click(object sender, EventArgs e)
        {

        }

        protected void btnSearchDept_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ddlDEPTID2.SelectedValue))
            {
                DataSearchDept();
            }
            else
            {
                Alert.Show("请选择科室", MessageBoxIcon.Warning);
                return;
            }
            
        }
        private void DataSearchDept()
        {

            if (RQXZ2.Text.Trim() == "" || RQXZ2.Text.Length != 7)
            {
                Alert.Show("请选择有效时间！");
                return;
            }

            DateTime time = DateTime.Parse(RQXZ2.Text + "-01");
            int total = 0;
            DataTable dtData = PubFunc.DbGetPage(GridListDept.PageIndex, GridListDept.PageSize, GetSearchSql(time.AddDays(-1).ToString("yyyyMMdd"), time.AddMonths(1).AddDays(-1).ToString("yyyyMMdd")), ref total);
            GridListDept.RecordCount = total;
            GridListDept.DataSource = dtData;
            GridListDept.DataBind();
            OutputSummaryData2(dtData);
        }
        private void OutputSummaryData2(DataTable source)
        {
            int QCKCSLSUM = 0;
            int LCDRKSUM = 0;
            int DSCRKSUM = 0;
            int CKDRKSUM = 0;
            int DBDRKSUM = 0;
            int PYRKSUM = 0;
            int LTDCKSUM = 0;
            int XSTCKSUM = 0;
            int DBDCKSUM = 0;
            int PKCKSUM = 0;
            int QMKCSLSUM = 0;
            foreach (DataRow row in source.Rows)
            {
                QCKCSLSUM += Convert.ToInt32(row["QCKCSL"]);
                LCDRKSUM += Convert.ToInt32(row["LCDRK"]);
                DSCRKSUM += Convert.ToInt32(row["DSCRK"]);
                PYRKSUM += Convert.ToInt32(row["PYRK"]);
                CKDRKSUM += Convert.ToInt32(row["CKDRK"]);
                DBDRKSUM += Convert.ToInt32(row["DBRK"]);
                LTDCKSUM += Convert.ToInt32(row["LTDCK"]);
                XSTCKSUM += Convert.ToInt32(row["XSTCK"]);
                DBDCKSUM += Convert.ToInt32(row["DBCK"]);
                PKCKSUM += Convert.ToInt32(row["PKCK"]);
                QMKCSLSUM += Convert.ToInt32(row["QMKCSL"]);
            }
            JObject summary = new JObject();
            summary.Add("GDSEQ", "汇总：");
            summary.Add("QCKCSL", QCKCSLSUM.ToString());
            summary.Add("LCDRK", LCDRKSUM.ToString());
            summary.Add("DSCRK", DSCRKSUM.ToString());
            summary.Add("PYRK", PYRKSUM.ToString());
            summary.Add("CKDRK", CKDRKSUM.ToString());
            summary.Add("DBRK", DBDRKSUM.ToString());
            summary.Add("LTDCK", LTDCKSUM.ToString());
            summary.Add("XSTCK", XSTCKSUM.ToString());
            summary.Add("DBCK", DBDCKSUM.ToString());
            summary.Add("PKCK", PKCKSUM.ToString());
            summary.Add("QMKCSL", QMKCSLSUM.ToString());
            GridListDept.SummaryData = summary;

        }
        protected void GridListDept_PageIndexChange(object sender, GridPageEventArgs e)
        {

        }
    }
}