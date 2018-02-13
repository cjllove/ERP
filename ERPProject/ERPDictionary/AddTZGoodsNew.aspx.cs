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
using XTBase.Utilities;

namespace ERPProject.ERPDictionary
{
    public partial class AddTZGoodsNew : BillBase
    {
        private string strDocSql = "SELECT t.*,DECODE(XGTYPE,'N','新增','M','修改','禁用') XGTYPENAME,XGTYPE TYPE FROM DAT_TZGOODS_DOC t WHERE SEQNO ='{0}' AND BILLTYPE='TZL'";
        private string strComSql = @"SELECT T.*,f_getunitname(T.UNIT) UNITNAME,
                                       f_getsupname(T.SUPPLIER) SUPPLIETNAME,
                                       f_getsupname(T.PRODUCER) PRODUCERNAME,
                                       f_getcatname(T.CATID) CATIDNAME,
                                       f_getcatid0name(T.CATID0) CATID0NAME,
                                       DECODE(T.ISGZ, 'Y', '是', '否') GZNAME,
                                       DECODE(T.ISFLAG7, 'Y', '是', '否') BDNAME,
                                       T.GDSEQ GDID,DECODE(T.ISFLAG9, 'Y', '是', '否') JFNAME,
                                       DECODE(T.ISLOT, '2', '是', '否') PHNAME FROM DAT_TZGOODS_COM T WHERE SEQNO = '{0}'";
        private string strFlagSql = @"SELECT '' CODE ,'--请选择--' NAME  FROM dual
                                      union all ";
        public AddTZGoodsNew()
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
                        ButtonHidden(btnAudit, btnCancel);
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
                        ButtonHidden(btnNew, btnDel, btnSave, btnTJ, btnAddRow, btnDelRow, btnUpdate, btnDelect,btnGoods);
                        TabStrip1.ActiveTabIndex = 0;
                        ddlSPR.SelectedValue = UserAction.UserID;
                        dpkSPRQ.SelectedDate = DateTime.Now;
                        billSearch();
                        //strFlagSql += @"SELECT 'N' CODE ,'提交' NAME  FROM dual
                        //                union all 
                        //                SELECT 'S' CODE ,'已审核' NAME  FROM dual
                        //                union all
                        //                SELECT 'Y' CODE ,'已审批' NAME  FROM dual
                        //                union all
                        //                SELECT 'R' CODE ,'已驳回' NAME  FROM DUAL  ";
                        strFlagSql += @"SELECT 'N' CODE ,'提交' NAME  FROM dual
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
            PubFunc.DdlDataGet("DDL_GOODS_TYPE", ddlCATID0,dllCATID0);
            PubFunc.DdlDataGet("DDL_DOC_SUPPLIERNULL", trbSUPPLIER, tbbSUPPLIER);
            PubFunc.DdlDataGet("DDL_PRODUCER", trbPRODUCER, tbbPRODUCER);
            PubFunc.DdlDataGet("DDL_UNIT", ddlUNIT, ddlUNIT_DABZ, ddlUNIT_ZHONGBZ);
            PubFunc.DdlDataGet("DDL_UNIT", dllUNIT, dllUNIT_DABZ, dllUNIT_ZHONGBZ);
            PubFunc.DdlDataGet("DDL_TZGOODS_STATUS", ddlFLAG);
            string strSql = @"select code,'【'||code||'】'||name name,(class-1) TreeLevel,decode(islast,'Y',1,0) islast
                                    from sys_category
                                    ORDER BY code  ";
            List<CategoryTreeBean> myList = new List<CategoryTreeBean>();
            DataTable categoryTreeTable = DbHelperOra.Query(strSql).Tables[0];
            foreach (DataRow dr in categoryTreeTable.Rows)
            {
                myList.Add(new CategoryTreeBean(dr["code"].ToString(), dr["name"].ToString(), Convert.ToInt16(dr["TreeLevel"]), Convert.ToInt16(dr["islast"]) == 1));
            }
            // 绑定到下拉列表（启用模拟树功能）
            ddlCATID.EnableSimulateTree = true;
            ddlCATID.DataTextField = "Name";
            ddlCATID.DataValueField = "Id";
            ddlCATID.DataEnableSelectField = "EnableSelect";
            ddlCATID.DataSimulateTreeLevelField = "Level";
            ddlCATID.DataSource = myList;
            ddlCATID.DataBind();


            dllCATID.EnableSimulateTree = true;
            dllCATID.DataTextField = "Name";
            dllCATID.DataValueField = "Id";
            dllCATID.DataEnableSelectField = "EnableSelect";
            dllCATID.DataSimulateTreeLevelField = "Level";
            dllCATID.DataSource = myList;
            dllCATID.DataBind();

            ddlFLAG.SelectedValue = "M";
            tbxBZHL.Text = "1";
            ddlCATID0.SelectedValue = "2";
        }

        protected override void billNew()
        {
            //原单据保存判断

            PubFunc.FormDataClear(FormDoc);
            PubFunc.FormDataClear(FormMain);
            PubFunc.FormDataClear(FormAssist);
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
            ddlType.Enabled = true;
            btnGoods.Enabled = true;
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
            //表单初始化
            PubFunc.FormDataClear(FormMain);
            PubFunc.FormDataClear(FormAssist);
            dpkINRQ.SelectedDate = DateTime.Now;
            tbxBZHL.Text = "1";
            ddlUNIT_ORDER.SelectedValue = "X";
            ddlUNIT_SELL.SelectedValue = "X";
            ddlISLOT.SelectedValue = "2";
            lstTYPE.Text = "新增";
            //lstTYPE.SelectedValue = "N";
            //tbxJFDM.Enabled = false;
            btn_state(true);
            Grid1.Hidden = true;
            tbxGDID.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelect.Enabled = false;
            Button2.Hidden = true;
            TabStripMain.Hidden = false;
            WindowGoods.Hidden = false;
        }
        protected void trbSearch_TriggerClick(object sender, EventArgs e)
        {
            dataSearch();
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            dataSearch();
        }
        private static DataTable dtData;
        private void dataSearch()
        {
            int total = 0;
            string msg = "";
            NameValueCollection nvc = new NameValueCollection();
            if (trbSearch.Text.Length > 0) nvc.Add("CX", trbSearch.Text.Trim());
            dtData = GetGoodsList(Grid1.PageIndex, Grid1.PageSize, nvc, ref total, ref msg);
            Grid1.RecordCount = total;
           // Grid1.RecordCount = 100;
            Grid1.DataSource = dtData;
            Grid1.DataBind();
        }
        protected void ddlISZS_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataSearch();
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
                                strSearch += string.Format(" AND (sp.GDSEQ LIKE '%{0}%' OR sp.GDNAME LIKE '%{0}%' OR sp.HISCODE LIKE '%{0}%' OR sp.HISNAME LIKE '%{0}%' OR sp.BARCODE  LIKE '%{0}%' OR  sp.BAR3 LIKE '%{0}%' OR sp.ZJM  LIKE '%{0}%' OR F_GETPRODUCERNAME(SP.PRODUCER) LIKE '%{0}%')", condition);
                                break;
                        }
                    }
                }
            }
            string strSql = "";
            if (ddlType.SelectedValue == "D")
            {
                strSql = @"SELECT SP.GDSEQ,SP.GDID,F_GETHISINFO（SP.GDSEQ，'GDNAME') GDNAME,SP.BARCODE,E.NAME CATID0NAME,B.NAME CATID0NAME_F,F_GETHISINFO（SP.GDSEQ，'GDSPEC') GDSPEC,D.NAME UNITNAME,SP.BZHL,
                               ROUND(SP.HSJJ,4) HSJJ,ROUND(SP.LSJ,4) LSJ,C.SUPNAME,SP.ZPBH,S.NAME FLAG_CN,SP.PIZNO,f_getsupname(SP.PRODUCER) PRODUCERNAME,decode(sp.isflag7,'Y','是','否') ISNEW_CN,decode(sp.isflag3,'Y','是','否') ISZSNAME
                          from DOC_GOODS SP,
                               SYS_CATEGORY B,
                               DOC_SUPPLIER C,
                               DOC_GOODSUNIT D,
                               doc_goodstype e,
                               (SELECT CODE, NAME FROM SYS_CODEVALUE WHERE TYPE = 'GOODS_STATUS') S
                         WHERE SP.CATID=B.CODE(+) AND SP.FLAG=S.CODE AND SP.SUPPLIER=C.SUPID(+) AND SP.UNIT = D.CODE(+) and SP.CATID0 = e.code(+) AND SP.FLAG<>'E'
                                AND SP.ISDELETE='N' ";

            }
            else
            {
                strSql = @"SELECT SP.GDSEQ,SP.GDID,F_GETHISINFO（SP.GDSEQ，'GDNAME') GDNAME,SP.BARCODE,E.NAME CATID0NAME,B.NAME CATID0NAME_F,F_GETHISINFO（SP.GDSEQ，'GDSPEC') GDSPEC,D.NAME UNITNAME,SP.BZHL,
                               ROUND(SP.HSJJ,4) HSJJ,ROUND(SP.LSJ,4) LSJ,SP.ZPBH,S.NAME FLAG_CN,SP.PIZNO,f_getsupname(SP.PRODUCER) PRODUCERNAME,decode(sp.isflag7,'Y','是','否') ISNEW_CN,f_getsupname(T.supid) supname,T.supid，decode(sp.isflag3,'Y','是','否') ISZSNAME
                          from DOC_GOODS SP,
                               SYS_CATEGORY B,
                               DOC_SUPPLIER C,
                               DOC_GOODSUNIT D,
                               doc_goodstype e,
                               (SELECT CODE, NAME FROM SYS_CODEVALUE WHERE TYPE = 'GOODS_STATUS') S,
                               doc_goodssup T
                         WHERE SP.CATID=B.CODE(+) AND SP.FLAG=S.CODE AND SP.SUPPLIER=C.SUPID(+) AND SP.UNIT = D.CODE(+) and SP.CATID0 = e.code(+) AND SP.FLAG<>'E'
                                AND SP.ISDELETE='N' and sp.gdseq=T.gdseq(+) ";
            }
            if (!string.IsNullOrWhiteSpace(trbGdSpec.Text))
            {
                strSql += string.Format(" AND SP.GDSPEC LIKE '%{0}%'", trbGdSpec.Text);
            }
            if (!string.IsNullOrEmpty(ddlISZS.SelectedValue))
            {
                strSql += string.Format(" AND SP.ISFLAG3='" + ddlISZS.SelectedValue + "' ");
            }
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
        protected void GridGoods_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            TabStripMain.ActiveTabIndex = 0;
            string seq = Grid1.Rows[e.RowIndex].Values[1].ToString();

            DataTable dt = DbHelperOra.Query("select * from doc_goods where gdseq='" + seq + "'").Tables[0];
            PubFunc.FormDataSet(FormMain, dt.Rows[0]);
            PubFunc.FormDataSet(FormAssist, dt.Rows[0]);
            PubFunc.FormDataSet(Form3, dt.Rows[0]);//原基本信息
            PubFunc.FormDataSet(Form4, dt.Rows[0]);//原辅助信息
            Form3.Enabled = false;//原基本信息不可修改
            Form4.Enabled = false;//原辅助信息不可修改
            switch (ddlType.SelectedValue) //商品当前状态
            {
                case "N":
                    lstTYPE.SelectedValue = "N";
                    dllTYPE.SelectedValue = "N";
                    break;
                case "M":
                    lstTYPE.SelectedValue = "M";
                    dllTYPE.SelectedValue = "M";
                    break;
                case "D":
                    lstTYPE.SelectedValue = "D";
                    dllTYPE.SelectedValue = "D";
                    break;
                default:
                    Alert.Show("请选择单据类型！", "提示信息", MessageBoxIcon.Warning);
                    break;
            }
            string sup = Grid1.Rows[e.RowIndex].Values[2].ToString();
            trbSUPPLIER.SelectedValue = sup;
            hfdSup.Text = sup;
            ddlISFLAG7.Enabled = false;
            btn_state(true);

            dllCATID.Enabled = true;
            txtGDSPEC.Enabled = true;
            dllUNIT.Enabled = true;
            tbbPRODUCER.Enabled = true;


            PubFunc.FormLock(FormAssist, false);
            tbxGDID.Enabled = false;
            if (DbHelperOra.Exists("SELECT 1 FROM  DAT_GOODSSTOCK B WHERE B.GDSEQ = '" + seq + "' AND B.KCSL > 0"))
            {
                nbbHSJJ.Enabled = false;
            }
        }
        protected void Grid1_PageIndexChange(object sender, GridPageEventArgs e)
        {
            Grid1.PageIndex = e.NewPageIndex;
            dataSearch();
        }
        protected void btn_state(bool Flag)
        {
            //按钮状态
            ddlISLOT.Enabled = Flag;
            tbsGDNAME.Enabled = Flag;
            tbxGDSPEC.Enabled = Flag;
            nbbBARCODE.Enabled = Flag;
            ddlUNIT.Enabled = Flag;
            nbbHSJJ.Enabled = Flag;
            //nbbBHSJJ.Enabled = Flag;
            trbPRODUCER.Enabled = Flag;
            trbSUPPLIER.Enabled = Flag;
            tbxPIZNO.Enabled = Flag;
            ddlUNIT_DABZ.Enabled = Flag;
            nbbBARCODE_DABZ.Enabled = Flag;
            ddlUNIT_ZHONGBZ.Enabled = Flag;
            nbbNUM_ZHONGBZ.Enabled = Flag;
            nbbBARCODE_ZHONGBZ.Enabled = Flag;
            nbbNUM_DABZ.Enabled = Flag;
            ddlCATID.Enabled = Flag;
            ddlISFLAG7.Enabled = Flag;
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
            for (int i = rowId.Length-1; i >=0; i--)
            {
                GridGoods.DataSource = DeleteRowByID(rowId[i]);
                GridGoods.DataBind();
            }
            
        }

        // 根据行ID来删除行数据
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
                          from DAT_TZGOODS_DOC A WHERE 1=1 ";
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
                flag = DbHelperOra.GetSingle("SELECT XGTYPE FROM DAT_TZGOODS_DOC T WHERE T.SEQNO='" + tbxSEQNO.Text + "'").ToString();
            }
            catch
            {
                Alert.Show("单据号存在问题！请检测！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            DataTable dt = DbHelperOra.Query("SELECT GDSEQ,SUPPLIER FROM DAT_TZGOODS_COM T WHERE T.SEQNO='" + tbxSEQNO.Text + "'").Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                if (flag.Trim() == "N")
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        if (!DbHelperOra.Exists("SELECT 1 FROM DOC_GOODS T WHERE T.GDSEQ='" + row["GDSEQ"].ToString() + "'"))
                        {
                            cmdList.Add(new CommandInfo("INSERT INTO DOC_GOODS SELECT * FROM DOC_GOODSTEMP WHERE GDSEQ='" + row["GDSEQ"].ToString() + "'", null));
                            cmdList.Add(new CommandInfo(string.Format(@"insert into doc_goodssup
                                                      (custid, gdseq, supid, supname,TYPE,ORDERSORT,str3)
                                                    values
                                                      (f_getpara('USERCODE'), '{0}', '{1}',f_getsupname('{1}'),'Z','Y',SUBSTR(nvl(f_getpara('FDSMODE'),'XSJ'),1,1))", row["GDSEQ"].ToString(), row["SUPPLIER"].ToString()), null));
                        }
                    }
                }
                else if (flag.Trim() == "M")
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        if (DbHelperOra.Exists("SELECT 1 FROM DOC_GOODS T WHERE T.GDSEQ='" + row["GDSEQ"].ToString() + "'"))
                        {
                            cmdList.Add(new CommandInfo(string.Format("UPDATE DOC_GOODS SET GDSEQ='{0}'||'停'|| TO_CHAR(SYSDATE, 'yyyy-MM-dd hh:mm:ss'),GDID='{0}'||'停'|| TO_CHAR(SYSDATE, 'yyyy-MM-dd hh:mm:ss'),FLAG='E' WHERE GDSEQ='{0}'", row["GDSEQ"].ToString()), null));
                            cmdList.Add(new CommandInfo("INSERT INTO DOC_GOODS SELECT * FROM DOC_GOODSTEMP WHERE GDSEQ='" + row["GDSEQ"].ToString() + "'", null));
                            cmdList.Add(new CommandInfo(string.Format("UPDATE DOC_GOODSTEMP SET GDSEQ='{0}'||'停'|| TO_CHAR(SYSDATE, 'yyyy-MM-dd hh:mm:ss'),GDID='{0}'||'停'|| TO_CHAR(SYSDATE, 'yyyy-MM-dd hh:mm:ss'),FLAG='E' WHERE GDSEQ='{0}'", row["GDSEQ"].ToString()), null));

                            //先判断sup是否为空  山东威高医药有限公司
                            string sup = "";
                            if (DbHelperOra.GetSingle(string.Format("select t.str9 from DOC_GOODSTEMP t where t.gdseq='{0}'", row["GDSEQ"].ToString())) == null)
                            {
                                //Alert.Show("单据状态存在问题！供应商信息为空！", "消息提示", MessageBoxIcon.Warning);
                                //return;
                            }
                            else
                            {
                                sup = DbHelperOra.GetSingle(string.Format("select t.str9 from DOC_GOODSTEMP t where t.gdseq='{0}'", row["GDSEQ"].ToString())).ToString();
                            }
                            //string sup = DbHelperOra.GetSingle(string.Format("select t.str9 from DOC_GOODSTEMP t where t.gdseq='{0}'", row["GDSEQ"].ToString())).ToString();


                            string sup_sql = @"declare
                                                  ln_num number;
                                                begin
                                                  select count(1) into ln_num from DOC_GOODS T, doc_goodssup A where T.GDSEQ=A.gdseq AND T.GDSEQ = '{0}' AND A.SUPID='{2}';
                                                  if ln_num > 0 then
                                                    update doc_goodssup s
                                                       set s.supid = '{1}', s.supname = f_getsupname('{1}')
                                                     where s.gdseq = '{0}' AND S.SUPID='{2}';
                                                  else
                                                    insert into doc_goodssup
                                                      (custid, gdseq, supid, supname,TYPE,ORDERSORT,str3)
                                                    values
                                                      (f_getpara('USERCODE'), '{0}', '{1}',f_getsupname('{1}'),'Z','Y',SUBSTR(nvl(f_getpara('FDSMODE'),'XSJ'),1,1));
                                                  end if;
                                                end;";
                            cmdList.Add(new CommandInfo(string.Format(sup_sql, row["GDSEQ"].ToString(), row["SUPPLIER"].ToString(), sup), null));
                            cmdList.Add(new CommandInfo("UPDATE DAT_GOODSSTOCK SET kchsjj=(select a.hsjj from doc_goods a where a.gdseq='" + row["GDSEQ"].ToString() + "') WHERE GDSEQ='" + row["GDSEQ"].ToString() + "'", null));
                        }
                    }
                }
                else if (flag.Trim() == "D")
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        if (DbHelperOra.Exists("SELECT 1 FROM DOC_GOODS T WHERE T.GDSEQ='" + row["GDSEQ"].ToString() + "'"))
                        {
                            cmdList.Add(new CommandInfo("UPDATE DOC_GOODS T SET T.FLAG='E' WHERE T.GDSEQ='" + row["GDSEQ"].ToString() + "'", null));
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
                    Alert.Show("商品信息审批成功！");
                    DbHelperOra.ExecuteSql(string.Format("UPDATE DAT_TZGOODS_DOC SET FLAG = 'Y',SPR='{1}',SPRQ=SYSDATE,STR1='N'  WHERE FLAG= 'N' AND SEQNO = '{0}'", tbxSEQNO.Text, UserAction.UserID));
                    btnCancel.Enabled = false;
                    OperLog("商品管理", "审核单据【" + tbxBILLNO.Text + "】");
                    billOpen(tbxBILLNO.Text);
                }
            }

        }
        protected void btnClose_Click(object sender, EventArgs e)
        {
            WindowGoods.Hidden = true;
        }
        protected void btnRejectSubmit_Click(object sender, EventArgs e)
        {
            if (ddlType.SelectedValue != "D")
            {
                if (PubFunc.FormDataCheck(FormMain).Length > 0) return;
            }

            //增加自动生成的编码
            if ((",M,R").IndexOf(ddlFLAG.SelectedValue) < 1)
            {
                WindowGoods.Hidden = true;
                return;

            }
            if (tbxGDID.Text.Trim().Length > 1)
            {
                if (ddlType.SelectedValue == "N")
                {
                    if (DbHelperOra.Exists("select 1 from doc_goods where gdseq='" + tbxGDID.Text + "'"))
                    {
                        Alert.Show("商品新增信息编码重复！");
                        return;
                    }
                }
                if (DbHelperOra.Exists("select 1 from doc_goodstemp where gdseq='" + tbxGDID.Text + "'"))
                {
                    DbHelperOra.ExecuteSql("DELETE FROM DOC_GOODSTEMP WHERE GDSEQ='" + tbxGDID.Text + "'");
                }
            }
            if (ddlType.SelectedValue == "N" && tbxGDID.Text.Trim().Length < 1)
            {
                string catid = string.IsNullOrWhiteSpace(ddlCATID0.SelectedValue) ? "2" : ddlCATID0.SelectedValue;
                tbxGDID.Text = DbHelperOra.GetSingle("SELECT F_GET_BGGDSEQ('" + catid + "') FROM DUAL").ToString();
                if (DbHelperOra.Exists("select 1 from doc_goodstemp where gdseq='" + tbxGDID.Text + "'"))
                {
                    tbxGDID.Text = DbHelperOra.GetSingle("SELECT F_GET_BGGDSEQTEMP('" + catid + "') FROM DUAL").ToString();
                }
            }
            else if (ddlType.SelectedValue == "M" && tbxGDID.Text.Trim().Length > 0)
            {
                if (!DbHelperOra.Exists("select 1 from doc_goods where gdseq='" + tbxGDID.Text + "' AND HSJJ=" + nbbHSJJ.Text + ""))
                {
                    if (DbHelperOra.Exists("SELECT 1 FROM DAT_GOODSSTOCK B WHERE B.GDSEQ = '" + tbxGDID.Text + "' AND B.KCSL > 0"))
                    {
                        Alert.Show("库存数量大于0，不允许更改价格!", "操作提示", MessageBoxIcon.Warning);
                        return;
                    }
                }
                if (DbHelperOra.Exists("select 1 from doc_goodstemp where gdseq='" + tbxGDID.Text + "'"))
                {
                    DbHelperOra.ExecuteSql("DELETE FROM DOC_GOODSTEMP WHERE GDSEQ='" + tbxGDID.Text + "'");
                }

            }
            else if (ddlType.SelectedValue == "D")
            {
                for (int i = 0; i < Grid1.SelectedRowIndexArray.Length; i++)
                {
                    if (DbHelperOra.Exists("select 1 from doc_goodstemp where gdseq='" + Grid1.Rows[Grid1.SelectedRowIndexArray[i]].DataKeys[0] + "'"))
                    {
                        DbHelperOra.ExecuteSql("DELETE FROM DOC_GOODSTEMP WHERE GDSEQ='" + Grid1.Rows[Grid1.SelectedRowIndexArray[i]].DataKeys[0] + "'");
                    }
                    if (DbHelperOra.Exists("select 1 from doc_goods where gdseq='" + Grid1.Rows[Grid1.SelectedRowIndexArray[i]].DataKeys[0] + "'"))
                    {
                        DbHelperOra.ExecuteSql("INSERT INTO DOC_GOODSTEMP SELECT * FROM DOC_GOODS where gdseq='" + Grid1.Rows[Grid1.SelectedRowIndexArray[i]].DataKeys[0] + "' ");
                    }
                    hfdTEMP.Text += Grid1.Rows[Grid1.SelectedRowIndexArray[i]].DataKeys[0] + ",";
                }
                GridInIt();
                WindowGoods.Hidden = true;
                return;
            }

            if (tbxGDID.Text.Trim().Length < 1)
            {
                Alert.Show("输入商品编码不符合规范!", "操作提示", MessageBoxIcon.Warning);
                return;
            }

            //if ((",N,Y").IndexOf(ddlFLAG.SelectedValue) < 1)
            //{
            //    Alert.Show("当前商品为【" + ddlFLAG.SelectedText + "】状态，不允许进行编辑！", "操作提示", MessageBoxIcon.Warning);
            //    return;
            //}

            //增加单位的验证
            if (ddlUNIT_ORDER.SelectedValue == "D" && (PubFunc.StrIsEmpty(nbbNUM_DABZ.Text) || nbbNUM_DABZ.Text.Trim().ToString()=="0"))
            {
                Alert.Show("订货单位为大包装,请维护大包装数量!", "操作提示", MessageBoxIcon.Warning);
                return;
            }
            if (ddlUNIT_ORDER.SelectedValue == "D" && PubFunc.StrIsEmpty(ddlUNIT_DABZ.SelectedValue))
            {
                Alert.Show("订货单位为大包装,请维护大包装单位!", "操作提示", MessageBoxIcon.Warning);
                return;
            }
            if (ddlUNIT_ORDER.SelectedValue == "Z" && (PubFunc.StrIsEmpty(nbbNUM_ZHONGBZ.Text)|| nbbNUM_ZHONGBZ.Text.Trim().ToString() == "0"))
            {
                Alert.Show("订货单位为中包装,请维护中包装数量!", "操作提示", MessageBoxIcon.Warning);
                return;
            }
            if (ddlUNIT_ORDER.SelectedValue == "Z" && PubFunc.StrIsEmpty(ddlUNIT_ZHONGBZ.SelectedValue))
            {
                Alert.Show("订货单位为中包装,请维护中包装单位!", "操作提示", MessageBoxIcon.Warning);
                return;
            }

            if (ddlUNIT_SELL.SelectedValue == "D" && (PubFunc.StrIsEmpty(nbbNUM_DABZ.Text) || nbbNUM_DABZ.Text.Trim().ToString() == "0"))
            {
                Alert.Show("出库单位为大包装,请维护大包装数量!", "操作提示", MessageBoxIcon.Warning);
                return;
            }
            if (ddlUNIT_SELL.SelectedValue == "D" && PubFunc.StrIsEmpty(ddlUNIT_DABZ.SelectedValue))
            {
                Alert.Show("出库单位为大包装,请维护大包装单位!", "操作提示", MessageBoxIcon.Warning);
                return;
            }
            if (ddlUNIT_SELL.SelectedValue == "Z" && (PubFunc.StrIsEmpty(nbbNUM_ZHONGBZ.Text) || nbbNUM_ZHONGBZ.Text.Trim().ToString() == "0"))
            {
                Alert.Show("出库单位为中包装,请维护中包装数量!", "操作提示", MessageBoxIcon.Warning);
                return;
            }
            if (ddlUNIT_SELL.SelectedValue == "Z" && PubFunc.StrIsEmpty(ddlUNIT_ZHONGBZ.SelectedValue))
            {
                Alert.Show("出库单位为中包装,请维护中包装单位!", "操作提示", MessageBoxIcon.Warning);
                return;
            }
            if (!PubFunc.StrIsEmpty(nbbHSJJ.Text))
            {
                if (Convert.ToDecimal(nbbHSJJ.Text) < 0)
                {
                    Alert.Show("含税进价不能小于0", "操作提示", MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                return;
            }


            //如果是高值商品，订货单位和出库单位要相等
            if (ckbISGZ.Checked)
            {
                if (ddlUNIT_ORDER.SelectedValue != "X" || ddlUNIT_SELL.SelectedValue != "X")
                {
                    Alert.Show("高值商品，订货单位和出库单位必须为【最小包装单位】不一致，请调整!", "消息提示", MessageBoxIcon.Warning);
                    return;
                }
                //不允许是定数商品
                if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DOC_GOODSCFG WHERE NVL(NUM1,0)> 0 AND NVL(DSNUM,0) > 0 AND GDSEQ = '{0}'", tbxGDID.Text)))
                {
                    Alert.Show("商品为定数商品，不允许设置为高值", "消息提示", MessageBoxIcon.Warning);
                    return;
                }
                //必须无库存时可设置为高值商品
                if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_GOODSSTOCK A,DOC_GOODS B WHERE A.KCSL > 0 AND A.GDSEQ = '{0}' AND A.GDSEQ =B.GDSEQ AND B.ISGZ = 'N'", tbxGDID.Text)))
                {
                    Alert.Show("商品存在未使用库存，不允许设置为高值商品", "消息提示", MessageBoxIcon.Warning);
                    return;
                }
            }

            if (ckbISFLAG3.Checked)
            {
                if (DbHelperOra.Exists(string.Format("select * from dat_goodsstock where gdseq='{0}' and kcsl>0", tbxGDID.Text.Trim())))
                {
                    Alert.Show("该商品被设置为【直送商品】，但它下面有库存，无法执行操作。", "操作提示", MessageBoxIcon.Warning);
                    return;
                }
            }
            hfdGDSEQ.Text = tbxGDID.Text;
            MyTable mtType = new MyTable("DOC_GOODSTEMP");
            mtType.ColRow = PubFunc.FormDataHT(FormMain);
            Hashtable htAssist = PubFunc.FormDataHT(FormAssist);
            foreach (string key in htAssist.Keys)
            {
                mtType.ColRow.Add(key, htAssist[key]);
            }
            //数据处理 
            mtType.ColRow.Remove("ISNEW");

            if (DbHelperOra.Exists(string.Format("select 1 from doc_goodstemp where GDSEQ='{0}'", tbxGDID.Text.Trim())))
            {
                Alert.Show(string.Format("您输入的商品编码【{0}】已存在，请重新输入！", tbxGDID.Text), MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(trbSUPPLIER.SelectedValue))
            {
                mtType.ColRow["SUPPLIER"] = "00002";
            }
            mtType.ColRow["STR9"] = hfdSup.Text;
            mtType.ColRow["GDSEQ"] = mtType.ColRow["GDID"];
            mtType.ColRow["ISFLAG7"] = ddlISFLAG7.SelectedValue;
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
            hfdTEMP.Text += tbxGDID.Text + ",";
            hfdTEMP.Text = hfdTEMP.Text.TrimEnd(',').Replace(",", "','");
            GridGoods.DataSource = DbHelperOra.Query(@"SELECT T.*,f_getunitname(T.UNIT) UNITNAME,
                                       f_getsupname(T.SUPPLIER) SUPPLIETNAME,
                                       f_getsupname(T.PRODUCER) PRODUCERNAME,
                                       f_getcatname(T.CATID) CATIDNAME,
                                       f_getcatid0name(T.CATID0) CATID0NAME,
                                       DECODE(T.ISGZ, 'Y', '是', '否') GZNAME,
                                       DECODE(T.ISFLAG7, 'Y', '是', '否') BDNAME,T.GDSEQ GDID,DECODE(T.ISFLAG9, 'Y', '是', '否') JFNAME,
                                       DECODE(T.ISLOT, '2', '是', '否') PHNAME FROM DOC_GOODSTEMP T WHERE T.GDSEQ IN ('" + hfdTEMP.Text + "')").Tables[0];
            GridGoods.DataBind();
            ddlUNIT.Enabled = false;
            ddlCATID.Enabled = false;
            ddlISFLAG7.Enabled = false;
            tbxGDID.Enabled = false;
            tbxGDSPEC.Enabled = false;
            ddlUNIT.Enabled = false;
            //nbbBHSJJ.Enabled = false;
            trbPRODUCER.Enabled = false;
            tbxPIZNO.Enabled = false;
            hfdTEMP.Text = "";
            WindowGoods.Hidden = true;
            trbSearch.Text = string.Empty;
            Grid1.DataSource = null;
            Grid1.DataBind();
            dataSearch();//20170414

        }

        protected override void listRow_DoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            billOpen(GridList.Rows[e.RowIndex].Values[1].ToString());
           // btnGoods.Hidden = true;//审批不需要追加按钮
        }

        protected void goodsRow_DoubleClick(object sender, GridRowClickEventArgs e)
        {
            TabStripMain.Hidden = false;
            TabStrip2.Hidden = true;
            Grid1.Hidden = true;
            DataTable dt1 = DbHelperOra.Query("SELECT * FROM DOC_GOODSTEMP t where t.gdseq='" + GridGoods.Rows[e.RowIndex].DataKeys[2] + "'").Tables[0];
            if (ddlType.SelectedValue == "D")
            {
                if (dt1 != null && dt1.Rows.Count > 0)
                {
                    //formGet(FormMain, dt1.Rows[0]);
                    //formGet(FormAssist, dt1.Rows[0]);
                    PubFunc.FormDataSet(FormMain, dt1.Rows[0]);
                    PubFunc.FormDataSet(FormAssist, dt1.Rows[0]);
                }
                else
                {
                    PubFunc.FormDataSet(FormMain, GridGoods.Rows[e.RowIndex]);
                    PubFunc.FormDataSet(FormAssist, GridGoods.Rows[e.RowIndex]);
                }
                btn_state(true);
                tbxGDID.Enabled = false;
                if (tbxSEQNO.Text.Length > 0)
                {
                    if (("M,R").IndexOf(DbHelperOra.GetSingle("SELECT T.FLAG FROM DAT_TZGOODS_DOC T WHERE T.SEQNO='" + tbxSEQNO.Text + "'").ToString()) < 0)
                    {
                        PubFunc.FormLock(FormMain, true);
                        PubFunc.FormLock(FormAssist, true);
                    }
                }
            }
            else if (ddlType.SelectedValue == "N")
            {
                TabStripMain.Hidden = false;
                TabStrip2.Hidden = true;
                Grid1.Hidden = true;
                if (dt1 != null && dt1.Rows.Count > 0)
                {
                    PubFunc.FormDataSet(FormMain, dt1.Rows[0]);
                    PubFunc.FormDataSet(FormAssist, dt1.Rows[0]);
                    ddlCATID.Enabled = true;
                    ddlISFLAG7.Enabled = true;
                    tbxGDSPEC.Enabled = true;
                    ddlUNIT.Enabled = true;
                    trbPRODUCER.Enabled = true;
                    tbxPIZNO.Enabled = true;
                    //formGet(FormMain, dt1.Rows[0]);
                    //formGet(FormAssist, dt1.Rows[0]);
                }
                else
                {
                    PubFunc.FormDataSet(FormMain, GridGoods.Rows[e.RowIndex]);
                    PubFunc.FormDataSet(FormAssist, GridGoods.Rows[e.RowIndex]);
                }
                Button2.Hidden = true;
                //PubFunc.FormLock(FormMain, false);
               // PubFunc.FormLock(FormAssist, false);
            }
            else if (ddlType.SelectedValue == "M")
            {
                TabStrip2.Hidden = false;
                TabStripMain.Hidden = false;
                TabStrip2.Hidden = false;
                Grid1.Hidden = true;
                DataTable dt = DbHelperOra.Query("SELECT * FROM DOC_GOODS T WHERE T.GDSEQ LIKE '%" + GridGoods.Rows[e.RowIndex].DataKeys[2] + "%' ORDER BY T.GDSEQ desc").Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    //formGet(Form3, dt.Rows[0]);
                    //formGet(Form4, dt.Rows[0]);
                    PubFunc.FormDataSet(FormMain, dt1.Rows[0]);
                    PubFunc.FormDataSet(FormAssist, dt1.Rows[0]);
                    PubFunc.FormDataSet(Form3, dt.Rows[0]);//原基本信息
                    PubFunc.FormDataSet(Form4, dt.Rows[0]);//原辅助信息

                    ddlUNIT_ZHONGBZ.Enabled = true;


                }
                else
                {
                    PubFunc.FormDataSet(FormMain, GridGoods.Rows[e.RowIndex]);
                    PubFunc.FormDataSet(FormAssist, GridGoods.Rows[e.RowIndex]);
                    PubFunc.FormDataSet(Form3, GridGoods.Rows[e.RowIndex]);
                    PubFunc.FormDataSet(Form4, GridGoods.Rows[e.RowIndex]);
                }

                PubFunc.FormLock(Form3, true);
                PubFunc.FormLock(Form4, true);
            }
            WindowGoods.Hidden = false;
            Grid1.Hidden = true;
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
                        txt.Text = row[txt.ID.Substring(3)].ToString();
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
                            ddl.SelectedValue = row[ddl.ID.Substring(3)].ToString();
                        }
                    }
                    else if (item is FineUIPro.CheckBox)
                    {
                        FineUIPro.CheckBox cbx = (FineUIPro.CheckBox)item;
                        if (row[cbx.ID.Substring(3)].ToString() == "Y")
                        {
                            cbx.Checked = true;
                        }
                        else
                        {
                            cbx.Checked = false;
                        }
                    }
                    else if (item is FineUIPro.NumberBox)
                    {
                        FineUIPro.NumberBox num = (FineUIPro.NumberBox)item;
                        num.Text = row[num.ID.Substring(3)].ToString();
                    }
                    else if (item is FineUIPro.DatePicker)
                    {
                        FineUIPro.DatePicker date = (FineUIPro.DatePicker)item;
                        date.Text = row[date.ID.Substring(3)].ToString();
                    }
                }
            }
        }

        //private void formGet(DataRow row)
        //{
        //    #region MyRegion
        //    //txtGDID.Text = row["GDID"].ToString();
        //    //dllTYPE.SelectedValue = "M";
        //    //dllCATID.SelectedValue = row["CATID"].ToString();
        //    //dllCATID0.SelectedValue = row["CATID0"].ToString();
        //    //toxGDNAME.Text = row["GDNAME"].ToString();
        //    //txtZJM.Text = row["ZJM"].ToString();
        //    //txtNAMEJC.Text = row["NAMEJC"].ToString();
        //    //dllISFLAG7.SelectedValue = row["GDID"].ToString();
        //    //txtGDSPEC.Text = row["GDSPEC"].ToString();
        //    //dllUNIT.SelectedValue = row["UNIT"].ToString();
        //    //txtGDMODE.Text = row["GDMODE"].ToString();
        //    //numHSJJ.Text = row["HSJJ"].ToString();
        //    //dllISLOT.SelectedValue = row["ISLOT"].ToString();
        //    //tbbPRODUCER.SelectedValue = row["PRODUCER"].ToString();
        //    //tbbSUPPLIER.SelectedValue = row["SUPPLIER"].ToString();
        //    //dllUNIT_DABZ.SelectedValue = row["UNIT_DABZ"].ToString();
        //    //numNUM_DABZ.Text = row["NUM_DABZ"].ToString();
        //    //dllUNIT_ZHONGBZ.SelectedValue = row["UNIT_ZHONGBZ"].ToString();
        //    //numNUM_ZHONGBZ.Text = row["NUM_ZHONGBZ"].ToString();
        //    //if (row["ISGZ"].ToString() == "Y")
        //    //{
        //    //    coxISGZ.Checked = true;
        //    //}
        //    //else
        //    //{
        //    //    coxISGZ.Checked = false;
        //    //}
        //    #endregion

        //}

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
                string flag = DbHelperOra.GetSingle("select XGTYPE from DAT_TZGOODS_DOC t where t.billno='" + strBillno + "'").ToString();
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
            }
            if (ddlFLAG.SelectedValue == "M")
            {
                btnAddRow.Enabled = false;
                btnUpdate.Enabled = false;
                btnDelect.Enabled = false;
                btnTJ.Enabled = true;
                btnGoods.Enabled = true;
                string flag = DbHelperOra.GetSingle("select XGTYPE from DAT_TZGOODS_DOC t where t.billno='" + strBillno + "'").ToString();
                if (flag == "N")
                {
                    btnAddRow.Enabled = true;
                   // btnGoods.Hidden = true;
                }
                else if (flag == "M")
                {
                    btnUpdate.Enabled = true;
                  //  btnGoods.Hidden = true;
                }
                else if (flag == "D")
                {
                    btnDelect.Enabled = true;
                }
                btnDelRow.Enabled = true;
                btnSave.Enabled = true;
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
            }
           
        }

        protected override void billSave()
        {
            save();
        }

        private void save(string flag="N")
        {
            #region 数据有效性验证
            if ((",M,R").IndexOf(ddlFLAG.SelectedValue) < 0)
            {
                Alert.Show("非新单不能保存！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            if (PubFunc.FormDataCheck(FormDoc).Length > 0) return;
            //List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
            //List<Dictionary<string, object>> newDict1 = GridGoods.Rows.Count();
            if (GridGoods.Rows.Count() == 0)
            {
                Alert.Show("请先进行数据维护，再进行保存操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            //验证单据信息
            if (DbHelperOra.Exists("SELECT 1 FROM DAT_TZGOODS_DOC where seqno = '" + tbxBILLNO.Text + "'") && tbxBILLNO.Enabled)
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
                string flg = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M') FROM DAT_TZGOODS_DOC WHERE SEQNO='{0}'", tbxBILLNO.Text));
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

            MyTable mtType = new MyTable("DAT_TZGOODS_DOC");
            mtType.ColRow = PubFunc.FormDataHT(FormDoc);
            mtType.ColRow["SEQNO"] = tbxBILLNO.Text;
            mtType.ColRow.Add("SUBNUM", GridGoods.Rows.Count);
            mtType.ColRow.Add("XGTYPE", ddlType.SelectedValue);
            mtType.ColRow.Add("BILLTYPE", "TZL");
            mtType.ColRow["FLAG"] = "M";
            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtTypeMx = new MyTable("DAT_TZGOODS_COM");
            //先删除单据信息在插入
            cmdList.Add(new CommandInfo("delete DAT_TZGOODS_DOC where seqno='" + tbxBILLNO.Text + "'", null));//删除单据台头
            cmdList.Add(new CommandInfo("delete DAT_TZGOODS_COM where seqno='" + tbxBILLNO.Text + "'", null));//删除单据明细
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
                if(flag == "N")
                    Alert.Show("商品信息保存成功！");
                OperLog("商品管理", "修改单据【" + tbxBILLNO.Text + "】");
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
            DbHelperOra.ExecuteSql("Delete from DAT_TZGOODS_DOC t WHERE T.SEQNO ='" + tbxBILLNO.Text.Trim() + "'");
            DbHelperOra.ExecuteSql("Delete from DAT_TZGOODS_COM t WHERE T.SEQNO ='" + tbxBILLNO.Text.Trim() + "'");
            Alert.Show("单据删除成功!");
            OperLog("商品管理", "删除单据【" + tbxSEQNO.Text + "】");
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
            string flg = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M') FROM DAT_TZGOODS_DOC WHERE SEQNO='{0}'", tbxSEQNO.Text));
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
            DataTable dt = DbHelperOra.Query(string.Format("SELECT A.GDSEQ,A.GDNAME,A.SEQNO  FROM DAT_TZGOODS_COM A, DAT_TZGOODS_DOC B WHERE GDSEQ IN(SELECT GDSEQ FROM DAT_TZGOODS_COM A WHERE SEQNO ='{0}' )AND A.SEQNO NOT LIKE '{0}'AND B.FLAG='N'AND A.SEQNO=B.SEQNO", tbxSEQNO.Text)).Tables[0];
            if (dt.Rows.Count > 0)
            {
                msg = "商品【[" + dt.Rows[0]["GDSEQ"] + "]" + dt.Rows[0]["GDNAME"] + "】被单据【" + dt.Rows[0]["SEQNO"] + "】使用,请维护！";
                Alert.Show(msg, "提示信息", MessageBoxIcon.Warning);
                return;
            }
            if (DbHelperOra.ExecuteSql(string.Format("UPDATE DAT_TZGOODS_DOC SET FLAG = 'N' WHERE FLAG= 'M' AND SEQNO = '{0}'", tbxSEQNO.Text)) > 0)
            {
                Alert.Show("商品信息提交成功！");
                OperLog("商品管理", "提交单据【" + tbxSEQNO.Text + "】");
                billOpen(tbxSEQNO.Text);
                btnPrint.Enabled = true;
                btnGoods.Enabled = false;
                btnDel.Enabled = false;//提交后删除单据按钮为灰    
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
            if (DbHelperOra.ExecuteSql(string.Format("UPDATE DAT_TZGOODS_DOC SET FLAG = 'F' WHERE FLAG= 'N' AND SEQNO = '{0}'", tbxSEQNO.Text)) > 0)
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
        protected void ddlCATID_SelectedIndexChanged(object sender, EventArgs e)
        {
            string strCATID0, strCATID, strSQL;
            strCATID = ((FineUIPro.DropDownList)sender).SelectedValue;
            if (PubFunc.StrIsEmpty(strCATID)) return;

            strSQL = "select type from sys_category where code='" + strCATID + "'";
            strCATID0 = DbHelperOra.GetSingle(strSQL).ToString();

            ddlCATID0.SelectedValue = strCATID0;
        }
        protected void tbsGDNAME_TextChanged(object sender, EventArgs e)
        {
            tbxNAMEJC.Text = tbsGDNAME.Text;
            tbxZJM.Text = Doc.GetPinYinFirst(tbsGDNAME.Text);
        }


        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            lstTYPE.SelectedValue = "M";
            btnDelect.Enabled = false;
            btnAddRow.Enabled = false;
            //if (ckbISFLAG9.Checked)
            //{
            //    tbxJFDM.Enabled = true;
            //}
            //else
            //{
            //    tbxJFDM.Enabled = false;
            //}
            TabStripMain.Hidden = false;
            TabStrip2.Hidden = false;
            Button2.Hidden = true;
            Grid1.DataSource = null;
            Grid1.DataBind();
            Grid1.Hidden = false;
            Grid1.Height = 250;
            dataSearch();

            WindowGoods.Hidden = false;
        }

        protected void btnDelect_Click(object sender, EventArgs e)
        {
            lstTYPE.SelectedValue = "D";
            btnUpdate.Enabled = false;
            btnAddRow.Enabled = false;
            //if (ckbISFLAG9.Checked)
            //{
            //    tbxJFDM.Enabled = true;
            //}
            //else
            //{
            //    tbxJFDM.Enabled = false;
            //}


            Grid1.DataSource = null;
            Grid1.DataBind();
            Grid1.Hidden = false;
            dataSearch();




            WindowGoods.Hidden = false;
            TabStripMain.Hidden = true;
            TabStrip2.Hidden = true;
            Button2.Hidden = false;
            Grid1.Height = 420;
        }

        //protected void ckbISFLAG9_CheckedChanged(object sender, CheckedEventArgs e)
        //{
        //    if (ckbISFLAG9.Checked)
        //    {
        //        tbxJFDM.Enabled = true;
        //    }
        //    else
        //    {
        //        tbxJFDM.Enabled = false;
        //    }
        //}
        protected void Button1_Click(object sender, EventArgs e)
        {
            if (txaMemo.Text.Trim().Length < 1)
            {
                Alert.Show("请填写驳回原因！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            if (DbHelperOra.ExecuteSql(string.Format("UPDATE DAT_TZGOODS_DOC SET FLAG = 'R', MEMO='驳回原因：'||'{1}' WHERE  SEQNO = '{0}'", tbxSEQNO.Text, txaMemo.Text.Trim())) > 0)
            {
                Alert.Show("商品信息驳回成功！");
                OperLog("商品管理", "驳回单据【" + tbxBILLNO.Text + "】");
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
            
            switch (ddlType.SelectedValue)
            {
                case "N":
                    billAddRow();
                    break;
                case "M":
                    btnUpdate_Click(null, null);
                    break;
                case "D":
                    btnDelect_Click(null, null);
                    break;
                default:
                    Alert.Show("请选择单据类型！", "提示信息", MessageBoxIcon.Warning);
                    break;
            }

            PubFunc.FormDataClear(FormMain);
            PubFunc.FormDataClear(FormAssist);
            PubFunc.FormDataClear(Form3);
            PubFunc.FormDataClear(Form4);


        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            if (ddlType.SelectedValue == "D")
            {
                for (int i = 0; i < Grid1.SelectedRowIndexArray.Length; i++)
                {
                    if (DbHelperOra.Exists("select 1 from doc_goodstemp where gdseq='" + Grid1.Rows[Grid1.SelectedRowIndexArray[i]].DataKeys[0] + "'"))
                    {
                        DbHelperOra.ExecuteSql("DELETE FROM DOC_GOODSTEMP WHERE GDSEQ='" + Grid1.Rows[Grid1.SelectedRowIndexArray[i]].DataKeys[0] + "'");
                    }
                    if (DbHelperOra.Exists("select 1 from doc_goods where gdseq='" + Grid1.Rows[Grid1.SelectedRowIndexArray[i]].DataKeys[0] + "'"))
                    {
                        DbHelperOra.ExecuteSql("INSERT INTO DOC_GOODSTEMP SELECT * FROM DOC_GOODS where gdseq='" + Grid1.Rows[Grid1.SelectedRowIndexArray[i]].DataKeys[0] + "' ");
                    }
                    hfdTEMP.Text += Grid1.Rows[Grid1.SelectedRowIndexArray[i]].DataKeys[0] + ",";
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
            GridGoods.DataSource = DbHelperOra.Query(@"SELECT T.*,f_getunitname(T.UNIT) UNITNAME,
                                       f_getsupname(T.SUPPLIER) SUPPLIETNAME,
                                       f_getsupname(T.PRODUCER) PRODUCERNAME,
                                       f_getcatname(T.CATID) CATIDNAME,
                                       f_getcatid0name(T.CATID0) CATID0NAME,
                                       DECODE(T.ISGZ, 'Y', '是', '否') GZNAME,
                                       DECODE(T.ISFLAG7, 'Y', '是', '否') BDNAME,T.GDSEQ GDID,DECODE(T.ISFLAG9, 'Y', '是', '否') JFNAME,
                                       DECODE(T.ISLOT, '2', '是', '否') PHNAME FROM DOC_GOODSTEMP T WHERE T.GDSEQ IN ('" + hfdTEMP.Text + "')").Tables[0];
            GridGoods.DataBind();
            ddlUNIT.Enabled = false;
            ddlCATID.Enabled = false;
            ddlISFLAG7.Enabled = false;
            tbxGDID.Enabled = false;
            tbxGDSPEC.Enabled = false;
            ddlUNIT.Enabled = false;
            //nbbBHSJJ.Enabled = false;
            trbPRODUCER.Enabled = false;
            tbxPIZNO.Enabled = false;
            hfdTEMP.Text = "";
            trbSearch.Text = string.Empty;
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
            string strSql = @"SELECT A.GDSEQ 商品编号,
                                    A.GDNAME 商品名称,
                                    A.GDSPEC 规格,
                                    A.HSJJ 价格,
                                    f_getunitname(A.UNIT) 单位,
                                    f_getsupname(A.SUPPLIER) 供应商,
                                    f_getsupname(A.PRODUCER) 生产商,
                                    f_getcatname(A.CATID) 商品类别,
                                    A.PIZNO 注册证号,
                                    DECODE(A.ISGZ, 'Y', '是', '否') 是否高值
                                FROM DAT_TZGOODS_COM A
                                WHERE A.SEQNO = '{0}'";
            DataTable dt = DbHelperOra.Query(string.Format(strSql, tbxBILLNO.Text)).Tables[0];
            ExcelHelper.ExportByWeb(dt, string.Format("新增商品信息"), "新增商品信息导出_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");

        }
    }
}