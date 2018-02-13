﻿using XTBase;
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

namespace ERPProject.ERPEvalution
{
    public partial class Evaluation : BillBase
    {
        public String html = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DepartmentBind.BindDDL("DDL_SYS_DEPTRANGE", UserAction.UserID, lstDEPTID);
                tgbPJYF.Text = DateTime.Now.ToString("yyyy-MM");
            }
        }

        protected void bntSearch_Click(object sender, EventArgs e)
        {
            String Sql = @"SELECT A.*,f_getdeptname(A.DEPTID) DEPTIDNAME,f_getusername(PJR) PJRNAME,f_getusername(SHR) SHRNAME,DECODE(A.FLAG,'Y','已评价','未评价') FLAGNAME
                        FROM DAT_PJ_DOC A";
            if (tgbPJYF.Text.Length > 0) Sql += " WHERE A.PJYF = '" + tgbPJYF.Text + "'";
            if (lstDEPTID.SelectedValue.Length > 0) Sql += " AND A.DEPTID = '" + lstDEPTID.SelectedValue + "'";
            if (lstFLAG.SelectedValue.Length > 0) Sql += " AND A.FLAG = '" + lstFLAG.SelectedValue + "'";
            Sql += string.Format(" AND deptid in( select code FROM SYS_DEPT where type <>'1' and  F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);

            GridList.DataSource = DbHelperOra.Query(Sql).Tables[0];
            GridList.DataBind();
        }

        protected void GridList_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            billopen(GridList.DataKeys[e.RowIndex][0].ToString());
        }
        protected void billopen(string Billno)
        {
            Panel5.IFrameUrl = "../ERPEvaluation/EvaluationFm.aspx?billno=" + Billno;
            System.Threading.Thread.Sleep(3000);
            TabStrip1.ActiveTabIndex = 1;
        }

        protected override void billClear()
        {
            PubFunc.FormDataClear(Formlist);
            tgbPJYF.Text = DateTime.Now.ToString("yyyy-MM");
        }
        protected void PageManager1_CustomEvent(object sender, CustomEventArgs e)
        {
            if (e.EventArgument == "Save")
            {
                save();
            }
        }
        private void save()
        {
        }

        protected void btnAudit_Click(object sender, EventArgs e)
        {
        }
    }
}