﻿using FineUIPro;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using XTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using System.Text;
using XTBase.Utilities;

namespace ERPProject.ERPStorage
{
    public partial class GoodsOrder : BillBase
    {
        private string strDocSql = "SELECT doc.* FROM DAT_DD_DOC doc WHERE doc.SEQNO ='{0}'";
        private string strComSql = @"SELECT --A.*,
                                              A.SEQNO,A.ROWNO,A.DEPTID,A.GDSEQ,A.BARCODE,A.GDNAME,A.GDSPEC,A.CATID,A.BZHL,
                                              A.BZSL,A.DHS,(A.DHSL/A.BZHL)DHSL,A.JXTAX,A.HSJJ,A.BHSJJ,A.HSJE,A.BHSJE,A.LSJ,A.LSJE,
                                              A.ISGZ,A.ISLOT,A.PH,A.PHID,A.PZWH,A.SPZTSL,A.FIRSTTIME,A.LASTTIME,A.MEMO,A.NUM1,A.NUM2,A.NUM3,
                                               A.UNIT,F_GETUNITNAME(A.UNIT) UNITNAME,
                                               F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME,
                                               F_GETSUPNAME(A.SUPID) SUPNAME,
                                               F_GETUNITNAME(B.UNIT) UNITSMALLNAME
                                          FROM DAT_DD_COM A, DOC_GOODS B
                                         WHERE A.SEQNO = '{0}'
                                           AND A.GDSEQ = B.GDSEQ";
        protected string GoodOrder = "/grf/GoodsOrder.grf";
        public override Field[] LockControl
        {
            get { return new Field[] { docBILLNO, docCGY, docPSSID, docDEPTID, docXDRQ, docMEMO }; }
        }

        public GoodsOrder()
        {
            BillType = "DHD";
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
                    //获取oper的值
                    hfdOper.Text = Request.QueryString["oper"].ToString();

                    if (Request.QueryString["oper"].ToString() == "input")
                    {
                        ButtonHidden(btnAudit, btnAuditBatch, btnAddRow, btnCancel, btnBatchPrint);
                        WebLine3.Hidden = true;
                        PubFunc.DdlDataGet("DDL_BILL_STATUSDHD", lstFLAG);
                    }
                    else if (Request.QueryString["oper"].ToString() == "audit")
                    {
                        PubFunc.DdlDataGet("DDL_BILL_STATUSFCH", lstFLAG);
                        billLockDoc(true);
                        ButtonHidden(btn_Auto, btnNew, btnCopy, btnAddRow, btnDelRow, btnSave, btnGoods, btnCommit, btnAllCommit, btnDel);
                        ListLine2.Hidden = true;
                        WebLine1.Hidden = true;
                        WebLine2.Hidden = true;
                        WebLine3.Hidden = true;

                        TabStrip1.ActiveTabIndex = 0;
                        if (Request.QueryString["pid"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["pid"].ToString()))
                        {
                            lstBILLNO.Text = Request.QueryString["pid"].ToString();
                            string date = "20" + lstBILLNO.Text.Substring(3, 2) + "-" + lstBILLNO.Text.Substring(5, 2) + "-" + lstBILLNO.Text.Substring(7, 2);
                            lstLRRQ1.SelectedDate = DateTime.Parse(date).AddDays(-1);
                            billOpen(lstBILLNO.Text);
                        }
                        else
                        {
                            billSearch();
                        }
                    }
                }
                //代管商品屏蔽自动订货按钮 c 20150507
                if (isDg())
                {
                    btn_Auto.Hidden = true;
                }
                else
                {
                    //隐藏到货时间列
                    GridColumn NUM2 = GridGoods.FindColumn("NUM2");
                    NUM2.Hidden = true;
                }
                hfdCurrent.Text = UserAction.UserID;
            }
        }

        private Boolean isDg()
        {
            if (Request.QueryString["dg"] != null && Request.QueryString["dg"].ToString() == "1")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void bindTreeDept(params FineUIPro.DropDownList[] ddls)
        {
            DataTable dt = new DataTable();
            dt = DbHelperOra.Query(@"select CODE, NAME, TreeLevel, islast
                                      from ((SELECT '--请选择--' NAME, '' CODE, 1 TreeLevel, 1 islast FROM dual)
                                            union all (
                                            SELECT '[' || CODE || ']' || NAME NAME,
                                                              CODE,
                                                              CLASS TreeLevel,
                                                              decode(islast, 'Y', 1, 0) isLast
                                                         FROM SYS_DEPT)
                                    )
                                    order by nvl(code,'0')").Tables[0];
            //return dt;


            List<CategoryTreeBean> myList = new List<CategoryTreeBean>();
            foreach (DataRow dr in dt.Rows)
            {
                myList.Add(new CategoryTreeBean(dr["code"].ToString(), dr["name"].ToString(), Convert.ToInt16(dr["TreeLevel"]), Convert.ToInt16(dr["islast"]) == 1));
            }
            // 绑定到下拉列表（启用模拟树功能）
            foreach (FineUIPro.DropDownList ddl in ddls)
            {
                ddl.EnableSimulateTree = true;
                ddl.DataTextField = "Name";
                ddl.DataValueField = "Id";
                ddl.DataEnableSelectField = "EnableSelect";
                ddl.DataSimulateTreeLevelField = "Level";
                ddl.DataSource = myList;
                ddl.DataBind();
            }
        }

        private void DataInit()
        {
            USERID.Text = UserAction.UserID;
            //代管需要绑定代管供应商，其他绑定非代管供应商
            if (!isDg())
            {
                PubFunc.DdlDataGet("DDL_DOC_SUPPLIER", docPSSID, lstPSSID);
            }
            else
            {
                //TODO 增加sys_report 代管供应商
                string sql = @"select  CODE,NAME from (
                                SELECT '--请选择--' NAME,'' CODE   FROM dual
                                union all
                                SELECT SUPNAME NAME,SUPID CODE FROM DOC_SUPPLIER where STR1 = 'N' AND 
                                SUPID IN (SELECT DISTINCT NVL(PSSID,SUPID) FROM DOC_GOODSSUP WHERE TYPE = '1')
                                )
                                ORDER BY DECODE(CODE,'',99,0) DESC ,CODE ASC ";
                PubFunc.DdlDataSql(docPSSID, sql);
                PubFunc.DdlDataSql(lstPSSID, sql);
                //PubFunc.DdlDataGet("DDL_DOC_SUPPLIER_DG", docPSSID, lstPSSID);
                ButtonHidden(btn_Auto);
            }
            docPSSID.SelectedIndex = 1;
            PubFunc.DdlDataGet(ddlReject, "DDL_BILL_REASON");
            PubFunc.DdlDataGet("DDL_SYS_DEPOTRANGE_ORDER", UserAction.UserID, docDEPTID, lstDEPTID, ddlDeptOrder);
            PubFunc.DdlDataGet("DDL_GOODSTYPE", docDHLX);
            PubFunc.DdlDataGet("DDL_BILL_STATUSDHD", docFLAG, lstFLAG);
            PubFunc.DdlDataGet("DDL_USER", lstLRY, lstCGY, docLRY, docCGY, docSHR);
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
            docDEPTID.SelectedValue = Doc.DbGetSysPara("DEFDEPT");
            if (Request.QueryString["tp"] != null)
            {
                docDHLX.SelectedValue = Request.QueryString["tp"].ToString();
            }
            else
            {
                docDHLX.SelectedValue = "2";
            }

            //获取客户化GRF文件地址  By c 2016年1月21日12:18:29 At 威海509
            string grf = Doc.DbGetGrf("GoodOrder");
            if (!string.IsNullOrWhiteSpace(grf))
            {
                GoodOrder = grf;
            }
        }

        protected override void billNew()
        {
            USERID.Text = UserAction.UserID;
            string strSup = docPSSID.SelectedValue;
            string strDept = docDEPTID.SelectedValue;
            //原单据保存判断
            PubFunc.FormDataClear(FormDoc);
            if (PubFunc.StrIsEmpty(strSup))
            {
                strSup = Doc.DbGetSysPara("SUPPLIER");
            }
            if (PubFunc.StrIsEmpty(strDept))
            {
                if (docDEPTID.Items.Count > 2 && !isDg())
                    strDept = docDEPTID.Items[1].Value;
            }
            docFLAG.SelectedValue = "M";
            docCGY.SelectedValue = UserAction.UserID;
            docLRY.SelectedValue = UserAction.UserID;
            docLRRQ.SelectedDate = DateTime.Now;
            docXDRQ.SelectedDate = DateTime.Now;

            string strDHRQ = PubFunc.DbGetPara("DHRQ");
            if (!string.IsNullOrWhiteSpace(strDHRQ) && PubFunc.isNumeric(strDHRQ))
            {
                int intDHRQ = int.Parse(PubFunc.DbGetPara("DHRQ"));
                dpkDHRQ.SelectedDate = DateTime.Now.AddDays(intDHRQ);
            }
            docDEPTID.SelectedValue = strDept;
            docPSSID.SelectedValue = strSup;
            nbxBZSL.Enabled = true;
            comMEMO.Enabled = true;
            billLockDoc(false);
            docFLAG.Enabled = false;
            dpkDHRQ.Enabled = true;
            btnGoods.Enabled = true;
            cbxISYX.Enabled = true;
            docDHLX.Enabled = true;
            GridGoods.SummaryData = null;
            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());

            btnDel.Enabled = false;
            btnSave.Enabled = true;
            btnCommit.Enabled = false;
            btnAudit.Enabled = false;
            btnCancel.Enabled = false;
            btnPrint.Enabled = false;
            btnDelRow.Enabled = true;
            btnGoods.Enabled = true;
            btn_Auto.Enabled = true;
            if (Request.QueryString["tp"] != null)
            {
                docDHLX.SelectedValue = Request.QueryString["tp"].ToString();
            }
            else
            {
                docDHLX.SelectedValue = "2";
            }
        }

        protected void GridGoods_AfterEdit(object sender, FineUIPro.GridAfterEditEventArgs e)
        {
            if (e.ColumnID == "BZSL" || e.ColumnID == "HSJJ")
            {
                string[] strCell = GridGoods.SelectedCell;
                List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
                if (newDict.Count == 0) return;
                JObject defaultObj = Doc.GetJObject(GridGoods, e.RowID);

                if (!PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "BZHL")) || !PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "BZSL")) || !PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "HSJJ")))
                {
                    Alert.Show("商品信息异常，请详细检查商品信息：包装含量或价格！", "异常信息", MessageBoxIcon.Warning);
                    return;
                }

                //处理返回jobject
                object objISFLAG5 = DbHelperOra.GetSingle(string.Format("SELECT ISFLAG5 FROM DOC_GOODS WHERE GDSEQ = '{0}'", defaultObj["GDSEQ"]));

                if (objISFLAG5.ToString() == "N")
                {
                    if (Convert.ToDecimal(defaultObj["BZSL"]) != (int)Convert.ToDecimal(defaultObj["BZSL"]) && Convert.ToDecimal(defaultObj["BZHL"] ?? "0") == 1)
                    {
                        Alert.Show("当前商品不支持申领数为小数，请调整", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                }

                decimal hl = 0, rs = 0, jg = 0;
                decimal.TryParse((defaultObj["BZHL"] ?? "0").ToString(), out hl);
                decimal.TryParse((defaultObj["BZSL"] ?? "0").ToString(), out rs);
                decimal.TryParse((defaultObj["HSJJ"] ?? "0").ToString(), out jg);
                defaultObj["DHS"] = rs * hl;
                defaultObj["HSJE"] = Math.Round(rs * jg, 2).ToString("F2");
                PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(e.RowID, defaultObj));

                #region //计算合计数量
                decimal bzslTotal = 0, feeTotal = 0;
                foreach (Dictionary<string, object> dic in newDict)
                {
                    bzslTotal += Convert.ToDecimal(dic["BZSL"] ?? "0");
                    feeTotal += (Convert.ToDecimal(dic["HSJJ"] ?? "0") * Convert.ToDecimal((dic["BZSL"] ?? "0")));
                }
                JObject summary = new JObject();
                summary.Add("GDNAME", "本页合计");
                summary.Add("BZSL", bzslTotal.ToString("F2"));
                summary.Add("HSJE", feeTotal.ToString("F2"));

                GridGoods.SummaryData = summary;
                //后调函数验证数据库
                hdfBH.Text = "1";
                #endregion
            }
        }

        protected override void billClear()
        {
            PubFunc.FormDataClear(Formlist);
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;

            //还原按钮显示状态

        }
        private void buttonClear(string strBillno)
        {//还原按钮显示状态
            bntClear.Enabled = true;
            btnAllCommit.Enabled = true;
            btnAuditBatch.Enabled = true;
            bntSearch.Enabled = true;

            btn_Auto.Enabled = true;
            btnNew.Enabled = true;
            btnDel.Enabled = true;
            btnSave.Enabled = true;
            btnCommit.Enabled = true;
            btnAudit.Enabled = true;
            btnCancel.Enabled = true;
            btnScan.Enabled = true;
            btnPrint.Enabled = true;
            btnCopy.Enabled = true;
            btnExport.Enabled = true;
            btnAddRow.Enabled = true;
            btnDelRow.Enabled = true;
            btnNext.Enabled = true;
            btnBef.Enabled = true;
            btnGoods.Enabled = true;

            if (DbHelperOra.Exists("select 1 from DAT_DD_DOC where SEQNO = '" + strBillno + "' AND (FLAG='N' or FLAG='M')"))
            {
                btnPrint.Enabled = false;
            }
            if (DbHelperOra.Exists("select 1 from DAT_DD_DOC where SEQNO = '" + strBillno + "' AND FLAG='Y'"))
            {
                btnDel.Enabled = false;
                btnSave.Enabled = false;
                btnCommit.Enabled = false;
                btnDelRow.Enabled = false;
                btnGoods.Enabled = false;
            }
        }

        protected override void billAddRow()
        {
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
            billLockDoc(true);
            docMEMO.Enabled = true;
            docDHLX.Enabled = false;
            PubFunc.GridRowAdd(GridGoods, "INIT");
        }
        private void billDelRow_mes()
        {
            string[] strCell = GridGoods.SelectedCell;
            GridGoods.DeleteRow(strCell[0]);
        }
        protected override void billCancel()
        {
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("只有已提交的单据才能驳回！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string strBillno = docSEQNO.Text;
            if (!Doc.getFlag(strBillno, "N", BillType))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！");
                return;
            }
            WindowReject.Hidden = false;
        }
        protected void btnRejectSubmit_Click(object sender, EventArgs e)
        {
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("已提交的单据不能驳回！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            //增加待办事宜
            List<CommandInfo> cmdList = new List<CommandInfo>();
            cmdList.Add(new CommandInfo("UPDATE DAT_DO_LIST SET FLAG = 'Y' WHERE PARA='" + docBILLNO.Text.Trim() + "'", null));
            if (docBILLNO.Text.Length < 1)
            { return; }

            if (string.IsNullOrWhiteSpace(ddlReject.SelectedValue))
            {
                Alert.Show("请选择驳回原因", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string strMemo = "驳回原因：" + ddlReject.SelectedText;
            if (!string.IsNullOrWhiteSpace(txaMemo.Text.Trim()))
            {
                strMemo += "；详细说明：" + txaMemo.Text;
            }
            if (strMemo.Length > 40)
            {
                Alert.Show("驳回备注超长！");
                return;

            }
            if (DbHelperOra.ExecuteSql(string.Format("update DAT_DD_DOC set flag='R',memo='{0}' where seqno='{1}' and flag='N'", strMemo, docBILLNO.Text)) == 1)
            {
                DbHelperOra.ExecuteSqlTran(cmdList);
                WindowReject.Hidden = true;
                billOpen(docBILLNO.Text);
                Alert.Show("单据驳回成功！");
                OperLog("商品订货", "驳回单据【" + docBILLNO.Text + "】");
            };
        }
        protected override void billDelRow()
        {
            if (docFLAG.SelectedValue != "M" && docFLAG.SelectedValue != "R")
            {
                Alert.Show("非新增单据不能删除！");
                return;
            }
            if (!CheckFlag(docFLAG.SelectedValue))
            {
                Alert.Show("此单据已经被别人操作，请等待操作!");
                return;
            }
            if (GridGoods.SelectedRowID == null)
            {
                Alert.Show("未选中任何行，无法进行【删行】操作!");
                return;
            }
            GridGoods.DeleteSelectedRows();
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
            UpdateSum(newDict);
        }

        protected override void billGoods()
        {
            //ymh取消后台调用商品界面
            if (docFLAG.SelectedValue != "M" && docFLAG.SelectedValue != "R")
            {
                Alert.Show("非新单不能增加商品");
                return;
            }
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
            billLockDoc(true);
            docMEMO.Enabled = true;
            docDHLX.Enabled = false;
            string url;
            if (isDg())
            {
                //供应商不决定商品
                url = "~/ERPQuery/GoodsWindow_New.aspx?Deptout=" + docDEPTID.SelectedValue + "&Shs=" + docPSSID.SelectedValue + "&goodsType=" + docDHLX.SelectedValue + "&isdg=Y&isbd=N&GoodsState=Y";
            }
            else
            {
                url = "~/ERPQuery/GoodsWindow_New.aspx?Deptout=" + docDEPTID.SelectedValue + "&Shs=" + docPSSID.SelectedValue + "&goodsType=" + docDHLX.SelectedValue + "&isdg=N&isbd=N&GoodsState=Y";
            }
            PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "商品信息查询"));
        }

        protected override void billSearch()
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【订货日期】！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                Alert.Show("【开始日期】大于【结束日期】，请重新输入！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            string strSql = @"SELECT A.SEQNO,A.BILLNO,A.FLAG,B.NAME FLAG_CN,F_GETDEPTNAME(A.DEPTID) DEPTID,A.XDRQ,F_GETSUPNAME(A.PSSID) SUPNAME,A.SUBSUM,
                                     A.SUBNUM,F_GETUSERNAME(A.CGY) CGY,F_GETUSERNAME(A.LRY) LRY,A.LRRQ,F_GETUSERNAME(A.SHR) SHR,A.SHRQ,A.MEMO,
                                     DECODE(A.ISSEND,'N','等待传输','Y','平台处理中','E','平台处理错误','S','平台处理完成','入库完成') ISSENDNAME,NVL((SELECT FUNCTIME FROM SYS_FUNCPRNNUM WHERE FUNCNO = a.SEQNO),0) PRINTNUM
                                from DAT_DD_DOC A, SYS_CODEVALUE B 
                                WHERE A.FLAG = B.CODE AND B.TYPE  ='DIC_BILLSTATUS' 
                                AND A.DEPTID in(select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '" + UserAction.UserID + "') = 'Y' ) ";
            string strSearch = "";

            if (lstBILLNO.Text.Trim().Length > 0)
            {
                strSearch += string.Format(" AND A.BILLNO  LIKE '%{0}%'", lstBILLNO.Text.Trim());
            }
            if (lstFLAG.SelectedItem != null && lstFLAG.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.FLAG='{0}'", lstFLAG.SelectedItem.Value);
            }
            if (lstCGY.SelectedItem != null && lstCGY.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.CGY='{0}'", lstCGY.SelectedItem.Value);
            }
            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedItem.Value);
            }
            if (lstLRY.SelectedItem != null && lstLRY.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.LRY='{0}'", lstLRY.SelectedItem.Value);
            }
            if (lstPSSID.SelectedItem != null && lstPSSID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.PSSID='{0}'", lstPSSID.SelectedItem.Value);
            }
            //过滤新单的单据，不能审核，提交后的才能审核
            if (Request.QueryString["oper"].ToString() == "audit")
            {
                strSearch += " AND A.FLAG<>'M'";
            }
            //代管筛选条件修改
            if (isDg())
            {
                //2-代管
                //系统默认00001表示医院，并且为代管模式（主要是针对医院初始库存导入的情况） By c 20160116
                strSearch += " AND (PSSID = '00001' OR PSSID IN (SELECT DISTINCT NVL(A.PSSID, A.SUPID) PSSID FROM DOC_GOODSSUP A WHERE A.TYPE ='1')) AND A.DHFS='2' ";
            }
            else
            {
                //1-托管，3-代管转正常
                strSearch += " AND  PSSID IN (SELECT DISTINCT NVL(A.PSSID, A.SUPID) PSSID FROM DOC_GOODSSUP A WHERE A.TYPE IN ('0','Z'))";
            }

            strSearch += string.Format(" AND A.LRRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.LRRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY A.LRRQ DESC,A.BILLNO DESC";
            highlightRows.Text = "";
            highlightRowYellow.Text = "";
            highlightRowRed.Text = "";
            GridList.DataSource = DbHelperOra.Query(strSql).Tables[0];
            GridList.DataBind();
        }
        
        protected override void billAudit()
        {
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("非提交单据不能审核！");
                return;
            }
            string strBillno = docSEQNO.Text;
            //DataTable dtDoc = DbHelperOra.Query(string.Format(strDocSql, strBillno)).Tables[0];
            //string supid = dtDoc.Rows[0]["PSSID"].ToString();

            //DataTable ddComDT = DbHelperOra.Query("SELECT GDSEQ FROM DAT_DD_COM WHERE SEQNO='" + strBillno + "'").Tables[0];
            //if (ddComDT.Rows.Count > 0)
            //{
            //string lisResult = "验证通过";
            //for (int i = 0; i < ddComDT.Rows.Count; i++)
            //{
            //    lisResult = lisResult + DbHelperOra.GetSingle("select F_LICENSE_ISEFFECTIVE('" + supid + "','" + ddComDT.Rows[i]["GDSEQ"].ToString() + "') from dual").ToString() + ",";
            //}

            //lisResult = lisResult.Substring(0, lisResult.Length - 1).Split(',')[0];
            //if (lisResult.Equals("验证通过"))
            //{
            if (!Doc.getFlag(strBillno, "N", BillType))
            {
                Alert.Show("此单据已经被别人操作，不能审核！");
                return;
            }

            //审核的时候允许修改订单信息
            //DataSave(false);
            
            if (BillOper(strBillno, "AUDIT") == 1)
            {
                billLockDoc(true);
                Alert.Show("单据【" + strBillno + "】审核成功！");
                billOpen(strBillno);
                OperLog("商品订货", "审核单据【" + docBILLNO.Text + "】");
            }
            //}
            //else
            //{
            //    Alert.Show(lisResult);
            //}
            //}
            //else
            //{
            //    Alert.Show("此单据审核失败，请联系系统管理人员！");
            //}
        }

        protected override void listRow_DoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            string strBillno = GridList.Rows[e.RowIndex].Values[1].ToString();
            billClear();
            buttonClear(strBillno);
            billOpen(strBillno);

            if ((",M,R").IndexOf(docFLAG.SelectedValue) > 0)
            {
                PubFunc.FormLock(FormDoc, true);
                comMEMO.Enabled = true;
                nbxBZSL.Enabled = true;
                dpkDHRQ.Enabled = true;
                docMEMO.Enabled = true;
                docDHLX.Enabled = false;
                cbxISYX.Enabled = true;
                //docDHLX.Enabled = true;
            }
            else
            {
                PubFunc.FormLock(FormDoc, true);
                nbxBZSL.Enabled = false;
                comMEMO.Enabled = false;
            }
        }

        protected override void billOpen(string strBillno)
        {
            DataTable dtDoc = DbHelperOra.Query(string.Format(strDocSql, strBillno)).Tables[0];
            if (dtDoc != null && dtDoc.Rows.Count > 0)
            {
                PubFunc.FormDataSet(FormDoc, dtDoc.Rows[0]);
                page(strBillno, 0);
            }
            //增加按钮控制
            if (docFLAG.SelectedValue == "M" || docFLAG.SelectedValue == "R")
            {
                btnDel.Enabled = true;
                btnSave.Enabled = true;
                btnCommit.Enabled = true;
                btnAudit.Enabled = false;
                btnCancel.Enabled = false;
                btnPrint.Enabled = false;
                btnDelRow.Enabled = true;
                btnGoods.Enabled = true;
            }
            else if (docFLAG.SelectedValue == "N")
            {
                btnDel.Enabled = false;
                btnSave.Enabled = false;
                btnCommit.Enabled = false;
                btnAudit.Enabled = true;
                btnCancel.Enabled = true;
                btnPrint.Enabled = false;
                btnDelRow.Enabled = false;
                btnGoods.Enabled = false;
            }
            else
            {
                btnDel.Enabled = false;
                btnSave.Enabled = false;
                btnCommit.Enabled = false;
                btnAudit.Enabled = false;
                btnCancel.Enabled = false;
                btnPrint.Enabled = true;
                btnDelRow.Enabled = false;
                btnGoods.Enabled = false;
            }
            btn_Auto.Enabled = false;
            hfdBills.Text = docBILLNO.Text;
        }
        protected void page(string billNo, int PageIndex)
        {
            if (PageIndex > 0)
            {
                GridGoods.PageIndex = Convert.ToInt32(hdftest.Text);
            }
            string strWhere = "";
            if (tgxGoods.Text.Trim().Length > 0)
            {
                strWhere = string.Format(" AND (B.GDSEQ LIKE '%{0}%' OR B.GDNAME LIKE '%{0}%' OR B.ZJM LIKE '%{0}%' OR B.BARCODE LIKE '%{0}%' OR B.GDSEQ LIKE '%{1}%' OR B.GDNAME LIKE '%{1}%' OR B.ZJM LIKE '%{1}%' OR B.BARCODE LIKE '%{1}%')", tgxGoods.Text.Trim().ToUpper(), tgxGoods.Text.Trim().ToLower());
                hdfGL.Text = tgxGoods.Text.Trim();
            }
            else
            {
                hdfGL.Text = "";
            }
            int total = 0;
            DataTable dtBill = DbHelperOra.Query(string.Format(strComSql + strWhere + " order by rowno asc", billNo)).Tables[0];

            decimal ddslTotal = 0, bzslTotal = 0, feeTotal = 0;
            foreach (DataRow row in dtBill.Rows)
            {
                ddslTotal += Convert.ToDecimal(string.IsNullOrWhiteSpace(row["DHSL"].ToString()) ? "0" : row["DHSL"].ToString());
                bzslTotal += Convert.ToDecimal(string.IsNullOrWhiteSpace(row["BZSL"].ToString()) ? "0" : row["BZSL"].ToString());
                feeTotal += (Convert.ToDecimal(row["HSJJ"]) * Convert.ToDecimal(row["BZSL"] ?? "0"));
            }
            GridGoods.RecordCount = total;
            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            //PubFunc.GridRowAdd(GridGoods, dtBill);
            Doc.GridRowAdd(GridGoods, dtBill);

            //计算合计数量
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("BZSL", bzslTotal.ToString("F2"));
            summary.Add("HSJE", feeTotal.ToString("F2"));
            GridGoods.SummaryData = summary;
            hdfBH.Text = "0";
            PubFunc.FormLock(FormDoc, true, "");
            if ((",M,R").IndexOf(docFLAG.SelectedValue) > 0) { docMEMO.Enabled = true; }
            TabStrip1.ActiveTabIndex = 1;
        }
        protected override void billSave()
        {
            DataSave(true);
        }
        /// <summary>
        /// 该功能是适应订货页面分页时使用，前台页面已经改为不分页，故该功能作废改用save() By c
        /// </summary>
        /// <param name="flag"></param>
        private void pagesave(bool flag)
        {
            #region 数据有效性验证
            if (docFLAG.SelectedValue != "M" && docFLAG.SelectedValue != "R")
            {
                Alert.Show("非新单不能保存！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!CheckFlag(docFLAG.SelectedValue))
            {
                Alert.Show("此单据已经被别人操作，请等待操作!");
                return;
            }

            if (PubFunc.FormDataCheck(FormDoc).Length > 0) return;
            string GoodsFlag = "Y";
            List<Dictionary<string, object>> newDict = new List<Dictionary<string, object>>();

            if (DbHelperOra.Exists("SELECT 1 FROM SYS_PARA WHERE CODE = 'GOODSMODE' AND VALUE = 'N'"))
            { GoodsFlag = "M"; }
            if (GoodsFlag == "Y")
            {
                //不允许排序变动 王阿磊 2015年9月29日 14:48:44
                //newDict = GridGoods.GetNewAddedList().OrderBy(x => x["GDSEQ"]).ToList();
                newDict = GridGoods.GetNewAddedList().ToList();
            }
            else
            { newDict = GridGoods.GetNewAddedList(); }
            if (newDict.Count == 0)
            {
                Alert.Show("请输入商品信息", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string gdseq = "";
            List<Dictionary<string, object>> goodsData = new List<Dictionary<string, object>>();
            //判断是否有空行、批号填写是否符合要求
            for (int i = 0; i < newDict.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(newDict[i]["GDSEQ"].ToString()) && !string.IsNullOrWhiteSpace(newDict[i]["GDNAME"].ToString()))
                {

                    //if (newDict[i]["BZSL"] ==null )
                    //{
                    //    Alert.Show("请填写商品[" + newDict[i]["GDSEQ"] + "]订货数量！", "消息提示", MessageBoxIcon.Warning);
                    //    return;
                    //}
                    if ((newDict[i]["BZSL"] ?? "").ToString() == "" || (newDict[i]["BZSL"] ?? "").ToString() == "0")
                    {
                        Alert.Show("请填写商品[" + newDict[i]["GDSEQ"] + "]订货数量！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    //if ((newDict[i]["HSJJ"] ?? "").ToString() == "" || (newDict[i]["HSJJ"] ?? "").ToString() == "0" || Convert.ToDecimal(newDict[i]["HSJJ"]) == 0)
                    //{
                    //    Alert.Show("商品[" + newDict[i]["GDSEQ"] + "]的【含税进价】不能为零,请维护好商品的含税进价，再次保存！", "消息提示", MessageBoxIcon.Warning);
                    //    return;
                    //}
                    if (string.IsNullOrWhiteSpace(newDict[i]["BZHL"].ToString()) || string.IsNullOrWhiteSpace(newDict[i]["UNIT"].ToString()))
                    {
                        Alert.Show("商品[" + newDict[i]["GDSEQ"] + "]包装单位信息错误，请联系管理员维护！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    //重新计算订货数/金额
                    newDict[i]["DHS"] = decimal.Parse(newDict[i]["BZSL"].ToString()) * decimal.Parse(newDict[i]["BZHL"].ToString());
                    newDict[i]["HSJE"] = decimal.Parse(newDict[i]["HSJJ"].ToString()) * decimal.Parse(newDict[i]["BZSL"].ToString());
                    if (gdseq != newDict[i]["GDSEQ"].ToString() || GoodsFlag == "M")
                    {
                        gdseq = newDict[i]["GDSEQ"].ToString();
                        goodsData.Add(newDict[i]);
                    }
                    else
                    {
                        if (goodsData[goodsData.Count - 1]["GDSEQ"].ToString() == gdseq)
                        {
                            goodsData[goodsData.Count - 1]["BZSL"] = decimal.Parse(goodsData[goodsData.Count - 1]["BZSL"].ToString()) + decimal.Parse(newDict[i]["BZSL"] == null ? "0" : newDict[i]["BZSL"].ToString());
                            //取消计算到货数
                            //goodsData[goodsData.Count - 1]["DHSL"] = decimal.Parse(goodsData[goodsData.Count - 1]["DHSL"].ToString()) + decimal.Parse(newDict[i]["DHSL"].ToString());
                            goodsData[goodsData.Count - 1]["HSJE"] = decimal.Parse(goodsData[goodsData.Count - 1]["HSJE"].ToString()) + decimal.Parse(newDict[i]["HSJE"] == null ? "0" : newDict[i]["HSJE"].ToString());
                            //由于界面问题,如果未计算订货数，保存时自动计算
                            goodsData[goodsData.Count - 1]["DHS"] = decimal.Parse(goodsData[goodsData.Count - 1]["DHS"].ToString()) + decimal.Parse(newDict[i]["DHS"].ToString());
                        }
                    }
                }
            }

            #endregion

            if (PubFunc.StrIsEmpty(docBILLNO.Text))
            {
                docSEQNO.Text = BillSeqGet();
                docBILLNO.Text = docSEQNO.Text;
                docBILLNO.Enabled = false;
            }
            else
            {
                string flg = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M') FROM DAT_DD_DOC WHERE SEQNO='{0}'", docBILLNO.Text));
                if (!PubFunc.StrIsEmpty(flg) && (",M,R").IndexOf(flg) < 0)
                {
                    Alert.Show("您输入的单据号存在重复信息，请重新输入或置空！", "消息提示", MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    docSEQNO.Text = docBILLNO.Text;
                    docBILLNO.Enabled = false;
                }
            }
            MyTable mtType = new MyTable("DAT_DD_DOC");
            mtType.ColRow = PubFunc.FormDataHT(FormDoc);
            mtType.ColRow["SEQNO"] = docBILLNO.Text;
            mtType.ColRow["FLAG"] = "M";
            mtType.ColRow.Add("BILLTYPE", BillType);
            mtType.ColRow.Add("DEPTDH", mtType.ColRow["DEPTID"]);

            if (isDg())
            {
                mtType.ColRow["DHFS"] = "2";
            }
            if (Grid.PageInsert(docSEQNO.Text, hdfGL.Text, "DAT_DD_COM", GridGoods.PageIndex, GridGoods.PageSize, goodsData))
            {
                if (Grid.saveDoc(mtType, "DAT_DD_DOC", docSEQNO.Text))
                {
                    if (flag)
                    {
                        Alert.Show("商品订货信息保存成功！");
                        billOpen(docBILLNO.Text);
                        billSearch();
                        OperLog("商品订货", "修改单据【" + docBILLNO.Text + "】");
                    }
                }
                else
                {
                    Alert.Show("商品订货信息保存失败，请联系管理员检查原因！");
                }
            }
            else
            {
                Alert.Show("商品订货信息保存失败，请联系管理员检查原因！");
            }
        }
        private bool SaveSuccess = false;
        private void DataSave(bool flag)
        {
            #region 数据有效性验证
            if (flag)
            {
                if (docFLAG.SelectedValue != "M" && docFLAG.SelectedValue != "R")
                {
                    Alert.Show("非新单不能保存！", "消息提示", MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                if (docFLAG.SelectedValue != "N")
                {
                    Alert.Show("非已提交单据不能审核！", "消息提示", MessageBoxIcon.Warning);
                    return;
                }
            }
            if (!CheckFlag(docFLAG.SelectedValue))
            {
                Alert.Show("此单据已经被别人操作，请等待操作!");
                return;
            }

            if (PubFunc.FormDataCheck(FormDoc).Length > 0) return;
            string GoodsFlag = "Y";

            if (DbHelperOra.Exists("SELECT 1 FROM SYS_PARA WHERE CODE = 'GOODSMODE' AND VALUE = 'N'"))
            { GoodsFlag = "M"; }

            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().ToList();

            if (newDict.Count == 0)
            {
                Alert.Show("请输入商品信息", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string gdseq = "";
            List<Dictionary<string, object>> goodsData = new List<Dictionary<string, object>>();
            //判断是否有空行、批号填写是否符合要求
            for (int i = 0; i < newDict.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(newDict[i]["GDSEQ"].ToString()) && !string.IsNullOrWhiteSpace(newDict[i]["GDNAME"].ToString()))
                {
                    if ((newDict[i]["BZSL"] ?? "").ToString() == "" || (newDict[i]["BZSL"] ?? "").ToString() == "0")
                    {
                        Alert.Show("请填写商品[" + newDict[i]["GDSEQ"] + "]订货数量！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    //if ((newDict[i]["HSJJ"] ?? "").ToString() == "" || (newDict[i]["HSJJ"] ?? "").ToString() == "0" || Convert.ToDecimal(newDict[i]["HSJJ"]) == 0)
                    //{
                    //    Alert.Show("商品[" + newDict[i]["GDSEQ"] + "]的【含税进价】不能为零,请维护好商品的含税进价，再次保存！", "消息提示", MessageBoxIcon.Warning);
                    //    return;
                    //}
                    if (string.IsNullOrWhiteSpace(newDict[i]["BZHL"].ToString()) || string.IsNullOrWhiteSpace(newDict[i]["UNIT"].ToString()))
                    {
                        Alert.Show("商品[" + newDict[i]["GDSEQ"] + "]包装单位信息错误，请联系管理员维护！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    //重新计算订货数/金额
                    newDict[i]["DHS"] = decimal.Parse(newDict[i]["BZSL"].ToString()) * decimal.Parse(newDict[i]["BZHL"].ToString());
                    newDict[i]["HSJE"] = decimal.Parse(newDict[i]["HSJJ"].ToString()) * decimal.Parse(newDict[i]["BZSL"].ToString());
                    if (gdseq != newDict[i]["GDSEQ"].ToString() || GoodsFlag == "M")
                    {
                        gdseq = newDict[i]["GDSEQ"].ToString();
                        goodsData.Add(newDict[i]);
                    }
                    else
                    {
                        if (goodsData[goodsData.Count - 1]["GDSEQ"].ToString() == gdseq)
                        {
                            goodsData[goodsData.Count - 1]["BZSL"] = decimal.Parse(goodsData[goodsData.Count - 1]["BZSL"].ToString()) + decimal.Parse(newDict[i]["BZSL"] == null ? "0" : newDict[i]["BZSL"].ToString());
                            //取消计算到货数
                            //goodsData[goodsData.Count - 1]["DHSL"] = decimal.Parse(goodsData[goodsData.Count - 1]["DHSL"].ToString()) + decimal.Parse(newDict[i]["DHSL"].ToString());
                            goodsData[goodsData.Count - 1]["HSJE"] = decimal.Parse(goodsData[goodsData.Count - 1]["HSJE"].ToString()) + decimal.Parse(newDict[i]["HSJE"] == null ? "0" : newDict[i]["HSJE"].ToString());
                            //由于界面问题,如果未计算订货数，保存时自动计算
                            goodsData[goodsData.Count - 1]["DHS"] = decimal.Parse(goodsData[goodsData.Count - 1]["DHS"].ToString()) + decimal.Parse(newDict[i]["DHS"].ToString());
                        }
                    }
                }
            }
            if (goodsData.Count == 0)
            {
                Alert.Show("商品信息不能为空", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            #endregion

            if (flag)
            {
                if (PubFunc.StrIsEmpty(docBILLNO.Text))
                {
                    docSEQNO.Text = BillSeqGet();
                    docBILLNO.Text = docSEQNO.Text;
                    docBILLNO.Enabled = false;
                }
                else
                {
                    string flg = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M') FROM DAT_DD_DOC WHERE SEQNO='{0}'", docBILLNO.Text));
                    if (!PubFunc.StrIsEmpty(flg) && (",M,R").IndexOf(flg) < 0)
                    {
                        Alert.Show("您输入的单据号存在重复信息，请重新输入或置空！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    else
                    {
                        docSEQNO.Text = docBILLNO.Text;
                        docBILLNO.Enabled = false;
                    }
                }
            }
            MyTable mtType = new MyTable("DAT_DD_DOC");
            mtType.ColRow = PubFunc.FormDataHT(FormDoc);
            mtType.ColRow["SEQNO"] = docBILLNO.Text;
            if (flag)
            {
                mtType.ColRow["FLAG"] = "M";
            }
            mtType.ColRow.Add("BILLTYPE", BillType);
            mtType.ColRow.Add("DEPTDH", mtType.ColRow["DEPTID"]);
            mtType.ColRow.Add("PSSNAME", docPSSID.SelectedText);
            if (isDg())
            {
                mtType.ColRow["DHFS"] = "2";
            }

            mtType.ColRow.Add("SUBNUM", newDict.Count);
            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtTypeMx = new MyTable("DAT_DD_COM");
            decimal subNum = 0;//总金额
            //先删除单据信息在插入
            cmdList.Add(new CommandInfo("delete DAT_DD_DOC where seqno='" + docBILLNO.Text + "'", null));//删除单据台头
            cmdList.Add(new CommandInfo("delete DAT_DD_COM where seqno='" + docBILLNO.Text + "'", null));//删除单据明细
            //newDict = newDict.OrderBy(x => x["GDSEQ"]).ToList();//按照商品编码重新排序
            for (int i = 0; i < newDict.Count; i++)
            {
                mtTypeMx.ColRow = PubFunc.GridDataGet(newDict[i]);

                mtTypeMx.ColRow.Add("SEQNO", docBILLNO.Text);
                mtTypeMx.ColRow["ROWNO"] = i + 1;
                mtTypeMx.ColRow.Add("BHSJJ", 0);
                mtTypeMx.ColRow.Add("BHSJE", 0);
                mtTypeMx.ColRow["DHS"] = decimal.Parse(mtTypeMx.ColRow["BZHL"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                mtTypeMx.ColRow["HSJE"] = (decimal.Parse(mtTypeMx.ColRow["HSJJ"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString()));
                subNum = subNum + decimal.Parse(mtTypeMx.ColRow["HSJE"].ToString());
                cmdList.Add(mtTypeMx.Insert());
            }
            mtType.ColRow.Add("SUBSUM", subNum);
            cmdList.Add(mtType.Insert());
            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                if (flag)
                {
                    if (hfdSave.Text != "S")
                        Alert.Show("商品订货数据保存成功！");
                    OperLog("商品订货", "修改单据【" + docBILLNO.Text + "】");
                    billOpen(docBILLNO.Text);                    
                }
                SaveSuccess = true;
            }
            else
            {
                Alert.Show("商品订货数据保存失败！", "错误提示", MessageBoxIcon.Error);
            }
        }

        private DataTable LoadGridData(DataTable dt, ref string msg)
        {
            DataTable mydt = dt.Clone();
            foreach (DataRow row in dt.Rows)
            {
                //处理金额格式
                decimal jingdu = 0;
                if (decimal.TryParse(row["HSJJ"].ToString(), out jingdu)) { row["HSJJ"] = jingdu.ToString("F6"); }
                if (decimal.TryParse(row["YBJ"].ToString(), out jingdu)) { row["YBJ"] = jingdu.ToString("F6"); }
                if (decimal.TryParse(row["DHS"].ToString(), out jingdu)) { row["DHS"] = jingdu.ToString("F6"); }
                if (decimal.TryParse(row["DHSL"].ToString(), out jingdu)) { row["DHSL"] = jingdu.ToString("F6"); }
                if (decimal.TryParse(row["KCSL"].ToString(), out jingdu)) { row["KCSL"] = jingdu.ToString("F6"); }
                if (decimal.TryParse(row["SPZTSL"].ToString(), out jingdu)) { row["SPZTSL"] = jingdu.ToString("F6"); }

                row["MEMO"] = row["MEMO"].ToString() + row["STR0"].ToString();

                if (row["BZHL"].ToString() == "0" || row["BZHL"].ToString() == "")
                {
                    msg += row["GDNAME"].ToString() + ",";
                }
                else
                {
                    mydt.Rows.Add(row.ItemArray);
                }
            }
            return mydt;
        }

        protected void Window1_Close(object sender, WindowCloseEventArgs e)
        {
            DataTable dt = GetGoods(hfdValue.Text);
            dt.Columns.Remove(dt.Columns["BZHL"]);
            dt.Columns.Remove(dt.Columns["UNIT"]);

            if (dt != null && dt.Rows.Count > 0)
            {
                dt.Columns["UNITNAME"].ColumnName = "UNITSMALLNAME";
                dt.Columns["BZHL_ORDER"].ColumnName = "BZHL";
                dt.Columns["UNIT_ORDER_NAME"].ColumnName = "UNITNAME";
                dt.Columns["UNIT_ORDER"].ColumnName = "UNIT";

                dt.Columns.Add("PH", Type.GetType("System.String"));
                dt.Columns.Add("RQ_SC", Type.GetType("System.String"));
                dt.Columns.Add("YXQZ", Type.GetType("System.String"));
                dt.Columns.Add("MEMO", Type.GetType("System.String"));
                dt.Columns.Add("BZSL", Type.GetType("System.Int32"));
                dt.Columns.Add("DHS", Type.GetType("System.Int32"));
                dt.Columns.Add("DHSL", Type.GetType("System.Int32"));
                dt.Columns.Add("HSJE", Type.GetType("System.Double"));
                dt.Columns.Add("KCSL", Type.GetType("System.Int32"));
                dt.Columns.Add("SPZTSL", Type.GetType("System.Int32"));

                string msg = "";
                string someDjbh = string.Empty;
                DataTable dtBill = LoadGridData(dt, ref msg);
                if (dtBill != null && dtBill.Rows.Count > 0)
                {

                    foreach (DataRow row in dt.Rows)
                    {
                        row["BZSL"] = "0";
                        row["DHSL"] = "0";
                        row["HSJE"] = "0";
                        //row["HSJJ"].ToString();
                        if (string.IsNullOrWhiteSpace(row["HSJJ"].ToString()) || row["HSJJ"].ToString() == "0")
                        {
                            msg += "【" + row["GDSEQ"].ToString() + " | " + row["GDNAME"].ToString() + "】,";
                            Alert.Show("商品" + msg + "【含税进价】为空，不能进行【商品补货管理】操作。", "消息提示", MessageBoxIcon.Warning);
                            continue;
                        }
                        //LoadGridRow(row, false);
                        //处理金额格式
                        decimal jingdu = 0;
                        decimal bzhl = 0;
                        if (decimal.TryParse(row["HSJJ"].ToString(), out jingdu) && decimal.TryParse(row["BZHL"].ToString(), out bzhl)) { row["HSJJ"] = Math.Round(jingdu * bzhl, 4).ToString("F4"); }
                        if (decimal.TryParse(row["YBJ"].ToString(), out jingdu)) { row["YBJ"] = jingdu.ToString("F4"); }
                        if (decimal.TryParse(row["HSJE"].ToString(), out jingdu)) { row["HSJE"] = Math.Round(jingdu, 2).ToString("F2"); }

                        if (isDg())
                        {
                            row["NUM2"] = DbHelperOra.GetSingle(string.Format("SELECT NVL(ORDERZQ,7) FROM DOC_GOODSSUP WHERE GDSEQ = '{0}' AND NVL(PSSID,SUPID) = '{1}'", row["GDSEQ"], docPSSID.SelectedValue));
                        }
                        //取得商品供应商
                        row["SUPID"] = (DbHelperOra.GetSingle(string.Format("SELECT SUPID FROM DOC_GOODSSUP WHERE GDSEQ='{0}' AND ORDERSORT = 'Y'", row["GDSEQ"])) ?? "");
                        List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().OrderBy(x => x["GDSEQ"]).ToList();
                        int sameRowCount = newDict.Where(a => a["GDSEQ"].ToString() == row["GDSEQ"].ToString()).Count();
                        if (sameRowCount > 0)
                        {
                            someDjbh += "【" + row["GDNAME"].ToString() + "】";
                        }
                        else
                        {
                            PubFunc.GridRowAdd(GridGoods, row, false);
                            docDEPTID.Enabled = false;
                        }
                    }

                    //PubFunc.GridRowAdd(GridGoods, dtBill);
                }
                if (!string.IsNullOrWhiteSpace(someDjbh))
                {
                    Alert.Show("商品名称：" + someDjbh + "申请明细中已存在", "消息提示", MessageBoxIcon.Warning);

                }
                if (!string.IsNullOrWhiteSpace(msg))
                {
                    Alert.Show(string.Format("请先维护商品【{0}】的订货包装含量", msg.TrimEnd(',')), "异常提示", MessageBoxIcon.Warning);

                }
            }
            else
            {
                Alert.Show("请选择商品！", "消息提示", MessageBoxIcon.Warning);
            }
        }

        #region 该内容由于业务变动废弃掉了 c 20150510
        /// <summary>
        /// 该方法废弃 c 20150510
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void comGDSEQ_TriggerClick(object sender, EventArgs e)
        {

            string code = labGDSEQ.Text;
            string dept = docDEPTID.SelectedValue;

            if (!string.IsNullOrWhiteSpace(code) && code.Trim().Length >= 2)
            {
                DataTable dt_goods = Doc.GetGoods_KCP(code, "", dept);

                if (dt_goods != null && dt_goods.Rows.Count > 0)
                {
                    dt_goods.Columns.Add("BZSL", Type.GetType("System.Int32"));
                    dt_goods.Columns.Add("DHS", Type.GetType("System.Int32"));
                    dt_goods.Columns.Add("DHSL", Type.GetType("System.Int32"));
                    dt_goods.Columns.Add("HSJE", Type.GetType("System.Double"));
                    dt_goods.Columns.Add("KCSL", Type.GetType("System.Int32"));
                    dt_goods.Columns.Add("SPZTSL", Type.GetType("System.Int32"));
                    DataRow dr_goods = dt_goods.Rows[0];
                    //订货不管理批号
                    //DataTable dtPH = Doc.GetGoodsPHList(code);
                    //if (dtPH != null && dtPH.Rows.Count > 0)
                    //{
                    //    if (dtPH.Rows.Count == 1)
                    //    {
                    //        dr_goods["PH"] = dtPH.Rows[0]["PH"];
                    //        //dr_goods["PZWH"] = dtPH.Rows[0]["PZWH"];
                    //        dr_goods["RQ_SC"] = dtPH.Rows[0]["RQ_SC"];
                    //        dr_goods["YXQZ"] = dtPH.Rows[0]["YXQZ"];
                    //    }
                    //    else
                    //    {
                    //        hfdRowIndex.Text = GridGoods.SelectedRowIndex.ToString();
                    //        GridLot.DataSource = dtPH;
                    //        GridLot.DataBind();
                    //        WindowLot.Hidden = false;
                    //    }
                    //}
                    LoadGridRow(dr_goods);
                }
                else
                {
                    Alert.Show(string.Format("{0}尚未配置商品【{1}】或此商品为直送商品！！！", docDEPTID.SelectedText, code), MessageBoxIcon.Warning);
                    PubFunc.GridRowAdd(GridGoods, "CLEAR");
                }
            }
        }

        /// <summary>
        /// FineUIPro.Grid控件数据加载
        /// </summary>
        /// <param name="row">要加载的行数据</param>
        /// <param name="firstRow">是否插入指定行</param>
        /// <param name="flag">数据来源：NEW-从数据库中获得，用于商品新增时；OLD-从销售单据明细中获得，用于修改或审核时</param>
        private void LoadGridRow(DataRow row, bool firstRow = true, string flag = "NEW")
        {
            if (flag == "NEW")
            {
                if (!string.IsNullOrWhiteSpace(row["UNIT_ORDER"].ToString()))
                {
                    if (row["UNIT_ORDER"].ToString() == "D")//出库单位为大包装时
                    {
                        if (!string.IsNullOrWhiteSpace(row["NUM_DABZ"].ToString()) && row["NUM_DABZ"].ToString() != "0")
                        {
                            row["UNIT"] = row["UNIT_DABZ"];
                            row["UNITNAME"] = row["UNIT_DABZ_NAME"];
                            row["BZHL"] = row["NUM_DABZ"];
                            decimal price = 0, number = 0;
                            decimal.TryParse(row["HSJJ"].ToString(), out price);
                            decimal.TryParse(row["NUM_DABZ"].ToString(), out number);
                            row["HSJE"] = price * number;
                        }
                    }
                    else if (row["UNIT_ORDER"].ToString() == "Z")//出库单位为中包装时
                    {
                        if (!string.IsNullOrWhiteSpace(row["NUM_ZHONGBZ"].ToString()) && row["NUM_ZHONGBZ"].ToString() != "0")
                        {
                            row["UNIT"] = row["UNIT_ZHONGBZ"];
                            row["UNITNAME"] = row["UNIT_ZHONGBZ_NAME"];
                            row["BZHL"] = row["NUM_ZHONGBZ"];
                            decimal price = 0, number = 0;
                            decimal.TryParse(row["HSJJ"].ToString(), out price);
                            decimal.TryParse(row["NUM_ZHONGBZ"].ToString(), out number);
                            row["HSJE"] = price * number;
                        }
                    }
                }
            }
            //处理金额格式
            decimal jingdu = 0;
            if (decimal.TryParse(row[12].ToString(), out jingdu)) { row[12] = jingdu.ToString("F6"); }
            if (decimal.TryParse(row[13].ToString(), out jingdu)) { row[13] = jingdu.ToString("F6"); }
            if (decimal.TryParse(row[17].ToString(), out jingdu)) { row[17] = jingdu.ToString("F6"); }
            if (decimal.TryParse(row[19].ToString(), out jingdu)) { row[19] = jingdu.ToString("F6"); }

            PubFunc.GridRowAdd(GridGoods, row, firstRow);
        }
        #endregion

        protected void btnClosePostBack_Click(object sender, EventArgs e)
        {
            bool firstRow = true;
            foreach (GridRow row in GridLot.Rows)
            {
                int rowIndex = row.RowIndex;
                System.Web.UI.WebControls.TextBox tbxNumber = (System.Web.UI.WebControls.TextBox)GridLot.Rows[rowIndex].FindControl("tbxNumber");
                if (!string.IsNullOrWhiteSpace(tbxNumber.Text) && tbxNumber.Text != "0")
                {
                    string[] strCell = GridGoods.SelectedCell;
                    JObject defaultObj = Doc.GetJObject(GridGoods, strCell[0]);
                    defaultObj["PH"] = row.Values[1].ToString();
                    defaultObj["YXQZ"] = row.Values[2].ToString();
                    defaultObj["PZWH"] = row.Values[4].ToString();
                    defaultObj["RQ_SC"] = row.Values[3].ToString();
                    defaultObj["BZSL"] = tbxNumber.Text;
                    if (firstRow)
                    {
                        firstRow = false;
                        //string cell = string.Format("[{0},{1}]", intCell[0], intCell[1]);
                        //PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(GetJObject(newDict), cell));
                        PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(strCell[0], defaultObj));
                    }
                    else
                    {
                        PageContext.RegisterStartupScript(GridGoods.GetAddNewRecordReference(defaultObj));
                    }
                }
            }
            WindowLot.Hidden = true;
        }

        protected override void billDel()
        {
            if (docBILLNO.Text.Trim() == "")
            {
                Alert.Show("请选择需要删除的单据");
                return;
            }
            if (docFLAG.SelectedValue != "M" && docFLAG.SelectedValue != "R")
            {
                Alert.Show("非新单不能删除!");
                return;
            }
            if (!CheckFlag(docFLAG.SelectedValue))
            {
                Alert.Show("此单据已经被别人操作，请等待操作!");
                return;
            }

            List<CommandInfo> cmdList = new List<CommandInfo>();
            cmdList.Add(new CommandInfo("Delete from DAT_DD_DOC t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'", null));
            cmdList.Add(new CommandInfo("Delete from DAT_DD_COM t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'", null));
            cmdList.Add(new CommandInfo(" UPDATE DAT_DO_LIST SET DOUSER='" + UserAction.UserID + "',DORQ=SYSDATE,FLAG='Y' WHERE  PARA='" + docBILLNO.Text.Trim() + "'", null));
            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                Alert.Show("单据删除成功!");
                OperLog("商品订货", "删除单据【" + docBILLNO.Text + "】");
                PubFunc.FormDataClear(FormDoc);
                if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
                {
                    return;
                }
                else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
                {
                    return;
                }
                billSearch();
                billNew();
            }
            else
            {
                Alert.Show("单据删除失败!", "错误提示", MessageBoxIcon.Information);
            }

        }
        protected override void billExport()
        {
            if (string.IsNullOrWhiteSpace(docSEQNO.Text))
            {
                Alert.Show("请先选择要导出的订单信息！", "提示信息", MessageBoxIcon.Warning);
                Window1.Hidden = true;
                return;
            }
            string sql = @"SELECT A.BILLNO 单据编号,
                                   F_GETDEPTNAME(A.DEPTID) 订货部门,
                                   F_GETSUPNAME(B.SUPID) 供应商,
                                   TO_CHAR(A.XDRQ, 'YYYY-MM-DD') 订货日期,
                                   F_GETUSERNAME(A.CGY) 操作员,
                                   A.MEMO 订单备注说明,
                                   ' '||B.GDSEQ 商品编码,
                                   G.BAR3 ERP编码,
                                   B.ROWNO 行号,
                                   B.GDNAME 商品名称,
                                   B.GDSPEC 商品规格,
                                   F_GETUNITNAME(B.UNIT) 订货单位,
                                   B.BZHL 包装含量,
                                   B.BZSL 订货包装数,
                                   B.DHS 订货数,
                                   B.HSJJ 含税进价,
                                   B.HSJE 含税金额,
                                   B.DHSL 入库数,
                                   B.PZWH 注册证号,
                                   F_GETPRODUCERNAME(B.PRODUCER) 生产厂家,
                                   ' '||G.HISCODE HIS编码,
                                   B.MEMO 商品明细备注
                              FROM DAT_DD_DOC A, DAT_DD_COM B,DOC_GOODS G
                             WHERE B.SEQNO = '{0}' AND B.GDSEQ=G.GDSEQ
                               AND A.SEQNO = B.SEQNO
                             ORDER BY ROWNO";
            DataTable dt = DbHelperOra.Query(string.Format(sql, docSEQNO.Text)).Tables[0];
            if (dt.Rows.Count < 1)
            {
                Alert.Show("请先选择要导出的订单信息！", "提示信息", MessageBoxIcon.Warning);
                Window1.Hidden = true;
                return;
            }
            ExcelHelper.ExportByWeb(dt, string.Format("【{0}】订货信息", docDEPTID.SelectedText), "订货信息导出_" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
        }
        protected override void billCopy()
        {
            if (docBILLNO.Text.Trim() == "") return;
            if (!DbHelperOra.Exists(string.Format("select 1 from DAT_DD_DOC where SEQNO='{0}'", docBILLNO.Text)))
            {
                Alert.Show("此单据未保存,请检查!");
                return;
            }
            string new_bill = "";
            new_bill = BillSeqGet();
            DbHelperOra.ExecuteSql("insert into DAT_DD_DOC(SEQNO,BILLNO,BILLTYPE,FLAG,ISEND,ISAUTO,DEPTDH,DEPTID,PSSID,PSSNAME,XDRQ,DHFS,DHRQ,SUBNUM,SUBSUM,CGY,LRY,LRRQ,STR1，STR2，STR3，NUM1,NUM2,NUM3,MEMO,DHLX,ISYX) select '" + new_bill + "','" + new_bill + "',BILLTYPE,'M','N','N',DEPTDH,DEPTID,PSSID,PSSNAME,XDRQ,DHFS,DHRQ,SUBNUM,SUBSUM,'" + UserAction.UserID + "','" + UserAction.UserID + "',sysdate,STR1，STR2，STR3，NUM1,NUM2,NUM3,MEMO,DHLX,ISYX from DAT_DD_DOC WHERE SEQNO = '" + docBILLNO.Text + "'");
            DbHelperOra.ExecuteSql("insert into DAT_DD_COM(SEQNO,ROWNO,DEPTID,GDSEQ,BARCODE,GDNAME,UNIT,GDSPEC,SUPID,CDID,SPLB,CATID,BZHL,BZSL,DHS,DHSL,JXTAX,HSJJ,BHSJJ,HSJE,BHSJE,LSJ,LSJE,ISLOT,PHID,PH,PZWH,RQ_SC,YXQZ,KCSL,KCHSJE,SPZTSL,ERPAYXS,HLKC,PRODUCER,ZPBH,CKBM,STR2,STR3,NUM1,NUM2,NUM3,MEMO,ISGZ) SELECT '" + new_bill + "',ROWNO,DEPTID,GDSEQ,BARCODE,GDNAME,UNIT,GDSPEC,SUPID,CDID,SPLB,CATID,BZHL,BZSL,DHS,DHSL,JXTAX,HSJJ,BHSJJ,HSJE,BHSJE,LSJ,LSJE,ISLOT,PHID,PH,PZWH,RQ_SC,YXQZ,KCSL,KCHSJE,SPZTSL,ERPAYXS,HLKC,PRODUCER,ZPBH,CKBM,STR2,STR3,NUM1,NUM2,NUM3,MEMO,ISGZ FROM DAT_DD_COM WHERE SEQNO = '" + docBILLNO.Text + "'");
            billOpen(new_bill);
            PubFunc.FormLock(FormDoc, true, "");
            docXDRQ.Enabled = true;
            docDEPTID.Enabled = true;
            docCGY.Enabled = true;
            docPSSID.Enabled = true;
            Alert.Show("商品订货信息复制成功,新单据编号为'" + new_bill + "'");
        }
        protected void btnBatchPrint_Click(object sender, EventArgs e)
        {
            if (GridList.SelectedRowIndexArray.Length < 1)
            {
                Alert.Show("请选择要打印的单据信息！", "消息提醒", MessageBoxIcon.Warning);
                return;
            }
            string bills = "";
            foreach (int index in GridList.SelectedRowIndexArray)
            {
                //验证单据状态是否正确   N ,Y,G以外的单据不能打印
                if ((",Y,G").IndexOf(GridList.Rows[index].DataKeys[1].ToString()) < 1)
                {
                    Alert.Show("第【" + (index + 1).ToString() + "】行单据【" + GridList.Rows[index].DataKeys[0].ToString() + "】当前状态为【" + GridList.Rows[index].DataKeys[4].ToString() + "】，不允许打印！！！", "操作提示", MessageBoxIcon.Warning);
                    return;
                }
                bills += GridList.Rows[index].DataKeys[0].ToString() + "_";
            }

            hfdBills.Text = bills.Trim('_');
            PageContext.RegisterStartupScript("PrintDHD();");
        }
        protected void btnAuditBatch_Click(object sender, EventArgs e)
        {
            int[] selections = GridList.SelectedRowIndexArray;
            if (selections.Length == 0)
            {
                Alert.Show("请选择要审核的科室申领信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string succeed = "";
            string billno = "";
            string msgUpdate = "";
            bool flagUpdate = false;

            string shs = docPSSID.SelectedValue;    //供应商

            foreach (int rowIndex in selections)
            {
                //添加拦截判断
                if (GridList.DataKeys[rowIndex][1].ToString() == "N")
                {
                    billno = GridList.DataKeys[rowIndex][0].ToString();

                    //DataTable dtDoc = DbHelperOra.Query(string.Format(strDocSql, billno)).Tables[0];
                    //string supid = dtDoc.Rows[0]["PSSID"].ToString();

                    //DataTable ddComDT = DbHelperOra.Query("SELECT GDSEQ FROM DAT_DD_COM WHERE SEQNO='" + billno + "'").Tables[0];
                    //if (ddComDT.Rows.Count > 0)
                    //{
                    //    string lisResult = "";
                    //    for (int i = 0; i < ddComDT.Rows.Count; i++)
                    //    {
                    //        lisResult = lisResult + DbHelperOra.GetSingle("select F_LICENSE_ISEFFECTIVE('" + supid + "','" + ddComDT.Rows[i]["GDSEQ"].ToString() + "') from dual").ToString() + ",";
                    //    }

                    //    lisResult = lisResult.Substring(0, lisResult.Length - 1).Split(',')[0];
                    //    if (lisResult.Equals("验证通过"))
                    //    {
                    if (!Doc.getFlag(billno, "N", BillType))
                    {
                        msgUpdate += billno + ";";
                        flagUpdate = true;
                    }
                    if (Doc.getFlag(billno, "N", BillType))
                    {
                        if (BillOper(billno, "AUDIT") == 1)
                        {
                            succeed = succeed + "【" + billno + "】";
                        }
                    }
                    if (flagUpdate)
                    {
                        Alert.Show("单据编号【" + msgUpdate + "】被别人在操作，不能执行审核操作！");
                        return;
                    }

                    //if (succeed.Length > 0)
                    //{
                    //    Alert.Show("单据" + succeed + "审核成功！");
                    //    billSearch();
                    //}
                    //    }
                    //    else
                    //    {
                    //        Alert.Show(lisResult);
                    //    }
                    //}
                    //else
                    //{
                    //    Alert.Show("此单据审核失败，请联系系统管理人员！");
                    //}
                }
            }
            if (succeed.Length > 0)
            {
                Alert.Show("单据" + succeed + "审核成功！");
                billSearch();
            }

        }

        protected void btnAuto_Click(object sender, EventArgs e)
        {
            string dept = "%";
            if (docDEPTID.SelectedValue != null && !PubFunc.StrIsEmpty(docDEPTID.SelectedValue))
            {
                dept = docDEPTID.SelectedValue;
            }
            OracleParameter[] parameters ={
                                              new OracleParameter("VI_DEPT" ,OracleDbType.Varchar2,20),
                                              new OracleParameter("VI_USER" ,OracleDbType.Varchar2,20),
                                              new OracleParameter("VO_BILLNO",OracleDbType.Varchar2,20),
                                              new OracleParameter("VO_BILLNUM",OracleDbType.Int32)
                                           };
            parameters[0].Value = dept;
            parameters[1].Value = UserAction.UserID;

            parameters[0].Direction = ParameterDirection.Input;
            parameters[1].Direction = ParameterDirection.Input;
            parameters[2].Direction = ParameterDirection.Output;
            parameters[3].Direction = ParameterDirection.Output;

            try
            {
                DbHelperOra.RunProcedure("STORE.P_DD_AUTO", parameters);
                if (!PubFunc.StrIsEmpty(parameters[2].Value.ToString()) && parameters[2].Value.ToString().ToLower() != "null")
                {
                    Alert.Show(string.Format("自动订货生成成功，订单号【{0}】，条目数：【{1}】", parameters[2].Value.ToString(), parameters[3].Value.ToString()), "消息提示", MessageBoxIcon.Information);
                    billOpen(parameters[2].Value.ToString());
                }
                else
                {
                    Alert.Show("暂时没有需要自动订货的商品信息", "消息提示", MessageBoxIcon.Information);
                }
            }
            catch (Exception err)
            {
                Alert.Show(err.Message, "提示信息", MessageBoxIcon.Warning);
            }
        }
        protected void SetGoods()
        {
            int total = 0;
            string msg = "";
            NameValueCollection nvc = new NameValueCollection();
            DataTable dtData = GetGoods(Grid2.PageIndex, Grid2.PageSize, nvc, ref total, ref msg);
            Grid2.RecordCount = total;
            Grid2.DataSource = dtData;
            Grid2.DataBind();
        }

        public DataTable GetGoods(int pageNum, int pageSize, NameValueCollection nvc, ref int total, ref string errMsg)
        {

            string strSearch = "";
            if (!PubFunc.StrIsEmpty(labGDSEQ.Text)) { strSearch += string.Format(" and (A.ZJM  LIKE '%{0}%' OR A.GDSEQ  LIKE '%{0}%' OR A.GDNAME  LIKE '%{0}%' OR A.BAR3  LIKE '%{0}%')", labGDSEQ.Text.Trim().ToUpper()); }
            //strSearch += " ORDER BY A.GDNAME";
            string Sql = @"SELECT A.*,f_getunitname(A.UNIT) UNITNAME,f_getproducername(A.PRODUCER) PRODUCERNAME FROM DOC_GOODS A,doc_goodscfg B
                WHERE A.FLAG = 'Y' AND A.GDSEQ = B.GDSEQ  AND B.DEPTID = '{0}'";
            StringBuilder strSql = new StringBuilder(string.Format(Sql, docDEPTID.SelectedValue));
            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql.Append(strSearch);
            }
            return GetDataTable(pageNum, pageSize, strSql, ref total);
        }
        protected void PageManager1_CustomEvent(object sender, CustomEventArgs e)
        {
            if (e.EventArgument == "Search") {
                billSearch();
            }
            if (e.EventArgument.IndexOf("tgbGDSEQ_change_") >= 0)
            {
                SetGoods();
            }
            if (e.EventArgument.IndexOf("Grid2_bind_") >= 0)
            {
                SetGoods();
            }
            if (e.EventArgument.IndexOf("Grid2_click_") >= 0)
            {
                comGDSEQ_TriggerClick(null, null);
            }
            if (e.EventArgument.IndexOf("GoodsAdd") >= 0)
            {
                Window1_Close(null, null);
            }
            switch (e.EventArgument)
            {
                //删行
                case "billDelRow_Ok":
                    billDelRow_mes();
                    break;
                case "billDelRow_Cancel":
                    break;
                case "Confirm_OK":
                    pagesave(false);
                    page(docSEQNO.Text, 1);
                    break;
                case "Confirm_Cancel":
                    page(docSEQNO.Text, 1);
                    break;
            }
        }
        protected void Grid2_PageIndexChange(object sender, GridPageEventArgs e)
        {
            Grid2.PageIndex = e.NewPageIndex;
            SetGoods();
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

        protected void lstBILLNO_TriggerClick(object sender, EventArgs e)
        {
            //查询信息统一触发
            billSearch();
        }
        protected void btnScan_Click(object sender, EventArgs e)
        {
            if (PubFunc.StrIsEmpty(docSEQNO.Text))
            {
                Alert.Show("请保存单据后进行扫描追溯码操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_DD_EXT A WHERE A.BILLNO = '{0}'", docSEQNO.Text)))
            {
                Alert.Show("明细中无高值商品，不能进行扫描追溯码操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string sql = "SELECT A.*,f_getunitname(A.UNIT) UNITNAME FROM DAT_DD_EXT A WHERE A.BILLNO = '{0}' ORDER BY A.GDSEQ,A.INSTIME DESC";

            DataTable dtScan = DbHelperOra.Query(string.Format(sql, docSEQNO.Text)).Tables[0];
            GridSacn.DataSource = dtScan;
            GridSacn.DataBind();
            WindowScan.Hidden = false;
        }

        /// <summary>
        ///  20150510 liuz  解决带出信息更新汇总信息显示
        /// </summary>
        /// <param name="newDict"></param>
        private void UpdateSum(List<Dictionary<string, object>> newDict)
        {
            //计算合计数量
            decimal bzslTotal = 0, feeTotal = 0;
            foreach (Dictionary<string, object> dic in newDict)
            {
                bzslTotal += Convert.ToDecimal(dic["BZSL"]);
                feeTotal += (Convert.ToDecimal(dic["HSJJ"]) * Convert.ToDecimal(dic["BZSL"] ?? "0"));
            }
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("BZSL", bzslTotal.ToString());
            summary.Add("HSJE", feeTotal.ToString("F2"));

            GridGoods.SummaryData = summary;
        }

        /// <summary>
        /// 20150510   liuz  增加提交操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCommit_Click(object sender, EventArgs e)
        {
            string strBILLNO = docBILLNO.Text;
            List<CommandInfo> sqlList = new List<CommandInfo>();
            if (strBILLNO.Length == 0)
            {
                Alert.Show("请先保存后，再次提交！");
                return;
            }
            if (docFLAG.SelectedValue != "M")
            {
                Alert.Show("只有保存后单据，才能提交！");
                return;
            }
            string strBillno = docSEQNO.Text;
            if (!Doc.getFlag(strBillno, "M", BillType))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                return;
            }
            hfdSave.Text = "S";
            SaveSuccess = false;
            DataSave(true);
            if(SaveSuccess == false)
            {
                return;
            }
            SaveSuccess = false;
            //string shs = docPSSID.SelectedValue;    //供应商
            //DataTable ddComDT = DbHelperOra.Query("SELECT GDSEQ FROM DAT_DD_COM WHERE SEQNO='" + docBILLNO.Text + "'").Tables[0];
            //if(ddComDT.Rows.Count > 0)
            //{
            //    string lisResult="";
            //    for (int i = 0; i < ddComDT.Rows.Count;i++ )
            //    {
            //        lisResult = lisResult + DbHelperOra.GetSingle("select F_LICENSE_ISEFFECTIVE('" + shs + "','" + ddComDT.Rows[i]["GDSEQ"].ToString() + "') from dual").ToString() + ",";
            //    }

            //    lisResult = lisResult.Substring(0, lisResult.Length - 1).Split(',')[0];
            //    if (lisResult.Equals("验证通过"))
            //    {
            sqlList.Add(new CommandInfo("update DAT_DD_DOC set flag='N' where BILLNO='" + strBILLNO + "' ", null));
            //增加待办事宜
            sqlList.Add(Doc.GETDOADD("DO_7", docDEPTID.SelectedValue, docLRY.SelectedValue, docBILLNO.Text));

            if (DbHelperOra.ExecuteSqlTran(sqlList))
            {
                Alert.Show("此单据提交成功！");
                OperLog("商品订货", "提交单据【" + docBILLNO.Text + "】");
                billSearch();
                billOpen(docBILLNO.Text);
                billLockDoc(true);
            }
            else
            {
                Alert.Show("此单据提交失败，请联系系统管理人员！");
            }
            //    }
            //    else
            //    {
            //        Alert.Show(lisResult);
            //    }
            //}
            //else
            //{
            //    Alert.Show("此单据提交失败，请联系系统管理人员！");
            //}


        }

        protected void btnAllCommit_Click(object sender, EventArgs e)
        {
            List<CommandInfo> sqlList = new List<CommandInfo>();
            string succeed = string.Empty;
            if (GridList.SelectedRowIndexArray.Length == 0)
            {
                Alert.Show("你选中的单据中，没有要提交的单据");
                return;
            }

            for (int i = 0; i < GridList.SelectedRowIndexArray.Length; i++)
            {
                int rowIndex = GridList.SelectedRowIndexArray[i];
                if (GridList.DataKeys[rowIndex][1].ToString() == "M")
                {
                    string strBILLNO = GridList.DataKeys[rowIndex][0].ToString();

                    //DataTable dtDoc = DbHelperOra.Query(string.Format(strDocSql, strBILLNO)).Tables[0];
                    //string supid = dtDoc.Rows[0]["PSSID"].ToString();
                    //string seqno = dtDoc.Rows[0]["BILLNO"].ToString();

                    //DataTable ddComDT = DbHelperOra.Query("SELECT GDSEQ FROM DAT_DD_COM WHERE SEQNO='" + seqno + "'").Tables[0];
                    //if (ddComDT.Rows.Count > 0)
                    //{
                    //    string lisResult = "";
                    //    for (int j = 0; j < ddComDT.Rows.Count; j++)
                    //    {
                    //        lisResult = lisResult + DbHelperOra.GetSingle("select F_LICENSE_ISEFFECTIVE('" + supid + "','" + ddComDT.Rows[i]["GDSEQ"].ToString() + "') from dual").ToString() + ",";
                    //    }

                    //    lisResult = lisResult.Substring(0, lisResult.Length - 1).Split(',')[0];
                    //    if (lisResult.Equals("验证通过"))
                    //    {
                    sqlList.Add(new CommandInfo("update DAT_DD_DOC set flag='N' where BILLNO='" + strBILLNO + "' ", null));
                    //增加待办事宜
                    sqlList.Add(Doc.GETDOADD("DO_7", GridList.DataKeys[rowIndex][2].ToString(), GridList.DataKeys[rowIndex][3].ToString(), strBILLNO));
                    if (DbHelperOra.ExecuteSqlTran(sqlList))
                    {
                        succeed = succeed + "【" + strBILLNO + "】";
                    }
                    //if (succeed.Length > 0)
                    //{
                    //    Alert.Show("单据" + succeed + "提交成功！");
                    //    billSearch();
                    //}
                    //    }
                    //    else
                    //    {
                    //        Alert.Show(lisResult);
                    //    }
                    //}
                    //else
                    //{
                    //    Alert.Show("此单据提交失败，请联系系统管理人员！");
                    //}
                }
            }
            if (succeed.Length > 0)
            {
                Alert.Show("单据" + succeed + "提交成功！");
                billSearch();
            }

            //else
            //{
            //    Alert.Show("你选中的单据中，没有要提交的单据");
            //}

        }


        /// <summary>
        /// 20150510 liuz  到货日期的验证 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //protected void dpkDHRQ_TextChanged(object sender, EventArgs e)
        //{
        //    DateTime StartTime = Convert.ToDateTime(docXDRQ.SelectedDate);
        //    DateTime endTime = Convert.ToDateTime(dpkDHRQ.SelectedDate);

        //    if (StartTime > endTime)
        //    {
        //        dpkDHRQ.SelectedDate = null;
        //        Alert.Show("订货日期要大于到货日期！");
        //    }
        //}

        private bool CheckFlag(string flag)
        {
            if (docBILLNO.Text.Length > 0)
            {
                return Doc.getFlag(docBILLNO.Text, flag, BillType);
            }
            return true;
        }

        protected void GridList_Sort(object sender, GridSortEventArgs e)
        {
            highlightRows.Text = "";
            highlightRowYellow.Text = "";
            highlightRowRed.Text = "";
            GridList.SortDirection = e.SortDirection;
            GridList.SortField = e.SortField;

            DataTable table = PubFunc.GridDataGet(GridList);
            DataView view1 = table.DefaultView;
            view1.Sort = String.Format("{0} {1}", GridList.SortField, GridList.SortDirection);
            GridList.DataSource = view1;
            GridList.DataBind();
        }
        protected void rblTYPE_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rblTYPE.SelectedValue == "XS")
            {
                dbpOrder1.Enabled = true;
                dbpOrder2.Enabled = true;
                memo.Text = "订货量 =（备货天数+订货周期)*日均用量 -库房库存";
            }
            else if (rblTYPE.SelectedValue == "DS")
            {
                dbpOrder1.Enabled = false;
                dbpOrder2.Enabled = false;
                memo.Text = "订货量 = ROUND｛[科室周期使用总量-(库存-定数总量)-在途库存]/定数总量}*定数总量";
            }
            else
            {
                dbpOrder1.Enabled = false;
                dbpOrder2.Enabled = false;
                memo.Text = "订货量 = 最高库存 -在途库存-库房库存";
            }
        }

        protected void GridGoods_PageIndexChange(object sender, GridPageEventArgs e)
        {
            if (hdfBH.Text == "1" && modif())
            {
                PageContext.RegisterStartupScript(Confirm.GetShowReference("数据已被修改是否保存？", String.Empty, MessageBoxIcon.Question, PageManager1.GetCustomEventReference("Confirm_OK"), PageManager1.GetCustomEventReference("Confirm_Cancel")));
                hdftest.Text = e.NewPageIndex.ToString();
                hdfBH.Text = "0";
            }
            else
            {
                GridGoods.PageIndex = e.NewPageIndex;
                page(docSEQNO.Text, 0);
            }
        }

        protected void tgxGoods_TriggerClick(object sender, EventArgs e)
        {
            if (docBILLNO.Text.Length < 1)
            {
                Alert.Show("请首先保存单据！", "提示信息");
                return;
            }
            if (hdfBH.Text == "1" && modif())
            {
                PageContext.RegisterStartupScript(Confirm.GetShowReference("数据已被修改是否保存？", String.Empty, MessageBoxIcon.Question, PageManager1.GetCustomEventReference("Confirm_OK"), PageManager1.GetCustomEventReference("Confirm_Cancel")));
                hdfBH.Text = "0";
                hdftest.Text = GridGoods.PageIndex.ToString();
            }
            else
            {
                page(docSEQNO.Text, 0);
            }
        }
        protected bool modif()
        {
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
            for (int i = 0; i < newDict.Count; i++)
            {
                if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_DD_COM WHERE SEQNO = '{0}' AND GDSEQ = '{1}' AND BZSL = {2} AND MEMO = '{3}'", docBILLNO.Text, newDict[i]["GDSEQ"], newDict[i]["BZSL"], newDict[i]["MEMO"])))
                {
                    return true;
                }
            }
            return false;
        }
        protected void GridList_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridList.PageIndex = e.NewPageIndex;
            billSearch();
        }
        protected void btn_Sure_Click(object sender, EventArgs e)
        {
            if (ddlDeptOrder.SelectedValue == "")
            {
                Alert.Show("【订货部门】未维护！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            string sup = "%";
            if (!string.IsNullOrWhiteSpace(docPSSID.SelectedValue))
            {
                sup = docPSSID.SelectedValue;
            }
            OracleParameter[] parameters ={
                                            new OracleParameter("VI_DEPT" ,OracleDbType.Varchar2),
                                            new OracleParameter("VI_TYPE" ,OracleDbType.Varchar2),
                                            //new OracleParameter("VI_PARAS" ,OracleDbType.Varchar2),
                                             new OracleParameter("VI_BEG",OracleDbType.Varchar2),
                                             new OracleParameter("VI_END",OracleDbType.Varchar2),
                                             new OracleParameter("VI_USER" ,OracleDbType.Varchar2),
                                             new OracleParameter("VO_BILLNUM",OracleDbType.Decimal)
                                           };
            parameters[0].Value = ddlDeptOrder.SelectedValue;
            parameters[1].Value = rblTYPE.SelectedValue;
            // parameters[2].Value = dbpOrder1.Text + "_" + dbpOrder2.Text + "_" + sup;
            parameters[2].Value = dbpOrder1.Text;
            parameters[3].Value = dbpOrder2.Text;
            parameters[4].Value = UserAction.UserID;

            parameters[0].Direction = ParameterDirection.Input;
            parameters[1].Direction = ParameterDirection.Input;
            parameters[2].Direction = ParameterDirection.Input;
            parameters[3].Direction = ParameterDirection.Input;
            parameters[4].Direction = ParameterDirection.Input;
            parameters[5].Direction = ParameterDirection.Output;

            try
            {
                DbHelperOra.RunProcedure("STORE.P_DD_AUTO", parameters);
                if (Convert.ToInt32(parameters[5].Value.ToString()) > 0)
                {
                    Alert.Show("共有【" + parameters[5].Value + "】条自动订货数据生成成功！", "消息提示", MessageBoxIcon.Information);
                    //billOpen(parameters[4].Value.ToString());
                    OperLog("自动订货", "生成自动订货单据单据");
                    WinAuto.Hidden = true;
                }
                else
                {
                    Alert.Show("未找到自动生成订单数据！", "消息提示", MessageBoxIcon.Warning);
                }
            }
            catch (Exception err)
            {
                Alert.Show(err.Message);
            }
        }
        protected void btn_Auto_Click(object sender, EventArgs e)
        {
            //自动订货
            WinAuto.Hidden = false;
            dbpOrder1.SelectedDate = DateTime.Now.AddMonths(-1);
            dbpOrder2.SelectedDate = DateTime.Now;
            ddlDeptOrder.SelectedValue = docDEPTID.SelectedValue;
        }
    }
}