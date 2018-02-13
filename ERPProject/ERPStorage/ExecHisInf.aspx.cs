﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using FineUIPro;
using System.Collections.Specialized;
using System.Text;
using System.Collections;
using XTBase;
using Oracle.ManagedDataAccess.Client;
using XTBase.Utilities;

namespace ERPProject.ERPStorage
{
    public partial class ExecHisInf : PageBase
    {
        private static DataTable dtData;
        private static bool FLAG = true;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
                dataSearch();
            }
        }

        private void DataInit()
        {
            dpkBEGRQ.SelectedDate = DateTime.Now.AddDays(-1);
            dpkENDRQ.SelectedDate = DateTime.Now;
          
          
        }
        protected void trbPRODUCER_TriggerClick(object sender, EventArgs e)
        {
            dataSearch();
        }
        private void dataSearch()
        {
            string strsql = @"SELECT RIG.YAZ01,RIG.DEPTID,(SELECT NAME FROM SYS_DEPT WHERE CODE=RIG.DEPTID)DEPTNAME,RIG.FLAG,DECODE(RIG.FLAG,'Y','处理成功','N','未处理','E','异常') FLAGNAME,RIG.BBY04,(SELECT GDNAME FROM DOC_GOODS WHERE GDSEQ=RIG.BBY04)GDNAME ,RIG.VAJ25,RIG.YAZ20,RIG.PRODATE,RIG.MEMO
            FROM INF_DOC_GOODS IDG WHERE IDG.FLAG<>'H' ";
            string where = "";
            if (dpkBEGRQ.SelectedDate == null || dpkENDRQ.SelectedDate == null)
            {
                Alert.Show("时间填写不能为空！");
                return;
            }
            else
            {
                if (dpkENDRQ.SelectedDate > dpkENDRQ.SelectedDate)
                {
                    Alert.Show("开始时间不能晚于结束时间！");
                }
            }
            if (srhFLAG.SelectedValue=="Y")
            {
                where += " AND RIG.FLAG='E' ";
            }
            else if (srhFLAG.SelectedValue == "N")
            {
                where += " AND RIG.FLAG IN('Y','N') ";
            }
            where += string.Format("AND RIG.YAZ20>=TO_DATE('{0}','YYYY-MM-DD HH24:MI:SS') AND RIG.YAZ20<=TO_DATE('{1}','YYYY-MM-DD HH24:MI:SS') ORDER BY DECODE(RIG.FLAG,'E','1','N','2','Y','3')",dpkBEGRQ.SelectedDate,dpkENDRQ.SelectedDate);
            
            int total = 0;
            strsql += where;
            DataTable dt = PubFunc.DbGetPage(GridGoods.PageIndex, GridGoods.PageSize, strsql, ref total);
            GridGoods.RecordCount = total;
            GridGoods.DataSource = dt;
            GridGoods.DataBind();
        }

        protected void ddlCATID_SelectedIndexChanged(object sender, EventArgs e)
        {
            string strCATID0, strCATID, strSQL;
            strCATID = ((FineUIPro.DropDownList)sender).SelectedValue;
            if (PubFunc.StrIsEmpty(strCATID)) return;

            strSQL = "select type from sys_category where code='" + strCATID + "'";
            strCATID0 = DbHelperOra.GetSingle(strSQL).ToString();

           
        }
       
        protected void trbSearch_TriggerClick(object sender, EventArgs e)
        {
            dataSearch();
        }
        protected void btnNew_Click(object sender, EventArgs e)
        {
                 }   
    
        protected void btExp_Click(object sender, EventArgs e)
        
        {
            if (GridGoods.Rows.Count < 1)
            {
                Alert.Show("没有数据，无法导出！");
                return;
            }
            string strsql = @"SELECT RIG.YAZ01,RIG.DEPTID,(SELECT NAME FROM SYS_DEPT WHERE CODE=RIG.DEPTID)DEPTNAME,RIG.FLAG,DECODE(RIG.FLAG,'Y','处理成功','N','未处理','E','异常') FLAGNAME,RIG.BBY04,(SELECT GDNAME FROM DOC_GOODS WHERE GDSEQ=RIG.BBY04)GDNAME ,RIG.VAJ25,RIG.YAZ20,RIG.PRODATE,RIG.MEMO
            FROM RCRM_INF_GOODS RIG WHERE 1=1 ";
            string where = "";
            if (dpkBEGRQ.SelectedDate == null || dpkENDRQ.SelectedDate == null)
            {
                Alert.Show("时间填写不能为空！");
                return;
            }
            else
            {
                if (dpkENDRQ.SelectedDate > dpkENDRQ.SelectedDate)
                {
                    Alert.Show("开始时间不能晚于结束时间！");
                }
            }
            if (srhFLAG.SelectedValue == "Y")
            {
                where += " AND RIG.FLAG='E' ";
            }
            else if (srhFLAG.SelectedValue == "N")
            {
                where += " AND RIG.FLAG IN('Y','N') ";
            }
            where += string.Format("AND RIG.YAZ20>=TO_DATE('{0}','YYYY-MM-DD HH24:MI:SS') AND RIG.YAZ20<=TO_DATE('{1}','YYYY-MM-DD HH24:MI:SS')", dpkBEGRQ.SelectedDate, dpkENDRQ.SelectedDate);
            strsql += where;
            DataTable dt = DbHelperOra.Query(strsql).Tables[0];
            string[,] colName = { { "YAZ01", "DEPTID", "DEPTNAME", "FLAGNAME", "BBY04", "GDNAME", "VAJ25", "YAZ20", "PRODATE", "MEMO" }, { "序列号", "领取部门编号", "领取部门名称", "状态", "商品编码", "商品名称", "数量", "数据生成时间", "处理生成时间", "备注" } };
            ExcelHelper.ExportByWeb(dt, "接口处理查询导出", "接口处理查询导出_" + DateTime.Now.ToString("yyyyMMddHH") + ".xls", colName);
        }
        protected void Window1_Close(object sender, EventArgs e)
        {
            dataSearch();
        }

      

        public DataTable GetGoods(NameValueCollection nvc)
        {
            int total = 0;
            string strMsg = "";
            return GetGoodsList(0, 0, nvc, ref total, ref strMsg);
        }
       
        public DataTable GetGoodsList(int pageNum, int pageSize, NameValueCollection nvc, ref int total, ref string errMsg)
        {
            string strSearch = "";
            if (nvc != null)
            {
                foreach (string key in nvc)
                {
                    string condition = nvc[key];
                    if (!string.IsNullOrEmpty(condition))
                    {
                        switch (key.ToUpper())
                        {
                            case "SEQ":
                                strSearch += string.Format(" AND sp.GDSEQ='{0}'", condition);
                                break;
                            case "CATID0":
                                strSearch += string.Format(" AND sp.CATID0='{0}'", condition);
                                break;
                            case "FLAG":
                                strSearch += string.Format(" AND sp.FLAG='{0}'", condition);
                                break;
                            case "CX":
                                strSearch += string.Format(" AND (sp.GDSEQ LIKE '%{0}%' OR sp.GDNAME LIKE '%{0}%' OR sp.HISCODE LIKE '%{0}%' OR sp.HISNAME LIKE '%{0}%' OR sp.BARCODE  LIKE '%{0}%' OR  sp.BAR3 LIKE '%{0}%' OR sp.ZJM  LIKE '%{0}%' OR F_GETPRODUCERNAME(SP.PRODUCER) LIKE '%{0}%')", condition.ToUpper());
                                break;
                        }
                    }
                }
            }
         
            string strSql = @"SELECT SP.GDSEQ,SP.GDID,F_GETHISINFO（SP.GDSEQ，'GDNAME') GDNAME,SP.BARCODE,E.NAME CATID0NAME,B.NAME CATID0NAME_F,F_GETHISINFO（SP.GDSEQ，'GDSPEC') GDSPEC,D.NAME UNITNAME,SP.BZHL,
                               ROUND(SP.HSJJ,4) HSJJ,ROUND(SP.LSJ,4) LSJ,C.SUPNAME,SP.ZPBH,S.NAME FLAG_CN,SP.PIZNO,F_GETPRODUCERNAME(SP.PRODUCER) PRODUCERNAME,decode(sp.isflag7,'Y','是','否') ISNEW_CN
                          from DOC_GOODS SP,
                               SYS_CATEGORY B,
                               DOC_SUPPLIER C,
                               DOC_GOODSUNIT D,
                               doc_goodstype e,
                               (SELECT CODE, NAME FROM SYS_CODEVALUE WHERE TYPE = 'GOODS_STATUS') S
                         WHERE SP.CATID=B.CODE(+) AND SP.FLAG=S.CODE AND SP.SUPPLIER=C.SUPID(+) AND SP.UNIT = D.CODE(+) and SP.CATID0 = e.code(+) ";

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql = strSql + strSearch + " order by SP.gdseq";
            }
            else
            {
                strSql = strSql + " order by SP.gdseq";
            }

            return PubFunc.DbGetPage(pageNum, pageSize, strSql, ref total);
        }

        protected void GridGoods_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridGoods.PageIndex = e.NewPageIndex;
            dataSearch();
        }

     
       

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            dataSearch();
        }

        protected void btnNoExec_Click(object sender, EventArgs e)
        {
            string strsql = "UPDATE  RCRM_INF_GOODS SET FLAG='H' WHERE YAZ01='{0}' AND FLAG='E'";
            int[] selectr = GridGoods.SelectedRowIndexArray;
            int countrow = 0;
            if (selectr.Length < 1)
            {
                Alert.Show("先选中行，后操作！");
                return;
            }
            foreach (int gr in GridGoods.SelectedRowIndexArray)
            {
                if (GridGoods.Rows[gr].DataKeys[1].ToString() == "E")
                {
                    DbHelperOra.ExecuteSql(string.Format(strsql, GridGoods.Rows[gr].DataKeys[0].ToString()));
                    countrow++;
                }
            }
            if (countrow > 0)
            {
                Alert.Show("已处理" + countrow + "条数据！");
            }
            else
            {
                Alert.Show("选中数据无需处理！");
            }
        }

        protected void btnEXEC_Click(object sender, EventArgs e)
        {
            List<CommandInfo> cmdlist = new List<CommandInfo>();
            OracleParameter[] parameters = {
                                                                                             new OracleParameter("V_BEGRQ", OracleDbType.Date),
                                                                                             new OracleParameter("V_ENDRQ", OracleDbType.Date) };
            parameters[0].Value =  DateTime.Parse(dpkBEGRQ.Text);
            parameters[1].Value = DateTime.Parse(dpkENDRQ.Text);
            cmdlist.Add(new CommandInfo("INF_STORE.RCRM_P_TOINF_SL", parameters, CommandType.StoredProcedure));
            try
            {
                if (DbHelperOra.ExecuteSqlTran(cmdlist))
                {
                    Alert.Show("执行成功！");
                }
                else
                {
                    Alert.Show("执行失败！");
                }
            }
            catch (Exception ex)
            {

                Alert.Show(Error_Parse(ex.Message));
                return;
            }

            dataSearch();
          
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

        protected void btnInitGoods_Click(object sender, EventArgs e)
        {

            List<CommandInfo> cmdlist = new List<CommandInfo>();
            OracleParameter[] parameters = {
                                                                                             new OracleParameter("V_BEGRQ", OracleDbType.Date),
                                                                                             new OracleParameter("V_ENDRQ", OracleDbType.Date) };
            parameters[0].Value = DateTime.Parse(dpkBEGRQ.Text);
            parameters[1].Value = DateTime.Parse(dpkENDRQ.Text);
            cmdlist.Add(new CommandInfo("INF_STORE.RCRM_P_TO_DOCGOODS", parameters, CommandType.StoredProcedure));
            try
            {
                if (DbHelperOra.ExecuteSqlTran(cmdlist))
                {
                    Alert.Show("商品资料初始化执行成功！");
                }
                else
                {
                    Alert.Show("商品资料初始化执行失败！");
                }
            }
            catch (Exception ex)
            {

                Alert.Show(Error_Parse(ex.Message));
                return;
            }

            dataSearch();
          
        }

        protected void btnInitUser_Click(object sender, EventArgs e)
        {

        }

        protected void btnInitDept_Click(object sender, EventArgs e)
        {

        }

   
      

    
    }
}