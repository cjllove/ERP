﻿using FineUIPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using XTBase;

namespace ERPProject.Certificate
{
    public partial class CertificateCat : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
            }
        }

        private void DataInit() {
            //PubFunc.DdlDataBind(ddlOBJUSER, renderObjUser());
            //PubFunc.DdlDataBind(ddlFLAG, renderFlag());
            //PubFunc.DdlDataBind(lstOBJUSER, renderObjUser());
            //PubFunc.DdlDataBind(lstFLAG, renderFlag());
            PubFunc.DdlDataGet("DDL_LICENSETYPE_W", ddlOBJUSER, lstOBJUSER);
            PubFunc.DdlDataGet("DDL_LICENSESTATUS_W", ddlFLAG, lstFLAG);
        }

        //private DataTable renderFlag() {
        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("CODE", typeof(string));
        //    dt.Columns.Add("NAME", typeof(string));
        //    DataRow dr = dt.NewRow();
        //    dr["CODE"] = "Y";
        //    dr["NAME"] = "正常";
        //    dt.Rows.Add(dr);
        //    dr = dt.NewRow();
        //    dr["CODE"] = "N";
        //    dr["NAME"] = "停用";
        //    dt.Rows.Add(dr);
        //    return dt;
        //}

        //private DataTable renderObjUser() { 
        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("CODE",typeof(string));
        //    dt.Columns.Add("NAME",typeof(string));
        //    DataRow dr = dt.NewRow();
        //    dr["CODE"] = "SUPPLIER";
        //    dr["NAME"] = "供应商证照";
        //    dt.Rows.Add(dr);
        //    dr = dt.NewRow();
        //    dr["CODE"] = "GOODS";
        //    dr["NAME"] = "商品证照";
        //    dt.Rows.Add(dr);
        //    return dt;
        //}

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            dataSearch();
        }
        private void dataSearch()
        {
            int total = 0;
            string sql = @"select CODE,NAME,DECODE(FLAG,'Y','正常','N','停用','其他') flag,
                                DECODE(OBJUSER,'SUPPLIER','供应商证照','GOODS','商品证照') objuser,
                                decode(isneed,'Y','是','N','否') isneed,
                                decode(isdate,'Y','是','N','否') isdate,
                                decode(isdup,'Y','是','N','否') isdup,
                                memo, SORT from  doc_license WHERE 1=1";
            string strFLAG = lstFLAG.SelectedValue.ToString();
            string strOBJUSER = lstOBJUSER.SelectedValue.ToString();
            if (!string.IsNullOrWhiteSpace(strFLAG))
            {
                sql += " and FLAG = '" + strFLAG + "'";
            }
            if (!string.IsNullOrWhiteSpace(strOBJUSER))
            {
                sql += " and OBJUSER = '" + strOBJUSER + "'";
            }
            string query = "%";
            if (!string.IsNullOrWhiteSpace(tgbSearch.Text))
            {
                query = tgbSearch.Text.Trim();
                sql += string.Format(" and ( CODE like '%{0}%' or NAME like '%{0}%')", query); 
            }
            DataTable dtData = PubFunc.DbGetPage(GridSupplier.PageIndex, GridSupplier.PageSize, sql, ref total);
            GridSupplier.RecordCount = total;
            GridSupplier.DataSource = dtData;
            //GridSupplier.DataSource = DbHelperOra.Query(string.Format(sql, query));
            GridSupplier.DataBind();
        }

        protected void GridSupplier_RowDoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            tbxCODE.Enabled = false;
            hfdIsNew.Text = "N";
            string strCode = GridSupplier.Rows[e.RowIndex].Values[0].ToString();
            string strSql = string.Format("select * from doc_license where code='{0}'", strCode);

            DataTable dtProducer = DbHelperOra.Query(strSql).Tables[0];
            if (dtProducer.Rows.Count > 0)
            {
                PubFunc.FormDataSet(FormProducer, dtProducer.Rows[0]);
            }
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            PubFunc.FormDataClear(FormProducer);
            hfdIsNew.Text = "Y";
            tbxCODE.Enabled = true;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            MyTable mtType = new MyTable("DOC_LICENSE");
            if (PubFunc.FormDataCheck(FormProducer).Length > 1)
            {
                return; //存在为空的非空列则返回！
            }
            if ((DbHelperOra.Exists("select 1 from DOC_LICENSE where code = '" + tbxCODE.Text + "'")) && ( !hfdIsNew.Text.Equals("N")))
            {
                Alert.Show("此证照编码已经存在,请重新输入!");
                return;
            }
            mtType.ColRow = PubFunc.FormDataHT(FormProducer);


            if (hfdIsNew.Text == "" || hfdIsNew.Text == "Y")
            {
                mtType.InsertExec();
            }
            else
            {
                mtType.UpdateExec("");
            }

            Alert.Show("数据保存成功！");
            dataSearch();
            hfdIsNew.Text = "N";
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            int[] rows = GridSupplier.SelectedRowIndexArray;
            if (rows.Length > 0)
            {
                string code = "";
                foreach (int index in rows)
                {
                    code += PubFunc.GridDataGet(GridSupplier, index)["CODE"].ToString() + ",";
                }
                code = code.TrimEnd(',').Replace(",", "','");
                DbHelperOra.ExecuteSql("DELETE FROM DOC_LICENSE WHERE CODE IN ('" + code + "')");
                Alert.Show("证照类别【" + code + "】删除成功！");

                dataSearch();
            }
            else
            {
                Alert.Show("请选择要删除的证照类别！");
            }
        }
        protected void Grid1_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridSupplier.PageIndex = e.NewPageIndex;
            dataSearch();
        }    
    }
}