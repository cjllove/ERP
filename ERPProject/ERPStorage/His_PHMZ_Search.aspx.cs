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
using Newtonsoft.Json.Linq;

namespace ERPProject.ERPStorage
{
    public partial class His_PHMZ_Search : PageBase
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
            dpkBEGRQ.SelectedDate = DateTime.Now.AddDays(-7);
            dpkENDRQ.SelectedDate = DateTime.Now;
            PubFunc.DdlDataGet(ddlDEPTID, "DDL_SYS_DEPTDEF");

        }
        protected void trbPRODUCER_TriggerClick(object sender, EventArgs e)
        {
            dataSearch();
        }
        private void dataSearch()
        {
            string strsql = @"SELECT IPU.SEQNO,
                               IPU.GDSEQ, IPU.GDNAME,
                               IPU.PRODUCER,
                               IPU.DEPTID,
                               IPU.PATIENT,      
                               IPU.BILLNO,IPU.GDSPEC,
                               IPU.FLAG,IPU.UNIT,IPU.HSJJ,IPU.SL,IPU.HSJE,IPU.XSRQ,
                               DECODE(IPU.FLAG, 'G', '已处理', 'N', '待处理', 'X', '异常','H','不处理') FLAGNAME,
                               IPU.YCTYPE,
                               DECODE(IPU.YCTYPE,0,'正常',1,'商品编码或名称不符',2,'价格不符',3,'单位不符',4,'库存不足',5,'是高值商品',6,'规格不符',7,'商品数量是小数')YCTYPENAME,
                               IPU.XSTYPE,
                               DECODE(IPU.XSTYPE,1,'计费',0,'退费')XSTYPENAME,
                               IPU.GETTIME,
                               IPU.EXECTIME,
                               SD.NAME DEPTNAME
                          FROM INF_PH_USE_MZ IPU,  SYS_DEPT SD
                         WHERE  IPU.DEPTID = SD.CODE
                           AND SD.FLAG = 'Y'  ";
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
            if (srhFLAG.SelectedValue == null)
            {
            }
            else if (srhFLAG.SelectedValue.Length > 0)
            {
                where += " AND IPU.FLAG ='" + srhFLAG.SelectedValue + "' ";
            }
            if (!PubFunc.StrIsEmpty(ddlXSTYPE.SelectedValue))
            {
                where += " AND IPU.XSTYPE = '" + ddlXSTYPE.SelectedValue + "'";

            }
            if (!PubFunc.StrIsEmpty(ddlDEPTID.SelectedValue))
            {
                where += " AND IPU.DEPTID = '" + ddlDEPTID.SelectedValue + "'";
            }
            if (!string.IsNullOrEmpty(txtPATIENT.Text))
            {
                where += " AND (IPU.PATIENT LIKE('%" + txtPATIENT.Text + "%'))  ";
            }
            if (!string.IsNullOrEmpty(ddlYCTYPE.SelectedValue))
            {
                where += " AND IPU.YCTYPE='"+ddlYCTYPE.SelectedValue+"'  ";
            }
            if (!string.IsNullOrEmpty(txbGDSEQ.Text))
            {
                where += string.Format(" AND (IPU.GDSEQ LIKE('%{0}%') OR IPU.GDNAME LIKE('%{0}%')) ", txbGDSEQ.Text.Trim());
            }
            where += string.Format("AND IPU.XSRQ>=TO_DATE('{0}','YYYY-MM-DD HH24:MI:SS') AND IPU.XSRQ<=TO_DATE('{1}','YYYY-MM-DD HH24:MI:SS')+1 ORDER BY DECODE(IPU.FLAG,'X','1','N','2','Y','3',4,'H')", dpkBEGRQ.SelectedDate, dpkENDRQ.SelectedDate);

            int total = 0;
            strsql += where;
            DataTable dt = PubFunc.DbGetPage(GridGoods.PageIndex, GridGoods.PageSize, strsql, ref total);
            DataTable dttotal = DbHelperOra.QueryForTable(strsql);
            GridGoods.RecordCount = total;
            GridGoods.DataSource = dt;
            OutputSummaryData(dt, dttotal);

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
                Alert.Show("没有数据,无法导出！");
                return;
            }
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=谱耗HIS计费明细.xls");
            Response.ContentType = "application/excel";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.Write(PubFunc.GridToHtml(GridGoods));
            Response.End();
            btnExp.Enabled = true;
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
            string strsql = "UPDATE  INF_PH_USE_MZ SET FLAG='H' WHERE SEQNO='{0}' AND FLAG IN('X','N')";
            int[] selectr = GridGoods.SelectedRowIndexArray;
            int countrow = 0;
            if (selectr.Length < 1)
            {
                Alert.Show("先选中行，后操作！");
                return;
            }
            foreach (int gr in GridGoods.SelectedRowIndexArray)
            {
                if (GridGoods.Rows[gr].DataKeys[1].ToString() == "X" || GridGoods.Rows[gr].DataKeys[1].ToString() == "N")
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
                Alert.Show("选中数据无需处理，只有待处理和异常数据才能进行忽略操作！");
            }
        }
        private void OutputSummaryData(DataTable source, DataTable dttotal)
        {
            decimal HSJJTotal = 0, HSJETotal = 0, TOTALSL = 0, TOTALJE = 0;
            foreach (DataRow row in source.Rows)
            {
                HSJJTotal += Convert.ToInt32(row["SL"]);
                HSJETotal += Convert.ToDecimal(row["HSJE"]);
            }
            foreach (DataRow dr in dttotal.Rows)
            {
                TOTALSL += Convert.ToDecimal(dr["SL"]);
                TOTALJE += Convert.ToDecimal(dr["HSJE"]);
            }
            JObject summary = new JObject();
            summary.Add("GDNAME", "分页合计</br>全部合计");
            summary.Add("SL", HSJJTotal.ToString("F2") + "</br>" + TOTALSL.ToString("F2"));
            summary.Add("HSJE", HSJETotal.ToString("F2") + "</br>" + TOTALJE.ToString("F2"));
            GridGoods.SummaryData = summary;
        }
        protected void btnEXEC_Click(object sender, EventArgs e)
        {
            List<CommandInfo> cmdlist = new List<CommandInfo>();
            //OracleParameter[] parameters = {
            //                                                                                 new OracleParameter("V_BEGRQ", OracleDbType.Date),
            //                                                                                 new OracleParameter("V_ENDRQ", OracleDbType.Date) };
            //parameters[0].Value = DateTime.Parse(dpkBEGRQ.Text);
            //parameters[1].Value = DateTime.Parse(dpkENDRQ.Text);
            OracleParameter[] parameters = { };
            cmdlist.Add(new CommandInfo("INF_STORE.ZQRM_P_EXECHIS_PH_MZ", parameters, CommandType.StoredProcedure));
            try
            {
                if (DbHelperOra.ExecuteSqlTran(cmdlist))
                {
                    Alert.Show("执行成功，生成门诊谱耗销售单！");
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
        protected void btnEXECTF_Click(object sender, EventArgs e)
        {
            List<CommandInfo> cmdlist = new List<CommandInfo>();
            //OracleParameter[] parameters = {
            //                                                                                 new OracleParameter("V_BEGRQ", OracleDbType.Date),
            //                                                                                 new OracleParameter("V_ENDRQ", OracleDbType.Date) };
            //parameters[0].Value = DateTime.Parse(dpkBEGRQ.Text);
            //parameters[1].Value = DateTime.Parse(dpkENDRQ.Text);
            OracleParameter[] parameters = { };
            cmdlist.Add(new CommandInfo("INF_STORE.ZQRM_P_EXTCHIS_PHTF_MZ", parameters, CommandType.StoredProcedure));
            try
            {
                if (DbHelperOra.ExecuteSqlTran(cmdlist))
                {
                    Alert.Show("执行成功，生成门诊谱耗退费单！");
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
            OracleParameter[] parameters = { };
            cmdlist.Add(new CommandInfo("INF_STORE.ZQRM_P_INIT_GOODS", parameters, CommandType.StoredProcedure));
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


        }

        protected void btnInitUser_Click(object sender, EventArgs e)
        {

        }

        protected void btnInitDept_Click(object sender, EventArgs e)
        {
            List<CommandInfo> cmdlist = new List<CommandInfo>();
            OracleParameter[] parameters = { };
         
            cmdlist.Add(new CommandInfo("INF_STORE.ZQRM_P_INIT_DEPT", parameters, CommandType.StoredProcedure));
            try
            {
                if (DbHelperOra.ExecuteSqlTran(cmdlist))
                {
                    Alert.Show("科室初始化执行成功！");
                }
                else
                {
                    Alert.Show("科室初始化执行失败！");
                }
            }
            catch (Exception ex)
            {

                Alert.Show(Error_Parse(ex.Message));
                return;
            }

        }
        protected void btnInitCatid_Click(object sender, EventArgs e)
        {
        }

        protected void btnSYNC_Click(object sender, EventArgs e)
        {
            List<CommandInfo> cmdlist = new List<CommandInfo>();
           
            OracleParameter[] parameters = { };
            cmdlist.Add(new CommandInfo("INF_STORE.ZQRM_P_GET_USEDPH_MZ", parameters, CommandType.StoredProcedure));
            try
            {
                if (DbHelperOra.ExecuteSqlTran(cmdlist))
                {
                    Alert.Show("执行成功，成功同步HIS门诊谱耗计费信息！");
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




    }
}