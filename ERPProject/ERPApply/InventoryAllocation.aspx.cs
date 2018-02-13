using FineUIPro;
using Oracle.ManagedDataAccess.Client;
using XTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ERPProject.ERPApply
{
    public partial class InventoryAllocation : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
                if (Request.QueryString["pid"] != null)
                {
                    string strSeqNo = Request.QueryString["pid"].ToString();
                    object obj = DbHelperOra.GetSingle(string.Format("SELECT DEPTOUT FROM DAT_SL_DOC WHERE SEQNO = '{0}'", strSeqNo));
                    if (!string.IsNullOrWhiteSpace(obj.ToString()))
                    {
                        lstDEPTOUT.SelectedValue = obj.ToString();
                        lstBILLNO.Text = strSeqNo;
                    }
                    else
                    {
                        if (Request.QueryString["tp"] != null && Request.QueryString["tp"].ToString().Trim().Length > 0)
                        {
                            lstDEPTOUT.SelectedValue = DbHelperOra.GetSingle("SELECT T.STR2 FROM DOC_GOODSTYPE T WHERE T.CODE=" + Request.QueryString["tp"].ToString()).ToString();
                        }
                        else
                        {
                            lstDEPTOUT.SelectedValue = DbHelperOra.GetSingle("SELECT T.STR2 FROM DOC_GOODSTYPE T WHERE T.CODE='2'").ToString();
                        }
                    }
                }

                if (lstDEPTOUT.SelectedValue.Length > 0)
                {
                    object obj = DbHelperOra.GetSingle("SELECT NUM1 FROM V_KCSP WHERE FLAG = 'Y' AND DEPTOUT = '" + lstDEPTOUT.SelectedValue + "' AND ROWNUM = '1'");
                    if ((obj ?? "").ToString().Length > 0)
                    {
                        Alert.Show("当前仓库存在未手工确认出库的波次，请及时处理！", "提示信息", MessageBoxIcon.Information);
                        TabStrip1.ActiveTabIndex = 1;
                        hfdBCode.Text = obj.ToString();
                        HDataSearch();
                    }
                }
                if (GridGoods.Rows.Count < 1)
                {
                    DataSearch();
                }
            }
        }

        private void DataInit()
        {
            PubFunc.DdlDataGet("DDL_SYS_DEPOTRANGE", UserAction.UserID, lstDEPTOUT);
            PubFunc.DdlDataGet("DDL_SYS_DEPTRANGE", UserAction.UserID, lstDEPTID);
            PubFunc.DdlDataGet(lstLX, "DAT_LX");
            PubFunc.DdlDataGet(ddlHOUSE, "DAT_LOUDONG");
            PubFunc.DdlDataGet(ddlFLOOR, "DDL_LOUCENG");
            dpktime1.SelectedDate = DateTime.Now.AddDays(-7);
            dpktime2.SelectedDate = DateTime.Now;
            if (UserAction.UserRole == "03")
            {
                lstDEPTOUT.SelectedValue = UserAction.UserDept;
            }
            else
            {
                lstDEPTOUT.SelectedIndex = 1;
            }

        }
        protected void GridGoods_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridGoods.PageIndex = e.NewPageIndex;
            HDataSearch();
        }
        protected void bntSearch_Click(object sender, EventArgs e)
        {
            DataSearch();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            DataSearch();
        }
        private void DataSearch()
        {
            string strSql = string.Format(@"SELECT DISTINCT A.SEQNO,A.SUBNUM,A.SHRQ, f_getdeptname(A.DEPTOUT) DEPTOUTNAME,f_getdeptname(A.DEPTID) DEPTIDNAME
                       ,f_getusername(A.LRY) LRYNAME,f_getusername(A.SHR) SHRNAME,DECODE(FPTYPE,'1','定数申领','非定数申领') FPTYPENAME,NVL(A.STR3,'未分配') STR3,
                        DECODE(A.BILLFLAG,'S','未分配','N','未分配','D','调拨中','W','调拨完成','未定义') FLAGNAME
                FROM V_KCSP A WHERE A.FLAG = 'N' AND A.SHRQ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd') + 1", dpktime1.Text, dpktime2.Text);

            if (lstBILLNO.Text.Trim().Length > 0)
            {
                strSql = strSql + " AND A.SEQNO LIKE '%" + lstBILLNO.Text.Trim() + "%'";
            }
            if (lstDEPTOUT.SelectedValue.Length > 0)
            {
                strSql = strSql + " AND A.DEPTOUT = '" + lstDEPTOUT.SelectedValue + "'";
            }
            else
            {
                Alert.Show("请选择【出库库房】！", "提示信息", MessageBoxIcon.Warning);
                GridList.DataSource = null;
                GridList.DataBind();
                return;
            }
            if (lstLX.SelectedValue != null && lstLX.SelectedValue.Length > 0)
            {
                strSql = strSql + " AND EXISTS(SELECT 1 FROM SYS_DEPT B WHERE B.STR1 =  '" + ddlTYPEFP.SelectedValue + "' AND B.CODE = A.DEPTID)";
            }
            if (ddlHOUSE.SelectedValue != null && ddlHOUSE.SelectedValue.Length > 0)
            {
                strSql = strSql + " AND A.DEPTID IN (SELECT CODE FROM SYS_DEPT WHERE  HOUSE='" + ddlHOUSE.SelectedValue + "')";
            }
            if (ddlFLOOR.SelectedValue != null && ddlFLOOR.SelectedValue.Length > 0)
            {
                strSql = strSql + " AND A.DEPTID IN (SELECT CODE FROM SYS_DEPT WHERE  FLOOR='" + ddlFLOOR.SelectedValue + "')";
            }
            if (lstGoods.Text.Trim().Length > 0)
            {
                strSql = strSql + String.Format(" AND EXISTS(SELECT 1 FROM DOC_GOODS B WHERE (B.GDSEQ LIKE  '%{0}%' OR B.GDNAME LIKE '%{0}%' OR B.ZJM LIKE '%{0}%') AND B.GDSEQ = A.GDSEQ)", lstGoods.Text.Trim().ToUpper());
            }

            if (ddlTYPEFP.SelectedValue.Length > 0)
            {
                strSql = strSql + " AND A.FPTYPE = '" + ddlTYPEFP.SelectedValue + "'";
            }
            if (lstDEPTID.SelectedValue.Length > 0)
            {
                strSql = strSql + " AND A.DEPTID = '" + lstDEPTID.SelectedValue + "'";
            }
            if (lstDEPTID.SelectedValue.Length > 0)
            {
                strSql = strSql + " AND A.DEPTID = '" + lstDEPTID.SelectedValue + "'";
            }
            DataTable dt = DbHelperOra.Query(strSql + " ORDER BY SHRQ DESC").Tables[0];
            GridList.DataSource = dt;
            GridList.DataBind();
            List<int> list = new List<int>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                list.Add(i);
            }
            GridList.SelectedRowIndexArray = list.ToArray();
        }
        protected void PageManager1_CustomEvent(object sender, CustomEventArgs e)
        {
            switch (e.EventArgument)
            {
                case "Confirm_Fp":
                    try
                    {
                        string strBill = "";
                        int[] index = GridList.SelectedRowIndexArray;
                        foreach (int i in index)
                        {
                            strBill = strBill + "'" + GridList.DataKeys[i][0] + "',";
                        }
                        string seq = DbHelperOra.GetSingle("SELECT SEQ_FPKC.NEXTVAL FROM DUAL").ToString();
                        hfdBCode.Text = seq;
                        List<CommandInfo> cmdList = new List<CommandInfo>();
                        cmdList.Add(new CommandInfo("UPDATE DAT_SL_DOC SET NUM1 = " + seq + " WHERE SEQNO IN (" + strBill.TrimEnd(',') + ")", null));
                        cmdList.Add(new CommandInfo("UPDATE DAT_CK_DOC SET NUM1 = " + seq + " WHERE SEQNO IN (" + strBill.TrimEnd(',') + ")", null));
                        OracleParameter[] parameters = {
                                               new OracleParameter("VIN_BILLNO", OracleDbType.Varchar2,20),
                                               new OracleParameter("VIN_OPERUSER", OracleDbType.Varchar2,800),
                                               new OracleParameter("VO_QHNUM",OracleDbType.Decimal)};
                        parameters[0].Value = hfdBCode.Text;
                        parameters[1].Value = UserAction.UserID;
                        parameters[0].Direction = ParameterDirection.Input;
                        parameters[1].Direction = ParameterDirection.Input;
                        parameters[2].Direction = ParameterDirection.Output;
                        cmdList.Add(new CommandInfo("STOREDS.P_FP_AUTO", parameters, CommandType.StoredProcedure));
                        DbHelperOra.ExecuteSqlTran(cmdList);
                        if (Convert.ToDecimal(parameters[2].Value.ToString()) > 0)
                        {
                            Alert.Show("波次[" + seq + "]自动分配成功！\n\r其中缺货明细[" + parameters[2].Value + "]条！");
                        }
                        else
                        {
                            Alert.Show("波次[" + seq + "]自动分配成功！");
                        }
                        OperLog("库存分配", "分配波次【" + seq + "】");
                        DataSearch();
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.IndexOf("ORA-20001") > -1)
                        {
                            string msg = ex.Message.Substring(0, ex.Message.IndexOf("\n"));
                            Alert.Show("自动分配失败,错误信息：\n\r\n\r                                  " + msg.Substring(msg.IndexOf("ORA-20001") + 10), "错误信息", MessageBoxIcon.Warning);
                        }
                        else
                        {
                            Alert.Show("自动分配失败：\n\r\n\r                                  " + ex.Message + "", "错误信息", MessageBoxIcon.Question);
                        }
                    }
                    break;
            }
        }
        protected void btnAuto_Click(object sender, EventArgs e)
        {
            if (lstDEPTOUT.SelectedValue.Length < 1)
            {
                Alert.Show("请选择出库库房！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            object obj = DbHelperOra.GetSingle("SELECT NUM1 FROM V_KCSP WHERE FLAG = 'Y' AND DEPTOUT = '" + lstDEPTOUT.SelectedValue + "'AND ROWNUM = '1'");
            if ((obj ?? "").ToString().Length > 0)
            {
                Alert.Show("仓库存在未手工确认出库的波次,请检查！", "提示信息", MessageBoxIcon.Warning);
                TabStrip1.ActiveTabIndex = 1;
                hfdBCode.Text = obj.ToString();
                HDataSearch();
                return;
            }
            int[] index = GridList.SelectedRowIndexArray;
            if (index.Count() < 1)
            {
                Alert.Show("请选择需要分配的单据！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            string strBill = "";
            foreach (int i in index)
            {
                strBill = strBill + "'" + GridList.DataKeys[i][0] + "',";
            }
            if (DbHelperOra.Exists("SELECT 1 FROM V_KCSP WHERE SEQNO IN (" + strBill.TrimEnd(',') + ") AND FLAG IN ('Y','D','W')"))
            {
                Alert.Show("您选择的单据已被他人操作,请检查！", "提示信息", MessageBoxIcon.Warning);
                DataSearch();
                return;
            }

            //判断是否进行调拨
            if (DbHelperOra.Exists(String.Format(@"SELECT 1 FROM
                        (SELECT A.GDSEQ,SUM(A.XSSL) SYSL
                        FROM V_KCSP A,SYS_DEPT B
                        WHERE A.DEPTOUT = B.CODE AND A.SEQNO IN ({1}) AND A.DEPTOUT = '{0}' AND B.ISORDER='N' AND A.BILLFLAG IN('S','N')
                        GROUP BY A.GDSEQ) A,
                        (SELECT GDSEQ,SUM(KCSL-LOCKSL) KCSL
                        FROM DAT_GOODSSTOCK
                        WHERE DEPTID = '{0}' AND KCSL > LOCKSL
                        GROUP BY GDSEQ) B
                        WHERE A.GDSEQ = B.GDSEQ(+) AND SYSL > NVL(KCSL,0)", lstDEPTOUT.SelectedValue, strBill.TrimEnd(','))))
            {
                OracleParameter[] parameters ={
                                            new OracleParameter("VI_DEPTIN" ,OracleDbType.Varchar2,20),
                                            new OracleParameter("VI_BILLNO" ,OracleDbType.Varchar2,2000),
                                            new OracleParameter("VI_USER" ,OracleDbType.Varchar2,20),
                                            new OracleParameter("VO_BILLNUM",OracleDbType.Varchar2,20)
                                           };
                parameters[0].Value = lstDEPTOUT.SelectedValue;
                parameters[1].Value = strBill.TrimEnd(',').Replace("'", "");
                parameters[2].Value = UserAction.UserID;

                parameters[0].Direction = ParameterDirection.Input;
                parameters[1].Direction = ParameterDirection.Input;
                parameters[2].Direction = ParameterDirection.Input;
                parameters[3].Direction = ParameterDirection.Output;

                try
                {
                    DbHelperOra.RunProcedure("STOREDS.P_DB_AUTOFP", parameters);
                    if (parameters[3].Value != null)
                    {
                        Alert.Show("商品库存不足部分已转缺货!", "消息提示", MessageBoxIcon.Information);
                    }
                    else
                    {
                        Alert.Show("商品库存预占分配成功!", "消息提示", MessageBoxIcon.Information);
                    }
                    DataSearch();
                }
                catch (Exception err)
                {
                    Alert.Show(err.Message);
                }
                return;
            }
            if (DbHelperOra.Exists("SELECT 1 FROM V_KCSP WHERE SEQNO IN (" + strBill.TrimEnd(',') + ") AND FLAG IN ('D')"))
            {
                PageContext.RegisterStartupScript(Confirm.GetShowReference("分配单据中存在未调拨完成单据，是否继续？",
                    "信息提示", MessageBoxIcon.Information, PageManager1.GetCustomEventReference(true, "Confirm_Fp", false, false), null));
                return;
            }
            try
            {
                string seq = DbHelperOra.GetSingle("SELECT SEQ_FPKC.NEXTVAL FROM DUAL").ToString();
                hfdBCode.Text = seq;
                List<CommandInfo> cmdList = new List<CommandInfo>();
                cmdList.Add(new CommandInfo("UPDATE DAT_SL_DOC SET NUM1 = " + seq + " WHERE SEQNO IN (" + strBill.TrimEnd(',') + ")", null));
                cmdList.Add(new CommandInfo("UPDATE DAT_CK_DOC SET NUM1 = " + seq + " WHERE SEQNO IN (" + strBill.TrimEnd(',') + ")", null));
                OracleParameter[] parameters = {
                                               new OracleParameter("VIN_BILLNO", OracleDbType.Varchar2,20),
                                               new OracleParameter("VIN_OPERUSER", OracleDbType.Varchar2,800),
                                               new OracleParameter("VO_QHNUM",OracleDbType.Decimal)};
                parameters[0].Value = hfdBCode.Text;
                parameters[1].Value = UserAction.UserID;
                parameters[0].Direction = ParameterDirection.Input;
                parameters[1].Direction = ParameterDirection.Input;
                parameters[2].Direction = ParameterDirection.Output;
                cmdList.Add(new CommandInfo("STOREDS.P_FP_AUTO", parameters, CommandType.StoredProcedure));
                DbHelperOra.ExecuteSqlTran(cmdList);
                if (Convert.ToDecimal(parameters[2].Value.ToString()) > 0)
                {
                    Alert.Show("波次[" + seq + "]自动分配成功！\n\r其中缺货明细[" + parameters[2].Value + "]条！");
                }
                else                
                {
                    Alert.Show("波次[" + seq + "]自动分配成功！");
                }
                OperLog("库存分配", "分配波次【" + seq + "】");
                DataSearch();
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("ORA-20001") > -1)
                {
                    string msg = ex.Message.Substring(0, ex.Message.IndexOf("\n"));
                    Alert.Show("自动分配失败,错误信息：\n\r\n\r                                  " + msg.Substring(msg.IndexOf("ORA-20001") + 10), "错误信息", MessageBoxIcon.Warning);
                }
                else
                {
                    Alert.Show("自动分配失败：\n\r\n\r                                  " + ex.Message + "", "错误信息", MessageBoxIcon.Question);
                }
            }
        }

        protected void btnHand_Click(object sender, EventArgs e)
        {
            if (lstDEPTOUT.SelectedValue.Length < 1)
            {
                Alert.Show("请选择【出库库房】！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            object obj = DbHelperOra.GetSingle("SELECT NUM1 FROM V_KCSP WHERE FLAG = 'Y' AND DEPTOUT = '" + lstDEPTOUT.SelectedValue + "' AND ROWNUM = 1");
            if ((obj ?? "").ToString().Length > 0)
            {
                Alert.Show("仓库存在未手工确认出库的波次,请检查", "提示信息", MessageBoxIcon.Warning);
                TabStrip1.ActiveTabIndex = 1;
                hfdBCode.Text = obj.ToString();
                HDataSearch();
                return;
            }
            int[] index = GridList.SelectedRowIndexArray;
            if (index.Count() < 1)
            {
                Alert.Show("请选择需要分配的单据！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            string strBill = "";
            foreach (int i in index)
            {
                strBill = strBill + "'" + GridList.DataKeys[i][0] + "',";
            }
            if (DbHelperOra.Exists("SELECT 1 FROM V_KCSP WHERE SEQNO IN (" + strBill.TrimEnd(',') + ") AND BILLFLAG IN ('D','W')"))
            {
                Alert.Show("【调拨中】，【调拨完成】的单据都不能进行分配，请检查！", "提示信息", MessageBoxIcon.Warning);
                DataSearch();
                return;
            }
            if (DbHelperOra.Exists("SELECT 1 FROM V_KCSP WHERE SEQNO IN (" + strBill.TrimEnd(',') + ") AND FLAG = 'Y'"))
            {
                Alert.Show("您选择的单据已被他人操作,请检查！", "提示信息", MessageBoxIcon.Warning);
                DataSearch();
                return;
            }
            TabStrip1.ActiveTabIndex = 1;
            string seq = DbHelperOra.GetSingle("SELECT SEQ_FPKC.NEXTVAL FROM DUAL").ToString();
            hfdBCode.Text = seq;
            List<CommandInfo> cmdList = new List<CommandInfo>();
            cmdList.Add(new CommandInfo("UPDATE DAT_SL_DOC SET NUM1 = " + seq + " WHERE SEQNO IN (" + strBill.TrimEnd(',') + ")", null));
            cmdList.Add(new CommandInfo("UPDATE DAT_SL_COM SET FPSL = BZSL*BZHL,FPUSER = '" + UserAction.UserID + "',FPDATE = SYSDATE WHERE SEQNO IN (" + strBill.TrimEnd(',') + ")", null));
            cmdList.Add(new CommandInfo("UPDATE DAT_CK_DOC SET NUM1 = " + seq + " WHERE SEQNO IN (" + strBill.TrimEnd(',') + ")", null));
            cmdList.Add(new CommandInfo("UPDATE DAT_CK_COM SET FPSL = BZSL*BZHL,FPUSER = '" + UserAction.UserID + "',FPDATE = SYSDATE WHERE SEQNO IN (" + strBill.TrimEnd(',') + ")", null));
            DbHelperOra.ExecuteSqlTran(cmdList);
            HDataSearch();
            OperLog("库存分配", "手工分配波次【" + hfdBCode.Text + "】");
        }
        private void HDataSearch()
        {
            //根据hfdBCode打开对应单据
            string strSQL = string.Format(@"SELECT A.DEPTOUT,GDSEQ,GDNAME,f_getunitname(A.UNIT) UNITNAME,GDSPEC,SUM(A.XSSL) SLSL,
                               NVL((SELECT SUM(KCSL-LOCKSL) FROM DAT_GOODSSTOCK WHERE GDSEQ = A.GDSEQ AND KCSL > 0 AND DEPTID = A.DEPTOUT),0) KCSL
                        FROM V_KCSP A WHERE NVL(NUM1,0) = {0}
                        GROUP BY A.DEPTOUT,GDSEQ,GDNAME,UNIT,GDSPEC", hfdBCode.Text);
            int total = 0;
            DataTable dtData = PubFunc.DbGetPage(GridGoods.PageIndex, GridGoods.PageSize, strSQL + " ORDER BY GDSEQ", ref total);
            GridGoods.RecordCount = total;
            GridGoods.DataSource = dtData;
            GridGoods.DataBind();
            GridGoodsList.DataSource = null;
            GridGoodsList.DataBind();
        }
        protected void GridGoods_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                FineUIPro.BoundField flagcol = GridGoods.FindColumn("SLSL") as FineUIPro.BoundField;
                if (Convert.ToInt32(row["KCSL"]) < Convert.ToInt32(row["SLSL"]))
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color3";
                }
            }
        }
        protected void GridGoods_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            lblMessage.Text = "<span  style='font-size: 12px;'>商品：</span><span style='color:red;font-weight:bold;'>[" + GridGoods.DataKeys[e.RowIndex][0] + "]" + GridGoods.DataKeys[e.RowIndex][2] + "</span></br><span  style='font-size: 12px;'>可分配库存数量：</span><span style='color:red;font-weight:bold;'>" + GridGoods.DataKeys[e.RowIndex][1] + "</span>";
            if (!save()) return;
            string gdseq = GridGoods.DataKeys[e.RowIndex][0].ToString();
            string strSql = string.Format(@"SELECT A.*,f_getdeptname(A.DEPTOUT) DEPTOUTNAME,f_getdeptname(A.DEPTID) DEPTIDNAME,f_getunitname(A.UNIT) UNITNAME
                       ,f_getusername(A.LRY) LRYNAME,f_getusername(A.SHR) SHRNAME,DECODE(FPTYPE,'1','定数申领','非定数申领') FPTYPENAME
                FROM V_KCSP A WHERE A.GDSEQ = '{0}' AND NVL(A.NUM1,0) = {1}", gdseq, hfdBCode.Text);
            GridGoodsList.DataSource = DbHelperOra.Query(strSql).Tables[0];
            GridGoodsList.DataBind();
        }
        protected bool save()
        {
            Dictionary<int, Dictionary<string, object>> modifiedDict = GridGoodsList.GetModifiedDict();
            List<CommandInfo> cmdList = new List<CommandInfo>();
            foreach (int rowIndex in modifiedDict.Keys)
            {
                string rowID = GridGoodsList.DataKeys[rowIndex][1].ToString();
                string Seqno = GridGoodsList.DataKeys[rowIndex][0].ToString();
                if (GridGoodsList.DataKeys[rowIndex][2].ToString() == "定数申领" && modifiedDict[rowIndex]["FPSL"].ToString() != "0" && modifiedDict[rowIndex]["FPSL"].ToString() != GridGoodsList.DataKeys[rowIndex][3].ToString())
                {
                    Alert.Show("单号【" + Seqno + "】为定数申领单，分配数量只能维护成0或申领数量");
                    return false;
                }
                cmdList.Add(new CommandInfo(string.Format("UPDATE DAT_SL_COM SET FPSL = '{0}',FPUSER = '{3}',FPDATE = SYSDATE WHERE SEQNO ='{1}' AND ROWNO = {2}", modifiedDict[rowIndex]["FPSL"], Seqno, rowID, UserAction.UserID), null));
                cmdList.Add(new CommandInfo(string.Format("UPDATE DAT_CK_COM SET FPSL = '{0}',FPUSER = '{3}',FPDATE = SYSDATE WHERE SEQNO ='{1}' AND ROWNO = {2}", modifiedDict[rowIndex]["FPSL"], Seqno, rowID, UserAction.UserID), null));
                OperLog("库存分配", "修改单据【" + Seqno + "】出库数量");
            }
            if (cmdList.Count > 0)
                DbHelperOra.ExecuteSqlTran(cmdList);
            return true;
        }

        protected void lstDEPTOUT_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataSearch();
        }

        protected void btnSure_Click(object sender, EventArgs e)
        {
            if (hfdBCode.Text.Length < 1) return;
            if (!save()) return;
            //对分配的数量进行判断
            string sql = string.Format(@"SELECT 1 
                                                        FROM (SELECT DEPTOUT, GDSEQ, SUM(XSSL) SL
                                                                    FROM V_KCSP
                                                                   WHERE NUM1 = '3247'
                                                                   GROUP BY DEPTOUT, GDSEQ) A,
                                                                 (SELECT SUM(KCSL - LOCKSL) SL, GDSEQ, DEPTID
                                                                    FROM DAT_GOODSSTOCK
                                                                   GROUP BY DEPTID, GDSEQ) B
                                                     WHERE A.DEPTOUT = B.DEPTID
                                                       AND A.GDSEQ = B.GDSEQ
                                                       AND A.SL > B.SL", hfdBCode.Text);
            if (DbHelperOra.Exists(sql))
            {
                Alert.Show("分配的商品数量大于可出库库存数量,请检查！", "", MessageBoxIcon.Warning);
                HDataSearch();
                return;
            }
            try
            {
                OracleParameter[] parameters = {
                                               new OracleParameter("VTASKID", OracleDbType.Varchar2,20),
                                               new OracleParameter("VPARA", OracleDbType.Varchar2,800) };
                parameters[0].Value = hfdBCode.Text;
                parameters[1].Value = UserAction.UserID;
                DbHelperOra.RunProcedure("STOREDS.P_FP_AUDIT", parameters);
                Alert.Show("波次[" + hfdBCode.Text + "]配给确认成功！");
                GridGoods.DataSource = null;
                GridGoods.DataBind();
                GridGoodsList.DataSource = null;
                GridGoodsList.DataBind();
                TabStrip1.ActiveTabIndex = 0;
                DataSearch();
                OperLog("库存分配", "分配波次【" + hfdBCode.Text + "】-2");
            }
            catch (Exception mx)
            {
                if (mx.Message.IndexOf("ORA-20001") > -1)
                {
                    string msg = mx.Message.Substring(0, mx.Message.IndexOf("\n"));
                    Alert.Show("配给确认失败,错误信息：\n\r\n\r                                  " + msg.Substring(msg.IndexOf("ORA-20001") + 10), "错误信息", MessageBoxIcon.Warning);
                }
                else if (mx.Message.IndexOf("ORA-20099") > -1)
                {
                    string msg = mx.Message.Substring(0, mx.Message.IndexOf("\n"));
                    Alert.Show("配给确认失败,错误信息：\n\r\n\r                                  " + msg.Substring(msg.IndexOf("ORA-20099") + 10), "错误信息", MessageBoxIcon.Warning);
                }
                else
                {
                    Alert.Show("配给确认失败！" + mx.Message + "", "错误信息", MessageBoxIcon.Question);
                }
                HDataSearch();
            }
        }

        protected void btnCanl_Click(object sender, EventArgs e)
        {
            if (hfdBCode.Text.Length < 1) return;
            if (!DbHelperOra.Exists("SELECT 1 FROM V_KCSP WHERE NUM1 = " + hfdBCode.Text + ""))
            {
                Alert.Show("您分配的商品信息已被其他人操做,请检查！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            List<CommandInfo> cmdList = new List<CommandInfo>();
            cmdList.Add(new CommandInfo(string.Format("UPDATE DAT_SL_COM SET FPSL = 0 WHERE SEQNO IN (SELECT SEQNO FROM V_KCSP WHERE NUM1 = {0} AND FLAG = 'Y')", hfdBCode.Text), null));
            cmdList.Add(new CommandInfo(string.Format("UPDATE DAT_SL_DOC SET NUM1 = 0 WHERE NUM1 = {0} AND FLAG IN ('S','D','W')", hfdBCode.Text), null));
            cmdList.Add(new CommandInfo(string.Format("UPDATE DAT_CK_COM SET FPSL = 0 WHERE SEQNO IN (SELECT SEQNO FROM V_KCSP WHERE NUM1 = {0} AND FLAG = 'Y')", hfdBCode.Text), null));
            cmdList.Add(new CommandInfo(string.Format("UPDATE DAT_CK_DOC SET NUM1 = 0 WHERE NUM1 = {0} AND FLAG = 'N'", hfdBCode.Text), null));
            DbHelperOra.ExecuteSqlTran(cmdList);
            Alert.Show("波次[" + hfdBCode.Text + "]取消分配成功！");
            OperLog("库存分配", "取消手工分配波次【" + hfdBCode.Text + "】");
            GridGoods.DataSource = null;
            GridGoods.DataBind();
            GridGoodsList.DataSource = null;
            GridGoodsList.DataBind();
            TabStrip1.ActiveTabIndex = 0;
            DataSearch();
        }

        protected void GridList_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            string billno = GridList.DataKeys[e.RowIndex][0].ToString();
            string url = url = "~/ERPApply/CKWindow.aspx?bm=" + billno + "";
            PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "出库信息:单号【" + billno + "】"));
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            //打印拣货单
            int[] index = GridList.SelectedRowIndexArray;
            if (index.Count() < 1)
            {
                Alert.Show("请选择需要打印的单据！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            string strBill = "";
            foreach (int i in index)
            {
                strBill = strBill + GridList.DataKeys[i][0] + ",";
            }
            PageContext.RegisterStartupScript("btnPrint_onclick('" + strBill.TrimEnd(',') + "')");
        }
    }
}