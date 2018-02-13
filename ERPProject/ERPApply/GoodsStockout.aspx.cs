﻿using FineUIPro;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using XTBase;
using XTBase.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPProject.ERPStorage
{
    public partial class GoodsStockout : BillBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindDDL();
                GoodsSearch();
            }
        }
        protected override void BindDDL()
        {
            string strpssid = @"SELECT CODE,NAME FROM(
                        SELECT '' CODE, '--请选择--' NAME
                          FROM DUAL
                        UNION
                        SELECT SUPID CODE, SUPNAME NAME
                          FROM DOC_SUPPLIER
                         WHERE ISPSS = 'Y'
                        UNION
                        SELECT DS.SUPID CODE, DS.SUPNAME NAME
                          FROM DOC_GOODSSUP DG, DOC_SUPPLIER DS
                         WHERE DG.SUPID = DS.SUPID(+)
                           AND DG.TYPE = 'Z'
                        ) ORDER BY CODE DESC";
            PubFunc.DdlDataGet("DDL_DOC_SUPPLIERALL", SUPNAME);
            PubFunc.DdlDataSql(lstPSSID, strpssid);
            PubFunc.DdlDataSql(docSUPID, strpssid);
            PubFunc.DdlDataGet("DDL_SYS_DEPTDEF", lstDEPTID, docDEPTID);
            PubFunc.DdlDataGet("DDL_SYS_DEPT", lstDEPTID);
            PubFunc.DdlDataGet("DDL_SYS_DEPOTRANGE", UserAction.UserID, ddlDEPTOUT);
            ddlDEPTOUT.SelectedIndex = 1;
            btnAudit.Enabled = isOrder(ddlDEPTOUT.SelectedValue);
            btnDb.Enabled = !btnAudit.Enabled;
            //PubFunc.DdlDataGet("DDL_STUTEQHM", lstFLAG);
            PubFunc.DdlDataGet("DDL_USER", lstLRY);
            PubFunc.DdlDataGet("DDL_GOODS_TYPE", docCatid);
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-7);
            lstLRRQ2.SelectedDate = DateTime.Now;
            docDATE_SL1.SelectedDate = DateTime.Now.AddDays(-7);
            docDATE_SL2.SelectedDate = DateTime.Now;

            hfdDeptCode.Text = DbHelperOra.GetSingle("SELECT VALUE FROM SYS_PARA WHERE CODE='DEFDEPT'").ToString();
        }
        protected bool isOrder(String deptout)
        {
            if (DbHelperOra.Exists("SELECT 1 FROM SYS_DEPT A WHERE A.ISORDER = 'Y' AND CODE = '" + deptout + "'"))
            {
                return true;
            }
            return false;
        }
        private void GoodsSearch()
        {
            string strSql = @"SELECT A.*,
                                     NVL(B.HISNAME,B.GDNAME) HISNAME,
                                     F_GETUNITNAME(A.UNIT) UNITNAME,
                                     F_GETBILLFLAG(A.FLAG) FLAG_CN,
                                     F_GETDEPTNAME(A.DEPTID) DEPTNAME,
                                     F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,
                                     F_GETSUPNAME(F_GETSHSID(a.gdseq)) PSSNAME,
                                     F_GETUSERNAME(A.OPERUSER) OPERUSER_CN,
                                     DECODE(B.ISFLAG3 ,'Y','true','false') ISFLAG3,
                                     DECODE(B.ISFLAG7 ,'Y','true','false') ISFLAG7,
                                     A.MEMO QHMEMO,
                                     DECODE(B.ISGZ,'Y','true','false') ISGZ,F_GETCATID0NAME(B.CATID0) CATID0NAME,CATID0,
                                     F_GETUNITNAME(DECODE(B.UNIT_SELL,'X',B.UNIT,'Z',B.UNIT_ZHONGBZ,'D',B.UNIT_DABZ))BZUNITNAME,
                                     ROUND(A.SLSL/DECODE(B.UNIT_SELL,'X',B.BZHL,'Z',B.NUM_ZHONGBZ,'D',B.NUM_DABZ),2)BZSLSL,
                                     ROUND(A.QHSL/DECODE(B.UNIT_SELL,'X',B.BZHL,'Z',B.NUM_ZHONGBZ,'D',B.NUM_DABZ),2)BZQHSL
                                FROM DAT_NOSTOCK_LIST A, DOC_GOODS B
                               WHERE A.GDSEQ = B.GDSEQ(+)   AND A.FLAG = 'N' AND B.FLAG IN('Y','T') ";
            string strSearch = "";

            if (!string.IsNullOrWhiteSpace(docISGZ.SelectedValue))
            {
                strSearch += string.Format(" AND B.ISGZ = '{0}'", docISGZ.SelectedValue);
            }
            if (!string.IsNullOrWhiteSpace(docISFLAG3.SelectedValue))
            {
                strSearch += string.Format(" AND B.ISFLAG3 = '{0}'", docISFLAG3.SelectedValue);
            }

            if (!string.IsNullOrWhiteSpace(docCatid.SelectedValue))
            {
                strSearch += string.Format(" AND B.CATID0 = '{0}'", docCatid.SelectedValue);
            }
            if (ddlISFLAG7.SelectedValue.Length > 0)
            {
                strSearch += string.Format(" AND B.ISFLAG7 = '{0}'", ddlISFLAG7.SelectedValue);
            }
            if (!string.IsNullOrWhiteSpace(docBILLNO.Text.Trim()))
            {
                strSearch += string.Format(" AND A.BILLNO_SL  LIKE '%{0}%'", docBILLNO.Text);
            }
            if (!string.IsNullOrWhiteSpace(docSearch.Text.Trim()))
            {
                strSearch += string.Format(" AND (A.GDSEQ LIKE '%{0}%' OR A.GDNAME LIKE '%{0}%')", docSearch.Text.Trim());
            }
            if (docSUPID.SelectedItem != null && docSUPID.SelectedItem.Value.Length > 0)
            {
                // strSearch += string.Format(" AND F_GETSHSID(a.gdseq)='{0}'", docSUPID.SelectedItem.Value);
                strSearch += string.Format(" AND A.PSSID='{0}'", docSUPID.SelectedItem.Value);

            }
            if (docDEPTID.SelectedItem != null && docDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", docDEPTID.SelectedItem.Value);
            }
            if (ddlDEPTOUT.SelectedItem != null && ddlDEPTOUT.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTOUT='{0}'", ddlDEPTOUT.SelectedItem.Value);
            }
            if (docDATE_SL1.Text.Length > 0)
            {
                strSearch += string.Format(" AND A.DATE_SL>=TO_DATE('{0}','YYYY-MM-DD')", docDATE_SL1.Text);
            }

            if (docDATE_SL2.Text.Length > 0)
            {
                strSearch += string.Format(" AND A.DATE_SL <TO_DATE('{0}','YYYY-MM-DD') + 1", docDATE_SL2.Text);
            }

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY A.BILLNO_SL DESC";

            int total = 0;
            DataTable dtBill = PubFunc.DbGetPage(GridCom.PageIndex, GridCom.PageSize, strSql, ref total);
            GridCom.DataSource = dtBill;
            GridCom.RecordCount = total;
            GridCom.DataBind();

            decimal bzslTotal = 0, feeTotal = 0;
            if (dtBill != null && dtBill.Rows.Count > 0)
            {
                foreach (DataRow row in dtBill.Rows)
                {
                    bzslTotal += Convert.ToDecimal(row["BZSL"]);
                    feeTotal += Convert.ToDecimal(row["QHSL"]);
                }
            }

            //if (Request.QueryString["tp"] != null && Request.QueryString["tp"].ToString().Trim().Length > 0)
            //{
            //    ddlDEPTOUT.SelectedValue = DbHelperOra.GetSingle("SELECT T.STR2 FROM DOC_GOODSTYPE T WHERE T.CODE=" + Request.QueryString["tp"].ToString()).ToString();
            //}
            //else
            //{
            //    ddlDEPTOUT.SelectedValue = DbHelperOra.GetSingle("SELECT T.STR2 FROM DOC_GOODSTYPE T WHERE T.CODE='2'").ToString();
            //}
            //计算合计数量
            JObject summary = new JObject();
            summary.Add("PSSID", "本页合计");
            summary.Add("BZSL", bzslTotal.ToString());
            summary.Add("QHSL", feeTotal.ToString("F2"));
            GridCom.SummaryData = summary;
        }

        private void BillSearch()
        {
            string strSql = @"SELECT A.*,nvl(b.HISNAME,a.gdname) hisname, f_getunitname(A.UNIT) unitName,DECODE(A.FLAG,'Y','已订货','G','已完成','C','已取消','未定义') FLAG_CN,F_GETDEPTNAME(A.DEPTID) DEPTNAME,F_GETSUPNAME(A.PSSID) PSSNAME,F_GETUSERNAME(A.OPERUSER) OPERUSER_CN,F_GETUSERNAME(A.SLR)SLY，
                                          F_GETUNITNAME(DECODE(B.UNIT_SELL,'X',B.UNIT,'Z',B.UNIT_ZHONGBZ,'D',B.UNIT_DABZ))BZUNITNAME,
                                          ROUND(A.SLSL/DECODE(B.UNIT_SELL,'X',B.BZHL,'Z',B.NUM_ZHONGBZ,'D',B.NUM_DABZ),2)BZSLSL,
                                          ROUND(A.QHSL/DECODE(B.UNIT_SELL,'X',B.BZHL,'Z',B.NUM_ZHONGBZ,'D',B.NUM_DABZ),2)BZQHSL
                                          FROM DAT_NOSTOCK_LIST A,DOC_GOODS B WHERE A.GDSEQ = B.GDSEQ AND A.FLAG IN ('Y','G','C')  AND QHTYPE <> 'A'";
            string strSearch = "";

            if (lstBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND A.BILLNO_SL  LIKE '%{0}%'", lstBILLNO.Text);
            }

            if (lstFLAG.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.FLAG='{0}'", lstFLAG.SelectedItem.Value);
            }
            if (lstLRY.SelectedItem != null && lstLRY.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.SLR='{0}'", lstLRY.SelectedItem.Value);
            }
            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedItem.Value);
            }
            if (lstPSSID.SelectedValue.Length > 0)
            {
                strSearch += string.Format(" AND A.PSSID='{0}'", lstPSSID.SelectedItem.Value);
            }
            if (tgbGoods.Text.Trim().Length > 0)
            {
                strSearch += string.Format(" AND (B.GDSEQ LIKE '%{0}%' OR B.GDNAME LIKE '%{0}%' OR B.ZJM LIKE '%{0}%' OR B.BARCODE LIKE '%{0}%' OR B.HISCODE LIKE '%{0}%' OR B.HISNAME LIKE '%{0}%' OR B.STR4 LIKE '%{0}%')", tgbGoods.Text.Trim().ToUpper());
            }

            strSearch += string.Format(" AND A.DATE_SL>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.DATE_SL <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY A.DATE_SL DESC";

            int total = 0;
            DataTable dt = PubFunc.DbGetPage(GridList.PageIndex, GridList.PageSize, strSql, ref total);

            GridList.DataSource = dt;
            GridList.RecordCount = total;
            GridList.DataBind();
        }

        protected void bntSearch_Click(object sender, EventArgs e)
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【录入日期】！", MessageBoxIcon.Warning);
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                Alert.Show("【开始日期】大于【结束日期】，请重新输入！", MessageBoxIcon.Warning);
                return;
            }

            BillSearch();
        }

        protected void btnGoodsSearch_Click(object sender, EventArgs e)
        {
            if (ddlDEPTOUT.SelectedValue.Length < 1)
            {

                Alert.Show("请选择【缺货库房】！", MessageBoxIcon.Warning);
                return;
            }
            GoodsSearch();

        }
        protected void btnAudit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(hfdDeptCode.Text))
            {
                Alert.Show("未设置默认订货部门，请联系系统管理员！！！", "消息提示", MessageBoxIcon.Question);
                return;
            }
            if (ddlDEPTOUT.SelectedValue.Length < 1)
            {
                Alert.Show("请选择【缺货库房】信息", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (GridCom.SelectedRowIndexArray.Length == 0)
            {
                Alert.Show("请选择要转订单的单据信息", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            string seq = DbHelperOra.GetSingle("SELECT SEQ_PUBLIC.NEXTVAL FROM DUAL").ToString();//缺货审核SEQ
            List<CommandInfo> cmdList = new List<CommandInfo>();
            string strSql = @"UPDATE DAT_NOSTOCK_LIST  SET OPERUSER ='" + UserAction.UserID + @"'  ,
                                    SEQNO ='" + seq + @"'
                              where BILLNO_SL='{0}' and ROWNO='{1}' ";
            foreach (int i in GridCom.SelectedRowIndexArray)
            {
                cmdList.Add(new CommandInfo(string.Format(strSql, GridCom.Rows[i].Values[2].ToString(), GridCom.Rows[i].Values[3].ToString()), null));
                string ss = GridCom.Rows[i].Values[8].ToString();
                if (!DbHelperOra.GetSingle("SELECT FLAG FROM DOC_GOODS WHERE GDSEQ='" + GridCom.Rows[i].Values[8].ToString() + "'").ToString().Equals("Y"))
                {
                    Alert.Show("商品【" + GridCom.Rows[i].Values[9].ToString() + "】状态不正确");
                    return;
                }

            }

            //调用存储过程
            OracleParameter[] parameters = {
                                               new OracleParameter("VI_SEQ", OracleDbType.Int32),
                                               new OracleParameter("VI_USER", OracleDbType.Varchar2),
                                               new OracleParameter("VI_MSG", OracleDbType.Varchar2),
                                               new OracleParameter("VI_BILLNO", OracleDbType.Varchar2),
                                               new OracleParameter("VI_DEPTID", OracleDbType.Varchar2)
                                           };
            parameters[0].Value = seq;
            parameters[1].Value = UserAction.UserID;
            parameters[2].Value = "缺货转订单";
            parameters[3].Value = "";
            parameters[4].Value = "";
            cmdList.Add(new CommandInfo("STORE.P_NOSTOCK_OK", parameters, CommandType.StoredProcedure));
            try
            {
                if (DbHelperOra.ExecuteSqlTran(cmdList))
                {
                    Alert.Show("缺货转订单生成成功！", "消息提示", MessageBoxIcon.Information);
                    GoodsSearch();
                }
            }
            catch (Exception ex)
            {
                Alert.Show(Error_Parse(ex.Message), "提示信息", MessageBoxIcon.Warning);
                return;
            }


        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (GridCom.SelectedRowIndexArray.Length == 0)
            {
                Alert.Show("请选择要作废的单据信息", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            List<CommandInfo> cmdList = new List<CommandInfo>();
            OracleParameter[] parameters = {
                                               new OracleParameter("VI_BILLNO", OracleDbType.Varchar2,20),
                                               new OracleParameter("VI_ROWNO", OracleDbType.Int16) };
            foreach (int index in GridCom.SelectedRowIndexArray)
            {
                cmdList.Add(new CommandInfo(string.Format("UPDATE DAT_NOSTOCK_LIST SET FLAG='C',OPERUSER='{0}' WHERE BILLNO_SL='{1}' AND ROWNO={2}", UserAction.UserName, GridCom.Rows[index].Values[2], GridCom.Rows[index].Values[3]), null));
                //不存在预占不需要执行
                if (DbHelperOra.Exists(String.Format(@"SELECT *
                                    FROM DAT_STOCKLOCK A
                                    WHERE A.LOCKBILLNO = '{0}' AND A.LOCKROWNO = {1} AND A.LOCKFLAG = 'Y'", GridCom.Rows[index].Values[2], GridCom.Rows[index].Values[3])))
                {
                    parameters[0].Value = GridCom.Rows[index].Values[2];
                    parameters[1].Value = GridCom.Rows[index].Values[3];
                    parameters[0].Direction = ParameterDirection.Input;
                    parameters[1].Direction = ParameterDirection.Input;
                    cmdList.Add(new CommandInfo("STOREDS.P_NOSTOCK_CANL", parameters, CommandType.StoredProcedure));
                }
            }
            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                GoodsSearch();
            }
            else
            {
                Alert.Show("单据操作失败，请检查单据信息是否正确！", "警告提示", MessageBoxIcon.Error);
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
        protected void GridCom_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridCom.PageIndex = e.NewPageIndex;
            GoodsSearch();
        }

        protected void GridList_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridList.PageIndex = e.NewPageIndex;
            BillSearch();
        }

        protected void bntClear_Click(object sender, EventArgs e)
        {
            PubFunc.FormDataClear(Formlist);
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-7);
            lstLRRQ2.SelectedDate = DateTime.Now;
            docDATE_SL1.SelectedDate = DateTime.Now.AddDays(-7);
            docDATE_SL2.SelectedDate = DateTime.Now;
        }
        /// <summary>
        ///  修改供应商信息
        ///  supName
        ///  supID
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSup_Click(object sender, EventArgs e)
        {
            if (GridCom.SelectedRowIndexArray.Length == 0)
            {
                Alert.Show("请选择要修改供应商的订单", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            //验证是否有威高商业集团
            foreach (int i in GridCom.SelectedRowIndexArray)
            {
                if (GridCom.Rows[i].Values[12].ToString().Equals("威高医疗商业集团"))
                {
                    Alert.Show("威高医疗商业集团不能修改", "消息提示", MessageBoxIcon.Warning);
                    return;
                }
            }
            WindowPH.Hidden = false;
        }
        /// <summary>
        /// 窗口关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void bntSave(object sender, EventArgs e)
        {
            //验证是否选择
            if (!string.IsNullOrWhiteSpace(SUPNAME.SelectedValue))
            {


                //判断该商品是否属于该供应商
                string strSql = @"update DAT_NOSTOCK_LIST 
                                set pssid ='{2}'
                              where BILLNO_SL='{0}' and ROWNO='{1}' ";
                List<CommandInfo> cmdList = new List<CommandInfo>();
                foreach (int i in GridCom.SelectedRowIndexArray)
                {
                    string sql = "SELECT 1 FROM DOC_GOODSSUP where supid='{0}' and gdseq='{1}'";//查询供应商商品配置表
                    string.Format(sql, SUPNAME.SelectedValue, GridCom.Rows[i].Values[6].ToString(), null);
                    if (DbHelperOra.Exists(string.Format(sql, SUPNAME.SelectedValue, GridCom.Rows[i].Values[6].ToString(), null)))
                    {
                        Alert.Show("【" + GridCom.Rows[i].Values[7] + "】没有配置改供应商");
                        return;

                    }
                    cmdList.Add(new CommandInfo(string.Format(strSql, GridCom.Rows[i].Values[2].ToString(), GridCom.Rows[i].Values[3].ToString(), SUPNAME.SelectedValue), null));
                }
                DbHelperOra.ExecuteSqlTran(cmdList);
                GoodsSearch();
                WindowPH.Hidden = true;
                // Alert.Show("修改成功", "消息提示", MessageBoxIcon.Warning);
            }
            else
            {
                Alert.Show("请选择供应商", "消息提示", MessageBoxIcon.Warning);
            }
        }
        public string billNo { get; set; }

        protected void GridCom_Sort(object sender, GridSortEventArgs e)
        {
            GridCom.SortDirection = e.SortDirection;
            GridCom.SortField = e.SortField;

            DataTable table = PubFunc.GridDataGet(GridCom);
            DataView view1 = table.DefaultView;
            view1.Sort = String.Format("{0} {1}", GridCom.SortField, GridCom.SortDirection);
            GridCom.DataSource = view1;
            GridCom.DataBind();
        }

        protected void btExport_Click(object sender, EventArgs e)
        {
            string strSql = @"SELECT F_GETBILLFLAG(A.FLAG) 当前状态,
                                        A.BILLNO_SL 申领单号,
                                        A.DATE_SL 申领日期,
                                        A.DEPTID 申领部门编码,
                                        F_GETDEPTNAME(A.DEPTOUT) 缺货库房,
                                        F_GETDEPTNAME(A.DEPTID) 申领部门,
                                        ' '||A.GDSEQ 商品编码,
                                        B.HISNAME 商品名称,
                                        F_GETCATID0NAME(CATID0) 商品类别,
                                        A.GDSPEC 商品规格,
                                        F_GETUNITNAME(A.UNIT) 单位,
                                        A.SLSL 申领数量,
                                        A.QHSL 缺货数,
                                        A.PSSID 配送商编码,
                                        F_GETSUPNAME(F_GETSHSID(a.gdseq)) 送货商名称,
                                        A.BZSL 申领数,
                                        DECODE(B.ISGZ,'Y','高值商品','非高值') 高值商品
                                FROM DAT_NOSTOCK_LIST A, DOC_GOODS B
                               WHERE A.GDSEQ = B.GDSEQ(+)
                                 AND A.FLAG = 'N' ";
            string strSearch = "";

            if (!string.IsNullOrWhiteSpace(docISGZ.SelectedValue))
            {
                strSearch += string.Format(" AND B.ISGZ = '{0}'", docISGZ.SelectedValue);
            }
            if (!string.IsNullOrWhiteSpace(docISFLAG3.SelectedValue))
            {
                strSearch += string.Format(" AND B.ISFLAG3 = '{0}'", docISFLAG3.SelectedValue);
            }

            if (!string.IsNullOrWhiteSpace(docCatid.SelectedValue))
            {
                strSearch += string.Format(" AND B.CATID0 = '{0}'", docCatid.SelectedValue);
            }

            if (!string.IsNullOrWhiteSpace(docBILLNO.Text.Trim()))
            {
                strSearch += string.Format(" AND A.BILLNO_SL  LIKE '%{0}%'", docBILLNO.Text);
            }
            if (!string.IsNullOrWhiteSpace(docSearch.Text.Trim()))
            {
                strSearch += string.Format(" AND (A.GDSEQ LIKE '%{0}%' OR A.GDNAME LIKE '%{0}%')", docSearch.Text.Trim());
            }
            if (docSUPID.SelectedItem != null && docSUPID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND F_GETSHSID(a.gdseq)='{0}'", docSUPID.SelectedItem.Value);
            }
            if (docDEPTID.SelectedItem != null && docDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", docDEPTID.SelectedItem.Value);
            }
            if (ddlDEPTOUT.SelectedItem != null && ddlDEPTOUT.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTOUT='{0}'", ddlDEPTOUT.SelectedItem.Value);
            }
            if (docDATE_SL1.Text.Length > 0)
            {
                strSearch += string.Format(" AND A.DATE_SL>=TO_DATE('{0}','YYYY-MM-DD')", docDATE_SL1.Text);
            }

            if (docDATE_SL2.Text.Length > 0)
            {
                strSearch += string.Format(" AND A.DATE_SL <TO_DATE('{0}','YYYY-MM-DD') + 1", docDATE_SL2.Text);
            }

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY A.BILLNO_SL DESC";
            DataTable dt = DbHelperOra.Query(strSql).Tables[0];
            String title = docDEPTID.SelectedText;
            if (docDEPTID.SelectedValue.Length < 1)
            {
                title = "全部库房";
            }
            ExcelHelper.ExportByWeb(dt, string.Format("【{0}】商品缺货管理", title), "商品缺货管理_" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
        }

        protected void GridCom_RowClick(object sender, GridRowClickEventArgs e)
        {
            if (GridCom.SelectedRowIDArray.ToList().Contains(e.RowID))
            {
                string strGDSEQ = GridCom.Rows[e.RowIndex].DataKeys[0].ToString();
                object obj = DbHelperOra.GetSingle(string.Format("SELECT ISFLAG7 FROM DOC_GOODS WHERE GDSEQ = '{0}' AND FLAG = 'Y'", strGDSEQ));
                if ((obj ?? "N").ToString() == "Y")
                {
                    Alert.Show("当前选择商品为【本地新增商品】，执行订单生成后会被过滤!", "消息提示", MessageBoxIcon.Warning);
                }
            }
        }

        protected void btnDb_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(hfdDeptCode.Text))
            {
                Alert.Show("未设置默认调拨部门，请联系系统管理员！！！", "消息提示", MessageBoxIcon.Question);
                return;
            }
            if (ddlDEPTOUT.SelectedValue.Length < 1)
            {
                Alert.Show("请选择【缺货库房】信息", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (GridCom.SelectedRowIndexArray.Length < 1)
            {
                Alert.Show("请选择要转调拨的单据信息!", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (DbHelperOra.Exists("SELECT 1 FROM SYS_DEPT A WHERE A.CODE = NVL(A.STOCK,'#') AND A.CODE = '" + ddlDEPTOUT.SelectedValue + "'"))
            {
                Alert.Show("【管理架构定义中】默认库房与选择缺货库房相同，不能进行调拨！", MessageBoxIcon.Warning);
                return;
            }

            string seq = DbHelperOra.GetSingle("SELECT SEQ_PUBLIC.NEXTVAL FROM DUAL").ToString();//缺货审核SEQ
            List<CommandInfo> cmdList = new List<CommandInfo>();
            string strSql = @"UPDATE DAT_NOSTOCK_LIST  set OPERUSER ='" + UserAction.UserID + @"',SEQNO ='" + seq + @"'
                              WHERE BILLNO_SL='{0}' and ROWNO={1} ";
            foreach (int i in GridCom.SelectedRowIndexArray)
            {
                cmdList.Add(new CommandInfo(string.Format(strSql, GridCom.Rows[i].Values[2].ToString(), GridCom.Rows[i].Values[3].ToString()), null));
            }

            //调用存储过程
            OracleParameter[] parameters = {
                                               new OracleParameter("VI_SEQ", OracleDbType.Int32),
                                               new OracleParameter("VI_USER", OracleDbType.Varchar2,20),
                                               new OracleParameter("VI_DEPTIN", OracleDbType.Varchar2,10),
                                               new OracleParameter("VO_BILL", OracleDbType.Varchar2,20)};
            parameters[0].Value = seq;
            parameters[1].Value = UserAction.UserID;
            parameters[2].Value = ddlDEPTOUT.SelectedValue;
            parameters[0].Direction = ParameterDirection.Input;
            parameters[1].Direction = ParameterDirection.Input;
            parameters[2].Direction = ParameterDirection.Input;
            parameters[3].Direction = ParameterDirection.Output;
            cmdList.Add(new CommandInfo("STOREDS.P_NOSTOCK_DB", parameters, CommandType.StoredProcedure));
            try
            {
                if (DbHelperOra.ExecuteSqlTran(cmdList))
                {
                    if (parameters[3].Value.ToString().Length < 5)
                    {
                        Alert.Show("选择的所有商品，都是高值商品，无法生成调拨单", MessageBoxIcon.Warning);
                        return;
                    }
                    Alert.Show("缺货转调拨单生成成功，单据编号【" + parameters[3].Value + "】！", "消息提示", MessageBoxIcon.Information);
                    GoodsSearch();
                }
            }
            catch (Exception ex)
            {
                Alert.Show(ex.Message, "提示信息", MessageBoxIcon.Warning);
                return;
            }
        }

        protected void ddlDEPTOUT_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnAudit.Enabled = isOrder(ddlDEPTOUT.SelectedValue);
            btnDb.Enabled = !btnAudit.Enabled;
            btnGoodsSearch_Click(null, null);
        }
    }
}