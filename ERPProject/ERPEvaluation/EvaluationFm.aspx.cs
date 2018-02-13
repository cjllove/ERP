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
    public partial class EvaluationFm : BillBase
    {
        public String html = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PubFunc.DdlDataGet("DDL_USER", docPJR);
                DepartmentBind.BindDDL("DDL_SYS_DEPTRANGE", UserAction.UserID, docDEPTID);
                if (Request.QueryString["billno"] != null)
                {
                    load(Request.QueryString["billno"].ToString());
                }
                else
                {
                    load();
                }
            }
        }

        protected void load(String BillNo = "")
        {
            DataTable dt;
            if (BillNo.Length > 0)
            {
                dt = DbHelperOra.Query("SELECT * FROM DAT_PJ_COM WHERE SEQNO = '" + BillNo + "' ORDER BY ROWNO").Tables[0];
            }
            else
            {
                dt = DbHelperOra.Query("SELECT * FROM DOC_EVALUATION WHERE FLAG = 'Y' ORDER BY SEQNO").Tables[0];
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int k = i + 1;
                string memd = "this,'" + k + "'," + (5 * k) + "," + (5 * i + 1) + "";
                html += "<div id=\"rateMe\">"
                      + "<span id = \"Project" + k + "\" style=\"float: left;width: 120px; font-size: 16px;\">" + dt.Rows[i]["PRONAME"] + "</span>"
                      + "<a onclick = \"rateIt(" + memd + ")\" id = \"_" + (1 + i * 5) + "\" title = \"较差\" onmousedown = \"down(" + memd + ")\" onmouseover = \"rating(" + memd + ")\" onmouseout = \"off(" + memd + ")\" ></a>"
                      + "<a onclick = \"rateIt(" + memd + ")\" id = \"_" + (2 + i * 5) + "\" title = \"一般\" onmousedown = \"down(" + memd + ")\" onmouseover = \"rating(" + memd + ")\" onmouseout = \"off(" + memd + ")\" ></a>"
                      + "<a onclick = \"rateIt(" + memd + ")\" id = \"_" + (3 + i * 5) + "\" title = \"良好\" onmousedown = \"down(" + memd + ")\" onmouseover = \"rating(" + memd + ")\" onmouseout = \"off(" + memd + ")\" ></a>"
                      + "<a onclick = \"rateIt(" + memd + ")\" id = \"_" + (4 + i * 5) + "\" title = \"极好\" onmousedown = \"down(" + memd + ")\" onmouseover = \"rating(" + memd + ")\" onmouseout = \"off(" + memd + ")\" ></a>"
                      + "<a onclick = \"rateIt(" + memd + ")\" id = \"_" + (5 + i * 5) + "\" title = \"完美\" onmousedown = \"down(" + memd + ")\" onmouseover = \"rating(" + memd + ")\" onmouseout = \"off(" + memd + ")\" ></a>"
                      + "<span id = \"rateStatus" + k + "\" > 评分...</span><span id = \"rateNow" + k + "\" style=\"color: #F2F5F7; \" >0</span>"
                      + "</div> ";
            }
            hdfcount.Text = dt.Rows.Count.ToString();
            if (BillNo.Length > 0)
            {
                billopen(BillNo);
            }
        }

        protected void bntSearch_Click(object sender, EventArgs e)
        {
        }
        protected void billopen(string Billno)
        {
            //PageContext.Redirect("~/Evaluation.aspx?Billno=" + Billno);
            DataTable dtDoc = DbHelperOra.Query("SELECT * FROM DAT_PJ_DOC A WHERE SEQNO = '" + Billno + "'").Tables[0];
            PubFunc.FormDataSet(FormDoc, dtDoc.Rows[0]);
            DataTable dt = DbHelperOra.Query("SELECT * FROM DAT_PJ_COM WHERE SEQNO = '" + Billno + "' ORDER BY ROWNO").Tables[0];
            String Res = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int k = i + 1;
                Res = DbHelperOra.GetSingle("SELECT NVL(PROCON,0) FROM DAT_PJ_COM A WHERE SEQNO = '" + Billno + "' AND PRONAME = '" + dt.Rows[i]["PRONAME"] + "'").ToString();
                PageContext.RegisterStartupScript("load(" + Res + "," + k + "," + 5 * k + "," + (5 * i + 1) + ",'" + docFLAG.SelectedValue + "');");
            }
            if (docDEPTID.SelectedValue.Length > 0)
            {
                //
                DataTable dtsl = DbHelperOra.Query(String.Format(@"SELECT NVL(SUM(B.XSSL),0) SLSL,COUNT(DISTINCT B.GDSEQ) SLPG,NVL(SUM(B.HSJE),0) SLJE
                            FROM DAT_SL_DOC A,DAT_SL_COM B
                            WHERE A.SEQNO = B.SEQNO AND A.FLAG IN('Y','S') AND A.DEPTID = '{2}'
                            AND A.SHRQ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd')", dpkPJYF.Text + "-01", (Convert.ToDateTime(dpkPJYF.Text + "-01").AddMonths(1)).ToString("yyyy-MM-dd"), docDEPTID.SelectedValue)).Tables[0];
                DataTable dtsl2 = DbHelperOra.Query(String.Format(@"SELECT NVL(SUM(B.XSSL),0) SLSL,COUNT(DISTINCT B.GDSEQ) SLPG,NVL(SUM(B.HSJE),0) SLJE
                            FROM DAT_CK_DOC A,DAT_CK_COM B
                            WHERE A.SEQNO = B.SEQNO AND A.FLAG IN('Y','S') AND A.BILLTYPE IN('LCD','CKD') AND A.DEPTID = '{2}'
                            AND A.SHRQ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd')", dpkPJYF.Text + "-01", (Convert.ToDateTime(dpkPJYF.Text + "-01").AddMonths(1)).ToString("yyyy-MM-dd"), docDEPTID.SelectedValue)).Tables[0];
                DataTable dtck = DbHelperOra.Query(String.Format(@"SELECT NVL(SUM(B.XSSL),0) SLSL,COUNT(DISTINCT B.GDSEQ) SLPG,NVL(SUM(B.HSJE),0) SLJE
                            FROM DAT_CK_DOC A,DAT_CK_COM B
                            WHERE A.SEQNO = B.SEQNO AND A.FLAG IN('Y','S') AND A.BILLTYPE IN('LCD','CKD','DSC') AND A.DEPTID = '{2}'
                            AND A.SHRQ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd')", dpkPJYF.Text + "-01", (Convert.ToDateTime(dpkPJYF.Text + "-01").AddMonths(1)).ToString("yyyy-MM-dd"), docDEPTID.SelectedValue)).Tables[0];
                DataTable thsl = DbHelperOra.Query(String.Format(@"SELECT NVL(SUM(B.XSSL),0) SLSL,COUNT(DISTINCT B.GDSEQ) SLPG,NVL(SUM(B.HSJE),0) SLJE
                                    FROM DAT_XS_DOC A,DAT_XS_COM B
                                    WHERE A.SEQNO = B.SEQNO AND A.FLAG IN('Y','S') AND A.BILLTYPE IN('XST') AND A.DEPTID = '{2}'
                                    AND A.SHRQ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd')", dpkPJYF.Text + "-01", (Convert.ToDateTime(dpkPJYF.Text + "-01").AddMonths(1)).ToString("yyyy-MM-dd"), docDEPTID.SelectedValue)).Tables[0];
                DataTable dtqh = DbHelperOra.Query(String.Format(@"SELECT NVL(sum(qhsl),0) qhs,count(distinct gdseq) qhpg
                    FROM dat_nostock_list
                    where flag in('N','Y') and deptid = '{2}' and date_sl BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd')", dpkPJYF.Text + "-01", (Convert.ToDateTime(dpkPJYF.Text + "-01").AddMonths(1)).ToString("yyyy-MM-dd"), docDEPTID.SelectedValue)).Tables[0];
                Decimal sls = 0, slpg = 0, je = 0, cks = 0, ckpg = 0, ckje = 0, ths = 0, thpg = 0, thje = 0, qhs = 0, qhpg = 0;
                if (dtsl.Rows.Count > 0)
                {
                    sls = Convert.ToDecimal(dtsl.Rows[0]["SLSL"]);
                    slpg = Convert.ToDecimal(dtsl.Rows[0]["SLPG"]);
                    je = Convert.ToDecimal(dtsl.Rows[0]["SLJE"]);
                }
                if (dtsl2.Rows.Count > 0)
                {
                    sls += Convert.ToDecimal(dtsl2.Rows[0]["SLSL"]);
                    slpg += Convert.ToDecimal(dtsl2.Rows[0]["SLPG"]);
                    je += Convert.ToDecimal(dtsl2.Rows[0]["SLJE"]);
                }
                if (dtck.Rows.Count > 0)
                {
                    cks = Convert.ToDecimal(dtck.Rows[0]["SLSL"]);
                    ckpg = Convert.ToDecimal(dtck.Rows[0]["SLPG"]);
                    ckje = Convert.ToDecimal(dtck.Rows[0]["SLJE"]);
                }
                if (thsl.Rows.Count > 0)
                {
                    ths = Convert.ToDecimal(thsl.Rows[0]["SLSL"]);
                    thpg = Convert.ToDecimal(thsl.Rows[0]["SLPG"]);
                    thje = Convert.ToDecimal(thsl.Rows[0]["SLJE"]);
                }
                if (dtqh.Rows.Count > 0)
                {
                    qhs = Convert.ToDecimal(dtqh.Rows[0]["QHS"]);
                    qhpg = Convert.ToDecimal(dtqh.Rows[0]["QHPG"]);
                }
                tbx1.Text = sls.ToString();
                tbx2.Text = cks.ToString();
                if (sls > 0)
                    tbx3.Text = (cks / sls).ToString("0%");
                else
                    tbx3.Text = "0%";

                tbx4.Text = slpg.ToString();
                tbx5.Text = ckpg.ToString();
                if (ckpg > 0)
                    tbx6.Text = (ckpg / slpg).ToString("0%");
                else
                    tbx6.Text = "0%";

                tbx7.Text = ths.ToString();

                tbx8.Text = qhs.ToString();
                tbx9.Text = qhpg.ToString();
            }
            if (docFLAG.SelectedValue == "Y")
            {
                btnSave.Enabled = false;
                btnAudit.Enabled = false;
            }
            else
            {
                btnSave.Enabled = true;
                btnAudit.Enabled = true;
            }
            txaMEMO.Text = dtDoc.Rows[0]["MEMO"].ToString();
        }
        protected void PageManager1_CustomEvent(object sender, CustomEventArgs e)
        {
            if (e.EventArgument.IndexOf("ave") > 0)
            {
                save(e.EventArgument.Replace("Save", ""));
            }
        }
        private void save(String value)
        {
            if (tbxSEQNO.Text.Length < 1) return;
            if (txaMEMO.Text.Length > 45)
            {
                Alert.Show("备注信息超长！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            List<CommandInfo> cmdList = new List<CommandInfo>();
            for (int i = 0; i < value.Trim(',').Split(',').Count(); i++)
            {
                cmdList.Add(new CommandInfo("UPDATE DAT_PJ_COM SET PROCON = " + value.Trim(',').Split(',')[i].Split('k')[1] + " WHERE SEQNO = '" + tbxSEQNO.Text + "' AND PRONAME = '" + value.Trim(',').Split(',')[i].Split('k')[0] + "'", null));
            }
            cmdList.Add(new CommandInfo("UPDATE DAT_PJ_DOC A SET RES = (SELECT SUM(QZ*PROCON/5) FROM DAT_PJ_COM B WHERE B.SEQNO = A.SEQNO),MEMO = '" + txaMEMO.Text.Trim() + "',PJR = '" + UserAction.UserID + "',PJRQ = SYSDATE WHERE SEQNO = '" + tbxSEQNO.Text + "'", null));
            DbHelperOra.ExecuteSqlTran(cmdList);
            Alert.Show("保存成功！");
            billopen(tbxSEQNO.Text);
        }

        protected void btnAudit_Click(object sender, EventArgs e)
        {
            if (tbxSEQNO.Text.Length < 1)
            {
                Alert.Show("请选择需要提交的评价单！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            if (DbHelperOra.Exists("SELECT 1 FROM DAT_PJ_COM WHERE NVL(PROCON,0) = 0 AND SEQNO = '" + tbxSEQNO.Text + "'"))
            {
                Alert.Show("科室未评价完成，请检查！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            String Sql = "UPDATE DAT_PJ_DOC SET FLAG = 'Y',SHR = '" + UserAction.UserID + "',SHRQ = SYSDATE WHERE SEQNO = '" + tbxSEQNO.Text + "' AND FLAG = 'N'";
            DbHelperOra.ExecuteSql(Sql);
            Alert.Show("评价提交成功！");
            billopen(tbxSEQNO.Text);
        }
    }
}