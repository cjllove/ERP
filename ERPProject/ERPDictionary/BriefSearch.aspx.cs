﻿using FineUIPro;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using System.IO;
using XTBase;
using XTBase.Utilities;
using System.Drawing;


namespace ERPProject.ERPDictionary
{
    public partial class BriefSearch : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
                DataQuery();
               
            }
        }

        private void DataInit()
        {
            PubFunc.DdlDataGet(ddlUSERS, "DDL_USER");
            ddlUSERS.SelectedValue = "";
            ddlISSEND.SelectedValue = "";
            dpkBegRQ.SelectedDate = System.DateTime.Now.AddMonths(-1);
            dpkEndRQ.SelectedDate = System.DateTime.Now;
            btnBriefScreen_Click(null,null);
        }

        private void DataQuery()
        {
            String sql = @"SELECT DBC.USERS USERID,OU.USERNAME,DBC.ISSEND,DECODE(DBC.ISSEND,'N','未发送','Y','已发送','E','异常')ISSENDNAME,DBC.BRCONTENT,DBC.TIMEUP FROM DOC_BRIEF_CON DBC,SYS_OPERUSER OU WHERE DBC.USERS=OU.USERID ";
            int total = 0;
            string strwhere = "";
            PubFunc.FormDataCheck(FormSearch);
            if (!string.IsNullOrEmpty(ddlUSERS.SelectedValue))
            {
                strwhere +=string.Format( " AND OU.USERID LIKE'%{0}%' ",ddlUSERS.SelectedValue);
            }
            if (!string.IsNullOrEmpty(ddlISSEND.SelectedValue))
            {
                strwhere += string.Format(" AND DBC.ISSEND='{0}' ",ddlISSEND.SelectedValue);
            }
            if (dpkBegRQ.SelectedDate != null && dpkEndRQ.SelectedDate != null)
            {
                strwhere += string.Format(" AND TRUNC(DBC.TIMEUP,'DD') BETWEEN TRUNC(TO_DATE('{0}','YYYY-MM-DD HH24:MI:SS'),'DD') AND TRUNC(TO_DATE('{1}','YYYY-MM-DD HH24:MI:SS'),'DD')  ",dpkBegRQ.SelectedDate,dpkEndRQ.SelectedDate);
            }
            DataTable dt = PubFunc.DbGetPage(GridToBrief.PageIndex, GridToBrief.PageSize, sql+strwhere+" ORDER BY DECODE(DBC.ISSEND,'1','2'),DBC.TIMEUP ", ref total);

                GridToBrief.DataSource = dt;
            GridToBrief.RecordCount = total;
            GridToBrief.DataBind();
        }
        protected void GridToBrief_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                string flag = row["ISSEND"].ToString();
                FineUIPro.BoundField flagcol = GridToBrief.FindColumn("ISSENDNAME") as FineUIPro.BoundField;
             
                if (flag == "E")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color3";
                }
            }
        }

        protected void GridToBrief_RowDoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            string strcode = GridToBrief.Rows[e.RowIndex].Values[0].ToString();
            DataTable dt = DbHelperOra.Query("select * from DOC_BRIEF_DOC where code='" + strcode + "'").Tables[0];
           
        }

        protected void GridToBrief_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridToBrief.PageIndex = e.NewPageIndex;
            DataQuery();
        }

        protected void tgbSearch_TriggerClick(object sender, EventArgs e)
        {
            DataQuery();
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
           
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (GridToBrief.SelectedRowIndexArray.Length > 0)
            {
                string ID = "";
                for (int i = 0; i < GridToBrief.SelectedRowIndexArray.Length; i++)
                {
                    ID += GridToBrief.Rows[GridToBrief.SelectedRowIndexArray[i]].Values[1].ToString() + ",";
                }
                DbHelperOra.ExecuteSql("delete from DAT_DO_TYPE where DOTYPE in ('" + ID.TrimEnd(',').Replace(",", "','") + "')");
                DataQuery();
            }
            else
            {
                Alert.Show("请选择要删除的信息！", MessageBoxIcon.Warning);
            }
        }

      

      
      
        public static string Error_Parse(string error)
        {
            string value = string.Empty;
            if (error.IndexOf("ORA-") > -1)
            {
                value = error.Replace("\n", "").Substring(error.IndexOf("ORA-") + 10);
                if (value.IndexOf("ORA-") > -1)
                {
                    value = value.Substring(0, value.IndexOf("ORA-"));
                }
            }
            else
            {
                value = error;
            }

            return value;
        }

        protected void btnUserScreen_Click(object sender, EventArgs e)
        {
            DataTable dt = DbHelperOra.QueryForTable(@"select CODE BRCODE,
                                       NAME BRNAME,
                                       FLAG,
                                       DECODE(FLAG, 'Y', '启用', 'N', '不启用','未维护') FLAGNAME
                                       F_JOINUSERID(CODE) USERID

                                       
                          from DOC_BRIEF_DOC A
                         where  1=1");
           
        }

       

        protected void btnBriefScreen_Click(object sender, EventArgs e)
        {
            DataTable dt = DbHelperOra.QueryForTable(@"select CODE BRCODE,
                                       NAME BRNAME,
                                       FLAG,
                                       DECODE(A.FLAG, 'Y', '启用', 'N', '不启用','未维护') FLAGNAME
                                     
                                       
                          from DOC_BRIEF_DOC A
                         where  1=1 ORDER BY CODE ASC");
           
        }

        protected void GridUsers_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
           
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            DataQuery();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            DataInit();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {

            String sql = @"SELECT DBC.USERS USERID,OU.USERNAME,DBC.ISSEND,DECODE(DBC.ISSEND,'N','未发送','Y','已发送','E','异常')ISSENDNAME,DBC.BRCONTENT,DBC.TIMEUP FROM DOC_BRIEF_CON DBC,SYS_OPERUSER OU WHERE DBC.USERS=OU.USERID ";
            int total = 0;
            string strwhere = "";
            PubFunc.FormDataCheck(FormSearch);
            if (!string.IsNullOrEmpty(ddlUSERS.SelectedValue))
            {
                strwhere += string.Format(" AND OU.USERID LIKE'%{0}%' ", ddlUSERS.SelectedValue);
            }
            if (!string.IsNullOrEmpty(ddlISSEND.SelectedValue))
            {
                strwhere += string.Format(" AND DBC.ISSEND='{0}' ", ddlISSEND.SelectedValue);
            }
            if (dpkBegRQ.SelectedDate != null && dpkEndRQ.SelectedDate != null)
            {
                strwhere += string.Format(" AND TRUNC(DBC.TIMEUP,'DD') BETWEEN TRUNC(TO_DATE('{0}','YYYY-MM-DD HH24:MI:SS'),'DD') AND TRUNC(TO_DATE('{1}','YYYY-MM-DD HH24:MI:SS'),'DD')  ", dpkBegRQ.SelectedDate, dpkEndRQ.SelectedDate);
            }
            sql += strwhere;
            DataTable dtData = DbHelperOra.Query(sql).Tables[0];
            if (dtData == null || dtData.Rows.Count == 0)
            {
                Alert.Show("无法导出！", "消息提示", MessageBoxIcon.Warning);
                return;
            }



            DataTable dt = PubFunc.GridDataGet(GridToBrief);
            string[,] colName = { { "USERID", "USERNAME", "ISSENDNAME", "TIMEUP", "BRCONTENT" }, { "用户编码", "用户名称", "状态", "更新时间", "发送内容" } };
            ExcelHelper.ExportByWeb(dt, "简报发送信息导出", "简报发送信息导出_" + DateTime.Now.ToString("yyyyMMddHH") + ".xls", colName);
        }

      
      
    }
}