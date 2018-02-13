using FineUIPro;
using XTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPProject.ERPQuery
{
    public partial class TurnoverAnalysis : PageBase
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
            lstLRRQ2.SelectedDate = DateTime.Now;
            lstLRRQ1.SelectedDate = DateTime.Now.AddMonths(-3);
            PubFunc.DdlDataGet("DDL_SYS_DEPOTRANGE", UserAction.UserID, ddlDEPTID);
            ddlDEPTID.SelectedIndex = 1;
            PubFunc.DdlDataGet("DDL_GOODS_TYPE", ddlCATID);
        }

        private string GetSearchSql()
        {
            string strSql = @"SELECT D.DEPTID,f_getdeptname(D.DEPTID) DEPTIDNAME,NVL(A.QCKC,0) QCKC,NVL(A.QMKC,0) QMKC,B.GDSEQ,B.GDNAME,B.GDSPEC,f_getunitname(B.UNIT) UNITNAME,f_getproducername(B.PRODUCER) PRODUCERNAME,B.HSJJ,B.PIZNO,
                            (CASE WHEN NVL(A.QCKC,0)+ NVL(A.QMKC,0) = 0 THEN 0 ELSE ROUND(NVL(C.SYSL,0)/(NVL(A.QCKC,0)+NVL(A.QMKC,0))*2,2) END) ZZL1,
                            (CASE WHEN NVL(C.SYSL,0) = 0 THEN 0 ELSE ROUND(360*(A.QCKC+A.QMKC)/2/C.SYSL,0) END) ZZL2,
                            NVL(E.KCSL,0) KCSL
                        FROM (SELECT A.DEPTID,A.GDSEQ,SUM(DECODE(TO_CHAR(A.RQ,'YYYY-MM-DD'),TO_CHAR(TO_DATE('{0}', 'yyyy-MM-dd')-1, 'YYYY-MM-DD'),A.KCSL,0)) QCKC ,SUM(DECODE(TO_CHAR(A.RQ,'YYYY-MM-DD'),TO_CHAR(TO_DATE('{1}', 'yyyy-MM-dd') - 1,'YYYY-MM-DD'),A.KCSL,0)) QMKC 
                        FROM V_STOCKALL A
                        WHERE --TO_CHAR(A.RQ,'YYYY-MM-DD') = '{0}' OR TO_CHAR(A.RQ,'YYYY-MM-DD') = '{1}'
                            A.RQ BETWEEN TO_DATE('{0}', 'yyyy-MM-dd')-1 and TO_DATE('{1}', 'yyyy-MM-dd')
                        GROUP BY A.DEPTID,A.GDSEQ) A,DOC_GOODS B,
                        (SELECT A.GDSEQ,A.DEPTID,ABS(SUM(A.SL)) SYSL
                        FROM DAT_GOODSJXC A
                        WHERE A.KCADD = -1 AND A.BILLTYPE NOT IN('RKD','SYD') AND A.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd')
                        GROUP BY A.GDSEQ,A.DEPTID) C,DOC_GOODSCFG D,
                        (SELECT DEPTID,GDSEQ,SUM(KCSL) KCSL
                        FROM DAT_GOODSSTOCK
                        WHERE KCSL > 0
                        GROUP BY DEPTID,GDSEQ) E
                        WHERE D.GDSEQ = A.GDSEQ(+) AND D.GDSEQ = C.GDSEQ(+) AND D.GDSEQ = B.GDSEQ AND D.GDSEQ = E.GDSEQ(+) AND D.DEPTID = A.DEPTID(+) AND D.DEPTID = C.DEPTID(+) AND D.DEPTID = E.DEPTID(+)";
            string strWhere = " ";

            if (!PubFunc.StrIsEmpty(ddlDEPTID.SelectedValue)) strWhere += " AND A.DEPTID = '" + ddlDEPTID.SelectedValue + "'";

            if (!PubFunc.StrIsEmpty(ddlCATID.SelectedValue)) strWhere += " AND B.CATID0 = '" + ddlCATID.SelectedValue + "'";

            if (!PubFunc.StrIsEmpty(ddlISGZ.SelectedValue)) strWhere += " and B.ISGZ = '" + ddlISGZ.SelectedValue + "'";

            if (!PubFunc.StrIsEmpty(tbxGOODS.Text)) strWhere += " AND (UPPER(B.GDSEQ) LIKE '%" + tbxGOODS.Text.ToUpper() + "%' OR UPPER(B.ZJM) LIKE '%" + tbxGOODS.Text.ToUpper() + "%' OR UPPER(B.GDNAME) LIKE '%" + tbxGOODS.Text.ToUpper() + "%')";
            if (strWhere != " ") strSql = strSql + strWhere;

            strSql = string.Format(strSql, lstLRRQ1.Text, Convert.ToDateTime(lstLRRQ2.Text).AddDays(1).ToString("yyyy-MM-dd")) + string.Format(" ORDER BY {0} {1}", GridList.SortField, GridList.SortDirection);
            return strSql;
        }

        private void DataSearch()
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【查询期间】！", MessageBoxIcon.Warning);
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                Alert.Show("【开始日期】大于【结束日期】，请重新输入！", MessageBoxIcon.Warning);
                return;
            }
            if (ddlDEPTID.SelectedValue.Length < 1)
            {
                Alert.Show("请输入条件【库房】！", MessageBoxIcon.Warning);
                return;
            }
            int total = 0;

            DataTable dtData = PubFunc.DbGetPage(GridList.PageIndex, GridList.PageSize, GetSearchSql(), ref total);
            GridList.RecordCount = total;
            GridList.DataSource = dtData;
            GridList.DataBind();
        }

        protected void GridList_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridList.PageIndex = e.NewPageIndex;
            DataSearch();
        }
        protected void btSearch_Click(object sender, EventArgs e)
        {
            DataSearch();
        }
        protected void btClear_Click(object sender, EventArgs e)
        {
            PubFunc.FormDataClear(FormQuery);
            BindDDL();
            ddlCATID.SelectedValue = "";
        }
        protected void btExport_Click(object sender, EventArgs e)
        {
            //string strSql = @"SELECT f_getdeptname(D.DEPTID) 库房,' '||B.GDSEQ 商品编码,B.GDNAME 商品名称,B.GDSPEC 规格容量,f_getunitname(B.UNIT) 单位,f_getproducername(B.PRODUCER) 生产厂家,B.HSJJ 价格,B.PIZNO 注册证号,
            //                NVL(A.QCKC,0) 期初库存,NVL(A.QMKC,0) 期末库存,NVL(E.KCSL,0)  库存数量,
            //                (CASE WHEN NVL(A.QCKC,0)+ NVL(A.QMKC,0) = 0 THEN 0 ELSE ROUND(NVL(C.SYSL,0)/(NVL(A.QCKC,0)+NVL(A.QMKC,0))*2,2) END)  库存周转率_次,
            //                (CASE WHEN NVL(C.SYSL,0) = 0 THEN 0 ELSE ROUND(360*(A.QCKC+A.QMKC)/2/C.SYSL,0) END)  库存周转率_天
            //            FROM (SELECT A.DEPTID,A.GDSEQ,SUM(DECODE(TO_CHAR(A.RQ,'YYYY-MM-DD'),'{0}',A.KCSL,0)) QCKC ,SUM(DECODE(TO_CHAR(A.RQ,'YYYY-MM-DD'),'{0}',0,A.KCSL)) QMKC 
            //            FROM V_STOCKALL A
            //            WHERE TO_CHAR(A.RQ,'YYYY-MM-DD') = '{0}' OR TO_CHAR(A.RQ,'YYYY-MM-DD') = '{1}'
            //            GROUP BY A.DEPTID,A.GDSEQ) A,DOC_GOODS B,
            //            (SELECT A.GDSEQ,A.DEPTID,ABS(SUM(A.SL)) SYSL
            //            FROM DAT_GOODSJXC A
            //            WHERE A.KCADD = -1 AND A.BILLTYPE NOT IN('RKD','SYD') AND A.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd')
            //            GROUP BY A.GDSEQ,A.DEPTID) C,DOC_GOODSCFG D,
            //            (SELECT DEPTID,GDSEQ,SUM(KCSL) KCSL
            //            FROM DAT_GOODSSTOCK
            //            WHERE KCSL > 0
            //            GROUP BY DEPTID,GDSEQ) E
            //            WHERE D.GDSEQ = A.GDSEQ(+) AND D.GDSEQ = C.GDSEQ(+) AND D.GDSEQ = B.GDSEQ AND D.GDSEQ = E.GDSEQ(+) AND D.DEPTID = A.DEPTID(+) AND D.DEPTID = C.DEPTID(+) AND D.DEPTID = E.DEPTID(+)";
            string strSql = @"SELECT f_getdeptname(D.DEPTID) 库房,' '||B.GDSEQ 商品编码,B.GDNAME 商品名称,B.GDSPEC 规格容量,f_getunitname(B.UNIT) 单位,f_getproducername(B.PRODUCER) 生产厂家,B.HSJJ 价格,B.PIZNO 注册证号,
                            NVL(A.QCKC,0) 期初库存,NVL(A.QMKC,0) 期末库存,NVL(E.KCSL,0)  库存数量,
                            (CASE WHEN NVL(A.QCKC,0)+ NVL(A.QMKC,0) = 0 THEN 0 ELSE ROUND(NVL(C.SYSL,0)/(NVL(A.QCKC,0)+NVL(A.QMKC,0))*2,2) END)  库存周转率,
                            (CASE WHEN NVL(C.SYSL,0) = 0 THEN 0 ELSE ROUND(360*(A.QCKC+A.QMKC)/2/C.SYSL,0) END)  库存周转天数
                        FROM (SELECT A.DEPTID,A.GDSEQ,SUM(DECODE(TO_CHAR(A.RQ,'YYYY-MM-DD'),TO_CHAR(TO_DATE('{0}', 'yyyy-MM-dd')-1, 'YYYY-MM-DD'),A.KCSL,0)) QCKC ,SUM(DECODE(TO_CHAR(A.RQ,'YYYY-MM-DD'),TO_CHAR(TO_DATE('{1}', 'yyyy-MM-dd') - 1,'YYYY-MM-DD'),A.KCSL,0)) QMKC 
                        FROM V_STOCKALL A
                        WHERE A.RQ BETWEEN TO_DATE('{0}', 'yyyy-MM-dd')-1 and TO_DATE('{1}', 'yyyy-MM-dd')
                        GROUP BY A.DEPTID,A.GDSEQ) A,DOC_GOODS B,
                        (SELECT A.GDSEQ,A.DEPTID,ABS(SUM(A.SL)) SYSL
                        FROM DAT_GOODSJXC A
                        WHERE A.KCADD = -1 AND A.BILLTYPE NOT IN('RKD','SYD') AND A.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd')
                        GROUP BY A.GDSEQ,A.DEPTID) C,DOC_GOODSCFG D,
                        (SELECT DEPTID,GDSEQ,SUM(KCSL) KCSL
                        FROM DAT_GOODSSTOCK
                        WHERE KCSL > 0
                        GROUP BY DEPTID,GDSEQ) E
                        WHERE D.GDSEQ = A.GDSEQ(+) AND D.GDSEQ = C.GDSEQ(+) AND D.GDSEQ = B.GDSEQ AND D.GDSEQ = E.GDSEQ(+) AND D.DEPTID = A.DEPTID(+) AND D.DEPTID = C.DEPTID(+) AND D.DEPTID = E.DEPTID(+)";
            string strWhere = " ";

            if (!PubFunc.StrIsEmpty(ddlDEPTID.SelectedValue)) strWhere += " AND A.DEPTID = '" + ddlDEPTID.SelectedValue + "'";

            if (!PubFunc.StrIsEmpty(ddlCATID.SelectedValue)) strWhere += " AND B.CATID0 = '" + ddlCATID.SelectedValue + "'";

            if (!PubFunc.StrIsEmpty(ddlISGZ.SelectedValue)) strWhere += " and B.ISGZ = '" + ddlISGZ.SelectedValue + "'";

            if (!PubFunc.StrIsEmpty(tbxGOODS.Text)) strWhere += " AND (B.GDSEQ LIKE '%" + tbxGOODS.Text + "%' OR B.ZJM LIKE '%" + tbxGOODS.Text + "%' OR B.GDNAME LIKE '%" + tbxGOODS.Text + "%')";
            if (strWhere != " ") strSql = strSql + strWhere;

          //  strSql = string.Format(strSql, lstLRRQ1.Text, Convert.ToDateTime(lstLRRQ2.Text).AddDays(1).ToString("yyyy-MM-dd")) + string.Format(" ORDER BY {0} {1}", GridList.SortField, GridList.SortDirection);
            strSql = string.Format(strSql, lstLRRQ1.Text, Convert.ToDateTime(lstLRRQ2.Text).AddDays(1).ToString("yyyy-MM-dd")) + string.Format(" ORDER BY {0} {1}", "库存周转率", GridList.SortDirection);
            DataTable dtData = DbHelperOra.Query(strSql).Tables[0];
            if (dtData == null || dtData.Rows.Count == 0)
            {
                Alert.Show("没有数据,无法导出！");
                return;
            }
            XTBase.Utilities.ExcelHelper.ExportByWeb(dtData, "库存周转分析", "库存周转分析_" + DateTime.Now.ToString("yyyyMMddHH") + ".xls");
        }

        protected void GridList_Sort(object sender, GridSortEventArgs e)
        {
            GridList.SortDirection = e.SortDirection;
            GridList.SortField = e.SortField;
            DataSearch();
        }
    }
}