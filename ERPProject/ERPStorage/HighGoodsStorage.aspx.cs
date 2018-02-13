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

namespace ERPProject.ERPStorage
{
    public partial class HighGoodsStorage : BillBase
    {
        string strDocSql = "SELECT * FROM DAT_YRK_DOC WHERE SEQNO ='{0}'";
        string strComSql = @"SELECT A.SEQNO,A.ROWNO,A.DEPTID,A.GDSEQ,A.BARCODE,A.GDNAME,A.UNIT,A.GDSPEC,A.GDMODE,A.CDID,A.SPLB,A.CATID,A.HWID,A.BZHL,A.BZSL,
                                                    A.DDSL,A.SSSL,A.JXTAX,A.HSJJ,A.BHSJJ,A.HSJE,A.BHSJE,A.LSJ,A.LSJE,A.ISGZ,A.ISLOT,A.PHID,A.PH, A.PZWH,TO_CHAR(A.RQ_SC,'YYYY-MM-DD') RQ_SC,
                                                    TO_CHAR(A.YXQZ,'YYYY-MM-DD') YXQZ,A.KCSL,A.KCHSJE,A.SPZTSL,A.ERPAYXS,A.HLKC,A.ZPBH,A.STR1,A.STR2,A.STR3,A.NUM1,A.NUM2,
                                                    A.NUM3,A.MEMO,A.ONECODE, F_GETUNITNAME(A.UNIT) UNITNAME,F_GETUNITNAME(B.UNIT) UNITSMALLNAME,F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME
                                                    ,F_GETSUPNAME(A.SUPID) SUPNAME,MJXQ,MJRQ,MJPH,A.PRODUCER,A.SUPID
                                           FROM DAT_YRK_COM A, DOC_GOODS B WHERE SEQNO = '{0}' AND A.GDSEQ = B.GDSEQ ";

        public override Field[] LockControl
        {
            get { return new Field[] { docBILLNO, docCGY, docDDBH, docPSSID, docDEPTID, docDHRQ }; }
        }

        public HighGoodsStorage()
        {
            BillType = "YRK";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindDDL();
                billNew();

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
                        ButtonHidden(btnAudit, btnCancel);
                    }
                    else if (Request.QueryString["oper"].ToString() == "audit")
                    {
                        billLockDoc(true);
                        ButtonHidden(btnNew, btnCopy, btnSave, btnAddRow, btnDelRow, btnGoods, btnCommit, btnAllCommit, btnDel);
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
                }

                //hfdOneCode.Text = Doc.DbGetSysPara("ISONECODE");
                //if (PubFunc.StrIsEmpty(hfdOneCode.Text))
                //{
                //    hfdOneCode.Text = "Y";
                //}
                ////不启用唯一码
                //if (hfdOneCode.Text == "N")
                //{
                //    btnScan.Hidden = true;
                //}
            }
            else
            {
                if (GetRequestEventArgument() == "TextBox1_ENTER")
                {
                    zsmScan_TextChanged("ENTER", null);
                }
                if (GetRequestEventArgument() == "TextBox2_ENTER")
                {
                    zsmERP_TextChanged("ENTER", null);
                }
            }
        }
        protected override void BindDDL()
        {
            ////if (!isDg())
            ////{
            ////    PubFunc.DdlDataGet("DDL_DOC_SUPPLIERNULL", lstSUPID, docSUPID);
            ////    PubFunc.DdlDataGet("DDL_SYS_DEPOT", lstDEPTID, docDEPTID);
            ////}
            ////else
            ////{
            //    PubFunc.DdlDataGet("DDL_DOC_SUPPLIER_DG", lstPSSID, docPSSID);
            //    bindTreeDept(docDEPTID, lstDEPTID);
            ////}
            PubFunc.DdlDataGet("DDL_DOC_SHS", lstPSSID, docPSSID);
            PubFunc.DdlDataGet("DDL_USERALL", lstLRY, docCGY, docSHR, docLRY);
            PubFunc.DdlDataGet("DDL_SYS_DEPOTRANGE", UserAction.UserID, docDEPTID, lstDEPTID);

            PubFunc.DdlDataGet("DDL_BILL_STATUSDHD", lstFLAG, docFLAG);
            PubFunc.DdlDataGet(ddlReject, "DDL_BILL_REASON");

            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
            docDHRQ.SelectedDate = DateTime.Now;
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

        private Boolean isDg()
        {
            if (Request.QueryString["dg"] == null)
            {
                return false;
            }
            else if (Request.QueryString["dg"].ToString() == "1")
            {
                return true;
            }
            else
            {
                return false;
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
                Alert.Show("开始日期大于结束日期，请重新输入！");
                return;
            }

            string strSql = @"SELECT A.SEQNO,A.BILLNO,F_GETBILLFLAG(FLAG) FLAG_CN,FLAG,A.DDBH,F_GETDEPTNAME(A.DEPTID) DEPTID,
                                     F_GETSUPNAME(A.PSSID) SUPNAME,A.DHRQ,A.SUBSUM,F_GETUSERNAME(A.CGY) CGY,
                                     F_GETUSERNAME(A.LRY) LRY,A.LRRQ,F_GETUSERNAME(A.SHR) SHR,A.SHRQ,A.MEMO 
                                FROM DAT_YRK_DOC A 
                               WHERE 1=1 ";
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
            if (lstPSSID.SelectedItem != null && lstPSSID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.PSSID='{0}'", lstPSSID.SelectedItem.Value);
            }
            if (lstDDBH.Text.Length > 0)
            {
                strSearch += string.Format(" AND A.DDBH LIKE '%{0}%'", lstDDBH.Text);
            }

            //if (lstSUPID.SelectedItem != null && lstSUPID.SelectedItem.Value.Length > 0)
            //{
            //    strSearch += string.Format(" AND FLAG='{0}'", lstSUPID.SelectedItem.Value);
            //}
            //过滤新单的单据，不能审核，提交后的才能审核
            if (Request.QueryString["oper"].ToString() == "audit")
            {
                strSearch += " AND A.FLAG<>'M'";
            }
            //if (isDg())
            //{
            //strSearch += " AND SUPID IN (SELECT SUPID FROM DOC_SUPPLIER WHERE ISDG = 'Y')";
            //}
            //else
            //{
            //    strSearch += " AND SUPID IN (SELECT SUPID FROM DOC_SUPPLIER WHERE ISDG = 'N')";
            //}

            strSearch += string.Format(" AND A.LRRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.LRRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY A.FLAG,A.BILLNO ASC";
            highlightRows.Text = "";
            highlightRowYellow.Text = "";

            DataTable table = DbHelperOra.Query(strSql).Tables[0];
            string sortField = GridList.SortField;
            string sortDirection = GridList.SortDirection;
            DataView view1 = table.DefaultView;
            view1.Sort = String.Format("{0} {1}", sortField, sortDirection);


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
            if (docBILLNO.Text.Length > 0 && docBILLNO.Text.StartsWith("TRI"))
            {
                Alert.Show("下传单据不能追加商品！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
            //string url = "";
            //if (!isDg())
            //{
            //    //参数说明：cx-查询内容，bm-商品配置部门,su-供应商,添加"&goodsType=SUPIDN"，作用是过滤出非代管商品
            //    url = "~/ERPQuery/GoodsWindow_Gather.aspx?bm=" + docDEPTID.SelectedValue + "&cx=&su=" + docSUPID.SelectedValue + "&goodsType=SUPIDN";
            //    PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "商品信息查询"));
            //}
            //else
            //{
            //url = "~/ERPQuery/GoodsWindow_Gather.aspx?bm=" + docDEPTID.SelectedValue + "&cx=&su=" + docSUPID.SelectedValue;
            //PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "商品信息查询"));
            //}
            string url = "~/ERPQuery/GoodsWindow_New.aspx?Deptout=" + docDEPTID.SelectedValue + "&Shs=" + docPSSID.SelectedValue + "&GoodsState=Y";
            PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "商品信息查询"));

        }

        protected override void listRow_DoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            billClear();
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
                tbxMEMO.Enabled = true;
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
                tbxMEMO.Enabled = false;
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
                tbxMEMO.Enabled = false;
            }
        }
        protected override void billNew()
        {
            string strSup = docPSSID.SelectedValue;
            string strDept = docDEPTID.SelectedValue;
            //原单据保存判断
            PubFunc.FormDataClear(FormDoc);
            if (PubFunc.StrIsEmpty(strDept)) //&& !isDg())
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
            comMEMO.Enabled = true;
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
            if (!Doc.getFlag(docBILLNO.Text, "M", BillType))
            {
                Alert.Show("此单据正在被别人操作，不能删除！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!docBILLNO.Text.Contains(BillType) && docBILLNO.Text.Length > 0)
            {
                Alert.Show("下传单据不能删除！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            List<string> listSql = new List<string>();
            listSql.Add("Delete from DAT_YRK_DOC t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            listSql.Add("Delete from DAT_YRK_COM t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            //listSql.Add("UPDATE DAT_DO_LIST SET FLAG = 'Y' WHERE PARA='" + docBILLNO.Text.Trim() + "'");
            if (DbHelperOra.ExecuteSqlTran(listSql))
            {
                Alert.Show("单据删除成功!", "消息提示", MessageBoxIcon.Information);
                billNew();
                billSearch();
                OperLog("跟台商品入库", "删除单据【" + docBILLNO.Text + "】");
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
            string strBillno = docBILLNO.Text;
            List<CommandInfo> sqlList = new List<CommandInfo>();
            if (strBillno.Length == 0)
            {
                Alert.Show("请先保存后，再次提交！");
                return;
            }
            if (docFLAG.SelectedValue != "M")
            {
                Alert.Show("只有保存后单据，才能提交！");
                return;
            }
            if (!Doc.getFlag(strBillno, "M", BillType))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                return;
            }
            SaveSuccess = false;
            save("Y");
            if (SaveSuccess == false)
                return;
            SaveSuccess = false;
            sqlList.Add(new CommandInfo("update DAT_YRK_DOC set flag='N' where BILLNO='" + strBillno + "' ", null));
            //增加待办事宜
            //sqlList.Add(Doc.GETDOADD("DO_8", docDEPTID.SelectedValue, docLRY.SelectedValue, docBILLNO.Text));

            if (DbHelperOra.ExecuteSqlTran(sqlList))
            {
                Alert.Show("此单据提交成功！");
                billSearch();
                billOpen(docBILLNO.Text);
                billLockDoc(true);
                OperLog("跟台商品入库", "提交单据【" + docBILLNO.Text + "】");
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
                    sqlList.Add(new CommandInfo("update DAT_YRK_DOC set flag='N' where BILLNO='" + strBILLNO + "' ", null));
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
                OperLog("跟台商品入库", "提交单据" + succeed + "");
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
                Alert.Show("未选择任何行，无法进行【删行】操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!docBILLNO.Text.Contains(BillType) && docBILLNO.Text.Length > 0)
            {
                Alert.Show("下传单据不能删除！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string strBillno = docSEQNO.Text;
            if (!Doc.getFlag(strBillno, "M", BillType) && !Doc.getFlag(strBillno, "R", BillType))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                return;
            }
            //GridCom.DeleteRow(GridCom.SelectedRowID);
            GridCom.DeleteSelectedRows();
            List<Dictionary<string, object>> newDict = GridCom.GetNewAddedList();
            UpdateSum(newDict);

        }
        protected void save(string flag="N")
        {
            #region 数据有效性验证
            if (("MR").IndexOf(docFLAG.SelectedValue) < 0 && hfdOper.Text == "input")
            {
                Alert.Show("非新单不能保存！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if ((!CheckFlag("M") && !CheckFlag("R")) && hfdOper.Text == "input")
            {
                Alert.Show("此单据已经被别人操作，请等待操作!");
                return;
            }
            List<Dictionary<string, object>> goodsData = GridCom.GetNewAddedList();
            if (goodsData.Count == 0)
            {
                Alert.Show("请输入商品信息", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            if (PubFunc.FormDataCheck(FormDoc).Length > 0) return;
            if (docDHRQ.SelectedDate == null)
            {
                Alert.Show("收货日期输入错误!");
                return;
            }
            string type = DbHelperOra.GetSingle(string.Format("select TYPE from SYS_DEPT where CODE='{0}'", docDEPTID.SelectedValue)).ToString();
            List<Dictionary<string, object>> newDict = new List<Dictionary<string, object>>();
            //判断是否有空行、批号填写是否符合要求
            for (int i = 0; i < goodsData.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(goodsData[i]["GDSEQ"].ToString()) && !string.IsNullOrWhiteSpace(goodsData[i]["GDNAME"].ToString()))
                {
                    if ((goodsData[i]["BZSL"] ?? "").ToString() == "" || (goodsData[i]["BZSL"] ?? "").ToString() == "0")
                    {
                        Alert.Show("请填写商品[" + goodsData[i]["GDSEQ"] + "]入库数！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    if (goodsData[i]["ISLOT"].ToString() == "1" || goodsData[i]["ISLOT"].ToString() == "2")
                    {
                        if (string.IsNullOrWhiteSpace((string)goodsData[i]["PH"]))
                        {
                            Alert.Show("第[" + (i + 1) + "]行商品【" + goodsData[i]["GDNAME"].ToString() + "】批号不能为空！！！", "消息提示", MessageBoxIcon.Warning);
                            return;
                        }
                        try
                        {
                            if (!string.IsNullOrWhiteSpace((goodsData[i]["RQ_SC"] ?? "").ToString()) && !string.IsNullOrWhiteSpace((goodsData[i]["YXQZ"] ?? "").ToString()))
                            {
                                DateTime t1, t2 = DateTime.Now;
                                if (!DateTime.TryParse(goodsData[i]["RQ_SC"].ToString(), out t1) || !DateTime.TryParse(goodsData[i]["YXQZ"].ToString(), out t2))
                                {
                                    Alert.Show("第[" + (i + 1) + "]行商品【" + goodsData[i]["GDNAME"].ToString() + "】生产日期/有效期至输入错误", "提示信息", MessageBoxIcon.Warning);
                                    return;
                                }
                                DateTime td = DateTime.Now;
                                if (DateTime.Compare(t1, t2) >= 0 || DateTime.Compare(t2, td) <= 0 || DateTime.Compare(t1, td) > 0)
                                {
                                    Alert.Show("第[" + (i + 1) + "]行商品【" + goodsData[i]["GDNAME"].ToString() + "】生产日期/有效期至存在问题！！！", "消息提示", MessageBoxIcon.Warning);
                                    return;
                                }
                            }
                        }
                        catch
                        {
                            Alert.Show("第[" + (i + 1) + "]行商品【" + goodsData[i]["GDNAME"].ToString() + "】生产日期/有效期至输入错误", "提示信息", MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    if (!string.IsNullOrEmpty(goodsData[i]["PH"].ToString()) && DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_GOODSSTOCK WHERE  PH='{0}'", goodsData[i]["PH"].ToString())))
                    {

                        if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_GOODSSTOCK WHERE  PH='{0}' AND （RQ_SC <> TO_DATE('{1}','YYYY-MM-DD') OR YXQZ <> TO_DATE('{2}','YYYY-MM-DD')）  AND GDSEQ='{3}'  ", goodsData[i]["PH"].ToString(), (goodsData[i]["RQ_SC"] ?? "").ToString(), (goodsData[i]["YXQZ"] ?? "").ToString(), goodsData[i]["GDSEQ"].ToString())))
                        {
                            Alert.Show("第[" + (i + 1) + "]行商品【" + goodsData[i]["GDNAME"].ToString() + "】 该批号的对应的生产日期/有效期至输入有误", "提示信息", MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    if ((goodsData[i]["HSJJ"] ?? "").ToString() == "" || (goodsData[i]["HSJJ"] ?? "").ToString() == "0" || Convert.ToDecimal(goodsData[i]["HSJJ"]) == 0)
                    {
                        Alert.Show("商品[" + goodsData[i]["GDSEQ"] + "]的【含税进价】不能为零,请维护好商品的含税进价，再次保存！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }


                    //当入库部门为1药库、2药房时
                    if ((",1,2").IndexOf(type) > 0 && string.IsNullOrWhiteSpace(goodsData[i]["HWID"].ToString()))
                    {
                        Alert.Show("第[" + (i + 1) + "]行商品【" + goodsData[i]["GDNAME"].ToString() + "】货位不能为空！！！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(goodsData[i]["HWID"].ToString()))
                        {
                            goodsData[i]["HWID"] = docDEPTID.SelectedValue;
                        }
                    }
                    newDict.Add(goodsData[i]);
                }
            }

            if (newDict.Count == 0)//所有Gird行都为空行时
            {
                Alert.Show("商品信息不能为空", "消息提示", MessageBoxIcon.Warning);
                return;
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
                docSEQNO.Text = docBILLNO.Text;
                docBILLNO.Enabled = false;
                
            }
            if (docBILLNO.Text.Length > 15)
            {
                Alert.Show("单据编号输入超长,请检查!");
                return;
            }
            MyTable mtType = new MyTable("DAT_YRK_DOC");
            mtType.ColRow = PubFunc.FormDataHT(FormDoc);
            mtType.ColRow["SEQNO"] = docBILLNO.Text;
            mtType.ColRow["FLAG"] = "M";
            if (hfdOper.Text == "audit")
            {
                mtType.ColRow["FLAG"] = "N";
            }
            mtType.ColRow.Add("BILLTYPE", BillType);
            mtType.ColRow.Add("SUBNUM", newDict.Count);
            mtType.ColRow.Add("PPSID", docPSSID.SelectedValue);
            //mtType.ColRow.Add("SUPNAME", docSUPID.SelectedText);
            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtTypeMx = new MyTable("DAT_YRK_COM");
            //取消高值写入逻辑
            //MyTable mtTypeExt = new MyTable("DAT_RK_EXT");
            MyTable mtTypePh = new MyTable("DOC_GOODSPH");
            decimal subNum = 0;//总金额
            //先删除单据信息在插入
            cmdList.Add(new CommandInfo("delete DAT_YRK_DOC where seqno='" + docBILLNO.Text + "'", null));//删除单据台头
            cmdList.Add(new CommandInfo("delete DAT_YRK_COM where seqno='" + docBILLNO.Text + "'", null));//删除单据明细
            newDict = newDict.OrderBy(x => x["GDSEQ"]).ToList();//按照商品编码重新排序
            for (int i = 0; i < newDict.Count; i++)
            {
                mtTypeMx.ColRow = PubFunc.GridDataGet(newDict[i]);

                mtTypeMx.ColRow.Add("SEQNO", docBILLNO.Text);
                mtTypeMx.ColRow["ROWNO"] = i + 1;
                mtTypeMx.ColRow.Add("PHID", mtTypeMx.ColRow["PH"]);
                //mtTypeMx.ColRow.Add("SUPID", mtTypeMx.ColRow["SUPID"]);
                //mtTypeMx.ColRow.Add("PRODUCER", mtTypeMx.ColRow["PRODUCER"]);
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
                mtTypeMx.ColRow["HSJE"] = decimal.Parse(mtTypeMx.ColRow["HSJJ"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                subNum = subNum + decimal.Parse(mtTypeMx.ColRow["HSJE"].ToString());
                //前台订单数量是按照订货包转来换算的，所以这里要再换算回来
                mtTypeMx.ColRow["DDSL"] = decimal.Parse(mtTypeMx.ColRow["BZHL"].ToString()) * decimal.Parse(mtTypeMx.ColRow["DDSL"].ToString());

                //string codeInfo = newDict[i]["CODEINFO"].ToString();
                //mtTypeMx.ColRow.Remove("CODEINFO");
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
                if(flag == "N")
                    Alert.Show("商品入库数据操作成功！");
                //billNew();
                billOpen(docBILLNO.Text);
                billLockDoc(true);
                OperLog("跟台商品入库", "保存单据【" + docBILLNO.Text + "】");
                SaveSuccess = true;
            }
            else
            {
                Alert.Show("商品入库数据保存失败！", "错误提示", MessageBoxIcon.Error);
            }
        }
        protected override void billSave()
        {
            save();
        }
        protected override void billAudit()
        {
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("非新单不能审核！", "警告提示", MessageBoxIcon.Warning);
                return;
            }

            string strBillno = docSEQNO.Text;
            if (!Doc.getFlag(strBillno, "N", BillType))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                return;
            }
            if (!DbHelperOra.Exists("SELECT 1 FROM DAT_YRK_DOC A,SYS_DEPT B WHERE A.DEPTID = B.CODE AND A.SEQNO = '" + strBillno + "' AND B.TYPE = '1'"))
            {
                Alert.Show("入库库房不存在，请检查！", "警告提示", MessageBoxIcon.Warning);
                return;
            }
            //if (DbHelperOra.Exists(@"SELECT 1 FROM DAT_YRK_EXT WHERE BILLNO = '" + strBillno + "' AND NVL(STR1,'#') = '#'") || !DbHelperOra.Exists(@"SELECT 1 FROM DAT_YRK_EXT WHERE BILLNO = '" + strBillno + "'"))
            //{
            //    PageContext.RegisterStartupScript(Confirm.GetShowReference("存在未扫描的本位码，是否继续审核？", String.Empty, MessageBoxIcon.Question, PageManager1.GetCustomEventReference("AUDIT"), PageManager1.GetCustomEventReference("")));
            //}
            //else
            //{
            string sql = string.Format("UPDATE DAT_YRK_DOC SET FLAG = 'Y', SHR='{0}', SHRQ=SYSDATE WHERE SEQNO='{1}' AND BILLTYPE = 'YRK'", UserAction.UserID, docSEQNO.Text);
            DbHelperOra.Query(sql);
            //Alert.Show("单据【" + strBillno + "】审核成功！");
            OperLog("跟台商品入库", "审核单据【" + strBillno + "】");
            billOpen(strBillno);
            //}
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
            //if (!docBILLNO.Text.Contains(BillType) && docBILLNO.Text.Length > 0)
            //{
            //    Alert.Show("下传单据不能驳回！", "消息提示", MessageBoxIcon.Warning);
            //    return;
            //}
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
                dtres = dt.Clone();
                foreach (DataRow row in dt.Rows)
                {
                    if ((row["BZHL"] ?? "").ToString() == "")
                    {

                        BZHLerror += "【" + row["GDSEQ"] + "】";
                        continue;
                    }
                    //if (!isDg())
                    //{
                    //    DataTable dtPH = Doc.GetGoodsPHList(row["GDSEQ"].ToString());
                    //    if (dtPH != null && dtPH.Rows.Count > 0)
                    //    {//如果存在多行也只取第一行数据
                    //        row["PH"] = dtPH.Rows[0]["PH"];
                    //        row["PZWH"] = dtPH.Rows[0]["PZWH"];
                    //        row["RQ_SC"] = dtPH.Rows[0]["RQ_SC"];
                    //        row["YXQZ"] = dtPH.Rows[0]["YXQZ"];
                    //    }
                    //}

                    row["DDSL"] = "0";
                    row["SSSL"] = "0";
                    row["HSJE"] = "0.00";
                    dtres.ImportRow(row);
                }
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
                        if (string.IsNullOrWhiteSpace(row["HSJJ"].ToString()) || row["HSJJ"].ToString() == "0")
                        {
                            msg += "【" + row["GDSEQ"].ToString() + " | " + row["GDNAME"].ToString() + "】,";
                            Alert.Show("商品" + msg + "【含税进价】为空，不能进行【商品入库管理】操作。", "消息提示", MessageBoxIcon.Warning);
                            continue;
                        }
                        //处理金额格式
                        decimal jingdu = 0;
                        decimal bzhl = 0;
                        if (decimal.TryParse(row["HSJJ"].ToString(), out jingdu) && decimal.TryParse(row["BZHL"].ToString(), out bzhl)) { row["HSJJ"] = Math.Round(jingdu * bzhl, 4).ToString("F4"); }
                        if (decimal.TryParse(row["YBJ"].ToString(), out jingdu)) { row["YBJ"] = jingdu.ToString("F4"); }
                        if (decimal.TryParse(row["HSJE"].ToString(), out jingdu)) { row["HSJE"] = Math.Round(jingdu, 2).ToString("F2"); }

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
                        //string someDjbh = string.Empty;放开限制，可以无限填加，cjl
                        //bool getDjbh = false;
                        //List<Dictionary<string, object>> newDict = GridCom.GetNewAddedList().OrderBy(x => x["GDSEQ"]).ToList();
                        //int sameRowCount = newDict.Where(a => a["GDSEQ"].ToString() == row["GDSEQ"].ToString()).Count();
                        //if (sameRowCount > 0)
                        //{
                        //    someDjbh += "【" + row["GDNAME"].ToString() + "】";
                        //    getDjbh = true;
                        //}

                        //if (getDjbh)
                        //{
                        //    Alert.Show("商品名称：" + someDjbh + "申请明细中已存在", "消息提示", MessageBoxIcon.Warning);
                        //}
                        //else
                        //{
                            PageContext.RegisterStartupScript(GridCom.GetAddNewRecordReference(defaultObj, true));

                        //}
                     
                    }
                 
        
                    }
                }
           
                docBILLNO.Enabled = false;
                docCGY.Enabled = false;
                docDDBH.Enabled = false;
                docPSSID.Enabled = false;
                docDEPTID.Enabled = false;
                if (!(BZHLerror == ""))
                    Alert.Show("商品" + BZHLerror + "包装含量异常，请维护后再选择！");
              
            
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

                // 处理返回jobject
                JObject defaultObj = Doc.GetJObject(GridCom, e.RowID);
                decimal hl = 0, rs = 0, jg = 0;
                decimal.TryParse((defaultObj["BZHL"] ?? "0").ToString(), out hl);
                decimal.TryParse((defaultObj["BZSL"] ?? "0").ToString(), out rs);
                decimal.TryParse((defaultObj["HSJJ"] ?? "0").ToString(), out jg);
                defaultObj["DDSL"] = rs * hl;
                defaultObj["HSJE"] = Math.Round(rs * jg, 2).ToString("F2");
                PageContext.RegisterStartupScript(GridCom.GetUpdateCellValueReference(e.RowID, defaultObj));

                //计算合计数量
                decimal ddslTotal = 0, bzslTotal = 0, feeTotal = 0;
                foreach (Dictionary<string, object> dic in newDict)
                {
                    ddslTotal += Convert.ToDecimal(dic["DDSL"] ?? "0");
                    bzslTotal += Convert.ToDecimal(dic["BZSL"] ?? "0");
                    feeTotal += Convert.ToDecimal(dic["HSJJ"] ?? "0") * Convert.ToDecimal(dic["BZSL"] ?? "0");
                }
                JObject summary = new JObject();
                summary.Add("GDNAME", "本页合计");
                summary.Add("DDSL", ddslTotal.ToString());
                summary.Add("BZSL", bzslTotal.ToString());
                summary.Add("HSJE", string.Format("{0:F2}", feeTotal));

                GridCom.SummaryData = summary;
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
            //if (!docBILLNO.Text.Contains(BillType) && docBILLNO.Text.Length > 0)
            //{
            //    Alert.Show("下传单据不能驳回！", "消息提示", MessageBoxIcon.Warning);
            //    return;
            //}

            if (docBILLNO.Text.Length < 1)
            { return; }

            if (string.IsNullOrWhiteSpace(ddlReject.SelectedValue))
            {
                Alert.Show("请选择驳回原因", "消息提示", MessageBoxIcon.Warning);
                return;
            }
          
            string strMemo = "驳回原因:" + ddlReject.SelectedText;
            if (!string.IsNullOrWhiteSpace(txaMemo.Text.Trim()))
            {
                strMemo += ";详细说明:" + txaMemo.Text;
            }
            if (strMemo.Length > 40)
            {
                Alert.Show("驳回备注超长！");
                return;

            }
            if (DbHelperOra.ExecuteSql(string.Format("UPDATE DAT_YRK_DOC SET FLAG='R',MEMO='{0}' WHERE SEQNO='{1}' AND FLAG='N'", strMemo, docBILLNO.Text)) == 1)
            {
                OperLog("跟台商品入库", "驳回单据【" + docBILLNO.Text + "】，" + strMemo);
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
            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_YRK_COM A,DOC_GOODS B WHERE A.GDSEQ = B.GDSEQ AND B.ISGZ = 'Y' AND SEQNO = '{0}'", docSEQNO.Text)))
            {
                Alert.Show("此单据中没有已经保存的高值商品,请检查！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (docFLAG.SelectedValue != "N")
            {
                zsmScan.Enabled = false;
                zsmDelete.Enabled = false;
            }
            else
            {
                zsmScan.Enabled = true;
                zsmDelete.Enabled = true;
            }
            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_YRK_EXT WHERE BILLNO = '{0}'", docSEQNO.Text)))
            {
                OracleParameter[] parameters = new OracleParameter[]
                {
                     new OracleParameter("V_BILL",OracleDbType.Varchar2),
                     new OracleParameter("V_TYPE",OracleDbType.Varchar2),

                };
                parameters[0].Value = docSEQNO.Text;
                parameters[1].Value = "";
                DbHelperOra.RunProcedure("P_YRKONECODE", parameters);
            }
            WindowScan.Hidden = false;
            ScanSearch("SHOW");
        }
        protected void ScanSearch(string type)
        {
            string sql = "";
            if (type == "SHOW")
            {
                sql = "SELECT A.*,f_getunitname(A.UNIT) UNITNAME FROM DAT_YRK_EXT A WHERE A.BILLNO = '{0}' ORDER BY DECODE(NVL(STR1,'#'),'#','1','2'),A.GDSEQ,A.INSTIME DESC";
            }
            else
            {
                sql = "SELECT A.*,f_getunitname(A.UNIT) UNITNAME FROM DAT_YRK_EXT A WHERE A.BILLNO = '{0}' ORDER BY DECODE(NVL(STR1,'#'),'#','1','2'),GDSEQ,A.INSTIME DESC";
            }
            DataTable dtScan = DbHelperOra.Query(string.Format(sql, docSEQNO.Text)).Tables[0];
            GridSacn.DataSource = dtScan;
            GridSacn.DataBind();
            if (dtScan.Rows.Count > 0)
                GridSacn.SelectedRowIndexArray = new int[] { 0 };
            zsmScan.Text = String.Empty;
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
                DataTable dtDoc = DbHelperOra.Query(string.Format("SELECT A.PSSID,A.DEPTID FROM DAT_DD_DOC A WHERE SEQNO ='{0}'", docDDBH.Text.Trim())).Tables[0];
                if (dtDoc != null && dtDoc.Rows.Count > 0)
                {
                    PubFunc.FormDataSet(FormDoc, dtDoc.Rows[0]);
                    PubFunc.FormLock(FormDoc, true, "");
                }
                else
                {
                    return;
                }

                PageContext.RegisterStartupScript(GridCom.GetRejectChangesReference());
                string sqlStr = string.Format(@"SELECT A.*,E.Supname,F_GETUNITNAME(A.UNIT) UNITNAME,F_GETHWID('{0}',A.GDSEQ) HWID,DHS SSSL,
                                        f_getproducername(A.PRODUCER) PRODUCERNAME,F_GETSUPPLIERNAME(A.SUPID)SUPNAME,F_GETUNITNAME(B.UNIT) UNITSMALLNAME 
                                        FROM DAT_DD_COM A, DOC_GOODS B, DAT_DD_DOC C,doc_goodssup D,doc_supplier E
                                        WHERE A.GDSEQ = B.GDSEQ and A.SEQNO=C.Seqno and b.gdseq=d.gdseq and d.supid=e.supid
                                        and d.ordersort='Y' AND A.SEQNO ='{1}'",
                    docDEPTID.SelectedValue, docDDBH.Text.Trim());
                DataTable dtBill = DbHelperOra.Query(sqlStr).Tables[0];
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
                                FROM DAT_YRK_DOC A, DAT_YRK_COM B
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
            if (lstPSSID.SelectedItem != null && lstPSSID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.PSSID='{0}'", lstPSSID.SelectedItem.Value);
            }
            if (lstDDBH.Text.Length > 0)
            {
                strSearch += string.Format(" AND A.DDBH LIKE '%{0}%'", lstDDBH.Text);
            }

          
            strSearch += string.Format(" AND A.LRRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.LRRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY A.BILLNO DESC,B.ROWNO";

            DataTable dt = DbHelperOra.Query(strSql).Tables[0];
            ExcelHelper.ExportByWeb(dt, "预入库信息导出", "预入库信息导出_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
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
            string onecode = (GridSacn.DataKeys[GridSacn.SelectedRowIndexArray[0]][2]).ToString();
            if (DbHelperOra.ExecuteSql(string.Format("UPDATE DAT_YRK_EXT SET STR1 = '' WHERE BILLNO = '{0}' AND STR1 = '{1}'", docSEQNO.Text, onecode)) < 1)
            {
                Alert.Show("删除清除条码失败,请检查！");
                return;
            }
            ScanSearch("");
            OperLog("跟台商品入库", "修改单据【" + docBILLNO.Text + "】高值码");

        }
        protected void zsmERP_TextChanged(object sender, EventArgs e)
        {
            if (Input.Checked && (sender ?? "").ToString() != "ENTER") return;
            string strBillno = docSEQNO.Text;
            // 背景色
            hdfScan.Text = "";
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
            if (GridSacn.SelectedRowIndexArray.Count() < 1)
            {
                Alert.Show("请选择需要扫描本位码的商品！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            if (Input.Checked && (sender ?? "").ToString() != "ENTER") return;
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("非『已提交』单据不允许操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (zsmScan.Text.Length < 1) return;
            if (DbHelperOra.Exists(String.Format("SELECT 1 FROM DAT_GZ_EXT WHERE ONECODE = '{0}' AND STR1 = '{0}'", zsmScan.Text.Trim())))
            {
                Alert.Show("您输入的本位码已被使用,请检查！", "提示信息", MessageBoxIcon.Warning);
                zsmScan.Text = string.Empty;
                zsmScan.Focus();
                return;
            }

            String exis = (DbHelperOra.GetSingle(String.Format("SELECT BILLNO FROM DAT_YRK_EXT WHERE STR1 = '{0}'", zsmScan.Text.Trim())) ?? "").ToString();
            if (!PubFunc.StrIsEmpty(exis))
            {
                Alert.Show("您输入的本位码 已被单据【" + exis + "】使用,请检查！", "提示信息", MessageBoxIcon.Warning);
                zsmScan.Text = string.Empty;
                zsmScan.Focus();
                return;
            }

            //写入数据库中
            DbHelperOra.ExecuteSql(string.Format(@"UPDATE DAT_YRK_EXT SET STR1 = '{0}'
                WHERE GDSEQ = '{1}' AND ROWNO = {2} AND BILLNO = '{3}'", zsmScan.Text.Trim(), GridSacn.DataKeys[GridSacn.SelectedRowIndexArray[0]][0], GridSacn.DataKeys[GridSacn.SelectedRowIndexArray[0]][1], docBILLNO.Text));
            ScanSearch("");
        }

        protected void GridCom_RowClick(object sender, GridRowClickEventArgs e)
        {
            //用+=选中多行的删除
            List<string> ListStr = ColorForGridGoods.Text.Split(',').ToList<string>();
            if (ListStr.Contains(e.RowIndex.ToString()))
            {
                ColorForGridGoods.Text = "";
                ListStr.Remove(e.RowIndex.ToString());
                for (int i = 0; i < ListStr.Count; i++)
                {
                    if (ListStr[i].Length > 0)
                    {
                        ColorForGridGoods.Text = ListStr[i] + ",";
                    }
                }
            }
            else
            {
                ColorForGridGoods.Text = e.RowIndex.ToString() + ",";
            }
            PageContext.RegisterStartupScript("highlightRowsForGoodsGrid();");
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
            }
        }

        protected void PageManager1_CustomEvent(object sender, CustomEventArgs e)
        {
            if (e.EventArgument.IndexOf("GoodsAdd") >= 0)
            {
                Window1_Close(null, null);
            }
            else if (e.EventArgument.IndexOf("AUDIT") >= 0)
            {
                string strBillno = docSEQNO.Text;
                string sql = string.Format("UPDATE DAT_YRK_DOC SET FLAG = 'Y', SHR='{0}', SHRQ=SYSDATE WHERE SEQNO='{1}' AND BILLTYPE = 'YRK'", UserAction.UserID, docSEQNO.Text);
                DbHelperOra.Query(sql);
                Alert.Show("单据【" + strBillno + "】审核成功！");
                OperLog("跟台商品入库", "审核单据【" + strBillno + "】");
                billOpen(strBillno);
            }
        }
        protected void tgbSUPNAME_TriggerClick(object sender, EventArgs e)
        {
            hfdrowID.Text = GridCom.SelectedRowID;
            if (hfdrowID.Text.Length < 1) return;
            String Sql = String.Format("SELECT SUPID,f_getsupname(SUPID) SUPNAME FROM DOC_GOODSSUP WHERE GDSEQ = '{0}'", Doc.GetGridInf(GridCom, hfdrowID.Text, "GDSEQ"));
            DataTable dt = DbHelperOra.Query(Sql).Tables[0];
            if (dt.Rows.Count < 2)
            {
                Alert.Show("只存在一个供应商，不需要选择！", MessageBoxIcon.Warning);
                return;
            }
            else
            {
                grdSup.DataSource = dt;
                grdSup.DataBind();
            }
            winSup.Hidden = false;
        }
        protected void btnSupSure_Click(object sender, EventArgs e)
        {
            JObject defaultObj = Doc.GetJObject(GridCom, hfdrowID.Text);
            if (grdSup.SelectedRowIndex < 0)
            {
                Alert.Show("请选择供应商！");
                return;
            }
            else
            {
                defaultObj["SUPNAME"] = grdSup.DataKeys[grdSup.SelectedRowIndex][1].ToString();
                defaultObj["SUPID"] = grdSup.DataKeys[grdSup.SelectedRowIndex][0].ToString();
                PageContext.RegisterStartupScript(GridCom.GetUpdateCellValueReference(hfdrowID.Text, defaultObj));
                winSup.Hidden = true;
            }
        }
        protected void grdSup_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            JObject defaultObj = Doc.GetJObject(GridCom, hfdrowID.Text);
            defaultObj["SUPNAME"] = grdSup.DataKeys[e.RowIndex][1].ToString();
            defaultObj["SUPID"] = grdSup.DataKeys[e.RowIndex][0].ToString();
            PageContext.RegisterStartupScript(GridCom.GetUpdateCellValueReference(hfdrowID.Text, defaultObj));
            winSup.Hidden = true;
        }

        protected void GridList_Sort(object sender, GridSortEventArgs e)
        {
            billSearch();
        }
    }
}