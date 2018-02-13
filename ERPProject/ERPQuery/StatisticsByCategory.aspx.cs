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
    public partial class StatisticsByCategory : System.Web.UI.Page
    {
        private static DataTable dtSource;
        #region Page_Init

        // 注意：动态创建的代码需要放置于Page_Init（不是Page_Load），这样每次构造页面时都会执行
        protected void Page_Init(object sender, EventArgs e)
        {
            dtSource = new DataTable();
            InitGrid();
        }

        private void InitGrid()
        {
            DataTable table = new DataTable();
            object objCache = CacheHelper.GetCache("STATISTICS_TYPE");
            if (objCache != null)
            {
                table = objCache as DataTable;
            }
            else
            {
                table = DbHelperOra.Query("SELECT CODE,NAME,SJCODE,CLASS FROM SYS_CATEGORY ORDER BY CODE").Tables[0];
                CacheHelper.SetCache("STATISTICS_TYPE", table, TimeSpan.FromMinutes(30.0));
            }

            if (table != null && table.Rows.Count > 0)
            {
                dtSource.Columns.Add("DEPTNAME", Type.GetType("System.String"));
                DataRow[] drs0 = table.Select("SJCODE='HC01'");

                foreach (DataRow row in drs0)
                {
                    DataRow[] drs = table.Select("SJCODE='" + row["CODE"].ToString() + "'");
                    if (drs.Length > 0)
                    {
                        GroupField gf = new GroupField();
                        gf.HeaderText = row["NAME"].ToString();
                        gf.TextAlign = FineUIPro.TextAlign.Center;
                        GridBoundAdd(GridJXC, drs, table, gf);
                        GridJXC.Columns.Add(gf);
                    }
                    else
                    {
                        GroupField gf2 = new GroupField();
                        gf2.HeaderText = row["NAME"].ToString();
                        gf2.TextAlign = FineUIPro.TextAlign.Center;
                    }
                }
            }
        }

        private void GridBoundAdd(FineUIPro.Grid grid, DataRow[] rows, DataTable table, GroupField gf)
        {
            foreach (DataRow row in rows)
            {
                DataRow[] drs = table.Select("SJCODE='" + row["CODE"].ToString() + "'");
                if (drs.Length > 0)
                {
                    GroupField gf1 = new GroupField();
                    gf1.HeaderText = row["NAME"].ToString();
                    gf1.TextAlign = FineUIPro.TextAlign.Center;
                    GridBoundAdd(grid, drs, table, gf1);
                    gf.Columns.Add(gf1);
                }
                else
                {
                    GroupField gf2 = new GroupField();
                    gf2.HeaderText = row["NAME"].ToString();
                    gf2.TextAlign = FineUIPro.TextAlign.Center;

                    FineUIPro.BoundField bfjs = new FineUIPro.BoundField();
                    bfjs.DataField = row["CODE"].ToString() + "_JS";
                    bfjs.HeaderText = "入库数量";
                    bfjs.TextAlign = FineUIPro.TextAlign.Right;
                    gf2.Columns.Add(bfjs);
                    FineUIPro.BoundField bfje = new FineUIPro.BoundField();
                    bfje.DataField = row["CODE"].ToString() + "_JE";
                    bfje.HeaderText = "入库金额";
                    bfje.TextAlign = FineUIPro.TextAlign.Right;
                    gf2.Columns.Add(bfje);
                    FineUIPro.BoundField bfxs = new FineUIPro.BoundField();
                    bfxs.DataField = row["CODE"].ToString() + "_XS";
                    bfxs.HeaderText = "出库数量";
                    bfxs.TextAlign = FineUIPro.TextAlign.Right;
                    gf2.Columns.Add(bfxs);
                    FineUIPro.BoundField bfxe = new FineUIPro.BoundField();
                    bfxe.DataField = row["CODE"].ToString() + "_XE";
                    bfxe.HeaderText = "出库金额";
                    bfxe.TextAlign = FineUIPro.TextAlign.Right;
                    gf2.Columns.Add(bfxe);
                    FineUIPro.BoundField bfcs = new FineUIPro.BoundField();
                    bfcs.DataField = row["CODE"].ToString() + "_CS";
                    bfcs.HeaderText = "库存数量";
                    bfcs.TextAlign = FineUIPro.TextAlign.Right;
                    gf2.Columns.Add(bfcs);
                    FineUIPro.BoundField bfce = new FineUIPro.BoundField();
                    bfce.DataField = row["CODE"].ToString() + "_CE";
                    bfce.HeaderText = "库存数量";
                    bfce.TextAlign = FineUIPro.TextAlign.Right;
                    gf2.Columns.Add(bfce);
                    gf.Columns.Add(gf2);

                    dtSource.Columns.Add(row["CODE"].ToString() + "_JS", Type.GetType("System.Decimal"));
                    dtSource.Columns.Add(row["CODE"].ToString() + "_JE", Type.GetType("System.Decimal"));
                    dtSource.Columns.Add(row["CODE"].ToString() + "_XS", Type.GetType("System.Decimal"));
                    dtSource.Columns.Add(row["CODE"].ToString() + "_XE", Type.GetType("System.Decimal"));
                    dtSource.Columns.Add(row["CODE"].ToString() + "_CS", Type.GetType("System.Decimal"));
                    dtSource.Columns.Add(row["CODE"].ToString() + "_CE", Type.GetType("System.Decimal"));
                }
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
            }
        }

        private void DataInit()
        {
            PubFunc.DdlDataGet("DDL_SYS_DEPTDEF", ddlDEPTID);
            lstLRRQ1.SelectedDate = DateTime.Now.AddMonths(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
            dpkRQ1.SelectedDate = DateTime.Now.AddMonths(-1);
            dpkRQ2.SelectedDate = DateTime.Now;

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
            // 绑定到下拉列表（启用模拟树功能）
            docCategory.EnableSimulateTree = true;
            docCategory.DataTextField = "Name";
            docCategory.DataValueField = "Id";
            //ddlCATID.DataEnableSelectField = "EnableSelect";
            docCategory.DataSimulateTreeLevelField = "Level";
            docCategory.DataSource = myList;
            docCategory.DataBind();
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
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat(@"SELECT S.CODE CATEGORYID,
                                                           S.NAME CATEGORYNAME,
                                                           NVL(KF.RKSL, 0) RKSL,
                                                           NVL(KF.RKJE, 0) RKJE,
                                                           NVL(ABS(KF.CKSL), 0) CKSL,
                                                           NVL(ABS(KF.CKJE), 0) CKJE,
                                                           NVL(ABS(SY.SYSL), 0) SYSL,
                                                           NVL(ABS(SY.SYJE), 0) SYJE,
                                                           NVL(KSKC.KCSL, 0) KSKCSL,
                                                           NVL(KSKC.KCJE, 0) KSKCJE,
                                                           NVL(KFKC.KCSL, 0) KFKCSL,
                                                           NVL(KFKC.KCJE, 0) KFKCJE
                                                      FROM SYS_CATEGORY S,
                                                           (SELECT J.CATID,
                                                                   SUM(DECODE(J.BILLTYPE, 'RKD', J.SL, 'THD', J.SL, 0)) RKSL,
                                                                   SUM(DECODE(J.BILLTYPE, 'RKD', J.HSJE, 'THD', J.HSJE, 0)) RKJE,
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
                                                                              0)) CKSL,
                                                                   SUM(DECODE(J.BILLTYPE,
                                                                              'LCD',
                                                                              J.HSJE,
                                                                              'DSC',
                                                                              J.HSJE,
                                                                              'CKD',
                                                                              J.HSJE,
                                                                              'LTD',
                                                                              J.HSJE,
                                                                              'DST',
                                                                              J.HSJE,
                                                                              0)) CKJE
                                                              FROM DAT_GOODSJXC J
                                                             WHERE J.DEPTID IN (SELECT CODE
                                                                                  FROM SYS_DEPT
                                                                                 WHERE TYPE IN ('1', '2')
                                                                                   AND ISLAST = 'Y')
                                                               AND J.CATID LIKE '%{2}%'
                                                               AND J.GDSEQ IN
                                                                   (SELECT GDSEQ FROM DOC_GOODS WHERE ISGZ LIKE '%{3}%')
                                                               AND J.RQSJ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                                   TO_DATE('{1}', 'YYYY-MM-DD') + 1
                                                             GROUP BY J.CATID) KF,
                                                           (SELECT J.CATID,
                                                                   SUM(DECODE(J.BILLTYPE,
                                                                              'XSD',
                                                                              J.SL,
                                                                              'XSG',
                                                                              J.SL,
                                                                              'DSH',
                                                                              J.SL,
                                                                              'XST',
                                                                              J.SL,
                                                                              0)) SYSL,
                                                                   SUM(DECODE(J.BILLTYPE,
                                                                              'XSD',
                                                                              J.HSJE,
                                                                              'XSG',
                                                                              J.HSJE,
                                                                              'DSH',
                                                                              J.HSJE,
                                                                              'XST',
                                                                              J.HSJE,
                                                                              0)) SYJE
                                                              FROM DAT_GOODSJXC J
                                                             WHERE J.DEPTID IN (SELECT CODE
                                                                                  FROM SYS_DEPT
                                                                                 WHERE TYPE IN ('3', '4')
                                                                                   AND ISLAST = 'Y')
                                                               AND J.CATID LIKE '%{2}%'
                                                               AND J.GDSEQ IN
                                                                   (SELECT GDSEQ FROM DOC_GOODS WHERE ISGZ LIKE '%{3}%')
                                                               AND J.RQSJ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                                   TO_DATE('{1}', 'YYYY-MM-DD') + 1
                                                             GROUP BY J.CATID) SY,
                                                           (SELECT G.CATID, SUM(G.KCSL) KCSL, SUM(G.KCSL * A.HSJJ) KCJE
                                                              FROM DAT_GOODSSTOCK G, DOC_GOODS A
                                                             WHERE G.CATID LIKE '%{2}%'
                                                               AND G.GDSEQ = A.GDSEQ
                                                               AND G.DEPTID IN (SELECT CODE
                                                                                  FROM SYS_DEPT
                                                                                 WHERE TYPE IN ('3', '4')
                                                                                   AND ISLAST = 'Y')
                                                               AND A.ISGZ LIKE '%{3}%'
                                                             GROUP BY G.CATID) KSKC,
                                                           (SELECT G.CATID, SUM(G.KCSL) KCSL, SUM(G.KCSL * A.HSJJ) KCJE
                                                              FROM DAT_GOODSSTOCK G, DOC_GOODS A
                                                             WHERE G.CATID LIKE '%{2}%'
                                                               AND G.GDSEQ = A.GDSEQ
                                                               AND G.DEPTID IN (SELECT CODE
                                                                                  FROM SYS_DEPT
                                                                                 WHERE TYPE IN ('1', '2')
                                                                                   AND ISLAST = 'Y')
                                                               AND A.ISGZ LIKE '%{3}%'
                                                             GROUP BY G.CATID) KFKC
                                                     WHERE S.CODE = SY.CATID(+)
                                                       AND S.CODE = KSKC.CATID(+)
                                                       AND S.CODE = KFKC.CATID(+)
                                                       AND S.CODE = KF.CATID(+)
                                                     ORDER BY S.NAME", dpkRQ1.Text, dpkRQ2.Text, docCategory.SelectedValue, ddlISGZ.SelectedValue);
            DataTable dt = DbHelperOra.Query(sbSql.ToString()).Tables[0];
            GridCategoryStock.DataSource = dt;
            GridCategoryStock.DataBind();

            decimal kfkcsl_total = 0, kfkcje_total = 0, kskcsl_total = 0, kskcje_total = 0, rksl_total = 0, rkje_total = 0, cksl_total = 0, ckje_total = 0, sysl_total = 0, syje_total = 0;
            foreach (DataRow row in dt.Rows)
            {
                rksl_total += Convert.ToDecimal(row["RKSL"]);
                rkje_total += Convert.ToDecimal(row["RKJE"]);
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
            summary.Add("TYPENAME", "汇总合计");
            summary.Add("RKSL", rksl_total.ToString("F2"));
            summary.Add("RKJE", rkje_total.ToString("F2"));
            summary.Add("CKSL", cksl_total.ToString("F2"));
            summary.Add("CKJE", ckje_total.ToString("F2"));
            summary.Add("SYSL", sysl_total.ToString("F2"));
            summary.Add("SYJE", syje_total.ToString("F2"));
            summary.Add("KSKCSL", kskcsl_total.ToString("F2"));
            summary.Add("KSKCJE", kskcje_total.ToString("F2"));
            summary.Add("KFKCSL", kfkcsl_total.ToString("F2"));
            summary.Add("KFKCJE", kfkcje_total.ToString("F2"));
            GridCategoryStock.SummaryData = summary;
        }

        protected void GridCategoryStock_RowDataBound(object sender, FineUIPro.GridRowEventArgs e)
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

        protected void GridCategoryStock_RowDoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {

        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            lstLRRQ1.SelectedDate = DateTime.Now;
            lstLRRQ2.SelectedDate = DateTime.Now;
            ddlDEPTID.SelectedValue = "";
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {

        }

        protected void btnSearch_Click(object sender, EventArgs e)
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

            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat(@"SELECT C.NAME DEPTNAME,A.*, B.KCSL, B.KCJE
                                      FROM (SELECT J.DEPTID,
                                                   J.CATID,
                                                   SUM(DECODE(J.KCADD, '1', J.SL, 0)) RKSL,
                                                   SUM(DECODE(J.KCADD, '1', J.HSJE, 0)) RKJE,
                                                   ABS(SUM(DECODE(J.KCADD, '-1', J.SL, 0))) CKSL,
                                                   ABS(SUM(DECODE(J.KCADD, '-1', J.HSJE, 0))) CKJE
                                              FROM DAT_GOODSJXC J 
                                            WHERE J.RQSJ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                                   TO_DATE('{1}', 'YYYY-MM-DD') + 1
                                                AND J.DEPTID LIKE '%{2}%'
                                             GROUP BY J.DEPTID, J.CATID) A,
                                           (SELECT S.CATID,
                                                   S.DEPTID,
                                                   SUM(S.KCSL) KCSL,
                                                   SUM(S.KCSL * G.HSJJ) KCJE
                                              FROM DAT_GOODSSTOCK S, DOC_GOODS G
                                             WHERE S.GDSEQ = G.GDSEQ AND S.DEPTID LIKE '%{2}%'
                                             GROUP BY S.CATID, S.DEPTID) B,
                                           (SELECT CODE, NAME FROM SYS_DEPT WHERE ISLAST = 'Y') C
                                     WHERE A.CATID = B.CATID
                                       AND A.DEPTID = B.DEPTID AND C.CODE LIKE '%{2}%'
                                       AND C.CODE = A.DEPTID(+) ORDER BY C.NAME
             ", lstLRRQ1.Text, lstLRRQ2.Text, ddlDEPTID.SelectedValue);
            DataTable table = DbHelperOra.Query(sbSql.ToString()).Tables[0];
            if (table != null && table.Rows.Count > 0)
            {
                DataTable dtCategoryJXC = dtSource.Clone();
                DataTable dtDept = table.DefaultView.ToTable(true, new string[] { "DEPTID", "DEPTNAME" });
                foreach (DataRow row in dtDept.Rows)
                {
                    DataRow dr = dtCategoryJXC.NewRow();
                    dr["DEPTNAME"] = row["DEPTNAME"];
                    foreach (DataColumn dc in dtCategoryJXC.Columns)
                    {
                        if (dc.ColumnName.IndexOf("_") < 0) continue;
                        string category = dc.ColumnName.Substring(0, dc.ColumnName.Length - 3);
                        DataRow[] drs = table.Select("DEPTID='" + row["DEPTID"].ToString() + "' AND CATID='" + category + "'");
                        if (drs.Length > 0)
                        {
                            if (dc.ColumnName.EndsWith("_JS"))
                            {
                                dr[dc.ColumnName] = drs[0]["RKSL"];
                            }
                            else if (dc.ColumnName.EndsWith("_JE"))
                            {
                                dr[dc.ColumnName] = drs[0]["RKJE"];
                            }
                            else if (dc.ColumnName.EndsWith("_XS"))
                            {
                                dr[dc.ColumnName] = drs[0]["CKSL"];
                            }
                            else if (dc.ColumnName.EndsWith("_XE"))
                            {
                                dr[dc.ColumnName] = drs[0]["CKJE"];
                            }
                            else if (dc.ColumnName.EndsWith("_CS"))
                            {
                                dr[dc.ColumnName] = drs[0]["KCSL"];
                            }
                            else if (dc.ColumnName.EndsWith("_CE"))
                            {
                                dr[dc.ColumnName] = drs[0]["KCJE"];
                            }
                        }
                    }
                    dtCategoryJXC.Rows.Add(dr);
                }
                GridJXC.DataSource = dtCategoryJXC;
                GridJXC.DataBind();
            }
        }
    }
}