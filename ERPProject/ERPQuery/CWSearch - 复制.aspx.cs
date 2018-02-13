﻿using FineUIPro;
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
    public partial class CWSearch : PageBase
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
            //PubFunc.DdlDataGet(ddlDEPTID, "DDL_SYS_DEPTDEF");
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-30);
            lstLRRQ2.SelectedDate = DateTime.Now;
            dpkBegRQ.SelectedDate = DateTime.Now.AddDays(-30);
            dpkEndRQ.SelectedDate = DateTime.Now;
            //PubFunc.DdlDataGet(ddlSupplier, "DDL_DOC_SUPID");
        }

        private string GetSearchSql()
        {

            string strSql = @"SELECT K.DEPTID, K.DEPTNAME, K.SUMGZ GZHC, K.SUMPH YLYP, (K.SUMGZ + K.SUMPH) HJ
  FROM (SELECT SD.CODE DEPTID,
               SD.NAME DEPTNAME,
               NVL(M.GZHSJE, 0) SUMGZ,
               NVL(M.PHHSJE, 0) SUMPH
          FROM SYS_DEPT SD,
               (SELECT SD.CODE,
                       SD.NAME,
                       SUM(DECODE(DG.ISGZ,
                                  'Y',
                                  DECODE(DGJ.KCADD,
                                         -1,
                                         DECODE(DGJ.BILLTYPE,
                                                'XSD',
                                                DGJ.HSJE * DGJ.KCADD,
                                                'XSG',
                                                DGJ.HSJE * DGJ.KCADD,0),
                                         1,
                                         DECODE(DGJ.BILLTYPE, 'XST', DGJ.HSJE*(-1),0)))) GZHSJE,
                       SUM(DECODE(DG.ISGZ,
                                  'N',
                                  DECODE(DGJ.KCADD,
                                         -1,
                                         DECODE(DGJ.BILLTYPE,
                                                'XSD',
                                                DGJ.HSJE * DGJ.KCADD,
                                                'XSG',
                                                DGJ.HSJE * DGJ.KCADD,0),
                                         1,
                                         DECODE(DGJ.BILLTYPE, 'XST', DGJ.HSJE*(-1),0)))) PHHSJE
                  FROM DAT_GOODSJXC DGJ, DOC_GOODS DG, SYS_DEPT SD
                 WHERE DGJ.GDSEQ = DG.GDSEQ
                   AND DGJ.DEPTID = SD.CODE
                   AND DGJ.BILLTYPE IN ('XSD', 'XST', 'XSG')
                   AND SD.TYPE <> '1' AND DG.FLAG='Y'
                   AND DGJ.RQSJ>=TO_DATE('{0}','YYYY-MM-DD') AND DGJ.RQSJ<=TO_DATE('{1}','YYYY-MM-DD')+1
                 GROUP BY CODE, NAME) M
         WHERE SD.CODE = M.CODE(+)
         ORDER BY M.CODE) K
                         ";
            string strWhere = " ";
            strSql = string.Format(strSql, dpkBegRQ.Text, dpkEndRQ.Text);

            //if (!PubFunc.StrIsEmpty(ddlDEPTID.SelectedValue)) strWhere += " AND M.CODE='"+ddlDEPTID.SelectedValue+"' ";

            //if (strWhere != " ") strSql = strSql + strWhere;
            //strSql = string.Format(strSql,lstLRRQ1.Text,lstLRRQ2.Text,ddlISGZ.SelectedValue);
            //strSql += string.Format(" GROUP BY M.CODE,M.NAME ORDER BY {0} {1} ", GridGoods.SortField, GridGoods.SortDirection);
            return strSql;
            
        }

        private string GetSearchSql1()
        {
            string strSql = @"SELECT K.DEPTID, K.DEPTNAME, K.SUMGZ GZHC, K.SUMPH YLYP, (K.SUMGZ + K.SUMPH) HJ
  FROM (SELECT SD.CODE DEPTID,
               SD.NAME DEPTNAME,
               NVL(M.GZHSJE, 0) SUMGZ,
               NVL(M.PHHSJE, 0) SUMPH
          FROM SYS_DEPT SD,
               (SELECT SD.CODE,
                       SD.NAME,
                       SUM(DECODE(DG.ISGZ,
                                  'Y',
                                  DECODE(DGJ.KCADD,
                                         -1,
                                         DECODE(DGJ.BILLTYPE,
                                                'XSD',
                                                DGJ.HSJE * DGJ.KCADD,
                                                'XSG',
                                                DGJ.HSJE * DGJ.KCADD),
                                         1,
                                         DECODE(DGJ.BILLTYPE, 'XST', DGJ.HSJE*(-1))))) GZHSJE,
                       SUM(DECODE(DG.ISGZ,
                                  'N',
                                  DECODE(DGJ.KCADD,
                                         -1,
                                         DECODE(DGJ.BILLTYPE,
                                                'XSD',
                                                DGJ.HSJE * DGJ.KCADD,
                                                'XSG',
                                                DGJ.HSJE * DGJ.KCADD),
                                         1,
                                         DECODE(DGJ.BILLTYPE, 'XST', DGJ.HSJE*(-1))))) PHHSJE
                  FROM DAT_GOODSJXC DGJ, DOC_GOODS DG, SYS_DEPT SD
                 WHERE DGJ.GDSEQ = DG.GDSEQ
                   AND DGJ.DEPTID = SD.CODE
                   AND DGJ.BILLTYPE IN ('XSD', 'XST', 'XSG')
                   AND SD.TYPE <> '1' AND DG.FLAG='Y'
                   AND DGJ.RQSJ>=TO_DATE('{0}','YYYY-MM-DD') AND DGJ.RQSJ<=TO_DATE('{1}','YYYY-MM-DD')+1
                 GROUP BY CODE, NAME) M
         WHERE SD.CODE = M.CODE(+)
         ORDER BY M.CODE) K

           
           
";
            string strWhere = " ";
            strSql = string.Format(strSql, dpkBegRQ.Text,dpkEndRQ.Text);
         
            if (strWhere != " ") strSql = strSql + strWhere;

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
            DataTable dtData = DbHelperOra.QueryForTable(GetSearchSql());
            ExcelHelper.ExportByWeb(dtData.DefaultView.ToTable(true), "财务对账查询", string.Format("财务对账查询_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
            //((FineUIPro.Button)sender).Enabled = true;
        }

        protected void GridGoods_Sort(object sender, GridSortEventArgs e)
        {
            GridGoods.SortDirection = e.SortDirection;
            GridGoods.SortField = e.SortField;

            DataSearch();
        }

     


        private void OutputSummaryData(DataTable source,DataTable dttotal)
        {
            decimal HSJJTotal = 0, HSJETotal = 0,TOTALSL=0,TOTALJE=0,THJ=0,TTHJ=0;
            foreach (DataRow row in source.Rows)
            {
                HSJJTotal += Convert.ToInt32(row["GZHC"]);
                HSJETotal += Convert.ToDecimal(row["YLYP"]);
                THJ += Convert.ToDecimal(row["HJ"]);
            }
            foreach (DataRow dr in dttotal.Rows)
            {
                TOTALSL += Convert.ToDecimal(dr["GZHC"]);
                TOTALJE += Convert.ToDecimal(dr["YLYP"]);
                TTHJ+=Convert.ToDecimal(dr["HJ"]);
            }
            JObject summary = new JObject();
            summary.Add("DEPTNAME", "分页合计</br>全部合计");
            summary.Add("GZHC", HSJJTotal.ToString("F2")+"</br>"+TOTALSL.ToString("F2"));
            summary.Add("YLYP", HSJETotal.ToString("F2")+"</br>"+TOTALJE.ToString("F2"));
            summary.Add("HJ", THJ.ToString("F2") + "</br>" + TTHJ.ToString("F2"));

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

        protected void btnExport1_Click(object sender, EventArgs e)
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
            DataTable dtData = DbHelperOra.Query(GetSearchSql1()).Tables[0];
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

            ExcelHelper.ExportByWeb(dtData.DefaultView.ToTable(true, columnNames), "计费与非计费商品明细", string.Format("计费与非计费商品明细_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
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

            DataTable dtData = DbHelperOra.Query(GetSearchSql1()).Tables[0];
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