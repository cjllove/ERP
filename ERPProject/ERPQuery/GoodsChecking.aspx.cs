using XTBase;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FineUIPro;
using XTBase.Utilities;

namespace ERPProject.ERPApply
{
    public partial class GoodsChecking : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dpkout1.SelectedDate = DateTime.Now;
                dpkout2.SelectedDate = DateTime.Now;
            }
        }
        private void DataSearch()
        {
            int total = 0;
            string msg = "";
            NameValueCollection nvc = new NameValueCollection();

            DataTable dtData = GetGoodsList(GridGoods.PageIndex, GridGoods.PageSize, nvc, ref total, ref msg);
            GridGoods.RecordCount = total;
            GridGoods.DataSource = dtData;
            GridGoods.DataBind();
        }

        protected void btSearch_Click(object sender, EventArgs e)
        {
            DataSearch();
        }

        protected void GridGoods_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridGoods.PageIndex = e.NewPageIndex;
            DataSearch();
        }

        /// <summary>
        /// 获取商品数据信息
        /// </summary>
        /// <param name="pageNum">第几页</param>
        /// <param name="pageSize">每页显示天数</param>
        /// <param name="nvc">查询条件</param>
        /// <param name="total">总的条目数</param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public DataTable GetGoodsList(int pageNum, int pageSize, NameValueCollection nvc, ref int total, ref string errMsg)
        {
            string strSearch = "";
            if (tgbGDSEQ.Text.Trim().Length > 0) { strSearch += string.Format(" and (b.GDSEQ LIKE UPPER('%{0}%') OR b.GDNAME LIKE UPPER('%{0}%') OR b.ZJM  LIKE UPPER('%{0}%') OR B.HISCODE LIKE UPPER('%{0}%') OR B.HISNAME LIKE UPPER('%{0}%'))", tgbGDSEQ.Text.Trim()); }
            if (dpkout1.SelectedDate != null) { strSearch += string.Format(" AND a.SHRQ>=TO_DATE('{0}','YYYY-MM-DD')", dpkout1.Text); }
            if (dpkout2.SelectedDate != null) { strSearch += string.Format(" AND a.SHRQ<TO_DATE('{0}','YYYY-MM-DD')+1", dpkout2.Text); }
            if (!PubFunc.StrIsEmpty(ddlISGZ.SelectedValue)) strSearch += " and B.ISGZ = '" + ddlISGZ.SelectedValue + "'";
            strSearch += string.Format(" AND a.deptid in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '{0}') = 'Y') ORDER BY A.SHRQ DESC", UserAction.UserID);
            string strGoods = @"SELECT A.*,F_GETHISINFO(B.GDSEQ,'GDNAME') GDNAME,F_GETHISINFO(B.GDSEQ,'GDSPEC') GDSPEC,f_getunitname(A.unit) UNITNAME,f_getproducername(B.PRODUCER) PRODUCERNAME,f_getbillflag(A.FLAG) FLAGNAME,f_get_billtypename(A.BILLTYPE) BILLTYPENAME,F_GETUSERNAME(A.SHR) SHRNAME,F_GETDEPTNAME(DEPTOUT) DEPTOUTNAME,F_GETDEPTNAME(A.DEPTID) DEPTINNAME,decode(B.ISGZ,'Y','是','否') ISGZ
                    FROM VIEW_GOODSGZ A,DOC_GOODS B WHERE A.GDSEQ = B.GDSEQ ";
            StringBuilder strSql = new StringBuilder(strGoods);
            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql.Append(strSearch);
            }
            return GetDataTable(pageNum, pageSize, strSql, ref total);
        }
        protected void btnExp_Click(object sender, EventArgs e)
        {
            if (GridGoods.Rows.Count < 1)
            {
                Window1.Hidden = true;
                Alert.Show("没有数据,无法导出！");
                return;
            }
            //Response.ClearContent();
            //Response.AddHeader("content-disposition", "attachment; filename=单品单据追踪.xls");
            //Response.ContentType = "application/excel";
            //Response.Write(PubFunc.GridToHtml(GridGoods));
            //Response.End();
            //btnExp.Enabled = true;
            string strSearch = "";
            if (tgbGDSEQ.Text.Trim().Length > 0) { strSearch += string.Format(" and (b.GDSEQ LIKE UPPER('%{0}%') OR b.GDNAME LIKE UPPER('%{0}%') OR b.NAMEJC  LIKE UPPER('%{0}%') OR  b.BAR3 LIKE UPPER('%{0}%') OR b.ZJM  LIKE UPPER('%{0}%'))", tgbGDSEQ.Text.Trim()); }
            if (dpkout1.SelectedDate != null) { strSearch += string.Format(" AND a.SHRQ>=TO_DATE('{0}','YYYY-MM-DD')", dpkout1.Text); }
            if (dpkout2.SelectedDate != null) { strSearch += string.Format(" AND a.SHRQ<TO_DATE('{0}','YYYY-MM-DD')+1", dpkout2.Text); }
            if (!PubFunc.StrIsEmpty(ddlISGZ.SelectedValue)) strSearch += " and B.ISGZ = '" + ddlISGZ.SelectedValue + "'";
            strSearch += string.Format(" AND a.deptid in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '{0}') = 'Y') ORDER BY A.SHRQ DESC) a", UserAction.UserID);
            string strGoods = @"SELECT rownum 序号,a.* from (select A.SHRQ 日期,A.SEQNO 单号编号,f_get_billtypename(A.BILLTYPE) 单号类型,f_getbillflag(A.FLAG) 单号状态,F_GETUSERNAME(A.SHR) 审核人,F_GETDEPTNAME(DEPTOUT) 出库使用库房,F_GETDEPTNAME(A.DEPTID) 使用退货科室,
                                    ' '||B.GDSEQ 商品编码,F_GETHISINFO(B.GDSEQ,'GDNAME') 商品名称,F_GETHISINFO(B.GDSEQ,'GDSPEC') 商品规格,f_getproducername(B.PRODUCER) 生产厂家,A.SL 数量,f_getunitname(A.unit) 单位,A.HSJJ 单价,A.HSJE 金额,decode(B.ISGZ,'Y','是','否') 是否高值
                    FROM VIEW_GOODSGZ A,DOC_GOODS B WHERE A.GDSEQ = B.GDSEQ ";
            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strGoods += strSearch;
            }
            DataTable dt = DbHelperOra.Query(strGoods).Tables[0];
            ExcelHelper.ExportByWeb(dt, string.Format("单品单据追踪"), "单品单据追踪导出_" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
        }

        protected void GridGoods_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            string billno = GridGoods.Rows[e.RowIndex].Values[2].ToString();
            string type = GridGoods.Rows[e.RowIndex].Values[3].ToString();
            //取得默认供应商
            Object obj = DbHelperOra.GetSingle("SELECT VALUE FROM SYS_PARA WHERE CODE = 'SUPPLIER'");
            string url = "";
            if (type == "CKD" || type == "DST" || type == "DSC" || type == "LCD" || type == "LTD")
            {
                url = "~/ERPPayment/Doc_CK_ComWindow.aspx?bm=" + billno + "&cx=dpdjzz&su=" + (obj ?? "").ToString();
                PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "单据信息:单号【" + billno + "】"));
            }
            if (type == "XSD" || type == "DSH" || type == "XSG" || type == "XST")
            {
                url = "~/ERPPayment/Doc_XS_ComWindow.aspx?bm=" + billno + "&cx=&su=" + (obj ?? "").ToString();
                PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "单据信息:单号【" + billno + "】"));
            }
            if (type == "RKD")
            {
                url = "~/ERPPayment/Doc_RK_ComWindow.aspx?bm=" + billno + "&cx=&su=" + (obj ?? "").ToString();
                PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "单据信息:单号【" + billno + "】"));
            }
            if (type == "THD")
            {
                url = "~/ERPPayment/Doc_TH_ComWindow.aspx?bm=" + billno + "&cx=&su=" + (obj ?? "").ToString();
                PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "单据信息:单号【" + billno + "】"));
            }
            if (type == "DHD")
            {
                url = "~/ERPPayment/Doc_DD_ComWindow.aspx?bm=" + billno + "&cx=&su=" + (obj ?? "").ToString();
                PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "单据信息:单号【" + billno + "】"));
            }
            if (type == "YKD")
            {
                url = "~/ERPPayment/Doc_SY_ComWindow.aspx?bm=" + billno + "&cx=&su=" + (obj ?? "").ToString();
                PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "单据信息:单号【" + billno + "】"));
            }
            if (type == "SYD")
            {
                url = "~/ERPPayment/Doc_SYD_ComWindow.aspx?bm=" + billno + "&cx=&su=" + (obj ?? "").ToString();
                PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "单据信息:单号【" + billno + "】"));
            }
            if (type == "KSD")
            {
                url = "~/ERPPayment/Doc_KS_ComWindow.aspx?bm=" + billno + "&cx=&su=" + (obj ?? "").ToString();
                PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "单据信息:单号【" + billno + "】"));
            }
        }
        public DataTable GetGoods(int pageNum, int pageSize, NameValueCollection nvc, ref int total, ref string errMsg)
        {
            string strSearch = "";
            if (!PubFunc.StrIsEmpty(tgbGDSEQ.Text)) { strSearch += string.Format(" and (A.ZJM  LIKE '%{0}%' OR A.GDSEQ LIKE '%{0}%' OR A.GDNAME LIKE '%{0}%')", tgbGDSEQ.Text.Trim().ToUpper()); }
            //strSearch += " ORDER BY A.GDNAME";
            string Sql = @"SELECT A.*,f_getunitname(A.UNIT) UNITNAME,f_getproducername(A.PRODUCER) PRODUCERNAME FROM DOC_GOODS A WHERE A.FLAG = 'Y'";
            StringBuilder strSql = new StringBuilder(Sql);
            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql.Append(strSearch);
            }
            return GetDataTable(pageNum, pageSize, strSql, ref total);
        }
    }
}