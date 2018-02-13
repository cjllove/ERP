﻿using FineUIPro;
using Oracle.ManagedDataAccess.Client;
using XTBase;
using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XTBase.Utilities;

namespace ERPProject.ERPStorage
{
    public partial class MonthPurchaseSchedule : BillBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                initData();
                if (Request.QueryString["oper"] != null)
                {
                    if (Request.QueryString["oper"].ToString() == "chart")
                    {
                        
                        hfdoper.Text = "chart";
                    }
                    else if (Request.QueryString["oper"].ToString() == "report")
                    {
                        hfdoper.Text = "report";
                        TabStrip1.ActiveTabIndex = 0;
                        DataSearch();
                    }
                }
            }

        }
        private void initData()
        {
             
            DepartmentBind.BindDDL("DDL_SYS_DEPTRANGE", UserAction.UserID,  lstDEPTID);
               
        }
        public  void DataSearch()
        {
            string strSQL = @"
                                SELECT DOC.BILLTYPE,
                                       COM.GDSEQ,
                                       COM.GDNAME,
                                       COM.GDSPEC,
                                       f_getcatname(catid) CATID,
                                       f_getsupname(supID) SUPID,
                                       DHS,
                                       DHS* HSJJ HSJE,
                                       DHSL,
                                       DHSL* HSJJ DHJE,
                                       ROUND(Decode(DHS, 0, DHS * HSJJ / DHSL * HSJJ), 2) PerRate,
                                       f_getproducername(PRODUCER) PRODUCER
                                  FROM DAT_DDPLAN_DOC DOC, DAT_DDPLAN_COM COM
                                 WHERE DOC.SEQNO = COM.SEQNO
                                   AND DOC.FLAG = 'Y'";
                                   
              if (!string.IsNullOrEmpty(txtGDSEQ.Text))
            {
                strSQL+= "  AND  COM.GDSEQ LIKE '%"+txtGDSEQ.Text.Trim()+"%'";

            }
              if(!string.IsNullOrEmpty(tbxJHYF.Text))
            {
                strSQL += "  AND TO_CHAR(DOC.XDRQ, 'YYYY-MM') = TO_CHAR('" + tbxJHYF.Text.Trim() + "-01', 'YYYY-MM') ";

            }
            if (!string.IsNullOrEmpty(lstBILLNO.Text))
            {

                strSQL += "  AND DOC.BILLNO LIKE '%"+ lstBILLNO.Text.Trim()+ "%' ";
            }
            if (!string.IsNullOrEmpty(txtProducter.Text))
            {
                strSQL += "  AND PRODUCER LIKE '%" + lstBILLNO.Text.Trim() + "%' ";
            }
            if (!string.IsNullOrEmpty(lstDEPTID.SelectedValue))
            {
                strSQL += "  AND DOC.DEPTID= '" + lstDEPTID.SelectedValue + "' ";
            }
            int total = 0;
            DataTable dt= PubFunc.DbGetPage(GridList.PageIndex, GridList.PageSize, strSQL, ref total);
            GridList.DataSource = dt;
            GridList.RecordCount = total;
            GridList.DataBind();


        }
        protected void btnMySerarch_Click(object sender, EventArgs e)
        {

            DataSearch();
        }
        protected void GridList_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridList.PageIndex = e.NewPageIndex;
            DataSearch();
        }
        protected void btnSearchChart_Click(object sender, EventArgs e)
        {



        }
    }
}