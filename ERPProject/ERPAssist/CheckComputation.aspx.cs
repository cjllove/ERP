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

namespace ERPProject.ERPAssist
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
            this.dpkhs1.SelectedDate = DateTime.Now.AddMonths(-1);
            this.dpkhs2.SelectedDate = DateTime.Now;
            DepartmentBind.BindDDL("DDL_SYS_DEPTRANGE", UserAction.UserID, ddlDEPTID);
            PubFunc.DdlDataGet("DDL_USER", ddlHSY);
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
            if (ddlHSY.SelectedValue.Length > 0)
            {
                str = " AND A.HSR = '" + ddlHSY.SelectedValue + "'";
            }
            if (this.ddlFLAG.SelectedValue.Length > 0)
            {
                str = str + " AND NVL(A.FLAG,'N') = '" + this.ddlFLAG.SelectedValue + "'";
            }
            string str2 = "";
            int num = 0;
            this.highlightRows.Text = ",";
            if (this.ddlFLAG.SelectedValue == "Y")
            {
                str2 = string.Format("SELECT *  FROM VIEW_BILL_COMPARE A   WHERE A.SHRQ BETWEEN TO_DATE('{0}', 'yyyy-MM-dd') AND TO_DATE('{1}', 'yyyy-MM-dd') +1 AND A.HSRQ BETWEEN TO_DATE('{2}', 'yyyy-MM-dd') AND TO_DATE('{3}', 'yyyy-MM-dd') +1 " + str, this.dpkout1.Text, this.dpkout2.Text, this.dpkhs1.Text, this.dpkhs2.Text);
            }
            else
            {
                str2 = string.Format("SELECT *  FROM VIEW_BILL_COMPARE A   WHERE A.SHRQ BETWEEN TO_DATE('{0}', 'yyyy-MM-dd') AND TO_DATE('{1}', 'yyyy-MM-dd') +1 " + str, this.dpkout1.Text, this.dpkout2.Text);
            }
            str2 += " ORDER BY " + GridGoods.SortField + " " + GridGoods.SortDirection;
            DataTable dtTotal = DbHelperOra.Query("SELECT SUM(SUBSUM) TOTAL, SUM(DECODE(HSFLAG, '已回收', SUBSUM, 0)) SUBSUM FROM (" + str2 + ")").Tables[0];
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
                //if ((this.GridGoods.DataKeys[num2][1] ?? "N").ToString() == "N")
                //{
                //    object[] objArray1 = new object[] { str, "'", this.GridGoods.DataKeys[num2][0], "'," };
                //    str = string.Concat(objArray1);
                //}
                 str = this.GridGoods.Rows[num2].Values[2].ToString();
           
            if (str.Length > 0)
            {
                //char[] trimChars = new char[] { ',' };
                //DbHelperOra.ExecuteSql("UPDATE DAT_XS_DOC SET STR7 = 'Y',STR8='" + UserAction.UserID + "' WHERE NVL(STR7,'N') = 'N' AND SEQNO IN(" + str.TrimEnd(trimChars) + ")");
                //char[] chArray2 = new char[] { ',' };
                //DbHelperOra.ExecuteSql("UPDATE DAT_CK_DOC SET HDFLAG = 'Y',HDSJ=SYSDATE,HDR='" + UserAction.UserID + "' WHERE NVL(HDFLAG,'N') = 'N' AND SEQNO IN(" + str.TrimEnd(chArray2) + ")");
                //char[] chArray3 = new char[] { ',' };
                //DbHelperOra.ExecuteSql("UPDATE DAT_RK_DOC SET STR5 = 'Y',STR4='" + UserAction.UserID + "' WHERE NVL(STR5,'N') = 'N' AND SEQNO IN(" + str.TrimEnd(chArray3) + ")");
                //DbHelperOra.ExecuteSql("UPDATE DAT_TH_DOC SET STR2 = 'Y',STR1='" + UserAction.UserID + "' WHERE NVL(STR2,'N') = 'N' AND SEQNO IN(" + str.TrimEnd(chArray3) + ")");
                string sSQL = string.Format(@"INSERT INTO DAT_BILL_SURE(SEQNO,FLAG,HSR,HSRQ) VALUES
                    ('{0}','{1}','{2}',sysdate)", str, 'Y', UserAction.UserID);
                DbHelperOra.ExecuteSql(sSQL);
                
            }
            else
            {
                Alert.Show("请选择需要回收的单据！", "提示信息", MessageBoxIcon.Warning);
            }
           }
            DataSearch();

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
            string flag = dataItem["HSFLAG"].ToString();
            if (flag == "未回收")
            {
                FineUIPro.BoundField flagcol = GridGoods.FindColumn("HSFLAG") as FineUIPro.BoundField;
                e.CellCssClasses[flagcol.ColumnIndex] = "color1";
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
            if (ddlHSY.SelectedValue.Length > 0)
            {
                str = " AND A.HDR = '" + ddlHSY.SelectedValue + "'";
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

        protected void GridGoods_Sort(object sender, GridSortEventArgs e)
        {
            DataSearch();
        }
    }
}