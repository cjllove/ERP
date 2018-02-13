﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FineUIPro;
using System.Data;
using XTBase;

namespace ERPProject.ERPStorage
{
    public partial class SupEvaluation : BillBase
    {
        public String html = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
                DataTable temp2 = DbHelperOra.Query("SELECT CODE,NAME,MEMO SCORE FROM SYS_CODEVALUE WHERE TYPE='GHQKPJ' order by CODE ASC").Tables[0];
                if (temp2 == null || temp2.Rows.Count < 1)
                {
                    temp2 = new DataTable();
                    temp2.Columns.Add("CODE");
                    temp2.Columns.Add("NAME");
                    temp2.Columns.Add("SCORE");
                    DataRow dr2 = temp2.NewRow();
                    dr2["CODE"] = "1";
                    dr2["NAME"] = "24小时以内";
                    dr2["SCORE"] = "100";
                    temp2.Rows.Add(dr2);
                    dr2 = temp2.NewRow();
                    dr2["CODE"] = "2";
                    dr2["NAME"] = "24-72小时";
                    dr2["SCORE"] = "80";
                    temp2.Rows.Add(dr2);
                    dr2 = temp2.NewRow();
                    dr2["CODE"] = "3";
                    dr2["NAME"] = "72-120小时";
                    dr2["SCORE"] = "50";
                    temp2.Rows.Add(dr2);
                    dr2 = temp2.NewRow();
                    dr2["CODE"] = "4";
                    dr2["NAME"] = "120小时以上";
                    dr2["SCORE"] = "30";
                    temp2.Rows.Add(dr2);
                }
                rblSTR5.DataTextField = "NAME";
                rblSTR5.DataValueField = "CODE";
                rblSTR5.DataSource = temp2;
                rblSTR5.DataBind();
                if (Request.QueryString["billno"] != null && Request.QueryString["PSSID"] != null && Request.QueryString["PSSNAME"] != null)
                {
                    load(Request.QueryString["billno"].ToString(), Request.QueryString["PSSID"].ToString());
                    tleSUPNAME.Text = Request.QueryString["PSSNAME"].ToString();
                    tleBILLNO.Text = Request.QueryString["billno"].ToString();
                    hdfPSSID.Text = Request.QueryString["PSSID"].ToString();
                }
                else
                {
                    load("", "");
                }
            }
        }
        protected void load(String BillNo,String PSSID)
        {
            #region 摘抄自科室评价  生成星状评级 阿磊 2016年7月20日 14:54:47
            int k = 1;
            int i = 0;
            string memd = "this,'" + k + "'," + (5 * k) + "," + (5 * i + 1) + "";
            html += "<div id=\"rateMe\">"
                  + "<span id = \"Project" + k + "\" style=\"float: left;width: 40px; font-size: 16px;\">" + "等级" + "</span>"
                  + "<a onclick = \"rateIt(" + memd + ")\" id = \"_" + (1 + i * 5) + "\" title = \"较差\" onmousedown = \"down(" + memd + ")\" onmouseover = \"rating(" + memd + ")\" onmouseout = \"off(" + memd + ")\" ></a>"
                  + "<a onclick = \"rateIt(" + memd + ")\" id = \"_" + (2 + i * 5) + "\" title = \"一般\" onmousedown = \"down(" + memd + ")\" onmouseover = \"rating(" + memd + ")\" onmouseout = \"off(" + memd + ")\" ></a>"
                  + "<a onclick = \"rateIt(" + memd + ")\" id = \"_" + (3 + i * 5) + "\" title = \"良好\" onmousedown = \"down(" + memd + ")\" onmouseover = \"rating(" + memd + ")\" onmouseout = \"off(" + memd + ")\" ></a>"
                  + "<a onclick = \"rateIt(" + memd + ")\" id = \"_" + (4 + i * 5) + "\" title = \"极好\" onmousedown = \"down(" + memd + ")\" onmouseover = \"rating(" + memd + ")\" onmouseout = \"off(" + memd + ")\" ></a>"
                  + "<a onclick = \"rateIt(" + memd + ")\" id = \"_" + (5 + i * 5) + "\" title = \"完美\" onmousedown = \"down(" + memd + ")\" onmouseover = \"rating(" + memd + ")\" onmouseout = \"off(" + memd + ")\" ></a>"
                  + "<span id = \"rateStatus" + k + "\" style=\"float: left;width: 60px; font-size: 16px;\" > 评分...</span><span id = \"rateNow" + k + "\" style=\"color: #F2F5F7; \" >0</span>"
                  + "</div> ";
            hdfcount.Text = "1";
            #endregion
            if (BillNo != "" && PSSID != "")
            {
                billopen(BillNo, PSSID);
            }
        }
        protected void billopen(string Billno,string pssid)
        {
            DataTable dtDoc = DbHelperOra.Query("select A.*,f_getsuppliername(A.SUPID) SUPNAME,f_getusername(A.PJR) PJRNAME from DAT_SUPEVALUATE A WHERE A.SUPID='" + pssid + "' AND A.BILLNO='" + Billno + "'").Tables[0];
            if (dtDoc != null && dtDoc.Rows.Count > 0)
            {
                
                int i = 0;
                int k = i + 1;
                PageContext.RegisterStartupScript("load(" + dtDoc.Rows[0]["GRADE"].ToString() + "," + k + "," + 5 * k + "," + (5 * i + 1) + ",'Y');");
                tleSUPNAME.Text = dtDoc.Rows[0]["SUPNAME"].ToString();
                hdfPSSID.Text = dtDoc.Rows[0]["SUPID"].ToString();
                tleBILLNO.Text = dtDoc.Rows[0]["BILLNO"].ToString();
                rblNUM1.SelectedIndex = Convert.ToInt32(dtDoc.Rows[0]["NUM1"].ToString());
                rblNUM2.SelectedIndex = Convert.ToInt32(dtDoc.Rows[0]["NUM2"].ToString());
                rblNUM3.SelectedIndex = Convert.ToInt32(dtDoc.Rows[0]["NUM3"].ToString());
                txaMEMO.Text = dtDoc.Rows[0]["MEMO"].ToString();
                rblSTR5.SelectedValue = dtDoc.Rows[0]["STR5"].ToString();
                rblNUM1.Enabled = false;
                rblNUM2.Enabled = false;
                rblNUM3.Enabled = false;
                txaMEMO.Enabled = false;
                btnSave.Enabled = false;
                rblSTR5.Enabled = false;
            }
            else
            {
                return;
            }
        }
        private void save(String value)
        {
            if (hdfPSSID.Text == null || hdfPSSID.Text == "" || tleSUPNAME.Text == null || tleSUPNAME.Text == "" || tleBILLNO.Text == null || tleBILLNO.Text == "") 
            { Alert.Show("请选定评价目标", MessageBoxIcon.Warning); return; }
            if (value == "0") { Alert.Show("请标注评价等级", MessageBoxIcon.Warning); return; }
            if (rblNUM1.SelectedIndex < 0) { Alert.Show("请标注包装外观检查结果", MessageBoxIcon.Warning); return; }
            if (rblNUM2.SelectedIndex < 0) { Alert.Show("请标注数量检查结果", MessageBoxIcon.Warning); return; }
            if (rblNUM3.SelectedIndex < 0) { Alert.Show("请标注评抽检结果", MessageBoxIcon.Warning); return; }
            if (rblSTR5.SelectedIndex < 0) { Alert.Show("请标注供货及时率", MessageBoxIcon.Warning); return; }
            if (txaMEMO.Text.Length>50)
            {
                 Alert.Show("详细说明内容超长", MessageBoxIcon.Warning); return;
            }
            if (DbHelperOra.ExecuteSql(string.Format("INSERT INTO DAT_SUPEVALUATE (SUPID,BILLNO,GRADE,PJR,MEMO,NUM1,NUM2,NUM3,STR5) VALUES ('{0}','{1}', '{2}','{3}','{4}','{5}','{6}','{7}','{8}')  ", hdfPSSID.Text.Trim(), tleBILLNO.Text.Trim(), value, UserAction.UserID, txaMEMO.Text, rblNUM1.SelectedIndex, rblNUM2.SelectedIndex, rblNUM3.SelectedIndex, rblSTR5.SelectedValue)) > 0)
            {
                
                Alert.Show("评价成功");
                billopen(tleBILLNO.Text.Trim(), hdfPSSID.Text.Trim());
                return;
            }
        }
        protected void PageManager1_CustomEvent(object sender, CustomEventArgs e)
        {
            if (e.EventArgument.IndexOf("ave") > 0)
            {
                save(e.EventArgument.Replace("Save", ""));
            }
        }
       
    }
}