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


namespace ERPProject.ERPApply
{
    public partial class DepartmentApply : BillBase
    {
        private string strDocSql = "SELECT c.*,c.BILLTYPE OPER,(SELECT  '['||CODE||']'||NAME NAME  FROM SYS_DEPT where CODE=c.DEPTID) DEPNAME FROM DAT_SL_DOC c WHERE C.SEQNO ='{0}' AND C.CATID = '{1}'";
        //private string strComSql = @"SELECT A.*, F_GETUNITNAME(A.UNIT) UNITNAME,F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME,f_getunitname(B.UNIT) UNITSMALLNAME,DECODE(FPFLAG,'Y','已分配','F','作废','未分配') FPFLAGNAME
        //                FROM DAT_SL_COM A,DOC_GOODS B WHERE SEQNO ='{0}' AND A.GDSEQ = B.GDSEQ AND NVL(A.MEMO,'#') not like '替代商品%' ORDER BY ROWNO";
        //private string strComSql = @"SELECT nvl(C.KCSL,0) CKKCSL,DECODE(nvl(D.QHSL,0),0,'0','<div style = '||'''background-color:red'''||'>'||D.QHSL||'</div>') QHSL,A.*, F_GETUNITNAME(A.UNIT) UNITNAME,F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME,f_getunitname(B.UNIT) UNITSMALLNAME,DECODE(FPFLAG,'Y','已分配','F','作废','未分配') FPFLAGNAME
        //                FROM DAT_SL_COM A,DOC_GOODS B,(SELECT GDSEQ,SUM(KCSL-LOCKSL) KCSL FROM DAT_GOODSSTOCK WHERE DEPTID='{1}' GROUP BY GDSEQ) C,dat_nostock_list D WHERE a.SEQNO ='{0}' AND A.GDSEQ = B.GDSEQ AND A.GDSEQ=C.GDSEQ(+) AND A.SEQNO=D.BILLNO_SL(+) and a.rowno=d.rowno(+) AND NVL(A.MEMO,'#') not like '替代商品%' ORDER BY a.ROWNO";
        private string strComSql = @"SELECT nvl(C.KCSL, 0) CKKCSL,
                                               DECODE(nvl(D.QHSL, 0),
                                                      0,
                                                      '0',
                                                      '<div style = ' || '''background-color:red''' || '>' || D.QHSL ||
                                                      '</div>') QHSL,
                                               A.ROWNO,A.GDSEQ,A.GDNAME,A.GDSPEC,A.BZHL,A.BZSL,A.HSJJ,A.HSJE,A.PZWH,A.MEMO,A.DHSL,A.ZPBH,A.PH,
                                               A.ISLOT,A.UNIT,A.ISGZ,A.ISJF,A.HWID,A.RQ_SC,A.YXQZ,A.JXTAX,A.BARCODE,--A.ZGKC,A.ZDKC
                                               F_GETUNITNAME(A.UNIT) UNITNAME,
                                               F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME,
                                               f_getunitname(B.UNIT) UNITSMALLNAME,
                                               DECODE(FPFLAG, 'Y', '已分配', 'F', '作废', '未分配') FPFLAGNAME,NVL(E.XSSL,0) FPSL
                                          FROM DAT_SL_COM A,
                                               DOC_GOODS B,
                                               (SELECT GDSEQ, SUM(KCSL - LOCKSL) KCSL
                                                  FROM DAT_GOODSSTOCK
                                                 WHERE DEPTID = '{1}'
                                                 GROUP BY GDSEQ) C,
                                               dat_nostock_list D,(SELECT A.XSSL,A.SEQNO,A.GDSEQ FROM DAT_CK_COM A,DAT_CK_DOC B WHERE A.SEQNO=B.SEQNO AND B.FLAG='Y') E
                                         WHERE a.SEQNO = '{0}' AND A.STR1=E.SEQNO(+) AND A.GDSEQ=E.GDSEQ(+)
                                           AND A.GDSEQ = B.GDSEQ
                                           AND A.GDSEQ = C.GDSEQ(+)
                                           AND A.SEQNO = D.BILLNO_SL(+)
                                           and a.rowno = d.rowno(+)
                                           AND NVL(A.MEMO, '#') not like '替代商品%'
                                         ORDER BY a.ROWNO
                                        ";
        private string strGoodsSql = @"SELECT G.GDSEQ,G.BARCODE,G.GDNAME,G.GDSPEC,G.UNIT,CEIL(Z.SL/G.BZHL) BZSL,CEIL(Z.SL/G.BZHL)*G.BZHL DHSL,CEIL(Z.SL/G.BZHL)*G.BZHL XSSL,
                                                                 (G.HSJJ*DECODE(G.UNIT_SELL,'D',G.NUM_DABZ,'Z',G.NUM_ZHONGBZ,G.BZHL)) HSJJ,CEIL(Z.SL / G.BZHL)*DECODE(G.UNIT_SELL, 'D', G.NUM_DABZ, 'Z', G.NUM_ZHONGBZ, G.BZHL)* (G.HSJJ *
                                                                  DECODE(G.UNIT_SELL, 'D', G.NUM_DABZ, 'Z', G.NUM_ZHONGBZ, G.BZHL)) HSJE,G.ZPBH,P.HJCODE1 HWID,G.PRODUCER,
                                                                  F_GETUNITNAME(G.UNIT) UNITSMALLNAME, DECODE(G.UNIT_SELL,'D',G.NUM_DABZ,'Z',G.NUM_ZHONGBZ,G.BZHL)  BZHL,G.ISLOT,G.ISGZ,
                                                                  F_GETUNITNAME(DECODE(G.UNIT_SELL,'D',G.UNIT_DABZ,'Z',G.UNIT_ZHONGBZ,G.UNIT))  UNITNAME,Z.MEMO,
                                                                  F_GETPRODUCERNAME(G.PRODUCER) PRODUCERNAME,'' FPFLAGNAME,'' MEMO,'' PH,'' RQ_SC,'' YXQZ,G.PIZNO PZWH,G.JXTAX
                                                        FROM DOC_GOODS G,DOC_GOODSCFG P,DOC_GROUPCOM Z WHERE G.GDSEQ=P.GDSEQ AND G.GDSEQ=Z.GDSEQ AND G.flag='Y' and P.DEPTID='{0}' AND Z.GROUPID='{1}' ";

        public override Field[] LockControl
        {
            get { return new Field[] { docSEQNO, docSLR, docDEPTOUT, docXSRQ, rblBILLTYPE, docMEMO }; }
        }

        public DepartmentApply()
        {
            BillType = "LYD";
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            string path = Request.RawUrl;
            if (!IsPostBack)
            {
                DataInit();
                billNew();
                //屏蔽不需要的操作按钮
                if (Request.QueryString["oper"] != null)
                {
                    if (Request.QueryString["oper"].ToString() == "input")
                    {
                        ButtonHidden(btnAudit, btnCancel, btnAuditBatch);
                        GridList.EnableCheckBoxSelect = false;
                        hfdoper.Text = "input";
                        Line1.Hidden = true;
                        lineCKKCSL.Hidden = true;
                        //取值，出库部门绑定当前商品类型的默认库房
                        string strGoodsType = Request.QueryString["tp"].ToString();
                        object obj = DbHelperOra.GetSingle(string.Format("SELECT STR2 FROM DOC_GOODSTYPE WHERE ISDELETE = 'N' AND CODE = '{0}'", strGoodsType));
                        lstDEPTOUT.SelectedValue = obj.ToString();
                        docDEPTOUT.SelectedValue = obj.ToString();
                        if (Doc.DbGetSysPara("USERXMID") == "WH_WDZXPH")
                        {
                            lstDEPTOUT.Enabled = false;
                            docDEPTOUT.Enabled = false;
                        }

                        if (Request.QueryString["tp"].ToString() == "5")
                        {
                            TabStrip1.ActiveTabIndex = 0;
                            billSearch();
                        }
                    }
                    else if (Request.QueryString["oper"].ToString() == "audit")
                    {
                        ToolbarText1.Text = "操作信息:科室商品审批";
                        lineQHSL.Hidden = true;
                        billLockDoc(true);
                        ButtonHidden(btnNew, btnSave, btnCopy, btnDelRow, btnPrint, btnGoods, btnSubmit, btnTemplate, btnDel);
                        if (UserAction.UserID == "gaodm")
                        {
                            ButtonHidden(btnAudit, btnCancel, btnAuditBatch);
                        }
                        TabStrip1.ActiveTabIndex = 0;
                        billSearch();
                        hfdoper.Text = "audit";
                    }
                }
                if (Request.QueryString["pid"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["pid"].ToString()))
                {
                    billOpen(Request.QueryString["pid"].ToString());
                }


            }

        }
        private void DataInit()
        {


            PubFunc.DdlDataGet("DDL_SYS_DEPOT", lstDEPTOUT, docDEPTOUT, ddlDeptOrder);

            //当前用户是护士长，申领人下拉只显示当前科室下的用户
            if (UserAction.UserRole == "02")
            {
                string sqls = string.Format(@"select  CODE,NAME from (
                                                                    SELECT '--请选择--' NAME,'' CODE  FROM dual
                                                                    union all
                                                                    select username name, userid code
                                                                           from sys_operuser 
                                                                           where roleid = '{0}' and dept = '{1}' and islogin = 'Y')",
                                                        UserAction.UserRole, UserAction.UserDept);
                PubFunc.DdlDataSql(lstSLR, sqls);
                PubFunc.DdlDataSql(lstLRY, sqls);
                PubFunc.DdlDataSql(docLRY, sqls);
                PubFunc.DdlDataSql(docSLR, sqls);
                PubFunc.DdlDataSql(docSHR, sqls);
            }
            else
            {
                PubFunc.DdlDataGet("DDL_USER", lstSLR, lstLRY, docLRY, docSLR, docSHR);
            }

            DepartmentBind.BindDDL("DDL_SYS_DEPTRANGE", UserAction.UserID, docDEPTID, lstDEPTID, ddlDeptid);
            PubFunc.DdlDataGet("DDL_BILL_STATUSSLD", lstFLAG, docFLAG);
            PubFunc.DdlDataGet(ddlReject, "DDL_BILL_REASON");
            PubFunc.DdlDataGet(ddlCATID, "DDL_SYS_CATEGORY_TYPE");

            if (docDEPTOUT.Items.Count > 0)
            {
                docDEPTOUT.SelectedIndex = 1;
            }
            if (docDEPTID.Items.Count > 0)
            {
                docDEPTID.SelectedIndex = 1;
            }

            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-7);
            lstLRRQ2.SelectedDate = DateTime.Now;
        }
        protected Boolean OPER()
        {
            if (docDEPTID.SelectedValue.Length < 1) return true;
            if (DbHelperOra.Exists("SELECT 1 FROM SYS_DEPT WHERE ISORDER = 'Y' AND CODE = '" + docDEPTID.SelectedValue + "'"))
            {
                return true;
            }
            return false;
        }
        protected override void billNew()
        {
            string strDept = docDEPTID.SelectedValue;
            string strDeptOut = docDEPTOUT.SelectedValue;
            if (PubFunc.StrIsEmpty(strDept))
            {
                if (docDEPTID.Items.Count > 1)
                    strDept = docDEPTID.Items[1].Value;
            }
            if (PubFunc.StrIsEmpty(strDeptOut))
            {
                if (docDEPTOUT.Items.Count > 1)
                    strDeptOut = docDEPTOUT.Items[1].Value;
            }
            PubFunc.FormDataClear(FormDoc);
            docFLAG.SelectedValue = "M";
            rblBILLTYPE.SelectedIndex = 0;
            docSLR.SelectedValue = UserAction.UserID;
            docLRY.SelectedValue = UserAction.UserID;
            docLRRQ.SelectedDate = DateTime.Now;
            docXSRQ.SelectedDate = DateTime.Now;
            docDEPTID.SelectedValue = strDept;
            docDEPTOUT.SelectedValue = strDeptOut;
            docDEPTID.Enabled = true;
            billLockDoc(false);
            docSEQNO.Enabled = false;
            docBILLNO.Text = string.Empty;
            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            //DataTable dt = new DataTable();
            //hfdTest.Text = "test";
            //GridGoods.DataSource = dt;
            //GridGoods.DataBind();
            //hfdTest.Text = "";
            btnGoods.Enabled = true;
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("BZSL", "0");
            summary.Add("HSJE", "0");
            summary.Add("DHSL", "0");
            GridGoods.SummaryData = summary;
            //按钮状态
            docMEMO.Enabled = true;
            btnDel.Enabled = false;
            btnSave.Enabled = true;
            btnSubmit.Enabled = false;
            btnCopy.Enabled = false;
            btnDelRow.Enabled = true;
            btnGoods.Enabled = true;
            btnLoadTemplate.Enabled = true;
            lineQHSL.Hidden = true;
            if (Request.QueryString["tp"] != null)
            {
                ddlCATID.SelectedValue = Request.QueryString["tp"].ToString();
            }
            //操作类别判断          
            rblBILLTYPE.Enabled = OPER();
            GetBudgetAndExec();
        }
        protected void tgbBILLNO_TriggerClick(object sender, EventArgs e)
        {
            billSearch();
        }
        protected void GridList_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                string flag = row["FLAG"].ToString();
                FineUIPro.BoundField flagcol = GridList.FindColumn("FLAG_CN") as FineUIPro.BoundField;
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
            }
        }
        protected void GridGoods_AfterEdit(object sender, FineUIPro.GridAfterEditEventArgs e)
        {
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
            if (newDict.Count == 0) return;
            if (e.ColumnID == "BZSL")
            {
                if (!PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "BZHL")) || !PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "BZSL")) || !PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "HSJJ")))
                {
                    Alert.Show("商品信息异常，请详细检查商品信息：包装含量或价格！", "异常信息", MessageBoxIcon.Warning);
                    return;
                }
                //处理返回jobject
                JObject defaultObj = Doc.GetJObject(GridGoods, e.RowID);
                decimal hl = 0, rs = 0, jg = 0;
                decimal.TryParse((defaultObj["BZHL"] ?? "0").ToString(), out hl);
                decimal.TryParse((defaultObj["BZSL"] ?? "0").ToString(), out rs);
                decimal.TryParse((defaultObj["HSJJ"] ?? "0").ToString(), out jg);
                defaultObj["DHSL"] = rs * hl;
                defaultObj["HSJE"] = Math.Round(rs * jg, 2).ToString("F2");
                PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(e.RowID, defaultObj));
                #region //计算合计数量
                decimal bzslTotal = 0, feeTotal = 0, dhslTatal = 0;
                foreach (Dictionary<string, object> dic in newDict)
                {
                    if ((dic["BZSL"] ?? "0").ToString().Length > 0 && (dic["HSJJ"] ?? "0").ToString().Length > 0)
                    {
                        bzslTotal += Convert.ToDecimal(dic["BZSL"] ?? "0");
                        feeTotal += Convert.ToDecimal(dic["HSJJ"] ?? "0") * Convert.ToDecimal(dic["BZSL"] ?? "0");
                        dhslTatal += Convert.ToDecimal(dic["BZHL"] ?? "0") * Convert.ToDecimal(dic["BZSL"] ?? "0");
                    }


                    bool strISFLAG5 = ISDecimal(dic["GDSEQ"].ToString());
                    if (strISFLAG5)
                    {
                        string str = Convert.ToString(Convert.ToDecimal(dic["BZSL"] ?? "0"));
                        if (Convert.ToDecimal(dic["BZSL"]) != (int)Convert.ToDecimal(dic["BZSL"]))
                        {
                            Alert.Show("当前商品不支持申领数为小数，请调整", "消息提示", MessageBoxIcon.Warning);

                        }
                    }
                }
                JObject summary = new JObject();
                summary.Add("GDNAME", "本页合计");
                summary.Add("BZSL", bzslTotal.ToString());
                summary.Add("HSJE", feeTotal.ToString("F2"));
                summary.Add("DHSL", dhslTatal.ToString());
                GridGoods.SummaryData = summary;
                #endregion
            }
        }


        private bool ISDecimal(string GDSEQ)
        {
            object objISFLAG5 = DbHelperOra.GetSingle(string.Format("SELECT ISFLAG5 FROM DOC_GOODS WHERE GDSEQ = '{0}'", GDSEQ));
            if (!string.IsNullOrWhiteSpace(objISFLAG5.ToString()) && objISFLAG5.ToString() == "N")
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        protected override void billClear()
        {
            PubFunc.FormDataClear(Formlist);
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-7);
            lstLRRQ2.SelectedDate = DateTime.Now;
        }
        protected override void billDelRow()
        {
            if (docFLAG.SelectedValue != "M" && docFLAG.SelectedValue != "R")
            {
                Alert.Show("非『新增』单据不能删行！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!CheckFlagForM() && !CheckFlagForR())
            {
                Alert.Show("此单据已经被别人操作，请等待操作!");
                return;
            }
            if (GridGoods.SelectedRowIDArray.Length == 0)
            {
                Alert.Show("当前未选中单元行，无法进行操作!", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            foreach (string srow in GridGoods.SelectedRowIDArray)
            {
                PageContext.RegisterStartupScript(GridGoods.GetDeleteRowReference(srow));
            }

            PubFunc.FormLock(FormDoc, true, "");
            docMEMO.Enabled = true;
        }

        //protected override void billGoods()
        //{
        //    billLockDoc(true);
        //    GridNoSelectGoods.Hidden = false;
        //    Panel4.Hidden = false;
        //    docDEPTID.Enabled = false;
        //    btnCollect.Text = "收藏商品";

        //    if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
        //    GridNoSelectGoods.DataSource = null;
        //    GridNoSelectGoods.DataBind();
        //    GridCFGGoods.DataSource = null;
        //    GridCFGGoods.DataBind();
        //    dataSearch();
        //    WindowGoods.Hidden = false;
        //}
        protected override void billGoods()
        {
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
            PubFunc.FormLock(FormDoc, true, "docMEMO,tbxBARCODE");
            string strISGZ = "";
            if (rblBILLTYPE.SelectedValue == "GBD")
            {
                strISGZ = "Y";
            }
            //参数说明：cx-查询内容，bm-商品配置部门,su-供应商
            //string url = "~/ERPQuery/GoodsWindow_New.aspx?Deptout=" + docDEPTOUT.SelectedValue + "&DeptIn=" + docDEPTID.SelectedValue + "&goodsType=" + ddlCATID.SelectedValue + "&isGZ=" + strISGZ;
            string url = "~/ERPQuery/GoodsWindow_New.aspx?Deptout=" + docDEPTOUT.SelectedValue + "&DeptIn=" + docDEPTID.SelectedValue + "&goodsType=" + ddlCATID.SelectedValue + "&isGZ=" + strISGZ + "&GoodsState=YT";
            PageContext.RegisterStartupScript(Window3.GetSaveStateReference(hfdValue.ClientID) + Window3.GetShowReference(url, "商品信息查询"));
        }

        protected override void billSearch()
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【申领日期】！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                Alert.Show("【开始日期】大于【结束日期】，请重新输入！", "提示信息", MessageBoxIcon.Warning);
                return;
            }

            string strSql = @"SELECT A.SEQNO,A.BILLNO,(select sum(hsje) from dat_sl_com sa where sa.seqno = a.seqno) subsum,
                                     DECODE(A.FLAG,'M','新 单','N','已提交','S','已审批','R','已驳回','B','缺货中','D','调拨中','W','调拨完成','Y','已完成','未定义' )FLAG_CN,A.FLAG,F_GETDEPTNAME(A.DEPTID) DEPTID,A.XSRQ,F_GETDEPTNAME(A.DEPTOUT) DEPTOUT,
                                     A.SUBNUM,F_GETUSERNAME(A.SLR) SLR,F_GETUSERNAME(A.LRY) LRY,A.LRRQ,F_GETUSERNAME(A.SHR) SHR,A.SHRQ,A.MEMO,A.STR2,DECODE(A.ISSH,'Y','是','N','否','否')ISSH,NVL(ISSH,'N')ISSH_FLAG
                                from DAT_SL_DOC A
                                WHERE BILLTYPE IN ('LYD','GBD') AND XSTYPE='1'";
            string strSearch = "";
            if (Request.QueryString["oper"].ToString().ToLower() == "audit")
            {
                strSql = strSql + " AND A.FLAG<>'M'";

                //根据DOC_GOODSTYPE表中的STR1字段来确定当前登录人员是否有审核权限 By c 2015年11月30日15:04:15
                strSql += string.Format(" AND F_CHK_AUDIT(NVL(A.CATID,'2'),'{0}') = 'Y'", UserAction.UserID);
                strSql += string.Format(" AND F_CHK_AUDIT_SJ(A.BILLNO,'{0}') = 'Y'", UserAction.UserID);

            }
            else
            {
                //根据商品类别【CATID】来加载信息，默认加载耗材-2  By c 2015年11月30日15:38:20
                if (Request.QueryString["tp"] != null && Request.QueryString["tp"].ToString() != "")
                {
                    strSql = strSql + string.Format(" AND NVL(A.CATID,'2') ='{0}'", Request.QueryString["tp"].ToString());
                }
            }

            if (tgbBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND A.BILLNO  LIKE '%{0}%'", tgbBILLNO.Text.ToUpper());
            }
            if (lstFLAG.SelectedItem != null && lstFLAG.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.FLAG='{0}'", lstFLAG.SelectedItem.Value);
            }
            if (lstLRY.SelectedItem != null && lstLRY.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.LRY='{0}'", lstLRY.SelectedItem.Value);
            }
            else
            {
                if (UserAction.UserRole == "02")
                {
                   // strSearch += string.Format(" AND A.LRY in (SELECT USERID FROM SYS_OPERUSER WHERE  dept = '{0}' and islogin = 'Y')", UserAction.UserDept);
                }
            }
            if (lstSLR.SelectedItem != null && lstSLR.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND SLR='{0}'", lstSLR.SelectedItem.Value);
            }
            else
            {
                if (UserAction.UserRole == "02")
                {
                    //strSearch += string.Format(" AND A.SLR in (SELECT USERID FROM SYS_OPERUSER WHERE  dept = '{0}' and islogin = 'Y')", UserAction.UserDept);
                }
            }

            if (lstDEPTID.SelectedValue != null && lstDEPTID.SelectedValue.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedValue);
            }
            if (lstDEPTOUT.SelectedItem != null && lstDEPTOUT.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND DEPTOUT='{0}'", lstDEPTOUT.SelectedItem.Value);
            }

            if (tgbGDSEQ.Text.Trim().Length > 0)
            {
                strSearch += string.Format(" AND A.SEQNO IN(SELECT SEQNO FROM DAT_SL_COM A, DOC_GOODS B WHERE A.GDSEQ = B.GDSEQ AND(B.GDSEQ LIKE '%{0}%' OR A.GDNAME LIKE '%{0}%' OR B.ZJM LIKE '%{0}%'  OR upper(B.GDSEQ) LIKE '%{1}%' OR upper(A.GDNAME) LIKE '%{1}%' OR upper(B.ZJM) LIKE '%{1}%'  OR lower(A.GDSEQ) LIKE '%{2}%' OR lower(B.GDNAME) LIKE '%{2}%' OR lower(B.ZJM) LIKE '%{2}%'))", tgbGDSEQ.Text.Trim(), tgbGDSEQ.Text.Trim().ToUpper(), tgbGDSEQ.Text.Trim().ToLower());
            }

            strSearch += string.Format(" AND deptid in( select code FROM SYS_DEPT where type <>'1' and  F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            strSearch += string.Format(" AND A.XSRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.XSRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }

            strSql = "select * from (" + strSql + ") order by DECODE(FLAG,'N',1,'R',2,'B',3,'D',4) " + GridList.SortDirection;
            PageContext.RegisterStartupScript(GridList.GetRejectChangesReference());
            int total = 0;
            DataTable dt = PubFunc.DbGetPage(GridList.PageIndex, GridList.PageSize, strSql, ref total);
            GridList.DataSource = dt;
            GridList.RecordCount = total;
            GridList.DataBind();
            decimal Total1 = 0, Total2 = 0;
            foreach (DataRow row in dt.Rows)
            {
                Total1 += Convert.ToDecimal(row["SUBSUM"] ?? "0");
                Total2 += Convert.ToDecimal(row["SUBNUM"] ?? "0");
            }
            JObject summary = new JObject();
            summary.Add("XSRQ", "本页合计");
            summary.Add("SUBSUM", Total1);
            summary.Add("SUBNUM", Total2);
            GridList.SummaryData = summary;



        }

        protected override void billAudit()
        {
            if (DataSave("AUDIT"))//在审批时允许修改数量
            {
                string strBillno = docSEQNO.Text;
                object obj = DbHelperOra.GetSingle("SELECT COUNT(1) FROM DAT_SL_COM A WHERE A.SEQNO='"+strBillno+"' AND EXISTS(SELECT 1 FROM  DOC_GOODS WHERE GDSEQ=A.GDSEQ AND CATID='03')");
                object objall = DbHelperOra.GetSingle("SELECT COUNT(1) FROM DAT_SL_COM WHERE SEQNO='" + strBillno + "'");
                if (!PubFunc.Equals(obj.ToString(), "0"))
                {
                    if (!PubFunc.Equals(obj.ToString(), objall.ToString()))
                    {
                        Alert.Show("单据【" + strBillno + "】同时存在耗材和试剂，请驳回重新申领！");
                        return;
                    }
                }
              
                if (BillOper(strBillno, "DECLARE") == 1)
                {
                    billLockDoc(true);
                    Alert.Show("单据【" + strBillno + "】审批成功！", "消息提示", MessageBoxIcon.Information);
                    OperLog("科室申领", "审核单据【" + docBILLNO.Text + "】");
                    billOpen(strBillno);
                    GetBudgetAndExec();
                }
            }
        }

        protected bool billFlag(string billno, string flag)
        {
            //从数据库里判断当前单据的状态，避免并发
            if (!DbHelperOra.Exists("SELECT 1 FROM DAT_SL_DOC WHERE BILLNO = '" + billno + "' AND FLAG = '" + flag + "'"))
            {
                Alert.Show("此单据已经被操作，请刷新确认再做操作。", "消息提示", MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        protected override void listRow_DoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            billOpen(GridList.Rows[e.RowIndex].Values[1].ToString());
        }
        protected override void billOpen(string strBillno)
        {
            DataTable dtDoc = new DataTable();
            if (Request.QueryString["oper"].ToString() == "audit")
            {
                string sql = "SELECT c.*,c.BILLTYPE OPER,(SELECT  '['||CODE||']'||NAME NAME  FROM SYS_DEPT where CODE=c.DEPTID) DEPNAME FROM DAT_SL_DOC c WHERE C.SEQNO ='{0}'";
                dtDoc = DbHelperOra.Query(string.Format(sql, strBillno)).Tables[0];
            }
            else
            {
                dtDoc = DbHelperOra.Query(string.Format(strDocSql, strBillno, (Request.QueryString["tp"] ?? "2").ToString())).Tables[0];
            }

            if (dtDoc != null && dtDoc.Rows.Count > 0)
            {
                PubFunc.FormDataSet(FormDoc, dtDoc.Rows[0]);
            }
            else
            {
                if (Request.QueryString["oper"].ToString() == "audit")
                {
                    Alert.Show("您没有审核该单据的权限！！！", "警告提示", MessageBoxIcon.Warning);
                    return;
                }

            }

            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            DataTable dtBill = DbHelperOra.Query(string.Format(strComSql, strBillno, docDEPTOUT.SelectedValue)).Tables[0];
            decimal bzslTotal = 0, feeTotal = 0, dhslTotal = 0;
            if (dtBill != null && dtBill.Rows.Count > 0)
            {
                foreach (DataRow row in dtBill.Rows)
                {
                    bzslTotal += Convert.ToDecimal(row["BZSL"] ?? "0");
                    feeTotal += Convert.ToDecimal(row["BZSL"] ?? "0") * Convert.ToDecimal(row["HSJJ"] ?? "0");
                    dhslTotal += Convert.ToDecimal(row["BZSL"] ?? "0") * Convert.ToDecimal(row["BZHL"] ?? "0");
                }
                //GridGoods.DataSource = dtBill;
                //GridGoods.DataBind();
                Doc.GridRowAdd(GridGoods, dtBill);

            }
            //增加合计
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("BZSL", bzslTotal.ToString());
            summary.Add("HSJE", feeTotal.ToString("F2"));
            summary.Add("DHSL", dhslTotal.ToString());
            GridGoods.SummaryData = summary;
            PubFunc.FormLock(FormDoc, true, "");
            TabStrip1.ActiveTabIndex = 1;
            //按钮状态转换
            if (docFLAG.SelectedValue == "M" || docFLAG.SelectedValue == "R")
            {
                docMEMO.Enabled = true;
                btnDel.Enabled = true;
                btnSave.Enabled = true;
                btnSubmit.Enabled = true;
                btnCopy.Enabled = true;
                btnDelRow.Enabled = true;
                btnGoods.Enabled = true;
                btnLoadTemplate.Enabled = true;
                btnPrint.Enabled = false;
                rblBILLTYPE.Enabled = false;
            }
            else if (docFLAG.SelectedValue == "N")
            {
                //提交
                docMEMO.Enabled = false;
                btnDel.Enabled = false;
                btnSave.Enabled = false;
                btnSubmit.Enabled = false;
                btnCopy.Enabled = true;
                btnDelRow.Enabled = false;
                btnGoods.Enabled = false;
                btnAudit.Enabled = true;
                btnCancel.Enabled = true;
                btnLoadTemplate.Enabled = false;
                btnPrint.Enabled = true;
            }
            else
            {
                //审核
                docMEMO.Enabled = false;
                btnDel.Enabled = false;
                btnSave.Enabled = false;
                btnSubmit.Enabled = false;
                btnCopy.Enabled = true;
                btnDelRow.Enabled = false;
                btnGoods.Enabled = false;
                btnAudit.Enabled = false;
                btnCancel.Enabled = false;
                btnLoadTemplate.Enabled = false;
                btnPrint.Enabled = true;
                if (docFLAG.SelectedValue == "B" && Request.QueryString["oper"].ToString() == "input") lineQHSL.Hidden = false;
                else lineQHSL.Hidden = true;
            }
            //判断用户是否有审核权限
            if (Request.QueryString["tp"] != null && DbHelperOra.Exists(String.Format("SELECT 1 FROM DUAL WHERE F_CHK_AUDIT('{0}', '{1}') = 'Y'", Request.QueryString["tp"].ToString(), UserAction.UserID)))
            {
                btnAudit.Enabled = false;
                btnCancel.Enabled = false;
            }

        }

        protected void GridGoods_RowDataBound(object sender, GridRowEventArgs e)
        {
            //    if (hfdTest.Text != "test")
            //    {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {

                int QHSL = Convert.ToInt32(row["QHSL"].ToString());
                if (QHSL > 0)
                {
                    //FineUIPro.BoundField flagcol = GridGoods.FindColumn("QHSL") as FineUIPro.BoundField;
                    e.CellAttributes[GridGoods.FindColumn("QHSL").ColumnIndex]["data-color"] = "color3";
                }

            }
            //}
        }
        protected override void billSave()
        {
            if (DataSave("SAVE"))
            {
                Alert.Show("商品申领信息保存成功！");
                OperLog("科室申领", "修改单据【" + docBILLNO.Text + "】");
                billOpen(docBILLNO.Text);
            }
        }

        private bool DataSave(string stats_flag)
        {
            #region 数据有效性验证
            if (stats_flag == "SAVE")
            {
                if ((",M,R").IndexOf(docFLAG.SelectedValue) < 0)
                {
                    Alert.Show("非新单不能保存！", "消息提示", MessageBoxIcon.Warning);
                    return false;
                }
                if (!CheckFlagForM() && !CheckFlagForR())
                {
                    Alert.Show("此单据已经被别人操作，请等待操作!");
                    return false;
                }

            }
            else if (stats_flag == "AUDIT")
            {
                if ((",N").IndexOf(docFLAG.SelectedValue) < 0)
                {
                    Alert.Show("非提交单据不能审批！", "消息提示", MessageBoxIcon.Warning);
                    return false;
                }
                string strBillno = docSEQNO.Text;
                if (!Doc.getFlag(strBillno, "N", BillType))
                {
                    Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                    return false;
                }

            }
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().ToList();
            if (newDict.Count == 0)
            {
                Alert.Show("请输入商品信息", "消息提示", MessageBoxIcon.Warning);
                return false;
            }
            List<Dictionary<string, object>> goodsData = new List<Dictionary<string, object>>();
            //判断是否有空行
            for (int i = 0; i < newDict.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(newDict[i]["GDSEQ"].ToString()) && !string.IsNullOrWhiteSpace(newDict[i]["GDNAME"].ToString()))
                {
                    bool strISFLAG5 = ISDecimal(newDict[i]["GDSEQ"].ToString());
                    if (strISFLAG5)
                    {
                        string str = Convert.ToString(Convert.ToDecimal(newDict[i]["BZSL"] ?? "0"));
                        if (Convert.ToDecimal(newDict[i]["BZSL"]) != (int)Convert.ToDecimal(newDict[i]["BZSL"]))
                        {
                            Alert.Show("第【" + (i + 1) + "】行【" + newDict[i]["GDNAME"] + "】商品不支持申领数为小数，请调整", "消息提示", MessageBoxIcon.Warning);
                            return false;
                        }
                    }
                    decimal dec = Convert.ToDecimal(string.IsNullOrWhiteSpace(newDict[i]["BZSL"].ToString()) ? "0" : newDict[i]["BZSL"].ToString());
                    if (!string.IsNullOrWhiteSpace(newDict[i]["BZSL"].ToString()) && dec > 0 && dec < 1000000)
                    {
                        goodsData.Add(newDict[i]);
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(newDict[i]["BZSL"].ToString()))
                        {
                            Alert.Show(string.Format("第【{0}】行商品【{1}】的申领数为空", i + 1, newDict[i]["GDNAME"].ToString()), "消息提示", MessageBoxIcon.Warning);
                            return false;
                        }
                        if (dec < 0)
                        {
                            Alert.Show(string.Format("第【{0}】行商品【{1}】的申领数小于等于0", i + 1, newDict[i]["GDNAME"].ToString()), "消息提示", MessageBoxIcon.Warning);
                            return false;
                        }
                        if (dec >= 1000000)
                        {
                            Alert.Show(string.Format("第【{0}】行商品【{1}】的申领数大于等于1000000", i + 1, newDict[i]["GDNAME"].ToString()), "消息提示", MessageBoxIcon.Warning);
                            return false;
                        }
                    }
                }
            }

            if (goodsData.Count == 0)//所有Gird行都为空行时
            {
                Alert.Show("商品信息的申领数都为0的话，请执行【驳回】操作", "消息提示", MessageBoxIcon.Warning);
                return false;
            }
            #endregion

            if (PubFunc.StrIsEmpty(docSEQNO.Text))
            {
                //科室高值备货商品 By c 20151221 at 新疆昌吉
                if (rblBILLTYPE.SelectedValue == "GBD")//高值备货单
                {
                    docSEQNO.Text = BillSeqGet("GBD");
                }
                else
                {
                    docSEQNO.Text = BillSeqGet();
                }
                docBILLNO.Text = docSEQNO.Text;
                docBILLNO.Enabled = false;
            }
            else
            {
                if (docSEQNO.Text.Length > 16)
                {
                    Alert.Show("单据编号长度不能大于16，请检查！", "消息提示", MessageBoxIcon.Warning);
                    return false;
                }
                string flg = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M') FROM DAT_SL_DOC WHERE SEQNO='{0}'", docSEQNO.Text));
                if (!PubFunc.StrIsEmpty(flg) && (",M,R,N").IndexOf(flg) < 0)
                {
                    Alert.Show("您输入的单据号存在重复信息，请重新输入或置空！", "消息提示", MessageBoxIcon.Warning);
                    return false;
                }
                else
                {
                    docBILLNO.Text = docSEQNO.Text;
                    docSEQNO.Enabled = false;
                }
            }

            MyTable mtType = new MyTable("DAT_SL_DOC");
            mtType.ColRow = PubFunc.FormDataHT(FormDoc);
            mtType.ColRow["SEQNO"] = docBILLNO.Text;

            if (stats_flag == "SAVE")
            {
                mtType.ColRow["FLAG"] = "M";//所有单据在保存时单据状态一律为新增N
            }
            else if (stats_flag == "AUDIT")
            {
                mtType.ColRow["FLAG"] = "N";//所有单据在保存时单据状态一律为新增N
            }
            //科室高值备货单 By c 20151221 at 新疆昌吉
            if (rblBILLTYPE.SelectedValue == "GBD")
            {
                mtType.ColRow["BILLTYPE"] = "GBD";
            }
            else
            {
                mtType.ColRow["BILLTYPE"] = BillType;
            }
            mtType.ColRow.Add("SUBNUM", goodsData.Count);
            mtType.ColRow.Add("XSTYPE", "1");
            mtType.ColRow.Remove("OPER");//By c 20151221 at 新疆昌吉
            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtTypeMx = new MyTable("DAT_SL_COM");
            //先删除单据信息在插入
            cmdList.Add(new CommandInfo("DELETE DAT_SL_DOC WHERE SEQNO='" + docBILLNO.Text + "'", null));//删除单据台头
            cmdList.Add(new CommandInfo("DELETE DAT_SL_COM WHERE SEQNO='" + docBILLNO.Text + "'", null));//删除单据明细
            cmdList.AddRange(mtType.InsertCommand());
            for (int i = 0; i < goodsData.Count; i++)
            {
                mtTypeMx.ColRow = PubFunc.GridDataGet(goodsData[i]);

                //判断 申领数，为0时不能保存
                if (PubFunc.StrIsEmpty(mtTypeMx.ColRow["BZSL"].ToString()) || mtTypeMx.ColRow["BZSL"].ToString() == "0")
                {
                    Alert.Show("商品【" + mtTypeMx.ColRow["GDSEQ"] + " | " + mtTypeMx.ColRow["GDNAME"] + "】【申领数】为0或空，无法进行【申领】操作。");
                    return false;
                }
                mtTypeMx.ColRow.Add("SEQNO", docBILLNO.Text);
                mtTypeMx.ColRow["ROWNO"] = i + 1;
                mtTypeMx.ColRow.Add("PHID", mtTypeMx.ColRow["PH"]);
                mtTypeMx.ColRow["DHSL"] = decimal.Parse(mtTypeMx.ColRow["BZHL"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                mtTypeMx.ColRow["HSJE"] = decimal.Parse(mtTypeMx.ColRow["HSJJ"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                mtTypeMx.ColRow["XSSL"] = 0;
                mtTypeMx.ColRow["FPSL"] = 0;
                mtTypeMx.ColRow.Add("BHSJJ", 0);
                mtTypeMx.ColRow.Add("BHSJE", 0);
                mtTypeMx.ColRow.Remove("UNITNAME");
                mtTypeMx.ColRow.Remove("PRODUCERNAME");

                cmdList.Add(mtTypeMx.Insert());
            }
            string flag = DbHelperOra.GetSingle("SELECT VALUE FROM SYS_PARA WHERE CODE='ISAUTOAUDIT'").ToString();
            //当系统设置为自动审批时，商品申领信息在保存之后即进行审批操作
            if (flag == "Y")
            {
                OracleParameter[] parameters = {
                                               new OracleParameter("VTASKID", OracleDbType.Varchar2,20),
                                               new OracleParameter("VPARA", OracleDbType.Varchar2,800) };
                parameters[0].Value = BillType;
                parameters[1].Value = "'" + docBILLNO.Text + "','" + BillType + "','" + UserAction.UserID + "','DECLARE'";
                cmdList.Add(new CommandInfo("P_EXECTASK", parameters, CommandType.StoredProcedure));
            }
            return DbHelperOra.ExecuteSqlTran(cmdList);
        }
        private void DataGridBack(DataTable dt)
        {
            string msg = "";
            DataTable result = new DataTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                dt.Columns["PIZNO"].ColumnName = "PZWH";
                dt.Columns["UNITNAME"].ColumnName = "UNITSMALLNAME";
                dt.Columns["UNIT_SELL_NAME"].ColumnName = "UNITNAME";
                dt.Columns["UNIT_SELL"].ColumnName = "UNIT";
                dt.Columns["BZHL_SELL"].ColumnName = "BZHL";

                dt.Columns.Add("PH", Type.GetType("System.String"));
                dt.Columns.Add("RQ_SC", Type.GetType("System.String"));
                dt.Columns.Add("YXQZ", Type.GetType("System.String"));
                dt.Columns.Add("MEMO", Type.GetType("System.String"));
                dt.Columns.Add("BZSL", Type.GetType("System.Int32"));
                dt.Columns.Add("DHSL", Type.GetType("System.Int32"));
                dt.Columns.Add("HSJE", Type.GetType("System.Double"));
                result = dt.Clone();
                string someDjbh = string.Empty;
                bool getDjbh = false;
                foreach (DataRow row in dt.Rows)
                {
                    if (btnCollect.Text == "全部商品")
                    {
                        row["BZSL"] = row["DEFSL"];
                    }
                    else
                    {
                        row["BZSL"] = "0";
                    }
                    row["DHSL"] = "0";
                    row["HSJE"] = "0";
                    //row["HSJJ"].ToString();
                    //if (string.IsNullOrWhiteSpace(row["HSJJ"].ToString()) || row["HSJJ"].ToString() == "0")
                    //{
                    //    msg += "【" + row["GDSEQ"].ToString() + " | " + row["GDNAME"].ToString() + "】,";
                    //    Alert.Show("商品" + msg + "【含税进价】为空，不能进行【科室申领录入】操作。", "消息提示", MessageBoxIcon.Warning);
                    //    continue;
                    //}
                    //处理金额格式
                    decimal jingdu = 0;
                    decimal bzhl = 0;
                    if (decimal.TryParse(row["HSJJ"].ToString(), out jingdu) && decimal.TryParse(row["BZHL"].ToString(), out bzhl)) { row["HSJJ"] = Math.Round(jingdu * bzhl, 4).ToString("F4"); }
                    if (decimal.TryParse(row["YBJ"].ToString(), out jingdu)) { row["YBJ"] = jingdu.ToString("F4"); }
                    if (decimal.TryParse(row["HSJE"].ToString(), out jingdu)) { row["HSJE"] = Math.Round(jingdu, 2).ToString("F2"); }
                    docMEMO.Enabled = true;
                    List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().OrderBy(x => x["GDSEQ"]).ToList();
                    int sameRowCount = newDict.Where(a => a["GDSEQ"].ToString() == row["GDSEQ"].ToString()).Count();
                    if (sameRowCount > 0)
                    {
                        someDjbh += "【" + row["GDNAME"].ToString() + "】";
                        getDjbh = true;
                    }
                    else
                    {
                        result.Rows.Add(row.ItemArray);
                        //PubFunc.GridRowAdd(GridGoods, row, false);
                        docDEPTID.Enabled = false;
                    }
                }
                if (getDjbh)
                {
                    Alert.Show("商品名称：" + someDjbh + "申请明细中已存在", "消息提示", MessageBoxIcon.Warning);
                }
            }
            else
            {
                Alert.Show("请先选择要加载的商品信息！", "消息提示", MessageBoxIcon.Warning);
            }
            PubFunc.GridRowAdd(GridGoods, result);
            WindowGoods.Hidden = true;
        }

        protected void btnClosePostBack_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            if (btnCollect.Text == "收藏商品")
            {
                dt = PubFunc.GridDataGet(GridCFGGoods);
            }
            else if (btnCollect.Text == "全部商品")
            {
                if (GridCFGGoods.SelectedRowIndexArray.Length == 0)
                {
                    Alert.Show("请选择商品信息！", "警告提醒", MessageBoxIcon.Warning);
                    return;
                }
                FineUIPro.GridRowCollection rows = new FineUIPro.GridRowCollection();
                for (int i = GridCFGGoods.SelectedRowIndexArray.Length - 1; i > -1; i--)
                {
                    rows.Add(GridCFGGoods.Rows[GridCFGGoods.SelectedRowIndexArray[i]]);
                }
                dt = GridDataGet(GridCFGGoods, rows);
            }
            dt.Columns.Remove(dt.Columns["BZHL"]);
            dt.Columns.Remove(dt.Columns["UNIT"]);

            DataGridBack(dt);
        }

        protected override void billCancel()
        {
            if (docBILLNO.Text.Length < 1)
            {
                Alert.Show("请选择需要驳回的单据!", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (docFLAG.SelectedValue.ToString() != "N")
            {
                Alert.Show("非『新增单据』不能驳回！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            WindowReject.Hidden = false;
        }

        protected void btnRejectSubmit_Click(object sender, EventArgs e)
        {
            if (ddlReject.SelectedText == "--请选择--")
            {
                Alert.Show("请选择驳回原因");
                return;
            }

            //增加待办事宜
            List<CommandInfo> cmdList = new List<CommandInfo>();
            cmdList.Add(new CommandInfo("UPDATE DAT_DO_LIST SET FLAG = 'Y' WHERE PARA='" + docBILLNO.Text.Trim() + "'", null));

            string strMemo = docMEMO.Text + ";驳回原因:" + ddlReject.SelectedText;
            if (!string.IsNullOrWhiteSpace(txaMemo.Text.Trim()))
            {
                strMemo += ";详细说明:" + txaMemo.Text;
            }
            if (strMemo.Length > 100)
            {
                Alert.Show("驳回备注超长！");
                return;

            }
            cmdList.Add(new CommandInfo(string.Format("update DAT_SL_DOC set flag='R',memo='{0}',SHR={1}，SHRQ=SYSDATE where seqno='{2}' and flag='N'", strMemo, "'" + UserAction.UserID + "'", docBILLNO.Text), null));
            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                WindowReject.Hidden = true;
                OperLog("科室申领", "驳回单据【" + docBILLNO.Text + "】，" + strMemo);
                billOpen(docBILLNO.Text);
                Alert.Show("驳回成功");
            }

        }
        protected override void billDel()
        {

            if (docBILLNO.Text.Trim() == "")
            {
                Alert.Show("请选择需要删除的单据", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (("MR").IndexOf(docFLAG.SelectedValue) < 0)
            {
                Alert.Show("非『新增』单据不能删除！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!CheckFlagForM() && !CheckFlagForR())
            {
                Alert.Show("此单据已经被别人操作，请等待操作!");
                return;
            }
            List<string> listSql = new List<string>();
            listSql.Add("Delete from DAT_SL_DOC t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            listSql.Add("Delete from DAT_SL_COM t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            listSql.Add("UPDATE DAT_DO_LIST SET FLAG = 'Y' WHERE PARA='" + docBILLNO.Text.Trim() + "'");
            if (DbHelperOra.ExecuteSqlTran(listSql))
            {
                Alert.Show("单据删除成功!", "消息提示", MessageBoxIcon.Information);
                OperLog("科室申领", "删除单据【" + docBILLNO.Text + "】");
                billNew();
                billSearch();
            }
            else
            {
                Alert.Show("单据删除失败!", "错误提示", MessageBoxIcon.Information);
            }
        }

        protected void btnAuditBatch_Click(object sender, EventArgs e)
        {
            int[] rowIndex = GridList.SelectedRowIndexArray;
            if (rowIndex.Length == 0)
            {
                Alert.Show("请选择要审批的科室申领信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            int count = 0;
            List<CommandInfo> sqlList = new List<CommandInfo>();
            foreach (int index in rowIndex)
            {
                if (GridList.Rows[index].Values[3].ToString() == "N")
                {
                    count += BillOper(GridList.Rows[index].Values[1].ToString(), "DECLARE");
                }
            }
            if (count > 0 && DbHelperOra.ExecuteSqlTran(sqlList))
            {
                Alert.Show("科室申领批量审批成功！", "消息提示", MessageBoxIcon.Information);
                OperLog("科室申领", "审批单据【" + docBILLNO.Text + "】");
                billSearch();
            }
        }

        protected void btExport_Click(object sender, EventArgs e)
        {
            if (GridList.Rows.Count < 1)
            {
                Alert.Show("没有数据,无法导出！");
                return;
            }
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【申领日期】！");
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                Alert.Show("【开始日期】大于【结束日期】，请重新输入！", "提示信息", MessageBoxIcon.Warning);
                return;
            }

            string strSql = @"SELECT A.BILLNO 单据编号,
                                       F_GETDEPTNAME(A.DEPTID) 申领部门,
                                       A.XSRQ 申领日期,
                                       F_GETDEPTNAME(A.DEPTOUT) 出库部门,
                                       F_GETUSERNAME(A.SLR) 申领人,
                                       F_GETUSERNAME(A.LRY) 录入人,
                                       A.LRRQ 录入日期,
                                       B.ROWNO 行号,
                                       ' '||B.GDSEQ 商品编码,
                                       B.GDNAME 商品名称,
                                       B.GDSPEC 商品规格,
                                       B.PZWH 注册证号,
                                       F_GETPRODUCERNAME(B.PRODUCER) 生产厂家,
                                       B.BZHL 包装含量,
                                       B.BZSL 申领包装数,
                                       B.DHSL 申领数量,
                                       B.FPSL 分配数量,
                                       F_GETUNITNAME(B.UNIT) 单位,
                                       B.HSJJ 价格,B.HSJE 金额
                                  FROM DAT_SL_DOC A, DAT_SL_COM B
                                 WHERE A.SEQNO=B.SEQNO  AND NVL(B.NUM1,0) = 0
                                   AND BILLTYPE IN ('LYD','GBD')
                                   AND A.XSTYPE = '1' ";
            string strSearch = "";
            if (Request.QueryString["oper"].ToString().ToLower() == "audit")
            {
                strSql = strSql + " AND A.FLAG<>'M'";

                //根据DOC_GOODSTYPE表中的STR1字段来确定当前登录人员是否有审核权限 By c 2015年11月30日15:04:15
                strSql += string.Format(" AND F_CHK_AUDIT(NVL(A.CATID,'2'),'{0}') = 'Y'", UserAction.UserID);
            }
            else
            {
                //根据商品类别【CATID】来加载信息，默认加载耗材-2  By c 2015年11月30日15:38:20
                if (Request.QueryString["tp"] != null && Request.QueryString["tp"].ToString() != "")
                {
                    strSql = strSql + string.Format(" AND NVL(A.CATID,'2') ='{0}'", Request.QueryString["tp"].ToString());
                }
                else
                {
                    strSql = strSql + " AND NVL(A.CATID,'2')  ='2'";
                }
            }

            if (tgbBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND A.BILLNO  LIKE '%{0}%'", tgbBILLNO.Text);
            }
            if (lstFLAG.SelectedItem != null && lstFLAG.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.FLAG='{0}'", lstFLAG.SelectedItem.Value);
            }
            if (lstLRY.SelectedItem != null && lstLRY.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.LRY='{0}'", lstLRY.SelectedItem.Value);
            }
            if (lstDEPTID.SelectedValue != null && lstDEPTID.SelectedValue.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedValue);
            }
            if (lstDEPTOUT.SelectedItem != null && lstDEPTOUT.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTOUT='{0}'", lstDEPTOUT.SelectedItem.Value);
            }

            strSearch += string.Format(" AND A.deptid in( select code FROM SYS_DEPT where type <>'1' and  F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            strSearch += string.Format(" AND A.LRRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.LRRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY A.BILLNO DESC,B.ROWNO";
            ExcelHelper.ExportByWeb(DbHelperOra.Query(strSql).Tables[0], "科室申领信息", string.Format("科室申领信息_{0}.xls", DateTime.Now.ToString("yyyyMMdd")));
            billSearch();
        }
        protected override void billCopy()
        {
            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_SL_DOC WHERE SEQNO = '{0}'", docSEQNO.Text)))
            {
                Alert.Show("单据【" + docSEQNO.Text + "】不存在,请检查！");
                return;
            }
            if (rblBILLTYPE.SelectedValue == "GBD")
            {
                BillType = "GBD";
            }
            //复制单据
            string billNew = BillSeqGet();
            DbHelperOra.ExecuteSql(string.Format(@"INSERT INTO DAT_SL_DOC(SEQNO,BILLNO,BILLTYPE,FLAG,DEPTOUT,DEPTID,CATID,SLR,XSTYPE,XSRQ,THTYPE,SUBNUM,LRY,LRRQ,STR1,MEMO)
                SELECT '{2}','{2}',BILLTYPE,'M',DEPTOUT,DEPTID,CATID,'{1}',XSTYPE,SYSDATE,THTYPE,SUBNUM,'{1}',SYSDATE,STR1,'复制单据{0}'
                FROM DAT_SL_DOC WHERE SEQNO = '{0}'", docSEQNO.Text, UserAction.UserID, billNew));
            DbHelperOra.ExecuteSql(string.Format(@"INSERT INTO DAT_SL_COM(SEQNO,ROWNO,GDSEQ,BARCODE,GDNAME,UNIT,GDSPEC,GDMODE,HWID,BZHL,BZSL,DHSL,XSSL,JXTAX,HSJJ,BHSJJ,HSJE,BHSJE,LSJ,LSJE,ISGZ,ISLOT,PHID,PH,PZWH,RQ_SC,YXQZ,PRODUCER,ZPBH,STR1,MEMO)
                SELECT '{1}',ROWNO,GDSEQ,BARCODE,GDNAME,UNIT,GDSPEC,GDMODE,HWID,BZHL,BZSL,DHSL,0 XSSL,JXTAX,HSJJ,BHSJJ,HSJE,BHSJE,LSJ,LSJE,ISGZ,ISLOT,PHID,PH,PZWH,RQ_SC,YXQZ,PRODUCER,ZPBH,STR1,MEMO
                FROM DAT_SL_COM WHERE SEQNO = '{0}' AND NVL(NUM1,0) = 0", docSEQNO.Text, billNew));
            billOpen(billNew);

            Alert.Show("新单据【" + billNew + "】生成成功！");
            docMEMO.Enabled = true;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (docSEQNO.Text.Length < 1)
            {
                Alert.Show("申领单:" + docSEQNO.Text + "没有保存，不能提交!", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string flg = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M') FROM DAT_SL_DOC WHERE SEQNO='{0}'", docSEQNO.Text));
            if (!PubFunc.StrIsEmpty(flg) && (",M").IndexOf(flg) < 0)
            {
                Alert.Show("申领单:" + docSEQNO.Text + "不是新增单据，不能提交!", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            //增加待办事宜
            List<CommandInfo> cmdList = new List<CommandInfo>();
            cmdList.Add(new CommandInfo(string.Format("update DAT_SL_DOC set flag='N' where seqno='{0}' and flag='M'", docBILLNO.Text), null));
            if (!DbHelperOra.Exists("select * from DAT_DO_LIST where PARA='" + docBILLNO.Text.Trim() + "'"))
            {
                cmdList.Add(new CommandInfo(" UPDATE DAT_DO_LIST SET DOUSER='" + UserAction.UserID + "',DORQ=SYSDATE,FLAG='Y' WHERE DOTYPE='" + docDEPTID.SelectedValue + "' AND PARA='" + docBILLNO.Text + "'", null));
                cmdList.Add(Doc.GETDOADD("DO_4", docDEPTID.SelectedValue, docLRY.SelectedValue, docBILLNO.Text));
            }

            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                Alert.Show("提交成功");
                OperLog("科室申领", "提交单据【" + docBILLNO.Text + "】");
                billOpen(docSEQNO.Text);
            }
        }

        private bool CheckFlagForM()
        {
            if (docBILLNO.Text.Length > 0)
            {
                return Doc.getFlag(docBILLNO.Text, "M", BillType);
            }
            return true;
        }

        private bool CheckFlagForR()
        {
            if (docBILLNO.Text.Length > 0)
            {
                return Doc.getFlag(docBILLNO.Text, "R", BillType);
            }
            return true;
        }

        protected void PageManager1_CustomEvent(object sender, CustomEventArgs e)
        {
            switch (e.EventArgument)
            {
                case "GoodsAdd": Window3_Close(null, null); break;
                case "CONTROLM_ENTER": billGoods(); break;
            }
        }

        #region 弹出商品资料界面
        protected void GridNoSelectGoods_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridNoSelectGoods.PageIndex = e.NewPageIndex;
            dataSearch();
        }

        protected void btnAddRight_Click(object sender, EventArgs e)
        {
            for (int i = GridNoSelectGoods.SelectedRowIndexArray.Length - 1; i > -1; i--)
            {
                goodsAddRow(GridNoSelectGoods.SelectedRowIndexArray[i]);
            }
            GridNoSelectGoods.SelectedRowIndexArray = new int[0];
            dataSearch();
        }
        protected void btnAddLeft_Click(object sender, EventArgs e)
        {
            for (int i = GridCFGGoods.SelectedRowIndexArray.Length - 1; i > -1; i--)
            {
                goodsRemoveRow(GridCFGGoods.SelectedRowIndexArray[i]);
            }
            GridCFGGoods.SelectedRowIndexArray = new int[0];
            dataSearch();

        }

        protected void tgbSearch_TriggerClick(object sender, EventArgs e)
        {
            dataSearch();
        }

        protected void GridCFGGoods_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            if (btnCollect.Text == "收藏商品" && GridNoSelectGoods.Hidden == false)
            {
                goodsRemoveRow(e.RowIndex);
            }
            else
            {
                FineUIPro.GridRowCollection rows = new FineUIPro.GridRowCollection();
                rows.Add(GridCFGGoods.Rows[e.RowIndex]);
                DataTable dt = GridDataGet(GridCFGGoods, rows);

                dt.Columns.Remove(dt.Columns["BZHL"]);
                dt.Columns.Remove(dt.Columns["UNIT"]);

                DataGridBack(dt);
            }
        }
        private void goodsRemoveRow(int rowIndex)
        {
            GridRow dr = GridCFGGoods.Rows[rowIndex];
            RemoveGridDetail(dr);

        }
        private void RemoveGridDetail(GridRow dr)
        {
            GridNoSelectGoods.Rows.Insert(0, dr);
            GridCFGGoods.Rows.Remove(dr);
        }

        private static DataTable newDt = new DataTable();
        private void dataSearch()
        {
            int total = 0;
            string msg = "";

            DataTable dt = GetGoodsList(GridNoSelectGoods.PageIndex, GridNoSelectGoods.PageSize, ref total, ref msg);

            GridNoSelectGoods.DataSource = dt;
            GridNoSelectGoods.DataBind();
            GridNoSelectGoods.RecordCount = total;
        }

        /// <summary>
        /// 获取商品数据信息
        /// </summary>
        /// <param name="pageNum">第几页</param>
        /// <param name="pageSize">每页显示天数</param>
        /// <param name="nvc">查询条件</param>
        /// <param name="total">总的条目数</param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public DataTable GetGoodsList(int pageNum, int pageSize, ref int total, ref string errMsg)
        {
            string sql = "";
            if (DbHelperOra.Exists("SELECT 1 FROM SYS_PARA WHERE CODE = 'ShowName' AND VALUE = 'HIS'"))
            {
                //使用his名称、规格,SP.GDNAME,SP.GDSPEC
                sql = @"SELECT  SP.GDSEQ,SP.GDID,SP.BARCODE,SP.ZJM,SP.YCODE,SP.NAMEJC,SP.NAMEEN,SP.GDMODE,SP.STRUCT,SP.BZHL,SP.UNIT,SP.FLAG,SP.CATID,SP.JX,SP.YX,SP.PIZNO,SP.BAR1,SP.BAR2,SP.BAR3,SP.DEPTID,SP.SUPPLIER,SP.LOGINLABEL,SP.PRODUCER,SP.ZPBH,SP.PPID,SP.CDID,SP.JXTAX,SP.XXTAX,SP.BHSJJ,SP.HSJJ,SP.LSJ,SP.YBJ,SP.HSID,SP.HSJ,SP.JHZQ,SP.ZDKC,
                                    SP.HLKC,SP.ZGKC,SP.SPZT,SP.DAYXS,SP.MANAGER,SP.INPER,SP.INRQ,SP.BEGRQ,SP.ENDRQ,SP.UPTRQ,SP.UPTUSER,SP.MEMO,DISABLEORG,SP.ISLOT,SP.ISJB,SP.ISFZ,SP.ISGZ,SP.ISIN,SP.ISJG,SP.ISDM,SP.ISCF,SP.ISYNZJ,SP.ISFLAG1,
                                    NVL(SP.STR3,SP.GDSPEC) GDSPEC,SP.UNIT_DABZ,SP.UNIT_ZHONGBZ,SP.BARCODE_DABZ,SP.NUM_DABZ,SP.NUM_ZHONGBZ,SP.HISCODE,NVL(SP.HISNAME,SP.GDNAME) GDNAME,SP.CATID0,
                                    F_GETUNITNAME(UNIT) UNITNAME,F_GETPRODUCERNAME(SP.PRODUCER) PRODUCERNAME,F_GETSUPNAME(SUPPLIER) SUPPLIERNAME, 
                                   F_GETUNITNAME(UNIT_DABZ) UNIT_DABZ_NAME,
                                   F_GETUNITNAME(UNIT_ZHONGBZ) UNIT_ZHONGBZ_NAME,NVL(PZ.HJCODE1,PZ.DEPTID) HWID,
                                   DECODE(SP.UNIT_ORDER,'D',SP.UNIT_DABZ,'Z',SP.UNIT_ZHONGBZ,SP.UNIT) UNIT_ORDER,
								   DECODE(SP.UNIT_SELL,'D',SP.UNIT_DABZ,'Z',SP.UNIT_ZHONGBZ,SP.UNIT) UNIT_SELL,
								   F_GETUNITNAME(DECODE(SP.UNIT_SELL,'D',SP.UNIT_DABZ,'Z',SP.UNIT_ZHONGBZ,SP.UNIT)) UNIT_SELL_NAME,
								   F_GETUNITNAME(DECODE(SP.UNIT_ORDER,'D',SP.UNIT_DABZ,'Z',SP.UNIT_ZHONGBZ,SP.UNIT)) UNIT_ORDER_NAME,
                                   DECODE(SP.UNIT_ORDER,'D',SP.NUM_DABZ,'Z',SP.NUM_ZHONGBZ,SP.BZHL) BZHL_ORDER,
								   DECODE(SP.UNIT_SELL,'D',SP.NUM_DABZ,'Z',SP.NUM_ZHONGBZ,SP.BZHL) BZHL_SELL,NVL(DEFSL,0) DEFSL,
                                   NVL(PZ.ISJF,'Y') ISJF
                             FROM  DOC_GOODS SP,DOC_GOODSCFG PZ,DOC_MYGOODS SC WHERE ISDELETE='N' AND SP.FLAG='Y' AND SP.GDSEQ=PZ.GDSEQ   ";
            }
            else
            {
                sql = @"SELECT  SP.GDSEQ,SP.GDID,SP.BARCODE,SP.ZJM,SP.YCODE,SP.NAMEJC,SP.NAMEEN,SP.GDMODE,SP.STRUCT,SP.BZHL,SP.UNIT,SP.FLAG,SP.CATID,SP.JX,SP.YX,SP.PIZNO,SP.BAR1,SP.BAR2,SP.BAR3,SP.DEPTID,SP.SUPPLIER,SP.LOGINLABEL,SP.PRODUCER,SP.ZPBH,SP.PPID,SP.CDID,SP.JXTAX,SP.XXTAX,SP.BHSJJ,SP.HSJJ,SP.LSJ,SP.YBJ,SP.HSID,SP.HSJ,SP.JHZQ,SP.ZDKC,
                                    SP.HLKC,SP.ZGKC,SP.SPZT,SP.DAYXS,SP.MANAGER,SP.INPER,SP.INRQ,SP.BEGRQ,SP.ENDRQ,SP.UPTRQ,SP.UPTUSER,SP.MEMO,DISABLEORG,SP.ISLOT,SP.ISJB,SP.ISFZ,SP.ISGZ,SP.ISIN,SP.ISJG,SP.ISDM,SP.ISCF,SP.ISYNZJ,SP.ISFLAG1,
                                    SP.GDSPEC,SP.UNIT_DABZ,SP.UNIT_ZHONGBZ,SP.BARCODE_DABZ,SP.NUM_DABZ,SP.NUM_ZHONGBZ,SP.HISCODE,SP.GDNAME,SP.CATID0,
                                    F_GETUNITNAME(UNIT) UNITNAME,F_GETPRODUCERNAME(SP.PRODUCER) PRODUCERNAME,F_GETSUPNAME(SUPPLIER) SUPPLIERNAME, 
                                   F_GETUNITNAME(UNIT_DABZ) UNIT_DABZ_NAME,
                                   F_GETUNITNAME(UNIT_ZHONGBZ) UNIT_ZHONGBZ_NAME,NVL(PZ.HJCODE1,PZ.DEPTID) HWID,
                                   DECODE(SP.UNIT_ORDER,'D',SP.UNIT_DABZ,'Z',SP.UNIT_ZHONGBZ,SP.UNIT) UNIT_ORDER,
								   DECODE(SP.UNIT_SELL,'D',SP.UNIT_DABZ,'Z',SP.UNIT_ZHONGBZ,SP.UNIT) UNIT_SELL,
								   F_GETUNITNAME(DECODE(SP.UNIT_SELL,'D',SP.UNIT_DABZ,'Z',SP.UNIT_ZHONGBZ,SP.UNIT)) UNIT_SELL_NAME,
								   F_GETUNITNAME(DECODE(SP.UNIT_ORDER,'D',SP.UNIT_DABZ,'Z',SP.UNIT_ZHONGBZ,SP.UNIT)) UNIT_ORDER_NAME,
                                   DECODE(SP.UNIT_ORDER,'D',SP.NUM_DABZ,'Z',SP.NUM_ZHONGBZ,SP.BZHL) BZHL_ORDER,
								   DECODE(SP.UNIT_SELL,'D',SP.NUM_DABZ,'Z',SP.NUM_ZHONGBZ,SP.BZHL) BZHL_SELL,NVL(DEFSL,0) DEFSL,
                                   NVL(PZ.ISJF,'Y') ISJF
                             FROM  DOC_GOODS SP,DOC_GOODSCFG PZ,DOC_MYGOODS SC  WHERE ISDELETE='N' AND SP.FLAG='Y' AND SP.GDSEQ=PZ.GDSEQ ";
            }
            if (btnCollect.Text == "全部商品" && GridNoSelectGoods.Hidden == true)
            {
                sql += " AND PZ.GDSEQ=SC.GDSEQ AND PZ.DEPTID=SC.DEPTID ";
            }
            else
            {
                sql += " AND PZ.GDSEQ=SC.GDSEQ(+) AND PZ.DEPTID=SC.DEPTID(+) ";
            }
            if (ddlCATID.SelectedValue.Length > 0)
            {
                sql += " AND SP.CATID0 = '" + ddlCATID.SelectedValue + "' ";
            }
            StringBuilder strSql = new StringBuilder(sql);
            if (!string.IsNullOrWhiteSpace(tgbSearch.Text))
            {
                strSql.AppendFormat(" AND (SP.GDSEQ LIKE '%{0}%' OR SP.GDNAME LIKE '%{0}%' OR SP.ZJM LIKE '%{0}%' OR SP.BARCODE LIKE '%{0}%' OR SP.HISCODE LIKE '%{0}%' OR SP.HISNAME LIKE '%{0}%' OR SP.STR4 LIKE '%{0}%')", tgbSearch.Text.ToUpper());
            }
            strSql.AppendFormat(" AND PZ.DEPTID='{0}' AND PZ.ISCFG in('Y','1')", docDEPTID.SelectedValue);

            if (rblBILLTYPE.SelectedValue == "GBD")
            {
                strSql.AppendFormat(" AND SP.ISGZ = 'Y'");
                //增加商品是否配置供应商验证
                strSql.AppendFormat(" and SP.gdseq in (select A.gdseq from DOC_GOODSCFG A,doc_goodssup B, doc_supplier C where A.DEPTID = '{0}' AND A.GDSEQ = B.GDSEQ AND B.SUPID = C.SUPID AND C.STR1 = 'N')", docDEPTOUT.SelectedValue);
            }
            //增加商品是否配置供应商验证
            strSql.AppendFormat(" and SP.gdseq in (select A.gdseq from DOC_GOODSCFG A,doc_goodssup B where A.DEPTID = '{0}' AND A.GDSEQ = B.GDSEQ)", docDEPTOUT.SelectedValue);

            if (GridCFGGoods.Rows.Count > 0)
            {
                string lstGDSEQ = string.Empty;
                for (int i = 0; i < GridCFGGoods.Rows.Count; i++)
                {
                    lstGDSEQ += "'" + GridCFGGoods.Rows[i].DataKeys[0] + "',";
                }
                lstGDSEQ = lstGDSEQ.Remove(lstGDSEQ.Length - 1, 1);
                strSql.AppendFormat(" and SP.GDSEQ not in (" + lstGDSEQ + ")");
            }

            strSql.Append(" order by SP.GDSEQ,SP.GDNAME");
            newDt = DbHelperOra.Query(strSql.ToString()).Tables[0];
            return PubFunc.DbGetPage(pageNum, pageSize, strSql.ToString(), ref total);
        }

        protected void GridNoSelectGoods_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            goodsAddRow(e.RowIndex);
        }
        private void goodsAddRow(int rowIndex)
        {
            GridRow dr = GridNoSelectGoods.Rows[rowIndex];
            addGridDetail(dr);
        }
        private void addGridDetail(GridRow dr)
        {
            GridCFGGoods.Rows.Insert(0, dr);
            GridNoSelectGoods.Rows.Remove(dr);
        }

        #endregion

        protected void GridList_Sort(object sender, GridSortEventArgs e)
        {
            GridList.SortDirection = e.SortDirection;
            GridList.SortField = e.SortField;
            DataTable table = PubFunc.GridDataGet(GridList);
            DataTable tbnew = table.Clone();
            tbnew.Columns["SUBSUM"].DataType = typeof(double);//指定SUBSUM为double类型
            foreach (DataRow s in table.Rows)
            {
                tbnew.ImportRow(s);//导入旧数据
            }

            DataView view1 = tbnew.DefaultView;
            view1.Sort = String.Format("{0} {1}", GridList.SortField, GridList.SortDirection);
            GridList.DataSource = view1;
            GridList.DataBind();
        }

        protected void btnSaveTemplate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(docDEPTID.SelectedValue))
            {
                Alert.Show("请选择要保存模板的科室", "警告提醒", MessageBoxIcon.Warning);
                return;
            }
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().ToList();
            if (newDict.Count > 0)
            {
                Window1.Hidden = false;
            }
            else
            {
                Alert.Show("保存模板之前请先添加商品明细信息！", "警告提醒", MessageBoxIcon.Warning);
            }
        }

        private void GridTemplateLoad()
        {
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("TEMPLATE", typeof(string)));
            table.Columns.Add(new DataColumn("TEMPLATENAME", typeof(string)));
            table.Columns.Add(new DataColumn("USERNAME", typeof(string)));

            string strPath = AppDomain.CurrentDomain.BaseDirectory + "ERPUpload/Json/";
            if (!Directory.Exists(strPath))
            {
                Directory.CreateDirectory(strPath);
            }
            string[] dirs = Directory.GetFiles(strPath);
            foreach (string dir in dirs)
            {
                string dirName = dir.Substring(dir.LastIndexOf('/') + 1);
                if (dirName.StartsWith(docDEPTID.SelectedValue + "_"))
                {
                    string fileName = dirName.Substring(0, dirName.IndexOf('.'));
                    string[] arrDir = fileName.Split('_');
                    if (arrDir.Length < 5) continue;
                    if (arrDir[3] == ddlCATID.SelectedValue && arrDir[4] == docDEPTOUT.SelectedValue)
                    {
                        DataRow row = table.NewRow();
                        row[0] = dir;
                        row[1] = arrDir[2];
                        row[2] = docLRY.Items.FindByValue(arrDir[1]).Text;
                        table.Rows.Add(row);
                    }
                }
            }
            GridTemplate.DataSource = table;
            GridTemplate.DataBind();
        }

        protected void btnLoadTemplate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(docDEPTID.SelectedValue))
            {
                Alert.Show("请选择要载入模板的科室", "警告提醒", MessageBoxIcon.Warning);
                return;
            }

            string sql = @"SELECT GROUPID, GROUPNAME, F_GETUSERNAME(LRY) USERNAME
                                  FROM DOC_GROUPDOC WHERE FLAG='Y' AND DEPTID = '{0}'AND (TYPE='K'OR TYPE='Y') ORDER BY GROUPNAME";
            GridTemplate.DataSource = DbHelperOra.Query(string.Format(sql, docDEPTID.SelectedValue)).Tables[0];
            GridTemplate.DataBind();

            Window2.Hidden = false;
        }

        protected void btnSaveTemplateClose_Click(object sender, EventArgs e)
        {
            FineUIPro.Button btn = sender as FineUIPro.Button;
            if (btn.ID == "btnSaveTemplateClose")
            {
                if (string.IsNullOrWhiteSpace(tbsFileName.Text))
                {
                    Alert.Show("请输入模板名称！", "警告提醒", MessageBoxIcon.Warning);
                    return;
                }

                List<CommandInfo> cmdList = new List<CommandInfo>();
                MyTable doc = new MyTable("DOC_GROUPDOC");
                object maxid = DbHelperOra.GetSingle("SELECT MAX(GROUPID) FROM DOC_GROUPDOC WHERE GROUPID LIKE 'ZU%'");
                if (maxid == null || maxid.ToString() == "")
                {
                    doc.ColRow["GROUPID"] = "ZU00001";
                }
                else
                {
                    doc.ColRow["GROUPID"] = "ZU" + (100001 + int.Parse(maxid.ToString().Substring(2))).ToString().Substring(1);
                }
                doc.ColRow["GROUPNAME"] = tbsFileName.Text;
                doc.ColRow["FLAG"] = "Y";
                doc.ColRow["TYPE"] = "Y";
                doc.ColRow["DEPTID"] = docDEPTID.SelectedValue;
                doc.ColRow["CATID"] = ddlCATID.SelectedValue;
                doc.ColRow["LRY"] = UserAction.UserID;
                doc.ColRow.Remove("LRRQ");
                List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().ToList();
                for (int i = 0; i < newDict.Count; i++)
                {
                    MyTable com = new MyTable("DOC_GROUPCOM");
                    com.ColRow["GROUPID"] = doc.ColRow["GROUPID"];
                    com.ColRow["ROWNO"] = i + 1;
                    com.ColRow["GDSEQ"] = newDict[i]["GDSEQ"].ToString();
                    com.ColRow["GDNAME"] = newDict[i]["GDNAME"].ToString();
                    com.ColRow["GDSPEC"] = newDict[i]["GDSPEC"].ToString();
                    com.ColRow["BZHL"] = newDict[i]["BZHL"].ToString();
                    com.ColRow["UNIT"] = newDict[i]["UNIT"].ToString();
                    com.ColRow["SL"] = decimal.Parse(newDict[i]["BZHL"].ToString()) * decimal.Parse(newDict[i]["BZSL"].ToString());
                    com.ColRow["HSJJ"] = newDict[i]["HSJJ"].ToString();
                    com.ColRow["MEMO"] = newDict[i]["MEMO"].ToString();
                    cmdList.Add(com.Insert());
                }
                doc.ColRow["SUBNUM"] = newDict.Count;
                cmdList.Add(doc.Insert());

                DbHelperOra.ExecuteSqlTran(cmdList);
                Window1.Hidden = true;
            }
            else if (btn.ID == "btnLoadTemplateClose")
            {
                if (GridTemplate.SelectedRowIndex < 0)
                {
                    Alert.Show("请选择要加载的模板！", "警告提醒", MessageBoxIcon.Warning);
                    return;
                }

                hfdTemplateName.Text = GridTemplate.Rows[GridTemplate.SelectedRowIndex].DataKeys[1].ToString();
                StringBuilder sbSql = new StringBuilder();
                sbSql.AppendFormat(strGoodsSql, docDEPTID.SelectedValue, GridTemplate.Rows[GridTemplate.SelectedRowIndex].DataKeys[0].ToString());
                DataTable dtGoods = DbHelperOra.Query(sbSql.ToString()).Tables[0];
                if (dtGoods != null && dtGoods.Rows.Count > 0)
                {
                    foreach (DataRow row in dtGoods.Rows)
                    {
                        PubFunc.GridRowAdd(GridGoods, row, false);
                    }
                    Window2.Hidden = true;
                }
                else
                {
                    string file = GridTemplate.Rows[GridTemplate.SelectedRowIndex].DataKeys[1].ToString();
                    Alert.Show("模板【" + file + "】内容为空或模版中商品被取消配置！", "警告提醒", MessageBoxIcon.Warning);
                }
            }
        }

        protected void GridTemplate_RowCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "FileDelete")
            {
                DbHelperOra.ExecuteSql("UPDATE DOC_GROUPDOC SET FLAG='N' WHERE GROUPID='" + GridTemplate.Rows[e.RowIndex].DataKeys[0].ToString() + "'");
                GridTemplateLoad();
            }
        }

        protected void btnCollect_Click(object sender, EventArgs e)
        {
            FineUIPro.Button btn = sender as FineUIPro.Button;
            if (btn.Text == "收藏商品")
            {
                GridNoSelectGoods.Hidden = true;
                Panel4.Hidden = true;
                btnCollect.Text = "全部商品";

                int total = 0;
                string msg = "";

                DataTable dt = GetGoodsList(GridNoSelectGoods.PageIndex, GridNoSelectGoods.PageSize, ref total, ref msg);

                GridCFGGoods.DataSource = dt;
                GridCFGGoods.DataBind();
                GridCFGGoods.RecordCount = total;
            }
            else if (btn.Text == "全部商品")
            {
                GridNoSelectGoods.Hidden = false;
                Panel4.Hidden = false;
                btnCollect.Text = "收藏商品";

                GridCFGGoods.DataSource = null;
                GridCFGGoods.DataBind();
                GridCFGGoods.RecordCount = 0;
            }
        }

        /// <summary>
        /// FineUIPro.Grid控件的数据转换成DataTable
        /// </summary>
        /// <param name="grid">Grid控件</param>
        /// <returns>DataTable数据源</returns>
        public static DataTable GridDataGet(FineUIPro.Grid grid, FineUIPro.GridRowCollection rows)
        {
            int rowCount = 0;
            DataTable dtGrid = new DataTable();
            foreach (GridColumn gc in grid.Columns)
            {
                if (gc is FineUIPro.BoundField)
                {
                    dtGrid.Columns.Add(new DataColumn(((FineUIPro.BoundField)(gc)).DataField.ToUpper(), typeof(string)));
                }
                else if (gc is FineUIPro.RenderField)
                {
                    dtGrid.Columns.Add(new DataColumn(((FineUIPro.RenderField)(gc)).DataField.ToUpper(), typeof(string)));
                }
                else if (gc is FineUIPro.TemplateField)
                {
                    dtGrid.Columns.Add(new DataColumn(((FineUIPro.TemplateField)(gc)).ColumnID.ToUpper(), typeof(string)));
                }
                rowCount++;
            }
            if (rowCount > 0)
            {
                foreach (GridRow gr in rows)
                {
                    DataRow row = dtGrid.NewRow();
                    foreach (GridColumn gc in grid.Columns)
                    {
                        if (gc is FineUIPro.BoundField)
                        {
                            row[((FineUIPro.BoundField)(gc)).DataField.ToUpper()] = gr.Values[gc.ColumnIndex];
                        }
                        if (gc is FineUIPro.RenderField)
                        {
                            row[((FineUIPro.RenderField)(gc)).DataField.ToUpper()] = gr.Values[gc.ColumnIndex];
                        }
                        else if (gc is FineUIPro.TemplateField)
                        {
                            row[((FineUIPro.TemplateField)(gc)).ColumnID.ToUpper()] = gr.Values[gc.ColumnIndex];
                        }
                    }
                    dtGrid.Rows.Add(row);
                }
            }
            return dtGrid;
        }

        protected void GridTemplate_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            hfdTemplateName.Text = GridTemplate.Rows[e.RowIndex].DataKeys[1].ToString();
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat(strGoodsSql, docDEPTID.SelectedValue, GridTemplate.Rows[e.RowIndex].DataKeys[0].ToString());
            DataTable dtGoods = DbHelperOra.Query(sbSql.ToString()).Tables[0];
            if (dtGoods != null && dtGoods.Rows.Count > 0)
            {
                foreach (DataRow row in dtGoods.Rows)
                {
                    JObject defaultObj = new JObject();
                    foreach (DataColumn col in row.Table.Columns)
                    {
                        if (col.ColumnName.ToUpper() == "RQ_SC" || col.ColumnName.ToUpper() == "YXQZ")
                        {
                            if (!PubFunc.StrIsEmpty(row[col.ColumnName].ToString()))
                            {
                                defaultObj.Add(col.ColumnName.ToUpper(), DateTime.Parse(row[col.ColumnName].ToString()).ToString("yyyy-MM-dd"));
                            }
                        }
                        else
                        {
                            defaultObj.Add(col.ColumnName.ToUpper(), row[col.ColumnName].ToString());
                        }
                    }

                    PageContext.RegisterStartupScript(GridGoods.GetAddNewRecordReference(defaultObj, true));
                }
                Window2.Hidden = true;
            }
            else
            {
                string file = GridTemplate.Rows[GridTemplate.SelectedRowIndex].DataKeys[1].ToString();
                Alert.Show("模板【" + file + "】内容为空或模版中商品被取消配置！", "警告提醒", MessageBoxIcon.Warning);
            }
        }
        protected void GridList_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridList.PageIndex = e.NewPageIndex;
            billSearch();
        }

        protected void docDEPTID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OPER())
            {
                rblBILLTYPE.Enabled = true;
                GetBudgetAndExec();
            }
            else
            {

                rblBILLTYPE.Enabled = false;
                rblBILLTYPE.SelectedIndex = 0;
            }
            if (docDEPTID.SelectedValue.Length > 0)
            {
                GetBudgetAndExec();
            }
        }

        protected void comGDSEQ_Click(object sender, EventArgs e)
        {
            Alert.Show(e.ToString());
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
        protected void Window3_Close(object sender, WindowCloseEventArgs e)
        {
            DataTable dt = GetGoods(hfdValue.Text);
            dt.Columns.Remove(dt.Columns["BZHL"]);
            dt.Columns.Remove(dt.Columns["UNIT"]);
            string msg = "";

            if (dt != null && dt.Rows.Count > 0)
            {
                dt.Columns["UNITNAME"].ColumnName = "UNITSMALLNAME";
                dt.Columns["UNIT_SELL_NAME"].ColumnName = "UNITNAME";
                dt.Columns["UNIT_SELL"].ColumnName = "UNIT";
                dt.Columns["BZHL_SELL"].ColumnName = "BZHL";

                dt.Columns.Add("PH", Type.GetType("System.String"));
                dt.Columns.Add("RQ_SC", Type.GetType("System.String"));
                dt.Columns.Add("YXQZ", Type.GetType("System.String"));
                dt.Columns.Add("MEMO", Type.GetType("System.String"));
                dt.Columns.Add("BZSL", Type.GetType("System.Int32"));
                dt.Columns.Add("DHSL", Type.GetType("System.Int32"));
                dt.Columns.Add("HSJE", Type.GetType("System.Double"));

                string someDjbh = string.Empty;
                bool getDjbh = false;
                foreach (DataRow row in dt.Rows)
                {
                    if (btnCollect.Text == "全部商品")
                    {
                        row["BZSL"] = row["DEFSL"];
                    }
                    else
                    {
                        row["BZSL"] = "0";
                    }
                    row["MEMO"] = row["ISZS"];
                    row["DHSL"] = "0";
                    row["HSJE"] = "0";
                    //row["HSJJ"].ToString();
                    //if (string.IsNullOrWhiteSpace(row["HSJJ"].ToString()) || row["HSJJ"].ToString() == "0")
                    //{
                    //    msg += "【" + row["GDSEQ"].ToString() + " | " + row["GDNAME"].ToString() + "】,";
                    //    Alert.Show("商品" + msg + "【含税进价】为空，不能进行【科室申领录入】操作。", "消息提示", MessageBoxIcon.Warning);
                    //    continue;
                    //}
                    //处理金额格式
                    decimal jingdu = 0;
                    decimal bzhl = 0;
                    if (decimal.TryParse(row["HSJJ"].ToString(), out jingdu) && decimal.TryParse(row["BZHL"].ToString(), out bzhl)) { row["HSJJ"] = Math.Round(jingdu * bzhl, 4).ToString("F4"); }
                    if (decimal.TryParse(row["YBJ"].ToString(), out jingdu)) { row["YBJ"] = jingdu.ToString("F4"); }
                    if (decimal.TryParse(row["HSJE"].ToString(), out jingdu)) { row["HSJE"] = Math.Round(jingdu, 2).ToString("F2"); }
                    docMEMO.Enabled = true;
                    List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().OrderBy(x => x["GDSEQ"]).ToList();
                    int sameRowCount = newDict.Where(a => a["GDSEQ"].ToString() == row["GDSEQ"].ToString()).Count();
                    if (sameRowCount > 0)
                    {
                        someDjbh += "【" + row["GDNAME"].ToString() + "】";
                        getDjbh = true;
                    }
                    else
                    {
                        PubFunc.GridRowAdd(GridGoods, row, false);
                        docDEPTID.Enabled = false;
                    }
                }
                if (getDjbh)
                {
                    Alert.Show("商品名称：" + someDjbh + "申请明细中已存在", "消息提示", MessageBoxIcon.Warning);
                }
            }
            else
            {
                Alert.Show("请先选择要加载的商品信息！", "消息提示", MessageBoxIcon.Warning);
            }
        }
        public void GetBudgetAndExec()
        {
            //          string strSQL = string.Format(@"SELECT  F_GETYSTOTAL(A.DEPTID,'{0}','{1}') PRENUM, NVL(SUM(A.SUBSUM),0) AS EXENUM
            //                                          FROM DAT_CK_DOC A WHERE A.FLAG IN ('Y', 'G')  AND BILLTYPE='CKD'  AND A.SHRQ BETWEEN TO_DATE('{1}', 'YYYY-MM-dd') AND   TO_DATE('{2}', 'YYYY-MM-dd') + 1    AND A.DEPTID = '{3}' GROUP BY DEPTID", Convert.ToDateTime(docXSRQ.SelectedDate).AddMonths(-1).ToString("yyyy-MM") + "-01", Convert.ToDateTime(docXSRQ.SelectedDate).ToString("yyyy-MM") + "-01", Convert.ToDateTime(docXSRQ.SelectedDate).ToString("yyyy-MM-dd"), docDEPTID.SelectedValue.ToString());

            string strSQL = string.Format(@"SELECT  F_GETYSTOTAL(A.DEPTID,'{0}','{1}') PRENUM, ABS(NVL(SUM(B.HSJE),0)) AS EXENUM
                                         FROM DAT_XS_DOC A,DAT_XS_COM B WHERE A.SEQNO=B.SEQNO AND A.FLAG IN ('Y', 'G')  AND A.SHRQ BETWEEN TO_DATE('{1}', 'YYYY-MM-dd') AND   TO_DATE('{2}', 'YYYY-MM-dd') + 1    AND A.DEPTID = '{3}' GROUP BY DEPTID", Convert.ToDateTime(docXSRQ.SelectedDate).AddMonths(-1).ToString("yyyy-MM") + "-01", Convert.ToDateTime(docXSRQ.SelectedDate).ToString("yyyy-MM") + "-01", Convert.ToDateTime(docXSRQ.SelectedDate).ToString("yyyy-MM-dd"), docDEPTID.SelectedValue.ToString());
            string ACCOUNTDAY = Doc.DbGetSysPara("ACCOUNTDAY");


            if (ACCOUNTDAY != "31")
            {
                strSQL = string.Format(@"SELECT  NVL((SELECT D.SUBSUM    FROM DAT_YS_DOC D  WHERE  D.FLAG = 'S' AND   D.SHRQ BETWEEN TO_DATE('{0}','YYYY-MM-dd') AND  TO_DATE('{1}', 'YYYY-MM-dd') + 1 AND DEPTID =A.DEPTID  AND   D.Begrq<TO_DATE('{2}','YYYY-MM-DD')  AND  D.ENDRQ> TO_DATE('{2}','YYYY-MM-DD')),0) PRENUM, NVL(SUM(A.SUBSUM),0) AS EXENUM
                                          FROM DAT_CK_DOC A WHERE A.FLAG IN ('Y', 'G') AND BILLTYPE='CKD'   AND A.SHRQ BETWEEN TO_DATE('{2}', 'YYYY-MM-dd') AND   TO_DATE('{3}', 'YYYY-MM-dd') + 1    AND A.DEPTID = '{4}' GROUP BY DEPTID", Convert.ToDateTime((Convert.ToDateTime(docXSRQ.SelectedDate).ToString("yyyy-MM") + "-" + ACCOUNTDAY)).AddMonths(-1).ToString("yyyy-MM-dd"), Convert.ToDateTime(docXSRQ.SelectedDate).ToString("yyyy-MM") + "-" + ACCOUNTDAY, Convert.ToDateTime(docXSRQ.SelectedDate).ToString("yyyy-MM-dd"), Convert.ToDateTime(docXSRQ.SelectedDate).AddMonths(1).ToString("yyyy-MM") + "-" + ACCOUNTDAY, docDEPTID.SelectedValue.ToString());



            }
            DataTable dtnull = DbHelperOra.QueryForTable(strSQL);
            if (dtnull.Rows.Count < 1)
            {
                if (PubFunc.StrIsEmpty(docDEPTID.SelectedValue))
                {
                    Alert.Show("请先选择查询科室！", "消息提示", MessageBoxIcon.Warning);
                    return;
                }
                strSQL = string.Format("SELECT  F_GETYSTOTAL('{0}','{1}','{2}') PRENUM, 0 EXENUM FROM DUAL", docDEPTID.SelectedValue, Convert.ToDateTime(docXSRQ.SelectedDate).AddMonths(-1).ToString("yyyy-MM") + "-01", Convert.ToDateTime(docXSRQ.SelectedDate).ToString("yyyy-MM") + "-01", Convert.ToDateTime(docXSRQ.SelectedDate).ToString("yyyy-MM-dd"));
            }
            DataTable dt = DbHelperOra.QueryForTable(strSQL);
            if (dt != null && dt.Rows.Count > 0)
            {
                docBuget.Text = "预算金额：" + dt.Rows[0]["PRENUM"].ToString() + " 元  已使用金额：" + dt.Rows[0]["EXENUM"].ToString() + "元 ";

            }
            else
            {
                docBuget.Text = "预算金额：0  元    已使用金额：0  元 ";

            }



        }
        protected void btnAuto_Click(object sender, EventArgs e)
        {
            WinAuto.Hidden = false;
            if (dbpOrder1.Text.Length < 1 && dbpOrder2.Text.Length < 1)
            {
                dbpOrder1.SelectedDate = DateTime.Now.AddMonths(-1);
                dbpOrder2.SelectedDate = DateTime.Now;
            }
        }
        protected void rblTYPE_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rblTYPE.SelectedValue == "XS")
            {
                dbpOrder1.Enabled = true;
                dbpOrder2.Enabled = true;
                memo.Text = "申请量 = 销售期间的销售量 - 科室库存 - 在途库存";
                if (dbpOrder1.Text.Length < 1 && dbpOrder2.Text.Length < 1)
                {
                    dbpOrder1.SelectedDate = DateTime.Now.AddMonths(-1);
                    dbpOrder2.SelectedDate = DateTime.Now;
                }
            }
            else
            {
                dbpOrder1.Enabled = false;
                dbpOrder2.Enabled = false;
                memo.Text = "申请量 = 最高库存 - 库房库存 - 在途库存";
            }
        }
        protected void btnSure_Click(object sender, EventArgs e)
        {
            //生成出库单
            if (ddlDeptOrder.SelectedValue.Length < 1)
            {
                Alert.Show("请选择【出库库房】！", MessageBoxIcon.Warning);
                return;
            }
            if (ddlDeptid.SelectedValue.Length < 1)
            {
                Alert.Show("请选择【入库科室】！", MessageBoxIcon.Warning);
                return;
            }
            if (rblTYPE.SelectedValue == "XS")
            {
                if (dbpOrder1.Text.Length < 1 || dbpOrder2.Text.Length < 1)
                {
                    Alert.Show("【销售日期】未维护完全，请检查！", MessageBoxIcon.Warning);
                    return;
                }
            }
            OracleParameter[] parameters = new OracleParameter[]
                            {
                                     new OracleParameter("VIN_DEPTOUT",OracleDbType.Varchar2,20),
                                     new OracleParameter("VIN_DEPTID",OracleDbType.Varchar2,400),
                                     new OracleParameter("VIN_TYPE",OracleDbType.Varchar2,2),
                                     new OracleParameter("VIN_PAGE1",OracleDbType.Varchar2,3),
                                     new OracleParameter("VIN_PAGE2",OracleDbType.Varchar2,1),
                                     new OracleParameter("VIN_OPERUSER",OracleDbType.Varchar2,20),
                                     new OracleParameter("VIN_TIME1",OracleDbType.Varchar2,10),
                                     new OracleParameter("VIN_TIME2",OracleDbType.Varchar2,10),
                                     new OracleParameter("VO_BILLNO",OracleDbType.Varchar2,20)
                            };
            parameters[0].Value = ddlDeptOrder.SelectedValue;
            parameters[1].Value = ddlDeptid.SelectedValue;
            parameters[2].Value = rblTYPE.SelectedValue;
            parameters[3].Value = rblBILTYPE.SelectedValue;
            parameters[4].Value = ddlCATID.SelectedValue;
            parameters[5].Value = UserAction.UserID;
            parameters[6].Value = dbpOrder1.Text;
            parameters[7].Value = dbpOrder2.Text;
            parameters[0].Direction = ParameterDirection.Input;
            parameters[1].Direction = ParameterDirection.Input;
            parameters[2].Direction = ParameterDirection.Input;
            parameters[3].Direction = ParameterDirection.Input;
            parameters[4].Direction = ParameterDirection.Input;
            parameters[5].Direction = ParameterDirection.Input;
            parameters[6].Direction = ParameterDirection.Input;
            parameters[7].Direction = ParameterDirection.Input;
            parameters[8].Direction = ParameterDirection.Output;
            DbHelperOra.RunProcedure("STOREDS.P_AUTOSL", parameters);
            if (parameters[8].Value.ToString() != "#" && parameters[8].Value.ToString().Length > 0)
            {
                Alert.Show("自动生成成功！");
                billOpen(parameters[8].Value.ToString());
                WinAuto.Hidden = true;
            }
            else
            {
                Alert.Show("科室[" + ddlDeptid.SelectedText + "]无需要申领的信息！", MessageBoxIcon.Warning);
            }
        }

        protected void btn_close_Click(object sender, EventArgs e)
        {
            WinAuto.Hidden = true;
        }

        protected void ddlDeptid_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OPER())
            {
                rblBILTYPE.Enabled = true;
            }
            else
            {

                rblBILTYPE.Enabled = false;
                rblBILTYPE.SelectedIndex = 0;
            }
        }

       
        protected void btnAllReConfirm_Click(object sender, EventArgs e)//批量收货确认
        {
            if (GridList.SelectedRowIDArray.Length < 1) return;
            List<CommandInfo> lstcmd = new List<CommandInfo>();

            foreach (int rowindex in GridList.SelectedRowIndexArray)
            {
                if (GridList.Rows[rowindex].Values[3].ToString() != "Y")
                {
                    Alert.Show("单据【"+GridList.Rows[rowindex].Values[2].ToString()+"】状态不正确，不能进行收货确认操作！");
                    return;
                }
                if (GridList.Rows[rowindex].Values[18].ToString() == "Y")
                {
                    Alert.Show("单据【" + GridList.Rows[rowindex].Values[2].ToString() + "】已收货确认，无需重复操作！");
                    return;
                }
                lstcmd.Add(new CommandInfo(string.Format("UPDATE DAT_SL_DOC SET ISSH='Y',SHOPERUSER='{0}',SHDATE=SYSDATE WHERE SEQNO='{1}' AND FLAG='Y' AND NVL(ISSH,'N')='N'",UserAction.UserID,GridList.Rows[rowindex].Values[2].ToString()),null));
            }
            if (DbHelperOra.ExecuteSqlTran(lstcmd))
            {
                Alert.Show("批量收货确认成功！");
            }
            else
            {
                Alert.Show("批量收货确认操作失败！");
                return;
            }
            billSearch();
        }

        protected void btnReConfirm_Click(object sender, EventArgs e)//单据中收货确认
        {
            if (string.IsNullOrEmpty(docSEQNO.Text)) return ;
            if ((",Y").IndexOf(docFLAG.SelectedValue) < 0)
            {
                Alert.Show("单据【"+docSEQNO.Text+"】状态不正确，只有送货中的单据可以收货确认！");
                return;
            }
            //if (rblISSH.SelectedValue == "Y")
            //{
            //    Alert.Show("单据【" + docSEQNO.Text + "】已收货确认，无需重复操作！");
            //    return;
            //}
            List<CommandInfo> lstcmd = new List<CommandInfo>();
            lstcmd.Add(new CommandInfo(string.Format("UPDATE DAT_SL_DOC SET ISSH='Y',SHOPERUSER='{0}',SHDATE=SYSDATE WHERE SEQNO='{1}' AND FLAG='Y' AND NVL(ISSH,'N')='N'",UserAction.UserID,docSEQNO.Text),null));
            if (DbHelperOra.ExecuteSqlTran(lstcmd))
            {
                Alert.Show("单据【" + docSEQNO.Text + "】收货确认成功！");
                billOpen(docSEQNO.Text);
                return;
            }
        }
    }
}