using FineUIPro;
using Newtonsoft.Json.Linq;
using XTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;

namespace ERPProject.ERPBasic
{
    public partial class FP_Manage : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                bind();
            }
        }
        protected void bind()
        {
            PubFunc.DdlDataGet("DDL_JS_FP", lstFPTYPE);
            PubFunc.DdlDataGet("DDL_JSD_FP", ddlTAXRATE);
            PubFunc.DdlDataGet("DDL_USER", lstLRY);
            PubFunc.DdlDataGet("DDL_DOC_SUPPLIERNULL", ddlSUPID);
            ddlSUPID.SelectedValue = "00002";
            lstLRY.SelectedValue = UserAction.UserID;
            lstLRRQ.SelectedDate = DateTime.Now;
        }
        protected void bntSearch_Click(object sender, EventArgs e)
        {
            open();
        }
        protected void open()
        {
            string strSql = @"SELECT A.*,F_GETUSERNAME(A.LRY) LRYNAME,B.NAME FPTYPENAME,DECODE(A.FLAG,'N','未使用','已使用') FLAGNAME FROM DAT_JSD_FP A,sys_codevalue B WHERE A.FPTYPE = B.CODE AND B.type ='DAT_JS_FP' ";
            string strSearch = "";
            if (lstSEQNO.Text.Length > 0)
            {
                strSearch += " and A.SEQNO like '%" + lstSEQNO.Text + "%'";
            }
            if (lstFPTYPE.SelectedValue.Length > 0)
            {
                strSearch += " and A.FPTYPE ='" + lstFPTYPE.SelectedValue + "'";
            }
            if (ddlSUPID.SelectedValue.Length > 0)
            {
                strSearch += " and A.SUPID ='" + ddlSUPID.SelectedValue + "'";
            }
            strSql = strSql + strSearch + " order by A.FLAG";
            GridList.DataSource = DbHelperOra.Query(strSql).Tables[0];
            GridList.DataBind();
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (ddlFLAG.SelectedValue != "N")
            {
                Alert.Show("此发票已被使用,不允许更改!");
                return;
            }
            if (PubFunc.FormDataCheck(Formlist).Length > 0)
            { Alert.Show("信息未维护完全!") ;return; }
            if (lstSEQNO.Enabled)
            {
                if (DbHelperOra.Exists("SELECT 1 FROM dat_jsd_fp WHERE SEQNO='" + lstSEQNO.Text + "'"))
                {
                    Alert.Show("此发票号码已存在,请检查!");
                    return;
                }
                string strSql = @"INSERT INTO dat_jsd_fp(CUSTID,SEQNO,FPTYPE,FLAG,TAXRATE,SUPID,SUPNAME,LRY,LRRQ,FPJE,JSJE,USEJE,MEMO) 
                select F_GETPARA('USERNAME'),'" + lstSEQNO.Text + "','" + lstFPTYPE.SelectedValue + "','N','" + ddlTAXRATE.SelectedValue + "','" + ddlSUPID.SelectedValue + "',F_GETSUPNAME('" + ddlSUPID.SelectedValue + "'),'" + lstLRY.SelectedValue + "',to_date('" + lstLRRQ.Text + "','YYYY-MM-DD')," + nbxFPJE.Text + ",0," + nbxFPJE.Text + ",'" + tbxMEMO.Text + "' from dual";
                DbHelperOra.ExecuteSql(strSql);
                open();
            }
            else
            {
                if (lstSEQNO.Text.Length < 1) return;
                string strUp = "update dat_jsd_fp set FPJE =" + nbxFPJE.Text + ",USEJE =" + nbxFPJE.Text + ",FPTYPE='" + lstFPTYPE.SelectedValue + "',SUPID='" + ddlSUPID.SelectedValue + "',TAXRATE='" + ddlTAXRATE.SelectedValue + "',MEMO ='" + tbxMEMO.Text + "' WHERE SEQNO='" + lstSEQNO.Text+ "'";
                DbHelperOra.ExecuteSql(strUp);
            }
            Alert.Show("保存数据成功!");
        }
        protected void btnNew_Click(object sender, EventArgs e)
        {
            lstSEQNO.Enabled = true;
            lstFPTYPE.Enabled = true;
            lstLRRQ.SelectedDate = DateTime.Now;
            lstSEQNO.Text = string.Empty;
            nbxFPJE.Text = "0";
            tbxUSEJE.Text = "0";
            ddlFLAG.SelectedValue = "N";
            tbxMEMO.Text = string.Empty;
            GridList.DataSource = null;
            GridList.DataBind();
        }

        protected void bntClear_Click(object sender, EventArgs e)
        {
            lstSEQNO.Text = string.Empty;
            lstFPTYPE.SelectedValue = string.Empty;
            ddlTAXRATE.SelectedValue = string.Empty;
            tbxMEMO.Text = string.Empty;
            ddlSUPID.SelectedValue = string.Empty;
            nbxFPJE.Text = "0";
            tbxUSEJE.Text = "0";
        }

        protected void btnDel_Click(object sender, EventArgs e)
        {
            if (GridList.SelectedRowIndexArray.Length < 1) return;
            int rowIndex = GridList.SelectedRowIndexArray[0];
            if (GridList.DataKeys[rowIndex][0].ToString() != "N")
            {
                Alert.Show("此发票已被使用,不允许删除!");
                return;
            }
            DbHelperOra.ExecuteSql("DELETE FROM dat_jsd_fp WHERE SEQNO='" + GridList.DataKeys[rowIndex][1] + "' AND FLAG='N'");
            PageContext.RegisterStartupScript(GridList.GetDeleteRowReference(rowIndex));
        }

        protected void GridList_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            if (GridList.SelectedRowIndexArray.Length < 1) return;
            int rowIndex = GridList.SelectedRowIndexArray[0];
            //表头赋值
            string strSql = "select * from dat_jsd_fp where SEQNO = {0}";
            DataTable dtDoc = DbHelperOra.Query(string.Format(strSql, GridList.DataKeys[rowIndex][1])).Tables[0];
            PubFunc.FormDataSet(Formlist, dtDoc.Rows[0]);
            lstSEQNO.Enabled = false;
        }
    }
}