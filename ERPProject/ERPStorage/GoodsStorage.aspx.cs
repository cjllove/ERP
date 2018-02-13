using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FineUIPro;
using System.Data;
using XTBase;
using Newtonsoft.Json.Linq;
using System.Net;
using ERPProject;
using Oracle.ManagedDataAccess.Client;
using XTBase.Utilities;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace ERPProject.ERPStorage
{
    public partial class GoodsStorage : BillBase
    {
        string strDocSql = "SELECT * FROM DAT_RK_DOC WHERE SEQNO ='{0}'";
        string strComSql = @"SELECT A.SEQNO,A.ROWNO,A.DEPTID,A.GDSEQ,A.BARCODE,A.GDNAME,A.UNIT,A.GDSPEC,A.GDMODE,A.CDID,A.SPLB,A.CATID,A.HWID,A.BZHL,A.BZSL,
                            A.DDSL,A.SSSL,A.JXTAX,A.HSJJ,A.BHSJJ,A.HSJE,A.BHSJE,A.LSJ,A.LSJE,A.ISGZ,A.ISLOT,A.PHID,A.PH, A.PZWH,TO_CHAR(A.RQ_SC,'YYYY-MM-DD') RQ_SC,
                            TO_CHAR(A.YXQZ,'YYYY-MM-DD') YXQZ,A.KCSL,A.KCHSJE,A.SPZTSL,A.ERPAYXS,A.HLKC,A.ZPBH,A.STR1,A.STR2,A.STR3,A.NUM1,A.NUM2,
                            A.NUM3,A.MEMO, F_GETUNITNAME(A.UNIT) UNITNAME,F_GETUNITNAME(B.UNIT) UNITSMALLNAME,A.MJRQ,A.MJPH,A.MJXQ,
                            A.PRODUCER,f_getproducername(A.PRODUCER) PRODUCERNAME,A.SUPID,f_getsupname(a.supid) supname,F_GETDEPTNAME(A.DEPTID) DEPTNAME,
                           NVL((SELECT SUM(NVL(SSSL/BZHL,0)) FROM DAT_RK_DOC DDD,DAT_RK_COM DDC WHERE DDD.SEQNO = DDC.SEQNO  AND DDC.GDSEQ=A.GDSEQ AND DDC.PH=A.PH AND DDD.DDBH = C.DDBH AND DDD.FLAG IN('Y','G') GROUP BY DDC.PH),0) YRKSL
                              FROM DAT_RK_COM A, DOC_GOODS B,DAT_RK_DOC C
                             WHERE A.SEQNO = '{0}'
                               AND A.GDSEQ = B.GDSEQ
                               AND A.SEQNO=C.SEQNO
                             ORDER BY A.ROWNO";

        private string strSql = @"SELECT A.SEQNO,A.BILLNO,F_GETBILLFLAG(FLAG) FLAG_CN,A.FLAG,A.DDBH,F_GETDEPTNAME(A.DEPTID) DEPTID,
                                 A.DHRQ,A.SUBSUM,F_GETUSERNAME(A.CGY) CGY,f_getsupname(a.pssid) pssname,A.PSSID,A.BILLTYPE,                               A.DHRQ,A.SUBSUM,F_GETUSERNAME(A.CGY) CGY,f_getsupname(a.pssid) pssname,A.PSSID,A.BILLTYPE,
                                F_GETUSERNAME(A.LRY) LRY,A.LRRQ,F_GETUSERNAME(A.SHR) SHR,A.SHRQ,A.MEMO ,        
                               NVL((SELECT FUNCTIME FROM SYS_FUNCPRNNUM WHERE FUNCNO = A.SEQNO),0) PRINTNUM
                                FROM DAT_RK_DOC A  ";
        DataTable PAD_dtZSM;
        int CloseWindow = 0;
        bool ISYPT = false;//是否对接云平台，对接-true,未对接-false
        protected string YSRKD = "/grf/rksjd.grf";
        public override Field[] LockControl
        {
            get { return new Field[] { docBILLNO, docCGY, docDDBH, docPSSID, docDEPTID, docDHRQ }; }
        }

        public GoodsStorage()
        {
            BillType = "RKD";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindDDL();
                billNew();
                #region 读取客户标记，更改客户化需求
                string USERXMID = "";
                USERXMID = (DbHelperOra.GetSingle("select VALUE from sys_para WHERE CODE='USERXMID'")).ToString();
                if (USERXMID == "TJ_YKGZ") TJ_YKGZ();
                #endregion

                ButtonHidden(btnCopy, btnNext, btnBef, btnExport, btnAddRow);
                if (Request.QueryString["dg"] != null)
                {
                    hfdDG.Text = Request.QueryString["dg"].ToString();
                }
                //屏蔽不需要的操作按钮
                if (Request.QueryString["oper"] != null)
                {
                    hfdOper.Text = Request.QueryString["oper"].ToString();
                    if (Request.QueryString["oper"].ToString() == "input")
                    {
                        ButtonHidden(btnAudit, btnCancel, btnEvaluate, btnScan, btnCK);
                        btnBatchAudit.Hidden = true;
                        btnBatchPrint.Hidden = true;
                        if (string.IsNullOrWhiteSpace(docDEPTID.SelectedValue))
                        {
                            docDEPTID.SelectedIndex = 1;
                        }
                        Line1.Hidden = true;
                    }
                    else if (Request.QueryString["oper"].ToString() == "audit")
                    {
                        btnBatchAudit.Hidden = false;
                        btnBatchPrint.Hidden = false;
                        //隐藏按钮
                        LinkButton1.Hidden = true;
                        fuDocument.Hidden = true;
                        billLockDoc(true);
                        ButtonHidden(btnNew, btnCopy, btnSave, btnAddRow, btnDelRow, btnGoods, btnCommit, btnAllCommit, btnDel, btnHC);
                        ListLine.Hidden = true;
                        WebLine1.Hidden = true;
                        WebLine2.Hidden = true;
                        TabStrip1.ActiveTabIndex = 0;
                        if (Request.QueryString["pid"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["pid"].ToString()))
                        {
                            lstBILLNO.Text = Request.QueryString["pid"].ToString();
                            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(90);
                            billOpen(lstBILLNO.Text);
                        }
                        else
                        {
                            billSearch();
                        }
                    }
                    else if (Request.QueryString["oper"].ToString() == "querylist")
                    {
                        string result = listSearchForPad();
                        Response.ContentType = "text/plain";
                        Response.Write(result);
                        Response.End();
                    }
                    else if (Request.QueryString.Get("oper") == "queryBill" && !string.IsNullOrEmpty(Request.QueryString.Get("billno")))
                    {
                        string result = billSearchForPad();
                        Response.ContentType = "text/plain";
                        Response.Write(result);
                        Response.End();
                    }
                    else if (Request.QueryString.Get("oper") == "scan" && !string.IsNullOrEmpty(Request.QueryString.Get("onecode")) && !string.IsNullOrEmpty(Request.QueryString.Get("billno")))
                    {
                        string result = zsmScan_forPAD();
                        Response.ContentType = "text/plain";
                        Response.Write(result);
                        Response.End();
                    }
                    else if (Request.QueryString.Get("oper") == "submit")
                    {
                        string result = billAuditForPad();
                        Response.ContentType = "text/plain";
                        Response.Write(result);
                        Response.End();
                    }
                    else if (Request.QueryString.Get("oper") == "btnCkSureClick")
                    {
                        string result = billCkSurePad();
                        Response.ContentType = "text/plain";
                        Response.Write(result);
                        Response.End();
                    }
                }

                hfdOneCode.Text = Doc.DbGetSysPara("ISONECODE");
                if (PubFunc.StrIsEmpty(hfdOneCode.Text))
                {
                    hfdOneCode.Text = "Y";
                }
                //不启用唯一码
                if (hfdOneCode.Text == "N")
                {
                    btnScan.Hidden = true;
                }
                hfdCurrent.Text = UserAction.UserID;
            }
            else
            {
                if (GetRequestEventArgument() == "TextBox1_ENTER")
                {
                    zsmScan_TextChanged("ENTER", null);
                }
                else if (GetRequestEventArgument() == "TextBox2_ENTER")
                {
                    zsmERP_TextChanged("ENTER", null);
                }
                else if (GetRequestEventArgument() == "ConfirmOK")
                {
                    Window2.Hidden = false;
                    Window2.IFrameUrl = "SupEvaluation.aspx?billno=" + docSEQNO.Text;
                }
            }

            //获取客户化GRF文件地址  By c 2016年1月21日12:18:29 At 威海509
            string grf = Doc.DbGetGrf("YSRKD");
            if (!string.IsNullOrWhiteSpace(grf))
            {
                YSRKD = grf;
            }
        }
        protected override void BindDDL()
        {
            //if (!isDg())
            //{
            //    PubFunc.DdlDataGet("DDL_SYS_DEPOTRANGE_STORE", UserAction.UserID, docDEPTID, lstDEPTID);
            //}
            //else
            //{
            //    PubFunc.DdlDataGet("DDL_SYS_DEPOTRANGE_ORDER_DG", UserAction.UserID, docDEPTID, lstDEPTID);
            //}

            DepartmentBind.BindDDL("DDL_SYS_DEPOTRANGE", UserAction.UserID, docDEPTID, lstDEPTID);

            PubFunc.DdlDataGet("DDL_SYS_DEPTDEF", ddlDEPTCK);
            PubFunc.DdlDataGet("DDL_USERALL", lstLRY, docCGY, docSHR, docLRY);

            PubFunc.DdlDataGet("DDL_BILL_STATUSDHD", lstFLAG, docFLAG);
            PubFunc.DdlDataGet(ddlReject, "DDL_BILL_REASON");
            if (!isDg())
            {
                PubFunc.DdlDataGet("DDL_DOC_SUPPLIER", lstSUPID, docPSSID);
            }
            else
            {
                PubFunc.DdlDataGet("DDL_DOC_SUPPLIER_DG", lstSUPID, docPSSID);
            }


            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
            docDHRQ.SelectedDate = DateTime.Now;
            lstDEPTID.SelectedValue = UserAction.UserDept;
            docDEPTID.SelectedValue = Doc.DbGetSysPara("DEFDEPT");
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

        protected void btnHC_Click(object sender, EventArgs e)
        {
            List<CommandInfo> cmdList = new List<CommandInfo>();
            if (docSEQNO.Text.Length < 1) return;
            if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_RK_DOC WHERE SEQNO = '{0}' AND FLAG = 'N'", docSEQNO.Text)))
            {
                cmdList.Add(new CommandInfo("UPDATE DAT_RK_DOC SET FLAG = 'M' WHERE SEQNO = '" + docSEQNO.Text.Trim() + "' and FLAG='N'", null));
                cmdList.Add(new CommandInfo("UPDATE DAT_DO_LIST SET FLAG='Y' WHERE PARA='" + docBILLNO.Text.Trim() + "'", null));
                if (DbHelperOra.ExecuteSqlTran(cmdList))
                {
                    Alert.Show("单据【" + docSEQNO.Text + "】撤回成功！");
                    billOpen(docSEQNO.Text);
                    OperLog("商品入库", "回撤单据【" + docBILLNO.Text + "】");

                }

            }
            else
            {
                Alert.Show("单据【" + docSEQNO.Text + "】未提交或已被审核，不能撤回！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
        }
        protected void bind_print()
        {
            string ip = "";
            string ip_rtn = "";
            if (Context.Request.ServerVariables["HTTP_VIA"] != null)
            {
                ip = Context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else
            {
                ip = Context.Request.ServerVariables["REMOTE_ADDR"].ToString();
            }
            foreach (IPAddress ip4 in Dns.GetHostEntry(ip).AddressList)
            {
                if (ip4.AddressFamily.ToString() == "InterNetwork")
                {
                    ip_rtn = ip4.ToString();
                    break;
                }
            }
            DataTable print = new DataTable();
            print = DbHelperOra.Query("SELECT PRINTNAME FROM SYS_PRINT WHERE IP='" + ip_rtn + "' AND BILLTYPE='BILL_RKD' ORDER BY STR1").Tables[0];
            if (print.Rows.Count > 0)
            {
                print_a4.Text = print.Rows[0]["PRINTNAME"].ToString();
                if (print.Rows.Count > 1)
                { print_liu.Text = print.Rows[1]["PRINTNAME"].ToString(); }
            }
        }
        protected override void billClear()
        {
            PubFunc.FormDataClear(Formlist);
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
        }
        protected override void billSearch()
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【录入日期】！");
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                Alert.Show("【开始日期】大于【结束日期】，请重新输入！", "提示信息", MessageBoxIcon.Warning);
                return;
            }

            strSql += " WHERE A.DEPTID in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '" + UserAction.UserID + "') = 'Y' ) ";
            string strSearch = "";


            if (lstBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND A.BILLNO  LIKE '%{0}%'", lstBILLNO.Text);
            }
            if (lstFLAG.SelectedItem != null && lstFLAG.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.FLAG='{0}'", lstFLAG.SelectedItem.Value);
            }
            if (lstLRY.SelectedItem != null && lstLRY.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.LRY='{0}'", lstLRY.SelectedItem.Value);
            }
            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedItem.Value);
            }
            if (lstSUPID.SelectedItem != null && lstSUPID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.PSSID='{0}'", lstSUPID.SelectedItem.Value);
            }
            if (lstDDBH.Text.Length > 0)
            {
                strSearch += string.Format(" AND A.DDBH LIKE '%{0}%'", lstDDBH.Text);
            }
            if (Request.QueryString["oper"].ToString() == "audit")
            {
                strSearch += " AND A.FLAG<>'M'";
            }
            if (isDg())
            {
                //系统默认00001表示医院，并且为代管模式（主要是针对医院初始库存导入的情况） By c 20160116
                strSearch += " AND (PSSID = '00001' OR PSSID IN (SELECT DISTINCT NVL(A.PSSID, A.SUPID) PSSID FROM DOC_GOODSSUP A WHERE A.TYPE = '1'))";
            }
            else
            {
                strSearch += " AND PSSID IN (SELECT DISTINCT NVL(A.PSSID, A.SUPID) PSSID FROM DOC_GOODSSUP A WHERE A.TYPE IN ('0','Z'))";
            }

            strSearch += string.Format(" AND  A.LRRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND  A.LRRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
             strSql += " ORDER BY A.FLAG,A.DHRQ DESC,A.DDBH DESC,A.BILLNO ASC";
            //strSql += "ORDER BY " + GridList.SortField + " " + GridList.SortDirection;
            highlightRows.Text = ",";
            highlightRowYellow.Text = ",";
            DataTable table = DbHelperOra.Query(strSql).Tables[0];
       //     string sortField = GridList.SortField;
         //   string sortDirection = GridList.SortDirection;
            DataView view1 = table.DefaultView;
      //      view1.Sort = String.Format("{0} {1}", sortField, sortDirection);


            GridList.DataSource = view1;
            GridList.DataBind();
        }

        protected override void billGoods()
        {
            if (docFLAG.SelectedValue != "M" && docFLAG.SelectedValue != "R")
            {
                Alert.Show("非新单不能增加商品");
                return;
            }
            if (ISYPT && docBILLNO.Text.Length > 0 && docBILLNO.Text.StartsWith("TRI"))
            {
                Alert.Show("下传单据不能追加商品！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
            string url = "";
            if (isDg())
            {
                url = "~/ERPQuery/GoodsWindow_New.aspx?Deptout=" + docDEPTID.SelectedValue + "&Shs=" + docPSSID.SelectedValue + "&isdg=Y&GoodsState=Y";
            }
            else
            {
                url = "~/ERPQuery/GoodsWindow_New.aspx?Deptout=" + docDEPTID.SelectedValue + "&Shs=" + docPSSID.SelectedValue + "&isdg=N&GoodsState=Y";
            }

            PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "商品信息查询"));
        }

        protected override void listRow_DoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            //billClear();
            billOpen(GridList.Rows[e.RowIndex].Values[1].ToString());


        }

        protected override void billOpen(string strBillno)
        {
            DataTable dtDoc = DbHelperOra.Query(string.Format(strDocSql, strBillno)).Tables[0];
            if (dtDoc != null && dtDoc.Rows.Count > 0)
            {
                PubFunc.FormDataSet(FormDoc, dtDoc.Rows[0]);
            }
            else
            {
                Alert.Show("单据信息获取失败！！！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (docLRY.SelectedValue == null)
            {
                docLRY.SelectedValue = UserAction.UserID;
            }

            PageContext.RegisterStartupScript(GridCom.GetRejectChangesReference());
            DataTable dtBill = DbHelperOra.Query(string.Format(strComSql, strBillno)).Tables[0];
            if (dtBill != null && dtBill.Rows.Count > 0)
            {
                decimal ddslTotal = 0, bzslTotal = 0, feeTotal = 0;
                foreach (DataRow row in dtBill.Rows)
                {
                    ddslTotal += Convert.ToDecimal(string.IsNullOrWhiteSpace(row["DDSL"].ToString()) ? "0" : row["DDSL"].ToString());
                    if (!PubFunc.StrIsEmpty(Convert.ToString(row["BZSL"] ?? "0")))
                    {
                        bzslTotal += Convert.ToDecimal(row["BZSL"] ?? "0");
                        // 还要乘以最小单位
                        //feeTotal += Convert.ToDecimal(Convert.ToString(row["HSJJ"])) * Convert.ToDecimal(Convert.ToString(row["BZSL"] ?? "0")) * Convert.ToDecimal(Convert.ToString(row["SSSL"] ?? "0"));
                        feeTotal += Convert.ToDecimal(row["HSJJ"]) * Convert.ToDecimal(row["BZSL"] ?? "0");
                    }
                    row["HSJE"] = Convert.ToDecimal(row["HSJE"]).ToString("F2");

                }

                Doc.GridRowAdd(GridCom, dtBill);

                //计算合计数量
                JObject summary = new JObject();
                summary.Add("GDNAME", "本页合计");
                summary.Add("DDSL", ddslTotal.ToString());
                summary.Add("BZSL", bzslTotal.ToString());
                summary.Add("HSJE", feeTotal.ToString("F2"));
                GridCom.SummaryData = summary;
            }

            PubFunc.FormLock(FormDoc, true, "");
            if (docDHRQ.Text == "")
            {
                docDHRQ.SelectedDate = DateTime.Now;
            }
            if (docFLAG.SelectedValue == "M")
            {
                docDHRQ.Enabled = true;
            }
            TabStrip1.ActiveTabIndex = 1;
            //增加按钮控制
            if (docFLAG.SelectedValue == "M" || docFLAG.SelectedValue == "R")
            {
                btnDel.Enabled = true;
                btnSave.Enabled = true;
                btnCommit.Enabled = true;
                btnAudit.Enabled = false;
                btnCancel.Enabled = false;
                btnPrint.Enabled = false;
                btnPrint.Enabled = false;
                btnDelRow.Enabled = true;
                btnGoods.Enabled = true;
                btnHC.Enabled = false;
                btnCK.Enabled = false;
            }
            else if (docFLAG.SelectedValue == "N")
            {
                btnDel.Enabled = false;
                btnSave.Enabled = false;
                btnCommit.Enabled = false;
                btnAudit.Enabled = true;
                btnCancel.Enabled = true;
                btnPrint.Enabled = false;
                btnPrint.Enabled = false;
                btnDelRow.Enabled = false;
                btnGoods.Enabled = false;
                btnHC.Enabled = true;
                btnCK.Enabled = false;
                comPH.Enabled = false;
            }
            else
            {
                btnDel.Enabled = false;
                btnSave.Enabled = false;
                btnCommit.Enabled = false;
                btnAudit.Enabled = false;
                btnCancel.Enabled = false;
                btnPrint.Enabled = true;
                btnPrint.Enabled = true;
                btnDelRow.Enabled = false;
                btnGoods.Enabled = false;
                btnHC.Enabled = false;
                btnCK.Enabled = true;
            }
            hfdBills.Text = strBillno;
        }
        protected override void billNew()
        {
            string strSup = docPSSID.SelectedValue;
            string strDept = docDEPTID.SelectedValue;
            //原单据保存判断
            PubFunc.FormDataClear(FormDoc);
            if (PubFunc.StrIsEmpty(strDept) && !isDg())
            {
                if (docDEPTID.Items.Count > 2)
                    strDept = docDEPTID.Items[1].Value;
            }
            if (PubFunc.StrIsEmpty(strSup))
            {
                strSup = Doc.DbGetSysPara("SUPPLIER");
            }
            docFLAG.SelectedValue = "M";
            docLRY.SelectedValue = UserAction.UserID;
            docCGY.SelectedValue = UserAction.UserID;
            docLRRQ.SelectedDate = DateTime.Now;
            docDHRQ.SelectedDate = DateTime.Now;
            docDEPTID.SelectedValue = strDept;
            docPSSID.SelectedValue = strSup;
            billLockDoc(false);
            comBZSL.Enabled = true;
            comPH.Enabled = true;
            comMEMO.Enabled = true;
            docMEMO.Enabled = true;
            docINVOICENUMBER.Enabled = true;
            //comGDSEQ.Enabled = true;
            GridCom.SummaryData = null;
            PageContext.RegisterStartupScript(GridCom.GetRejectChangesReference());

            btnDel.Enabled = false;
            btnSave.Enabled = true;
            btnCommit.Enabled = false;
            btnAudit.Enabled = false;
            btnCancel.Enabled = false;
            btnPrint.Enabled = false;
            btnPrint.Enabled = false;
            btnDelRow.Enabled = true;
            btnGoods.Enabled = true;
            btnHC.Enabled = false;
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
                int num = int.Parse(row["PRINTNUM"].ToString());
                FineUIPro.BoundField flagcol1 = GridList.FindColumn("PRINTNUM") as FineUIPro.BoundField;
                if (num > 0)
                {
                    e.CellCssClasses[flagcol1.ColumnIndex] = "printcolor";
                }
            }
        }
        protected override void billDel()
        {
            if (docBILLNO.Text.Trim() == "")
            {
                Alert.Show("请选择需要删除的单据", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (docFLAG.SelectedValue != "M" && docFLAG.SelectedValue != "R")
            {
                Alert.Show("非『新增』单据不能删除！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!CheckFlag("M") && !CheckFlag("R"))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                return;
            }

            if (ISYPT && !docBILLNO.Text.Contains(BillType) && docBILLNO.Text.Length > 0)
            {
                Alert.Show("下传单据不能删除！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            List<string> listSql = new List<string>();
            listSql.Add("Delete from DAT_RK_DOC t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            listSql.Add("Delete from DAT_RK_COM t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            listSql.Add("UPDATE DAT_DO_LIST SET FLAG = 'Y' WHERE PARA='" + docBILLNO.Text.Trim() + "'");
            if (DbHelperOra.ExecuteSqlTran(listSql))
            {
                Alert.Show("单据删除成功!", "消息提示", MessageBoxIcon.Information);
                billNew();
                billSearch();
                OperLog("商品入库", "删除单据【" + docBILLNO.Text + "】");
            }
            else
            {
                Alert.Show("单据删除失败!", "错误提示", MessageBoxIcon.Information);
            }
        }
        private bool SaveSuccess = false;
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
            SaveSuccess = false;
            save("Y");
            if (SaveSuccess == false)
            {
                return;
            }
            SaveSuccess = false;
            sqlList.Add(new CommandInfo("update DAT_RK_DOC set flag='N' where BILLNO='" + strBILLNO + "' ", null));
            //增加待办事宜
            sqlList.Add(Doc.GETDOADD("DO_8", docDEPTID.SelectedValue, docLRY.SelectedValue, docBILLNO.Text));

            if (DbHelperOra.ExecuteSqlTran(sqlList))
            {
                Alert.Show("此单据提交成功！");
                billSearch();
                billOpen(docBILLNO.Text);
                billLockDoc(true);
                OperLog("商品入库", "提交单据【" + docBILLNO.Text + "】");
            }
            else
            {
                Alert.Show("此单据提交失败，请联系系统管理人员！");
            }

        }
        protected void btnAllCommit_Click(object sender, EventArgs e)
        {
            int[] selections = GridList.SelectedRowIndexArray;
            if (GridList.SelectedRowIndexArray.Length <= 0)
            {
                Alert.Show("请选择要审核的单据信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            List<CommandInfo> sqlList = new List<CommandInfo>();
            string succeed = string.Empty;
            for (int i = 0; i < selections.Length; i++)
            {
                int rowIndex = GridList.SelectedRowIndexArray[i];
                //Alert.Show(GridList.DataKeys[rowIndex][1].ToString());
                if (GridList.DataKeys[rowIndex][1].ToString() == "M")
                {
                    string strBILLNO = GridList.DataKeys[rowIndex][0].ToString();
                    sqlList.Add(new CommandInfo("update DAT_RK_DOC set flag='N' where BILLNO='" + strBILLNO + "' ", null));
                    //增加待办事宜
                    //sqlList.Add(Doc.GETDOADD("DO_8", GridList.DataKeys[rowIndex][2].ToString(), GridList.DataKeys[rowIndex][3].ToString(), strBILLNO));
                    if (DbHelperOra.ExecuteSqlTran(sqlList))
                    {
                        succeed = succeed + "【" + strBILLNO + "】";
                    }
                }
            }
            if (succeed.Length > 0)
            {
                Alert.Show("单据" + succeed + "批量提交成功！", "消息提示", MessageBoxIcon.Warning);
                billSearch();
                OperLog("商品入库", "提交单据" + succeed + "");
            }
            else
            {
                Alert.Show("你选中的单据均为『非新增』单据，不能进行提交操作", "消息提示", MessageBoxIcon.Warning);
            }

        }
        protected override void billAddRow()
        {
            if (docFLAG.SelectedValue != "M")
            {
                Alert.Show("非『新增』单据不能增行！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
            billLockDoc(true);
            PubFunc.GridRowAdd(GridCom, "INIT");
        }
        protected override void billDelRow()
        {
            if (docFLAG.SelectedValue != "M" && docFLAG.SelectedValue != "R")
            {
                Alert.Show("非『新增』单据不能删行！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (GridCom.SelectedCell == null)
            {
                Alert.Show("空单不能删行", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (ISYPT && !docBILLNO.Text.Contains(BillType) && docBILLNO.Text.Length > 0)
            {
                Alert.Show("下传单据不能删除！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!CheckFlag("M") && !CheckFlag("R"))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(GridCom.SelectedRowID))
            {
                Alert.Show("没有选中任何行，无法进行【删行】操作！", MessageBoxIcon.Warning);
                return;
            }
            GridCom.DeleteSelectedRows();
            JArray megData = GridCom.GetMergedData();
            //ymh 删除合计未更新到前台，暂时排除需要合计的数据
            //UpdateSum(newDict);
            decimal bzslTotal = 0, feeTotal = 0;
            foreach (JObject dic in megData)
            {
                if (dic["id"].ToString() != GridCom.SelectedRowID)
                {
                    bzslTotal += Convert.ToDecimal(dic["values"]["BZSL"]);
                    feeTotal += Convert.ToDecimal(dic["values"]["HSJJ"]) * Convert.ToDecimal(dic["values"]["BZSL"]);
                }
            }
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("BZSL", bzslTotal.ToString());
            summary.Add("HSJE", feeTotal.ToString("F2"));
            GridCom.SummaryData = summary;
        }
        protected string getalreadyexistgdseq(string dhdbillno)
        {
            string alreadyexistgdseqbeg = "商品【";
            string alreadyexistgdseqend = "】之前已入库数量为 ";
            string body = "";
            object checknvl = "";
            bool check = false;
            DataTable dt = DbHelperOra.QueryForTable("select ddc.gdseq from  dat_dd_doc ddd, dat_dd_com  ddc where ddd.billno=ddc.seqno(+) and ddd.billno='" + dhdbillno + "'");
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    checknvl = DbHelperOra.GetSingle(string.Format(@"select sum(nvl(dhsl,0)) from dat_dd_doc ddd,dat_dd_com ddc where ddd.billno=ddc.seqno(+) and ddd.seqno='{0}' and ddc.gdseq='{1}' ", dhdbillno, dr[0].ToString()));
                    if (checknvl != null)
                    {
                        body += alreadyexistgdseqbeg + checknvl + alreadyexistgdseqend;
                        check = true;
                    }

                }
            }
            if (check)
            {
                return body;
            }
            else
            {
                return "";
            }

        }
        protected bool save(String flag = "N")
        {
            #region 数据有效性验证
            if (("MR").IndexOf(docFLAG.SelectedValue) < 0 && hfdOper.Text == "input")
            {
                Alert.Show("非新单不能保存！", "消息提示", MessageBoxIcon.Warning);
                return false;
            }
            if ((!CheckFlag("M") && !CheckFlag("R")) && hfdOper.Text == "input")
            {
                Alert.Show("此单据已经被别人操作，请等待操作!");
                return false;
            }
            List<Dictionary<string, object>> goodsData = GridCom.GetNewAddedList();
            if (goodsData.Count == 0)
            {
                Alert.Show("请输入商品信息", "消息提示", MessageBoxIcon.Warning);
                return false;
            }

            if (PubFunc.FormDataCheck(FormDoc).Length > 0) return false;
            if (docDHRQ.SelectedDate == null)
            {
                Alert.Show("收货日期输入错误!", MessageBoxIcon.Warning);
                return false;
            }
            string type = DbHelperOra.GetSingle(string.Format("select TYPE from SYS_DEPT where CODE='{0}'", docDEPTID.SelectedValue)).ToString();
            List<Dictionary<string, object>> newDict = new List<Dictionary<string, object>>();
            String msErr = "";
            //判断是否有空行、批号填写是否符合要求
            for (int i = 0; i < goodsData.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(goodsData[i]["GDSEQ"].ToString()) && !string.IsNullOrWhiteSpace(goodsData[i]["GDNAME"].ToString()))
                {
                    if ((goodsData[i]["BZSL"] ?? "").ToString() == "" || (goodsData[i]["BZSL"] ?? "").ToString() == "0")
                    {
                        Alert.Show("请填写商品[" + goodsData[i]["GDSEQ"] + "]入库数！", "消息提示", MessageBoxIcon.Warning);
                        return false;
                    }
                    //lvj 20160818 将批号，效期，生产日期校验挪到提交操作中
                    if (goodsData[i]["ISLOT"].ToString() == "1" || goodsData[i]["ISLOT"].ToString() == "2")
                    {
                        if (string.IsNullOrWhiteSpace((string)goodsData[i]["PH"]))
                        {
                            Alert.Show("第[" + (i + 1) + "]行商品【" + goodsData[i]["GDNAME"].ToString() + "】批号不能为空！！！", "消息提示", MessageBoxIcon.Warning);
                            return false;
                        }

                        try
                        {

                            if (string.IsNullOrWhiteSpace((goodsData[i]["RQ_SC"] ?? "").ToString()) || string.IsNullOrWhiteSpace((goodsData[i]["YXQZ"] ?? "").ToString()))
                            {
                                Alert.Show("第[" + (i + 1) + "]行商品【" + goodsData[i]["GDNAME"].ToString() + "】生产日期/有效期至不能为空！！！", "消息提示", MessageBoxIcon.Warning);
                                return false;
                            }

                        }
                        catch
                        {
                            Alert.Show("第[" + (i + 1) + "]行商品【" + goodsData[i]["GDNAME"].ToString() + "】生产日期/有效期至不能为空！！！", "消息提示", MessageBoxIcon.Warning);
                            return false;
                        }
                        try
                        {
                            if (!string.IsNullOrWhiteSpace((goodsData[i]["RQ_SC"] ?? "").ToString()) && !string.IsNullOrWhiteSpace((goodsData[i]["YXQZ"] ?? "").ToString()))
                            {
                                DateTime t1, t2 = DateTime.Now;
                                if (!DateTime.TryParse(goodsData[i]["RQ_SC"].ToString(), out t1) || !DateTime.TryParse(goodsData[i]["YXQZ"].ToString(), out t2))
                                {
                                    Alert.Show("第[" + (i + 1) + "]行商品【" + goodsData[i]["GDNAME"].ToString() + "】生产日期/有效期至输入错误", "提示信息", MessageBoxIcon.Warning);
                                    return false;
                                }
                                DateTime td = DateTime.Now;
                                if (DateTime.Compare(t1, t2) >= 0 || DateTime.Compare(t2, td) <= 0|| DateTime.Compare(t1, td) > 0)
                                {
                                    Alert.Show("第[" + (i + 1) + "]行商品【" + goodsData[i]["GDNAME"].ToString() + "】生产日期/有效期至存在问题！！！", "消息提示", MessageBoxIcon.Warning);
                                    return false;
                                }
                            }
                            for (int j = 0; j < goodsData.Count; j++)
                            {
                                if (goodsData[j]["GDSEQ"].ToString().Equals(goodsData[i]["GDSEQ"].ToString()) && goodsData[j]["PH"].ToString().Equals(goodsData[i]["PH"].ToString()))
                                {
                                    if (!goodsData[j]["RQ_SC"].ToString().Equals(goodsData[i]["RQ_SC"].ToString()) || !goodsData[j]["YXQZ"].ToString().Equals(goodsData[i]["YXQZ"].ToString()))
                                    {
                                        string ss = goodsData[j]["RQ_SC"].ToString();
                                        string mm = goodsData[i]["RQ_SC"].ToString();
                                        string tt = goodsData[j]["YXQZ"].ToString();
                                        string me = goodsData[i]["YXQZ"].ToString();
                                        Alert.Show("第[" + (j + 1) + "]行商品【" + goodsData[i]["GDNAME"].ToString() + "】生产日期/有效期至与之前相同批号的商品不符！", "提示信息", MessageBoxIcon.Warning);
                                        return false;
                                    }
                                }
                            }
                        }
                        catch
                        {
                            Alert.Show("第[" + (i + 1) + "]行商品【" + goodsData[i]["GDNAME"].ToString() + "】生产日期/有效期至输入错误", "提示信息", MessageBoxIcon.Warning);
                            return false;
                        }


                    }
                    //modify  zhanghaicheng 2016-10-14 判断如果批号不为空， 验证数据表中该批号对应的效期和生产日期是否相等，如果不相等提示

                    if (!string.IsNullOrEmpty(goodsData[i]["PH"].ToString()) && DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_GOODSSTOCK WHERE  PH='{0}'", goodsData[i]["PH"].ToString())))
                    {

                        if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_GOODSSTOCK WHERE  PH='{0}' AND (RQ_SC <> TO_DATE('{1}','YYYY-MM-DD') OR YXQZ <> TO_DATE('{2}','YYYY-MM-DD'))  AND GDSEQ='{3}'  ", goodsData[i]["PH"].ToString(), (goodsData[i]["RQ_SC"] ?? "").ToString(), (goodsData[i]["YXQZ"] ?? "").ToString(), goodsData[i]["GDSEQ"].ToString())))
                        {
                            Alert.Show("第[" + (i + 1) + "]行商品【" + goodsData[i]["GDNAME"].ToString() + "】 该批号的对应的生产日期/有效期至输入有误", "提示信息", MessageBoxIcon.Warning);
                            return false;
                        }
                    }

                    //if ((goodsData[i]["HSJJ"] ?? "").ToString() == "" || (goodsData[i]["HSJJ"] ?? "").ToString() == "0" || Convert.ToDecimal(goodsData[i]["HSJJ"]) == 0)
                    //{
                    //    Alert.Show("商品[" + goodsData[i]["GDSEQ"] + "]的【含税进价】不能为零,请维护好商品的含税进价，再次保存！", "消息提示", MessageBoxIcon.Warning);
                    //    return false;
                    //}

                    //当入库部门为1药库、2药房时
                    if ((",1,2").IndexOf(type) > 0 && string.IsNullOrWhiteSpace(goodsData[i]["HWID"].ToString()))
                    {
                        Alert.Show("第[" + (i + 1) + "]行商品【" + goodsData[i]["GDNAME"].ToString() + "】货位不能为空！！！", "消息提示", MessageBoxIcon.Warning);
                        return false;
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(goodsData[i]["HWID"].ToString()))
                        {
                            goodsData[i]["HWID"] = docDEPTID.SelectedValue;
                        }
                    }
                    if (flag == "N")
                    {
                        if (Convert.ToInt32(goodsData[i]["YRKSL"] ?? "0") + Convert.ToInt32(goodsData[i]["BZSL"]) > Convert.ToInt32(goodsData[i]["DDSL"]))
                        {
                            msErr += "商品【" + goodsData[i]["GDNAME"] + "】；";
                        }
                    }
                    newDict.Add(goodsData[i]);
                }
            }

            if (newDict.Count == 0)//所有Gird行都为空行时
            {
                Alert.Show("商品信息不能为空", "消息提示", MessageBoxIcon.Warning);
                return false;
            }
            if (msErr.Length > 1)
            {
                PageContext.RegisterStartupScript(Confirm.GetShowReference(msErr,
                    "入库数量大于订货数量，是否继续？", MessageBoxIcon.Information, PageManager1.GetCustomEventReference(true, "Confirm_Save", false, false),
                    null));
                return false;
            }
            else
            {
                Window1.Hidden = true;
            }
            #endregion

            if (PubFunc.StrIsEmpty(docBILLNO.Text))
            {
                //获取单据编号
                docSEQNO.Text = BillSeqGet();
                docBILLNO.Text = docSEQNO.Text;
                docBILLNO.Enabled = false;
            }
            else
            {
                string flg = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M') FROM DAT_RK_DOC WHERE SEQNO='{0}'", docBILLNO.Text));
                if (!PubFunc.StrIsEmpty(flg) && (",M,R").IndexOf(flg) < 0 && hfdOper.Text == "input")
                {
                    Alert.Show("您输入的单据号存在重复信息，请重新输入或置空！", "消息提示", MessageBoxIcon.Warning);
                    return false;
                }
                else
                {
                    docSEQNO.Text = docBILLNO.Text;
                    docBILLNO.Enabled = false;
                }
            }
            if (docBILLNO.Text.Length > 15)
            {
                Alert.Show("单据编号输入超长,请检查!");
                
                return false;
            }
            if(!string.IsNullOrEmpty(docDDBH.Text))
            {
                if (!DbHelperOra.Exists("select 1 from dat_dd_doc where billno='" + docDDBH.Text + "'"))
                {
                    Alert.Show("查询不到此订单号，请检查！");
                    billNew();
                    return false;
                }
            }
            

            MyTable mtType = new MyTable("DAT_RK_DOC");
            mtType.ColRow = PubFunc.FormDataHT(FormDoc);
            mtType.ColRow["SEQNO"] = docBILLNO.Text;
            mtType.ColRow["FLAG"] = "M";
            if (hfdOper.Text == "audit")
            {
                mtType.ColRow["FLAG"] = "N";
            }
            if (docMEMO.Text == "订单转入库单-GZ")
            {
                mtType.ColRow["MEMO"] = "订单转入库单";
            }




            mtType.ColRow.Add("BILLTYPE", BillType);
            mtType.ColRow.Add("SUBNUM", newDict.Count);
            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtTypeMx = new MyTable("DAT_RK_COM");
            //取消高值写入逻辑
            //MyTable mtTypeExt = new MyTable("DAT_RK_EXT");
            MyTable mtTypePh = new MyTable("DOC_GOODSPH");
            decimal subNum = 0;//总金额
            //先删除单据信息在插入
            cmdList.Add(new CommandInfo("delete DAT_RK_DOC where seqno='" + docBILLNO.Text + "'", null));//删除单据台头
            cmdList.Add(new CommandInfo("delete DAT_RK_COM where seqno='" + docBILLNO.Text + "'", null));//删除单据明细
            for (int i = 0; i < newDict.Count; i++)
            {
                mtTypeMx.ColRow = PubFunc.GridDataGet(newDict[i]);

                mtTypeMx.ColRow.Add("SEQNO", docBILLNO.Text);
                mtTypeMx.ColRow["ROWNO"] = i + 1;
                mtTypeMx.ColRow.Add("PHID", mtTypeMx.ColRow["PH"]);
                mtTypeMx.ColRow.Add("BHSJJ", 0);
                mtTypeMx.ColRow.Add("BHSJE", 0);
                if (mtTypeMx.ColRow["RQ_SC"] == null || string.IsNullOrWhiteSpace(mtTypeMx.ColRow["RQ_SC"].ToString()))
                {
                    mtTypeMx.ColRow.Remove("RQ_SC");
                }
                if (mtTypeMx.ColRow["YXQZ"] == null || string.IsNullOrWhiteSpace(mtTypeMx.ColRow["YXQZ"].ToString()))
                {
                    mtTypeMx.ColRow.Remove("YXQZ");
                }
                mtTypeMx.ColRow["SSSL"] = decimal.Parse(mtTypeMx.ColRow["BZHL"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                mtTypeMx.ColRow["HSJE"] = (decimal.Parse(mtTypeMx.ColRow["HSJJ"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString()));
                subNum = subNum + decimal.Parse(mtTypeMx.ColRow["HSJE"].ToString());
                cmdList.Add(mtTypeMx.Insert());
            }
            mtType.ColRow.Add("SUBSUM", subNum);
            if (PubFunc.StrIsEmpty(mtType.ColRow["NUM1"].ToString()))
            {
                mtType.ColRow.Remove("NUM1");
            }
            cmdList.AddRange(mtType.InsertCommand());

            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                if (flag == "N")
                {

                    Alert.Show("单据【" + docBILLNO.Text + "】操作成功！");
                }
                OperLog("商品入库", "修改单据【" + docBILLNO.Text + "】");
                billOpen(docBILLNO.Text);
                SaveSuccess = true;
                return true;
            }
            else
            {
                Alert.Show("商品入库数据保存失败！", "错误提示", MessageBoxIcon.Error);
                return false;
            }
        }
        protected override void billSave()
        {
            save();
        }
        protected override void billAudit()
        {
            if (Doc.DbGetSysPara("LOCKSTOCK") == "Y")
            {
                Alert.Show("系统库存已被锁定，请等待盘点处理完毕再做审核处理！", "消息提醒", MessageBoxIcon.Warning);
                return;
            }
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("非【已提交】单据，不能审核！", "警告提示", MessageBoxIcon.Warning);
                return;
            }
            if (DbHelperOra.Exists("SELECT 1 FROM DAT_PD_LOCK WHERE DEPTID ='" + docDEPTID.SelectedValue + "' AND FLAG='N'"))
            {
                Alert.Show("库房正在盘点,请检查!", "警告提示", MessageBoxIcon.Warning);
                return;
            }
            string strBillno = docSEQNO.Text;
            if (!Doc.getFlag(strBillno, "N", BillType))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                return;
            }
            //增加控制同一商品同一批次只能一条记录
            // Object obj = DbHelperOra.GetSingle(String.Format("SELECT * FROM (SELECT GDSEQ FROM DAT_RK_COM WHERE SEQNO = '{0}' GROUP BY GDSEQ,PH HAVING COUNT(1) > 1)", strBillno));
            // if ((obj ?? "").ToString().Length > 0)
            // {
            // Alert.Show(string.Format("入库商品【{0}】同一批号，两条入库信息！", obj), "警告提示", MessageBoxIcon.Warning);
            // return;
            // }
            if (string.IsNullOrWhiteSpace(docDDBH.Text) || !DbHelperOra.Exists("SELECT 1 FROM DAT_USE_DET WHERE ORDBILLNO = '" + docDDBH.Text + "' AND FLAG = 'Y'"))
            {
                string strSql = @"SELECT 1 FROM DAT_RK_EXT A, DOC_GOODS B
                                     WHERE A.GDSEQ = B.GDSEQ AND A.BILLNO = '{0}' AND A.FLAG = 'N'
                                      UNION
                                     SELECT 1 FROM DAT_RK_COM A, DOC_GOODS B
                                     WHERE A.GDSEQ = B.GDSEQ AND A.SEQNO = '{0}'
                                          AND B.ISGZ = 'Y' AND A.GDSEQ NOT IN (SELECT GDSEQ FROM DAT_RK_EXT WHERE BILLNO = '{0}')";
                //判断高值码是否符合入库标准
                if (DbHelperOra.Exists(string.Format(strSql, strBillno)))
                {
                    //Alert.Show("存在未扫描的追溯码,不允许入库！", "提示信息", MessageBoxIcon.Warning);
                    btnScan_Click(null, null);
                    return;
                }
            }
            if (!save("Y"))
            {
                return;
            }
            try
            {
                BillOper(strBillno, "AUDIT");
                billLockDoc(true);
                billOpen(strBillno);
                string msg = "";
                if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_RK_COM A,DOC_GOODS B WHERE A.GDSEQ = B.GDSEQ AND B.ISFLAG3 = 'Y' AND A.SEQNO = '{0}'", strBillno)))
                {
                    msg = "单据【" + strBillno + "】审核成功，该单据中有直送商品，已自动生成出库单！";
                }
                else
                {
                    //msg = "单据【" + strBillno + "】审核成功！";
                }
                OperLog("商品入库", "审核单据【" + strBillno + "】");
                String DEPTDH = "";
                DEPTDH = (DbHelperOra.GetSingle(string.Format("SELECT B.DEPTDH FROM DAT_DD_DOC B WHERE B.SEQNO=(select DDBH from DAT_RK_DOC A WHERE A.SEQNO='{0}' AND A.FLAG='Y' ) AND B.DEPTDH<>'{1}'", strBillno, docDEPTID.SelectedValue)) ?? "").ToString();
                if (DEPTDH != null && DEPTDH != "")
                {
                    winCk.Hidden = false;
                    ddlDEPTCK.SelectedValue = DEPTDH;
                }
            }
            catch (Exception ex)
            {
                Alert.Show(Error_Parse(ex.Message));
                return;
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
        protected override void billCancel()
        {
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("已提交单据才能驳回！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string strBillno = docSEQNO.Text;
            if (!Doc.getFlag(strBillno, "N", BillType))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                return;
            }
            if (ISYPT && !docBILLNO.Text.Contains(BillType) && docBILLNO.Text.Length > 0)
            {
                Alert.Show("下传单据不能驳回！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            //List<Dictionary<string, object>> newDict = GridCom.GetNewAddedList();
            //int count = newDict.Where(a => a["MEMO"].ToString().Contains("FROM_PLATFORM")).Count();
            //if (count > 0)
            //{
            //    Alert.Show("接口下传的单据不能驳回！", "消息提示", MessageBoxIcon.Warning);
            //    return;
            //}
            WindowReject.Hidden = false;
        }


        #region 该内容由于业务变动废弃掉了 alei 2015年5月10日
        /// <summary>
        /// 该方法废弃 alei 2015年5月10日
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void comGDSEQ_TriggerClick(object sender, EventArgs e)
        {
            string code = labGDSEQ.Text;
            string dept = docDEPTID.SelectedValue;
            string sup = docPSSID.SelectedValue;

            if (!string.IsNullOrWhiteSpace(code))
            {
                DataTable dt_goods = Doc.GetGoods(code, sup, dept);
                if (dt_goods != null && dt_goods.Rows.Count > 0)
                {
                    //添加缺失数据项
                    dt_goods.Columns.Add("BZSL", Type.GetType("System.Int32"));
                    dt_goods.Columns.Add("DDSL", Type.GetType("System.Int32"));
                    dt_goods.Columns.Add("SSSL", Type.GetType("System.Int32"));
                    dt_goods.Columns.Add("HSJE", Type.GetType("System.Double"));
                    DataRow dr_goods = dt_goods.Rows[0];
                    dr_goods["BZSL"] = "0";
                    dr_goods["DDSL"] = "0";
                    dr_goods["SSSL"] = "0";
                    dr_goods["HSJE"] = "0.00";

                    DataTable dtPH = Doc.GetGoodsPHList(dr_goods["GDSEQ"].ToString());
                    if (dtPH != null && dtPH.Rows.Count > 0)
                    {
                        if (dtPH.Rows.Count == 1)
                        {
                            dr_goods["PH"] = dtPH.Rows[0]["PH"];
                            //dr_goods["PZWH"] = dtPH.Rows[0]["PZWH"];
                            dr_goods["RQ_SC"] = dtPH.Rows[0]["RQ_SC"];
                            dr_goods["YXQZ"] = dtPH.Rows[0]["YXQZ"];
                        }
                        else
                        {
                            hfdRowIndex.Text = GridCom.SelectedRowIndex.ToString();
                            GridLot.DataSource = dtPH;
                            GridLot.DataBind();
                            WindowLot.Hidden = false;
                        }
                    }
                    else
                    {
                        Alert.Show("请先维护商品批号！", MessageBoxIcon.Warning);
                    }
                    LoadGridRow(dr_goods);
                }
                else
                {
                    Alert.Show(string.Format("{0}尚未配置商品【{1}】！！！", docDEPTID.SelectedText, code), MessageBoxIcon.Warning);
                    PubFunc.GridRowAdd(GridCom, "CLEAR");
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
            //if (flag == "NEW")
            //{
            //    if (!string.IsNullOrWhiteSpace(row["UNIT_ORDER"].ToString()))
            //    {
            //        if (row["UNIT_ORDER"].ToString() == "D")//订货单位为大包装时
            //        {
            //            if (!string.IsNullOrWhiteSpace(row["NUM_DABZ"].ToString()) && row["NUM_DABZ"].ToString() != "0")
            //            {
            //                row["UNIT"] = row["UNIT_DABZ"];
            //                row["UNITNAME"] = row["UNIT_DABZ_NAME"];
            //                row["BZHL"] = row["NUM_DABZ"];
            //                int price = 0, number = 0;
            //                int.TryParse(row["HSJJ"].ToString(), out price);
            //                int.TryParse(row["NUM_DABZ"].ToString(), out number);
            //                row["HSJJ"] = price * number;
            //            }
            //        }
            //        else if (row["UNIT_ORDER"].ToString() == "Z")//订货单位为中包装时
            //        {
            //            if (!string.IsNullOrWhiteSpace(row["NUM_ZHONGBZ"].ToString()) && row["NUM_ZHONGBZ"].ToString() != "0")
            //            {
            //                row["UNIT"] = row["UNIT_ZHONGBZ"];
            //                row["UNITNAME"] = row["UNIT_ZHONGBZ_NAME"];
            //                row["BZHL"] = row["NUM_ZHONGBZ"];
            //                int price = 0, number = 0;
            //                int.TryParse(row["HSJJ"].ToString(), out price);
            //                int.TryParse(row["NUM_ZHONGBZ"].ToString(), out number);
            //                row["HSJJ"] = price * number;
            //            }
            //        }
            //    }
            //}

            PubFunc.GridRowAdd(GridCom, row, firstRow);
        }
        #endregion
        protected void Window1_Close(object sender, WindowCloseEventArgs e)
        {
            string BZHLerror = "";
            DataTable dt = GetGoods(hfdValue.Text);
            dt.Columns.Remove(dt.Columns["BZHL"]);
            dt.Columns.Remove(dt.Columns["UNIT"]);
            DataTable dtres = new DataTable();

            if (dt != null && dt.Rows.Count > 0)
            {
                //dt.Columns["UNITNAME"].ColumnName = "UNITSMALLNAME";
                //dt.Columns["BZHL_SELL"].ColumnName = "BZHL";
                //dt.Columns["UNIT_SELL_NAME"].ColumnName = "UNITNAME";
                dt.Columns["UNITNAME"].ColumnName = "UNITSMALLNAME";
                dt.Columns["BZHL_ORDER"].ColumnName = "BZHL";
                dt.Columns["UNIT_ORDER_NAME"].ColumnName = "UNITNAME";
                dt.Columns["UNIT_ORDER"].ColumnName = "UNIT";

                dt.Columns.Add("PH", Type.GetType("System.String"));
                dt.Columns.Add("RQ_SC", Type.GetType("System.String"));
                dt.Columns.Add("YXQZ", Type.GetType("System.String"));
                dt.Columns.Add("MEMO", Type.GetType("System.String"));
                dt.Columns.Add("BZSL", Type.GetType("System.Int32"));
                dt.Columns.Add("DDSL", Type.GetType("System.Int32"));
                dt.Columns.Add("SSSL", Type.GetType("System.Int32"));
                dt.Columns.Add("HSJE", Type.GetType("System.Double"));
                dt.Columns.Add("YRKSL", Type.GetType("System.Int32"));
                dtres = dt.Clone();
                foreach (DataRow row in dt.Rows)
                {
                    if ((row["BZHL"] ?? "").ToString() == "")
                    {

                        BZHLerror += "【" + row["GDSEQ"] + "】";
                        continue;
                    }

                    row["YRKSL"] = "0";
                    row["DDSL"] = "0";
                    row["SSSL"] = "0";
                    row["HSJE"] = "0.00";
                    row["RQ_SC"] = null;
                    row["YXQZ"] = "";
                    //LoadGridRow(row, false);
                    //DataTable dtBill = LoadGridData(row, false);
                    //dtres.Rows.Add(row);
                    dtres.ImportRow(row);
                }
                //DataTable dtBill = dt.Copy();
                //PubFunc.GridRowAdd(GridCom, dtBill);
                DataTable dtBill = LoadGridData(dtres);
                string msg = "";
                if (dtBill != null && dtBill.Rows.Count > 0)
                {
                    //string someDjbh = string.Empty;
                    //bool getDjbh = false;
                    foreach (DataRow row in dt.Rows)
                    {
                        row["BZSL"] = "0";
                        row["HSJE"] = "0";
                        //row["HSJJ"].ToString();
                        //if (string.IsNullOrWhiteSpace(row["HSJJ"].ToString()) || row["HSJJ"].ToString() == "0")
                        //{
                        //    msg += "【" + row["GDSEQ"].ToString() + " | " + row["GDNAME"].ToString() + "】,";
                        //    Alert.Show("商品" + msg + "【含税进价】为空，不能进行【商品入库管理】操作。", "消息提示", MessageBoxIcon.Warning);
                        //    continue;
                        //}
                        //LoadGridRow(row, false);
                        //处理金额格式
                        decimal jingdu = 0;
                        decimal bzhl = 0;
                        if (decimal.TryParse(row["HSJJ"].ToString(), out jingdu) && decimal.TryParse(row["BZHL"].ToString(), out bzhl)) { row["HSJJ"] = Math.Round(jingdu * bzhl, 4).ToString("F4"); }
                        if (decimal.TryParse(row["YBJ"].ToString(), out jingdu)) { row["YBJ"] = jingdu.ToString("F4"); }
                        if (decimal.TryParse(row["HSJE"].ToString(), out jingdu)) { row["HSJE"] = Math.Round(jingdu, 2).ToString("F2"); }

                        //List<Dictionary<string, object>> newDict = GridCom.GetNewAddedList().OrderBy(x => x["GDSEQ"]).ToList();
                        //int sameRowCount = newDict.Where(a => a["GDSEQ"].ToString() == row["GDSEQ"].ToString()).Count();
                        //if (sameRowCount > 0)
                        //{
                        //    someDjbh += "【" + row["GDNAME"].ToString() + "】";
                        //    getDjbh = true;
                        //}
                        //else
                        //{
                        //PubFunc.GridRowAdd(GridCom, row, false);
                        //docDEPTID.Enabled = false;
                        //}

                        JObject defaultObj = new JObject();
                        foreach (DataColumn col in row.Table.Columns)
                        {
                            if (col.ColumnName.ToUpper() == "RQ_SC" || col.ColumnName.ToUpper() == "YXQZ")
                            {
                                if (!PubFunc.StrIsEmpty(row[col.ColumnName].ToString()))
                                {
                                    defaultObj.Add(col.ColumnName.ToUpper(), DateTime.Parse(row[col.ColumnName].ToString()));
                                }
                            }
                            else
                            {
                                defaultObj.Add(col.ColumnName.ToUpper(), row[col.ColumnName].ToString());
                            }
                        }

                        PageContext.RegisterStartupScript(GridCom.GetAddNewRecordReference(defaultObj, true));
                    }

                    //PubFunc.GridRowAdd(GridCom, dtBill);
                }

                docBILLNO.Enabled = false;
                docCGY.Enabled = false;
                docDDBH.Enabled = false;
                docPSSID.Enabled = false;
                docDEPTID.Enabled = false;
                if (!(BZHLerror == ""))
                    Alert.Show("商品" + BZHLerror + "包装含量异常，请维护后再选择！");

            }
            else
            {
                Alert.Show("系统传值错误！！！", "消息提示", MessageBoxIcon.Warning);
            }
        }
        private DataTable LoadGridData(DataTable dt)
        {
            DataTable mydt = dt.Copy();
            foreach (DataRow row in mydt.Rows)
            {
                //处理金额格式
                decimal jingdu = 0;
                if (decimal.TryParse(row["HSJJ"].ToString(), out jingdu)) { row["HSJJ"] = jingdu.ToString("F6"); }
                if (decimal.TryParse(row["YBJ"].ToString(), out jingdu)) { row["YBJ"] = jingdu.ToString("F6"); }
                if (decimal.TryParse(row["SSSL"].ToString(), out jingdu)) { row["SSSL"] = jingdu.ToString("F0"); }
                if (decimal.TryParse(row["DDSL"].ToString(), out jingdu)) { row["DDSL"] = jingdu.ToString("F0"); }
                if (decimal.TryParse(row["HSJE"].ToString(), out jingdu)) { row["HSJE"] = jingdu.ToString("F4"); }

                //PubFunc.GridRowAdd(GridGoods, row, firstRow);
            }
            return mydt;
        }
        private JObject GetJObject(Dictionary<string, object> dicRecord)
        {
            JObject defaultObj = new JObject();
            foreach (string key in dicRecord.Keys)
            {
                if (dicRecord[key] == null)
                {
                    defaultObj.Add(key, null);
                }
                else
                {
                    defaultObj.Add(key, dicRecord[key].ToString());
                }
                //defaultObj.Add(key, dicRecord[key].ToString());
            }
            decimal hl = 0, rs = 0, jg = 0;
            decimal.TryParse((dicRecord["BZHL"] ?? "0").ToString(), out hl);
            decimal.TryParse((dicRecord["BZSL"] ?? "0").ToString(), out rs);
            decimal.TryParse((dicRecord["HSJJ"] ?? "0").ToString(), out jg);

            defaultObj.Remove("SSSL");
            defaultObj.Remove("HSJE");
            defaultObj.Add("SSSL", rs * hl);
            defaultObj.Add("HSJE", rs * jg);

            return defaultObj;
        }

        protected void GridCom_AfterEdit(object sender, GridAfterEditEventArgs e)
        {
            List<Dictionary<string, object>> newDict = GridCom.GetNewAddedList();
            if (newDict.Count == 0) return;
            if (e.ColumnID == "BZSL")
            {
                if (!PubFunc.isNumeric(Doc.GetGridInf(GridCom, e.RowID, "BZHL")) || !PubFunc.isNumeric(Doc.GetGridInf(GridCom, e.RowID, "BZSL")) || !PubFunc.isNumeric(Doc.GetGridInf(GridCom, e.RowID, "HSJJ")))
                {
                    Alert.Show("商品信息异常，请详细检查商品信息：包装含量或价格！", "异常信息", MessageBoxIcon.Warning);
                    return;
                }
                //处理返回jobject
                JObject defaultObj = Doc.GetJObject(GridCom, e.RowID);
                decimal hl = 0, rs = 0, jg = 0;

                decimal.TryParse((defaultObj["BZHL"] ?? "0").ToString(), out hl);
                decimal.TryParse((defaultObj["BZSL"] ?? "0").ToString(), out rs);
                decimal.TryParse((defaultObj["HSJJ"] ?? "0").ToString(), out jg);
                defaultObj["SSSL"] = rs * hl;
                defaultObj["HSJE"] = Math.Round(rs * jg, 2).ToString("F2");
                PageContext.RegisterStartupScript(GridCom.GetUpdateCellValueReference(e.RowID, defaultObj));

                #region //计算合计数量
                decimal ddslTotal = 0, bzslTotal = 0, feeTotal = 0;
                foreach (Dictionary<string, object> dic in newDict)
                {
                    ddslTotal += Convert.ToDecimal(dic["BZHL"] ?? "0") * Convert.ToDecimal(dic["BZSL"] ?? "0");
                    bzslTotal += Convert.ToDecimal(dic["BZSL"] ?? "0");
                    feeTotal += (Convert.ToDecimal(dic["HSJJ"] ?? "0") * Convert.ToDecimal(dic["BZSL"] ?? "0"));


                    object objISFLAG5 = DbHelperOra.GetSingle(string.Format("SELECT ISFLAG5 FROM DOC_GOODS WHERE GDSEQ = '{0}'", dic["GDSEQ"]));

                    if (objISFLAG5.ToString() == "N")
                    {
                        string str = Convert.ToString(Convert.ToDecimal(dic["BZSL"] ?? "0"));
                        if (Convert.ToDecimal(dic["BZSL"]) != (int)Convert.ToDecimal(dic["BZSL"]) && Convert.ToDecimal(dic["BZHL"] ?? "0") == 1)
                        {
                            Alert.Show("当前商品不支持申领数为小数，请调整", "消息提示", MessageBoxIcon.Warning);

                        }
                    }
                }
                //计算合计数量

                JObject summary = new JObject();
                summary.Add("GDNAME", "本页合计");
                summary.Add("DDSL", ddslTotal.ToString());
                summary.Add("BZSL", bzslTotal.ToString());
                summary.Add("HSJE", string.Format("{0:F2}", feeTotal));

                GridCom.SummaryData = summary;
                #endregion

            }
        }
        protected void btnClosePostBack_Click(object sender, EventArgs e)
        {
            bool firstRow = true;
            foreach (GridRow row in GridLot.Rows)
            {
                int rowIndex = row.RowIndex;
                System.Web.UI.WebControls.TextBox tbxNumber = (System.Web.UI.WebControls.TextBox)GridLot.Rows[rowIndex].FindControl("tbxNumber");
                if (!string.IsNullOrWhiteSpace(tbxNumber.Text) && tbxNumber.Text != "0")
                {
                    string[] strCell = GridCom.SelectedCell;
                    Dictionary<string, object> newDict = GridCom.GetNewAddedList()[Convert.ToInt32(strCell[0])];
                    newDict["PH"] = row.Values[1];
                    newDict["YXQZ"] = row.Values[2];
                    newDict["PZWH"] = row.Values[4];
                    newDict["RQ_SC"] = row.Values[3];
                    newDict["BZSL"] = tbxNumber.Text;
                    if (firstRow)
                    {
                        firstRow = false;
                        //string cell = string.Format("[{0},{1}]", intCell[0], intCell[1]);
                        //PageContext.RegisterStartupScript(GridCom.GetUpdateCellValueReference(GetJObject(newDict), cell));
                        PageContext.RegisterStartupScript(GridCom.GetUpdateCellValueReference(strCell[0], strCell[1], GetJObject(newDict).ToString()));
                    }
                    else
                    {
                        PageContext.RegisterStartupScript(GridCom.GetAddNewRecordReference(GetJObject(newDict)));
                    }
                }
            }
            WindowLot.Hidden = true;
        }
        protected void btnClose_Click(object sender, EventArgs e)
        {
            Window1.Hidden = true;
        }

        protected void btnRejectSubmit_Click(object sender, EventArgs e)
        {
            if (docFLAG.SelectedValue != "N" || docBILLNO.Text.Length == 0)
            {
                Alert.Show("已提交不能驳回！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (ISYPT && !docBILLNO.Text.Contains(BillType) && docBILLNO.Text.Length > 0)
            {
                Alert.Show("下传单据不能驳回！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

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
            if (strMemo.Length > 100)
            {
                Alert.Show("驳回备注超长！");
                return;

            }
            if (DbHelperOra.ExecuteSql(string.Format("UPDATE DAT_RK_DOC SET FLAG='R',MEMO='{0}' WHERE SEQNO='{1}' AND FLAG='N'", strMemo, docBILLNO.Text)) == 1)
            {
                OperLog("验收入库审核", "驳回单据【" + docBILLNO.Text + "】，" + strMemo);
                WindowReject.Hidden = true;
                billOpen(docBILLNO.Text);
            };
        }

        protected void btnScan_Click(object sender, EventArgs e)
        {
            if (PubFunc.StrIsEmpty(docSEQNO.Text))
            {
                Alert.Show("请保存单据后进行扫描追溯码操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_RK_COM A,DOC_GOODS B WHERE A.GDSEQ = B.GDSEQ AND B.ISGZ = 'Y' AND SEQNO = '{0}'", docSEQNO.Text)))
            {
                Alert.Show("此单据中没有已经保存的高值商品,请检查！", "消息提示", MessageBoxIcon.Warning);
                //Alert.Show("此单据中商品属性异常，请检查！", "消息提示", MessageBoxIcon.Warning); by congwm 16/11/14
                return;
            }
            if (docFLAG.SelectedValue != "N")
            {
                zsmScan.Enabled = false;
                zsmERP.Enabled = false;
                zsmALL.Enabled = false;
                zsmDelete.Enabled = false;
            }
            else
            {
                zsmScan.Enabled = true;
                zsmERP.Enabled = true;
                zsmALL.Enabled = true;
                zsmDelete.Enabled = true;
            }
            //处理EAS下传单据无高值条码
            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_RK_EXT WHERE BILLNO = '{0}'", docSEQNO.Text)))
            {
                OracleParameter[] parameters = new OracleParameter[]
                {
                     new OracleParameter("V_BILL",OracleDbType.Varchar2),
                     new OracleParameter("V_TYPE",OracleDbType.Varchar2),

                };
                parameters[0].Value = docSEQNO.Text;
                parameters[1].Value = "INS";
                DbHelperOra.RunProcedure("P_EASONECODE", parameters);
            }
            WindowScan.Hidden = false;
            ScanSearch("SHOW");
            zsmERP.Focus();
        }
        protected void ScanSearch(string type)
        {
            string sql = "";
            if (type == "SHOW")
            {
                sql = "SELECT A.*,f_getunitname(A.UNIT) UNITNAME,DECODE(A.FLAG,'S','TRUE','Y','TRUE','FALSE') FLAGNAME FROM DAT_RK_EXT A WHERE A.BILLNO = '{0}' ORDER BY A.GDSEQ,A.INSTIME DESC";
            }
            else
            {
                sql = "SELECT A.*,f_getunitname(A.UNIT) UNITNAME,DECODE(A.FLAG,'S','TRUE','Y','TRUE','FALSE') FLAGNAME FROM DAT_RK_EXT A WHERE A.BILLNO = '{0}' ORDER BY A.INSTIME DESC";
            }
            DataTable dtScan = DbHelperOra.Query(string.Format(sql, docSEQNO.Text)).Tables[0];
            //PubFunc.GridRowAdd(GridSacn, dtScan);
            GridSacn.DataSource = dtScan;
            GridSacn.DataBind();
            zsmScan.Text = String.Empty;
            zsmERP.Text = string.Empty;
            zsmScan.Focus();

        }
        protected void btnScanClose_Click(object sender, EventArgs e)
        {
            #region 原逻辑
            //string seq = GridSacn.Rows[0].Values[1].ToString(), oneCode = "";
            //foreach (GridRow row in GridSacn.Rows)
            //{
            //    int rowIndex = row.RowIndex;
            //    seq = GridSacn.Rows[rowIndex].Values[1].ToString();
            //    oneCode += GridSacn.Rows[rowIndex].Values[5].ToString() + ",";
            //    if ((rowIndex + 1) == GridSacn.Rows.Count)
            //    {
            //        int rowNo = 0;
            //        List<Dictionary<string, object>> newDict = GridCom.GetNewAddedList();
            //        foreach (Dictionary<string, object> dic in newDict)
            //        {
            //            if (seq == dic["GDSEQ"].ToString())
            //            {
            //                dic["CODEINFO"] = oneCode.TrimEnd(',');
            //                PageContext.RegisterStartupScript(GridCom.GetDeleteRowReference(rowNo) + GridCom.GetAddNewRecordReference(GetJObject(dic), rowNo));
            //                oneCode = "";
            //                break;
            //            }
            //            rowNo++;
            //        }
            //    }
            //    else
            //    {
            //        if (seq != GridSacn.Rows[rowIndex + 1].Values[1].ToString())
            //        {
            //            int rowNo = 0;
            //            List<Dictionary<string, object>> newDict = GridCom.GetNewAddedList();
            //            foreach (Dictionary<string, object> dic in newDict)
            //            {
            //                if (seq == dic["GDSEQ"].ToString())
            //                {
            //                    dic["CODEINFO"] = oneCode.TrimEnd(',');
            //                    PageContext.RegisterStartupScript(GridCom.GetDeleteRowReference(rowNo) + GridCom.GetAddNewRecordReference(GetJObject(dic), rowNo));
            //                    oneCode = "";
            //                    break;
            //                }
            //                rowNo++;
            //            }
            //            seq = GridSacn.Rows[rowIndex].Values[1].ToString();
            //        }
            //    }
            //}
            //WindowScan.Hidden = true;
            #endregion
        }
        protected void docDDBH_TextChanged(object sender, EventArgs e)
        {
            if (docDDBH.Text.Trim().Length >= 11)
            {
                //对代管和非代管进行判断
                DataTable dtDoc = DbHelperOra.Query(string.Format("SELECT A.PSSID,A.DEPTID,DHFS FROM DAT_DD_DOC A WHERE SEQNO ='{0}' AND A.FLAG IN('Y','G')", docDDBH.Text.Trim())).Tables[0];
                if (dtDoc != null && dtDoc.Rows.Count > 0)
                {
                    if ((dtDoc.Rows[0]["DHFS"] ?? "").ToString() == "2" && !isDg())
                    {
                        Alert.Show("此订单单号为代管单号,请在【代管入库管理】中操作！");
                        return;
                    }
                    else if ((dtDoc.Rows[0]["DHFS"] ?? "").ToString() != "2" && isDg())
                    {
                        Alert.Show("此订单单号为非代管单号,请在【商品入库管理】中操作！");
                        return;
                    }
                    PubFunc.FormDataSet(FormDoc, dtDoc.Rows[0]);
                    PubFunc.FormLock(FormDoc, true, "");
                }
                else
                {
                    Alert.Show("您输入的订单信息不存在，或状态不正确！", "提示信息", MessageBoxIcon.Warning);
                    docDDBH.Text = "";
                    return;
                }

                PageContext.RegisterStartupScript(GridCom.GetRejectChangesReference());
                DataTable dtBill = DbHelperOra.Query(string.Format(@"SELECT A.*, F_GETUNITNAME(A.UNIT) UNITNAME, F_GETHWID('{0}', A.GDSEQ) HWID, DHS SSSL, f_getproducername(A.PRODUCER) PRODUCERNAME,
                                NVL((SELECT SUM(NVL(SSSL,0)) FROM DAT_RK_DOC DDD,DAT_RK_COM DDC WHERE DDD.SEQNO = DDC.SEQNO  AND DDC.GDSEQ=A.GDSEQ AND DDD.DDBH = '{1}' AND DDD.FLAG IN('Y','G')),0) YRKSL,F_GETDEPTNAME(A.DEPTID) DEPTNAME,
                                F_GETUNITNAME(B.UNIT) UNITSMALLNAME FROM DAT_DD_COM A, DOC_GOODS B WHERE A.GDSEQ = B.GDSEQ AND A.SEQNO = '{1}' ORDER BY ROWNO", docDEPTID.SelectedValue, docDDBH.Text.Trim())).Tables[0];
                // AND DDC.NUM1 = A.ROWNO 行号没有回写，特去掉
                if (dtBill != null && dtBill.Rows.Count > 0)
                {
                    dtBill.Columns.Add(new DataColumn("DDSL", typeof(string)));
                    foreach (DataRow row in dtBill.Rows)
                    {
                        row["DDSL"] = row["BZSL"];
                        LoadGridRow(row, false, "OLD");
                    }
                }

                docBILLNO.Text = "";
                docDHRQ.SelectedDate = DateTime.Now;
                docFLAG.SelectedValue = "M";
            }
        }
        protected void btExport_Click(object sender, EventArgs e)
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【录入日期】！");
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                Alert.Show("开始日期大于结束日期，请重新输入！");
                return;
            }

            string strSql = @"SELECT A.BILLNO 单据编号,A.DDBH 订单编号,
                                    F_GETDEPTNAME(A.DEPTID) 入库部门,
                                    A.DHRQ 入库日期,
                                    F_GETUSERNAME(A.CGY) 入库人,
                                    F_GETUSERNAME(A.LRY) 录入人,
                                    A.LRRQ 录入日期,
                                    B.ROWNO 行号,
                                    ' '||B.GDSEQ 商品编码,
                                    B.GDNAME 商品名称,
                                    B.GDSPEC 商品规格,
                                    B.PZWH 注册证号,
                                    F_GETUNITNAME(B.UNIT) 单位,
                                    B.BZHL 包装含量,
                                    B.BZSL 入库包装数,
                                    B.SSSL 入库数量,
                                    B.PH 批号,
                                    B.RQ_SC 生产日期,
                                    B.YXQZ 有效期至,
                                    B.HSJJ 价格
                                FROM DAT_RK_DOC A, DAT_RK_COM B
                                WHERE A.SEQNO = B.SEQNO";
            string strSearch = "";

            if (lstBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND A.BILLNO  LIKE '%{0}%'", lstBILLNO.Text);
            }
            if (lstFLAG.SelectedItem != null && lstFLAG.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.FLAG='{0}'", lstFLAG.SelectedItem.Value);
            }
            if (lstLRY.SelectedItem != null && lstLRY.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.LRY='{0}'", lstLRY.SelectedItem.Value);
            }
            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedItem.Value);
            }
            if (lstSUPID.SelectedItem != null && lstSUPID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.PSSID='{0}'", lstSUPID.SelectedItem.Value);
            }
            if (lstDDBH.Text.Length > 0)
            {
                strSearch += string.Format(" AND A.DDBH LIKE '%{0}%'", lstDDBH.Text);
            }
            if (Request.QueryString["oper"].ToString() == "audit")
            {
                strSearch += " AND A.FLAG<>'M'";
            }
            if (isDg())
            {
                strSearch += " AND (PSSID = '00001' OR PSSID IN (SELECT DISTINCT NVL(A.PSSID, A.SUPID) PSSID FROM DOC_GOODSSUP A WHERE A.TYPE IN ='1'))";
            }
            else
            {
                strSearch += " AND PSSID IN (SELECT DISTINCT NVL(A.PSSID, A.SUPID) PSSID FROM DOC_GOODSSUP A WHERE A.TYPE IN ('0','Z'))";
            }

            strSearch += string.Format(" AND  A.LRRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND  A.LRRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY A.BILLNO DESC,B.ROWNO";

            DataTable dt = DbHelperOra.Query(strSql).Tables[0];
            ExcelHelper.ExportByWeb(dt, string.Format("入库信息"), "入库信息导出_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
        }

        protected void zsmDelete_Click(object sender, EventArgs e)
        {
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("非『已提交』单据不允许操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (GridSacn.SelectedRowIndexArray.Length < 1)
            {
                Alert.Show("请选择您需要删除的数据！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string onecode = (GridSacn.DataKeys[GridSacn.SelectedRowIndexArray[0]][0]).ToString();
            if (DbHelperOra.ExecuteSql(string.Format("UPDATE DAT_RK_EXT SET STR1 = '',FLAG = 'N' WHERE BILLNO = '{0}' AND ONECODE = '{1}'", docSEQNO.Text, onecode)) < 1)
            {
                Alert.Show("删除清除条码失败,请检查！");
                return;
            }
            OperLog("商品入库", "修改单据【" + docBILLNO.Text + "】高值码");
            ScanSearch("");
        }
        protected void zsmERP_TextChanged(object sender, EventArgs e)
        {
            if (Input.Checked && (sender ?? "").ToString() != "ENTER") return;
            if (zsmERP.Text.Length < 28) return;
            string strBillno = docSEQNO.Text;
            string onecode = zsmERP.Text;
            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_RK_EXT WHERE BILLNO = '{0}' AND ONECODE = '{1}'", strBillno, onecode)))
            {
                Alert.Show("您输入的ERP高值条码不存在此单据中，请检查！", "提示信息", MessageBoxIcon.Warning);
                zsmERP.Text = string.Empty;
                zsmERP.Focus();
                return;
            }
            // 背景色
            hdfScan.Text = "";
            if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_RK_EXT A,DOC_GOODS B WHERE A.GDSEQ = B.GDSEQ AND B.ISFLAG6 = 'Y' AND A.BILLNO = '{0}' AND A.ONECODE = '{1}'", docSEQNO.Text, zsmERP.Text)))
            {
                zsmScan.Text = string.Empty;
                zsmScan.Focus();
                ScanSearch("SHOW");
            }
            else
            {
                DbHelperOra.ExecuteSql(string.Format("UPDATE DAT_RK_EXT SET FLAG = 'S' WHERE BILLNO = '{0}' AND ONECODE = '{1}' AND ROWNUM = 1", docSEQNO.Text, zsmERP.Text));
                ScanSearch("SHOW");
                zsmERP.Text = string.Empty;
                zsmERP.Focus();
                closeGZwindow();
            }
        }
        protected void zsmALL_Click(object sender, EventArgs e)
        {
            if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_RK_EXT A,DOC_GOODS B WHERE A.GDSEQ = B.GDSEQ AND B.ISFLAG6 = 'Y' AND A.BILLNO = '{0}'", docSEQNO.Text)))
            {
                Alert.Show("存在必须扫描供应商条码商品，不允许一键入库！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            DbHelperOra.ExecuteSql(string.Format("UPDATE DAT_RK_EXT SET FLAG = 'S' WHERE BILLNO = '{0}'", docSEQNO.Text));
            Alert.Show("追溯码全部入库成功！");
            ScanSearch("SHOW");
        }
        protected void zsmScan_TextChanged(object sender, EventArgs e)
        {
            if (Input.Checked && (sender ?? "").ToString() != "ENTER") return;
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("非『已提交』单据不允许操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (zsmScan.Text.Length < 1) return;
            //String exis = (DbHelperOra.GetSingle(String.Format("SELECT BILLNO FROM DAT_RK_EXT WHERE STR1 = '{0}' OR ONECODE = '{0}'", zsmScan.Text)) ?? "").ToString();
            //if (!PubFunc.StrIsEmpty(exis))
            //{
            //    Alert.Show("您输入的追溯码已被单据【" + exis + "】使用,请检查！", "提示信息", MessageBoxIcon.Warning);
            //    zsmScan.Text = string.Empty;
            //    zsmScan.Focus();
            //    return;
            //}
            string onecode = zsmERP.Text;
            if (onecode.Length < 1)
            {
                if (hdfScan.Text.TrimEnd(',').Length < 1)
                {
                    if (GridSacn.SelectedRowIndexArray.Count() < 1)
                    {
                        Alert.Show("请选择需要扫码的条码！", "提示信息", MessageBoxIcon.Warning);
                        return;
                    }
                    onecode = GridSacn.DataKeys[GridSacn.SelectedRowIndexArray[0]][0].ToString();
                }
                else
                {
                    onecode = GridSacn.DataKeys[Convert.ToInt32(hdfScan.Text.TrimEnd(','))][0].ToString();
                }
            }
            //写入数据库中
            DbHelperOra.ExecuteSql(string.Format("UPDATE DAT_RK_EXT SET STR1 = '{0}',FLAG = 'S' WHERE BILLNO = '{1}' AND ONECODE = '{2}'", zsmScan.Text, docSEQNO.Text, onecode));
            ScanSearch("");
            #region
            //if (docFLAG.SelectedValue != "N")
            //{
            //    Alert.Show("非『新增』单据不允许操作！", "消息提示", MessageBoxIcon.Warning);
            //    return;
            //}
            //if (zsmScan.Text.Length < 28) return;
            //if (zsmScan.Text.Substring(0, 1) != "2")
            //{
            //    Alert.Show("您扫描的条码不是贵重码,请检查！", "提示信息", MessageBoxIcon.Warning);
            //    zsmScan.Text = string.Empty;
            //    zsmScan.Focus();
            //    return;
            //}
            ////增加输入二维码验证
            //if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_DD_EXT WHERE ONECODE = '{0}' AND FLAG ='N'", zsmScan.Text)))
            //{
            //    Alert.Show("您扫描的二维码不存在或已被使用，请检查！", "提示信息", MessageBoxIcon.Warning);
            //    zsmScan.Text = string.Empty;
            //    zsmScan.Focus();
            //    return;
            //}
            //String exis = (DbHelperOra.GetSingle(String.Format("SELECT BILLNO FROM DAT_RK_EXT WHERE ONECODE = '{0}'", zsmScan.Text)) ?? "").ToString();
            //if (!PubFunc.StrIsEmpty(exis))
            //{
            //    Alert.Show("您输入的追溯码已被单据【" + exis + "】使用,请检查！", "提示信息", MessageBoxIcon.Warning);
            //    zsmScan.Text = string.Empty;
            //    zsmScan.Focus();
            //    return;
            //}
            ////写入数据库中
            //DbHelperOra.ExecuteSql(string.Format(@"INSERT INTO DAT_RK_EXT(DEPTID,BILLNO,ROWNO,ONECODE,GDSEQ,GDNAME,BARCODE,UNIT,GDSPEC,DEPTCUR,BZHL,INSTIME)
            //        SELECT '{0}','{1}',NVL((SELECT MAX(ROWNO)+1 FROM DAT_RK_EXT WHERE BILLNO = '{1}'),1),'{2}',GDSEQ,GDNAME,BARCODE,UNIT,GDSPEC,DEPTCUR,(SELECT BZHL FROM DAT_RK_COM WHERE SEQNO = '{1}' AND GDSEQ = A.GDSEQ AND ROWNO = 1),SYSDATE
            //        FROM DAT_DD_EXT A
            //        WHERE A.ONECODE = '{2}'", docDEPTID.SelectedValue, docBILLNO.Text, zsmScan.Text));
            //ScanSearch("");
            #endregion
            zsmERP.Focus();
            closeGZwindow();
        }

        /// <summary>
        /// 更新汇总信息 20150510 liuz  解决带出信息更新汇总信息显示
        /// </summary>
        /// <param name="newDict"></param>
        private void UpdateSum(List<Dictionary<string, object>> newDict)
        {
            //计算合计数量
            decimal bzslTotal = 0, feeTotal = 0;
            foreach (Dictionary<string, object> dic in newDict)
            {
                bzslTotal += Convert.ToDecimal(dic["BZSL"]);
                feeTotal += Convert.ToDecimal(dic["HSJJ"]) * Convert.ToDecimal(dic["BZSL"]);
            }
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("BZSL", bzslTotal.ToString());
            summary.Add("HSJE", feeTotal.ToString("F2"));
            GridCom.SummaryData = summary;
        }

        private bool CheckFlag(string flag)
        {
            if (docBILLNO.Text.Length > 0)
            {
                return Doc.getFlag(docBILLNO.Text, flag, BillType);
            }
            return true;
        }

        protected void GridSacn_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                string onecode = row["ONECODE"].ToString();
                string flag = row["FLAG"].ToString();
                if (onecode == zsmERP.Text)
                {
                    hdfScan.Text += e.RowIndex.ToString() + ",";
                }
                if (flag == "N")
                {
                    CloseWindow++;
                }
            }
        }

        protected void PageManager1_CustomEvent(object sender, CustomEventArgs e)
        {
            switch (e.EventArgument)
            {
                case "Confirm_OK": BillCk(); break;
                case "Confirm_Save": save("Y"); break;
                case "GoodsAdd": Window1_Close(null, null); break;
                case "Search":billSearch();break;
            }
        }

        protected void btnEvaluate_Click(object sender, EventArgs e)
        {
            if (GridList.SelectedRowIndexArray.Length != 1)
            {
                Alert.Show("请选择一条送货单进行评价！！！", "异常提示", MessageBoxIcon.Warning);
                return;
            }
            if (GridList.Rows[GridList.SelectedRowIndex].DataKeys[1].ToString() != "Y")
            {
                Alert.Show("非【已审核】单据，不能进行供应商评价！！！", "异常提示", MessageBoxIcon.Warning);
                return;
            }
            string billNo = GridList.Rows[GridList.SelectedRowIndex].DataKeys[0].ToString();
            string PSSID = GridList.Rows[GridList.SelectedRowIndex].DataKeys[2].ToString();
            string PSSNAME = GridList.Rows[GridList.SelectedRowIndex].DataKeys[3].ToString();
            Window2.Hidden = false;
            Window2.IFrameUrl = "SupEvaluation.aspx?billno=" + billNo + "&PSSID=" + PSSID + "&PSSNAME=" + PSSNAME;
        }
        protected void btnCK_Click(object sender, EventArgs e)
        {
            //入库转出库单
            if (docBILLNO.Text.Length < 1)
            {
                Alert.Show("请选择需要转出库的单据！", MessageBoxIcon.Warning);
                return;
            }
            if (docDDBH.Text.Length > 0)
            {
                String sql = @"SELECT A.DEPTDH
                        FROM DAT_DD_DOC A,SYS_DEPT B
                        WHERE A.DEPTDH = B.CODE AND B.TYPE = '3' AND A.SEQNO = '{0}'";
                Object obj = String.Format(String.Format(sql, docDDBH.Text));
                if ((obj ?? "").ToString().Length > 0)
                {
                    ddlDEPTCK.SelectedValue = obj.ToString();
                }
            }
            winCk.Hidden = false;
        }
        protected void btnCkSure_Click(object sender, EventArgs e)
        {
            if (ddlDEPTCK.SelectedValue == null || ddlDEPTCK.SelectedValue.Length < 1)
            {
                Alert.Show("请选择需要出库的科室！", MessageBoxIcon.Warning);
                return;
            }
            if (!DbHelperOra.Exists(String.Format("SELECT 1 FROM DAT_RK_COM A WHERE A.SEQNO = '{0}' AND EXISTS(SELECT 1 FROM DOC_GOODSCFG B WHERE B.GDSEQ = A.GDSEQ AND B.DEPTID = '{1}')", docBILLNO.Text, ddlDEPTCK.SelectedValue)))
            {
                Alert.Show("单据中所有商品都未配置到科室【" + ddlDEPTCK.SelectedText + "】，请检查!", MessageBoxIcon.Warning);
                return;
            }
            Object obj = DbHelperOra.GetSingle(String.Format(@"SELECT TO_CHAR(wmsys.wm_concat(DISTINCT GDNAME)) BILLSUM
                FROM DAT_RK_COM A
                WHERE A.SEQNO = '{0}' AND NOT EXISTS(SELECT 1 FROM DOC_GOODSCFG B WHERE B.GDSEQ = A.GDSEQ AND B.DEPTID = '{1}')", docBILLNO.Text, ddlDEPTCK.SelectedValue));
            if ((obj ?? "").ToString().Length > 1)
            {
                PageContext.RegisterStartupScript(Confirm.GetShowReference("商品【" + obj + "】未配置到科室，是否继续？",
                        String.Empty, MessageBoxIcon.Information,
                        PageManager1.GetCustomEventReference("Confirm_OK"),
                        PageManager1.GetCustomEventReference("Confirm_Cancel")));
            }
            else
            {
                BillCk();
            }
        }
        protected void BillCk()
        {
            if (ddlDEPTCK.SelectedValue.Length < 1)
            {
                Alert.Show("请选择转出库部门！", MessageBoxIcon.Warning);
                return;
            }
            if (docBILLNO.Text.Length < 1)
            {
                Alert.Show("请选择需要转出库的入库单据！", MessageBoxIcon.Warning);
                return;
            }
            OracleParameter[] parameters = new OracleParameter[]
                                {
                                     new OracleParameter("VI_BILLNO",OracleDbType.Varchar2),
                                     new OracleParameter("VI_DEPTID",OracleDbType.Varchar2),
                                     new OracleParameter("VI_OPERUSER",OracleDbType.Varchar2)
                                };
            parameters[0].Value = docBILLNO.Text;
            parameters[1].Value = ddlDEPTCK.SelectedValue;
            parameters[2].Value = UserAction.UserID;
            try
            {
                DbHelperOra.RunProcedure("STOREDS.P_RKTOCK", parameters);
                billOpen(docBILLNO.Text);
                Alert.Show("转出库生成成功！");
                winCk.Hidden = true;
            }
            catch (Exception e)
            {
                Alert.Show("单据异常，请检查", "提示信息", MessageBoxIcon.Warning);
            }
        }
        private void closeGZwindow()
        {
            //天津医科大学总医院高值 优化建议，条码全部扫描完成后，窗口直接关闭，不需要手动关闭。 阿磊 2016年7月15日 14:57:55
            if (CloseWindow == 0)
            {
                WindowScan.Hidden = true;
            }
            CloseWindow = 0;
        }
        private void TJ_YKGZ()
        {//天津医科大学总医院高值 客户化需求 2016年7月6日 10:37:44 必须扫描追溯码，不允许一键入库。
            //zsmALL.Hidden = true;
        }
        #region PAD 功能调用
        private string getSerilizedData(string sql)
        {
            return JsonConvert.SerializeObject(DbHelperOra.Query(sql).Tables[0]);
        }
        private string listSearchForPad()
        {
            string result = "";
            JObject jo = new JObject();
            string strSearch = "";

            string BILLNO = Request.QueryString.Get("BILLNO");
            string DEPTID = Request.QueryString.Get("DEPTID");
            string DDBH = Request.QueryString.Get("DDBH");
            string SUPID = Request.QueryString.Get("SUPID");
            string FLAG = Request.QueryString.Get("FLAG");
            string LRY = Request.QueryString.Get("LRY");
            string LRRQ1 = Request.QueryString.Get("LRRQ1");
            string LRRQ2 = Request.QueryString.Get("LRRQ2");

            if (string.IsNullOrWhiteSpace(LRRQ1) || string.IsNullOrWhiteSpace(LRRQ2))
            {
                jo.Add("result", "fail");
                jo.Add("data", "请输入条件【使用日期】！");
                return JsonConvert.SerializeObject(jo);
            }
            if (Convert.ToDateTime(LRRQ1) > Convert.ToDateTime(LRRQ2))
            {
                jo.Add("result", "fail");
                jo.Add("data", "开始日期大于结束日期，请重新输入！");
                return JsonConvert.SerializeObject(jo);
            }
            strSql += " WHERE  A.FLAG<>'M' AND  A.DEPTID in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '" + UserAction.UserID + "') = 'Y' ) ";
            if (!string.IsNullOrWhiteSpace(BILLNO))
            {
                strSearch += string.Format(" AND A.BILLNO  LIKE '%{0}%'", BILLNO.Trim());
            }
            if (!string.IsNullOrWhiteSpace(FLAG))
            {
                strSearch += string.Format(" AND A.FLAG='{0}'", FLAG.Trim());
            }

            if (!string.IsNullOrWhiteSpace(LRY))
            {
                strSearch += string.Format(" AND A.LRY='{0}'", LRY.Trim());
            }
            if (!string.IsNullOrWhiteSpace(DEPTID))
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", DEPTID.Trim());
            }
            if (!string.IsNullOrWhiteSpace(SUPID))
            {
                strSearch += string.Format(" AND A.PSSID='{0}'", SUPID.Trim());
            }
            if (!string.IsNullOrWhiteSpace(DDBH))
            {
                strSearch += string.Format(" AND A.DDBH LIKE '%{0}%'", DDBH.Trim());
            }

            strSearch += string.Format(" AND  A.LRRQ>=TO_DATE('{0}','YYYY-MM-DD')", LRRQ1);
            strSearch += string.Format(" AND  A.LRRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", LRRQ2);
            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY A.FLAG,A.DHRQ DESC,A.DDBH DESC,A.BILLNO ASC";
            jo.Add("result", "success");
            jo.Add("data", getSerilizedData(strSql));
            result = JsonConvert.SerializeObject(jo);
            return result;
        }
        private string billSearchForPad()
        {
            string result = "";
            string billno = Request.QueryString.Get("billno");
            JObject jo = new JObject();
            DataTable dtDoc = DbHelperOra.Query(string.Format(strDocSql, billno)).Tables[0];
            jo.Add("result", "success");
            jo.Add("doc", JsonConvert.SerializeObject(dtDoc));
            DataTable dtBill = DbHelperOra.Query(string.Format(strComSql, billno)).Tables[0];
            jo.Add("com", JsonConvert.SerializeObject(dtBill));
            btnScan_Click_forPAD(billno);
            jo.Add("zsm", JsonConvert.SerializeObject(PAD_dtZSM));
            result = JsonConvert.SerializeObject(jo);
            return result;
        }
        protected void btnScan_Click_forPAD(string billno)
        {
            if (PubFunc.StrIsEmpty(billno))
            {
                Alert.Show("请保存单据后进行扫描追溯码操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_RK_COM A,DOC_GOODS B WHERE A.GDSEQ = B.GDSEQ AND B.ISGZ = 'Y' AND SEQNO = '{0}'", billno)))
            {
                //Alert.Show("此单据中没有已经保存的高值商品,请检查！", "消息提示", MessageBoxIcon.Warning);
                Alert.Show("此单据中商品属性有问题，请检查！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (docFLAG.SelectedValue != "N")
            {
                zsmScan.Enabled = false;
                zsmERP.Enabled = false;
                zsmALL.Enabled = false;
                zsmDelete.Enabled = false;
            }
            else
            {
                zsmScan.Enabled = true;
                zsmERP.Enabled = true;
                zsmALL.Enabled = true;
                zsmDelete.Enabled = true;
            }
            //处理EAS下传单据无高值条码
            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_RK_EXT WHERE BILLNO = '{0}'", billno)) && !PubFunc.StrIsEmpty(docDDBH.Text))
            {
                OracleParameter[] parameters = new OracleParameter[]
                {
                     new OracleParameter("V_BILL",OracleDbType.Varchar2),
                     new OracleParameter("V_TYPE",OracleDbType.Varchar2),

                };
                parameters[0].Value = billno;
                parameters[1].Value = "INS";
                DbHelperOra.RunProcedure("P_EASONECODE", parameters);
            }
            WindowScan.Hidden = false;
            ScanSearchForPad("SHOW", billno);
        }
        protected void ScanSearchForPad(string type, string billno)
        {
            string sql = "";
            if (type == "SHOW")
            {
                sql = "SELECT A.*,f_getunitname(A.UNIT) UNITNAME,DECODE(A.FLAG,'S','TRUE','Y','TRUE','FALSE') FLAGNAME FROM DAT_RK_EXT A WHERE A.BILLNO = '{0}' ORDER BY A.GDSEQ,A.INSTIME DESC";
            }
            else
            {
                sql = "SELECT A.*,f_getunitname(A.UNIT) UNITNAME,DECODE(A.FLAG,'S','TRUE','Y','TRUE','FALSE') FLAGNAME FROM DAT_RK_EXT A WHERE A.BILLNO = '{0}' ORDER BY A.INSTIME DESC";
            }
            PAD_dtZSM = DbHelperOra.Query(string.Format(sql, billno)).Tables[0];

        }
        protected string zsmScan_forPAD()
        {
            string billno = Request.QueryString.Get("billno");
            string onecode = Request.QueryString.Get("onecode");
            string result = "";
            JObject jo = new JObject();
            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_RK_EXT WHERE BILLNO = '{0}' AND ONECODE = '{1}'", billno, onecode)))
            {
                jo.Add("result", "fail");
                jo.Add("data", "您输入的ERP高值条码不存在此单据中，请检查！");
                return JsonConvert.SerializeObject(jo);
            }
            if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_RK_EXT A,DOC_GOODS B WHERE A.GDSEQ = B.GDSEQ AND B.ISFLAG6 = 'Y' AND A.BILLNO = '{0}' AND A.ONECODE = '{1}'", billno, onecode)))
            {
                ScanSearchForPad("", billno);
                jo.Add("zsm", JsonConvert.SerializeObject(PAD_dtZSM));
                result = JsonConvert.SerializeObject(jo);
                return result;
            }
            else
            {
                DbHelperOra.ExecuteSql(string.Format("UPDATE DAT_RK_EXT SET FLAG = 'S' WHERE BILLNO = '{0}' AND ONECODE = '{1}' AND ROWNUM = 1", billno, onecode));
                ScanSearchForPad("", billno);
                jo.Add("zsm", JsonConvert.SerializeObject(PAD_dtZSM));
                result = JsonConvert.SerializeObject(jo);
                return result;
            }
        }
        protected string billAuditForPad()
        {
            string billno = Request.QueryString.Get("billno");
            string DEPTID = Request.QueryString.Get("DEPTID");
            string DDBH = Request.QueryString.Get("DDBH");
            JObject jo = new JObject();

            if (!Doc.getFlag(billno, "N", BillType))
            {
                jo.Add("result", "fail");
                jo.Add("data", "非【已提交】单据，不能审核,请检查单据状态！");
                return JsonConvert.SerializeObject(jo);
            }
            if (string.IsNullOrWhiteSpace(docDDBH.Text) || !DbHelperOra.Exists("SELECT 1 FROM DAT_USE_DET WHERE ORDBILLNO = '" + DDBH + "' AND FLAG = 'Y'"))
            {
                string strSql = @"SELECT 1 FROM DAT_RK_EXT A, DOC_GOODS B
                                     WHERE A.GDSEQ = B.GDSEQ AND A.BILLNO = '{0}' AND A.FLAG = 'N'
                                      UNION
                                     SELECT 1 FROM DAT_RK_COM A, DOC_GOODS B
                                     WHERE A.GDSEQ = B.GDSEQ AND A.SEQNO = '{0}'
                                          AND B.ISGZ = 'Y' AND A.GDSEQ NOT IN (SELECT GDSEQ FROM DAT_RK_EXT WHERE BILLNO = '{0}')";
                //判断高值码是否符合入库标准
                if (DbHelperOra.Exists(string.Format(strSql, billno)))
                {
                    jo.Add("result", "fail");
                    jo.Add("data", "存在未扫描的追溯码");
                    return JsonConvert.SerializeObject(jo);
                }
            }
            //save();
            if (BillOper(billno, "AUDIT") == 1)
            {
                string msg = "";
                //if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_RK_COM A,DOC_GOODS B WHERE A.GDSEQ = B.GDSEQ AND B.ISFLAG3 = 'Y' AND A.SEQNO = '{0}'", billno)))
                //{
                //    msg = "单据【" + billno + "】审核成功，该单据中有直送商品，已自动生成出库单！";
                //}
                //else
                //{
                //    msg = "单据【" + billno + "】审核成功！";
                //}
                msg = "单据【" + billno + "】审核成功！";
                OperLog("商品入库", "审核单据【" + billno + "】（平板）");
                String DEPTDH = "";
                DEPTDH = (DbHelperOra.GetSingle(string.Format("SELECT B.DEPTDH FROM DAT_DD_DOC B WHERE B.SEQNO=(select DDBH from DAT_RK_DOC A WHERE A.SEQNO='{0}' AND A.FLAG='Y' ) AND B.DEPTDH<>'{1}'", billno, DEPTID)) ?? "").ToString();
                if (DEPTDH != null && DEPTDH != "")
                {
                    jo.Add("DEPTDH", DEPTDH);
                }
                jo.Add("result", "fail");
                jo.Add("data", msg);
                return JsonConvert.SerializeObject(jo);
            }
            return null;
        }
        protected string billCkSurePad()
        {
            string billno = Request.QueryString.Get("billno");
            string DEPTID = Request.QueryString.Get("DEPTID");
            JObject jo = new JObject();

            if (DEPTID.Length < 1)
            {
                jo.Add("result", "fail");
                jo.Add("data", "请选择需要出库的科室！");
                return JsonConvert.SerializeObject(jo);
            }
            if (!DbHelperOra.Exists(String.Format("SELECT 1 FROM DAT_RK_COM A WHERE A.SEQNO = '{0}' AND EXISTS(SELECT 1 FROM DOC_GOODSCFG B WHERE B.GDSEQ = A.GDSEQ AND B.DEPTID = '{1}')", billno, DEPTID)))
            {
                jo.Add("result", "fail");
                jo.Add("data", "单据中有商品都未配置到科室，请检查!");
                return JsonConvert.SerializeObject(jo);
            }
            Object obj = DbHelperOra.GetSingle(String.Format(@"SELECT TO_CHAR(wmsys.wm_concat(DISTINCT GDNAME)) BILLSUM
                FROM DAT_RK_COM A
                WHERE A.SEQNO = '{0}' AND NOT EXISTS(SELECT 1 FROM DOC_GOODSCFG B WHERE B.GDSEQ = A.GDSEQ AND B.DEPTID = '{1}')", billno, DEPTID));
            if ((obj ?? "").ToString().Length > 1)
            {
                jo.Add("result", "fail");
                jo.Add("data", "商品【" + obj + "】未配置到科室");
                return JsonConvert.SerializeObject(jo);
            }
            else
            {
                if (DEPTID.Length < 1)
                {
                    jo.Add("result", "fail");
                    jo.Add("data", "请选择转出库部门");
                    return JsonConvert.SerializeObject(jo);
                }
                if (billno.Length < 1)
                {
                    jo.Add("result", "fail");
                    jo.Add("data", "请选择需要转出库的入库单据");
                    return JsonConvert.SerializeObject(jo);

                }
                OracleParameter[] parameters = new OracleParameter[]
                                {
                                     new OracleParameter("VI_BILLNO",OracleDbType.Varchar2),
                                     new OracleParameter("VI_DEPTID",OracleDbType.Varchar2),
                                     new OracleParameter("VI_OPERUSER",OracleDbType.Varchar2)
                                };
                parameters[0].Value = billno;
                parameters[1].Value = DEPTID;
                parameters[2].Value = UserAction.UserID;
                try
                {
                    DbHelperOra.RunProcedure("STOREDS.P_RKTOCK", parameters);
                    jo.Add("result", "success");
                    jo.Add("data", "转出库生成成功");
                    return JsonConvert.SerializeObject(jo);
                }
                catch (Exception e)
                {
                    jo.Add("result", "fail");
                    jo.Add("data", "单据异常，请检查");
                    return JsonConvert.SerializeObject(jo);
                }
            }
        }
        #endregion

        protected void btnBatchAudit_Click(object sender, EventArgs e)
        {
            string bills = "";//天津批量审核后批量打印，记录批量打印
            hfdBills.Text = "";
            if (GridList.SelectedRowIndexArray.Length < 1)
            {
                Alert.Show("请选择要审核的单据信息！", "消息提醒", MessageBoxIcon.Warning);
                return;
            }
            if (Doc.DbGetSysPara("LOCKSTOCK") == "Y")
            {
                Alert.Show("系统库存已被锁定，请等待物资管理科结存处理完毕再做审核处理！", "消息提醒", MessageBoxIcon.Warning);
                return;
            }
            string seqnos = string.Empty;
            string strBill = string.Empty;
            List<CommandInfo> cmdList = new List<CommandInfo>();
            foreach (int index in GridList.SelectedRowIndexArray)
            {
                //验证科室是否盘点
                if (DbHelperOra.Exists("SELECT 1 FROM DAT_PD_STOCK WHERE DEPTID IN('" + GridList.Rows[index].DataKeys[3].ToString() + "','" + GridList.Rows[index].DataKeys[4].ToString() + "')"))
                {
                    Alert.Show("第【" + (index + 1).ToString() + "】行单据【" + GridList.Rows[index].DataKeys[0].ToString() + "】出库或申领科室正在盘点,请检查!", "警告提示", MessageBoxIcon.Warning);
                    return;
                }
                //验证单据状态是否正确
                if (GridList.Rows[index].DataKeys[1].ToString() != "N")
                {
                    Alert.Show("第【" + (index + 1).ToString() + "】行单据【" + GridList.Rows[index].DataKeys[0].ToString() + "】当前状态为【" + GridList.Rows[index].DataKeys[6].ToString() + "】，不能进行审核！！！", "操作提示", MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace((GridList.Rows[index].DataKeys[5] ?? "").ToString()) || !DbHelperOra.Exists("SELECT 1 FROM DAT_USE_DET WHERE ORDBILLNO = '" + (GridList.Rows[index].DataKeys[5] ?? "").ToString() + "' AND FLAG = 'Y'"))
                {
                    string strSql = @"SELECT 1 FROM DAT_RK_EXT A, DOC_GOODS B
                                     WHERE A.GDSEQ = B.GDSEQ AND A.BILLNO = '{0}' AND A.FLAG = 'N'
                                      UNION
                                     SELECT 1 FROM DAT_RK_COM A, DOC_GOODS B
                                     WHERE A.GDSEQ = B.GDSEQ AND A.SEQNO = '{0}'
                                          AND B.ISGZ = 'Y' AND A.GDSEQ NOT IN (SELECT GDSEQ FROM DAT_RK_EXT WHERE BILLNO = '{0}')";
                    //判断高值码是否符合入库标准
                    if (DbHelperOra.Exists(string.Format(strSql, GridList.Rows[index].DataKeys[0].ToString())))
                    {
                        Alert.Show("存在未扫描的追溯码,不允许入库！", "提示信息", MessageBoxIcon.Warning);
                        return;
                    }
                }
                seqnos += GridList.Rows[index].DataKeys[0].ToString() + ",";

                OracleParameter[] parameters = {
                                               new OracleParameter("VIN_BILLSEQ", OracleDbType.Varchar2),
                                               new OracleParameter("VIN_BILLTYPE", OracleDbType.Varchar2),
                                               new OracleParameter("VIN_OPERUSER", OracleDbType.Varchar2),
                                               new OracleParameter("VIN_OPERTYPE", OracleDbType.Varchar2) };
                parameters[0].Value = GridList.Rows[index].DataKeys[0].ToString();
                parameters[1].Value = GridList.Rows[index].DataKeys[4].ToString();
                parameters[2].Value = UserAction.UserID;
                parameters[3].Value = "AUDIT";
                cmdList.Add(new CommandInfo("STORE.P_BILLOPER", parameters, CommandType.StoredProcedure));
                //记录批量审核的单据
                bills += GridList.Rows[index].DataKeys[0].ToString() + "_";
                //记录需要判断是否盘点的单据
                strBill = strBill + "'" + GridList.Rows[index].DataKeys[0].ToString() + "',";
            }
            if (DbHelperOra.Exists("select 1 from  DAT_rk_DOC where deptid in (SELECT deptid FROM DAT_PD_LOCK WHERE  FLAG = 'N') AND SEQNO IN (" + strBill.TrimEnd(',') + ") "))
            {
                Alert.Show("系统库存已被锁定，请等待盘点处理完毕再做审核处理！", "消息提醒", MessageBoxIcon.Warning);
                return;
            }
            try
            {
                DbHelperOra.ExecuteSqlTran(cmdList);
            }
            catch (Exception ex)
            {
                Alert.Show(ERPUtility.errorParse(ex.Message), "错误提示", MessageBoxIcon.Error);
                bills = "";//如果审核失败清楚需要打印的单据
                return;
            }
            int glselectedcount = GridList.SelectedRowIndexArray.Length;
            string[] seqnoarr = new string[glselectedcount];
            for (int i = 0; i < GridList.SelectedRowIndexArray.Length; i++)
            {
                seqnoarr[i] = GridList.Rows[GridList.SelectedRowIndexArray[i]].DataKeys[0].ToString();
            }
            billSearch();
            AfterPriSelect(seqnoarr);//传递被选中的审批单据号，刷新完页面再次自动选中CJL
            Alert.Show("单据批量审核成功！", "消息提示", MessageBoxIcon.Information);
            hfdBills.Text = bills.Trim('_');//录入需要打印的单据
            //PageContext.RegisterStartupScript("PrintYSRKD();");//调用打印,不自动调用打印
            OperLog("验收入库批量审核", "审核单据【" + seqnos.Trim(',') + "】");
        }
        protected void AfterPriSelect(string[] arrseqno)
        {
            int[] tem = new int[arrseqno.Length];
            for (int i = 0; i < arrseqno.Length; i++)
            {

                for (int j = 0; j < GridList.Rows.Count; j++)
                {
                    if (GridList.Rows[j].DataKeys[0].ToString() == arrseqno[i].ToString())
                    {

                        tem[i] = j;
                        break;
                    }
                }
            }
            GridList.SelectedRowIndexArray = tem;
        }
        protected void btnBatchPrint_Click(object sender, EventArgs e)
        {
            if (GridList.SelectedRowIndexArray.Length < 1)
            {
                Alert.Show("请选择要打印的单据信息！", "消息提醒", MessageBoxIcon.Warning);
                return;
            }
            string bills = "";
            hfdBills.Text = "";
            foreach (int index in GridList.SelectedRowIndexArray)
            {
                //验证单据状态是否正确
                if ((",Y,G").IndexOf(GridList.Rows[index].DataKeys[1].ToString()) < 1)
                {
                    Alert.Show("第【" + (index + 1).ToString() + "】行单据【" + GridList.Rows[index].DataKeys[0].ToString() + "】当前状态为【" + GridList.Rows[index].DataKeys[6].ToString() + "】，不允许打印！！！", "操作提示", MessageBoxIcon.Warning);
                    return;
                }
                bills += GridList.Rows[index].DataKeys[0].ToString() + "_";
            }
            hfdBills.Text = bills.Trim('_');
            PageContext.RegisterStartupScript("PrintYSRKD();");
        }

        protected void GridList_Sort(object sender, GridSortEventArgs e)
        {
            billSearch();
        }

        protected void btnSelect_Click(object sender, EventArgs e)
        {
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
            highlightRows.Text = "";
            #region 导入
            if (this.fuDocument.HasFile)
            {
                string toFilePath = "~/ERPUpload/DownloadExcel/";

                //获得文件扩展名
                string fileNameExt = Path.GetExtension(this.fuDocument.FileName).ToLower();

                if (!ValidateFileType(fileNameExt))
                {
                    Alert.Show("无效的文件类型！", "消息提示", MessageBoxIcon.Warning);
                    return;
                }

                //验证合法的文件
                if (CheckFileExt(fileNameExt))
                {
                    //生成将要保存的随机文件名
                    string fileName = this.fuDocument.FileName.Substring(0, this.fuDocument.FileName.IndexOf(".")) + DateTime.Now.ToString("yyyyMMddHHmmss") + fileNameExt;

                    //按日期归类保存
                    string datePath = DateTime.Now.ToString("yyyyMM") + "/" + DateTime.Now.ToString("dd") + "/";
                    toFilePath += datePath;

                    //获得要保存的文件路径
                    string DownloadUrl = toFilePath + fileName;
                    //物理完整路径                    
                    string toFileFullPath = HttpContext.Current.Server.MapPath(toFilePath);

                    //检查是否有该路径,没有就创建
                    if (!Directory.Exists(toFileFullPath))
                    {
                        Directory.CreateDirectory(toFileFullPath);
                    }

                    //将要保存的完整物理文件名                
                    string serverFileName = toFileFullPath + fileName;

                    //获取保存的excel路径
                    this.fuDocument.SaveAs(serverFileName);

                    if (File.Exists(serverFileName))
                    {
                        DataTable table = new DataTable();

                        if (fileNameExt == ".xlsx")
                        {
                            table = ExcelHelper.ImportExcelxtoDt(serverFileName, 0, 1); //导入excel2007
                        }
                        else
                        {
                            table = ExcelHelper.ImportExceltoDt(serverFileName, 0, 1);//导入excel2003
                        }
                        StringBuilder sbr = new StringBuilder();
                        if (table != null && table.Rows.Count > 0)
                        {
                            try
                            {


                                table.Columns.Add("录入人编号", Type.GetType("System.String"));
                                table.Columns.Add("录入人姓名", Type.GetType("System.String"));
                                for (int i = 0; i < table.Rows.Count; i++)
                                {
                                    if (!string.IsNullOrEmpty(table.Rows[i]["入库部门编码"].ToString()))
                                    {

                                        //校验数据的有效性并提示
                                        //入库部门，送货商编码，商品编码，包装含量，入库包装数，不能为空

                                        if (string.IsNullOrEmpty(table.Rows[i]["入库部门"].ToString()))
                                        {
                                            sbr.Append("第" + (i + 1) + "行【入库部门】为空");
                                        }
                                        if (string.IsNullOrEmpty(table.Rows[i]["送货商编码"].ToString()))
                                        {
                                            sbr.Append("第" + (i + 1) + "行【送货商编码】为空");
                                        }
                                        if (string.IsNullOrEmpty(table.Rows[i]["商品编码"].ToString()))
                                        {
                                            sbr.Append("第" + (i + 1) + "行【商品编码】为空");
                                        }
                                        if (string.IsNullOrEmpty(table.Rows[i]["包装含量"].ToString()))
                                        {
                                            sbr.Append("第" + (i + 1) + "行【包装含量】为空");
                                        }
                                        if (string.IsNullOrEmpty(table.Rows[i]["入库包装数"].ToString()))
                                        {
                                            sbr.Append("第" + (i + 1) + "行【入库包装数】为空");
                                        }
                                        if (string.IsNullOrEmpty(table.Rows[i]["入库数"].ToString()))
                                        {
                                            sbr.Append("第" + i + 1 + "行【入库数】为空");
                                        }
                                        if (string.IsNullOrEmpty(table.Rows[i]["批号"].ToString()))
                                        {
                                            sbr.Append("第" + (i + 1) + "行【批号】为空");
                                        }
                                        if (string.IsNullOrEmpty(table.Rows[i]["生产日期"].ToString()))
                                        {

                                            sbr.Append("第" + (i + 1) + "行【生产日期】为空");
                                        }
                                        if (string.IsNullOrEmpty(table.Rows[i]["有效日期"].ToString()))
                                        {
                                            sbr.Append("第" + (i + 1) + "行【有效日期】为空");
                                        }
                                        /****
                                        * 校验数据
                                        * 1.收获地点必须是库房，商品必须在配置到了该库房
                                        * 2.送货商必须是送货商,必须有只如果没有取供应商
                                        * 3.商品的状态必须是可用的 Y S
                                        * 4.批号要检验长度，生产日期和有效期必须是日期格式，
                                        * 生产日期小于当前日期，有效期大于当前日期
                                        * 批号一百个字符(100)
                                        */
                                        //商品必须配置到改库房
                                        if (DbHelperOra.Query(@"SELECT 1 FROM SYS_DEPT
WHERE TYPE ='1' AND CODE ='" + table.Rows[i]["入库部门编码"].ToString() + "'").Tables[0].Rows.Count == 0)
                                        {
                                            sbr.Append("第" + (i + 1) + "行【入库部门编码】库房不存在</br>");
                                        }
                                        //商品是都配置到库房
                                        if (DbHelperOra.Query(@"SELECT 1 FROM DOC_GOODSCFG
WHERE GDSEQ='" + table.Rows[i]["商品编码"].ToString() + "' AND DEPTID ='" + table.Rows[i]["入库部门编码"].ToString() + "'").Tables[0].Rows.Count == 0)
                                        {
                                            sbr.Append("第" + (i + 1) + "行商品【" + table.Rows[i]["商品名称"].ToString() + "】没有配置到【" + table.Rows[i]["入库部门编码"].ToString() + "】库房</br>");
                                        }

                                        if (string.IsNullOrEmpty(table.Rows[i]["入库数"].ToString()))
                                        {//入库数不能为空
                                            sbr.Append("第" + (i + 1) + "行商品【" + table.Rows[i]["商品名称"].ToString() + "】入库数量不能为空</br>");
                                        }
                                        //送货是否有该商品的配送权限
                                        if (DbHelperOra.Query(@"SELECT 1 FROM DOC_GOODSSUP
WHERE GDSEQ='" + table.Rows[i]["商品编码"].ToString() + "' AND PSSID ='" + table.Rows[i]["送货商编码"].ToString() + "'").Tables[0].Rows.Count == 0 && DbHelperOra.Query(@"SELECT 1 FROM DOC_GOODSSUP
WHERE GDSEQ='" + table.Rows[i]["商品编码"].ToString() + "' AND SUPID ='" + table.Rows[i]["送货商编码"].ToString() + "'").Tables[0].Rows.Count == 0)
                                        {
                                            sbr.Append("第" + (i + 1) + "行商品【" + table.Rows[i]["商品名称"].ToString() + "】没有配置到该配送商:【" + table.Rows[i]["送货商"].ToString() + "】</br>");
                                        }

                                        table.Rows[i]["录入人编号"] = UserAction.UserID;
                                        table.Rows[i]["录入人姓名"] = UserAction.UserName;
                                        //计算是否准确
                                        if (Int32.Parse(table.Rows[i]["包装含量"].ToString()) * Int32.Parse(table.Rows[i]["入库包装数"].ToString()
                                            ) != Int32.Parse(table.Rows[i]["入库数"].ToString()))
                                        {
                                            //
                                            sbr.Append("第" + (i + 1) + "行商品【" + table.Rows[i]["商品名称"].ToString() + "】订货数量计算错误</br>");
                                        }
                                        //交验日期格式
                                        if (!IsDate(table.Rows[i]["生产日期"].ToString()))
                                        {
                                            sbr.Append("第" + (i + 1) + "行商品【" + table.Rows[i]["商品名称"].ToString() + "】生产日期不是有效的日期格式</br>");
                                        }
                                        if (!IsDate(table.Rows[i]["有效日期"].ToString()))
                                        {
                                            sbr.Append("第" + (i + 1) + "行商品【" + table.Rows[i]["商品名称"].ToString() + "】有效日期不是有效的日期格式</br>");
                                        }
                                        //有效期要大于当前日期
                                        DateTime dt1 = DateTime.Parse(table.Rows[i]["有效日期"].ToString());
                                        DateTime dt2 = DateTime.Parse(table.Rows[i]["生产日期"].ToString());
                                        if (DateTime.Compare(dt1, System.DateTime.Now) < 0)
                                        {
                                            sbr.Append("第" + (i + 1) + "行商品【" + table.Rows[i]["商品名称"].ToString() + "】有效日期不得小于当前日期</br>");
                                        }

                                        //生产日期小于有效期
                                        if (DateTime.Compare(dt2, dt1) > 0)
                                        {
                                            sbr.Append("第" + (i + 1) + "行商品【" + table.Rows[i]["商品名称"].ToString() + "】生产日期不得大于有效日期</br>");
                                        }
                                        //生产日期大于当前日期
                                        if (DateTime.Compare(dt2, System.DateTime.Now) > 0)
                                        {
                                            sbr.Append("第" + (i + 1) + "行商品【" + table.Rows[i]["商品名称"].ToString() + "】生产日期不得大于当前日期</br>");
                                        }

                                    }
                                }

                            }
                            catch
                            {
                                fuDocument.Reset();
                                Alert.Show("导入模板有误，请检查", "操作提示", MessageBoxIcon.Warning);
                                return;

                            }
                            if (sbr.Length > 0)
                            {
                                fuDocument.Reset();
                                Alert.Show(sbr.ToString(), "操作提示", MessageBoxIcon.Warning);
                                return;
                            }
                            //DbHelperOra.Query("TRUNCATE TABLE TEMP_DAT_RK_DOC_GOODS");
                            if (table != null && table.Rows.Count > 0)
                            {
                                DataTable insertTable = new DataTable();
                                insertTable.Columns.Add("DEPTID");
                                insertTable.Columns.Add("DEPNAME");
                                insertTable.Columns.Add("PSSID");
                                insertTable.Columns.Add("PSSNAME");
                                insertTable.Columns.Add("GDSEQ");
                                insertTable.Columns.Add("GDNAME");
                                insertTable.Columns.Add("GDSPEC");
                                insertTable.Columns.Add("UNIT");
                                insertTable.Columns.Add("BZHL");
                                insertTable.Columns.Add("PH");
                                insertTable.Columns.Add("SSSL");
                                insertTable.Columns.Add("KCHSJE");
                                insertTable.Columns.Add("HSJJ");
                                insertTable.Columns.Add("LRLID");
                                insertTable.Columns.Add("LRLNAME");
                                insertTable.Columns.Add("RKSL");
                                insertTable.Columns.Add("MANUFACTUREDATE");
                                insertTable.Columns.Add("EFFECTIVEDATE");

                                foreach (DataRow row in table.Rows)
                                {
                                    DataRow newrow = insertTable.NewRow();
                                    if (!string.IsNullOrEmpty(row["入库部门编码"].ToString()))
                                    {

                                        try
                                        {
                                            newrow["DEPTID"] = row["入库部门编码"];
                                            newrow["DEPNAME"] = row["入库部门"];
                                            newrow["PSSID"] = row["送货商编码"];
                                            newrow["PSSNAME"] = row["送货商"];
                                            newrow["GDSEQ"] = row["商品编码"];
                                            newrow["GDNAME"] = row["商品名称"];
                                            newrow["GDSPEC"] = row["商品规格"];
                                            newrow["UNIT"] = row["单位"];
                                            newrow["BZHL"] = row["包装含量"];
                                            newrow["KCHSJE"] = row["含税进价"];
                                            newrow["HSJJ"] = row["含税金额"];
                                            newrow["LRLID"] = row["录入人编号"];
                                            newrow["LRLNAME"] = row["录入人姓名"];
                                            newrow["SSSL"] = row["入库数"];
                                            newrow["RKSL"] = row["入库数"];
                                            newrow["PH"] = row["批号"];
                                            newrow["MANUFACTUREDATE"] = row["生产日期"];
                                            newrow["EFFECTIVEDATE"] = row["有效日期"];
                                            insertTable.Rows.Add(newrow);
                                        }
                                        catch
                                        {

                                            Alert.Show("导入模板有误，请检查", "操作提示", MessageBoxIcon.Warning);
                                            fuDocument.Reset();
                                            return;
                                        }
                                    }


                                }


                                /**
                                 * 批量添加数据
                                 * */
                                try
                                {
                                    if (insertTable.Rows.Count > 0)
                                    {
                                        OracleParameter[] parameters = new OracleParameter[1];
                                        parameters[0] = new OracleParameter();
                                        parameters[0].ParameterName = "";
                                        parameters[0].Value = "vi_seq";
                                        ExecuteNonQuery(insertTable, "SELECT * FROM TEMP_DAT_RK_DOC_GOODS", "P_YS_RK_GOODS_IMPORT", parameters);


                                        Alert.Show("数据导入成功,共导入[" + insertTable.Rows.Count.ToString() + "]条数据！", "消息提示", MessageBoxIcon.Information);
                                        billSearch();
                                        fuDocument.Reset();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Alert.Show("数据库错误：" + ex.Message.ToString(), "异常信息", MessageBoxIcon.Warning);
                                    fuDocument.Reset();
                                }
                                finally
                                {
                                    fuDocument.Reset();
                                }
                            }
                            File.Delete(serverFileName);
                            fuDocument.Reset();

                        }
                    }
                }
                else
                {
                    Alert.Show("请选择excel文件！");
                }
            }
            #endregion
        }

        /// <summary>
        /// 验证数字
        /// </summary>
        /// <param name="number">数字内容</param>
        /// <returns>true 验证成功 false 验证失败</returns>
        public bool CheckNumber(string number)
        {
            if (string.IsNullOrEmpty(number))
            {
                return false;
            }
            Regex regex = new Regex(@"^(-)?\d+(\.\d+)?$");
            if (regex.IsMatch(number))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 是否为日期型字符串
        /// </summary>
        /// <param name="StrSource">日期字符串(2008-05-08)</param>
        /// <returns></returns>
        public bool IsDate(string strDate)
        {
            try
            {
                DateTime.Parse(strDate);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void ExecuteNonQuery(DataTable table, string strSql, string storedProcedureName, params OracleParameter[] args)
        {
            using (OracleConnection con = new OracleConnection(XTBase.DbHelperOra.ConnString()))
            {
                con.Open();
                OracleTransaction transaction = con.BeginTransaction();

                OracleCommand cmd = transaction.Connection.CreateCommand();
                cmd.CommandText = strSql;
                OracleDataAdapter da = new OracleDataAdapter(cmd);
                OracleCommandBuilder builer = new OracleCommandBuilder(da);
                try
                {
                    da.Update(table);
                    if (args != null)
                    {
                        foreach (var p in args)
                        {
                            cmd.Parameters.Add(p);
                        }
                    }
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = storedProcedureName;
                    cmd.Transaction = transaction;
                    cmd.ExecuteNonQuery();
                    transaction.Commit();
                    con.Close();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    con.Close();
                    throw;
                }

            }

        }
        private bool CheckFileExt(string fileNameExt)
        {
            if (String.IsNullOrEmpty(fileNameExt))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}