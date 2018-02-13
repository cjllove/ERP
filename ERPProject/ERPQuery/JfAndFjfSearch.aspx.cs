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
    public partial class JfAndFjfSearch : PageBase
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
            PubFunc.DdlDataGet(ddlDEPTID, "DDL_SYS_DEPTDEF");
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-30);
            lstLRRQ2.SelectedDate = DateTime.Now;
            dpkBegRQ.SelectedDate = DateTime.Now.AddDays(-30);
            dpkEndRQ.SelectedDate = DateTime.Now;
            //PubFunc.DdlDataGet(ddlSupplier, "DDL_DOC_SUPID");
        }

        private string GetSearchSql()
        {

            string strSql = @"SELECT M.CODE DEPTID,
                               M.NAME DEPTNAME,
                               SUM(DECODE(M.ISJF, 'Y', M.SL, 0)) JFSL,
                               SUM(DECODE(M.ISJF, 'N', M.SL, 0)) FJFSL,
                               SUM(M.SL) SL,
                               SUM(DECODE(M.ISJF, 'Y', M.JE, 0)) JFJE,
                               SUM(DECODE(M.ISJF, 'N', M.JE, 0)) FJFJE,
                               SUM(M.JE) JE
                          FROM (SELECT DGJ.BILLNO,
                                       SD.CODE,
                                       SD.NAME,
                                       DECODE(DGJ.KCADD,
                                              -1,
                                              DECODE(DGJ.BILLTYPE,
                                                     'XSD',
                                                     DGJ.SL * DGJ.KCADD,
                                                     'XSG',DGJ.SL*DGJ.KCADD),1,DECODE(DGJ.BILLTYPE,'XST',DGJ.SL*(-1))) SL,
                                       NVL(DG.STR6,'N') ISJF,
                                       DECODE(DGJ.KCADD,
                                              -1,
                                              DECODE(DGJ.BILLTYPE,
                                                     'XSD',
                                                     DGJ.HSJE * DGJ.KCADD,
                                                     'XSG',DGJ.HSJE*DGJ.KCADD),1,DECODE(DGJ.BILLTYPE,'XST',DGJ.HSJE*(-1))) JE
                                  FROM DAT_GOODSJXC DGJ, DOC_GOODS DG, DOC_GOODSCFG DGC, SYS_DEPT SD
                                 WHERE DGJ.DEPTID = SD.CODE
                                   AND DGJ.GDSEQ = DG.GDSEQ
                                   AND DGJ.GDSEQ = DGC.GDSEQ
                                   AND DGJ.DEPTID = DGC.DEPTID
                                   AND DG.FLAG = 'Y'
                                   AND DGC.ISCFG = 'Y'
                                   AND SD.FLAG = 'Y'
                                   AND DGJ.BILLTYPE IN ('XSD', 'XST','XSG')
                                   AND SD.TYPE <> '1'
                                   AND DGJ.RQSJ >=TO_DATE('{0}','YYYY-MM-DD') AND DGJ.RQSJ<=TO_DATE('{1}','YYYY-MM-DD')+1
                                   AND DG.ISGZ LIKE NVL('{2}','%')) M
                         WHERE M.SL IS NOT NULL

                         ";
            string strWhere = " ";

            if (!PubFunc.StrIsEmpty(ddlDEPTID.SelectedValue)) strWhere += " AND M.CODE='"+ddlDEPTID.SelectedValue+"' ";

            if (strWhere != " ") strSql = strSql + strWhere;
            strSql = string.Format(strSql,lstLRRQ1.Text,lstLRRQ2.Text,ddlISGZ.SelectedValue);
            strSql += string.Format(" GROUP BY M.CODE,M.NAME ORDER BY {0} {1} ", GridGoods.SortField, GridGoods.SortDirection);
            return strSql;
            
        }

        private string GetSearchSql1()
        {
            string strSql = @"SELECT DEPTID,DEPTNAME,GDSEQ,GDNAME,GDSPEC,UNIT,ROUND(HSJJ,4)HSJJ,SUM(SL)SL,SUM(JE)JE FROM(
                              SELECT M.DEPTID,M.DEPTNAME,M.GDSEQ,M.GDNAME,M.GDSPEC,M.UNIT,M.HSJJ,M.SL,M.JE,DECODE(M.ISJF,'Y','是','N','否')ISJF FROM(
                                    SELECT DGJ.BILLNO,
                                                   SD.CODE DEPTID,
                                                   SD.NAME DEPTNAME,
                                                   DGJ.GDSEQ,
                                                   DG.GDNAME,
                                                   DG.GDSPEC,DGJ.HSJJ,
                                                   f_getunitname(DG.UNIT)UNIT,
                                                   DECODE(DGJ.KCADD,
                                                          -1,
                                                          DECODE(DGJ.BILLTYPE,
                                                                 'XSD',                                                                
                                                                 DGJ.SL*DGJ.KCADD,
                                                                  'XSG',DGJ.SL*DGJ.KCADD),1,DECODE(DGJ.BILLTYPE,'XST', DGJ.SL * DGJ.KCADD*(-1))
                                                                 ) SL,
                                                   NVL(DG.STR6,'N') ISJF,
                                                   DECODE(DGJ.KCADD,
                                                          -1,
                                                          DECODE(DGJ.BILLTYPE,
                                                                 'XSD',
                                                                 DGJ.HSJE * DGJ.KCADD,                                                                
                                                                 'XSG',DGJ.HSJE*DGJ.KCADD),1,DECODE(DGJ.BILLTYPE,'XST',DGJ.HSJE*(-1))) JE
                                              FROM DAT_GOODSJXC DGJ, DOC_GOODS DG, DOC_GOODSCFG DGC, SYS_DEPT SD
                                             WHERE DGJ.DEPTID = SD.CODE
                                               AND DGJ.GDSEQ = DG.GDSEQ
                                               AND DGJ.GDSEQ = DGC.GDSEQ
                                               AND DGJ.DEPTID = DGC.DEPTID
                                               AND DG.FLAG = 'Y'
                                               AND DGC.ISCFG = 'Y'
                                               AND SD.FLAG = 'Y'
                                               AND DGJ.BILLTYPE IN ('XSD', 'XST','XSG')
                                               AND SD.TYPE <> '1'
                                               AND DGJ.RQSJ >= TO_DATE('{0}', 'YYYY-MM-DD')
                                               AND DGJ.RQSJ <= TO_DATE('{1}', 'YYYY-MM-DD')+1
                                               )M WHERE M.SL IS NOT NULL AND M.ISJF LIKE NVL('{2}','%') 
           
           
";
            string strWhere = " ";
            strSql = string.Format(strSql, dpkBegRQ.Text,dpkEndRQ.Text,ddlISJF.SelectedValue);
            if (!PubFunc.StrIsEmpty(hfdDEPTID.Text)) strWhere += " AND M.DEPTID='"+hfdDEPTID.Text+"' ";
            if (!PubFunc.StrIsEmpty(trbSearch.Text)) strWhere += " and (M.GDSEQ like '%" + trbSearch.Text + "%'  or M.gdname like '%" + trbSearch.Text + "%')";

            if (strWhere != " ") strSql = strSql + strWhere+"  )GROUP BY DEPTID,DEPTNAME,GDSEQ,GDNAME,GDSPEC,UNIT,HSJJ ";

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
            string sql = "", headname = "";
            if (TabStrip1.ActiveTabIndex == 0)
            {
                if (GridCom.Rows.Count < 1)
                {
                    Alert.Show("没有数据，无法导出！");
                    return;
                }
                sql = GetSearchSql1();
                headname = "计费与非计费商品明细";
                string[,] col = { { "DEPTID", "DEPTNAME", "GDSEQ", "GDNAME", "GDSPEC", "UNIT", "HSJJ", "SL", "JE" }, { "科室编码", "科室名称", "商品编码", "商品名称", "规格", "单位", "单价", "数量", "金额" } };
                DataTable dtData = DbHelperOra.QueryForTable(sql);
                ExcelHelper.ExportByWeb(dtData.DefaultView.ToTable(true), headname, string.Format(headname + "_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")), col);
            }
            else
            {
                if (GridGoods.Rows.Count < 1)
                {
                    Alert.Show("没有数据，无法导出！");
                    return;
                }
                sql = GetSearchSql();
                headname = "计费与非计费商品汇总";
                string[,] col = { { "DEPTID", "DEPTNAME", "JFSL", "FJFSL", "SL", "JFJE", "FJFJE", "JE" }, { "科室编码", "科室名称", "计费总数", "非计费总数", "总数量", "计费总额", "非计费总额", "总金额"} };

                DataTable dtData = DbHelperOra.QueryForTable(sql);
                ExcelHelper.ExportByWeb(dtData.DefaultView.ToTable(true), headname, string.Format(headname + "_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")), col);
            }
        }

        protected void GridGoods_Sort(object sender, GridSortEventArgs e)
        {
            GridGoods.SortDirection = e.SortDirection;
            GridGoods.SortField = e.SortField;

            DataSearch();
        }

        //protected void SELECTSUPNAME_TextChanged(object sender, EventArgs e)
        //{
        //    if (SELECTSUPNAME.Text.Length == 0)
        //    {
        //        SELECTSUPID.Text = "";
        //    }
        //}

        //protected void SELECTPRODUCERNAME_TextChanged(object sender, EventArgs e)
        //{
        //    if (SELECTPRODUCERNAME.Text.Length == 0)
        //    {
        //        SELECTPRODUCERID.Text = "";
        //    }
        //}


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
            summary.Add("DEPTNAME", "分页合计</br>全部合计");
            summary.Add("SL", HSJJTotal.ToString("F2")+"</br>"+TOTALSL.ToString("F2"));
            summary.Add("JE", HSJETotal.ToString("F2")+"</br>"+TOTALJE.ToString("F2"));
            GridGoods.SummaryData = summary;
        }

        private void OutputSummaryData1(DataTable source)
        {
            decimal HSJJTotal = 0, HSJETotal = 0;
            foreach (DataRow row in source.Rows)
            {
                HSJJTotal += Convert.ToInt32(row["SL"]);
                HSJETotal += Convert.ToDecimal(row["JE"]);
            }
            JObject summary = new JObject();
            summary.Add("COUNTTITLE", "全部合计");
            summary.Add("HJSL", HSJJTotal.ToString("F2"));
            summary.Add("HJJE", HSJETotal.ToString("F2"));
            GridCom.SummaryData = summary;
        }

        protected void btnClear1_Click(object sender, EventArgs e)
        {
            PubFunc.FormDataClear(FormSearch);
            dpkBegRQ.SelectedDate = DateTime.Now.AddDays(-30);
            dpkEndRQ.SelectedDate = DateTime.Now;
        }

      

        protected void btnSearch1_Click(object sender, EventArgs e)
        {
            if (dpkBegRQ.SelectedDate == null || dpkEndRQ.SelectedDate == null)
            {
                Alert.Show("请输入条件【查询期间】！");
                return;
            }
            else if (dpkBegRQ.SelectedDate > dpkEndRQ.SelectedDate)
            {
                Alert.Show("开始日期大于结束日期，请重新输入！");
                return;
            }
            DataTable dtData = DbHelperOra.QueryForTable(GetSearchSql1());
            OutputSummaryData1(dtData);
            GridCom.DataSource = dtData;
            GridCom.DataBind();
        }
        protected void GridGoods_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            hfdDEPTID.Text=GridGoods.DataKeys[e.RowIndex][0].ToString();
            dpkBegRQ.SelectedDate = lstLRRQ1.SelectedDate;
            dpkEndRQ.SelectedDate = lstLRRQ2.SelectedDate;
            btnSearch1_Click(null,null);
            TabStrip1.ActiveTabIndex = 0;

        }
    }
}