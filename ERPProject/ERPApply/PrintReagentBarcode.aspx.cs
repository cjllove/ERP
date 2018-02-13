﻿using FineUIPro;
using XTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPProject.ERPApply
{
    public partial class PrintReagentBarcode : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //PubFunc.DdlDataGet(ddlDEPTINT, "DDL_SYS_DEPTNULL");
                DepartmentBind.BindDDL("DDL_SYS_DEPTRANGE", UserAction.UserID, ddlDEPTINT);
                dpkout1.SelectedDate = DateTime.Now.AddDays(-1);
                dpkout2.SelectedDate = DateTime.Now;
            }

        }

        protected void btSearch_Click(object sender, EventArgs e)
        {
            DataSearch();
        }

        private void DataSearch()
        {
            int total = 0;
            DataTable dtData = PubFunc.DbGetPage(GridGoods.PageIndex, GridGoods.PageSize, GetSearchSql(), ref total);
            GridGoods.RecordCount = total;
            GridGoods.DataSource = dtData;
            GridGoods.DataBind();
        }

        private string GetSearchSql()
        {
            string strSql = @" select a.rkseqno,
                                       B.GDSEQ,
                                       B.GDNAME,
                                       B.GDSPEC,
                                       f_getunitname(b.unit) UNITNAME,
                                       f_getsupname(b.SUPPLIER) producername,
                                       A.GDBARCODE BARCODE,
                                       A.DEPTID,
                                       f_getdeptname(A.DEPTID) DEPTIN,
                                       A.RKRQ INRQ,
                                       f_getusername(A.RKY) RKY,
                                       A.CKSEQNO OUTBILLNO,
                                       A.CKRQ OUTRQ,
                                       A.LYR SLY,A.NUM1,A.NUM2,DECODE(A.FLAG,'Y','已使用','N','未使用','R','部分使用','已作废') FLAGNAME,
                                       '1' SL,A.PH,A.YXQZ,A.RQ_SC
                                  from DAT_BARCODE_SJ A, DOC_GOODS B
                                 WHERE A.GDSEQ = B.GDSEQ ";
            string strWhere = " ";

            if (tbxGDSEQ.Text.Trim().Length > 0) { strWhere += " and ( a.GDSEQ like '%" + tbxGDSEQ.Text.Trim() + "%' or b.gdname like '%" + tbxGDSEQ.Text.Trim() + "%'  )"; }
            if (ddlDEPTINT.SelectedValue.Length > 0) { strWhere += " and a.DEPTID ='" + ddlDEPTINT.SelectedValue.ToString() + "'"; }
            if (tbxBILL.Text.Trim().Length > 0) { strWhere += " and a.rkseqno like '%" + tbxBILL.Text.Trim() + "%'"; }
            if (dpkout1.SelectedDate != null) { strWhere += string.Format(" AND A.RKRQ>=TO_DATE('{0}','YYYY-MM-DD')", dpkout1.Text); }
            if (dpkout2.SelectedDate != null) { strWhere += string.Format(" AND A.RKRQ<TO_DATE('{0}','YYYY-MM-DD')+1", dpkout2.Text); }
            if (ddlFLAG.SelectedValue.Length > 0) { strWhere += " and a.flag ='" + ddlFLAG.SelectedValue + "'"; }

            if (strWhere != " ") strSql = strSql + strWhere;

            return strSql;
        }
        protected void GridList_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridGoods.PageIndex = e.NewPageIndex;
            DataSearch();
        }

        protected void GridGoods_RowSelect(object sender, GridRowSelectEventArgs e)
        {
            int[] selections = GridGoods.SelectedRowIndexArray;
            string no = "";
            foreach (int rowIndex in selections)
            {
                no = no + "'" + GridGoods.DataKeys[rowIndex][0].ToString() + "',";
            }
            echo.Text = no.TrimEnd(',');
        }

        protected void GridGoods_RowCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "actZF")
            {
                string Sql = "UPDATE DAT_BARCODE_SJ SET FLAG ='F' WHERE GDBARCODE = '" + GridGoods.DataKeys[e.RowIndex][0] + "'";
                DbHelperOra.ExecuteSql(Sql);
                DataSearch();
            }
        }
    }
}