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
    public partial class XSSearch : PageBase
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
            PubFunc.DdlDataGet("DDL_SYS_DEPTDEF", ddlDEPTID, ddlDEPTIDZ);
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-30);
            lstLRRQ2.SelectedDate = DateTime.Now;
            lstBEGRQ.SelectedDate = DateTime.Now.AddDays(-30);
            lstENDRQ.SelectedDate = DateTime.Now;
            //PubFunc.DdlDataSql(ddlWZLB, "SELECT ''CODE,'--请选择--' FROM DUAL UNION ALL SELECT  CODE, NAME              FROM SYS_CATEGORY WHERE FLAG='Y'");
            PubFunc.DdlDataGet("DDL_DOC_SUPID", ddlSupplier, ddlSupplierZ);
        }

        private string GetSearchSql()
        {
            string strSql = @"SELECT * FROM (SELECT DGJ.RQSJ,DGJ.BILLNO,f_get_billtypename(DGJ.BILLTYPE) BILLTYPE,
                                DGJ.ROWNO,DG.GDSEQ,DG.GDNAME,DG.GDSPEC,F_GETUNITNAME(DG.UNIT) UNIT,
                                SD.NAME DEPTNAME,f_getsupname(DGJ.SUPID) SUPNAME,
                                f_getsupname(DGJ.PSSID) PSSNAME,DGJ.HSJJ,
                                  DECODE(DGJ.KCADD,
                                                  -1,
                                                  DECODE(DGJ.BILLTYPE,
                                                         'DST',
                                                         DGJ.SL,
                                                        'XSG',ABS(DGJ.SL),
                                                         'XSD',
                                                         ABS(DGJ.SL),
                                                         0),
                                                  1,
                                                  DECODE(DGJ.BILLTYPE,
                                                         'DSC',
                                                         DGJ.SL,
                                                         'XST',
                                                          DGJ.SL*DGJ.KCADD*(-1),
                                                         0)) SL,
                                DECODE(DGJ.KCADD,
                                                                      -1,
                                                                      DECODE(DGJ.BILLTYPE,
                                                                             'DST',
                                                                             DGJ.HSJJ * DGJ.SL,
                                                                              'XSG',
                                                                               ABS(DGJ.HSJJ*DGJ.SL),
                                                                             'XSD',
                                                                             ABS(DGJ.HSJJ * DGJ.SL),0),
                                                                      1,
                                                                      DECODE(DGJ.BILLTYPE,'DSC',
                                                                             DGJ.HSJJ * DGJ.SL,'XST',DGJ.HSJJ*DGJ.SL*(-1),0))JE,DGJ.PH,DGJ.PZWH,TO_CHAR(DGJ.RQ_SC,'YYYY-MM-DD')RQ_SC,TO_CHAR(DGJ.YXQZ,'YYYY-MM-DD')YXQZ
                                                      FROM DAT_GOODSJXC DGJ,DOC_GOODS DG,SYS_DEPT SD
                                                     WHERE DGJ.BILLTYPE IN ('XSD', 'DST', 'DSC','XST','XSG') AND DGJ.GDSEQ=DG.GDSEQ AND DGJ.DEPTID=SD.CODE
                                                     AND SD.TYPE<>'1' 
                                                        ";
            string strWhere = " ";

            if (!PubFunc.StrIsEmpty(ddlDEPTID.SelectedValue)) strWhere += " AND DGJ.DEPTID = '" + ddlDEPTID.SelectedValue + "'";
            if (!PubFunc.StrIsEmpty(tbxGOODS.Text)) strWhere += " and (DG.GDSEQ like '%" + tbxGOODS.Text + "%' or DG.zjm like '%" + tbxGOODS.Text + "%' or DG.gdname like '%" + tbxGOODS.Text + "%')";
            if (!PubFunc.StrIsEmpty(ddlSupplier.SelectedValue)) strWhere += " AND DGJ.SUPID='"+ddlSupplier.SelectedValue+"' ";
            if (!PubFunc.StrIsEmpty(ddlBilltype.SelectedValue)) strWhere += " AND DGJ.BILLTYPE = '" + ddlBilltype.SelectedValue + "'";
            if (!PubFunc.StrIsEmpty(ddlISGZ.SelectedValue)) strWhere += " AND DG.ISGZ = '" + ddlISGZ.SelectedValue + "'";

            strWhere += string.Format(" and DGJ.RQSJ>=TO_DATE('{0}','YYYY-MM-DD') and DGJ.RQSJ < TO_DATE('{1}','YYYY-MM-DD')+1 ", lstLRRQ1.Text, lstLRRQ2.Text);

            if (strWhere != " ") strSql = strSql + strWhere;

            strSql += " ) WHERE SL<>0 ORDER BY RQSJ,BILLNO,ROWNO ";
            return strSql;
            
        }

        private void DataSearch()
        {
            int total = 0;

            DataTable dtData = PubFunc.DbGetPage(GridGoods.PageIndex, GridGoods.PageSize, GetSearchSql(), ref total);
            DataTable dttotal = DbHelperOra.QueryForTable(GetSearchSql());
            OutputSummaryData(dtData,dttotal);
            GridGoods.RecordCount = total;
            GridGoods.DataSource = dtData;
            GridGoods.DataBind();
        }

        protected void GridGoods_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridGoods.PageIndex = e.NewPageIndex;
            DataSearch();
        }
        protected void btSearch_Click(object sender, EventArgs e)
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【查询期间】！");
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                Alert.Show("开始日期大于结束日期，请重新输入！");
                return;
            }
            DataSearch();
        }
        protected void btClear_Click(object sender, EventArgs e)
        {
            PubFunc.FormDataClear(FormUser);
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-30);
            lstLRRQ2.SelectedDate = DateTime.Now;

        }
        protected void btExport_Click(object sender, EventArgs e)
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【查询期间】！");
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                Alert.Show("开始日期大于结束日期，请重新输入！");
                return;
            }
            DataTable dtData = DbHelperOra.Query(GetSearchSql()).Tables[0];
            if (dtData == null || dtData.Rows.Count == 0)
            {
                Alert.Show("没有数据,无法导出！");
                return;
            }
            string[] columnNames = new string[GridGoods.Columns.Count - 1];
            for (int index = 1; index < GridGoods.Columns.Count; index++)
            {
                GridColumn column = GridGoods.Columns[index];
                if (column is FineUIPro.BoundField)
                {
                    dtData.Columns[((FineUIPro.BoundField)(column)).DataField.ToUpper()].ColumnName = column.HeaderText;
                    columnNames[index - 1] = column.HeaderText;
                }
            }
            ExcelHelper.ExportByWeb(dtData.DefaultView.ToTable(true, columnNames), "销售对账明细报表", string.Format("销售对账明细报表_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }

        protected void GridGoods_Sort(object sender, GridSortEventArgs e)
        {
            GridGoods.SortDirection = e.SortDirection;
            GridGoods.SortField = e.SortField;

            DataSearch();
        }
        private void OutputSummaryData(DataTable source,DataTable dttotal)
        {
            decimal HSJJTotal = 0, HSJETotal = 0,TOTALSL=0,TOTALJE=0;
            foreach (DataRow row in source.Rows)
            {
                HSJJTotal += Convert.ToInt32(row["SL"]);
                HSJETotal += Convert.ToDecimal(row["JE"]);
            }
            foreach (DataRow dr in dttotal.Rows)
            {
                TOTALSL += Convert.ToDecimal(dr["SL"]);
                TOTALJE += Convert.ToDecimal(dr["JE"]);
            }
            JObject summary = new JObject();
            summary.Add("GDNAME", "分页合计</br>全部合计");
            summary.Add("SL", HSJJTotal.ToString("F2")+"</br>"+TOTALSL.ToString("F2"));
            summary.Add("JE", HSJETotal.ToString("F2")+"</br>"+TOTALJE.ToString("F2"));
            GridGoods.SummaryData = summary;
        }
        protected  void billSearch()
        {
            string strSql = @"SELECT * FROM(SELECT DEPTID,DEPTNAME,SUPID,SUM(SL)SL,SUM(JE)JE FROM (SELECT DGJDEPTID,
                                SD.NAME DEPTNAME,DGJ.SUPID,f_getsupname(DGJ.SUPID) SUPNAME,
                                                      DECODE(DGJ.KCADD,
                                                  -1,
                                                  DECODE(DGJ.BILLTYPE,
                                                         'DST',
                                                         DGJ.SL,
                                                        'XSG',ABS(DGJ.SL),
                                                         'XSD',
                                                         ABS(DGJ.SL),
                                                         0),
                                                  1,
                                                  DECODE(DGJ.BILLTYPE,
                                                         'DSC',
                                                         DGJ.SL,
                                                         'XST',
                                                          DGJ.SL*DGJ.KCADD*(-1),
                                                         0)) SL,
                                DECODE(DGJ.KCADD,
                                                                      -1,
                                                                      DECODE(DGJ.BILLTYPE,
                                                                             'DST',
                                                                             DGJ.HSJJ * DGJ.SL,
                                                                              'XSG',
                                                                               ABS(DGJ.HSJJ*DGJ.SL),
                                                                             'XSD',
                                                                             ABS(DGJ.HSJJ * DGJ.SL),0),
                                                                      1,
                                                                      DECODE(DGJ.BILLTYPE,'DSC',
                                                                             DGJ.HSJJ * DGJ.SL,'XST',DGJ.HSJJ*DGJ.SL*(-1),0))JE,DGJ.PH,DGJ.PZWH,TO_CHAR(DGJ.RQ_SC,'YYYY-MM-DD')RQ_SC,TO_CHAR(DGJ.YXQZ,'YYYY-MM-DD')YXQZ
                                                      FROM DAT_GOODSJXC DGJ,DOC_GOODS DG,SYS_DEPT SD
                                                     WHERE DGJ.BILLTYPE IN ('XSD', 'DST', 'DSC','XST','XSG') AND DGJ.GDSEQ=DG.GDSEQ AND DGJ.DEPTID=SD.CODE
                                                     AND SD.TYPE<>'1'  )
                                                     GROUP BY DEPTID,DEPTNAME,SUPID )WHERE 1=1 
                                                        ";
            string strWhere = " ";
        }
        protected  void listRow_DoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            string SBILLNO=GridList.Rows[e.RowIndex].Values[1].ToString();
        }
        protected void GridList_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridList.PageIndex = e.NewPageIndex;
            billSearch();
        }
        protected void GridList_Sort(object sender, GridSortEventArgs e)
        {
            billSearch();
        }
    }
       
}