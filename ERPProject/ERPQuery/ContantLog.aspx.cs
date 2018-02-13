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

namespace ERPProject.ERPQuery
{
    public partial class ContantLog : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
            }
        }

        private void DataInit()
        {
            PubFunc.DdlDataGet(ddlDEPTID, "DDL_SYS_DEPTDEF");
            PubFunc.DdlDataGet("DDL_TZLX", ddlTYPE);
            dpkbegin.SelectedDate = DateTime.Now.AddDays(-1);
            dpkend.SelectedDate = DateTime.Now;
        }

        private void DataSearch()
        {
            int total = 0;
            string msg = "";
            NameValueCollection nvc = new NameValueCollection();
            if (ddlDEPTID.SelectedValue.Length > 0) nvc.Add("DEPTID", ddlDEPTID.SelectedValue);
            if (tbxGDSEQ.Text.Length > 0) nvc.Add("SEQ", tbxGDSEQ.Text);
            if (tbxGDSEQ_OLD.Text.Length > 0) nvc.Add("SEQ_OLD", tbxGDSEQ_OLD.Text);
            if (ddlTYPE.SelectedValue.Length > 0) nvc.Add("TYPE", ddlTYPE.SelectedValue);
            if (tbxSEQNO.Text.Length > 0) nvc.Add("SEQNO", tbxSEQNO.Text);
            if (ddlSTR1.SelectedValue.Length > 0) nvc.Add("STR1", ddlSTR1.SelectedValue);

            DataTable dtData = GetGoodsList(GridGoods.PageIndex, GridGoods.PageSize, nvc, ref total, ref msg);
            GridGoods.RecordCount = total;
            GridGoods.DataSource = dtData;
            GridGoods.DataBind();
        }

        protected void btSearch_Click(object sender, EventArgs e)
        {
            btnExport.Enabled = true;
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
            if (nvc != null)
            {
                foreach (string key in nvc)
                {
                    string condition = nvc[key];
                    if (!string.IsNullOrEmpty(condition))
                    {
                        switch (key.ToUpper())
                        {
                            case "SEQ":
                                strSearch += string.Format(" AND (SP.GDSEQ LIKE '%{0}%' OR SP.HISNAME LIKE '%{0}%' OR SP.GDNAME LIKE '%{0}%' OR SP.BAR3 LIKE '%{0}%' OR SP.HISCODE LIKE '%{0}%' OR SP.ZJM LIKE '%{0}%' OR SP.STR4 LIKE '%{0}%')", condition);
                                break;
                            case "SEQ_OLD":
                                strSearch += string.Format(" AND (B.GDSEQ LIKE '%{0}%' OR B.HISNAME LIKE '%{0}%' OR B.GDNAME LIKE '%{0}%' OR B.BAR3 LIKE '%{0}%' OR B.HISCODE LIKE '%{0}%' OR B.ZJM LIKE '%{0}%' OR B.STR4 LIKE '%{0}%')", condition);
                                break;
                            case "DEPTID":
                                strSearch += string.Format(" AND A.DEPTID='{0}'", condition);
                                break;
                            case "TYPE":
                                strSearch += string.Format(" AND A.TYPE='{0}'", condition);
                                break;
                            case "SEQNO":
                                strSearch += string.Format(" AND A.SEQNO = '{0}'", condition);
                                break;
                            case "STR1":
                                strSearch += string.Format(" AND A.STR1 = '{0}'", condition);
                                break;
                        }
                    }
                }
            }
            string strGoods = @"SELECT A.*,f_getunitname(A.UNIT_OLD) UNITNAME_OLD,f_getunitname(A.UNIT) UNITNAME,f_getdeptname(A.DEPTID) DEPTIDNAME,
                            f_getproducername(A.PRODUCER_OLD) PRODUCER_OLDNAME,f_getproducername(A.PRODUCER) PRODUCERNAME,DECODE(A.TYPE,'1','品种切换','2','一品多码','2','定数调整','未定义') TYPENAME
                        FROM doc_ds_log A,DOC_GOODS SP,DOC_GOODS B WHERE A.GDSEQ = SP.GDSEQ AND A.GDSEQ_OLD = B.GDSEQ";
            StringBuilder strSql = new StringBuilder(strGoods);
            strSearch += string.Format(" AND INSTIME BETWEEN TO_DATE('{0}','YYYY-MM-DD') AND TO_DATE('{1}','YYYY-MM-DD')+1 ORDER BY A.INSTIME DESC", dpkbegin.Text, dpkend.Text);
            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql.Append(strSearch);
            }
            return GetDataTable(pageNum, pageSize, strSql, ref total);
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            if (GridGoods.Rows.Count < 1)
            {
                Alert.Show("没有数据,无法导出！");
                return;
            }
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=信息导出.xls");
            Response.ContentType = "application/excel";
            Response.Write(PubFunc.GridToHtml(GridGoods));
            Response.End();
            btnExport.Enabled = true;
        }
    }
}