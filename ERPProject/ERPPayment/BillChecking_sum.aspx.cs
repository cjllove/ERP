﻿using FineUIPro;
using Newtonsoft.Json.Linq;
using XTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using Oracle.ManagedDataAccess.Client;

namespace ERPProject.ERPApply
{
    public partial class BillChecking_sum : BillBase
    {
        public BillChecking_sum()
        {
            BillType = "JSD";
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
                billNew();
            }
        }
        private void DataInit()
        {
            if (Request.QueryString["oper"].ToString() == "ck" || Request.QueryString["oper"].ToString() == "xs")
            {
                PubFunc.DdlDataGet("DDL_SYS_DEPT", lstDEPTOUT, ddlDEPTID);
            }
            else
            {
                PubFunc.DdlDataGet("DDL_SYS_DEPOT", lstDEPTOUT, ddlDEPTID);
            }

            PubFunc.DdlDataGet("DDL_USER", lstJSY);
            PubFunc.DdlDataGet("DDL_DOC_SHS", ddlsup, ddlSUPID);
            PubFunc.FormLock(Formlis, true);
            //lstLRRQ1默认为上次结算日期
            //lstLRRQ1.SelectedDate = DateTime.Now;
            lstLRRQ2.SelectedDate = DateTime.Now;
            //默认为威高
            ddlsup.SelectedValue = PubFunc.DbGetPara("SUPPLIER");
            lstJSY.SelectedValue = UserAction.UserID;
        }

        protected override void billNew()
        {
            billLockDoc(false);
        }

        protected override void billClear()
        {
            PubFunc.FormDataClear(Formlist);
            //默认为威高
            ddlsup.SelectedValue = PubFunc.DbGetPara("SUPPLIER"); ;
            //lstLRRQ1.SelectedDate = DateTime.Now;
            lstLRRQ2.SelectedDate = DateTime.Now;
        }
        protected override void billSearch()
        {
            if (lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【结算日期】！");
                return;
            }
            if (lstLRRQ2.SelectedDate > DateTime.Now.Date)
            {
                Alert.Show("【结算日期】不能大于当前日期");
                return;
            }
            string strSql = "";
            if (Request.QueryString["oper"].ToString() == "rk")
            {
                strSql = @"SELECT A.DEPTID,A.PSSID,SUM(JSJE) PRICE_USE,SUM(THJE) PRICE_RTN,SUM(JSJE+THJE) YJJJ,f_getdeptname(A.DEPTID) DEPTNAME,f_getsupname(A.PSSID) PSSNAME,F_GETTIME_JS(A.DEPTID,A.TYPE) KSTIME,'{0}' JSTIME
                        FROM VIEW_JS A 
                        WHERE RQSJ < TO_DATE('{0}','yyyy-MM-dd')+1 AND NVL(STR1,'#') = '#' AND A.TYPE = 'RKJ' AND A.PSSID<>'00001'";
            }
            else if (Request.QueryString["oper"].ToString() == "ck")
            {
                strSql = @"SELECT A.DEPTID,A.PSSID,SUM(JSJE) PRICE_USE,SUM(THJE) PRICE_RTN,SUM(JSJE+THJE) YJJJ,f_getdeptname(A.DEPTID) DEPTNAME,f_getsupname(A.PSSID) PSSNAME,F_GETTIME_JS(A.DEPTID,A.TYPE) KSTIME,'{0}' JSTIME
                        FROM VIEW_JS A
                        WHERE RQSJ < TO_DATE('{0}','yyyy-MM-dd')+1 AND NVL(STR1,'#') = '#' AND A.TYPE = 'CKJ' AND A.PSSID<>'00001'";
            }
            else
            {
                strSql = @"SELECT A.DEPTID,A.PSSID,SUM(JSJE) PRICE_USE,SUM(THJE) PRICE_RTN,SUM(JSJE+THJE) YJJJ,f_getdeptname(A.DEPTID) DEPTNAME,f_getsupname(A.PSSID) PSSNAME,F_GETTIME_JS(A.DEPTID,A.TYPE) KSTIME,'{0}' JSTIME
                        FROM VIEW_JS A
                        WHERE RQSJ < TO_DATE('{0}','yyyy-MM-dd')+1 AND NVL(STR1,'#') = '#' AND A.TYPE = 'XSJ' AND A.PSSID<>'00001'";
            }
            if (lstDEPTOUT.SelectedValue.Length > 0)
            { strSql += " AND A.DEPTID = '" + lstDEPTOUT.SelectedValue.ToString() + "' "; }
            if (ddlsup.SelectedValue.Length > 0)
            { strSql += " AND A.PSSID = '" + ddlsup.SelectedValue.ToString() + "' "; }
            strSql = string.Format(strSql, lstLRRQ2.Text);
            strSql += " GROUP BY A.DEPTID,A.PSSID,A.TYPE ORDER BY DEPTID";
            DataTable dtBill = new DataTable();
            dtBill = DbHelperOra.Query(strSql).Tables[0];
            GridList.DataSource = dtBill;
            GridList.DataBind();
            if (dtBill.Rows.Count > 0)
                Bill_create.Enabled = true;
            //计算合计数量
            decimal bzslTotal = 0, feTotal = 0, feeTotal = 0;
            foreach (DataRow row in dtBill.Rows)
            {
                bzslTotal += Convert.ToDecimal(row["PRICE_USE"]);
                feeTotal += Convert.ToDecimal(row["PRICE_RTN"]);
                feTotal += Convert.ToDecimal(row["YJJJ"]);
            }
            JObject summary = new JObject();
            summary.Add("DEPTNAME", "本页合计");
            summary.Add("PRICE_USE", bzslTotal.ToString("F2"));
            summary.Add("PRICE_RTN", feeTotal.ToString("F2"));
            summary.Add("YJJJ", feTotal.ToString("F2"));
            GridList.SummaryData = summary;
        }
        protected override void listRow_DoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {

            billOpen(GridList.DataKeys[e.RowIndex][0].ToString(), GridList.DataKeys[e.RowIndex][1].ToString(), GridList.DataKeys[e.RowIndex][2].ToString(), GridList.DataKeys[e.RowIndex][3].ToString());
        }

        protected void billOpen(string DEPTID, string Pssid, string datestart, string dateend)
        {
            //表头赋值
            dpkBEGRQ.SelectedDate = Convert.ToDateTime(datestart);
            ddlDEPTID.SelectedValue = DEPTID;
            dpkENDRQ.SelectedDate = Convert.ToDateTime(dateend);
            ddlSUPID.SelectedValue = Pssid;
            string strComSql = "";
            if (Request.QueryString["oper"].ToString() == "rk")
            {
                strComSql = @"SELECT A.*,f_getusername(NVL(B.LRY,C.LRY)) LRYNAME,f_getusername(NVL(B.SHR,C.SHR)) SHRNAME,NVL(B.LRRQ,C.LRRQ) LRRQ,NVL(B.SHRQ,C.SHRQ) SHRQ FROM
                            (SELECT A.BILLNO,A.BILLTYPE,F_GET_BILLTYPENAME(BILLTYPE) BILLTYPENAME,f_getdeptname(A.DEPTID) DEPTNAME,SUM(A.JSJE + A.THJE) JE,A.PSSID
                            FROM VIEW_JS A WHERE A.DEPTID = '{0}' AND A.RQSJ BETWEEN TO_DATE('{1}','yyyy-MM-dd') AND TO_DATE('{2}','yyyy-MM-dd') + 1 AND A.TYPE = 'RKJ' AND NVL(A.STR1,'#') = '#'
                            GROUP BY A.BILLNO,A.BILLTYPE,A.DEPTID,A.PSSID) A,DAT_RK_DOC B,DAT_TH_DOC C
                            WHERE A.BILLNO = B.SEQNO(+) AND A.BILLNO = C.SEQNO(+) AND A.PSSID = '{3}' ";
            }
            else if (Request.QueryString["oper"].ToString() == "ck")
            {
                strComSql = @"SELECT A.*,f_getusername(NVL(NVL(B.LRY,C.LRY),D.LRY)) LRYNAME,f_getusername(NVL(NVL(B.SHR,C.SHR),D.SHR)) SHRNAME,NVL(NVL(B.LRRQ,C.LRRQ),D.LRRQ) LRRQ,NVL(NVL(B.SHRQ,C.SHRQ),D.SHRQ) SHRQ FROM
                            (SELECT A.BILLNO,A.BILLTYPE,F_GET_BILLTYPENAME(BILLTYPE) BILLTYPENAME,f_getdeptname(A.DEPTID) DEPTNAME,SUM(A.JSJE + A.THJE) JE,A.PSSID
                            FROM VIEW_JS A WHERE A.DEPTID = '{0}' AND A.RQSJ BETWEEN TO_DATE('{1}','yyyy-MM-dd') AND TO_DATE('{2}','yyyy-MM-dd') + 1 AND A.TYPE = 'CKJ' AND NVL(A.STR1,'#') = '#'
                            GROUP BY A.BILLNO,A.BILLTYPE,A.DEPTID,A.PSSID) A,DAT_CK_DOC B,DAT_KD_DOC C,DAT_XS_DOC D
                            WHERE A.BILLNO = B.SEQNO(+) AND A.BILLNO = C.SEQNO(+)  AND A.BILLNO = D.SEQNO(+) AND A.PSSID = '{3}' ";
            }
            else
            {
                strComSql = @"SELECT A.*,f_getusername(B.LRY) LRYNAME,f_getusername(B.SHR) SHRNAME,B.LRRQ LRRQ,B.SHRQ SHRQ FROM
                            (SELECT A.BILLNO,A.BILLTYPE,F_GET_BILLTYPENAME(BILLTYPE) BILLTYPENAME,f_getdeptname(A.DEPTID) DEPTNAME,SUM(A.JSJE + A.THJE) JE,A.PSSID
                            FROM VIEW_JS A WHERE A.DEPTID = '{0}' AND A.RQSJ BETWEEN TO_DATE('{1}','yyyy-MM-dd') AND TO_DATE('{2}','yyyy-MM-dd') + 1  AND A.TYPE = 'XSJ' AND NVL(A.STR1,'#') = '#'
                            GROUP BY A.BILLNO,A.BILLTYPE,A.DEPTID,A.PSSID) A,DAT_XS_DOC B
                            WHERE A.BILLNO = B.SEQNO(+) AND A.PSSID = '{3}' ";
            }
            //表体赋值
            DataTable dtBill = DbHelperOra.Query(string.Format(strComSql, DEPTID, datestart, dateend, Pssid)).Tables[0];
            GridLis.DataSource = dtBill;
            GridLis.DataBind();
            //计算合计数量
            decimal bzslTotal = 0;
            foreach (DataRow row in dtBill.Rows)
            {
                bzslTotal += Convert.ToDecimal(row["JE"]);
            }
            JObject summary = new JObject();
            summary.Add("BILLNO", "本页合计");
            summary.Add("JE", bzslTotal.ToString("F2"));
            GridLis.SummaryData = summary;
            TabStrip1.ActiveTabIndex = 1;

        }
        protected void GridLis_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            string billno = GridLis.Rows[e.RowIndex].Values[1].ToString();
            string type = GridLis.DataKeys[e.RowIndex][1].ToString();
            string url = "";
            string jtype = "";
            if (Request.QueryString["oper"].ToString() == "rk")
            {
                jtype = "R";
            }
            else if (Request.QueryString["oper"].ToString() == "ck")
            {
                jtype = "C";
            }
            else
            {
                jtype = "X";
            }
            if (type == "CKD" || type == "DSC" || type == "DST" || type == "LCD" || type == "LTD")
            {
                url = "~/ERPPayment/Doc_CK_ComWindow.aspx?bm=" + billno + "&cx=&su=" + ddlSUPID.SelectedValue + "&tp=" + jtype;
                PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "出库信息:单号【" + billno + "】"));
            }
            if (type == "XSD" || type == "XST" || type == "DSH")
            {
                url = "~/ERPPayment/Doc_XS_ComWindow.aspx?bm=" + billno + "&cx=&su=" + ddlSUPID.SelectedValue + "&tp=" + jtype;
                PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "销售信息:单号【" + billno + "】"));
            }
            if (type == "RKD")
            {
                url = "~/ERPPayment/Doc_RK_ComWindow.aspx?bm=" + billno + "&cx=&su=" + ddlSUPID.SelectedValue + "&tp=" + jtype;
                PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "入库信息:单号【" + billno + "】"));
            }
            if (type == "KSD")
            {
                url = "~/ERPPayment/Doc_DB_ComWindow.aspx?bm=" + billno + "&cx=&su=" + ddlSUPID.SelectedValue + "&tp=" + jtype;
                PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "调拨信息:单号【" + billno + "】"));
            }
            if (type == "THD")
            {
                url = "~/ERPPayment/Doc_TH_ComWindow.aspx?bm=" + billno + "&cx=&su=" + ddlSUPID.SelectedValue + "&tp=" + jtype;
                PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "退货信息:单号【" + billno + "】"));
            }
        }

        protected void Bill_create_Click(object sender, EventArgs e)
        {
            if (Convert.ToDateTime(lstLRRQ2.Text) > DateTime.Today)
            {
                Alert.Show("结算日期必须小于当前日期!");
            }
            //勾选科室生成结算
            int[] selections = GridList.SelectedRowIndexArray;
            if (selections.Count() < 1)
            {
                Alert.Show("请选择需要结算的信息！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            string strSql = "";
            string seq = DbHelperOra.GetSingle("SELECT SEQ_PUBLIC.NEXTVAL FROM DUAL").ToString();
            List<CommandInfo> cmdList = new List<CommandInfo>();
            foreach (int rowIndex in selections)
            {
                if (Request.QueryString["oper"].ToString() == "rk")
                {
                    strSql = @"UPDATE DAT_GOODSJXC A SET A.STR1 = '{1}'
                        WHERE A.RQSJ < TO_DATE('{0}','yyyy-MM-dd')+1 AND NVL(A.STR1,'#') = '#' AND A.PSSID = '{2}' AND A.DEPTID = '{3}'
                         AND (A.GDSEQ,A.PSSID) IN(SELECT GDSEQ,NVL(PSSID,SUPID) FROM V_GOODSPS WHERE JSMODE = 'R')";
                }
                else if (Request.QueryString["oper"].ToString() == "ck")
                {
                    strSql = @"UPDATE DAT_GOODSJXC A SET A.STR1 = '{1}'
                        WHERE A.RQSJ < TO_DATE('{0}','yyyy-MM-dd')+1 AND NVL(A.STR1,'#') = '#' AND A.PSSID = '{2}' AND A.DEPTID = '{3}'
                         AND (A.GDSEQ,A.PSSID) IN(SELECT GDSEQ,NVL(PSSID,SUPID) FROM V_GOODSPS WHERE JSMODE = 'C')";
                }
                else
                {
                    strSql = @"UPDATE DAT_GOODSJXC A SET A.STR1 = '{1}'
                        WHERE A.RQSJ < TO_DATE('{0}','yyyy-MM-dd')+1 AND NVL(A.STR1,'#') = '#' AND A.PSSID = '{2}' AND A.DEPTID = '{3}'
                         AND (A.GDSEQ,A.PSSID) IN(SELECT GDSEQ,NVL(PSSID,SUPID) FROM V_GOODSPS WHERE JSMODE = 'X')";
                }
                string ss = String.Format(strSql, lstLRRQ2.Text, seq, GridList.DataKeys[rowIndex][1], GridList.DataKeys[rowIndex][0]);
                cmdList.Add(new CommandInfo(String.Format(strSql, lstLRRQ2.Text, seq, GridList.DataKeys[rowIndex][1], GridList.DataKeys[rowIndex][0]), null));
            }

            try
            {
                OracleParameter[] parameters = new OracleParameter[]
                        {
                             new OracleParameter("END_TIME",OracleDbType.Varchar2),
                             new OracleParameter("SEQ",OracleDbType.Varchar2),
                             new OracleParameter("USERID",OracleDbType.Varchar2),
                              new OracleParameter("V_MODE",OracleDbType.Varchar2)
                             
                        };
                parameters[0].Value = lstLRRQ2.Text;
                parameters[1].Value = seq;
                parameters[2].Value = UserAction.UserID;
                if (Request.QueryString["oper"].ToString() == "rk")
                {
                    parameters[3].Value = "RKJ";
                }
                else if (Request.QueryString["oper"].ToString() == "ck")
                {
                    parameters[3].Value = "CKJ";
                }
                else
                {
                    parameters[3].Value = "XSJ";
                }
                cmdList.Add(new CommandInfo("P_EXE_JSD", parameters, CommandType.StoredProcedure));
                DbHelperOra.ExecuteSqlTran(cmdList);
                Alert.Show("结算单生成成功!");
            }
            catch (Exception ex)
            {
                Alert.Show("结算失败：\n\r" + ex.Message + "", "错误信息", MessageBoxIcon.Question);
            }
            billSearch();
        }
    }
}