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
using Oracle.ManagedDataAccess.Client;

namespace ERPProject.ERPQuery
{
    public partial class DepartmentUseQuery : PageBase
    {
        private static DataTable dtSource;

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
                table = DbHelperOra.Query("SELECT CODE,NAME FROM SYS_CATEGORY WHERE SJCODE='0' ").Tables[0];
                CacheHelper.SetCache("STATISTICS_TYPE", table, TimeSpan.FromMinutes(30.0));
            }

            if (table != null && table.Rows.Count > 0)
            {
                dtSource.Columns.Add("DEPTID", Type.GetType("System.String"));
                dtSource.Columns.Add("DEPTNAME", Type.GetType("System.String"));
                dtSource.Columns.Add("TOTALJE", Type.GetType("System.String"));

                foreach (DataRow row in table.Rows)
                {
                    dtSource.Columns.Add(row["CODE"].ToString(), Type.GetType("System.String"));

                    FineUIPro.BoundField category = new FineUIPro.BoundField();
                    category.DataField = row["CODE"].ToString();
                    category.ColumnID = row["CODE"].ToString();
                    category.HeaderText = row["NAME"].ToString();
                    category.TextAlign = FineUIPro.TextAlign.Right;
                    GridDeptKC.Columns.Add(category);
                }

                FineUIPro.BoundField total = new FineUIPro.BoundField();
                total.DataField = "TOTALJE";
                total.ColumnID = "TOTALJE";
                total.HeaderText = "总计";
                total.TextAlign = FineUIPro.TextAlign.Right;
                GridDeptKC.Columns.Add(total);
            }
        }

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
            PubFunc.DdlDataGet("DDL_SYS_DEPTDEF", ddlDept, ddlDEPTID);
            PubFunc.DdlDataGet("DDL_GOODSTYPE", docDHLX, docDHLX1, docDHLX2);
            docDHLX.SelectedValue = "2";
            docDHLX1.SelectedValue = "2";
            docDHLX2.SelectedValue = "2";
            lstLRRQ1.SelectedDate = DateTime.Now.AddMonths(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
            dpkBegRQ.SelectedDate = DateTime.Now.AddMonths(-1);
            dpkEndRQ.SelectedDate = DateTime.Now;
            dpkRQ1.SelectedDate = DateTime.Now.AddMonths(-1);
            dpkRQ2.SelectedDate = DateTime.Now;

            DataTable table = new DataTable();
            object objCache = CacheHelper.GetCache("STATISTICS_TYPE");
            if (objCache != null)
            {
                DataTable dt = objCache as DataTable;
                table = dt.Copy();
            }
            else
            {
                table = DbHelperOra.Query("SELECT CODE,NAME FROM SYS_CATEGORY WHERE SJCODE='HC01' ").Tables[0];
                CacheHelper.SetCache("STATISTICS_TYPE", table, TimeSpan.FromMinutes(30.0));
            }
            DataRow dr = table.NewRow();
            dr["CODE"] = "";
            dr["NAME"] = "---请选择---";
            table.Rows.InsertAt(dr, 0);
            ddlCATID.DataTextField = "NAME";
            ddlCATID.DataValueField = "CODE";
            ddlCATID.DataSource = table;
            ddlCATID.DataBind();
        }

        private string GetSearchSql()
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat(@"SELECT D.NAME DEPTNAME,
                                                   B.GDSEQ,
                                                   B.GDNAME,
                                                   B.GDSPEC,
                                                   F_GETUNITNAME(B.UNIT) UNIT,
                                                   C.NAME CATTYPE,
                                                   B.HSJJ,B.HSID,
                                                   DECODE(B.ISCF,'Y','是','否') ISSF,
                                                   F_GETPRODUCERNAME(B.PRODUCER) PRODUCER,
                                                   (-SUM(DECODE(A.BILLTYPE,'XST',DECODE(A.KCADD,'1',A.SL,0),A.SL))) SL,
                                                   (-SUM(DECODE(A.BILLTYPE,'XST',DECODE(A.KCADD,'1',A.SL,0),A.SL) * A.HSJJ)) JE
                                              FROM DAT_GOODSJXC A, DOC_GOODS B, SYS_CATEGORY C, SYS_DEPT D
                                             WHERE A.GDSEQ = B.GDSEQ
                                               AND A.DEPTID = D.CODE
                                               AND B.CATID0='{2}'
                                               AND B.CATID = C.CODE
                                               AND A.BILLTYPE IN ('DSH','XSD','XSG','XST')
                                               AND A.RQSJ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND TO_DATE('{1}', 'YYYY-MM-DD') + 1
                                               AND A.DEPTID IN (SELECT CODE FROM SYS_DEPT WHERE TYPE IN ('3', '4')) ", lstLRRQ1.Text, lstLRRQ2.Text, docDHLX2.SelectedValue);
            if (!PubFunc.StrIsEmpty(ddlDEPTID.SelectedValue))
            {
                sbSql.AppendFormat(" AND A.DEPTID='{0}'", ddlDEPTID.SelectedValue);
            }
            if (!PubFunc.StrIsEmpty(ddlCATID.SelectedValue))
            {
                sbSql.AppendFormat(" AND SUBSTR(A.CATID,1,6)='{0}'", ddlCATID.SelectedValue);
            }
            if (!PubFunc.StrIsEmpty(tbxGOODS.Text.Trim()))
            {
                sbSql.AppendFormat(" AND A.GDSEQ='{0}'", tbxGOODS.Text.Trim());
            }
            sbSql.Append(@"   GROUP BY D.NAME,
                                                        B.GDSEQ,
                                                        B.GDNAME,
                                                        B.GDSPEC,B.ISCF,
                                                        B.UNIT,C.NAME,
                                                        B.HSJJ,B.HSID,
                                                        C.NAME,
                                                        B.PRODUCER
                                             ORDER BY D.NAME, B.GDNAME");
            return sbSql.ToString();
            //string strSql = @"SELECT SD.NAME DEPTNAME,B.GDSEQ,B.GDNAME,B.GDSPEC,G.HSID,DECODE(G.ISCF,'Y','是','否') ISCF,C.NAME CATTYPE,
            //                                     F_GETUNITNAME(B.UNIT) UNIT,B.HSJJ,SUM(B.XSSL) SL,SUM(B.HSJE) JE,F_GETPRODUCERNAME(G.PRODUCER) PRODUCER
            //                           FROM DAT_XS_DOC A, DAT_XS_COM B, SYS_DEPT SD, DOC_GOODS G,SYS_CATEGORY C
            //                          WHERE A.SEQNO = B.SEQNO AND B.GDSEQ = G.GDSEQ AND A.DEPTID = SD.CODE  AND A.FLAG='Y' AND G.CATID=C.CODE ";
            //string strWhere = " ";

            ////if (!PubFunc.StrIsEmpty(SELECTSUPID.Text.Trim())) strWhere += " AND A.SUPID = '" + SELECTSUPID.Text.Trim() + "'";
            //if (!PubFunc.StrIsEmpty(ddlDEPTID.SelectedValue)) strWhere += " AND A.DEPTID = '" + ddlDEPTID.SelectedValue + "'";
            //if (!PubFunc.StrIsEmpty(tbxGOODS.Text)) strWhere += " and (G.GDSEQ like '%" + tbxGOODS.Text + "%' or g.zjm like '%" + tbxGOODS.Text + "%' or g.gdname like '%" + tbxGOODS.Text + "%')";

            ////if (!PubFunc.StrIsEmpty(SELECTPRODUCERID.Text.Trim())) strWhere += " and g.producer = '" + SELECTPRODUCERID.Text.Trim() + "'";
            //strWhere += string.Format(" and A.SHRQ>=TO_DATE('{0}','YYYY-MM-DD') and A.SHRQ < TO_DATE('{1}','YYYY-MM-DD')+1 ", lstLRRQ1.Text, lstLRRQ2.Text);
            ////strWhere += string.Format(" AND a.deptid in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            //if (strWhere != " ") strSql = strSql + strWhere;

            //strSql += string.Format(" GROUP BY SD.NAME, B.GDSEQ, B.GDNAME, B.GDSPEC, B.UNIT, B.HSJJ,C.NAME,G.HSID,G.ISCF,G.PRODUCER ORDER BY {0} {1}", GridGoods.SortField, GridGoods.SortDirection);
            //return strSql;
        }

        private string GetSearchSql1()
        {
            //string strSql = @"SELECT B.GDSEQ,B.GDNAME,B.GDSPEC,F_GETUNITNAME(B.UNIT) UNIT,B.HSJJ,SUM(B.XSSL) SL,SUM(B.HSJE) JE,G.HISCODE
            //                           FROM DAT_CK_DOC A, DAT_CK_COM B, DOC_GOODS G
            //                          WHERE A.SEQNO = B.SEQNO AND B.GDSEQ = G.GDSEQ AND A.FLAG='Y' ";
            //string strWhere = " ";

            //if (!PubFunc.StrIsEmpty(trbSearch.Text)) strWhere += " and (G.GDSEQ like '%" + trbSearch.Text + "%' or g.zjm like '%" + trbSearch.Text + "%' or g.gdname like '%" + trbSearch.Text + "%')";

            //strWhere += string.Format(" and A.SHRQ>=TO_DATE('{0}','YYYY-MM-DD') and A.SHRQ < TO_DATE('{1}','YYYY-MM-DD')+1 ", dpkBegRQ.Text, dpkEndRQ.Text);
            //if (strWhere != " ") strSql = strSql + strWhere;

            //strSql += " GROUP BY B.GDSEQ, B.GDNAME, B.GDSPEC, B.UNIT, B.HSJJ, G.HISCODE";
            //return strSql;

            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat(@"SELECT B.GDSEQ,
                                                   B.GDNAME,
                                                   B.GDSPEC,
                                                   F_GETUNITNAME(B.UNIT) UNIT,
                                                   C.NAME CATTYPE,
                                                   B.HSJJ,B.HISCODE,
                                                   DECODE(B.ISCF,'Y','是','否') ISSF,
                                                   F_GETPRODUCERNAME(B.PRODUCER) PRODUCER,
                                                   (-SUM(DECODE(A.BILLTYPE,'XST',DECODE(A.KCADD,'1',A.SL,0),A.SL))) SL,
                                                   (-SUM(DECODE(A.BILLTYPE,'XST',DECODE(A.KCADD,'1',A.SL,0),A.SL) * A.HSJJ)) JE
                                              FROM DAT_GOODSJXC A, DOC_GOODS B, SYS_CATEGORY C
                                             WHERE A.GDSEQ = B.GDSEQ
                                               AND B.CATID = C.CODE
                                               AND A.BILLTYPE IN ('DSH','XSD','XSG','XST')
                                               AND A.RQSJ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND TO_DATE('{1}', 'YYYY-MM-DD') + 1
                                               AND B.CATID0='{2}'
                                               AND A.DEPTID IN (SELECT CODE FROM SYS_DEPT WHERE TYPE IN ('3', '4')) ", dpkBegRQ.Text, dpkEndRQ.Text, docDHLX1.SelectedValue);

            if (!PubFunc.StrIsEmpty(trbSearch.Text.Trim()))
            {
                sbSql.AppendFormat(" AND (A.GDSEQ='%{0}%' OR B.GDNAME LIKE '%{0}%'  OR B.GDSPEC LIKE '%{0}%' OR UPPER(B.ZJM) LIKE UPPER('%{0}%'))", trbSearch.Text.Trim());
            }
            sbSql.Append(@"   GROUP BY B.GDSEQ,
                                                        B.GDNAME,
                                                        B.GDSPEC,B.ISCF,
                                                        B.UNIT,
                                                        B.HSJJ,B.HISCODE,
                                                        C.NAME,
                                                        B.PRODUCER
                                             ORDER BY B.GDNAME,B.GDSPEC");
            return sbSql.ToString();
        }

        private void DataSearch()
        {
            int total = 0;
            lblSUBNUM.Text = "0";
            lblSUBSUM.Text = "0";
            DataTable dtSum = DbHelperOra.Query("SELECT SUM(NVL(SL,0)) SL,SUM(NVL(JE,0)) JE FROM (" + GetSearchSql() + ")").Tables[0];
            if (dtSum.Rows.Count > 0)
            {
                lblSUBNUM.Text = dtSum.Rows[0]["JE"].ToString();
                lblSUBSUM.Text = dtSum.Rows[0]["SL"].ToString();
            }
            if (docDHLX2.SelectedValue.Length < 1)
            {
                Alert.Show("请选择转商品类型！", MessageBoxIcon.Warning);
                return;
            }
            DataTable dtData = PubFunc.DbGetPage(GridGoods.PageIndex, GridGoods.PageSize, GetSearchSql(), ref total);
            OutputSummaryData(dtData);
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
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("序号", typeof(int)));
            foreach (DataColumn col in dtData.Columns)
            {
                table.Columns.Add(new DataColumn(col.ColumnName, col.DataType));
            }
            decimal rowno = 0, deptcount = 0, sum = 0, num = 0, sum_total = 0, num_total = 0;
            string dept = dtData.Rows[0]["DEPTNAME"].ToString();
            foreach (DataRow row in dtData.Rows)
            {
                //每个科室最后增加一行小计行
                if (dept != row["DEPTNAME"].ToString())
                {
                    dept = row["DEPTNAME"].ToString();
                    DataRow dr1 = table.NewRow();

                    dr1["DEPTNAME"] = deptcount;
                    dr1["SL"] = num;
                    dr1["JE"] = sum;
                    table.Rows.Add(dr1);

                    deptcount = 0;
                    sum = 0;
                    num = 0;
                }

                DataRow dr = table.NewRow();
                deptcount++;
                dr["序号"] = ++rowno;
                foreach (DataColumn col in dtData.Columns)
                {
                    dr[col.ColumnName] = row[col.ColumnName];
                }
                table.Rows.Add(dr);

                sum += decimal.Parse(row["JE"].ToString());
                num += decimal.Parse(row["SL"].ToString());
                sum_total += decimal.Parse(row["JE"].ToString());
                num_total += decimal.Parse(row["SL"].ToString());
                //最后一行时增加小计行
                if (rowno == dtData.Rows.Count)
                {
                    DataRow dr2 = table.NewRow();

                    dr2["DEPTNAME"] = deptcount;
                    dr2["SL"] = num;
                    dr2["JE"] = sum;
                    table.Rows.Add(dr2);
                }
            }
            //添加汇总合计行
            DataRow dr3 = table.NewRow();
            dr3["DEPTNAME"] = "汇总合计";
            dr3["SL"] = num_total;
            dr3["JE"] = sum_total;
            table.Rows.Add(dr3);

            string[] columnNames = new string[GridGoods.Columns.Count];
            columnNames[0] = "序号";
            for (int index = 1; index < GridGoods.Columns.Count; index++)
            {
                GridColumn column = GridGoods.Columns[index];
                if (column is FineUIPro.BoundField)
                {
                    table.Columns[((FineUIPro.BoundField)(column)).DataField.ToUpper()].ColumnName = column.HeaderText;
                    columnNames[index] = column.HeaderText;
                }
            }
            ExcelHelper.ExportByWeb(table.DefaultView.ToTable(true, columnNames), string.Format("{0:yyyy年MM月}", lstLRRQ2.SelectedDate) + "医疗物资科室领用明细", string.Format("医疗物资科室领用明细表_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
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


        private void OutputSummaryData(DataTable source)
        {
            decimal HSJJTotal = 0, HSJETotal = 0;
            foreach (DataRow row in source.Rows)
            {
                HSJJTotal += Convert.ToInt32(row["SL"]);
                HSJETotal += Convert.ToDecimal(row["JE"]);
            }
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("SL", HSJJTotal.ToString("F2"));
            summary.Add("JE", HSJETotal.ToString("F2"));
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
            List<string> columnNames = new List<string>();
            for (int index = 1; index < GridCom.Columns.Count; index++)
            {
                GridColumn column = GridCom.Columns[index];
                if (column != null && column.Hidden == false && column is FineUIPro.BoundField)
                {
                    dtData.Columns[((FineUIPro.BoundField)(column)).DataField.ToUpper()].ColumnName = column.HeaderText;
                    columnNames.Add(column.HeaderText);
                }
            }

            ExcelHelper.ExportByWeb(dtData.DefaultView.ToTable(false, columnNames.ToArray()), "商品出库信息汇总", string.Format("商品出库信息汇总_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
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

            if (docDHLX1.SelectedValue.Length < 1)
            {
                Alert.Show("请选择转商品类型！", MessageBoxIcon.Warning);
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
            ddlDEPTID.SelectedValue = GridDeptKC.Rows[e.RowIndex].DataKeys[0].ToString();
            docDHLX2.SelectedValue = docDHLX.SelectedValue;
            docDHLX2.Enabled = false;
            DataSearch();
            TabStrip1.ActiveTabIndex = 2;
        }

        private DataTable GetSumData()
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat(@"SELECT D.CODE DEPTID,
                                                           D.NAME DEPTNAME,
                                                           SUBSTR(A.CATID,1,6) CATID,
                                                           -SUM(DECODE(A.BILLTYPE,
                                                                          'XSD',
                                                                          A.SL * A.HSJJ,
                                                                          'XSG',
                                                                          A.SL * A.HSJJ,
                                                                          'DSH',
                                                                          A.SL * A.HSJJ,
                                                                          'SYD',
                                                                          A.SL * A.HSJJ,
                                                                          'XST',
                                                                          DECODE(A.KCADD, '1', A.SL * A.HSJJ, 0),
                                                                          0)) JE
                                                      FROM DAT_GOODSJXC A, SYS_DEPT D, DOC_GOODS B
                                                     WHERE A.DEPTID = D.CODE
                                                       AND A.GDSEQ = B.GDSEQ
                                                       AND B.CATID0 = '{2}'
                                                       AND A.RQSJ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                               TO_DATE('{1}', 'YYYY-MM-DD') + 1
                                                       AND A.DEPTID IN (SELECT CODE FROM SYS_DEPT WHERE TYPE IN ('3', '4'))", dpkRQ1.Text, dpkRQ2.Text, docDHLX.SelectedValue);
            if (!string.IsNullOrWhiteSpace(ddlDept.SelectedValue))
            {
                sbSql.AppendFormat(" AND A.DEPTID='{0}'", ddlDept.SelectedValue);
            }
            if (!string.IsNullOrWhiteSpace(ddlISGZ.SelectedValue))
            {
                sbSql.AppendFormat(" AND A.GDSEQ IN (SELECT GDSEQ FROM DOC_GOODS WHERE ISGZ='{0}')", ddlISGZ.SelectedValue);
            }
            sbSql.Append(@"   GROUP BY D.CODE,D.NAME,SUBSTR(A.CATID,1,6)
                                             ORDER BY D.NAME");
            DataTable table = DbHelperOra.Query(sbSql.ToString()).Tables[0];
            DataTable dtCategoryJXC = dtSource.Clone();
            if (table != null && table.Rows.Count > 0)
            {
                DataTable dtDept = table.DefaultView.ToTable(true, new string[] { "DEPTID", "DEPTNAME" });
                foreach (DataRow row in dtDept.Rows)
                {
                    decimal total_row = 0;
                    DataRow dr = dtCategoryJXC.NewRow();
                    dr["DEPTID"] = row["DEPTID"];
                    dr["DEPTNAME"] = row["DEPTNAME"];
                    foreach (DataColumn dc in dtCategoryJXC.Columns)
                    {
                        string category = dc.ColumnName;
                        DataRow[] drs = table.Select("DEPTID='" + row["DEPTID"].ToString() + "' AND CATID = '" + category + "'");
                        if (drs.Length > 0)
                        {
                            decimal dec_rowsum = 0;
                            foreach (DataRow dr1 in drs)
                            {
                                dec_rowsum += string.IsNullOrWhiteSpace((dr1["JE"] ?? "").ToString()) ? 0 : decimal.Parse(dr1["JE"].ToString());
                            }
                            dr[dc.ColumnName] = dec_rowsum;
                            total_row += dec_rowsum;
                        }
                    }
                    dr["TOTALJE"] = total_row;
                    dtCategoryJXC.Rows.Add(dr);
                }
            }
            return dtCategoryJXC;
        }

        protected void btnQuery_Click(object sender, EventArgs e)
        {
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
            if (docDHLX.SelectedValue.Length < 1)
            {
                Alert.Show("请选择转商品类型！", MessageBoxIcon.Warning);
                return;
            }
            DataTable table = GetSumData();
            if (table != null && table.Rows.Count > 0)
            {
                GridDeptKC.DataSource = table;
                GridDeptKC.DataBind();

                JObject summary = new JObject();
                summary.Add("DEPTNAME", "汇总合计");

                DataTable dt = dtSource.Clone();
                dt.Columns.Remove("DEPTID");
                dt.Columns.Remove("DEPTNAME");
                dt.Columns.Remove("TOTALJE");
                decimal[] total = new decimal[dt.Columns.Count];
                decimal sum_total = 0;
                string[] colid = new string[dt.Columns.Count];
                foreach (DataRow row in table.Rows)
                {
                    decimal dec = 0;
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        DataColumn col = dt.Columns[i];
                        decimal.TryParse((row[col.ColumnName] ?? "0").ToString(), out dec);
                        total[i] += dec;
                        colid[i] = col.ColumnName;
                    }
                    sum_total += decimal.Parse((row["TOTALJE"] ?? "0").ToString());
                }
                for (int index = 0; index < dt.Columns.Count; index++)
                {
                    summary.Add(colid[index], total[index].ToString("F2"));
                }
                summary.Add("TOTALJE", sum_total);
                GridDeptKC.SummaryData = summary;
            }
            else
            {
                Alert.Show("没有查询到相应的数据！");
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
            DataTable dtData = GetSumData();
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

            int rowIndex = 0;
            decimal sum_total = 0, num_total = 0;
            foreach (DataRow row in dtData.Rows)
            {
                DataRow dr = table.NewRow();
                dr["序号"] = ++rowIndex;
                foreach (DataColumn col in dtData.Columns)
                {
                    dr[col.ColumnName] = row[col.ColumnName];
                }
                table.Rows.Add(dr);

            }
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat(@"SELECT D.CODE DEPTID,
                                                           D.NAME DEPTNAME,
                                                           SUBSTR(A.CATID,1,6) CATID,
                                                           -SUM(DECODE(A.BILLTYPE,
                                                                          'XSD',
                                                                          A.SL * A.HSJJ,
                                                                          'XSG',
                                                                          A.SL * A.HSJJ,
                                                                          'DSH',
                                                                          A.SL * A.HSJJ,
                                                                          'SYD',
                                                                          A.SL * A.HSJJ,
                                                                          'XST',
                                                                          DECODE(A.KCADD, '1', A.SL * A.HSJJ, 0),
                                                                          0)) JE
                                                      FROM DAT_GOODSJXC A, SYS_DEPT D, DOC_GOODS B
                                                     WHERE A.DEPTID = D.CODE
                                                       AND A.GDSEQ = B.GDSEQ
                                                       AND B.CATID0 = '{2}'
                                                       AND A.RQSJ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                               TO_DATE('{1}', 'YYYY-MM-DD') + 1
                                                       AND A.DEPTID IN (SELECT CODE FROM SYS_DEPT WHERE TYPE IN ('3', '4'))", dpkRQ1.Text, dpkRQ2.Text, docDHLX.SelectedValue);
            if (!string.IsNullOrWhiteSpace(ddlDept.SelectedValue))
            {
                sbSql.AppendFormat(" AND A.DEPTID='{0}'", ddlDept.SelectedValue);
            }
            if (!string.IsNullOrWhiteSpace(ddlISGZ.SelectedValue))
            {
                sbSql.AppendFormat(" AND A.GDSEQ IN (SELECT GDSEQ FROM DOC_GOODS WHERE ISGZ='{0}')", ddlISGZ.SelectedValue);
            }
            sbSql.Append(@"   GROUP BY D.CODE,D.NAME,SUBSTR(A.CATID,1,6)
                                             ORDER BY D.NAME");
            DataTable table1 = DbHelperOra.Query(sbSql.ToString()).Tables[0];
            foreach (DataRow row in table1.Rows)
            {
                sum_total += decimal.Parse(row["JE"].ToString());

            }
            DataRow dr3 = table.NewRow();
            dr3["DEPTNAME"] = "汇总合计";
            dr3["TOTALJE"] = sum_total;
            table.Rows.Add(dr3);

            string[] columnNames = new string[GridDeptKC.Columns.Count];
            columnNames[0] = "序号";
            for (int index = 1; index < GridDeptKC.Columns.Count; index++)
            {
                GridColumn column = GridDeptKC.Columns[index];
                if (column is FineUIPro.BoundField)
                {
                    table.Columns[((FineUIPro.BoundField)(column)).DataField.ToUpper()].ColumnName = column.HeaderText;
                    columnNames[index] = column.HeaderText;
                }
            }
            ExcelHelper.ExportByWeb(table.DefaultView.ToTable(false, columnNames), string.Format("{0:yyyy年MM月}", dpkRQ2.SelectedDate) + "医疗物资科室领用汇总", string.Format("医疗物资科室领用汇总表_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }

        protected void btnPrint2_Click(object sender, EventArgs e)
        {
            if (dpkRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【查询期间】！");
                return;
            }
            else if (dpkRQ1.SelectedDate > dpkRQ2.SelectedDate)
            {
                Alert.Show("开始日期大于结束日期，请重新输入！");
                return;
            }

            DataTable table = GetSumData();
            if (table == null || table.Rows.Count == 0)
            {
                Alert.Show("没有数据,无法打印！");
                return;
            }
            int rowid = 1;
            table.Columns.Add("ROWNO");
            foreach (DataRow row in table.Rows)
            {
                row["ROWNO"] = rowid;
                rowid++;
            }

            string sql_del = "DELETE FROM TEMP_KSYDBB WHERE BEGRQ='{0}' AND ENDRQ='{1}'";

            DbHelperOra.ExecuteSql(string.Format(sql_del, dpkRQ1.Text, dpkRQ2.Text));

            string sql = String.Format(@"INSERT INTO TEMP_KSYDBB
                                                                  (BEGRQ,
                                                                   ENDRQ,
                                                                   ROWNO,
                                                                   DEPTID,
                                                                   HC0101,
                                                                   HC0102,
                                                                   HC0103,
                                                                   HC0104,
                                                                   HC0105,
                                                                   HC0106,
                                                                   HC0107,
                                                                   HC0108,
                                                                   HC0109,
                                                                   HC0110,
                                                                   HC0111,
                                                                   TOTALJE)
                                                                VALUES
                                                                  ('{0}',
                                                                   '{1}',:ROWNO,
                                                                   :DEPTID,
                                                                   :HC0101,
                                                                   :HC0102,
                                                                   :HC0103,
                                                                   :HC0104,
                                                                   :HC0105,
                                                                   :HC0106,
                                                                   :HC0107,
                                                                   :HC0108,
                                                                   :HC0109,
                                                                   :HC0110,
                                                                   :HC0111,
                                                                   :TOTALJE)
                                                                ", dpkRQ1.Text, dpkRQ2.Text);
            OracleConnection con = new OracleConnection(DbHelperOra.connectionString);
            OracleDataAdapter da = new OracleDataAdapter(sql, con);
            //在批量添加数据前的准备工作
            da.InsertCommand = new OracleCommand(sql, con);
            OracleParameter param = new OracleParameter();

            param = da.InsertCommand.Parameters.Add(new OracleParameter("ROWNO", OracleDbType.Decimal));
            param.SourceVersion = DataRowVersion.Current;
            param.SourceColumn = "ROWNO";

            param = da.InsertCommand.Parameters.Add(new OracleParameter("DEPTID", OracleDbType.Varchar2, 10));
            param.SourceVersion = DataRowVersion.Current;
            param.SourceColumn = "DEPTID";

            param = da.InsertCommand.Parameters.Add(new OracleParameter("HC0101", OracleDbType.Decimal));
            param.SourceVersion = DataRowVersion.Current;
            param.SourceColumn = "HC0101";

            param = da.InsertCommand.Parameters.Add(new OracleParameter("HC0102", OracleDbType.Decimal));
            param.SourceVersion = DataRowVersion.Current;
            param.SourceColumn = "HC0102";

            param = da.InsertCommand.Parameters.Add(new OracleParameter("HC0103", OracleDbType.Decimal));
            param.SourceVersion = DataRowVersion.Current;
            param.SourceColumn = "HC0103";

            param = da.InsertCommand.Parameters.Add(new OracleParameter("HC0104", OracleDbType.Decimal));
            param.SourceVersion = DataRowVersion.Current;
            param.SourceColumn = "HC0104";

            param = da.InsertCommand.Parameters.Add(new OracleParameter("HC0105", OracleDbType.Decimal));
            param.SourceVersion = DataRowVersion.Current;
            param.SourceColumn = "HC0105";

            param = da.InsertCommand.Parameters.Add(new OracleParameter("HC0106", OracleDbType.Decimal));
            param.SourceVersion = DataRowVersion.Current;
            param.SourceColumn = "HC0106";

            param = da.InsertCommand.Parameters.Add(new OracleParameter("HC0107", OracleDbType.Decimal));
            param.SourceVersion = DataRowVersion.Current;
            param.SourceColumn = "HC0107";

            param = da.InsertCommand.Parameters.Add(new OracleParameter("HC0108", OracleDbType.Decimal));
            param.SourceVersion = DataRowVersion.Current;
            param.SourceColumn = "HC0108";

            param = da.InsertCommand.Parameters.Add(new OracleParameter("HC0109", OracleDbType.Decimal));
            param.SourceVersion = DataRowVersion.Current;
            param.SourceColumn = "HC0109";

            param = da.InsertCommand.Parameters.Add(new OracleParameter("HC0110", OracleDbType.Decimal));
            param.SourceVersion = DataRowVersion.Current;
            param.SourceColumn = "HC0110";

            param = da.InsertCommand.Parameters.Add(new OracleParameter("HC0111", OracleDbType.Decimal));
            param.SourceVersion = DataRowVersion.Current;
            param.SourceColumn = "HC0111";

            param = da.InsertCommand.Parameters.Add(new OracleParameter("TOTALJE", OracleDbType.Decimal));
            param.SourceVersion = DataRowVersion.Current;
            param.SourceColumn = "TOTALJE";

            

            //批量添加数据
            try
            {
                con.Open();
                da.Update(table);
            }
            catch (Exception ex)
            {
                Alert.Show("数据库错误：" + ex.Message.ToString(), "异常信息", MessageBoxIcon.Warning);
            }
            finally
            {
                con.Close();
            }

            PageContext.RegisterStartupScript("btnPrint2_onclick()");
        }
    }
}