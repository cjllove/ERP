using FineUIPro;
using Newtonsoft.Json.Linq;
using XTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPProject.ERPApply
{
    public partial class BillChecking_SP : BillBase
    {
        private string strDocSql = "select billno,SEQNO, LRY,F_GETUSERNAME(LRY) LRYNAME,LRRQ,SHR,F_GETUSERNAME(SHR) SHRNAME,SHRQ,BILLTYPE,F_GET_BILLTYPENAME(BILLTYPE) BILLTYPENAME,F_GETDEPTNAME(DEPTID) DEPTIDNAME,DEPTOUT,F_GETDEPTNAME(DEPTOUT) DEPTOUTNAME,decode(FLAG,'Y','已审核','G','已执行','未结算') FLAGNAME,DECODE(XSTYPE,'1','申领','申退') XSTYPENAME,PRICE_HSJE,PRICE_RTN,PRICE from DAT_JSD_BILL where seqno='{0}' order by BILLNO,SHRQ desc";
        private string strLisSQL = "SELECT SEQNO,DEPTID,SUPID,BEGRQ,ENDRQ,LRY,FLAG,decode(FLAG,'Y','已审核','G','已付款','R','驳回','未结算') FLAGNAME,LRRQ,YJJJ FROM dat_jsd_doc WHERE SEQNO = '{0}' ORDER by DEPTID,SEQNO";
        public BillChecking_SP()
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
            PubFunc.DdlDataGet("DDL_USER", docLRY);
            PubFunc.DdlDataGet("DDL_BILL_STATUSJSD", docFLAG, ddlJSZT);
            PubFunc.DdlDataGet(ddlReject, "DDL_BILL_REASON_JSD");
            PubFunc.DdlDataGet("DDL_DOC_SHS", ddlSUPID);
            lstLRRQ1.SelectedDate = DateTime.Now;
            lstLRRQ2.SelectedDate = DateTime.Now;
            PubFunc.FormLock(FormDoc, true, "");
        }
        protected override void billClear()
        {
            PubFunc.FormDataClear(Formlist);
            lstLRRQ1.SelectedDate = DateTime.Now;
            lstLRRQ2.SelectedDate = DateTime.Now;
        }
        protected override void billSearch()
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【结算日期】！");
                return;
            }

            string strSql = @"select CUSTID,SEQNO,DEPTID,F_GETDEPTNAME(DEPTID) DEPTIDNAME,SUPID,SUPNAME,FLAG,decode(FLAG,'Y','已审核','G','已付款','R','驳回','未结算') FLAGNAME,GATFUNDCORP,GATFUNDBANK,GATACCNTNO,LINKMAN,LINKTEL,
                                CWLINKMAN,CWLINKTEL,BEGRQ,ENDRQ,SYJE,XSJE,THJE,JSJE,FPJE,FPHM,LRY,LRRQ,SPR,SPRQ,SHR,YJJJ,
                                SHRQ,MEMO,STR1,STR2,STR3,NUM1,NUM2,NUM3,UPTTIME     
                                from dat_jsd_doc where 1=1 ";
            string strSearch = "";
            if (lstBILLNO.Text.Trim().Length > 0)
            { strSearch += string.Format(" AND SEQNO  LIKE '%{0}%'", lstBILLNO.Text.Trim()); }
            if (ddlJSZT.SelectedValue.Length > 0)
            { strSearch += string.Format(" AND FLAG='{0}'", ddlJSZT.SelectedValue.ToString()); }
            strSearch += string.Format(" AND LRRQ between TO_DATE('{0}','YYYY-MM-DD') and TO_DATE('{1}','YYYY-MM-DD') + 1", lstLRRQ1.Text, lstLRRQ2.Text);
            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY DEPTID,ENDRQ DESC";
            DataTable dtBill = new DataTable();
            dtBill = DbHelperOra.Query(strSql).Tables[0];
            GridList.DataSource = dtBill;
            GridList.DataBind();
            //计算合计数量
            decimal ddslTotal = 0, bzslTotal = 0, feTotal = 0, feeTotal = 0;
            foreach (DataRow row in dtBill.Rows)
            {
                ddslTotal += Convert.ToDecimal(string.IsNullOrWhiteSpace(row["SYJE"].ToString()) ? "0" : row["SYJE"]);
                bzslTotal += Convert.ToDecimal(string.IsNullOrWhiteSpace(row["XSJE"].ToString()) ? "0" : row["XSJE"]);
                feeTotal += Convert.ToDecimal(string.IsNullOrWhiteSpace(row["THJE"].ToString()) ? "0" : row["THJE"]);
                feTotal += Convert.ToDecimal(string.IsNullOrWhiteSpace(row["YJJJ"].ToString()) ? "0" : row["YJJJ"]);
            }
            JObject summary = new JObject();
            summary.Add("FLAGNAME", "本页合计");
            summary.Add("SYJE", ddslTotal.ToString("F2"));
            summary.Add("XSJE", bzslTotal.ToString("F2"));
            summary.Add("THJE", feeTotal.ToString("F2"));
            summary.Add("YJJJ", feTotal.ToString("F2"));
            GridList.SummaryData = summary;

        }
        protected void btnRejectSubmit_Click(object sender, EventArgs e)
        {
            string strMemo = "驳回原因：" + ddlReject.SelectedText;
            if (!string.IsNullOrWhiteSpace(txaMemo.Text.Trim()))
            {
                strMemo += "；详细说明：" + txaMemo.Text;
            }
            if (strMemo.Length > 50)
            {
                Alert.Show("驳回备注超长！");
                return;

            }
            List<CommandInfo> cmdList = new List<CommandInfo>();
            cmdList.Add(new CommandInfo("update dat_jsd_doc set flag='R',UPTTIME =sysdate,SPRQ=sysdate,memo ='" + strMemo + "',SPR='" + UserAction.UserID + "' where seqno in (" + hid_bill.Text + ")", null));
            cmdList.Add(new CommandInfo("update dat_goodsjxc set str1='' where str1 in (" + hid_bill.Text + ")", null));

            DbHelperOra.ExecuteSqlTran(cmdList);
            WindowReject.Hidden = true;
            billSearch();
        }
        protected override void billCancel()
        {
            //将选中单据驳回
            string str_bill = "";
            int[] selections = GridList.SelectedRowIndexArray;
            bool first = true;
            foreach (int rowIndex in selections)
            {
                //检查单据是否需要审核
                if (DbHelperOra.Exists("select 1 from dat_jsd_doc where seqno = '" + GridList.DataKeys[rowIndex][0].ToString() + "' and flag ='N' "))
                {
                    if (first)
                    { str_bill = str_bill + "'" + GridList.DataKeys[rowIndex][0].ToString() + "'"; first = false; }
                    else
                    { str_bill = str_bill + ",'" + GridList.DataKeys[rowIndex][0].ToString() + "'"; }
                }
            }
            if (str_bill.Trim().Length < 1)
            {
                Alert.Show("您未选中数据或选中单据已被审核,请检查!");
                return;
            }
            hid_bill.Text = str_bill;
            WindowReject.Hidden = false;
        }
        protected override void billAudit()
        {
            //将选中单据审核
            string str_bill = "";
            string str_bill2 = "";
            string str_dept = "";
            int[] selections = GridList.SelectedRowIndexArray;
            foreach (int rowIndex in selections)
            {
                //检查单据是否需要审核
                if (DbHelperOra.Exists("select 1 from dat_jsd_doc where seqno = '" + GridList.DataKeys[rowIndex][0].ToString() + "' and flag ='N' "))
                {
                    str_bill += "'" + GridList.DataKeys[rowIndex][0].ToString() + "'" + ",";
                    str_bill2 += GridList.DataKeys[rowIndex][0].ToString() + ",";
                    //特殊结算单不更改科室结算日期
                    if (DbHelperOra.Exists("SELECT 1 FROM DAT_JSD_BILL WHERE SEQNO = '" + GridList.DataKeys[rowIndex][0].ToString() + "' AND MEMO like '特殊结算单'"))
                    {
                        str_dept += "'" + GridList.DataKeys[rowIndex][1].ToString() + "'" + ",";
                    }
                }
            }
            if (str_bill.Trim().Length < 1)
            {
                Alert.Show("您未选中数据或选中单据已被审核,请检查!");
                return;
            }
            List<CommandInfo> cmdList = new List<CommandInfo>();
            cmdList.Add(new CommandInfo("update dat_jsd_doc set flag='Y',UPTTIME =sysdate,SPRQ=sysdate,SPR='" + UserAction.UserID + "' where seqno in (" + str_bill.TrimEnd(',') + ")", null));
            //object objmode = DbHelperOra.GetSingle("select VALUE from sys_para where CODE = 'JSMODE'");
            //string jsmode = (objmode ?? "").ToString();
            //if (PubFunc.StrIsEmpty(jsmode)) { Alert.Show("未定义结算方式,请联系系统管理员!"); return; }
            //if (jsmode == "CKD")
            //    cmdList.Add(new CommandInfo("update DAT_CK_DOC set flag = 'G' where STR3 in (" + str_bill.TrimEnd(',') + ")", null));
            //if (jsmode == "XSD")
            //    cmdList.Add(new CommandInfo("update DAT_XS_DOC set flag = 'G' where STR3 in (" + str_bill.TrimEnd(',') + ")", null));
            //if (jsmode == "RKD")
            //{
            //    cmdList.Add(new CommandInfo("update DAT_RK_DOC set flag = 'G' where STR3 in (" + str_bill.TrimEnd(',') + ")", null));
            //    cmdList.Add(new CommandInfo("update DAT_TH_DOC set flag = 'G' where STR3 in (" + str_bill.TrimEnd(',') + ")", null));
            //}
            if (PubFunc.StrIsEmpty(str_dept))
            {
                cmdList.Add(new CommandInfo("UPDATE SYS_DEPT A SET STR2 = (SELECT TO_CHAR(MAX(B.ENDRQ),'YYYY-MM-DD') FROM dat_jsd_doc B,DAT_JSD_BILL C WHERE B.SEQNO = C.SEQNO AND C.DEPTID = A.CODE AND B.seqno in (" + str_bill.TrimEnd(',') + ")) WHERE EXISTS(SELECT 1 FROM dat_jsd_doc B,DAT_JSD_BILL C WHERE B.SEQNO = C.SEQNO AND C.DEPTID = A.CODE AND B.seqno in (" + str_bill.TrimEnd(',') + "))", null));
            }
            else
            {
                cmdList.Add(new CommandInfo("UPDATE SYS_DEPT A SET STR2 = (SELECT TO_CHAR(MAX(B.ENDRQ),'YYYY-MM-DD') FROM DAT_JSD_DOC B,DAT_JSD_BILL C WHERE B.SEQNO = C.SEQNO AND C.DEPTID = A.CODE AND B.seqno in (" + str_bill.TrimEnd(',') + ")) WHERE EXISTS(SELECT 1 FROM DAT_JSD_DOC B,DAT_JSD_BILL C WHERE B.SEQNO = C.SEQNO AND C.DEPTID = A.CODE AND B.seqno in (" + str_bill.TrimEnd(',') + ")) AND A.CODE NOT IN(" + str_dept.Trim(',') + ")", null));
            }
            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                Alert.Show("单据【" + str_bill.TrimEnd(',') + "】审核成功!");
                OperLog("结算单管理", "审核单据【" + str_bill2.TrimEnd(',') + "】");
                billSearch();
            }
            else
            {
                Alert.Show("因系统调度原因审核失败,请联系管理员！");
            }
        }

        protected override void listRow_DoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            //ymh系统偶尔报错未找到原因，暂时拦截操作

            billOpen(GridList.DataKeys[e.RowIndex][0].ToString());
        }

        protected override void billOpen(string SEQNO)
        {
            //子页中表头赋值
            DataTable dtDoc = DbHelperOra.Query(string.Format(strLisSQL, SEQNO)).Tables[0];
            PubFunc.FormDataSet(FormDoc, dtDoc.Rows[0]);
            //表体赋值
            DataTable dtBill = DbHelperOra.Query(string.Format(strDocSql, SEQNO)).Tables[0];
            GridGoods.DataSource = dtBill;
            GridGoods.DataBind();

            string flag = dtDoc.Rows[0]["FLAG"].ToString();

            if (flag.Equals("N"))
            {
                lisAudi.Enabled = true;
                lisBH.Enabled = true;
            }
            else
            {
                lisAudi.Enabled = false;
                lisBH.Enabled = false;
            }

            TabStrip1.ActiveTabIndex = 1;
        }
        protected void GridLis_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            string billno = GridGoods.DataKeys[e.RowIndex][0].ToString();
            string type = GridGoods.DataKeys[e.RowIndex][1].ToString();
            string url = "";
            if (type == "CKD" || type == "DSC" || type == "DST" || type == "LCD" || type == "LTD")
            {
                url = "~/ERPPayment/Doc_CK_ComWindow.aspx?bm=" + billno + "&cx=&su=" + ddlSUPID.SelectedValue;
                PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "出库信息:单号【" + billno + "】"));
            }
            if (type == "XSD" || type == "XST")
            {
                url = "~/ERPPayment/Doc_XS_ComWindow.aspx?bm=" + billno + "&cx=&su=" + ddlSUPID.SelectedValue;
                PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "销售信息:单号【" + billno + "】"));
            }
            if (type == "RKD")
            {
                url = "~/ERPPayment/Doc_RK_ComWindow.aspx?bm=" + billno + "&cx=&su=" + ddlSUPID.SelectedValue;
                PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "入库信息:单号【" + billno + "】"));
            }
            if (type == "KSD")
            {
                url = "~/ERPPayment/Doc_DB_ComWindow.aspx?bm=" + billno + "&cx=&su=" + ddlSUPID.SelectedValue;
                PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "入库信息:单号【" + billno + "】"));
            }
        }

        protected void lisAudi_Click(object sender, EventArgs e)
        {
            if (docSEQNO.Text.Length < 1) return;
            object flag = DbHelperOra.GetSingle("SELECT FLAG FROM dat_jsd_doc WHERE SEQNO = '" + docSEQNO.Text + "'");
            if ((flag ?? "").ToString() != "N")
            {
                Alert.Show("非未结算单据,不能审核！");
                return;
            }
            List<CommandInfo> cmdList = new List<CommandInfo>();
            cmdList.Add(new CommandInfo("update dat_jsd_doc set flag='Y',UPTTIME =sysdate,SPRQ=sysdate,SPR='" + UserAction.UserID + "' where seqno ='" + docSEQNO.Text + "'", null));
            //object objmode = DbHelperOra.GetSingle("select VALUE from sys_para where CODE = 'JSMODE'");
            //string jsmode = (objmode ?? "").ToString();
            //if (jsmode == "CKD")
            //    cmdList.Add(new CommandInfo("update DAT_CK_DOC set flag = 'G' where STR3 ='" + docSEQNO.Text + "'", null));
            //if (jsmode == "XSD")
            //    cmdList.Add(new CommandInfo("update DAT_XS_DOC set flag = 'G' where STR3 ='" + docSEQNO.Text + "'", null));
            //if (jsmode == "RKD")
            //{
            //    cmdList.Add(new CommandInfo("update DAT_RK_DOC set flag = 'G' where STR3 ='" + docSEQNO.Text + "'", null));
            //    cmdList.Add(new CommandInfo("update DAT_TH_DOC set flag = 'G' where STR3 ='" + docSEQNO.Text + "'", null));
            //}
            //特殊结算单不更改科室结算日期
            if (!DbHelperOra.Exists("SELECT 1 FROM DAT_JSD_BILL WHERE SEQNO = '" + docSEQNO.Text + "' AND MEMO like '特殊结算单'"))
            {
                cmdList.Add(new CommandInfo("UPDATE SYS_DEPT A SET STR2 = (SELECT TO_CHAR(B.ENDRQ,'YYYY-MM-DD') FROM dat_jsd_doc B WHERE B.DEPTID = A.CODE AND seqno = '" + docSEQNO.Text + "') WHERE EXISTS(SELECT 1 FROM dat_jsd_doc B WHERE B.DEPTID = A.CODE AND seqno ='" + docSEQNO.Text + "')", null));
            }
            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                Alert.Show("单据【" + docSEQNO.Text + "】审核成功!");
                OperLog("结算单据", "审核单据【" + docSEQNO.Text + "】");
                billOpen(docSEQNO.Text);
            }
            else
            {
                Alert.Show("因系统调度原因审核失败,请联系管理员！");
            }
        }

        protected void lisBH_Click(object sender, EventArgs e)
        {
            //将选中单据驳回
            if (docSEQNO.Text.Length < 1) return;
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("非新单据不能驳回！");
                return;
            }
            hid_bill.Text = "'" + docSEQNO.Text + "'";
            WindowReject.Hidden = false;
            OperLog("结算单据", "驳回单据【" + docSEQNO.Text + "】");
        }

        protected void GridList_Sort(object sender, GridSortEventArgs e)
        {
            GridList.SortDirection = e.SortDirection;
            GridList.SortField = e.SortField;

            DataView view1 = PubFunc.GridDataGet(GridList).DefaultView;
            view1.Sort = String.Format("{0} {1}", GridList.SortField, GridList.SortDirection);
            GridList.DataSource = view1;
            GridList.DataBind();
        }
    }
}