using FineUIPro;
using Newtonsoft.Json.Linq;
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

namespace ERPProject.ERPQuery
{
    public partial class SupplierCheckList : PageBase
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
            PubFunc.DdlDataGet("DDL_DOC_SUPID", docPSSID);
            PubFunc.DdlDataGet("DDL_GOODSTYPE", docDHLX);
            docDHLX.SelectedValue = "2";
            dpkRQ1.SelectedDate = DateTime.Now;
            dpkRQ2.SelectedDate = DateTime.Now;
        }

        private string GetQuerySql()
        {
            string gys = "";
            if (string.IsNullOrWhiteSpace(docPSSID.SelectedValue))
            {
                gys = " AND J.PSSID LIKE '%'";
            }
            else
            {
                gys = "  AND J.PSSID = '" + docPSSID.SelectedValue + "'";
            }
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat(@"SELECT S.SUPID,
                                                           S.SUPNAME,
                                                           NVL(KF.RKSL, 0) RKSL,
                                                           NVL(KF.RKJE, 0) RKJE,
                                                           NVL(ABS(SY.SYSL), 0) SYSL,
                                                           NVL(ABS(SY.SYJE), 0) SYJE,
                                                           FP.FPS,
                                                           FP.INVOICESUM FPJE,
                                                           NVL(-(SY.SYJE), 0) - NVL(FP.INVOICESUM, 0) WFPJE,
                                                           '' MEMO
                                                      FROM (SELECT * FROM DOC_SUPPLIER WHERE ISSUPPLIER = 'Y') S,
                                                           (SELECT J.PSSID,
                                                                   SUM(DECODE(J.BILLTYPE, 'RKD', J.SL, 'THD', J.SL, 0)) RKSL,
                                                                   SUM(DECODE(J.BILLTYPE,
                                                                              'RKD',
                                                                              J.SL * J.HSJJ,
                                                                              'THD',
                                                                              J.SL * J.HSJJ,
                                                                              0)) RKJE
                                                              FROM DAT_GOODSJXC J , DOC_GOODS G
                                                             WHERE J.GDSEQ=G.GDSEQ AND G.CATID0='{4}' AND J.DEPTID IN (SELECT CODE
                                                                                  FROM SYS_DEPT
                                                                                 WHERE TYPE IN ('1', '2')
                                                                                   AND ISLAST = 'Y')
                                                               AND J.RQSJ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                                   TO_DATE('{1}', 'YYYY-MM-DD') + 1
                                                               {2}
                                                               AND J.GDSEQ IN (SELECT GDSEQ FROM DOC_GOODS WHERE ISGZ LIKE '%{3}%')
                                                             GROUP BY J.PSSID) KF,
                                                           (SELECT J.PSSID,
                                                                   SUM(DECODE(J.BILLTYPE,
                                                                              'XSD',
                                                                              J.SL,
                                                                              'XSG',
                                                                              J.SL,
                                                                              'DSH',
                                                                              J.SL,
                                                                              'SYD',
                                                                              J.SL,
                                                                              'XST',
                                                                              DECODE(J.KCADD, '1', J.SL, 0),
                                                                              0)) SYSL,
                                                                   SUM(DECODE(J.BILLTYPE,
                                                                              'XSD',
                                                                              J.SL * J.HSJJ,
                                                                              'XSG',
                                                                              J.SL * J.HSJJ,
                                                                              'DSH',
                                                                              J.SL * J.HSJJ,
                                                                              'SYD',
                                                                              J.SL * J.HSJJ,
                                                                              'XST',
                                                                              DECODE(J.KCADD, '1', J.SL * J.HSJJ, 0),
                                                                              0)) SYJE
                                                              FROM DAT_GOODSJXC J , DOC_GOODS G
                                                             WHERE J.GDSEQ=G.GDSEQ AND G.CATID0='{4}' AND J.DEPTID IN (SELECT CODE
                                                                                  FROM SYS_DEPT
                                                                                 WHERE TYPE IN ('3', '4')
                                                                                   AND ISLAST = 'Y')
                                                               AND J.RQSJ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                                   TO_DATE('{1}', 'YYYY-MM-DD') + 1
                                                               {2}
                                                               AND J.GDSEQ IN (SELECT GDSEQ FROM DOC_GOODS WHERE ISGZ LIKE '%{3}%')
                                                             GROUP BY J.PSSID) SY,
                                                           (SELECT ORG SUPID,
                                                                   COUNT(1) FPS,
                                                                   SUM(INVOICESUM) INVOICESUM,
                                                                   LISTAGG(INVOICENO, ',') WITHIN GROUP(ORDER BY ORG) INVOICENO
                                                              FROM DAT_FP_DOC
                                                             WHERE TO_CHAR(BEGRQ, 'YYYY-MM-DD') = '{0}'
                                                               AND TO_CHAR(ENDRQ, 'YYYY-MM-DD') = '{1}'
                                                             GROUP BY ORG) FP
                                                     WHERE S.SUPID = SY.PSSID(+)
                                                       AND S.SUPID = KF.PSSID(+)
                                                       AND S.SUPID = FP.SUPID(+) ", dpkRQ1.Text, dpkRQ2.Text, gys, ddlISGZ.SelectedValue,docDHLX.SelectedValue);

            if (ckbAll.Checked == false)
            {
                sbSql.Append(@" AND (ABS(NVL(KF.RKSL, 0))>0 OR
                                                      ABS(NVL(KF.RKJE, 0))>0 OR
                                                       NVL(ABS(SY.SYSL), 0)>0 OR
                                                       NVL(ABS(SY.SYJE), 0)>0) ");
            }
            sbSql.Append(@" ORDER BY S.SUPNAME");

            return sbSql.ToString();
        }

        private void OutputSummaryData(DataTable source)
        {
            decimal rksl_total = 0, rkje_total = 0, sysl_total = 0, syje_total = 0, fps_total = 0, fpje_total = 0, wfpje_total = 0;
            foreach (DataRow row in source.Rows)
            {
                rksl_total += Convert.ToDecimal(row["RKSL"]);
                rkje_total += Convert.ToDecimal(row["RKJE"]);
                sysl_total += Convert.ToDecimal(row["SYSL"]);
                syje_total += Convert.ToDecimal(row["SYJE"]);
                fps_total += Convert.ToDecimal(row["FPS"].ToString()==""?"0": row["FPS"]);
                fpje_total += Convert.ToDecimal(row["FPJE"].ToString() == "" ? "0" : row["FPJE"]);
                wfpje_total += Convert.ToDecimal(row["WFPJE"].ToString() == "" ? "0" : row["WFPJE"]);
            }
            JObject summary = new JObject();
            summary.Add("SUPNAME", "汇总合计");
            summary.Add("RKSL", rksl_total.ToString("F2"));
            summary.Add("RKJE", rkje_total.ToString("F2"));
            summary.Add("SYSL", sysl_total.ToString("F2"));
            summary.Add("SYJE", syje_total.ToString("F2"));
            summary.Add("FPS", fps_total.ToString("F2"));
            summary.Add("FPJE", fpje_total.ToString("F2"));
            summary.Add("WFPJE", wfpje_total.ToString("F2"));
            GridSupKC.SummaryData = summary;
        }

        protected void btnQuery_Click(object sender, EventArgs e)
        {
            if (dpkRQ1.SelectedDate == null || dpkRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【查询期间】！");
                return;
            }
            else if (dpkRQ1.SelectedDate > dpkRQ2.SelectedDate)
            {
                Alert.Show("开始日期大于结束日期，请重新输入！");
                return;
            }
            if (string.IsNullOrWhiteSpace(docDHLX.SelectedValue))
            {
                Alert.Show("请先选择商品类型！", "异常提示", MessageBoxIcon.Warning);
                return;
            }
            DataTable dtData = DbHelperOra.Query(GetQuerySql()).Tables[0]; 
            OutputSummaryData(dtData);
            GridSupKC.DataSource = dtData;
            GridSupKC.DataBind();
        }

        protected void GridSupKC_RowDataBound(object sender, FineUIPro.GridRowEventArgs e)
        {

        }

        protected void GridSupKC_RowDoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {

        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            if (dpkRQ1.SelectedDate == null || dpkRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【查询期间】！");
                return;
            }
            else if (dpkRQ1.SelectedDate > dpkRQ2.SelectedDate)
            {
                Alert.Show("开始日期大于结束日期，请重新输入！");
                return;
            }
            DataTable dtData = DbHelperOra.Query(GetQuerySql()).Tables[0];
            if (dtData == null || dtData.Rows.Count == 0)
            {
                Alert.Show("没有数据,无法导出！");
                return;
            }
            string[] columnNames = new string[GridSupKC.Columns.Count - 1];
            for (int index = 1; index < GridSupKC.Columns.Count; index++)
            {
                GridColumn column = GridSupKC.Columns[index];
                if (column is FineUIPro.BoundField)
                {
                    dtData.Columns[((FineUIPro.BoundField)(column)).DataField.ToUpper()].ColumnName = column.HeaderText;
                    columnNames[index - 1] = column.HeaderText;
                }
            }
            ExcelHelper.ExportByWeb(dtData.DefaultView.ToTable(true, columnNames), "供应商结账表", string.Format("供应商结账表_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }
    }
}