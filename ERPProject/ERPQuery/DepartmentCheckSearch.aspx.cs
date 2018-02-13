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
using System.Text;

namespace ERPProject.ERPQuery
{
    public partial class DepartmentCheckSearch : PageBase
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
            PubFunc.DdlDataGet(ddlSupplier, "DDL_DOC_SUPID");
        }

        private string GetSearchSql()
        {
            string strsql="";
            string sqlstring="";
            if (string.IsNullOrEmpty(lstLRRQ1.Text) || string.IsNullOrEmpty(lstLRRQ2.Text))
            {
                Alert.Show("时间查询不能为空！");
                return null;
            }
            if (lstLRRQ2.SelectedDate < lstLRRQ1.SelectedDate)
            {
                Alert.Show("结束时间不能小于开始时间！");
                return null;
            }
            string strSQL = @"SELECT A.GDSEQ,A.GDNAME,A.HSJJ,A.GDSPEC,A.SL,A.JE,NVL(B.TSL,0)TSL,NVL(B.TJE,0)TJE,(A.SL+NVL(B.TSL,0))TOTALSL,(A.JE+NVL(B.TJE,0))TOTALJE
                              FROM (SELECT E.GDSEQ,
                                           D.GDNAME,
                                           E.HSJJ,
                                           D.GDSPEC,
                                           SUM(E.SL) SL,
                                           SUM(E.HSJE) JE
                                      FROM DAT_CK_DOC A, DAT_CK_CHK B, DOC_GOODS D, DAT_GOODSJXC E
                                     WHERE A.SEQNO = B.SEQNO(+)
                                       AND A.SEQNO = E.BILLNO(+)
                                       AND E.GDSEQ = D.GDSEQ
                                       AND A.DEPTID = E.DEPTID
                                       AND INSTR('LCD,CKD,DSC', A.BILLTYPE) > 0
                                       AND E.SUPID LIKE NVL('{3}','%')
                                       AND INSTR('Y,G', A.FLAG) > 0
                                       AND DECODE(B.FLAG,
                                                  'Y',
                                                  '已收货',
                                                  DECODE(A.FLAG, 'Y', '已出库', 'G', '已出库', '未出库')) = '{2}'
                                       AND A.SHRQ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                           TO_DATE('{1}', 'YYYY-MM-DD') + 1
                                     GROUP BY E.GDSEQ, D.GDNAME, E.HSJJ, D.GDSPEC
                                     ORDER BY E.GDSEQ DESC) A,
                                   (SELECT GDSEQ,SUM(JFSL+FJFSL)TSL,SUM(JFJE+FJFJE)TJE FROM(SELECT A.GDSEQ,
                                           SUM(NVL(DECODE(B.STR6,
                                                      'Y',
                                                      DECODE(A.KCADD,
                                                             -1,
                                                             DECODE(A.BILLTYPE,
                                                                    'LTD',
                                                                    A.SL * A.KCADD * (-1),
                                                                    0)
                                 
                                                             ),
                                                      0),
                                               0)) JFSL,
                                           SUM(NVL(DECODE(B.STR6,
                                                      'Y',
                                                      DECODE(A.KCADD,
                                                             -1,
                                                             DECODE(A.BILLTYPE,
                                                                    'LTD',
                                                                    A.HSJE * A.KCADD * (-1),
                                                                    0)
                                 
                                                             ),
                                                      0),
                                               0)) JFJE,
                                           SUM(NVL(DECODE(B.STR6,
                                                      'N',
                                                      DECODE(A.KCADD,
                                                             -1,
                                                             DECODE(A.BILLTYPE,
                                                                    'XST',
                                                                    A.SL * A.KCADD * (-1),
                                                                    'DST',
                                                                    A.SL * A.KCADD * (-1),
                                                                    0)),
                                                      0),
                                               0)) FJFSL,
                                                 SUM(NVL(DECODE(B.STR6,
                                                      'N',
                                                      DECODE(A.KCADD,
                                                             -1,
                                                             DECODE(A.BILLTYPE,
                                                                    'XST',
                                                                    A.HSJE * A.KCADD * (-1),
                                                                    'DST',
                                                                    A.HSJE * A.KCADD * (-1),
                                                                    0)),
                                                      0),
                                               0)) FJFJE
                                      FROM DAT_GOODSJXC A, DOC_GOODS B, SYS_DEPT C
                                     WHERE A.GDSEQ = B.GDSEQ
                                       AND A.DEPTID = C.CODE
                                       AND C.TYPE <> 1
                                       AND A.BILLTYPE IN ('XST', 'LTD')
                                       AND A.SUPID LIKE NVL('{3}','%')
                                       AND A.RQSJ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                           TO_DATE('{1}', 'YYYY-MM-DD') + 1
                                       GROUP BY A.GDSEQ )GROUP BY GDSEQ) B
                             WHERE A.GDSEQ = B.GDSEQ(+)
                               AND A.SL > 0

";
           StringBuilder sql = new StringBuilder();
           sql.Append(@"SELECT * FROM (SELECT E.GDSEQ,D.GDNAME,E.HSJJ,D.GDSPEC,SUM(E.SL)SL,SUM(E.HSJE)JE
                                          FROM DAT_CK_DOC A, DAT_CK_CHK B,DOC_GOODS D,DAT_GOODSJXC E
                                         WHERE A.SEQNO = B.SEQNO(+) AND B.SEQNO=E.BILLNO(+)  AND E.GDSEQ=D.GDSEQ  AND A.DEPTID=E.DEPTID 
                                           AND INSTR('LCD,CKD,DSC',A.BILLTYPE) > 0  {0}
                                           AND INSTR('Y,G', A.FLAG) > 0 ");
           string flag = "";

            if (!PubFunc.StrIsEmpty(ddlISSH.SelectedValue))
            {
                if (ddlISSH.SelectedValue == "Y")
                {
                    flag = "已收货";
                }
                else if (ddlISSH.SelectedValue == "N")
                {
                    flag = "已出库";
                }
              

               
            }
          
          
            //sql.AppendFormat(" AND A.SHRQ BETWEEN TO_DATE('{0}','YYYY-MM-DD') AND TO_DATE('{1}','YYYY-MM-DD') + 1",
            //                           lstLRRQ1.Text, lstLRRQ2.Text);
            //sql.Append(" GROUP BY E.GDSEQ,D.GDNAME,E.HSJJ,D.GDSPEC ORDER BY E.GDSEQ DESC)WHERE SL>0");
            //if (!PubFunc.StrIsEmpty(ddlSupplier.SelectedValue))
            //    {
            //        strsql = string.Format("AND E.SUPID='{0}'", ddlSupplier.SelectedValue);
            //    }
            //    else
            //    {
            //        strsql = " AND 1=1 ";
            //    }
            sqlstring = string.Format(strSQL, lstLRRQ1.Text,lstLRRQ2.Text,flag,ddlSupplier.SelectedValue);
                return sqlstring.ToString();
               // return sql.ToString();
        }
            
        

        private string GetSearchSql1()
        {
            string strSql = @"SELECT K.DEPTID,
       K.DEPTNAME,
       K.SUMGZ GZHC,
       K.SUMPH YLYP,
       (K.SUMGZ + K.SUMPH) HJ
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
                                         DECODE(DGJ.HSJE * DGJ.KCADD,
                                                'XSG',
                                                DGJ.HSJE * DGJ.KCADD,
                                                0),
                                         1,
                                         DECODE(DGJ.BILLTYPE,
                                                'XST',
                                                DGJ.HSJE * (-1),
                                                0)))) GZHSJE,
                       SUM(DECODE(DG.ISGZ,
                                  'N',
                                  DECODE(DG.STR6,
                                         'Y',
                                         DECODE(DGJ.KCADD,
                                                -1,
                                                DECODE(DGJ.BILLTYPE,
                                                       'LTD',
                                                       DGJ.HSJE * DGJ.KCADD,
                                                       0),
                                                1,
                                                DECODE(DGJ.BILLTYPE,
                                                       'LCD',
                                                       DGJ.HSJE,
                                                       'CKD',
                                                       DGJ.HSJE,
                                                       0)),
                                         'N',
                                         DECODE(DGJ.KCADD,
                                                -1,
                                                DECODE(DGJ.BILLTYPE,
                                                       'XST',
                                                       DGJ.HSJE * DGJ.KCADD,
                                                       'DST',
                                                       DGJ.HSJE * DGJ.KCADD,
                                                       0),
                                                1,
                                                DECODE(DGJ.BILLTYPE,
                                                       'LCD',
                                                       DGJ.HSJE,
                                                       'CKD',
                                                       DGJ.HSJE,
                                                       'DSC',
                                                       DGJ.HSJE,
                                                       0))))) PHHSJE
                  FROM DAT_GOODSJXC DGJ, DOC_GOODS DG, SYS_DEPT SD
                 WHERE DGJ.GDSEQ = DG.GDSEQ
                   AND DGJ.DEPTID = SD.CODE
                   AND DGJ.BILLTYPE IN
                       ('CKD', 'LCD', 'LTD', 'DSC', 'DST', 'XST', 'XSG')
                   AND SD.TYPE <> '1'
                   AND DG.FLAG = 'Y'
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
            decimal SLTotal = 0, JETotal = 0, SLT = 0, JET = 0, TSL = 0, TJE = 0,TSLT = 0, TJET = 0, TOTALSL = 0, TOTALJE=0,TOTALSLT = 0, TOTALJET=0;
            foreach (DataRow row in source.Rows)
            {
                SLTotal += Convert.ToDecimal(row["SL"]);
                JETotal += Convert.ToDecimal(row["JE"]);
                TSL += Convert.ToDecimal(row["TSL"]);
                TJE += Convert.ToDecimal(row["TJE"]);
                TOTALSL+=Convert.ToDecimal(row["TOTALSL"]);
                TOTALJE += Convert.ToDecimal(row["TOTALJE"]);
            }
            foreach (DataRow dr in dttotal.Rows)
            {
                SLT += Convert.ToDecimal(dr["SL"]);
                JET += Convert.ToDecimal(dr["JE"]);
                TSLT += Convert.ToDecimal(dr["TSL"]);
                TJET += Convert.ToDecimal(dr["TJE"]);
                TOTALSLT += Convert.ToDecimal(dr["TOTALSL"]);
                TOTALJET += Convert.ToDecimal(dr["TOTALJE"]);
            }
            JObject summary = new JObject();
            summary.Add("GDNAME", "分页合计</br>全部合计");
            summary.Add("SL", SLTotal.ToString("F2") + "</br>" + SLT.ToString("F2"));
            summary.Add("JE", JETotal.ToString("F2") + "</br>" + JET.ToString("F2"));
            summary.Add("TSL", TSL.ToString("F2") + "</br>" + TSLT.ToString("F2")); ;
            summary.Add("TJE",TJE.ToString("F2")+"</br>"+TJET.ToString("F2"));
            summary.Add("TOTALSL", TOTALSL.ToString("F2") + "</br>" + TOTALSLT.ToString("F2"));
            summary.Add("TOTALJE", TOTALJE.ToString("F2") + "</br>" + TOTALJET.ToString("F2"));
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