﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FineUIPro;
using Oracle.ManagedDataAccess.Client;
using XTBase;

namespace ERPProject.ERPPayment
{
    public partial class CheckComputation : PageBase
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
            this.dpkout1.SelectedDate = DateTime.Now.AddMonths(-1);
            this.dpkout2.SelectedDate = DateTime.Now;
            DepartmentBind.BindDDL("DDL_SYS_DEPTRANGE", UserAction.UserID, ddlDEPTID);
            PubFunc.DdlDataGet("DDL_USER", lblLRY);
        }

        private void DataSearch()
        {
            string str = "";
            if (this.ddlDEPTID.SelectedValue.Length > 0)
            {
                str = " AND A.DEPTID = '" + this.ddlDEPTID.SelectedValue + "'";
            }
            if (this.tgbSEQNO.Text.Trim().Length > 0)
            {
                str = " AND A.SEQNO LIKE '%" + this.tgbSEQNO.Text.Trim() + "%'";
            }
            if (this.lblLRY.SelectedValue.Length > 0)
            {
                str = " AND A.SHRNAME = '" + this.lblLRY.SelectedText + "'";
            }
            if (this.ddlFLAG.SelectedValue.Length > 0)
            {
                str = str + " AND NVL(A.ISCHECK,'N') = '" + this.ddlFLAG.SelectedValue + "'";
            }

            int num = 0;
            this.highlightRows.Text = ",";
            string str2 = string.Format("SELECT *  FROM VIEW_BILL_COMPARE A   WHERE A.SHRQ BETWEEN TO_DATE('{0}', 'yyyy-MM-dd') AND TO_DATE('{1}', 'yyyy-MM-dd') +1" + str + " order by deptid,shrq desc", this.dpkout1.Text, this.dpkout2.Text);
            DataTable dtTotal = DbHelperOra.Query("SELECT SUM(SUBSUM) TOTAL, SUM(DECODE(STR5NAME, '已核对', SUBSUM, 0)) SUBSUM FROM (" + str2 + ")").Tables[0];
            if (dtTotal != null && dtTotal.Rows.Count > 0)
            {
                lblTOTAL.Text = dtTotal.Rows[0]["TOTAL"].ToString();
                lblSUBSUM.Text = dtTotal.Rows[0]["SUBSUM"].ToString();
            }
            DataTable table = PubFunc.DbGetPage(this.GridGoods.PageIndex, this.GridGoods.PageSize, str2, ref num);
            this.GridGoods.RecordCount = num;
            this.GridGoods.DataSource = table;
            this.GridGoods.DataBind();

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

        protected void btnAudit_Click(object sender, EventArgs e)
        {
            string str = "";
            foreach (int num2 in this.GridGoods.SelectedRowIndexArray)
            {
                if ((this.GridGoods.DataKeys[num2][1] ?? "N").ToString() == "N")
                {
                    object[] objArray1 = new object[] { str, "'", this.GridGoods.DataKeys[num2][0], "'," };
                    str = string.Concat(objArray1);
                }
            }
            if (str.Length > 0)
            {
                char[] trimChars = new char[] { ',' };
                DbHelperOra.ExecuteSql("UPDATE DAT_XS_DOC SET STR7 = 'Y',STR8='" + UserAction.UserID + "' WHERE NVL(STR7,'N') = 'N' AND SEQNO IN(" + str.TrimEnd(trimChars) + ")");
                char[] chArray2 = new char[] { ',' };
                DbHelperOra.ExecuteSql("UPDATE DAT_CK_DOC SET STR5 = 'Y',NUM4='" + UserAction.UserID + "' WHERE NVL(STR5,'N') = 'N' AND SEQNO IN(" + str.TrimEnd(chArray2) + ")");
                char[] chArray3 = new char[] { ',' };
                DbHelperOra.ExecuteSql("UPDATE DAT_JZ_DOC SET STR3 = 'Y',STR4='" + UserAction.UserID + "' WHERE NVL(STR3,'N') = 'N' AND SEQNO IN(" + str.TrimEnd(chArray3) + ")");
                DataSearch();
            }
            else
            {
                Alert.Show("请选择需要核对的单据！", "提示信息", MessageBoxIcon.Warning);
            }
        }

        protected void GridGoods_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            string str = this.GridGoods.Rows[e.RowIndex].Values[2].ToString();
            string str2 = this.GridGoods.Rows[e.RowIndex].Values[5].ToString();
            string iframeUrl = "";

            if (str2 == "LCD" || str2 == "CKD" || str2 == "DSC" || str2 == "DST")
            {
                iframeUrl = "~/ERPPayment/Doc_CK_ComWindow.aspx?bm=" + str + "&cx=&su=";
                string[] values = new string[] { this.hfdValue.ClientID };
                PageContext.RegisterStartupScript(this.Window1.GetSaveStateReference(values) + this.Window1.GetShowReference(iframeUrl, "单据信息:单号【" + str + "】"));
            }
            if (str2 == "XSD" || str2 == "DSH" || str2 == "XSG")
            {
                iframeUrl = "~/ERPPayment/Doc_XS_ComWindow.aspx?bm=" + str + "&cx=&su=";
                PageContext.RegisterStartupScript(this.Window1.GetSaveStateReference(hfdValue.Text) + this.Window1.GetShowReference(iframeUrl, "单据编号：【" + str + "】"));
            }
            if (str2 == "RKD")
            {
                iframeUrl = "~/ERPPayment/Doc_RK_ComWindow.aspx?bm=" + str + "&cx=&su=";
                PageContext.RegisterStartupScript(this.Window1.GetSaveStateReference(hfdValue.Text) + this.Window1.GetShowReference(iframeUrl, "单据信息:单号【" + str + "】"));
            }
            if (str2 == "THD")
            {
                iframeUrl = "~/ERPPayment/Doc_TH_ComWindow.aspx?bm=" + str + "&cx=&su=";
                PageContext.RegisterStartupScript(this.Window1.GetSaveStateReference(hfdValue.Text) + this.Window1.GetShowReference(iframeUrl, "单据信息:单号【" + str + "】"));
            }
            if (str2 == "DHD")
            {
                iframeUrl = "~/ERPPayment/Doc_DD_ComWindow.aspx?bm=" + str + "&cx=&su=";
                PageContext.RegisterStartupScript(this.Window1.GetSaveStateReference(hfdValue.Text) + this.Window1.GetShowReference(iframeUrl, "单据信息:单号【" + str + "】"));
            }
            if (str2 == "CKH" || str2 == "JSD")
            {
                iframeUrl = "~/ERPPayment/Doc_CX_ComWindow.aspx?bm=" + str + "&cx=&su=";
                PageContext.RegisterStartupScript(this.Window1.GetSaveStateReference(hfdValue.Text) + this.Window1.GetShowReference(iframeUrl, "单据编号：【" + str + "】"));
            }
        }

        protected void GridGoods_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView dataItem = e.DataItem as DataRowView;
            if ((dataItem != null) && (dataItem["STR5NAME"].ToString() == "已核对"))
            {
                highlightRows.Text = highlightRows.Text + e.RowIndex.ToString() + ",";
            }
            string STR5NAME = dataItem["STR5NAME"].ToString();
            if (STR5NAME == "未核对")
            {
                e.RowAttributes["data-color"] = "color1";
            }
           
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            //打印
            //int[] indexs = GridGoods.SelectedRowIndexArray;
            //if (indexs.Count() < 1)
            //{
            //    Alert.Show("请选择需要打印的单据信息！", "提示信息", MessageBoxIcon.Warning);
            //    return;
            //}
            //String str = "";
            //foreach (int index in indexs)
            //{
            //    str += "'" + GridGoods.DataKeys[index][0].ToString() + "',";
            //}
            //hfdBillno.Text = str.TrimEnd(',');
            string str = "";
            if (this.ddlDEPTID.SelectedValue.Length > 0)
            {
                str = " AND A.DEPTID = '" + this.ddlDEPTID.SelectedValue + "'";
            }
            if (this.tgbSEQNO.Text.Trim().Length > 0)
            {
                str = " AND A.SEQNO LIKE '%" + this.tgbSEQNO.Text.Trim() + "%'";
            }
            if (this.lblLRY.SelectedValue.Length > 0)
            {
                str = " AND A.SHRNAME = '" + this.lblLRY.SelectedText + "'";
            }
            if (this.ddlFLAG.SelectedValue.Length > 0)
            {
                str = str + " AND NVL(A.STR5,'N') = '" + this.ddlFLAG.SelectedValue + "'";
            }
            String Sql = string.Format("INSERT INTO TEMP_DY SELECT SEQNO FROM VIEW_BILL_COMPARE A   WHERE A.SHRQ BETWEEN TO_DATE('{0}', 'yyyy-MM-dd') AND TO_DATE('{1}', 'yyyy-MM-dd') +1" + str + " order by deptid,shrq desc", this.dpkout1.Text, this.dpkout2.Text);
            DbHelperOra.ExecuteSql("delete from TEMP_DY");
            DbHelperOra.ExecuteSql(Sql);
            if (!DbHelperOra.Exists("select 1 from TEMP_DY"))
            {
                Alert.Show("请选择需要打印的单据信息！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            PageContext.RegisterStartupScript("btnPrint_onclick()");
        }
    }
}