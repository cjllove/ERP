﻿using FineUIPro;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using XTBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using XTBase.Utilities;

namespace ERPProject.ERPDictionary
{
    public partial class AddTZSuppliersNew : BillBase
    {
        private string strDocSql = "SELECT t.*,DECODE(XGTYPE,'N','新增','M','修改','禁用') XGTYPENAME,XGTYPE TYPE  FROM DAT_TZSUP_DOC t WHERE SEQNO ='{0}' AND BILLTYPE='TZL'";
        private string strComSql = @"SELECT T.*,
                                       DECODE(T.STR1, 'Y', '是', '否') STR1NAME,
                                       DECODE(T.ISSUPPLIER, 'Y', '是', '否') ISSUPPLIERNAME,
                                       DECODE(T.ISPRODUCER, '2', '是', '否') ISPRODUCERNAME FROM DAT_TZSUP_COM T WHERE T.SEQNO = '{0}'";
        private string strFlagSql = @"SELECT '' CODE ,'--请选择--' NAME  FROM dual
                                      union all ";
        public AddTZSuppliersNew()
        {
            BillType = "TZL";//商品新增单
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
                billNew();

                //屏蔽不需要的操作按钮
                if (Request.QueryString["oper"] != null)
                {
                    string oper = Request.QueryString["oper"].ToString();
                    hfdOper.Text = oper;
                    if (oper == "sq")
                    {
                        ButtonHidden(btnAudit, btnCancel, btnExport);
                        TabStrip1.ActiveTabIndex = 1;
                        dpkLRRQ.SelectedDate = DateTime.Now;
                        strFlagSql += @"SELECT 'M' CODE ,'新单' NAME  FROM dual
                                        union all 
                                        SELECT 'N' CODE ,'提交' NAME  FROM dual  
                                        union all
                                        SELECT 'Y' CODE ,'已审批' NAME  FROM dual
                                        union all
                                        SELECT 'R' CODE ,'已驳回' NAME  FROM DUAL ";

                        PubFunc.DdlDataSql(lstFLAG, strFlagSql);
                    }
                    else if (oper == "zj")
                    {
                        ButtonHidden(btnNew, btnDel, btnSave, btnTJ, btnAddRow, btnDelRow, btnUpdate, btnDelect);
                        TabStrip1.ActiveTabIndex = 0;
                        ddlSPR.SelectedValue = UserAction.UserID;
                        dpkSPRQ.SelectedDate = DateTime.Now;
                        billSearch();
                        strFlagSql += @"SELECT 'N' CODE ,'提交' NAME  FROM dual  
                                        union all
                                        SELECT 'S' CODE ,'已审核' NAME  FROM dual
                                        union all
                                        SELECT 'Y' CODE ,'已审批' NAME  FROM dual
                                        union all
                                        SELECT 'R' CODE ,'已驳回' NAME  FROM DUAL  ";

                        PubFunc.DdlDataSql(lstFLAG, strFlagSql);
                    }
                }

            }
        }

        private void DataInit()
        {
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
            PubFunc.DdlDataGet(docLRY, "DDL_USER");
            PubFunc.DdlDataGet(ddlSPR, "DDL_USER");
            PubFunc.DdlDataGet(lstFLAG, "DDL_ADDGOODSNEWCJ");
            PubFunc.DdlDataGet("DDL_TZGOODS_STATUS", ddlFLAG);
            PubFunc.DdlDataSql(ddlCORPKID, @"select  CODE,NAME from (
                                                        SELECT '0' CODE,'--请选择--' NAME  FROM dual
                                                        union all
                                                        SELECT code,NAME FROM SYS_CODEVALUE where type='DOC_FIRMNATURE'
                                                        )
                                                        ORDER BY NAME,code  ");
            PubFunc.DdlDataSql(docCORPKID, @"select  CODE,NAME from (
                                                        SELECT '0' CODE,'--请选择--' NAME  FROM dual
                                                        union all
                                                        SELECT code,NAME FROM SYS_CODEVALUE where type='DOC_FIRMNATURE'
                                                        )
                                                        ORDER BY NAME,code  ");
            // 绑定到下拉列表（启用模拟树功能）
            ddlFLAG.SelectedValue = "M";
        }

        protected override void billNew()
        {
            //原单据保存判断
            PubFunc.FormDataClear(FormProducer);
            PubFunc.FormDataClear(FormDoc);
            dpkLRRQ.SelectedDate = DateTime.Now;
            ddlFLAG.SelectedValue = "M";
            docLRY.SelectedValue = UserAction.UserID;
            billLockDoc(false);
            btnPrint.Enabled = false;
            btnAddRow.Enabled = true;
            btnUpdate.Enabled = true;
            btnDelect.Enabled = true;
            btnDelRow.Enabled = true;
            btnSave.Enabled = true;
            tbxBILLNO.Enabled = false;
            btnGoods.Enabled = true;
            ddlType.Enabled = true;
            btnDel.Enabled = true;
            hfdIsNew.Text = "";
            GridGoods.DataSource = null;
            GridGoods.DataBind();
        }

        protected override void billClear()
        {
            PubFunc.FormDataClear(Formlist);
            lstLRRQ1.SelectedDate = DateTime.Now;
            lstLRRQ2.SelectedDate = DateTime.Now;
        }

        protected override void billAddRow()
        {
            if ((",M,R").IndexOf(ddlFLAG.SelectedValue) < 0)
            {
                Alert.Show("非『新增』单据不能增行！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            PubFunc.FormDataClear(FormProducer);
            //表单初始化
            hfdIsNew.Text = "N";
            btnUpdate.Enabled = false;
            btnDelect.Enabled = false;
            GridSupplier.Hidden = true;
            PanelCond.Hidden = false;
            PanelCond.Height = 400;
            WindowGoods.Hidden = false;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            dataSearch();
        }
        private void dataSearch()
        {
            string query = "%";
            if (!string.IsNullOrWhiteSpace(tgbSearch.Text))
            {
                query = tgbSearch.Text.Trim();
            }
            int total = 0;
            string sql = string.Format("select SUPID,SUPNAME,TEL,LINKMAN,LOGINADDR,DECODE(FLAG, 'Y', '审核通过', 'N', '未审核') flag,DECODE(ISDG,'Y','是','N','否') ISDG,DECODE(STR1,'Y','是','否') ISBD from  DOC_SUPPLIER WHERE  (SUPID like '%{0}%' or SUPNAME like'%{0}%') and FLAG<>'E'", query);
            if (!string.IsNullOrWhiteSpace(lstFLAG.SelectedValue))
            {
                sql += string.Format(" and FLAG = '{0}'", lstFLAG.SelectedValue);
            }
            //if (!string.IsNullOrWhiteSpace(lstISSEND.SelectedValue))
            //{
            //    sql += string.Format(" and ISSEND = '{0}'", lstISSEND.SelectedValue);
            //}
            DataTable dtData = PubFunc.DbGetPage(GridSupplier.PageIndex, GridSupplier.PageSize, sql, ref total);
            GridSupplier.RecordCount = total;
            GridSupplier.DataSource = dtData;
            GridSupplier.DataBind();
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
                         WHERE SP.CATID=B.CODE(+) AND SP.FLAG=S.CODE AND SP.SUPPLIER=C.SUPID(+) AND SP.UNIT = D.CODE(+) and SP.CATID0 = e.code(+)
                                AND SP.ISDELETE='N'";

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


        protected override void billDelRow()
        {

            if ((",M,R").IndexOf(ddlFLAG.SelectedValue) < 0)
            {
                Alert.Show("非『新增』单据不能删行！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (GridGoods.SelectedRowID == null)
            {
                Alert.Show("当前没有选中行，无法进行【删行】操作", "操作警告", MessageBoxIcon.Warning);
                return;
            }
            int[] rowId = GridGoods.SelectedRowIndexArray;
            for (int i = 0; i < rowId.Length; i++)
            {
                GridGoods.DataSource = DeleteRowByID(rowId[i]);
                GridGoods.DataBind();
            }
            //GridGoods.DeleteSelectedRows();
        }

        //根据行ID来删除行数据
        private DataTable DeleteRowByID(int rowID)
        {
            DataTable table = PubFunc.GridDataGet(GridGoods);
            DataRow found = table.Rows[rowID];
            if (found != null)
            {
                table.Rows.Remove(found);
            }
            return table;
        }
        protected override void billSearch()
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【使用日期】！");
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                Alert.Show("开始日期大于结束日期，请重新输入！");
                return;
            }

            string strSql = @"SELECT A.SEQNO,A.BILLNO,
                               a.flag,
                               decode(a.flag,'M','新单','N','提交','Q','启用','S','已审核','Y','已审批','R','已驳回','F','已废弃') flagname,
                               decode(a.XGTYPE,'N','新增','M','修改','D','作废') xgtypename,
                               A.SUBNUM,
                               F_GETUSERNAME(A.LRY) LRY,
                               A.LRRQ,
                               F_GETUSERNAME(A.SPR) SPR,
                               A.SPRQ,
                               F_GETUSERNAME(A.SHR) SHR,
                               A.SHRQ,
                               A.MEMO
                          from DAT_TZSUP_DOC A WHERE 1=1 ";
            string strSearch = "";


            if (lstBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND TRIM(UPPER(A.BILLNO)) LIKE '%{0}%'", lstBILLNO.Text.Trim().ToUpper());
            }
            if (!string.IsNullOrWhiteSpace(lstFLAG.SelectedValue))
            {
                strSearch += string.Format(" AND A.FLAG='{0}'", lstFLAG.SelectedValue);
            }
            if (Request.QueryString["oper"] != null)
            {
                string oper = Request.QueryString["oper"].ToString();
                if (oper == "zj")
                {
                    strSearch += string.Format(" AND A.FLAG<>'M'", lstFLAG.SelectedValue);
                }
            }
            strSearch += string.Format(" AND A.LRRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.LRRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY DECODE(A.FLAG,'M','1','R','2','N','3','Q','4','S','5','6'),A.BILLNO DESC";
            highlightRowYellow.Text = "";
            highlightRowRed.Text = "";
            highlightRowGreen.Text = "";
            GridList.DataSource = DbHelperOra.Query(strSql).Tables[0];
            GridList.DataBind();
        }

        protected override void billAudit()
        {

            if (ddlFLAG.SelectedValue != "N")
            {
                Alert.Show("单据状态不正确！请检测！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            List<CommandInfo> cmdList = new List<CommandInfo>();
            string flag = string.Empty;
            try
            {
                flag = DbHelperOra.GetSingle("SELECT XGTYPE FROM DAT_TZSUP_DOC T WHERE T.SEQNO='" + tbxSEQNO.Text + "'").ToString();
            }
            catch
            {
                Alert.Show("单据号存在问题！请检测！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            DataTable dt = DbHelperOra.Query("SELECT SUPID FROM DAT_TZSUP_COM T WHERE T.SEQNO='" + tbxSEQNO.Text + "'").Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                if (flag.Trim() == "N")
                {
                    foreach (DataRow row in dt.Rows)
                    {

                        if (!DbHelperOra.Exists("SELECT 1 FROM DOC_SUPPLIER T WHERE T.SUPID='" + row["SUPID"].ToString() + "'"))
                        {
                            cmdList.Add(new CommandInfo("UPDATE DOC_SUPPLIERTEMP SET UPTTIME=SYSDATE WHERE SUPID='" + row["SUPID"].ToString() + "'", null));
                            cmdList.Add(new CommandInfo("INSERT INTO DOC_SUPPLIER SELECT * FROM DOC_SUPPLIERTEMP WHERE SUPID='" + row["SUPID"].ToString() + "'", null));
                        }
                    }
                }
                else if (flag.Trim() == "M")
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        if (DbHelperOra.Exists("SELECT 1 FROM DOC_SUPPLIER T WHERE T.SUPID='" + row["SUPID"].ToString() + "'"))
                        {
                            cmdList.Add(new CommandInfo(string.Format("UPDATE DOC_SUPPLIER SET SUPID='{0}'||'停'|| TO_CHAR(SYSDATE, 'yyyy-MM-dd hh:mm:ss'),FLAG='E' WHERE SUPID='{0}'", row["SUPID"].ToString()), null));
                            cmdList.Add(new CommandInfo("UPDATE DOC_SUPPLIERTEMP SET UPTTIME=SYSDATE WHERE SUPID='" + row["SUPID"].ToString() + "'", null));
                            cmdList.Add(new CommandInfo("INSERT INTO DOC_SUPPLIER SELECT * FROM DOC_SUPPLIERTEMP WHERE SUPID='" + row["SUPID"].ToString() + "'", null));
                            cmdList.Add(new CommandInfo(string.Format("UPDATE DOC_SUPPLIERTEMP SET SUPID='{0}'||'停'|| TO_CHAR(SYSDATE, 'yyyy-MM-dd hh:mm:ss'),FLAG='E' WHERE SUPID='{0}'", row["SUPID"].ToString()), null));
                        }
                    }
                }
                else if (flag.Trim() == "D")
                {
                    foreach (DataRow row in dt.Rows)
                    {

                        if (DbHelperOra.Exists("SELECT 1 FROM DOC_SUPPLIER T WHERE T.SUPID='" + row["SUPID"].ToString() + "'"))
                        {
                            cmdList.Add(new CommandInfo("UPDATE DOC_SUPPLIER T SET T.FLAG='E' WHERE T.SUPID='" + row["SUPID"].ToString() + "'", null));
                        }
                    }
                }
                else
                {
                    Alert.Show("单据状态存在问题！请检测！", "消息提示", MessageBoxIcon.Warning);
                    return;
                }

                if (DbHelperOra.ExecuteSqlTran(cmdList))
                {
                    Alert.Show("供应商信息审批成功！");
                    DbHelperOra.ExecuteSql(string.Format("UPDATE DAT_TZSUP_DOC SET FLAG = 'Y',SPR='{1}',SPRQ=SYSDATE,STR1='N'  WHERE FLAG= 'N' AND SEQNO = '{0}'", tbxSEQNO.Text, UserAction.UserID));
                    btnCancel.Enabled = false;
                    OperLog("供应商管理", "审核单据【" + tbxBILLNO.Text + "】");
                    billOpen(tbxBILLNO.Text);
                }
            }

        }
        protected void btnRejectSubmit_Click(object sender, EventArgs e)
        {
            if (ddlType.SelectedValue != "D")
            {
                if (PubFunc.FormDataCheck(FormProducer).Length > 0) return;
            }

            if (tbxSUPID.Text.Trim().Length > 1)
            {
                if (DbHelperOra.Exists("select 1 from DOC_SUPPLIERTEMP where SUPID='" + tbxSUPID.Text + "'"))
                {
                    DbHelperOra.ExecuteSql("DELETE FROM DOC_SUPPLIERTEMP WHERE SUPID='" + tbxSUPID.Text + "'");
                }
            }
            if (hfdIsNew.Text == "N" && tbxSUPID.Text.Trim().Length < 1)
            {
                DataTable dt = DbHelperOra.QueryForTable("SELECT F_GET_SUPCODE FROM DUAL");
                if (dt.Rows.Count > 0)
                {
                    tbxSUPID.Text = dt.Rows[0][0].ToString();
                }
            }
            else if (hfdIsNew.Text == "M" && tbxSUPID.Text.Trim().Length > 0)
            {
                if (DbHelperOra.Exists("select 1 from DOC_SUPPLIERTEMP where SUPID='" + tbxSUPID.Text + "'"))
                {
                    DbHelperOra.ExecuteSql("DELETE FROM DOC_SUPPLIERTEMP WHERE SUPID='" + tbxSUPID.Text + "'");
                }
            }
            else if (hfdIsNew.Text == "D")
            {
                for (int i = 0; i < GridSupplier.SelectedRowIndexArray.Length; i++)
                {
                    if (DbHelperOra.Exists("select 1 from DOC_SUPPLIERTEMP where SUPID='" + GridSupplier.Rows[GridSupplier.SelectedRowIndexArray[i]].DataKeys[0] + "'"))
                    {
                        DbHelperOra.ExecuteSql("DELETE FROM DOC_SUPPLIERTEMP WHERE SUPID='" + GridSupplier.Rows[GridSupplier.SelectedRowIndexArray[i]].DataKeys[0] + "'");
                    }
                    if (DbHelperOra.Exists("select 1 from DOC_SUPPLIER where SUPID='" + GridSupplier.Rows[GridSupplier.SelectedRowIndexArray[i]].DataKeys[0] + "'"))
                    {
                        DbHelperOra.ExecuteSql("INSERT INTO DOC_SUPPLIERTEMP SELECT * FROM DOC_SUPPLIER where SUPID='" + GridSupplier.Rows[GridSupplier.SelectedRowIndexArray[i]].DataKeys[0] + "' ");
                    }
                    hfdTEMP.Text += GridSupplier.Rows[GridSupplier.SelectedRowIndexArray[i]].DataKeys[0] + ",";
                }
                GridInIt();
                WindowGoods.Hidden = true;
                return;
            }
            MyTable mtType = new MyTable("DOC_SUPPLIERTEMP");
            if (tbxSUPNAME.Text.Trim().Length < 1)
            {
                Alert.Show("请输入供应商名称!", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            if ((DbHelperOra.Exists("select 1 from DOC_SUPPLIER where SUPID = '" + tbxSUPID.Text + "'")) && (tbxSUPID.Enabled))
            {
                Alert.Show("此供应商编码已经存在,请重新输入!", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            if (hfdIsNew.Text == "N")
            {
                if (DbHelperOra.Exists("SELECT 1 FROM DOC_SUPPLIER WHERE SUPNAME = '" + tbxSUPNAME.Text + "'"))
                {
                    Alert.Show("供应商名称已存在,请重新输入!", "提示信息", MessageBoxIcon.Warning);
                    return;
                }
            }
            mtType.ColRow = PubFunc.FormDataHT(FormProducer);
            mtType.ColRow.Add("SUPCAT", "03");
            mtType.ColRow["ISSEND"] = chkSTR1.Checked ? "N" : "Y";
            mtType.ColRow["ISDG"] = chkSTR1.Checked ? "Y" : "N";
            mtType.ColRow["FLAG"] = "Y";
            mtType.InsertExec();
            GridInTo();

        }
        protected void GridInTo()
        {
            if (GridGoods != null && GridGoods.Rows.Count > 0)
            {
                foreach (GridRow row in GridGoods.Rows)
                {
                    hfdTEMP.Text += row.Values[1].ToString() + ",";
                }
            }
            hfdTEMP.Text += tbxSUPID.Text + ",";
            hfdTEMP.Text = hfdTEMP.Text.TrimEnd(',').Replace(",", "','");
            GridGoods.DataSource = DbHelperOra.Query(@"SELECT T.*,
                                       DECODE(T.STR1, 'Y', '是', '否') STR1NAME,
                                       DECODE(T.ISSUPPLIER, 'Y', '是', '否') ISSUPPLIERNAME,
                                       DECODE(T.ISPRODUCER, '2', '是', '否') ISPRODUCERNAME FROM DOC_SUPPLIERTEMP T WHERE T.SUPID IN ('" + hfdTEMP.Text + "')").Tables[0];
            GridGoods.DataBind();

            string s = hfdIsNew.Text;
            hfdTEMP.Text = "";
            WindowGoods.Hidden = true;
            tgbSearch.Text = string.Empty;
            GridSupplier.DataSource = null;
            GridSupplier.DataBind();
        }
        protected void tbxSUPNAME_TextChanged(object sender, EventArgs e)
        {
            tbxSTR2.Text = PinYinUtil.GetCodstring(tbxSUPNAME.Text);
        }
        protected override void listRow_DoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            billOpen(GridList.Rows[e.RowIndex].Values[1].ToString());
        }
        protected void GridSupplier_RowDoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            string strCode = GridSupplier.DataKeys[e.RowIndex][0].ToString();
            cxsj(strCode);
        }
        protected void cxsj(String strCode)
        {
            string strSql = string.Format("select * from DOC_SUPPLIER where SUPID='{0}'", strCode);

            DataTable dtProducer = DbHelperOra.Query(strSql).Tables[0];
            if (dtProducer.Rows.Count > 0)
            {

                PubFunc.FormDataSet(FormProducer, dtProducer.Rows[0]);
            }

            if (ddlFLAG.SelectedValue == "Y")
            {
                PubFunc.FormLock(FormProducer, true);
                btnSave.Enabled = false;
                btnAudit.Enabled = false;
            }
            else
            {
                PubFunc.FormLock(FormProducer, false);
                btnAudit.Enabled = true;
                tbxSUPID.Enabled = false;
                btnSave.Enabled = true;
            }

            ddlFLAG.Enabled = false;
            chkSTR1.Enabled = false;
        }
        protected void Grid1_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridSupplier.PageIndex = e.NewPageIndex;
            dataSearch();
        }
        protected void goodsRow_DoubleClick(object sender, GridRowClickEventArgs e)
        {
            DataTable dtnew = DbHelperOra.Query("SELECT * FROM DOC_SUPPLIERTEMP t where t.supid='" + GridGoods.Rows[e.RowIndex].DataKeys[2] + "'").Tables[0];
            if (dtnew.Rows.Count > 0)
            {
                //GridGoods.Rows[e.RowIndex]
            }
            if (hfdOper.Text == "sq")
            {
                PubFunc.FormDataSet(FormProducer, dtnew.Rows[0]);
                if (tbxSEQNO.Text.Length > 0)
                {
                    if (DbHelperOra.GetSingle("SELECT T.FLAG FROM DAT_TZSUP_DOC T WHERE T.SEQNO='" + tbxSEQNO.Text + "'").ToString() != "M")
                    {
                        PubFunc.FormLock(FormProducer, true);
                    }
                }
            }
            if (hfdOper.Text == "zj")
            {
                PubFunc.FormDataSet(FormProducer, dtnew.Rows[0]);
                PubFunc.FormLock(FormProducer, true);
            }
            if (ddlType.SelectedValue == "M")
            {
                DataTable dt = DbHelperOra.Query("SELECT * FROM DOC_SUPPLIER T WHERE T.SUPID = '" + GridGoods.Rows[e.RowIndex].DataKeys[2] + "' ORDER BY T.SUPID desc").Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {

                    formGet(Form3, dt.Rows[0]);
                }
                else
                {
                    PubFunc.FormDataSet(Form3, GridGoods.Rows[e.RowIndex]);
                }

                PubFunc.FormLock(Form3, true);
                Panel4.Hidden = false;
            }
            WindowGoods.Hidden = false;
            GridSupplier.Hidden = true;
        }

        private void formGet(Form formItms, DataRow row)
        {
            foreach (Control control in formItms.Controls)
            {
                foreach (Control item in control.Controls)
                {
                    if (item is FineUIPro.TextBox)
                    {
                        FineUIPro.TextBox txt = (FineUIPro.TextBox)item;
                        try
                        {
                            txt.Text = row[txt.ID.Substring(3)].ToString();
                        }
                        catch { }

                    }
                    else if (item is FineUIPro.DropDownList)
                    {
                        FineUIPro.DropDownList ddl = (FineUIPro.DropDownList)item;
                        if (ddl.ID.Substring(3) == "TYPE")
                        {
                            ddl.SelectedValue = ddlType.SelectedValue;
                        }
                        else
                        {
                            try
                            {
                                ddl.SelectedValue = row[ddl.ID.Substring(3)].ToString();
                            }
                            catch { }

                        }
                    }
                    else if (item is FineUIPro.CheckBox)
                    {
                        FineUIPro.CheckBox cbx = (FineUIPro.CheckBox)item; try
                        {
                            if (row[cbx.ID.Substring(3)].ToString() == "Y")
                            {
                                cbx.Checked = true;
                            }
                            else
                            {
                                cbx.Checked = false;
                            }
                        }
                        catch { }

                    }
                    else if (item is FineUIPro.NumberBox)
                    {
                        FineUIPro.NumberBox num = (FineUIPro.NumberBox)item;
                        try
                        {
                            num.Text = row[num.ID.Substring(3)].ToString();
                        }
                        catch { }

                    }
                    else if (item is FineUIPro.DatePicker)
                    {
                        FineUIPro.DatePicker date = (FineUIPro.DatePicker)item;
                        try
                        {
                            date.Text = row[date.ID.Substring(3)].ToString();
                        }
                        catch { }

                    }
                }
            }
        }

        protected override void billOpen(string strBillno)
        {
            DataTable dtDoc = DbHelperOra.Query(string.Format(strDocSql, strBillno)).Tables[0];
            PubFunc.FormDataSet(FormDoc, dtDoc.Rows[0]);
            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            DataTable dtBill = DbHelperOra.Query(string.Format(strComSql, strBillno)).Tables[0];

            GridGoods.DataSource = dtBill;
            GridGoods.DataBind();

            PubFunc.FormLock(FormDoc, true, "");
            TabStrip1.ActiveTabIndex = 1;
            //屏蔽不需要的操作按钮
            if (ddlFLAG.SelectedValue == "N")
            {
                btnTJ.Enabled = false;
                btnAddRow.Enabled = false;
                btnUpdate.Enabled = false;
                btnDelect.Enabled = false;
                btnDelRow.Enabled = false;
                btnSave.Enabled = false;
                btnPrint.Enabled = true;
                btnCancel.Enabled = true;
                btnAudit.Enabled = true;
                btnGoods.Enabled = false;
                btnDel.Enabled = false;
            }
            if (ddlFLAG.SelectedValue == "R")
            {
                btnAddRow.Enabled = false;
                btnUpdate.Enabled = false;
                btnDelect.Enabled = false;
                btnDelRow.Enabled = false;
                btnSave.Enabled = false;
                btnPrint.Enabled = true;
                btnCancel.Enabled = true;
                btnAudit.Enabled = false;
                btnGoods.Enabled = true;
                string flag = DbHelperOra.GetSingle("select XGTYPE from DAT_TZSUP_DOC t where t.billno='" + strBillno + "'").ToString();
                if (flag == "N")
                {
                    btnAddRow.Enabled = true;
                }
                else if (flag == "M")
                {
                    btnUpdate.Enabled = true;
                }
                else if (flag == "D")
                {
                    btnDelect.Enabled = true;
                }
                btnDelRow.Enabled = true;
                btnSave.Enabled = true;
                btnCancel.Enabled = false;
                btnDel.Enabled = true;
            }
            if (ddlFLAG.SelectedValue == "M")
            {
                btnAddRow.Enabled = false;
                btnUpdate.Enabled = false;
                btnDelect.Enabled = false;
                btnTJ.Enabled = true;
                btnGoods.Enabled = true;
                string flag = DbHelperOra.GetSingle("select XGTYPE from DAT_TZSUP_DOC t where t.billno='" + strBillno + "'").ToString();
                if (flag == "N")
                {
                    btnAddRow.Enabled = true;
                }
                else if (flag == "M")
                {
                    btnUpdate.Enabled = true;
                }
                else if (flag == "D")
                {
                    btnDelect.Enabled = true;
                }
                btnDelRow.Enabled = true;
                btnSave.Enabled = true;
                btnDel.Enabled = true;
            }
            else if (ddlFLAG.SelectedValue == "Y")
            {
                btnAddRow.Enabled = false;
                btnUpdate.Enabled = false;
                btnDelect.Enabled = false;
                btnDelRow.Enabled = false;
                btnSave.Enabled = false;
                btnTJ.Enabled = false;
                btnAudit.Enabled = false;
                btnPrint.Enabled = true;
                btnCancel.Enabled = false;
                btnUpdate.Enabled = false;
                btnDelect.Enabled = false;
                btnGoods.Enabled = false;
                btnDel.Enabled = false;
            }
        }

        protected override void billSave()
        {
            save();
        }

        private void save(string flag = "N")
        {
            #region 数据有效性验证
            if ((",M,R").IndexOf(ddlFLAG.SelectedValue) < 0)
            {
                Alert.Show("非新单不能保存！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            if (PubFunc.FormDataCheck(FormDoc).Length > 0) return;
            if (GridGoods.Rows.Count() == 0)
            {
                Alert.Show("请先进行数据维护，再进行保存操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            //验证单据信息

            if (DbHelperOra.Exists("SELECT 1 FROM DAT_TZSUP_DOC where seqno = '" + tbxBILLNO.Text + "'") && tbxBILLNO.Enabled)
            {
                Alert.Show("您输入的单号已存在,请检查!");
                return;
            }
            #endregion

            if (PubFunc.StrIsEmpty(tbxBILLNO.Text))
            {
                tbxSEQNO.Text = BillSeqGet();
                tbxBILLNO.Text = tbxSEQNO.Text;
                tbxBILLNO.Enabled = false;
            }
            else
            {
                string flg = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M') FROM DAT_TZSUP_DOC WHERE SEQNO='{0}'", tbxBILLNO.Text));
                if (!string.IsNullOrWhiteSpace(flg) && (",M,R").IndexOf(flg) < 0)
                {
                    Alert.Show("您输入的单据号存在重复信息，请重新输入或置空！", "消息提示", MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    tbxSEQNO.Text = tbxBILLNO.Text;
                    tbxBILLNO.Enabled = false;
                }
            }

            MyTable mtType = new MyTable("DAT_TZSUP_DOC");
            mtType.ColRow = PubFunc.FormDataHT(FormDoc);
            mtType.ColRow["SEQNO"] = tbxBILLNO.Text;
            mtType.ColRow.Add("SUBNUM", GridGoods.Rows.Count);
            mtType.ColRow.Add("XGTYPE", ddlType.SelectedValue);
            mtType.ColRow.Add("BILLTYPE", "TZL");
            mtType.ColRow["FLAG"] = "M";
            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtTypeMx = new MyTable("DAT_TZSUP_COM");
            //先删除单据信息在插入
            cmdList.Add(new CommandInfo("delete DAT_TZSUP_DOC where seqno='" + tbxBILLNO.Text + "'", null));//删除单据台头
            cmdList.Add(new CommandInfo("delete DAT_TZSUP_COM where seqno='" + tbxBILLNO.Text + "'", null));//删除单据明细
            cmdList.AddRange(mtType.InsertCommand());
            int index = 0;
            foreach (GridRow row in GridGoods.Rows)
            {
                index++;
                mtTypeMx.ColRow = PubFunc.GridDataGet(row);
                mtTypeMx.ColRow["SEQNO"] = tbxBILLNO.Text;
                mtTypeMx.ColRow["ROWNO"] = index;
                cmdList.Add(mtTypeMx.Insert());
            }

            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                Alert.Show("供应商信息保存成功！");
                OperLog("供应商管理", "修改单据【" + tbxBILLNO.Text + "】");
                billOpen(tbxBILLNO.Text);

            }
            SaveSuccess = true;
        }

        /// <summary>
        /// FineUIPro.Grid控件数据加载
        /// </summary>
        /// <param name="row">要加载的行数据</param>
        /// <param name="firstRow">是否插入指定行</param>
        /// <param name="flag">数据来源：NEW-从数据库中获得，用于商品新增时；OLD-从销售单据明细中获得，用于修改或审核时</param>
        private void LoadGridRow(DataRow row, bool firstRow = true, string flag = "NEW")
        {
            PubFunc.GridRowAdd(GridGoods, row, firstRow);
        }


        protected override void billDel()
        {
            if (tbxBILLNO.Text.Trim() == "")
            {
                Alert.Show("请选择需要删除的单据");
                return;
            }

            if (",M,R".IndexOf(ddlFLAG.SelectedValue) < 1)
            {
                Alert.Show("非新单不能删除!");
                return;
            }
            DbHelperOra.ExecuteSql("Delete from DAT_TZSUP_DOC t WHERE T.SEQNO ='" + tbxBILLNO.Text.Trim() + "'");
            DbHelperOra.ExecuteSql("Delete from DAT_TZSUP_COM t WHERE T.SEQNO ='" + tbxBILLNO.Text.Trim() + "'");
            Alert.Show("单据删除成功!");
            OperLog("供应商管理", "删除单据【" + tbxSEQNO.Text + "】");
            billNew();
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                return;
            }
            billSearch();
        }



        protected void GridList_Sort(object sender, GridSortEventArgs e)
        {
            GridList.SortDirection = e.SortDirection;
            GridList.SortField = e.SortField;

            DataTable table = PubFunc.GridDataGet(GridList);
            DataView view1 = table.DefaultView;
            view1.Sort = String.Format("{0} {1}", GridList.SortField, GridList.SortDirection);
            GridList.DataSource = view1;
            GridList.DataBind();
        }
        protected void GridList_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                string flag = row["FLAG"].ToString();
                FineUIPro.BoundField flagcol = GridList.FindColumn("FLAGNAME") as FineUIPro.BoundField;
                if (flag == "M")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color1";
                }
                if (flag == "N")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color2";
                }
                if (flag == "R")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color3";
                }
                //屏蔽不需要的操作按钮
                if (Request.QueryString["oper"] != null)
                {
                    string oper = Request.QueryString["oper"].ToString();
                    hfdOper.Text = oper;
                    if (oper == "sq")
                    {
                        if (flag == "M")
                        {
                            highlightRowGreen.Text += e.RowIndex.ToString() + ",";
                        }
                        else if (flag == "N")
                        {
                            highlightRowYellow.Text += e.RowIndex.ToString() + ",";
                        }
                    }
                    else if (oper == "zj")
                    {
                        if (flag == "S")
                        {
                            highlightRowYellow.Text += e.RowIndex.ToString() + ",";
                        }
                        else if (flag == "R")
                        {
                            highlightRowRed.Text += e.RowIndex.ToString() + ",";
                        }
                    }
                }

            }
        }
        private bool SaveSuccess = false;
        protected void btnTJ_Click(object sender, EventArgs e)
        {
            if (tbxSEQNO.Text.Length < 1)
            {
                Alert.Show("请先保存再提交！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            string flg = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M') FROM DAT_TZSUP_DOC WHERE SEQNO='{0}'", tbxSEQNO.Text));
            if (!PubFunc.StrIsEmpty(flg) && (",M").IndexOf(flg) < 0)
            {
                Alert.Show("只有保存后单据，才能提交！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            SaveSuccess = false;
            save("Y");
            if (SaveSuccess == false)
                return;
            SaveSuccess = false;
            string msg = "";
            DataTable dt = DbHelperOra.Query(string.Format("SELECT A.SUPID,A.SUPNAME,A.SEQNO  FROM DAT_TZSUP_COM A,DAT_TZSUP_DOC B WHERE A.SUPID IN(SELECT SUPID FROM DAT_TZSUP_COM WHERE SEQNO ='{0}' )AND A.SEQNO NOT LIKE '{0}' AND B.FLAG='N' AND A.SEQNO=B.SEQNO", tbxSEQNO.Text)).Tables[0];
            if (dt.Rows.Count > 0)
            {
                msg = "商品【[" + dt.Rows[0]["SUPID"] + "]" + dt.Rows[0]["SUPNAME"] + "】被单据【" + dt.Rows[0]["SEQNO"] + "】使用,请维护！";
                Alert.Show(msg, "提示信息", MessageBoxIcon.Warning);
                return;
            }
            if (DbHelperOra.ExecuteSql(string.Format("UPDATE DAT_TZSUP_DOC SET FLAG = 'N' WHERE FLAG= 'M' AND SEQNO = '{0}'", tbxSEQNO.Text)) > 0)
            {
                Alert.Show("供应商信息单据提交成功！");
                OperLog("供应商管理", "提交单据【" + tbxBILLNO.Text + "】");
                billOpen(tbxSEQNO.Text);
                btnPrint.Enabled = true;
            }
            else
            {
                Alert.Show("提交失败，请检查单据状态！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
        }


        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (ddlFLAG.SelectedValue == "Y")
            {
                Alert.Show("本条申领信息已经审批确认，不能进行【驳回】操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            WindowReject.Hidden = false;
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (ddlFLAG.SelectedValue == "N")
            {
                Alert.Show("本条申领信息不是【已提交】状态，不能进行【废弃】操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (DbHelperOra.ExecuteSql(string.Format("UPDATE DAT_TZSUP_DOC SET FLAG = 'F' WHERE FLAG= 'N' AND SEQNO = '{0}'", tbxSEQNO.Text)) > 0)
            {
                Alert.Show("废弃成功！");
                billOpen(tbxSEQNO.Text);
            }
            else
            {
                Alert.Show("废弃失败，请检查单据状态！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
        }




        protected void btnUpdate_Click(object sender, EventArgs e)
        {

            hfdIsNew.Text = "M";
            PubFunc.FormDataClear(FormProducer);
            btnDelect.Enabled = false;
            btnAddRow.Enabled = false;
            GridSupplier.DataSource = null;
            GridSupplier.DataBind();
            Button2.Hidden = true;
            PanelCond.Hidden = false;
            WindowGoods.Hidden = false;
            GridSupplier.Height = 250;
            GridSupplier.Hidden = false;
        }

        protected void btnDelect_Click(object sender, EventArgs e)
        {

            hfdIsNew.Text = "D";
            PubFunc.FormDataClear(FormProducer);
            btnUpdate.Enabled = false;
            btnAddRow.Enabled = false;
            tgbSearch.Text = "";
            GridSupplier.DataSource = null;
            GridSupplier.DataBind();
            WindowGoods.Hidden = false;
            GridSupplier.Hidden = false;
            PanelCond.Hidden = true;
            GridSupplier.Height = 420;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (txaMemo.Text.Trim().Length < 1)
            {
                Alert.Show("请填写驳回原因！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            if (DbHelperOra.ExecuteSql(string.Format("UPDATE DAT_TZSUP_DOC SET FLAG = 'R', MEMO='驳回原因：'||'{1}' WHERE  SEQNO = '{0}'", tbxSEQNO.Text, txaMemo.Text.Trim())) > 0)
            {
                Alert.Show("驳回成功！");
                OperLog("供应商审批", "驳回单据【" + tbxSEQNO.Text + "】");
                billOpen(tbxSEQNO.Text);
                btnAudit.Enabled = false;
            }
            else
            {
                Alert.Show("驳回失败，请检查单据状态！", "提示信息", MessageBoxIcon.Warning);
                WindowReject.Hidden = true;
                return;
            }
            WindowReject.Hidden = true;
        }

        protected void btnGoods_Click(object sender, EventArgs e)
        {
            if (ddlType.SelectedValue.Length < 1)
            {
                Alert.Show("请选择单据类型！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            ddlType.Enabled = false;
            Panel4.Hidden = true;
            switch (ddlType.SelectedValue)
            {
                case "N":
                    Button2.Hidden = true;
                    tbxSUPID.Enabled = true;
                    billAddRow();
                    break;
                case "M":
                    Button2.Hidden = true;
                    btnUpdate_Click(null, null);
                    break;
                case "D":
                    Button2.Hidden = false;
                    btnDelect_Click(null, null);
                    break;
                default:
                    Alert.Show("请选择单据类型！", "提示信息", MessageBoxIcon.Warning);
                    break;
            }
        }
        protected void Button2_Click(object sender, EventArgs e)
        {
            if (ddlType.SelectedValue == "D")
            {
                for (int i = 0; i < GridSupplier.SelectedRowIndexArray.Length; i++)
                {
                    if (DbHelperOra.Exists("select 1 from DOC_SUPPLIERTEMP where SUPID='" + GridSupplier.Rows[GridSupplier.SelectedRowIndexArray[i]].DataKeys[0] + "'"))
                    {
                        DbHelperOra.ExecuteSql("DELETE FROM DOC_SUPPLIERTEMP WHERE SUPID='" + GridSupplier.Rows[GridSupplier.SelectedRowIndexArray[i]].DataKeys[0] + "'");
                    }
                    if (DbHelperOra.Exists("select 1 from DOC_SUPPLIER where SUPID='" + GridSupplier.Rows[GridSupplier.SelectedRowIndexArray[i]].DataKeys[0] + "'"))
                    {
                        DbHelperOra.ExecuteSql("INSERT INTO DOC_SUPPLIERTEMP SELECT * FROM DOC_SUPPLIER where SUPID='" + GridSupplier.Rows[GridSupplier.SelectedRowIndexArray[i]].DataKeys[0] + "' ");
                    }
                    hfdTEMP.Text += GridSupplier.Rows[GridSupplier.SelectedRowIndexArray[i]].DataKeys[0] + ",";
                }
                GridInIt();
            }
        }
        protected void GridInIt()
        {
            if (GridGoods != null && GridGoods.Rows.Count > 0)
            {
                foreach (GridRow row in GridGoods.Rows)
                {
                    hfdTEMP.Text += row.Values[1].ToString() + ",";
                }
            }
            hfdTEMP.Text = hfdTEMP.Text.TrimEnd(',').Replace(",", "','");
            GridGoods.DataSource = DbHelperOra.Query(@"SELECT T.*,
                                       DECODE(T.STR1, 'Y', '是', '否') STR1NAME,
                                       DECODE(T.ISSUPPLIER, 'Y', '是', '否') ISSUPPLIERNAME,
                                       DECODE(T.ISPRODUCER, '2', '是', '否') ISPRODUCERNAME FROM DOC_SUPPLIERTEMP T WHERE T.SUPID IN ('" + hfdTEMP.Text + "')").Tables[0];
            GridGoods.DataBind();

            hfdTEMP.Text = "";
            tgbSearch.Text = string.Empty;
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            if (GridGoods.Rows.Count < 1)
            {
                Alert.Show("没有数据,无法导出！");
                return;
            }
            if (tbxBILLNO.Text.Length < 1)
            {
                Alert.Show("请先保存，再导出！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            string strSql = @"SELECT A.SUPID 供应商编号,
                                     A.SUPNAME 供应商名称,
                                     A.TEL 公司点哈,
                                     A.LINKMAN 联系人，
                                     A.LOGINADDR 注册地址，
                                     DECODE(A.ISSUPPLIER, 'Y', '是', '否') 是否供应商，
                                     DECODE(A.ISPRODUCER, 'Y', '是', '否') 是否生产商，
                                     DECODE(A.STR1, 'Y', '是', '否') 是否本地
                                FROM DAT_TZSUP_COM A
                                WHERE A.SEQNO = '{0}'";
            DataTable dt = DbHelperOra.Query(string.Format(strSql, tbxBILLNO.Text)).Tables[0];
            ExcelHelper.ExportByWeb(dt, string.Format("新增供应商信息"), "新增供应商信息导出_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");

        }
    }
}