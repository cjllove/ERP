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
    public partial class SupplierInvoicing : PageBase
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
            PubFunc.DdlDataGet("DDL_DOC_SUPID", docPSSID, ddlPSSID);
            PubFunc.DdlDataGet("DDL_GOODSTYPE", docDHLX, docDHLX1);
            docDHLX.SelectedValue = "2";
            docDHLX1.SelectedValue = "2";
            lstLRRQ1.SelectedDate = DateTime.Now;
            lstLRRQ2.SelectedDate = DateTime.Now;
            dpkBegRQ.SelectedDate = DateTime.Now;
            dpkEndRQ.SelectedDate = DateTime.Now;
            dpkRQ1.SelectedDate = DateTime.Now.AddMonths(-1);
            dpkRQ2.SelectedDate = DateTime.Now;

            hfdUser.Text = UserAction.UserID;
        }

        private string GetSearchSql()
        {
            string gys = "";
            if (string.IsNullOrWhiteSpace(ddlPSSID.SelectedValue))
            {
                gys = "LIKE '%'";
            }
            else
            {
                gys = " = '" + ddlPSSID.SelectedValue + "'";
            }

            //要考虑商品供应商发生变更的情况，所以要从进销存表中获取DOC_GOODSSUP表中不存在的商品供应商对应关系
            string sup = @"SELECT DISTINCT *
                                  FROM (SELECT SP.SUPID,
                                               F_GETSUPNAME(SP.SUPID) SUPNAME,
                                               HC.GDSEQ,
                                               HC.GDNAME,
                                               HC.GDSPEC,
                                               HC.UNIT,
                                               HC.PIZNO,
                                               HC.PRODUCER
                                          FROM DOC_GOODSSUP SP, DOC_GOODS HC
                                         WHERE SP.GDSEQ = HC.GDSEQ
                                        UNION ALL
                                        SELECT SP.SUPID,
                                               SP.SUPNAME,
                                               HC.GDSEQ,
                                               HC.GDNAME,
                                               HC.GDSPEC,
                                               HC.UNIT,
                                               HC.PIZNO,
                                               HC.PRODUCER
                                          FROM DOC_SUPPLIER SP, DOC_GOODS HC, DAT_GOODSJXC J
                                         WHERE SP.SUPID = J.SUPID
                                           AND J.GDSEQ = HC.GDSEQ)
                                    WHERE SUPID  " + gys;

            string stock = string.Empty;
            if (lstLRRQ2.Text == DateTime.Now.ToString("yyyy-MM-dd"))
            {
                //当天库存从库存表汇总直接获取
                stock = string.Format(@" (SELECT G.PSSID, G.GDSEQ, SUM(G.KCSL) KCSL, SUM(G.KCSL * G.KCHSJJ) KCJE
                                                      FROM DAT_GOODSSTOCK G, DOC_GOODS A
                                                     WHERE G.GDSEQ = A.GDSEQ
                                                       AND G.PSSID {0}
                                                       AND G.GDSEQ LIKE '%{1}%'
                                                       AND G.DEPTID IN (SELECT CODE
                                                                          FROM SYS_DEPT
                                                                         WHERE TYPE IN ('3', '4')
                                                                           AND ISLAST = 'Y')
                                                       AND A.ISGZ LIKE '%{2}%'
                                                     GROUP BY G.PSSID, G.GDSEQ) KSKC,
                                                   (SELECT G.PSSID, G.GDSEQ, SUM(G.KCSL) KCSL, SUM(G.KCSL * G.KCHSJJ) KCJE
                                                      FROM DAT_GOODSSTOCK G, DOC_GOODS A
                                                     WHERE G.GDSEQ = A.GDSEQ
                                                       AND G.PSSID {0}
                                                       AND G.GDSEQ LIKE '%{1}%'
                                                       AND G.DEPTID IN (SELECT CODE
                                                                          FROM SYS_DEPT
                                                                         WHERE TYPE IN ('1', '2')
                                                                           AND ISLAST = 'Y')
                                                       AND A.ISGZ LIKE '%{2}%'
                                                     GROUP BY G.PSSID, G.GDSEQ) KFKC", gys, tbxGOODS.Text.Trim(), lstISGZ.SelectedValue);
            }
            else
            {
                //非当天库存都从日库存表中获取
                stock = string.Format(@" (SELECT G.PSSID, G.GDSEQ, SUM(G.KCSL) KCSL, SUM(G.KCSL * G.KCHSJJ) KCJE
                                                      FROM DAT_STOCKDAY G, DOC_GOODS A
                                                     WHERE G.GDSEQ = A.GDSEQ
                                                       AND G.PSSID {0}
                                                       AND G.GDSEQ LIKE '%{1}%'
                                                       AND TO_CHAR(G.RQ,'YYYY-MM-DD')='{3}'
                                                       AND G.DEPTID IN (SELECT CODE
                                                                          FROM SYS_DEPT
                                                                         WHERE TYPE IN ('3', '4')
                                                                           AND ISLAST = 'Y')
                                                       AND A.ISGZ LIKE '%{2}%'
                                                     GROUP BY G.PSSID, G.GDSEQ) KSKC,
                                                   (SELECT G.PSSID, G.GDSEQ, SUM(G.KCSL) KCSL, SUM(G.KCSL * G.KCHSJJ) KCJE
                                                      FROM DAT_STOCKDAY G, DOC_GOODS A
                                                     WHERE G.GDSEQ = A.GDSEQ
                                                       AND G.PSSID {0}
                                                       AND G.GDSEQ LIKE '%{1}%'
                                                       AND TO_CHAR(G.RQ,'YYYY-MM-DD')='{3}'
                                                       AND G.DEPTID IN (SELECT CODE
                                                                          FROM SYS_DEPT
                                                                         WHERE TYPE IN ('1', '2')
                                                                           AND ISLAST = 'Y')
                                                       AND A.ISGZ LIKE '%{2}%'
                                                     GROUP BY G.PSSID, G.GDSEQ) KFKC", gys, tbxGOODS.Text.Trim(), lstISGZ.SelectedValue, lstLRRQ2.Text);
            }

            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat(@"SELECT S.SUPID,
                                                   S.SUPNAME,
                                                   S.GDSEQ,
                                                   S.GDNAME,
                                                   S.GDSPEC,
                                                   S.PIZNO,
                                                   F_GETPRODUCERNAME(S.PRODUCER) PRODUCERNAME,
                                                   F_GETUNITNAME(S.UNIT) UNIT,
                                                   NVL(KF.RKSL, 0) RKSL,
                                                   NVL(KF.RKJE, 0) RKJE,
                                                   NVL(-(KF.SHSL), 0) SHSL,
                                                   NVL(-(KF.SHJE), 0) SHJE,
                                                   NVL(-(KF.CKSL), 0) CKSL,
                                                   NVL(-(KF.CKJE), 0) CKJE,
                                                   NVL(-(SY.SYSL), 0) SYSL,
                                                   NVL(-(SY.SYJE), 0) SYJE,
                                                   NVL(KSKC.KCSL, 0) KSKCSL,
                                                   NVL(KSKC.KCJE, 0) KSKCJE,
                                                   NVL(KFKC.KCSL, 0) KFKCSL,
                                                   NVL(KFKC.KCJE, 0) KFKCJE
                                              FROM ({5}) S,
                                                   (SELECT J.PSSID,
                                                           J.GDSEQ,
                                                           SUM(DECODE(J.BILLTYPE, 'RKD', J.SL, 'THD', J.SL, 0)) RKSL,
                                                           SUM(DECODE(J.BILLTYPE, 'RKD', J.HSJE, 'THD', J.HSJE, 0)) RKJE,
                                                           SUM(DECODE(J.BILLTYPE, 'SYD', J.SL, 0)) SHSL,
                                                           SUM(DECODE(J.BILLTYPE, 'SYD', J.SL * J.HSJJ, 0)) SHJE,
                                                           SUM(DECODE(J.BILLTYPE,
                                                                      'LCD',
                                                                      J.SL,
                                                                      'DSC',
                                                                      J.SL,
                                                                      'CKD',
                                                                      J.SL,
                                                                      'LTD',
                                                                      J.SL,
                                                                      'DST',
                                                                      J.SL,
                                                                      'XST',
                                                                      J.SL,
                                                                      0)) CKSL,
                                                           SUM(DECODE(J.BILLTYPE,
                                                                      'LCD',
                                                                      J.SL * J.HSJJ,
                                                                      'DSC',
                                                                      J.SL * J.HSJJ,
                                                                      'CKD',
                                                                      J.SL * J.HSJJ,
                                                                      'LTD',
                                                                      J.SL * J.HSJJ,
                                                                      'DST',
                                                                      J.SL * J.HSJJ,
                                                                      'XST',
                                                                      J.SL * J.HSJJ,
                                                                      0)) CKJE
                                                      FROM DAT_GOODSJXC J,DOC_GOODS G
                                                     WHERE J.DEPTID IN (SELECT CODE
                                                                          FROM SYS_DEPT
                                                                         WHERE TYPE IN ('1', '2')
                                                                           AND ISLAST = 'Y')
                                                       AND J.RQSJ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                           TO_DATE('{1}', 'YYYY-MM-DD') + 1
                                                       AND J.PSSID {2}
                                                        AND J.GDSEQ=G.GDSEQ
                                                        AND G.CATID0={7}
                                                       AND J.GDSEQ LIKE '%{4}%'
                                                       AND J.GDSEQ IN
                                                           (SELECT GDSEQ FROM DOC_GOODS WHERE ISGZ LIKE '%{3}%')
                                                     GROUP BY J.PSSID, J.GDSEQ) KF,
                                                   (SELECT J.PSSID,
                                                           J.GDSEQ,
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
                                                      FROM DAT_GOODSJXC J
                                                     WHERE J.DEPTID IN (SELECT CODE
                                                                          FROM SYS_DEPT
                                                                         WHERE TYPE IN ('3', '4')
                                                                           AND ISLAST = 'Y')
                                                       AND J.RQSJ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                           TO_DATE('{1}', 'YYYY-MM-DD') + 1
                                                       AND J.PSSID {2}
                                                       AND J.GDSEQ LIKE '%{4}%'
                                                       AND J.GDSEQ IN
                                                           (SELECT GDSEQ FROM DOC_GOODS WHERE ISGZ LIKE '%{3}%')
                                                     GROUP BY J.PSSID, J.GDSEQ) SY,
                                                   {6}
                                             WHERE S.SUPID = SY.PSSID(+)
                                               AND S.GDSEQ = SY.GDSEQ(+)
                                               AND S.SUPID = KSKC.PSSID(+)
                                               AND S.GDSEQ = KSKC.GDSEQ(+)
                                               AND S.SUPID = KFKC.PSSID(+)
                                               AND S.GDSEQ = KFKC.GDSEQ(+)
                                               AND S.SUPID = KF.PSSID(+)
                                               AND S.GDSEQ = KF.GDSEQ(+)
                                               AND (NVL(KF.RKSL, 0)>0 OR
                                                       NVL(ABS(KF.SHSL), 0)>0 OR
                                                       NVL(ABS(KF.CKSL), 0)>0 OR
                                                       NVL(ABS(SY.SYSL), 0)>0 OR
                                                       NVL(KSKC.KCSL, 0)>0 OR
                                                       NVL(KFKC.KCSL, 0) >0) 
                                             ORDER BY S.SUPNAME, S.GDNAME
            ", lstLRRQ1.Text, lstLRRQ2.Text, gys, lstISGZ.SelectedValue, tbxGOODS.Text.Trim(), sup, stock, docDHLX1.SelectedValue);

            //昌吉有结存处理，取库存时如果有结存库存则取结存，无结存库存则取日库存
            //By c 2016年5月25日12:31:59 At 新疆昌吉
            string stockjc = @"(SELECT A.*
                                      FROM DAT_STOCKDAY A
                                     WHERE NOT EXISTS
                                     (SELECT 1
                                              FROM DAT_STOCKJCDAY B
                                             WHERE TO_CHAR(B.RQSJ, 'YYYY-MM-DD') = TO_CHAR(A.RQ, 'YYYY-MM-DD'))
                                    UNION ALL
                                    SELECT *
                                      FROM DAT_STOCKJCDAY)";
            return sbSql.ToString().Replace("DAT_STOCKDAY", stockjc);
        }

        private string GetSearchSql1()
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat(@"SELECT G.GDSEQ,
                                                           G.GDNAME,
                                                           G.GDSPEC,
                                                           F_GETUNITNAME(G.UNIT) UNIT,
                                                           G.HSJJ,
                                                           G.PRODUCER,
                                                           G.PIZNO PZWHA,
                                                           SUM(DECODE(J.BILLTYPE, 'RKD', J.SL, 'THD', J.SL, 0)) SL,
                                                           SUM(DECODE(J.BILLTYPE, 'RKD', J.HSJE, 'THD', J.HSJE, 0)) JE
                                                      FROM DAT_GOODSJXC J,
                                                           DOC_GOODS G
                                                     WHERE (G.GDSEQ LIKE '%{2}%' OR G.GDNAME LIKE '%{2}%' OR
                                                           UPPER(G.ZJM) LIKE UPPER('%{2}%'))
                                                       AND J.RQSJ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                           TO_DATE('{1}', 'YYYY-MM-DD') + 1
                                                     GROUP BY G.GDSEQ,
                                                              G.GDNAME,
                                                              G.GDSPEC,
                                                              G.UNIT,
                                                              G.HSJJ,
                                                              G.PRODUCER,
                                                              G.PIZNO", dpkBegRQ.Text, dpkEndRQ.Text, trbSearch.Text.Trim());
            return sbSql.ToString();
        }

        private void DataSearch()
        {
            // int total = 0;

            DataTable dtData = DbHelperOra.Query(GetSearchSql()).Tables[0]; //PubFunc.DbGetPage(GridGoods.PageIndex, GridGoods.PageSize, GetSearchSql(), ref total);
            OutputSummaryData(dtData);
            //GridGoods.RecordCount = total;
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
            if (string.IsNullOrWhiteSpace(ddlPSSID.SelectedValue))
            {
                Alert.Show("请选择要查询的供应商！");
                return;
            }
            if (string.IsNullOrWhiteSpace(docDHLX1.SelectedValue))
            {
                Alert.Show("请先选择商品类型！", "异常提示", MessageBoxIcon.Warning);
                return;
            }
            DataSearch();
        }
        protected void btClear_Click(object sender, EventArgs e)
        {
            PubFunc.FormDataClear(FormUser);
            lstLRRQ1.SelectedDate = DateTime.Now;
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
            string[] columnNames = new string[19];
            for (int index = 1; index < GridGoods.Columns.Count; index++)
            {
                GridColumn column = GridGoods.Columns[index];
                if (column is FineUIPro.BoundField)
                {
                    //dtData.Columns[((FineUIPro.BoundField)(column)).DataField.ToUpper()].DataType = Type.GetType("System.String");
                    dtData.Columns[((FineUIPro.BoundField)(column)).DataField.ToUpper()].ColumnName = column.HeaderText;
                    columnNames[index - 1] = column.HeaderText;
                }
                if (column is FineUIPro.GroupField)
                {
                    FineUIPro.GroupField group = column as FineUIPro.GroupField;
                    for (int i = 0; i < group.Columns.Count; i++)
                    {
                        GridColumn column1 = group.Columns[i];
                        if (column1 is FineUIPro.BoundField)
                        {
                            dtData.Columns[((FineUIPro.BoundField)(column1)).DataField.ToUpper()].ColumnName = column1.HeaderText;
                            columnNames[index - 1 + i] = column1.HeaderText;
                        }
                    }
                }
            }

            ExcelHelper.ExportByWeb(dtData.DefaultView.ToTable(true, columnNames), "供应商进销存信息汇总", string.Format("供应商进销存信息汇总_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }

        protected void GridGoods_Sort(object sender, GridSortEventArgs e)
        {
            GridGoods.SortDirection = e.SortDirection;
            GridGoods.SortField = e.SortField;

            DataSearch();
        }

        private void OutputSummaryData(DataTable source)
        {
            decimal kfkcsl_total = 0, kfkcje_total = 0, kskcsl_total = 0, kskcje_total = 0, rksl_total = 0, rkje_total = 0, cksl_total = 0, ckje_total = 0, sysl_total = 0, syje_total = 0, shsl_total = 0, shje_total = 0;
            foreach (DataRow row in source.Rows)
            {
                rksl_total += Convert.ToDecimal(row["RKSL"]);
                rkje_total += Convert.ToDecimal(row["RKJE"]);
                shsl_total += Convert.ToDecimal(row["SHSL"]);
                shje_total += Convert.ToDecimal(row["SHJE"]);
                cksl_total += Convert.ToDecimal(row["CKSL"]);
                ckje_total += Convert.ToDecimal(row["CKJE"]);
                sysl_total += Convert.ToDecimal(row["SYSL"]);
                syje_total += Convert.ToDecimal(row["SYJE"]);
                kskcsl_total += Convert.ToDecimal(row["KSKCSL"]);
                kskcje_total += Convert.ToDecimal(row["KSKCJE"]);
                kfkcsl_total += Convert.ToDecimal(row["KFKCSL"]);
                kfkcje_total += Convert.ToDecimal(row["KFKCJE"]);
            }
            JObject summary = new JObject();
            summary.Add("GDNAME", "汇总合计");
            summary.Add("G_RKSL", rksl_total.ToString("F2"));
            summary.Add("G_RKJE", rkje_total.ToString("F2"));
            summary.Add("G_SHSL", shsl_total.ToString("F2"));
            summary.Add("G_SHJE", shje_total.ToString("F2"));
            summary.Add("G_CKSL", cksl_total.ToString("F2"));
            summary.Add("G_CKJE", ckje_total.ToString("F2"));
            summary.Add("G_SYSL", sysl_total.ToString("F2"));
            summary.Add("G_SYJE", syje_total.ToString("F2"));
            summary.Add("G_KSKCSL", kskcsl_total.ToString("F2"));
            summary.Add("G_KSKCJE", kskcje_total.ToString("F2"));
            summary.Add("G_KFKCSL", kfkcsl_total.ToString("F2"));
            summary.Add("G_KFKCJE", kfkcje_total.ToString("F2"));
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
            dpkBegRQ.SelectedDate = DateTime.Now;
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

            ExcelHelper.ExportByWeb(dtData.DefaultView.ToTable(true, columnNames), "商品出库信息汇总", string.Format("商品出库信息汇总_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
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

        protected void GridDeptKC_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            tbxGOODS.Text = "";
            lstLRRQ1.SelectedDate = dpkRQ1.SelectedDate;
            lstLRRQ2.SelectedDate = dpkRQ2.SelectedDate;
            ddlPSSID.SelectedValue = GridDeptKC.Rows[e.RowIndex].DataKeys[0].ToString();
            lstISGZ.SelectedValue = ddlISGZ.SelectedValue;
            docDHLX1.SelectedValue = docDHLX.SelectedValue;
            docDHLX1.Enabled = false;
            DataSearch();
            TabStrip1.ActiveTabIndex = 2;
        }

        private string GetSumSql()
        {
            string gys = "";
            if (string.IsNullOrWhiteSpace(docPSSID.SelectedValue))
            {
                gys = "LIKE '%'";
            }
            else
            {
                gys = " = '" + docPSSID.SelectedValue + "'";
            }

            string qcb, qmb;
            if (dpkRQ1.Text == DateTime.Now.ToString("yyyy-MM-dd"))
            {
                qcb = string.Format(@"(SELECT A.SUPID, SUM(A.KCSL) QCSL, SUM(A.KCSL * A.KCHSJJ) QCJE
                                 FROM DAT_GOODSSTOCK A ,SYS_CATEGORY G
                               WHERE A.CATID=G.CODE AND G.TYPE='{1}' AND A.GDSEQ IN (SELECT GDSEQ FROM DOC_GOODS WHERE ISGZ LIKE '%{1}%' )
                               GROUP BY A.SUPID) QC", ddlISGZ.SelectedValue, docDHLX.SelectedValue);
            }
            else
            {
                qcb = string.Format(@"(SELECT A.SUPID, SUM(A.KCSL) QCSL, SUM(A.KCSL * A.KCHSJJ) QCJE
                                                    FROM DAT_STOCKDAY A,SYS_CATEGORY G
                               WHERE A.CATID=G.CODE AND G.TYPE='{2}' AND  TO_CHAR(A.RQ+1, 'YYYY-MM-DD') = '{0}'
                                                      AND A.GDSEQ IN
                                                                   (SELECT GDSEQ FROM DOC_GOODS WHERE ISGZ LIKE '%{1}%' AND CATID0='{2}')
                                                  GROUP BY A.SUPID) QC", dpkRQ1.Text, ddlISGZ.SelectedValue, docDHLX.SelectedValue);

            }

            if (dpkRQ2.Text == DateTime.Now.ToString("yyyy-MM-dd"))
            {
                qmb = string.Format(@"  (SELECT G.PSSID, SUM(G.KCSL) KCSL, SUM(G.KCSL * G.KCHSJJ) KCJE
                                                              FROM DAT_GOODSSTOCK G  ,SYS_CATEGORY B
                                                             WHERE G.CATID=B.CODE AND B.TYPE='{2}'AND G.PSSID {0}
                                                               AND G.DEPTID IN (SELECT CODE
                                                                                  FROM SYS_DEPT
                                                                                 WHERE TYPE IN ('3', '4'))
                                                               AND G.GDSEQ IN
                                                                   (SELECT GDSEQ FROM DOC_GOODS WHERE ISGZ LIKE '%{1}%' AND CATID0='{2}')
                                                             GROUP BY G.PSSID) KSKC,
                                                           (SELECT G.PSSID, SUM(G.KCSL) KCSL, SUM(G.KCSL * G.KCHSJJ) KCJE
                                                              FROM DAT_GOODSSTOCK G ,SYS_CATEGORY B
                                                             WHERE G.CATID=B.CODE AND B.TYPE='{2}' AND G.PSSID {0}
                                                               AND G.DEPTID IN (SELECT CODE
                                                                                  FROM SYS_DEPT
                                                                                 WHERE TYPE IN ('1', '2'))
                                                               AND G.GDSEQ IN
                                                                   (SELECT GDSEQ FROM DOC_GOODS WHERE ISGZ LIKE '%{1}%' AND CATID0='{2}')
                                                             GROUP BY G.PSSID) KFKC", gys, ddlISGZ.SelectedValue, docDHLX.SelectedValue);
            }
            else
            {
                qmb = string.Format(@"  (SELECT G.PSSID, SUM(G.KCSL) KCSL, SUM(G.KCSL * G.KCHSJJ) KCJE
                                                              FROM DAT_STOCKDAY G , SYS_CATEGORY B
                                                             WHERE G.CATID=B.CODE AND B.TYPE='{3}' AND G.PSSID {0}
                                                               AND TO_CHAR(G.RQ,'YYYY-MM-DD')='{2}'
                                                               AND G.DEPTID IN (SELECT CODE
                                                                                  FROM SYS_DEPT
                                                                                 WHERE TYPE IN ('3', '4'))
                                                               AND G.GDSEQ IN
                                                                   (SELECT GDSEQ FROM DOC_GOODS WHERE ISGZ LIKE '%{1}%'AND CATID0='{3}')
                                                             GROUP BY G.PSSID) KSKC,
                                                           (SELECT G.PSSID, SUM(G.KCSL) KCSL, SUM(G.KCSL * G.KCHSJJ) KCJE
                                                              FROM DAT_STOCKDAY G , SYS_CATEGORY B
                                                             WHERE G.CATID=B.CODE AND B.TYPE='{3}' AND  G.PSSID {0}
                                                               AND TO_CHAR(G.RQ,'YYYY-MM-DD')='{2}'
                                                               AND G.DEPTID IN (SELECT CODE
                                                                                  FROM SYS_DEPT
                                                                                 WHERE TYPE IN ('1', '2'))
                                                               AND G.GDSEQ IN
                                                                   (SELECT GDSEQ FROM DOC_GOODS WHERE ISGZ LIKE '%{1}%' AND CATID0='{3}')
                                                             GROUP BY G.PSSID) KFKC", gys, ddlISGZ.SelectedValue, dpkRQ2.Text,docDHLX.SelectedValue);
            }
            StringBuilder sbSql = new StringBuilder();
            //昌吉州人民医院期初数据处理 By c 2016年5月3日09:53:21
            if (dpkRQ1.Text == "2016-03-01")
            {
                sbSql.AppendFormat(@"SELECT S.SUPID,
                                                           S.SUPNAME,
                                                           NVL(QC.QCSL,0) QCSL,
                                                           NVL(QC.QCJE,0) QCJE,
                                                           NVL(KF.RKSL, 0)-NVL(QC.QCSL,0) RKSL,
                                                           NVL(KF.RKJE, 0)-NVL(QC.QCJE,0) RKJE,
                                                           NVL(-(KF.SHSL), 0) SHSL,
                                                           NVL(-(KF.SHJE), 0) SHJE,
                                                           NVL(-(KF.CKSL), 0) CKSL,
                                                           NVL(-(KF.CKJE), 0) CKJE,
                                                           NVL(-(SY.SYSL), 0) SYSL,
                                                           NVL(-(SY.SYJE), 0) SYJE,
                                                           NVL(KSKC.KCSL, 0) KSKCSL,
                                                           NVL(KSKC.KCJE, 0) KSKCJE,
                                                           NVL(KFKC.KCSL, 0) KFKCSL,
                                                           NVL(KFKC.KCJE, 0) KFKCJE
                                                      FROM (SELECT * FROM DOC_SUPPLIER WHERE ISSUPPLIER = 'Y') S,
                                                           (SELECT J.PSSID,
                                                                   SUM(DECODE(J.BILLTYPE, 'RKD', J.SL, 'THD', J.SL, 0)) RKSL,
                                                                   SUM(DECODE(J.BILLTYPE, 'RKD', J.SL*J.HSJJ, 'THD', J.SL*J.HSJJ, 0)) RKJE,
                                                                   SUM(DECODE(J.BILLTYPE, 'SYD', J.SL, 0)) SHSL,
                                                                   SUM(DECODE(J.BILLTYPE, 'SYD', J.SL * J.HSJJ, 0)) SHJE,          
                                                                   SUM(DECODE(J.BILLTYPE,
                                                                              'LCD',
                                                                              J.SL,
                                                                              'DSC',
                                                                              J.SL,
                                                                              'CKD',
                                                                              J.SL,
                                                                              'LTD',
                                                                              J.SL,
                                                                              'DST',
                                                                              J.SL,
                                                                              'XST',
                                                                              J.SL,
                                                                              0)) CKSL,
                                                                   SUM(DECODE(J.BILLTYPE,
                                                                              'LCD',
                                                                              J.SL*J.HSJJ,
                                                                              'DSC',
                                                                              J.SL*J.HSJJ,
                                                                              'CKD',
                                                                              J.SL*J.HSJJ,
                                                                              'LTD',
                                                                              J.SL*J.HSJJ,
                                                                              'DST',
                                                                              J.SL*J.HSJJ,
                                                                              'XST',
                                                                              J.SL*J.HSJJ,
                                                                              0)) CKJE
                                                              FROM DAT_GOODSJXC J , DOC_GOODS G
                                                             WHERE J.GDSEQ=G.GDSEQ AND G.CATID0='{6}' AND J.DEPTID IN (SELECT CODE
                                                                                  FROM SYS_DEPT
                                                                                 WHERE TYPE IN ('1', '2')
                                                                                   AND ISLAST = 'Y')
                                                               AND J.PSSID {2}
                                                               AND J.GDSEQ IN
                                                                   (SELECT GDSEQ FROM DOC_GOODS WHERE ISGZ LIKE '%{3}%' AND CATID0='{6}')
                                                               AND J.RQSJ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                                   TO_DATE('{1}', 'YYYY-MM-DD') + 1
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
                                                                              DECODE(J.KCADD,'1',J.SL,0),
                                                                              0)) SYSL,
                                                                   SUM(DECODE(J.BILLTYPE,
                                                                              'XSD',
                                                                              J.SL*J.HSJJ,
                                                                              'XSG',
                                                                              J.SL*J.HSJJ,
                                                                              'DSH',
                                                                              J.SL*J.HSJJ,
                                                                              'SYD',
                                                                              J.SL*J.HSJJ,
                                                                              'XST',
                                                                              DECODE(J.KCADD,'1',J.SL*J.HSJJ,0),
                                                                              0)) SYJE
                                                              FROM DAT_GOODSJXC J , DOC_GOODS G
                                                             WHERE J.GDSEQ=G.GDSEQ AND G.CATID0='{6}' AND J.DEPTID IN (SELECT CODE
                                                                                  FROM SYS_DEPT
                                                                                 WHERE TYPE IN ('3', '4')
                                                                                   AND ISLAST = 'Y')
                                                               AND J.PSSID {2}
                                                               AND J.GDSEQ IN
                                                                   (SELECT GDSEQ FROM DOC_GOODS WHERE ISGZ LIKE '%{3}%' AND CATID0='{6}')
                                                               AND J.RQSJ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                                   TO_DATE('{1}', 'YYYY-MM-DD') + 1
                                                             GROUP BY J.PSSID) SY,{4},
                                                          {5} ", dpkRQ1.Text, dpkRQ2.Text, gys, ddlISGZ.SelectedValue, qcb, qmb, docDHLX.SelectedValue);
            }
            else
            {
                sbSql.AppendFormat(@"SELECT S.SUPID,
                                                           S.SUPNAME,
                                                           NVL(KF.RKSL, 0) RKSL,
                                                           NVL(KF.RKJE, 0) RKJE,
                                                           NVL(QC.QCSL,0) QCSL,
                                                           NVL(QC.QCJE,0) QCJE,
                                                           NVL(-(KF.SHSL), 0) SHSL,
                                                           NVL(-(KF.SHJE), 0) SHJE,
                                                           NVL(-(KF.CKSL), 0) CKSL,
                                                           NVL(-(KF.CKJE), 0) CKJE,
                                                           NVL(-(SY.SYSL), 0) SYSL,
                                                           NVL(-(SY.SYJE), 0) SYJE,
                                                           NVL(KSKC.KCSL, 0) KSKCSL,
                                                           NVL(KSKC.KCJE, 0) KSKCJE,
                                                           NVL(KFKC.KCSL, 0) KFKCSL,
                                                           NVL(KFKC.KCJE, 0) KFKCJE
                                                      FROM (SELECT * FROM DOC_SUPPLIER WHERE ISSUPPLIER = 'Y') S,
                                                           (SELECT J.PSSID,
                                                                   SUM(DECODE(J.BILLTYPE, 'RKD', J.SL, 'THD', J.SL, 0)) RKSL,
                                                                   SUM(DECODE(J.BILLTYPE, 'RKD', J.HSJE, 'THD', J.HSJE, 0)) RKJE,
                                                                   SUM(DECODE(J.BILLTYPE, 'SYD', J.SL, 0)) SHSL,
                                                                   SUM(DECODE(J.BILLTYPE, 'SYD', J.SL * J.HSJJ, 0)) SHJE,          
                                                                   SUM(DECODE(J.BILLTYPE,
                                                                              'LCD',
                                                                              J.SL,
                                                                              'DSC',
                                                                              J.SL,
                                                                              'CKD',
                                                                              J.SL,
                                                                              'LTD',
                                                                              J.SL,
                                                                              'DST',
                                                                              J.SL,
                                                                              'XST',
                                                                              J.SL,
                                                                              0)) CKSL,
                                                                   SUM(DECODE(J.BILLTYPE,
                                                                              'LCD',
                                                                              J.SL*J.HSJJ,
                                                                              'DSC',
                                                                              J.SL*J.HSJJ,
                                                                              'CKD',
                                                                              J.SL*J.HSJJ,
                                                                              'LTD',
                                                                              J.SL*J.HSJJ,
                                                                              'DST',
                                                                              J.SL*J.HSJJ,
                                                                              'XST',
                                                                              J.SL*J.HSJJ,
                                                                              0)) CKJE
                                                              FROM DAT_GOODSJXC J , DOC_GOODS G
                                                             WHERE J.GDSEQ=G.GDSEQ AND G.CATID0='{6}' AND J.DEPTID IN (SELECT CODE
                                                                                  FROM SYS_DEPT
                                                                                 WHERE TYPE IN ('1', '2')
                                                                                   AND ISLAST = 'Y')
                                                               AND J.PSSID {2}
                                                               AND J.GDSEQ IN
                                                                   (SELECT GDSEQ FROM DOC_GOODS WHERE ISGZ LIKE '%{3}%' AND CATID0='{6}')
                                                               AND J.RQSJ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                                   TO_DATE('{1}', 'YYYY-MM-DD') + 1
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
                                                                              DECODE(J.KCADD,'1',J.SL,0),
                                                                              0)) SYSL,
                                                                   SUM(DECODE(J.BILLTYPE,
                                                                              'XSD',
                                                                              J.SL*J.HSJJ,
                                                                              'XSG',
                                                                              J.SL*J.HSJJ,
                                                                              'DSH',
                                                                              J.SL*J.HSJJ,
                                                                              'SYD',
                                                                              J.SL*J.HSJJ,
                                                                              'XST',
                                                                              DECODE(J.KCADD,'1',J.SL*J.HSJJ,0),
                                                                              0)) SYJE
                                                              FROM DAT_GOODSJXC J , DOC_GOODS G
                                                             WHERE J.GDSEQ=G.GDSEQ AND G.CATID0='{6}' AND J.DEPTID IN (SELECT CODE
                                                                                  FROM SYS_DEPT
                                                                                 WHERE TYPE IN ('3', '4'))
                                                               AND J.PSSID {2}
                                                               AND J.GDSEQ IN
                                                                   (SELECT GDSEQ FROM DOC_GOODS WHERE ISGZ LIKE '%{3}%' AND CATID0='{6}')
                                                               AND J.RQSJ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                                   TO_DATE('{1}', 'YYYY-MM-DD') + 1
                                                             GROUP BY J.PSSID) SY,{4},
                                                          {5} ", dpkRQ1.Text, dpkRQ2.Text, gys, ddlISGZ.SelectedValue, qcb, qmb, docDHLX.SelectedValue);
            }
            if (string.IsNullOrWhiteSpace(docPSSID.SelectedValue))
            {
                sbSql.Append(@" WHERE S.SUPID = SY.PSSID(+)
                                              AND S.SUPID = QC.SUPID(+)
                                              AND S.SUPID = KSKC.PSSID(+)
                                              AND S.SUPID = KFKC.PSSID(+)
                                              AND S.SUPID = KF.PSSID(+) ");
                if (ckbAll.Checked == false)
                {
                    sbSql.Append(@" AND (NVL(KF.RKSL, 0)>0 OR
                                                       NVL(ABS(KF.SHSL), 0)>0 OR
                                                       NVL(ABS(KF.CKSL), 0)>0 OR
                                                       NVL(ABS(SY.SYSL), 0)>0 OR
                                                       NVL(KSKC.KCSL, 0)>0 OR
                                                       NVL(KFKC.KCSL, 0) >0) ");
                }
                sbSql.Append(@" ORDER BY S.SUPNAME");
            }
            else
            {
                sbSql.AppendFormat(@" WHERE S.SUPID = SY.PSSID(+)
                                                        AND S.SUPID = QC.SUPID(+)
                                                        AND S.SUPID = KSKC.PSSID(+)
                                                        AND S.SUPID = KFKC.PSSID(+)
                                                        AND S.SUPID = KF.PSSID(+)
                                                        AND S.SUPID = '{0}'
                                                      ORDER BY S.SUPNAME", docPSSID.SelectedValue);
            }
            //昌吉有结存处理，取库存时如果有结存库存则取结存，无结存库存则取日库存
            //By c 2016年5月25日12:31:59 At 新疆昌吉
            string stockjc = @"(SELECT A.*
                                      FROM DAT_STOCKDAY A
                                     WHERE NOT EXISTS
                                     (SELECT 1
                                              FROM DAT_STOCKJCDAY B
                                             WHERE TO_CHAR(B.RQSJ, 'YYYY-MM-DD') = TO_CHAR(A.RQ, 'YYYY-MM-DD'))
                                    UNION ALL
                                    SELECT *
                                      FROM DAT_STOCKJCDAY)";
            return sbSql.ToString().Replace("DAT_STOCKDAY", stockjc);
        }

        protected void btnQuery_Click(object sender, EventArgs e)
        {
            highlightRows.Text = "";
            if (dpkRQ1.SelectedDate == null || dpkRQ1.SelectedDate == null)
            {
                Alert.Show("请输入条件【查询期间】！");
                return;
            }
            else if (dpkRQ2.SelectedDate > dpkRQ2.SelectedDate)
            {
                Alert.Show("开始日期大于结束日期，请重新输入！");
                return;
            }
            if (string.IsNullOrWhiteSpace(docDHLX.SelectedValue))
            {
                Alert.Show("请先选择商品类型！", "异常提示", MessageBoxIcon.Warning);
                return;
            }
            DataTable dt = DbHelperOra.Query(GetSumSql()).Tables[0];
            GridDeptKC.DataSource = dt;
            GridDeptKC.DataBind();
            decimal qcsl_total = 0, qcje_total = 0, kfkcsl_total = 0, kfkcje_total = 0, kskcsl_total = 0, kskcje_total = 0, rksl_total = 0, rkje_total = 0, cksl_total = 0, ckje_total = 0, sysl_total = 0, syje_total = 0, shsl_total = 0, shje_total = 0;
            foreach (DataRow row in dt.Rows)
            {
                qcsl_total += Convert.ToDecimal(row["QCSL"]);
                qcje_total += Convert.ToDecimal(row["QCJE"]);
                rksl_total += Convert.ToDecimal(row["RKSL"]);
                rkje_total += Convert.ToDecimal(row["RKJE"]);
                shsl_total += Convert.ToDecimal(row["SHSL"]);
                shje_total += Convert.ToDecimal(row["SHJE"]);
                cksl_total += Convert.ToDecimal(row["CKSL"]);
                ckje_total += Convert.ToDecimal(row["CKJE"]);
                sysl_total += Convert.ToDecimal(row["SYSL"]);
                syje_total += Convert.ToDecimal(row["SYJE"]);
                kskcsl_total += Convert.ToDecimal(row["KSKCSL"]);
                kskcje_total += Convert.ToDecimal(row["KSKCJE"]);
                kfkcsl_total += Convert.ToDecimal(row["KFKCSL"]);
                kfkcje_total += Convert.ToDecimal(row["KFKCJE"]);
            }
            JObject summary = new JObject();
            summary.Add("DEPTNAME", "汇总合计");
            summary.Add("QCSL", qcsl_total.ToString("F2"));
            summary.Add("QCJE", qcje_total.ToString("F2"));
            summary.Add("RKSL", rksl_total.ToString("F2"));
            summary.Add("RKJE", rkje_total.ToString("F2"));
            summary.Add("SHSL", shsl_total.ToString("F2"));
            summary.Add("SHJE", shje_total.ToString("F2"));
            summary.Add("CKSL", cksl_total.ToString("F2"));
            summary.Add("CKJE", ckje_total.ToString("F2"));
            summary.Add("SYSL", sysl_total.ToString("F2"));
            summary.Add("SYJE", syje_total.ToString("F2"));
            summary.Add("KSKCSL", kskcsl_total.ToString("F2"));
            summary.Add("KSKCJE", kskcje_total.ToString("F2"));
            summary.Add("KFKCSL", kfkcsl_total.ToString("F2"));
            summary.Add("KFKCJE", kfkcje_total.ToString("F2"));
            GridDeptKC.SummaryData = summary;
        }

        protected void GridDeptKC_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                decimal rks = 0, sys = 0, kskcs = 0, kfkcs = 0;
                decimal.TryParse(row["RKSL"].ToString(), out rks);
                decimal.TryParse(row["SYSL"].ToString(), out sys);
                decimal.TryParse(row["KSKCSL"].ToString(), out kskcs);
                decimal.TryParse(row["KFKCSL"].ToString(), out kfkcs);

                if (rks != (sys + kskcs + kfkcs))
                {
                    highlightRows.Text += e.RowIndex.ToString() + ",";
                }
            }
        }

        protected void btnExportSum_Click(object sender, EventArgs e)
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
            DataTable dtData = DbHelperOra.Query(GetSumSql()).Tables[0];
            if (dtData == null || dtData.Rows.Count == 0)
            {
                Alert.Show("没有数据,无法导出！");
                return;
            }

            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("序号", typeof(int)));
            foreach (DataColumn col in dtData.Columns)
            {
                table.Columns.Add(new DataColumn(col.ColumnName, col.DataType));
            }
            //计算汇总金额
            decimal rowno = 0, sum_qcsl = 0, sum_qcje = 0, sum_rksl = 0, sum_rkje = 0, sum_shsl = 0, sum_shje = 0, sum_cksl = 0, sum_ckje = 0,
                       sum_kfkcsl = 0, sum_kfkcje = 0, sum_sysl = 0, sum_syje = 0, sum_kskcsl = 0, sum_kskcje = 0;
            foreach (DataRow row in dtData.Rows)
            {
                DataRow dr = table.NewRow();
                dr["序号"] = ++rowno;
                foreach (DataColumn col in dtData.Columns)
                {
                    dr[col.ColumnName] = row[col.ColumnName];
                }
                table.Rows.Add(dr);

                sum_qcsl += decimal.Parse(row["QCSL"].ToString());
                sum_qcje += decimal.Parse(row["QCJE"].ToString());
                sum_rksl += decimal.Parse(row["RKSL"].ToString());
                sum_rkje += decimal.Parse(row["RKJE"].ToString());
                sum_shsl += decimal.Parse(row["SHSL"].ToString());
                sum_shje += decimal.Parse(row["SHJE"].ToString());
                sum_cksl += decimal.Parse(row["CKSL"].ToString());
                sum_ckje += decimal.Parse(row["CKJE"].ToString());
                sum_kfkcsl += decimal.Parse(row["KFKCSL"].ToString());
                sum_kfkcje += decimal.Parse(row["KFKCJE"].ToString());
                sum_sysl += decimal.Parse(row["SYSL"].ToString());
                sum_syje += decimal.Parse(row["SYJE"].ToString());
                sum_kskcsl += decimal.Parse(row["KSKCSL"].ToString());
                sum_kskcje += decimal.Parse(row["KSKCJE"].ToString());
            }
            DataRow dr2 = table.NewRow();
            dr2["SUPNAME"] = "汇总合计";
            dr2["QCSL"] = sum_qcsl;
            dr2["QCJE"] = sum_qcje;
            dr2["RKSL"] = sum_rksl;
            dr2["RKJE"] = sum_rkje;
            dr2["SHSL"] = sum_shsl;
            dr2["SHJE"] = sum_shje;
            dr2["CKSL"] = sum_cksl;
            dr2["CKJE"] = sum_ckje;
            dr2["KFKCSL"] = sum_kfkcsl;
            dr2["KFKCJE"] = sum_kfkcje;
            dr2["SYSL"] = sum_sysl;
            dr2["SYJE"] = sum_syje;
            dr2["KSKCSL"] = sum_kskcsl;
            dr2["KSKCJE"] = sum_kskcje;
            table.Rows.Add(dr2);

            List<string> colmns = new List<string>();
            colmns.Add("序号");
            for (int index = 1; index < GridDeptKC.Columns.Count; index++)
            {
                GridColumn column = GridDeptKC.Columns[index];
                if (column is FineUIPro.BoundField)
                {
                    table.Columns[((FineUIPro.BoundField)(column)).DataField.ToUpper()].ColumnName = column.HeaderText;
                    colmns.Add(column.HeaderText);
                }
                else if (column is FineUIPro.GroupField)
                {
                    FineUIPro.GroupField group = column as FineUIPro.GroupField;
                    for (int i = 0; i < group.Columns.Count; i++)
                    {
                        GridColumn column1 = group.Columns[i];
                        if (column1 is FineUIPro.BoundField)
                        {
                            table.Columns[((FineUIPro.BoundField)(column1)).DataField.ToUpper()].ColumnName = column1.HeaderText;
                            colmns.Add(column1.HeaderText);
                        }
                    }
                }
            }
            ExcelHelper.ExportByWeb(table.DefaultView.ToTable(true, colmns.ToArray()), "医疗物资供应商对账汇总表", string.Format("医疗物资供应商对账汇总表_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }
    }
}