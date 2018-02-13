using FineUIPro;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using XTBase;
using XTBase.Utilities;

namespace ERPProject.ERPQuery
{
    public partial class Dx_Search : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 在页面第一次加载时
                BindDDL();
                hfdUSERID.Text = UserAction.UserID;
            }
        }
        private void BindDDL()
        {
            PubFunc.DdlDataGet("DDL_SYS_DEPTALLRANGE", UserAction.UserID, schDEPTID, ddlDEPTID);
            btnClear_Click(null, null);

        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            dpkTime1.SelectedDate = DateTime.Now.AddMonths(-1);
            dpkTime2.SelectedDate = DateTime.Now;
            dpkDATE1.SelectedDate = DateTime.Now.AddMonths(-1);
            dpkDATE2.SelectedDate = DateTime.Now;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (dpkTime1.SelectedDate == null || dpkTime2.SelectedDate == null)
            {
                Alert.Show("请输入条件【查询期间】！");
                return;
            }
            else if (dpkTime1.SelectedDate > dpkTime2.SelectedDate)
            {
                Alert.Show("开始日期大于结束日期，请重新输入！");
                return;
            }
            String sql = @"SELECT A.DEPTID,f_getdeptname(A.DEPTID) DEPTNAME,B.GDSEQ,B.GDNAME,B.GDSPEC,f_getunitname(B.UNIT) UNITNAME,B.HSJJ,f_getproducername(B.PRODUCER) PRODUCERNAME,
                           SUM(DECODE(A.KCADD,'1',DECODE(A.BILLTYPE,'XST',0,A.SL),0)) RKSL,SUM(DECODE(A.KCADD,'1',DECODE(A.BILLTYPE,'XST',0,A.HSJE),0)) RKJE,
                           SUM(DECODE(A.KCADD,'-1',DECODE(A.BILLTYPE,'XST',0，'LTD',0,'DST',0,'THD',0,A.SL),0)) CKSL,SUM(DECODE(A.KCADD,'-1',DECODE(A.BILLTYPE,'XST',0，'LTD',0,'DST',0,'THD',0,A.HSJE),0)) CKJE,
                           SUM(DECODE(A.KCADD,'-1',DECODE(A.BILLTYPE,'XST',A.SL，'LTD',A.SL,'DST',A.SL,'THD',A.SL,0),0)) THSL,SUM(DECODE(A.KCADD,'-1',DECODE(A.BILLTYPE,'XST',A.HSJE，'LTD',A.HSJE,'DST',A.HSJE,'THD',A.HSJE,0),0)) THJE,
                           NVL((SELECT SUM(KCSL) FROM DAT_GOODSSTOCK WHERE DEPTID = A.DEPTID AND GDSEQ = B.GDSEQ),0) KCSL,
                           NVL((SELECT SUM(KCSL) FROM DAT_GOODSSTOCK WHERE DEPTID = A.DEPTID AND GDSEQ = B.GDSEQ),0)*B.HSJJ KCJE
                    FROM DAT_GOODSJXC A,DOC_GOODS B
                    WHERE A.RQSJ BETWEEN TO_DATE('{0}','YYYY-MM-DD') AND TO_DATE('{1}','YYYY-MM-DD') + 1
                    AND A.GDSEQ = B.GDSEQ AND F_CHK_DATARANGE(A.DEPTID, '{2}') = 'Y'";
            String search = @" ";
            if (tbxGDSEQ.Text.Trim().Length > 0)
            {
                search += String.Format(" AND (B.GDSEQ LIKE '%{0}%' OR B.GDNAME  LIKE '%{0}%' OR B.ZJM  LIKE '%{0}%' OR B.BAR3 LIKE '%{0}%')", tbxGDSEQ.Text.Trim());
            }
            if (schDEPTID.SelectedValue.Length > 0)
            {
                search += String.Format(" AND A.DEPTID = '{0}'", schDEPTID.SelectedValue);
            }
            if (isGZ.SelectedValue.Length > 0)
            {
                search += String.Format(" AND B.ISGZ = '{0}'", isGZ.SelectedValue);
            }
            string sortField = gidGoods.SortField;
            string sortDirection = gidGoods.SortDirection;
            sql = String.Format(sql + search + " GROUP BY A.DEPTID,B.GDSEQ,B.GDNAME,B.GDSPEC,B.UNIT,B.HSJJ,B.PRODUCER" + String.Format(" ORDER BY {0} {1}", sortField, sortDirection), dpkTime1.Text, dpkTime2.Text, UserAction.UserID);
            int total = 0;
            DataTable dtData = PubFunc.DbGetPage(gidGoods.PageIndex, gidGoods.PageSize, sql, ref total);
            gidGoods.RecordCount = total;
            gidGoods.DataSource = dtData;
            gidGoods.DataBind();

            Decimal RKSL = 0, RKJE = 0, CKSL = 0, CKJE = 0, THSL = 0, THJE = 0, KCSL = 0, KCJE = 0;
            foreach (DataRow row in dtData.Rows)
            {
                RKSL += Convert.ToDecimal(row["RKSL"]);
                RKJE += Convert.ToDecimal(row["RKJE"]);
                CKSL += Convert.ToDecimal(row["CKSL"]);
                CKJE += Convert.ToDecimal(row["CKJE"]);
                THSL += Convert.ToDecimal(row["THSL"]);
                THJE += Convert.ToDecimal(row["THJE"]);
                KCSL += Convert.ToDecimal(row["KCSL"]);
                KCJE += Convert.ToDecimal(row["KCJE"]);
            }
            JObject summary = new JObject();
            summary.Add("GDSPEC", "本页合计");
            summary.Add("RKSL", RKSL.ToString("F2"));
            summary.Add("RKJE", RKJE.ToString("F2"));
            summary.Add("CKSL", CKSL.ToString("F2"));
            summary.Add("CKJE", CKJE.ToString("F2"));
            summary.Add("THSL", THSL.ToString("F2"));
            summary.Add("THJE", THJE.ToString("F2"));
            summary.Add("KCSL", KCSL.ToString("F2"));
            summary.Add("KCJE", KCJE.ToString("F2"));
            gidGoods.SummaryData = summary;
        }
        protected void btnExpect_Click(object sender, EventArgs e)
        {
            String sql = @"SELECT f_getdeptname(A.DEPTID) 部门名称,B.GDSEQ 商品编码,B.GDNAME 商品名称,B.GDSPEC 商品规格,f_getunitname(B.UNIT) 单位,B.HSJJ 价格,f_getproducername(B.PRODUCER) 生产厂家,
                           SUM(DECODE(A.KCADD,'1',A.SL,0)) 入库数量,SUM(DECODE(A.KCADD,'1',A.HSJE,0)) 入库金额,
                           SUM(DECODE(A.KCADD,'-1',DECODE(A.BILLTYPE,'XST',0，'LTD',0,'DST',0,'THD',0,A.SL),0)) 出库数量,SUM(DECODE(A.KCADD,'-1',DECODE(A.BILLTYPE,'XST',0，'LTD',0,'DST',0,'THD',0,A.HSJE),0)) 出库金额,
                           SUM(DECODE(A.KCADD,'-1',DECODE(A.BILLTYPE,'XST',A.SL，'LTD',A.SL,'DST',A.SL,'THD',A.SL,0),0)) 退货数量,SUM(DECODE(A.KCADD,'-1',DECODE(A.BILLTYPE,'XST',A.HSJE，'LTD',A.HSJE,'DST',A.HSJE,'THD',A.HSJE,0),0)) 退货金额,
                           NVL((SELECT SUM(KCSL) FROM DAT_GOODSSTOCK WHERE DEPTID = A.DEPTID AND GDSEQ = B.GDSEQ),0) 库存数量,
                           NVL((SELECT SUM(KCSL) FROM DAT_GOODSSTOCK WHERE DEPTID = A.DEPTID AND GDSEQ = B.GDSEQ),0)*B.HSJJ 库存金额
                    FROM DAT_GOODSJXC A,DOC_GOODS B
                    WHERE A.RQSJ BETWEEN TO_DATE('{0}','YYYY-MM-DD') AND TO_DATE('{1}','YYYY-MM-DD') + 1
                    AND A.GDSEQ = B.GDSEQ AND F_CHK_DATARANGE(A.DEPTID, '{2}') = 'Y'";
            String search = @" ";
            if (tbxGDSEQ.Text.Trim().Length > 0)
            {
                search += String.Format(" AND (B.GDSEQ LIKE '%{0}%' OR B.GDNAME  LIKE '%{0}%' OR B.ZJM  LIKE '%{0}%' OR B.BAR3 LIKE '%{0}%')", tbxGDSEQ.Text.Trim());
            }
            if (schDEPTID.SelectedValue.Length > 0)
            {
                search += String.Format(" AND A.DEPTID = '{0}'", schDEPTID.SelectedValue);
            }
            if (isGZ.SelectedValue.Length > 0)
            {
                search += String.Format(" AND B.ISGZ = '{0}'", isGZ.SelectedValue);
            }
            sql = String.Format(sql + search + " GROUP BY A.DEPTID,B.GDSEQ,B.GDNAME,B.GDSPEC,B.UNIT,B.HSJJ,B.PRODUCER", dpkTime1.Text, dpkTime2.Text, UserAction.UserID);
            DataTable dtData = DbHelperOra.Query(sql).Tables[0];
            ExcelHelper.ExportByWeb(dtData, "商品动销汇总", string.Format("商品动销汇总_{0}.xls", DateTime.Now.ToString("yyyyMMdd")));
        }
        protected void gidGoods_PageIndexChange(object sender, GridPageEventArgs e)
        {
            gidGoods.PageIndex = e.NewPageIndex;
            btnSearch_Click(null, null);
        }

        protected void gidGoods_Sort(object sender, GridSortEventArgs e)
        {
            gidGoods.SortDirection = e.SortDirection;
            gidGoods.SortField = e.SortField;
            btnSearch_Click(null, null);
        }

        protected void gidGoods_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            txbGDSEQ.Text = gidGoods.DataKeys[e.RowIndex][0].ToString();
            ddlDEPTID.SelectedValue = gidGoods.DataKeys[e.RowIndex][1].ToString();
            dpkDATE1.Text = dpkTime1.Text;
            dpkDATE2.Text = dpkTime2.Text;
            ddlGZ.SelectedValue = isGZ.SelectedValue;
            TabStrip1.ActiveTabIndex = 0;
            btSearch_Click(null, null);
        }

        protected void btSearch_Click(object sender, EventArgs e)
        {
            if (dpkDATE1.SelectedDate == null || dpkDATE2.SelectedDate == null)
            {
                Alert.Show("请输入条件【查询期间】！");
                return;
            }
            else if (dpkDATE1.SelectedDate > dpkDATE2.SelectedDate)
            {
                Alert.Show("开始日期大于结束日期，请重新输入！");
                return;
            }

            String sql = @"SELECT A.RQSJ,f_getdeptname(A.DEPTID) DEPTNAME,B.GDSEQ,B.GDNAME,B.GDSPEC,f_getunitname(B.UNIT) UNITNAME,B.HSJJ,f_getproducername(B.PRODUCER) PRODUCERNAME,
                            A.SL,A.HSJE,A.PH,A.RQ_SC,A.YXQZ
                    FROM DAT_GOODSJXC A,DOC_GOODS B
                    WHERE A.RQSJ BETWEEN TO_DATE('{0}','YYYY-MM-DD') AND TO_DATE('{1}','YYYY-MM-DD')  + 1
                    AND A.GDSEQ = B.GDSEQ AND F_CHK_DATARANGE(A.DEPTID, '{2}') = 'Y'";
            String search = @" ";
            if (txbGDSEQ.Text.Trim().Length > 0)
            {
                search += String.Format(" AND (B.GDSEQ LIKE '%{0}%' OR B.GDNAME  LIKE '%{0}%' OR B.ZJM  LIKE '%{0}%' OR B.BAR3 LIKE '%{0}%')", txbGDSEQ.Text.Trim());
            }
            if (ddlDEPTID.SelectedValue.Length > 0)
            {
                search += String.Format(" AND A.DEPTID = '{0}'", ddlDEPTID.SelectedValue);
            }
            if (ddlGZ.SelectedValue.Length > 0)
            {
                search += String.Format(" AND B.ISGZ = '{0}'", ddlGZ.SelectedValue);
            }
            string sortField = grdList.SortField;
            string sortDirection = grdList.SortDirection;
            sql = String.Format(sql + search + String.Format(" ORDER BY {0} {1}", sortField, sortDirection), dpkDATE1.Text, dpkDATE2.Text, UserAction.UserID);
            int total = 0;
            DataTable dtData = PubFunc.DbGetPage(grdList.PageIndex, grdList.PageSize, sql, ref total);
            grdList.RecordCount = total;
            grdList.DataSource = dtData;
            grdList.DataBind();

            Double donateTotal = 0, feeTotal = 0;
            foreach (DataRow row in dtData.Rows)
            {
                donateTotal += Convert.ToInt32(row["SL"]);
                feeTotal += Convert.ToInt32(row["HSJE"]);
            }
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("SL", donateTotal.ToString("F2"));
            summary.Add("HSJE", feeTotal.ToString("F2"));
            grdList.SummaryData = summary;
        }

        protected void btnExp_Click(object sender, EventArgs e)
        {
            String sql = @"SELECT A.RQSJ 单据日期,f_getdeptname(A.DEPTID) 科室,B.GDSEQ 商品编码,B.GDNAME 商品名称,B.GDSPEC 商品规格,f_getunitname(B.UNIT) 单位,B.HSJJ 价格,f_getproducername(B.PRODUCER) 生产厂家,
                            A.SL 数量,A.HSJE 金额,A.PH 批号,A.RQ_SC 生产日期,A.YXQZ 有效期至
                    FROM DAT_GOODSJXC A,DOC_GOODS B
                    WHERE A.RQSJ BETWEEN TO_DATE('{0}','YYYY-MM-DD') AND TO_DATE('{1}','YYYY-MM-DD')  + 1
                    AND A.GDSEQ = B.GDSEQ AND F_CHK_DATARANGE(A.DEPTID, '{2}') = 'Y'";
            String search = @" ";
            if (txbGDSEQ.Text.Trim().Length > 0)
            {
                search += String.Format(" AND (B.GDSEQ LIKE '%{0}%' OR B.GDNAME  LIKE '%{0}%' OR B.ZJM  LIKE '%{0}%' OR B.BAR3 LIKE '%{0}%')", txbGDSEQ.Text.Trim());
            }
            if (ddlDEPTID.SelectedValue.Length > 0)
            {
                search += String.Format(" AND A.DEPTID = '{0}'", ddlDEPTID.SelectedValue);
            }
            if (ddlGZ.SelectedValue.Length > 0)
            {
                search += String.Format(" AND B.ISGZ = '{0}'", ddlGZ.SelectedValue);
            }
            string sortField = grdList.SortField;
            string sortDirection = grdList.SortDirection;
            sql = String.Format(sql + search, dpkDATE1.Text, dpkDATE2.Text, UserAction.UserID);
            DataTable dtData = DbHelperOra.Query(sql).Tables[0];
            ExcelHelper.ExportByWeb(dtData, "商品动销明细", string.Format("商品动销明细_{0}.xls", DateTime.Now.ToString("yyyyMMdd")));
        }
    }
}